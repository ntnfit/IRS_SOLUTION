using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IPackingClient
{
	Task<DataTable> GetDoc(DataTable pr);

	Task<DataTable> GetLines(int docEntry);

	Task<int> GetNextId();

	Task<DataTable> GetLineByPickNo(int pickno);

	Task<DataSet> ChoosePickList(string cardcode);

	Task<HttpResult> Save(DataSet ds);

	Task<DataTable> GetLineByPackNo(int packno);

	Task<DataSet> ChoosePacking();

	Task<DataSet> ChooseBatch();

	Task<HttpResult> Close(int docEntry);

	Task<HttpResult> Cancel(int docEntry);
}
