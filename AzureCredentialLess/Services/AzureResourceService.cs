using Azure.Core;

namespace AzureCredentialLess.Services
{
    public abstract class AzureResourceService
    {
        private IAzureAuthService azureAuthService;
        protected AzureResourceService(IAzureAuthService azureAuthService)
        {
            this.azureAuthService = azureAuthService;
        }
        protected TokenCredential GetCredential(string tenantId) => azureAuthService.GetClientAssertionCredential(tenantId);

    }
}
