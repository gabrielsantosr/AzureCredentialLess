using AzureCredentialLess.Classes;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace AzureCredentialLess.Services
{

    public class CRMService : ICRMService
    {
        ILogger<CRMService> logger {  get; init; }
        IAzureAuthService azureAuthService { get; init; }
        public CRMService(ILogger<CRMService> logger,IAzureAuthService azureAuthService)
        {
            this.logger = logger;
            this.azureAuthService = azureAuthService;
        }

        public async Task<Result> Get(string tenantId, string url, string query)
        {
            using var client = new HttpClient();
            HttpRequestHeaders headers = client.DefaultRequestHeaders;
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            headers.Authorization = new AuthenticationHeaderValue("Bearer", await azureAuthService.GetCredentialLessToken(tenantId, url));
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("OData-Version", "4.0");

            var response = await client.GetAsync(url + "api/data/v9.2/" + query);
            string content = await response.Content.ReadAsStringAsync();
            return new Result { Content = content, StatusCode = (int)response.StatusCode };
        }




    }
}
