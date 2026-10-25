using Apzon.Entities.Models.UserFields;

namespace Apzon.Entities.Models.Administrator;

public class DefaultCreditCardsImpl
{
	public string Code { get; set; }

	public int Count { get; set; }

	public string CreditAccountCode { get; set; }

	public int CreditCardCode { get; set; }

	public UserFieldsImpl UserFields { get; set; }
}
