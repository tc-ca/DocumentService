using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class UploadResults
    {
            public bool Uploaded { get; set; }
            public string FileName { get; set; }
            public string StoredFileName { get; set; }
            public int ErrorCode { get; set; }  
    }
}
