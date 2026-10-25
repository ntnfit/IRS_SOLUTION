using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Services.Interfaces.Production;
using SAPbobsCOM;

namespace Apzon.Services.Repository.Production;

public class BillOfMasterialsRepository : BaseService<DataSet>, IBillOfMasterials
{
	private readonly string _connection;

	public BillOfMasterialsRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable ShowOITT()
	{
		throw new NotImplementedException();
	}

	public DataTable GetItemName(string itemCode)
	{
		DataTable result = new DataTable();
		try
		{
			string itemNameQuery = GetItemNameQuery();
			result = ExecuteData(itemNameQuery, CommandType.Text, new SqlParameter("@itemCode", itemCode)).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetItemName ", ex.Message);
		}
		return result;
	}

	protected virtual string GetItemNameQuery()
	{
		return "select * from oitm Where  ItemCode = N'@itemCode'";
	}

	public DataTable ShowITT1(string BoMCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "apz_BOM_GetLine";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@father", BoMCode));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowITT1 ", ex.Message);
		}
		return result;
	}

	public DataTable GetItemCFL()
	{
		DataTable result = new DataTable();
		try
		{
			string itemCFLQuery = GetItemCFLQuery();
			result = ExecuteData(itemCFLQuery, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowITT1 ", ex.Message);
		}
		return result;
	}

	protected virtual string GetItemCFLQuery()
	{
		return "select ItemCode, ItemName, OnHand from OITM where VALIDFOR='Y' AND InvntItem='Y' and ItemCode not in(select Code from oitt)";
	}

	public HttpResult DeleteDoc(string code)
	{
		try
		{
			string query = DeleteDocQuery();
			return (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@code", code ?? "")) == 0) ? new HttpResult(Enums.MessageCode.Error, "Could not delete Bom") : new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteDoc FAILED ", ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual string DeleteDocQuery()
	{
		return "DELETE FROM ITT1 WHERE Father = @code\r\n                                        delete oitt where Code=@code";
	}

	public DataTable getBoMByItemCode(string code)
	{
		try
		{
			string boMByItemCode = getBoMByItemCode();
			SqlParameter sqlParameter = new SqlParameter("@Code", SqlDbType.NVarChar, 50);
			sqlParameter.Value = code ?? "";
			return ExecuteDataTable(boMByItemCode, CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "getBoMByItemCode() BoM ", ex.Message);
		}
		return new DataTable();
	}

	protected virtual string getBoMByItemCode()
	{
		return "select T0.*,T1.\"ItemName\" \"ItemName\",T1.\"ItemName\" \"Name\" from OItt T0 LEFT JOIN OITM T1 ON T0.\"Code\"=T1.\"ItemCode\" where T0.\"Code\"=@Code;";
	}

	public DataTable Search(DataTable source, DataTable dtOCondition)
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
							list.Add(string.Format("a.\"{0}\" = @{0}", columnName));
							list2.Add(new SqlParameter($"@{columnName}", obj));
						}
					}
					else if (dataType == typeof(bool))
					{
						if (Function.ParseBool(obj))
						{
							list.Add(string.Format("a.\"{0}\" = @{0}", columnName));
							list2.Add(new SqlParameter($"@{columnName}", obj));
						}
					}
					else if (dataType == typeof(decimal) || dataType == typeof(double) || dataType == typeof(float))
					{
						if (Math.Abs(Function.ParseDouble(obj)) > 0.0)
						{
							list.Add(string.Format("a.\"{0}\" = @{0}", columnName));
							list2.Add(new SqlParameter($"@{columnName}", obj));
						}
					}
					else if (dataType == typeof(short) || dataType == typeof(int) || dataType == typeof(long) || dataType == typeof(long) || dataType == typeof(short))
					{
						if (Math.Abs(Function.ParseInt(obj)) > 0)
						{
							list.Add(string.Format("a.\"{0}\" = @{0}", columnName));
							list2.Add(new SqlParameter($"@{columnName}", obj));
						}
					}
					else
					{
						list.Add(string.Format(SearchQuery(), columnName));
						list2.Add(new SqlParameter($"@{columnName}", obj));
					}
				}
				string text = "SELECT a.*\r\n\t                                ,b.\"ItemName\" as \"ItemName\"\r\n                                FROM  OITT a left join OITM b on a.\"Code\"=b.\"ItemCode\" ";
				if (list.Count > 0)
				{
					text = text + " WHERE " + string.Join(" AND ", list.ToArray());
				}
				text += ";";
				return ExecuteDataTable(text, CommandType.Text, list2.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	protected virtual string SearchQuery()
	{
		return "a.\"{0}\" LIKE N'%'+@{0}+'%' ";
	}

	public DataTable SearchLine(BillOfMasterials_LineImpl_ITT1 line)
	{
		DataTable result = new DataTable();
		try
		{
			string query = SearchLineQuery();
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@father", line.Father));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Search - Line WOR1 ", ex.Message);
		}
		return result;
	}

	protected virtual string SearchLineQuery()
	{
		return "SELECT isnull(Father,'') as Father,isnull(ChildNum,0) as ChildNum ,isnull(Code,'') as Code \r\n                                    ,isnull(Quantity,0) as Quantity ,isnull(Warehouse,'') as Warehouse ,isnull(Price,0) as Price \r\n                                    ,isnull(Currency,'') as Currency ,isnull(PriceList,0) as PriceList ,isnull(OrigPrice,0) as OrigPrice ,isnull(OrigCurr,'') as OrigCurr ,isnull(IssueMthd,'') as IssueMthd\r\n                                    ,isnull(Uom,'') as Uom ,isnull(Comment,'') as Comment ,isnull(LogInstanc,0) as LogInstanc ,isnull(Object,'') as Object \r\n                                    ,isnull(OcrCode,'') as OcrCode ,isnull(OcrCode2,'') as OcrCode2 ,isnull(OcrCode3,'') as OcrCode3 ,isnull(OcrCode4,'') as OcrCode4 \r\n                                    ,isnull(OcrCode5,'') as OcrCode5 ,isnull(PrncpInput,'') as PrncpInput ,isnull(Project,'') as Project ,isnull(Type,0) as Type \r\n                                    ,isnull(WipActCode,'') as WipActCode ,isnull(AddQuantit,0) as AddQuantit ,isnull(LineText,'') as LineText\r\n                                 FROM ITT1 a WHERE a.father = @father";
	}

	public DataTable SearchADoc(string code)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "apz_BOM_SearchADoc";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@Code", code));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SearchADoc  ", ex.Message);
		}
		return result;
	}

	public DataTable CheckValidItemType(string code)
	{
		DataTable result = new DataTable();
		try
		{
			string query = CheckValidItemTypeQuery();
			SqlParameter sqlParameter = new SqlParameter("@Code", code);
			result = ExecuteData(query, CommandType.Text, sqlParameter).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CheckValidItemType ", ex.Message);
		}
		return result;
	}

	protected virtual string CheckValidItemTypeQuery()
	{
		return "select \r\n                                    isnull(PrchseItem,'N') PrchseItem\r\n                                    ,isnull(SellItem,'N') SellItem\r\n                                    ,isnull(InvntItem,'N') InvntItem\r\n                                from OITM where ItemCode=@Code";
	}

	public DataTable GetItemPriceByPriceList(string itemCode, int priceList)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select \"Price\", \"Currency\" from itm1 where \"ItemCode\"=@itemCode and \"PriceList\"=@priceList;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@itemCode", itemCode));
			list.Add(new SqlParameter("@priceList", priceList));
			result = ExecuteDataTable(query, CommandType.Text, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetItemPriceByPriceList ", ex.Message);
		}
		return result;
	}

	public DataTable GetActiveDimensionCount()
	{
		DataTable result = new DataTable();
		try
		{
			string activeDimensionCountQuery = GetActiveDimensionCountQuery();
			result = ExecuteDataTable(activeDimensionCountQuery, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetActiveDimensionCount ", ex.Message);
		}
		return result;
	}

	protected virtual string GetActiveDimensionCountQuery()
	{
		return "SELECT Count(*) FROM ODIM WHERE  DimActive='Y'";
	}

	public DataTable GetSearchTableLine()
	{
		return GetUnitOfWork().Commons.GetSearchTable("ITT1");
	}

	public DataTable GetSearchTableDoc()
	{
		return GetUnitOfWork().Commons.GetSearchTable("OITT");
	}

	public DataTable GetItemDetails(string type, string code)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "";
			string text2 = type.ToUpper();
			if (!(text2 == "I"))
			{
				if (text2 == "R")
				{
					text = GetItemDetailsRQuery();
				}
			}
			else
			{
				text = GetItemDetailsIQuery();
			}
			if (text != "")
			{
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@Code", code));
				result = ExecuteDataTable(text, CommandType.Text, list.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string GetItemDetailsIQuery()
	{
		return "SELECT \r\n                                    isnull(ItemName,'') ItemName\r\n\t                                ,isnull(InvntryUom,'') InvntryUom\r\n\t                                ,isnull(OnHand,0) OnHand\r\n\t                                ,isnull(OnOrder,0) OnOrder\r\n\t                                ,isnull(DfltWH,'') DefaultWarehouse\r\n\t                                ,isnull(IssueMthd,'') IssueMthd\r\n\t                             FROM OITM where itemcode=@Code";
	}

	protected virtual string GetItemDetailsRQuery()
	{
		return "SELECT \r\n                                    isnull(ResName,'') ResName\r\n\t                                ,isnull(UnitOfMsr,'') InvntryUom\r\n\t                                ,0 OnHand\r\n\t                                ,0 OnOrder\r\n\t                                ,isnull(DfltWH,'') DefaultWarehouse\r\n\t                                ,isnull(IssueMthd,'') IssueMthd\r\n\t                             FROM ORSC where ResCode=@Code";
	}

	public DataTable IsValidCode(int type, string code)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "";
			switch (type)
			{
			case 4:
				text = IsValidCodeItemQuery();
				break;
			case 290:
				text = IsValidCodeResourceQuery();
				break;
			}
			if (text != "")
			{
				List<SqlParameter> list = new List<SqlParameter>();
				list.Add(new SqlParameter("@Code", code));
				result = ExecuteDataTable(text, CommandType.Text, list.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string IsValidCodeItemQuery()
	{
		return "SELECT 1 FROM OITM where ItemCode=@Code and\r\n(validFor='Y' and frozenFor='N' and getdate() between isnull(validFrom, getdate()) and isnull(validTo, getdate()) or\r\nvalidFor='N' and frozenFor='Y' and getdate() not between isnull(frozenFrom, getdate()) and isnull(frozenTo, getdate()) or\r\nvalidFor='Y' and frozenFor='Y' and getdate() between isnull(validFrom, getdate()) and isnull(validTo, getdate())\r\nand getdate() not between isnull(frozenFrom, getdate()) and isnull(frozenTo, getdate()))";
	}

	protected virtual string IsValidCodeResourceQuery()
	{
		return "SELECT 1 FROM ORSC where ResCode=@Code";
	}

	public bool IsABOM(string code)
	{
		try
		{
			string query = IsABOMQuery();
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Code", code));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	protected virtual string IsABOMQuery()
	{
		return "SELECT 1 FROM OITT where Code=@Code";
	}

	private bool CheckDoc(string code)
	{
		try
		{
			string query = CheckDocQuery();
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

	protected virtual string CheckDocQuery()
	{
		return "SELECT a.Code FROM OITT a WHERE a.Code = @code";
	}

	public string SaveDocNoSAP(DataSet ds, Enums.TransactionType type, string code)
	{
		return Process(ApzObjectTypes.oProductTrees, -1, type, ds, code).Message;
	}

	public string SaveDocNoSAPUpdate(DataSet ds)
	{
		return Process(ApzObjectTypes.oProductTrees, -1, Enums.TransactionType.U, ds).Message;
	}

	public override HttpResult ProcessData(ApzObjectTypes objtype, int bplId, ref Enums.TransactionType transtype, DataSet ds, ref string objId, ref string objNum)
	{
		return transtype switch
		{
			Enums.TransactionType.A => ProcessSave(ds, transtype, ref objId, ref objNum), 
			Enums.TransactionType.U => ProcessSave(ds, transtype, ref objId, ref objNum), 
			Enums.TransactionType.D => DeleteDoc(objId), 
			_ => new HttpResult(), 
		};
	}

	private HttpResult ProcessSave(DataSet ds, Enums.TransactionType transtype, ref string objId, ref string objNum)
	{
		HttpResult httpResult = new HttpResult();
		try
		{
			double price = 0.0;
			string itemCode = Function.ToString(ds.Tables[0].Rows[0]["Code"]);
			short value = Function.ParseShort(ds.Tables[0].Rows[0]["PriceList"]).Value;
			if (ds.Tables[0].Columns.Contains("ItemPrice111"))
			{
				price = Function.ParseDouble(ds.Tables[0].Rows[0]["ItemPrice111"]);
				ds.Tables[0].Columns.Remove("ItemPrice111");
				ds.Tables[0].AcceptChanges();
			}
			CreateTempTable(ds.Tables[0], "#APZ_DOC_BOM");
			BulkCopy(ds.Tables[0], "#APZ_DOC_BOM");
			List<DataRow> list = new List<DataRow>();
			for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
			{
				if (string.IsNullOrWhiteSpace(Function.ToString(ds.Tables[1].Rows[i]["Code"])))
				{
					list.Add(ds.Tables[1].Rows[i]);
				}
			}
			for (int j = 0; j < list.Count; j++)
			{
				ds.Tables[1].Rows.Remove(list[j]);
			}
			if (ds.Tables[1].Columns.Contains("ItemName"))
			{
				ds.Tables[1].Columns.Remove("ItemName");
			}
			if (ds.Tables[1].Columns.Contains("Total"))
			{
				ds.Tables[1].Columns.Remove("Total");
			}
			ds.Tables[1].AcceptChanges();
			CreateTempTable(ds.Tables[1], "#APZ_DOCLINE_BOM");
			BulkCopy(ds.Tables[1], "#APZ_DOCLINE_BOM");
			string query = "apz_process_BOM";
			string value2 = ((transtype == Enums.TransactionType.A) ? "A" : "U");
			List<SqlParameter> list2 = new List<SqlParameter>();
			list2.Add(new SqlParameter("@TransType", value2));
			list2.Add(new SqlParameter("@Code", Function.ToString(ds.Tables[0].Rows[0]["Code"])));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list2.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0][1])))
				{
					objId = Function.ToString(dataTable.Rows[0][0]);
					objNum = objId;
					return new HttpResult(Enums.MessageCode.Success, "", objId);
				}
				return new HttpResult(Enums.MessageCode.Error, $"{Function.ParseInt(dataTable.Rows[0][0])}-{Function.ToString(dataTable.Rows[0][1])}");
			}
			httpResult.Message = "";
			httpResult.Message = UpdateItemPrice(itemCode, value, price);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			httpResult.Message = ex.Message;
		}
		return httpResult;
	}

	private string UpdateItemPrice(string itemCode, short priceList, double price)
	{
		try
		{
			string query = UpdateItemPriceQuery();
			ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@price", price), new SqlParameter("@itemCode", itemCode), new SqlParameter("@priceList", priceList));
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "Update Item Price Error: " + ex.Message;
		}
	}

	protected virtual string UpdateItemPriceQuery()
	{
		return "Update ITM1 Set Price=@price Where ItemCode=@itemCode AND PriceList=@priceList";
	}

	public DataTable GetFirstRecord()
	{
		return GetUnitOfWork().Commons.GetFirstRecord("oitt", "Code");
	}

	public DataTable GetLastRecord()
	{
		return GetUnitOfWork().Commons.GetLastRecord("oitt", "Code");
	}

	public DataTable GetNextRecord(string value)
	{
		return GetUnitOfWork().Commons.GetNextRecord("oitt", "Code", value);
	}

	public DataTable GetPreviousRecord(string value)
	{
		return GetUnitOfWork().Commons.GetPreviousRecord("oitt", "Code", value);
	}

	public DataTable GetRecord(string p, string c)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "apz_BOM_GetRecord";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@position", p));
			list.Add(new SqlParameter("@Code", c));
			result = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecord  ", ex.Message);
		}
		return result;
	}

	public DataTable GetList_BillOfMster()
	{
		DataTable result = new DataTable();
		try
		{
			string list_BillOfMsterQuery = GetList_BillOfMsterQuery();
			result = ExecuteData(list_BillOfMsterQuery, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ShowOITT ", ex.Message);
		}
		return result;
	}

	protected virtual string GetList_BillOfMsterQuery()
	{
		return "select a.*, b.ItemName\r\n                                    from oitt a left join oitm b on a.Code = b.ItemCode ";
	}

	public DataTable GetRawMaterial(string itemCode)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "apz_bom_getRawMaterial";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@ItemCode", itemCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return result;
		}
	}

	public DataTable GetChildItemsInBom(string code)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "SP_APZ_BOM_GetChildItemsInBom";
			return ExecuteDataTable(query, CommandType.StoredProcedure, new SqlParameter("@Father", code));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return result;
		}
	}

	public DataTable GetLineByFather(string father, string username, int branch)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			if (!string.IsNullOrEmpty(father))
			{
				list.Add(new SqlParameter("@Father", father));
			}
			if (!string.IsNullOrEmpty(username))
			{
				list.Add(new SqlParameter("@Username", username));
			}
			if (branch != -1)
			{
				list.Add(new SqlParameter("@branch", branch));
			}
			return ExecuteDataTable("apz_productionforbatch_GetLineByFather", CommandType.StoredProcedure, list.ToArray());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new DataTable();
	}

	private string GetSelectColumnUDF(string table)
	{
		string result = "";
		try
		{
			string query = "SELECT  CASE DATA_TYPE \r\n\t                            WHEN 'bit' THEN 'isnull(A.'+COLUMN_NAME+',0) '+COLUMN_NAME+','\r\n\t                            WHEN 'money'  THEN 'isnull(A.'+COLUMN_NAME+',0) '+COLUMN_NAME+','\r\n\t                            WHEN 'int'  THEN 'isnull(A.'+COLUMN_NAME+',0) '+COLUMN_NAME+','\r\n\t                            WHEN 'decimal'  THEN 'isnull(A.'+COLUMN_NAME+',0) '+COLUMN_NAME+','\r\n\t                            WHEN 'smallint'  THEN 'isnull(A.'+COLUMN_NAME+',0) '+COLUMN_NAME+','\r\n\t                            WHEN 'numeric'  THEN 'isnull(A.'+COLUMN_NAME+',0) '+COLUMN_NAME+','\r\n\t                            WHEN 'tinyint'  THEN 'isnull(A.'+COLUMN_NAME+',0) '+COLUMN_NAME+','\r\n\t                            WHEN 'float'  THEN 'isnull(A.'+COLUMN_NAME+',0) '+COLUMN_NAME+','\r\n\t                            WHEN 'bigint'  THEN 'isnull(A.'+COLUMN_NAME+',0) '+COLUMN_NAME+','\r\n\t                            else\r\n\t                            'A.'+COLUMN_NAME+' '+COLUMN_NAME+',' end\r\n\t                            FROM INFORMATION_SCHEMA.COLUMNS\r\n\t                            WHERE TABLE_NAME = N'" + table + "' AND LEFT(COLUMN_NAME,2) ='U_'\r\n\t                            FOR XML PATH('')";
			ExecuteDataTable(query, CommandType.StoredProcedure, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecord  ", ex.Message);
		}
		return result;
	}

	private string GetInsertColumnUDF(List<UserFieldsImpl> lstUDf)
	{
		string text = "";
		try
		{
			for (int i = 0; i < lstUDf.Count; i++)
			{
				text = text + "," + lstUDf[i].Name;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecord  ", ex.Message);
		}
		return text;
	}

	private List<string> GetListColumnUDF(string table)
	{
		List<string> list = new List<string>();
		DataTable dataTable = new DataTable();
		try
		{
			string query = "SELECT  COLUMN_NAME+','\r\n\t                            FROM INFORMATION_SCHEMA.COLUMNS\r\n\t                            WHERE TABLE_NAME = N'" + table + "' AND LEFT(COLUMN_NAME,2) ='U_'\r\n\t                            FOR XML PATH('')";
			dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				string text = Function.ToString(dataTable.Rows[0][0]);
				if (text.Length > 0)
				{
					text = text.Remove(text.Length - 1);
				}
				string[] array = text.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					list.Add(array[i]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetRecord  ", ex.Message);
		}
		return list;
	}

	private SqlParameter GetSQLParam(UserFieldsImpl udfField)
	{
		SqlParameter sqlParameter = new SqlParameter();
		try
		{
			switch (udfField.Type)
			{
			case BoFieldTypes.db_Alpha:
				sqlParameter.DbType = DbType.String;
				sqlParameter.SqlValue = Function.ToString(udfField.Value);
				break;
			case BoFieldTypes.db_Date:
				sqlParameter.DbType = DbType.DateTime;
				sqlParameter.SqlValue = Function.ParseDateTime(udfField.Value).Value;
				break;
			case BoFieldTypes.db_Float:
				sqlParameter.DbType = DbType.Decimal;
				sqlParameter.SqlValue = Function.ParseDecimal(udfField.Value);
				break;
			case BoFieldTypes.db_Memo:
				sqlParameter.DbType = DbType.String;
				sqlParameter.SqlValue = Function.ToString(udfField.Value);
				break;
			case BoFieldTypes.db_Numeric:
				sqlParameter.DbType = DbType.Int32;
				sqlParameter.SqlValue = Function.ParseInt(udfField.Value);
				break;
			default:
				sqlParameter.DbType = DbType.String;
				sqlParameter.SqlValue = Function.ToString(udfField.Value);
				break;
			}
			sqlParameter.ParameterName = "@" + udfField.Name;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return sqlParameter;
	}

	public DataTable GetDataLineBomVersion(string fatherCode, string version)
	{
		try
		{
			return ExecuteDataTable("APZ_Bom_GetDataLineBomVersion", CommandType.StoredProcedure, new SqlParameter("@fatherCode", fatherCode), new SqlParameter("@version", version));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDataLineBomVersion ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable GetAllChilds()
	{
		try
		{
			return ExecuteDataTable("Select * from ITT1;", CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetDataLineBomVersion ", ex.Message);
		}
		return new DataTable();
	}

	public DataTable CopyChildItemsInBom(string itemCode, int baseType, int baseEntry, int baseRow)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "APZ_BOM_CopyChildItemsInBom";
			SqlParameter[] parameters = new SqlParameter[4]
			{
				new SqlParameter("@ItemCode", itemCode),
				new SqlParameter("@BaseType", baseType),
				new SqlParameter("@BaseEntry", baseEntry),
				new SqlParameter("@BaseRow", baseRow)
			};
			return ExecuteDataTable(query, CommandType.StoredProcedure, parameters);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return result;
		}
	}
}
