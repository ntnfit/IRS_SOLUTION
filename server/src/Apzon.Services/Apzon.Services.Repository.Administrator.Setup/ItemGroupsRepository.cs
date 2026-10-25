using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services.Interfaces.Administrator.Setup;

namespace Apzon.Services.Repository.Administrator.Setup;

public class ItemGroupsRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IItemGroups, IService<ItemGroupsImpl>
{
	private string TempTableTab = "#dtOITB";

	private string TempTableTabWhs = "#dtITB1";

	public List<ItemGroupsImpl> Cfl()
	{
		DataTable dataTable = ExecuteDataTable("SELECT \"ItmsGrpCod\" ITMSGRPCOD,\"ItmsGrpNam\" ITMSGRPNAM FROM OITB;", CommandType.Text);
		List<ItemGroupsImpl> list = new List<ItemGroupsImpl>();
		try
		{
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(new ItemGroupsImpl
				{
					Number = Function.ParseInt(dataTable.Rows[i]["ITMSGRPCOD"]),
					GroupName = Function.ToString(dataTable.Rows[i]["ITMSGRPNAM"])
				});
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	public string Insert(ItemGroupsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(ItemGroupsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(ItemGroupsImpl entity)
	{
		throw new NotImplementedException();
	}

	public ItemGroupsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public DataTable GetItemGroup()
	{
		try
		{
			string query = "SELECT \"ItmsGrpCod\" ITMSGRPCOD,\"ItmsGrpNam\" ITMSGRPNAM FROM OITB where \"Locked\"='N' order by \"ItmsGrpNam\";";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable ShowItemGroup()
	{
		try
		{
			string query = "SELECT \"ItmsGrpCod\" ITMSGRPCOD,\"ItmsGrpNam\" ITMSGRPNAM,\r\n\t\t\t\t\t\t\t\t\tCASE \r\n\t\t\t\t\t\t\t\t\t\tWHEN \"Locked\" = 'N' then 'Locked'\r\n\t\t\t\t\t\t\t\t\t    WHEN \"Locked\" = 'Y' then 'Changeable'\r\n\t\t\t\t\t\t\t\t\tEND \"Locked\"\r\n\t\t\t\t\t\t\t   FROM  OITB ;  ";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable Search(ItemGroupsImpl doc)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "SELECT \"ItmsGrpCod\" ITMSGRPCOD, \"ItmsGrpNam\" ITMSGRPNAM,\r\n\t\t\t\t\t\t\t\t\tCASE \r\n\t\t\t\t\t\t\t\t\t\tWHEN \"Locked\" = 'N' then 'Locked'\r\n\t\t\t\t\t\t\t\t\t    WHEN \"Locked\" = 'Y' then 'Changeable'\r\n\t\t\t\t\t\t\t\t\tEND \"Locked\"\r\n\t\t\t\t\t\t\t   FROM  OITB    \r\n                                Where 1=1";
			if (doc.Locked == "Y")
			{
				text += "AND \"Locked\" = 'Y'";
			}
			else if (doc.Locked == "N")
			{
				text += "AND \"Locked\" = 'N'";
			}
			if (!string.IsNullOrEmpty(doc.ITMSGRPNAM))
			{
				text = text + "AND \"ItmsGrpNam\" like N'%" + doc.ITMSGRPNAM + "%' ";
			}
			result = ExecuteData(text + ";", CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data ItemGroup Search - Doc ", ex.Message);
		}
		return result;
	}

	public int GetMaxCode()
	{
		try
		{
			string maxCodeQuery = GetMaxCodeQuery();
			DataTable dataTable = ExecuteDataTable(maxCodeQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["ItmsGrpCod"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	protected virtual string GetMaxCodeQuery()
	{
		return "SELECT ISNULL( MAX(\"ItmsGrpCod\" ),0) as \"ItmsGrpCod\"\r\n                                    FROM OITB ;";
	}

	public bool Check(int code)
	{
		try
		{
			string query = "SELECT   A.\"ItmsGrpCod\" \r\n                                    FROM OITB A \r\n                                    WHERE A.\"ItmsGrpCod\" = @code;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@code", code));
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

	public HttpResult SaveDoc(DataSet ds)
	{
		return Process(ApzObjectTypes.oItemGroups, -1, Enums.TransactionType.U, ds);
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		try
		{
			string overSizeColumns = GetUnitOfWork().Commons.GetOverSizeColumns(ds.Tables[0], "OITB");
			if (overSizeColumns != "")
			{
				return new HttpResult(Enums.MessageCode.Error, overSizeColumns);
			}
			CreateTempTable(ds.Tables[0], TempTableTab);
			BulkCopy(ds.Tables[0], TempTableTab);
			CreateTempTable(ds.Tables[1], TempTableTabWhs);
			BulkCopy(ds.Tables[1], TempTableTabWhs);
			ExecuteNonQuery("apz_process_ItemGroups", CommandType.StoredProcedure);
			objId = "";
			objNum = objId;
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string DeleteDoc(int code)
	{
		try
		{
			DataTable dataTable = CheckExistItemGroupCode(code);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return "Item group is used in table '" + Function.ToString(dataTable.Rows[0][0]) + "' , can not delete!";
			}
			string query = DeleteDocQuery();
			if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@code", code)) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	protected virtual string DeleteDocQuery()
	{
		return " delete from OITB where ITMSGRPCOD = @code;\r\n                             delete OIGW where ITMSGRPCOD = @code;\r\n                             UPDATE ONNM SET AutoKey = (Select MAX(ItmsGrpCod) FROM OITB) +1 WHERE ObjectCode = '52';";
	}

	public DataTable CheckExistItemGroupCode(int itgrCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_ItemGroups_CheckExistsTrans", CommandType.StoredProcedure, new SqlParameter("@ItmsGrpCod", itgrCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable LoadDoc()
	{
		try
		{
			return ExecuteDataTable("select * from oitb;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetItemGroupWarehouse(int code)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select top 0 * from OIGW;", CommandType.Text);
			if (dataTable != null && dataTable.Columns.Count > 0)
			{
				DataTable dataTable2 = ExecuteDataTable(GetItemGroupWarehouseQuery(), CommandType.Text);
				if (dataTable2 != null && dataTable2.Rows.Count > 0)
				{
					dataTable.Columns.Add("WhsName", typeof(string));
					for (int i = 0; i < dataTable2.Rows.Count; i++)
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow["ItmsGrpCod"] = Function.ParseShort(code);
						dataRow["WhsCode"] = Function.ToString(dataTable2.Rows[i]["WhsCode"]);
						dataRow["WhsName"] = Function.ToString(dataTable2.Rows[i]["WhsName"]);
						dataRow["DftBinEnfd"] = "N";
						dataTable.Rows.Add(dataRow);
					}
					DataTable dt = ExecuteDataTable("select * from OIGW where \"ItmsGrpCod\"=@Code;", CommandType.Text, new SqlParameter("@Code", code));
					if (dt != null && dt.Rows.Count > 0)
					{
						int j;
						for (j = 0; j < dt.Rows.Count; j++)
						{
							DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.Compare(Function.ToString(dt.Rows[j]["WhsCode"]), Function.ToString(r["WhsCode"]), ignoreCase: true) == 0);
							if (dataRow2 != null)
							{
								for (int num = 0; num < dt.Columns.Count; num++)
								{
									dataRow2[dt.Columns[num].ColumnName] = dt.Rows[j][dt.Columns[num].ColumnName];
								}
							}
						}
						dataTable.AcceptChanges();
					}
					return dataTable;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetItemGroupWarehouseQuery()
	{
		return "select WhsCode,WhsName from owhs where isnull(locked, 'N') = 'N' AND isnull(Nettable, 'Y') = 'Y' AND isnull(BinActivat, 'N') = 'Y'";
	}
}
