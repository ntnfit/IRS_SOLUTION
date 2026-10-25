using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Api.Models;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Firebase.Database;
using Firebase.Database.Streaming;

namespace Apzon.Forms.DMS;

public class frm_UserOnlineDashboard : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private string _firebaseCheckOnlineKey = "";

	private string _dataChildPath = "";

	private IContainer components;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gc_UserId;

	private GridColumnEx gc_UserName;

	private GridColumnEx gc_Status;

	private GridViewEx gridViewEx1;

	private RepositoryItemLookUpEdit rep_Status;

	public int AbsEntry { get; set; }

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
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpload;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_UserOnlineDashboard()
	{
		InitializeComponent();
		OnLoadAsync += frm_UserOnlineDashboard_Load;
	}

	private async Task frm_UserOnlineDashboard_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			_isLoading = true;
			gv_Doc.RowHeight = -1;
			LoadInit();
			string auth = "6z2f5qBveTKNuzuySeCpTEq2rdOB6U5cK94Gsf0G";
			FirebaseClient firebase = new FirebaseClient("https://vertical-veld-145604.firebaseio.com/", new FirebaseOptions
			{
				AuthTokenAsyncFactory = () => Task.FromResult(auth),
				SyncPeriod = TimeSpan.FromSeconds(5.0)
			});
			string apiServer = Configuration.ApiServer;
			string dbName = GlobalData.DatabaseName;
			string domain = GlobalData.Domain;
			FormMode = Enums.FormViewModeType.View;
			List<UserOnlineStatusImpl> dt = new List<UserOnlineStatusImpl>();
			List<FirebaseObject<OnlineDataImpl>> list = (await firebase.Child("OnlineData").OnceAsync<OnlineDataImpl>()) as List<FirebaseObject<OnlineDataImpl>>;
			for (int num = 0; num < list.Count; num++)
			{
				OnlineDataImpl onlineDataImpl = list[num].Object;
				if (onlineDataImpl.ServerAddress.Replace("/", "").ToUpper() == apiServer.Replace("/", "").ToUpper() && onlineDataImpl.Domain.ToUpper() == domain.ToUpper() && onlineDataImpl.DatabaseName.ToUpper() == dbName.ToUpper())
				{
					_firebaseCheckOnlineKey = list[num].Key;
					_dataChildPath = "OnlineData/" + _firebaseCheckOnlineKey + "/Data";
					break;
				}
			}
			firebase.Child(_dataChildPath).AsObservable<UserOnlineStatusImpl>().Subscribe((FirebaseEvent<UserOnlineStatusImpl> d) =>
			{
				if (d.Object != null && grd_Doc.DataSource is List<UserOnlineStatusImpl> { Count: >0 } list2)
				{
					for (int i = 0; i < list2.Count; i++)
					{
						if (list2[i].UserId == d.Object.UserId)
						{
							list2[i].Status = d.Object.Status;
						}
					}
					grd_Doc.DataSource = list2;
					grd_Doc.RefreshDataSource();
				}
			});
			List<FirebaseObject<UserOnlineStatusImpl>> dtOnline = new List<FirebaseObject<UserOnlineStatusImpl>>();
			List<UserOnlineStatusImpl> dtOnlineHaveNull = new List<UserOnlineStatusImpl>();
			try
			{
				dtOnlineHaveNull = await firebase.Child(_dataChildPath).OnceSingleAsync<List<UserOnlineStatusImpl>>();
			}
			catch (Exception)
			{
				dtOnline = (await firebase.Child(_dataChildPath).OnceAsync<UserOnlineStatusImpl>()) as List<FirebaseObject<UserOnlineStatusImpl>>;
			}
			if (dtOnlineHaveNull.Count > 0)
			{
				for (int num2 = 0; num2 < dtOnlineHaveNull.Count; num2++)
				{
					UserOnlineStatusImpl userOnlineStatusImpl = dtOnlineHaveNull[num2];
					if (userOnlineStatusImpl != null)
					{
						dt.Add(userOnlineStatusImpl);
					}
				}
			}
			else if (dtOnline.Count > 0)
			{
				for (int num3 = 0; num3 < dtOnline.Count; num3++)
				{
					UserOnlineStatusImpl userOnlineStatusImpl2 = dtOnline[num3].Object;
					if (userOnlineStatusImpl2 != null)
					{
						dt.Add(userOnlineStatusImpl2);
					}
				}
			}
			grd_Doc.DataSource = dt;
			grd_Doc.RefreshDataSource();
			_isLoading = false;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
	}

	private void LoadInit()
	{
		try
		{
			Dictionary<string, string> dic = new Dictionary<string, string>
			{
				{ "O", "Trư\u0323c tuyê\u0301n" },
				{ "F", "Ngoa\u0323i tuyê\u0301n" },
				{ "U", "Không xa\u0301c đi\u0323nh" }
			};
			rep_Status.SetDataSource(dic, hideCodeColumn: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if ((FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await frm_UserOnlineDashboard_Load(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void frm_UploadItemImage_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
		{
			e.Cancel = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes;
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column == gc_Status)
			{
				if (Function.ToString(e.CellValue).ToUpper().Equals("O"))
				{
					e.Appearance.BackColor = Color.LightGreen;
				}
				else if (Function.ToString(e.CellValue).ToUpper().Equals("F"))
				{
					e.Appearance.BackColor = Color.LightGray;
				}
				else
				{
					e.Appearance.BackColor = Color.White;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
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
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_UserId = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UserName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridViewEx1 = new Apzon.Controls.Grid.GridViewEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).BeginInit();
		base.SuspendLayout();
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(83, 482);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 11;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 482);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 10;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(2, 12);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Status });
		this.grd_Doc.Size = new System.Drawing.Size(1129, 464);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_Doc, this.gridViewEx1 });
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gc_UserId, this.gc_UserName, this.gc_Status });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oUploadItemImage";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_D_UUPIM";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsBehavior.ReadOnly = true;
		this.gv_Doc.OptionsCustomization.AllowRowSizing = true;
		this.gv_Doc.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.RowHeight = 1;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gc_UserId.Caption = "Mã ngươ\u0300i du\u0300ng";
		this.gc_UserId.CurrencyDateField = null;
		this.gc_UserId.CurrencyField = null;
		this.gc_UserId.CurrencyFixed = null;
		this.gc_UserId.CurrencyFixedRate = 0.0;
		this.gc_UserId.CurrencyRateField = null;
		this.gc_UserId.FieldName = "UserId";
		this.gc_UserId.Name = "gc_UserId";
		this.gc_UserId.PrimaryKey = true;
		this.gc_UserId.UnitField = null;
		this.gc_UserId.Visible = true;
		this.gc_UserId.VisibleIndex = 0;
		this.gc_UserId.Width = 150;
		this.gc_UserName.Caption = "Tên ngươ\u0300i du\u0300ng";
		this.gc_UserName.CurrencyDateField = null;
		this.gc_UserName.CurrencyField = null;
		this.gc_UserName.CurrencyFixed = null;
		this.gc_UserName.CurrencyFixedRate = 0.0;
		this.gc_UserName.CurrencyRateField = null;
		this.gc_UserName.FieldName = "UserName";
		this.gc_UserName.Name = "gc_UserName";
		this.gc_UserName.OptionsColumn.ReadOnly = true;
		this.gc_UserName.PrimaryKey = false;
		this.gc_UserName.UnitField = null;
		this.gc_UserName.Visible = true;
		this.gc_UserName.VisibleIndex = 1;
		this.gc_UserName.Width = 150;
		this.gc_Status.Caption = "Tra\u0323ng tha\u0301i";
		this.gc_Status.ColumnEdit = this.rep_Status;
		this.gc_Status.CurrencyDateField = null;
		this.gc_Status.CurrencyField = null;
		this.gc_Status.CurrencyFixed = null;
		this.gc_Status.CurrencyFixedRate = 0.0;
		this.gc_Status.CurrencyRateField = null;
		this.gc_Status.FieldName = "Status";
		this.gc_Status.Name = "gc_Status";
		this.gc_Status.OptionsColumn.ReadOnly = true;
		this.gc_Status.PrimaryKey = false;
		this.gc_Status.UnitField = null;
		this.gc_Status.Visible = true;
		this.gc_Status.VisibleIndex = 2;
		this.gc_Status.Width = 150;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.DisplayMember = "Name";
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.rep_Status.ValueMember = "Code";
		this.gridViewEx1.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridViewEx1.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridViewEx1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridViewEx1.ColumnKey = null;
		this.gridViewEx1.DisableFormSetting = false;
		this.gridViewEx1.DisplayException = false;
		this.gridViewEx1.EditColumn = null;
		this.gridViewEx1.FieldCurrency = null;
		this.gridViewEx1.FieldRate = null;
		this.gridViewEx1.FormId = null;
		this.gridViewEx1.GridControl = this.grd_Doc;
		this.gridViewEx1.IgnoreFilter = false;
		this.gridViewEx1.IndexEditRow = -1;
		this.gridViewEx1.IndicatorWidth = 35;
		this.gridViewEx1.IsApzTable = false;
		this.gridViewEx1.IsInsertNewRow = false;
		this.gridViewEx1.IsLoadingData = false;
		this.gridViewEx1.IsMappingContextAction = false;
		this.gridViewEx1.ItemId = null;
		this.gridViewEx1.LstColumnAllowBatch = null;
		this.gridViewEx1.LstColumnNotAllowBatch = null;
		this.gridViewEx1.LstColumnNotEditable = null;
		this.gridViewEx1.LstColumNotSetting = null;
		this.gridViewEx1.Name = "gridViewEx1";
		this.gridViewEx1.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridViewEx1.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridViewEx1.OptionsPrint.AutoWidth = false;
		this.gridViewEx1.OptionsPrint.UsePrintStyles = false;
		this.gridViewEx1.OptionsView.BestFitMaxRowCount = 100;
		this.gridViewEx1.OptionsView.ColumnAutoWidth = false;
		this.gridViewEx1.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridViewEx1.OptionsView.ShowGroupedColumns = true;
		this.gridViewEx1.OptionsView.ShowGroupPanel = false;
		this.gridViewEx1.UseBatchMod = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1135, 510);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.FormId = "oUserOnlineDashboard";
		base.FormObjectType = Apzon.Commons.ApzObjectTypes.oKPIsSetting;
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_UserOnlineDashboard";
		this.Text = "Tra\u0323ng tha\u0301i ngươ\u0300i du\u0300ng app DMS";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_UploadItemImage_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridViewEx1).EndInit();
		base.ResumeLayout(false);
	}
}
