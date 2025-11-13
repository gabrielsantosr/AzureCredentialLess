using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureCredentialLess.Classes
{
    public abstract class ODataQueryRequest: AzureRequest
    {
        public string ODataQuery {  get; set; }
    }
}
