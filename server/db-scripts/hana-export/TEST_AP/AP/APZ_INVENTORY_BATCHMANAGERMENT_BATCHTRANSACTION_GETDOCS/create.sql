
CREATE PROCEDURE APZ_Inventory_BatchManagerment_BatchTransaction_GetDocs (
	IN 	distNumber NVARCHAR(30) DEFAULT ''
,		ItemFrom NVARCHAR(50) DEFAULT ''
,		ItemTo NVARCHAR(50) DEFAULT ''
,		Status INT DEFAULT -1
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
BEGIN
	DECLARE maxitem nvarchar(50);  
	DECLARE minitem nvarchar(50);
	SELECT MAX("ItemCode") INTO maxitem FROM OITM;
	SELECT MIN("ItemCode") INTO minitem FROM OITM;
	create local temporary table #tbl as (select top 0 b.*, cast(null as numeric(19,6)) "Allocated", cast(null as nvarchar(50)) "LocCode", cast(null as numeric(19,6)) "Qty" from (select 1 "A" from DUMMY) a left join OBTN b ON 1=2);
	
	insert into #tbl
	SELECT b.*, "CommitQty" AS "Allocated", "WhsCode" AS "LocCode" , a."Quantity"
		FROM OBTQ a 
		INNER JOIN OBTN b ON a."MdAbsEntry" = b."AbsEntry" 
		WHERE (b."DistNumber" = :distNumber OR :distNumber = '') 
			AND (:Status = -1 OR b."Status" = :Status) 
			AND a."ItemCode" BETWEEN IFNULL(NULLIF(:ItemFrom, ''), :minitem) AND IFNULL(NULLIF(:ItemTo, ''), :maxitem);
			
	update #tbl set "Quantity" = "Qty";
	select * from #tbl;
END;




