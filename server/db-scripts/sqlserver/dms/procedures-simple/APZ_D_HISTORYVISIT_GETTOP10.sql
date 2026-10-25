USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_HistoryVisit_GetTop10]
    @storeCode nvarchar(100) = ''
AS
BEGIN
    SET NOCOUNT ON;
SELECT TOP 10 A.*, ISNULL((CAST((SELECT COUNT(*) FROM ORDR C WHERE a.[StoreCode] = C.[CardCode] AND C.U_GUID = A.[Guid]) AS integer) + 
	CAST((SELECT COUNT(*) FROM OINV D WHERE a.[StoreCode] = D.[CardCode] AND U_GUID = D.U_GUID) AS integer)), 0) AS [OrderNumber], 
	ISNULL([firstName], '') + ' ' + ISNULL([middleName], '') + ' ' + ISNULL([lastName], '') AS [U_fullName] 
	FROM APZ_D_VISIT A INNER JOIN OHEM B ON A.[UserSign] = B.[userId] WHERE A.[StoreCode] = @storeCode ORDER BY [DocEntry] DESC;
END;
GO
