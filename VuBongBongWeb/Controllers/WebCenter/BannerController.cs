using VuBongBongWeb.Management.WebCenterManagement;
using VuBongBongWeb.Security;
using MainLibrary.Entity.WebCenter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VuBongBongWeb.Controllers.BE
{
    [Authentication]
    [MenuSync(Description = "Banner", CssClass = "fa fa-lg fa-fw fa-bar-chart-o", IsBindingWithParent = false, Level = 0, SyncOrder = 1)]
    public class BannerController : Controller
    {
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 1, Description = "Danh sách Banner", CssClass = "fa fa-file-image-o")]
        public ActionResult BannerMain()
        {
            var data = new Banner[0];
            using (var _manager = new BannerManager())
            {
                data = _manager.GetAllBanner(string.Empty, out string error);
            }
            return View(data);
        }

        [MenuSync(ParentAction = "BannerMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 1, Description = "Details")]
        public ActionResult Details(int? id)
        {
            var dt = new Banner();
            if (id.HasValue && id.Value != 0)
            {
                string error = string.Empty;
                using (var repo = new BannerManager())
                {
                    dt = repo.GetDetailBanner(id.Value, out error);
                }
            }
            return View(dt);
        }

        // POST: Banner/Edit/5
        [HttpPost]
        [MenuSync(ParentAction = "BannerMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 2, Description = "Edit", CssClass = "fa fa-money")]
        public ActionResult SaveBanner(int? bannerId, int? order, int? fileId, bool? isEnable, HttpPostedFileBase myFile)
        {
            var collection = new Banner()
            {
                Id = bannerId ?? 0,
                Title = "title",
                Description = "description",
                TargetUrl = "#",
                IsEnable = isEnable ?? true,
                //FileId = newId,
                Order = order ?? 0
            };
            try
            {
                // TODO: Add update logic here
                #region Validate

                if (string.IsNullOrEmpty(collection.Title))
                {
                    ModelState.AddModelError("", "Title is required !");
                    return View("Details", collection);
                }
                if (string.IsNullOrEmpty(collection.Description))
                {
                    ModelState.AddModelError("", "description is required !");
                    return View("Details", collection);
                }
                if (string.IsNullOrEmpty(collection.TargetUrl))
                {
                    ModelState.AddModelError("", "targetUrl is required !");
                    return View("Details", collection);
                }
                if (myFile != null && myFile.ContentLength >= (2024 * 1024))
                {
                    ModelState.AddModelError("", "File too big !");
                    return View("Details", collection);
                }
                if (!order.HasValue)
                {
                    ModelState.AddModelError("", "Order is required !");
                    return View("Details", collection);
                }
                #endregion

                string error = string.Empty;
                int newId = fileId ?? 0;
                #region SaveFile
                if (newId == 0 && myFile == null)
                {
                    ModelState.AddModelError("", "Please input image !");
                    return View("Details", collection);
                }
                if (myFile != null)
                {
                    MemoryStream ms = new MemoryStream();
                    myFile.InputStream.CopyTo(ms);
                    var fileAtt = new FileManagement()
                    {
                        Id = fileId ?? 0,
                        DisplayName = myFile.FileName
                    };
                    newId = FileManager.SaveFile(ms.ToArray(), fileAtt, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                if (!string.IsNullOrEmpty(error))
                {
                    ModelState.AddModelError("", error);
                    return View("Details", collection);
                }
                if (newId == 0)
                {
                    ModelState.AddModelError("", "Cannot save this file !");
                    return View("Details", collection);
                }
                #endregion

                #region Save Banner
                collection.FileId = newId;
                bool dt = false;
                using (var repo = new BannerManager())
                {
                    dt = repo.SaveBanner(collection, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                if (!dt)
                {
                    ModelState.AddModelError("", error);
                    return View("Details", collection);
                }
                #endregion
                return RedirectToAction("BannerMain");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View("Details", collection);
            }
        }

        // POST: Banner/Edit/5
        [HttpPost]
        [MenuSync(ParentAction = "BannerMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 3, Description = "ChangeStatus", CssClass = "fa fa-money")]
        public JsonResult ChangeStatus(int id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new BannerManager())
                {
                    dt = repo.ChangeStatusBanner(id, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json("Fail");
            }
        }

        // POST: Banner/Edit/5
        [HttpPost]
        [MenuSync(ParentAction = "BannerMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 4, Description = "Delete", CssClass = "fa fa-money")]
        public JsonResult Delete(int id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new BannerManager())
                {
                    dt = repo.DeleteBanner(id, out error);
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
