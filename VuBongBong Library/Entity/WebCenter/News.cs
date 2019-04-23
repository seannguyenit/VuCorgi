using MainLibrary.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.WebCenter
{
    [Table("News")]
    public class News : BasicEntity
    {
        public News()
        {
            IsActive = true;
            IsDelete = false;
            FileInfo = new FileManagement();
            LstImg = new List<AlbumDetail>();
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Type { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDelete { get; set; }
        public int? FileId { get; set; }
        public int CateID { get; set; }
        public string Content { get; set; }
        [NotMapped]
        public string CateName { get; set; }
        [NotMapped]
        public string FilePath { get; set; }
        //[NotMapped]
        //public string Path { get; set; }
        [NotMapped]
        public string FEPath
        {
            get
            {
                if (!string.IsNullOrEmpty(FilePath)) return (string.Concat("..\\..", FilePath).Replace("\\", "/"));
                else return Library.Resource.ConstResource.S_DEFAULT_URL;
            }
        }
        [NotMapped]
        public FileManagement FileInfo { get; set; }
        //[NotMapped]
        //public string LastModifier { get; set; }
        [NotMapped]
        public string FileError { get; set; }
        [NotMapped]
        public List<AlbumDetail> LstImg { get; set; }
    }
}
