using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.SystemInitialization;

namespace Apzon.Services.Repository.Administrator.SystemInitialization;

public class DocumentNumberingRepository : BaseService<DataSet>, IDocumentNumbering
{
	public DocumentNumberingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDataSeries(int userId)
	{
		try
		{
			string query = "SELECT T0.*,CASE WHEN T0.\"Series\" = (SELECT MIN(\"Series\") FROM NNM2 WHERE \"ObjectCode\" = T0.\"ObjectCode\" and \"DocSubType\" = T0.\"DocSubType\" and \"UserSign\" = @userId) THEN 'Y' ELSE 'N' END \"IsDflt\" FROM NNM1 T0 INNER JOIN ONNM T1 ON t0.\"ObjectCode\" = T1.\"ObjectCode\" AND T1.\"DocSubType\" = T0.\"DocSubType\";";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@userId", userId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetLoadDocQuery(IEnumerable<string> lstDocNumber, int userId)
	{
		return "SELECT a.ObjectCode as ObjectCode, a.ObjectCode + ISNULL(NULLIF(a.DocSubType,'--'),'') Code, SeriesName,InitialNum,Series,NextNumber,A.DocSubType,LastNum,DocAlias\r\n                    ,case a.ObjectCode when '2' then 'B' when '4' then 'I' when '290' then 'R' else 'D' end SeriesType \r\n                    FROM dbo.ONNM a\r\n                    INNER JOIN dbo.NNM1 b ON a.ObjectCode = b.ObjectCode AND b.DocSubType = a.DocSubType AND b.Series = ISNULL((select top 1 Series from dbo.NNM2 where ObjectCode = a.ObjectCode and DocSubType = a.DocSubType and UserSign = @userId), a.DfltSeries)\r\n                    WHERE a.ObjectCode +ISNULL(NULLIF(a.DocSubType,'--'),'') in (" + string.Join(",", lstDocNumber.ToArray()) + ") ORDER BY try_cast(a.ObjectCode as int) ASC";
	}

	public DataTable LoadDoc(int userId)
	{
		try
		{
			IEnumerable<string> lstDocNumber = CommonList.oDocumentNumberingCode.Select((KeyValuePair<string, string> t) => "'" + t.Key + "'");
			string loadDocQuery = GetLoadDocQuery(lstDocNumber, userId);
			DataTable dataTable = ExecuteDataTable(loadDocQuery, CommandType.Text, new SqlParameter("@userId", userId));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataSet dt)
	{
		return Process(ApzObjectTypes.oDocumentNumbering, -1, Enums.TransactionType.U, dt);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		if (transtype == Enums.TransactionType.U)
		{
			if (!document.IsNotNull())
			{
				return new HttpResult(Enums.MessageCode.Error, "Data is empty");
			}
			if (!document.Tables.Contains("ONNM") || !document.Tables.Contains("NNM1"))
			{
				return new HttpResult(Enums.MessageCode.Error, "Data is empty");
			}
			DataTable dt = document.Tables["ONNM"];
			DataTable dt2 = document.Tables["NNM1"];
			CreateTempTable(dt, "#ONNM");
			BulkCopy(dt, "#ONNM");
			CreateTempTable(dt2, "#NNM1");
			BulkCopy(dt2, "#NNM1");
			DataTable dataTable = ExecuteDataTable("apz_documentnumbering_process", CommandType.StoredProcedure);
			if (dataTable.IsNotNull())
			{
				int num = Function.ParseInt(dataTable.Rows[0][0]);
				if (num > 0)
				{
					string text = Function.ToString(dataTable.Rows[0][1]);
					throw new Exception(num + "-" + text);
				}
			}
			return new HttpResult();
		}
		return new HttpResult(Enums.MessageCode.Error, "Unknown Error");
	}
}
