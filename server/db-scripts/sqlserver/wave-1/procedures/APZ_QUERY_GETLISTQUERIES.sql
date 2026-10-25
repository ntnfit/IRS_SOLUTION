-- HANA: Apz_Query_GetListQueries — result 1: grid structure; result 2: SAP saved queries in the
-- query authorization groups granted to the user (OPRM menucode 'barSub_QueryAuthGrp<id>').
CREATE OR ALTER PROCEDURE dbo.Apz_Query_GetListQueries
    @userName nvarchar(20) = N'',
    @branch nvarchar(20) = N''
AS
BEGIN
    SET NOCOUNT ON;
    SELECT * FROM (VALUES
        (N'IntrnalKey', N'IntrnalKey', 'int',      '', 75,  -1, 0),
        (N'QCategory',  N'QCategory',  'int',      '', 75,  -1, 0),
        (N'CatName',    N'Category',   'nvarchar', '', 75,   0, 1),
        (N'QName',      N'QName',      'nvarchar', '', 200, -1, 1)
    ) s (ColumnName, Caption, DataType, SubType, Width, GroupIndex, Visible)
    CROSS APPLY (SELECT CAST(N'' AS nvarchar(20)) AS FormatType, CAST(N'' AS nvarchar(20)) AS DisplayFormat,
                        CAST(NULL AS smallint) AS ReadOnly, CAST(NULL AS smallint) AS AllowEdit, CAST(NULL AS smallint) AS AllowMove,
                        CAST(NULL AS smallint) AS AllowSize, CAST(NULL AS smallint) AS AllowSort, CAST('' AS varchar(1)) AS Fixed,
                        CAST('' AS varchar(10)) AS SummaryType, CAST('' AS varchar(10)) AS GroupSummaryType) d;

    -- 'barSub_QueryAuthGrp' is 19 characters; the group id follows it
    SELECT a.IntrnalKey, b.CategoryId AS QCategory, b.CatName,
           CASE WHEN a.QName IS NULL THEN N'No queries belong to this group' ELSE a.QName END AS QName,
           CASE WHEN a.QString IS NULL THEN CAST(N'' AS nvarchar(max)) ELSE CAST(a.QString AS nvarchar(max)) END AS QString,
           a.QType, b.PermMask
    FROM OUQR a
    RIGHT JOIN OQCN b ON a.QCategory = b.CategoryId
    WHERE b.CategoryId IN (
        SELECT q.CATEGORYID
        FROM QAG1 q
        JOIN (SELECT SUBSTRING(menucode, 20, 50) AS GroupId FROM dbo.OPRM
              WHERE menucode LIKE N'barSub_QueryAuthGrp%' AND USER_CODE = @userName AND authoriza = N'F') g
          ON CAST(q.AUTHGRPID AS nvarchar(50)) = g.GroupId
        JOIN OQAG c ON CAST(c.AUTHGRPID AS nvarchar(50)) = g.GroupId
        WHERE c.Deleted = 'N')
    ORDER BY b.CategoryId, a.QName;
END;
