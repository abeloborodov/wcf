using System;
using System.Collections.Generic;
using System.IO;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using WCF.XmlDSig.Service.Contracts;

namespace WCF.XmlDSig.Service
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

                var endpoint = host.AddServiceEndpoint(typeof(ITestService), new BasicHttpBinding(), "");
                endpoint.Behaviors.Add(new ServerBehavior());

                var debug = host.Description.Behaviors.Find<ServiceDebugBehavior>();
                if (debug == null)
                {
                    host.Description.Behaviors.Add(
                         new ServiceDebugBehavior { IncludeExceptionDetailInFaults = true });
                }
                else
                {
                    if (!debug.IncludeExceptionDetailInFaults)
                    {
                        debug.IncludeExceptionDetailInFaults = true;
                    }
                }

                host.Open();
                Console.WriteLine("service started at {0}", host.BaseAddresses[0]);
                Console.ReadLine();
            }
        }
    }
}
