using System.Data;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;

namespace Apzon.Services.Interfaces.RoutingAndResource;

public interface IResourceWorkCenter
{
	DataTable GetDataResourceWorkCenter();

	DataSet GetDataOriginal();

	DataTable GetDataDoc(string resCode, Enums.NavigateType type);

	DataTable SearchResourceWorkCenter(DataTable dtSearch);

	HttpResult SaveData(DataTable dt);

	HttpResult DeleteResource(DataTable dt);

	DataTable GetDataWorkCenterByCode(string code);
}
