
CREATE PROCEDURE Apz_InventoryTransfer_GetWhsByBin (
	IN 	binCode NVARCHAR(5000)
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	DECLARE isExist INT := 0;
	DECLARE isExistBin INT := 0;
		
	SELECT IFNULL((SELECT 1 FROM OBIN WHERE "BinCode"= :binCode), 0) INTO isExistBin FROM DUMMY;
	IF isExistBin = 0 THEN
		SELECT IFNULL((SELECT 1 FROM OWHS WHERE "WhsCode" = :binCode), 0) INTO isExistBin FROM DUMMY;
		IF isExistBin > 0 THEN
			SELECT :binCode AS "WhsCode", -1 AS "AbsEntry" FROM DUMMY;
		ELSE
			SELECT :binCode AS "WhsCode", -99 AS "AbsEntry" FROM DUMMY;
		END IF;
	ELSE
		SELECT TOP 1 "WhsCode", "AbsEntry" FROM OBIN WHERE "BinCode" = :binCode;
	END IF;
	
END;




