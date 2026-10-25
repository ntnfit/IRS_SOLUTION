-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

------------------------------- TmSp_ArcSaveArchivedProductionRecords -------------------------------

-- This stored procedure copies records from OWOR and WOR1 to UWOR and UWOR1 OR
-- from OWKO and WKO1 to UWKO and UWKO1.
-- The records that are copied are such that are going to be archived, i.e. they exist in the temporary
-- table ##TDAR_<pid> (@in_TDAR_name) with CanArcObj = 'Y' and CanArcClus = 'Y'

--------------------------------------------------------------------------------------------------------------
CREATE PROCEDURE TmSp_ArcSaveArchivedProductionRecords
(
in in_TDAR_name varchar (5000),
in in_Object varchar (5000),
out ret int
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
	
	--*With Encryption

AS
TDAR_name varchar (5000);					
UTable varchar (5000);
OTable varchar (5000);
UTable1 varchar (5000);
OTable1 varchar (5000);
AbsField varchar (5000);
object varchar (5000);
s_sql varchar(5000);

BEGIN

--Parameters validation for security issues
call _TmSp_ValidateSpParam(:in_TDAR_name);
call _TmSp_ValidateSpParam(:in_Object);

ret :=0;
TDAR_name := :in_TDAR_name;		-- ##TDAR_<pid>

IF (:in_Object = 202) then--WOR
	object := '202';
	UTable := 'UWOR';
	OTable := 'OWOR';
	UTable1 := 'UWOR1';
	OTable1 := 'WOR1';
	AbsField := 'T0."DocEntry"';

ELSE 
	IF (:in_Object = 68) then--WKO
		object := '68';
		UTable := 'UWKO';
		OTable := 'OWKO';
		UTable1 := 'UWKO1';
		OTable1 := 'WKO1';
		AbsField := 'T0."OrderNum"';
	ELSE
		ret :=-1;
		return;
	END IF;
END IF;

s_sql :=	'-------------------'||
		'-- Header'||
		'-------------------'||
	'INSERT INTO ' || :UTable ||
	'SELECT T0.* '||--o "" hviezdicku??
	'FROM ' || :OTable || ' T0 '||
	'JOIN ' || :TDAR_name || ' T1 ON T1."DocType" = '|| :object ||'  AND T1."DocAbs" = ' || :AbsField ||
	' WHERE '||
	'T1."CanArcObj" = ''Y'' AND '||
	'T1."CanArcClus" = ''Y'' '||
	'------------------- '||
	'-- Array 1 '||
	'------------------- '||

	'INSERT INTO ' || :UTable1 ||
	'SELECT T0.* '||--o "" hviezdicku??
	'FROM ' || :OTable1 || ' T0 '||
	'JOIN ' || :TDAR_name || ' T1 ON T1."DocType" = '|| :object || '  AND T1."DocAbs" = ' || :AbsField ||
	' WHERE '||
	'T1."CanArcObj" = ''Y'' AND '||
	'T1."CanArcClus" = ''Y'' ';

exec (:s_sql);

ret :=0;

END;


