using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public class KeyVaultService : AzureResourceService, IKeyVaultService
    {
        public KeyVaultService(IAzureAuthService azureAuthService) : base(azureAuthService)
        {
        }

        public async Task<KeyVaultSecret> GetSecret(KeyVaultSecretRequest request)
        {
            var client = GetSecretClient(request);
            var response = await client.GetSecretAsync(request.SecretName);
            if (!response.HasValue)
            {
                throw new Exception(response.GetRawResponse().Content.ToString());
            }
            return response.Value;
        }
        private SecretClient GetSecretClient(KeyVaultRequest request)
        {
            var vaultURI = string.Format("https://{0}.vault.azure.net/", request.KeyVaultName);
            TokenCredential credential = GetCredential(request);
            return new SecretClient(new Uri(vaultURI), credential);
        }


    }
}
