using System;
using System.Collections.Generic;
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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.DMS;

public class frm_DMSDelivery : XtraFormEx
{
	private DataTable _dtDoc;

	private DataTable _dtLine;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private DataTable dtSearch;

	private readonly Dictionary<string, string> _lstStatus = new Dictionary<string, string>
	{
		{ "O", "Opend" },
		{ "L", "Closed" }
	};

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ContextMenuStrip ctm_Line;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private TextEditEx txt_No;

	private LabelLine labelLine1;

	private RepositoryItemButtonEditCFL btn_UserSign;

	private RepositoryItemLookUpEditSimple luk_StatusGrd;

	private SimpleButtonEx btn_Search;

	private LookUpEditEx luk_Status;

	private LabelLine labelLine4;

	private DateEditEx date_toDate;

	private DateEditEx date_fromDate;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine5;

	private TextEditEx txt_User;

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
				btn_Ok.Text = Resources.ButtonAdd;
				addNewToolStripMenuItem.Enabled = false;
				deleteRowToolStripMenuItem.Enabled = true;
				addRowToolStripMenuItem.Enabled = true;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = Resources.ButtonUpdate;
				addNewToolStripMenuItem.Enabled = true;
				deleteRowToolStripMenuItem.Enabled = true;
				addRowToolStripMenuItem.Enabled = true;
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = Resources.ButtonOK;
				addNewToolStripMenuItem.Enabled = true;
				deleteRowToolStripMenuItem.Enabled = true;
				addRowToolStripMenuItem.Enabled = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_DMSDelivery()
	{
		InitializeComponent();
		FormId = "oDMSDelivery";
		gv_Doc.FormId = "oDMSDelivery";
		gv_Doc.ItemId = "APZ_D_ODLO";
		gv_Line.FormId = "oDMSDelivery";
		gv_Line.ItemId = "APZ_D_DLO1";
		OnLoadAsync += frm_DMSDelivery_Load;
	}

