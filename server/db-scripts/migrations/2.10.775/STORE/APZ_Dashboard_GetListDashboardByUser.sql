
CALL APZ_EXISTSPROC('APZ_Dashboard_GetListDashboardByUser');

GO

create procedure APZ_Dashboard_GetListDashboardByUser(in userName nvarchar(50))
as
begin
	select a."Id"
	from APZ_DAB1 a join OPRM b on 'barSub_Dashboard' || cast(a."Id" as nvarchar) = b."menucode" and b.USER_CODE=:userName
	where a."CatId" > 0;
end