using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Interfaces.Commons;

namespace Apzon.Services.Repository.Commons;

public class DiagramRepository(IDatabaseService databaseService) : BaseService<DataSet>(databaseService), IDiagram
{
	protected readonly Tuple<ApzObjectTypes, string, string>[] PurchaseDocuments = new Tuple<ApzObjectTypes, string, string>[8]
	{
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oPurchaseRequest, "OPRQ", "PRQ1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oPurchaseOrders, "OPOR", "POR1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oPurchaseQuotations, "OPQT", "PQT1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oPurchaseCreditNotes, "ORPC", "RPC1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oPurchaseDeliveryNotes, "OPDN", "PDN1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oPurchaseInvoices, "OPCH", "PCH1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oPurchaseReturns, "ORPD", "RPD1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oPurchaseDownPayments, "ODPO", "DPO1")
	};

	protected readonly Tuple<ApzObjectTypes, string, string>[] SaleDocuments = new Tuple<ApzObjectTypes, string, string>[7]
	{
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oOrders, "ORDR", "RDR1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oQuotations, "OQUT", "QUT1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oCreditNotes, "ORIN", "RIN1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oDeliveryNotes, "ODLN", "DLN1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oInvoices, "OINV", "INV1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oReturns, "ORDN", "RDN1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oDownPayments, "ODPI", "DPI1")
	};

	protected readonly Tuple<ApzObjectTypes, string, string>[] Transfer = new Tuple<ApzObjectTypes, string, string>[2]
	{
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oInventoryTransferRequest, "OWTQ", "WTQ1"),
		new Tuple<ApzObjectTypes, string, string>(ApzObjectTypes.oStockTransfer, "OWTR", "WTR1")
	};

	public void GetDiagram(ApzObjectTypes type, string objId, ref DataTable dt, List<string> lstChecked = null, bool isreturn = false)
	{
		try
		{
			if (new ApzObjectTypes[2]
			{
				ApzObjectTypes.oProductionOrders,
				ApzObjectTypes.oProductTrees
			}.Contains(type))
			{
				dt = ExecuteDataTable("apz_diagram_get", CommandType.StoredProcedure, new SqlParameter("@ObjectType", (int)type), new SqlParameter("@ObjId", objId));
			}
			else if (new ApzObjectTypes[3]
			{
				ApzObjectTypes.oReportCompletion,
				ApzObjectTypes.oProjectConfirm,
				ApzObjectTypes.oProjectManager
			}.Contains(type))
			{
				dt = ExecuteDataTable("APZ_PMDiagram_Get", CommandType.StoredProcedure, new SqlParameter("@ObjectType", (int)type), new SqlParameter("@ObjId", objId));
			}
			else
			{
				dt = ExecuteDataTable("APZ_DIAGRAM_DOCUMENT_GET", CommandType.StoredProcedure, new SqlParameter("@ObjType", (int)type), new SqlParameter("@DocEntry", objId));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	protected virtual void GetDiagramOtherObject(ApzObjectTypes type, string objId, ref DataTable dt, List<string> lstChecked = null, bool isreturn = false)
	{
		string text = "";
		Tuple<ApzObjectTypes, string, string>[] source = new Tuple<ApzObjectTypes, string, string>[0];
		if (PurchaseDocuments.FirstOrDefault((Tuple<ApzObjectTypes, string, string> t) => t.Item1 == type) != null)
		{
			text = "select t0.DocEntry,t0.BaseEntry,t0.BaseType,t0.ObjType,t1.DocNum,t1.DocStatus,T1.CANCELED,t1.RelatedTyp,t1.RelatedEnt,t1.DocTotal,T1.DocCur,T1.isIns,T1.DocTotalFC,T1.CardCode,T1.DocDate,T0.ObjType+Convert(varchar(11), T0.DocEntry) AS Id,Convert(varchar(50),Convert(varchar(10),T0.BaseType)+Convert(varchar(11), t0.BaseEntry)) FatherId\r\n                            into #tbl from PRQ1 t0 inner join OPRQ T1 on t0.DocEntry=t1.DocEntry\r\n                        union all\r\n                    select t0.DocEntry,t0.BaseEntry,t0.BaseType,'69',t1.DocNum,t1.DocStatus,T1.CANCELED,-1,null,t1.DocTotal,T1.DocCur,'N',T1.DocTotalFC,T1.CardCode,T1.DocDate,'69'+Convert(varchar(11), T0.DocEntry) AS Id,Convert(varchar(50),Convert(varchar(10),T0.BaseType)+Convert(varchar(11), t0.BaseEntry)) FatherId\r\n                              from IPF1 t0 inner join OIPF T1 on t0.DocEntry=t1.DocEntry";
			source = PurchaseDocuments;
		}
		else if (SaleDocuments.FirstOrDefault((Tuple<ApzObjectTypes, string, string> t) => t.Item1 == type) != null)
		{
			text = "select t0.DocEntry,t0.BaseEntry,t0.BaseType,t0.ObjType,t1.DocNum,t1.DocStatus,T1.CANCELED,t1.RelatedTyp,t1.RelatedEnt,t1.DocTotal,T1.DocCur,T1.isIns,T1.DocTotalFC,T1.CardCode,T1.DocDate,T0.ObjType+Convert(varchar(11), T0.DocEntry) AS Id,Convert(varchar(50),Convert(varchar(10),T0.BaseType)+Convert(varchar(11), t0.BaseEntry)) FatherId\r\n                            into #tbl from RDR1 t0 inner join ORDR T1 on t0.DocEntry=t1.DocEntry";
			source = SaleDocuments;
		}
		else if (Transfer.FirstOrDefault((Tuple<ApzObjectTypes, string, string> t) => t.Item1 == type) != null)
		{
			text = "select t0.DocEntry,t0.BaseEntry,t0.BaseType,t0.ObjType,t1.DocNum,t1.DocStatus,T1.CANCELED,t1.RelatedTyp,t1.RelatedEnt,t1.DocTotal,T1.DocCur,T1.isIns,T1.DocTotalFC,T1.CardCode,T1.DocDate,T0.ObjType+Convert(varchar(11), T0.DocEntry) AS Id,Convert(varchar(50),Convert(varchar(10),T0.BaseType)+Convert(varchar(11), t0.BaseEntry)) FatherId\r\n                            into #tbl from WTQ1 t0 inner join OWTQ T1 on t0.DocEntry=t1.DocEntry";
			source = Transfer;
		}
		List<string> list = new List<string>();
		text += "\r\n                             insert into #tbl ";
		foreach (Tuple<ApzObjectTypes, string, string> item2 in source.Where((Tuple<ApzObjectTypes, string, string> t) => !new string[3] { "OPRQ", "ORDR", "OWTQ" }.Contains(t.Item2)))
		{
			list.Add(" \r\n                            select t0.DocEntry,t0.BaseEntry,t0.BaseType,t0.ObjType,t1.DocNum,t1.DocStatus,T1.CANCELED,t1.RelatedTyp,t1.RelatedEnt,t1.DocTotal,T1.DocCur,T1.isIns,T1.DocTotalFC,T1.CardCode,T1.DocDate,T0.ObjType+Convert(varchar(11), T0.DocEntry) AS Id,Convert(varchar(50),Convert(varchar(10),T0.BaseType)+Convert(varchar(11), t0.BaseEntry)) FatherId\r\n                              from " + item2.Item3 + " t0 inner join " + item2.Item2 + " T1 on t0.DocEntry=t1.DocEntry");
		}
		text += string.Join(" union all ", list.ToArray());
		text += " \r\n\r\n                            select distinct * into #tblo from #tbl;\r\n                            with diagram_cte as (\r\n\r\n\t                            select   * from #tblo where ObjType=@ObjType and DocNum=@DocNum\r\n\t                            union all\r\n\t                                  select   t0.DocEntry,T0.BaseEntry,T0.BaseType,t0.ObjType,t0.DocNum,t0.DocStatus,T0.CANCELED,t0.RelatedTyp,t0.RelatedEnt,t0.DocTotal,t0.DocCur,t0.isIns,t0.DocTotalFC,t0.CardCode,t0.DocDate,\r\n\t\t                              T0.ObjType+Convert(varchar(11), T0.DocEntry) AS Id,Convert(varchar(50),Convert(varchar(10),e.ObjType) +convert(varchar(11), e.DocEntry)) FatherId \r\n\t\t                              \r\n\t\t                              from #tbl T0 inner join diagram_cte e on e.DocEntry=t0.BaseEntry and t0.BaseType=e.ObjType            \r\n                            )\r\n                            select distinct * into #tblx from diagram_cte;\r\n\r\n                            with diagram_base as (\r\n\r\n\t                            select * from #tblx\r\n\t                            union all\r\n\t                            select   t0.DocEntry,T0.BaseEntry,T0.BaseType,t0.ObjType,t0.DocNum,t0.DocStatus,T0.CANCELED,t0.RelatedTyp,t0.RelatedEnt,t0.DocTotal,t0.DocCur,t0.isIns,t0.DocTotalFC,t0.CardCode,t0.DocDate,\r\n\t\t                             T0.ObjType+Convert(varchar(11), T0.DocEntry) AS Id,Convert(varchar(50),convert(varchar(11),T0.BaseType)+Convert(varchar(11),t0.BaseEntry)) FatherId\r\n\t\t                             from #tblo t0 inner join diagram_base e on e.BaseEntry=t0.DocEntry and t0.ObjType=e.BaseType\r\n\r\n                            )\r\n\r\n\t                            \r\n                            select distinct * into #tblBase from diagram_base;\r\n\r\n\t\t\t\t\t\t\tdeclare @maxResult int = isnull((select count(1) from #tblBase),0)\r\n\r\n\t\t\t\t\t\t\tdeclare @checkResult int = 0 \r\n\r\n\t\t\t\t\t\t\twhile @maxResult < @checkResult or @checkResult = 0\r\n\t\t\t\t\t\t\tBEGIN\r\n\r\n\t\t\t\t\t\t\t\tset @maxResult = isnull((select count(1) from #tblBase),0);\r\n\t\t\t\t\t\t\t\r\n\t\t\t\t\t\t\t\twith diagram_cte as (\r\n\r\n\t                            select   * from #tblBase\r\n\t                            union all\r\n\t                                  select   t0.DocEntry,T0.BaseEntry,T0.BaseType,t0.ObjType,t0.DocNum,t0.DocStatus,T0.CANCELED,t0.RelatedTyp,t0.RelatedEnt,t0.DocTotal,t0.DocCur,t0.isIns,t0.DocTotalFC,t0.CardCode,t0.DocDate,\r\n\t\t                              T0.ObjType+Convert(varchar(11), T0.DocEntry) AS Id,Convert(varchar(50),Convert(varchar(10),e.ObjType) +convert(varchar(11), e.DocEntry)) FatherId \r\n\t\t                              \r\n\t\t                              from #tbl T0 inner join diagram_cte e on e.DocEntry=t0.BaseEntry and t0.BaseType=e.ObjType            \r\n\t\t\t\t\t\t\t\t)\r\n\r\n\t\t\t\t\t\t\t\tinsert into #tblBase\r\n\t\t\t\t\t\t\t\tselect distinct * from diagram_cte x where not exists(select 1 from #tblBase where DocEntry = x.DocEntry and ObjType = x.ObjType);\r\n\r\n\t\t\t\t\t\t\t\twith diagram_base as (\r\n\r\n\t\t\t\t\t\t\t\t\tselect * from #tblBase\r\n\t\t\t\t\t\t\t\t\tunion all\r\n\t\t\t\t\t\t\t\t\tselect   t0.DocEntry,T0.BaseEntry,T0.BaseType,t0.ObjType,t0.DocNum,t0.DocStatus,T0.CANCELED,t0.RelatedTyp,t0.RelatedEnt,t0.DocTotal,t0.DocCur,t0.isIns,t0.DocTotalFC,t0.CardCode,t0.DocDate,\r\n\t\t\t\t\t\t\t\t\t\t T0.ObjType+Convert(varchar(11), T0.DocEntry) AS Id,Convert(varchar(50),convert(varchar(11),T0.BaseType)+Convert(varchar(11),t0.BaseEntry)) FatherId\r\n\t\t\t\t\t\t\t\t\t\t from #tblo t0 inner join diagram_base e on e.BaseEntry=t0.DocEntry and t0.ObjType=e.BaseType\r\n\r\n\t\t\t\t\t\t\t\t)\r\n\r\n\t\t\t\t\t\t\t\tinsert into #tblBase\r\n\t\t\t\t\t\t\t\tselect distinct * from diagram_base x where not exists(select 1 from #tblBase where DocEntry = x.DocEntry and ObjType = x.ObjType);\r\n\r\n\t\t\t\t\t\t\t\tset @checkResult = isnull((select count(1) from #tblBase),0)\r\n\r\n\t\t\t\t\t\t\tEND\r\n\t                            \r\n                            select distinct ObjType,DocEntry ObjectId,DocNum ObjectNum,'' Type,CardCode ObjectCode,case when CANCELED is not null and CANCELED='Y' then 'C' WHEN DocStatus='C' then 'L' else  DocStatus END Status,DocDate,RelatedTyp,RelatedEnt, case when DocTotalFC is not null and  DocTotalFC<>0 then  DocTotalFC else DocTotal end DocTotal,DocCur,isIns,Id,FatherId FROM #tblBase order by ObjectId, ObjType\r\n\r\n                            drop table #tbl\r\n                            drop table #tblo\r\n                            drop table #tblx\r\n\t\t\t\t\t\t\tdrop table #tblBase";
		string query = text;
		SqlParameter[] array = new SqlParameter[2]
		{
			new SqlParameter("@DocNum", objId),
			null
		};
		int num = (int)type;
		array[1] = new SqlParameter("@ObjType", num.ToString());
		DataTable dataTable = ExecuteDataTable(query, CommandType.Text, array);
		if (!dataTable.IsNotNull())
		{
			return;
		}
		lstChecked = lstChecked ?? new List<string>();
		if (!dt.IsNotNull())
		{
			dt = dataTable.Clone();
		}
		foreach (DataRow row in dataTable.Rows)
		{
			string item = string.Format("'{0}'", Function.ToString(row["Id"]));
			lstChecked.Add(item);
			dt.ImportRow(row);
		}
		dt.AcceptChanges();
	}
}
