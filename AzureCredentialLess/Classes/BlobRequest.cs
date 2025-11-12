using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AzureCredentialLess.Classes
{
    public class BlobRequest
    {
        public string TenantId {  get; set; }
        public string Account { get; set; }
        public string Container {  get; set; }
        public string Blob {  get; set; }
    }
}
