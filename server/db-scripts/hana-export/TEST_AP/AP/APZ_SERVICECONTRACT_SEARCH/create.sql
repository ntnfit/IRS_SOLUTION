
CREATE  procedure APZ_ServiceContract_Search(in StartDate timestamp default null, in EndDate timestamp default null)
as
begin
	DECLARE UDFString NVARCHAR(40000);
	DECLARE sqlString NVARCHAR(40000);
	
	select ( SELECT string_agg('"' || COLUMN_NAME || '"', ',') FROM TABLE_COLUMNS WHERE SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'OCTR' AND LEFT(COLUMN_NAME, 2) = 'U_') into udfString from DUMMY;
	sqlString = 'select c."ContractID",c."CntrcType",c."CntrcTmplt",c."ResponseV",c."ResponseU",c."ResponsVal",c."ResponsUnt",c."Status",c."Owner",c."Renewal",c."RemindVal",c."RemindUnit",c."Remarks1",c."Remarks2"
				,c."SrvcType",c."CstmrCode",c."CstmrName",c."CntctCode",c."StartDate",c."EndDate",c."Descriptio",c."TermDate",c."AtcEntry",c."InclParts",c."InclWork","InclTravel","InclHldays",(select Count(1) from CTR1 b Join OCTR a on b."ContractID"= a."ContractID" where b."ContractID" = c."ContractID") as "CTR1Count" ';
	IF IFNULL(:udfString, '') <> ''
	THEN 
			sqlString = :sqlString || ',' || :udfString || ' From OCTR c WHERE 1= 1 ';
	ELSE 
			sqlString = :sqlString || ' From OCTR c WHERE 1= 1 ';
	END	IF;
	IF :StartDate IS NOT NULL  and :EndDate IS NOT NULL 
    THEN 
         sqlString = :sqlString || ' AND c."StartDate" >= ''' || TO_VARCHAR(:StartDate, 'yyyy-MM-dd') || ''' and c."StartDate" <=''' || TO_VARCHAR(:EndDate, 'yyyy-MM-dd') || '''';
    END	IF;
	sqlString = :sqlString || ' Order by c."ContractID" DESC;';
	--PRINT (@SQL)
	EXECUTE immediate :sqlString;
end



