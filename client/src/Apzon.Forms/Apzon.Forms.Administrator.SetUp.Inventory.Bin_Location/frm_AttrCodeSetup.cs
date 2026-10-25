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
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location;

public class frm_AttrCodeSetup : XtraFormEx
{
	private const string FormController = "AttrCodeSetup";

	public int FldNum;

	public string AttrValue = "";

	private DataTable _dtOriginal;

	private IContainer components;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnOk;

	private GridControlEx grdAttrCode;

	private GridViewEx gvAttrCode;

	private GridColumnEx colCode;

	private GridColumnEx colNoOfBinLocation;

	private LabelLine labelLine1;

	private LookUpEdit cbAttr;

	private RepositoryItemButtonEdit rep_BinLoc;

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

	public frm_AttrCodeSetup()
	{
		InitializeComponent();
	}

	private async Task LoadData(int fldNum)
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AttrCodeSetup");
			DataTable dataTable = await helper.GetAsync("GetAttributeByFldNum", new { fldNum });
			int num = gvAttrCode.LocateByValue("AttrValue", AttrValue);
			grdAttrCode.DataSource = dataTable;
			grdAttrCode.RefreshDataSource();
			_dtOriginal = dataTable.Copy();
			AddNewRow();
			if (num != int.MinValue)
			{
				gvAttrCode.FocusedRowHandle = num;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadCb()
	{
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "AttrCodeSetup");
		DataTable dataTable = await hp.GetAsync("GetAttribute");
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			var dataSource = from r in dataTable.AsEnumerable()
				select new
				{
					FldNum = Function.ParseInt(r["FldNum"]),
					FldAbs = Function.ParseInt(r["AbsEntry"]),
					Text = Function.ToString(r["DispName"])
				};
			cbAttr.Properties.DataSource = dataSource;
			cbAttr.EditValue = ((FldNum > 0) ? FldNum : Function.ParseInt(dataTable.Rows[0]["FldNum"]));
		}
	}

	private bool CheckExistInBinLoc(int fldAbs, string attr)
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "AttrCodeSetup");
			if (string.IsNullOrEmpty(httpClientHelper.Get("CheckExistInBinLoc", new
			{
				fldAbs = fldAbs,
				attrValue = attr
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
		try
		{
			DataTable dataTable = grdAttrCode.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				dataTable = GetTableStructure();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["FldAbs"] = Function.ParseInt(cbAttr.GetColumnValue("FldAbs"));
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grdAttrCode.DataSource = dataTable;
			grdAttrCode.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private DataTable GetTableStructure()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("AbsEntry", typeof(int));
		dataTable.Columns.Add("FldAbs", typeof(int));
		dataTable.Columns.Add("AttrValue", typeof(string));
		dataTable.Columns.Add("NoOfBin", typeof(int));
		dataTable.AcceptChanges();
		return dataTable;
	}

	private async Task<string> Save()
	{
		try
		{
			DataTable dataTable = grdAttrCode.DataSourceTable.Copy();
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
				if (string.IsNullOrWhiteSpace(Function.ToString(dataTable.Rows[num]["AbsEntry"])) && string.IsNullOrWhiteSpace(Function.ToString(dataTable.Rows[num]["AttrValue"])))
				{
					dataTable.Rows.RemoveAt(num);
				}
			}
			dataTable.AcceptChanges();
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "AttrCodeSetup");
			return await http.PostAsync("Save", dataTable);
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
		bool enabled = (cbAttr.Enabled = isEnable);
		simpleButtonEx.Enabled = enabled;
		gvAttrCode.ReadOnly = !isEnable;
	}

	private async void frm_AttrCodeSetup_Load(object sender, EventArgs e)
	{
		await LoadCb();
		await LoadData(Function.ParseInt(cbAttr.EditValue));
	}

	private void gvAttrCode_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
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
				WinCommons._OpenProcessing();
				string text = await Save();
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					await LoadData(Function.ParseInt(cbAttr.EditValue));
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

	private void rep_BinLoc_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gvAttrCode.GetFocusedRowCellValue("NoOfBin")) > 0)
			{
				frm_BinLocList frm_BinLocList2 = new frm_BinLocList();
				int rowHandle = gvAttrCode.GetSelectedRows()[0];
				DataRow dataRow = gvAttrCode.GetDataRow(rowHandle);
				frm_BinLocList2.AttrVal = Function.ToString(dataRow["AttrValue"]);
				frm_BinLocList2.FldAbs = Function.ToString(cbAttr.EditValue);
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

	private async void cbAttr_Properties_EditValueChanged(object sender, EventArgs e)
	{
		await LoadData(Function.ParseInt(cbAttr.EditValue));
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdAttrCode.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			int focusedRowHandle = gvAttrCode.FocusedRowHandle;
			string attr = Function.ToString(gvAttrCode.GetFocusedRowCellValue("AttrValue"));
			if (!CheckExistInBinLoc(Function.ParseInt(cbAttr.EditValue), attr))
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

	private void cbAttr_Properties_EditValueChanging(object sender, ChangingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
		{
			e.Cancel = true;
		}
	}

	private void gvAttrCode_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			DataTable dataTable = grdAttrCode.DataSource as DataTable;
			if (dataTable.IsNotNull() && e.Column == colCode && e.RowHandle == dataTable.Rows.Count - 1)
			{
				AddNewRow();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void gvAttrCode_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		if (e.Column == colNoOfBinLocation)
		{
			if (!string.IsNullOrEmpty(Function.ToString(gvAttrCode.GetRowCellValue(e.RowHandle, "AbsEntry"))))
			{
				e.RepositoryItem = rep_BinLoc;
			}
			else
			{
				e.RepositoryItem = null;
			}
		}
	}

	private void gvAttrCode_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		if (sender is ColumnView columnView && columnView.FocusedColumn == colCode && !string.IsNullOrEmpty(Function.ToString(gvAttrCode.GetFocusedRowCellValue("AbsEntry"))))
		{
			string text = Function.ToString(columnView.ActiveEditor.OldEditValue);
			if (!text.Equals(Function.ToString(e.Value)) && !CheckExistInBinLoc(Function.ParseInt(cbAttr.ItemIndex + 1), text))
			{
				e.Valid = false;
			}
		}
	}

	private void gvAttrCode_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		WinCommons.Error(Enums.MessageCode.CannotDeleteWHSublevelCode, "");
		e.ExceptionMode = ExceptionMode.Ignore;
	}

	private async void frm_AttrCodeSetup_FormClosing(object sender, FormClosingEventArgs e)
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
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdAttrCode = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvAttrCode = new Apzon.Controls.Grid.GridViewEx();
		this.colCode = new Apzon.Controls.Grid.GridColumnEx();
		this.colNoOfBinLocation = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BinLoc = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.cbAttr = new DevExpress.XtraEditors.LookUpEdit();
		((System.ComponentModel.ISupportInitialize)this.grdAttrCode).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvAttrCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BinLoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbAttr.Properties).BeginInit();
		base.SuspendLayout();
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(93, 321);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 10;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.EditControl = null;
		this.btnOk.EditValue = "OK";
		this.btnOk.FieldName = null;
		this.btnOk.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btnOk.IsSearchControl = false;
		this.btnOk.Location = new System.Drawing.Point(12, 321);
		this.btnOk.Name = "btnOk";
		this.btnOk.ReadOnly = false;
		this.btnOk.SettingInVisible = false;
		this.btnOk.SettingReadonly = false;
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 9;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.grdAttrCode.AllowPasteExcel = true;
		this.grdAttrCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdAttrCode.ClipboardData = null;
		this.grdAttrCode.ContextMenuStrip = this.contextMenuStrip1;
		this.grdAttrCode.Location = new System.Drawing.Point(1, 31);
		this.grdAttrCode.MainView = this.gvAttrCode;
		this.grdAttrCode.Name = "grdAttrCode";
		this.grdAttrCode.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_BinLoc });
		this.grdAttrCode.Size = new System.Drawing.Size(655, 284);
		this.grdAttrCode.TabIndex = 6;
		this.grdAttrCode.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvAttrCode });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvAttrCode.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvAttrCode.ColumnKey = null;
		this.gvAttrCode.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.colCode, this.colNoOfBinLocation });
		this.gvAttrCode.DisableFormSetting = false;
		this.gvAttrCode.DisplayException = false;
		this.gvAttrCode.EditColumn = null;
		this.gvAttrCode.FieldCurrency = null;
		this.gvAttrCode.FieldRate = null;
		this.gvAttrCode.FormId = null;
		this.gvAttrCode.GridControl = this.grdAttrCode;
		this.gvAttrCode.IgnoreFilter = false;
		this.gvAttrCode.IndexEditRow = -1;
		this.gvAttrCode.IndicatorWidth = 40;
		this.gvAttrCode.IsApzTable = false;
		this.gvAttrCode.IsInsertNewRow = false;
		this.gvAttrCode.IsLoadingData = false;
		this.gvAttrCode.IsMappingContextAction = false;
		this.gvAttrCode.ItemId = null;
		this.gvAttrCode.LstColumnAllowBatch = null;
		this.gvAttrCode.LstColumnNotAllowBatch = null;
		this.gvAttrCode.LstColumnNotEditable = null;
		this.gvAttrCode.LstColumNotSetting = null;
		this.gvAttrCode.Name = "gvAttrCode";
		this.gvAttrCode.OptionsView.ShowGroupPanel = false;
		this.gvAttrCode.UseBatchMod = false;
		this.gvAttrCode.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvAttrCode_CustomDrawRowIndicator);
		this.gvAttrCode.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gvAttrCode_CustomRowCellEdit);
		this.gvAttrCode.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvAttrCode_CellValueChanged);
		this.gvAttrCode.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gvAttrCode_ValidatingEditor);
		this.gvAttrCode.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gvAttrCode_InvalidValueException);
		this.colCode.Caption = "Code";
		this.colCode.CurrencyDateField = null;
		this.colCode.CurrencyField = null;
		this.colCode.CurrencyFixed = null;
		this.colCode.CurrencyFixedRate = 0.0;
		this.colCode.CurrencyRateField = null;
		this.colCode.FieldName = "AttrValue";
		this.colCode.Name = "colCode";
		this.colCode.PrimaryKey = true;
		this.colCode.UnitField = null;
		this.colCode.Visible = true;
		this.colCode.VisibleIndex = 0;
		this.colCode.Width = 180;
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
		this.colNoOfBinLocation.PrimaryKey = false;
		this.colNoOfBinLocation.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.colNoOfBinLocation.UnitField = null;
		this.colNoOfBinLocation.Visible = true;
		this.colNoOfBinLocation.VisibleIndex = 1;
		this.colNoOfBinLocation.Width = 433;
		this.rep_BinLoc.Appearance.Options.UseTextOptions = true;
		this.rep_BinLoc.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.rep_BinLoc.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_BinLoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_BinLoc.Name = "rep_BinLoc";
		this.rep_BinLoc.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.rep_BinLoc.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_BinLoc_ButtonClick);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Attribute";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 5);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(135, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 11;
		this.labelLine1.Utext = "Attribute";
		this.cbAttr.Location = new System.Drawing.Point(154, 5);
		this.cbAttr.Name = "cbAttr";
		this.cbAttr.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbAttr.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Text", "Name4"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FldNum", "Name5", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("FldAbs", "Name6", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.cbAttr.Properties.DisplayMember = "Text";
		this.cbAttr.Properties.NullText = "";
		this.cbAttr.Properties.PopupSizeable = false;
		this.cbAttr.Properties.ShowHeader = false;
		this.cbAttr.Properties.ValueMember = "FldNum";
		this.cbAttr.Properties.EditValueChanged += new System.EventHandler(this.cbAttr_Properties_EditValueChanged);
		this.cbAttr.Properties.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.cbAttr_Properties_EditValueChanging);
		this.cbAttr.Size = new System.Drawing.Size(224, 20);
		this.cbAttr.TabIndex = 8;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(655, 356);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.grdAttrCode);
		base.Controls.Add(this.cbAttr);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AttrCodeSetup";
		this.Text = "Bin Location Attribute Codes - Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_AttrCodeSetup_FormClosing);
		base.Load += new System.EventHandler(this.frm_AttrCodeSetup_Load);
		((System.ComponentModel.ISupportInitialize)this.grdAttrCode).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvAttrCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BinLoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbAttr.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
