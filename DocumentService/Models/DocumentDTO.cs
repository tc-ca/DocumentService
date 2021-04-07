using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class DocumentDTO
    {//CorrelationId, document/file name, document type, document size, byte array, description, language, requested user
        public Guid CorrelationId { get; set; }
        public List<Document> Documents { get; set; }
    }
}
