using Apzon.Services.Repository.Commons;

namespace Apzon.Services.HanaRepository.Commons;

public class HanaPublishServiceRepository : PublishServiceRepository
{
	public HanaPublishServiceRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string SaveDataGetQueryDocEntry()
	{
		return "SELECT IFNULL((SELECT MAX(\"AbsEntry\") FROM APZ_OCPS),0)+1 from DUMMY;";
	}

	protected override string NavigateGetNextQuery()
	{
		return "DECLARE MaxId INT;\r\n                                    select IFNULL((SELECT MAX(\"AbsEntry\") FROM APZ_OCPS),0) into MaxId from DUMMY;\r\n                                    IF MaxId = AbsEntry\r\n                                    THEN\r\n                                        SELECT TOP 1 \"AbsEntry\" FROM APZ_OCPS ORDER BY \"AbsEntry\";\r\n                                    ELSE\r\n                                        SELECT TOP 1 \"AbsEntry\" FROM APZ_OCPS WHERE \"AbsEntry\" > AbsEntry ORDER BY \"AbsEntry\";\r\n                                    END IF;";
	}

	protected override string NavigateGetPreviousQuery()
	{
		return "DECLARE MinId INT;\r\n                                    select IFNULL((SELECT MIN(\"AbsEntry\") FROM APZ_OCPS),0) into MinId from DUMMY;\r\n                                    IF MinId=AbsEntry\r\n                                    THEN\r\n                                        SELECT TOP 1 \"AbsEntry\" FROM APZ_OCPS ORDER BY \"AbsEntry\" DESC;\r\n                                    ELSE\r\n                                        SELECT TOP 1 \"AbsEntry\" FROM APZ_OCPS WHERE \"AbsEntry\"<AbsEntry ORDER BY \"AbsEntry\" DESC;\r\n                                    END IF;";
	}

	protected override string GetDeleteQuery()
	{
		return "DECLARE ActionEntry int;\r\n                                DECLARE ActionFormId nvarchar(100);\r\n                                DECLARE ActionItemId nvarchar(100);\r\n                                select ifnull((SELECT \"ActionFormId\" FROM APZ_OCPS WHERE \"AbsEntry\"=AbsEntry),'') into ActionFormId from DUMMY;\r\n                                select ifnull((SELECT \"ActionEntry\" FROM APZ_OCPS WHERE \"AbsEntry\"=AbsEntry),0) into ActionEntry from DUMMY;\r\n                                select IFnull((SELECT \"ActionItemId\" FROM APZ_OCPS WHERE \"AbsEntry\"=AbsEntry),'') into ActionItemId from DUMMY;\r\n\r\n                                DELETE FROM APZ_OCTA WHERE \"AbsEntry\"=ActionEntry and \"FormId\"=ActionFormId and \"ItemId\"=ActionItemId;\r\n                                DELETE FROM APZ_OCPS WHERE \"AbsEntry\"=AbsEntry;\r\n                                DELETE FROM APZ_CPS1 WHERE \"AbsEntry\"=AbsEntry;";
	}
}
