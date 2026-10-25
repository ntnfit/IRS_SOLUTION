using System;
using System.Collections.Generic;
using System.Data;
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

public class SettingApprovalDetailRepository : BaseService<DataSet>, ISettingApprovalDetail
{
	private const string docTemp = "#APZ_D_OSAD";

	private const string fieldTemp = "#APZ_D_SAD2";

	private const string headerTemp = "#APZ_D_SAD1";

	public SettingApprovalDetailRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable LoadData(int id, string fromDate, string toDate, string report)
	{
		try
		{
			DateTime? dateTime = Function.ParseDateTime(fromDate, "yyyyMMdd");
			DateTime? dateTime2 = Function.ParseDateTime(toDate, "yyyyMMdd");
			string query = "APZ_D_SettingApprovalDetail_LoadData";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@id", id), new SqlParameter("@fromDate", dateTime), new SqlParameter("@toDate", dateTime2), new SqlParameter("@objectType", report));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDataHeaderById(int Id)
	{
		try
		{
			return ExecuteDataTable("APZ_D_SettingApprovalDetail_GetDataHeaderById", CommandType.StoredProcedure, new SqlParameter("@Id", Id));
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
			return ExecuteDataTable("APZ_D_SettingApprovalDetail_GetLineById", CommandType.StoredProcedure, new SqlParameter("@Id", Id));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
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

	public HttpResult DeleteData(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@Id", Function.ParseInt(objId));
			DataTable dataTable = ExecuteDataTable("APZ_D_SettingApprovalDetail_Remove", CommandType.StoredProcedure, sqlParameter);
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

	public HttpResult SaveData(DataSet ds, Enums.TransactionType tranType, ref string objId, ref string objNum)
	{
		try
		{
			DataTable tableStruct = GetTableStruct("APZ_D_OSAD");
			DataTable dataTable = ds.Tables[0];
			if (!tableStruct.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source = from DataColumn t in tableStruct.Columns
				select t.ColumnName;
			dataTable = dataTable.DefaultView.ToTable(distinct: false, source.ToArray());
			CreateTempTable(dataTable, "#APZ_D_OSAD");
			BulkCopy(dataTable, "#APZ_D_OSAD");
			DataTable tableStruct2 = GetTableStruct("APZ_D_SAD2");
			DataTable dataTable2 = ds.Tables[1];
			if (!tableStruct2.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source2 = from DataColumn t in tableStruct2.Columns
				select t.ColumnName;
			dataTable2 = dataTable2.DefaultView.ToTable(distinct: false, source2.ToArray());
			CreateTempTable(dataTable2, "#APZ_D_SAD2");
			BulkCopy(dataTable2, "#APZ_D_SAD2");
			DataTable tableStruct3 = GetTableStruct("APZ_D_SAD1");
			DataTable dataTable3 = ds.Tables[2];
			if (!tableStruct3.IsNotNull())
			{
				throw new Exception("Could not get data structure");
			}
			IEnumerable<string> source3 = from DataColumn t in tableStruct3.Columns
				select t.ColumnName;
			dataTable3 = dataTable3.DefaultView.ToTable(distinct: false, source3.ToArray());
			CreateTempTable(dataTable3, "#APZ_D_SAD1");
			BulkCopy(dataTable3, "#APZ_D_SAD1");
			string value = Function.ToString(tranType);
			SqlParameter sqlParameter = new SqlParameter("@type", value);
			DataTable dataTable4 = ExecuteDataTable("APZ_D_SettingApprovalDetail_Save", CommandType.StoredProcedure, sqlParameter);
			if (dataTable4 != null && dataTable4.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable4.Rows[0][1])))
				{
					objId = Function.ToString(dataTable4.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable4.Rows[0][0])}-{Function.ToString(dataTable4.Rows[0][1])}");
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

	public ResponseResult GetDataSource(string docNo)
	{
		DataSet dataSet = new DataSet();
		try
		{
			string query = "APZ_D_SettingApprovalDetail_GetDataSource";
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@docNo",
					Value = docNo
				}
			};
			dataSet = ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
			return new ResponseResult
			{
				Message = "",
				MessageCode = "200",
				Content = dataSet
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), " FAILED ", ex.Message);
			return new ResponseResult
			{
				Message = ex.Message,
				MessageCode = "403",
				Content = null
			};
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
					text += "\"size\":1,";
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
					text = text + "\"size\":" + Function.ParseFloat(dataTable.Rows[num]["size"]);
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
			DataTable dataTable = ExecuteDataTable("APZ_D_SettingApprovalDetail_GetLineById", CommandType.StoredProcedure, new SqlParameter("@Id", Id));
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
			DataTable dataHeaderById = GetDataHeaderById(Id);
			return new ResponseResult
			{
				Message = "",
				MessageCode = "200",
				Content = Tuple.Create(item, dataTable2, dataHeaderById)
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
}
