
CREATE PROCEDURE APZ_BarcodeInventoryCounting_SearchDoc(
	in docEntry int default null,
	in countDate timestamp default null,
	in takerType int default null,
	in takerId nvarchar(254) default null,
	in whsCode nvarchar(50) default null,
	in binCode nvarchar(50) default null,
	in status nvarchar(10) default null)
AS
BEGIN
	
	SELECT * FROM APZ_OBIC WHERE :docEntry IS NOT NULL AND "DocEntry" = :docEntry OR :docEntry IS NULL AND (:countDate IS NULL OR "CountDate" = :countDate) AND (:takerType IS NULL OR "TakerType" = :takerType) AND (:takerId IS NULL OR "TakerId" = :takerId) AND (:whsCode IS NULL OR "WhsCode" = :whsCode) AND (:binCode IS NULL OR "BinCode" = :binCode) AND (:status IS NULL OR "Status" = :status);
	
END




