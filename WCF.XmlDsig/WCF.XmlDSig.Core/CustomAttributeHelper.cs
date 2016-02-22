using System;
using System.Linq;

namespace WCF.XmlDSig.Core
{
    public static class CustomAttributeHelper
    {
        #region Type

        private static CustomAttribute GetCustomAttribute(Type type)
        {
            return
                type.GetCustomAttributes(typeof(CustomAttribute), true).FirstOrDefault() as
                    CustomAttribute;
        }

        public static string GetAttribute_SoapHeaderNamespace(this Type type)
        {
            var dnAttribute =
                GetCustomAttribute(type);
            return dnAttribute == null ? null : dnAttribute.SoapHeaderNamespace;
        }

        public static string GetAttribute_EntityTableName(this Type type)
        {
            var dnAttribute =
                GetCustomAttribute(type);
            return dnAttribute == null ? null : dnAttribute.TableName;
        }


        #endregion

        #region Enum

        private static CustomAttribute GetEnumCustomAttribute(Enum source)
        {
            return
                Attribute.GetCustomAttribute(source.GetType().GetField(source.ToString()),
                    typeof(CustomAttribute)) as CustomAttribute;
        }

        public static string GetAttribute_ContourType(this Enum source)
        {
            var attribute = GetEnumCustomAttribute(source);
            return attribute == null ? null : attribute.ContourType;
        }

        public static string GetAttribute_FaultCode(this Enum source)
        {
            var attribute = GetEnumCustomAttribute(source);
            return attribute == null ? null : attribute.FaultCode;
        }


        public static SoapPrefix GetAttribute_NodePrefix(this Enum source)
        {
            var attribute = GetEnumCustomAttribute(source);
            return attribute.NodePrefix;
        }

        public static string GetAttribute_SoapNamespace(this Enum source)
        {
            var attribute = GetEnumCustomAttribute(source);
            return attribute.SoapNamespace;
        }

        /// <summary>
        /// Возвращает статус задачи в црм
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetAttribute_TaskStatus(this Enum source)
        {
            var attribute = GetEnumCustomAttribute(source);
            return attribute == null ? null : attribute.TaskStatus;
        }

        /// <summary>
        /// Возвращает тип контура
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static string GetAttribute_WebServiceContourType(this Enum source)
        {
            var attribute = GetEnumCustomAttribute(source);
            return attribute == null ? null : attribute.WebServiceContourType;

        }

        public static string GetAttribute_SubserviceVersionStatus(this Enum source)
        {
            var attribute = GetEnumCustomAttribute(source);
            return attribute == null ? null : attribute.SubserviceVersionStatus;
        }
        public static string GetAttribute_IncidentStatus(this Enum source)
        {
            var attribute = GetEnumCustomAttribute(source);
            return attribute == null ? null : attribute.IncidentStatus;
        }

        public static string GetAttribute_FaultText(this Enum source)
        {
            var codeStringAttribute = GetEnumCustomAttribute(source);
            return codeStringAttribute == null ? null : codeStringAttribute.FaultText;
        }

        public static string GetAttribute_FieldTypeRusName(this Enum source)
        {
            var codeStringAttribute = GetEnumCustomAttribute(source);
            return codeStringAttribute == null ? null : codeStringAttribute.FieldTypeRusName;
        }
        public static string GetAttribute_XrmEntityType(this Enum source)
        {
            var codeStringAttribute = GetEnumCustomAttribute(source);
            return codeStringAttribute == null ? null : codeStringAttribute.XrmEntityType;
        }

        public static Type GetAttribute_FieldTypeType(this Enum source)
        {
            var codeStringAttribute = GetEnumCustomAttribute(source);
            return codeStringAttribute == null ? null : codeStringAttribute.FieldTypeType;
        }

        public static string GetAttribute_StatusMessages(this Enum source)
        {
            var attribute = GetEnumCustomAttribute(source);
            return attribute == null ? null : attribute.StatusMessages;
        }

        public static string GetAttribute_DirectionShortName(this Enum source)
        {
            var attribute = GetEnumCustomAttribute(source);
            return attribute == null ? null : attribute.DirectionShortName;
        }

        public static string GetAttribute_DirectionFullName(this Enum source)
        {
            var attribute = GetEnumCustomAttribute(source);
            return attribute == null ? null : attribute.DirectionFullName;
        }

        #endregion
    }
}