using log4net;
using MainLibrary.Entity.WebCenter;
using SeanAPI.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VuBongBongWeb.Management.WebCenterManagement
{
    public class RoleAndPermissionManager : DataExtension, IDisposable
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(WebUserManager));
        private WebCenterContext _context = new WebCenterContext();

        public RoleAndPermissionManager()
        {
            cn = (SqlConnection)(_context.Database.Connection);
        }

        public void Dispose()
        {
            cn.Close();
        }

        public Role[] GetAllRole()
        {
            try
            {
                var para = new Hashtable();
                var dt = this.ExecuteStoreProcedure<Role>("Role_GetAll", para).ToArray();
                return dt;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return new Role[0];
        }

        public Role GetRoleDetails(int id)
        {
            try
            {
                var dt = _context.Roles.FirstOrDefault(c => c.Id == id);
                if (dt != null)
                {
                    dt.MenuRoles = _context.MenuManagements.Where(c => c.RoleId == dt.Id).ToList();
                }
                return dt;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return new Role();
        }
        public Role CreateRole(Role role, out string error)
        {
            error = string.Empty;
            try
            {
                role.CreatedDate = DateTime.Now;
                _context.Entry(role).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                error = ex.Message;
            }
            return role;
        }

        public Role UpdateRole(Role role, out string error)
        {
            error = string.Empty;
            try
            {
                role.UpdatedDate = DateTime.Now;
                _context.Entry(role).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                error = ex.Message;
            }
            return role;
        }

        public bool ChangeStatusRole(int id, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Roles.FirstOrDefault(c => c.Id == id);
                dt.IsActive = !dt.IsActive;
                dt.UpdatedDate = DateTime.Now;
                dt.UpdatedUser = userId;
                _context.Entry(dt).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }


        public bool DeleteRole(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Roles.FirstOrDefault(c => c.Id == id);
                _context.Entry(dt).State = System.Data.Entity.EntityState.Deleted;
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }


        public MenuManagement[] GetAllMenuManagement()
        {
            try
            {
                var para = new Hashtable();
                var dt = this.ExecuteStoreProcedure<MenuManagement>("User_GetAll", para).ToArray();
                return dt;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
            }
            return new MenuManagement[0];
        }

        public bool SaveManagement(Dictionary<int, bool> menuManagement, int roleId, Guid user, out string error)
        {
            error = string.Empty;
            try
            {
                menuManagement = this.CorrectMenuPermission(menuManagement);
                foreach (var item in menuManagement)
                {
                    var menuPermission = _context.MenuManagements.FirstOrDefault(m => m.MenuItemId == item.Key && m.RoleId == roleId);
                    if (menuPermission != null)
                    {
                        menuPermission.IsActive = item.Value;
                        menuPermission.UpdatedUser = user;
                        menuPermission.UpdatedDate = DateTime.Now;
                        _context.Entry(menuPermission).State = System.Data.Entity.EntityState.Modified;
                        _context.SaveChanges();
                    }
                    else
                    {
                        menuPermission = new MenuManagement
                        {
                            MenuItemId = item.Key,
                            IsActive = item.Value,
                            CreatedUser = user,
                            RoleId = roleId,
                            CreatedDate = DateTime.Now
                        };
                        _context.Entry(menuPermission).State = System.Data.Entity.EntityState.Added;
                        _context.SaveChanges();
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message);
                error = ex.Message;
            }
            return false;
        }

        #region method helper

        /// <summary>
        ///     Check Again Menu Management List and re-assign IsActive value of Menu_Management
        ///     If Menu-Item has parent menu that is inactive then it will be set to inactive also
        /// </summary>
        /// <param name="menuManagement"></param>
        /// <returns></returns>
        private Dictionary<int, bool> CorrectMenuPermission(Dictionary<int, bool> menuManagement)
        {
            Dictionary<int, bool> copyMenuManagment = new Dictionary<int, bool>(menuManagement);
            Dictionary<int, bool> result = new Dictionary<int, bool>();
            foreach (var item in menuManagement)
            {
                Log.Debug($"Menu[{item.Key}] - value [{item.Value}]");
                result.Add(item.Key, item.Value);
                var menu = _context.MenuItems.FirstOrDefault(c => c.Id == item.Key);
                var parentId = menu?.ParentId;
                if (parentId.HasValue)
                {
                    if (!copyMenuManagment[parentId.Value])
                    {
                        var menuIdsInSameLevel = _context.MenuItems.Where(s => s.ParentId == parentId.Value).Select(s => s.Id);
                        foreach (var id in menuIdsInSameLevel)
                        {
                            Log.Debug($"Change Menu[{id}] to False");
                            copyMenuManagment[id] = false;
                        }
                        Log.Debug($"Change Menu[{item.Key}] to False");
                        result[item.Key] = false;
                    }
                }
            }
            return result;
        }


        #endregion

    }
}