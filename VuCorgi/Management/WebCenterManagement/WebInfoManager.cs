using MainLibrary.Entity.WebCenter;
using MainLibrary.Resource.WebCenter;
using SeanAPI.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace VuBongBongWeb.Management.WebCenterManagement
{
    public class WebInfoManager : DataExtension, IDisposable
    {
        private WebCenterContext _context = new WebCenterContext();

        public WebInfoManager()
        {
            cn = (SqlConnection)(_context.Database.Connection);
        }

        public void Dispose()
        {
            cn.Close();
        }

        public WebInfomation GetInfoByType(string info)
        {
            var data = new WebInfomation();
            try
            {
                data = _context.WebInfomations.FirstOrDefault(c => c.Type == info.ToString());
                if (data != null)
                {
                    if (data.FileId.HasValue && data.FileId.Value != 0)
                    {
                        string folder = Settings.FilePath;
                        string path = _context.FileManagements.FirstOrDefault(c => c.Id == data.FileId.Value).Path;
                        data.FilePath = folder + path;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return data;
        }

        public WebInfomation[] GetAllInfo()
        {
            var data = new WebInfomation[0];
            try
            {
                data = _context.WebInfomations.ToArray();
                if (data != null)
                {
                    foreach (var item in data)
                    {
                        if (item.FileId.HasValue && item.FileId.Value != 0)
                        {
                            string folder = Settings.FilePath;
                            string path = _context.FileManagements.FirstOrDefault(c => c.Id == item.FileId.Value).Path;
                            item.FilePath = folder + path;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return data;
        }

        public bool UpdateInfo(string info, string text, int? fileId)
        {
            var data = new WebInfomation();
            try
            {
                data = _context.WebInfomations.FirstOrDefault(c => c.Type == info.ToString());
                data.Description = text;
                if (fileId.HasValue && fileId.Value != 0)
                {
                    data.FileId = fileId.Value;
                }
                _context.Entry(data).State = System.Data.Entity.EntityState.Modified;
                _context.SaveChanges();
            }
            catch (Exception ex)
            {
                return false;
            }
            return true;
        }

    }
}