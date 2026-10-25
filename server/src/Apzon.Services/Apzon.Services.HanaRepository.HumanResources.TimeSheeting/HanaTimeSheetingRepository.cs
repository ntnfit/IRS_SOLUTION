using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.HanaRepository.HumanResources.TimeSheeting;

public class HanaTimeSheetingRepository : TimeSheetingRepository
{
	public HanaTimeSheetingRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string DeleteWorkCodeQuery()
	{
		return "delete from APZ_OHWC where \"Code\"=@code and \"Branch\"=@Branch \r\n                 and not exists(select top 1 a.\"EmpId\" from APZ_TSL1 a \r\n                 inner join OHEM b on a.\"EmpId\"=b.\"empID\" where b.\"branch\"=@Branch and a.\"WorkCode\"=@code);";
	}

	protected override string DeleteOvertimeTypeQuery()
	{
		return "delete from APZ_OHOT \r\n                     where \"Code\"=@code and \"Branch\"=@Branch \r\n                     and not exists(select top 1  a.\"EmpId\" from APZ_TSH1 a inner join OHEM b on a.\"EmpId\" = b.\"empID\"\r\n                     where b.\"branch\"=@Branch and a.\"OvertimeType\"=@code);";
	}

	protected override string GetSaveTimeSheetingSettingQuery()
	{
		return "UPDATE APZ_TSLS SET \"Branch\"=@Branch, \"Department\"=@Department, \"Status\"=@Status, \"Period\"=@Period, \"UserSign\"=@UserSign;\r\n                            IF ::ROWCOUNT = 0 \r\n                                THEN  \r\n                                    INSERT INTO APZ_TSLS (\"Branch\", \"Department\",\"Status\", \"Period\", \"UserSign\")  \r\n                                    VALUES (@Branch, @Department,@Status,@Period,@UserSign);\r\n                                END IF;";
	}
}
