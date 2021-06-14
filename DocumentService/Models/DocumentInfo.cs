using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentService.Models
{
    //defines the Document info table
    [Table("DOCUMENT_INFO")]
    public class DocumentInfo
    {
        /// <summary>
        /// gets or sets the description of the file
        /// </summary>
        [Column("DESCRIPTION_TXT")]
        public string Description { get; set; } 

        /// <summary>
        /// gets or sets the size of the file
        /// </summary>
        [Column("FILE_SIZE_NBR")]
        public long FileSize { get; set; } 

        /// <summary>
        /// gets or sets the language the file is in
        /// </summary>
        [Column("LANGUAGE_TXT")]
        public string Language { get; set; }

        /// <summary>
        /// gets or sets the name of the file
        /// </summary>
        [Column("FILE_NAME_NM")]
        public string FileName { get; set; } 

        /// <summary>
        /// gets or sets the URL to the document
        /// </summary>
        [Column("DOCUMENT_URL")]
        public string DocumentUrl { get; set; } 

        /// <summary>
        /// gets or sets the foreign key
        /// </summary>
        [Column("CORRELATION_ID")]
        public Guid? CorrelationId { get; set; }

        /// <summary>
        /// gets or sets types of documents, stored in a JSONB type in postgres
        /// </summary>
        [Column("DOCUMENT_TYPES", TypeName = "jsonb")]
        public List<DocumentType> DocumentTypes { get; set; } 

        /// <summary>
        /// gets or sets the information on how the file was submitted
        /// </summary>
        [Column("SUBMISSION_METHOD_CD")]
        public string  SubmissionMethod { get; set; } 

        /// <summary>
        /// gets or sets the file type
        /// </summary>
        [Column("FILE_TYPE_CD")]
        public string FileType { get; set; } 

        /// <summary>
        /// gets or sets the date the document was created
        /// </summary>
        [Column("DATE_CREATED_DTE")]
        public DateTime? DateCreated { get; set; } 

        /// <summary>
        /// gets or sets the user who created the document
        /// </summary>
        [Column("USER_CREATED_BY_ID")]
        public string UserCreatedById { get; set; } 

        /// <summary>
        /// gets or sets the date the file was modified
        /// </summary>
        [Column("DATE_LAST_UPDATED_DTE")]
        public DateTime? DateLastUpdated { get; set; } 

        /// <summary>
        /// gets or sets the user who last updated the document
        /// </summary>
        [Column("USER_LAST_UPDATED_BY_ID")]
        public string UserLastUpdatedById { get; set; } 

        /// <summary>
        /// gets or sets the flag to determine if file was removed or not (soft delete)
        /// </summary>
        [Column("IS_DELETED_IND")]
        public bool IsDeleted { get; set; } 

        /// <summary>
        /// gets or sets the date the file was removed
        /// </summary>
        [Column("DATE_DELETED_DTE")]
        public DateTime? DateDeleted { get; set; } 

        /// <summary>
        /// gets or sets the user who removed the file
        /// </summary>
        [Column("DELETED_BY_ID")]
        public string DeletedById { get; set; }

        /// <summary>
        /// gets or sets this table's primary key
        /// </summary>
        [Column("DOCUMENT_ID")]
        [Key]
        public Guid DocumentId { get; set; }
        
        /// <summary>
        /// gets or sets the correlation information
        /// </summary>
        [ForeignKey("CorrelationId")]
        public virtual Correlation Correlation { get; set; }
        
        public override bool Equals(object obj)
        {
            return Equals(obj as DocumentInfo);
        }

        public bool Equals(DocumentInfo other)
        {
            return other != null &&
                   Description == other.Description &&
                   FileSize == other.FileSize &&
                   Language == other.Language &&
                   FileName == other.FileName &&
                   DocumentUrl == other.DocumentUrl &&
                   CorrelationId.Equals(other.CorrelationId) &&
                   EqualityComparer<List<DocumentType>>.Default.Equals(DocumentTypes, other.DocumentTypes) &&
                   SubmissionMethod == other.SubmissionMethod &&
                   FileType == other.FileType &&
                   DateCreated == other.DateCreated &&
                   UserCreatedById == other.UserCreatedById &&
                   DateLastUpdated == other.DateLastUpdated &&
                   UserLastUpdatedById == other.UserLastUpdatedById &&
                   IsDeleted == other.IsDeleted &&
                   DateDeleted == other.DateDeleted &&
                   DeletedById == other.DeletedById &&
                   DocumentId.Equals(other.DocumentId) &&
                   EqualityComparer<Correlation>.Default.Equals(Correlation, other.Correlation);
        }
    }
}
