using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public interface ICRMService
    {
        Task<Result> FetchOData(CRMFetchODataRequest request);
        Result FetchXML(CRMFetchXMLRequest request);
    }
}