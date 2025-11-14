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

        public new Task<Result> Retrieve(DataverseQueryRequest request)
        {
            string resource = (request.EnvironmentUrl ?? string.Empty);
            if (!resource.EndsWith("/"))
            {
                resource += "/";
            }
            string fullURL = string.Format("{0}api/data/v9.2/{1}", resource, request.ODataQuery);

            return base.Retrieve(request.TenantId, resource, fullURL);
        }
    }
}
