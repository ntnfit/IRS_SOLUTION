

CREATE  PROCEDURE APZ_SubActivities_LoadEventByOpportunity(in objType int, in oprId int, in bpCode varchar(50))
as
BEGIN

	if :objType <> 97
	then
		return;
	end if;
	
	select * from APZ_OCLG where "CardCode" = :bpCode 
	and "OprId" = :oprId;

END;



