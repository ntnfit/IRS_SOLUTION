-- HANA: apz_BusinessMasterData_GetADoc — BP header with contact, ship-to street, payment terms and avatar,
-- filtered by data ownership when CINF.DoFilter = 'Y' and OADM.DoMngMth in ('A','B')
CREATE OR ALTER PROCEDURE dbo.apz_BusinessMasterData_GetADoc
    @CardCode nvarchar(50) = N'',
    @UserName nvarchar(200) = N'',
    @BPLId int = 0
AS
BEGIN
    SET NOCOUNT ON;
    DECLARE @dofilter char(1) = ISNULL((SELECT TOP 1 DoFilter FROM CINF), 'N');
    DECLARE @DoMngMth char(1) = ISNULL((SELECT TOP 1 DoMngMth FROM OADM), '');

    SELECT a.*, b.CntctCode, d.Street AS MailAddress, b.Name AS CntPrsName, f.PayDuMonth, f.ExtraMonth, f.ExtraDays, f.BslineDate,
           ISNULL((SELECT TOP 1 u.LinkThumbnails FROM dbo.APZ_D_UPIM u
                   WHERE u.IsAvatar = 'Y' AND u.CardCode = a.CardCode
                   ORDER BY u.UploadDate, u.UploadTime DESC), N'') AS CloudImage
    FROM OCRD a
    LEFT JOIN OCPR b ON a.CardCode = b.CardCode AND a.CntctPrsn = b.Name
    LEFT JOIN CRD1 d ON a.CardCode = d.CardCode AND a.ShipToDef = d.Address AND d.AdresType = 'S'
    LEFT JOIN CRD1 e ON a.CardCode = e.CardCode AND a.BillToDef = e.Address AND e.AdresType = 'B'
    LEFT JOIN OCTG f ON a.GroupNum = f.GroupNum
    WHERE a.CardCode = @CardCode
      AND (@dofilter = 'N'
           OR (@dofilter = 'Y' AND @DoMngMth NOT IN ('A', 'B'))
           OR (@dofilter = 'Y' AND @DoMngMth IN ('A', 'B')
               AND (ISNULL(a.OwnerCode, -1) = -1
                    OR a.OwnerCode IN (SELECT DISTINCT OwnerId FROM dbo.apz_func_OwnerOverShip(@UserName, N'2')))));
END;
