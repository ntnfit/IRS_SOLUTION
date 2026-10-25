using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class WarehouseRepository(IDatabaseService databaseService) : BaseService<DataTable>(databaseService), IWarehouse, IService<WarehouseImpl>
{
	private string TempWHSTableName = "#APZ_WAREHOUSE_DOC";

	public DataTable Cfl()
	{
		try
		{
			string query = CflQuery();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string CflQuery()
	{
		return "SELECT *\r\n                    FROM OWHS\r\n                    WHERE (ISNULL(Nettable, 'Y') ='Y' OR ISNULL(DropShip, 'N') ='Y')\r\n                          AND ISNULL(Inactive, 'N') ='N'\r\n                          AND ISNULL(Locked, 'N') ='N'; ";
	}

	public List<WarehouseImpl> CflByWhseCode(string whsCode)
	{
		List<WarehouseImpl> list = new List<WarehouseImpl>();
		try
		{
			string text = "SELECT WhsCode,WhsName FROM OWHS ";
			if (!string.IsNullOrEmpty(whsCode))
			{
				text += " WHERE WhsCode = @WhsCode";
				SqlParameter sqlParameter = new SqlParameter("@WhsCode", whsCode);
				DataTable dataTable = ExecuteDataTable(text, CommandType.Text, sqlParameter);
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					list.Add(new WarehouseImpl
					{
						WhsCode = Function.ToString(dataTable.Rows[i]["WhsCode"]),
						WhsName = Function.ToString(dataTable.Rows[i]["WhsName"])
					});
				}
			}
			else
			{
				DataTable dataTable2 = ExecuteDataTable(text, CommandType.Text);
				for (int j = 0; j < dataTable2.Rows.Count; j++)
				{
					list.Add(new WarehouseImpl
					{
						WhsCode = Function.ToString(dataTable2.Rows[j]["WhsCode"]),
						WhsName = Function.ToString(dataTable2.Rows[j]["WhsName"])
					});
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	public string Insert(WarehouseImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(WarehouseImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(WarehouseImpl entity)
	{
		throw new NotImplementedException();
	}

	public WarehouseImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable ShowWarehouse()
	{
		try
		{
			string query = ShowWarehouseQuery();
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string ShowWarehouseQuery()
	{
		return "select WhsCode,WhsName FROM OWHS ";
	}

	public DataTable Search(string whsCode, string whsName, int bPLid)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_WareHouse_Search";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@WareHouseCode", whsCode));
			list.Add(new SqlParameter("@WareHouseName", whsName));
			list.Add(new SqlParameter("@branch", bPLid));
			return result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Warehouse Search - Doc ", ex.Message);
			return result;
		}
	}

	public int GetMaxCode()
	{
		throw new NotImplementedException();
	}

	public string Save(DataTable dt)
	{
		try
		{
			bool flag = Check(Function.ToString(dt.Rows[0]["WhsCode"]));
			return Process(ApzObjectTypes.oWarehouses, -1, flag ? Enums.TransactionType.U : Enums.TransactionType.A, dt).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataTable dt, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessWarehouse(ref transtype, dt, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessWarehouse(ref transtype, dt, ref objId, ref objNum, ref ret);
			break;
		}
		return ret;
	}

	private void ProcessWarehouse(ref Enums.TransactionType transtype, DataTable dtWhs, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (dtWhs == null || dtWhs.Rows.Count == 0)
			{
				ret.Message = "No data to save";
			}
			else
			{
				string value = ((transtype == Enums.TransactionType.A) ? "A" : "U");
				CreateTempTable(dtWhs, TempWHSTableName);
				BulkCopy(dtWhs, TempWHSTableName);
				SqlParameter sqlParameter = new SqlParameter("@Type", value);
				DataTable dataTable = ExecuteDataTable("apz_process_Warehouse", CommandType.StoredProcedure, sqlParameter);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					string text = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
					ret.Message = text + " - " + ret.Message;
				}
				else
				{
					ret.Message = "";
				}
			}
			if (string.IsNullOrEmpty(ret.Message))
			{
				objId = Function.ToString(dtWhs.Rows[0]["WhsCode"]);
				objNum = Function.ToString(dtWhs.Rows[0]["WhsCode"]);
			}
			else
			{
				objId = "";
				objNum = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	public bool Check(string whsCode)
	{
		try
		{
			string query = "SELECT 1 FROM OWHS WHERE \"WhsCode\" = @WhsCode;";
			SqlParameter sqlParameter = new SqlParameter("@WhsCode", whsCode);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Warehouse Search - Doc ", ex.Message);
			return true;
		}
	}

	public string DeleteDoc(string whsCode)
	{
		return "[IRS].[WarehouseRepository] - Under construction";
	}

	public DataTable GetItemOnhandByWarehouseItemCode(string whsCode, string itemCode)
	{
		try
		{
			string query = "SELECT  \"OnHand\" as \"QuantityOnStock\" from oitw where \"ItemCode\"=@Itemcode and \"WhsCode\"=@Whscode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Itemcode", itemCode));
			list.Add(new SqlParameter("@Whscode", whsCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Item Onhand By Warehouse ItemCode ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetDefaultTable()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_WareHouse_DefaultTable";
			return result = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Warehouse DefaultTable  ", ex.Message);
			return result;
		}
	}

	private string Update(DataRow dataRow)
	{
		return "Under construction";
	}

	public string RemoveWarehouses(List<string> lstWarehouse)
	{
		return "This function not supported";
	}

	public DataTable GetAllCode()
	{
		try
		{
			return ExecuteDataTable("SELECT \"WhsCode\",\"WhsName\" FROM OWHS;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetRecord(string position, string whsCode)
	{
		try
		{
			string query = "APZ_WareHouse_GetRecord";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@position", position));
			list.Add(new SqlParameter("@whsCode", whsCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public string DeleteWarehouse(string whsCode)
	{
		try
		{
			string query = "APZ_WareHouse_DeleteWarehouse";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@whsCode", whsCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public Common.Table_Code WhsCheckFollowBin(string whsCode)
	{
		try
		{
			string query = "Select \"BinActivat\", \"DftBinEnfd\" from OWHS where \"WhsCode\" = @whsCode;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@whsCode", whsCode));
			if (dataTable.IsNotNull())
			{
				return new Common.Table_Code
				{
					Code = Function.ToString(dataTable.Rows[0][0]),
					Name = Function.ToString(dataTable.Rows[0][1])
				};
			}
			return new Common.Table_Code("N", "N");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new Common.Table_Code();
	}

	public Common.Table_Code WhsGetDefBinItem(string whsCode, string itemCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select b.\"AbsEntry\", b.\"BinCode\" from OITW a inner JOIN OBIN b ON a.\"DftBinAbs\" = b.\"AbsEntry\" where a.\"WhsCode\" = @whsCode and a.\"ItemCode\" = @itemCode;", CommandType.Text, new SqlParameter("@whsCode", whsCode), new SqlParameter("@itemCode", itemCode));
			if (dataTable.IsNotNull())
			{
				return new Common.Table_Code
				{
					Code = Function.ToString(dataTable.Rows[0][0]),
					Name = Function.ToString(dataTable.Rows[0][1])
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new Common.Table_Code();
	}

	public DataTable CflDocument()
	{
		try
		{
			string query = CflDocumentQuery();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string CflDocumentQuery()
	{
		return "SELECT * FROM OWHS WHERE (isnull(Nettable,'Y') = 'Y' OR isnull(DropShip,'N')='Y') AND isnull(Inactive,'N') = 'N' AND isnull(Locked,'N') = 'N'";
	}
}
