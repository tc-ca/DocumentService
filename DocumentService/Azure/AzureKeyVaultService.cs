using System;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Configuration;

namespace DocumentService.Azure
{
    /// <summary>
    ///  Represents the Azure key vault service.
    /// </summary>
    public class AzureKeyVaultService : IKeyVaultService
    {
        private string dNs;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureKeyVaultService"/> class.
        /// </summary>
        /// <param name="dNS">DNS for the key vault.</param>
        public AzureKeyVaultService(IConfiguration configuration)
        {
            this.dNs = configuration.GetSection("AzureKeyVaultSettings")["KeyVaultServiceEndpoint"];
        }

        /// <summary>
        /// Gets a secret from azure key vault.
        /// </summary>
        /// <param name="secretName">Secret to get.</param>
        /// <returns>Secret value.</returns>
        public string GetSecretByName(string secretName)
        {

            var secret = GetSecretClient().GetSecretAsync(secretName).GetAwaiter().GetResult().Value;
            
            return secret.Value;

        }

        ///// <summary>
        ///// Get a list of secrets from the azure key vault.
        ///// </summary>
        ///// <returns>List of secrets.</returns>
        //public IEnumerable<SecretItem> GetListOfSecrets()
        //{
        //    IEnumerable<SecretItem> secrets = new Page<SecretItem>();

        //    using (var keyVaultClient = GetKeyVaultClient())
        //    {
        //        secrets = keyVaultClient.GetSecretsAsync(vaultBaseUrl: this.dNs).GetAwaiter().GetResult();
        //    }

        //    return secrets;
        //}

        /// <summary>
        /// Get the client
        /// </summary>
        /// <returns>Secret client</returns>
        public SecretClient GetSecretClient()
        {
            var kvUri = "https://kv-document-dev.vault.azure.net/";

            return new SecretClient(new Uri(this.dNs), new DefaultAzureCredential());
        }
    }
}
