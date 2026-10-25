using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Apzon.Entities.Models.Production;

namespace Apzon.HttpClientHelper;

public interface IActivityClient
{
	Task<DataTable> GetCountries();

	Task<DataTable> GetStates(string country);

	Task<string> SaveDoc(Tuple<List<AttachmentFile_Impl>, DataTable> tpl);

	Task<DataTable> SearchDoc(DataTable dt);

	Task<DataTable> LoadOneDoc(int clgCode);

	Task<string> DeleteOCLG(int id);

	Task<DataTable> GetDefaultTable();

	Task<DataTable> GetAllTask();

	Task<DataTable> GetActivityByClgCode(int clgCode);

	Task<DataTable> BusinessPartner_GetTel_ByContactPerson(string cardCode, string cntctCode);

	Task<int> GetMaxActivity_ClgCode();

	Task<DataTable> ActivityStatus_GetAll();

	Task<DataTable> BusinessPartner_GetListContactPerson(string cardCode);

	Task<DataTable> ActivitySubject_GetAll(string madk);

	Task<DataTable> ActivityType_GetAll();

	Task<DataTable> Requester_GetRequesterByEmployee();

	Task<DataTable> Requester_GetUser_tableOUSR();

	Task<DataTable> ActivityMeetingLocation_GetAll();

	Task<int> GetRecord(string position, int clgCode);

	Task<bool> CheckDoc(int clgCode);

	string GetLinkedObject(int clgCode);
}
