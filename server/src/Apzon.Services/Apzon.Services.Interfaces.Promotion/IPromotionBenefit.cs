using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Promotion;

public interface IPromotionBenefit
{
	DataSet Search(DataTable dt);

	DataSet GetLines(int docentry);

	HttpResult Save(DataSet ds, string action);

	string GetNameByCode(string code);

	DataTable GetAttribute(string code);
}
