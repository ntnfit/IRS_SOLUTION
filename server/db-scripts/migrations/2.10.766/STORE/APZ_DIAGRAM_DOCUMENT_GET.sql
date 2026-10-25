
CALL APZ_EXISTSPROC('APZ_DIAGRAM_DOCUMENT_GET');

GO

CREATE PROCEDURE APZ_DIAGRAM_DOCUMENT_GET (IN DocEntry int, ObjType nvarchar(20))
AS
BEGIN

	declare cnt, i int;
	declare typ int;
	declare entry nvarchar(10000);
	declare tblName, tblLine nvarchar(10);
	declare query nvarchar(10000);
	declare objArr int ARRAY;
	
	declare tblRet table ("ObjType" nvarchar(20), "ObjectId" int, "ObjectNum" int, "Type" nvarchar(10), "ObjectCode" nvarchar(20), "Status" char(1), "DocDate" timestamp, "RelatedTyp" nvarchar(10), "RelatedEnt" int, "DocTotal" decimal, "DocCur" nvarchar(10), "isIns" char(1), "Id" nvarchar(30), "FatherId" nvarchar(30));
	declare tbl table ("ObjType" nvarchar(20), "DocEntry" int, "DocNum" int, "Type" nvarchar(10), "CardCode" nvarchar(20), "Status" char(1), "DocDate" timestamp, "RelatedTyp" nvarchar(10), "RelatedEnt" int, "DocTotal" decimal, "DocCur" nvarchar(10), "isIns" char(1), "Id" nvarchar(30), "FatherId" nvarchar(30));
	declare temp table ("ObjType" nvarchar(20), "ObjectId" int, "ObjectNum" int, "Type" nvarchar(10), "ObjectCode" nvarchar(20), "Status" char(1), "DocDate" timestamp, "RelatedTyp" nvarchar(10), "RelatedEnt" int, "DocTotal" decimal, "DocCur" nvarchar(10), "isIns" char(1), "Id" nvarchar(30), "FatherId" nvarchar(30));
	
	-- danh sách phiếu đã process
	declare pDoc table ("ObjType" nvarchar(20), "ObjectId" int);
	insert into :pDoc values('', -1);
	
	IF (:ObjType = '13000') THEN 
		ObjType := '13';
	END IF;
	
	select 'select distinct b."ObjType",b."DocEntry",b."DocNum", '''' "Type",b."CardCode",CASE WHEN b.CANCELED = ''Y'' THEN ''C'' WHEN b."DocStatus" = ''C'' THEN ''L'' ELSE b."DocStatus" END "Status",b."DocDate",b."RelatedTyp", b."RelatedEnt", CASE WHEN b."DocTotalFC" <> 0 THEN b."DocTotalFC" ELSE b."DocTotal" END "DocTotal", b."DocCur", b."isIns",b."ObjType" || ''-'' || CAST(b."DocEntry" AS nvarchar) "Id", CAST(a."BaseType" AS nvarchar) || ''-'' || CAST(a."BaseEntry" AS nvarchar) "FatherId" from '
				|| "TblLine" || ' a join ' || "TblName" || ' b on a."DocEntry"=b."DocEntry" where a."DocEntry"=:DocEntry'-- || cast(:DocEntry as nvarchar) || ';'
				 into query from APZ_TblObject where "ObjType"=:objType;
	exec :query into tbl using :DocEntry;
	
	insert into :tblRet select * from :tbl;
	insert into :temp select * from :tbl;
	
	while exists(select 1 from :temp) do
		
		-- danh sách phiếu chuẩn bị process
		lst1 = select "ObjType", "ObjectId" from (select distinct "ObjType", "ObjectId" from :temp) x
		 	  where not exists(select 1 from :pDoc where "ObjType"=x."ObjType" and "ObjectId"=x."ObjectId");
		insert into :pDoc select "ObjType", "ObjectId" from :lst1;
		delete from :temp;
		
		-- group theo object type
		lst = select "ObjType", string_agg("ObjectId",',') "ObjectId", count("ObjectId") CntId, row_number() over() RowNum from :lst1 group by "ObjType";
		
		for i in 1..(select count(*) from :lst) do
			
			declare x, y int;
			select "ObjType", "ObjectId", CntId into typ, entry, cnt from :lst where RowNum=:i;
			select "TblName", "TblLine" into tblName, tblLine from APZ_TblObject where "ObjType"=:typ;

			objArr = apz_document_getTargetType(:typ);
			y = CARDINALITY(:objArr);
			
			for x in 1..:y do
				select 'select distinct b."ObjType",b."DocEntry",b."DocNum", '''' "Type",b."CardCode",CASE WHEN b.CANCELED = ''Y'' THEN ''C'' WHEN b."DocStatus" = ''C'' THEN ''L'' ELSE b."DocStatus" END "Status",b."DocDate",b."RelatedTyp", b."RelatedEnt", CASE WHEN b."DocTotalFC" <> 0 THEN b."DocTotalFC" ELSE b."DocTotal" END "DocTotal", b."DocCur", b."isIns",b."ObjType" || ''-'' || CAST(b."DocEntry" AS nvarchar) "Id", CAST(c."BaseType" AS nvarchar) || ''-'' || CAST(c."BaseEntry" AS nvarchar) "FatherId" from '
				|| :tblLine || ' a join ' || "TblName" || ' b join ' || "TblLine" || ' c on b."DocEntry"=c."DocEntry" on a."ObjType"=c."BaseType" and a."DocEntry"=c."BaseEntry" and a."LineNum"=c."BaseLine" where a."DocEntry" ' || case when :cnt = 1 then '=:entry' else 'in (' || :entry || ')' end
				 into query from APZ_TblObject where "ObjType"=:objArr[:x];
				
				if :cnt = 1 then
					exec :query into tbl using :entry;
				else
					exec :query into tbl;
				end if;
				
				insert into :temp select * from :tbl;
			end for;
			
			objArr = apz_document_getBaseType(:typ);
			y = CARDINALITY(:objArr);
			
			for x in 1..:y do
				select 'select distinct b."ObjType", b."DocEntry",b."DocNum", '''' "Type",b."CardCode",CASE WHEN b.CANCELED = ''Y'' THEN ''C'' WHEN b."DocStatus" = ''C'' THEN ''L'' ELSE b."DocStatus" END "Status",b."DocDate",b."RelatedTyp", b."RelatedEnt", CASE WHEN b."DocTotalFC" <> 0 THEN b."DocTotalFC" ELSE b."DocTotal" END "DocTotal", b."DocCur", b."isIns",b."ObjType" || ''-'' || CAST(b."DocEntry" AS nvarchar) "Id", CAST(c."BaseType" AS nvarchar) || ''-'' || CAST(c."BaseEntry" AS nvarchar) "FatherId" from '
				|| :tblLine || ' a join ' || "TblName" || ' b join ' || "TblLine" || ' c on b."DocEntry"=c."DocEntry" on a."BaseType"=b."ObjType" and a."BaseEntry"=b."DocEntry" and a."BaseLine"=c."LineNum" where a."DocEntry" ' || case when :cnt = 1 then '=:entry' else 'in (' || :entry || ')' end
				 into query from APZ_TblObject where "ObjType"=:objArr[:x];
				
				if :cnt = 1 then
					exec :query into tbl using :entry;
				else
					exec :query into tbl;
				end if;
				
				insert into :temp select * from :tbl;
			end for;
			
		end for;
		
		delete from :temp where ("ObjType", "ObjectId", ifnull("FatherId",'')) in (select "ObjType", "ObjectId", ifnull("FatherId",'') from :tblRet);
		insert into :tblRet select distinct * from :temp;
		
	end while;
	
	select * from :tblRet;
	
END