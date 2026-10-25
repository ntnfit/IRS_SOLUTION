using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_HRHealth : XtraFormEx
{
	public DataTable DtHealth;

	public ReloadHealth SendHealthBackToMain;

	public int EmpId;

	private bool _isCancel;

	private bool _forceClose;

	private IContainer components;

	private GridControlEx gridControl1;

	private GridViewEx gridView1;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemLookUpEdit luk_Kind;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private RepositoryItemDateEditEx rep_MedDate;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_HRHealth()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
			}
			FormId = "oHRHealth";
			gridView1.FormId = FormId;
			gridView1.ItemId = "APZ_OHEH";
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_HRHealth_Load(object sender, EventArgs e)
	{
		try
		{
			LoadControl();
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx> { ["APZ_OHEH"] = gridView1 };
			await WinFuntions.LoadUdfColumns(gridView1, "APZ_OHEH", tblFormular);
			LoadData();
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
			if (Function.ParseInt(gridView1.GetFocusedRowCellValue("healthID")) != GetNextId() - 1)
			{
				gridView1.BeginUpdate();
				gridView1.DeleteSelectedRows();
				gridView1.EndUpdate();
				gridControl1.DataSourceTable.AcceptChanges();
				gridControl1.RefreshDataSource();
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = gridControl1.DataSourceTable;
			if (Function.ParseInt(gridView1.GetRowCellValue(e.RowHandle, "healthID")) == GetNextId() - 1 && !string.IsNullOrEmpty(Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "kind"))) && !string.IsNullOrEmpty(Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "medicalDate"))))
			{
				gridView1.BeginInsertRow();
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["healthID"] = GetNextId();
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
				dataSourceTable.SetDefaultAndMaxLen(gridView1.ItemId);
				gridControl1.DataSource = dataSourceTable;
				gridControl1.RefreshDataSource();
				gridView1.EndInsertRow();
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gridView1.EndInsertRow();
	}

	private void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			btn_Save.Enabled = false;
			btnCancel.Enabled = false;
			SaveData();
			btn_Save.Enabled = true;
			btnCancel.Enabled = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			_isCancel = true;
			Close();
			_isCancel = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void frm_HRHealth_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_isCancel)
			{
				return;
			}
			if (!_forceClose)
			{
				e.Cancel = true;
			}
			if (_forceClose)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.No:
					_forceClose = true;
					Close();
					break;
				case DialogResult.Yes:
					if (string.IsNullOrEmpty(SaveData()))
					{
						_forceClose = true;
						Close();
					}
					break;
				}
			}
			else
			{
				_forceClose = true;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadControl()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "",
					Name = ""
				},
				new Common.Table_Code
				{
					Code = "1",
					Name = "1"
				},
				new Common.Table_Code
				{
					Code = "2",
					Name = "2"
				},
				new Common.Table_Code
				{
					Code = "3",
					Name = "3"
				},
				new Common.Table_Code
				{
					Code = "4",
					Name = "4"
				}
			};
			luk_Kind.DataSource = dataSource;
			luk_Kind.DisplayMember = "Name";
			luk_Kind.ValueMember = "Code";
			luk_Kind.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_Kind.PopulateColumns();
			luk_Kind.Columns["Code"].Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadData()
	{
		try
		{
			if (!DtHealth.IsNotNull())
			{
				DtHealth = WinFuntions.GetTableStructure("APZ_OHEH");
			}
			DataRow dataRow = DtHealth.NewRow();
			dataRow["empID"] = EmpId;
			if (DtHealth.Rows.Count > 0)
			{
				dataRow["healthID"] = DtHealth.AsEnumerable().Max((DataRow s) => Function.ParseInt(s.Field<int>("healthID"))) + 1;
			}
			else
			{
				dataRow["healthID"] = 1;
			}
			DtHealth.Rows.Add(dataRow);
			DtHealth.AcceptChanges();
			DataTable dataTable = DtHealth.Copy();
			dataTable.SetDefaultAndMaxLen(gridView1.ItemId);
			gridControl1.DataSource = dataTable;
			gridControl1.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int GetNextId()
	{
		try
		{
			DataTable dataSourceTable = gridControl1.DataSourceTable;
			dataSourceTable.SetDefaultAndMaxLen(gridView1.ItemId);
			dataSourceTable.AcceptChanges();
			return (!dataSourceTable.IsNotNull()) ? 1 : (dataSourceTable.AsEnumerable().Max((DataRow s) => Function.ParseInt(s.Field<int>("healthID"))) + 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private string SaveData()
	{
		try
		{
			DataTable dataSourceTable = gridControl1.DataSourceTable;
			DataRow dataRow = dataSourceTable.Rows[dataSourceTable.Rows.Count - 1];
			for (int i = 4; i < dataSourceTable.Columns.Count - 1; i++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dataRow[i])))
				{
					WinCommons.Error(Enums.MessageCode.DateOrKindFieldCannotBeNull, "");
					return "error";
				}
			}
			if (!string.IsNullOrEmpty(Function.ToString(dataRow["medicalDate"])) || !string.IsNullOrEmpty(Function.ToString(dataRow["kind"])))
			{
				WinCommons.Error(Enums.MessageCode.DateOrKindFieldCannotBeNull, "");
				return "error";
			}
			DataTable dataTable = dataSourceTable.Copy();
			dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
			dataTable.AcceptChanges();
			DataRow dataRow2 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["medicalDate"])));
			DataRow dataRow3 = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["kind"])));
			if (dataRow2 != null || dataRow3 != null)
			{
				WinCommons.Error(Enums.MessageCode.DateOrKindFieldCannotBeNull, "");
				return "error";
			}
			WinCommons._OpenProcessing();
			if (SendHealthBackToMain != null)
			{
				SendHealthBackToMain(dataTable.Copy());
				FormMode = Enums.FormViewModeType.View;
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
		return string.Empty;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_HRHealth));
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_MedDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Kind = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MedDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MedDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Kind).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		base.SuspendLayout();
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.gridControl1.Location = new System.Drawing.Point(1, 1);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.luk_Kind, this.repositoryItemMemoExEdit1, this.rep_MedDate });
		this.gridControl1.Size = new System.Drawing.Size(731, 359);
		this.gridControl1.TabIndex = 0;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = "healthID";
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7 });
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "oHRHealth";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 40;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = true;
		this.gridView1.IsLoadingData = false;
		this.gridView1.ItemId = "APZ_OHTH";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gridColumn1.Caption = "HealthStatus ID";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "healthID\r\n";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "Employee No";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "empID";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn3.Caption = "Medical Date";
		this.gridColumn3.ColumnEdit = this.rep_MedDate;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumn3.FieldName = "medicalDate";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 0;
		this.rep_MedDate.AutoHeight = false;
		this.rep_MedDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MedDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_MedDate.IsSearchControl = false;
		this.rep_MedDate.Name = "rep_MedDate";
		this.gridColumn4.Caption = "Kind";
		this.gridColumn4.ColumnEdit = this.luk_Kind;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "kind";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 1;
		this.luk_Kind.AutoHeight = false;
		this.luk_Kind.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Kind.Name = "luk_Kind";
		this.luk_Kind.NullText = "";
		this.gridColumn5.Caption = "Disease";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "disease";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 2;
		this.gridColumn6.Caption = "Disease Profile";
		this.gridColumn6.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "diseaseProfile";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 3;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gridColumn7.Caption = "Note";
		this.gridColumn7.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "note";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 4;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(2, 366);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 61;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(69, 366);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 60;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(732, 389);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.gridControl1);
		base.FormId = "oHRHealth";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_HRHealth";
		this.Text = "Health Status";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_HRHealth_FormClosing);
		base.Load += new System.EventHandler(this.frm_HRHealth_Load);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MedDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_MedDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Kind).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
