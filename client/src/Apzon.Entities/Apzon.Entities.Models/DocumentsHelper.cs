using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.Service;
using Apzon.Entities.Models.UserFields;
using SAPbobsCOM;

namespace Apzon.Entities.Models;

public static class DocumentsHelper
{
	private static DocumentLineTable _lines;

	public static DataTable BatchSerialNumbersStruct { get; set; }

	public static DocumentTable Document { get; set; }

	public static DocumentLineTable Lines
	{
		get
		{
			return _lines;
		}
		set
		{
			_lines = value;
			_lines.ChangeColumnDataType("Quantity", typeof(double));
			_lines.ChangeColumnDataType("LineNum", typeof(int));
			_lines.ChangeColumnDataType("Price", typeof(double));
			_lines.ChangeColumnDataType("DiscPrcnt", typeof(double));
			_lines.ChangeColumnDataType("Rate", typeof(double));
			_lines.ChangeColumnDataType("LineTotal", typeof(double));
			_lines.ChangeColumnDataType("TotalFrgn", typeof(double));
			_lines.ChangeColumnDataType("LineVat", typeof(double));
			_lines.ChangeColumnDataType("PriceBefDi", typeof(double));
			_lines.ChangeColumnDataType("ShipDate", typeof(DateTime));
			_lines.ChangeColumnDataType("OpenQty", typeof(double));
			_lines.ChangeColumnDataType("BaseOpenQuantity", typeof(double));
			_lines.ChangeColumnDataType("VatSumFrgn", typeof(double));
			if (_lines != null && !_lines.Columns.Contains("SpecifiedPriceBefDi"))
			{
				_lines.Columns.Add("SpecifiedPriceBefDi", typeof(bool));
			}
			if (_lines != null && !_lines.Columns.Contains("SpecifiedPriceAfterVat"))
			{
				_lines.Columns.Add("SpecifiedPriceAfterVat", typeof(bool));
			}
		}
	}

	public static DataTable TablePayment { get; set; }

	public static DocPaymentTable DocPayments { get; set; }

