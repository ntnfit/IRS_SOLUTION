using System.Data;
using Apzon.Entities.Models.BusinessPartners;

namespace Apzon.Services.Interfaces.BusinessPartner;

public interface ICardGroups : IService<CardGroupsImpl>
{
	DataTable ShowCardGroup();

	int GetMaxCode();

	string SaveDoc(CardGroupsImpl doc);

	string DeleteDoc(int code);
}
