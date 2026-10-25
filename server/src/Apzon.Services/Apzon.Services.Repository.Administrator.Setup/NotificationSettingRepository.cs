using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class NotificationSettingRepository : BaseService<DataTable>, INotificationSetting
{
	public NotificationSettingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetData(string user_code)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@usercode", user_code);
			DataTable dataTable = ExecuteDataTable(GetDataQuery(), CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetData  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetDataQuery()
	{
		return "select * from APZ_NOSE where UserCode=@usercode";
	}

	public string SaveData(NotificationSettingImpl setting)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@usercode", setting.user_code),
				new SqlParameter("@typeday", setting.typeDay),
				new SqlParameter("@fromday", setting.fromDay),
				new SqlParameter("@today", setting.toDay),
				new SqlParameter("@pickday", setting.pickDay),
				new SqlParameter("@typetime", setting.typeTime),
				new SqlParameter("@fromtime", setting.fromTime),
				new SqlParameter("@totime", setting.toTime),
				new SqlParameter("@specifynotification", setting.specifyNotification),
				new SqlParameter("@typespecify", setting.typeSpecify)
			};
			if (ExecuteNonQuery("APZ_Process_SaveNotiSetting", CommandType.StoredProcedure, list.ToArray()) == 0)
			{
				return "Failed";
			}
			return "Successful";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveData  FAILED ", ex.Message);
			return ex.Message;
		}
	}
}
