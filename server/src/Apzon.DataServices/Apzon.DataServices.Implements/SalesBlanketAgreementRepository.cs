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
using Apzon.Entities.Models.Documents;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class SalesBlanketAgreementRepository : Repository<DocumentSBAImpl>, ISalesBlanketAgreement, IRepository<DocumentSBAImpl>, IDisposable
{
	public SalesBlanketAgreementRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public void Dispose()
	{
		Dispose();
	}

	public virtual string Save(string sessionId, DataSet doc)
	{
		try
		{
			string retxml = "";
			string empty = string.Empty;
			empty = (Exists(Function.ParseInt(doc.Tables[0].Rows[0]["AbsID"]), sessionId, ref retxml) ? Update(doc.Tables[0], doc.Tables[1], doc.Tables[2], sessionId, retxml) : Add(doc.Tables[0], doc.Tables[1], doc.Tables[2], sessionId));
			if (string.IsNullOrEmpty(empty) && doc.Tables.Count > 3 && doc.Tables[3] != null && doc.Tables[3].Rows.Count > 0)
			{
				return SaveOAT3(doc.Tables[3]);
			}
			return empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private void MappingUdfToTable(DataRow target, List<DataColumn> columns, DataRow row)
	{
		foreach (DataColumn column in columns)
		{
			if (column.DataType == typeof(double) || column.DataType == typeof(decimal))
			{
				target[$"{column.ColumnName}"] = Function.ParseDouble(row[column]);
			}
			if (column.DataType == typeof(short))
			{
				if (Function.ToString(row[column]).Length < 4)
				{
					string arg = Function.ParseInt(row[column]).ToString("0000");
					string value = $"{arg}";
					target[$"{column.ColumnName}"] = value;
				}
				else
				{
					target[$"{column.ColumnName}"] = Function.ParseInt(row[column]);
				}
			}
			if (column.DataType == typeof(int) || column.DataType == typeof(int) || column.DataType == typeof(long))
			{
				target[$"{column.ColumnName}"] = Function.ParseInt(row[column]);
			}
			if (column.DataType == typeof(DateTime) && Function.ParseDateTime(row[column]).HasValue)
			{
				target[$"{column.ColumnName}"] = Convert.ToDateTime(row[column]).ToString("yyyy-MM-dd");
			}
			if (column.DataType == typeof(string))
			{
				target[$"{column.ColumnName}"] = Function.RawText(row[column]);
			}
		}
	}

	public bool Exists(int AbsID, string sessionId, ref string retxml)
	{
		try
		{
			if (AbsID <= 0)
			{
				return false;
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                                    <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BlanketAgreementsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetBlanketAgreement xmlns=\"BlanketAgreementsService\">\r\n                                     <BlanketAgreementParams>   <AgreementNo>" + AbsID + "</AgreementNo></BlanketAgreementParams>\r\n                                    </GetBlanketAgreement>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
			XmlElement xmlElement = DataService.GetByKey(sessionId, xmlObject).Content as XmlElement;
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				return false;
			}
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("AgreementNo"))
			{
				retxml = xmlElement.InnerXml;
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public string MappingDataSet(DataSet ds, DataTable doc, DataTable dtLines, DataTable dtDetails, EnumCommandType commandType)
	{
		try
		{
			if (!ds.Tables["BlanketAgreement"].IsExistsColumn("TerminateDate"))
			{
				ds.Tables["BlanketAgreement"].Columns.Add("TerminateDate", typeof(DateTime));
			}
			DataRow dataRow = ds.Tables["BlanketAgreement"].Rows[0];
			if (ColumnExist(doc.Rows[0], "Type") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Type"])))
			{
				string text = Function.ToString(doc.Rows[0]["Type"]);
				if (!(text == "G"))
				{
					if (text == "S")
					{
						dataRow["AgreementType"] = Function.ToString(BlanketAgreementTypeEnum.atSpecific);
					}
				}
				else
				{
					dataRow["AgreementType"] = Function.ToString(BlanketAgreementTypeEnum.atGeneral);
				}
			}
			if (ColumnExist(doc.Rows[0], "Method") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Method"])))
			{
				string text = Function.ToString(doc.Rows[0]["Method"]);
				if (!(text == "I"))
				{
					if (text == "M")
					{
						dataRow["AgreementMethod"] = Function.ToString(BlanketAgreementMethodEnum.amMonetary);
					}
				}
				else
				{
					dataRow["AgreementMethod"] = Function.ToString(BlanketAgreementMethodEnum.amItem);
				}
			}
			if (ColumnExist(doc.Rows[0], "AmendedTo") && Function.ParseInt(doc.Rows[0]["AmendedTo"]) > 0)
			{
				dataRow["AmendmentTo"] = Function.RawText(doc.Rows[0]["AmendedTo"]);
			}
			if (ColumnExist(doc.Rows[0], "AtchEntry") && Function.ParseInt(doc.Rows[0]["AtchEntry"]) > 0)
			{
				dataRow["AttachmentEntry"] = Function.ParseInt(doc.Rows[0]["AtchEntry"]);
			}
			if (ColumnExist(doc.Rows[0], "BpCode") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["BpCode"])))
			{
				dataRow["BPCode"] = Function.RawText(doc.Rows[0]["BpCode"]);
			}
			if (ColumnExist(doc.Rows[0], "BpName") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["BpName"])))
			{
				dataRow["BPName"] = Function.RawText(doc.Rows[0]["BpName"]);
			}
			if (ColumnExist(doc.Rows[0], "CntctCode") && Function.ParseInt(doc.Rows[0]["CntctCode"]) > 0)
			{
				dataRow["ContactPersonCode"] = Function.ParseInt(doc.Rows[0]["CntctCode"]);
			}
			if (ColumnExist(doc.Rows[0], "Descript") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["Descript"])))
			{
				dataRow["Descript"] = Function.RawText(doc.Rows[0]["Descript"]);
			}
			if (ColumnExist(doc.Rows[0], "EndDate") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["EndDate"])))
			{
				dataRow["EndDate"] = Convert.ToDateTime(doc.Rows[0]["EndDate"]).ToString("yyyy-MM-dd");
			}
			if (ColumnExist(doc.Rows[0], "Handwrtten") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Handwrtten"])))
			{
				string text = Function.ToString(doc.Rows[0]["Handwrtten"]);
				if (!(text == "Y"))
				{
					if (text == "N")
					{
						dataRow["HandWritten"] = Function.ToString(BoYesNoEnum.tNO);
					}
				}
				else
				{
					dataRow["HandWritten"] = Function.ToString(BoYesNoEnum.tYES);
				}
			}
			if (ColumnExist(doc.Rows[0], "UseDiscnt") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["UseDiscnt"])))
			{
				string text = Function.ToString(doc.Rows[0]["UseDiscnt"]);
				if (!(text == "Y"))
				{
					if (text == "N")
					{
						dataRow["IgnorePricesInAgreement"] = Function.ToString(BoYesNoEnum.tNO);
					}
				}
				else
				{
					dataRow["IgnorePricesInAgreement"] = Function.ToString(BoYesNoEnum.tYES);
				}
			}
			if (ColumnExist(doc.Rows[0], "Owner") && Function.ParseInt(doc.Rows[0]["Owner"]) > 0)
			{
				dataRow["Owner"] = Function.ParseInt(doc.Rows[0]["Owner"]);
			}
			if (ColumnExist(doc.Rows[0], "PayMethod") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["PayMethod"])))
			{
				dataRow["PaymentMethod"] = Function.RawText(doc.Rows[0]["PayMethod"]);
			}
			dataRow["PeriodIndicator"] = "Default";
			if (ColumnExist(doc.Rows[0], "GroupNum") && Function.ParseInt(doc.Rows[0]["GroupNum"]) > 0)
			{
				dataRow["PaymentTerms"] = Function.ParseInt(doc.Rows[0]["GroupNum"]);
			}
			if (ColumnExist(doc.Rows[0], "ListNum") && Function.ParseInt(doc.Rows[0]["ListNum"]) > 0)
			{
				dataRow["PriceList"] = Function.ParseInt(doc.Rows[0]["ListNum"]);
			}
			if (ColumnExist(doc.Rows[0], "Remarks") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["Remarks"])))
			{
				dataRow["Remarks"] = Function.RawText(doc.Rows[0]["Remarks"]);
			}
			if (ColumnExist(doc.Rows[0], "RemindVal") && Function.ParseInt(doc.Rows[0]["RemindVal"]) > 0)
			{
				dataRow["RemindTime"] = Function.ParseInt(doc.Rows[0]["RemindVal"]);
			}
			if (ColumnExist(doc.Rows[0], "RemindUnit") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["RemindUnit"])))
			{
				switch (Function.ToString(doc.Rows[0]["RemindUnit"]))
				{
				case "D":
					dataRow["RemindUnit"] = Function.ToString(BoRemindUnits.reu_Days);
					break;
				case "W":
					dataRow["RemindUnit"] = Function.ToString(BoRemindUnits.reu_Weeks);
					break;
				case "M":
					dataRow["RemindUnit"] = Function.ToString(BoRemindUnits.reu_Month);
					break;
				}
			}
			if (ColumnExist(doc.Rows[0], "Renewal") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Renewal"])))
			{
				string text = Function.ToString(doc.Rows[0]["Renewal"]);
				if (!(text == "Y"))
				{
					if (text == "N")
					{
						dataRow["Renewal"] = Function.ToString(BoYesNoEnum.tNO);
					}
				}
				else
				{
					dataRow["Renewal"] = Function.ToString(BoYesNoEnum.tYES);
				}
			}
			if (ColumnExist(doc.Rows[0], "Series") && Function.ParseInt(doc.Rows[0]["Series"]) > 0)
			{
				dataRow["Series"] = Function.ParseInt(doc.Rows[0]["Series"]);
			}
			if (ColumnExist(doc.Rows[0], "Number") && Function.ParseInt(doc.Rows[0]["Number"]) > 0)
			{
				dataRow["DocNum"] = Function.ParseInt(doc.Rows[0]["Number"]);
			}
			if (ColumnExist(doc.Rows[0], "SettleProb") && Function.ParseDecimal(doc.Rows[0]["SettleProb"]) > 0m)
			{
				dataRow["SettlementProbability"] = Function.ParseDecimal(doc.Rows[0]["SettleProb"]);
			}
			if (ColumnExist(doc.Rows[0], "SignDate") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["SignDate"])))
			{
				dataRow["SigningDate"] = Convert.ToDateTime(doc.Rows[0]["SignDate"]).ToString("yyyy-MM-dd");
			}
			if (ColumnExist(doc.Rows[0], "StartDate") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["StartDate"])))
			{
				dataRow["StartDate"] = Convert.ToDateTime(doc.Rows[0]["StartDate"]).ToString("yyyy-MM-dd");
			}
			if (ColumnExist(doc.Rows[0], "Status") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["Status"])))
			{
				dataRow["Status"] = Function.RawText(doc.Rows[0]["Status"]);
			}
			if (ColumnExist(doc.Rows[0], "TermDate") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["TermDate"])))
			{
				dataRow["TerminateDate"] = Convert.ToDateTime(doc.Rows[0]["TermDate"]).ToString("yyyy-MM-dd");
			}
			List<DataColumn> list = (from DataColumn m in doc.Columns
				where m.ColumnName.StartsWith("U_")
				select m).ToList();
			if (list != null && list.Count > 0)
			{
				foreach (DataColumn item in list)
				{
					if (!dataRow.IsExistsColumn(item.ColumnName))
					{
						ds.Tables["BlanketAgreement"].Columns.Add(new DataColumn(item.ColumnName, item.DataType));
					}
				}
				MappingUdfToTable(dataRow, list, doc.Rows[0]);
			}
			MappingLinesDataSet(ds, dtLines, dtDetails);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public void MappingLinesDataSet(DataSet ds, DataTable dtLines, DataTable dtDetails)
	{
		try
		{
			int num = 0;
			List<DataColumn> list = (from DataColumn m in dtLines.Columns
				where m.ColumnName.StartsWith("U_")
				select m).ToList();
			if (list != null && list.Count > 0)
			{
				foreach (DataColumn item in list)
				{
					if (!ds.Tables["BlanketAgreements_ItemsLine"].IsExistsColumn(item.ColumnName))
					{
						ds.Tables["BlanketAgreements_ItemsLine"].Columns.Add(new DataColumn(item.ColumnName, item.DataType));
					}
				}
			}
			foreach (DataRow row in dtLines.Rows)
			{
				int linenum = Function.ParseInt(row["AgrLineNum"]);
				DataRow dataRow2 = ds.Tables["BlanketAgreements_ItemsLine"].AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["AgreementRowNumber"]) == linenum);
				if (dataRow2 == null)
				{
					dataRow2 = ds.Tables["BlanketAgreements_ItemsLine"].NewRow();
					dataRow2["AgreementRowNumber"] = linenum;
					dataRow2["BlanketAgreements_ItemsLines_Id"] = Function.ParseInt(ds.Tables["BlanketAgreements_ItemsLines"].Rows[0]["BlanketAgreements_ItemsLines_Id"]);
					dataRow2["BlanketAgreements_ItemsLine_Id"] = ds.Tables["BlanketAgreements_ItemsLine"].AsEnumerable().Max((DataRow t) => Function.ParseInt(t["BlanketAgreements_ItemsLine_Id"])) + 1;
					ds.Tables["BlanketAgreements_ItemsLine"].Rows.InsertAt(dataRow2, num);
					ds.AcceptChanges();
				}
				if (ColumnExist(row, "AgrNo") && Function.ParseInt(row["AgrNo"]) > 0)
				{
					dataRow2["AgreementNo"] = Function.ParseInt(row["AgrNo"]);
				}
				if (ColumnExist(row, "AgrLineNum") && Function.ParseInt(row["AgrLineNum"]) > 0)
				{
					dataRow2["AgreementRowNumber"] = Function.ParseInt(row["AgrLineNum"]);
				}
				if (ColumnExist(row, "CumAmntFC") && Function.ParseDecimal(row["CumAmntFC"]) > 0m)
				{
					dataRow2["CumulativeAmountFC"] = Function.ParseDecimal(row["CumAmntFC"]);
				}
				if (ColumnExist(row, "CumAmntLC") && Function.ParseDecimal(row["CumAmntLC"]) > 0m)
				{
					dataRow2["CumulativeAmountLC"] = Function.ParseDecimal(row["CumAmntLC"]);
				}
				if (ColumnExist(row, "CumQty") && Function.ParseDecimal(row["CumQty"]) > 0m)
				{
					dataRow2["CumulativeQuantity"] = Function.ParseDecimal(row["CumQty"]);
				}
				if (ColumnExist(row, "WrrtyEnd") && !string.IsNullOrEmpty(Function.ToString(row["WrrtyEnd"])))
				{
					dataRow2["EndOfWarranty"] = Convert.ToDateTime(row["WrrtyEnd"]).ToString("yyyy-MM-dd");
				}
				if (ColumnExist(row, "FreeTxt") && !string.IsNullOrEmpty(Function.ToString(row["FreeTxt"])))
				{
					dataRow2["FreeTxt"] = Function.RawText(row["FreeTxt"]);
				}
				if (ColumnExist(row, "InvntryUom") && !string.IsNullOrEmpty(Function.ToString(row["InvntryUom"])))
				{
					dataRow2["InventoryUoM"] = Function.RawText(row["InvntryUom"]);
				}
				if (ColumnExist(row, "ItemName") && !string.IsNullOrEmpty(Function.ToString(row["ItemName"])))
				{
					dataRow2["ItemDescription"] = Function.RawText(row["ItemName"]);
				}
				if (ColumnExist(row, "ItemGroup") && Function.ParseInt(row["ItemGroup"]) > 0)
				{
					dataRow2["ItemGroup"] = Function.ParseInt(row["ItemGroup"]);
				}
				if (ColumnExist(row, "ItemCode") && !string.IsNullOrEmpty(Function.ToString(row["ItemCode"])))
				{
					dataRow2["ItemNo"] = Function.RawText(row["ItemCode"]);
				}
				if (ColumnExist(row, "Discount") && Function.ParseDecimal(row["Discount"]) > 0m)
				{
					dataRow2["LineDiscount"] = Function.ParseDecimal(row["Discount"]);
				}
				if (ColumnExist(row, "PlanAmtFC") && Function.ParseDecimal(row["PlanAmtFC"]) > 0m)
				{
					dataRow2["PlannedAmountFC"] = Function.ParseDecimal(row["PlanAmtFC"]);
				}
				if (ColumnExist(row, "PlanAmtLC") && Function.ParseDecimal(row["PlanAmtLC"]) > 0m)
				{
					dataRow2["PlannedAmountLC"] = Function.ParseDecimal(row["PlanAmtLC"]);
				}
				if (ColumnExist(row, "PlanQty") && Function.ParseDecimal(row["PlanQty"]) > 0m)
				{
					dataRow2["PlannedQuantity"] = Function.ParseDecimal(row["PlanQty"]);
				}
				if (ColumnExist(row, "RetPortion") && Function.ParseDecimal(row["RetPortion"]) > 0m)
				{
					dataRow2["PortionOfReturns"] = Function.ParseDecimal(row["RetPortion"]);
				}
				if (ColumnExist(row, "Currency") && !string.IsNullOrEmpty(Function.ToString(row["Currency"])))
				{
					dataRow2["PriceCurrency"] = Function.RawText(row["Currency"]);
				}
				if (ColumnExist(row, "UnitPrice") && Function.ParseDecimal(row["UnitPrice"]) > 0m)
				{
					dataRow2["UnitPrice"] = Function.ParseDecimal(row["UnitPrice"]);
				}
				if (ColumnExist(row, "UoMEntry") && Function.ParseInt(row["UoMEntry"]) > 0)
				{
					dataRow2["UoMEntry"] = Function.ParseInt(row["UoMEntry"]);
				}
				if (ColumnExist(row, "NumPerMsr") && Function.ParseDecimal(row["NumPerMsr"]) > 0m)
				{
					dataRow2["UnitsOfMeasurement"] = Function.ParseDecimal(row["NumPerMsr"]);
				}
				if (list != null && list.Count > 0)
				{
					MappingUdfToTable(dataRow2, list, row);
				}
				num++;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private string Add(DataTable doc, DataTable dtLines, DataTable dtDetails, string sessionId)
	{
		try
		{
			string text = MappingXml(doc, dtLines, dtDetails, EnumCommandType.Add);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BlanketAgreementsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <AddBlanketAgreement xmlns=\"BlanketAgreementsService\">" + text + "</AddBlanketAgreement>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
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
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Add BlanketAgreement - {arg}");
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

	public void LoadXml(DataSet ds, XElement element)
	{
		LoadXmlColumn(ds, element);
		LoadXmlValue(ds, element, "", -1);
	}

	public void LoadXmlColumn(DataSet ds, XElement element, string fatherTableName = "", string fatherColumnName = "")
	{
		DataTable dataTable = new DataTable();
		if (ds.Tables.Contains(element.Name.LocalName))
		{
			dataTable = ds.Tables[element.Name.LocalName];
		}
		else
		{
			dataTable = new DataTable(element.Name.LocalName);
			ds.Tables.Add(dataTable);
		}
		string text = $"{element.Name.LocalName}_Id";
		DataColumn dataColumn = new DataColumn(text, typeof(int));
		dataColumn.ColumnMapping = MappingType.Hidden;
		if (!dataTable.IsExistsColumn(text))
		{
			dataTable.Columns.Add(dataColumn);
		}
		if (!string.IsNullOrEmpty(fatherTableName) && !string.IsNullOrEmpty(fatherColumnName))
		{
			DataColumn dataColumn2 = new DataColumn(fatherColumnName, typeof(int));
			dataColumn2.ColumnMapping = MappingType.Hidden;
			if (!dataTable.IsExistsColumn(fatherColumnName))
			{
				dataTable.Columns.Add(dataColumn2);
			}
			string text2 = $"{fatherTableName}_{element.Name.LocalName}";
			if (!ds.Relations.Contains(text2))
			{
				DataRelation dataRelation = new DataRelation(text2, ds.Tables[fatherTableName].Columns[fatherColumnName], dataColumn2);
				dataRelation.Nested = true;
				ds.Relations.Add(dataRelation);
			}
		}
		foreach (XElement item in element.Elements())
		{
			if (item == null)
			{
				continue;
			}
			if (item.HasElements)
			{
				LoadXmlColumn(ds, item, element.Name.LocalName, text);
			}
			else if (!item.IsEmpty && !dataTable.Columns.Contains(item.Name.LocalName))
			{
				DataColumn dataColumn3 = new DataColumn();
				dataColumn3.ColumnName = item.Name.LocalName;
				dataColumn3.DataType = typeof(string);
				if (!dataTable.IsExistsColumn(dataColumn3.ColumnName))
				{
					dataTable.Columns.Add(dataColumn3);
				}
			}
		}
	}

	public void LoadXmlValue(DataSet ds, XElement element, string fatherCol, int fatherid)
	{
		try
		{
			DataTable dataTable = ds.Tables[element.Name.LocalName];
			DataRow dataRow = dataTable.NewRow();
			string colName = $"{element.Name.LocalName}_Id";
			int num = 0;
			if (dataTable.Rows.Count > 0)
			{
				num = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t[colName])) + 1;
			}
			dataRow[colName] = num;
			if (!string.IsNullOrEmpty(fatherCol))
			{
				dataRow[fatherCol] = fatherid;
			}
			dataTable.Rows.Add(dataRow);
			foreach (XElement item in element.Elements())
			{
				if (item != null)
				{
					if (item.HasElements)
					{
						LoadXmlValue(ds, item, colName, num);
					}
					else if (!item.IsEmpty)
					{
						dataRow[item.Name.LocalName] = item.Value;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw ex;
		}
	}

	private string Update(DataTable doc, DataTable dtLines, DataTable dtDetails, string sessionId, string retxml)
	{
		try
		{
			string text = MappingXml(doc, dtLines, dtDetails, EnumCommandType.Update);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BlanketAgreementsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <UpdateBlanketAgreement xmlns=\"BlanketAgreementsService\">" + text + "</UpdateBlanketAgreement>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
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
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Update BlanketAgreement - {arg}");
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

	public virtual string Cancel(int docEntry, string sessionId)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>BlanketAgreementsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                     <CancelBlanketAgreement xmlns=\"BlanketAgreementsService\">\r\n                                     <BlanketAgreementParams>   <AgreementNo>" + docEntry + "</AgreementNo></BlanketAgreementParams>\r\n                                    </CancelBlanketAgreement>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
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
					string arg = Function.ToString(xElement3.Value);
					Logging.Write("WATCH", $"Cancel BlanketAgreement - {arg}");
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

	protected bool ColumnExist(DataRow dr, string colName)
	{
		if (dr.Table.Columns.Contains(colName))
		{
			return true;
		}
		return false;
	}

	protected string SaveOAT3(DataTable dtClgCode)
	{
		try
		{
			int num = 0;
			using Transaction transaction = new Transaction();
			foreach (DataRow row in dtClgCode.Rows)
			{
				int num2 = Function.ParseInt(row["AgrNo"]);
				int num3 = Function.ParseInt(row["AgrLnNum"]);
				int num4 = Function.ParseInt(row["AgrEfctNum"]);
				int num5 = Function.ParseInt(row["ActivityID"]);
				ExecuteNonQuery("DELETE FROM OAT3 WHERE \"AgrNo\" = @agrNo AND \"AgrLnNum\" = @agrLine AND \"AgrEfctNum\" = @agrDetail AND \"ActivityID\" = @activity;", CommandType.Text, new SqlParameter("@agrNo", num2), new SqlParameter("@agrLine", num3), new SqlParameter("@agrDetail", num4), new SqlParameter("@activity", num5));
				string query = "INSERT INTO OAT3(\"AgrNo\", \"AgrLnNum\", \"AgrEfctNum\", \"ActivityID\", \"LogInstanc\")\r\n                                        VALUES (@agrNo, @agrLine, @agrDetail, @activity, 0);";
				if (ExecuteNonQuery(query, CommandType.Text, new SqlParameter("@agrNo", num2), new SqlParameter("@agrLine", num3), new SqlParameter("@agrDetail", num4), new SqlParameter("@activity", num5)) > 0)
				{
					num++;
				}
			}
			if (num == dtClgCode.Rows.Count)
			{
				transaction.Commit();
				return string.Empty;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private string MappingXml(DataTable doc, DataTable dtLines, DataTable dtDetails, EnumCommandType commandType)
	{
		try
		{
			string text = "<BlanketAgreement>";
			if (ColumnExist(doc.Rows[0], "AbsID") && Function.ParseInt(Function.ParseInt(doc.Rows[0]["AbsID"])) > 0)
			{
				text = text + "<AgreementNo>" + Function.ParseInt(doc.Rows[0]["AbsID"]) + "</AgreementNo>";
			}
			if (ColumnExist(doc.Rows[0], "BpCode") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["BpCode"])))
			{
				text = text + "<BPCode>" + Function.RawText(doc.Rows[0]["BpCode"]) + "</BPCode>";
			}
			if (ColumnExist(doc.Rows[0], "BpName") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["BpName"])))
			{
				text = text + "<BPName>" + Function.RawText(doc.Rows[0]["BpName"]) + "</BPName>";
			}
			if (ColumnExist(doc.Rows[0], "CntctCode") && Function.ParseInt(doc.Rows[0]["CntctCode"]) > 0)
			{
				text = text + "<ContactPersonCode>" + Function.ParseInt(doc.Rows[0]["CntctCode"]) + "</ContactPersonCode>";
			}
			if (ColumnExist(doc.Rows[0], "StartDate") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["StartDate"])))
			{
				text = text + "<StartDate>" + Convert.ToDateTime(doc.Rows[0]["StartDate"]).ToString("yyyy-MM-dd") + "</StartDate>";
			}
			if (ColumnExist(doc.Rows[0], "EndDate") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["EndDate"])))
			{
				text = text + "<EndDate>" + Convert.ToDateTime(doc.Rows[0]["EndDate"]).ToString("yyyy-MM-dd") + "</EndDate>";
			}
			if (ColumnExist(doc.Rows[0], "TermDate") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["TermDate"])))
			{
				text = text + "<TerminateDate>" + Convert.ToDateTime(doc.Rows[0]["TermDate"]).ToString("yyyy-MM-dd") + "</TerminateDate>";
			}
			if (ColumnExist(doc.Rows[0], "Descript") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["Descript"])))
			{
				text = text + "<Description>" + Function.RawText(doc.Rows[0]["Descript"]) + "</Description>";
			}
			if (ColumnExist(doc.Rows[0], "Type") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Type"])))
			{
				string text2 = Function.ToString(doc.Rows[0]["Type"]);
				if (!(text2 == "G"))
				{
					if (text2 == "S")
					{
						text = text + "<AgreementType>" + Function.ToString(BlanketAgreementTypeEnum.atSpecific) + "</AgreementType>";
					}
				}
				else
				{
					text = text + "<AgreementType>" + Function.ToString(BlanketAgreementTypeEnum.atGeneral) + "</AgreementType>";
				}
			}
			if (ColumnExist(doc.Rows[0], "Status") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["Status"])))
			{
				text = Function.ToString(doc.Rows[0]["Status"]) switch
				{
					"A" => text + "<Status>" + Function.ToString(BlanketAgreementStatusEnum.asApproved) + "</Status>", 
					"F" => text + "<Status>" + Function.ToString(BlanketAgreementStatusEnum.asOnHold) + "</Status>", 
					"T" => text + "<Status>" + Function.ToString(BlanketAgreementStatusEnum.asTerminated) + "</Status>", 
					_ => text + "<Status>" + Function.ToString(BlanketAgreementStatusEnum.asDraft) + "</Status>", 
				};
			}
			if (ColumnExist(doc.Rows[0], "Owner") && Function.ParseInt(doc.Rows[0]["Owner"]) > 0)
			{
				text = text + "<Owner>" + Function.ParseInt(doc.Rows[0]["Owner"]) + "</Owner>";
			}
			else if (Function.ParseInt(doc.Rows[0]["Owner"]) <= 0)
			{
				text = text + "<Owner>" + -1 + "</Owner>";
			}
			if (ColumnExist(doc.Rows[0], "UseDiscnt"))
			{
				string text2 = Function.ToString(doc.Rows[0]["UseDiscnt"]);
				if (text2 == "Y")
				{
					text = text + "<IgnorePricesInAgreement>" + Function.ToString(BoYesNoEnum.tYES) + "</IgnorePricesInAgreement>";
				}
				else
				{
					text = ((!(text2 == "N")) ? (text + "<IgnorePricesInAgreement>" + Function.ToString(BoYesNoEnum.tYES) + "</IgnorePricesInAgreement>") : (text + "<IgnorePricesInAgreement>" + Function.ToString(BoYesNoEnum.tNO) + "</IgnorePricesInAgreement>"));
				}
			}
			if (ColumnExist(doc.Rows[0], "Renewal") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Renewal"])))
			{
				string text2 = Function.ToString(doc.Rows[0]["Renewal"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<Renewal>" + Function.ToString(BoYesNoEnum.tNO) + "</Renewal>";
					}
				}
				else
				{
					text = text + "<Renewal>" + Function.ToString(BoYesNoEnum.tYES) + "</Renewal>";
				}
			}
			if (ColumnExist(doc.Rows[0], "RemindUnit") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["RemindUnit"])))
			{
				switch (Function.ToString(doc.Rows[0]["RemindUnit"]))
				{
				case "D":
					text = text + "<RemindUnit>" + Function.ToString(BoRemindUnits.reu_Days) + "</RemindUnit>";
					break;
				case "W":
					text = text + "<RemindUnit>" + Function.ToString(BoRemindUnits.reu_Weeks) + "</RemindUnit>";
					break;
				case "M":
					text = text + "<RemindUnit>" + Function.ToString(BoRemindUnits.reu_Month) + "</RemindUnit>";
					break;
				}
			}
			if (ColumnExist(doc.Rows[0], "RemindVal") && Function.ParseInt(doc.Rows[0]["RemindVal"]) > 0)
			{
				text = text + "<RemindTime>" + Function.ParseInt(doc.Rows[0]["RemindVal"]) + "</RemindTime>";
			}
			if (ColumnExist(doc.Rows[0], "Remarks") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["Remarks"])))
			{
				text = text + "<Remarks>" + Function.RawText(doc.Rows[0]["Remarks"]) + "</Remarks>";
			}
			if (ColumnExist(doc.Rows[0], "SettleProb") && Function.ParseDecimal(doc.Rows[0]["SettleProb"]) > 0m)
			{
				text = text + "<SettlementProbability>" + Function.ParseDecimal(doc.Rows[0]["SettleProb"]) + "</SettlementProbability>";
			}
			if (ColumnExist(doc.Rows[0], "Method") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Method"])))
			{
				string text2 = Function.ToString(doc.Rows[0]["Method"]);
				if (!(text2 == "I"))
				{
					if (text2 == "M")
					{
						text = text + "<AgreementMethod>" + Function.ToString(BlanketAgreementMethodEnum.amMonetary) + "</AgreementMethod>";
					}
				}
				else
				{
					text = text + "<AgreementMethod>" + Function.ToString(BlanketAgreementMethodEnum.amItem) + "</AgreementMethod>";
				}
			}
			if (ColumnExist(doc.Rows[0], "GroupNum") && Function.ParseInt(doc.Rows[0]["GroupNum"]) > 0)
			{
				text = text + "<PaymentTerms>" + Function.ParseInt(doc.Rows[0]["GroupNum"]) + "</PaymentTerms>";
			}
			if (ColumnExist(doc.Rows[0], "ListNum") && Function.ParseInt(doc.Rows[0]["ListNum"]) > 0)
			{
				text = text + "<PriceList>" + Function.ParseInt(doc.Rows[0]["ListNum"]) + "</PriceList>";
			}
			if (ColumnExist(doc.Rows[0], "SignDate") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["SignDate"])))
			{
				text = text + "<SigningDate>" + Convert.ToDateTime(doc.Rows[0]["SignDate"]).ToString("yyyy-MM-dd") + "</SigningDate>";
			}
			if (ColumnExist(doc.Rows[0], "AmendedTo") && Function.ParseInt(doc.Rows[0]["AmendedTo"]) > 0)
			{
				text = text + "<AmendmentTo>" + Function.RawText(doc.Rows[0]["AmendedTo"]) + "</AmendmentTo>";
			}
			if (ColumnExist(doc.Rows[0], "Series") && Function.ParseInt(doc.Rows[0]["Series"]) != 0)
			{
				text = text + "<Series>" + Function.ParseInt(doc.Rows[0]["Series"]) + "</Series>";
			}
			if (ColumnExist(doc.Rows[0], "Number") && Function.ParseInt(doc.Rows[0]["Number"]) > 0)
			{
				text = text + "<DocNum>" + Function.ParseInt(doc.Rows[0]["Number"]) + "</DocNum>";
			}
			if (ColumnExist(doc.Rows[0], "Handwrtten") && !string.IsNullOrEmpty(Function.ToString(doc.Rows[0]["Handwrtten"])))
			{
				string text2 = Function.ToString(doc.Rows[0]["Handwrtten"]);
				if (!(text2 == "Y"))
				{
					if (text2 == "N")
					{
						text = text + "<HandWritten>" + Function.ToString(BoYesNoEnum.tNO) + "</HandWritten>";
					}
				}
				else
				{
					text = text + "<HandWritten>" + Function.ToString(BoYesNoEnum.tYES) + "</HandWritten>";
				}
			}
			text += "<PeriodIndicator>Default</PeriodIndicator>";
			if (ColumnExist(doc.Rows[0], "FixedRate") && (Function.ParseInt(doc.Rows[0]["FixedRate"]) > 0 || commandType == EnumCommandType.Update))
			{
				text = text + "<ExchangeRate>" + Function.ParseInt(doc.Rows[0]["FixedRate"]) + "</ExchangeRate>";
			}
			if (ColumnExist(doc.Rows[0], "PayMethod") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["PayMethod"])))
			{
				text = text + "<PaymentMethod>" + Function.RawText(doc.Rows[0]["PayMethod"]) + "</PaymentMethod>";
			}
			if (ColumnExist(doc.Rows[0], "TrnspCode") && Function.ParseInt(doc.Rows[0]["TrnspCode"]) != 0)
			{
				text = text + "<ShippingType>" + Function.ParseInt(doc.Rows[0]["TrnspCode"]) + "</ShippingType>";
			}
			if (ColumnExist(doc.Rows[0], "NumAtCard") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["NumAtCard"])))
			{
				text = text + "<NumAtCard>" + Function.RawText(doc.Rows[0]["NumAtCard"]) + "</NumAtCard>";
			}
			if (ColumnExist(doc.Rows[0], "Project") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["Project"])))
			{
				text = text + "<Project>" + Function.RawText(doc.Rows[0]["Project"]) + "</Project>";
			}
			text += "<PriceMode>pmNet</PriceMode>";
			if (ColumnExist(doc.Rows[0], "BPCurr") && !string.IsNullOrEmpty(Function.RawText(doc.Rows[0]["BPCurr"])))
			{
				text = text + "<BPCurrency>" + Function.RawText(doc.Rows[0]["BPCurr"]) + "</BPCurrency>";
			}
			text += DataService.MappingUdf(doc.Rows[0]);
			if (dtLines.IsNotNull())
			{
				text += "<BlanketAgreements_ItemsLines>";
				foreach (DataRow dr in dtLines.Rows)
				{
					text += "<BlanketAgreements_ItemsLine>";
					if (ColumnExist(dr, "AgrNo") && Function.ParseInt(dr["AgrNo"]) > 0)
					{
						text = text + "<AgreementNo>" + Function.ParseInt(dr["AgrNo"]) + "</AgreementNo>";
					}
					if (ColumnExist(dr, "AgrLineNum") && Function.ParseInt(dr["AgrLineNum"]) >= 0)
					{
						text = text + "<AgreementRowNumber>" + Function.ParseInt(dr["AgrLineNum"]) + "</AgreementRowNumber>";
					}
					if (ColumnExist(dr, "ItemCode") && !string.IsNullOrEmpty(Function.ToString(dr["ItemCode"])))
					{
						text = text + "<ItemNo>" + Function.RawText(dr["ItemCode"]) + "</ItemNo>";
					}
					if (ColumnExist(dr, "ItemName") && !string.IsNullOrEmpty(Function.ToString(dr["ItemName"])))
					{
						text = text + "<ItemDescription>" + Function.RawText(dr["ItemName"]) + "</ItemDescription>";
					}
					if (ColumnExist(dr, "ItemGroup") && Function.ParseInt(dr["ItemGroup"]) > 0)
					{
						text = text + "<ItemGroup>" + Function.ParseInt(dr["ItemGroup"]) + "</ItemGroup>";
					}
					if (ColumnExist(dr, "PlanQty") && Function.ParseDecimal(dr["PlanQty"]) > 0m)
					{
						text = text + "<PlannedQuantity>" + Function.ParseDecimal(dr["PlanQty"]) + "</PlannedQuantity>";
					}
					if (ColumnExist(dr, "UnitPrice") && Function.ParseDecimal(dr["UnitPrice"]) > 0m)
					{
						text = text + "<UnitPrice>" + Function.ParseDecimal(dr["UnitPrice"]) + "</UnitPrice>";
					}
					if (ColumnExist(dr, "Currency") && !string.IsNullOrEmpty(Function.ToString(dr["Currency"])))
					{
						text = text + "<PriceCurrency>" + Function.RawText(dr["Currency"]) + "</PriceCurrency>";
					}
					if (ColumnExist(dr, "CumQty") && (Function.ParseDecimal(dr["CumQty"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<CumulativeQuantity>" + Function.ParseDecimal(dr["CumQty"]) + "</CumulativeQuantity>";
					}
					if (ColumnExist(dr, "CumAmntFC") && (Function.ParseDecimal(dr["CumAmntFC"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<CumulativeAmountFC>" + Function.ParseDecimal(dr["CumAmntFC"]) + "</CumulativeAmountFC>";
					}
					if (ColumnExist(dr, "CumAmntLC") && (Function.ParseDecimal(dr["CumAmntLC"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<CumulativeAmountLC>" + Function.ParseDecimal(dr["CumAmntLC"]) + "</CumulativeAmountLC>";
					}
					if (ColumnExist(dr, "FreeTxt") && !string.IsNullOrEmpty(Function.ToString(dr["FreeTxt"])))
					{
						text = text + "<FreeText>" + Function.RawText(dr["FreeTxt"]) + "</FreeText>";
					}
					if (ColumnExist(dr, "InvntryUom") && !string.IsNullOrEmpty(Function.ToString(dr["InvntryUom"])))
					{
						text = text + "<InventoryUoM>" + Function.RawText(dr["InvntryUom"]) + "</InventoryUoM>";
					}
					if (ColumnExist(dr, "RetPortion") && Function.ParseDecimal(dr["RetPortion"]) > 0m)
					{
						text = text + "<PortionOfReturns>" + Function.ParseDecimal(dr["RetPortion"]) + "</PortionOfReturns>";
					}
					if (ColumnExist(dr, "WrrtyEnd") && !string.IsNullOrEmpty(Function.ToString(dr["WrrtyEnd"])))
					{
						text = text + "<EndOfWarranty>" + Convert.ToDateTime(dr["WrrtyEnd"]).ToString("yyyy-MM-dd") + "</EndOfWarranty>";
					}
					if (ColumnExist(dr, "PlanAmtLC") && (Function.ParseDecimal(dr["PlanAmtLC"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<PlannedAmountLC>" + Function.ParseDecimal(dr["PlanAmtLC"]) + "</PlannedAmountLC>";
					}
					if (ColumnExist(dr, "PlanAmtFC") && (Function.ParseDecimal(dr["PlanAmtFC"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<PlannedAmountFC>" + Function.ParseDecimal(dr["PlanAmtFC"]) + "</PlannedAmountFC>";
					}
					if (ColumnExist(dr, "Discount") && (Function.ParseDecimal(dr["Discount"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<LineDiscount>" + Function.ParseDecimal(dr["Discount"]) + "</LineDiscount>";
					}
					if (ColumnExist(dr, "UgpEntry") && (Function.ParseInt(dr["UgpEntry"]) > 0 || commandType == EnumCommandType.Update))
					{
						text = text + "<UoMEntry>" + Function.ParseInt(dr["UgpEntry"]) + "</UoMEntry>";
					}
					if (ColumnExist(dr, "UomCode") && !string.IsNullOrEmpty(Function.ToString(dr["UomCode"])))
					{
						text = text + "<UoMCode>" + Function.RawText(dr["UomCode"]) + "</UoMCode>";
					}
					if (ColumnExist(dr, "NumPerMsr") && (Function.ParseDecimal(dr["NumPerMsr"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<UnitsOfMeasurement>" + Function.ParseDecimal(dr["NumPerMsr"]) + "</UnitsOfMeasurement>";
					}
					if (ColumnExist(dr, "UndlvQty") && (Function.ParseDecimal(dr["UndlvQty"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<UndeliveredCumulativeQuantity>" + Function.ParseDecimal(dr["UndlvQty"]) + "</UndeliveredCumulativeQuantity>";
					}
					if (ColumnExist(dr, "UndlvAmntL") && (Function.ParseDecimal(dr["UndlvAmntL"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<UndeliveredCumulativeAmountLC>" + Function.ParseDecimal(dr["UndlvAmntL"]) + "</UndeliveredCumulativeAmountLC>";
					}
					if (ColumnExist(dr, "UndlvAmntF") && (Function.ParseDecimal(dr["UndlvAmntF"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<UndeliveredCumulativeAmountFC>" + Function.ParseDecimal(dr["UndlvAmntF"]) + "</UndeliveredCumulativeAmountFC>";
					}
					if (ColumnExist(dr, "TrnspCode") && Function.ParseInt(dr["TrnspCode"]) > 0)
					{
						text = text + "<ShippingType>" + Function.ParseInt(dr["TrnspCode"]) + "</ShippingType>";
					}
					if (ColumnExist(dr, "Project") && !string.IsNullOrEmpty(Function.ToString(dr["Project"])))
					{
						text = text + "<Project>" + Function.RawText(dr["Project"]) + "</Project>";
					}
					if (ColumnExist(dr, "TaxCode") && !string.IsNullOrEmpty(Function.ToString(dr["TaxCode"])))
					{
						text = text + "<TaxCode>" + Function.RawText(dr["TaxCode"]) + "</TaxCode>";
					}
					if (ColumnExist(dr, "TAXRate") && commandType == EnumCommandType.Update)
					{
						text = text + "<TAXRate>" + Function.ParseDecimal(dr["TAXRate"]) + "</TAXRate>";
					}
					if (ColumnExist(dr, "PlVatAmtLC") && !string.IsNullOrEmpty(Function.ToString(dr["PlVatAmtLC"])) && (Function.ParseDecimal(dr["PlVatAmtLC"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<PlannedVATAmountLC>" + Function.ParseDecimal(dr["PlVatAmtLC"]) + "</PlannedVATAmountLC>";
					}
					if (ColumnExist(dr, "PlVatAmtFC") && !string.IsNullOrEmpty(Function.ToString(dr["PlVatAmtFC"])) && (Function.ParseDecimal(dr["PlVatAmtFC"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<PlannedVATAmountFC>" + Function.ParseDecimal(dr["PlVatAmtFC"]) + "</PlannedVATAmountFC>";
					}
					if (ColumnExist(dr, "CumVtAmtLC") && !string.IsNullOrEmpty(Function.ToString(dr["CumVtAmtLC"])) && (Function.ParseDecimal(dr["CumVtAmtLC"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<CumulativeVATAmountLC>" + Function.ParseDecimal(dr["CumVtAmtLC"]) + "</CumulativeVATAmountLC>";
					}
					if (ColumnExist(dr, "CumVtAmtFC") && !string.IsNullOrEmpty(Function.ToString(dr["CumVtAmtFC"])) && (Function.ParseDecimal(dr["CumVtAmtFC"]) > 0m || commandType == EnumCommandType.Update))
					{
						text = text + "<CumulativeVATAmountFC>" + Function.ParseDecimal(dr["CumVtAmtFC"]) + "</CumulativeVATAmountFC>";
					}
					text += DataService.MappingUdf(dr);
					if (dtDetails != null && dtDetails.Rows.Count > 0)
					{
						List<DataRow> list = (from t in dtDetails.AsEnumerable()
							where Function.ParseInt(t["AgrNo"]) == Function.ParseInt(dr["AgrNo"]) && Function.ParseInt(t["AgrLnNum"]) == Function.ParseInt(dr["AgrLineNum"])
							select t).ToList();
						if (list != null && list.Count > 0)
						{
							text += "<BlanketAgreements_DetailsLines>";
							foreach (DataRow item in list)
							{
								text += "<BlanketAgreements_DetailsLine>";
								if (ColumnExist(item, "AgrNo") && Function.ParseInt(item["AgrNo"]) > 0)
								{
									text = text + "<AgreementNo>" + Function.ParseInt(item["AgrNo"]) + "</AgreementNo>";
								}
								if (ColumnExist(item, "AgrLnNum") && Function.ParseInt(item["AgrLnNum"]) > 0)
								{
									text = text + "<AgreementRowNumber>" + Function.ParseInt(item["AgrLnNum"]) + "</AgreementRowNumber>";
								}
								if (ColumnExist(item, "AgrEfctNum") && Function.ParseInt(item["AgrEfctNum"]) > 0)
								{
									text = text + "<AgreementEffectiveRowNumber>" + Function.ParseInt(item["AgrEfctNum"]) + "</AgreementEffectiveRowNumber>";
								}
								if (ColumnExist(item, "DatePeriod") && !string.IsNullOrEmpty(Function.ToString(item["DatePeriod"])))
								{
									switch (Function.ToString(item["DatePeriod"]))
									{
									case "D":
										text = text + "<Frequency>" + Function.ToString(BlanketAgreementDatePeriodsEnum.Daily) + "</Frequency>";
										break;
									case "W":
										text = text + "<Frequency>" + Function.ToString(BlanketAgreementDatePeriodsEnum.Weekly) + "</Frequency>";
										break;
									case "M":
										text = text + "<Frequency>" + Function.ToString(BlanketAgreementDatePeriodsEnum.Monthly) + "</Frequency>";
										break;
									case "Q":
										text = text + "<Frequency>" + Function.ToString(BlanketAgreementDatePeriodsEnum.Quarterly) + "</Frequency>";
										break;
									case "S":
										text = text + "<Frequency>" + Function.ToString(BlanketAgreementDatePeriodsEnum.Semiannually) + "</Frequency>";
										break;
									case "A":
										text = text + "<Frequency>" + Function.ToString(BlanketAgreementDatePeriodsEnum.Annually) + "</Frequency>";
										break;
									case "O":
										text = text + "<Frequency>" + Function.ToString(BlanketAgreementDatePeriodsEnum.OneTime) + "</Frequency>";
										break;
									}
								}
								if (ColumnExist(item, "FromDate") && !string.IsNullOrEmpty(Function.ToString(item["FromDate"])))
								{
									text = text + "<From>" + Convert.ToDateTime(item["FromDate"]).ToString("yyyy-MM-dd") + "</From>";
								}
								if (ColumnExist(item, "ToDate") && !string.IsNullOrEmpty(Function.ToString(item["ToDate"])))
								{
									text = text + "<To>" + Convert.ToDateTime(item["ToDate"]).ToString("yyyy-MM-dd") + "</To>";
								}
								if (ColumnExist(item, "CallUp") && !string.IsNullOrEmpty(Function.ToString(item["CallUp"])))
								{
									text = text + "<ReleaseInformation>" + Function.RawText(item["CallUp"]) + "</ReleaseInformation>";
								}
								if (ColumnExist(item, "Quantity") && Function.ParseDecimal(item["Quantity"]) > 0m)
								{
									text = text + "<Quantity>" + Function.ParseDecimal(item["Quantity"]) + "</Quantity>";
								}
								if (ColumnExist(item, "WhsCode") && !string.IsNullOrEmpty(Function.ToString(item["WhsCode"])))
								{
									text = text + "<Warehouse>" + Function.RawText(item["WhsCode"]) + "</Warehouse>";
								}
								if (ColumnExist(item, "FreeTxt") && !string.IsNullOrEmpty(Function.ToString(item["FreeTxt"])))
								{
									text = text + "<FreeText>" + Function.RawText(item["FreeTxt"]) + "</FreeText>";
								}
								if (ColumnExist(item, "ConsumeFCT") && !string.IsNullOrEmpty(Function.ToString(item["ConsumeFCT"])))
								{
									string text2 = Function.ToString(item["ConsumeFCT"]);
									if (!(text2 == "Y"))
									{
										if (text2 == "N")
										{
											text = text + "<ConsumeSalesForecast>" + Function.ToString(BoYesNoEnum.tNO) + "</ConsumeSalesForecast>";
										}
									}
									else
									{
										text = text + "<ConsumeSalesForecast>" + Function.ToString(BoYesNoEnum.tYES) + "</ConsumeSalesForecast>";
									}
								}
								if (ColumnExist(item, "AmountLC") && Function.ParseDecimal(item["AmountLC"]) >= 0m)
								{
									text = text + "<PlannedAmountLC>" + Function.ParseDecimal(item["AmountLC"]) + "</PlannedAmountLC>";
								}
								if (ColumnExist(item, "AmountFC") && Function.ParseDecimal(item["AmountFC"]) >= 0m)
								{
									text = text + "<PlannedAmountFC>" + Function.ParseDecimal(item["AmountFC"]) + "</PlannedAmountFC>";
								}
								text += DataService.MappingUdf(item);
								text += "</BlanketAgreements_DetailsLine>";
							}
							text += "</BlanketAgreements_DetailsLines>";
						}
					}
					else
					{
						text += "<BlanketAgreements_DetailsLines />";
					}
					text += "</BlanketAgreements_ItemsLine>";
				}
				text += "</BlanketAgreements_ItemsLines>";
			}
			return text + "</BlanketAgreement>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}
}
