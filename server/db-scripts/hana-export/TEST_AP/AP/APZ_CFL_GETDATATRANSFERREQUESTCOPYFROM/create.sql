
CREATE PROCEDURE APZ_CFL_GetDataTransferRequestCopyFrom(in fromWhsCode nvarchar(8) default null, in toWhsCode nvarchar(8) default null)
AS
BEGIN
	declare fromDate timestamp;-- =  (select DATEADD(DAY,-1,GetDate()))
	declare toDate timestamp;-- =  (select DATEADD(DAY,1,GetDate()))
	declare sqlstring nvarchar(500000); 

	select ADD_DAYS(CURRENT_DATE, -1) into fromDate from DUMMY;
	select ADD_DAYS(CURRENT_DATE, 1) into toDate from DUMMY;
		-- Khai báo bảng Structure
		create local temporary table #tblStructure
        (
             "ColumnName" NVARCHAR(50) NOT NULL
            ,"Caption" NVARCHAR(100) NULL
            ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
			,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
            ,"Width" INT NULL DEFAULT 75
            ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
            ,"Visible" smallint NULL DEFAULT 1
            ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
            ,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
            ,"ReadOnly" smallint NULL DEFAULT 0
            ,"AllowEdit" smallint NULL DEFAULT 1
            ,"AllowMove" smallint NULL DEFAULT 1
            ,"AllowSize" smallint NULL DEFAULT 1
            ,"AllowSort" smallint NULL DEFAULT 1
            ,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
			-- Summary Item
            ,"SummaryType" VARCHAR(10) NULL	-- sum,average...
			-- Group Summary
            ,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
			--,Parent BIT NULL	-- dùng cho treelist
			--,[Key] BIT NULL		-- dùng cho treelist (identity)
        );
        
        INSERT INTO #tblStructure ("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType") 
	        select n'DocNum', n'DocNum', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from DUMMY
	        	UNION ALL	
			select n'CardCode', n'CardCode', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL FROM DUMMY
				UNION ALL
			SELECT n'CardName', n'CardName', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL FROM DUMMY
				UNION ALL
			SELECT n'Filler', n'From Warehouse', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL FROM DUMMY
				UNION ALL
			select n'ToWhsCode', n'To Warehouse', 'nvarchar', 150, -1, 1, NULL, NULL, 1, 0, 1, 1, 1, 'L', NULL, NULL from dummy;			

		SELECT * FROM #tblStructure;
			
		select * from OWTQ where 
			"DocEntry" in (select a."DocEntry" from WTQ1 a inner join OITM b on a."ItemCode" = b."ItemCode" where ifnull(a."ShipDate",'') <> '' and ifnull(a."OpenQty", 0) > 0 and a."LineStatus" = 'O' and 
					((b."validFor" = 'Y' and ifnull(b."frozenFor",'N') = 'N' and (ifnull(b."validFrom", :fromDate) <= CURRENT_DATE and ifnull(b."validTo", :toDate) >= CURRENT_DATE)) 
					OR (ifnull(b."validFor",'N') = 'N' and b."frozenFor" = 'Y' and (ifnull(b."frozenFrom", :fromDate) > CURRENT_DATE or ifnull(b."frozenTo", :toDate) < CURRENT_DATE))
					or (b."validFor" = 'Y' and b."frozenFor" = 'Y' and (ifnull(b."validFrom", :fromDate) <= CURRENT_DATE and ifnull(b."validTo", :toDate) >= CURRENT_DATE) and
					(ifnull(b."frozenFrom", :fromDate) > CURRENT_DATE or ifnull(b."frozenTo", :toDate) < CURRENT_DATE))
					OR (b."validFor" = 'N' AND b."frozenFor" = 'N')) group by a."DocEntry") and "DocStatus" = 'O' and (ifnull(:fromWhsCode, '') = '' or "Filler" = :fromWhsCode) --and (isnull(@toWhsCode, '') = '' or ToWhsCode = @toWhsCode)
					order by "DocEntry" DESC;
END



