USE [SBODemoAU];
GO
CREATE OR ALTER PROCEDURE dbo.[APZ_D_UploadFile_GetFileByOjectType]
    @objectType nvarchar(50) = '',
    @objectCode nvarchar(50) = ''
AS
BEGIN
    SET NOCOUNT ON;
select * from APZ_D_UUPIM 
	where (ISNULL(@objectType, '') = '' or [ObjectType] = @objectType) 
	and (ISNULL(@objectCode, '') = '' or [ObjectCode] = @objectCode)
	order by [UploadDate] desc;
END;
GO
