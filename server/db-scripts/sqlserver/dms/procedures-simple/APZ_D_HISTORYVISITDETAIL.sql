USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_HistoryVisitDetail]
    @visitId nvarchar(100),
    @objectType nvarchar(100)
AS
BEGIN
    SET NOCOUNT ON;
SELECT A.*, B.[FileName], ISNULL(B.[LinkThumbnails], ISNULL(B.[LinkDownload], (SELECT TOP 1 [DefaultAvatar] FROM APZ_D_OAPC WHERE [AppName] = 'i-DMS'))) AS [LinkThumbnails], 
	ISNULL(B.[LinkDownload], (SELECT TOP 1 [DefaultAvatar] FROM APZ_D_OAPC WHERE [AppName] = 'i-DMS')) AS [LinkDownload], B.[Size], B.[UploadDate] 
	FROM APZ_D_VISIT A LEFT OUTER JOIN APZ_D_UPIM B ON A.[StoreCode] = B.[CardCode] AND B.[IsAvatar] = 'Y' WHERE [Guid] = @visitId;

	SELECT A.*, B.[Caption], B.[ObjectKey], B.[Active] FROM APZ_D_VISIT1 A LEFT OUTER JOIN APZ_D_VSTEP B ON A.[StepCode] = B.[Code] WHERE [Guid] = @visitId ORDER BY A.[LineNum] ASC;
	
	SELECT * FROM APZ_D_UPIM WHERE [VisitCode] = @visitId AND [IsAvatar] = 'N';
	
	SELECT *,[Dscription] as [ItemName] FROM APZ_D_STC1 WHERE [Code] = @visitId;
	
	SELECT *, (SELECT COUNT(*) FROM APZ_D_CPA1 WHERE [Code] = a.[Code]) AS [NumOfItem] FROM APZ_D_OCPA a WHERE [VisitCode] = @visitId;
	
	SELECT * FROM APZ_D_CPA1 WHERE [VisitCode] = @visitId ORDER BY [LineNum];
	
	SELECT A.*, B.[LinkThumbnails] AS [LinkThumbnails] FROM APZ_D_OCMC A LEFT OUTER JOIN APZ_D_UUPIM B ON A.[UserSign] = B.[UserSign] AND [ObjectType] = @objectType 
	WHERE [RelatedEnt] = (SELECT TOP 1 [StoreCode] FROM APZ_D_VISIT WHERE [Guid] = @visitId) ORDER BY [CreatedTime] DESC;
END;
GO
