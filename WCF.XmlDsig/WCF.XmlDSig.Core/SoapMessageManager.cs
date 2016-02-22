using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.ServiceModel.Channels;
using System.Xml;

namespace WCF.XmlDSig.Core
{
    public class SoapMessageManager
    {
        private readonly XmlNamespaceManager _ns;

        private void BindNamespaces()
        {
            var fields = typeof(SoapPrefix).GetFields();

            foreach (FieldInfo fieldInfo in fields)
            {
                var customAttr = (CustomAttribute)Attribute.GetCustomAttribute(fieldInfo, typeof(CustomAttribute));
                if (customAttr != null)
                {
                    _ns.AddNamespace(fieldInfo.Name, customAttr.SoapNamespace);
                }
            }
        }
        private readonly XmlDocument _document;
        private readonly MessageProperties _properties;
        private readonly MessageVersion _version;


        public Message DefaceSign(SoapNode nodeToDeface)
        {
            XmlElement personal = GetInnerNode(nodeToDeface);
            personal.ChildNodes.OfType<XmlElement>().Last().InnerText = "fake";
            return ToWcfMessage();
        }

        public SoapMessageManager(Message message)
        {
            _properties = message.Properties;
            _version = message.Version;
            using (var ms = new MemoryStream())
            {
                XmlWriter writer = XmlWriter.Create(ms);
                message.WriteMessage(writer);
                writer.Flush();
                ms.Position = 0;
                _document = new XmlDocument { PreserveWhitespace = true };
                _document.Load(ms);
            }
            _ns = new XmlNamespaceManager(_document.NameTable);
            BindNamespaces();
        }

        public static implicit operator Message(SoapMessageManager message) 
        {
            return message.ToWcfMessage(); 
        }

        public void SignElement(XmlElement element, XmlElement signatureOwnerElement, X509Certificate2 certificate)
        {
            try
            {
                // Получаем подпись
                var mySignedXml = new SignedXml(_document) { SigningKey = certificate.PrivateKey };
                var reference = new Reference
                {
                    Uri = "#" + element.Attributes["Id"].InnerText,
                    DigestMethod = "http://www.w3.org/2000/09/xmldsig#sha1"
                };

                var excC14Ntransform = new XmlDsigExcC14NTransform();
                reference.AddTransform(excC14Ntransform);
                //reference.AddTransform(CreateCustomSmev3Transform());

                mySignedXml.AddReference(reference);
                var keyInfo = new KeyInfo();
                keyInfo.AddClause(new KeyInfoX509Data(certificate));
                mySignedXml.KeyInfo = keyInfo;
                mySignedXml.SignedInfo.CanonicalizationMethod = excC14Ntransform.Algorithm;
                mySignedXml.SignedInfo.SignatureMethod = "http://www.w3.org/2000/09/xmldsig#rsa-sha1";
                mySignedXml.ComputeSignature();

                // Вставляем подпись в сообщение
                InsertSign(mySignedXml, signatureOwnerElement);
            }
            catch (Exception exception)
            {
                Console.WriteLine("Unhandled exception during adding signature. " + exception.Message);
            }
        }

        
        private bool CheckSign(SoapNode nodeIndication)
        {
            var node = GetInnerNode(nodeIndication);
            if (node == null)
                return true;
            // Создаем объект SignedXml для проверки подписи документа.
            var signedXml = new SignedXml(_document);

            var signatureNode =
                (XmlElement)
                    node.GetElementsByTagName(SoapNode.Signature.ToString(),
                        SoapNode.Signature.GetAttribute_NodePrefix().GetAttribute_SoapNamespace())[0];
            var keyInfoNode = (XmlElement)signatureNode.GetElementsByTagName("KeyInfo", SignedXml.XmlDsigNamespaceUrl)[0];
            var x509DataNode = (XmlElement)keyInfoNode.GetElementsByTagName("X509Data", SignedXml.XmlDsigNamespaceUrl)[0];
            var x509CertificateNode = (XmlElement)x509DataNode.GetElementsByTagName("X509Certificate", SignedXml.XmlDsigNamespaceUrl)[0];
            byte[] cert = Convert.FromBase64String(x509CertificateNode.InnerText);
            var c = new X509Certificate2(cert);

            signedXml.LoadXml(signatureNode);

            // Проверяем подпись
            return signedXml.CheckSignature(c, true);
        }

