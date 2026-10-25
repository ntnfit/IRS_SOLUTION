using System.Data;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface IContactPersons : IService<ContactPersonImpl>
{
	DataTable ShowContactPerson(string code);

	string SaveDoc(ContactPersonImpl doc);

	string DeleteDoc(string code);

	int GetMaxCode();
}
