CREATE procedure APZ_ProjectManager_UndoApproval(in abs_entry int,user_code nvarchar(50))
as 
begin
	Update apz_oprg set "WddStatus"='N',"Approver"=:user_code,"NextApprover"='' where "AbsEntry"=:abs_entry;
	
	insert into APZ_LOG_APPROVAL
	select '1470000132',cast(:abs_entry as nvarchar(50)),:user_code,'UNDO',now() from dummy;
end



