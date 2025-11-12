using AzureCredentialLess.Classes;
using System.Net.Http.Headers;

namespace AzureCredentialLess.Services
{
    public abstract class DynamicsCRUDService
    {
        public delegate Task<string> TokenGetter(string tenantId, string resource);

        private TokenGetter tokenGetter;
        protected DynamicsCRUDService(TokenGetter tokenGetter)
        {
            this.tokenGetter = tokenGetter;
        }
        protected async Task<Result> Get(string tenantId, string resource, string fullURL)
        {
            using var client = new HttpClient();
            HttpRequestHeaders headers = client.DefaultRequestHeaders;
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            headers.Authorization = new AuthenticationHeaderValue("Bearer", await tokenGetter(tenantId, resource));
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("OData-Version", "4.0");
            headers.Add("Prefer", "odata.include-annotations=*");

            var response = await client.GetAsync(fullURL);
            string content = await response.Content.ReadAsStringAsync();
            return new Result { Content = content, StatusCode = (int)response.StatusCode };
        }
    }
}
