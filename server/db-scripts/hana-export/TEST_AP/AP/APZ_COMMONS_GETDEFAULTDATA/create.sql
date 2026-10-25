
--	APZ_Commons_GetDefaultData 'manager',-1
CREATE PROCEDURE APZ_Commons_GetDefaultData(in userName nvarchar(50) default '', in branch int default -1)
AS
BEGIN
		DECLARE WhsDefault VARCHAR(8);
		select IFNULL((SELECT "Warehouse" FROM OUDG WHERE "Code"=(SELECT TOP 1 "DfltsGroup" FROM OUSR WHERE USER_CODE=userName)),'') into WhsDefault from DUMMY;
		
		IF :WhsDefault=''
		then
			 select ifNULL((SELECT "DflWhs" FROM OBPL WHERE "BPLId"=branch),'') into WhsDefault from DUMMY;
        END	if;
        
		IF :WhsDefault=''
		then
			 select IFNULL((SELECT "DfltWhs" FROM OADM),'') into WhsDefault from DUMMY;
        END	if;
	Select "MltpBrnchs" "MltpBrnchs", :WhsDefault "DftWhs" from OADM;
END;




