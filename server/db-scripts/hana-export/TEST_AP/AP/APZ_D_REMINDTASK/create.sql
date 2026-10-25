

CREATE PROCEDURE APZ_D_RemindTask
AS
BEGIN
	
	--Lấy ra danh sách các task trong ngày
	create local temporary table #tblTask as (select a."UserSign",b."U_NAME" as "Creator","ClgCode", "Details", "GroupUser", (select string_agg(USER_CODE, ',') from OUSR where CompareSplit(USERID, a."GroupUser", ';') = 1) USER_CODE
	, ifnull("AllUserInvite",'N') as "AllUserInvite" from APZ_OCLG a inner join OUSR b on a."UserSign" = b."USERID" where current_date between a."Recontact" and ifnull(a."endDate", a."Recontact"));
	
	INSERT INTO APZ_D_OELN ("EventCode", "Data", "SendUser", "SendAll", "DataExtra", "Message") 
	SELECT 'E006', "ClgCode", LEFT(USER_CODE, LENGTH(USER_CODE) - 1), "AllUserInvite", 
	'{"objectType": "1470000128","docEntry": ' || CAST("ClgCode" AS nvarchar) || '}', "Details" || ' (Người tạo ' || "Creator" || ')' FROM #tblTask WHERE IFNULL("GroupUser", '') <> '';
	
	select * from #tblTask;

END




