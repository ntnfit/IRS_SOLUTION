CREATE procedure ILD_LayFlocCumChucNang
(
	in TaiSanCode nvarchar(5000)
)
as
begin
 select distinct "U_Floc" 
 from "@BOMAINTENANCE_L" a
 inner join  "@BOMAINTENANCE_H" b on a."DocEntry"  = b."DocEntry"
 where "U_Floc" is not null 
 		and 
 		(
 			ifnull(:TaiSanCode, '') = '' 
					or cast (b."U_ItemCodeF" as nvarchar) in (select "DATA" from split(:TaiSanCode,','))
		);

end;

