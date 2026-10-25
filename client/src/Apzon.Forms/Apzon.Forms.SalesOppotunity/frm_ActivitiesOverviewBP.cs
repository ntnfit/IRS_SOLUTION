using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.BusinessMaster;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.SalesOppotunity;

public class frm_ActivitiesOverviewBP : XtraFormEx
{
	private string _privilege = "";

	public string _cardCode = string.Empty;

	private DataTable dtObject = new DataTable();

	private DataTable dt = new DataTable();

	private IContainer components;

	private CheckEditEx chkDisplay;

	private GridControlEx grdActivity;

	private GridViewEx gvActivity;

	private SimpleButtonEx btOK;

	private GridColumnEx gcClgCode;

	private GridColumnEx gcRecontact;

	private GridColumnEx gcStartTime;

	private GridColumnEx gcAttendUser;

	private GridColumnEx gcActivity;

	private GridColumnEx gcRecurPat;

	private GridColumnEx gcCardName;

	private GridColumnEx gcCntctCode;

	private GridColumnEx gcDetails;

	private GridColumnEx gcAssignedBy;

	private RepositoryItemButtonEdit rIBt_ClgCode;

	private GridColumnEx gcContactPerson;

	private RepositoryItemButtonEdit rIBt_CardCode;

	private RepositoryItemButtonEdit rIBt_ContactP;

	private GridColumnEx gcCardCode;

	public DataTable dtSearch { get; set; }

