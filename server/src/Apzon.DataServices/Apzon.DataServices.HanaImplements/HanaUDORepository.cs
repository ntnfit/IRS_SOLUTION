using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaUDORepository : UDORepository
{
	public HanaUDORepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override HttpResult Update(DataTable udoTable, DataTable dtFather, DataSet dsChild, string sessionId)
	{
		try
		{
			HttpResult httpResult = new HttpResult();
			string text = "";
			string text2 = Function.ToString(udoTable.Rows[0]["Code"]);
			string obj = ((dtFather.Rows[0]["DocEntry"] != null) ? dtFather.Rows[0]["DocEntry"].ToString() : "0");
			if (Function.ParseInt(obj) > 0)
			{
				text = MappingXml(udoTable, dtFather, dsChild, sessionId, "Update");
				if (Function.ToString(udoTable.Rows[0]["TYPE"]).Equals("1"))
				{
					string objectKey = Function.ToString(dtFather.Rows[0]["Code"]);
					httpResult = DataService.ProcessJson(ApzObjectTypes.oNone, sessionId, text, Enums.HttpAction.Update, objectKey, text2);
				}
				else
				{
					httpResult = DataService.ProcessJson(ApzObjectTypes.oNone, sessionId, text, Enums.HttpAction.Update, Function.ParseInt(obj), text2);
				}
			}
			else
			{
				text = MappingXml(udoTable, dtFather, dsChild, sessionId, "Add");
				httpResult = DataService.ProcessJson(ApzObjectTypes.oNone, sessionId, text, Enums.HttpAction.AddNew, null, text2);
			}
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Execute UDO ERROR: " + text2, sessionId, httpResult.Message);
			}
			else if (Function.ParseInt(obj) > 0 && !string.IsNullOrEmpty(Function.ToString(httpResult.Content)))
			{
				Logging.Write("WATCH", "Execute UDO Success: " + text2, Function.ToString(httpResult.Content));
			}
			return httpResult;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override string Execute(string udoCode, string udoName, int docEntry, string sessionId, string command)
	{
		try
		{
			Enums.HttpAction action = Enums.HttpAction.Close;
			if (command.Equals("Close"))
			{
				action = Enums.HttpAction.Close;
			}
			else if (command.Equals("Cancel"))
			{
				action = Enums.HttpAction.Cancel;
			}
			else if (command.Equals("Remove"))
			{
				action = Enums.HttpAction.Delete;
			}
			HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oNone, sessionId, "", action, docEntry, udoCode);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Execute UDO ERROR: " + udoCode, sessionId, command, httpResult.Message);
				return httpResult.Message;
			}
			Logging.Write("WATCH", "Execute UDO Success: " + udoCode, sessionId, command, docEntry.ToString());
			return httpResult.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string ExecuteMasterData(string udoCode, string udoName, string code, string sessionId, string command)
	{
		try
		{
			Enums.HttpAction action = Enums.HttpAction.Close;
			if (command.Equals("Close"))
			{
				action = Enums.HttpAction.Close;
			}
			else if (command.Equals("Cancel"))
			{
				action = Enums.HttpAction.Cancel;
			}
			else if (command.Equals("Remove"))
			{
				action = Enums.HttpAction.Delete;
			}
			HttpResult httpResult = DataService.ProcessJson(ApzObjectTypes.oNone, sessionId, "", action, code, udoCode);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				Logging.Write("ERROR", "Execute UDO ERROR: " + udoCode, command, httpResult.Message);
				return httpResult.Message;
			}
			Logging.Write("WATCH", "Execute UDO Success: " + udoCode, command, code);
			return httpResult.Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string MappingXml(DataTable udoTable, DataTable dtFather, DataSet dsChild, string sessionId, string command)
	{
		Function.ToString(udoTable.Rows[0]["TYPE"]);
		string text = "{\r\n";
		if (command.ToUpper().Equals("UPDATE"))
		{
			text += Function.GetJsonString("DocEntry", dtFather.Rows[0]["DocEntry"]);
		}
		IEnumerable<DataColumn> enumerable = dtFather.Columns.Cast<DataColumn>();
		foreach (DataRow row in dtFather.Rows)
		{
			foreach (DataColumn item in enumerable)
			{
				if (item.ColumnName.Equals("DocEntry") || item.ColumnName.Equals("DocNum"))
				{
					continue;
				}
				if (item.ColumnName.Equals("Series") && Function.ToString(ExecuteScarlar("select \"MngSeries\" from OUDO where \"Code\" = @udoCode;", CommandType.Text, new SqlParameter("@udoCode", udoTable.Rows[0]["Code"]))) == "Y" && Function.ParseInt(row["Series"]) > 0)
				{
					text += Function.GetJsonString("Series", Function.ParseInt(row["Series"]));
				}
				if (Function.ToString(row[item]) == "")
				{
					text += $"\"{item.ColumnName}\": null,";
				}
				else if (item.DataType == typeof(double) || item.DataType == typeof(decimal))
				{
					text += Function.GetJsonString(item.ColumnName, row[item]);
				}
				else if (item.DataType == typeof(short))
				{
					if (Function.ToString(row[item]).Length < 4)
					{
						string arg = Function.ParseInt(row[item]).ToString("0000");
						string value = $"{arg}";
						text += Function.GetJsonString(item.ColumnName, value);
					}
					else
					{
						text += Function.GetJsonString(item.ColumnName, Function.ParseInt(row[item]));
					}
				}
				else if (!(item.DataType == typeof(int)) && !(item.DataType == typeof(int)) && !(item.DataType == typeof(long)))
				{
					text = ((!(item.DataType == typeof(DateTime))) ? (text + Function.GetJsonString(item.ColumnName, row[item])) : (text + Function.GetJsonString(item.ColumnName, Function.ParseDateTime(row[item]))));
				}
				else
				{
					text += Function.GetJsonString(item.ColumnName, row[item]);
				}
			}
		}
		if (dsChild.Tables.Count > 0)
		{
			foreach (DataTable table in dsChild.Tables)
			{
				text += $" \"{table.TableName}Collection\":[";
				IEnumerable<DataColumn> columnNames = from DataColumn c in table.Columns
					where c.ColumnName != "Code" && c.ColumnName != "DocEntry"
					select c;
				var source = table.AsEnumerable().Select((DataRow t, int index) => new
				{
					index = index,
					Item = t
				});
				ConcurrentBag<Tuple<int, string>> lstItemJson = new ConcurrentBag<Tuple<int, string>>();
				if (Parallel.ForEach(source, row =>
				{
					string text2 = "";
					text2 += "{\r\n";
					foreach (DataColumn item2 in columnNames)
					{
						if (item2.ColumnName.Equals("DocEntry") && command.ToUpper().Equals("UPDATE"))
						{
							text2 += Function.GetJsonString(item2.ColumnName, Function.ParseInt(dtFather.Rows[0]["DocEntry"]));
						}
						else if (!item2.ColumnName.Equals("LineId") || Function.ParseInt(row.Item[item2]) != 0)
						{
							if (Function.ToString(row.Item[item2]) == "")
							{
								text2 += $"\"{item2.ColumnName}\": null,";
							}
							else if (item2.DataType == typeof(double) || item2.DataType == typeof(decimal))
							{
								text2 += Function.GetJsonString(item2.ColumnName, row.Item[item2]);
							}
							else if (item2.DataType == typeof(short))
							{
								if (Function.ToString(row.Item[item2]).Length < 4)
								{
									string arg2 = Function.ParseInt(row.Item[item2]).ToString("0000");
									string value2 = $"{arg2}";
									text2 += Function.GetJsonString(item2.ColumnName, value2);
								}
								else
								{
									text2 += Function.GetJsonString(item2.ColumnName, Function.ParseInt(row.Item[item2]));
								}
							}
							else
							{
								text2 = ((!(item2.DataType == typeof(int)) && !(item2.DataType == typeof(int)) && !(item2.DataType == typeof(long))) ? ((!(item2.DataType == typeof(DateTime))) ? (text2 + Function.GetJsonString(item2.ColumnName, row.Item[item2])) : (text2 + Function.GetJsonString(item2.ColumnName, Function.ParseDateTime(row.Item[item2])))) : (text2 + Function.GetJsonString(item2.ColumnName, row.Item[item2])));
							}
						}
					}
					text2 += "},";
					lstItemJson.Add(new Tuple<int, string>(row.index, text2));
				}).IsCompleted)
				{
					text += string.Join("", (from t in lstItemJson.ToArray()
						orderby t.Item1
						select t.Item2).ToArray());
					try
					{
						GC.Collect();
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", ex);
					}
					text += $"],";
					continue;
				}
				throw new Exception("Could not complete action-parallel mapping json");
			}
		}
		return text + "\r\n}";
	}
}
