using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class DocumentInfo
    {
        public Guid Document_Id { get; set; }
        public string Description { get; set; }
        public int File_Size { get; set; }
        public string File_Name { get; set; }
        public string Language { get; set; }
        public string Document_URL { get; set; }
        public DateTime Date_Created { get; set; }

        public int User_Created_By_Id { get; set; }
        public DateTime Date_Last_Updated { get; set; }
        public int User_Last_Updated_By_Id { get; set; }

    }
}
