using MainLibrary.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.WebCenter
{
    [Table("Customer_Feedback")]
    public class Customer_Feedback : BasicEntity
    {
        public Customer_Feedback()
        {
            this.IsActive = true;
        }

        public int Id { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public int? FileId { get; set; }
        public int Order { get; set; }
        public bool IsActive { get; set; }
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
    }
}
