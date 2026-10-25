using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text;
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

public class UserFieldsRepository : Repository<UserFieldsImpl>, Apzon.DataServices.Interfaces.IUserFields, IRepository<UserFieldsImpl>
{
	protected readonly IUnitOfWork _unitOfWork;

	public UserFieldsRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public List<UserFieldsImpl> GetListField(string tableName, string sessionId)
	{
		try
		{
			SqlParameter[] parameters = new SqlParameter[1]
			{
				new SqlParameter("@TableId", tableName)
			};
			DataSet dataSet = ServiceUnitOfWork.Commons.ExecuteData("APZ_UserFields_GetListField", CommandType.StoredProcedure, parameters);
			if (dataSet.IsNotNull() && dataSet.Tables.Count > 1)
			{
				List<UserFieldsImpl> list = new List<UserFieldsImpl>();
				DataTable dataTable = dataSet.Tables[0];
				DataTable dataTable2 = dataSet.Tables[1];
				if (dataTable.IsNotNull())
				{
					for (int i = 0; i < dataTable.Rows.Count; i++)
					{
						DataRow dataRow = dataTable.Rows[i];
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						userFieldsImpl.DefaultValue = Function.ToString(dataRow["Dflt"]);
						userFieldsImpl.Description = Function.ToString(dataRow["Descr"]);
						userFieldsImpl.EditSize = Function.ParseLong(dataRow["EditSize"]);
						userFieldsImpl.FieldID = Function.ParseLong(dataRow["FieldID"]);
						userFieldsImpl.LinkedTable = Function.ToString(dataRow["RTable"]);
						userFieldsImpl.LinkedUDO = Function.ToString(dataRow["RelUDO"]);
						userFieldsImpl.LinkedObject = (dataRow.IsExistsColumn("RelSO") ? Function.ToString(dataRow["RelSO"]) : "");
						userFieldsImpl.Mandatory = (Function.ToString(dataRow["NotNull"]).Equals("Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
						userFieldsImpl.Name = Function.ToString(dataRow["AliasID"]);
						userFieldsImpl.Size = Function.ParseLong(dataRow["SizeID"]);
						userFieldsImpl.TableName = Function.ToString(dataRow["TableID"]);
						switch (Function.ToString(dataRow["TypeID"]))
						{
						case "A":
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
							break;
						case "M":
							userFieldsImpl.Type = BoFieldTypes.db_Memo;
							break;
						case "N":
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
							break;
						case "D":
							userFieldsImpl.Type = BoFieldTypes.db_Date;
							break;
						case "B":
							userFieldsImpl.Type = BoFieldTypes.db_Float;
							break;
						}
						switch (Function.ToString(dataRow["EditType"]))
						{
						case "S":
							userFieldsImpl.SubType = BoFldSubTypes.st_Sum;
							break;
						case "P":
							userFieldsImpl.SubType = BoFldSubTypes.st_Price;
							break;
						case "Q":
							userFieldsImpl.SubType = BoFldSubTypes.st_Quantity;
							break;
						case "%":
							userFieldsImpl.SubType = BoFldSubTypes.st_Percentage;
							break;
						case "?":
							userFieldsImpl.SubType = BoFldSubTypes.st_Address;
							break;
						case "T":
							userFieldsImpl.SubType = BoFldSubTypes.st_Time;
							break;
						case "R":
							userFieldsImpl.SubType = BoFldSubTypes.st_Rate;
							break;
						case "M":
							userFieldsImpl.SubType = BoFldSubTypes.st_Measurement;
							break;
						case "B":
							userFieldsImpl.SubType = BoFldSubTypes.st_Link;
							break;
						case "I":
							userFieldsImpl.SubType = BoFldSubTypes.st_Image;
							break;
						default:
							userFieldsImpl.SubType = BoFldSubTypes.st_None;
							break;
						}
						if (dataTable2.IsNotNull() && dataTable2.ExistsColumn("TableID") && dataTable2.ExistsColumn("FieldID"))
						{
							DataRow[] array = dataTable2.Select($"TableID='{userFieldsImpl.TableName}' AND FieldID='{userFieldsImpl.FieldID}'");
							if (array != null && array.Length != 0)
							{
								userFieldsImpl.ValidValues = new List<ValidValuesMDImpl>();
								DataRow[] array2 = array;
								foreach (DataRow dataRow2 in array2)
								{
									ValidValuesMDImpl validValuesMDImpl = new ValidValuesMDImpl();
									validValuesMDImpl.Value = Function.ToString(dataRow2["FldValue"]);
									validValuesMDImpl.Description = Function.ToString(dataRow2["Descr"]);
									userFieldsImpl.ValidValues.Add(validValuesMDImpl);
								}
							}
						}
						list.Add(userFieldsImpl);
					}
					return list;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return new List<UserFieldsImpl>();
	}

	public HttpResult Update(UserFieldsImpl field, string sessionId)
	{
		try
		{
			string xml = Mapping(field, "Update");
			return Process(xml, sessionId, field.TableName, Function.ParseInt(field.FieldID), "Update");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual HttpResult Process(string xml, string sessionId, string tableName, int fieldId, string command = "")
	{
		string xmlObject = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n                                        <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                      <env:Header>\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                      </env:Header>\r\n                                      <env:Body>" + xml + "\r\n                                      </env:Body>\r\n                                    </env:Envelope>";
		XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
		XElement xElement = XElement.Parse(xmlElement.OuterXml);
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
		{
			XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
			if (xElement2 != null)
			{
				return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
			}
		}
		else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
		{
			XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
			if (xElement3 != null)
			{
				int num = Function.ParseInt(xElement3.Value);
				return new HttpResult(Enums.MessageCode.Success, "", num);
			}
		}
		return new HttpResult();
	}

	public HttpResult Add(UserFieldsImpl field, string sessionId)
	{
		try
		{
			string xml = Mapping(field, "Add");
			return Process(xml, sessionId, field.TableName, Function.ParseInt(field.FieldID), "Add");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual string Mapping(UserFieldsImpl field, string command)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder("<dis:" + command + " xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                          <Service>UserFieldsMDService</Service>");
			stringBuilder.Append("<UserFieldMD>");
			stringBuilder.Append("<Name>" + field.Name + "</Name>");
			stringBuilder.Append("<Size>" + field.Size + "</Size>");
			stringBuilder.Append("<Description>" + field.Description + "</Description>");
			stringBuilder.Append("<TableName>" + field.TableName + "</TableName>");
			stringBuilder.Append("<FieldID>" + field.FieldID + "</FieldID>");
			stringBuilder.Append("<Mandatory>" + field.Mandatory.ToString() + "</Mandatory>");
			stringBuilder.Append("<Type>" + field.Type.ToString() + "</Type>");
			stringBuilder.Append("<SubType>" + field.SubType.ToString() + "</SubType>");
			stringBuilder.Append("<EditSize>" + field.EditSize + "</EditSize>");
			if (!string.IsNullOrEmpty(field.LinkedTable))
			{
				stringBuilder.Append("<LinkedTable>" + field.LinkedTable + "</LinkedTable>");
			}
			if (!string.IsNullOrEmpty(field.DefaultValue))
			{
				stringBuilder.Append("<DefaultValue>" + field.DefaultValue + "</DefaultValue>");
			}
			if (!string.IsNullOrEmpty(field.LinkedUDO))
			{
				stringBuilder.Append("<LinkedUDO>" + field.LinkedUDO + "</LinkedUDO>");
			}
			if (!string.IsNullOrEmpty(field.LinkedObject))
			{
				stringBuilder.Append("<LinkedSystemObject>" + field.LinkedObject + "</LinkedSystemObject>");
			}
			if (field.ValidValues != null && field.ValidValues.Count > 0)
			{
				stringBuilder.Append("<ValidValuesMD>");
				foreach (ValidValuesMDImpl validValue in field.ValidValues)
				{
					stringBuilder.Append("<ValidValueMD>");
					if (field.SubType == BoFldSubTypes.st_Time)
					{
						stringBuilder.Append("<Value>" + Function.ParseInt(validValue.Value.Replace(":", "")).ToString("0000") + "</Value>");
					}
					else
					{
						stringBuilder.Append("<Value>" + validValue.Value + "</Value>");
					}
					stringBuilder.Append("<Description>" + validValue.Description + "</Description>");
					stringBuilder.Append("</ValidValueMD>");
				}
				stringBuilder.Append("</ValidValuesMD>");
			}
			stringBuilder.Append("</UserFieldMD >\r\n                        </dis:" + command + ">");
			if (field.TableName.Equals("AITT") || field.TableName.Equals("ATT1"))
			{
				string text = (field.TableName.Equals("AITT") ? "APZ_OITT" : "APZ_ITT1");
				string text2;
				if (field.Type == BoFieldTypes.db_Numeric)
				{
					text2 = "int";
				}
				else if (field.Type == BoFieldTypes.db_Date)
				{
					text2 = "datetime";
				}
				else if (field.Type != BoFieldTypes.db_Alpha)
				{
					text2 = ((field.Type == BoFieldTypes.db_Float) ? "numeric(19,6)" : "nvarchar(max)");
				}
				else
				{
					text2 = $"nvarchar({field.EditSize})";
				}
				string sql = string.Format(" ALTER TABLE dbo.[{0}] {3} U_{1} {2} null", text, field.Name, text2, command.Equals("Add") ? "ADD" : "ALTER COLUMN");
				stringBuilder.Append(_unitOfWork.UDOs.GetXmlExecuteSQL(sql, "", isGetBody: true));
			}
			return stringBuilder.ToString();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public virtual string GetByParams(string sessionId, UserFieldsImpl field)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>UserFieldsMDService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <GetByParams xmlns=\"UserFieldsMDService\">\r\n                                        <UserFieldMDParams>\r\n                                            <TableName>" + field.TableName + "</TableName>\r\n                                            <FieldID>" + field.FieldID + "</FieldID>\r\n                                        </UserFieldMDParams>\r\n                                    </GetByParams>\r\n                                </soap:Body>\r\n                            </soap:Envelope>";
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

	public HttpResult Save(DataSet ds, string sessionId)
	{
		try
		{
			if (ds == null)
			{
				return new HttpResult(Enums.MessageCode.Error, "Dataset is null");
			}
			if (ds.Tables.Count != 5)
			{
				return new HttpResult(Enums.MessageCode.Error, "Dataset is null");
			}
			if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "DataTable - UDF is null or have no row");
			}
			if (ds.Tables[4] == null || ds.Tables[4].Rows.Count == 0)
			{
				return new HttpResult(Enums.MessageCode.Error, "DataTable - Action is null or have no row");
			}
			string text = Function.ToString(ds.Tables[4].Rows[0][0]);
			UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
			userFieldsImpl.TableName = Function.ToString(ds.Tables[0].Rows[0]["TableID"]);
			userFieldsImpl.FieldID = Function.ParseLong(ds.Tables[0].Rows[0]["FieldID"]);
			userFieldsImpl.Name = Function.ToString(ds.Tables[0].Rows[0]["AliasID"]);
			string byParams = GetByParams(sessionId, userFieldsImpl);
			if (text == "A")
			{
				if (!string.IsNullOrEmpty(byParams))
				{
					return new HttpResult(Enums.MessageCode.Error, "FieldID '" + userFieldsImpl.FieldID + "' Existed");
				}
				if (ExistsFieldByColumnName(userFieldsImpl.TableName, userFieldsImpl.Name))
				{
					return new HttpResult(Enums.MessageCode.Error, "Field name '" + userFieldsImpl.Name + "' Existed");
				}
			}
			else
			{
				if (!(text == "U"))
				{
					return new HttpResult(Enums.MessageCode.Error, "Invalid action '" + text + "'. Must in 'A', 'B' ");
				}
				if (string.IsNullOrEmpty(byParams))
				{
					return new HttpResult(Enums.MessageCode.Error, "FieldID '" + userFieldsImpl.FieldID + "' does not Existed");
				}
				if (!ExistsFieldByColumnName(userFieldsImpl.TableName, userFieldsImpl.Name))
				{
					return new HttpResult(Enums.MessageCode.Error, "Field name '" + userFieldsImpl.Name + "' does not Existed");
				}
			}
			UserFieldsImpl field = DataSetToUDFEntity(ds);
			if (text == "A")
			{
				return Add(field, sessionId);
			}
			return Update(field, sessionId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private UserFieldsImpl DataSetToUDFEntity(DataSet ds)
	{
		try
		{
			UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
			DataRow dataRow = ds.Tables[0].Rows[0];
			userFieldsImpl.DefaultValue = Function.ToString(dataRow["Dflt"]);
			userFieldsImpl.Description = Function.ToString(dataRow["Descr"]);
			userFieldsImpl.FieldID = Function.ParseLong(dataRow["FieldID"]);
			userFieldsImpl.LinkedTable = Function.ToString(dataRow["RTable"]);
			userFieldsImpl.LinkedUDO = Function.ToString(dataRow["RelUDO"]);
			userFieldsImpl.LinkedObject = Function.ToString(dataRow["RelSO"]);
			string strA = Function.ToString(dataRow["TypeID"]);
			string text = Function.ToString(dataRow["EditType"]);
			if (string.Compare(strA, "B", ignoreCase: true) == 0)
			{
				if (text == null || text.Length != 0)
				{
					switch (text)
					{
					case "S":
						userFieldsImpl.SubType = BoFldSubTypes.st_Sum;
						userFieldsImpl.Size = 16L;
						userFieldsImpl.EditSize = 16L;
						break;
					case "P":
						userFieldsImpl.SubType = BoFldSubTypes.st_Price;
						userFieldsImpl.Size = 16L;
						userFieldsImpl.EditSize = 16L;
						break;
					case "Q":
						userFieldsImpl.SubType = BoFldSubTypes.st_Quantity;
						userFieldsImpl.Size = 16L;
						userFieldsImpl.EditSize = 16L;
						break;
					case "%":
						userFieldsImpl.SubType = BoFldSubTypes.st_Percentage;
						userFieldsImpl.Size = 16L;
						userFieldsImpl.EditSize = 16L;
						break;
					case "M":
						userFieldsImpl.SubType = BoFldSubTypes.st_Measurement;
						userFieldsImpl.Size = 16L;
						userFieldsImpl.EditSize = 16L;
						break;
					}
				}
				else
				{
					userFieldsImpl.SubType = BoFldSubTypes.st_Rate;
					userFieldsImpl.Size = 10L;
					userFieldsImpl.EditSize = 10L;
				}
				userFieldsImpl.Type = BoFieldTypes.db_Float;
			}
			else if (string.Compare(strA, "N", ignoreCase: true) == 0)
			{
				if (string.Compare(text, "", ignoreCase: true) == 0)
				{
					userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					userFieldsImpl.SubType = BoFldSubTypes.st_None;
					userFieldsImpl.Size = Function.ParseLong(dataRow["SizeID"]);
					userFieldsImpl.EditSize = Function.ParseLong(dataRow["EditSize"]);
				}
				else if (string.Compare(text, "T", ignoreCase: true) == 0)
				{
					userFieldsImpl.Type = BoFieldTypes.db_Date;
					userFieldsImpl.SubType = BoFldSubTypes.st_Time;
					userFieldsImpl.Size = 6L;
					userFieldsImpl.EditSize = 6L;
				}
			}
			else if (string.Compare(strA, "A", ignoreCase: true) == 0)
			{
				if (!(text == "I"))
				{
					if (text == null || text.Length != 0)
					{
						if (!(text == "?"))
						{
							if (text == "#")
							{
								userFieldsImpl.SubType = BoFldSubTypes.st_Phone;
								userFieldsImpl.Size = 20L;
								userFieldsImpl.EditSize = 20L;
							}
						}
						else
						{
							userFieldsImpl.SubType = BoFldSubTypes.st_Address;
							userFieldsImpl.Size = 254L;
							userFieldsImpl.EditSize = 254L;
						}
					}
					else
					{
						userFieldsImpl.SubType = BoFldSubTypes.st_None;
						userFieldsImpl.Size = Function.ParseLong(dataRow["SizeID"]);
						userFieldsImpl.EditSize = Function.ParseLong(dataRow["EditSize"]);
					}
				}
				else
				{
					userFieldsImpl.SubType = BoFldSubTypes.st_Image;
					userFieldsImpl.Size = 200L;
					userFieldsImpl.EditSize = 200L;
				}
				userFieldsImpl.Type = BoFieldTypes.db_Alpha;
			}
			else if (string.Compare(strA, "M", ignoreCase: true) == 0)
			{
				if (text == null || text.Length != 0)
				{
					if (text == "B")
					{
						userFieldsImpl.SubType = BoFldSubTypes.st_Link;
						userFieldsImpl.Size = 4L;
						userFieldsImpl.EditSize = 254L;
					}
				}
				else
				{
					userFieldsImpl.SubType = BoFldSubTypes.st_None;
					userFieldsImpl.Size = 4L;
					userFieldsImpl.EditSize = 10L;
				}
				userFieldsImpl.Type = BoFieldTypes.db_Memo;
			}
			else if (string.Compare(strA, "D", ignoreCase: true) == 0)
			{
				userFieldsImpl.SubType = BoFldSubTypes.st_None;
				userFieldsImpl.Type = BoFieldTypes.db_Date;
				userFieldsImpl.Size = 8L;
				userFieldsImpl.EditSize = 8L;
			}
			string text2 = Function.ToString(dataRow["NotNull"]);
			userFieldsImpl.Mandatory = ((text2 == "Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO);
			userFieldsImpl.Name = Function.ToString(dataRow["AliasID"]);
			userFieldsImpl.TableName = Function.ToString(dataRow["TableID"]);
			List<ValidValuesMDImpl> list = new List<ValidValuesMDImpl>();
			for (int i = 0; i < ds.Tables[1].Rows.Count; i++)
			{
				ValidValuesMDImpl validValuesMDImpl = new ValidValuesMDImpl();
				validValuesMDImpl.Description = Function.ToString(ds.Tables[1].Rows[i]["Descr"]);
				validValuesMDImpl.Value = Function.ToString(ds.Tables[1].Rows[i]["FldValue"]);
				list.Add(validValuesMDImpl);
			}
			userFieldsImpl.ValidValues = list;
			return userFieldsImpl;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	private bool ExistsFieldByColumnName(string TableID, string AliasID)
	{
		try
		{
			SqlParameter[] parameters = new SqlParameter[2]
			{
				new SqlParameter("@TableID", TableID),
				new SqlParameter("@AliasID", AliasID)
			};
			DataTable dataTable = ServiceUnitOfWork.Commons.ExecuteDataTable("select * from cufd where \"TableID\"=@TableID and \"AliasID\"=@AliasID;", CommandType.Text, parameters);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public virtual string RemoveUdf(string tableID, int fieldID, string sessionId)
	{
		try
		{
			if (!tableID.Equals("AITT"))
			{
				tableID.Equals("ATT1");
			}
			string xmlObject = "<?xml version=\"1.0\" ?>  \r\n                            <soap:Envelope xmlns:soap=\"http://schemas.xmlsoap.org/soap/envelope/\"\r\n                                            xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\"\r\n                                            xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\r\n                                <soap:Header>  \r\n                                    <MsgHeader xmlns=\"http://www.sap.com/SBO/DIS\">\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                        <ServiceName>UserFieldsMDService</ServiceName>\r\n                                    </MsgHeader>\r\n                                </soap:Header>\r\n                                <soap:Body>\r\n                                    <Remove xmlns=\"UserFieldsMDService\">\r\n                                        <UserFieldMDParams>\r\n                                            <TableName>" + tableID + "</TableName>\r\n                                            <FieldID>" + fieldID + "</FieldID>\r\n                                        </UserFieldMDParams>\r\n                                    </Remove></soap:Body>\r\n                            </soap:Envelope>";
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
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}
}
