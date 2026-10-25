-- B1 DEPENDS: BEFORE:PT:PROCESS_START AFTER:SP:_TmSp_ValidateSpParam
--
-- This is pre-upgrade proc, which must be executed before CRD object upgrade.
-- It fixes duplicate keys in ACRD table - because of new unique index (DocEntry, LogInstanc).

CREATE PROCEDURE _TmSp_MakeUnique_ACRD_DocEntryLogInstance(IN tableName nvarchar(100))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
  textSQL    nvarchar(2000);
  existsSeq  int;
BEGIN
--For HANA security issue call procedure "_TmSp_ValidateSpParam"
  call _TmSp_ValidateSpParam(:tableName);      
  
  SELECT Count(1) INTO existsSeq
  FROM "PUBLIC"."SEQUENCES"
  WHERE "SEQUENCE_NAME" = 'ACRD_MAXLOGINSTANCE'
    AND "SCHEMA_NAME" = CURRENT_SCHEMA;

  IF :existsSeq <> 0 THEN
    Exec('DROP SEQUENCE ACRD_MaxLogInstance');
  END IF;

  Exec('CREATE SEQUENCE ACRD_MaxLogInstance');

  textSQL := '';

  textSQL := :textSQL || 'UPDATE "%TABLE%" ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || 'SET "LogInstanc" = (SELECT MAX("LogInstanc") FROM "%TABLE%") + ACRD_MaxLogInstance.NextVal ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || 'WHERE ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '  ("CardCode", "LogInstanc") ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '  IN ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '  (SELECT tblMain2."CardCode", tblMain2."LogInstanc" ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '   FROM ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '     "%TABLE%" tblMain2 ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '     INNER JOIN (SELECT "DocEntry", "LogInstanc" AS "LogInstance" ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '                 FROM "%TABLE%" ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '                 GROUP BY "DocEntry", "LogInstanc" ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '                 HAVING COUNT(*) > 1 ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '                ) tblMulti ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '       ON tblMulti."DocEntry"    = tblMain2."DocEntry" AND ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '          tblMulti."LogInstance" = tblMain2."LogInstanc" ' || CHAR(13) || CHAR(10);
  textSQL := :textSQL || '  )';

  textSQL := REPLACE(:textSQL, '%TABLE%', :tableName);
  Exec(:textSQL);

  Exec('DROP SEQUENCE ACRD_MaxLogInstance');
END;


