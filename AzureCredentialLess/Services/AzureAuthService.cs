using Azure.Core;
using Azure.Identity;
using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Services
{
    public class AzureAuthService : IAzureAuthService
    {

        const string clientIdParamName = "client_id";


        /// <summary>
        ///  Audience to get the <i>ManagedIdentity-to-Client</i> tokens
        /// </summary>
        const string federatedCredentialAudience = "api://AzureADTokenExchange/";

        /// <summary>
        /// Dictionary to cache the <i>Client-to-Resource</i> tokens
        /// </summary>
        private Dictionary<string, Token> resourcesTokens = new();

        /// <summary>
        /// Managed Identity token issuer.
        /// It generates and caches the <i>ManagedIdentity-to-Client</i> tokens
        /// </summary>
        public ManagedIdentityCredential IdentityCredential { get; private set; }
        /// <summary>
        ///  Dictionary containing the parameters needed for the <i>Client-to-Resource</i> token POST request that are always the same.
        /// </summary>
        private static Dictionary<string, string> basicTokenRequestParams;
        public AzureAuthService()
        {
            resourcesTokens = new();
            InitManagedIdentityCredential();
            InitBasicTokenRequestParams();
        }

        public Task<string> GetToken(string tenantId, string resource) => tenantId is null ? GetManagedIdentityToken(resource) : GetServicePrincipalToken(tenantId, resource);

        private async Task<string> GetManagedIdentityToken(string resource)
        {
            var context = new TokenRequestContext(new[] { resource + ".default" });
            AccessToken token = await IdentityCredential.GetTokenAsync(context); // the credential caches tokens, so if it has a non-expired one for the context, it won't request another one. 
            return token.Token;
        }
        private async Task<string> GetServicePrincipalToken(string tenantId, string resource)
        {
            resource = (resource ?? string.Empty).ToLower();
            string tokenKey = string.Format("{0}|{1}", tenantId, resource);
            if (!resourcesTokens.ContainsKey(tokenKey) || resourcesTokens[tokenKey].IsExpired)
            {
                using (HttpClient client = new HttpClient())
                {
                    string url = $"https://login.microsoftonline.com/{tenantId}/oauth2/v2.0/token";
                    var RequestContent = new FormUrlEncodedContent(await GetTokenRequestParams(resource));
                    var response = await client.PostAsync(url, RequestContent);
                    resourcesTokens[tokenKey] = System.Text.Json.JsonSerializer.Deserialize<Token>(await response.Content.ReadAsStringAsync());
                }
            }
            return resourcesTokens[tokenKey].AccessToken;
        }


        private Task<string> GetAssertion(CancellationToken token) => GetAssertion();

        private Task<string> GetAssertion() => GetManagedIdentityToken(federatedCredentialAudience);
        
        // If tenant is null, it return access the vault with the managed identity, otherwise, with the service principal on behalf of the managed identity
        /// <summary>
        /// If <see cref="AzureRequest.TenantId"/> is null, it returns <see cref="ManagedIdentityCredential"/> to connect managed identity &#x2192; resource<br/>
        /// Otherwise, it returns a <see cref="ClientAssertionCredential"/> to connect managed identity &#x2192; app registration &#x2192; service principal &#x2192; resource
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public TokenCredential GetCredential(string tenantId) => tenantId is null? IdentityCredential : new ClientAssertionCredential(tenantId, ClientId, GetAssertion);

        /// <summary>
        /// Initializes <see cref="IdentityCredential"/><br/>
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
                IdentityCredential = new ManagedIdentityCredential(); // If existent, this will pick up the system-assigned identity.
            }
            else
            {
                IdentityCredential = new ManagedIdentityCredential(managedIdentityClientId); // This will pick up the specified user-assigned identity. 
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
                throw new Exception($"There is no value set to environment variable '{clientIdParamName}'.");
            }
            Dictionary<string, string> body = new();
            foreach (var kv in basicTokenRequestParams)
            {
                body[kv.Key] = kv.Value;
            }
            body["scope"] = resource + ".default"; // user_impersonation doesn't work in this case
            body["client_assertion"] = await GetAssertion();
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
                [clientIdParamName] = ClientId,
                ["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer",
                ["grant_type"] = "client_credentials"
            };
        }

        private static string ClientId { get { if (_clientId is null) { _clientId = Environment.GetEnvironmentVariable(clientIdParamName); } return _clientId; } }
        private static string _clientId;
    }
}
