using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Apzon.Commons;
using Apzon.Entities.Models.MasterData;
using Apzon.Services.Interfaces.Inventory;

namespace Apzon.Services.Repository.Inventory;

public class STDPriceListsRepository : BaseService<DataTable>, ISTDPriceLists
{
	public STDPriceListsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable LoadData()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SP_APZ_STDPriceLists_LoadData";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string SaveDoc(DataTable dt)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				bool flag = true;
				string result = "";
				for (int i = 0; i < dt.Rows.Count; i++)
				{
					string text = SaveADoc(dt.Rows[i]);
					if (!string.IsNullOrWhiteSpace(text))
					{
						flag = false;
						result = text;
						break;
					}
				}
				if (flag)
				{
					dbTransaction.Commit();
				}
				return result;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string SaveRelatedDoc(DataRow dataRow, DataTable dtItemCode = null)
	{
		throw new NotImplementedException();
	}

	private DataTable GetListItemCode()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SELECT \"ItemCode\" FROM ITM1 WHERE \"PriceList\" = (SELECT TOP 1 \"PriceList\" FROM ITM1);";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	private string GenerateItemPriceTable(int listNum, int basePLNum, double factor)
	{
		try
		{
			ExecuteDataTable("APZ_PriceListSetup_GetItemPriceTable", CommandType.StoredProcedure, new SqlParameter("@listNum", listNum), new SqlParameter("@basePLNum", basePLNum), new SqlParameter("@factor", factor));
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string SaveDocSelection(DataSet ds)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				bool flag = true;
				string result = "";
				for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
				{
					string text = SaveADoc(ds.Tables[0].Rows[i], ds.Tables[1]);
					if (!string.IsNullOrWhiteSpace(text))
					{
						flag = false;
						result = text;
						break;
					}
				}
				if (flag)
				{
					dbTransaction.Commit();
				}
				else
				{
					dbTransaction.Rollback();
				}
				return result;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string SaveADoc(DataRow row, DataTable dtItemCode = null)
	{
		string text = "";
		try
		{
			DataTable dataTable = new DataTable();
			dataTable = ((dtItemCode != null) ? dtItemCode : GetListItemCode());
			GetAllField();
			int listNum = Function.ParseInt(row["ListNum"]);
			string text2 = Function.ToString(row["ListName"]);
			if (listNum < 0 && CheckExistPriceList(text2))
			{
				return "'" + text2 + "'. This entry already exists in the following tables  'Price Lists' (OPLN)";
			}
			int basePLNum = Function.ParseInt(row["BASE_NUM"]);
			double factor = Function.ParseDouble(row["Factor"]);
			bool changeBasePL = IsChangedBasePL(listNum, basePLNum);
			int num = 0;
			List<SqlParameter> list = new List<SqlParameter>();
			StringBuilder stringBuilder = new StringBuilder();
			if (listNum < 0)
			{
				text = RestoreNummering();
				if (text != string.Empty)
				{
					return text;
				}
				num = Function.ParseInt(GetObjectId(ApzObjectTypes.oPriceLists));
				if (Function.ParseInt(row["BASE_NUM"]) <= 0)
				{
					row["BASE_NUM"] = num;
					basePLNum = num;
				}
				stringBuilder.Append("INSERT INTO dbo.OPLN\r\n                                   ([ListNum]\r\n                                   ,[ListName]\r\n                                   ,[BASE_NUM]\r\n                                   ,[Factor]\r\n                                   ,[RoundSys]\r\n                                   ,[GroupCode]\r\n                                   ,[DataSource]\r\n                                   ,[SPPCounter]\r\n                                   ,[UserSign]\r\n                                   ,[IsGrossPrc]\r\n                                   ,[LogInstanc]\r\n                                   ,[UserSign2]\r\n                                   ,[UpdateDate]\r\n                                   ,[ValidFor]\r\n                                   ,[ValidFrom]\r\n                                   ,[ValidTo]\r\n                                   ,[CreateDate]\r\n                                   ,[PrimCurr]\r\n                                   ,[AddCurr1]\r\n                                   ,[AddCurr2]\r\n                                   ,[RoundRule]\r\n                                   ,[ExtAmount]\r\n                                   ,[RndFrmtInt]\r\n                                   ,[RndFrmtDec]");
				stringBuilder.Append(GetInsertUDFFieldString(row.Table));
				stringBuilder.Append(") VALUES (");
				stringBuilder.Append("@ListNum ");
				list.Add(new SqlParameter("@ListNum", num));
				stringBuilder.Append(",@ListName ");
				list.Add(new SqlParameter("@ListName", Function.ToString(row["ListName"])));
				stringBuilder.Append(",@BASE_NUM ");
				list.Add(new SqlParameter("@BASE_NUM", Function.ParseInt(row["BASE_NUM"])));
				if (Function.ParseInt(row["BASE_NUM"]) == Function.ParseInt(row["ListNum"]))
				{
					stringBuilder.Append(",1");
				}
				else
				{
					stringBuilder.Append(",@Factor ");
					list.Add(new SqlParameter("@Factor", Function.ParseDecimal(row["Factor"])));
				}
				stringBuilder.Append(",@RoundSys ");
				list.Add(new SqlParameter("@RoundSys", Function.ParseInt(row["RoundSys"])));
				stringBuilder.Append(",@GroupCode ");
				list.Add(new SqlParameter("@GroupCode", Function.ParseInt(row["GroupCode"])));
				stringBuilder.Append(",@DataSource ");
				list.Add(new SqlParameter("@DataSource", Function.ToString(row["DataSource"])));
				stringBuilder.Append(",@SPPCounter ");
				list.Add(new SqlParameter("@SPPCounter", Function.ParseInt(row["SPPCounter"])));
				stringBuilder.Append(",@UserSign ");
				list.Add(new SqlParameter("@UserSign", Function.ParseInt(row["UserSign"])));
				stringBuilder.Append(",@IsGrossPrc ");
				list.Add(new SqlParameter("@IsGrossPrc", Function.ParseInt(row["IsGrossPrc"])));
				stringBuilder.Append(",@LogInstanc ");
				list.Add(new SqlParameter("@LogInstanc", Function.ParseInt(row["LogInstanc"])));
				stringBuilder.Append(",@UserSign2 ");
				list.Add(new SqlParameter("@UserSign2", Function.ParseInt(row["UserSign2"])));
				stringBuilder.Append(",@UpdateDate ");
				list.Add(new SqlParameter("@UpdateDate", DBNull.Value));
				stringBuilder.Append(",@ValidFor ");
				list.Add(new SqlParameter("@ValidFor", Function.ToString(row["ValidFor"])));
				stringBuilder.Append(",@ValidFrom ");
				DateTime? dateTime = Function.ParseDateTime(row["ValidFrom"]);
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					list.Add(new SqlParameter("@ValidFrom", dateTime.Value));
				}
				else
				{
					list.Add(new SqlParameter("@ValidFrom", DBNull.Value));
				}
				stringBuilder.Append(",@ValidTo ");
				DateTime? dateTime2 = Function.ParseDateTime(row["ValidTo"]);
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					list.Add(new SqlParameter("@ValidTo", dateTime2.Value));
				}
				else
				{
					list.Add(new SqlParameter("@ValidTo", DBNull.Value));
				}
				stringBuilder.Append(",@CreateDate ");
				list.Add(new SqlParameter("@CreateDate", DateTime.Now));
				stringBuilder.Append(",@PrimCurr ");
				list.Add(new SqlParameter("@PrimCurr", Function.ToString(row["PrimCurr"])));
				stringBuilder.Append(",@AddCurr1 ");
				list.Add(new SqlParameter("@AddCurr1", Function.ToString(row["AddCurr1"])));
				stringBuilder.Append(",@AddCurr2 ");
				list.Add(new SqlParameter("@AddCurr2", Function.ToString(row["AddCurr2"])));
				stringBuilder.Append(",@RoundRule ");
				list.Add(new SqlParameter("@RoundRule", Function.ToString(row["RoundRule"])));
				stringBuilder.Append(",@ExtAmount ");
				list.Add(new SqlParameter("@ExtAmount", Function.ParseDecimal(row["ExtAmount"])));
				stringBuilder.Append(",@RndFrmtInt ");
				list.Add(new SqlParameter("@RndFrmtInt", Function.ToString(row["RndFrmtInt"])));
				stringBuilder.Append(",@RndFrmtDec ");
				list.Add(new SqlParameter("@RndFrmtDec", Function.ToString(row["RndFrmtDec"])));
				for (int i = 0; i < row.Table.Columns.Count; i++)
				{
					if (row.Table.Columns[i].ColumnName.StartsWith("U_"))
					{
						stringBuilder.Append(",@" + row.Table.Columns[i].ColumnName + " ");
						list.Add(new SqlParameter("@" + row.Table.Columns[i].ColumnName, Function.ToString(row[row.Table.Columns[i].ColumnName])));
					}
				}
				stringBuilder.Append(" )");
			}
			else
			{
				list = new List<SqlParameter>();
				stringBuilder.Append("Update OPLN SET ");
				stringBuilder.Append(" ListName=@ListName ");
				list.Add(new SqlParameter("@ListName", Function.ToString(row["ListName"])));
				stringBuilder.Append(",BASE_NUM=@BASE_NUM ");
				list.Add(new SqlParameter("@BASE_NUM", Function.ParseInt(row["BASE_NUM"])));
				if (Function.ParseInt(row["BASE_NUM"]) == Function.ParseInt(row["ListNum"]))
				{
					stringBuilder.Append(",Factor=1");
				}
				else
				{
					stringBuilder.Append(",Factor=@Factor ");
					list.Add(new SqlParameter("@Factor", Function.ParseDecimal(row["Factor"])));
				}
				stringBuilder.Append(",RoundSys=@RoundSys ");
				list.Add(new SqlParameter("@RoundSys", Function.ParseInt(row["RoundSys"])));
				stringBuilder.Append(",GroupCode=@GroupCode ");
				list.Add(new SqlParameter("@GroupCode", Function.ParseInt(row["GroupCode"])));
				stringBuilder.Append(",DataSource=@DataSource ");
				list.Add(new SqlParameter("@DataSource", Function.ToString(row["DataSource"])));
				stringBuilder.Append(",SPPCounter=@SPPCounter ");
				list.Add(new SqlParameter("@SPPCounter", Function.ParseInt(row["SPPCounter"])));
				stringBuilder.Append(",UserSign=@UserSign ");
				list.Add(new SqlParameter("@UserSign", Function.ParseInt(row["UserSign"])));
				stringBuilder.Append(",IsGrossPrc=@IsGrossPrc ");
				list.Add(new SqlParameter("@IsGrossPrc", Function.ParseInt(row["IsGrossPrc"])));
				stringBuilder.Append(",LogInstanc=@LogInstanc ");
				list.Add(new SqlParameter("@LogInstanc", Function.ParseInt(row["LogInstanc"])));
				stringBuilder.Append(",UserSign2=@UserSign2 ");
				list.Add(new SqlParameter("@UserSign2", Function.ParseInt(row["UserSign2"])));
				stringBuilder.Append(",UpdateDate=@UpdateDate ");
				list.Add(new SqlParameter("@UpdateDate", DateTime.Now));
				stringBuilder.Append(",ValidFor=@ValidFor ");
				list.Add(new SqlParameter("@ValidFor", Function.ToString(row["ValidFor"])));
				stringBuilder.Append(",ValidFrom=@ValidFrom ");
				DateTime? dateTime3 = Function.ParseDateTime(row["ValidFrom"]);
				if (dateTime3.HasValue)
				{
					_ = dateTime3.Value;
					list.Add(new SqlParameter("@ValidFrom", dateTime3.Value));
				}
				else
				{
					list.Add(new SqlParameter("@ValidFrom", DBNull.Value));
				}
				stringBuilder.Append(",ValidTo=@ValidTo ");
				DateTime? dateTime4 = Function.ParseDateTime(row["ValidTo"]);
				if (dateTime4.HasValue)
				{
					_ = dateTime4.Value;
					list.Add(new SqlParameter("@ValidTo", dateTime4.Value));
				}
				else
				{
					list.Add(new SqlParameter("@ValidTo", DBNull.Value));
				}
				stringBuilder.Append(",PrimCurr=@PrimCurr ");
				list.Add(new SqlParameter("@PrimCurr", Function.ToString(row["PrimCurr"])));
				stringBuilder.Append(",AddCurr1=@AddCurr1 ");
				list.Add(new SqlParameter("@AddCurr1", Function.ToString(row["AddCurr1"])));
				stringBuilder.Append(",AddCurr2=@AddCurr2 ");
				list.Add(new SqlParameter("@AddCurr2", Function.ToString(row["AddCurr2"])));
				stringBuilder.Append(",RoundRule=@RoundRule ");
				list.Add(new SqlParameter("@RoundRule", Function.ToString(row["RoundRule"])));
				stringBuilder.Append(",ExtAmount=@ExtAmount ");
				list.Add(new SqlParameter("@ExtAmount", Function.ParseDecimal(row["ExtAmount"])));
				stringBuilder.Append(",RndFrmtInt=@RndFrmtInt ");
				list.Add(new SqlParameter("@RndFrmtInt", Function.ToString(row["RndFrmtInt"])));
				stringBuilder.Append(",RndFrmtDec=@RndFrmtDec ");
				list.Add(new SqlParameter("@RndFrmtDec", Function.ToString(row["RndFrmtDec"])));
				for (int j = 0; j < row.Table.Columns.Count; j++)
				{
					if (row.Table.Columns[j].ColumnName.StartsWith("U_"))
					{
						stringBuilder.Append("," + row.Table.Columns[j].ColumnName + "=@" + row.Table.Columns[j].ColumnName + " ");
						list.Add(new SqlParameter("@" + row.Table.Columns[j].ColumnName, Function.ToString(row[row.Table.Columns[j].ColumnName])));
					}
				}
				stringBuilder.Append(" WHERE ListNum =@ListNum");
				list.Add(new SqlParameter("@ListNum", Function.ParseInt(row["ListNum"])));
			}
			if (ExecuteNonQuery(stringBuilder.ToString(), CommandType.Text, list.ToArray()) > 0)
			{
				if (listNum < 0)
				{
					SetNextAutoKey(ApzObjectTypes.oPriceLists);
					text = AddItemsPrice(num, basePLNum, factor);
					if (string.IsNullOrEmpty(text))
					{
						DataTable table = row.Table;
						EnumerableRowCollection<DataRow> enumerableRowCollection = from t in table.AsEnumerable()
							where Function.ParseInt(t["BASE_NUM"]) == listNum
							select t;
						if (enumerableRowCollection != null)
						{
							List<DataRow> list2 = enumerableRowCollection.ToList();
							if (list2.Count > 0)
							{
								for (int num2 = 0; num2 < list2.Count; num2++)
								{
									list2[num2]["BASE_NUM"] = num;
								}
								table.AcceptChanges();
							}
						}
					}
				}
				else if (dataTable.Rows.Count > 0)
				{
					text = UpdateItemsPrice(listNum, basePLNum, factor, dataTable, changeBasePL);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE A DOC OPLN  FAILED ", ex.Message);
			text = ex.Message;
		}
		return text;
	}

	private bool CheckExistPriceList(string listName)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT 1 FROM OPLN WHERE \"ListName\" = @listName;", CommandType.Text, new SqlParameter("@listName", listName));
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

	public DataTable GetGroupNumPriceList()
	{
		try
		{
			string query = "SELECT * FROM OPLN;";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public string DeletePriceList(int listNum)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				DataTable dataTable = ExecuteDataTable("APZ_PriceListSetup_Delete", CommandType.StoredProcedure, new SqlParameter("@listNum", listNum));
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return Function.ToString(dataTable.Rows[0][0]);
				}
				dbTransaction.Commit();
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public string RestoreNummering()
	{
		try
		{
			string query = RestoreNummeringQuery();
			ExecuteNonQuery(query, CommandType.Text);
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual string RestoreNummeringQuery()
	{
		return "DECLARE @maxListNum INT = ISNULL((SELECT MAX(ISNULL(listnum,0)) FROM opln),0)\r\n                                SELECT  @maxListNum\r\n                                UPDATE onnm SET AutoKey = @maxListNum + 1 WHERE  objectcode='6';";
	}

	public string CorrectPriceCurrency()
	{
		try
		{
			ExecuteNonQuery("APZ_STDPriceList_CorrectPriceCurrency", CommandType.StoredProcedure);
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual DataTable GetPriceForListItem(DataTable lstItemCode, int priceList)
	{
		try
		{
			return ExecuteDataTable("APZ_PriceList_GetPriceForListItem", CommandType.StoredProcedure, new SqlParameter("@itemCode", lstItemCode), new SqlParameter("@priceList", priceList));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable DetailsLoadData(int ListNum)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SP_APZ_STDPriceLists_DetailsLoadData";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ListNum", ListNum));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public string DetailsSaveDoc(DataSet ds)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				bool flag = true;
				string text = "";
				if (ds == null || ds.Tables.Count != 2 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[1] == null || ds.Tables[1].Rows.Count == 0)
				{
					flag = false;
					text = "Some data is missing (wrong number of table or a table have no row)";
				}
				else
				{
					DataTable dataTable = ds.Tables[0];
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						string text2 = DetailsSaveADoc(dataTable.Rows[i]);
						if (!string.IsNullOrWhiteSpace(text2))
						{
							flag = false;
							text = text2;
							break;
						}
					}
					if (string.IsNullOrWhiteSpace(text))
					{
						string isGross = ((string.Compare(Function.ToString(ds.Tables[1].Rows[0]["IsGross"]), "Y", ignoreCase: true) == 0) ? "Y" : "N");
						int listNum = Function.ParseInt(ds.Tables[1].Rows[0]["ListNum"]);
						string text3 = DetailsSaveIsGross(isGross, listNum);
						if (!string.IsNullOrWhiteSpace(text3))
						{
							flag = false;
							text = text3;
						}
					}
				}
				if (flag)
				{
					dbTransaction.Commit();
				}
				else
				{
					dbTransaction.Rollback();
				}
				return text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string DetailsSaveIsGross(string isGross, int listNum)
	{
		try
		{
			string query = "UPDATE OPLN SET \"IsGrossPrc\" = @isGross FROM OPLN WHERE \"ListNum\" =  @listNum;";
			ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@isGross", isGross), new SqlParameter("@listNum", listNum));
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string DetailsSaveADoc(DataRow row)
	{
		string result = "";
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			StringBuilder stringBuilder = new StringBuilder();
			list = new List<SqlParameter>();
			stringBuilder.Append("Update ITM1 SET ");
			if (Function.ParseDecimal(row["Price"]) != 0m)
			{
				stringBuilder.Append("Price=@Price ");
				list.Add(new SqlParameter("@Price", Function.ParseDecimal(row["Price"])));
				stringBuilder.Append(",Currency=@Currency ");
				list.Add(new SqlParameter("@Currency", Function.ToString(row["Currency"])));
			}
			else
			{
				stringBuilder.Append("Price=0 ");
				stringBuilder.Append(",Currency=null ");
			}
			stringBuilder.Append(",Ovrwritten=@Ovrwritten ");
			list.Add(new SqlParameter("@Ovrwritten", Function.ToString(row["Ovrwritten"])));
			if (Function.ParseDecimal(row["Factor"]) != 0m)
			{
				stringBuilder.Append(",Factor=@Factor ");
				list.Add(new SqlParameter("@Factor", Function.ParseDecimal(row["Factor"])));
			}
			else
			{
				stringBuilder.Append(",Factor=1 ");
			}
			stringBuilder.Append(",LogInstanc=@LogInstanc ");
			list.Add(new SqlParameter("@LogInstanc", Function.ParseInt(row["LogInstanc"])));
			stringBuilder.Append(",ObjType=@ObjType ");
			list.Add(new SqlParameter("@ObjType", Function.ToString(row["ObjType"])));
			if (Function.ParseDecimal(row["AddPrice1"]) != 0m)
			{
				stringBuilder.Append(",AddPrice1=@AddPrice1 ");
				list.Add(new SqlParameter("@AddPrice1", Function.ParseDecimal(row["AddPrice1"])));
				stringBuilder.Append(",Currency1=@Currency1 ");
				list.Add(new SqlParameter("@Currency1", Function.ToString(row["Currency1"])));
			}
			else
			{
				stringBuilder.Append(",AddPrice1 = 0 ");
				stringBuilder.Append(",Currency1 = null ");
			}
			if (Function.ParseDecimal(row["AddPrice2"]) != 0m)
			{
				stringBuilder.Append(",AddPrice2=@AddPrice2 ");
				list.Add(new SqlParameter("@AddPrice2", Function.ParseDecimal(row["AddPrice2"])));
				stringBuilder.Append(",Currency2=@Currency2 ");
				list.Add(new SqlParameter("@Currency2", Function.ToString(row["Currency2"])));
			}
			else
			{
				stringBuilder.Append(",AddPrice2 = 0 ");
				stringBuilder.Append(",Currency2 = null ");
			}
			stringBuilder.Append(",Ovrwrite1=@Ovrwrite1 ");
			list.Add(new SqlParameter("@Ovrwrite1", Function.ToString(row["Ovrwrite1"])));
			stringBuilder.Append(",Ovrwrite2=@Ovrwrite2 ");
			list.Add(new SqlParameter("@Ovrwrite2", Function.ToString(row["Ovrwrite2"])));
			stringBuilder.Append(",BasePLNum=@BasePLNum ");
			list.Add(new SqlParameter("@BasePLNum", Function.ParseInt(row["BasePLNum"])));
			for (int i = 0; i < row.Table.Columns.Count; i++)
			{
				if (row.Table.Columns[i].ColumnName.StartsWith("U_"))
				{
					stringBuilder.Append("," + row.Table.Columns[i].ColumnName + "=@" + row.Table.Columns[i].ColumnName + " ");
					list.Add(new SqlParameter("@" + row.Table.Columns[i].ColumnName, Function.ToString(row[row.Table.Columns[i].ColumnName])));
				}
			}
			stringBuilder.Append(" WHERE ItemCode =@ItemCode");
			list.Add(new SqlParameter("@ItemCode", Function.ToString(row["ItemCode"])));
			stringBuilder.Append(" and PriceList=@PriceList ");
			list.Add(new SqlParameter("@PriceList", Function.ParseInt(row["PriceList"])));
			if (ExecuteNonQuery(stringBuilder.ToString(), CommandType.Text, list.ToArray()) > 0)
			{
				result = UpdateRelatedPrices(Function.ToString(row["ItemCode"]), Function.ParseInt(row["PriceList"]), Function.ParseInt(row["PriceList"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE A DOC ITM1  FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public DataTable DetailsLoadIsGross(int listNum)
	{
		DataTable result = new DataTable();
		try
		{
			string query = DetailsLoadIsGrossQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@listNum", listNum));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string DetailsLoadIsGrossQuery()
	{
		return "SELECT isnull(IsGrossPrc,'N') FROM OPLN WHERE ListNum = @listNum";
	}

	private string AddItemsPrice(int listNum, int basePLNum, double factor)
	{
		try
		{
			GetListItemCode();
			string text = GenerateItemPriceTable(listNum, basePLNum, factor);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private DataTable GetTableStruct()
	{
		try
		{
			return ExecuteDataTable("Select top 0 * From ITM1;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private void AddRowToTable(DataTable dtBulkCopy, string itemCode, int listNum, DataTable dtAllField, int basePLNum, double factor)
	{
	}

	private string UpdateItemsPrice(int listNum, int basePLNum, double factor, DataTable dtItemCode = null, bool changeBasePL = false)
	{
		try
		{
			DataTable listItemCode = GetListItemCode();
			listItemCode = ((dtItemCode != null) ? dtItemCode : GetListItemCode());
			for (int i = 0; i < listItemCode.Rows.Count; i++)
			{
				string text = UpdateAnItemsPrice(Function.ToString(listItemCode.Rows[i]["ItemCode"]), listNum, basePLNum, factor, changeBasePL);
				if (!string.IsNullOrWhiteSpace(text))
				{
					return text;
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string UpdateAnItemsPrice(string itemCode, int listNum, int basePLNum, double factor, bool changeBasePriceList)
	{
		string result = "";
		try
		{
			double price = 0.0;
			double addPrice = 0.0;
			double addPrice2 = 0.0;
			string currency = "";
			string currency2 = "";
			string currency3 = "";
			DataTable dataTable = new DataTable();
			if (listNum != basePLNum)
			{
				dataTable = GetPriceCurrency(basePLNum, itemCode);
			}
			else
			{
				factor = 1.0;
				dataTable = GetPriceCurrency(listNum, itemCode);
			}
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				price = factor * Function.ParseDouble(dataTable.Rows[0]["Price"]);
				addPrice = factor * Function.ParseDouble(dataTable.Rows[0]["AddPrice1"]);
				addPrice2 = factor * Function.ParseDouble(dataTable.Rows[0]["AddPrice2"]);
				currency = Function.ToString(dataTable.Rows[0]["Currency"]);
				currency2 = Function.ToString(dataTable.Rows[0]["Currency1"]);
				currency3 = Function.ToString(dataTable.Rows[0]["Currency2"]);
			}
			if (changeBasePriceList)
			{
				return ChangePriceListBasePLChanged(itemCode, listNum, basePLNum, factor, result, price, addPrice, addPrice2, currency, currency2, currency3);
			}
			return ChangePriceListBasePLNotChanged(itemCode, listNum, factor, result, price, addPrice, addPrice2, currency, currency2, currency3);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE A DOC ITM1  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	private string ChangePriceListBasePLNotChanged(string itemCode, int listNum, double factor, string result, double price, double addPrice1, double addPrice2, string currency, string currency1, string currency2)
	{
		StringBuilder stringBuilder = new StringBuilder();
		List<SqlParameter> list = new List<SqlParameter>();
		stringBuilder.Append("UPDATE [dbo].[ITM1]  SET ");
		if (price != 0.0)
		{
			stringBuilder.Append("Price=@Price,");
			list.Add(new SqlParameter("@Price", price));
			stringBuilder.Append("Currency=@Currency");
			list.Add(new SqlParameter("@Currency", currency));
		}
		else
		{
			stringBuilder.Append("Price=0,");
			stringBuilder.Append("Currency=null");
		}
		stringBuilder.Append(" WHERE ItemCode=@ItemCode AND PriceList=@PriceList AND ISNULL(Ovrwritten,'N')<>'Y'");
		list.Add(new SqlParameter("@ItemCode", itemCode));
		list.Add(new SqlParameter("@PriceList", listNum));
		ExecuteNonQuery(stringBuilder.ToString(), CommandType.Text, list.ToArray());
		if (result == "")
		{
			stringBuilder = new StringBuilder();
			stringBuilder.Append("UPDATE [dbo].[ITM1]  SET ");
			list = new List<SqlParameter>();
			if (addPrice1 != 0.0)
			{
				stringBuilder.Append("AddPrice1=@AddPrice1,");
				list.Add(new SqlParameter("@AddPrice1", addPrice1));
				stringBuilder.Append("Currency1=@Currency1");
				list.Add(new SqlParameter("@Currency1", currency1));
			}
			else
			{
				stringBuilder.Append("AddPrice1=0,");
				stringBuilder.Append("Currency1=null");
			}
			stringBuilder.Append(" WHERE ItemCode=@ItemCode AND PriceList=@PriceList AND ISNULL(Ovrwrite1,'N')<>'Y'");
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@PriceList", listNum));
			ExecuteNonQuery(stringBuilder.ToString(), CommandType.Text, list.ToArray());
		}
		if (result == "")
		{
			stringBuilder = new StringBuilder();
			stringBuilder.Append("UPDATE [dbo].[ITM1]  SET ");
			list = new List<SqlParameter>();
			if (addPrice2 != 0.0)
			{
				stringBuilder.Append("AddPrice2=@AddPrice2, ");
				list.Add(new SqlParameter("@AddPrice2", addPrice2));
				stringBuilder.Append("Currency2=@Currency2");
				list.Add(new SqlParameter("@Currency2", currency2));
			}
			else
			{
				stringBuilder.Append("AddPrice2=0,");
				stringBuilder.Append("Currency2=null");
			}
			stringBuilder.Append(" WHERE ItemCode=@ItemCode AND PriceList=@PriceList AND ISNULL(Ovrwrite2,'N')<>'Y'");
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@PriceList", listNum));
			ExecuteNonQuery(stringBuilder.ToString(), CommandType.Text, list.ToArray());
		}
		if (result == "")
		{
			stringBuilder = new StringBuilder();
			stringBuilder.Append("UPDATE [dbo].[ITM1]  SET ");
			list = new List<SqlParameter>();
			if (factor != 0.0)
			{
				stringBuilder.Append("Factor= @Factor");
				list.Add(new SqlParameter("@Factor", factor));
			}
			else
			{
				stringBuilder.Append("Factor=1");
			}
			stringBuilder.Append(" WHERE ItemCode=@ItemCode AND PriceList=@PriceList");
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@PriceList", listNum));
			ExecuteNonQuery(stringBuilder.ToString(), CommandType.Text, list.ToArray());
		}
		if (result == "")
		{
			result = UpdateRelatedPrices(itemCode, listNum, listNum);
		}
		return result;
	}

	private string ChangePriceListBasePLChanged(string itemCode, int listNum, int basePLNum, double factor, string result, double price, double addPrice1, double addPrice2, string currency, string currency1, string currency2)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("UPDATE [dbo].[ITM1]  SET ");
		List<SqlParameter> list = new List<SqlParameter>();
		if (listNum != basePLNum)
		{
			if (price != 0.0)
			{
				stringBuilder.Append("Price=@Price,");
				list.Add(new SqlParameter("@Price", price));
				stringBuilder.Append("Currency=@Currency,");
				list.Add(new SqlParameter("@Currency", currency));
			}
			else
			{
				stringBuilder.Append("Price=0,");
				stringBuilder.Append("Currency=null,");
			}
			if (addPrice1 != 0.0)
			{
				stringBuilder.Append("AddPrice1=@AddPrice1,");
				list.Add(new SqlParameter("@AddPrice1", addPrice1));
				stringBuilder.Append("Currency1=@Currency1,");
				list.Add(new SqlParameter("@Currency1", currency1));
			}
			else
			{
				stringBuilder.Append("AddPrice1=0,");
				stringBuilder.Append("Currency1=null,");
			}
			if (addPrice2 != 0.0)
			{
				stringBuilder.Append("AddPrice2=@AddPrice2, ");
				list.Add(new SqlParameter("@AddPrice2", addPrice2));
				stringBuilder.Append("Currency2=@Currency2,");
				list.Add(new SqlParameter("@Currency2", currency2));
			}
			else
			{
				stringBuilder.Append("AddPrice2=0,");
				stringBuilder.Append("Currency2=null,");
			}
		}
		if (basePLNum != 0)
		{
			stringBuilder.Append("BasePLNum=@BasePLNum, ");
			list.Add(new SqlParameter("@BasePLNum", basePLNum));
		}
		else
		{
			stringBuilder.Append("0,");
		}
		if (factor != 0.0)
		{
			stringBuilder.Append("Factor= @Factor,");
			list.Add(new SqlParameter("@Factor", factor));
		}
		else
		{
			stringBuilder.Append("0,");
		}
		stringBuilder.Append("Ovrwritten= null,");
		stringBuilder.Append("Ovrwrite1=null,");
		stringBuilder.Append("Ovrwrite2= null");
		if (stringBuilder[stringBuilder.Length - 1] == ',')
		{
			stringBuilder.Length--;
		}
		stringBuilder.Append(" WHERE ItemCode=@ItemCode AND PriceList=@PriceList");
		list.Add(new SqlParameter("@ItemCode", itemCode));
		list.Add(new SqlParameter("@PriceList", listNum));
		ExecuteNonQuery(stringBuilder.ToString(), CommandType.Text, list.ToArray());
		if (result == "")
		{
			result = UpdateRelatedPrices(itemCode, listNum, listNum);
		}
		return result;
	}

	public string UpdateRelatedPrices(string itemCode, int basePriceList, int OriginalPriceList)
	{
		try
		{
			DataTable relatedPriceList = GetRelatedPriceList(basePriceList, OriginalPriceList, itemCode);
			if (relatedPriceList != null && relatedPriceList.Rows.Count > 0)
			{
				int num = 0;
				if (num < relatedPriceList.Rows.Count)
				{
					string text = UpdateARelatedPrice(Function.ToString(relatedPriceList.Rows[num]["ItemCode"]), basePriceList, Function.ParseInt(relatedPriceList.Rows[num]["PriceList"]), Function.ParseDouble(relatedPriceList.Rows[num]["Factor"]));
					if (!string.IsNullOrEmpty(text))
					{
						return text;
					}
					return UpdateRelatedPrices(Function.ToString(relatedPriceList.Rows[num]["ItemCode"]), Function.ParseInt(relatedPriceList.Rows[num]["PriceList"]), OriginalPriceList);
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string UpdateARelatedPrice(string itemCode, int basePLNum, int listNum, double factor)
	{
		string result = "";
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			StringBuilder stringBuilder = new StringBuilder();
			double num = 0.0;
			double num2 = 0.0;
			double num3 = 0.0;
			string value = "";
			string value2 = "";
			string value3 = "";
			DataTable dataTable = new DataTable();
			if (listNum != basePLNum)
			{
				dataTable = GetPriceCurrency(basePLNum, itemCode);
			}
			else
			{
				factor = 1.0;
				dataTable = GetPriceCurrency(listNum, itemCode);
			}
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				num = factor * Function.ParseDouble(dataTable.Rows[0]["Price"]);
				num2 = factor * Function.ParseDouble(dataTable.Rows[0]["AddPrice1"]);
				num3 = factor * Function.ParseDouble(dataTable.Rows[0]["AddPrice2"]);
				value = Function.ToString(dataTable.Rows[0]["Currency"]);
				value2 = Function.ToString(dataTable.Rows[0]["Currency1"]);
				value3 = Function.ToString(dataTable.Rows[0]["Currency2"]);
			}
			list = new List<SqlParameter>();
			stringBuilder.Append("UPDATE [dbo].[ITM1]  SET ");
			if (num != 0.0)
			{
				stringBuilder.Append("Price=@Price,Currency= @Currency");
				list.Add(new SqlParameter("@Price", num));
				list.Add(new SqlParameter("@Currency", value));
			}
			else
			{
				stringBuilder.Append("Price=0,Currency= null");
			}
			stringBuilder.Append(" WHERE ItemCode=@ItemCode AND PriceList=@PriceList AND ISNULL(Ovrwritten,'N')<>'Y'");
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@PriceList", listNum));
			ExecuteNonQuery(stringBuilder.ToString(), CommandType.Text, list.ToArray());
			list = new List<SqlParameter>();
			stringBuilder = new StringBuilder();
			stringBuilder.Append("UPDATE [dbo].[ITM1]  SET ");
			if (num2 != 0.0)
			{
				stringBuilder.Append("AddPrice1=@AddPrice1,Currency1= @Currency1");
				list.Add(new SqlParameter("@AddPrice1", num2));
				list.Add(new SqlParameter("@Currency1", value2));
			}
			else
			{
				stringBuilder.Append("AddPrice1=0,Currency1= null");
			}
			stringBuilder.Append(" WHERE ItemCode=@ItemCode AND PriceList=@PriceList AND ISNULL(Ovrwrite1,'N')<>'Y'");
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@PriceList", listNum));
			ExecuteNonQuery(stringBuilder.ToString(), CommandType.Text, list.ToArray());
			list = new List<SqlParameter>();
			stringBuilder = new StringBuilder();
			stringBuilder.Append("UPDATE [dbo].[ITM1]  SET ");
			if (num3 != 0.0)
			{
				stringBuilder.Append("AddPrice2=@AddPrice2,Currency2= @Currency2");
				list.Add(new SqlParameter("@AddPrice2", num3));
				list.Add(new SqlParameter("@Currency2", value3));
			}
			else
			{
				stringBuilder.Append("AddPrice2=0,Currency2= null");
			}
			stringBuilder.Append(" WHERE ItemCode=@ItemCode AND PriceList=@PriceList AND ISNULL(Ovrwrite2,'N')<>'Y'");
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@PriceList", listNum));
			ExecuteNonQuery(stringBuilder.ToString(), CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE A DOC ITM1  FAILED ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	private DataTable GetRelatedPriceList(int priceList, int orgPriceList, string itemCode)
	{
		try
		{
			return ExecuteDataTable("SELECT \"PriceList\", \"Factor\", \"ItemCode\" FROM ITM1 WHERE  \"BasePLNum\" =@ListNum \r\n                                    AND \"PriceList\" <> @ListNum AND \"PriceList\" <> @orgPriceList AND \"ItemCode\"=@itemCode;", CommandType.Text, new SqlParameter("@ListNum", priceList), new SqlParameter("@orgPriceList", orgPriceList), new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	private bool IsChangedBasePL(int listNum, int basePLNum)
	{
		try
		{
			string query = "SELECT 1 FROM OPLN  WHERE \"ListNum\" = @ListNum AND \"BASE_NUM\"=@BasePLNum;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ListNum", listNum));
			list.Add(new SqlParameter("@BasePLNum", basePLNum));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	private DataTable GetAllField()
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select top 1 * from ITM1;";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	private string GetInsertUDFFieldString(DataTable dt)
	{
		string text = "";
		try
		{
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				if (dt.Columns[i].ColumnName.StartsWith("U_"))
				{
					text = text + "," + dt.Columns[i].ColumnName;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public int GetUgpEntry(string itemCode)
	{
		try
		{
			string query = "SELECT \"UgpEntry\" FROM OITM WHERE \"ItemCode\" = @ItemCode;";
			SqlParameter sqlParameter = new SqlParameter("@ItemCode", itemCode);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	public DataTable GetPriceCurrency(int priceList, string itemCode)
	{
		try
		{
			string query = "SELECT a.\"Price\", a.\"AddPrice1\", a.\"AddPrice2\", a.\"Currency\", a.\"Currency1\", a.\"Currency2\", a.\"Factor\",b.\"ItemName\",b.\"InvntryUom\" \"UomName\"\r\n                                    FROM ITM1 a inner join OITM b on a.\"ItemCode\" = b.\"ItemCode\" WHERE a.\"ItemCode\" = @ItemCode AND a.\"PriceList\" =@PriceList;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@PriceList", priceList));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new DataTable();
	}

	public PriceListCurrencyImpl GetDataCurrencyByPriceList(int priceList)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("SELECT \"ListNum\" AS \"PriceList\", \"PrimCurr\", \"AddCurr1\", \"AddCurr2\" FROM OPLN WHERE \"ListNum\" = @PriceList;", CommandType.Text, new SqlParameter("@PriceList", priceList));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return new PriceListCurrencyImpl
				{
					PriceList = priceList,
					PrimCurr = Function.ToString(dataTable.Rows[0][1]),
					AddCurr1 = Function.ToString(dataTable.Rows[0][2]),
					AddCurr2 = Function.ToString(dataTable.Rows[0][3])
				};
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return new PriceListCurrencyImpl
		{
			PriceList = priceList
		};
	}

	public DataTable GetDataSystemPriceListDetail(int priceList)
	{
		try
		{
			return ExecuteDataTable("APZ_STDPriceLists_GetDataSystemPriceListDetail", CommandType.StoredProcedure, new SqlParameter("@priceList", priceList));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
