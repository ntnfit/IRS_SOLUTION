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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_QualificationsGroup : XtraFormEx
{
	private const string FormController = "HRMQualifications";

	public RefreshLookupQuaGroup RefreshQuaGroup;

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private IContainer components;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridControlEx grd_QuaGroup;

	private GridViewEx gv_QuaGroup;

	private GridColumnEx cl_Id;

	private GridColumnEx cl_name;

	private GridColumnEx cl_Remark;

	private ContextMenuStrip contextMenuStrip_QuaGroup;

	private ToolStripMenuItem toolStripMenuItem_Remove;

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

	public frm_QualificationsGroup()
	{
		InitializeComponent();
		FormId = "oQualificationsGroup";
		gv_QuaGroup.FormId = FormId;
		gv_QuaGroup.ItemId = "APZ_OHQG";
		OnLoadAsync += frm_QualificationsGroup_Load;
	}

	private async Task frm_QualificationsGroup_Load(object sender, EventArgs e)
	{
		try
		{
			await RefreshQuaGroups();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			WinCommons._OpenProcessing();
			btn_Save.Enabled = false;
			await SaveQuaGroups();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Save.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_QuaGroup_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_QuaGroup_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < gv_QuaGroup.RowCount; i++)
			{
				num = gv_QuaGroup.GetVisibleRowHandle(i);
			}
			string text = Function.ToString(gv_QuaGroup.GetRowCellValue(e.RowHandle, "Name"));
			if (e.Column.FieldName == "Name" && text != "" && e.RowHandle == num)
			{
				AddNewRowQuaGroups();
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
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
			int num = Function.ParseInt(gv_QuaGroup.GetFocusedRowCellValue("Id"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				RemoveQuaGroups(num);
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_QuaGroup_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await RefreshQuaGroups();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveQuaGroups()
	{
		_ = 1;
		try
		{
			DataTable dataTable = (grd_QuaGroup.DataSource as DataTable).Copy();
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["Id"]) == 0);
				if (string.IsNullOrEmpty(Function.ToString(dataRow["name"])) && string.IsNullOrEmpty(Function.ToString(dataRow["Remark"])))
				{
					dataTable.Rows.Remove(dataRow);
					dataTable.AcceptChanges();
				}
				if ((from t in dataTable.AsEnumerable()
					select Function.ToString(t["name"])).Distinct().Count() != dataTable.Rows.Count)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
				}
				else
				{
					if (!dataTable.IsNotNull())
					{
						return;
					}
					dataTable = dataTable.Copy();
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Name"])) && string.IsNullOrEmpty(Function.ToString(t["Remark"]))) != null)
					{
						dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
					}
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Name"]))) != null)
					{
						WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
						return;
					}
					dataTable.AcceptChanges();
					using HttpClientHelper<HttpResult> _dtHelp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "HRMQualifications");
					HttpResult httpResult = await _dtHelp.PostAsync("SaveQualificationsGroup", dataTable);
					if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						if (RefreshQuaGroup != null)
						{
							RefreshQuaGroup();
						}
						FormMode = Enums.FormViewModeType.View;
						await RefreshQuaGroups();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
			else
			{
				grd_QuaGroup.DataSource = dataTable.Clone();
				grd_QuaGroup.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RemoveQuaGroups(int ID)
	{
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "HRMQualifications");
			string text = hp.Post("RemoveQualificationsGroup", ID);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_QuaGroup.BeginUpdate();
				gv_QuaGroup.DeleteSelectedRows();
				gv_QuaGroup.EndUpdate();
				if (RefreshQuaGroup != null)
				{
					RefreshQuaGroup();
				}
				await RefreshQuaGroups();
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

	private async Task RefreshQuaGroups()
	{
		try
		{
			GridControlEx gridControlEx = grd_QuaGroup;
			gridControlEx.DataSource = await GetNewQuaGroups();
			grd_QuaGroup.DataSourceTable.AcceptChanges();
			grd_QuaGroup.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			AddNewRowQuaGroups();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetNewQuaGroups()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "HRMQualifications");
			return await helper.GetAsync("GetQualificationsGroup");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void AddNewRowQuaGroups()
	{
		try
		{
			gv_QuaGroup.BeginInsertRow();
			DataTable dataTable = grd_QuaGroup.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Id"] = 0;
				dataRow["Name"] = "";
				dataRow["Remark"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_QuaGroup.DataSource = dataTable;
				grd_QuaGroup.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHQG");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["Id"] = 0;
				dataRow2["Name"] = "";
				dataRow2["Remark"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_QuaGroup.DataSource = tableStructure;
				grd_QuaGroup.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_QuaGroup.EndInsertRow();
		}
	}

	private async void frm_QualificationsGroup_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			WinCommons._OpenProcessing();
			await SaveQuaGroups();
			WinCommons._CloseProcessing();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_QualificationsGroup));
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_QuaGroup = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_QuaGroup = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_QuaGroup = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Id = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Remark = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.grd_QuaGroup).BeginInit();
		this.contextMenuStrip_QuaGroup.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_QuaGroup).BeginInit();
		base.SuspendLayout();
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 334);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 82;
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
		this.btnCancel.Location = new System.Drawing.Point(71, 334);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 81;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grd_QuaGroup.AllowPasteExcel = true;
		this.grd_QuaGroup.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_QuaGroup.ClipboardData = null;
		this.grd_QuaGroup.ContextMenuStrip = this.contextMenuStrip_QuaGroup;
		this.grd_QuaGroup.Location = new System.Drawing.Point(4, 5);
		this.grd_QuaGroup.MainView = this.gv_QuaGroup;
		this.grd_QuaGroup.Name = "grd_QuaGroup";
		this.grd_QuaGroup.Size = new System.Drawing.Size(562, 323);
		this.grd_QuaGroup.TabIndex = 80;
		this.grd_QuaGroup.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_QuaGroup });
		this.contextMenuStrip_QuaGroup.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip_QuaGroup.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem_Remove });
		this.contextMenuStrip_QuaGroup.Name = "contextMenuStrip_QuaGroup";
		this.contextMenuStrip_QuaGroup.Size = new System.Drawing.Size(118, 26);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.gv_QuaGroup.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_QuaGroup.ColumnKey = null;
		this.gv_QuaGroup.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.cl_Id, this.cl_name, this.cl_Remark });
		this.gv_QuaGroup.DisableFormSetting = false;
		this.gv_QuaGroup.DisplayException = false;
		this.gv_QuaGroup.EditColumn = null;
		this.gv_QuaGroup.FieldCurrency = null;
		this.gv_QuaGroup.FieldRate = null;
		this.gv_QuaGroup.FormId = "oQualificationsGroup";
		this.gv_QuaGroup.GridControl = this.grd_QuaGroup;
		this.gv_QuaGroup.IgnoreFilter = false;
		this.gv_QuaGroup.IndexEditRow = -1;
		this.gv_QuaGroup.IndicatorWidth = 30;
		this.gv_QuaGroup.IsApzTable = false;
		this.gv_QuaGroup.IsInsertNewRow = false;
		this.gv_QuaGroup.IsLoadingData = false;
		this.gv_QuaGroup.IsMappingContextAction = false;
		this.gv_QuaGroup.ItemId = "APZ_OHQG";
		this.gv_QuaGroup.LstColumnAllowBatch = null;
		this.gv_QuaGroup.LstColumnNotAllowBatch = null;
		this.gv_QuaGroup.LstColumnNotEditable = null;
		this.gv_QuaGroup.LstColumNotSetting = null;
		this.gv_QuaGroup.Name = "gv_QuaGroup";
		this.gv_QuaGroup.OptionsView.RowAutoHeight = true;
		this.gv_QuaGroup.OptionsView.ShowGroupPanel = false;
		this.gv_QuaGroup.UseBatchMod = false;
		this.gv_QuaGroup.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_QuaGroup_CustomDrawRowIndicator);
		this.gv_QuaGroup.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_QuaGroup_CellValueChanged);
		this.gv_QuaGroup.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_QuaGroup_CellValueChanging);
		this.cl_Id.Caption = "ID";
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
		this.cl_name.Caption = "Name";
		this.cl_name.CurrencyDateField = null;
		this.cl_name.CurrencyField = null;
		this.cl_name.CurrencyFixed = null;
		this.cl_name.CurrencyFixedRate = 0.0;
		this.cl_name.CurrencyRateField = null;
		this.cl_name.FieldName = "Name";
		this.cl_name.Name = "cl_name";
		this.cl_name.PrimaryKey = true;
		this.cl_name.UnitField = null;
		this.cl_name.Visible = true;
		this.cl_name.VisibleIndex = 0;
		this.cl_name.Width = 384;
		this.cl_Remark.Caption = "Description";
		this.cl_Remark.CurrencyDateField = null;
		this.cl_Remark.CurrencyField = null;
		this.cl_Remark.CurrencyFixed = null;
		this.cl_Remark.CurrencyFixedRate = 0.0;
		this.cl_Remark.CurrencyRateField = null;
		this.cl_Remark.FieldName = "Remark";
		this.cl_Remark.Name = "cl_Remark";
		this.cl_Remark.PrimaryKey = false;
		this.cl_Remark.UnitField = null;
		this.cl_Remark.Visible = true;
		this.cl_Remark.VisibleIndex = 1;
		this.cl_Remark.Width = 288;
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(571, 358);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_QuaGroup);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_QualificationsGroup";
		this.Text = "Qualifications Group";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_QualificationsGroup_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_QuaGroup).EndInit();
		this.contextMenuStrip_QuaGroup.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_QuaGroup).EndInit();
		base.ResumeLayout(false);
	}
}
