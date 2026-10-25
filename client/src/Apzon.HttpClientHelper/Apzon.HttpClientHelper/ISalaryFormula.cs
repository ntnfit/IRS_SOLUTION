using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface ISalaryFormula
{
	Task<DataTable> GetFormula(string formCode, Enums.EventId enumEvent, int branch, int dept);

	Task<HttpResult> Save(DataTable model);

	string GetFormulaText(string formCode, Enums.EventId eventId, int branch, int dept);

	Task<string> DeleteFormula(string formId, Enums.EventId eventId, int branch, int dept);
}
