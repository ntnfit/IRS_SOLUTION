using System.Data;

namespace Apzon.Services.Interfaces.Payment;

public interface IPaymentRecon
{
	DataTable GetDoc();

	DataTable GetLines(int docentry);

	string Save(DataSet ds);

	string Remove(int docentry);

	DataTable Search(DataTable dtSearch);
}
