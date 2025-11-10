using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public interface IBCService
    {
        Task<Result> Get(string tenantId, string environment, string query);
    }
}