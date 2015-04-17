using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Xml.Linq;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;
using IDTSInputColumnCollection = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInputColumnCollection100;
using IDTSExternalMetadataColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSExternalMetadataColumn100;
using IDTSInput = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInput100;
using IDTSInputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSInputColumn100;
using IDTSCustomProperty = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSCustomProperty100;
using IDTSVirtualInputColumn = Microsoft.SqlServer.Dts.Pipeline.Wrapper.IDTSVirtualInputColumn100;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IO;
using System.Reflection;
using Microsoft.SqlServer.Dts.Runtime;

namespace Microsoft.Samples.SqlServer.SSIS.PowerBIAdaptersOnline
{
    [DtsPipelineComponent(DisplayName = "Power BI Destination Online",
        CurrentVersion = 7,
        IconResource = "Microsoft.Samples.SqlServer.SSIS.PowerBIAdaptersOnline.Icons.PowerBIDestination.ico",
        Description = "Add data to Power BI",
        ComponentType = ComponentType.DestinationAdapter)]
    public class SharePointListDestination : PipelineComponent
    {
        private PowerBIConnectionManager.PowerBIConnectionManager powerBIConnManager;

        private string ClientID;
        private string RedirectUri;
        private string ResourceUri;
        private string OAuth2AuthorityUri;
        private string PowerBIDataSets;
        //Cred
        private string UserName;
        private string Password;




        //Data Set Name
        private const string C_DATASET = "Data Set";
        private const string C_DATATABLE = "Tabel";
        private const string C_USERFIFO = "Use basic FIFO";


        private Dictionary<string, int> _bufferLookup;
        private Dictionary<string, DataType> _bufferLookupDataType;
        private Dictionary<string, string> _existingColumnData;
        private CultureInfo _culture;
        private NetworkCredential _credentials;



        private static AuthenticationContext authContext = null;
        private static string token = String.Empty;

        #region Design Time Methods
        /// <summary>
        ///  The ProvideComponentProperties() method provides initialization of the the component 
        ///  when the component is first added to the Data Flow designer.
        /// </summary>
        public override void ProvideComponentProperties()
        {
            // Add the Properties
            AddUserProperties();
        }

        private void AddUserProperties()
        {


            IDTSCustomProperty100 DataSetName = ComponentMetaData.CustomPropertyCollection.New();
            DataSetName.Name = C_DATASET;
            DataSetName.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
            DataSetName.Description = "Data Set Name";
            DataSetName.Value = "<Data Set>";


            IDTSCustomProperty100 DataTableName = ComponentMetaData.CustomPropertyCollection.New();
            DataTableName.Name = C_DATATABLE;
            DataTableName.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
            DataTableName.Description = "Data Table Name";
            DataTableName.Value = "<Data Table>";


            var UseFIFO = ComponentMetaData.CustomPropertyCollection.New();
            UseFIFO.Name = C_USERFIFO;
            UseFIFO.Value = Enums.TrueFalseValue.False;
            UseFIFO.Description = "When loading items, should subfolders within the list also be loaded. Set to 'true' to load child folders.";
            UseFIFO.TypeConverter = typeof(Enums.TrueFalseValue).AssemblyQualifiedName;






            var input = ComponentMetaData.InputCollection.New();
            input.Name = "Component Input";
            input.Description = "This is what we see from the upstream component";
            input.HasSideEffects = true;


            // Add the connection manager (by default)
            var connection = ComponentMetaData.RuntimeConnectionCollection.New();
            connection.Name = "Power BI Credential Connection";


        }


        /// <summary>
        /// Called at design time and runtime. Establishes a connection using a ConnectionManager in the package.
        /// </summary>
        /// <param name="transaction">Not used.</param>


