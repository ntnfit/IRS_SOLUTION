
create procedure APZ_ProjectReportCompletion_ItemInBom
(
in code nvarchar(50)
)
as
begin
SELECT a.*, b."ItemName" as "ItemName", b."InvntryUom" as "UoM", b."ItmsGrpCod"
	 FROM OITT a LEFT JOIN OITM b ON   b."ItemCode"=a."Code" where a."Code"=:code;
end;




