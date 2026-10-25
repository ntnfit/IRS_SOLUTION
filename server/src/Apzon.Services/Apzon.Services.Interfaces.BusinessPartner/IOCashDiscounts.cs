using System.Data;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface IOCashDiscounts : IService<OCashDiscountsImpl>
{
	DataTable ShowOCashDiscount();

	DataTable GetOCashDiscount(string code);

	string SaveDoc(OCashDiscountsImpl doc);

	string DeleteDoc(string code);
}
