
CALL APZ_EXISTSPROC('APZ_D_GetDocDraft');

GO

create PROCEDURE APZ_D_GetDocDraft
(
	in UserName nvarchar(25),
	in ObjType Nvarchar(254) default '0',
	in UserID nvarchar(30) default '',
	in OpenOnly tinyint default 0,
	in Date tinyint default 0,
	in TypeDate int default 0,
	in FromValue nvarchar(50) default '',
	in ToValue nvarchar(50) default '',
	in pageIndex int default -1,
	in searchText nvarchar(254) default ''
)
As 
BEGIN
	DECLARE SQLValue NVARCHAR(10000) ='';
	DECLARE cols  NVARCHAR(5000) = '';
	DECLARE DATE_TYPE NVARCHAR(500)='';
	declare authMode nvarchar(5) = '';
	declare checkAuthQuery nvarchar(5000);
	declare loginUserID int = ifnull((select USERID from OUSR where USER_CODE=:UserName), -1);
	declare udfCreate nvarchar(5000);
	declare SQLFilterStore nvarchar(5000);
	declare lowLimit int;
	declare upLimit int;
	
	lowLimit = 20 * :pageIndex - 20;
	upLimit = :pageIndex * 20;
	SQLFilterStore='create local temporary table #tmpStore("CardCode" nvarchar(50), "CardName" nvarchar(254));
	call APZ_D_FilterStores('''','''',''S'','''||:UserName||''',''#tmpStore'');';
	select top 1 "AuthorizationMode" into authMode from APZ_D_OAPC;
	
	--check authorization
	if :authMode = 'R'
	then
		checkAuthQuery = ' AND (a."CardCode" is null or a."CardCode" in (select "CardCode" from #tmpStore))';
	else 
		IF(UserID <>'' AND UserID<>'0')
		then
			checkAuthQuery = ' AND a."UserSign"='''||ifnull(:UserID,'')||'''';
		else
			checkAuthQuery = ' AND (a."UserSign"=' || cast(:loginUserID as varchar) || ' or a."CardCode" is null or ifnull(b."OwnerCode",-1)=-1 or b."OwnerCode" in (select distinct "OwnerId" from  :OwnerView))';
		END if;
	end if;
	SQLValue:= 'call apz_func_OwnerOverShip(N''' || ifnull(:UserName, '') || ''',''2'', OwnerView);' || '
	create local temporary table #tblResult as 
	(select top 0 cast(0 as int) "RowNum",cast(null as nvarchar(50)), a.*
	,cast(null as nvarchar(20)) "ReqTypeName"
	,cast(0 as numeric(21,6)) "ListNum"
	,cast(null as nvarchar(20)) "ListName"
	,cast(0 as numeric(21,6)) "Total"
	,cast(0 as numeric(21,6)) "TotalFreight"
	,cast(0 as numeric(21,6)) "TotalBeforeDis"
	,cast(0 as numeric(21,6)) "TaxTotal"
	,cast(0 as numeric(21,6)) "DPMTotal"
	,cast(null as nvarchar(20)) "DocStatusName"
	 FROM ODRF a where 1=2);
	insert into #tblResult
	 select * from (select 
	ROW_NUMBER() OVER (ORDER BY a."DocDate" desc) "RowNum",
	case a."ObjType"
	when 21 then ''Goods Return'' 
	when 23 then ''Sales Quotation'' 
	when 17 then ''Sales Order''
	when 15 then ''Delivery''
	when 16 then ''Return''
	when 203 then ''A/R Down Payment''
	when 13 then ''A/R Invoice''
	when 14 then ''A/R Credit Memo''
	when 1470000113 then ''Purchase Request''
	when 540000006 then ''Purchase Quotation''
	when 22 then ''Purchase Order''
	when 20 then ''Goods Receipt P/O''
	when 204 then ''A/P Down Payment''
	when 18 then ''A/P Invoice''
	when 19 then ''A/P Credit Memo''
	when 59 then ''Goods Receipt''
	when 60 then ''Goods Issue''
	when 1250000001 then ''Inventory Transfer Request''
	when 67 then ''Inventory Transfer''
	else ''''
	end as "Document", a.*
	,CASE "ReqType"
		WHEN 12 then ''User'' 
		WHEN 171 then ''Employee''
	else '''' END as "ReqTypeName",
	ifnull(a."GroupNum",-1) as "ListNum" , 
	ifnull((select "ListName" from OPLN Where a."GroupNum" = OPLN."ListNum" ),'''') as "ListName"
	,CASE WHEN IFNULL(a."DocTotalFC",0)>0 THEN IFNULL(a."DocTotalFC",0) ELSE IFNULL(a."DocTotal",0) END "Total"
	,CASE WHEN IFNULL(a."TotalExpFC",0)>0 THEN IFNULL(a."TotalExpFC",0) ELSE IFNULL(a."TotalExpns",0) END "TotalFreight"
	,CASE WHEN IFNULL(a."DocTotalFC",0)>0 THEN (SELECT SUM("TotalFrgn") FROM DRF1 B WHERE B."DocEntry"=a."DocEntry") ELSE (SELECT SUM("LineTotal") FROM DRF1 B WHERE B."DocEntry"=a."DocEntry") END "TotalBeforeDis"
	,CASE WHEN IFNULL(a."VatSumFC",0) >0 THEN IFNULL(a."VatSumFC",0) ELSE IFNULL(a."VatSum",0) END "TaxTotal"
	,CASE WHEN IFNULL(a."DpmAmntFC",0) >0 THEN IFNULL(a."DpmAmntFC",0) ELSE IFNULL(a."DpmAmnt",0) END "DPMTotal"
	,CASE WHEN a."DocStatus" =''O'' then ''Open'' WHEN  a."DocStatus"  = ''C'' AND (a."CANCELED" =''N'' OR a."CANCELED" ='''' OR a."CANCELED" IS NULL OR a."CANCELED" =''C'') THEN ''Closed'' WHEN a."DocStatus"  = ''C'' AND a."CANCELED" =''Y'' then ''CANCELED'' END AS "DocStatusName"';
	
	SQLValue := :SQLValue ||' from ODRF a WHERE 1=1 ';
	
	
	IF(ObjType<>'')
	then
		SQLValue:= :SQLValue || ' AND COMPARESPLIT(a."ObjType",'''||ifnull(:ObjType,'')||''', '','')=1 ';
	END if;
	SQLValue:= :SQLValue || :checkAuthQuery;
	IF(OpenOnly=1)
	then
		SQLValue:=:SQLValue || ' AND "DocStatus"=''O'' ';
	END if;
	--search text
	IF (:searchText <> '')
    then
        SQLValue := :SQLValue || ' AND (cast(A."DocEntry" as nvarchar(20)) like 
        	N''%' || :searchText || '%''
        	or (cast(A."DocNum" as nvarchar(20))) like N''%' || :searchText || '%'' 
			or A."CardCode" like N''%' || :searchText || '%'' 
			or A."CardName" like N''%' || :searchText || '%'')';
	end if;
	--Date
	IF (:Date = 1) THEN 
		IF (:TypeDate = 0) THEN 
			DATE_TYPE := 'a."CreateDate"';
		ELSE 
			IF (:TypeDate = 1) THEN 
				DATE_TYPE := 'a."UpdateDate"';
			END IF;
		END IF;
		IF (:TypeDate = 2) THEN 
			DATE_TYPE := 'a."DocDate"';
		END IF;
		IF (:TypeDate = 3) THEN 
			DATE_TYPE := 'a."DocDueDate"';
		END IF;
		IF (:TypeDate = 4) THEN 
			DATE_TYPE := 'a."TaxDate"';
		END IF;
		IF (:DATE_TYPE <> '') THEN 
			IF (:FromValue <> '') THEN 
				SQLValue := :SQLValue || ' AND CAST(' || :DATE_TYPE || ' as date)>=CAST(''' || ifnull(:FromValue,'') || ''' as date) ';
			END IF;
			IF (:ToValue <> '') THEN 
				SQLValue := :SQLValue || ' AND CAST(' || :DATE_TYPE || ' as date)<=CAST(''' ||ifnull(:ToValue,'')  || ''' as date) ';
			END IF;
		END IF;
	END IF;
	SQLValue:=:SQLValue || ')';
	-- Inventory Posting Draft
	IF( COMPARESPLIT('10000071',:ObjType, ',')=1) then
		SQLValue:=:SQLValue||'UNION (select ''Inventory Posting'' as "Document","DocEntry","DocNum",null as "DocDate",null as "CardCode",null as "DocTotal",null as "Comments","ObjType" from OIPD Where 1=1 ';
	
		IF(UserID <>'' AND UserID<>'0') then
			SQLValue:=:SQLValue || ' AND "UserSign"='''||ifnull(:UserID,'')||'''';
		END if;
		IF(OpenOnly=1)
		then
			SQLValue:=:SQLValue||' AND Status=''O'' ';
		END if;
		-- Date
		DATE_TYPE := '';
		IF (:Date = 1) THEN 
			IF (:TypeDate = 0) THEN 
				DATE_TYPE := 'CreateDate';
			ELSE 
				IF (:TypeDate = 1) THEN 
					DATE_TYPE := 'UpdateDate';
				END IF;
			END IF;
			IF (:TypeDate = 2) THEN 
				DATE_TYPE := 'DocDate';
			END IF;
			IF (:DATE_TYPE <> '') THEN 
				IF (:FromValue <> '') THEN 
					SQLValue := :SQLValue || ' AND cast("' || :DATE_TYPE || '" as date)>=Cast(''' || ifnull(:FromValue,'') || ''' as date) ';
				END IF;
				IF (:ToValue <> '') THEN 
					SQLValue := :SQLValue || ' AND cast("' || :DATE_TYPE || '" as date)<=Cast(''' ||ifnull(:ToValue,'')|| ''' as date) ';
				END IF;
			END IF;
		END IF;
		SQLValue := :SQLValue || ')';
	END IF;
	-- Inventory opening balance Draft
	IF(COMPARESPLIT('310000001',:ObjType, ',')=1) then
		SQLValue:=:SQLValue || 'UNION (select ''Inventory Opening Balance'' as "Document","DocEntry","DocNum","DocDate",Null as "CardCode","DocTotal","Comments","ObjType" from OIOD Where 1=1 ';
		IF(UserID <>'' AND UserID<>'0') then
			SQLValue:=:SQLValue || ' AND "UserSign"='''||ifnull(:UserID,'')||'''';
		END if;
		IF(OpenOnly=1) then
			SQLValue:=:SQLValue|| ' AND "Status"=''O'' ';
		END if;
	
		-- Date
		DATE_TYPE := '';
		IF (:Date = 1) THEN 
			IF (:TypeDate = 0) THEN 
				DATE_TYPE := 'CreateDate';
			ELSE 
				IF (:TypeDate = 1) THEN 
					DATE_TYPE := 'UpdateDate';
				END IF;
			END IF;
			IF (:TypeDate = 2) THEN 
				DATE_TYPE := 'DocDate';
			END IF;
			IF (:DATE_TYPE <> '') THEN 
				IF (:FromValue <> '') THEN 
					SQLValue := :SQLValue || ' AND cast("' || :DATE_TYPE || '" as date)>=Cast(''' ||ifnull(:FromValue,'') || ''' as date) ';
				END IF;
				IF (:ToValue <> '') THEN 
					SQLValue := :SQLValue || ' AND cast("' || :DATE_TYPE || '" as date)<=Cast(''' || ifnull(:ToValue,'') || ''' as date) ';
				END IF;
			END IF;
		END IF;
		SQLValue := :SQLValue || ')';
	END IF;
	-- Inventory Counting Draft
	IF(COMPARESPLIT('1470000065',:ObjType,',')=1)
	then
		SQLValue:=:SQLValue||'UNION (select ''Inventory Counting'' as "Document","DocEntry","DocNum",null as "DocDate",null as "CardCode",null as "DocTotal",null as "Comments","ObjType" from OICD Where 1=1 ';
	
		IF(UserID <>'' AND UserID<>'0')
		then
			SQLValue:=:SQLValue||' AND "UserSign"='''||ifnull(:UserID,'')||'''';
		END if;
		IF(OpenOnly=1)
		then
			SQLValue:=:SQLValue||' AND "Status"=''O'' ';
		END if;
	
		-- Date
		DATE_TYPE:='';
		IF(Date=1) then
			IF(TypeDate=0)
			then
				DATE_TYPE:='CreateDate';	
			ELSE
				IF(TypeDate=1) then
					DATE_TYPE:='UpdateDate';
				END IF;
			END IF;
			IF(DATE_TYPE<>'') then
				IF(FromValue<>'') then
					SQLValue:=:SQLValue||' AND CAST("'||:DATE_TYPE||'" as date)>=CAST('''||ifnull(:FromValue,'')||''' as date) ';
				END if;
				IF(ToValue<>'') then
					SQLValue:=:SQLValue||' AND CAST("'||:DATE_TYPE||'" as date)<=CAST('''||ifnull(:ToValue,'')||''' as date) ';
				END if;
			END if;
		END if;
		SQLValue:=:SQLValue||')';
	END IF;
	--pagination
	SQLValue:=:SQLValue||' PagingDocument where ('|| :pageIndex|| ' = -1 or (PagingDocument."RowNum" > '||:lowLimit|| ' AND PagingDocument."RowNum" <=' || :upLimit || '))';
	sqlValue = 'do begin
	' ||:SQLFilterStore|| '
	' || :sqlValue || ';
	end;';
	--select :sqlValue from dummy;
	--return;
	--print @SQLValue
	exec :SQLValue;
	
	execute immediate 'select * from #tblResult order by "RowNum" asc';
	exec 'drop table #tblResult';
END;