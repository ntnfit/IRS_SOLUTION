using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraNavBar;
using DevExpress.XtraNavBar.ViewInfo;

namespace Apzon.Forms.Dashboard;

public class frm_Dashboard : XtraFormEx
{
	private const string Controller = "Dashboard";

	private int _objectFrom;

	private readonly Enums.FormAuthorities _authorities;

	private IContainer components;

	private SimpleButtonEx btnCancel;

	private LabelLine lbToDate;

	private LabelLine lbFromDate;

	private DateEditEx txtFromDate;

	private DateEditEx txtToDate;

	private Panel panel1;

	private Panel panel2;

	private NavBarControl navBarControl;

	private NavBarItem navBarItem1;

	private NavBarItem navBarItem2;

	private NavBarItem navBarItem3;

	private NavBarItem navBarItem4;

	private NavBarItem navBarItem5;

	private SplitContainerControl splitContainerControl1;

	private GridSplitContainer gridSplitContainer1;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gcController;

	private RepositoryItemButtonEdit rControllerCode;

	private GridColumnEx gcControllerDes;

	private GridColumnEx gcRemark;

	private GridViewEx gridView3;

	private GridSplitContainer gridSplitContainer2;

	private GridControlEx grdLine;

	private GridViewEx gvLine;

	private ContextMenuStrip contextMenuStrip;

	private ToolStripMenuItem viewDetailToolStripMenuItem;

	private DropDownButton btnCopyTo;

