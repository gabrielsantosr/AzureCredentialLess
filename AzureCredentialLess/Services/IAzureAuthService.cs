using Azure.Identity;

namespace AzureCredentialLess.Services
{
    public interface IAzureAuthService
    {
        Task<string> GetAppRegistrationCredentialLessToken(string tanantId, string resource);
        ClientAssertionCredential GetClientAssertionCredential(string tenantId);
        Task<string> GetManagedIdentityToken(string resource);
        ManagedIdentityCredential IdentityCredential { get; }
    }
}
