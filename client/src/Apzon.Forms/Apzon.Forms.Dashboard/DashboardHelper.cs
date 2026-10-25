using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.DashboardCommon;
using DevExpress.Data;
using DevExpress.DataAccess;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Dashboard;

public static class DashboardHelper
{
	private static readonly IHttpClient HttpClient = HttpClientHelperFactory.GetHttpClient();

	public static DevExpress.DashboardCommon.Dashboard LoadDashboardFromXml(this Control control, XDocument xml)
	{
		DevExpress.DashboardCommon.Dashboard dashboard = new DevExpress.DashboardCommon.Dashboard();
		dashboard.LoadFromXDocument(xml);
		List<Tuple<string, DashboardObjectDataSource>> list = new List<Tuple<string, DashboardObjectDataSource>>();
		foreach (IDashboardDataSource dataSource in dashboard.DataSources)
		{
			if (!(dataSource is DashboardSqlDataSource))
			{
				continue;
			}
			DashboardSqlDataSource dashboardSqlDataSource = (DashboardSqlDataSource)dataSource;
			foreach (SqlQuery query in dashboardSqlDataSource.Queries)
			{
				if (query is StoredProcQuery)
				{
					StoredProcQuery storedprocQry = (StoredProcQuery)query;
					string storedProcName = storedprocQry.StoredProcName;
					List<QueryParamater> lstPr = new List<QueryParamater>();
					storedProcName = GetParameters(control, storedprocQry.Parameters, ref lstPr, storedProcName, dashboard);
					DataSet dataSet = HttpClient.Commons.ExecuteSql(storedProcName, lstPr);
					dataSet.Tables[1].TableName = storedprocQry.Name;
					if (dataSet.Tables[1] != null && dataSet.Tables[1].Columns.Count > 0)
					{
						foreach (CalculatedField item in dashboardSqlDataSource.CalculatedFields.Where((CalculatedField t) => t.DataMember == storedprocQry.Name))
						{
							DataColumn dataColumn = new DataColumn(item.Name);
							dataColumn.Expression = item.Expression;
							dataSet.Tables[1].Columns.Add(dataColumn);
						}
					}
					DashboardObjectDataSource dashboardObjectDataSource = new DashboardObjectDataSource();
					dashboardObjectDataSource.Name = storedprocQry.Name;
					dashboardObjectDataSource.DataSource = dataSet.Tables[1].Copy();
					dashboardObjectDataSource.Constructor = ObjectConstructorInfo.Default;
					dashboardObjectDataSource.Fill();
					list.Add(new Tuple<string, DashboardObjectDataSource>(dashboardObjectDataSource.Name, dashboardObjectDataSource));
				}
				else
				{
					if (!(query is SelectQuery))
					{
						continue;
					}
					SelectQuery selectQuery = (SelectQuery)query;
					string text = "SELECT ";
					List<string> list2 = new List<string>();
					foreach (ColumnBase column2 in selectQuery.Columns)
					{
						if (column2 is AllColumns)
						{
							AllColumns allColumns = (AllColumns)column2;
							list2.Add(allColumns.Table.HasAlias ? allColumns.Table.Alias : (allColumns.Table.ActualName + ".*"));
						}
						else
						{
							if (!(column2 is Column))
							{
								continue;
							}
							Column column = (Column)column2;
							if (column.Expression is ColumnExpression columnExpression)
							{
								string text2 = (columnExpression.Table.HasAlias ? columnExpression.Table.Alias : (columnExpression.Table.ActualName + "." + columnExpression.ColumnName));
								string text3 = (column.HasAlias ? column.Alias : column.ActualName);
								switch (columnExpression.Aggregate)
								{
								case AggregationType.None:
									list2.Add(text2 + " " + text3);
									break;
								case AggregationType.Avg:
									list2.Add($"AVG({text2}) {text3}");
									break;
								case AggregationType.AvgDistinct:
									list2.Add($"AVG( DISTINCT {text2})  {text3}");
									break;
								case AggregationType.CountDistinct:
									list2.Add($"COUNT( DISTINCT {text2})  {text3}");
									break;
								case AggregationType.Count:
									list2.Add($"COUNT({text2})  {text3}");
									break;
								case AggregationType.Max:
									list2.Add($"MAX({text2}) {text3} ");
									break;
								case AggregationType.Min:
									list2.Add($"MIN({text2})  {text3}");
									break;
								case AggregationType.Sum:
									list2.Add($"SUM({text2})  {text3} ");
									break;
								case AggregationType.SumDistinct:
									list2.Add($"SUM( DISTINCT {text2})  {text3} ");
									break;
								}
							}
						}
					}
					text += string.Join(",", list2.ToArray());
					List<string> list3 = new List<string>();
					List<string> list4 = new List<string>();
					foreach (Table table in selectQuery.Tables)
					{
						string text4 = table.ActualName + " " + (table.HasAlias ? table.Alias : "");
						if (list4.Contains(table.ActualName))
						{
							continue;
						}
						IEnumerable<Join> enumerable = selectQuery.Relations.Where((Join t) => t.Parent.ActualName == table.ActualName);
						list4.Add(table.ActualName);
						foreach (Join item2 in enumerable)
						{
							string text5 = (item2.Parent.HasAlias ? item2.Parent.Alias : item2.Parent.ActualName);
							string text6 = (item2.Nested.HasAlias ? item2.Parent.Alias : item2.Nested.ActualName);
							if (list4.Contains(item2.Nested.ActualName))
							{
								continue;
							}
							list4.Add(item2.Nested.ActualName);
							text4 += string.Format(" {0} {1}   ON ", (item2.Type == JoinType.Inner) ? " INNER JOIN " : " LEFT OUTER JOIN ", text6);
							List<string> list5 = new List<string>();
							foreach (RelationColumnInfo keyColumn in item2.KeyColumns)
							{
								string text7 = GetOperator(keyColumn.ConditionOperator);
								list5.Add($"{text5}.{keyColumn.ParentKeyColumn} {text7} {text6}.{keyColumn.NestedKeyColumn}");
							}
							text4 += string.Join(" AND ", list5.ToArray());
						}
						list3.Add(text4);
					}
					text = text + " FROM " + string.Join(",", list3.ToArray());
					List<string> list6 = new List<string>();
					foreach (Group group in selectQuery.Groups)
					{
						if (group.Expression is ColumnExpression columnExpression2)
						{
							string text8 = (columnExpression2.Table.HasAlias ? columnExpression2.Table.Alias : (columnExpression2.Table.ActualName + "." + columnExpression2.ColumnName));
							switch (columnExpression2.Aggregate)
							{
							case AggregationType.None:
								list6.Add(text8);
								break;
							case AggregationType.Avg:
								list6.Add($"AVG({text8})");
								break;
							case AggregationType.AvgDistinct:
								list6.Add($"AVG( DISTINCT {text8}) ");
								break;
							case AggregationType.CountDistinct:
								list6.Add($"COUNT( DISTINCT {text8}) ");
								break;
							case AggregationType.Count:
								list6.Add($"COUNT({text8}) ");
								break;
							case AggregationType.Max:
								list6.Add($"MAX({text8}) ");
								break;
							case AggregationType.Min:
								list6.Add($"MIN({text8}) ");
								break;
							case AggregationType.Sum:
								list6.Add($"SUM({text8})  ");
								break;
							case AggregationType.SumDistinct:
								list6.Add($"SUM( DISTINCT {text8})  ");
								break;
							}
						}
					}
					if (list6.Count > 0)
					{
						text = text + " GROUP BY " + string.Join(",", list6);
					}
					List<QueryParamater> lstPr2 = new List<QueryParamater>();
					text = GetParameters(control, selectQuery.Parameters, ref lstPr2, text, dashboard);
					DataSet dataSet2 = HttpClient.Commons.ExecuteSql(text, lstPr2);
					dataSet2.Tables[1].TableName = selectQuery.Name;
					if (dataSet2.Tables[1] != null && dataSet2.Tables[1].Columns.Count > 0)
					{
						foreach (CalculatedField item3 in dashboardSqlDataSource.CalculatedFields.Where((CalculatedField t) => t.DataMember == selectQuery.Name))
						{
							DataColumn dataColumn2 = new DataColumn(item3.Name);
							dataColumn2.Expression = item3.Expression;
							dataSet2.Tables[1].Columns.Add(dataColumn2);
						}
					}
					DashboardObjectDataSource dashboardObjectDataSource2 = new DashboardObjectDataSource();
					dashboardObjectDataSource2.Name = selectQuery.Name;
					dashboardObjectDataSource2.DataSource = dataSet2.Tables[1].Copy();
					dashboardObjectDataSource2.Constructor = ObjectConstructorInfo.Default;
					dashboardObjectDataSource2.Fill();
					list.Add(new Tuple<string, DashboardObjectDataSource>(dashboardObjectDataSource2.Name, dashboardObjectDataSource2));
				}
			}
		}
		foreach (Tuple<string, DashboardObjectDataSource> obj in list)
		{
			if (obj == null)
			{
				continue;
			}
			foreach (DashboardItem item4 in dashboard.Items.Where((DashboardItem t) => t is DataDashboardItem && ((DataDashboardItem)t).DataSource != null && ((DataDashboardItem)t).DataMember == obj.Item1))
			{
				if (item4 is DataDashboardItem)
				{
					((DataDashboardItem)item4).DataSource = obj.Item2;
					((DataDashboardItem)item4).DataMember = null;
				}
			}
		}
		for (int num = 0; num < dashboard.DataSources.Count; num++)
		{
			if (dashboard.DataSources[num] is DashboardSqlDataSource)
			{
				dashboard.DataSources.Remove(dashboard.DataSources[num]);
				num--;
			}
		}
		dashboard.Parameters.Clear();
		return dashboard;
	}

