using System.Data;

namespace Apzon.HttpClientHelper;

public interface IPaymentTermSetup
{
	DataTable LoadDoc();

	string SaveDoc(DataTable dt);

	string DeleteDoc(int groupNum);

	DataTable GetPaymentTermInfo(int groupNum);
}
