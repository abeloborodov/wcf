using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Xml.Serialization;

namespace WCF.XmlDSig.Service.Contracts
{
    [MessageContract(WrapperNamespace = "urn://x-artefacts-smev-gov-ru/services/message-exchange/types/1.1")]
    public class Request
    {
        [MessageBodyMember(Namespace = "urn://x-artefacts-smev-gov-ru/services/message-exchange/types/1.1", Order = 0)]
        public SenderProvidedRequestData SenderProvidedRequestData { get; set; }

        [MessageBodyMember(Namespace = "urn://x-artefacts-smev-gov-ru/services/message-exchange/types/1.1", Order = 1)]
        public System.Xml.XmlElement DepartmentSignature { get; set; }
    }

    public class SenderProvidedRequestData
    {
        [XmlElement(Namespace = "urn://x-artefacts-smev-gov-ru/services/message-exchange/types/basic/1.1", Order = 0)]
        public PrimaryContent PrimaryContent { get; set; }

        [XmlElement(Order = 1)]
        public BusinessProcessMetadata BusinessProcessMetadata { get; set; }

        [XmlElement(Order = 2)]
        public System.Xml.XmlElement PersonalSignature { get; set; }

        [XmlAttribute(DataType = "ID")]
        public string Id { get; set; }
    }

    public class PrimaryContent
    {
        public string SignificantData { get; set; }

        [XmlAttribute(DataType = "ID")]
        public string Id { get; set; }
    }

    public class BusinessProcessMetadata
    {
        [XmlElement(Order = 0)]
        public string ServiceCode { get; set; }
        [XmlElement(Order = 1)]
        public string CaseNumber { get; set; }
    }


    [ServiceContract]
    [XmlSerializerFormat]
    public interface ITestService
    {
        [OperationContract]
        void SendRequest(Request message);
    }

    public sealed class TestService : ITestService
    {
        public void SendRequest(Request message)
        {
            Console.WriteLine("Accept");
        }
    }
}