
create procedure APZ_Dashboard_GetListDashboard()
as
begin
	select a."Id", a."Name"
	from APZ_DAB1 a join APZ_ODAB b on a."CatId"=b."Id"
	where ifnull(a."MarkAsRemoved",'N')='N'
	order by a."CatId";
end

