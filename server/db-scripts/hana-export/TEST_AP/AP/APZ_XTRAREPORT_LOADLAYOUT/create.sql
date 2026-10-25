
CREATE PROCEDURE Apz_XtraReport_LoadLayout(
	in userName 	NVARCHAR(50) default 0,
	in branch 		int default -1
)
as
begin
	 DECLARE tblValidValue TABLE(
		 "ColumnName" NVARCHAR(50) NOT NULL,
	     "Description" NVARCHAR(254) NOT NULL,
	     "Value" NVARCHAR(254) NOT NULL
     );
     DECLARE tblStructure TABLE
            (
             "ID" INT
            ,"ColumnName" NVARCHAR(50) NOT NULL
            ,"Caption" NVARCHAR(100)
            ,"DataType" VARCHAR(20) --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
            ,"SubType" VARCHAR(100)			--None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time
            ,"Width" INT
            ,"GroupIndex" INT	-- dùng cho grid  default -1
            ,"Visible" INT
            ,"FormatType" NVARCHAR(20)    -- numeric, datetime/ 
            ,"DisplayFormat" NVARCHAR(20)   -- '{0:n2}'
            ,"ReadOnly" INT
            ,"AllowEdit" INT
            ,"AllowMove" INT
            ,"AllowSize" INT
            ,"AllowSort" INT
            ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
			-- Summary Item
            ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
			-- Group Summary
            ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
            ,"Parent" INT NULL	-- dùng cho treelist
            ,"Key" INT NULL	-- dùng cho treelist (identity)
    );
    
    branch:=0;
    
	INSERT INTO :tblValidValue("ColumnName","Description","Value") VALUES('Status','Active','A');
	INSERT INTO :tblValidValue("ColumnName","Description","Value") VALUES('Status','Inactive','I');
     
	INSERT INTO :tblStructure("ID","ColumnName","Caption","DataType","Width","GroupIndex","Visible","ReadOnly","AllowEdit","AllowMove","AllowSize","AllowSort","FormatType","DisplayFormat","SummaryType","Fixed","SubType")
	select 1,	'ID',			'ID',			'int',			100,	-1, 0, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy union
    select 2,	'TypeCode',		'TypeCode',		'nvarchar',		100,	-1, 0, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy union
	select 3,	'TypeName',		'TypeName',		'nvarchar',		100,	-1, 0, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy union
	select 4,	'DocCode',		'DocCode',		'nvarchar',		75,		-1, 1, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy union
	select 5,	'DocName',		'DocName',		'nvarchar',		100,	-1, 1, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy union
	select 6,	'Menu',			'Menu',			'nvarchar',		150,	-1, 0, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy union
	select 7,	'CreateDate',	'CreateDate',	'datetime',		50,		-1, 0, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy union
	select 8,	'UpdateDate',	'UpdateDate',	'datetime',		50,		-1, 0, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy union
	select 9,	'UserSign',		'UserSign',		'nvarchar',		50,		-1, 0, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy union
	select 10,	'UserSign2',	'UserSign2',	'nvarchar',		50,		-1, 0, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy union
	select 11,	'Status',		'Status',		'ValidValue',	75,		-1, 1, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy union
	select 12,	'Description',	'Description',	'nvarchar',		60,		-1, 0, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy union
	select 13,	'Text',			'Text',			'nvarchar',		100,	-1, 0, 1, 0, 0, 1, 1, NULL, NULL, NULL, '', NULL from dummy;
	
	SELECT * FROM :tblStructure ORDER BY ID;
			
	SELECT * FROM APZ_RDOC WHERE LTRIM("DocCode") != '';

	SELECT * FROM :tblValidValue;
	
end

