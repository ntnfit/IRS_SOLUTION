using Apzon.Services.Repository.Widget;

namespace Apzon.Services.HanaRepository.Widget;

public class HanaWidgetRepository : WidgetRepository
{
	protected const string TempTableHeaderDataName = "#APZ_OWIDDATA";

	protected const string TempTableItemDataName = "#APZ_WID1DATA";

	protected const string TempTablePageDATAName = "#APZ_WID2DATA";

	public HanaWidgetRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string AddNewQuery()
	{
		return string.Format("create local temporary table {6} as (select top 0 * from {0});\r\n                                   exec 'insert into {6} select * from \"{1}\";';\r\n                                   create local temporary table {7} as (select top 0 * from {2});\r\n                                   exec 'insert into {7} select * from \"{3}\";';\r\n                                   create local temporary table {8} as (select top 0 * from {4});\r\n                                   exec 'insert into {8} select * from \"{5}\";';\r\n                                   insert into {0} select * from {6};\r\n                                   update {7} set \"AbsEntry\" = @DocEntry;\r\n                                   insert into {2} select * from {7};\r\n                                   update {8} set \"AbsEntry\" = @DocEntry;\r\n                                   insert into {4} select * from {8};", "APZ_OWID", "#APZ_OWID", "APZ_WID1", "#APZ_WID1", "APZ_WID2", "#APZ_WID2", "#APZ_OWIDDATA", "#APZ_WID1DATA", "#APZ_WID2DATA");
	}

	protected override string UpdateDataQuery()
	{
		return string.Format("create local temporary table {6} as (select top 0 * from {0});\r\n                                   exec 'insert into {6} select * from \"{1}\";';\r\n                                   create local temporary table {7} as (select top 0 * from {2});\r\n                                   exec 'insert into {7} select * from \"{3}\";';\r\n                                   create local temporary table {8} as (select top 0 * from {4});\r\n                                   exec 'insert into {8} select * from \"{5}\";';\r\n                                   delete from {0} where \"AbsEntry\" = @DocEntry;\r\n                                   delete from {2} where \"AbsEntry\" = @DocEntry;\r\n                                   delete from {4} where \"AbsEntry\" = @DocEntry;\r\n\r\n                                   insert into {0} select * from {6};\r\n                                   update {7} set \"AbsEntry\" = @DocEntry;\r\n                                   insert into {2} select * from {7};\r\n                                   update {8} set \"AbsEntry\" = @DocEntry;\r\n                                   insert into {4} select * from {8};", "APZ_OWID", "#APZ_OWID", "APZ_WID1", "#APZ_WID1", "APZ_WID2", "#APZ_WID2", "#APZ_OWIDDATA", "#APZ_WID1DATA", "#APZ_WID2DATA");
	}
}
