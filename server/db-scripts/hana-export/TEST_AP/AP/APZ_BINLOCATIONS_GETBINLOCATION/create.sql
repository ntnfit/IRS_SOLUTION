
CREATE PROCEDURE APZ_BinLocations_GetBinLocation(IN absEntry int default 0)
AS
BEGIN

	declare query nvarchar(200) = 'SELECT "AbsEntry", "WhsCode", "SL1Code", "SL2Code", "SL3Code", "SL4Code", "BinCode" from OBIN';
	if :absEntry > 0 then
		query = :query || ' where "AbsEntry" = ' || TO_NVARCHAR(:absEntry);
	end if;
	execute immediate (:query);
	
END;