	public static DocumentRow GetByEntry(this DocumentTable dt, int docEntry)
	{
		return (DocumentRow)dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["DocEntry"]).Equals(docEntry));
	}

	public static DocumentRow GetByNum(this DocumentTable dt, int docNum)
	{
		return (DocumentRow)dt.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["DocNum"]).Equals(docNum));
	}

	public static DocumentsImpl ToDocument(this DocumentRow dr, List<Tuple<string, string>> lstUdfNameAndType)
	{
		DocumentsImpl documentsImpl = new DocumentsImpl();
		try
		{
			IEnumerable<string> source = from DataColumn c in dr.Table.Columns
				select c.ColumnName;
			PropertyInfo[] properties = typeof(DocumentsImpl).GetProperties();
			foreach (PropertyInfo pro in properties)
			{
				if (source.Any((string t) => t.Equals(pro.Name)))
				{
					try
					{
						PropertyInfo property = documentsImpl.GetType().GetProperty(pro.Name);
						pro.SetValue(documentsImpl, (dr[pro.Name] == DBNull.Value || string.IsNullOrWhiteSpace(Function.ToString(dr[pro.Name]))) ? null : Convert.ChangeType(dr[pro.Name], property.PropertyType));
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
				}
			}
			List<string> udfCols = source.Where((string t) => t.StartsWith("U_")).ToList();
			documentsImpl.UserFields = new List<UserFieldsImpl>();
			if (udfCols.Count > 0)
			{
				int i;
				for (i = 0; i < udfCols.ToList().Count; i++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					userFieldsImpl.Name = udfCols.ToList()[i];
					Tuple<string, string> tuple = lstUdfNameAndType.FirstOrDefault((Tuple<string, string> t) => t.Item1 == udfCols.ToList()[i]);
					if (tuple != null)
					{
						if (tuple.Item2 == "datetime" || tuple.Item2 == "timestamp")
						{
							userFieldsImpl.Value = Function.ParseDateTime(dr[udfCols.ToList()[i]]);
							userFieldsImpl.Type = BoFieldTypes.db_Date;
						}
						else if (tuple.Item2 == "numeric" || tuple.Item2 == "double" || tuple.Item2 == "integer" || tuple.Item2 == "int" || tuple.Item2 == "smallint" || tuple.Item2 == "bigint" || tuple.Item2 == "long" || tuple.Item2 == "decimal")
						{
							userFieldsImpl.Value = dr[udfCols.ToList()[i]] ?? ((object)Function.ParseDecimal(dr[udfCols.ToList()[i]]));
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
						}
						else if (tuple.Item2 == "float")
						{
							userFieldsImpl.Value = dr[udfCols.ToList()[i]] ?? ((object)Function.ParseDouble(dr[udfCols.ToList()[i]]));
							userFieldsImpl.Type = BoFieldTypes.db_Float;
						}
						else
						{
							userFieldsImpl.Value = Function.ToString(dr[udfCols.ToList()[i]]);
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
						}
					}
					documentsImpl.UserFields.Add(userFieldsImpl);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			throw;
		}
		return documentsImpl;
	}

	public static DocumentsImpl ToDocument(this DataRow dr)
	{
		DocumentsImpl documentsImpl = new DocumentsImpl();
		try
		{
			IEnumerable<string> source = from DataColumn c in dr.Table.Columns
				select c.ColumnName;
			PropertyInfo[] properties = typeof(DocumentsImpl).GetProperties();
			foreach (PropertyInfo propertyInfo in properties)
			{
				if (source.Contains(propertyInfo.Name))
				{
					try
					{
						PropertyInfo property = documentsImpl.GetType().GetProperty(propertyInfo.Name);
						propertyInfo.SetValue(documentsImpl, (dr[propertyInfo.Name] == DBNull.Value || string.IsNullOrWhiteSpace(Function.ToString(dr[propertyInfo.Name]))) ? null : Convert.ChangeType(dr[propertyInfo.Name], property.PropertyType));
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
				}
			}
			List<string> list = source.Where((string t) => t.StartsWith("U_")).ToList();
			if (list.Count > 0)
			{
				documentsImpl.UserFields = new List<UserFieldsImpl>();
				for (int num2 = 0; num2 < list.Count; num2++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					userFieldsImpl.Name = list[num2];
					userFieldsImpl.Value = dr[list[num2]];
					Type dataType = dr.Table.Columns[list[num2]].DataType;
					if (dataType == typeof(DateTime))
					{
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (dataType == typeof(float))
					{
						userFieldsImpl.Type = BoFieldTypes.db_Float;
					}
					else if (dataType == typeof(double) || dataType == typeof(int) || dataType == typeof(short) || dataType == typeof(decimal))
					{
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					documentsImpl.UserFields.Add(userFieldsImpl);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			throw;
		}
		return documentsImpl;
	}

	public static List<Document_LinesImpl> ToDocumentLine(this DocumentLineTable dt)
	{
		List<string> source = (from DataColumn c in dt.Columns
			select c.ColumnName).ToList();
		List<string> list = source.Where((string t) => t.StartsWith("U_")).ToList();
		List<string> list2 = (from t in source
			where !t.StartsWith("U_")
			select t.ToUpper()).ToList();
		PropertyInfo[] properties = typeof(Document_LinesImpl).GetProperties();
		List<Document_LinesImpl> list3 = new List<Document_LinesImpl>();
		for (int num = 0; num < dt.Rows.Count; num++)
		{
			DataRow dataRow = dt.Rows[num];
			Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
			PropertyInfo[] array = properties;
			foreach (PropertyInfo propertyInfo in array)
			{
				if (list2.Contains(propertyInfo.Name.ToUpper()))
				{
					PropertyInfo property = document_LinesImpl.GetType().GetProperty(propertyInfo.Name);
					if (property != null)
					{
						Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
						object value = ((dataRow[propertyInfo.Name] == DBNull.Value || string.IsNullOrEmpty(Function.ToString(dataRow[propertyInfo.Name]))) ? null : Convert.ChangeType(dataRow[propertyInfo.Name], conversionType));
						propertyInfo.SetValue(document_LinesImpl, value, null);
					}
				}
			}
			if (list.Count > 0)
			{
				document_LinesImpl.UserFields = new List<UserFieldsImpl>();
				for (int num3 = 0; num3 < list.ToList().Count; num3++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					if (dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(DateTime))
					{
						userFieldsImpl.Value = Function.ParseDateTime(dataRow[list.ToList()[num3]]);
						userFieldsImpl.Type = BoFieldTypes.db_Date;
					}
					else if (dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(float))
					{
						userFieldsImpl.Value = Function.ParseFloat(dataRow[list.ToList()[num3]]);
						userFieldsImpl.Type = BoFieldTypes.db_Float;
					}
					else if (dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(double) || dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(int) || dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(short) || dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(decimal) || dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(long))
					{
						userFieldsImpl.Value = Function.ParseDouble(dataRow[list.ToList()[num3]]);
						userFieldsImpl.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						userFieldsImpl.Value = Function.ToString(dataRow[list.ToList()[num3]]);
						userFieldsImpl.Type = BoFieldTypes.db_Alpha;
					}
					userFieldsImpl.Name = list.ToList()[num3];
					document_LinesImpl.UserFields.Add(userFieldsImpl);
				}
			}
			document_LinesImpl.VisualOrder = Function.ParseInt(dataRow["VisOrder"]);
			list3.Add(document_LinesImpl);
		}
		return list3;
	}

	public static List<Document_LinesImpl> ToDocumentLine(this DocumentLineTable dt, Dictionary<string, string> lstUdfNameAndType, bool priceSpecified = false)
	{
		try
		{
			List<string> source = (from DataColumn c in dt.Columns
				select c.ColumnName).ToList();
			List<string> list = source.Where((string t) => t.StartsWith("U_")).ToList();
			List<string> list2 = (from t in source
				where !t.StartsWith("U_")
				select t.ToUpper()).ToList();
			PropertyInfo[] properties = typeof(Document_LinesImpl).GetProperties();
			List<Document_LinesImpl> list3 = new List<Document_LinesImpl>();
			for (int num = 0; num < dt.Rows.Count; num++)
			{
				DataRow dataRow = dt.Rows[num];
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (list2.Contains(propertyInfo.Name.ToUpper()))
					{
						PropertyInfo property = document_LinesImpl.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
							object value = ((dataRow[propertyInfo.Name] == DBNull.Value || string.IsNullOrEmpty(Function.ToString(dataRow[propertyInfo.Name]))) ? null : Convert.ChangeType(dataRow[propertyInfo.Name], conversionType));
							propertyInfo.SetValue(document_LinesImpl, value, null);
						}
					}
				}
				if (priceSpecified)
				{
					document_LinesImpl.SpecifiedPriceBefDi = priceSpecified;
				}
				if (list.Count > 0)
				{
					document_LinesImpl.UserFields = new List<UserFieldsImpl>();
					for (int num3 = 0; num3 < list.Count; num3++)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						string text = list[num3];
						if (lstUdfNameAndType.ContainsKey(text))
						{
							if (lstUdfNameAndType[text] == "datetime" || lstUdfNameAndType[text] == "timestamp")
							{
								userFieldsImpl.Value = Function.ParseDateTime(dataRow[text]);
								userFieldsImpl.Type = BoFieldTypes.db_Date;
							}
							else if (lstUdfNameAndType[text] == "integer" || lstUdfNameAndType[text] == "smallint" || lstUdfNameAndType[text] == "bigint" || lstUdfNameAndType[text] == "numeric" || lstUdfNameAndType[text] == "double" || lstUdfNameAndType[text] == "decimal")
							{
								userFieldsImpl.Value = dataRow[text] ?? Function.ToString(Function.ParseDecimal(dataRow[text]));
								userFieldsImpl.Type = BoFieldTypes.db_Numeric;
							}
							else if (lstUdfNameAndType[text] == "float")
							{
								userFieldsImpl.Value = dataRow[text] ?? Function.ToString(Function.ParseDouble(dataRow[text]));
								userFieldsImpl.Type = BoFieldTypes.db_Float;
							}
							else
							{
								userFieldsImpl.Value = Function.ToString(Function.ToString(dataRow[text]));
								userFieldsImpl.Type = BoFieldTypes.db_Alpha;
							}
						}
						userFieldsImpl.Name = text;
						document_LinesImpl.UserFields.Add(userFieldsImpl);
					}
				}
				document_LinesImpl.VisualOrder = Function.ParseInt(dataRow["VisOrder"]);
				list3.Add(document_LinesImpl);
			}
			return list3;
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static List<Document_LinesImpl> ToDocumentLine(this DataTable dt, bool priceSpecified = false)
	{
		try
		{
			List<string> columnNames = (from DataColumn c in dt.Columns
				select c.ColumnName).ToList();
			List<string> udfCols = columnNames.Where((string t) => t.StartsWith("U_")).ToList();
			PropertyInfo[] properties = typeof(Document_LinesImpl).GetProperties();
			return dt.AsEnumerable().Select((DataRow row) =>
			{
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (columnNames.Contains(propertyInfo.Name))
					{
						PropertyInfo property = document_LinesImpl.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
							object value = ((row[propertyInfo.Name] == DBNull.Value || string.IsNullOrWhiteSpace(Function.ToString(row[propertyInfo.Name]))) ? null : Convert.ChangeType(row[propertyInfo.Name], conversionType));
							propertyInfo.SetValue(document_LinesImpl, value, null);
						}
					}
				}
				if (priceSpecified)
				{
					document_LinesImpl.SpecifiedPriceBefDi = priceSpecified;
				}
				if (udfCols.Count > 0)
				{
					document_LinesImpl.UserFields = new List<UserFieldsImpl>();
					for (int j = 0; j < udfCols.ToList().Count; j++)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						if (row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(DateTime))
						{
							userFieldsImpl.Value = Function.ParseDateTime(row[udfCols.ToList()[j]]);
							userFieldsImpl.Type = BoFieldTypes.db_Date;
						}
						else if (row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(double) || row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(int) || row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(short) || row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(decimal) || row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(long))
						{
							userFieldsImpl.Value = row[udfCols.ToList()[j]];
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
						}
						else if (row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(float))
						{
							userFieldsImpl.Value = row[udfCols.ToList()[j]];
							userFieldsImpl.Type = BoFieldTypes.db_Float;
						}
						else
						{
							userFieldsImpl.Value = Function.ToString(Function.ToString(row[udfCols.ToList()[j]]));
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
						}
						userFieldsImpl.Name = udfCols.ToList()[j];
						document_LinesImpl.UserFields.Add(userFieldsImpl);
					}
				}
				return document_LinesImpl;
			}).ToList();
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static List<DocumentsAdditionalExpensesImpl> ToDocumentsAdditional(this DataTable dt)
	{
		try
		{
			List<string> columnNames = (from DataColumn c in dt.Columns
				select c.ColumnName).ToList();
			IEnumerable<string> udfCols = columnNames.Where((string t) => t.StartsWith("U_"));
			PropertyInfo[] properties = typeof(DocumentsAdditionalExpensesImpl).GetProperties();
			return dt.AsEnumerable().Select((DataRow row) =>
			{
				DocumentsAdditionalExpensesImpl documentsAdditionalExpensesImpl = new DocumentsAdditionalExpensesImpl();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (columnNames.Contains(propertyInfo.Name))
					{
						PropertyInfo property = documentsAdditionalExpensesImpl.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
							object value = ((row[propertyInfo.Name] == DBNull.Value) ? null : Convert.ChangeType(row[propertyInfo.Name], conversionType));
							propertyInfo.SetValue(documentsAdditionalExpensesImpl, value, null);
						}
					}
				}
				if (udfCols != null && udfCols.ToList() != null && udfCols.ToList().Count > 0)
				{
					documentsAdditionalExpensesImpl.UserFields = new List<UserFieldsImpl>();
					for (int j = 0; j < udfCols.ToList().Count; j++)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						if (row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(DateTime))
						{
							userFieldsImpl.Value = Function.ParseDateTime(row[udfCols.ToList()[j]]);
							userFieldsImpl.Type = BoFieldTypes.db_Date;
						}
						else if (row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(double))
						{
							userFieldsImpl.Value = Function.ParseDouble(row[udfCols.ToList()[j]]);
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
						}
						else
						{
							userFieldsImpl.Value = Function.ToString(row[udfCols.ToList()[j]]);
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
						}
						userFieldsImpl.Name = udfCols.ToList()[j];
						documentsAdditionalExpensesImpl.UserFields.Add(userFieldsImpl);
					}
				}
				return documentsAdditionalExpensesImpl;
			}).ToList();
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static List<KnowledgeBaseSolutionsImpl> ToDocumentsSolution(this DataTable dt)
	{
		try
		{
			List<string> columnNames = (from DataColumn c in dt.Columns
				select c.ColumnName).ToList();
			IEnumerable<string> udfCols = columnNames.Where((string t) => t.StartsWith("U_"));
			PropertyInfo[] properties = typeof(KnowledgeBaseSolutionsImpl).GetProperties();
			return dt.AsEnumerable().Select((DataRow row) =>
			{
				KnowledgeBaseSolutionsImpl knowledgeBaseSolutionsImpl = new KnowledgeBaseSolutionsImpl();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (columnNames.Contains(propertyInfo.Name))
					{
						PropertyInfo property = knowledgeBaseSolutionsImpl.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
							object value = ((row[propertyInfo.Name] == DBNull.Value) ? null : Convert.ChangeType(row[propertyInfo.Name], conversionType));
							propertyInfo.SetValue(knowledgeBaseSolutionsImpl, value, null);
						}
					}
				}
				if (udfCols != null && udfCols.ToList() != null && udfCols.ToList().Count > 0)
				{
					knowledgeBaseSolutionsImpl.UserFields = new List<UserFieldsImpl>();
					for (int j = 0; j < udfCols.ToList().Count; j++)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						if (row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(DateTime))
						{
							userFieldsImpl.Value = Function.ToString(Function.ParseDateTime(row[udfCols.ToList()[j]]));
							userFieldsImpl.Type = BoFieldTypes.db_Date;
						}
						else if (row.Table.Columns[udfCols.ToList()[j]].DataType == typeof(double))
						{
							userFieldsImpl.Value = Function.ToString(Function.ParseDouble(row[udfCols.ToList()[j]]));
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
						}
						else
						{
							userFieldsImpl.Value = Function.ToString(Function.ToString(row[udfCols.ToList()[j]]));
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
						}
						userFieldsImpl.Name = udfCols.ToList()[j];
						knowledgeBaseSolutionsImpl.UserFields.Add(userFieldsImpl);
					}
				}
				return knowledgeBaseSolutionsImpl;
			}).ToList();
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static List<AttachmentFile_Impl> ToDocumentAttachment(this DataTable dt)
	{
		try
		{
			List<string> columnNames = (from DataColumn c in dt.Columns
				select c.ColumnName).ToList();
			columnNames.Where((string t) => t.StartsWith("U_"));
			PropertyInfo[] properties = typeof(AttachmentFile_Impl).GetProperties();
			return dt.AsEnumerable().Select((DataRow row) =>
			{
				AttachmentFile_Impl attachmentFile_Impl = new AttachmentFile_Impl();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (columnNames.Contains(propertyInfo.Name))
					{
						PropertyInfo property = attachmentFile_Impl.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
							object value = ((row[propertyInfo.Name] == DBNull.Value) ? null : Convert.ChangeType(row[propertyInfo.Name], conversionType));
							propertyInfo.SetValue(attachmentFile_Impl, value, null);
						}
					}
				}
				return attachmentFile_Impl;
			}).ToList();
		}
		catch (Exception ex)
		{
			throw ex;
		}
	}

	public static int MaxEntry(this DocumentTable dt)
	{
		return Function.ParseInt(dt.AsEnumerable().Max((DataRow row) => row["DocEntry"]));
	}

	public static int Count(this DataTable dt)
	{
		return dt.Rows.Count;
	}

	public static object Sum(this DataTable dt, string field)
	{
		return (from dr in dt.AsEnumerable()
			select Function.ParseDouble(dr[field])).Sum();
	}

	internal static DocumentLineRow GetNewLine()
	{
		return Lines.GetClone().GetNewRow();
	}

	public static DocumentRow GetNewDocument()
	{
		return Document.GetClone().GetNewRow();
	}

	public static bool ChangeColumnDataType(this DataTable table, string columnname, Type newtype)
	{
		if (!table.Columns.Contains(columnname))
		{
			return false;
		}
		if (table.Columns[columnname].DataType == newtype)
		{
			return true;
		}
		try
		{
			using DataColumn dataColumn = new DataColumn(columnname + "_temp", newtype);
			int ordinal = table.Columns[columnname].Ordinal;
			table.Columns.Add(dataColumn);
			dataColumn.SetOrdinal(ordinal);
			foreach (DataRow row in table.Rows)
			{
				try
				{
					row[dataColumn.ColumnName] = Convert.ChangeType(row[columnname], newtype);
				}
				catch
				{
				}
			}
			table.Columns.Remove(columnname);
			dataColumn.ColumnName = columnname;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return false;
		}
		return true;
	}

	public static DocumentsImpl ToDocument(this DocumentRow dr, List<UserFieldsImpl> lstUdf = null)
	{
		DocumentsImpl documentsImpl = new DocumentsImpl();
		try
		{
			List<string> source = (from DataColumn c in dr.Table.Columns
				select c.ColumnName).ToList();
			PropertyInfo[] properties = typeof(DocumentsImpl).GetProperties();
			foreach (PropertyInfo pro in properties)
			{
				if (source.Where((string t) => t.ToUpper().Equals(pro.Name.ToUpper())).ToList().Count > 0)
				{
					try
					{
						PropertyInfo property = documentsImpl.GetType().GetProperty(pro.Name);
						pro.SetValue(documentsImpl, (dr[pro.Name] == DBNull.Value) ? null : Convert.ChangeType(dr[pro.Name], property.PropertyType));
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					}
				}
			}
			List<string> list = source.Where((string t) => t.StartsWith("U_")).ToList();
			if (list != null && list.ToList() != null && list.ToList().Count > 0)
			{
				documentsImpl.UserFields = new List<UserFieldsImpl>();
				List<string> list2 = list.ToList();
				for (int num2 = 0; num2 < list2.Count; num2++)
				{
					UserFieldsImpl item = new UserFieldsImpl();
					item.Name = list2[num2];
					if (lstUdf != null)
					{
						UserFieldsImpl userFieldsImpl = lstUdf.SingleOrDefault((UserFieldsImpl m) => ("U_" + m.Name).Equals(item.Name));
						if (userFieldsImpl != null)
						{
							BoFieldTypes type = userFieldsImpl.Type;
							item.Type = userFieldsImpl.Type;
							item.SubType = userFieldsImpl.SubType;
							switch (type)
							{
							case BoFieldTypes.db_Numeric:
								if (userFieldsImpl.SubType == BoFldSubTypes.st_Time)
								{
									item.Value = $"{Function.ToString(dr[list2[num2]])}";
								}
								else
								{
									item.Value = Function.ParseDouble(dr[list2[num2]]);
								}
								break;
							case BoFieldTypes.db_Float:
								item.Value = Function.ParseDouble(dr[list2[num2]]);
								break;
							case BoFieldTypes.db_Alpha:
								item.Value = Function.ToString(dr[list2[num2]]);
								break;
							case BoFieldTypes.db_Date:
								if (userFieldsImpl.SubType == BoFldSubTypes.st_Time)
								{
									item.Value = $"{Function.ToString(dr[list2[num2]])}";
								}
								else
								{
									item.Value = Function.ParseDateTime(dr[list2[num2]]);
								}
								break;
							case BoFieldTypes.db_Memo:
								item.Value = Function.ToString(dr[list2[num2]]);
								break;
							default:
								item.Value = Function.ToString(dr[list2[num2]]);
								break;
							}
						}
					}
					else if (dr.Table.Columns[list2[num2]].DataType == typeof(DateTime))
					{
						item.Value = Function.ParseDateTime(dr[list.ToList()[num2]]);
						item.Type = BoFieldTypes.db_Date;
					}
					else if (dr.Table.Columns[list2[num2]].DataType == typeof(double))
					{
						item.Value = Function.ParseDouble(dr[list2[num2]]);
						item.Type = BoFieldTypes.db_Numeric;
					}
					else
					{
						item.Value = Function.ToString(dr[list2[num2]]);
						item.Type = BoFieldTypes.db_Alpha;
					}
					documentsImpl.UserFields.Add(item);
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			throw;
		}
		return documentsImpl;
	}

	public static List<Document_LinesImpl> ToDocumentLine(this DocumentLineTable dt, List<UserFieldsImpl> lstUdf)
	{
		try
		{
			List<string> columnNames = (from DataColumn c in dt.Columns
				select c.ColumnName).ToList();
			List<string> udfCols = columnNames.Where((string t) => t.StartsWith("U_")).ToList();
			PropertyInfo[] properties = typeof(Document_LinesImpl).GetProperties();
			return dt.AsEnumerable().Select((DataRow row) =>
			{
				Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo pro in array)
				{
					if (columnNames.Any((string t) => t.ToUpper().Equals(pro.Name)))
					{
						PropertyInfo property = document_LinesImpl.GetType().GetProperty(pro.Name);
						if (property != null)
						{
							Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
							object value = ((row[pro.Name] == DBNull.Value || string.IsNullOrEmpty(Function.ToString(row[pro.Name]))) ? null : Convert.ChangeType(row[pro.Name], conversionType));
							pro.SetValue(document_LinesImpl, value, null);
						}
					}
				}
				if (udfCols != null && udfCols.ToList() != null && udfCols.ToList().Count > 0)
				{
					document_LinesImpl.UserFields = new List<UserFieldsImpl>();
					List<string> list = udfCols.ToList();
					for (int num = 0; num < list.Count; num++)
					{
						UserFieldsImpl item = new UserFieldsImpl();
						item.Name = list[num];
						UserFieldsImpl userFieldsImpl = lstUdf.SingleOrDefault((UserFieldsImpl m) => ("U_" + m.Name).Equals(item.Name));
						if (userFieldsImpl != null)
						{
							BoFieldTypes type = userFieldsImpl.Type;
							item.Type = userFieldsImpl.Type;
							item.SubType = userFieldsImpl.SubType;
							switch (type)
							{
							case BoFieldTypes.db_Numeric:
								if (userFieldsImpl.SubType == BoFldSubTypes.st_Time)
								{
									item.Value = $"{Function.ToString(row[list[num]])}";
								}
								else
								{
									item.Value = Function.ParseDouble(row[list[num]]);
								}
								break;
							case BoFieldTypes.db_Float:
								item.Value = Function.ParseDouble(row[list[num]]);
								break;
							case BoFieldTypes.db_Alpha:
								item.Value = Function.ToString(row[list[num]]);
								break;
							case BoFieldTypes.db_Date:
								if (userFieldsImpl.SubType == BoFldSubTypes.st_Time)
								{
									item.Value = $"{Function.ToString(row[list[num]])}";
								}
								else
								{
									item.Value = Function.ParseDateTime(row[list[num]]);
								}
								break;
							case BoFieldTypes.db_Memo:
								item.Value = Function.ToString(row[list[num]]);
								break;
							default:
								item.Value = Function.ToString(row[list[num]]);
								break;
							}
						}
						else if (row.Table.Columns[list[num]].DataType == typeof(DateTime))
						{
							item.Value = Function.ParseDateTime(row[udfCols.ToList()[num]]);
							item.Type = BoFieldTypes.db_Date;
						}
						else if (row.Table.Columns[list[num]].DataType == typeof(double))
						{
							item.Value = Function.ParseDouble(row[list[num]]);
							item.Type = BoFieldTypes.db_Numeric;
						}
						else
						{
							item.Value = Function.ToString(row[list[num]]);
							item.Type = BoFieldTypes.db_Alpha;
						}
						document_LinesImpl.UserFields.Add(item);
					}
				}
				document_LinesImpl.BatchNumbers = new List<BatchNumbersImpl>();
				document_LinesImpl.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
				return document_LinesImpl;
			}).ToList();
		}
		catch (Exception)
		{
			throw;
		}
	}

	public static void ToDocumentLineBatchSerial(this Document_LinesImpl line, DataTable dtBatchSerial, ApzObjectTypes formObjectType)
	{
		if (!dtBatchSerial.IsNotNull())
		{
			return;
		}
		List<DataColumn> list = (from DataColumn t in dtBatchSerial.Columns
			where Function.ToString(t.ColumnName).StartsWith("U_")
			select t).ToList();
		string text = Function.ToString(dtBatchSerial.Rows[0]["ObjType"]);
		DataTable dtBatchResult = new DataTable();
		dtBatchResult.Columns.Add("DistNumber", typeof(string));
		dtBatchResult.Columns.Add("LotNumber", typeof(string));
		dtBatchResult.Columns.Add("MnfSerial", typeof(string));
		dtBatchResult.Columns.Add("LogQty", typeof(double));
		if (!(text == "10000044"))
		{
			if (!(text == "10000045"))
			{
				return;
			}
			line.SerialNumbers = new List<SerialNumbersImpl>();
			if (line.BinAllocations == null)
			{
				line.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
			}
			DataRow[] array = (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["TranType"]) == 1
				select t).ToArray();
			foreach (DataRow dataRow in array)
			{
				SerialNumbersImpl serialNumbersImpl = new SerialNumbersImpl();
				serialNumbersImpl.BatchID = Function.ToString(dataRow["LotNumber"]);
				serialNumbersImpl.BaseLineNumber = Function.ParseInt(dataRow["BaseLineNumber"]);
				serialNumbersImpl.ReceptionDate = Function.ParseDateTime(dataRow["InDate"]);
				serialNumbersImpl.ExpiryDate = Function.ParseDateTime(dataRow["ExpDate"]);
				serialNumbersImpl.InternalSerialNumber = Function.ToString(dataRow["DistNumber"]);
				serialNumbersImpl.Location = Function.ToString(dataRow["Location"]);
				serialNumbersImpl.ManufacturerSerialNumber = Function.ToString(dataRow["MnfSerial"]);
				serialNumbersImpl.ManufactureDate = Function.ParseDateTime(dataRow["MnfDate"]);
				serialNumbersImpl.Notes = Function.ToString(dataRow["Notes"]);
				serialNumbersImpl.Quantity = 1.0;
				serialNumbersImpl.TrackingNote = Function.ParseInt(dataRow["TrackingNt"]);
				serialNumbersImpl.TrackingNoteLine = Function.ParseInt(dataRow["TrackiNtLn"]);
				serialNumbersImpl.SystemSerialNumber = Function.ParseInt(dataRow["SysNumber"]);
				if (list.Count > 0)
				{
					serialNumbersImpl.UserFields = new List<UserFieldsImpl>();
					foreach (DataColumn item4 in list)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						userFieldsImpl.Name = item4.ColumnName;
						Type dataType = item4.DataType;
						if (dataType == typeof(DateTime))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Date;
							userFieldsImpl.Value = Function.ParseDateTime(dataRow[item4.ColumnName]);
						}
						else if (dataType == typeof(float))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Float;
							userFieldsImpl.Value = Function.ParseDouble(dataRow[item4.ColumnName]);
						}
						else if (dataType == typeof(double))
						{
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
							userFieldsImpl.Value = Function.ParseDouble(dataRow[item4.ColumnName]);
						}
						else
						{
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
							userFieldsImpl.Value = Function.ToString(dataRow[item4.ColumnName]);
						}
						serialNumbersImpl.UserFields.Add(userFieldsImpl);
					}
				}
				line.SerialNumbers.Add(serialNumbersImpl);
			}
			DataRow[] array2 = (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["TranType"]) == -1
				select t).ToArray();
			foreach (DataRow dataRow2 in array2)
			{
				SerialNumbersImpl serialNumbersImpl2 = new SerialNumbersImpl();
				serialNumbersImpl2.BatchID = Function.ToString(dataRow2["LotNumber"]);
				serialNumbersImpl2.BaseLineNumber = Function.ParseInt(dataRow2["BaseLineNumber"]);
				serialNumbersImpl2.ReceptionDate = Function.ParseDateTime(dataRow2["InDate"]);
				serialNumbersImpl2.ExpiryDate = Function.ParseDateTime(dataRow2["ExpDate"]);
				serialNumbersImpl2.InternalSerialNumber = Function.ToString(dataRow2["DistNumber"]);
				serialNumbersImpl2.Location = Function.ToString(dataRow2["Location"]);
				serialNumbersImpl2.ManufacturerSerialNumber = Function.ToString(dataRow2["MnfSerial"]);
				serialNumbersImpl2.ManufactureDate = Function.ParseDateTime(dataRow2["MnfDate"]);
				serialNumbersImpl2.Notes = Function.ToString(dataRow2["Notes"]);
				serialNumbersImpl2.Quantity = 0.0;
				serialNumbersImpl2.TrackingNote = Function.ParseInt(dataRow2["TrackingNt"]);
				serialNumbersImpl2.TrackingNoteLine = Function.ParseInt(dataRow2["TrackiNtLn"]);
				serialNumbersImpl2.SystemSerialNumber = Function.ParseInt(dataRow2["SysNumber"]);
				if (list.Count > 0)
				{
					serialNumbersImpl2.UserFields = new List<UserFieldsImpl>();
					foreach (DataColumn item5 in list)
					{
						UserFieldsImpl userFieldsImpl2 = new UserFieldsImpl();
						userFieldsImpl2.Name = item5.ColumnName;
						Type dataType2 = item5.DataType;
						if (dataType2 == typeof(DateTime))
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Date;
							userFieldsImpl2.Value = Function.ParseDateTime(dataRow2[item5.ColumnName]);
						}
						else if (dataType2 == typeof(float))
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Float;
							userFieldsImpl2.Value = Function.ParseDouble(dataRow2[item5.ColumnName]);
						}
						else if (dataType2 == typeof(double))
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Numeric;
							userFieldsImpl2.Value = Function.ParseDouble(dataRow2[item5.ColumnName]);
						}
						else
						{
							userFieldsImpl2.Type = BoFieldTypes.db_Alpha;
							userFieldsImpl2.Value = Function.ToString(dataRow2[item5.ColumnName]);
						}
						serialNumbersImpl2.UserFields.Add(userFieldsImpl2);
					}
				}
				line.SerialNumbers.Add(serialNumbersImpl2);
			}
			if (formObjectType == ApzObjectTypes.oOrders)
			{
				return;
			}
			int j;
			for (j = 0; j < dtBatchSerial.Rows.Count; j++)
			{
				if (Function.ParseInt(dtBatchSerial.Rows[j]["BinAbs"]) > 0)
				{
					SerialNumbersImpl item = line.SerialNumbers.AsEnumerable().FirstOrDefault((SerialNumbersImpl t) => Function.ToString(t.InternalSerialNumber) == Function.ToString(dtBatchSerial.Rows[j]["DistNumber"]) && Function.ToString(t.BatchID) == Function.ToString(dtBatchSerial.Rows[j]["LotNumber"]) && Function.ToString(t.ManufacturerSerialNumber) == Function.ToString(dtBatchSerial.Rows[j]["MnfSerial"]));
					DocumentLinesBinAllocationsImpl item2 = new DocumentLinesBinAllocationsImpl
					{
						BaseLineNumber = Function.ParseInt(dtBatchSerial.Rows[j]["BaseLineNumber"]),
						BinAbsEntry = Function.ParseInt(dtBatchSerial.Rows[j]["BinAbs"]),
						Quantity = Function.ParseDouble(dtBatchSerial.Rows[j]["LogQty"]),
						SerialAndBatchNumbersBaseLine = line.SerialNumbers.IndexOf(item),
						BinActionType = ((Function.ParseInt(dtBatchSerial.Rows[j]["TranType"]) == 2) ? BinActionTypeEnum.batToWarehouse : BinActionTypeEnum.batFromWarehouse)
					};
					line.BinAllocations.Add(item2);
				}
			}
			return;
		}
		line.BatchNumbers = new List<BatchNumbersImpl>();
		if (line.BinAllocations == null)
		{
			line.BinAllocations = new List<DocumentLinesBinAllocationsImpl>();
		}
		DataRow[] batchResult = (from row in (from t in dtBatchSerial.AsEnumerable()
				where Function.ParseInt(t["TranType"]) == 1
				select t).AsEnumerable()
			group row by new
			{
				DistNumber = Function.ToString(row["DistNumber"])
			} into g
			select dtBatchResult.LoadDataRow(new object[4]
			{
				g.Key.DistNumber,
				"",
				"",
				Function.ParseDouble(g.Sum((DataRow r) => Function.ParseDouble(r["LogQty"])))
			}, fAcceptChanges: false)).ToArray();
		int j2;
		for (j2 = 0; j2 < batchResult.Length; j2++)
		{
			DataRow dataRow3 = dtBatchSerial.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["DistNumber"]) == Function.ToString(batchResult[j2]["DistNumber"]) && Function.ParseInt(t["TranType"]) == 1);
			if (dataRow3 == null)
			{
				continue;
			}
			BatchNumbersImpl batchNumbersImpl = new BatchNumbersImpl
			{
				BatchNumber = Function.ToString(dataRow3["DistNumber"]),
				BaseLineNumber = Function.ParseInt(dataRow3["BaseLineNumber"]),
				AddmisionDate = Function.ParseDateTime(dataRow3["InDate"]),
				ExpiryDate = Function.ParseDateTime(dataRow3["ExpDate"]),
				InternalSerialNumber = Function.ToString(dataRow3["LotNumber"]),
				Location = Function.ToString(dataRow3["Location"]),
				ManufacturerSerialNumber = Function.ToString(dataRow3["MnfSerial"]),
				ManufacturingDate = Function.ParseDateTime(dataRow3["MnfDate"]),
				Notes = Function.ToString(dataRow3["Notes"]),
				Quantity = Function.ParseDouble(batchResult[j2]["LogQty"]),
				TrackingNote = Function.ParseInt(dataRow3["TrackingNt"]),
				TrackingNoteLine = Function.ParseInt(dataRow3["TrackiNtLn"])
			};
			if (list.Count > 0)
			{
				batchNumbersImpl.UserFields = new List<UserFieldsImpl>();
				foreach (DataColumn item6 in list)
				{
					UserFieldsImpl userFieldsImpl3 = new UserFieldsImpl
					{
						Name = item6.ColumnName
					};
					Type dataType3 = item6.DataType;
					if (dataType3 == typeof(DateTime))
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Date;
						userFieldsImpl3.Value = Function.ParseDateTime(dataRow3[item6.ColumnName]);
					}
					else if (dataType3 == typeof(float))
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Float;
						userFieldsImpl3.Value = Function.ParseDouble(dataRow3[item6.ColumnName]);
					}
					else if (dataType3 == typeof(double))
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Numeric;
						userFieldsImpl3.Value = Function.ParseDouble(dataRow3[item6.ColumnName]);
					}
					else
					{
						userFieldsImpl3.Type = BoFieldTypes.db_Alpha;
						userFieldsImpl3.Value = Function.ToString(dataRow3[item6.ColumnName]);
					}
					batchNumbersImpl.UserFields.Add(userFieldsImpl3);
				}
			}
			line.BatchNumbers.Add(batchNumbersImpl);
		}
		DataRow[] array3 = (from t in dtBatchSerial.AsEnumerable()
			where Function.ParseInt(t["TranType"]) == -1
			select t).ToArray();
		for (int num3 = 0; num3 < array3.Length; num3++)
		{
			BatchNumbersImpl batchNumbersImpl2 = new BatchNumbersImpl
			{
				BatchNumber = Function.ToString(array3[num3]["DistNumber"]),
				BaseLineNumber = Function.ParseInt(array3[num3]["BaseLineNumber"]),
				AddmisionDate = Function.ParseDateTime(array3[num3]["InDate"]),
				ExpiryDate = Function.ParseDateTime(array3[num3]["ExpDate"]),
				InternalSerialNumber = Function.ToString(array3[num3]["LotNumber"]),
				Location = Function.ToString(array3[num3]["Location"]),
				ManufacturerSerialNumber = Function.ToString(array3[num3]["MnfSerial"]),
				ManufacturingDate = Function.ParseDateTime(array3[num3]["MnfDate"]),
				Notes = Function.ToString(array3[num3]["Notes"]),
				Quantity = 0.0,
				TrackingNote = Function.ParseInt(array3[num3]["TrackingNt"]),
				TrackingNoteLine = Function.ParseInt(array3[num3]["TrackiNtLn"])
			};
			if (list.Count > 0)
			{
				batchNumbersImpl2.UserFields = new List<UserFieldsImpl>();
				foreach (DataColumn item7 in list)
				{
					UserFieldsImpl userFieldsImpl4 = new UserFieldsImpl
					{
						Name = item7.ColumnName
					};
					Type dataType4 = item7.DataType;
					if (dataType4 == typeof(DateTime))
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Date;
						userFieldsImpl4.Value = Function.ParseDateTime(array3[num3][item7.ColumnName]);
					}
					else if (dataType4 == typeof(float))
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Float;
						userFieldsImpl4.Value = Function.ParseDouble(array3[num3][item7.ColumnName]);
					}
					else if (dataType4 == typeof(double))
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Numeric;
						userFieldsImpl4.Value = Function.ParseDouble(array3[num3][item7.ColumnName]);
					}
					else
					{
						userFieldsImpl4.Type = BoFieldTypes.db_Alpha;
						userFieldsImpl4.Value = Function.ToString(array3[num3][item7.ColumnName]);
					}
					batchNumbersImpl2.UserFields.Add(userFieldsImpl4);
				}
			}
			line.BatchNumbers.Add(batchNumbersImpl2);
		}
		if (formObjectType == ApzObjectTypes.oOrders)
		{
			return;
		}
		int j3;
		for (j3 = 0; j3 < dtBatchSerial.Rows.Count; j3++)
		{
			if (Function.ParseInt(dtBatchSerial.Rows[j3]["BinAbs"]) > 0)
			{
				BatchNumbersImpl batchNumbersImpl3 = line.BatchNumbers.AsEnumerable().FirstOrDefault((BatchNumbersImpl t) => Function.ToString(t.BatchNumber) == Function.ToString(dtBatchSerial.Rows[j3]["DistNumber"]));
				if (batchNumbersImpl3 != null)
				{
					DocumentLinesBinAllocationsImpl item3 = new DocumentLinesBinAllocationsImpl
					{
						BaseLineNumber = Function.ParseInt(dtBatchSerial.Rows[j3]["BaseLineNumber"]),
						BinAbsEntry = Function.ParseInt(dtBatchSerial.Rows[j3]["BinAbs"]),
						Quantity = Function.ParseDouble(dtBatchSerial.Rows[j3]["LogQty"]),
						SerialAndBatchNumbersBaseLine = line.BatchNumbers.IndexOf(batchNumbersImpl3),
						BinActionType = ((Function.ParseInt(dtBatchSerial.Rows[j3]["TranType"]) == 2) ? BinActionTypeEnum.batToWarehouse : BinActionTypeEnum.batFromWarehouse)
					};
					line.BinAllocations.Add(item3);
				}
			}
		}
	}

	public static List<AttachmentFile_Impl> ToAttachmentFile(this DataTable dt)
	{
		try
		{
			List<string> source = (from DataColumn c in dt.Columns
				select c.ColumnName).ToList();
			List<string> list = source.Where((string t) => t.StartsWith("U_")).ToList();
			List<string> list2 = (from t in source
				where !t.StartsWith("U_")
				select t.ToUpper()).ToList();
			PropertyInfo[] properties = typeof(AttachmentFile_Impl).GetProperties();
			List<AttachmentFile_Impl> list3 = new List<AttachmentFile_Impl>();
			for (int num = 0; num < dt.Rows.Count; num++)
			{
				DataRow dataRow = dt.Rows[num];
				AttachmentFile_Impl attachmentFile_Impl = new AttachmentFile_Impl();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (list2.Contains(propertyInfo.Name.ToUpper()))
					{
						PropertyInfo property = attachmentFile_Impl.GetType().GetProperty(propertyInfo.Name);
						if (property != null)
						{
							Type conversionType = Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType;
							object value = ((dataRow[propertyInfo.Name] == DBNull.Value || string.IsNullOrEmpty(Function.ToString(dataRow[propertyInfo.Name]))) ? null : Convert.ChangeType(dataRow[propertyInfo.Name], conversionType));
							propertyInfo.SetValue(attachmentFile_Impl, value, null);
						}
					}
				}
				if (list.Count > 0)
				{
					attachmentFile_Impl.UserFields = new List<UserFieldsImpl>();
					for (int num3 = 0; num3 < list.ToList().Count; num3++)
					{
						UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
						if (dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(DateTime))
						{
							userFieldsImpl.Value = Function.ParseDateTime(dataRow[list.ToList()[num3]]);
							userFieldsImpl.Type = BoFieldTypes.db_Date;
						}
						else if (dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(float))
						{
							userFieldsImpl.Value = Function.ParseFloat(dataRow[list.ToList()[num3]]);
							userFieldsImpl.Type = BoFieldTypes.db_Float;
						}
						else if (dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(double) || dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(int) || dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(short) || dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(decimal) || dataRow.Table.Columns[list.ToList()[num3]].DataType == typeof(long))
						{
							userFieldsImpl.Value = Function.ParseDouble(dataRow[list.ToList()[num3]]);
							userFieldsImpl.Type = BoFieldTypes.db_Numeric;
						}
						else
						{
							userFieldsImpl.Value = Function.ToString(dataRow[list.ToList()[num3]]);
							userFieldsImpl.Type = BoFieldTypes.db_Alpha;
						}
						userFieldsImpl.Name = list.ToList()[num3];
						attachmentFile_Impl.UserFields.Add(userFieldsImpl);
					}
				}
				list3.Add(attachmentFile_Impl);
			}
			return list3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			throw;
		}
	}
}