        public override void AcquireConnections(object transaction)
        {
            if (ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager != null)
            {
                ConnectionManager connectionManager = Microsoft.SqlServer.Dts.Runtime.DtsConvert.GetWrapper(
                  ComponentMetaData.RuntimeConnectionCollection[0].ConnectionManager);

                this.powerBIConnManager = connectionManager.InnerObject as PowerBIConnectionManager.PowerBIConnectionManager;

                if (this.powerBIConnManager == null)
                    throw new Exception("Couldn't get the Power BI connection manager, ");



                this.ClientID = this.powerBIConnManager.ClientID;
                this.PowerBIDataSets = this.powerBIConnManager.PowerBIDataSets;
                this.RedirectUri = this.powerBIConnManager.RedirectUri;
                this.ResourceUri = this.powerBIConnManager.ResourceUri;
                this.OAuth2AuthorityUri = this.powerBIConnManager.OAuth2AuthorityUri;
                this.PowerBIDataSets = this.powerBIConnManager.PowerBIDataSets;
                this.UserName = this.powerBIConnManager.UserName;
                this.Password = this.powerBIConnManager.Password;







            }
        }



        /// <summary>
        /// The Validate() function is mostly called during the design-time phase of 
        /// the component. Its main purpose is to perform validation of the contents of the component.
        /// </summary>
        /// <returns></returns>
        [CLSCompliant(false)]
        public override DTSValidationStatus Validate()
        {
            bool canCancel = false;

            if (ComponentMetaData.OutputCollection.Count != 0)
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name,
                    "Unexpected Output found. Destination components do not support outputs.",
                    "", 0, out canCancel);
                return DTSValidationStatus.VS_ISCORRUPT;
            }

