using Microsoft.Practices.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnS.PnP.ProvisionEngine
{
    public class ServiceExecutor
    {
        public static void ConnectAndDeploy(string configName = "")
        {
            if (string.IsNullOrEmpty(configName))
            {
                //Read it from application settings
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine(Constants.DeploymentCommand1);
                //configName = System.Configuration.ConfigurationManager.AppSettings[Constants.ConfigLocationKey];
                configName = string.Concat(Environment.CurrentDirectory, "\\..\\..\\Deployment.json"); 
            }

            string confirmationConfig = ReadConfigLocation(ref configName);
            while (confirmationConfig != "y")
            {
                configName = "";
                confirmationConfig = ReadConfigLocation(ref configName);
            }

            string deploymentType = ReadDeploymentType();
            if (deploymentType == "d")
            {
                Console.WriteLine(string.Format(Constants.DeploymentStartedCaption, Environment.NewLine));
                Console.WriteLine("**********************");

                IUnityContainer container = new UnityContainer();
                SPODeployer deployer = RegisterTypes(container, configName);
                deployer.Deploy();

                Console.WriteLine("**********************");
            }
            else if (deploymentType == "e")
            {
                Console.WriteLine(string.Format(Constants.ExtractionStartedCaption, Environment.NewLine));
                Console.WriteLine("**********************");

                IUnityContainer container = new UnityContainer();
                SPODeployer deployer = RegisterTypes(container, configName);
                deployer.Extract();

                Console.WriteLine("**********************");
            }
        }

        private static string ReadConfigLocation(ref string configName)
        {
            if (string.IsNullOrEmpty(configName))
            {
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.WriteLine(string.Format(Constants.DeploymentCommand2, Environment.NewLine));
                configName = Console.ReadLine();
            }

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Format(Constants.DeploymentCommand3, configName, Environment.NewLine));
            ConsoleKeyInfo configFileConfirmation = Console.ReadKey();
            return configFileConfirmation.Key.ToString().ToLowerInvariant();
        }

        private static string ReadDeploymentType()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(string.Format(Constants.DeploymentCommand6, Environment.NewLine));
            ConsoleKeyInfo deploymentType = Console.ReadKey();
            return deploymentType.Key.ToString().ToLowerInvariant();
        }

        private static SPODeployer RegisterTypes(IUnityContainer container, string configName)
        {
            container.RegisterType<IDeploymentService, SPODeploymentService>();
            container.RegisterType<ILogger, ConsoleLogger>();
            container.RegisterType<IConfigurationManager, DeploymentConfigurationManager>(new InjectionConstructor(configName));
            return container.Resolve<SPODeployer>();
        }
    }
}
