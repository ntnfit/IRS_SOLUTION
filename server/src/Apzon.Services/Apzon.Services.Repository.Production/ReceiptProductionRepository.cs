using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Production;
using Apzon.Services.Interfaces.Production;

namespace Apzon.Services.Repository.Production;

public class ReceiptProductionRepository : BaseService<ReceiptProduction_Impl_OIGN>, IReceiptProduction
{
	public ReceiptProductionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public int GetMaxDocEntry()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(DocEntry ),0) as DocEntry\r\n                            FROM OIGN ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["DocEntry"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadItemsByParentCode  FAILED ", ex.Message);
			return 0;
		}
	}

	public DataTable LoadItemsByProdOrderEntry(int docEntry)
	{
		try
		{
			string query = "select isnull(a.docentry,0) as BaseRef\r\n\t\t\t,isnull(a.series,0) as Series\r\n\t\t\t,0 as LineNum\r\n\t\t\t, isnull(a.ItemCode,'') as ItemCode\r\n\t\t\t,'Complete' as TranType\r\n\t\t\t,isnull(b.ItemName,'') as Dscription\r\n\t\t\t,0 as Quantity\r\n\t\t\t,isnull(a.Warehouse,'') as WhsCode \r\n\t\t\t,isnull(c.AvgPrice,0) as Price\r\n\t\t\t,isnull(a.PlannedQty,0) as PlannedQty\r\n\t\t\t,isnull(a.CmpltQty,0) as CmpltQty\r\n\t\t\t,isnull(b.InvntryUom,'') as UomCode \r\n\t\t\t,isnull(b.SalUnitMsr,'') as unitMsr\r\n\t\t\t,'N' as IsByPrdct\r\n            ,isnull(a.RjctQty,'') as RjctQty \r\n\t\t\tfrom owor a left join oitm b on a.ItemCode= b.ItemCode\r\n\t\t\tleft join oitw c on (c.itemcode=a.itemcode and c.WhsCode=a.Warehouse)\r\n                            where a.docentry=" + docEntry;
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadItemsByParentCode  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	private List<decimal> GetProductionOrderQuantity(int docEntry)
	{
		try
		{
			string query = "Apz_IssueForProduction_GetProductionOrder";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocEntry", docEntry));
			DataTable dataTable = ExecuteDataTable(query, CommandType.StoredProcedure, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				List<decimal> list2 = new List<decimal>();
				decimal item = Function.ParseDecimal(dataTable.Rows[0][0]);
				decimal item2 = Function.ParseDecimal(dataTable.Rows[0][1]);
				decimal item3 = Function.ParseDecimal(dataTable.Rows[0][2]);
				list2.Add(item);
				list2.Add(item2);
				list2.Add(item3);
				return list2;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "LoadItemsByParentCode  FAILED ", ex.Message);
			return null;
		}
	}

	public string CloseProductionOrder(int orderEntry)
	{
		string text = "";
		try
		{
			string query = "Apz_IssueForProduction_CloseProductionOrder";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocEntry", orderEntry));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC LINE FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string UpdateQtyProductionOrder(int orderEntry, decimal Qty, string tranType)
	{
		string text = "";
		try
		{
			string query = "Apz_IssueForProduction_UpdateQtyProductionOrder";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocEntry", orderEntry));
			list.Add(new SqlParameter("@Qty", Qty));
			list.Add(new SqlParameter("@tranType", tranType));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC LINE FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveFile(int absEntry)
	{
		string empty = string.Empty;
		try
		{
			string empty2 = string.Empty;
			empty2 = "insert into oatc (absEntry) values (" + absEntry + ")";
			if (ExecuteNonQuery(empty2, CommandType.Text, (SqlParameter[])null) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC file oatc table FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveLineFile(AttachmentFile_Impl line)
	{
		string empty = string.Empty;
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			bool flag = CheckLineFile(line.AbsEntry, line.Line);
			string empty2 = string.Empty;
			if (!flag)
			{
				empty2 = "INSERT INTO [dbo].[ATC1]\r\n                               ([AbsEntry]\r\n                               ,[Line]\r\n                               ,[srcPath]\r\n                               ,[trgtPath]\r\n                               ,[FileName]\r\n                               ,[FileExt]\r\n                               ,[Date]\r\n                               ,[UsrID]\r\n                               ,[Copied]\r\n                               ,[Override]\r\n                               ,[subPath])\r\n                         VALUES (";
				empty2 += "@AbsEntry ";
				list.Add(new SqlParameter("@AbsEntry", Function.ParseInt(line.AbsEntry)));
				empty2 += ",@Line ";
				list.Add(new SqlParameter("@Line", Function.ParseInt(line.Line)));
				empty2 += ", @srcPath ";
				list.Add(new SqlParameter("@srcPath", Function.ToString(line.srcPath)));
				empty2 += ",@trgtPath ";
				list.Add(new SqlParameter("@trgtPath", Function.ToString(line.trgtPath)));
				empty2 += ",@FileName ";
				list.Add(new SqlParameter("@FileName", Function.ToString(line.FileName)));
				empty2 += ", @FileExt ";
				list.Add(new SqlParameter("@FileExt", Function.ToString(line.FileExt)));
				empty2 += ",@Date ";
				list.Add(new SqlParameter("@Date", Function.ParseDateTime(line.Date)));
				empty2 += ",@UsrID ";
				list.Add(new SqlParameter("@UsrID", Function.ParseInt(line.UsrID)));
				empty2 += ", @Copied ";
				list.Add(new SqlParameter("@Copied", Function.ToString(line.Copied)));
				empty2 += ",@Override ";
				list.Add(new SqlParameter("@Override", Function.ToString(line.Override)));
				empty2 += ", @subPath ";
				list.Add(new SqlParameter("@subPath", Function.ToString(line.subPath)));
				empty2 += " )";
			}
			else
			{
				empty2 = "update [dbo].[ATC1] set ";
				empty2 += "srcPath=@srcPath ";
				list.Add(new SqlParameter("@srcPath", Function.ToString(line.srcPath)));
				empty2 += ",trgtPath=@trgtPath ";
				list.Add(new SqlParameter("@trgtPath", Function.ToString(line.trgtPath)));
				empty2 += ",FileName=@FileName ";
				list.Add(new SqlParameter("@FileName", Function.ToString(line.FileName)));
				empty2 += ", FileExt=@FileExt ";
				list.Add(new SqlParameter("@FileExt", Function.ToString(line.FileExt)));
				empty2 += ",Date=@Date ";
				list.Add(new SqlParameter("@Date", Function.ParseDateTime(line.Date)));
				empty2 += ",UsrID=@UsrID ";
				list.Add(new SqlParameter("@UsrID", Function.ParseInt(line.UsrID)));
				empty2 += ",Copied=@Copied ";
				list.Add(new SqlParameter("@Copied", Function.ToString(line.Copied)));
				empty2 += ",Override=@Override ";
				list.Add(new SqlParameter("@Override", Function.ToString(line.Override)));
				empty2 += ", subPath=@subPath ";
				list.Add(new SqlParameter("@subPath", Function.ToString(line.subPath)));
				empty2 += " where AbsEntry=@AbsEntry ";
				list.Add(new SqlParameter("@AbsEntry", Function.ParseInt(line.AbsEntry)));
				empty2 += " and Line=@Line ";
				list.Add(new SqlParameter("@Line", Function.ParseInt(line.Line)));
			}
			if (ExecuteNonQuery(empty2, CommandType.Text, list.ToArray()) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC LINE FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string CreateIssue(IssueForProduction_Impl_OIGE doc)
	{
		string result = "";
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string text = null;
			text = "INSERT INTO dbo.OIGE\r\n                               (DocEntry ,DocNum ,DocType ,CANCELED ,Handwrtten ,Printed\r\n                               ,DocStatus ,ObjType ,DocDate ,DocDueDate ,CardCode ,CardName\r\n                               ,Address ,NumAtCard ,VatSum ,VatSumFC ,DiscPrcnt ,DiscSum\r\n                               ,DiscSumFC  ,DocCur ,DocRate ,DocTotal ,DocTotalFC ,PaidToDate\r\n                               ,PaidFC ,GrosProfit ,GrosProfFC ,Ref1 ,Ref2 ,Comments ,JrnlMemo\r\n                               ,TransId ,ReceiptNum ,GroupNum ,SlpCode ,TrnspCode\r\n                               ,CntctCode ,VatSumSy ,DiscSumSy ,DocTotalSy ,PaidSys ,UpdateDate\r\n                               ,IsICT ,CreateDate ,Series ,TaxDate ,FinncPriod ,UserSign  ,UserSign2\r\n                               ,VatDate ,PaidSum ,PaidSumFc ,PaidSumSc  ,CurSource\r\n                               ,Requester ,ReqName ,Branch ,Department ,ReqDate,AtcEntry)\r\n                         VALUES (";
			text += "@DocEntry ";
			list.Add(new SqlParameter("@DocEntry", Function.ParseInt(doc.DocEntry)));
			text += ",@DocNum ";
			list.Add(new SqlParameter("@DocNum", Function.ParseInt(doc.DocNum)));
			text += ",@DocType ";
			list.Add(new SqlParameter("@DocType", Function.ToString(doc.DocType)));
			text += ",@CANCELED ";
			list.Add(new SqlParameter("@CANCELED", Function.ToString(doc.CANCELED)));
			text += ",@Handwrtten ";
			list.Add(new SqlParameter("@Handwrtten", Function.ToString(doc.Handwrtten)));
			text += ",@Printed ";
			list.Add(new SqlParameter("@Printed", Function.ToString(doc.Printed)));
			text += ",@DocStatus ";
			list.Add(new SqlParameter("@DocStatus", Function.ToString(doc.DocStatus)));
			text += ",@ObjType ";
			list.Add(new SqlParameter("@ObjType", Function.ToString(doc.ObjType)));
			text += ",@DocDate ";
			list.Add(new SqlParameter("@DocDate", Function.ParseDateTime(doc.DocDate)));
			text += ",@DocDueDate ";
			list.Add(new SqlParameter("@DocDueDate", Function.ParseDateTime(doc.DocDueDate)));
			text += ",@CardCode ";
			list.Add(new SqlParameter("@CardCode", Function.ToString(doc.CardCode)));
			text += ",@CardName ";
			list.Add(new SqlParameter("@CardName", Function.ToString(doc.CardName)));
			text += ",@Address ";
			list.Add(new SqlParameter("@Address", Function.ToString(doc.Address)));
			text += ",@NumAtCard ";
			list.Add(new SqlParameter("@NumAtCard", Function.ToString(doc.NumAtCard)));
			text += ",@VatSum ";
			list.Add(new SqlParameter("@VatSum", Function.ParseDecimal(doc.VatSum)));
			text += ",@VatSumFC ";
			list.Add(new SqlParameter("@VatSumFC", Function.ParseDecimal(doc.VatSumFC)));
			text += ",@DiscPrcnt ";
			list.Add(new SqlParameter("@DiscPrcnt", Function.ParseDecimal(doc.DiscPrcnt)));
			text += ",@DiscSum ";
			list.Add(new SqlParameter("@DiscSum", Function.ParseDecimal(doc.DiscSum)));
			text += ",@DiscSumFC ";
			list.Add(new SqlParameter("@DiscSumFC", Function.ParseDecimal(doc.DiscSumFC)));
			text += ",@DocCur ";
			list.Add(new SqlParameter("@DocCur", Function.ToString(doc.DocCur)));
			text += ",@DocRate ";
			list.Add(new SqlParameter("@DocRate", Function.ParseDecimal(doc.DocRate)));
			text += ",@DocTotal ";
			list.Add(new SqlParameter("@DocTotal", Function.ParseDecimal(doc.DocTotal)));
			text += ",@DocTotalFC ";
			list.Add(new SqlParameter("@DocTotalFC", Function.ParseDecimal(doc.DocTotalFC)));
			text += ",@PaidToDate ";
			list.Add(new SqlParameter("@PaidToDate", Function.ParseDecimal(doc.PaidToDate)));
			text += ",@PaidFC ";
			list.Add(new SqlParameter("@PaidFC", Function.ParseDecimal(doc.PaidFC)));
			text += ",@GrosProfit ";
			list.Add(new SqlParameter("@GrosProfit", Function.ParseDecimal(doc.GrosProfit)));
			text += ",@GrosProfFC ";
			list.Add(new SqlParameter("@GrosProfFC", Function.ParseDecimal(doc.GrosProfFC)));
			text += ",@Ref1 ";
			list.Add(new SqlParameter("@Ref1", Function.ToString(doc.Ref1)));
			text += ",@Ref2 ";
			list.Add(new SqlParameter("@Ref2", Function.ToString(doc.Ref2)));
			text += ",@Comments ";
			list.Add(new SqlParameter("@Comments", Function.ToString(doc.Comments)));
			text += ",@JrnlMemo ";
			list.Add(new SqlParameter("@JrnlMemo", Function.ToString(doc.JrnlMemo)));
			text += ",@TransId ";
			list.Add(new SqlParameter("@TransId", Function.ParseInt(doc.TransId)));
			text += ",@ReceiptNum ";
			list.Add(new SqlParameter("@ReceiptNum", Function.ParseInt(doc.ReceiptNum)));
			text += ",@GroupNum ";
			list.Add(new SqlParameter("@GroupNum", Function.ParseInt(doc.GroupNum)));
			text += ",@SlpCode ";
			list.Add(new SqlParameter("@SlpCode", Function.ParseInt(doc.SlpCode)));
			text += ",@TrnspCode ";
			list.Add(new SqlParameter("@TrnspCode", Function.ParseInt(doc.TrnspCode)));
			text += ",@CntctCode ";
			list.Add(new SqlParameter("@CntctCode", Function.ParseInt(doc.CntctCode)));
			text += ",@VatSumSy ";
			list.Add(new SqlParameter("@VatSumSy", Function.ParseDecimal(doc.VatSumSy)));
			text += ",@DiscSumSy ";
			list.Add(new SqlParameter("@DiscSumSy", Function.ParseDecimal(doc.DiscSumSy)));
			text += ",@DocTotalSy ";
			list.Add(new SqlParameter("@DocTotalSy", Function.ParseDecimal(doc.DocTotalSy)));
			text += ",@PaidSys ";
			list.Add(new SqlParameter("@PaidSys", Function.ParseDecimal(doc.PaidSys)));
			text += ",@UpdateDate ";
			list.Add(new SqlParameter("@UpdateDate", Function.ParseDateTime(doc.UpdateDate)));
			text += ",@IsICT ";
			list.Add(new SqlParameter("@IsICT", Function.ToString(doc.IsICT)));
			text += ",@CreateDate ";
			list.Add(new SqlParameter("@CreateDate", Function.ParseDateTime(doc.CreateDate)));
			text += ",@Series ";
			list.Add(new SqlParameter("@Series", Function.ParseInt(doc.Series)));
			text += ",@TaxDate ";
			list.Add(new SqlParameter("@TaxDate", Function.ParseDateTime(doc.TaxDate)));
			text += ",@FinncPriod ";
			list.Add(new SqlParameter("@FinncPriod", Function.ParseInt(doc.FinncPriod)));
			text += ",@UserSign ";
			list.Add(new SqlParameter("@UserSign", Function.ParseInt(doc.UserSign)));
			text += ",@UserSign2 ";
			list.Add(new SqlParameter("@UserSign2", Function.ParseInt(doc.UserSign2)));
			text += ",@VatDate ";
			list.Add(new SqlParameter("@VatDate", Function.ParseDateTime(doc.VatDate)));
			text += ",@PaidSum ";
			list.Add(new SqlParameter("@PaidSum", Function.ParseDecimal(doc.PaidSum)));
			text += ",@PaidSumFc ";
			list.Add(new SqlParameter("@PaidSumFc", Function.ParseDecimal(doc.PaidSumFc)));
			text += ",@PaidSumSc ";
			list.Add(new SqlParameter("@PaidSumSc", Function.ParseDecimal(doc.PaidSumSc)));
			text += ",@CurSource ";
			list.Add(new SqlParameter("@CurSource", Function.ToString(doc.CurSource)));
			text += ",@Requester ";
			list.Add(new SqlParameter("@Requester", Function.ToString(doc.Requester)));
			text += ",@ReqName ";
			list.Add(new SqlParameter("@ReqName", Function.ToString(doc.ReqName)));
			text += ",@Branch ";
			list.Add(new SqlParameter("@Branch", Function.ParseInt(doc.Branch)));
			text += ",@Department ";
			list.Add(new SqlParameter("@Department", Function.ParseInt(doc.Department)));
			text += ",@ReqDate ";
			list.Add(new SqlParameter("@ReqDate", Function.ParseDateTime(doc.ReqDate)));
			text += ",@AtcEntry ";
			list.Add(new SqlParameter("@AtcEntry", Function.ParseInt(doc.AtcEntry)));
			text += " )";
			if (ExecuteNonQuery(text, CommandType.Text, list.ToArray()) > 0)
			{
				foreach (IssueForProductionLine_Impl_IGE1 item in doc.Line_IGE1)
				{
					if (string.IsNullOrEmpty(CreateIssueLine(item)))
					{
						result = "";
						continue;
					}
					result = "ERROR";
					break;
				}
			}
			else
			{
				result = "ERROR";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CreateIssue ", ex.Message);
			result = ex.Message;
		}
		return result;
	}

	public string CreateIssueLine(IssueForProductionLine_Impl_IGE1 line)
	{
		string text = "";
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "Apz_IssueFromProduction_CreateReceiptLine";
			list.Add(new SqlParameter("@DocEntry", Function.ParseInt(line.DocEntry)));
			list.Add(new SqlParameter("@LineNum", Function.ParseInt(line.LineNum)));
			list.Add(new SqlParameter("@TargetType", Function.ParseInt(line.TargetType)));
			list.Add(new SqlParameter("@TrgetEntry", Function.ParseInt(line.TrgetEntry)));
			list.Add(new SqlParameter("@BaseRef", Function.ToString(line.BaseRef)));
			list.Add(new SqlParameter("@BaseType", Function.ParseInt(line.BaseType)));
			list.Add(new SqlParameter("@BaseEntry", Function.ParseInt(line.BaseEntry)));
			list.Add(new SqlParameter("@BaseLine", Function.ParseInt(line.BaseLine)));
			list.Add(new SqlParameter("@LineStatus", Function.ToString(line.LineStatus)));
			list.Add(new SqlParameter("@ItemCode", Function.ToString(line.ItemCode)));
			list.Add(new SqlParameter("@Dscription", Function.ToString(line.Dscription)));
			list.Add(new SqlParameter("@Quantity", Function.ParseDecimal(line.Quantity)));
			list.Add(new SqlParameter("@OpenQty", Function.ParseDecimal(line.OpenQty)));
			list.Add(new SqlParameter("@Price", Function.ParseDecimal(line.Price)));
			list.Add(new SqlParameter("@Currency", Function.ToString(line.Currency)));
			list.Add(new SqlParameter("@Rate", Function.ParseDecimal(line.Rate)));
			list.Add(new SqlParameter("@DiscPrcnt", Function.ParseDecimal(line.DiscPrcnt)));
			list.Add(new SqlParameter("@LineTotal", Function.ParseDecimal(line.LineTotal)));
			list.Add(new SqlParameter("@TotalFrgn", Function.ParseDecimal(line.TotalFrgn)));
			list.Add(new SqlParameter("@OpenSum", Function.ParseDecimal(line.OpenSum)));
			list.Add(new SqlParameter("@OpenSumFC", Function.ParseDecimal(line.OpenSumFC)));
			list.Add(new SqlParameter("@WhsCode", Function.ToString(line.WhsCode)));
			list.Add(new SqlParameter("@SlpCode", Function.ParseInt(line.SlpCode)));
			list.Add(new SqlParameter("@AcctCode", Function.ToString(line.AcctCode)));
			list.Add(new SqlParameter("@PriceBefDi", Function.ParseDecimal(line.PriceBefDi)));
			list.Add(new SqlParameter("@DocDate", Function.ParseDateTime(line.DocDate)));
			list.Add(new SqlParameter("@OcrCode", Function.ToString(line.OcrCode)));
			list.Add(new SqlParameter("@Project", Function.ToString(line.Project)));
			list.Add(new SqlParameter("@CodeBars", Function.ToString(line.CodeBars)));
			list.Add(new SqlParameter("@VatPrcnt", Function.ParseDecimal(line.VatPrcnt)));
			list.Add(new SqlParameter("@VatGroup", Function.ToString(line.VatGroup)));
			list.Add(new SqlParameter("@PriceAfVAT", Function.ParseDecimal(line.PriceAfVAT)));
			list.Add(new SqlParameter("@BaseDocNum", Function.ParseInt(line.BaseDocNum)));
			list.Add(new SqlParameter("@VatSum", Function.ParseDecimal(line.VatSum)));
			list.Add(new SqlParameter("@VatSumFrgn", Function.ParseDecimal(line.VatSumFrgn)));
			list.Add(new SqlParameter("@VatSumSy", Function.ParseDecimal(line.VatSumSy)));
			list.Add(new SqlParameter("@FinncPriod", Function.ParseInt(line.FinncPriod)));
			list.Add(new SqlParameter("@ObjType", Function.ToString(line.ObjType)));
			list.Add(new SqlParameter("@GrssProfit", Function.ParseDecimal(line.GrssProfit)));
			list.Add(new SqlParameter("@GrssProfFC", Function.ParseDecimal(line.GrssProfFC)));
			list.Add(new SqlParameter("@TaxCode", Function.ToString(line.TaxCode)));
			list.Add(new SqlParameter("@BaseQty", Function.ParseDecimal(line.BaseQty)));
			list.Add(new SqlParameter("@BaseOpnQty", Function.ParseDecimal(line.BaseOpnQty)));
			list.Add(new SqlParameter("@VatDscntPr", Function.ParseDecimal(line.VatDscntPr)));
			list.Add(new SqlParameter("@GTotal", Function.ParseDecimal(line.GTotal)));
			list.Add(new SqlParameter("@GTotalFC", Function.ParseDecimal(line.GTotalFC)));
			list.Add(new SqlParameter("@OcrCode2", Function.ToString(line.OcrCode2)));
			list.Add(new SqlParameter("@OcrCode3", Function.ToString(line.OcrCode3)));
			list.Add(new SqlParameter("@OcrCode4", Function.ToString(line.OcrCode4)));
			list.Add(new SqlParameter("@OcrCode5", Function.ToString(line.OcrCode5)));
			list.Add(new SqlParameter("@CogsOcrCo2", Function.ToString(line.CogsOcrCo2)));
			list.Add(new SqlParameter("@CogsOcrCo3", Function.ToString(line.CogsOcrCo3)));
			list.Add(new SqlParameter("@CogsOcrCo4", Function.ToString(line.CogsOcrCo4)));
			list.Add(new SqlParameter("@CogsOcrCo5", Function.ToString(line.CogsOcrCo5)));
			list.Add(new SqlParameter("@Factor1", Function.ParseDecimal(line.Factor1)));
			list.Add(new SqlParameter("@Factor2", Function.ParseDecimal(line.Factor2)));
			list.Add(new SqlParameter("@Factor3", Function.ParseDecimal(line.Factor3)));
			list.Add(new SqlParameter("@Factor4", Function.ParseDecimal(line.Factor4)));
			list.Add(new SqlParameter("@UseBaseUn", Function.ToString(line.UseBaseUn)));
			list.Add(new SqlParameter("@unitMsr", Function.ToString(line.unitMsr)));
			list.Add(new SqlParameter("@NumPerMsr", Function.ParseDecimal(line.NumPerMsr)));
			list.Add(new SqlParameter("@ShipDate", Function.ParseDateTime(line.ShipDate)));
			list.Add(new SqlParameter("@FreeTxt", Function.ToString(line.FreeTxt)));
			list.Add(new SqlParameter("@UomCode", Function.ToString(line.UomCode)));
			list.Add(new SqlParameter("@PQTReqdate", Function.ParseDateTime(line.PQTReqdate)));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CreateIssue ", ex.Message);
			return ex.Message;
		}
	}

	public string UpdateQtyProductionOrderLine(int orderEntry, decimal Qty, string tranType)
	{
		string text = "";
		try
		{
			string query = "Apz_IssueForProduction_UpdateQtyProductionOrderLine";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocEntry", orderEntry));
			list.Add(new SqlParameter("@Qty", Qty));
			list.Add(new SqlParameter("@tranType", tranType));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "UpdateQtyProductionOrderLine ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteOIGN(int docEntry)
	{
		throw new NotImplementedException();
	}

	public string DeleteIGN1(int docEntry, int lineNum)
	{
		try
		{
			string query = "delete from IGN1 where DocEntry = '" + docEntry + "' and LineNum = '" + lineNum + "'  ";
			if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) > 0)
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

	private string DeleteFile(int absEntry)
	{
		try
		{
			string query = "delete from ATC1 WHERE absEntry = " + absEntry;
			if (ExecuteNonQuery(query, CommandType.Text, (SqlParameter[])null) >= 0)
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

	public DataTable Search(ReceiptProduction_Impl_OIGN doc)
	{
		DataTable result = new DataTable();
		try
		{
			string empty = string.Empty;
			doc.UserSign = Function.ParseInt(empty);
			string text = "select *\r\n                               from OIGN a                                                                                                                  \r\n                               WHERE 1=1 ";
			if (doc.DocEntry == 0)
			{
				_ = doc.DocDate;
				if (doc.DocDate != Convert.ToDateTime(string.Empty))
				{
					text = text + " AND a.PostDate = '" + doc.DocDate.ToString() + "' ";
				}
				if (doc.Series != -1)
				{
					text = text + " AND a.Series = '" + doc.Series + "'";
				}
			}
			else
			{
				text = text + "AND a.DocEntry = '" + doc.DocEntry + "' ";
			}
			result = ExecuteData(text, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Search - Doc ", ex.Message);
		}
		return result;
	}

	public DataTable SearchLine(ReceiptProduction_LineImpl_IGN1 line)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "select \r\n                                isnull(a.docentry,0) as docentry,\r\n                                isnull(a.BaseRef,'') as BaseRef\r\n                                ,0 as LineNum\r\n                                , isnull(a.ItemCode,'') as ItemCode\r\n                                ,case isnull(TranType,'C')\r\n                                when 'C' then 'Complete'\r\n                                when 'R' then 'Reject'\r\n                                end as TranType\r\n                                ,isnull(c.ItemName,'') as Dscription\r\n                                ,a.Quantity as Quantity\r\n                                ,isnull(a.WhsCode,'') as WhsCode \r\n                                ,isnull(a.Price,0) as Price\r\n                                ,isnull(d.PlannedQty,0) as PlannedQty\r\n                                ,isnull(d.CmpltQty,0) as CmpltQty\r\n                                ,isnull(c.InvntryUom,'') as UomCode \r\n                                ,isnull(c.SalUnitMsr,'') as unitMsr\r\n                                ,'N' as IsByPrdct\r\n                                ,isnull(d.RjctQty,0) as RjctQty \r\n\r\n                            from IGN1 a left join OIGN b on a.DocEntry=b.DocEntry\t\r\n                            left join oitm c on a.itemcode=c.itemcode \r\n                            left join owor d \r\n                            on Convert(nvarchar(10),d.docentry)=a.baseref                           \t\t\t\t\t\t\t\t\t \r\n                         Where 1=1 ";
			text = text + "AND a.DocEntry = '" + line.DocEntry + "' ";
			result = ExecuteData(text, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Search - Line IGN1 ", ex.Message);
		}
		return result;
	}

	private bool CheckDoc(int docEntry)
	{
		try
		{
			string query = "SELECT docEntry\r\n                            FROM oign \r\n                            WHERE docEntry = " + docEntry;
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

	private bool CheckLine(int docEntry, int lineNum)
	{
		try
		{
			string query = "SELECT docEntry FROM ign1  WHERE\r\n                        docEntry = " + docEntry + " and  lineNum = " + lineNum;
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

	private bool CheckFile(int AbsEntry)
	{
		try
		{
			string query = "SELECT AbsEntry FROM oatc  WHERE\r\n                        AbsEntry = " + AbsEntry;
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

	private bool CheckLineFile(int AbsEntry, int Line)
	{
		try
		{
			string query = "SELECT AbsEntry FROM atc1  WHERE\r\n                        AbsEntry = " + AbsEntry + " and  Line = " + Line;
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

	public DataTable GetFirstRecord()
	{
		return GetUnitOfWork().Commons.GetFirstRecord("OIGN", "DocEntry");
	}

	public DataTable GetLastRecord()
	{
		return GetUnitOfWork().Commons.GetLastRecord("OIGN", "DocEntry");
	}

	public DataTable GetNextRecord(string docEntry)
	{
		return GetUnitOfWork().Commons.GetNextRecord("OIGN", "DocEntry", docEntry);
	}

	public DataTable GetPreviousRecord(string docEntry)
	{
		return GetUnitOfWork().Commons.GetPreviousRecord("OIGN", "DocEntry", docEntry);
	}
}
