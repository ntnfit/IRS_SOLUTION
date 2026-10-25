-- HANA: APZ_SalesBlanketAgreement_GetLine
CREATE OR ALTER PROCEDURE dbo.APZ_SalesBlanketAgreement_GetLine
    @AgrNo int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT OAT1.*, OITM.InvntryUom, OITM.UgpEntry, OITM.SalUnitMsr AS SalUnitMsr, ISNULL(OITM.NumInSale, 0) AS NumInSale,
           OITM.BuyUnitMsr, ISNULL(OITM.NumInBuy, 0) AS NumInBuy, ISNULL(OITM.UgpEntry, -1) AS UgpCode,
           OITM.InvntryUom AS InvUnitMsr, ISNULL(d.AltQty, 1) / ISNULL(d.BaseQty, 1) AS NumInInven,
           CAST(1 AS numeric(19, 6)) AS Rate, CAST(-1 AS bigint) AS OpenAmntLC, CAST(-1 AS bigint) AS OpenAmntFC
    FROM OAT1
    INNER JOIN OITM ON OAT1.ItemCode = OITM.ItemCode
    LEFT JOIN UGP1 d ON OITM.UgpEntry = d.UgpEntry
                    AND d.UomEntry IN (SELECT UomEntry FROM OUOM WHERE UomName = OITM.InvntryUom)
    WHERE OAT1.AgrNo = @AgrNo
    ORDER BY OAT1.VisOrder;
END;
