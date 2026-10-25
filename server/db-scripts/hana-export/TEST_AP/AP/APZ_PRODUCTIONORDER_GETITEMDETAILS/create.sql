
create procedure Apz_ProductionOrder_GetItemDetails(in Code NVARCHAR(50),in Type NVARCHAR(1))
As
Begin
	IF :Type = 'I' 
	THEN 
		SELECT "ItemName", "InvntryUom", a."OnHand", a."OnOrder", 
		"DfltWH" AS "DefaultWarehouse", a."IsCommited",
		 t1."OnHand" AS "OnWhs", t1."IsCommited" AS "IsCommitedInWhs",
		  t1."OnOrder" AS "OnOrderInWhs", a."DfltWH", a."IssueMthd",
		   CASE WHEN a."ManBtchNum" = 'Y' THEN 'B' WHEN a."ManSerNum" = 'Y' THEN 'S' ELSE 'N' END AS "ManBatchSerial"
	    FROM OITM a LEFT OUTER JOIN (SELECT "ItemCode", "WhsCode", "OnHand", "IsCommited", "OnOrder" 
	    FROM OITW WHERE "ItemCode" = :Code) AS t1 ON a."DfltWH" = t1."WhsCode" WHERE a."ItemCode" = :Code;
	ELSEIF :Type = 'R' 
	THEN 
		SELECT "ResName", "UnitOfMsr" AS "InvntryUom", 0 AS "OnHand", 0 AS "OnOrder", "DfltWH" AS "DefaultWarehouse", "IssueMthd" FROM ORSC WHERE "ResCode" = :Code;
	END IF;
End;





