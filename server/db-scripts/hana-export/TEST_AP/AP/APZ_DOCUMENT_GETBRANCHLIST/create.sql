CREATE  PROCEDURE APZ_Document_GetBranchList(
in UserCode nvarchar(30) default ''
)
as
BEGIN
		select IFnull(a."BPLId",0) as "BPLId" ,"BPLName",IFnull("VATRegNum",'') as "VATRegNum", IFnull("DflWhs",'') as "DflWhs",ifnull("County",'') as "County"
		,ifnull("AliasName",'') as "AliasName", "State", "Country", ifnull(a."BPLId",0) as "Value" ,"BPLName" "Name"
			from obpl a inner join usr6 b on a."BPLId"=b."BPLId"
				where "Disabled"='N' and b."UserCode"=:UserCode
			order by "BPLId";
END



