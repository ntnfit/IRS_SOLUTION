

CREATE PROCEDURE APZ_UserFields_GetListField (
	IN 	TableId NVARCHAR(50) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
READS SQL DATA
AS
BEGIN
	IF TableId <> '' THEN
		IF TableId LIKE 'APZ%' THEN
			SELECT "TableID", "FieldID", "AliasID", "Descr", "TypeID", "EditType", "SizeID", "EditSize", "Dflt", "NotNull", "IndexID", "RTable", "RField", "Action", "Sys", "DfltDate", "RelSO", "RelUDO", "ValidRule" 
				FROM APZ_CUFD 
				WHERE "TableID" = :TableId;
			SELECT "TableID", "FieldID", "IndexID", "FldValue", "Descr", "FldDate" 
				FROM APZ_UFD1 
				WHERE "TableID" = :TableId 
				ORDER BY "FieldID", "IndexID";
		ELSE
			SELECT "TableID", "FieldID", "AliasID", "Descr", "TypeID", "EditType", "SizeID", "EditSize", "Dflt", "NotNull", "IndexID", "RTable", "RField", "Action", "Sys", "DfltDate", "RelSO", "RelUDO", "ValidRule"				
				FROM CUFD 
				WHERE "TableID" = :TableId
					AND (('U_' || "AliasID" NOT IN ('U_ItemCode', 'U_ItemName', 'U_FrgnName', 'U_ItemCode', 'U_Quantity', 'U_IPercent', 'U_Price', 'U_PotentAmount', 'U_DeliveryDate', 'U_OpportId', 'U_line', 'U_AttFile') 
						AND "TableID"='@OPR62')
					OR "TableID"<>'@OPR62');
			SELECT "TableID", "FieldID", "IndexID", "FldValue", "Descr", "FldDate" 
				FROM UFD1 
				WHERE "TableID" = :TableId 
				ORDER BY "FieldID", "IndexID";			
		END IF;
	ELSE
		SELECT "TableID", "FieldID", "AliasID", "Descr", "EditType" 
			FROM CUFD;
	END IF;
END;




