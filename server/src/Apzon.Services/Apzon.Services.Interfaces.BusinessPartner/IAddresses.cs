using System.Data;
using Apzon.Entities.Models.MasterData;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface IAddresses : IService<AddressesImpl>
{
	DataTable ShowAddress(string code, string addrType);

	string SaveDoc(AddressesImpl doc);

	string DeleteDoc(string addr, string code, string addrType);

	int GetMaxCode();
}
