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
using Apzon.Entities.Models.Items;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class SpecialPricesRepository : Repository<SpecialBP_Impl>, Apzon.DataServices.Interfaces.ISpecialPrices, IRepository<SpecialBP_Impl>, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public SpecialPricesRepository(Apzon.Services.IUnitOfWork serviUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public void Dispose()
	{
	}

	public virtual string SaveDoc(DataSet docs, string sessionId)
	{
		try
		{
			sessionId = GetSessionId(sessionId);
			if (docs == null || docs.Tables.Count != 4 || !docs.Tables[0].IsNotNull())
			{
				return "Data not provide";
			}
			string cardCode = Function.ToString(docs.Tables[0].Rows[0]["CardCode"]);
			DataTable currentData = GetCurrentData(cardCode);
			bool flag = Function.ToString(docs.Tables[0].Rows[0]["IsSpecial"]).Equals("Y");
			string text = string.Empty;
			string text2 = "";
			if (currentData.IsNotNull())
			{
				List<string> lstItemCode = (docs.Tables[1].IsNotNull() ? (from t in docs.Tables[1].AsEnumerable()
					select Function.ToString(t["ItemCode"])).ToList() : new List<string>());
				List<DataRow> list = (from t in currentData.AsEnumerable()
					where !lstItemCode.Contains(Function.ToString(t["ItemCode"]))
					select t).ToList();
				if (list.Any())
				{
					foreach (DataRow item in list)
					{
						_ = item;
						text += MappingDelete(Function.ToString(currentData.Rows[0]["ItemCode"]), cardCode, sessionId);
					}
				}
			}
			if (docs.Tables[1].IsNotNull())
			{
				if (!flag)
				{
					cardCode = "";
				}
				List<DataRow> lstUpdate = (from t in docs.Tables[1].AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["ItemCode"])) && Function.ToString(t["IsUpdate"]).Equals("Y")
					select t).ToList();
				int i;
				for (i = 0; i < lstUpdate.Count; i++)
				{
					text2 = ((!currentData.IsNotNull() || !currentData.AsEnumerable().Any((DataRow t) => Function.ToString(t["ItemCode"]).Equals(Function.ToString(lstUpdate[i]["ItemCode"])))) ? ("<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <env:Header>\r\n                                            <SessionID>" + sessionId + "</SessionID>\r\n                                        </env:Header><env:Body>\r\n                                        <dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXml(docs, lstUpdate[i], flag, EnumCommandType.Add) + "</dis:AddObject></env:Body></env:Envelope>") : ("<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <env:Header>\r\n                                            <SessionID>" + sessionId + "</SessionID>\r\n                                        </env:Header><env:Body>\r\n                                        <dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXml(docs, lstUpdate[i], flag, EnumCommandType.Update) + "</dis:UpdateObject></env:Body></env:Envelope>"));
					text += text2;
				}
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Envelopes>";
			XmlElement xmlElement = DataService.BatchProcess(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Save SpecialPrices Error ", xElement2.Value);
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				List<XElement> list2 = (from t in xElement.Descendants()
					where t.Name.LocalName.Equals("RetKey")
					select t).ToList();
				if (list2.Count > 0)
				{
					List<string> list3 = new List<string>();
					list3.AddRange(list2.Select((XElement t) => Function.ToString(t.Value)));
					if (list3.Count > 0)
					{
						Logging.Write("WATCH", "Save SpecialPrices Success ", string.Join(",", list3.ToArray()));
						return "";
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

	protected virtual string MappingDelete(string itemCode, string cardCode, string sessionId)
	{
		return "<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                 <env:Header>\r\n                                  <SessionID>" + sessionId + "</SessionID>\r\n                                 </env:Header>\r\n                                 <env:Body>\r\n                                  <dis:RemoveObject  xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>" + Function.ToString(ApzObjectTypes.oSpecialPrices) + "</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <ItemCode>" + itemCode + "</ItemCode>\r\n                                     <CardCode>" + cardCode + "</CardCode>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                    </dis:RemoveObject >\r\n                                 </env:Body>\r\n                                </env:Envelope>";
	}

	public virtual string Delete(string itemCode, string cardCode, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                 <env:Header>\r\n                                  <SessionID>" + sessionId + "</SessionID>\r\n                                 </env:Header>\r\n                                 <env:Body>\r\n                                  <dis:RemoveObject  xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                  <BOM>\r\n                                   <BO>\r\n                                    <AdmInfo>\r\n                                     <Object>" + Function.ToString(ApzObjectTypes.oSpecialPrices) + "</Object>\r\n                                    </AdmInfo>\r\n                                    <QueryParams>\r\n                                     <ItemCode>" + itemCode + "</ItemCode>\r\n                                     <CardCode>" + cardCode + "</CardCode>\r\n                                    </QueryParams>\r\n                                   </BO>\r\n                                  </BOM>\r\n                                  </dis:RemoveObject >\r\n                                 </env:Body>\r\n                                </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", string.Format("Remove SpecialPrices Error", xElement2.Value));
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					string value = xElement3.Value;
					Logging.Write("WATCH", string.Format("Remove SpecialPrices Success", value));
					return "";
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

	protected DataTable GetCurrentData(string cardCode)
	{
		try
		{
			string query = "SELECT \"CardCode\", \"ItemCode\" FROM OSPP WHERE \"CardCode\" = @cardCode;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@cardCode", cardCode));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	private string MappingXml(DataSet model, DataRow rowData, bool isSpecialPrice, EnumCommandType type)
	{
		try
		{
			string obj = Function.ToString(rowData["CardCode"]);
			if (isSpecialPrice)
			{
				obj = "";
			}
			string itemCode = Function.ToString(rowData["ItemCode"]);
			string text = "<BOM>\r\n                              <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                              <AdmInfo>\r\n                                <Object>oSpecialPrices</Object>\r\n                              </AdmInfo>";
			if (type == EnumCommandType.Update)
			{
				text = text + "<QueryParams>\r\n                                <ItemCode>" + Function.RawText(rowData["ItemCode"]) + "</ItemCode>\r\n                                <CardCode>" + Function.RawText(obj) + "</CardCode>\r\n                            </QueryParams>";
			}
			text += "<SpecialPrices>\r\n                             <row>";
			if (type == EnumCommandType.Add)
			{
				text = text + "<ItemCode>" + Function.RawText(rowData["ItemCode"]) + "</ItemCode>\r\n                             <CardCode>" + Function.RawText(obj) + "</CardCode>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["Currency"])))
			{
				text = text + "<Currency>" + Function.RawText(rowData["Currency"]) + "</Currency>";
			}
			if (Function.ParseDouble(rowData["Discount"]) > 0.0)
			{
				text = text + "<DiscountPercent>" + Function.ParseDouble(rowData["Discount"]) + "</DiscountPercent>";
			}
			if (Function.ParseInt(rowData["ListNum"]) > 0)
			{
				text = text + "<PriceListNum>" + Function.ParseInt(rowData["ListNum"]) + "</PriceListNum>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["AutoUpdt"])))
			{
				text = text + "<AutoUpdate>" + (Function.ToString(rowData["AutoUpdt"]).Equals("Y") ? BoYesNoEnum.tYES.ToString() : BoYesNoEnum.tNO.ToString()) + "</AutoUpdate>";
			}
			if (Function.ParseInt(rowData["SrcPrice"]) >= 0)
			{
				switch (Function.ParseInt(rowData["SrcPrice"]))
				{
				case 1:
					text = text + "<SourcePrice>" + SourceCurrencyEnum.sc_AdditionalCurrency1.ToString() + "</SourcePrice>";
					if (Function.ParseDouble(rowData["AddPrice1"]) > 0.0)
					{
						text = text + "<Price>" + Function.ParseDouble(rowData["AddPrice1"]) + "</Price>";
					}
					break;
				case 2:
					text = text + "<SourcePrice>" + SourceCurrencyEnum.sc_AdditionalCurrency2.ToString() + "</SourcePrice>";
					if (Function.ParseDouble(rowData["AddPrice2"]) > 0.0)
					{
						text = text + "<Price>" + Function.ParseDouble(rowData["AddPrice2"]) + "</Price>";
					}
					break;
				default:
					text = text + "<SourcePrice>" + SourceCurrencyEnum.sc_PrimaryCurrency.ToString() + "</SourcePrice>";
					if (Function.ParseDouble(rowData["Price"]) > 0.0)
					{
						text = text + "<Price>" + Function.ParseDouble(rowData["Price"]) + "</Price>";
					}
					break;
				}
			}
			if (!string.IsNullOrEmpty(Function.ToString(rowData["Valid"])))
			{
				text = text + "<AutoUpdate>" + (Function.ToString(rowData["Valid"]).Equals("Y") ? BoYesNoEnum.tYES.ToString() : BoYesNoEnum.tNO.ToString()) + "</AutoUpdate>";
			}
			if (Function.ParseDateTime(rowData["ValidFrom"]).HasValue)
			{
				text = text + "<ValidFrom>" + Function.ParseDateTimes(rowData["ValidFrom"]).ToString("yyyy-MM-dd") + "</ValidFrom>";
			}
			if (Function.ParseDateTime(rowData["ValidTo"]).HasValue)
			{
				text = text + "<ValidTo>" + Function.ParseDateTimes(rowData["ValidTo"]).ToString("yyyy-MM-dd") + "</ValidTo>";
			}
			List<string> list = (from DataColumn c in model.Tables[1].Columns
				select c.ColumnName into t
				where t.StartsWith("U_")
				select t).ToList();
			if (list.Any())
			{
				List<UserFieldsImpl> list2 = new List<UserFieldsImpl>();
				for (int num = 0; num < list.Count; num++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					if (model.Tables[1].Columns[list[num]].DataType == typeof(DateTime))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(rowData[list[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (model.Tables[1].Columns[list[num]].DataType == typeof(double))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDecimal(rowData[list[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Value = Function.ToString(Function.ToString(rowData[list[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					userFieldsImpl.Name = list[num];
					list2.Add(userFieldsImpl);
				}
				text += DataService.MappingUdf(list2);
			}
			text += "</row>\r\n                        </SpecialPrices>";
			text += "<SpecialPricesDataAreas>";
			if (model.Tables[2].IsNotNull())
			{
				List<DataRow> list3 = (from t in model.Tables[2].AsEnumerable()
					where Function.ToString(t["ItemCode"]).Equals(itemCode)
					select t).ToList();
				if (list3.Any())
				{
					for (int num2 = 0; num2 < list3.Count; num2++)
					{
						int lineNum = Function.ParseInt(list3[num2]["LINENUM"]);
						text += "<row>";
						text = text + "<RowNumber>" + lineNum + "</RowNumber>";
						if (!string.IsNullOrEmpty(Function.ToString(list3[num2]["AutoUpdt"])))
						{
							text = text + "<AutoUpdate>" + (Function.ToString(list3[num2]["AutoUpdt"]).Equals("Y") ? BoYesNoEnum.tYES.ToString() : BoYesNoEnum.tNO.ToString()) + "</AutoUpdate>";
						}
						if (Function.ParseDouble(list3[num2]["Discount"]) > 0.0)
						{
							text = text + "<Discount>" + Function.ParseDouble(list3[num2]["Discount"]) + "</Discount>";
						}
						if (Function.ParseDouble(list3[num2]["Price"]) > 0.0)
						{
							text = text + "<SpecialPrice>" + Function.ParseDouble(list3[num2]["Price"]) + "</SpecialPrice>";
						}
						if (!string.IsNullOrEmpty(Function.ToString(list3[num2]["Currency"])))
						{
							text = text + "<PriceCurrency>" + Function.ToString(list3[num2]["Currency"]) + "</PriceCurrency>";
						}
						if (Function.ParseDateTime(list3[num2]["FromDate"]).HasValue)
						{
							text = text + "<DateFrom>" + Function.ParseDateTimes(list3[num2]["FromDate"]).ToString("yyyy-MM-dd") + "</DateFrom>";
						}
						if (Function.ParseDateTime(list3[num2]["ToDate"]).HasValue)
						{
							text = text + "<Dateto>" + Function.ParseDateTimes(list3[num2]["ToDate"]).ToString("yyyy-MM-dd") + "</Dateto>";
						}
						if (Function.ParseInt(list3[num2]["ListNum"]) > 0)
						{
							text = text + "<PriceListNo>" + Function.ParseInt(list3[num2]["ListNum"]) + "</PriceListNo>";
						}
						List<string> list4 = (from DataColumn c in model.Tables[2].Columns
							select c.ColumnName into t
							where t.StartsWith("U_")
							select t).ToList();
						if (list4.Any())
						{
							List<UserFieldsImpl> list5 = new List<UserFieldsImpl>();
							for (int num3 = 0; num3 < list4.Count; num3++)
							{
								UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
								if (model.Tables[2].Columns[list4[num3]].DataType == typeof(DateTime))
								{
									userFieldsImpl2.Value = Function.ToString(Function.ParseDateTime(list3[num2][list4[num3]]));
									userFieldsImpl2.Type = BoFieldTypes.db_Date;
								}
								else if (model.Tables[2].Columns[list4[num3]].DataType == typeof(double))
								{
									userFieldsImpl2.Value = Function.ToString(Function.ParseDecimal(list3[num2][list4[num3]]));
									userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
								}
								else
								{
									userFieldsImpl2.Value = Function.ToString(Function.ToString(list3[num2][list4[num3]]));
									userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
								}
								userFieldsImpl2.Name = list4[num3];
								list5.Add(userFieldsImpl2);
							}
							text += DataService.MappingUdf(list5);
						}
						text += "<SpecialPricesQuantityAreas>";
						if (model.Tables[3].IsNotNull())
						{
							List<DataRow> list6 = (from t in model.Tables[3].AsEnumerable()
								where Function.ToString(t["ItemCode"]).Equals(itemCode) && Function.ParseInt(t["SPP1LNum"]) == lineNum
								select t).ToList();
							if (list6.Any())
							{
								for (int num4 = 0; num4 < list6.Count; num4++)
								{
									text += "<row>";
									text = text + "<RowNumber>" + Function.ParseInt(list6[num4]["SPP1LNum"]) + "</RowNumber>";
									if (Function.ParseDouble(list6[num4]["Discount"]) > 0.0)
									{
										text = text + "<Discount>" + Function.ParseDouble(list6[num4]["Discount"]) + "</Discount>";
									}
									if (Function.ParseDouble(list6[num4]["Price"]) > 0.0)
									{
										text = text + "<SpecialPrice>" + Function.ParseDouble(list6[num4]["Price"]) + "</SpecialPrice>";
									}
									if (!string.IsNullOrEmpty(Function.ToString(list6[num4]["Currency"])))
									{
										text = text + "<PriceCurrency>" + Function.ToString(list6[num4]["Currency"]) + "</PriceCurrency>";
									}
									if (Function.ParseDouble(list6[num4]["Amount"]) > 0.0)
									{
										text = text + "<Quantity>" + Function.ParseDouble(list6[num4]["Amount"]) + "</Quantity>";
									}
									List<string> list7 = (from DataColumn c in model.Tables[3].Columns
										select c.ColumnName into t
										where t.StartsWith("U_")
										select t).ToList();
									if (list7.Any())
									{
										List<UserFieldsImpl> list8 = new List<UserFieldsImpl>();
										for (int num5 = 0; num5 < list7.Count; num5++)
										{
											UserFieldsImpl userFieldsImpl3 = new UserFieldsImpl();
											if (model.Tables[3].Columns[list7[num5]].DataType == typeof(DateTime))
											{
												userFieldsImpl3.Value = Function.ToString(Function.ParseDateTime(list6[num4][list7[num5]]));
												userFieldsImpl3.Type = BoFieldTypes.db_Date;
											}
											else if (model.Tables[3].Columns[list7[num5]].DataType == typeof(double))
											{
												userFieldsImpl3.Value = Function.ToString(Function.ParseDecimal(list6[num4][list7[num5]]));
												userFieldsImpl3.Type = BoFieldTypes.db_Numeric;
											}
											else
											{
												userFieldsImpl3.Value = Function.ToString(Function.ToString(list6[num4][list7[num5]]));
												userFieldsImpl3.Type = BoFieldTypes.db_Alpha;
											}
											userFieldsImpl3.Name = list7[num5];
											list8.Add(userFieldsImpl3);
										}
										text += DataService.MappingUdf(list8);
									}
									text += "</row>";
								}
							}
						}
						text += "</SpecialPricesQuantityAreas>";
						text += "</row>";
					}
				}
			}
			text += "</SpecialPricesDataAreas>";
			return text + "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public virtual string SaveDataAreas(List<SpecialBP_LineSPP1Impl> docs, string sessionId)
	{
		try
		{
			sessionId = GetSessionId(sessionId);
			if (docs != null && docs.Count > 0)
			{
				string text = "";
				text = "<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <env:Header>\r\n                                            <SessionID>" + sessionId + "</SessionID>\r\n                                        </env:Header><env:Body>\r\n                                        <dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXmlDataArea(docs, EnumCommandType.Update) + "</dis:UpdateObject></env:Body></env:Envelope>";
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Envelopes>";
				XmlElement xmlElement = DataService.BatchProcess(xmlObject).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						Logging.Write("ERROR", "Save SpecialPrices Error ", xElement2.Value);
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
							Logging.Write("WATCH", "Save SpecialPrices Success ", string.Join(",", list2.ToArray()));
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

	private string MappingXmlDataArea(List<SpecialBP_LineSPP1Impl> docs, EnumCommandType type)
	{
		try
		{
			string text = "<BOM>\r\n                              <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                              <AdmInfo>\r\n                                <Object>oSpecialPrices</Object>\r\n                              </AdmInfo>";
			text = text + "<QueryParams>\r\n                            <ItemCode>" + Function.ToString(docs[0].ItemCode) + "</ItemCode>\r\n                            <CardCode>" + Function.ToString(docs[0].CardCode) + "</CardCode>\r\n                        </QueryParams>";
			text += "<SpecialPrices>\r\n                            <row>";
			text += "</row>\r\n                        </SpecialPrices>";
			text += "<SpecialPricesDataAreas>";
			foreach (SpecialBP_LineSPP1Impl doc in docs)
			{
				text += "<row>";
				text = text + "<RowNumber>" + doc.LINENUM + "</RowNumber>";
				if (!string.IsNullOrEmpty(doc.AutoUpdt))
				{
					text = text + "<AutoUpdate>" + (doc.AutoUpdt.StartsWith("Y") ? BoYesNoEnum.tYES.ToString() : BoYesNoEnum.tNO.ToString()) + "</AutoUpdate>";
				}
				if (doc.Discount > 0.0)
				{
					text = text + "<Discount>" + doc.Discount + "</Discount>";
				}
				if (doc.Price > 0.0)
				{
					text = text + "<SpecialPrice>" + doc.Price + "</SpecialPrice>";
				}
				if (!string.IsNullOrEmpty(doc.Currency))
				{
					text = text + "<PriceCurrency>" + doc.Currency + "</PriceCurrency>";
				}
				if (doc.FromDate.HasValue && doc.FromDate > new DateTime(1900, 1, 1))
				{
					text = text + "<DateFrom>" + Convert.ToDateTime(doc.FromDate).ToString("yyyyMMdd") + "</DateFrom>";
				}
				if (doc.ToDate.HasValue && doc.ToDate > new DateTime(1900, 1, 1))
				{
					text = text + "<Dateto>" + Convert.ToDateTime(doc.ToDate).ToString("yyyyMMdd") + "</Dateto>";
				}
				if (doc.ListNum > 0)
				{
					text = text + "<PriceListNo>" + doc.ListNum + "</PriceListNo>";
				}
				text += "</row>";
			}
			text += "</SpecialPricesDataAreas>";
			return text + "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public virtual string SaveQuantityAreas(List<SpecialBP_LineSPP2Impl> docs, string sessionId)
	{
		try
		{
			sessionId = GetSessionId(sessionId);
			docs = GetSpecialPricesQuantityAreas(docs);
			if (docs != null && docs.Count > 0)
			{
				string text = "";
				text = "<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <env:Header>\r\n                                            <SessionID>" + sessionId + "</SessionID>\r\n                                        </env:Header><env:Body>\r\n                                        <dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + MappingXmlQuantityArea(docs, EnumCommandType.Update) + "</dis:UpdateObject></env:Body></env:Envelope>";
				string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Envelopes>";
				XmlElement xmlElement = DataService.BatchProcess(xmlObject).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						Logging.Write("ERROR", "Save SpecialPrices Error ", xElement2.Value);
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
							Logging.Write("WATCH", "Save SpecialPrices Success ", string.Join(",", list2.ToArray()));
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

	private string MappingXmlQuantityArea(List<SpecialBP_LineSPP2Impl> docs, EnumCommandType type)
	{
		try
		{
			string text = "<BOM>\r\n                              <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                              <AdmInfo>\r\n                                <Object>oSpecialPrices</Object>\r\n                              </AdmInfo>";
			text = text + "<QueryParams>\r\n                            <ItemCode>" + docs[0].ItemCode + "</ItemCode>\r\n                            <CardCode>" + docs[0].CardCode + "</CardCode>\r\n                        </QueryParams>";
			text += "<SpecialPrices>\r\n                            <row>";
			text += "</row>\r\n                        </SpecialPrices>";
			text += "<SpecialPricesQuantityAreas>";
			for (int i = 0; i < docs.Count; i++)
			{
				text += "<row>";
				text = text + "<RowNumber>" + i + "</RowNumber>";
				if (docs[i].Discount > 0.0)
				{
					text = text + "<Discountin>" + docs[i].Discount + "</Discountin>";
				}
				if (docs[i].Price > 0.0)
				{
					text = text + "<SpecialPrice>" + docs[i].Price + "</SpecialPrice>";
				}
				if (!string.IsNullOrEmpty(docs[i].Currency))
				{
					text = text + "<PriceCurrency>" + docs[i].Currency + "</PriceCurrency>";
				}
				if (docs[i].Amount > 0.0)
				{
					text = text + "<Quantity>" + docs[i].Amount + "</Quantity>";
				}
				text = text + "<SPDARowNumber>" + docs[i].LINENUM + "</SPDARowNumber>";
				text += DataService.MappingUdf(docs[i].UserFields);
				text += "</row>";
			}
			text += "</SpecialPricesQuantityAreas>";
			return text + "</BO></BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	private List<SpecialBP_LineSPP2Impl> GetSpecialPricesQuantityAreas(List<SpecialBP_LineSPP2Impl> docs)
	{
		try
		{
			string cardCode = docs[0].CardCode;
			string itemCode = docs[0].ItemCode;
			int lINENUM = docs[0].LINENUM;
			string query = "SELECT * FROM SPP2\r\n                            WHERE \"CardCode\" = '" + cardCode + "' AND \"ItemCode\" = '" + itemCode + "' AND SPP1LNum != " + lINENUM + ";";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					SpecialBP_LineSPP2Impl specialBP_LineSPP2Impl = new SpecialBP_LineSPP2Impl();
					specialBP_LineSPP2Impl.ItemCode = itemCode;
					specialBP_LineSPP2Impl.CardCode = cardCode;
					specialBP_LineSPP2Impl.LINENUM = Function.ParseInt(dataTable.Rows[i]["SPP1LNum"]);
					specialBP_LineSPP2Impl.SPP2LNum = Function.ParseInt(dataTable.Rows[i]["SPP2LNum"]);
					specialBP_LineSPP2Impl.Price = Function.ParseDouble(dataTable.Rows[i]["Price"]);
					specialBP_LineSPP2Impl.Discount = Function.ParseDouble(dataTable.Rows[i]["Discount"]);
					specialBP_LineSPP2Impl.Amount = Function.ParseDouble(dataTable.Rows[i]["Amount"]);
					specialBP_LineSPP2Impl.Currency = Function.ToString(dataTable.Rows[i]["Currency"]);
					docs.Add(specialBP_LineSPP2Impl);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return docs;
	}
}
