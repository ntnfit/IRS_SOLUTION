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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_InventoryCountingRequest : XtraFormEx
{
	private const string OkString = "OK";

	private const string AddString = "Save";

	private DataTable _dtDoc;

	private bool _isLoading;

	private bool _isCpyWbs;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public Func<DataTable, int, Task> SendDataBackToMain;

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private ButtonEditCFL btn_WhsCode;

	private SimpleButton btn_Search;

	private SimpleButton btn_Ok;

	private SimpleButton btn_Cancel;

	private LabelLine labelLine1;

	private DateEditEx date_FromDate;

	private DateEditEx date_ToDate;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private TextEditEx txt_No;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemButtonEditCFL btn_WhsGrid;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	public int Id { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_Ok.Text = "Save";
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = "OK";
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_InventoryCountingRequest()
	{
		InitializeComponent();
		OnLoadAsync += frm_InventoryCountingRequest_Load;
	}

	private async Task frm_InventoryCountingRequest_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			btn_WhsCode.Properties.ValueMember = "WhsCode";
			btn_WhsCode.Properties.DisplayMember = "WhsCode";
			btn_WhsCode.Properties.TypeCFL = Enums.TypeCFL.GetListWareHouse;
			RepositoryItemButtonEditCFL properties = btn_WhsCode.Properties;
			properties.DataSourceLoaded = (Action<DataTable>)Delegate.Combine(properties.DataSourceLoaded, (Action<DataTable>)((DataTable dtData) =>
			{
				btn_WhsGrid.DataSource = dtData.Copy();
			}));
			btn_WhsCode.Properties.RefreshDataSource();
			btn_WhsCode.Properties.TypeDrillDown = ApzObjectTypes.oWarehouses;
			btn_WhsGrid.ValueMember = "WhsCode";
			btn_WhsGrid.DisplayMember = "WhsCode";
			btn_WhsGrid.TypeCFL = Enums.TypeCFL.GetListWareHouse;
			btn_WhsGrid.TypeDrillDown = ApzObjectTypes.oWarehouses;
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OCRQ"));
			date_FromDate.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_ToDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			new DataTable();
			string startFrom = DateTime.Today.AddDays(-Configuration.SearchDate).ToString("ddMMyyyy");
			string startTo = DateTime.Today.AddDays(Configuration.SearchDate).ToString("ddMMyyyy");
			string whs = Function.ToString(btn_WhsCode.EditValue);
			if (Id > 0)
			{
				txt_No.EditValue = Id;
			}
			DataTable dataSource = await LoadData(Id, whs, startFrom, startTo);
			Id = 0;
			grd_Doc.DataSource = dataSource;
			grd_Doc.DatasourceTable().AcceptChanges();
			gv_Doc.FocusedRowHandle = 0;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			string whs = Function.ToString(btn_WhsCode.EditValue);
			if (Id > 0)
			{
				txt_No.EditValue = Id;
			}
			string startFrom = (Function.ParseDateTime(date_FromDate.EditValue).HasValue ? ((DateTime)date_FromDate.EditValue).ToString("yyyyMMdd") : "");
			string startTo = (Function.ParseDateTime(date_ToDate.EditValue).HasValue ? ((DateTime)date_ToDate.EditValue).ToString("yyyyMMdd") : "");
			DataTable dataSource = await LoadData(Function.ParseInt(txt_No.EditValue), whs, startFrom, startTo);
			Id = 0;
			if (!_isLoading && (FormMode != Enums.FormViewModeType.Add || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes))
			{
				FormMode = Enums.FormViewModeType.View;
				grd_Doc.DataSourceTable.Rows.Clear();
				grd_Doc.DataSource = dataSource;
				grd_Doc.DatasourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					await Save();
				}
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			REFRESH();
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.GetSelectedRows() == null || gv_Doc.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				if (string.IsNullOrEmpty(Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"))))
				{
					grd_Doc.BeginUpdate();
					gv_Doc.DeleteSelectedRows();
					gv_Doc.EndUpdate();
					return;
				}
				int[] selectedRows = gv_Doc.GetSelectedRows();
				string text = "";
				int[] array = selectedRows;
				foreach (int rowHandle in array)
				{
					text = text + gv_Doc.GetRowCellValue(rowHandle, "DocEntry").ToString() + ",";
				}
				if (text.Length > 0)
				{
					text = text.Remove(text.Length - 1);
					string text2 = await _httpClient.InventoryCountingRequest.DeleteDocs(text);
					if (string.IsNullOrEmpty(text2))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						btn_Search_Click(null, null);
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text2);
					}
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (!grd_Doc.DataSourceTable.IsNotNull() || gv_Doc.GetFocusedDataRow() == null)
			{
				return;
			}
			string ID = Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "DocEntry"));
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			DataRow[] array = (from t in dataSourceTable.AsEnumerable()
				where Function.ToString(t["DocEntry"]) == ID
				select t).ToArray();
			if (array != null && array.Length != 0)
			{
				for (int num = 0; num < array.Count(); num++)
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow.ItemArray = array[num].ItemArray;
					int num2 = Function.ParseInt(dataSourceTable.Compute("MAX(DocEntry)", ""));
					dataRow["DocEntry"] = num2 + 1;
					dataSourceTable.Rows.Add(dataRow);
				}
				grd_Doc.DataSource = dataSourceTable;
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private async Task<int> Save()
	{
		try
		{
			WinCommons._OpenProcessing();
			_dtDoc = grd_Doc.DataSourceTable;
			if (_dtDoc != null && _dtDoc.Rows.Count > 0)
			{
				for (int i = 0; i < _dtDoc.Rows.Count; i++)
				{
					if (Function.ToString(_dtDoc.Rows[i]["WhsCode"]) == "")
					{
						_dtDoc.Rows.RemoveAt(i--);
					}
				}
			}
			_dtDoc.TableName = "dtDoc";
			HttpResult httpResult = await _httpClient.InventoryCountingRequest.Saves(_dtDoc);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				_isLoading = true;
				FormMode = Enums.FormViewModeType.View;
				btn_Search_Click(null, null);
				_isLoading = false;
				return 0;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			WinCommons._CloseProcessing();
			return 1;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
	}

	private async Task<DataTable> LoadData(int id, string whs, string startFrom, string startTo)
	{
		try
		{
			_isLoading = true;
			DataTable result = await _httpClient.InventoryCountingRequest.LoadData(id, whs.Trim(), startFrom, startTo);
			_isLoading = false;
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			_dtDoc = grd_Doc.DataSourceTable;
			if (_dtDoc == null || _dtDoc.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow dataRow = _dtDoc.NewRow();
			int num = Function.ParseInt(_dtDoc.Compute("MAX(DocEntry)", ""));
			dataRow["DocEntry"] = num + 1;
			dataRow["Active"] = "Y";
			dataRow["DocDate"] = DateTime.Now.Date;
			_dtDoc.Rows.Add(dataRow);
			grd_Doc.DataSource = _dtDoc;
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DataChanged()
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void frm_InventoryCountingRequest_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
		{
			e.Cancel = true;
		}
	}

	public async Task<int> FindNextLastFrist(Enums.NavigateType type)
	{
		try
		{
			DataTable dataTable = await _httpClient.InventoryCountingRequest.FindNextLastFrist(Function.ParseInt(txt_No.EditValue), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.DatasourceTable().AcceptChanges();
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				txt_No.EditValue = num;
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_InventoryCountingRequest));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_WhsGrid = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btn_WhsCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.btn_Search = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Ok = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_No = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WhsGrid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WhsCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(1, 58);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.btn_WhsGrid, this.repositoryItemCheckEdit1 });
		this.grd_Doc.Size = new System.Drawing.Size(976, 336);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.duplicateToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(125, 48);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oInventoryCountingRequest";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OCRQ";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.gridColumnEx1.Caption = "Doc. No";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "DocEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx2.Caption = "Counting Date";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx2.FieldName = "DocDate";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 85;
		this.gridColumnEx3.Caption = "Whs";
		this.gridColumnEx3.ColumnEdit = this.btn_WhsGrid;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "WhsCode";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.btn_WhsGrid.AutoHeight = false;
		this.btn_WhsGrid.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.btn_WhsGrid.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_WhsGrid.ContainerControl = this;
		this.btn_WhsGrid.DisplayMember = "WhsCode";
		this.btn_WhsGrid.DisplayMember2 = "";
		this.btn_WhsGrid.LoadDataSourceWhenEnter = false;
		this.btn_WhsGrid.MultiSelectMode = true;
		this.btn_WhsGrid.Name = "btn_WhsGrid";
		this.btn_WhsGrid.SeparateCharacterForMulti = ';';
		this.btn_WhsGrid.SeparateString = ", ";
		this.btn_WhsGrid.UsingAutoCompleteBox = false;
		this.btn_WhsGrid.UsingDataSource = true;
		this.btn_WhsGrid.ValidateWhenNoDataSource = false;
		this.btn_WhsGrid.ValueMember = "WhsCode";
		this.gridColumnEx4.Caption = "Remarks  ";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "Remarks";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx5.Caption = "Active ";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Active";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueGrayed = "N";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.btn_WhsCode.EditControl = null;
		this.btn_WhsCode.FieldName = null;
		this.btn_WhsCode.IsSearchControl = false;
		this.btn_WhsCode.Location = new System.Drawing.Point(303, 29);
		this.btn_WhsCode.Name = "btn_WhsCode";
		this.btn_WhsCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_WhsCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_WhsCode.Properties.ContainerControl = null;
		this.btn_WhsCode.Properties.DisplayMember = "WhsCode";
		this.btn_WhsCode.Properties.DisplayMember2 = "";
		this.btn_WhsCode.Properties.LoadDataSourceWhenEnter = false;
		this.btn_WhsCode.Properties.SeparateCharacterForMulti = ';';
		this.btn_WhsCode.Properties.SeparateString = ", ";
		this.btn_WhsCode.Properties.UsingAutoCompleteBox = false;
		this.btn_WhsCode.Properties.UsingDataSource = true;
		this.btn_WhsCode.Properties.ValidateWhenNoDataSource = false;
		this.btn_WhsCode.Properties.ValueMember = "WhsCode";
		this.btn_WhsCode.SettingInVisible = false;
		this.btn_WhsCode.SettingReadonly = false;
		this.btn_WhsCode.Size = new System.Drawing.Size(100, 20);
		this.btn_WhsCode.TabIndex = 1;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.Location = new System.Drawing.Point(409, 26);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 2;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.Location = new System.Drawing.Point(1, 400);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 3;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(79, 400);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 4;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(1, 10);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 5;
		this.labelLine1.Utext = "From Date";
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = null;
		this.date_FromDate.Location = new System.Drawing.Point(76, 7);
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
		this.date_FromDate.Size = new System.Drawing.Size(100, 20);
		this.date_FromDate.TabIndex = 10;
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
		this.date_ToDate.Location = new System.Drawing.Point(76, 29);
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
		this.date_ToDate.Size = new System.Drawing.Size(100, 20);
		this.date_ToDate.TabIndex = 12;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(1, 32);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 11;
		this.labelLine2.Utext = "To Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Doc. No";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(222, 10);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 13;
		this.labelLine3.Utext = "Doc. No";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Whs Code";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(222, 32);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 14;
		this.labelLine4.Utext = "Whs Code";
		this.txt_No.EditControl = null;
		this.txt_No.FieldName = null;
		this.txt_No.IsSearchControl = false;
		this.txt_No.Location = new System.Drawing.Point(303, 7);
		this.txt_No.Name = "txt_No";
		this.txt_No.SettingInVisible = false;
		this.txt_No.SettingReadonly = false;
		this.txt_No.Size = new System.Drawing.Size(100, 20);
		this.txt_No.TabIndex = 15;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(978, 427);
		base.Controls.Add(this.txt_No);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.date_ToDate);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.date_FromDate);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.btn_WhsCode);
		base.Controls.Add(this.grd_Doc);
		base.FormId = "oInventoryCountingRequest";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_InventoryCountingRequest";
		this.Text = "Inventory Counting Request";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_InventoryCountingRequest_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WhsGrid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WhsCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
