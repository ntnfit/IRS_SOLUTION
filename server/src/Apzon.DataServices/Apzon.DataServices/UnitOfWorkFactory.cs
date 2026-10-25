using Apzon.Commons.Enums;
using Apzon.Services;

namespace Apzon.DataServices;

public class UnitOfWorkFactory
{
	public static IUnitOfWork GetUnitOfWork(string connectionString, string serviceAddress, Enums.DatabaseSystemType systemType, Enums.ServiceType serviceType)
	{
		if (systemType == Enums.DatabaseSystemType.HANA)
		{
			return new HanaUnitOfWork(new Apzon.Services.HanaUnitOfWork(connectionString), serviceAddress);
		}
		if (serviceType == Enums.ServiceType.DiServer)
		{
			return new UnitOfWork(new Apzon.Services.UnitOfWork(connectionString), serviceAddress);
		}
		return new HanaUnitOfWork(new Apzon.Services.UnitOfWork(connectionString), serviceAddress);
	}
}
