

CREATE PROCEDURE APZ_D_SettingReport_GetLineById(in id int default 0)
AS
BEGIN

	--select * from APZ_D_SRT1 where "DocEntry" = :id;
	select * from APZ_D_SRT2 where "DocEntry" = :id order by "Id" asc;

END




