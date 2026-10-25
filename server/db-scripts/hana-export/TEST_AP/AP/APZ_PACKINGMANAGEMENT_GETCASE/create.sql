
create procedure APZ_PackingManagement_GetCase(in POCode int)
as
begin
	SELECT "CaseCode", "POCode", APZ_DMC.USERID, "PostingDate", APZ_DMC."CreateDate", APZ_DMC."TypeCode", APZ_DMTC."Amount", "LineNumber"
	From APZ_DMC
	INNER JOIN APZ_DMTC ON APZ_DMC."TypeCode" = APZ_DMTC."TypeCode"
	WHERE APZ_DMC."POCode" =:POCode
	ORDER BY "LineNumber" DESC;
end;




