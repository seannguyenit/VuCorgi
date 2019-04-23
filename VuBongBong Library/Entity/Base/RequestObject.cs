using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;

namespace Library.Entity.Base
{
    [Serializable]
    [DataContract]
    public class RequestObject
    {
        #region Properties

        /// <summary>
        /// Module name
        /// </summary>
        [DataMember]
        public string ModuleName { get; set; }

        /// <summary>
        /// Method name
        /// </summary>
        [DataMember]
        public string MethodName { get; set; }

        /// <summary>
        /// Module name
        /// </summary>
        [DataMember]
        public int ModuleCode { get; set; }

        /// <summary>
        /// Method name
        /// </summary>
        [DataMember]
        public int MethodCode { get; set; }

        //[DataMember]
        //public string Token { get; set; }

        [DataMember]
        public Hashtable Parameters { get; set; }

        /// <summary>
        /// Is send message via signal R, one signal. 
        /// </summary>
        [DataMember]
        public bool IsSendViaSignalR { get; set; }

        #endregion Properties

        #region Constructors

        public RequestObject()
        {
            Parameters = new Hashtable();
        }

        public RequestObject(Enum eModule, Enum eMethod)
        {
            ModuleName = eModule.ToString();
            MethodName = eMethod.ToString();

            var codeAssemblyPath = Assembly.GetAssembly(
                eModule.GetType()).GetName().FullName;
            var code = Convert.ToInt32(eModule);
            ModuleCode = code;

            codeAssemblyPath = Assembly.GetAssembly(
                eMethod.GetType()).GetName().FullName;
            code = Convert.ToInt32(eMethod);
            MethodCode = code;

            Parameters = new Hashtable();
        }

        public RequestObject(Enum eModule, Enum eMethod, Hashtable parameters)
        {
            ModuleName = eModule.ToString();
            MethodName = eMethod.ToString();

            var codeAssemblyPath = Assembly.GetAssembly(
                eModule.GetType()).GetName().FullName;
            var code = Convert.ToInt32(eModule);
            ModuleCode = code;

            codeAssemblyPath = Assembly.GetAssembly(
                eMethod.GetType()).GetName().FullName;
            code = Convert.ToInt32(eMethod);
            MethodCode = code;

            Parameters = parameters;
        }

        #endregion Constructors

        public static RequestObject Init(Enum eModule, Enum eMethod, Hashtable parameters)
        {
            var retVal = new RequestObject();
            retVal.ModuleName = eModule.ToString();
            retVal.MethodName = eMethod.ToString();
            retVal.Parameters = parameters;
            return retVal;
        }
    }
}
