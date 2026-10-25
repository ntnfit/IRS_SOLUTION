using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class UDORepository : Repository<DataTable>, IUDOs, IRepository<DataTable>, IDisposable
{
	public UDORepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public HttpResult Save(DataSet ds, string sessionId)
	{
		try
		{
			string text = "";
			string[] array = sessionId.Split(',');
			if (array != null && array.Length > 1)
			{
				_ = sessionId.Split(',')[0];
				text = sessionId.Split(',')[1];
			}
			else
			{
				text = sessionId;
			}
			if (ds.Tables.Count >= 2)
			{
				DataTable udoTable = ds.Tables[0];
				DataTable dtFather = ds.Tables[1];
				ds.Tables.RemoveAt(0);
				ds.Tables.RemoveAt(0);
				return Update(udoTable, dtFather, ds, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new HttpResult(Enums.MessageCode.Error, "Cannot Save UDO");
	}

	public string DeleteDoc(DataSet ds, string sessionId)
	{
		try
		{
			if (ds.Tables.Count >= 2)
			{
				DataTable dataTable = ds.Tables[0];
				DataTable dataTable2 = ds.Tables[1];
				string udoCode = Function.ToString(dataTable.Rows[0]["Code"]);
				string text = Function.ToString(dataTable.Rows[0]["TYPE"]);
				string udoName = Function.ToString(dataTable.Rows[0]["Name"]);
				int docEntry = Function.ParseInt(dataTable2.Rows[0]["DocEntry"]);
				if (text.Equals("1"))
				{
					string code = Function.ToString(dataTable2.Rows[0]["Code"]);
					return ExecuteMasterData(udoCode, udoName, code, sessionId, "Remove");
				}
				return Execute(udoCode, udoName, docEntry, sessionId, "Remove");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
		return "";
	}

	public string CancelDoc(DataSet ds, string sessionId)
	{
		try
		{
			if (ds.Tables.Count >= 2)
			{
				DataTable dataTable = ds.Tables[0];
				DataTable dataTable2 = ds.Tables[1];
				string obj = Function.ToString(dataTable2.Rows[0]["DocEntry"]);
				string udoCode = Function.ToString(dataTable.Rows[0]["Code"]);
				string udoName = Function.ToString(dataTable.Rows[0]["Name"]);
				if (Function.ToString(dataTable.Rows[0]["TYPE"]).Equals("1"))
				{
					string code = Function.ToString(dataTable2.Rows[0]["Code"]);
					return ExecuteMasterData(udoCode, udoName, code, sessionId, "Cancel");
				}
				return Execute(udoCode, udoName, Function.ParseInt(obj), sessionId, "Cancel");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
		return "";
	}

	public string CloseDoc(DataSet ds, string sessionId)
	{
		try
		{
			if (ds.Tables.Count >= 2)
			{
				DataTable dataTable = ds.Tables[0];
				DataTable dataTable2 = ds.Tables[1];
				string obj = Function.ToString(dataTable2.Rows[0]["DocEntry"]);
				string udoCode = Function.ToString(dataTable.Rows[0]["Code"]);
				string udoName = Function.ToString(dataTable.Rows[0]["Name"]);
				if (Function.ToString(dataTable.Rows[0]["TYPE"]).Equals("1"))
				{
					string code = Function.ToString(dataTable2.Rows[0]["Code"]);
					return ExecuteMasterData(udoCode, udoName, code, sessionId, "Close");
				}
				return Execute(udoCode, udoName, Function.ParseInt(obj), sessionId, "Close");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return ex.Message;
		}
		return "";
	}

	public XElement GetByParams(string udoCode, string udoName, int docEntry, string sessionId)
	{
		try
		{
			string text = "";
			text = string.Format("<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                    <env:Header>\r\n                        <SessionID>{0}</SessionID>\r\n                    </env:Header>\r\n                    <env:Body>\r\n                        <dis:GetByParams xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                            <Service>{1}</Service>\r\n                            <{3}>\r\n                                <DocEntry>{2}</DocEntry>\r\n                            </{3}>\r\n                        </dis:GetByParams>\r\n                    </env:Body>\r\n                </env:Envelope>", sessionId, udoCode, docEntry, udoName);
			XmlElement xmlElement = DataService.Process(text).Content as XmlElement;
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				return null;
			}
			return XElement.Parse(xmlElement.OuterXml);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public XElement ExecuteSQL(string sql, string sessionId)
	{
		try
		{
			string text = "";
			text = $"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n                <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                    <env:Header>\r\n                        <SessionID>{sessionId}</SessionID>\r\n                    </env:Header>\r\n                    <env:Body>\r\n                        <dis:ExecuteSQL xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                            <DoQuery>{sql}</DoQuery>\r\n                        </dis:ExecuteSQL>\r\n                    </env:Body>\r\n                </env:Envelope>";
			return XElement.Parse((DataService.Process(text).Content as XmlElement).OuterXml);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public string GetXmlExecuteSQL(string sql, string sessionId, bool isGetBody = false)
	{
		string text = "";
		try
		{
			text = $"<dis:ExecuteSQL xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                        <DoQuery>{sql}</DoQuery>\r\n                                    </dis:ExecuteSQL>";
			if (isGetBody)
			{
				return text;
			}
			return $"<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n                <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                    <env:Header>\r\n                        <SessionID>{sessionId}</SessionID>\r\n                    </env:Header>\r\n                    <env:Body>\r\n                        {text}\r\n                    </env:Body>\r\n                </env:Envelope>";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return text;
		}
	}

	public virtual string Execute(string udoCode, string udoName, int docEntry, string sessionId, string command)
	{
		try
		{
			string xmlObject = string.Format("<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                            <env:Header>\r\n                                                            <SessionID>{0}</SessionID>\r\n                                                            </env:Header>\r\n                                                            <env:Body>\r\n                                                            <dis:{1} xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                                                <Service>{2}</Service>\r\n                                                                <{3}>\r\n                                                                    <DocEntry>{4}</DocEntry>\r\n                                                                </{3}>\r\n                                                            </dis:{1}>\r\n                                                            </env:Body>\r\n                                                        </env:Envelope>", sessionId, command, udoCode, udoName, docEntry);
			Logging.Write("WATCH", "Start Execute UDO: " + udoCode, command);
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Execute UDO ERROR: " + udoCode, command, xElement2.Value);
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					Logging.Write("WATCH", "Execute UDO Success: " + udoCode, command, xElement3.Value);
					Function.ParseInt(xElement3.Value);
				}
			}
			Logging.Write("WATCH", "Execute UDO Success: " + udoCode, command, docEntry.ToString());
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual string ExecuteMasterData(string udoCode, string udoName, string code, string sessionId, string command)
	{
		try
		{
			string xmlObject = string.Format("<env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                                            <env:Header>\r\n                                                            <SessionID>{0}</SessionID>\r\n                                                            </env:Header>\r\n                                                            <env:Body>\r\n                                                            <dis:{1} xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                                                <Service>{2}</Service>\r\n                                                                <{3}>\r\n                                                                    <Code>{4}</Code>\r\n                                                                </{3}>\r\n                                                            </dis:{1}>\r\n                                                            </env:Body>\r\n                                                        </env:Envelope>", sessionId, command, udoCode, udoName, code);
			Logging.Write("WATCH", "Start Execute UDO: " + udoCode, command);
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Execute UDO ERROR: " + udoCode, command, xElement2.Value);
					return xElement2.Value;
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					Logging.Write("WATCH", "Execute UDO Success: " + udoCode, command, xElement3.Value);
					Function.ParseInt(xElement3.Value);
				}
			}
			Logging.Write("WATCH", "Execute UDO Success: " + udoCode, command, code);
			return string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual HttpResult Update(DataTable udoTable, DataTable dtFather, DataSet dsChild, string sessionId)
	{
		try
		{
			string text = "";
			string text2 = Function.ToString(udoTable.Rows[0]["Code"]);
			string text3 = ((dtFather.Rows[0]["DocEntry"] != null) ? dtFather.Rows[0]["DocEntry"].ToString() : "0");
			if (Function.ParseInt(text3) > 0)
			{
				text = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n                                        <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                      <env:Header>\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                      </env:Header>\r\n                                      <env:Body>" + MappingXml(udoTable, dtFather, dsChild, sessionId, "Update") + "\r\n                                      </env:Body>\r\n                                    </env:Envelope>";
				Logging.Write("WATCH", "Start Update UDO: " + text2, text3);
			}
			else
			{
				text = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n                                    <env:Envelope xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                      <env:Header>\r\n                                        <SessionID>" + sessionId + "</SessionID>\r\n                                      </env:Header>\r\n                                      <env:Body>" + MappingXml(udoTable, dtFather, dsChild, sessionId, "Add") + "\r\n                                      </env:Body>\r\n                                    </env:Envelope>";
				Logging.Write("WATCH", "Start Add UDO: " + text2);
			}
			XmlElement xmlElement = DataService.Process(text).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Execute UDO ERROR: " + text2, xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("DocEntry"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("DocEntry"));
				if (xElement3 != null)
				{
					Logging.Write("WATCH", "Execute UDO Success: " + text2, xElement3.Value);
					int num = Function.ParseInt(xElement3.Value);
					return new HttpResult(Enums.MessageCode.Success, "", num);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Code"))
			{
				XElement xElement4 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Code"));
				if (xElement4 != null)
				{
					Logging.Write("WATCH", "Execute UDO Success: " + text2, xElement4.Value);
					return new HttpResult(Enums.MessageCode.Success, "", xElement4.Value);
				}
			}
			else
			{
				try
				{
					if (Function.ParseInt(udoTable.Rows[0]["TYPE"]) == 1)
					{
						return new HttpResult(Enums.MessageCode.Success, "", Function.ToString(dtFather.Rows[0]["Code"]));
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", ex);
				}
			}
			Logging.Write("WATCH", "Execute UDO Success: " + text2, text3);
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	public virtual string MappingXml(DataTable udoTable, DataTable dtFather, DataSet dsChild, string sessionId, string command)
	{
		string text = Function.ToString(udoTable.Rows[0]["TYPE"]);
		string text2 = string.Format("\r\n                                        <dis:{0} xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                          <Service>{1}</Service>\r\n                                          <{2}>", command, Function.ToString(udoTable.Rows[0]["Code"]), Function.ToString(udoTable.Rows[0]["Name"]));
		if (command.ToUpper().Equals("UPDATE"))
		{
			text2 += string.Format("  <DocEntry>{0}</DocEntry>", Function.ToString(dtFather.Rows[0]["DocEntry"]));
			if (text.Equals("3"))
			{
				text2 += string.Format("  <DocNum>{0}</DocNum>", Function.ToString(dtFather.Rows[0]["DocEntry"]));
				text2 += "  <Series>-1</Series>";
			}
		}
		List<DataColumn> list = dtFather.Columns.Cast<DataColumn>().ToList();
		foreach (DataRow row in dtFather.Rows)
		{
			foreach (DataColumn item in list)
			{
				if (item.ColumnName.Equals("DocEntry") || item.ColumnName.Equals("DocNum") || item.ColumnName.Equals("Series"))
				{
					continue;
				}
				if (Function.ToString(row[item]) == "")
				{
					text2 += string.Format("<{0}></{0}>", item.ColumnName);
				}
				else if (item.DataType == typeof(double) || item.DataType == typeof(decimal))
				{
					text2 += string.Format("<{0}>{1}</{0}>", item.ColumnName, Function.ParseDouble(row[item]));
				}
				else if (item.DataType == typeof(short))
				{
					if (Function.ToString(row[item]).Length < 4)
					{
						string arg = Function.ParseInt(row[item]).ToString("0000");
						string arg2 = $"{arg}";
						text2 += string.Format("<{0}>{1}</{0}>", item.ColumnName, arg2);
					}
					else
					{
						text2 += string.Format("<{0}>{1}</{0}>", item.ColumnName, Function.ParseInt(row[item]));
					}
				}
				else if (!(item.DataType == typeof(int)) && !(item.DataType == typeof(int)) && !(item.DataType == typeof(long)))
				{
					text2 = ((!(item.DataType == typeof(DateTime))) ? (text2 + string.Format("<{0}>{1}</{0}>", item.ColumnName, Function.RawText(row[item]))) : (text2 + string.Format("<{0}>{1}</{0}>", item.ColumnName, Convert.ToDateTime(row[item]).ToString("yyyyMMdd"))));
				}
				else
				{
					text2 += string.Format("<{0}>{1}</{0}>", item.ColumnName, Function.ParseInt(row[item]));
				}
			}
		}
		if (dsChild.Tables.Count > 0)
		{
			foreach (DataTable dtChild in dsChild.Tables)
			{
				text2 += $" <{dtChild.TableName}Collection>";
				List<DataColumn> columnNames = (from DataColumn c in dtChild.Columns
					where c.ColumnName != "Code" && c.ColumnName != "DocEntry"
					select c).ToList();
				var source = dtChild.AsEnumerable().Select((DataRow t, int index) => new
				{
					index = index,
					Item = t
				});
				ConcurrentBag<Tuple<int, string>> lstItemXml = new ConcurrentBag<Tuple<int, string>>();
				if (Parallel.ForEach(source, row =>
				{
					string text3 = "";
					text3 += $"<{dtChild.TableName}>";
					foreach (DataColumn item2 in columnNames)
					{
						if (item2.ColumnName.Equals("DocEntry") && command.ToUpper().Equals("UPDATE"))
						{
							text3 += string.Format("<{0}>{1}</{0}>", item2.ColumnName, Function.ParseInt(dtFather.Rows[0]["DocEntry"]));
						}
						else if (!item2.ColumnName.Equals("LineId") || Function.ParseInt(row.Item[item2]) != 0)
						{
							if (Function.ToString(row.Item[item2]) == "")
							{
								text3 += string.Format("<{0}></{0}>", item2.ColumnName);
							}
							else if (item2.DataType == typeof(double) || item2.DataType == typeof(decimal))
							{
								text3 += string.Format("<{0}>{1}</{0}>", item2.ColumnName, Function.ParseDouble(row.Item[item2]));
							}
							else if (item2.DataType == typeof(short))
							{
								if (Function.ToString(row.Item[item2]).Length < 4)
								{
									string arg3 = Function.ParseInt(row.Item[item2]).ToString("0000");
									string arg4 = $"{arg3}";
									text3 += string.Format("<{0}>{1}</{0}>", item2.ColumnName, arg4);
								}
								else
								{
									text3 += string.Format("<{0}>{1}</{0}>", item2.ColumnName, Function.ParseInt(row.Item[item2]));
								}
							}
							else
							{
								text3 = ((!(item2.DataType == typeof(int)) && !(item2.DataType == typeof(int)) && !(item2.DataType == typeof(long))) ? ((!(item2.DataType == typeof(DateTime))) ? (text3 + string.Format("<{0}>{1}</{0}>", item2.ColumnName, Function.RawText(row.Item[item2]))) : (text3 + string.Format("<{0}>{1}</{0}>", item2.ColumnName, Convert.ToDateTime(row.Item[item2]).ToString("yyyyMMdd")))) : (text3 + string.Format("<{0}>{1}</{0}>", item2.ColumnName, Function.ParseInt(row.Item[item2]))));
							}
						}
					}
					text3 += $"</{dtChild.TableName}>";
					lstItemXml.Add(new Tuple<int, string>(row.index, text3));
				}).IsCompleted)
				{
					text2 += string.Join("", (from t in lstItemXml.ToArray()
						orderby t.Item1
						select t.Item2).ToArray());
					try
					{
						GC.Collect();
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", ex);
					}
					text2 += $" </{dtChild.TableName}Collection>";
					continue;
				}
				throw new Exception("Could not complete action-parallel mapping xml");
			}
		}
		return text2 + string.Format("  </{0}>\r\n                                        </dis:{1}>", Function.ToString(udoTable.Rows[0]["Name"]), command);
	}

	public void Dispose()
	{
	}
}
