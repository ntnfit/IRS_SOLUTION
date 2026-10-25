using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Banking;

public interface ICreditCard
{
	DataTable LoadDoc();

	string Save(DataTable dtDoc);
}
