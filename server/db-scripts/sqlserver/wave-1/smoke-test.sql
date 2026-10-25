-- Runs every wave-1 module once with real SAP B1 demo data and reports OK / error per call.
-- Read-mostly: APZ_Integrated_CheckIRSAuthorizations and apz_filtersetting_insertquery write IRS rows (by design).
-- sqlcmd -S localhost -E -C -I -d SBODemoAU -i smoke-test.sql
SET NOCOUNT ON;
DECLARE @card nvarchar(50) = (SELECT TOP 1 CardCode FROM OCRD ORDER BY CardCode);
DECLARE @item nvarchar(50) = (SELECT TOP 1 ItemCode FROM OITM WHERE ItemType <> 'F' ORDER BY ItemCode);
DECLARE @bom nvarchar(50) = (SELECT TOP 1 Code FROM OITT ORDER BY Code);
DECLARE @ordr int = (SELECT TOP 1 DocEntry FROM ORDR ORDER BY DocEntry DESC);
DECLARE @wor int = ISNULL((SELECT TOP 1 DocEntry FROM OWOR ORDER BY DocEntry DESC), 0);
DECLARE @oppr int = ISNULL((SELECT TOP 1 OpprId FROM OOPR ORDER BY OpprId DESC), 0);
DECLARE @agr int = ISNULL((SELECT TOP 1 AbsID FROM OOAT ORDER BY AbsID DESC), 0);
DECLARE @uid int = (SELECT USERID FROM OUSR WHERE USER_CODE = N'manager');

