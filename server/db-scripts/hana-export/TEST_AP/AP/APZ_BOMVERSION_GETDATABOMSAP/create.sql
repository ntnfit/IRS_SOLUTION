
CREATE PROCEDURE APZ_BomVersion_GetDataBomSap(in Code nvarchar(50) default '')
AS
BEGIN
	declare name nvarchar(254) = '';
	declare id int;
	
	create local temporary table #tempdoc as (select top 0 *, cast(null as nvarchar(254)) "ItemName" from APZ_OITT);
	create local temporary table #templine as (select top 0 *, cast(null as nvarchar(254)) "ItemName" from APZ_ITT1);

	select ifnull((select top 1 "BomVerID" from APZ_OITT where "Code" = :Code and "Type" = 'N' and ("ActTo" = null or current_date between "ActFrom" and "ActTo")),0) into id from DUMMY;

	if :id = 0
	THEN

		INSERT INTO #tempdoc("BomVerID", "Version", "Code", "CreateDate", "Line", "Remark", "ToWH", "Project", "PriceList", "Qauntity", "Type", "Status", "ItemName", "ActFrom", "ActTo") 
			SELECT 0, '', "Code", current_date, NULL, '', "ToWH", "Project", "PriceList", "Qauntity", 'S', 'Y', '', current_date, current_date FROM OITT WHERE "Code" = :Code;

		INSERT INTO #templine("Father", "ChildNum", "Code", "Quantity", "Warehouse", "Price", "PriceList", "IssueMthd", "Uom", "Comment", "Project", "Type", "AddQuantit", "WipActCode", "ItemName") 
			SELECT 0, a."ChildNum", a."Code", a."Quantity", a."Warehouse", a."Price", a."PriceList", a."IssueMthd", a."Uom", a."Comment", a."Project", a."Type", a."AddQuantit", a."WipActCode"
				, CASE WHEN a."Type" = 4 THEN b."ItemName" WHEN a."Type" = 290 THEN c."ResName" WHEN a."Type" = -18 THEN '' END 
			FROM ITT1 a LEFT OUTER JOIN OITM b ON a."Code" = b."ItemCode" LEFT OUTER JOIN ORSC c ON a."Code" = c."ResCode" WHERE "Father" = :Code;
	else

		INSERT INTO #tempdoc (SELECT a.*, b."ItemName" FROM APZ_OITT a INNER JOIN OITM b ON a."Code" = b."ItemCode" WHERE a."BomVerID" = :id);
		INSERT INTO #templine (SELECT a.*, b."ItemCode" FROM APZ_ITT1 a INNER JOIN OITM b ON a."Code" = b."ItemCode" WHERE a."Father" = :id);

	end if;

	SELECT *, CAST('' AS nvarchar(254)) AS "DisRule" FROM #tempdoc;
	SELECT *, CAST('' AS nvarchar(254)) AS "DisRule" FROM #templine;

END;



