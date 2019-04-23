using Library;
using MainLibrary.Entity.WebCenter;
using MainLibrary.Resource.WebCenter;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VuBongBongWeb.Management.WebCenterManagement;
using VuBongBongWeb.Models;
using VuBongBongWeb.Security;

namespace VuBongBongWeb.Controllers.WebCenter
{
    [MenuSync(Description = "Task", CssClass = "fa fa-lg fa-fw fa-bar-chart-o", IsBindingWithParent = false, Level = 0, SyncOrder = 4)]
    public class TaskController : Controller
    {
        #region TaskType
        [Authentication]
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 1, Description = "Task type", CssClass = "fa fa-money")]
        public ActionResult TaskTypeMain()
        {
            var data = new Task_Type[0];
            using (var _manager = new TaskManager())
            {
                data = _manager.GetAllTask_Type(string.Empty, out string error);
            }
            return View(data);
        }
        [Authentication]
        [MenuSync(ParentAction = "TaskTypeMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 1, Description = "Details")]
        public ActionResult TypeDetails(int? id)
        {
            var dt = new Task_Type();
            string error = string.Empty;
            if (id.HasValue)
            {
                using (var repo = new TaskManager())
                {
                    dt = repo.GetDetailTask_Type(id.Value, out error);
                }
            }
            return View(dt);
        }

        // POST: Business/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "TaskTypeMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 2, Description = "Edit", CssClass = "fa fa-money")]
        public ActionResult SaveTask_Type(Task_Type collection)
        {
            //var collection = htmlcontent.Entity as Task_Type;
            //collection.Content = htmlcontent.HtmlStr.ToString();
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new TaskManager())
                {
                    dt = repo.SaveTask_Type(collection, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                if (!dt)
                {
                    ModelState.AddModelError("", error);
                    return View(collection);
                }
                return RedirectToAction("TaskTypeMain");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(collection);
            }
        }

        // POST: Business/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "TaskTypeMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 3, Description = "ChangeStatus", CssClass = "fa fa-money")]
        public JsonResult ChangeTypeStatus(int id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new TaskManager())
                {
                    dt = repo.ChangeStatusTask_Type(id, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json("Fail");
            }
        }

        // POST: Business/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "TaskTypeMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 4, Description = "Delete", CssClass = "fa fa-money")]
        public JsonResult DeleteType(int id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new TaskManager())
                {
                    dt = repo.DeleteTask_Type(id, out error);
                }
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json("Fail");
            }
        }


        #endregion

        #region Task

        [Authentication]
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 2, Description = "My Task", CssClass = "fa fa-money")]
        public ActionResult TaskMain()
        {

            #region TaskType
            var types = new List<Task_Type>();
            using (var repo = new TaskManager())
            {
                types = repo.GetAllTask_Type(string.Empty, out string error, true).ToList();
            }
            types.Add(new Task_Type()
            {
                Id = 0,
                Name = "All Type"
            });
            ViewBag.Types = types.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Name,
                Selected = (f.Id == 0)
            });
            #endregion
            #region Status
            var allStatus = Enum.GetValues(typeof(TaskStatus)).Cast<TaskStatus>();
            ViewBag.Status = allStatus.Select(f => new SelectListItem
            {
                Value = f.ToString(),
                Text = Enum.GetName(typeof(TaskStatus), f)
            });
            #endregion
            #region All User
            var targetUsers = new List<User>();
            using (var repo = new WebUserManager())
            {
                targetUsers = repo.GetAllUser(string.Empty, out string error, true).ToList();
            }
            targetUsers.Add(new User()
            {
                UserId = Guid.Empty,
                UserName = "All User"
            });
            ViewBag.TargetUser = targetUsers.Select(f => new SelectListItem
            {
                Value = f.UserId.ToString(),
                Text = f.UserName,
                Selected = (f.UserId == Guid.Empty)
            });
            #endregion
            //var data = new TaskWeb[0];
            //var currentUser = ((WebPrincipal)HttpContext.User).UserDetail;
            //using (var _manager = new TaskManager())
            //{
            //    data = _manager.GetAllMyTask(string.Empty, DateTime.Now.AddDays(-15), DateTime.Now, 0, null, null, 0, null, currentUser.UserId, out string error, true);
            //}
            return View();
        }

        [Authentication]
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 3, Description = "Report", CssClass = "fa fa-money")]
        public ActionResult Report(Guid? id)
        {
            #region All User
            var targetUsers = new List<User>();
            using (var repo = new WebUserManager())
            {
                targetUsers = repo.GetAllUser(string.Empty, out string error, true).ToList();
            }
            targetUsers.Add(new User()
            {
                UserId = Guid.Empty,
                UserName = "Please Select User"
            });
            ViewBag.TargetUser = targetUsers.Select(f => new SelectListItem
            {
                Value = f.UserId.ToString(),
                Text = f.UserName,
                Selected = (f.UserId == Guid.Empty)
            });


            #endregion
            return View();
        }

        public ActionResult PartialReportUser(Guid? userId)
        {
            var data = new TaskReportUser[0];
            if (userId.HasValue && userId.Value != Guid.Empty)
            {
                using (var _manager = new TaskManager())
                {
                    data = _manager.GetTaskReportUser(userId.Value);
                }
            }
            return PartialView(data);
        }

        public ActionResult PartialTaskDetail(int taskId)
        {
            var data = new TaskWeb();
            using (var _manager = new TaskManager())
            {
                data = _manager.GetDetailTask(taskId, out string error);
            }
            return PartialView(data);
        }

        public JsonResult TaskAction(int taskId, string status)
        {
            var user = ((WebPrincipal)HttpContext.User).UserDetail;
            bool result = false;
            string error = string.Empty;
            using (var _manager = new TaskManager())
            {
                if (status == TaskStatus.New.ToString())
                {
                    result = _manager.StartTask(taskId, user.UserId, out error);
                }
                else if (status == TaskStatus.Working.ToString())
                {
                    result = _manager.PauseTask(taskId, user.UserId, out error);
                }
                else if (status == TaskStatus.Pending.ToString())
                {
                    result = _manager.ResumeTask(taskId, user.UserId, out error);
                }
                else if (status == TaskStatus.Finished.ToString())
                {
                    result = _manager.FinishTask(taskId, user.UserId, out error);
                }
            }
            if (result)
            {
                return Json("Success");
            }
            return Json("Fail !" + error);
        }

        [HttpPost]
        public ActionResult SearchTaskMain(
            string keyword,
            string fromDate,
            string toDate,
            int typeId = 0,
            Guid? targetUser = null,
            Guid? executor = null,
            int status = 0,
            bool? isUrgent = null,
            bool? isActive = true)
        {
            if (string.IsNullOrEmpty(fromDate))
            {
                fromDate = DateTime.Now.AddDays(-15).Date.ToString();
            }
            if (string.IsNullOrEmpty(toDate))
            {
                toDate = DateTime.Now.Date.ToString();
            }
            if (targetUser.HasValue && targetUser.Value == Guid.Empty)
            {
                targetUser = null;
            }
            if (executor.HasValue && executor.Value == Guid.Empty)
            {
                executor = null;
            }
            var data = new TaskWeb[0];
            var currentUser = ((WebPrincipal)HttpContext.User).UserDetail;
            using (var _manager = new TaskManager())
            {
                data = _manager.GetAllMyTask(keyword, Convert.ToDateTime(fromDate), Convert.ToDateTime(toDate), typeId, targetUser, executor, status, isUrgent, currentUser.UserId, out string error, isActive);
            }
            return PartialView(data);
        }

        [Authentication]
        [MenuSync(ParentAction = "TaskMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 1, Description = "Details")]
        public ActionResult TaskDetails(int? id)
        {
            var dt = new TaskWeb();
            string error = string.Empty;
            var types = new List<Task_Type>();
            var users = new List<User>();
            if (id.HasValue)
            {
                using (var repo = new TaskManager())
                {
                    dt = repo.GetDetailTask(id.Value, out error);
                }
            }
            using (var repo = new TaskManager())
            {
                types = repo.GetAllTask_Type(string.Empty, out error, true).ToList();
            }
            ViewBag.Types = types.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Name,
                Selected = dt?.TypeId == f.Id
            });
            using (var repo = new WebUserManager())
            {
                users = repo.GetAllUser(string.Empty, out error, true).ToList();
            }
            users.Insert(0, new User() { UserId = Guid.Empty, UserName = "Non taget" });
            ViewBag.Users = users.Select(f => new SelectListItem
            {
                Value = f.UserId.ToString(),
                Text = f.UserName,
                Selected = dt?.TagetUserId == f.UserId
            });
            return View(dt);
        }

        // POST: Business/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "TaskMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 2, Description = "Edit", CssClass = "fa fa-money")]
        public ActionResult SaveTask(TaskWeb collection, string strDateEstimated, string strTimeEstimated, string strDateDeadline, string strTimeDeadline, bool? isStart)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                DateTime? estimated = ConvertStrToDateTime(strDateEstimated, strTimeEstimated);
                DateTime? deadline = ConvertStrToDateTime(strDateDeadline, strTimeDeadline);
                collection.Estimate = estimated;
                collection.Deadline = deadline;
                if (isStart ?? false)
                {
                    collection.StartTime = DateTime.Now;
                    collection.Executor = ((WebPrincipal)HttpContext.User).UserDetail.UserId;
                }
                using (var repo = new TaskManager())
                {
                    dt = repo.SaveTask(collection, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                if (!dt)
                {
                    ModelState.AddModelError("", error);
                    return View(collection);
                }
                return RedirectToAction("TaskMain");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(collection);
            }
        }

        // POST: Business/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "TaskMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 3, Description = "ChangeStatus", CssClass = "fa fa-money")]
        public JsonResult ChangeTaskStatus(int id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new TaskManager())
                {
                    dt = repo.ChangeStatusTask(id, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json("Fail");
            }
        }

        // POST: Business/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "TaskMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 4, Description = "Delete", CssClass = "fa fa-money")]
        public JsonResult DeleteTask(int id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new TaskManager())
                {
                    dt = repo.DeleteTask(id, out error);
                }
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json("Fail");
            }
        }

        #endregion

        #region Helper Method

        public DateTime? ConvertStrToDateTime(string strDate, string strTime)
        {
            if (!string.IsNullOrEmpty(strDate) && !string.IsNullOrEmpty(strTime))
            {
                DateTime value = DateTime.Parse(strDate);
                DateTime timeValue = Convert.ToDateTime(strTime);
                value = value.AddHours(timeValue.Hour).AddMinutes(timeValue.Minute);
                return value;
            }
            return null;
        }

        #endregion

    }
}
