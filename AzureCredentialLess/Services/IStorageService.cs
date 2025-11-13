using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public interface IStorageService
    {
        Task<List<BlobDetail>> GetBlobsDetails(BlobCollectionRequest request);
    }
}