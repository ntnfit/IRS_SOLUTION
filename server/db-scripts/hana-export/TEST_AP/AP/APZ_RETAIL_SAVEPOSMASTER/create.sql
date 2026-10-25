
CREATE PROCEDURE apz_retail_SavePosMaster
AS
BEGIN
	create local temporary table #temp_RPOS as (select top 0* from APZ_RPOS);
	
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'insert into #temp_RPOS select * from #APZ_RPOS';
	UPSERT APZ_RPOS 
SELECT Case when ifnull(S."PosID",0) = 0 then (SELECT IFNULL(MAX("PosID"), 0) + 1 FROM APZ_RPOS)
else S."PosID" end,
 S."PosCode", S."Store", S."Dscription", S."UserSign", S."UpdateDate", S."CreateDate", S."UserSign2", S."Inactive" FROM #temp_RPOS S;

END




