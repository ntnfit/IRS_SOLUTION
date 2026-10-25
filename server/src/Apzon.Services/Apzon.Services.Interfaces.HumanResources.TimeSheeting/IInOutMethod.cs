using System.Data;

namespace Apzon.Services.Interfaces.HumanResources.TimeSheeting;

public interface IInOutMethod
{
	DataTable GetAll();

	string Save(DataSet ds);

	string Remove(int methodId);
}
