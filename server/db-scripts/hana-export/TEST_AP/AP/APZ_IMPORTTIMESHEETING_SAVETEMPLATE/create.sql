
CREATE PROCEDURE APZ_ImportTimeSheeting_SaveTemplate(in UserSign varchar(50) default '',in TableName varchar(100) default '')
AS
BEGIN
	--Bulk Copy
	Create local temporary table #APZ_OTSIData as (select top 0 * from APZ_OTSI);
	exec 'insert into #APZ_OTSIData select * from "#dtAPZ_OTSI";';
	--Insert Data
	DELETE FROM APZ_OTSI WHERE "UserSign"=UserSign AND "TableName"=TableName;
    INSERT INTO APZ_OTSI SELECT * FROM #APZ_OTSIData;
END



