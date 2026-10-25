using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.SalesOppotunity;

public class frm_ActivitiesListForOpportunity : XtraFormEx
{
	private string _privilege = "";

	public int _clgCode_ActivityListForOpportunity = -1;

	private DataTable dtObject = new DataTable();

	private DataTable dt = new DataTable();

	private IContainer components;

	private GridControlEx grdActivity;

	private GridViewEx gvActivity;

	private SimpleButtonEx btChoose;

	private SimpleButtonEx btCancel;

	private GridColumnEx gcClgCode;

	private GridColumnEx gcCardName;

	private GridColumnEx gcCntctDate;

	private GridColumnEx gcRecontact;

	private GridColumnEx gcRecurPat;

	public DataTable dtSearch { get; set; }

	public frm_ActivitiesListForOpportunity(DataTable dtSearch)
	{
		InitializeComponent();
		dt = dtSearch;
	}

	private void gvActivity_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void frm_ActivitiesListForOpportunity_Load(object sender, EventArgs e)
	{
		Search(dt);
	}

	private async void Search(DataTable dt)
	{
		try
		{
			WinCommons._OpenProcessing();
			using (HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivitiesForOpportunity"))
			{
				dtObject = await _helper.PostAsync("SearchDoc", dt);
				if (dtObject != null && dtObject.Rows.Count > 0)
				{
					dtObject.PrimaryKey = new DataColumn[1] { dtObject.Columns["ClgCode"] };
					grdActivity.DataSource = dtObject;
					grdActivity.RefreshDataSource();
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btChoose_Click(object sender, EventArgs e)
	{
		ChooseListActivity();
	}

	private void gvActivity_DoubleClick(object sender, EventArgs e)
	{
		ChooseListActivity();
	}

	private void ChooseListActivity()
	{
		try
		{
			_clgCode_ActivityListForOpportunity = Function.ParseInt(gvActivity.GetFocusedRowCellValue("ClgCode"));
			Close();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
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
		this.grdActivity = new Apzon.Controls.Grid.GridControlEx();
		this.gvActivity = new Apzon.Controls.Grid.GridViewEx();
		this.gcClgCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCardName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCntctDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRecontact = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRecurPat = new Apzon.Controls.Grid.GridColumnEx();
		this.btChoose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdActivity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvActivity).BeginInit();
		base.SuspendLayout();
		this.grdActivity.AllowPasteExcel = true;
		this.grdActivity.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdActivity.ClipboardData = null;
		this.grdActivity.Location = new System.Drawing.Point(3, 1);
		this.grdActivity.MainView = this.gvActivity;
		this.grdActivity.Name = "grdActivity";
		this.grdActivity.Size = new System.Drawing.Size(674, 385);
		this.grdActivity.TabIndex = 0;
		this.grdActivity.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvActivity });
		this.gvActivity.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvActivity.ColumnKey = null;
		this.gvActivity.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gcClgCode, this.gcCardName, this.gcCntctDate, this.gcRecontact, this.gcRecurPat });
		this.gvActivity.DisableFormSetting = false;
		this.gvActivity.DisplayException = false;
		this.gvActivity.EditColumn = null;
		this.gvActivity.FieldCurrency = null;
		this.gvActivity.FieldRate = null;
		this.gvActivity.FormId = null;
		this.gvActivity.GridControl = this.grdActivity;
		this.gvActivity.IgnoreFilter = false;
		this.gvActivity.IndexEditRow = -1;
		this.gvActivity.IndicatorWidth = 35;
		this.gvActivity.IsApzTable = false;
		this.gvActivity.IsInsertNewRow = false;
		this.gvActivity.IsLoadingData = false;
		this.gvActivity.IsMappingContextAction = false;
		this.gvActivity.ItemId = null;
		this.gvActivity.LstColumnAllowBatch = null;
		this.gvActivity.LstColumnNotAllowBatch = null;
		this.gvActivity.LstColumnNotEditable = null;
		this.gvActivity.LstColumNotSetting = null;
		this.gvActivity.Name = "gvActivity";
		this.gvActivity.OptionsBehavior.ReadOnly = true;
		this.gvActivity.OptionsFind.AlwaysVisible = true;
		this.gvActivity.OptionsFind.FindDelay = 100;
		this.gvActivity.OptionsView.ShowGroupPanel = false;
		this.gvActivity.UseBatchMod = false;
		this.gvActivity.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvActivity_CustomDrawRowIndicator);
		this.gvActivity.DoubleClick += new System.EventHandler(this.gvActivity_DoubleClick);
		this.gcClgCode.Caption = "ClgCode";
		this.gcClgCode.CurrencyDateField = null;
		this.gcClgCode.CurrencyField = null;
		this.gcClgCode.CurrencyFixed = null;
		this.gcClgCode.CurrencyFixedRate = 0.0;
		this.gcClgCode.CurrencyRateField = null;
		this.gcClgCode.FieldName = "ClgCode";
		this.gcClgCode.Name = "gcClgCode";
		this.gcClgCode.PrimaryKey = true;
		this.gcClgCode.UnitField = null;
		this.gcClgCode.Width = 69;
		this.gcCardName.Caption = "BP Name";
		this.gcCardName.CurrencyDateField = null;
		this.gcCardName.CurrencyField = null;
		this.gcCardName.CurrencyFixed = null;
		this.gcCardName.CurrencyFixedRate = 0.0;
		this.gcCardName.CurrencyRateField = null;
		this.gcCardName.FieldName = "CardName";
		this.gcCardName.Name = "gcCardName";
		this.gcCardName.PrimaryKey = false;
		this.gcCardName.UnitField = null;
		this.gcCardName.Visible = true;
		this.gcCardName.VisibleIndex = 0;
		this.gcCardName.Width = 417;
		this.gcCntctDate.Caption = "System Date";
		this.gcCntctDate.CurrencyDateField = null;
		this.gcCntctDate.CurrencyField = null;
		this.gcCntctDate.CurrencyFixed = null;
		this.gcCntctDate.CurrencyFixedRate = 0.0;
		this.gcCntctDate.CurrencyRateField = null;
		this.gcCntctDate.FieldName = "CntctDate";
		this.gcCntctDate.Name = "gcCntctDate";
		this.gcCntctDate.PrimaryKey = false;
		this.gcCntctDate.UnitField = null;
		this.gcCntctDate.Visible = true;
		this.gcCntctDate.VisibleIndex = 1;
		this.gcCntctDate.Width = 163;
		this.gcRecontact.Caption = "Activity Date";
		this.gcRecontact.CurrencyDateField = null;
		this.gcRecontact.CurrencyField = null;
		this.gcRecontact.CurrencyFixed = null;
		this.gcRecontact.CurrencyFixedRate = 0.0;
		this.gcRecontact.CurrencyRateField = null;
		this.gcRecontact.FieldName = "Recontact";
		this.gcRecontact.Name = "gcRecontact";
		this.gcRecontact.PrimaryKey = false;
		this.gcRecontact.UnitField = null;
		this.gcRecontact.Visible = true;
		this.gcRecontact.VisibleIndex = 2;
		this.gcRecontact.Width = 163;
		this.gcRecurPat.Caption = "Recurrence";
		this.gcRecurPat.CurrencyDateField = null;
		this.gcRecurPat.CurrencyField = null;
		this.gcRecurPat.CurrencyFixed = null;
		this.gcRecurPat.CurrencyFixedRate = 0.0;
		this.gcRecurPat.CurrencyRateField = null;
		this.gcRecurPat.FieldName = "RecurPat";
		this.gcRecurPat.Name = "gcRecurPat";
		this.gcRecurPat.PrimaryKey = false;
		this.gcRecurPat.UnitField = null;
		this.gcRecurPat.Visible = true;
		this.gcRecurPat.VisibleIndex = 3;
		this.gcRecurPat.Width = 179;
		this.btChoose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btChoose.EditControl = null;
		this.btChoose.EditValue = "Choose";
		this.btChoose.FieldName = null;
		this.btChoose.ImageOptions.Image = Apzon.Forms.Properties.Resources.withtextwrapping_topright_16x16;
		this.btChoose.IsSearchControl = false;
		this.btChoose.Location = new System.Drawing.Point(3, 400);
		this.btChoose.Name = "btChoose";
		this.btChoose.ReadOnly = false;
		this.btChoose.SettingInVisible = false;
		this.btChoose.SettingReadonly = false;
		this.btChoose.Size = new System.Drawing.Size(75, 23);
		this.btChoose.TabIndex = 1;
		this.btChoose.Text = "Choose";
		this.btChoose.Click += new System.EventHandler(this.btChoose_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(84, 400);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 1;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(679, 426);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btChoose);
		base.Controls.Add(this.grdActivity);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ActivitiesListForOpportunity";
		this.Text = "List of Activities";
		base.Load += new System.EventHandler(this.frm_ActivitiesListForOpportunity_Load);
		((System.ComponentModel.ISupportInitialize)this.grdActivity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvActivity).EndInit();
		base.ResumeLayout(false);
	}
}
