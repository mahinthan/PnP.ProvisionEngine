using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnS.PnP.ProvisionEngine
{
    public class DeploymentConfigurationManager : IConfigurationManager
    {
        private string ConfigName;
        public DeploymentConfigurationManager(string configName)
        {
            this.ConfigName = configName;
        }
        public ConfigurationData GetConfigData()
        {
            return LoadJson<ConfigurationData>(this.ConfigName);
        }
        public T LoadJson<T>(string filePath)
        {
            T items = default(T);
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<T>(json);
            }

            return items;
        }

        public IList<T> LoadJsonToList<T>(string filePath)
        {
            List<T> items = null;
            using (StreamReader r = new StreamReader(filePath))
            {
                string json = r.ReadToEnd();
                items = JsonConvert.DeserializeObject<List<T>>(json);
            }

            return items;
        }

        public bool IsFileAvailable(string filePath)
        {
            if (!System.IO.File.Exists(filePath))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error: File {0} doesn't exist", filePath);
                Console.ResetColor();
                Console.ReadKey();

                return false;
            }
            return true;
        }

        public void UpdateConfigData(ConfigurationData data, string path)
        {
            try
            {
                string serializedData = JsonConvert.SerializeObject(data);
                using (StreamWriter writer = new StreamWriter(path))
                {
                    writer.WriteLine(serializedData);
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
}
