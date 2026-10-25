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

public class frm_AdvertisementSource : XtraFormEx
{
	private const string FormController = "HRMQualifications";

	protected Enums.FormAuthorities Authoritues = Enums.FormAuthorities.Read;

	private IContainer components;

	private GridControlEx grd_AdvSource;

	private GridViewEx gv_dvSource;

	private GridColumnEx cl_Id;

	private GridColumnEx cl_name;

	private GridColumnEx cl_Remark;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private ContextMenuStrip contextMenuStrip_AdvSource;

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

	public frm_AdvertisementSource()
	{
		InitializeComponent();
		FormId = "oAdvertisementSource";
		gv_dvSource.FormId = FormId;
		gv_dvSource.ItemId = "APZ_OHAS";
		OnLoadAsync += frm_AdvertisementSource_Load;
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
				await SaveAdvSource();
			}
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

	private async Task frm_AdvertisementSource_Load(object sender, EventArgs e)
	{
		try
		{
			await RefreshAdvSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await RefreshAdvSource();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveAdvSource()
	{
		_ = 1;
		try
		{
			DataTable dataTable = (grd_AdvSource.DataSource as DataTable).Copy();
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
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["Name"]))) != null)
					{
						WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
						return;
					}
					dataTable.AcceptChanges();
					using HttpClientHelper<HttpResult> _dtHelp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "HRMQualifications");
					WinCommons._OpenProcessing();
					HttpResult httpResult = await _dtHelp.PostAsync("SaveAdvSource", dataTable);
					WinCommons._CloseProcessing();
					if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						FormMode = Enums.FormViewModeType.View;
						await RefreshAdvSource();
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
			else
			{
				grd_AdvSource.DataSource = dataTable.Clone();
				grd_AdvSource.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RemoveAdvSource(int ID)
	{
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "HRMQualifications");
			string text = hp.Post("RemoveAdvSource", ID);
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_dvSource.BeginUpdate();
				gv_dvSource.DeleteSelectedRows();
				gv_dvSource.EndUpdate();
				await RefreshAdvSource();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task RefreshAdvSource()
	{
		try
		{
			GridControlEx gridControlEx = grd_AdvSource;
			gridControlEx.DataSource = await GetNewAdvSource();
			grd_AdvSource.DataSourceTable.AcceptChanges();
			grd_AdvSource.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
			AddNewRowAdvSource();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetNewAdvSource()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "HRMQualifications");
			return await helper.GetAsync("GetAdvSource");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void AddNewRowAdvSource()
	{
		try
		{
			gv_dvSource.BeginInsertRow();
			DataTable dataTable = grd_AdvSource.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Id"] = 0;
				dataRow["Name"] = "";
				dataRow["Remark"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_AdvSource.DataSource = dataTable;
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHAS");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["Id"] = 0;
				dataRow2["Name"] = "";
				dataRow2["Remark"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_AdvSource.DataSource = tableStructure;
				grd_AdvSource.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gv_dvSource.EndInsertRow();
		}
	}

	private async void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_dvSource.GetFocusedRowCellValue("Id"));
			if (num > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveAdvSource(num);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_dvSource_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < gv_dvSource.RowCount; i++)
			{
				num = gv_dvSource.GetVisibleRowHandle(i);
			}
			string text = Function.ToString(gv_dvSource.GetRowCellValue(e.RowHandle, "name"));
			if (e.Column.FieldName == "name" && text != "" && e.RowHandle == num)
			{
				AddNewRowAdvSource();
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

	private void gv_dvSource_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void frm_AdvertisementSource_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			await SaveAdvSource();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_AdvertisementSource));
		this.grd_AdvSource = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_AdvSource = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_dvSource = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Id = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_name = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_AdvSource).BeginInit();
		this.contextMenuStrip_AdvSource.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_dvSource).BeginInit();
		base.SuspendLayout();
		this.grd_AdvSource.AllowPasteExcel = true;
		this.grd_AdvSource.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_AdvSource.ClipboardData = null;
		this.grd_AdvSource.ContextMenuStrip = this.contextMenuStrip_AdvSource;
		this.grd_AdvSource.Location = new System.Drawing.Point(3, 2);
		this.grd_AdvSource.MainView = this.gv_dvSource;
		this.grd_AdvSource.Name = "grd_AdvSource";
		this.grd_AdvSource.Size = new System.Drawing.Size(581, 328);
		this.grd_AdvSource.TabIndex = 73;
		this.grd_AdvSource.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_dvSource });
		this.contextMenuStrip_AdvSource.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem_Remove });
		this.contextMenuStrip_AdvSource.Name = "contextMenuStrip_AdvSource";
		this.contextMenuStrip_AdvSource.Size = new System.Drawing.Size(118, 26);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.gv_dvSource.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_dvSource.ColumnKey = null;
		this.gv_dvSource.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.cl_Id, this.cl_name, this.cl_Remark });
		this.gv_dvSource.DisableFormSetting = false;
		this.gv_dvSource.DisplayException = false;
		this.gv_dvSource.EditColumn = null;
		this.gv_dvSource.FieldCurrency = null;
		this.gv_dvSource.FieldRate = null;
		this.gv_dvSource.FormId = "oAdvertisementSource";
		this.gv_dvSource.GridControl = this.grd_AdvSource;
		this.gv_dvSource.IgnoreFilter = false;
		this.gv_dvSource.IndexEditRow = -1;
		this.gv_dvSource.IndicatorWidth = 30;
		this.gv_dvSource.IsApzTable = false;
		this.gv_dvSource.IsInsertNewRow = false;
		this.gv_dvSource.IsLoadingData = false;
		this.gv_dvSource.IsMappingContextAction = false;
		this.gv_dvSource.ItemId = "APZ_OHAS";
		this.gv_dvSource.LstColumnAllowBatch = null;
		this.gv_dvSource.LstColumnNotAllowBatch = null;
		this.gv_dvSource.LstColumnNotEditable = null;
		this.gv_dvSource.LstColumNotSetting = null;
		this.gv_dvSource.Name = "gv_dvSource";
		this.gv_dvSource.OptionsView.RowAutoHeight = true;
		this.gv_dvSource.OptionsView.ShowGroupPanel = false;
		this.gv_dvSource.UseBatchMod = false;
		this.gv_dvSource.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_dvSource_CustomDrawRowIndicator);
		this.gv_dvSource.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_dvSource_CellValueChanged);
		this.cl_Id.Caption = "ID";
		this.cl_Id.CurrencyDateField = null;
		this.cl_Id.CurrencyField = null;
		this.cl_Id.CurrencyFixed = null;
		this.cl_Id.CurrencyFixedRate = 0.0;
		this.cl_Id.CurrencyRateField = null;
		this.cl_Id.FieldName = "Id";
		this.cl_Id.Name = "cl_Id";
		this.cl_Id.OptionsColumn.ReadOnly = true;
		this.cl_Id.PrimaryKey = false;
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
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 334);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 20);
		this.btn_Save.TabIndex = 76;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(82, 334);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 75;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(590, 357);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grd_AdvSource);
		base.FormId = "oAdvertisementSource";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AdvertisementSource";
		this.Text = "Advertisement Source";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_AdvertisementSource_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_AdvSource).EndInit();
		this.contextMenuStrip_AdvSource.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_dvSource).EndInit();
		base.ResumeLayout(false);
	}
}
