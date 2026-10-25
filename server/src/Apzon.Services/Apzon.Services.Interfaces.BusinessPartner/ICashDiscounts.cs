using System.Data;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface ICashDiscounts : IService<CashDiscountsImpl>
{
	DataTable ShowCashDiscount();

	DataTable GetCashDiscount(string code);

	int GetMaxLineId();

	string SaveDoc(CashDiscountsImpl doc);

	string DeleteDoc(string code);
}
