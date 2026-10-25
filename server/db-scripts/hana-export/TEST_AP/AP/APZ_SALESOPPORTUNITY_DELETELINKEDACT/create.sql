
CREATE PROCEDURE APZ_SalesOpportunity_DeleteLinkedAct(in OprId nvarchar(12) default '')
AS
BEGIN
	UPDATE oclg SET "OprId" = NULL, "OprLine" = NULL WHERE "OprId" = :OprId;

END;



