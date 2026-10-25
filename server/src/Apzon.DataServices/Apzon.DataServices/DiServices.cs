using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.DiServerServiceWebRef;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Microsoft.VisualBasic;
using SAPbobsCOM;

namespace Apzon.DataServices;

public class DiServices : IDataService
{
	private bool isResetLicense;

	private bool isComexception;

	public HttpResult GetBusinessObjectTemplate(string sessionId, string objType)
	{
		using DiServerServiceSoapClient diServerServiceSoapClient = BrokerDisService.GetUserDiServer(sessionId);
		return new HttpResult
		{
			Content = diServerServiceSoapClient.GetBusinessObjectTemplate(sessionId, objType)
		};
	}

	public HttpResult GetServiceMetaData(string sessionId, string servicename)
	{
		using DiServerServiceSoapClient diServerServiceSoapClient = BrokerDisService.GetUserDiServer(sessionId);
		return new HttpResult
		{
			Content = diServerServiceSoapClient.GetServiceMetaData(sessionId, servicename)
		};
	}

	public HttpResult BatchProcess(string xmlObject, int trytime = 0)
	{
		string text = "";
		string text2 = "";
		try
		{
			text = GetCurrentSessionId(xmlObject);
			text2 = DiConnectionPool.OpenConnection(text);
			if (!string.IsNullOrEmpty(text2) && !text.Equals(text2))
			{
				xmlObject = UpdateSessionId(text2, xmlObject);
			}
			using DiServerServiceSoapClient diServerServiceSoapClient = BrokerDisService.GetUserDiServer(text2);
			xmlObject = UpdateSessionId(text2, xmlObject);
			XmlElement ret = diServerServiceSoapClient.BatchProcess(xmlObject);
			return new HttpResult
			{
				Content = CheckError(xmlObject, ret, trytime)
			};
		}
		finally
		{
			GC.Collect();
		}
	}

	public HttpResult Process(string xmlObject, int trytime = 0)
	{
		string text = "";
		string text2 = "";
		try
		{
			if (isComexception)
			{
				Thread.Sleep(6000);
				return Process(xmlObject);
			}
			text = GetCurrentSessionId(xmlObject);
			text2 = DiConnectionPool.OpenConnection(text);
			if (!string.IsNullOrEmpty(text2) && !text.Equals(text2))
			{
				xmlObject = UpdateSessionId(text2, xmlObject);
			}
			using DiServerServiceSoapClient diServerServiceSoapClient = BrokerDisService.GetUserDiServer(text2);
			XmlElement ret = diServerServiceSoapClient.Process(xmlObject);
			return new HttpResult
			{
				Content = CheckError(xmlObject, ret, trytime)
			};
		}
		catch (Exception ex)
		{
			if (!ex.ToString().ToLower().Contains("comexception"))
			{
				throw;
			}
			isComexception = true;
			Thread.Sleep(6000);
			DiConnectionPool.ReConnect(text2);
			isComexception = false;
			XmlElement content = TryProcess(xmlObject, null, trytime, ProcessType.Process, out var error);
			if (!string.IsNullOrEmpty(error))
			{
				throw;
			}
			return new HttpResult
			{
				Content = content
			};
		}
		finally
		{
			isComexception = false;
			GC.Collect();
		}
	}

	public HttpResult AddObject(ApzObjectTypes objectTypes, string sessionId, string xmlObject, int trytime = 0, bool isTryProcess = false)
	{
		try
		{
			string sessionId2 = DiConnectionPool.OpenConnection(sessionId);
			using DiServerServiceSoapClient diServerServiceSoapClient = BrokerDisService.GetUserDiServer(sessionId2);
			XmlElement ret = diServerServiceSoapClient.AddObject(sessionId2, xmlObject);
			return new HttpResult
			{
				Content = CheckError(xmlObject, ret, trytime)
			};
		}
		catch (Exception ex)
		{
			if (!ex.ToString().ToLower().Contains("comexception"))
			{
				throw;
			}
			Thread.Sleep(2000);
			DiConnectionPool.ReConnect(sessionId);
			XmlElement content = TryProcess(xmlObject, null, trytime, ProcessType.Process, out var error);
			if (!string.IsNullOrEmpty(error))
			{
				throw;
			}
			return new HttpResult
			{
				Content = content
			};
		}
		finally
		{
			GC.Collect();
		}
	}

