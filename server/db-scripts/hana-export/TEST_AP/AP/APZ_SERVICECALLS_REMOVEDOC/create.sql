
CREATE PROCEDURE APZ_ServiceCalls_RemoveDoc(in CallId int)
AS
BEGIN
	
	DELETE FROM APZ_OSCL WHERE "CallID"=:CallId;
	DELETE FROM SCL1 WHERE "srvcCallID"=:CallId;
	DELETE FROM APZ_SCL3 WHERE "srvcCallID"=:CallId;
	
	UPDATE APZ_OCLG SET "DocNum"=NULL, "DocEntry"=NULL WHERE "DocNum"=:CallId AND "DocType"='191';
		
END



