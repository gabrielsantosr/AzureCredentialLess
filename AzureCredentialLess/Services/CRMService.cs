using AzureCredentialLess.Classes;
using Microsoft.Extensions.Logging;
using Microsoft.PowerPlatform.Dataverse.Client;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;

namespace AzureCredentialLess.Services
{
    public class CRMService : DynamicsCRUDService, ICRMService
    {
        ILogger<CRMService> logger { get; init; }
        public CRMService(ILogger<CRMService> logger, IAzureAuthService azureAuthService) : base(azureAuthService)
        {
            this.logger = logger;
        }

        public Task<Result> FetchOData(CRMFetchODataRequest request)
        {
            string resource = (request.EnvironmentUrl ?? string.Empty);
            if (!resource.EndsWith("/"))
            {
                resource += "/";
            }
            string fullURL = string.Format("{0}api/data/v9.2/{1}", resource, request.ODataQuery);

            return base.Retrieve(request.TenantId, resource, fullURL);
        }

        public Result FetchXML(CRMFetchXMLRequest request)
        {

            var service = GetService(request.TenantId, request.EnvironmentUrl);
            var results = service.RetrieveMultiple(new FetchExpression(request.FetchXML));
            return new Result() { Content = System.Text.Json.JsonSerializer.Serialize(results), StatusCode = 200 };

        }

        private IOrganizationService GetService(string tenantId, string environmentURL)
        {
            return new ServiceClient(new Uri(environmentURL), url => azureAuthService.GetAppRegistrationCredentialLessToken(tenantId, environmentURL));
        }
    }
}
