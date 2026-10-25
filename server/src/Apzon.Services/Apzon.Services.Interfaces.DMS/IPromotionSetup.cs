using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface IPromotionSetup
{
	DataSet GetData();

	HttpResult SaveData(DataSet ds);

	string CheckUsed(string type, int id);
}
