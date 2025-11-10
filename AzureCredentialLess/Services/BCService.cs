using AzureCredentialLess.Classes;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace AzureCredentialLess.Services
{

    public class BCService : IBCService
    {
        IAzureAuthService azureAuthService { get; init; }
        ILogger<BCService> logger { get; init; }
        public BCService(ILogger<BCService> logger, IAzureAuthService azureAuthService)
        {
            this.logger = logger;
            this.azureAuthService = azureAuthService;
        }

        public async Task<Result> Get(string tenantId, string environment, string query)
        {
            using var client = new HttpClient();
            HttpRequestHeaders headers = client.DefaultRequestHeaders;
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            headers.Authorization = new AuthenticationHeaderValue("Bearer", await azureAuthService.GetCredentialLessToken(tenantId, url));
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("OData-Version", "4.0");

            var response = await client.GetAsync(url + "v2.0/" + environment + "/api/v2.0/" + query);
            string content = await response.Content.ReadAsStringAsync();
            return new Result { Content = content, StatusCode = (int)response.StatusCode };
        }
        const string url = "https://api.businesscentral.dynamics.com/";




    }
}
