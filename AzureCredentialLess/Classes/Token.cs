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

        // Let's consider it expired 2 minutes before the actual expiry moment, to play it safe.
        internal bool IsExpired { get => CreatedOn.AddSeconds(ExpiresIn - 120) < DateTime.UtcNow; } 

    }
}
