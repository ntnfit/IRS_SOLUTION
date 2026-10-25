

CREATE Procedure SP_APZ_WorkingTable_GetTopDraft(
	UserCode nvarchar(50) default ''
)
as
BEGIN
	SELECT top 1 a."Subject", d.U_NAME AS "FromUser", c."ObjType"
	FROM OALR a join OAIB b on a."Code"=b."AlertCode"
	join ALR3 c on a."Code"=c."Code"
	join OUSR d on d.USERID=a."UserSign"
	join OUSR e on e.USERID=b."UserSign"
	where  a."Type"='M' AND b."WasRead"='N'
	and c."ObjType" IN ('122','112') and e.USER_CODE=:UserCode
	ORDER BY b."RecDate" DESC;
END






