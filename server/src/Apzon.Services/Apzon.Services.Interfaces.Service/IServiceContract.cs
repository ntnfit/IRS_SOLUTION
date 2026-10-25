using System.Data;

namespace Apzon.Services.Interfaces.Service;

public interface IServiceContract
{
	DataTable Search(DataTable dts);

	DataSet GetDetail(int contractID);

	DataTable GetContractID();

	string Save(DataSet data);
}