	public HttpResult UpdateObject(ApzObjectTypes objectTypes, string sessionId, string xmlObject, object objectKey = null, int trytime = 0, bool isTryProcess = false)
	{
		try
		{
			string sessionId2 = DiConnectionPool.OpenConnection(sessionId);
			using DiServerServiceSoapClient diServerServiceSoapClient = BrokerDisService.GetUserDiServer(sessionId2);
			XmlElement ret = diServerServiceSoapClient.UpdateObject(sessionId2, xmlObject);
			return new HttpResult
			{
				Content = CheckError(xmlObject, ret, trytime)
			};
		}
		catch (Exception ex)
		{
			if (!ex.ToString().ToLower().Contains("comexception"))
			{
				throw;
			}
			Thread.Sleep(2000);
			DiConnectionPool.ReConnect(sessionId);
			XmlElement content = TryProcess(xmlObject, null, trytime, ProcessType.Process, out var error);
			if (!string.IsNullOrEmpty(error))
			{
				throw;
			}
			return new HttpResult
			{
				Content = content
			};
		}
		finally
		{
			GC.Collect();
		}
	}

	public HttpResult CloseObject(ApzObjectTypes objectType, string sessionId, object objectKey)
	{
		throw new Exception("Function not support");
	}

	public HttpResult RemoveObject(ApzObjectTypes objectType, string sessionId, object objectKey)
	{
		throw new Exception("Function not support");
	}

	public HttpResult AddServiceObject(ApzObjectTypes objectTypes, string sessionId, string xmlObject, int trytime = 0, bool isTryProcess = false)
	{
		throw new Exception("Function not support");
	}

	public HttpResult UpdateServiceObject(ApzObjectTypes objectTypes, string sessionId, string xmlObject, object objectKey = null, int trytime = 0, bool isTryProcess = false)
	{
		throw new Exception("Function not support");
	}

	public HttpResult RemoveServiceObject(ApzObjectTypes objectType, string sessionId, string jsonObject)
	{
		throw new Exception("Function not support");
	}

	public HttpResult GetByKey(string sessionId, string xmlObject, int trytime = 0)
	{
		string sessionId2 = "";
		try
		{
			sessionId2 = DiConnectionPool.OpenConnection(sessionId);
			using DiServerServiceSoapClient diServerServiceSoapClient = BrokerDisService.GetUserDiServer(sessionId2);
			xmlObject = UpdateSessionId(sessionId2, xmlObject);
			XmlElement byKey = diServerServiceSoapClient.GetByKey(sessionId2, xmlObject);
			return new HttpResult
			{
				Content = CheckError(xmlObject, byKey, trytime)
			};
		}
		catch (Exception ex)
		{
			if (!ex.ToString().ToLower().Contains("comexception"))
			{
				throw;
			}
			Thread.Sleep(2000);
			DiConnectionPool.ReConnect(sessionId2);
			XmlElement content = TryProcess(xmlObject, null, trytime, ProcessType.GetByKey, out var error);
			if (!string.IsNullOrEmpty(error))
			{
				throw;
			}
			return new HttpResult
			{
				Content = content
			};
		}
		finally
		{
			GC.Collect();
		}
	}

