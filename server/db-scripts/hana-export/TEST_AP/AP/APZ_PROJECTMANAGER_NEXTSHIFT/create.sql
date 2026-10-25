
CREATE PROCEDURE APZ_ProjectManager_NextShift (code nvarchar(50) DEFAULT '')
AS
BEGIN
    select ROW_NUMBER() over (order by a."Start1") as ID,* from (select "Start1", "End1" from apz_OSHP where
    "Code" in (SELECT DATA FROM Split(:code,','))
    union all
    select "Start2", "End2" from apz_OSHP
    where
    "Code" in (SELECT DATA FROM Split(:code,',')) and "End2" > 0) a order by "Start1";
END



