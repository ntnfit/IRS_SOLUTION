-- HANA: Apz_UDT_GetListUDT — user-defined tables the user may open (all when no user given)
CREATE OR ALTER PROCEDURE dbo.Apz_UDT_GetListUDT
    @UserName nvarchar(20) = N'',
    @Branch nvarchar(20) = N''
AS
BEGIN
    SET NOCOUNT ON;
    IF @UserName = N''
        SELECT ISNULL(a.TableName, N'''') AS TableName, ISNULL(a.Descr, N'''') AS Descr
        FROM OUTB a
        WHERE a.ObjectType = 0 OR a.ObjectType = 5
        ORDER BY a.TableName;
    ELSE
        SELECT ISNULL(a.TableName, N'') AS TableName, ISNULL(a.Descr, N'') AS Descr
        FROM OUTB a
        INNER JOIN dbo.OPRM b ON N'UDT_' + a.TableName = b.menucode
        WHERE (a.ObjectType = 0 OR a.ObjectType = 5) AND b.USER_CODE = @UserName AND b.authoriza IN ('F', 'R')
        ORDER BY a.TableName;
END;
