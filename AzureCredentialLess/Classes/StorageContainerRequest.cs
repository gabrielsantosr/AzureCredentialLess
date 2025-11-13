namespace AzureCredentialLess.Classes
{
    public class StorageContainerRequest : AzureRequest
    {
        public string Account { get; set; }
        public string Container { get; set; }

    }
}
