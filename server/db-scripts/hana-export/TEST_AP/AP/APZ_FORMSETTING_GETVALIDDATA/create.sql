
CREATE PROCEDURE Apz_FormSetting_GetValidData (
	IN 	id INT DEFAULT 0
,		formId	NVARCHAR(20) DEFAULT ''
,		itemId	NVARCHAR(50) DEFAULT ''
,		userName NVARCHAR(30) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	DECLARE isExist INT := 0;
		
	IF :id = 0 THEN
		SELECT COUNT(*) INTO isExist FROM APZ_OFST a WHERE a."FormId" = :formId AND a."ItemId" = :itemId AND a."UserSign" = :userName;
		IF isExist > 0 THEN
			SELECT a.ID INTO id
				FROM APZ_OFST a
				WHERE a."FormId" = :formId 
					AND a."ItemId" = :itemId 
					AND a."UserSign" = :userName 
					ORDER BY a."SetDefault" DESC, ID DESC;
		ELSE
			SELECT COUNT(*) INTO isExist FROM APZ_OFST a WHERE a."FormId" = :formId AND a."ItemId" = :itemId AND a."UserSign" = 'manager';
			IF isExist > 0 THEN
				SELECT a.ID INTO id
					FROM APZ_OFST a
					WHERE a."FormId" = :formId 
						AND a."ItemId" = :itemId 
						AND a."UserSign" = 'manager' 
						ORDER BY a."SetDefault" DESC, ID DESC;
			END IF;
		END IF;
	ELSEIF :id = -1 THEN
		SELECT COUNT(*) INTO isExist FROM APZ_OFST a WHERE  a."SetDefault" = 1;
		SELECT TOP 1 a.ID INTO id
			FROM APZ_OFST a 
			WHERE a."SetDefault" = 1;
	END IF;
	
	SELECT * FROM APZ_FST2 a WHERE a.ID = :id;
	
END;




