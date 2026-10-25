
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE type='p' and id = OBJECT_ID('APZ_D_HistoryVisit'))
BEGIN
    DROP PROCEDURE APZ_D_HistoryVisit
END

GO


CREATE PROCEDURE APZ_D_HistoryVisit
	@fromDate nvarchar(100)=null,
	@toDate nvarchar(100)=null,
	@userSign int=-1,
	@pageIndex int=1
AS
BEGIN
	declare @lowLimit int
	set @lowLimit = 10 * @pageIndex - 10
	declare @upLimit int
	set @upLimit = @pageIndex * 10


	select * from (select ROW_NUMBER() OVER (ORDER BY A.FinishedDate desc) RowNum,A.*,
	isnull(B.LinkThumbnails,isnull(B.LinkDownload,(select top 1 DefaultAvatar from APZ_D_OAPC  where AppName='i-DMS'))) as LinkThumbnails,
	isnull(B.LinkDownload,(select top 1 DefaultAvatar from APZ_D_OAPC  where AppName='i-DMS')) as LinkDownload ,
	ISNULL((CONVERT(int,(select count(*) from ORDR C where a.StoreCode=C.CardCode and C.U_GUID = A.Guid)) +
	CONVERT(int,(select count(*) from OINV D where a.StoreCode=D.CardCode and U_GUID = D.U_GUID)) ),0)
	as OrderNumber,
	CONCAT(ISNULL(CONVERT(nvarchar(50),E.firstName),''),' ',ISNULL(CONVERT(nvarchar(50),E.middleName),''),' ',ISNULL(CONVERT(nvarchar(50),E.lastName),'')) as U_fullName,
	case when ISNULL((CONVERT(int,(select count(*) from ORDR C where a.StoreCode=C.CardCode and C.U_GUID = A.Guid)) + 
	CONVERT(int,(select count(*) from OINV D where a.StoreCode=D.CardCode and U_GUID = D.U_GUID)) ),0)>0 then 'Y' else 'N' end as HaveOrder
from APZ_D_VISIT A 
	left join APZ_D_UPIM B on A.StoreCode=B.CardCode and B.IsAvatar='Y'
	inner join OHEM E on A.Usersign = E.userId
	where
	((isnull(@fromDate,'') <> '' and @fromDate = @toDate and cast(A.FinishedDate as date)=@fromDate)
	or ((isnull(@fromDate,'') = '' or cast(A.FinishedDate as date)>=@fromDate) 
	and (isnull(@toDate,'') = '' or cast(A.FinishedDate as date)<=@toDate)))
	and ((@userSign is null or @userSign=-1) or A.UserSign=@userSign)
	and ((not exists (select 1 from #tbl)) or A.StoreCode in (select CardCode from #tbl) )) B
	where (B.RowNum > @lowLimit AND B.RowNum <=@upLimit)
END