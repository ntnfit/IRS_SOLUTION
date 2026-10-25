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
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class TerritoryRepository : Repository<DataTable>, ITerritory, IRepository<DataTable>, IDisposable
{
	public TerritoryRepository(Apzon.Services.IUnitOfWork unitOfWork, IDataService dataService)
		: base(unitOfWork, dataService)
	{
	}

	public void Dispose()
	{
	}

	public virtual string SaveData(DataTable dtS, string sessionId)
	{
		try
		{
			string text = Mapping(dtS, sessionId);
			if (!text.StartsWith("ERR:"))
			{
				StringBuilder stringBuilder = new StringBuilder("<?xml version=\"1.0\" encoding=\"UTF-16\"?>");
				stringBuilder.Append(text);
				XmlElement xmlElement = DataService.Process(stringBuilder.ToString()).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						throw new Exception(xElement2.Value);
					}
				}
				return "";
			}
			return text.Remove(0, 4);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public virtual int GetNextKey()
	{
		try
		{
			DataTable dataTable = ExecuteDataTable("Select autokey from ONNM where objectcode='200'", CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return 0;
		}
	}

	public virtual string GetDeleteXML(DataTable dtSource)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			DataTable dataTable = ExecuteDataTable("Select territryID,parent from OTER", CommandType.Text);
			int num = 0;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					int id = Function.ParseInt(dataTable.Rows[i]["territryID"]);
					int value = Function.ParseInt(dataTable.Rows[i]["parent"]);
					if (!dtSource.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["territryID"]) == id))
					{
						dictionary.Add(id, value);
					}
				}
				List<int> lstChild = new List<int>();
				foreach (KeyValuePair<int, int> item in dictionary)
				{
					GetAllChildID(item.Key, ref lstChild, dictionary);
				}
				foreach (int item2 in lstChild)
				{
					dictionary.Remove(item2);
				}
				stringBuilder.Append("<dis:RemoveObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                   <BOM>");
				foreach (KeyValuePair<int, int> item3 in dictionary)
				{
					stringBuilder.Append("<BO>\r\n                                        <AdmInfo>");
					stringBuilder.Append("<Object>oTerritories</Object>");
					stringBuilder.Append("</AdmInfo>\r\n                                        <QueryParams>");
					stringBuilder.Append("<TerritoryID>" + item3.Key + "</TerritoryID>\r\n                                        </QueryParams>\r\n                                        </BO>");
					num++;
				}
				stringBuilder.Append("</BOM>\r\n                                  </dis:RemoveObject>");
			}
			if (num > 0)
			{
				return stringBuilder.ToString();
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	public virtual string Mapping(DataTable dt, string sessionId)
	{
		try
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append("<env:Envelope xmlns:env = \"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                                <env:Header>\r\n                                 <SessionID>" + sessionId + "</SessionID></env:Header><env:Body>");
			string deleteXML = GetDeleteXML(dt);
			stringBuilder.Append(deleteXML);
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder2.Append("<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                    <BOM>");
			StringBuilder stringBuilder3 = new StringBuilder();
			stringBuilder3.Append("<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">\r\n                                    <BOM>");
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				int num3 = Function.ParseInt(dt.Rows[i]["territryID"]);
				if (Exists(num3, sessionId))
				{
					num++;
					stringBuilder2.Append("\r\n                                        <BO>\r\n                                        <AdmInfo>\r\n                                          <Object>oTerritories</Object>");
					stringBuilder2.Append("</AdmInfo>");
					stringBuilder2.Append("<QueryParams>\r\n                                             <TerritoryID>" + Function.RawText(num3) + "</TerritoryID>\r\n                                            </QueryParams>");
					stringBuilder2.Append("<Territories>");
					stringBuilder2.Append("<row>");
					stringBuilder2.Append("<Description>" + Function.ToString(dt.Rows[i]["descript"]) + "</Description>");
					stringBuilder2.Append("<Parent>" + Function.ParseInt(dt.Rows[i]["parent"]) + "</Parent>");
					stringBuilder2.Append("<LocationIndex>" + Function.ParseInt(dt.Rows[i]["lindex"]) + "</LocationIndex>");
					stringBuilder2.Append("<Inactive>" + Function.ToString(dt.Rows[i]["inactive"]) + "</Inactive>");
					stringBuilder2.Append("</row>");
					stringBuilder2.Append("</Territories>\r\n                                    </BO>");
				}
				else
				{
					num2++;
					stringBuilder3.Append("\r\n                                        <BO>\r\n                                        <AdmInfo>\r\n                                          <Object>oTerritories</Object>");
					stringBuilder3.Append("</AdmInfo>\r\n                                    <Territories>");
					stringBuilder3.Append("<row>");
					stringBuilder3.Append("<Description>" + Function.ToString(dt.Rows[i]["descript"]) + "</Description>");
					stringBuilder3.Append("<Parent>" + Function.ParseInt(dt.Rows[i]["parent"]) + "</Parent>");
					stringBuilder3.Append("<LocationIndex>" + Function.ParseInt(dt.Rows[i]["lindex"]) + "</LocationIndex>");
					stringBuilder3.Append("<Inactive>" + Function.ToString(dt.Rows[i]["inactive"]) + "</Inactive>");
					stringBuilder3.Append("</row>");
					stringBuilder3.Append("</Territories>\r\n                                    </BO>");
				}
			}
			stringBuilder2.Append("</BOM></dis:UpdateObject>");
			stringBuilder3.Append("</BOM></dis:AddObject>");
			if (num2 > 0)
			{
				stringBuilder.Append(stringBuilder3);
			}
			if (num > 0)
			{
				stringBuilder.Append(stringBuilder2);
			}
			if (num2 > 0 || num > 0)
			{
				stringBuilder.Append("</env:Body></env:Envelope>");
				return stringBuilder.ToString();
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "ERR:" + ex.Message;
		}
	}

	public virtual bool Exists(int territryID, string sessionId)
	{
		try
		{
			string query = "SELECT 1\r\n                            FROM OTER \r\n                            WHERE territryID =  @territryID";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@territryID", territryID)).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return false;
	}

	public void GetAllChildID(int parentID, ref List<int> lstChild, Dictionary<int, int> dicSource)
	{
		try
		{
			IEnumerable<KeyValuePair<int, int>> source = dicSource.Where((KeyValuePair<int, int> r) => r.Value == parentID);
			if (!source.Any())
			{
				return;
			}
			KeyValuePair<int, int>[] array = source.ToArray();
			for (int num = 0; num < array.Length; num++)
			{
				if (!lstChild.Contains(array[num].Key))
				{
					lstChild.Add(array[num].Key);
				}
				GetAllChildID(array[num].Key, ref lstChild, dicSource);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}
}
