
IF EXISTS (SELECT * FROM dbo.sysobjects WHERE type='P' and id = OBJECT_ID('APZ_TimeSheeting_Save'))
BEGIN
    DROP PROCEDURE APZ_TimeSheeting_Save
END

GO


CREATE PROC [dbo].[APZ_TimeSheeting_Save]
AS
BEGIN
	
	DECLARE @Period VARCHAR(max)=ISNULL((SELECT TOP 1 Period FROM #dtAPZ_OTSL),'')

	DECLARE @Branch INT =ISNULL((SELECT TOP 1 Branch FROM #dtAPZ_OTSL),0)
	DECLARE @BaseEntry INT =ISNULL((SELECT TOP 1 BaseEntry FROM #dtAPZ_OTSL where isnull(BaseEntry,0) != 0),0)
	DECLARE @OldEntry INT = isnull((select top 1 BaseEntry from APZ_OTSL where BaseEntry = @BaseEntry and @BaseEntry>0),0)
	DECLARE @IsImport CHAR(1) =CASE WHEN @OldEntry = 0 and @BaseEntry !=0 then 'Y' else 'N' end
	DECLARE @Usersign2 int = (select Top 1 UserSign2 from #dtAPZ_OTSL)

	--Update BaseEntry all lines
	IF(@BaseEntry != 0)
	BEGIN
		UPDATE #dtAPZ_OTSL set BaseEntry = @BaseEntry
	END
	DECLARE @err_message nvarchar(max)=''
	IF((SELECT PeriodStatus FROM APZ_OHWP Where PeriodCode =@Period and Branch = @Branch) !='U')
	BEGIN
		set @err_message='The selected Period has been closed.';
		--RAISERROR (@err_message,10,1)
		--RETURN;
		select 1600008 error_code, @err_message "error_message";
		return;
	END
	--If import is override then alert error
	IF(@OldEntry != 0 and @OldEntry != @BaseEntry)
	BEGIN
		set @err_message='Current Timesheet is based on other In/Out AbsEntry. Please cancel this TimeSheet and re-import!';
		select 1600008 error_code, @err_message "error_message";
		return;
	END
	DECLARE @maxLineNum int = ISNULL((SELECT MAX(LineNum) FROM APZ_TSL1 WHERE Period=@Period),0)
	
	--create a temp table for list of empid 
	DECLARE @tbl TABLE
	(
		EmpId int,
		WorkDay DateTime
	)
	INSERT INTO @tbl (EmpId,WorkDay)
	SELECT DISTINCT a.EmpId, a.WorkDay FROM #dtAPZ_OTSL a
	DECLARE @EmpList int = @@ROWCOUNT
	
	DELETE dbo.APZ_OTSL WHERE Period = @Period AND EmpId IN (SELECT DISTINCT b.EmpId FROM @tbl b)
	DECLARE @RowDeletedOTSL int = @@ROWCOUNT

	DECLARE @sql nvarchar(max)
	DECLARE @Udfcols AS NVARCHAR(MAX)
	Select @Udfcols = (SELECT ',' + COLUMN_NAME
                    From INFORMATION_SCHEMA.COLUMNS
					Where TABLE_NAME = 'APZ_OTSL' AND COLUMN_NAME like 'U[_]%'
					Group By COLUMN_NAME
            FOR XML PATH(''))

	set @sql = '
		INSERT INTO dbo.APZ_OTSL
		        ( Period ,
		          EmpId ,
				  EmpName,
		          TotalBefore ,
				  WorkSum,
				  AbsentSum,
				  LeaveSum,
		          WorkQuota ,
		          DocStatus ,
		          Extra ,
		          WorkTotal ,
				  LeaveTotal,
		          Comments,
				  UserSign2,
				  UpdateDate,
				  BaseEntry,
				  LateSum,
				  EarlySum
				  '+isnull(@Udfcols,'')+'
		        )
		SELECT DISTINCT '''+isnull(@Period,'')+''',EmpId,EmpName,TotalBefore,WorkSum,AbsentSum,LeaveSum,WorkQuota,DocStatus,Extra,WorkTotal,LeaveTotal,Comments,UserSign2,Convert(Date,GetDate()),BaseEntry,LateSum,EarlySum'+isnull(@Udfcols,'')+'
		FROM #dtAPZ_OTSL  WHERE WorkCode<>'''' OR Extra<>0 or Comments<>''''    '
	PRINT @sql
	EXECUTE (@sql)
	DECLARE @RowInsertedOTSL int =@@ROWCOUNT

	DELETE a From dbo.APZ_TSL1 a INNER JOIN @tbl b ON ISNULL(a.EmpId,0) = ISNULL(b.EmpId,0) And ISNULL(a.WorkDay, 'N/A') =  ISNULL(b.WorkDay, 'N/A') WHERE Period=@Period;

	DECLARE @RowDeletedTSL1 int =@@ROWCOUNT

	INSERT INTO dbo.APZ_TSL1 (Period, EmpId, LineNum, WorkDay, WorkCode, WorkTotal)
	SELECT @Period,a.EmpId,ROW_NUMBER() OVER(PARTITION BY a.EmpId ORDER BY a.EmpId ASC) + ISNULL(d.MaxLineNum,0),a.WorkDay,b.WorkCode,c.Rate 
	FROM #dtAPZ_OTSL a 
	 outer apply (
		select * from dbo.Split(a.WorkCode,',')
	 ) b(ID,WorkCode) 
	 LEFT JOIN APZ_OHWC c on c.Branch = @Branch and b.WorkCode=c.Code
	 LEFT JOIN (Select Max(LineNum) AS MaxLineNum, EmpId From APZ_TSL1 Where Period=@Period Group By EmpId) d ON a.EmpId = d.EmpId
	WHERE b.WorkCode<>''
	/*Cập nhật lại thông tin tổng hợp giờ làm, nghỉ phép*/
	--Tông hợp dữ liệu từ bảng VÀO - RA
	;WITH q AS
		(
			SELECT  a.*, ROW_NUMBER() OVER (PARTITION BY empID,WorkDay ORDER BY empID) rn,SUM(isnull(WorkTotal,0)) OVER (PARTITION BY empID,WorkDay ORDER BY empID) SumInDay,b.CalcType
			FROM APZ_IOD1 a join APZ_OWSC b on a.SchID = b.SchID where Period = @Period
		)
	SELECT  * into #tblInOut
	FROM q
	WHERE (rn = 1 and CalcType = '2') or CalcType = '1'
	
	select a.* into #tblWorkCode from APZ_OHWC a join APZ_OHWT b on a.WorkType = b.Id and b.Typ = 'W' and a.Branch = @Branch
	--Trường hợp không tính công theo ca
	update #tblInOut  set WorkCode = isnull((Select TOP 1 Code From #tblWorkCode 
									where Rate =ROUND(#tblInOut.SumInDay,4) or Rate =ROUND(#tblInOut.SumInDay,3) or Rate =ROUND(#tblInOut.SumInDay,2) 
										or Rate =ROUND(#tblInOut.SumInDay,1) or Rate <#tblInOut.SumInDay Order By Rate DESC),'') 
	where CalcType = '2'
	--Lấy thông tin tổng hợp,tổng đi muộn, tổng về sớm	
	;WITH q AS
		(
			SELECT  a.*, ROW_NUMBER() OVER (PARTITION BY empID ORDER BY empID) rn2
				,SUM(isnull(Late,0)) OVER (PARTITION BY empID ORDER BY empID) SumLate,SUM(isnull(Early,0)) OVER (PARTITION BY empID ORDER BY empID) SumEarly
			FROM    #tblInOut a LEFT JOIN APZ_OHWC c on c.Branch = @Branch and a.WorkCode=c.Code where Period = @Period and AbsEntry = @BaseEntry
		)
		SELECT  * into #tblLateEarly
		FROM q
		WHERE rn2 = 1

	--Tổng hợp mã đi làm
	;WITH q AS
		(
			SELECT  a.empID,ROW_NUMBER() OVER (PARTITION BY empID ORDER BY empID) rn2,SUM(isnull(c.Rate,0)) OVER (PARTITION BY empID ORDER BY empID) SumTotal
			FROM    #tblInOut a LEFT JOIN APZ_OHWC c on c.Branch = @Branch and a.WorkCode=c.Code LEFT JOIN APZ_OHWT d on c.WorkType = d.Id where Period = @Period and AbsEntry = @BaseEntry  and d.Typ = 'W' 
		)
	SELECT * into #tblWorkSum FROM q WHERE   rn2 = 1
	
	--Tổng hợp mã nghỉ
	;WITH q AS
		(
			SELECT  a.empID,ROW_NUMBER() OVER (PARTITION BY empID ORDER BY empID) rn2,SUM(isnull(c.Rate,0)) OVER (PARTITION BY empID ORDER BY empID) SumTotal
			FROM    #tblInOut a LEFT JOIN APZ_OHWC c on c.Branch = @Branch and a.WorkCode=c.Code LEFT JOIN APZ_OHWT d on c.WorkType = d.Id  where Period = @Period and AbsEntry = @BaseEntry and d.Typ = 'A'
		)
	SELECT  * into #tblAbsentSum FROM q WHERE rn2 = 1

	--Tổng hợp mã phép
	;WITH q AS
		(
			SELECT a.empID,ROW_NUMBER() OVER (PARTITION BY empID ORDER BY empID) rn2,SUM(isnull(c.Rate,0)) OVER (PARTITION BY empID ORDER BY empID) SumTotal
			FROM #tblInOut a LEFT JOIN APZ_OHWC c on c.Branch = @Branch and a.WorkCode=c.Code LEFT JOIN APZ_OHWT d on c.WorkType = d.Id where Period = @Period and AbsEntry = @BaseEntry and d.Typ = 'L'
		)
	SELECT * into #tblLeaveSum FROM q WHERE rn2 = 1

	--Tổng hợp tất cả các phần ở trên để cho vào bảng #tblSUM
	SELECT a.*,b.SumTotal AS WorkSum,c.SumTotal AS AbsentSum,d.SumTotal AS LeaveSum 
	INTO #tblSUM 
	FROM #tblLateEarly a left join #tblWorkSum b ON a.empID = b.empID 
				left join #tblAbsentSum c ON a.empID = c.empID 
				left join #tblLeaveSum d ON a.empID = d.empID

	Update a set a.TotalBefore = b.WorkSum,a.WorkSum = b.WorkSum,a.WorkTotal = b.WorkSum,a.AbsentSum = b.AbsentSum,a.LeaveSum = b.LeaveSum,a.LateSum = b.SumLate,a.EarlySum = b.Early 
	from APZ_OTSL a left join #tblSUM b on a.empID = b.empID And a.Period = @Period;

	/*End Update*/
	DECLARE @RowInsertedTSL1 int =@@ROWCOUNT

	DELETE FROM APZ_TSL2 WHERE Period=@Period AND EmpId IN (SELECT b.EmpId FROM @tbl b)

    INSERT INTO APZ_TSL2(Period,EmpId,EmpName,LeaveTotal,Used,Remain,UserSign2,UpdateDate)
	SELECT DISTINCT Period,EmpId,EmpName,ISNULL(LeaveTotal,0),ISNULL(LeaveSum,0),ISNULL(Remain,0),UserSign2,UpdateDate FROM #dtAPZ_OTSL WHERE WorkCode<>''

	DELETE FROM APZ_TSL3 WHERE  Period=@Period AND EmpId IN (SELECT b.EmpId FROM @tbl b)

    INSERT INTO APZ_TSL3(Period,EmpId,EmpName,CompLeaveTotal,Used,Remain)
	SELECT DISTINCT Period,EmpId,EmpName,ISNULL(TotalOT,0),ISNULL(UsedOT,0),ISNULL(RemainOT,0) FROM #dtAPZ_OTSL WHERE WorkCode<>''

	IF(@IsImport = 'Y')
	BEGIN
		IF NOT Exists(Select 1 From APZ_IOD2 Where AbsEntry = @BaseEntry) 
		BEGIN
			--Case Data OT is empty
			UPDATE APZ_OIOD set status = 'C' where AbsEntry = @BaseEntry
		END
		ELSE
		BEGIN
			IF Exists(Select 1 From APZ_OTSH Where BaseEntry = @BaseEntry)
			BEGIN
				--Case Data had been saved in data OT
				--Check Data had been saved in data OT (Multiples OT Type)
				IF NOT Exists(Select Distinct(x.OvertimeType)
						From APZ_IOD2 x LEFT JOIN APZ_OTSH y on x.Period = y.Period And x.OvertimeType = y.OvertimeType And x.AbsEntry = y.BaseEntry
						Where x.AbsEntry = @BaseEntry AND y.OvertimeType IS NULL) 
				BEGIN
					UPDATE APZ_OIOD set status = 'C' where AbsEntry = @BaseEntry
				END
			END
		END
	END

	SELECT @Period Period, @maxLineNum LineNum, @EmpList EmpList, @RowDeletedOTSL RowDeletedOTSL, @RowDeletedTSL1 RowDeletedTSL1, @RowInsertedOTSL RowInsertedOTSL, @RowInsertedTSL1 RowInsertedTSL1

end