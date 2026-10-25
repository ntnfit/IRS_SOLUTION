using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Apzon.Commons;

namespace Apzon.DataServices.Implements;

public static class DataSetXml
{
	public static string ToXml(this DataRow dr)
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < dr.Table.Columns.Count; i++)
		{
			DataColumn dataColumn = dr.Table.Columns[i];
			string columnName = dataColumn.ColumnName;
			if (dr.Table.Columns[i].DataType == typeof(DateTime) && Function.ParseDateTime(dr[columnName]).HasValue)
			{
				stringBuilder.Append(string.Format("<{0}>{1}</{0}>", columnName, Convert.ToDateTime(dr[columnName]).ToString("yyyy-MM-dd")));
			}
			else if (dataColumn.DataType == typeof(decimal) || dataColumn.DataType == typeof(double) || dataColumn.DataType == typeof(float))
			{
				stringBuilder.Append(string.Format("<{0}>{1}</{0}>", columnName, Function.ParseDecimal(dr[columnName])));
			}
			else if (dataColumn.DataType == typeof(int) || dataColumn.DataType == typeof(long) || dataColumn.DataType == typeof(short))
			{
				stringBuilder.Append(string.Format("<{0}>{1}</{0}>", columnName, Function.ParseInt(dr[columnName])));
			}
			else
			{
				stringBuilder.Append(string.Format("<{0}>{1}</{0}>", columnName, Function.RawText(dr[columnName])));
			}
		}
		return stringBuilder.ToString();
	}

	public static string ToXml(this DataTable dt)
	{
		string text = dt.TableName ?? "NewTable";
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<" + text + "s>");
		for (int i = 0; i < dt.Rows.Count; i++)
		{
			stringBuilder.Append("<" + text + ">");
			stringBuilder.Append("\n").Append(dt.Rows[i].ToXml());
			stringBuilder.Append("<" + text + ">");
		}
		stringBuilder.Append("</" + text + "s>");
		return stringBuilder.ToString();
	}

	public static string ToXml(this DataSet ds)
	{
		string text = ds.DataSetName ?? "NewDataSet";
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<" + text + ">");
		for (int i = 0; i < ds.Tables.Count; i++)
		{
			stringBuilder.Append(ds.Tables[i].ToXml());
		}
		stringBuilder.Append("</" + text + ">");
		return stringBuilder.ToString();
	}

	public static DataTable ToTable(this XElement ele)
	{
		DataTable dataTable = new DataTable();
		XElement xElement = (from p in ele.Descendants()
			select (p)).First();
		foreach (XElement item in xElement.Descendants())
		{
			dataTable.Columns.Add(new DataColumn(item.Name.ToString(), typeof(string)));
		}
		foreach (XElement item2 in from p in ele.Descendants(xElement.Name.ToString())
			select (p))
		{
			DataRow dataRow = dataTable.NewRow();
			foreach (XElement item3 in item2.Descendants())
			{
				bool flag = false;
				foreach (DataColumn column in dataTable.Columns)
				{
					if (column.ColumnName == item3.Name.ToString())
					{
						flag = true;
						break;
					}
				}
				if (flag)
				{
					dataRow[item3.Name.ToString()] = item3.Value;
				}
			}
			dataTable.Rows.Add(dataRow);
		}
		return dataTable;
	}
}
