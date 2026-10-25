
Create PROCEDURE Apz_udo_GetTableUDO1(
 in udoType int default -1,
 in colUDO1 nvarchar(19))
as

BEGIN
    DECLARE typeChil INT;
    IF :udoType = 1
    Then
        typeChil=2;
    ELSEIF udoType=3
    THEN
        typeChil=4;
    END IF;
    
    SELECT CASE
               WHEN b."TableName" IS NULL THEN 'N'
               ELSE 'Y' END "IsCheck", a."TableName", a."Descr"
    FROM OUTB  a
        LEFT JOIN UDO1 b ON a."TableName"=b."TableName"
    WHERE (IFNULL(a."UsedInObj", '') =:colUDO1 OR IFNULL(a."UsedInObj", '') ='')
          AND "ObjectType"=:typeChil;
END;



