using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Production;

namespace Apzon.Services.Interfaces.Opportunity;

public interface IActivitiesForOpportunity
{
	int GetMaxActivity_ClgCode();

	int GetMinClgCode();

	string SaveDoc(DataTable dt, List<AttachmentFile_Impl> lstAttachment);

	DataTable SearchDoc(DataTable dt);

	DataTable SearchActivityOverviewBP(DataTable dt);

	DataTable GetDefaultTable();

	DataTable GetActivityByClgCode(int clgCode);

	int CheckExistAttachment(int clgCode);

	DataTable ExistFileInDoc(int ClgCode, string fileName, string fileExt);

	DataTable GetFirstRecord();

	string GetLinkedObject(int clgCode);

	DataTable GetLastRecord();

	DataTable GetNextRecord(string id);

	DataTable GetPreviousRecord(string id);

	string Delete(int id);

	DataTable LoadOneDoc(int clgCode);

	DataTable GetCountries();

	DataTable GetStates(string country);

	int GetRecord(string position, int clgCode);

	bool CheckDoc(string clgCode);
}
