-- B1 DEPENDS: AFTER:PT:PROCESS_END

 create procedure TmSp_ValidateReadOnlyUser
	(IN LoginName nvarchar(128), 
		IN DbName nvarchar(128),
		IN DbName2 nvarchar(128),
		OUT ret int
	) 
LANGUAGE SQLSCRIPT 
SQL SECURITY INVOKER
as cnt int;
begin 
	ret := 0;
	
	if  :ret = 0
	then
		select count(*) into cnt from "PUBLIC".GRANTED_PRIVILEGES 
		where GRANTEE = :LoginName 
		and schema_name = :DbName 
		and privilege = 'SELECT'
		and object_name is null 
		and is_valid = 'TRUE';
		IF :cnt = 0 
		then 
			ret :=-1; --User does not have read permissions
		end if;
	end if;
	
	if :DbName2<>'' and :ret = 0
	then
	 	select count(*) into cnt from "PUBLIC".GRANTED_PRIVILEGES 
		where GRANTEE = :LoginName 
		and schema_name = :DbName2 
		and privilege = 'SELECT' 
		and object_name is null
		and is_valid = 'TRUE';
		IF :cnt = 0 
		then 
			ret :=-2; --User does not have read permissions on second db
		end	if;
	end if;
	
	if :ret = 0 
	then
		select count(*)	into cnt from "PUBLIC".GRANTED_PRIVILEGES 
		where GRANTEE = :LoginName 
		and schema_name = :DbName 
		and privilege IN ('DELETE','INSERT','UPDATE') 
		and object_name is null
		and is_valid = 'TRUE';
		IF :cnt > 0 
		then 
			ret :=-3; --User may have write permissions
		end if;
	end if;
	
	if :DbName2<>'' and :ret = 0
	then
		select count(*) into cnt from "PUBLIC".GRANTED_PRIVILEGES 
		where GRANTEE = :LoginName 
		and schema_name = :DbName2 
		and privilege IN ('DELETE','INSERT','UPDATE')
		and object_name is null 
		and is_valid = 'TRUE';	 
		IF :cnt > 0 
		then
			ret :=-4; --User may have write permissions on second db
		end	if;
	end if;
	
	if :ret = 0
	then
		select count(*) into cnt from "PUBLIC".GRANTED_PRIVILEGES 
		where GRANTEE = :LoginName 
		and schema_name NOT IN (:DbName,:DbName2) 
		and privilege IN ('DELETE','INSERT','UPDATE')
		and object_name is null 
		and is_valid = 'TRUE';
		IF :cnt > 0 
		then 
			ret :=-5; --User has access to other databases without db_denydatawriter restriction
		end if;
	end if;
	
	if :ret = 0
	then 
		select count(*) into cnt from "PUBLIC".GRANTED_ROLES 
		where GRANTEE = :LoginName 
		and role_name IN ('CONTENT_ADMIN','MODELING');
		IF :cnt > 0 
		then 
			ret :=-6; --User is a member of server role
			
		end if;
	end if;
	
	if :ret = 0
	then
		select count(*) into cnt from "PUBLIC".GRANTED_PRIVILEGES 
		where GRANTEE = :LoginName 
		and schema_name = :DbName 
		and privilege NOT IN ('DELETE','INSERT','UPDATE','SELECT')
		and object_name is null 
		and is_valid = 'TRUE';
		IF :cnt > 0 
		then 
			ret :=-7; --User member in db roles other than  read\deny write permissions
		end if;
	end if;
	
	if :DbName2<>'' and :ret = 0
	then
		select	count(*) into cnt from "PUBLIC".GRANTED_PRIVILEGES 
		where GRANTEE = :LoginName 
		and schema_name = :DbName2 
		and privilege NOT IN ('DELETE','INSERT','UPDATE','SELECT') 
		and is_valid = 'TRUE';
		IF :cnt > 0 
		then 
			ret :=-8; --User member in db roles other than  read\deny write permissions on second db
		end if; 
	end if;
	
	if :ret = 0
	then 
		select count(*) into cnt from "PUBLIC".GRANTED_PRIVILEGES 
		where GRANTEE = :LoginName 
		and privilege NOT IN ('SELECT') 
		and object_name is not null
		and is_valid = 'TRUE';
		if :cnt > 0 
		then 
			ret :=-9; --User have permissions on objects other than select\connect db
		end if;
	end if;
	
	if :ret = 0
	then 
		select count(*) into cnt from "PUBLIC".GRANTED_PRIVILEGES 
		where GRANTEE = :LoginName 
		and object_type = 'SYSTEMPRIVILEGE'
		and is_valid = 'TRUE';
		if :cnt > 0 
		then 
			ret :=-9; --SYSTEM PRIV. reuse -9 
		end if;
	end if;
	
end;


