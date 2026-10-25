using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.Queries;
using Apzon.Forms.Dashboard;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.DashboardCommon;
using DevExpress.DashboardCommon.ViewerData;
using DevExpress.DashboardWin;
using DevExpress.Images;
using DevExpress.XtraBars;

namespace Apzon.Forms.Widget;

public class UcDashboardViewer : System.Windows.Forms.UserControl
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int _dashboardId;

	private DataTable _dashboardSource;

	private DataTable _selectedSource;

	private string _selectedItemId;

	private bool _ignorePara;

	private IContainer components;

	private DashboardViewer dbv_Main;

	private ContextMenuStrip contextMenuStrip1;

	public string DashboardName { get; set; }

	public UcDashboardViewer()
	{
		InitializeComponent();
	}

	public UcDashboardViewer(int dashboardId, bool ignorePara = false)
	{
		InitializeComponent();
		_dashboardId = dashboardId;
		_ignorePara = ignorePara;
	}

	public async Task SetDashboardId(int dashboardId)
	{
		_ = 4;
		try
		{
			_dashboardId = dashboardId;
			if (_dashboardId == 0)
			{
				return;
			}
			Task<DataTable> t1 = _httpClient.DashboardDetail.GetById(_dashboardId);
			XDocument xDocument = await LoadDashboard(_dashboardId);
			if (xDocument == null)
			{
				return;
			}
			DevExpress.DashboardCommon.Dashboard dashboard = this.LoadDashboardFromXml(xDocument);
			DataTable dataTable = await t1;
			string text = Function.ToString(dataTable.Rows[0]["SourceType"]);
			if (!(text == "R"))
			{
				if (text == "Q")
				{
					List<QueryParamater> lstParamater = new List<QueryParamater>();
					string text2 = Function.ToString(dataTable.Rows[0]["SqlType"]);
					string input;
					if (!(text2 == "Q"))
					{
						if (!(text2 == "T"))
						{
							WinCommons.Warning(Enums.MessageCode.None, "Choose type of query");
							return;
						}
						input = Function.ToString(dataTable.Rows[0]["Query"]);
						input = Regex.Replace(input, "@CURRENTUSER", $"N'{GlobalData.UserName}'", RegexOptions.IgnoreCase);
						input = Regex.Replace(input, "\\$\\[USER\\]", $"N'{GlobalData.UserID}'", RegexOptions.IgnoreCase);
						input = Regex.Replace(input, "@CURRENTBRANCH", $"N'{GlobalData.DefaultBranch}'", RegexOptions.IgnoreCase);
						List<DatabaseParamater> list = Function.ParseSql(input);
						if (!_ignorePara && list != null && list.Count > 0)
						{
							frm_SetParamater frm_SetParamater2 = new frm_SetParamater
							{
								List = list,
								StartPosition = FormStartPosition.CenterScreen
							};
							if (frm_SetParamater2.ShowDialog(this) != DialogResult.OK)
							{
								return;
							}
							lstParamater = frm_SetParamater2.Paramater;
						}
						_ignorePara = false;
					}
					else
					{
						int queryId = Function.ParseInt(dataTable.Rows[0]["Query"]);
						input = await _httpClient.Queries.GetQueryText(queryId);
					}
					DataSet dataSet = _httpClient.Commons.ExecuteSql(input, lstParamater);
					if (dataSet.Tables.Count == 2 && dataSet.Tables[1].Columns.Count > 0)
					{
						_dashboardSource = dataSet.Tables[1];
						foreach (IDashboardDataSource dataSource in dashboard.DataSources)
						{
							if (dataSource is DashboardObjectDataSource)
							{
								((DashboardObjectDataSource)dataSource).DataSource = _dashboardSource.Copy();
							}
						}
					}
				}
				else
				{
					foreach (IDashboardDataSource dataSource2 in dashboard.DataSources)
					{
						if (dataSource2 is DashboardObjectDataSource && ((DashboardObjectDataSource)dataSource2).DataSource is DataTable dashboardSource)
						{
							_dashboardSource = dashboardSource;
						}
					}
				}
			}
			else
			{
				string code = Function.ToString(dataTable.Rows[0]["UdrCode"]);
				DataTable dataTable2 = await _httpClient.Udr.GetQueryByCode(code);
				if (dataTable2.IsNotNull())
				{
					DataTable dataTable3 = (_dashboardSource = DashboardHelper.GetDashboardSourceByReport(storename: Function.ToString(dataTable2.Rows[0]["StoreName"]), drPr: await _httpClient.DashboardDetail.GetParams(_dashboardId)));
					foreach (IDashboardDataSource dataSource3 in dashboard.DataSources)
					{
						if (dataSource3 is DashboardObjectDataSource)
						{
							((DashboardObjectDataSource)dataSource3).DataSource = dataTable3.Copy();
						}
					}
				}
			}
			dashboard.CustomParameters += Dashboard_CustomParameters;
			DashboardName = dashboard.Title.Text;
			dbv_Main.Dashboard = dashboard;
			dbv_Main.ReloadDataAsync();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Dashboard_CustomParameters(object sender, CustomParametersEventArgs e)
	{
	}

	public void SetDashboard(DevExpress.DashboardCommon.Dashboard dashboard, int dashboardId)
	{
		try
		{
			_dashboardId = dashboardId;
			DashboardName = dashboard.Title.Text;
			dashboard.Title.Text = string.Empty;
			dashboard.Title.Visible = false;
			dbv_Main.Dashboard = dashboard;
			dbv_Main.ReloadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void UcDashboardViewer_Load(object sender, EventArgs e)
	{
		try
		{
			await SetDashboardId(_dashboardId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<XDocument> LoadDashboard(int dasboardId)
	{
		try
		{
			if (dasboardId > 0)
			{
				DataTable dataTable = await _httpClient.CategoriesDABClient.GetXMLByDasId(dasboardId, GlobalData.UserID);
				if (dataTable.IsNotNull())
				{
					if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["Layout"])))
					{
						return XDocument.Parse(Function.ToString(dataTable.Rows[0]["Layout"]));
					}
					WinCommons.Error(Enums.MessageCode.CanNotLoadLayoutXml, "");
					return new XDocument();
				}
				WinCommons.Error(Enums.MessageCode.CanNotLoadLayoutXml, "");
				return new XDocument();
			}
			WinCommons.Error(Enums.MessageCode.CanNotLoadLayoutXml, "");
			return null;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CanNotLoadLayoutXml, "");
			return null;
		}
	}

	public void Clear()
	{
		try
		{
			dbv_Main.Dashboard = null;
			dbv_Main.Refresh();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void dbv_Main_dbv_Main_DashboardItemMouseDown(object sender, DashboardItemMouseActionEventArgs e)
	{
		try
		{
			_selectedItemId = $"{_dashboardId}-{e.DashboardItemName}";
			DashboardUnderlyingDataSet underlyingData = e.GetUnderlyingData();
			if (underlyingData != null)
			{
				DataTable dataTable = new DataTable();
				List<string> columnNames = underlyingData.GetColumnNames();
				if (columnNames != null && columnNames.Count > 0)
				{
					foreach (string item in columnNames)
					{
						dataTable.Columns.Add(item, typeof(string));
					}
				}
				DataRow dataRow = dataTable.NewRow();
				foreach (string axisName in e.Data.GetAxisNames())
				{
					AxisPoint axisPoint = e.GetAxisPoint(axisName);
					if (axisPoint == null)
					{
						continue;
					}
					foreach (DimensionDescriptor dimension in e.Data.GetDimensions(axisName))
					{
						DimensionValue dimensionValue = axisPoint.GetDimensionValue(dimension);
						if (dimensionValue != null)
						{
							dataRow[dimension.DataMember] = dimensionValue.Value;
						}
					}
				}
				MultiDimensionalData multiDimensionalData = e.Data;
				foreach (string axisName2 in e.Data.GetAxisNames())
				{
					AxisPoint axisPoint2 = e.GetAxisPoint(axisName2);
					if (axisPoint2 != null)
					{
						multiDimensionalData = multiDimensionalData.GetSlice(axisPoint2);
					}
				}
				foreach (MeasureDescriptor measure in e.Data.GetMeasures())
				{
					dataRow[measure.DataMember] = multiDimensionalData.GetValue(measure).Value;
				}
				dataTable.Rows.Add(dataRow);
				_selectedSource = dataTable;
			}
			else
			{
				_selectedSource = _dashboardSource;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			_selectedSource = new DataTable();
		}
	}

	private void dbv_Main_PopupMenuShowing(object sender, DashboardPopupMenuShowingEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(_selectedItemId))
			{
				return;
			}
			BarButtonItem barButtonItem = new BarButtonItem();
			barButtonItem.ItemClick += contextAction_ItemClick;
			barButtonItem.Caption = "Context Action";
			e.Menu.ItemLinks.Insert(0, barButtonItem);
			DataSet data = _httpClient.ContextAction.GetData(ApzObjectTypes.oDashboardDetail.ToString(), _selectedItemId);
			if (!data.IsNotNull())
			{
				return;
			}
			DataTable source = data.Tables[0];
			foreach (DataRow item2 in from t in source.AsEnumerable()
				where Function.ParseInt(t["FatherId"]) <= 0
				select t)
			{
				int absEntry = Function.ParseInt(item2["AbsEntry"]);
				string ctmName = "CTA_" + absEntry;
				DataRow[] array = (from t in source.AsEnumerable()
					where Function.ParseInt(t["FatherId"]) == absEntry
					select t).ToArray();
				if (array.Length != 0)
				{
					BarSubItem ctm = CreateBarSubItem(ctmName, item2, absEntry);
					e.Menu.ItemLinks.Add(ctm);
					AddDropDown(array, ref ctm);
				}
				else
				{
					BarButtonItem item = CreateBarButtonItem(ctmName, item2, absEntry);
					e.Menu.ItemLinks.Add(item);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddDropDown(DataRow[] dr, ref BarSubItem ctm)
	{
		foreach (DataRow dataRow in dr)
		{
			int absEntry = Function.ParseInt(dataRow["AbsEntry"]);
			string ctmName = "CTA_" + absEntry;
			if ((from t in dr.AsEnumerable()
				where Function.ParseInt(t["FatherId"]) == absEntry
				select t).ToArray().Length != 0)
			{
				BarSubItem ctm2 = CreateBarSubItem(ctmName, dataRow, absEntry);
				ctm.AddItem(ctm2);
				AddDropDown(dr, ref ctm2);
			}
			else
			{
				BarButtonItem item = CreateBarButtonItem(ctmName, dataRow, absEntry);
				ctm.AddItem(item);
			}
		}
	}

	private BarSubItem CreateBarSubItem(string ctmName, DataRow item, int absEntry)
	{
		BarSubItem barSubItem = new BarSubItem
		{
			Name = ctmName,
			Caption = Function.ToString(item["Caption"])
		};
		string text = Function.ToString(item["ActionType"]);
		if (!string.IsNullOrEmpty(text) && text != "N")
		{
			barSubItem.ItemClick += (object s, ItemClickEventArgs events) =>
			{
				try
				{
					new ContextActionHandle(absEntry, ApzObjectTypes.oDashboardDetail.ToString(), _selectedItemId, _selectedSource);
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
		}
		return barSubItem;
	}

	private BarButtonItem CreateBarButtonItem(string ctmName, DataRow item, int absEntry)
	{
		BarButtonItem barButtonItem = new BarButtonItem();
		barButtonItem.Name = ctmName;
		barButtonItem.Caption = Function.ToString(item["Caption"]);
		string text = Function.ToString(item["ActionType"]);
		if (!string.IsNullOrEmpty(text) && text != "N")
		{
			barButtonItem.ItemClick += (object s, ItemClickEventArgs events) =>
			{
				try
				{
					new ContextActionHandle(absEntry, ApzObjectTypes.oDashboardDetail.ToString(), _selectedItemId, _selectedSource);
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
				}
			};
		}
		return barButtonItem;
	}

	private void contextAction_ItemClick(object sender, ItemClickEventArgs e)
	{
		try
		{
			new frm_ContextAction(ApzObjectTypes.oDashboardDetail.ToString(), _selectedItemId, _selectedSource).ShowDialog();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void dbv_Main_DashboardItemDoubleClick(object sender, DashboardItemMouseActionEventArgs e)
	{
		frm_DashboardViewer frm_DashboardViewer2 = new frm_DashboardViewer(dbv_Main.Dashboard, _dashboardId);
		frm_DashboardViewer2.Text = DashboardName;
		frm_DashboardViewer2.MdiParent = Program.MainForm;
		frm_DashboardViewer2.Show();
	}

	private void dbv_Main_CustomizeDashboardTitle(object sender, CustomizeDashboardTitleEventArgs e)
	{
		try
		{
			DashboardToolbarItem item = new DashboardToolbarItem("Refresh Data", async (DashboardToolbarItemClickEventArgs args) =>
			{
				await SetDashboardId(_dashboardId);
			})
			{
				ButtonImage = ImageResourceCache.Default.GetImage("images/actions/refresh_16x16.png")
			};
			e.Items.Insert(0, item);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
		}
	}

	protected override void Dispose(bool disposing)
	{
		if (disposing && components != null)
		{
			components.Dispose();
		}
		base.Dispose(disposing);
	}

	private void InitializeComponent()
	{
		this.components = new System.ComponentModel.Container();
		this.dbv_Main = new DevExpress.DashboardWin.DashboardViewer(this.components);
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		((System.ComponentModel.ISupportInitialize)this.dbv_Main).BeginInit();
		base.SuspendLayout();
		this.dbv_Main.ContextMenuStrip = this.contextMenuStrip1;
		this.dbv_Main.Dock = System.Windows.Forms.DockStyle.Fill;
		this.dbv_Main.Location = new System.Drawing.Point(0, 0);
		this.dbv_Main.Name = "dbv_Main";
		this.dbv_Main.Size = new System.Drawing.Size(472, 472);
		this.dbv_Main.TabIndex = 0;
		this.dbv_Main.DashboardItemDoubleClick += new DevExpress.DashboardWin.DashboardItemMouseActionEventHandler(this.dbv_Main_DashboardItemDoubleClick);
		this.dbv_Main.DashboardItemMouseDown += new DevExpress.DashboardWin.DashboardItemMouseActionEventHandler(this.dbv_Main_dbv_Main_DashboardItemMouseDown);
		this.dbv_Main.CustomizeDashboardTitle += new DevExpress.DashboardWin.CustomizeDashboardTitleEventHandler(this.dbv_Main_CustomizeDashboardTitle);
		this.dbv_Main.PopupMenuShowing += new DevExpress.DashboardWin.DashboardPopupMenuShowingEventHandler(this.dbv_Main_PopupMenuShowing);
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.Controls.Add(this.dbv_Main);
		base.Name = "UcDashboardViewer";
		base.Size = new System.Drawing.Size(472, 472);
		base.Load += new System.EventHandler(this.UcDashboardViewer_Load);
		((System.ComponentModel.ISupportInitialize)this.dbv_Main).EndInit();
		base.ResumeLayout(false);
	}
}
