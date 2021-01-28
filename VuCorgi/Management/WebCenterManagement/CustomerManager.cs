using log4net;
using MainLibrary.Entity.WebCenter;
using SeanAPI.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VuBongBongWeb.Management.WebCenterManagement
{
    public class CustomerManager : DataExtension, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WebUserManager));
        private WebCenterContext _context = new WebCenterContext();

        public CustomerManager()
        {
            cn = (SqlConnection)(_context.Database.Connection);
        }

        public void Dispose()
        {
            cn.Close();
        }
        
        #region Message

        public Customer_Message[] GetAllMessage(string keyword, out string error, bool? isRead = null)
        {
            try
            {
                error = string.Empty;
                var para = new Hashtable();
                para.Add("@Keyword", keyword);
                para.Add("@IsRead", isRead);
                var dt = this.ExecuteStoreProcedure<Customer_Message>("Customer_Message_GetAll", para).ToArray();
                return dt;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                error = ex.Message;
            }
            return null;
        }
        public bool SaveMessage(Customer_Message entity, out string error)
        {
            try
            {
                error = string.Empty;
                Customer_Message dt = null;
                if (entity.Id == 0)
                {
                    dt = CreateCustomer_Message(entity, out error);
                }
                if (dt != null)
                    return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        public Customer_Message GetDetailMessage(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Customer_Messages.FirstOrDefault(c => c.Id == id);
                if (!dt.IsRead)
                {
                    MarkReadMessage(id, out string err);
                }
                return dt;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public bool MarkReadMessage(int id, out string error, bool isAll = false)
        {
            try
            {
                error = string.Empty;
                if (isAll)
                {
                    var dt = _context.Customer_Messages.Where(c => !c.IsRead);
                    foreach (var item in dt)
                    {
                        item.IsRead = true;
                        _context.Entry(item).State = System.Data.Entity.EntityState.Modified;
                    }
                }
                else
                {
                    var dt = _context.Customer_Messages.FirstOrDefault(c => c.Id == id);
                    dt.IsRead = !dt.IsRead;
                    _context.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }


        public bool DeleteMessage(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Customer_Messages.FirstOrDefault(c => c.Id == id);
                _context.Entry(dt).State = System.Data.Entity.EntityState.Deleted;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        public Customer_Message CreateCustomer_Message(Customer_Message entity, out string error)
        {
            try
            {
                error = string.Empty;
                entity.CreatedDate = DateTime.Now;
                _context.Entry(entity).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return entity;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        #endregion

        #region Feedback


        public Customer_Feedback[] GetAllCustomer_Feedback(string keyword,out string error)
        {
            try
            {
                //if (string.IsNullOrEmpty(type)) type = Customer_FeedbackType.Customer_Feedback.ToString();
                error = string.Empty;
                var para = new Hashtable();
                para.Add("@Keyword", keyword);
                var dt = this.ExecuteStoreProcedure<Customer_Feedback>("Customer_Feedback_GetAll", para).ToArray();
                //string folder = HttpContext.Current.Server.MapPath("/ServerFile");
                string folder = Settings.FilePath;
                foreach (var item in dt)
                {
                    item.FilePath = folder + item.FilePath;
                }
                return dt;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public bool SaveCustomer_Feedback(Customer_Feedback entity, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                Customer_Feedback dt = null;
                if (entity.Id == 0)
                {
                    entity.CreatedUser = userId;
                    entity.CreatedDate = DateTime.Now;
                    dt = CreateCustomer_Feedback(entity, out error);
                }
                else
                {
                    entity.UpdatedUser = userId;
                    entity.UpdatedDate = DateTime.Now;
                    dt = UpdateCustomer_Feedback(entity, out error);
                }
                if (dt != null)
                    return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        public Customer_Feedback GetDetailCustomer_Feedback(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Customer_Feedbacks.FirstOrDefault(c => c.Id == id);
                if (dt != null)
                {
                    string folder = Settings.FilePath;
                    dt.FilePath = folder + _context.FileManagements.FirstOrDefault(c => c.Id == dt.FileId)?.Path;
                }
                return dt;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public bool ChangeStatusCustomer_Feedback(int id, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Customer_Feedbacks.FirstOrDefault(c => c.Id == id);
                dt.IsActive = !dt.IsActive;
                dt.UpdatedDate = DateTime.Now;
                dt.UpdatedUser = userId;
                _context.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }


        public bool DeleteCustomer_Feedback(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Customer_Feedbacks.FirstOrDefault(c => c.Id == id);
                //dt.IsDelete = true;
                _context.Entry(dt).State = System.Data.Entity.EntityState.Deleted;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        public Customer_Feedback CreateCustomer_Feedback(
            Customer_Feedback feedback, out string error)
        {
            error = string.Empty;
            try
            {
                _context.Entry(feedback).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return feedback;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public Customer_Feedback UpdateCustomer_Feedback(Customer_Feedback feedback, out string error)
        {
            error = string.Empty;
            try
            {
                var oldDt = _context.Customer_Feedbacks.FirstOrDefault(c => c.Id == feedback.Id);
                oldDt.Title = feedback.Title;
                oldDt.Content = feedback.Content;
                oldDt.IsActive = feedback.IsActive;
                oldDt.UpdatedDate = feedback.UpdatedDate;
                oldDt.UpdatedUser = feedback.UpdatedUser;
                oldDt.FileId = feedback.FileId;
                _context.Entry(oldDt).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return oldDt;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        #endregion
    }
}