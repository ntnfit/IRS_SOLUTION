using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Services;
using SAPbobsCOM;

namespace Apzon.DataServices.HanaImplements;

public class HanaMasterRepository : MasterRepository
{
	public HanaMasterRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override HttpResult Remove(object value, string sessionId, ApzObjectTypes objType, string keyTag = "")
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				ObjectType = objType,
				OutObjectKey = true,
				Key = value,
				Action = Enums.HttpAction.Delete
			});
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult Save(DataTable dt, string sessionId, ApzObjectTypes objType, string keyFieldName, Dictionary<string, string> dicFieldTags, bool deleteOldRows = false, Enums.ImportActionEnum actionType = Enums.ImportActionEnum.AddUpdate, string tableName = "", string rowTag = "", string keyTag = "", object parameter = null)
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
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (dt != null)
			{
				if (deleteOldRows)
				{
					foreach (DataRow row in GetDocs(tableName).Rows)
					{
						if (dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t[keyFieldName]) == Function.ToString(row[keyFieldName]) && Function.ParseInt(row[keyFieldName]) != 0) == null)
						{
							list.Add(new BodyRequestObject
							{
								ObjectType = objType,
								OutObjectKey = true,
								Key = row[keyFieldName],
								Action = Enums.HttpAction.Delete
							});
						}
					}
				}
				for (int num = 0; num < dt.Rows.Count; num++)
				{
					string bodyObject = Mapping(dt.Rows[num], dicFieldTags);
					list.Add(new BodyRequestObject
					{
						ObjectType = objType,
						OutObjectKey = true,
						Key = dt.Rows[num][keyFieldName],
						Action = ((!Exists(Function.ToString(dt.Rows[num][keyFieldName]), tableName, keyFieldName)) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
						BodyObject = bodyObject
					});
				}
				return DataService.ProcessMutiTransaction(sessionId, list);
			}
			return new HttpResult(Enums.MessageCode.Success);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public virtual string Mapping(DataRow doc, Dictionary<string, string> dicFieldTags)
	{
		string text = "";
		try
		{
			text += "{";
			foreach (KeyValuePair<string, string> dicFieldTag in dicFieldTags)
			{
				object obj = doc[dicFieldTag.Value];
				switch (doc.Table.Columns[dicFieldTag.Value].DataType.Name.ToLower())
				{
				case "int16":
					text = (string.IsNullOrEmpty(Function.ToString(obj)) ? (text + Function.GetJsonString(dicFieldTag.Key ?? "", -1)) : (text + Function.GetJsonString(dicFieldTag.Key ?? "", Function.ParseShort(obj))));
					break;
				case "int32":
					text = (string.IsNullOrEmpty(Function.ToString(obj)) ? (text + Function.GetJsonString(dicFieldTag.Key ?? "", -1)) : (text + Function.GetJsonString(dicFieldTag.Key ?? "", Function.ParseInt(obj))));
					break;
				case "int64":
					text = (string.IsNullOrEmpty(Function.ToString(obj)) ? (text + Function.GetJsonString(dicFieldTag.Key ?? "", -1)) : (text + Function.GetJsonString(dicFieldTag.Key ?? "", Function.ParseLong(obj))));
					break;
				case "datetime":
				{
					DateTime? dateTime = Function.ParseDateTime(obj);
					if (dateTime > new DateTime(1900, 1, 1))
					{
						string value = dateTime.Value.ToString("yyyyMMdd");
						text += Function.GetJsonString(dicFieldTag.Key ?? "", value);
					}
					break;
				}
				case "decimal":
					text += Function.GetJsonString(dicFieldTag.Key ?? "", Function.ParseDecimal(obj));
					break;
				case "double":
					text += Function.GetJsonString(dicFieldTag.Key ?? "", Function.ParseDecimal(obj));
					break;
				case "float":
					text += Function.GetJsonString(dicFieldTag.Key ?? "", Function.ParseDecimal(obj));
					break;
				default:
					text += Function.GetJsonString(dicFieldTag.Key ?? "", Function.ToString(obj));
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
			text += "},";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return text;
	}

	protected override string CustomMapping(DataRow dr)
	{
		return HanaCustomMapping(dr);
	}

	protected virtual string HanaCustomMapping(DataRow dr)
	{
		return "";
	}

	public override HttpResult RemoveService(int code, string sessionId, ApzObjectTypes objType, string serviceName, string deleteMethodTag = "", string paramsTag = "", string keyTag = "")
	{
		try
		{
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			list.Add(new BodyRequestObject
			{
				ObjectType = objType,
				OutObjectKey = true,
				Key = code,
				Action = Enums.HttpAction.Delete
			});
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult SaveService(DataTable dt, string sessionId, ApzObjectTypes objType, string serviceName, string keyFieldName, Dictionary<string, string> dicFieldTags, bool deleteOldRows = false, Enums.ImportActionEnum actionType = Enums.ImportActionEnum.AddUpdate, string tableName = "", string getMethodTag = "", string addMethodTag = "", string updateMethodTag = "", string deleteMethodTag = "", string paramsTag = "", string keyTag = "", string rowTag = "")
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
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			if (dt != null)
			{
				if (deleteOldRows)
				{
					foreach (DataRow row in GetDocs(tableName).Rows)
					{
						if (dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t[keyFieldName]) == Function.ToString(row[keyFieldName]) && Function.ParseInt(row[keyFieldName]) != 0) == null)
						{
							list.Add(new BodyRequestObject
							{
								ObjectType = objType,
								OutObjectKey = true,
								Key = row[keyFieldName],
								Action = Enums.HttpAction.Delete
							});
						}
					}
				}
				for (int num = 0; num < dt.Rows.Count; num++)
				{
					switch (actionType)
					{
					case Enums.ImportActionEnum.Add:
						if (!Exists(Function.ToString(dt.Rows[num][keyFieldName]), tableName, keyFieldName))
						{
							string bodyObject = Mapping(dt.Rows[num], dicFieldTags);
							list.Add(new BodyRequestObject
							{
								ObjectType = objType,
								OutObjectKey = true,
								Key = dt.Rows[num][keyFieldName],
								Action = ((!Exists(Function.ToString(dt.Rows[num][keyFieldName]), tableName, keyFieldName)) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
								BodyObject = bodyObject
							});
						}
						break;
					case Enums.ImportActionEnum.Update:
						if (Exists(Function.ToString(dt.Rows[num][keyFieldName]), tableName, keyFieldName))
						{
							string bodyObject = Mapping(dt.Rows[num], dicFieldTags);
							list.Add(new BodyRequestObject
							{
								ObjectType = objType,
								OutObjectKey = true,
								Key = dt.Rows[num][keyFieldName],
								Action = ((!Exists(Function.ToString(dt.Rows[num][keyFieldName]), tableName, keyFieldName)) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
								BodyObject = bodyObject
							});
						}
						break;
					default:
					{
						string bodyObject = Mapping(dt.Rows[num], dicFieldTags);
						list.Add(new BodyRequestObject
						{
							ObjectType = objType,
							OutObjectKey = true,
							Key = dt.Rows[num][keyFieldName],
							Action = ((!Exists(Function.ToString(dt.Rows[num][keyFieldName]), tableName, keyFieldName)) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
							BodyObject = bodyObject
						});
						break;
					}
					}
				}
				return DataService.ProcessMutiTransaction(sessionId, list);
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
