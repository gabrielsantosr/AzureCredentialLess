using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCredentialLess.Classes
{
    public class DataverseFetchXMLQueryRequest: AzureRequest
    {
        public string EnvironmentUrl { get; set; }
        public string FetchXML { get; set; }
    }
}
