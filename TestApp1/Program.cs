using MnS.PnP.ProvisionEngine;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace SPSiteProvision
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceExecutor.ConnectAndDeploy();
            Console.ReadLine();
        }
        
    }
 
}
