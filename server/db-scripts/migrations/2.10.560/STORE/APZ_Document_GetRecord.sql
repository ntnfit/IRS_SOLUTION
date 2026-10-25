
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE type='p' and id = OBJECT_ID('APZ_Document_GetRecord'))
BEGIN
    DROP PROCEDURE APZ_Document_GetRecord
END

GO

CREATE  PROC APZ_Document_GetRecord
@ObjType int = 0,
@tbl NVARCHAR(10) ,
@DocNum int =0,
@TypeRecord nvarchar(1) ='',
@userCode nvarchar(25)
as
 DECLARE @SQL NVARCHAR(MAX)
 Declare @id int
 declare @isInsQuery nvarchar(50)=''

 if @ObjType in (13, 18)
	set @isInsQuery = ' and isIns=''N'' '
else if @ObjType in (13000, 25)
	set @isInsQuery = ' and isIns=''Y'' '

 --Get First
 IF @TypeRecord in ('F', 'L')
BEGIN
	  SET @SQL = 'SELECT top 1 A.DocEntry, A.DocNum from ' + @tbl + ' A WHERE
	  (A.OwnerCode IN (select distinct x.OwnerId from apz_func_OwnerOverShip(N'''+@userCode + ''',''' + cast(@ObjType as nvarchar) +''') x) OR ISNULL(A.OwnerCode,-1)=-1)'
	  + @isInsQuery +
	  'and (isnull(a.BPLId, -1) = -1 or a.BPLId in (select x.BPLId from OBPL x inner join USR6 y on x.BPLId = y.BPLId where Disabled = ''N'' and y.UserCode = ''' + @userCode + '''))
	  order by ' + case when @TypeRecord = 'F' then 'DocNum, DocEntry' else 'DocNum desc, DocEntry desc' end;

	  execute(@SQL)

END
ELSE
BEGIN
	create table #cte(DocEntry int, DocNum int)
	 set @SQL = 'insert into #cte select DocEntry, DocNum from ' + @tbl + ' where 
	 ((OwnerCode IN ( select distinct x.OwnerId from apz_func_OwnerOverShip(N'''+@userCode + ''',''' + cast(@ObjType as nvarchar) +''') x)) OR ISNULL(OwnerCode,-1)=-1)'
	  + @isInsQuery +
	  'and (isnull(BPLId, -1) = -1 or BPLId in (select x.BPLId from OBPL x inner join USR6 y on x.BPLId = y.BPLId where Disabled = ''N'' and y.UserCode = ''' + @userCode + '''))'
	  execute(@SQL)
			
	  if @TypeRecord = 'N'
	begin
		IF @DocNum >= isnull((SELECT MAX(DocNum) from #cte), 0)
			select top 1 DocEntry, DocNum from #cte order by DocNum, DocEntry
		select top 1 DocEntry, DocNum from #cte where DocNum > @DocNum order by DocNum, DocEntry
	end
	else
	begin
		IF @DocNum <= isnull((SELECT MIN(DocNum) from #cte), 0)
			select top 1 DocEntry, DocNum from #cte order by DocNum desc, DocEntry desc
		select top 1 DocEntry, DocNum from #cte where DocNum < @DocNum order by DocNum desc, DocEntry desc
	end
END
  