	public frm_Dashboard()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Dashboard"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Dashboard" && x.authoriza == "F"))
			{
				_authorities = Enums.FormAuthorities.Read;
			}
			else
			{
				_authorities = Enums.FormAuthorities.Full;
			}
			FormMode = Enums.FormViewModeType.View;
			return;
		}
		throw new Exception("Not access form!");
	}

	private async void frm_Dashboard_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			txtFromDate.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			txtFromDate.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			txtFromDate.Properties.EditFormat.FormatType = FormatType.DateTime;
			txtFromDate.Properties.EditFormat.FormatString = GlobalData.DateTimeFormat;
			txtToDate.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			txtToDate.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
			txtToDate.Properties.EditFormat.FormatType = FormatType.DateTime;
			txtToDate.Properties.EditFormat.FormatString = GlobalData.DateTimeFormat;
			txtFromDate.EditValue = new DateTime(DateTime.Now.Year, 1, 1);
			txtToDate.EditValue = DateTime.Now;
			await CreateTabCategories();
			await LoadDataController();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvLine_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void navBarControl_GroupExpanded(object sender, NavBarGroupEventArgs e)
	{
		try
		{
			NavBarGroup activeGroup = navBarControl.ActiveGroup;
			if (activeGroup.ItemLinks != null && activeGroup.ItemLinks.Count > 0)
			{
				navBarControl.SelectedLink = activeGroup.ItemLinks.First();
				await LoadDataController();
				return;
			}
			grdDoc.DataSource = null;
			grdLine.DataSource = null;
			gvLine.Columns.Clear();
			grdDoc.RefreshDataSource();
			grdLine.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void menu_Click(object sender, NavBarLinkEventArgs e)
	{
		try
		{
			navBarControl.SelectedLink = ((NavBarItem)sender).Links.First();
			await LoadDataController();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void navBarControl_CustomDrawLink(object sender, CustomDrawNavBarElementEventArgs e)
	{
		try
		{
			if (e.ObjectInfo is NavLinkInfoArgs { State: ObjectState.Selected } navLinkInfoArgs)
			{
				LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 10), new Point(237, 10), Color.NavajoWhite, Color.LightYellow);
				e.Graphics.FillRectangle(brush, e.ObjectInfo.Bounds);
				e.Appearance.DrawString(e.Cache, navLinkInfoArgs.Link.Caption, navLinkInfoArgs.CaptionRectangle);
				e.Handled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void navBarControl_CustomDrawGroupCaption(object sender, CustomDrawNavBarElementEventArgs e)
	{
		try
		{
			NavGroupInfoArgs navGroupInfoArgs = e.ObjectInfo as NavGroupInfoArgs;
			if (navGroupInfoArgs != null && navGroupInfoArgs.State == ObjectState.Selected)
			{
				LinearGradientBrush brush = new LinearGradientBrush(new Point(0, 10), new Point(237, 10), Color.FromArgb(75, 185, 190), Color.LightYellow);
				e.Graphics.FillRectangle(brush, e.ObjectInfo.Bounds);
				e.Appearance.DrawString(e.Cache, navGroupInfoArgs.Group.Caption, navGroupInfoArgs.CaptionBounds);
				e.Handled = true;
			}
			else if (navGroupInfoArgs != null && navGroupInfoArgs.State == ObjectState.Hot)
			{
				LinearGradientBrush brush2 = new LinearGradientBrush(new Point(0, 10), new Point(237, 10), Color.LightGreen, Color.LightYellow);
				e.Graphics.FillRectangle(brush2, e.ObjectInfo.Bounds);
				e.Appearance.DrawString(e.Cache, navGroupInfoArgs.Group.Caption, navGroupInfoArgs.CaptionBounds);
				e.Handled = true;
			}
			else if (navGroupInfoArgs != null)
			{
				LinearGradientBrush brush3 = new LinearGradientBrush(new Point(0, 10), new Point(237, 10), Color.FromArgb(100, 200, 255), Color.LightYellow);
				e.Graphics.FillRectangle(brush3, e.ObjectInfo.Bounds);
				e.Appearance.DrawString(e.Cache, navGroupInfoArgs.Group.Caption, navGroupInfoArgs.CaptionBounds);
				e.Handled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void navBarControl_CustomDrawGroupClientBackground(object sender, DevExpress.XtraNavBar.ViewInfo.CustomDrawObjectEventArgs e)
	{
		try
		{
			Rectangle realBounds = e.RealBounds;
			int num = realBounds.Height;
			Rectangle rect = new Rectangle(realBounds.Left, realBounds.Top, realBounds.Width, num);
			Color lemonChiffon = Color.LemonChiffon;
			Color lightBlue = Color.LightBlue;
			LinearGradientBrush brush = new LinearGradientBrush(rect, lemonChiffon, lightBlue, LinearGradientMode.Vertical);
			e.Graphics.FillRectangle(brush, rect);
			e.Handled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void gvDoc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			if (gvDoc.FocusedRowHandle >= 0)
			{
				await LoadDataLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async void viewDetailToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			await LoadDataChildController();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private async void gvLine_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			GridView gridView = (GridView)sender;
			Point pt = gridView.GridControl.PointToClient(Control.MousePosition);
			GridHitInfo gridHitInfo = gridView.CalcHitInfo(pt);
			if (gridHitInfo != null && (gridHitInfo.InRow || gridHitInfo.InRowCell))
			{
				await LoadDataChildController();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	public async Task MODE_FIND()
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			txtFromDate.EditValue = new DateTime(DateTime.Now.Year, 1, 1);
			txtToDate.EditValue = DateTime.Now;
			await CreateTabCategories();
			await LoadDataController();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private async Task<DataSet> LoadDataCategories()
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Dashboard");
		return await http.GetAsync("GetDataCategories", new
		{
			usercode = GlobalData.UserCodeLogin
		});
	}

	private async Task CreateTabCategories()
	{
		try
		{
			DataSet dataSet = await LoadDataCategories();
			if (dataSet == null || dataSet.Tables.Count < 3 || dataSet.Tables[0].Rows.Count == 0 || dataSet.Tables[1].Rows.Count == 0 || dataSet.Tables[2].Rows.Count == 0)
			{
				grdDoc.DataSource = null;
				grdLine.DataSource = null;
				gvLine.Columns.Clear();
				navBarControl.Visible = false;
				splitContainerControl1.Visible = false;
				WinCommons.Warning(Enums.MessageCode.DataIsEmpty, "");
				return;
			}
			navBarControl.Visible = true;
			splitContainerControl1.Visible = true;
			navBarControl.Groups.Clear();
			navBarControl.Items.Clear();
			grdDoc.DataSource = null;
			grdLine.DataSource = null;
			gvLine.Columns.Clear();
			navBarControl.BeginUpdate();
			for (int i = 0; i < dataSet.Tables[0].Rows.Count; i++)
			{
				NavBarGroup navBarGroup = new NavBarGroup
				{
					Caption = Function.ToString(dataSet.Tables[0].Rows[i]["Name"]),
					Name = "navBarGroup" + Function.ParseInt(dataSet.Tables[0].Rows[i]["CatId"]),
					Expanded = true,
					SelectedLinkIndex = i + 2
				};
				navBarControl.Groups.Add(navBarGroup);
				DataRow[] array = dataSet.Tables[1].Select("CatId = " + dataSet.Tables[0].Rows[i]["CatId"]);
				if (array.Length != 0)
				{
					for (int j = 0; j < array.Length; j++)
					{
						if (dataSet.Tables[2].Select("tabcode = " + array[j]["TabId"]).Length != 0)
						{
							NavBarItem navBarItem = new NavBarItem
							{
								Caption = Function.ToString(array[j]["Name"]),
								Name = "navBarItem" + Function.ParseInt(dataSet.Tables[0].Rows[i]["CatId"]) + Function.ParseInt(array[j]["TabId"])
							};
							navBarItem.LinkClicked += menu_Click;
							navBarControl.Items.Add(navBarItem);
							navBarGroup.ItemLinks.Add(navBarItem);
							if (navBarControl.Groups.Count == 1 && navBarGroup.ItemLinks.Count == 1)
							{
								_objectFrom = Function.ParseInt(array[j]["ObjType"]);
							}
						}
					}
				}
				if (navBarGroup.ItemLinks.Count == 0)
				{
					navBarControl.Groups.RemoveAt(navBarControl.Groups.Count - 1);
				}
				else if (navBarControl.Groups.Count == 1)
				{
					navBarControl.ActiveGroup = navBarGroup;
					navBarControl.SelectedLink = navBarGroup.ItemLinks.First();
				}
			}
			navBarControl.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddMenuButton(int obj, ref DXPopupMenu menu)
	{
		try
		{
			DXMenuItem dXMenuItem = new DXMenuItem();
			switch (obj)
			{
			case 23:
				dXMenuItem.Caption = MenuCaption.SalesQuotation;
				break;
			case 17:
				dXMenuItem.Caption = MenuCaption.SalesOrder;
				break;
			case 15:
				dXMenuItem.Caption = MenuCaption.Delivery;
				break;
			case 16:
				dXMenuItem.Caption = MenuCaption.Return;
				break;
			case 13:
				dXMenuItem.Caption = ((_objectFrom == 15) ? MenuCaption.AR_ReserveInvoices : MenuCaption.ARInvoices);
				break;
			case 14:
				dXMenuItem.Caption = MenuCaption.ARCreditMemos;
				break;
			case 540000006:
				dXMenuItem.Caption = MenuCaption.PurchaseQuotation;
				break;
			case 1470000113:
				dXMenuItem.Caption = MenuCaption.PurchaseRequest;
				break;
			case 22:
				dXMenuItem.Caption = MenuCaption.PurchaseOder;
				break;
			case 20:
				dXMenuItem.Caption = MenuCaption.GoodReceiptPO;
				break;
			case 21:
				dXMenuItem.Caption = MenuCaption.GoodsReturn;
				break;
			case 18:
			{
				dXMenuItem.Caption = MenuCaption.APReserveInvoice;
				DXMenuItem dXMenuItem2 = new DXMenuItem(MenuCaption.APInvoices);
				dXMenuItem2.Click += (object s, EventArgs e) =>
				{
					try
					{
						WinFuntions.CopyToDocument((ApzObjectTypes)_objectFrom, (ApzObjectTypes)obj, this, gvLine);
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex2.Message);
					}
				};
				menu.Items.Add(dXMenuItem2);
				break;
			}
			case 19:
				dXMenuItem.Caption = MenuCaption.APCreditMemos;
				break;
			}
			dXMenuItem.Click += (object s, EventArgs e) =>
			{
				try
				{
					WinFuntions.CopyToDocument((ApzObjectTypes)_objectFrom, (ApzObjectTypes)obj, this, gvLine);
				}
				catch (Exception ex2)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
					WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex2.Message);
				}
			};
			menu.Items.Add(dXMenuItem);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataController()
	{
		_ = 2;
		try
		{
			if (navBarControl.ActiveGroup == null || navBarControl.SelectedLink == null)
			{
				return;
			}
			string name = navBarControl.ActiveGroup.Name;
			string itemName = navBarControl.SelectedLink.ItemName;
			int tabId = Function.ParseInt(itemName.Substring(10 + Function.ParseInt(name.Substring(11)).ToString().Length));
			DataTable dataTable = await GetDataObj(tabId);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				_objectFrom = Function.ParseInt(dataTable.Rows[0]["ObjType"]);
				string text = Function.ToString(dataTable.Rows[0]["TargetType"]);
				if (!string.IsNullOrEmpty(text))
				{
					btnCopyTo.Visible = true;
					DXPopupMenu menu = new DXPopupMenu();
					string[] array = text.Split(',');
					for (int i = 0; i < array.Length; i++)
					{
						AddMenuButton(Function.ParseInt(array[i]), ref menu);
					}
					btnCopyTo.DropDownControl = menu;
					if (menu.Items.Count == 0)
					{
						btnCopyTo.Visible = false;
					}
				}
				else
				{
					btnCopyTo.Visible = false;
				}
			}
			else
			{
				btnCopyTo.Visible = false;
			}
			grdDoc.DataSource = null;
			grdLine.DataSource = null;
			gvLine.Columns.Clear();
			grdDoc.RefreshDataSource();
			grdLine.RefreshDataSource();
			int oldDocus = gvDoc.FocusedRowHandle;
			GridControlEx gridControlEx = grdDoc;
			gridControlEx.DataSource = await GetDataController(tabId);
			grdDoc.RefreshDataSource();
			grdDoc.DataSourceTable.AcceptChanges();
			if (gvDoc.FocusedRowHandle == oldDocus)
			{
				await LoadDataLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetDataController(int tabId)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Dashboard");
		return await http.GetAsync("GetDataController", new { tabId });
	}

	private void CreateColumGrid(DataTable dt)
	{
		try
		{
			gvLine.Columns.Clear();
			if (dt == null)
			{
				return;
			}
			for (int i = 0; i < dt.Columns.Count; i++)
			{
				GridColumn gridColumn = gvLine.Columns.Add();
				gridColumn.Caption = (dt.Columns[i].ColumnName.StartsWith("U_") ? dt.Columns[i].ColumnName.Substring(2) : dt.Columns[i].ColumnName);
				gridColumn.FieldName = dt.Columns[i].ColumnName;
				gridColumn.Name = dt.Columns[i].ColumnName + i;
				gridColumn.VisibleIndex = i;
				gridColumn.Visible = true;
				gridColumn.OptionsColumn.AllowSort = DefaultBoolean.False;
				gridColumn.OptionsColumn.AllowMove = false;
				gridColumn.OptionsColumn.AllowSize = true;
				gridColumn.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
				if (dt.Columns[i].DataType == typeof(DateTime))
				{
					gridColumn.DisplayFormat.FormatType = FormatType.DateTime;
					gridColumn.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
				}
				else if (dt.Columns[i].DataType == typeof(double) || dt.Columns[i].DataType == typeof(float) || dt.Columns[i].DataType == typeof(decimal))
				{
					gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
					gridColumn.DisplayFormat.FormatString = ((GlobalData.QuantitiesFormat == null) ? "" : ("n" + GlobalData.QuantitiesFormat.NumberDecimalDigits));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetDataQuery(string sql, string type, DataTable dtPara)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Dashboard");
		return await http.PostAsync("GetDataQuery", new Tuple<string, string, DataTable>(sql, type, dtPara));
	}

	private async Task LoadDataLine()
	{
		grdLine.DataSource = null;
		gvLine.Columns.Clear();
		grdLine.RefreshDataSource();
		DataTable dataTable = new DataTable();
		string sql = Function.ToString(gvDoc.GetFocusedRowCellValue("QueryText"));
		string type = (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue("QueryType"))) ? "T" : Function.ToString(gvDoc.GetFocusedRowCellValue("QueryType")));
		dataTable.Columns.Add("FromDate", typeof(DateTime));
		dataTable.Columns.Add("ToDate", typeof(DateTime));
		DataRow dataRow = dataTable.NewRow();
		dataRow["FromDate"] = txtFromDate.EditValue;
		dataRow["ToDate"] = txtToDate.EditValue;
		dataTable.Rows.Add(dataRow);
		dataTable.AcceptChanges();
		DataTable dataTable2 = await GetDataQuery(sql, type, dataTable);
		CreateColumGrid(dataTable2);
		grdLine.DataSource = dataTable2;
		grdLine.RefreshDataSource();
		grdLine.DataSourceTable.AcceptChanges();
	}

	private async Task LoadDataChildController()
	{
		try
		{
			int fatherId = Function.ParseInt(gvDoc.GetFocusedRowCellValue("ControllerId"));
			DataTable dataSourceTable = grdLine.DataSourceTable;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("FromDate", typeof(DateTime));
			dataTable.Columns.Add("ToDate", typeof(DateTime));
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("ColumnKey"));
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = text.Split(',');
				for (int i = 0; i < array.Length; i++)
				{
					dataTable.Columns.Add(new DataColumn(array[i], (!dataSourceTable.Columns.Contains(array[i])) ? typeof(string) : dataSourceTable.Columns[array[i]].DataType));
				}
				DataRow dataRow = dataTable.NewRow();
				dataRow["FromDate"] = txtFromDate.EditValue;
				dataRow["ToDate"] = txtToDate.EditValue;
				for (int j = 0; j < array.Length; j++)
				{
					dataRow[array[j]] = (dataSourceTable.Columns.Contains(array[j]) ? dataSourceTable.Rows[gvLine.FocusedRowHandle][array[j]] : "");
				}
				dataTable.Rows.Add(dataRow);
			}
			DataSet dataSet = await GetDataChidController(fatherId, dataTable);
			if (dataSet != null && dataSet.Tables.Count == 2 && dataSet.Tables[0].Rows.Count > 0)
			{
				frm_DataSubController frm_DataSubController2 = new frm_DataSubController(dataSet, Function.ParseDateTimes(txtFromDate.EditValue), Function.ParseDateTimes(txtToDate.EditValue));
				frm_DataSubController2.MdiParent = MdiParent;
				frm_DataSubController2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task<DataSet> GetDataChidController(int fatherId, DataTable dtPara)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Dashboard");
		return await http.PostAsync("GetDataChidController", new Tuple<int, DataTable>(fatherId, dtPara));
	}

	private async Task<DataTable> GetDataObj(int tabId)
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Dashboard");
		return await http.GetAsync("GetDataObj", new { tabId });
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Dashboard.frm_Dashboard));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbToDate = new Apzon.Controls.Controls.LabelLine();
		this.lbFromDate = new Apzon.Controls.Controls.LabelLine();
		this.txtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.panel1 = new System.Windows.Forms.Panel();
		this.btnCopyTo = new DevExpress.XtraEditors.DropDownButton();
		this.panel2 = new System.Windows.Forms.Panel();
		this.navBarControl = new DevExpress.XtraNavBar.NavBarControl();
		this.navBarItem1 = new DevExpress.XtraNavBar.NavBarItem();
		this.navBarItem2 = new DevExpress.XtraNavBar.NavBarItem();
		this.navBarItem3 = new DevExpress.XtraNavBar.NavBarItem();
		this.navBarItem4 = new DevExpress.XtraNavBar.NavBarItem();
		this.navBarItem5 = new DevExpress.XtraNavBar.NavBarItem();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.gridSplitContainer1 = new DevExpress.XtraGrid.GridSplitContainer();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcController = new Apzon.Controls.Grid.GridColumnEx();
		this.rControllerCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcControllerDes = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.gridView3 = new Apzon.Controls.Grid.GridViewEx();
		this.gridSplitContainer2 = new DevExpress.XtraGrid.GridSplitContainer();
		this.grdLine = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.viewDetailToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvLine = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).BeginInit();
		this.panel1.SuspendLayout();
		this.panel2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.navBarControl).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer1).BeginInit();
		this.gridSplitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rControllerCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer2).BeginInit();
		this.gridSplitContainer2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdLine).BeginInit();
		this.contextMenuStrip.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvLine).BeginInit();
		base.SuspendLayout();
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Close";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(692, 4);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(70, 20);
		this.btnCancel.TabIndex = 10;
		this.btnCancel.Text = "Close";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.lbToDate.EditControl = null;
		this.lbToDate.EditValue = "To Date";
		this.lbToDate.FieldName = null;
		this.lbToDate.HideWhenEmpty = false;
		this.lbToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbToDate.Location = new System.Drawing.Point(267, 8);
		this.lbToDate.Name = "lbToDate";
		this.lbToDate.ReadOnly = false;
		this.lbToDate.SettingInVisible = false;
		this.lbToDate.SettingReadonly = false;
		this.lbToDate.ShowUnderLine = true;
		this.lbToDate.Size = new System.Drawing.Size(80, 20);
		this.lbToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbToDate.TabIndex = 0;
		this.lbToDate.Utext = "To Date";
		this.lbFromDate.EditControl = null;
		this.lbFromDate.EditValue = "From Date";
		this.lbFromDate.FieldName = null;
		this.lbFromDate.HideWhenEmpty = false;
		this.lbFromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFromDate.Location = new System.Drawing.Point(10, 8);
		this.lbFromDate.Name = "lbFromDate";
		this.lbFromDate.ReadOnly = false;
		this.lbFromDate.SettingInVisible = false;
		this.lbFromDate.SettingReadonly = false;
		this.lbFromDate.ShowUnderLine = true;
		this.lbFromDate.Size = new System.Drawing.Size(80, 20);
		this.lbFromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFromDate.TabIndex = 0;
		this.lbFromDate.Utext = "From Date";
		this.txtFromDate.EditControl = null;
		this.txtFromDate.EditValue = null;
		this.txtFromDate.FieldName = null;
		this.txtFromDate.Location = new System.Drawing.Point(90, 6);
		this.txtFromDate.Name = "txtFromDate";
		this.txtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtFromDate.Properties.IsSearchControl = false;
		this.txtFromDate.Properties.Mask.EditMask = "";
		this.txtFromDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txtFromDate.SettingInVisible = false;
		this.txtFromDate.SettingReadonly = false;
		this.txtFromDate.Size = new System.Drawing.Size(160, 20);
		this.txtFromDate.TabIndex = 1;
		this.txtToDate.EditControl = null;
		this.txtToDate.EditValue = null;
		this.txtToDate.FieldName = null;
		this.txtToDate.Location = new System.Drawing.Point(347, 6);
		this.txtToDate.Name = "txtToDate";
		this.txtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtToDate.Properties.IsSearchControl = false;
		this.txtToDate.Properties.Mask.EditMask = "";
		this.txtToDate.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.txtToDate.SettingInVisible = false;
		this.txtToDate.SettingReadonly = false;
		this.txtToDate.Size = new System.Drawing.Size(160, 20);
		this.txtToDate.TabIndex = 2;
		this.panel1.Controls.Add(this.btnCopyTo);
		this.panel1.Controls.Add(this.btnCancel);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(0, 434);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(767, 28);
		this.panel1.TabIndex = 15;
		this.btnCopyTo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCopyTo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCopyTo.ImageOptions.Image");
		this.btnCopyTo.Location = new System.Drawing.Point(588, 4);
		this.btnCopyTo.Name = "btnCopyTo";
		this.btnCopyTo.Size = new System.Drawing.Size(98, 20);
		this.btnCopyTo.TabIndex = 11;
		this.btnCopyTo.Text = "Copy To";
		this.panel2.Controls.Add(this.txtToDate);
		this.panel2.Controls.Add(this.txtFromDate);
		this.panel2.Controls.Add(this.lbFromDate);
		this.panel2.Controls.Add(this.lbToDate);
		this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
		this.panel2.Location = new System.Drawing.Point(0, 0);
		this.panel2.Name = "panel2";
		this.panel2.Size = new System.Drawing.Size(767, 30);
		this.panel2.TabIndex = 17;
		this.navBarControl.Appearance.ButtonPressed.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.navBarControl.Appearance.ButtonPressed.Options.UseBackColor = true;
		this.navBarControl.Appearance.GroupHeader.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.navBarControl.Appearance.GroupHeader.Options.UseFont = true;
		this.navBarControl.Appearance.GroupHeaderActive.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.navBarControl.Appearance.GroupHeaderActive.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.navBarControl.Appearance.GroupHeaderActive.Options.UseBackColor = true;
		this.navBarControl.Appearance.GroupHeaderActive.Options.UseFont = true;
		this.navBarControl.Appearance.GroupHeaderHotTracked.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.navBarControl.Appearance.GroupHeaderHotTracked.Options.UseFont = true;
		this.navBarControl.Appearance.GroupHeaderPressed.BackColor = System.Drawing.Color.PaleGoldenrod;
		this.navBarControl.Appearance.GroupHeaderPressed.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold);
		this.navBarControl.Appearance.GroupHeaderPressed.Options.UseBackColor = true;
		this.navBarControl.Appearance.GroupHeaderPressed.Options.UseFont = true;
		this.navBarControl.Appearance.Item.BackColor = System.Drawing.Color.FromArgb(128, 255, 128);
		this.navBarControl.Appearance.Item.BackColor2 = System.Drawing.Color.Green;
		this.navBarControl.Appearance.Item.Options.UseBackColor = true;
		this.navBarControl.Appearance.ItemActive.BackColor = System.Drawing.Color.Khaki;
		this.navBarControl.Appearance.ItemActive.Options.UseBackColor = true;
		this.navBarControl.Appearance.ItemHotTracked.BackColor = System.Drawing.Color.FromArgb(255, 128, 0);
		this.navBarControl.Appearance.ItemHotTracked.BackColor2 = System.Drawing.Color.FromArgb(255, 128, 0);
		this.navBarControl.Appearance.ItemHotTracked.BorderColor = System.Drawing.Color.FromArgb(128, 64, 0);
		this.navBarControl.Appearance.ItemHotTracked.Options.UseBackColor = true;
		this.navBarControl.Appearance.ItemHotTracked.Options.UseBorderColor = true;
		this.navBarControl.Appearance.ItemHotTracked.Options.UseFont = true;
		this.navBarControl.Appearance.ItemHotTracked.Options.UseForeColor = true;
		this.navBarControl.Appearance.ItemHotTracked.Options.UseImage = true;
		this.navBarControl.Appearance.ItemPressed.BackColor = System.Drawing.Color.DeepSkyBlue;
		this.navBarControl.Appearance.ItemPressed.Options.UseBackColor = true;
		this.navBarControl.Appearance.NavPaneContentButtonHotTracked.BackColor = System.Drawing.Color.Yellow;
		this.navBarControl.Appearance.NavPaneContentButtonHotTracked.BackColor2 = System.Drawing.Color.FromArgb(192, 192, 0);
		this.navBarControl.Appearance.NavPaneContentButtonHotTracked.Options.UseBackColor = true;
		this.navBarControl.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.navBarControl.Dock = System.Windows.Forms.DockStyle.Left;
		this.navBarControl.Font = new System.Drawing.Font("Arial", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
		this.navBarControl.Items.AddRange(new DevExpress.XtraNavBar.NavBarItem[5] { this.navBarItem1, this.navBarItem2, this.navBarItem3, this.navBarItem4, this.navBarItem5 });
		this.navBarControl.LinkSelectionMode = DevExpress.XtraNavBar.LinkSelectionModeType.OneInGroupAndAllowAutoSelect;
		this.navBarControl.Location = new System.Drawing.Point(0, 30);
		this.navBarControl.LookAndFeel.SkinName = "Visual Studio 2013 Blue";
		this.navBarControl.LookAndFeel.Style = DevExpress.LookAndFeel.LookAndFeelStyle.Office2003;
		this.navBarControl.LookAndFeel.UseWindowsXPTheme = true;
		this.navBarControl.Name = "navBarControl";
		this.navBarControl.OptionsNavPane.ExpandedWidth = 172;
		this.navBarControl.PaintStyleKind = DevExpress.XtraNavBar.NavBarViewKind.NavigationPane;
		this.navBarControl.Size = new System.Drawing.Size(172, 404);
		this.navBarControl.StoreDefaultPaintStyleName = true;
		this.navBarControl.TabIndex = 18;
		this.navBarControl.Text = "navBarControl";
		this.navBarControl.GroupExpanded += new DevExpress.XtraNavBar.NavBarGroupEventHandler(this.navBarControl_GroupExpanded);
		this.navBarControl.CustomDrawGroupCaption += new DevExpress.XtraNavBar.ViewInfo.CustomDrawNavBarElementEventHandler(this.navBarControl_CustomDrawGroupCaption);
		this.navBarControl.CustomDrawLink += new DevExpress.XtraNavBar.ViewInfo.CustomDrawNavBarElementEventHandler(this.navBarControl_CustomDrawLink);
		this.navBarControl.CustomDrawGroupClientBackground += new DevExpress.XtraNavBar.ViewInfo.CustomDrawObjectEventHandler(this.navBarControl_CustomDrawGroupClientBackground);
		this.navBarItem1.Caption = "navBarItem1";
		this.navBarItem1.Name = "navBarItem1";
		this.navBarItem2.Caption = "navBarItem2";
		this.navBarItem2.Name = "navBarItem2";
		this.navBarItem3.Caption = "navBarItem3";
		this.navBarItem3.Name = "navBarItem3";
		this.navBarItem4.Caption = "navBarItem4";
		this.navBarItem4.Name = "navBarItem4";
		this.navBarItem5.Caption = "navBarItem5";
		this.navBarItem5.Name = "navBarItem5";
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(172, 30);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.splitContainerControl1.Panel1.Controls.Add(this.gridSplitContainer1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Simple;
		this.splitContainerControl1.Panel2.Controls.Add(this.gridSplitContainer2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(595, 404);
		this.splitContainerControl1.SplitterPosition = 176;
		this.splitContainerControl1.TabIndex = 19;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.gridSplitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridSplitContainer1.Grid = this.grdDoc;
		this.gridSplitContainer1.Location = new System.Drawing.Point(0, 1);
		this.gridSplitContainer1.Name = "gridSplitContainer1";
		this.gridSplitContainer1.Panel1.Controls.Add(this.grdDoc);
		this.gridSplitContainer1.Size = new System.Drawing.Size(586, 173);
		this.gridSplitContainer1.TabIndex = 0;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rControllerCode });
		this.grdDoc.Size = new System.Drawing.Size(586, 173);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gvDoc, this.gridView3 });
		this.gvDoc.Appearance.FocusedCell.BackColor = System.Drawing.Color.White;
		this.gvDoc.Appearance.FocusedCell.Options.UseBackColor = true;
		this.gvDoc.Appearance.FocusedRow.BackColor = System.Drawing.Color.BlanchedAlmond;
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.Appearance.Preview.BackColor = System.Drawing.Color.Wheat;
		this.gvDoc.Appearance.Preview.Options.UseBackColor = true;
		this.gvDoc.Appearance.SelectedRow.BackColor = System.Drawing.Color.FromArgb(255, 224, 192);
		this.gvDoc.Appearance.SelectedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcController, this.gcControllerDes, this.gcRemark });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 30;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = null;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.ReadOnly = true;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvDoc_FocusedRowChanged);
		this.gcController.Caption = "Controller Code";
		this.gcController.ColumnEdit = this.rControllerCode;
		this.gcController.CurrencyDateField = null;
		this.gcController.CurrencyField = null;
		this.gcController.CurrencyFixed = null;
		this.gcController.CurrencyFixedRate = 0.0;
		this.gcController.CurrencyRateField = null;
		this.gcController.FieldName = "Code";
		this.gcController.Name = "gcController";
		this.gcController.PrimaryKey = true;
		this.gcController.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcController.UnitField = null;
		this.gcController.Visible = true;
		this.gcController.VisibleIndex = 0;
		this.gcController.Width = 98;
		this.rControllerCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rControllerCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rControllerCode.Name = "rControllerCode";
		this.gcControllerDes.Caption = "Controller Desc";
		this.gcControllerDes.CurrencyDateField = null;
		this.gcControllerDes.CurrencyField = null;
		this.gcControllerDes.CurrencyFixed = null;
		this.gcControllerDes.CurrencyFixedRate = 0.0;
		this.gcControllerDes.CurrencyRateField = null;
		this.gcControllerDes.FieldName = "Description";
		this.gcControllerDes.Name = "gcControllerDes";
		this.gcControllerDes.PrimaryKey = false;
		this.gcControllerDes.UnitField = null;
		this.gcControllerDes.Visible = true;
		this.gcControllerDes.VisibleIndex = 1;
		this.gcControllerDes.Width = 109;
		this.gcRemark.Caption = "Remarks";
		this.gcRemark.CurrencyDateField = null;
		this.gcRemark.CurrencyField = null;
		this.gcRemark.CurrencyFixed = null;
		this.gcRemark.CurrencyFixedRate = 0.0;
		this.gcRemark.CurrencyRateField = null;
		this.gcRemark.FieldName = "Remarks";
		this.gcRemark.Name = "gcRemark";
		this.gcRemark.PrimaryKey = false;
		this.gcRemark.UnitField = null;
		this.gcRemark.Visible = true;
		this.gcRemark.VisibleIndex = 2;
		this.gcRemark.Width = 270;
		this.gridView3.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView3.ColumnKey = null;
		this.gridView3.DisableFormSetting = false;
		this.gridView3.DisplayException = false;
		this.gridView3.EditColumn = null;
		this.gridView3.FieldCurrency = null;
		this.gridView3.FieldRate = null;
		this.gridView3.FormId = null;
		this.gridView3.GridControl = this.grdDoc;
		this.gridView3.IgnoreFilter = false;
		this.gridView3.IndexEditRow = -1;
		this.gridView3.IndicatorWidth = 35;
		this.gridView3.IsApzTable = false;
		this.gridView3.IsInsertNewRow = false;
		this.gridView3.IsLoadingData = false;
		this.gridView3.IsMappingContextAction = false;
		this.gridView3.ItemId = null;
		this.gridView3.LstColumnAllowBatch = null;
		this.gridView3.LstColumnNotAllowBatch = null;
		this.gridView3.LstColumnNotEditable = null;
		this.gridView3.LstColumNotSetting = null;
		this.gridView3.Name = "gridView3";
		this.gridView3.UseBatchMod = false;
		this.gridSplitContainer2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridSplitContainer2.Grid = this.grdLine;
		this.gridSplitContainer2.Location = new System.Drawing.Point(0, 1);
		this.gridSplitContainer2.Name = "gridSplitContainer2";
		this.gridSplitContainer2.Panel1.Controls.Add(this.grdLine);
		this.gridSplitContainer2.Size = new System.Drawing.Size(591, 219);
		this.gridSplitContainer2.TabIndex = 0;
		this.grdLine.AllowPasteExcel = true;
		this.grdLine.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdLine.ClipboardData = null;
		this.grdLine.ContextMenuStrip = this.contextMenuStrip;
		this.grdLine.Location = new System.Drawing.Point(0, 1);
		this.grdLine.MainView = this.gvLine;
		this.grdLine.Name = "grdLine";
		this.grdLine.Size = new System.Drawing.Size(591, 219);
		this.grdLine.TabIndex = 0;
		this.grdLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvLine });
		this.contextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.viewDetailToolStripMenuItem });
		this.contextMenuStrip.Name = "contextMenuStrip";
		this.contextMenuStrip.Size = new System.Drawing.Size(133, 26);
		this.viewDetailToolStripMenuItem.Name = "viewDetailToolStripMenuItem";
		this.viewDetailToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
		this.viewDetailToolStripMenuItem.Text = "View Detail";
		this.viewDetailToolStripMenuItem.Click += new System.EventHandler(this.viewDetailToolStripMenuItem_Click);
		this.gvLine.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvLine.ColumnKey = null;
		this.gvLine.DisableFormSetting = false;
		this.gvLine.DisplayException = false;
		this.gvLine.EditColumn = null;
		this.gvLine.FieldCurrency = null;
		this.gvLine.FieldRate = null;
		this.gvLine.FormId = null;
		this.gvLine.GridControl = this.grdLine;
		this.gvLine.IgnoreFilter = false;
		this.gvLine.IndexEditRow = -1;
		this.gvLine.IndicatorWidth = 40;
		this.gvLine.IsApzTable = false;
		this.gvLine.IsInsertNewRow = false;
		this.gvLine.IsLoadingData = false;
		this.gvLine.IsMappingContextAction = false;
		this.gvLine.ItemId = null;
		this.gvLine.LstColumnAllowBatch = null;
		this.gvLine.LstColumnNotAllowBatch = null;
		this.gvLine.LstColumnNotEditable = null;
		this.gvLine.LstColumNotSetting = null;
		this.gvLine.Name = "gvLine";
		this.gvLine.OptionsBehavior.ReadOnly = true;
		this.gvLine.OptionsView.ColumnAutoWidth = false;
		this.gvLine.OptionsView.ShowGroupPanel = false;
		this.gvLine.UseBatchMod = false;
		this.gvLine.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvLine_CustomDrawRowIndicator);
		this.gvLine.DoubleClick += new System.EventHandler(this.gvLine_DoubleClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(767, 462);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.navBarControl);
		base.Controls.Add(this.panel2);
		base.Controls.Add(this.panel1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Dashboard";
		this.Text = "Working Table";
		base.Load += new System.EventHandler(this.frm_Dashboard_Load);
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtToDate.Properties).EndInit();
		this.panel1.ResumeLayout(false);
		this.panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.navBarControl).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer1).EndInit();
		this.gridSplitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rControllerCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer2).EndInit();
		this.gridSplitContainer2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdLine).EndInit();
		this.contextMenuStrip.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvLine).EndInit();
		base.ResumeLayout(false);
	}
}
