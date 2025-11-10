using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public interface ICRMService
    {
        Task<Result> Get(string tenantId, string url, string query);
    }
}