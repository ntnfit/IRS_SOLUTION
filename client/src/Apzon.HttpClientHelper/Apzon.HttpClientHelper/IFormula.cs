using System.Data;
using System.Threading.Tasks;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.HttpClientHelper;

public interface IFormula
{
	Task<DataTable> GetFormula(string formCode, Enums.EventId enumEvent, string itemId);

	Task<HttpResult> Save(DataTable model);

	string GetFormulaText(string formCode, Enums.EventId eventId, string itemEvent);

	Task<string> DeleteFormula(string formId, string itemId, Enums.EventId eventId);
}
