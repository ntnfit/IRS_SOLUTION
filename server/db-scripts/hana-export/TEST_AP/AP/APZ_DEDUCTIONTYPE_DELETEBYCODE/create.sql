
CREATE PROCEDURE APZ_DeductionType_DeleteByCode(in ids nvarchar(5000))
AS 
BEGIN
delete from APZ_HDT1 where "Code" in (select Data from split(:ids,','));
	
END




