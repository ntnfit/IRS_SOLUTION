using System.Data;

namespace Apzon.Services.Interfaces.Financials;

public interface IDistributeRule
{
	DataTable GetOOCR();

	DataTable GetOCR1(string key);

	string Save(DataSet ds);

	DataTable Search(DataTable dtSearch);

	DataSet LoadComBoBox();

	string DeleteDistributeRule(string tupleOcrDim);

	DataTable LoadPrcCode();

	bool GetIsHasLinkedObject(string key);

	DataTable GetStruct();

	DataTable GetStructRow();
}
