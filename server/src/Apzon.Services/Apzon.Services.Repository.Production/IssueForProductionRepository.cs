using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Services.Interfaces.Production;

namespace Apzon.Services.Repository.Production;

public class IssueForProductionRepository : BaseService<BillOfMasterials_Impl_OITT>, IIssueForProduction
{
	public IssueForProductionRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	public DataTable Search(int docEntry)
	{
		DataTable result = new DataTable();
		try
		{
			string query = "select isnull([DocEntry],0) as [DocEntry]\r\n                      ,isnull([DocNum],0) as [DocNum]\r\n                      ,isnull([DocType],'') as [DocType]\r\n                      ,isnull([CANCELED],'') as [CANCELED]\r\n                      ,isnull([Handwrtten],'') as [Handwrtten]\r\n                      ,isnull([Printed],'') as [Printed]\r\n                      ,isnull([DocStatus],'') as [DocStatus]\r\n                      ,isnull([ObjType],'') as [ObjType]\r\n                      ,isnull([DocDate],'') as [DocDate]\r\n                      ,isnull([DocDueDate],'') as [DocDueDate]\r\n                      ,isnull([CardCode],'') as [CardCode]\r\n                      ,isnull([CardName],'') as [CardName]\r\n                      ,isnull([Address],'') as [Address]\r\n                      ,isnull([NumAtCard],'') as [NumAtCard]\r\n                      ,isnull([VatSum],0) as [VatSum]\r\n                      ,isnull([VatSumFC],0) as [VatSumFC]\r\n                      ,isnull([DiscPrcnt],0) as [DiscPrcnt]\r\n                      ,isnull([DiscSum],0) as [DiscSum]\r\n                      ,isnull([DiscSumFC],0) as [DiscSumFC]\r\n                      ,isnull([DocCur],'') as [DocCur]\r\n                      ,isnull([DocRate],0) as [DocRate]\r\n                      ,isnull([DocTotal],0) as [DocTotal]\r\n                      ,isnull([DocTotalFC],0) as [DocTotalFC]\r\n                      ,isnull([PaidToDate],0) as [PaidToDate]\r\n                      ,isnull([PaidFC],0) as [PaidFC]\r\n                      ,isnull([GrosProfit],0) as [GrosProfit]\r\n                      ,isnull([GrosProfFC],0) as [GrosProfFC]\r\n                      ,isnull([Ref1],'') as [Ref1]\r\n                      ,isnull([Ref2],'') as [Ref2]\r\n                      ,isnull([Comments],'') as [Comments]\r\n                      ,isnull([JrnlMemo],'') as [JrnlMemo]\r\n                      ,isnull([TransId],0) as [TransId]\r\n                      ,isnull([ReceiptNum],0) as [ReceiptNum]\r\n                      ,isnull([GroupNum],0) as [GroupNum]\r\n                      ,isnull([DocTime],'') as [DocTime]\r\n                      ,isnull([SlpCode],0) as [SlpCode]\r\n                      ,isnull([TrnspCode],0) as [TrnspCode]\r\n                      ,isnull([CntctCode],0) as [CntctCode]\r\n                      ,isnull([VatSumSy],0) as [VatSumSy]\r\n                      ,isnull([DiscSumSy],0) as [DiscSumSy]\r\n                      ,isnull([DocTotalSy],0) as [DocTotalSy]\r\n                      ,isnull([PaidSys],0) as [PaidSys]\r\n                      ,isnull([UpdateDate],'') as [UpdateDate]\r\n                      ,isnull([IsICT],'') as [IsICT]\r\n                      ,isnull([CreateDate],'') as [CreateDate]\r\n                      ,isnull([Series],0) as [Series]\r\n                      ,isnull([TaxDate],'') as [TaxDate]\r\n                      ,isnull([FinncPriod],0) as [FinncPriod]\r\n                      ,isnull([UserSign],0) as [UserSign]\r\n                      ,isnull([UserSign2],0) as [UserSign2]\r\n                      ,isnull([VatDate],'') as [VatDate]\r\n                      ,isnull([PaidSum],0) as[PaidSum]\r\n                      ,isnull([PaidSumFc],0) as [PaidSumFc]\r\n                      ,isnull([PaidSumSc],0) as [PaidSumSc]\r\n                      ,isnull([CurSource],'') as [CurSource]\r\n                      ,isnull([Cashremainder],0) as [Cashremainder]\r\n                      ,isnull([IsRIV],'') as [IsRIV]\r\n                      ,isnull([Requester],'') as [Requester]\r\n                      ,isnull([ReqName],'') as [ReqName]\r\n                      ,isnull([Branch],0) as [Branch]\r\n                      ,isnull([Department],0) as[Department]\r\n                      ,isnull([ReqDate],'') as [ReqDate]\r\n                        ,isnull([AtcEntry],'') as [AtcEntry]\r\n                  FROM [dbo].[OIGE] where DocEntry =" + docEntry;
			result = ExecuteData(query, CommandType.Text, (SqlParameter[])null).Tables[0];
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get data Search - issue for production ", ex.Message);
		}
		return result;
	}

