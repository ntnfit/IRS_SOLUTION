-- HANA: APZ_SalesBlanketAgreement_GetDoc (owner filter was commented out in HANA too — "chưa convert")
CREATE OR ALTER PROCEDURE dbo.APZ_SalesBlanketAgreement_GetDoc
    @FromDate datetime = NULL,
    @ToDate datetime = NULL,
    @BpCode nvarchar(50) = N'',
    @Status char(1) = '',
    @BpType nvarchar(1) = N'',
    @UserCode nvarchar(50) = N'',
    @Number int = 0,
    @AbsID int = 0
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @maxdate datetime = ISNULL((SELECT MAX(SignDate) FROM OOAT), GETDATE());
    DECLARE @mindate datetime = ISNULL((SELECT MIN(SignDate) FROM OOAT), GETDATE());

    SELECT a.*, CAST(N'' AS nvarchar(100)) AS Name, b.Tel1, b.E_MailL
    INTO #tbl
    FROM OOAT a
    LEFT JOIN OCPR b ON a.CntctCode <> 0 AND a.CntctCode = b.CntctCode
    WHERE a.SignDate BETWEEN ISNULL(@FromDate, @mindate) AND ISNULL(@ToDate, @maxdate)
      AND (ISNULL(@BpType, N'') = N'' OR a.BpType = @BpType)
      AND (ISNULL(@BpCode, N'') = N'' OR a.BpCode = @BpCode)
      AND (ISNULL(@Status, '') = '' OR a.Status = @Status OR (@Status = 'C' AND a.Cancelled = 'Y'))
      AND (@Number = 0 OR (@Number > 0 AND a.Number = @Number))
      AND (@AbsID = 0 OR (@AbsID > 0 AND a.AbsID = @AbsID));

    UPDATE #tbl SET Status = 'C' WHERE Cancelled = 'Y';
    SELECT * FROM #tbl ORDER BY AbsID DESC;
    DROP TABLE #tbl;
END;
