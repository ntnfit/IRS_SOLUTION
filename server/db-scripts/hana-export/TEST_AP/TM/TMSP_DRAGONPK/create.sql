-- B1 DEPENDS: AFTER:PT:PROCESS_END

Create procedure TmSp_DragOnPk(
	IN onObj nvarchar (4),
	IN fromObj nvarchar (4),
	IN fldAias nvarchar (15),
	IN srchVal nvarchar (255)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
-- NewDB do not support parameter of the input is NULL,
-- so here, '' is equivalent to NULL for the parameters 'fldAias' and 'srchVal' 
AS

SQLString	nvarchar (4000);
JoinString	nvarchar (256);
FilterString	nvarchar (256);
whereString	nvarchar (512);
groupString	nvarchar (512);
tableStr	nvarchar (8);
dbCol		nvarchar (128);
table_id	int;

cursor colsTmpCursor for
	select  c."COLUMN_NAME"
	from	"PUBLIC"."INDEXES" i, "PUBLIC"."TABLE_COLUMNS" c, "PUBLIC"."OBJECTS" o
	where	
		o.object_oid = (select "TABLE_OID" from "PUBLIC"."TABLES" where "TABLE_NAME" = :onObj and SCHEMA_NAME = CURRENT_SCHEMA)
		and o."OBJECT_OID" = c."TABLE_OID"
		and o."OBJECT_OID" = i."TABLE_OID"
		and i."CONSTRAINT" = 'PRIMARY KEY'
		and (c."COLUMN_NAME" = (select "COLUMN_NAME" from "PUBLIC"."INDEX_COLUMNS" 
							  where "TABLE_NAME" = :onObj 
								AND "INDEX_OID" = i."INDEX_OID" 
								AND "POSITION" = 1) or
			 c."COLUMN_NAME" = (select "COLUMN_NAME" from "PUBLIC"."INDEX_COLUMNS" 
							  where "TABLE_NAME" = :onObj 
								AND "INDEX_OID" = i."INDEX_OID" 
								AND "POSITION" = 2) or
			 c."COLUMN_NAME" = (select "COLUMN_NAME" from "PUBLIC"."INDEX_COLUMNS" 
							  where "TABLE_NAME" = :onObj 
								AND "INDEX_OID" = i."INDEX_OID"
								AND "POSITION" = 3) or
			 c."COLUMN_NAME" = (select "COLUMN_NAME" from "PUBLIC"."INDEX_COLUMNS" 
							  where "TABLE_NAME" = :onObj 
								AND "INDEX_OID" = i."INDEX_OID"
								AND "POSITION" = 4) or
			 c."COLUMN_NAME" = (select "COLUMN_NAME" from "PUBLIC"."INDEX_COLUMNS" 
							  where "TABLE_NAME" = :onObj 
								AND "INDEX_OID" = i."INDEX_OID"
								AND "POSITION" = 5) or
			 c."COLUMN_NAME" = (select "COLUMN_NAME" from "PUBLIC"."INDEX_COLUMNS" 
							  where "TABLE_NAME" = :onObj 
								AND "INDEX_OID" = i."INDEX_OID"
								AND "POSITION" = 6) or
			 c."COLUMN_NAME" = (select "COLUMN_NAME" from "PUBLIC"."INDEX_COLUMNS" 
							  where "TABLE_NAME" = :onObj 
								AND "INDEX_OID" = i."INDEX_OID"
								AND "POSITION" = 7) or
			 c."COLUMN_NAME" = (select "COLUMN_NAME" from "PUBLIC"."INDEX_COLUMNS" 
							  where "TABLE_NAME" = :onObj 
								AND "INDEX_OID" = i."INDEX_OID"
								AND "POSITION" = 8)
		    )
	order by c."COLUMN_ID";

begin
	select TOP 1 "JoinStr", "FilterStr" into JoinString, FilterString
	from "SBOCOMMON"."SDRQ"
	where "ObjTo" = :onObj and "ObjFrom" = :fromObj and "ServerType" = 5;

	SQLString := 'select ';

	groupString := ' ';

	open colsTmpCursor;
	fetch colsTmpCursor into dbCol;
	while not colsTmpCursor::NOTFOUND do
		groupString := :groupString || 'T0."' || :dbCol || '",';
		fetch colsTmpCursor into dbCol;
	end while;
	close colsTmpCursor ;

	groupString := LEFT( :groupString, LENGTH( :groupString) - 1 );
	SQLString := :SQLString || :groupString;
	
	if (:onObj = :fromObj) then
		tableStr := 'T0.';
	else
		tableStr := 'T1.';
	end if;
	 
	SQLString := :SQLString || ' FROM ' || :JoinString;
	
	-- Add filtering & conditions
	if (:FilterString IS NOT NULL) then
		whereString := ' WHERE (' || :FilterString;
		if (LENGTH(:fldAias) > 0) then
			whereString := :whereString || 'AND ' || :tableStr || '"'|| :fldAias || '"';
			if (substr(:srchVal,1,1) = '''') then
				whereString := :whereString  || ' = N' || :srchVal;
			else
				whereString := :whereString  || ' = N''' || :srchVal || '''';
			end if;			
		end if;
		whereString := :whereString || ')';
	else
		if (LENGTH(:fldAias) > 0) then
			whereString := ' WHERE ' || :tableStr || '"'|| :fldAias || '"';
			if (substr(:srchVal,1,1) = '''') then
				whereString := :whereString  || ' = N' || :srchVal;
			else
				whereString := :whereString  || ' = N''' || :srchVal || '''';
			end if;			
		end if;	
	end if;

	if (LENGTH(:whereString) > 0) then
		SQLString := :SQLString || :whereString;
	end if;
	 
	SQLString := :SQLString || ' Group By ' || :groupString;
	SQLString := :SQLString || ' Order By ' || :groupString;

	-- Obtain the result set
	execute immediate (:SQLString);
end;


