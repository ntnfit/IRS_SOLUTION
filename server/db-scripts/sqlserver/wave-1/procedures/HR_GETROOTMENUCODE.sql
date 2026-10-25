-- HANA: function HR_GetRootMenuCode — walks APZ_OMNI up to the root menu code
CREATE OR ALTER FUNCTION dbo.HR_GetRootMenuCode (@p_Code nvarchar(50))
RETURNS nvarchar(50)
AS
BEGIN
    DECLARE @current nvarchar(50) = @p_Code, @fatherId int, @guard int = 0;
    WHILE @guard < 100  -- protects against cycles in the menu tree
    BEGIN
        SET @fatherId = ISNULL((SELECT TOP 1 FatherId FROM dbo.APZ_OMNI WHERE Code = @current), -1);
        IF @fatherId = -1 BREAK;
        SET @current = ISNULL((SELECT TOP 1 Code FROM dbo.APZ_OMNI WHERE Id = @fatherId), N'');
        SET @guard += 1;
    END;
    RETURN @current;
END;
