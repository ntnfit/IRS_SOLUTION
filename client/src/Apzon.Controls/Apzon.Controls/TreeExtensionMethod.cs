using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls.Tree;
using Apzon.Global;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;
using DevExpress.XtraTreeList.ViewInfo;

namespace Apzon.Controls;

public static class TreeExtensionMethod
{
	public static bool ExcelToDataSource(this TreeEx tv, List<string> key = null)
	{
		try
		{
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (dataObject != null)
			{
				string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
				if (dataObject.GetDataPresent(DataFormats.Text))
				{
					DataTable dataTable = Function.PasteFromExcel(clipboardData);
					if (dataTable != null && (dataTable.Rows.Count > 0 || dataTable.Columns.Count > 0))
					{
						DataTable dataSourceTable = tv.DataSourceTable;
						int visibleIndex = tv.FocusedColumn.VisibleIndex;
						VisibleColumnsList visibleColumns = tv.VisibleColumns;
						int nodeID = tv.FocusedNode.Id;
						string fieldId = tv.KeyFieldName;
						string parentFieldName = tv.ParentFieldName;
						int num = Function.ParseInt(tv.FocusedNode.GetValue(parentFieldName));
						TreeListNode treeListNode = tv.FocusedNode;
						if (visibleColumns == null || visibleColumns.Count <= 0)
						{
							return false;
						}
						for (int i = 0; i < dataTable.Rows.Count; i++)
						{
							bool flag = false;
							if (treeListNode == null)
							{
								DataRow dataRow = dataSourceTable.NewRow();
								dataRow[fieldId] = dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t[fieldId])) + 1;
								treeListNode = tv.Nodes.Add(dataRow);
								treeListNode[parentFieldName] = num;
								tv.RefreshNode(treeListNode);
								nodeID = treeListNode.Id;
								flag = true;
							}
							int num2 = 0;
							for (int num3 = visibleIndex; num3 < visibleColumns.Count; num3++)
							{
								if (num2 >= dataTable.Columns.Count)
								{
									break;
								}
								if (visibleColumns[num3].ColumnType == typeof(DateTime))
								{
									treeListNode.SetValue(visibleColumns[num3], Function.ParseDateTime(dataTable.Rows[i][num2], GlobalData.DateTimeFormat));
									flag = false;
								}
								else
								{
									treeListNode.SetValue(visibleColumns[num3], dataTable.Rows[i][num2]);
									flag = false;
								}
								num2++;
							}
							if (num2 > 0 && !flag)
							{
								nodeID++;
							}
							tv.RefreshNode(treeListNode);
							treeListNode = tv.GetNodeList().FirstOrDefault((TreeListNode t) => t.Id == nodeID);
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		return true;
	}

	public static bool IsExistColumn(this TreeList tree, string fieldName)
	{
		try
		{
			return tree.Columns.FirstOrDefault((TreeListColumn t) => t.FieldName == fieldName) != null;
		}
		catch (Exception)
		{
			return false;
		}
	}

	public static string Format(this TimeSpan source, string timeFormat)
	{
		string text = "AM";
		if (timeFormat.ToUpper() == "12H")
		{
			int num = source.Hours;
			if (num > 12)
			{
				num -= 12;
				text = "PM";
			}
			return num.ToString("00") + ":" + source.Minutes.ToString("00") + " " + text;
		}
		return source.ToString("hh\\:mm");
	}
}
