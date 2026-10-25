using System;
using System.Collections.Generic;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaInvoicesRepository : InvoicesRepository
{
	public HanaInvoicesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override HttpResult CreateInvoices(List<DocumentsImpl> docs, string sessionId)
	{
		try
		{
			if (docs != null && docs.Count > 0)
			{
				List<BodyRequestObject> list = new List<BodyRequestObject>();
				for (int i = 0; i < docs.Count; i++)
				{
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.AddNew,
						BodyObject = _unitOfWork.Documents.Mapping(docs[i], EnumCommandType.Add, ApzObjectTypes.oInvoices),
						ObjectType = ApzObjectTypes.oInvoices,
						OutObjectKey = true
					});
				}
				return DataService.ProcessMutiTransaction(sessionId, list);
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
