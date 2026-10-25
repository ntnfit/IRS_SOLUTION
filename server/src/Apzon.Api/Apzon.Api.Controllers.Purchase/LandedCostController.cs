using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Broker;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;

namespace Apzon.Api.Controllers.Purchase;

[ValidateLoginSapExceptionHandling]
public class LandedCostController : BaseApiController
{
	[HttpGet]
	public int GetMaxDocEntry()
	{
		return UnitOfWork.PLandedCost.GetMaxDocEntry();
	}

	[HttpGet]
	public DataTable GetLandedCosts(string docEntry)
	{
		return UnitOfWork.PLandedCost.GetLandedCosts(docEntry);
	}

	[HttpPost]
	public async Task<string> SaveDoc([FromBody] DocumentsImpl doc)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			return (await BrokerService.GetUnitOfWork(SessionId).Documents.Save(doc, SessionUserInfo.SessionDiServer, ApzObjectTypes.oPurchaseLandedCost)).Message;
		}
		return UnitOfWork.PLandedCost.SaveDoc(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDoc([FromBody] string code)
	{
		if (Global.LoginSap)
		{
			if (string.IsNullOrEmpty(SessionId))
			{
				return "Invalid session";
			}
			BrokerService.GetUnitOfWork(SessionId);
			return "Chức năng đang được cập nhật!";
		}
		return UnitOfWork.PLandedCost.DeleteDoc(code);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveUDF([FromBody] List<Common.Table_ID_Code> lst)
	{
		return UnitOfWork.PLandedCost.SaveUDF(lst);
	}

	[HttpGet]
	public DataTable LoadUDFData(string docEntry)
	{
		return UnitOfWork.PLandedCost.LoadUDFData(docEntry);
	}

	[HttpPost]
	public DataTable Search([FromBody] DocumentsImpl doc)
	{
		return UnitOfWork.PLandedCost.Search(doc);
	}

	[HttpGet]
	public DataTable GetLandedCostItem(string tblLine, string codes)
	{
		return UnitOfWork.PLandedCost.GetLandedCostItem(tblLine, codes);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDocItemLine([FromBody] Document_LinesImpl doc)
	{
		return UnitOfWork.PLandedCost.SaveDocItemLine(doc);
	}

	[HttpGet]
	public DataTable GetLandedCostLineByDocEntry(string docEntry)
	{
		return UnitOfWork.PLandedCost.GetLandedCostLineByDocEntry(docEntry);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDocCostLine([FromBody] Document_LinesImpl doc)
	{
		return UnitOfWork.PLandedCost.SaveDocCostLine(doc);
	}

	[HttpGet]
	public DataTable GetLandedCostItemByDocEntry(string docEntry)
	{
		return UnitOfWork.PLandedCost.GetLandedCostItemByDocEntry(docEntry);
	}

	[HttpGet]
	public int GetMaxDocEntryCostCode()
	{
		return UnitOfWork.PLandedCost.GetMaxDocEntryCostCode();
	}

	[HttpGet]
	public DataTable GetLandedCostCodeFormMain()
	{
		return UnitOfWork.PLandedCost.GetLandedCostCodeFormMain();
	}

	[HttpGet]
	public DataTable GetLandedCostCode()
	{
		return UnitOfWork.PLandedCost.GetLandedCostCode();
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDocCode([FromBody] Document_LinesImpl doc)
	{
		return UnitOfWork.PLandedCost.SaveDocCode(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteDocCode([FromBody] string code)
	{
		return UnitOfWork.PLandedCost.DeleteDocCode(code);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDocCostGRPODetail([FromBody] Document_LinesImpl doc)
	{
		return UnitOfWork.PLandedCost.SaveDocCostGRPODetail(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDocCostGRPO([FromBody] Document_LinesImpl doc)
	{
		return UnitOfWork.PLandedCost.SaveDocCostGRPO(doc);
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string Savesp_InsertWhseJournal([FromBody] DocumentsImpl doc)
	{
		return UnitOfWork.PLandedCost.Savesp_InsertWhseJournal(doc);
	}

	[HttpGet]
	public DataTable GetNext(string docEntry)
	{
		return UnitOfWork.PLandedCost.GetNext(docEntry);
	}

	[HttpGet]
	public DataTable GetPrevious(string docEntry)
	{
		return UnitOfWork.PLandedCost.GetPrevious(docEntry);
	}

	[HttpGet]
	public DataTable GetFirst()
	{
		return UnitOfWork.PLandedCost.GetFirst();
	}

	[HttpGet]
	public DataTable GetLast()
	{
		return UnitOfWork.PLandedCost.GetLast();
	}
}
