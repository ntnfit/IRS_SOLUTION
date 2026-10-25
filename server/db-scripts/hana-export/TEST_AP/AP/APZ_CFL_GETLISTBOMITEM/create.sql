
create procedure APZ_CFL_GETLISTBOMITEM(in Code nvarchar(50))

as
begin
	
	declare fromDate date;
	declare toDate date;
	select add_days(current_date, -1) into fromDate from dummy;
	select add_days(current_date, 1) into toDate from dummy; 

	create local temporary table #tblStructure
            (
             "ColumnName" NVARCHAR(50) NOT NULL
            ,"Caption" NVARCHAR(100) NULL
            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar'  --int smallint decimal money float datetime date numeric
            ,"Width" INT NULL DEFAULT 75
            ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid-  default -1
            ,"Visible" tinyint NULL DEFAULT 1
            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime
            ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
            ,"ReadOnly" tinyint NULL DEFAULT 0
            ,"AllowEdit" tinyint NULL DEFAULT 1
            ,"AllowMove" tinyint NULL DEFAULT 1
            ,"AllowSize" tinyint NULL DEFAULT 1
            ,"AllowSort" tinyint NULL DEFAULT 1
            ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
			-- Summary Item
            ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
			-- Group Summary
            ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
            );
            
            INSERT INTO #tblStructure 
						("ColumnName",			"Caption",				"DataType",		"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
			select N'ItemCode',			N'item Code',			'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from dummy
			union select N'ItemDescription',			N'item Name',			'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		 'L',	NULL,		 NULL from dummy;
			
			select "ItemCode","ItemName" "ItemDescription","DfltWH" "WarehouseCode","AvgPrice" "Price","IssueMthd" from OITM  
			where "ItemCode" <> :Code and  (("validFor" = 'Y' and ifnull("frozenFor",'N') = 'N' and (ifnull("validFrom", :fromDate) <= current_date and ifnull("validTo", :toDate) >= current_date)) 
			OR (ifnull("validFor",'N') = 'N' and "frozenFor" = 'Y' and (ifnull("frozenFrom", :fromDate) > current_date or ifnull("frozenTo", :toDate) < current_date))
			or ("validFor" = 'Y' and "frozenFor" = 'Y' and (ifnull("validFrom", :fromDate) <= current_date and ifnull("validTo", :toDate) >= current_date and
					(ifnull("frozenFrom", :fromDate) > current_date or ifnull("frozenTo", :toDate) < current_date))
			OR ("validFor" = 'N' AND "frozenFor" = 'N')) and 
			"ItemCode" not in  (select "FGCode" from GetFatherCodeByItem(:Code)));
					 
end;




