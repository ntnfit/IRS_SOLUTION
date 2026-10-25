Create PROCEDURE APZ_D_SettingReport_GetUserReportParams(
	in docEntry int,
	in userCode nvarchar(50)
)
AS
BEGIN
	select * from APZ_D_SRT4
	where "DocEntry" = :docEntry and "UserCode" = :userCode;
end



