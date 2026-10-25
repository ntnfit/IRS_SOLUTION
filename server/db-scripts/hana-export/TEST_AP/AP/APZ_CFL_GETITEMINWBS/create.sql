
CREATE PROCEDURE APZ_CFL_GetItemInWbs(
in projectId int,
in wbs nvarchar(50)
)
AS 
BEGIN
declare idWBS int=0;
create local temporary table #tblStructure
    (
     "ColumnName" NVARCHAR(50) NOT NULL
    ,"Caption" NVARCHAR(100) NULL
    ,"DataType" VARCHAR(20) NULL DEFAULT 'nvarchar' --int decimal money datetime date/ numeric alpha Memo(general) Numeric Date float(unit)  ValidValue(combobox)
	,"SubType" VARCHAR(20) NULL			--none phone percentage address image measurement price quantity rate sum time
    ,"Width" INT NULL DEFAULT 75
    ,"GroupIndex" INT NULL DEFAULT -1	-- dùng cho grid  default -1
    ,"Visible" tinyint NULL DEFAULT 1
    ,"FormatType" NVARCHAR(20) NULL   -- numeric, datetime/ 
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
	--,Parent BIT NULL	-- dùng cho treelist
	--,[Key] BIT NULL		-- dùng cho treelist (identity)
    );
    
    INSERT INTO #tblStructure
				("ColumnName",		"Caption",				"DataType",		"Width",	"GroupIndex", "Visible",	"FormatType", "DisplayFormat", "ReadOnly", "AllowEdit", "AllowMove", "AllowSize", "AllowSort", "Fixed", "SummaryType", "GroupSummaryType")
        SELECT	 'ItemCode',		'Item Code',			'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'ItemDescription',	'Item Name',			'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'ItmsGrpNam',		'Item Group',			'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'BOMTreeType',		'BOMTreeType',			'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy
	union select 'TreeType',		'TreeType',				'nvarchar',			150,		-1,				1,			NULL,			NULL,			1,		 0,			1,			1,			1,		 'L',		NULL,		 NULL 		from dummy;
		
SELECT *FROM #tblStructure;
select ifnull((select top 1 "LineNum"  from APZ_PRG1 where "AbsEntry"= :projectId and "TaskCode"=:wbs),0) into idWBS from dummy;
SELECT A."ItemCode", 
		A."ItemName" AS "ItemDescription", 
		A."ItemName" AS "ItemName",
		'' "Substitute",
		IFNULL(A."FrgnName", '') AS "FrgnName", 
		IFNULL(f."ItmsGrpCod", 0) AS "ItmsGrpCod",
		ifnull(f."ItmsGrpNam", '') "ItmsGrpNam",
        a."OnHand" as "QuantityOnStock", l."TreeType" AS "BOMTreeType", 
		IFNULL(k."Inactive", '') AS "VatPuInActive",
        IFNULL(g."Inactive", '') AS "VatSaInActive",
        
		case ifnull(l."TreeType",'N')
			when 'A' then 1
			when 'S' then 2
			when 'P' then 3
			when 'T' then 4
			when 'N' then
					case IFNULL((select "TreeType" from OITT where "Code" = (select "Father" from (select "Father", row_number () over 
               (partition by itt1."Father" ORDER BY itt1."Father" asc) as RN from itt1 where "Code"=a."ItemCode") where rn = 2)) ,'N')
					when 'S' then 5
					else 0
				End
		end AS "TreeType", 
		A."BuyUnitMsr", 
		A."SalUnitMsr",
		A."NumInBuy",
		A."NumInSale",
		A."InvntryUom",
		a."UgpEntry" as "UgpCode",
		ifnull(a1."AltQty", 1) / IFNULL(a1."BaseQty", 1) as "NumInInven",
		 A."CardCode",
		 '' "SubCatNum",
		A."IssueMthd"
            FROM OITM A
            LEFT JOIN OSHP D ON A."ShipType" = D."TrnspCode"
            LEFT JOIN OMRC E ON A."FirmCode" = E."FirmCode"
            LEFT JOIN OITB f ON A."ItmsGrpCod" = f."ItmsGrpCod"
            LEFT JOIN OVTG g ON A."VatGourpSa" = g."Code"
            LEFT JOIN OVTG k ON A."VatGroupPu" = k."Code"
            LEFT JOIN OITT l ON l."Code" = A."ItemCode"
			left join UGP1 a1 on a."UgpEntry" = a1."UgpEntry" and a1."UomEntry" in (select "UomEntry" from OUOM where "UomName" = a."InvntryUom")
			left join  APZ_PRG4  on APZ_PRG4."ItemCode"=a."ItemCode" where APZ_PRG4."AbsEntry"=:projectId and APZ_PRG4."BaseLine"=:idWBS;
Drop table #tblStructure;

 end;