	public static async Task<DevExpress.DashboardCommon.Dashboard> LoadDashboardFromSource(this Control control, int dashboardId, DataTable dt)
	{
		if (dashboardId > 0)
		{
			DataTable dataTable = await HttpClient.DashboardDetail.GetById(dashboardId);
			if (dataTable.IsNotNull())
			{
				XDocument xml = XDocument.Parse(Function.ToString(dataTable.Rows[0]["Layout"]));
				DevExpress.DashboardCommon.Dashboard dashboard = control.LoadDashboardFromXml(xml);
				foreach (IDashboardDataSource dataSource in dashboard.DataSources)
				{
					if (dataSource is DashboardObjectDataSource)
					{
						((DashboardObjectDataSource)dataSource).DataSource = dt.Copy();
					}
				}
				return dashboard;
			}
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Could not get data dashboard");
		}
		return null;
	}

	private static string GetOperator(ConditionType condition)
	{
		string result = "";
		switch (condition)
		{
		case ConditionType.Equal:
			result = "=";
			break;
		case ConditionType.Greater:
			result = ">";
			break;
		case ConditionType.GreaterOrEqual:
			result = ">=";
			break;
		case ConditionType.Less:
			result = "<";
			break;
		case ConditionType.LessOrEqual:
			result = "<=";
			break;
		case ConditionType.NotEqual:
			result = "<>";
			break;
		}
		return result;
	}

