using MainLibrary.Entity.Base;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainLibrary.Entity.WebCenter
{
    [Table("Category")]
    public class Category : BasicEntity
    {
        public Category()
        {
            FileInfo = new FileManagement();
            IsActive = true;
            IsPin = false;
            Order = 0;
            Child = new List<News>();
        }

        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public bool IsPin { get; set; }
        public bool IsShowHomePage { get; set; }
        public bool IsShowLibrary { get; set; }
        public bool IsShowLeft { get; set; }
        public int Order { get; set; }
        public int? FileId { get; set; }
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
        public List<News> Child { get; set; }

    }
}
