using System.Data;

namespace Apzon.Services.Interfaces.PM;

public interface IFormatRule
{
	DataTable GetFormatRule(string formId, string itemId, string user);

	string Saves(DataTable dt);

	string CopyFormatRule(DataTable dt);

	string Delete(string formId, string itemId, string userId);
}
