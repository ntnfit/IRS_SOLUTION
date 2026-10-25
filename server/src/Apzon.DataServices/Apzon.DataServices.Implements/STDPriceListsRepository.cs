using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class STDPriceListsRepository : Repository<DataTable>, ISTDPriceLists, IDisposable
{
	protected IUnitOfWork _unitOfWork;

	public STDPriceListsRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService, IUnitOfWork dataUnitOfWork)
		: base(unitOfWork, dataService)
	{
		_unitOfWork = dataUnitOfWork;
	}

	public virtual string Save(DataTable dt, string sessionId)
	{
		try
		{
			string text = "";
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string text2 = "";
				text2 = (Exist(Function.ParseInt(dt.Rows[i]["ListNum"]), sessionId) ? UpdateData(dt.Rows[i], sessionId) : AddNew(dt.Rows[i], sessionId));
				if (!string.IsNullOrEmpty(text2))
				{
					if (string.IsNullOrEmpty(text))
					{
						text = "Update price list - ";
					}
					text = text + "; ListNum[" + Function.ToString(dt.Rows[i]["ListNum"]) + "] error: " + text2;
					break;
				}
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private bool Exist(int listNum, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n\r\n                             <env:Header>\r\n\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n\r\n                             </env:Header>\r\n\r\n                             <env:Body>\r\n\r\n                              <dis:GetByKey xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n\r\n                               <Object>oPriceLists</Object> \r\n\r\n                               <PriceListNo>" + listNum + "</PriceListNo> \r\n\r\n                              </dis:GetByKey>\r\n\r\n                             </env:Body>\r\n\r\n                            </env:Envelope>";
			XmlElement xmlElement = DataService.GetByKey(sessionId, xmlObject).Content as XmlElement;
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("PriceListNo"))
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
		return false;
	}

	private string UpdateData(DataRow row, string sessionId)
	{
		try
		{
			string xmlObject = Mapping(row, EnumCommandType.Update, sessionId);
			XmlElement xmlElement = DataService.UpdateObject(ApzObjectTypes.oPriceLists, sessionId, xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"update price list: listNum - {num}");
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string AddNew(DataRow row, string sessionId)
	{
		try
		{
			string xmlObject = Mapping(row, EnumCommandType.Add, sessionId);
			XmlElement xmlElement = DataService.AddObject(ApzObjectTypes.oPriceLists, sessionId, xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", $"Add new price list - {num}");
					return string.Empty;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	public virtual string Delete(int listNum, string sessionId)
	{
		try
		{
			string xmlObject = "<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <env:Header>\r\n                                            <SessionID>" + sessionId + "</SessionID>\r\n                                        </env:Header>\r\n                                        <env:Body>\r\n                                            <dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                                <BOM>\r\n                                                    <BO>\r\n                                                        <AdmInfo>\r\n                                                            <Object>oPriceLists</Object>\r\n                                                        </AdmInfo>\r\n                                                        <QueryParams>\r\n                                                            <PriceListNo>" + listNum + "</PriceListNo>\r\n                                                        </QueryParams>\r\n                                                    </BO>\r\n                                                </BOM>\r\n                                            </dis:RemoveObject>\r\n                                        </env:Body>\r\n                                    </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("WATCH", string.Format("Delete PriceList Error \t" + xElement2.Value));
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Delete PriceList\t{arg}");
					return string.Empty;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return string.Empty;
	}

	public string Mapping(DataRow row, EnumCommandType commandType, string sessionID)
	{
		string text = "<BOM>\r\n                          <BO>\r\n                          <AdmInfo>\r\n                            <Object>oPriceLists</Object>\r\n                            <Version>2</Version>\r\n                          </AdmInfo>";
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				text = text + "<QueryParams><PriceListNo>" + Function.ParseInt(row["ListNum"]) + "</PriceListNo></QueryParams>";
			}
			text += " <PriceLists>\r\n                            <row>";
			BoYesNoEnum boYesNoEnum;
			if (!string.IsNullOrEmpty(Function.ToString(row["ValidFor"])))
			{
				if (row["ValidFor"].ToString() == "Y")
				{
					boYesNoEnum = BoYesNoEnum.tYES;
					DateTime? dateTime = Function.ParseDateTime(row["ValidFrom"]);
					if (dateTime.HasValue)
					{
						_ = dateTime.Value;
						if (dateTime.Value.Year > 1990)
						{
							text = text + "<ValidFrom>" + Function.RawText(dateTime.Value.ToString("yyyyMMdd")) + "</ValidFrom>";
							goto IL_00f1;
						}
					}
					text += "<ValidFrom/>";
					goto IL_00f1;
				}
				boYesNoEnum = BoYesNoEnum.tNO;
				goto IL_016d;
			}
			goto IL_018b;
			IL_00f1:
			DateTime? dateTime2 = Function.ParseDateTime(row["ValidTo"]);
			if (dateTime2.HasValue)
			{
				_ = dateTime2.Value;
				if (dateTime2.Value.Year > 1990)
				{
					text = text + "<ValidTo>" + Function.RawText(dateTime2.Value.ToString("yyyyMMdd")) + "</ValidTo>";
					goto IL_016d;
				}
			}
			text += "<ValidTo/>";
			goto IL_016d;
			IL_018b:
			if (row["BASE_NUM"] != null && row["BASE_NUM"] != DBNull.Value && Exist(Function.ParseInt(row["BASE_NUM"]), sessionID))
			{
				text = text + "<BasePriceList>" + Function.RawText(row["BASE_NUM"]) + "</BasePriceList>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["AddCurr1"])))
			{
				text = text + "<DefaultAdditionalCurrency1>" + Function.RawText(row["AddCurr1"]) + "</DefaultAdditionalCurrency1>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["AddCurr2"])))
			{
				text = text + "<DefaultAdditionalCurrency2>" + Function.RawText(row["AddCurr2"]) + "</DefaultAdditionalCurrency2>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["PrimCurr"])))
			{
				text = text + "<DefaultPrimeCurrency>" + Function.RawText(row["PrimCurr"]) + "</DefaultPrimeCurrency>";
			}
			if (Function.ParseDouble(row["Factor"]) > 0.0)
			{
				text = text + "<Factor>" + Function.ParseInt(row["Factor"]) + "</Factor>";
			}
			if (Function.ParseDouble(row["ExtAmount"]) > 0.0)
			{
				text = text + "<FixedAmount>" + Function.ParseDecimal(row["ExtAmount"]) + "</FixedAmount>";
			}
			if (Function.ParseInt(row["GroupCode"]) > 0)
			{
				text = text + "<GroupNum>" + (Function.ParseInt(row["GroupCode"]) switch
				{
					1 => BoPriceListGroupNum.boplgn_Group1, 
					2 => BoPriceListGroupNum.boplgn_Group2, 
					3 => BoPriceListGroupNum.boplgn_Group3, 
					4 => BoPriceListGroupNum.boplgn_Group4, 
					_ => BoPriceListGroupNum.boplgn_Group1, 
				}).ToString() + "</GroupNum>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["PrimCurr"])))
			{
				text = text + "<IsGrossPrice>" + ((row["IsGrossPrc"].ToString() == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO).ToString() + "</IsGrossPrice>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["ListName"])))
			{
				text = text + "<PriceListName>" + Function.RawText(row["ListName"]) + "</PriceListName>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["RndFrmtDec"])))
			{
				text = text + "<RoundingFormatDecimalPart>" + Function.RawText(row["RndFrmtDec"]) + "</RoundingFormatDecimalPart>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["RndFrmtInt"])))
			{
				text = text + "<RoundingFormatIntegerPart>" + Function.RawText(row["RndFrmtInt"]) + "</RoundingFormatIntegerPart>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["RoundSys"])))
			{
				text = text + "<RoundingMethod>" + (Function.ParseInt(row["RoundSys"]) switch
				{
					0 => BoRoundingMethod.borm_NoRounding, 
					1 => BoRoundingMethod.borm_RoundToFullDecAmount, 
					2 => BoRoundingMethod.borm_RoundToFullAmount, 
					3 => BoRoundingMethod.borm_RoundToFullTensAmount, 
					4 => BoRoundingMethod.borm_FixedEnding, 
					5 => BoRoundingMethod.borm_FixedInterval, 
					_ => BoRoundingMethod.borm_NoRounding, 
				}).ToString() + "</RoundingMethod>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["RoundRule"])))
			{
				text = text + "<RoundingRule>" + Function.RawText(row["RoundRule"]) + "</RoundingRule>";
			}
			text += DataService.MappingUdf(row);
			text += "</row>\r\n                        </PriceLists>";
			text += "</BO></BOM>";
			goto end_IL_0006;
			IL_016d:
			text = text + "<Active>" + boYesNoEnum.ToString() + "</Active>";
			goto IL_018b;
			end_IL_0006:;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public virtual string SaveItemPrice(DataTable dt, string sessionId)
	{
		try
		{
			string text = "";
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string text2 = UpdateDataItemPrice(dt.Rows[i], sessionId);
				if (!string.IsNullOrEmpty(text2))
				{
					if (text == "")
					{
						text = "SaveItemPrice - ";
					}
					text = text + ";item[" + Function.ToString(dt.Rows[i]["ItemCode"]) + "]: " + text2;
					break;
				}
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public string UpdatePriceFromBOM(string itemCode, int priceList, double price, string currency, string sessionId)
	{
		try
		{
			string byParams = GetByParams(sessionId, itemCode);
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[UpdateDataItemPrice].[GetByPr] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Item"));
			if (!string.IsNullOrEmpty(Function.ToString(xElement)))
			{
				string text = ProcessPriceFromBOM(xElement, itemCode, priceList, price, currency);
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Replace("xmlns=\"\"", "");
					string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ItemsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"ItemsService\">" + Function.ToString(text) + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
					XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
					XElement xElement2 = XElement.Parse(xmlElement.OuterXml);
					if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
					{
						XElement xElement3 = xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
						if (xElement3 != null)
						{
							throw new Exception(xElement3.Value);
						}
					}
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string ProcessPriceFromBOM(XElement itemXml1, string itemCode, int priceList, double price, string currency)
	{
		try
		{
			XElement xElement = new XElement(itemXml1);
			IEnumerable<XElement> enumerable = from t in xElement.Descendants()
				where t.Name.LocalName.Equals("ItemPrice")
				select t;
			if (enumerable != null)
			{
				int num = -1;
				for (int num2 = 0; num2 < enumerable.Count(); num2++)
				{
					XElement xElement2 = enumerable.ElementAt(num2).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("PriceList"));
					if (xElement2 != null && Function.ParseInt(xElement2.Value) == priceList)
					{
						num = num2;
						break;
					}
				}
				if (num != -1)
				{
					XElement xElement3 = enumerable.ElementAt(num);
					if (!xElement3.Descendants().Any((XElement t) => t.Name.LocalName.Equals("Price")))
					{
						xElement3.Add(XElement.Parse("<Price />"));
					}
					xElement3.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Price")).SetValue(Function.ToString(price));
					if (!xElement3.Descendants().Any((XElement t) => t.Name.LocalName.Equals("Currency")))
					{
						xElement3.Add(XElement.Parse("<Currency />"));
					}
					xElement3.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Currency")).SetValue(currency);
					return Function.ToString(xElement);
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	private string UpdateDataItemPrice(DataRow row, string sessionId)
	{
		try
		{
			string byParams = GetByParams(sessionId, Function.ToString(row["ItemCode"]));
			if (string.IsNullOrEmpty(byParams))
			{
				throw new Exception("[UpdateDataItemPrice].[GetByPr] No matching record found.");
			}
			XElement xElement = XElement.Parse(byParams).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Item"));
			if (!string.IsNullOrEmpty(Function.ToString(xElement)))
			{
				string text = ProcessItemPrices(xElement, row);
				if (!string.IsNullOrEmpty(text))
				{
					text = text.Replace("xmlns=\"\"", "");
					string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ItemsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"ItemsService\">" + Function.ToString(text) + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
					XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
					XElement xElement2 = XElement.Parse(xmlElement.OuterXml);
					if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
					{
						XElement xElement3 = xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
						if (xElement3 != null)
						{
							throw new Exception(xElement3.Value);
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
		return string.Empty;
	}

	private string ProcessItemPrices(XElement itemXml, DataRow row)
	{
		try
		{
			IEnumerable<XElement> enumerable = from t in itemXml.Descendants()
				where t.Name.LocalName.Equals("ItemPrice")
				select t;
			if (enumerable != null)
			{
				int num = -1;
				for (int num2 = 0; num2 < enumerable.Count(); num2++)
				{
					XElement xElement = enumerable.ElementAt(num2).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("PriceList"));
					if (xElement != null && Function.ParseInt(xElement.Value) == Function.ParseInt(row["PriceList"]))
					{
						num = num2;
						break;
					}
				}
				if (num != -1)
				{
					XElement xElement2 = enumerable.ElementAt(num);
					if (!xElement2.Descendants().Any((XElement t) => t.Name.LocalName.Equals("Price")))
					{
						xElement2.Add(XElement.Parse("<Price />"));
					}
					xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Price")).SetValue(Function.ToString(row["Price"]));
					if (!xElement2.Descendants().Any((XElement t) => t.Name.LocalName.Equals("Currency")))
					{
						xElement2.Add(XElement.Parse("<Currency />"));
					}
					xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Currency")).SetValue(Function.ToString(row["Currency"]));
					if (!xElement2.Descendants().Any((XElement t) => t.Name.LocalName.Equals("Factor")))
					{
						xElement2.Add(XElement.Parse("<Factor />"));
					}
					xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Factor")).SetValue(Function.ToString(row["Factor"]));
					if (!xElement2.Descendants().Any((XElement t) => t.Name.LocalName.Equals("BasePriceList")))
					{
						xElement2.Add(XElement.Parse("<BasePriceList />"));
					}
					xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("BasePriceList")).SetValue(Function.ToString(row["BasePLNum"]));
					if (Function.ParseDouble(row["AddPrice1"]) > 0.0)
					{
						if (!xElement2.Descendants().Any((XElement t) => t.Name.LocalName.Equals("AdditionalPrice1")))
						{
							xElement2.Add(XElement.Parse("<AdditionalPrice1 />"));
						}
						xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("AdditionalPrice1")).SetValue(Function.ToString(row["AddPrice1"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(row["Currency1"])))
					{
						if (!xElement2.Descendants().Any((XElement t) => t.Name.LocalName.Equals("AdditionalCurrency1")))
						{
							xElement2.Add(XElement.Parse("<AdditionalCurrency1 />"));
						}
						xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("AdditionalCurrency1")).SetValue(Function.ToString(row["Currency1"]));
					}
					if (Function.ParseDouble(row["AddPrice2"]) > 0.0)
					{
						if (!xElement2.Descendants().Any((XElement t) => t.Name.LocalName.Equals("AdditionalPrice2")))
						{
							xElement2.Add(XElement.Parse("<AdditionalPrice2 />"));
						}
						xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("AdditionalPrice2")).SetValue(Function.ToString(row["AddPrice2"]));
					}
					if (!string.IsNullOrEmpty(Function.ToString(row["Currency2"])))
					{
						if (!xElement2.Descendants().Any((XElement t) => t.Name.LocalName.Equals("AdditionalCurrency2")))
						{
							xElement2.Add(XElement.Parse("<AdditionalCurrency2 />"));
						}
						xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("AdditionalCurrency2")).SetValue(Function.ToString(row["Currency2"]));
					}
					return Function.ToString(itemXml);
				}
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public string GetByParams(string sessionId, string itemCode)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>ItemsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetByParams xmlns=\"ItemsService\">\r\n                                     <Item>   <ItemCode>" + itemCode + "</ItemCode></Item>\r\n                                    </GetByParams>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					throw new Exception(xElement2.Value);
				}
			}
			return xmlElement.OuterXml;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public void Dispose()
	{
		Dispose();
	}
}
