using AzureCredentialLess.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using System.Net.Http.Headers;

namespace AzureCredentialLess;

public class Dataverse
{
    private readonly ILogger<Dataverse> _logger;

    public Dataverse(ILogger<Dataverse> logger)
    {
        _logger = logger;
    }

    [Function("QueryDataverse")]
    public async Task<IActionResult> QueryDataverse([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var query = req.Query["query"][0];
            var url = req.Query["url"][0];
            var tenant = req.Query["tenant-id"][0];

            if (!url.EndsWith("/"))
                url += "/";

            using var client = new HttpClient();
            HttpRequestHeaders headers = client.DefaultRequestHeaders;
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            headers.Authorization = new AuthenticationHeaderValue("Bearer", await Authorization.GetToken(url, tenant));
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("OData-Version", "4.0");

            // Web API call
            var response = await client.GetAsync(url + "api/data/v9.2/" + query);
            string content = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(content) { StatusCode = (int)response.StatusCode };
        }
        catch (Exception ex)
        {
            return new OkObjectResult(ex.Message + ex.StackTrace) { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
        }
    }
}