            if (ComponentMetaData.InputCollection.Count != 1)
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name,
                    "There must be one input into this component.",
                    "", 0, out canCancel);
                return DTSValidationStatus.VS_ISCORRUPT;
            }


            if (ComponentMetaData.AreInputColumnsValid == false)
            {
                return DTSValidationStatus.VS_NEEDSNEWMETADATA;
            }





            if ((ComponentMetaData.CustomPropertyCollection[C_DATASET].Value == null) ||
             ((ComponentMetaData.CustomPropertyCollection[C_DATASET].Value.ToString()).Length == 0))
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name,
                    "You must set Data Set Name.",
                    "", 0, out canCancel);
                return DTSValidationStatus.VS_ISBROKEN;
            }


            if ((ComponentMetaData.CustomPropertyCollection[C_DATATABLE].Value == null) ||
             ((ComponentMetaData.CustomPropertyCollection[C_DATATABLE].Value.ToString()).Length == 0))
            {
                ComponentMetaData.FireError(0, ComponentMetaData.Name,
                    "You must set Data Table Name.",
                    "", 0, out canCancel);
                return DTSValidationStatus.VS_ISBROKEN;
            }




            if ((ComponentMetaData.InputCollection.Count == 0))
            {
                return DTSValidationStatus.VS_NEEDSNEWMETADATA;
            }

            // Validate the columns defined against an actual SharePoint Site
            var isValid = ValidateSharePointColumns();
            if (isValid != DTSValidationStatus.VS_ISVALID)
                return isValid;

            return base.Validate();
        }

        /// <summary>
        /// Lookup the data dynamically against the SharePoint, and check if the columns marked for output exist
        /// exist and are up to date.
        /// </summary>
        /// <returns></returns>
        private DTSValidationStatus ValidateSharePointColumns()
        {
            bool canCancel;


              

            return DTSValidationStatus.VS_ISVALID;
        }


        /// <summary>
        /// The ReinitializeMetaData() method will be called when the Validate() function returns VS_NEEDSNEWMETADATA. 
        /// Its primary purpose is to repair the component's metadata to a consistent state.
        /// </summary>
        public override void ReinitializeMetaData()
        {
            if (ComponentMetaData.InputCollection.Count > 0)
            {
                var input = ComponentMetaData.InputCollection[0];

                // Capture the existing column names and detail data before data is re-initialized.
                _existingColumnData =
                    (from metaCol in
                         input.ExternalMetadataColumnCollection.Cast<IDTSExternalMetadataColumn>()
                     select new
                     {
                         ColumnName = (string)metaCol.Name,
                         SpColName = metaCol.ID.ToString()
                     }).ToDictionary(a => a.SpColName, a => a.ColumnName);

                // Reset the input columns
                ComponentMetaData.InputCollection[0].InputColumnCollection.RemoveAll();

                // Reload the input path columns
                OnInputPathAttached(ComponentMetaData.InputCollection[0].ID);
            }

            base.ReinitializeMetaData();
        }

        /// <summary>
        /// Setup the metadata and link to this object
        /// </summary>
        /// <param name="inputID"></param>
        public override void OnInputPathAttached(int inputID)
        {
            var input = ComponentMetaData.InputCollection.GetObjectByID(inputID);
            var vInput = input.GetVirtualInput();
            foreach (IDTSVirtualInputColumn vCol in vInput.VirtualInputColumnCollection)
            {
                this.SetUsageType(inputID, vInput, vCol.LineageID, DTSUsageType.UT_READONLY);
            }

            // Load meta information and map to columns
            input.ExternalMetadataColumnCollection.RemoveAll();

            // If the existing columns are not set yet, then initialize them.
            if (_existingColumnData == null)
                _existingColumnData = new Dictionary<string, string>();

            LoadDataSourceInformation(_existingColumnData);
        }

        /// <summary>
        /// Lodas the column data into the dts objects from the datasource for columns
        /// </summary>
        private void LoadDataSourceInformation(Dictionary<string, string> existingColumnData)
        {
            if (ComponentMetaData.InputCollection.Count == 1)
            {
                var input = ComponentMetaData.InputCollection[0];

                CreateExternalMetaDataColumns(input,existingColumnData);
            }
        }


        //TODO
        /*
        /// <summary>
        /// Get the columns that are public
        /// </summary>
        /// <param name="sharepointUrl"></param>
        /// <param name="listName"></param>
        /// <returns></returns>
        private List<SharePointUtilityOnline.DataObject.ColumnData>
            GetAccessibleSharePointColumns(string sharepointUrl, string listName, string viewName)
        {
            List<SharePointUtilityOnline.DataObject.ColumnData> columnList =
                ListServiceUtility.GetFields(new Uri(sharepointUrl), _credentials, listName, viewName);

            // Pull out the ID Field because we want this to be first in the list, and the other columns
            // will keep their order that SharePoint sends them.
            var idField =
                from c in columnList
                where (c.Name == "ID" || c.Name == "FsObjType")
                select c;

            var accessibleColumns =
                from c in columnList
                where (!c.IsHidden && !c.IsReadOnly)
                select c;

            return idField.Union(accessibleColumns).ToList();
        }
        */





        /// <summary>
        /// Connects to SharePoint and gets any columns on the target
        /// </summary>
        /// <param name="input"></param>
        /// <param name="sharepointUrl"></param>
        /// <param name="listName"></param>
        private void CreateExternalMetaDataColumns(
            IDTSInput input,Dictionary<string, string> existingColumnData)
        {

            input.ExternalMetadataColumnCollection.IsUsed = true;

            var inputColumns = (from col in input.InputColumnCollection.Cast<IDTSInputColumn>() select col).ToList();
            foreach (var colItem in inputColumns)
            {
                var dtsColumnMeta = input.ExternalMetadataColumnCollection.New();
                dtsColumnMeta.Name = colItem.Name;
                dtsColumnMeta.DataType = colItem.DataType;
                dtsColumnMeta.Description = colItem.Description;
                dtsColumnMeta.Length = 0;
                dtsColumnMeta.Precision = 0;
                dtsColumnMeta.Scale = 0;
            }


        }

        /// <summary>
        /// Enables updating of an existing version of a component to a newer version
        /// </summary>
        /// <param name="pipelineVersion">Seems to always be 0</param>
        public override void PerformUpgrade(int pipelineVersion)
        {
            ComponentMetaData.CustomPropertyCollection["UserComponentTypeName"].Value = this.GetType().AssemblyQualifiedName;

        }

        /// <summary>
        /// Finds a property by name if it exists
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private IDTSCustomProperty FindCustomProperty(string name)
        {
            foreach (IDTSCustomProperty property in ComponentMetaData.CustomPropertyCollection)
            {
                if (property.Name.ToUpper() == name.ToUpper())
                    return property;
            }
            return null;

        }

        #endregion

        #region Runtime Methods
        /// <summary>
        /// Do any initial setup operations
        /// </summary>
        public override void PreExecute()
        {
            base.PreExecute();

            
            // Get the field names from the input collection
            _bufferLookup = (from col in
                                 ComponentMetaData.InputCollection[0].InputColumnCollection.Cast<IDTSInputColumn>()
                             join metaCol in ComponentMetaData.InputCollection[0].ExternalMetadataColumnCollection.Cast<IDTSExternalMetadataColumn>()
                                  on col.ExternalMetadataColumnID equals metaCol.ID
                             select new
                             {
                                 Name = (string)metaCol.Name,
                                 BufferColumn = BufferManager.FindColumnByLineageID(ComponentMetaData.InputCollection[0].Buffer, col.LineageID)
                             }).ToDictionary(a => a.Name, a => a.BufferColumn);

            // Get the field data types from the input collection
            _bufferLookupDataType = (from col in
                                         ComponentMetaData.InputCollection[0].InputColumnCollection.Cast<IDTSInputColumn>()
                                     join metaCol in ComponentMetaData.InputCollection[0].ExternalMetadataColumnCollection.Cast<IDTSExternalMetadataColumn>()
                                          on col.ExternalMetadataColumnID equals metaCol.ID
                                     select new
                                     {
                                         Name = (string)metaCol.Name,
                                         DataType = col.DataType
                                     }).ToDictionary(a => a.Name, a => a.DataType);


            
            /*
            var input = ComponentMetaData.InputCollection[0];

            var cols = new int[input.InputColumnCollection.Count];

            for (int x = 0; x < input.InputColumnCollection.Count; x++)
            {
                cols[x] = BufferManager.FindColumnByLineageID(input.Buffer, input.InputColumnCollection[x].LineageID);
            }
            */
        }

        /// <summary>
        /// This is where the data is read from the input buffer
        /// </summary>
        /// <param name="inputID"></param>
        /// <param name="buffer"></param>
        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            string sharepointUrl = "";
            string sharepointList = "";
            string sharepointListView = "";
            short batchSize = (short)2;
            Enums.BatchType batchType = Enums.BatchType.Deletion;

            if (!buffer.EndOfRowset)
            {
                // Queue the data up for batching by the sharepoint accessor object
                var dataQueue = new List<Dictionary<string, FieldValue>>();
                while (buffer.NextRow())
                {
                    var rowData = new Dictionary<string, FieldValue>();
                    foreach (var fieldName in _bufferLookup.Keys)
                    {
                        if (buffer.IsNull(_bufferLookup[fieldName]))
                        {
                            // Do nothing, can ignore this field
                        }
                        else
                        {
                            FieldValue filedObj = new FieldValue();
                            switch (_bufferLookupDataType[fieldName])
                            {
                                case DataType.DT_STR:
                                case DataType.DT_WSTR:

                                    filedObj.value = buffer.GetString(_bufferLookup[fieldName]);
                                    filedObj.type = "string";
                                    rowData.Add(fieldName, filedObj);
                                    break;
                                case DataType.DT_NTEXT:
                                    int colDataLength = (int)buffer.GetBlobLength(_bufferLookup[fieldName]);
                                    byte[] stringData = buffer.GetBlobData(_bufferLookup[fieldName], 0, colDataLength);

                                    filedObj.value = Encoding.Unicode.GetString(stringData);
                                    filedObj.type = "string";
                                    rowData.Add(fieldName, filedObj);
                                    break;
                                case DataType.DT_R4:
                                    filedObj.value = buffer.GetSingle(_bufferLookup[fieldName]).ToString(_culture);
                                    filedObj.type = "Double";
                                    rowData.Add(fieldName, filedObj);
                                    break;
                                case DataType.DT_CY:

                                    filedObj.value = buffer.GetDecimal(_bufferLookup[fieldName]).ToString(_culture);
                                    filedObj.type = "Double";
                                    rowData.Add(fieldName, filedObj);
                                    break;
                                case DataType.DT_R8:
                                    filedObj.value = buffer.GetDouble(_bufferLookup[fieldName]).ToString(_culture);
                                    filedObj.type = "Double";
                                    rowData.Add(fieldName, filedObj);
                                    break;
                                case DataType.DT_UI1:
                                case DataType.DT_I1:
                                case DataType.DT_BOOL:

                                    filedObj.value = buffer.GetBoolean(_bufferLookup[fieldName]).ToString(_culture);
                                    filedObj.type = "Boolean";
                                    rowData.Add(fieldName, filedObj);

                                    break;
                                case DataType.DT_UI2:
                                case DataType.DT_I2:

                                    filedObj.value = buffer.GetInt16(_bufferLookup[fieldName]).ToString(_culture);
                                    filedObj.type = "Int64";
                                    rowData.Add(fieldName, filedObj);
                                    break;
                                case DataType.DT_UI4:
                                case DataType.DT_I4:

                                    filedObj.value = buffer.GetInt32(_bufferLookup[fieldName]).ToString(_culture);
                                    filedObj.type = "Int64";
                                    rowData.Add(fieldName, filedObj);
                                    break;
                                case DataType.DT_UI8:
                                case DataType.DT_I8:

                                    filedObj.value = buffer.GetInt64(_bufferLookup[fieldName]).ToString(_culture);
                                    filedObj.type = "Int64";
                                    rowData.Add(fieldName, filedObj);
                                    break;
                                case DataType.DT_GUID:

                                    filedObj.value = buffer.GetGuid(_bufferLookup[fieldName]).ToString();
                                    filedObj.type = "String";
                                    rowData.Add(fieldName, filedObj);
                                    break;
                                case DataType.DT_DBTIMESTAMP:

                                    filedObj.value = buffer.GetDateTime(_bufferLookup[fieldName]).ToString("u").Replace(" ", "T");
                                    filedObj.type = "Datetime";
                                    rowData.Add(fieldName, filedObj);

                                    break;

                                case DataType.DT_DATE:
                                    
                                    filedObj.value = buffer.GetDateTime(_bufferLookup[fieldName]).ToString("yyyy-MM-dd");
                                    filedObj.type = "Datetime";
                                    rowData.Add(fieldName, filedObj);

                                    break;
                            }
                        }
                    }
                    dataQueue.Add(rowData);
                }

                bool fireAgain = false;

  
                if (dataQueue.Count() > 0)
                {
                    System.Diagnostics.Stopwatch timer = new System.Diagnostics.Stopwatch();
                    timer.Start();
                    System.Xml.Linq.XElement resultData;


                    CreateDataset(dataQueue);
                    AddClassRows(dataQueue);





                    timer.Stop();


                    /*
                    var errorRows = from result in resultData.Descendants("errorCode")
                                    select result.Parent;

                    int successRowsWritten = resultData.Elements().Count() - errorRows.Count();
                    string infoMsg = string.Format(CultureInfo.InvariantCulture,
                        "Affected {0} records in list '{1}' at '{2}'. Elapsed time is {3}ms",
                        successRowsWritten,
                        sharepointList,
                        sharepointUrl,
                        timer.ElapsedMilliseconds);
                    ComponentMetaData.FireInformation(0, ComponentMetaData.Name, infoMsg, "", 0, ref fireAgain);
                    ComponentMetaData.IncrementPipelinePerfCounter(
                        DTS_PIPELINE_CTR_ROWSWRITTEN, (uint)successRowsWritten);

                    // Shovel any error rows to the error flow
                    bool cancel;
                    int errorIter = 0;
                    foreach (var row in errorRows)
                    {
                        // Do not flood the error log.
                        errorIter++;
                        if (errorIter > 10)
                        {
                            ComponentMetaData.FireError(0,
                                ComponentMetaData.Name,
                                "Total of " + errorRows.Count().ToString(_culture) + ", only  showing first 10.", "", 0, out cancel);
                            return;

                        }

                        string idString = "";
                        XAttribute attrib = row.Element("row").Attribute("ID");
                        if (attrib != null)
                            idString = "(SP ID=" + attrib.Value + ")";

                        string errorString = string.Format(CultureInfo.InvariantCulture,
                            "Error on row {0}: {1} - {2} {3}",
                            row.Attribute("ID"),
                            row.Element("errorCode").Value,
                            row.Element("errorDescription").Value,
                            idString);

                        ComponentMetaData.FireError(0, ComponentMetaData.Name, errorString, "", 0, out cancel);

                        // Need to throw an exception, or else this step's box is green (should be red), even though the flow
                        // is marked as failure regardless.
                        throw new PipelineProcessException("Errors detected in this component - see SSIS Errors");
                    }


                    */
                }
                else
                {
                    ComponentMetaData.FireInformation(0, ComponentMetaData.Name,
                        "No rows found to update in destination.", "", 0, ref fireAgain);
                }

            }
        }
        #endregion



