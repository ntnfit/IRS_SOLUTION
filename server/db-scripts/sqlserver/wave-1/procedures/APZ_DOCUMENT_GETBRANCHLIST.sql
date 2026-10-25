-- HANA: APZ_Document_GetBranchList
CREATE OR ALTER PROCEDURE dbo.APZ_Document_GetBranchList
    @UserCode nvarchar(30) = N''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT ISNULL(a.BPLId, 0) AS BPLId, a.BPLName, ISNULL(a.VATRegNum, N'') AS VATRegNum,
           ISNULL(a.DflWhs, N'') AS DflWhs, ISNULL(a.County, N'') AS County,
           ISNULL(a.AliasName, N'') AS AliasName, a.State, a.Country,
           ISNULL(a.BPLId, 0) AS Value, a.BPLName AS Name
    FROM OBPL a
    INNER JOIN USR6 b ON a.BPLId = b.BPLId
    WHERE a.Disabled = 'N' AND b.UserCode = @UserCode
    ORDER BY a.BPLId;
END;
