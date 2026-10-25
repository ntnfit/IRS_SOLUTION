using Apzon.Services.Repository.Administrator.Banking;

namespace Apzon.Services.SqlRepository.Administrator.Banking;

public class SqlCreditCardRepository(IDatabaseService databaseService) : CreditCardRepository(databaseService)
{
	public new string TempTable = "#OCRC";

	private readonly string connection;
}
