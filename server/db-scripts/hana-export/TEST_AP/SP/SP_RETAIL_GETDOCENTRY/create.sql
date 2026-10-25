
CREATE PROCEDURE SP_Retail_GetDocEntry 
(
StoreCode nvarchar(50)
, POSID int)
AS
BEGIN
declare StartDate nvarchar(30);
declare EndDate nvarchar(30);
declare StartTime nvarchar(30);
declare EndTime nvarchar(30);
	CREATE LOCAL TEMPORARY TABLE #tblClockIn AS (select TOP 0* from APZ_CLOCK);
	insert into #tblClockIn SELECT * FROM APZ_CLOCK WHERE "StoreCode" = :StoreCode AND "PosID" = :POSID AND "ClockType" = 'I' ORDER BY "LineId" DESC;
	CREATE LOCAL TEMPORARY TABLE #tblClockOut AS (select TOP 0* from APZ_CLOCK);
	insert into #tblClockOut SELECT * FROM APZ_CLOCK WHERE "StoreCode" = :StoreCode AND "PosID" = :POSID AND "ClockType" = 'O' ORDER BY "LineId" DESC;
	select ifnull((SELECT CAST("ClockDate" AS nvarchar(30)) FROM #tblClockIn),'') into StartDate from DUMMY;
	select ifnull((SELECT CAST("ClockDate" AS nvarchar(30)) FROM #tblClockOut),'') into EndDate from DUMMY;
	select ifnull((SELECT "ClockTime" FROM #tblClockIn),'') into StartTime from DUMMY;
	select ifnull((SELECT "ClockTime" FROM #tblClockOut),'') into EndTime from DUMMY;

CREATE LOCAL TEMPORARY TABLE #tblDoc AS (SELECT Top 0 * FROM ORDR);
	insert into #tblDoc select * from ORDR WHERE "U_RetailStore" = :StoreCode AND "U_Posid" = :POSID AND IFNULL("U_RetailPaidEnt", 0) > 0 AND IFNULL(CANCELED, 'N') <> 'Y' AND 
	(
	(:StartDate = :EndDate AND "DocDate" = :StartDate AND "DocTime" BETWEEN :StartTime AND :EndTime)
					 OR (:StartDate < :EndDate AND 
					 (("DocDate" > :StartDate AND "DocDate" < :EndDate) OR ("DocDate" = :StartDate AND "DocTime" >= :StartTime) OR ("DocDate" = :EndDate AND "DocTime" <= :EndTime))
					 				));
	CREATE LOCAL TEMPORARY TABLE #tblCardCode AS (SELECT DISTINCT ("CardCode") FROM #tblDoc);

	create local temporary table #tbl ("ListDocEntry" nvarchar(5000),"BpCode" nvarchar(50));
	--L?y các DocEntry mà không có VAT
	insert into #tbl 
		select 
			(
          SELECT STRING_AGG(CAST(',' AS varchar(5000)) || CAST(m."DocEntry" AS nvarchar))
          FROM #tblDoc m
          WHERE m."CardCode" = b."CardCode" AND IFNULL("VatSum", 0) <= 0)
			,"CardCode"  from #tblCardCode b order by "CardCode"  ASC;
			
	
	--L?y các DocEntry mà có VAT
	insert into #tbl 
		select 
			(
          SELECT STRING_AGG(CAST(',' AS varchar(5000)) || CAST(m."DocEntry" AS nvarchar))
          FROM #tblDoc m
          WHERE m."CardCode" = b."CardCode" AND IFNULL("VatSum", 0) > 0)
			,"CardCode"  from #tblCardCode b order by "CardCode"  ASC;
	-- Cắt dấu phẩy ở đầu chuỗi DocENtry
	update #tbl set "ListDocEntry" = Right("ListDocEntry",length("ListDocEntry")-1); 
	select * from #tbl;

END

--SELECT Right ('Hello', 4) "left" FROM DUMMY;




