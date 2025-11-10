using AzureCredentialLess.Classes;
using AzureCredentialLess.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace AzureCredentialLess;

public class Service
{
    private readonly ILogger<Service> logger;
    private readonly ICRMService crmService;
    private readonly IBCService bcService;
    public Service(ILogger<Service> logger, ICRMService crmService, IBCService bcService)
    {
        this.logger = logger;
        this.crmService = crmService;
        this.bcService = bcService;
    }

    [Function("QueryDataverse")]
    public async Task<IActionResult> QueryDataverse([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var tenantId = req.Query["tenant-id"][0];
            var url = req.Query["url"][0];
            var query = req.Query["query"][0];

            if (!url.EndsWith("/"))
                url += "/";
            Result result = await crmService.Get(tenantId, url, query);
            return new OkObjectResult(result.Content) { StatusCode = result.StatusCode };
        }
        catch (Exception ex)
        {
            return new OkObjectResult($"Error: {ex.Message}. StackTrace: {ex.StackTrace}") { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
        }
    }
    [Function("QueryBC")]
    public async Task<IActionResult> QueryBC([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequest req)
    {
        try
        {
            var tenantId = req.Query["tenant-id"][0];
            var environment = req.Query["environment"][0];
            var query = req.Query["query"][0];

            Result result = await bcService.Get(tenantId, environment, query);
            return new OkObjectResult(result.Content) { StatusCode = result.StatusCode };
        }
        catch (Exception ex)
        {
            return new OkObjectResult($"Error: {ex.Message}. StackTrace: {ex.StackTrace}") { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
        }
    }
}