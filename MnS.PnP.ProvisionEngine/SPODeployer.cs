using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace MnS.PnP.ProvisionEngine
{
    public class SPODeployer
    {
        IDeploymentService DeploymentService;
        IConfigurationManager ConfigurationManager;
        ILogger Logger;

        public SPODeployer(IDeploymentService _deploymentService, IConfigurationManager _configManager, ILogger _logger)
        {
            this.DeploymentService = _deploymentService;
            this.ConfigurationManager = _configManager;
            this.Logger = _logger;
        }

        public bool Deploy()
        {
            try
            {
                
                ConfigurationData configData = this.ConfigurationManager.GetConfigData();
                if (configData == null)
                {
                    Logger.LogMessage(Constants.DeploymentCommand4, LogType.ErrorAndAbort);
                }

                if (string.IsNullOrEmpty(configData.Name) || string.IsNullOrEmpty(configData.User) || string.IsNullOrEmpty(configData.Url))
                {
                    Logger.LogMessage(Constants.DeploymentCommand4, LogType.ErrorAndAbort);
                }

                ClientContext ctx = this.DeploymentService.Connect(configData, Logger);
                if (ctx == null)
                {
                    Logger.LogMessage(Constants.SPConnectionErrorMessage, LogType.ErrorAndAbort);
                }

                return this.DeploymentService.Deploy(ctx, Logger);
            }
            catch (Exception ex)
            {
                Logger.LogMessage(ex.Message, LogType.ErrorAndAbort);
                return false;
            }
        }

        public bool Extract()
        {
            try
            {

                ConfigurationData configData = this.ConfigurationManager.GetConfigData();
                if (configData == null)
                {
                    Logger.LogMessage(Constants.DeploymentCommand4, LogType.ErrorAndAbort);
                }

                ClientContext ctx = this.DeploymentService.Connect(configData, Logger);
                if (ctx == null)
                {
                    Logger.LogMessage(Constants.SPConnectionErrorMessage, LogType.ErrorAndAbort);
                }

                return this.DeploymentService.Extract(ctx, Logger);
            }
            catch (Exception ex)
            {
                Logger.LogMessage(ex.Message, LogType.ErrorAndAbort);
                return false;
            }
        }
    }
}
