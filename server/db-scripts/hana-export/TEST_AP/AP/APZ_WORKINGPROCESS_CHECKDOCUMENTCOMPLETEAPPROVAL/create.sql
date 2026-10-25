

CREATE PROCEDURE APZ_WorkingProcess_CheckDocumentCompleteApproval(in wddCode int)
AS
BEGIN

	select case when "Status" = 'Y' then 'Approved' else 'Not Approval' end from OWDD where "WddCode" = :wddCode;

END




