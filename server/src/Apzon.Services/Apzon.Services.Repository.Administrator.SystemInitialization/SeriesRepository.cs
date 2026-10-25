using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Administrator.SystemInitialization;

namespace Apzon.Services.Repository.Administrator.SystemInitialization;

public class SeriesRepository : BaseService<ISeries>, ISeries
{
	public SeriesRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable LoadDoc(string objCode, string docSubType, int userId)
	{
		try
		{
			return ExecuteDataTable(LoadDocQuery(), CommandType.Text, new SqlParameter("@objCode", objCode), new SqlParameter("@docSubType", docSubType), new SqlParameter("@userId", userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadDocQuery()
	{
		return "declare @dfltAllUser int = (select top 1 DfltSeries from ONNM where ObjectCode = @objCode and DocSubType = @docSubType)\r\n                declare @dflUser int = isnull((select top 1 Series from NNM2 where ObjectCode = @objCode and DocSubType = @docSubType and UserSign = @userId), @dfltAllUser)\r\n                select *, CASE when Series = @dflUser then 'Y' else 'N' end IsDflt from NNM1 where ObjectCode = @objCode and DocSubType = @docSubType";
	}

	public string Save(DataTable dtDoc)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				for (int i = 0; i < dtDoc.Rows.Count; i++)
				{
					List<SqlParameter> list = new List<SqlParameter>();
					list.Add(new SqlParameter("@InitialNum", Function.ParseInt(dtDoc.Rows[i]["InitialNum"])));
					list.Add(new SqlParameter("@NextNumber", Function.ParseInt(dtDoc.Rows[i]["NextNumber"])));
					list.Add(new SqlParameter("@ObjectCode", Function.ToString(dtDoc.Rows[i]["ObjectCode"])));
					list.Add(new SqlParameter("@DocSubType", Function.ToString(dtDoc.Rows[i]["DocSubType"])));
					list.Add(new SqlParameter("@Series", Function.ParseInt(dtDoc.Rows[i]["Series"])));
					list.Add(new SqlParameter("@LastNum", Function.ParseInt(dtDoc.Rows[i]["LastNum"])));
					list.Add(new SqlParameter("@Remark", Function.ToString(dtDoc.Rows[i]["Remark"])));
					list.Add(new SqlParameter("@GroupCode", Function.ParseInt(dtDoc.Rows[i]["GroupCode"])));
					list.Add(new SqlParameter("@Indicator", Function.ParseInt(dtDoc.Rows[i]["Indicator"])));
					list.Add(new SqlParameter("@Locked", Function.ToString(dtDoc.Rows[i]["Locked"])));
					list.Add(new SqlParameter("@IsForCncl", Function.ToString(dtDoc.Rows[i]["IsForCncl"])));
					if (ExecuteNonQuery("UPDATE dbo.NNM1 SET InitialNum=@InitialNum,NextNumber=@NextNumber,LastNum=@LastNum,Remark=@Remark,GroupCode=@GroupCode,Indicator=@Indicator,Locked= @Locked  WHERE ObjectCode = @ObjectCode AND DocSubType =@DocSubType AND Series=@Series", CommandType.Text, list.ToArray()) < 0)
					{
						return "Error";
					}
				}
				dbTransaction.Commit();
				return string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save  FAILED ", ex.Message);
			return ex.Message;
		}
	}
}
