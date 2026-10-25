
CREATE PROCEDURE APZ_BOM_GetDefaultWhs(in userName varchar(20), in branch int default -2)
AS
BEGIN
	declare whsDefault varchar(20) = '';
	select (select ifnull("Warehouse", '') from OUDG where "Code" = (select top 1 "DfltsGroup" from OUSR where USER_CODE = :userName)) into whsDefault from dummy;
	if (ifnull(:whsDefault, '')) = ''
    then
         select (select ifnull("DflWhs", '') from OBPL where "BPLId" = :branch) into whsDefault from dummy;
    end if;
    if (ifnull(:whsDefault, '')) = ''
    then
        select (select ifnull("DfltWhs", '') from OADM) into whsDefault from dummy;
    end if;
	select :whsDefault from dummy;
END;




