 

CREATE PROCEDURE APZ_D_AlertTemplate_Remove(IN code varchar(50))
AS
BEGIN
	
	--Delete Item data by code
	delete from APZ_D_OALT where "Code" = :code;
	select '' FROM DUMMY;
	
END




