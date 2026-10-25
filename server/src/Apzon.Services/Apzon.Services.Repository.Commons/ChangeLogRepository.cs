using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class ChangeLogRepository : BaseService<DataTable>, IChangeLog
{
	private string _dateFormat;

	private string _dateSeparator;

	public ChangeLogRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
		GetFormatString();
	}

	public int GetDocEntryItemDetail(int id)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT TOP 1 U_Itemdetail Id FROM dbo.OOPR WHERE OpprId=@DocEntry", CommandType.Text, new SqlParameter("@DocEntry", id));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["Id"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return 0;
	}

	public DataTable GetLog(int docEntry)
	{
		try
		{
			return ExecuteDataTable("apz_changelog", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetChangeLogRow(int docEntry, int logId, int type)
	{
		try
		{
			return ExecuteDataTable("apz_changelog_row", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry), new SqlParameter("@LogId", logId), new SqlParameter("@Type", type));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetChangeLog(GetChangeLogParams pr)
	{
		try
		{
			List<LogInstance> value = new List<LogInstance>();
			if (CommonList.ChangeLogObjMapping.TryGetValue(pr.Object, out var value2))
			{
				if (CommonList.ListLogInstance.TryGetValue(value2, out value))
				{
					LogInstance logInstance = value.AsEnumerable().FirstOrDefault((LogInstance t) => t.HistoryTableName == t.TableLogInstanceKey);
					if (logInstance != null)
					{
						string text = "";
						List<SqlParameter> list = new List<SqlParameter>();
						string[] array = logInstance.Key.Split(',');
						string[] array2 = pr.PrimaryKey.Split(',');
						for (int num = 0; num < array.Length; num++)
						{
							string text2 = array[num];
							if (!string.IsNullOrWhiteSpace(text2))
							{
								text = text + " AND a.\"" + text2 + "\"=@PrimaryKey" + num;
								list.Add(new SqlParameter("@PrimaryKey" + num, array2[num]));
							}
						}
						string text3 = "";
						string text4 = "";
						string text5 = "";
						if (!string.IsNullOrWhiteSpace(logInstance.UpdateDateKey))
						{
							text4 = text4 + ", a.\"" + logInstance.UpdateDateKey + "\" as \"UpdatedDate\" ";
						}
						if (!string.IsNullOrWhiteSpace(logInstance.UpdateTimeKey))
						{
							text5 = text5 + " , a.\"" + logInstance.UpdateTimeKey + "\" as \"UpdatedTime\" ";
						}
						if (string.IsNullOrWhiteSpace(logInstance.UserSignCode2Key))
						{
							text3 = (string.IsNullOrWhiteSpace(logInstance.UserSign2Key) ? (text3 + $"Select DISTINCT(a.\"{logInstance.LogInstanceKey}\") as \"LogInstance\",'{(int)pr.Object}' as \"ObjectCode\",b.\"U_NAME\" as \"UserName\" " + text4 + text5 + "from  " + logInstance.HistoryTableName + " a left join OUSR b on 0 =1 where 1=1 " + text) : (text3 + $"Select DISTINCT(a.\"{logInstance.LogInstanceKey}\") as \"LogInstance\",'{(int)pr.Object}' as \"ObjectCode\",b.\"U_NAME\" as \"UserName\" " + text4 + text5 + "from  " + logInstance.HistoryTableName + " a left join OUSR b on a.\"" + logInstance.UserSign2Key + "\" = b.\"USERID\" where   1=1 " + text));
						}
						else
						{
							text3 = text3 + $"Select DISTINCT(a.\"{logInstance.LogInstanceKey}\") as \"LogInstance\",'{(int)pr.Object}' as \"ObjectCode\",b.\"U_NAME\" as \"UserName\" " + text4 + text5 + "from  " + logInstance.HistoryTableName + " a left join OUSR b on a.\"" + logInstance.UserSignCode2Key + "\" = b.\"USER_CODE\" where  1=1 " + text;
						}
						text3 = text3 + " order by a.\"" + logInstance.LogInstanceKey + "\" desc;";
						return ExecuteDataTable(text3, CommandType.Text, list.ToArray());
					}
					Logging.Write("ERROR", "No header table Defined");
				}
				else
				{
					Logging.Write("ERROR", $"No Log Config for this Object {value2}");
				}
			}
			else
			{
				Logging.Write("ERROR", $"No Mapping Object found for this Change Log Object {pr.Object}");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetChangeLogDifferences(GetChangeLogParams pr)
	{
		try
		{
			if (CommonList.ChangeLogObjMapping.TryGetValue(pr.Object, out var value))
			{
				if (CommonList.ListLogInstance.TryGetValue(value, out var value2))
				{
					DataTable dataTable = new DataTable();
					dataTable.Columns.Add("LineNumber", typeof(string));
					dataTable.Columns.Add("ArrayOffset", typeof(string));
					dataTable.Columns.Add("Date", typeof(DateTime));
					dataTable.Columns.Add("ChangedField", typeof(string));
					dataTable.Columns.Add("OldValue", typeof(string));
					dataTable.Columns.Add("NewValue", typeof(string));
					dataTable.Columns.Add("UserName", typeof(string));
					DateTime? dateTime = null;
					string value3 = "";
					int num = 0;
					LogInstance logInstance = value2.AsEnumerable().FirstOrDefault((LogInstance t) => t.HistoryTableName == t.TableLogInstanceKey);
					foreach (LogInstance item in value2)
					{
						if (item.TableLogInstanceKey != logInstance.TableLogInstanceKey)
						{
							continue;
						}
						string text = "";
						string[] array = item.Key.Split(',');
						string[] array2 = pr.PrimaryKey.Split(',');
						for (int num2 = 0; num2 < array.Length; num2++)
						{
							string text2 = array[num2];
							if (!string.IsNullOrWhiteSpace(text2))
							{
								text = text + " AND a.\"" + text2 + "\"='" + array2[num2] + "'";
							}
						}
						List<string> list = new List<string> { item.LogInstanceKey, item.UpdateDateKey, item.UpdateTimeKey, item.CreateDateKey, item.CreateTimeKey, item.UserSignKey, item.UserSign2Key, item.UserSignCodeKey, item.UserSignCode2Key, "CL_UserChanged" };
						if (item.NotShowLogKey != null)
						{
							list.AddRange(item.NotShowLogKey.ToList());
						}
						string text3 = "";
						if (!string.IsNullOrWhiteSpace(item.UserSignCode2Key))
						{
							text3 += $"Select a.*,b.\"U_NAME\" as \"CL_UserChanged\" from  {item.HistoryTableName} a left join OUSR b on a.\"{item.UserSignCode2Key}\" = b.\"USER_CODE\" where  1=1  {text} and a.\"{item.LogInstanceKey}\" = {pr.LogInstance};";
							text3 += $"Select a.*,b.\"U_NAME\" as \"CL_UserChanged\" from  {item.HistoryTableName} a left join OUSR b on a.\"{item.UserSignCode2Key}\" = b.\"USER_CODE\" where  1=1  {text} and a.\"{item.LogInstanceKey}\" = {pr.LogInstance2};";
						}
						else if (!string.IsNullOrWhiteSpace(item.UserSign2Key))
						{
							text3 += $"Select a.*,b.\"U_NAME\" as \"CL_UserChanged\" from  {item.HistoryTableName} a left join OUSR b on a.\"{item.UserSign2Key}\" = b.\"USERID\"  where  1=1  {text} and a.\"{item.LogInstanceKey}\" = {pr.LogInstance};";
							text3 += $"Select a.*,b.\"U_NAME\"  as \"CL_UserChanged\" from  {item.HistoryTableName} a left join OUSR b on a.\"{item.UserSign2Key}\" = b.\"USERID\"  where  1=1  {text} and a.\"{item.LogInstanceKey}\" = {pr.LogInstance2};";
						}
						else
						{
							text3 += $"Select *,b.\"U_NAME\"  as \"CL_UserChanged\" from  {item.HistoryTableName} a left join OUSR b on 0=1 where  1=1 {text} and a.\"{item.LogInstanceKey}\" = {pr.LogInstance};";
							text3 += $"Select *,b.\"U_NAME\"  as \"CL_UserChanged\" from  {item.HistoryTableName} a left join OUSR b on 0=1 where  1=1  {text} and a.\"{item.LogInstanceKey}\" = {pr.LogInstance2};";
						}
						DataSet dataSet = ExecuteData(text3, CommandType.Text, (SqlParameter[])null);
						DataTable dataTable2 = dataSet.Tables[0].Copy();
						DataTable dataTable3 = dataSet.Tables[1].Copy();
						List<DataRow> list2 = new List<DataRow>();
						List<string> list3 = new List<string>();
						List<string> list4 = new List<string>();
						if (!string.IsNullOrWhiteSpace(item.LineKey))
						{
							string[] array3 = item.LineKey.Split(',');
							foreach (string text4 in array3)
							{
								if (dataTable2.Columns.Contains(text4))
								{
									list4.Add(text4);
									list.Add(text4);
								}
							}
							DataView dataView = new DataView(dataTable2);
							DataView dataView2 = new DataView(dataTable3);
							DataTable source = dataView.ToTable("src", distinct: false, list4.ToArray());
							list2 = dataView2.ToTable("des", distinct: false, list4.ToArray()).AsEnumerable().Except(source.AsEnumerable(), DataRowComparer.Default)
								.ToList();
						}
						if (!string.IsNullOrWhiteSpace(item.Key))
						{
							string[] array3 = item.Key.Split(',');
							foreach (string text5 in array3)
							{
								if (dataTable2.Columns.Contains(text5))
								{
									list3.Add(text5);
									list.Add(text5);
								}
							}
						}
						for (int num4 = 0; num4 < dataTable2.Rows.Count; num4++)
						{
							DataRow dataRow = dataTable2.Rows[num4];
							if (dataTable3.IsNotNull() && (item.HistoryTableName == item.TableLogInstanceKey || num < 1))
							{
								value3 = Function.ToString(dataTable3.Rows[0]["CL_UserChanged"]);
								if (dataTable2.Columns.Contains(Function.ToString(item.UpdateDateKey)) && !string.IsNullOrWhiteSpace(Function.ToString(dataTable3.Rows[0][item.UpdateDateKey])))
								{
									dateTime = Function.ParseDateTimes(dataTable3.Rows[0][item.UpdateDateKey]);
								}
								num++;
							}
							string text6 = "1=1 ";
							DataRow dataRow2;
							if (!string.IsNullOrWhiteSpace(item.LineKey))
							{
								foreach (string item2 in list4)
								{
									if (dataTable2.Columns.Contains(item2))
									{
										text6 = text6 + " AND " + item2 + "='" + Function.ToString(dataRow[item2]) + "'";
									}
								}
								dataRow2 = dataTable3.Select(text6).FirstOrDefault();
							}
							else
							{
								dataRow2 = dataTable3.AsEnumerable().FirstOrDefault();
							}
							if (dataRow2 != null)
							{
								if (string.IsNullOrWhiteSpace(value3))
								{
									value3 = Function.ToString(dataTable3.Rows[0]["CL_UserChanged"]);
								}
								if (!dateTime.HasValue && dataTable2.Columns.Contains(Function.ToString(item.UpdateDateKey)) && !string.IsNullOrWhiteSpace(Function.ToString(dataTable3.Rows[0][item.UpdateDateKey])))
								{
									dateTime = Function.ParseDateTimes(dataTable3.Rows[0][item.UpdateDateKey]);
								}
								object[] itemArray = dataRow.ItemArray;
								object[] itemArray2 = dataRow2.ItemArray;
								int num5 = dataRow.ItemArray.Length;
								for (int num6 = 0; num6 < num5; num6++)
								{
									if (!(Function.ToString(itemArray[num6]) != Function.ToString(itemArray2[num6])) || list.Contains(dataTable2.Columns[num6].ColumnName))
									{
										continue;
									}
									DataRow dataRow3 = dataTable.NewRow();
									string text7 = "";
									foreach (string item3 in list4)
									{
										if (dataTable2.Columns.Contains(item3))
										{
											text7 = ((!(dataTable2.Columns[item3].DataType == typeof(DateTime))) ? (text7 + item3 + ":" + dataRow[item3]?.ToString() + ",") : (text7 + item3 + ":" + Function.ParseDateTimes(dataRow[item3]).ToString(GetDateFormat(_dateFormat, _dateSeparator)) + ","));
										}
									}
									dataRow3["LineNumber"] = (text7.EndsWith(",") ? text7.Substring(0, text7.Length - 1) : text7);
									dataRow3["ArrayOffset"] = item.TableName;
									dataRow3["Date"] = ((object)dateTime) ?? DBNull.Value;
									dataRow3["ChangedField"] = dataTable2.Columns[num6].ColumnName;
									dataRow3["OldValue"] = Function.ToString(itemArray[num6]);
									dataRow3["NewValue"] = Function.ToString(itemArray2[num6]);
									dataRow3["UserName"] = value3;
									dataTable.Rows.Add(dataRow3);
								}
								continue;
							}
							DataRow dataRow4 = dataTable.NewRow();
							string text8 = "";
							foreach (string item4 in list4)
							{
								if (dataTable2.Columns.Contains(item4))
								{
									text8 = text8 + item4 + ":" + dataRow[item4]?.ToString() + ",";
								}
							}
							dataRow4["LineNumber"] = (text8.EndsWith(",") ? text8.Substring(0, text8.Length - 1) : text8);
							dataRow4["ArrayOffset"] = item.TableName;
							dataRow4["Date"] = ((object)dateTime) ?? DBNull.Value;
							dataRow4["ChangedField"] = "";
							dataRow4["OldValue"] = "";
							dataRow4["NewValue"] = "Row deleted!";
							dataRow4["UserName"] = value3;
							dataTable.Rows.Add(dataRow4);
						}
						if (!dataTable2.IsNotNull())
						{
							continue;
						}
						foreach (DataRow item5 in list2)
						{
							DataRow dataRow5 = dataTable.NewRow();
							string text9 = "";
							foreach (string item6 in list4)
							{
								if (dataTable2.Columns.Contains(item6))
								{
									text9 = text9 + item6 + ":" + item5[item6]?.ToString() + ",";
								}
							}
							dataRow5["LineNumber"] = (text9.EndsWith(",") ? text9.Substring(0, text9.Length - 1) : text9);
							dataRow5["ArrayOffset"] = item.TableName;
							dataRow5["Date"] = ((object)dateTime) ?? DBNull.Value;
							dataRow5["ChangedField"] = "";
							dataRow5["OldValue"] = "";
							dataRow5["NewValue"] = "Row added!";
							dataRow5["UserName"] = value3;
							dataTable.Rows.Add(dataRow5);
						}
					}
					return dataTable;
				}
				Logging.Write("ERROR", $"No Log Config for this Object {value}");
			}
			else
			{
				Logging.Write("ERROR", $"No Mapping Object found for this Change Log Object {pr.Object}");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	private static string GetDateFormat(string format, string separator)
	{
		try
		{
			return (format.ToUpper() switch
			{
				"DD/MM/YY" => "dd/MM/yy", 
				"DD/MM/CCYY" => "dd/MM/yyyy", 
				"MM/DD/YY" => "MM/dd/yy", 
				"MM/DD/CCYY" => "MM/dd/yyyy", 
				"CCYY/MM/DD" => "yyyy/MM/dd", 
				"DD/Month/YYYY" => "dd/MMMM/yyyy", 
				"YY/MM/DD" => "yy/MM/dd", 
				_ => "dd/MM/yy", 
			}).Replace("/", separator ?? "-");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "dd/MM/yyyy HH:mm:ss";
		}
	}

	private void GetFormatString()
	{
		DataTable dataTable = new DataTable();
		try
		{
			dataTable = ExecuteDataTable("APZ_GeneralSetting_GetFormatString", CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable.IsNotNull())
			{
				_dateFormat = Function.ToString(dataTable.Rows[0]["DateFormat"]);
				_dateSeparator = Function.ToString(dataTable.Rows[0]["DateSep"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}
}
