
CALL APZ_EXISTSPROC('apz_document_copy_header');

GO

CREATE PROCEDURE apz_document_copy_header(
	in fromtype NVARCHAR(20),
	in docentry NVARCHAR(5000),
	in totype nvarchar(20),
	in bplId INT default -1,
	in username NVARCHAR(50) default ''
)
AS
BEGIN
	DECLARE sqlQuery NVARCHAR(5000000);
	DECLARE tblname NVARCHAR(10)='';
	DECLARE tblline NVARCHAR(10)='';
	DECLARE tblTo nvarchar(10) = '';
	DECLARE MainCurrency NVARCHAR(10)= '';
	DECLARE Series INT = -1;
	DECLARE DocNum INT = -1;
	declare checkCount int = 0;
	----- biến để lấy giá trị default price list. để set cho info price trong trường hợp các phiếu không có BP
	declare defaultPriceList int = -1;
	select ifnull((select top 1 "ListNum" from OPLN order by "ListNum"),-1) into defaultPriceList FROM DUMMY;
	
	IF :fromtype='700021' -- oShipping
	then
		tblname='ORDR';
		tblline='RDR1';
		select ifnull((SELECT TOP 1 t1."DocEntry" FROM apz_ship1 T0 INNER JOIN ordr t1 ON t0."TransNo"=t1."DocNum" 
		 INNER JOIN APZ_SHIP T2 ON T0."DocEntry"=T2."DocEntry" WHERE T0."DocEntry" IN (SELECT top 1 Data FROM split(:docentry,',')) AND T2."DocStatus"='O'),0) into docentry from DUMMY;	
	ELSE
		select ifnull((SELECT TOP 1 "TblName" FROM apz_tblobject a WHERE a."ObjType"=:fromtype),'') into tblname from DUMMY;
		Select ifnull((SELECT TOP 1 "TblLine" FROM apz_tblobject a WHERE a."ObjType"=:fromtype),'') into tblline from DUMMY;
	END if;

	select ifnull((SELECT  TOP 1  "TblName" FROM apz_tblobject a WHERE a."ObjType"=:totype),'') into tblTo from DUMMY;
	
	select ifnull((SELECT "MainCurncy" FROM OADM),'') into MainCurrency from DUMMY;
	
	IF :bplId=-1
	then
		select ifnull((SELECT top 1 "DfltSeries" FROM ONNM WHERE "ObjectCode"=:totype),-1) into Series from dummy; 
	ELSE 
		select ifnull((SELECT TOP 1 "Series" FROM NNM1 WHERE "ObjectCode"=:totype AND 
						"BPLId" IN (SELECT "BPLId" FROM USR6 A INNER JOIN OUSR B ON A."UserCode"=b.USER_CODE WHERE b.USERID IN (SELECT USERID FROM OUSR WHERE USER_CODE=:username) OR ifnull("BPLId",-1)=-1)),-1) into Series from DUMMY;
	END if;

	select ifnull((SELECT "NextNumber" FROM NNM1 WHERE "ObjectCode"=:totype AND ifnull("Series",-1)=:Series and "IsManual" = 'N'),(SELECT top 1 "AutoKey" FROM ONNM WHERE "ObjectCode"=:totype)) into DocNum from DUMMY;
	
	IF :fromtype='1250000025' or :fromtype='1250000026'	-- Blanket Agreement
	then
		sqlQuery=N'SELECT  0 AS "DocEntry"
		, a."AbsID" AS "DocNum"
		, a."BpCode" AS "CardCode"
		, a."BpName" AS "CardName"
		, a."GroupNum"
		, current_date AS "DocDate"
		, current_date AS "DocDueDate"
		, current_date AS "TaxDate"
		, current_date AS "DocDate"
		, ' || :totype || N' AS "ObjType"
		, a."AbsID" AS "AgrNo"
		, cast(null as numeric(19,6)) AS "TotalBeforeDis"
		, cast(null as numeric(19,6)) AS "Total"
		, cast(null as numeric(19,6)) AS "TaxTotal"
		, cast(null as numeric(19,6)) AS "TotalFreight"
		, cast(null as numeric(19,6)) AS "DPMTotal"
		, CASE 
							WHEN C."Currency"=''##'' then ''' || :MainCurrency || N'''
							WHEN C."Currency"<>''##'' THEN C."Currency" END as "DocCur"
		, ifnull((SELECT ifnull(x."Rate",1) FROM ORTT x WHERE x."RateDate"=CAST(current_date AS DATE) AND x."Currency" = c."Currency"),1) AS "DocRate"
		, D.* 
						,ifnull(B."Name",'''') "ContactPersonName"
						,CASE 
							WHEN C."Currency"=''##'' then ''' || :MainCurrency || N'''
							WHEN C."Currency"<>''##'' THEN C."Currency" END as "Currency", c."ListNum" "InfoPriceList"
			FROM OOAT A 
									LEFT JOIN OQUT d on d."DocEntry" = 0
									LEFT JOIN OCRD C on A."BpCode"=C."CardCode" 
									LEFT JOIN OCPR B on A."BpCode"=B."CardCode" and A."CntctCode"=B."CntctCode"
										where A."AbsID" IN (SELECT top 1 Data FROM split(''' || :docentry || N''','',''));';
		
		execute immediate :sqlQuery;
		RETURN;
	END if;

	sqlQuery='DO
	BEGIN
	create local temporary table #tbl as (SELECT A.*  ';
	
	If :fromtype='112'	-- oDrafts
	then
		sqlQuery= :sqlQuery || ', cast(1 as tinyint) "IsDraft"';
	END if;

	sqlQuery= :sqlQuery || ' ,(select "lastName" || '', '' || "firstName" from ohem where "empID"=A."OwnerCode") as "OwnerName"
					,CASE WHEN ifnull(A."TotalExpFC",0)>0 THEN A."TotalExpFC" ELSE ifnull(A."TotalExpns",0) END "TotalFreight"
					,(SELECT SUM(case when "TotalFrgn">0 then "TotalFrgn" else "LineTotal" end) FROM ' || :tblLine || ' B WHERE B."DocEntry"=A."DocEntry") "TotalBeforeDis"
					,ifnull(B."Name",'''') "ContactPersonName" 
					,CASE WHEN ifnull(A."VatSumFC",0) >0 THEN A."VatSumFC" ELSE ifnull(A."VatSum",0) END "TaxTotal"
					,CASE WHEN IfNULL(A."DpmAmntFC",0) >0 THEN IfNULL(A."DpmAmntFC",0) ELSE IfNULL(A."DpmAmnt",0) END "DPMTotal"
					,CASE WHEN ifnull(A."DocTotalFC",0)>0 THEN A."DocTotalFC" ELSE ifnull(A."DocTotal",0) END "Total"
					,CASE WHEN A."DiscSumFC">0 THEN A."DiscSumFC" ELSE A."DiscSum" END "Discount"
					,CASE 
						WHEN C."Currency"=''##'' then ''' || :MainCurrency || '''
						WHEN C."Currency"<>''##'' THEN C."Currency" END as "Currency", ifnull(c."ListNum",' || :defaultPriceList || ') "InfoPriceList"
		 FROM ' || :tblname || ' A LEFT JOIN OCRD C on A."CardCode"=C."CardCode" 
									LEFT JOIN OCPR B on A."CardCode"=B."CardCode" and A."CntctCode"=B."CntctCode"
									where A."DocEntry" IN (SELECT top 1 Data FROM split(''' || :docentry || ''','','')));
			
		Update #tbl set "Series"=' || cast(:Series as NVARCHAR(11)) ||'	
						,"DocStatus"=''O'';';
					
	If :fromtype<>'112'	-- oDrafts
	then
		sqlQuery= :sqlQuery || 'Update #tbl set "DocNum" =' || cast(:DocNum as NVARCHAR(11)) ||'
						,"DocDate"=current_Date
						,"DocDueDate"=current_Date
						,"draftKey"=NULL
						,"TaxDate"=current_Date
						,"ObjType"=' || :totype || ';';
	END if;
	
	select IFNULL((select 1 from ADP1 where "ObjType" = :fromtype and "CpyCVRef" = 'Y'),0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN

		sqlQuery= :sqlQuery || N'
					Update #tbl set "NumAtCard" = (select string_agg("NumAtCard",'';'') from ' || :tblname || ' where "DocEntry" IN (SELECT Data FROM split(''' || :docentry || ''','','')) and ifnull("NumAtCard",'''') <> '''');';

	END IF;
						

	IF :fromtype='112'	-- oDrafts
	then
		sqlQuery= :sqlQuery || ' Update #tbl set "draftKey"=' || :docentry || ';';
		sqlQuery= :sqlQuery || ' Update #tbl set "DocEntry"=0;';
		
	ELSE
		sqlQuery= :sqlQuery || ' Update #tbl set "DocDate"=current_Date
										,"DocDueDate"=current_Date
										,"TaxDate"=current_Date
										,"DocEntry"=0
										,"WddStatus"=''-'';';
	END	if;
	
	
	if :toType = '60' or :toType = '59' then
		sqlQuery= :sqlQuery || '
		update #tbl set "Branch"="BPLId";';
	end if;
	
	sqlQuery= :sqlQuery || '
	update #tbl set "DiscPrcnt"=0 where (select count(distinct "DiscPrcnt") from ' || :tblname || ' where "DocEntry" in (SELECT Data FROM split(''' || :docentry || ''','',''))) > 1;';
	
	sqlQuery= :sqlQuery || ' 
		END';
	--PRINT @SQL
	exec :sqlQuery;
	EXECUTE immediate 'select * from #tbl;';
	--select :sqlQuery from DUMMY;

END