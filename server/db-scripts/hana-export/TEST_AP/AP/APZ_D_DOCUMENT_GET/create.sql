
-- APZ_D_Document_Get 'OQUT','QUT1',@DocStatus='O'
CREATE PROCEDURE APZ_D_Document_Get(
    in tbl NVARCHAR(10) ,
    in tblLine NVARCHAR(10) ,
    in DocEntry INT default 0 ,
    in DocDate timestamp default NULL ,
    in EndDate timestamp default NULL ,
    in DocStatus NVARCHAR(1) default '' ,
    in Canceled NVARCHAR(1) default '' ,
    in CardCode NVARCHAR(50) default '' ,
    in UserName NVARCHAR(50) default '' ,
    in isIns NVARCHAR(1) default 'N',	
	in Branch int default -1,
	in DocNum INT default 0,
	in Requester NVARCHAR(10) default '',
	in ReqType INT default 0,
	in pageIndex int default 1,
	-- bỏ qua việc check authorization R hoặc O chỉ lấy theo usersign
	in skipAuthorization nvarchar(10) default '', 
	in DMSSubType nvarchar(50) default '',
	in searchText NVARCHAR(254) default '')
AS
BEGIN
	declare userId int;
	DECLARE ObjType NVARCHAR(20);
	DECLARE SQLQuery NVARCHAR(50000)='';
	DECLARE SQLCreateTable NVARCHAR(50000)='';
	DECLARE SQLFilterStore NVARCHAR(50000)='';
	declare authMode NVARCHAR(10) = ifnull(ifnull((select "AuthorizationMode" from APZ_D_OAPC),''), 'R');
	
	DECLARE SQLConditionCheckAuth NVARCHAR(50000)='';
	
	declare lowLimit int;
	declare upLimit int;
	----- biến để lấy giá trị default price list. để set cho info price trong trường hợp các phiếu không có BP
	declare defaultPriceList int = -1;
	select ifnull((select top 1 "ListNum" from OPLN order by "ListNum"),-1) into defaultPriceList FROM DUMMY;
	select (SELECT TOP 1 a."ObjType" FROM Apz_TblObject a WHERE a."TblName"=:tbl) into ObjType from DUMMY;
	select ifnull((select USERID from OUSR where USER_CODE = :UserName), 0) into userId from DUMMY;
	lowLimit = 20 * :pageIndex - 20;
	upLimit = :pageIndex * 20;
	
	SQLConditionCheckAuth = '
	declare conditionCheckAuth NVARCHAR(10)='''';
	select case when ('''||:skipAuthorization||''' = ''Y'') then 1 when ('''||:authMode||''' = ''R'' and '''||:tbl||''' in (''OINV'',''ORDR'',''ODLN'')) then 2 else 3 end as "Mode" into conditionCheckAuth from dummy;';


	SQLCreateTable='create local temporary table #tempDoc as (select top 0 cast(0 as int) "RowNum",A.*
			,cast(0 as numeric(21,6)) "Total"
			,cast(0 as numeric(21,6)) "TotalFreight"
			,cast(0 as numeric(21,6)) "TotalBeforeDis"
			,cast(0 as numeric(21,6)) "TaxTotal"
			,cast(0 as numeric(21,6)) "DPMTotal"
			,cast(null as nvarchar(20)) "DocStatusName"
	 FROM ' || :tbl || ' A where 1=2);';
	 
	 SQLFilterStore='create local temporary table #tmpStore("CardCode" nvarchar(50), "CardName" nvarchar(254));
	call APZ_D_FilterStores('''','''',''S'','''||:UserName||''',''#tmpStore'');';
	 
	SQLQuery='insert into #tempDoc select * from (
			SELECT ROW_NUMBER() OVER (ORDER BY A."DocDate" desc) "RowNum",A.*
			,CASE WHEN IFNULL(A."DocTotalFC",0)>0 THEN IFNULL(A."DocTotalFC",0) ELSE IFNULL(A."DocTotal",0) END "Total"
			,CASE WHEN IFNULL(A."TotalExpFC",0)>0 THEN IFNULL(A."TotalExpFC",0) ELSE IFNULL(A."TotalExpns",0) END "TotalFreight"
			,CASE WHEN IFNULL(A."DocTotalFC",0)>0 THEN (SELECT SUM("TotalFrgn") FROM ' || :tblLine || ' B WHERE B."DocEntry"=A."DocEntry") ELSE (SELECT SUM("LineTotal") FROM ' || :tblLine || ' B WHERE B."DocEntry"=A."DocEntry") END "TotalBeforeDis"
			,CASE WHEN IFNULL(A."VatSumFC",0) >0 THEN IFNULL(A."VatSumFC",0) ELSE IFNULL(A."VatSum",0) END "TaxTotal"
			,CASE WHEN IFNULL(A."DpmAmntFC",0) >0 THEN IFNULL(A."DpmAmntFC",0) ELSE IFNULL(A."DpmAmnt",0) END "DPMTotal"
			,CASE WHEN A."DocStatus" =''O'' then ''Open'' WHEN  A."DocStatus"  = ''C'' AND (A."CANCELED" =''N'' OR A."CANCELED" ='''' OR A."CANCELED" IS NULL OR A."CANCELED" =''C'') THEN ''Closed'' WHEN A."DocStatus"  = ''C'' AND A."CANCELED" =''Y'' then ''CANCELED'' END AS "DocStatusName"
	 FROM ' || :tbl || ' A where 1=1';
			IF :DocEntry <> 0
                then	
                    SQLQuery = :SQLQuery || ' AND A."DocEntry"='''|| cast(:DocEntry as NVARCHAR) || '''';
            ELSEIF :DocNum <> 0
                then	
                    SQLQuery = :SQLQuery || ' AND A."DocNum"=''' || cast(:DocNum as nvarchar) || '''';      
            ELSE     
        		   IF :searchText <> ''
                    then 
                        SQLQuery = :SQLQuery || ' AND (cast(A."DocEntry" as nvarchar(20)) like 
                        	N''%' || :searchText || '%''
                        	or (cast(A."DocNum" as nvarchar(20))) like N''%' || :searchText || '%'' 
							or A."CardCode" like N''%' || :searchText || '%'' 
							or A."Address" like N''%' || :searchText || '%'' 
							or A."CardName" like N''%' || :searchText || '%''
							or A."Address2" like N''%' || :searchText || '%''
							or A."Comments" like N''%' || :searchText || '%'')';
                    END if;
                    IF :DocDate IS NOT NULL
                        then 
                            SQLQuery = :SQLQuery || ' AND A."DocDate">='''|| cast(cast(:DocDate as date) as nvarchar) || '''';
                        END if;
                    IF :EndDate IS NOT NULL
                        then 
                            SQLQuery = :SQLQuery || ' AND A."DocDate"<='''|| cast(cast(:EndDate as date) as nvarchar) || '''';
                        END if;
                   
                    IF :Canceled = 'Y' and :DocStatus <> 'A'
                        then
                            SQLQuery = :SQLQuery || ' AND A."CANCELED"=''Y''';
                        
					ELSEIF :DocStatus <> '' and :DocStatus <> 'A'
                        then
                            SQLQuery = :SQLQuery || ' AND A."DocStatus"=''' || :DocStatus || ''' AND A."CANCELED" IN (''N'',''C'')';
                        END if;
                    IF :CardCode <> ''
                        then
                            SQLQuery = :SQLQuery || ' AND A."CardCode"=N''' || :CardCode || '''';
                        END if;
					IF :ReqType > 0
                        then
                            SQLQuery = :SQLQuery || ' AND A."ReqType"=''' || cast(:ReqType as nvarchar) || '''';

							IF :Requester <> ''
							then
								SQLQuery = :SQLQuery || ' AND A."Requester"=N'''  ||  :Requester ||  '''';
							END if;
                        END if;
						
                END if;
                
            IF :isIns <> '' AND ( :tbl = 'OPCH' OR :tbl = 'OINV')
                then
                    SQLQuery = :SQLQuery || ' AND A."isIns"=''' || :isIns || '''';
                END if;
            SQLQuery = :SQLQuery ||
            
             
            ' AND (
            (:conditionCheckAuth = 1 and A."UserSign" = ' || cast(:userId as nvarchar) || ')
            OR
            ( :conditionCheckAuth = 2 and '''||:tbl||''' in (''OINV'',''ORDR'',''ODLN'') And A."CardCode" in (select "CardCode" from #tmpStore))
            OR
            (:conditionCheckAuth = 3 and '''||:tbl||''' not in (''OINV'',''ORDR'',''ODLN'') AND ((A."OwnerCode" IN (select distinct x."OwnerId" from  :OwnerView x)) OR IFNULL(A."OwnerCode",-1)=-1 or 
            a."UserSign" = ' || cast(:userId as nvarchar) || ')))
            ) PagingDocument
            where ('|| :pageIndex|| ' = -1 or (PagingDocument."RowNum" > '||:lowLimit|| ' AND PagingDocument."RowNum" <=' || :upLimit || '));';

     --exec :SQLCreateTable;
     sqlQuery =  'DO BEGIN' || '
     ' || :SQLConditionCheckAuth || '
     ' || :sqlCreateTable || '
     ' || :SQLFilterStore || '
     	call apz_func_OwnerOverShip(N''' || ifnull(:UserName, '') || ''',''' || :ObjType || ''', OwnerView);' || '
     ' || :sqlQuery || '
     end';
     exec :SQLQuery;
     
	 SQLQuery = ' SELECT A.*,b."Currency",c."Name" "ContactPersonName",(select "lastName" || '', '' || "firstName" from ohem where "empID"=A."OwnerCode") as "OwnerName", ifnull(b."ListNum", ' || :defaultPriceList || ') "InfoPriceList" FROM #tempDoc A 
			 LEFT JOIN ocrd b ON a."CardCode"=b."CardCode"
			 LEFT JOIN ocpr c ON a."CardCode"=c."CardCode" AND A."CntctCode"=c."CntctCode"
			 left join ohem on a."OwnerCode"=ohem."empID"
			 WHERE ifnull(A."BPLId", -1) = -1 or A."BPLId" in (select x."BPLId" from OBPL x inner join USR6 y on x."BPLId" = y."BPLId" where x."Disabled" = ''N'' and y."UserCode" = ''' || ifnull(:UserName, '') || ''')
		 ORDER BY A."RowNum" asc;';
	execute immediate :sqlquery;

	exec 'DROP TABLE #tempDoc;';
	exec 'DROP TABLE #tmpStore;';
 END


