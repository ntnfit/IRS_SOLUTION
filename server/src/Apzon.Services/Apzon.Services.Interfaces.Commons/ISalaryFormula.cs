using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface ISalaryFormula
{
	DataSet GetData(string sql);

	HttpResult Save(DataTable model);

	string GetFormulaText(string formId, Enums.EventId eventId, int branch, int dept);

	DataTable GetFormula(string formCode, Enums.EventId enumEvent, int branch, int dept);

	string DeleteFormula(string formId, Enums.EventId eventId, int branch, int dept);
}
