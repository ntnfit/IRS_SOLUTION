-- B1 DEPENDS: AFTER:PT:PROCESS_END AFTER:SP:_TmSp_ValidateSpParam

CREATE PROCEDURE TmSp_GetUsers( IN InputDbName nvarchar(128),
                                IN ProcID INT)
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
AS
myHost nvarchar(254);
iVer int;
insertsql nvarchar(2048);
begin

--Parameters validation for security issues
call _TmSp_ValidateSpParam(:InputDbName);

select count(*) into iVer from m_database 
where to_int (substr_before (version, '.')) > 1 
or (to_int (substr_before (version, '.')) = 1 
    and to_int (substr_before (substr_after (version, '.'), '.')) > 0)
or (to_int (substr_before (version, '.')) = 1 
    and to_int (substr_before (substr_after (version, '.'), '.')) = 0 
    and to_int (substr_before (substr_after (substr_after (version, '.'), '.') , '.')) >= 36);
	
if :iVer > 0 then
	create LOCAL TEMPORARY COLUMN table #dbActiveUsers (
		programname nvarchar(128),
		hostname nvarchar(128),
		clientpid int,
		spid int, status nvarchar(30),
		loginame nvarchar(128), 
		dbname nvarchar(128));

-- Get active host name
	select CLIENT_HOST into myHost from m_connections
		where connection_id = current_connection;

	insertsql := 'insert into #dbActiveUsers 
		select ''Process:''|| CLIENT_PID,
			CLIENT_HOST, 
			CLIENT_PID,
			connection_id,
			CONNECTION_STATUS, 
			USER_NAME,
	   		CURRENT_SCHEMA_NAME from  m_connections
	   			WHERE (CONNECTION_STATUS = ''RUNNING'' OR CONNECTION_STATUS = ''IDLE'')';
	exec(:insertsql);	
	if length(:InputDbName) > 0 THEN
	--select * from #dbActiveUsers where dbname = :InputDbName and hostname <> :myHost;
		select distinct programname,hostname,clientpid from #dbActiveUsers where dbname = :InputDbName and ( (hostname = :myHost and clientpid <> :ProcID) or hostname <> :myHost );
	else
		select distinct programname,hostname,clientpid from #dbActiveUsers where hostname <> :myHost;			
	end if;
	drop table #dbActiveUsers;
end if;
end;


