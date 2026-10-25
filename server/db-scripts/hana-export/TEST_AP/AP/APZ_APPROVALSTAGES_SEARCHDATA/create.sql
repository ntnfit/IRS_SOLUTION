

CREATE PROCEDURE APZ_ApprovalStages_SearchData(in stageCode varchar(50), stageName nvarchar(254))
AS
BEGIN

	if IFNULL(:stageCode, '') = '' and IFNULL(:stageName, '') = ''
	then
		select * from OWST;
		return;
	end if;
	
	select * from OWST where (TO_VARCHAR("WstCode") = NULL or TO_VARCHAR("WstCode") like N'%' || :stageCode || '%') 
	and ("Name" = null or "Name" like N'%'|| :stageName || '%');

END



