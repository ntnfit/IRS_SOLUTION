using Apzon.Services.Repository.Inventory.PickAndPack;

namespace Apzon.Services.HanaRepository.Inventory.PickAndPack;

public class HanaPickingRepository : PickingRepository
{
	public HanaPickingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetNextIdQuery()
	{
		return "SELECT IFNULL((SELECT Max(\"AbsEntry\") FROM APZ_OPKL),0)+1 from DUMMY;";
	}

	protected override string GetAddNewQuery()
	{
		return string.Format("create local temporary table #tempDoc as (select top 0 * from {0});\r\n                                            create local temporary table #tempLine as (select top 0 * from {3});\r\n                                            create local temporary table #tempBatch as (select top 0 * from {4});\r\n                                            exec 'drop table #tempDoc;';\r\n                                            exec 'drop table #tempLine;';\r\n                                            exec 'drop table #tempBatch;';\r\n                                            exec 'create local temporary table #tempDoc as (select * from \"{1}\");';\r\n                                            exec 'create local temporary table #tempLine as (select * from \"{2}\");';\r\n                                            exec 'create local temporary table #tempBatch as (select * from \"{5}\");';\r\n                                            update #tempDoc set \"ObjType\"=@ObjCode;\r\n                                            insert into {0} select * from #tempDoc;\r\n                                            update #tempLine set \"AbsEntry\"=@DocEntry;\r\n                                            update #tempBatch set \"AbsEntry\"=@DocEntry;\r\n                                            insert into {3} select * from #tempLine where #tempLine.\"ItemCode\" is not null and #tempLine.\"ItemCode\" <>'';\r\n                                            insert into {4} select * from #tempBatch where #tempBatch.\"MdAbsEntry\" <>0;", "APZ_OPKL", "#APZ_OPKL", "#APZ_PKL1", "APZ_PKL1", "APZ_PKL3", "#APZ_PKL3");
	}

	protected override string GetUpdateQuery()
	{
		return string.Format("create local temporary table #tempDoc as (select top 0 * from {0});\r\n                                            create local temporary table #tempLine as (select top 0 * from {3});\r\n                                            create local temporary table #tempBatch as (select top 0 * from {4});\r\n                                            exec 'drop table #tempDoc;';\r\n                                            exec 'drop table #tempLine;';\r\n                                            exec 'drop table #tempBatch;';\r\n                                            exec 'create local temporary table #tempDoc as (select * from \"{1}\");';\r\n                                            exec 'create local temporary table #tempLine as (select * from \"{2}\");';\r\n                                            exec 'create local temporary table #tempBatch as (select * from \"{5}\");';\r\n                                            delete from {0} where \"AbsEntry\"=@DocEntry;\r\n                                            delete from {3} where \"AbsEntry\"=@DocEntry;\r\n                                            delete from  {4} where \"AbsEntry\"=@DocEntry;\r\n                                            \r\n                                            update #tempDoc set \"ObjType\"=@ObjCode;\r\n                                            insert into {0} select * from #tempDoc;\r\n                                            update #tempLine set \"AbsEntry\"=@DocEntry;\r\n                                            update #tempBatch set \"AbsEntry\"=@DocEntry;\r\n                                             \r\n                                            insert into {3} select * from #tempLine where #tempLine.\"ItemCode\" is not null and #tempLine.\"ItemCode\" <>'';\r\n                                            insert into {4} select * from #tempBatch where #tempBatch.\"MdAbsEntry\" <>0;\r\n                                           \r\n", "APZ_OPKL", "#APZ_OPKL", "#APZ_PKL1", "APZ_PKL1", "APZ_PKL3", "#APZ_PKL3");
	}
}
