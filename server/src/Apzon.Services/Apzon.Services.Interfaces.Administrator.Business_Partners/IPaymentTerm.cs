using System.Data;

namespace Apzon.Services.Interfaces.Administrator.Business_Partners;

public interface IPaymentTerm
{
	DataTable LoadDoc();

	string Save(DataTable dtDoc);
}
