namespace AzureCredentialLess.Helpers
{
    internal static class HttpHelper
    {
        internal static async Task<T> ParseBody<T>(Stream body)
        {
            using var reader = new StreamReader(body);
            string serializedBody = await reader.ReadToEndAsync();
            T output = System.Text.Json.JsonSerializer.Deserialize<T>(serializedBody, new System.Text.Json.JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            return output;
        }
    }
}
