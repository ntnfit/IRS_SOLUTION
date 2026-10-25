using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaManufacturersRepository : ManufacturersRepository
{
	public HanaManufacturersRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetMaxCodeQuery()
	{
		return "SELECT IFNULL( MAX(\"FirmCode\" ),0) as \"FirmCode\"\r\n                            FROM OMRC ;";
	}
}
