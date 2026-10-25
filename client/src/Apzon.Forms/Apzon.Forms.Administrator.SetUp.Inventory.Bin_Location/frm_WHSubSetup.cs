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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location;

public class frm_WHSubSetup : XtraFormEx
{
	private const string FormController = "WHSubSetup";

	public int WhFldAbs;

	public string SlCode = "";

	private DataTable _dtOriginal;

	private IContainer components;

	private GridControlEx grdWHSub;

	private GridViewEx gvWHSub;

	private GridColumnEx colCode;

	private GridColumnEx colDescr;

	private GridColumnEx colNoOfBinLocation;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnOk;

	private RepositoryItemButtonEdit eb_NoOfbinLocations;

	private LookUpEdit cbWHSub;

	private LabelLine labelLine1;

	private SimpleButtonEx btnManage;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOk.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOk.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_WHSubSetup()
	{
		InitializeComponent();
	}

	private async Task<int> GetUserSignByUserCode_tableOUSR()
	{
		string userCodeLogin = GlobalData.UserCodeLogin;
		try
		{
			using HttpClientHelper<UsersImpl> helper = new HttpClientHelper<UsersImpl>(Configuration.ApiServer, "Users");
			DataTable dataTable = await helper.GetDataTableAsync("GetUserSign", new
			{
				nameLogin = userCodeLogin
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["UserSign"].ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		return -1;
	}

	private async Task LoadCb()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "WHSubSetup");
			DataTable dataTable = await hp.GetAsync("GetAllWHSub");
			var dataSource = from r in dataTable.AsEnumerable()
				select new
				{
					id = Function.ParseInt(r["FldNum"]),
					text = Function.ToString(r["DispName"])
				};
			cbWHSub.Properties.DataSource = dataSource;
			cbWHSub.EditValue = ((WhFldAbs > 0) ? WhFldAbs : Function.ParseInt(dataTable.Rows[0]["FldNum"]));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private async Task LoadGridFromCb(int whSublevel)
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "WHSubSetup");
			DataTable dataTable = await hp.GetAsync("GetWHSubByAbs", new { whSublevel });
			grdWHSub.DataSource = dataTable;
			grdWHSub.RefreshDataSource();
			int num = gvWHSub.LocateByValue("SLCode", SlCode);
			_dtOriginal = dataTable.Copy();
			AddNewRow();
			if (num != int.MinValue)
			{
				gvWHSub.FocusedRowHandle = num;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private bool CheckUsedWHSub(int fldAbs, string code)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "WHSubSetup");
			if (string.IsNullOrEmpty(httpClientHelper.Get("IsUsedInBinLocCodes", new
			{
				fldAbs = fldAbs,
				slCode = code
			})))
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
			return false;
		}
	}

	private void AddNewRow()
	{
		DataTable dataTable = grdWHSub.DataSource as DataTable;
		if (!dataTable.IsNotNull())
		{
			dataTable = GetTableStructure();
		}
		DataRow dataRow = dataTable.NewRow();
		dataRow["FldAbs"] = Function.ParseInt(cbWHSub.EditValue);
		dataTable.Rows.Add(dataRow);
		dataTable.AcceptChanges();
		grdWHSub.DataSource = dataTable;
		grdWHSub.RefreshDataSource();
	}

	private DataTable GetTableStructure()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("AbsEntry", typeof(int));
		dataTable.Columns.Add("FldAbs", typeof(int));
		dataTable.Columns.Add("SLCode", typeof(string));
		dataTable.Columns.Add("Descr", typeof(string));
		dataTable.Columns.Add("NoOfBin", typeof(int));
		dataTable.AcceptChanges();
		return dataTable;
	}

	private async Task<string> Save()
	{
		try
		{
			DataTable dataTable = grdWHSub.DataSourceTable.Copy();
			dataTable.Columns.Add("Type", typeof(string));
			DataTable dataTable2 = _dtOriginal.Copy();
			foreach (DataRow row in dataTable.Rows)
			{
				if (Function.ParseInt(row["AbsEntry"]) == 0)
				{
					row["Type"] = "A";
					continue;
				}
				DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["AbsEntry"]) == Function.ParseInt(row["AbsEntry"]));
				if (dataRow != null)
				{
					row["Type"] = "U";
					dataTable2.Rows.Remove(dataRow);
				}
			}
			foreach (DataRow row2 in dataTable2.Rows)
			{
				dataTable.ImportRow(row2);
			}
			for (int num = dataTable.Rows.Count - 1; num >= 0; num--)
			{
				if (string.IsNullOrWhiteSpace(Function.ToString(dataTable.Rows[num]["AbsEntry"])) && string.IsNullOrWhiteSpace(Function.ToString(dataTable.Rows[num]["SLCode"])))
				{
					dataTable.Rows.RemoveAt(num);
				}
			}
			dataTable.AcceptChanges();
			DataTable pr = dataTable.Select("", "Type ASC").CopyToDataTable();
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "WHSubSetup");
			HttpResult httpResult = await http.PostAsync("Save", pr);
			if (httpResult.MessageCode != Enums.MessageCode.Success)
			{
				return httpResult.Message;
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
			return ex.Message;
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		SimpleButtonEx simpleButtonEx = btnOk;
		SimpleButtonEx simpleButtonEx2 = btnManage;
		bool flag = (cbWHSub.Enabled = isEnable);
		bool enabled = (simpleButtonEx2.Enabled = flag);
		simpleButtonEx.Enabled = enabled;
		gvWHSub.ReadOnly = !isEnable;
	}

	private async void frm_WHSublevel_Load(object sender, EventArgs e)
	{
		await LoadCb();
		await LoadGridFromCb(Function.ParseInt(cbWHSub.EditValue));
	}

	private async void cbWHSub_EditValueChanged(object sender, EventArgs e)
	{
		await LoadGridFromCb(Function.ParseInt(cbWHSub.EditValue));
	}

	private void eb_NoOfbinLocations_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gvWHSub.GetFocusedRowCellValue("NoOfBin")) > 0)
			{
				frm_BinLocList frm_BinLocList2 = new frm_BinLocList();
				int rowHandle = gvWHSub.GetSelectedRows()[0];
				DataRow dataRow = gvWHSub.GetDataRow(rowHandle);
				frm_BinLocList2.SlCode = Function.ToString(dataRow["SLCode"]);
				frm_BinLocList2.FldAbs = Function.ToString(cbWHSub.EditValue);
				frm_BinLocList2.MdiParent = MdiParent;
				frm_BinLocList2.Show();
			}
			else
			{
				WinCommons.Info(Enums.MessageCode.DataIsNotAvailable, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.View;
		Close();
	}

	private async void btnOk_Click(object sender, EventArgs e)
	{
		EnableDisableControl(isEnable: false);
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				foreach (DataRow row in grdWHSub.DataSourceTable.Rows)
				{
					if (string.IsNullOrEmpty(Function.ToString(row["SLCode"])) && !string.IsNullOrWhiteSpace(Function.ToString(row["Descr"])))
					{
						WinCommons.Error(Enums.MessageCode.None, "Invalid code");
						EnableDisableControl(isEnable: true);
						return;
					}
				}
				WinCommons._OpenProcessing();
				string text = await Save();
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					await LoadGridFromCb(Function.ParseInt(cbWHSub.EditValue));
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		EnableDisableControl(isEnable: true);
	}

	private void gvWHSub_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void btnManage_Click(object sender, EventArgs e)
	{
		try
		{
			frm_WHSubManager frm_WHSubManager2 = new frm_WHSubManager();
			frm_WHSubManager2.MdiParent = MdiParent;
			frm_WHSubManager2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void cbWHSub_EditValueChanging(object sender, ChangingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
		{
			e.Cancel = true;
		}
	}

	private void gvWHSub_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			DataTable dataTable = grdWHSub.DataSource as DataTable;
			if (dataTable.IsNotNull() && e.Column == colCode && e.RowHandle == dataTable.Rows.Count - 1)
			{
				AddNewRow();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdWHSub.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			int focusedRowHandle = gvWHSub.FocusedRowHandle;
			string code = Function.ToString(gvWHSub.GetFocusedRowCellValue("SLCode"));
			if (!CheckUsedWHSub(Function.ParseInt(cbWHSub.EditValue), code))
			{
				WinCommons.Error(Enums.MessageCode.None, "You cannot delete this row");
				return;
			}
			dataTable.Rows.RemoveAt(focusedRowHandle);
			dataTable.AcceptChanges();
			if (dataTable.AsEnumerable().All((DataRow r) => Function.ParseInt(r["AbsEntry"]) != 0))
			{
				AddNewRow();
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvWHSub_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (Function.ParseInt(cbWHSub.EditValue) == 1 && gvWHSub.FocusedColumn == colCode && Function.ToString(gvWHSub.GetFocusedValue()).Equals("SYSTEM-BIN-LOCATION"))
		{
			e.Cancel = true;
		}
	}

	private void gvWHSub_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		if (e.Column == colNoOfBinLocation)
		{
			if (!string.IsNullOrEmpty(Function.ToString(gvWHSub.GetRowCellValue(e.RowHandle, "AbsEntry"))))
			{
				e.RepositoryItem = eb_NoOfbinLocations;
			}
			else
			{
				e.RepositoryItem = null;
			}
		}
	}

	private void gvWHSub_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		if (sender is ColumnView columnView && columnView.FocusedColumn == colCode && !string.IsNullOrEmpty(Function.ToString(gvWHSub.GetFocusedRowCellValue("AbsEntry"))))
		{
			string text = Function.ToString(columnView.ActiveEditor.OldEditValue);
			if (!text.Equals(Function.ToString(e.Value)) && !CheckUsedWHSub(Function.ParseInt(cbWHSub.EditValue), text))
			{
				e.Valid = false;
			}
		}
	}

	private void gvWHSub_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		WinCommons.Error(Enums.MessageCode.CannotDeleteWHSublevelCode, "");
		e.ExceptionMode = ExceptionMode.Ignore;
	}

	private async void frm_WHSubSetup_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Update)
		{
			return;
		}
		switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question))
		{
		case DialogResult.Yes:
		{
			e.Cancel = true;
			EnableDisableControl(isEnable: false);
			WinCommons._OpenProcessing();
			string text = await Save();
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				FormMode = Enums.FormViewModeType.View;
				Close();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				EnableDisableControl(isEnable: true);
			}
			break;
		}
		case DialogResult.Cancel:
			e.Cancel = true;
			break;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location.frm_WHSubSetup));
		this.grdWHSub = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvWHSub = new Apzon.Controls.Grid.GridViewEx();
		this.colCode = new Apzon.Controls.Grid.GridColumnEx();
		this.colDescr = new Apzon.Controls.Grid.GridColumnEx();
		this.colNoOfBinLocation = new Apzon.Controls.Grid.GridColumnEx();
		this.eb_NoOfbinLocations = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.cbWHSub = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btnManage = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdWHSub).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvWHSub).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.eb_NoOfbinLocations).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbWHSub.Properties).BeginInit();
		base.SuspendLayout();
		this.grdWHSub.AllowPasteExcel = true;
		this.grdWHSub.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdWHSub.ClipboardData = null;
		this.grdWHSub.ContextMenuStrip = this.contextMenuStrip1;
		this.grdWHSub.Location = new System.Drawing.Point(0, 35);
		this.grdWHSub.MainView = this.gvWHSub;
		this.grdWHSub.Name = "grdWHSub";
		this.grdWHSub.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.eb_NoOfbinLocations });
		this.grdWHSub.Size = new System.Drawing.Size(650, 281);
		this.grdWHSub.TabIndex = 0;
		this.grdWHSub.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvWHSub });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvWHSub.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvWHSub.ColumnKey = null;
		this.gvWHSub.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.colCode, this.colDescr, this.colNoOfBinLocation });
		this.gvWHSub.DisableFormSetting = false;
		this.gvWHSub.DisplayException = false;
		this.gvWHSub.EditColumn = null;
		this.gvWHSub.FieldCurrency = null;
		this.gvWHSub.FieldRate = null;
		this.gvWHSub.FormId = null;
		this.gvWHSub.GridControl = this.grdWHSub;
		this.gvWHSub.IgnoreFilter = false;
		this.gvWHSub.IndexEditRow = -1;
		this.gvWHSub.IndicatorWidth = 40;
		this.gvWHSub.IsApzTable = false;
		this.gvWHSub.IsInsertNewRow = false;
		this.gvWHSub.IsLoadingData = false;
		this.gvWHSub.IsMappingContextAction = false;
		this.gvWHSub.ItemId = null;
		this.gvWHSub.LstColumnAllowBatch = null;
		this.gvWHSub.LstColumnNotAllowBatch = null;
		this.gvWHSub.LstColumnNotEditable = null;
		this.gvWHSub.LstColumNotSetting = null;
		this.gvWHSub.Name = "gvWHSub";
		this.gvWHSub.OptionsView.ShowFooter = true;
		this.gvWHSub.OptionsView.ShowGroupPanel = false;
		this.gvWHSub.UseBatchMod = false;
		this.gvWHSub.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvWHSub_CustomDrawRowIndicator);
		this.gvWHSub.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvWHSub_CustomRowCellEdit);
		this.gvWHSub.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvWHSub_ShowingEditor);
		this.gvWHSub.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvWHSub_CellValueChanged);
		this.gvWHSub.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gvWHSub_ValidatingEditor);
		this.gvWHSub.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gvWHSub_InvalidValueException);
		this.colCode.Caption = "Code";
		this.colCode.CurrencyDateField = null;
		this.colCode.CurrencyField = null;
		this.colCode.CurrencyFixed = null;
		this.colCode.CurrencyFixedRate = 0.0;
		this.colCode.CurrencyRateField = null;
		this.colCode.FieldName = "SLCode";
		this.colCode.Name = "colCode";
		this.colCode.PrimaryKey = true;
		this.colCode.UnitField = null;
		this.colCode.Visible = true;
		this.colCode.VisibleIndex = 0;
		this.colCode.Width = 194;
		this.colDescr.Caption = "Description";
		this.colDescr.CurrencyDateField = null;
		this.colDescr.CurrencyField = null;
		this.colDescr.CurrencyFixed = null;
		this.colDescr.CurrencyFixedRate = 0.0;
		this.colDescr.CurrencyRateField = null;
		this.colDescr.FieldName = "Descr";
		this.colDescr.Name = "colDescr";
		this.colDescr.PrimaryKey = false;
		this.colDescr.UnitField = null;
		this.colDescr.Visible = true;
		this.colDescr.VisibleIndex = 1;
		this.colDescr.Width = 194;
		this.colNoOfBinLocation.AppearanceCell.BackColor = System.Drawing.Color.LightGray;
		this.colNoOfBinLocation.AppearanceCell.BackColor2 = System.Drawing.Color.LightGray;
		this.colNoOfBinLocation.AppearanceCell.Options.UseBackColor = true;
		this.colNoOfBinLocation.AppearanceCell.Options.UseTextOptions = true;
		this.colNoOfBinLocation.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.colNoOfBinLocation.Caption = "No. Of Bin Locations";
		this.colNoOfBinLocation.CurrencyDateField = null;
		this.colNoOfBinLocation.CurrencyField = null;
		this.colNoOfBinLocation.CurrencyFixed = null;
		this.colNoOfBinLocation.CurrencyFixedRate = 0.0;
		this.colNoOfBinLocation.CurrencyRateField = null;
		this.colNoOfBinLocation.FieldName = "NoOfBin";
		this.colNoOfBinLocation.Name = "colNoOfBinLocation";
		this.colNoOfBinLocation.OptionsColumn.ReadOnly = true;
		this.colNoOfBinLocation.PrimaryKey = false;
		this.colNoOfBinLocation.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.colNoOfBinLocation.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "NoOfBin", "{0}")
		});
		this.colNoOfBinLocation.UnitField = null;
		this.colNoOfBinLocation.Visible = true;
		this.colNoOfBinLocation.VisibleIndex = 2;
		this.colNoOfBinLocation.Width = 198;
		this.eb_NoOfbinLocations.Appearance.Options.UseImage = true;
		this.eb_NoOfbinLocations.AppearanceFocused.BorderColor = System.Drawing.Color.Transparent;
		this.eb_NoOfbinLocations.AppearanceFocused.Options.UseBorderColor = true;
		this.eb_NoOfbinLocations.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.eb_NoOfbinLocations.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.eb_NoOfbinLocations.Name = "eb_NoOfbinLocations";
		this.eb_NoOfbinLocations.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.eb_NoOfbinLocations.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.eb_NoOfbinLocations_ButtonClick);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(93, 322);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 5;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.EditControl = null;
		this.btnOk.EditValue = "OK";
		this.btnOk.FieldName = null;
		this.btnOk.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btnOk.IsSearchControl = false;
		this.btnOk.Location = new System.Drawing.Point(12, 322);
		this.btnOk.Name = "btnOk";
		this.btnOk.ReadOnly = false;
		this.btnOk.SettingInVisible = false;
		this.btnOk.SettingReadonly = false;
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 4;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.cbWHSub.Location = new System.Drawing.Point(155, 9);
		this.cbWHSub.Name = "cbWHSub";
		this.cbWHSub.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbWHSub.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("id", "FldAbs", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("text", "Display Name")
		});
		this.cbWHSub.Properties.DisplayMember = "text";
		this.cbWHSub.Properties.NullText = "";
		this.cbWHSub.Properties.PopupSizeable = false;
		this.cbWHSub.Properties.ShowFooter = false;
		this.cbWHSub.Properties.ShowHeader = false;
		this.cbWHSub.Properties.ValueMember = "id";
		this.cbWHSub.RightToLeft = System.Windows.Forms.RightToLeft.No;
		this.cbWHSub.Size = new System.Drawing.Size(224, 20);
		this.cbWHSub.TabIndex = 3;
		this.cbWHSub.EditValueChanged += new System.EventHandler(this.cbWHSub_EditValueChanged);
		this.cbWHSub.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbWHSub_EditValueChanging);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Warehouse Sublevel";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 9);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(137, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 6;
		this.labelLine1.Utext = "Warehouse Sublevel";
		this.btnManage.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnManage.EditControl = null;
		this.btnManage.EditValue = "Manage Sublevel Codes";
		this.btnManage.FieldName = null;
		this.btnManage.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnManage.ImageOptions.Image");
		this.btnManage.IsSearchControl = false;
		this.btnManage.Location = new System.Drawing.Point(477, 322);
		this.btnManage.Name = "btnManage";
		this.btnManage.ReadOnly = false;
		this.btnManage.SettingInVisible = false;
		this.btnManage.SettingReadonly = false;
		this.btnManage.Size = new System.Drawing.Size(161, 23);
		this.btnManage.TabIndex = 7;
		this.btnManage.Text = "Manage Sublevel Codes";
		this.btnManage.Click += new System.EventHandler(this.btnManage_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(650, 357);
		base.Controls.Add(this.btnManage);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.grdWHSub);
		base.Controls.Add(this.cbWHSub);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WHSubSetup";
		this.Text = "Warehouse Sublevel Codes - Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_WHSubSetup_FormClosing);
		base.Load += new System.EventHandler(this.frm_WHSublevel_Load);
		((System.ComponentModel.ISupportInitialize)this.grdWHSub).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvWHSub).EndInit();
		((System.ComponentModel.ISupportInitialize)this.eb_NoOfbinLocations).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbWHSub.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
