using AzureCredentialLess.Classes;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace AzureCredentialLess.Services
{

    public class CRMService :DynamicsCRUDService,ICRMService
    {
        ILogger<CRMService> logger {  get; init; }
        public CRMService(ILogger<CRMService> logger,IAzureAuthService azureAuthService):base(azureAuthService.GetCredentialLessToken)
        {
            this.logger = logger;
        }

        public new Task<Result> Get(string tenantId, string url, string query)
        {
            return base.Get(tenantId, url, url + "api/data/v9.2/" + query);
        }




    }
}
