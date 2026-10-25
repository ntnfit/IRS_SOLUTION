using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaVatGroupsRepository : VatGroupsRepository
{
	public HanaVatGroupsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	public override bool CheckExists(string code)
	{
		try
		{
			string query = "SELECT 1 FROM OVTG WHERE \"Code\"=@code;";
			if (ExecuteDataTable(query, CommandType.Text, new SqlParameter("@code", code)).IsNotNull())
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	public string Mapping(VatGroupsImpl doc, EnumCommandType commandType, string sessionId)
	{
		try
		{
			string text = "{";
			if (commandType == EnumCommandType.Add)
			{
				text += Function.GetJsonString("Code", doc.Code);
			}
			text += Function.GetJsonString("Name", doc.Name);
			text = ((!string.IsNullOrEmpty(doc.Category)) ? (text + (doc.Category.Equals("I") ? Function.GetJsonString("Category", "bovcInputTax") : Function.GetJsonString("Category", "bovcOutputTax"))) : (text + Function.GetJsonString("Category", "bovcOutputTax")));
			text += Function.GetJsonString("TaxAccount", doc.TaxAccount);
			text += ((doc.Inactive == "Y") ? Function.GetJsonString("Inactive", "tYES") : Function.GetJsonString("Inactive", "tNO"));
			text = ((string.IsNullOrEmpty(doc.EU) || !(doc.EU == "Y")) ? (text + Function.GetJsonString("EU", "tNO")) : (text + Function.GetJsonString("EU", "tYES")));
			if (!string.IsNullOrEmpty(doc.TriangularDeal))
			{
				text += Function.GetJsonString("TriangularDeal", doc.TriangularDeal);
			}
			text = ((string.IsNullOrEmpty(doc.AcquisitionReverse) || !(doc.AcquisitionReverse == "Y")) ? (text + Function.GetJsonString("AcquisitionReverse", "tNO")) : (text + Function.GetJsonString("AcquisitionReverse", "tYES")));
			text = ((!(doc.NonDeduct > 0.0)) ? (text + Function.GetJsonString("NonDeduct", 0)) : (text + Function.GetJsonString("NonDeduct", doc.NonDeduct)));
			text += Function.GetJsonString("AcquisitionTax", doc.AcquisitionTax);
			text += Function.GetJsonString("GoodsShipment", doc.GoodsShipment);
			if (!string.IsNullOrEmpty(doc.NonDeductAcc))
			{
				text += Function.GetJsonString("NonDeductAcc", doc.NonDeductAcc);
			}
			text += Function.GetJsonString("DeferredTaxAcc", doc.DeferredTaxAcc);
			text += ((doc.Correction == "Y") ? Function.GetJsonString("Correction", "tYES") : Function.GetJsonString("Correction", "tNO"));
			text += Function.GetJsonString("VatCorrection", doc.VatCorrection);
			text += Function.GetJsonString("EqualizationTaxAccount", doc.EqualizationTaxAccount);
			text += Function.GetJsonString("ServiceSupply", doc.ServiceSupply);
			if (doc.TaxTypeBlackList == "0")
			{
				text += Function.GetJsonString("TaxTypeBlackList", "ttblExcluded");
			}
			else if (doc.TaxTypeBlackList == "1")
			{
				text += Function.GetJsonString("TaxTypeBlackList", "ttblTaxable");
			}
			else if (doc.TaxTypeBlackList == "2")
			{
				text += Function.GetJsonString("TaxTypeBlackList", "ttblExempt");
			}
			else
			{
				text = ((!(doc.TaxTypeBlackList == "3")) ? (text + Function.GetJsonString("TaxTypeBlackList", "ttblNonSubject")) : (text + Function.GetJsonString("TaxTypeBlackList", "ttblNotTaxable")));
			}
			if (doc.Report349Code == "0")
			{
				text += Function.GetJsonString("Report349Code", "r349cEmpty");
			}
			else if (doc.Report349Code == "1")
			{
				text += Function.GetJsonString("Report349Code", "r349cE");
			}
			else if (doc.Report349Code == "2")
			{
				text += Function.GetJsonString("Report349Code", "r349cA");
			}
			else if (doc.Report349Code == "3")
			{
				text += Function.GetJsonString("Report349Code", "r349cT");
			}
			else if (doc.Report349Code == "4")
			{
				text += Function.GetJsonString("Report349Code", "r349cS");
			}
			else if (doc.Report349Code == "5")
			{
				text += Function.GetJsonString("Report349Code", "r349cI");
			}
			else
			{
				text = ((!(doc.Report349Code == "6")) ? (text + Function.GetJsonString("Report349Code", "r349cH")) : (text + Function.GetJsonString("Report349Code", "r349cM")));
			}
			if (doc.TaxRegion == "0")
			{
				text += Function.GetJsonString("TaxRegion", "vgtrPT");
			}
			else
			{
				text = ((!(doc.TaxRegion == "1")) ? (text + Function.GetJsonString("TaxRegion", "vgtrPT_MA")) : (text + Function.GetJsonString("TaxRegion", "vgtrPT_AC")));
			}
			text += "\"VatGroups_Lines\":[";
			foreach (VatGroups_LinesImpl vatGroups_Line in doc.VatGroups_Lines)
			{
				text += "{";
				text += Function.GetJsonString("Effectivefrom", Function.ParseDateTimes(vatGroups_Line.EffecDate).ToString("yyyy-MM-dd").Replace("-", ""));
				text += Function.GetJsonString("Rate", vatGroups_Line.Rate);
				text += Function.GetJsonString("EqualizationTax", vatGroups_Line.EqualizationTax);
				text += Function.GetJsonString("DatevCode", vatGroups_Line.DatevCode);
				text += "},";
			}
			text += "]";
			return text + "}";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public override string Save(List<VatGroupsImpl> docs, string sessionId)
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			string text = "";
			foreach (VatGroupsImpl doc in docs)
			{
				if (CheckExists(doc.Code))
				{
					text = Mapping(doc, EnumCommandType.Update, sessionId);
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.Update,
						ObjectType = ApzObjectTypes.oVatGroups,
						BodyObject = text,
						Key = Function.ToString(doc.Code),
						OutObjectKey = true
					});
				}
				else
				{
					text = Mapping(doc, EnumCommandType.Add, sessionId);
					list.Add(new BodyRequestObject
					{
						Action = Enums.HttpAction.AddNew,
						ObjectType = ApzObjectTypes.oVatGroups,
						BodyObject = text,
						OutObjectKey = true
					});
				}
			}
			return DataService.ProcessMutiTransaction(sessionId, list).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public override string Delete(string code, string sessionId)
	{
		try
		{
			HttpResult httpResult = DataService.RemoveObject(ApzObjectTypes.oVatGroups, sessionId, code);
			if (httpResult == null)
			{
				return "Can not get response from Service";
			}
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				return httpResult.Message;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}
}
