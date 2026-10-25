using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public class CommonsClient : ICommonClient
{
	public readonly string FormController = "Commons";

	public async Task<DataTable> GetAllEmployees()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetAllEmployees");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DateTime> GetServerDate()
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			string text = await helper.GetAsync("GetServerDate");
			if (!string.IsNullOrEmpty(text) && Function.ParseDateTime(text, "yyyyMMdd").HasValue)
			{
				return Convert.ToDateTime(Function.ParseDateTime(text, "yyyyMMdd"));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
		return default;
	}

	public async Task<DataTable> GetTableStructure(string tablename)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("GetAllFieldTable", tablename);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetLicenseCar(string userName, int branch)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetLicenseCar", new { userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> ExistFileInDoc(int docEntry, ApzObjectTypes FormObjectType, string fileName, string fileExt)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("ExistFileInDoc", new { docEntry, FormObjectType, fileName, fileExt });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> ExistFileInDocOpportunity(int oprId, string fileName)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("ExistFileInDocOpportunity", new { oprId, fileName });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable ValidAttachmentEntry(int atcEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return httpClientHelper.Get("ValidAttachmentEntry", new { atcEntry });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetBranchList(string userCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetBranchList", new { userCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDTGeneralSettings()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetDTGeneralSettings");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetADM1()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CompanyDetails");
			return await helper.GetAsync("GetADM1");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> ExistsTransaction()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("ExistsTransaction");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataSet GetDataCombobox(string cardCode)
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
			return httpClientHelper.Get("GetDataCombobox", new { cardCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetPriceList()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetDataPricelist");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public async Task<DataTable> GetDataTypeObject(ApzObjectTypes objType)
	{
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await helper.GetAsync("GetDataTypeObject", new { objType });
	}

	public async Task<DataTable> GetSaleEmployee()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
		return await http.GetAsync("GetSaleEmployee");
	}

	public DataTable GetDataBrach(string userCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetDataBrach", new { userCode });
	}

	public DataTable GetDataBatchAndSerial(int absEntry, string disNumber, string itemCode, string whsCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetDataBatchAndSerial", new { absEntry, disNumber, itemCode, whsCode });
	}

	public DataTable DocumentPrice(string itemCode, string cardCode, DateTime? docDate, double qty, string uomCode, int infoPriceList)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("ItemCode", typeof(string));
		dataTable.Columns.Add("CardCode", typeof(string));
		dataTable.Columns.Add("DocDate", typeof(DateTime));
		dataTable.Columns.Add("Quantity", typeof(double));
		dataTable.Columns.Add("UomCode", typeof(string));
		dataTable.Columns.Add("InfoPriceList", typeof(int));
		DataRow dataRow = dataTable.NewRow();
		dataRow["ItemCode"] = itemCode;
		dataRow["CardCode"] = cardCode;
		dataRow["DocDate"] = docDate ?? DateTime.Today;
		dataRow["Quantity"] = qty;
		dataRow["UomCode"] = uomCode;
		dataRow["InfoPriceList"] = infoPriceList;
		dataTable.Rows.Add(dataRow);
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Post("DocumentPrice", dataTable);
	}

	public DataTable ItemMaster(string itemCode, int BPLId, Enums.ItemObjectType itObjType = Enums.ItemObjectType.SellItem, string cardcode = "", DateTime? docdate = null, double quantity = 1.0, string subCatNum = "")
	{
		string value = (docdate.HasValue ? Convert.ToDateTime(docdate).ToString("yyyyMMdd") : "");
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add(new DataColumn("itemCode", typeof(string)));
		dataTable.Columns.Add(new DataColumn("BPLId", typeof(int)));
		dataTable.Columns.Add(new DataColumn("itObjType", typeof(int)));
		dataTable.Columns.Add(new DataColumn("cardcode", typeof(string)));
		dataTable.Columns.Add(new DataColumn("docdate", typeof(string)));
		dataTable.Columns.Add(new DataColumn("quantity", typeof(double)));
		dataTable.Columns.Add(new DataColumn("SubCatNum", typeof(string)));
		DataRow dataRow = dataTable.NewRow();
		dataRow["itemCode"] = itemCode;
		dataRow["BPLId"] = BPLId;
		dataRow["itObjType"] = itObjType;
		dataRow["cardcode"] = cardcode;
		dataRow["docdate"] = value;
		dataRow["quantity"] = quantity;
		dataRow["SubCatNum"] = subCatNum;
		dataTable.Rows.Add(dataRow);
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Post("ItemMaster", dataTable);
	}

	public async Task<DataSetResult> ExecuteStore(string sql, List<Common.ParameterValue> lstParamater)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
			DataSet dataSet = await helper.PostAsync("ExecuteStore", new Tuple<string, List<Common.ParameterValue>>(sql, lstParamater));
			if (dataSet.IsNotNull())
			{
				DataTable dataTable = dataSet.Tables["Message"];
				if (dataTable.IsNotNull())
				{
					string text = Function.ToString(dataTable.Rows[0]["MsgType"]);
					string msg = Function.ToString(dataTable.Rows[0]["Message"]);
					if (text == "200")
					{
						dataSet.Tables.Remove("Message");
						return new DataSetResult("", dataSet);
					}
					if (text == "5xx")
					{
						return new DataSetResult(msg);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
		return new DataSetResult();
	}

	public async Task<DataTableResult> ExecuteQuery(string sql)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
			DataSet dataSet = await helper.PostAsync("Query", sql);
			if (dataSet != null && dataSet.Tables.Count > 1)
			{
				DataTable dataTable = dataSet.Tables["Message"];
				if (dataTable.IsNotNull())
				{
					string text = Function.ToString(dataTable.Rows[0]["MsgType"]);
					string msg = Function.ToString(dataTable.Rows[0]["Message"]);
					if (text == "200")
					{
						return new DataTableResult("", dataSet.Tables[1]);
					}
					if (text == "5xx")
					{
						return new DataTableResult(msg);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTableResult(ex.Message);
		}
		return new DataTableResult();
	}

	public async Task<DataTable> LoadSeriesByPostingDate(string usercode, ApzObjectTypes apzObjectTypes, string docSubType, string postingDate)
	{
		try
		{
			using HttpClientHelper<DataTable> client = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await client.GetAsync("LoadSeriesByPostingDate", new { usercode, apzObjectTypes, docSubType, postingDate });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetCodeBySeries(string s, string o, int b)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return httpClientHelper.Get("GetCodeBySeries", new { s, o, b });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataTable GetListSeries(string o, int b, string docSubType)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return httpClientHelper.Get("GetListSeries", new { o, b, docSubType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataSet ExecuteSql(string sql, List<QueryParamater> prs, string filter = "")
	{
		if (prs == null)
		{
			prs = new List<QueryParamater>();
		}
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
		return httpClientHelper.Post("ExecuteSql", new Tuple<string, List<QueryParamater>, string>(sql, prs, filter));
	}

	public DataTable GetExecuteSqlResult(string sql, List<QueryParamater> prs, string filter = "")
	{
		if (prs == null)
		{
			prs = new List<QueryParamater>();
		}
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
		DataSet dataSet = httpClientHelper.Post("ExecuteSql", new Tuple<string, List<QueryParamater>, string>(sql, prs, filter));
		if (dataSet != null && dataSet.Tables.Count > 1)
		{
			DataTable dataTable = dataSet.Tables["Message"];
			if (dataTable.IsNotNull())
			{
				string text = Function.ToString(dataTable.Rows[0]["MsgType"]);
				string message = Function.ToString(dataTable.Rows[0]["Message"]);
				if (text == "5xx")
				{
					throw new Exception(message);
				}
			}
			return dataSet.Tables[1];
		}
		throw new Exception("Could not get source data");
	}

	public DataTable GetExecuteStoreResult(string sql, List<QueryParamater> prs = null)
	{
		if (prs == null)
		{
			prs = new List<QueryParamater>();
		}
		using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
		DataSet dataSet = httpClientHelper.Post("ExecuteSqlStoredProcedure", new Tuple<string, List<QueryParamater>>(sql, prs));
		if (dataSet != null && dataSet.Tables.Count > 1)
		{
			DataTable dataTable = dataSet.Tables["Message"];
			if (dataTable.IsNotNull())
			{
				string text = Function.ToString(dataTable.Rows[0]["MsgType"]);
				string message = Function.ToString(dataTable.Rows[0]["Message"]);
				if (text == "5xx")
				{
					throw new Exception(message);
				}
			}
			return dataSet.Tables[1];
		}
		throw new Exception("Could not get source data");
	}

	public bool CheckExistsByCodeObject(Enums.TypeCFL type, string code)
	{
		HttpResult httpResult;
		using (HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController))
		{
			httpResult = httpClientHelper.Get("CheckExistsByCodeObject", new { type, code });
		}
		return Function.ParseBool(httpResult.Content);
	}

	public DataTable GetOnstockItem(string itemCode, string whsCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetItemOnwh", new
		{
			item = itemCode,
			wh = whsCode
		});
	}

	public DataTable GetDataItemCode(string itemCode, int priceList, string userName, int branch)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetDataItemByCode", new { itemCode, priceList, userName, branch });
	}

	public DataTable GetBusinessPartner(string cardCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetDataBPByCode", new { cardCode });
	}

	public async Task<DataTable> GetCurrency()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await http.GetAsync("GetCurrency");
	}

	public async Task<DataTable> GetDataUomGroup()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await http.GetAsync("GetDataUomGroup");
	}

	public string GetFileNameReport(string code)
	{
		using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetFileNameReport", new { code });
	}

	public DataTable GetDefaultSeries(string obj, string docSubType, int userId)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetDefaultSeries", new { obj, docSubType, userId });
	}

	public DataTable GetDataTableNewRow(string tableName)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetDataTableNewRow", new { tableName });
	}

	public DataTable GetFormulaSearchByTable(string tblName)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetFormulaSearchByTable", new { tblName });
	}

	public DataTable GetCflFormulars(string objType)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetCflFormulars", new { objType });
	}

	public async Task<DataTable> GetDataOperationByCode(string opeCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetDataOperationByCode", new { opeCode });
	}

	public async Task<DataTable> GetPrintPreferences(ApzObjectTypes objType, string userName, int branch)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetPrintPreferences", new { objType, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}

	public DataSet ExecuteTemporyTable(string querytext, DataTable temporaryTable, string temporaryTableName)
	{
		try
		{
			using HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, FormController);
			return httpClientHelper.Post("ExecuteTemporyTable", new Tuple<string, DataTable, string>(querytext, temporaryTable, temporaryTableName));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<HttpResult> DoQuery(List<ContextActionDoQuery> doquery)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("DoQuery", doquery);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<HttpResult> DoQueryWithTempTable(string queryText, string tempTableName, DataTable tempTableData)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("DoQueryWithTempTable", new Tuple<string, string, DataTable>(queryText, tempTableName, tempTableData));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public DataTable GetFreight(string tableName, string docEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return httpClientHelper.Get("GetFreight", new
			{
				tableName = tableName,
				DocEntry = docEntry
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public DataTable CreateFreightGridDatasource(string vatType)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return httpClientHelper.Get("CreateFreightGridDatasource", new { vatType });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}

	public async Task<List<Common.Table_ID>> GetDatabaseListByByDomain(string domainName)
	{
		using HttpClientHelper<Common.Table_ID> commonClientHelper = new HttpClientHelper<Common.Table_ID>(Configuration.ApiServer, "Commons");
		return await commonClientHelper.GetListAsync("GetDbList", new
		{
			domain = domainName
		});
	}

	public async Task<CompanyInfoImpl> GetCompanyInfo()
	{
		using HttpClientHelper<CompanyInfoImpl> commonClientHelper = new HttpClientHelper<CompanyInfoImpl>(Configuration.ApiServer, "Commons");
		return await commonClientHelper.GetAsync("GetCompanyInfo");
	}

	public async Task<Tuple<string, DataTable>> GetDataByObject(ApzObjectTypes objType, string userName, string lngCode)
	{
		using HttpClientHelper<Tuple<string, DataTable>> hp = new HttpClientHelper<Tuple<string, DataTable>>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("GetDataByObject", new { objType, userName, lngCode });
	}

	public async Task<DataTable> GetContactPerson(string cardCode)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("GetContactPerson", new { cardCode });
	}

	public async Task<string> CheckIsIns(ApzObjectTypes oInvoices, int docNum)
	{
		using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("CheckIsIns", new
		{
			type = (int)oInvoices,
			docNum = docNum
		});
	}

	public async Task<string> CheckPosted(ApzObjectTypes oDownPayment, int docEntry)
	{
		using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("CheckPosted", new
		{
			type = (int)oDownPayment,
			docEntry = docEntry
		});
	}

	public async Task<HttpResult> CheckDocumentDuplicateBpRefNo(ApzObjectTypes objType, string bpCode, string refNo)
	{
		using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("CheckDocumentDuplicateBpRefNo", new { objType, bpCode, refNo });
	}

	public async Task<string> GetSystemSuperUser()
	{
		using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("GetSystemSuperUser");
	}

	public DataTable GetDataState(string countryCode)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetDataState", new { countryCode });
	}

	public DataTable GetTableStructureWithDefaultValue(string tableName)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return httpClientHelper.Get("GetTableStructureWithDefaultValue", new { tableName });
	}

	public async Task<string> GetGoogleMap()
	{
		using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("GetGoogleMap");
	}

	public async Task<DataTable> GetEInvoiceToken(string usercode)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("GetEInvoiceToken", new { usercode });
	}

	public async Task<DataTable> GetAccessLog(string user, string fromDate, string toDate)
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("GetAccessLog", new { user, fromDate, toDate });
	}

	public async Task<DataTable> GetAccessLogLastLogin()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("GetAccessLogLastLogin");
	}

	public async Task<DataTable> GetAccessLogUser()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("GetAccessLogUser");
	}

	public async Task<string> CheckExistColumn(string table, string column)
	{
		using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
		return await hp.GetAsync("CheckExistColumn", new { table, column });
	}

	public async Task<HttpResult> ProcessSAPJson(ProcessJsonImpl data)
	{
		try
		{
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("ProcessSAPJson", data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			throw;
		}
	}
}
