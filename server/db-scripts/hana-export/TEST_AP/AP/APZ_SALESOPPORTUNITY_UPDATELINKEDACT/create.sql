
CREATE PROCEDURE APZ_SalesOpportunity_UpdateLinkedAct(
	in ClgCode nvarchar(12) default '',
	in OprId nvarchar(12) default '',
	in OprLine nvarchar(12) default '')
AS
BEGIN
	UPDATE oclg SET "OprId" = :OprId, "OprLine" = :OprLine WHERE "ClgCode" = :ClgCode;
END;



