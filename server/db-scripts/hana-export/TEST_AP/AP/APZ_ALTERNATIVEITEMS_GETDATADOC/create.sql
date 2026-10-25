
CREATE PROCEDURE APZ_AlternativeItems_GetDataDoc(in typeGet int, in origItem nvarchar(50))
AS
BEGIN
	declare checkOrigItem nvarchar(50) = '';
	----------------------------------- First ------------------------------------
	if :typeGet = 0
	THEN
		SELECT TOP 1 "OrigItem" FROM OALI GROUP BY "OrigItem" ORDER BY "OrigItem" ASC;
	---------------------------------- Last -------------------------------------
	elseif :typeGet = 3
	THEN
		SELECT TOP 1 "OrigItem" FROM OALI GROUP BY "OrigItem" ORDER BY "OrigItem" DESC;
	---------------------------------- Next ------------------------------------
	elseif :typeGet = 2
	THEN
		select IFNULL((select max("OrigItem") from OALI),'') into checkOrigItem from DUMMY;
		if :origItem = checkOrigItem
		THEN
			SELECT TOP 1 "OrigItem" FROM OALI GROUP BY "OrigItem" ORDER BY "OrigItem";
		else	
			SELECT TOP 1 "OrigItem" FROM OALI WHERE "OrigItem" > :origItem GROUP BY "OrigItem" ORDER BY "OrigItem" ASC;
		end IF;
		
	----------------------------------- Previous ------------------------------
	elseif :typeGet = 1
	THEN
		select IFNULL((select min("OrigItem") from OALI),'') into checkOrigItem from DUMMY;
		if :origItem = :checkOrigItem or ifnull(:origItem,'') = ''
		THEN
			SELECT TOP 1 "OrigItem" FROM OALI GROUP BY "OrigItem" ORDER BY "OrigItem" DESC;
		
		else
			SELECT TOP 1 "OrigItem" FROM OALI WHERE "OrigItem" < :origItem GROUP BY "OrigItem" ORDER BY "OrigItem" DESC;
		end IF;
		
	end IF;
END;



