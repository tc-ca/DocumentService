using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class Document
    {
        /*
         * Primary key
         */
        [Key]
        public Guid DocumentId { get; set; }

        [Required]
        public Byte[] DocumentImage { get; set; }
       /* public virtual DocumentInfo DocumentInfo { get; set; }
        public virtual DocumentType DocumentType { get; set; }*/

    }
}