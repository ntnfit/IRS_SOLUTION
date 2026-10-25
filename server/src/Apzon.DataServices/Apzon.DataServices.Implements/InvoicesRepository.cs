using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class InvoicesRepository : Repository<DocumentsImpl>, IInvoices, IRepository<DocumentsImpl>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public InvoicesRepository(Apzon.Services.IUnitOfWork serviUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public virtual HttpResult CreateInvoices(List<DocumentsImpl> docs, string sessionId)
	{
		try
		{
			string text = string.Empty;
			if (docs != null && docs.Count > 0)
			{
				for (int i = 0; i < docs.Count; i++)
				{
					text = text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(docs[i], EnumCommandType.Add, ApzObjectTypes.oInvoices) + "</dis:AddObject>";
				}
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
				XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						Logging.Write("ERROR", "Add Invoice error: " + xElement2.Value);
						return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
					}
				}
				else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
				{
					List<XElement> list = (from t in xElement.Descendants()
						where t.Name.LocalName.Equals("RetKey")
						select t).ToList();
					if (list.Count > 0)
					{
						List<string> list2 = new List<string>();
						list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
						if (list2.Count > 0)
						{
							Logging.Write("WATCH", "Add Invoice Success: " + string.Join(",", list2.ToArray()));
							return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
						}
					}
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void Dispose()
	{
	}
}
