using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Services.Interfaces.Production;

namespace Apzon.Services.Repository.Production;

public class OutsourcingRepository : BaseService<DataSet>, IOutsourcing
{
	public OutsourcingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable Search(DataTable dtSearch)
	{
		try
		{
			int num = Function.ParseInt(dtSearch.Rows[0]["DocNum"]);
			string value = Function.ToString(dtSearch.Rows[0]["Comments"]);
			string value2 = Function.ToString(dtSearch.Rows[0]["DocDate"]);
			return ExecuteDataTable("APZ_Outsourcing_Search", CommandType.StoredProcedure, new SqlParameter("@DocNum", num), new SqlParameter("@Comments", value), new SqlParameter("@DocDate", value2));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetADoc(int docNum, string grid)
	{
		try
		{
			return ExecuteDataTable("APZ_Outsourcing_GetADoc", CommandType.StoredProcedure, new SqlParameter("@DocNum", docNum), new SqlParameter("@Grid", grid));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable AddNewRow(string grid)
	{
		try
		{
			return ExecuteDataTable("APZ_Outsourcing_AddNewRow", CommandType.StoredProcedure, new SqlParameter("@grid", grid));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetItemDetails(string itemCode)
	{
		try
		{
			return ExecuteDataTable("APZ_Outsourcing_GetItemDetails", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBatchSerials(string docEntry, string objType)
	{
		try
		{
			return ExecuteDataTable("apz_document_getbatchserial_byDocEntry", CommandType.StoredProcedure, new SqlParameter("@DocEntry", docEntry), new SqlParameter("@ObjType", objType));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBatchSerialsColumns()
	{
		try
		{
			string query = "SELECT \r\n                                top 0 \r\n\t                                a.*,\r\n\t                                0 BinAbs\r\n\t                                , '' BinCode\r\n\t                                , CONVERT(NUMERIC(19,6),0) SelectedQty\r\n\t                                , '' WhsCode\r\n\t                                , '' BinDescription\r\n\t                                ,0 BaseLineNumber\r\n\t                                ,0 BaseDocEntry\r\n\t                                ,'S' ManBatchSerial\r\n\t                                ,CONVERT(NUMERIC(19,6),0) LogQty\r\n\t                                ,'Y' IsExits\r\n\t                         FROM dbo.OSRN a";
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetAllBinData(string itemCode, string whsCode)
	{
		try
		{
			return ExecuteDataTable("APZ_Outsourcing_GetAllBinData", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@whsCode", whsCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetBOMComponents(DataTable dtItemCodes)
	{
		try
		{
			DataTable dataTable = AddNewRow("I");
			if (dtItemCodes != null && dtItemCodes.Rows.Count > 0)
			{
				for (int i = 0; i < dtItemCodes.Rows.Count; i++)
				{
					DataTable aBOMComponents = GetABOMComponents(Function.ToString(dtItemCodes.Rows[i]["ItemCode"]), Function.ParseDouble(dtItemCodes.Rows[i]["Quantity"]));
					if (aBOMComponents != null && aBOMComponents.Rows.Count > 0)
					{
						for (int j = 0; j < aBOMComponents.Rows.Count; j++)
						{
							dataTable.ImportRow(aBOMComponents.Rows[j]);
						}
					}
				}
				dataTable.AcceptChanges();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private DataTable GetABOMComponents(string itemCode, double quantity)
	{
		try
		{
			return ExecuteDataTable("APZ_Outsourcing_GetBOMComponents", CommandType.StoredProcedure, new SqlParameter("@itemCode", itemCode), new SqlParameter("@qty", quantity));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetTotalAvgPrice(DataTable dtItemWhs)
	{
		try
		{
			string text = "";
			string text2 = "";
			string text3 = "";
			for (int i = 0; i < dtItemWhs.Rows.Count; i++)
			{
				text = text + Function.ToString(dtItemWhs.Rows[i]["ItemCode"]) + ",";
				text2 = text2 + Function.ToString(dtItemWhs.Rows[i]["WhsCode"]) + ",";
				text3 = text3 + Function.ParseDouble(dtItemWhs.Rows[i]["Quantity"]) + ",";
			}
			if (text.EndsWith(","))
			{
				text = text.Remove(text.Length - 1);
			}
			if (text2.EndsWith(","))
			{
				text2 = text2.Remove(text2.Length - 1);
			}
			if (text3.EndsWith(","))
			{
				text3 = text3.Remove(text3.Length - 1);
			}
			return ExecuteDataTable("APZ_Outsourcing_GetTotalAvgPrice", CommandType.StoredProcedure, new SqlParameter("@item", text), new SqlParameter("@whs", text2), new SqlParameter("@qty", text3));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public virtual int GetRecord(string position, int code)
	{
		try
		{
			string query = "";
			switch (position)
			{
			case "F":
				query = "Select top 1 Code From [@TBL_OUTS_DOCNUM] order by Code asc";
				break;
			case "P":
				query = " With CTE as (\r\n                            select RN = ROW_NUMBER() OVER (ORDER BY Code), Code FROM [@TBL_OUTS_DOCNUM])\r\n                             SELECT [Previous Row].Code FROM CTE [Current Row]\r\n                             LEFT JOIN CTE [Previous Row] ON \r\n                             [Previous Row].RN = [Current Row].RN - 1\r\n                            WHERE [Current Row].Code = @Code";
				break;
			case "N":
				query = " With CTE as (\r\n                            select RN = ROW_NUMBER() OVER (ORDER BY Code), Code FROM [@TBL_OUTS_DOCNUM])\r\n                            SELECT [Next Row].Code FROM CTE [Current Row]\r\n                            LEFT JOIN CTE [Next Row] ON \r\n                            [Next Row].RN = [Current Row].RN + 1\r\n                            WHERE [Current Row].Code = @Code";
				break;
			case "L":
				query = "Select top 1 Code From [@TBL_OUTS_DOCNUM] order by Code desc";
				break;
			}
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Code", code));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public virtual bool ExistsCode(int code)
	{
		try
		{
			string query = "SELECT 1\r\n                            FROM [@TBL_OUTS_DOCNUM] \r\n                            WHERE code = @code";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@code", code));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
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

	public DataTable GetItemsAvgPrice(DataTable dtItemWhs)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("AvgPrice", typeof(double));
			dataTable.AcceptChanges();
			if (dtItemWhs != null && dtItemWhs.Rows.Count > 0)
			{
				for (int i = 0; i < dtItemWhs.Rows.Count; i++)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow[0] = GetAvgPrice(Function.ToString(dtItemWhs.Rows[i]["ItemCode"]), Function.ToString(dtItemWhs.Rows[i]["WhsCode"]));
					dataTable.Rows.Add(dataRow);
				}
				dataTable.AcceptChanges();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private double GetAvgPrice(string itemCode, string whsCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select avgprice from OITW where itemCode=@itemCode and whsCode=@whsCode", CommandType.Text, new SqlParameter("@itemCode", itemCode), new SqlParameter("@whsCode", whsCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseDouble(dataTable.Rows[0][0]);
			}
			return 0.0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0.0;
		}
	}

	public DataTable GetBinData(string code, int type, string itemCode)
	{
		try
		{
			return ExecuteDataTable("APZ_Outsourcing_GetBinData", CommandType.StoredProcedure, new SqlParameter("@code", code), new SqlParameter("@typeDoc", type), new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
