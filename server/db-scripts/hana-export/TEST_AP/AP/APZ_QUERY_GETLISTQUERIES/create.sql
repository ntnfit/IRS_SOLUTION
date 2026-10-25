
CREATE  PROCEDURE Apz_Query_GetListQueries(in UserName NVARCHAR(20) default '',	in Branch NVARCHAR(20) default '')
AS
	BEGIN
        -- Khai báo bảng Structure
		declare tblStructure table
			(
			 "ColumnName" NVARCHAR(50) NOT NULL
			,"Caption" NVARCHAR(100) NULL
			,"DataType" VARCHAR(50) NULL --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox) LinkedUDO* cfl(choosefromlist)
			,"SubType" VARCHAR(100) NULL	-- None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time (udo code : dùng cho LinkedUDO)*	 
                                        -- type of cfl - số của type hoặc tên store(storename @value, @userName, @branch) lấy dữ liệu cfl-store phải có structure
			,"Width" INT NULL
			,"GroupIndex" INT NULL	-- dùng cho grid  default -1
			,"Visible" smallint NULL
			,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
			,"DisplayFormat" NVARCHAR(20) NULL  -- '{0:n2}'
			,"ReadOnly" smallint NULL
			,"AllowEdit" smallint NULL
			,"AllowMove" smallint NULL
			,"AllowSize" smallint NULL
			,"AllowSort" smallint NULL
			,"Fixed" VARCHAR(1) NULL	-- L:Left / R:Right
			-- Summary Item
			,"SummaryType" VARCHAR(10) NULL	-- sum,average...
			-- Group Summary
			,"GroupSummaryType" VARCHAR(10) NULL	-- sum,average...
			--,Parent BIT NULL	-- dùng cho treelist
			--,[Key] BIT NULL	-- dùng cho treelist (identity)
			);
		INSERT INTO :tblStructure ("ColumnName", "Caption", "DataType", "SubType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove",
								   "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
			select N'IntrnalKey', N'IntrnalKey', 'int', '', 75, -1, 0, N'', N'', NULL, NULL, NULL, NULL, NULL, '', '', '' from DUMMY
				union all
			select N'QCategory', N'QCategory', 'int', '', 75, -1, 0, N'', N'', NULL, NULL, NULL, NULL, NULL, '', '', '' from DUMMY
				union all
			select N'CatName', N'Category', 'nvarchar', '', 75, 0, 1, N'', N'', NULL, NULL, NULL, NULL, NULL, '', '', '' from DUMMY
				union all
			select N'QName', N'QName', 'nvarchar', '', 200, -1, 1, N'', N'', NULL, NULL, NULL, NULL, NULL, '', '', '' from DUMMY;

		SELECT * FROM :tblStructure;
		
		tbl = select substring("menucode", 20) GroupId
		from OPRM where "menucode" like 'barSub_QueryAuthGrp%' and USER_CODE=:UserName and "authoriza"='F';
		
		SELECT a."IntrnalKey", b."CategoryId" AS "QCategory", b."CatName", CASE WHEN a."QName" IS NULL THEN 'No queries belong to this group'
														   ELSE a."QName"
													  END AS "QName", CASE WHEN a."QString" IS NULL THEN cast('' as nvarchar)
																		 ELSE cast(a."QString" as nvarchar)
																	END AS "QString", a."QType", b."PermMask"
			FROM OUQR a
			RIGHT JOIN OQCN b ON a."QCategory" = b."CategoryId"
			where b."CategoryId" in (select a.CATEGORYID from QAG1 a join :tbl b on a.AUTHGRPID = b.GroupId
														      join OQAG c on c.AUTHGRPID = b.GroupId where c."Deleted" = 'N')
			ORDER BY b."CategoryId", a."QName";
			 
	END

