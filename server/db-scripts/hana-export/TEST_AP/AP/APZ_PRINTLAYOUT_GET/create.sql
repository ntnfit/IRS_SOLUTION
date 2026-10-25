
CREATE PROCEDURE Apz_Printlayout_Get (
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER
AS
BEGIN
	CREATE LOCAL TEMPORARY COLUMN TABLE #tbl (
	  "TypeCode" nvarchar(50)
  	, "TypeName" nvarchar(254)
  	, "DocCode" nvarchar(50)
  	, "DocName" nvarchar(254)
  	, "FileName" nvarchar(254)
  	, "FilePath" nvarchar(254) DEFAULT ''
  );

	INSERT INTO #tbl("TypeCode", "TypeName", "DocCode", "DocName", "FileName") 
		SELECT "TypeCode", "TypeName", "DocCode", "DocName", "FileName" 
		FROM APZ_PRINTLAYOUT;
	
	INSERT INTO #tbl("TypeCode", "TypeName", "DocCode", "DocName", "FileName") 
		SELECT b.CODE, b.NAME, a."DocCode", a."DocName", '' 
		FROM RDOC a 
		LEFT OUTER JOIN RTYP b ON a."TypeCode" = b.CODE 
		WHERE b.CODE <> 'RCRI' 
			AND NOT EXISTS (SELECT 1 FROM #tbl WHERE a."TypeCode" = "TypeCode" AND a."DocCode" = "DocCode");
	
	SELECT * FROM #tbl ORDER BY "DocCode";
END;




