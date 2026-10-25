using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Commons;

public interface IFormula
{
	DataSet GetData(string sql);

	HttpResult Save(DataTable model);

	string GetFormulaText(string formId, Enums.EventId eventId, string itemEvent);

	DataTable GetCategory();

	DataTable GetFormula(string formCode, Enums.EventId enumEvent, string itemId);

	string DeleteFormula(string formId, string itemId, Enums.EventId eventId);
}
