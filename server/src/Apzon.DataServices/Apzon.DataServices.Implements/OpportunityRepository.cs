using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.SalesOpportunity;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class OpportunityRepository : Repository<OpportunitiesImpl>, IOpportunity, IRepository<OpportunitiesImpl>
{
	protected readonly IUnitOfWork _unitOfWork;

	protected Apzon.Services.IUnitOfWork _serviceUnitOfWork;

	public OpportunityRepository(Apzon.Services.IUnitOfWork serUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
		_serviceUnitOfWork = serUnitOfWork;
	}

	public string Save(DataSet ds, string sessionId)
	{
		try
		{
			int num = Function.ParseInt(ds.Tables[0].Rows[0]["OpprId"]);
			List<AttachmentFile_Impl> lstFiles = TableToEntity(ds.Tables[7], sessionId);
			string text = ServiceUnitOfWork.Opportunity.SaveAttachmentFiles(lstFiles, num);
			if (!string.IsNullOrWhiteSpace(text) && text.Any((char t) => !char.IsDigit(t)))
			{
				return text;
			}
			ds.Tables[0].Rows[0]["AtcEntry"] = Function.ParseInt(text);
			if (num == 0)
			{
				return Add(ds, sessionId);
			}
			return Update(ds, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	protected virtual string Update(DataSet ds, string sessionId)
	{
		try
		{
			string text = sessionId.Split(',')[1];
			object obj = ds.Tables[0].Rows[0]["U_ItemDetail"];
			string text2 = "";
			int refItemDetailId = 0;
			if (obj == null || obj == DBNull.Value || string.IsNullOrWhiteSpace(obj.ToString()) || Function.ParseInt(obj) == 0)
			{
				AddItemDetail(ds.Tables[5], text, ref refItemDetailId);
			}
			else
			{
				refItemDetailId = Function.ParseInt(obj);
				text2 = MappingItemDetail(ds.Tables[5], text, "Update");
			}
			List<int> list = new List<int>();
			string text3 = string.Join(",", from t in ds.Tables[1].AsEnumerable()
				select Function.ParseInt(t["Line"]));
			if (string.IsNullOrEmpty(text3))
			{
				text3 = "-1";
			}
			DataTable dataTable = ExecuteDataTable("SELECT * FROM OPR1 WHERE \"OpprId\"=@DocEntry AND \"Line\" NOT IN (" + text3 + ");", CommandType.Text, new SqlParameter("@DocEntry", Function.ParseInt(ds.Tables[0].Rows[0]["OpprId"])));
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = ds.Tables[1];
				list.AddRange(from t in dataTable.AsEnumerable()
					select Function.ParseInt(t["Line"]));
				for (int num = 0; num < dataTable.Rows.Count; num++)
				{
					dataTable2.ImportRow(dataTable.Rows[num]);
				}
				dataTable2 = (from t in dataTable2.AsEnumerable()
					orderby Function.ParseInt(t["Line"])
					select t).CopyToDataTable();
				dataTable2.AcceptChanges();
				List<DataTable> list2 = new List<DataTable>();
				list2.Add(ds.Tables[0].Copy());
				list2.Add(dataTable2);
				for (int num2 = 2; num2 < ds.Tables.Count; num2++)
				{
					list2.Add(ds.Tables[num2].Copy());
				}
				ds = new DataSet();
				ds.Tables.AddRange(list2.ToArray());
			}
			string text4 = Mapping(ds, isUpdate: true, refItemDetailId, 0L);
			bool flag = CheckConfigForecast();
			string text5 = string.Empty;
			if (flag)
			{
				text5 = MappingForecast(ds, text, "UpdateObject");
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\"><env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                        <env:Header>\r\n                                            <SessionID>" + text + "</SessionID>\r\n                                        </env:Header><env:Body>\r\n                                        <dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text4 + "</dis:UpdateObject></env:Body></env:Envelope>" + text2 + " " + text5 + "</env:Envelopes>";
			XmlElement xmlElement = DataService.BatchProcess(xmlObject).Content as XmlElement;
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
					int num3 = Function.ParseInt(xElement3.Value);
					string text6 = DeleteLinkedAct(num3);
					if (ds.Tables[6].Rows.Count > 0)
					{
						foreach (DataRow row in ds.Tables[6].Rows)
						{
							text6 = UpdateLinkedAct(row);
							if (!string.IsNullOrEmpty(text6))
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text6);
							}
						}
					}
					text6 = DeleteInterestRange(ds.Tables[8].Rows.Count, num3);
					if (!string.IsNullOrEmpty(text6))
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text6);
					}
					text6 = ServiceUnitOfWork.Opportunity.ProcessDataReasons(ds.Tables[4], num3);
					if (!string.IsNullOrEmpty(text6))
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text6);
					}
				}
				if (list.Count > 0)
				{
					DeleteLines(ds, text, ApzObjectTypes.oSalesOpportunities, list);
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	private List<AttachmentFile_Impl> TableToEntity(DataTable dt, string sessionId)
	{
		List<AttachmentFile_Impl> list = new List<AttachmentFile_Impl>();
		try
		{
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				AttachmentFile_Impl item = (AttachmentFile_Impl)DataRowToEntity(new AttachmentFile_Impl(), dt.Rows[i]);
				list.Add(item);
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	private List<UserFieldsImpl> DataRowToUserFields(DataRow row, string tableName, string sessionId)
	{
		List<UserFieldsImpl> listField = _unitOfWork.UserFields.GetListField(tableName, sessionId);
		List<UserFieldsImpl> list = new List<UserFieldsImpl>();
		for (int i = 0; i < row.Table.Columns.Count; i++)
		{
			DataColumn col = row.Table.Columns[i];
			if (!col.ColumnName.StartsWith("U_"))
			{
				continue;
			}
			UserFieldsImpl userFieldsImpl = listField.FirstOrDefault((UserFieldsImpl u) => u.Name == col.ColumnName.Remove(0, 2));
			if (userFieldsImpl == null)
			{
				continue;
			}
			string name = col.DataType.Name;
			UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
			switch (name)
			{
			case "DateTime":
			{
				if (userFieldsImpl.SubType == BoFldSubTypes.st_Time)
				{
					userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
					userFieldsImpl2.Value = Function.ParseInt(row[col]).ToString("0000");
					break;
				}
				DateTime? dateTime = Function.ParseDateTime(row[col]);
				DateTime dateTime2;
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					dateTime2 = dateTime.Value;
				}
				else
				{
					dateTime2 = new DateTime(1900, 1, 1);
				}
				userFieldsImpl2.Value = Function.ToString(dateTime2);
				userFieldsImpl2.Type = BoFieldTypes.db_Date;
				break;
			}
			case "Int16":
			case "Int32":
			case "Int64":
				if (userFieldsImpl.SubType == BoFldSubTypes.st_Time)
				{
					userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
					userFieldsImpl2.Value = Function.ParseInt(row[col]).ToString("0000");
				}
				else
				{
					userFieldsImpl2.Value = Function.ToString(Function.ParseInt(row[col]));
					userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
				}
				break;
			case "Single":
			case "Double":
			case "Float":
				userFieldsImpl2.Value = Function.ToString(Function.ParseDouble(row[col]));
				userFieldsImpl2.Type = BoFieldTypes.db_Float;
				break;
			default:
				userFieldsImpl2.Value = Function.ToString(row[col]);
				userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
				break;
			}
			userFieldsImpl2.Name = col.ColumnName;
			list.Add(userFieldsImpl2);
		}
		return list;
	}

	private static object DataRowToEntity(object obj, DataRow row)
	{
		for (int i = 0; i < row.Table.Columns.Count; i++)
		{
			string columnName = row.Table.Columns[i].ColumnName;
			PropertyInfo property = obj.GetType().GetProperty(columnName);
			if (property == null)
			{
				continue;
			}
			switch (Function.GetTypeName(property.PropertyType).ToLower())
			{
			case "string":
				obj.GetType().GetProperty(columnName).SetValue(obj, Function.ToString(row[row.Table.Columns[i]]));
				break;
			case "double":
				obj.GetType().GetProperty(columnName).SetValue(obj, Function.ParseDouble(row[row.Table.Columns[i]]));
				break;
			case "int16":
				obj.GetType().GetProperty(columnName).SetValue(obj, Function.ParseShort(row[row.Table.Columns[i]]));
				break;
			case "int32":
				obj.GetType().GetProperty(columnName).SetValue(obj, Function.ParseInt(row[row.Table.Columns[i]]));
				break;
			case "int64":
				obj.GetType().GetProperty(columnName).SetValue(obj, Function.ParseLong(row[row.Table.Columns[i]]));
				break;
			case "datetime":
			{
				DateTime? dateTime = ((property == null) ? ((DateTime?)null) : Function.ParseDateTime(row[row.Table.Columns[i]]));
				PropertyInfo property2 = obj.GetType().GetProperty(columnName);
				DateTime dateTime2;
				if (dateTime.HasValue)
				{
					_ = dateTime.Value;
					dateTime2 = dateTime.Value;
				}
				else
				{
					dateTime2 = new DateTime(1900, 1, 1);
				}
				property2.SetValue(obj, dateTime2);
				break;
			}
			}
		}
		return obj;
	}

	public int GetForeCast(string code)
	{
		try
		{
			string query = "select AbsId from ofct where code=@Code";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text, new SqlParameter("@Code", code));
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["AbsId"]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	protected virtual string Add(DataSet ds, string sessionId)
	{
		try
		{
			string sesB1Ws = sessionId.Split(',')[0];
			string sessionId2 = sessionId.Split(',')[1];
			DataTable dataTable = ds.Tables[5];
			string text = string.Empty;
			int refItemDetailId = 0;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				text = AddItemDetail(dataTable, sessionId2, ref refItemDetailId);
			}
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
			string text2 = string.Empty;
			long forecastId = 0L;
			bool flag = CheckConfigForecast();
			if ((dataTable != null && dataTable.Rows.Count > 0) & flag)
			{
				text2 = AddForecast(ds, sesB1Ws, refItemDetailId, ref forecastId);
			}
			if (!string.IsNullOrEmpty(text2))
			{
				RemoveItemDetail(refItemDetailId, sessionId2);
				return text2;
			}
			string xmlObject = Mapping(ds, isUpdate: false, refItemDetailId, forecastId);
			XmlElement xmlElement = DataService.AddObject(ApzObjectTypes.oSalesOpportunities, sessionId2, xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					RemoveItemDetail(refItemDetailId, sessionId2);
					RemoveForecast(forecastId, sesB1Ws);
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int oprId = Function.ParseInt(xElement3.Value);
					string text3 = DeleteLinkedAct(oprId);
					if (ds.Tables[6].Rows.Count > 0)
					{
						foreach (DataRow row in ds.Tables[6].Rows)
						{
							text3 = UpdateLinkedAct(row);
							if (!string.IsNullOrEmpty(text3))
							{
								Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text3);
							}
						}
					}
					if (ds.Tables[8] != null && ds.Tables[8].Rows.Count > 0)
					{
						text3 = DeleteInterestRange(ds.Tables[8].Rows.Count, oprId);
						if (!string.IsNullOrEmpty(text3))
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), text3);
						}
					}
					return string.Empty;
				}
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	protected string DeleteInterestRange(int count, int oprId)
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select \"Line\" From OPR4 where \"OprId\"=@OprId order by \"Line\" asc;", CommandType.Text, new SqlParameter("@OprId", oprId));
			if (dataTable != null && dataTable.Rows.Count > 0 && dataTable.Rows.Count - count > 0)
			{
				string text = "";
				for (int i = count; i < dataTable.Rows.Count; i++)
				{
					text = text + Function.ToString(dataTable.Rows[i][0]) + ",";
				}
				if (text.EndsWith(","))
				{
					text = text.Remove(text.Length - 1);
				}
				if (ExecuteNonQuery("DELETE OPR4 WHERE \"OprId\"=@OprId AND \"Line\" in (" + text + ");", CommandType.Text, new SqlParameter("@OprId", oprId)) > 0)
				{
					return "";
				}
				return "No Interest Range deleted";
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	protected string AddForecast(DataSet ds, string sesB1Ws, int itemDetailEntry, ref long forecastId)
	{
		return string.Empty;
	}

	protected void RemoveForecast(long forecastId, string sesB1Ws)
	{
	}

	protected string DeleteLinkedAct(int OprId)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "APZ_SalesOpportunity_DeleteLinkedAct";
			list.Add(new SqlParameter("@OprId", OprId));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "DeleteLinkedAct FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected string UpdateLinkedAct(DataRow r)
	{
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string query = "APZ_SalesOpportunity_UpdateLinkedAct";
			list.Add(new SqlParameter("@ClgCode", r["ClgCode"]));
			list.Add(new SqlParameter("@OprId", r["OprId"]));
			list.Add(new SqlParameter("@OprLine", r["OprLine"]));
			if (ExecuteNonQuery(query, CommandType.StoredProcedure, list.ToArray()) > 0)
			{
				return "";
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "UpdateLinkedAct FAILED ", ex.Message);
			return ex.Message;
		}
	}

	protected bool CheckConfigForecast()
	{
		try
		{
			string query = "SELECT top 1 \"OppForecast\" FROM IRS_CONFIG;";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0]["OppForecast"]).Equals("Y");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	protected virtual string RemoveItemDetail(int itemDetailEntry, string sessionId)
	{
		try
		{
			string xmlObject = "<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                      <env:Header>\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                      </env:Header>\r\n                                      <env:Body>\r\n                                        <dis:Remove xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                          <Service>OPR6</Service>\r\n                                            <OpportunityItemDetail>\r\n                                                <DocEntry>" + itemDetailEntry + "</DocEntry>\r\n                                            </OpportunityItemDetail>\r\n                                        </dis:Remove>\r\n                                      </env:Body>\r\n                                    </env:Envelope>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
				return "Item detail - Unknown Error";
			}
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("DocEntry"))
			{
				if (xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("DocEntry")) != null)
				{
					return string.Empty;
				}
				return "Item detail - Could not get return Entry";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	protected virtual string AddItemDetail(DataTable dt, string sessionId, ref int refItemDetailId)
	{
		try
		{
			refItemDetailId = 0;
			string xmlObject = MappingItemDetail(dt, sessionId, "Add");
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					return xElement2.Value;
				}
				return "Item detail - Unknown Error";
			}
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("DocEntry"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("DocEntry"));
				if (xElement3 != null)
				{
					refItemDetailId = Function.ParseInt(xElement3.Value);
					return string.Empty;
				}
				return "Item detail - Could not get return Entry";
			}
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
	}

	protected virtual string MappingItemDetail(DataTable dt, string sessionId, string command)
	{
		if (dt.Rows.Count == 0)
		{
			return "";
		}
		string text = "<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                      <env:Header>\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                      </env:Header>\r\n                                      <env:Body>\r\n                                        <dis:" + command + " xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                          <Service>OPR6</Service>\r\n                                          <OpportunityItemDetail>";
		if (command.ToUpper().Equals("UPDATE") && Function.ParseInt(dt.Rows[0]["DocEntry"]) > 0)
		{
			text = text + "  <DocEntry>" + Function.ToString(dt.Rows[0]["DocEntry"]) + "</DocEntry>";
			text = text + "  <DocNum>" + Function.ToString(dt.Rows[0]["DocEntry"]) + "</DocNum>";
			text += "  <Series>-1</Series>";
		}
		text += " <OPR62Collection>";
		List<DataColumn> list = (from DataColumn c in dt.Columns
			select (c) into t
			where t.ColumnName.StartsWith("U_")
			select t).ToList();
		foreach (DataRow row in dt.Rows)
		{
			text += "<OPR62>";
			if (!string.IsNullOrEmpty(Function.ToString(row["ItemCode"])))
			{
				text = text + "<U_ItemCode>" + Function.RawText(row["ItemCode"]) + "</U_ItemCode>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["ItemName"])))
			{
				text = text + "<U_ItemName>" + Function.RawText(row["ItemName"]) + "</U_ItemName>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(row["FrgnName"])))
			{
				text = text + "<U_FrgnName>" + Function.RawText(row["FrgnName"]) + "</U_FrgnName>";
			}
			if (!Function.ParseDouble(row["Quantity"]).Equals(0.0))
			{
				text = text + "<U_Quantity>" + Function.ParseDouble(row["Quantity"]) + "</U_Quantity>";
			}
			if (!Function.ParseDouble(row["IPercent"]).Equals(0.0))
			{
				text = text + "<U_IPercent>" + Function.ParseDouble(row["IPercent"]) + "</U_IPercent>";
			}
			if (!Function.ParseDouble(row["Price"]).Equals(0.0))
			{
				text = text + "<U_Price>" + Function.ParseDouble(row["Price"]) + "</U_Price>";
			}
			if (!Function.ParseDouble(row["PotentAmount"]).Equals(0.0))
			{
				text = text + "<U_PotentAmount>" + Function.ParseDouble(row["PotentAmount"]) + "</U_PotentAmount>";
			}
			if (Function.ParseDateTime(row["DeliveryDate"]).HasValue && Function.ParseDateTime(row["DeliveryDate"]) != new DateTime(1900, 1, 1))
			{
				text = text + "<U_DeliveryDate>" + Convert.ToDateTime(row["DeliveryDate"]).ToString("yyyyMMdd") + "</U_DeliveryDate>";
			}
			try
			{
				if (!string.IsNullOrEmpty(Function.ToString(row["AttFile"])))
				{
					text = text + "<U_AttFile>" + Function.RawText(row["AttFile"]) + "</U_AttFile>";
				}
			}
			catch (Exception)
			{
			}
			string itemDetailUdfColumn = _serviceUnitOfWork.Opportunity.GetItemDetailUdfColumn();
			DataTable source = ExecuteDataTable(itemDetailUdfColumn, CommandType.Text, new SqlParameter("@TableName", "@OPR62"));
			foreach (DataColumn col in list)
			{
				DataRow dataRow2 = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["name"]).ToUpper().Equals(col.ColumnName.ToUpper()));
				if (dataRow2 == null)
				{
					continue;
				}
				switch (Function.ToString(dataRow2["datatype"]).ToLower())
				{
				case "numeric":
					text += string.Format("<{0}>{1}</{0}>", col.ColumnName, Function.ParseDouble(row[col]));
					break;
				case "datetime":
					if (Function.ParseDateTime(row[col]).HasValue)
					{
						text += string.Format("<{0}>{1}</{0}>", col.ColumnName, Convert.ToDateTime(row[col]).ToString("yyyyMMdd"));
					}
					break;
				case "int":
				case "smallint":
				case "tinyint":
				case "bigint":
					text += string.Format("<{0}>{1}</{0}>", col.ColumnName, Function.ParseInt(row[col]));
					break;
				case "bit":
					text += string.Format("<{0}>{1}</{0}>", col.ColumnName, Function.ParseBool(row[col]));
					break;
				default:
					text += string.Format("<{0}>{1}</{0}>", col.ColumnName, Function.RawText(row[col]));
					break;
				}
			}
			text += " </OPR62>";
		}
		return text + "</OPR62Collection>\r\n                                            </OpportunityItemDetail>\r\n                                        </dis:" + command + ">\r\n                                      </env:Body>\r\n                                    </env:Envelope>";
	}

	protected virtual string Mapping(DataSet ds, bool isUpdate = false, int itemDetailEntry = 0, long forecastId = 0L)
	{
		try
		{
			DataRow dataRow = ds.Tables[0].Rows[0];
			string text = "<BOM>\r\n                            <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                              <AdmInfo>\r\n                                <Object>oSalesOpportunities</Object>\r\n                              </AdmInfo>";
			if (isUpdate)
			{
				text = text + " <QueryParams><SequentialNo>" + Function.ParseInt(dataRow["OpprId"]) + "</SequentialNo></QueryParams>";
			}
			text += "  <SalesOpportunities>\r\n                                <row>";
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["CardCode"])))
			{
				text = text + "<CardCode>" + Function.RawText(dataRow["CardCode"]) + "</CardCode>";
			}
			if (Function.ParseInt(dataRow["SlpCode"]) != 0)
			{
				text = text + "<SalesPerson>" + Function.ParseInt(dataRow["SlpCode"]) + "</SalesPerson>";
			}
			if (Function.ParseInt(dataRow["CprCode"]) != 0)
			{
				text = text + "<ContactPerson>" + Function.ParseInt(dataRow["CprCode"]) + "</ContactPerson>";
			}
			if (Function.ParseInt(dataRow["Source"]) != 0)
			{
				text = text + "<Source>" + Function.ParseInt(dataRow["Source"]) + "</Source>";
			}
			if (Function.ParseInt(dataRow["IntCat1"]) != 0)
			{
				text = text + "<InterestField1>" + Function.ParseInt(dataRow["IntCat1"]) + "</InterestField1>";
			}
			if (Function.ParseInt(dataRow["IntCat2"]) != 0)
			{
				text = text + "<InterestField2>" + Function.ParseInt(dataRow["IntCat2"]) + "</InterestField2>";
			}
			if (Function.ParseInt(dataRow["IntCat3"]) != 0)
			{
				text = text + "<InterestField3>" + Function.ParseInt(dataRow["IntCat3"]) + "</InterestField3>";
			}
			if (Function.ParseInt(dataRow["IntRate"]) != 0)
			{
				text = text + "<InterestLevel>" + Function.ParseInt(dataRow["IntRate"]) + "</InterestLevel>";
			}
			if (Function.ParseDateTime(dataRow["OpenDate"]).HasValue)
			{
				text = text + "<StartDate>" + Convert.ToDateTime(dataRow["OpenDate"]).ToString("yyyyMMdd") + "</StartDate>";
			}
			if (Function.ParseDateTime(dataRow["PredDate"]).HasValue)
			{
				text = text + "<PredictedClosingDate>" + Convert.ToDateTime(dataRow["PredDate"]).ToString("yyyyMMdd") + "</PredictedClosingDate>";
			}
			if (!Function.ParseDouble(dataRow["PrcnProf"]).Equals(0.0))
			{
				text = text + "<GrossProfit>" + Function.ParseDouble(dataRow["PrcnProf"]) + "</GrossProfit>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["Memo"])))
			{
				text = text + "<Remarks>" + Function.RawText(dataRow["Memo"]) + "</Remarks>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["Status"])))
			{
				text = text + "<Status>" + Function.RawText(dataRow["Status"]) + "</Status>";
			}
			if (!Function.ParseInt(dataRow["Reason"]).Equals(0))
			{
				text = text + "<ReasonForClosing>" + Function.ToString(dataRow["Reason"]) + "</ReasonForClosing>";
			}
			if (!Function.ParseDouble(dataRow["MaxSumLoc"]).Equals(0.0))
			{
				text = text + "<TotalAmountLocal>" + Function.ParseDouble(dataRow["MaxSumLoc"]) + "</TotalAmountLocal>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["Name"])))
			{
				text = text + "<OpportunityName>" + Function.RawText(dataRow["Name"]) + "</OpportunityName>";
			}
			if (!Function.ParseInt(dataRow["Industry"]).Equals(0))
			{
				text = text + "<Industry>" + Function.ParseInt(dataRow["Industry"]) + "</Industry>";
			}
			if (Function.ParseInt(dataRow["Owner"]) > 0)
			{
				text = text + "<DataOwnershipfield>" + Function.ParseInt(dataRow["Owner"]) + "</DataOwnershipfield>";
				if (dataRow.IsExistsColumn("U_DataOwner"))
				{
					text = text + "<U_DataOwner>" + Function.ParseInt(dataRow["Owner"]) + "</U_DataOwner>";
				}
			}
			else
			{
				text += "<DataOwnershipfield></DataOwnershipfield>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["StatusRem"])))
			{
				text = text + "<StatusRemarks>" + Function.RawText(dataRow["StatusRem"]) + "</StatusRemarks>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["PrjCode"])))
			{
				text = text + "<ProjectCode>" + Function.RawText(dataRow["PrjCode"]) + "</ProjectCode>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["ChnCrdName"])))
			{
				text = text + "<BPChanelName>" + Function.RawText(dataRow["ChnCrdName"]) + "</BPChanelName>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["CardName"])))
			{
				text = text + "<CustomerName>" + Function.RawText(dataRow["CardName"]) + "</CustomerName>";
			}
			if (!Function.ParseInt(dataRow["Territory"]).Equals(0))
			{
				text = text + "<Territory>" + Function.ParseInt(dataRow["Territory"]) + "</Territory>";
			}
			if (Function.ParseDateTime(dataRow["CloseDate"]).HasValue)
			{
				text = text + "<ClosingDate>" + Convert.ToDateTime(dataRow["CloseDate"]).ToString("yyyyMMdd") + "</ClosingDate>";
			}
			if (!Function.ParseInt(dataRow["ChnCrdCon"]).Equals(0))
			{
				text = text + "<BPChannelContact>" + Function.ParseInt(dataRow["ChnCrdCon"]) + "</BPChannelContact>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["ChnCrdCode"])))
			{
				text = text + "<BPChanelCode>" + Function.RawText(dataRow["ChnCrdCode"]) + "</BPChanelCode>";
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["OpprType"])))
			{
				string text2 = (Function.RawText(dataRow["OpprType"]).Equals("R") ? "boOpSales" : "boOpPurchasing");
				text = text + "<OpportunityType>" + text2 + "</OpportunityType>";
			}
			if (dataRow.IsExistsColumn("U_Itemdetail"))
			{
				dataRow["U_Itemdetail"] = itemDetailEntry;
			}
			if (dataRow.IsExistsColumn("U_ForecastId"))
			{
				dataRow["U_ForecastId"] = forecastId;
			}
			text += DataService.MappingUdf(dataRow, (!isUpdate) ? EnumCommandType.Add : EnumCommandType.Update);
			text += "</row></SalesOpportunities>";
			if (ds.Tables[1].Rows.Count > 0)
			{
				text += "<SalesOpportunitiesLines>";
				for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
				{
					DataRow dataRow2 = ds.Tables[1].Rows[i];
					if (ExistsStage(i, Function.ParseInt(dataRow["OpprId"])) && i != ds.Tables[1].Rows.Count - 1)
					{
						text += "<row/>";
						continue;
					}
					text = text + "<row><LineNum>" + i + "</LineNum>";
					if (!Function.ParseInt(dataRow2["SlpCode"]).Equals(0))
					{
						text = text + "<SalesPerson>" + Function.ParseInt(dataRow2["SlpCode"]) + "</SalesPerson>";
					}
					if (Function.ParseDateTime(dataRow2["OpenDate"]).HasValue)
					{
						text = text + "<StartDate>" + Convert.ToDateTime(dataRow2["OpenDate"]).ToString("yyyyMMdd") + "</StartDate>";
					}
					if (Function.ParseDateTime(dataRow2["CloseDate"]).HasValue)
					{
						text = text + "<ClosingDate>" + Convert.ToDateTime(dataRow2["CloseDate"]).ToString("yyyyMMdd") + "</ClosingDate>";
					}
					if (!Function.ParseInt(dataRow2["Step_Id"]).Equals(0))
					{
						text = text + "<StageKey>" + Function.ParseInt(dataRow2["Step_Id"]) + "</StageKey>";
					}
					if (!Function.ParseDouble(dataRow2["ClosePrcnt"]).Equals(0.0))
					{
						text = text + "<PercentageRate>" + Function.ParseInt(dataRow2["ClosePrcnt"]) + "</PercentageRate>";
					}
					text = text + "<MaxLocalTotal>" + Function.ParseDouble(dataRow2["MaxSumLoc"]) + "</MaxLocalTotal>";
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["Memo"])))
					{
						text = text + "<Remarks>" + Function.RawText(dataRow2["Memo"]) + "</Remarks>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["ObjType"])))
					{
						string table = "";
						string text3 = Function.ToString(dataRow2["ObjType"]);
						string text4 = string.Empty;
						switch (text3)
						{
						case "13":
							text4 = "bodt_Invoice";
							table = "OINV";
							break;
						case "14":
							text4 = "bodt_CreditNote";
							table = "ORIN";
							break;
						case "15":
							text4 = "bodt_DeliveryNote";
							table = "ODLN";
							break;
						case "16":
							text4 = "bodt_Return";
							table = "ORDN";
							break;
						case "17":
							text4 = "bodt_Order";
							table = "ORDR";
							break;
						case "18":
							text4 = "bodt_PurchaseInvoice";
							table = "OPCH";
							break;
						case "19":
							text4 = "bodt_PurchaseCreditNote";
							table = "ORPC";
							break;
						case "20":
							text4 = "bodt_PurchaseDeliveryNote";
							table = "OPDN";
							break;
						case "21":
							text4 = "bodt_PurchaseReturn";
							table = "ORPD";
							break;
						case "22":
							text4 = "bodt_PurchaseOrder";
							table = "OPOR";
							break;
						case "23":
							text4 = "bodt_Quotation";
							table = "OQUT";
							break;
						case "163":
							text4 = "bodt_CorrectionAPInvoice";
							table = "OCPI";
							break;
						case "165":
							text4 = "bodt_CorrectionARInvoice";
							table = "OCSI";
							break;
						case "540000006":
							text4 = "bodt_PurchaseQutation";
							table = "OPQT";
							break;
						}
						if (!string.IsNullOrEmpty(text4))
						{
							text = text + "<DocumentType>" + text4 + "</DocumentType>";
						}
						string docEntry = GetDocEntry(Function.ParseInt(dataRow2["DocNumber"]), table);
						if (docEntry != "" && docEntry != "0")
						{
							text = text + "<DocumentNumber>" + docEntry + "</DocumentNumber>";
						}
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["DocChkbox"])))
					{
						string text5 = Function.ToString(dataRow2["DocChkbox"]);
						if (!(text5 == "N"))
						{
							if (text5 == "Y")
							{
								text += "<DocumentCheckbox>Y</DocumentCheckbox>";
							}
						}
						else
						{
							text += "<DocumentCheckbox>N</DocumentCheckbox>";
						}
					}
					if (!Function.ParseInt(dataRow2["CntctCode"]).Equals(0))
					{
						text = text + "<ContactPerson>" + Function.ParseInt(dataRow2["CntctCode"]) + "</ContactPerson>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["ChnCrdName"])))
					{
						text = text + "<BPChanelName>" + Function.RawText(dataRow2["ChnCrdName"]) + "</BPChanelName>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow2["ChnCrdCode"])))
					{
						text = text + "<BPChanelCode>" + Function.RawText(dataRow2["ChnCrdCode"]) + "</BPChanelCode>";
					}
					if (!Function.ParseInt(dataRow2["Owner"]).Equals(0) && !isUpdate)
					{
						text = text + "<DataOwnershipfield>" + Function.ParseInt(dataRow2["Owner"]) + "</DataOwnershipfield>";
					}
					if (!Function.ParseInt(dataRow2["ChnCrdCon"]).Equals(0))
					{
						text = text + "<BPChannelContact>" + Function.ParseInt(dataRow2["ChnCrdCon"]) + "</BPChannelContact>";
					}
					text += DataService.MappingUdf(dataRow2, (!isUpdate) ? EnumCommandType.Add : EnumCommandType.Update);
					text += "</row>";
				}
				text += "</SalesOpportunitiesLines>";
			}
			if (ds.Tables[2].Rows.Count > 0)
			{
				text += "<SalesOpportunitiesCompetition>";
				for (int j = 0; j < ds.Tables[2].Rows.Count; j++)
				{
					DataRow dataRow3 = ds.Tables[2].Rows[j];
					text += "<row>";
					if (!Function.ParseInt(dataRow3["CompetId"]).Equals(0))
					{
						text = text + "<Competition>" + Function.ParseInt(dataRow3["CompetId"]) + "</Competition>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow3["Memo"])))
					{
						text = text + "<Details>" + Function.RawText(dataRow3["Memo"]) + "</Details>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow3["Won"])))
					{
						text = text + "<WonOrLost>" + Function.RawText(dataRow3["Won"]) + "</WonOrLost>";
					}
					text += DataService.MappingUdf(dataRow3, (!isUpdate) ? EnumCommandType.Add : EnumCommandType.Update);
					text += "</row>";
				}
				text += "</SalesOpportunitiesCompetition>";
			}
			if (ds.Tables[3].Rows.Count > 0)
			{
				text += "<SalesOpportunitiesPartners>";
				for (int k = 0; k < ds.Tables[3].Rows.Count; k++)
				{
					DataRow dataRow4 = ds.Tables[3].Rows[k];
					text += "<row>";
					if (!Function.ParseInt(dataRow4["ParterId"]).Equals(0))
					{
						text = text + "<Partners>" + Function.ParseInt(dataRow4["ParterId"]) + "</Partners>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataRow4["Memo"])))
					{
						text = text + "<Details>" + Function.RawText(dataRow4["Memo"]) + "</Details>";
					}
					if (!Function.ParseInt(dataRow4["OrlCode"]).Equals(0))
					{
						text = text + "<RelationshipCode>" + Function.ParseInt(dataRow4["OrlCode"]) + "</RelationshipCode>";
					}
					text += DataService.MappingUdf(dataRow4, (!isUpdate) ? EnumCommandType.Add : EnumCommandType.Update);
					text += "</row>";
				}
				text += "</SalesOpportunitiesPartners>";
			}
			if (ds.Tables[8].Rows.Count > 0)
			{
				text += "<SalesOpportunitiesInterests>";
				for (int l = 0; l < ds.Tables[8].Rows.Count; l++)
				{
					DataRow dataRow5 = ds.Tables[8].Rows[l];
					text += "<row>";
					if (!Function.ParseInt(dataRow5["IntId"]).Equals(0))
					{
						text = text + "<InterestId>" + Function.ParseInt(dataRow5["IntId"]) + "</InterestId>";
					}
					text = ((!(Function.ToString(dataRow5["Prmry"]).ToUpper() == "Y")) ? (text + "<PrimaryInterest>tNO</PrimaryInterest>") : (text + "<PrimaryInterest>tYES</PrimaryInterest>"));
					text += "</row>";
				}
				text += "</SalesOpportunitiesInterests>";
			}
			return text + "</BO>\r\n                        </BOM>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return string.Empty;
		}
	}

	protected string GetDocEntry(int docNum, string table)
	{
		try
		{
			string query = "Select \"DocEntry\" From " + table + " WHERE \"DocNum\" = " + docNum + ";";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	protected bool ExistsStage(int stage, int p)
	{
		try
		{
			string query = "select * from opr1 where \"OpprId\"=" + p + " and \"Line\"=" + stage + ";";
			DataTable dataTable = ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return true;
		}
	}

	protected virtual string MappingForecast(DataSet ds, string sessionId, string command)
	{
		try
		{
			DataTable dataTable = ds.Tables[5];
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				string text = "FCS-OOP-" + Function.ToString(dataTable.Rows[0]["DocEntry"]);
				string text2 = string.Empty;
				if (!string.IsNullOrEmpty(Function.ToString(ds.Tables[0].Rows[0]["Name"])))
				{
					text2 = "FCS-OOP-" + Function.RawText(ds.Tables[0].Rows[0]["Name"]);
				}
				DateTime? dateTime = Function.ParseDateTime(dataTable.Compute("min(DeliveryDate)", ""));
				DateTime? dateTime2 = Function.ParseDateTime(dataTable.Compute("max(DeliveryDate)", ""));
				string text3 = "<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                      <env:Header>\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                      </env:Header>\r\n                                      <env:Body>\r\n                                        <dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                            <BOM>\r\n                                             <BO xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                                <AdmInfo>\r\n                                                <Object>oSalesForecast</Object>\r\n                                                </AdmInfo>\r\n                                                <QueryParams>\r\n                                                <Numerator>" + Function.ParseInt(ds.Tables[0].Rows[0]["U_ForecastId"]) + "</Numerator>\r\n                                                </QueryParams>\r\n                                                <SalesForecast>\r\n                                                <row>";
				if (dateTime.HasValue)
				{
					text3 = text3 + "<ForecastStartDate>" + Convert.ToDateTime(dateTime).ToString("yyyyMMdd") + "</ForecastStartDate>";
				}
				if (dateTime2.HasValue)
				{
					text3 = text3 + "<ForecastEndDate>" + Convert.ToDateTime(dateTime2).ToString("yyyyMMdd") + "</ForecastEndDate>";
				}
				if (!string.IsNullOrEmpty(text))
				{
					text3 = text3 + "<ForecastCode>" + text + "</ForecastCode>";
				}
				if (!string.IsNullOrEmpty(text2))
				{
					text3 = text3 + "<ForecastName>" + text2 + "</ForecastName>";
				}
				text3 += "</row>\r\n                                                </SalesForecast>\r\n                                                <SalesForecast_Lines>";
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					text3 += "<row>";
					if (!Function.ParseDouble(dataTable.Rows[i]["Quantity"]).Equals(0.0))
					{
						text3 = text3 + "<Quantity>" + Function.ParseDouble(dataTable.Rows[i]["Quantity"]) + "</Quantity>";
					}
					if (Function.ParseDateTime(dataTable.Rows[i]["DeliveryDate"]).HasValue)
					{
						text3 = text3 + "<ForecastedDay>" + Convert.ToDateTime(dataTable.Rows[i]["DeliveryDate"]).ToString("yyyyMMdd") + "</ForecastedDay>";
					}
					if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["ItemCode"])))
					{
						text3 = text3 + "<ItemNo>" + Function.RawText(dataTable.Rows[i]["ItemCode"]) + "</ItemNo>";
					}
					text3 += "</row>";
				}
				return text3 + "</SalesForecast_Lines>\r\n                                            </BO> \r\n                                            </BOM>\r\n                                            </dis:" + command + ">\r\n                                        </env:Body>\r\n                                    </env:Envelope>";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return string.Empty;
		}
		return string.Empty;
	}

	public virtual string DeleteDocs(string ids, string sessionId)
	{
		try
		{
			string text = sessionId.Split(',')[1];
			string[] array = ids.Split(',');
			if (array.Length != 0)
			{
				string text2 = string.Empty;
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					int num = Function.ParseInt(array2[i]);
					if (num != 0)
					{
						string text3 = "<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                             <env:Header>\r\n                                              <SessionID>" + text + "</SessionID>\r\n                                             </env:Header>\r\n                                             <env:Body>\r\n                                              <dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                               <BOM>\r\n                                                <BO>\r\n                                                 <AdmInfo>\r\n                                                  <Object>oSalesOpportunities</Object>\r\n                                                 </AdmInfo>\r\n                                                 <QueryParams><SequentialNo>" + num + "</SequentialNo></QueryParams>\r\n                                                </BO>\r\n                                               </BOM>\r\n                                              </dis:RemoveObject>\r\n                                            </env:Body>\r\n                                            </env:Envelope>";
						text2 += text3;
					}
				}
				XmlElement xmlElement = DataService.Process(text2).Content as XmlElement;
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
						Function.ParseInt(xElement3.Value);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return string.Empty;
		}
		return string.Empty;
	}

	public string GetSalesOpportunityByKey(string sessionId, ApzObjectTypes objType, string docEntry)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>SalesOpportunitiesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetByParams xmlns=\"SalesOpportunitiesService\">\r\n                                     <SalesOpportunities>   <SequentialNo>" + docEntry + "</SequentialNo></SalesOpportunities>\r\n                                    </GetByParams>  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>  ";
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

	protected virtual string DeleteLines(DataSet doc, string sessionId, ApzObjectTypes objType, List<int> lstRemoved)
	{
		try
		{
			string salesOpportunityByKey = GetSalesOpportunityByKey(sessionId, objType, Function.ToString(doc.Tables[0].Rows[0]["OpprId"]));
			if (string.IsNullOrEmpty(salesOpportunityByKey))
			{
				throw new Exception("[DI-SERVER] No matching record found.");
			}
			XElement xElement = XElement.Parse(salesOpportunityByKey).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("SalesOpportunities"));
			DataSet dataSet = new DataSet();
			using (StringReader reader = new StringReader(Function.ToString(xElement)))
			{
				dataSet.ReadXml(reader);
			}
			if (dataSet.Tables.Contains("SalesOpportunitiesLine"))
			{
				bool flag = false;
				DataTable dataTable = dataSet.Tables["SalesOpportunitiesLine"];
				if (lstRemoved.Count > 0)
				{
					flag = true;
					foreach (int i in lstRemoved)
					{
						DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]).Equals(i));
						if (dataRow != null)
						{
							dataTable.Rows.Remove(dataRow);
						}
					}
				}
				if (!flag)
				{
					return string.Empty;
				}
				string xml = dataSet.GetXml();
				if (!string.IsNullOrEmpty(xml))
				{
					string xmlObject = "<?xml version=\"1.0\" ?>\r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>SalesOpportunitiesService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Update xmlns=\"SalesOpportunitiesService\">" + xml + "</Update>\r\n                                </soap:Body>  \r\n                            </soap:Envelope> ";
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
}
