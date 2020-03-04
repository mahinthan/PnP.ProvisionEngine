using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnS.PnP.ProvisionEngine
{
    public interface IConfigurationManager
    {
        ConfigurationData GetConfigData();

        T LoadJson<T>(string filePath);

        bool IsFileAvailable(string filePath);

        IList<T> LoadJsonToList<T>(string filePath);
    }
}
