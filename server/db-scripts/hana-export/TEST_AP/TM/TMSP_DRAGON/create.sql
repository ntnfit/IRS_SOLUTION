-- B1 DEPENDS: AFTER:PT:PROCESS_END

CREATE PROCEDURE TmSp_DragOn (
	IN onObj nvarchar(4),
 	IN fromObj nvarchar(4),
 	IN fldAias nvarchar(15),
 	IN srchVal nvarchar(255),
 	IN ColTypeI int
) LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
    errCode 	int;
	SQLstring  	nvarchar (4000);
	SQLstring1  nvarchar (4000);
	SQLString2  nvarchar (4000);
	SQLString3  nvarchar (4000);
	SQLString4  nvarchar (4000);
	JoinString  nvarchar (256);
	FilterString nvarchar (256);
	whereString	 nvarchar (512);
	groupString	 nvarchar (2048);
	fakeString  nvarchar (4000);
	tableStr	nvarchar (8);
	dbCol		nvarchar (128);
	prefix		nvarchar (32);
	isNull		nvarchar (16);
	tblName 	nvarchar (16);
	colType	    int;
	keyCount	int;
	fldCount	int;
	stringCount	int;
	objId 		int;
	colNameIdx 	int;

cursor dbColsList for
		select "COLUMN_NAME", "IS_NULLABLE", "DATA_TYPE_ID"
		from "PUBLIC"."TABLE_COLUMNS"
		where "TABLE_OID" = 
		         (select "TABLE_OID" from "PUBLIC"."TABLES" where "TABLE_NAME" = :onObj and SCHEMA_NAME = CURRENT_SCHEMA)
		order by "COLUMN_ID";

BEGIN
    select "JoinStr", "FilterStr" into JoinString, FilterString
	from SBOCOMMON.SDRQ
	where "ObjTo" = :onObj and "ObjFrom" = :fromObj and "ServerType" = 5;
	
	whereString := '';
		
	stringCount := 0;
	SQLString1 := '';
	SQLString2 := '';
	SQLString3 := '';
	SQLString4 := '';
	
	SQLString := 'Select ';
	groupString := ' Group By ';
	
	select "TABLE_OID" into objId from "PUBLIC"."TABLES" where "TABLE_NAME" = :onObj and SCHEMA_NAME = CURRENT_SCHEMA;
	
	select count("COLUMN_NAME") into keyCount from "PUBLIC"."TABLE_COLUMNS"
	        where "TABLE_OID" = :objId and "IS_NULLABLE" = 'FALSE';
	select count("COLUMN_NAME") into fldCount from "PUBLIC"."TABLE_COLUMNS"
			where "TABLE_OID" = :objId;		
	
	if :onObj = :fromObj or :JoinString = (:onObj || ' T0') then
		tableStr := 'T0.';
	else
		tableStr := 'T1.';
	end if;
	
	colNameIdx := 0;
	
	open dbColsList;
	fetch dbColsList into dbCol, isNull, colType;
	
	while not dbColsList::NOTFOUND do
		
		dbCol := ltrim(rtrim(dbCol));
	----------------------------------------------------------------
		--find who is the current string
		if length(:SQLString) > 3900 then
				if (:stringCount = 0) then SQLString1 := :SQLString;
			elseif (:stringCount = 1) then SQLString2 := :SQLString;
			elseif (:stringCount = 2) then SQLString3 := :SQLString;	
			elseif (:stringCount = 3) then SQLString4 := :SQLString;		
			end if;
			
			SQLString := '';
			stringCount := :stringCount + 1;		
		end if;
	------------------------------------------------------------------
		
		if(:isNull = 'TRUE') then
			fldCount := :fldCount - 1;
			
			if( :colType = -1 or :colType = -4 or :colType = -10) then
				SQLString := :SQLString || 'CAST(''FFFF'' as nchar(10)) as Col' || :colNameIdx || ' ';
				colNameIdx := colNameIdx + 1;
			else
				SQLString := :SQLString || 'MIN(T0."' || :dbCol || '") ';
			end if;
			
			if :fldCount > 0 then
				SQLString := :SQLString || ',';
			end if;
		else
			fldCount := :fldCount - 1;
			if :fldCount > 0 then
				SQLString := :SQLString || 'T0."' || :dbCol || '",';
			else
				SQLString := :SQLString || 'T0."' || :dbCol || '"';
			end if;
			
			keyCount := :keyCount - 1;
			
			if :keyCount > 0 then
				groupString := :groupString || 'T0."' || :dbCol || '",';
			else
				groupString := :groupString || 'T0."' || :dbCol|| '"';
			end if;
		end if;
		
		fetch dbColsList into dbCol, isNull, colType;
	end while;

	close dbColsList;	
	
	------------------------------------------------------------------ 
	--copy from sqlstring to the last string and rtrim
	stringCount := :stringCount + 1;

		if :stringCount = 1 then SQLString1 := :SQLString;
	elseif :stringCount = 2 then SQLString2 := :SQLString;
	elseif :stringCount = 3 then SQLString3 := :SQLString;		
	elseif :stringCount = 4 then SQLString4 := :SQLString;		
	end if;
	
	SQLString1 := rtrim(SQLString1);
	SQLString2 := rtrim(SQLString2);
	SQLString3 := rtrim(SQLString3);
	SQLString4 := rtrim(SQLString4);
	------------------------------------------------------------------
	-- Add filtering & conditions
	if LENGTH(:fldAias) > 0 then
		whereString := ' WHERE ' || :tableStr || '"'|| :fldAias || '"';
	end if;
	
	if (LENGTH(:srchVal) > 0) then
		if (substr(:srchVal,1,6) = 'TO_INT') then
			whereString := :whereString  || ' = ' ||  :srchVal;
		else 
			if  (substr(:srchVal,1,9) = 'TO_BIGINT') then
				whereString := :whereString  || ' = ' ||  :srchVal;
			else 
				if  (substr(:srchVal,1,7) = 'TO_DATE') then
					whereString := :whereString  || ' = ' ||  :srchVal;
				else 
					if  (substr(:srchVal,1,12) = 'TO_TIMESTAMP') then
						whereString := :whereString  || ' = ' ||  :srchVal;
					else
						if (substr(:srchVal,1,1) = '''') then
							whereString := :whereString  || ' = N' || :srchVal;
						else
							whereString := :whereString  || ' = N''' || :srchVal || '''';
						end if;
					end if;
				end if;
			end if;
		end if;
	end if;
	
	-- Obtain the result set (Using @SQLString1-4 bypass the max length of 4000 to an nvarchar)
	fakeString := ' FROM ' || :JoinString || :whereString || ' ' || :groupString;
	execute immediate (:SQLString1 || :SQLString2 || :SQLString3 || :SQLString4 || :fakeString);
END;


