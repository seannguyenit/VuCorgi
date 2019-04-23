using Library.Entity.Base;
using Library.Resource;
using MainLibrary.Entity.API_Security;
using MainLibrary.Resource.Security;
using SeanAPI.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VuBongBongWeb.Management.SecurityManagement
{
    public class UserAPIManager : DataExtension
    {
        private SecurityContext _context = new SecurityContext();

        public UserAPIManager()
        {
            cn = (SqlConnection)(_context.Database.Connection);
        }

        public ResponseObject ActionMethod(RequestObject request)
        {
            ResponseObject result = new ResponseObject();
            result.Data = new Hashtable();
            result.Data.Add(Resource.K_RETURN_STATUS, false);
            result.Data.Add(Resource.K_RETURN_MESSAGE, "Incorrect Request method.");
            try
            {
                SecurityMethods method;
                string methodName = request.MethodName;
                Enum.TryParse(methodName, true, out method);
                switch (method)
                {
                    //case SecurityMethods.CheckAccessAPI:
                    //    return CheckAccessAPI(request);
                    default:
                        return result;
                }
            }
            catch (Exception ex)
            {
            }
            return result;
        }

        public UserAPI_License CheckAccessAPI(RequestObject request)
        {
            var userName = (Guid)request.Parameters[Resource.P_USERNAME];
            var password = (string)request.Parameters[Resource.P_PASSWORD];
            var projectName = (string)request.Parameters[Resource.P_PROJECT];
            var HDDCode = (string)request.Parameters[Resource.P_HDDCODE];
            var computerName = (string)request.Parameters[Resource.P_COMPUTERNAME];
            //ResponseObject responseObject = new ResponseObject();
            var para = new Hashtable();
            para.Add("@UserName", userName);
            para.Add("@Password", password);
            para.Add("@ProjectName", projectName);
            para.Add("@HDDCode", HDDCode);
            para.Add("@ComputerName", computerName);
            var dt = this.ExecuteStoreProcedure<UserAPI_License>("CheckAccessAPI", para).FirstOrDefault();
            //responseObject.SetHashtable(dt);
            //responseObject.SetMessage("Success");
            //responseObject.SetStatus(true);
            return dt;
        }
    }
}