	private static string GetParameters(Control control, List<QueryParameter> parameters, ref List<QueryParamater> lstPr, string sql, DevExpress.DashboardCommon.Dashboard dashboard)
	{
		int num = 0;
		GridControl gridControl = new GridControl
		{
			Dock = DockStyle.Fill
		};
		GridView gridView = new GridView(gridControl);
		gridControl.ViewCollection.Add(gridView);
		gridControl.MainView = gridView;
		gridControl.Size = new Size(0, 0);
		gridView.Columns.Add(new GridColumn
		{
			FieldName = "Id",
			Visible = true,
			Caption = "Id"
		});
		gridView.GridControl = gridControl;
		control.Controls.Add(gridControl);
		foreach (QueryParameter parameter in parameters)
		{
			if (string.IsNullOrEmpty(parameter.Name))
			{
				continue;
			}
			QueryParamater pr = new QueryParamater();
			pr.ParaName = "[%" + num + "]";
			if (parameter.Value is Expression expression)
			{
				object value = null;
				Expression expression2 = expression;
				Type resultType = expression2.ResultType;
				if (Function.ToString(expression2).StartsWith("[Parameters."))
				{
					string text = Function.ToString(expression2);
					string paramname = text.Replace("[", "").Replace("]", "").Split('.')[1];
					DashboardParameter dashboardParameter = dashboard.Parameters.FirstOrDefault((DashboardParameter t) => t.Name == paramname);
					if (dashboardParameter != null)
					{
						value = dashboardParameter.Value;
					}
				}
				else
				{
					if (resultType == typeof(DateTime) || resultType == typeof(DateTime?))
					{
						gridView.Columns.Add(new GridColumn
						{
							UnboundExpression = Function.ToString(expression2),
							UnboundType = UnboundColumnType.DateTime,
							Visible = true,
							Caption = Function.ToString(expression2),
							FieldName = num.ToString()
						});
					}
					else if (resultType == typeof(int) || typeof(short) == resultType)
					{
						gridView.Columns.Add(new GridColumn
						{
							UnboundType = UnboundColumnType.Integer,
							UnboundExpression = Function.ToString(expression2),
							Visible = true,
							Caption = Function.ToString(expression2),
							FieldName = num.ToString()
						});
					}
					else if (resultType == typeof(long))
					{
						gridView.Columns.Add(new GridColumn
						{
							UnboundType = UnboundColumnType.Integer,
							UnboundExpression = Function.ToString(expression2),
							Visible = true,
							Caption = Function.ToString(expression2),
							FieldName = num.ToString()
						});
					}
					else if (resultType == typeof(double) || resultType == typeof(decimal) || resultType == typeof(float))
					{
						gridView.Columns.Add(new GridColumn
						{
							UnboundType = UnboundColumnType.Decimal,
							UnboundExpression = Function.ToString(expression2),
							Visible = true,
							Caption = Function.ToString(expression2),
							FieldName = num.ToString()
						});
					}
					else if (resultType == typeof(bool))
					{
						gridView.Columns.Add(new GridColumn
						{
							UnboundType = UnboundColumnType.Boolean,
							UnboundExpression = Function.ToString(expression2),
							Visible = true,
							Caption = Function.ToString(expression2),
							FieldName = num.ToString()
						});
					}
					else
					{
						gridView.Columns.Add(new GridColumn
						{
							UnboundType = UnboundColumnType.Object,
							UnboundExpression = Function.ToString(expression2),
							Visible = true,
							Caption = Function.ToString(expression2),
							FieldName = num.ToString()
						});
					}
					DataTable dataTable = new DataTable();
					dataTable.Columns.Add("Id", typeof(int));
					DataRow dataRow = dataTable.NewRow();
					dataRow["Id"] = 0;
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
					gridControl.DataSource = dataTable;
					gridControl.RefreshDataSource();
					value = gridView.GetRowCellValue(0, gridView.Columns.Last());
				}
				SetParmeterValue(resultType, value, ref pr);
			}
			else if (parameter.Value != null)
			{
				SetParmeterValue(parameter.Value.GetType(), parameter.Value, ref pr);
			}
			lstPr.Add(pr);
			num++;
		}
		if (lstPr.Count > 0)
		{
			sql = sql + " " + string.Join(",", lstPr.Select((QueryParamater t) => t.ParaName));
		}
		return sql;
	}

