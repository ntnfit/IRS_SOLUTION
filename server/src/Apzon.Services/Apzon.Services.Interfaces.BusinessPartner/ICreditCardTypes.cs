using System.Data;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface ICreditCardTypes : IService<CreditCardTypesImpl>
{
	DataTable ShowCreditCardType();

	string SaveDoc(CreditCardTypesImpl doc);

	string DeleteDoc(int code);

	int GetMaxCode();
}
