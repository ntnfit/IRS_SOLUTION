using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.DataServices.Interfaces;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class DiscountGroupRepository : Repository<DataSet>, IDiscountGroup, IDisposable
{
	protected readonly IUnitOfWork _unitOfWork;

	public DiscountGroupRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public async Task<string> Save(string sessionId, DataSet ds)
	{
		_ = 1;
		try
		{
			if (ds == null || ds.Tables.Count != 2 || ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0 || ds.Tables[1] == null || ds.Tables[1].Rows.Count == 0)
			{
				return "No data to save - DataSet or DataTable is null";
			}
			if (Exist(sessionId, Function.ParseInt(ds.Tables[0].Rows[0]["AbsEntry"])) == 0)
			{
				return await AddNew(sessionId, ds);
			}
			return await UpdateData(sessionId, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public int Exist(string sessionId, int absEntry)
	{
		try
		{
			return Function.ParseInt(ExecuteScarlar("select count(1) from OEDG where \"AbsEntry\" = @absEntry;", CommandType.Text, new SqlParameter("@absEntry", absEntry)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return -1;
	}

	public virtual async Task<string> UpdateData(string sessionId, DataSet ds)
	{
		try
		{
			string text = Mapping(ds, EnumCommandType.Update);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>EnhancedDiscountGroupsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <UpdateDiscountGroup xmlns=\"EnhancedDiscountGroupsService\">" + Function.ToString(text) + "</UpdateDiscountGroup>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
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

	public virtual async Task<string> AddNew(string sessionId, DataSet ds)
	{
		try
		{
			string text = Mapping(ds, EnumCommandType.Add);
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>EnhancedDiscountGroupsService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Add xmlns=\"EnhancedDiscountGroupsService\">" + Function.ToString(text) + "</Add>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
			XmlElement xmlElement = (await DataService.ProcessAsync(xmlObject)).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
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

	public virtual string Mapping(DataSet ds, EnumCommandType commandType)
	{
		string text = "";
		try
		{
			DataRow dataRow = ds.Tables[0].Rows[0];
			DataTable dataTable = ds.Tables[1];
			text += "<EnhancedDiscountGroup>";
			text = text + "<AbsEntry>" + Function.ParseInt(dataRow["AbsEntry"]) + "</AbsEntry>";
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["Type"])))
			{
				text = Function.ToString(dataRow["Type"]) switch
				{
					"A" => text + "<Type>dgt_AllBPs</Type>", 
					"C" => text + "<Type>dgt_CustomerGroup</Type>", 
					"V" => text + "<Type>dgt_VendorGroup</Type>", 
					"S" => text + "<Type>dgt_SpecificBP</Type>", 
					_ => text + "<Type>dgt_AllBPs</Type>", 
				};
			}
			text = (string.IsNullOrEmpty(Function.ToString(dataRow["ObjCode"])) ? (text + "<ObjectCode/>") : (text + "<ObjectCode>" + Function.RawText(dataRow["ObjCode"]) + "</ObjectCode>"));
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["DiscRel"])))
			{
				switch (Function.ToString(dataRow["DiscRel"]))
				{
				case "L":
					text = text + "<DiscountRelations>" + DiscountGroupRelationsEnum.dgrLowestDiscount.ToString() + "</DiscountRelations>";
					break;
				case "H":
					text = text + "<DiscountRelations>" + DiscountGroupRelationsEnum.dgrHighestDiscount.ToString() + "</DiscountRelations>";
					break;
				case "A":
					text = text + "<DiscountRelations>" + DiscountGroupRelationsEnum.dgrAverageDiscount.ToString() + "</DiscountRelations>";
					break;
				case "S":
					text = text + "<DiscountRelations>" + DiscountGroupRelationsEnum.dgrDiscountTotals.ToString() + "</DiscountRelations>";
					break;
				case "M":
					text = text + "<DiscountRelations>" + DiscountGroupRelationsEnum.dgrMultipliedDiscount.ToString() + "</DiscountRelations>";
					break;
				}
			}
			text = ((!string.IsNullOrEmpty(Function.ToString(dataRow["ValidFor"])) && !(Function.ToString(dataRow["ValidFor"]) == "N")) ? (text + "<Active>tYES</Active>") : (text + "<Active>tNO</Active>"));
			text += "<DiscountGroupLineCollection>";
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				text += "<DiscountGroupLine>";
				DataRow dataRow2 = dataTable.Rows[i];
				text = text + "<AbsEntry>" + Function.ParseInt(dataRow["AbsEntry"]) + "</AbsEntry>";
				if (!string.IsNullOrEmpty(Function.ToString(dataRow2["ObjType"])))
				{
					switch (Function.ParseInt(dataRow2["ObjType"]))
					{
					case 0:
						text = text + "<ObjectType>" + DiscountGroupBaseObjectEnum.dgboNone.ToString() + "</ObjectType>";
						break;
					case 52:
						text = text + "<ObjectType>" + DiscountGroupBaseObjectEnum.dgboItemGroups.ToString() + "</ObjectType>";
						break;
					case 8:
						text = text + "<ObjectType>" + DiscountGroupBaseObjectEnum.dgboItemProperties.ToString() + "</ObjectType>";
						break;
					case 43:
						text = text + "<ObjectType>" + DiscountGroupBaseObjectEnum.dgboManufacturer.ToString() + "</ObjectType>";
						break;
					case 4:
						text = text + "<ObjectType>" + DiscountGroupBaseObjectEnum.dgboItems.ToString() + "</ObjectType>";
						break;
					}
				}
				text = (string.IsNullOrEmpty(Function.ToString(dataRow2["ObjKey"])) ? (text + "<ObjectCode/>") : (text + "<ObjectCode>" + Function.RawText(dataRow2["ObjKey"]) + "</ObjectCode>"));
				if (!string.IsNullOrEmpty(Function.ToString(dataRow2["DiscType"])))
				{
					string text2 = Function.ToString(dataRow2["DiscType"]);
					if (!(text2 == "D"))
					{
						if (text2 == "P")
						{
							text = text + "<DiscountType>" + DiscountGroupDiscountTypeEnum.dgdt_Variable.ToString() + "</DiscountType>";
						}
					}
					else
					{
						text = text + "<DiscountType>" + DiscountGroupDiscountTypeEnum.dgdt_Fixed.ToString() + "</DiscountType>";
					}
				}
				text = text + "<Discount>" + Function.ParseDecimal(dataRow2["Discount"]) + "</Discount>";
				text = text + "<PaidQuantity>" + Function.ParseDecimal(dataRow2["PayFor"]) + "</PaidQuantity>";
				text = text + "<FreeQuantity>" + Function.ParseDecimal(dataRow2["ForFree"]) + "</FreeQuantity>";
				text = text + "<MaximumFreeQuantity>" + Function.ParseDecimal(dataRow2["UpTo"]) + "</MaximumFreeQuantity>";
				text += "</DiscountGroupLine>";
			}
			text += "</DiscountGroupLineCollection>";
			text += "</EnhancedDiscountGroup>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public void Dispose()
	{
	}

	public string Remove(string treeCode, string sessionId)
	{
		throw new NotImplementedException();
	}
}
