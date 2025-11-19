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
        // If tenant is null, it returnaccess the vault with the managed identity, otherwise, with the service principal on behalf of the managed identity
        /// <summary>
        /// If <see cref="AzureRequest.TenantId"/> is null, it returns <see cref="ManagedIdentityCredential"/> to connect managed identity &#x2192; resource<br/>
        /// Otherwise, it returns a <see cref="ClientAssertionCredential"/> to connect managed identity &#x2192; app registration &#x2192; service principal &#x2192; resource
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        protected TokenCredential GetCredential(AzureRequest request) {
            return request.TenantId is null ? azureAuthService.IdentityCredential : azureAuthService.GetClientAssertionCredential(request.TenantId);
        }
    }
}
