namespace DocumentService.Tests.Common.Services
{
    using DocumentService.Models;
    using System;
    using System.Collections.Generic;
    using Xunit;

    public interface IDatabaseFixture : IDisposable, ICollectionFixture<DatabaseFixture>
    {
        /// <summary>
        /// Creates multiple new document info in the database.
        /// </summary>
        /// <param name="ids">The list of ids to create the <see cref="DocumentInfo"/> with.</param>
        /// <returns>The newly created list of <see cref="DocumentInfo"/></returns>
        IEnumerable<DocumentInfo> CreateListOfDocumentInfos(Guid[] ids);

        /// <summary>
        /// Adds a new <see cref="DocumentInfo"/> to the database.
        /// </summary>
        /// <param name="documentInfo">The <see cref="DocumentInfo"/> to add.</param>
        void InsertDocumentInfo(DocumentInfo documentInfo);
    }
}
