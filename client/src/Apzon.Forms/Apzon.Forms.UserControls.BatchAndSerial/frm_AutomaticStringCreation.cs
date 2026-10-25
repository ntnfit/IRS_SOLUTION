using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.UserControls.BatchAndSerial;

public class frm_AutomaticStringCreation : XtraFormEx
{
	public new DataTable DataSource;

	public string FinalString = "";

	private IContainer components;

	private GridControlEx grdCreation;

	private GridViewEx gvCreation;

	private LabelLine lbFinalString;

	private TextEditEx txtFinalString;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnOk;

	private GridColumnEx gc_String;

	private GridColumnEx gc_Type;

	private GridColumnEx gc_Operation;

	private RepositoryItemLookUpEdit rType;

	private RepositoryItemLookUpEdit rOperation;

	public frm_AutomaticStringCreation()
	{
		InitializeComponent();
	}

	public frm_AutomaticStringCreation(DataTable dt)
	{
		InitializeComponent();
		DataSource = dt;
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}

	private void btnOk_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.OK;
		DataSource = grdCreation.DataSourceTable;
		FinalString = txtFinalString.Text;
		Close();
	}

	private void frm_AutomaticStringCreation_Load(object sender, EventArgs e)
	{
		try
		{
			LoadDataType();
			LoadDataOperation();
			if (DataSource == null || DataSource.Columns.Count == 0)
			{
				DataSource = new DataTable();
				DataSource.Columns.Add("String", typeof(string));
				DataSource.Columns.Add("Type", typeof(string));
				DataSource.Columns.Add("Operation", typeof(string));
			}
			grdCreation.DataSource = DataSource;
			grdCreation.DataSourceTable.AcceptChanges();
			string text = "";
			for (int i = 0; i < DataSource.Rows.Count; i++)
			{
				text += Function.ToString(DataSource.Rows[i][0]);
			}
			txtFinalString.EditValue = text;
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvCreation_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals(gc_String.FieldName))
			{
				string text = "";
				for (int i = 0; i < DataSource.Rows.Count; i++)
				{
					text += Function.ToString(DataSource.Rows[i][0]);
				}
				txtFinalString.EditValue = text;
				AddNewRow();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvCreation_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (gvCreation.GetFocusedRowCellValue(gc_Type).Equals("2") && !Function.IsInt(Function.ToString(gvCreation.GetFocusedRowCellValue(gc_String))))
			{
				WinCommons.Error(Enums.MessageCode.InvalidNumericValue, "");
				e.Allow = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvCreation_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gvCreation.FocusedColumn.FieldName.Equals(gc_Operation.FieldName) && Function.ToString(gvCreation.GetFocusedRowCellValue(gc_Type)).Equals("1"))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadDataType()
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			list.Add(new Common.Table_Code
			{
				Name = "String",
				Code = "1"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Number",
				Code = "2"
			});
			rType.DataSource = list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadDataOperation()
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			list.Add(new Common.Table_Code
			{
				Name = "No Oper.",
				Code = "1"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Increase",
				Code = "2"
			});
			list.Add(new Common.Table_Code
			{
				Name = "Decrease",
				Code = "3"
			});
			rOperation.DataSource = list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRow()
	{
		try
		{
			grdCreation.DataSourceTable.AcceptChanges();
			DataTable dataSourceTable = grdCreation.DataSourceTable;
			DataRow[] array = dataSourceTable.Select("isnull(String,'') = ''");
			if (array.Length == 1)
			{
				return;
			}
			if (array.Length > 1)
			{
				for (int i = 0; i < array.Length - 1; i++)
				{
					dataSourceTable.Rows.Remove(array[i]);
				}
				dataSourceTable.AcceptChanges();
				grdCreation.DataSource = dataSourceTable;
				grdCreation.DataSourceTable.AcceptChanges();
			}
			else
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow[1] = "1";
				dataRow[2] = "1";
				dataSourceTable.Rows.Add(dataRow);
				grdCreation.DataSource = dataSourceTable;
				grdCreation.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.BatchAndSerial.frm_AutomaticStringCreation));
		this.grdCreation = new Apzon.Controls.Grid.GridControlEx();
		this.gvCreation = new Apzon.Controls.Grid.GridViewEx();
		this.gc_String = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Type = new Apzon.Controls.Grid.GridColumnEx();
		this.rType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Operation = new Apzon.Controls.Grid.GridColumnEx();
		this.rOperation = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.lbFinalString = new Apzon.Controls.Controls.LabelLine();
		this.txtFinalString = new Apzon.Controls.Editor.TextEditEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdCreation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvCreation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOperation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFinalString.Properties).BeginInit();
		base.SuspendLayout();
		this.grdCreation.AllowPasteExcel = true;
		this.grdCreation.ClipboardData = null;
		this.grdCreation.Dock = System.Windows.Forms.DockStyle.Top;
		this.grdCreation.Location = new System.Drawing.Point(0, 0);
		this.grdCreation.MainView = this.gvCreation;
		this.grdCreation.Name = "grdCreation";
		this.grdCreation.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rType, this.rOperation });
		this.grdCreation.Size = new System.Drawing.Size(342, 219);
		this.grdCreation.TabIndex = 0;
		this.grdCreation.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvCreation });
		this.gvCreation.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvCreation.ColumnKey = null;
		this.gvCreation.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gc_String, this.gc_Type, this.gc_Operation });
		this.gvCreation.DisableFormSetting = false;
		this.gvCreation.DisplayException = false;
		this.gvCreation.EditColumn = null;
		this.gvCreation.FieldCurrency = null;
		this.gvCreation.FieldRate = null;
		this.gvCreation.FormId = "oAutoStringCreation";
		this.gvCreation.GridControl = this.grdCreation;
		this.gvCreation.IgnoreFilter = false;
		this.gvCreation.IndexEditRow = -1;
		this.gvCreation.IndicatorWidth = 35;
		this.gvCreation.IsApzTable = false;
		this.gvCreation.IsInsertNewRow = false;
		this.gvCreation.IsLoadingData = false;
		this.gvCreation.IsMappingContextAction = false;
		this.gvCreation.ItemId = "ODATA";
		this.gvCreation.LstColumnAllowBatch = null;
		this.gvCreation.LstColumnNotAllowBatch = null;
		this.gvCreation.LstColumnNotEditable = null;
		this.gvCreation.LstColumNotSetting = null;
		this.gvCreation.Name = "gvCreation";
		this.gvCreation.OptionsView.BestFitMaxRowCount = 100;
		this.gvCreation.OptionsView.ColumnAutoWidth = false;
		this.gvCreation.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvCreation.OptionsView.ShowGroupPanel = false;
		this.gvCreation.UseBatchMod = false;
		this.gvCreation.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvCreation_ShowingEditor);
		this.gvCreation.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvCreation_CellValueChanged);
		this.gvCreation.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvCreation_BeforeLeaveRow);
		this.gc_String.Caption = "String";
		this.gc_String.CurrencyDateField = null;
		this.gc_String.CurrencyField = null;
		this.gc_String.CurrencyFixed = null;
		this.gc_String.CurrencyFixedRate = 0.0;
		this.gc_String.CurrencyRateField = null;
		this.gc_String.FieldName = "String";
		this.gc_String.Name = "gc_String";
		this.gc_String.PrimaryKey = true;
		this.gc_String.UnitField = null;
		this.gc_String.Visible = true;
		this.gc_String.VisibleIndex = 0;
		this.gc_String.Width = 121;
		this.gc_Type.Caption = "Type";
		this.gc_Type.ColumnEdit = this.rType;
		this.gc_Type.CurrencyDateField = null;
		this.gc_Type.CurrencyField = null;
		this.gc_Type.CurrencyFixed = null;
		this.gc_Type.CurrencyFixedRate = 0.0;
		this.gc_Type.CurrencyRateField = null;
		this.gc_Type.FieldName = "Type";
		this.gc_Type.Name = "gc_Type";
		this.gc_Type.PrimaryKey = false;
		this.gc_Type.UnitField = null;
		this.gc_Type.Visible = true;
		this.gc_Type.VisibleIndex = 1;
		this.gc_Type.Width = 87;
		this.rType.AutoHeight = false;
		this.rType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rType.DisplayMember = "Name";
		this.rType.Name = "rType";
		this.rType.NullText = "";
		this.rType.ShowHeader = false;
		this.rType.ValueMember = "Code";
		this.gc_Operation.Caption = "Operation";
		this.gc_Operation.ColumnEdit = this.rOperation;
		this.gc_Operation.CurrencyDateField = null;
		this.gc_Operation.CurrencyField = null;
		this.gc_Operation.CurrencyFixed = null;
		this.gc_Operation.CurrencyFixedRate = 0.0;
		this.gc_Operation.CurrencyRateField = null;
		this.gc_Operation.FieldName = "Operation";
		this.gc_Operation.Name = "gc_Operation";
		this.gc_Operation.PrimaryKey = false;
		this.gc_Operation.UnitField = null;
		this.gc_Operation.Visible = true;
		this.gc_Operation.VisibleIndex = 2;
		this.gc_Operation.Width = 94;
		this.rOperation.AutoHeight = false;
		this.rOperation.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rOperation.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name5"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name6", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rOperation.DisplayMember = "Name";
		this.rOperation.Name = "rOperation";
		this.rOperation.NullText = "";
		this.rOperation.ShowHeader = false;
		this.rOperation.ValueMember = "Code";
		this.lbFinalString.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbFinalString.EditControl = null;
		this.lbFinalString.EditValue = "Final String";
		this.lbFinalString.FieldName = null;
		this.lbFinalString.HideWhenEmpty = false;
		this.lbFinalString.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFinalString.Location = new System.Drawing.Point(17, 228);
		this.lbFinalString.Name = "lbFinalString";
		this.lbFinalString.ReadOnly = false;
		this.lbFinalString.SettingInVisible = false;
		this.lbFinalString.SettingReadonly = false;
		this.lbFinalString.ShowUnderLine = true;
		this.lbFinalString.Size = new System.Drawing.Size(80, 20);
		this.lbFinalString.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFinalString.TabIndex = 0;
		this.lbFinalString.Utext = "Final String";
		this.txtFinalString.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtFinalString.EditControl = null;
		this.txtFinalString.FieldName = null;
		this.txtFinalString.IsSearchControl = false;
		this.txtFinalString.Location = new System.Drawing.Point(97, 225);
		this.txtFinalString.Name = "txtFinalString";
		this.txtFinalString.Properties.ReadOnly = true;
		this.txtFinalString.SettingInVisible = false;
		this.txtFinalString.SettingReadonly = false;
		this.txtFinalString.Size = new System.Drawing.Size(169, 20);
		this.txtFinalString.TabIndex = 2;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(85, 258);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 3;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.EditControl = null;
		this.btnOk.EditValue = "Ok";
		this.btnOk.FieldName = null;
		this.btnOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOk.ImageOptions.Image");
		this.btnOk.IsSearchControl = false;
		this.btnOk.Location = new System.Drawing.Point(5, 258);
		this.btnOk.Name = "btnOk";
		this.btnOk.ReadOnly = false;
		this.btnOk.SettingInVisible = false;
		this.btnOk.SettingReadonly = false;
		this.btnOk.Size = new System.Drawing.Size(75, 20);
		this.btnOk.TabIndex = 4;
		this.btnOk.Text = "Ok";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(342, 283);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.txtFinalString);
		base.Controls.Add(this.lbFinalString);
		base.Controls.Add(this.grdCreation);
		base.FormId = "oAutoStringCreation";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AutomaticStringCreation";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Automatic String Creation";
		base.Load += new System.EventHandler(this.frm_AutomaticStringCreation_Load);
		((System.ComponentModel.ISupportInitialize)this.grdCreation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvCreation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOperation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFinalString.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
