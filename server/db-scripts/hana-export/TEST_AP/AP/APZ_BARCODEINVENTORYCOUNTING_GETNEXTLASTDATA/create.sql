
CREATE PROCEDURE APZ_BarcodeInventoryCounting_GetNextLastData(in typeGet int default null, in docEntry int default 0)
AS
BEGIN
	
	declare getCode int = 0;

	---------------------------------chuẩn hóa input để loại bỏ các trường hợp cần kiểm tra-------------------------------------------
	if :typeGet is null
	THEN
		typeGet = 4;
	elseif :typeGet = 2
	THEN
		if IFNULL(:docEntry,0) = 0
		THEN
			typeGet = 3;
		END IF;
	elseif :typeGet = 1
	THEN
		if ifnull(:docEntry,0) = 0
		THEN
			typeGet = 0;
		END IF;
	END IF;
	--------------------------------- ENd chuẩn hóa input ------------------------------------------------------------------

	if :typeGet = 4
	THEN
		SELECT * FROM APZ_OBIC WHERE (IFNULL(:docEntry, 0) = 0 OR "DocEntry" = :docEntry);
	elseif :typeGet = 3
	THEN

		select IFNULL((select max("DocEntry") from APZ_OBIC), 0) into getCode FROM DUMMY;
		select * from APZ_OBIC where "DocEntry" = :getCode;

	elseif :typeGet = 2
	THEN

		select IFNULL((select min("DocEntry") from APZ_OBIC where "DocEntry" > :docEntry),0) into getCode from DUMMY;

		if :getCode = 0
		THEN
			select IFNULL((select min("DocEntry") from APZ_OBIC), 0) into getCode from DUMMY;
		END IF;

		select * from APZ_OBIC where "DocEntry" = :getCode;

	elseif :typeGet = 1
	THEN

		select IFNULL((select Max("DocEntry") from APZ_OBIC where "DocEntry" < :docEntry), 0) into getCode from DUMMY;

		if :getCode = 0
		THEN
			select IFNULL((select Max(DocEntry) from APZ_OBIC), 0) into getCode from DUMMY;
		END IF;

		select * from APZ_OBIC where "DocEntry" = :getCode;

	else
	
		select IFNULL((select min("DocEntry") from APZ_OBIC), 0) into getCode from DUMMY;
		select * from APZ_OBIC where "DocEntry" = :getCode;

	END IF;
	
END;



