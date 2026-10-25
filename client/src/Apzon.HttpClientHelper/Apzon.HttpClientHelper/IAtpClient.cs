using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IAtpClient
{
	Task<DataTable> LoadAtpType();

	Task<HttpResult> GetNewIdAtp();

	Task<DataTable> GetStructureInput();

	Task<DataTable> GetDocStructure();

	Task<DataTable> RunAtp(DataSet ds);

	Task<HttpResult> Save(DataSet ds);

	Task<DataSet> SearchData(Enums.NavigateType type, int code);

	Task<HttpResult> AtpUpdate(DataSet ds);

	HttpResult GetNewIdScheduleForAtp();

	Task<DataSet> GetDataAtpForSchedle(int code);

	Task<DataSet> RunSchedule(string code, string type);

	Task<DataSet> GetDataLineSchedule(int id);

	Task<DataTable> GetDocScheduleStructure();

	Task<HttpResult> SaveSchedule(DataSet dtSet);

	Task<HttpResult> UpdateSchedule(DataSet dtSet);

	Task<DataTable> GetDataDocSchedule(int code, Enums.NavigateType type);
}
