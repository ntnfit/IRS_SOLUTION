
CREATE PROCEDURE APZ_SubActivities_SaveInterval(
	in clgCode int,
	in durType char(1),
	in duration numeric(19,6),
	in recontact datetime,
	in startTime int,
	in endDate timestamp,
	in endTime int,
	in groupUser nvarchar(4000) default '',
	in groupEmp nvarchar(4000) default '',
	in resourceIdsInfo nvarchar(4000) default '')
AS
BEGIN
	if(:groupUser != '' OR :groupEmp != '')
	THEN
		UPDATE APZ_OCLG SET "DurType" = :durType, "Duration" = :duration, "Recontact" = :recontact, "BeginTime" = :startTime, "endDate" = :endDate, "ENDTime" = :endTime
			, "GroupUser" = :groupUser, "GroupEmployee" = :groupEmp, "ResourceIdsInfo" = :resourceIdsInfo, "AllDayEvent" = 0, "UpdateDate" = CURRENT_DATE
			, "UpdateTime" = (SELECT CAST(REPLACE((TO_VARCHAR(NOW(), 'hh:mm')), ':', '') AS integer) FROM DUMMY) WHERE "ClgCode" = :clgCode;
		--and Closed = 'N'
		select cast(:clgCode as nvarchar) from DUMMY;
	else
	
		UPDATE "APZ_OCLG" SET "DurType" = :durType, "Duration" = :duration, "Recontact" = :recontact, "BeginTime" = :startTime, "endDate" = :endDate, "ENDTime" = :endTime, "AllDayEvent" = 0
			, "UpdateDate" = CURRENT_DATE, "UpdateTime" = (SELECT CAST(REPLACE((TO_VARCHAR(NOW(), 'hh:mm')), ':', '') AS integer) FROM DUMMY) WHERE "ClgCode" = :clgCode;
		--  and Closed = 'N'

		select cast(:clgCode as nvarchar) from DUMMY;
	end IF;
END





