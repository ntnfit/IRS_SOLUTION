using System.Collections.Generic;
using System.Data;
using System.Web.Http;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Production;

public class OutsourcingController : BaseApiController
{
	[HttpGet]
	public DataTable AddNewRow(string grid)
	{
		return UnitOfWork.Outsourcing.AddNewRow(grid);
	}

	[HttpGet]
	public DataTable GetItemDetails(string itemCode)
	{
		return UnitOfWork.Outsourcing.GetItemDetails(itemCode);
	}

	[HttpGet]
	public DataTable GetBatchSerials(string docEntry, string objType)
	{
		return UnitOfWork.Outsourcing.GetBatchSerials(docEntry, objType);
	}

	[HttpGet]
	public DataTable GetBinData(string code, int type, string itemCode)
	{
		return UnitOfWork.Outsourcing.GetBinData(code, type, itemCode);
	}

	[HttpGet]
	public DataTable GetBatchSerialsColumns()
	{
		return UnitOfWork.Outsourcing.GetBatchSerialsColumns();
	}

	[HttpPost]
	public DataTable Search([FromBody] DataTable dtSearch)
	{
		return UnitOfWork.Outsourcing.Search(dtSearch);
	}

	[HttpGet]
	public DataTable GetADoc(int docNum, string grid)
	{
		return UnitOfWork.Outsourcing.GetADoc(docNum, grid);
	}

	[HttpGet]
	public DataTable GetAllBinData(string itemCode, string whsCode)
	{
		return UnitOfWork.Outsourcing.GetAllBinData(itemCode, whsCode);
	}

	[HttpPost]
	public DataTable GetBOMComponents([FromBody] DataTable itemCodes)
	{
		return UnitOfWork.Outsourcing.GetBOMComponents(itemCodes);
	}

	[HttpPost]
	public DataTable GetTotalAvgPrice([FromBody] DataTable dtItemWhs)
	{
		return UnitOfWork.Outsourcing.GetTotalAvgPrice(dtItemWhs);
	}

	[HttpGet]
	public int GetRecord(string position, int code)
	{
		return UnitOfWork.Outsourcing.GetRecord(position, code);
	}

	[HttpGet]
	public bool ExistsCode(int code)
	{
		return UnitOfWork.Outsourcing.ExistsCode(code);
	}

	[HttpPost]
	public DataTable GetItemsAvgPrice([FromBody] DataTable dtItemWhs)
	{
		return UnitOfWork.Outsourcing.GetItemsAvgPrice(dtItemWhs);
	}

	[HttpPost]
	public string SaveDoc([FromBody] List<DocumentsImpl> lst)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "INVALIDSESSION";
			}
			return BrokerService.GetUnitOfWork(SessionId).Outsourcing.SaveDoc(lst, SessionUserInfo.SessionDiServer);
		}
		return "This method must be log in to SAP - (IRS)";
	}
}
