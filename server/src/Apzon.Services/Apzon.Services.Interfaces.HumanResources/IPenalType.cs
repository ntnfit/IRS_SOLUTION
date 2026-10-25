using System.Data;

namespace Apzon.Services.Interfaces.HumanResources;

public interface IPenalType
{
	DataTable GetData();

	string Save(DataTable dt);

	string Remove(int code);
}
