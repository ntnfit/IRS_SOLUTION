using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;

namespace Apzon.Api.Controllers.Commons;

[ValidateLoginSapExceptionHandling]
public class UDFController : BaseApiController
{
	[HttpGet]
	public List<UserFieldsImpl> GetListField(string tableName)
	{
		string tbl = SessionUserInfo.DatabaseName + "." + GetTableID(tableName);
		if (Global.ListUDF.ContainsKey(tbl))
		{
			lock (Global.ListUDF)
			{
				if (Global.ListUDF.ContainsKey(tbl))
				{
					return Global.ListUDF[tbl];
				}
			}
		}
		List<UserFieldsImpl> ret = UnitOfWork.UDFs.GetListField(tableName);
		if (ret != null)
		{
			Task.Run(() =>
			{
				if (!Global.ListUDF.ContainsKey(tbl))
				{
					lock (Global.ListUDF)
					{
						if (!Global.ListUDF.ContainsKey(tbl))
						{
							Global.ListUDF.Add(tbl, ret);
						}
					}
				}
			});
		}
		return ret;
	}

	private string GetTableID(string tableName)
	{
		switch (tableName)
		{
		case "OQUT":
		case "ORDR":
		case "ODLN":
		case "OINV":
		case "ORDN":
		case "ORIN":
		case "OPQT":
		case "OPRQ":
		case "ORRR":
		case "OPRR":
		case "OPOR":
		case "OPDN":
		case "OPCH":
		case "ORPD":
		case "ORPC":
		case "ODRF":
		case "OIGN":
		case "OIGE":
		case "OWTQ":
		case "OWTR":
			return "ADOC";
		case "QUT1":
		case "RDR1":
		case "DLN1":
		case "INV1":
		case "RDN1":
		case "RIN1":
		case "PQT1":
		case "PRQ1":
		case "RRR1":
		case "PRR1":
		case "POR1":
		case "PDN1":
		case "PCH1":
		case "RPD1":
		case "RPC1":
		case "DRF1":
		case "IGN1":
		case "IGE1":
		case "WTQ1":
		case "WTR1":
			return "ADO1";
		default:
			return tableName;
		}
	}

	[HttpGet]
	public DataTable GetListUDFFieldSQL(string tableName)
	{
		return UnitOfWork.UDFs.GetListUDFFieldSQL(tableName);
	}

	[HttpGet]
	public DataTable GetTableLinked()
	{
		return UnitOfWork.UDFs.GetTableLinked();
	}

	[HttpGet]
	public DataTable GetStructureUKD1()
	{
		return UnitOfWork.UDFs.GetStructureUKD1();
	}

	[HttpGet]
	public DataTable GetStructureUFD1()
	{
		return UnitOfWork.UDFs.GetStructureUFD1();
	}

	[HttpGet]
	public DataTable GetKeyFieldOUDO(string tblName)
	{
		return UnitOfWork.UDFs.GetKeyFieldOUDO(tblName);
	}

	[HttpGet]
	public DataTable GetObject()
	{
		return UnitOfWork.UDFs.GetObject();
	}

	[HttpGet]
	public DataTable GetUkd1(string tableName, int keyId)
	{
		return UnitOfWork.UDFs.GetUkd1(tableName, keyId);
	}

	[HttpGet]
	public DataTable GetUKD1ByTable(string tableName)
	{
		return UnitOfWork.UDFs.GetUKD1ByTable(tableName);
	}

	[HttpGet]
	public DataTable GetListKeyAlias(string tableName)
	{
		return UnitOfWork.UDFs.GetListKeyAlias(tableName);
	}

	[HttpGet]
	public DataTable GetOUKD(string tableName)
	{
		return UnitOfWork.UDFs.GetOUKD(tableName);
	}

	[HttpGet]
	public DataTable GetUfd1(string tableCode, int fieldID)
	{
		return UnitOfWork.UDFs.GetUfd1(tableCode, fieldID);
	}

	[HttpGet]
	public List<Common.Table_Code> GetDefaultValueUdf(string tableName)
	{
		return UnitOfWork.UDFs.GetDefaultValueUdf(tableName);
	}

