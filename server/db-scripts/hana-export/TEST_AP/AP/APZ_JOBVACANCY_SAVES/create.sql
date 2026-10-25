create procedure APZ_JobVacancy_Saves(in type char(1))
as
begin
	Declare docId int =0;
	declare docNum int =0;
	declare lineNum int = 0;
	declare checkCount int = 0;
	declare sqlString nvarchar(400000) = '';
	DECLARE Udfcols NVARCHAR(40000) = '';
	
	create local temporary table #APZ_OHJVDATA as (select top 0 * from APZ_OHJV);
	
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	exec 'DROP TABLE #APZ_OHJVDATA;';
	exec 'create local temporary table #APZ_OHJVDATA as (select * from "#APZ_OHJV");';
	
	create local temporary table #APZ_HJV1Data as (select top 0* from APZ_HJV1);
	exec 'DROP TABLE #APZ_HJV1Data;';
	exec 'create local temporary table #APZ_HJV1DATa as (select * from "#APZ_HJV1");';
	
	create local temporary table #APZ_HJV2DATa as (select top 0* from APZ_HJV2);
	exec 'DROP TABLE #APZ_HJV2DATa;';
	exec 'create local temporary table #APZ_HJV2DATa as (select * from "#APZ_HJV2");';
	
	create local temporary table #APZ_HJV3DATa as (select top 0* from APZ_HJV3);
	exec 'DROP TABLE #APZ_HJV3DATa;';
	exec 'create local temporary table #APZ_HJV3DATa as (select * from "#APZ_HJV3");';
	
	create local temporary table #APZ_HJV4DATa as (select top 0* from APZ_HJV4);
	exec 'DROP TABLE #APZ_HJV4DATa;';
	exec 'create local temporary table #APZ_HJV4DATa as (select * from "#APZ_HJV4");';
	
	create local temporary table #APZ_HJV5DATa as (select top 0* from APZ_HJV5);
	exec 'DROP TABLE #APZ_HJV5DATa;';
	exec 'create local temporary table #APZ_HJV5DATa as (select * from "#APZ_HJV5");';
	
	create local temporary table #APZ_HJV6DATa as (select top 0* from APZ_HJV6);
	exec 'DROP TABLE #APZ_HJV6DATa;';
	exec 'create local temporary table #APZ_HJV6DATa as (select * from "#APZ_HJV6");';
	
	create local temporary table #APZ_HJV7DATa as (select top 0* from APZ_HJV7);
	exec 'DROP TABLE #APZ_HJV7DATa;';
	exec 'create local temporary table #APZ_HJV7DATa as (select * from "#APZ_HJV7");';
	
	create local temporary table #APZ_HJV8DATa as (select top 0* from APZ_HJV8);
	exec 'DROP TABLE #APZ_HJV8DATa;';
	exec 'create local temporary table #APZ_HJV8DATa as (select * from "#APZ_HJV8");';

	if :type = 'U'
	then
		select	ifnull((select top 1 "DocEntry" from #APZ_OHJVDATa),0) into docId from dummy;
		delete from APZ_HJV1 where "DocEntry" = :docId;
		delete from APZ_HJV2 where "DocEntry" = :docId;
		delete from APZ_HJV3 where "DocEntry" = :docId;
		delete from APZ_HJV4 where "DocEntry" = :docId;
		delete from APZ_HJV5 where "DocEntry" = :docId;
		delete from APZ_HJV6 where "DocEntry" = :docId;
		delete from APZ_HJV7 where "DocEntry" = :docId;
		delete from APZ_HJV8 where "DocEntry" = :docId;
		delete from APZ_OHJV where "DocEntry" = :docId;
	
	else
		
		select	ifnull((select MAX("DocEntry") from APZ_OHJV), 0) + 1 into docId from dummy;
	
		update #APZ_OHJVDATa set "DocEntry" = :docId;
			
		select ifnull((select MAX("DocNum") from APZ_OHJV),0)+1 into docNum from dummy;
		update #APZ_OHJVDATa set "DocNum" = :docNum;
	end if;
	
	--Đánh số từ 1 vì trên form các điều kiện line num đều từ 1
	insert into APZ_OHJV select * from #APZ_OHJVDATa;
	UPDATE #APZ_HJV1DATa set "DocEntry" = :docId ,"LineNum"= -1;
	select ifnull((select top 1 1 from #APZ_HJV1DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	while :checkCount = 1
	DO
		lineNum = :lineNum + 1;
		update top 1 #APZ_HJV1DATa set "LineNum" = :lineNum where "LineNum" = -1;
		select ifnull((select top 1 1 from #APZ_HJV1DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	END WHILE;
		
	update #APZ_HJV2DATa set "DocEntry" = :docId,"LineNum"= -1;
	select ifnull((select top 1 1 from #APZ_HJV2DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	lineNum = 0;
	while :checkCount = 1
	DO
		lineNum = :lineNum + 1;
		update top 1 #APZ_HJV2DATa set "LineNum" = :lineNum where "LineNum" = -1;
		select ifnull((select top 1 1 from #APZ_HJV2DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	END WHILE;
		
	update #APZ_HJV3DATa set "DocEntry" = :docId,"LineNum"= -1;
	select ifnull((select top 1 1 from #APZ_HJV3DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	lineNum = 0;
	while :checkCount = 1
	DO
		lineNum = :lineNum + 1;
		update top 1 #APZ_HJV3DATa set "LineNum" = :lineNum where "LineNum" = -1;
		select ifnull((select top 1 1 from #APZ_HJV3DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	END WHILE;
						
	update #APZ_HJV4DATa set "DocEntry" = :docId,"LineNum"= -1;
	select ifnull((select top 1 1 from #APZ_HJV4DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	lineNum = 0;
	while :checkCount = 1
	DO
		lineNum = :lineNum + 1;
		update top 1 #APZ_HJV4DATa set "LineNum" = :lineNum where "LineNum" = -1;
		select ifnull((select top 1 1 from #APZ_HJV4DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	END WHILE;

	update #APZ_HJV5DATa set "DocEntry" = :docId,"LineNum"= -1;
	select ifnull((select top 1 1 from #APZ_HJV5DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	lineNum = 0;
	while :checkCount = 1
	DO
		lineNum = :lineNum + 1;
		update top 1 #APZ_HJV5DATa set "LineNum" = :lineNum where "LineNum" = -1;
		select ifnull((select top 1 1 from #APZ_HJV5DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	END WHILE;
				
	update #APZ_HJV6DATa set "DocEntry" = :docId,"LineNum"= -1;
	select ifnull((select top 1 1 from #APZ_HJV6DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	lineNum = 0;
	while :checkCount = 1
	DO
		lineNum = :lineNum + 1;
		update top 1 #APZ_HJV6DATa set "LineNum" = :lineNum where "LineNum" = -1;
		select ifnull((select top 1 1 from #APZ_HJV6DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	END WHILE;
	
	UPDATE #APZ_HJV7DATa set "DocEntry" = :docId,"LineNum"= -1;
	select ifnull((select top 1 1 from #APZ_HJV7DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	lineNum = 0;
	while :checkCount = 1
	DO
		lineNum = :lineNum + 1;
		update top 1 #APZ_HJV7DATa set "LineNum" = :lineNum where "LineNum" = -1;
		select ifnull((select top 1 1 from #APZ_HJV7DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	END WHILE;

	--declare @lineInterView int = isnull((ROW_NUMBER() Over(Order by DocEntry)),0)
	UPDATE #APZ_HJV8DATa set "DocEntry" = :docId,"LineNum"= -1;
	select ifnull((select top 1 1 from #APZ_HJV8DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	lineNum = 0;
	while :checkCount = 1
	DO
		lineNum = :lineNum + 1;
		update top 1 #APZ_HJV8DATa set "LineNum" = :lineNum where "LineNum" = -1;
		select ifnull((select top 1 1 from #APZ_HJV8DATa where "LineNum" = -1),0) into checkCount from DUMMY;
	END WHILE;
		

	insert into APZ_HJV7 select*from #APZ_HJV7DATa;
	insert into APZ_HJV1 select*from #APZ_HJV1DATa;
	insert into APZ_HJV2 select*from #APZ_HJV2DATa;
	insert into APZ_HJV3 select*from #APZ_HJV3DATa;
	insert into APZ_HJV4 select*from #APZ_HJV4DATa;
	
	select IFNULL((SELECT string_agg('"' || COLUMN_NAME || '"',',') from TABLE_COLUMNS where SCHEMA_NAME = CURRENT_SCHEMA AND TABLE_NAME = 'APZ_HJV5' AND COLUMN_NAME like 'U_%'),'') into Udfcols from DUMMY;
	if ifnull(:Udfcols, '') <> '' then
		Udfcols = ',' || :Udfcols;
	END IF;
	sqlString = 'insert into APZ_HJV5 ("DocEntry","LineNum","ApplicantNo","ApplicantName","OfferLetter","TrialPeriodSalary","TrialCur","ContractSalary","ContCur","OtherBenefits","Status","LogInstanc"' || ifnull(:Udfcols,'') || ') 
		select "DocEntry","LineNum","ApplicantNo","ApplicantName","OfferLetter","TrialPeriodSalary","TrialCur","ContractSalary","ContCur","OtherBenefits","Status","LogInstanc"' || ifnull(:Udfcols,'') || ' from #APZ_HJV5DATa;';
	EXEC :sqlString;
	
	insert into APZ_HJV6 select*from #APZ_HJV6DATa;
	insert into APZ_HJV8 select*from #APZ_HJV8DATa;
	
	
	update NNM1 set "NextNumber"=(select MAX("DocNum") from APZ_OHJV)+1 where "ObjectCode"='700012';

	select :docId, '' from dummy;
end;

