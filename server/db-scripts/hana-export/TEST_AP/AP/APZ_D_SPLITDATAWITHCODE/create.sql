
CREATE FUNCTION APZ_D_SplitDataWithCode (

    Code nvarchar(50),

    RowData nclob,

    SplitOn nvarchar(5)

)

RETURNS TABLE (

    "Code" nvarchar(50)

    ,"DATA" nvarchar(5000)

)

AS

BEGIN

    DECLARE _items nvarchar(1000) ARRAY;

    DECLARE _index integer;

    _index := 1;

    WHILE LOCATE(:RowData,:SplitOn) > 0 DO

        _items[:_index] := trim(SUBSTR_BEFORE(:RowData,:SplitOn));

        RowData := SUBSTR_AFTER(:RowData,:SplitOn);

        _index := :_index + 1;

    END WHILE;

    _items[:_index] := TRIM(:RowData);

    rst = UNNEST(:_items) AS ("DATA");

    rs = SELECT :Code as "Code", * FROM :rst;

    RETURN :rs;

END;

