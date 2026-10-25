
CREATE PROCEDURE APZ_DeductionType_DeleteDocs(in ids nvarchar(5000))
AS 
BEGIN
delete from APZ_HDT1 where "Code"=:ids;
delete from APZ_OHDT where "Code" =:ids;
END




