using System;
using System.Diagnostics;
using System.IO;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace WCF.BinaryData.MTOM.Client.TestService.Proxy
{
    public partial class AttachmentContent : IXmlSerializable
    {
        private readonly FileStream _streamedData;
        public AttachmentContent(FileStream streamedData, Guid id)
        {
            _streamedData = streamedData;
            IdField = id.ToString("D");
        }
        public XmlSchema GetSchema()
        {
            return null;
        }
        public void ReadXml(XmlReader reader)
        {
            throw new System.NotImplementedException();
        }
        public void WriteXml(XmlWriter writer)
        {
            writer.WriteStartElement("Id");
            writer.WriteValue(Id);
            writer.WriteEndElement();

            writer.WriteStartElement("ContentData");
            if (writer is IXmlMtomWriterInitializer)
            {
                writer.WriteValue(new CustomStreamProvider(_streamedData));
            }
            else
            {
                using (_streamedData)
                {
                    var buffer = new byte[10000];
                    int bytesRead = _streamedData.Read(buffer, 0, buffer.Length);
                    while (bytesRead > 0)
                    {
                        writer.WriteBase64(buffer, 0, bytesRead);
                        bytesRead = _streamedData.Read(buffer, 0, buffer.Length);
                    }
                }
            }
            writer.WriteEndElement();
        }

        class CustomStreamProvider : IStreamProvider
        {
            readonly Stream _stream;
            public CustomStreamProvider(Stream stream)
            {
                _stream = stream;
            }
            public Stream GetStream()
            {
                return _stream;
            }
            public void ReleaseStream(Stream stream)
            {
                if (stream != null)
                    stream.Dispose();
            }
        }
    }
}