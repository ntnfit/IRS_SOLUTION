
CREATE FUNCTION CompareListValueSplit(
	rowCompareData nvarchar(5000),
	RowData	nvarchar(5000),
	rowCompareSplitOn nvarchar(50),
	SplitOn	nvarchar(50)
)
RETURNS result smallint
AS
BEGIN
	result = IFNULL((select top 1 1 from split(:rowCompareData, :rowCompareSplitOn) x where x.Data in (select DATA from split(:RowData, :SplitOn))),0);
END;




