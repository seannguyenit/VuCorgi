using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace MainLibrary.Entity.Base
{
    public class BasicEntity
    {
        public Guid CreatedUser { get; set; }

        public DateTime CreatedDate { get; set; }

        public Guid? UpdatedUser { get; set; }

        public DateTime? UpdatedDate { get; set; }
        [NotMapped]
        public DateTime LastModified { get { return UpdatedDate ?? CreatedDate; } }
        [NotMapped]
        public string Editer { get; set; }
    }
}
