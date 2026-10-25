
CREATE PROCEDURE APZ_BusinessMasterData_all_linkedtype
(in CardCode NVARCHAR(50))
AS
BEGIN
CREATE LOCAL TEMPORARY TABLE #tbl
	(
		"ObjType" VARCHAR(20)
	);

INSERT INTO #tbl ("ObjType") (
SELECT TOP 1 "TransType" FROM oilm WHERE "BPCardCode" = :CardCode 
UNION SELECT TOP 1 '1250000025' FROM OOAT WHERE "BpCode" = :CardCode 
UNION SELECT TOP 1 '97' FROM OOPR WHERE "CardCode" = :CardCode 
UNION SELECT TOP 1 '202' FROM owor WHERE "CardCode" = :CardCode 
UNION SELECT TOP 1 '33' FROM OCLG WHERE "CardCode" = :CardCode 
UNION SELECT TOP 1 '112' FROM ODRF WHERE "CardCode" = :CardCode 
UNION SELECT TOP 1 '4' FROM OITM WHERE "CardCode" = :CardCode 
UNION SELECT TOP 1 '191' FROM OSCL WHERE "customer" = :CardCode 
UNION SELECT TOP 1 '7' FROM OSPP WHERE "CardCode" = :CardCode 
UNION SELECT TOP 1 '23' FROM OQUT WHERE "CardCode" = :CardCode 
UNION SELECT TOP 1 '18' FROM OPCH WHERE "CardCode" = :CardCode AND IFNULL("isIns", 'N') = 'N' 
UNION SELECT TOP 1 '20' FROM OPDN WHERE "CardCode" = :CardCode 
UNION SELECT TOP 1 '13' FROM OINV WHERE "CardCode" = :CardCode AND IFNULL("isIns", 'N') = 'N' 
UNION SELECT TOP 1 '15' FROM ODLN WHERE "CardCode" = :CardCode 
UNION SELECT TOP 1 '540000006' FROM OpQT WHERE "CardCode" = :CardCode-- Delivery
-- AR Invoice
-- Goods receipt PO
-- AP Invoice
);
select * from #tbl;
END




