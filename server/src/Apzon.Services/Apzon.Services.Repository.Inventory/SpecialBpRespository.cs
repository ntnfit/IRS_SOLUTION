using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Items;
using Apzon.Services.Interfaces.Inventory;

namespace Apzon.Services.Repository.Inventory;

public class SpecialBpRespository : BaseService<SpecialBP_Impl>, ISpecialBP
{
	public SpecialBpRespository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public bool CheckDoc(string cardCode, string itemCode)
	{
		try
		{
			string text = "SELECT *\r\n                            FROM OSPP \r\n                            WHERE \"CardCode\" = '" + cardCode + "'";
			text = text + " AND \"ItemCode\" = '" + itemCode + "';";
			DataTable dataTable = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
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

	public DataSet Search(string cardCode)
	{
		try
		{
			return ExecuteData("APZ_SpecialBp_LoadData", CommandType.StoredProcedure, new SqlParameter("@cardCode", cardCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Search - Doc ", ex.Message);
			return new DataSet();
		}
	}

	public string SaveDoc(List<SpecialBP_Impl> docs)
	{
		try
		{
			using (DatabaseService.OpenConnection())
			{
				using DbTransaction dbTransaction = DatabaseService.BeginTransaction();
				string text = "";
				for (int i = 0; i < docs.Count; i++)
				{
					string text2 = SaveADoc(docs[i]);
					if (!string.IsNullOrWhiteSpace(text2))
					{
						text = text2;
						break;
					}
				}
				if (string.IsNullOrWhiteSpace(text))
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

	private string SaveADoc(SpecialBP_Impl doc)
	{
		string text = "";
		try
		{
			_ = string.Empty;
			List<SqlParameter> list = new List<SqlParameter>();
			bool flag = CheckDoc(doc.CardCode, doc.ItemCode);
			string empty = string.Empty;
			if (!flag)
			{
				empty = "INSERT INTO OSPP  (ItemCode , CardCode ,Price, Currency, Discount,ListNum,AutoUpdt ,EXPAND, UserSign\r\n                                                          )\r\n                               VALUES \r\n                                        ( ";
				empty += " @itemCode ";
				list.Add(new SqlParameter("@itemCode", Function.ToString(doc.ItemCode)));
				empty += ", @cardCode ";
				list.Add(new SqlParameter("@cardCode", Function.ToString(doc.CardCode)));
				empty += ", @price ";
				list.Add(new SqlParameter("@price", Function.ParseDouble(doc.Price)));
				empty += ", @currency ";
				list.Add(new SqlParameter("@currency", Function.ToString(doc.Currency)));
				empty += ", @discount ";
				list.Add(new SqlParameter("@discount", Function.ParseDouble(doc.Discount)));
				empty += ", @listNum ";
				list.Add(new SqlParameter("@listNum", Function.ParseInt(doc.ListNum)));
				empty += ", @autoUpdt ";
				if (doc.AutoUpdt == "No")
				{
					doc.AutoUpdt = "N";
				}
				else if (doc.AutoUpdt == "Yes")
				{
					doc.AutoUpdt = "Y";
				}
				list.Add(new SqlParameter("@autoUpdt", doc.AutoUpdt));
				empty += ", @expand ";
				if (doc.Expand == "No")
				{
					doc.Expand = "N";
				}
				else if (doc.Expand == "Yes")
				{
					doc.Expand = "Y";
				}
				list.Add(new SqlParameter("@expand", Function.ToString(doc.Expand)));
				empty += ", @userSign ";
				list.Add(new SqlParameter("@userSign", Function.ParseInt(doc.UserSign)));
				empty += " )";
			}
			else
			{
				list = new List<SqlParameter>();
				empty = "UPDATE OSPP  \r\n                            SET\r\n                                Price= ";
				empty += " @price ";
				list.Add(new SqlParameter("@price", Function.ParseDouble(doc.Price)));
				empty += ",Currency= @currency ";
				list.Add(new SqlParameter("@currency", Function.ToString(doc.Currency)));
				empty += ",Discount= @discount ";
				list.Add(new SqlParameter("@discount", Function.ParseDouble(doc.Discount)));
				empty += ",ListNum= @listNum ";
				list.Add(new SqlParameter("@listNum", Function.ParseInt(doc.ListNum)));
				empty += ",AutoUpdt= @autoUpdt ";
				if (doc.AutoUpdt == "No")
				{
					doc.AutoUpdt = "N";
				}
				else if (doc.AutoUpdt == "Yes")
				{
					doc.AutoUpdt = "Y";
				}
				list.Add(new SqlParameter("@autoUpdt", doc.AutoUpdt));
				empty += ",Expand= @expand ";
				if (doc.Expand == "No")
				{
					doc.Expand = "N";
				}
				else if (doc.Expand == "Yes")
				{
					doc.Expand = "Y";
				}
				list.Add(new SqlParameter("@expand", Function.ToString(doc.Expand)));
				empty += ",UserSign= @userSign ";
				list.Add(new SqlParameter("@userSign", Function.ParseInt(doc.UserSign)));
				empty += " WHERE CardCode =@cardCode";
				list.Add(new SqlParameter("@cardCode", doc.CardCode));
				empty += " AND ItemCode = @itemCode";
				list.Add(new SqlParameter("@itemCode", doc.ItemCode));
			}
			if (ExecuteNonQuery(empty, CommandType.Text, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string Delete(List<Tuple<string, string>> obj)
	{
		try
		{
			string query = "delete from OSPP where \"CardCode\" = '" + obj.Find((Tuple<string, string> t) => t.Item1 == "cardCode").Item2 + "' and \"ItemCode\" = '" + obj.Find((Tuple<string, string> t) => t.Item1 == "itemCode").Item2 + "';";
			ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null);
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public DataTable ShowCardCode(string cardCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select a.\"CardCode\",b.\"CardName\",\r\n\t                             Case When b.\"CardType\" ='C' then 'Customer'\r\n\t\t                                                         When b.\"CardType\"= 'V' then \t' Vendor'\r\n\t\t                                                         When b.\"CardType\" = 'L' then 'Lead'\r\n                                                            END \"CardType\" \r\n\t                             from OSPP a\r\n\t                             inner join OCRD b\r\n\t                             on a.\"CardCode\" = b.\"CardCode\" \r\n                                   Where a.\"CardCode\"= '" + cardCode + "' ;";
			result = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Show CardCode - Doc ", ex.Message);
		}
		return result;
	}

	public DataTable GetItemByItemCode(string itemCode, int listNum)
	{
		DataTable result = new DataTable();
		try
		{
			string itemByItemCodeQuery = GetItemByItemCodeQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@ItemCode", itemCode));
			list.Add(new SqlParameter("@ListNum", listNum));
			result = ExecuteDataTable(itemByItemCodeQuery, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string GetItemByItemCodeQuery()
	{
		return "SELECT \r\n\t                    ItemCode\r\n\t                    ,ItemName\r\n\t                    ,(select price from itm1 where itemcode=a.itemcode and PriceList=@ListNum) Price\r\n\t                    ,0 Discount\r\n\t                    ,1 ListNum,\r\n\t                    'Y' AutoUpdt,\r\n\t                    (select isnull(PrimCurr,'') from opln where ListNum=@ListNum) as Currency,\r\n\t                    'Y' EXPAND,\r\n\t\t\t\t\t    (select price from itm1 where itemcode=a.itemcode and PriceList=@ListNum) PrimPrice,\r\n\t\t\t\t\t    (select AddPrice1 from itm1 where itemcode=a.itemcode and PriceList=@ListNum) AddPrice1,\r\n\t\t\t\t\t    (select AddPrice2 from itm1 where itemcode=a.itemcode and PriceList=@ListNum) AddPrice2,\r\n\t\t\t\t\t    (select ItmsGrpNam from OITB where a.ItmsGrpCod=OITB.ItmsGrpCod) ItmsGrpCod,\r\n\t\t\t\t\t    (select UomName from OUOM where a.IUoMEntry=OUOM.UomEntry) UoMName\r\n                    FROM OITM  a\r\n                    WHERE a.ItemCode=@ItemCode";
	}
}
