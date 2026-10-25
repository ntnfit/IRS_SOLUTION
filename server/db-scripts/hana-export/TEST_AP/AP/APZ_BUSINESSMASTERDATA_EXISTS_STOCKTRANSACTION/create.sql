
CREATE PROCEDURE APZ_BusinessMasterData_exists_stocktransaction(
CardCode NVARCHAR(50))
AS
BEGIN
	SELECT TOP 1 T1."TransType", T1."DocEntry" FROM oivl T0 INNER JOIN OILM T1 ON T0."MessageID" = T1."MessageID" WHERE T1."BPCardCode" = :CardCode;
END




