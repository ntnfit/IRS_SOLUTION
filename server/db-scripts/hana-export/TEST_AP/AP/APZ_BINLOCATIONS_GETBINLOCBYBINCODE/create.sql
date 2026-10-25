
Create procedure APZ_BinLocations_GetBinLocByBinCode
(in binCode nvarchar(50) default '')
as
begin
	declare query nvarchar(500) = 'select "AbsEntry", "WhsCode", "SL1Code", "SL2Code", "SL3Code", "SL4Code", "BinCode" from OBIN';
	
	if ifnull(:binCode,'') <> '' then
		query = :query || ' where "BinCode" = ''' || :binCode || '''';
	end if;
	
	execute immediate (:query); 
end




