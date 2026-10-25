
CREATE PROCEDURE Apz_InventoryTransfer_GetItemByBarcode (
	IN 	barcode NVARCHAR(5000)
,		type NVARCHAR(1) DEFAULT 'N'
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	CREATE LOCAL TEMPORARY COLUMN TABLE #tblStructure (
		 "ID" INT
		,"ColumnName" NVARCHAR(50) NOT NULL
		,"Caption" NVARCHAR(100)
		,"DataType" NVARCHAR(50) DEFAULT 'nvarchar'	--int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox) LinkedTable LinkedUDO* LinkedObject cfl(choosefromlist) LinkedCflObject
	                                        		-- color, validcolor (combobox+color),	linkedStore, LinkedCrystal
		,"Width" INT DEFAULT 75
		,"FormatType" NVARCHAR(20)					-- numeric, datetime/ 
		,"DisplayFormat" NVARCHAR(20) NULL			-- '{0:n2}'
		,"Expression" NVARCHAR(5000)
	);	
	 
	INSERT INTO #tblStructure  ("ID", "ColumnName", "Caption", "DataType", "Width", "FormatType", "DisplayFormat", "Expression")
  		SELECT   		1, N'DistNumber',N'DistNumber',	'nvarchar',		75,		N'', N'', '' FROM DUMMY
		UNION SELECT	2, N'WhsCode',	N'WhsCode',		'nvarchar',		40,		N'', N'', '' FROM DUMMY				
		UNION SELECT	3, N'BinCode',	N'BinCode',		'nvarchar',		80,		N'', N'', '' FROM DUMMY		
		UNION SELECT	4, N'ItemCode',	N'ItemCode',	'nvarchar',		75,		N'', N'', '' FROM DUMMY		
		UNION SELECT	5, N'MnfSerial',N'MnfSerial',	'nvarchar',		0,		N'', N'', '' FROM DUMMY		
		UNION SELECT	6, N'LotNumber',N'LotNumber',	'nvarchar',		0,		N'', N'', '' FROM DUMMY		
		UNION SELECT	7, N'Quantity',	N'Quantity',	'numeric',		75,		N'', N'', '' FROM DUMMY		
		UNION SELECT	8, N'BinAbs',	N'BinAbs',		'nvarchar',		0,		N'', N'', '' FROM DUMMY		
		UNION SELECT	9, N'SysNumber',N'SysNumber',	'int',			0,		N'', N'', '' FROM DUMMY		
		UNION SELECT	10,N'ObjType',	N'ObjType',		'nvarchar',		0,		N'', N'', '' FROM DUMMY;	
				
	SELECT * FROM #tblStructure ORDER BY ID;
	
	SELECT a."DistNumber", a."WhsCode", a."BinCode", a."ItemCode", a."MnfSerial", a."LotNumber", a."Instock" AS "Quantity", a."BinAbs", a."SysNumber", CASE WHEN a."ManBtchNum" = 'Y' THEN '10000044' ELSE '10000045' END AS "ObjType" 
		FROM APZ_OITW_LOCATION a 
		WHERE "DistNumber" = :barcode;
	
	DROP TABLE #tblStructure;
	
END;




