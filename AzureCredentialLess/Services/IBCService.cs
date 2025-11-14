using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public interface IBCService
    {
        Task<Result> Retrieve(BCQueryRequest request);
    }
}