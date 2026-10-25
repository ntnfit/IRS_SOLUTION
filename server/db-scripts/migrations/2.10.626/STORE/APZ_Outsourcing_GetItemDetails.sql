
CALL APZ_EXISTSPROC('APZ_Outsourcing_GetItemDetails');

GO

CREATE PROCEDURE APZ_Outsourcing_GetItemDetails
(
	in itemCode nvarchar(50) default ''
)
AS
BEGIN
	DECLARE whsCode nvarchar(50) = '';
	Create local temporary table #tmp ("Factor" numeric(16,9));
	
	IF Not Exists(select 1 from OITT where "Code" = :itemCode)-- not a BOM
	THEN
		Select IFNULL((select "DfltWH" from OITM where "ItemCode" = :itemCode), '') Into whsCode From Dummy;
	ELSE -- a BOM
		Select IFNULL((select "ToWH" from OITT where "Code" = :itemCode), '') Into whsCode From Dummy;
	END IF;
	
	INSERT INTO #tmp Select 1 From Dummy;
	Select "ItemName", Cast(:whsCode AS Nvarchar) As "WhsCode", "ManBtchNum", "ManSerNum", (select top 1 "Factor" from #tmp) As "AmountFactor"
	from OITM
	Where "ItemCode" = :itemCode;
	
END