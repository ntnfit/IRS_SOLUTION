
CALL APZ_EXISTSPROC('APZ_Capacity_GetColorSetting');

GO

CREATE PROCEDURE APZ_Capacity_GetColorSetting
AS
BEGIN
	SELECT "OpeCode" AS "ActionId", "OpeName" AS "ActionName", IFNULL("Color", -4128769) AS "Color" FROM APZ_OOMD WHERE "Status" = 'A'
	UNION ALL 
	SELECT 'ApzOverLoad', 'Over load', -47872 FROM Dummy
	UNION ALL
	SELECT 'ApzNormal', 'Normal', -13447886 FROM Dummy
	UNION ALL
	SELECT 'ApzProd', 'Production', -5658199 FROM Dummy;
END