using AzureCredentialLess.Classes;
using System.Net.Http.Headers;
using System.Text;

namespace AzureCredentialLess.Services
{
    public abstract class DynamicsCRUDService
    {
        protected delegate Task<string> TokenGetter(string tenantId, string resource);

        private TokenGetter tokenGetter;
        protected DynamicsCRUDService(TokenGetter tokenGetter)
        {
            this.tokenGetter = tokenGetter;
        }
        protected Task<Result> Create(string tenantId, string resource, string fullURL, object payload) => Request(Enums.CRUD.Retrieve, tenantId, resource, fullURL, payload);
        protected Task<Result> Retrieve(string tenantId, string resource, string fullURL) => Request(Enums.CRUD.Retrieve, tenantId, resource, fullURL);
        protected Task<Result> Update(string tenantId, string resource, string fullURL, object payload) => Request(Enums.CRUD.Retrieve, tenantId, resource, fullURL, payload);
        protected Task<Result> Delete(string tenantId, string resource, string fullURL) => Request(Enums.CRUD.Retrieve, tenantId, resource, fullURL);



        private async Task<Result> Request(Enums.CRUD crudOperation, string tenantId, string resource, string fullURL, object payload = null)
        {
            using StringContent reqBody = payload is null ? null : new StringContent(System.Text.Json.JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");
            using var client = new HttpClient();
            HttpRequestHeaders headers = client.DefaultRequestHeaders;
            headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            headers.Authorization = new AuthenticationHeaderValue("Bearer", await tokenGetter(tenantId, resource));
            headers.Add("OData-MaxVersion", "4.0");
            headers.Add("OData-Version", "4.0");
            headers.Add("Prefer", "odata.include-annotations=*");
            Task<HttpResponseMessage> responseTask = null;
            switch (crudOperation)
            {
                case Enums.CRUD.Create:
                    responseTask = client.PostAsync(fullURL, reqBody); break;
                case Enums.CRUD.Retrieve:
                    responseTask = client.GetAsync(fullURL); break;
                case Enums.CRUD.Update:
                    responseTask = client.PatchAsync(fullURL, reqBody); break;
                case Enums.CRUD.Delete:
                    responseTask = client.DeleteAsync(fullURL); break;
            }
            using HttpResponseMessage response = await responseTask;

            string responseBody = await response.Content.ReadAsStringAsync();
            return new Result()
            {
                StatusCode = (int)response.StatusCode,
                Content = responseBody,
            };
        }
    }
}
