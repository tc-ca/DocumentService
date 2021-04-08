using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class Document
    {
        public string FileName { get; set; }
        public DocumentTypes DocumentType { get; set; }
        public int DocumentSize { get; set; }
        public byte[] DocumentImage { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string Language { get; set; }
        public string RequesterId { get; set; }

    }
}
