using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Services.Interfaces.Inventory;

namespace Apzon.Services.Repository.Inventory;

public class ItemMasterDataRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IItemMasterData, IService<ItemsImpl>
{
	private Items_LineImpl lines = new Items_LineImpl();

	private string TempItemTableName = "#APZ_ITEM_DOC";

	private string TempAttFileTableName = "#APZ_ITEM_ATC";

	private string TempItemWarehouseTableName = "#APZ_ITEM_WHS";

	private string TempItemPriceTableName = "#APZ_ITEM_LINE";

	private string TempUoMInItemTableName = "#APZ_ITEM_ITM12";

	private string TempOtherDateTableName = "#APZ_ITEM_OTHER";

	private string TempPrefVendorDateTableName = "#APZ_ITEM_PREFVENDOR";

	public DataTable Table()
	{
		DataTable result = new DataTable();
		try
		{
			SqlParameter[] parameters = new SqlParameter[1]
			{
				new SqlParameter("@ListNum", -2)
			};
			result = ExecuteData("SP_APZ_CFL_ITMPRICE_REQUEST", CommandType.StoredProcedure, parameters).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Read data oitm  FAILED ", ex.Message);
		}
		return result;
	}

	public string Insert(ItemsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Update(ItemsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(ItemsImpl entity)
	{
		throw new NotImplementedException();
	}

	public ItemsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public List<ItemsImpl> Cfl()
	{
		List<ItemsImpl> list = new List<ItemsImpl>();
		try
		{
			string query = CflQuery();
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(new ItemsImpl
				{
					ItemCode = Function.ToString(dataTable.Rows[i]["ItemCode"]),
					ItemName = Function.ToString(dataTable.Rows[i]["ItemName"]),
					DfltWH = Function.ToString(dataTable.Rows[i]["DfltWH"]),
					OnHand = Function.ParseDouble(dataTable.Rows[i]["OnHand"])
				});
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return list;
	}

	protected virtual string CflQuery()
	{
		return "SELECT ItemCode,ItemName,DfltWH,OnHand FROM OITM";
	}

	public DataTable FindDataByShip(ItemsImpl doc)
	{
		return new DataTable();
	}

	public DataTable Check_FirstRecord()
	{
		try
		{
			string query = Check_FirstRecordQuery();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Check_FirstRecord  ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string Check_FirstRecordQuery()
	{
		return " select itemcode from oitm  where itemcode=(select min(itemcode) from oitm) ";
	}

	public DataTable Check_LastRecord()
	{
		try
		{
			string query = Check_LastRecordQuery();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Check_LastRecord  ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string Check_LastRecordQuery()
	{
		return " select itemcode from oitm where itemcode=(select max(itemcode) from oitm) ";
	}

	public DataTable ShowItem()
	{
		throw new NotImplementedException();
	}

	public DataTable ShowWareHouse()
	{
		try
		{
			string query = ShowWareHouseQuery();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string ShowWareHouseQuery()
	{
		return "SELECT \"WhsCode\", \"WhsName\", CASE WHEN \"Locked\" = 'N' THEN 'No' WHEN \"Locked\" = 'Y' THEN 'Yes' END AS \"Locked\" FROM OWHS WHERE (\"Nettable\" = 'Y' OR \"DropShip\" = 'Y') AND \"Inactive\" = 'N';";
	}

	public bool CheckDoc(string itemCode)
	{
		try
		{
			string query = CheckDocQuery();
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@itemCode", itemCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckDoc  ", ex.Message);
			return false;
		}
	}

	protected virtual string CheckDocQuery()
	{
		return "SELECT 1\r\n                            FROM OITM \r\n                            WHERE ItemCode = @itemCode";
	}

	public string SaveDoc(DataSet ds, string type = "")
	{
		try
		{
			Enums.TransactionType transactionType = Enums.TransactionType.A;
			new HttpResult();
			transactionType = ((type == "U") ? Enums.TransactionType.U : ((!(type == "A")) ? (CheckDoc(Function.ToString(ds.Tables["OITM"].Rows[0]["ItemCode"])) ? Enums.TransactionType.U : Enums.TransactionType.A) : Enums.TransactionType.A));
			return Process(ApzObjectTypes.oItems, -1, transactionType, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string SaveDocFromImport(DataSet ds, string action)
	{
		try
		{
			Enums.TransactionType transtype = Enums.TransactionType.A;
			if (action == "U")
			{
				transtype = Enums.TransactionType.U;
			}
			return Process(ApzObjectTypes.oItems, -1, transtype, ds).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		HttpResult ret = new HttpResult();
		switch (transtype)
		{
		case Enums.TransactionType.A:
			ProcessItem(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.U:
			ProcessItem(ref transtype, ds, ref objId, ref objNum, ref ret);
			break;
		case Enums.TransactionType.D:
			ProcessDeleteItem(ref ret, ref objId, ref objNum);
			break;
		}
		return ret;
	}

	private void ProcessItem(ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum, ref HttpResult ret)
	{
		try
		{
			if (ds == null || ds.Tables.Count < 5 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				ret.Message = "No data to save";
			}
			else
			{
				string text = ((transtype == Enums.TransactionType.A) ? "A" : "U");
				if (text == "A" && ds.Tables.Contains("OtherTable") && ds.Tables["OtherTable"].Rows.Count > 0 && !string.IsNullOrWhiteSpace(Function.ToString(ds.Tables["OtherTable"].Rows[0]["OldCode"])))
				{
					text = "U";
				}
				CreateTempTable(ds.Tables["OITM"], TempItemTableName);
				BulkCopy(ds.Tables["OITM"], TempItemTableName);
				CreateTempTable(ds.Tables["OITW"], TempItemWarehouseTableName);
				BulkCopy(ds.Tables["OITW"], TempItemWarehouseTableName);
				CreateTempTable(ds.Tables["ITM1"], TempItemPriceTableName);
				BulkCopy(ds.Tables["ITM1"], TempItemPriceTableName);
				CreateTempTable(ds.Tables["ITM12"], TempUoMInItemTableName);
				BulkCopy(ds.Tables["ITM12"], TempUoMInItemTableName);
				CreateTempTable(ds.Tables["ATC1"], TempAttFileTableName);
				BulkCopy(ds.Tables["ATC1"], TempAttFileTableName);
				CreateTempTable(ds.Tables["OtherTable"], TempOtherDateTableName);
				BulkCopy(ds.Tables["OtherTable"], TempOtherDateTableName);
				if (!ds.Tables.Contains("PrefVendor"))
				{
					DataTable preferredVendor = GetPreferredVendor("");
					preferredVendor.TableName = "PrefVendor";
					ds.Tables.Add(preferredVendor.Copy());
				}
				CreateTempTable(ds.Tables["PrefVendor"], TempPrefVendorDateTableName);
				BulkCopy(ds.Tables["PrefVendor"], TempPrefVendorDateTableName);
				SqlParameter sqlParameter = new SqlParameter("@Type", text);
				DataTable dataTable = ExecuteDataTable("apz_process_Item", CommandType.StoredProcedure, sqlParameter);
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					ret.Message = Function.ToString(dataTable.Rows[0]["ErrorMessage"]);
					string text2 = Function.ToString(dataTable.Rows[0]["ErrorNumber"]);
					ret.Message = text2 + " - " + ret.Message;
				}
				else if (text == "U")
				{
					string text3 = UpdateRelatedPrice(ds.Tables["ITM1"]);
					if (string.IsNullOrEmpty(text3))
					{
						ret.Message = string.Empty;
					}
					else
					{
						ret.Message = text3;
					}
				}
				else
				{
					ret.Message = "";
				}
			}
			if (string.IsNullOrEmpty(ret.Message))
			{
				objId = Function.ToString(ds.Tables[0].Rows[0]["ItemCode"]);
				objNum = Function.ToString(ds.Tables[0].Rows[0]["ItemCode"]);
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

	public string UpdateRelatedPrice(DataTable dt)
	{
		try
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string text = UpdateAnItemPrice(dt.Rows[i]);
				if (!string.IsNullOrEmpty(text))
				{
					return text;
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string UpdateAnItemPrice(DataRow row)
	{
		try
		{
			string itemCode = Function.ToString(row["ItemCode"]);
			int num = Function.ParseInt(row["PriceList"]);
			return GetUnitOfWork().STDPriceLists.UpdateRelatedPrices(itemCode, num, num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private void ProcessDeleteItem(ref HttpResult ret, ref string objId, ref string objNum)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@ItemCode", objId);
			DataTable dataTable = ExecuteDataTable("apz_itemmaster_RemoveItem", CommandType.StoredProcedure, sqlParameter);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				ret.Message = Function.ToString(dataTable.Rows[0][0]);
			}
			else
			{
				ret.Message = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			ret.Message = ex.Message;
		}
	}

	public string DeleteOITW(string itemCode, string whsCode)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				SqlParameter[] parameters = new SqlParameter[2]
				{
					new SqlParameter("@itemCode", itemCode),
					new SqlParameter("@whsCode", whsCode)
				};
				string query = DeleteOITWQuery();
				if (ExecuteNonQuery(query, CommandType.Text, parameters) > 0)
				{
					dbTransaction.Commit();
					return "";
				}
				return "No rows deleted";
			}
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	protected virtual string DeleteOITWQuery()
	{
		return " delete OITW where ItemCode =@itemCode and WhsCode = @whsCode; ";
	}

	public string DeleteDoc(string itemCode)
	{
		throw new NotImplementedException();
	}

	public DataTable Search(Tuple<ItemsImpl, DataTable> tpl)
	{
		DataTable result = new DataTable();
		try
		{
			ItemsImpl item = tpl.Item1;
			string text = "SELECT * FROM OITM WHERE 1=1 ";
			List<SqlParameter> prms = new List<SqlParameter>();
			text += SearchQuery2(item, ref prms);
			DataTable item2 = tpl.Item2;
			if (item2 != null && item2.Rows.Count > 0)
			{
				text += SearchQuery1(item2, ref prms);
			}
			result = ExecuteDataTable(text + ";", CommandType.Text, prms.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Return Search - Doc ", ex.Message);
		}
		return result;
	}

	protected virtual string SearchQuery2(ItemsImpl doc, ref List<SqlParameter> prms)
	{
		string text = "";
		if (!string.IsNullOrWhiteSpace(doc.ItemCode))
		{
			text += " AND \"ItemCode\" like '%' + @ItemCode + '%'";
			prms.Add(new SqlParameter("@ItemCode", Function.ToString(doc.ItemCode)));
		}
		if (!string.IsNullOrWhiteSpace(doc.ItemName))
		{
			text += " AND \"ItemName\" like '%' + @ItemName + '%'";
			prms.Add(new SqlParameter("@ItemName", Function.ToString(doc.ItemName)));
		}
		if (!string.IsNullOrWhiteSpace(doc.FrgnName))
		{
			text += " AND \"FrgnName\" like '%' + @FrgnName + '%'";
			prms.Add(new SqlParameter("@FrgnName", Function.ToString(doc.FrgnName)));
		}
		if (doc.ItmsGrpCod != -1 && doc.ItmsGrpCod != 0)
		{
			text += " AND \"ItmsGrpCod\" = @ItmsGrpCod";
			prms.Add(new SqlParameter("@ItmsGrpCod", Function.ToString(doc.ItmsGrpCod)));
		}
		if (doc.UgpEntry != 0)
		{
			text += " AND \"UgpEntry\" = @UgpEntry";
			prms.Add(new SqlParameter("@UgpEntry", Function.ToString(doc.UgpEntry)));
		}
		if (doc.ItemType == "Items")
		{
			doc.ItemType = "I";
		}
		else if (doc.ItemType == "Labor")
		{
			doc.ItemType = "L";
		}
		else if (doc.ItemType == "Travel")
		{
			doc.ItemType = "T";
		}
		if (!string.IsNullOrWhiteSpace(doc.ItemType))
		{
			text += " AND \"ItemType\" like '%' + @ItemType + '%'";
			prms.Add(new SqlParameter("@ItemType", Function.ToString(doc.ItemType)));
		}
		if (doc.ShipType != -1 && doc.ShipType != 0)
		{
			text += " AND \"ShipType\" = @ShipType";
			prms.Add(new SqlParameter("@ShipType", Function.ToString(doc.ShipType)));
		}
		if (doc.FirmCode != -1 && doc.FirmCode != 0)
		{
			text += " AND \"FirmCode\" = @FirmCode";
			prms.Add(new SqlParameter("@FirmCode", Function.ToString(doc.FirmCode)));
		}
		if (!string.IsNullOrWhiteSpace(doc.VatGroupPu))
		{
			text += " AND \"VatGroupPu\" like '%' + @VatGroupPu + '%'";
			prms.Add(new SqlParameter("@VatGroupPu", Function.ToString(doc.VatGroupPu)));
		}
		if (!string.IsNullOrWhiteSpace(doc.VatGourpSa))
		{
			text += " AND \"VatGourpSa\" like '%' + @VatGourpSa + '%'";
			prms.Add(new SqlParameter("@VatGourpSa", Function.ToString(doc.VatGourpSa)));
		}
		if (!string.IsNullOrWhiteSpace(doc.CardCode))
		{
			text += " AND \"CardCode\" like '%' + @CardCode + '%'";
			prms.Add(new SqlParameter("@CardCode", Function.ToString(doc.CardCode)));
		}
		if (!string.IsNullOrWhiteSpace(doc.ManBtchNum))
		{
			text += " AND \"ManBtchNum\" = @ManBtchNum";
			prms.Add(new SqlParameter("@ManBtchNum", Function.ToString(doc.ManBtchNum)));
		}
		if (!string.IsNullOrWhiteSpace(doc.ManSerNum))
		{
			text += " AND \"ManSerNum\" = @ManSerNum";
			prms.Add(new SqlParameter("@ManSerNum", Function.ToString(doc.ManSerNum)));
		}
		return text;
	}

	protected virtual string SearchQuery1(DataTable udfDt, ref List<SqlParameter> prms)
	{
		string text = "";
		DataTable dataTable = ExecuteDataTable("SELECT COLUMN_NAME,DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = 'OITM' AND substring(COLUMN_NAME, 1, 2) = 'U_'", CommandType.Text);
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				string text2 = Function.ToString(dataTable.Rows[i]["COLUMN_NAME"]);
				string text3 = Function.ToString(dataTable.Rows[i]["DATA_TYPE"]);
				if (!udfDt.Columns.Contains(text2))
				{
					continue;
				}
				string text4 = "@" + text2;
				switch (text3.ToLower())
				{
				case "numeric":
				{
					decimal num2 = Function.ParseDecimal(udfDt.Rows[0][text2]);
					if (num2 != 0m)
					{
						text = text + " AND \"" + text2 + "\" = " + text4;
						prms.Add(new SqlParameter(text4, num2));
					}
					continue;
				}
				case "int":
				{
					int num = Function.ParseInt(udfDt.Rows[0][text2]);
					if (num != 0)
					{
						text = text + " AND \"" + text2 + "\" = " + text4;
						prms.Add(new SqlParameter(text4, num));
					}
					continue;
				}
				case "datetime":
				{
					DateTime? dateTime = Function.ParseDateTime(udfDt.Rows[0][text2]);
					if (dateTime.HasValue)
					{
						text = text + " AND \"" + text2 + "\" = " + text4;
						prms.Add(new SqlParameter(text4, dateTime.Value));
					}
					continue;
				}
				}
				string value = Function.ToString(udfDt.Rows[0][text2]);
				if (!string.IsNullOrWhiteSpace(value))
				{
					text = text + " AND \"" + text2 + "\" like '%' + " + text4 + " + '%'";
					prms.Add(new SqlParameter(text4, value));
				}
			}
		}
		return text;
	}

	public DataTable SearchLine(string itemCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = SearchLineQuery();
			SqlParameter sqlParameter = new SqlParameter("@ItemCode", itemCode);
			result = ExecuteDataTable(query, CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string SearchLineQuery()
	{
		return "\r\nselect  @ItemCode \"ItemCode\",T1.\"ListNum\" \"PriceList\",t0.\"Price\",t0.\"Currency\",t0.\"Ovrwritten\",t0.\"Factor\",t0.\"LogInstanc\",t0.\"ObjType\",t0.\"AddPrice1\",t0.\"Currency1\",t0.\"AddPrice2\",t0.\"Currency2\",t0.\"Ovrwrite1\",t0.\"Ovrwrite2\",t0.\"BasePLNum\",t0.\"UomEntry\",t0.\"PriceType\"\r\nfrom ITM1 t0 right join opln t1 on t0.\"PriceList\"=t1.\"ListNum\" and T0.\"ItemCode\"=@ItemCode Order By T1.\"ListName\";";
	}

	public DataTable SearchOITW(string itemCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = SearchOITWQuery();
			SqlParameter sqlParameter = new SqlParameter("@ItemCode", itemCode);
			result = ExecuteDataTable(query, CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Search oitw - Doc ", ex.Message);
		}
		return result;
	}

	protected virtual string SearchOITWQuery()
	{
		return "select a.* from OITW a inner join OWHS b on a.WhsCode = b.WhsCode\r\nwhere a.ItemCode = @ItemCode And isnull(b.Inactive,'N') <> 'Y'";
	}

	public DataTable GetFirstRecord()
	{
		return GetUnitOfWork().Commons.GetFirstRecord("oitm", "ItemCode");
	}

	public DataTable GetLastRecord()
	{
		return GetUnitOfWork().Commons.GetLastRecord("oitm", "ItemCode");
	}

	public DataTable GetNextRecord(string item)
	{
		return GetUnitOfWork().Commons.GetNextRecord("oitm", "ItemCode", item);
	}

	public DataTable GetPreviousRecord(string item)
	{
		return GetUnitOfWork().Commons.GetPreviousRecord("oitm", "ItemCode", item);
	}

	public DataTable GetOITM(ItemsImpl doc)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_ItemMaster_GetOITM";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", doc.ItemCode));
			list.Add(new SqlParameter("@UserCode", doc.USER_CODE));
			list.Add(new SqlParameter("@BPLId", doc.BPLId));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetOITM ", ex.Message);
		}
		return result;
	}

	public DataTable GetItemByItemCode(string itemCode)
	{
		try
		{
			string query = "APZ_ItemMasterData_GetDoc";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@UserCode", "manager"));
			list.Add(new SqlParameter("@BPLId", 1));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable CheckExistItem(string itemCode)
	{
		try
		{
			string query = CheckExistItemQuery();
			SqlParameter sqlParameter = new SqlParameter("@ItemCode", itemCode);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
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

	protected virtual string CheckExistItemQuery()
	{
		return "Select itemcode from oitm where itemcode=@ItemCode";
	}

	public DataTable GetAllDivision()
	{
		try
		{
			string allDivisionQuery = GetAllDivisionQuery();
			List<string> list = new List<string> { "https://lh3.googleusercontent.com/y_D61BQXewUJuPaL4s9umD429hFouGxI8OSPP5d02eikI0smDOV-3N0xiwwwy2XNi-OV2V22LyHkl6faZszxk54L8QpETx8Onl1840S1gHpVjh3vuQ5eejK3gchTrviQn0KU-vC7gONMSHsgoE0A82ULmsnsYudIrYR7oVzP3IH-CnWvbLxJrs9iVq9lQzAfoy3paD4u99I4dw5a0zyBYlFSl2sf_y0FCZOa_hbFZmJ7riwClQ-1ZMNKm7CHMjqT3n27-n7RXFGKImEUVjEGwRCvRfADfDIKd4xyzmbSnXDGbl5XxdVqhgqomDmlq79Ms86CtmDqy6RDGauNKlN5UT4Oc_gO-r9pcvJYUGpDW_AWTDpLMKikdIiX_nmlw5OuMGllSv-210luAiOXMp6PlhrYRjkU-y1yL5wi4YSKDUW3ShXY2FPpLR-F-xYaWWtsd-JaPEPdXjV1n2amgHb0xYFQVss0fx-jzZSHaMf9MhfV490RH6M8IkZhlndtN357YSpvFT2hxp6DP2LQ7LXxWj11_jxDLUes5prATFbgfMi9=w1080-h642-no", "https://lh3.googleusercontent.com/Yqz89llV2VFUbM3SSsSDtucE6HcxIGbQpz51jtROa75gSNAlTWh0MqFVilCCVgCtiJKNK1sJyoXkJfVW8_Oyf_-C9n49y9if5rDJtHPx4s8aCv2Za2luy0V3FPcr2oTXn-paR9doTRmzQamNIWxaP6ibMFzf8HJgtjEQZ6LzYgO5rFKFFz-fFO1qKiIQuk41rHfWNUN5hM4LEujAYz5s3sDzk-0Cjd13pedLH8jvNbfw0OJ-DrgXrDcJa7wmQj6P3mIzk-Dg4enOqMWFoGbAL2GGOqQG6z5wY3wzodOTjEWVI9D-Qsh9TBN5hPwSmVLc1Z4Y8cTPfr0pNh-8_6GSMFPGlgdcZOFQ5_AWbKG971dFAjAYUFKS9roWpTHooxMlpqiIMOWVGMjj3SnDy3E4sxJkIOdSgyExYL_X09dMo57VnNt7cnO5wLv_4p-icl2ggNMYu_J51iSkSuCzMKeBZUu6vinfqBcrfjPyv-C62eHZAiwBfj2Vog3Y9invLF-TZ4u52-MvmSnHqNYdJwp2OPu4d6ARlW94kR4F4GGqgd8H=w1080-h640-no", "https://lh3.googleusercontent.com/ryr6cO1GnjfZref_ezxxWiI81LWoJ3oEeJsEr3t3e8ym15NRw45nng7S-A206E31JbneYq--DWeTeYkkCvm9TJBbuCj-4wghOFKwdwsdtykbJvxOFPn4SD_eixUZbQoRuhzjxHpiAPmygVnAVmopKeb0wRdvBR9U3tt75cNN5pZmxxdNxizMNd_L27MAhMm9fBqbasy53VdcAP3-cP4X7NAjUCFu-7rktIngLydr98jsbPHOHrT3m5eSwHvypLHKZNXQ2sif-FLgeLw4oajB7HeFZyuLZ9ISJc2TnbUF_JXUHeOJjp7t_TDkfEibSWrdk_yrPhQ_T-VfrFRuzCCbeVUjPLsfyOodRwK0vhYbrJ2q6rHBMX6VPrORpbmDmRt0rwmvvJoCr7QkZVyIjuOBxhU39zYLX5W8Lm7_HV-6K7EcfQOnawjE5It6QpDC6N0iJ6i2rf9oZqa5NfceWJqz3N575CdxdMjrFrAbyjwSvIhFUoiepvuNZY2QAXxFFqtAx1actQDNgeZw0Jc7tKm9dn1qpayGMDqrRVMI0i1zQu05=w1080-h642-no", "https://lh3.googleusercontent.com/a1PMdfps9Vu5YpYdJR5WHTeUJZ4w158fweZhECZX9pGDWrAFJnT4Clj7IRKT70jjiuFhg7sWW6VGjxEHRjC4_MS2AfCW7jS0KNTGfXumD33AFB965pD5RZ7p85gnYsDCFRC8rO9rUw37uwWo6B4BlBojSKBJWlH1tuaGSiIw7KreXklKEsG4xd9Bd1mwPw8N9IRsKnHMgVo_u2d3q7goLggB7JhYHUVYQSOJi0ac7vcNcz-laqCR7ieKl_wRg6lPGxSft89IeXsLyJxKsiikDeFztFxAd-Ok9TxRdKddXi8ovsFkuvJJGX4DSBR-TmSN44JugjoVHoXRYxK4ekzWyr4JEIUQbvdaEkNAe1n_84LeJuKJ_PkFFQAaAqVQiHVHQQ3H7BuGNRgX15cBcM8oQAxUIWfa_UqHk4kIu_X2ESo6qaMYapTSrfKGLjpxsaZriDkPGG8FwVuvUfVH66z3iH9mk1d5g8XCzChX0yR45gydBsjH3RxWZ73TSPzPJ4_26I124wvU6yWMalghkqWcOg-sY9XhuBJlOBShDCxtgxnf=w1080-h640-no", "https://lh3.googleusercontent.com/3rReSGLiu-S0g7pNzbT0RGrGzRlDbwOnkfXEB2UkyZvLt8vRP6aOq5RkA0TpoLe-mBMmt4aVkza6iEoHlFNijShDQ16NzHnIhls9x8XD7fxrdNqUI12_V9i8vHusse0VX2G6SGan_u_BzA30foXV-r58l3buMJk8bMKpTz3MObuUwBXezsFp_S-3zPCWvEto-HEAt1CeQFZxCIBS27MbsR6SUA-4MWQ7z6MZSao1-gfZK3Ao4NLhENwAKjQuELZyhEgPTEquU9IbDmNJuo6Kpvg3xfj9HExB86FtYzJYQPScl8hyl7KHtv_CWpNXMrknJW4afrW1wvB0vbLKh8Ur4rvkKqiNJvkQAAqHUlJ6zWx22lPJqOtIKEQmBYOSBDVBLkcqvpDGq-gSir0my3r0Rzq8AR0scfl6jy9d36moZE9rYngqfLZwP_PqHj6UjtDcgjZAsXlzgsTFeousgUAAyUhKZ25tMXDwaRFygwbop3THtspAoF74kXxq0Y7xiVaWiZ8lg29EsYJ9kSpXT2BX-44zCiY8TJlLDeTOcf3KZB_i=w1080-h640-no", "https://lh3.googleusercontent.com/wMF7ZU-Zz_6GgIgoowYur4akhUipOfP0soF4coQhRy3nk2BAXcE4iquki954C7oXqGghMfYBTc7R7dlzcLKbRylHckKMp0_OGhQvGEieBiXhZallRz6LURZv7yiC5UDwtB9AouxPYfRwxrGKBFs0JoXt10KP0-KtKzNZKWDIj9faNklmPbxHVUKJH-pwM7H2rbpEZGAFGgbFl7yLX9518TjhCMzksZh7p0LkLZL3jVpXGrxq9DNIvkVt0jdEBWosYhchAZTh8QaRotGtOu800Uhhcd84l4_r8KobRbZYJ6692SE4g7U7mrNuVh06opjkhvg_b26ud9v5ZFtTJhaTkICAkj6jMeR8aCzqnFaaiafEZyLoxJ4qV_m1q-UZOtUTcCePCVUP-7Ibml8E8VrG9Cy8sySDV2plim9IophOxHa63SLuPv9_-_91UVP_RFe-upt-NvSuDbzwMW9wdUhy3tlO2I6N82rOl2fMJnWaI2UUdf37GAqmtOEEu0G-ELqTAN06t1CLgdCRlHJzbOgM4DCB5KxW5ZnuRQDPHub1-ajg=w1080-h640-no", "https://lh3.googleusercontent.com/m9hfJGWAI7OqWLNd9W-lsEBz1H9M3FL6Bck8-pK_Qy9EHzg-ts5fnU44ST8lu80_xsIUJMEcTCsqQ7Dsmq1V6rDNJx2yosvFNy3nYsc8_Z0eyq7cD0J8411dkeE-YCz4hadWbRlg5YAgSsJaGy3GJ_QId7vKNgIrxn7C7QE4YAsECnFWefhihfTImuYp2uVvlAVZVKpjk5jGj4uk-tuike5e4zIro2y7bi7XGa5w03JRYnoeH_nuKOCZ-5eh-M3k5dAMd7lsvl1-u89p9j0J0MnTzR_y7OcdfoPr0cwMVRJf6vVYkEI92ZkSiR-M9NwtigEnpAoQ-Sb5ewFz2JBaIsRvxNjqCwwvHlAz5XzxpojbYtDG0QjS8GfUnXEZD_rvugjt0xyZQMS6VF2iuM5sKlsrXU8YrWvZlh_7wYkQMp6mVYfTGjK4koY0WcsLFsi_ufxH_8cytEexJwwIEm-v6uQzCW8rzIzW4SsfP0OBnZ0bhDg1sZNd3yNlyf310X2a3C4TFI3nq2dF4qXtGtY-l8pxJqeijk2ecT5gI5m759_Y=w1080-h485-no" };
			DataTable dataTable = ExecuteDataTable(allDivisionQuery, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				int num = 0;
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (num > 7)
					{
						num = 0;
					}
					dataTable.Rows[i]["Url"] = list[num];
				}
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetAllDivisionQuery()
	{
		return "SELECT Code,Name,'' Url  FROM [dbo].[@ARGNS_DIV]\r\n                            WHERE U_type='01'";
	}

	public DataTable GetCategory(string divCode)
	{
		try
		{
			string categoryQuery = GetCategoryQuery();
			DataTable dataTable = ExecuteDataTable(categoryQuery, CommandType.Text, new SqlParameter("@DivCode", divCode));
			List<string> list = new List<string> { "https://lh3.googleusercontent.com/zBSPU8Y60FuSprkm3-txVkcnPPRXPo3IlayBMwgS16LAAZBmKSv-_pYs_KY24SxauMyP3tnTEOk7X7_poTpkMhiMkxs8l_7sa-MFW0gWLcWD3QxwnnUGP99szGLpFCWEuSNu6Eu7kgPbusrXlhXnkmGz2yTwG4TAdFl99MzjJsixASJGZ-RQEmXrarnQUsa5i3K1HD4KP8-nzJVeMcDbr_Wqb_AadaxHFRx7dANAadGCpp1WFAol3XS-mgjqg9CV5R-4lMgSAhdsNewykV-dl5AinjhB-PE9F1KA6Ae3mmylljjS1iWrDiSFGyrQ3wvIl_d3UkJcv992I0-nZKdVaHNWvFecl0Ky2UqJfxanxzkQy7KldDM4t6iNmrNoJXiMIhkLkNI2OGKAPRzi6zbNYbUaHs8o6XTrcbOTP8s5-eG7-FThJ_ISOFsdar7pW84JKv6oHZt2myh5Y7f5Dx7wXhCgk9AJWWS_FVVq_jZI_CNsH6RdbP-9SlkAzIwm-4znKXocpBGRaPSyqlOjC9iD2W-dgnpA_FI4cTXD1t4MBw2_=w521-h677-no", "https://lh3.googleusercontent.com/JXHgkoIQxqFWKoPa41B0sYjmE3NvjmrQC0U1-RdXpUJ5Of-9SCGopHUBvqKv6TGMBqFASF4Ogh0J5Y7zBquZoz9kUifSVpnxm1rVvlWaZElFXgCCUVj_cxAvN_D47hFqZGCa7ed3UM9vUC_7FzMj1l816ENKVkzgujDO_vobZR4u8RrEHGrIf54Es2HF_IO-1awAUvZ7L2OGLRlkqbiLNf1nY8T-1_Q3lMYunN6ZL024qpJ5dkoSQuHCDSt7rgGtXk-1RjHPNcscjeyCYnovayg_1aRl3zCjFIb13whvtxFNUCXQ8taVRwaE5Bfh9fWwXRhlFDgrP2QvVeTb68jsylRuxMU_T2QWWCnAKacypXIlu5oRz8yH-NxIMHu3m5107Eehrjv4Sn-plPaFs3IBwow2AXiqeVGJG3bqwJFWfXve7F3CCMmGtKOvXIJoff9orr1M1uE4zCbOxzdH-UVVsX5EsBOzlPFMX74sguJRmafxNsEPfQNrMjdALiMA04gteoaJD9PtL9XSHGDREXvvDupQ_4dugSLtvg47VGHS27vx=w495-h660-no", "https://lh3.googleusercontent.com/UG-gI_kjqlICrgrXkxXvd3Smnm3wNx1HqFhQlwp7ROGVlpoQHp1UT__N8W_Bg8uGQ2bcr_FZHAHLho3pxDnT4L38DHV1E6Qptxxa5bcuCit3dPlZoXZSTeB_2jijTYLm0pCRp06NNbG9WQ6NJwRvoMiZx-mLqf_eILyJFK91QPOki9WwBWCTnAzeutMcD3TEw40YZRpVru92ukJg4HfNk6NIr2DxBUC1zUMEWDXKCudEClAB1BHjjbs_odteolbAKqPj_fr5A2AUFYU2ra-PIqie45iCpw1bFV_e180w-BCU7pyZtJnHip1rZAFAtD5eyUc4EjHl49GpArPDHs97ZP7_lR43ePXwZ8a6Pod27hBOvFQ1RNfVnWChF7wPbJNt1jkPKhc0vfQkciRsYb34fPtZHx5d6iBY_muspeEX1CEPH6TTB3nCR68yJPVkIyfKRZqcumntIocgMb6fdiZq2fCzYspy6Ihpvs9uokY95hOF72KGjW90I1mFaR9KUgkksCN-D5oohrBZb6OieHWhxBUqrRzZsDEgiNJ7rPv5oh83=w495-h660-no", "https://lh3.googleusercontent.com/J6BZdH0l-dQJqaAlDdMQl3eARTxoyXcmHvCoqQWuvP8qvpjevZ6pDU_HIVtW62pQy-kI2O1KBEITolbV78H0g6fckTy8egD99kcVkFmmYSYlTR80Ed7jrSBsPyztW98JSNTZtnmmml_BWdshuzQAvjEjO9NtmTv5zvj_aT7dvhrrbB1KPl_6WuZvcYErNZKvukjAU2jy0HtHhyIZHVz8UFuvKEjridM6rzhf-0Hk1dlsUWEDZVANtkU6D6hSMGaVKib7liC3FgN60SrAhcYDQTFSznu8pOeIIZWDDbYI1eBhkOYARLlblht5RvglGXOxZuM8ewIy49JEXs8bkc3wrzSbkUAHqSAMh1o_LbjiQv9M6MT1AQ2nFOzXx9zBlgrwDLTMdAxBMzNzTt7yepVUEBdEVW3f7Yp2cf-vUQz1oU5LpnjBRGI_nLHT0iT0MwQhoZs4XzQIzGcsaHh6X61aZ-1Q1kXyPsbu7bxm4PNguIclT-n9vSFhMKLh-nBrZvXb_Xud-pmjobY0Scw4RZ1t1geESm2-FvOgYTOu5Hhpfu8D=w495-h660-no", "https://lh3.googleusercontent.com/NrmuTupTbOZdh6ncIrW2jl3PZbhurSx4RASmBNPa_wKokpdYYJ2eCHuNYUXIY8RHhkd7AfHqGK4xYft4rKrbwvfhd2BrebNWOzGMieV1QX6RJ4RY4yc61s8CeJEaRDF4rR0agnsajp0Uex_z3yupirkPudGNN99suKxPZM7rg-UZrImlTJu4pVXXqi0AM-Yz4AtPBQpHrf7l8GdJE4pIljnyVoflVc_VtxpBlXinA0vQFIxo6dn-APy_xfih0hdjJSssqf1Gl1X-clizJrYHh9NB1Se6idTDvi6YEZ_VsGiV69Omyw2NQiSKxcsGR_wtLtaxzzhVKwVEXu2fkj9sJ8Af3ILszW7vV0ApIsw6AOWbOJgdGiXE9yvNj650PkdUAbREFuBzdt0VlAFwLnu8Bt3Ns6jRgRlPYXCZDRPFMngT6rQefXDM3E7OIRfF85k2LRk_uceh-bdRN4qu9FknfTi4HTYs_RHzkj5JdVyUgwjt3nXysABTtUSMk2jIOcODJknbZ_u4zBW8hsQhVnxcmaBB8VnT6nkehRPRpy8myS1O=w495-h660-no", "https://lh3.googleusercontent.com/i9y3pIEY5qGV2czDGsLskvFJYPzgm0RR1XeSOZN6taznB8o3odOIIv1qb0ZL1lyyzWShJLhShV7iQjRA5mqEJLKRowkkxaZ9y1LHCRwYpPRqIAtYsZJSXFPPfiW1lGyn6xiNohAvk8AVZpO5vcjUSN1GzpPyIqukrGP3laSA0FL5WgZN58s1ek0KRPSHYuqlfVp1y8yIyx7xeFoqTsSybJ4pBmw4d4Nf-TZO31Vz6J0XyrJRa2CvWSRLqDXn4fLS0i3B_rdQIRfNeq48AfuZYfS9TzCX6UkBthoeBCB2i8ZK5r_kdcW_4Yl8JtNmv556C0gwOtbXLOa7HAFLNQ86LeKW40NwT-be4MH4Ki3_z8c1nub5UMukxRN3v9tvKx2azFLDkhZ7Zc80ZD2a7qu9BbeorvkeTPJVnzd1PJhyAuxER6yW_-5VJngG-6xEF7Q_vungIsxzyeAvQAs0SEnQdTLL7V5M6y05vtwr3MEW4KkIrMRXHl0gSUWlnMtgzovjTNlURCARy6waZhs3HaWX81p4DG-e-WfawRZvBQhp_LFT=w495-h660-no", "https://lh3.googleusercontent.com/qkE7YA0UHJ15kG5cy9T0BYsx5LPVa7KszPjy-6y-BIGnferfv_WaCGkO82PdAcvlC2JiTZy8E6T4bn1vHmFt6iMBNZN0NjmFRiCiQGJEih5anZcoMb9cAWwZYjret65xKMV4LUKOu0QmFS-ytvRVcnPzlLpDxnAiCdVk0dknlE36mFrDSEMQh7_HB-touGxAEzB_UPwG6aceCzXer48uFqo4frs_735p7rHF9V5cFTRZg8a62QdE2wJ8yLqaQELMoaVdeWntOws5fTPKm-Yzh2SfzK7LB3HCc_fjRP_J9hrUU64aG67kz2IWVlyPPoxKxXcqu3f2saAJIKJdJpmqZfwWoQROVRZuZQbrUKjTz156uAgHIZBBXMM4KzhUEEIYIQqXDzhtuLde9iEw8HbMIgSa9fI5PVEcb5gMI78OSvsLPzy6Neh0cjo6o33uv8EBWYme5LleikEshMk_8erJnbYWuS17ZmCflw9eP8xK_hV1-Uvx120EChfhRJRlgKHAcaRlw9Xz0jZ6ri8BoOq7luuBSSAL53GyW2Wbil13KWgT=w495-h660-no" };
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				int num = 0;
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (num > 7)
					{
						num = 0;
					}
					dataTable.Rows[i]["Url"] = list[num];
				}
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetCategoryQuery()
	{
		return "SELECT DISTINCT U_ARGNS_M_GROUP CatId,b.Name Cat,'' Url\t FROM oitm a LEFT JOIN [dbo].[@ARGNS_MODELGRP] b ON a.U_ARGNS_M_GROUP=b.Code\r\n                WHERE U_ARGNS_DIV=@DivCode AND a.validFor='Y' OR (a.validFor='N' AND a.frozenFor='N')";
	}

	public DataTable GetModel(string group)
	{
		try
		{
			string modelQuery = GetModelQuery();
			DataTable dataTable = ExecuteDataTable(modelQuery, CommandType.Text, new SqlParameter("@Group", group));
			List<string> list = new List<string> { "https://lh3.googleusercontent.com/zBSPU8Y60FuSprkm3-txVkcnPPRXPo3IlayBMwgS16LAAZBmKSv-_pYs_KY24SxauMyP3tnTEOk7X7_poTpkMhiMkxs8l_7sa-MFW0gWLcWD3QxwnnUGP99szGLpFCWEuSNu6Eu7kgPbusrXlhXnkmGz2yTwG4TAdFl99MzjJsixASJGZ-RQEmXrarnQUsa5i3K1HD4KP8-nzJVeMcDbr_Wqb_AadaxHFRx7dANAadGCpp1WFAol3XS-mgjqg9CV5R-4lMgSAhdsNewykV-dl5AinjhB-PE9F1KA6Ae3mmylljjS1iWrDiSFGyrQ3wvIl_d3UkJcv992I0-nZKdVaHNWvFecl0Ky2UqJfxanxzkQy7KldDM4t6iNmrNoJXiMIhkLkNI2OGKAPRzi6zbNYbUaHs8o6XTrcbOTP8s5-eG7-FThJ_ISOFsdar7pW84JKv6oHZt2myh5Y7f5Dx7wXhCgk9AJWWS_FVVq_jZI_CNsH6RdbP-9SlkAzIwm-4znKXocpBGRaPSyqlOjC9iD2W-dgnpA_FI4cTXD1t4MBw2_=w521-h677-no", "https://lh3.googleusercontent.com/JXHgkoIQxqFWKoPa41B0sYjmE3NvjmrQC0U1-RdXpUJ5Of-9SCGopHUBvqKv6TGMBqFASF4Ogh0J5Y7zBquZoz9kUifSVpnxm1rVvlWaZElFXgCCUVj_cxAvN_D47hFqZGCa7ed3UM9vUC_7FzMj1l816ENKVkzgujDO_vobZR4u8RrEHGrIf54Es2HF_IO-1awAUvZ7L2OGLRlkqbiLNf1nY8T-1_Q3lMYunN6ZL024qpJ5dkoSQuHCDSt7rgGtXk-1RjHPNcscjeyCYnovayg_1aRl3zCjFIb13whvtxFNUCXQ8taVRwaE5Bfh9fWwXRhlFDgrP2QvVeTb68jsylRuxMU_T2QWWCnAKacypXIlu5oRz8yH-NxIMHu3m5107Eehrjv4Sn-plPaFs3IBwow2AXiqeVGJG3bqwJFWfXve7F3CCMmGtKOvXIJoff9orr1M1uE4zCbOxzdH-UVVsX5EsBOzlPFMX74sguJRmafxNsEPfQNrMjdALiMA04gteoaJD9PtL9XSHGDREXvvDupQ_4dugSLtvg47VGHS27vx=w495-h660-no", "https://lh3.googleusercontent.com/UG-gI_kjqlICrgrXkxXvd3Smnm3wNx1HqFhQlwp7ROGVlpoQHp1UT__N8W_Bg8uGQ2bcr_FZHAHLho3pxDnT4L38DHV1E6Qptxxa5bcuCit3dPlZoXZSTeB_2jijTYLm0pCRp06NNbG9WQ6NJwRvoMiZx-mLqf_eILyJFK91QPOki9WwBWCTnAzeutMcD3TEw40YZRpVru92ukJg4HfNk6NIr2DxBUC1zUMEWDXKCudEClAB1BHjjbs_odteolbAKqPj_fr5A2AUFYU2ra-PIqie45iCpw1bFV_e180w-BCU7pyZtJnHip1rZAFAtD5eyUc4EjHl49GpArPDHs97ZP7_lR43ePXwZ8a6Pod27hBOvFQ1RNfVnWChF7wPbJNt1jkPKhc0vfQkciRsYb34fPtZHx5d6iBY_muspeEX1CEPH6TTB3nCR68yJPVkIyfKRZqcumntIocgMb6fdiZq2fCzYspy6Ihpvs9uokY95hOF72KGjW90I1mFaR9KUgkksCN-D5oohrBZb6OieHWhxBUqrRzZsDEgiNJ7rPv5oh83=w495-h660-no", "https://lh3.googleusercontent.com/J6BZdH0l-dQJqaAlDdMQl3eARTxoyXcmHvCoqQWuvP8qvpjevZ6pDU_HIVtW62pQy-kI2O1KBEITolbV78H0g6fckTy8egD99kcVkFmmYSYlTR80Ed7jrSBsPyztW98JSNTZtnmmml_BWdshuzQAvjEjO9NtmTv5zvj_aT7dvhrrbB1KPl_6WuZvcYErNZKvukjAU2jy0HtHhyIZHVz8UFuvKEjridM6rzhf-0Hk1dlsUWEDZVANtkU6D6hSMGaVKib7liC3FgN60SrAhcYDQTFSznu8pOeIIZWDDbYI1eBhkOYARLlblht5RvglGXOxZuM8ewIy49JEXs8bkc3wrzSbkUAHqSAMh1o_LbjiQv9M6MT1AQ2nFOzXx9zBlgrwDLTMdAxBMzNzTt7yepVUEBdEVW3f7Yp2cf-vUQz1oU5LpnjBRGI_nLHT0iT0MwQhoZs4XzQIzGcsaHh6X61aZ-1Q1kXyPsbu7bxm4PNguIclT-n9vSFhMKLh-nBrZvXb_Xud-pmjobY0Scw4RZ1t1geESm2-FvOgYTOu5Hhpfu8D=w495-h660-no", "https://lh3.googleusercontent.com/NrmuTupTbOZdh6ncIrW2jl3PZbhurSx4RASmBNPa_wKokpdYYJ2eCHuNYUXIY8RHhkd7AfHqGK4xYft4rKrbwvfhd2BrebNWOzGMieV1QX6RJ4RY4yc61s8CeJEaRDF4rR0agnsajp0Uex_z3yupirkPudGNN99suKxPZM7rg-UZrImlTJu4pVXXqi0AM-Yz4AtPBQpHrf7l8GdJE4pIljnyVoflVc_VtxpBlXinA0vQFIxo6dn-APy_xfih0hdjJSssqf1Gl1X-clizJrYHh9NB1Se6idTDvi6YEZ_VsGiV69Omyw2NQiSKxcsGR_wtLtaxzzhVKwVEXu2fkj9sJ8Af3ILszW7vV0ApIsw6AOWbOJgdGiXE9yvNj650PkdUAbREFuBzdt0VlAFwLnu8Bt3Ns6jRgRlPYXCZDRPFMngT6rQefXDM3E7OIRfF85k2LRk_uceh-bdRN4qu9FknfTi4HTYs_RHzkj5JdVyUgwjt3nXysABTtUSMk2jIOcODJknbZ_u4zBW8hsQhVnxcmaBB8VnT6nkehRPRpy8myS1O=w495-h660-no", "https://lh3.googleusercontent.com/i9y3pIEY5qGV2czDGsLskvFJYPzgm0RR1XeSOZN6taznB8o3odOIIv1qb0ZL1lyyzWShJLhShV7iQjRA5mqEJLKRowkkxaZ9y1LHCRwYpPRqIAtYsZJSXFPPfiW1lGyn6xiNohAvk8AVZpO5vcjUSN1GzpPyIqukrGP3laSA0FL5WgZN58s1ek0KRPSHYuqlfVp1y8yIyx7xeFoqTsSybJ4pBmw4d4Nf-TZO31Vz6J0XyrJRa2CvWSRLqDXn4fLS0i3B_rdQIRfNeq48AfuZYfS9TzCX6UkBthoeBCB2i8ZK5r_kdcW_4Yl8JtNmv556C0gwOtbXLOa7HAFLNQ86LeKW40NwT-be4MH4Ki3_z8c1nub5UMukxRN3v9tvKx2azFLDkhZ7Zc80ZD2a7qu9BbeorvkeTPJVnzd1PJhyAuxER6yW_-5VJngG-6xEF7Q_vungIsxzyeAvQAs0SEnQdTLL7V5M6y05vtwr3MEW4KkIrMRXHl0gSUWlnMtgzovjTNlURCARy6waZhs3HaWX81p4DG-e-WfawRZvBQhp_LFT=w495-h660-no", "https://lh3.googleusercontent.com/qkE7YA0UHJ15kG5cy9T0BYsx5LPVa7KszPjy-6y-BIGnferfv_WaCGkO82PdAcvlC2JiTZy8E6T4bn1vHmFt6iMBNZN0NjmFRiCiQGJEih5anZcoMb9cAWwZYjret65xKMV4LUKOu0QmFS-ytvRVcnPzlLpDxnAiCdVk0dknlE36mFrDSEMQh7_HB-touGxAEzB_UPwG6aceCzXer48uFqo4frs_735p7rHF9V5cFTRZg8a62QdE2wJ8yLqaQELMoaVdeWntOws5fTPKm-Yzh2SfzK7LB3HCc_fjRP_J9hrUU64aG67kz2IWVlyPPoxKxXcqu3f2saAJIKJdJpmqZfwWoQROVRZuZQbrUKjTz156uAgHIZBBXMM4KzhUEEIYIQqXDzhtuLde9iEw8HbMIgSa9fI5PVEcb5gMI78OSvsLPzy6Neh0cjo6o33uv8EBWYme5LleikEshMk_8erJnbYWuS17ZmCflw9eP8xK_hV1-Uvx120EChfhRJRlgKHAcaRlw9Xz0jZ6ri8BoOq7luuBSSAL53GyW2Wbil13KWgT=w495-h660-no" };
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				int num = 0;
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (num > 7)
					{
						num = 0;
					}
					dataTable.Rows[i]["Url"] = list[num];
				}
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string GetModelQuery()
	{
		return "SELECT DISTINCT\t U_ARGNS_MOD Code,b.U_ModDesc Name,'' Url FROM oitm a LEFT JOIN [@ARGNS_MODEL] b ON a.U_ARGNS_MOD=b.U_ModCode WHERE U_ARGNS_DIV=1 AND U_ARGNS_M_GROUP=@Group";
	}

	public DataTable GetItem(string model, string color, string size)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("android_getItem", CommandType.StoredProcedure, new SqlParameter("@Model", model), new SqlParameter("@Color", color), new SqlParameter("@Size", size));
			List<string> list = new List<string> { "https://lh3.googleusercontent.com/zBSPU8Y60FuSprkm3-txVkcnPPRXPo3IlayBMwgS16LAAZBmKSv-_pYs_KY24SxauMyP3tnTEOk7X7_poTpkMhiMkxs8l_7sa-MFW0gWLcWD3QxwnnUGP99szGLpFCWEuSNu6Eu7kgPbusrXlhXnkmGz2yTwG4TAdFl99MzjJsixASJGZ-RQEmXrarnQUsa5i3K1HD4KP8-nzJVeMcDbr_Wqb_AadaxHFRx7dANAadGCpp1WFAol3XS-mgjqg9CV5R-4lMgSAhdsNewykV-dl5AinjhB-PE9F1KA6Ae3mmylljjS1iWrDiSFGyrQ3wvIl_d3UkJcv992I0-nZKdVaHNWvFecl0Ky2UqJfxanxzkQy7KldDM4t6iNmrNoJXiMIhkLkNI2OGKAPRzi6zbNYbUaHs8o6XTrcbOTP8s5-eG7-FThJ_ISOFsdar7pW84JKv6oHZt2myh5Y7f5Dx7wXhCgk9AJWWS_FVVq_jZI_CNsH6RdbP-9SlkAzIwm-4znKXocpBGRaPSyqlOjC9iD2W-dgnpA_FI4cTXD1t4MBw2_=w521-h677-no", "https://lh3.googleusercontent.com/JXHgkoIQxqFWKoPa41B0sYjmE3NvjmrQC0U1-RdXpUJ5Of-9SCGopHUBvqKv6TGMBqFASF4Ogh0J5Y7zBquZoz9kUifSVpnxm1rVvlWaZElFXgCCUVj_cxAvN_D47hFqZGCa7ed3UM9vUC_7FzMj1l816ENKVkzgujDO_vobZR4u8RrEHGrIf54Es2HF_IO-1awAUvZ7L2OGLRlkqbiLNf1nY8T-1_Q3lMYunN6ZL024qpJ5dkoSQuHCDSt7rgGtXk-1RjHPNcscjeyCYnovayg_1aRl3zCjFIb13whvtxFNUCXQ8taVRwaE5Bfh9fWwXRhlFDgrP2QvVeTb68jsylRuxMU_T2QWWCnAKacypXIlu5oRz8yH-NxIMHu3m5107Eehrjv4Sn-plPaFs3IBwow2AXiqeVGJG3bqwJFWfXve7F3CCMmGtKOvXIJoff9orr1M1uE4zCbOxzdH-UVVsX5EsBOzlPFMX74sguJRmafxNsEPfQNrMjdALiMA04gteoaJD9PtL9XSHGDREXvvDupQ_4dugSLtvg47VGHS27vx=w495-h660-no", "https://lh3.googleusercontent.com/UG-gI_kjqlICrgrXkxXvd3Smnm3wNx1HqFhQlwp7ROGVlpoQHp1UT__N8W_Bg8uGQ2bcr_FZHAHLho3pxDnT4L38DHV1E6Qptxxa5bcuCit3dPlZoXZSTeB_2jijTYLm0pCRp06NNbG9WQ6NJwRvoMiZx-mLqf_eILyJFK91QPOki9WwBWCTnAzeutMcD3TEw40YZRpVru92ukJg4HfNk6NIr2DxBUC1zUMEWDXKCudEClAB1BHjjbs_odteolbAKqPj_fr5A2AUFYU2ra-PIqie45iCpw1bFV_e180w-BCU7pyZtJnHip1rZAFAtD5eyUc4EjHl49GpArPDHs97ZP7_lR43ePXwZ8a6Pod27hBOvFQ1RNfVnWChF7wPbJNt1jkPKhc0vfQkciRsYb34fPtZHx5d6iBY_muspeEX1CEPH6TTB3nCR68yJPVkIyfKRZqcumntIocgMb6fdiZq2fCzYspy6Ihpvs9uokY95hOF72KGjW90I1mFaR9KUgkksCN-D5oohrBZb6OieHWhxBUqrRzZsDEgiNJ7rPv5oh83=w495-h660-no" };
			string value = string.Join(";", list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					dataTable.Rows[i]["Url"] = value;
				}
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetModelByDivision(string divCode, int? pageNum, int? pageSize, string catId, string model)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("android_GetModelByDivision", CommandType.StoredProcedure, new SqlParameter("@divCode", divCode), new SqlParameter("@PageNum", pageNum), new SqlParameter("@PageSize", pageSize), new SqlParameter("@CatId", catId), new SqlParameter("@Model", model));
			List<string> list = new List<string> { "https://lh3.googleusercontent.com/zBSPU8Y60FuSprkm3-txVkcnPPRXPo3IlayBMwgS16LAAZBmKSv-_pYs_KY24SxauMyP3tnTEOk7X7_poTpkMhiMkxs8l_7sa-MFW0gWLcWD3QxwnnUGP99szGLpFCWEuSNu6Eu7kgPbusrXlhXnkmGz2yTwG4TAdFl99MzjJsixASJGZ-RQEmXrarnQUsa5i3K1HD4KP8-nzJVeMcDbr_Wqb_AadaxHFRx7dANAadGCpp1WFAol3XS-mgjqg9CV5R-4lMgSAhdsNewykV-dl5AinjhB-PE9F1KA6Ae3mmylljjS1iWrDiSFGyrQ3wvIl_d3UkJcv992I0-nZKdVaHNWvFecl0Ky2UqJfxanxzkQy7KldDM4t6iNmrNoJXiMIhkLkNI2OGKAPRzi6zbNYbUaHs8o6XTrcbOTP8s5-eG7-FThJ_ISOFsdar7pW84JKv6oHZt2myh5Y7f5Dx7wXhCgk9AJWWS_FVVq_jZI_CNsH6RdbP-9SlkAzIwm-4znKXocpBGRaPSyqlOjC9iD2W-dgnpA_FI4cTXD1t4MBw2_=w521-h677-no", "https://lh3.googleusercontent.com/JXHgkoIQxqFWKoPa41B0sYjmE3NvjmrQC0U1-RdXpUJ5Of-9SCGopHUBvqKv6TGMBqFASF4Ogh0J5Y7zBquZoz9kUifSVpnxm1rVvlWaZElFXgCCUVj_cxAvN_D47hFqZGCa7ed3UM9vUC_7FzMj1l816ENKVkzgujDO_vobZR4u8RrEHGrIf54Es2HF_IO-1awAUvZ7L2OGLRlkqbiLNf1nY8T-1_Q3lMYunN6ZL024qpJ5dkoSQuHCDSt7rgGtXk-1RjHPNcscjeyCYnovayg_1aRl3zCjFIb13whvtxFNUCXQ8taVRwaE5Bfh9fWwXRhlFDgrP2QvVeTb68jsylRuxMU_T2QWWCnAKacypXIlu5oRz8yH-NxIMHu3m5107Eehrjv4Sn-plPaFs3IBwow2AXiqeVGJG3bqwJFWfXve7F3CCMmGtKOvXIJoff9orr1M1uE4zCbOxzdH-UVVsX5EsBOzlPFMX74sguJRmafxNsEPfQNrMjdALiMA04gteoaJD9PtL9XSHGDREXvvDupQ_4dugSLtvg47VGHS27vx=w495-h660-no", "https://lh3.googleusercontent.com/UG-gI_kjqlICrgrXkxXvd3Smnm3wNx1HqFhQlwp7ROGVlpoQHp1UT__N8W_Bg8uGQ2bcr_FZHAHLho3pxDnT4L38DHV1E6Qptxxa5bcuCit3dPlZoXZSTeB_2jijTYLm0pCRp06NNbG9WQ6NJwRvoMiZx-mLqf_eILyJFK91QPOki9WwBWCTnAzeutMcD3TEw40YZRpVru92ukJg4HfNk6NIr2DxBUC1zUMEWDXKCudEClAB1BHjjbs_odteolbAKqPj_fr5A2AUFYU2ra-PIqie45iCpw1bFV_e180w-BCU7pyZtJnHip1rZAFAtD5eyUc4EjHl49GpArPDHs97ZP7_lR43ePXwZ8a6Pod27hBOvFQ1RNfVnWChF7wPbJNt1jkPKhc0vfQkciRsYb34fPtZHx5d6iBY_muspeEX1CEPH6TTB3nCR68yJPVkIyfKRZqcumntIocgMb6fdiZq2fCzYspy6Ihpvs9uokY95hOF72KGjW90I1mFaR9KUgkksCN-D5oohrBZb6OieHWhxBUqrRzZsDEgiNJ7rPv5oh83=w495-h660-no", "https://lh3.googleusercontent.com/J6BZdH0l-dQJqaAlDdMQl3eARTxoyXcmHvCoqQWuvP8qvpjevZ6pDU_HIVtW62pQy-kI2O1KBEITolbV78H0g6fckTy8egD99kcVkFmmYSYlTR80Ed7jrSBsPyztW98JSNTZtnmmml_BWdshuzQAvjEjO9NtmTv5zvj_aT7dvhrrbB1KPl_6WuZvcYErNZKvukjAU2jy0HtHhyIZHVz8UFuvKEjridM6rzhf-0Hk1dlsUWEDZVANtkU6D6hSMGaVKib7liC3FgN60SrAhcYDQTFSznu8pOeIIZWDDbYI1eBhkOYARLlblht5RvglGXOxZuM8ewIy49JEXs8bkc3wrzSbkUAHqSAMh1o_LbjiQv9M6MT1AQ2nFOzXx9zBlgrwDLTMdAxBMzNzTt7yepVUEBdEVW3f7Yp2cf-vUQz1oU5LpnjBRGI_nLHT0iT0MwQhoZs4XzQIzGcsaHh6X61aZ-1Q1kXyPsbu7bxm4PNguIclT-n9vSFhMKLh-nBrZvXb_Xud-pmjobY0Scw4RZ1t1geESm2-FvOgYTOu5Hhpfu8D=w495-h660-no", "https://lh3.googleusercontent.com/NrmuTupTbOZdh6ncIrW2jl3PZbhurSx4RASmBNPa_wKokpdYYJ2eCHuNYUXIY8RHhkd7AfHqGK4xYft4rKrbwvfhd2BrebNWOzGMieV1QX6RJ4RY4yc61s8CeJEaRDF4rR0agnsajp0Uex_z3yupirkPudGNN99suKxPZM7rg-UZrImlTJu4pVXXqi0AM-Yz4AtPBQpHrf7l8GdJE4pIljnyVoflVc_VtxpBlXinA0vQFIxo6dn-APy_xfih0hdjJSssqf1Gl1X-clizJrYHh9NB1Se6idTDvi6YEZ_VsGiV69Omyw2NQiSKxcsGR_wtLtaxzzhVKwVEXu2fkj9sJ8Af3ILszW7vV0ApIsw6AOWbOJgdGiXE9yvNj650PkdUAbREFuBzdt0VlAFwLnu8Bt3Ns6jRgRlPYXCZDRPFMngT6rQefXDM3E7OIRfF85k2LRk_uceh-bdRN4qu9FknfTi4HTYs_RHzkj5JdVyUgwjt3nXysABTtUSMk2jIOcODJknbZ_u4zBW8hsQhVnxcmaBB8VnT6nkehRPRpy8myS1O=w495-h660-no", "https://lh3.googleusercontent.com/i9y3pIEY5qGV2czDGsLskvFJYPzgm0RR1XeSOZN6taznB8o3odOIIv1qb0ZL1lyyzWShJLhShV7iQjRA5mqEJLKRowkkxaZ9y1LHCRwYpPRqIAtYsZJSXFPPfiW1lGyn6xiNohAvk8AVZpO5vcjUSN1GzpPyIqukrGP3laSA0FL5WgZN58s1ek0KRPSHYuqlfVp1y8yIyx7xeFoqTsSybJ4pBmw4d4Nf-TZO31Vz6J0XyrJRa2CvWSRLqDXn4fLS0i3B_rdQIRfNeq48AfuZYfS9TzCX6UkBthoeBCB2i8ZK5r_kdcW_4Yl8JtNmv556C0gwOtbXLOa7HAFLNQ86LeKW40NwT-be4MH4Ki3_z8c1nub5UMukxRN3v9tvKx2azFLDkhZ7Zc80ZD2a7qu9BbeorvkeTPJVnzd1PJhyAuxER6yW_-5VJngG-6xEF7Q_vungIsxzyeAvQAs0SEnQdTLL7V5M6y05vtwr3MEW4KkIrMRXHl0gSUWlnMtgzovjTNlURCARy6waZhs3HaWX81p4DG-e-WfawRZvBQhp_LFT=w495-h660-no", "https://lh3.googleusercontent.com/qkE7YA0UHJ15kG5cy9T0BYsx5LPVa7KszPjy-6y-BIGnferfv_WaCGkO82PdAcvlC2JiTZy8E6T4bn1vHmFt6iMBNZN0NjmFRiCiQGJEih5anZcoMb9cAWwZYjret65xKMV4LUKOu0QmFS-ytvRVcnPzlLpDxnAiCdVk0dknlE36mFrDSEMQh7_HB-touGxAEzB_UPwG6aceCzXer48uFqo4frs_735p7rHF9V5cFTRZg8a62QdE2wJ8yLqaQELMoaVdeWntOws5fTPKm-Yzh2SfzK7LB3HCc_fjRP_J9hrUU64aG67kz2IWVlyPPoxKxXcqu3f2saAJIKJdJpmqZfwWoQROVRZuZQbrUKjTz156uAgHIZBBXMM4KzhUEEIYIQqXDzhtuLde9iEw8HbMIgSa9fI5PVEcb5gMI78OSvsLPzy6Neh0cjo6o33uv8EBWYme5LleikEshMk_8erJnbYWuS17ZmCflw9eP8xK_hV1-Uvx120EChfhRJRlgKHAcaRlw9Xz0jZ6ri8BoOq7luuBSSAL53GyW2Wbil13KWgT=w495-h660-no" };
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				int num = 0;
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (num > 7)
					{
						num = 0;
					}
					dataTable.Rows[i]["Url"] = list[num];
				}
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable LoadUDFDataSAP(string itemCode)
	{
		try
		{
			List<string> uDFFieldSAP = GetUnitOfWork().Commons.getUDFFieldSAP("OITM");
			string text = "";
			foreach (string item in uDFFieldSAP)
			{
				text = text + "\"" + item + "\",";
			}
			text = text.Remove(text.Length - 1);
			string query = LoadUDFDataSAPQuery(text);
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadUDFDataSAPQuery(string udfString)
	{
		return "select " + udfString + " from OITM where \"ItemCode\"=@itemCode;";
	}

	public DataTable GetItemDetails(string itemCode, string userCode, int BPLId)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_ItemMaster_GetItemDetails";
			result = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@ItemCode", itemCode), new SqlParameter("@UserCode", userCode), new SqlParameter("@BPLId", BPLId));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetRecord(string itmCode, string typeRecord, string userCode, int BPLId, bool isAssetMaster = false)
	{
		try
		{
			string query = "APZ_ItemMaster_GetRecord";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itmCode ?? ""));
			list.Add(new SqlParameter("@TypeRecord", typeRecord));
			list.Add(new SqlParameter("@UserCode", userCode));
			list.Add(new SqlParameter("@BPLId", BPLId));
			list.Add(new SqlParameter("@IsAsset", isAssetMaster ? "Y" : "N"));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecord  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetOCYC()
	{
		try
		{
			string oCYCQuery = GetOCYCQuery();
			DataTable dataTable = ExecuteDataTable(oCYCQuery, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetOCYC  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetOCYCQuery()
	{
		return "select isnull(Code,0)as Code, isnull(Name,'') as Name  from OCYC where type='M' order by Code ";
	}

	public DataTable GetAnItemByItemCode(string itemCode, string username)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_ItemMaster_GetAnItemByItemCode";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@UserName", username));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetAnItemByItemCode  FAILED ", ex.Message);
		}
		return result;
	}

	public HttpResult Navigate(int type, string currentCode, string user, int branch)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_itemmaster_navigate", CommandType.StoredProcedure, new SqlParameter("@type", type), new SqlParameter("@current", currentCode), new SqlParameter("@user", user), new SqlParameter("@branch", branch));
			return dataTable.IsNotNull() ? new HttpResult(Enums.MessageCode.Success, "", Function.ToString(dataTable.Rows[0][0])) : new HttpResult(Enums.MessageCode.Error, Resources.Msg_UnknownError);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public DataSet GetByCode(string itemCode, string user, int branch)
	{
		try
		{
			DataSet dataSet = ExecuteData("apz_itemmaster_getbycode", CommandType.StoredProcedure, new SqlParameter("@item", itemCode), new SqlParameter("@user", user), new SqlParameter("@branch", branch));
			return dataSet.IsNotNull() ? dataSet : new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable LoadUomGroup()
	{
		try
		{
			string query = LoadUomGroupQuery();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadUomGroupQuery()
	{
		return "SELECT \"UgpEntry\" AS \"Entry\", \"UgpCode\" AS \"Code\", \"UgpName\" AS \"Name\" FROM OUGP;";
	}

	public DataTable LoadPriceList()
	{
		try
		{
			string query = LoadPriceListQuery();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadPriceListQuery()
	{
		return "SELECT ListNum,ListName FROM opln\r\n                                    UNION ALL\r\n                                    SELECT -1,N'Last Purchase Price'\r\n                                    UNION ALL\r\n                                    SELECT -2,N'Last Evaluated Price';";
	}

	public DataTable LoadManufactures()
	{
		try
		{
			string query = LoadManufacturesQuery();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadManufacturesQuery()
	{
		return "SELECT \"FirmCode\",\"FirmName\" FROM OMRC;";
	}

	public DataTable LoadShippingType()
	{
		try
		{
			string query = LoadShippingTypeQuery();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadShippingTypeQuery()
	{
		return "SELECT \"TrnspCode\" AS \"Code\", \"TrnspName\" AS \"Name\" FROM OSHP;";
	}

	public DataTable LoadCustomGroup()
	{
		try
		{
			string query = LoadCustomGroupQuery();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadCustomGroupQuery()
	{
		return "SELECT \"CstGrpCode\" \"Code\",\"CstGrpName\" \"Name\" FROM OARG;";
	}

	public DataTable LoadVatGroup(string category)
	{
		try
		{
			string query = LoadVatGroupQuery();
			SqlParameter sqlParameter = new SqlParameter("@Category", category);
			return ExecuteDataTable(query, CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadVatGroupQuery()
	{
		return "SELECT \"Code\", \"Name\", \"Rate\" FROM OVTG WHERE \"Category\" = @Category;";
	}

	public DataTable LoadProperties()
	{
		try
		{
			string query = LoadPropertiesQuery();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string LoadPropertiesQuery()
	{
		return "SELECT \"ItmsTypCod\" AS \"Code\", \"ItmsGrpNam\" AS \"Name\", 'N' AS \"Checked\" FROM OITG ORDER BY \"ItmsTypCod\" ASC;";
	}

	public HttpResult GetItemsPerUnit(string itmCode, string type)
	{
		try
		{
			string query = "";
			if (!(type == "I"))
			{
				if (type == "O")
				{
					query = GetItemsPerUnitQuery2();
				}
			}
			else
			{
				query = GetItemsPerUnitQuery1();
			}
			SqlParameter sqlParameter = new SqlParameter("@ItemCode", itmCode);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable.IsNotNull())
			{
				double num = Function.ParseDouble(dataTable.Rows[0][0]);
				return new HttpResult(Enums.MessageCode.Success, "", num);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Success, "", 0);
	}

	protected virtual string GetItemsPerUnitQuery1()
	{
		return "SELECT NumInBuy FROM OITM WHERE ITEMCODE=@ItemCode";
	}

	protected virtual string GetItemsPerUnitQuery2()
	{
		return "SELECT NumInSale FROM OITM WHERE ITEMCODE=@ItemCode";
	}

	public DataTable LoadSeries(string user)
	{
		try
		{
			string query = LoadSeriesQuery();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string LoadSeriesQuery()
	{
		return "SELECT \"Series\" AS \"Code\", \"SeriesName\" AS \"Name\" FROM NNM1 WHERE \"ObjectCode\" = '4' AND \"Locked\" = 'N';";
	}

	public DataTable GetItemStruct()
	{
		try
		{
			DataTable dataStructure = GetUnitOfWork().Commons.GetDataStructure("OITM");
			DataTable dataTable = new DataTable();
			if (dataStructure.IsNotNull())
			{
				for (int i = 0; i < dataStructure.Rows.Count; i++)
				{
					string text = Function.ToString(dataStructure.Rows[i]["DATA_TYPE"]).ToLower();
					string columnName = Function.ToString(dataStructure.Rows[i]["COLUMN_NAME"]);
					DataColumn dataColumn = new DataColumn();
					dataColumn.ColumnName = columnName;
					Type typeFromHandle;
					switch (text)
					{
					case "int":
					case "bigint":
					case "smallint":
						typeFromHandle = typeof(int);
						dataColumn.DefaultValue = 0;
						break;
					case "decimal":
						typeFromHandle = typeof(decimal);
						dataColumn.DefaultValue = 0;
						break;
					case "datetime":
					case "date":
						typeFromHandle = typeof(DateTime);
						dataColumn.DefaultValue = new DateTime(1900, 1, 1);
						break;
					case "numeric":
					case "float":
						typeFromHandle = typeof(double);
						dataColumn.DefaultValue = 0;
						break;
					case "bit":
						typeFromHandle = typeof(bool);
						dataColumn.DefaultValue = false;
						break;
					default:
						typeFromHandle = typeof(string);
						dataColumn.DefaultValue = "";
						break;
					}
					dataColumn.DataType = typeFromHandle;
					dataTable.Columns.Add(dataColumn);
				}
			}
			if (dataTable.Columns.Count > 0)
			{
				dataTable.Rows.Add(dataTable.NewRow());
				return dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	public DataTable Find(DataTable source, string user)
	{
		try
		{
			List<string> list = new List<string>();
			List<SqlParameter> list2 = new List<SqlParameter>();
			if (source.IsNotNull())
			{
				DataRow dataRow = source.Rows[0];
				for (int i = 0; i < source.Columns.Count; i++)
				{
					object obj = dataRow[source.Columns[i].ColumnName];
					string columnName = source.Columns[i].ColumnName;
					if (obj == null || string.IsNullOrEmpty(Function.ToString(obj)))
					{
						continue;
					}
					Type dataType = source.Columns[i].DataType;
					if (dataType == typeof(DateTime))
					{
						if (Function.ParseDateTime(obj).HasValue && !((DateTime)obj).Equals(new DateTime(1900, 1, 1)))
						{
							list.Add(string.Format("\"{0}\" = @{0}", columnName));
							list2.Add(new SqlParameter($"@{columnName}", obj));
						}
					}
					else if (dataType == typeof(bool))
					{
						if (Function.ParseBool(obj))
						{
							list.Add(string.Format("\"{0}\" = @{0}", columnName));
							list2.Add(new SqlParameter($"@{columnName}", obj));
						}
					}
					else if (dataType == typeof(decimal) || dataType == typeof(double) || dataType == typeof(float))
					{
						if (Math.Abs(Function.ParseDouble(obj)) > 0.0)
						{
							list.Add(string.Format("\"{0}\" = @{0}", columnName));
							list2.Add(new SqlParameter($"@{columnName}", obj));
						}
					}
					else if (dataType == typeof(short) || dataType == typeof(int) || dataType == typeof(long) || dataType == typeof(long) || dataType == typeof(short))
					{
						if (Math.Abs(Function.ParseInt(obj)) > 0)
						{
							list.Add(string.Format("\"{0}\" = @{0}", columnName));
							list2.Add(new SqlParameter($"@{columnName}", obj));
						}
					}
					else
					{
						list.Add(string.Format(FindQuery(), columnName));
						list2.Add(new SqlParameter($"@{columnName}", obj));
					}
				}
				string text = "SELECT * into #tbl FROM  OITM ";
				if (list.Count > 0)
				{
					text = text + " WHERE " + string.Join(" AND ", list.ToArray()) + "; ";
				}
				text += "SELECT * FROM #tbl; drop table #tbl;";
				return ExecuteDataTable(text, CommandType.Text, list2.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string FindQuery()
	{
		return "\"{0}\" LIKE N'%'+@{0}+'%' ";
	}

	public DataTable ReportItemSummary(DateTime fDate, DateTime tDate, string item, string whs, string display, string num)
	{
		return ExecuteDataTable("VAS_T17_ITEM_SUMMARY", CommandType.StoredProcedure, new SqlParameter("@fdate", (fDate < new DateTime(1900, 1, 1)) ? DateTime.Today : fDate), new SqlParameter("@tdate", (tDate < new DateTime(1900, 1, 1)) ? DateTime.Today : tDate), new SqlParameter("@itms", Function.ToString(item)), new SqlParameter("@whs", Function.ToString(whs)), new SqlParameter("@filter", Function.ToString(display)));
	}

	public DataTable GetADoc(string itmCode)
	{
		try
		{
			string query = "APZ_ItemMaster_GetADoc";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itmCode));
			return ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetADoc  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetItemProperties(string itemCode)
	{
		try
		{
			string query = GetItemPropertiesQuery1();
			if (itemCode == "-1")
			{
				query = GetItemPropertiesQuery2();
			}
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itemCode));
			DataSet dataSet = ExecuteData(query, CommandType.Text, list.ToArray());
			if (dataSet != null && dataSet.Tables.Count == 2)
			{
				return dataSet;
			}
			return new DataSet();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetADoc  FAILED ", ex.Message);
			return new DataSet();
		}
	}

	protected virtual string GetItemPropertiesQuery1()
	{
		return "Select QryGroup1,QryGroup2,QryGroup3,QryGroup4,QryGroup5,QryGroup6,QryGroup7,\r\n                                QryGroup8,QryGroup9,QryGroup10,QryGroup11,QryGroup12,QryGroup13,QryGroup14,\r\n                                QryGroup15,QryGroup16,QryGroup17,QryGroup18,QryGroup19,QryGroup20,QryGroup21,\r\n                                QryGroup22,QryGroup23,QryGroup24,QryGroup25,QryGroup26,QryGroup27,QryGroup28,\r\n                                QryGroup29,QryGroup30,QryGroup31,QryGroup32,QryGroup33,QryGroup34,QryGroup35,\r\n                                QryGroup36,QryGroup37,QryGroup38,QryGroup39,QryGroup40,QryGroup41,QryGroup42,\r\n                                QryGroup43,QryGroup44,QryGroup45,QryGroup46,QryGroup47,QryGroup48,QryGroup49,\r\n                                QryGroup50,QryGroup51,QryGroup52,QryGroup53,QryGroup54,QryGroup55,QryGroup56,\r\n                                QryGroup57,QryGroup58,QryGroup59,QryGroup60,QryGroup61,QryGroup62,QryGroup63,QryGroup64\r\n                                From OITM Where ItemCode=@ItemCode;\r\n                                Select ItmsGrpNam From OITG Order By ItmsTypCod ASC";
	}

	protected virtual string GetItemPropertiesQuery2()
	{
		return "Select 'N' QryGroup1,'N' QryGroup2,'N' QryGroup3,'N' QryGroup4,'N' QryGroup5,'N' QryGroup6,'N' QryGroup7,\r\n                                'N' QryGroup8,'N' QryGroup9,'N' QryGroup10,'N' QryGroup11,'N' QryGroup12,'N' QryGroup13,'N' QryGroup14,\r\n                                'N' QryGroup15,'N' QryGroup16,'N' QryGroup17,'N' QryGroup18,'N' QryGroup19,'N' QryGroup20,'N' QryGroup21,\r\n                                'N' QryGroup22,'N' QryGroup23,'N' QryGroup24,'N' QryGroup25,'N' QryGroup26,'N' QryGroup27,'N' QryGroup28,\r\n                                'N' QryGroup29,'N' QryGroup30,'N' QryGroup31,'N' QryGroup32,'N' QryGroup33,'N' QryGroup34,'N' QryGroup35,\r\n                                'N' QryGroup36,'N' QryGroup37,'N' QryGroup38,'N' QryGroup39,'N' QryGroup40,'N' QryGroup41,'N' QryGroup42,\r\n                                'N' QryGroup43,'N' QryGroup44,'N' QryGroup45,'N' QryGroup46,'N' QryGroup47,'N' QryGroup48,'N' QryGroup49,\r\n                                'N' QryGroup50,'N' QryGroup51,'N' QryGroup52,'N' QryGroup53,'N' QryGroup54,'N' QryGroup55,'N' QryGroup56,\r\n                                'N' QryGroup57,'N' QryGroup58,'N' QryGroup59,'N' QryGroup60,'N' QryGroup61,'N' QryGroup62,'N' QryGroup63,'N' QryGroup64;\r\n                                Select ItmsGrpNam From OITG Order By ItmsTypCod ASC";
	}

	public DataTable ExistFileInDoc(string itemCode, string fileName, string fileExt)
	{
		DataTable result = new DataTable();
		try
		{
			string query = ExistFileInDocQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@itemCode", itemCode));
			list.Add(new SqlParameter("@fileExt", fileExt));
			list.Add(new SqlParameter("@fileName", fileName));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string ExistFileInDocQuery()
	{
		return "select FileName from atc1 where AbsEntry=(select AtcEntry from OITM where itemCode=@itemCode) \r\n\t                and FileExt=@fileExt and FileName=@fileName and AbsEntry is not null";
	}

	public DataTable GetItemLineTableStructure()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("select Top 0 * from ITM1;", CommandType.Text);
			if (dataTable != null && dataTable.Columns.Count > 0)
			{
				string itemLineTableStructureQuery = GetItemLineTableStructureQuery();
				DataTable dataTable2 = ExecuteDataTable(itemLineTableStructureQuery, CommandType.Text);
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

	protected virtual string GetItemLineTableStructureQuery()
	{
		return "select * from OPLN Where ValidFor='Y';";
	}

	public DataTable GetUoMGroup()
	{
		try
		{
			string uoMGroupQuery = GetUoMGroupQuery();
			DataTable dataTable = ExecuteDataTable(uoMGroupQuery, CommandType.Text);
			if (dataTable != null && dataTable.Columns.Count > 0)
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

	protected virtual string GetUoMGroupQuery()
	{
		return "select UgpEntry Code,UgpCode Name from OUGP";
	}

	public DataTable GetVolumeUnit()
	{
		try
		{
			string volumeUnitQuery = GetVolumeUnitQuery();
			DataTable dataTable = ExecuteDataTable(volumeUnitQuery, CommandType.Text);
			if (dataTable != null && dataTable.Columns.Count > 0)
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

	protected virtual string GetVolumeUnitQuery()
	{
		return "select \"UnitCode\" \"Code\", \"VolDisply\" \"Name\" from OLGT;";
	}

	public DataTable GetLghtAndWghtDisplay()
	{
		try
		{
			string lghtAndWghtDisplayQuery = GetLghtAndWghtDisplayQuery();
			DataTable dataTable = ExecuteDataTable(lghtAndWghtDisplayQuery, CommandType.Text);
			if (dataTable != null && dataTable.Columns.Count > 0)
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

	protected virtual string GetLghtAndWghtDisplayQuery()
	{
		return "SELECT \r\n                                        b.UnitDisply UnitDisplyLength\r\n\t                                    ,b.UnitCode UnitCodeLength\r\n                                        ,c.UnitDisply UnitDisplyWeight\r\n\t                                    ,c.UnitCode UnitCodeWeight\r\n                                    from OADM a INNER JOIN OLGT b ON a.DefLengthU = b.UnitCode\r\n                                    INNER JOIN OWGT c ON a.DefWeightU = c.UnitCode";
	}

	public DataTable GetUoMCombo(int ugpEntry, string itemCode, string type)
	{
		try
		{
			string query = "APZ_ItemMaster_GetUoMCombo";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UgpEntry", ugpEntry));
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@Type", type));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Columns.Count > 0)
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

	public DataTable GetUomInItemByUomGroup(int ugpEntry, string itemCode)
	{
		try
		{
			string query = "APZ_ItemMaster_GetUomInItemByUomGroup";
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@UgpEntry", ugpEntry), new SqlParameter("@itemCode", itemCode));
			if (dataTable != null && dataTable.Columns.Count > 0)
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

	public DataTable GetUoMByUomGroup(int ugpEntry)
	{
		try
		{
			string query = "APZ_ItemMaster_GetUoMByUomGroup";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UgpEntry", ugpEntry));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Columns.Count > 0)
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

	public DataTable LoadBaseUoM(int ugpEntry)
	{
		try
		{
			string query = LoadBaseUoMQuery();
			SqlParameter sqlParameter = new SqlParameter("@UgpEntry", ugpEntry);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Columns.Count > 0)
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

	protected virtual string LoadBaseUoMQuery()
	{
		return "SELECT b.\"UomEntry\", b.\"UomName\" FROM OUGP a INNER JOIN OUOM b ON a.\"BaseUom\" = b.\"UomEntry\" Where \"UgpEntry\"=@UgpEntry;";
	}

	public DataTable SearchITM12(string itemCode)
	{
		try
		{
			string query = SearchITM12Query();
			SqlParameter sqlParameter = new SqlParameter("@ItemCode", itemCode);
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Columns.Count > 0)
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

	protected virtual string SearchITM12Query()
	{
		return "SELECT a.*, b.\"UomCode\", b.\"UomName\" FROM ITM12 a INNER JOIN OUOM b ON a.\"UomEntry\" = b.\"UomEntry\" WHERE \"ItemCode\" = @ItemCode;";
	}

	public DataTable UoMInItemGetRowsDetails(int uomEntry, int ugpEntry)
	{
		try
		{
			string query = UoMInItemGetRowsDetailsQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UgpEntry", ugpEntry));
			list.Add(new SqlParameter("@UomEntry", uomEntry));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Columns.Count > 0)
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

	protected virtual string UoMInItemGetRowsDetailsQuery()
	{
		return "SELECT a.\"AltQty\", a.\"BaseQty\", a.\"UomEntry\", b.\"UomName\" FROM UGP1 a INNER JOIN OUOM b ON a.\"UomEntry\" = b.\"UomEntry\" WHERE a.\"UgpEntry\" = @UgpEntry AND a.\"UomEntry\" = @UomEntry;";
	}

	public DataTable GetUoMPricesByItmCodePrList(int? priceList, string itemCode, int ugpEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@PriceList", priceList.GetValueOrDefault()));
			list.Add(new SqlParameter("@ugpEntry", ugpEntry));
			return ExecuteDataTable("apz_item_GetUoMPricesByItmCodePrList", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable UoMPricesLoadPriceList(int listNum)
	{
		try
		{
			string query = UoMPricesLoadPriceListQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ListNum", listNum));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Columns.Count > 0)
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

	protected virtual string UoMPricesLoadPriceListQuery()
	{
		return "SELECT * From OPLN Where \"ListNum\"=@ListNum;";
	}

	public DataTable UoMPricesLoadComBo(string itemCode)
	{
		try
		{
			string query = UoMPricesLoadComBoQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itemCode));
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, list.ToArray());
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

	protected virtual string UoMPricesLoadComBoQuery()
	{
		return "SELECT \"UomEntry\", \"UomCode\", \"UomName\" FROM OUOM WHERE \"UomEntry\" IN (SELECT DISTINCT \"UomEntry\" FROM ITM12 a WHERE \"ItemCode\" = @ItemCode);";
	}

	public DataTable ItemsMasterGetListCustomsGroup()
	{
		try
		{
			string query = ItemsMasterGetListCustomsGroupQuery();
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Columns.Count > 0)
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

	protected virtual string ItemsMasterGetListCustomsGroupQuery()
	{
		return "SELECT \"CstGrpCode\" AS \"Code\", \"CstGrpName\" AS \"Name\", \"TotalTax\" AS \"Rate\" FROM OARG;";
	}

	public DataTable GetItemType(string itemCode)
	{
		try
		{
			string itemTypeQuery = GetItemTypeQuery();
			SqlParameter sqlParameter = new SqlParameter("@ItemCode", itemCode);
			DataTable dataTable = ExecuteDataTable(itemTypeQuery, CommandType.Text, sqlParameter);
			if (dataTable != null && dataTable.Columns.Count > 0)
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

	protected virtual string GetItemTypeQuery()
	{
		return "SELECT ItemType From OITM Where ItemCode=@ItemCode";
	}

	public DataTable GetTop1(string itemCode)
	{
		try
		{
			string top1Query = GetTop1Query();
			return ExecuteDataTable(top1Query, CommandType.Text, new SqlParameter("@ItemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetTop1Query()
	{
		return "SELECT * FROM oitm  WHERE ItemCode=@ItemCode;";
	}

	public string IsExistsTransItemWarehouse(string itemCode, string whsCode)
	{
		string result = "";
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_itemmaster_exists_trans_itemwhs", CommandType.StoredProcedure, new SqlParameter("@ItemCode", itemCode), new SqlParameter("@WhsCode", whsCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public string IsStockInItemWarehouse(string itemCode, string whsCode)
	{
		string result = "";
		try
		{
			string query = IsStockInItemWarehouseQuery();
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@ItemCode", itemCode), new SqlParameter("@WhsCode", whsCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ToString(dataTable.Rows[0][0]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	protected virtual string IsStockInItemWarehouseQuery()
	{
		return "Select 1 From OITW Where \"ItemCode\"=@ItemCode AND \"WhsCode\"=@WhsCode AND \r\n                (\"OnHand\" > 0 OR \"IsCommited\" > 0 OR \"OnOrder\" > 0);";
	}

	public string IsLinkedTrans(string itemCode)
	{
		string result = "";
		try
		{
			DataTable dataTable = ExecuteDataTable("apz_itemmaster_linked_trans", CommandType.StoredProcedure, new SqlParameter("@ItemCode", itemCode ?? ""));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = "Y";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public string RemoveItem(string itemCode)
	{
		try
		{
			return Process(ApzObjectTypes.oItems, -1, Enums.TransactionType.D, new DataSet(), itemCode).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetPriceList()
	{
		DataTable result = new DataTable();
		try
		{
			result = ExecuteDataTable("Select * from OPLN;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	public DataTable GetItemGroupData(int itmsGrpCod)
	{
		try
		{
			return ExecuteDataTable(GetItemGroupDataQuery(), CommandType.Text, new SqlParameter("@itmsGrpCod", itmsGrpCod));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetItemGroupDataQuery()
	{
		return "select \tPlaningSys\r\n\t,PrcrmntMtd\r\n\t,CompoWH\r\n\t,OrdrIntrvl\r\n\t,OrdrMulti\r\n\t,MinOrdrQty\r\n\t,LeadTime\r\n\t,ToleranDay\r\n\t,UgpEntry\r\n\t,InvntSys from OITB where itmsGrpCod=@itmsGrpCod";
	}

	public DataTable GetNext(ItemsImpl doc)
	{
		throw new NotImplementedException();
	}

	public DataTable GetPrevious(ItemsImpl doc)
	{
		throw new NotImplementedException();
	}

	public DataTable GetFirst(ItemsImpl doc)
	{
		throw new NotImplementedException();
	}

	public DataTable GetLast(ItemsImpl doc)
	{
		throw new NotImplementedException();
	}

	public DataTable GetProductionData(string itemCode)
	{
		try
		{
			string productionDataQuery = GetProductionDataQuery();
			return ExecuteDataTable(productionDataQuery, CommandType.Text, new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetProductionDataQuery()
	{
		return "select \r\n    (select  case treetype when 'A' then 'Assembly' when 'S' then 'Sales' when 'P' then 'Production' when 'T' then 'Template' end from OITT where code = @itemCode) TreeType\r\n\t,(select count(1) from ITT1 where father = @itemCode and Type = 4) NoOfItem\r\n\t,(select count(1) from ITT1 where father = @itemCode and Type = 290) NoOfRes";
	}

	public DataTable GetPreferredVendor(string itemCode)
	{
		try
		{
			string preferredVendorQuery = GetPreferredVendorQuery();
			return ExecuteDataTable(preferredVendorQuery, CommandType.Text, new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetPreferredVendorQuery()
	{
		return "select \r\n\t                        a.*\r\n\t                        ,b.CardName\r\n\t                        ,b.ListNum\r\n\t                        ,c.Price\r\n\t\t\t\t\t\t\t,c.Currency PriceCurr\r\n\t                        ,d.Price LastPurPrc\r\n\t\t\t\t\t\t\t,d.Currency PurchaseCurr\r\n                        from ITM2 a left join OCRD b on a.VendorCode=b.cardcode\r\n\t                        left join ITM1 c on a.ItemCode=c.ItemCode and c.PriceList=b.ListNum\r\n\t                        left join ITM1 d on a.ItemCode=d.ItemCode and d.PriceList = b.ListNum\r\n                        where a.ItemCode = @itemCode";
	}

	public DataTable GetAPreferredVendorData(string itemCode, string vendorCode)
	{
		try
		{
			string aPreferredVendorDataQuery = GetAPreferredVendorDataQuery();
			return ExecuteDataTable(aPreferredVendorDataQuery, CommandType.Text, new SqlParameter("@itemCode", itemCode), new SqlParameter("@cardCode", vendorCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetAPreferredVendorDataQuery()
	{
		return "select\r\n                                    a.CardName\r\n\t                                ,a.ListNum\r\n\t                                ,b.Price\r\n\t\t\t\t\t\t\t\t\t,b.Currency PriceCurr\r\n\t                                ,c.Price LastPurPrc\r\n\t\t\t\t\t\t\t\t\t,c.Currency PurchaseCurr\r\n                                from OCRD a left join ITM1 b on b.PriceList = a.ListNum and b.ItemCode = @itemCode \r\n                                            left join ITM1 c on c.ItemCode = @itemCode and c.PriceList = a.ListNum\r\n                                where a.cardCode = @cardCode";
	}

	public bool ItemHaveQuantity(string itemCode)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(ItemHaveQuantityQuery(), CommandType.Text, new SqlParameter("@itemCode", itemCode));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseDouble(dataTable.Rows[0][0]) > 0.0;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	protected virtual string ItemHaveQuantityQuery()
	{
		return "select sum(\"OnHand\") from OITW where \"ItemCode\"=@itemCode;";
	}

	public int GetDefaultLengthCode()
	{
		try
		{
			string defaultLengthCodeQuery = GetDefaultLengthCodeQuery();
			DataTable dataTable = ExecuteDataTable(defaultLengthCodeQuery, CommandType.Text);
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

	protected virtual string GetDefaultLengthCodeQuery()
	{
		return "select top 1 UnitCode from OLGT order by UnitName;";
	}

	public DataTable GetPrice(string itemCode)
	{
		try
		{
			return ExecuteDataTable(GetPriceQuery(), CommandType.Text, new SqlParameter("@ItemCode", itemCode ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetPriceQuery()
	{
		return "SELECT * FROM ITM1 WHERE ItemCode=@ItemCode;";
	}

	public DataTable GetItemWarehouse(string itemCode)
	{
		try
		{
			return ExecuteDataTable(GetItemWarehouseQuery(), CommandType.Text, new SqlParameter("@ItemCode", itemCode ?? ""));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	protected virtual string GetItemWarehouseQuery()
	{
		return "SELECT * FROM OITW WHERE ItemCode=@ItemCode;";
	}

	public DataTable GetAllDataItem()
	{
		try
		{
			string query = "select ItemCode,ItemName,NumInBuy from OITM ";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataSet GetSaleItems(string userName, string cardCode)
	{
		try
		{
			string query = "APZ_D_GetSaleItems";
			new DataSet();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@UserName", Function.ToString(userName)));
			list.Add(new SqlParameter("@CardCode", Function.ToString(cardCode)));
			return ExecuteData(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	public DataTable GetDataItemByItemCode(string itemCode)
	{
		try
		{
			string query = "select * from OITM where ItemCode=@itemCode";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@itemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable GetDataItemGroup()
	{
		try
		{
			string query = "select * from OITB;";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable SearchWishItemMaster(string userCode)
	{
		try
		{
			string query = "APZ_ItemMasterData_SearchWishMobile";
			new DataTable();
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@UserCode", userCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	public DataTable SearchItemMaster(string itemCode, int ItmsGrpCod)
	{
		try
		{
			string query = "APZ_ItemMaster_SearchMobile";
			SqlParameter[] parameters = new SqlParameter[2]
			{
				new SqlParameter("@ItemCode", itemCode),
				new SqlParameter("@ItmsGrpCod", ItmsGrpCod)
			};
			new DataTable();
			return ExecuteDataTable(query, CommandType.StoredProcedure, parameters);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}
}
