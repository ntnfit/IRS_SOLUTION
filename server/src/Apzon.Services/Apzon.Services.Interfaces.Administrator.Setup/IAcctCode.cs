using System.Collections.Generic;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Services.Interfaces.Administrator.Setup;

public interface IAcctCode : IService<AcctCodeImpl>
{
	List<AcctCodeImpl> Cfl(string accountType);
}
