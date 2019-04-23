using Library;
using Library.Resource;
using log4net;
using MainLibrary.Entity.WebCenter;
using SeanAPI.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Web;

namespace VuBongBongWeb.Management.WebCenterManagement
{
    public class WebUserManager : DataExtension, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WebUserManager));
        private WebCenterContext _context = new WebCenterContext();

        public WebUserManager()
        {
            cn = (SqlConnection)(_context.Database.Connection);
        }

        public void Dispose()
        {
            cn.Close();
        }

        public User GetUserByUserName(string username, string password, out string error, bool isRemember = false)
        {
            error = string.Empty;
            try
            {
                Utils helper = new Utils();
                string token = string.Empty;
                string roleName = string.Empty;
                int roleId = -1;
                DateTime start = DateTime.Now;
                DateTime end = DateTime.Now;
                var user = _context.Users.FirstOrDefault(c => c.UserName == username && c.IsActive);
                if (user != null && helper.VerifyMd5Hash(password, user.Password) && user.IsActive)
                {
                    CreateLoginSession(user.UserId, isRemember, ref error, ref token, ref roleName, ref roleId, ref start, ref end);
                }
                else
                {
                    error = "Username and password are incorrect !";
                    return null;
                }
                user.Token = token;
                user.RoleId = roleId;
                user.RoleName = roleName;
                user.Session_Start = start;
                user.Session_End = end;
                return user;
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetInnerMessage());
                error = ex.Message;
            }
            return null;
        }

        public User CreateUser(User user)
        {
            try
            {
                user.IsActive = true;
                user.IsAdmin = false;
                Utils helper = new Utils();
                user.Password = helper.HashMd5(user.Password);
                _context.Entry(user).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return user;
            }
            catch (DbEntityValidationException ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// created user by Guest with no check username
        /// </summary>
        /// <param name="user"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public User CreateGuestUser(User user, out string error, out string createToken)
        {
            error = string.Empty;
            createToken = string.Empty;
            try
            {
                user.IsActive = false;
                user.IsAdmin = false;
                user.UserId = Guid.NewGuid();
                user.CreatedDate = DateTime.Now;
                Utils helper = new Utils();
                user.Password = helper.HashMd5(user.Password);
                user.CreateToken = createToken = helper.HashMd5(user.UserName + "/" + user.UserId + "/" + DateTime.Now.Date.ToString());
                _context.Entry(user).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return user;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public User[] GetAllUser(string keyword, out string error, bool? isEnable = null)
        {
            try
            {
                error = string.Empty;
                var para = new Hashtable();
                para.Add("@Keyword", keyword);
                para.Add("@IsEnable", isEnable);
                var dt = this.ExecuteStoreProcedure<User>("User_GetAll", para).ToArray();
                return dt;
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetInnerMessage());
                error = ex.Message;
            }
            return null;
        }

        public User GetUserDetails(Guid id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Users.FirstOrDefault(c => c.UserId == id);
                return dt;
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetInnerMessage());
                error = ex.Message;
            }
            return null;
        }

        public User UpdateUser(User user, Guid editer, out string error)
        {
            error = string.Empty;
            try
            {
                var oldUser = _context.Users.FirstOrDefault(c => c.UserId == user.UserId);
                if ((user.UserName.ToUpper().Trim() != oldUser.UserName.ToUpper().Trim()) && (!CheckOKUsername(user.UserName)))
                {
                    error = "Your username is existed !";
                    return null;
                }
                oldUser.UserName = user.UserName;
                oldUser.IsAdmin = user.IsAdmin;
                oldUser.IsActive = user.IsActive;
                _context.Entry(oldUser).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                if (user.RoleId.HasValue) UpdateUserRole(oldUser.UserId, user.RoleId.Value, editer);
                return oldUser;
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetInnerMessage());
                error = ex.Message;
            }
            return null;
        }

        public User CreateUserFromAdmin(User user, Guid editer, out string error)
        {
            error = string.Empty;
            try
            {
                if (!CheckOKUsername(user.UserName))
                {
                    error = "Your username is existed !";
                    return null;
                }
                Utils helper = new Utils();
                user.Password = helper.HashMd5(ConstResource.S_ORIGINAL_PASSWORD);
                user.CreatedDate = DateTime.Now;
                if (user.UserId == Guid.Empty) user.UserId = Guid.NewGuid();
                _context.Entry(user).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                if (user.RoleId.HasValue) UpdateUserRole(user.UserId, user.RoleId.Value, editer);
                return user;
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetInnerMessage());
                error = ex.Message;
            }
            return null;
        }

        public bool ChangeStatusUser(Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Users.FirstOrDefault(c => c.UserId == userId);
                dt.IsActive = !dt.IsActive;
                _context.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetInnerMessage());
                error = ex.Message;
            }
            return false;
        }
        public bool CheckToken(string token)
        {
            try
            {
                var dt = _context.Users.FirstOrDefault(c => c.CreatedDate.HasValue && c.CreateToken == token && !c.IsActive);
                if (dt != null && ((DateTime.Now - dt.CreatedDate.Value).TotalHours <= 24))
                    return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetInnerMessage());
            }
            return false;
        }
        public bool CheckOKUsername(string username)
        {
            try
            {
                var dt = _context.Users.Any(c => c.UserName.ToUpper().Trim() == username.ToUpper().Trim());
                return !dt;
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetInnerMessage());
            }
            return false;
        }
        public bool CheckOKEmail(string email)
        {
            try
            {
                var dt = _context.Users.Any(c => c.Email.ToUpper().Trim() == email.ToUpper().Trim());
                return !dt;
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetInnerMessage());
            }
            return false;
        }
        public bool ChangePassword(Guid userId, string oldPass, string newpass, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Users.FirstOrDefault(c => c.UserId == userId);
                Utils helper = new Utils();
                if (helper.VerifyMd5Hash(oldPass, dt.Password))
                {
                    dt.Password = helper.HashMd5(newpass);
                    _context.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    error = "Current password is incorrect !";
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetInnerMessage());
                error = ex.Message;
            }
            return false;
        }

        public bool ResetPassword(Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Users.FirstOrDefault(c => c.UserId == userId);
                Utils helper = new Utils();
                dt.Password = helper.HashMd5(ConstResource.S_ORIGINAL_PASSWORD);
                _context.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetInnerMessage());
                error = ex.Message;
            }
            return false;
        }

        public bool ActiveUserByToken(string token, out string error)
        {
            error = string.Empty;
            try
            {
                var dt = _context.Users.FirstOrDefault(c => c.CreatedDate.HasValue && c.CreateToken == token && !c.IsActive);
                if (dt != null && ((DateTime.Now - dt.CreatedDate.Value).TotalHours <= 24))
                {
                    dt.IsActive = true;
                    dt.Token = string.Empty;
                    _context.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                    _context.SaveChanges();
                    return true;
                }
                else
                {
                    error = "Tài khoản không tồn tại.";
                }
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        public bool DeleteUser(Guid id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Users.FirstOrDefault(c => c.UserId == id);
                _context.Entry(dt).State = System.Data.Entity.EntityState.Deleted;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.GetInnerMessage());
                error = ex.Message;
            }
            return false;
        }

        public bool UpdateUserRole(Guid userId, int roleId, Guid editer)
        {
            try
            {
                var checkExisted = _context.UserRoles.FirstOrDefault(c => c.UserId == userId);
                if (checkExisted != null)
                {
                    checkExisted.RoleId = roleId;
                    checkExisted.UpdatedDate = DateTime.Now;
                    checkExisted.UpdatedUser = editer;
                    _context.Entry(checkExisted).State = System.Data.Entity.EntityState.Modified;
                }
                else
                {
                    checkExisted = new UserRole();
                    checkExisted.UserId = userId;
                    checkExisted.RoleId = roleId;
                    checkExisted.CreatedDate = DateTime.Now;
                    checkExisted.CreatedUser = editer;
                    _context.Entry(checkExisted).State = System.Data.Entity.EntityState.Added;
                }
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return false;
        }

        #region private method
        private bool CreateLoginSession(Guid userId, bool isRemember, ref string message, ref string token, ref string roleName, ref int roleId, ref DateTime start, ref DateTime end)
        {
            bool status = false;
            // Create new Session For Employee
            User_Session user_Session = _context.User_Sessions.FirstOrDefault(c => c.UserId == userId);
            if (user_Session != null)
            {
                user_Session.User_Token = Convert.ToBase64String(Guid.NewGuid().ToByteArray());
                user_Session.User_Start = DateTime.Now;
                var sessionEnd = DateTime.Now.AddMinutes(ConstResource.I_LOGIN_SESSION_MINUTES);
                if (isRemember)
                {
                    sessionEnd = DateTime.Now.AddMinutes(ConstResource.I_LOGIN_SESSION_MINUTES * ConstResource.I_REMEMBER_SESSION_MINUTES_MULTIPLIER_TIMES);
                }
                user_Session.User_End = sessionEnd;

                _context.Entry(user_Session).State = System.Data.Entity.EntityState.Modified;
            }
            else
            {
                // First Time Login
                user_Session = new User_Session()
                {
                    UserId = userId,
                    User_Start = DateTime.Now,
                    User_End = DateTime.Now.AddMinutes(ConstResource.I_LOGIN_SESSION_MINUTES)
                };
                _context.Entry(user_Session).State = System.Data.Entity.EntityState.Added;
            }
            var userRole = _context.UserRoles.FirstOrDefault(e => e.UserId.Equals(userId) && e.IsActive);
            if (userRole != null)
            {
                var role = _context.Roles.FirstOrDefault(r => r.Id == userRole.RoleId);
                // Get Avatar Image From Rydiam2012.dbo.[PolisherDetail].[Avatar]
                roleName = role?.RoleName;
                roleId = userRole.RoleId;
            }
            else
            {
                roleId = -1;
                roleName = string.Empty;
            }
            //else
            //{
            //    status = false;
            //    message = "Employee still not set Role.";
            //    Log.Error($"Employee[{employeeId}] still not set Role.");
            //}
            //avatar = this.ReadAvatarImage(employeeId);
            _context.SaveChanges();
            status = true;
            message = "Login Success";
            token = user_Session.User_Token;
            start = user_Session.User_Start;
            end = user_Session.User_End;
            //Log.Info($"Login User[{employeeId}] Success");
            return status;
        }

        #endregion
    }
}