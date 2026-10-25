
CREATE PROCEDURE Apz_KPIScope_CFLByType(in typeName INT default 0)
AS
BEGIN
	IF :typeName = 1
	THEN
		SELECT *, b."ItmsGrpNam" "ItemGroup", 'Y' "Active"
		FROM OITM a
		LEFT JOIN OITB b ON b."ItmsGrpCod" = a."ItmsGrpCod";
	ELSEIF :typeName = 2
	THEN
		SELECT "ItmsGrpCod" "Code", "ItmsGrpNam" "Name", * FROM OITB WHERE "Locked" = 'N';
	ELSEIF :typeName = 3
	THEN
		SELECT *, "CardName" "Name" FROM OCRD WHERE "validFor" = 'Y';
	ELSEIF :typeName = 4
	THEN
		SELECT * FROM OCRG WHERE "GroupType" = 'C';
	ELSEIF :typeName = 5
	THEN
		SELECT *,  ifnull("firstName",'') || ifnull("middleName",'') || ifnull("lastName", '') "EmpName" FROM OHEM WHERE "Active" = 'Y';
	ELSEIF :typeName = 6
	THEN
		SELECT * FROM OUDP;
	END IF;
END




