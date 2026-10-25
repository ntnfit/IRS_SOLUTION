-- HANA: Apz_ProductionOrder_SearchLine — item, resource and text lines of a production order
CREATE OR ALTER PROCEDURE dbo.Apz_ProductionOrder_SearchLine
    @DocEntry int
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.*, b.ItemName, b.IsCommited, b.OnOrder, b.OnHand, c.OnHand + c.OnOrder - c.IsCommited AS Available, b.InvntryUom,
           c.OnHand AS OnHandInWhse, c.IsCommited AS IsCommitedInWhse, c.OnOrder AS OnOrderInWhse,
           CASE WHEN b.ManBtchNum = 'Y' THEN 'B' WHEN b.ManSerNum = 'Y' THEN 'S' ELSE 'N' END AS ManBtchSerial,
           a.PlannedQty AS OldLineQty, d.PlannedQty AS OldFatherQty
    FROM WOR1 a
    INNER JOIN OITM b ON a.ItemCode = b.ItemCode AND a.ItemType = 4
    INNER JOIN OITW c ON a.ItemCode = c.ItemCode AND a.wareHouse = c.WhsCode
    JOIN OWOR d ON a.DocEntry = d.DocEntry
    WHERE a.DocEntry = @DocEntry
    UNION ALL
    SELECT a.*, b.ResName, 0, 0, 0, 0, N'', 0, 0, 0, 'N', a.PlannedQty, c.PlannedQty
    FROM WOR1 a
    INNER JOIN ORSC b ON a.ItemCode = b.ResCode AND a.ItemType = 290
    JOIN OWOR c ON a.DocEntry = c.DocEntry
    WHERE a.DocEntry = @DocEntry
    UNION ALL
    SELECT *, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL
    FROM WOR1 WHERE DocEntry = @DocEntry AND ItemType = -18
    ORDER BY VisOrder;
END;
