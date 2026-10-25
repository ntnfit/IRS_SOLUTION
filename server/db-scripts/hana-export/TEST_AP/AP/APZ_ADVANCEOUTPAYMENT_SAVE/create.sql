

CREATE PROCEDURE APZ_AdvanceOutPayment_Save
AS
BEGIN
	declare DocEntry int = 0;
	declare NextDocNum int = 0;
	declare chkExists varchar(2) = '';
	declare rowNumber int = 0;
	
	create local temporary table #APZ_OAPMTemp("DocEntry" integer NOT NULL, "DocNum" integer NOT NULL, "Handwrtten" char(1) NULL DEFAULT 'N', "DocStatus" char(1) NULL DEFAULT 'O', "ObjType" nvarchar(20) NULL DEFAULT '810017', "DocDate" timestamp NULL, "DocCur" nvarchar(10) NULL, "DocRate" decimal(19, 6) NULL, "DocTotal" decimal(19, 6) NULL, "DocTotalFC" decimal(19, 6) NULL, "UpdateDate" timestamp NULL, "CreateDate" timestamp NULL, "Series" integer NULL, "UserSign" integer NULL, "UserSign2" integer NULL, "WddStatus" char(1) NULL DEFAULT '-', "LogInstanc" integer NULL DEFAULT 0, "CreateTime" integer, "UpdateTime" integer, "Comments" nvarchar(254) NULL, "PeyMethod" integer NULL);
	exec 'drop table #APZ_OAPMTemp';
	exec 'create local temporary table #APZ_OAPMTemp as (select * from #APZ_OAPM)';
	
	create local temporary table #APZ_APM1Temp("DocEntry" integer NOT NULL, "LineNum" integer NOT NULL, "VisOrder" integer NULL, "ObjType" nvarchar(20) NULL DEFAULT '810017', "CardType" char(1) NULL DEFAULT 'C', "CardCode" nvarchar(15) NULL, "CardName" nvarchar(100) NULL, "LineTotal" decimal(19, 6) NULL, "Comments" nvarchar(254) NULL, "TransDate" timestamp NULL, "OcrCode" nvarchar(8) NULL, "OcrCode2" nvarchar(8) NULL, "OcrCode3" nvarchar(8) NULL, "OcrCode4" nvarchar(8) NULL, "OcrCode5" nvarchar(8) NULL, "Currency" nvarchar(10) NULL);
	exec 'drop table #APZ_APM1Temp';
	exec 'create local temporary table #APZ_APM1Temp as (select * from #APZ_APM1)';
	
	select ifnull((select top 1 ifnull("DocEntry", 0) from #APZ_OAPMTemp), 0) into DocEntry from DUMMY;
	if :DocEntry > 0 then
		delete from APZ_OAPM where "DocEntry" = :DocEntry;
		delete from APZ_APM1 where "DocEntry" = :DocEntry;
	end if;
	
	select ifnull((select max("DocEntry") from APZ_OAPM), 0) + 1 into DocEntry from DUMMY;
	update #APZ_OAPMTemp set "DocEntry" = :DocEntry where ifnull("DocEntry", 0) = 0;
	update #APZ_APM1Temp set "DocEntry" = :DocEntry where ifnull("DocEntry", 0) = 0;
		
	insert into APZ_OAPM select "DocEntry", ROW_NUMBER() OVER(ORDER BY "DocNum") as "DocNum", "Handwrtten", "DocStatus", "ObjType", "DocDate", "DocCur", "DocRate", "DocTotal", "DocTotalFC", "UpdateDate", "CreateDate", "Series", "UserSign", "UserSign2", "WddStatus", "LogInstanc", "CreateTime", "UpdateTime", "Comments", "PeyMethod" from #APZ_OAPMTemp;
	insert into APZ_APM1 select "DocEntry", ROW_NUMBER() OVER(ORDER BY "LineNum") as "LineNum", "VisOrder", "ObjType", "CardType", "CardCode", "CardName", "LineTotal", "Comments", "TransDate", "OcrCode", "OcrCode2", "OcrCode3", "OcrCode4", "OcrCode5", "Currency" from #APZ_APM1Temp;

	if ::ROWCOUNT <= 0 then
		select 01 as "ErrorNumber", 'Save failed' as "ErrorMessage" from dummy;
		return;
	end if;
END;



