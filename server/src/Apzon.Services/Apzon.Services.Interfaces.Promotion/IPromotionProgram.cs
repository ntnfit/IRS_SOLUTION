using System.Data;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.Promotion;

public interface IPromotionProgram
{
	DataSet Search(DataTable dt);

	DataSet GetLinesBenefit(int docentry);

	DataSet GetLinesCondition(int docentry);

	HttpResult Save(DataSet ds, string action);
}
