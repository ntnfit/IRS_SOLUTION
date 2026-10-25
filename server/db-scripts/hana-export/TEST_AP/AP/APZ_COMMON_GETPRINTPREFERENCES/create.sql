
CREATE PROCEDURE Apz_Common_GetPrintPreferences(
	IN  objType NVARCHAR(50)
,		userName NVARCHAR(50) DEFAULT ''
,		branch INT DEFAULT -1
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
    BEGIN
		
		DECLARE dfGroup NVARCHAR(8);
		
		-- kiểm tra user có user default không, nếu có lấy theo user default
		SELECT IFNULL((SELECT "DfltsGroup" FROM OUSR WHERE USER_CODE = :userName),'') INTO dfGroup FROM DUMMY;
		IF :dfGroup IS NOT NULL OR :dfGroup <> '' THEN 
			SELECT * FROM UDG1 WHERE "Code" = :dfGroup AND "ObjType" = :objType;
		ELSE 
			SELECT * FROM ADP1 WHERE "ObjType" = :objType;
		END IF;
    END;




