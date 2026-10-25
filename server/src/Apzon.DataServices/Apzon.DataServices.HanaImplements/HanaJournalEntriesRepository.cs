using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Financials;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaJournalEntriesRepository : JournalEntriesRepository
{
	private readonly IUnitOfWork _unitOfWork;

	private ApzObjectTypes objType = ApzObjectTypes.oJournalEntries;

	public HanaJournalEntriesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public override string Mapping(JournalEntriesImpl model, EnumCommandType commandType)
	{
		try
		{
			string text = "{";
			if (commandType == EnumCommandType.Update)
			{
				text += Function.GetJsonString("JdtNum", Function.ParseInt(model.TransId));
			}
			if (model.RefDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("ReferenceDate", Convert.ToDateTime(model.RefDate).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(model.Memo))
			{
				text += Function.GetJsonString("Memo", Function.RawText(model.Memo));
			}
			if (!string.IsNullOrEmpty(model.Ref1))
			{
				text += Function.GetJsonString("Reference", Function.RawText(model.Ref1));
			}
			if (!string.IsNullOrEmpty(model.Ref2))
			{
				text += Function.GetJsonString("Reference2", Function.RawText(model.Ref2));
			}
			if (!string.IsNullOrEmpty(model.TransCode))
			{
				text += Function.GetJsonString("TransactionCode", Function.RawText(model.TransCode));
			}
			if (!string.IsNullOrEmpty(model.Project))
			{
				text += Function.GetJsonString("ProjectCode", Function.RawText(model.Project));
			}
			if (model.TaxDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("TaxDate", Convert.ToDateTime(model.TaxDate).ToString("yyyyMMdd"));
			}
			if (!string.IsNullOrEmpty(model.Indicator))
			{
				text += Function.GetJsonString("Indicator", Function.RawText(model.Indicator));
			}
			text += Function.GetJsonString("UseAutoStorno", model.AutoStorno.ToString());
			if (model.StornoDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("StornoDate", Convert.ToDateTime(model.StornoDate).ToString("yyyyMMdd"));
			}
			if (model.VatDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("VatDate", Convert.ToDateTime(model.VatDate).ToString("yyyyMMdd"));
			}
			if (!Function.ParseInt(model.Series).Equals(0))
			{
				text += Function.GetJsonString("Series", Function.ParseInt(model.Series));
			}
			if (model.DueDate > new DateTime(1900, 1, 1))
			{
				text += Function.GetJsonString("DueDate", Convert.ToDateTime(model.DueDate).ToString("yyyyMMdd"));
			}
			if (!Function.ParseInt(model.Location).Equals(0))
			{
				text += Function.GetJsonString("LocationCode", Function.ParseInt(model.Location));
			}
			if (!string.IsNullOrEmpty(model.Ref3))
			{
				text += Function.GetJsonString("Reference3", Function.RawText(model.Ref3));
			}
			if (!string.IsNullOrEmpty(model.DocType))
			{
				text += Function.GetJsonString("DocumentType", Function.RawText(model.DocType));
			}
			text += DataService.MappingUdf(model.UserFields);
			if (model.Lines != null && model.Lines.Count > 0)
			{
				text += "JournalEntryLines:[";
				for (int i = 0; i < model.Lines.Count; i++)
				{
					text += "{";
					JournalEntries_LinesImpl journalEntries_LinesImpl = model.Lines[i];
					if (!Function.ParseInt(journalEntries_LinesImpl.Line_ID).Equals(0))
					{
						text += Function.GetJsonString("Line_ID", Function.ParseInt(journalEntries_LinesImpl.Line_ID));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.Account))
					{
						text += Function.GetJsonString("AccountCode", Function.RawText(journalEntries_LinesImpl.Account));
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.Debit).Equals(0.0))
					{
						text += Function.GetJsonString("Debit", Function.ParseDouble(journalEntries_LinesImpl.Debit));
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.Credit).Equals(0.0))
					{
						text += Function.GetJsonString("Credit", Function.ParseDouble(journalEntries_LinesImpl.Credit));
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.FCDebit).Equals(0.0))
					{
						text += Function.GetJsonString("FCDebit", Function.ParseDouble(journalEntries_LinesImpl.FCDebit));
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.FCCredit).Equals(0.0))
					{
						text += Function.GetJsonString("FCCredit", Function.ParseDouble(journalEntries_LinesImpl.FCCredit));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.FCCurrency))
					{
						text += Function.GetJsonString("FCCurrency", Function.RawText(journalEntries_LinesImpl.FCCurrency));
					}
					if (journalEntries_LinesImpl.DueDate > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("DueDate", Convert.ToDateTime(journalEntries_LinesImpl.DueDate).ToString("yyyyMMdd"));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.ShortName))
					{
						text += Function.GetJsonString("ShortName", Function.RawText(journalEntries_LinesImpl.ShortName));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.ContraAct))
					{
						text += Function.GetJsonString("ContraAccount", Function.RawText(journalEntries_LinesImpl.ContraAct));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.LineMemo))
					{
						text += Function.GetJsonString("LineMemo", Function.RawText(journalEntries_LinesImpl.LineMemo));
					}
					if (journalEntries_LinesImpl.RefDate > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("ReferenceDate1", Convert.ToDateTime(journalEntries_LinesImpl.RefDate).ToString("yyyyMMdd"));
					}
					if (journalEntries_LinesImpl.Ref2Date > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("ReferenceDate2", Convert.ToDateTime(journalEntries_LinesImpl.Ref2Date).ToString("yyyyMMdd"));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.Ref1))
					{
						text += Function.GetJsonString("Reference1", Function.RawText(journalEntries_LinesImpl.Ref1));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.Ref2))
					{
						text += Function.GetJsonString("Reference2", Function.RawText(journalEntries_LinesImpl.Ref2));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.Project))
					{
						text += Function.GetJsonString("ProjectCode", Function.RawText(journalEntries_LinesImpl.Project));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.ProfitCode))
					{
						text += Function.GetJsonString("CostingCode", Function.RawText(journalEntries_LinesImpl.ProfitCode));
					}
					if (journalEntries_LinesImpl.TaxDate > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("TaxDate", Convert.ToDateTime(journalEntries_LinesImpl.TaxDate).ToString("yyyyMMdd"));
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.BaseSum).Equals(0.0))
					{
						text += Function.GetJsonString("BaseSum", Function.ParseDouble(journalEntries_LinesImpl.BaseSum));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.VatGroup))
					{
						text += Function.GetJsonString("TaxGroup", Function.RawText(journalEntries_LinesImpl.VatGroup));
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.SYSDeb).Equals(0.0))
					{
						text += Function.GetJsonString("DebitSys", Function.ParseDouble(journalEntries_LinesImpl.SYSDeb));
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.SYSCred).Equals(0.0))
					{
						text += Function.GetJsonString("CreditSys", Function.ParseDouble(journalEntries_LinesImpl.SYSCred));
					}
					if (journalEntries_LinesImpl.VatDate > new DateTime(1900, 1, 1))
					{
						text += Function.GetJsonString("VatDate", Convert.ToDateTime(journalEntries_LinesImpl.VatDate).ToString("yyyyMMdd"));
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.SYSBaseSum).Equals(0.0))
					{
						text += Function.GetJsonString("SystemBaseAmount", Function.ParseDouble(journalEntries_LinesImpl.SYSBaseSum));
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.VatAmount).Equals(0.0))
					{
						text += Function.GetJsonString("VatAmount", Function.ParseDouble(journalEntries_LinesImpl.VatAmount));
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.SYSVatSum).Equals(0.0))
					{
						text += Function.GetJsonString("SystemVatAmount", Function.ParseDouble(journalEntries_LinesImpl.SYSVatSum));
					}
					if (!Function.ParseDouble(journalEntries_LinesImpl.GrossValue).Equals(0.0))
					{
						text += Function.GetJsonString("GrossValue", Function.ParseDouble(journalEntries_LinesImpl.GrossValue));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.Ref3Line))
					{
						text += Function.GetJsonString("AdditionalReference", Function.RawText(journalEntries_LinesImpl.Ref3Line));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.OcrCode2))
					{
						text += Function.GetJsonString("CostingCode2", Function.RawText(journalEntries_LinesImpl.OcrCode2));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.OcrCode3))
					{
						text += Function.GetJsonString("CostingCode3", Function.RawText(journalEntries_LinesImpl.OcrCode3));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.OcrCode4))
					{
						text += Function.GetJsonString("CostingCode4", Function.RawText(journalEntries_LinesImpl.OcrCode4));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.OcrCode5))
					{
						text += Function.GetJsonString("CostingCode5", Function.RawText(journalEntries_LinesImpl.OcrCode5));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.TaxCode))
					{
						text += Function.GetJsonString("TaxCode", Function.RawText(journalEntries_LinesImpl.TaxCode));
					}
					if (!string.IsNullOrEmpty(journalEntries_LinesImpl.ControlAccount))
					{
						text += Function.GetJsonString("ControlAccount", Function.RawText(journalEntries_LinesImpl.ControlAccount));
					}
					if (!Function.ParseInt(journalEntries_LinesImpl.BPLId).Equals(0))
					{
						text += Function.GetJsonString("BPLID", Function.ParseInt(journalEntries_LinesImpl.BPLId));
					}
					text += DataService.MappingUdf(journalEntries_LinesImpl.UserFields);
					text += "},";
				}
				text += "]";
			}
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public override HttpResult AddNew(JournalEntriesImpl doc, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				Action = Enums.HttpAction.AddNew,
				BodyObject = Mapping(doc, EnumCommandType.Add),
				ObjectType = objType,
				OutObjectKey = true
			});
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult UpdateData(JournalEntriesImpl doc, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				Action = Enums.HttpAction.Update,
				BodyObject = Mapping(doc, EnumCommandType.Update),
				Key = Function.ParseInt(doc.TransId),
				ObjectType = objType,
				OutObjectKey = true
			});
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override bool Exist(JournalEntriesImpl doc, string sessionId)
	{
		try
		{
			return ExecuteDataTable("select 1 from OJDT where \"TransId\" = @transId;", CommandType.Text, new SqlParameter("@transId", doc.TransId)).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public override HttpResult SaveData(JournalEntriesImpl doc, string sessionId)
	{
		try
		{
			if (!Exist(doc, sessionId))
			{
				return AddNew(doc, sessionId);
			}
			return UpdateData(doc, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult CreateJournalVoucher(JournalEntriesImpl lstJE, string sessionId)
	{
		try
		{
			string text = Mapping(lstJE, EnumCommandType.Add);
			string xmlObject = "{\r\n                                \"JournalVoucher\":{\r\n                                    \"JournalEntry\": " + text + "  \r\n                                                    }\r\n                                }";
			return DataService.AddServiceObject(ApzObjectTypes.oJournalVouchers, sessionId, xmlObject);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult CancelJE(int transId, string sessionId)
	{
		try
		{
			return DataService.ProcessJson(ApzObjectTypes.oJournalEntries, sessionId, "", Enums.HttpAction.Cancel, transId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
