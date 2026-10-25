using System;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class ContextActionRepository : BaseService<DataSet>, IContextAction
{
	private const string TempTableDoc = "#APZ_OCTA";

	private const string TempTableParams = "#APZ_CTA1";

	private const string TempTableMapping = "#APZ_CTA2";

	private const string TempTableAtt = "#APZ_CTA3";

	private const string TempTableAttPr = "#APZ_CTA4";

	private const string TempTableHttpRequestConfig = "#APZ_CTA5";

	private const string TableDoc = "APZ_OCTA";

	private const string TableParams = "APZ_CTA1";

	private const string TableMapping = "APZ_CTA2";

	private const string TableAtt = "APZ_CTA3";

	private const string TableAttPr = "APZ_CTA4";

	private const string TableHttpRequestConfig = "APZ_CTA5";

	private string _formId;

	private string _itemId;

	public ContextActionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetListContextAction(string formId, string itemId)
	{
		try
		{
			if (!string.IsNullOrEmpty(formId))
			{
				return ExecuteDataTable("SELECT * FROM APZ_OCTA WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId;", CommandType.Text, new SqlParameter("@FormId", formId), new SqlParameter("@ItemId", itemId ?? ""));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetListParams(string formId, string itemId)
	{
		try
		{
			if (!string.IsNullOrEmpty(formId))
			{
				return ExecuteDataTable("SELECT * FROM APZ_CTA1 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId;", CommandType.Text, new SqlParameter("@FormId", formId), new SqlParameter("@ItemId", itemId ?? ""));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataSet GetData(string formId, string itemId)
	{
		try
		{
			if (!string.IsNullOrEmpty(formId))
			{
				return ExecuteData("SELECT * FROM APZ_OCTA WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId;\r\n                                        SELECT * FROM APZ_CTA1 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId;\r\n                                        SELECT * FROM APZ_CTA2 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId;\r\n                                        SELECT * FROM APZ_CTA3 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId;\r\n                                        SELECT * FROM APZ_CTA4 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId;\r\n                                        SELECT * FROM APZ_CTA5 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId;", CommandType.Text, new SqlParameter("@FormId", formId), new SqlParameter("@ItemId", itemId ?? ""));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataSet GetDataByEntry(int absEntry, string formId, string itemId)
	{
		try
		{
			return ExecuteData("   SELECT * FROM APZ_OCTA WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId and \"AbsEntry\"=@AbsEntry;\r\n                                        SELECT * FROM APZ_CTA1 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId and \"AbsEntry\"=@AbsEntry;\r\n                                        SELECT * FROM APZ_CTA2 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId and \"AbsEntry\"=@AbsEntry;\r\n                                        SELECT * FROM APZ_CTA3 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId and \"AbsEntry\"=@AbsEntry;\r\n                                        SELECT * FROM APZ_CTA4 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId and \"AbsEntry\"=@AbsEntry;\r\n                                        SELECT * FROM APZ_CTA5 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId and \"AbsEntry\"=@AbsEntry;", CommandType.Text, new SqlParameter("@FormId", formId), new SqlParameter("@ItemId", itemId), new SqlParameter("@AbsEntry", absEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataSet();
	}

	public DataTable GetListMapping(string formId, string itemId)
	{
		try
		{
			if (!string.IsNullOrEmpty(formId))
			{
				return ExecuteDataTable("SELECT * FROM APZ_CTA2 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId;", CommandType.Text, new SqlParameter("@FormId", formId), new SqlParameter("@ItemId", itemId ?? ""));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult Save(string formId, string itemId, DataSet ds)
	{
		_formId = formId;
		_itemId = itemId;
		return Process(ApzObjectTypes.oContextAction, -1, Enums.TransactionType.U, ds);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet document, ref string objId, ref string objNum)
	{
		switch (transtype)
		{
		case Enums.TransactionType.A:
		case Enums.TransactionType.U:
			return SaveData(document, ref transtype);
		default:
			return new HttpResult(Enums.MessageCode.Error, "Unknown error");
		}
	}

	private HttpResult SaveData(DataSet doc, ref Enums.TransactionType transtype)
	{
		if (doc.IsNotNull())
		{
			transtype = Enums.TransactionType.U;
			BulkToTempTable(doc.Tables["APZ_OCTA"], "APZ_OCTA", "#APZ_OCTA");
			BulkToTempTable(doc.Tables["APZ_CTA1"], "APZ_CTA1", "#APZ_CTA1");
			BulkToTempTable(doc.Tables["APZ_CTA2"], "APZ_CTA2", "#APZ_CTA2");
			BulkToTempTable(doc.Tables["APZ_CTA3"], "APZ_CTA3", "#APZ_CTA3");
			BulkToTempTable(doc.Tables["APZ_CTA4"], "APZ_CTA4", "#APZ_CTA4");
			BulkToTempTable(doc.Tables["APZ_CTA5"], "APZ_CTA5", "#APZ_CTA5");
			string query = "apz_contextaction_process";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@transtype", transtype.ToString()), new SqlParameter("@formId", _formId), new SqlParameter("@itemId", _itemId));
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
		return new HttpResult(Enums.MessageCode.Error, "Data is not valid");
	}

	public DataTable GetMappingUdo(string udocode, string actionFormId, string actionItemId, int absEntry, string sonname)
	{
		try
		{
			if (!string.IsNullOrEmpty(sonname))
			{
				string query = "   SELECT  @AbsEntry \"AbsEntry\", @FormId \"FormId\", @ItemId \"ItemId\",'O' \"MappingType\",T2.\"SonName\" \"TargetItem\",T0.\"ColAlias\" \"TargetField\",t1.\"SourceField\",T1.\"DfltValue\",T1.\"SumType\" FROM UDO4 T0\r\n\t\t\t\tLEFT JOIN apz_CTA2 T1 ON T1.\"MappingType\"='O' AND @SonName=T1.\"TargetItem\" AND t0.\"ColAlias\"=t1.\"TargetField\"  AND T1.\"FormId\"=@FormId AND T1.\"ItemId\"=@ItemId AND T1.\"AbsEntry\"=@AbsEntry\r\n                INNER JOIN UDO1 T2 ON T0.\"Code\"=T2.\"Code\" AND T0.\"SonNum\"=T2.\"SonNum\" AND T2.\"SonName\"=@SonName\r\n                        AND T0.\"Code\" = @UdoCode\r\n                        AND T0.\"ColIsUsed\" = 'Y'\r\n\t\t\t\t\t\tAND T0.\"ColEdit\"='Y' Order by T0.\"ColumnNum\";";
				return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@UdoCode", udocode), new SqlParameter("@AbsEntry", absEntry), new SqlParameter("@FormId", actionFormId), new SqlParameter("@ItemId", actionItemId), new SqlParameter("@SonName", sonname));
			}
			string query2 = "   SELECT  @AbsEntry \"AbsEntry\", @FormId \"FormId\", @ItemId \"ItemId\",'O' \"MappingType\",T0.\"Code\" \"TargetItem\",T0.\"ColAlias\" \"TargetField\",t1.\"SourceField\",T1.\"DfltValue\",T1.\"SumType\"\r\n                                  FROM UDO3 T0  \r\n                                  left JOIN apz_CTA2 T1 ON T1.\"MappingType\"='O' AND t0.\"Code\"=T1.\"TargetItem\" AND t0.\"ColAlias\"=t1.\"TargetField\" AND T1.\"FormId\"=@FormId AND T1.\"ItemId\"=@ItemId AND T1.\"AbsEntry\"=@AbsEntry\r\n                                  WHERE T0.\"Code\" = @UdoCode\r\n\t\t\t\t\t\t                               AND T0.\"ColEdit\"='Y'  \r\n                                                    ORDER BY T0.\"ColumnNum\";";
			return ExecuteDataTable(query2, CommandType.Text, new SqlParameter("@UdoCode", udocode), new SqlParameter("@AbsEntry", absEntry), new SqlParameter("@FormId", actionFormId), new SqlParameter("@ItemId", actionItemId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public virtual string GetTargetFieldQuery(string sonName)
	{
		try
		{
			string empty = string.Empty;
			return string.IsNullOrEmpty(sonName) ? "SELECT T0.\"ColAlias\"  \"Code\", T0.\"ColDesc\" \"Name\", T0.ColumnNum FROM UDO3 T0 WHERE T0.\"Code\" = @UdoCode AND T0.\"ColEdit\" = 'Y' \r\n                            UNION\r\n                            SELECT 'DocEntry', 'DocEntry', -1\r\n                            ORDER BY T0.\"ColumnNum\";" : "SELECT   T0.\"ColAlias\" \"Code\",T0.\"ColDesc\" \"Name\"  \r\n                            FROM UDO4 T0\r\n                            INNER JOIN UDO1 T2 ON T0.\"Code\" = T2.\"Code\" AND T0.\"SonNum\" = T2.\"SonNum\" AND T2.\"SonName\" = @SonName\r\n                            AND T0.\"Code\" = @UdoCode\r\n                            AND T0.\"ColIsUsed\" = 'Y' AND T0.\"ColEdit\" = 'Y' Order by T0.\"ColumnNum\";";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public DataTable GetTargetField(string udocode, string sonname)
	{
		try
		{
			string targetFieldQuery = GetTargetFieldQuery(sonname);
			if (!string.IsNullOrEmpty(sonname))
			{
				return ExecuteDataTable(targetFieldQuery, CommandType.Text, new SqlParameter("@UdoCode", udocode), new SqlParameter("@SonName", sonname));
			}
			return ExecuteDataTable(targetFieldQuery, CommandType.Text, new SqlParameter("@UdoCode", udocode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetListAttachment(string formId, string itemId)
	{
		try
		{
			if (!string.IsNullOrEmpty(formId))
			{
				return ExecuteDataTable("SELECT * FROM APZ_CTA3 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId;", CommandType.Text, new SqlParameter("@FormId", formId), new SqlParameter("@ItemId", itemId ?? ""));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetListAttachParams(string formId, string itemId)
	{
		try
		{
			if (!string.IsNullOrEmpty(formId))
			{
				return ExecuteDataTable("SELECT * FROM APZ_CTA4 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId;", CommandType.Text, new SqlParameter("@FormId", formId), new SqlParameter("@ItemId", itemId ?? ""));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public HttpResult SavePoint(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string query = "UPDATE APZ_OCTA SET \"LocationX\"=@LocationX,\"LocationY\"=@LocationY,\"Anchor\"=@Anchor,\"LocationLeft\"=@LocationLeft,\"LocationTop\"=@LocationTop,\"LocationRight\"=@LocationRight,\"LocationBottom\"=@LocationBottom\r\n                                WHERE \"AbsEntry\"=@AbsEntry AND \"FormId\"=@FormId  AND \"ItemId\"=@ItemId;";
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@LocationX", Function.ParseInt(dt.Rows[i]["LocationX"])), new SqlParameter("@LocationY", Function.ParseInt(dt.Rows[i]["LocationY"])), new SqlParameter("@LocationLeft", Function.ParseInt(dt.Rows[i]["LocationLeft"])), new SqlParameter("@LocationTop", Function.ParseInt(dt.Rows[i]["LocationTop"])), new SqlParameter("@LocationRight", Function.ParseInt(dt.Rows[i]["LocationRight"])), new SqlParameter("@LocationBottom", Function.ParseInt(dt.Rows[i]["LocationBottom"])), new SqlParameter("@Anchor", Function.ToString(dt.Rows[i]["Anchor"])), new SqlParameter("@AbsEntry", Function.ParseInt(dt.Rows[i]["AbsEntry"])), new SqlParameter("@FormId", Function.ToString(dt.Rows[i]["FormId"])), new SqlParameter("@ItemId", Function.ToString(dt.Rows[i]["ItemId"])));
				}
				dbTransaction.Commit();
				return new HttpResult();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataTable GetProcessGroup(int absEntry, string formId, string itemId)
	{
		try
		{
			string processGroupQuery = GetProcessGroupQuery();
			return ExecuteDataTable(processGroupQuery, CommandType.Text, new SqlParameter("@FormId", formId), new SqlParameter("@ItemId", itemId), new SqlParameter("@AbsEntry", absEntry));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new DataTable();
		}
	}

	public virtual string GetProcessGroupQuery()
	{
		return "SELECT * INTO #ret FROM dbo.APZ_OCTA  where FormId=@FormId AND ItemId=@ItemId AND AbsEntry=@AbsEntry\r\n                                    SELECT  * INTO #tbl FROM dbo.APZ_OCTA  where FormId=@FormId AND ItemId=@ItemId AND AbsEntry<>@AbsEntry\r\n                                    DECLARE @i INT = 0\r\n                                    DECLARE @n int=ISNULL( ( SELECT COUNT(1) FROM #tbl),0)\r\n                                    WHILE (@i<@n)\r\n                                    BEGIN\r\n\t \r\n\t                                        INSERT INTO #ret \r\n\t                                        SELECT * FROM #tbl WHERE FatherId IN (SELECT AbsEntry FROM #ret) \r\n\t                                        DELETE #tbl WHERE AbsEntry IN (SELECT AbsEntry FROM #ret)\r\n\t                                        set @n  =ISNULL( ( SELECT COUNT(1) FROM #tbl),0)\r\n\t                                        SET @i=@i+1\r\n                                    END\r\n                                    SELECT * FROM #ret";
	}

	public DataTable GetListHttpRequestConfig(string formId, string itemId)
	{
		try
		{
			if (!string.IsNullOrEmpty(formId))
			{
				return ExecuteDataTable("SELECT * FROM APZ_CTA5 WHERE \"FormId\"=@FormId AND \"ItemId\"=@ItemId;", CommandType.Text, new SqlParameter("@FormId", formId), new SqlParameter("@ItemId", itemId ?? ""));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public virtual DataTable GetHeaderByAlias(string alias, string formId, string itemId)
	{
		try
		{
			if (!string.IsNullOrEmpty(alias))
			{
				return ExecuteDataTable("SELECT * FROM APZ_OCTA WHERE \"UniqueAlias\" =@Alias and \"FormId\" = @formId \r\nAND (@formId <> 'oUserDefinedReports' or (@formId = 'oUserDefinedReports' and (\"ItemId\" = @itemId or \"ItemId\" = 'B$' + @itemId)));", CommandType.Text, new SqlParameter("@Alias", alias), new SqlParameter("@formId", formId), new SqlParameter("@itemId", itemId));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public DataTable GetColumnsFromPhysicalTableName(string physicalTableName)
	{
		try
		{
			if (!string.IsNullOrEmpty(physicalTableName))
			{
				string query = QueryGetColumnsFromPhysicalTableName();
				return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@TableName", physicalTableName));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public virtual string QueryGetColumnsFromPhysicalTableName()
	{
		return " select COLUMN_NAME COLUMN_NAME,DATA_TYPE DATA_TYPE_NAME,CHARACTER_MAXIMUM_LENGTH LENGTH from INFORMATION_SCHEMA.columns where TABLE_NAME=@TableName";
	}

	public HttpResult SavePhysicalData(string tableName, DataSet dsAction, DataTable source)
	{
		try
		{
			if (source == null)
			{
				return new HttpResult(Enums.MessageCode.Error, "source is empty");
			}
			if (string.IsNullOrEmpty(tableName))
			{
				return new HttpResult(Enums.MessageCode.Error, "Table name is invalid - empty");
			}
			if (dsAction.Tables.Count >= 3)
			{
				DataTable dataTable = dsAction.Tables[2];
				if (!dataTable.IsNotNull())
				{
					return new HttpResult(Enums.MessageCode.Error, "Could not get data mapping - empty");
				}
				ContextActionMappingImpl[] array = (from t in dataTable.AsEnumerable()
					select new ContextActionMappingImpl(t)).ToArray();
				DataTable dataTable2 = new DataTable();
				for (int num = 0; num < array.Length; num++)
				{
					if (source.Columns[array[num].SourceField] == null)
					{
						Logging.Write("ERROR", "Column does not exist " + array[num].SourceField);
					}
					Type dataType = source.Columns[array[num].SourceField].DataType;
					dataTable2.Columns.Add(array[num].TargetField, dataType);
				}
				if (!dataTable2.IsExistsColumn("CREATE_DATE"))
				{
					dataTable2.Columns.Add("CREATE_DATE", typeof(DateTime));
				}
				if (!dataTable2.IsExistsColumn("CREATE_TIME"))
				{
					dataTable2.Columns.Add("CREATE_TIME", typeof(int));
				}
				DateTime today = DateTime.Today;
				string value = DateTime.Now.ToString("HHmmss");
				for (int num2 = 0; num2 < source.Rows.Count; num2++)
				{
					DataRow dataRow = dataTable2.NewRow();
					for (int num3 = 0; num3 < array.Length; num3++)
					{
						dataRow[array[num3].TargetField] = source.Rows[num2][array[num3].SourceField];
					}
					dataRow["CREATE_DATE"] = today;
					dataRow["CREATE_TIME"] = value;
					dataTable2.Rows.Add(dataRow);
				}
				dataTable2.AcceptChanges();
				using (DatabaseService.OpenConnection())
				{
					BulkToTempTable(dataTable2, tableName, "#TEMP_" + tableName);
					string query = QuerySavePhysicalData(tableName);
					ExecuteNonQuery(query, CommandType.Text);
				}
				return new HttpResult();
			}
			return new HttpResult(Enums.MessageCode.Error, "Could not get data mapping");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual string QuerySavePhysicalData(string tableName)
	{
		return "insert into " + tableName + " select * from #TEMP_" + tableName;
	}
}
