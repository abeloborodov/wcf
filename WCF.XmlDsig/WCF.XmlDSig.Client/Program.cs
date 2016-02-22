using System;
using System.ServiceModel;
using System.Xml;
using WCF.XmlDSig.Client.TestClient;

namespace WCF.XmlDSig.Client
{
    class Program
    {
        static void Main(string[] args)
        {
            var binding = new BasicHttpBinding { MessageEncoding = WSMessageEncoding.Text, TransferMode = TransferMode.Buffered };
            var client = new TestClient.TestServiceClient(binding,
                new EndpointAddress("http://abeloborodov-pc:8731/TestService/"));
            client.Endpoint.Behaviors.Add(new ClientBehavior());
            try
            {
                client.SendRequest(
                    new SenderProvidedRequestData
                    {
                        BusinessProcessMetadata =
                            new BusinessProcessMetadata { CaseNumber = "123", ServiceCode = "somecode" },
                        Id = Guid.NewGuid().ToString("N"),
                        PrimaryContent = new PrimaryContent
                        {
                            SignificantData = "some significant data",
                            Id = Guid.NewGuid().ToString("N") 
                        }
                    }, null);
            }
            catch (FaultException e)
            {
                Console.WriteLine(e.Reason);
            }
            Console.ReadLine();
        }
    }
}
