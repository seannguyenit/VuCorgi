using MainLibrary.Entity.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MainLibrary.Entity.WebCenter
{
    [Table("FileManagement")]
    public class FileManagement : BasicEntity
    {
        public int Id { get; set; }
        public string Path { get; set; }
        public string DisplayName { get; set; }
        public string FileType { get; set; }
        //[NotMapped]
        //public byte[] File { get; set; }
        //public string TagetId { get; set; }
    }
}
