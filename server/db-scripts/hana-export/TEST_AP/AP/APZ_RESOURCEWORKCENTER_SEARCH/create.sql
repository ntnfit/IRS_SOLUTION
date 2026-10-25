
CREATE PROCEDURE APZ_ResourceWorkCenter_Search(
	in resCode nvarchar(50) default '',
	in resGroup nvarchar(50) default '',
	in deptCode nvarchar(50) default '',
	in valid nvarchar(1) default '',
	in cardCode nvarchar(50) default '',
	in calCode nvarchar(50) default '')
AS
BEGIN
	select * from ORSC a 
		where ifnull(a."ResCode",'') like '%'|| :resCode || '%' and ifnull(a."ResGrpCod",0) like '%' || :resGroup || '%' and ifnull(a."CardCode",'') like '%' || :cardCode || '%';
	
END



