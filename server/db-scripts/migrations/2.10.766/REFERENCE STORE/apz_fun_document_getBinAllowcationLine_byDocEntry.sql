
CALL APZ_EXISTSPROC('apz_fun_document_getBinAllowcationLine_byDocEntry');

GO

CREATE PROCEDURE apz_fun_document_getBinAllowcationLine_byDocEntry
(
	in docEntry nvarchar(5000) default null,
	in ObjType VARCHAR(20),
	out bin table
	(
		"ItemCode" nvarchar(50),
		"BinAbs" int,
		"BinCode" nvarchar(50),
		"CreateDate" datetime,
		"SelectedQty" decimal(21,6),
		"WhsCode" nvarchar(50),		
		"BinDescription" nvarchar(254),
		"BaseLineNumber" int,
		"BaseDocEntry" int,
		"ManBatchSerial" char(1),
		"LogQty"	decimal(21,6),		
		"IsExits" char(1),
		"TranType" char(1),
		"IsBase" char(1)
	)
)
AS
begin

	declare obj nvarchar(20);
	declare ret table
	(
		"ItemCode" nvarchar(50),
		"BinAbs" int,
		"BinCode" nvarchar(50),
		"CreateDate" datetime,
		"SelectedQty" decimal(21,6),
		"WhsCode" nvarchar(50),		
		"BinDescription" nvarchar(254),
		"BaseLineNumber" int,
		"BaseDocEntry" int,
		"ManBatchSerial" char(1),
		"LogQty"	decimal(21,6),		
		"IsExits" char(1),
		"TranType" char(1),
		"IsBase" char(1)
	);

	if :ObjType = 112
	then

		select (select top 1 "ObjType" from ODRF where "DocEntry" in (select cast(Data as int) from Split(:docEntry,','))) into obj from dummy;

		INSERT INTO :ret ("ItemCode", "BinAbs", "BinCode", "BinDescription", "CreateDate", "BaseLineNumber", "TranType", "LogQty", "WhsCode") 
		SELECT a."ItemCode", c."AbsEntry", c."BinCode", c."Descr", c."CreateDate", a."LineNum", 
			case when :obj = 67 then (case when a."BinActTyp" in (2, 20) then 1 else 2 end) else 1 end "TranType", a."Quantity", a."WhsCode" 
				FROM DRF19 a INNER JOIN OBIN c ON a."BinAbs" = c."AbsEntry"
		WHERE a."DocEntry" = :docEntry AND a."SnBType" = -1;
		
		update :ret a set "SelectedQty"=(select sum("OnHandQty") from OIBQ where "BinAbs"=a."BinAbs" and "ItemCode"=a."ItemCode");
		
	else

		INSERT INTO :ret ("ItemCode", "BinAbs", "BinCode", "BinDescription", "CreateDate", "BaseLineNumber", "BaseDocEntry", "TranType", "LogQty", "WhsCode")
		SELECT a."ItemCode", c."AbsEntry", c."BinCode", c."Descr", c."CreateDate", a."DocLineNum", a."DocEntry", 
				case when :ObjType = 67 then (case when a."ActionType" in (2, 20) then 1 else 2 end) else 1 end "TranType", b."Quantity", c."WhsCode" 
		FROM OILM a JOIN OBTL b ON a."MessageID" = b."MessageID"
					JOIN OBIN c ON b."BinAbs" = c."AbsEntry"
		where a."DocEntry" in (select cast(Data as int) from Split(:docEntry,',')) and a."TransType" = :ObjType and a."SnBType" = 4;
		
		update :ret a set "SelectedQty"=(select sum("OnHandQty") from OIBQ where "BinAbs"=a."BinAbs" and "ItemCode"=a."ItemCode");
	END if;
	
	bin = select * from :ret;
end