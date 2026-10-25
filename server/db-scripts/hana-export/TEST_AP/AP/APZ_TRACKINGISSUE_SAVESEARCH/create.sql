
create procedure APZ_TrackingIssue_SaveSearch
(
in userId int default 0,
in formId nvarchar(50) default ''
)
as
begin

	create local temporary table #APZ_TGIE2Data as (select top 0 * from APZ_OSPH);
		
		-- ở đây là lệnh đổ dữ liệu từ table bulkcopy sang table tep vừa tạo chú ý table temp từ bulk copy có dấu nháy "" vì phân biệt hoa thường
	execute immediate 'insert into #APZ_TGIE2Data select * from "#APZ_TGIE2"';
	
	delete from  APZ_OSPH where "UserCode"=:userId and "FormId"= :formId;
	insert into APZ_OSPH select * from #APZ_TGIE2Data;
	select 0,'' from dummy;
end;






