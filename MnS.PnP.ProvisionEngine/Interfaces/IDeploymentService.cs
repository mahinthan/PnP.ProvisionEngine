using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnS.PnP.ProvisionEngine
{
    public interface IDeploymentService
    {
        bool Deploy(ClientContext clientContext, ILogger logger);

        bool Extract(ClientContext clientContext, ILogger logger);

        ClientContext Connect(ConfigurationData configData, ILogger logger);

    }
}
