using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IShippingClient
{
	Task<DataTable> GetDoc(DataTable pr);

	Task<DataTable> GetLines(int docEntry);

	Task<int> GetNextId();

	Task<DataTable> GetLineByPickNo(int pickno);

	Task<DataSet> ChoosePickList();

	Task<HttpResult> Save(DataSet ds);

	Task<DataTable> GetLineByPackNo(int packno);

	Task<DataSet> ChoosePacking();

	Task<DataSet> ChooseBatch();

	Task<HttpResult> Close(int docEntry);

	Task<HttpResult> Cancel(int docEntry);

	Task<DataTable> Copy(ApzObjectTypes fromtype, ApzObjectTypes totype, List<int> lst);

	Task<DataSet> ChooseList(ApzObjectTypes apzObjectTypes, string cardcode);

	int GetOrderEntry(ApzObjectTypes fromtype, int fromentry);

	Task<DataTable> CopyHeader(ApzObjectTypes fromtype, ApzObjectTypes totype, List<int> lst);
}
