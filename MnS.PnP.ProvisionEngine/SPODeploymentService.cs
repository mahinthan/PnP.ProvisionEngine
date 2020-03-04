using CredentialManagement;
using Microsoft.SharePoint.Client;
using Newtonsoft.Json;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace MnS.PnP.ProvisionEngine
{
    public class SPODeploymentService : IDeploymentService
    {
        private ConfigurationData ConfigData;

        public ClientContext Connect(ConfigurationData configData, ILogger logger)
        {
            try
            {
                logger.LogMessage(Constants.SPConnectionAttempt, LogType.Info);
                this.ConfigData = configData;
                string password = configData.Password;
                if (string.IsNullOrEmpty(password))
                {
                    password = this.RetrievePassword();
                    
                }

                SecureString securedPass = this.GetSecurePassword(password);
                ClientContext ctx = new ClientContext(configData.Url);
                ctx.AuthenticationMode = ClientAuthenticationMode.Default;
                ctx.Credentials = new SharePointOnlineCredentials(configData.User, securedPass);
                return ctx;
            }
            catch (Exception ex)
            {
                logger.LogMessage(string.Format(Constants.UnknownError, ex.Message), LogType.ErrorAndAbort);
            }

            return null;
        }

        public bool Deploy(ClientContext clientContext, ILogger logger)
        {
            try
            {
                Web provisioningWeb = clientContext.Web;
                clientContext.Load(clientContext.Web, ee => ee.Title);
                clientContext.ExecuteQuery();
                logger.LogMessage(Constants.ConnectionSuccess, LogType.Success);
                ProvisionSPOElements(ConfigData.RootSiteMappingFolder, "Template.xml", clientContext, provisioningWeb, logger);
            }
            catch (Exception ex)
            {
                logger.LogMessage(string.Format(Constants.UnknownError, ex.Message), LogType.ErrorAndAbort);
            }

            return false;
        }

        public bool Extract(ClientContext clientContext, ILogger logger)
        {
            try
            {
                
                Web provisioningWeb = clientContext.Web;
                clientContext.Load(provisioningWeb, ee => ee.Title, ee=>ee.Webs, ee=>ee.ParentWeb);
                clientContext.ExecuteQuery();
                
                logger.LogMessage(Constants.ConnectionSuccess, LogType.Success);
                //List<string> extractedTemplateFiles = ExtractCompleteSPOElements(clientContext, provisioningWeb, logger);
                CreateSiteStructure(clientContext, provisioningWeb.Webs, provisioningWeb, provisioningWeb.Title, logger);
                
            }
            catch (Exception ex)
            {
                logger.LogMessage(string.Format(Constants.UnknownError, ex.Message), LogType.ErrorAndAbort);
            }

            return false;
        }

        private void ExtractCompleteSPOWebElements(ClientContext ctx, Web extractionWeb, string templateName, string currentDirectory, ILogger logger)
        {
            try
            {
                XMLTemplateProvider provider =
                    new XMLFileSystemTemplateProvider(Environment.CurrentDirectory, "");
                
                var ptci = new ProvisioningTemplateCreationInformation(extractionWeb);
                ctx.Load(extractionWeb.ParentWeb);
                ctx.ExecuteQuery();
                if (extractionWeb.ParentWeb.ServerObjectIsNull.Value)
                {
                    ptci.IncludeSiteCollectionTermGroup = true;
                    ptci.IncludeAllTermGroups = true;
                }
                
                ProvisioningTemplate template = extractionWeb.GetProvisioningTemplate(ptci);
                
                provider.SaveAs(template, templateName);
                SplitTemplateFile(ctx, extractionWeb, string.Format("{0}\\{1}.--.complete-template.xml", Environment.CurrentDirectory, extractionWeb.Title), currentDirectory, logger);
                logger.LogMessage(string.Format("Completed template file is extracted for '{0}'.", extractionWeb.Title), LogType.Info);
            }
            catch (Exception ex)
            {
                logger.LogMessage(string.Format(Constants.UnknownError, ex.Message), LogType.ErrorAndContinue);
            }
        }

        private void SplitTemplateFile(ClientContext ctx, Web provisionWeb, string sourceTemplateFile, string currentWorkingDirectory, ILogger logger)
        {
            try
            {
                string namespacePrefix = "pnp";
                string templateFile = string.Format("{0}\\..\\..\\Template Store\\templatefile.xml", Environment.CurrentDirectory);
                
                if (!System.IO.File.Exists(templateFile))
                {
                    logger.LogMessage("Please make sure, the Template Store folder has not been modified.", LogType.ErrorAndAbort);
                }

                foreach (var folder in ConfigData.Folders)
                {
                    try
                    {
                        string targetFile = string.Concat(currentWorkingDirectory, "\\", folder.Value, "\\Template.xml");
                        if (System.IO.File.Exists(targetFile))
                            System.IO.File.Delete(targetFile);

                        System.IO.File.Copy(templateFile, targetFile);

                        XmlReaderSettings settings = new XmlReaderSettings();
                        settings.DtdProcessing = DtdProcessing.Parse;
                        XmlReader reader = XmlReader.Create(sourceTemplateFile, settings);
                        XDocument root = XDocument.Load(reader);
                        XmlNameTable nameTable = reader.NameTable;
                        XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
                        namespaceManager.AddNamespace(namespacePrefix, ConfigData.PnPSchema);

                        IEnumerable<XElement> parameters = root.XPathSelectElements(string.Format("//{0}:ProvisioningTemplate/{0}:{1}", namespacePrefix, folder.Key), namespaceManager);
                        if (parameters.Count() > 0)
                        {
                            XDocument docu = XDocument.Load(targetFile);
                            IEnumerable<XElement> parentElement = docu.XPathSelectElements(string.Format("//{0}:ProvisioningTemplate", namespacePrefix), namespaceManager);
                            parentElement.FirstOrDefault().Add(parameters.FirstOrDefault());
                            parentElement.FirstOrDefault().Document.Save(targetFile);
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogMessage(string.Concat("There is an error occured while spliting template file for folder : ", folder.Value,
                            ", under the ", currentWorkingDirectory, ". ", ex.Message), LogType.ErrorAndContinue);
                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogMessage(string.Concat(Constants.UnknownError, ex.Message), LogType.ErrorAndContinue);
            }
        }

        private void CreateSiteStructure(ClientContext ctx, WebCollection webs, Web rootWeb, string parentFolder, ILogger logger)
        {
            try
            {
                string rootFolder = string.Format("{0}\\..\\..\\{1}\\", Environment.CurrentDirectory, parentFolder);
                //rename root directory
                if (!System.IO.Directory.Exists(rootFolder))
                {
                    System.IO.Directory.CreateDirectory(rootFolder);
                    if (rootWeb.ParentWeb.ServerObjectIsNull.HasValue && rootWeb.ParentWeb.ServerObjectIsNull.Value)
                    {
                        //This is the root site
                        //Move template folders into the current directory

                        DirectoryInfo dirInfo = new DirectoryInfo(string.Format("{0}\\..\\..\\{1}\\", Environment.CurrentDirectory, "Template Store"));
                        ProcessDirectories(dirInfo, rootFolder);
                    }
                }

                foreach (var folder in ConfigData.Folders)
                {
                    if (!System.IO.Directory.Exists(string.Concat(rootFolder, folder.Value)))
                    {
                        System.IO.Directory.CreateDirectory(string.Concat(rootFolder, folder.Value));
                    }
                }
                
                ExtractCompleteSPOWebElements(ctx, rootWeb, string.Format("{0}.--.complete-template.xml", rootWeb.Title), rootFolder, logger);
                if (webs.Count > 0)
                {
                    parentFolder += "\\SubSites\\";
                    if (!System.IO.Directory.Exists(string.Format("{0}\\..\\..\\{1}\\", Environment.CurrentDirectory, parentFolder)))
                    {
                        System.IO.Directory.CreateDirectory(string.Format("{0}\\..\\..\\{1}\\", Environment.CurrentDirectory, parentFolder));
                    }
                }

                foreach (Web subWeb in webs)
                {
                    try
                    {
                        WebCollection subWebCollection = subWeb.Webs;
                        ctx.Load(subWebCollection);
                        ctx.ExecuteQuery();
                        parentFolder += "\\" + subWeb.Title;

                        CreateSiteStructure(ctx, subWebCollection, subWeb, parentFolder, logger);
                        parentFolder += "\\..\\";
                    }
                    catch (Exception ex)
                    {

                    }
                }
            }
            catch (Exception ex)
            {
                logger.LogMessage(string.Concat(Constants.UnknownError, ex.Message), LogType.ErrorAndContinue);
            }
        }

        private void ProcessDirectories(DirectoryInfo dirInput, string dirOutput)
        {
            string dirOutputfix = String.Empty;

            foreach (DirectoryInfo di in dirInput.GetDirectories())
            {
                dirOutputfix = dirOutput + "\\" + di.Name;

                if (!System.IO.Directory.Exists(dirOutputfix))
                {
                    try
                    {
                        System.IO.Directory.CreateDirectory(dirOutputfix);
                    }
                    catch (Exception e)
                    {
                        throw (e);
                    }
                }

                ProcessDirectories(di, dirOutputfix);
            }
        }

        private List<string> ExtractCompleteSPOElements(ClientContext context, Web extractionWeb, ILogger logger)
        {
            try
            {
                List<string> extractedCompleteFiles = new List<string>();
                string templateName = string.Format("{0}.--.complete-template.xml", extractionWeb.Title);
                WebCollection webs = extractionWeb.Webs;
                context.Load(webs);
                context.ExecuteQuery();
                //ExtractCompleteSPOWebElements(context, extractionWeb, templateName, logger);
                foreach (Web web in webs)
                {
                    templateName = string.Format("{0}.--.complete-template.xml", web.Title);
                    //ExtractCompleteSPOWebElements(context, web, templateName, logger);
                }

                logger.LogMessage("Working on splitting template files...");
                return extractedCompleteFiles;
            }
            catch (Exception ex)
            {
                logger.LogMessage(string.Format(Constants.UnknownError, ex.Message), LogType.ErrorAndAbort);
            }
            return null;
        }

        private string RetrievePassword()
        {
            CredentialManager credentialManager = new CredentialManager(this.ConfigData.Name);
            UserPass pass = credentialManager.GetCredentials();
            string password = String.Empty;
            if (pass == null || string.IsNullOrEmpty(pass.Password))
            {
                Console.WriteLine(Constants.DeploymentCommand5, this.ConfigData.User);

                password = Console.ReadLine();

                credentialManager.SetCredentials(new UserPass(new Credential(this.ConfigData.User, password)));

                Console.WriteLine(Constants.WinCredentialsCreated);
            }
            else
            {
                Console.WriteLine(Constants.ReadWinCredentials);
                password = pass.Password;
            }
            return password;
        }

        private SecureString GetSecurePassword(string password)
        {
            //Get the user's password as a SecureString
            SecureString securePassword = new SecureString();
            foreach (char keyChar in password)
            {
                securePassword.AppendChar(keyChar);
            }

            return securePassword;
        }

        private string[] ProvisionSPOElements(string directoryName, string fileName, ClientContext context, Web provisioningWeb, ILogger logger)
        {
            logger.LogMessage(Constants.ProvisionElementsAttempt, LogType.Info);
            string[] files = new string[0];
            ArrayList directories = new ArrayList();
            ArrayList foundfiles = new ArrayList();
            directories.Add(directoryName);
            DirectoryInfo baseDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory);
            string[] csProjFiles = System.IO.Directory.GetFiles(baseDir.Parent.Parent.FullName, "*.csproj");
            List<string> includedProjectElements = ReadProjectStructure(csProjFiles.FirstOrDefault(), baseDir.Parent.Parent.FullName);

            while (directories.Count > 0)
            {
                string n = (string)directories[0];

                try
                {
                    if (!System.IO.Directory.Exists(n))
                        logger.LogMessage(string.Format(Constants.DirectoryNotFound, n), LogType.ErrorAndAbort);

                    string[] foundfiles1 = System.IO.Directory.GetFiles(n, fileName, SearchOption.TopDirectoryOnly);
                    if (foundfiles1 != null)
                    {
                        XMLTemplateProvider provider = new XMLFileSystemTemplateProvider(String.Format(@"{0}\..\..\", AppDomain.CurrentDomain.BaseDirectory), "");
                        foreach (string templateFile in foundfiles1)
                        {
                            if (!includedProjectElements.Contains(templateFile.ToLowerInvariant()))
                                continue;

                            //UpdateTermStoreId(templateFile, logger);
                            //UpdatePnPFileRef(templateFile, logger);
                            DirectoryInfo dirInfo = new DirectoryInfo(string.Format(@"{0}\..\..\", templateFile));
                            logger.LogMessage(string.Format(Constants.ProcessingElement, templateFile), LogType.Info);
                            string siteTitle = dirInfo.FullName.Substring(0, dirInfo.FullName.LastIndexOf('\\'));
                            siteTitle = siteTitle.Substring(siteTitle.LastIndexOf('\\') + 1);

                            if (context.Web.Title != siteTitle)
                            {
                                context.Load(context.Web.Webs);
                                context.ExecuteQuery();
                                foreach (Web webNode in context.Web.Webs)
                                {
                                    if (webNode.Title == siteTitle)
                                    {
                                        provisioningWeb = webNode;
                                        break;
                                    }
                                }
                                
                            }

                            //Apply provision
                            var template = provider.GetTemplate(templateFile);
                            string connectionString = ConfigData.RootSiteMappingFolder.ToLowerInvariant();
                            if (!ConfigData.RootSiteMappingFolder.ToLowerInvariant().EndsWith("\\"))
                                connectionString += "\\";
                            string container = templateFile.ToLowerInvariant().Replace(connectionString, string.Empty).Replace("template.xml", string.Empty);
                            template.Connector = new FileSystemConnector(connectionString, container);

                            try
                            {
                                ProvisioningTemplateApplyingInformation ptai = new ProvisioningTemplateApplyingInformation();
                                
                                provisioningWeb.ApplyProvisioningTemplate(template);
                                logger.LogMessage(string.Format(Constants.ProvisionSuccess, dirInfo.Name, provisioningWeb.Title), LogType.Success);
                            }
                            catch (Exception ex)
                            {
                                logger.LogMessage(string.Format(Constants.UnknownError, ex.Message), LogType.ErrorAndContinue);
                            }
                        }
                        
                        foundfiles.AddRange(foundfiles1);
                    }
                }
                catch(Exception ex)
                {
                    logger.LogMessage(string.Format(Constants.UnknownError, ex.Message), LogType.ErrorAndContinue);
                }

                try
                { 
                    string[] subdirectories = System.IO.Directory.GetDirectories(n);
                    if (subdirectories != null)
                    {
                        string subsitesFolder = RemoveEmptySpaces(n.ToLowerInvariant());
                        if (subsitesFolder.EndsWith("subsites"))
                        {
                            //C:\something\MnS\Subsites\across\
                            DirectoryInfo dirInfo = new DirectoryInfo(string.Format(@"{0}\..\", n));
                            string parentSiteTitle = dirInfo.FullName.Substring(0, dirInfo.FullName.LastIndexOf('\\'));
                            parentSiteTitle = parentSiteTitle.Substring(parentSiteTitle.LastIndexOf('\\') + 1);
                            WebCollection subsites = context.Web.Webs;
                            context.Load(subsites);
                            context.ExecuteQuery();
                            foreach (string subsiteDir in subdirectories)
                            {
                                foreach (string item in includedProjectElements)
                                {
                                    if (item.StartsWith(subsiteDir.ToLowerInvariant()))
                                    {
                                        CreateSubsite(context, parentSiteTitle, subsiteDir.Replace(n, string.Empty).Substring(1), subsites, logger);
                                        break;
                                    }
                                }
                                    
                            }
                        }

                        directories.AddRange(subdirectories);
                    }
                }
                catch(Exception ex)
                {
                    logger.LogMessage(string.Format(Constants.UnknownError, ex.Message), LogType.ErrorAndContinue);
                }

                directories.RemoveAt(0);
            }

            return (string[])foundfiles.ToArray(typeof(string));
        }

        private void CreateSubsite(ClientContext context, string parentWebTitle, string subsiteFolderName, WebCollection subSiteCollection, ILogger logger)
        {
            try
            {
                if (context == null || string.IsNullOrEmpty(subsiteFolderName))
                    return;
                
                string subsiteInternalTitle = RemoveEmptySpaces(subsiteFolderName);
                Web parentWeb = context.Web;
                if (parentWeb.Title != parentWebTitle)
                {
                    parentWeb = subSiteCollection.SingleOrDefault(e => e.Title.Equals(parentWebTitle));
                }

                if (parentWeb == null)
                {
                    subsiteInternalTitle = RemoveEmptySpaces(parentWebTitle);
                    context.Web.Webs.Add(new Microsoft.SharePoint.Client.WebCreationInformation()
                    {
                        WebTemplate = "CMSPUBLISHING#0",
                        Title = parentWebTitle,
                        Description = parentWebTitle,
                        Url = subsiteInternalTitle,
                        Language = 1033,
                        UseSamePermissionsAsParentSite = true
                    });
                    context.ExecuteQuery();
                    parentWeb = subSiteCollection.SingleOrDefault(e => e.Title.Equals(parentWebTitle));
                }

                if (!subSiteCollection.Where(w => w.Title == subsiteFolderName).Any())
                {
                    //If there is no subsites with the same name as the given subsite, create it
                    parentWeb.Webs.Add(new Microsoft.SharePoint.Client.WebCreationInformation()
                    {
                        WebTemplate = "CMSPUBLISHING#0",
                        Title = subsiteFolderName,
                        Description = subsiteFolderName,
                        Url = subsiteInternalTitle,
                        Language = 1033,
                        UseSamePermissionsAsParentSite = true
                    });
                    context.ExecuteQuery();
                    logger.LogMessage(string.Format(Constants.SubSiteCreationSuccess, subsiteFolderName), LogType.Success);
                }
                else
                {
                    logger.LogMessage(string.Format(Constants.DuplicateSubsite, subsiteFolderName), LogType.Info);
                }   

            }
            catch (Exception ex)
            {
                logger.LogMessage(string.Format(Constants.SubSiteCreationError, subsiteFolderName, ex.Message), LogType.ErrorAndContinue);
            }
        }

        private string RemoveEmptySpaces(string folderName)
        {
            try
            {
                return Regex.Replace(folderName, @"\s+", "",
                                RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            catch (Exception)
            {

                throw;
            }
        }

        private List<string> ReadProjectStructure(string csProjPath, string basePath)
        {
            List<string> itemsIncluded = new List<string>();
            try
            {
                XElement root = XElement.Load(csProjPath);
                XNamespace ad = "http://schemas.microsoft.com/developer/msbuild/2003";
                IEnumerable<XElement> tests =
                from el in root.Elements(ad + "ItemGroup")
                select el;
                foreach (XElement item in tests)
                {
                    foreach (XNode node in item.Nodes())
                    {
                        XElement contentNode = ((XElement)node);
                        if (contentNode.Name.LocalName != "Content")
                            break;

                        itemsIncluded.Add(string.Concat(basePath.ToLowerInvariant(), "\\", contentNode.Attribute("Include").Value.ToLowerInvariant()));
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }

            return itemsIncluded;
        }

        private bool UpdateTermStoreId(string file, ILogger logger)
        {
            try
            {
                XmlReaderSettings settings = new XmlReaderSettings();
                settings.DtdProcessing = DtdProcessing.Parse;
                XmlReader reader = XmlReader.Create(file, settings);
                XDocument root = XDocument.Load(reader);
                XmlNameTable nameTable = reader.NameTable;
                XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
                namespaceManager.AddNamespace("pnp", ConfigData.PnPSchema);
                IEnumerable<XElement> parameters = root.XPathSelectElements("//pnp:Preferences/pnp:Parameters/pnp:Parameter", namespaceManager);

                foreach (XElement parameterElement in parameters)
                {
                    if (parameterElement.Attribute("Key") != null && parameterElement.Attribute("Key").Value.ToLowerInvariant() == "sspid")
                    {
                        parameterElement.SetValue(ConfigData.TermStoreID);
                        reader.Close();
                        root.Save(file);
                    }
                }

                logger.LogMessage(string.Concat("Term store id has been updated in ", file), LogType.Info);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogMessage(string.Concat("There is an error occured while updating the term-store-id. ", ex.Message), LogType.ErrorAndAbort);
                return false;
            }
        }

        //private bool UpdatePnPFileRef(string file, ILogger logger)
        //{
        //    try
        //    {
        //        GC.Collect();
        //        using (FileStream fs = new FileStream(file, FileMode.OpenOrCreate, FileAccess.ReadWrite))
        //        {
        //            XmlReader reader = XmlReader.Create(fs);
        //            XDocument root = XDocument.Load(reader);
        //            XmlNameTable nameTable = reader.NameTable;
        //            XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
        //            namespaceManager.AddNamespace("pnp", ConfigData.PnPSchema);
        //            IEnumerable<XElement> parameters = root.XPathSelectElements("//pnp:Templates/pnp:ProvisioningTemplate/pnp:Files/pnp:File", namespaceManager);

        //            foreach (XElement parameterElement in parameters)
        //            {
        //                if (parameterElement.Attribute("Src") != null)
        //                {
        //                    DirectoryInfo dirInfo = new DirectoryInfo(parameterElement.Attribute("Src").Value);
        //                    if (dirInfo.FullName.ToLowerInvariant() != parameterElement.Attribute("Src").Value.ToLowerInvariant())
        //                    {
        //                        string fullValue = string.Concat(file.ToLowerInvariant().Replace("template.xml", string.Empty), parameterElement.Attribute("Src").Value);
        //                        parameterElement.Attribute("Src").SetValue(fullValue);
        //                        if (fs.CanRead)
        //                        {
        //                            fs.Flush();
        //                            fs.Close();

        //                            reader.Close();
        //                            GC.Collect();
        //                        }
        //                        root.Save(file);
        //                    }
        //                }
        //            }

        //            reader.Close();
        //        }
        //        //logger.LogMessage(string.Concat("Term store id has been updated in ", file), LogType.Info);
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        logger.LogMessage(string.Concat("There is an error occured while updating the file elements in template file. ", ex.Message), LogType.ErrorAndAbort);
        //        return false;
        //    }
        //    finally
        //    {
        //        GC.Collect();
        //    }
        //}
    }
}
