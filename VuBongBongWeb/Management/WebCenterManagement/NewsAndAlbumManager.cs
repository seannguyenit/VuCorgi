using MainLibrary.Entity.WebCenter;
using MainLibrary.Resource.WebCenter;
using SeanAPI.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VuBongBongWeb.Management.WebCenterManagement
{
    public class NewsAndAlbumManager : DataExtension, IDisposable
    {
        private WebCenterContext _context = new WebCenterContext();

        public NewsAndAlbumManager()
        {
            cn = (SqlConnection)(_context.Database.Connection);
        }

        public void Dispose()
        {
            cn.Close();
        }

        #region News

        public News[] GetAllNews(string keyword, Guid? user, DateTime? from, DateTime? to, out string error, bool? isEnable = null, int? cateId = null, string type = null)
        {
            try
            {
                //if (string.IsNullOrEmpty(type)) type = NewsType.News.ToString();
                error = string.Empty;
                var para = new Hashtable();
                para.Add("@Keyword", keyword);
                para.Add("@From", from);
                para.Add("@To", to);
                para.Add("@User", user);
                para.Add("@IsActive", isEnable);
                para.Add("@CateId", cateId);
                para.Add("@Type", type);
                var dt = this.ExecuteStoreProcedure<News>("News_GetAll", para).ToArray();
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

        public bool SaveNews(News entity, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                News dt = null;
                if (entity.Id == 0)
                {
                    entity.CreatedUser = userId;
                    entity.CreatedDate = DateTime.Now;
                    dt = CreateNews(entity, out error);
                }
                else
                {
                    entity.UpdatedUser = userId;
                    entity.UpdatedDate = DateTime.Now;
                    dt = UpdateNews(entity, out error);
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

        public News GetDetailNews(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Newss.FirstOrDefault(c => c.Id == id);
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

        public bool ChangeStatusNews(int id, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Newss.FirstOrDefault(c => c.Id == id);
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


        public bool DeleteNews(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.Newss.FirstOrDefault(c => c.Id == id);
                dt.IsDelete = true;
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

        public News CreateNews(
            News banner, out string error)
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

        public News UpdateNews(News news, out string error)
        {
            error = string.Empty;
            try
            {
                var oldDt = _context.Newss.FirstOrDefault(c => c.Id == news.Id);
                oldDt.Title = news.Title;
                oldDt.Description = news.Description;
                oldDt.Content = news.Content;
                oldDt.CateID = news.CateID;
                oldDt.IsActive = news.IsActive;
                oldDt.Description = news.Description;
                oldDt.UpdatedDate = news.UpdatedDate;
                oldDt.UpdatedUser = news.UpdatedUser;
                oldDt.FileId = news.FileId;
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
        #endregion

        #region Album


        public AlbumDetail[] GetAllAlbumDetail(int? id, out string error, bool? isEnable = null,bool? isPin = null)
        {
            try
            {
                error = string.Empty;
                var para = new Hashtable();
                para.Add("@AlbumId", id);
                para.Add("@IsActive", isEnable);
                para.Add("@IsPin", isPin);
                var dt = this.ExecuteStoreProcedure<AlbumDetail>("AlbumDetail_GetAll", para).ToArray();
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

        public bool SaveAlbumDetail(AlbumDetail entity, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                AlbumDetail dt = null;
                if (entity.Id == 0)
                {
                    entity.CreatedUser = userId;
                    entity.CreatedDate = DateTime.Now;
                    dt = CreateAlbumDetail(entity, out error);
                }
                else
                {
                    entity.UpdatedUser = userId;
                    entity.UpdatedDate = DateTime.Now;
                    dt = UpdateAlbumDetail(entity, out error);
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

        public AlbumDetail GetDetailAlbum(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.AlbumDetails.FirstOrDefault(c => c.Id == id);
                if (dt != null)
                {
                    string folder = Settings.FilePath;
                    dt.FilePath = folder + _context.FileManagements.FirstOrDefault(c => c.Id == dt.FileId)?.Path ?? string.Empty;
                }
                return dt;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public bool ChangeStatusAlbumDetail(int id, Guid userId, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.AlbumDetails.FirstOrDefault(c => c.Id == id);
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


        public bool DeleteAlbumDetail(int id, out string error)
        {
            try
            {
                error = string.Empty;
                var dt = _context.AlbumDetails.FirstOrDefault(c => c.Id == id);
                dt.IsDelete = true;
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

        public AlbumDetail CreateAlbumDetail(
            AlbumDetail banner, out string error)
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

        public AlbumDetail UpdateAlbumDetail(AlbumDetail banner, out string error)
        {
            error = string.Empty;
            try
            {
                var oldDt = _context.AlbumDetails.FirstOrDefault(c => c.Id == banner.Id);
                oldDt.Title = banner.Title;
                oldDt.Description = banner.Description;
                oldDt.UpdatedDate = banner.UpdatedDate;
                oldDt.UpdatedUser = banner.UpdatedUser;
                oldDt.IsPin = banner.IsPin;
                oldDt.FileId = banner.FileId;
                //oldDt.Order = banner.Order;
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

        #endregion
    }
}