	public frm_ActivitiesOverviewBP(DataTable dtSearch)
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ActivitiesOverviewBP"))
		{
			InitializeComponent();
			SettingLanguage();
			dt = dtSearch;
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ActivitiesOverviewBP" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void SettingLanguage()
	{
		Text = Resources.Titel_frmActivitiesOverviewBP;
		btOK.Text = Resources.ButtonOK;
		chkDisplay.Text = Resources.OCLG_inactive;
		gcClgCode.Caption = Resources.OCLG_ClgCode;
		gcRecontact.Caption = Resources.OCLG_Recontact;
		gcStartTime.Caption = Resources.OCLG_BeginTime;
		gcAttendUser.Caption = Resources.OCLG_HandledBy;
		gcActivity.Caption = Resources.OCLG_Action;
		gcStartTime.Caption = Resources.OCLG_BeginTime;
		gcRecurPat.Caption = Resources.OCLG_RecurPat;
		gcCardName.Caption = Resources.lb_CardName;
		gcCntctCode.Caption = Resources.lb_CntctCode;
		gcDetails.Caption = Resources.OCLG_Details;
		gcAssignedBy.Caption = Resources.OCLG_AssignedBy;
	}

	private void btOK_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void frm_ActivitiesOverviewBP_Load(object sender, EventArgs e)
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

	private void chkDisplay_CheckedChanged(object sender, EventArgs e)
	{
		dt = dtObject.Clone();
		DataRow dataRow = dt.NewRow();
		dataRow["CardCode"] = _cardCode;
		if (chkDisplay.Checked)
		{
			dataRow["inactive"] = "Y";
		}
		else
		{
			dataRow["inactive"] = "";
		}
		dt.Rows.Add(dataRow);
		SearhInactive(dt);
	}

	private async void SearhInactive(DataTable dt)
	{
		try
		{
			WinCommons._OpenProcessing();
			using (HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ActivitiesForOpportunity"))
			{
				dtObject = await _helper.PostAsync("SearchActivityOverviewBP", dt);
				if (dtObject != null && dtObject.Rows.Count > 0)
				{
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

	private void rIBt_ClgCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		frm_ActivitiesForOpportunity frm_ActivitiesForOpportunity2 = new frm_ActivitiesForOpportunity();
		frm_ActivitiesForOpportunity2._clgCode_FromOppotunity = Function.ParseInt(gvActivity.GetFocusedRowCellValue("ClgCode"));
		frm_ActivitiesForOpportunity2.Show();
	}

	private void rIBt_CardCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
		frm_BusinessMasterData2._copyF_CodeCustomer = Function.ToString(gvActivity.GetFocusedRowCellValue("CardCode"));
		frm_BusinessMasterData2.Show();
	}

	private void rIBt_ContactP_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		frm_BusinessMasterData frm_BusinessMasterData2 = new frm_BusinessMasterData();
		frm_BusinessMasterData2._copyF_CodeCustomer = Function.ToString(gvActivity.GetFocusedRowCellValue("CardCode"));
		frm_BusinessMasterData2.Show();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.SalesOppotunity.frm_ActivitiesOverviewBP));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		this.chkDisplay = new Apzon.Controls.Editor.CheckEditEx();
		this.grdActivity = new Apzon.Controls.Grid.GridControlEx();
		this.gvActivity = new Apzon.Controls.Grid.GridViewEx();
		this.gcClgCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBt_ClgCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcRecontact = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStartTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAttendUser = new Apzon.Controls.Grid.GridColumnEx();
		this.gcActivity = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRecurPat = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCardName = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBt_CardCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcCntctCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rIBt_ContactP = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcContactPerson = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDetails = new Apzon.Controls.Grid.GridColumnEx();
		this.gcAssignedBy = new Apzon.Controls.Grid.GridColumnEx();
		this.gcCardCode = new Apzon.Controls.Grid.GridColumnEx();
		this.btOK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.chkDisplay.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdActivity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvActivity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBt_ClgCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBt_CardCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIBt_ContactP).BeginInit();
		base.SuspendLayout();
		this.chkDisplay.EditControl = null;
		this.chkDisplay.EditValue = "N";
		this.chkDisplay.FieldName = null;
		this.chkDisplay.IsSearchControl = false;
		this.chkDisplay.Location = new System.Drawing.Point(12, 12);
		this.chkDisplay.Name = "chkDisplay";
		this.chkDisplay.Properties.Caption = "Display Only Open Activites";
		this.chkDisplay.Properties.ValueChecked = "Y";
		this.chkDisplay.Properties.ValueUnchecked = "N";
		this.chkDisplay.SettingInVisible = false;
		this.chkDisplay.SettingReadonly = false;
		this.chkDisplay.Size = new System.Drawing.Size(214, 20);
		this.chkDisplay.TabIndex = 0;
		this.chkDisplay.CheckedChanged += new System.EventHandler(this.chkDisplay_CheckedChanged);
		this.grdActivity.AllowPasteExcel = true;
		this.grdActivity.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdActivity.ClipboardData = null;
		this.grdActivity.Location = new System.Drawing.Point(3, 37);
		this.grdActivity.MainView = this.gvActivity;
		this.grdActivity.Name = "grdActivity";
		this.grdActivity.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rIBt_ClgCode, this.rIBt_CardCode, this.rIBt_ContactP });
		this.grdActivity.Size = new System.Drawing.Size(844, 346);
		this.grdActivity.TabIndex = 1;
		this.grdActivity.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvActivity });
		this.gvActivity.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvActivity.ColumnKey = null;
		this.gvActivity.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gcClgCode, this.gcRecontact, this.gcStartTime, this.gcAttendUser, this.gcActivity, this.gcRecurPat, this.gcCardName, this.gcCntctCode, this.gcContactPerson, this.gcDetails,
			this.gcAssignedBy, this.gcCardCode
		});
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
		this.gvActivity.OptionsView.ShowGroupPanel = false;
		this.gvActivity.UseBatchMod = false;
		this.gcClgCode.Caption = "Number";
		this.gcClgCode.ColumnEdit = this.rIBt_ClgCode;
		this.gcClgCode.CurrencyDateField = null;
		this.gcClgCode.CurrencyField = null;
		this.gcClgCode.CurrencyFixed = null;
		this.gcClgCode.CurrencyFixedRate = 0.0;
		this.gcClgCode.CurrencyRateField = null;
		this.gcClgCode.FieldName = "ClgCode";
		this.gcClgCode.Name = "gcClgCode";
		this.gcClgCode.PrimaryKey = true;
		this.gcClgCode.UnitField = null;
		this.gcClgCode.Visible = true;
		this.gcClgCode.VisibleIndex = 0;
		this.gcClgCode.Width = 54;
		this.rIBt_ClgCode.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.rIBt_ClgCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rIBt_ClgCode.Name = "rIBt_ClgCode";
		this.rIBt_ClgCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBt_ClgCode_ButtonPressed);
		this.gcRecontact.Caption = "Start Date";
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
		this.gcRecontact.VisibleIndex = 1;
		this.gcRecontact.Width = 66;
		this.gcStartTime.Caption = "Start Time";
		this.gcStartTime.CurrencyDateField = null;
		this.gcStartTime.CurrencyField = null;
		this.gcStartTime.CurrencyFixed = null;
		this.gcStartTime.CurrencyFixedRate = 0.0;
		this.gcStartTime.CurrencyRateField = null;
		this.gcStartTime.FieldName = "BeginTime";
		this.gcStartTime.Name = "gcStartTime";
		this.gcStartTime.PrimaryKey = false;
		this.gcStartTime.UnitField = null;
		this.gcStartTime.Visible = true;
		this.gcStartTime.VisibleIndex = 2;
		this.gcStartTime.Width = 64;
		this.gcAttendUser.Caption = "Handled By";
		this.gcAttendUser.CurrencyDateField = null;
		this.gcAttendUser.CurrencyField = null;
		this.gcAttendUser.CurrencyFixed = null;
		this.gcAttendUser.CurrencyFixedRate = 0.0;
		this.gcAttendUser.CurrencyRateField = null;
		this.gcAttendUser.FieldName = "AttendUser";
		this.gcAttendUser.Name = "gcAttendUser";
		this.gcAttendUser.PrimaryKey = false;
		this.gcAttendUser.UnitField = null;
		this.gcAttendUser.Visible = true;
		this.gcAttendUser.VisibleIndex = 3;
		this.gcAttendUser.Width = 77;
		this.gcActivity.Caption = "Activity";
		this.gcActivity.CurrencyDateField = null;
		this.gcActivity.CurrencyField = null;
		this.gcActivity.CurrencyFixed = null;
		this.gcActivity.CurrencyFixedRate = 0.0;
		this.gcActivity.CurrencyRateField = null;
		this.gcActivity.FieldName = "Action";
		this.gcActivity.Name = "gcActivity";
		this.gcActivity.PrimaryKey = false;
		this.gcActivity.UnitField = null;
		this.gcActivity.Visible = true;
		this.gcActivity.VisibleIndex = 4;
		this.gcActivity.Width = 77;
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
		this.gcRecurPat.VisibleIndex = 5;
		this.gcRecurPat.Width = 77;
		this.gcCardName.Caption = "BP Name";
		this.gcCardName.ColumnEdit = this.rIBt_CardCode;
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
		this.gcCardName.VisibleIndex = 6;
		this.gcCardName.Width = 110;
		this.rIBt_CardCode.AutoHeight = false;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rIBt_CardCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rIBt_CardCode.Name = "rIBt_CardCode";
		this.rIBt_CardCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBt_CardCode_ButtonPressed);
		this.gcCntctCode.Caption = "Contact Person";
		this.gcCntctCode.ColumnEdit = this.rIBt_ContactP;
		this.gcCntctCode.CurrencyDateField = null;
		this.gcCntctCode.CurrencyField = null;
		this.gcCntctCode.CurrencyFixed = null;
		this.gcCntctCode.CurrencyFixedRate = 0.0;
		this.gcCntctCode.CurrencyRateField = null;
		this.gcCntctCode.FieldName = "Name";
		this.gcCntctCode.Name = "gcCntctCode";
		this.gcCntctCode.PrimaryKey = false;
		this.gcCntctCode.UnitField = null;
		this.gcCntctCode.Visible = true;
		this.gcCntctCode.VisibleIndex = 7;
		this.gcCntctCode.Width = 111;
		this.rIBt_ContactP.AutoHeight = false;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.rIBt_ContactP.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rIBt_ContactP.Name = "rIBt_ContactP";
		this.rIBt_ContactP.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rIBt_ContactP_ButtonPressed);
		this.gcContactPerson.Caption = "Contact Person";
		this.gcContactPerson.CurrencyDateField = null;
		this.gcContactPerson.CurrencyField = null;
		this.gcContactPerson.CurrencyFixed = null;
		this.gcContactPerson.CurrencyFixedRate = 0.0;
		this.gcContactPerson.CurrencyRateField = null;
		this.gcContactPerson.FieldName = "CntctCode";
		this.gcContactPerson.Name = "gcContactPerson";
		this.gcContactPerson.PrimaryKey = false;
		this.gcContactPerson.UnitField = null;
		this.gcDetails.Caption = "Remarks";
		this.gcDetails.CurrencyDateField = null;
		this.gcDetails.CurrencyField = null;
		this.gcDetails.CurrencyFixed = null;
		this.gcDetails.CurrencyFixedRate = 0.0;
		this.gcDetails.CurrencyRateField = null;
		this.gcDetails.FieldName = "Details";
		this.gcDetails.Name = "gcDetails";
		this.gcDetails.PrimaryKey = false;
		this.gcDetails.UnitField = null;
		this.gcDetails.Visible = true;
		this.gcDetails.VisibleIndex = 8;
		this.gcDetails.Width = 43;
		this.gcAssignedBy.Caption = "Assigned By";
		this.gcAssignedBy.CurrencyDateField = null;
		this.gcAssignedBy.CurrencyField = null;
		this.gcAssignedBy.CurrencyFixed = null;
		this.gcAssignedBy.CurrencyFixedRate = 0.0;
		this.gcAssignedBy.CurrencyRateField = null;
		this.gcAssignedBy.FieldName = "AssignedBy";
		this.gcAssignedBy.Name = "gcAssignedBy";
		this.gcAssignedBy.PrimaryKey = false;
		this.gcAssignedBy.UnitField = null;
		this.gcAssignedBy.Visible = true;
		this.gcAssignedBy.VisibleIndex = 9;
		this.gcAssignedBy.Width = 51;
		this.gcCardCode.Caption = "CardCode";
		this.gcCardCode.CurrencyDateField = null;
		this.gcCardCode.CurrencyField = null;
		this.gcCardCode.CurrencyFixed = null;
		this.gcCardCode.CurrencyFixedRate = 0.0;
		this.gcCardCode.CurrencyRateField = null;
		this.gcCardCode.FieldName = "CardCode";
		this.gcCardCode.Name = "gcCardCode";
		this.gcCardCode.PrimaryKey = false;
		this.gcCardCode.UnitField = null;
		this.btOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOK.EditControl = null;
		this.btOK.EditValue = "OK";
		this.btOK.FieldName = null;
		this.btOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOK.ImageOptions.Image");
		this.btOK.IsSearchControl = false;
		this.btOK.Location = new System.Drawing.Point(3, 393);
		this.btOK.Name = "btOK";
		this.btOK.ReadOnly = false;
		this.btOK.SettingInVisible = false;
		this.btOK.SettingReadonly = false;
		this.btOK.Size = new System.Drawing.Size(75, 23);
		this.btOK.TabIndex = 2;
		this.btOK.Text = "OK";
		this.btOK.Click += new System.EventHandler(this.btOK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(849, 428);
		base.Controls.Add(this.btOK);
		base.Controls.Add(this.grdActivity);
		base.Controls.Add(this.chkDisplay);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ActivitiesOverviewBP";
		this.Text = "Activities Overview for Business Partner ";
		base.Load += new System.EventHandler(this.frm_ActivitiesOverviewBP_Load);
		((System.ComponentModel.ISupportInitialize)this.chkDisplay.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdActivity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvActivity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBt_ClgCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBt_CardCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIBt_ContactP).EndInit();
		base.ResumeLayout(false);
	}
}
