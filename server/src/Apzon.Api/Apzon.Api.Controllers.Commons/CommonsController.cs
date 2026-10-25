using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Api.Models;
using Apzon.Commons;
using Apzon.Commons.ApzJson;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.DataServices;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services;
using Newtonsoft.Json;

namespace Apzon.Api.Controllers.Commons;

public class CommonsController : BaseApiController
{
	private readonly Apzon.DataServices.IUnitOfWork _uow = Apzon.DataServices.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.ServiceAddress, Global.DatabaseSystemType, Global.ServiceType);

	[HttpGet]
	public DataTable GetAllEmployees()
	{
		return UnitOfWork.Commons.GetAllEmployees();
	}

	[HttpGet]
	public DataTable GetSaleEmployee()
	{
		return UnitOfWork.Commons.SaleEmployee();
	}

	[HttpGet]
	public DataTable GetContactPerson(string cardCode)
	{
		return UnitOfWork.Commons.ContactPerson(cardCode);
	}

	[HttpGet]
	public DataTable GetVatGroup(string category)
	{
		return UnitOfWork.Commons.VatGroup(category);
	}

	[HttpGet]
	public DataTable GetAllContactPerson()
	{
		return UnitOfWork.Commons.GetAllContactPerson();
	}

	[HttpGet]
	public DataTable GetPosition()
	{
		return UnitOfWork.Commons.GetPosition();
	}

	[HttpGet]
	public DataTable GetUserCode()
	{
		return UnitOfWork.Commons.GetUserCode();
	}

	[HttpGet]
	public DataTable LoadDataCashFlow()
	{
		return UnitOfWork.Commons.LoadDataCashFlow();
	}

	[HttpGet]
	public DataTable ListChooseItemMaster(string itemCode, int BPLId, Enums.ItemObjectType itObjType = Enums.ItemObjectType.SellItem, string userName = "", string cardcode = "", DateTime? docdate = null, double quantity = 1.0)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataTable();
			}
			string userName2 = SessionUserInfo.UserName;
			return UnitOfWork.Commons.ListChooseItemMaster(itemCode, itObjType, userName2, BPLId, userName, cardcode, docdate, quantity);
		}
		return new DataTable();
	}

	[HttpPost]
	public DataTable ItemMaster([FromBody] DataTable dtSearch)
	{
		if (string.IsNullOrEmpty(SessionId))
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
			return new DataTable();
		}
		if (dtSearch != null && dtSearch.Rows.Count > 0)
		{
			string itemCode = Function.ToString(dtSearch.Rows[0]["itemCode"]);
			int bPLId = Function.ParseInt(dtSearch.Rows[0]["BPLId"]);
			Enums.ItemObjectType itObjType = (Enums.ItemObjectType)Function.ParseInt(dtSearch.Rows[0]["itObjType"]);
			string cardcode = Function.ToString(dtSearch.Rows[0]["cardcode"]);
			double quantity = Function.ParseDouble(dtSearch.Rows[0]["quantity"]);
			DateTime value = Function.ParseDateTime(dtSearch.Rows[0]["docdate"]) ?? DateTime.Today;
			string userName = SessionUserInfo.UserName;
			string subCatNum = Function.ToString(dtSearch.Rows[0]["SubCatNum"]);
			return UnitOfWork.Commons.ItemMaster(itemCode, itObjType, userName, bPLId, cardcode, value, quantity, subCatNum);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable CheckExistsItem(string itemCode)
	{
		return UnitOfWork.Commons.CheckExistsItem(itemCode);
	}

	[HttpGet]
	public DataTable ItemMaterInventory(int BPLId)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataTable();
			}
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.Commons.ItemMaterInventory(userName, BPLId);
		}
		string userName2 = SessionUserInfo.UserName;
		return UnitOfWork.Commons.ItemMaterInventory(userName2, BPLId);
	}

	[HttpGet]
	public DataTable GetItemMasterTypes(string itemTypes, int BPLId)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataTable();
			}
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.Commons.ItemMasterTypes(itemTypes, userName, BPLId);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetItemMasterByGroup(string itemGroup, int BPLId)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "INVALIDSESSION");
				return new DataTable();
			}
			string userName = SessionUserInfo.UserName;
			return UnitOfWork.Commons.ItemMasterByGroup(itemGroup, userName, BPLId);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable ItemMasters(List<Common.ListItems> itemCode)
	{
		return UnitOfWork.Commons.ItemMasters(itemCode);
	}

	[HttpGet]
	public DataTable GetWareHose()
	{
		return UnitOfWork.Commons.WareHouse();
	}

	[HttpGet]
	public DateTime GetDate()
	{
		return UnitOfWork.Commons.GetDate();
	}

	[HttpGet]
	public string GetServerDate()
	{
		return UnitOfWork.Commons.GetServerDate();
	}

	[HttpGet]
	public DataSet GetDataCombobox(string cardCode)
	{
		return UnitOfWork.Commons.GetDataCombobox(cardCode);
	}

	[HttpGet]
	public DataTable ListDocByCardCode(ApzObjectTypes type, string cardCode)
	{
		return UnitOfWork.Commons.ListDocByCardCode(type, cardCode);
	}

	[HttpGet]
	public DataTable GetListDocumentCopy(ApzObjectTypes objType, ApzObjectTypes toType, string cardCode, string postingDate = "")
	{
		return UnitOfWork.Commons.GetListDocumentCopy(objType, toType, cardCode, postingDate);
	}

	[HttpGet]
	public List<DocumentsImpl> ListDocDraft(ApzObjectTypes type)
	{
		return UnitOfWork.Commons.ListDocDraft(type);
	}

	[HttpGet]
	public DataTable ListDocByCardCode_SalesOpportunity(ApzObjectTypes type, string cardCode)
	{
		return UnitOfWork.Commons.ListDocByCardCode_SalesOpportunity(type, cardCode);
	}

	[HttpGet]
	public List<BusinessPartnersImpl> GetBusinessPartner(string cardCode, ApzObjectTypes type = ApzObjectTypes.oNone)
	{
		return UnitOfWork.Commons.GetBusinessPartner(cardCode, type);
	}

	[HttpGet]
	public DataTable GetShipType(int id)
	{
		DataTable shipType = UnitOfWork.Commons.GetShipType(id);
		if (shipType != null && shipType.Rows.Count > 0)
		{
			DataRow dataRow = shipType.NewRow();
			dataRow[shipType.Columns[0].ColumnName] = -1;
			shipType.Rows.Add(dataRow);
		}
		return shipType;
	}

	[HttpGet]
	public DataTable GetBillTo(string cardCode)
	{
		DataTable billTo = UnitOfWork.Commons.GetBillTo(cardCode);
		if (billTo != null && billTo.Rows.Count > 0)
		{
			DataRow dataRow = billTo.NewRow();
			dataRow["CardCode"] = cardCode;
			billTo.Rows.InsertAt(dataRow, 0);
		}
		return billTo;
	}

	[HttpGet]
	public DataTable GetShipTo(string cardCode)
	{
		DataTable shipTo = UnitOfWork.Commons.GetShipTo(cardCode);
		if (shipTo != null && shipTo.Rows.Count > 0)
		{
			DataRow dataRow = shipTo.NewRow();
			dataRow["CardCode"] = cardCode;
			shipTo.Rows.InsertAt(dataRow, 0);
		}
		return shipTo;
	}

	[HttpGet]
	public DataTable GetRequester(string requester)
	{
		return UnitOfWork.Commons.GetRequester(requester);
	}

	[HttpGet]
	public DataTable GetAcctCode(string acctCode)
	{
		return UnitOfWork.Commons.GetAcctCode(acctCode);
	}

	[HttpGet]
	public List<DocumentsImpl> ListTranferRequest(ApzObjectTypes type)
	{
		return UnitOfWork.Commons.ListTranferRequest(type);
	}

	[HttpGet]
	public DataTable GetSchedule()
	{
		return UnitOfWork.Commons.GetSchedule();
	}

	[HttpGet]
	public DataTable GetProductionOrder()
	{
		return UnitOfWork.Commons.GetProductionOrder();
	}

	[HttpGet]
	public DataTable GetMRP()
	{
		return UnitOfWork.Commons.GetMRP();
	}

	[HttpGet]
	public DataTable GetForeCast()
	{
		return UnitOfWork.Commons.ForeCast();
	}

	[HttpGet]
	public DataTable GetCustomer()
	{
		return UnitOfWork.Commons.GetCustomer();
	}

	[HttpGet]
	public List<Common.Table_ID> GetDbList(string domain)
	{
		try
		{
			List<Common.Table_ID> databaseList = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType).Commons.GetDatabaseList(Global.LoginSap, domain);
			if (Global.ListAccessDb != null && Global.ListAccessDb.Count != 0)
			{
				return databaseList.Where((Common.Table_ID d) => Global.ListAccessDb.Contains(d.ID.ToString().ToUpper())).ToList();
			}
			if (Global.ListNotAccessDb != null && Global.ListNotAccessDb.Count != 0)
			{
				return databaseList.Where((Common.Table_ID d) => !Global.ListNotAccessDb.Contains(d.ID.ToString().ToUpper())).ToList();
			}
			return databaseList;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<Common.Table_ID>();
	}

	[HttpGet]
	public List<Common.Table_ID> GetDbList()
	{
		return new List<Common.Table_ID>();
	}

	[HttpGet]
	public List<Common.Table_ID> GetNewDbList(string domain)
	{
		try
		{
			List<Common.Table_ID> newDbList = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType).Commons.GetNewDbList(Global.CloudSystem, Global.LoginSap, domain, Function.Encrypt(RSA.PublicKey, Global.CommonDbServer), Function.Encrypt(RSA.PublicKey, Global.CommonDbUserName), Function.Encrypt(RSA.PublicKey, Global.CommonDbPassword));
			if (Global.ListAccessDb != null && Global.ListAccessDb.Count != 0)
			{
				return newDbList.Where((Common.Table_ID d) => Global.ListAccessDb.Contains(d.ID.ToString().ToUpper())).ToList();
			}
			if (Global.ListNotAccessDb != null && Global.ListNotAccessDb.Count != 0)
			{
				return newDbList.Where((Common.Table_ID d) => !Global.ListNotAccessDb.Contains(d.ID.ToString().ToUpper())).ToList();
			}
			return newDbList;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new List<Common.Table_ID>();
		}
	}

	public int GetMaxDocEntrySaveAsDraf()
	{
		return UnitOfWork.Commons.GetMaxDocEntrySaveAsDraf();
	}

	[HttpGet]
	public DataTable GetFormulars(string objType)
	{
		return UnitOfWork.Commons.GetFormulars(objType);
	}

	[HttpGet]
	public DataTable GetCflFormulars(string objType)
	{
		return UnitOfWork.Commons.GetCflFormulars(objType);
	}

	[HttpPost]
	public DataSet Query([FromBody] string query)
	{
		return UnitOfWork.Commons.Query(query);
	}

	[HttpGet]
	public DataTable GetFormulaSearch(string fieldName, string tblName)
	{
		return UnitOfWork.Commons.GetFormularSearch(fieldName, tblName);
	}

	[HttpGet]
	public DataTable GetFormulaSearchByTable(string tblName)
	{
		return UnitOfWork.Commons.GetFormulaSearchByTable(tblName);
	}

	[HttpPost]
	public string CreateStoreUdf([FromBody] string[] pr)
	{
		try
		{
			return UnitOfWork.Commons.CreateStoreUdf(pr[0], pr[1]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	[HttpGet]
	public DataTable GetColumnNames(string tblName)
	{
		return UnitOfWork.Commons.GetColumnNames(tblName);
	}

	[HttpGet]
	public DataTable GetIRSConfig()
	{
		return UnitOfWork.Commons.GetIRSConfig();
	}

	[HttpPost]
	public DataTable GetAllFieldTable([FromBody] string tablename)
	{
		return UnitOfWork.Commons.GetAllFieldTable(tablename);
	}

	[HttpGet]
	public string GetFieldNameFromTable(bool header, string field)
	{
		return UnitOfWork.Commons.GetFieldNameFromTable(header, field);
	}

	[HttpGet]
	public DataTable GetRecord(int ObjType, string tbl, int DocNum, string TypeRecord, string userCode)
	{
		return UnitOfWork.Commons.GetRecord(ObjType, tbl, DocNum, TypeRecord, userCode);
	}

	[HttpPost]
	public DataTable GetExecute([FromBody] Tuple<string, string> model)
	{
		return UnitOfWork.Commons.GetExecute(model.Item1, model.Item2);
	}

	[HttpPost]
	public DataTable GetCheckQuantityInventory([FromBody] string item)
	{
		return UnitOfWork.Commons.GetCheckQuantityInventory(item);
	}

	[HttpGet]
	public DataTable GetListSQL(string filter)
	{
		return UnitOfWork.Commons.GetListSQL(filter);
	}

	[HttpGet]
	public DataTable GetQuantityInWarehouse(string item, string warehouse)
	{
		return UnitOfWork.Commons.GetQuantityInWarehouse(item, warehouse);
	}

	public DataTable GetAllProject()
	{
		return UnitOfWork.Commons.GetAllProject();
	}

	public DataTable GetRevenueAccount()
	{
		return UnitOfWork.Commons.GetRevenueAccount();
	}

	[HttpGet]
	public DataTable GetUDFFieldValueBOMItem(string Father, int ChildNum)
	{
		return UnitOfWork.Commons.GetUDFFieldValueBOMItem(Father, ChildNum);
	}

	[HttpGet]
	public DataTable GetFreight(string tableName, string DocEntry)
	{
		return UnitOfWork.Commons.GetFreight(tableName, DocEntry);
	}

	[HttpGet]
	public DataTable GetCountry(int user)
	{
		return UnitOfWork.Commons.GetCountry(user);
	}

	[HttpGet]
	public DataTable GetRole()
	{
		return UnitOfWork.Commons.GetRole();
	}

	[HttpGet]
	public DataTable GetQualifications()
	{
		return UnitOfWork.Commons.GetQualifications();
	}

	[HttpGet]
	public DataTable GetMaxID(string table)
	{
		return UnitOfWork.Commons.GetMaxID(table);
	}

	[HttpGet]
	public DataTable GetState(string Code)
	{
		return UnitOfWork.Commons.GetState(Code);
	}

	[HttpGet]
	public DataTable LoadDoc(string tableName)
	{
		return UnitOfWork.Commons.LoadDoc(tableName);
	}

	[HttpGet]
	public DataTable GetAddNewRow(string tableName, int codeMax)
	{
		return UnitOfWork.Commons.GetAddNewRow(tableName, codeMax);
	}

	[HttpGet]
	public DataTable GetDataPricelist()
	{
		return UnitOfWork.Commons.GetDataPricelist();
	}

	[HttpGet]
	public DataTable GetCurrencyByListNum(int listNum)
	{
		return UnitOfWork.Commons.GetCurrencyByListNum(listNum);
	}

	[HttpGet]
	public DataTable GetTableColumns(string table)
	{
		return UnitOfWork.Commons.GetTableColumns(table);
	}

	[HttpGet]
	public DataTable GetDataStructure(string table)
	{
		return UnitOfWork.Commons.GetDataStructure(table);
	}

	[HttpGet]
	public DataTable GetLicenseCar(string userName, int branch)
	{
		return UnitOfWork.Commons.GetLicenseCar(userName, branch);
	}

	[HttpGet]
	public DataTable CheckExistUgpEntry(int itgrCode)
	{
		return UnitOfWork.Commons.CheckExistUgpEntry(itgrCode);
	}

	[HttpPost]
	public DataTable GetDataParamaterSql([FromBody] string sql)
	{
		return UnitOfWork.Commons.GetDataParamaterSql(sql);
	}

	[HttpPost]
	public DataSet ExecuteSql()
	{
		Tuple<string, List<QueryParamater>, string> tuple = JsonConvert.DeserializeObject<Tuple<string, List<QueryParamater>, string>>(Request.Content.ReadAsStringAsync().Result, new JsonConverter[1]
		{
			new ApzJsonConverterTuple()
		});
		return UnitOfWork.Commons.ExecuteSql(tuple.Item1, tuple.Item2, tuple.Item3);
	}

	[HttpPost]
	public HttpResult ExecuteQuery([FromBody] string query)
	{
		if (string.IsNullOrWhiteSpace(query))
		{
			return new HttpResult(Enums.MessageCode.Error, "No query to execute");
		}
		return UnitOfWork.Commons.ExecuteSql(query);
	}

	[HttpPost]
	public DataSet ExecuteSqlDataSet()
	{
		Tuple<string, List<QueryParamater>, string> tuple = JsonConvert.DeserializeObject<Tuple<string, List<QueryParamater>, string>>(Request.Content.ReadAsStringAsync().Result, new JsonConverter[1]
		{
			new ApzJsonConverterTuple()
		});
		return UnitOfWork.Commons.ExecuteSqlDataSet(tuple.Item1, tuple.Item2, tuple.Item3);
	}

	[HttpPost]
	public DataSet ExecuteSqlStoredProcedure()
	{
		Tuple<string, List<QueryParamater>> tuple = JsonConvert.DeserializeObject<Tuple<string, List<QueryParamater>>>(Request.Content.ReadAsStringAsync().Result, new JsonConverter[1]
		{
			new ApzJsonConverterTuple()
		});
		return UnitOfWork.Commons.ExecuteSqlStoredProcedure(tuple.Item1, tuple.Item2);
	}

	[HttpPost]
	public DataSet ExecuteTemporyTable([FromBody] Tuple<string, DataTable, string> model)
	{
		string item = model.Item1;
		DataTable item2 = model.Item2;
		string item3 = model.Item3;
		item2.TableName = (string.IsNullOrEmpty(item3) ? "tbltemp" : item3);
		return UnitOfWork.Commons.ExecuteTemporyTable(item, item2);
	}

	[HttpGet]
	public DataSet GetDataCategories(string filter = "")
	{
		return UnitOfWork.Commons.GetDataCategories(SessionUserInfo.UserName, filter);
	}

	[HttpPost]
	public string SaveQuery([FromBody] Tuple<string, string, string> model)
	{
		return UnitOfWork.Commons.SaveQuery(model.Item1, model.Item2, model.Item3);
	}

	[HttpPost]
	public string SaveQueryCategory([FromBody] DataTable dt)
	{
		return UnitOfWork.Commons.SaveQueryCategory(dt);
	}

	[HttpGet]
	public DataTable GetItemOnwh(string item, string wh)
	{
		return UnitOfWork.Commons.GetItemOnwh(item, wh);
	}

	[HttpGet]
	public DataTable GetPriceItem(int priceList, string itemcode)
	{
		return UnitOfWork.Commons.GetPriceItem(priceList, itemcode);
	}

	[HttpGet]
	public DataTable LoadSeriesByPostingDate(string usercode, ApzObjectTypes apzObjectTypes, string docSubType, string postingDate)
	{
		return UnitOfWork.Commons.LoadSeriesByPostingDate(usercode, apzObjectTypes, docSubType, postingDate);
	}

	[HttpGet]
	public DataTable GetSeries(int ObjType)
	{
		return UnitOfWork.Commons.GetSeries(ObjType);
	}

	[HttpGet]
	public DataTable GetNextNumberSeries(int series)
	{
		return UnitOfWork.Commons.GetNextNumberSeries(series);
	}

	[HttpGet]
	public DataTable ShowSeries()
	{
		return UnitOfWork.Commons.ShowSeries();
	}

	[HttpGet]
	public DataTable GetCurrency()
	{
		return UnitOfWork.Commons.GetCurrency();
	}

	[HttpGet]
	public DataTable GetListCardCodeByTableName(string table, string bpType)
	{
		return UnitOfWork.Commons.GetListCardCodeByTableName(table, bpType);
	}

	[HttpGet]
	public DataTable GetListContacrPersonByAllCardCode()
	{
		return UnitOfWork.Commons.GetListContacrPersonByAllCardCode();
	}

	[HttpGet]
	public DataTable GetWareHouseByCode(string whsCode)
	{
		return UnitOfWork.Commons.GetWareHouseByCode(whsCode);
	}

	[HttpGet]
	public DataTable GetBinByWhsCode(string whsCode)
	{
		return UnitOfWork.Commons.GetBinByWhsCode(whsCode);
	}

	[HttpGet]
	public string GetWhsDefaultByUser(string UserName)
	{
		return UnitOfWork.Commons.GetWhsDefaultByUser(UserName);
	}

	[HttpGet]
	public DataTable GetBranchList(string userCode)
	{
		return UnitOfWork.Commons.GetBranchList(userCode);
	}

	[HttpGet]
	public DataTable GetIsMultiBranch()
	{
		return UnitOfWork.Commons.GetIsMultiBranch();
	}

	[HttpGet]
	public DataTable GetDefaultData(string userCode, int branch)
	{
		return UnitOfWork.Commons.GetDefaultData(userCode, branch);
	}

	[HttpGet]
	public DataTable GetListSeries(string o, int b, string docSubType)
	{
		return UnitOfWork.Commons.GetListSeries(o, b, docSubType);
	}

	[HttpGet]
	public DataTable GetCodeBySeries(string s, string o, int b)
	{
		return UnitOfWork.Commons.GetCodeBySeries(s, o, b);
	}

	[HttpPost]
	public string SaveOUGP(DataTable dtDoc)
	{
		return UnitOfWork.Commons.Save(dtDoc, "OUGP");
	}

	[HttpPost]
	public string SaveOUOM(DataTable dtDoc)
	{
		return UnitOfWork.Commons.Save(dtDoc, "OUOM");
	}

	[HttpPost]
	public string SaveOCTG(DataTable dtDoc)
	{
		return UnitOfWork.Commons.Save(dtDoc, "OCTG");
	}

	[HttpPost]
	public string SaveOBPP(DataTable dtDoc)
	{
		return UnitOfWork.Commons.Save(dtDoc, "OBPP");
	}

	[HttpPost]
	public string SaveOCQG(DataTable dtDoc)
	{
		return UnitOfWork.Commons.Save(dtDoc, "OCQG");
	}

	[HttpPost]
	public string SaveOCRC(DataTable dtDoc)
	{
		return UnitOfWork.Commons.Save(dtDoc, "OCRC");
	}

	[HttpPost]
	public string SaveOCRN(DataTable dtDoc)
	{
		return UnitOfWork.Commons.Save(dtDoc, "OCRN");
	}

	[HttpPost]
	public string SaveOUTB(DataTable dtDoc)
	{
		return UnitOfWork.Commons.Save(dtDoc, "OUTB");
	}

	[HttpGet]
	public DataSet DocumentGetComBoTabAccounting(string condition)
	{
		return UnitOfWork.Commons.DocumentGetComBoTabAccounting(condition);
	}

	[HttpGet]
	public DataTable DocumentLoadTabAccounting(int entry, ApzObjectTypes objType)
	{
		return UnitOfWork.Commons.DocumentLoadTabAccounting(entry, objType);
	}

	[HttpGet]
	public DataTable DocumentLoadTabAccountingColumns()
	{
		return UnitOfWork.Commons.DocumentLoadTabAccountingColumns();
	}

	[HttpGet]
	public DataTable ExistFileInDoc(int docEntry, ApzObjectTypes FormObjectType, string fileName, string fileExt)
	{
		return UnitOfWork.Commons.ExistFileInDoc(docEntry, FormObjectType, fileName, fileExt);
	}

	[HttpGet]
	public DataTable ExistFileInDocOpportunity(int oprId, string fileName)
	{
		return UnitOfWork.Commons.ExistFileInDocOpportunity(oprId, fileName);
	}

	[HttpGet]
	public DataTable GetOldBOMSalesQty(int docEntry, int lineNum, ApzObjectTypes objectType)
	{
		return UnitOfWork.Commons.GetOldBOMSalesQty(docEntry, lineNum, objectType);
	}

	[HttpPost]
	public DataTable DocumentPrice([FromBody] DataTable dt)
	{
		return UnitOfWork.Commons.DocumentPrice(dt);
	}

	[HttpGet]
	public DataTable GetDocLineUDFNameAndType()
	{
		return UnitOfWork.Commons.GetDocLineUDFNameAndType();
	}

	[HttpGet]
	public DataTable GetDocUDFNameAndType()
	{
		return UnitOfWork.Commons.GetDocUDFNameAndType();
	}

	[HttpGet]
	public DataTable GetListForm()
	{
		return UnitOfWork.CreateFormula.GetListForm();
	}

	[HttpGet]
	public DataTable GetSettingSap()
	{
		return UnitOfWork.Commons.GetSettingSap();
	}

	[HttpGet]
	public DataTable GetDataBrach(string userCode)
	{
		return UnitOfWork.Commons.GetDataBrach(userCode);
	}

	[HttpGet]
	public DataTable CheckExistValidValue(string queryID, string value)
	{
		return UnitOfWork.Commons.CheckExistValidValue(queryID, value);
	}

	public string GetItemNameByCode(string code)
	{
		return UnitOfWork.Commons.GetItemNameByCode(code);
	}

	[HttpGet]
	public string GetBPCurrency(string cardCode)
	{
		return UnitOfWork.Commons.GetBPCurrency(cardCode);
	}

	[HttpGet]
	public string GetNewDocNum(int objType, int series, int bplId, int usersign)
	{
		return UnitOfWork.Commons.GetNewDocNum(objType, series, bplId, usersign);
	}

	[HttpGet]
	public DataTable GetDataTaxCode()
	{
		return UnitOfWork.Commons.GetDataTaxCode();
	}

	[HttpGet]
	public DataTable GetDataItemByCode(string itemCode, int priceList, string userName, int branch)
	{
		return UnitOfWork.Commons.GetDataItemByCode(itemCode, priceList, userName, branch);
	}

	[HttpGet]
	public DataTable GetDataUomGroup()
	{
		return UnitOfWork.Commons.GetDataUomGroup();
	}

	[HttpGet]
	public string GetUoMCodeByName(int uomGroup, string uomName)
	{
		return UnitOfWork.Commons.GetUoMCodeByName(uomGroup, uomName);
	}

	[HttpGet]
	public DataTable GetDataUomByCode(int uomGroup, string uomCode)
	{
		return UnitOfWork.Commons.GetDataUomByCode(uomGroup, uomCode);
	}

	[HttpGet]
	public DataTable GetInventoryUoMByItemCode(string itemCode, Enums.ItemUomType type)
	{
		return UnitOfWork.Commons.GetInventoryUoMByItemCode(itemCode, type);
	}

	[HttpGet]
	public DataTable GetBaseUomByUomGroup(int uomGroup)
	{
		return UnitOfWork.Commons.GetBaseUomByUomGroup(uomGroup);
	}

	[HttpGet]
	public DataTable GetDataItemOnhand(string itemCode, string whsCode)
	{
		return UnitOfWork.Commons.GetDataItemOnhand(itemCode, whsCode);
	}

	[HttpGet]
	public DataTable GetDataBPByCode(string cardCode)
	{
		return UnitOfWork.Commons.GetDataBPByCode(cardCode);
	}

	[HttpGet]
	public int CheckConnectServer()
	{
		return 1;
	}

	[HttpGet]
	public DataTable CreateFreightGridDatasource(string vatType)
	{
		return UnitOfWork.Commons.CreateFreightGridDatasource(vatType);
	}

	[HttpGet]
	public DataTable GetDTGeneralSettings()
	{
		return UnitOfWork.Commons.GetDTGeneralSettings();
	}

	[HttpGet]
	public DataTable GetListUserReports(string userName, string branch)
	{
		try
		{
			return UnitOfWork.Commons.GetListUserReports(userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetStoreName(string code)
	{
		return UnitOfWork.Commons.GetStoreName(code);
	}

	[HttpGet]
	public DataTable GetParamStore(string store, string userName, string branch)
	{
		try
		{
			return UnitOfWork.Commons.GetParamStore(store, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	[HttpGet]
	public string GetFileNameReport(string code)
	{
		return UnitOfWork.Commons.GetFileNameReport(code);
	}

	[HttpPost]
	public DataSet ExecuteStore([FromBody] Tuple<string, List<Common.ParameterValue>> model)
	{
		try
		{
			return UnitOfWork.Commons.ExecuteStore(model.Item1, model.Item2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	[HttpGet]
	public DataTable CopyFrom(string type, string entry, string to, int bplId, string username)
	{
		return UnitOfWork.Documents.CopyFrom(type, entry, 15.ToString(), bplId, username);
	}

	[HttpGet]
	public DataTable CopyFromLine(string type, string entry, string to)
	{
		return UnitOfWork.Documents.CopyFromLine(type, entry, 15.ToString());
	}

	[HttpGet]
	public DataTable GetColumnSchema(string tableName)
	{
		return UnitOfWork.Commons.GetColumnSchema(tableName);
	}

	[HttpPost]
	public async Task<string> SaveDocument([FromBody] Tuple<DocumentsImpl, ApzObjectTypes> model)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			Apzon.DataServices.IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return (await unitOfWork.Documents.Save(model.Item1, SessionUserInfo.SessionDiServer, model.Item2)).Message;
			}
			return "INVALIDSESSION";
		}
		return "Function Not Support!";
	}

	[HttpPost]
	public HttpResult AddListDocument([FromBody] Tuple<List<DocumentsImpl>, ApzObjectTypes> model)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				Apzon.DataServices.IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return unitOfWork.Documents.AddListDocument(model.Item1, SessionUserInfo.SessionDiServer, model.Item2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	[HttpPost]
	public HttpResult UpdateListDocument([FromBody] List<DocumentsImpl> lstDoc)
	{
		try
		{
			if (Global.LoginSap)
			{
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				Apzon.DataServices.IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return unitOfWork.Documents.UpdateListDocument(lstDoc, SessionUserInfo.SessionDiServer);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult();
	}

	[HttpGet]
	public DataTable VirDocumentGetBPPaymentTerm(string cardCode)
	{
		return UnitOfWork.Commons.VirDocumentGetBPPaymentTerm(cardCode);
	}

	[HttpGet]
	public DataTable VirDocumentGetItemPriceByListNum(int listNum, string itemCode)
	{
		return UnitOfWork.Commons.VirDocumentGetItemPriceByListNum(listNum, itemCode);
	}

	[HttpGet]
	public DataTable GetPaymentTermByGroupNum(int groupNum)
	{
		return UnitOfWork.Commons.GetPaymentTermByGroupNum(groupNum);
	}

	[HttpGet]
	public DataTable ValidAttachmentEntry(int atcEntry)
	{
		return UnitOfWork.Commons.ValidAttachmentEntry(atcEntry);
	}

	[HttpGet]
	public string ResetLicenseService()
	{
		string command = Function.ToString(ConfigurationManager.AppSettings["ResetLicenseBatFile"]);
		return _uow.Common.ExecuteBatFile(command);
	}

	[HttpGet]
	public string ResetDIServer()
	{
		string command = Function.ToString(ConfigurationManager.AppSettings["ResetDIBatFile"]);
		return _uow.Common.ExecuteBatFile(command);
	}

	[HttpPost]
	public async Task<HttpResult> ProcessXmlRequest(Tuple<string> model)
	{
		try
		{
			if (Global.LoginSap)
			{
				string item = model.Item1;
				if (string.IsNullOrEmpty(SessionId))
				{
					return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
				}
				Apzon.DataServices.IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
				if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
				{
					return await unitOfWork.Common.ProcessXmlRequest(item, SessionUserInfo.SessionDiServer);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, Resources.Msg_UnknownError);
	}

	[HttpGet]
	public DataTable GetDataBatchAndSerial(int absEntry, string disNumber, string itemCode, string whsCode)
	{
		return UnitOfWork.Commons.GetDataBatchAndSerial(absEntry, disNumber, itemCode, whsCode);
	}

	[HttpGet]
	public DataTable ExistsTransaction()
	{
		return UnitOfWork.Commons.ExistsTransaction();
	}

	[HttpGet]
	public DataTable GetDataTypeObject(ApzObjectTypes objType)
	{
		return UnitOfWork.Commons.GetDataTypeObject(objType);
	}

	[HttpGet]
	public HttpResult CheckExistsByCodeObject(Enums.TypeCFL type, string code)
	{
		return UnitOfWork.Commons.CheckExistsByCodeObject(type, code);
	}

	[HttpGet]
	public string GetDocTypeFromDocEntryForDraft(int docEntry)
	{
		return UnitOfWork.Commons.GetDocTypeFromDocEntryForDraft(docEntry);
	}

	[HttpGet]
	public DataTable ValidateDocNumDraft(string docNum, int docType)
	{
		return UnitOfWork.Commons.ValidateDocNumDraft(docNum, docType);
	}

	[HttpGet]
	public DataTable ValidateDocNumByDocType(string docNum, ApzObjectTypes docType)
	{
		return UnitOfWork.Commons.ValidateDocNumByDocType(docNum, docType);
	}

	[HttpGet]
	public DataTable GetTableColumnSchema(string tableName)
	{
		return UnitOfWork.Commons.GetTableColumnSchema(tableName);
	}

	[HttpGet]
	public DataTable GetDefaultSeries(string obj, string docSubType, int userId)
	{
		return UnitOfWork.Commons.GetDefaultSeries(obj, docSubType, userId);
	}

	[HttpGet]
	public DataTable GetDataTableNewRow(string tableName)
	{
		return UnitOfWork.Commons.GetDataTableNewRow(tableName);
	}

	[HttpGet]
	public DataTable GetDocumentHeader(int docNum, int type)
	{
		return UnitOfWork.Commons.GetDocumentHeader(docNum, type);
	}

	[HttpGet]
	public bool CheckExistProject(string code)
	{
		return UnitOfWork.Commons.CheckExistProject(code);
	}

	[HttpGet]
	public DataTable GetDataOperationByCode(string opeCode)
	{
		return UnitOfWork.Commons.GetDataOperationByCode(opeCode);
	}

	[HttpGet]
	public DataTable GetPrintPreferences(ApzObjectTypes objType, string userName, int branch)
	{
		return UnitOfWork.Commons.GetPrintPreferences(objType, userName, branch);
	}

	[HttpPost]
	public HttpResult DoQuery(List<ContextActionDoQuery> doquery)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			Apzon.DataServices.IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return unitOfWork.Common.DoQuery(doquery, SessionUserInfo.SessionDiServer, SessionUserInfo.UserName);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpPost]
	public HttpResult DoQueryWithTempTable(Tuple<string, string, DataTable> data)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
			}
			Apzon.DataServices.IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				ContextActionDoQuery doquery = new ContextActionDoQuery
				{
					QueryText = data.Item1,
					TempTableName = data.Item2,
					TempTableData = data.Item3
				};
				return unitOfWork.Common.DoQueryWithTempTable(doquery, SessionUserInfo.SessionDiServer, SessionUserInfo.UserName);
			}
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		return new HttpResult(Enums.MessageCode.Error, "This function is not supported");
	}

	[HttpGet]
	public DataTable GetCompanyInfo()
	{
		return UnitOfWork.Commons.GetCompanyInfo();
	}

	[HttpGet]
	public Tuple<string, DataTable> GetDataByObject(ApzObjectTypes objType, string userName, string lngCode)
	{
		return UnitOfWork.Commons.GetDataByObject(objType, userName, lngCode);
	}

	[HttpGet]
	public HttpResult DMSGetDataByObject(ApzObjectTypes objType, string userName, string lngCode)
	{
		return UnitOfWork.Commons.DMSGetDataByObject(objType, userName, lngCode);
	}

	[HttpGet]
	public string CheckIsIns(int type, int docNum)
	{
		return UnitOfWork.Commons.CheckIsIns(type, docNum);
	}

	[HttpGet]
	public string CheckPosted(int type, int docEntry)
	{
		return UnitOfWork.Commons.CheckPosted(type, docEntry);
	}

	[HttpGet]
	public HttpResult CheckDocumentDuplicateBpRefNo(ApzObjectTypes objType, string bpCode, string refNo)
	{
		return UnitOfWork.Commons.CheckDocumentDuplicateBpRefNo(objType, bpCode, refNo);
	}

	[HttpGet]
	public string GetWhsDefault(string userName, int branch)
	{
		return UnitOfWork.Commons.GetWhsDefault(userName, branch);
	}

	[HttpGet]
	public DataTable GetListDomain()
	{
		return Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType).Commons.GetListDomain();
	}

	[HttpGet]
	public string GetSystemSuperUser()
	{
		return Global.SuperUser;
	}

	[HttpGet]
	public DataTable GetDataState(string countryCode)
	{
		return UnitOfWork.Commons.GetDataState(countryCode);
	}

	[HttpGet]
	public DataTable GetInfomationQuantityMenuDms(string userName)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return null;
			}
			return UnitOfWork.Commons.GetInfomationQuantityMenuDms(userName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetDataConfigDMS(string appName)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return null;
			}
			return UnitOfWork.Commons.GetDataConfigDMS(appName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetDataKPIDms(int userCode, string KPIType)
	{
		try
		{
			if (UnitOfWork == null)
			{
				return null;
			}
			return UnitOfWork.Commons.GetDataKPIDms(userCode, KPIType);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	[HttpGet]
	public DataTable GetTableStructureWithDefaultValue(string tableName)
	{
		try
		{
			return UnitOfWork.Commons.GetTableStructureWithDefaultValue(tableName);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string GetGoogleMap()
	{
		try
		{
			return Function.ToString(ConfigurationManager.AppSettings["GoogleMap"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return string.Empty;
	}

	[HttpGet]
	public DataTable GetEInvoiceToken(string usercode)
	{
		try
		{
			return UnitOfWork.Commons.GetEInvoiceToken(usercode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetAccessLog(string user, string fromDate, string toDate)
	{
		try
		{
			return Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType).Commons.GetAccessLog(SessionUserInfo.DatabaseName, user, fromDate, toDate);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public DataTable GetAccessLogUser()
	{
		try
		{
			return UnitOfWork.Commons.GetAccessLogUser();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	[HttpGet]
	public DataTable GetAccessLogLastLogin()
	{
		try
		{
			DataTable accessLogLastLogin = Apzon.Services.UnitOfWorkFactory.GetUnitOfWork(Global.CommonConnectionString, Global.DatabaseSystemType).Commons.GetAccessLogLastLogin(SessionUserInfo.DatabaseName);
			for (int i = 0; i < accessLogLastLogin.Rows.Count; i++)
			{
				string user = Function.ToString(accessLogLastLogin.Rows[i]["User"]);
				if (!Global.SessionUserInfo.Values.Any((SessionUserInfoImpl u) => u.UserName == user))
				{
					accessLogLastLogin.Rows[i]["status"] = "N";
					accessLogLastLogin.Rows[i]["IS_ONLINE"] = 0;
				}
			}
			accessLogLastLogin.AcceptChanges();
			return accessLogLastLogin;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	[HttpGet]
	public string CheckExistColumn(string table, string column)
	{
		return UnitOfWork.Commons.CheckExistColumn(table, column);
	}

	[HttpPost]
	public HttpResult ProcessSAPJson([FromBody] ProcessJsonImpl data)
	{
		if (data == null)
		{
			return new HttpResult(Enums.MessageCode.Error, "No data to process");
		}
		if (string.IsNullOrEmpty(SessionId))
		{
			return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
		}
		if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
		{
			bool flag = int.TryParse(Function.ToString(data.ObjType), out var result);
			ApzObjectTypes objectType = (flag ? ((ApzObjectTypes)result) : ApzObjectTypes.oNone);
			string udoCode = (flag ? "" : data.ObjType);
			Enums.HttpAction action = data.Action switch
			{
				"A" => Enums.HttpAction.AddNew, 
				"U" => Enums.HttpAction.Update, 
				"D" => Enums.HttpAction.Delete, 
				"C" => Enums.HttpAction.Cancel, 
				"L" => Enums.HttpAction.Close, 
				_ => Enums.HttpAction.Get, 
			};
			IDataService dataService = ((Global.ServiceType != Enums.ServiceType.DiServer) ? ((IDataService)new HanaService(Global.ServiceAddress)) : ((IDataService)new DiServices()));
			return dataService.ProcessJson(objectType, SessionUserInfo.SessionDiServer, data.Json, action, data.ObjKey, udoCode);
		}
		return new HttpResult(Enums.MessageCode.Error, "INVALIDSESSION");
	}
}
