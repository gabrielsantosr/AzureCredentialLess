using Azure.Core;
using Azure.Identity;
using AzureCredentialLess.Classes;

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
