-- HANA: RA_CONNECTION_GET (UserName is accepted but unused, as in HANA)
CREATE OR ALTER PROCEDURE dbo.RA_CONNECTION_GET
    @UserName nvarchar(50) = N''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM dbo.[@RA_CONN];
END;
