using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
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
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using Newtonsoft.Json;

namespace Apzon.Forms.Administrator;

public class frm_SettingPassword : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	public ShowForms frmShowChildrenForm;

	private IContainer components;

	private PanelControl panelControl3;

	private SimpleButtonEx btRefresh;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btOk;

	private PanelControl panelControl4;

	private TextEditEx textEdit1;

	private LabelLine lb_Find;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gcUSER_CODE;

	private GridColumnEx gcU_NAME;

	private GridColumnEx gcDepartment;

	private GridColumnEx gcBranch;

	private GridColumnEx gcUSERID;

	private GridColumnEx gcPASSWORD;

	private GridColumnEx gcINTERNAL_K;

	private GridColumnEx gcGROUPS;

	private GridColumnEx gcSUPERUSER;

	private GridColumnEx gcE_Mail;

	private GridColumnEx gcPortNum;

	private GridColumnEx gcFax;

	private GridColumnEx gcLocked;

	private GridColumnEx gcuserSign;

	private GridColumnEx gccreateDate;

	private GridColumnEx gcuserSign2;

	private GridColumnEx gcupdateDate;

	private GridColumnEx gclastLogin;

	private GridColumnEx gcNameDepartment;

	private GridColumnEx gcNameBranch;

	private TextEditEx txtPassword;

	private LabelLine lb_Password;

	private TextEditEx txtUserCode;

	private LabelLine lb_UserName;

	public frm_SettingPassword()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SettingPassword"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SettingPassword" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void SettingLanguage()
	{
		Text = Resources.Titel_SettingPassword;
		lb_Find.Utext = Resources.ButtonFind;
		lb_UserName.Utext = Resources.lb_UserName;
		lb_Password.Utext = Resources.lb_Password;
		btOk.Text = Resources.ButtonSetting;
		btCancel.Text = Resources.ButtonCancel;
		btRefresh.Text = Resources.ButtonRefresh;
		gcUSER_CODE.Caption = Resources.OUSR_USERID;
		gcU_NAME.Caption = Resources.OUSR_U_NAME;
		gcNameBranch.Caption = Resources.OUBR_BranchName;
		gcNameDepartment.Caption = Resources.Doc_gvDoc_gcDepartment;
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void frm_SettingPassword_Load(object sender, EventArgs e)
	{
		LoadData();
		txtUserCode.Enabled = false;
	}

	private void LoadData()
	{
		ShowUsers();
	}

	private async void ShowUsers()
	{
		using HttpClient client = new HttpClient();
		client.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "Users", "ShowUsers");
		new List<UsersImpl>();
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (response.IsSuccessStatusCode)
		{
			List<UsersImpl> dataSource = JsonConvert.DeserializeObject<List<UsersImpl>>(await response.Content.ReadAsStringAsync()).ToList();
			grdDoc.DataSource = dataSource;
			grdDoc.RefreshDataSource();
		}
	}

	private void textEdit1_TextChanged(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(textEdit1.Text))
		{
			Search();
		}
		else
		{
			ShowUsers();
		}
	}

	private void Search()
	{
		try
		{
			UsersImpl usersImpl = new UsersImpl();
			usersImpl.USER_CODE = textEdit1.Text;
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
			httpRequestMessage.Content = new ObjectContent<UsersImpl>(usersImpl, new JsonMediaTypeFormatter());
			httpRequestMessage.Method = HttpMethod.Post;
			httpRequestMessage.RequestUri = new Uri(Function.GetApiUri(Configuration.ApiServer, "Users", "Search"));
			List<UsersImpl> lst = new List<UsersImpl>();
			HttpClient httpClient = new HttpClient();
			httpClient.DefaultRequestHeaders.Add("SessionId", GlobalData.SessionId);
			httpClient.SendAsync(httpRequestMessage).ContinueWith((Task<HttpResponseMessage> task) =>
			{
				if (task.Result.IsSuccessStatusCode)
				{
					Task<string> task2 = task.Result.Content.ReadAsStringAsync();
					if (task2 != null && !string.IsNullOrEmpty(task2.Result))
					{
						lst = JsonConvert.DeserializeObject<List<UsersImpl>>(task2.Result).ToList();
						if (grdDoc.InvokeRequired)
						{
							BeginInvoke((MethodInvoker)(() =>
							{
								grdDoc.DataSource = lst;
								grdDoc.RefreshDataSource();
							}));
						}
						else
						{
							grdDoc.DataSource = lst;
							grdDoc.RefreshDataSource();
						}
					}
				}
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void gvDoc_RowClick(object sender, RowClickEventArgs e)
	{
		if (grdDoc.DataSource is List<UsersImpl> { Count: >0 })
		{
			txtUserCode.Text = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcUSER_CODE));
			txtPassword.Focus();
		}
		else
		{
			txtUserCode.Text = string.Empty;
		}
	}

	private void btRefresh_Click(object sender, EventArgs e)
	{
		Reset();
	}

	private void Reset()
	{
		ShowUsers();
		txtUserCode.Text = string.Empty;
		txtPassword.Text = string.Empty;
		textEdit1.Text = string.Empty;
	}

	private void btOk_Click(object sender, EventArgs e)
	{
		Save();
	}

	private async void Save()
	{
		try
		{
			ClassMD5 classMD = new ClassMD5();
			UsersImpl usersImpl = new UsersImpl();
			if (btOk.Text.Equals("Setting"))
			{
				usersImpl.USERID = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcUSERID));
			}
			usersImpl.PASSWORD = classMD.GetMd5Hash(txtPassword.Text.ToString().Trim());
			usersImpl.USER_CODE = txtUserCode.Text.ToString().Trim();
			usersImpl.U_NAME = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcU_NAME));
			usersImpl.INTERNAL_K = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcINTERNAL_K));
			usersImpl.GROUPS = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcGROUPS));
			usersImpl.SUPERUSER = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcSUPERUSER));
			usersImpl.E_Mail = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcE_Mail));
			usersImpl.PortNum = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcPortNum));
			usersImpl.Locked = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcLocked));
			usersImpl.Department = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcDepartment));
			usersImpl.Branch = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcBranch));
			usersImpl.userSign = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcuserSign));
			usersImpl.createDate = Convert.ToDateTime(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gccreateDate));
			usersImpl.userSign2 = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcuserSign2));
			usersImpl.updateDate = Convert.ToDateTime(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcupdateDate));
			usersImpl.lastLogin = Convert.ToDateTime(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gclastLogin));
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await Save(usersImpl);
				Reset();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Error, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private async Task<string> Save(UsersImpl doc)
	{
		try
		{
			HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
			new MultipartFormDataContent();
			httpRequestMessage.Content = new ObjectContent<UsersImpl>(doc, new JsonMediaTypeFormatter());
			httpRequestMessage.Method = HttpMethod.Post;
			string apiUri = Function.GetApiUri(Configuration.ApiServer, "Users", "SaveDoc");
			httpRequestMessage.RequestUri = new Uri(apiUri);
			new List<UsersImpl>();
			await new HttpClient
			{
				DefaultRequestHeaders = { 
				{
					"SessionId",
					GlobalData.SessionId
				} }
			}.SendAsync(httpRequestMessage).ContinueWith((Task<HttpResponseMessage> task) =>
			{
				if (task.Result.IsSuccessStatusCode)
				{
					Task<string> task2 = task.Result.Content.ReadAsStringAsync();
					if (task2 != null && string.IsNullOrEmpty(task2.Result))
					{
						return task2.Result;
					}
					WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					gvDoc.HideLoadingPanel();
				}
				return (string)null;
			});
			return null;
		}
		catch (Exception)
		{
			throw;
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_SettingPassword));
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.btRefresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.txtPassword = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Password = new Apzon.Controls.Controls.LabelLine();
		this.txtUserCode = new Apzon.Controls.Editor.TextEditEx();
		this.lb_UserName = new Apzon.Controls.Controls.LabelLine();
		this.textEdit1 = new Apzon.Controls.Editor.TextEditEx();
		this.lb_Find = new Apzon.Controls.Controls.LabelLine();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcUSER_CODE = new Apzon.Controls.Grid.GridColumnEx();
		this.gcU_NAME = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDepartment = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBranch = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUSERID = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPASSWORD = new Apzon.Controls.Grid.GridColumnEx();
		this.gcINTERNAL_K = new Apzon.Controls.Grid.GridColumnEx();
		this.gcGROUPS = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSUPERUSER = new Apzon.Controls.Grid.GridColumnEx();
		this.gcE_Mail = new Apzon.Controls.Grid.GridColumnEx();
		this.gcPortNum = new Apzon.Controls.Grid.GridColumnEx();
		this.gcFax = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLocked = new Apzon.Controls.Grid.GridColumnEx();
		this.gcuserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gccreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcuserSign2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcupdateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gclastLogin = new Apzon.Controls.Grid.GridColumnEx();
		this.gcNameDepartment = new Apzon.Controls.Grid.GridColumnEx();
		this.gcNameBranch = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).BeginInit();
		this.panelControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtPassword.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtUserCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		base.SuspendLayout();
		this.panelControl3.Controls.Add(this.btRefresh);
		this.panelControl3.Controls.Add(this.btCancel);
		this.panelControl3.Controls.Add(this.btOk);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl3.Location = new System.Drawing.Point(0, 311);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(473, 31);
		this.panelControl3.TabIndex = 4;
		this.btRefresh.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btRefresh.EditControl = null;
		this.btRefresh.EditValue = "Refresh";
		this.btRefresh.FieldName = null;
		this.btRefresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btRefresh.ImageOptions.Image");
		this.btRefresh.IsSearchControl = false;
		this.btRefresh.Location = new System.Drawing.Point(80, 6);
		this.btRefresh.Name = "btRefresh";
		this.btRefresh.ReadOnly = false;
		this.btRefresh.SettingInVisible = false;
		this.btRefresh.SettingReadonly = false;
		this.btRefresh.Size = new System.Drawing.Size(75, 20);
		this.btRefresh.TabIndex = 16;
		this.btRefresh.Text = "Refresh";
		this.btRefresh.Click += new System.EventHandler(this.btRefresh_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(158, 6);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 20);
		this.btCancel.TabIndex = 15;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btOk.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOk.EditControl = null;
		this.btOk.EditValue = "Setting";
		this.btOk.FieldName = null;
		this.btOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOk.ImageOptions.Image");
		this.btOk.IsSearchControl = false;
		this.btOk.Location = new System.Drawing.Point(2, 6);
		this.btOk.Name = "btOk";
		this.btOk.ReadOnly = false;
		this.btOk.SettingInVisible = false;
		this.btOk.SettingReadonly = false;
		this.btOk.Size = new System.Drawing.Size(75, 20);
		this.btOk.TabIndex = 13;
		this.btOk.Text = "Setting";
		this.btOk.Click += new System.EventHandler(this.btOk_Click);
		this.panelControl4.Controls.Add(this.txtPassword);
		this.panelControl4.Controls.Add(this.lb_Password);
		this.panelControl4.Controls.Add(this.txtUserCode);
		this.panelControl4.Controls.Add(this.lb_UserName);
		this.panelControl4.Controls.Add(this.textEdit1);
		this.panelControl4.Controls.Add(this.lb_Find);
		this.panelControl4.Controls.Add(this.grdDoc);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl4.Location = new System.Drawing.Point(0, 0);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(473, 311);
		this.panelControl4.TabIndex = 6;
		this.txtPassword.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtPassword.EditControl = null;
		this.txtPassword.FieldName = null;
		this.txtPassword.IsSearchControl = false;
		this.txtPassword.Location = new System.Drawing.Point(311, 284);
		this.txtPassword.Name = "txtPassword";
		this.txtPassword.Properties.PasswordChar = '*';
		this.txtPassword.SettingInVisible = false;
		this.txtPassword.SettingReadonly = false;
		this.txtPassword.Size = new System.Drawing.Size(156, 20);
		this.txtPassword.TabIndex = 9;
		this.lb_Password.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lb_Password.EditControl = null;
		this.lb_Password.EditValue = "Password";
		this.lb_Password.FieldName = null;
		this.lb_Password.HideWhenEmpty = false;
		this.lb_Password.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Password.Location = new System.Drawing.Point(235, 286);
		this.lb_Password.Name = "lb_Password";
		this.lb_Password.ReadOnly = false;
		this.lb_Password.SettingInVisible = false;
		this.lb_Password.SettingReadonly = false;
		this.lb_Password.ShowUnderLine = true;
		this.lb_Password.Size = new System.Drawing.Size(80, 18);
		this.lb_Password.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Password.TabIndex = 8;
		this.lb_Password.Utext = "Password";
		this.txtUserCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtUserCode.EditControl = null;
		this.txtUserCode.FieldName = null;
		this.txtUserCode.IsSearchControl = false;
		this.txtUserCode.Location = new System.Drawing.Point(81, 284);
		this.txtUserCode.Name = "txtUserCode";
		this.txtUserCode.SettingInVisible = false;
		this.txtUserCode.SettingReadonly = false;
		this.txtUserCode.Size = new System.Drawing.Size(145, 20);
		this.txtUserCode.TabIndex = 7;
		this.lb_UserName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lb_UserName.EditControl = null;
		this.lb_UserName.EditValue = "User Name";
		this.lb_UserName.FieldName = null;
		this.lb_UserName.HideWhenEmpty = false;
		this.lb_UserName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_UserName.Location = new System.Drawing.Point(5, 286);
		this.lb_UserName.Name = "lb_UserName";
		this.lb_UserName.ReadOnly = false;
		this.lb_UserName.SettingInVisible = false;
		this.lb_UserName.SettingReadonly = false;
		this.lb_UserName.ShowUnderLine = true;
		this.lb_UserName.Size = new System.Drawing.Size(80, 18);
		this.lb_UserName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_UserName.TabIndex = 6;
		this.lb_UserName.Utext = "User Name";
		this.textEdit1.EditControl = null;
		this.textEdit1.FieldName = null;
		this.textEdit1.IsSearchControl = false;
		this.textEdit1.Location = new System.Drawing.Point(89, 3);
		this.textEdit1.Name = "textEdit1";
		this.textEdit1.SettingInVisible = false;
		this.textEdit1.SettingReadonly = false;
		this.textEdit1.Size = new System.Drawing.Size(226, 20);
		this.textEdit1.TabIndex = 1;
		this.textEdit1.TextChanged += new System.EventHandler(this.textEdit1_TextChanged);
		this.lb_Find.EditControl = null;
		this.lb_Find.EditValue = "Find";
		this.lb_Find.FieldName = null;
		this.lb_Find.HideWhenEmpty = false;
		this.lb_Find.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Find.Location = new System.Drawing.Point(12, 6);
		this.lb_Find.Name = "lb_Find";
		this.lb_Find.ReadOnly = false;
		this.lb_Find.SettingInVisible = false;
		this.lb_Find.SettingReadonly = false;
		this.lb_Find.ShowUnderLine = true;
		this.lb_Find.Size = new System.Drawing.Size(80, 20);
		this.lb_Find.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Find.TabIndex = 0;
		this.lb_Find.Utext = "Find";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(5, 31);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.Size = new System.Drawing.Size(468, 250);
		this.grdDoc.TabIndex = 5;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[20]
		{
			this.gcUSER_CODE, this.gcU_NAME, this.gcDepartment, this.gcBranch, this.gcUSERID, this.gcPASSWORD, this.gcINTERNAL_K, this.gcGROUPS, this.gcSUPERUSER, this.gcE_Mail,
			this.gcPortNum, this.gcFax, this.gcLocked, this.gcuserSign, this.gccreateDate, this.gcuserSign2, this.gcupdateDate, this.gclastLogin, this.gcNameDepartment, this.gcNameBranch
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = null;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gvDoc_RowClick);
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gcUSER_CODE.Caption = "UserId";
		this.gcUSER_CODE.CurrencyDateField = null;
		this.gcUSER_CODE.CurrencyField = null;
		this.gcUSER_CODE.CurrencyFixed = null;
		this.gcUSER_CODE.CurrencyFixedRate = 0.0;
		this.gcUSER_CODE.CurrencyRateField = null;
		this.gcUSER_CODE.FieldName = "USER_CODE";
		this.gcUSER_CODE.Name = "gcUSER_CODE";
		this.gcUSER_CODE.PrimaryKey = true;
		this.gcUSER_CODE.UnitField = null;
		this.gcUSER_CODE.Visible = true;
		this.gcUSER_CODE.VisibleIndex = 0;
		this.gcU_NAME.Caption = "Full Name";
		this.gcU_NAME.CurrencyDateField = null;
		this.gcU_NAME.CurrencyField = null;
		this.gcU_NAME.CurrencyFixed = null;
		this.gcU_NAME.CurrencyFixedRate = 0.0;
		this.gcU_NAME.CurrencyRateField = null;
		this.gcU_NAME.FieldName = "U_NAME";
		this.gcU_NAME.Name = "gcU_NAME";
		this.gcU_NAME.PrimaryKey = false;
		this.gcU_NAME.UnitField = null;
		this.gcU_NAME.Visible = true;
		this.gcU_NAME.VisibleIndex = 1;
		this.gcDepartment.Caption = "Department";
		this.gcDepartment.CurrencyDateField = null;
		this.gcDepartment.CurrencyField = null;
		this.gcDepartment.CurrencyFixed = null;
		this.gcDepartment.CurrencyFixedRate = 0.0;
		this.gcDepartment.CurrencyRateField = null;
		this.gcDepartment.FieldName = "Department";
		this.gcDepartment.Name = "gcDepartment";
		this.gcDepartment.PrimaryKey = false;
		this.gcDepartment.UnitField = null;
		this.gcBranch.Caption = "Branch";
		this.gcBranch.CurrencyDateField = null;
		this.gcBranch.CurrencyField = null;
		this.gcBranch.CurrencyFixed = null;
		this.gcBranch.CurrencyFixedRate = 0.0;
		this.gcBranch.CurrencyRateField = null;
		this.gcBranch.FieldName = "Branch";
		this.gcBranch.Name = "gcBranch";
		this.gcBranch.PrimaryKey = false;
		this.gcBranch.UnitField = null;
		this.gcUSERID.Caption = "USERID";
		this.gcUSERID.CurrencyDateField = null;
		this.gcUSERID.CurrencyField = null;
		this.gcUSERID.CurrencyFixed = null;
		this.gcUSERID.CurrencyFixedRate = 0.0;
		this.gcUSERID.CurrencyRateField = null;
		this.gcUSERID.FieldName = "USERID";
		this.gcUSERID.Name = "gcUSERID";
		this.gcUSERID.PrimaryKey = false;
		this.gcUSERID.UnitField = null;
		this.gcPASSWORD.Caption = "PASSWORD";
		this.gcPASSWORD.CurrencyDateField = null;
		this.gcPASSWORD.CurrencyField = null;
		this.gcPASSWORD.CurrencyFixed = null;
		this.gcPASSWORD.CurrencyFixedRate = 0.0;
		this.gcPASSWORD.CurrencyRateField = null;
		this.gcPASSWORD.FieldName = "PASSWORD";
		this.gcPASSWORD.Name = "gcPASSWORD";
		this.gcPASSWORD.PrimaryKey = false;
		this.gcPASSWORD.UnitField = null;
		this.gcINTERNAL_K.Caption = "INTERNAL_K";
		this.gcINTERNAL_K.CurrencyDateField = null;
		this.gcINTERNAL_K.CurrencyField = null;
		this.gcINTERNAL_K.CurrencyFixed = null;
		this.gcINTERNAL_K.CurrencyFixedRate = 0.0;
		this.gcINTERNAL_K.CurrencyRateField = null;
		this.gcINTERNAL_K.FieldName = "INTERNAL_K";
		this.gcINTERNAL_K.Name = "gcINTERNAL_K";
		this.gcINTERNAL_K.PrimaryKey = false;
		this.gcINTERNAL_K.UnitField = null;
		this.gcGROUPS.Caption = "GROUPS";
		this.gcGROUPS.CurrencyDateField = null;
		this.gcGROUPS.CurrencyField = null;
		this.gcGROUPS.CurrencyFixed = null;
		this.gcGROUPS.CurrencyFixedRate = 0.0;
		this.gcGROUPS.CurrencyRateField = null;
		this.gcGROUPS.FieldName = "GROUPS";
		this.gcGROUPS.Name = "gcGROUPS";
		this.gcGROUPS.PrimaryKey = false;
		this.gcGROUPS.UnitField = null;
		this.gcSUPERUSER.Caption = "SUPERUSER";
		this.gcSUPERUSER.CurrencyDateField = null;
		this.gcSUPERUSER.CurrencyField = null;
		this.gcSUPERUSER.CurrencyFixed = null;
		this.gcSUPERUSER.CurrencyFixedRate = 0.0;
		this.gcSUPERUSER.CurrencyRateField = null;
		this.gcSUPERUSER.FieldName = "SUPERUSER";
		this.gcSUPERUSER.Name = "gcSUPERUSER";
		this.gcSUPERUSER.PrimaryKey = false;
		this.gcSUPERUSER.UnitField = null;
		this.gcE_Mail.Caption = "E_Mail";
		this.gcE_Mail.CurrencyDateField = null;
		this.gcE_Mail.CurrencyField = null;
		this.gcE_Mail.CurrencyFixed = null;
		this.gcE_Mail.CurrencyFixedRate = 0.0;
		this.gcE_Mail.CurrencyRateField = null;
		this.gcE_Mail.FieldName = "E_Mail";
		this.gcE_Mail.Name = "gcE_Mail";
		this.gcE_Mail.PrimaryKey = false;
		this.gcE_Mail.UnitField = null;
		this.gcPortNum.Caption = "PortNum";
		this.gcPortNum.CurrencyDateField = null;
		this.gcPortNum.CurrencyField = null;
		this.gcPortNum.CurrencyFixed = null;
		this.gcPortNum.CurrencyFixedRate = 0.0;
		this.gcPortNum.CurrencyRateField = null;
		this.gcPortNum.FieldName = "PortNum";
		this.gcPortNum.Name = "gcPortNum";
		this.gcPortNum.PrimaryKey = false;
		this.gcPortNum.UnitField = null;
		this.gcFax.Caption = "Fax";
		this.gcFax.CurrencyDateField = null;
		this.gcFax.CurrencyField = null;
		this.gcFax.CurrencyFixed = null;
		this.gcFax.CurrencyFixedRate = 0.0;
		this.gcFax.CurrencyRateField = null;
		this.gcFax.FieldName = "Fax";
		this.gcFax.Name = "gcFax";
		this.gcFax.PrimaryKey = false;
		this.gcFax.UnitField = null;
		this.gcLocked.Caption = "Locked";
		this.gcLocked.CurrencyDateField = null;
		this.gcLocked.CurrencyField = null;
		this.gcLocked.CurrencyFixed = null;
		this.gcLocked.CurrencyFixedRate = 0.0;
		this.gcLocked.CurrencyRateField = null;
		this.gcLocked.FieldName = "Locked";
		this.gcLocked.Name = "gcLocked";
		this.gcLocked.PrimaryKey = false;
		this.gcLocked.UnitField = null;
		this.gcuserSign.Caption = "userSign";
		this.gcuserSign.CurrencyDateField = null;
		this.gcuserSign.CurrencyField = null;
		this.gcuserSign.CurrencyFixed = null;
		this.gcuserSign.CurrencyFixedRate = 0.0;
		this.gcuserSign.CurrencyRateField = null;
		this.gcuserSign.FieldName = "userSign";
		this.gcuserSign.Name = "gcuserSign";
		this.gcuserSign.PrimaryKey = false;
		this.gcuserSign.UnitField = null;
		this.gccreateDate.Caption = "createDate";
		this.gccreateDate.CurrencyDateField = null;
		this.gccreateDate.CurrencyField = null;
		this.gccreateDate.CurrencyFixed = null;
		this.gccreateDate.CurrencyFixedRate = 0.0;
		this.gccreateDate.CurrencyRateField = null;
		this.gccreateDate.FieldName = "createDate";
		this.gccreateDate.Name = "gccreateDate";
		this.gccreateDate.PrimaryKey = false;
		this.gccreateDate.UnitField = null;
		this.gcuserSign2.Caption = "userSign2";
		this.gcuserSign2.CurrencyDateField = null;
		this.gcuserSign2.CurrencyField = null;
		this.gcuserSign2.CurrencyFixed = null;
		this.gcuserSign2.CurrencyFixedRate = 0.0;
		this.gcuserSign2.CurrencyRateField = null;
		this.gcuserSign2.FieldName = "userSign2";
		this.gcuserSign2.Name = "gcuserSign2";
		this.gcuserSign2.PrimaryKey = false;
		this.gcuserSign2.UnitField = null;
		this.gcupdateDate.Caption = "updateDate";
		this.gcupdateDate.CurrencyDateField = null;
		this.gcupdateDate.CurrencyField = null;
		this.gcupdateDate.CurrencyFixed = null;
		this.gcupdateDate.CurrencyFixedRate = 0.0;
		this.gcupdateDate.CurrencyRateField = null;
		this.gcupdateDate.FieldName = "updateDate";
		this.gcupdateDate.Name = "gcupdateDate";
		this.gcupdateDate.PrimaryKey = false;
		this.gcupdateDate.UnitField = null;
		this.gclastLogin.Caption = "lastLogin";
		this.gclastLogin.CurrencyDateField = null;
		this.gclastLogin.CurrencyField = null;
		this.gclastLogin.CurrencyFixed = null;
		this.gclastLogin.CurrencyFixedRate = 0.0;
		this.gclastLogin.CurrencyRateField = null;
		this.gclastLogin.FieldName = "lastLogin";
		this.gclastLogin.Name = "gclastLogin";
		this.gclastLogin.PrimaryKey = false;
		this.gclastLogin.UnitField = null;
		this.gcNameDepartment.Caption = "Department";
		this.gcNameDepartment.CurrencyDateField = null;
		this.gcNameDepartment.CurrencyField = null;
		this.gcNameDepartment.CurrencyFixed = null;
		this.gcNameDepartment.CurrencyFixedRate = 0.0;
		this.gcNameDepartment.CurrencyRateField = null;
		this.gcNameDepartment.FieldName = "Name";
		this.gcNameDepartment.Name = "gcNameDepartment";
		this.gcNameDepartment.PrimaryKey = false;
		this.gcNameDepartment.UnitField = null;
		this.gcNameDepartment.Visible = true;
		this.gcNameDepartment.VisibleIndex = 2;
		this.gcNameBranch.Caption = "Branch";
		this.gcNameBranch.CurrencyDateField = null;
		this.gcNameBranch.CurrencyField = null;
		this.gcNameBranch.CurrencyFixed = null;
		this.gcNameBranch.CurrencyFixedRate = 0.0;
		this.gcNameBranch.CurrencyRateField = null;
		this.gcNameBranch.FieldName = "NameBranch";
		this.gcNameBranch.Name = "gcNameBranch";
		this.gcNameBranch.PrimaryKey = false;
		this.gcNameBranch.UnitField = null;
		this.gcNameBranch.Visible = true;
		this.gcNameBranch.VisibleIndex = 3;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(473, 342);
		base.Controls.Add(this.panelControl3);
		base.Controls.Add(this.panelControl4);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_SettingPassword";
		this.Text = "Setting Password";
		base.Load += new System.EventHandler(this.frm_SettingPassword_Load);
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtPassword.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtUserCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		base.ResumeLayout(false);
	}
}
