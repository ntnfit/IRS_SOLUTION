CREATE PROCEDURE APZ_CFL_GetPreDecisionTask
(
IN clgCode INT DEFAULT 0
)
LANGUAGE SQLSCRIPT
AS
BEGIN
	Declare flag TinyInt = 0;
	Declare Temp Int = 1;
	Declare TempCount Int = 0;
	--Khai báo bảng Structure
	CREATE LOCAL TEMPORARY TABLE #tblStructure
	(
		"ColumnName" NVARCHAR(50) NOT NULL,
		"Caption" NVARCHAR(100) NULL,
		"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar', --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
		"SubType" VARCHAR(20) NULL,			--none phone percentage address image measurement price quantity rate sum time
        "Width" INT NULL DEFAULT 75,
        "GroupIndex" INT NULL DEFAULT -1,	-- dùng cho grid  default -1
        "Visible" TINYINT NULL DEFAULT 1,
        "FormatType" NVARCHAR(20) NULL,   -- numeric, datetime/ 
        "DisplayFormat" NVARCHAR(20) NULL,  -- '{0:n2}'
        "ReadOnly" TINYINT NULL DEFAULT 0,
        "AllowEdit" TINYINT NULL DEFAULT 1,
        "AllowMove" TINYINT NULL DEFAULT 1,
        "AllowSize" TINYINT NULL DEFAULT 1,
        "AllowSort" TINYINT NULL DEFAULT 1,
        "Fixed" VARCHAR(1) NULL,	-- L:Left / R:Right
		-- Summary Item
        "SummaryType" VARCHAR(10) NULL,	-- sum,average...
		-- Group Summary
        "GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
		--,Parent BIT NULL	-- dùng cho treelist
		--,[Key] BIT NULL		-- dùng cho treelist (identity)
	);
	
	INSERT INTO #tblStructure 
	("ColumnName",	"Caption", "DataType", "Width",	"GroupIndex", "Visible","FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
    VALUES (N'ClgCode',N'Task Code','int',	75,	-1,	1,NULL,NULL,1,0,1,1,1,'L',NULL,NULL);
	INSERT INTO #tblStructure 
	("ColumnName",	"Caption", "DataType", "Width",	"GroupIndex", "Visible","FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")		
	VALUES (N'Details',N'Details','nvarchar',150,-1,1,NULL,NULL,1,0,1,1,1,'L',NULL,NULL);
	
	SELECT * FROM #tblStructure;
	IF (:clgCode = null)
	THEN
		clgCode := 0;
	End If;
	
	CREATE LOCAL TEMPORARY TABLE #tbl as (SELECT Top 0 * FROM "APZ_OCLG");
	INSERT INTO #tbl SELECT * FROM "APZ_OCLG" Where "parentTask" = :clgCode;
	--Recursive
	CREATE LOCAL TEMPORARY TABLE #tblSub ("ClgCode" INT,"parentTask" INT, "ID" INT GENERATED ALWAYS AS IDENTITY (START WITH 1 INCREMENT BY 1));
	Temp := 1;
	Select Count(*) into TempCount from #tblSub;
	while Temp <= :TempCount
	do
		INSERT INTO #tblSub 
		Select "ClgCode" ,"parentTask" from #tbl
		union all 
		Select "t0"."ClgCode", "t0"."parentTask" 
		From #tbl "t0" inner join #tblSub "t1" on "t0"."ClgCode" = IFNULL("t1"."parentTask", 0)
		Where "ID"  = :Temp;
		
		Temp = :Temp + 1;
	end while;
	
	select * from "APZ_OCLG" where "ClgCode" not in (Select "ClgCode" from #tblSub) and "ClgCode"<>:clgCode;
END;




