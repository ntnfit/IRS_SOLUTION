
CREATE PROCEDURE apz_diagram_get(in ObjectType VARCHAR(20), in ObjId VARCHAR(50))
AS
begin

	declare maxId int = 0;
	declare i int = 0;
	declare fatherCode nvarchar(50) = '';
	
	IF :ObjectType='202'
	THEN
		declare tableData table
		(
			"Id" VARCHAR(50),
			"ObjType" VARCHAR(20),
			"ObjectId" VARCHAR(20),
			"ObjectNum" VARCHAR(20),
			"Type" CHAR(1),
			"ObjectCode" varchar(50),
			"Status" char(1),
			"DocDate" timestamp,
			"DocTotal" NUMERIC(19,6),
			"DocCur" VARCHAR(10),
			"FatherId" VARCHAR(50),
			"RelatedTyp" VARCHAR(20),
			"RelatedEnt" INT,
			"isIns" CHAR(1)
		);
	
		INSERT INTO :tableData ("ObjType", "ObjectId", "ObjectNum", "Type", "ObjectCode", "Status", "DocDate", "FatherId", "RelatedTyp", "RelatedEnt", "DocTotal", "DocCur", "isIns", "Id") 
			SELECT '202', "DocEntry", "DocNum", "Type", "ItemCode", "Status", a."PostDate", '-1', '', cast(0 as int),cast(0 as numeric(19,6)), '', 'N', :ObjectType || :ObjId FROM owor a WHERE a."DocEntry" = CAST(:ObjId AS integer); 
		
		INSERT INTO :tableData ("ObjType", "ObjectId", "ObjectNum", "Type", "ObjectCode", "Status", "DocDate", "FatherId", "RelatedTyp", "RelatedEnt", "DocTotal", "DocCur", "isIns", "Id") 
			SELECT "ObjType", "DocEntry", "DocNum", '' AS "Type", "CardCode", "DocStatus" AS "Status", "DocDate", :ObjectType || :ObjId, "RelatedTyp", "RelatedEnt", "DocTotal", "DocCur", "isIns"
				, "ObjType" || CAST("DocEntry" AS varchar(11)) FROM oign WHERE "DocEntry" IN (SELECT "DocEntry" FROM IGN1 WHERE "BaseEntry" = CAST(:ObjId AS integer) AND "BaseType" = '202') 
		UNION ALL 
			SELECT "ObjType", "DocEntry", "DocNum", '' AS "Type", "CardCode", "DocStatus" AS "Status", "DocDate", :ObjectType || :ObjId, "RelatedTyp", "RelatedEnt", "DocTotal", "DocCur", "isIns"
				, "ObjType" || CAST("DocEntry" AS varchar(11)) FROM oige WHERE "DocEntry" IN (SELECT "DocEntry" FROM IGE1 WHERE "BaseEntry" = CAST(:ObjId AS integer) AND "BaseType" = '202');
	
		SELECT DISTINCT * FROM :tableData;

	ELSEIF :ObjectType='66'
	THEN
		
		---------------------------------------------------- Lấy dữ liệu phiếu gốc ---------------------------------------------------------------------------------------------------------
		declare diagram_cte table ("Code" nvarchar(50), "Father" nvarchar(50), "Type" int, ID int);
		insert into :diagram_cte SELECT "Code", "Father", T0."Type", row_number() over (order by "Code") ID FROM itt1 T0 INNER JOIN oitm T1 ON t0."Code" = t1."ItemCode" WHERE "Father" = :ObjId OR "Code" = :ObjId;
		---------------------------------------------------- Lấy dữ liệu maxid để đánh ID cho các phiếu tiếp theo trong quá trình đệ quy ----------------------------------------------------
		select ifnull((select max(ID) from :diagram_cte), 0) into maxId from DUMMY;
		--------------------------------------------------- đệ quy để lấy dữ liệu các phiếu liên quan --------------------------------------------------------------------------------------
		WHILE :i <= :maxId
		DO
		
			select ifnull((select "Code" from :diagram_cte where Id = :i), '') into fatherCode from DUMMY;
			insert into :diagram_cte
				SELECT T0."Code",T0."Father",T0."Type", row_number() over (order by T0."Code") + :maxId FROM itt1 T0 
				 INNER JOIN oitm T1 ON t0."Code"=t1."ItemCode" where t0."Father"= :fatherCode;
				 
			select ifnull((select max(ID) from :diagram_cte), 0) into maxId from DUMMY;
			i = :i + 1;
		
		END WHILE;	
		------------------------------------------------------ Kết thúc đệ quy ----------------------------------------------------------------------------------------------------------
		 
		/*WITH diagram_father AS (
			SELECT Code,Father,Type FROM #tbl
			UNION ALL
			SELECT T0.Code,T0.Father,T0.Type FROM ITT1 T0 
			INNER JOIN oitm T1 ON t0.Code=t1.ItemCode
			INNER JOIN diagram_father e ON e.Father=t0.Code
			)*/
		SELECT DISTINCT * FROM (SELECT distinct "Code",CASE WHEN IFNULL(T2."ItemName",'')='' THEN t3."ResName" ELSE T2."ItemName" END "Name",T0."Father" FROM :diagram_cte T0 
			LEFT JOIN oitm t2 ON t0."Code"=t2."ItemCode"
			LEFT JOIN orsc t3 ON T0."Code"=T3."ResCode"
			UNION ALL
	       SELECT DISTINCT T0."Father" AS "Code", t1."ItemName" AS "Name",'' FROM :diagram_cte T0 INNER JOIN oitm t1 ON t0."Father"=t1."ItemCode") x ORDER BY x."Father";
	END	IF;
end

