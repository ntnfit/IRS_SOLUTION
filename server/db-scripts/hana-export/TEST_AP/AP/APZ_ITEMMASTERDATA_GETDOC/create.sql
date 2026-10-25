
CREATE PROCEDURE APZ_ItemMasterData_GetDoc
(
ItemCode nvarchar(50)  default  '',
UserCode nvarchar(30)  default  '',
BPLId int  default  0)
AS
BEGIN
SELECT  *
 FROM oitm WHERE ItemCode = :ItemCode;
END





