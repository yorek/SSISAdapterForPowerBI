using Microsoft.SqlServer.Dts.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.Samples.SqlServer.SSIS.PowerBIConnectionManager
{
    [DtsConnection(ConnectionType = "POWERBI", DisplayName = "Power BI",
       UITypeName = "Microsoft.Samples.SqlServer.SSIS.PowerBIConnectionManager.PowerBIConnectionManagerUI, PowerBIConnectionManager, Version=1.0.0.0, Culture=neutral,PublicKeyToken=79d8162de6e0a1ee",
               Description = "Connection Manager for Power BI REST API")]
    public class PowerBIConnectionManager : ConnectionManagerBase
    {
        public string ClientID { get; set; }
        public string RedirectUri { get; set; }
        public string ResourceUri { get; set; }
        public string OAuth2AuthorityUri { get; set; }
        public string PowerBIDataSets { get; set; }
        //Cred
        public string UserName { get; set; }
        public string Password { get; set; }

        public PowerBIConnectionManager()
        {
            ClientID = "Your Client ID (0c13e266-bde8-4758-ae29-a0c5fbfee504)";
            RedirectUri = "Your Redirect Url (https://oauth.powerbi.com/BI)";
            ResourceUri = "https://analysis.windows.net/powerbi/api";
            OAuth2AuthorityUri = "https://login.windows.net/common/oauth2/authorize";
            PowerBIDataSets = "https://api.powerbi.com/beta/myorg/datasets";
            UserName = "<Username>";
            Password = "<Password>";
        }


        public override Microsoft.SqlServer.Dts.Runtime.DTSExecResult Validate(Microsoft.SqlServer.Dts.Runtime.IDTSInfoEvents infoEvents)
        {
            if (string.IsNullOrWhiteSpace(ClientID))
            {
                return DTSExecResult.Failure;
            }
            else if (string.IsNullOrWhiteSpace(RedirectUri))
            {
                return DTSExecResult.Failure;
            }
            else if (string.IsNullOrWhiteSpace(ResourceUri))
            {
                return DTSExecResult.Failure;
            }
            else if (string.IsNullOrWhiteSpace(OAuth2AuthorityUri))
            {
                return DTSExecResult.Failure;
            }
            else if (string.IsNullOrWhiteSpace(PowerBIDataSets))
            {
                return DTSExecResult.Failure;
            }
            else if (string.IsNullOrWhiteSpace(UserName))
            {
                return DTSExecResult.Failure;
            }
            else if (string.IsNullOrWhiteSpace(Password))
            {
                return DTSExecResult.Failure;
            }


            return DTSExecResult.Success;
        }


        public override object AcquireConnection(object txn)
        {
            Dictionary<string, string> connObj = new Dictionary<string, string>();

            connObj.Add("ClientID", this.ClientID);
            connObj.Add("RedirectUri", this.RedirectUri);
            connObj.Add("ResourceUri", this.ResourceUri);
            connObj.Add("OAuth2AuthorityUri", this.OAuth2AuthorityUri);
            connObj.Add("PowerBIDataSets", this.PowerBIDataSets);
            connObj.Add("UserName", this.UserName);
            connObj.Add("Password", this.Password);

            return connObj;

        }

        public override void ReleaseConnection(object connection)
        {
            base.ReleaseConnection(connection);
            connection = null;
        }


    }

}
