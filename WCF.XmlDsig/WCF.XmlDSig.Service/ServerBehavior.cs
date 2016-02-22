using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;
using System.Threading;
using WCF.XmlDSig.Core;

namespace WCF.XmlDSig.Service
{
    public class ServerBehavior : IEndpointBehavior, IDispatchMessageInspector
    {
        public void Validate(ServiceEndpoint endpoint)
        {
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
            endpointDispatcher.DispatchRuntime.MessageInspectors.Add(this);
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
        }

        public object AfterReceiveRequest(ref Message request, IClientChannel channel, InstanceContext instanceContext)
        {
            var messageManager = new SoapMessageManager(request);

            SoapNode failedNode;
            if (!messageManager.CheckSign(out failedNode))
            {
                throw new FaultException(
                    new FaultReason(
                        new FaultReasonText(TestFault.InvalidSignature.GetAttribute_FaultText() + ". " + failedNode + " node")),
                    FaultCode.CreateReceiverFaultCode(TestFault.InvalidSignature.ToString(),
                        SoapPrefix.fault.GetAttribute_SoapNamespace()));
            }
            request = messageManager;
            return null;
        }

        public void BeforeSendReply(ref Message reply, object correlationState)
        {
        }
    }
}