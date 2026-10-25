-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure SBO_GetNextAutoKey (in objectID nvarchar(100), in advance nvarchar(1))
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as 
sqlStr nvarchar(4000);
begin
	--BEGIN TRAN
	if :advance = 'Y' then
		sqlStr := 'SELECT "AutoKey" FROM ONNM WHERE "ObjectCode" = ''' || :objectID || ''' FOR UPDATE' ;
	else
		sqlStr := 'SELECT "AutoKey" FROM ONNM WHERE "ObjectCode" = ''' || :objectID || '''';
	end if;

	exec(:sqlStr);

	if :advance = 'Y' then
		sqlStr := 'UPDATE ONNM SET "AutoKey" = "AutoKey" + 1 WHERE "ObjectCode" = ''' || :objectID || ''''; 
		--+ 'AND  DocSubType = '
		exec(:sqlStr);
	end if;
	--COMMIT
end;


