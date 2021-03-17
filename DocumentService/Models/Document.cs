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
        public Guid Document_Id { get; set; }

        [Required]
        public Byte[] Document_Image { get; set; }
    }
}