using Azure.Security.KeyVault.Secrets;
using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public interface IKeyVaultService
    {
        Task<KeyVaultSecret> GetSecret(KeyVaultSecretRequest request);
    }
}