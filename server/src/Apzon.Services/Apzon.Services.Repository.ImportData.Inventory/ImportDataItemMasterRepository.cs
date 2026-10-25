using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;

namespace Apzon.Services.Repository.ImportData.Inventory;

public class ImportDataItemMasterRepository : BaseService<DataTable>
{
	public ImportDataItemMasterRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable GetDefaultItemPrice()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select Top 0 * from ITM1", CommandType.Text);
			if (dataTable != null && dataTable.Columns.Count > 0)
			{
				string query = "select * from OPLN Where ValidFor='Y'";
				DataTable dataTable2 = ExecuteDataTable(query, CommandType.Text);
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					for (int i = 0; i < dataTable2.Rows.Count; i++)
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["ItemCode"] = "";
						dataRow["PriceList"] = Function.ParseShort(dataTable2.Rows[i]["ListNum"]);
						dataRow["Price"] = 0.0;
						dataRow["Currency"] = Function.ToString(dataTable2.Rows[i]["PrimCurr"]);
						dataRow["Factor"] = Function.ParseDouble(dataTable2.Rows[i]["Factor"]);
						dataRow["Currency1"] = Function.ToString(dataTable2.Rows[i]["AddCurr1"]);
						dataRow["Currency2"] = Function.ToString(dataTable2.Rows[i]["AddCurr2"]);
						dataRow["ObjType"] = "4";
						dataRow["AddPrice1"] = 0.0;
						dataRow["AddPrice2"] = 0.0;
						dataRow["BasePLNum"] = Function.ParseShort(dataTable2.Rows[i]["BASE_NUM"]);
						dataTable.Rows.Add(dataRow);
						dataTable.AcceptChanges();
					}
				}
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public int ItemGetDefaultInventoryUomEntry(int ugpEntry)
	{
		try
		{
			string query = "SELECT BaseUom FROM OUGP WHERE UgpEntry=@ugpEntry";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@ugpEntry", ugpEntry));
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

	public DataTable GetWareHouse()
	{
		try
		{
			string query = "select WhsCode,WhsName FROM OWHS Where (Nettable='Y' OR DropShip='Y') And Inactive='N'";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public bool CheckExistItem(string itemCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select 1 from OITM where itemCode=@itemCode", CommandType.Text, new SqlParameter("@itemCode", itemCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}
}
