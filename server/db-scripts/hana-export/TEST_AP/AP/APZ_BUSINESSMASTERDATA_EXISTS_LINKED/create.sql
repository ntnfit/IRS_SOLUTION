
CREATE PROCEDURE APZ_BusinessMasterData_exists_linked(
CardCode NVARCHAR(50))
AS
BEGIN
	CREATE LOCAL TEMPORARY TABLE #tbl(
		"ObjType" VARCHAR(20),
		"DocEntry" int
	);

	INSERT INTO #tbl
	        ( "ObjType", "DocEntry" )
	SELECT TOP 1 "TransType","DocEntry" FROM oilm WHERE "BPCardCode"=:CardCode

	UNION SELECT TOP 1 '1250000025',"AbsID" FROM OOAT WHERE "BpCode"=:CardCode

	UNION	SELECT TOP 1 '97',"OpprId" FROM OOPR WHERE "CardCode"=:CardCode
	
	UNION	SELECT TOP 1 '202',"DocEntry" FROM owor WHERE "CardCode"=:CardCode

	UNION	SELECT TOP 1 '33',"DocEntry" FROM OCLG WHERE "CardCode"=:CardCode

	UNION	SELECT TOP 1 '112',"DocEntry" FROM ODRF WHERE "CardCode"=:CardCode

	UNION	SELECT TOP 1 '4',"DocEntry" FROM OITM WHERE "CardCode"=:CardCode

	UNION	SELECT TOP 1 '191',"callID" FROM OSCL WHERE "customer"=:CardCode
	UNION	SELECT TOP 1 '7',0 FROM OSPP WHERE "CardCode"=:CardCode;
	select TOP 1 * from #tbl;
END




