
CREATE procedure APZ_CFl_GetDataBin(
	in WhsCode VARCHAR(50) default '',
	in itemCode nvarchar(50) default '',
	tranType int default 0
)
AS
BEGIN	
	-- Khai báo bảng Structure
	declare tblStructure table
    (
		 "ColumnName" NVARCHAR(50) NOT NULL
		,"Caption" NVARCHAR(100) NULL
		,"DataType" VARCHAR(20) NULL --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
		,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
		,"Width" INT NULL
		,"GroupIndex" INT NULL -- dùng cho grid  default -1
		,"Visible" tinyint NULL
		,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
		,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
		,"ReadOnly" tinyint NULL
		,"AllowEdit" tinyint NULL
		,"AllowMove" tinyint NULL
		,"AllowSize" tinyint NULL
		,"AllowSort" tinyint NULL
		,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
		-- Summary Item
		,"SummaryType" VARCHAR(10) NULL	-- sum,average...
		-- Group Summary
		,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
		--,Parent BIT NULL	-- dùng cho treelist
		--,[Key] BIT NULL		-- dùng cho treelist (identity)
    );
    
    INSERT INTO :tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType") 
    select n'AbsEntry', n'BinAbs', 'int', 75, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from DUMMY
	union all
	select n'BinCode', n'Bin Code', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from dummy
	union all
	select n'WhsCode', n'Whs Code', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from dummy;
	
	SELECT * FROM :tblStructure;
	
	if :tranType = 1 then
		select a.*, b."OnHandQty" "AvailQty"
		from OBIN a join OIBQ b on a."AbsEntry"=b."BinAbs"
		where b."ItemCode"=:itemCode and b."WhsCode"=:whsCode and b."OnHandQty">0;
	else
		select a.*, b."OnHandQty" "AvailQty"
		from OBIN a left join OIBQ b on b."ItemCode"=:itemCode and a."AbsEntry"=b."BinAbs"
		where a."WhsCode"=:whsCode;
	end if;
END

