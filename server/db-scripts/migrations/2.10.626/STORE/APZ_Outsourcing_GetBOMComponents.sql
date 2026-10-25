
CALL APZ_EXISTSPROC('APZ_Outsourcing_GetBOMComponents');

GO

CREATE PROCEDURE APZ_Outsourcing_GetBOMComponents
(
	in itemCode nvarchar(50) default '',
	in qty numeric(19,6) default 0
)
AS
BEGIN
	DECLARE docQty decimal(19, 6);
	Select IFNULL((SELECT "Qauntity" FROM OITT WHERE "Code" = :itemCode), 0) Into docQty From Dummy;
	
	SELECT 0 AS "LineNum", a."Code" AS "ItemCode", b."ItemName", a."Warehouse" AS "WhsCode", (a."Quantity" * :qty) / :docQty AS "Quantity", 0 AS "DocEntry", 
		CASE b."ManBtchNum" 
			WHEN 'Y' THEN 'B' 
			ELSE (CASE b."ManSerNum" WHEN 'Y' THEN 'S' ELSE 'N' END) END AS "ManBatchSer", 
		CAST(:itemCode as nvarchar) AS "Father", a."ChildNum" 
	FROM ITT1 a 
		INNER JOIN OITM b ON b."ItemCode" = a."Code" 
	WHERE "Father" = :itemCode;
	
END