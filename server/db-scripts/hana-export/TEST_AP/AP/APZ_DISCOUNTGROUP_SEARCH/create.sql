
Create procedure APZ_DiscountGroup_Search(in Type varchar(1) default '',in ObjCode nvarchar(100))
As
Begin
declare sqlQuery nvarchar(5000)='select 
		*,
		case ifnull("Type",'''')
		when ''C'' then (select "GroupCode" from ocrg T0 where T0."GroupCode"=oedg."ObjCode")
		when ''V'' then (select "GroupCode" from ocrg T0 where T0."GroupCode"=oedg."ObjCode")
		end "GroupCode",
		case ifnull("Type",'''')
		when ''S'' then (select "CardCode" from OCRD T1 where T1."CardCode"=oedg."ObjCode")
		end "CardCode",
		case ifnull("Type",'''')
		when ''S'' then (select "CardName" from OCRD T1 where T1."CardCode"=oedg."ObjCode")
		end "CardName"
		from oedg where 1=1 ';
if Type<>''
then
	sqlQuery:=:sqlQuery || 'and "Type"=''' || ifnull(:Type,'') || '''';
end if;
if ObjCode<>''
then
	sqlQuery:=:sqlQuery || 'and "ObjCode"=''' || ifnull(:ObjCode,'') || '''';
end if;
execute immediate :sqlQuery;
End;




