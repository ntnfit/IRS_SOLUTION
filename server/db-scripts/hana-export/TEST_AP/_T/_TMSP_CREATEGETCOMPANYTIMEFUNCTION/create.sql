-- B1 DEPENDS: BEFORE:PT:PROCESS_START

CREATE PROCEDURE _TmSp_CreateGetCompanyTimeFunction
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER

AS 
sqlStr nvarchar (2000);
exists1 int;

BEGIN 
	select count(*) into exists1 from "PUBLIC".procedures 
	where procedure_name = 'GETCOMPANYTIME' and schema_name = current_schema;

	IF exists1 <> 0 then
		exec ('DROP PROCEDURE GetCompanyTime CASCADE ');
	end if;


--	exec ('create PROCEDURE GetCompanyTime(out outDate timestamp) '
--			|| 'language sqlscript AS '
--			|| 'offset1 DECIMAL(9,4); '
--			|| 'dst varchar(1); '
--			|| 'colNum int;'
--			|| 'BEGIN '
--			|| 'select count(*) into colNum from OTIZ; '
--			|| 'if :colNum <> 0 then '
--			|| 'select top 1 "offset", ifnull("ActiveDst",0) into offset1, dst '
--			|| 'from OTIZ '
--			|| 'order by "Id" desc; '
--			|| 'end if; '
--			|| 'if :offset1 is NULL then '
--			|| '	outDate := CURRENT_TIMESTAMP; '
--			|| 'else '
--			|| 'if (:dst = ''Y'') then '
--			|| '	offset1 := :offset1 + 60; '
--			|| 'end if; '
--			|| 'offset1 := :offset1 / 1440; '
--			|| 'outDate := add_seconds(CURRENT_UTCTIMESTAMP, offset1*24*60*60); '
--			|| 'end if; '
--			|| 'END'); 

End;


