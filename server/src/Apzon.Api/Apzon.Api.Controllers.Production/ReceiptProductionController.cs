using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;
using Apzon.Entities.Models.Production;

namespace Apzon.Api.Controllers.Production;

public class ReceiptProductionController : BaseApiController
{
	[HttpPost]
	public DataTable Search([FromBody] ReceiptProduction_Impl_OIGN doc)
	{
		return UnitOfWork.ReceiptProduction.Search(doc);
	}

	[HttpPost]
	public DataTable SearchLine([FromBody] ReceiptProduction_LineImpl_IGN1 line)
	{
		return UnitOfWork.ReceiptProduction.SearchLine(line);
	}

	[HttpGet]
	public DataTable LoadItemsByProdOrderEntry(int docEntry)
	{
		return UnitOfWork.ReceiptProduction.LoadItemsByProdOrderEntry(docEntry);
	}

	[HttpGet]
	public int GetMaxDocEntry()
	{
		return UnitOfWork.ReceiptProduction.GetMaxDocEntry();
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string SaveDoc([FromBody] ReceiptProduction_Impl_OIGN doc)
	{
		return "This function is not supported";
	}

	[HttpPost]
	[MappingSap(IsMapping = false)]
	public string DeleteIGN1(int docEntry, int lineNum)
	{
		return UnitOfWork.ReceiptProduction.DeleteIGN1(docEntry, lineNum);
	}

	[HttpGet]
	public DataTable GetFirstRecord()
	{
		return UnitOfWork.ReceiptProduction.GetFirstRecord();
	}

	[HttpGet]
	public DataTable GetLastRecord()
	{
		return UnitOfWork.ReceiptProduction.GetLastRecord();
	}

	[HttpGet]
	public DataTable GetPreviousRecord(string docEntry)
	{
		return UnitOfWork.ReceiptProduction.GetPreviousRecord(docEntry);
	}

	[HttpGet]
	public DataTable GetNextRecord(string docEntry)
	{
		return UnitOfWork.ReceiptProduction.GetNextRecord(docEntry);
	}

	[HttpGet]
	public DataTable CopyFrom(string type, string entry, int bplId, string username)
	{
		return UnitOfWork.Documents.CopyFrom(type, entry, 59.ToString(), bplId, username);
	}

	[HttpGet]
	public DataTable CopyFromLine(string type, string entry)
	{
		return UnitOfWork.Documents.CopyFromLine(type, entry, 59.ToString());
	}

	[HttpGet]
	public DataTable CopyBatchSerial(string fromentry, string fromtype)
	{
		return UnitOfWork.Documents.CopyBatchSerial(fromentry, fromtype);
	}
}
