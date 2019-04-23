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
    public class CategoryManager : DataExtension, IDisposable
    {
        private WebCenterContext _context = new WebCenterContext();

        public CategoryManager()
        {
            cn = (SqlConnection)(_context.Database.Connection);
        }

        public void Dispose()
        {
            cn.Close();
        }

        public Category[] GetAllCategory(string keyword, out string error, bool? isEnable = null,int? parentId = null,bool? isMainTable = null,bool? isShowHomePage = null, bool? isShowLibrary = null, bool? isShowLeft = null)
        {
            try
            {
                error = string.Empty;
                var para = new Hashtable();
                para.Add("@Keyword", keyword);
                para.Add("@IsEnable", isEnable);
                para.Add("@ParentId", parentId);
                para.Add("@IsMainTable", isMainTable);
                para.Add("@IsShowHomePage", isShowHomePage);
                para.Add("@IsShowLibrary", isShowLibrary);
                para.Add("@IsShowLeft", isShowLeft);
                var dt = this.ExecuteStoreProcedure<Category>("Category_GetAll", para).ToArray();
                //string folder = HttpContext.Current.Server.MapPath("/ServerFile");
                string folder = Settings.FilePath;
                foreach (var item in dt)
                {
                    item.FilePath = folder + item.FilePath;
                }
                return dt;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public bool SaveCategory(Category entity, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                Category dt = null;
                if (entity.Id == 0)
                {
                    entity.CreatedUser = userId;
                    entity.CreatedDate = DateTime.Now;
                    dt = CreateCategory(entity, out error);
                }
                else
                {
                    entity.UpdatedUser = userId;
                    entity.UpdatedDate = DateTime.Now;
                    dt = UpdateCategory(entity, out error);
                }
                if (dt != null)
                    return true;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return false;
        }

        public Category GetDetailCategory(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Categories.FirstOrDefault(c => c.Id == id);
                if (dt != null)
                {
                    string folder = Settings.FilePath;
                    dt.FilePath = folder + _context.FileManagements.FirstOrDefault(c => c.Id == dt.FileId)?.Path;
                }
                return dt;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public bool ChangeStatusCategory(int id, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Categories.FirstOrDefault(c => c.Id == id);
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


        public bool DeleteCategory(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Categories.FirstOrDefault(c => c.Id == id);
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

        public Category CreateCategory(
            Category cate, out string error)
        {
            error = string.Empty;
            try
            {
                _context.Entry(cate).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return cate;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public Category UpdateCategory(Category cate, out string error)
        {
            error = string.Empty;
            try
            {
                var oldDt = _context.Categories.FirstOrDefault(c => c.Id == cate.Id);
                oldDt.Name = cate.Name;
                oldDt.IsActive = cate.IsActive;
                oldDt.IsPin = cate.IsPin;
                oldDt.IsShowHomePage = cate.IsShowHomePage;
                oldDt.IsShowLibrary = cate.IsShowLibrary;
                oldDt.IsShowLeft = cate.IsShowLeft;
                oldDt.Description = cate.Description;
                oldDt.UpdatedDate = cate.UpdatedDate;
                oldDt.UpdatedUser = cate.UpdatedUser;
                oldDt.FileId = cate.FileId;
                oldDt.ParentId = cate.ParentId;
                oldDt.Order = cate.Order;
                _context.Entry(oldDt).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
                return oldDt;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }
    }
}