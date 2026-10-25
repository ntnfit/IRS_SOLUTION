
CALL APZ_EXISTSPROC('Apz_UDO_GetData');

GO


CREATE PROCEDURE Apz_UDO_GetData (
	IN 	udoCode NVARCHAR(50)
,		tableName NVARCHAR(100)
,		docEntry INT DEFAULT -1
,		fromDate TIMESTAMP DEFAULT '1900-01-01'
,		toDate TIMESTAMP DEFAULT '1900-01-01'
,		username NVARCHAR(100) DEFAULT ''
,		branch NVARCHAR(2) DEFAULT '-1'
,		code NVARCHAR(50) DEFAULT ''
)
LANGUAGE SQLSCRIPT
SQL SECURITY INVOKER

AS
BEGIN
	-- khai báo các biến
	DECLARE udoType CHAR(1) := '3';
	DECLARE sonNum INT := 0;
	DECLARE isExistSonNum INT := 0;
	DECLARE colNames NVARCHAR(5000);
	DECLARE sqlString NVARCHAR(5000);
	DECLARE strFromDate NVARCHAR(10);
	DECLARE strToDate NVARCHAR(10);
	DECLARE seriesQuery nvarchar(500);
	DECLARE origUser nvarchar(50) = :userName;
	
	-- Khai báo bảng dữ liệu combobox (tùy chọn)
	DECLARE tblValidValue TABLE (
		 "ColumnName" NVARCHAR(50) NOT NULL
		 ,"Value" NVARCHAR(254) NOT NULL
		 ,"Description" NVARCHAR(254) NOT NULL
		 ,"BackColor" NVARCHAR(20)
		 ,"ForeColor" NVARCHAR(20)
	 );
	 		
	DECLARE tblStructure TABLE (
		 "ID" INT
		,"ColumnName" NVARCHAR(50) NOT NULL
		,"Caption" NVARCHAR(100)
		,"DataType" NVARCHAR(50) --DEFAULT 'nvarchar'	--int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox) LinkedTable LinkedUDO* LinkedObject cfl(choosefromlist) LinkedCflObject
	                                        		-- color, validcolor (combobox+color),	linkedStore, LinkedCrystal
		,"SubType" NVARCHAR(500)					-- None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time, UDT dùng cho LinkedTable, (udo code : dùng cho LinkedUDO)*	 , ObjectType(số): dùng cho LinkedObject
	                            					-- type of cfl - số của type hoặc tên store(storename @value, @userName, @branch) lấy dữ liệu cfl-store phải có structure, sub - sub form
		,"Width" INT --DEFAULT 75
		,"GroupIndex" INT --DEFAULT -1				-- dùng cho grid  default -1
		,"Visible" TINYINT --DEFAULT 1
		,"FormatType" NVARCHAR(20)					-- numeric, datetime/ 
		,"DisplayFormat" NVARCHAR(20) NULL			-- '{0:n2}'
		,"FilterString" NVARCHAR(5000)
		,"ReadOnly" TINYINT --DEFAULT 0
		,"AllowEdit" TINYINT --DEFAULT 1
		,"AllowMove" TINYINT --DEFAULT 1
		,"AllowSize" TINYINT --DEFAULT 1
		,"AllowSort" TINYINT --DEFAULT 1
		,"Fixed" VARCHAR(1)							-- L:Left / R:Right
		,"BackColor" NVARCHAR(20)					-- color column //list color https://msdn.microsoft.com/en-us/library/aa358802(v=vs.85).aspx
		,"ForeColor" NVARCHAR(20)
		,"SummaryType" NVARCHAR(10)					-- sum,average...
		,"GroupSummaryType" NVARCHAR(10)
		,"GroupSummaryShowInFooter" TINYINT
		,"SummaryFormat" NVARCHAR(20)				-- (AVR={0::.::})/ Count ({0})/ Custom format/ Max (MAX={0})/ Min (MIN={0})/ Sum (SUM={0::.::})
		,"Expression" NVARCHAR(5000)
		,"Parent" TINYINT							-- dùng cho treelist
		,"Key" TINYINT
		,"PivotArea" NVARCHAR(20)
		);
		
	IF :docentry = 0
	THEN	
		docentry = -1;	
	END IF;
	
	INSERT INTO :tblValidValue  ("ColumnName", "Description", "Value")
		SELECT 			N'Status',N'Open' ,		N'O' FROM DUMMY
		UNION SELECT 	N'Status',N'Closed' ,	N'L' FROM DUMMY	
		UNION SELECT 	N'Status',N'Canceled' ,	N'C' FROM DUMMY;
		
	-- lấy ra type của UDO 1: master data; 3: document
	SELECT "TYPE" INTO udoType FROM OUDO WHERE "Code" = :udoCode;
	-- lấy ra formsetting nếu có
	IF NOT EXISTS(SELECT 1 FROM APZ_FORMSETTING WHERE "UserSign" = :username AND "FormId" = :udoCode AND "ItemId" = :tableName) THEN
		username := 'manager';
	END IF;
	-- lấy ra sonnum của table
	SELECT COUNT(*) INTO isExistSonNum FROM UDO1 WHERE "TableName" = (REPLACE (:tableName, '@', ''));
	IF isExistSonNum > 0 THEN
		SELECT "SonNum" INTO sonNum FROM UDO1 WHERE "TableName" = (REPLACE (:tableName, '@', ''));
	END IF;	
	
	-- Insert structure
	IF sonNum = 0 THEN		-- là bảng cha
		INSERT INTO :tblStructure (ID, "ColumnName", "Caption", "DataType", "Width", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort") 
  			SELECT row_number() over(ORDER BY IFNULL(d."VisIndex", a."ColumnNum")), "ColAlias", "ColDesc", CASE b.DATA_TYPE_NAME WHEN 'TIMESTAMP' THEN 'DATETIME' WHEN 'DECIMAL' THEN 'NUMERIC' ELSE b.DATA_TYPE_NAME END, 75, 1, '', '', CASE WHEN a."ColEdit" = 'Y' THEN 0 ELSE 1 END, CASE WHEN a."ColEdit" = 'Y' THEN 1 ELSE 0 END, 1, 1, 1 
			FROM UDO3 a 
			INNER JOIN TABLE_COLUMNS b ON b.COLUMN_NAME = a."ColAlias" 
			LEFT JOIN APZ_FORMSETTING d ON d."FormId" = :udoCode AND d."ItemId" = :tableName AND d."UserSign" = :username AND d."ColumnId" = a."ColAlias" 
			WHERE SCHEMA_NAME = CURRENT_SCHEMA 
				AND b.TABLE_NAME = :tableName 
				AND a."SonNum" = 0 
				AND a."Code" = :udoCode 
				ORDER BY IFNULL(d."VisIndex", a."ColumnNum");
				
		-- kiểm tra nếu không có cột DocEntry thì tự lấy lên, mặc định không đc sua
		IF (SELECT COUNT(1) FROM :tblStructure WHERE "ColumnName" = 'DocEntry') = 0 THEN
			INSERT INTO :tblStructure ("ColumnName", "Caption", "DataType", "Width", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort")
				VALUES ('DocEntry', 'DocEntry', 'int', 75, 0, '', '', 1, 1, 1, 1, 1 );
		END IF;
		
	ELSE		-- là bảng con
		INSERT INTO :tblStructure (ID, "ColumnName", "Caption", "DataType", "Width", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort") 
  			SELECT row_number() over(ORDER BY IFNULL(d."VisIndex", a."ColumnNum")), "ColAlias", "ColDesc", CASE b.DATA_TYPE_NAME WHEN 'TIMESTAMP' THEN 'DATETIME' WHEN 'DECIMAL' THEN 'NUMERIC' ELSE b.DATA_TYPE_NAME END, 75, 1, '', '', CASE WHEN a."ColEdit" = 'Y' THEN 0 ELSE 1 END, CASE WHEN a."ColEdit" = 'Y' THEN 1 ELSE 0 END, 1, 1, 1 
			FROM UDO4 a 
			INNER JOIN TABLE_COLUMNS b ON b.COLUMN_NAME = a."ColAlias" 
			LEFT JOIN APZ_FORMSETTING d ON d."FormId" = :udoCode AND d."ItemId" = :tableName AND d."UserSign" = :username AND d."ColumnId" = a."ColAlias" 
			WHERE SCHEMA_NAME = CURRENT_SCHEMA 
				AND b.TABLE_NAME = :tableName 
				AND a."SonNum" = :sonNum 
				AND a."Code" = :udoCode
				AND a."ColIsUsed" = 'Y' 
				ORDER BY IFNULL(d."VisIndex", a."ColumnNum");
	END IF;
	UPDATE :tblStructure SET "GroupIndex" = -1 WHERE "GroupIndex" IS NULL;
	-- set màu cho các cột ko đc sửa
	UPDATE :tblStructure SET "BackColor" = 'LightGray' WHERE "AllowEdit" = 0;
	-- set valivalue cho cột Status
	UPDATE :tblStructure SET "DataType" = 'ValidValue' WHERE "ColumnName" = 'Status';	
	SELECT * FROM :tblStructure ORDER BY ID;
	
	-- lấy tên các cột cần lấy dữ liệu
	SELECT STRING_AGG((CASE "ColumnName" WHEN 'Status' THEN '(CASE WHEN a."Status" = ''C'' AND a."Canceled" = ''N'' THEN ''L'' ELSE a."Status" END) AS "Status", ' ELSE 'a."' || "ColumnName" || '", ' END),'') INTO colNames 
		FROM :tblStructure;
	-- xóa 2 ký tự cuối (dấu phẩy)
	SELECT SUBSTRING (:colNames,0,LENGTH(:colNames)-2) INTO colNames FROM DUMMY;
	
	-- Format lại ngày tháng
	SELECT TO_VARCHAR(TO_DATE(:fromDate),'yyyy-MM-dd') INTO strFromDate FROM DUMMY;
	SELECT TO_VARCHAR(TO_DATE(:toDate),'yyyy-MM-dd') INTO strToDate FROM DUMMY;
	
	IF sonNum = 0 THEN
		IF udoType = '3' THEN
			SELECT 'SELECT ' || colNames || ' FROM "' || tableName || '" a LEFT JOIN OUSR b ON b."USERID" = a."UserSign"' 
			|| 'WHERE ((' || :docEntry ||' >-1 AND a."DocEntry" = '|| :docEntry ||') OR (' || :docEntry || ' <0 AND IFNULL(a."CreateDate",'''|| :strFromDate || ''')'
			|| 'BETWEEN ''' || :strFromDate || ''' AND ''' || :strToDate || ''' AND ( ' || :branch || '= -1 OR ( ' || :branch || ' != -1 AND b."USER_CODE" in (SELECT "UserCode" FROM USR6 WHERE "BPLId" = '
			|| :branch || ')))))'
			INTO sqlString FROM DUMMY;
		ELSE
			SELECT 'SELECT ' || colNames || ' FROM "' || tableName || '" a LEFT JOIN OUSR b ON b."USERID" = a."UserSign"' 
			|| 'WHERE ((''' || :code ||''' !='''' AND a."Code" = N'''|| :code ||''') OR (''' || :code || '''= '''' AND IFNULL(a."CreateDate",'''|| :strFromDate || ''')'
			|| 'BETWEEN ''' || :strFromDate || ''' AND ''' || :strToDate || ''' AND ( ' || :branch || '= -1 OR ( ' || :branch || ' != -1 AND b."USER_CODE" in (SELECT "UserCode" FROM USR6 WHERE "BPLId" = '
			|| :branch || ')))))'
			INTO sqlString FROM DUMMY;
		END IF;		
	ELSE
		IF udoType = '3' THEN
			SELECT 'SELECT ' || colNames || ' FROM "' || tableName || '" a WHERE a."DocEntry" =' || CAST(:docEntry AS VARCHAR) || ' ORDER BY a."LineId"' INTO sqlString FROM DUMMY;
		ELSE
			SELECT 'SELECT ' || colNames || ' FROM "' || tableName || '" a WHERE a."Code" =''' || :code || ''' ORDER BY a."LineId"' INTO sqlString FROM DUMMY;
		END IF;			
	END IF;
	
	if sonNum = 0 then
		if exists(select 1 from OUDO where "Code"=:udoCode and "MngSeries"='Y') then
			select string_agg("Series",',') into seriesQuery from NNM1 where "ObjectCode" = :udoCode and cast("GroupCode" as nvarchar) in 
			(select substring("menucode", 19) from oprm where "menucode" like 'barSub_SeriesGroup%' and USER_CODE=:origUser and "authoriza"='F');
			if ifnull(:seriesQuery,'')='' then
				seriesQuery = '0';
			end if;
			
			sqlString := sqlString || ' and "Series" in (' || :seriesQuery || ',-1)';
		end if;
		
		sqlString := sqlString || (case when udoType = '3' then ' order by a."DocEntry";' else ' order by lower(a."Code");' end);
	end if;

	EXECUTE IMMEDIATE :sqlString;
	
	SELECT * FROM :tblValidValue;
	
END;



