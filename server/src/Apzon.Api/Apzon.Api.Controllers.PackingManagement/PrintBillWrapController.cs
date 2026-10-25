using System;
using System.Data;
using System.Web.Http;
using Apzon.Api.Attributes;
using Apzon.Api.Controllers.Sales;

namespace Apzon.Api.Controllers.PackingManagement;

[ValidateLoginSapExceptionHandling]
public class PrintBillWrapController : BaseApiController
{
	[HttpGet]
	public DataTable GetCase(int POCode)
	{
		return UnitOfWork.PrintBillWrap.GetCase(POCode);
	}

	[HttpGet]
	public string DelCase(int LineNumber)
	{
		return UnitOfWork.PrintBillWrap.DelCase(LineNumber);
	}

	[HttpGet]
	public string DelItem(string LineNum)
	{
		return UnitOfWork.PrintBillWrap.DelItem(LineNum);
	}

	[HttpPost]
	public string InsertCase([FromBody] Tuple<string, int, int> model)
	{
		return UnitOfWork.PrintBillWrap.InsertCase(model.Item1, model.Item2, model.Item3);
	}

	[HttpPost]
	public string InsertItem([FromBody] Tuple<string, string, int> model)
	{
		return UnitOfWork.PrintBillWrap.InsertItem(model.Item1, model.Item2, model.Item3);
	}
}
