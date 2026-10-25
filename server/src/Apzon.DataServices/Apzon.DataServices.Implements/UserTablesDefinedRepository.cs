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
using Apzon.Services;

namespace Apzon.DataServices.Implements;

public class UserTablesDefinedRepository : Repository<DataTable>, IUserTablesDefined, IDisposable
{
	public UserTablesDefinedRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public virtual bool CheckTableUDTExists(string tableName)
	{
		try
		{
			string query = "select 1 from OUTB where \"TableName\" = @tableName;";
			return ExecuteDataTable(query, CommandType.Text, new SqlParameter("@tableName", tableName)).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
	}

	protected virtual string GetListTablesUDTQuery()
	{
		return "SELECT CONVERT(NVARCHAR,TableName)+',' as TableName FROM OUTB  FOR XML PATH('')";
	}

	public List<string> GetListTablesUDT()
	{
		try
		{
			string listTablesUDTQuery = GetListTablesUDTQuery();
			DataTable dataTable = ExecuteDataTable(listTablesUDTQuery, CommandType.Text);
			if (dataTable.IsNotNull())
			{
				return Function.ToString(dataTable.Rows[0]["TableName"]).Split(',').ToList();
			}
			return new List<string>();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new List<string>();
		}
	}

	protected virtual string Mapping(DataRow row, EnumCommandType commandType, string sessionId)
	{
		try
		{
			string empty = string.Empty;
			empty = ((commandType != EnumCommandType.Add) ? "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" : "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">");
			empty += "<BOM>\r\n                          <BO>\r\n                          <AdmInfo>\r\n                            <Object>UserTablesMD</Object>\r\n                          </AdmInfo>";
			empty += "<UserTablesMD>";
			empty += "<row>";
			if (commandType == EnumCommandType.Add)
			{
				empty = empty + "<TableName>" + Function.ToString(row["TableName"]) + "</TableName>";
			}
			empty = empty + "<TableDescription>" + Function.ToString(row["Descr"]) + "</TableDescription>";
			switch (Function.ParseInt(row["ObjectType"]))
			{
			case 0:
				empty += "<TableType>bott_NoObject</TableType>";
				break;
			case 1:
				empty += "<TableType>bott_MasterData</TableType>";
				break;
			case 2:
				empty += "<TableType>bott_MasterDataLines</TableType>";
				break;
			case 3:
				empty += "<TableType>bott_Document</TableType>";
				break;
			case 4:
				empty += "<TableType>bott_DocumentLines</TableType>";
				break;
			}
			empty = empty + "<Archivable>" + Function.ToString(row["Archivable"]) + "</Archivable>";
			empty = empty + "<ArchiveDateField>" + Function.ToString(row["ArchivDate"]) + "</ArchiveDateField>";
			empty += "</row>";
			empty += "</UserTablesMD>";
			empty += "</BO></BOM>";
			return (commandType != EnumCommandType.Add) ? (empty + "</dis:UpdateObject>") : (empty + "</dis:AddObject>");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
		}
	}

	public virtual string SaveData(DataTable dt, string sessionId)
	{
		try
		{
			string text = string.Empty;
			foreach (DataRow row in dt.Rows)
			{
				if (CheckTableUDTExists(Function.ToString(row["TableName"])))
				{
					text = "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">";
					text += Mapping(row, EnumCommandType.Update, sessionId);
				}
				else
				{
					text += Mapping(row, EnumCommandType.Add, sessionId);
				}
			}
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text + "</env:Body></env:Envelopes>";
			DataService.GetBusinessObjectTemplate(sessionId, "UserTablesMD");
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "Save User Defined Table error: " + xElement2.Value);
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
						Logging.Write("WATCH", "Save Save User Defined Table Success: " + string.Join(",", list2.ToArray()));
						return string.Join(",", list2.ToArray());
					}
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

	public void Dispose()
	{
		throw new NotImplementedException();
	}
}
