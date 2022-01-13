using System;

namespace DocumentService.Models
{
    public class DocumentDeleteDTO
    {
        public Guid Id { get; set; }
        public string UserName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