DECLARE @calls TABLE (n int IDENTITY, sqltext nvarchar(max));
INSERT INTO @calls (sqltext) VALUES
 (N'EXEC dbo.RA_CONNECTION_GET @UserName = N''manager'''),
 (N'EXEC dbo.APZ_AdditionalGeneralSettings_GetData'),
 (N'EXEC dbo.apz_BOM_SearchADoc @Code = ''' + ISNULL(@bom, N'') + N''''),
 (N'EXEC dbo.apz_BOM_GetLine @father = ''' + ISNULL(@bom, N'') + N''''),
 (N'EXEC dbo.APZ_WorkingTable_GetUserSettings @user_code = N''manager'''),
 (N'EXEC dbo.APZ_Dashboard_GetListDashboardByUser @userName = N''manager'''),
 (N'EXEC dbo.APZ_AdvanceForecast_GetColumnsDoc'),
 (N'EXEC dbo.APZ_AdvanceForecast_GetColumnLine'),
 (N'EXEC dbo.APZ_SalesBlanketAgreement_GetBlanketAgreementDetails @AgrNo = ' + CAST(@agr AS nvarchar) + N', @AgrLnNum = 0'),
 (N'EXEC dbo.APZ_SalesBlanketAgreement_GetLine @AgrNo = ' + CAST(@agr AS nvarchar)),
 (N'EXEC dbo.APZ_SalesBlanketAgreement_GetDoc @FromDate = NULL, @ToDate = NULL, @UserCode = N''manager'''),
 (N'EXEC dbo.APZ_ItemMaster_GetADoc @ItemCode = N''' + ISNULL(@item, N'') + N''''),
 (N'EXEC dbo.APZ_Document_GetBranchList @UserCode = N''manager'''),
 (N'EXEC dbo.apz_filtersetting_insertquery @UserName = N''manager'''),
 (N'EXEC dbo.APZ_Integrated_GetListConfig'),
 (N'EXEC dbo.APZ_Integrated_CheckIRSAuthorizations'),
 (N'SELECT dbo.CompareSplit(N''2'', N''1;2;3'', N'';'')'),
 (N'SELECT dbo.HR_GetRootMenuCode(N''x'')'),
 (N'SELECT TOP 5 * FROM dbo.apz_func_OwnerOverShip(N''manager'', N''2'')'),
 (N'EXEC dbo.apz_alert_getbyuser @UserName = ''manager'', @MailType = 0'),
 (N'EXEC dbo.APZ_SalesOpportunity_GetDoc @OpprId = ' + CAST(@oppr AS nvarchar) + N', @UserName = N''manager'''),
 (N'EXEC dbo.APZ_SalesOpportunity_GetItemDetail @OpprId = -1'),
 (N'EXEC dbo.APZ_SalesOpportunity_GetItemDetail @OpprId = ' + CAST(@oppr AS nvarchar)),
 (N'EXEC dbo.Apz_UDT_GetListUDT @UserName = N''manager'''),
 (N'EXEC dbo.APZ_Commons_GetDefaultData @userName = N''manager'', @branch = -1'),
 (N'EXEC dbo.Apz_Auth_GetAllAuth @userName = N''manager'', @lngCode = N''en-US'''),
 (N'EXEC dbo.Apz_Auth_GetAllAuthForm @userName = N''manager'', @lngCode = N''en-US'', @superUser = N''apzadmin'''),
 (N'EXEC dbo.Apz_Auth_GetAllAuthForm @userName = N''apzadmin'', @lngCode = N''en-US'', @superUser = N''apzadmin'''),
 (N'EXEC dbo.Apz_ProductionOrder_SearchLine @DocEntry = ' + CAST(@wor AS nvarchar)),
 (N'EXEC dbo.apz_ProductionOrder_GetADoc @DocEntry = ' + CAST(@wor AS nvarchar)),
 (N'EXEC dbo.APZ_GeneralSetting_GetFormatString'),
 (N'EXEC dbo.apz_BusinessMasterData_GetADoc @CardCode = N''' + ISNULL(@card, N'') + N''', @UserName = N''manager'''),
 (N'EXEC dbo.Apz_Query_GetListQueries @userName = N''manager'', @branch = N'''''),
 (N'EXEC dbo.apz_formsetting_get @FormId = N''oMain'', @Itemid = N''oMain'', @UserSign = N''-'', @lngCode = N''en-US'''),
 (N'EXEC dbo.apz_formsetting_get @FormId = N''frm_SalesOrder'', @Itemid = N''RDR1'', @UserSign = N''manager'', @lngCode = N''en-US'''),
 (N'EXEC dbo.APZ_SubActivities_GetAllNotification @userId = ' + CAST(@uid AS nvarchar)),
 (N'EXEC dbo.APZ_ProjectManager_GetLineById @Id = 1'),
 (N'EXEC dbo.APZ_Document_Get @tbl = N''ORDR'', @tblLine = N''RDR1'', @UserName = N''manager'''),
 (N'EXEC dbo.APZ_Document_Get @tbl = N''ORDR'', @tblLine = N''RDR1'', @DocEntry = ' + CAST(@ordr AS nvarchar) + N', @UserName = N''manager'''),
 (N'EXEC dbo.apz_Document_GetDocLine @DocEntry = ' + CAST(@ordr AS nvarchar) + N', @tbl = ''ORDR'', @tblLine = N''RDR1''');

DECLARE @i int = 1, @max int = (SELECT MAX(n) FROM @calls), @s nvarchar(max);
DECLARE @results TABLE (n int, callText nvarchar(300), result nvarchar(400));
WHILE @i <= @max
BEGIN
    SELECT @s = sqltext FROM @calls WHERE n = @i;
    BEGIN TRY
        -- results are discarded: only success/failure matters here
        EXEC (N'SET FMTONLY OFF; ' + @s);
        INSERT INTO @results VALUES (@i, LEFT(@s, 300), N'OK');
    END TRY
    BEGIN CATCH
        INSERT INTO @results VALUES (@i, LEFT(@s, 300), CONCAT(N'ERROR ', ERROR_NUMBER(), N': ', ERROR_MESSAGE()));
    END CATCH;
    SET @i += 1;
END;
SELECT N'#RESULT|' + result + N'|' + callText FROM @results ORDER BY CASE WHEN result = N'OK' THEN 1 ELSE 0 END, n;
