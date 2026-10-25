
CREATE PROCEDURE Util_ReportTemplate_GetDataSetting
AS
BEGIN

	create local temporary table #tblStructure
		(
			"ColumnName" NVARCHAR(50) NOT NULL
			,"Caption" NVARCHAR(100) NULL
			,"Visible" NVARCHAR(1) NULL
			,"Width" int NULL
			,"DataType" NVARCHAR(50) NULL -- string, integer, double, datetime
			,"ROnly" NVARCHAR(1) NULL
			,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
			,"AllowEdit" VARCHAR(1) NULL DEFAULT 'Y'
		);

         INSERT INTO #tblStructure
						("ColumnName",		"Caption",			"Visible",		"Width",		"DataType",		"ROnly",		"Fixed",		"AllowEdit")
                select N'RepCode',			N'Report Code',		'Y',		100,		'string',		'N',		 'L',		'Y' from DUMMY
                	UNION ALL
				select N'RepName',		N'Report Name',		'Y',		100,		'string',		'N',		 'L',		'Y' from dummy
					UNION ALL
				select N'RepType',		N'Report Type',		'Y',		100,		'string',		'N',		 null,		'Y' from DUMMY;
					 
	SELECT * FROM #tblStructure;
END



