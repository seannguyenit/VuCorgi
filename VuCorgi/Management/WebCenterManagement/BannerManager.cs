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
    public class BannerManager : DataExtension, IDisposable
    {
        private WebCenterContext _context = new WebCenterContext();

        public BannerManager()
        {
            cn = (SqlConnection)(_context.Database.Connection);
        }

        public void Dispose()
        {
            cn.Close();
        }

        public Banner[] GetAllBanner(string keyword, out string error, bool? isEnable = null)
        {
            try
            {
                error = string.Empty;
                var para = new Hashtable();
                para.Add("@Keyword", keyword);
                para.Add("@IsEnable", isEnable);
                var dt = this.ExecuteStoreProcedure<Banner>("Banner_GetAll", para).ToArray();
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

        public bool SaveBanner(Banner entity, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                Banner dt = null;
                if (entity.Id == 0)
                {
                    entity.CreatedUser = userId;
                    entity.CreatedDate = DateTime.Now;
                    dt = CreateBanner(entity, out error);
                }
                else
                {
                    entity.UpdatedUser = userId;
                    entity.UpdatedDate = DateTime.Now;
                    dt = UpdateBanner(entity, out error);
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

        public Banner GetDetailBanner(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Banners.FirstOrDefault(c => c.Id == id);
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

        public bool ChangeStatusBanner(int id, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Banners.FirstOrDefault(c => c.Id == id);
                dt.IsEnable = !dt.IsEnable;
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


        public bool DeleteBanner(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Banners.FirstOrDefault(c => c.Id == id);
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

        public Banner CreateBanner(
            Banner banner, out string error)
        {
            error = string.Empty;
            try
            {
                _context.Entry(banner).State = System.Data.Entity.EntityState.Added;
                _context.SaveChanges();
                return banner;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public Banner UpdateBanner(Banner banner, out string error)
        {
            error = string.Empty;
            try
            {
                var oldDt = _context.Banners.FirstOrDefault(c => c.Id == banner.Id);
                oldDt.Title = banner.Title;
                oldDt.TargetUrl = banner.TargetUrl;
                oldDt.Description = banner.Description;
                oldDt.UpdatedDate = banner.UpdatedDate;
                oldDt.UpdatedUser = banner.UpdatedUser;
                oldDt.FileId = banner.FileId;
                oldDt.Order = banner.Order;
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