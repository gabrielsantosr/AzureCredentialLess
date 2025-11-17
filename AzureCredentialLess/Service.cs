using AzureCredentialLess.Classes;
using AzureCredentialLess.Helpers;
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
    private readonly IStorageService storageService;
    private readonly IKeyVaultService keyVaultService;
    public Service(ILogger<Service> logger, ICRMService crmService, IBCService bcService, IStorageService storageService, IKeyVaultService keyVaultService)
    {
        this.logger = logger;
        this.crmService = crmService;
        this.bcService = bcService;
        this.storageService = storageService;
        this.keyVaultService = keyVaultService;
    }

    [Function("QueryDataverse")]
    public async Task<IActionResult> QueryDataverse([HttpTrigger(AuthorizationLevel.Function, "POST")] HttpRequest req)
    {
        try
        {
            CRMFetchODataRequest request = await HttpHelper.ParseBody<CRMFetchODataRequest>(req.Body);
            Result result = await crmService.FetchOData(request);
            return new OkObjectResult(result.Content) { StatusCode = result.StatusCode };
        }
        catch (Exception ex)
        {
            return new OkObjectResult($"Error: {ex.Message}. StackTrace: {ex.StackTrace}") { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
        }
    }
    [Function("FetchDataverse")]
    public async Task<IActionResult> FetchDataverse([HttpTrigger(AuthorizationLevel.Function, "POST")] HttpRequest req)
    {
        try
        {
            CRMFetchXMLRequest request = await HttpHelper.ParseBody<CRMFetchXMLRequest>(req.Body);
            Result result = crmService.FetchXML(request);
            return new OkObjectResult(result.Content) { StatusCode = result.StatusCode };
        }
        catch (Exception ex)
        {
            return new OkObjectResult($"Error: {ex.Message}. StackTrace: {ex.StackTrace}") { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
        }
    }
    [Function("QueryBC")]
    public async Task<IActionResult> QueryBC([HttpTrigger(AuthorizationLevel.Function, "POST")] HttpRequest req)
    {
        try
        {
            BCQueryRequest request = await HttpHelper.ParseBody<BCQueryRequest>(req.Body);
            Result result = await bcService.Retrieve(request);
            return new OkObjectResult(result.Content) { StatusCode = result.StatusCode };
        }
        catch (Exception ex)
        {
            return new OkObjectResult($"Error: {ex.Message}. StackTrace: {ex.StackTrace}") { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
        }
    }

    [Function("GetBlobsDetails")]
    public async Task<IActionResult> GetBlobsDetails([HttpTrigger(AuthorizationLevel.Function, "POST")] HttpRequest req)
    {
        try
        {
            using var reader = new StreamReader(req.Body);
            BlobCollectionRequest request = await HttpHelper.ParseBody<BlobCollectionRequest>(req.Body);
            return new OkObjectResult(await storageService.GetBlobsDetails(request));
        }
        catch (Exception ex)
        {
            return new OkObjectResult($"Error: {ex.Message}. StackTrace: {ex.StackTrace}") { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
        }
    }
    [Function("GetKeyVaultSecret")]
    public async Task<IActionResult> GetKeyVaultSecret([HttpTrigger(AuthorizationLevel.Function, "POST")] HttpRequest req)
    {
        try
        {
            using var reader = new StreamReader(req.Body);
            KeyVaultSecretRequest request = await HttpHelper.ParseBody<KeyVaultSecretRequest>(req.Body);
            return new OkObjectResult(await keyVaultService.GetSecret(request));
        }
        catch (Exception ex)
        {
            return new OkObjectResult($"Error: {ex.Message}. StackTrace: {ex.StackTrace}") { StatusCode = (int)System.Net.HttpStatusCode.InternalServerError };
        }
    }
}