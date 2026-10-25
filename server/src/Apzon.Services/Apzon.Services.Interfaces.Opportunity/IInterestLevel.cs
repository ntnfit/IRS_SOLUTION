using System.Data;

namespace Apzon.Services.Interfaces.Opportunity;

public interface IInterestLevel
{
	DataTable GetDocs();

	string SaveDoc(DataTable dt);

	string DeleteDoc(int num);
}
