
CREATE PROCEDURE APZ_SalesBlanketAgreement_GetNumerMax (
	IN Series INT DEFAULT 47
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	DECLARE Seri INT := 0;
	-- nếu là sales blanket
	IF :Series = 47 THEN
		-- lấy ra số series từ bảng NNM1
		SELECT IFNULL((SELECT TOP 1 "Series" FROM NNM1 WHERE "ObjectCode" = '1250000025' AND "DocSubType" = 'C' ORDER BY "Series"),:Series) INTO Seri  FROM DUMMY;	
	-- nếu là purchase blanket
	ELSE
		-- lấy ra số series từ bảng NNM1
		SELECT IFNULL((SELECT TOP 1 "Series" FROM NNM1 WHERE "ObjectCode" = '1250000025' AND "DocSubType" = 'S' ORDER BY "Series"),:Series) INTO Seri  FROM DUMMY;	
	END IF;
	
	SELECT TOP 1 "Number" 
			FROM OOAT 
			WHERE "Series" = :Seri 
			ORDER BY "Number" DESC;
END;




