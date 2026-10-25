
CREATE PROCEDURE APZ_MaterialShortageReport_SearchData(in type int default 0, in code int default 0)
AS
BEGIN
	DECLARE getCode int;
	declare minCode int = ifnull((select min("Code") from APZ_OMSR), 0);
	declare maxCode int = ifnull((select max("Code") from APZ_OMSR), 0);

	if :type = 0 then
		select IFNULL((select top 1 "Code" from APZ_OMSR order by "Code" ASC),0) into getCode from dummy;

	else if :type = 1 then
		
		if :code <= :minCode then

			select IFNULL((select top 1 "Code" from APZ_OMSR order by "Code" DESC),0) into getCode from dummy;
		
		else

			select IFNULL((select top 1 "Code" from APZ_OMSR WHERE "Code" < :code order by "Code" DESC),0) into getCode from dummy;

		end if;

	else if :type = 2 then

		if :code >= :maxCode then

			select IFNULL((select top 1 "Code" from APZ_OMSR order by "Code" ASC),0) into getCode from dummy;

		else

			select IFNULL((select top 1 "Code" from APZ_OMSR WHERE "Code" > :code order by "Code" ASC),0) into getCode from dummy;

		end if;

	else if :type = 3 then

		select IFNULL((select top 1 "Code" from APZ_OMSR order by "Code" DESC),0) into getCode from dummy;

	Else

		select :code into getCode from dummy;

	END if;
	end if;
	end if;
	end if;

	select * from APZ_OMSR WHERE "Code" = :getCode;
	SELECT a.*, b."Uom" as "PrdUoM", c."InvntryUom" as "SubUoM" 
	FROM APZ_MSR1 a
	LEFT JOIN OWOR b on a.WOID = b."DocNum"
	LEFT JOIN OITM c on a."SubCode" = c."ItemCode"
	WHERE "Code" = :getCode;
	
END;




