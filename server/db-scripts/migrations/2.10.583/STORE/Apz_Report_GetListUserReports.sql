
CALL APZ_EXISTSPROC('Apz_Report_GetListUserReports');

GO

CREATE PROCEDURE Apz_Report_GetListUserReports (in UserName NVARCHAR(20) default '' ,in Branch NVARCHAR(20) default '')
AS
BEGIN
        create local temporary TABLE #tbl
            (
             "TypeCode" NVARCHAR(50)
            ,"TypeName" NVARCHAR(100)
            ,"DocCode" NVARCHAR(50)
            ,"DocName" NVARCHAR(254)
            ,"FileName" NVARCHAR(254)
            ,"StoreName" NVARCHAR(5000)
            ,"TableType" NVARCHAR(10)
			,"FatherId" INT
            );
		-- insert các báo cáo crystal
        INSERT INTO #tbl ("TypeCode", "TypeName", "DocCode", "DocName", "FileName")
            SELECT * FROM APZ_PRINTLAYOUT WHERE "TypeName" = N'User Reports' AND "FileName" IS NOT NULL AND "FileName" != '';

        -- insert các báo cáo form
        INSERT INTO #tbl ("TypeCode", "TypeName", "DocCode", "DocName", "FileName", "StoreName", "TableType", "FatherId")
            SELECT a."Code", 'Form Reports', a."Code", a."MenuCapt", a."ReportFile", a."StoreName", a."TYPE", b."Id"
                FROM APZ_OUDR a
				INNER JOIN APZ_OMNI b ON b."Code" = a."FatherMenu"
				WHERE a."MenuItem" = 'Y';
        
		-- insert các báo cáo form
        INSERT INTO #tbl ("TypeCode", "TypeName", "DocCode", "DocName", "FileName", "StoreName", "TableType", "FatherId")
            SELECT a."TypeCode", 'XtraReports', a."DocCode", a."DocName", a."Description", a."Text", '', b."Id"
                FROM APZ_RDOC a
				INNER JOIN APZ_OMNI b ON b."Code" = a."Menu"
				WHERE a."Status" = 'A' AND (a."DocCode" IS NULL OR a."DocCode" = '');

        SELECT * FROM #tbl;
        DROP TABLE #tbl;
  END