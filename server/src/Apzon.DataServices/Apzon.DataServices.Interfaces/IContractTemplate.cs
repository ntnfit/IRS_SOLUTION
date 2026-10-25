using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IContractTemplate
{
	string SaveDoc(DataSet ds, string sessionId);
}
