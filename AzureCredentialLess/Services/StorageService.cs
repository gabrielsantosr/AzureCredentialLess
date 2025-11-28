using Azure.Storage.Blobs;
using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public class StorageService : AzureResourceService, IStorageService
    {
        public StorageService(IAzureAuthService azureAuthService) : base(azureAuthService) { }

        public async Task<List<BlobDetail>> GetBlobsDetails(BlobCollectionRequest request)
        {
            string blobURL = $"https://{request.Account}.blob.core.windows.net/{request.Container}";
            var credential = azureAuthService.GetCredential(request.TenantId);
            BlobContainerClient client = new BlobContainerClient(new Uri(blobURL), credential);
            var asyncBlobCollection = client.GetBlobsAsync(prefix: request.BlobsPrefix);
            List<BlobDetail> output = new();
            await foreach (var blob in asyncBlobCollection)
            {
                output.Add(new BlobDetail()
                {
                    Name = blob.Name,
                    Type = blob.Properties.BlobType.HasValue ? blob.Properties.BlobType.Value.ToString() : null,
                    SizeInBytes = blob.Properties.ContentLength.HasValue ? blob.Properties.ContentLength.Value : 0
                });
            }
            return output;
        }


    }
}
