-- HANA: CompareSplit — 1 when compareData equals one of the SplitOn-separated items of RowData, else 0
CREATE OR ALTER FUNCTION dbo.CompareSplit
(
    @compareData nvarchar(4000),
    @RowData nvarchar(4000),
    @SplitOn nvarchar(5)
)
RETURNS smallint
AS
BEGIN
    DECLARE @pos int = CHARINDEX(@SplitOn, @RowData);
    WHILE @pos > 0
    BEGIN
        IF LEFT(@RowData, @pos - 1) = @compareData
            RETURN 1;
        SET @RowData = SUBSTRING(@RowData, @pos + LEN(@SplitOn), 4000);
        SET @pos = CHARINDEX(@SplitOn, @RowData);
    END;
    RETURN CASE WHEN @RowData = @compareData THEN 1 ELSE 0 END;
END;
