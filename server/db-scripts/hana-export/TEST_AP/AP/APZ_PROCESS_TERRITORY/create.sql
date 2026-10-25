
create procedure APZ_Process_Territory
(
in action char(1)
)
as
begin
create local temporary table #APZ_OTERLocal as (select top 0* from OTER);
	
	-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
execute immediate 'insert into #APZ_OTERLocal select * from "#APZ_OTER"';
	
delete from  OTER;

if action='U' -- Update
then

	insert into OTER
	select * from #APZ_OTERLocal;
end if;
end;




