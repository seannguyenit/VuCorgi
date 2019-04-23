using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MainLibrary.Entity.WebCenter
{
    [Table("WebInfomation")]
    public class WebInfomation
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        public int? FileId { get; set; }
        [NotMapped]
        public string FilePath { get; set; }
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
