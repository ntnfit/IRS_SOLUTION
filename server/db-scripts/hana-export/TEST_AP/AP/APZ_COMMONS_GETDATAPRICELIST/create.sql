
CREATE PROCEDURE APZ_Commons_GetDataPricelist

AS
BEGIN
	select NULL as "Code", NULL as "Name" from dummy
	union all
	select "ListNum" as "Code", "ListName" as "Name"  from OPLN
	Union all
	select -1 "Code", 'Last Purchase Price' "Name" from DUMMY
	union all 
	select -2 "Code", 'Last Evaluated Price' "Name" from DUMMY;
END;




