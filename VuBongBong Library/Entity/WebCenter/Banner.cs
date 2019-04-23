using MainLibrary.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.WebCenter
{
    [Table("Banner")]
    public class Banner : BasicEntity
    {
        public Banner()
        {
            FileInfo = new FileManagement();
            IsEnable = true;
            Order = 0;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string TargetUrl { get; set; }
        public int FileId { get; set; }
        public int Order { get; set; }
        public bool IsEnable { get; set; }
        [NotMapped]
        public string FilePath { get; set; }
        [NotMapped]
        public FileManagement FileInfo { get; set; }
        [NotMapped]
        public string LastModifier { get; set; }
        [NotMapped]
        public string FileError { get; set; }
        [NotMapped]
        public string FEPath
        {
            get
            {
                if (!string.IsNullOrEmpty(FilePath)) return (string.Concat("..\\..", FilePath).Replace("\\", "/"));
                else return Library.Resource.ConstResource.S_DEFAULT_URL;
            }
        }
    }
}
