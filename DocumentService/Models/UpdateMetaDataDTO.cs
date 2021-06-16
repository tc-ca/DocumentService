namespace DocumentService.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text.Json.Serialization;

    public class UpdateMetaDataDTO
    {
        [JsonPropertyName("documentId")]
        public Guid DocumentId { get; set; }

        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("fileName")]
        public string FileName { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("submissionMethod")]
        public string SubmissionMethod { get; set; }

        [JsonPropertyName("fileLanguage")]
        public string FileLanguage { get; set; }

        [JsonPropertyName("documentTypes")]
        public List<DocumentType> DocumentTypes { get; set; }

        [JsonPropertyName("customMetadata")]
        public string CustomMetadata { get; set; }
    }
}
