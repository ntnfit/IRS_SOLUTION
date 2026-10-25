using System.Data;

namespace Apzon.HttpClientHelper;

public interface ICostCenters
{
	DataTable Search(DataTable dtSearch);

	string DeleteCostCenters(string prcCode);

	string Save(DataTable dt);

	DataSet LoadComboBox();

	bool IsExistsCostCenters(string p);
}
