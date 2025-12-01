using DataversePlugins.Package.Classes;
using DataversePlugins.Package.Utility;
using DataversePlugins.Tools;
using Microsoft.Xrm.Sdk;
using System;
using System.Text.Json;

namespace DataversePlugins.Package
{
    internal class KeyVaultSecretPlugin : IPlugin
    {
        private static Config config;
        private static JsonSerializerOptions caseInsensitive = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };
        public KeyVaultSecretPlugin(string unsecureConfiguration, string secureConfiguration)

        {
            if (config == null)
            {
                config = JsonSerializer.Deserialize<Config>(unsecureConfiguration, caseInsensitive);
            }
        }

        public void Execute(IServiceProvider serviceProvider)
        {
            var contexto = new Contexto(serviceProvider);
            var entity = contexto.GetTarget();

            if (TryGetSecret(contexto.ManagedIdentityService, (string)entity["gsr_name"], out string output))
            {
                entity["gsr_value"] = output;
                entity["gsr_error"] = null;
            }
            else
            {
                entity["gsr_error"] = output is string && output.Length > 800 ? output.Substring(0, 800) : output;
            }
        }

        private static bool TryGetSecret(IManagedIdentityService service, string secretName, out string output)
        {
            var vaultScope = new string[] { Scopes.KeyVault };
            output = null;
            bool ok = true;
            try
            {
                string token = service.AcquireToken(vaultScope);
                output = Helpers.SecretHelper.GetSecretUsingHttp(config.VaultName, secretName, config.ApiVersion, token);
            }
            catch (Exception ex)
            {
                output = ex.Message;
                ok = false;
            }
            return ok;
        }
    }
}