        public bool CheckSign(out SoapNode failedNode)
        {
            failedNode = SoapNode.DepartmentSignature;
            if (!CheckSign(failedNode))
                return false;
            failedNode = SoapNode.PersonalSignature;
            if (!CheckSign(failedNode))
                return false;
            return true;
        }

        /// <summary>
        /// Вставка подписи в сообщение
        /// </summary>
        /// <param name="mySignedXml">подписанное сообщение</param>
        /// <param name="insertIntoNode">элемент в который будет вставлена подпись</param>
        public void InsertSign(SignedXml mySignedXml, XmlElement insertIntoNode)
        {
            XmlNode signatureNode = _document.CreateNode(XmlNodeType.Element, SoapPrefix.ds.ToString(), "Signature", SoapPrefix.ds.GetAttribute_SoapNamespace());
            insertIntoNode.AppendChild(signatureNode);

            //var signedInfoNode = mySignedXml.GetXml(SoapPrefix.ds.ToString()).SelectSingleNode("/" + SoapPrefix.ds + ":SignedInfo", _ns);
            var signedInfoNode = mySignedXml.GetXml().SelectSingleNode("/" + SoapPrefix.ds + ":SignedInfo", _ns);

            XmlNode importedSignInfoNode = _document.ImportNode(signedInfoNode, true);
            signatureNode.AppendChild(importedSignInfoNode);

            //var signatureValueNode = mySignedXml.GetXml(SoapPrefix.ds.ToString()).SelectSingleNode("/" + SoapPrefix.ds + ":SignatureValue", _ns);
            var signatureValueNode = mySignedXml.GetXml().SelectSingleNode("/" + SoapPrefix.ds + ":SignatureValue", _ns);


            XmlNode importedSignatureValueNode = _document.ImportNode(signatureValueNode, true);
            signatureNode.AppendChild(importedSignatureValueNode);

            XmlNode keyInfoNode = _document.CreateNode(XmlNodeType.Element, SoapPrefix.ds.ToString(), "KeyInfo", SoapPrefix.ds.GetAttribute_SoapNamespace());
            signatureNode.AppendChild(keyInfoNode);

            var x509CertificateNodeSource = mySignedXml.GetXml().SelectSingleNode(string.Format("/{0}:KeyInfo/{0}:X509Data/{0}:X509Certificate", SoapPrefix.ds), _ns);

            XmlNode x509DataNode = _document.CreateNode(XmlNodeType.Element, SoapPrefix.ds.ToString(), "X509Data", SoapPrefix.ds.GetAttribute_SoapNamespace());
            XmlNode x509CertificateNode = _document.CreateNode(XmlNodeType.Element, SoapPrefix.ds.ToString(), "X509Certificate", SoapPrefix.ds.GetAttribute_SoapNamespace());
            x509DataNode.AppendChild(x509CertificateNode);
            x509CertificateNode.InnerText = x509CertificateNodeSource.InnerText;

            keyInfoNode.AppendChild(x509DataNode);
        }

        public XmlElement Root
        {
            get
            {
                return _document.DocumentElement.SelectSingleNode(SoapPrefix.s + ":Body", _ns).ChildNodes.OfType<XmlElement>().Single();
            }
        }

        public Message SignMessage()
        {
            var store = new X509Store(StoreName.My, StoreLocation.LocalMachine);
            store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certCollection = store.Certificates.Find(X509FindType.FindByThumbprint, ConfigurationManager.AppSettings["ClientThumbprint"], false);
            
            var senderDatasNode = GetInnerNode(SoapNode.SenderProvidedRequestData);
            var personalSignatureNode = _document.CreateElement(
                SoapNode.PersonalSignature.ToString(),
                SoapNode.PersonalSignature.GetAttribute_NodePrefix().GetAttribute_SoapNamespace());
            senderDatasNode.AppendChild(personalSignatureNode);

            var callerInformationSystemSignatureNode = _document.CreateElement(
                SoapNode.DepartmentSignature.ToString(),
                SoapNode.DepartmentSignature.GetAttribute_NodePrefix().GetAttribute_SoapNamespace());
            Root.AppendChild(callerInformationSystemSignatureNode);

            SignElement(GetInnerNode(SoapNode.PrimaryContent), GetInnerNode(SoapNode.PersonalSignature), certCollection[0]);
            SignElement(senderDatasNode, GetInnerNode(SoapNode.DepartmentSignature), certCollection[0]);

            return ToWcfMessage();
        }

