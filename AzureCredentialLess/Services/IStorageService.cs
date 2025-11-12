
using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public interface IStorageService
    {
        Task<string> GetBlobEtag(BlobRequest blobRequest);
    }
}