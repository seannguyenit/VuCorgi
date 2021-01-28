using VuBongBongWeb.Management.WebCenterManagement;
using VuBongBongWeb.Security;
using MainLibrary.Entity.WebCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VuBongBongWeb.Controllers.WebCenter
{
    [Authentication]
    [MenuSync(Description = "Tài khoản", CssClass = "fa fa-lg fa-fw fa-bar-chart-o", IsBindingWithParent = false, Level = 0, SyncOrder = 6)]
    public class WebUserController : Controller
    {
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 1, Description = "Danh sách tài khoản", CssClass = "fa fa-male")]
        public ActionResult UserMain()
        {
            var data = new User[0];
            using (var _manager = new WebUserManager())
            {
                data = _manager.GetAllUser(string.Empty, out string error);
            }
            return View(data);
        }

        [MenuSync(ParentAction = "UserMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 1, Description = "Details")]
        public ActionResult Details(Guid id)
        {
            var dt = new User();
            string error = string.Empty;
            using (var repo = new WebUserManager())
            {
                dt = repo.GetUserDetails(id, out error);
            }
            var roles = new List<Role>();
            using (var repo = new RoleAndPermissionManager())
            {
                roles = repo.GetAllRole().ToList();
            }
            roles.Insert(0, new Role() { Id = 0, RoleName = "Default" });
            ViewBag.RoleId = roles.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.RoleName,
                Selected = dt?.RoleId == f.Id
            });
            return View(dt);
        }

        [HttpPost]
        [MenuSync(ParentAction = "UserMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 2, Description = "Edit", CssClass = "fa fa-money")]
        public ActionResult SaveUser(string username, Guid? userId, bool? isEnable, int? roleId, bool? isAdmin = false)
        {
            var collection = new User()
            {
                UserId = userId ?? Guid.Empty,
                UserName = username,
                IsActive = isEnable ?? true,
                IsAdmin = isAdmin ?? false
            };
            if (roleId.HasValue && roleId.Value != 0)
            {
                collection.RoleId = roleId.Value;
            }
            try
            {
                string error = string.Empty;
                var dt = new User();
                if (string.IsNullOrEmpty(collection.UserName))
                {
                    ModelState.AddModelError("", "Username is required !");
                    return View(collection);
                }
                using (var repo = new WebUserManager())
                {
                    if (collection.UserId != Guid.Empty)
                    {
                        dt = repo.UpdateUser(collection, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                    }
                    else
                    {
                        dt = repo.CreateUserFromAdmin(collection, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                    }
                }
                if (dt == null)
                {
                    ModelState.AddModelError("", error);
                    return View(collection);
                }
                return RedirectToAction("UserMain");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(collection);
            }
        }

        // POST: User/Edit/5
        [HttpPost]
        [MenuSync(ParentAction = "UserMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 3, Description = "ChangeStatus", CssClass = "fa fa-money")]
        public JsonResult ChangeStatus(Guid id)
        {
            try
            {
                string error = string.Empty;
                bool dt = false;
                using (var repo = new WebUserManager())
                {
                    dt = repo.ChangeStatusUser(id, out error);
                }
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json("Fail");
            }
        }

        // POST: User/Edit/5
        [HttpPost]
        [MenuSync(ParentAction = "UserMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 4, Description = "Delete", CssClass = "fa fa-money")]
        public JsonResult Delete(Guid id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new WebUserManager())
                {
                    dt = repo.DeleteUser(id, out error);
                }
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json("Fail");
            }
        }

        [HttpPost]
        [MenuSync(ParentAction = "UserMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 5, Description = "ResetPassword", CssClass = "fa fa-money")]
        public JsonResult ResetPassword(Guid id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new WebUserManager())
                {
                    dt = repo.ResetPassword(id, out error);
                }
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json("Fail");
            }
        }
    }
}
