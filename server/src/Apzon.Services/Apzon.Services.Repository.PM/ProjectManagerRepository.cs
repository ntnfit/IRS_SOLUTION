using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.PM;

namespace Apzon.Services.Repository.PM;

public class ProjectManagerRepository : BaseService<DataSet>, IProjectManager
{
	private const string docTemp = "#APZ_OPRG";

	private const string taskTemp = "#APZ_PRG1";

	private const string dependentTemp = "#APZ_PRG2";

	private const string resourcesTemp = "#APZ_PRG3";

	private const string itemTemp = "#APZ_PRG4";

	private const string documentTemp = "#APZ_PRG5";

	private const string subTemp = "#APZ_OPRG1";

	private const string paymentTemp = "#APZ_PRG6";

	public ProjectManagerRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable LoadData(int absEntry, string code, string status, string bpCode, string startFrom, string startTo, string userName, string filterString)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(startFrom, "yyyyMMdd");
			DateTime? dateTime2 = Function.ParseDateTime(startTo, "yyyyMMdd");
			string query = "APZ_ProjectManager_LoadData";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@absEntry", absEntry), new SqlParameter("@code", code), new SqlParameter("@bpCode", bpCode), new SqlParameter("@status", status), new SqlParameter("@startFrom", dateTime), new SqlParameter("@startTo", dateTime2), new SqlParameter("@userName", userName), new SqlParameter("@filterString", filterString));
			SetDefaultValueToTable("APZ_OPRG", dataTable);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetDataById(int Id)
	{
		try
		{
			DataSet dataSet = ExecuteData("APZ_ProjectManager_GetLineById", CommandType.StoredProcedure, new SqlParameter("@Id", Id));
			if (dataSet != null)
			{
				SetDefaultValueToTable("APZ_PRG1", dataSet.Tables[0]);
				SetDefaultValueToTable("APZ_PRG3", dataSet.Tables[1]);
				SetDefaultValueToTable("APZ_PRG4", dataSet.Tables[2]);
				SetDefaultValueToTable("APZ_PRG5", dataSet.Tables[3]);
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetListTask(int projectId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@projectEntry",
					Value = projectId
				}
			};
			return ExecuteDataTable("APZ_D_ProjectManager_GetDataTask", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetListTracking(int projectId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@projectEntry",
					Value = projectId
				}
			};
			return ExecuteDataTable("APZ_D_ProjectManager_ListTrackingById", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			switch (action)
			{
			case "A":
				transtype = Enums.TransactionType.A;
				break;
			case "U":
				transtype = Enums.TransactionType.U;
				break;
			case "D":
				transtype = Enums.TransactionType.D;
				break;
			}
			return Process(ApzObjectTypes.oProjectManager, -1, transtype, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet dsSave, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(dsSave, transtype, ref objId, ref objNum);
		case Enums.TransactionType.D:
			return DeleteData(ref ret, ref objId, ref objNum);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Could not execute function");
		}
	}

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "Data must not null!");
			}
			DataTable tableStruct = GetTableStruct("APZ_OPRG");
			DataTable dataTable = ds.Tables[0];
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, "#APZ_OPRG");
			BulkCopy(dataTable, "#APZ_OPRG");
			DataTable tableStruct2 = GetTableStruct("APZ_PRG1");
			DataTable dataTable2 = ds.Tables[1];
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			(from DataRow t in dataTable2.Rows
				where string.IsNullOrEmpty(Function.ToString(t["TaskCode"]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
			CreateTempTable(dataTable2, "#APZ_PRG1");
			BulkCopy(dataTable2, "#APZ_PRG1");
			DataTable tableStruct3 = GetTableStruct("APZ_PRG2");
			DataTable dataTable3 = ds.Tables[2];
			if (!tableStruct3.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source3 = from DataColumn t in tableStruct3.Columns
				select t.ColumnName;
			dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source3.ToArray());
			(from DataRow t in dataTable3.Rows
				where string.IsNullOrEmpty(Function.ToString(t["Dependent"]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
			CreateTempTable(dataTable3, "#APZ_PRG2");
			BulkCopy(dataTable3, "#APZ_PRG2");
			DataTable tableStruct4 = GetTableStruct("APZ_PRG3");
			DataTable dataTable4 = ds.Tables[3];
			if (!tableStruct4.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			if (!dataTable4.IsNotNull())
			{
				dataTable4 = tableStruct4;
			}
			IEnumerable<string> source4 = from DataColumn t in tableStruct4.Columns
				select t.ColumnName;
			dataTable4 = dataTable4.DefaultView.ToTable(distinct: false, source4.ToArray());
			(from DataRow t in dataTable4.Rows
				where string.IsNullOrEmpty(Function.ToString(t["Resource"]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
			CreateTempTable(dataTable4, "#APZ_PRG3");
			BulkCopy(dataTable4, "#APZ_PRG3");
			DataTable tableStruct5 = GetTableStruct("APZ_PRG4");
			DataTable dataTable5 = ds.Tables[4];
			if (!tableStruct5.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source5 = from DataColumn t in tableStruct5.Columns
				select t.ColumnName;
			dataTable5 = dataTable5.DefaultView.ToTable(distinct: false, source5.ToArray());
			(from DataRow t in dataTable5.Rows
				where string.IsNullOrEmpty(Function.ToString(t["ItemCode"])) && string.IsNullOrEmpty(Function.ToString(t["TaskCode"]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
			CreateTempTable(dataTable5, "#APZ_PRG4");
			BulkCopy(dataTable5, "#APZ_PRG4");
			DataTable tableStruct6 = GetTableStruct("APZ_PRG5");
			DataTable dataTable6 = ds.Tables[5];
			if (!tableStruct6.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source6 = from DataColumn t in tableStruct6.Columns
				select t.ColumnName;
			dataTable6 = dataTable6.DefaultView.ToTable(distinct: false, source6.ToArray());
			(from DataRow t in dataTable6.Rows
				where string.IsNullOrEmpty(Function.ToString(t["ObjType"]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
			CreateTempTable(dataTable6, "#APZ_PRG5");
			BulkCopy(dataTable6, "#APZ_PRG5");
			DataTable dataTable7 = ds.Tables[6];
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source7 = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable7 = dataTable7.DefaultView.ToTable(distinct: false, source7.ToArray());
			(from DataRow t in dataTable7.Rows
				where string.IsNullOrEmpty(Function.ToString(t["Code"]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
			CreateTempTable(dataTable7, "#APZ_OPRG1");
			BulkCopy(dataTable7, "#APZ_OPRG1");
			DataTable dataTable8 = ds.Tables[7];
			DataTable tableStruct7 = GetTableStruct("APZ_PRG6");
			if (!tableStruct7.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source8 = from DataColumn t in tableStruct7.Columns
				select t.ColumnName;
			dataTable8 = dataTable8.DefaultView.ToTable(distinct: false, source8.ToArray());
			(from DataRow t in dataTable8.Rows
				where string.IsNullOrEmpty(Function.ToString(t["TaskCode"])) && string.IsNullOrEmpty(Function.ToString(t["InvoicePlan"])) && string.IsNullOrEmpty(Function.ToString(t["IncomingPayment"]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
			CreateTempTable(dataTable8, "#APZ_PRG6");
			BulkCopy(dataTable8, "#APZ_PRG6");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable9 = ExecuteDataTable("APZ_ProjectManager_Save", CommandType.StoredProcedure, sqlParameter);
			if (dataTable9 != null && dataTable9.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable9.Rows[0][1])))
				{
					objId = Function.ToString(dataTable9.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable9.Rows[0][0])}-{Function.ToString(dataTable9.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, (tranType == Enums.TransactionType.A) ? "Data is not valid" : "Data is not valid");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string DeleteDocs(string Id)
	{
		try
		{
			return Process(ApzObjectTypes.oProjectManager, -1, Enums.TransactionType.D, new DataSet(), Id).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Id", Function.ParseInt(objId));
			DataTable dataTable = ExecuteDataTable("APZ_ProjectManager_Remove", CommandType.StoredProcedure, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
				{
					objId = Function.ToString(dataTable.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable LoadSubProjectByMasterId(int projectId)
	{
		try
		{
			string query = LoadSubProjectByMasterIdQuery();
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@projectId", projectId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string LoadSubProjectByMasterIdQuery()
	{
		return "select * from APZ_OPRG where \"MasterEntry\"=@projectId;";
	}

	private void AmountFather(DataTable dtItem, int fatherId = -1)
	{
		try
		{
			double num = (from t in dtItem.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == fatherId
				select t).ToList().Sum((DataRow t) => Function.ParseDouble(t["Amount"]));
			DataRow dataRow = dtItem.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == fatherId);
			if (dataRow != null)
			{
				dataRow["Amount"] = num;
				dtItem.AcceptChanges();
				AmountFather(dtItem, Function.ParseInt(dataRow["FatherNum"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void ActualAmountFather(DataTable dtItem, int fatherId = -1)
	{
		try
		{
			double num = (from t in dtItem.AsEnumerable()
				where Function.ParseInt(t["FatherNum"]) == fatherId
				select t).ToList().Sum((DataRow t) => Function.ParseDouble(t["ActualAmount"]));
			DataRow dataRow = dtItem.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == fatherId);
			if (dataRow != null)
			{
				dataRow["ActualAmount"] = num;
				dtItem.AcceptChanges();
				ActualAmountFather(dtItem, Function.ParseInt(dataRow["FatherNum"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void SumAmountHeader(DataTable dt, DataTable dtDoc)
	{
		try
		{
			double num = 0.0;
			double num2 = 0.0;
			if (dt == null || dt.Rows.Count <= 0)
			{
				return;
			}
			int i;
			for (i = 0; i < dtDoc.Rows.Count; i++)
			{
				List<DataRow> list = (from t in dt.AsEnumerable()
					where Function.ParseInt(t["FatherNum"]) == 0 && Function.ParseInt(t["AbsEntry"]) == Function.ParseInt(dtDoc.Rows[i]["AbsEntry"])
					select t).ToList();
				if (list != null && list.Count > 0)
				{
					int num3 = 0;
					while (num3 < list.Count)
					{
						Function.ParseDouble(list[i]["RefPrice"]);
						Function.ParseDouble(list[i]["ActualQty"]);
						num += Function.ParseDouble(list[i]["Amount"]);
						num2 += Function.ParseDouble(list[i]["ActualAmount"]);
						i++;
					}
					double num4 = num2 - num;
					dtDoc.Rows[i]["BudgetAmount"] = num;
					dtDoc.Rows[i]["ActualAmount"] = num2;
					dtDoc.Rows[i]["AmountDiff"] = num4;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public DataTable GetDataBOMSaleOrder(string code)
	{
		try
		{
			return ExecuteDataTable("APZ_ProjectManager_GetDataBOMSaleOrder", CommandType.StoredProcedure, new SqlParameter("@Code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataItemProduction(string code)
	{
		try
		{
			return ExecuteDataTable("APZ_ProjectReportCompletion_ItemProducton", CommandType.StoredProcedure, new SqlParameter("@Code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataItemInBom(string code)
	{
		try
		{
			return ExecuteDataTable("APZ_ProjectReportCompletion_ItemInBom", CommandType.StoredProcedure, new SqlParameter("@Code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataItemGroup()
	{
		try
		{
			string query = "SELECT * FROM OITB;";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult UpdateStatus(int docEntry, string status)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_ProjectManager_UpdateStatus", CommandType.StoredProcedure, new SqlParameter("@status", status), new SqlParameter("@docEntry", docEntry));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
				{
					return new HttpResult(Enums.MessageCode.Success);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, "Update error");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable FindNextLastFrist(int id, int type)
	{
		try
		{
			return ExecuteDataTable("APZ_ProjectManager_FindNextLastFrist", CommandType.StoredProcedure, new SqlParameter("@id", id), new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetDataLineByWbsTemplate(string code)
	{
		try
		{
			return ExecuteData("APZ_ProjectManager_GetDataLineByWbsTemplate", CommandType.StoredProcedure, new SqlParameter("@Code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetIdWorkScheduling(int year)
	{
		try
		{
			string query = "select top 1 * from APZ_OWSH where \"Year\"=@year;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@year", year));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataShift(string code)
	{
		try
		{
			if (!string.IsNullOrEmpty(code))
			{
				return ExecuteDataTable("APZ_ProjectManager_GetDataShift", CommandType.StoredProcedure, new SqlParameter("@code", code));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetTaskTemplateByCode(string code)
	{
		try
		{
			string query = "APZ_ProjectManager_GetTaskTtemplateByCode";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetTaskByWbsCode(string baseType, int baseEntry, int baseLine, string wbsCode)
	{
		try
		{
			string query = "APZ_ProjectManager_GetTaskByWbsCode";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@baseType", baseType), new SqlParameter("@baseEntry", baseEntry), new SqlParameter("@baseLine", baseLine), new SqlParameter("@wbsCode", wbsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string CheckItemInDocument(int Id, int lineNum = -1)
	{
		try
		{
			string query = "APZ_ProjectManager_CheckItem";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@Id", Id), new SqlParameter("@lineNum", lineNum));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
				{
					return "";
				}
				return Function.ToString(dataTable.Rows[0][1]);
			}
			return Function.ToString(dataTable.Rows[0][1]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetShifOrderByTime(string code)
	{
		try
		{
			string query = "APZ_ProjectManager_NextShift";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetBrachByEmpID(int empId)
	{
		try
		{
			string query = "select * from HEM10 where \"empID\"=@empID;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@empID", empId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string CheckMltpBrnchs()
	{
		try
		{
			string query = "SELECT \"MltpBrnchs\" FROM OADM;";
			return Function.ToString(ExecuteScalar(query, CommandType.Text));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "N";
	}

	public DataSet GetProjectByAbsEntryDMS(int absEntry)
	{
		try
		{
			string query = "APZ_ProjectManager_GetById";
			DataSet dataSet = ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@Id", absEntry));
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				DataTable dataTable = dataSet.Tables[0];
				if (dataTable.Rows.Count == 0)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["Code"] = "";
					dataTable.Rows.Add(dataRow);
				}
				DataTable dataTable2 = dataSet.Tables[1];
				if (dataTable2.Rows.Count == 0)
				{
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["LineNum"] = 1;
					dataTable2.Rows.Add(dataRow2);
				}
				DataTable dataTable3 = dataSet.Tables[2];
				if (dataTable3.Rows.Count == 0)
				{
					DataRow dataRow3 = dataTable3.NewRow();
					dataRow3["LineNum"] = 1;
					dataTable3.Rows.Add(dataRow3);
				}
				DataTable dataTable4 = dataSet.Tables[3];
				if (dataTable4.Rows.Count == 0)
				{
					DataRow dataRow4 = dataTable4.NewRow();
					dataRow4["LineNum"] = 1;
					dataTable4.Rows.Add(dataRow4);
				}
				DataTable dataTable5 = dataSet.Tables[4];
				if (dataTable5.Rows.Count == 0)
				{
					DataRow dataRow5 = dataTable5.NewRow();
					dataRow5["LineNum"] = 1;
					dataTable5.Rows.Add(dataRow5);
				}
				DataTable dataTable6 = dataSet.Tables[7];
				if (dataTable6.Rows.Count == 0)
				{
					DataRow dataRow6 = dataTable6.NewRow();
					dataRow6["LineNum"] = 1;
					dataTable6.Rows.Add(dataRow6);
				}
				DataTable dataTable7 = dataSet.Tables[5];
				if (dataTable7.Rows.Count == 0)
				{
					DataRow dataRow7 = dataTable7.NewRow();
					dataRow7["AbsEntry"] = 0;
					dataTable7.Rows.Add(dataRow7);
				}
				DataTable dataTable8 = dataSet.Tables[6];
				if (dataTable8.Rows.Count == 0)
				{
					DataRow dataRow8 = dataTable8.NewRow();
					dataRow8["AbsEntry"] = 0;
					dataTable8.Rows.Add(dataRow8);
				}
			}
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataSet();
	}

	public DataTable GetScheduleFromDate(string startDate)
	{
		string query = "APZ_ProjectManager_GetScheduleFromDate";
		return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@startDate", startDate));
	}

	public DataTable GetAllBaseLine(int absEntry)
	{
		try
		{
			string query = LoadAllBaseLine();
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@absEntry", absEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string LoadAllBaseLine()
	{
		return "select * from APZ_OPRG where \"BaseProject\"=@absEntry;";
	}

	public HttpResult SetApproval(string userName, NameID_Short model)
	{
		string query = "APZ_ProjectManager_SetApproval";
		ExecuteNonQuery(query, CommandType.StoredProcedure, new SqlParameter("@status", model.Name), new SqlParameter("@user_code", userName), new SqlParameter("@id", model.ID));
		return new HttpResult();
	}

	public DataTable GetTreeData(string itemCode)
	{
		try
		{
			return ExecuteDataTable("APZ_BOM_GetTreeData", CommandType.StoredProcedure, new SqlParameter("@productCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadDataDMS(int absEntry, string code, string status, string bpCode, string startFrom, string startTo, string userName, string filterString)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(startFrom, "yyyyMMdd");
			DateTime? dateTime2 = Function.ParseDateTime(startTo, "yyyyMMdd");
			string query = "APZ_D_ProjectManager_LoadData";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@absEntry", absEntry), new SqlParameter("@code", code), new SqlParameter("@bpCode", bpCode), new SqlParameter("@status", status), new SqlParameter("@startFrom", dateTime), new SqlParameter("@startTo", dateTime2), new SqlParameter("@userName", userName), new SqlParameter("@filterString", filterString));
			SetDefaultValueToTable("APZ_OPRG", dataTable);
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult UndoApproval(int absEntry, string user)
	{
		string query = "APZ_ProjectManager_UndoApproval";
		ExecuteNonQuery(query, CommandType.StoredProcedure, new SqlParameter("@abs_entry", absEntry), new SqlParameter("@user_code", user));
		return new HttpResult();
	}
}
