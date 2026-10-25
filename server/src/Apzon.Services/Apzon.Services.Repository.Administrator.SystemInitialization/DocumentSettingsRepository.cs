using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Services.Interfaces.Administrator.SystemInitialization;

namespace Apzon.Services.Repository.Administrator.SystemInitialization;

public class DocumentSettingsRepository : BaseService<IDocumentSettings>, IDocumentSettings
{
	public DocumentSettingsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataSet LoadDoc()
	{
		try
		{
			string query = "select * from OADM;\r\n\t                          select \"BlockZeroQ\", \"NegStoLv\", \"ChkQunty\" from CINF;\r\n\t                          select * from ADP1;\r\n\t                          select \"ObjList\" from OADP;";
			DataSet dataSet = ExecuteData(query, CommandType.Text);
			if (dataSet.Tables.Count > 0)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadDoc  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	public string Save(DataSet ds)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				DataTable dataTable = ds.Tables[0];
				DataTable dataTable2 = ds.Tables[1];
				DataTable dt3 = ds.Tables[2];
				DataTable dataTable3 = ds.Tables[3];
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@AllowFuPos", Function.ToString(dataTable.Rows[0]["AllowFuPos"])));
				list.Add(new SqlParameter("@OrderBlock", Function.ToString(dataTable.Rows[0]["OrderBlock"])));
				list.Add(new SqlParameter("@LevelWarn", Function.ToString(dataTable.Rows[0]["LevelWarn"])));
				list.Add(new SqlParameter("@ChangeRdr", Function.ToString(dataTable.Rows[0]["ChangeRdr"])));
				list.Add(new SqlParameter("@RevisionPo", Function.ToString(dataTable.Rows[0]["RevisionPo"])));
				list.Add(new SqlParameter("@ICDifExPe1", Function.ParseDouble(dataTable.Rows[0]["ICDifExPe1"])));
				list.Add(new SqlParameter("@ICDifExPe3", Function.ParseDouble(dataTable.Rows[0]["ICDifExPe3"])));
				list.Add(new SqlParameter("@PostDiffR", Function.ParseDouble(dataTable.Rows[0]["PostDiffR"])));
				list.Add(new SqlParameter("@ClsZoDiffR", Function.ToString(dataTable.Rows[0]["ClsZoDiffR"])));
				list.Add(new SqlParameter("@ClsNoConfi", Function.ToString(dataTable.Rows[0]["ClsNoConfi"])));
				list.Add(new SqlParameter("@AllowPostZ", Function.ToString(dataTable.Rows[0]["AllowPostZ"])));
				list.Add(new SqlParameter("@ClosedQuot", Function.ToString(dataTable.Rows[0]["ClosedQuot"])));
				list.Add(new SqlParameter("@NegStoLv", Function.ToString(dataTable2.Rows[0]["NegStoLv"])));
				list.Add(new SqlParameter("@BlockZeroQ", Function.ToString(dataTable2.Rows[0]["BlockZeroQ"])));
				list.Add(new SqlParameter("@ChkQunty", Function.ToString(dataTable2.Rows[0]["ChkQunty"])));
				DataRow dataRow = (from p in dt3.AsEnumerable()
					where p.Field<string>(dt3.Columns["ObjType"]) == "23"
					select p).FirstOrDefault();
				list.Add(new SqlParameter("@AllowFuPosSale", Function.ToString(dataRow["AllowFuPos"])));
				dataRow = (from p in dt3.AsEnumerable()
					where p.Field<string>(dt3.Columns["ObjType"]) == "17"
					select p).FirstOrDefault();
				list.Add(new SqlParameter("@ReopOrderSale", Function.ToString(dataRow["ReopOrder"])));
				list.Add(new SqlParameter("@ForceReOrdSale", Function.ToString(dataRow["ForceReOrd"])));
				dataRow = (from p in dt3.AsEnumerable()
					where p.Field<string>(dt3.Columns["ObjType"]) == "1470000113"
					select p).FirstOrDefault();
				list.Add(new SqlParameter("@CpyPRPrice", Function.ToString(dataRow["CpyPRPrice"])));
				dataRow = (from p in dt3.AsEnumerable()
					where p.Field<string>(dt3.Columns["ObjType"]) == "540000006"
					select p).FirstOrDefault();
				list.Add(new SqlParameter("@ClosePQ", Function.ToString(dataRow["ClosePQ"])));
				dataRow = (from p in dt3.AsEnumerable()
					where p.Field<string>(dt3.Columns["ObjType"]) == "22"
					select p).FirstOrDefault();
				list.Add(new SqlParameter("@ReopOrderPurchase", Function.ToString(dataRow["ReopOrder"])));
				list.Add(new SqlParameter("@ForceReOrdPurchase", Function.ToString(dataRow["ForceReOrd"])));
				list.Add(new SqlParameter("@ObjList", Function.ParseInt(dataTable3.Rows[0]["ObjList"])));
				if (ExecuteNonQuery("APZ_DocumentSettings_SaveDoc", CommandType.StoredProcedure, list.ToArray()) <= 0)
				{
					return "Error!";
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

	public DataTable GetDataSettingByObjectType(ApzObjectTypes objectType)
	{
		try
		{
			return ExecuteDataTable("select * from ADP1 where \"ObjType\" = @objType;", CommandType.Text, new SqlParameter("@objType", objectType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDataSettingByObjectType  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
