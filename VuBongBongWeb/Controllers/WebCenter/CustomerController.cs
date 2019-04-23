using VuBongBongWeb.Management.WebCenterManagement;
using MainLibrary.Entity.WebCenter;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VuBongBongWeb.Security;
using Library;
using System.IO;

namespace VuBongBongWeb.Controllers.WebCenter
{
    [MenuSync(Description = "Khách hàng", CssClass = "fa fa-lg fa-fw fa-bar-chart-o", IsBindingWithParent = false, Level = 0, SyncOrder = 8)]
    public class CustomerController : Controller
    {
        #region Back-End
        [Authentication]
        [MenuSync(IsBindingWithParent = false, Level = 1, SyncOrder = 9, Description = "Feedback", CssClass = "fa fa-info")]
        public ActionResult FeedbackMain()
        {
            var data = new Customer_Feedback[0];
            using (var _manager = new CustomerManager())
            {
                data = _manager.GetAllCustomer_Feedback(string.Empty, out string error);
            }
            return View(data);
        }

        [MenuSync(ParentAction = "FeedbackMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 1, Description = "Details")]
        public ActionResult Details(int? id)
        {
            var dt = new Customer_Feedback();
            if (id.HasValue && id.Value != 0)
            {
                string error = string.Empty;
                using (var repo = new CustomerManager())
                {
                    dt = repo.GetDetailCustomer_Feedback(id.Value, out error);
                }
            }
            return View(dt);
        }

        // POST: Customer_Feedback/Edit/5
        [HttpPost]
        [MenuSync(ParentAction = "FeedbackMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 2, Description = "Edit", CssClass = "fa fa-money")]
        public ActionResult SaveCustomer_Feedback(Customer_Feedback collection, HttpPostedFileBase myFile)
        {
            try
            {
                // TODO: Add update logic here
                #region Validate

                if (string.IsNullOrEmpty(collection.Title))
                {
                    ModelState.AddModelError("", "Title is required !");
                    return View("Details", collection);
                }
                if (string.IsNullOrEmpty(collection.Content))
                {
                    ModelState.AddModelError("", "content is required !");
                    return View("Details", collection);
                }
                if (myFile != null && myFile.ContentLength >= (3024 * 1024))
                {
                    ModelState.AddModelError("", "File too big !");
                    return View("Details", collection);
                }
                #endregion

                string error = string.Empty;
                int newId = collection.FileId ?? 0;
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
                if (newId == 0)
                {
                    ModelState.AddModelError("", "Cannot save this file !");
                    return View("Details", collection);
                }
                #endregion

                #region Save Customer_Feedback
                collection.FileId = newId;
                bool dt = false;
                using (var repo = new CustomerManager())
                {
                    dt = repo.SaveCustomer_Feedback(collection, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                if (!dt)
                {
                    ModelState.AddModelError("", error);
                    return View("Details", collection);
                }
                #endregion
                return RedirectToAction("FeedbackMain");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                return View("Details", collection);
            }
        }

        // POST: Customer_Feedback/Edit/5
        [HttpPost]
        [MenuSync(ParentAction = "FeedbackMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 3, Description = "ChangeStatus", CssClass = "fa fa-money")]
        public JsonResult ChangeStatus(int id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new CustomerManager())
                {
                    dt = repo.ChangeStatusCustomer_Feedback(id, ((WebPrincipal)HttpContext.User).UserDetail.UserId, out error);
                }
                return Json("Success");
            }
            catch (Exception e)
            {
                return Json("Fail");
            }
        }

        // POST: Customer_Feedback/Edit/5
        [HttpPost]
        [MenuSync(ParentAction = "FeedbackMain", IsShownOnMenuBar = false, IsBindingWithParent = true, Level = 2, SyncOrder = 4, Description = "Delete", CssClass = "fa fa-money")]
        public JsonResult Delete(int id)
        {
            try
            {
                // TODO: Add update logic here
                string error = string.Empty;
                bool dt = false;
                using (var repo = new CustomerManager())
                {
                    dt = repo.DeleteCustomer_Feedback(id, out error);
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

        public ActionResult Partial_Feedback()
        {
            var data = new Customer_Feedback[0];
            using (var _manager = new CustomerManager())
            {
                data = _manager.GetAllCustomer_Feedback(string.Empty, out string error);
            }
            return PartialView(data);
        }

        #endregion

    }
}