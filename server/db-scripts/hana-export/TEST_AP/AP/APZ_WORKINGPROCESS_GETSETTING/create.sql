

CREATE PROCEDURE APZ_WorkingProcess_GetSetting(in userId int)
AS
BEGIN
		
	--Get process and document menu
	select * from APZ_WPS1 where "UserId" = :userId and "Visible" = 'Y';
	
	--Get tooltips data
	select * from APZ_WPS2 where "UserId" = :userId 
	and "ColId" not in('VirStatus', 'IsCreated', 'Checked', 'RequestType')
	order by "VisOrder";
	
END




