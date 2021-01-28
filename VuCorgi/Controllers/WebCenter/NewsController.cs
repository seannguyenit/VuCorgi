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
    [MenuSync(Description = "Tin và Ảnh", CssClass = "fa fa-lg fa-fw fa-bar-chart-o", IsBindingWithParent = false, Level = 0, SyncOrder = 8)]
    public class NewsController : Controller
    {
        #region Back-End
        //[Authentication]
        //[MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 1, Description = "Danh sách Tin", CssClass = "fa fa-info")]
        //public ActionResult NewsMain()
        //{
        //    var listCate = new List<Category>();
        //    using (var repo = new CategoryManager())
        //    {
        //        listCate = repo.GetAllCategory(string.Empty, out string error, true).ToList();
        //    }
        //    listCate.Insert(0, new Category() { Id = 0, Name = "Tất cả danh mục" });
        //    ViewBag.Cate = listCate.Select(f => new SelectListItem
        //    {
        //        Value = f.Id.ToString(),
        //        Text = f.Name,
        //        Selected = (listCate.FirstOrDefault() ?? new Category()).Id == f.Id
        //    });
        //    return View();
        //}

        [Authentication]
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 1, Description = "Danh sách tin tức", CssClass = "fa fa-info")]
        public ActionResult AlbumMain()
        {

            var listCate = new List<Category>();
            using (var repo = new CategoryManager())
            {
                listCate = repo.GetAllCategory(string.Empty, out string error, true).ToList();
            }
            listCate.Insert(0, new Category() { Id = 0, Name = "Tất cả danh mục" });
            ViewBag.Cate = listCate.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Name,
                Selected = f.Id == 0
            });
            return View();
        }

        [Authentication]
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 2, Description = "Danh sách sản phẩm", CssClass = "fa fa-info")]
        public ActionResult ProductMain()
        {

            var listCate = new List<Category>();
            using (var repo = new CategoryManager())
            {
                listCate = repo.GetAllCategory(string.Empty, out string error, true).ToList();
            }
            listCate.Insert(0, new Category() { Id = 0, Name = "Tất cả danh mục" });
            ViewBag.Cate = listCate.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Name,
                Selected = f.Id == 0
            });
            return View();
        }
        [Authentication]
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 3, Description = "Thư viện hình ảnh", CssClass = "fa fa-info")]
        public ActionResult ImageMain()
        {
            var data = new AlbumDetail[0];
            using (var _manager = new NewsAndAlbumManager())
            {
                data = _manager.GetAllAlbumDetail(null, out string error,true,true);
            }
            return View(data);
        }

        [Authentication]
        [MenuSync(ParentAction = "AlbumMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 1, Description = "List Image")]
        public ActionResult AlbumImagePartial(int id, int parentId,bool isPin = false)
        {
            var listCate = new List<Category>();
            var data = new AlbumDetail();
            if (id != 0)
            {
                using (var _manager = new NewsAndAlbumManager())
                {
                    data = _manager.GetDetailAlbum(id, out string error);
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
                Selected = data?.CatId == f.Id
            });
            ViewBag.parent = parentId;
            ViewBag.IsPin = isPin;
            return View(data);
        }

        [Authentication]
        [MenuSync(ParentAction = "AlbumMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 2, Description = "Image Details")]
        public ActionResult AlbumDetails(int id)
        {
            var data = new AlbumDetail[0];
            using (var _manager = new NewsAndAlbumManager())
            {
                data = _manager.GetAllAlbumDetail(id, out string error);
            }
            ViewBag.Parent = id;
            return View(data);
        }

        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "AlbumMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 3, Description = "Save List Image")]
        public ActionResult SaveAlbumImage(AlbumDetail collection, HttpPostedFileBase myFile)
        {
            try
            {
               

                string error = string.Empty;
                int newId = collection.FileId;
                #region SaveFile
                if (newId == 0 && myFile == null)
                {
                    ModelState.AddModelError("", "Please input image !");
                    return View("AlbumImagePartial", collection);
                }
                if (myFile != null)
                {
                    MemoryStream ms = new MemoryStream();
                    myFile.InputStream.CopyTo(ms);
                    var fileAtt = new FileManagement()
                    {
                        Id = collection.FileId,
                        DisplayName = myFile.FileName
                    };
                    newId = FileManager.SaveFile(ms.ToArray(), fileAtt, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                if (!string.IsNullOrEmpty(error))
                {
                    ModelState.AddModelError("", error);
                    return View("AlbumImagePartial", collection);
                }
                if (newId == 0)
                {
                    ModelState.AddModelError("", "Cannot save this file !");
                    return View("AlbumImagePartial", collection);
                }
                #endregion

                #region Save Banner
                collection.FileId = newId;
                bool dt = false;
                using (var repo = new NewsAndAlbumManager())
                {
                    dt = repo.SaveAlbumDetail(collection, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                if (!dt)
                {
                    ModelState.AddModelError("", error);
                    return View("AlbumImagePartial", collection);
                }
                #endregion
                if (collection.AlbumId != 0)
                {
                    return RedirectToAction("AlbumDetails", new { id = collection.AlbumId });
                }
                else
                {
                    return RedirectToAction("ImageMain");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View("AlbumImagePartial", collection);
            }
        }

        // POST: News/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "AlbumMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 4, Description = "Delete", CssClass = "fa fa-money")]
        public ActionResult DeleteImage(int id, int parentId)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new NewsAndAlbumManager())
                {
                    dt = repo.DeleteAlbumDetail(id, out error);
                }
                return RedirectToAction("AlbumDetails", new { id = parentId });
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return RedirectToAction("AlbumDetails", new { id = parentId });
            }
        }

        public ActionResult NewsPatialSearch(string keyword = null, string from = null, string to = null, int? cateId = null, string type = null)
        {
            DateTime? fromDate = Utils.ConvertStrToDateTime(from, null);
            DateTime? toDate = Utils.ConvertStrToDateTime(to, null);
            var currentUser = ((WebPrincipal)HttpContext.User).UserDetail.UserId;
            var data = new News[0];
            using (var _manager = new NewsAndAlbumManager())
            {
                data = _manager.GetAllNews(keyword, currentUser, null, toDate.Value, out string error, true, cateId, type);
            }
            ViewBag.Type = type;
            return PartialView(data);
        }


        [Authentication]
        [MenuSync(ParentAction = "AlbumMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 5, Description = "Details")]
        public ActionResult Details(int? id, string type = null)
        {
            var dt = new News();
            var listCate = new List<Category>();
            string error = string.Empty;
            if (id.HasValue && id.Value != 0)
            {
                using (var repo = new NewsAndAlbumManager())
                {
                    dt = repo.GetDetailNews(id.Value, out error);
                }
            }
            using (var repo = new CategoryManager())
            {
                listCate = repo.GetAllCategory(string.Empty, out error, true).ToList();
            }
            listCate.Insert(0, new Category() { Id = 0, Name = "Root" });
            ViewBag.Cate = listCate.Select(f => new SelectListItem
            {
                Value = f.Id.ToString(),
                Text = f.Name,
                Selected = dt?.CateID == f.Id
            });
            ViewBag.TypeNews = dt?.Type ?? type;
            return View(dt);
        }

        // POST: News/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "AlbumMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 6, Description = "Edit", CssClass = "fa fa-money")]
        public ActionResult Details(NewsModel collection, HttpPostedFileBase myFile, bool? isProduct)
        {
            try
            {
                if (isProduct ?? true)
                {
                    collection.Type = MainLibrary.Resource.WebCenter.NewsType.Product.ToString();
                }
                else
                {
                    collection.Type = MainLibrary.Resource.WebCenter.NewsType.News.ToString();
                }
                var listCate = new List<Category>();
                using (var repo = new CategoryManager())
                {
                    listCate = repo.GetAllCategory(string.Empty, out string error2, true).ToList();
                }
                listCate.Insert(0, new Category() { Id = 0, Name = "Root" });
                ViewBag.Cate = listCate.Select(f => new SelectListItem
                {
                    Value = f.Id.ToString(),
                    Text = f.Name,
                    Selected = collection?.CateID == f.Id
                });
                #region Validate

                if (string.IsNullOrEmpty(collection.Title))
                {
                    ModelState.AddModelError("", "Nhập tên !");
                    return View(collection.GetOriginal());
                }
                if (string.IsNullOrEmpty(collection.Description))
                {
                    ModelState.AddModelError("", "Nhập mô tả !");
                    return View(collection.GetOriginal());
                }
                if (myFile != null && myFile.ContentLength >= (2024 * 1024))
                {
                    ModelState.AddModelError("", "File quá lớn (< 2Mb) !");
                    return View(collection.GetOriginal());
                }

                #endregion

                string error = string.Empty;
                int newId = collection.FileId ?? 0;
                #region SaveFile
                if (newId == 0 && myFile == null)
                {
                    ModelState.AddModelError("", "Nhập file !");
                    return View(collection.GetOriginal());
                }
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
                    return View(collection.GetOriginal());
                }
                if (newId == 0)
                {
                    ModelState.AddModelError("", "Cannot save this file !");
                    return View(collection.GetOriginal());
                }
                #endregion

                #region Save News
                collection.FileId = newId;
                //collection.Type = NewsType.News.ToString();
                bool dt = false;
                var newsE = new News()
                {
                    CateID = collection.CateID,
                    Content = collection.HTMLstr ?? string.Empty,
                    Order = collection.Order,
                    CreatedDate = collection.CreatedDate,
                    CreatedUser = collection.CreatedUser,
                    Description = collection.Description,
                    FileId = collection.FileId,
                    Id = collection.Id,
                    IsActive = collection.IsActive,
                    Title = collection.Title,
                    Type = collection.Type,
                    UpdatedDate = collection.UpdatedDate,
                    UpdatedUser = collection.UpdatedUser
                };
                using (var repo = new NewsAndAlbumManager())
                {
                    dt = repo.SaveNews(newsE, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                if (!dt)
                {
                    ModelState.AddModelError("", error);
                    return View(collection.GetOriginal());
                }
                #endregion
                if (collection.Type == MainLibrary.Resource.WebCenter.NewsType.Product.ToString())
                {
                    return RedirectToAction("ProductMain");
                }
                return RedirectToAction("AlbumMain");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View(collection.GetOriginal());
            }
        }

        // POST: News/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "AlbumMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 7, Description = "ChangeStatus", CssClass = "fa fa-money")]
        public JsonResult ChangeStatus(int id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new NewsAndAlbumManager())
                {
                    dt = repo.ChangeStatusNews(id, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json("Fail");
            }
        }

        // POST: News/Edit/5
        [HttpPost]
        [Authentication]
        [MenuSync(ParentAction = "AlbumMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 8, Description = "Delete", CssClass = "fa fa-money")]
        public JsonResult Delete(int id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new NewsAndAlbumManager())
                {
                    dt = repo.DeleteNews(id, out error);
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
        public ActionResult Chitiet(int id)
        {
            var dt = new News();
            var relatedNews = new News[0];
            var listCate = new List<Category>();
            string error = string.Empty;
            if (id != 0)
            {
                using (var repo = new NewsAndAlbumManager())
                {
                    dt = repo.GetDetailNews(id, out error);
                    if (dt != null)
                    {
                        relatedNews = repo.GetAllNews(string.Empty, null, null, null, out error, cateId: dt.CateID, isEnable: true).OrderBy(c => c.CreatedDate).Take(3).ToArray();
                        dt.LstImg = repo.GetAllAlbumDetail(dt.Id, out error, true).ToList();
                    }
                }

            }
            ViewBag.Related = relatedNews;
            return View(dt);
        }

        [HttpPost]
        public ActionResult TimKiem(string key)
        {
            var dtDefault = new News[0];
            using (var repo = new NewsAndAlbumManager())
            {
                if (dtDefault != null)
                {
                    dtDefault = repo.GetAllNews(key, null, null, null, out string error).OrderBy(c => c.CreatedDate).ToArray();
                }
            }
            return View(dtDefault ?? new News[0]);
        }
        #endregion
    }
}