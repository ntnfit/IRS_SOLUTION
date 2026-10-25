
CREATE PROCEDURE apz_approval_emailcontent(in WddCode INT, in WtmCode INT, in DraftKey INT, in ObjType NVARCHAR(20), in Remarks NVARCHAR(500) default '', in ReqId INT default 0, in ReceiptId INT default 0)
AS
BEGIN
	SELECT :WddCode AS "WddCode", :WtmCode AS "WtmCode", :DraftKey AS "DraftKey", :ObjType AS "ObjType", :Remarks AS "Remarks", T0.U_NAME AS "FromName", T0."E_Mail" AS "MailFrom", T1."E_Mail" AS "MailTo"
		, n'Request for Document Approval' AS "Subject", CURRENT_SCHEMA AS "DbName"
		, n'Va8+cyxRFpxDnqmlgVrIPbK3T3gC3G0qQpwCShcecx3PvwTo9+gJhQ6C7ay9XVdmyVeRyOhESx1hCHgU28wsdD38e6U+TbOe7ttNQXio2pX8ozqREaQJ4edv7hlhsX6oLc5MzvU3DRihrvPONsHeBkYLjv7oiDardHsbJJb6VJ4BNfMR1KiGbbFTTsNpbi2LjxIJS4SjmSKZqFPQM6Bs1PFP2s9vOb3qS7bvUSIRvsgCnS/PL77ebpIl90lUr8gOQn5o/IOJ30i+dgTZRMODFxWhM60TKMNe4ueHiK8BczFgFomREARWeectpe4Acn3grlTiok3mluhuYbgrVMh80A==' AS "Pwd"
		, 'http://localhost:17505' AS "ApiAddress" FROM OUSR T0, OUSR T1, "IRS-COMMON".APZ_OUCF T2 WHERE T0.USERID = :ReqId AND T1.USERID = :ReceiptId AND T2."Status" = 'N';

END




