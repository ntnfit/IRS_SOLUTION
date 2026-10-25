
create PROCEDURE SP_APZ_GetDocDraft
(
	in UserName nvarchar(25),
	ObjType Nvarchar(254) default '0',
	UserID nvarchar(30) default '',
	OpenOnly tinyint default 0,
	Date tinyint default 0,
	TypeDate int default 0,
	FromValue nvarchar(50) default '',
	ToValue nvarchar(50) default ''
)
As 
BEGIN
	DECLARE SQLValue NVARCHAR(10000) ='';
	DECLARE cols  NVARCHAR(5000) = '';
	DECLARE DATE_TYPE NVARCHAR(500)='';
	declare loginUserID int = ifnull((select USERID from OUSR where USER_CODE=:UserName), -1);
	declare udfCreate nvarchar(5000);
	
	select string_agg('"' || column_name || '" ' || data_type_name || case when data_type_name = 'DECIMAL' then '(' || cast(length as varchar) || ',' || cast(scale as varchar) || ')'
																	   when data_type_name in ('NVARCHAR', 'VARCHAR', 'CHAR', 'NCHAR') then '(' || cast(length as varchar) || ')'
																	   else '' end, ',') into udfCreate
	from table_columns where schema_name = current_schema and table_name = 'ODRF' and substring(column_name, 1, 2) = 'U_';
	
	if ifnull(udfCreate,'') != '' then
		udfCreate = ',' || udfCreate;
	end if;
	
	SQLValue:= 'call apz_func_OwnerOverShip(N''' || ifnull(:UserName, '') || ''',''2'', OwnerView);' || '
	create local temporary table #tblResult (
		"Document" nvarchar(30),
		"DocEntry" int,
		"DocNum" int,
		"DocDate" date,
		"DocTime" smallint,
		"CardCode" nvarchar(15),
		"CardName" nvarchar(100),
		"DocTotal" decimal(21,6),
		"Comments" nvarchar(254),
		"ObjType" nvarchar(20),
		"Requester" nvarchar(25),
		"ReqName" nvarchar(155),
		"ReqType" int,
		"ReqTypeName" nvarchar(10),
		"CANCELED" varchar(1),
		"UserSign" int,
		"ReqDate" date,
		"RequriedDate" date,
		"DocStatus" varchar(1),
		"WddStatus" varchar(1),
		"ListNum" int,
		"ListName" nvarchar(32)' || udfCreate ||
		'
	);
	insert into #tblResult
	 (select 
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
	end as "Document",
	a."DocEntry",a."DocNum",a."DocDate",a."DocTime", ifnull (a."CardCode",'''') as "CardCode"
	,b."CardName",
	 ifnull ( "DocTotal",0) as "DocTotal", ifnull ("Comments",'''') as "Comments" , ifnull (a."ObjType",'''') as "ObjType" , ifnull( "Requester",'''') as "Requester", ifnull("ReqName",'''') as "ReqName" , ifnull ("ReqType",null) as "ReqType"
	,CASE "ReqType"
		WHEN 12 then ''User'' 
		WHEN 171 then ''Employee''
	else ''''
	END as "ReqTypeName"
	 , CANCELED , a."UserSign" , "ReqDate", "ReqDate" as "RequriedDate", ifnull("DocStatus",''O'')as "DocStatus" , a."WddStatus", ifnull(a."GroupNum",-1) as "ListNum" , ifnull((select "ListName" from OPLN Where a."GroupNum" = OPLN."ListNum" ),'''') as "ListName" ';
	
	
	
		select (SELECT distinct string_agg(',a."' || COLUMN_NAME || '"') FROM  TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = N'ODRF' AND substring(column_name, 1, 2) = 'U_') into cols from DUMMY;
		if length(:cols) > 1
		then
			cols = substring(:cols, 1, length(:cols));
		end if;
	
		SQLValue := :SQLValue ||  ifnull(:cols,'')||' from ODRF a left join OCRD b on a."CardCode"=b."CardCode" WHERE 1=1 ';
	
	
	IF(ObjType<>'')
	then
		SQLValue:= :SQLValue || ' AND COMPARESPLIT(a."ObjType",'''||ifnull(:ObjType,'')||''', '','')=1 ';
	END if;
	IF(UserID <>'' AND UserID<>'0')
	then
		SQLValue:=:SQLValue || ' AND a."UserSign"='''||ifnull(:UserID,'')||'''';
	else
		sqlValue = sqlValue || ' AND (a."UserSign"=' || cast(:loginUserID as varchar) || ' or a."CardCode" is null or ifnull(b."OwnerCode",-1)=-1 or b."OwnerCode" in (select distinct "OwnerId" from  :OwnerView))';
	END if;
	IF(OpenOnly=1)
	then
		SQLValue:=:SQLValue || ' AND "DocStatus"=''O'' ';
	END if;
	--Date
	IF (:Date = 1) THEN IF (:TypeDate = 0) THEN DATE_TYPE := 'a."CreateDate"';
	ELSE IF (:TypeDate = 1) THEN DATE_TYPE := 'a."UpdateDate"';
	END IF;
	END IF;
	
	IF (:TypeDate = 2) THEN DATE_TYPE := 'a."DocDate"';
	END IF;
	IF (:TypeDate = 3) THEN DATE_TYPE := 'a."DocDueDate"';
	END IF;
	IF (:TypeDate = 4) THEN DATE_TYPE := 'a."TaxDate"';
	END IF;
	IF (:DATE_TYPE <> '') THEN IF (:FromValue <> '') THEN SQLValue := :SQLValue || ' AND CAST(' || :DATE_TYPE || ' as date)>=CAST(''' || ifnull(:FromValue,'') || ''' as date) ';
	END IF;
	IF (:ToValue <> '') THEN SQLValue := :SQLValue || ' AND CAST(' || :DATE_TYPE || ' as date)<=CAST(''' ||ifnull(:ToValue,'')  || ''' as date) ';
	END IF;
	END IF;
	END IF;
	SQLValue:=:SQLValue || ')';
	
	-- Inventory Posting Draft
	IF( COMPARESPLIT('10000071',:ObjType, ',')=1)
	then
		SQLValue:=:SQLValue||'UNION (select ''Inventory Posting'' as "Document","DocEntry","DocNum",null as "DocDate",null as "CardCode",null as "DocTotal",null as "Comments","ObjType" from OIPD Where 1=1 ';
	
	IF(UserID <>'' AND UserID<>'0')
	then
		SQLValue:=:SQLValue || ' AND "UserSign"='''||ifnull(:UserID,'')||'''';
	END if;
	IF(OpenOnly=1)
	then
		SQLValue:=:SQLValue||' AND Status=''O'' ';
	END if;
	
-- Date
	DATE_TYPE := '';
	IF (:Date = 1) THEN IF (:TypeDate = 0) THEN DATE_TYPE := 'CreateDate';
	ELSE IF (:TypeDate = 1) THEN DATE_TYPE := 'UpdateDate';
	END IF;
	END IF;
	IF (:TypeDate = 2) THEN DATE_TYPE := 'DocDate';
	END IF;
	IF (:DATE_TYPE <> '') THEN IF (:FromValue <> '') THEN SQLValue := :SQLValue || ' AND cast("' || :DATE_TYPE || '" as date)>=Cast(''' || ifnull(:FromValue,'') || ''' as date) ';
	END IF;
	IF (:ToValue <> '') THEN SQLValue := :SQLValue || ' AND cast("' || :DATE_TYPE || '" as date)<=Cast(''' ||ifnull(:ToValue,'')|| ''' as date) ';
	END IF;
	END IF;
	END IF;
	SQLValue := :SQLValue || ')';
	END IF;
-- Inventory opening balance Draft
	IF(COMPARESPLIT('310000001',:ObjType, ',')=1)
	then
		SQLValue:=:SQLValue || 'UNION (select ''Inventory Opening Balance'' as "Document","DocEntry","DocNum","DocDate",Null as "CardCode","DocTotal","Comments","ObjType" from OIOD Where 1=1 ';
	
	IF(UserID <>'' AND UserID<>'0')
	then
		SQLValue:=:SQLValue || ' AND "UserSign"='''||ifnull(:UserID,'')||'''';
	END if;
	IF(OpenOnly=1)
	then
		SQLValue:=:SQLValue|| ' AND "Status"=''O'' ';
	END if;
	
	-- Date
	DATE_TYPE := '';
	IF (:Date = 1) THEN IF (:TypeDate = 0) THEN DATE_TYPE := 'CreateDate';
	ELSE IF (:TypeDate = 1) THEN DATE_TYPE := 'UpdateDate';
	END IF;
	END IF;
	IF (:TypeDate = 2) THEN DATE_TYPE := 'DocDate';
	END IF;
	IF (:DATE_TYPE <> '') THEN IF (:FromValue <> '') THEN SQLValue := :SQLValue || ' AND cast("' || :DATE_TYPE || '" as date)>=Cast(''' ||ifnull(:FromValue,'') || ''' as date) ';
	END IF;
	IF (:ToValue <> '') THEN SQLValue := :SQLValue || ' AND cast("' || :DATE_TYPE || '" as date)<=Cast(''' || ifnull(:ToValue,'') || ''' as date) ';
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
	IF(Date=1)
	then
	
		IF(TypeDate=0)
		then
				DATE_TYPE:='CreateDate';
			
			ELSEIF(TypeDate=1)
			then
				DATE_TYPE:='UpdateDate';
		END if;
		IF(DATE_TYPE<>'')
		then
				IF(FromValue<>'')
				then
					SQLValue:=:SQLValue||' AND CAST("'||:DATE_TYPE||'" as date)>=CAST('''||ifnull(:FromValue,'')||''' as date) ';
				END if;
				IF(ToValue<>'')
				then
					SQLValue:=:SQLValue||' AND CAST("'||:DATE_TYPE||'" as date)<=CAST('''||ifnull(:ToValue,'')||''' as date) ';
				END if;
			END if;
	END if;
	
	
	SQLValue:=:SQLValue||')';

	END IF;
	sqlValue = 'do begin
	' || :sqlValue || ';
	end;';
	--select :SQLValue from dummy;
	--print @SQLValue
	exec :SQLValue;
	
	execute immediate 'select * from #tblResult order by "DocEntry" desc';
	exec 'drop table #tblResult';

END;

