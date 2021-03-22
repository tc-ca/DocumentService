using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    /*Document_Info entity
     * Columns:
     * DESCRIPTION_TXT : STRING
     * FILE_SIZE_NBR : INT
     * LANGUAGE_TXT : STRING
     * FILE_NAME_NM : STRING
     * DOCUMENT_URL : STRING
     * CORRELATION_ID : GUID
     * DOCUMENT_TYPES : STRING, jsonb
     * SUBMISSION_METHOD_CD : STRING
     * FILE_TYPE_CD: STRING
     * DATE_CREATED_DTE : DATETIME
     * USER_CREATED_BY_ID : STRING
     * DATE_LAST_UPDATED_DTE : DATETIME
     * USER_LAST_UPDATED_BY_ID : STRING
     * IS_DELETED_IND : BOOL
     * DATE_DELETED_DTE : DATETIME
     * DELETED_BY_ID : STRING
     * DOCUMENT_ID : GUID
     * Correlation : navigation property
     */
    public class DocumentInfo
    {   

        
        public string DESCRIPTION_TXT { get; set; } //description of the file
        public int FILE_SIZE_NBR { get; set; } //size of the file
        public string LANGUAGE_TXT { get; set; } //language the file is in
        public string FILE_NAME_NM { get; set; } //name of the file
        public string DOCUMENT_URL { get; set; } //URL to the document
        public Guid CORRELATION_ID { get; set; }//foreign key
        [Column(TypeName = "jsonb")]
        public string DOCUMENT_TYPES { get; set; } //types of documents, stored in a JSONB type in postgres
        public string  SUBMISSION_METHOD_CD { get; set; } //How the file was submitted
        public string FILE_TYPE_CD { get; set; } //file type
        public DateTime DATE_CREATED_DTE { get; set; } //date the document was created
        public string USER_CREATED_BY_ID { get; set; } //user who created the document
        public DateTime DATE_LAST_UPDATED_DTE { get; set; } //date the file was modified
        public string USER_LAST_UPDATED_BY_ID { get; set; } //user who last updated the document
        public bool IS_DELETED_IND { get; set; } //flag to determine if file was removed or not (soft delete)
        public DateTime DATE_DELETED_DTE { get; set; } //date the file was removed
        public string DELETED_BY_ID { get; set; } //user who removed the file
        [Key]
        public Guid DOCUMENT_ID { get; set; }//primary key
        public Correlation Correlation { get; set; } //navigation property
    }
}
