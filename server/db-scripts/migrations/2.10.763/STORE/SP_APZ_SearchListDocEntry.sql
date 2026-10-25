
CALL APZ_EXISTSPROC('SP_APZ_SearchListDocEntry');

GO

create PROCEDURE SP_APZ_SearchListDocEntry
(
in ObjType Nvarchar(254) default '0',
UserID nvarchar(30) default '',
OpenOnly tinyint default 0,
Date tinyint default 0,
TypeDate int default 0,
FromValue nvarchar(50) default '',
ToValue nvarchar(50)default ''
)
AS 
BEGIN
	DECLARE SQLValue NVARCHAR(5000) = ' select top 1 "DocEntry" from ODRF WHERE 1=1 ';
	DECLARE DATE_TYPE NVARCHAR(5000)='';
	IF (:ObjType <> '') THEN SQLValue := :SQLValue || ' AND  COMPARESPLIT("ObjType", '''||ifnull(:ObjType,'')||''', '','') = 1 ';
	End if;
	IF (:UserID <> '' AND :UserID <> '0') THEN SQLValue := :SQLValue || ' AND "UserSign"=''' ||ifnull(:UserID,'') || '''';
	END IF;
	IF (:OpenOnly = 1) THEN SQLValue := :SQLValue || ' AND "DocStatus"=''O'' ';
	END IF;
	
	IF (:Date = 1) THEN 
	IF (:TypeDate = 0) THEN DATE_TYPE := 'CreateDate'; 
	ELSEIF (:TypeDate = 1) THEN DATE_TYPE := 'UpdateDate';
	ELSEIF (:TypeDate = 2) THEN DATE_TYPE := 'DocDate';
	ELSEIF (:TypeDate = 3) THEN DATE_TYPE := 'DocDueDate';
	ELSEIF (:TypeDate = 4) THEN DATE_TYPE := 'TaxDate';
	ELSEIF (:DATE_TYPE <> '') THEN IF (:FromValue <> '') THEN SQLValue := :SQLValue || ' AND Cast("' || :DATE_TYPE || '" as date)>=cast(''' || ifnull(:FromValue,'') || ''' as Date) ';
	ELSEIF (:ToValue <> '') THEN SQLValue := :SQLValue || ' AND Cast("' || :DATE_TYPE || '"  as Date)<=Cast(''' || ifnull(:ToValue,'') || ''' as date) ';
	END IF;
	
	SQLValue:=:SQLValue;
	
-- Inventory Posting Draft
	IF( COMPARESPLIT('10000071',:ObjType, ',') =1)
	then
		SQLValue:=:SQLValue || 'UNION (select top 1 "DocEntry" from OIPD Where 1=1 ';
	
	IF(UserID <>'' AND UserID<>'0')
	then
		SQLValue:=:SQLValue+' AND "UserSign"='''||ifnull(:UserID,'')||'''';
	END if;
	IF(OpenOnly=1)
	then
		SQLValue:=:SQLValue || ' AND "Status"=''O'' ';
	END if;
	
-- Date
	DATE_TYPE := '';
	IF (:Date = 1) 
	THEN 
	IF (:TypeDate = 0) 
		THEN DATE_TYPE := 'CreateDate';
			ELSEIF (:TypeDate = 1) 
			THEN 
			DATE_TYPE := 'UpdateDate';
		END IF;
	END IF;
	IF (:TypeDate = 2) THEN DATE_TYPE := 'DocDate';
	END IF;
	IF (:DATE_TYPE <> '') THEN IF (:FromValue <> '') THEN SQLValue := :SQLValue || ' AND CAST( "' || :DATE_TYPE || '" as date)>=CAST(''' || ifnull(:FromValue,'') || ''' as date) ';
	END IF;
	IF (:ToValue <> '') THEN SQLValue := :SQLValue || ' AND CAST("' || :DATE_TYPE || '" as date)<=CAST(''' || ifnull(:ToValue,'') || ''' as date) ';
	END IF;
	END IF;
	END IF;
	SQLValue := :SQLValue ;
	End if;
-- Inventory Counting Draft
	IF( COMPARESPLIT('1470000065',:ObjType, ',') =1)
	then
		SQLValue :=:SQLValue || 'UNION (select top 1 "DocEntry" from OICD Where 1=1 ';
	
	IF(UserID <>'' AND UserID<>'0')
	then
		SQLValue:=:SQLValue || ' AND "UserSign"='''||ifnull(:UserID,'')||'''';
	END if;
	IF(OpenOnly=1)
	then
		SQLValue:=:SQLValue+' AND "Status"=''O'' ';
	END if;
	
-- Date
	DATE_TYPE:='';
	IF(Date=1)
	then
	
		IF(TypeDate=0)
		then
			DATE_TYPE :='CreateDate';
			end if;
		ELSEIF(TypeDate=1)
		then
			DATE_TYPE:='UpdateDate';
		END if;
		IF(DATE_TYPE<>'')
		then
			IF(FromValue<>'')
				then
					SQLValue:=:SQLValue || ' AND CAST("'||:DATE_TYPE||'" as date)>=CAST('''||ifnull(:FromValue,'')||'''  as date) ';
				END if;
				IF(ToValue<>'')
				then
					SQLValue:= :SQLValue || ' AND CAST("'||:DATE_TYPE||'"  as date )<=CAST('''||ifnull(:ToValue,'')||''' as date) ';
				END if;
		END if;
	END if;
	
	End if;

	execute immediate :SQLValue;

END;