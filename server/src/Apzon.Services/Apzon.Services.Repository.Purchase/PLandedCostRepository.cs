using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services.Interfaces.Purchase;

namespace Apzon.Services.Repository.Purchase;

public class PLandedCostRepository : BaseService<DocumentsImpl>, IILandedCost
{
	public PLandedCostRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public string Savesp_InsertWhseJournal(DocumentsImpl doc)
	{
		string empty = string.Empty;
		try
		{
			string empty2 = string.Empty;
			empty2 = "sp_InsertWhseJournal";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@documentNumber", doc.DocNum.ToString()));
			list.Add(new SqlParameter("@transType", doc.ObjType.ToString()));
			if (ExecuteNonQuery(empty2, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string Update(DocumentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public string Remove(DocumentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public DocumentsImpl GetById(int id)
	{
		throw new NotImplementedException();
	}

	public string Insert(DocumentsImpl entity)
	{
		throw new NotImplementedException();
	}

	public int GetMaxDocEntry()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(DocEntry ),0) as MaxDocEntry\r\n                            FROM OIPF ";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["MaxDocEntry"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	public bool Check(int docEntry)
	{
		try
		{
			string query = "SELECT   A.DocEntry \r\n                            FROM OIPF A \r\n                            WHERE A.DocEntry = '" + docEntry + "'";
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

	public string SaveDoc(DocumentsImpl doc)
	{
		string empty = string.Empty;
		try
		{
			string query = string.Format(" INSERT INTO [dbo].[OIPF] ([DocEntry]  ,[DocNum]  ,[DocDate] ,[DocDueDate]\r\n                                                   ,[CardCode]\r\n                                                   ,[SuppName]\r\n                                                   ,[AgentCode]\r\n                                                   ,[AgentName]\r\n                                                   ,[DocStatus]\r\n                                                   ,[AgentNum]\r\n                                                   ,[Descr]\r\n                                                   ,[Ref1]\r\n                                                   ,[Ref2]\r\n                                                   ,[DocCur]\r\n                                                   ,[DocRate]\r\n                                                   ,[ExpCustom]\r\n                                                   ,[ActCustom]\r\n                                                   ,[Vat1]\r\n                                                   ,[Vat2]\r\n                                                   ,[BeforeVat]\r\n                                                   ,[DocTotal]\r\n                                                   ,[CostSum]\r\n                                                   ,[DocTime]\r\n                                                   ,[Canceled]\r\n                                                   ,[ExCustomFC]\r\n                                                   ,[AcCustomFC]\r\n                                                   ,[Vat1FC]\r\n                                                   ,[Vat2FC]\r\n                                                   ,[BeforVatFC]\r\n                                                   ,[DocTotalFC]\r\n                                                   ,[CostSumFC]\r\n                                                   ,[CloseDate]\r\n                                                   ,[Cost_Match]\r\n                                                   ,[C_Match_FC]\r\n                                                   ,[BillOfLad]\r\n                                                   ,[TrnspCode]\r\n                                                   ,[CostFactor]\r\n                                                   ,[UpdateDate]\r\n                                                   ,[CreateDate]\r\n                                                   ,[TaxDate]\r\n                                                   ,[Series]\r\n                                                   ,[JdtNum]\r\n                                                   ,[JdtMemo]\r\n                                                   ,[UserSign]\r\n                                                   ,[ObjType]\r\n                                                   ,[ExCustomSC]\r\n                                                   ,[ActCustSC]\r\n                                                   ,[TtlCostSC]\r\n                                                   ,[OpenForLaC]\r\n                                                   ,[incCustom])\r\n                                             VALUES ({0}, {1}, N'{2}', N'{3}',N'{4}', N'{5}', N'{6}', N'{7}',\r\nN'{8}', N'{9}', N'{10}', N'{11}',N'{12}', N'{13}', {14}, {15},{16}, {17}, {18},\r\n{19},{20}, {21}, N'{22}', N'{23}',\r\n{24}, {25}, {26}, {26},{28}, {29}, {30}, N'{31}',{32}, {33},\r\nN'{34}', {35}, {36}, N'{37}',N'{38}', N'{39}', {40}, {41},N'{42}', {43}, N'{44}',\r\n{45}, {46}, {47}, N'{48}',N'{49}')", doc.DocEntry, doc.DocNum, doc.DocDate, doc.DocDueDate, doc.CardCode, doc.SuppName, doc.AgentCode, doc.AgentName, doc.DocStatus, doc.AgentNum, doc.Descr, doc.Ref1, doc.Ref2, doc.DocCur, doc.DocRate, doc.ExpCustom, doc.ActCustom, doc.Vat1, doc.Vat2, doc.BeforeVat, doc.DocTotal, doc.CostSum, doc.DocTime, doc.Canceled, doc.ExCustomFC, doc.AcCustomFC, doc.Vat1FC, doc.Vat2FC, doc.BeforVatFC, doc.DocTotalFC, doc.CostSumFC, doc.CloseDate, doc.Cost_Match, doc.C_Match_FC, doc.BillOfLad, doc.TrnspCode, doc.CostFactor, doc.UpdateDate, doc.CreateDate, doc.TaxDate, doc.Series, doc.JdtNum, doc.JdtMemo, doc.UserSign, doc.ObjType, doc.ExCustomSC, doc.ActCustSC, doc.TtlCostSC, doc.OpenForLaC, doc.incCustom);
			if (ExecuteNonQueryNoParam(query) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDoc(string docEntry)
	{
		try
		{
			string query = " delete OIPF where docEntry = " + docEntry;
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

	public DataTable GetLandedCosts(string docEntry)
	{
		try
		{
			string query = "SELECT \r\n                                   isnull( [DocEntry] , 0) as DocEntry\r\n                                          , isnull([DocNum],0) as DocNum\r\n                                         , [DocDate]\r\n                                       ,  [DocDueDate]\r\n                                      ,  isnull(    [CardCode],'') as CardCode\r\n                                   ,    isnull(    [SuppName],'') as SuppName\r\n                                  ,      isnull(   [AgentCode],'') as AgentCode\r\n                                        ,  isnull( [AgentName] ,'')  as AgentName\r\n                                        ,  isnull( [DocStatus] ,'O') as DocStatus\r\n                                         , isnull( [AgentNum] , '') as AgentNum\r\n                                      , isnull(    [Descr],'') as Descr\r\n                                    , isnull(      [Ref1],'') as Ref1\r\n                                          , isnull([Ref2],'') as Ref2\r\n                                         , isnull( [DocCur],'') as DocCur\r\n                                         ,isnull(  [DocRate],0) as DocRate\r\n                                           ,isnull([ExpCustom],0) as ExpCustom\r\n                                          , isnull([ActCustom],0) as ActCustom\r\n                                           ,isnull([Vat1],0) as Vat1\r\n                                          , isnull([Vat2],0) as Vat2\r\n                                           ,isnull([BeforeVat],0) as BeforeVat\r\n                                         , isnull( [DocTotal],0) as DocTotal\r\n                                           ,isnull([CostSum],0) as CostSum\r\n                                           ,[DocTime]\r\n                                          ,Case  When Canceled = null then 'N'\r\n\t\t\t\t\t\t\t\t\t\t          When Canceled= '' then 'N'         \t\t\t\t\t\t\t\t\t\t\r\n\t\t\t\t\t\t\t\t\t\t   end Canceled\r\n                                          , isnull([ExCustomFC], 0) as ExCustomFC\r\n                                           ,isnull([AcCustomFC], 0) as AcCustomFC\r\n                                           ,isnull([Vat1FC], 0) as Vat1FC\r\n                                          ,isnull( [Vat2FC], 0) as Vat2FC\r\n                                           ,isnull([BeforVatFC], 0) as BeforVatFC\r\n                                          , isnull([DocTotalFC], 0) as DocTotalFC\r\n                                          , isnull([CostSumFC], 0) as CostSumFC\r\n                                          , [CloseDate]\r\n                                          , isnull([Cost_Match], 0) as Cost_Match\r\n                                         ,  isnull([C_Match_FC], 0) as C_Match_FC\r\n                                          ,isnull( [BillOfLad], '') as BillOfLad\r\n                                          , isnull([TrnspCode], 0) as TrnspCode\r\n                                          , isnull([CostFactor], 0) as CostFactor\r\n                                          , [UpdateDate]\r\n                                           ,[CreateDate]\r\n                                          , [TaxDate]\r\n                                         , isnull( [Series], 0) as Series\r\n                                         , isnull( [JdtNum], 0) as JdtNum\r\n                                          , isnull([JdtMemo], '') as JdtMemo\r\n                                          , isnull([UserSign], 0) as UserSign\r\n                                         , isnull(  [ObjType], '') as ObjType\r\n                                         , isnull( [ExCustomSC], 0) as ExCustomSC\r\n                                          , isnull([ActCustSC], 0) as ActCustSC\r\n                                       ,   isnull( [TtlCostSC], 0) as TtlCostSC\r\n                                          , isnull([OpenForLaC], '') as OpenForLaC\r\n                                          , isnull([incCustom], '') as incCustom\r\n                                    --    , isnull(  [Uabc]\r\n                                      --   , isnull( [Udfg]\r\n                                        --  , isnull([Utest]\r\n\r\n                                    FROM OIPF where docentry=" + docEntry;
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return dataTable;
			}
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable LoadUDFData(string docEntry)
	{
		try
		{
			List<string> uDFField = getUDFField();
			string text = "";
			foreach (string item in uDFField)
			{
				text = text + item + ",";
			}
			text = text.Remove(text.Length - 1);
			string query = "select " + text + " from OIPF where docentry=" + docEntry;
			return ExecuteDataTable(query, CommandType.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	private List<string> getUDFField()
	{
		try
		{
			string query = "select FieldName from tblUFieldTable where TableName='OIPF'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			List<string> list = new List<string>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				list.Add(dataTable.Rows[i][0].ToString());
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new List<string>();
		}
	}

	public string SaveUDF(List<Common.Table_ID_Code> lst)
	{
		try
		{
			string text = "";
			string text2 = "";
			text2 = lst[lst.Count - 1].ID.ToString();
			lst.RemoveAt(lst.Count - 1);
			foreach (Common.Table_ID_Code item in lst)
			{
				text = ((!(item.Name == "CheckEdit") && !(item.Name == "CalcEdit")) ? (text + item.ID?.ToString() + "=N'" + item.Code + "',") : (text + item.ID?.ToString() + "=" + item.Code + ","));
			}
			text = text.Remove(text.Length - 1);
			string query = "update OIPF set " + text + " where docEntry=" + text2;
			if (ExecuteNonQueryNoParam(query) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	public DataTable Search(DocumentsImpl doc)
	{
		DataTable result = new DataTable();
		try
		{
			string text = "where ((1=1) ";
			if (!string.IsNullOrEmpty(doc.CardCode))
			{
				text = text + "and (CardCode like upper( N'%" + doc.CardCode + "%')) ";
			}
			if (!string.IsNullOrEmpty(doc.CardName))
			{
				text = text + "and (CardName like upper( N'%" + doc.CardName + "%')) ";
			}
			if (doc.DocEntry > 0)
			{
				text = text + "and (DocEntry =" + doc.DocEntry + ") ";
			}
			text += ") ";
			string query = "SELECT \r\n                                   isnull( [DocEntry] , 0) as DocEntry\r\n                                          , isnull([DocNum],0) as DocNum\r\n                                         , [DocDate]\r\n                                       ,  [DocDueDate]\r\n                                      ,  isnull(    [CardCode],'') as CardCode\r\n                                   ,    isnull(    [SuppName],'') as SuppName\r\n                                  ,      isnull(   [AgentCode],'') as AgentCode\r\n                                        ,  isnull( [AgentName] ,'')  as AgentName\r\n                                        ,  isnull( [DocStatus] ,'O') as DocStatus\r\n                                         , isnull( [AgentNum] , '') as AgentNum\r\n                                      , isnull(    [Descr],'') as Descr\r\n                                    , isnull(      [Ref1],'') as Ref1\r\n                                          , isnull([Ref2],'') as Ref2\r\n                                         , isnull( [DocCur],'') as DocCur\r\n                                         ,isnull(  [DocRate],0) as DocRate\r\n                                           ,isnull([ExpCustom],0) as ExpCustom\r\n                                          , isnull([ActCustom],0) as ActCustom\r\n                                           ,isnull([Vat1],0) as Vat1\r\n                                          , isnull([Vat2],0) as Vat2\r\n                                           ,isnull([BeforeVat],0) as BeforeVat\r\n                                         , isnull( [DocTotal],0) as DocTotal\r\n                                           ,isnull([CostSum],0) as CostSum\r\n                                           ,[DocTime]\r\n                                          ,Case  When Canceled = null then 'N'\r\n\t\t\t\t\t\t\t\t\t\t          When Canceled= '' then 'N'         \t\t\t\t\t\t\t\t\t\t\r\n\t\t\t\t\t\t\t\t\t\t   end Canceled\r\n                                          , isnull([ExCustomFC], 0) as ExCustomFC\r\n                                           ,isnull([AcCustomFC], 0) as AcCustomFC\r\n                                           ,isnull([Vat1FC], 0) as Vat1FC\r\n                                          ,isnull( [Vat2FC], 0) as Vat2FC\r\n                                           ,isnull([BeforVatFC], 0) as BeforVatFC\r\n                                          , isnull([DocTotalFC], 0) as DocTotalFC\r\n                                          , isnull([CostSumFC], 0) as CostSumFC\r\n                                          , [CloseDate]\r\n                                          , isnull([Cost_Match], 0) as Cost_Match\r\n                                         ,  isnull([C_Match_FC], 0) as C_Match_FC\r\n                                          ,isnull( [BillOfLad], '') as BillOfLad\r\n                                          , isnull([TrnspCode], 0) as TrnspCode\r\n                                          , isnull([CostFactor], 0) as CostFactor\r\n                                          , [UpdateDate]\r\n                                           ,[CreateDate]\r\n                                          , [TaxDate]\r\n                                         , isnull( [Series], 0) as Series\r\n                                         , isnull( [JdtNum], 0) as JdtNum\r\n                                          , isnull([JdtMemo], '') as JdtMemo\r\n                                          , isnull([UserSign], 0) as UserSign\r\n                                         , isnull(  [ObjType], '') as ObjType\r\n                                         , isnull( [ExCustomSC], 0) as ExCustomSC\r\n                                          , isnull([ActCustSC], 0) as ActCustSC\r\n                                       ,   isnull( [TtlCostSC], 0) as TtlCostSC\r\n                                          , isnull([OpenForLaC], '') as OpenForLaC\r\n                                          , isnull([incCustom], '') as incCustom\r\n                                    --    , isnull(  [Uabc]\r\n                                      --   , isnull( [Udfg]\r\n                                        --  , isnull([Utest]\r\n\r\n                                    FROM OIPF " + text;
			result = ExecuteData(query, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Return Search - Doc ", ex.Message);
		}
		return result;
	}

	public DataTable GetLandedCostItem(string tblLine, string docNums)
	{
		try
		{
			string query = " SELECT isnull( [DocEntry],0) as DocEntry  ,isnull ( [LineNum],0) as LineNum  ,[TargetType] ,isnull([TrgetEntry],0 ) as TrgetEntry, isnull([BaseRef],0) as BaseRef  ,[BaseType]\r\n                              , isnull ( [BaseEntry],0) as BaseEntry  ,[BaseLine] ,isnull ( [LineStatus],'O') as PDN1LineStatus ,isnull( [ItemCode],'') as ItemCode ,isnull ( [Dscription],'') as Dscription,isnull([Quantity],0) as [Quantity] ,isnull([OpenQty],0) as [OpenQty] ,isnull([Price],0) as [Price] , isnull ([Currency],'##') as Currency\r\n                              ,isnull([Rate],0) as [Rate]  ,isnull([DiscPrcnt],0) as [DiscPrcnt] ,isnull([LineTotal],0) as [LineTotal] ,isnull([TotalFrgn],0) as [TotalFrgn]  ,isnull([OpenSum],0) as [OpenSum] ,isnull([OpenSumFC],0) as [OpenSumFC] , isnull( [WhsCode],'') as WhsCode  ,isnull( [SlpCode],0) as SlpCode , isnull ([AcctCode],0) as AcctCode\r\n                              ,isnull ( [PriceBefDi],0) as PriceBefDi  , [DocDate] ,isnull( [OcrCode],'') as OcrCode ,[Project] ,[CodeBars] ,[VatPrcnt] , [VatGroup]\r\n                              ,[PriceAfVAT] ,[BaseDocNum] ,isnull ( [VatSum],0) as VatSum ,isnull( [VatSumFrgn],0) as VatSumFrgn ,isnull( [VatSumSy],0) as VatSumSy  ,[FinncPriod] ,[ObjType] ,[GrssProfit] ,[GrssProfFC]\r\n                              ,[TaxCode] ,[BaseQty] ,[BaseOpnQty],[VatDscntPr] ,[GTotal],[GTotalFC] ,isnull ([OcrCode2],'') as OcrCode2 , isnull ([OcrCode3],'') as OcrCode3 , isnull([OcrCode4],'') as OcrCode4\r\n                              ,isnull( [OcrCode5],'')  as OcrCode5,isnull([CogsOcrCo2],'') as CogsOcrCo2 ,isnull([CogsOcrCo3],'') as CogsOcrCo3, isnull( [CogsOcrCo4],'') as CogsOcrCo4, isnull( [CogsOcrCo5],'') as CogsOcrCo5 ,isnull([Factor1],0) as Factor1,isnull([Factor2],0) as Factor2\r\n                                ,isnull([Factor3],0) as Factor3,isnull([Factor4],0) as Factor4\r\n                              ,[UseBaseUn],[unitMsr],isnull([NumPerMsr],0) as NumPerMsr,isnull([ShipDate],'') as ShipDate,isnull([FreeTxt],'') as FreeTxt,[UoMCode],isnull([LineVat],0) as LineVat, isnull ([LinManClsd],'N') as LinManClsd\r\n                          \r\n                 from  " + tblLine + "  where DocEntry in (" + docNums + ") and LineStatus='O' ";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public DataTable GetLandedCostItemByDocEntry(string docEntry)
	{
		try
		{
			string query = "select \r\n                               isnull( [DocEntry],0 ) as DocEntry\r\n                                      ,isnull([LineNum],0 ) as LineNum\r\n                                      ,isnull([BaseType],0 ) as BaseType\r\n                                      ,isnull([BaseEntry],0 ) as BaseEntry\r\n                                      ,isnull([ItemCode],'' ) as ItemCode\r\n                                      ,isnull([Dscription],'' ) as Dscription\r\n                                      ,isnull([Quantity], 0) as Quantity\r\n                                      ,isnull([PriceFOB],0 ) as PriceFOB\r\n                                      ,isnull([Currency],'' ) as Currency\r\n                                      ,isnull([Rate], 0 ) as Rate\r\n                                      ,isnull([Custom], 0) as Custom\r\n                                      ,isnull([CustomFC],0 ) as CustomFC\r\n                                      ,isnull([Cost],0 ) as Cost\r\n                                      ,isnull([CostFC],0 ) as CostFC\r\n                                      ,isnull([PriceAtWH],0 ) as PriceAtWH\r\n                                      ,isnull([PricAtWHFC],0 ) as PricAtWHFC\r\n                                      ,isnull([LineTotal],0 ) as LineTotal\r\n                                      ,isnull([TotalFrgn],0 ) as TotalFrgn\r\n                                      ,isnull([Volume],0 ) as Volume\r\n                                      ,isnull([UnitCode],0 ) as UnitCode\r\n                                      ,isnull([Weight1],0 ) as Weight1\r\n                                      ,isnull([UnitCode1],0 ) as UnitCode1\r\n                                      ,isnull([Weight2],0 ) as Weight2\r\n                                      ,isnull([UnitCode2],0 ) as UnitCode2\r\n                                      ,isnull([CardCode],'' ) as CardCode\r\n                                      ,isnull([Reference],'' ) as Reference\r\n                                      ,isnull([OrigLine],0 ) as OrigLine\r\n                                      ,isnull([FactNoCust], 0) as FactNoCust\r\n                                      ,isnull([FacWthCust], 0) as FacWthCust\r\n                                      ,isnull([PriceList],0 ) as PriceList\r\n                                      ,isnull([CostOH],'Y' ) as CostOH\r\n                                      ,isnull([StockEval],'Y' ) as StockEval\r\n                                      ,isnull([UseBaseUn],'N' ) as UseBaseUn\r\n                                      ,isnull([BlockNum], '') as BlockNum\r\n                                      ,isnull([ImportLog], '') as ImportLog\r\n                                      ,isnull([WhsCode],'' ) as WhsCode\r\n                                      ,isnull([OrigRow], 0) as OrigRow\r\n                                      ,isnull([Locked],'N' ) as Locked\r\n                                      ,isnull([OrigWhs],'' ) as OrigWhs\r\n                                      ,isnull([ReleaseNum],0 ) as ReleaseNum\r\n                                      ,isnull([VarCosts], 0) as VarCosts\r\n                                      ,isnull([ConstCosts],0 ) as ConstCosts\r\n                                      ,isnull([VarCostsFR],0 ) as VarCostsFR\r\n                                      ,isnull([CnstCostFR],0 ) as CnstCostFR\r\n                                      ,isnull([UserCustom],0 ) as UserCustom\r\n                                      ,isnull([UsrCusomFC],0 ) as UsrCusomFC\r\n                                      ,isnull([FobValue],0 ) as FobValue\r\n                                      ,isnull([FobValueFC], 0) as FobValueFC\r\n                                      ,isnull([TtlExpndLC],0 ) as TtlExpndLC\r\n                                      ,isnull([TtlExpndFC],0 ) as TtlExpndFC\r\n                                      ,isnull([TtlCustLC],0 ) as TtlCustLC\r\n                                      ,isnull([TtlCustFC],0 ) as TtlCustFC\r\n                                      ,isnull([TtlCostLC],0 ) as TtlCostLC\r\n                                      ,isnull([TtlCostFC], 0) as TtlCostFC\r\n                                      ,isnull([TtlVolume],0 ) as TtlVolume\r\n                                      ,isnull([TtlWeight],0 ) as TtlWeight\r\n                                      ,isnull([BaseRowNum],0 ) as BaseRowNum\r\n                                      ,isnull([TtlCustSC],0 ) as TtlCustSC\r\n                                      ,isnull([TtlExpndSC],0 ) as TtlExpndSC\r\n                                      ,isnull([GRPOAbsEnt],0 ) as GRPOAbsEnt\r\n                                      ,isnull([GRPOLineNu],0 ) as GRPOLineNu\r\n                                      ,isnull([TargetDoc],0 ) as TargetDoc\r\n                                      ,isnull([FobValCurr],'' ) as FobValCurr\r\n                                      ,isnull([FobnLaC],0 ) as FobnLaC\r\n                                      ,isnull([FobnLaCFC],0 ) as FobnLaCFC\r\n                                      ,isnull([NumPerMsr],0 ) as NumPerMsr\r\n                                  FROM [dbo].[IPF1] where docentry=" + docEntry;
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public string SaveDocItemLine(Document_LinesImpl doc)
	{
		string empty = string.Empty;
		try
		{
			bool flag = CheckCostItem(doc.DocEntry, doc.LineNum);
			string empty2 = string.Empty;
			empty2 = (flag ? $"UPDATE OIPF  SET \r\n            (LineNum={doc.LineNum}\r\n           ,<BaseType={doc.BaseType}\r\n           ,<BaseEntry={doc.BaseEntry}\r\n           ,<ItemCode=N'{doc.ItemCode}'\r\n           ,<DscriptionN'{doc.Dscription}'\r\n           ,<Quantity={doc.Quantity}\r\n           ,<PriceFOB={doc.PriceFOB}\r\n           ,<CurrencyN'{doc.Currency}'\r\n           ,<Rate={doc.Rate}\r\n           ,<Custom={doc.Custom}\r\n           ,<CustomFC={doc.CustomFC}\r\n           ,<Cost={doc.Cost}\r\n           ,<CostFC={doc.CostFC}\r\n           ,<PriceAtWH={doc.PriceAtWH}\r\n           ,<PricAtWHFC={doc.PricAtWHFC}\r\n           ,<LineTotal={doc.LineTotal}\r\n           ,<TotalFrgn={doc.TotalFrgn}\r\n           ,<Volume={doc.Volume}\r\n           ,<UnitCode={doc.UnitCode}\r\n           ,<Weight1={doc.Weight1}\r\n           ,<UnitCode1={doc.UnitCode1}\r\n           ,<Weight2={doc.Weight2}\r\n           ,<UnitCode2={doc.UnitCode2}\r\n           ,<CardCode=N'{doc.CardCode}'\r\n           ,<Reference=N'{doc.Reference}'\r\n           ,<OrigLine={doc.OrigLine}\r\n           ,<FactNoCust={doc.FactNoCust}\r\n           ,<FacWthCust={doc.FacWthCust}\r\n           ,<PriceList={doc.PriceList}\r\n           ,<CostOH=N'{doc.CostOH}'\r\n           ,<StockEval=N'{doc.StockEval}'\r\n           ,<UseBaseUn=N'{doc.UseBaseUn}'\r\n           ,<BlockNum=N'{doc.BlockNum}'\r\n           ,<ImportLog=N'{doc.ImportLog}'\r\n           ,<WhsCode=N'{doc.WhsCode}'\r\n           ,<OrigRow={doc.OrigRow}\r\n           ,<Locked=N'{doc.Locked}'\r\n           ,<OrigWhs=N'{doc.OrigWhs}'\r\n           ,<ReleaseNum={doc.ReleaseNum}\r\n           ,<VarCosts={doc.VarCosts}\r\n           ,<ConstCosts={doc.ConstCosts}\r\n           ,<VarCostsFR={doc.VarCostsFR}\r\n           ,<CnstCostFR={doc.CnstCostFR}\r\n           ,<UserCustom={doc.UserCustom}\r\n           ,<UsrCusomFC={doc.UsrCusomFC}\r\n           ,<FobValue={doc.FobValue}\r\n           ,<FobValueFC={doc.FobValueFC}\r\n           ,<TtlExpndLC={doc.TtlExpndLC}\r\n           ,<TtlExpndFC={doc.TtlExpndFC}\r\n           ,<TtlCustLC={doc.TtlCustLC}\r\n           ,<TtlCustFC={doc.TtlCustFC}\r\n           ,<TtlCostLC={doc.TtlCostLC}\r\n           ,<TtlCostFC={doc.TtlCostFC}\r\n           ,<TtlVolume={doc.TtlVolume}\r\n           ,<TtlWeight={doc.TtlWeight}\r\n           ,<BaseRowNum={doc.BaseRowNum}\r\n           ,<TtlCustSC={doc.TtlCustSC}\r\n           ,<TtlExpndSC={doc.TtlExpndSC}\r\n           ,<GRPOAbsEnt={doc.GRPOAbsEnt}\r\n           ,<GRPOLineNu={doc.GRPOLineNu}\r\n           ,<TargetDoc={doc.TargetDoc}\r\n           ,<FobValCurr=N'{doc.FobValCurr}'\r\n           ,<FobnLaC={doc.FobnLaC}\r\n           ,<FobnLaCFC={doc.FobnLaCFC}\r\n           ,<NumPerMsr={doc.NumPerMsr}) where docentry={doc.DocEntry}" : $"INSERT INTO [dbo].[IPF1]\r\n           ([DocEntry]\r\n           ,[LineNum]\r\n           ,[BaseType]\r\n           ,[BaseEntry]\r\n           ,[ItemCode]\r\n           ,[Dscription]\r\n           ,[Quantity]\r\n           ,[PriceFOB]\r\n           ,[Currency]\r\n           ,[Rate]\r\n           ,[Custom]\r\n           ,[CustomFC]\r\n           ,[Cost]\r\n           ,[CostFC]\r\n           ,[PriceAtWH]\r\n           ,[PricAtWHFC]\r\n           ,[LineTotal]\r\n           ,[TotalFrgn]\r\n           ,[Volume]\r\n           ,[UnitCode]\r\n           ,[Weight1]\r\n           ,[UnitCode1]\r\n           ,[Weight2]\r\n           ,[UnitCode2]\r\n           ,[CardCode]\r\n           ,[Reference]\r\n           ,[OrigLine]\r\n           ,[FactNoCust]\r\n           ,[FacWthCust]\r\n           ,[PriceList]\r\n           ,[CostOH]\r\n           ,[StockEval]\r\n           ,[UseBaseUn]\r\n           ,[BlockNum]\r\n           ,[ImportLog]\r\n           ,[WhsCode]\r\n           ,[OrigRow]\r\n           ,[Locked]\r\n           ,[OrigWhs]\r\n           ,[ReleaseNum]\r\n           ,[VarCosts]\r\n           ,[ConstCosts]\r\n           ,[VarCostsFR]\r\n           ,[CnstCostFR]\r\n           ,[UserCustom]\r\n           ,[UsrCusomFC]\r\n           ,[FobValue]\r\n           ,[FobValueFC]\r\n           ,[TtlExpndLC]\r\n           ,[TtlExpndFC]\r\n           ,[TtlCustLC]\r\n           ,[TtlCustFC]\r\n           ,[TtlCostLC]\r\n           ,[TtlCostFC]\r\n           ,[TtlVolume]\r\n           ,[TtlWeight]\r\n           ,[BaseRowNum]\r\n           ,[TtlCustSC]\r\n           ,[TtlExpndSC]\r\n           ,[GRPOAbsEnt]\r\n           ,[GRPOLineNu]\r\n           ,[TargetDoc]\r\n           ,[FobValCurr]\r\n           ,[FobnLaC]\r\n           ,[FobnLaCFC]\r\n           ,[NumPerMsr])\r\n     VALUES\r\n           ({doc.DocEntry}\r\n           ,{doc.LineNum}\r\n           ,{doc.BaseType}\r\n           ,{doc.BaseEntry}\r\n           ,N'{doc.ItemCode}'\r\n           ,N'{doc.Dscription}'\r\n           ,{doc.Quantity}\r\n           ,{doc.PriceFOB}\r\n           ,N'{doc.Currency}'\r\n           ,{doc.Rate}\r\n           ,{doc.Custom}\r\n           ,{doc.CustomFC}\r\n           ,{doc.Cost}\r\n           ,{doc.CostFC}\r\n           ,{doc.PriceAtWH}\r\n           ,{doc.PricAtWHFC}\r\n           ,{doc.LineTotal}\r\n           ,{doc.TotalFrgn}\r\n           ,{doc.Volume}\r\n           ,{doc.UnitCode}\r\n           ,{doc.Weight1}\r\n           ,{doc.UnitCode1}\r\n           ,{doc.Weight2}\r\n           ,{doc.UnitCode2}\r\n           ,N'{doc.CardCode}'\r\n           ,N'{doc.Reference}'\r\n           ,{doc.OrigLine}\r\n           ,{doc.FactNoCust}\r\n           ,{doc.FacWthCust}\r\n           ,{doc.PriceList}\r\n           ,N'{doc.CostOH}'\r\n           ,N'{doc.StockEval}'\r\n           ,N'{doc.UseBaseUn}'\r\n           ,N'{doc.BlockNum}'\r\n           ,N'{doc.ImportLog}'\r\n           ,N'{doc.WhsCode}'\r\n           ,{doc.OrigRow}\r\n           ,N'{doc.Locked}'\r\n           ,N'{doc.OrigWhs}'\r\n           ,{doc.ReleaseNum}\r\n           ,{doc.VarCosts}\r\n           ,{doc.ConstCosts}\r\n           ,{doc.VarCostsFR}\r\n           ,{doc.CnstCostFR}\r\n           ,{doc.UserCustom}\r\n           ,{doc.UsrCusomFC}\r\n           ,{doc.FobValue}\r\n           ,{doc.FobValueFC}\r\n           ,{doc.TtlExpndLC}\r\n           ,{doc.TtlExpndFC}\r\n           ,{doc.TtlCustLC}\r\n           ,{doc.TtlCustFC}\r\n           ,{doc.TtlCostLC}\r\n           ,{doc.TtlCostFC}\r\n           ,{doc.TtlVolume}\r\n           ,{doc.TtlWeight}\r\n           ,{doc.BaseRowNum}\r\n           ,{doc.TtlCustSC}\r\n           ,{doc.TtlExpndSC}\r\n           ,{doc.GRPOAbsEnt}\r\n           ,{doc.GRPOLineNu}\r\n           ,{doc.TargetDoc}\r\n           ,N'{doc.FobValCurr}'\r\n           ,{doc.FobnLaC}\r\n           ,{doc.FobnLaCFC}\r\n           ,{doc.NumPerMsr})");
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public bool CheckCostItem(int docEntry, int lineNum)
	{
		try
		{
			string query = "SELECT docEntry \r\n                            FROM IPF1\r\n                            WHERE DocEntry = " + docEntry + " and lineNum=" + lineNum;
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

	public DataTable GetLandedCostLineByDocEntry(string docEntry)
	{
		try
		{
			string query = "SELECT a.*,b.AlcName FROM  IPF2 a inner join oalc b on a.AlcCode=b.AlcCode where docentry=" + docEntry;
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public string SaveDocCostLine(Document_LinesImpl doc)
	{
		string empty = string.Empty;
		try
		{
			bool flag = CheckCostLine(doc.DocEntry, doc.AlcCode);
			string empty2 = string.Empty;
			empty2 = (flag ? $"UPDATE IPF2  SET\r\n                                         [AlcCode] =N'{doc.AlcCode}',[OhType] =N'{doc.OhType}', [CostSum] ={doc.CostSum},\r\n                                             ,[CostSumFC]   ={doc.CostSumFC},[Factor] ={doc.Factor} ,[CostType]  =N'{doc.CostType}',[Locked] =N'{doc.Locked}'\r\n                                        ,[LaCAllcAcc] =N'{doc.LaCAllcAcc}' ,[CostSumSC]  ={doc.CostSumSC},[InCustCalc] =N'{doc.InCustCalc}'\r\n                                                     WHERE [DocEntry] ={doc.DocEntry}" : $"INSERT INTO IPF2  ([DocEntry] , [AlcCode],[OhType], [CostSum]\r\n                                             ,[CostSumFC],[Factor] ,[CostType] ,[Locked] ,[LaCAllcAcc]\r\n                                                   ,[CostSumSC] ,[InCustCalc])   \r\n                                                     VALUES ({doc.DocEntry}, N'{doc.AlcCode}', N'{doc.OhType}', {doc.CostSum}\r\n                                                            , {doc.CostSumFC}, {doc.Factor}, N'{doc.CostType}' , N'{doc.Locked}', N'{doc.LaCAllcAcc}', {doc.CostSumSC}\r\n                                                            , N'{doc.InCustCalc}')");
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveDocCostLine  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public bool CheckCostLine(int docEntry, string AlcCode)
	{
		try
		{
			string query = "SELECT docEntry \r\n                            FROM IPF2\r\n                            WHERE DocEntry = " + docEntry + " and AlcCode='" + AlcCode + "'";
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

	public DataTable GetLandedCostCode()
	{
		try
		{
			string query = "SELECT * FROM  OALC";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public int GetMaxDocEntryCostCode()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(AlcCode ),0) as AlcCode\r\n                            FROM  oalc";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["AlcCode"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	public string SaveDocCode(Document_LinesImpl doc)
	{
		string empty = string.Empty;
		try
		{
			bool flag = CheckCode(int.Parse(doc.AlcCode));
			string empty2 = string.Empty;
			empty2 = (flag ? $"UPDATE OALC  SET AlcName=N'{doc.AlcName}', OhType=N'{doc.OhType}' , AcctCode=N'{doc.AcctCode}'\r\n                                                     , UserSign=N'{doc.UserSign}', LaCAllcAcc=N'{doc.LaCAllcAcc}' WHERE AlcCode={doc.AlcCode}" : $"INSERT INTO [OALC]  (AlcCode , AlcName,OhType, AcctCode,UserSign,LaCAllcAcc)\r\n                                                     VALUES (N'{doc.AlcCode}', N'{doc.AlcName}', N'{doc.OhType}', N'{doc.AcctCode}', {doc.UserSign}, N'{doc.LaCAllcAcc}' )");
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string DeleteDocCode(string code)
	{
		try
		{
			string query = " delete OALC where AlcCode = '" + code + "'";
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

	public bool CheckCode(int docEntry)
	{
		try
		{
			string query = "SELECT   alcCode   FROM  OALC WHERE alcCode = '" + docEntry + "'";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Read data OCRD  FAILED ", ex.Message);
			return true;
		}
	}

	public DataTable GetLandedCostCodeFormMain()
	{
		try
		{
			string query = "select 0 as DocEntry,AlcName,AlcCode,OhType,0 as CostSum,0 as CostSumFC,0 as Factor,\r\n                                'F' as CostType,'N' as Locked,0 as CostSumSC, 'Y' as InCustCalc \r\n                                from oalc";
			new DataTable();
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Read data OCRD  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public string SaveDocCostGRPODetail(Document_LinesImpl doc)
	{
		string empty = string.Empty;
		try
		{
			string empty2 = string.Empty;
			empty2 = $"update PDN1 set LineStatus='C', OpenQty=OpenQty + -1.000000,\r\n                     OpenSum=OpenSum + -1.000000 * (select PriceBefDi from PDN1 where DocEntry={doc.DocEntry} \r\n                    and LineNum={doc.LineNum}) , TargetType='69', TrgetEntry={doc.TrgetEntry} where DocEntry={doc.DocEntry} and LineNum={doc.LineNum}";
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SaveDocCostGRPODetail  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public string SaveDocCostGRPO(Document_LinesImpl doc)
	{
		string empty = string.Empty;
		try
		{
			string empty2 = string.Empty;
			empty2 = string.Format("Declare @count int\r\n                                             select @count = count(*) from PDN1 where DocEntry=" + doc.DocEntry + " and LineStatus='O'if (@count=0) begin update OPDN set DocStatus='C' where DocEntry=" + doc.DocEntry + " end  ");
			if (ExecuteNonQueryNoParam(empty2) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return ex.Message;
		}
	}

	public DataTable GetNext(string docEntry)
	{
		DataTable result = new DataTable();
		try
		{
			string text = null;
			text = " WITH CTE AS (  select RN = ROW_NUMBER() OVER (ORDER BY DocEntry), * FROM oipf)\r\n                                SELECT  next.DocEntry as DocEntry\r\n                                FROM CTE \r\n                                LEFT JOIN CTE next ON next.RN = CTE.RN + 1\r\n                     WHERE CTE.[DocEntry] =" + docEntry;
			result = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
		}
		return result;
	}

	public DataTable GetPrevious(string docEntry)
	{
		DataTable result = new DataTable();
		try
		{
			string text = null;
			text = " WITH CTE AS (  select RN = ROW_NUMBER() OVER (ORDER BY DocEntry), * FROM oipf)\r\n                                SELECT  prev.DocEntry as DocEntry\r\n                                FROM CTE \r\n                                LEFT JOIN CTE prev ON prev.RN = CTE.RN - 1\r\n                     WHERE CTE.[DocEntry] =" + docEntry;
			result = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
		}
		return result;
	}

	public DataTable GetFirst()
	{
		DataTable result = new DataTable();
		try
		{
			string text = null;
			text = "SELECT top 1      [DocEntry]\r\n                                    FROM  oipf";
			result = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
		}
		return result;
	}

	public DataTable GetLast()
	{
		DataTable result = new DataTable();
		try
		{
			string text = null;
			text = "SELECT top 1      [DocEntry]\r\n                                    FROM  oipf ORDER BY DocEntry DESC ";
			result = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
		}
		return result;
	}
}
