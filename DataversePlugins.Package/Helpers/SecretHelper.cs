using DataversePlugins.Package.Classes;
using System;
using System.Net.Http;
using System.Text.Json;

namespace DataversePlugins.Package.Helpers
{
    internal static class SecretHelper
    {
        private static JsonSerializerOptions caseInsensitive = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        internal static string GetSecretUsingHttp(string vaultName, string secretName, string apiVersion, string token)
        {
            string url = string.Format("https://{0}.vault.azure.net/secrets/{1}?api-version={2}", vaultName, secretName, apiVersion);
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                var result = client.GetAsync(url).Result;
                int status = (int)result.StatusCode;
                string content = result.Content.ReadAsStringAsync().Result;
                if (result.IsSuccessStatusCode)
                {
                    return JsonSerializer.Deserialize<Secret>(content, caseInsensitive).Value;
                }
                else
                {
                    throw new Exception(content);
                }
            }
        }
    }
}
