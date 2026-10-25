
CREATE PROCEDURE APZ_Cycle_GetWareHouse()
AS 
BEGIN
select "WhsCode","WhsName" from OWHS where "Locked"='N' and "DropShip"='N';
END;




