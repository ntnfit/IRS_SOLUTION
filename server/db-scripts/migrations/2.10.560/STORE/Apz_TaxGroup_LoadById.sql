
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE type='p' and id = OBJECT_ID('Apz_TaxGroup_LoadById'))
BEGIN
    DROP PROCEDURE Apz_TaxGroup_LoadById
END

GO

--Apz_Cfl_LoadBusinessPartner 'S',@bpType = 'E'
CREATE PROC Apz_TaxGroup_LoadById 
	@code NVARCHAR(20) = ''
AS
BEGIN
	Select Code,Name,Rate From OVTG Where ISNULL(Inactive,'N') = 'N' AND ISNULL(Code,'') = @code;
END