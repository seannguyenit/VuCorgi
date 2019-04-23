using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Library.Entity.Base
{
    [Serializable]
    [DataContract(IsReference = true)]
    public class ResponseObject
    {
        public ResponseObject()
        {
            Data = new Hashtable();
        }

        [DataMember]
        public Hashtable Data { get; set; }

        //[DataMember]
        //public RET_CODE RetCode { get; set; }

        ////[IgnoreDataMember]
        ////public NoticeContentModel NoticeContent { get; set; }

        public static ResponseObject GetErrorResponse(string errorMessage)
        {
            ResponseObject result = new ResponseObject();
            result.Data.Add(Resource.Resource.K_RETURN_STATUS, false);
            result.Data.Add(Resource.Resource.K_RETURN_MESSAGE, errorMessage);
            return result;
        }

        /// <summary>
        /// Init once response model 
        /// </summary>
        /// <returns></returns>
        public static ResponseObject Init()
        {
            var result = new ResponseObject
            {
                Data = new Hashtable
                {
                    [Resource.Resource.K_RETURN_STATUS] = false,
                    [Resource.Resource.K_RETURN_MESSAGE] = string.Empty,
                    [Resource.Resource.K_HASHTABLE_DATA] = null
                }
            };
            return result;
        }

        public bool ReturnStatus
        {
            get
            {
                if (Data != null && Data.ContainsKey(Resource.Resource.K_RETURN_STATUS))
                {
                    var bo = Data[Resource.Resource.K_RETURN_STATUS] as bool?;
                    return bo ?? false;
                }
                return false;
            }
        }

        public object HashTableData
        {
            get
            {
                if (Data != null && Data.ContainsKey(Resource.Resource.K_HASHTABLE_DATA))
                {
                    return Data[Resource.Resource.K_HASHTABLE_DATA];
                }
                return null;
            }
        }

        public string ErrorMessage
        {
            get
            {
                if (Data != null && Data.ContainsKey(Resource.Resource.K_RETURN_MESSAGE))
                {
                    return (string)Data[Resource.Resource.K_RETURN_MESSAGE];
                }
                return "";
            }
        }

        /// <summary>
        /// Set status for response model
        /// </summary>
        /// <param name="status"></param>
        public void SetStatus(bool status)
        {
            Data[Resource.Resource.K_RETURN_STATUS] = status;
        }

        /// <summary>
        /// Set message to return client
        /// </summary>
        /// <param name="message"></param>
        public void SetMessage(string message)
        {
            Data[Resource.Resource.K_RETURN_MESSAGE] = message;
        }

        /// <summary>
        /// Set data to return client. 
        /// P/s: hastable mean Resource.K_HASHTABLE_DATA
        /// </summary>
        /// <param name="data"></param>
        public void SetHashtable(object data)
        {
            Data[Resource.Resource.K_HASHTABLE_DATA] = data;
        }

        /// <summary>
        /// Set notice content. 
        /// </summary>
        /// <param name="content"></param>
        //public void SetNotice(NoticeContentModel content)
        //{
        //    this.NoticeContent = content;
        //}

        /// <summary>
        /// Throw new exception
        /// </summary>
        public Exception Error()
        {
            var error = ErrorMessage;
            if (string.IsNullOrWhiteSpace(error))
                error = "Not implement set error message!";

            return new Exception(error);
        }
    }
}
