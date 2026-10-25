using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IPaymentTermSetUp
{
	string Save(DataTable dtDoc, string sessionId);

	string Remove(int groupNum, string sessionId);
}
