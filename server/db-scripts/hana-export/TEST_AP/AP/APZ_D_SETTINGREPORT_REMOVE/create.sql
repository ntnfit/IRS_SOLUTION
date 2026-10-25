

CREATE PROCEDURE APZ_D_SettingReport_Remove(in id int default 0)
AS
BEGIN

	delete from APZ_D_SRT2 where "DocEntry" = :id;
	delete from APZ_D_SRT1 where "DocEntry" = :id;
	delete from APZ_D_OSRT where "DocEntry" = :id;
	select 0, '' from dummy;

END



