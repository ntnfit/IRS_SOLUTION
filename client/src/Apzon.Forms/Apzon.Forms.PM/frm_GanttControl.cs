using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraGantt;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Apzon.Forms.PM;

public class frm_GanttControl : XtraFormEx
{
	private IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GanttControl ganttControl1;

	private TreeListColumnEx treeListColumn1;

	private TreeListColumnEx treeListColumn2;

	private TreeListColumnEx tc_Recontact;

	private TreeListColumnEx tc_endDate;

	private TreeListColumnEx treeListColumn5;

	private SimpleButton btn_OK;

	private SimpleButton btn_Cancel;

	public int ProjectAbsEntry { get; set; }

	public frm_GanttControl()
	{
		InitializeComponent();
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = ((value == 0) ? Enums.FormViewModeType.View : value);
			switch (_formMode)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButton simpleButton4 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButton4.Enabled = enabled;
				btn_OK.Text = Resources.ButtonAdd;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButton simpleButton3 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButton3.Enabled = enabled;
				btn_OK.Text = Resources.ButtonUpdate;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButton simpleButton2 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButton2.Enabled = enabled;
				btn_OK.Text = Resources.ButtonOK;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButton simpleButton = btn_OK;
				bool enabled = (btn_Cancel.Enabled = false);
				simpleButton.Enabled = enabled;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_GanttControl_Load(object sender, EventArgs e)
	{
		try
		{
			tc_Recontact.ColumnEdit = new RepositoryItemDateEditEx();
			tc_endDate.ColumnEdit = new RepositoryItemDateEditEx();
			ganttControl1.TreeListMappings.KeyFieldName = "ClgCode";
			ganttControl1.TreeListMappings.ParentFieldName = "ParenparentTasktID";
			ganttControl1.ChartMappings.TextFieldName = "Details";
			ganttControl1.ChartMappings.StartDateFieldName = "Recontact";
			ganttControl1.ChartMappings.FinishDateFieldName = "endDate";
			ganttControl1.ChartMappings.PredecessorsFieldName = "Predecessors";
			if (ProjectAbsEntry >= 0)
			{
				DataTable dataSource = await httpClient.GanttControl.LoadByProject(ProjectAbsEntry);
				ganttControl1.DataSource = dataSource;
				ganttControl1.RefreshDataSource();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ganttControl1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currMode = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (currMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					FormMode = currMode;
					return;
				}
				DataTable dt = ganttControl1.DataSource as DataTable;
				HttpResult httpResult = await httpClient.GanttControl.UpdateTask(dt);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.Error, httpResult.Message);
					FormMode = currMode;
				}
				else
				{
					WinCommons.Success(Enums.MessageCode.Success, Resources.Msg_Success);
					FormMode = Enums.FormViewModeType.View;
				}
				return;
			}
			DialogResult = DialogResult.OK;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = currMode;
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.PM.frm_GanttControl));
		this.ganttControl1 = new DevExpress.XtraGantt.GanttControl();
		this.treeListColumn1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn2 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_Recontact = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_endDate = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn5 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_OK = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.ganttControl1).BeginInit();
		base.SuspendLayout();
		this.ganttControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ganttControl1.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[5] { this.treeListColumn1, this.treeListColumn2, this.tc_Recontact, this.tc_endDate, this.treeListColumn5 });
		this.ganttControl1.Cursor = System.Windows.Forms.Cursors.SizeWE;
		this.ganttControl1.Location = new System.Drawing.Point(0, 0);
		this.ganttControl1.Name = "ganttControl1";
		this.ganttControl1.OptionsCustomization.AllowModifyDependencies = DevExpress.Utils.DefaultBoolean.True;
		this.ganttControl1.OptionsCustomization.AllowModifyTasks = DevExpress.Utils.DefaultBoolean.True;
		this.ganttControl1.OptionsView.AutoWidth = false;
		this.ganttControl1.Size = new System.Drawing.Size(971, 555);
		this.ganttControl1.SplitterPosition = 550;
		this.ganttControl1.TabIndex = 0;
		this.ganttControl1.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.ganttControl1_CellValueChanged);
		this.treeListColumn1.Caption = "#";
		this.treeListColumn1.CurrencyDateField = null;
		this.treeListColumn1.CurrencyField = null;
		this.treeListColumn1.CurrencyFixed = null;
		this.treeListColumn1.CurrencyFixedRate = 0.0;
		this.treeListColumn1.CurrencyRateField = null;
		this.treeListColumn1.FieldName = "ClgCode";
		this.treeListColumn1.Name = "treeListColumn1";
		this.treeListColumn1.Visible = true;
		this.treeListColumn1.VisibleIndex = 0;
		this.treeListColumn1.Width = 45;
		this.treeListColumn2.Caption = "Details";
		this.treeListColumn2.CurrencyDateField = null;
		this.treeListColumn2.CurrencyField = null;
		this.treeListColumn2.CurrencyFixed = null;
		this.treeListColumn2.CurrencyFixedRate = 0.0;
		this.treeListColumn2.CurrencyRateField = null;
		this.treeListColumn2.FieldName = "Details";
		this.treeListColumn2.Name = "treeListColumn2";
		this.treeListColumn2.Visible = true;
		this.treeListColumn2.VisibleIndex = 1;
		this.treeListColumn2.Width = 200;
		this.tc_Recontact.Caption = "Start Date";
		this.tc_Recontact.CurrencyDateField = null;
		this.tc_Recontact.CurrencyField = null;
		this.tc_Recontact.CurrencyFixed = null;
		this.tc_Recontact.CurrencyFixedRate = 0.0;
		this.tc_Recontact.CurrencyRateField = null;
		this.tc_Recontact.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.tc_Recontact.FieldName = "Recontact";
		this.tc_Recontact.Name = "tc_Recontact";
		this.tc_Recontact.Visible = true;
		this.tc_Recontact.VisibleIndex = 2;
		this.tc_Recontact.Width = 100;
		this.tc_endDate.Caption = "End Date";
		this.tc_endDate.CurrencyDateField = null;
		this.tc_endDate.CurrencyField = null;
		this.tc_endDate.CurrencyFixed = null;
		this.tc_endDate.CurrencyFixedRate = 0.0;
		this.tc_endDate.CurrencyRateField = null;
		this.tc_endDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.tc_endDate.FieldName = "endDate";
		this.tc_endDate.Name = "tc_endDate";
		this.tc_endDate.Visible = true;
		this.tc_endDate.VisibleIndex = 3;
		this.tc_endDate.Width = 100;
		this.treeListColumn5.Caption = "Predecessors";
		this.treeListColumn5.CurrencyDateField = null;
		this.treeListColumn5.CurrencyField = null;
		this.treeListColumn5.CurrencyFixed = null;
		this.treeListColumn5.CurrencyFixedRate = 0.0;
		this.treeListColumn5.CurrencyRateField = null;
		this.treeListColumn5.FieldName = "Predecessors";
		this.treeListColumn5.Name = "treeListColumn5";
		this.treeListColumn5.Visible = true;
		this.treeListColumn5.VisibleIndex = 4;
		this.treeListColumn5.Width = 100;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.Location = new System.Drawing.Point(6, 561);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(87, 561);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(971, 591);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.ganttControl1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_GanttControl.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_GanttControl";
		base.ShowInTaskbar = false;
		this.Text = "Gantt view";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_GanttControl_Load);
		((System.ComponentModel.ISupportInitialize)this.ganttControl1).EndInit();
		base.ResumeLayout(false);
	}
}
