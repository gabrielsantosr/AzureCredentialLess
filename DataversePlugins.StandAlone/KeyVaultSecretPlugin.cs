using DataversePlugins.StandAlone.Classes;
using Microsoft.Xrm.Sdk;
using System;
using System.Net.Http;
using System.Text.Json;

namespace DataversePlugins.StandAlone
{
    /// <summary>
    /// Plugin development guide: https://docs.microsoft.com/powerapps/developer/common-data-service/plug-ins
    /// Best practices and guidance: https://docs.microsoft.com/powerapps/developer/common-data-service/best-practices/business-logic/
    /// </summary>
    public class KeyVaultSecretPlugin : IPlugin
    {
        private static Config config;
        private static JsonSerializerOptions caseInsensitive = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        const string vaultScope = "https://vault.azure.net/.default";
        public KeyVaultSecretPlugin(string unsecureConfiguration, string secureConfiguration)

        {
            if (config == null)
            {
                config = JsonSerializer.Deserialize<Config>(unsecureConfiguration, caseInsensitive);
            }
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var Tracer = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            var Context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var ManagedIdentityService = (IManagedIdentityService)serviceProvider.GetService(typeof(IManagedIdentityService));
            var Target = (Entity)Context.InputParameters["Target"];

            if (TryGetSecret(ManagedIdentityService, (string)Target["gsr_name"], out string output))
            {
                Target["gsr_value"] = output;
                Target["gsr_error"] = null;
            }
            else
            {
                Target["gsr_error"] = output is string && output.Length > 800 ? output.Substring(0, 800) : output;
            }
        }

        private static bool TryGetSecret(IManagedIdentityService service, string secretName, out string output)
        {
            output = null;
            bool ok = false;
            try
            {

                string token = service.AcquireToken(new string[] { "https://vault.azure.net/.default" });
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Add("Authorization", "Bearer " + token);
                    var result = client.GetAsync(string.Format("https://{0}.vault.azure.net/secrets/{1}?api-version={2}", config.VaultName, secretName, config.ApiVersion)).Result;
                    int status = (int)result.StatusCode;
                    string content = result.Content.ReadAsStringAsync().Result;
                    if (result.IsSuccessStatusCode)
                    {
                        output = JsonSerializer.Deserialize<Secret>(content, caseInsensitive).Value;
                        ok = true;
                    }
                    else
                    {
                        output = content;
                    }
                }
            }
            catch (Exception ex)
            {
                output = ex.Message;
            }
            return ok;
        }
    }
}

