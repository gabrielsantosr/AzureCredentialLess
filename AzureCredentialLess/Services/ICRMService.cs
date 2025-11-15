using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public interface ICRMService
    {
        Task<Result> FetchOData(DataverseODataQueryRequest request);
        Result FetchXML(DataverseFetchXMLQueryRequest request);
    }
}