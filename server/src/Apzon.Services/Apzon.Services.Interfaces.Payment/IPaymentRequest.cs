using System.Data;

namespace Apzon.Services.Interfaces.Payment;

public interface IPaymentRequest
{
	DataTable GetDoc();

	string Save(DataSet ds);

	string UpdateStatus(DataSet ds);

	string Remove(int docentry);

	DataTable Search(DataTable dtSearch);

	string Approval(DataTable dt);
}
