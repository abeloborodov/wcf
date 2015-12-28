using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading.Tasks;
using WCF.BinaryData.MTOM.Client.TestService.Proxy;

namespace WCF.BinaryData.MTOM.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            SendWithProxy();
            //SendWithMessageDirectly();
            //SimpleSend();
        }

        //static void SimpleSend()
        //{
        //    using (var factory = new ChannelFactory<ITestService>(
        //        new BasicHttpBinding { MessageEncoding = WSMessageEncoding.Mtom, TransferMode = TransferMode.StreamedRequest}, "http://abeloborodov-pc:8731/TestService/"))
        //    {
        //        ITestService client = factory.CreateChannel();

        //        var randomData = new byte[10240];
        //        new Random().NextBytes(randomData);

        //        client.SendRequest(new RequestWithAttachments
        //        {
        //            MessageID = Guid.NewGuid().ToString("D"),
        //            AttachmentContentList = new List<AttachmentContent>
        //                {
        //                    new AttachmentContent
        //                    {
        //                        ContentData = randomData,
        //                        Id = Guid.NewGuid().ToString("D"),
        //                    }
        //                }
        //        });
        //    }
        //}

        static void SendWithMessageDirectly()
        {
            using (var factory = new ChannelFactory<ITestService>(
                new BasicHttpBinding
                {
                    MessageEncoding = WSMessageEncoding.Text,
                    TransferMode = TransferMode.StreamedRequest,
                },
                "http://abeloborodov-pc:8731/TestService/"))
            {
                ITestService client = factory.CreateChannel();
                client.SendRequest(new CustomBodyWriter(Guid.NewGuid(), Guid.NewGuid(), File.OpenRead(@"test.jpg")).CreateNewMessage());
            }
        }

        static void SendWithProxy()
        {
            var binding = new BasicHttpBinding { MessageEncoding = WSMessageEncoding.Mtom, TransferMode = TransferMode.StreamedRequest };
            var client = new TestService.Proxy.TestServiceClient(binding, new EndpointAddress("http://abeloborodov-pc:8731/TestService/"));
            TestService.Proxy.AttachmentContent[] attachments = { new TestService.Proxy.AttachmentContent(File.OpenRead(@"test.jpg"), Guid.NewGuid()) };
            client.SendRequest(Guid.NewGuid().ToString("D"), attachments);
        }
    }

    [ServiceContract]
    [XmlSerializerFormat]
    public interface ITestService
    {
        //[OperationContract]
        //void SendRequest(RequestWithAttachments message);

        [OperationContract]
        void SendRequest(Message message);
    }

    [MessageContract]
    public class RequestWithAttachments
    {
        [MessageBodyMember(Order = 0)]
        public string MessageID { get; set; }

        [MessageBodyMember(Order = 1)]
        public List<AttachmentContent> AttachmentContentList { get; set; }
    }

    public class AttachmentContent
    {
        public string Id { get; set; }
        public byte[] ContentData { get; set; }
    }

}
