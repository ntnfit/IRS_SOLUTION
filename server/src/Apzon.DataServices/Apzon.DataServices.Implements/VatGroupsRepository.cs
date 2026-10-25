using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class VatGroupsRepository : Repository<VatGroupsImpl>, IVatGroups, IRepository<VatGroupsImpl>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public VatGroupsRepository(Apzon.Services.IUnitOfWork serviUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public virtual bool CheckExists(string code)
	{
		return false;
	}

	public virtual string Save(List<VatGroupsImpl> docs, string sessionId)
	{
		try
		{
			sessionId = GetSessionId(sessionId);
			if (docs != null && docs.Count > 0)
			{
				string text = string.Empty;
				string text2 = "";
				for (int i = 0; i < docs.Count; i++)
				{
					text2 = ((!CheckExist(docs[i].Code)) ? ("<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <env:Header>\r\n                                            <SessionID>" + sessionId + "</SessionID>\r\n                                        </env:Header><env:Body>\r\n                                        <dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXml(docs[i], EnumCommandType.Add) + "</dis:AddObject></env:Body></env:Envelope>") : ("<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <env:Header>\r\n                                            <SessionID>" + sessionId + "</SessionID>\r\n                                        </env:Header><env:Body>\r\n                                        <dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXml(docs[i], EnumCommandType.Update) + "</dis:UpdateObject></env:Body></env:Envelope>"));
					text += text2;
				}
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Envelopes>";
				XmlElement xmlElement = DataService.BatchProcess(xmlObject).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						Logging.Write("ERROR", "Save  Error ", xElement2.Value);
						return xElement2.Value;
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
							Logging.Write("WATCH", "Save  Success ", string.Join(",", list2.ToArray()));
							return "";
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	public virtual HttpResult SaveVatGroups(DataTable dt, string sessionId)
	{
		return new HttpResult();
	}

	public void Dispose()
	{
		throw new NotImplementedException();
	}

	protected bool CheckExist(string code)
	{
		try
		{
			string query = "SELECT 1\r\n                            FROM OVTG \r\n                            WHERE Code =  @code ";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@code", code)).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return false;
		}
	}

	private string MappingXml(VatGroupsImpl model, EnumCommandType type)
	{
		try
		{
			string text = "<BOM>\r\n                              <BO>\r\n                              <AdmInfo>\r\n                                <Object>oVatGroups</Object>\r\n                              </AdmInfo>";
			if (type == EnumCommandType.Update)
			{
				text = text + "<QueryParams>\r\n                                <Code>" + Function.ToString(model.Code) + "</Code>\r\n                            </QueryParams>";
			}
			text += "<VatGroups>\r\n                             <row>";
			if (type == EnumCommandType.Add)
			{
				text = text + "<Code>" + Function.ToString(model.Code) + "</Code>";
			}
			if (!string.IsNullOrEmpty(model.Name))
			{
				text = text + "<Name>" + model.Name + "</Name>";
			}
			if (!string.IsNullOrEmpty(model.AcquisitionTax))
			{
				text = text + "<AcquisitionTax>" + model.AcquisitionTax + "</AcquisitionTax>";
			}
			text = ((!string.IsNullOrEmpty(model.Category)) ? (text + (model.Category.Equals("I") ? "<Category>bovcInputTax</Category>" : "<Category>bovcOutputTax</Category>")) : (text + "<Category>bovcOutputTax</Category>"));
			if (!string.IsNullOrEmpty(model.TaxAccount))
			{
				text = text + "<TaxAccount>" + model.TaxAccount + "</TaxAccount>";
			}
			text = ((!(Function.ToString(model.EU) == "Y")) ? (text + "<EU>tNO</EU>") : (text + "<EU>tYES</EU>"));
			if (!string.IsNullOrEmpty(model.TriangularDeal))
			{
				text = text + "<TriangularDeal>" + model.TriangularDeal + "</TriangularDeal>";
			}
			text = ((!(Function.ToString(model.AcquisitionReverse) == "Y")) ? (text + "<AcquisitionReverse>tNO</AcquisitionReverse>") : (text + "<AcquisitionReverse>tYES</AcquisitionReverse>"));
			if (model.NonDeduct > 0.0)
			{
				text = text + "<NonDeduct>" + model.NonDeduct + "</NonDeduct>";
			}
			if (!string.IsNullOrEmpty(model.NonDeductAcc))
			{
				text = text + "<NonDeductAcc>" + model.NonDeductAcc + "</NonDeductAcc>";
			}
			if (!string.IsNullOrEmpty(model.AcquisitionTax))
			{
				text = text + "<AcquisitionTax>" + model.AcquisitionTax + "</AcquisitionTax>";
			}
			if (!string.IsNullOrEmpty(model.DeferredTaxAcc))
			{
				text = text + "<DeferredTaxAcc>" + model.DeferredTaxAcc + "</DeferredTaxAcc>";
			}
			if (!string.IsNullOrEmpty(model.GoodsShipment))
			{
				text = text + "<GoodsShipment>" + model.GoodsShipment + "</GoodsShipment>";
			}
			text = ((!(Function.ToString(model.Correction) == "Y")) ? (text + "<Correction>tNO</Correction>") : (text + "<Correction>tYES</Correction>"));
			if (!string.IsNullOrEmpty(model.VatCorrection))
			{
				text = text + "<VatCorrection>" + model.VatCorrection + "</VatCorrection>";
			}
			if (!string.IsNullOrEmpty(model.EqualizationTaxAccount))
			{
				text = text + "<EqualizationTaxAccount>" + model.EqualizationTaxAccount + "</EqualizationTaxAccount>";
			}
			if (!string.IsNullOrEmpty(model.ServiceSupply))
			{
				text = text + "<ServiceSupply>" + model.ServiceSupply + "</ServiceSupply>";
			}
			text = ((!(Function.ToString(model.Inactive) == "Y")) ? (text + "<Inactive>tNO</Inactive>") : (text + "<Inactive>tYES</Inactive>"));
			if (!string.IsNullOrEmpty(model.TaxTypeBlackList))
			{
				text = text + "<TaxTypeBlackList>" + model.TaxTypeBlackList + "</TaxTypeBlackList>";
			}
			if (!string.IsNullOrEmpty(model.Report349Code))
			{
				text = text + "<Report349Code>" + model.Report349Code + "</Report349Code>";
			}
			if (!string.IsNullOrEmpty(model.VATInRevenueAccount))
			{
				text = text + "<VATInRevenueAccount>" + model.VATInRevenueAccount + "</VATInRevenueAccount>";
			}
			if (!string.IsNullOrEmpty(model.CashDiscountAccount))
			{
				text = text + "<CashDiscountAccount>" + model.CashDiscountAccount + "</CashDiscountAccount>";
			}
			if (!string.IsNullOrEmpty(model.DownPaymentTaxOffsetAccount))
			{
				text = text + "<DownPaymentTaxOffsetAccount>" + model.DownPaymentTaxOffsetAccount + "</DownPaymentTaxOffsetAccount>";
			}
			if (!string.IsNullOrEmpty(model.VATDeductibleAccount))
			{
				text = text + "<VATDeductibleAccount>" + model.VATDeductibleAccount + "</VATDeductibleAccount>";
			}
			text += DataService.MappingUdf(model.UserFields);
			text += "</row>\r\n                        </VatGroups>";
			if (model.VatGroups_Lines != null && model.VatGroups_Lines.Count > 0)
			{
				text += "<VatGroups_Lines>";
				for (int i = 0; i < model.VatGroups_Lines.Count; i++)
				{
					text += "<row>";
					VatGroups_LinesImpl vatGroups_LinesImpl = model.VatGroups_Lines[i];
					if (!vatGroups_LinesImpl.Rate.Equals(0.0))
					{
						text = text + "<Rate>" + (decimal)Function.ParseDouble(vatGroups_LinesImpl.Rate) + "</Rate>";
					}
					_ = vatGroups_LinesImpl.EffecDate;
					if (vatGroups_LinesImpl.EffecDate > new DateTime(1900, 1, 1))
					{
						text = text + "<Effectivefrom>" + Convert.ToDateTime(vatGroups_LinesImpl.EffecDate).ToString("yyyyMMdd") + "</Effectivefrom>";
					}
					if (!vatGroups_LinesImpl.EqualizationTax.Equals(0.0))
					{
						text = text + "<EqualizationTax>" + (decimal)Function.ParseDouble(vatGroups_LinesImpl.EqualizationTax) + "</EqualizationTax>";
					}
					if (!vatGroups_LinesImpl.DatevCode.Equals(0))
					{
						text = text + "<DatevCode>" + Function.ParseInt(vatGroups_LinesImpl.DatevCode) + "</DatevCode>";
					}
					text += DataService.MappingUdf(vatGroups_LinesImpl.UserFields);
					text += "</row>";
				}
				text += "</VatGroups_Lines>";
			}
			return text + "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public virtual string Delete(string code, string sessionId)
	{
		try
		{
			string text = string.Format("<dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                        <BOM>\r\n                          <BO>\r\n                            <AdmInfo>\r\n                                <Object>oVatGroups</Object>\r\n                            </AdmInfo>\r\n                            <QueryParams><Code>" + Function.RawText(code) + "</Code></QueryParams>\r\n                            </BO>\r\n                        </BOM>\r\n                        </dis:RemoveObject>");
			text = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                             <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n                             </env:Header>\r\n                             <env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                              " + text + "\r\n                             </env:Body>\r\n                            </env:Envelope>";
			if (!(DataService.Process(text).Content is XmlElement xmlElement))
			{
				return "Can not get response from Service";
			}
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
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
