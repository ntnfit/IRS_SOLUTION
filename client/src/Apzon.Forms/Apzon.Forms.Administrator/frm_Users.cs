using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator;

public class frm_Users : XtraFormEx
{
	public delegate void ShowForms(XtraFormEx frm);

	public ShowForms FrmShowChildrenForm;

	public RefreshMain RefreshMain;

	private const string Controller = "Users";

	private bool _isLoad;

	private bool _isChangePassword;

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dtBranchAssignment = new DataTable();

	private int _userId;

	private IContainer components;

	private PanelControl panelControl1;

	private TextEditEx txtEMail;

	private TextEditEx txtUserName;

	private TextEditEx txt_USER_CODE;

	private LabelLine lb_MobilePhone;

	private LabelLine lb_MobileDeviceID;

	private LabelLine lb_Email;

	private LabelLine lb_UserName;

	private LabelLine lb_UserCode;

	private LabelLine lb_Password;

	private LabelLine lb_Department;

	private LabelLine lb_Branch;

	private CheckEditEx chkLocked;

	private CheckEditEx chkSuperuser;

	private TextEditEx txt_Password;

	private GridControlEx grdDoc;

	private GridColumnEx gcUSER_CODE;

	private GridColumnEx gcUSERID;

	private TextEditEx txtPhone;

	private TextEditEx txt_MobileIMEI;

	private SimpleButtonEx btOk;

	private GridViewEx gvDoc;

	private LabelLine labelLine2;

	private SimpleButtonEx btnCancel;

	private ContextMenuStrip contextMenuUser;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private SimpleButtonEx btn_ChangePassWord;

	private LookUpEditCombo luk_Branch;

	private LookUpEditCombo luk_Department;

	private SimpleButtonEx btn_BranchAssignment;

	private LabelLine lbl_BranchAssignment;

	private CheckEditEx chk_MobileUser;

	private TextEditEx textEditEx1;

	private LabelLine labelLine1;

	private LookUpEditCombo luk_Employee;

	private LabelLine labelLine3;

	private TextEditEx txt_Fax;

	private LabelLine lb_Fax;

	private CheckEditEx checkEditEx2;

	private CheckEditEx checkEditEx1;

	private CheckEditEx chk_AllBrnchF;

	private TextEditEx txt_AuthGroup;

	private SimpleButtonEx btn_AuthorizationGroup;

	private LabelLine labelLine5;

	private LookUpEditCombo luk_Language;

	private LabelLine lbl_Language;

	private ButtonEditEx txtDefault;

	private CheckEditEx chk_IntgrtEb;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage1;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grd_Udf;

	private GridViewEx gv_Udf;

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
		case Enums.FormViewModeType.Add:
			btOk.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btOk.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btOk.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_Users()
	{
		InitializeComponent();
		OnLoadAsync += frm_Users_Load;
	}

	public frm_Users(int userId)
	{
		InitializeComponent();
		OnLoadAsync += frm_Users_Load;
		_userId = userId;
	}

	private async Task frm_Users_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			if (GlobalData.IsMultiBranch)
			{
				lbl_BranchAssignment.Visible = true;
				btn_BranchAssignment.Visible = true;
				lb_Branch.Visible = false;
				luk_Branch.Visible = false;
			}
			else
			{
				lbl_BranchAssignment.Visible = false;
				btn_BranchAssignment.Visible = false;
				lb_Branch.Top = lbl_BranchAssignment.Top;
				lb_Branch.Left = lbl_BranchAssignment.Left;
				luk_Branch.Top = btn_BranchAssignment.Top;
				luk_Branch.Left = btn_BranchAssignment.Left;
				lb_Branch.Visible = true;
				luk_Branch.Visible = true;
			}
			await WinFuntions.LoadUdfColumns(gv_Udf, "OUSR");
			await LoadCb();
			await LoadData();
			if (_userId > 0)
			{
				int num = gvDoc.LocateByValue("USERID", (short)_userId);
				if (gvDoc.IsValidRowHandle(num))
				{
					gvDoc.FocusedRowHandle = num;
					await LoadControl();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
			}
			txt_USER_CODE.ReadOnly = true;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_userId = 0;
	}

