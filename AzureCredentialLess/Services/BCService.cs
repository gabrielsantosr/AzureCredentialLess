using AzureCredentialLess.Classes;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace AzureCredentialLess.Services
{

    public class BCService : DynamicsCRUDService,IBCService
    {
        ILogger<BCService> logger { get; init; }
        public BCService(ILogger<BCService> logger, IAzureAuthService azureAuthService):base(azureAuthService.GetCredentialLessToken)
        {
            this.logger = logger;
        }

        public Task<Result> Get(string tenantId, string environment, string query)
        {
            return base.Get(tenantId, url, url + "v2.0/" + environment + "/api/v2.0/" + query);
        }
        const string url = "https://api.businesscentral.dynamics.com/";




    }
}
