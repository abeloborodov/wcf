using System;
using System.IO;
using System.ServiceModel.Channels;
using System.Xml;

namespace WCF.BinaryData.MTOM.Client
{
    public class CustomBodyWriter : BodyWriter
    {
        private Guid _messageId;
        private Guid _contentId;
        private FileStream _fileStream;

        public CustomBodyWriter(Guid messageId, Guid contentId, FileStream fileStream)
            : base(false)
        {
            _messageId = messageId;
            _contentId = contentId;
            _fileStream = fileStream;
        }

        protected override void OnWriteBodyContents(XmlDictionaryWriter writer)
        {
            writer.WriteStartElement("RequestWithAttachments", "http://tempuri.org/");

            writer.WriteStartElement("MessageID");
            writer.WriteString(_messageId.ToString("D"));
            writer.WriteFullEndElement();

            writer.WriteStartElement("AttachmentContentList");
            writer.WriteStartElement("AttachmentContent");

            writer.WriteStartElement("Id");
            writer.WriteString(_contentId.ToString("D"));
            writer.WriteFullEndElement();

            writer.WriteStartElement("ContentData");
            var buffer = new byte[10000];
            int bytesReadCount = 0;
            using (_fileStream)
            {
                do
                {
                    bytesReadCount = _fileStream.Read(buffer, 0, buffer.Length);
                    if (bytesReadCount > 0)
                    {
                        writer.WriteBase64(buffer, 0, bytesReadCount);
                    }
                } while (bytesReadCount > 0);
            }
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();
            writer.WriteFullEndElement();

            writer.WriteFullEndElement();
        }

        public Message CreateNewMessage()
        {
            return Message.CreateMessage(MessageVersion.Soap11, "http://tempuri.org/ITestService/SendRequest", this);
        }
    }
}