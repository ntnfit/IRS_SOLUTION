
create procedure APZ_ProjectManager_Remove(in Id int)
as
begin
	declare isexist int;
	declare isClg int;
	
	select count(*) into isexist  from APZ_TGIE where "ProjectCode"=:Id;
	select count(*) into isClg  from APZ_OCLG where "ProjectEntry"=:Id;
	
	if(ifnull(isexist,0)>=1 or ifnull(isClg,0)>=1 
		 or exists((select 1 from PRQ1 where "U_OriginType" = '1470000132' and "U_OriginCode" =:Id))
		 or exists(select 1 from PDN1 where "U_OriginType" = '1470000132'  and "U_OriginCode" =:Id)
		 or exists(select 1 from POR1 where "U_OriginType" = '1470000132'  and "U_OriginCode" =:Id)
		 or exists(select 1 from DLN1 where "U_OriginType" = '1470000132'  and "U_OriginCode" =:Id)
		 or exists(select 1 from PCH1 where "U_OriginType" = '1470000132'  and "U_OriginCode" =:Id)
		 or exists(select 1 from INV1 where "U_OriginType" = '1470000132'  and "U_OriginCode" =:Id)
		 or exists(select 1 from PQT1 where "U_OriginType" = '1470000132'  and "U_OriginCode" =:Id)
		 or exists(select 1 from DPI1 where "U_OriginType" = '1470000132'  and "U_OriginCode" =:Id)
		 or exists(select 1 from DPO1 where "U_OriginType" = '1470000132'  and "U_OriginCode" =:Id)
		 or exists(select 1 from APZ_OPRG where "MasterEntry"=:Id))
	then
	
	 	select 1, 'Cannot remove name because it has been used' from dummy;
	 	return;
	
	end if;
	 
	Delete from APZ_OPRG where "AbsEntry"=:Id ;
	Delete from APZ_PRG1 where "AbsEntry"=:Id;
	Delete from APZ_PRG2 where "AbsEntry"=:Id;
	Delete from APZ_PRG3 where "AbsEntry"=:Id;
	Delete from APZ_PRG4 where "AbsEntry"=:Id;
	Delete from APZ_PRG5 where "AbsEntry"=:Id;
	Delete from APZ_PRG6 where "AbsEntry"=:Id;
	
	select 0, '' from dummy;
end;




