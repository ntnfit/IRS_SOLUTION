
CALL APZ_EXISTSPROC('Apz_CFL_BatchSerialForReturnComponents');

GO

CREATE PROCEDURE Apz_CFL_BatchSerialForReturnComponents (
	IN baseEntry int,
	IN baseLine int,
	IN whsCode nvarchar(8)
)
AS
BEGIN
	DECLARE ManBtchNum char(1) := '';
	DECLARE ManSerNum char(1) := '';
	
	CREATE LOCAL TEMPORARY COLUMN TABLE #tblStructure (
		 "ID" INT
		,"ColumnName" NVARCHAR(50) NOT NULL
		,"Caption" NVARCHAR(100)
		,"DataType" NVARCHAR(50) DEFAULT 'nvarchar'	--int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox) LinkedTable LinkedUDO* LinkedObject cfl(choosefromlist) LinkedCflObject
	                                        		-- color, validcolor (combobox+color),	linkedStore, LinkedCrystal
		,"SubType" NVARCHAR(500)					-- None Phone Percentage Address Image Measurement Price Quantity Rate Sum Time, UDT dùng cho LinkedTable, (udo code : dùng cho LinkedUDO)*	 , ObjectType(số): dùng cho LinkedObject
	                            					-- type of cfl - số của type hoặc tên store(storename @value, @userName, @branch) lấy dữ liệu cfl-store phải có structure, sub - sub form
		,"Width" INT DEFAULT 75
		,"GroupIndex" INT DEFAULT -1				-- dùng cho grid  default -1
		,"Visible" TINYINT DEFAULT 1
		,"FormatType" NVARCHAR(20)					-- numeric, datetime/ 
		,"DisplayFormat" NVARCHAR(20) NULL			-- '{0:n2}'
		,"FilterString" NVARCHAR(5000)
		,"ReadOnly" TINYINT DEFAULT 0
		,"AllowEdit" TINYINT DEFAULT 1
		,"AllowMove" TINYINT DEFAULT 1
		,"AllowSize" TINYINT DEFAULT 1
		,"AllowSort" TINYINT DEFAULT 1
		,"Fixed" VARCHAR(1)							-- L:Left / R:Right
		,"BackColor" NVARCHAR(20)					-- color column //list color https://msdn.microsoft.com/en-us/library/aa358802(v=vs.85).aspx
		,"ForeColor" NVARCHAR(20)
		,"SummaryType" NVARCHAR(10)					-- sum,average...
		,"GroupSummaryType" NVARCHAR(10)
		,"GroupSummaryShowInFooter" TINYINT
		,"SummaryFormat" NVARCHAR(20)				-- (AVR={0:#.##})/ Count ({0})/ Custom format/ Max (MAX={0})/ Min (MIN={0})/ Sum (SUM={0:#.##})
		,"Expression" NVARCHAR(5000)
		,"Parent" TINYINT							-- dùng cho treelist
		,"Key" TINYINT
		,"PivotArea" NVARCHAR(20)
		);	
	 
	INSERT INTO #tblStructure 
					("ColumnName", "Caption", "DataType", "Width", "GroupIndex", "Visible", "FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
  		SELECT   		N'DistNumber',	N'Batch/Serial No',		'nvarchar',		75,		-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		'L',	NULL,		 NULL FROM DUMMY
		UNION SELECT	N'Instock',		N'Instock',				'numeric',		150,	-1,			1,			NULL,		'{0:n5}',		1,		 0,			1,			1,			1,		'L',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	N'CommitQty',	N'Committed Quantity',	'numeric',		150,	-1,			1,			NULL,		'{0:n5}',		1,		 0,			1,			1,			1,		'L',	NULL,		 NULL FROM DUMMY		
		--UNION SELECT	N'BinCode',		N'BinCode',				'nvarchar',		150,	-1,			1,			NULL,		NULL,			1,		 0,			1,			1,			1,		'L',	NULL,		 NULL FROM DUMMY		
		UNION SELECT	N'SnBAbsEntry',	N'Serial/Batch Entry',	'int',			150,	-1,			0,			NULL,		NULL,			1,		 0,			1,			1,			1,		'L',	NULL,		 NULL FROM DUMMY;
		
	SELECT * FROM #tblStructure;
	
	select a."ManBtchNum", a."ManSerNum" into ManBtchNum, ManSerNum from OITM a join WOR1 b on a."ItemCode"=b."ItemCode" where b."DocEntry"=:baseEntry and b."LineNum"=:baseLine;
	
	lstBatch = select distinct a."BatchNum", a."ItemCode", ifnull(a."Quantity",0)-ifnull(b."Quantity",0) "Quantity" from
		(select "BatchNum", sum("Quantity") "Quantity", "ItemCode" from IBT1
			where "BaseType"=60 and "BsDocType"=202 and "BsDocEntry"=:baseEntry and "BsDocLine"=:baseLine
			group by "BatchNum", "ItemCode") a left join
		(select "BatchNum", sum("Quantity") "Quantity", "ItemCode" from IBT1
			where "BaseType"=59 and "BsDocType"=202 and "BsDocEntry"=:baseEntry and "BsDocLine"=:baseLine
			group by "BatchNum", "ItemCode") b on a."BatchNum"=b."BatchNum" and a."ItemCode"=b."ItemCode"
		where ifnull(a."Quantity",0)-ifnull(b."Quantity",0)>0;
	
	IF :ManBtchNum = 'Y' THEN
	
		select a."Quantity", b.*, (select sum("Quantity") from OBTQ where "BatchNum"=a."BatchNum" and "ItemCode"=a."ItemCode") "InStock", :whsCode "WhsCode", 0 "LogQty", 0 "CommitQty", b."AbsEntry" "SnBAbsEntry", 'B' "ManBatchSerial"
		from :lstBatch a join OBTN b on a."BatchNum"=b."DistNumber" and a."ItemCode"=b."ItemCode";

	ELSEIF :ManSerNum = 'Y' THEN
		
		select a."Quantity", b.*, (select sum("Quantity") from OSRQ where "BatchNum"=a."BatchNum" and "ItemCode"=a."ItemCode") "InStock", :whsCode "WhsCode", 0 "LogQty", 0 "CommitQty", b."AbsEntry" "SnBAbsEntry", 'S' "ManBatchSerial"
		from :lstBatch a join OSRN b on a."BatchNum"=b."DistNumber" and a."ItemCode"=b."ItemCode";
		
	ELSE
		SELECT TOP 0 '' AS "DistNumber", 0 AS "Instock", '' AS "BinCode" FROM DUMMY;
	END IF;
	DROP TABLE #tblStructure;

END;