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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_WorkType : XtraFormEx
{
	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	public Func<Task> refreshType;

	private IContainer components;

	private GridControlEx grd_WorkType;

	private GridViewEx gv_WorkType;

	private GridColumnEx cl_Id;

	private GridColumnEx cl_Name;

	private GridColumnEx cl_Typ;

	private GridColumnEx cl_Salary;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private RepositoryItemLookUpEdit LkUp_Type;

	private RepositoryItemLookUpEdit LkUpSalary;

	private ContextMenuStrip contextMenuStrip_WorkType;

	private ToolStripMenuItem toolStripMenuItem_Remove;

	private ToolStripMenuItem addNewToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
			return;
		}
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

	public frm_WorkType()
	{
		InitializeComponent();
		FormId = "oWorkType";
		gv_WorkType.ItemId = "APZ_OHWT";
		gv_WorkType.FormId = "oWorkType";
		gv_WorkType.ShowingEditor += GvWorkTypeOnShowingEditor;
		OnLoadAsync += frmWorkType_Load;
	}

	private void GvWorkTypeOnShowingEditor(object sender, CancelEventArgs e)
	{
		if (gv_WorkType.FocusedColumn.FieldName == "Code")
		{
			e.Cancel = true;
		}
	}

	private async Task frmWorkType_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			SetDataType();
			SetDataSalary();
			await WinFuntions.LoadUdfColumns(gv_WorkType, "APZ_OHWT");
			await refreshWorkType();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				await SaveWorkType();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_WorkType.GetFocusedRowCellValue("Id"));
			if (num != 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveItem, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveWorkType(num);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void AddNewRow()
	{
		try
		{
			gv_WorkType.BeginInsertRow();
			DataTable dataSourceTable = grd_WorkType.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				if (!dataSourceTable.AsEnumerable().Any((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Name"]))))
				{
					DataRow dataRow = dataSourceTable.NewRow();
					dataRow["Id"] = 0;
					dataRow["Name"] = "";
					dataRow["Typ"] = 'W';
					dataRow["Salary"] = 'S';
					dataSourceTable.Rows.Add(dataRow);
					dataSourceTable.AcceptChanges();
					grd_WorkType.DataSource = dataSourceTable;
					grd_WorkType.RefreshDataSource();
				}
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHWT");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["Id"] = 0;
				dataRow2["Name"] = "";
				dataRow2["Typ"] = 'W';
				dataRow2["Salary"] = 'S';
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_WorkType.DataSource = tableStructure;
				grd_WorkType.RefreshDataSource();
			}
			gv_WorkType.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		finally
		{
			gv_WorkType.EndInsertRow();
		}
	}

	public async Task refreshWorkType()
	{
		try
		{
			GridControlEx gridControlEx = grd_WorkType;
			gridControlEx.DataSource = await GetWorkType();
			grd_WorkType.DataSourceTable.AcceptChanges();
			grd_WorkType.RefreshDataSource();
			AddNewRow();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataTable> GetWorkType()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "TimeSheeting");
			return await _help.GetAsync("GetWorkType");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public async Task SaveWorkType()
	{
		try
		{
			DataTable dataTable = grd_WorkType.DataSourceTable.Copy();
			if (dataTable.IsNotNull())
			{
				List<DataRow> list = (from t in dataTable.AsEnumerable()
					where string.IsNullOrEmpty(Function.ToString(t["Name"]))
					select t).ToList();
				if (list.Any())
				{
					for (int num = 0; num < list.Count; num++)
					{
						dataTable.Rows.Remove(list[num]);
					}
					dataTable.AcceptChanges();
				}
				if ((from t in dataTable.AsEnumerable()
					select Function.ToString(t["Name"])).Distinct().Count() != dataTable.Rows.Count)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
				}
				else
				{
					if (!dataTable.IsNotNull())
					{
						return;
					}
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["Name"]).Length > 254) != null)
					{
						WinCommons.Warning(Enums.MessageCode.TheLengthOfStringEnteredIsLargerThanSpecified, "");
						return;
					}
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow d) => string.IsNullOrEmpty(Function.ToString(d["Name"]))) != null)
					{
						WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
						return;
					}
					dataTable.AcceptChanges();
					using HttpClientHelper<string> _hel = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
					WinCommons._OpenProcessing();
					string text = _hel.Post("SaveWorkType", dataTable);
					WinCommons._CloseProcessing();
					if (string.IsNullOrEmpty(text))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						FormMode = Enums.FormViewModeType.View;
						await refreshWorkType();
						if (refreshType != null && !((XtraForm)refreshType.Target).IsDisposed)
						{
							refreshType();
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
				}
			}
			else
			{
				grd_WorkType.DataSource = dataTable.Clone();
				grd_WorkType.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void RemoveWorkType(int Id)
	{
		try
		{
			using HttpClientHelper<string> _helper = new HttpClientHelper<string>(Configuration.ApiServer, "TimeSheeting");
			string text = _helper.Post("RemoveWorkType", Id);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_WorkType.BeginUpdate();
				gv_WorkType.DeleteSelectedRows();
				gv_WorkType.EndUpdate();
				await refreshWorkType();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_WorkType_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_WorkType_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Name")
			{
				AddNewRow();
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SetDataType()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Work",
			Code = "W"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Leave",
			Code = "L"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Absent",
			Code = "A"
		});
		list.Add(new Common.Table_Code
		{
			Name = "OverTime",
			Code = "O"
		});
		LkUp_Type.DataSource = list;
	}

	public void SetDataSalary()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Salary",
			Code = "S"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Insurrance",
			Code = "I"
		});
		LkUpSalary.DataSource = list;
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_WorkType_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			await SaveWorkType();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_WorkType));
		this.grd_WorkType = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_WorkType = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_WorkType = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Id = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Typ = new Apzon.Controls.Grid.GridColumnEx();
		this.LkUp_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_Salary = new Apzon.Controls.Grid.GridColumnEx();
		this.LkUpSalary = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_WorkType).BeginInit();
		this.contextMenuStrip_WorkType.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_WorkType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.LkUp_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.LkUpSalary).BeginInit();
		base.SuspendLayout();
		this.grd_WorkType.AllowPasteExcel = true;
		this.grd_WorkType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_WorkType.ClipboardData = null;
		this.grd_WorkType.ContextMenuStrip = this.contextMenuStrip_WorkType;
		this.grd_WorkType.Location = new System.Drawing.Point(3, 3);
		this.grd_WorkType.MainView = this.gv_WorkType;
		this.grd_WorkType.Name = "grd_WorkType";
		this.grd_WorkType.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.LkUp_Type, this.LkUpSalary });
		this.grd_WorkType.Size = new System.Drawing.Size(734, 368);
		this.grd_WorkType.TabIndex = 73;
		this.grd_WorkType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_WorkType });
		this.contextMenuStrip_WorkType.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip_WorkType.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.toolStripMenuItem_Remove });
		this.contextMenuStrip_WorkType.Name = "contextMenuStrip_WorkType";
		this.contextMenuStrip_WorkType.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(123, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.gv_WorkType.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_WorkType.ColumnKey = null;
		this.gv_WorkType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.cl_Id, this.cl_Name, this.cl_Typ, this.cl_Salary });
		this.gv_WorkType.DisableFormSetting = false;
		this.gv_WorkType.DisplayException = false;
		this.gv_WorkType.EditColumn = null;
		this.gv_WorkType.FieldCurrency = null;
		this.gv_WorkType.FieldRate = null;
		this.gv_WorkType.FormId = "oWorkTypes";
		this.gv_WorkType.GridControl = this.grd_WorkType;
		this.gv_WorkType.IgnoreFilter = false;
		this.gv_WorkType.IndexEditRow = -1;
		this.gv_WorkType.IndicatorWidth = 30;
		this.gv_WorkType.IsApzTable = false;
		this.gv_WorkType.IsInsertNewRow = false;
		this.gv_WorkType.IsLoadingData = false;
		this.gv_WorkType.IsMappingContextAction = false;
		this.gv_WorkType.ItemId = "APZ_OHOT";
		this.gv_WorkType.LstColumnAllowBatch = null;
		this.gv_WorkType.LstColumnNotAllowBatch = null;
		this.gv_WorkType.LstColumnNotEditable = null;
		this.gv_WorkType.LstColumNotSetting = null;
		this.gv_WorkType.Name = "gv_WorkType";
		this.gv_WorkType.OptionsView.RowAutoHeight = true;
		this.gv_WorkType.OptionsView.ShowGroupPanel = false;
		this.gv_WorkType.UseBatchMod = false;
		this.gv_WorkType.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_WorkType_CustomDrawRowIndicator);
		this.gv_WorkType.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_WorkType_CellValueChanged);
		this.cl_Id.Caption = "Id";
		this.cl_Id.CurrencyDateField = null;
		this.cl_Id.CurrencyField = null;
		this.cl_Id.CurrencyFixed = null;
		this.cl_Id.CurrencyFixedRate = 0.0;
		this.cl_Id.CurrencyRateField = null;
		this.cl_Id.FieldName = "Id";
		this.cl_Id.Name = "cl_Id";
		this.cl_Id.OptionsColumn.ReadOnly = true;
		this.cl_Id.PrimaryKey = true;
		this.cl_Id.UnitField = null;
		this.cl_Id.Width = 44;
		this.cl_Name.Caption = "Name";
		this.cl_Name.CurrencyDateField = null;
		this.cl_Name.CurrencyField = null;
		this.cl_Name.CurrencyFixed = null;
		this.cl_Name.CurrencyFixedRate = 0.0;
		this.cl_Name.CurrencyRateField = null;
		this.cl_Name.FieldName = "Name";
		this.cl_Name.Name = "cl_Name";
		this.cl_Name.PrimaryKey = true;
		this.cl_Name.UnitField = null;
		this.cl_Name.Visible = true;
		this.cl_Name.VisibleIndex = 0;
		this.cl_Name.Width = 112;
		this.cl_Typ.Caption = "Type ";
		this.cl_Typ.ColumnEdit = this.LkUp_Type;
		this.cl_Typ.CurrencyDateField = null;
		this.cl_Typ.CurrencyField = null;
		this.cl_Typ.CurrencyFixed = null;
		this.cl_Typ.CurrencyFixedRate = 0.0;
		this.cl_Typ.CurrencyRateField = null;
		this.cl_Typ.FieldName = "Typ";
		this.cl_Typ.Name = "cl_Typ";
		this.cl_Typ.PrimaryKey = false;
		this.cl_Typ.UnitField = null;
		this.cl_Typ.Visible = true;
		this.cl_Typ.VisibleIndex = 1;
		this.cl_Typ.Width = 147;
		this.LkUp_Type.AutoHeight = false;
		this.LkUp_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.LkUp_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.LkUp_Type.DisplayMember = "Name";
		this.LkUp_Type.Name = "LkUp_Type";
		this.LkUp_Type.NullText = "";
		this.LkUp_Type.ValueMember = "Code";
		this.cl_Salary.Caption = "Salary";
		this.cl_Salary.ColumnEdit = this.LkUpSalary;
		this.cl_Salary.CurrencyDateField = null;
		this.cl_Salary.CurrencyField = null;
		this.cl_Salary.CurrencyFixed = null;
		this.cl_Salary.CurrencyFixedRate = 0.0;
		this.cl_Salary.CurrencyRateField = null;
		this.cl_Salary.FieldName = "Salary";
		this.cl_Salary.Name = "cl_Salary";
		this.cl_Salary.PrimaryKey = false;
		this.cl_Salary.UnitField = null;
		this.cl_Salary.Visible = true;
		this.cl_Salary.VisibleIndex = 2;
		this.cl_Salary.Width = 179;
		this.LkUpSalary.AutoHeight = false;
		this.LkUpSalary.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.LkUpSalary.DisplayMember = "Name";
		this.LkUpSalary.Name = "LkUpSalary";
		this.LkUpSalary.NullText = "";
		this.LkUpSalary.ValueMember = "Code";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 377);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 76;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(70, 377);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 75;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(733, 405);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_WorkType);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WorkType";
		this.Text = "Work Type";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_WorkType_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_WorkType).EndInit();
		this.contextMenuStrip_WorkType.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_WorkType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.LkUp_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.LkUpSalary).EndInit();
		base.ResumeLayout(false);
	}
}
