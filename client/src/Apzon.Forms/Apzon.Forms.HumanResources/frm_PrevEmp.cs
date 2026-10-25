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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Global;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_PrevEmp : XtraFormEx
{
	private const string FormController = "PreviousEmp";

	private bool isCancel;

	private bool _isLoading;

	private int lineMax;

	private DataTable dtPrevEmp;

	public Action<DataTable> SendFromChildDelegate;

	private IContainer components;

	private GridControlEx gridControl1;

	private GridViewEx gridView1;

	private GridColumnEx line;

	private GridColumnEx fromDate;

	private GridColumnEx toDate;

	private GridColumnEx employer;

	private GridColumnEx position;

	private GridColumnEx remarks;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private RepositoryItemDateEditEx dateRep;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btnOK;

	private GridColumnEx gridColumn1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_PrevEmp()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			FormId = "oPreviousEmployee";
			gridView1.FormId = FormId;
			gridView1.ItemId = "HEM4";
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public void GetFromParentMethod(DataTable dt)
	{
		try
		{
			_isLoading = true;
			if (dt.IsNotNull())
			{
				DataView defaultView = dt.DefaultView;
				defaultView.Sort = "fromDate asc";
				dt = defaultView.ToTable();
				lineMax = Function.ParseInt((from s in dt.AsEnumerable()
					where Function.ToString(s.Field<short?>("line")) != ""
					select s).Max((DataRow s) => s.Field<short?>("line")));
			}
			if (!dt.ExistsColumn("IsNewRow"))
			{
				dt.Columns.Add("IsNewRow", typeof(string));
			}
			foreach (DataRow row in dt.Rows)
			{
				row["IsNewRow"] = "N";
			}
			dtPrevEmp = dt.Copy();
			gridControl1.DataSource = dt;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				_isLoading = true;
				gridControl1.DataSource = dtPrevEmp.Copy();
				AddNewRow();
				_isLoading = false;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	private void frm_PrevEmp_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && !isCancel)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					btnOK_Click(null, null);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckSaveState()
	{
		if (!_isLoading)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void AddNewRow()
	{
		try
		{
			gridView1.BeginInsertRow();
			DataTable dataTable = gridControl1.DataSource as DataTable;
			DataRow dataRow = dataTable.NewRow();
			dataRow["line"] = 0;
			dataRow["fromDate"] = DBNull.Value;
			dataRow["toDate"] = DBNull.Value;
			dataRow["employer"] = "";
			dataRow["position"] = "";
			dataRow["remarks"] = "";
			dataRow["IsNewRow"] = "Y";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			gridControl1.DataSource = dataTable;
			gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gridView1.EndInsertRow();
		}
	}

	private void UpdateData()
	{
		try
		{
			gridView1.ClearSorting();
			string requiredField = "employer";
			DataTable dataTable = gridControl1.DataSource as DataTable;
			dataTable.AcceptChanges();
			if (dataTable.IsNotNull())
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (Function.ToString(item[requiredField]) == "")
					{
						if (!(Function.ToString(item["line"]) == "0"))
						{
							WinCommons.Error(Enums.MessageCode.EmpIdCannotBeEmpty, "");
							return;
						}
						if (Function.ToString(item["fromDate"]) != "" || Function.ToString(item["toDate"]) != "" || Function.ToString(item["position"]) != "" || Function.ToString(item["remarks"]) != "")
						{
							WinCommons.Error(Enums.MessageCode.EmpIdCannotBeEmpty, "");
							return;
						}
					}
					if (Function.ToString(item["fromDate"]) != "" && Function.ToString(item["toDate"]) != "" && Function.ParseDateTime(item["fromDate"]) > Function.ParseDateTime(item["toDate"]))
					{
						WinCommons.Error(Enums.MessageCode.FromDateMustBeBefore2Date, "");
						return;
					}
				}
			}
			DataRow[] array = (from s in dataTable.AsEnumerable()
				where Function.ToString(s.Field<string>(requiredField)) != ""
				select s).ToArray();
			if (array.Length == 0)
			{
				gridControl1.DataSource = dataTable.Clone();
				SendFromChildDelegate(dataTable.Clone());
			}
			else
			{
				dataTable = array.CopyToDataTable();
				dataTable.AcceptChanges();
				foreach (DataRow item2 in dataTable.AsEnumerable())
				{
					if (Function.ToString(item2["line"]) == "0" && Function.ToString(item2[requiredField]) != "")
					{
						item2["line"] = lineMax + 1;
						lineMax++;
					}
				}
				dataTable.AcceptChanges();
				dtPrevEmp = dataTable.Copy();
				gridControl1.DataSource = dataTable.Copy();
				if (dataTable.ExistsColumn("IsNewRow"))
				{
					dataTable.Columns.Remove("IsNewRow");
				}
				SendFromChildDelegate(dataTable.Copy());
			}
			FormMode = Enums.FormViewModeType.View;
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				UpdateData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_PrevEmp_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
			repositoryItemTextEdit.MaxLength = 50;
			gridView1.Columns["position"].ColumnEdit = repositoryItemTextEdit;
			RepositoryItemTextEdit repositoryItemTextEdit2 = new RepositoryItemTextEdit();
			repositoryItemTextEdit2.MaxLength = 50;
			gridView1.Columns["employer"].ColumnEdit = repositoryItemTextEdit2;
			gridView1.OptionsBehavior.KeepFocusedRowOnUpdate = true;
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx> { ["HEM4"] = gridView1 };
			await WinFuntions.LoadUdfColumns(gridView1, "HEM4", tblFormular);
			AddNewRow();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
		if (!_isLoading)
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			string text = Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "IsNewRow"));
			object rowCellValue = gridViewEx.GetRowCellValue(e.RowHandle, "employer");
			if (text == "Y" && e.Column.FieldName == "employer" && rowCellValue != null)
			{
				gridView1.SetRowCellValue(e.RowHandle, "IsNewRow", "N");
				AddNewRow();
			}
		}
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gridView1.BeginUpdate();
			gridView1.DeleteSelectedRows();
			gridView1.EndUpdate();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
	}

	private void gridControl1_EditorKeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			gridView1.PostEditor();
			UpdateData();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_PrevEmp));
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.line = new Apzon.Controls.Grid.GridColumnEx();
		this.fromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.dateRep = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.toDate = new Apzon.Controls.Grid.GridColumnEx();
		this.employer = new Apzon.Controls.Grid.GridColumnEx();
		this.position = new Apzon.Controls.Grid.GridColumnEx();
		this.remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateRep).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateRep.CalendarTimeProperties).BeginInit();
		base.SuspendLayout();
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.gridControl1.Location = new System.Drawing.Point(0, 0);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.dateRep });
		this.gridControl1.Size = new System.Drawing.Size(608, 344);
		this.gridControl1.TabIndex = 1;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.gridControl1.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.gridControl1_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.line, this.fromDate, this.toDate, this.employer, this.position, this.remarks, this.gridColumn1 });
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "oPreviousEmployee";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "HEM4";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
		this.line.Caption = "line";
		this.line.CurrencyDateField = null;
		this.line.CurrencyField = null;
		this.line.CurrencyFixed = null;
		this.line.CurrencyFixedRate = 0.0;
		this.line.CurrencyRateField = null;
		this.line.FieldName = "line";
		this.line.Name = "line";
		this.line.PrimaryKey = false;
		this.line.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
		this.line.UnitField = null;
		this.line.Width = 21;
		this.fromDate.Caption = "From Date";
		this.fromDate.ColumnEdit = this.dateRep;
		this.fromDate.CurrencyDateField = null;
		this.fromDate.CurrencyField = null;
		this.fromDate.CurrencyFixed = null;
		this.fromDate.CurrencyFixedRate = 0.0;
		this.fromDate.CurrencyRateField = null;
		this.fromDate.FieldName = "fromDate";
		this.fromDate.Name = "fromDate";
		this.fromDate.PrimaryKey = false;
		this.fromDate.UnitField = null;
		this.fromDate.Visible = true;
		this.fromDate.VisibleIndex = 0;
		this.fromDate.Width = 73;
		this.dateRep.AutoHeight = false;
		this.dateRep.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateRep.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateRep.IsSearchControl = false;
		this.dateRep.Name = "dateRep";
		this.toDate.Caption = "To Date";
		this.toDate.ColumnEdit = this.dateRep;
		this.toDate.CurrencyDateField = null;
		this.toDate.CurrencyField = null;
		this.toDate.CurrencyFixed = null;
		this.toDate.CurrencyFixedRate = 0.0;
		this.toDate.CurrencyRateField = null;
		this.toDate.FieldName = "toDate";
		this.toDate.Name = "toDate";
		this.toDate.PrimaryKey = false;
		this.toDate.UnitField = null;
		this.toDate.Visible = true;
		this.toDate.VisibleIndex = 1;
		this.toDate.Width = 69;
		this.employer.Caption = "Employer";
		this.employer.CurrencyDateField = null;
		this.employer.CurrencyField = null;
		this.employer.CurrencyFixed = null;
		this.employer.CurrencyFixedRate = 0.0;
		this.employer.CurrencyRateField = null;
		this.employer.FieldName = "employer";
		this.employer.Name = "employer";
		this.employer.PrimaryKey = false;
		this.employer.UnitField = null;
		this.employer.Visible = true;
		this.employer.VisibleIndex = 2;
		this.employer.Width = 97;
		this.position.Caption = "Position";
		this.position.CurrencyDateField = null;
		this.position.CurrencyField = null;
		this.position.CurrencyFixed = null;
		this.position.CurrencyFixedRate = 0.0;
		this.position.CurrencyRateField = null;
		this.position.FieldName = "position";
		this.position.Name = "position";
		this.position.PrimaryKey = false;
		this.position.UnitField = null;
		this.position.Visible = true;
		this.position.VisibleIndex = 3;
		this.position.Width = 106;
		this.remarks.Caption = "Remarks";
		this.remarks.CurrencyDateField = null;
		this.remarks.CurrencyField = null;
		this.remarks.CurrencyFixed = null;
		this.remarks.CurrencyFixedRate = 0.0;
		this.remarks.CurrencyRateField = null;
		this.remarks.FieldName = "remarks";
		this.remarks.Name = "remarks";
		this.remarks.PrimaryKey = false;
		this.remarks.UnitField = null;
		this.remarks.Visible = true;
		this.remarks.VisibleIndex = 4;
		this.remarks.Width = 111;
		this.gridColumn1.Caption = "gridColumn1";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "IsNewRow";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(83, 350);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 9;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(4, 350);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 8;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		base.AcceptButton = this.btnOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(608, 378);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.gridControl1);
		base.FormId = "oPreviousEmployee";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_PrevEmp";
		base.ShowInTaskbar = false;
		this.Text = "Previous Employment";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_PrevEmp_FormClosing);
		base.Load += new System.EventHandler(this.frm_PrevEmp_Load);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateRep.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateRep).EndInit();
		base.ResumeLayout(false);
	}
}
