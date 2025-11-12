using Azure.Identity;

namespace AzureCredentialLess.Services
{
    public interface IAzureAuthService
    {
        Task<string> GetCredentialLessToken(string tanantId, string resource);
        ClientAssertionCredential GetClientAssertionCredential(string tenantId);
    }
}
