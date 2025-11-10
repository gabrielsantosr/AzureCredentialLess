using Azure.Core;
using Azure.Identity;
using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public class AzureAuthService : IAzureAuthService
    {

        const string clientIdParamName = "client_id";

        /// <summary>
        /// Dictionary to cache the <i>Client-to-Resource</i> tokens
        /// </summary>
        private Dictionary<string, Token> resourcesTokens = new();

        /// <summary>
        /// Managed Identity token issuer.
        /// It generates and caches the <i>ManagedIdentity-to-Client</i> tokens
        /// </summary>
        private ManagedIdentityCredential identityCredential;
        /// <summary>
        ///  Context to get the <i>ManagedIdentity-to-Client</i> tokens
        /// </summary>
        private TokenRequestContext identityToClientContext;
        /// <summary>
        ///  Dictionary containing the parameters needed for the <i>Client-to-Resource</i> token POST request that are always the same.
        /// </summary>
        private static Dictionary<string, string> basicTokenRequestParams;
        public AzureAuthService()
        {
            resourcesTokens = new();
            InitManagedIdentityCredential();
            InitBasicTokenRequestParams();
            identityToClientContext = new TokenRequestContext(new[] { $"api://AzureADTokenExchange/.default" });
        }

        public async Task<string> GetCredentialLessToken(string tenantId, string resource)
        {

            if (!resourcesTokens.ContainsKey(resource) || resourcesTokens[resource].IsExpired)
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
                    var RequestContent = new FormUrlEncodedContent(await GetTokenRequestParams(resource));
                    var response = await client.PostAsync($"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token", RequestContent);
                    resourcesTokens[resource] = System.Text.Json.JsonSerializer.Deserialize<Token>(await response.Content.ReadAsStringAsync());
                }
            }
            return resourcesTokens[resource].AccessToken;
        }

        /// <summary>
        /// Initializes <see cref="identityCredential"/><br/>
        /// If the environment variable <i>identity_client_id</i> is set, it means it will pick up that user-assigned identity<br/>
        /// Otherwise, if existent, it will pick up the system-assigned identity, which has no client id<br/>
        /// 
        /// The app registration must have a federated credential for the identity. 
        /// </summary>
        private void InitManagedIdentityCredential()

        {

            string managedIdentityClientId = Environment.GetEnvironmentVariable("identity_client_id");
            if (managedIdentityClientId is null)
            {
                identityCredential = new ManagedIdentityCredential(); // If existent, this will pick up the system-assigned identity.
            }
            else
            {
                identityCredential = new ManagedIdentityCredential(managedIdentityClientId); // This will pick up the specified user-assigned identity. 
            }
        }

        /// <summary>
        /// Gets the params necessary for the <i>Client-to-Resource</i> token POST request on behalf of the Managed Identity.<br/>
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private async Task<Dictionary<string, string>> GetTokenRequestParams(string resource)
        {
            if (string.IsNullOrWhiteSpace(basicTokenRequestParams[clientIdParamName]))
            {
                throw new Exception($"There is no value in environment variable {clientIdParamName}");
            }
            Dictionary<string, string> body = new();
            foreach (var kv in basicTokenRequestParams)
            {
                body[kv.Key] = kv.Value;
            }
            body["scope"] = resource + ".default"; // user_impersonation doesn't work in this case
            body["client_assertion"] = (await identityCredential.GetTokenAsync(identityToClientContext)).Token; // the credential caches tokens, so if it has a non-expired one for the context, it won't request another one.
            return body;
        }
        /// <summary>
        /// Initializes <see cref="basicTokenRequestParams"/><br/>
        /// <inheritdoc cref="basicTokenRequestParams"/>
        /// </summary>
        private static void InitBasicTokenRequestParams()
        {
            basicTokenRequestParams = new()
            {
                [clientIdParamName] = Environment.GetEnvironmentVariable(clientIdParamName),
                ["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer",
                ["grant_type"] = "client_credentials"
            };
        }
    }
}