	public int GetMaxCode()
	{
		try
		{
			string query = "SELECT ISNULL( MAX(DocEntry),0) as maxCode\r\n                                    FROM OIGE";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["maxCode"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return 0;
		}
	}

	public int GetMaxFileEntry()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable(GetMaxFileEntryQuery(), CommandType.Text, (SqlParameter[])null);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Convert.ToInt32(dataTable.Rows[0]["maxCode"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC  FAILED ", ex.Message);
			return 0;
		}
	}

	protected virtual string GetMaxFileEntryQuery()
	{
		return "SELECT ISNULL( MAX(absentry),0) as maxCode\r\n                                    FROM oatc";
	}

	public DataTable LoadItemsByParentCode(int docEntry)
	{
		try
		{
			string query = "SELECT isnull(BaseRef,'') as BaseRef\r\n\t\t\t,isnull(b.series,0) as Series,\r\n\t\t\tisnull(LineNum,0) as LineNum\r\n\t\t\t, isnull(c.ItemType,'') as ItemType\r\n\t\t\t, isnull(a.ItemCode,'') as ItemCode\r\n\t\t\t,Dscription,isnull(Quantity,0) as Quantity\r\n\t\t\t,isnull(a.WhsCode,'') as WhsCode \r\n            ,isnull(d.AvgPrice,0) as ItemCost\r\n\t\t\t,isnull(b.PlannedQty,0) as PlannedQty\r\n\t\t\t,isnull(b.CmpltQty,0) as CmpltQty\r\n\t\t\t,isnull(d.OnHand,0) - isnull(d.IsCommited,0) + isnull(d.OnOrder,0) as Avail\r\n\t\t\t,isnull(c.InvntryUom,'') as UomCode \r\n\t\t\t,isnull(c.SalUnitMsr,'') as unitMsr\r\n\t\t\tfrom ige1 a left join owor b on a.BaseRef = b.DocEntry\r\n\t\t\tleft join oitm c on a.ItemCode= c.ItemCode \r\n\t\t\tleft join oitw d on (d.itemcode=a.itemcode and d.WhsCode=a.WhsCode)\r\n                    where a.docentry=" + docEntry;
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
			return null;
		}
	}

	public DataTable LoadItemsByProdOrderEntry(int docEntry)
	{
		try
		{
			string query = "SELECT  isnull(a.docentry,0) as BaseRef\r\n                                ,isnull(a.series,0) as Series\r\n                                ,0 as LineNum\r\n                                , isnull(b.ItemType,'') as ItemType\r\n                                , isnull(a.ItemCode,'') as ItemCode\r\n                                ,b.ItemName as Dscription\r\n                                ,0 as Quantity\r\n                                ,isnull(a.Warehouse,'') as WhsCode \r\n                                ,isnull(c.AvgPrice,0) as ItemCost\r\n                                ,isnull(a.PlannedQty,0) as PlannedQty\r\n                                ,isnull(a.CmpltQty,0) as CmpltQty\r\n                                ,isnull(c.OnHand,0) - isnull(c.IsCommited,0) + isnull(c.OnOrder,0) as Avail\r\n                                ,isnull(b.InvntryUom,'') as UomCode \r\n                                ,isnull(b.SalUnitMsr,'') as unitMsr\r\n\r\n                                from owor a left join oitm b on a.ItemCode= b.ItemCode\r\n                                left join oitw c on (c.itemcode=a.itemcode and c.WhsCode=a.Warehouse)\r\n                            where a.docentry=" + docEntry;
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
			return null;
		}
	}

	public DataTable LoadAttachFiles(int absEntry)
	{
		try
		{
			return ExecuteDataTable(GetLoadAttachFilesQuery(absEntry), CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
		}
	}

	protected virtual string GetLoadAttachFilesQuery(int absEntry)
	{
		return "SELECT * FROM [dbo].[ATC1]where AbsEntry=" + absEntry;
	}

	public DataTable GetFirstRecord()
	{
		try
		{
			string text = null;
			text = "SELECT top 1 isnull([DocEntry],0) as [DocEntry]\r\n                      ,isnull([DocNum],0) as [DocNum]\r\n                      ,isnull([DocType],'') as [DocType]\r\n                      ,isnull([CANCELED],'') as [CANCELED]\r\n                      ,isnull([Handwrtten],'') as [Handwrtten]\r\n                      ,isnull([Printed],'') as [Printed]\r\n                      ,isnull([DocStatus],'') as [DocStatus]\r\n                      ,isnull([ObjType],'') as [ObjType]\r\n                      ,isnull([DocDate],'') as [DocDate]\r\n                      ,isnull([DocDueDate],'') as [DocDueDate]\r\n                      ,isnull([CardCode],'') as [CardCode]\r\n                      ,isnull([CardName],'') as [CardName]\r\n                      ,isnull([Address],'') as [Address]\r\n                      ,isnull([NumAtCard],'') as [NumAtCard]\r\n                      ,isnull([VatSum],0) as [VatSum]\r\n                      ,isnull([VatSumFC],0) as [VatSumFC]\r\n                      ,isnull([DiscPrcnt],0) as [DiscPrcnt]\r\n                      ,isnull([DiscSum],0) as [DiscSum]\r\n                      ,isnull([DiscSumFC],0) as [DiscSumFC]\r\n                      ,isnull([DocCur],'') as [DocCur]\r\n                      ,isnull([DocRate],0) as [DocRate]\r\n                      ,isnull([DocTotal],0) as [DocTotal]\r\n                      ,isnull([DocTotalFC],0) as [DocTotalFC]\r\n                      ,isnull([PaidToDate],0) as [PaidToDate]\r\n                      ,isnull([PaidFC],0) as [PaidFC]\r\n                      ,isnull([GrosProfit],0) as [GrosProfit]\r\n                      ,isnull([GrosProfFC],0) as [GrosProfFC]\r\n                      ,isnull([Ref1],'') as [Ref1]\r\n                      ,isnull([Ref2],'') as [Ref2]\r\n                      ,isnull([Comments],'') as [Comments]\r\n                      ,isnull([JrnlMemo],'') as [JrnlMemo]\r\n                      ,isnull([TransId],0) as [TransId]\r\n                      ,isnull([ReceiptNum],0) as [ReceiptNum]\r\n                      ,isnull([GroupNum],0) as [GroupNum]\r\n                      ,isnull([DocTime],'') as [DocTime]\r\n                      ,isnull([SlpCode],0) as [SlpCode]\r\n                      ,isnull([TrnspCode],0) as [TrnspCode]\r\n                      ,isnull([CntctCode],0) as [CntctCode]\r\n                      ,isnull([VatSumSy],0) as [VatSumSy]\r\n                      ,isnull([DiscSumSy],0) as [DiscSumSy]\r\n                      ,isnull([DocTotalSy],0) as [DocTotalSy]\r\n                      ,isnull([PaidSys],0) as [PaidSys]\r\n                      ,isnull([UpdateDate],'') as [UpdateDate]\r\n                      ,isnull([IsICT],'') as [IsICT]\r\n                      ,isnull([CreateDate],'') as [CreateDate]\r\n                      ,isnull([Series],0) as [Series]\r\n                      ,isnull([TaxDate],'') as [TaxDate]\r\n                      ,isnull([FinncPriod],0) as [FinncPriod]\r\n                      ,isnull([UserSign],0) as [UserSign]\r\n                      ,isnull([UserSign2],0) as [UserSign2]\r\n                      ,isnull([VatDate],'') as [VatDate]\r\n                      ,isnull([PaidSum],0) as[PaidSum]\r\n                      ,isnull([PaidSumFc],0) as [PaidSumFc]\r\n                      ,isnull([PaidSumSc],0) as [PaidSumSc]\r\n                      ,isnull([CurSource],'') as [CurSource]\r\n                      ,isnull([Cashremainder],0) as [Cashremainder]\r\n                      ,isnull([IsRIV],'') as [IsRIV]\r\n                      ,isnull([Requester],'') as [Requester]\r\n                      ,isnull([ReqName],'') as [ReqName]\r\n                      ,isnull([Branch],0) as [Branch]\r\n                      ,isnull([Department],0) as[Department]\r\n                      ,isnull([ReqDate],'') as [ReqDate]\r\n                        ,isnull([AtcEntry],0) as [AtcEntry]\r\n                  FROM [dbo].[OIGE]";
			return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch
		{
			return null;
		}
	}

	public DataTable GetLastRecord()
	{
		try
		{
			string text = null;
			text = "SELECT top 1 isnull([DocEntry],0) as [DocEntry]\r\n                      ,isnull([DocNum],0) as [DocNum]\r\n                      ,isnull([DocType],'') as [DocType]\r\n                      ,isnull([CANCELED],'') as [CANCELED]\r\n                      ,isnull([Handwrtten],'') as [Handwrtten]\r\n                      ,isnull([Printed],'') as [Printed]\r\n                      ,isnull([DocStatus],'') as [DocStatus]\r\n                      ,isnull([ObjType],'') as [ObjType]\r\n                      ,isnull([DocDate],'') as [DocDate]\r\n                      ,isnull([DocDueDate],'') as [DocDueDate]\r\n                      ,isnull([CardCode],'') as [CardCode]\r\n                      ,isnull([CardName],'') as [CardName]\r\n                      ,isnull([Address],'') as [Address]\r\n                      ,isnull([NumAtCard],'') as [NumAtCard]\r\n                      ,isnull([VatSum],0) as [VatSum]\r\n                      ,isnull([VatSumFC],0) as [VatSumFC]\r\n                      ,isnull([DiscPrcnt],0) as [DiscPrcnt]\r\n                      ,isnull([DiscSum],0) as [DiscSum]\r\n                      ,isnull([DiscSumFC],0) as [DiscSumFC]\r\n                      ,isnull([DocCur],'') as [DocCur]\r\n                      ,isnull([DocRate],0) as [DocRate]\r\n                      ,isnull([DocTotal],0) as [DocTotal]\r\n                      ,isnull([DocTotalFC],0) as [DocTotalFC]\r\n                      ,isnull([PaidToDate],0) as [PaidToDate]\r\n                      ,isnull([PaidFC],0) as [PaidFC]\r\n                      ,isnull([GrosProfit],0) as [GrosProfit]\r\n                      ,isnull([GrosProfFC],0) as [GrosProfFC]\r\n                      ,isnull([Ref1],'') as [Ref1]\r\n                      ,isnull([Ref2],'') as [Ref2]\r\n                      ,isnull([Comments],'') as [Comments]\r\n                      ,isnull([JrnlMemo],'') as [JrnlMemo]\r\n                      ,isnull([TransId],0) as [TransId]\r\n                      ,isnull([ReceiptNum],0) as [ReceiptNum]\r\n                      ,isnull([GroupNum],0) as [GroupNum]\r\n                      ,isnull([DocTime],'') as [DocTime]\r\n                      ,isnull([SlpCode],0) as [SlpCode]\r\n                      ,isnull([TrnspCode],0) as [TrnspCode]\r\n                      ,isnull([CntctCode],0) as [CntctCode]\r\n                      ,isnull([VatSumSy],0) as [VatSumSy]\r\n                      ,isnull([DiscSumSy],0) as [DiscSumSy]\r\n                      ,isnull([DocTotalSy],0) as [DocTotalSy]\r\n                      ,isnull([PaidSys],0) as [PaidSys]\r\n                      ,isnull([UpdateDate],'') as [UpdateDate]\r\n                      ,isnull([IsICT],'') as [IsICT]\r\n                      ,isnull([CreateDate],'') as [CreateDate]\r\n                      ,isnull([Series],0) as [Series]\r\n                      ,isnull([TaxDate],'') as [TaxDate]\r\n                      ,isnull([FinncPriod],0) as [FinncPriod]\r\n                      ,isnull([UserSign],0) as [UserSign]\r\n                      ,isnull([UserSign2],0) as [UserSign2]\r\n                      ,isnull([VatDate],'') as [VatDate]\r\n                      ,isnull([PaidSum],0) as[PaidSum]\r\n                      ,isnull([PaidSumFc],0) as [PaidSumFc]\r\n                      ,isnull([PaidSumSc],0) as [PaidSumSc]\r\n                      ,isnull([CurSource],'') as [CurSource]\r\n                      ,isnull([Cashremainder],0) as [Cashremainder]\r\n                      ,isnull([IsRIV],'') as [IsRIV]\r\n                      ,isnull([Requester],'') as [Requester]\r\n                      ,isnull([ReqName],'') as [ReqName]\r\n                      ,isnull([Branch],0) as [Branch]\r\n                      ,isnull([Department],0) as[Department]\r\n                      ,isnull([ReqDate],'') as [ReqDate]\r\n                        ,isnull([AtcEntry],0) as [AtcEntry]\r\n                  FROM [dbo].[OIGE] where docentry=(select max(docentry) from oige)";
			return ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch
		{
			return null;
		}
	}

	public DataTable GetNextRecord(string docEntry)
	{
		DataTable result = new DataTable();
		try
		{
			string text = null;
			text = "WITH CTE AS (  select RN = ROW_NUMBER() OVER (ORDER BY DocEntry), * FROM oige)\r\n                                SELECT \r\n                                        isnull(next.DocEntry,0) as DocEntry\r\n                      ,isnull(next.DocNum,0) as DocNum\r\n                      ,isnull(next.DocType,'') as DocType\r\n                      ,isnull(next.CANCELED,'') as CANCELED\r\n                      ,isnull(next.Handwrtten,'') as Handwrtten\r\n                      ,isnull(next.Printed,'') as Printed\r\n                      ,isnull(next.DocStatus,'') as DocStatus\r\n                      ,isnull(next.ObjType,'') as ObjType\r\n                      ,isnull(next.DocDate,'') as DocDate\r\n                      ,isnull(next.DocDueDate,'') as DocDueDate\r\n                      ,isnull(next.CardCode,'') as CardCode\r\n                      ,isnull(next.CardName,'') as CardName\r\n                      ,isnull(next.Address,'') as Address\r\n                      ,isnull(next.NumAtCard,'') as NumAtCard\r\n                      ,isnull(next.VatSum,0) as VatSum\r\n                      ,isnull(next.VatSumFC,0) as VatSumFC\r\n                      ,isnull(next.DiscPrcnt,0) as DiscPrcnt\r\n                      ,isnull(next.DiscSum,0) as DiscSum\r\n                      ,isnull(next.DiscSumFC,0) as DiscSumFC\r\n                      ,isnull(next.DocCur,'') as DocCur\r\n                      ,isnull(next.DocRate,0) as DocRate\r\n                      ,isnull(next.DocTotal,0) as DocTotal\r\n                      ,isnull(next.DocTotalFC,0) as DocTotalFC\r\n                      ,isnull(next.PaidToDate,0) as PaidToDate\r\n                      ,isnull(next.PaidFC,0) as PaidFC\r\n                      ,isnull(next.GrosProfit,0) as GrosProfit\r\n                      ,isnull(next.GrosProfFC,0) as GrosProfFC\r\n                      ,isnull(next.Ref1,'') as Ref1\r\n                      ,isnull(next.Ref2,'') as Ref2\r\n                      ,isnull(next.Comments,'') as Comments\r\n                      ,isnull(next.JrnlMemo,'') as JrnlMemo\r\n                      ,isnull(next.TransId,0) as TransId\r\n                      ,isnull(next.ReceiptNum,0) as ReceiptNum\r\n                      ,isnull(next.GroupNum,0) as GroupNum\r\n                      ,isnull(next.DocTime,'') as DocTime\r\n                      ,isnull(next.SlpCode,0) as SlpCode\r\n                      ,isnull(next.TrnspCode,0) as TrnspCode\r\n                      ,isnull(next.CntctCode,0) as CntctCode\r\n                      ,isnull(next.VatSumSy,0) as VatSumSy\r\n                      ,isnull(next.DiscSumSy,0) as DiscSumSy\r\n                      ,isnull(next.DocTotalSy,0) as DocTotalSy\r\n                      ,isnull(next.PaidSys,0) as PaidSys\r\n                      ,isnull(next.UpdateDate,'') as UpdateDate\r\n                      ,isnull(next.IsICT,'') as IsICT\r\n                      ,isnull(next.CreateDate,'') as CreateDate\r\n                      ,isnull(next.Series,0) as Series\r\n                      ,isnull(next.TaxDate,'') as TaxDate\r\n                      ,isnull(next.FinncPriod,0) as FinncPriod\r\n                      ,isnull(next.UserSign,0) as UserSign\r\n                      ,isnull(next.UserSign2,0) as UserSign2\r\n                      ,isnull(next.VatDate,'') as VatDate\r\n                      ,isnull(next.PaidSum,0) asPaidSum\r\n                      ,isnull(next.PaidSumFc,0) as PaidSumFc\r\n                      ,isnull(next.PaidSumSc,0) as PaidSumSc\r\n                      ,isnull(next.CurSource,'') as CurSource\r\n                      ,isnull(next.Cashremainder,0) as Cashremainder\r\n                      ,isnull(next.IsRIV,'') as IsRIV\r\n                      ,isnull(next.Requester,'') as Requester\r\n                      ,isnull(next.ReqName,'') as ReqName\r\n                      ,isnull(next.Branch,0) as Branch\r\n                      ,isnull(next.Department,0) asDepartment\r\n                      ,isnull(next.ReqDate,'') as ReqDate  \r\n                        ,isnull(next.AtcEntry,0) as AtcEntry\r\n                                FROM CTE \r\n                                LEFT JOIN CTE next ON next.RN = CTE.RN + 1\r\n                     WHERE CTE.DocEntry =" + docEntry;
			result = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
		}
		return result;
	}

	public DataTable GetPreviousRecord(string docEntry)
	{
		DataTable result = new DataTable();
		try
		{
			string text = null;
			text = "WITH CTE AS (  select RN = ROW_NUMBER() OVER (ORDER BY DocEntry), * FROM oige)\r\n                                SELECT \r\n                                        isnull(prev.DocEntry,0) as DocEntry\r\n                      ,isnull(prev.DocNum,0) as DocNum\r\n                      ,isnull(prev.DocType,'') as DocType\r\n                      ,isnull(prev.CANCELED,'') as CANCELED\r\n                      ,isnull(prev.Handwrtten,'') as Handwrtten\r\n                      ,isnull(prev.Printed,'') as Printed\r\n                      ,isnull(prev.DocStatus,'') as DocStatus\r\n                      ,isnull(prev.ObjType,'') as ObjType\r\n                      ,isnull(prev.DocDate,'') as DocDate\r\n                      ,isnull(prev.DocDueDate,'') as DocDueDate\r\n                      ,isnull(prev.CardCode,'') as CardCode\r\n                      ,isnull(prev.CardName,'') as CardName\r\n                      ,isnull(prev.Address,'') as Address\r\n                      ,isnull(prev.NumAtCard,'') as NumAtCard\r\n                      ,isnull(prev.VatSum,0) as VatSum\r\n                      ,isnull(prev.VatSumFC,0) as VatSumFC\r\n                      ,isnull(prev.DiscPrcnt,0) as DiscPrcnt\r\n                      ,isnull(prev.DiscSum,0) as DiscSum\r\n                      ,isnull(prev.DiscSumFC,0) as DiscSumFC\r\n                      ,isnull(prev.DocCur,'') as DocCur\r\n                      ,isnull(prev.DocRate,0) as DocRate\r\n                      ,isnull(prev.DocTotal,0) as DocTotal\r\n                      ,isnull(prev.DocTotalFC,0) as DocTotalFC\r\n                      ,isnull(prev.PaidToDate,0) as PaidToDate\r\n                      ,isnull(prev.PaidFC,0) as PaidFC\r\n                      ,isnull(prev.GrosProfit,0) as GrosProfit\r\n                      ,isnull(prev.GrosProfFC,0) as GrosProfFC\r\n                      ,isnull(prev.Ref1,'') as Ref1\r\n                      ,isnull(prev.Ref2,'') as Ref2\r\n                      ,isnull(prev.Comments,'') as Comments\r\n                      ,isnull(prev.JrnlMemo,'') as JrnlMemo\r\n                      ,isnull(prev.TransId,0) as TransId\r\n                      ,isnull(prev.ReceiptNum,0) as ReceiptNum\r\n                      ,isnull(prev.GroupNum,0) as GroupNum\r\n                      ,isnull(prev.DocTime,'') as DocTime\r\n                      ,isnull(prev.SlpCode,0) as SlpCode\r\n                      ,isnull(prev.TrnspCode,0) as TrnspCode\r\n                      ,isnull(prev.CntctCode,0) as CntctCode\r\n                      ,isnull(prev.VatSumSy,0) as VatSumSy\r\n                      ,isnull(prev.DiscSumSy,0) as DiscSumSy\r\n                      ,isnull(prev.DocTotalSy,0) as DocTotalSy\r\n                      ,isnull(prev.PaidSys,0) as PaidSys\r\n                      ,isnull(prev.UpdateDate,'') as UpdateDate\r\n                      ,isnull(prev.IsICT,'') as IsICT\r\n                      ,isnull(prev.CreateDate,'') as CreateDate\r\n                      ,isnull(prev.Series,0) as Series\r\n                      ,isnull(prev.TaxDate,'') as TaxDate\r\n                      ,isnull(prev.FinncPriod,0) as FinncPriod\r\n                      ,isnull(prev.UserSign,0) as UserSign\r\n                      ,isnull(prev.UserSign2,0) as UserSign2\r\n                      ,isnull(prev.VatDate,'') as VatDate\r\n                      ,isnull(prev.PaidSum,0) asPaidSum\r\n                      ,isnull(prev.PaidSumFc,0) as PaidSumFc\r\n                      ,isnull(prev.PaidSumSc,0) as PaidSumSc\r\n                      ,isnull(prev.CurSource,'') as CurSource\r\n                      ,isnull(prev.Cashremainder,0) as Cashremainder\r\n                      ,isnull(prev.IsRIV,'') as IsRIV\r\n                      ,isnull(prev.Requester,'') as Requester\r\n                      ,isnull(prev.ReqName,'') as ReqName\r\n                      ,isnull(prev.Branch,0) as Branch\r\n                      ,isnull(prev.Department,0) asDepartment\r\n                      ,isnull(prev.ReqDate,'') as ReqDate  \r\n                        ,isnull(prev.AtcEntry,0) as AtcEntry\r\n                                FROM CTE \r\n                                LEFT JOIN CTE prev ON prev.RN = CTE.RN - 1\r\n                     WHERE CTE.DocEntry =" + docEntry;
			result = ExecuteDataTable(text, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetNext  ", ex.Message);
		}
		return result;
	}

	private bool CheckDoc(int docEntry)
	{
		try
		{
			string query = "SELECT docEntry\r\n                            FROM oige \r\n                            WHERE docEntry = " + docEntry;
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
			string query = "SELECT docEntry FROM ige1  WHERE\r\n                        docEntry = " + docEntry + " and  lineNum = " + lineNum;
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

	public string CreateReceipt(ReceiptProduction_Impl_OIGN doc)
	{
		string result = "";
		try
		{
			string query = "Apz_IssueForProduction_CreateReceipt";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@DocEntry", Function.ParseInt(doc.DocEntry)));
			list.Add(new SqlParameter("@DocNum", Function.ParseInt(doc.DocNum)));
			list.Add(new SqlParameter("@DocType", Function.ToString(doc.DocType)));
			list.Add(new SqlParameter("@CANCELED", Function.ToString(doc.CANCELED)));
			list.Add(new SqlParameter("@Handwrtten", Function.ToString(doc.Handwrtten)));
			list.Add(new SqlParameter("@Printed", Function.ToString(doc.Printed)));
			list.Add(new SqlParameter("@DocStatus", Function.ToString(doc.DocStatus)));
			list.Add(new SqlParameter("@ObjType", Function.ToString(doc.ObjType)));
			list.Add(new SqlParameter("@DocDate", Function.ParseDateTime(doc.DocDate)));
			list.Add(new SqlParameter("@DocDueDate", Function.ParseDateTime(doc.DocDueDate)));
			list.Add(new SqlParameter("@CardCode", Function.ToString(doc.CardCode)));
			list.Add(new SqlParameter("@CardName", Function.ToString(doc.CardName)));
			list.Add(new SqlParameter("@Address", Function.ToString(doc.Address)));
			list.Add(new SqlParameter("@NumAtCard", Function.ToString(doc.NumAtCard)));
			list.Add(new SqlParameter("@VatSum", Function.ParseDecimal(doc.VatSum)));
			list.Add(new SqlParameter("@VatSumFC", Function.ParseDecimal(doc.VatSumFC)));
			list.Add(new SqlParameter("@DiscPrcnt", Function.ParseDecimal(doc.DiscPrcnt)));
			list.Add(new SqlParameter("@DiscSum", Function.ParseDecimal(doc.DiscSum)));
			list.Add(new SqlParameter("@DiscSumFC", Function.ParseDecimal(doc.DiscSumFC)));
			list.Add(new SqlParameter("@DocCur", Function.ToString(doc.DocCur)));
			list.Add(new SqlParameter("@DocRate", Function.ParseDecimal(doc.DocRate)));
			list.Add(new SqlParameter("@DocTotal", Function.ParseDecimal(doc.DocTotal)));
			list.Add(new SqlParameter("@DocTotalFC", Function.ParseDecimal(doc.DocTotalFC)));
			list.Add(new SqlParameter("@PaidToDate", Function.ParseDecimal(doc.PaidToDate)));
			list.Add(new SqlParameter("@PaidFC", Function.ParseDecimal(doc.PaidFC)));
			list.Add(new SqlParameter("@GrosProfit", Function.ParseDecimal(doc.GrosProfit)));
			list.Add(new SqlParameter("@GrosProfFC", Function.ParseDecimal(doc.GrosProfFC)));
			list.Add(new SqlParameter("@Ref1", Function.ToString(doc.Ref1)));
			list.Add(new SqlParameter("@Ref2", Function.ToString(doc.Ref2)));
			list.Add(new SqlParameter("@Comments", Function.ToString(doc.Comments)));
			list.Add(new SqlParameter("@JrnlMemo", Function.ToString(doc.JrnlMemo)));
			list.Add(new SqlParameter("@TransId", Function.ParseInt(doc.TransId)));
			list.Add(new SqlParameter("@ReceiptNum", Function.ParseInt(doc.ReceiptNum)));
			list.Add(new SqlParameter("@GroupNum", Function.ParseInt(doc.GroupNum)));
			list.Add(new SqlParameter("@DocTime", TimeSpan.Zero));
			list.Add(new SqlParameter("@SlpCode", Function.ParseInt(doc.SlpCode)));
			list.Add(new SqlParameter("@TrnspCode", Function.ParseInt(doc.TrnspCode)));
			list.Add(new SqlParameter("@CntctCode", Function.ParseInt(doc.CntctCode)));
			list.Add(new SqlParameter("@VatSumSy", Function.ParseDecimal(doc.VatSumSy)));
			list.Add(new SqlParameter("@DiscSumSy", Function.ParseDecimal(doc.DiscSumSy)));
			list.Add(new SqlParameter("@DocTotalSy", Function.ParseDecimal(doc.DocTotalSy)));
			list.Add(new SqlParameter("@PaidSys", Function.ParseDecimal(doc.PaidSys)));
			list.Add(new SqlParameter("@UpdateDate", Function.ParseDateTime(doc.UpdateDate)));
			list.Add(new SqlParameter("@IsICT", Function.ToString(doc.IsICT)));
			list.Add(new SqlParameter("@CreateDate", Function.ParseDateTime(doc.CreateDate)));
			list.Add(new SqlParameter("@Series", Function.ParseInt(doc.Series)));
			list.Add(new SqlParameter("@TaxDate", Function.ParseDateTime(doc.TaxDate)));
			list.Add(new SqlParameter("@FinncPriod", Function.ParseInt(doc.FinncPriod)));
			list.Add(new SqlParameter("@UserSign", Function.ParseInt(doc.UserSign)));
			list.Add(new SqlParameter("@UserSign2", Function.ParseInt(doc.UserSign2)));
			list.Add(new SqlParameter("@VatDate", Function.ParseDateTime(doc.VatDate)));
			list.Add(new SqlParameter("@PaidSum", Function.ParseDecimal(doc.PaidSum)));
			list.Add(new SqlParameter("@PaidSumFc", Function.ParseDecimal(doc.PaidSumFc)));
			list.Add(new SqlParameter("@PaidSumSc", Function.ParseDecimal(doc.PaidSumSc)));
			list.Add(new SqlParameter("@CurSource", Function.ToString(doc.CurSource)));
			list.Add(new SqlParameter("@Cashremainder", Function.ParseDecimal(doc.Cashremainder)));
			list.Add(new SqlParameter("@IsRIV", Function.ToString(doc.IsRIV)));
			list.Add(new SqlParameter("@Requester", Function.ToString(doc.Requester)));
			list.Add(new SqlParameter("@ReqName", Function.ToString(doc.ReqName)));
			list.Add(new SqlParameter("@Branch", Function.ParseInt(doc.Branch)));
			list.Add(new SqlParameter("@Department", Function.ParseInt(doc.Department)));
			list.Add(new SqlParameter("@ReqDate", Function.ParseDateTime(doc.ReqDate)));
			list.Add(new SqlParameter("@AtcEntry", Function.ParseInt(doc.AtcEntry)));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				foreach (ReceiptProduction_LineImpl_IGN1 item in doc.lineIGN1)
				{
					if (string.IsNullOrEmpty(CreateReceiptLine(item)))
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

	public string CreateReceiptLine(ReceiptProduction_LineImpl_IGN1 line)
	{
		string text = "";
		try
		{
			string query = "Apz_IssueForProduction_CreateReceiptLine";
			List<SqlParameter> list = new List<SqlParameter>();
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

	public string SaveLine(IssueForProductionLine_Impl_IGE1 line)
	{
		string empty = string.Empty;
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			bool flag = CheckLine(line.DocEntry, line.LineNum);
			string empty2 = string.Empty;
			if (!flag)
			{
				empty2 = "INSERT INTO [dbo].[IGE1]\r\n                               ([DocEntry] ,[LineNum] ,[TargetType] ,[TrgetEntry]\r\n                               ,[BaseRef] ,[BaseType] ,[BaseEntry] ,[BaseLine] ,[LineStatus]\r\n                               ,[ItemCode] ,[Dscription] ,[Quantity] ,[OpenQty] ,[Price]\r\n                               ,[Currency] ,[Rate] ,[DiscPrcnt] ,[LineTotal] ,[TotalFrgn]\r\n                               ,[OpenSum] ,[OpenSumFC] ,[WhsCode] ,[SlpCode] ,[AcctCode]\r\n                               ,[PriceBefDi] ,[DocDate] ,[OcrCode] ,[Project] ,[CodeBars] ,[VatPrcnt]\r\n                               ,[VatGroup] ,[PriceAfVAT] ,[BaseDocNum] ,[VatSum] ,[VatSumFrgn]\r\n                               ,[VatSumSy]  ,[FinncPriod] ,[ObjType] ,[GrssProfit] ,[GrssProfFC]\r\n                               ,[TaxCode] ,[BaseQty] ,[BaseOpnQty] ,[VatDscntPr] ,[GTotal]\r\n                               ,[GTotalFC] ,[OcrCode2] ,[OcrCode3] ,[OcrCode4] ,[OcrCode5]\r\n                               ,[CogsOcrCo2] ,[CogsOcrCo3] ,[CogsOcrCo4] ,[CogsOcrCo5]\r\n                               ,[Factor1] ,[Factor2] ,[Factor3] ,[Factor4] ,[UseBaseUn]\r\n                               ,[unitMsr] ,[NumPerMsr] ,[ShipDate] ,[FreeTxt] ,[UomCode] ,[PQTReqdate])\r\n                         VALUES ( ";
				empty2 += "@DocEntry ";
				list.Add(new SqlParameter("@DocEntry", Function.ParseInt(line.DocEntry)));
				empty2 += ",@LineNum ";
				list.Add(new SqlParameter("@LineNum", Function.ParseInt(line.LineNum)));
				empty2 += ", @TargetType ";
				list.Add(new SqlParameter("@TargetType", Function.ParseInt(line.TargetType)));
				empty2 += ",@TrgetEntry ";
				list.Add(new SqlParameter("@TrgetEntry", Function.ParseInt(line.TrgetEntry)));
				empty2 += ",@BaseRef ";
				list.Add(new SqlParameter("@BaseRef", Function.ToString(line.BaseRef)));
				empty2 += ", @BaseType ";
				list.Add(new SqlParameter("@BaseType", Function.ParseInt(line.BaseType)));
				empty2 += ",@BaseEntry ";
				list.Add(new SqlParameter("@BaseEntry", Function.ParseInt(line.BaseEntry)));
				empty2 += ",@BaseLine ";
				list.Add(new SqlParameter("@BaseLine", Function.ParseInt(line.BaseLine)));
				empty2 += ", @LineStatus ";
				list.Add(new SqlParameter("@LineStatus", Function.ToString(line.LineStatus)));
				empty2 += ",@ItemCode ";
				list.Add(new SqlParameter("@ItemCode", Function.ToString(line.ItemCode)));
				empty2 += ", @Dscription ";
				list.Add(new SqlParameter("@Dscription", Function.ToString(line.Dscription)));
				empty2 += ",@Quantity ";
				list.Add(new SqlParameter("@Quantity", Function.ParseDecimal(line.Quantity)));
				empty2 += ",@OpenQty ";
				list.Add(new SqlParameter("@OpenQty", Function.ParseDecimal(line.OpenQty)));
				empty2 += ", @Price ";
				list.Add(new SqlParameter("@Price", Function.ParseDecimal(line.Price)));
				empty2 += ",@Currency ";
				list.Add(new SqlParameter("@Currency", Function.ToString(line.Currency)));
				empty2 += ",@Rate ";
				list.Add(new SqlParameter("@Rate", Function.ParseDecimal(line.Rate)));
				empty2 += ", @DiscPrcnt ";
				list.Add(new SqlParameter("@DiscPrcnt", Function.ParseDecimal(line.DiscPrcnt)));
				empty2 += ",@LineTotal ";
				list.Add(new SqlParameter("@LineTotal", Function.ParseDecimal(line.LineTotal)));
				empty2 += ",@TotalFrgn ";
				list.Add(new SqlParameter("@TotalFrgn", Function.ParseDecimal(line.TotalFrgn)));
				empty2 += ", @OpenSum ";
				list.Add(new SqlParameter("@OpenSum", Function.ParseDecimal(line.OpenSum)));
				empty2 += ",@OpenSumFC ";
				list.Add(new SqlParameter("@OpenSumFC", Function.ParseDecimal(line.OpenSumFC)));
				empty2 += ", @WhsCode ";
				list.Add(new SqlParameter("@WhsCode", Function.ToString(line.WhsCode)));
				empty2 += ",@SlpCode ";
				list.Add(new SqlParameter("@SlpCode", Function.ParseInt(line.SlpCode)));
				empty2 += ",@AcctCode ";
				list.Add(new SqlParameter("@AcctCode", Function.ToString(line.AcctCode)));
				empty2 += ", @PriceBefDi ";
				list.Add(new SqlParameter("@PriceBefDi", Function.ParseDecimal(line.PriceBefDi)));
				empty2 += ",@DocDate ";
				list.Add(new SqlParameter("@DocDate", Function.ParseDateTime(line.DocDate)));
				empty2 += ",@OcrCode ";
				list.Add(new SqlParameter("@OcrCode", Function.ToString(line.OcrCode)));
				empty2 += ", @Project ";
				list.Add(new SqlParameter("@Project", Function.ToString(line.Project)));
				empty2 += ",@CodeBars ";
				list.Add(new SqlParameter("@CodeBars", Function.ToString(line.CodeBars)));
				empty2 += ",@VatPrcnt ";
				list.Add(new SqlParameter("@VatPrcnt", Function.ParseDecimal(line.VatPrcnt)));
				empty2 += ", @VatGroup ";
				list.Add(new SqlParameter("@VatGroup", Function.ToString(line.VatGroup)));
				empty2 += ",@PriceAfVAT ";
				list.Add(new SqlParameter("@PriceAfVAT", Function.ParseDecimal(line.PriceAfVAT)));
				empty2 += ", @BaseDocNum ";
				list.Add(new SqlParameter("@BaseDocNum", Function.ParseInt(line.BaseDocNum)));
				empty2 += ",@VatSum ";
				list.Add(new SqlParameter("@VatSum", Function.ParseDecimal(line.VatSum)));
				empty2 += ",@VatSumFrgn ";
				list.Add(new SqlParameter("@VatSumFrgn", Function.ParseDecimal(line.VatSumFrgn)));
				empty2 += ", @VatSumSy ";
				list.Add(new SqlParameter("@VatSumSy", Function.ParseDecimal(line.VatSumSy)));
				empty2 += ",@FinncPriod ";
				list.Add(new SqlParameter("@FinncPriod", Function.ParseInt(line.FinncPriod)));
				empty2 += ", @ObjType ";
				list.Add(new SqlParameter("@ObjType", Function.ToString(line.ObjType)));
				empty2 += ",@GrssProfit ";
				list.Add(new SqlParameter("@GrssProfit", Function.ParseDecimal(line.GrssProfit)));
				empty2 += ",@GrssProfFC ";
				list.Add(new SqlParameter("@GrssProfFC", Function.ParseDecimal(line.GrssProfFC)));
				empty2 += ", @TaxCode ";
				list.Add(new SqlParameter("@TaxCode", Function.ToString(line.TaxCode)));
				empty2 += ",@BaseQty ";
				list.Add(new SqlParameter("@BaseQty", Function.ParseDecimal(line.BaseQty)));
				empty2 += ",@BaseOpnQty ";
				list.Add(new SqlParameter("@BaseOpnQty", Function.ParseDecimal(line.BaseOpnQty)));
				empty2 += ", @VatDscntPr ";
				list.Add(new SqlParameter("@VatDscntPr", Function.ParseDecimal(line.VatDscntPr)));
				empty2 += ",@GTotal ";
				list.Add(new SqlParameter("@GTotal", Function.ParseDecimal(line.GTotal)));
				empty2 += ",@GTotalFC ";
				list.Add(new SqlParameter("@GTotalFC", Function.ParseDecimal(line.GTotalFC)));
				empty2 += ", @OcrCode2 ";
				list.Add(new SqlParameter("@OcrCode2", Function.ToString(line.OcrCode2)));
				empty2 += ",@OcrCode3 ";
				list.Add(new SqlParameter("@OcrCode3", Function.ToString(line.OcrCode3)));
				empty2 += ", @OcrCode4 ";
				list.Add(new SqlParameter("@OcrCode4", Function.ToString(line.OcrCode4)));
				empty2 += ",@OcrCode5 ";
				list.Add(new SqlParameter("@OcrCode5", Function.ToString(line.OcrCode5)));
				empty2 += ",@CogsOcrCo2 ";
				list.Add(new SqlParameter("@CogsOcrCo2", Function.ToString(line.CogsOcrCo2)));
				empty2 += ", @CogsOcrCo3 ";
				list.Add(new SqlParameter("@CogsOcrCo3", Function.ToString(line.CogsOcrCo3)));
				empty2 += ",@CogsOcrCo4 ";
				list.Add(new SqlParameter("@CogsOcrCo4", Function.ToString(line.CogsOcrCo4)));
				empty2 += ", @CogsOcrCo5 ";
				list.Add(new SqlParameter("@CogsOcrCo5", Function.ToString(line.CogsOcrCo5)));
				empty2 += ",@Factor1 ";
				list.Add(new SqlParameter("@Factor1", Function.ParseDecimal(line.Factor1)));
				empty2 += ",@Factor2 ";
				list.Add(new SqlParameter("@Factor2", Function.ParseDecimal(line.Factor2)));
				empty2 += ", @Factor3 ";
				list.Add(new SqlParameter("@Factor3", Function.ParseDecimal(line.Factor3)));
				empty2 += ",@Factor4 ";
				list.Add(new SqlParameter("@Factor4", Function.ParseDecimal(line.Factor4)));
				empty2 += ",@UseBaseUn ";
				list.Add(new SqlParameter("@UseBaseUn", Function.ToString(line.UseBaseUn)));
				empty2 += ", @unitMsr ";
				list.Add(new SqlParameter("@unitMsr", Function.ToString(line.unitMsr)));
				empty2 += ",@NumPerMsr ";
				list.Add(new SqlParameter("@NumPerMsr", Function.ParseDecimal(line.NumPerMsr)));
				empty2 += ",@ShipDate ";
				list.Add(new SqlParameter("@ShipDate", Function.ParseDateTime(line.ShipDate)));
				empty2 += ", @FreeTxt ";
				list.Add(new SqlParameter("@FreeTxt", Function.ToString(line.FreeTxt)));
				empty2 += ",@UomCode ";
				list.Add(new SqlParameter("@UomCode", Function.ToString(line.UomCode)));
				empty2 += ", @PQTReqdate ";
				list.Add(new SqlParameter("@PQTReqdate", Function.ParseDateTime(line.PQTReqdate)));
				empty2 += " )";
			}
			else
			{
				empty2 = "update [dbo].[IGE1]\r\n                               set ";
				empty2 += "TargetType=@TargetType ";
				list.Add(new SqlParameter("@TargetType", Function.ParseInt(line.TargetType)));
				empty2 += ",TrgetEntry=@TrgetEntry ";
				list.Add(new SqlParameter("@TrgetEntry", Function.ParseInt(line.TrgetEntry)));
				empty2 += ",BaseRef=@BaseRef ";
				list.Add(new SqlParameter("@BaseRef", Function.ToString(line.BaseRef)));
				empty2 += ", BaseType=@BaseType ";
				list.Add(new SqlParameter("@BaseType", Function.ParseInt(line.BaseType)));
				empty2 += ",BaseEntry=@BaseEntry ";
				list.Add(new SqlParameter("@BaseEntry", Function.ParseInt(line.BaseEntry)));
				empty2 += ",BaseLine=@BaseLine ";
				list.Add(new SqlParameter("@BaseLine", Function.ParseInt(line.BaseLine)));
				empty2 += ",LineStatus= @LineStatus ";
				list.Add(new SqlParameter("@LineStatus", Function.ToString(line.LineStatus)));
				empty2 += ",ItemCode=@ItemCode ";
				list.Add(new SqlParameter("@ItemCode", Function.ToString(line.ItemCode)));
				empty2 += ", Dscription=@Dscription ";
				list.Add(new SqlParameter("@Dscription", Function.ToString(line.Dscription)));
				empty2 += ",Quantity=@Quantity ";
				list.Add(new SqlParameter("@Quantity", Function.ParseDecimal(line.Quantity)));
				empty2 += ",OpenQty=@OpenQty ";
				list.Add(new SqlParameter("@OpenQty", Function.ParseDecimal(line.OpenQty)));
				empty2 += ", Price=@Price ";
				list.Add(new SqlParameter("@Price", Function.ParseDecimal(line.Price)));
				empty2 += ",Currency=@Currency ";
				list.Add(new SqlParameter("@Currency", Function.ToString(line.Currency)));
				empty2 += ",Rate=@Rate ";
				list.Add(new SqlParameter("@Rate", Function.ParseDecimal(line.Rate)));
				empty2 += ", DiscPrcnt=@DiscPrcnt ";
				list.Add(new SqlParameter("@DiscPrcnt", Function.ParseDecimal(line.DiscPrcnt)));
				empty2 += ",LineTotal=@LineTotal ";
				list.Add(new SqlParameter("@LineTotal", Function.ParseDecimal(line.LineTotal)));
				empty2 += ",TotalFrgn=@TotalFrgn ";
				list.Add(new SqlParameter("@TotalFrgn", Function.ParseDecimal(line.TotalFrgn)));
				empty2 += ", OpenSum=@OpenSum ";
				list.Add(new SqlParameter("@OpenSum", Function.ParseDecimal(line.OpenSum)));
				empty2 += ",OpenSumFC=@OpenSumFC ";
				list.Add(new SqlParameter("@OpenSumFC", Function.ParseDecimal(line.OpenSumFC)));
				empty2 += ", WhsCode=@WhsCode ";
				list.Add(new SqlParameter("@WhsCode", Function.ToString(line.WhsCode)));
				empty2 += ",SlpCode=@SlpCode ";
				list.Add(new SqlParameter("@SlpCode", Function.ParseInt(line.SlpCode)));
				empty2 += ",AcctCode=@AcctCode ";
				list.Add(new SqlParameter("@AcctCode", Function.ToString(line.AcctCode)));
				empty2 += ", PriceBefDi=@PriceBefDi ";
				list.Add(new SqlParameter("@PriceBefDi", Function.ParseDecimal(line.PriceBefDi)));
				empty2 += ",DocDate=@DocDate ";
				list.Add(new SqlParameter("@DocDate", Function.ParseDateTime(line.DocDate)));
				empty2 += ",OcrCode=@OcrCode ";
				list.Add(new SqlParameter("@OcrCode", Function.ToString(line.OcrCode)));
				empty2 += ", Project=@Project ";
				list.Add(new SqlParameter("@Project", Function.ToString(line.Project)));
				empty2 += ",CodeBars=@CodeBars ";
				list.Add(new SqlParameter("@CodeBars", Function.ToString(line.CodeBars)));
				empty2 += ",VatPrcnt=@VatPrcnt ";
				list.Add(new SqlParameter("@VatPrcnt", Function.ParseDecimal(line.VatPrcnt)));
				empty2 += ", VatGroup=@VatGroup ";
				list.Add(new SqlParameter("@VatGroup", Function.ToString(line.VatGroup)));
				empty2 += ",PriceAfVAT=@PriceAfVAT ";
				list.Add(new SqlParameter("@PriceAfVAT", Function.ParseDecimal(line.PriceAfVAT)));
				empty2 += ", BaseDocNum=@BaseDocNum ";
				list.Add(new SqlParameter("@BaseDocNum", Function.ParseInt(line.BaseDocNum)));
				empty2 += ",VatSum=@VatSum ";
				list.Add(new SqlParameter("@VatSum", Function.ParseDecimal(line.VatSum)));
				empty2 += ",VatSumFrgn=@VatSumFrgn ";
				list.Add(new SqlParameter("@VatSumFrgn", Function.ParseDecimal(line.VatSumFrgn)));
				empty2 += ", VatSumSy=@VatSumSy ";
				list.Add(new SqlParameter("@VatSumSy", Function.ParseDecimal(line.VatSumSy)));
				empty2 += ",FinncPriod=@FinncPriod ";
				list.Add(new SqlParameter("@FinncPriod", Function.ParseInt(line.FinncPriod)));
				empty2 += ", ObjType=@ObjType ";
				list.Add(new SqlParameter("@ObjType", Function.ToString(line.ObjType)));
				empty2 += ",GrssProfit=@GrssProfit ";
				list.Add(new SqlParameter("@GrssProfit", Function.ParseDecimal(line.GrssProfit)));
				empty2 += ",GrssProfFC=@GrssProfFC ";
				list.Add(new SqlParameter("@GrssProfFC", Function.ParseDecimal(line.GrssProfFC)));
				empty2 += ", TaxCode=@TaxCode ";
				list.Add(new SqlParameter("@TaxCode", Function.ToString(line.TaxCode)));
				empty2 += ",BaseQty=@BaseQty ";
				list.Add(new SqlParameter("@BaseQty", Function.ParseDecimal(line.BaseQty)));
				empty2 += ",BaseOpnQty=@BaseOpnQty ";
				list.Add(new SqlParameter("@BaseOpnQty", Function.ParseDecimal(line.BaseOpnQty)));
				empty2 += ", VatDscntPr=@VatDscntPr ";
				list.Add(new SqlParameter("@VatDscntPr", Function.ParseDecimal(line.VatDscntPr)));
				empty2 += ",GTotal=@GTotal ";
				list.Add(new SqlParameter("@GTotal", Function.ParseDecimal(line.GTotal)));
				empty2 += ",GTotalFC=@GTotalFC ";
				list.Add(new SqlParameter("@GTotalFC", Function.ParseDecimal(line.GTotalFC)));
				empty2 += ", OcrCode2=@OcrCode2 ";
				list.Add(new SqlParameter("@OcrCode2", Function.ToString(line.OcrCode2)));
				empty2 += ",OcrCode3=@OcrCode3 ";
				list.Add(new SqlParameter("@OcrCode3", Function.ToString(line.OcrCode3)));
				empty2 += ", OcrCode4=@OcrCode4 ";
				list.Add(new SqlParameter("@OcrCode4", Function.ToString(line.OcrCode4)));
				empty2 += ",OcrCode5=@OcrCode5 ";
				list.Add(new SqlParameter("@OcrCode5", Function.ToString(line.OcrCode5)));
				empty2 += ",CogsOcrCo2=@CogsOcrCo2 ";
				list.Add(new SqlParameter("@CogsOcrCo2", Function.ToString(line.CogsOcrCo2)));
				empty2 += ", CogsOcrCo3=@CogsOcrCo3 ";
				list.Add(new SqlParameter("@CogsOcrCo3", Function.ToString(line.CogsOcrCo3)));
				empty2 += ",CogsOcrCo4=@CogsOcrCo4 ";
				list.Add(new SqlParameter("@CogsOcrCo4", Function.ToString(line.CogsOcrCo4)));
				empty2 += ", CogsOcrCo5=@CogsOcrCo5 ";
				list.Add(new SqlParameter("@CogsOcrCo5", Function.ToString(line.CogsOcrCo5)));
				empty2 += ",Factor1=@Factor1 ";
				list.Add(new SqlParameter("@Factor1", Function.ParseDecimal(line.Factor1)));
				empty2 += ",Factor2=@Factor2 ";
				list.Add(new SqlParameter("@Factor2", Function.ParseDecimal(line.Factor2)));
				empty2 += ", Factor3=@Factor3 ";
				list.Add(new SqlParameter("@Factor3", Function.ParseDecimal(line.Factor3)));
				empty2 += ",Factor4=@Factor4 ";
				list.Add(new SqlParameter("@Factor4", Function.ParseDecimal(line.Factor4)));
				empty2 += ",UseBaseUn=@UseBaseUn ";
				list.Add(new SqlParameter("@UseBaseUn", Function.ToString(line.UseBaseUn)));
				empty2 += ", unitMsr=@unitMsr ";
				list.Add(new SqlParameter("@unitMsr", Function.ToString(line.unitMsr)));
				empty2 += ",NumPerMsr=@NumPerMsr ";
				list.Add(new SqlParameter("@NumPerMsr", Function.ParseDecimal(line.NumPerMsr)));
				empty2 += ",ShipDate=@ShipDate ";
				list.Add(new SqlParameter("@ShipDate", Function.ParseDateTime(line.ShipDate)));
				empty2 += ", FreeTxt=@FreeTxt ";
				list.Add(new SqlParameter("@FreeTxt", Function.ToString(line.FreeTxt)));
				empty2 += ",UomCode=@UomCode ";
				list.Add(new SqlParameter("@UomCode", Function.ToString(line.UomCode)));
				empty2 += ", PQTReqdate=@PQTReqdate ";
				list.Add(new SqlParameter("@PQTReqdate", Function.ParseDateTime(line.PQTReqdate)));
				empty2 += " where DocEntry=@DocEntry ";
				list.Add(new SqlParameter("@DocEntry", Function.ParseInt(line.DocEntry)));
				empty2 += " and LineNum=@LineNum ";
				list.Add(new SqlParameter("@LineNum", Function.ParseInt(line.LineNum)));
			}
			if (ExecuteNonQuery(empty2, CommandType.Text, list.ToArray()) > 0)
			{
				if (string.IsNullOrEmpty(GetUnitOfWork().ReceiptProduction.UpdateQtyProductionOrder(Function.ParseInt(line.BaseRef), Function.ParseDecimal(line.Quantity), "C")))
				{
					return "";
				}
				return "ERROR";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC LINE FAILED ", ex.Message);
			return ex.Message;
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

	public string UpdateQtyProductionOrderLine(int orderEntry, decimal Qty, string tranType)
	{
		string text = "";
		try
		{
			string query = "Apz_ReceiptFromProduction_UpdateQtyProductionOrderLine";
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

	public string DeleteIGE1(string docEntry, string lineNum)
	{
		try
		{
			string query = "delete from IGE1 WHERE docEntry = '" + docEntry + "' and  lineNum = '" + lineNum + "'";
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

	public DataTableResult GetDataByPOEntry(string barcode)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@barcode", barcode)
			};
			return new DataTableResult("", ExecuteDataTable("Apz_IssueForProduction_GetDataByPOEntry", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTableResult(ex.Message);
		}
	}

	public DataSetResult GetDataByBarcode(string barcode, int docEntry)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>
			{
				new SqlParameter("@barcode", barcode),
				new SqlParameter("@docEntry", docEntry)
			};
			return new DataSetResult("", ExecuteData("Apz_IssueForProduction_GetDataByBarcode", CommandType.StoredProcedure, list.ToArray()));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSetResult(ex.Message);
		}
	}
}
