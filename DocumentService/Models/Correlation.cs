using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class Correlation
    {
        public Guid CorrelationId { get; set; }
        public bool TransactionCompleteInd { get; set; }
        public string UserLastUpdatedById { get; set; }
        public DateTime DateLastUpdatedDte { get; set; }
        public string UserCreatedById { get; set; }
        public DateTime DateCreatedDte { get; set; }
    }
}
