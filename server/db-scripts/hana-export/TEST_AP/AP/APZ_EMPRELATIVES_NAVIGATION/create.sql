
CREATE PROCEDURE  apz_EmpRelatives_navigation
(in Currnt INT,
in EmpID INT,
in Typ INT
)
AS
BEGIN
DECLARE Mx INT;
	DECLARE Mn INT;
	 Select IFNULL((SELECT TOP 1 "relaID" FROM APZ_OHER WHERE "empID" = :EmpID ORDER BY "relaID" DESC),0) into Mx from Dummy;	
	 Select IFNULL((SELECT TOP 1 "relaID" FROM APZ_OHER WHERE "empID" = :EmpID ORDER BY "relaID" ASC),0) into Mn from Dummy;
-- First:
IF(:Typ=1 OR (:Typ = 3 AND :Currnt>=:Mx))
THEN
SELECT TOP 1 "relaID" FROM APZ_OHER WHERE "empID" = :EmpID ORDER BY "relaID" ASC;
-- Last:
ELSE IF(:Typ=4 OR (:Typ = 2 AND :Currnt<=:Mn))
THEN
SELECT TOP 1 "relaID" FROM APZ_OHER WHERE "empID" = :EmpID ORDER BY "relaID" DESC;
-- Previous:
ELSE IF(:Typ=2 AND :Currnt>:Mn)
THEN
		
	 SELECT TOP 1 "relaID" FROM APZ_OHER WHERE "relaID"<:Currnt AND "empID" = :EmpID ORDER BY "relaID" desc;

ELSE IF (:Typ=3 AND :Currnt<:Mx)
THEN
	
	 SELECT TOP 1 "relaID" FROM APZ_OHER WHERE "relaID">:Currnt AND "empID" = :EmpID ORDER BY "relaID" ASC;
	
END IF;
END IF;
END IF;
END IF;	
END
--apz_EmpRelatives_navigation 2,2,36




