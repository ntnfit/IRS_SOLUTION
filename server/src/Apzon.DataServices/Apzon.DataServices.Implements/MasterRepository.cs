using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class MasterRepository : Repository<DataTable>, IMaster, IRepository<DataTable>
{
	protected readonly IUnitOfWork _unitOfWork;

	public MasterRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public MasterRepository(Apzon.Services.IUnitOfWork serviceWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public virtual HttpResult Remove(object code, string sessionId, ApzObjectTypes objType, string keyTag = "")
	{
		try
		{
			if (DataService.GetBusinessObjectTemplate(sessionId, objType.ToString()).Content is XmlElement xmlElement)
			{
				XElement xElement = XElement.Parse(xmlElement.OuterXml).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("QueryParams"));
				DataSet dataSet = new DataSet();
				using StringReader reader = new StringReader(Function.ToString(xElement));
				dataSet.ReadXml(reader);
				if (dataSet.Tables.Count > 0)
				{
					DataTable dataTable = dataSet.Tables[0];
					if (dataTable.Columns.Count > 0 && string.IsNullOrWhiteSpace(keyTag))
					{
						keyTag = dataTable.Columns[0].ColumnName;
					}
				}
			}
			string xmlRequest = BuildXml(sessionId, MappingRemove(code, objType, keyTag));
			return ProcessData(xmlRequest, objType.ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual HttpResult ProcessData(string xmlRequest, string objType = "")
	{
		try
		{
			if (!(DataService.Process(xmlRequest).Content is XmlElement xmlElement))
			{
				return new HttpResult();
			}
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Operation error for object " + objType + ": " + xElement2.Value);
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
						Logging.Write("WATCH", "Operation completed successfully for object " + objType + " : " + string.Join(",", list2.ToArray()));
						return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
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

	public virtual HttpResult Save(DataTable dt, string sessionId, ApzObjectTypes objType, string keyFieldName, Dictionary<string, string> dicFieldTags, bool deleteOldRows = false, Enums.ImportActionEnum actionType = Enums.ImportActionEnum.AddUpdate, string tableName = "", string rowTag = "", string keyTag = "", object parameter = null)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(tableName))
			{
				List<string> tableName2 = Function.GetTableName(objType);
				if (tableName2.Count > 0)
				{
					tableName = tableName2[0];
				}
			}
			if (DataService.GetBusinessObjectTemplate(sessionId, objType.ToString()).Content is XmlElement xmlElement)
			{
				XElement xElement = XElement.Parse(xmlElement.OuterXml).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("BO"));
				DataSet dataSet = new DataSet();
				using (StringReader reader = new StringReader(Function.ToString(xElement)))
				{
					dataSet.ReadXml(reader);
					if (dataSet.Tables.Count > 0)
					{
						int num = 0;
						for (int num2 = 0; num2 < dataSet.Tables.Count; num2++)
						{
							if (dataSet.Tables[num2].TableName.Equals("QueryParams"))
							{
								num = num2;
							}
						}
						if (num > 0 && dataSet.Tables.Count > num + 1)
						{
							DataTable dataTable = dataSet.Tables[num + 1];
							if (string.IsNullOrWhiteSpace(rowTag))
							{
								rowTag = dataTable.TableName;
							}
						}
					}
				}
				XElement xElement2 = XElement.Parse(xmlElement.OuterXml).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("QueryParams"));
				DataSet dataSet2 = new DataSet();
				using StringReader reader2 = new StringReader(Function.ToString(xElement2));
				dataSet2.ReadXml(reader2);
				if (dataSet2.Tables.Count > 0)
				{
					DataTable dataTable2 = dataSet2.Tables[0];
					if (dataTable2.Columns.Count > 0 && string.IsNullOrWhiteSpace(keyTag))
					{
						keyTag = dataTable2.Columns[0].ColumnName;
					}
				}
			}
			string text = string.Empty;
			if (dt != null)
			{
				if (deleteOldRows)
				{
					DataTable docs = GetDocs(tableName, parameter);
					if (docs == new DataTable())
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Save Data Error! Table Name not found! Object Type:" + objType);
						return new HttpResult(Enums.MessageCode.Error, "Table Name not found! Object Type:" + objType);
					}
					foreach (DataRow row in docs.Rows)
					{
						if (dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t[keyFieldName]) == Function.ToString(row[keyFieldName]) && Function.ParseInt(row[keyFieldName]) != 0) == null)
						{
							text += MappingRemove(Function.ParseInt(row[keyFieldName]), objType, keyTag);
						}
					}
				}
				for (int num3 = 0; num3 < dt.Rows.Count; num3++)
				{
					switch (actionType)
					{
					case Enums.ImportActionEnum.Add:
						if (!Exists(Function.ToString(dt.Rows[num3][keyFieldName]), tableName, keyFieldName))
						{
							text += Mapping(dt.Rows[num3], (!Exists(Function.ToString(dt.Rows[num3][keyFieldName]), tableName, keyFieldName)) ? EnumCommandType.Add : EnumCommandType.Update, objType, keyTag, keyFieldName, rowTag, dicFieldTags);
						}
						break;
					case Enums.ImportActionEnum.Update:
						if (Exists(Function.ToString(dt.Rows[num3][keyFieldName]), tableName, keyFieldName))
						{
							text += Mapping(dt.Rows[num3], (!Exists(Function.ToString(dt.Rows[num3][keyFieldName]), tableName, keyFieldName)) ? EnumCommandType.Add : EnumCommandType.Update, objType, keyTag, keyFieldName, rowTag, dicFieldTags);
						}
						break;
					default:
						text += Mapping(dt.Rows[num3], (!Exists(Function.ToString(dt.Rows[num3][keyFieldName]), tableName, keyFieldName)) ? EnumCommandType.Add : EnumCommandType.Update, objType, keyTag, keyFieldName, rowTag, dicFieldTags);
						break;
					}
				}
				string xmlRequest = BuildXml(sessionId, text);
				return ProcessData(xmlRequest, objType.ToString());
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual DataTable GetDocs(string tableName, object parameter = null)
	{
		try
		{
			string query = "Select * from " + tableName + ";";
			return ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Get Docs  FAILED ", ex.Message);
			return new DataTable();
		}
	}

	public virtual string MappingRemove(object code, ApzObjectTypes objType, string keyTag)
	{
		return string.Format("<dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                        <BOM>\r\n                          <BO>\r\n                            <AdmInfo>\r\n                                <Object>" + objType.ToString() + "</Object>\r\n                            </AdmInfo>\r\n                            <QueryParams><{0} >" + Function.RawText(code) + "</{0} ></QueryParams>\r\n                            </BO>\r\n                        </BOM>\r\n                        </dis:RemoveObject>", keyTag);
	}

	protected virtual string Mapping(DataRow doc, EnumCommandType commandType, ApzObjectTypes objType, string keyTag, string keyFieldName, string rowTag, Dictionary<string, string> dicFieldTags)
	{
		string text = "";
		text = ((commandType != EnumCommandType.Update) ? (text + "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">") : (text + "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">"));
		text = text + "<BOM>\r\n                          <BO>\r\n                          <AdmInfo>\r\n                            <Object>" + objType.ToString() + "</Object>\r\n                          </AdmInfo>";
		try
		{
			if (commandType == EnumCommandType.Update)
			{
				text += string.Format("<QueryParams><{0}>" + Function.RawText(doc[keyFieldName]) + "</{0}></QueryParams>", keyTag);
			}
			text = text + "<" + rowTag + ">";
			text += "<row>";
			foreach (KeyValuePair<string, string> dicFieldTag in dicFieldTags)
			{
				object obj = doc[dicFieldTag.Value];
				switch (doc.Table.Columns[dicFieldTag.Value].DataType.Name.ToLower())
				{
				case "int16":
					text = (string.IsNullOrEmpty(Function.ToString(obj)) ? (text + "<" + dicFieldTag.Key + ">-1</" + dicFieldTag.Key + ">") : (text + "<" + dicFieldTag.Key + ">" + Function.ParseShort(obj) + "</" + dicFieldTag.Key + ">"));
					break;
				case "int32":
					text = (string.IsNullOrEmpty(Function.ToString(obj)) ? (text + "<" + dicFieldTag.Key + ">-1</" + dicFieldTag.Key + ">") : (text + "<" + dicFieldTag.Key + ">" + Function.ParseInt(obj) + "</" + dicFieldTag.Key + ">"));
					break;
				case "int64":
					text = (string.IsNullOrEmpty(Function.ToString(obj)) ? (text + "<" + dicFieldTag.Key + ">-1</" + dicFieldTag.Key + ">") : (text + "<" + dicFieldTag.Key + ">" + Function.ParseLong(obj) + "</" + dicFieldTag.Key + ">"));
					break;
				case "datetime":
				{
					DateTime? dateTime = Function.ParseDateTime(obj);
					if (dateTime > new DateTime(1900, 1, 1))
					{
						string text2 = dateTime.Value.ToString("yyyyMMdd");
						text = text + "<" + dicFieldTag.Key + ">" + text2 + "</" + dicFieldTag.Key + ">";
					}
					break;
				}
				case "decimal":
					text = text + "<" + dicFieldTag.Key + ">" + Function.ParseDecimal(obj) + "</" + dicFieldTag.Key + ">";
					break;
				case "double":
					text = text + "<" + dicFieldTag.Key + ">" + Function.ParseDecimal(obj) + "</" + dicFieldTag.Key + ">";
					break;
				case "float":
					text = text + "<" + dicFieldTag.Key + ">" + Function.ParseDecimal(obj) + "</" + dicFieldTag.Key + ">";
					break;
				default:
					text = text + "<" + dicFieldTag.Key + ">" + Function.RawText(obj) + "</" + dicFieldTag.Key + ">";
					break;
				}
			}
			text += CustomMapping(doc);
			IEnumerable<string> enumerable = from t in (from DataColumn c in doc.Table.Columns
					select c.ColumnName).ToList()
				where t.StartsWith("U_")
				select t;
			if (enumerable != null && enumerable.ToList() != null && enumerable.ToList().Count > 0)
			{
				List<UserFieldsImpl> list = new List<UserFieldsImpl>();
				for (int num = 0; num < enumerable.ToList().Count; num++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					if (doc.Table.Columns[enumerable.ToList()[num]].DataType == typeof(DateTime))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(doc[enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (doc.Table.Columns[enumerable.ToList()[num]].DataType == typeof(double))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDecimal(doc[enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Value = Function.ToString(Function.ToString(doc[enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					userFieldsImpl.Name = enumerable.ToList()[num];
					list.Add(userFieldsImpl);
				}
				text += DataService.MappingUdf(list);
			}
			text += "</row>";
			text = text + "</" + rowTag + ">";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		text += "</BO></BOM>";
		if (commandType == EnumCommandType.Update)
		{
			return text + "</dis:UpdateObject>";
		}
		return text + "</dis:AddObject>";
	}

	protected virtual string CustomMapping(DataRow dr)
	{
		return "";
	}

	protected bool Exists(string code, string tableName, string keyFieldName)
	{
		try
		{
			if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(tableName) || string.IsNullOrWhiteSpace(keyFieldName))
			{
				return false;
			}
			return !string.IsNullOrEmpty(ExecuteScarlar("select 1 from " + tableName + " where \"" + keyFieldName + "\" = @docEntry;", CommandType.Text, new SqlParameter("@docEntry", code)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public virtual string BuildXml(string sessionId, string body)
	{
		return "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                             <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n                             </env:Header>\r\n                             <env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                              " + body + "\r\n                             </env:Body>\r\n                            </env:Envelope>";
	}

	public virtual HttpResult RemoveService(int code, string sessionId, ApzObjectTypes objType, string serviceName, string deleteMethodTag = "", string paramsTag = "", string keyTag = "")
	{
		try
		{
			if (DataService.GetServiceMetaData(sessionId, serviceName).Content is XmlElement xmlElement)
			{
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				XElement xElement2 = XElement.Parse(xmlElement.OuterXml).Descendants().FirstOrDefault((XElement t) =>
				{
					XAttribute xAttribute = t.Attribute("code");
					return xAttribute != null && t.Name.LocalName.Equals("Command") && xAttribute.Value == "200";
				});
				DataSet dataSet = new DataSet();
				DataSet dataSet2 = new DataSet();
				string rowTag = "";
				using (StringReader reader = new StringReader(Function.ToString(xElement2)))
				{
					dataSet2.ReadXml(reader);
					if (dataSet2.Tables.Count > 0)
					{
						for (int num = 0; num < dataSet2.Tables.Count; num++)
						{
							DataTable dataTable = dataSet2.Tables[num];
							if (dataTable.TableName.Equals("Input"))
							{
								if (dataTable.Columns.Contains("data") && dataTable.IsNotNull() && string.IsNullOrWhiteSpace(paramsTag))
								{
									paramsTag = Function.ToString(dataTable.Rows[0]["data"]);
								}
							}
							else if (dataTable.TableName.Equals("Output") && dataTable.Columns.Contains("data") && dataTable.IsNotNull() && string.IsNullOrWhiteSpace(rowTag))
							{
								rowTag = Function.ToString(dataTable.Rows[0]["data"]);
							}
						}
					}
				}
				using (StringReader reader2 = new StringReader(Function.ToString(xElement)))
				{
					dataSet.ReadXml(reader2);
					if (dataSet.Tables.Count > 0)
					{
						for (int num2 = 0; num2 < dataSet.Tables.Count; num2++)
						{
							DataTable dataTable2 = dataSet.Tables[num2];
							if (!dataTable2.TableName.Equals("Command") || !dataTable2.Columns.Contains("code") || !dataTable2.Columns.Contains("name"))
							{
								continue;
							}
							for (int num3 = 0; num3 < dataTable2.Rows.Count; num3++)
							{
								DataRow dataRow = dataTable2.Rows[num3];
								if (Function.ToString(dataRow["code"]) == "203" && string.IsNullOrWhiteSpace(deleteMethodTag))
								{
									deleteMethodTag = Function.ToString(dataRow["name"]);
								}
							}
						}
					}
				}
				XElement xElement3 = XElement.Parse(xmlElement.OuterXml).Descendants().FirstOrDefault((XElement t) =>
				{
					XAttribute xAttribute = t.Attribute("item_name");
					return xAttribute != null && t.Name.LocalName.Equals("Data") && xAttribute.Value == rowTag;
				});
				DataSet dataSet3 = new DataSet();
				using StringReader reader3 = new StringReader(Function.ToString(xElement3));
				dataSet3.ReadXml(reader3);
				if (dataSet3.Tables.Count > 0)
				{
					for (int num4 = 0; num4 < dataSet3.Tables.Count; num4++)
					{
						DataTable dataTable3 = dataSet3.Tables[num4];
						if (dataTable3.TableName.Equals("Property") && dataTable3.Columns.Contains("name") && dataTable3.IsNotNull() && string.IsNullOrWhiteSpace(keyTag))
						{
							keyTag = Function.ToString(dataTable3.Rows[0]["name"]);
						}
					}
				}
			}
			string xmlRequest = BuildXmlService(sessionId, serviceName, MappingRemoveService(code, serviceName, deleteMethodTag, paramsTag, keyTag));
			return ProcessData(xmlRequest);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual HttpResult SaveService(DataTable dt, string sessionId, ApzObjectTypes objType, string serviceName, string keyFieldName, Dictionary<string, string> dicFieldTags, bool deleteOldRows = false, Enums.ImportActionEnum actionType = Enums.ImportActionEnum.AddUpdate, string tableName = "", string getMethodTag = "", string addMethodTag = "", string updateMethodTag = "", string deleteMethodTag = "", string paramsTag = "", string keyTag = "", string rowTag = "")
	{
		try
		{
			if (string.IsNullOrWhiteSpace(tableName))
			{
				List<string> tableName2 = Function.GetTableName(objType);
				if (tableName2.Count > 0)
				{
					tableName = tableName2[0];
				}
			}
			if (DataService.GetServiceMetaData(sessionId, serviceName).Content is XmlElement xmlElement)
			{
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				XElement xElement2 = XElement.Parse(xmlElement.OuterXml).Descendants().FirstOrDefault((XElement t) =>
				{
					XAttribute xAttribute = t.Attribute("code");
					return xAttribute != null && t.Name.LocalName.Equals("Command") && xAttribute.Value == "200";
				});
				DataSet dataSet = new DataSet();
				DataSet dataSet2 = new DataSet();
				using (StringReader reader = new StringReader(Function.ToString(xElement2)))
				{
					dataSet2.ReadXml(reader);
					if (dataSet2.Tables.Count > 0)
					{
						for (int num = 0; num < dataSet2.Tables.Count; num++)
						{
							DataTable dataTable = dataSet2.Tables[num];
							if (dataTable.TableName.Equals("Input"))
							{
								if (dataTable.Columns.Contains("data") && dataTable.IsNotNull() && string.IsNullOrWhiteSpace(paramsTag))
								{
									paramsTag = Function.ToString(dataTable.Rows[0]["data"]);
								}
							}
							else if (dataTable.TableName.Equals("Output") && dataTable.Columns.Contains("data") && dataTable.IsNotNull() && string.IsNullOrWhiteSpace(rowTag))
							{
								rowTag = Function.ToString(dataTable.Rows[0]["data"]);
							}
						}
					}
				}
				using (StringReader reader2 = new StringReader(Function.ToString(xElement)))
				{
					dataSet.ReadXml(reader2);
					if (dataSet.Tables.Count > 0)
					{
						for (int num2 = 0; num2 < dataSet.Tables.Count; num2++)
						{
							DataTable dataTable2 = dataSet.Tables[num2];
							if (!dataTable2.TableName.Equals("Command") || !dataTable2.Columns.Contains("code") || !dataTable2.Columns.Contains("name"))
							{
								continue;
							}
							for (int num3 = 0; num3 < dataTable2.Rows.Count; num3++)
							{
								DataRow dataRow = dataTable2.Rows[num3];
								switch (Function.ToString(dataRow["code"]))
								{
								case "200":
									if (string.IsNullOrWhiteSpace(getMethodTag))
									{
										getMethodTag = Function.ToString(dataRow["name"]);
									}
									break;
								case "201":
									if (string.IsNullOrWhiteSpace(addMethodTag))
									{
										addMethodTag = Function.ToString(dataRow["name"]);
									}
									break;
								case "202":
									if (string.IsNullOrWhiteSpace(updateMethodTag))
									{
										updateMethodTag = Function.ToString(dataRow["name"]);
									}
									break;
								case "203":
									if (string.IsNullOrWhiteSpace(deleteMethodTag))
									{
										deleteMethodTag = Function.ToString(dataRow["name"]);
									}
									break;
								}
							}
						}
					}
				}
				XElement xElement3 = XElement.Parse(xmlElement.OuterXml).Descendants().FirstOrDefault((XElement t) =>
				{
					XAttribute xAttribute = t.Attribute("item_name");
					return xAttribute != null && t.Name.LocalName.Equals("Data") && xAttribute.Value == paramsTag;
				});
				DataSet dataSet3 = new DataSet();
				using StringReader reader3 = new StringReader(Function.ToString(xElement3));
				dataSet3.ReadXml(reader3);
				if (dataSet3.Tables.Count > 0)
				{
					for (int num4 = 0; num4 < dataSet3.Tables.Count; num4++)
					{
						DataTable dataTable3 = dataSet3.Tables[num4];
						if (dataTable3.TableName.Equals("Property") && dataTable3.Columns.Contains("name") && dataTable3.IsNotNull() && string.IsNullOrWhiteSpace(keyTag))
						{
							keyTag = Function.ToString(dataTable3.Rows[0]["name"]);
						}
					}
				}
			}
			string text = string.Empty;
			if (dt != null && dt.Rows.Count > 0)
			{
				if (deleteOldRows)
				{
					foreach (DataRow row in GetDocs(tableName).Rows)
					{
						if (dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t[keyFieldName]) == Function.ToString(row[keyFieldName]) && Function.ParseInt(row[keyFieldName]) != 0) == null)
						{
							text += MappingRemoveService(Function.ParseInt(row[keyFieldName]), serviceName, deleteMethodTag, paramsTag, keyTag);
						}
					}
				}
				for (int num5 = 0; num5 < dt.Rows.Count; num5++)
				{
					switch (actionType)
					{
					case Enums.ImportActionEnum.Add:
						if (!ExistsService(Function.ToString(dt.Rows[num5][keyFieldName]), sessionId, serviceName, getMethodTag, paramsTag, keyTag))
						{
							text += MappingService(dt.Rows[num5], (!ExistsService(Function.ToString(dt.Rows[num5][keyFieldName]), sessionId, serviceName, getMethodTag, paramsTag, keyTag)) ? EnumCommandType.Add : EnumCommandType.Update, serviceName, addMethodTag, updateMethodTag, rowTag, dicFieldTags);
						}
						break;
					case Enums.ImportActionEnum.Update:
						if (ExistsService(Function.ToString(dt.Rows[num5][keyFieldName]), sessionId, serviceName, getMethodTag, paramsTag, keyTag))
						{
							text += MappingService(dt.Rows[num5], (!ExistsService(Function.ToString(dt.Rows[num5][keyFieldName]), sessionId, serviceName, getMethodTag, paramsTag, keyTag)) ? EnumCommandType.Add : EnumCommandType.Update, serviceName, addMethodTag, updateMethodTag, rowTag, dicFieldTags);
						}
						break;
					default:
						text += MappingService(dt.Rows[num5], (!ExistsService(Function.ToString(dt.Rows[num5][keyFieldName]), sessionId, serviceName, getMethodTag, paramsTag, keyTag)) ? EnumCommandType.Add : EnumCommandType.Update, serviceName, addMethodTag, updateMethodTag, rowTag, dicFieldTags);
						break;
					}
				}
				string xmlRequest = BuildXmlService(sessionId, serviceName, text);
				return ProcessData(xmlRequest);
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string MappingRemoveService(object code, string serviceName, string deleteMethodTag, string paramsTag, string keyTag)
	{
		return "<" + deleteMethodTag + " xmlns=\"" + serviceName + "\">\r\n                                     <" + paramsTag + ">   <" + keyTag + ">" + Function.RawText(code) + "</" + keyTag + "></" + paramsTag + ">\r\n                                    </" + deleteMethodTag + ">  ";
	}

	private string MappingService(DataRow doc, EnumCommandType commandType, string serviceName, string addMethodTag, string updateMethodTag, string rowTag, Dictionary<string, string> dicFieldTags)
	{
		string text = "";
		text = ((commandType != EnumCommandType.Update) ? (text + "<" + addMethodTag + " xmlns=\"" + serviceName + "\">") : (text + "<" + updateMethodTag + " xmlns=\"" + serviceName + "\">"));
		try
		{
			text = text + "<" + rowTag + ">";
			foreach (KeyValuePair<string, string> dicFieldTag in dicFieldTags)
			{
				object obj = doc[dicFieldTag.Value];
				switch (doc.Table.Columns[dicFieldTag.Value].DataType.Name.ToLower())
				{
				case "int16":
					text = (string.IsNullOrEmpty(Function.ToString(obj)) ? (text + "<" + dicFieldTag.Key + " >-1</" + dicFieldTag.Key + ">") : (text + "<" + dicFieldTag.Key + " >" + Function.ParseShort(obj) + "</" + dicFieldTag.Key + ">"));
					break;
				case "int32":
					text = (string.IsNullOrEmpty(Function.ToString(obj)) ? (text + "<" + dicFieldTag.Key + " >-1</" + dicFieldTag.Key + ">") : (text + "<" + dicFieldTag.Key + " >" + Function.ParseInt(obj) + "</" + dicFieldTag.Key + ">"));
					break;
				case "int64":
					text = (string.IsNullOrEmpty(Function.ToString(obj)) ? (text + "<" + dicFieldTag.Key + " >-1</" + dicFieldTag.Key + ">") : (text + "<" + dicFieldTag.Key + " >" + Function.ParseLong(obj) + "</" + dicFieldTag.Key + ">"));
					break;
				case "datetime":
				{
					DateTime dateTime = Function.ParseDateTime(obj) ?? new DateTime(1900, 1, 1);
					text = text + "<" + dicFieldTag.Key + ">" + dateTime.ToString() + "</" + dicFieldTag.Key + ">";
					break;
				}
				case "decimal":
					text = text + "<" + dicFieldTag.Key + ">" + Function.ParseDecimal(obj) + "</" + dicFieldTag.Key + ">";
					break;
				case "double":
					text = text + "<" + dicFieldTag.Key + ">" + Function.ParseDecimal(obj) + "</" + dicFieldTag.Key + ">";
					break;
				case "float":
					text = text + "<" + dicFieldTag.Key + ">" + Function.ParseDecimal(obj) + "</" + dicFieldTag.Key + ">";
					break;
				default:
					text = text + "<" + dicFieldTag.Key + ">" + Function.RawText(obj) + "</" + dicFieldTag.Key + ">";
					break;
				}
			}
			IEnumerable<string> enumerable = from t in (from DataColumn c in doc.Table.Columns
					select c.ColumnName).ToList()
				where t.StartsWith("U_")
				select t;
			if (enumerable != null && enumerable.ToList() != null && enumerable.ToList().Count > 0)
			{
				List<UserFieldsImpl> list = new List<UserFieldsImpl>();
				for (int num = 0; num < enumerable.ToList().Count; num++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					if (doc.Table.Columns[enumerable.ToList()[num]].DataType == typeof(DateTime))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(doc[enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (doc.Table.Columns[enumerable.ToList()[num]].DataType == typeof(double))
					{
						userFieldsImpl.Value = Function.ToString(Function.ParseDecimal(doc[enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Value = Function.ToString(Function.ToString(doc[enumerable.ToList()[num]]));
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					userFieldsImpl.Name = enumerable.ToList()[num];
					list.Add(userFieldsImpl);
				}
				text += DataService.MappingUdf(list);
			}
			text = text + "</" + rowTag + ">";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		if (commandType == EnumCommandType.Update)
		{
			return text + "</" + updateMethodTag + ">";
		}
		return text + "</" + addMethodTag + ">";
	}

	public bool ExistsService(string p, string sessionId, string serviceName, string getMethodTag, string paramsTag, string keyTag)
	{
		try
		{
			string body = "<" + getMethodTag + " xmlns=\"" + serviceName + "\">\r\n                                     <" + paramsTag + ">  \r\n                                        <" + keyTag + ">" + p + "</" + keyTag + ">\r\n                                    </" + paramsTag + ">\r\n                              </" + getMethodTag + ">";
			string xmlObject = BuildXmlService(sessionId, serviceName, body);
			XmlElement xmlElement = DataService.GetByKey(sessionId, xmlObject).Content as XmlElement;
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	private string BuildXmlService(string sessionId, string serviceName, string body)
	{
		return "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>" + serviceName + "</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    " + body + "  \r\n                                </soap:Body>  \r\n                            </soap:Envelope>";
	}
}