	[HttpGet]
	public List<Common.Table_Code> GetValidValue(string tableId, string fieldId)
	{
		try
		{
			return UnitOfWork.UDFs.GetValidValue(tableId, fieldId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<Common.Table_Code>();
	}

	[HttpGet]
	public List<Common.Table_Code> GetLinkedTable(string linkedTable, string userName, int branch)
	{
		try
		{
			return UnitOfWork.UDFs.GetLinkedTable(linkedTable, userName, branch);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<Common.Table_Code>();
	}

	[HttpGet]
	public DataTable LoadDataOUTB()
	{
		return UnitOfWork.UDFs.LoadDataOUTB();
	}

	[HttpGet]
	public DataTable GetListUdf(string table)
	{
		return UnitOfWork.UDFs.GetListUdf(table);
	}

	[HttpPost]
	public string CheckExistUFD1([FromBody] Tuple<string, int, int> tup)
	{
		return UnitOfWork.UDFs.CheckExistUFD1(tup);
	}

	[HttpGet]
	public DataTable GetListCUFD(string tableName)
	{
		return UnitOfWork.UDFs.GetListCUFD(tableName);
	}

	[HttpGet]
	public DataTable CheckEnumConnect(string dbName)
	{
		return UnitOfWork.UDFs.CheckEnumConnect(dbName);
	}

	[HttpGet]
	public DataTable GetAddNewRow(string tableName)
	{
		return UnitOfWork.UDFs.GetAddNewRow(tableName);
	}

	[HttpPost]
	public string RemoveUdf([FromBody] Tuple<string, int, string> tupe)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			string text = ((!tupe.Item1.ToUpper().StartsWith("APZ_")) ? BrokerService.GetUnitOfWork(SessionId).UserFields.RemoveUdf(tupe.Item1, tupe.Item2, SessionUserInfo.SessionDiServer) : UnitOfWork.UDFs.RemoveUdf(tupe));
			if (string.IsNullOrEmpty(text))
			{
				Task.Run(() =>
				{
					if (Global.ListUDF.Count != 0)
					{
						lock (Global.ListUDF)
						{
							Global.ListUDF.Clear();
						}
					}
				});
			}
			return text;
		}
		return UnitOfWork.UDFs.RemoveUdf(tupe);
	}

	[HttpPost]
	public string RemoveUFD1([FromBody] Tuple<string, int, int> tupe)
	{
		string text = UnitOfWork.UDFs.RemoveUFD1(tupe);
		if (string.IsNullOrEmpty(text))
		{
			Task.Run(() =>
			{
				if (Global.ListUDF.Count != 0)
				{
					lock (Global.ListUDF)
					{
						Global.ListUDF.Clear();
					}
				}
			});
		}
		return text;
	}

	[HttpPost]
	public HttpResult Save(DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return new HttpResult(Enums.MessageCode.Error, "Invalid session");
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			HttpResult httpResult = ((!Function.ToString(ds.Tables[0].Rows[0]["TableID"]).ToUpper().StartsWith("APZ_")) ? unitOfWork.UserFields.Save(ds, SessionUserInfo.SessionDiServer) : UnitOfWork.UDFs.Save(ds));
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				Task.Run(() =>
				{
					if (Global.ListUDF.Count != 0)
					{
						lock (Global.ListUDF)
						{
							Global.ListUDF.Clear();
						}
					}
				});
			}
			return httpResult;
		}
		return UnitOfWork.UDFs.Save(ds);
	}

	[HttpPost]
	public HttpResult UpdateDataUdf([FromBody] Tuple<UserFieldsImpl, QueryParamater, string> model)
	{
		return UnitOfWork.UDFs.UpdateDataUdf(model.Item1, model.Item2, model.Item3);
	}

	[HttpPost]
	public HttpResult RefreshUDF()
	{
		try
		{
			if (Global.ListUDF.Count != 0)
			{
				lock (Global.ListUDF)
				{
					Global.ListUDF.Clear();
				}
			}
			if (DocumentsRepository.ListUdfHeaderNotValidated.ContainsKey(UnitOfWork.ConnectionString))
			{
				lock (DocumentsRepository.ListUdfHeaderNotValidated)
				{
					if (DocumentsRepository.ListUdfHeaderNotValidated.ContainsKey(UnitOfWork.ConnectionString))
					{
						DocumentsRepository.ListUdfHeaderNotValidated.Remove(UnitOfWork.ConnectionString);
					}
				}
			}
			if (DocumentsRepository.ListUdfLineNotValidated.ContainsKey(UnitOfWork.ConnectionString))
			{
				lock (DocumentsRepository.ListUdfLineNotValidated)
				{
					if (DocumentsRepository.ListUdfLineNotValidated.ContainsKey(UnitOfWork.ConnectionString))
					{
						DocumentsRepository.ListUdfLineNotValidated.Remove(UnitOfWork.ConnectionString);
					}
				}
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.Error);
		}
	}
}
