using Apzon.Services.Repository.HumanResources.TimeSheeting;

namespace Apzon.Services.HanaRepository.HumanResources.TimeSheeting;

public class HanaTimeSheetingHourRepository : TimeSheetingHourRepository
{
	public HanaTimeSheetingHourRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetSaveTimeSheetingHourSettingQuery()
	{
		return "UPDATE APZ_TSHS SET \"Branch\"=@Branch, \"Department\"=@Department, \"Status\"=@Status, \"Period\"=@Period, \"OvertimeType\"=@OvertimeType, \"UserSign\"=@UserSign;\r\n                            IF (::ROWCOUNT = 0 ) \r\n                            THEN  \r\n                                    INSERT INTO APZ_TSHS (\"Branch\", \"Department\",\"Status\", \"Period\", \"OvertimeType\", \"UserSign\")  \r\n                                    VALUES (@Branch, @Department,@Status,@Period,@OvertimeType,@UserSign);\r\n                            END IF;";
	}
}
