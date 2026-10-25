
CREATE PROCEDURE APZ_Routing_GetDataDoc(
	in code nvarchar(50) default '',
	in status char(1) default 'A',
	in fDate timestamp default '',
	in tDate timestamp default '',
	in typeGet int default 4)
AS
BEGIN

	declare checkCode nvarchar(50) = '';

	select case when :code is null then '' else :code end into code from DUMMY;
	select  case when :typeGet is null then 4 else :typeGet end into typeGet from DUMMY;
	select case when :fDate is null then '' else :fDate end into fDate from DUMMY;
	select case when :tDate is null then '' else :tDate end into tDate from DUMMY;
	select case when :status is null then '' else :status end into status from DUMMY;

	create local temporary table #TEMP_APZ_ORTA as (select top 0 * from APZ_ORTA);

	if :typeGet = 4
	THEN
		INSERT INTO #TEMP_APZ_ORTA 
			SELECT * FROM APZ_ORTA WHERE (:code = '' OR "RoutCode" LIKE '%' || :code || '%') AND (:status = '' OR "Status" = :status) AND (:fDate = '' OR CAST("CreateDate" AS date) >= :fDate) 
										AND (:tDate = '' OR CAST("CreateDate" AS date) <= :tDate) ORDER BY "RoutCode" DESC;
	elseif :typeGet = 1
	THEN
			select ifnull((select min("RoutCode") from APZ_ORTA),'') into checkCode from DUMMY;
			if :code <= :checkCode
			THEN
				INSERT INTO #TEMP_APZ_ORTA (SELECT TOP 1 * FROM APZ_ORTA WHERE "RoutCode" = ifnull((SELECT MAX("RoutCode") FROM APZ_ORTA), ''));
			else
				INSERT INTO #TEMP_APZ_ORTA (SELECT TOP 1 * FROM APZ_ORTA WHERE "RoutCode" < :code ORDER BY "RoutCode" DESC);
			END IF;

	ELSEIF :typeGet = 2
	THEN
			select ifnull((select max("RoutCode") from APZ_ORTA),'') into checkCode from DUMMY;
			if :code = :checkCode
			THEN
				INSERT INTO #TEMP_APZ_ORTA (SELECT TOP 1 * FROM APZ_ORTA WHERE "RoutCode" = ifnull((SELECT MIN("RoutCode") FROM APZ_ORTA), ''));
			else
				INSERT INTO #TEMP_APZ_ORTA (SELECT TOP 1 * FROM APZ_ORTA WHERE "RoutCode" > :code ORDER BY "RoutCode" ASC);
			END IF;

	elseif :typeGet = 3
	THEN

			INSERT INTO #TEMP_APZ_ORTA (SELECT * FROM APZ_ORTA WHERE "RoutCode" = ifnull((SELECT MAX("RoutCode") FROM APZ_ORTA), ''));

	ELSEIF :typeGet = 0
	THEN

			INSERT INTO #TEMP_APZ_ORTA (SELECT * FROM APZ_ORTA WHERE "RoutCode" = ifnull((SELECT MIN("RoutCode") FROM APZ_ORTA), ''));

	end IF;

	SELECT a.*, b."SalUnitMsr" AS "SalUnitMsr", ifnull(b."NumInSale", 0) AS "NumInSale", b."BuyUnitMsr", ifnull(b."NumInBuy", 0) AS "NumInBuy", ifnull(b."UgpEntry", -1) AS "UgpCode", b."InvntryUom" AS "InvUnitMsr"
			, ifnull(d."AltQty", 1) / ifnull(d."BaseQty", 1) AS "NumInInven" 
			FROM #TEMP_APZ_ORTA a LEFT OUTER JOIN OITM b ON a."ProdCode" = b."ItemCode" 
							LEFT OUTER JOIN UGP1 d ON b."UgpEntry" = d."UgpEntry" AND d."UomEntry" IN (SELECT "UomEntry" FROM OUOM WHERE "UomName" = b."InvntryUom") ORDER BY "RoutCode";
 	
END



