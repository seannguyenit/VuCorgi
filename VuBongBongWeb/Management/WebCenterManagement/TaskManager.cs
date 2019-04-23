using log4net;
using MainLibrary.Entity.WebCenter;
using MainLibrary.Resource.WebCenter;
using SeanAPI.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VuBongBongWeb.Management.WebCenterManagement
{
    public class TaskManager : DataExtension, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WebUserManager));
        private WebCenterContext _context = new WebCenterContext();

        public TaskManager()
        {
            cn = (SqlConnection)(_context.Database.Connection);
        }

        public void Dispose()
        {
            cn.Close();
        }

        #region Task

        public TaskReportUser[] GetTaskReportUser(Guid userId)
        {
            try
            {
                var para = new Hashtable();
                para.Add("@UserId", userId);
                var dt = this.ExecuteStoreProcedure<TaskReportUser>("Task_ReportUser", para).ToArray();
                return dt;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return null;
        }

        public TaskWeb[] GetAllMyTask(
            string keyword,
            DateTime fromDate,
            DateTime toDate,
            int typeId,
            Guid? tagetUser,
            Guid? executor,
            int status,
            bool? isUrgent,
            Guid currentUser,
            out string error,
            bool? isEnable = null)
        {
            try
            {
                error = string.Empty;
                var para = new Hashtable();
                para.Add("@Keyword", keyword);
                para.Add("@FromDate", fromDate);
                para.Add("@ToDate", toDate);
                para.Add("@TypeId", typeId);
                para.Add("@TagetUser", tagetUser);
                para.Add("@Executor", executor);
                para.Add("@IsEnable", isEnable);
                para.Add("@Status", status);
                para.Add("@IsUrgent", isUrgent);
                para.Add("@User", currentUser);
                var dt = this.ExecuteStoreProcedure<TaskWeb>("Task_GetAllMyTask", para).ToArray();
                return dt;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                error = ex.Message;
            }
            return null;
        }
        public bool SaveTask(TaskWeb entity, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                TaskWeb dt = null;
                if (entity.Id == 0)
                {
                    dt = CreateTask(entity, userId, out error);
                }
                else
                {
                    dt = UpdateTask(entity, userId, out error);
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

        public TaskWeb GetDetailTask(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var para = new Hashtable();
                para.Add("@taskId", id);
                var dt = this.ExecuteStoreProcedure<TaskWeb>("Task_GetDetailTask", para).FirstOrDefault();
                if (dt != null)
                {
                    dt.ListDetails = _context.Task_Detailss.Where(c => c.TaskId == dt.Id).ToList();
                }
                return dt;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public bool ChangeStatusTask(int id, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Tasks.FirstOrDefault(c => c.Id == id);
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


        public bool DeleteTask(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Tasks.FirstOrDefault(c => c.Id == id);
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

        public TaskWeb CreateTask(TaskWeb entity, Guid user, out string error)
        {
            try
            {
                error = string.Empty;
                entity.CreatedDate = DateTime.Now;
                entity.CreatedUser = user;
                _context.Entry(entity).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                if (entity.StartTime.HasValue)
                {
                    var details = new Task_Details()
                    {
                        EditerId = entity.Executor.Value,
                        TaskId = entity.Id,
                        FromTime = entity.StartTime.Value,
                        IsActive = true,
                        LastModified = entity.StartTime.Value,
                        Note = string.Empty
                    };
                    _context.Entry(details).State = System.Data.Entity.EntityState.Added;
                    _context.SaveChanges();
                }
                return entity;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public TaskWeb UpdateTask(TaskWeb entity, Guid user, out string error)
        {
            try
            {
                error = string.Empty;
                var oldEntity = _context.Tasks.FirstOrDefault(c => c.Id == entity.Id);
                oldEntity.IsActive = entity.IsActive;
                oldEntity.Name = entity.Name;
                oldEntity.Description = entity.Description;
                oldEntity.IsUrgent = entity.IsUrgent;
                oldEntity.Pin = entity.Pin;
                oldEntity.Cost = entity.Cost;
                oldEntity.Order = entity.Order;
                oldEntity.UpdatedDate = DateTime.Now;
                oldEntity.UpdatedUser = user;
                _context.Entry(oldEntity).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return oldEntity;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                error = ex.Message;
            }
            return null;
        }

        #region Task Action

        public bool StartTask(int taskId, Guid userId, out string error)
        {
            error = string.Empty;
            try
            {
                DateTime time = DateTime.Now;
                var entity = GetDetailTask(taskId, out string err);
                if (entity == null || (entity.Status != TaskStatus.New.ToString() && entity.Status != TaskStatus.Cancel.ToString()))
                {
                    error = "Wrong status !";
                    return false;
                }
                entity.StartTime = time;
                entity.Executor = userId;
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                var details = new Task_Details()
                {
                    TaskId = taskId,
                    FromTime = time,
                    Note = string.Empty,
                    IsActive = true,
                    LastModified = time,
                    EditerId = userId
                };
                _context.Entry(details).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        public bool PauseTask(int taskId, Guid userId, out string error)
        {
            error = string.Empty;
            try
            {
                DateTime time = DateTime.Now;
                var entity = GetDetailTask(taskId, out string err);
                var details = _context.Task_Detailss.FirstOrDefault(c => c.TaskId == taskId && !c.ToTime.HasValue);
                if (entity == null || entity.Status != TaskStatus.Working.ToString())
                {
                    error = "Wrong status !";
                    return false;
                }
                if (details != null)
                {
                    details.ToTime = DateTime.Now;
                    _context.Entry(details).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        public bool ResumeTask(int taskId, Guid userId, out string error)
        {
            error = string.Empty;
            try
            {
                DateTime time = DateTime.Now;
                var entity = GetDetailTask(taskId, out string err);
                if (entity == null || entity.Status != TaskStatus.Pending.ToString())
                {
                    error = "Wrong status !";
                    return false;
                }
                var details = new Task_Details()
                {
                    TaskId = taskId,
                    FromTime = time,
                    Note = string.Empty,
                    IsActive = true,
                    LastModified = time,
                    EditerId = userId
                };
                _context.Entry(details).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        public bool FinishTask(int taskId, Guid userId, out string error)
        {
            error = string.Empty;
            try
            {
                DateTime time = DateTime.Now;
                var entity = GetDetailTask(taskId, out string err);
                var details = _context.Task_Detailss.FirstOrDefault(c => c.TaskId == taskId && !c.ToTime.HasValue);
                if (entity.Duration < 3)
                {
                    error = "Task should have duration 3 minutes at least !";
                    return false;
                }
                if (entity == null || (entity.Status != TaskStatus.Cancel.ToString() && entity.Status != TaskStatus.New.ToString()))
                {
                    error = "Wrong status !";
                    return false;
                }
                entity.FinishTime = time;
                _context.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                if (details != null)
                {
                    details.ToTime = DateTime.Now;
                    _context.Entry(details).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                }
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        #endregion

        #endregion

        #region Task_Type
        public Task_Type[] GetAllTask_Type(string keyword, out string error, bool? isEnable = null)
        {
            try
            {
                error = string.Empty;
                var para = new Hashtable();
                para.Add("@Keyword", keyword);
                para.Add("@IsEnable", isEnable);
                var dt = this.ExecuteStoreProcedure<Task_Type>("Task_Type_GetAll", para).ToArray();
                return dt;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                error = ex.Message;
            }
            return null;
        }
        public bool SaveTask_Type(Task_Type entity, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                Task_Type dt = null;
                if (entity.Id == 0)
                {
                    dt = CreateTask_Type(entity, userId, out error);
                }
                else
                {
                    dt = UpdateTask_Type(entity, userId, out error);
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

        public Task_Type GetDetailTask_Type(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Task_Types.FirstOrDefault(c => c.Id == id);
                return dt;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public bool ChangeStatusTask_Type(int id, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Task_Types.FirstOrDefault(c => c.Id == id);
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

        public bool DeleteTask_Type(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Task_Types.FirstOrDefault(c => c.Id == id);
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

        public Task_Type CreateTask_Type(Task_Type entity, Guid user, out string error)
        {
            try
            {
                error = string.Empty;
                entity.CreatedDate = DateTime.Now;
                entity.CreatedUser = user;
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

        public Task_Type UpdateTask_Type(Task_Type entity, Guid user, out string error)
        {
            try
            {
                error = string.Empty;
                var oldEntity = _context.Task_Types.FirstOrDefault(c => c.Id == entity.Id);
                oldEntity.IsActive = entity.IsActive;
                oldEntity.Name = entity.Name;
                oldEntity.UpdatedDate = DateTime.Now;
                oldEntity.UpdatedUser = user;
                _context.Entry(oldEntity).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return oldEntity;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                error = ex.Message;
            }
            return null;
        }
        #endregion


    }
}