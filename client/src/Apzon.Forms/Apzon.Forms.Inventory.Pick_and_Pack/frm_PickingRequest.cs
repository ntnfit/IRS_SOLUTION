using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.Pick_and_Pack;

public class frm_PickingRequest : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable dtSearch;

	private IContainer components;

	private RepositoryItemCheckEdit rep_chk_Allocated;

	private SplitContainerControl splitContainerControl2;

	private PanelControl panelControl2;

	private SimpleButtonEx btn_Close;

	private SimpleButtonEx btn_Release;

	private PanelControl panelControl3;

	private TextEditEx txt_DocNo;

	private LabelLine labelLine6;

	private ButtonEditCFL txt_Whs;

	private DateEditEx date_ToDate;

	private LabelLine labelLine1;

	private DateEditEx date_FromDate;

	private LabelLine labelLine2;

	private ButtonEditCFL txt_BpCode;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private SimpleButtonEx btn_search;

	private GridControlEx grd_SalesOrder;

	private GridViewEx gv_SalesOrder;

	private RepositoryItemCheckEdit rCheckEdit;

	public frm_PickingRequest()
	{
		InitializeComponent();
	}

	public async Task REFRESH_DATA()
	{
		btn_search_Click(null, null);
	}

	private void frm_frm_PickingRequest_Load(object sender, EventArgs e)
	{
		try
		{
			date_FromDate.EditValue = DateTime.Today;
			date_ToDate.EditValue = DateTime.Today;
			gv_SalesOrder.OptionsSelection.MultiSelect = true;
			gv_SalesOrder.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
			btn_search_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			int num = Function.ParseInt(txt_DocNo.Text);
			DateTime dateTime = (string.IsNullOrEmpty(date_FromDate.Text) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(date_FromDate.EditValue));
			DateTime dateTime2 = (string.IsNullOrEmpty(date_ToDate.Text) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(date_ToDate.EditValue));
			string value = Function.ToString(txt_BpCode.EditValue);
			string value2 = Function.ToString(txt_Whs.EditValue);
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dataRow["docentry"] = num;
			dataRow["whs"] = value2;
			dataRow["fromDate"] = dateTime;
			dataRow["toDate"] = dateTime2;
			dataRow["docStatus"] = "";
			dataRow["bpCode"] = value;
			dataRow["userName"] = GlobalData.UserName;
			dataRow["branch"] = GlobalData.DefaultBranch;
			dtSearch.Rows.Add(dataRow);
			new DataSet();
			DataSet ds = await _httpClient.Picking.LoadSalesOrder(dtSearch);
			if (ds.IsNotNull())
			{
				await GridViewHelper.GenerateGrid(grd_SalesOrder, gv_SalesOrder, ds);
				GridColumn gridColumn = gv_SalesOrder.Columns.FirstOrDefault((GridColumn t) => t.FieldName.Equals("Checked"));
				if (gridColumn != null && gridColumn.ColumnEdit == null)
				{
					gridColumn.ColumnEdit = rCheckEdit;
				}
			}
			ds.Tables.Clear();
			ds.Dispose();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Release_Click(object sender, EventArgs e)
	{
		try
		{
			if (!grd_SalesOrder.DataSourceTable.IsNotNull() || !gv_SalesOrder.IsExistColumn("Checked"))
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Datasource null or not exist column 'Checked'");
				return;
			}
			DataRow[] array = grd_SalesOrder.DataSourceTable.Select("Checked='Y'");
			if (array.Length != 0)
			{
				DataTable dataTable = array.CopyToDataTable();
				if ((from t in dataTable.AsEnumerable()
					group t by new
					{
						docentry = Function.ParseInt(t["OrderEntry"])
					} into g
					select new
					{
						Key = g.Key,
						Items = g
					}).Count() > 1)
				{
					WinCommons.Error(Enums.MessageCode.OnlySelect1Order2Release, "");
					return;
				}
				frm_PickingDocument frm_PickingDocument2 = new frm_PickingDocument(dataTable);
				frm_PickingDocument2.MdiParent = MdiParent;
				frm_PickingDocument2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.SelectRowToRelease, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void txt_BpCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			FindBpCodeByCFL();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void FindBpCodeByCFL()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Bp, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txt_BpCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["CardCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CreateDataSearch()
	{
		if (!dtSearch.IsNotNull() || dtSearch.Columns.Count == 0)
		{
			dtSearch = new DataTable();
			dtSearch.Columns.Add(new DataColumn("docentry", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("whs", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("fromDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("toDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("docStatus", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("bpCode", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("userName", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("branch", typeof(int)));
		}
		else
		{
			dtSearch.Rows.Clear();
		}
	}

	private void gv_SalesOrder_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!new string[2] { "Checked", "PickQtty" }.Contains(e.Column.FieldName))
			{
				return;
			}
			DataTable dataSourceTable = grd_SalesOrder.DataSourceTable;
			int focusedDataSourceRowIndex = gv_SalesOrder.GetFocusedDataSourceRowIndex();
			if (gv_SalesOrder.IsExistColumn("Checked") && gv_SalesOrder.IsExistColumn("PickQtty") && "PickQtty".Equals(e.Column.FieldName))
			{
				if (Function.ParseDouble(gv_SalesOrder.GetRowCellValue(e.RowHandle, "PickQtty")) > 0.0)
				{
					dataSourceTable.Rows[focusedDataSourceRowIndex]["Checked"] = "Y";
				}
				else
				{
					dataSourceTable.Rows[focusedDataSourceRowIndex]["Checked"] = "N";
				}
			}
			if (e.Column.FieldName.Equals("Checked"))
			{
				dataSourceTable.Rows[focusedDataSourceRowIndex]["PickQtty"] = (Function.ToString(e.Value).Equals("Y") ? gv_SalesOrder.GetFocusedRowCellValue("OpenQty") : ((object)0));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_SalesOrder_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu((GridViewEx)sender);
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = "Release All Selected Rows";
			dXMenuItem.Click += gv_SalesOrderMenu_Click;
			gridViewMenu.Items.Add(dXMenuItem);
			if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
				gridViewMenu.Show(e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_SalesOrderMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if (sender is DXMenuItem dXMenuItem && dXMenuItem.Caption.Equals("Release All Selected Rows") && gv_SalesOrder.GetSelectedRows() != null && gv_SalesOrder.GetSelectedRows().Length != 0 && gv_SalesOrder.IsExistColumn("PickQtty") && gv_SalesOrder.IsExistColumn("OpenQty"))
			{
				int[] selectedRows = gv_SalesOrder.GetSelectedRows();
				foreach (int rowHandle in selectedRows)
				{
					DataRow dataRow = gv_SalesOrder.GetDataRow(rowHandle);
					dataRow["PickQtty"] = dataRow["OpenQty"];
					dataRow["Checked"] = "Y";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Pick_and_Pack.frm_PickingRequest));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.rep_chk_Allocated = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Close = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Release = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.txt_DocNo = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Whs = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_BpCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.btn_search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_SalesOrder = new Apzon.Controls.Grid.GridControlEx();
		this.gv_SalesOrder = new Apzon.Controls.Grid.GridViewEx();
		this.rCheckEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Allocated).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Whs.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_SalesOrder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SalesOrder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCheckEdit).BeginInit();
		base.SuspendLayout();
		this.rep_chk_Allocated.AutoHeight = false;
		this.rep_chk_Allocated.DisplayValueChecked = "Y";
		this.rep_chk_Allocated.DisplayValueGrayed = "N";
		this.rep_chk_Allocated.DisplayValueUnchecked = "N";
		this.rep_chk_Allocated.Name = "rep_chk_Allocated";
		this.rep_chk_Allocated.ValueChecked = "Y";
		this.rep_chk_Allocated.ValueGrayed = "N";
		this.rep_chk_Allocated.ValueUnchecked = "N";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.panelControl2);
		this.splitContainerControl2.Panel2.Controls.Add(this.panelControl3);
		this.splitContainerControl2.Panel2.Controls.Add(this.grd_SalesOrder);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.PanelVisibility = DevExpress.XtraEditors.SplitPanelVisibility.Panel2;
		this.splitContainerControl2.Size = new System.Drawing.Size(1051, 520);
		this.splitContainerControl2.SplitterPosition = 0;
		this.splitContainerControl2.TabIndex = 13;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.panelControl2.Controls.Add(this.btn_Close);
		this.panelControl2.Controls.Add(this.btn_Release);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl2.Location = new System.Drawing.Point(0, 485);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(1051, 35);
		this.panelControl2.TabIndex = 2;
		this.btn_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Close.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Close.EditControl = null;
		this.btn_Close.EditValue = "Close";
		this.btn_Close.FieldName = null;
		this.btn_Close.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Close.ImageOptions.Image");
		this.btn_Close.IsSearchControl = false;
		this.btn_Close.Location = new System.Drawing.Point(83, 6);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.ReadOnly = false;
		this.btn_Close.SettingInVisible = false;
		this.btn_Close.SettingReadonly = false;
		this.btn_Close.Size = new System.Drawing.Size(75, 23);
		this.btn_Close.TabIndex = 7;
		this.btn_Close.Text = "Close";
		this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
		this.btn_Release.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Release.EditControl = null;
		this.btn_Release.EditValue = "Release ";
		this.btn_Release.FieldName = null;
		this.btn_Release.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Release.ImageOptions.Image");
		this.btn_Release.IsSearchControl = false;
		this.btn_Release.Location = new System.Drawing.Point(5, 6);
		this.btn_Release.Name = "btn_Release";
		this.btn_Release.ReadOnly = false;
		this.btn_Release.SettingInVisible = false;
		this.btn_Release.SettingReadonly = false;
		this.btn_Release.Size = new System.Drawing.Size(75, 23);
		this.btn_Release.TabIndex = 6;
		this.btn_Release.Text = "Release ";
		this.btn_Release.Click += new System.EventHandler(this.btn_Release_Click);
		this.panelControl3.Controls.Add(this.txt_DocNo);
		this.panelControl3.Controls.Add(this.labelLine6);
		this.panelControl3.Controls.Add(this.txt_Whs);
		this.panelControl3.Controls.Add(this.date_ToDate);
		this.panelControl3.Controls.Add(this.labelLine1);
		this.panelControl3.Controls.Add(this.date_FromDate);
		this.panelControl3.Controls.Add(this.labelLine2);
		this.panelControl3.Controls.Add(this.txt_BpCode);
		this.panelControl3.Controls.Add(this.labelLine4);
		this.panelControl3.Controls.Add(this.labelLine3);
		this.panelControl3.Controls.Add(this.btn_search);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl3.Location = new System.Drawing.Point(0, 0);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(1051, 39);
		this.panelControl3.TabIndex = 4;
		this.txt_DocNo.EditControl = null;
		this.txt_DocNo.FieldName = null;
		this.txt_DocNo.IsSearchControl = false;
		this.txt_DocNo.Location = new System.Drawing.Point(85, 7);
		this.txt_DocNo.Name = "txt_DocNo";
		this.txt_DocNo.SettingInVisible = false;
		this.txt_DocNo.SettingReadonly = false;
		this.txt_DocNo.Size = new System.Drawing.Size(102, 20);
		this.txt_DocNo.TabIndex = 0;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Doc No";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(25, 8);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(63, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 29;
		this.labelLine6.Utext = "Doc No";
		this.txt_Whs.EditControl = null;
		this.txt_Whs.FieldName = null;
		this.txt_Whs.IsSearchControl = false;
		this.txt_Whs.Location = new System.Drawing.Point(751, 6);
		this.txt_Whs.Name = "txt_Whs";
		this.txt_Whs.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.txt_Whs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_Whs.Properties.ContainerControl = null;
		this.txt_Whs.Properties.DisplayMember = "WhsCode";
		this.txt_Whs.Properties.DisplayMember2 = "";
		this.txt_Whs.Properties.LoadDataSourceWhenEnter = false;
		this.txt_Whs.Properties.SeparateCharacterForMulti = ';';
		this.txt_Whs.Properties.SeparateString = ", ";
		this.txt_Whs.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.GetListWareHouse;
		this.txt_Whs.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oWarehouses;
		this.txt_Whs.Properties.UsingAutoCompleteBox = false;
		this.txt_Whs.Properties.UsingDataSource = true;
		this.txt_Whs.Properties.ValidateWhenNoDataSource = false;
		this.txt_Whs.Properties.ValueMember = "WhsCode";
		this.txt_Whs.SettingInVisible = false;
		this.txt_Whs.SettingReadonly = false;
		this.txt_Whs.Size = new System.Drawing.Size(102, 20);
		this.txt_Whs.TabIndex = 4;
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
		this.date_ToDate.Location = new System.Drawing.Point(583, 6);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = false;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(102, 20);
		this.date_ToDate.TabIndex = 3;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(353, 8);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(63, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 21;
		this.labelLine1.Utext = "From Date";
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = null;
		this.date_FromDate.Location = new System.Drawing.Point(416, 6);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.IsSearchControl = false;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(102, 20);
		this.date_FromDate.TabIndex = 2;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(524, 8);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(63, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 22;
		this.labelLine2.Utext = "To Date";
		this.txt_BpCode.EditControl = null;
		this.txt_BpCode.FieldName = null;
		this.txt_BpCode.IsSearchControl = false;
		this.txt_BpCode.Location = new System.Drawing.Point(245, 7);
		this.txt_BpCode.Name = "txt_BpCode";
		this.txt_BpCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.txt_BpCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_BpCode.Properties.ContainerControl = null;
		this.txt_BpCode.Properties.DisplayMember = "CardCode";
		this.txt_BpCode.Properties.DisplayMember2 = "";
		this.txt_BpCode.Properties.LoadDataSourceWhenEnter = false;
		this.txt_BpCode.Properties.SeparateCharacterForMulti = ';';
		this.txt_BpCode.Properties.SeparateString = ", ";
		this.txt_BpCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.txt_BpCode.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.txt_BpCode.Properties.UsingAutoCompleteBox = false;
		this.txt_BpCode.Properties.UsingDataSource = true;
		this.txt_BpCode.Properties.ValidateWhenNoDataSource = false;
		this.txt_BpCode.Properties.ValueMember = "CardCode";
		this.txt_BpCode.SettingInVisible = false;
		this.txt_BpCode.SettingReadonly = false;
		this.txt_BpCode.Size = new System.Drawing.Size(102, 20);
		this.txt_BpCode.TabIndex = 1;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Bp Code";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(193, 9);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(63, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 23;
		this.labelLine4.Utext = "Bp Code";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Warehouse";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(691, 8);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(63, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 24;
		this.labelLine3.Utext = "Warehouse";
		this.btn_search.EditControl = null;
		this.btn_search.EditValue = "Search";
		this.btn_search.FieldName = null;
		this.btn_search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_search.ImageOptions.Image");
		this.btn_search.IsSearchControl = false;
		this.btn_search.Location = new System.Drawing.Point(890, 5);
		this.btn_search.Name = "btn_search";
		this.btn_search.ReadOnly = false;
		this.btn_search.SettingInVisible = false;
		this.btn_search.SettingReadonly = false;
		this.btn_search.Size = new System.Drawing.Size(75, 23);
		this.btn_search.TabIndex = 5;
		this.btn_search.Text = "Search";
		this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
		this.grd_SalesOrder.AllowPasteExcel = true;
		this.grd_SalesOrder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_SalesOrder.ClipboardData = null;
		this.grd_SalesOrder.Location = new System.Drawing.Point(-1, 45);
		this.grd_SalesOrder.MainView = this.gv_SalesOrder;
		this.grd_SalesOrder.Name = "grd_SalesOrder";
		this.grd_SalesOrder.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rCheckEdit });
		this.grd_SalesOrder.Size = new System.Drawing.Size(1052, 434);
		this.grd_SalesOrder.TabIndex = 3;
		this.grd_SalesOrder.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_SalesOrder });
		this.gv_SalesOrder.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_SalesOrder.ColumnKey = null;
		this.gv_SalesOrder.DisableFormSetting = false;
		this.gv_SalesOrder.DisplayException = false;
		this.gv_SalesOrder.EditColumn = null;
		this.gv_SalesOrder.FieldCurrency = null;
		this.gv_SalesOrder.FieldRate = null;
		this.gv_SalesOrder.FormId = "PickingRequest";
		this.gv_SalesOrder.GridControl = this.grd_SalesOrder;
		this.gv_SalesOrder.IgnoreFilter = false;
		this.gv_SalesOrder.IndexEditRow = -1;
		this.gv_SalesOrder.IndicatorWidth = 35;
		this.gv_SalesOrder.IsApzTable = false;
		this.gv_SalesOrder.IsInsertNewRow = false;
		this.gv_SalesOrder.IsLoadingData = false;
		this.gv_SalesOrder.IsMappingContextAction = false;
		this.gv_SalesOrder.ItemId = "PickingRequest";
		this.gv_SalesOrder.LstColumnAllowBatch = null;
		this.gv_SalesOrder.LstColumnNotAllowBatch = null;
		this.gv_SalesOrder.LstColumnNotEditable = null;
		this.gv_SalesOrder.LstColumNotSetting = null;
		this.gv_SalesOrder.Name = "gv_SalesOrder";
		this.gv_SalesOrder.UseBatchMod = false;
		this.gv_SalesOrder.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_SalesOrder_PopupMenuShowing);
		this.gv_SalesOrder.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_SalesOrder_CellValueChanged);
		this.rCheckEdit.AutoHeight = false;
		this.rCheckEdit.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.rCheckEdit.Name = "rCheckEdit";
		this.rCheckEdit.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Unchecked;
		this.rCheckEdit.ValueChecked = "Y";
		this.rCheckEdit.ValueUnchecked = "N";
		base.AcceptButton = this.btn_Release;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Close;
		base.ClientSize = new System.Drawing.Size(1051, 520);
		base.Controls.Add(this.splitContainerControl2);
		base.FormId = "PickingRequest";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_PickingRequest.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PickingRequest";
		this.Text = "Picking Request";
		base.Load += new System.EventHandler(this.frm_frm_PickingRequest_Load);
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Allocated).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Whs.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_SalesOrder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_SalesOrder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCheckEdit).EndInit();
		base.ResumeLayout(false);
	}
}
