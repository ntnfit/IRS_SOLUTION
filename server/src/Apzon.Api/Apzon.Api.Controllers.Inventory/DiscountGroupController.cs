using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.DataServices;
using Apzon.Entities.Models.Common;

namespace Apzon.Api.Controllers.Inventory;

public class DiscountGroupController : BaseApiController
{
	[HttpGet]
	public DataTable GetTableColumns(string table)
	{
		return UnitOfWork.DiscountGroup.GetTableColumns(table);
	}

	[HttpGet]
	public DataTable SearchLine(int entry)
	{
		return UnitOfWork.DiscountGroup.SearchLine(entry);
	}

	[HttpGet]
	public DataSet LoadTabPagesData()
	{
		return UnitOfWork.DiscountGroup.LoadTabPagesData();
	}

	[HttpGet]
	public DataTable GenDocTable()
	{
		return UnitOfWork.DiscountGroup.GenDocTable();
	}

	[HttpGet]
	public DataTable GetAllBPEntry()
	{
		return UnitOfWork.DiscountGroup.GetAllBPEntry();
	}

	[HttpPost]
	public async Task<string> SaveDoc([FromBody] DataSet ds)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			IUnitOfWork unitOfWork = BrokerService.GetUnitOfWork(SessionId);
			if (!string.IsNullOrEmpty(SessionUserInfo.SessionDiServer))
			{
				return await unitOfWork.DiscountGroup.Save(SessionUserInfo.SessionDiServer, ds);
			}
			return "INVALIDSESSION";
		}
		return "This function is not supported";
	}

	[HttpPost]
	public HttpResult SaveDocNoSap([FromBody] DataSet ds)
	{
		return UnitOfWork.DiscountGroup.SaveDoc(ds);
	}

	[HttpPost]
	public DataTable Search([FromBody] DataTable dt)
	{
		return UnitOfWork.DiscountGroup.Search(dt);
	}
}
