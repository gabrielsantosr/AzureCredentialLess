namespace AzureCredentialLess.Services
{
    public interface IAzureAuthService
    {
        Task<string> GetCredentialLessToken(string tanantId, string resource);
    }
}
