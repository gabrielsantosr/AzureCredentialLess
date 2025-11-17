namespace AzureCredentialLess.Classes
{
    public class BCQueryRequest : AzureRequest
    {
        public string EnvironmentName { get; set; }
        public string ODataQuery { get; set; }
    }
}
