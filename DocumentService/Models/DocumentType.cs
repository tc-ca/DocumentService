using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class DocumentType
    {
        public int Document_Type_code { get; set; }
        public Guid Document_Id { get; set; }
        public DateTime Date_Created { get; set; }
        public DateTime Date_Last_Updated { get; set; }
        public int User_Last_Updated_By_Id { get; set; }
        public int User_Created_By_Id { get; set; }
    }
}
