-- HANA: APZ_SalesOpportunity_GetDoc
CREATE OR ALTER PROCEDURE dbo.APZ_SalesOpportunity_GetDoc
    @OpprId int = 0,
    @UserName nvarchar(50)
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.*,
           (SELECT ISNULL(lastName, N'') + N', ' + ISNULL(firstName, N'') FROM OHEM WHERE empID = a.Owner) AS OwnerName,
           ISNULL(a.Owner, -1) AS OwnerCode, b.MailAddres, CAST('VND' AS nvarchar(3)) AS DocCurr
    FROM OOPR a
    LEFT JOIN OCRD b ON a.CardCode = b.CardCode
    WHERE a.OpprId = @OpprId
      AND (ISNULL(a.Owner, -1) = -1
           OR a.Owner IN (SELECT DISTINCT x.OwnerId FROM dbo.apz_func_OwnerOverShip(@UserName, N'97') x)
           OR @UserName = N'manager');
END;
