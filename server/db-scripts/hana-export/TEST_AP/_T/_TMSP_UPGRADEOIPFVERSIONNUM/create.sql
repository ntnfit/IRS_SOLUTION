-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE procedure _TmSp_UpgradeOIPFVersionNum
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS
 sqll varchar(5000);
BEGIN


sqll :='update OIPF set "VersionNum" = LEFT("VersionNum",1)||''.''||SUBSTRING ("VersionNum",2,2)||''.''||SUBSTRING ("VersionNum",4,3)||''.00'' where length("VersionNum") = 6';

exec (:sqll);

END;


