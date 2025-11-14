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

        public Task<Result> Retrieve(BCQueryRequest request)
        {
            string fullURL = string.Format("{0}v2.0/{1}/api/v2.0/{2}", resource, request.EnvironmentName, request.ODataQuery);
            return base.Retrieve(request.TenantId, resource, fullURL);
        }

        const string resource = "https://api.businesscentral.dynamics.com/";




    }
}
