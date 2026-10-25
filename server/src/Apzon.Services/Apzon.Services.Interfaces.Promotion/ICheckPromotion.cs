using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Promotion;

public interface ICheckPromotion
{
	HttpResult Save(DataTable model);

	DataTable GetExistDocPromotion(string guid);
}