#region Custom Code



        private void CreateDataset(List<Dictionary<string, FieldValue>> data)
        {
            //In a production application, use more specific exception handling.           
            try
            {
                //Create a POST web request to list all datasets

                Enums.TrueFalseValue isRecursive = (Enums.TrueFalseValue)ComponentMetaData.CustomPropertyCollection[C_USERFIFO].Value;
                var useFifostr = "";
                if (isRecursive == Enums.TrueFalseValue.True)
                    useFifostr = "?defaultRetentionPolicy=basicFIFO";


                HttpWebRequest request = DatasetRequest(this.PowerBIDataSets + useFifostr, "POST", AccessToken());

                var datasets = GetAllDatasets().Datasets((string)ComponentMetaData.CustomPropertyCollection[C_DATASET].Value);

                if (datasets.Count() == 0)
                {
                    //POST request using the json schema from Product
                    PostRequest(request, CreateNewDataSet(data));
                }
                else
                {

                }
            }
            catch (Exception ex)
            {
            }
        }


        private string createRows(List<Dictionary<string, FieldValue>> data)
        {

            StringBuilder jsonSchemaBuilder = new StringBuilder();
            string typeName = string.Empty;

            jsonSchemaBuilder.Append("{\"rows\":[");

            foreach (Dictionary<string, FieldValue> n in data)
            {

                    jsonSchemaBuilder.Append("{");


                    foreach (KeyValuePair<string, FieldValue> m in n)
                    {
                        jsonSchemaBuilder.Append(string.Format("\"{0}\": \"{1}\",", m.Key, m.Value.value));
                    }

                    jsonSchemaBuilder.Remove(jsonSchemaBuilder.ToString().Length - 1, 1);
                    jsonSchemaBuilder.Append("},");

            }




            jsonSchemaBuilder.Remove(jsonSchemaBuilder.ToString().Length - 1, 1);
            jsonSchemaBuilder.Append("]}");

            return jsonSchemaBuilder.ToString();


        }


        private string CreateNewDataSet(List<Dictionary<string, FieldValue>> data)
        {
            StringBuilder jsonSchemaBuilder = new StringBuilder();
            string typeName = string.Empty;

            jsonSchemaBuilder.Append(string.Format("{0}\"name\": \"{1}\",\"tables\": [", "{", (string)ComponentMetaData.CustomPropertyCollection[C_DATASET].Value));
            jsonSchemaBuilder.Append(String.Format("{0}\"name\": \"{1}\", ", "{", (string)ComponentMetaData.CustomPropertyCollection[C_DATATABLE].Value));
            jsonSchemaBuilder.Append("\"columns\": [");

           // PropertyInfo[] properties = obj.GetType().GetProperties();


            foreach (KeyValuePair<string,FieldValue> p in data.First())
            {

                jsonSchemaBuilder.Append(string.Format("{0} \"name\": \"{1}\", \"dataType\": \"{2}\"{3},", "{", p.Key, p.Value.type, "}"));
            }
            
            jsonSchemaBuilder.Remove(jsonSchemaBuilder.ToString().Length - 1, 1);
            jsonSchemaBuilder.Append("]}]}");

            return jsonSchemaBuilder.ToString();
        }


        private void AddClassRows(List<Dictionary<string, FieldValue>> data)
        {
            //Get dataset id from a table name
            string datasetId = GetAllDatasets().Datasets((string)ComponentMetaData.CustomPropertyCollection[C_DATASET].Value).First()["id"].ToString();

            //In a production application, use more specific exception handling. 
            try
            {
                HttpWebRequest request = DatasetRequest(String.Format("{0}/{1}/tables/{2}/rows", this.PowerBIDataSets, datasetId, (string)ComponentMetaData.CustomPropertyCollection[C_DATATABLE].Value), "POST", AccessToken());

                //POST request using the json from a list of Product
                //NOTE: Posting rows to a model that is not created through the Power BI API is not currently supported. 
                //      Please create a dataset by posting it through the API following the instructions on http://dev.powerbi.com.
                PostRequest(request, createRows(data));

            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.Message);
            }
        }



        private static string PostRequest(HttpWebRequest request, string json)
        {
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(json);
            request.ContentLength = byteArray.Length;

            //Write JSON byte[] into a Stream
            using (Stream writer = request.GetRequestStream())
            {
                writer.Write(byteArray, 0, byteArray.Length);
            }
            return GetResponse(request);
        }


        private string TestConnection()
        {

            // Check the connection for redirects
            HttpWebRequest request = System.Net.WebRequest.Create(this.PowerBIDataSets) as System.Net.HttpWebRequest;
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


            return this.PowerBIDataSets;
        }



        private string AccessToken()
        {
            bool fireAgain = false;

            try
            {
                if (token == String.Empty)
                {
                    // Create an instance of TokenCache to cache the access token
                    TokenCache TC = new TokenCache();
                    // Create an instance of AuthenticationContext to acquire an Azure access token
                    authContext = new AuthenticationContext(this.OAuth2AuthorityUri, TC);
                    // Call AcquireToken to get an Azure token from Azure Active Directory token issuance endpoint
                    //token = authContext.AcquireToken(resourceUri, clientID, new Uri(redirectUri)).AccessToken.ToString();

                    UserCredential user = new UserCredential(this.UserName, this.Password);
                    token = authContext.AcquireToken(this.ResourceUri, this.ClientID, user).AccessToken.ToString();
                    ComponentMetaData.FireInformation(0, ComponentMetaData.Name, "Toked Acquired", "", 0, ref fireAgain);
                }
                else
                {
                    // Get the token in the cache
                    token = authContext.AcquireTokenSilent(this.ResourceUri,this.ClientID).AccessToken;
                    ComponentMetaData.FireInformation(0, ComponentMetaData.Name, "Refresh Token Acquired", "", 0, ref fireAgain);
                }

                return token;
            }
            catch (Exception e)
            {
                ComponentMetaData.FireInformation(0, ComponentMetaData.Name, "Token Error:" + e.Message, "", 0, ref fireAgain);
                return "";
            }

        }


        private List<Object> GetAllDatasets()
        {
            List<Object> datasets = null;

            //In a production application, use more specific exception handling.
            try
            {
                //Create a GET web request to list all datasets
                HttpWebRequest request = DatasetRequest(this.PowerBIDataSets, "GET", AccessToken());

                //Get HttpWebResponse from GET request
                string responseContent = GetResponse(request);

                //Get list from response
                datasets = responseContent.ToObject<List<Object>>();

            }
            catch (Exception ex)
            {
                //In a production application, handle exception
            }

            return datasets;
        }




        private HttpWebRequest DatasetRequest(string datasetsUri, string method, string accessToken)
        {
            HttpWebRequest request = System.Net.WebRequest.Create(datasetsUri) as System.Net.HttpWebRequest;
            request.KeepAlive = true;
            request.Method = method;
            request.ContentLength = 0;
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", String.Format("Bearer {0}", accessToken));

            return request;
        }


        private static string GetResponse(HttpWebRequest request)
        {
            string response = string.Empty;

            using (HttpWebResponse httpResponse = request.GetResponse() as System.Net.HttpWebResponse)
            {
                //Get StreamReader that holds the response stream
                using (StreamReader reader = new System.IO.StreamReader(httpResponse.GetResponseStream()))
                {
                    response = reader.ReadToEnd();
                }
            }

            return response;
        }




#endregion

    }


    public class Product
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public bool IsCompete { get; set; }
        public DateTime ManufacturedOn { get; set; }
    }



    public class FieldValue
    {
        public string value { get; set; }
        public string type { get; set; }
    }
}
