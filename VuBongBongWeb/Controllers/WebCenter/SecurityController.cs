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
    [MenuSync(Description = "Quyền truy cập", CssClass = "fa fa-lg fa-fw fa-bar-chart-o", IsBindingWithParent = false, Level = 0, SyncOrder = 3)]
    public class SecurityController : Controller
    {
        // GET: Security
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 1, Description = "Danh sách quyền", CssClass = "fa fa-stop")]
        public ActionResult RoleMain()
        {
            var data = new Role[0];
            using (var _manager = new RoleAndPermissionManager())
            {
                data = _manager.GetAllRole();
            }
            return View(data);
        }

        [MenuSync(ParentAction = "RoleMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 1, Description = "Details")]
        public ActionResult Details(int? id)
        {
            var dt = new Role();
            string error = string.Empty;

            if (id.HasValue)
            {
                using (var repo = new RoleAndPermissionManager())
                {
                    dt = repo.GetRoleDetails(id.Value);
                }
            }
            else
            {
                using (var repo = new WebBaseManager())
                {
                    var menuByRole = repo.GetMenuItemListByRole(id, true).OrderBy(m => m.MenuOrderInList);
                    dt.MenuList = menuByRole.ToDictionary(m => m, m => m.IsActive).Select(m => new RoleMenu { Menu = m.Key, IsActive = m.Value });
                }
            }
            return View(dt);
        }

        // POST: Business/Edit/5
        [HttpPost]
        [MenuSync(ParentAction = "RoleMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 2, Description = "Edit", CssClass = "fa fa-money")]
        public ActionResult SaveRole(Role collection, Dictionary<int, bool> menuItems)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                int? roleId = null;
                Role dt = null;
                var editer = ((WebPrincipal)HttpContext.User).UserDetail;
                if (collection?.Id > 0)
                {
                    roleId = collection.Id;
                }
                if (collection.RoleName == string.Empty)
                {
                    using (var repo = new WebBaseManager())
                    {
                        collection.MenuList = repo.GetMenuItemListByRole(roleId, true).OrderBy(m => m.MenuOrderInList).ToDictionary(m => m, m => m.IsActive).Select(m => new RoleMenu { Menu = m.Key, IsActive = m.Value });
                    }
                    ModelState.AddModelError("RoleName", "Role Name Are Required.");
                    return View(collection);
                }
                using (var repo = new RoleAndPermissionManager())
                {
                    if (collection.Id == 0)
                    {
                        collection.CreatedUser = editer.UserId;
                        dt = repo.CreateRole(collection, out error);
                    }
                    else
                    {
                        collection.UpdatedUser = editer.UserId;
                        dt = repo.UpdateRole(collection, out error);
                    }

                }
                if (dt == null)
                {
                    ModelState.AddModelError("", error);
                    return View(collection);
                }
                bool savePermission = false;
                using (var repo = new RoleAndPermissionManager())
                {
                    savePermission = repo.SaveManagement(menuItems, dt.Id, editer.UserId, out error);
                }
                if (!savePermission)
                {
                    ModelState.AddModelError("", error);
                    return View(collection);
                }
                return RedirectToAction("RoleMain");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(collection);
            }
        }

        // POST: Business/Edit/5
        [HttpPost]
        [MenuSync(ParentAction = "RoleMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 3, Description = "ChangeStatus", CssClass = "fa fa-money")]
        public JsonResult ChangeStatus(int id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new RoleAndPermissionManager())
                {
                    dt = repo.ChangeStatusRole(id, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
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
        [MenuSync(ParentAction = "RoleMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 4, Description = "Delete", CssClass = "fa fa-money")]
        public JsonResult Delete(int id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new RoleAndPermissionManager())
                {
                    dt = repo.DeleteRole(id, out error);
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
