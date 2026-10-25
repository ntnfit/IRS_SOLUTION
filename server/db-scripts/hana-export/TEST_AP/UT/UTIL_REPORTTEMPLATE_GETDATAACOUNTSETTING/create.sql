
CREATE PROCEDURE Util_ReportTemplate_GetDataAcountSetting
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
            select N'Account',			N'Account',						'Y',			100,		'string',		'N',		 'L',		'Y' from DUMMY
              	UNION ALL
			select N'BPCode',			N'BP name',						'Y',			100,		'string',		'N',		 'L',		'Y' from DUMMY
					     	UNION ALL
			select N'Dim1',			N'Dim 1',						'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'Dim2',			N'Dim 2',						'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					    	UNION ALL
			select N'Dim3',			N'Dim 3',						'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'Dim4',			N'Dim 4',						'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					    	UNION ALL
			select N'Dim5',			N'Dim 5',						'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'Project',		N'Project',						'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'RecipAccount',	N'Account (Offset)',			'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'RecipBP',		N'BP (Offset)',					'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'RecipDim1',		N'Dim 1 (Offset)',				'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'RecipDim2',		N'Dim 2 (Offset)',				'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'RecipDim3',		N'Dim 3 (Offset)',				'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'RecipDim4',		N'Dim 4 (Offset)',				'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'RecipDim5',		N'Dim 5 (Offset)',				'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'CalMth',			N'Phương pháp tính',			'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'RecipProject',	N'Project (Offset)',			'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY
					     	UNION ALL
			select N'PLTO',			N'Include Closing Balances',	'Y',			100,		'string',		'N',		 null,		'Y' from DUMMY;
					 
	SELECT * FROM #tblStructure;
END



