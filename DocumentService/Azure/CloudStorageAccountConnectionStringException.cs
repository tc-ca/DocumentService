namespace DocumentService.Azure
{
    using System;

    /// <summary>
    /// Defines the cloude storage account connection string exception.
    /// </summary>
    public class CloudStorageAccountConnectionStringException : Exception
    {
        private static readonly string ErrorMessage = "Invalid connection string provided";

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudStorageAccountConnectionStringException"/> class.
        /// </summary>
        public CloudStorageAccountConnectionStringException()
            : base(CloudStorageAccountConnectionStringException.ErrorMessage)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudStorageAccountConnectionStringException"/> class.
        /// </summary>
        /// <param name="innerException">The inner exception.</param>
        public CloudStorageAccountConnectionStringException(Exception innerException)
            : base(CloudStorageAccountConnectionStringException.ErrorMessage, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudStorageAccountConnectionStringException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public CloudStorageAccountConnectionStringException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CloudStorageAccountConnectionStringException"/> class.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public CloudStorageAccountConnectionStringException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}