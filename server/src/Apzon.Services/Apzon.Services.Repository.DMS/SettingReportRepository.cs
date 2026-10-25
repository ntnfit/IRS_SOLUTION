using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MobileApp;
using Apzon.Services.Interfaces.DMS;
using Newtonsoft.Json;

namespace Apzon.Services.Repository.DMS;

public class SettingReportRepository : BaseService<DataSet>, ISettingReport
{
	private const string docTemp = "#APZ_D_OSRT";

	private const string fieldTemp = "#APZ_D_SRT2";

	private const string chartTemp = "#APZ_D_SRT1";

	private const string dashboardTemp = "#APZ_D_SRT3";

	public SettingReportRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable LoadData(int id, string fromDate, string toDate, string report)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(fromDate, "yyyyMMdd");
			DateTime? dateTime2 = Function.ParseDateTime(toDate, "yyyyMMdd");
			string query = "APZ_D_SettingReport_LoadData";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@id", id), new SqlParameter("@fromDate", dateTime), new SqlParameter("@toDate", dateTime2), new SqlParameter("@report", report));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetAllLineById(int Id)
	{
		try
		{
			return ExecuteDataTable("APZ_D_SettingReport_GetLineById", CommandType.StoredProcedure, new SqlParameter("@Id", Id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDataChartById(int Id)
	{
		try
		{
			return ExecuteDataTable("APZ_D_SettingReport_GetDataChartById", CommandType.StoredProcedure, new SqlParameter("@Id", Id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDataDashboardById(int docEntry, int id)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@id", id),
				new SqlParameter("@docEntry", docEntry)
			};
			return ExecuteDataTable("APZ_D_SettingReport_GetDataDashboardById", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetUserReportParams(int docEntry, string userCode)
	{
		DataTable result = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@docEntry",
					Value = docEntry
				},
				new SqlParameter
				{
					ParameterName = "@userCode",
					Value = userCode
				}
			};
			result = ExecuteDataTable("APZ_D_SettingReport_GetUserReportParams", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public ResponseResult SaveUserReportParams(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				CreateTempTable(dt.Copy(), "#APZ_D_SRT4");
				BulkCopy(dt.Copy(), "#APZ_D_SRT4");
				DataTable dataTable = ExecuteDataTable("APZ_D_SettingReport_SaveUserReportParams", CommandType.StoredProcedure);
				if (dataTable.IsNotNull())
				{
					dbTransaction.Commit();
					string text = Function.ToString(dataTable.Rows[0]["Message"]);
					if (string.IsNullOrEmpty(text))
					{
						return new ResponseResult
						{
							Message = "",
							MessageCode = "200"
						};
					}
					return new ResponseResult
					{
						Message = text,
						MessageCode = "406"
					};
				}
			}
			return new ResponseResult
			{
				Message = "Notfound",
				MessageCode = "404"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "405"
			};
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
			return Process(ApzObjectTypes.oSettingReport, -1, transtype, ds);
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
			DataTable tableStruct = GetTableStruct("APZ_D_OSRT");
			DataTable dataTable = ds.Tables[0];
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, "#APZ_D_OSRT");
			BulkCopy(dataTable, "#APZ_D_OSRT");
			DataTable tableStruct2 = GetTableStruct("APZ_D_SRT2");
			DataTable dataTable2 = ds.Tables[1];
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dataTable2, "#APZ_D_SRT2");
			BulkCopy(dataTable2, "#APZ_D_SRT2");
			DataTable tableStruct3 = GetTableStruct("APZ_D_SRT1");
			DataTable dataTable3 = ds.Tables[2];
			if (!tableStruct3.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source3 = from DataColumn t in tableStruct3.Columns
				select t.ColumnName;
			dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source3.ToArray());
			CreateTempTable(dataTable3, "#APZ_D_SRT1");
			BulkCopy(dataTable3, "#APZ_D_SRT1");
			DataTable tableStruct4 = GetTableStruct("APZ_D_SRT3");
			DataTable dataTable4 = ds.Tables[3];
			if (!tableStruct4.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source4 = from DataColumn t in tableStruct4.Columns
				select t.ColumnName;
			dataTable4 = dataTable4.DefaultView.ToTable(distinct: false, source4.ToArray());
			CreateTempTable(dataTable4, "#APZ_D_SRT3");
			BulkCopy(dataTable4, "#APZ_D_SRT3");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable5 = ExecuteDataTable("APZ_D_SettingReport_Save", CommandType.StoredProcedure, sqlParameter);
			if (dataTable5 != null && dataTable5.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable5.Rows[0][1])))
				{
					objId = Function.ToString(dataTable5.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable5.Rows[0][0])}-{Function.ToString(dataTable5.Rows[0][1])}");
			}
			return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
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
			return Process(ApzObjectTypes.oSettingReport, -1, Enums.TransactionType.D, new DataSet(), Id).Message;
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
			DataTable dataTable = ExecuteDataTable("APZ_D_SettingReport_Remove", CommandType.StoredProcedure, sqlParameter);
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

	protected virtual string GetNoQuery()
	{
		return "select isnull((select MAX(DocEntry) from APZ_D_OSRT), 0) + 1";
	}

	public int GetNo()
	{
		try
		{
			string noQuery = GetNoQuery();
			DataTable dataTable = ExecuteDataTable(noQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public DataTable FindNextLastFrist(int id, int type)
	{
		try
		{
			return ExecuteDataTable("APZ_D_SettingReport_FindNextLastFrist", CommandType.StoredProcedure, new SqlParameter("@id", id), new SqlParameter("@type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetAllData(string userCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_D_SettingReport_GetAll";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@userCode",
					Value = userCode
				}
			};
			return result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable DataSourceReportByStore(DataTable dtValuePar)
	{
		DataTable result = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "";
			if (dtValuePar != null && dtValuePar.Rows.Count > 0)
			{
				query = Function.ToString(dtValuePar.Rows[0]["Store"]);
				for (int i = 0; i < dtValuePar.Rows.Count; i++)
				{
					if (!string.IsNullOrEmpty(Function.ToString(dtValuePar.Rows[i]["ParamName"])))
					{
						string text = Function.ToString(dtValuePar.Rows[i]["TYPE"]).ToLower();
						if (text == "datetime" || text == "timestamp")
						{
							list.Add(new SqlParameter(Function.ToString(dtValuePar.Rows[i]["ParamName"]), Function.ParseDateTimes(dtValuePar.Rows[i]["value"])));
						}
						else
						{
							list.Add(new SqlParameter(Function.ToString(dtValuePar.Rows[i]["ParamName"]), Function.ToString(dtValuePar.Rows[i]["value"])));
						}
					}
				}
			}
			if (list.Count > 0)
			{
				return result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			}
			return result = ExecuteDataTable(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return result;
		}
	}

	public DataSet GetParameterByCode(string code)
	{
		DataSet result = new DataSet();
		try
		{
			string query = "APZ_D_SettingReport_GetDetailByCode";
			return result = ExecuteData(query, CommandType.StoredProcedure, new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable DistinctByRowId(DataTable dtDoc)
	{
		try
		{
			List<DataRow> list = new List<DataRow>();
			int i;
			for (i = 0; i < dtDoc.Rows.Count; i++)
			{
				if (list.Count == 0)
				{
					list.Add(dtDoc.Rows[i]);
				}
				else if (!list.Any((DataRow x) => Function.ToString(x["RowId"]) == Function.ToString(dtDoc.Rows[i]["RowId"])))
				{
					list.Add(dtDoc.Rows[i]);
				}
			}
			return list.CopyToDataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string MappingJson(int fatherId, DataTable dtSource)
	{
		DataTable source = dtSource.Copy();
		string text = "";
		EnumerableRowCollection<DataRow> source2 = from x in source.AsEnumerable()
			where Function.ToString(x["FatherNum"]) == Function.ToString(fatherId)
			select x;
		DataTable dataTable = new DataTable();
		if (source2.Count() > 0)
		{
			dataTable = source2.CopyToDataTable();
		}
		for (int num = 0; num < dataTable.Rows.Count; num++)
		{
			EnumerableRowCollection<DataRow> source3 = from x in source.AsEnumerable()
				where Function.ToString(x["Id"]) == Function.ToString(fatherId)
				select x;
			DataTable dataTable2 = new DataTable();
			if (source3.Count() > 0)
			{
				dataTable2 = source3.CopyToDataTable();
			}
			string text2 = "";
			if (dataTable2.IsNotNull())
			{
				text2 = Function.ToString(dataTable2.Rows[0]["Type"]);
			}
			string text3 = Function.ToString(dataTable.Rows[num]["Type"]);
			if (text2 == "rowArr" && text3 != "itemArr")
			{
				text += "{\"itemArr\":[";
			}
			text += "{";
			if (text3 == "itemArr" || text3 == "rowArr")
			{
				if (text3 == "rowArr")
				{
					if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["size"])))
					{
						text = text + "\"size\":" + Function.ToString(Function.ParseFloat(dataTable.Rows[num]["size"])).Replace(",", ".");
						text += ",";
					}
					else
					{
						text = text + "\"size\":" + 1;
						text += ",";
					}
				}
				text = text + "\"" + text3 + "\":[";
				text += MappingJson(Function.ParseInt(dataTable.Rows[num]["Id"]), dtSource.Copy());
				text += "]";
				string text4 = Function.ToString(dataTable.Rows[num]["paddingBottom"]);
				string text5 = Function.ToString(dataTable.Rows[num]["paddingLeft"]);
				string text6 = Function.ToString(dataTable.Rows[num]["paddingTop"]);
				string text7 = Function.ToString(dataTable.Rows[num]["paddingRight"]);
				string value = Function.ToString(dataTable.Rows[num]["justifyContent"]);
				string value2 = Function.ToString(dataTable.Rows[num]["alignItems"]);
				string value3 = Function.ToString(dataTable.Rows[num]["alignSelf"]);
				if (!string.IsNullOrEmpty(text4) || !string.IsNullOrEmpty(text7) || !string.IsNullOrEmpty(text6) || !string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value2) || !string.IsNullOrEmpty(text5) || !string.IsNullOrEmpty(value3))
				{
					text += ",";
					text = ((!(text3 == "itemArr")) ? (text + "\"itemStyle\":") : (text + "\"rowStyle\":"));
					text += "{";
					if (!string.IsNullOrEmpty(text4))
					{
						text = text + "\"paddingBottom\":" + Function.ParseInt(text4);
						text += ",";
					}
					if (!string.IsNullOrEmpty(text5))
					{
						text = text + "\"paddingLeft\":" + Function.ParseInt(text5);
						text += ",";
					}
					if (!string.IsNullOrEmpty(text6))
					{
						text = text + "\"paddingTop\":" + Function.ParseInt(text6);
						text += ",";
					}
					if (!string.IsNullOrEmpty(text7))
					{
						text = text + "\"paddingRight\":" + Function.ParseInt(text7);
						text += ",";
					}
					if (!string.IsNullOrEmpty(value))
					{
						text += Function.GetJsonString("justifyContent", value);
					}
					if (!string.IsNullOrEmpty(value2))
					{
						text += Function.GetJsonString("alignItems", value2);
					}
					if (!string.IsNullOrEmpty(value3))
					{
						text += Function.GetJsonString("alignSelf", value3);
					}
					if (text.EndsWith(","))
					{
						text = text.Substring(0, text.Length - 1);
					}
					text += "}";
				}
			}
			else
			{
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["fieldName"])))
				{
					text += Function.GetJsonString("fieldName", dataTable.Rows[num]["fieldName"]);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["size"])))
				{
					text = text + "\"size\":" + Function.ToString(Function.ParseFloat(dataTable.Rows[num]["size"])).Replace(",", ".");
					text += ",";
				}
				else
				{
					text = text + "\"size\":" + 1;
					text += ",";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["currencyField"])))
				{
					text += Function.GetJsonString("currencyField", dataTable.Rows[num]["currencyField"]);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["DateTimeValueFormat"])))
				{
					text += Function.GetJsonString("dateValueFormat", dataTable.Rows[num]["DateTimeValueFormat"]);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["DateTimeDisplayFormat"])))
				{
					text += Function.GetJsonString("dateDisplayFormat", dataTable.Rows[num]["DateTimeDisplayFormat"]);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["textBefore"])))
				{
					text += string.Format("\"{0}\":{1},", "textBefore", JsonConvert.ToString(dataTable.Rows[num]["textBefore"]));
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["ItemType"])))
				{
					text += "\"editor\":";
					text += JsonConvert.ToString(Function.ToString(dataTable.Rows[num]["ItemType"]));
					text += ",";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["textAfter"])))
				{
					text += string.Format("\"{0}\":{1},", "textAfter", JsonConvert.ToString(dataTable.Rows[num]["textAfter"]));
				}
				text += "\"editorStyle\":{";
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["fontSize"])))
				{
					text = text + "\"fontSize\":" + Function.ParseInt(dataTable.Rows[num]["fontSize"]);
					text += ",";
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["fontWeight"])))
				{
					text += Function.GetJsonString("fontWeight", dataTable.Rows[num]["fontWeight"]);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["color"])))
				{
					Color color = ColorTranslator.FromHtml(Function.ToString(dataTable.Rows[num]["color"]));
					Color color2 = Color.FromArgb(color.R, color.G, color.B);
					string text8 = color2.R.ToString("X2") + color2.G.ToString("X2") + color2.B.ToString("X2");
					text += Function.GetJsonString("color", "#" + text8);
				}
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[num]["textAlign"])))
				{
					text += Function.GetJsonString("textAlign", dataTable.Rows[num]["textAlign"]);
				}
				if (text.EndsWith(","))
				{
					text = text.Substring(0, text.Length - 1);
				}
				text += "}";
				string text9 = Function.ToString(dataTable.Rows[num]["paddingBottom"]);
				string text10 = Function.ToString(dataTable.Rows[num]["paddingLeft"]);
				string text11 = Function.ToString(dataTable.Rows[num]["paddingTop"]);
				string text12 = Function.ToString(dataTable.Rows[num]["paddingRight"]);
				string value4 = Function.ToString(dataTable.Rows[num]["justifyContent"]);
				string value5 = Function.ToString(dataTable.Rows[num]["alignItems"]);
				string value6 = Function.ToString(dataTable.Rows[num]["alignSelf"]);
				if (!string.IsNullOrEmpty(text9) || !string.IsNullOrEmpty(text12) || !string.IsNullOrEmpty(text11) || !string.IsNullOrEmpty(value4) || !string.IsNullOrEmpty(value5) || !string.IsNullOrEmpty(text10))
				{
					text += ",";
					text += "\"itemStyle\":{";
					if (!string.IsNullOrEmpty(text9))
					{
						text = text + "\"paddingBottom\":" + Function.ParseInt(text9);
						text += ",";
					}
					if (!string.IsNullOrEmpty(text10))
					{
						text = text + "\"paddingLeft\":" + Function.ParseInt(text10);
						text += ",";
					}
					if (!string.IsNullOrEmpty(text11))
					{
						text = text + "\"paddingTop\":" + Function.ParseInt(text11);
						text += ",";
					}
					if (!string.IsNullOrEmpty(text12))
					{
						text = text + "\"paddingRight\":" + Function.ParseInt(text12);
						text += ",";
					}
					if (!string.IsNullOrEmpty(value4))
					{
						text += Function.GetJsonString("justifyContent", value4);
					}
					if (!string.IsNullOrEmpty(value5))
					{
						text += Function.GetJsonString("alignItems", value5);
					}
					if (!string.IsNullOrEmpty(value6))
					{
						text += Function.GetJsonString("alignSelf", value6);
					}
					if (text.EndsWith(","))
					{
						text = text.Substring(0, text.Length - 1);
					}
					text += "}";
				}
			}
			text += "}";
			if (num != dataTable.Rows.Count - 1 && text2 != "rowArr")
			{
				text += ",";
			}
			if (text2 == "rowArr")
			{
				if (text3 != "itemArr")
				{
					text += "]}";
				}
				if (num != dataTable.Rows.Count - 1)
				{
					text += ",";
				}
			}
		}
		return text;
	}

	public ResponseResult GetDataById(int Id)
	{
		try
		{
			string item = "";
			DataTable dataTable = ExecuteDataTable("APZ_D_SettingReport_GetLineById", CommandType.StoredProcedure, new SqlParameter("@Id", Id));
			DataTable dataTable2 = new DataTable();
			if (dataTable.IsNotNull())
			{
				dataTable2 = dataTable;
				item = "[" + MappingJson(-1, dataTable2) + "]";
			}
			if (dataTable2.IsNotNull())
			{
				for (int i = 0; i < dataTable2.Rows.Count; i++)
				{
					if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[i]["color"])))
					{
						Color color = ColorTranslator.FromHtml(Function.ToString(dataTable2.Rows[i]["color"]));
						Color color2 = Color.FromArgb(color.R, color.G, color.B);
						string text = color2.R.ToString("X2") + color2.G.ToString("X2") + color2.B.ToString("X2");
						dataTable2.Rows[i]["color"] = "#" + text;
					}
				}
			}
			DataTable dataChartById = GetDataChartById(Id);
			if (dataChartById.IsNotNull())
			{
				for (int j = 0; j < dataChartById.Rows.Count; j++)
				{
					if (!string.IsNullOrEmpty(Function.ToString(dataChartById.Rows[j]["Color"])))
					{
						Color color3 = ColorTranslator.FromHtml(Function.ToString(dataChartById.Rows[j]["Color"]));
						Color color4 = Color.FromArgb(color3.R, color3.G, color3.B);
						string text2 = color4.R.ToString("X2") + color4.G.ToString("X2") + color4.B.ToString("X2");
						dataChartById.Rows[j]["color"] = "#" + text2;
					}
				}
			}
			return new ResponseResult
			{
				Message = "",
				MessageCode = "200",
				Content = Tuple.Create(item, dataTable2, dataChartById)
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "406"
			};
		}
	}

	public DataTable DataRowByRowId(int docEntry, int rowId, int fatherNum = -1)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_D_SettingReport_GetRowByRowId";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@docEntry", docEntry));
			list.Add(new SqlParameter("@rowId", rowId));
			list.Add(new SqlParameter("@fatherNum", fatherNum));
			return result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return result;
		}
	}

	private string Mapping(DataTable data, DataTable dtLines, int fatherNum = -1)
	{
		try
		{
			string text = "[";
			DataTable dataTable = DistinctByRowId(data);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					int docEntry = Function.ParseInt(dataTable.Rows[i]["DocEntry"]);
					int rowId = Function.ParseInt(dataTable.Rows[i]["RowId"]);
					text += "{";
					text += "\"itemArr\":[";
					DataTable dataTable2 = DataRowByRowId(docEntry, rowId, fatherNum);
					if (dataTable2 != null && dataTable2.Rows.Count > 0)
					{
						for (int j = 0; j < dataTable2.Rows.Count; j++)
						{
							int fatherId = Function.ParseInt(dataTable2.Rows[j]["LineNum"]);
							text += "{";
							if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[j]["fieldName"])))
							{
								text += Function.GetJsonString("fieldName", dataTable2.Rows[j]["fieldName"]);
							}
							if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[j]["size"])))
							{
								text = text + "\"size\":" + Function.ParseFloat(dataTable2.Rows[j]["size"]);
								text += ",";
							}
							else
							{
								text = text + "\"size\":" + 1;
								text += ",";
							}
							if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[j]["textBefore"])))
							{
								text += Function.GetJsonString("textBefore", dataTable2.Rows[j]["textBefore"]);
							}
							if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[j]["ItemType"])))
							{
								text += "\"editor\":";
								text += JsonConvert.ToString(Function.ToString(dataTable2.Rows[j]["ItemType"]));
								text += ",";
							}
							if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[j]["textAfter"])))
							{
								text += Function.GetJsonString("textAfter", dataTable2.Rows[j]["textAfter"]);
							}
							text += "\"editorStyle\":{";
							if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[j]["fontSize"])))
							{
								text = text + "\"fontSize\":" + Function.ParseInt(dataTable2.Rows[j]["fontSize"]);
								text += ",";
							}
							if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[j]["fontWeight"])))
							{
								text += Function.GetJsonString("fontWeight", dataTable2.Rows[j]["fontWeight"]);
							}
							if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[j]["color"])))
							{
								Color color = ColorTranslator.FromHtml(Function.ToString(dataTable2.Rows[j]["color"]));
								Color color2 = Color.FromArgb(color.R, color.G, color.B);
								string text2 = color2.R.ToString("X2") + color2.G.ToString("X2") + color2.B.ToString("X2");
								text += Function.GetJsonString("color", "#" + text2);
							}
							if (!string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[j]["textAlign"])))
							{
								text += Function.GetJsonString("textAlign", dataTable2.Rows[j]["textAlign"]);
							}
							if (text.EndsWith(","))
							{
								text = text.Substring(0, text.Length - 1);
							}
							text += "}";
							EnumerableRowCollection<DataRow> source = from t in dtLines.AsEnumerable()
								where Function.ParseInt(t["FatherNum"]) == fatherId
								select t;
							if (source.Count() > 0)
							{
								DataTable dataTable3 = source.CopyToDataTable();
								if (dataTable3 != null && dataTable3.Rows.Count > 0)
								{
									text += ",";
									text += "\"rowArr\":";
									Mapping(dataTable3, dtLines, fatherId);
									text += Mapping(dataTable3, dtLines, fatherId);
								}
							}
							string value = Function.ToString(dataTable2.Rows[j]["paddingBottom"]);
							string value2 = Function.ToString(dataTable2.Rows[j]["paddingLeft"]);
							string value3 = Function.ToString(dataTable2.Rows[j]["paddingTop"]);
							string value4 = Function.ToString(dataTable2.Rows[j]["paddingRight"]);
							string value5 = Function.ToString(dataTable2.Rows[j]["justifyContent"]);
							string value6 = Function.ToString(dataTable2.Rows[j]["alignItems"]);
							string value7 = Function.ToString(dataTable2.Rows[j]["alignSelf"]);
							if (!string.IsNullOrEmpty(value) || !string.IsNullOrEmpty(value4) || !string.IsNullOrEmpty(value3) || !string.IsNullOrEmpty(value5) || !string.IsNullOrEmpty(value6) || !string.IsNullOrEmpty(value2))
							{
								text += ",";
								text += "\"itemStyle\":{";
								if (!string.IsNullOrEmpty(value))
								{
									text = text + "\"paddingBottom\":" + Function.ParseInt(dataTable2.Rows[j]["paddingBottom"]);
									text += ",";
								}
								if (!string.IsNullOrEmpty(value2))
								{
									text = text + "\"paddingLeft\":" + Function.ParseInt(dataTable2.Rows[j]["paddingLeft"]);
									text += ",";
								}
								if (!string.IsNullOrEmpty(value3))
								{
									text = text + "\"paddingTop\":" + Function.ParseInt(dataTable2.Rows[j]["paddingTop"]);
									text += ",";
								}
								if (!string.IsNullOrEmpty(value4))
								{
									text = text + "\"paddingRight\":" + Function.ParseInt(dataTable2.Rows[j]["paddingRight"]);
									text += ",";
								}
								if (!string.IsNullOrEmpty(value5))
								{
									text += Function.GetJsonString("justifyContent", dataTable2.Rows[j]["justifyContent"]);
								}
								if (!string.IsNullOrEmpty(value6))
								{
									text += Function.GetJsonString("alignItems", dataTable2.Rows[j]["alignItems"]);
								}
								if (!string.IsNullOrEmpty(value7))
								{
									text += Function.GetJsonString("alignSelf", dataTable2.Rows[j]["alignSelf"]);
								}
								if (text.EndsWith(","))
								{
									text = text.Substring(0, text.Length - 1);
								}
								text += "}";
							}
							if (text.EndsWith(","))
							{
								text = text.Substring(0, text.Length - 1);
							}
							text += "}";
							text += ",";
						}
						if (text.EndsWith(","))
						{
							text = text.Substring(0, text.Length - 1);
						}
					}
					text += "]";
					string value8 = Function.ToString(dataTable.Rows[i]["paddingBottom"]);
					string value9 = Function.ToString(dataTable.Rows[i]["paddingLeft"]);
					string value10 = Function.ToString(dataTable.Rows[i]["paddingTop"]);
					string value11 = Function.ToString(dataTable.Rows[i]["paddingRight"]);
					string value12 = Function.ToString(dataTable.Rows[i]["justifyContent"]);
					string value13 = Function.ToString(dataTable.Rows[i]["alignItems"]);
					string value14 = Function.ToString(dataTable.Rows[i]["fontSize"]);
					if (!string.IsNullOrEmpty(value8) || !string.IsNullOrEmpty(value11) || !string.IsNullOrEmpty(value10) || !string.IsNullOrEmpty(value12) || !string.IsNullOrEmpty(value13) || !string.IsNullOrEmpty(value9))
					{
						text += ",";
						text += "\"rowStyle\":{";
						if (!string.IsNullOrEmpty(value14))
						{
							text = text + "\"fontSize\":" + Function.ParseInt(dataTable.Rows[i]["fontSize"]);
							text += ",";
						}
						if (!string.IsNullOrEmpty(value8))
						{
							text = text + "\"paddingBottom\":" + Function.ParseInt(dataTable.Rows[i]["paddingBottom"]);
							text += ",";
						}
						if (!string.IsNullOrEmpty(value9))
						{
							text = text + "\"paddingLeft\":" + Function.ParseInt(dataTable.Rows[i]["paddingLeft"]);
							text += ",";
						}
						if (!string.IsNullOrEmpty(value10))
						{
							text = text + "\"paddingTop\":" + Function.ParseInt(dataTable.Rows[i]["paddingTop"]);
							text += ",";
						}
						if (!string.IsNullOrEmpty(value11))
						{
							text = text + "\"paddingRight\":" + Function.ParseInt(dataTable.Rows[i]["paddingRight"]);
							text += ",";
						}
						if (!string.IsNullOrEmpty(value12))
						{
							text += Function.GetJsonString("justifyContent", dataTable.Rows[i]["justifyContent"]);
						}
						if (!string.IsNullOrEmpty(value13))
						{
							text += Function.GetJsonString("alignItems", dataTable.Rows[i]["alignItems"]);
						}
						if (text.EndsWith(","))
						{
							text = text.Substring(0, text.Length - 1);
						}
						text += "}";
					}
					text += "},";
				}
			}
			if (text.EndsWith(","))
			{
				text = text.Substring(0, text.Length - 1);
			}
			return text + "]";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public DataTable DataSourceCFL(string parName, string valuepar, string storeCFL)
	{
		DataTable result = new DataTable();
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter(parName, valuepar));
			if (string.IsNullOrEmpty(parName))
			{
				return result = ExecuteDataTable(storeCFL, CommandType.StoredProcedure);
			}
			return result = ExecuteDataTable(storeCFL, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable GetDefaultDataSourceReport(string code)
	{
		DataTable result = new DataTable();
		try
		{
			if (!string.IsNullOrEmpty(code))
			{
				DataTable dataTable = DataReportByCode(code);
				DataSet parameterByCode = GetParameterByCode(code);
				if (parameterByCode != null && parameterByCode.Tables.Count > 0)
				{
					DataTable dataTable2 = parameterByCode.Tables[0];
					if (dataTable2 != null)
					{
						dataTable2.Columns.Add("value", typeof(string));
						dataTable2.Columns.Add("Store", typeof(string));
						string value = "";
						if (dataTable != null && dataTable.Rows.Count > 0)
						{
							value = Function.ToString(dataTable.Rows[0]["StoreName"]);
						}
						if (dataTable2.Rows.Count > 0)
						{
							for (int i = 0; i < dataTable2.Rows.Count; i++)
							{
								string text = Function.ToString(dataTable2.Rows[i]["Type"]);
								Function.ToString(dataTable2.Rows[i]["ParamName"]);
								string text2 = Function.ToString(dataTable2.Rows[i]["DefaultValue"]);
								dataTable2.Rows[i]["Store"] = value;
								string text3 = "";
								if (string.IsNullOrEmpty(text2))
								{
									string text4 = text.ToLower();
									text3 = ((!(text4 == "int") && !(text4 == "integer")) ? "" : "0");
								}
								else
								{
									text3 = Function.ToString(text2).ToLower() switch
									{
										"@userid" => "-1", 
										"@usercode" => "", 
										"@branch" => "-1", 
										"@firstdaymonth" => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).ToString("yyyy-MM-dd"), 
										"@lastdaymonth" => new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1).AddMonths(1).AddDays(-1.0).ToString("yyyy-MM-dd"), 
										_ => Function.ToString(text2), 
									};
								}
								dataTable2.Rows[i]["value"] = text3;
							}
						}
						else
						{
							DataRow dataRow = dataTable2.NewRow();
							dataRow["value"] = "";
							dataRow["Store"] = value;
							dataTable2.Rows.Add(dataRow);
						}
						result = DataSourceReportByStore(dataTable2);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
		}
		return result;
	}

	public DataTable GetColumnStoreReport(string code)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Code", typeof(string));
		dataTable.Columns.Add("Name", typeof(string));
		try
		{
			if (!string.IsNullOrEmpty(code))
			{
				DataTable defaultDataSourceReport = GetDefaultDataSourceReport(code);
				if (defaultDataSourceReport != null && defaultDataSourceReport.Columns.Count > 0)
				{
					for (int i = 0; i < defaultDataSourceReport.Columns.Count; i++)
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["Name"] = defaultDataSourceReport.Columns[i].ColumnName;
						dataRow["Code"] = defaultDataSourceReport.Columns[i].ColumnName;
						dataTable.Rows.Add(dataRow);
					}
				}
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return dataTable;
		}
	}

	private DataTable DataReportByCode(string code)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "Select * from APZ_OUDR where \"Code\" = @code;";
			return result = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable GetReportByCode(string code)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_D_SettingReport_GetReportByCode";
			return result = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@code", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return result;
		}
	}

	public DataTable GetDataStoreNotParameter(string store)
	{
		DataTable result = new DataTable();
		try
		{
			return result = ExecuteDataTable(store, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return result;
		}
	}
}
