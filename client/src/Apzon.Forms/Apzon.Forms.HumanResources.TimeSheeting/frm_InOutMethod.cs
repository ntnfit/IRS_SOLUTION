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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_InOutMethod : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SplitContainerControl splitContainerControl1;

	private LabelLine labelLine1;

	private TextEditEx txt_Description;

	private LabelLine labelLine2;

	private TextEditEx txt_Code;

	private LookUpEditEx luk_InOutMethod;

	private LabelLine labelLine3;

	private LabelLine labelLine5;

	private LabelLine labelLine4;

	private CalcEditEx calcEditEx2;

	private CalcEditEx calcEditEx1;

	private LabelLine labelLine6;

	private CalcEditEx calcEditEx3;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Ok;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
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
			break;
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Ok.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_InOutMethod()
	{
		InitializeComponent();
		FormId = "oInOutMethod";
		gv_Doc.FormId = FormId;
		gv_Doc.ItemId = "APZ_OIOM";
		OnLoadAsync += OnOnLoadAsync;
	}

	private async Task OnOnLoadAsync(object sender, EventArgs e)
	{
		try
		{
			LoadInitForm();
			await LoadData();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataSource = await _httpClient.InOutMethodClient.GetAll();
			grd_Doc.DataSource = dataSource;
			grd_Doc.DataSourceTable.AcceptChanges();
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private void LoadInitForm()
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "1",
					Name = "Alternative In Out"
				},
				new Common.Table_Code
				{
					Code = "3",
					Name = "In/Out by Machine number"
				},
				new Common.Table_Code
				{
					Code = "4",
					Name = "First In Last Out"
				},
				new Common.Table_Code
				{
					Code = "5",
					Name = "Check In/Out by Machine"
				}
			};
			luk_InOutMethod.SetDataSourceByList(list);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				if (string.IsNullOrEmpty(txt_Code.Text))
				{
					WinCommons.Error(Enums.MessageCode.CodeCannotBeEmpty, "");
					txt_Code.Focus();
				}
				else if (string.IsNullOrEmpty(txt_Description.Text))
				{
					WinCommons.Error(Enums.MessageCode.DescriptionCannotBeNull, "");
					txt_Description.Focus();
				}
				else
				{
					btn_Ok.Enabled = false;
					await SaveData();
					btn_Ok.Enabled = true;
				}
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async Task SaveData()
	{
		_ = 1;
		try
		{
			DataTable dataTable = DataSource as DataTable;
			DataSet dataSet = new DataSet();
			if (dataTable != null)
			{
				dataSet.Tables.Add(dataTable.Copy());
			}
			if (dataSet.Tables.Count > 0)
			{
				WinCommons._OpenProcessing();
				string text = await _httpClient.InOutMethodClient.Save(dataSet);
				WinCommons._CloseProcessing();
				if (string.IsNullOrWhiteSpace(text))
				{
					FormMode = Enums.FormViewModeType.View;
					await LoadData();
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewDoc();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			btn_Ok.Enabled = false;
			try
			{
				int focusedRowHandle = gv_Doc.FocusedRowHandle;
				int num = Function.ParseInt(gv_Doc.GetRowCellValue(focusedRowHandle, "MethodId"));
				if (num > 0)
				{
					string text = await _httpClient.InOutMethodClient.Remove(num);
					if (string.IsNullOrWhiteSpace(text))
					{
						gv_Doc.BeginUpdate();
						gv_Doc.DeleteSelectedRows();
						gv_Doc.EndUpdate();
						txt_Code.ResetText();
						txt_Description.ResetText();
						luk_InOutMethod.Text = "";
						calcEditEx1.Text = "";
						calcEditEx2.Text = "";
						calcEditEx3.Text = "";
						DataSource = WinFuntions.GetTableStructure("APZ_OIOM");
						WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
						FormMode = Enums.FormViewModeType.View;
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, Resources.Msg_DeleteError + ": " + text);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
			btn_Ok.Enabled = true;
		}
		catch (Exception ex2)
		{
			WinCommons.ErrorLineNumber(ex2);
			Logging.Write("ERROR", ex2);
		}
	}

	public async Task REFRESH()
	{
		AddNewDoc();
	}

	public async Task REFRESH_DATA()
	{
		await LoadData();
	}

	private void AddNewDoc()
	{
		_isLoading = true;
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			btn_Ok.Enabled = true;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OIOM");
			DataRow row = tableStructure.NewRow();
			tableStructure.Rows.Add(row);
			grd_Doc.DataSource = tableStructure;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	private void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			int focusId = Function.ParseInt(gv_Doc.GetRowCellValue(e.FocusedRowHandle, "MethodId"));
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["MethodId"]) == focusId);
				if (dataRow != null)
				{
					DataSource = dataRow.ToArray().CopyToDataTable();
				}
			}
			else
			{
				DataSource = WinFuntions.GetTableStructure("APZ_OIOM");
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private void frm_InOutMethod_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode != Enums.FormViewModeType.Add && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async void frm_InOutMethod_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			await SaveData();
			Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_InOutMethod));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.calcEditEx3 = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.calcEditEx2 = new Apzon.Controls.Editor.CalcEditEx();
		this.calcEditEx1 = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.luk_InOutMethod = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Description = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calcEditEx3.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcEditEx2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcEditEx1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_InOutMethod.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Description.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(226, 301);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.removeToolStripMenuItem });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(118, 48);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oInOutMethod";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OIOPM";
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
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gridColumnEx1.Caption = "ID";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "MethodId";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Code";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Code";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 0;
		this.gridColumnEx3.Caption = "Description";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Dscription";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.calcEditEx3);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine6);
		this.splitContainerControl1.Panel2.Controls.Add(this.calcEditEx2);
		this.splitContainerControl1.Panel2.Controls.Add(this.calcEditEx1);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine5);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine4);
		this.splitContainerControl1.Panel2.Controls.Add(this.luk_InOutMethod);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine3);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_Description);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_Code);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(595, 301);
		this.splitContainerControl1.SplitterPosition = 226;
		this.splitContainerControl1.TabIndex = 1;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.calcEditEx3.EditControl = null;
		this.calcEditEx3.FieldName = "BetweenInterval";
		this.calcEditEx3.IsSearchControl = false;
		this.calcEditEx3.Location = new System.Drawing.Point(185, 161);
		this.calcEditEx3.Name = "calcEditEx3";
		this.calcEditEx3.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcEditEx3.Properties.IsSearchControl = false;
		this.calcEditEx3.SettingInVisible = false;
		this.calcEditEx3.SettingReadonly = false;
		this.calcEditEx3.Size = new System.Drawing.Size(100, 20);
		this.calcEditEx3.TabIndex = 6;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Interval between IN - OUT pair allowed";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(4, 161);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(176, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 10;
		this.labelLine6.Utext = "Interval between IN - OUT pair allowed";
		this.calcEditEx2.EditControl = null;
		this.calcEditEx2.FieldName = "MaxInterval";
		this.calcEditEx2.IsSearchControl = false;
		this.calcEditEx2.Location = new System.Drawing.Point(185, 135);
		this.calcEditEx2.Name = "calcEditEx2";
		this.calcEditEx2.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcEditEx2.Properties.IsSearchControl = false;
		this.calcEditEx2.SettingInVisible = false;
		this.calcEditEx2.SettingReadonly = false;
		this.calcEditEx2.Size = new System.Drawing.Size(100, 20);
		this.calcEditEx2.TabIndex = 5;
		this.calcEditEx1.EditControl = null;
		this.calcEditEx1.FieldName = "MinInterval";
		this.calcEditEx1.IsSearchControl = false;
		this.calcEditEx1.Location = new System.Drawing.Point(185, 109);
		this.calcEditEx1.Name = "calcEditEx1";
		this.calcEditEx1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcEditEx1.Properties.IsSearchControl = false;
		this.calcEditEx1.SettingInVisible = false;
		this.calcEditEx1.SettingReadonly = false;
		this.calcEditEx1.Size = new System.Drawing.Size(100, 20);
		this.calcEditEx1.TabIndex = 4;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Max interval between IN - OUT";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(3, 135);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(176, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 7;
		this.labelLine5.Utext = "Max interval between IN - OUT";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Min interval between IN - OUT";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(4, 109);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(176, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 6;
		this.labelLine4.Utext = "Min interval between IN - OUT";
		this.luk_InOutMethod.EditControl = null;
		this.luk_InOutMethod.FieldName = "InOutMethod";
		this.luk_InOutMethod.IsSearchControl = false;
		this.luk_InOutMethod.Location = new System.Drawing.Point(103, 61);
		this.luk_InOutMethod.Name = "luk_InOutMethod";
		this.luk_InOutMethod.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_InOutMethod.Properties.NullText = "";
		this.luk_InOutMethod.SettingInVisible = false;
		this.luk_InOutMethod.SettingReadonly = false;
		this.luk_InOutMethod.Size = new System.Drawing.Size(250, 20);
		this.luk_InOutMethod.TabIndex = 3;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "In/Out Method";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(3, 64);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(102, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 4;
		this.labelLine3.Utext = "In/Out Method";
		this.txt_Description.EditControl = null;
		this.txt_Description.FieldName = "Dscription";
		this.txt_Description.IsSearchControl = false;
		this.txt_Description.Location = new System.Drawing.Point(103, 35);
		this.txt_Description.Name = "txt_Description";
		this.txt_Description.SettingInVisible = false;
		this.txt_Description.SettingReadonly = false;
		this.txt_Description.Size = new System.Drawing.Size(250, 20);
		this.txt_Description.TabIndex = 2;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Description";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(3, 38);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(102, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "Description";
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = "Code";
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(103, 9);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(250, 20);
		this.txt_Code.TabIndex = 1;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(3, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(102, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Code";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(81, 308);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 19;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(0, 308);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 20);
		this.btn_Ok.TabIndex = 18;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(595, 333);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oInOutMethod";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_InOutMethod";
		this.Text = "In/Out Pick Up Method";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_InOutMethod_OnControlValueChanged);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_InOutMethod_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.calcEditEx3.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcEditEx2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcEditEx1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_InOutMethod.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Description.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
