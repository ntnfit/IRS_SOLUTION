using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Interfaces;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Financials;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using Apzon.Services.Interfaces.Commons;
using SAPbobsCOM;

namespace Apzon.DataServices.Implements;

public class CommonRepository : Repository<Common.Table_ID>, ICommon, IRepository<Common.Table_ID>
{
	protected readonly IUnitOfWork _unitOfWork;

	public CommonRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
		_unitOfWork = unitOfWork;
	}

	public bool GetIsMultiBranch()
	{
		try
		{
			DataTable dataTable = new DataTable();
			string query = "select \"MltpBrnchs\" from OADM;";
			dataTable = ServiceUnitOfWork.Commons.ExecuteDataTable(query, CommandType.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (dataTable.Rows[0] != null && dataTable.Rows[0][0] != null && dataTable.Rows[0][0].ToString() == "Y")
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetIsMultiBranch  FAILED ", ex.Message);
			return false;
		}
	}

	public bool GetAvailableQty(string itemCode)
	{
		try
		{
			DataTable dataTable = new DataTable();
			string query = "select sum(\"OnHand\" - \"IsCommited\" + \"OnOrder\") from OITW Where \"ItemCode\"=@itemCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@itemCode", itemCode));
			dataTable = ServiceUnitOfWork.Commons.ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (dataTable.Rows[0] != null && dataTable.Rows[0][0] != null && Function.ParseDouble(dataTable.Rows[0][0].ToString()) > 0.0)
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetAvailableQty  FAILED ", ex.Message);
			return false;
		}
	}

	public bool GetSerialBatch(string itemCode)
	{
		try
		{
			DataTable dataTable = new DataTable();
			string query = "Select \"ItemCode\" \"itemcode\" from OSRN Where \"ItemCode\"=@itemCode\r\n                                Union All\r\n                            Select \"ItemCode\" \"itemcode\" from OIBT Where \"ItemCode\"=@itemCode;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@itemCode", itemCode));
			dataTable = ServiceUnitOfWork.Commons.ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (dataTable.Rows[0] != null && dataTable.Rows[0][0] != null && !string.IsNullOrWhiteSpace(dataTable.Rows[0][0].ToString()))
				{
					return true;
				}
				return false;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetSerialBatch  FAILED ", ex.Message);
			return false;
		}
	}

	public bool GetIsManualSerial(int series)
	{
		try
		{
			DataTable dataTable = new DataTable();
			string query = "Select \"IsManual\" \"isManual\" from nnm1 Where \"Series\"=@series;";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@series", series));
			dataTable = ServiceUnitOfWork.Commons.ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]) == "Y";
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GetIsManualSerial  FAILED ", ex.Message);
			return false;
		}
	}

	public string ExecuteBatFile(string command)
	{
		try
		{
			if (File.Exists(command))
			{
				Process process = Process.Start(new ProcessStartInfo("cmd.exe", "/c " + command)
				{
					CreateNoWindow = true,
					UseShellExecute = false,
					RedirectStandardError = true,
					RedirectStandardOutput = true
				});
				process.WaitForExit();
				string text = process.StandardOutput.ReadToEnd();
				string text2 = process.StandardError.ReadToEnd();
				int exitCode = process.ExitCode;
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "output>>" + (string.IsNullOrEmpty(text) ? "(none)" : text));
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "error>>" + (string.IsNullOrEmpty(text2) ? "(none)" : text2));
				Logging.Write("WATCH", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "ExitCode: " + exitCode, "ExecuteCommand");
				process.Close();
			}
			else
			{
				Logging.Write("WATCH", "Error: No file found", command);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
		return "";
	}

	public async Task<HttpResult> ProcessXmlRequest(string xmlRequest, string sessionId)
	{
		XElement xElement = XElement.Parse(xmlRequest);
		if (xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("SessionID")) != null)
		{
			xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("SessionID")).Value = sessionId;
		}
		xmlRequest = $"<?xml version=\"1.0\" encoding=\"UTF-16\"?>\r\n                         {xElement}";
		XmlElement xmlElement = (await DataService.ProcessAsync(xmlRequest)).Content as XmlElement;
		XElement xElement2 = XElement.Parse(xmlElement.OuterXml);
		if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
		{
			XElement xElement3 = xElement2.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
			if (xElement3 != null)
			{
				Logging.Write("ERROR", xElement3.Value);
				return new HttpResult(Enums.MessageCode.Error, xElement3.Value);
			}
		}
		else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
		{
			List<XElement> list = (from t in xElement2.Descendants()
				where t.Name.LocalName.Equals("RetKey")
				select t).ToList();
			if (list.Count > 0)
			{
				List<string> list2 = new List<string>();
				list2.AddRange(list.Select((XElement t) => Function.ToString(t.Value)));
				if (list2.Count > 0)
				{
					return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list2.ToArray()));
				}
			}
		}
		else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("DocEntry"))
		{
			List<XElement> list3 = (from t in xElement2.Descendants()
				where t.Name.LocalName.Equals("DocEntry")
				select t).ToList();
			if (list3.Count > 0)
			{
				List<string> list4 = new List<string>();
				list4.AddRange(list3.Select((XElement t) => Function.ToString(t.Value)));
				if (list4.Count > 0)
				{
					return new HttpResult(Enums.MessageCode.Success, "", string.Join(",", list4.ToArray()));
				}
			}
		}
		return new HttpResult(Enums.MessageCode.Success);
	}

	public DataTable GetTableStructure(string table)
	{
		DataTable result = new DataTable();
		try
		{
			if (ExistsTable(table))
			{
				string query = "Select top 0 * From " + table + ";";
				result = ServiceUnitOfWork.Commons.ExecuteDataTable(query, CommandType.Text, (SqlParameter[])null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	private bool ExistsTable(string table)
	{
		bool result = false;
		try
		{
			string query = "DECLARE @TableName VARCHAR(100) = @Table;\r\n                            DECLARE @Exists smallint = (SELECT top 1 1 FROM sys.tables WHERE name = @TableName AND type = 'U');\r\n                            select @Exists";
			List<SqlParameter> list = new List<SqlParameter>();
			list.Add(new SqlParameter("@TableName", table));
			DataTable dataTable = ServiceUnitOfWork.Commons.ExecuteDataTable(query, CommandType.Text, list.ToArray());
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = Function.ParseInt(dataTable.Rows[0][0]) == 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return result;
	}

	protected virtual string GetQueryExistsTable(string table)
	{
		return "DECLARE @Exists smallint = (SELECT top 1 1 FROM sys.tables WHERE name = @TableName AND type = 'U');\r\n                     select @Exists";
	}

	public DataTable GetSeries(int ObjectCode)
	{
		try
		{
			SqlParameter sqlParameter = new SqlParameter("@ObjectCode", Function.ToString(ObjectCode));
			return ServiceUnitOfWork.Commons.ExecuteDataTable("select * from nnm1 where \"ObjectCode\" = @ObjectCode;", CommandType.Text, sqlParameter);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	public List<UserFieldsImpl> GetListField(string tableName)
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
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl
						{
							DefaultValue = Function.ToString(dataRow["Dflt"]),
							Description = Function.ToString(dataRow["Descr"]),
							EditSize = Function.ParseLong(dataRow["EditSize"]),
							FieldID = Function.ParseLong(dataRow["FieldID"]),
							LinkedTable = Function.ToString(dataRow["RTable"]),
							LinkedUDO = Function.ToString(dataRow["RelUDO"]),
							LinkedObject = (dataRow.IsExistsColumn("RelSO") ? Function.ToString(dataRow["RelSO"]) : ""),
							Mandatory = (Function.ToString(dataRow["NotNull"]).Equals("Y") ? BoYesNoEnum.tYES : BoYesNoEnum.tNO),
							Name = Function.ToString(dataRow["AliasID"]),
							Size = Function.ParseLong(dataRow["SizeID"]),
							TableName = Function.ToString(dataRow["TableID"])
						};
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
							if (array.Length != 0)
							{
								userFieldsImpl.ValidValues = new List<ValidValuesMDImpl>();
								DataRow[] array2 = array;
								foreach (DataRow dataRow2 in array2)
								{
									ValidValuesMDImpl item = new ValidValuesMDImpl
									{
										Value = Function.ToString(dataRow2["FldValue"]),
										Description = Function.ToString(dataRow2["Descr"])
									};
									userFieldsImpl.ValidValues.Add(item);
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

	public static object TableToEntity(string code, DataTable dt, string sessionId, IUnitOfWork unitOfWork)
	{
		object result = new object();
		try
		{
			List<UserFieldsImpl> list = new List<UserFieldsImpl>();
			if (dt != null && dt.Rows.Count > 0)
			{
				switch (code)
				{
				case "UserFieldsImpl":
				{
					List<UserFieldsImpl> list4 = new List<UserFieldsImpl>();
					for (int k = 0; k < dt.Rows.Count; k++)
					{
						UserFieldsImpl item2 = (UserFieldsImpl)DataRowToEntity(new UserFieldsImpl(), dt.Rows[k]);
						list4.Add(item2);
					}
					result = list4;
					break;
				}
				case "ProductionOder_Impl_OWOR":
				{
					List<ProductionOder_Impl_OWOR> list3 = new List<ProductionOder_Impl_OWOR>();
					list = unitOfWork.UserFields.GetListField("OWOR", sessionId);
					for (int j = 0; j < dt.Rows.Count; j++)
					{
						ProductionOder_Impl_OWOR productionOder_Impl_OWOR = (ProductionOder_Impl_OWOR)DataRowToEntity(new ProductionOder_Impl_OWOR(), dt.Rows[j]);
						productionOder_Impl_OWOR.UserFields = DataRowToUserFields(dt.Rows[j], list, sessionId, unitOfWork);
						list3.Add(productionOder_Impl_OWOR);
					}
					result = list3;
					break;
				}
				case "ProductionOder_LineImpl_WOR1":
				{
					List<ProductionOder_LineImpl_WOR1> list5 = new List<ProductionOder_LineImpl_WOR1>();
					list = unitOfWork.UserFields.GetListField("WOR1", sessionId);
					for (int l = 0; l < dt.Rows.Count; l++)
					{
						ProductionOder_LineImpl_WOR1 productionOder_LineImpl_WOR = (ProductionOder_LineImpl_WOR1)DataRowToEntity(new ProductionOder_LineImpl_WOR1(), dt.Rows[l]);
						productionOder_LineImpl_WOR.UserFields = DataRowToUserFields(dt.Rows[l], list, sessionId, unitOfWork);
						list5.Add(productionOder_LineImpl_WOR);
					}
					result = list5;
					break;
				}
				case "AttachmentFile_Impl":
				{
					List<AttachmentFile_Impl> list2 = new List<AttachmentFile_Impl>();
					for (int i = 0; i < dt.Rows.Count; i++)
					{
						AttachmentFile_Impl item = (AttachmentFile_Impl)DataRowToEntity(new AttachmentFile_Impl(), dt.Rows[i]);
						list2.Add(item);
					}
					result = list2;
					break;
				}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
		return result;
	}

	public static List<UserFieldsImpl> DataRowToUserFields(DataRow row, List<UserFieldsImpl> lstSource, string sessionId, IUnitOfWork unitOfWork)
	{
		List<UserFieldsImpl> list = new List<UserFieldsImpl>();
		for (int i = 0; i < row.Table.Columns.Count; i++)
		{
			DataColumn col = row.Table.Columns[i];
			if (!col.ColumnName.StartsWith("U_"))
			{
				continue;
			}
			UserFieldsImpl userFieldsImpl = lstSource.FirstOrDefault((UserFieldsImpl u) => u.Name == col.ColumnName.Remove(0, 2));
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
			DataColumn dataColumn = row.Table.Columns[i];
			PropertyInfo property = obj.GetType().GetProperty(dataColumn.ColumnName);
			if (!(property == null))
			{
				switch (Function.GetTypeName(property.PropertyType))
				{
				case "String":
					property.SetValue(obj, Function.ToString(row[dataColumn]));
					break;
				case "Double":
					property.SetValue(obj, Function.ParseDouble(row[dataColumn]));
					break;
				case "Int16":
					property.SetValue(obj, Function.ParseShort(row[dataColumn]));
					break;
				case "Int32":
					property.SetValue(obj, Function.ParseInt(row[dataColumn]));
					break;
				case "Int64":
					property.SetValue(obj, Function.ParseLong(row[dataColumn]));
					break;
				case "DateTime":
				{
					DateTime? dateTime = Function.ParseDateTime(row[dataColumn]);
					property.SetValue(obj, (!dateTime.HasValue) ? new DateTime(1900, 1, 1) : dateTime.Value);
					break;
				}
				}
			}
		}
		return obj;
	}

	public string SaveAttachmentFiles(List<AttachmentFile_Impl> lstFiles, Dictionary<string, string> code, string table)
	{
		string text = "";
		try
		{
			using Transaction transaction = new Transaction();
			int num = GetAbsEntryByCode(code, table);
			if (lstFiles != null && lstFiles.Count > 0)
			{
				if (num == 0)
				{
					num = Function.ParseInt(AddFile(0));
					if (num > 0)
					{
						text = DeleteFile(num, onlyLine: true);
						foreach (AttachmentFile_Impl lstFile in lstFiles)
						{
							lstFile.AbsEntry = num;
							text = SaveLineFile(lstFile);
							if (!string.IsNullOrEmpty(text))
							{
								break;
							}
						}
					}
				}
				else
				{
					AddFile(num);
					text = DeleteFile(num, onlyLine: true);
					foreach (AttachmentFile_Impl lstFile2 in lstFiles)
					{
						lstFile2.AbsEntry = num;
						text = SaveLineFile(lstFile2);
						if (!string.IsNullOrEmpty(text))
						{
							break;
						}
					}
				}
			}
			else if (num > 0)
			{
				text = DeleteFile(num, onlyLine: false);
				num = 0;
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				text = num.ToString();
				transaction.Commit();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			text = ex.Message;
		}
		return text;
	}

	public string SaveAttachmentFiles(DataTable dtFiles, Dictionary<string, string> code, string table, string sessionId, IUnitOfWork unitOfWork)
	{
		string text = "";
		try
		{
			if (dtFiles != null && dtFiles.Rows.Count > 0)
			{
				List<AttachmentFile_Impl> lstFiles = (List<AttachmentFile_Impl>)TableToEntity("AttachmentFile_Impl", dtFiles, sessionId, unitOfWork);
				return SaveAttachmentFiles(lstFiles, code, table);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	public bool ExistsSeries(ApzObjectTypes oItems, int seriesNum)
	{
		try
		{
			ICommons commons = ServiceUnitOfWork.Commons;
			SqlParameter[] array = new SqlParameter[2];
			int num = (int)oItems;
			array[0] = new SqlParameter("@Code", num.ToString());
			array[1] = new SqlParameter("@Num", seriesNum);
			return commons.ExecuteDataTable("SELECT 1 FROM NNM1 WHERE \"ObjectCode\"=@Code AND \"Series\"=@Num;", CommandType.Text, array).IsNotNull();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return false;
		}
	}

	private int GetAbsEntryByCode(Dictionary<string, string> codes, string table)
	{
		try
		{
			string text = "SELECT \"AtcEntry\" FROM " + table + "  WHERE  1=1 ";
			SqlParameter[] array = new SqlParameter[codes.Count];
			int num = 0;
			foreach (KeyValuePair<string, string> code in codes)
			{
				string[] array2 = code.Key.Split(',');
				if (array2.Length <= 1)
				{
					continue;
				}
				string text2 = array2[1];
				if (!(text2 == "int"))
				{
					if (text2 == "string")
					{
						array[num] = new SqlParameter("@" + array2[0], Function.ToString(code.Value));
					}
				}
				else
				{
					array[num] = new SqlParameter("@" + array2[0], Function.ParseInt(code.Value));
				}
				text = text + " AND \"" + array2[0] + "\"=@" + array2[0];
				num++;
			}
			DataTable dataTable = ServiceUnitOfWork.Commons.ExecuteDataTable(text + ";", CommandType.Text, array);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0][0]);
			}
			return 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return 0;
		}
	}

	private string AddFile(int absEntry)
	{
		return ServiceUnitOfWork.Commons.AddFile(absEntry);
	}

	private string SaveLineFile(AttachmentFile_Impl line)
	{
		string empty = string.Empty;
		try
		{
			List<SqlParameter> list = new List<SqlParameter>();
			string text = "INSERT INTO \"ATC1\"\r\n                               (\"AbsEntry\"\r\n                               ,\"Line\"\r\n                               ,\"srcPath\"\r\n                               ,\"trgtPath\"\r\n                               ,\"FileName\"\r\n                               ,\"FileExt\"\r\n                               ,\"Date\"\r\n                               ,\"UsrID\"\r\n                               ,\"Copied\"\r\n                               ,\"Override\"\r\n                               ,\"subPath\")\r\n                         VALUES (";
			text += "@AbsEntry ";
			list.Add(new SqlParameter("@AbsEntry", Function.ParseInt(line.AbsEntry)));
			text += ",@Line ";
			list.Add(new SqlParameter("@Line", Function.ParseInt(line.Line)));
			text += ", @srcPath ";
			list.Add(new SqlParameter("@srcPath", Function.ToString(line.srcPath)));
			text += ",@trgtPath ";
			list.Add(new SqlParameter("@trgtPath", Function.ToString(line.trgtPath)));
			text += ",@FileName ";
			list.Add(new SqlParameter("@FileName", Function.ToString(line.FileName)));
			text += ", @FileExt ";
			list.Add(new SqlParameter("@FileExt", Function.ToString(line.FileExt)));
			text += ",@Date ";
			list.Add(new SqlParameter("@Date", Function.ParseDateTime(line.Date)));
			text += ",@UsrID ";
			list.Add(new SqlParameter("@UsrID", Function.ParseInt(line.UsrID)));
			text += ", @Copied ";
			list.Add(new SqlParameter("@Copied", Function.ToString(line.Copied)));
			text += ",@Override ";
			list.Add(new SqlParameter("@Override", Function.ToString(line.Override)));
			text += ", @subPath ";
			list.Add(new SqlParameter("@subPath", Function.ToString(line.subPath)));
			text += " );";
			if (ServiceUnitOfWork.Commons.ExecuteNonQuery(text, CommandType.Text, list.ToArray()) > 0)
			{
				return null;
			}
			return "ERROR";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "SAVE DOC LINE FAILED ", ex.Message);
			return ex.Message;
		}
	}

	private string DeleteFile(int absEntry, bool onlyLine)
	{
		try
		{
			string text = "delete from ATC1 WHERE \"AbsEntry\" = @absEntry;";
			if (!onlyLine)
			{
				text += "\r\ndelete from OATC Where \"AbsEntry\" = @absEntry;";
			}
			ServiceUnitOfWork.Commons.ExecuteNonQuery(text, CommandType.Text, new SqlParameter("@absEntry", absEntry));
			return null;
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	public virtual HttpResult DoQuery(List<ContextActionDoQuery> doquery, string sessionId, string userExecute)
	{
		try
		{
			List<string> list = new List<string>();
			foreach (ContextActionDoQuery item in doquery)
			{
				string text = item.QueryText;
				foreach (QueryParamater paramater in item.Paramaters)
				{
					text = text.Replace(paramater.ParaName, $"'{paramater.Value}'");
				}
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(text);
				}
			}
			string text2 = Function.RawText(string.Join(" ", list.ToArray()));
			if (!string.IsNullOrEmpty(text2))
			{
				string xmlExecuteSQL = _unitOfWork.UDOs.GetXmlExecuteSQL(text2, sessionId);
				Logging.Write("WATCH", "Start Do query");
				XmlElement xmlElement = DataService.Process(xmlExecuteSQL).Content as XmlElement;
				XElement xElement = XElement.Parse(xmlElement.OuterXml);
				if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
				{
					XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
					if (xElement2 != null)
					{
						Logging.Write("ERROR", "***[End Do Query failed]" + xElement2.Value);
						return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
					}
				}
				else
				{
					Logging.Write("TRACE", "***[End Do Query OK]");
				}
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual HttpResult DoQueryWithTempTable(ContextActionDoQuery doquery, string sessionId, string userExecute)
	{
		return new HttpResult();
	}

	public virtual HttpResult HandleWorkflow(List<Workflow> processGroup, string sessionId)
	{
		try
		{
			Logging.Write("WATCH", "Start Handle Workflow");
			List<string> list = new List<string>();
			foreach (Workflow item in processGroup)
			{
				switch (item.ActionType)
				{
				case Enums.ContextActionActionTypeEx.MaketingDocument:
				{
					DocumentsImpl document = item.Document;
					string text2 = "";
					if (item.Method == Enums.ContextActionMethod.Update)
					{
						string text3 = string.Join(",", document.Lines.Select((Document_LinesImpl t) => t.LineNum));
						List<string> tableName = Function.GetTableName(item.ObjectType);
						DataTable dataTable = ExecuteDataTable("SELECT \"LineNum\",\"ItemCode\" FROM " + tableName[1] + " WHERE \"DocEntry\"=@DocEntry AND \"LineNum\" NOT IN (" + text3 + ");", CommandType.Text, new SqlParameter("@DocEntry", document.DocEntry));
						if (dataTable.IsNotNull())
						{
							for (int num = 0; num < dataTable.Rows.Count; num++)
							{
								Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
								document_LinesImpl.LineNum = Function.ParseInt(dataTable.Rows[num]["LineNum"]);
								document_LinesImpl.ItemCode = Function.ToString(dataTable.Rows[num]["ItemCode"]);
								document.Lines.Add(document_LinesImpl);
							}
							document.Lines = document.Lines.OrderBy((Document_LinesImpl t) => t.LineNum).ToList();
						}
						text2 = "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(document, EnumCommandType.Update, item.ObjectType, item.ObjectType, ignoreUpdateEmptyValue: true) + "</dis:UpdateObject>";
					}
					else
					{
						text2 = "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.Documents.Mapping(document, EnumCommandType.Add, item.ObjectType, item.ObjectType) + "</dis:AddObject>";
					}
					if (!string.IsNullOrEmpty(text2))
					{
						Logging.Write("TRACE", "cta-workflow-xmldocument: " + text2);
						list.Add(text2);
					}
					break;
				}
				case Enums.ContextActionActionTypeEx.UserDefinedObject:
					if (item.ObjectType == ApzObjectTypes.oUserDefinedObject)
					{
						DataSet dataSet = item.DataSet;
						DataTable udoTable = dataSet.Tables[0];
						DataTable dtFather = dataSet.Tables[1];
						dataSet.Tables.RemoveAt(0);
						dataSet.Tables.RemoveAt(0);
						string text = _unitOfWork.UDOs.MappingXml(udoTable, dtFather, dataSet, sessionId, "Add");
						if (!string.IsNullOrEmpty(text))
						{
							Logging.Write("TRACE", "cta-workflow-xmludo: " + text);
							list.Add(text);
						}
					}
					break;
				}
			}
			string text4 = string.Join(" ", list.ToArray());
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text4 + "</env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "End - Handle Workflow" + xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num2 = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", "[SUCCESS]End - Handle Workflow " + num2);
					return new HttpResult(Enums.MessageCode.Success, "", num2);
				}
			}
			Logging.Write("WATCH", "End - Handle Workflow");
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("WATCH", "[ERROR]End - Handle Workflow");
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual HttpResult HandleWorkflowProcess(List<IWorkflowProcess> processGroup, string sessionId)
	{
		try
		{
			Logging.Write("WATCH", "Start Handle Workflow");
			List<string> list = new List<string>();
			foreach (IWorkflowProcess item in processGroup)
			{
				Type typeOfClass = item.TypeOfClass;
				if (typeOfClass == typeof(DocumentsImpl))
				{
					WorkflowProcess<DocumentsImpl> workflowProcess = (WorkflowProcess<DocumentsImpl>)item;
					DocumentsImpl content = workflowProcess.Content;
					string text = "";
					switch (workflowProcess.Method)
					{
					case Enums.ContextActionMethod.SaveAsDraft:
					{
						text = ((workflowProcess.ObjectType != ApzObjectTypes.oStockTransfer && workflowProcess.ObjectType != ApzObjectTypes.oInventoryTransferRequest) ? _unitOfWork.Documents.Mapping(content, (content.DocEntry <= 0) ? EnumCommandType.Add : EnumCommandType.Update, ApzObjectTypes.oDrafts, workflowProcess.ObjectType) : _unitOfWork.StockTransfer.MappingXml(workflowProcess.ObjectType, content, isDraft: true, (content.DocEntry <= 0) ? EnumCommandType.Add : EnumCommandType.Update));
						string text3 = ((content.DocEntry > 0) ? "UpdateObject" : "AddObject");
						text = "<dis:" + text3 + " xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:" + text3 + ">";
						break;
					}
					case Enums.ContextActionMethod.Update:
					{
						string text2 = string.Join(",", content.Lines.Select((Document_LinesImpl t) => t.LineNum));
						List<string> tableName = Function.GetTableName(workflowProcess.ObjectType);
						DataTable dataTable = ExecuteDataTable("SELECT LineNum,ItemCode FROM " + tableName[1] + " WHERE DocEntry=@DocEntry AND LineNum NOT IN (" + text2 + ")", CommandType.Text, new SqlParameter("@DocEntry", content.DocEntry));
						if (dataTable.IsNotNull())
						{
							for (int num = 0; num < dataTable.Rows.Count; num++)
							{
								Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
								document_LinesImpl.LineNum = Function.ParseInt(dataTable.Rows[num]["LineNum"]);
								document_LinesImpl.ItemCode = Function.ToString(dataTable.Rows[num]["ItemCode"]);
								content.Lines.Add(document_LinesImpl);
							}
							content.Lines = content.Lines.OrderBy((Document_LinesImpl t) => t.LineNum).ToList();
						}
						text = ((workflowProcess.ObjectType != ApzObjectTypes.oStockTransfer && workflowProcess.ObjectType != ApzObjectTypes.oInventoryTransferRequest) ? _unitOfWork.Documents.Mapping(content, EnumCommandType.Update, workflowProcess.ObjectType, workflowProcess.ObjectType, ignoreUpdateEmptyValue: true) : _unitOfWork.StockTransfer.MappingXml(workflowProcess.ObjectType, content, isDraft: false, EnumCommandType.Update));
						text = "<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:UpdateObject>";
						break;
					}
					default:
						text = ((workflowProcess.ObjectType != ApzObjectTypes.oStockTransfer && workflowProcess.ObjectType != ApzObjectTypes.oInventoryTransferRequest) ? _unitOfWork.Documents.Mapping(content, EnumCommandType.Add, workflowProcess.ObjectType, workflowProcess.ObjectType, ignoreUpdateEmptyValue: true) : _unitOfWork.StockTransfer.MappingXml(workflowProcess.ObjectType, content, isDraft: false));
						text = "<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + text + "</dis:AddObject>";
						break;
					}
					if (!string.IsNullOrEmpty(text))
					{
						Logging.Write("TRACE", "cta-workflow-xmldocument: " + text);
						list.Add(text);
					}
				}
				if (typeOfClass == typeof(JournalEntriesImpl))
				{
					WorkflowProcess<JournalEntriesImpl> workflowProcess2 = (WorkflowProcess<JournalEntriesImpl>)item;
					JournalEntriesImpl content2 = workflowProcess2.Content;
					string text4 = "";
					text4 = ((workflowProcess2.Method != Enums.ContextActionMethod.Update) ? ("<dis:AddObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.JournalEntries.Mapping(content2, EnumCommandType.Add) + "</dis:AddObject>") : ("<dis:UpdateObject xmlns:dis=\"http://www.sap.com/SBO/DIS\">" + _unitOfWork.JournalEntries.Mapping(content2, EnumCommandType.Update) + " </dis:UpdateObject>"));
					if (!string.IsNullOrEmpty(text4))
					{
						Logging.Write("TRACE", "cta-workflow-xml journalEntry: " + text4);
						list.Add(text4);
					}
				}
				else
				{
					if (!(typeOfClass == typeof(DataSet)))
					{
						continue;
					}
					WorkflowProcess<DataSet> workflowProcess3 = (WorkflowProcess<DataSet>)item;
					if (workflowProcess3.ObjectType == ApzObjectTypes.oUserDefinedObject)
					{
						DataSet content3 = workflowProcess3.Content;
						DataTable udoTable = content3.Tables[0];
						DataTable dtFather = content3.Tables[1];
						content3.Tables.RemoveAt(0);
						content3.Tables.RemoveAt(0);
						string text5 = _unitOfWork.UDOs.MappingXml(udoTable, dtFather, content3, sessionId, "Add");
						if (!string.IsNullOrEmpty(text5))
						{
							Logging.Write("TRACE", "cta-workflow-xmludo: " + text5);
							list.Add(text5);
						}
					}
				}
			}
			string text6 = string.Join(" ", list.ToArray());
			string xmlObject = "<?xml version=\"1.0\" encoding=\"UTF-16\"?><env:Envelopes xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <env:Header xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">\r\n                        <SessionID>" + sessionId + "</SessionID>\r\n                        </env:Header><env:Body xmlns:env=\"http://schemas.xmlsoap.org/soap/envelope/\">" + text6 + "</env:Body></env:Envelopes>";
			XmlElement xmlElement = DataService.Process(xmlObject).Content as XmlElement;
			XElement xElement = XElement.Parse(xmlElement.OuterXml);
			if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("Fault"))
			{
				XElement xElement2 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("Text"));
				if (xElement2 != null)
				{
					Logging.Write("ERROR", "End - Handle Workflow" + xElement2.Value);
					return new HttpResult(Enums.MessageCode.Error, xElement2.Value);
				}
			}
			else if (!string.IsNullOrEmpty(xmlElement.OuterXml) && xmlElement.OuterXml.Contains("RetKey"))
			{
				XElement xElement3 = xElement.Descendants().FirstOrDefault((XElement t) => t.Name.LocalName.Equals("RetKey"));
				if (xElement3 != null)
				{
					int num2 = Function.ParseInt(xElement3.Value);
					Logging.Write("WATCH", "[SUCCESS]End - Handle Workflow " + num2);
					return new HttpResult(Enums.MessageCode.Success, "", num2);
				}
			}
			Logging.Write("WATCH", "End - Handle Workflow");
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("WATCH", "[ERROR]End - Handle Workflow");
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
