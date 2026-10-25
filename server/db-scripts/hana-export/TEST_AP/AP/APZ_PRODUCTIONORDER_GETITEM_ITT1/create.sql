
create procedure Apz_ProductionOrder_GetItem_ITT1(in Father nvarchar(50) default '')
As
Begin
	SELECT 0 AS "DocEntry", 
	a."ChildNum" AS "LineNum",
	a."Code" AS "ItemCode", 
	a."Quantity" / b."Qauntity" AS "BaseQty", 
	a."Quantity" / b."Qauntity" AS "PlannedQty",
	a."Quantity" "OldLineQty",
	b."Qauntity" "OldFatherQty",
	0 AS "IssuedQty", a."IssueMthd" AS "IssueType",
	IFNULL(a."Warehouse", '') AS "wareHouse", 
	0 AS "VisOrder", '' AS "WipActCode", 
	0 AS "CompTotal", a."OcrCode" AS "OcrCode", 
	a."OcrCode2" AS "OcrCode2", 
	a."OcrCode3" AS "OcrCode3", 
	a."OcrCode4" AS "OcrCode4",
	a."OcrCode5" AS "OcrCode5", 0 AS "LocCode", 
	IFNULL(a."LogInstanc", 0) AS "LogInstanc", 
	0 AS "LoadFrBOM",
	a."Project" AS "Project",
	0 AS "UomEntry", 
	'' AS "UomCode", 
	a."Type" AS "ItemType", 
	0 AS "AdditQty", 
	'' AS "LineText", 
	'N' AS "PickStatus", 
	0 AS "PickQty", 
	0 AS "PickIdNo", 
	0 AS "ReleaseQty", 
	'' AS "ResAlloc", 
	IFNULL(CASE a."Type" WHEN 4 THEN c."ItemName" WHEN 290 THEN e."ResName" END, '') AS "ItemName",
	IFNULL(c."InvntryUom", '') AS "InvntryUom", 
	IFNULL(c."OnHand", 0) AS "OnHand", 
	IFNULL(c."OnOrder", 0) AS "OnOrder1", 
	IFNULL(d."IsCommited", 0) AS "IsCommited",
	IFNULL(d."OnOrder", 0) AS "OnOrder", 
	IFNULL(d."OnHand",0) + IFNULL(d."OnOrder",0) - IFNULL(d."IsCommited",0) AS "Available", 
	CASE WHEN c."ManBtchNum" = 'Y' THEN 'B' WHEN c."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBtchSerial"
	FROM itt1 a 
	LEFT OUTER JOIN oitt b ON b."Code" = a."Father" LEFT OUTER JOIN OITM c ON c."ItemCode" = a."Code" 
	LEFT OUTER JOIN OITW d ON d."ItemCode" = c."ItemCode" AND a."Warehouse" = d."WhsCode"
	LEFT OUTER JOIN orsc e ON e."ResCode" = a."Code" WHERE b."Code" = :Father
	ORDER BY a."VisOrder";

End;


