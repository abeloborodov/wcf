using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using WCF.XmlDSig.Core;

namespace WCF.XmlDSig.Client
{
    public class ClientBehavior : IEndpointBehavior, IClientMessageInspector
    {
        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            var m = new SoapMessageManager(request);
            request = m.SignMessage();
            
            request = new SoapMessageManager(request).DefaceSign(SoapNode.PrimaryContent);
            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            clientRuntime.MessageInspectors.Add(this);
        }
    }
}