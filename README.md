SSIS adapter for Power BI REST api

You can find all information related to Power BI REST api here: https://msdn.microsoft.com/library/dn877544

For now, the api is very limited. It only allows you to:
  1. Create new DataSet and Tables in it.
  2. Get list of all DataSets.
  2. Insert new records into tables.
  3. Remove all records from Tables.
  
##Type mappings
DT_STR
DT_WSTR
DT_NTEXT
DT_GUID
	string

DT_R4
DT_CY
DT_R8
	double

DT_UI1
DT_I1
DT_BOOL
	Boolean

DT_UI2
DT_I2
DT_UI4
DT_I4
DT_UI8
DT_I8
	Int64

DT_DBTIMESTAMP
DT_DATE
	Datetime

  
How to use this adapter, please refer to the blog post here:
http://ioi.solutions/ssis-adapter-for-power-bi-rest-api/


##Update 26/04/2015
  * Added new Parameter to clean rows in a table before insert.
