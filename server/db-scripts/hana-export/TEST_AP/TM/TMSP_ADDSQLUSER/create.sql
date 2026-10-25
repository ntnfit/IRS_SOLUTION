-- B1 DEPENDS: AFTER:PT:PROCESS_END

create procedure TmSp_AddSqlUser( 
IN uName varchar (32),
IN uPwd  varchar (32)
)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
	cnt int;
begin

	select count(*) into cnt from "PUBLIC".users where user_name = :uName ;
	if :cnt = 0 
	then
		exec 'CREATE USER ' || :uName || ' PASSWORD ' || :uPwd;
		exec 'GRANT CREATE ANY,ALTER, DELETE,DROP,EXECUTE,INDEX,INSERT,SELECT,UPDATE ON SCHEMA ' || '"'|| CURRENT_SCHEMA || '"' || ' TO ' || :uName;
		exec 'alter user ' || :uName || ' DISABLE PASSWORD LIFETIME';
	end if;

end;


