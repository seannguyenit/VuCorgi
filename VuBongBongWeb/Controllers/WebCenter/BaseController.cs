using VuBongBongWeb.Management.WebCenterManagement;
using VuBongBongWeb.Security;
using MainLibrary.Entity.WebCenter;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VuBongBongWeb.Controllers.WebCenter
{
    [MenuSync(Description = "BaseController", CssClass = "fa fa-lg fa-fw fa-bar-chart-o", IsBindingWithParent = false, Level = 0, SyncOrder = 7, IsShownOnMenuBar = false)]
    public class BaseController : Controller
    {
        // GET: Base
        [Authentication]
        [HttpPost]
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 1, Description = "Savefile", CssClass = "fa fa-money", IsShownOnMenuBar = false)]
        public JsonResult Savefile(HttpPostedFileBase myFile, string type = null)
        {
            string located = @"/ServerFile";
            string url = string.Empty;
            string[] rs = { "Fail", "" };
            if (string.IsNullOrEmpty(type))
            {
                type = MainLibrary.Resource.WebCenter.FileType.Image.ToString();
            }
            try
            {
                if (myFile != null && myFile.ContentLength >= (3024 * 1024))
                {
                    rs[1] = "File too big !";
                    return Json(rs);
                }
                // TODO: Add update logic here
                string error = string.Empty;
                #region SaveFile
                MemoryStream ms = new MemoryStream();
                myFile.InputStream.CopyTo(ms);
                var fileAtt = new FileManagement()
                {
                    DisplayName = myFile.FileName
                };
                url = FileManager.SaveFileStr(ms.ToArray(), fileAtt, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error, type);
                if (!string.IsNullOrEmpty(error))
                {
                    rs[1] = error;
                    return Json(rs);
                }
                if (string.IsNullOrEmpty(url))
                {
                    rs[1] = "Cannot save this file !";
                    return Json(rs);
                }
                url = HttpContext.Request.Url.Authority + located + url.Replace(@"\", "/");
                if (!url.Contains("http"))
                {
                    url = @"http://" + url;
                }
                #endregion
            }
            catch (Exception ex)
            {
                rs[1] = ex.Message;
                return Json(rs);
            }
            return Json(url);
        }
    }
}