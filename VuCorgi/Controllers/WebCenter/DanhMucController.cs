using MainLibrary.Entity.WebCenter;
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

    [MenuSync(Description = "Danh mục", CssClass = "fa fa-lg fa-fw fa-bar-chart-o", IsBindingWithParent = false, Level = 0, SyncOrder = 7)]
    public class DanhMucController : Controller
    {
        #region Back-End
        [Authentication]
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 1, Description = "Danh sách danh mục", CssClass = "fa fa-minus-square-o")]
        public ActionResult DanhMucMain()
        {
            var data = new Category[0];
            using (var _manager = new CategoryManager())
            {
                data = _manager.GetAllCategory(string.Empty, out string error);
            }
            return View(data);
        }
        [Authentication]
        [MenuSync(ParentAction = "DanhMucMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 1, Description = "Details")]
        public ActionResult Details(int? id)
        {
            var dt = new Category();
            var listCate = new List<Category>();
            if (id.HasValue && id.Value != 0)
            {
                string error = string.Empty;
                using (var repo = new CategoryManager())
                {
                    dt = repo.GetDetailCategory(id.Value, out error);
                }
            }
            using (var repo = new CategoryManager())
            {
                listCate = repo.GetAllCategory(string.Empty, out string error, true).ToList();
            }
            listCate.Insert(0, new Category() { Id = 0, Name = "Root" });
            ViewBag.Cate = listCate.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Name,
                Selected = dt?.ParentId == f.Id
            });
            return View(dt);
        }

        // POST: DanhMuc/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "DanhMucMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 2, Description = "Edit", CssClass = "fa fa-money")]
        public ActionResult SaveDanhMuc(CateModel param, HttpPostedFileBase myFile)
        {
            var collection = param.GetOriginal();
            collection.IsShowLibrary = true;
            try
            {
                #region Validate

                if (string.IsNullOrEmpty(collection.Name))
                {
                    ModelState.AddModelError("", "Name is required !");
                    return View("Details", collection);
                }
                if (myFile != null && myFile.ContentLength >= (2024 * 1024))
                {
                    ModelState.AddModelError("", "File too big !");
                    return View("Details", collection);
                }
                #endregion

                string error = string.Empty;
                int? newId = collection.FileId;
                #region SaveFile
                if (myFile != null)
                {
                    MemoryStream ms = new MemoryStream();
                    myFile.InputStream.CopyTo(ms);
                    var fileAtt = new FileManagement()
                    {
                        Id = collection.FileId ?? 0,
                        DisplayName = myFile.FileName
                    };
                    newId = FileManager.SaveFile(ms.ToArray(), fileAtt, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                if (!string.IsNullOrEmpty(error))
                {
                    ModelState.AddModelError("", error);
                    return View("Details", collection);
                }
                if (newId.HasValue && newId.Value == 0)
                {
                    ModelState.AddModelError("", "Cannot save this file !");
                    return View("Details", collection);
                }
                #endregion

                #region Save DanhMuc
                collection.FileId = newId;
                bool dt = false;
                using (var repo = new CategoryManager())
                {
                    dt = repo.SaveCategory(collection, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                if (!dt)
                {
                    ModelState.AddModelError("", error);
                    return View("Details", collection);
                }
                #endregion
                return RedirectToAction("DanhMucMain");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View("Details", collection);
            }
        }

        // POST: DanhMuc/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "DanhMucMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 3, Description = "ChangeStatus", CssClass = "fa fa-money")]
        public JsonResult ChangeStatus(int id)
        {
            try
            {
                string error = string.Empty;
                bool dt = false;
                using (var repo = new CategoryManager())
                {
                    dt = repo.ChangeStatusCategory(id, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json("Fail");
            }
        }

        // POST: DanhMuc/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "DanhMucMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 4, Description = "Delete", CssClass = "fa fa-money")]
        public JsonResult Delete(int id)
        {
            try
            {
                string error = string.Empty;
                bool dt = false;
                using (var repo = new CategoryManager())
                {
                    dt = repo.DeleteCategory(id, out error);
                }
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json("Fail");
            }
        }
        #endregion

        #region Front-End
        public ActionResult LeftMenu()
        {
            var data = new Category[0];
            Dictionary<Category, Category[]> valuePairs = new Dictionary<Category, Category[]>();
            using (var _manager = new CategoryManager())
            {
                data = _manager.GetAllCategory(string.Empty, out string error, true);
            }
            if (data != null && data.Any())
            {
                foreach (var d in data.Where(c => (c.ParentId ?? 0) == 0).OrderBy(o => o.Order))
                {
                    valuePairs.Add(d, data.Where(p => (p.ParentId ?? 0) == d.Id).ToArray());
                }
            }
            return PartialView(valuePairs);
        }

        public ActionResult Chitiet(int id)
        {
            var data = new Category();
            var news = new List<News>();
            var prod = new List<News>();
            string error = string.Empty;
            using (var _manager = new CategoryManager())
            {
                data = _manager.GetDetailCategory(id, out error);
            }
            if (data != null)
            {
                using (var repo = new NewsAndAlbumManager())
                {
                    prod = repo.GetAllNews(string.Empty, null, null, null, out error, true, cateId: data.Id,type:MainLibrary.Resource.WebCenter.NewsType.Product.ToString()).ToList();
                    news = repo.GetAllNews(string.Empty, null, null, null, out error, true, cateId: data.Id,type:MainLibrary.Resource.WebCenter.NewsType.News.ToString()).ToList();
                }
            }
            ViewBag.News = news ?? new List<News>();
            ViewBag.Prod = prod ?? new List<News>();
            return View(data);
        }
        #endregion
    }
}