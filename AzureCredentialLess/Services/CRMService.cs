using AzureCredentialLess.Classes;
using Microsoft.Extensions.Logging;

namespace AzureCredentialLess.Services
{

    public class CRMService : DynamicsCRUDService, ICRMService
    {
        ILogger<CRMService> logger { get; init; }
        public CRMService(ILogger<CRMService> logger, IAzureAuthService azureAuthService) : base(azureAuthService.GetCredentialLessToken)
        {
            this.logger = logger;
        }

        public new Task<Result> Get(DataverseQueryRequest request)
        {
            string url = (request.EnvironmentUrl ?? string.Empty);
            if (!url.EndsWith("/"))
            {
                url += "/";
            }
            return base.Get(request.TenantId, url, url + "api/data/v9.2/" + request.ODataQuery);
        }
    }
}
