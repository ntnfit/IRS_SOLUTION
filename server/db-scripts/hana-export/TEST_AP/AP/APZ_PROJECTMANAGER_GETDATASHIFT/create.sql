
CREATE PROCEDURE APZ_ProjectManager_GetDataShift(in code nvarchar(50) default '')
AS
BEGIN
 select * from APZ_OSHP a inner join Split(:code,',') b on a."Code" = trim(b.Data);

END



