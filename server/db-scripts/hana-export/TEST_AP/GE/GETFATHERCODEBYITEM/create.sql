

create function GetFatherCodeByItem(itemcode nvarchar(50))
returns table ("FGCode" nvarchar(50))
as
begin
	declare tbl table("Code" nvarchar(50));
	declare tblF table ("Code" nvarchar(50));
	declare tblS table ("Code" nvarchar(50));
	insert into :tblS("Code") values (:itemcode);
	insert into :tblF("Code") select a."Code" from APZ_OITT a, APZ_ITT1 b where a."BomVerID"=b."Father" and b."Code" in (select x."Code" from :tblS x) group by a."Code";
	insert into :tblF("Code") select a."Code" from OITT a, ITT1 b where a."Code"=b."Father" and b."Code" in (select x."Code" from :tblS x) group by a."Code";

	if (select count(1) from :tblF)>0 then

		insert into :tbl("Code") select "Code" from :tblF;
		delete from :tblS;
		insert into :tblS ("Code") select "Code" from :tblF;
		delete from :tblF;
		insert into :tblF("Code") select a."Code" from APZ_OITT a, APZ_ITT1 b where a."BomVerID"=b."Father" and b."Code" in (select x."Code" from :tblS x) group by a."Code";
		insert into :tblF("Code") select a."Code" from OITT a, ITT1 b where a."Code"=b."Father" and b."Code" in (select x."Code" from :tblS x) group by a."Code";
	end if;
	
	Return select "Code" as "FGCode" from :tbl group by "Code";
	
end;




