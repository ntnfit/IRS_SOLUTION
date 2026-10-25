using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Production_Addon;
using Apzon.Services.Interfaces.ProductionAddon;

namespace Apzon.Services.Repository.ProdutionAddon;

public class ApzBillOfMaterialsRepository : BaseService<ApzBillOfMaterials>, IApzBillOfMaterials
{
	public ApzBillOfMaterialsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable Search(ApzBillOfMaterials bom)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_BillOfMaterial_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@itemCode", bom.Code));
			list.Add(new SqlParameter("@itemName", bom.ItemName));
			list.Add(new SqlParameter("@status", bom.Status));
			list.Add(new SqlParameter("@costCenter", bom.OcrCode));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data BOM Search", ex.Message);
		}
		return result;
	}

	public DataTable SearchLines(ApzBillOfMaterials bom)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_BillOfMaterial_SearchLine";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@fatherCode", bom.Code));
			list.Add(new SqlParameter("@cardCode", bom.CardCode));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data BOM line Search", ex.Message);
		}
		return result;
	}

	public bool CheckExist(string itemCode, string cardCode)
	{
		try
		{
			string query = $"SELECT  A.bomEntry \r\n                                                      FROM OITT A \r\n                                                      WHERE A.Code = '{itemCode}' AND A.CardCode = '{cardCode}'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public bool CheckLineExist(string fatherCode, string cardCode, int childNum)
	{
		try
		{
			string query = $"SELECT  A.bomEntry \r\n                                                      FROM ITT1 A \r\n                                                      WHERE A.FatherCode = '{fatherCode}' AND A.CardCode = '{cardCode}' AND A.ChilNum = {childNum}";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	public string SaveBoms(DataSet ds)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			List<SqlParameter> list = new List<SqlParameter>();
			try
			{
				if (ds != null && ds.Tables.Count != 2)
				{
					return "Dataset is empty!";
				}
				ds.Tables[0].Columns.Remove("ItemName");
				ds.Tables[0].Columns.Remove("CardName");
				ds.Tables[1].Columns.Remove("ItemName");
				ds.Tables[1].Columns.Remove("CardName");
				string text = null;
				string query = "APZ_ProductionAddOn_BOM_SaveBom";
				list.Add(new SqlParameter("@dt", ds.Tables[0]));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					text = SaveLines(ds.Tables[1]);
					if (!string.IsNullOrEmpty(text))
					{
						dbTransaction.Rollback();
						return text;
					}
					dbTransaction.Commit();
					return "";
				}
				return "Error!";
			}
			catch (Exception ex)
			{
				dbTransaction.Rollback();
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save Stage  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}

	public string SaveLines(DataTable dt)
	{
		try
		{
			if (dt != null && dt.Rows.Count > 0)
			{
				List<SqlParameter> list = new List<SqlParameter>();
				string query = "APZ_ProductionAddOn_BOM_SaveLine";
				list.Add(new SqlParameter("@dt", dt));
				if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
				{
					return "";
				}
				return "ERROR";
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save Stage  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteBom(ApzBillOfMaterials bom)
	{
		using (DatabaseService.OpenConnection())
		{
			using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
			List<SqlParameter> list = new List<SqlParameter>();
			string empty = string.Empty;
			try
			{
				empty = "DELETE FROM dbo.APZ_OITT WHERE Code = @Code AND CardCode = @CardCode  \\n";
				empty += "DELETE FROM dbo.APZ_ITT1 WHERE FatherCode = @Code AND CardCode = @CardCode  \\n";
				list.Add(new SqlParameter("@Code", bom.Code));
				list.Add(new SqlParameter("@CardCode", bom.CardCode));
				if (ExecuteNonQuery(empty, CommandType.Text, list.ToArray()) > 0)
				{
					dbTransaction.Commit();
					return null;
				}
				return "ERROR";
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE bom  FAILED ", ex.Message);
				return ex.Message;
			}
		}
	}

	public DataTable GetList_BOM()
	{
		try
		{
			string query = "APZ_ProductionAddOn_GetBOM";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
			if (dataTable != null)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Data Receipt  FAILED ", ex.Message);
			return new DataTable();
		}
	}
}
