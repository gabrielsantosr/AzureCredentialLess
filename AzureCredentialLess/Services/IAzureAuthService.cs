using Azure.Core;

namespace AzureCredentialLess.Services
{
    public interface IAzureAuthService
    {
        Task<string> GetToken(string tenantId, string resource);
        TokenCredential GetCredential(string tenantId);
    }
}
