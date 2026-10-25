-- HANA: APZ_GeneralSetting_GetFormatString — company formats from OADM
CREATE OR ALTER PROCEDURE dbo.APZ_GeneralSetting_GetFormatString
AS
BEGIN
    SET NOCOUNT ON;
    SELECT a.Code, a.CompnyName, a.CompnyAddr, a.PrintHeadr, a.Manager, a.AliasName, a.Phone1, a.Phone2, a.Fax,
           ISNULL(a.E_Mail, N'') AS E_Mail, a.TaxIdNum, a.HldCode, a.MainCurncy, a.SysCurrncy,
           CASE a.DfActCurr WHEN 'Y' THEN 'All Currencies' WHEN 'N' THEN 'Local Currency' END AS DfActCurr,
           CASE a.InvntSystm WHEN 'F' THEN 'FIFO' WHEN 'S' THEN 'Standard' WHEN 'A' THEN 'Moving Averager' END AS InvntSystm,
           CASE a.TimeFormat WHEN '0' THEN '24H' WHEN '1' THEN '12H' END AS TimeFormat,
           CASE a.DateFormat WHEN '0' THEN N'DD/MM/YY' WHEN '1' THEN N'DD/MM/CCYY' WHEN '2' THEN N'MM/DD/YY'
                             WHEN '3' THEN N'MM/DD/CCYY' WHEN '4' THEN N'CCYY/MM/DD' WHEN '5' THEN N'DD/Month/YYYY'
                             WHEN '6' THEN N'YY/MM/DD' END AS DateFormat,
           a.DateSep, a.SumDec, a.QtyDec, a.PriceDec, a.RateDec, a.PercentDec, a.DecSep, a.ThousSep,
           ISNULL(a.ClosedQuot, 'N') AS ClosedQuot
    FROM OADM a;
END;
