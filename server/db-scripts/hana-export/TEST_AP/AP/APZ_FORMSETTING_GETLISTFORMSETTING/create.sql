
CREATE PROCEDURE Apz_FormSetting_GetListFormSetting (
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
			SELECT a.ID, a."Name", a."FormId", a."ItemId", a."SetDefault", a."UserSign", a."CreateDate", a."UpdateTime", a."Usersign2", a."BaseRef" 
				FROM APZ_OFST a
				WHERE a."FormId" = :formId 
					AND a."ItemId" = :itemId 
					AND a."UserSign" = :userName 
					ORDER BY a."SetDefault" DESC, ID DESC;
		ELSE
			SELECT a.ID, a."Name", a."FormId", a."ItemId", a."SetDefault", a."UserSign", a."CreateDate", a."UpdateTime", a."Usersign2", a."BaseRef" 
				FROM APZ_OFST a
				WHERE a."FormId" = :formId 
					AND a."ItemId" = :itemId 
					AND a."UserSign" = 'manager' 
					ORDER BY a."SetDefault" DESC, ID DESC;
		END IF;
	ELSEIF :id = -1 THEN
		SELECT TOP 1 a.ID, a."Name", a."FormId", a."ItemId", a."SetDefault", a."UserSign", a."CreateDate", a."UpdateTime", a."Usersign2", a."BaseRef" 
			FROM APZ_OFST a 
			WHERE a."SetDefault" = 1;
	ELSE
		SELECT TOP 1 a.ID, a."Name", a."FormId", a."ItemId", a."SetDefault", a."UserSign", a."CreateDate", a."UpdateTime", a."Usersign2", a."BaseRef" 
			FROM APZ_OFST a 
			WHERE a.ID = :id;
	END IF;
	
END;




