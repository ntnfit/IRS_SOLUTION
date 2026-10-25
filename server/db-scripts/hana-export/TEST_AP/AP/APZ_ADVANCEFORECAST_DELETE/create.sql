
CREATE PROCEDURE APZ_AdvanceForecast_Delete(in absId int)
AS
BEGIN
	declare checkCount int = 0;
	select ifnull((select top 1 1 from apz_OMRP  where comparesplit(:absId, "FCCode", ',') = 1), 0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN

		select 80000051, 'Could not delete forecase. Forecast had been use to create mrp' from DUMMY;
		return;

	END IF;

	Delete from APZ_OFCT where "Code" = :absId;
	Delete from APZ_FCT1 where "Code" = :absId;
	select 0, '' from DUMMY;
END




