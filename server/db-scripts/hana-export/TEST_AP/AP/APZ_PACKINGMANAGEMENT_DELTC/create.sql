
CREATE PROCEDURE APZ_PackingManagement_DelTC(in TypeCode nvarchar(50))
as
begin
	If EXISTS (SELECT 1 FROM APZ_DMPO WHERE "TypeCode" = :TypeCode ) THEN
	BEGIN
			 select 'Cant Delete because This TypeCode is used' from dummy;
			 return;

	END;
	END IF;
	
	DELETE FROM APZ_DMTC WHERE "TypeCode" = :TypeCode;

	select '' FROM DUMMY;
end;




