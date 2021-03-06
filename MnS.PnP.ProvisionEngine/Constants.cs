﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MnS.PnP.ProvisionEngine
{
    public class Constants
    {
        public const string ConfigLocationKey = "ConfigLocation";

        public const string DeploymentCommand1 = "Deployment configuration file is being read from your application settings.";

        public const string DeploymentStartedCaption = "{0}Deployment Started";

        public const string ExtractionStartedCaption = "{0}Extraction Started";

        public const string DeploymentCommand2 = "{0}Please enter the deplyment configuration file location";

        public static string DeploymentCommand3 = @"Deployment configuration file location is : {0}
                                                {1}Confirm (Y/N) to continue...";

        public const string DeploymentCommand4 = "Please validate your config file.";

        public const string SPConnectionErrorMessage = "Unable to connect to sharepoint. Please try again.";

        public const string DeploymentCommand5 = "Please provide your password for user {0}";

        public const string DeploymentCommand6 = "{0}Do you want to extract template file from SharePoint (E){0} Deploy to SharePoint : (D) {0}";

        public const string WinCredentialsCreated = "Credentials were created in Windows Vault";

        public const string ReadWinCredentials = "Retrieving credentials from Windows Vault...";

        public const string AbortCommand = "Press any key to abort the operation.";

        public const string SPConnectionAttempt = "Trying to connect to SharePoint....";

        public const string UnknownError = "There is an error occured : {0}";

        public const string ConnectionSuccess = "Successfully connected to SharePoint";

        public const string ProvisionElementsAttempt = "Attempting to provision elements based on your solution files.";

        public const string ProcessingElement = "Attempting to proccess {0}";

        public const string ProvisionSuccess = "Successfully provisioned {0} of {1}";

        public const string SubSiteCreationError = "There is an error occured while creating a subsite {0}, {1}";

        public const string SubSiteCreationSuccess = "'{0}' has been successfully created.";

        public const string DuplicateSubsite = "'{0}' is already available in site collection";

        public const string DirectoryNotFound = "{0} is not found in the file system. Please correct this in your application configuration file.";

        internal const string LOGGING_SOURCE = "OfficeDevPnP.Core";
        internal const string LOGGING_SOURCE_FRAMEWORK_PROVISIONING = "PnP Provisioning";

        internal const string FIELD_XML_FORMAT = @"<Field Type=""{0}"" Name=""{1}"" DisplayName=""{2}"" ID=""{3}"" Group=""{4}"" Required=""{5}"" {6}/>";
        internal const string FIELD_XML_PARAMETER_FORMAT = @"{0}=""{1}""";
        internal const string FIELD_XML_PARAMETER_WRAPPER_FORMAT = @"<Params {0}></Params>";

        internal const string TAXONOMY_FIELD_XML_FORMAT = "<Field Type=\"{0}\" DisplayName=\"{1}\" ID=\"{8}\" ShowField=\"Term1033\" Required=\"{2}\" EnforceUniqueValues=\"FALSE\" {3} Sortable=\"FALSE\" Name=\"{4}\" Group=\"{9}\"><Default/><Customization><ArrayOfProperty><Property><Name>SspId</Name><Value xmlns:q1=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q1:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{5}</Value></Property><Property><Name>GroupId</Name></Property><Property><Name>TermSetId</Name><Value xmlns:q2=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q2:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{6}</Value></Property><Property><Name>AnchorId</Name><Value xmlns:q3=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q3:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">00000000-0000-0000-0000-000000000000</Value></Property><Property><Name>UserCreated</Name><Value xmlns:q4=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q4:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>Open</Name><Value xmlns:q5=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q5:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>TextField</Name><Value xmlns:q6=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q6:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">{7}</Value></Property><Property><Name>IsPathRendered</Name><Value xmlns:q7=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q7:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">true</Value></Property><Property><Name>IsKeyword</Name><Value xmlns:q8=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q8:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>TargetTemplate</Name></Property><Property><Name>CreateValuesInEditForm</Name><Value xmlns:q9=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q9:boolean\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">false</Value></Property><Property><Name>FilterAssemblyStrongName</Name><Value xmlns:q10=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q10:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">Microsoft.SharePoint.Taxonomy, Version=15.0.0.0, Culture=neutral, PublicKeyToken=71e9bce111e9429c</Value></Property><Property><Name>FilterClassName</Name><Value xmlns:q11=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q11:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">Microsoft.SharePoint.Taxonomy.TaxonomyField</Value></Property><Property><Name>FilterMethodName</Name><Value xmlns:q12=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q12:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">GetFilteringHtml</Value></Property><Property><Name>FilterJavascriptProperty</Name><Value xmlns:q13=\"http://www.w3.org/2001/XMLSchema\" p4:type=\"q13:string\" xmlns:p4=\"http://www.w3.org/2001/XMLSchema-instance\">FilteringJavascript</Value></Property></ArrayOfProperty></Customization></Field>";
        internal const string THEMES_DIRECTORY = "/_catalogs/theme/15/{0}";
        internal const string MASTERPAGE_SEATTLE = "/_catalogs/masterpage/seattle.master";
        internal const string MASTERPAGE_DIRECTORY = "/_catalogs/masterpage/{0}";
        internal const string MASTERPAGE_CONTENT_TYPE = "0x01010500B45822D4B60B7B40A2BFCC0995839404";
        internal const string PAGE_LAYOUT_CONTENT_TYPE = "0x01010007FF3E057FA8AB4AA42FCB67B453FFC100E214EEE741181F4E9F7ACC43278EE811";
        internal const string HTMLPAGE_LAYOUT_CONTENT_TYPE = "0x01010007FF3E057FA8AB4AA42FCB67B453FFC100E214EEE741181F4E9F7ACC43278EE8110003D357F861E29844953D5CAA1D4D8A3B001EC1BD45392B7A458874C52A24C9F70B";
    }
}
