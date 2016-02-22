using System;

namespace WCF.XmlDSig.Core
{
    /// <summary>
    /// ��� ������������ ������������ ����� ��� ����� ������������� ������������ ����� ��������� � ������������� ���� � ����� ���� ���������� � ����� � �� ����
    /// </summary>
    /// <remarks>
    /// ���� �������� ������� ����� ���������� �������� �� � ������� �������������� ���� �� �������� ��� �������� ��������� � CustomAttribute
    /// </remarks>
    public enum SoapPrefix
    {
        /// <summary>
        /// urn://x-artefacts-smev-gov-ru/services/message-exchange/types/1.1
        /// </summary>
        [Custom(SoapNamespace = "urn://x-artefacts-smev-gov-ru/services/message-exchange/types/1.1")]
        types,
        /// <summary>
        /// http://schemas.xmlsoap.org/soap/envelope/
        /// </summary>
        [Custom(SoapNamespace = @"http://schemas.xmlsoap.org/soap/envelope/")]
        s,
        /// <summary>
        /// http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd
        /// </summary>
        [Custom(SoapNamespace = @"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd")]
        wsse,
        /// <summary>
        /// http://schemas.microsoft.com/ws/2005/05/addressing/none
        /// </summary>
        [Custom(SoapNamespace = @"http://schemas.microsoft.com/ws/2005/05/addressing/none")]
        a,
        /// <summary>
        /// http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd
        /// </summary>
        [Custom(SoapNamespace = @"http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd")]
        wsu,
        /// <summary>
        /// http://www.w3.org/2000/09/xmldsig#
        /// </summary>
        [Custom(SoapNamespace = @"http://www.w3.org/2000/09/xmldsig#")]
        ds,

        /// <summary>
        /// urn://x-artefacts-smev-gov-ru/services/message-exchange/types/basic/1.1
        /// </summary>
        [Custom(SoapNamespace = @"urn://x-artefacts-smev-gov-ru/services/message-exchange/types/basic/1.1")]
        basic,

        /// <summary>
        /// urn://x-artefacts-smev-gov-ru/services/message-exchange/types/faults/1.1
        /// </summary>
        [Custom(SoapNamespace = @"urn://x-artefacts-smev-gov-ru/services/message-exchange/types/faults/1.1")]
        fault,

        /// <summary>
        /// ��� ������������ ���� ����� ������� ����������� � ��������� ����� ��������. 
        /// ��������� ������ �� ����� � ������� 1.0.0 ������� ����� �� ������� SmevXsdVersions"/>
        /// urn://x-artefacts-smev-gov-ru/supplementary/commons/
        /// </summary>
        [Custom(SoapNamespace = @"urn://x-artefacts-smev-gov-ru/supplementary/commons/")]
        q1

    }

    /// <summary>
    /// ������� ��� �������� �������� ������������
    /// </summary>
    [AttributeUsage(AttributeTargets.All, Inherited = false)]
    public class CustomAttribute : Attribute
    {
        #region �����

        public string ContourType { get; set; }
        public string FaultCode { get; set; }
        public string FaultText { get; set; }
        public string WebServiceContourType { get; set; }
        public string SoapNamespace { get; set; }
        public SoapPrefix NodePrefix { get; set; }
        public string StatusMessages { get; set; }
        public string TaskStatus { get; set; }
        public string OrderStatus { get; set; }
        public string SubserviceVersionStatus { get; set; }
        public string FieldTypeRusName { get; set; }
        public Type FieldTypeType { get; set; }

        public string DirectionShortName { get; set; }

        public string DirectionFullName { get; set; }

        #endregion

        #region ������

        public string TableName { get; set; }
        public string SoapHeaderNamespace { get; set; }
        public string FieldTypeDescription { get; set; }
        public string FieldTypeDescriptionRus { get; set; }

        #endregion

        #region ������

        public string XrmEntityType { get; set; }
        public string WebServiceState { get; set; }
        public string WebServiceAccessibility { get; set; }
        public string IncidentStatus { get; set; }

        #endregion

    }
}