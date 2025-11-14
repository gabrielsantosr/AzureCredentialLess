namespace AzureCredentialLess.Classes
{
    public class KeyVaultSecretRequest: KeyVaultRequest
    {
        public string SecretName {  get; set; }
        public string SecretValue {  get; set; }
    }
}
