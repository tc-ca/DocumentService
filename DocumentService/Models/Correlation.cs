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
        [Key]
        public Guid CORRELATION_ID { get; set; }
        public bool TRANSACTION_COMPLETE_IND { get; set; }
        public string USER_LAST_UPDATED_BY_ID { get; set; }
        public DateTime DATE_LAST_UPDATED_DTE { get; set; }
        public string USER_CREATED_BY_ID { get; set; }
        public DateTime DATE_CREATED_DTE { get; set; }
    }
}
