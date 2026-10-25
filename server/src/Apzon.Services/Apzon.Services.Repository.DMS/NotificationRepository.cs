using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.DMS;

namespace Apzon.Services.Repository.DMS;

internal class NotificationRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), INotification
{
	private string _processType = "";

	public DataTable GetListApp()
	{
		try
		{
			string query = "APZ_D_NotificationEvent_GetAllApp";
			new DataTable();
			return ExecuteDataTable(query, CommandType.StoredProcedure);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetEventSetting(string appName)
	{
		try
		{
			string query = "APZ_D_NotificationEvent_GetEventSetting";
			new DataTable();
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter
				{
					ParameterName = "@appName",
					Value = appName
				}
			};
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetNotifications(string userName, string appName, string dbName, int startIndex, int pageSize)
	{
		try
		{
			string query = "APZ_D_GetListNotification";
			new DataTable();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userName", Function.ToString(userName)));
			list.Add(new SqlParameter("@appName", Function.ToString(appName)));
			list.Add(new SqlParameter("@dbName", Function.ToString(dbName)));
			list.Add(new SqlParameter("@startIndex", Function.ParseInt(startIndex)));
			list.Add(new SqlParameter("@pageSize", Function.ParseInt(pageSize)));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetUnreadNotifications(string userName, string appName, string dbName)
	{
		try
		{
			string query = "APZ_D_GetUnreadNotifications";
			new DataTable();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@userName", Function.ToString(userName)));
			list.Add(new SqlParameter("@appName", Function.ToString(appName)));
			list.Add(new SqlParameter("@dbName", Function.ToString(dbName)));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public string PushDeviceToken(DataTable dt)
	{
		_processType = "PushDeviceToken";
		return Process(ApzObjectTypes.oNotification, -1, Enums.TransactionType.A, dt).Message;
	}

	public string RemoveDeviceToken(DataTable dt)
	{
		_processType = "RemoveDeviceToken";
		return Process(ApzObjectTypes.oNotification, -1, Enums.TransactionType.A, dt).Message;
	}

	public string UpdateNotification(DataTable dt)
	{
		_processType = "UpdateNotification";
		return Process(ApzObjectTypes.oNotification, -1, Enums.TransactionType.A, dt).Message;
	}

	public string MarkAllAsRead(DataTable dt)
	{
		_processType = "MarkAllAsRead";
		return Process(ApzObjectTypes.oNotification, -1, Enums.TransactionType.A, dt).Message;
	}

	public string UpdateNotificationEvent(DataTable dt)
	{
		_processType = "UpdateNotificationEvent";
		return Process(ApzObjectTypes.oNotification, -1, Enums.TransactionType.A, dt).Message;
	}

	public string AddNotificationEvent(DataTable dt)
	{
		_processType = "AddNotificationEvent";
		return Process(ApzObjectTypes.oNotification, -1, Enums.TransactionType.A, dt).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		try
		{
			switch (_processType)
			{
			case "PushDeviceToken":
			{
				CreateTempTable(dt, "#APZ_D_OADT");
				BulkCopy(dt, "#APZ_D_OADT");
				DataTable dataTable5 = ExecuteDataTable("APZ_D_InsertDeviceToken", CommandType.StoredProcedure);
				if (dataTable5 != null && dataTable5.Rows.Count > 0)
				{
					return new HttpResult(Enums.MessageCode.Error, 810037 + " - " + Function.ToString(dataTable5.Rows[0]["ErrorMessage"]));
				}
				break;
			}
			case "RemoveDeviceToken":
			{
				CreateTempTable(dt, "#APZ_D_OADT");
				BulkCopy(dt, "#APZ_D_OADT");
				DataTable dataTable4 = ExecuteDataTable("APZ_D_RemoveDeviceToken", CommandType.StoredProcedure);
				if (dataTable4 != null && dataTable4.Rows.Count > 0)
				{
					Logging.Write("TRACE", "Remove Device Token Successfully! ID: " + dataTable4.Rows[0][0]);
				}
				break;
			}
			case "UpdateNotification":
			{
				DataTable dataTable2 = GetTableStruct("APZ_D_ELN1").Clone();
				dataTable2.ImportRow(dt.Rows[0]);
				CreateTempTable(dataTable2, "#APZ_D_ELN1");
				BulkCopy(dataTable2, "#APZ_D_ELN1");
				if (Function.ParseInt(ExecuteScalar("Apz_D_SaveEventListenerApp", CommandType.StoredProcedure)) <= 0)
				{
					return new HttpResult(Enums.MessageCode.Error, 810037 + " - Save Notification failed!");
				}
				break;
			}
			case "MarkAllAsRead":
			{
				List<SqlParameter> list2 = new List<SqlParameter>();
				list2.Add(new SqlParameter("@userName", Function.ToString(dt.Rows[0][0])));
				list2.Add(new SqlParameter("@appName", Function.ToString(dt.Rows[0][1])));
				list2.Add(new SqlParameter("@dbName", Function.ToString(dt.Rows[0][2])));
				Function.ParseInt(ExecuteNonQuery("APZ_D_MarkAllNotiAsRead", CommandType.StoredProcedure, list2.ToArray()));
				break;
			}
			case "UpdateNotificationEvent":
			{
				DataTable dataTable3 = new DataTable();
				CreateTempTable(dt, "#tempTable");
				BulkCopy(dt, "#tempTable");
				List<SqlParameter> list3 = new List<SqlParameter>
				{
					new SqlParameter
					{
						ParameterName = "@type",
						Value = "update"
					}
				};
				dataTable3 = ExecuteDataTable("APZ_D_NotificationEvent_Update", CommandType.StoredProcedure, list3.ToArray());
				string message2 = "Not Found";
				if (dataTable3.IsNotNull())
				{
					message2 = Function.ToString(dataTable3.Rows[0][0]);
				}
				return new HttpResult(Enums.MessageCode.Error, message2);
			}
			case "AddNotificationEvent":
			{
				DataTable dataTable = new DataTable();
				CreateTempTable(dt, "#tempTable");
				BulkCopy(dt, "#tempTable");
				List<SqlParameter> list = new List<SqlParameter>
				{
					new SqlParameter
					{
						ParameterName = "@type",
						Value = "add"
					}
				};
				dataTable = ExecuteDataTable("APZ_D_NotificationEvent_Update", CommandType.StoredProcedure, list.ToArray());
				string message = "Not Found";
				if (dataTable.IsNotNull())
				{
					message = Function.ToString(dataTable.Rows[0][0]);
				}
				return new HttpResult(Enums.MessageCode.Error, message);
			}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
