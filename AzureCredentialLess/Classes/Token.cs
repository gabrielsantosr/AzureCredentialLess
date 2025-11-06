using System.Text.Json.Serialization;

namespace AzureCredentialLess.Classes
{
    public class Token
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; init; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; init; }

        private readonly DateTime CreatedOn = DateTime.UtcNow;

        // these tokens expire an hour after they have been issued. Let's consider them expired 5 minutes before then, to play it safe.
        internal bool IsExpired { get => CreatedOn.AddSeconds(ExpiresIn - 300) < DateTime.UtcNow; } 

    }
}