        private Message ToWcfMessage()
        {
            var stream = new MemoryStream();
            _document.Save(stream);
            stream.Position = 0;
            XmlReader reader = XmlReader.Create(stream);
            Message messageWrapper = Message.CreateMessage(reader, int.MaxValue, _version);
            messageWrapper.Properties.CopyProperties(_properties);
            return messageWrapper;
        }

        /// <summary>
        /// Возвращает элемент любой вложенности или null если такого элемента не найдено
        /// </summary>
        /// <param name="nodeName">локально имя ноды</param>
        /// <returns></returns>
        private XmlElement GetInnerNode(SoapNode nodeName)
        {
            SoapPrefix prefix = nodeName.GetAttribute_NodePrefix();
            XmlNode element = _document.SelectSingleNode(string.Format("//{0}:{1}", prefix, nodeName), _ns);
            return (XmlElement)element;
        }
    }

    /// <summary>
    /// Это все возможные в ноды в соап сообщениях по которым клиентский код может искать или наполнять
    /// </summary>
    /// <remarks>
    /// можно избегать ситуаций когда есть две ноды с одним названием но разными пространствами имен. 
    /// Например можно все наши кастомные ноды начинать с уникального префикса Mcx
    /// В случае острой необходимости можно вынести имя ноды в <see cref="CustomAttribute"/>
    /// клас инкапсулирует сопоставление нод с пространствами имен из <see cref="SoapPrefix"/>
    /// </remarks>
    public enum SoapNode
    {
        [Custom(NodePrefix = SoapPrefix.types)]
        RequestMessage = 0,

        [Custom(NodePrefix = SoapPrefix.types)]
        DepartmentSignature = 1,

        [Custom(NodePrefix = SoapPrefix.types)]
        SenderProvidedRequestData = 2,

        [Custom(NodePrefix = SoapPrefix.types)]
        SenderProvidedResponseData = 3,

        [Custom(NodePrefix = SoapPrefix.basic)]
        AttachmentContentList = 4,

        [Custom(NodePrefix = SoapPrefix.basic)]
        PrimaryContent = 5,

        [Custom(NodePrefix = SoapPrefix.types)]
        Request = 6,

        [Custom(NodePrefix = SoapPrefix.types)]
        Response = 7,

        [Custom(NodePrefix = SoapPrefix.types)]
        SenderInformationSystemSignature = 8,

        [Custom(NodePrefix = SoapPrefix.types)]
        ResponseMessage = 9,

        [Custom(NodePrefix = SoapPrefix.types)]
        To,

        [Custom(NodePrefix = SoapPrefix.types)]
        ReplyTo,

        [Custom(NodePrefix = SoapPrefix.wsse)]
        Security,

        [Custom(NodePrefix = SoapPrefix.basic)]
        SignaturePKCS7,

        [Custom(NodePrefix = SoapPrefix.basic)]
        Content,

        [Custom(NodePrefix = SoapPrefix.types)]
        SMEVSignature,

        [Custom(NodePrefix = SoapPrefix.wsse)]
        BinarySecurityToken,

        [Custom(NodePrefix = SoapPrefix.types)]
        PersonalSignature,

        [Custom(NodePrefix = SoapPrefix.ds)]
        Signature,

        [Custom(NodePrefix = SoapPrefix.types)]
        TestMessage,

        [Custom(NodePrefix = SoapPrefix.types)]
        AckRequest,

        [Custom(NodePrefix = SoapPrefix.basic)]
        Timestamp,

        [Custom(NodePrefix = SoapPrefix.basic)]
        MessageTypeSelector,

        [Custom(NodePrefix = SoapPrefix.types)]
        RequestRejected,

        [Custom(NodePrefix = SoapPrefix.basic)]
        AckTargetMessage,
        [Custom(NodePrefix = SoapPrefix.types)]
        RejectionReasonCode,
        [Custom(NodePrefix = SoapPrefix.types)]
        RejectionReason,

        [Custom(NodePrefix = SoapPrefix.types)]
        BusinessProcessMetadata
    }
}