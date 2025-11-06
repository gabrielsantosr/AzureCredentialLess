using Azure.Core;
using Azure.Identity;
using AzureCredentialLess.Classes;

namespace AzureCredentialLess.Helpers
{
    internal class Authorization
    {
        private static Dictionary<string, Token> urlTokens = new();
        private static string clientId = Environment.GetEnvironmentVariable("sp_client_id");
        private static string managedIdentityClientId = Environment.GetEnvironmentVariable("mi_client_id");
        private static ManagedIdentityCredential miCredential = new(managedIdentityClientId);
        private static TokenRequestContext TokenRequestContext = new TokenRequestContext(new[] { $"api://AzureADTokenExchange/.default" });
        private static Dictionary<string, string> GetTokenBodyParams = new()
        {
            ["client_id"] = clientId,
            ["client_assertion_type"] = "urn:ietf:params:oauth:client-assertion-type:jwt-bearer",
            ["grant_type"] = "client_credentials"
        };

        internal static async Task<string> GetToken(string crmUrl, string crmTenant)
        {

            if (!urlTokens.ContainsKey(crmUrl) || urlTokens[crmUrl].IsExpired)
            {
                Dictionary<string, string> body = new();
                foreach (var kv in GetTokenBodyParams)
                {
                    body[kv.Key] = kv.Value;
                }
                body["scope"] = crmUrl + ".default";
                body["client_assertion"] = (await miCredential.GetTokenAsync(TokenRequestContext)).Token;
                using (HttpClient client = new HttpClient())
                {
                    var RequestContent = new FormUrlEncodedContent(body);
                    var response = await client.PostAsync($"https://login.microsoftonline.com/{crmTenant}/oauth2/v2.0/token", RequestContent);
                    urlTokens[crmUrl] = System.Text.Json.JsonSerializer.Deserialize<Token>(await response.Content.ReadAsStringAsync());
                }

            }
            return urlTokens[crmUrl].AccessToken;
        }


    }


}
