
-- APZ_PMDiagram_Get '1470000132','1'
CREATE PROCEDURE APZ_PMDiagram_Get(in ObjectType VARCHAR(20), in ObjId VARCHAR(50))
AS
begin
	create local temporary table #tableTemp(
		"Id" VARCHAR(50),
		"ObjType" VARCHAR(20),
		"ObjectId" VARCHAR(20),
		"ObjectNum" VARCHAR(20),
		"Type" CHAR(1),
		"ObjectCode" varchar(50),
		"Status" char(1),
		"DocDate" DATETIME,
		"DocTotal" NUMERIC(19,6),
		"DocCur" VARCHAR(10),
		"FatherId" VARCHAR(50),
		"RelatedTyp" VARCHAR(20),
		"RelatedEnt" INT,
		"isIns" CHAR(1) DEFAULT 'N'
	);
	
	--Report
	IF(:ObjectType='1470000134')
	THEN
		INSERT INTO #tableTemp
				("ObjType",
				  "ObjectId",
				  "ObjectNum",
				  "Type",
				  "ObjectCode",
				  "Status",
				  "DocDate",
				  "FatherId",
				  "RelatedTyp",
				  "RelatedEnt",
				  "DocTotal",
				  "DocCur",
				  "isIns",
				  "Id"
				)
		SELECT '1470000134',"DocEntry","DocEntry",'','',"Status",a."PostingDate",'1470000132' || cast((Select top 1 "ProjectId" from APZ_ORTC where "DocEntry"=cast(:ObjId as int)) as VARCHAR(11)),'',0,0,'','N'
					,:ObjectType || :ObjId FROM APZ_ORTC a WHERE a."DocEntry"=cast(:ObjId as int)
			UNION all
		SELECT '1470000132',"AbsEntry","AbsEntry",'', "CardCode","PrgStatus","StartDate",'-1','',0,"ActualAmount",'','N','1470000132' || cast((Select top 1 "ProjectId" from APZ_ORTC where "DocEntry"=Cast(:ObjId as int)) as VARCHAR(11)) FROM APZ_OPRG 
			WHERE "AbsEntry" = (Select top 1 "ProjectId" from APZ_ORTC where "DocEntry"=cast(:ObjId as int))
			UNION all
		SELECT '1470000135',"DocEntry","DocEntry",'' "Type",'',"Status","PostingDate",:ObjectType || :ObjId,'',0,"Total",'','N','1470000135' || cast("DocEntry" as VARCHAR(11)) 
			FROM APZ_OPJC WHERE "DocEntry" IN (SELECT "DocEntry" FROM APZ_PJC1 WHERE "BaseEntry"=Cast(:ObjId as int));
		
		SELECT DISTINCT * FROM #tableTemp order by "FatherId";
	END IF;
	
	--Confirm
	IF(:ObjectType='1470000135')
	THEN
		INSERT INTO #tableTemp
				("ObjType",
				  "ObjectId",
				  "ObjectNum",
				  "Type",
				  "ObjectCode",
				  "Status",
				  "DocDate",
				  "FatherId",
				  "RelatedTyp",
				  "RelatedEnt",
				  "DocTotal",
				  "DocCur",
				  "isIns",
				  "Id"
				)
		SELECT '1470000135',"DocEntry","DocEntry",'' "Type",'',"Status","PostingDate",'1470000134' || cast((SELECT top 1 x."DocEntry" FROM APZ_RTC1 x INNER JOIN APZ_PJC1 y ON x."DocEntry" = y."BaseEntry" WHERE y."DocEntry"=cast(:ObjId as int)) as VARCHAR(11))
				,'',0,"Total",'','N',cast(:ObjectType as nVARCHAR(50)) || cast(:ObjId as nVARCHAR(50)) FROM APZ_OPJC a WHERE a."DocEntry"=cast(:ObjId as int)
			UNION all
		SELECT '1470000132',"AbsEntry","AbsEntry",'',"CardCode","PrgStatus","StartDate",'-1','',0,"ActualAmount",'','N','1470000132' || cast((Select top 1 "ProjectId" from APZ_ORTC where "DocEntry"=cast(:ObjId as int)) as VARCHAR(11)) FROM APZ_OPRG 
			WHERE "AbsEntry" = (Select top 1 "ProjectId" from APZ_OPJC where "DocEntry"=cast(:ObjId as int))
			UNION all
		SELECT '1470000134',"DocEntry","DocEntry",'' "Type",'',"Status","PostingDate",'1470000132' || Cast((Select top 1 "ProjectId" from APZ_ORTC where "DocEntry"=cast(:ObjId as int)) as VARCHAR(11)),'',0,"Total",'','N',
			'1470000134' || cast("DocEntry" as VARCHAR(11)) FROM APZ_ORTC WHERE "DocEntry" IN (SELECT "BaseEntry" FROM APZ_PJC1 WHERE "DocEntry"=cast(:ObjId as int))
			UNION all
		SELECT t0."ObjType",t0."DocEntry",t1."DocNum",'',t1."CardCode",case when T1."CANCELED" ='Y' THEN 'C' WHEN T1."DocStatus"='C' THEN 'L' ELSE T1."DocStatus" END "DocStatus",t0."DocDate"
				,cast(:ObjectType as nVARCHAR(50)) || cast(:ObjId as nVARCHAR(50)),'',0,t1."DocTotal",t1."DocCur",'N', t0."ObjType" || cast(t0."DocEntry" as nVARCHAR(11)) 
			FROM DLN1 t0  left JOIN ODLN  t1 ON t0."DocEntry"=t1."DocEntry" WHERE t0."DocEntry" IN (SELECT "DocEntry" FROM DLN1 WHERE "U_OriginCode"=cast(:ObjId as int) and "U_OriginType"='1470000135')
			UNION all
		SELECT t0."ObjType",t0."DocEntry",t1."DocNum",'',t1."CardCode",case when T1."CANCELED" ='Y' THEN 'C' WHEN T1."DocStatus"='C' THEN 'L' ELSE T1."DocStatus" END "DocStatus",t0."DocDate"
				,cast(:ObjectType as nVARCHAR(50)) || cast(:ObjId as nVARCHAR(50)),'',0,t1."DocTotal",t1."DocCur",'N', t0."ObjType" || cast(t0."DocEntry" as nVARCHAR(11))
			FROM INV1 t0  left JOIN OINV  t1 ON t0."DocEntry"=t1."DocEntry" WHERE t0."DocEntry" IN (SELECT "DocEntry" FROM INV1 WHERE "U_OriginCode"=cast(:ObjId as int) and "U_OriginType"='1470000135');
	
		SELECT DISTINCT * FROM #tableTemp order by "FatherId";
	END IF;
	
	--Project
	IF(:ObjectType='1470000132')
	THEN
		INSERT INTO #tableTemp
				( "ObjType",
				  "ObjectId",
				  "ObjectNum",
				  "Type",
				  "ObjectCode",
				  "Status",
				  "DocDate",
				  "FatherId",
				  "RelatedTyp",
				  "RelatedEnt",
				  "DocTotal",
				  "DocCur",
				  "isIns",
				  "Id"
				)
		SELECT '1470000132',"AbsEntry","AbsEntry",'' "Type",'',"PrgStatus","StartDate",'-1','',0,"ActualAmount",'','N',cast(:ObjectType as nVARCHAR(50)) || Cast(:ObjId as nVARCHAR(50)) FROM APZ_OPRG g WHERE g."AbsEntry"=cast(:ObjId as int)
			UNION all
		SELECT '1470000135',"DocEntry","DocEntry",'' "Type",'',"Status","PostingDate",'-1','',0,"Total",'','N',cast(:ObjectType as nVARCHAR(50)) || Cast(:ObjId as nVARCHAR(50)) FROM APZ_OPJC a WHERE a."DocEntry"=cast(:ObjId as int)
			UNION all
		SELECT t0."ObjType",t0."DocEntry",t1."DocNum",'',t1."CardCode",case when T1."CANCELED" ='Y' THEN 'C' WHEN T1."DocStatus"='C' THEN 'L' ELSE T1."DocStatus" END "DocStatus",t0."DocDate"
				,cast(:ObjectType as nVARCHAR(50)) || cast(:ObjId as nVARCHAR(50)),'',0,t1."DocTotal",t1."DocCur",'N', t0."ObjType" || cast(t0."DocEntry" as nVARCHAR(11)) 
			FROM PRQ1 t0  left JOIN OPRQ  t1 ON t0."DocEntry"=t1."DocEntry" WHERE t0."DocEntry" IN (SELECT "DocEntry" FROM PRQ1 WHERE "U_OriginCode"=cast(:ObjId as int) and "U_OriginType"='1470000132')
			UNION all
		SELECT t0."ObjType",t0."DocEntry",t1."DocNum",'',t1."CardCode",case when T1."CANCELED" ='Y' THEN 'C' WHEN T1."DocStatus"='C' THEN 'L' ELSE T1."DocStatus" END "DocStatus",t0."DocDate"
				, cast(:ObjectType as nVARCHAR(50)) || cast(:ObjId as nVARCHAR(50)),'',0,t1."DocTotal",t1."DocCur",'N', t0."ObjType" || cast(t0."DocEntry" as nVARCHAR(11)) 
			FROM POR1 t0  left JOIN OPOR  t1 ON t0."DocEntry"=t1."DocEntry" WHERE t0."DocEntry" IN (SELECT "DocEntry" FROM POR1 WHERE "U_OriginCode"=cast(:ObjId as int) and "U_OriginType"='1470000132')
			UNION all
		SELECT t0."ObjType",t0."DocEntry",t1."DocNum",'',t1."CardCode",case when T1."CANCELED" ='Y' THEN 'C' WHEN T1."DocStatus"='C' THEN 'L' ELSE T1."DocStatus" END "DocStatus",t0."DocDate"
				,cast(:ObjectType as nVARCHAR(50)) || cast(:ObjId as nVARCHAR(50)),'',0,t1."DocTotal",t1."DocCur",'N', t0."ObjType" || cast(t0."DocEntry" as nVARCHAR(11)) 
			FROM PDN1 t0  left JOIN OPDN  t1 ON t0."DocEntry"=t1."DocEntry" WHERE t0."DocEntry" IN (SELECT "DocEntry" FROM PDN1 WHERE "U_OriginCode"=cast(:ObjId as int) and "U_OriginType"='1470000132')
			UNION all
		SELECT t0."ObjType",t0."DocEntry",t1."DocNum",'',t1."CardCode",case when T1."CANCELED" ='Y' THEN 'C' WHEN T1."DocStatus"='C' THEN 'L' ELSE T1."DocStatus" END "DocStatus",t0."DocDate"
				,cast(:ObjectType as nVARCHAR(50)) || cast(:ObjId as nVARCHAR(50)),'',0,t1."DocTotal",t1."DocCur",'N', t0."ObjType" || cast(t0."DocEntry" as nVARCHAR(11)) 
			FROM PCH1 t0  left JOIN OPCH  t1 ON t0."DocEntry"=t1."DocEntry" WHERE t0."DocEntry" IN (SELECT "DocEntry" FROM PCH1 WHERE "U_OriginCode"=cast(:ObjId as int) and "U_OriginType"='1470000132')
			UNION all
		SELECT t0."ObjType",t0."DocEntry",t1."DocNum",'',t1."CardCode",case when T1."CANCELED" ='Y' THEN 'C' WHEN T1."DocStatus"='C' THEN 'L' ELSE T1."DocStatus" END "DocStatus",t0."DocDate"
				,cast(:ObjectType as nVARCHAR(50)) || cast(:ObjId as nVARCHAR(50)),'',0,t1."DocTotal",t1."DocCur",'N', t0."ObjType" || cast(t0."DocEntry" as nVARCHAR(11)) 
			FROM INV1 t0  left JOIN OINV  t1 ON t0."DocEntry"=t1."DocEntry" WHERE t0."DocEntry" IN (SELECT "DocEntry" FROM INV1 WHERE "U_OriginCode"=cast(:ObjId as int) and "U_OriginType"='1470000132')
			UNION all
			SELECT t0."ObjType",t0."DocEntry",t1."DocNum",'',t1."CardCode",case when T1."CANCELED" ='Y' THEN 'C' WHEN T1."DocStatus"='C' THEN 'L' ELSE T1."DocStatus" END "DocStatus",t0."DocDate"
				,cast(:ObjectType as nVARCHAR(50)) || cast(:ObjId as nVARCHAR(50)),'',0,t1."DocTotal",t1."DocCur",'N', t0."ObjType" || cast(t0."DocEntry" as nVARCHAR(11)) 
			FROM IGE1 t0  left JOIN OIGE  t1 ON t0."DocEntry"=t1."DocEntry" WHERE t0."DocEntry" IN (SELECT "DocEntry" FROM IGE1 WHERE "U_OriginCode"=cast(:ObjId as int) and "U_OriginType"='1470000132')
	UNION all
		SELECT '1470000134',"DocEntry","DocEntry",'' "Type",'',"Status","PostingDate",cast(:ObjectType as nVARCHAR(50)) || cast(:ObjId as nVARCHAR(50)),'',0,"Total",'','N','1470000134' || cast("DocEntry" as nVARCHAR(11))
			FROM APZ_ORTC WHERE "DocEntry" IN (SELECT "DocEntry" FROM APZ_ORTC WHERE "ProjectId"=cast(:ObjId as int))
			UNION all
		SELECT '1470000135',"DocEntry","DocEntry",'' "Type",'',"Status","PostingDate",cast(:ObjectType as nVARCHAR(50)) || cast(:ObjId as nVARCHAR(50)),'',0,"Total",'','N','1470000135' || cast("DocEntry" as nVARCHAR(11))
			FROM APZ_ORTC WHERE "DocEntry" IN (SELECT "DocEntry" FROM APZ_OPJC WHERE "ProjectId"=cast(:ObjId as int));
		
		SELECT DISTINCT * FROM #tableTemp order by "FatherId";
	END IF;
end




