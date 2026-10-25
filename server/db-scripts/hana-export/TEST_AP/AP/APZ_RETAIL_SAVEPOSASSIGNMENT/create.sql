
CREATE PROCEDURE apz_retail_SavePosAssignment
(UserCode nvarchar(20))

AS
BEGIN
create local temporary table #temp_POS1 as (select top 0* from APZ_POS1);
	
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'insert into #temp_POS1 select * from #APZ_POS1';
	delete from APZ_POS1 where "UserCode" = :UserCode;
	insert into APZ_POS1 select * from #temp_POS1;
END




