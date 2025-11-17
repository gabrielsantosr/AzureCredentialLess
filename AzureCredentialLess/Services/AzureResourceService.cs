using Azure.Identity;

namespace AzureCredentialLess.Services
{
    public abstract class AzureResourceService
    {
        protected IAzureAuthService azureAuthService;
        protected AzureResourceService(IAzureAuthService azureAuthService)
        {
            this.azureAuthService = azureAuthService;
        }
    }
}
