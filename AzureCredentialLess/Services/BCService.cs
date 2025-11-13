using AzureCredentialLess.Classes;
using Microsoft.Extensions.Logging;

namespace AzureCredentialLess.Services
{

    public class BCService : DynamicsCRUDService, IBCService
    {
        ILogger<BCService> logger { get; init; }
        public BCService(ILogger<BCService> logger, IAzureAuthService azureAuthService) : base(azureAuthService.GetCredentialLessToken)
        {
            this.logger = logger;
        }

        public Task<Result> Get(BCQueryRequest request)
        {
            return base.Get(request.TenantId, url, url + "v2.0/" + request.EnvironmentName + "/api/v2.0/" + request.ODataQuery);
        }

        const string url = "https://api.businesscentral.dynamics.com/";




    }
}
