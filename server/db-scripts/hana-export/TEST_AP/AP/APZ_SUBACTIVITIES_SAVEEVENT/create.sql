

CREATE PROCEDURE APZ_SubActivities_SaveEvent(in typeSave int default 1,in checkDateChange char(1) default 'N', in checkTimeChange char(1) default 'N', in newOriRecurrenceInfo nvarchar(40000) default '')
AS
BEGIN

	declare checkCount int = 0;
	DECLARE ClgCode int = 0;
	declare AbsEntry int = 0;
	declare UpdateClgCode int = 0;
	declare SeriesNum int = 0;
	declare RecurPat char(1);
	DECLARE ColsUpdate NVARCHAR(40000);
	declare queryUpdateRecur nvarchar(40000);
	
	create local temporary table #APZ_EventData as (select top 0 * from APZ_OCLG);
	exec 'insert into #APZ_EventData select * from "#APZ_Event";';
	
	SELECT IFNULL((SELECT TOP 1 "ClgCode" from #APZ_EventData ORDER BY "ClgCode" DESC),0) into ClgCode FROM DUMMY;
	/* Attachment Files */
	SELECT IFnull((SELECT ifnull("AtcEntry",0) FROM APZ_OCLG  WHERE  "ClgCode"= :ClgCode),0) into AbsEntry from DUMMY;
		
	select IFNULL((select top 1 1 from M_TEMPORARY_TABLES where SCHEMA_NAME = current_schema and Table_Name = N'#APZ_Attachment'), 0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
		call APZ_SetAttachment(AbsEntry);
	ELSE
		/* No file in documents delete all file entry of this document */
			DELETE FROM ATC1 WHERE "AbsEntry"=:AbsEntry;
	END IF;
			 
	select ifnull((Select top 1 "ClgCode" from APZ_OCLG  order by "ClgCode" DESC),0) +1 into ClgCode from DUMMY;

	select IFnull((Select top 1 "SeriesNum" from #APZ_EventData  order by "ClgCode" DESC),0) into SeriesNum from DUMMY;

	-- set new relaID code to temp table
	UPDATE #APZ_EventData SET "ClgCode" = :ClgCode where ifnull("ClgCode",0) =0;
	select ifnull((Select top 1 "ClgCode" from #APZ_EventData  order by "ClgCode" DESC),0) into UpdateClgCode FROM DUMMY;

	SELECT IFnull((Select top 1 "RecurPat" from #APZ_EventData  order by "ClgCode" DESC),'N') into RecurPat FROM DUMMY;

	--Nếu là đang sửa 1 Occrurence thành 1 Exception thì sẽ lấy thông tin REcurrence ở sự kiện gốc để lưu
	if :typeSave != 1 AND :SeriesNum > 0
	THEN
		UPDATE #APZ_EventData SET "RecurPat" = S."RecurPat", "SeStartDat" = S."SeStartDat", "SeEndDat" = S."SeEndDat", "MaxOccur" = S."MaxOccur", "Interval" = S."Interval", "Sunday" = S."Sunday", "Monday" = S."Monday"
								, "Tuesday" = S."Tuesday", "Wednesday" = S."Wednesday", "Thursday" = S."Thursday", "Friday" = S."Friday", "Saturday" = S."Saturday", "SubOption" = S."SubOption", "DayInMonth" = S."DayInMonth"
								, "Month" = S."Month", "DayOfWeek" = S."DayOfWeek", "Week" = S."Week" FROM (SELECT TOP 1 * FROM APZ_OCLG WHERE "ClgCode" = :SeriesNum) AS S, #APZ_EventData;
	END IF;

	DELETE FROM APZ_OCLG where "ClgCode" = :UpdateClgCode;
	INSERT into APZ_OCLG select top 1 * from #APZ_EventData;

	-- Save All Event
	if :typeSave = 1
	THEN
		--Sửa những thông tin chung cho tất cả event trong chuỗi recurring		
		SELECT string_agg('APZ_OCLG."' || COLUMN_NAME || '" = S."' || COLUMN_NAME || '"', ',') into ColsUpdate from TABLE_COLUMNS
	                 where SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'APZ_OCLG' AND COLUMN_NAME not in ('ClgCode','CntctDate','CntctTime','Recontact','Attachment','CntctType','BeginTime','ENDTime','endDate'
	                 				,'SeriesNum','OrigDate','IsRemoved','AssignedBy','RecurrenceInfo');
		--UPDATE UDF Fields
		queryUpdateRecur = 'UPDATE APZ_OCLG	Set ' 
			|| :ColsUpdate || ' from  (SELECT TOP 1 * FROM #APZ_EventData) S, APZ_OCLG WHERE (' || cast(:SeriesNum as nvarchar) || ' = 0 AND APZ_OCLG."SeriesNum" = '|| cast(:UpdateClgCode as nvarchar) || ') 
			or (' || cast(:SeriesNum as nvarchar) || ' != 0 AND (APZ_OCLG."SeriesNum" = ' || cast(:SeriesNum as nvarchar) || ' and APZ_OCLG."ClgCode" = ' || cast(:SeriesNum as nvarchar) || '))';
		
		EXEC :queryUpdateRecur;
	
		-- Sửa những thông tin liên quan đến ngày giờ và ReccurenceInfo tương ứng
		--Trường hợp bỏ recurring thì sẽ đổi tất cả các ChangedOccurence thành các sự kiện riêng biệt và bỏ Recurring ở cả sự kiện gốc
		if :RecurPat = 'N'
		THEN
		
			UPDATE APZ_OCLG SET "SeriesNum" = null, "OrigDate" = null, "RecurrenceInfo" = null, "RecurPat" = 'N',"CntctType" =0 Where "SeriesNum" = :UpdateClgCode and "IsRemoved" = 'N';
			DELETE FROM APZ_OCLG where "IsRemoved" = 'Y' and "SeriesNum" = :UpdateClgCode;

		else
			--Trường hợp vẫn là recurring
			if :RecurPat != 'N'
			THEN
				--Khi sự kiện đang sửa là 1 exception
				if :SeriesNum != 0 
				THEN
					--Nếu có sửa ngày mà ko sửa giờ của exception thì sẽ giống như trường hợp bỏ reccurence cho sự kiện gốc đó nhưng vẫn giữ Reccurence cho sự kiện gốc, chỉ update thông tin RecurrenceInfo
					if :checkDateChange = 'Y' AND :checkTimeChange = 'N'
					THEN
						DELETE FROM APZ_OCLG where "IsRemoved" = 'Y' and "SeriesNum" = :UpdateClgCode;
						UPDATE APZ_OCLG	Set "Recontact" = S."Recontact","endDate" = S."endDate" from  (SELECT TOP 1 * FROM #APZ_EventData) S, APZ_OCLG where APZ_OCLG."SeriesNum" = :SeriesNum;
	
						UPDATE APZ_OCLG	 Set "Recontact" = S."Recontact","endDate" = S."endDate","RecurrenceInfo" = :newOriRecurrenceInfo from  (SELECT TOP 1 * FROM #APZ_EventData) S, APZ_OCLG where APZ_OCLG."ClgCode" = :SeriesNum;
					
					--Nếu có sửa cả ngày và giờ của exception thì sẽ tách luôn exception đó thành 1 sự kiện riêng biệt
					elseif :checkDateChange ='Y' AND :checkTimeChange = 'Y'
					THEN
						DELETE FROM APZ_OCLG where "IsRemoved" = 'Y' and "SeriesNum" = :UpdateClgCode;
						UPDATE APZ_OCLG	Set "BeginTime" = S."BeginTime","ENDTime" = S."ENDTime","SeriesNum" = null, "OrigDate" = null, "RecurrenceInfo" = null, "RecurPat" = 'N',"CntctType" =0
								from  (SELECT TOP 1 * FROM #APZ_EventData) S, APZ_OCLG where APZ_OCLG."SeriesNum" = :SeriesNum;
							--Nếu chỉ sửa giờ thì sẽ sửa giờ cho tất cả Exception và update thông tin RecurrenceInfo cho sự kiện gốc
					elseif :checkTimeChange = 'Y'
					THEN
								UPDATE APZ_OCLG Set "BeginTime" = S."BeginTime","ENDTime" = S."ENDTime" from  (SELECT TOP 1 * FROM #APZ_EventData) S, APZ_OCLG Where APZ_OCLG."SeriesNum" = :SeriesNum;
								UPDATE APZ_OCLG Set "BeginTime" = S."BeginTime","ENDTime" = S."ENDTime","RecurrenceInfo" = :newOriRecurrenceInfo from (SELECT TOP 1 * FROM #APZ_EventData) S, APZ_OCLG where APZ_OCLG."ClgCode" = :SeriesNum;
					END IF;

				else
					--Sửa sự kiện Recurring gốc (sửa ở trên form Event Management)
					--Nếu có sửa ngày của sự kiện gốc thì sẽ giống như trường hợp bỏ reccurence cho sự kiện gốc đó nhưng vẫn giữ Reccurence cho sự kiện gốc, chỉ update thông tin RecurrenceInfo
					if :checkDateChange  = 'Y'
					THEN
							DELETE FROM APZ_OCLG where "IsRemoved" = 'Y' and "SeriesNum" = :UpdateClgCode;
							UPDATE APZ_OCLG Set "SeriesNum" = null, "OrigDate" = null, "RecurrenceInfo" = null, "RecurPat" = 'N',"CntctType" =0 Where "SeriesNum" = :UpdateClgCode;
							UPDATE APZ_OCLG Set "Recontact" = S."Recontact","BeginTime" = S."BeginTime","ENDTime" = S."ENDTime","endDate" = S."endDate","RecurrenceInfo" = :newOriRecurrenceInfo 
									from  (SELECT TOP 1 * FROM #APZ_EventData) S, APZ_OCLG where APZ_OCLG."ClgCode" = :UpdateClgCode;
					-- Nếu chỉ sửa giờ thì sẽ update lại giờ cho tất cả sự kiện và lưu lại thông tin RecurrenceInfo cho sự kiện gốc đó (lưu lại thông tin giờ cho RecurrenceInfo)
					elseif :checkTimeChange = 'Y'
					THEN
							UPDATE APZ_OCLG Set "BeginTime" = S."BeginTime","ENDTime" = S."ENDTime" from  (SELECT TOP 1 * FROM #APZ_EventData) S, APZ_OCLG Where APZ_OCLG."SeriesNum" = :UpdateClgCode;
							UPDATE APZ_OCLG Set "BeginTime" = S."BeginTime","ENDTime" = S."ENDTime","RecurrenceInfo" = :newOriRecurrenceInfo from (SELECT TOP 1 * FROM #APZ_EventData) S, APZ_OCLG where APZ_OCLG."ClgCode" = :UpdateClgCode;
					END IF;
				END IF;
			END IF;
		END IF;
	END IF;
	select cast((select TOP 1 "ClgCode" from #APZ_EventData) as nvarchar) || ',' || ifnull(cast(:AbsEntry as nvarchar),'') from DUMMY;
END