	private async void comboBoxEdit2_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				frm_Department frm_Department2 = new frm_Department();
				frm_Department2.MaximizeBox = false;
				frm_Department2.ShowDialog(this);
				await LoadCbDepartment();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btOk_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					Close();
				}
			}
			else if (string.IsNullOrEmpty(txt_USER_CODE.Text))
			{
				WinCommons.Error(Enums.MessageCode.UserCodeCannotBeNull, "");
				txt_USER_CODE.Focus();
			}
			else if (string.IsNullOrEmpty(txtUserName.Text))
			{
				WinCommons.Error(Enums.MessageCode.UserNameCannotBeNull, "");
				txtUserName.Focus();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				await Save();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtEMail_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (!WinFuntions._isemail(txtEMail))
			{
				txtEMail.ErrorText = "Invalid Email Address!";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	private void btn_ChangePassWord_Click(object sender, EventArgs e)
	{
		try
		{
			frm_ChangePassword frm = new frm_ChangePassword
			{
				CallFromUser = true,
				Password = txt_Password.Text.Trim()
			};
			frm.FormClosing += (object se, FormClosingEventArgs ev) =>
			{
				if (frm.IsChanged)
				{
					_isChangePassword = true;
					txt_Password.Text = frm.Password;
				}
			};
			frm.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (_userId <= 0)
			{
				await LoadControl();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.DataRowCount > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteUser, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				int code = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "USERID"));
				await Delete(code);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			txt_USER_CODE.Text = string.Empty;
			txtUserName.Text = string.Empty;
			txtEMail.Text = string.Empty;
			txt_MobileIMEI.Text = string.Empty;
			txtPhone.Text = string.Empty;
			luk_Branch.EditValue = (short)(-2);
			luk_Department.EditValue = (short)(-2);
			chkSuperuser.Checked = false;
			chkLocked.Checked = false;
			txt_Password.Text = string.Empty;
			await LoadCbBranch();
			await LoadCbDepartment();
			FormMode = Enums.FormViewModeType.Add;
			txt_USER_CODE.ReadOnly = false;
			grdDoc.DataSource = new DataTable();
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Control_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			SetButtonOkText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_BranchAssignment_Click(object sender, EventArgs e)
	{
		try
		{
			frm_BranchAssignment frm = new frm_BranchAssignment(_dtBranchAssignment.Copy(), Function.ToString(gvDoc.GetFocusedRowCellValue(gcUSER_CODE)), ApzObjectTypes.oUsers);
			frm.FormClosing += (object se, FormClosingEventArgs ev) =>
			{
				if (frm.Changed)
				{
					_dtBranchAssignment = frm.DtSource.Copy();
					DataTable dataTable = grdDoc.DataSource as DataTable;
					if (FormMode == Enums.FormViewModeType.View && dataTable != null && dataTable.Rows.Count > 0)
					{
						FormMode = Enums.FormViewModeType.Update;
					}
				}
			};
			frm.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_isLoad)
			{
				e.Allow = false;
			}
			else if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				e.Allow = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtDefault_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.DefaultForUser, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.gv_CFL.ItemId = "DefaultForUser";
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				txtDefault.EditValue = Function.ToString(frm_CFL2.ChooseItem["Code"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chkSuperuser_CheckedChanged(object sender, EventArgs e)
	{
	}

	private void gv_Udf_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		Control_EditValueChanged(null, null);
	}

	public async Task REFRESH_DATA()
	{
		_ = 1;
		try
		{
			await LoadCb();
			await LoadData();
			txt_USER_CODE.ReadOnly = true;
			FormMode = Enums.FormViewModeType.Update;
			_dtBranchAssignment = new DataTable();
			btn_BranchAssignment.Enabled = false;
			if (grd_Udf.DataSource is DataTable dataTable)
			{
				dataTable.Rows.Clear();
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
		_ = 1;
		try
		{
			txt_USER_CODE.Text = string.Empty;
			txtUserName.Text = string.Empty;
			txtEMail.Text = string.Empty;
			txt_MobileIMEI.Text = string.Empty;
			txtPhone.Text = string.Empty;
			luk_Branch.EditValue = (short)(-2);
			luk_Department.EditValue = (short)(-2);
			chkSuperuser.Checked = false;
			chkLocked.Checked = false;
			txt_Password.Text = string.Empty;
			await LoadCbBranch();
			await LoadCbDepartment();
			FormMode = Enums.FormViewModeType.Add;
			txt_USER_CODE.ReadOnly = false;
			grdDoc.DataSource = new DataTable();
			grdDoc.RefreshDataSource();
			_dtBranchAssignment = new DataTable();
			btn_BranchAssignment.Enabled = true;
			txt_Fax.Text = "";
			txtDefault.Text = "";
			if (grd_Udf.DataSource is DataTable dataTable)
			{
				dataTable.Rows.Clear();
				dataTable.Rows.Add(dataTable.NewRow());
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SettingLanguage()
	{
		lb_UserCode.Utext = Resources.lb_UserCode;
		lb_UserName.Utext = Resources.lb_UserName;
		lb_Email.Utext = Resources.lb_Email;
		lb_Fax.Utext = Resources.lb_Fax;
		lb_MobilePhone.Utext = Resources.lb_MobilePhone;
		lb_Department.Utext = Resources.lb_Department;
		lb_Branch.Utext = Resources.lb_Branch;
		lb_Password.Utext = Resources.lb_Password;
		chkLocked.Text = Resources.lb_Locked;
		FormMode = Enums.FormViewModeType.View;
		gcUSER_CODE.Caption = Resources.OUSR_USER_CODE;
	}

	private async Task LoadData()
	{
		try
		{
			_isChangePassword = false;
			GridControlEx gridControlEx = grdDoc;
			gridControlEx.DataSource = await _httpClient.User.LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadCb()
	{
		_ = 2;
		try
		{
			await LoadCbDepartment();
			await LoadCbBranch();
			LoadCbLanguage();
			await LoadCbEmployee();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadCbBranch()
	{
		try
		{
			luk_Department.Properties.Columns.Clear();
			luk_Department.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
			DataTable dataSource = await _httpClient.User.ShowBranch();
			luk_Branch.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadCbEmployee()
	{
		try
		{
			luk_Employee.Properties.Columns.Clear();
			luk_Employee.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
			DataTable dataSource = await _httpClient.User.GetEmployee();
			luk_Employee.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadCbDepartment()
	{
		try
		{
			luk_Department.Properties.Columns.Clear();
			luk_Department.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
			DataTable dataSource = await _httpClient.User.ShowDepartment();
			luk_Department.Properties.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadCbLanguage()
	{
		try
		{
			luk_Language.Properties.Columns.Clear();
			luk_Language.Properties.Columns.Add(new LookUpColumnInfo("Name", ""));
			luk_Language.Properties.DisplayMember = "Name";
			luk_Language.Properties.ValueMember = "Code";
			DataTable dataTable = _httpClient.BusinessMaster.LoadComboLanguage();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				luk_Language.Properties.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void Reset()
	{
		_ = 2;
		try
		{
			txt_USER_CODE.Text = string.Empty;
			txtUserName.Text = string.Empty;
			txtEMail.Text = string.Empty;
			txt_MobileIMEI.Text = string.Empty;
			txtPhone.Text = string.Empty;
			luk_Branch.EditValue = (short)(-2);
			luk_Department.EditValue = (short)(-2);
			chkSuperuser.Checked = false;
			chkLocked.Checked = false;
			txt_Password.Text = string.Empty;
			txt_USER_CODE.ReadOnly = true;
			await LoadCbBranch();
			await LoadCbDepartment();
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
		}
	}

	private async Task LoadControl()
	{
		_isLoad = true;
		try
		{
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("USERID"));
			if (num > 0)
			{
				DataTable dataTable = await _httpClient.User.GetAnUser(num);
				if (dataTable == null || dataTable.Rows.Count == 0)
				{
					dataTable = WinFuntions.GetDataTableNewRow(new DataTable
					{
						TableName = "OUSR"
					});
				}
				DataSource = dataTable;
				DataTable dataTable2 = dataTable.Copy();
				dataTable2.Columns.Remove(dataTable2.Columns["U_NAME"]);
				for (int num2 = dataTable2.Columns.Count - 1; num2 >= 0; num2--)
				{
					if (!dataTable2.Columns[num2].ColumnName.StartsWith("U_"))
					{
						dataTable2.Columns.RemoveAt(num2);
					}
				}
				dataTable2.AcceptChanges();
				if (dataTable2.Columns.Count == 0)
				{
					dataTable2 = null;
				}
				grd_Udf.DataSource = dataTable2;
				FormMode = Enums.FormViewModeType.View;
				if (GlobalData.IsMultiBranch)
				{
					btn_BranchAssignment.Enabled = true;
					_dtBranchAssignment = await _httpClient.User.GetUserBranchAssignment(Function.ToString(txt_USER_CODE.Text));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoad = false;
	}

	private async Task Delete(int code)
	{
		_ = 1;
		try
		{
			string text = await _httpClient.User.DeleteDoc(code);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gvDoc.DeleteSelectedRows();
				await LoadControl();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetEnableControls(bool isEnable)
	{
		btOk.Enabled = isEnable;
		CheckEditEx checkEditEx = chkSuperuser;
		CheckEditEx checkEditEx2 = chk_MobileUser;
		CheckEditEx checkEditEx3 = chk_AllBrnchF;
		CheckEditEx checkEditEx4 = checkEditEx1;
		CheckEditEx checkEditEx5 = this.checkEditEx2;
		CheckEditEx checkEditEx6 = chk_IntgrtEb;
		bool flag = (chkLocked.ReadOnly = !isEnable);
		bool flag3 = (checkEditEx6.ReadOnly = flag);
		bool flag5 = (checkEditEx5.ReadOnly = flag3);
		bool flag7 = (checkEditEx4.ReadOnly = flag5);
		bool flag9 = (checkEditEx3.ReadOnly = flag7);
		bool readOnly = (checkEditEx2.ReadOnly = flag9);
		checkEditEx.ReadOnly = readOnly;
		TextEditEx textEditEx = textEditEx1;
		TextEditEx textEditEx2 = txtUserName;
		LookUpEditCombo lookUpEditCombo = luk_Employee;
		TextEditEx textEditEx3 = txtEMail;
		TextEditEx textEditEx4 = txtPhone;
		TextEditEx textEditEx5 = txt_MobileIMEI;
		TextEditEx textEditEx6 = txt_Fax;
		LookUpEditCombo lookUpEditCombo2 = luk_Department;
		TextEditEx textEditEx7 = txt_AuthGroup;
		LookUpEditCombo lookUpEditCombo3 = luk_Language;
		bool flag12 = (txt_Password.ReadOnly = !isEnable);
		bool flag14 = (lookUpEditCombo3.ReadOnly = flag12);
		bool flag16 = (textEditEx7.ReadOnly = flag14);
		bool flag18 = (lookUpEditCombo2.ReadOnly = flag16);
		flag = (textEditEx6.ReadOnly = flag18);
		flag3 = (textEditEx5.ReadOnly = flag);
		flag5 = (textEditEx4.ReadOnly = flag3);
		flag7 = (textEditEx3.ReadOnly = flag5);
		flag9 = (lookUpEditCombo.ReadOnly = flag7);
		readOnly = (textEditEx2.ReadOnly = flag9);
		textEditEx.ReadOnly = readOnly;
		GridControlEx gridControlEx = grdDoc;
		GridControlEx gridControlEx2 = grd_Udf;
		SimpleButtonEx simpleButtonEx = btn_BranchAssignment;
		SimpleButtonEx simpleButtonEx2 = btn_AuthorizationGroup;
		SimpleButtonEx simpleButtonEx3 = btn_ChangePassWord;
		flag3 = (txtDefault.Enabled = isEnable);
		flag5 = (simpleButtonEx3.Enabled = flag3);
		flag7 = (simpleButtonEx2.Enabled = flag5);
		flag9 = (simpleButtonEx.Enabled = flag7);
		readOnly = (gridControlEx2.Enabled = flag9);
		gridControlEx.Enabled = readOnly;
	}

	private async Task Save()
	{
		_ = 1;
		try
		{
			SetEnableControls(isEnable: false);
			WinCommons._OpenProcessing();
			UsersImpl doc = new UsersImpl();
			if (FormMode == Enums.FormViewModeType.Add)
			{
				doc.USERID = -1;
			}
			else
			{
				doc.USERID = Function.ParseInt(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcUSERID));
			}
			doc.empID = Function.ParseInt(luk_Employee.EditValue);
			if (_isChangePassword)
			{
				doc.IsChangePassword = true;
			}
			if (!string.IsNullOrEmpty(txt_Password.Text.Trim()))
			{
				if (!GlobalData.LoginSap)
				{
					doc.PASSWORD = Function.GetMd5Hash(txt_Password.Text.Trim());
				}
				else
				{
					doc.PASSWORD = txt_Password.Text.Trim();
				}
			}
			doc.USER_CODE = txt_USER_CODE.Text.Trim();
			doc.U_NAME = txtUserName.Text.Trim();
			doc.GROUPS = 0;
			if (chkSuperuser.Checked)
			{
				doc.SUPERUSER = "Y";
			}
			else
			{
				doc.SUPERUSER = "N";
			}
			doc.E_Mail = txtEMail.Text;
			doc.PortNum = txtPhone.Text;
			doc.Fax = txt_Fax.Text;
			if (chkLocked.Checked)
			{
				doc.Locked = "Y";
			}
			else
			{
				doc.Locked = "N";
			}
			doc.Department = Function.ParseInt(luk_Department.EditValue);
			doc.Branch = Function.ParseInt(luk_Branch.EditValue);
			doc.DfltsGroup = txtDefault.Text;
			doc.userSign = 3;
			doc.createDate = DateTime.Today;
			doc.userSign2 = 0;
			doc.updateDate = DateTime.Today;
			doc.lastLogin = DateTime.Today;
			DataTable tableStructure = WinFuntions.GetTableStructure("OPRM");
			for (int i = 0; i < GlobalData.AllModule.Rows.Count; i++)
			{
				string value = Function.ToString(GlobalData.AllModule.Rows[i]["Code"]);
				if (Function.ToString(GlobalData.AllModule.Rows[i]["MenuType"]) != "N")
				{
					DataRow dataRow = tableStructure.NewRow();
					dataRow["authoriza"] = "N";
					dataRow["menucode"] = value;
					dataRow["USER_CODE"] = doc.USER_CODE;
					tableStructure.Rows.Add(dataRow);
				}
			}
			tableStructure.AcceptChanges();
			doc.DTAuth = tableStructure;
			if (_dtBranchAssignment != null && _dtBranchAssignment.Rows.Count > 0)
			{
				doc.UserBranchAssignment = new List<UserBranchAssignmentImpls>();
				for (int j = 0; j < _dtBranchAssignment.Rows.Count; j++)
				{
					if (Function.ToString(_dtBranchAssignment.Rows[j]["Checked"]).ToLower() == "y")
					{
						UserBranchAssignmentImpls userBranchAssignmentImpls = new UserBranchAssignmentImpls();
						userBranchAssignmentImpls.BPLId = Function.ParseInt(_dtBranchAssignment.Rows[j]["BPLId"]);
						doc.UserBranchAssignment.Add(userBranchAssignmentImpls);
					}
				}
			}
			List<UserFieldsImpl> list = await WinFuntions.GetUdfFields("OUSR");
			if (list != null && list.Count > 0 && grd_Udf.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				DataRow dataRow2 = dataTable.Rows[0];
				doc.UserFields = new List<UserFieldsImpl>();
				for (int k = 0; k < list.Count; k++)
				{
					UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
					userFieldsImpl.Name = list[k].Name;
					userFieldsImpl.Value = dataRow2[list[k].Name];
					userFieldsImpl.Type = list[k].Type;
					doc.UserFields.Add(userFieldsImpl);
				}
			}
			string text = await _httpClient.User.SaveDoc(doc);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				return;
			}
			if (doc.USER_CODE == GlobalData.UserCodeLogin && RefreshMain != null)
			{
				RefreshMain();
			}
			WinCommons.Success(Enums.MessageCode.Success, "");
			if (FormMode == Enums.FormViewModeType.Add)
			{
				REFRESH();
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
			SetEnableControls(isEnable: true);
		}
	}

	private void gvDoc_RowStyle(object sender, RowStyleEventArgs e)
	{
		if (e.RowHandle == gvDoc.FocusedRowHandle)
		{
			e.Appearance.BackColor = StyleColor.GridEditting;
		}
	}

	private void SetButtonOkText()
	{
		try
		{
			if (!_isLoad && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_Users));
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.chk_IntgrtEb = new Apzon.Controls.Editor.CheckEditEx();
		this.txtDefault = new Apzon.Controls.Editor.ButtonEditEx();
		this.txt_AuthGroup = new Apzon.Controls.Editor.TextEditEx();
		this.btn_AuthorizationGroup = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_Language = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.chk_AllBrnchF = new Apzon.Controls.Editor.CheckEditEx();
		this.checkEditEx2 = new Apzon.Controls.Editor.CheckEditEx();
		this.checkEditEx1 = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_Fax = new Apzon.Controls.Editor.TextEditEx();
		this.luk_Employee = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_USER_CODE = new Apzon.Controls.Editor.TextEditEx();
		this.textEditEx1 = new Apzon.Controls.Editor.TextEditEx();
		this.txtUserName = new Apzon.Controls.Editor.TextEditEx();
		this.chk_MobileUser = new Apzon.Controls.Editor.CheckEditEx();
		this.txtEMail = new Apzon.Controls.Editor.TextEditEx();
		this.btn_BranchAssignment = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_MobileIMEI = new Apzon.Controls.Editor.TextEditEx();
		this.txtPhone = new Apzon.Controls.Editor.TextEditEx();
		this.luk_Department = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_Password = new Apzon.Controls.Editor.TextEditEx();
		this.luk_Branch = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.btn_ChangePassWord = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chkSuperuser = new Apzon.Controls.Editor.CheckEditEx();
		this.chkLocked = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Language = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.lb_Fax = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.lb_UserCode = new Apzon.Controls.Controls.LabelLine();
		this.lb_UserName = new Apzon.Controls.Controls.LabelLine();
		this.lb_Email = new Apzon.Controls.Controls.LabelLine();
		this.lb_MobileDeviceID = new Apzon.Controls.Controls.LabelLine();
		this.lb_MobilePhone = new Apzon.Controls.Controls.LabelLine();
		this.lbl_BranchAssignment = new Apzon.Controls.Controls.LabelLine();
		this.lb_Branch = new Apzon.Controls.Controls.LabelLine();
		this.lb_Department = new Apzon.Controls.Controls.LabelLine();
		this.lb_Password = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Udf = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Udf = new Apzon.Controls.Grid.GridViewEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuUser = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcUSER_CODE = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUSERID = new Apzon.Controls.Grid.GridColumnEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btOk = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_IntgrtEb.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDefault.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AuthGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Language.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllBrnchF.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkEditEx2.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkEditEx1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Employee.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_USER_CODE.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEditEx1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtUserName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_MobileUser.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtEMail.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MobileIMEI.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtPhone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Password.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkSuperuser.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkLocked.Properties).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Udf).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Udf).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuUser.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		base.SuspendLayout();
		this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.panelControl1.Controls.Add(this.xtraTabControl1);
		this.panelControl1.Controls.Add(this.grdDoc);
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(643, 543);
		this.panelControl1.TabIndex = 0;
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(200, 3);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(441, 528);
		this.xtraTabControl1.TabIndex = 72;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage1, this.xtraTabPage2 });
		this.xtraTabPage1.Controls.Add(this.chk_IntgrtEb);
		this.xtraTabPage1.Controls.Add(this.txtDefault);
		this.xtraTabPage1.Controls.Add(this.txt_AuthGroup);
		this.xtraTabPage1.Controls.Add(this.btn_AuthorizationGroup);
		this.xtraTabPage1.Controls.Add(this.luk_Language);
		this.xtraTabPage1.Controls.Add(this.chk_AllBrnchF);
		this.xtraTabPage1.Controls.Add(this.checkEditEx2);
		this.xtraTabPage1.Controls.Add(this.checkEditEx1);
		this.xtraTabPage1.Controls.Add(this.txt_Fax);
		this.xtraTabPage1.Controls.Add(this.luk_Employee);
		this.xtraTabPage1.Controls.Add(this.txt_USER_CODE);
		this.xtraTabPage1.Controls.Add(this.textEditEx1);
		this.xtraTabPage1.Controls.Add(this.txtUserName);
		this.xtraTabPage1.Controls.Add(this.chk_MobileUser);
		this.xtraTabPage1.Controls.Add(this.txtEMail);
		this.xtraTabPage1.Controls.Add(this.btn_BranchAssignment);
		this.xtraTabPage1.Controls.Add(this.txt_MobileIMEI);
		this.xtraTabPage1.Controls.Add(this.txtPhone);
		this.xtraTabPage1.Controls.Add(this.luk_Department);
		this.xtraTabPage1.Controls.Add(this.txt_Password);
		this.xtraTabPage1.Controls.Add(this.luk_Branch);
		this.xtraTabPage1.Controls.Add(this.btn_ChangePassWord);
		this.xtraTabPage1.Controls.Add(this.chkSuperuser);
		this.xtraTabPage1.Controls.Add(this.chkLocked);
		this.xtraTabPage1.Controls.Add(this.labelLine1);
		this.xtraTabPage1.Controls.Add(this.lbl_Language);
		this.xtraTabPage1.Controls.Add(this.labelLine5);
		this.xtraTabPage1.Controls.Add(this.lb_Fax);
		this.xtraTabPage1.Controls.Add(this.labelLine3);
		this.xtraTabPage1.Controls.Add(this.lb_UserCode);
		this.xtraTabPage1.Controls.Add(this.lb_UserName);
		this.xtraTabPage1.Controls.Add(this.lb_Email);
		this.xtraTabPage1.Controls.Add(this.lb_MobileDeviceID);
		this.xtraTabPage1.Controls.Add(this.lb_MobilePhone);
		this.xtraTabPage1.Controls.Add(this.lbl_BranchAssignment);
		this.xtraTabPage1.Controls.Add(this.lb_Branch);
		this.xtraTabPage1.Controls.Add(this.lb_Department);
		this.xtraTabPage1.Controls.Add(this.lb_Password);
		this.xtraTabPage1.Controls.Add(this.labelLine2);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Details";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(439, 503);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Details";
		this.chk_IntgrtEb.EditControl = null;
		this.chk_IntgrtEb.FieldName = "IntgrtEb";
		this.chk_IntgrtEb.IsSearchControl = false;
		this.chk_IntgrtEb.Location = new System.Drawing.Point(18, 415);
		this.chk_IntgrtEb.Name = "chk_IntgrtEb";
		this.chk_IntgrtEb.Properties.Caption = "En&able Setting Integration Packages";
		this.chk_IntgrtEb.Properties.DisplayValueChecked = "Y";
		this.chk_IntgrtEb.Properties.DisplayValueGrayed = "N";
		this.chk_IntgrtEb.Properties.DisplayValueUnchecked = "N";
		this.chk_IntgrtEb.Properties.ValueChecked = "Y";
		this.chk_IntgrtEb.Properties.ValueGrayed = "N";
		this.chk_IntgrtEb.Properties.ValueUnchecked = "N";
		this.chk_IntgrtEb.SettingInVisible = false;
		this.chk_IntgrtEb.SettingReadonly = false;
		this.chk_IntgrtEb.Size = new System.Drawing.Size(197, 20);
		this.chk_IntgrtEb.TabIndex = 71;
		this.chk_IntgrtEb.Visible = false;
		this.txtDefault.EditControl = null;
		this.txtDefault.FieldName = "DfltsGroup";
		this.txtDefault.IsSearchControl = false;
		this.txtDefault.Location = new System.Drawing.Point(212, 207);
		this.txtDefault.Name = "txtDefault";
		this.txtDefault.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txtDefault.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtDefault_Properties_ButtonPressed);
		this.txtDefault.SettingInVisible = false;
		this.txtDefault.SettingReadonly = false;
		this.txtDefault.Size = new System.Drawing.Size(194, 20);
		this.txtDefault.TabIndex = 70;
		this.txtDefault.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txt_AuthGroup.EditControl = null;
		this.txt_AuthGroup.Enabled = false;
		this.txt_AuthGroup.FieldName = null;
		this.txt_AuthGroup.IsSearchControl = false;
		this.txt_AuthGroup.Location = new System.Drawing.Point(212, 274);
		this.txt_AuthGroup.Name = "txt_AuthGroup";
		this.txt_AuthGroup.Properties.PasswordChar = '•';
		this.txt_AuthGroup.SettingInVisible = false;
		this.txt_AuthGroup.SettingReadonly = false;
		this.txt_AuthGroup.Size = new System.Drawing.Size(194, 20);
		this.txt_AuthGroup.TabIndex = 68;
		this.txt_AuthGroup.Visible = false;
		this.btn_AuthorizationGroup.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_AuthorizationGroup.EditControl = null;
		this.btn_AuthorizationGroup.EditValue = "...";
		this.btn_AuthorizationGroup.FieldName = null;
		this.btn_AuthorizationGroup.IsSearchControl = false;
		this.btn_AuthorizationGroup.Location = new System.Drawing.Point(408, 274);
		this.btn_AuthorizationGroup.Name = "btn_AuthorizationGroup";
		this.btn_AuthorizationGroup.ReadOnly = false;
		this.btn_AuthorizationGroup.SettingInVisible = false;
		this.btn_AuthorizationGroup.SettingReadonly = false;
		this.btn_AuthorizationGroup.Size = new System.Drawing.Size(22, 20);
		this.btn_AuthorizationGroup.TabIndex = 69;
		this.btn_AuthorizationGroup.Text = "...";
		this.btn_AuthorizationGroup.Visible = false;
		this.luk_Language.EditControl = null;
		this.luk_Language.FieldName = "Language";
		this.luk_Language.IsSearchControl = false;
		this.luk_Language.Location = new System.Drawing.Point(212, 296);
		this.luk_Language.Name = "luk_Language";
		this.luk_Language.Properties.AutoHeight = false;
		this.luk_Language.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Language.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Language.Properties.DisplayMember = "Name";
		this.luk_Language.Properties.IsSearchControl = false;
		this.luk_Language.Properties.NullText = "";
		this.luk_Language.Properties.ShowFooter = false;
		this.luk_Language.Properties.ShowHeader = false;
		this.luk_Language.Properties.ValueMember = "Code";
		this.luk_Language.SettingInVisible = false;
		this.luk_Language.SettingReadonly = false;
		this.luk_Language.Size = new System.Drawing.Size(194, 20);
		this.luk_Language.TabIndex = 66;
		this.luk_Language.Visible = false;
		this.chk_AllBrnchF.EditControl = null;
		this.chk_AllBrnchF.FieldName = "AllBrnchF";
		this.chk_AllBrnchF.IsSearchControl = false;
		this.chk_AllBrnchF.Location = new System.Drawing.Point(18, 438);
		this.chk_AllBrnchF.Name = "chk_AllBrnchF";
		this.chk_AllBrnchF.Properties.Caption = "Ignore Branch Assignment in Financial Reports";
		this.chk_AllBrnchF.Properties.DisplayValueChecked = "Y";
		this.chk_AllBrnchF.Properties.DisplayValueGrayed = "N";
		this.chk_AllBrnchF.Properties.DisplayValueUnchecked = "N";
		this.chk_AllBrnchF.Properties.ValueChecked = "Y";
		this.chk_AllBrnchF.Properties.ValueGrayed = "N";
		this.chk_AllBrnchF.Properties.ValueUnchecked = "N";
		this.chk_AllBrnchF.SettingInVisible = false;
		this.chk_AllBrnchF.SettingReadonly = false;
		this.chk_AllBrnchF.Size = new System.Drawing.Size(258, 20);
		this.chk_AllBrnchF.TabIndex = 64;
		this.chk_AllBrnchF.Visible = false;
		this.checkEditEx2.EditControl = null;
		this.checkEditEx2.FieldName = "PwdNeverEx";
		this.checkEditEx2.IsSearchControl = false;
		this.checkEditEx2.Location = new System.Drawing.Point(18, 346);
		this.checkEditEx2.Name = "checkEditEx2";
		this.checkEditEx2.Properties.Caption = "Password Never Expires";
		this.checkEditEx2.Properties.DisplayValueChecked = "Y";
		this.checkEditEx2.Properties.DisplayValueGrayed = "N";
		this.checkEditEx2.Properties.DisplayValueUnchecked = "N";
		this.checkEditEx2.Properties.ValueChecked = "Y";
		this.checkEditEx2.Properties.ValueGrayed = "N";
		this.checkEditEx2.Properties.ValueUnchecked = "N";
		this.checkEditEx2.SettingInVisible = false;
		this.checkEditEx2.SettingReadonly = false;
		this.checkEditEx2.Size = new System.Drawing.Size(142, 20);
		this.checkEditEx2.TabIndex = 63;
		this.checkEditEx2.Visible = false;
		this.checkEditEx2.CheckedChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.checkEditEx1.EditControl = null;
		this.checkEditEx1.FieldName = "OneLogPwd";
		this.checkEditEx1.IsSearchControl = false;
		this.checkEditEx1.Location = new System.Drawing.Point(18, 369);
		this.checkEditEx1.Name = "checkEditEx1";
		this.checkEditEx1.Properties.Caption = "Change Password at Next Logon";
		this.checkEditEx1.Properties.DisplayValueChecked = "Y";
		this.checkEditEx1.Properties.DisplayValueGrayed = "N";
		this.checkEditEx1.Properties.DisplayValueUnchecked = "N";
		this.checkEditEx1.Properties.ValueChecked = "Y";
		this.checkEditEx1.Properties.ValueGrayed = "N";
		this.checkEditEx1.Properties.ValueUnchecked = "N";
		this.checkEditEx1.SettingInVisible = false;
		this.checkEditEx1.SettingReadonly = false;
		this.checkEditEx1.Size = new System.Drawing.Size(178, 20);
		this.checkEditEx1.TabIndex = 62;
		this.checkEditEx1.Visible = false;
		this.checkEditEx1.CheckedChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txt_Fax.EditControl = null;
		this.txt_Fax.FieldName = "Fax";
		this.txt_Fax.IsSearchControl = false;
		this.txt_Fax.Location = new System.Drawing.Point(212, 185);
		this.txt_Fax.Name = "txt_Fax";
		this.txt_Fax.SettingInVisible = false;
		this.txt_Fax.SettingReadonly = false;
		this.txt_Fax.Size = new System.Drawing.Size(194, 20);
		this.txt_Fax.TabIndex = 61;
		this.txt_Fax.EditValueChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.luk_Employee.EditControl = null;
		this.luk_Employee.Enabled = false;
		this.luk_Employee.FieldName = "empID";
		this.luk_Employee.IsSearchControl = false;
		this.luk_Employee.Location = new System.Drawing.Point(212, 97);
		this.luk_Employee.Name = "luk_Employee";
		this.luk_Employee.Properties.AutoHeight = false;
		this.luk_Employee.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Employee.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Employee.Properties.DisplayMember = "Name";
		this.luk_Employee.Properties.IsSearchControl = false;
		this.luk_Employee.Properties.NullText = "";
		this.luk_Employee.Properties.ShowFooter = false;
		this.luk_Employee.Properties.ShowHeader = false;
		this.luk_Employee.Properties.ValueMember = "Code";
		this.luk_Employee.SettingInVisible = false;
		this.luk_Employee.SettingReadonly = false;
		this.luk_Employee.Size = new System.Drawing.Size(194, 20);
		this.luk_Employee.TabIndex = 59;
		this.txt_USER_CODE.EditControl = null;
		this.txt_USER_CODE.FieldName = "USER_CODE";
		this.txt_USER_CODE.IsSearchControl = false;
		this.txt_USER_CODE.Location = new System.Drawing.Point(212, 31);
		this.txt_USER_CODE.Name = "txt_USER_CODE";
		this.txt_USER_CODE.SettingInVisible = false;
		this.txt_USER_CODE.SettingReadonly = false;
		this.txt_USER_CODE.Size = new System.Drawing.Size(194, 20);
		this.txt_USER_CODE.TabIndex = 1;
		this.textEditEx1.EditControl = null;
		this.textEditEx1.FieldName = null;
		this.textEditEx1.IsSearchControl = false;
		this.textEditEx1.Location = new System.Drawing.Point(212, 53);
		this.textEditEx1.Name = "textEditEx1";
		this.textEditEx1.SettingInVisible = false;
		this.textEditEx1.SettingReadonly = false;
		this.textEditEx1.Size = new System.Drawing.Size(194, 20);
		this.textEditEx1.TabIndex = 57;
		this.textEditEx1.Visible = false;
		this.txtUserName.EditControl = null;
		this.txtUserName.FieldName = "U_NAME";
		this.txtUserName.IsSearchControl = false;
		this.txtUserName.Location = new System.Drawing.Point(212, 75);
		this.txtUserName.Name = "txtUserName";
		this.txtUserName.SettingInVisible = false;
		this.txtUserName.SettingReadonly = false;
		this.txtUserName.Size = new System.Drawing.Size(194, 20);
		this.txtUserName.TabIndex = 2;
		this.txtUserName.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanged);
		this.chk_MobileUser.EditControl = null;
		this.chk_MobileUser.FieldName = "MobileUser";
		this.chk_MobileUser.IsSearchControl = false;
		this.chk_MobileUser.Location = new System.Drawing.Point(109, 8);
		this.chk_MobileUser.Name = "chk_MobileUser";
		this.chk_MobileUser.Properties.Caption = "Mobile User";
		this.chk_MobileUser.Properties.DisplayValueChecked = "Y";
		this.chk_MobileUser.Properties.DisplayValueGrayed = "N";
		this.chk_MobileUser.Properties.DisplayValueUnchecked = "N";
		this.chk_MobileUser.Properties.ValueChecked = "Y";
		this.chk_MobileUser.Properties.ValueGrayed = "N";
		this.chk_MobileUser.Properties.ValueUnchecked = "N";
		this.chk_MobileUser.SettingInVisible = false;
		this.chk_MobileUser.SettingReadonly = false;
		this.chk_MobileUser.Size = new System.Drawing.Size(75, 20);
		this.chk_MobileUser.TabIndex = 55;
		this.chk_MobileUser.Visible = false;
		this.chk_MobileUser.CheckedChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.txtEMail.EditControl = null;
		this.txtEMail.FieldName = "E_Mail";
		this.txtEMail.IsSearchControl = false;
		this.txtEMail.Location = new System.Drawing.Point(212, 119);
		this.txtEMail.Name = "txtEMail";
		this.txtEMail.SettingInVisible = false;
		this.txtEMail.SettingReadonly = false;
		this.txtEMail.Size = new System.Drawing.Size(194, 20);
		this.txtEMail.TabIndex = 3;
		this.txtEMail.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanged);
		this.txtEMail.Validating += new System.ComponentModel.CancelEventHandler(this.txtEMail_Validating);
		this.btn_BranchAssignment.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_BranchAssignment.EditControl = null;
		this.btn_BranchAssignment.EditValue = "...";
		this.btn_BranchAssignment.FieldName = null;
		this.btn_BranchAssignment.IsSearchControl = false;
		this.btn_BranchAssignment.Location = new System.Drawing.Point(212, 229);
		this.btn_BranchAssignment.Name = "btn_BranchAssignment";
		this.btn_BranchAssignment.ReadOnly = false;
		this.btn_BranchAssignment.SettingInVisible = false;
		this.btn_BranchAssignment.SettingReadonly = false;
		this.btn_BranchAssignment.Size = new System.Drawing.Size(22, 20);
		this.btn_BranchAssignment.TabIndex = 54;
		this.btn_BranchAssignment.Text = "...";
		this.btn_BranchAssignment.Click += new System.EventHandler(this.btn_BranchAssignment_Click);
		this.txt_MobileIMEI.EditControl = null;
		this.txt_MobileIMEI.FieldName = "MobileIMEI";
		this.txt_MobileIMEI.IsSearchControl = false;
		this.txt_MobileIMEI.Location = new System.Drawing.Point(212, 163);
		this.txt_MobileIMEI.Name = "txt_MobileIMEI";
		this.txt_MobileIMEI.SettingInVisible = false;
		this.txt_MobileIMEI.SettingReadonly = false;
		this.txt_MobileIMEI.Size = new System.Drawing.Size(194, 20);
		this.txt_MobileIMEI.TabIndex = 4;
		this.txt_MobileIMEI.Visible = false;
		this.txt_MobileIMEI.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanged);
		this.txtPhone.EditControl = null;
		this.txtPhone.FieldName = "PortNum";
		this.txtPhone.IsSearchControl = false;
		this.txtPhone.Location = new System.Drawing.Point(212, 141);
		this.txtPhone.Name = "txtPhone";
		this.txtPhone.SettingInVisible = false;
		this.txtPhone.SettingReadonly = false;
		this.txtPhone.Size = new System.Drawing.Size(194, 20);
		this.txtPhone.TabIndex = 5;
		this.txtPhone.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanged);
		this.luk_Department.EditControl = null;
		this.luk_Department.FieldName = "Department";
		this.luk_Department.IsSearchControl = false;
		this.luk_Department.Location = new System.Drawing.Point(212, 252);
		this.luk_Department.Name = "luk_Department";
		this.luk_Department.Properties.AutoHeight = false;
		this.luk_Department.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Department.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Department.Properties.DisplayMember = "Name";
		this.luk_Department.Properties.IsSearchControl = false;
		this.luk_Department.Properties.NullText = "";
		this.luk_Department.Properties.ShowFooter = false;
		this.luk_Department.Properties.ShowHeader = false;
		this.luk_Department.Properties.ValueMember = "Code";
		this.luk_Department.SettingInVisible = false;
		this.luk_Department.SettingReadonly = false;
		this.luk_Department.Size = new System.Drawing.Size(194, 20);
		this.luk_Department.TabIndex = 51;
		this.luk_Department.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanged);
		this.txt_Password.EditControl = null;
		this.txt_Password.Enabled = false;
		this.txt_Password.FieldName = "";
		this.txt_Password.IsSearchControl = false;
		this.txt_Password.Location = new System.Drawing.Point(212, 318);
		this.txt_Password.Name = "txt_Password";
		this.txt_Password.Properties.PasswordChar = '•';
		this.txt_Password.SettingInVisible = false;
		this.txt_Password.SettingReadonly = false;
		this.txt_Password.Size = new System.Drawing.Size(194, 20);
		this.txt_Password.TabIndex = 10;
		this.txt_Password.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanged);
		this.luk_Branch.EditControl = null;
		this.luk_Branch.FieldName = "Branch";
		this.luk_Branch.IsSearchControl = false;
		this.luk_Branch.Location = new System.Drawing.Point(221, 476);
		this.luk_Branch.Name = "luk_Branch";
		this.luk_Branch.Properties.AutoHeight = false;
		this.luk_Branch.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Branch.Properties.DisplayMember = "Name";
		this.luk_Branch.Properties.IsSearchControl = false;
		this.luk_Branch.Properties.NullText = "";
		this.luk_Branch.Properties.ShowFooter = false;
		this.luk_Branch.Properties.ShowHeader = false;
		this.luk_Branch.Properties.ValueMember = "Code";
		this.luk_Branch.SettingInVisible = false;
		this.luk_Branch.SettingReadonly = false;
		this.luk_Branch.Size = new System.Drawing.Size(194, 20);
		this.luk_Branch.TabIndex = 50;
		this.luk_Branch.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.Control_EditValueChanged);
		this.btn_ChangePassWord.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_ChangePassWord.EditControl = null;
		this.btn_ChangePassWord.EditValue = "...";
		this.btn_ChangePassWord.FieldName = null;
		this.btn_ChangePassWord.IsSearchControl = false;
		this.btn_ChangePassWord.Location = new System.Drawing.Point(408, 318);
		this.btn_ChangePassWord.Name = "btn_ChangePassWord";
		this.btn_ChangePassWord.ReadOnly = false;
		this.btn_ChangePassWord.SettingInVisible = false;
		this.btn_ChangePassWord.SettingReadonly = false;
		this.btn_ChangePassWord.Size = new System.Drawing.Size(22, 20);
		this.btn_ChangePassWord.TabIndex = 49;
		this.btn_ChangePassWord.Text = "...";
		this.btn_ChangePassWord.Click += new System.EventHandler(this.btn_ChangePassWord_Click);
		this.chkSuperuser.EditControl = null;
		this.chkSuperuser.FieldName = "SUPERUSER";
		this.chkSuperuser.IsSearchControl = false;
		this.chkSuperuser.Location = new System.Drawing.Point(18, 8);
		this.chkSuperuser.Name = "chkSuperuser";
		this.chkSuperuser.Properties.Caption = "Superuser";
		this.chkSuperuser.Properties.Tag = "";
		this.chkSuperuser.Properties.ValueChecked = "Y";
		this.chkSuperuser.Properties.ValueGrayed = "N";
		this.chkSuperuser.Properties.ValueUnchecked = "N";
		this.chkSuperuser.SettingInVisible = false;
		this.chkSuperuser.SettingReadonly = false;
		this.chkSuperuser.Size = new System.Drawing.Size(75, 20);
		this.chkSuperuser.TabIndex = 12;
		this.chkSuperuser.CheckedChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.chkLocked.EditControl = null;
		this.chkLocked.FieldName = "Locked";
		this.chkLocked.IsSearchControl = false;
		this.chkLocked.Location = new System.Drawing.Point(18, 392);
		this.chkLocked.Name = "chkLocked";
		this.chkLocked.Properties.Caption = "Locked";
		this.chkLocked.Properties.DisplayValueChecked = "Y";
		this.chkLocked.Properties.DisplayValueGrayed = "N";
		this.chkLocked.Properties.DisplayValueUnchecked = "N";
		this.chkLocked.Properties.ValueChecked = "Y";
		this.chkLocked.Properties.ValueGrayed = "N";
		this.chkLocked.Properties.ValueUnchecked = "N";
		this.chkLocked.SettingInVisible = false;
		this.chkLocked.SettingReadonly = false;
		this.chkLocked.Size = new System.Drawing.Size(75, 20);
		this.chkLocked.TabIndex = 13;
		this.chkLocked.CheckedChanged += new System.EventHandler(this.Control_EditValueChanged);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Bind with Microsoft Windows Account";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(23, 56);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(197, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 56;
		this.labelLine1.Utext = "Bind with Microsoft Windows Account";
		this.labelLine1.Visible = false;
		this.lbl_Language.EditControl = null;
		this.lbl_Language.EditValue = "Language";
		this.lbl_Language.FieldName = null;
		this.lbl_Language.HideWhenEmpty = false;
		this.lbl_Language.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Language.Location = new System.Drawing.Point(23, 299);
		this.lbl_Language.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_Language.Name = "lbl_Language";
		this.lbl_Language.ReadOnly = false;
		this.lbl_Language.SettingInVisible = false;
		this.lbl_Language.SettingReadonly = false;
		this.lbl_Language.ShowUnderLine = true;
		this.lbl_Language.Size = new System.Drawing.Size(203, 20);
		this.lbl_Language.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Language.TabIndex = 65;
		this.lbl_Language.Utext = "Language";
		this.lbl_Language.Visible = false;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Authorization Groups";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(23, 277);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(203, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 67;
		this.labelLine5.Utext = "Authorization Groups";
		this.labelLine5.Visible = false;
		this.lb_Fax.EditControl = null;
		this.lb_Fax.EditValue = "Fax";
		this.lb_Fax.FieldName = null;
		this.lb_Fax.HideWhenEmpty = false;
		this.lb_Fax.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Fax.Location = new System.Drawing.Point(23, 188);
		this.lb_Fax.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Fax.Name = "lb_Fax";
		this.lb_Fax.ReadOnly = false;
		this.lb_Fax.SettingInVisible = false;
		this.lb_Fax.SettingReadonly = false;
		this.lb_Fax.ShowUnderLine = true;
		this.lb_Fax.Size = new System.Drawing.Size(197, 20);
		this.lb_Fax.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Fax.TabIndex = 60;
		this.lb_Fax.Utext = "Fax";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Employee";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(23, 100);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(203, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 58;
		this.labelLine3.Utext = "Employee";
		this.lb_UserCode.EditControl = null;
		this.lb_UserCode.EditValue = "User Code";
		this.lb_UserCode.FieldName = null;
		this.lb_UserCode.HideWhenEmpty = false;
		this.lb_UserCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_UserCode.Location = new System.Drawing.Point(23, 34);
		this.lb_UserCode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_UserCode.Name = "lb_UserCode";
		this.lb_UserCode.ReadOnly = false;
		this.lb_UserCode.SettingInVisible = false;
		this.lb_UserCode.SettingReadonly = false;
		this.lb_UserCode.ShowUnderLine = true;
		this.lb_UserCode.Size = new System.Drawing.Size(197, 20);
		this.lb_UserCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_UserCode.TabIndex = 0;
		this.lb_UserCode.Utext = "User Code";
		this.lb_UserName.EditControl = null;
		this.lb_UserName.EditValue = "User Name";
		this.lb_UserName.FieldName = null;
		this.lb_UserName.HideWhenEmpty = false;
		this.lb_UserName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_UserName.Location = new System.Drawing.Point(23, 78);
		this.lb_UserName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_UserName.Name = "lb_UserName";
		this.lb_UserName.ReadOnly = false;
		this.lb_UserName.SettingInVisible = false;
		this.lb_UserName.SettingReadonly = false;
		this.lb_UserName.ShowUnderLine = true;
		this.lb_UserName.Size = new System.Drawing.Size(197, 20);
		this.lb_UserName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_UserName.TabIndex = 0;
		this.lb_UserName.Utext = "User Name";
		this.lb_Email.EditControl = null;
		this.lb_Email.EditValue = "E-Mail";
		this.lb_Email.FieldName = null;
		this.lb_Email.HideWhenEmpty = false;
		this.lb_Email.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Email.Location = new System.Drawing.Point(23, 122);
		this.lb_Email.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Email.Name = "lb_Email";
		this.lb_Email.ReadOnly = false;
		this.lb_Email.SettingInVisible = false;
		this.lb_Email.SettingReadonly = false;
		this.lb_Email.ShowUnderLine = true;
		this.lb_Email.Size = new System.Drawing.Size(197, 20);
		this.lb_Email.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Email.TabIndex = 0;
		this.lb_Email.Utext = "E-Mail";
		this.lb_MobileDeviceID.EditControl = null;
		this.lb_MobileDeviceID.EditValue = "Mobile Device ID";
		this.lb_MobileDeviceID.FieldName = null;
		this.lb_MobileDeviceID.HideWhenEmpty = false;
		this.lb_MobileDeviceID.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_MobileDeviceID.Location = new System.Drawing.Point(23, 166);
		this.lb_MobileDeviceID.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_MobileDeviceID.Name = "lb_MobileDeviceID";
		this.lb_MobileDeviceID.ReadOnly = false;
		this.lb_MobileDeviceID.SettingInVisible = false;
		this.lb_MobileDeviceID.SettingReadonly = false;
		this.lb_MobileDeviceID.ShowUnderLine = true;
		this.lb_MobileDeviceID.Size = new System.Drawing.Size(197, 20);
		this.lb_MobileDeviceID.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_MobileDeviceID.TabIndex = 0;
		this.lb_MobileDeviceID.Utext = "Mobile Device ID";
		this.lb_MobileDeviceID.Visible = false;
		this.lb_MobilePhone.EditControl = null;
		this.lb_MobilePhone.EditValue = "Mobile Phone";
		this.lb_MobilePhone.FieldName = null;
		this.lb_MobilePhone.HideWhenEmpty = false;
		this.lb_MobilePhone.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_MobilePhone.Location = new System.Drawing.Point(23, 144);
		this.lb_MobilePhone.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_MobilePhone.Name = "lb_MobilePhone";
		this.lb_MobilePhone.ReadOnly = false;
		this.lb_MobilePhone.SettingInVisible = false;
		this.lb_MobilePhone.SettingReadonly = false;
		this.lb_MobilePhone.ShowUnderLine = true;
		this.lb_MobilePhone.Size = new System.Drawing.Size(197, 20);
		this.lb_MobilePhone.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_MobilePhone.TabIndex = 0;
		this.lb_MobilePhone.Utext = "Mobile Phone";
		this.lbl_BranchAssignment.EditControl = null;
		this.lbl_BranchAssignment.EditValue = "Branch Assignment";
		this.lbl_BranchAssignment.FieldName = null;
		this.lbl_BranchAssignment.HideWhenEmpty = false;
		this.lbl_BranchAssignment.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_BranchAssignment.Location = new System.Drawing.Point(23, 232);
		this.lbl_BranchAssignment.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbl_BranchAssignment.Name = "lbl_BranchAssignment";
		this.lbl_BranchAssignment.ReadOnly = false;
		this.lbl_BranchAssignment.SettingInVisible = false;
		this.lbl_BranchAssignment.SettingReadonly = false;
		this.lbl_BranchAssignment.ShowUnderLine = true;
		this.lbl_BranchAssignment.Size = new System.Drawing.Size(203, 20);
		this.lbl_BranchAssignment.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_BranchAssignment.TabIndex = 52;
		this.lbl_BranchAssignment.Utext = "Branch Assignment";
		this.lb_Branch.EditControl = null;
		this.lb_Branch.EditValue = "Branch";
		this.lb_Branch.FieldName = null;
		this.lb_Branch.HideWhenEmpty = false;
		this.lb_Branch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Branch.Location = new System.Drawing.Point(32, 479);
		this.lb_Branch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Branch.Name = "lb_Branch";
		this.lb_Branch.ReadOnly = false;
		this.lb_Branch.SettingInVisible = false;
		this.lb_Branch.SettingReadonly = false;
		this.lb_Branch.ShowUnderLine = true;
		this.lb_Branch.Size = new System.Drawing.Size(203, 20);
		this.lb_Branch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Branch.TabIndex = 0;
		this.lb_Branch.Utext = "Branch";
		this.lb_Department.EditControl = null;
		this.lb_Department.EditValue = "Department";
		this.lb_Department.FieldName = null;
		this.lb_Department.HideWhenEmpty = false;
		this.lb_Department.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Department.Location = new System.Drawing.Point(23, 255);
		this.lb_Department.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Department.Name = "lb_Department";
		this.lb_Department.ReadOnly = false;
		this.lb_Department.SettingInVisible = false;
		this.lb_Department.SettingReadonly = false;
		this.lb_Department.ShowUnderLine = true;
		this.lb_Department.Size = new System.Drawing.Size(203, 20);
		this.lb_Department.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Department.TabIndex = 0;
		this.lb_Department.Utext = "Department";
		this.lb_Password.EditControl = null;
		this.lb_Password.EditValue = "Password";
		this.lb_Password.FieldName = null;
		this.lb_Password.HideWhenEmpty = false;
		this.lb_Password.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Password.Location = new System.Drawing.Point(23, 321);
		this.lb_Password.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_Password.Name = "lb_Password";
		this.lb_Password.ReadOnly = false;
		this.lb_Password.SettingInVisible = false;
		this.lb_Password.SettingReadonly = false;
		this.lb_Password.ShowUnderLine = true;
		this.lb_Password.Size = new System.Drawing.Size(203, 20);
		this.lb_Password.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Password.TabIndex = 0;
		this.lb_Password.Utext = "Password";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Default";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(23, 210);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(203, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 0;
		this.labelLine2.Utext = "Default";
		this.xtraTabPage2.Controls.Add(this.grd_Udf);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "User Defined Fields";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(439, 503);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "User Defined Fields";
		this.grd_Udf.AllowPasteExcel = false;
		this.grd_Udf.ClipboardData = null;
		this.grd_Udf.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Udf.Location = new System.Drawing.Point(0, 0);
		this.grd_Udf.MainView = this.gv_Udf;
		this.grd_Udf.Name = "grd_Udf";
		this.grd_Udf.Size = new System.Drawing.Size(439, 503);
		this.grd_Udf.TabIndex = 0;
		this.grd_Udf.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Udf });
		this.gv_Udf.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Udf.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Udf.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Udf.ColumnKey = null;
		this.gv_Udf.DisableFormSetting = false;
		this.gv_Udf.DisplayException = false;
		this.gv_Udf.EditColumn = null;
		this.gv_Udf.FieldCurrency = null;
		this.gv_Udf.FieldRate = null;
		this.gv_Udf.FormId = "oUser";
		this.gv_Udf.GridControl = this.grd_Udf;
		this.gv_Udf.HasFormSetting = true;
		this.gv_Udf.IgnoreFilter = false;
		this.gv_Udf.IndexEditRow = -1;
		this.gv_Udf.IndicatorWidth = 35;
		this.gv_Udf.IsApzTable = false;
		this.gv_Udf.IsInsertNewRow = false;
		this.gv_Udf.IsLoadingData = false;
		this.gv_Udf.IsMappingContextAction = false;
		this.gv_Udf.ItemId = "OUDF";
		this.gv_Udf.LstColumnAllowBatch = null;
		this.gv_Udf.LstColumnNotAllowBatch = null;
		this.gv_Udf.LstColumnNotEditable = null;
		this.gv_Udf.LstColumNotSetting = null;
		this.gv_Udf.LstOldUDF = null;
		this.gv_Udf.Name = "gv_Udf";
		this.gv_Udf.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Udf.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Udf.OptionsView.ColumnAutoWidth = false;
		this.gv_Udf.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Udf.OptionsView.ShowGroupPanel = false;
		this.gv_Udf.ReadOnly = false;
		this.gv_Udf.SettingReadonly = false;
		this.gv_Udf.UseBatchMod = false;
		this.gv_Udf.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Udf_CellValueChanged);
		this.grdDoc.AllowPasteExcel = false;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuUser;
		this.grdDoc.Location = new System.Drawing.Point(12, 12);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.Size = new System.Drawing.Size(182, 515);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuUser.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuUser.Name = "contextMenuUser";
		this.contextMenuUser.Size = new System.Drawing.Size(108, 48);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(107, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcUSER_CODE, this.gcUSERID });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oUser";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.HasFormSetting = true;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OUSR";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.LstOldUDF = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.Editable = false;
		this.gvDoc.OptionsBehavior.ReadOnly = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.ReadOnly = true;
		this.gvDoc.SettingReadonly = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvDoc_RowStyle);
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvDoc_BeforeLeaveRow);
		this.gcUSER_CODE.Caption = "User Code";
		this.gcUSER_CODE.CurrencyDateField = null;
		this.gcUSER_CODE.CurrencyField = null;
		this.gcUSER_CODE.CurrencyFixed = null;
		this.gcUSER_CODE.CurrencyFixedRate = 0.0;
		this.gcUSER_CODE.CurrencyRateField = null;
		this.gcUSER_CODE.FieldName = "USER_CODE";
		this.gcUSER_CODE.Name = "gcUSER_CODE";
		this.gcUSER_CODE.OptionsColumn.ReadOnly = true;
		this.gcUSER_CODE.PrimaryKey = false;
		this.gcUSER_CODE.UnitField = null;
		this.gcUSER_CODE.Visible = true;
		this.gcUSER_CODE.VisibleIndex = 0;
		this.gcUSER_CODE.Width = 91;
		this.gcUSERID.Caption = "User ID";
		this.gcUSERID.CurrencyDateField = null;
		this.gcUSERID.CurrencyField = null;
		this.gcUSERID.CurrencyFixed = null;
		this.gcUSERID.CurrencyFixedRate = 0.0;
		this.gcUSERID.CurrencyRateField = null;
		this.gcUSERID.FieldName = "USERID";
		this.gcUSERID.Name = "gcUSERID";
		this.gcUSERID.PrimaryKey = false;
		this.gcUSERID.UnitField = null;
		this.gcUSERID.Width = 54;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(81, 549);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 15;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btOk.EditControl = null;
		this.btOk.EditValue = "OK";
		this.btOk.FieldName = null;
		this.btOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btOk.ImageOptions.Image");
		this.btOk.IsSearchControl = false;
		this.btOk.Location = new System.Drawing.Point(6, 549);
		this.btOk.Name = "btOk";
		this.btOk.ReadOnly = false;
		this.btOk.SettingInVisible = false;
		this.btOk.SettingReadonly = false;
		this.btOk.Size = new System.Drawing.Size(72, 20);
		this.btOk.TabIndex = 14;
		this.btOk.Text = "OK";
		this.btOk.Click += new System.EventHandler(this.btOk_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(643, 574);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btOk);
		base.Controls.Add(this.panelControl1);
		base.FormId = "oUser";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Users";
		this.Text = "Users";
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_IntgrtEb.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDefault.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_AuthGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Language.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_AllBrnchF.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkEditEx2.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkEditEx1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Employee.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_USER_CODE.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEditEx1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtUserName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_MobileUser.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtEMail.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MobileIMEI.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtPhone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Password.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkSuperuser.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkLocked.Properties).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Udf).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Udf).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuUser.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		base.ResumeLayout(false);
	}
}
