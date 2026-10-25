using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading.Tasks;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaInternalReconciliationsRepository : InternalReconciliationsRepository
{
	public HanaInternalReconciliationsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override HttpResult Add(InternalReconciliationImpl doc, string sessionId)
	{
		try
		{
			HttpResult httpResult = DataService.AddObject(ApzObjectTypes.oInternalReconciliations, sessionId, Mapping(doc));
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult;
			}
			int num = Function.ParseInt(httpResult.Content);
			return new HttpResult(Enums.MessageCode.Success, "", num);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage)
			{
				Message = ex.Message
			};
		}
	}

	protected string Mapping(InternalReconciliationImpl doc)
	{
		string text = "{";
		_ = doc.ReconDate;
		text += Function.GetJsonString("ReconDate", doc.ReconDate.ToString("yyyyMMdd"));
		text = ((!Function.ToString(doc.IsCard).Equals("C")) ? (text + Function.GetJsonString("CardOrAccount", "coaAccount")) : (text + Function.GetJsonString("CardOrAccount", "coaCard")));
		switch (Function.ToString(doc.ReconType))
		{
		default:
			text += "\"InternalReconciliationOpenTransRows\": [";
			if (doc.InternalReconciliationRows != null)
			{
				if (doc.InternalReconciliationRows.Count > 100)
				{
					ConcurrentBag<string> lstLine = new ConcurrentBag<string>();
					if (!Parallel.ForEach(doc.InternalReconciliationRows, (InternalReconciliationLineImpl line) =>
					{
						string text2 = "{";
						text2 += Function.GetJsonString("Selected", "tYES");
						if (!string.IsNullOrEmpty(line.ShortName))
						{
							text2 += Function.GetJsonString("ShortName", line.ShortName);
						}
						if (Function.ParseInt(line.TransId) > 0)
						{
							text2 += Function.GetJsonString("TransId", line.TransId);
						}
						if (Function.ParseInt(line.TransRowId) >= 0)
						{
							text2 += Function.GetJsonString("TransRowId", line.TransRowId);
						}
						if (!string.IsNullOrEmpty(line.SrcObjTyp))
						{
							text2 += Function.GetJsonString("SrcObjTyp", line.SrcObjTyp);
						}
						if (Function.ParseInt(line.SrcObjAbs) > 0)
						{
							text2 += Function.GetJsonString("SrcObjAbs", line.SrcObjAbs);
						}
						text2 = ((!Function.ToString(line.IsCredit).Equals("C")) ? (text2 + Function.GetJsonString("CreditOrDebit", "codDebit")) : (text2 + Function.GetJsonString("CreditOrDebit", "codCredit")));
						text2 += Function.GetJsonString("ReconcileAmount", line.ReconSum);
						text2 += Function.GetJsonString("CashDiscount", line.CashDiscount);
						text2 += "}";
						lstLine.Add(text2);
					}).IsCompleted)
					{
						throw new Exception("Could not complete action-parallel mapping json");
					}
					text += string.Join(",\n", lstLine);
					try
					{
						GC.Collect();
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", ex);
					}
				}
				else
				{
					for (int num = 0; num < doc.InternalReconciliationRows.Count; num++)
					{
						InternalReconciliationLineImpl internalReconciliationLineImpl = doc.InternalReconciliationRows[num];
						text += "{";
						text += Function.GetJsonString("Selected", "tYES");
						if (!string.IsNullOrEmpty(internalReconciliationLineImpl.ShortName))
						{
							text += Function.GetJsonString("ShortName", internalReconciliationLineImpl.ShortName);
						}
						if (Function.ParseInt(internalReconciliationLineImpl.TransId) > 0)
						{
							text += Function.GetJsonString("TransId", internalReconciliationLineImpl.TransId);
						}
						if (Function.ParseInt(internalReconciliationLineImpl.TransRowId) >= 0)
						{
							text += Function.GetJsonString("TransRowId", internalReconciliationLineImpl.TransRowId);
						}
						if (!string.IsNullOrEmpty(internalReconciliationLineImpl.SrcObjTyp))
						{
							text += Function.GetJsonString("SrcObjTyp", internalReconciliationLineImpl.SrcObjTyp);
						}
						if (Function.ParseInt(internalReconciliationLineImpl.SrcObjAbs) > 0)
						{
							text += Function.GetJsonString("SrcObjAbs", internalReconciliationLineImpl.SrcObjAbs);
						}
						text = ((!Function.ToString(internalReconciliationLineImpl.IsCredit).Equals("C")) ? (text + Function.GetJsonString("CreditOrDebit", "codDebit")) : (text + Function.GetJsonString("CreditOrDebit", "codCredit")));
						text += Function.GetJsonString("ReconcileAmount", internalReconciliationLineImpl.ReconSum);
						text += Function.GetJsonString("CashDiscount", internalReconciliationLineImpl.CashDiscount);
						text += "},";
					}
				}
			}
			return text + "]}";
		}
	}
}
