using VuBongBongWeb.Management.WebCenterManagement;
using VuBongBongWeb.Models;
using MainLibrary.Entity.WebCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using VuBongBongWeb.Security;

namespace VuBongBongWeb.Controllers.WebCenter
{
    [Authentication]
    [MenuSync(Description = "Thông tin chung", CssClass = "fa fa-lg fa-fw fa-bar-chart-o", IsBindingWithParent = false, Level = 0, SyncOrder = 5)]
    public class WebInfoController : Controller
    {
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 1, Description = "Các tin chung", CssClass = "fa fa-info")]
        public ActionResult AllInfo()
        {
            var data = new WebInfomation[0];
            using (var repo = new WebInfoManager())
            {
                data = repo.GetAllInfo();
            }
            return View(data);
        }

        [MenuSync(ParentAction = "AllInfo", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 1, Description = "Details")]
        // GET: WebInfo/Details/5
        public ActionResult Details(string type)
        {
            var data = new WebInfomation();
            using (var repo = new WebInfoManager())
            {
                data = repo.GetInfoByType(type);
            }
            var re = new WebInfoModel()
            {
                HTMLstr = data.Description,
                FileId = data.FileId,
                Type = data.Type,
                FilePath = data.FilePath
            };
            return View(re);
        }

        [HttpPost]
        [MenuSync(ParentAction = "AllInfo", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 2, Description = "Edit", CssClass = "fa fa-money")]
        public ActionResult SaveWebInfo(WebInfoModel obj, HttpPostedFileBase myFile)
        {
            try
            {
                if (myFile != null && myFile.ContentLength >= (2024 * 1024))
                {
                    ModelState.AddModelError("", "File too big !");
                    return View("Details");
                }
                string error = string.Empty;
                int? newId = obj.FileId;
                #region SaveFile
                if (myFile != null)
                {
                    MemoryStream ms = new MemoryStream();
                    myFile.InputStream.CopyTo(ms);
                    var fileAtt = new FileManagement()
                    {
                        Id = obj.FileId ?? 0,
                        DisplayName = myFile.FileName
                    };
                    newId = FileManager.SaveFile(ms.ToArray(), fileAtt, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                if (!string.IsNullOrEmpty(error))
                {
                    ModelState.AddModelError("", error);
                    return View("Details");
                }
                if (newId.HasValue && newId.Value == 0)
                {
                    ModelState.AddModelError("", "Cannot save this file !");
                    return View("Details");
                }
                #endregion
                bool dt = false;
                using (var repo = new WebInfoManager())
                {
                    dt = repo.UpdateInfo(obj.Type, obj.HTMLstr??string.Empty, newId);
                }
                if (!dt)
                {
                    ModelState.AddModelError("", "Fail");
                    return View("Details");
                }
                return RedirectToAction("AllInfo");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View("Details");
            }
        }
    }
}
