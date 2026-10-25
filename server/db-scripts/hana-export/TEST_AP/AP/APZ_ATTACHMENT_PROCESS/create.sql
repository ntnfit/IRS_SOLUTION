
create procedure APZ_Attachment_Process(in absEntry int)
as
begin
	create local temporary table #dtFile as (select top 0 * from ATC1);
	execute immediate 'drop table #dtFile';
	execute immediate 'create local temporary table #dtFile as (select * from "#dtFile");';
	
	delete from ATC1 where "AbsEntry" = :absEntry;
	update #dtFile set "AbsEntry" = :absEntry;
	insert into ATC1 select * from #dtFile;
end



