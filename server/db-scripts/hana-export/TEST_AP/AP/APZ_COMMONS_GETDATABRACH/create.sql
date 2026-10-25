

CREATE PROCEDURE APZ_Commons_GetDataBrach(in userCode nvarchar(25))
AS
BEGIN
	select a."BPLId" as "Value", b."BPLName" as "Name" from USR6 a inner join OBPL b on a."BPLId" = b."BPLId" where "UserCode" = :userCode;
END