	public HttpResult GetByKey(string sessionId, ApzObjectTypes objType, int docentry)
	{
		try
		{
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                            <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                             <env:Header>\r\n                              <SessionID>" + sessionId + "</SessionID> \r\n                             </env:Header>\r\n                             <env:Body>\r\n                              <dis:GetByKey xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                               <Object>" + objType.ToString() + "</Object> \r\n                               <DocEntry>" + docentry + "</DocEntry> \r\n                              </dis:GetByKey>\r\n                             </env:Body>\r\n                            </env:Envelope>";
			using DiServerServiceSoapClient diServerServiceSoapClient = BrokerDisService.GetUserDiServer(sessionId);
			XmlElement byKey = diServerServiceSoapClient.GetByKey(sessionId, xmlObject);
			return new HttpResult
			{
				Content = byKey
			};
		}
		catch (Exception ex)
		{
			return new HttpResult
			{
				Message = ex.Message
			};
		}
		finally
		{
			GC.Collect();
		}
	}

	public HttpResult Login(string dataBaseServer, string dataBaseName, string dataBaseType, string dataBaseUserName, string dataBasePassword, string companyUserName, string companyPassword, string language, string licenseServer)
	{
		try
		{
			BrokerDisService.SetListDis();
			using DiServerServiceSoapClient diServerServiceSoapClient = BrokerDisService.GetAvaiableDiServer();
			string text = diServerServiceSoapClient.Login(dataBaseServer, dataBaseName, dataBaseType, dataBaseUserName, dataBasePassword, companyUserName, companyPassword, language, licenseServer);
			if (text.Contains("License is fully used Login") || text.Contains("Connection with license server failed Login"))
			{
				bool.TryParse(ConfigurationManager.AppSettings["LicenseAutoResetEnabled"], out var licenseAutoResetEnabled);
				string licenseApi = Function.ToString(ConfigurationManager.AppSettings["LicenseAPI"]);
				if (!licenseAutoResetEnabled || !Uri.TryCreate(licenseApi, UriKind.Absolute, out var licenseApiUri))
				{
					Logging.Write("WATCH", text, "Automatic license reset is disabled or LicenseAPI is invalid");
				}
				else if (!isResetLicense)
				{
					isResetLicense = true;
					Logging.Write("WATCH", text, "Start auto reset license Service...");
					HttpClient httpClient = new HttpClient();
					httpClient.BaseAddress = licenseApiUri;
					httpClient.GetAsync(new Uri(licenseApiUri, "/api/Commons/ResetLicenseService"));
					Thread.Sleep(6000);
					isResetLicense = false;
					text = diServerServiceSoapClient.Login(dataBaseServer, dataBaseName, dataBaseType, dataBaseUserName, dataBasePassword, companyUserName, companyPassword, language, licenseServer);
				}
				else
				{
					Thread.Sleep(6000);
					text = diServerServiceSoapClient.Login(dataBaseServer, dataBaseName, dataBaseType, dataBaseUserName, dataBasePassword, companyUserName, companyPassword, language, licenseServer);
				}
			}
			if (!string.IsNullOrEmpty(text) && !text.ToUpper().Contains("ERROR"))
			{
				string ipServer = Function.ToString(diServerServiceSoapClient.Endpoint.Address.Uri).Split('/')[2];
				BrokerDisService.UserDisList[text] = ipServer;
				if (!BrokerDisService.ListDis.Any((KeyValuePair<string, int> m) => m.Key.Equals(ipServer)))
				{
					BrokerDisService.ListDis[ipServer] = 0;
				}
				int value = BrokerDisService.ListDis[ipServer] + 1;
				BrokerDisService.ListDis[ipServer] = value;
				if (BrokerDisService.ListDis.Count > 1)
				{
					Logging.Write("WATCH", "Login DIS:" + ipServer, companyUserName);
				}
			}
			return new HttpResult
			{
				Content = text
			};
		}
		catch (Exception ex)
		{
			if (ex.Message.ToLower().Contains("comexception"))
			{
				isResetLicense = true;
				Logging.Write("WATCH", "COMException", "Auto reset DI Service...");
				BrokerDisService.ExecuteBatFile(Function.ToString(ConfigurationManager.AppSettings["ResetDIBatFile"]));
				Thread.Sleep(6000);
				isResetLicense = false;
				Logging.Write("WATCH", "Auto reset DI Service done!", "Auto Relogin...");
				return Login(dataBaseServer, dataBaseName, dataBaseType, dataBaseUserName, dataBasePassword, companyUserName, companyPassword, language, licenseServer);
			}
			isResetLicense = false;
			return new HttpResult
			{
				Content = "[ERROR] " + ex.Message
			};
		}
		finally
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
	}

