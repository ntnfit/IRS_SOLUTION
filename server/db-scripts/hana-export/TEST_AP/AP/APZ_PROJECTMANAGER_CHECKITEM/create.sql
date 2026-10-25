
Create PROCEDURE APZ_ProjectManager_CheckItem (IN Id int DEFAULT 0, IN lineNum int DEFAULT -1)
AS
BEGIN
	 
	 IF (:lineNum IN (SELECT "U_OriginLine" FROM PRQ1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id AND "U_OriginLine" = :lineNum) 
	 OR :lineNum IN (SELECT "U_OriginLine" FROM PDN1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id AND "U_OriginLine" = :lineNum) 
	 OR :lineNum IN (SELECT "U_OriginLine" FROM POR1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id AND "U_OriginLine" = :lineNum) 
	 OR :lineNum IN (SELECT "U_OriginLine" FROM DLN1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id AND "U_OriginLine" = :lineNum) 
	 OR :lineNum IN (SELECT "U_OriginLine" FROM PCH1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id AND "U_OriginLine" = :lineNum) 
	 OR :lineNum IN (SELECT "U_OriginLine" FROM INV1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id AND "U_OriginLine" = :lineNum) 
	 OR :lineNum IN (SELECT "U_OriginLine" FROM PQT1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id AND "U_OriginLine" = :lineNum) 
	 OR :lineNum IN (SELECT "U_OriginLine" FROM DPI1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id AND "U_OriginLine" = :lineNum) 
	 OR :lineNum IN (SELECT "U_OriginLine" FROM DPO1 WHERE "U_OriginType" = '1470000132' AND "U_OriginCode" = :Id AND "U_OriginLine" = :lineNum)) THEN 
	 
	 	SELECT :lineNum, 'Cannot remove name because it has been used' FROM DUMMY;
		RETURN;
		
	END IF;
	 
	select 0, '' FROM DUMMY;

end