	private async Task frm_DMSDelivery_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			luk_Status.SetDataSource(new Dictionary<string, string>
			{
				{ "", "" },
				{ "Waiting", "Waiting" },
				{ "Processing", "Processing" },
				{ "Done", "Done" }
			});
			new DataTable();
			DateTime dateTime = DateTime.Today.AddDays(-Configuration.SearchDate);
			DateTime dateTime2 = DateTime.Today.AddDays(Configuration.SearchDate);
			date_fromDate.EditValue = dateTime;
			date_toDate.EditValue = dateTime2;
			string value = Function.ToString(luk_Status.EditValue);
			if (Id > 0)
			{
				txt_No.EditValue = Id;
			}
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dataRow["docentry"] = Id;
			dataRow["fromDate"] = dateTime;
			dataRow["toDate"] = dateTime2;
			dataRow["userSign"] = txt_User.Text;
			dataRow["status"] = value;
			dtSearch.Rows.Add(dataRow);
			DataSet ds = await _httpClient.DMSDeliveryClient.SearchDeliveryOrder(dtSearch.Copy());
			await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, ds);
			if (ds.IsHasStructure() && !ds.Tables[1].IsNotNull())
			{
				await LoadLineById(0);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private void CreateDataSearch()
	{
		if (!dtSearch.IsNotNull() || dtSearch.Columns.Count == 0)
		{
			dtSearch = new DataTable();
			dtSearch.Columns.Add(new DataColumn("docEntry", typeof(int)));
			dtSearch.Columns.Add(new DataColumn("userSign", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("fromDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("toDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("status", typeof(string)));
		}
		else
		{
			dtSearch.Rows.Clear();
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Save(1);
				break;
			case Enums.FormViewModeType.Update:
				await Save(2);
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> Save(int type)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			return 0;
		}
		try
		{
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			if (type == 2)
			{
				DataTable dataSourceTable = grd_Doc.DataSourceTable;
				dataSourceTable.AcceptChanges();
				DataRow[] array = dataSourceTable.Select("DocEntry=" + Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry")));
				if (array.Length != 0)
				{
					_dtDoc = array.CopyToDataTable();
				}
			}
			else
			{
				_dtDoc = grd_Doc.DataSourceTable;
			}
			_dtLine = grd_Line.DataSourceTable().Copy();
			if (_dtLine != null && _dtLine.Rows.Count > 0)
			{
				for (int i = 0; i < _dtLine.Rows.Count; i++)
				{
					if (Function.ToString(_dtLine.Rows[i]["ItemCode"]) == "")
					{
						_dtLine.Rows.RemoveAt(i--);
					}
				}
			}
			_dtDoc.TableName = "dtDoc";
			_dtLine.TableName = "dtLine";
			dataSet.Tables.Add(_dtDoc.Copy());
			dataSet.Tables.Add(_dtLine.Copy());
			HttpResult httpResult = ((type != 2) ? (await _httpClient.DMSDeliveryClient.Add(dataSet)) : (await _httpClient.DMSDeliveryClient.Update(dataSet)));
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
				if (type == 2)
				{
					grd_Doc.DatasourceTable().AcceptChanges();
					grd_Line.DatasourceTable().AcceptChanges();
				}
				else
				{
					btn_Search_Click(null, null);
				}
				return 0;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			WinCommons._CloseProcessing();
			return 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
		}
		else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				await LoadLineById(id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		NewLine();
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Line.DataRowCount > 0)
			{
				grd_Line.BeginUpdate();
				gv_Line.DeleteSelectedRows();
				grd_Line.EndUpdate();
				grd_Line.DatasourceTable().AcceptChanges();
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadLineById(int id)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			_isLoading = true;
			DataSet ds = await _httpClient.DMSDeliveryClient.GetLines(id);
			await GridViewHelper.GenerateGrid(grd_Line, gv_Line, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || (FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				FormMode = Enums.FormViewModeType.Add;
				grd_Doc.DataSourceTable.Rows.Clear();
				grd_Line.DataSourceTable().Rows.Clear();
				_dtDoc = grd_Doc.DataSourceTable;
				if (_dtDoc == null || _dtDoc.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				}
				DataRow dataRow = _dtDoc.NewRow();
				dataRow["DocEntry"] = 0;
				dataRow["Status"] = "Waiting";
				_dtDoc.Rows.Add(dataRow);
				grd_Doc.DataSource = _dtDoc;
				NewLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewLine()
	{
		try
		{
			_dtLine = grd_Line.DataSourceTable();
			if (_dtLine != null || _dtLine.Columns.Count != 0)
			{
				DataRow row = _dtLine.NewRow();
				_dtLine.Rows.Add(row);
				_dtLine.AcceptChanges();
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DataChanged()
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (!_isLoading && ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes))
			{
				FormMode = Enums.FormViewModeType.View;
				string value = Function.ToString(luk_Status.EditValue);
				object editValue = date_fromDate.EditValue;
				object editValue2 = date_toDate.EditValue;
				if (Id > 0)
				{
					txt_No.EditValue = Id;
				}
				CreateDataSearch();
				DataRow dataRow = dtSearch.NewRow();
				dataRow["docentry"] = Id;
				dataRow["fromDate"] = editValue;
				dataRow["toDate"] = editValue2;
				dataRow["userSign"] = txt_User.Text;
				dataRow["status"] = value;
				dtSearch.Rows.Add(dataRow);
				DataSet ds = await _httpClient.DMSDeliveryClient.SearchDeliveryOrder(dtSearch.Copy());
				await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, ds);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.Update)
			{
				DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				if (dialogResult == DialogResult.Yes && dialogResult == DialogResult.Yes)
				{
					grd_Doc.DataSourceTable.RejectChanges();
					FormMode = Enums.FormViewModeType.View;
				}
				if (dialogResult == DialogResult.No)
				{
					e.Allow = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			_ = gv_Doc.FocusedRowHandle;
			if (e.Column.FieldName == "DocEntry")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable source = grd_Line.DataSourceTable();
			if (e.Column.FieldName == "ItemCode" && (from t in source.AsEnumerable()
				where string.IsNullOrWhiteSpace(Function.ToString(t["ItemCode"]))
				select t).ToList().Count <= 0)
			{
				NewLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_DMSDelivery));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.btn_UserSign = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.luk_StatusGrd = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_No = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.date_fromDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_toDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.luk_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.txt_User = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_UserSign).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusGrd).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.ctm_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_toDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_toDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_User.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 48);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1113, 504);
		this.splitContainerControl1.SplitterPosition = 211;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = false;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.btn_UserSign, this.luk_StatusGrd });
		this.grd_Doc.Size = new System.Drawing.Size(1113, 211);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.ImageScalingSize = new System.Drawing.Size(28, 28);
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.addNewToolStripMenuItem });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(181, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oQuestionMasterData";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.ItemId = "APZ_D_OCHM";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.btn_UserSign.AutoHeight = false;
		this.btn_UserSign.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_UserSign.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_UserSign.ContainerControl = this;
		this.btn_UserSign.DisplayMember = "";
		this.btn_UserSign.DisplayMember2 = "";
		this.btn_UserSign.LoadDataSourceWhenEnter = false;
		this.btn_UserSign.Name = "btn_UserSign";
		this.btn_UserSign.SeparateCharacterForMulti = ';';
		this.btn_UserSign.SeparateString = ", ";
		this.btn_UserSign.UsingAutoCompleteBox = false;
		this.btn_UserSign.UsingDataSource = true;
		this.btn_UserSign.ValidateWhenNoDataSource = false;
		this.btn_UserSign.ValueMember = "";
		this.luk_StatusGrd.AutoHeight = false;
		this.luk_StatusGrd.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusGrd.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusGrd.IsSearchControl = false;
		this.luk_StatusGrd.Name = "luk_StatusGrd";
		this.luk_StatusGrd.NullText = "";
		this.luk_StatusGrd.ShowFooter = false;
		this.luk_StatusGrd.ShowHeader = false;
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.ctm_Line;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.Size = new System.Drawing.Size(1113, 283);
		this.grd_Line.TabIndex = 0;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.ctm_Line.ImageScalingSize = new System.Drawing.Size(28, 28);
		this.ctm_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.ctm_Line.Name = "ctm_Line";
		this.ctm_Line.Size = new System.Drawing.Size(134, 48);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Line.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Line.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oQuestionMasterData";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = false;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.ItemId = "APZ_D_CHM1";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumnNotEditable = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Line.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Line.OptionsPrint.AutoWidth = false;
		this.gv_Line.OptionsPrint.UsePrintStyles = false;
		this.gv_Line.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Line.OptionsView.ColumnAutoWidth = false;
		this.gv_Line.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Line.OptionsView.ShowGroupedColumns = true;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanged);
		this.gv_Line.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanging);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(1, 558);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(82, 558);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.txt_No.FieldName = null;
		this.txt_No.IsSearchControl = false;
		this.txt_No.Location = new System.Drawing.Point(82, 18);
		this.txt_No.Name = "txt_No";
		this.txt_No.Size = new System.Drawing.Size(100, 20);
		this.txt_No.TabIndex = 3;
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(7, 18);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(5);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(72, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 4;
		this.labelLine1.Utext = "Doc.No";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(190, 18);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(5);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(72, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 5;
		this.labelLine2.Utext = "From Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(365, 18);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(5);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(72, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 6;
		this.labelLine3.Utext = "To Date";
		this.date_fromDate.EditValue = null;
		this.date_fromDate.FieldName = null;
		this.date_fromDate.Location = new System.Drawing.Point(259, 18);
		this.date_fromDate.Name = "date_fromDate";
		this.date_fromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_fromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_fromDate.Properties.IsSearchControl = false;
		this.date_fromDate.Size = new System.Drawing.Size(100, 20);
		this.date_fromDate.TabIndex = 7;
		this.date_toDate.EditValue = null;
		this.date_toDate.FieldName = null;
		this.date_toDate.Location = new System.Drawing.Point(427, 18);
		this.date_toDate.Name = "date_toDate";
		this.date_toDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_toDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_toDate.Properties.IsSearchControl = false;
		this.date_toDate.Size = new System.Drawing.Size(100, 20);
		this.date_toDate.TabIndex = 8;
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(748, 17);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(5);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(55, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 9;
		this.labelLine4.Utext = "Status";
		this.luk_Status.FieldName = null;
		this.luk_Status.IsSearchControl = false;
		this.luk_Status.Location = new System.Drawing.Point(798, 17);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Properties.NullText = "";
		this.luk_Status.Size = new System.Drawing.Size(100, 20);
		this.luk_Status.TabIndex = 10;
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(916, 15);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 11;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(547, 18);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(5);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(52, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 13;
		this.labelLine5.Utext = "User";
		this.txt_User.FieldName = null;
		this.txt_User.IsSearchControl = false;
		this.txt_User.Location = new System.Drawing.Point(600, 18);
		this.txt_User.Name = "txt_User";
		this.txt_User.Size = new System.Drawing.Size(123, 20);
		this.txt_User.TabIndex = 12;
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(1116, 585);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.txt_User);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.luk_Status);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.date_toDate);
		base.Controls.Add(this.date_fromDate);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txt_No);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oQuestionMasterData";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DMSDelivery";
		this.Text = "DMS Delivery Management";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_UserSign).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusGrd).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.ctm_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_fromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_toDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_toDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_User.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
