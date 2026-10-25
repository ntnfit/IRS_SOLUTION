

CREATE PROCEDURE APZ_AdvanceInPayment_Save
AS
BEGIN
	declare DocEntry int = 0;
	declare NextDocNum int = 0;
	
	create local temporary table #APZ_OARCTemp("DocEntry" integer NOT NULL, "DocNum" integer NOT NULL, "Handwrtten" char(1) NULL DEFAULT 'N', "DocStatus" char(1) NULL DEFAULT 'O', "ObjType" nvarchar(20) NULL DEFAULT '810016', "DocDate" timestamp NULL, "DocCur" nvarchar(10) NULL, "DocRate" decimal(19, 6) NULL, "DocTotal" decimal(19, 6) NULL, "DocTotalFC" decimal(19, 6) NULL, "UpdateDate" timestamp NULL, "CreateDate" timestamp NULL, "Series" integer NULL, "UserSign" integer NULL, "UserSign2" integer NULL, "WddStatus" char(1) NULL DEFAULT '-', "LogInstanc" integer NULL DEFAULT 0, "CreateTime" integer, "UpdateTime" integer, "Comments" nvarchar(254) NULL, "PeyMethod" integer NULL);
	exec 'drop table #APZ_OARCTemp';
	exec 'create local temporary table #APZ_OARCTemp as (select * from #APZ_OARC)';
	
	create local temporary table #APZ_ARC1Temp("DocEntry" integer NOT NULL, "LineNum" integer NOT NULL, "VisOrder" integer NULL, "ObjType" nvarchar(20) NULL DEFAULT '810016', "CardType" char(1) NULL DEFAULT 'C', "CardCode" nvarchar(15) NULL, "CardName" nvarchar(100) NULL, "LineTotal" decimal(19, 6) NULL, "TotalFrgn" decimal(19, 6) NULL, "Comments" nvarchar(254) NULL, "TransDate" timestamp NULL, "OcrCode" nvarchar(8) NULL, "OcrCode2" nvarchar(8) NULL, "OcrCode3" nvarchar(8) NULL, "OcrCode4" nvarchar(8) NULL, "OcrCode5" nvarchar(8) NULL, "Currency" nvarchar(10) NULL, "Rate" decimal(19, 6) NULL);
	exec 'drop table #APZ_ARC1Temp';
	exec 'create local temporary table #APZ_ARC1Temp as (select * from #APZ_ARC1)';
	
	select ifnull((select top 1 "DocEntry" from #APZ_OARCTemp), 0) into DocEntry from DUMMY;
	if :DocEntry > 0 then	
		delete from APZ_OARC where "DocEntry" = :DocEntry;
		delete from APZ_ARC1 where "DocEntry" = :DocEntry;	
	end if;
	
	select ifnull((select max("DocEntry") from APZ_OARC), 0) + 1 into DocEntry from DUMMY;
	update #APZ_OARCTemp set "DocEntry" = :DocEntry where ifnull("DocEntry", 0) = 0;
	update #APZ_ARC1Temp set "DocEntry" = :DocEntry where ifnull("DocEntry", 0) = 0;
	
	insert into APZ_OARC select "DocEntry", ROW_NUMBER() OVER(ORDER BY "DocNum") as "DocNum", "Handwrtten", "DocStatus", "ObjType", "DocDate", "DocCur", "DocRate", "DocTotal", "DocTotalFC", "UpdateDate", "CreateDate", "Series", "UserSign", "UserSign2", "WddStatus", "LogInstanc", "CreateTime", "UpdateTime", "Comments", "PeyMethod" from #APZ_OARCTemp;
	insert into APZ_ARC1 select "DocEntry", ROW_NUMBER() OVER (ORDER BY "LineNum"), "VisOrder", "ObjType", "CardType", "CardCode", "CardName", "LineTotal", "TotalFrgn", "Comments", "TransDate", "OcrCode", "OcrCode2", "OcrCode3", "OcrCode4", "OcrCode5" , "Currency", "Rate" from #APZ_ARC1Temp;
END;




