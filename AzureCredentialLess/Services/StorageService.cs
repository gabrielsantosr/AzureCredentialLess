using Azure.Identity;
using Azure.Storage.Blobs;
using AzureCredentialLess.Classes;
using static System.Net.WebRequestMethods;

namespace AzureCredentialLess.Services
{
    public class StorageService : IStorageService
    {
        private IAzureAuthService azureAuthService;
        public StorageService(IAzureAuthService azureAuthService)
        {
            this.azureAuthService = azureAuthService;
        }
        public async Task<string> GetBlobEtag(BlobRequest blobRequest)
        {
            string blobURL = $"https://{blobRequest.Account}.blob.core.windows.net/{blobRequest.Container}/{blobRequest.Blob}";
            var credential = azureAuthService.GetClientAssertionCredential(blobRequest.TenantId);
            BlobClient client = new BlobClient(new Uri(blobURL), credential);
            var props = await client.GetPropertiesAsync();
            return props.Value.ETag.ToString();

        }
    }
}
