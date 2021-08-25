using DocumentService.ServiceModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class DocumentDTO
    {
        public List<Document> Documents { get; set; } = new List<Document>();
    }
}
