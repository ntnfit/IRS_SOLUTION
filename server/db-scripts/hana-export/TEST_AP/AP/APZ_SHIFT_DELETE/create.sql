
CREATE PROCEDURE APZ_Shift_Delete(IN shiftCode int)

AS
BEGIN
	declare code nvarchar(50) = ifnull((select "Code" from APZ_OSHP where "SpID" = :shiftCode),'');
	if :code <> '' and ((select count(1) from apz_WSH1 where "Shift" = :code) > 0 or (select 1 from APZ_RWS1 where "Shift" = :code) > 0) then
		select 10001,'Shift "' || cast(:code as nvarchar) || '" already exists in work schedule or resource schedule' from dummy;

	END if;
	Delete from APZ_OSHP where "SpID" = :shiftCode;
	select 0, '' from dummy;
END





