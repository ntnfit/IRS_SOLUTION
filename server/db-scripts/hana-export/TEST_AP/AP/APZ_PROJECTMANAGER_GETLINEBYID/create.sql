CREATE PROCEDURE APZ_ProjectManager_GetLineById (IN Id integer) 
AS 
BEGIN
/*
	------------------------------------------------------------ Create temporary table -----------------------------------------------------------------------------------------
	create local temporary table #tbl as (select top 0 *,cast(null as nvarchar(254)) "Dependent",cast(null as nvarchar(254)) "Resource",cast(null as nvarchar(1)) "IsExist", cast(null as integer) "tempvis", 0 "CommentsCount" from APZ_PRG1);
	---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------
	---------------------------------------------- Task Data ------------------------------------------------------------------------------------------------------------------------------------------------------
	------------------------------------------------------------ INERT DATA TO TEMPTABLE -------------------------------------------------------------------------------------
	insert into #tbl
		select a.*,(select  string_agg(cast("Dependent" as nvarchar(11)),',') from  APZ_PRG2 b where a."AbsEntry"=b."AbsEntry" and a."LineNum"=b."LineNum") as "Dependent"
				,(select  string_agg(cast("Resource" as nvarchar(11)),';') from  APZ_PRG3 c where a."AbsEntry"=c."AbsEntry"  and a."LineNum"=c."LineNum") as "Resource"
				,'Y' as IsExist, ROW_NUMBER() OVER (ORDER BY a."Wbs") tempvis,
				(select count(1) from APZ_COMMENTS c where a."AbsEntry"=c.OBJECT_CODE and c.DETAIL_CODE=a."Wbs" and c.OBJTYPE='1470000132WBS') "CommentsCount"	from APZ_PRG1 a where a."AbsEntry"=:Id;
	-------------------------------------------------------- END TASK -------------------------------------------------------------------------------------
	--Team ------------------------------------- đoạn này chỉ convert còn hổng có hiểu ------------------------------------------------------
	UPDATE T0	SET T0."VisOrder" = T0."tempvis" FROM #tbl T0 WHERE T0."VisOrder" IS NULL;
	SELECT * FROM #tbl ORDER BY "Wbs";
	SELECT  a.*,IFNULL(b."firstName", '')  || ' ' || IFNULL(b."middleName", '')  || ' ' || IFNULL(b."lastName", '') as "EmployeeName",b."branch" as "Branch",b."dept" as "Department" 
								from APZ_PRG3 a left join OHEM b on a."Resource"=b."empID" where "AbsEntry"=:Id;
	
	--Item
	---------------------------------------- Ếch chập nhận được đoạn này nhưng phải convert cho nó chạy ---------------------------------------------------

	UPDATE T0 
	SET PR = i."prQty", 
	    GRPO = i."grQty" ,
		PO="poQty",
		"Delivery"="DeliveryQty",
		AR="ARQty",
		"Invoice"="apQty",
		PQ="pqQty",
		"ActualAmount"="AmountRpt",
		"ActualQty"="ActQty",
		ARDR="ARDAmount",
		APDR="APDAmount",
		IGE="igeQty",
		WOR="WORQty"
		
	FROM 
	APZ_PRG4 T0
	 inner join
	(
		------------------------------------------------------------ APZ_PRG4 -----------------------------------------------------------------
		SELECT a."AbsEntry", a."LineNum", IFNULL(b.PR1, 0) AS "prQty", IFNULL(n.GRPO1, 0) AS "grQty", IFNULL(p.PO1, 0) AS "poQty", IFNULL(d."Delivery1", 0) + IFNULL(dl."DLQuantity", 0) AS "DeliveryQty"
			, IFNULL(pqt.PQ1, 0) AS "pqQty", IFNULL(IGE.IG1, 0) as "igeQty", IFNULL(h."Invoice1", 0) AS "apQty", IFNULL(c.AR1, 0) + IFNULL(iv."ARQuantity", 0) AS "ARQty", IFNULL(rt1."ReportQuantity", 0) AS "ActQty"
			, IFNULL(rt1."ReportTotal", 0) AS "AmountRpt", IFNULL(ODP."ODPAmount", 0) AS "ARDAmount", IFNULL(DPI."DPIAmount", 0) AS "APDAmount", IFNULL(WOR.WOR1, 0) AS "WORQty" FROM APZ_PRG4 a
			
		------------------------------------------------------------ purchase request---------------------------------------------------------------------
		left join(SELECT SUM("Quantity") AS "PR1", "U_OriginCode", "U_OriginLine", "U_OriginType" FROM prq1
						LEFT OUTER JOIN OPRQ ON OPRQ."DocEntry" = PRQ1."DocEntry" WHERE OPRQ."DocStatus" <> 'L' GROUP BY "U_OriginCode", "U_OriginLine", "U_OriginType") b on b."U_OriginCode"=:Id and a."LineNum"=b."U_OriginLine" and b."U_OriginType"='1470000132'
	
		--------------------------------------------------- goods receipt po ------------------------------------------------------------------------------
		left join (SELECT SUM("Quantity") AS "GRPO1", "U_OriginCode", "U_OriginLine", "U_OriginType" FROM PDN1 GROUP BY "U_OriginCode", "U_OriginLine", "U_OriginType") n on n."U_OriginCode"=:Id and a."LineNum"=n."U_OriginLine" and n."U_OriginType"='1470000132'
		
		--------------------------------------------------- purchase order --------------------------------------------------------------------------------
		left join (SELECT SUM("Quantity") AS "PO1", "U_OriginCode", "U_OriginLine", "U_OriginType" FROM POR1 
						LEFT OUTER JOIN OPOR ON OPOR."DocEntry" = POR1."DocEntry" WHERE OPOR."DocStatus" <> 'L' GROUP BY "U_OriginCode", "U_OriginLine", "U_OriginType") p on p."U_OriginCode"=:Id and a."LineNum"=p."U_OriginLine" and p."U_OriginType"='1470000132'
		
		--------------------------------------------------- Delivery --------------------------------------------------------------------------------------
		left join (SELECT SUM("Quantity") AS "Delivery1", "U_OriginCode", "U_OriginLine", "U_OriginType" FROM DLN1 GROUP BY "U_OriginCode", "U_OriginLine", "U_OriginType") d on  d."U_OriginCode"=:Id and a."LineNum"=d."U_OriginLine" and d."U_OriginType"='1470000132'
		
		--------------------------------------------------- A/P invoice ----------------------------------------------------------------------------------
		left join (SELECT SUM("Quantity") AS "Invoice1", "U_OriginCode", "U_OriginLine", "U_OriginType" FROM PCH1 GROUP BY "U_OriginCode", "U_OriginLine", "U_OriginType") h on  h."U_OriginCode"=:Id and a."LineNum"=h."U_OriginLine" and h."U_OriginType"='1470000132'
		
		--------------------------------------------------- A/R invoice ---------------------------------------------------------------------------------
		left join (SELECT SUM("LineTotal") AS "AR1", "U_OriginCode", "U_OriginLine", "U_OriginType" FROM INV1 GROUP BY "U_OriginCode", "U_OriginLine", "U_OriginType") c on c."U_OriginCode"=:Id and a."LineNum"=c."U_OriginLine" and c."U_OriginType"='1470000132'
		
		--------------------------------------------------- Purchase Quotation --------------------------------------------------------------------------
		left join (SELECT SUM("Quantity") AS "PQ1", "U_OriginCode", "U_OriginLine", "U_OriginType" FROM PQT1 GROUP BY "U_OriginCode", "U_OriginLine", "U_OriginType") pqt on pqt."U_OriginCode"=:Id and a."LineNum"=pqt."U_OriginLine" and pqt."U_OriginType"='1470000132'
		
		--------------------------------------------------- A/R Down payment ----------------------------------------------------------------------------
		left join(SELECT SUM("LineTotal") AS "ODPAmount", "U_OriginCode", "U_OriginLine", "U_OriginType" FROM DPI1 
						LEFT OUTER JOIN ODPI ON ODPI."DocEntry" = DPI1."DocEntry" WHERE ODPI."Posted" = 'N' GROUP BY "U_OriginCode", "U_OriginLine", "U_OriginType") ODP on ODP."U_OriginCode"=:Id and a."LineNum"=ODP."U_OriginLine" and ODP."U_OriginType"='1470000132'
		
		--------------------------------------------------- A/P down paymnet ----------------------------------------------------------------------------
		left join(SELECT SUM("LineTotal") AS "DPIAmount", "U_OriginCode", "U_OriginLine", "U_OriginType" FROM DPO1 
						LEFT OUTER JOIN ODPO ON ODPO."DocEntry" = DPO1."DocEntry" WHERE ODPO."Posted" = 'N' GROUP BY "U_OriginCode", "U_OriginLine", "U_OriginType") DPI on DPI."U_OriginCode"=:Id and a."LineNum"=DPI."U_OriginLine" and DPI."U_OriginType"='1470000132'
	
		--------------------------------------------------- lay qty tu document tu Project confirm ------------------------------------------------------
		left join (SELECT y."DocEntry", y."LineNum", y1."BaseEntry", y1."BaseLine" FROM APZ_PJC1 y 
						INNER JOIN APZ_RTC1 y1 ON y1."DocEntry" = y."BaseEntry" AND y1."LineNum" = y."BaseLine") x ON x."BaseEntry" = a."AbsEntry" and x."BaseLine" = a."LineNum" 
		
		left join (SELECT SUM("Quantity") AS "DLQuantity", "U_OriginCode", "U_OriginLine", "U_OriginType" FROM DLN1 GROUP BY "U_OriginCode", "U_OriginLine", "U_OriginType") dl on dl."U_OriginCode"=x."DocEntry" and dl."U_OriginLine"=x."LineNum" and dl."U_OriginType"='1470000135'
		
		left join (SELECT SUM("Quantity") AS "ARQuantity", "U_OriginCode", "U_OriginLine", "U_OriginType" FROM INV1 GROUP BY "U_OriginCode", "U_OriginLine", "U_OriginType") iv on iv."U_OriginCode"=x."DocEntry" and iv."U_OriginLine"=x."LineNum" and iv."U_OriginType"='1470000135'
		 
		 ---------------------------------------------------- lay qty tu Project report ----------------------------------------------------------------
		 left join(SELECT SUM("ActualQty") AS "ReportQuantity", SUM("ActualAmount") AS "ReportTotal", "BaseEntry", "BaseLine" FROM APZ_RTC1 
		 				LEFT OUTER JOIN APZ_ORTC ON APZ_ORTC."DocEntry" = APZ_RTC1."DocEntry" WHERE APZ_ORTC."Status" = 'O' GROUP BY "BaseEntry", "BaseLine") rt1 ON rt1."BaseEntry" = a."AbsEntry" and rt1."BaseLine" = a."LineNum"
		 
		 left join(SELECT SUM("Quantity") as IG1, "U_OriginCode", "U_OriginLine", "U_OriginType" from IGE1
					left join OIGE on OIGE."DocEntry" = IGE1."DocEntry" where OIGE."DocStatus" <> 'L'
					group by "U_OriginCode", "U_OriginLine", "U_OriginType") IGE on IGE."U_OriginCode" = :Id and a."LineNum" = IGE."U_OriginLine" and IGE."U_OriginType" = '1470000132'-- Goods issue
	--------------------------------------Production Order-------------------------------------------------------
		left join(select SUM("PlannedQty") as WOR1,"U_OriginCode","U_OriginLine","U_OriginType" from OWOR where "Status" <>'C'  group by "U_OriginCode","U_OriginLine","U_OriginType") WOR on WOR."U_OriginCode"=:Id and a."LineNum"=WOR."U_OriginLine" and WOR."U_OriginType"='1470000132'
		
		where a."AbsEntry"=:Id  
	) i ON t0."AbsEntry" = i."AbsEntry" and t0."LineNum" = i."LineNum";
	
	select a. *,(select count(1) from APZ_COMMENTS c where a."AbsEntry"=c.OBJECT_CODE and ifnull(c.DETAIL_CODE,'')=a."LineNum" and c.OBJTYPE='1470000132BUDGET') "CommentsCount"  from APZ_PRG4 a where "AbsEntry"=:Id ORDER BY a."VisOrder";
	
	-------------------------------------------------- Document --------------------------------------------------------
	------------------------------------------ CREATE TEMP TABLE -------------------------------------------------------
	create local temporary table #temline as (select Top 0 *, cast(null as int) as "DocNum",cast(null as nvarchar(250)) as "ObjTypeName",cast(null as numeric(19,6)) as "Total",cast(null as nvarchar(254)) as "Remark",cast(null as nvarchar(3)) as "Currency" from APZ_PRG5);

	INSERT INTO #temline ("AbsEntry", "ObjType", "DocEntry", "DocNum", "ObjTypeName", "Total", "Remark", "Currency")
		SELECT :Id, '1470000113', q1."DocEntry", q1."DocNum", 'Purchase Request', q1."DocTotal", q1."Comments", q1."DocCur" FROM OPRQ q1 
									INNER JOIN (SELECT "DocEntry" FROM PRQ1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id GROUP BY "DocEntry") AS q2 ON q2."DocEntry" = q1."DocEntry"
	UNION ALL
		SELECT :Id, '20', q1."DocEntry", q1."DocNum", 'Goods Receipt PO', q1."DocTotal", q1."Comments", q1."DocCur" FROM OPDN q1 
									INNER JOIN (SELECT "DocEntry" FROM PDN1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id GROUP BY "DocEntry") AS q2 ON q2."DocEntry" = q1."DocEntry"
	UNION ALL
		SELECT :Id, '22', q1."DocEntry", q1."DocNum", 'Purchase Order', q1."DocTotal", q1."Comments", q1."DocCur" FROM OPOR q1 
									INNER JOIN (SELECT "DocEntry" FROM POR1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id GROUP BY "DocEntry") AS q2 ON q2."DocEntry" = q1."DocEntry"
	UNION ALL
		SELECT :Id, '15', q1."DocEntry", q1."DocNum", 'Delivery', q1."DocTotal", q1."Comments", q1."DocCur" FROM ODLN q1 
									INNER JOIN (SELECT "DocEntry" FROM DLN1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id GROUP BY "DocEntry") AS q2 ON q2."DocEntry" = q1."DocEntry"
	UNION ALL
		SELECT :Id, '18', q1."DocEntry", q1."DocNum", 'A/P Invoice', q1."DocTotal", q1."Comments", q1."DocCur" FROM OPCH q1 
									INNER JOIN (SELECT "DocEntry" FROM PCH1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id GROUP BY "DocEntry") AS q2 ON q2."DocEntry" = q1."DocEntry"
	UNION ALL
		SELECT :Id, '13', q1."DocEntry", q1."DocNum", 'A/R Invoice', q1."DocTotal", q1."Comments", q1."DocCur" FROM OINV q1 
									INNER JOIN (SELECT "DocEntry" FROM INV1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id GROUP BY "DocEntry") AS q2 ON q2."DocEntry" = q1."DocEntry"
	UNION ALL
		SELECT :Id, '540000006', q1."DocEntry", q1."DocNum", 'Purchase Quotation', q1."DocTotal", q1."Comments", q1."DocCur" FROM OPQT q1 
									INNER JOIN (SELECT "DocEntry" FROM PQT1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id GROUP BY "DocEntry") AS q2 ON q2."DocEntry" = q1."DocEntry"
	UNION ALL
		SELECT :Id, '203', q1."DocEntry", q1."DocNum", 'A/R Down Payment Request', q1."DocTotal", q1."Comments", q1."DocCur" FROM ODPI q1 
									INNER JOIN (SELECT "DocEntry" FROM DPI1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id GROUP BY "DocEntry") AS q2 ON q2."DocEntry" = q1."DocEntry"
	UNION ALL
		SELECT :Id, '204', q1."DocEntry", q1."DocNum", 'A/P Down Payment Request', q1."DocTotal", q1."Comments", q1."DocCur" FROM ODPO q1 
									INNER JOIN (SELECT "DocEntry" FROM DPO1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id GROUP BY "DocEntry") AS q2 ON q2."DocEntry" = q1."DocEntry"
	UNION ALL
		SELECT :Id, '60', q1."DocEntry", q1."DocNum", 'Goods Issue', q1."DocTotal", q1."Comments", q1."DocCur" FROM OIGE q1
									INNER JOIN (SELECT "DocEntry" FROM IGE1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id GROUP BY "DocEntry") q2 on q2."DocEntry" = q1."DocEntry"
	--po
UNION ALL
		SELECT :Id, '202', "DocEntry", "DocNum", 'Production Order',0,"Comments",'' FROM OWOR  WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id;
	
	select * from #temline where "AbsEntry"=:id;
--Sub
select * from APZ_OPRG where  "MasterEntry" =:id and( "BaseLine"='N' or ( "BaseLine"='Y' and "BaseLineActive"='Y') or ("BaseLine"='Y'  and  "BaseLineActive"='N' and "WddStatus" ='W'));
--Dêpnt
	select * from APZ_PRG2 where  "AbsEntry"=:Id;
--Issue
	select a.* from APZ_TGIE a
--right join ATC1 b on a.AttachedFiled=b.AbsEntry
	 where  "ProjectCode"=:Id;
	 --Payment
	 select * from APZ_PRG6 where "AbsEntry"=:Id  ORDER BY "VisOrder";
*/
end;

