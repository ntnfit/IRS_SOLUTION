using System.Collections.Generic;
using System.Data;
using Apzon.Entities.Models.Production;

namespace Apzon.Services.Interfaces.Service;

public interface IServiceCall
{
	DataTable GetSeries();

	DataTable GetCallStatus();

	DataTable GetOrigin();

	DataTable GetProblemType();

	DataTable GetProblemSubType();

	DataTable GetCallType();

	DataTable GetAssignee();

	DataTable GetLocation();

	DataTable GetCountries();

	DataTable GetState(string CountryCode);

	DataTable GetDoc(DataTable dts);

	DataTable GetLineGeneral(int callID);

	DataTable GetDocNum(int series);

	DataTable GetContactPerson();

	DataTable GetContactPerson(string cardCode);

	DataTable GetCallID();

	DataTable GetAddrType(string addressID);

	DataTable GetAdressID(string cardCode);

	DataTable GetStreet(string cardCode, string adressID);

	DataTable GetListItemGroup();

	DataTable GetActivities(int callID, int docNum);

	DataTable GetSolutions(int callID, int sltCode);

	DataTable GetHistory(int callID);

	DataTable GetNewAbsEntry();

	DataTable GetAttachment(int callID);

	DataTable GetItemsInfo(string itemCode);

	DataTable GetItemsInfo();

	string Save(DataRow doc, DataTable line, DataTable activiti, DataTable expenses, DataTable solution, List<AttachmentFile_Impl> attachment);
}
