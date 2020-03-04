using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnS.PnP.ProvisionEngine
{
    public class ConfigurationData
    {
        public string Name { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
        public string Url { get; set; }
        public string RootSiteMappingFolder { get; set; }
        public string TermStoreID { get; set; }
        public string PathToSearchConfig { get; set; }
        public string PnPSchema {
            get
            {
                return "http://schemas.dev.office.com/PnP/2015/08/ProvisioningSchema";
            }
        }

        [JsonConverter(typeof(DictionaryConverter))]
        public IDictionary<string, string> Folders { get; set; }
    }

    public enum SPOElementType
    {
        Fields,
        ContentTypes,
        Lists,
        TaxonomyGroup
    }
}
