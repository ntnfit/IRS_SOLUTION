using Apzon.Services.Repository.ImportData.BillOfMaterials;
using Apzon.Services.Repository.ImportData.BusinessPartners;
using Apzon.Services.Repository.ImportData.HumanResources;
using Apzon.Services.Repository.ImportData.Inventory;

namespace Apzon.Services.Repository.ImportData;

public class BaseImportDataFactory
{
	public static BaseImportData GetBaseImportData(string objectType, string _connection, int userID, IUnitOfWork unitOfWork)
	{
		return objectType switch
		{
			"ImportDataBPMaster" => new ImportDataBPMaster(_connection, userID, unitOfWork), 
			"ImportDataItemMaster" => new ImportDataItemMaster(_connection, userID, unitOfWork), 
			"ImportDataBillOfMaterials" => new ImportDataBillOfMaterials(_connection, userID, unitOfWork), 
			"ImportDataEmployees" => new ImportDataEmployees(_connection, userID, unitOfWork), 
			_ => new BaseImportData(), 
		};
	}
}
