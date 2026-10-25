
Create PROCEDURE APZ_D_SettingApprovalDetail_GetDataSource(in docNo NVARCHAR(100) default '')
AS
BEGIN
    select top 1 * from ODRF where "DocEntry" = :docNo;
    select * from DRF1 where "DocEntry" = :docNo;
end



