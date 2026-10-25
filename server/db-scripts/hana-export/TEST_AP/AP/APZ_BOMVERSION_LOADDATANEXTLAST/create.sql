
CREATE PROCEDURE APZ_BomVersion_LoadDataNextLast(in typeGet int default  0,in code nvarchar(50) default '')
AS
BEGIN
	declare item nvarchar(50);
	declare checkItem nvarchar(50) = '';
	if :typeGet = 0
	THEN
		
		Select ifnull((select top 1 "Code" from APZ_OITT order by "Code" ASC),'') into item  from DUMMY;

	elseif :typeGet = 1
	THEN

		if :code = ''
		THEN

			select ifnull((select top 1 "Code" from APZ_OITT order by "Code" ASC),'') into item from DUMMY;

		else

			select ifnull((select top 1 "Code" from APZ_OITT order by "Code" ASC),'') into checkItem from DUMMY;
			if :code = :checkItem
			THEN

				select ifnull((select top 1 "Code" from APZ_OITT order by "Code" DESC),'') into item from DUMMY;

			else

				select ifnull((select top 1 "Code" from APZ_OITT where "Code" < :code order by "Code" ASC),'') into item from DUMMY;

			end if;
		end if;

	elseif :typeGet = 2
	THEN

		if :code = ''
		THEN

			select ifnull((select top 1 "Code" from APZ_OITT order by "Code" DESC),'') into item from DUMMY;

		else

			select ifnull((select top 1 "Code" from APZ_OITT order by "Code" DESC),'') into checkItem FROM DUMMY;
			if :code = :checkItem
			THEN

				select ifnull((select top 1 "Code" from APZ_OITT order by "Code" ASC),'') into item from DUMMY;

			else

				select ifnull((select top 1 Code from APZ_OITT where Code > :code order by Code DESC),'') into item from DUMMY;

			end if;
		end if;
		
	elseIF :typeGet = 3
	THEN

		select ifnull((select top 1 "Code" from APZ_OITT order by "Code" DESC),'') into item from DUMMY;

	end IF;

	SELECT a.*, b."ItemName" AS "ItemName", CASE WHEN IFNULL(a."OcrCode", '') = '' THEN '' WHEN IFNULL(a."OcrCode2", '') = '' THEN a."OcrCode" WHEN IFNULL(a."OcrCode3", '') = '' 
												THEN a."OcrCode" || ',' || a."OcrCode2" WHEN IFNULL(a."OcrCode4", '') = '' THEN a."OcrCode" || ',' || a."OcrCode2" || ',' || a."OcrCode3" WHEN IFNULL(a."OcrCode5", '') = '' 
												THEN a."OcrCode" || ',' || a."OcrCode2" || ',' || a."OcrCode3" || ',' || a."OcrCode4" 
												ELSE a."OcrCode" || ',' || a."OcrCode2" || ',' || a."OcrCode3" || ',' || a."OcrCode4" || ',' || a."OcrCode5" END AS "DisRule" 
							FROM APZ_OITT a INNER JOIN oitm b ON a."Code" = b."ItemCode" WHERE a."Code" = :item;
END




