
CREATE PROCEDURE APZ_SubActivities_LoadComment(in ID int, in ObjType nvarchar(40000), in UserSign int)
AS
BEGIN

	SELECT a.*, ifnull(b."firstName", '') || ' ' ||  ifnull(b."middleName", '') || ' ' || ifnull(b."lastName", '') as "EmployeeName"  FROM APZ_CLG1 a inner join OHEM b on a."UserSign" = b."userId" Where "ID"=:ID and "ObjType"=:ObjType;
	
	SELECT * FROM APZ_CLG3 WHERE "CmtCode" IN ( SELECT "CmtCode" FROM APZ_CLG1 WHERE "ID"=:ID and "ObjType"=:ObjType) AND "UserSign"=:UserSign AND "Unread"='Y';
END