	private static void SetParmeterValue(Type type, object value, ref QueryParamater pr)
	{
		if (type == typeof(DateTime) || type == typeof(DateTime?))
		{
			pr.DataType = "datetime";
			pr.Value = Function.ParseDateTime(value);
		}
		else if (type == typeof(int) || typeof(short) == type)
		{
			pr.DataType = "int";
			pr.Value = Function.ParseInt(value);
		}
		else if (type == typeof(long))
		{
			pr.DataType = "bigint";
			pr.Value = Function.ParseLong(value);
		}
		else if (type == typeof(double) || type == typeof(decimal) || type == typeof(float))
		{
			pr.DataType = "numeric";
			pr.Value = Function.ParseDouble(value);
		}
		else
		{
			pr.Value = Function.ToString(value);
		}
	}

	public static DataTable GetDashboardSourceByReport(DataTable drPr, string storename)
	{
		try
		{
			List<QueryParamater> list = new List<QueryParamater>();
			foreach (DataRow row in drPr.Rows)
			{
				QueryParamater queryParamater = new QueryParamater
				{
					ParaName = Function.ToString(row["ParamName"])
				};
				object obj = row["DefaultValue"];
				switch (queryParamater.DataType = Function.ToString(row["Type"]).ToLower())
				{
				case "datetime":
					queryParamater.Value = Function.ParseDateTime(obj) ?? DateTime.Today;
					break;
				case "int":
				case "bigint":
				case "tinyint":
				case "smallint":
					queryParamater.Value = Function.ParseInt(obj);
					break;
				case "numeric":
				case "numeric(19,6)":
					queryParamater.Value = Function.ParseDecimal(obj);
					break;
				default:
					queryParamater.Value = Function.ToString(obj);
					break;
				}
				list.Add(queryParamater);
			}
			return HttpClient.Commons.GetExecuteStoreResult(storename, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}
}
