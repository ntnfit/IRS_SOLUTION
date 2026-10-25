
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE type='P' and id = OBJECT_ID('APZ_TimeSheeting_Export'))
BEGIN
    DROP PROCEDURE APZ_TimeSheeting_Export
END

GO

/*
[APZ_TimeSheeting_Export] 
		--@Dept='',
		@Emp=0,
		--@EmpName='',
		--@SStatus='',
		@Period ='2017-01',
		@Branch=-2
		
 */
CREATE PROC [dbo].[APZ_TimeSheeting_Export]
@Dept VARCHAR(254)='',
@Emp VARCHAR(254)='',
@EmpName NVARCHAR(254)='',
@SStatus varchar(254)='',
@Period VARCHAR(20),
@Branch INT
AS
BEGIN
DECLARE @err_message nvarchar(255)
SELECT a.empID,a.ExtEmpNo,a.firstName, a.lastName,a.dept,b.Name Department INTO #temp_NhanVien
  FROM OHEM a 
	INNER JOIN OUDP b ON a.dept=b.code
	WHERE 
	( a.branch=@Branch)
	AND ( a.dept IN (SELECT Data From dbo.Split(@Dept,',')) OR @Dept='')
	AND (a.empID =@Emp OR @Emp='' OR @Emp=0)
	AND (a.firstName LIKE '%'+@EmpName+'%' OR a.lastName LIKE '%'+@EmpName+'%' OR @EmpName='')
	AND (a.[status] IN (SELECT Data From dbo.Split(@SStatus,',')) OR @SStatus='')
	AND a.Active='Y'

	DECLARE @ot VARCHAR(MAX)=''
	SET @ot= (select '[' +code + '],' from APZ_OHOT where Branch=@Branch for xml path (''))
	SET @ot=LEFT(@ot,LEN(@ot)-1)

	DECLARE @StartDate DATETIME
	DECLARE @EndDate DATETIME
	SET @StartDate =ISNULL((SELECT a.FromDate FROM APZ_OHWP a WHERE a.PeriodCode=@Period AND (a.Branch=@Branch)),'1900-01-01')
	SET @EndDate =ISNULL((SELECT a.ToDate FROM APZ_OHWP a WHERE a.PeriodCode=@Period  AND (a.Branch=@Branch)),'1900-01-01')
	DECLARE @daterage VARCHAR(MAX)=''
	-- a string for pivot
	SET @daterage=(SELECT  DISTINCT '['+ CONVERT(NVARCHAR, DATEADD(DD,a.number,@StartDate),102)+'],' FROM master..spt_values a where a.type = 'P' and a.number between 0 and DATEDIFF(DD,@StartDate,@EndDate) FOR XML PATH(''))

	
	--if @daterange has value and ot has value
IF ISNULL(@daterage,'')<>'' AND @daterage <> '[1900.01.01],' 
--AND ISNULL(@ot,'')<>''
BEGIN
	
	--set @ot=''
	SELECT DISTINCT Convert(date, DATEADD(DD,a.number,@StartDate)) AS ddate INTO #Date FROM master..spt_values a WHERE a.type = 'P' and a.number between 0 and DATEDIFF(DD,@StartDate,@EndDate)

	select @Period Period,b.EmpId,b.ExtEmpNo,b.lastName +', '+b.firstName as EmpName,t0.ddate AS WorkDay,c.InTime,c.OutTime,'' FreeTxt,
	ISNULL((select d.WorkCode + ',' from dbo.APZ_TSL1 d where DATEDIFF(DD,d.WorkDay,c.WorkDay)=0 AND  d.EmpId=b.empID for xml path('')),'') AS WorkCode,
	(SELECT max(overtimetype) from APZ_TSH1 aa where t0.ddate=aa.workday and a.empid=aa.EmpId and a.linenum=aa.LineNum) OvertimeType ,
	a.overtimehour OvertimeHour into #dtTSH1 
	from #temp_NhanVien b  LEFT JOIN #Date T0 ON  t0.ddate IS NOT NULL 
	 LEFT JOIN APZ_TSH1 a ON a.EmpId=b.empID and a.Period=@Period and a.WorkDay=t0.ddate
	 LEFT JOIN APZ_TSL1 c  ON c.EmpId=b.empID and c.Period=@Period and c.WorkDay=t0.ddate


	--select * from #dtTSH1

	DECLARE @sql VARCHAR(MAX)=''
	--if have OT codes in OHOT, try to pivot columns
	if ISNULL(@ot,'')<>''
	BEGIN
	SET @sql='SELECT p.* FROM #dtTSH1   PIVOT	   (MAX(overtimehour) FOR overtimetype in ('+@ot+'))AS p ORDER BY empid'
	END
	--if there aren't any OT codes in OHOT, skip pivoting.
	ELSE
	BEGIN
	SET @sql='SELECT Period,EmpId,ExtEmpNo,EmpName,WorkDay,FreeTxt,WorkCode FROM #dtTSH1 AS p ORDER BY empid'
	END

	PRINT @sql
	EXECUTE(@sql)

	drop table #dtTSH1
	drop table #Date

END
	ELSE
	BEGIN
	
	SET @err_message='The selected Period has invalid start date or end date.'
	RAISERROR (@err_message,10,1)
	END
	DROP TABLE #temp_NhanVien
END
