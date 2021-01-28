using MainLibrary.Entity.WebCenter;
using SeanAPI.DAL;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;

namespace VuBongBongWeb.Management.WebCenterManagement
{
    public class FileManager : DataExtension, IDisposable
    {
        private WebCenterContext _context = new WebCenterContext();

        public FileManager()
        {
            cn = (SqlConnection)(_context.Database.Connection);
        }

        public void Dispose()
        {
            cn.Close();
        }
        public FileManagement GetFileInfoById(int id, out string error)
        {
            error = string.Empty;
            try
            {
                var data = _context.FileManagements.FirstOrDefault(c => c.Id == id);
                //if (data != null)
                //{
                //    data.File = ReadFile(data.Path, out error);
                //}
                return data;
            }
            catch (Exception ex)
            {
                error = ex.Message;
            }
            return null;
        }

        public FileManagement InsertNew(FileManagement fileInfo)
        {
            _context.Entry(fileInfo).State = System.Data.Entity.EntityState.Added;
            _context.SaveChanges();
            return fileInfo;
        }

        public FileManagement Update(FileManagement fileInfo)
        {
            var oldData = _context.FileManagements.FirstOrDefault(c => c.Id == fileInfo.Id);
            oldData.UpdatedDate = fileInfo.UpdatedDate;
            oldData.UpdatedUser = fileInfo.UpdatedUser;
            oldData.Path = fileInfo.Path;
            oldData.FileType = fileInfo.FileType;
            _context.Entry(oldData).State = System.Data.Entity.EntityState.Modified;
            _context.SaveChanges();
            return oldData;
        }

        #region Support Method

        //public static FileManagement GetFile(int id, out string error)
        //{
        //    FileManagement obj = null;
        //    using (var context = new FileManager())
        //    {
        //        obj = context.GetFileInfoById(id, out error);
        //    }
        //    return obj;
        //}

        /// <summary>
        /// save and return Id
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileInfo"></param>
        /// <param name="userId"></param>
        /// <param name="error"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static int SaveFile(byte[] file, FileManagement fileInfo, Guid userId, out string error, string type = "banner")
        {
            string path = string.Empty;
            int newId = 0;
            error = string.Empty;
            try
            {
                string folder = HttpContext.Current.Server.MapPath("/ServerFile");
                //String FolderPath = System.IO.Directory.GetCurrentDirectory();
                string folderTime = string.Concat(@"\", DateTime.Now.Year, @"\", DateTime.Now.Day, "-", DateTime.Now.Month, @"\");
                string basicPath = string.Concat(folder, folderTime);
                string nameSave = DateTime.Now.Ticks.ToString() + fileInfo.DisplayName;
                if (!Directory.Exists(basicPath))
                {
                    Directory.CreateDirectory(basicPath);
                }
                File.WriteAllBytes(basicPath + nameSave, file);
                path = folderTime + fileInfo.DisplayName;
                using (var context = new FileManager())
                {
                    if (fileInfo.Id == 0)
                    {
                        fileInfo.CreatedDate = DateTime.Now;
                        fileInfo.CreatedUser = userId;
                        fileInfo.Path = folderTime + nameSave;
                        fileInfo.FileType = type;
                        newId = context.InsertNew(fileInfo).Id;
                    }
                    else
                    {
                        fileInfo.UpdatedDate = DateTime.Now;
                        fileInfo.UpdatedUser = userId;
                        fileInfo.Path = folderTime + nameSave;
                        fileInfo.FileType = type;
                        newId = context.Update(fileInfo).Id;
                    }
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            return newId;
        }

        /// <summary>
        /// save and return path
        /// </summary>
        /// <param name="file"></param>
        /// <param name="fileInfo"></param>
        /// <param name="userId"></param>
        /// <param name="error"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string SaveFileStr(byte[] file, FileManagement fileInfo, Guid userId, out string error, string type = "banner")
        {
            string path = string.Empty;
            string pathReturn = string.Empty;
            error = string.Empty;
            string folder = HttpContext.Current.Server.MapPath("/ServerFile");
            try
            {
                //String FolderPath = System.IO.Directory.GetCurrentDirectory();
                string folderTime = string.Concat(@"\", DateTime.Now.Year, @"\", DateTime.Now.Day, "-", DateTime.Now.Month, @"\");
                string basicPath = string.Concat(folder, folderTime);
                string nameSave = DateTime.Now.Ticks.ToString() + fileInfo.DisplayName;
                if (!Directory.Exists(basicPath))
                {
                    Directory.CreateDirectory(basicPath);
                }
                File.WriteAllBytes(basicPath + nameSave, file);
                path = folderTime + fileInfo.DisplayName;
                using (var context = new FileManager())
                {
                    if (fileInfo.Id == 0)
                    {
                        fileInfo.CreatedDate = DateTime.Now;
                        fileInfo.CreatedUser = userId;
                        fileInfo.Path = folderTime + nameSave;
                        fileInfo.FileType = type;
                        pathReturn = context.InsertNew(fileInfo).Path;
                    }
                    else
                    {
                        fileInfo.UpdatedDate = DateTime.Now;
                        fileInfo.UpdatedUser = userId;
                        fileInfo.Path = folderTime + nameSave;
                        fileInfo.FileType = type;
                        pathReturn = context.Update(fileInfo).Path;
                    }
                }
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            return pathReturn;
        }

        public static byte[] ReadFile(string path, out string error)
        {
            error = string.Empty;
            byte[] file = null;
            try
            {
                string basicPath = string.Concat(Settings.FilePath, path);
                file = File.ReadAllBytes(basicPath);
            }
            catch (Exception e)
            {
                error = e.Message;
            }
            return file;
        }
        #endregion

    }
}