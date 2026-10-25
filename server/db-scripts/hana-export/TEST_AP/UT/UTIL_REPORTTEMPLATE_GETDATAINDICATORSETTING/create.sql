
CREATE PROCEDURE Util_ReportTemplate_GetDataIndicatorSetting
AS
BEGIN

	Create local temporary table #tblStructure
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
                select N'IndCode',		N'Code',				'Y',		100,		'string',		'N',		 'L',		'Y' FROM DUMMY
                	UNION ALL
				SELECT N'IndName',		N'Name',				'Y',		100,		'string',		'N',		 'L',		'Y' from DUMMY
					UNION ALL
				SELECT N'FathCode',		N'Father Code',			'Y',		100,		'string',		'N',		 null,		'Y' FROM DUMMY
					UNION ALL
				SELECT N'Caculation',		N'Type',				'Y',		100,		'string',		'N',		 null,		'Y' FROM DUMMY
					UNION ALL
				select N'Formula',		N'Formula',				'Y',		100,		'string',		'N',		 null,		'Y' from dummy
					UNION ALL
				select N'Detail',			N'Detail',				'Y',		100,		'string',		'N',		 null,		'Y' from dummy
					   	UNION ALL
				select N'Level',			N'Level',				'Y',		100,		'string',		'N',		 null,		'Y' from dummy
					   	UNION ALL
				select N'Font',			N'Font',				'Y',		100,		'string',		'N',		 null,		'Y' from dummy
					   	UNION ALL
				select N'Type',			N'Style',				'Y',		100,		'string',		'N',		 null,		'Y' from dummy
					   	UNION ALL
				select N'UnderLine',		N'UnderLine',			'Y',		100,		'string',		'N',		 null,		'Y' from dummy
					   	UNION ALL
				select N'Size',			N'Font Size',			'Y',		100,		'integer',		'N',		 null,		'Y' from dummy
					   	UNION ALL
				select N'Color1',			N'Color',				'Y',		100,		'string',		'N',		 null,		'Y' from dummy
					   	UNION ALL
				select N'Display',		N'Display',				'Y',		100,		'string',		'N',		 null,		'Y' from dummy
					   	UNION ALL
				select N'Color',			N'Color',				'N',		100,		'string',		'N',		 null,		'Y' from dummy
					   	UNION ALL
				select N'Comments',		N'Comments',			'Y',		100,		'Comments',		'N',		 null,		'Y' from dummy
					   	UNION ALL
				select N'Code',			N'Code',				'Y',		100,		'Code',			'N',		 null,		'Y' from dummy;
					 
	SELECT * FROM #tblStructure;
END



