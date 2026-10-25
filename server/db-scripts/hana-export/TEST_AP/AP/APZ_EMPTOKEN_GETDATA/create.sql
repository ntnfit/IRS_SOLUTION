
CREATE PROCEDURE APZ_EmpToken_GetData
(in UserId int)
as
BEGIN
 declare query nvarchar(5000);
 declare checkUdf int;
SELECT COUNT(*) into checkUdf FROM TABLE_COLUMNS
WHERE column_name = N'U_OldCode' AND table_name = 'OHEM';
IF(checkUdf >0)
then
query = N'SELECT m.U_OldCode,m.EmpID ,ISNULL(m.firstName,'''') + '' '' + ISNULL(m.lastName,'''') + '' '' +ISNULL(m.middleName,'''') AS FullName ,t.TokenStr FROM APZ_OHET t INNER JOIN OHEM m ON t.EmpId = m.EmpId INNER JOIN APZ_HET1 a ON t.EmpId = a.EmpId WHERE a.UserId = ' + TO_NVARCHAR(:UserId); 

ELSE

query = N'SELECT m.EmpID ,ISNULL(m.firstName,'''') + '' '' + ISNULL(m.lastName,'''') + '' '' +ISNULL(m.middleName,'''') AS FullName ,t.TokenStr FROM APZ_OHET t INNER JOIN OHEM m ON t.EmpId = m.EmpId INNER JOIN APZ_HET1 a ON t.EmpId = a.EmpId WHERE a.UserId = ' + TO_NVARCHAR(:UserId);

end if;

execute immediate :query;
END;




