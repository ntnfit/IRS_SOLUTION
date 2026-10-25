using System.Data;
using Apzon.Entities.Models.Documents;

namespace Apzon.Services.Interfaces.Sales;

public interface ISalesBlanketAgreement
{
	DataTable GetDoc(DocumentSBAImpl condiDoc, string username);

	DataTable GetOwner();

	DataTable GetNumerMax(int series);

	DataTable GetNameCntctCode();

	DataTable GetLine(int AbsId);

	DataTable GetContactPerson(string cardCode);

	DataTable GetItemGroup();

	DataTable GetPriceList();

	string Save(DataSet dt);

	string DeleteRow(int AgrNo);

	int GetOwnerByBPCode(string cardCode);

	DataTable GetBlanketAgreementDetails(int AgrNo, int AgrLnNum);

	DataTable CheckExistActivity(int AgrNo, int AgrLnNum, int AgrEfctNum);

	DataTable GetActivitiesByActivityID(string lstClgCodes);
}