	public object LogOut(string sSessionId)
	{
		try
		{
			string text = DiConnectionPool.OpenConnection(sSessionId);
			DiServerServiceSoapClient userDiServer = BrokerDisService.GetUserDiServer(text);
			userDiServer.LogOut(sSessionId);
			object result = userDiServer.LogOut(text);
			DiConnectionPool.CloseConnection(sSessionId);
			BrokerDisService.CloseDiServer(text);
			return result;
		}
		finally
		{
			GC.Collect();
			GC.WaitForPendingFinalizers();
			GC.Collect();
		}
	}

	public string GetCurrentSessionId(string xmlObject)
	{
		try
		{
			XElement xElement = XElement.Parse(xmlObject).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("SessionID"));
			if (xElement != null)
			{
				return xElement.Value;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return string.Empty;
	}

	public string UpdateSessionId(string sessionId, string xmlObject)
	{
		try
		{
			XElement xElement = XElement.Parse(xmlObject);
			if (xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("SessionID")) != null)
			{
				xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("SessionID")).Value = sessionId;
			}
			return "<?xml version=\"1.0\" encoding=\"UTF-16\"?>\n" + xElement;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return xmlObject;
		}
	}

	private XmlElement CheckError(string xmlObject, XmlElement ret, int trytime)
	{
		XElement xElement = XElement.Parse(ret.OuterXml);
		if (!string.IsNullOrEmpty(ret.OuterXml) && ret.OuterXml.Contains("Fault"))
		{
			XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
			if (xElement2 != null)
			{
				string error;
				if (IsInvalidSessionId(xElement2.Value, trytime))
				{
					Logging.Write("WATCH", "invalid session id", "Auto Relogin...");
					return TryProcess(xmlObject, ret, trytime, ProcessType.Process, out error);
				}
				if (IsCriticalCache(xElement2.Value, trytime))
				{
					if (!isResetLicense)
					{
						isResetLicense = true;
						Logging.Write("WATCH", "Critical cache refresh failure", "Auto reset DI Service...");
						BrokerDisService.ExecuteBatFile(Function.ToString(ConfigurationManager.AppSettings["ResetDIBatFile"]));
						Thread.Sleep(6000);
						isResetLicense = false;
						Logging.Write("WATCH", "Auto reset DI Service done!", "Auto Relogin...");
						return TryProcess(xmlObject, ret, trytime, ProcessType.Process, out error);
					}
					Thread.Sleep(6000);
					return TryProcess(xmlObject, ret, trytime, ProcessType.Process, out error);
				}
			}
		}
		return ret;
	}

	private bool IsInvalidSessionId(XmlElement ret, int trytime)
	{
		XElement xElement = XElement.Parse(ret.OuterXml);
		if (!string.IsNullOrEmpty(ret.OuterXml) && ret.OuterXml.Contains("Fault"))
		{
			XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
			if (xElement2 != null && trytime == 0 && xElement2.Value.ToLower().Contains("invalid session id"))
			{
				return true;
			}
		}
		return false;
	}

	private bool IsInvalidSessionId(string ret, int trytime)
	{
		if (trytime <= 1 && ret.ToLower().Contains("invalid session id"))
		{
			return true;
		}
		return false;
	}

	private bool IsCriticalCache(string ret, int trytime)
	{
		if (trytime == 0 && (ret.ToLower().Contains("critical cache refresh failure") || ret.ToLower().Contains("failed to create business service")))
		{
			return true;
		}
		return false;
	}

	private XmlElement TryProcess(string xmlObject, XmlElement ret, int trytime, ProcessType type, out string error)
	{
		error = string.Empty;
		XElement xElement = XElement.Parse(xmlObject).Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("SessionID"));
		if (xElement != null)
		{
			string value = xElement.Value;
			BrokerDisService.CloseDiServer(value);
			string text = DiConnectionPool.ReConnect(value);
			if (!string.IsNullOrEmpty(text))
			{
				switch (type)
				{
				case ProcessType.AddObject:
					return AddObject(ApzObjectTypes.oNone, text, xmlObject, 1).Content as XmlElement;
				case ProcessType.UpdateObject:
					return UpdateObject(ApzObjectTypes.oNone, text, xmlObject, 1).Content as XmlElement;
				case ProcessType.GetByKey:
					return GetByKey(text, xmlObject, 1).Content as XmlElement;
				case ProcessType.Process:
				case ProcessType.BatchProcess:
				{
					XElement xElement2 = XElement.Parse(xmlObject);
					xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("SessionID")).Value = text;
					switch (type)
					{
					case ProcessType.Process:
						return Process("<?xml version=\"1.0\" encoding=\"UTF-16\"?>\n" + xElement2, ++trytime).Content as XmlElement;
					case ProcessType.BatchProcess:
						return BatchProcess("<?xml version=\"1.0\" encoding=\"UTF-16\"?>\n" + xElement2, ++trytime).Content as XmlElement;
					}
					break;
				}
				}
			}
		}
		error = "Could not process";
		return ret;
	}

	private string GetParseResultFromSoapAnswer(string sSoaPans)
	{
		string text = "";
		XmlValidatingReader xmlValidatingReader = new XmlValidatingReader(sSoaPans, XmlNodeType.Document, null);
		while (xmlValidatingReader.Read())
		{
			if (xmlValidatingReader.NodeType == XmlNodeType.Text)
			{
				if (text == "")
				{
					text += xmlValidatingReader.Value;
				}
				else
				{
					text = ((!text.StartsWith("Error")) ? ("Error " + text + " " + xmlValidatingReader.Value) : (text + " " + xmlValidatingReader.Value));
				}
			}
		}
		if ((Strings.InStr(sSoaPans, "<env:Fault>") & Convert.ToInt32(!text.StartsWith("Error"))) > 0)
		{
			text = "Error: " + text;
		}
		return text;
	}

	public async Task<HttpResult> ProcessAsync(string xmlObject, int trytime = 0)
	{
		return await Task.FromResult(Process(xmlObject, trytime));
	}

	public HttpResult ProcessMutiTransaction(string seddionId, List<BodyRequestObject> lstBodyRequestObjects, bool isTryProcess = false, bool replaceOnPatch = true)
	{
		throw new Exception("Function not support in DI server");
	}

	public HttpResult ProcessJson(ApzObjectTypes objectType, string sessionId, string jsonObject, Enums.HttpAction action, object objectKey = null, string udoCode = "", bool isTryProcess = false, bool replaceOnPatch = true)
	{
		throw new NotImplementedException();
	}

	public HttpResult ProcessServiceJson(ApzObjectTypes objectType, string sessionId, string jsonObject, Enums.HttpAction action, object objectKey = null, string udoCode = "", bool isTryProcess = false)
	{
		throw new NotImplementedException();
	}

	public HttpResult GetDataObjectFromService(ApzObjectTypes objectType, string sessionId, Enums.HttpAction action = Enums.HttpAction.Get, object objectKey = null, string bodyString = "", bool isServiceAction = false, bool isTryProcess = false)
	{
		return new HttpResult(Enums.MessageCode.Error, "Function not support");
	}

	public virtual string MappingUdf(List<UserFieldsImpl> lst, EnumCommandType commandType = EnumCommandType.Add, bool ignoreUpdateEmptyValue = false, List<string> lstFieldNotValidated = null)
	{
		string text = string.Empty;
		try
		{
			if (lst != null && lst.Count > 0)
			{
				foreach (UserFieldsImpl item in lst)
				{
					if (item == null || string.IsNullOrEmpty(item.Name))
					{
						continue;
					}
					if (!item.Name.StartsWith("U_"))
					{
						item.Name = "U_" + item.Name;
					}
					switch (item.Type)
					{
					case BoFieldTypes.db_Alpha:
					case BoFieldTypes.db_Memo:
						if (commandType == EnumCommandType.Add || !string.IsNullOrEmpty(Function.ToString(item.Value)) || !ignoreUpdateEmptyValue)
						{
							text += string.Format("<{0}>{1}</{0}>", item.Name, (!string.IsNullOrEmpty(Function.ToString(item.Value))) ? Function.RawText(item.Value) : "");
						}
						break;
					case BoFieldTypes.db_Date:
						if (Function.ParseDateTime(item.Value).HasValue && Convert.ToDateTime(item.Value) > new DateTime(1900, 1, 1))
						{
							text += string.Format("<{0}>{1}</{0}>", item.Name, Convert.ToDateTime(item.Value).ToString("yyyyMMdd"));
						}
						else if (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update)
						{
							text += string.Format("<{0}>{1}</{0}>", item.Name, "00000000");
						}
						break;
					case BoFieldTypes.db_Numeric:
					case BoFieldTypes.db_Float:
						if (!((Math.Abs(Function.ParseDouble(item.Value)) < 1E-07) & ignoreUpdateEmptyValue) || commandType != EnumCommandType.Update)
						{
							text = ((item.SubType != BoFldSubTypes.st_Time && (!Function.IsInt(Function.ToString(item.Value)) || Function.ToString(item.Value).Length > 4)) ? (text + string.Format("<{0}>{1}</{0}>", item.Name, Function.ParseDecimal(item.Value))) : (text + string.Format("<{0}>{1}</{0}>", item.Name, Function.ParseInt(item.Value).ToString("0000"))));
						}
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	public virtual string MappingUdf(DataRow dataRow, EnumCommandType commandType = EnumCommandType.Add, bool ignoreUpdateEmptyValue = false)
	{
		string text = string.Empty;
		try
		{
			List<DataColumn> list = (from DataColumn t in dataRow.Table.Columns
				where Function.ToString(t.ColumnName).StartsWith("U_")
				select t).ToList();
			if (list != null && list.Count > 0)
			{
				foreach (DataColumn item in list)
				{
					if (item == null || string.IsNullOrEmpty(item.ColumnName))
					{
						continue;
					}
					object obj = dataRow[item];
					if (item.DataType == typeof(string) && (commandType == EnumCommandType.Add || !string.IsNullOrEmpty(Function.ToString(obj)) || !ignoreUpdateEmptyValue))
					{
						text += string.Format("<{0}>{1}</{0}>", item.ColumnName, Function.RawText(obj));
					}
					if (item.DataType == typeof(DateTime) || item.DataType == typeof(DateTime?))
					{
						if (Function.ParseDateTime(obj).HasValue && Convert.ToDateTime(obj) > new DateTime(1900, 1, 1))
						{
							text += string.Format("<{0}>{1}</{0}>", item.ColumnName, Convert.ToDateTime(obj).ToString("yyyyMMdd"));
						}
						else if (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update)
						{
							text += string.Format("<{0}>{1}</{0}>", item.ColumnName, "");
						}
					}
					if (new Type[6]
					{
						typeof(double),
						typeof(double?),
						typeof(float),
						typeof(float?),
						typeof(decimal),
						typeof(decimal?)
					}.Contains(item.DataType) && (Math.Abs(Function.ParseDecimal(obj)) > 0m || (!ignoreUpdateEmptyValue && commandType == EnumCommandType.Update)))
					{
						text += string.Format("<{0}>{1}</{0}>", item.ColumnName, Function.ParseDecimal(obj));
					}
					if (new Type[6]
					{
						typeof(short),
						typeof(short?),
						typeof(int),
						typeof(int?),
						typeof(long),
						typeof(long?)
					}.Contains(item.DataType))
					{
						text += string.Format("<{0}>{1}</{0}>", item.ColumnName, Function.ParseInt(obj));
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}
}
