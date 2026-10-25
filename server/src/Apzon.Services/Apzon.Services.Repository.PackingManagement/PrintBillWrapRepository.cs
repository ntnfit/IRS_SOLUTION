using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.PackingManagement;

namespace Apzon.Services.Repository.PackingManagement;

public class PrintBillWrapRepository : BaseService<DataSet>, IPrintBillWrap
{
	private const string TableTempDoc = "#tempDoc";

	private const string TableTempLine = "#tempLine";

	public PrintBillWrapRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetCase(int POCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@POCode", POCode));
			return ExecuteDataTable("APZ_PackingManagement_GetCase", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetItemByCaseCode(string CaseCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CaseCode", CaseCode));
			return ExecuteDataTable("APZ_PackingManagement_GetItemByIdCase", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string DelCase(int LineNumber)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@LineNumber", LineNumber));
			ExecuteDataTable("APZ_PackingManagement_DelCase", CommandType.StoredProcedure, list.ToArray());
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string DelItem(string LineNum)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@LineNum", LineNum));
			ExecuteDataTable("APZ_PackingManagement_DelItem", CommandType.StoredProcedure, list.ToArray());
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string InsertCase(string CaseCode, int POCode, int USERID)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CaseCode", CaseCode));
			list.Add(new SqlParameter("@POCode", POCode));
			list.Add(new SqlParameter("@USERID", USERID));
			ExecuteDataTable("APZ_PackingManagement_InsertCase", CommandType.StoredProcedure, list.ToArray());
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string InsertItem(string CaseCode, string BundleCode, int LineNumber)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@CaseCode", CaseCode));
			list.Add(new SqlParameter("@BundleCode", BundleCode));
			list.Add(new SqlParameter("@LineNumber", LineNumber));
			ExecuteDataTable("APZ_PackingManagement_InsertItem", CommandType.StoredProcedure, list.ToArray());
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetPO()
	{
		try
		{
			return ExecuteDataTable("APZ_PackingManagement_GetPO", CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public new virtual HttpResult Process(ApzObjectTypes objtype, int bplId, Enums.TransactionType transtype, DataSet ds, string objId = "", string objCode = "", bool lockNumbering = true)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				HttpResult result = new HttpResult();
				using (DbTransaction dbTransaction = DatabaseService.BeginTransaction())
				{
					DatabaseService.LockNumbering(lockNumbering, objtype);
					string objNum = "";
					HttpResult httpResult = ((objtype != ApzObjectTypes.oUserDefinedObject) ? ProcessData(objtype, bplId, ref transtype, ds, ref objId, ref objNum) : ProcessData(objtype, bplId, ref transtype, ds, ref objId, ref objNum, objCode));
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						return new HttpResult(Enums.MessageCode.Error, httpResult.Message);
					}
					string objtype2;
					if (objtype == ApzObjectTypes.oUserDefinedObject)
					{
						objtype2 = objCode;
					}
					else
					{
						try
						{
							int num = (int)objtype;
							objtype2 = num.ToString();
						}
						catch (Exception)
						{
							objtype2 = objtype.ToString();
						}
					}
					HttpResult httpResult2 = ProcessProcedure(objtype2, transtype, -1, "", objId);
					if (!string.IsNullOrEmpty(httpResult2.Message))
					{
						return httpResult2;
					}
					DataTable dataTable = (DataTable)httpResult2.Content;
					if (dataTable != null && dataTable.Rows.Count > 0)
					{
						int num2 = Function.ParseInt(dataTable.Rows[0][0]);
						string text = Function.ToString(dataTable.Rows[0][1]);
						if (!string.IsNullOrEmpty(text))
						{
							return new HttpResult(Enums.MessageCode.Error, num2 + "-" + text);
						}
					}
					HttpResult httpResult3 = NotifyProcedure(objtype2, transtype, -1, "", objId);
					if (!string.IsNullOrEmpty(httpResult3.Message))
					{
						return httpResult3;
					}
					DataTable dataTable2 = (DataTable)httpResult3.Content;
					if (dataTable2 != null && dataTable2.Rows.Count > 0)
					{
						int num2 = Function.ParseInt(dataTable2.Rows[0][0]);
						string text = Function.ToString(dataTable2.Rows[0][1]);
						if (!string.IsNullOrEmpty(text))
						{
							return new HttpResult(Enums.MessageCode.Error, num2 + "-" + text);
						}
					}
					dbTransaction.Commit();
					result = new HttpResult(Enums.MessageCode.Success, "", objNum);
				}
				bool flag = true;
				using (DbTransaction dbTransaction2 = DatabaseService.BeginTransaction())
				{
					if (CommonList.ListLogInstance.TryGetValue(objtype, out var value) && value != null)
					{
						try
						{
							string query = "select top 1 \"MaxHistory\" from OADM;";
							int num3 = Function.ParseInt(ExecuteScalar(query, CommandType.Text));
							string[] array = objId.Split(';');
							if (num3 >= 0)
							{
								LogInstance logInstance = value.AsEnumerable().FirstOrDefault((LogInstance t) => t.HistoryTableName == t.TableLogInstanceKey);
								string[] array2 = array;
								foreach (string text2 in array2)
								{
									if (string.IsNullOrEmpty(text2))
									{
										continue;
									}
									if (logInstance != null)
									{
										int headerMaxInsKey = DatabaseService.MaxInstanceKey(text2, logInstance);
										foreach (LogInstance item in value)
										{
											DatabaseService.ProcessChangeLog(transtype, objtype, logInstance, item, headerMaxInsKey, text2, num3);
										}
										continue;
									}
									Logging.Write("ERROR", $"create log failed, throw exception. object {objtype} id {text2}. No Header Table Log Found!");
									flag = false;
									break;
								}
							}
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", $"create log failed, throw exception. object {objtype} id {objId}. {ex2.Message}");
							flag = false;
						}
						if (flag)
						{
							dbTransaction2.Commit();
						}
					}
				}
				return result;
			}
		}
		catch (SqlException ex3)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex3.Message);
			return new HttpResult(Enums.MessageCode.Error, "Could not commit transaction");
		}
		catch (Exception ex4)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex4.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex4.Message);
		}
	}
}
