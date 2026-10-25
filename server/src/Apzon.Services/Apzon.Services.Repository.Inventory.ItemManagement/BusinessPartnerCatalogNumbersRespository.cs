using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Inventory.ItemManagement;

namespace Apzon.Services.Repository.Inventory.ItemManagement;

public class BusinessPartnerCatalogNumbersRespository : BaseService<DataTable>, IBusinessPartnerCatalogNumbers
{
	public BusinessPartnerCatalogNumbersRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetItemByCardCode(string cardcode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@cardcode", cardcode);
			return ExecuteDataTable("APZ_BusinessPartnerCatalogNumbers_GetItemByCardCode", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetItemByCardCode  ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBPByItemCode(string itemcode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@itemcode", itemcode);
			return ExecuteDataTable("APZ_BusinessPartnerCatalogNumbers_GetBPByItemCode", CommandType.StoredProcedure, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetItemByCardCode  ", ex.Message);
			return new DataTable();
		}
	}

	public HttpResult Save(DataTable dt, ApzObjectTypes type)
	{
		try
		{
			return Process(type, -1, Enums.TransactionType.A, dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes type, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		DataTable tableStruct = GetTableStruct("OSCN");
		if (!tableStruct.IsNotNull())
		{
			throw new Exception("Could not get data structure");
		}
		IEnumerable<string> source = from DataColumn t in tableStruct.Columns
			select t.ColumnName;
		dt = dt.DefaultView.ToTable(distinct: false, source.ToArray());
		CreateTempTable(dt, "#TempCateNum");
		BulkCopy(dt, "#TempCateNum");
		SqlParameter[] array = new SqlParameter[2];
		string value;
		if (type == ApzObjectTypes.oBusinessPartners)
		{
			array[0] = new SqlParameter("@type", 1);
			value = Function.ToString(dt.Rows[0]["CardCode"]);
		}
		else
		{
			array[0] = new SqlParameter("@type", 2);
			value = Function.ToString(dt.Rows[0]["ItemCode"]);
		}
		array[1] = new SqlParameter("@paramater", value);
		if (ExecuteNonQuery("APZ_BusinessPartnerCatalogNumbers_Save", CommandType.StoredProcedure, array) > 0)
		{
			return new HttpResult(Enums.MessageCode.Success);
		}
		return new HttpResult(Enums.MessageCode.Error, "Could not save data");
	}

	public string Remove(string code, string subtitle, string item, string type)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				SqlParameter[] parameters = new SqlParameter[3]
				{
					new SqlParameter("@Substitute", subtitle),
					new SqlParameter("@CarCode", code),
					new SqlParameter("@ItemCode", item)
				};
				string text = "delete from oscn where ";
				text = ((!(type == "TabBP")) ? (text + " Substitute=@Substitute and CardCode=@CarCode and ItemCode=@ItemCode") : (text + "ItemCode = @ItemCode"));
				if (ExecuteNonQuery(text, CommandType.Text, parameters) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				dbTransaction.Rollback();
				return "remove Error";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return "error!";
		}
	}

	public DataTable GetData(Enums.NavigateType type, Enums.BusinessPartnerCatalogNumbersTableType objectType, string par)
	{
		try
		{
			string dataQuery = GetDataQuery(type, objectType);
			SqlParameter sqlParameter = new SqlParameter("@parm", Function.ToString(par));
			DataSet dataSet = ExecuteData(dataQuery, CommandType.Text, sqlParameter);
			if (dataSet != null && dataSet.Tables.Count > 0)
			{
				if (dataSet.Tables[0] == null || dataSet.Tables[0].Rows.Count == 0)
				{
					return dataSet.Tables[1];
				}
				return dataSet.Tables[0];
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetDataQuery(Enums.NavigateType type, Enums.BusinessPartnerCatalogNumbersTableType objectType)
	{
		switch (type)
		{
		case Enums.NavigateType.Previous:
			switch (objectType)
			{
			case Enums.BusinessPartnerCatalogNumbersTableType.TabBP:
				return "select a.*, b.CardName, c.ItemName   \r\n\t                            from oscn a inner join ocrd b on a.CardCode = b.CardCode inner join OITM c on a.ItemCode = c.ItemCode  \r\n\t                            where a.CardCode = isnull( (select top 1 CardCode from oscn where CardCode < @parm order by CardCode DESC), '' )\r\n                                select a.CardCode, b.CardName, a.ItemCode, c.ItemName, a.Substitute, a.ShowSCN   \r\n\t                            from oscn a inner join ocrd b on a.CardCode = b.CardCode inner join OITM c on a.ItemCode = c.ItemCode  \r\n                                where a.CardCode = isnull( (select top 1 CardCode from oscn  order by CardCode DESC), '' )";
			case Enums.BusinessPartnerCatalogNumbersTableType.TabItem:
				return "select top 1 a.*, b.CardName, c.ItemName\r\n\t                            from oscn a inner join ocrd b on a.CardCode = b.CardCode inner join OITM c on a.ItemCode = c.ItemCode  \r\n\t                            where a.ItemCode = isnull( (select top 1 ItemCode from oscn where ItemCode < @parm order by ItemCode DESC), '' )\r\n                                select a.CardCode, b.CardName, a.ItemCode, c.ItemName, a.Substitute, a.ShowSCN   \r\n\t                            from oscn a inner join ocrd b on a.CardCode = b.CardCode inner join OITM c on a.ItemCode = c.ItemCode  \r\n                                where a.ItemCode = isnull( (select top 1 ItemCode from oscn order by ItemCode DESC), '' )";
			}
			break;
		case Enums.NavigateType.Next:
			switch (objectType)
			{
			case Enums.BusinessPartnerCatalogNumbersTableType.TabBP:
				return "select a.*, b.CardName, c.ItemName \r\n\t                            from oscn a inner join ocrd b on a.CardCode = b.CardCode inner join OITM c on a.ItemCode = c.ItemCode  \r\n\t                            where a.CardCode = isnull( (select top 1 CardCode from oscn where CardCode > @parm order by CardCode ASC), '' )\r\n                                select  a.CardCode, b.CardName, a.ItemCode, c.ItemName, a.Substitute, a.ShowSCN   \r\n\t                            from oscn a inner join ocrd b on a.CardCode = b.CardCode inner join OITM c on a.ItemCode = c.ItemCode  \r\n                                where a.CardCode = isnull( (select top 1 CardCode from oscn order by CardCode ASC), '' )";
			case Enums.BusinessPartnerCatalogNumbersTableType.TabItem:
				return "select a.*, b.CardName, c.ItemName \r\n\t                            from oscn a inner join ocrd b on a.CardCode = b.CardCode inner join OITM c on a.ItemCode = c.ItemCode  \r\n\t                            where a.ItemCode = isnull( (select top 1 ItemCode from oscn where ItemCode > @parm order by ItemCode ASC), '' )\r\n                                select a.CardCode, b.CardName, a.ItemCode, c.ItemName, a.Substitute, a.ShowSCN   \r\n\t                            from oscn a inner join ocrd b on a.CardCode = b.CardCode inner join OITM c on a.ItemCode = c.ItemCode  \r\n                                where a.ItemCode = isnull( (select top 1 ItemCode from oscn order by ItemCode ASC), '' )";
			}
			break;
		case Enums.NavigateType.Last:
			switch (objectType)
			{
			case Enums.BusinessPartnerCatalogNumbersTableType.TabBP:
				return "select a.*, b.CardName, c.ItemName\r\n\t                            from oscn a inner join ocrd b on a.CardCode = b.CardCode inner join OITM c on a.ItemCode = c.ItemCode  \r\n                                where a.CardCode = isnull((select top 1 CardCode from oscn order by CardCode DESC),'') ";
			case Enums.BusinessPartnerCatalogNumbersTableType.TabItem:
				return "select a.*, b.CardName, c.ItemName \r\n\t                            from oscn a inner join ocrd b on a.CardCode = b.CardCode inner join OITM c on a.ItemCode = c.ItemCode  \r\n\t                            where a.ItemCode = isnull((select top 1 ItemCode from oscn order by ItemCode DESC),'') ";
			}
			break;
		case Enums.NavigateType.First:
			switch (objectType)
			{
			case Enums.BusinessPartnerCatalogNumbersTableType.TabBP:
				return "select a.*, b.CardName, c.ItemName\r\n\t                            from oscn a inner join ocrd b on a.CardCode = b.CardCode inner join OITM c on a.ItemCode = c.ItemCode  \r\n\t                            where a.CardCode = isnull((select top 1 CardCode from oscn order by CardCode ASC),'') ";
			case Enums.BusinessPartnerCatalogNumbersTableType.TabItem:
				return "select a.*, b.CardName, c.ItemName\r\n\t                            from oscn a inner join ocrd b on a.CardCode = b.CardCode inner join OITM c on a.ItemCode = c.ItemCode  \r\n\t                            where a.ItemCode = isnull((select top 1 ItemCode from oscn order by ItemCode ASC),'') ";
			}
			break;
		}
		return "";
	}

	public string GetNameKey(string code, Enums.BusinessPartnerCatalogNumbersTableType type)
	{
		try
		{
			string text = "";
			text = ((type != Enums.BusinessPartnerCatalogNumbersTableType.TabBP) ? "select \"CardName\" as Name from ocrd where \"CardCode\" = @par;" : "select \"ItemName\" as \"Name\" from OITM where \"ItemCode\" = @par;");
			SqlParameter sqlParameter = new SqlParameter("@par", code);
			DataTable dataTable = ExecuteDataTable(text, CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0]["Name"]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return "";
		}
	}

	public DataTable GetBPByCardCode(string cardCode)
	{
		try
		{
			string query = "select * from oscn where  \"CardCode\"='" + cardCode + "';";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBPByCardCode  ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBPByCode(string catalogCode, string cardCode)
	{
		try
		{
			string query = "select * from oscn where \"Substitute\"='" + catalogCode + "' and \"CardCode\"='" + cardCode + "';";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetBPByCode  ", ex.Message);
			return new DataTable();
		}
	}

	public string GetCardNameByCode(string cardCode)
	{
		try
		{
			if (string.IsNullOrEmpty(cardCode))
			{
				return "";
			}
			DataTable dataTable = ExecuteDataTable("Select \"CardName\" from OCRD where \"CardCode\" = @cardCode;", CommandType.Text, new SqlParameter("@cardCode", cardCode));
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public string GetItemNameByCode(string itemName)
	{
		try
		{
			if (string.IsNullOrEmpty(itemName))
			{
				return "";
			}
			DataTable dataTable = ExecuteDataTable("Select \"ItemName\" from OITM where \"ItemCode\" = @itemName;", CommandType.Text, new SqlParameter("@itemName", itemName));
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return "";
	}

	public DataTable GetDataItemByBpCatalog(string subCatNum, string cardCode, string itemCode)
	{
		try
		{
			if (string.IsNullOrEmpty(subCatNum))
			{
				return new DataTable();
			}
			return ExecuteData("APZ_CFL_GetDataBpCatalogItem", CommandType.StoredProcedure, new SqlParameter("@subCatNum", subCatNum), new SqlParameter("@bpCode", cardCode), new SqlParameter("@itemCode", itemCode)).Tables[1];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public virtual DataTable GetExisted(DataTable model, ApzObjectTypes fatherObjectTypes)
	{
		try
		{
			if (fatherObjectTypes == ApzObjectTypes.oBusinessPartners)
			{
				return ExecuteDataTable("SELECT * FROM OSCN WHERE \"CardCode\"= @cardCode;", CommandType.Text, new SqlParameter("@cardCode", Function.ToString(model.Rows[0]["CardCode"])));
			}
			return ExecuteDataTable("SELECT * FROM OSCN WHERE \"ItemCode\"= @itemCode;", CommandType.Text, new SqlParameter("@itemCode", Function.ToString(model.Rows[0]["ItemCode"])));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return new DataTable();
	}

	public virtual string CheckExist(string code, string subtitle, string item, ApzObjectTypes objtype)
	{
		try
		{
			if (string.IsNullOrEmpty(code))
			{
				return "N";
			}
			switch (objtype)
			{
			case ApzObjectTypes.oBusinessPartners:
			{
				DataTable dataTable2 = ExecuteDataTable("SELECT *,Substitute OldSub FROM OSCN WHERE Substitute=@Substitute and CardCode=@CarCode and ItemCode=@ItemCode", CommandType.Text, new SqlParameter("@Substitute", subtitle), new SqlParameter("@CarCode", code), new SqlParameter("@ItemCode", item));
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					return "Y";
				}
				return "N";
			}
			case ApzObjectTypes.oItems:
			{
				DataTable dataTable = ExecuteDataTable("SELECT *,Substitute OldSub FROM OSCN WHERE ItemCode=@ItemCode", CommandType.Text, new SqlParameter("@ItemCode", item));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return "Y";
				}
				return "N";
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
		return "N";
	}

	public string GetSubCatNumByItemAndCardCode(string itemCode, string cardCode)
	{
		try
		{
			return Function.ToString(ExecuteScalar("select top 1 \"Substitute\" from OSCN where \"ItemCode\" = @itemCode and \"CardCode\" = @cardCode;", CommandType.Text, new SqlParameter("@cardCode", cardCode), new SqlParameter("@itemCode", itemCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return "";
		}
	}
}
