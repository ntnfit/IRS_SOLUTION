using Apzon.Services.Repository.Administrator.Setup;

namespace Apzon.Services.HanaRepository.Administrator.Setup;

public class HanaVehicleRepository : VehicleRepository
{
	public HanaVehicleRepository(IDatabaseService databaseService)
		: base(databaseService)
	{
	}

	protected override string GetListQuery()
	{
		return "Select * from APZ_VHCE Order by \"Code\",\"Name\";";
	}

	protected override string GetDataTypeQuery()
	{
		return "Select \"Code\",\"Name\" from APZ_VHCT;";
	}

	protected override string GetDataBranchQuery()
	{
		return "Select \"BPLId\",\"BPLName\" from OBPL;";
	}
}
