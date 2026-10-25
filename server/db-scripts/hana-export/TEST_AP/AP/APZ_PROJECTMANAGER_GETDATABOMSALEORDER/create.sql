
create procedure APZ_ProjectManager_GetDataBOMSaleOrder
(
in code nvarchar(50)
)
as
begin
select a.*, b."ItemName" as "ItemName", b."InvntryUom",b."ItmsGrpCod"  from ITT1 a left join OITM b on a."Code"=b."ItemCode" where "Father"=:code;
end;




