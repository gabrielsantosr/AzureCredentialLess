namespace AzureCredentialLess.Classes
{
    public class CRMFetchODataRequest : AzureRequest
    {
        public string EnvironmentUrl { get; set; }
        public string ODataQuery { get; set; }
    }
}
