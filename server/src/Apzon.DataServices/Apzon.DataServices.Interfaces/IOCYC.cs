using Apzon.Entities.Models.MasterData;

namespace Apzon.DataServices.Interfaces;

public interface IOCYC : IRepository<OCYC_Impl>
{
	string Save(OCYC_Impl item, string sessionId);
}
