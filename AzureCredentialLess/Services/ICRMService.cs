using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public interface ICRMService
    {
        Task<Result> Get(DataverseQueryRequest request);
    }
}