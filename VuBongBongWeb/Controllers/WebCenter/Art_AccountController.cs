using Library.Resource;
using VuBongBongWeb.Management.WebCenterManagement;
using VuBongBongWeb.Security;
using MainLibrary.Entity.WebCenter;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace VuBongBongWeb.Controllers.BE
{
    //[Authentication]
    [MenuSync(IsShownOnMenuBar = false, Description = "Account", CssClass = "fa fa-lg fa-fw fa-bar-chart-o", IsBindingWithParent = false, Level = 0, SyncOrder = 6)]
    public class Art_AccountController : Controller
    {
        // TODO: This should be moved to the constructor of the controller in combination with a DependencyResolver setup
        // NOTE: You can use NuGet to find a strategy for the various IoC packages out there (i.e. StructureMap.MVC5)
        //private readonly UserManager _manager = UserManager.Instance;

        // GET: /account/forgotpassword
        //[AllowAnonymous]
        public ActionResult ForgotPassword()
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();

            return View();
        }

        // GET: /account/login
        //[AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();

            // Store the originating URL so we can attach it to a form field
            var viewModel = new AccountLoginModel { ReturnUrl = returnUrl };

            return View(viewModel);
        }

        // POST: /account/login
        [HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(AccountLoginModel viewModel)
        {
            // Ensure we have a valid viewModel to work with
            if (!ModelState.IsValid)
                return View(viewModel);

            //var data = new Hashtable();
            //data.Add(CommonResources.Resource.K_EMPLOYEE_CODE, viewModel.EmployeeCode);
            //data.Add(CommonResources.Resource.K_EMPLOYEE_PASSWORD, viewModel.Password);
            //data.Add(CommonResources.Resource.K_IS_REMEMBER_ME, viewModel.RememberMe);
            string message = string.Empty;
            try
            {
                // Verify if a user exists with the provided identity information and sign-in user
                var userReturn = await Task.Run(() =>
                {
                    using (var repo = new WebUserManager())
                    {
                        var dt = repo.GetUserByUserName(viewModel.UserName, viewModel.Password, out string error, viewModel.RememberMe);
                        message = error;
                        return dt;
                    }
                });

                if (userReturn != null && userReturn.UserId != Guid.Empty)
                {
                    User user = userReturn;
                    int roleId = user.RoleId ?? 0;
                    string roleName = user.RoleName;
                    string token = user.Token;
                    if (userReturn.IsAdmin) roleId = -1;
                    var menuResult = await Task.Run(() =>
                    {
                        using (var repo = new WebBaseManager())
                        {
                            return repo.GetMenuItemListByRole(roleId);
                        }
                    });
                    if (menuResult != null)
                    {
                        //string jsonDataString = (string)menuResult.Data[CommonResources.Resource.K_HASHTABLE_DATA];
                        //var menuItems = JsonConvert.DeserializeObject<IEnumerable<MenuItem>>(jsonDataString);
                        var menuItems = menuResult;
                        int? currentOrder = null;
                        //MenuItem firstMenu = null;
                        var menuSort = menuItems.SortMenuItem(menuItems, ref currentOrder).Where(m => m.IsActive && !m.IsPageBlank && !m.IsBindingWithParent && m.MenuAction != null && m.MenuController != null && m.MenuAction != string.Empty && m.MenuController != string.Empty);
                        var mainPage = menuSort.FirstOrDefault(c => c.IsMainPage);
                        var guestPage = menuSort.FirstOrDefault(c => c.IsAllowGuest);
                        MenuItem firstMenu = mainPage ?? (menuSort.FirstOrDefault() ?? guestPage);
                        //if(menuItems.Count()>0) //TODO first null
                        //    firstMenu = menuItems.SortMenuItem(menuItems, ref currentOrder).Where(m => m.IsActive && !m.IsPageBlank && !m.IsBindingWithParent).FirstOrDefault(m => m.MenuAction != null && m.MenuController != null && m.MenuAction != string.Empty && m.MenuController != string.Empty);
                        //else
                        //    firstMenu = menuItems.Where(m => m.IsActive && !m.IsPageBlank && !m.IsBindingWithParent).FirstOrDefault(m => m.MenuAction != null && m.MenuController != null && m.MenuAction != string.Empty && m.MenuController != string.Empty);
                        if (firstMenu != null)
                        {
                            Session["DefaultUrl"] = new KeyValuePair<string, string>(firstMenu.MenuAction, firstMenu.MenuController);
                            if (string.IsNullOrWhiteSpace(viewModel.ReturnUrl))
                            {
                                var redirectUrl = Url.Action(firstMenu.MenuAction, firstMenu.MenuController);
                                viewModel.ReturnUrl = redirectUrl;
                            }

                            // Store login information
                            var um = UserManager.Instance;
                            Response.Cookies.Add(um.CreateAuthenticationTicket(
                                user, roleId, roleName, token,
                                string.Empty,
                                string.Empty));

                            Response.Cookies.Add(um.CreateEmployeeInfoTicket(user));

                            Session[Resource.K_USER_MENU_LIST.ToString()] = menuItems;

                            //return RedirectToAction(firstMenu.MenuAction, firstMenu.MenuController);
                        }
                        else // If role don't have any permission to access any action then AddModelError
                        {
                            //var userModel = System.Web.HttpContext.Current.User as WebCenterPrincipal;
                            message = $"You don't have permission to use any function.";
                            ModelState.AddModelError("", message);
                            return View(viewModel);
                        }
                        //Log.Info($"Save Session For Menu List of RoleId[{roleId}] From Ip[{Request.UserHostAddress}]. Count Menu[{menuItems.Count()}]");
                    }

                    GC.Collect();
                    // If the user came from a specific page, redirect back to it
                    return RedirectToLocal(viewModel.ReturnUrl);
                }
                else
                {
                    message = "Login fail !";
                }
            }
            catch (System.Exception ex)
            {
                message = ex.Message;
            }
            // If a user was found

            // No existing user was found that matched the given criteria
            ModelState.AddModelError("", message);

            // If we got this far, something failed, redisplay form
            return View(viewModel);
        }

        // GET: /account/error
        //[AllowAnonymous]
        public ActionResult Error()
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();

            return View();
        }

        [AllowAnonymous]
        public ActionResult Error404()
        {
            return View();
        }

        // GET: /account/register
        //[AllowAnonymous]
        [Authentication]
        [MenuSync(IsShownOnMenuBar = false, IsBindingWithParent = false, Level = 1, SyncOrder = 1, Description = "Change Password", CssClass = "fa fa-money")]
        public ActionResult ChangePassword()
        {
            // We do not want to use any existing identity information
            //EnsureLoggedOut();
            var currentUser = ((WebPrincipal)HttpContext.User).UserDetail;

            return View(new AccountResetPasswordModel()
            {
                UserId = currentUser.UserId,
                UserName = currentUser.UserName
            });
        }

        // POST: /account/register
        [HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public async Task<ActionResult> ChangePassword(AccountResetPasswordModel viewModel)
        {
            // Ensure we have a valid viewModel to work with
            if (!ModelState.IsValid)
                return View(viewModel);

            //// Prepare the identity with the provided information
            //var user = new User
            //{
            //    UserName = viewModel.UserName ?? viewModel.UserName,
            //    Password = viewModel.Password ?? viewModel.Password
            //};

            // Try to create a user with the given identity
            try
            {
                string exErr = string.Empty;
                try
                {
                    var result = await Task.Run(() =>
                    {
                        var rs = false;
                        using (var repo = new WebUserManager())
                        {
                            rs = repo.ChangePassword(viewModel.UserId, viewModel.OldPassword, viewModel.Password, out exErr);
                        }
                        return rs;
                    });
                }
                catch (Exception ex)
                {
                    exErr = ex.Message;
                }

                // If the user could not be created
                if (!string.IsNullOrEmpty(exErr))
                {
                    ModelState.AddModelError("", exErr);
                    return View(viewModel);
                }

                // If the user was able to be created we can sign it in immediately
                // Note: Consider using the email verification proces
                //await SignInAsync(user, false);

                return Logout();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(viewModel);
            }
        }

        // POST: /account/Logout
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            // First we clean the authentication ticket like always
            FormsAuthentication.SignOut();

            // Second we clear the principal to ensure the user does not retain any authentication
            HttpContext.User = new WebPrincipal(string.Empty);

            // Last we redirect to a controller/action that requires authentication to ensure a redirect takes place
            // this clears the Request.IsAuthenticated flag since this triggers a new request
            return RedirectToLocal();
        }

        private ActionResult RedirectToLocal(string returnUrl = "")
        {
            // If the return url starts with a slash "/" we assume it belongs to our site
            // so we will redirect to this "action"
            if (!returnUrl.IsNullOrWhiteSpace() && Url.IsLocalUrl(returnUrl))
                return Redirect(returnUrl);

            // If we cannot verify if the url is local to our host we redirect to a default location
            return RedirectToAction("Login", "Art_Account");
        }

        private void AddErrors(DbEntityValidationException exc)
        {
            foreach (var error in exc.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors.Select(validationError => validationError.ErrorMessage)))
            {
                ModelState.AddModelError("", error);
            }
        }

        private void AddErrors(IdentityResult result)
        {
            // Add all errors that were returned to the page error collection
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            if (Request.IsAuthenticated)
                Logout();
        }

        //private async Task SignInAsync(User user, bool isPersistent)
        //{
        //    // Clear any lingering authencation data
        //    FormsAuthentication.SignOut();

        //    // Create a claims based identity for the current user
        //    var identity = await _manager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);

        //    // Write the authentication cookie
        //    FormsAuthentication.SetAuthCookie(identity.Name, isPersistent);
        //}

        // GET: /account/lock
        //[AllowAnonymous]
        public ActionResult Lock()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult ActiveAccount(string token)
        {
            bool tokenOk = false;
            string error = string.Empty;
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Error404");
            }
            using (var repo = new WebUserManager())
            {
                tokenOk = repo.CheckToken(token);
            }
            if (!tokenOk)
            {
                return RedirectToAction("Error404");
            }
            else
            {
                using (var repo = new WebUserManager())
                {
                    tokenOk = repo.ActiveUserByToken(token, out error);
                }
            }
            ViewBag.Message = string.Empty;
            if (tokenOk)
            {
                ViewBag.Message = "Tài khoản của bạn đã được kích hoạt thành công.";
            }
            else
            {
                ViewBag.Message = "Tài khoản của bạn đã được kích hoạt thất bại." + error;
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult RegisterCompleted()
        {
            return View();
        }


        [AllowAnonymous]
        public ActionResult Register(string returnUrl)
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();

            // Store the originating URL so we can attach it to a form field
            var viewModel = new AccountRegistrationModel { ReturnUrl = returnUrl };

            return View(viewModel);
        }

        [HttpPost]
        public ActionResult Register(AccountRegistrationModel collection, string birthdate)
        {
            if (!string.IsNullOrEmpty(birthdate))
            {
                DateTime.TryParse(birthdate, out DateTime rs);
                collection.DOB = rs;
            }
            // Ensure we have a valid viewModel to work with
            if (!ModelState.IsValid)
                return View(collection);

            try
            {
                string exErr = string.Empty;
                try
                {
                    User data = null;
                    bool rs = false;
                    using (var repo = new WebUserManager())
                    {
                        rs = repo.CheckOKUsername(collection.UserName);
                    }
                    if (!rs)
                    {
                        ModelState.AddModelError("", "Tên đăng nhập đã tồn tại !");
                        return View(collection);
                    }
                    using (var repo = new WebUserManager())
                    {
                        rs = repo.CheckOKEmail(collection.Email);
                    }
                    if (!rs)
                    {
                        ModelState.AddModelError("", "Email đã đã được sử dụng !");
                        return View(collection);
                    }
                    string token = string.Empty;
                    using (var repo = new WebUserManager())
                    {
                        data = repo.CreateGuestUser(new MainLibrary.Entity.WebCenter.User
                        {
                            City = collection.City,
                            Address = collection.Address,
                            Company = collection.Company,
                            DOB = collection.DOB,
                            Email = collection.Email,
                            FullName = collection.FullName,
                            Password = collection.Password,
                            Phone = collection.Phone,
                            UserName = collection.UserName,
                            Slogan = collection.Slogan
                        }, out exErr, out token);
                    }
                    if (data == null)
                    {
                        ModelState.AddModelError("", exErr);
                        return View(collection);
                    }
                    var urlBuilder = new System.UriBuilder(Request.Url.AbsoluteUri)
                    {
                        Path = Url.Action("ActiveAccount", new { token }),
                        Query = null,
                    };
                    //Uri uri = urlBuilder.Uri;
                    string urlComplete = urlBuilder.ToString();
                    Task.Run(() =>
                    {
                        MailManager.SendRegisterEmail(data, urlComplete, out exErr);
                    });
                    return RedirectToAction("RegisterCompleted");
                }
                catch (Exception ex)
                {
                    exErr = ex.Message;
                }

                // If the user could not be created
                if (!string.IsNullOrEmpty(exErr))
                {
                    ModelState.AddModelError("", exErr);
                    return View(collection);
                }

                // If the user was able to be created we can sign it in immediately
                // Note: Consider using the email verification proces
                //await SignInAsync(user, false);

                return Logout();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(collection);
            }
        }
    }
}