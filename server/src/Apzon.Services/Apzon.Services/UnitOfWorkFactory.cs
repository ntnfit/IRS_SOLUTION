using Apzon.Commons.Enums;

namespace Apzon.Services;

public static class UnitOfWorkFactory
{
	public static IUnitOfWork GetUnitOfWork(string strConnection, Enums.DatabaseSystemType databaseSystemType)
	{
		if (databaseSystemType == Enums.DatabaseSystemType.HANA)
		{
			return new HanaUnitOfWork(strConnection);
		}
		return new UnitOfWork(strConnection);
	}
}
