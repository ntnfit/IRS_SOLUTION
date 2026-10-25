
create PROCEDURE APZ_DocumentDraft_GetDefaultTable(in docEntry int default -1)
AS 
BEGIN
if(docEntry=-1)
then
create local temporary table #tblLocal as (select top 0 *   from ODRF);
select * from #tblLocal;
Drop table #tblLocal;
End if;
END;




