using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class UploadFileParameters
    {
        public string FileName { get; set; }
        public Stream FileStream { get; set; }
        public string Container { get; set; } = null;
    }
}
