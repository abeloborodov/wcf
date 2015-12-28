using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace WCF.BinaryData.MTOM
{
    class Program
    {
        static void Main(string[] args)
        {
            Host();
        }

        public static void Host()
        {
            using (var host = new ServiceHost(typeof(TestService), new Uri("http://abeloborodov-pc:8731/TestService/")))
            {
                var metadataBehavior = host.Description.Behaviors.Find<ServiceMetadataBehavior>();
                metadataBehavior = new ServiceMetadataBehavior();
                metadataBehavior.HttpGetEnabled = true;
                metadataBehavior.MetadataExporter.PolicyVersion = PolicyVersion.Policy15;
                host.Description.Behaviors.Add(metadataBehavior);
                host.AddServiceEndpoint(
                    ServiceMetadataBehavior.MexContractName,
                    MetadataExchangeBindings.CreateMexHttpBinding(),
                    "mex");

                host.AddServiceEndpoint(typeof(ITestService),
                    new BasicHttpBinding { MessageEncoding = WSMessageEncoding.Mtom, MaxReceivedMessageSize = int.MaxValue }, "");

                host.Open();
                Console.WriteLine("service started at {0}", host.BaseAddresses[0]);
                Console.ReadLine();
            }
        }
    }

    [ServiceContract]
    [XmlSerializerFormat]
    public interface ITestService
    {
        [OperationContract]
        void SendRequest(RequestWithAttachments message);
    }

    public class TestService : ITestService
    {
        public void SendRequest(RequestWithAttachments message)
        {
            Console.WriteLine("Ok.");
        }
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
