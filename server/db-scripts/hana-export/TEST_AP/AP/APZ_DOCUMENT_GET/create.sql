
CREATE PROCEDURE APZ_Document_Get
(
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
	in ReqType INT default 0
)
AS
BEGIN
	declare userId int;
	DECLARE ObjType NVARCHAR(20);
	DECLARE SQLQuery NVARCHAR(2000)='';
	declare mainCur nvarchar(3);
	
	----- biến để lấy giá trị default price list. để set cho info price trong trường hợp các phiếu không có BP
	declare defaultPriceList int = -1;
	select ifnull((select top 1 "ListNum" from OPLN order by "ListNum"),-1) into defaultPriceList FROM DUMMY;
	select (SELECT TOP 1 a."ObjType" FROM Apz_TblObject a WHERE a."TblName"=:tbl) into ObjType from DUMMY;
	select ifnull((select USERID from OUSR where USER_CODE = :UserName), 0) into userId from DUMMY;
	
	select "MainCurncy" into mainCur from OADM;
	
	SQLQuery='create local temporary table #tempDoc as (select top 0 *
			,cast(null as numeric(21,6)) "Total"
			,cast(null as numeric(21,6)) "TotalFreight"
			,cast(null as numeric(21,6)) "TotalBeforeDis"
			,cast(null as numeric(21,6)) "TaxTotal"
			,cast(null as numeric(21,6)) "DPMTotal"
			,cast(null as numeric(21,6)) "Discount"
			,cast(null as nvarchar(20)) "DocStatusName"
	 FROM ' || :tbl || ');
	insert into #tempDoc
			SELECT A.*
			,CASE WHEN A."DocCur"=''' || :mainCur || ''' THEN A."DocTotal" ELSE A."DocTotalFC" END
			,CASE WHEN A."DocCur"=''' || :mainCur || ''' THEN A."TotalExpns" ELSE A."TotalExpFC" END
			,CASE WHEN A."DocCur"=''' || :mainCur || ''' THEN (SELECT ifnull(SUM("LineTotal"),0) FROM ' || :tblLine || ' B WHERE B."DocEntry"=A."DocEntry" AND B."TaxOnly"=''N'') ELSE (SELECT ifnull(SUM("TotalFrgn"),0) FROM ' || :tblLine || ' B WHERE B."DocEntry"=A."DocEntry" AND B."TaxOnly"=''N'') END
			,CASE WHEN A."DocCur"=''' || :mainCur || ''' THEN A."VatSum" ELSE A."VatSumFC" END
			,CASE WHEN A."DocCur"=''' || :mainCur || ''' THEN A."DpmAmnt" ELSE A."DpmAmntFC" END
			,CASE WHEN A."DocCur"=''' || :mainCur || ''' THEN A."DiscSum" ELSE A."DiscSumFC" END
			,CASE WHEN A."DocStatus" =''O'' then ''Open'' WHEN  A."DocStatus"  = ''C'' AND (A."CANCELED" =''N'' OR A."CANCELED" ='''' OR A."CANCELED" IS NULL OR A."CANCELED" =''C'') THEN ''Closed'' WHEN A."DocStatus"  = ''C'' AND A."CANCELED" =''Y'' then ''CANCELED'' END
	 		FROM ' || :tbl || ' A where (IFNULL(A."OwnerCode",-1)=-1 OR A."OwnerCode" IN (select distinct x."OwnerId" from  :OwnerView x))';
	IF :DocEntry <> 0
                then	
                    SQLQuery = :SQLQuery || ' AND A."DocEntry"='''|| cast(:DocEntry as NVARCHAR) || '''';
            ELSEIF :DocNum <> 0
                then	
                    SQLQuery = :SQLQuery || ' AND A."DocNum"=''' || cast(:DocNum as nvarchar) || '''';       
            ELSE        
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
                    ELSEIF :DocStatus = 'D' then
                    		SQLQuery = :SQLQuery || ' AND A."InvntSttus" = ''C'' AND A."DocStatus"=''O''';
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
                
            -- Dùng luôn :isIns để phân biệt Down payment request và Down payment invoice
            IF :isIns <> '' then
            	if :tbl = 'OPCH' OR :tbl = 'OINV' then
                    SQLQuery = :SQLQuery || ' AND A."isIns"=''' || :isIns || '''';
                elseif :tbl = 'ODPO' OR :tbl = 'ODPI' then
                	SQLQuery = :SQLQuery || ' AND A."Posted"=''' || :isIns || '''';
                END if;
            end if;
            SQLQuery = :SQLQuery || '
            AND (ifnull(A."BPLId", -1) = -1 or A."BPLId" in (select x."BPLId" from OBPL x inner join USR6 y on x."BPLId" = y."BPLId" where x."Disabled" = ''N'' and y."UserCode" = ''' || ifnull(:UserName, '') || '''));';

     sqlQuery =  'DO BEGIN' || '
     	call apz_func_OwnerOverShip(N''' || ifnull(:UserName, '') || ''',''' || :ObjType || ''', OwnerView);' || '
     ' || :sqlQuery || '
     end';
     
     exec :SQLQuery;
     
	 SQLQuery = 'SELECT A.*,b."Currency",c."Name" "ContactPersonName",(select "lastName" || '', '' || "firstName" from ohem where "empID"=A."OwnerCode") as "OwnerName", ifnull(b."ListNum", ' || :defaultPriceList || ') "InfoPriceList" FROM #tempDoc A 
			 LEFT JOIN ocrd b ON a."CardCode"=b."CardCode"
			 LEFT JOIN ocpr c ON a."CardCode"=c."CardCode" AND A."CntctCode"=c."CntctCode"
			 left join ohem on a."OwnerCode"=ohem."empID"
		 ORDER BY A."DocEntry" desc';
	
	execute immediate :sqlquery;
	exec 'DROP TABLE #tempDoc;';
 END

