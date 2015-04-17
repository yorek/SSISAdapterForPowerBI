using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.SqlServer.Dts.Runtime;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Microsoft.Samples.SqlServer.SSIS.PowerBIConnectionManager
{
    public partial class PowerBIConnectionManagerUIForm : Form
    {
        private ConnectionManager connectionManager;
        private IServiceProvider serviceProvider;
        private static AuthenticationContext authContext = null;
        private static string token = String.Empty;

        public PowerBIConnectionManagerUIForm(Microsoft.SqlServer.Dts.Runtime.ConnectionManager connectionManager, IServiceProvider serviceProvider)
          : this()
        {
          this.connectionManager = connectionManager;
          this.serviceProvider = serviceProvider;
 
          SetFormValuesFromConnectionManager();
        }

        private void UpdateConnectionFromControls()
        {

            connectionManager.Properties["ClientID"].SetValue(connectionManager, ClientIdTxt.Text);
            connectionManager.Properties["RedirectUri"].SetValue(connectionManager, RedirectUriTxt.Text);
            connectionManager.Properties["ResourceUri"].SetValue(connectionManager, ResourceUriTxt.Text);
            connectionManager.Properties["OAuth2AuthorityUri"].SetValue(connectionManager, OAuthTxt.Text);
            connectionManager.Properties["PowerBIDataSets"].SetValue(connectionManager, PowerBIDataSetsTxt.Text);
            connectionManager.Properties["UserName"].SetValue(connectionManager, UserNameTxt.Text);
            connectionManager.Properties["Password"].SetValue(connectionManager, PasswordTxt.Text);

        }

        private void SetFormValuesFromConnectionManager()
        {
            string ClientID = connectionManager.Properties["ClientID"].GetValue(connectionManager).ToString();
            string RedirectUri = connectionManager.Properties["RedirectUri"].GetValue(connectionManager).ToString();
            string ResourceUri = connectionManager.Properties["ResourceUri"].GetValue(connectionManager).ToString();
            string OAuth2AuthorityUri = connectionManager.Properties["OAuth2AuthorityUri"].GetValue(connectionManager).ToString();
            string PowerBIDataSets = connectionManager.Properties["PowerBIDataSets"].GetValue(connectionManager).ToString();
            string UserName = connectionManager.Properties["UserName"].GetValue(connectionManager).ToString();
            string Password = connectionManager.Properties["Password"].GetValue(connectionManager).ToString();


            if (!string.IsNullOrWhiteSpace(ClientID))
            {
                ClientIdTxt.Text = ClientID;
            }
            if (!string.IsNullOrWhiteSpace(RedirectUri))
            {
                RedirectUriTxt.Text = RedirectUri;
            }
            if (!string.IsNullOrWhiteSpace(ResourceUri))
            {
                ResourceUriTxt.Text = ResourceUri;
            }
            if (!string.IsNullOrWhiteSpace(OAuth2AuthorityUri))
            {
                OAuthTxt.Text = OAuth2AuthorityUri;
            }
            if (!string.IsNullOrWhiteSpace(PowerBIDataSets))
            {
                PowerBIDataSetsTxt.Text = PowerBIDataSets;
            }
            if (!string.IsNullOrWhiteSpace(UserName))
            {
                UserNameTxt.Text = UserName;
            }
            if (!string.IsNullOrWhiteSpace(Password))
            {
                PasswordTxt.Text = Password;
            }
        }

        public PowerBIConnectionManagerUIForm()
        {
            InitializeComponent();
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void CancelBtn_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;

            this.Close();
        }

        private void OkBtn_Click(object sender, EventArgs e)
        {
            UpdateConnectionFromControls();

            this.DialogResult = DialogResult.OK;

            this.Close();
        }

        private void TestBtn_Click(object sender, EventArgs e)
        {
            TestBtn.Enabled = false;
            OkBtn.Enabled = false;
            CancelBtn.Enabled = false;

            string res = AccessToken();


            TestBtn.Enabled = true;
            OkBtn.Enabled = true;
            CancelBtn.Enabled = true;

            if (res != null)
            {
                connectionManager.Properties["PowerBIDataSets"].SetValue(connectionManager, TestConnection());
                MessageBox.Show("Test connection verified", "Power BI Connection Manager", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("Test connection failed", "Power BI Connection Manager", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        private string TestConnection()
        {

            // Check the connection for redirects
            HttpWebRequest request = System.Net.WebRequest.Create(PowerBIDataSetsTxt.Text) as System.Net.HttpWebRequest;
            request.KeepAlive = true;
            request.Method = "GET";
            request.ContentLength = 0;
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", String.Format("Bearer {0}", AccessToken()));
            request.AllowAutoRedirect = false;

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            if (response.StatusCode == HttpStatusCode.TemporaryRedirect)
            {
                return response.Headers["Location"];
            }


            return PowerBIDataSetsTxt.Text;
        }


        private string AccessToken()
        {
            bool fireAgain = false;
            connectionManager.Properties["PowerBIDataSets"].SetValue(connectionManager, PowerBIDataSetsTxt.Text);
            connectionManager.Properties["ClientID"].SetValue(connectionManager, ClientIdTxt.Text);
            connectionManager.Properties["RedirectUri"].SetValue(connectionManager, RedirectUriTxt.Text);
            connectionManager.Properties["ResourceUri"].SetValue(connectionManager, ResourceUriTxt.Text);
            connectionManager.Properties["OAuth2AuthorityUri"].SetValue(connectionManager, OAuthTxt.Text);
            connectionManager.Properties["PowerBIDataSets"].SetValue(connectionManager, PowerBIDataSetsTxt.Text);
            connectionManager.Properties["UserName"].SetValue(connectionManager, UserNameTxt.Text);
            connectionManager.Properties["Password"].SetValue(connectionManager, PasswordTxt.Text);


            try
            {
                if (token == String.Empty)
                {
                    // Create an instance of TokenCache to cache the access token
                    TokenCache TC = new TokenCache();
                    // Create an instance of AuthenticationContext to acquire an Azure access token
                    authContext = new AuthenticationContext(OAuthTxt.Text, TC);
                    // Call AcquireToken to get an Azure token from Azure Active Directory token issuance endpoint
                    //token = authContext.AcquireToken(resourceUri, clientID, new Uri(redirectUri)).AccessToken.ToString();

                    UserCredential user = new UserCredential(UserNameTxt.Text, PasswordTxt.Text);
                    token = authContext.AcquireToken(ResourceUriTxt.Text, ClientIdTxt.Text, user).AccessToken.ToString();
                }
                else
                {
                    // Get the token in the cache
                    token = authContext.AcquireTokenSilent(ResourceUriTxt.Text, ClientIdTxt.Text).AccessToken;

                }

                return token;
            }
            catch (Exception e)
            {
                return null;
            }

        }

    }
}
