using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public interface ICRMService
    {
        Task<Result> Retrieve(DataverseQueryRequest request);
    }
}