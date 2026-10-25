
CALL APZ_EXISTSPROC('APZ_DIAGRAM_DOCUMENT_GET');

GO

CREATE PROCEDURE APZ_DIAGRAM_DOCUMENT_GET (IN DocEntry int, ObjType nvarchar(20))
AS
BEGIN

	declare cnt, i int;
	declare typ, entry int;
	declare tblName, tblLine nvarchar(10);
	declare query nvarchar(1000);
	declare objArr int ARRAY;
	
	IF (:ObjType = '13000') THEN 
		ObjType := '13';
	END IF;

	create local temporary table #tblRet ("ObjType" nvarchar(20), "ObjectId" int, "ObjectNum" int, "Type" nvarchar(10) DEFAULT '', "ObjectCode" nvarchar(20), "Status" char(1), "DocDate" timestamp, "RelatedTyp" nvarchar(10), "RelatedEnt" int, "DocTotal" decimal, "DocCur" nvarchar(10), "isIns" char(1) DEFAULT 'N', "Id" nvarchar(30), "FatherId" nvarchar(30));
	select 'insert into #tblRet select distinct b."ObjType",b."DocEntry",b."DocNum", '''',b."CardCode",CASE WHEN b.CANCELED = ''Y'' THEN ''C'' WHEN b."DocStatus" = ''C'' THEN ''L'' ELSE b."DocStatus" END,b."DocDate",b."RelatedTyp", b."RelatedEnt", CASE WHEN b."DocTotalFC" <> 0 THEN b."DocTotalFC" ELSE b."DocTotal" END, b."DocCur", b."isIns",b."ObjType" || ''-'' || CAST(b."DocEntry" AS nvarchar), CAST(a."BaseType" AS nvarchar) || ''-'' || CAST(a."BaseEntry" AS nvarchar) from '
				|| "TblLine" || ' a join ' || "TblName" || ' b on a."DocEntry"=b."DocEntry" where a."DocEntry"=:DocEntry;'-- || cast(:DocEntry as nvarchar) || ';'
				 into query from APZ_TblObject where "ObjType"=:objType;
	exec :query using :DocEntry; 
	
	create local temporary table #temp as (select * from #tblRet);
	
	while exists(select 1 from #temp) do
	
		lst = select distinct "ObjType", "ObjectId", row_number() over() RowNum from #temp;
		delete from #temp;
		
		select count(*) into cnt from :lst;
		
		for i in 1..:cnt do
			
			declare x, y int;
			select "ObjType", "ObjectId" into typ, entry from :lst where RowNum=:i;
			select "TblName", "TblLine" into tblName, tblLine from APZ_TblObject where "ObjType"=:typ;
			
			objArr = apz_document_getTargetType(:typ);
			y = CARDINALITY(:objArr);
			
			for x in 1..:y do
				select 'insert into #temp select distinct b."ObjType",b."DocEntry",b."DocNum", '''',b."CardCode",CASE WHEN b.CANCELED = ''Y'' THEN ''C'' WHEN b."DocStatus" = ''C'' THEN ''L'' ELSE b."DocStatus" END,b."DocDate",b."RelatedTyp", b."RelatedEnt", CASE WHEN b."DocTotalFC" <> 0 THEN b."DocTotalFC" ELSE b."DocTotal" END, b."DocCur", b."isIns",b."ObjType" || ''-'' || CAST(b."DocEntry" AS nvarchar), CAST(c."BaseType" AS nvarchar) || ''-'' || CAST(c."BaseEntry" AS nvarchar) from '
				|| :tblLine || ' a join ' || "TblName" || ' b join ' || "TblLine" || ' c on b."DocEntry"=c."DocEntry" on a."ObjType"=c."BaseType" and a."DocEntry"=c."BaseEntry" and a."LineNum"=c."BaseLine" where a."DocEntry"=:entry;'-- || cast(:entry as nvarchar) || ';'
				 into query from APZ_TblObject where "ObjType"=:objArr[:x];
				
				exec :query using :entry;
			end for;
			
			objArr = apz_document_getBaseType(:typ);
			y = CARDINALITY(:objArr);
			
			for x in 1..:y do
				select 'insert into #temp select distinct b."ObjType", b."DocEntry",b."DocNum", '''',b."CardCode",CASE WHEN b.CANCELED = ''Y'' THEN ''C'' WHEN b."DocStatus" = ''C'' THEN ''L'' ELSE b."DocStatus" END,b."DocDate",b."RelatedTyp", b."RelatedEnt", CASE WHEN b."DocTotalFC" <> 0 THEN b."DocTotalFC" ELSE b."DocTotal" END, b."DocCur", b."isIns",b."ObjType" || ''-'' || CAST(b."DocEntry" AS nvarchar), CAST(c."BaseType" AS nvarchar) || ''-'' || CAST(c."BaseEntry" AS nvarchar) from '
				|| :tblLine || ' a join ' || "TblName" || ' b join ' || "TblLine" || ' c on b."DocEntry"=c."DocEntry" on a."BaseType"=b."ObjType" and a."BaseEntry"=b."DocEntry" and a."BaseLine"=c."LineNum" where a."DocEntry"=:entry;'-- || cast(:entry as nvarchar) || ';'
				 into query from APZ_TblObject where "ObjType"=:objArr[:x];
				
				exec :query using :entry;
			end for;
			
		end for;
		
		delete from #temp where ("ObjType", "ObjectId", ifnull("FatherId",'')) in (select "ObjType", "ObjectId", ifnull("FatherId",'') from #tblRet);
		insert into #tblRet select distinct * from #temp;
		
	end while;
	
	select * from #tblRet;
	
	drop table #tblRet;
	drop table #temp;
	
END