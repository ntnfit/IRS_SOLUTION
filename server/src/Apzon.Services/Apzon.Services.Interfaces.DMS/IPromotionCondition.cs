using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.DMS;

public interface IPromotionCondition
{
	DataSet SearchPromotionCondition(DataTable dt);

	DataSet GetLines(int docentry);

	HttpResult Save(DataSet ds, string action);

	string GetNameByCode(string conditionCode);

	DataTable GetAttribute(string code, string objectType);
}
