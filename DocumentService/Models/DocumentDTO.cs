using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class DocumentDTO
    {
        public Guid CorrelationId { get; set; }
        public List<Document> Documents { get; set; }

        public DocumentDTO()
        {
            this.Documents = new List<Document>();
        }
    }
}
