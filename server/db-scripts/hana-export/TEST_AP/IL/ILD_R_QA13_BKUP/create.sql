CREATE procedure ILD_R_QA13_Bkup (
	in fdate			timestamp,
	in tdate			timestamp,
	in itemcode			nclob,--ILD_LayItemThanhPham
	in warehouse		nclob,
	in groupID			nclob, --USP_CFL_ITEMGROUP
	in batch			nclob, --ILD_LayBatch_BCQAQC
	in UDCode			nclob, --ILD_Lay_UDCODEQA
	in QAResult			nclob
)
as
begin


call USP_RPT_PROD_04 (fdate,tdate,itemcode,warehouse,batch,'','Y',tab_result);
create local temporary table #tbl as (select * from $tab_result);

create local temporary table #QCRESULT_SSCC
(
	Row_num int,
	QADocEntry varchar(50),
	ItemCode varchar(50),
	InsLotStock_SSCC varchar(5000),
	InsLotStock_Type varchar(15),
	InsLotStock_Status varchar(15),
	UDCode varchar(10),
	PlanNo	 varchar(50)
);
---
insert into #QCRESULT_SSCC (Row_num,QADocEntry,
							ItemCode,  
							InsLotStock_SSCC,
							InsLotStock_Type,
							InsLotStock_Status,
							UDCode,
							PlanNo
							)							
							
select ROW_NUMBER() OVER (),a."DocEntry" as "QADocEntry", 
			  a."U_ItemCode",
			  b."U_SSCC" ,
			  b."U_Type", 
			  b."U_Status",
		 	  a."U_UDCode",
		 	  a."U_TemplateCode"
		      --a."U_BaseReceiptEntry"
from "@QC_RESULT_H" a
inner join "@QC_RESULT_INS_L" b on a."DocEntry"  = b."DocEntry"
where ifnull(cast(b."U_SSCC" as varchar(5000)),'') <> ''
		and "U_BaseDocType"  in (59,202)
		and "U_BaseDocEntry" in (select distinct DocID from  #tbl);
---
Create local temporary table #SSCCDetail_QAResult
(
		QADocEntry varchar(50),
		Row_num int,
		SSCC_Part nvarchar(50),
		InsLotStock_Type varchar(15),
		UDCode varchar(50),
		ItemCode varchar(50),
		PlanNo	 varchar(50)
		
);
----
Begin
	Declare loopCount int = 0;
	Declare i int = 1;
	Declare QAResult_SSCC nvarchar(5000);
	Declare QADocEntry varchar(50);
	Declare InsLotStock_Type varchar(15);
	Declare UDCode varchar(50);
	Declare ItemCode varchar(50);
	Declare PlanNo varchar(50);
	
	Select Count(*) Into loopCount From #QCRESULT_SSCC;
	       While :i <= :loopCount
				DO
				Select a.InsLotStock_SSCC Into QAResult_SSCC From #QCRESULT_SSCC a Where Row_num = :i;
				Select a.QADocEntry Into QADocEntry From #QCRESULT_SSCC a Where Row_num = :i;
				Select a.InsLotStock_Type Into InsLotStock_Type From #QCRESULT_SSCC a Where Row_num = :i;
				Select a.UDCode Into UDCode From #QCRESULT_SSCC a Where Row_num = :i;
				Select a.ItemCode Into ItemCode From #QCRESULT_SSCC a Where Row_num = :i;
				Select a.PlanNo Into PlanNo From #QCRESULT_SSCC a Where Row_num = :i;
				
			 	insert into #SSCCDetail_QAResult ( SSCC_Part, Row_Num)
			 	select  "part",i
			 	from  "SPLITSTRING" (:QAResult_SSCC,',');
				
				--
				Update  a
				set QADocEntry = :QADocEntry,  InsLotStock_Type  =:InsLotStock_Type,
					UDCode  =:UDCode,
					ItemCode = :ItemCode,
					PlanNo   = :PlanNo
				from #SSCCDetail_QAResult a
				where i = row_num;
				
					i := i + 1;
			END WHILE;

End;


 


---
 select a.GRPCODE,a.GRPNAME,
		a.ITEMCODE,a.ITEMNAME,a.ITEMUOM,a.WOID,a.WONO,a.WOSTATUS,a.WOTYPE,
		a.DOCTYPE,a.DOCNOTE,a.DOCID,a.DOCDATE,a.DOCTIME,a.TOTALQTY,a.QTY,
		a.UNITPRICE,a.AMOUNT,a.DISNO,a.DISID,a.NOTE,a.SSCC,a.BATCH,a.SSCC_STATUS,
		a.ISSSCC_FATHER,a.ORIBATCH,a.OUTSOURCINGID,a.MnfDate, a.ExpDate,
		
 		b.QADocEntry, b.InsLotStock_Type, b.UDCode, b.PlanNo
 from #tbl a
 left join #SSCCDetail_QAResult b on a.SSCC = b.SSCC_Part
 
 
 where   (
			 IFNULL(:UDCODE, '') = ''  OR cast (b.UDCode as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:UDCODE, ','))
		    or      (cast (ifnull(b.UDCode,'') as nvarchar(50)) = case when IFNULL(:UDCODE, '') = '999' then '' end )
		    
		 )		 
		 
		 And 
		 (
		 	  Ifnull(:QAResult, '') = ''  OR cast (QADocEntry as nvarchar) IN (SELECT "DATA" FROM "SPLIT"(:QAResult, ','))
		      or      (cast (ifnull(QADocEntry,'') as nvarchar(50)) = case when IFNULL(:QAResult, '') = '0' then '' end )
		 );

drop table #tbl;
drop table #QCRESULT_SSCC;
drop table #SSCCDetail_QAResult;
 
end;
