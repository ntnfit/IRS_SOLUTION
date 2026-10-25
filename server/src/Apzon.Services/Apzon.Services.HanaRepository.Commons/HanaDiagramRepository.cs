using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaDiagramRepository : DiagramRepository
{
	public HanaDiagramRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override void GetDiagramOtherObject(ApzObjectTypes type, string objId, ref DataTable dt, List<string> lstChecked = null, bool isreturn = false)
	{
		string text = "declare maxid int = 0;\r\n                        declare i int = 0;\r\n                        declare tempDocEntry int = -1;\r\n                        declare tempObj int = -1;\r\n";
		Tuple<ApzObjectTypes, string, string>[] source = new Tuple<ApzObjectTypes, string, string>[0];
		if (PurchaseDocuments.FirstOrDefault((Tuple<ApzObjectTypes, string, string> t) => t.Item1 == type) != null)
		{
			text += "create local temporary table #tbl as (select t0.\"DocEntry\",t0.\"BaseEntry\",t0.\"BaseType\",t0.\"ObjType\",t1.\"DocNum\",t1.\"DocStatus\",T1.\"CANCELED\",t1.\"RelatedTyp\",t1.\"RelatedEnt\",t1.\"DocTotal\",T1.\"DocCur\"\r\n                    ,T1.\"isIns\",T1.\"DocTotalFC\",T1.\"CardCode\",T1.\"DocDate\",T0.\"ObjType\" || cast(T0.\"DocEntry\" as varchar(11)) AS \"Id\",cast((cast(T0.\"BaseType\" as nvarchar(10)) || cast(t0.\"BaseEntry\" as nvarchar(11))) as nvarchar(50)) \"FatherId\"\r\n                          from PRQ1 t0 inner join OPRQ T1 on t0.\"DocEntry\"=t1.\"DocEntry\");";
			source = PurchaseDocuments;
		}
		else if (SaleDocuments.FirstOrDefault((Tuple<ApzObjectTypes, string, string> t) => t.Item1 == type) != null)
		{
			text += "create local temporary table #tbl as (select t0.\"DocEntry\",t0.\"BaseEntry\",t0.\"BaseType\",t0.\"ObjType\",t1.\"DocNum\",t1.\"DocStatus\",T1.\"CANCELED\",t1.\"RelatedTyp\"\r\n                            ,t1.\"RelatedEnt\",t1.\"DocTotal\",T1.\"DocCur\",T1.\"isIns\",T1.\"DocTotalFC\",T1.\"CardCode\",T1.\"DocDate\",T0.\"ObjType\" || cast(T0.\"DocEntry\" as nvarchar(11)) AS \"Id\",cast((cast(T0.\"BaseType\" as nvarchar(10)) || cast(t0.\"BaseEntry\" as nvarchar(11))) as nvarchar(50)) \"FatherId\"\r\n                            from RDR1 t0 inner join ORDR T1 on t0.\"DocEntry\"=t1.\"DocEntry\");";
			source = SaleDocuments;
		}
		else if (Transfer.FirstOrDefault((Tuple<ApzObjectTypes, string, string> t) => t.Item1 == type) != null)
		{
			text += "create local temporary table #tbl as (select t0.\"DocEntry\",t0.\"BaseEntry\",t0.\"BaseType\",t0.\"ObjType\",t1.\"DocNum\",t1.\"DocStatus\",T1.\"CANCELED\",t1.\"RelatedTyp\",t1.\"RelatedEnt\",t1.\"DocTotal\"\r\n                        ,T1.\"DocCur\",T1.\"isIns\",T1.\"DocTotalFC\",T1.\"CardCode\",T1.\"DocDate\",T0.\"ObjType\" || cast(T0.\"DocEntry\" as nvarchar(11)) AS \"Id\",cast((cast(T0.\"BaseType\" as nvarchar(10)) || cast(t0.\"BaseEntry\" as nvarchar(11))) as nvarchar(50)) \"FatherId\"\r\n                            from WTQ1 t0 inner join OWTQ T1 on t0.\"DocEntry\"=t1.\"DocEntry\");";
			source = Transfer;
		}
		List<string> list = new List<string>();
		text += "\r\n                             insert into #tbl ";
		foreach (Tuple<ApzObjectTypes, string, string> item2 in source.Where((Tuple<ApzObjectTypes, string, string> t) => !new string[3] { "OPRQ", "ORDR", "OWTQ" }.Contains(t.Item2)))
		{
			list.Add(" \r\n                            select t0.\"DocEntry\",t0.\"BaseEntry\",t0.\"BaseType\",t0.\"ObjType\",t1.\"DocNum\",t1.\"DocStatus\",T1.\"CANCELED\",t1.\"RelatedTyp\",t1.\"RelatedEnt\",t1.\"DocTotal\"\r\n                                ,T1.\"DocCur\",T1.\"isIns\",T1.\"DocTotalFC\",T1.\"CardCode\",T1.\"DocDate\",T0.\"ObjType\" || cast(T0.\"DocEntry\" as nvarchar(11)) AS \"Id\",cast((cast(T0.\"BaseType\" as nvarchar(10)) || cast(t0.\"BaseEntry\" as nvarchar(11))) as nvarchar(50)) \"FatherId\"\r\n                              from " + item2.Item3 + " t0 inner join " + item2.Item2 + " T1 on t0.\"DocEntry\"=t1.\"DocEntry\"");
		}
		text = text + string.Join(" union all ", list.ToArray()) + ";";
		text += " \r\n\r\n                            create local temporary table #tblo as (select distinct * from #tbl);\r\n                            create local temporary table #diagram_cte as (select  top 0 *, cast(null as int) NewId from #tblo);\r\n                            insert into #diagram_cte \r\n                                select   *, row_number() over (order by \"DocEntry\") from #tblo where \"ObjType\"=:ObjType and \"DocNum\"=:DocNum;\r\n                            select ifnull((select max(NewId) from #diagram_cte), 0) into maxid From DUMMY;\r\n                            WHILE $i <= $maxid\r\n                            DO\r\n                                \r\n                                select ifnull((select \"DocEntry\" from #diagram_cte where NewId = $i), -1) into tempDocEntry from DUMMY;\r\n                                select ifnull((select \"ObjType\" from #diagram_cte where NewId = $i), -1) into tempObj from DUMMY;\r\n                                insert into #diagram_cte\r\n                                    select t0.\"DocEntry\",T0.\"BaseEntry\",T0.\"BaseType\",t0.\"ObjType\",t0.\"DocNum\",t0.\"DocStatus\",T0.\"CANCELED\",t0.\"RelatedTyp\",t0.\"RelatedEnt\"\r\n                                        ,t0.\"DocTotal\",t0.\"DocCur\",t0.\"isIns\",t0.\"DocTotalFC\",t0.\"CardCode\",t0.\"DocDate\",\r\n\t\t                              T0.\"ObjType\" || cast(T0.\"DocEntry\" as nvarchar(11)) AS \"Id\",cast((cast(t0.\"BaseType\" as nvarchar(10)) || cast(t0.\"BaseEntry\" as nvarchar(11))) as nvarchar(50)) \"FatherId\"\r\n\t\t                              ,$maxid + row_number() over (order by \"DocEntry\") NewId\r\n\t\t                              from #tbl T0 where t0.\"BaseEntry\" = $tempDocEntry and t0.\"BaseType\"= $tempObj;\r\n                                i = $i + 1;\r\n                                select ifnull((select max(NewId) from #diagram_cte), 0) into maxid From DUMMY;\r\n\r\n                            END WHILE;\r\n\r\n                            create local temporary table #tblx as (select distinct * from #diagram_cte);\r\n                            select ifnull((select max(NewId) from #tblx), 0) into maxid From DUMMY;\r\n                            i = 0;\r\n                            WHILE $i <= $maxid\r\n                            DO\r\n                                \r\n                                select ifnull((select \"BaseEntry\" from #tblx where NewId = $i), -1) into tempDocEntry from DUMMY;\r\n                                select ifnull((select \"BaseType\" from #tblx where NewId = $i), -1) into tempObj from DUMMY;\r\n                                insert into #tblx\r\n                                    select t0.\"DocEntry\",T0.\"BaseEntry\",T0.\"BaseType\",t0.\"ObjType\",t0.\"DocNum\",t0.\"DocStatus\",T0.\"CANCELED\",t0.\"RelatedTyp\",t0.\"RelatedEnt\"\r\n                                        ,t0.\"DocTotal\",t0.\"DocCur\",t0.\"isIns\",t0.\"DocTotalFC\",t0.\"CardCode\",t0.\"DocDate\",\r\n\t\t                             T0.\"ObjType\" || cast(T0.\"DocEntry\" as nvarchar(11)) AS \"Id\",cast((cast(T0.\"BaseType\" as nvarchar(10)) || cast(t0.\"BaseEntry\" as nvarchar(11))) as nvarchar(50)) \"FatherId\"\r\n\t\t                              ,$maxid + row_number() over (order by \"DocEntry\") NewId\r\n\t\t                              from #tblo T0 where t0.\"DocEntry\" = $tempDocEntry and t0.\"ObjType\"= $tempObj;\r\n                                i = $i + 1;\r\n                                select ifnull((select max(NewId) from #tblx), 0) into maxid From DUMMY;\r\n\r\n                            END WHILE;\r\n\t                            \r\n                            select distinct \"ObjType\",\"DocEntry\" \"ObjectId\",\"DocNum\" \"ObjectNum\",'' \"Type\",\"CardCode\" \"ObjectCode\",case when \"CANCELED\" is not null and \"CANCELED\"='Y' then 'C' WHEN \"DocStatus\"='C' then 'L' else  \"DocStatus\" END \"Status\"\r\n                                ,\"DocDate\",\"RelatedTyp\",\"RelatedEnt\", case when \"DocTotalFC\" is not null and  \"DocTotalFC\"<>0 then  \"DocTotalFC\" else \"DocTotal\" end \"DocTotal\"\r\n                                ,\"DocCur\",\"isIns\",\"Id\",\"FatherId\" FROM #tblx order by \"FatherId\";\r\n\r\n\r\n                            drop table #tbl;\r\n                            drop table #tblo;\r\n                            drop table #tblx;";
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
