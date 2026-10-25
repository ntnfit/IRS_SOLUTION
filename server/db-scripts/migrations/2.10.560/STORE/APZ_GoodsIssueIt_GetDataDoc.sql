
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE type='p' and id = OBJECT_ID('APZ_GoodsIssueIt_GetDataDoc'))
BEGIN
    DROP PROCEDURE APZ_GoodsIssueIt_GetDataDoc
END

GO

CREATE PROCEDURE APZ_GoodsIssueIt_GetDataDoc
	@basetype int = -1,
	@docNum int = 0,
	@docEntry int = 0,
	@priceList int = 0,
	@fromDate datetime = '',
	@toDate datetime = '',
	@type int = 0,
	@userCode nvarchar(25)
AS
BEGIN
	
	if @type = 4
	BEGIN
		select *, (SELECT SUM( ISNULL(B.LineTotal,0) )  FROM  IGE1 B WHERE B.DocEntry = a.DocEntry)  As TotalBeforeDis , a.DocCur Currency 
		from OIGE a where exists (select top 1 docentry from IGE1 where docentry=a.docentry and BaseType=@basetype) and 
								 ((isnull(@docEntry,0) <> 0 and a.DocEntry = @docEntry) or  
								 (isnull(@docEntry,0) = 0 and (isnull(@fromDate, '') = '' or a.DocDate >= @fromDate) and (isnull(@toDate,'') = '' or a.DocDate <= @toDate))
								 AND (isnull(@docNum, 0) = 0 or a.DocNum = @docNum) and (isnull(@priceList,0) = 0 or a.GroupNum = @priceList))
							AND (isnull(a.OwnerCode, -1) = -1 or a.OwnerCode in (select distinct OwnerId from apz_func_OwnerOverShip(@userCode, '60')))
							AND (isnull(a.BPLId, -1) = -1 or a.BPLId in (select x.BPLId from OBPL x inner join USR6 y on x.BPLId = y.BPLId where x."Disabled" = 'N' and y.UserCode = @userCode))
					ORDER BY DocNum DESC

		return
	END

	select a.DocEntry, a.DocNum into #lstDoc from OIGE a where 
	exists (select 1 from IGE1 where DocEntry = a.DocEntry and "BaseType" = @basetype)
	AND (isnull(a.OwnerCode, -1) = -1 or a.OwnerCode in (select distinct OwnerId from apz_func_OwnerOverShip(@userCode, '60')))
	AND (isnull(a.BPLId, -1) = -1 or a.BPLId in (select x.BPLId from OBPL x inner join USR6 y on x.BPLId = y.BPLId where x.Disabled = 'N' and y.UserCode = @userCode))

	if @type = 1
	BEGIN



		if @docNum <= isnull((select min(DocNum) from #lstDoc),0)
			set @DocNum = ISNULL((select max(DocNum) from #lstDoc), 0) + 1

		select top 1 a.*, (SELECT SUM( ISNULL(B.LineTotal,0) )  FROM  IGE1 B WHERE B.DocEntry = a.DocEntry)  As TotalBeforeDis , a.DocCur Currency 
									from OIGE a where a.DocNum < @DocNum and a.DocEntry in (select DocEntry from #lstDoc)
									order by a.DocEntry DESC

	END
	ELSE IF @type = 2
	BEGIN

		if @DocNum = isnull((select max(DocNum) from #lstDoc),0)
		begin
			set @DocNum = ISNULL((select min(DocNum) from #lstDoc), 0) - 1
		end

		select top 1 a.*, (SELECT SUM( ISNULL(B.LineTotal,0) )  FROM  IGE1 B WHERE B.DocEntry = a.DocEntry)  As TotalBeforeDis , a.DocCur Currency 
								from OIGE a where a.DocNum > @DocNum and a.DocEntry in (select DocEntry from #lstDoc)
								order by DocEntry ASC

	END
	else if @type = 3
	begin

		select a.*, (SELECT SUM( ISNULL(B.LineTotal,0) )  FROM  IGE1 B WHERE B.DocEntry = a.DocEntry)  As TotalBeforeDis , a.DocCur Currency 
								from OIGE a where DocNum = isnull((select max(DocNum) from #lstDoc),0)

	end
	ELSE IF @type = 0
	begin

		select a.*, (SELECT SUM( ISNULL(B.LineTotal,0) )  FROM  IGE1 B WHERE B.DocEntry = a.DocEntry)  As TotalBeforeDis , a.DocCur Currency 
								from OIGE a where DocNum = isnull((select MIN(DocNum) from #lstDoc),0)

	end

END
