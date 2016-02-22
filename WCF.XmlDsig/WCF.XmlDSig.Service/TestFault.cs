using WCF.XmlDSig.Core;

namespace WCF.XmlDSig.Service
{
    public enum TestFault
    {
        [Custom(FaultText = "Invalid signature", FaultCode = "100")]
        InvalidSignature
    }
}