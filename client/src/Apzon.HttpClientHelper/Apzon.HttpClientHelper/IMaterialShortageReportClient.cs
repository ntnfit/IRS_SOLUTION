using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;

namespace Apzon.HttpClientHelper;

public interface IMaterialShortageReportClient
{
	Task<DataTable> Run(MaterialShortageReport_Impl input);

	Task<DataSet> SearchData(Enums.NavigateType type, int code);

	Task<HttpResult> GetNewId();

	Task<DataTable> LoadAtpType();

	Task<DataTable> GetDocStructure();

	Task<HttpResult> AddNew(DataSet ds);

	Task<HttpResult> Update(DataSet ds);
}
