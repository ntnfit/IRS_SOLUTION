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

public class ChannelRepository : BaseService<DataSet>, IChannel
{
	public ChannelRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDataDoc(string channelCode, string channelName, string status)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@chnCode", Function.ToString(channelCode)));
			list.Add(new SqlParameter("@chnName", Function.ToString(channelName)));
			list.Add(new SqlParameter("@status", Function.ToString(status)));
			ExecuteDataTable("APZ_D_Channel_GetData", CommandType.StoredProcedure, list.ToArray());
			return ExecuteDataTable("APZ_D_Channel_GetData", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetRecord(string position, string channelCode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@position", position));
			list.Add(new SqlParameter("@channelCode", channelCode));
			return ExecuteDataTable("APZ_D_Channel_GetRecord", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataLine(string channelCode)
	{
		try
		{
			return ExecuteDataTable("select a.*, b.\"ItemName\", b.\"ItemName\" as \"ItemDescrition\", b.\"FrgnName\"\r\n                                    from APZ_D_CHN1 a\r\n                                    inner join OITM b on a.\"ItemCode\" = b.\"ItemCode\"\r\n                                    where a.\"ChannelCode\" = @channelCode;", CommandType.Text, new SqlParameter("@channelCode", Function.ToString(channelCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public HttpResult SaveData(DataSet ds, string type)
	{
		try
		{
			if (type == "A")
			{
				return Process(ApzObjectTypes.oChannel, -1, Enums.TransactionType.A, ds);
			}
			return Process(ApzObjectTypes.oChannel, -1, Enums.TransactionType.U, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		try
		{
			CreateTempTable(document.Tables[0], "#APZ_D_OCHN");
			BulkCopy(document.Tables[0], "#APZ_D_OCHN");
			CreateTempTable(document.Tables[1], "#APZ_D_CHN1");
			BulkCopy(document.Tables[1], "#APZ_D_CHN1");
			DataTable dataTable = ExecuteDataTable("APZ_D_Process_Channel", CommandType.StoredProcedure, new SqlParameter("@type", (transtype == Enums.TransactionType.A) ? "A" : "U"));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return new HttpResult(Enums.MessageCode.Error, 20600003 + " - " + Function.ToString(dataTable.Rows[0]["ErrorMessage"]));
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public HttpResult Remove(string channelCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("APZ_D_Channel_Remove", CommandType.StoredProcedure, new SqlParameter("@channelCode", Function.ToString(channelCode)));
			if (dataTable.IsNotNull() && !string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][0])))
			{
				return new HttpResult(Enums.MessageCode.Error, 20600003 + " - " + Function.ToString(dataTable.Rows[0][0]));
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
