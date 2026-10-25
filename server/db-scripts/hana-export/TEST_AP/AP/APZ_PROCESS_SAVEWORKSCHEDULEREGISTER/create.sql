CREATE PROCEDURE APZ_Process_SaveWorkScheduleRegister
AS
BEGIN
	DECLARE errorCode int = 0;
	DECLARE checkDulp int = 0;
	DECLARE lineNum int = 0;
	
	--Create temp table
	CREATE LOCAL TEMPORARY TABLE #APZ_OWSRTmp AS (SELECT top 0 * FROM APZ_OWSR);
	exec 'DROP TABLE #APZ_OWSRTmp;';
	exec 'CREATE LOCAL TEMPORARY TABLE #APZ_OWSRTmp AS (SELECT * FROM #APZ_OWSR);';
	
	--Check Dulplicate timestamp
	SELECT (SELECT top 1 a."empID" 
		FROM #APZ_OWSRTmp a JOIN #APZ_OWSRTmp b
			ON a."empID" = b."empID" AND a."LineNum" <> b."LineNum"
			AND ((a."ValidTo" IS NULL AND b."ValidTo" IS NULL) 
				OR (a."ValidFrom" <= b."ValidTo" AND a."ValidTo" >= b."ValidFrom"))) 
	INTO checkDulp FROM DUMMY;	
	IF IFNULL(:checkDulp,0) != 0
	THEN
		SELECT 810036 AS "ErrorNumber", 'Timestamp cannot be duplicate' AS "ErrorMessage" FROM DUMMY;
		RETURN;
	END IF;
	
	--Insert data
	DELETE FROM #APZ_OWSRTmp WHERE "ValidFrom" IS NULL and "ValidTo" IS NULL; 
	DELETE FROM APZ_OWSR a 
	WHERE EXISTS(
		SELECT 1 
		FROM #APZ_OWSRTmp b
		WHERE a."empID" = b."empID" AND 
			(a."LineNum" = IFNULL(b."LineNum", -1)
			OR (a."ValidFrom" = b."ValidFrom" AND a."ValidTo" = b."ValidTo")));
	
	SELECT IFNULL((SELECT max("LineNum") FROM APZ_OWSR),0) INTO lineNum from DUMMY;
	WHILE EXISTS(SELECT 1 FROM #APZ_OWSRTmp WHERE "LineNum" IS NULL) DO
		UPDATE top 1 #APZ_OWSRTmp set "LineNum" = :lineNum + 1 WHERE "LineNum" is null;
		lineNum = :lineNum + 1;
	END WHILE;
	
	INSERT INTO APZ_OWSR("empID", "LineNum", "ValidFrom", "ValidTo", "SchID") SELECT "empID", "LineNum", "ValidFrom", "ValidTo", "SchID" FROM #APZ_OWSRTmp;
END;

