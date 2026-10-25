
CREATE PROCEDURE apz_retail_SaveClock
AS
BEGIN	
	
	create local temporary table #temp_CLOCK as (select top 0* from APZ_CLOCK);
	
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'insert into #temp_CLOCK select * from #APZ_CLOCK';
	UPSERT APZ_CLOCK 
SELECT Case when ifnull(S."LineId",0) = 0 then (SELECT IFNULL(MAX("LineId"), 0) + 1 FROM APZ_CLOCK)
else S."LineId" end,
 S."StoreCode", S."PosID", S."UserSign", S."ClockDate", S."ClockTime", S."ClockType", S."CashID" FROM #temp_CLOCK S;
END





