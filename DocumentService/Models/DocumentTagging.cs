using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class DocumentTagging
    {
        //WIP
        public enum tags
        {
            
        }
        public Guid DocumentTaggingId { get; set; }
        public tags? tag { get; set; }
        public Guid DocumentInfoId { get; set; }

    }
}
