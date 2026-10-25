using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Commons.Helper;
using DevExpress.Data;
using DevExpress.Data.Filtering;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.HumanResources;

public class frm_HumanResources : XtraFormEx
{
	private enum AutoCompleteFlag
	{
		Auto = 1,
		BackToOldText = 2,
		SetEmpty = 3,
		None = 0
	}

	public Action<DataTable> SendFromParentDelegate;

	public int EmpId;

	private string _empName;

	private string _strImage;

	private string _cloudImage;

	private string _picFilePath = "";

	private bool _edit;

	private bool _forceClose;

	private bool _add;

	private readonly FtpHelper _ftpHelper = new FtpHelper();

	private DataTable _dtAbsence;

	private DataTable _dtEducation;

	private DataTable _dtReview;

	private DataTable _dtPrevEmp;

	private DataTable _dtHealth;

	private DataTable _dtContract;

	private Dictionary<string, Control> _dicControls;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly List<int> _listRole = new List<int>();

	private readonly List<int> _listTeam = new List<int>();

	private bool isPressed;

	private bool fromCFL;

	private bool isTab;

	private bool _isChanging;

	private bool _isCancel;

	private string autoName = "";

	private int autoNumber;

	private int _defaultRole;

	private DataTable dtOHEM;

	private Dictionary<int, string> dicOHEM;

	private Dictionary<int, string> dicButtonEditValueType = new Dictionary<int, string>();

	private List<int> listContract = new List<int>();

	private AutoCompleteFlag _autoCompleteFlag;

	private List<CurrencyCodesImpl> salaryCurrData;

	public bool isCalledFromOutside;

	private bool _isLoading;

	private bool _isRefresh;

	private IContainer components;

	private LabelLine lbl_FirstName;

	private LabelLine lbl_Department;

	private LabelLine lbl_Position;

	private LabelLine lbl_Job;

	private LabelLine lbl_LastName;

	private LabelLine lbl_MiddleName;

	private LabelLine lbl_UserCode;

	private LabelLine lbl_Manager;

	private LabelLine lbl_Branch;

	private LabelLine lbl_EmpNo;

	private LabelLine lbl_Pager;

	private LabelLine lbl_MPhone;

	private LabelLine lbl_ext;

	private LabelLine lbl_off;

	private LabelLine lbl_ExtEmp;

	private LabelLine lbl_Home;

	private LabelLine lbl_Fax;

	private TextEditEx txt_FirstName;

	private TextEditEx txt_MiddleName;

	private TextEditEx txt_LastName;

	private TextEditEx txt_JobTitle;

	private LabelLine lbl_SalesEmp;

	private LabelLine lbl_Email;

	private TextEditEx txt_EmpNo;

	private TextEditEx txt_ExtEmpNo;

	private CheckEditEx chk_Active;

	private TextEditEx txt_OfficePhone;

	private TextEditEx txt_Pager;

	private TextEditEx txt_MobilePhone;

	private TextEditEx txt_ExtPhone;

	private TextEditEx txt_Email;

	private TextEditEx txt_Fax;

	private TextEditEx txt_HomePhone;

	private XtraTabControlEx tab_Detail;

	private XtraTabPageEx tab_Address;

	private XtraTabPageEx tab_Membership;

	private XtraTabPageEx tab_Personal;

	private XtraTabPageEx tab_Remarks;

	private XtraTabPageEx tab_Attachments;

	private XtraTabPageEx tab_User;

	private PictureEdit pic_Images;

	private PanelControl panelControl2;

	private PanelControl pnl_Personal;

	private MemoEditEx txt_Remark;

	private PanelControl pnl_Address;

	private TextEditEx txt_H_Block;

	private TextEditEx txt_H_StreetNo;

	private TextEditEx txt_H_Street;

	private LabelLine lbl_H_StrNo;

	private LabelLine lbl_H_Block;

	private LabelLine lbl_H_Street;

	private TextEditEx txt_W_Street;

	private TextEditEx txt_W_Block;

	private LabelLine lbl_WStreet;

	private TextEditEx txt_W_StreetNo;

	private LabelLine lbl_StrNo;

	private LabelLine lbl_Block;

	private TextEditEx txt_H_County;

	private TextEditEx txt_H_City;

	private TextEditEx txt_H_Zipcode;

	private TextEditEx txt_H_Build;

	private LabelLine lbl_H_zipCode;

	private LabelLine lbl_H_city;

	private LabelLine lbl_H_County;

	private LabelLine lbl_H_State;

	private LabelLine lbl_H_Country;

	private LabelLine lbl_H_Build;

	private LabelLine lbl_WorkAdress;

	private LabelLine lbl_HomeAddress;

	private TextEditEx txt_W_County;

	private TextEditEx txt_W_Build;

	private TextEditEx txt_W_City;

	private LabelLine lbl_Build;

	private TextEditEx txt_W_Zipcode;

	private LabelLine lbl_Country;

	private LabelLine lbl_State;

	private LabelLine lbl_ZipCode;

	private LabelLine lbl_County;

	private LabelLine lbl_City;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private SplitContainerControl splitContainerControl1;

	public SimpleButtonEx btn_SetRole;

	private GridControlEx grd_Roles;

	private GridViewEx gv_Roles;

	private GridColumnEx cl_Role;

	private RepositoryItemLookUpEdit rlup_Role;

	private LabelLine lbl_Roles;

	private GridControlEx grd_Teams;

	private GridViewEx gv_Teams;

	private LabelLine labelLine2;

	private GridColumnEx cl_Team;

	private GridColumnEx cl_TeamRole;

	private TextEditEx txt_PasIss;

	private TextEditEx txt_PassNo;

	private TextEditEx txt_IDNo;

	private LabelLine lbl_PassNo;

	private LabelLine lbl_PassExpDate;

	private LabelLine lbl_PassIss;

	private LabelLine lbl_PasIssuer;

	private LabelLine lbl_Citizenship;

	private LabelLine lbl_IDNo;

	private LabelLine lbl_Date;

	private LabelLine lbl_CountryOfBirth;

	private LabelLine lbl_Marital;

	private LabelLine lbl_NoOfChildren;

	private LabelLine lbl_Gender;

	private SpinEditEx spin_NoChildren;

	private PanelControl panelControl5;

	private GridColumnEx cl_LineNum;

	private GridControlEx grd_Udf;

	private GridViewEx gv_Udf;

	private XtraTabPageEx Tab_Administration;

	private XtraTabPageEx tab_Finance;

	private PanelControl panelControl1;

	public SimpleButtonEx btn_PrevEmployment;

	public SimpleButtonEx btn_Review;

	public SimpleButtonEx btn_Education;

	public SimpleButtonEx btn_Absence;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private LabelLine labelLine1;

	private LabelLine labelLine3;

	private LookUpEditCombo luk_Status;

	private LookUpEditCombo luk_TermReason;

	private PanelControl panelControl3;

	private TextEditEx txt_bankAccount;

	private TextEditEx txt_bankBranch;

	private LabelLine labelLine11;

	private LabelLine labelLine10;

	private LabelLine labelLine9;

	private LabelLine labelLine8;

	private CalcEditEx calc_EmplCost;

	private LabelLine labelLine7;

	private CalcEditEx calc_salary;

	private LabelLine labelLine6;

	private LookUpEditCombo luk_salaryUnit;

	private LookUpEditCombo luk_empCostUnt;

	private LookUpEditCombo luk_BankCode;

	private LookUpEditCombo luk_Position;

	private LookUpEditCombo luk_Department;

	private LookUpEditCombo luk_Branch;

	private LookUpEditCombo luk_UserCode;

	private LookUpEditCombo luk_SalesEmp;

	private LookUpEditCombo luk_W_Country;

	private LookUpEditCombo luk_W_State;

	private LookUpEditCombo luk_H_State;

	private LookUpEditCombo luk_H_Country;

	private LookUpEditCombo luk_CountryOfBirth;

	private LookUpEditCombo luk_Citizenship;

	private LookUpEditCombo luk_Gender;

	private LookUpEditCombo luk_MartialStt;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemLookUpEdit rlup_Team;

	private RepositoryItemLookUpEdit rlup_TeamRole;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem deleteRowToolStripMenuItem1;

	private GridColumnEx cl_lineNumber;

	public SimpleButtonEx btn_Relatives;

	private ContextMenuStrip contextMenuStrip3;

	private ToolStripMenuItem deletePictureToolStripMenuItem;

	private XtraTabPageEx tab_Contract;

	private PanelControl panelControl4;

	private GridControlEx grd_Contract;

	private GridViewEx gv_Contract;

	private GridColumnEx contractID;

	private GridColumnEx code;

	private GridColumnEx type;

	private GridColumnEx signDate;

	private RepositoryItemDateEdit repositoryItemDateEdit2;

	private GridColumnEx startDate;

	private GridColumnEx endDate;

	private GridColumnEx salary;

	private GridColumnEx note;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	public SimpleButtonEx btn_Health;

	private GridColumnEx gcSignedBy;

	private GridColumnEx gcProbSalary;

	private GridColumnEx gcBasicSalary;

	private GridColumnEx gcPayment;

	private GridColumnEx gcSalaryCurr;

	private RepositoryItemLookUpEdit luk_contractType;

	private RepositoryItemButtonEditCFL bte_SignedBy;

	private RepositoryItemLookUpEdit luk_PymMeth;

	private RepositoryItemLookUpEdit luk_salaryCurr;

	private RepositoryItemCalcEdit cal_Salary;

	private ContextMenuStrip contextMenuContract;

	private ToolStripMenuItem contextMenuContract_DeleteRow;

	private GridColumnEx IsNewRow;

	private ContextMenuStrip contextMenuStrip4;

	private ToolStripMenuItem removeEmployeeToolStripMenuItem;

	private ucAttachment ucAttachment1;

	private ButtonEditCFL luk_Manager1;

	private TextEditEx txt_BankCode;

	private GridColumnEx gcParentContract;

	private RepositoryItemButtonEditCFL rep_cfl_SubContract;

	private RepositoryItemLookUpEditSimple rep_luk_ParentContract;

	private DateEditEx date_StartDate;

	private DateEditEx date_TermDate;

	private DateEditEx date_Brith;

	private DateEditEx date_PassExp;

	private DateEditEx date_PassportDate;

	private ButtonEditCFL luk_CostCenter;

	private LabelLine lbl_CostCenter;

	private ButtonEditCFL luk_LinkedVendor;

	private LabelLine lbl_LinkedVendor;

	private ToolStripMenuItem organizationalChartToolStripMenuItem;

	private TextEditEx txt_empCode;

	private LabelLine lbl_empCode;

	[DllImport("Shlwapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern bool PathFileExists(StringBuilder path);

	private void GetFromChildMethod(DataTable dt)
	{
		try
		{
			_dtEducation = dt.Copy();
			if (FormMode == Enums.FormViewModeType.View)
			{
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				FormMode = Enums.FormViewModeType.Update;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				_add = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetFromChildMethod_PrevEmp(DataTable dt)
	{
		try
		{
			_dtPrevEmp = dt.Copy();
			if (FormMode == Enums.FormViewModeType.View)
			{
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				FormMode = Enums.FormViewModeType.Update;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				_add = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetFromChildMethod_Review(DataTable dt)
	{
		try
		{
			_dtReview = dt.Copy();
			if (FormMode == Enums.FormViewModeType.View)
			{
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				FormMode = Enums.FormViewModeType.Update;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				_add = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetFromChildMethod_Absence(DataTable dt)
	{
		try
		{
			_dtAbsence = dt.Copy();
			if (FormMode == Enums.FormViewModeType.View)
			{
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				FormMode = Enums.FormViewModeType.Update;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				_add = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_HumanResources()
	{
		InitializeComponent();
		IsSetSuperTip = true;
		IsScrollable = true;
		FormId = "oEmployeesInfo";
		gv_Contract.FormId = FormId;
		gv_Contract.ItemId = "APZ_OHEC";
		gv_Udf.FormId = FormId;
		gv_Udf.ItemId = "APZ_CUFD";
		bte_SignedBy.ContainerControl = this;
		bte_SignedBy.ValueMember = "EmpId";
		bte_SignedBy.DisplayMember = "lastName";
		bte_SignedBy.DisplayMember2 = "firstName";
		bte_SignedBy.UsingAutoCompleteBox = true;
		bte_SignedBy.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
		bte_SignedBy.AutoLoadDataSource = false;
		OnLoadAsync += frm_HumanResources_Load;
		rep_luk_ParentContract.ValueMember = "contractID";
		rep_luk_ParentContract.DisplayMember = "code";
		rep_luk_ParentContract.UseDropDownRowsAsMaxCount = true;
		calc_EmplCost.CustomDisplayText += calc_salary_Properties_CustomDisplayText;
	}

	private async Task frm_HumanResources_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			bte_SignedBy.FindCFLBy(Enums.TypeCFL.EmployeeListForSalesEmpSetup, false, "", Enums.ItemObjectType.NoItemType, null);
			Task task = LoadLookUp();
			Task task2 = LoadUDF();
			Task task3 = SetDictionaryControls();
			AddHandlers();
			await Task.WhenAll(task, task2, task3);
			if (!isCalledFromOutside)
			{
				await MODE_FIND();
			}
			else if (EmpId > 0)
			{
				await GetDetails(EmpId);
			}
			AcceptButton = btn_Save;
			CancelButton = btnCancel;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "F"))
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
			}
			await SaveData();
			if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "F"))
			{
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task<string> SaveData()
	{
		_ = 6;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Find:
			{
				WinCommons._OpenProcessing();
				DataTable dataTable3 = new DataTable();
				if (_edit)
				{
					DataTable controlsValueOCRDNoSAP2 = GetControlsValueOCRDNoSAP();
					if (controlsValueOCRDNoSAP2.IsNotNull())
					{
						controlsValueOCRDNoSAP2.Rows[0]["Active"] = chk_Active.EditValue;
						controlsValueOCRDNoSAP2.AcceptChanges();
					}
					dataTable3 = await _httpClient.EmployeeMasterClient.Search(controlsValueOCRDNoSAP2);
				}
				if (dataTable3.IsNotNull())
				{
					if (dataTable3.Rows.Count > 1)
					{
						string text6 = string.Join(",", from r in dataTable3.AsEnumerable()
							select Function.ToString(r["empID"]));
						frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Employees, false, string.Empty, "Apz_Cfl_EmployleeData2", Enums.ItemObjectType.NoItemType, null, "", text6);
						frm_CFL2.ShowDialog(this);
						DataRow chooseItem = frm_CFL2.ChooseItem;
						if (chooseItem != null)
						{
							EmpId = Function.ParseInt(chooseItem["empID"]);
							await GetDetails(EmpId);
							if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "R"))
							{
								Controls.EnableDisableControl(isEnable: false);
								luk_Manager1.Properties.Buttons[0].Visible = false;
								EnableAdminTabButtons();
							}
						}
					}
					else if (dataTable3.Rows.Count == 1)
					{
						EmpId = Function.ParseInt(dataTable3.Rows[0]["empID"]);
						await GetDetails(EmpId);
						if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "R"))
						{
							Controls.EnableDisableControl(isEnable: false);
							luk_Manager1.Properties.Buttons[0].Visible = false;
							EnableAdminTabButtons();
						}
					}
				}
				else if (_edit)
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
				break;
			}
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
			{
				WinCommons._OpenProcessing();
				DataSet dataSet = new DataSet();
				DataTable controlsValueOCRDNoSAP = GetControlsValueOCRDNoSAP();
				if (controlsValueOCRDNoSAP.IsNotNull())
				{
					DateTime? dateTime = Function.ParseDateTime(controlsValueOCRDNoSAP.Rows[0]["startDate"]);
					DateTime? dateTime2 = Function.ParseDateTime(controlsValueOCRDNoSAP.Rows[0]["birthDate"]);
					DateTime? dateTime3 = Function.ParseDateTime(controlsValueOCRDNoSAP.Rows[0]["termDate"]);
					if (dateTime < dateTime2)
					{
						WinCommons.writeStatus(Enums.MessageCode.StartDateBeforeBirthDate, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
						tab_Detail.SelectedTabPage = Tab_Administration;
						if (Tab_Administration.Focus())
						{
							date_StartDate.Focus();
						}
						else if (tab_Personal.Focus())
						{
							date_Brith.Focus();
						}
						return "error";
					}
					if (dateTime3 < dateTime)
					{
						WinCommons.writeStatus(Enums.MessageCode.TermDateNotBePriorToStartDate, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
						tab_Detail.SelectedTabPage = Tab_Administration;
						if (Tab_Administration.Focus())
						{
							date_TermDate.Focus();
						}
						return "error";
					}
					if (FormMode == Enums.FormViewModeType.Add)
					{
						controlsValueOCRDNoSAP.Rows[0]["empID"] = 0;
						controlsValueOCRDNoSAP.Rows[0]["UserSign"] = GlobalData.UserID;
						controlsValueOCRDNoSAP.Rows[0]["LogInstanc"] = 0;
					}
					controlsValueOCRDNoSAP.Rows[0]["type"] = ((_defaultRole == 0) ? DBNull.Value : ((object)_defaultRole));
					controlsValueOCRDNoSAP.Rows[0]["Active"] = chk_Active.EditValue;
					controlsValueOCRDNoSAP.Rows[0]["UserSign2"] = GlobalData.UserID;
					controlsValueOCRDNoSAP.Rows[0]["UpdateDate"] = DateTime.Now.Date;
					if (luk_SalesEmp.EditValue == null)
					{
						controlsValueOCRDNoSAP.Rows[0]["salesPrson"] = -1;
					}
					DataTable dataSourceTable = grd_Udf.DataSourceTable;
					if (dataSourceTable.IsNotNull())
					{
						foreach (DataColumn item in from DataColumn col in controlsValueOCRDNoSAP.Columns
							where col.ColumnName.ToUpper().StartsWith("U_")
							select col)
						{
							controlsValueOCRDNoSAP.Rows[0][item.ColumnName] = dataSourceTable.Rows[0][item.ColumnName];
						}
					}
					controlsValueOCRDNoSAP.AcceptChanges();
				}
				string text = "";
				if (pic_Images.EditValue != null && _picFilePath != "")
				{
					string picFilePath = _picFilePath;
					string fileName = Path.GetFileName(picFilePath);
					if (!VerifyFileExists(picFilePath, 5000))
					{
						WinCommons._CloseProcessing();
						WinCommons.Error(Enums.MessageCode.UploadPictureFileFailedSourceFileNotFoundWithPath, "", picFilePath);
						return "error";
					}
					string upLoadPath = CopyFileShareHelper.GetUpLoadPath("jpg");
					text = ((fileName == null || fileName.Contains("APZ-Employee-" + Function.ToString(EmpId))) ? fileName : ("APZ-Employee-" + Function.ToString(EmpId) + "-" + fileName));
					string text2 = upLoadPath + "\\" + text;
					if (!VerifyFileExists(upLoadPath, 5000))
					{
						WinCommons._CloseProcessing();
						WinCommons.Error(Enums.MessageCode.UploadPictureFailedFilePathDidNotExists, "");
						return "error";
					}
					string text3 = _ftpHelper.DeleteFileSharing(CopyFileSharingCfg.Domain, text2, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password);
					if (text3 != "")
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), $"Upload Picture delete file [{picFilePath}]: {text3}");
						return "error";
					}
					string text4 = _ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, picFilePath, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, text2);
					if (text4 != text2)
					{
						WinCommons._CloseProcessing();
						WinCommons.Error(Enums.MessageCode.UploadPictureFailed, "", text4);
						return "error";
					}
				}
				controlsValueOCRDNoSAP.Rows[0]["picture"] = text;
				controlsValueOCRDNoSAP.TableName = "dtDoc";
				dataSet.Tables.Add(controlsValueOCRDNoSAP.Copy());
				DataTable dataTable = new DataTable();
				if (grd_Roles.DataSource != null)
				{
					dataTable = grd_Roles.DataSource as DataTable;
				}
				if (!dataTable.IsNotNull())
				{
					dataTable = WinFuntions.GetTableStructure("HEM6");
				}
				DataTable dataTable2 = new DataTable();
				if (grd_Teams.DataSource != null)
				{
					dataTable2 = grd_Teams.DataSource as DataTable;
				}
				if (!dataTable2.IsNotNull())
				{
					dataTable2 = WinFuntions.GetTableStructure("HTM1");
				}
				if (!ucAttachment1.GetAttachmentNoSAP(out var dt))
				{
					return "error";
				}
				new List<string>();
				new List<string> { "type", "PymMeth", "salaryCurr", "BPLId", "IsNewRow" };
				new List<string> { "code", "signDate", "startDate" };
				if (_dtContract.IsNotNull())
				{
					foreach (DataRow item2 in _dtContract.AsEnumerable())
					{
						if (Function.ToString(item2["startDate"]) != "" && Function.ToString(item2["endDate"]) != "" && Function.ParseDateTime(item2["startDate"]) > Function.ParseDateTime(item2["endDate"]))
						{
							WinCommons.Error(Enums.MessageCode.EndDateGreaterStartDate, "");
							return "error";
						}
					}
					if (dicButtonEditValueType.ContainsValue("User"))
					{
						WinCommons.Error(Enums.MessageCode.PleaseChooseSignerFromEmployeeList, "", Function.ToString(dicButtonEditValueType.First((KeyValuePair<int, string> s) => s.Value == "User").Key + 1));
						return "error";
					}
					int num = Function.ParseInt(_dtContract.AsEnumerable().Max((DataRow row) => row["contractID"]));
					foreach (DataRow item3 in _dtContract.AsEnumerable())
					{
						if (Function.ParseInt(item3["contractID"]) <= 0)
						{
							item3["contractID"] = ++num;
						}
					}
				}
				DataRow[] array = (from s in _dtContract.AsEnumerable()
					where Function.ToString(s["code"]) != "" && s["startDate"] != DBNull.Value && s["signDate"] != DBNull.Value
					select s).ToArray();
				if (array.Length == 0)
				{
					_dtContract = _dtContract.Clone();
					_dtContract.AcceptChanges();
				}
				else
				{
					_dtContract = array.CopyToDataTable();
					_dtContract.AcceptChanges();
				}
				if (_dtContract.ExistsColumn("IsNewRow"))
				{
					_dtContract.Columns.Remove("IsNewRow");
				}
				_dtContract.AcceptChanges();
				if (!_dtEducation.IsNotNull())
				{
					_dtEducation = WinFuntions.GetTableStructure("HEM2");
				}
				if (!_dtAbsence.IsNotNull())
				{
					_dtAbsence = WinFuntions.GetTableStructure("HEM1");
				}
				if (!_dtReview.IsNotNull())
				{
					_dtReview = WinFuntions.GetTableStructure("HEM3");
				}
				if (!_dtPrevEmp.IsNotNull())
				{
					_dtPrevEmp = WinFuntions.GetTableStructure("HEM4");
				}
				if (!_dtHealth.IsNotNull())
				{
					_dtHealth = WinFuntions.GetTableStructure("APZ_OHEH");
				}
				if (!_dtContract.IsNotNull())
				{
					_dtContract = WinFuntions.GetTableStructure("APZ_OHEC");
				}
				dataTable.TableName = "dtRole";
				dataTable2.TableName = "dtTeams";
				_dtEducation.TableName = "dtEducation";
				_dtAbsence.TableName = "dtAbsence";
				_dtReview.TableName = "dtReview";
				_dtPrevEmp.TableName = "dtPrevEmp";
				dt.TableName = "AttFile";
				_dtHealth.TableName = "dtHealth";
				_dtContract.TableName = "dtContract";
				dataSet.Tables.Add(dataTable2.Copy());
				dataSet.Tables.Add(dataTable.Copy());
				dataSet.Tables.Add(_dtAbsence.Copy());
				dataSet.Tables.Add(_dtEducation.Copy());
				dataSet.Tables.Add(_dtReview.Copy());
				dataSet.Tables.Add(_dtPrevEmp.Copy());
				dataSet.Tables.Add(dt.Copy());
				dataSet.Tables.Add(_dtHealth.Copy());
				dataSet.Tables.Add(_dtContract.Copy());
				string rs = FormMode switch
				{
					Enums.FormViewModeType.Add => (await _httpClient.EmployeeMasterClient.SaveDoc(dataSet)).Message, 
					Enums.FormViewModeType.Update => (await _httpClient.EmployeeMasterClient.SaveDocUpdate(dataSet)).Message, 
					_ => string.Empty, 
				};
				bool flag = rs?.StartsWith("APZ_OHEC:") ?? false;
				if (string.IsNullOrEmpty(rs) | flag)
				{
					if (string.IsNullOrEmpty(rs))
					{
						WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					}
					else
					{
						string text5 = ((GlobalData.CURRENTLANGUAGE == "vi-VN") ? "Lưu Hợp đồng lỗi :" : "Save Contract failed: ");
						if (FormMode == Enums.FormViewModeType.Add)
						{
							WinCommons.Warning(Enums.MessageCode.None, text5 + rs.Substring(9));
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, text5 + rs.Substring(9));
						}
					}
					if (btn_Save.Text.Equals(Apzon.Commons.Properties.Resources.ButtonAdd))
					{
						await REFRESH();
					}
					else
					{
						if (!flag)
						{
							FormMode = Enums.FormViewModeType.View;
							btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
						}
						await REFRESH_DATA(!flag);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, rs);
				}
				WinCommons._CloseProcessing();
				return rs;
			}
			default:
				Close();
				break;
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		return string.Empty;
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			_isCancel = true;
			Close();
			_isCancel = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void pic_Images_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				ShowReadOnly = true,
				Filter = "Files Pictures (*.BMP,*.GIF,*.JPG,*.PNG)|*.BMP;*.GIF;*.JPG;*.PNG"
			};
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				Bitmap image = new Bitmap(openFileDialog.FileName);
				pic_Images.Image = image;
				_picFilePath = openFileDialog.FileName;
				OnContentChanged(sender, e);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deletePictureToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			pic_Images.Image = null;
			pic_Images.Properties.NullText = "No image data!";
			_picFilePath = "";
			OnContentChanged(sender, e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Manager1_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (EmpId == Function.ParseInt(e.NewValue) && (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update))
			{
				WinCommons.Error(Enums.MessageCode.EmployeeCannotBeHisOwnManager, "");
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Position_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10000)
			{
				frm_HREmpPosition frm_HREmpPosition2 = new frm_HREmpPosition
				{
					MdiParent = MdiParent
				};
				frm_HREmpPosition2.refreshPos = (RefreshLookupPos)Delegate.Combine(frm_HREmpPosition2.refreshPos, new RefreshLookupPos(LoadLookupPosition));
				frm_HREmpPosition2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Status_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10000)
			{
				frm_HREmpStatus frm_HREmpStatus2 = new frm_HREmpStatus
				{
					MdiParent = MdiParent
				};
				frm_HREmpStatus2.refreshStatus = (RefreshLookupStatus)Delegate.Combine(frm_HREmpStatus2.refreshStatus, new RefreshLookupStatus(LoadLookupStatus));
				frm_HREmpStatus2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_TermReason_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10000)
			{
				frm_HRTerminationReasons frm_HRTerminationReasons2 = new frm_HRTerminationReasons
				{
					MdiParent = MdiParent
				};
				frm_HRTerminationReasons2.refreshTerReason = (RefreshLookupTerReason)Delegate.Combine(frm_HRTerminationReasons2.refreshTerReason, new RefreshLookupTerReason(LoadLookupReasons));
				frm_HRTerminationReasons2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Department_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10000)
			{
				frm_HRDepartment frm_HRDepartment2 = new frm_HRDepartment
				{
					MdiParent = MdiParent
				};
				frm_HRDepartment2.refreshDepartment = (RefreshLookupDepartment)Delegate.Combine(frm_HRDepartment2.refreshDepartment, new RefreshLookupDepartment(LoadLookupDepartment));
				frm_HRDepartment2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Branches_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10000)
			{
				frm_HRBranches frm_HRBranches2 = new frm_HRBranches
				{
					MdiParent = MdiParent
				};
				frm_HRBranches2.refeshBranches = (RefreshLookupBranches)Delegate.Combine(frm_HRBranches2.refeshBranches, new RefreshLookupBranches(LoadLookupBranches));
				frm_HRBranches2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_H_Country_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (luk_H_Country.EditValue != null)
			{
				string text = Function.ToString(luk_H_Country.EditValue);
				LoadComboState(text, 2);
				DataTable bankCode = _httpClient.EmployeeMasterClient.GetBankCode(Function.ToString(luk_H_Country.EditValue));
				SetDataSourceByDataTable(luk_BankCode, bankCode);
				OnContentChanged(sender, e);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_W_Country_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (luk_W_Country.EditValue != null)
			{
				string text = Function.ToString(luk_W_Country.EditValue);
				LoadComboState(text, 1);
				OnContentChanged(sender, e);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_W_State_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10000)
			{
				frm_HRStates frm_HRStates2 = new frm_HRStates
				{
					MdiParent = MdiParent,
					CountryCode = Function.ToString(luk_W_Country.EditValue),
					Type = 1
				};
				frm_HRStates2.RefreshStates = (RefreshLookupStates)Delegate.Combine(frm_HRStates2.RefreshStates, new RefreshLookupStates(LoadComboState));
				frm_HRStates2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_H_State_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ParseInt(e.NewValue) == -10000)
			{
				frm_HRStates frm_HRStates2 = new frm_HRStates
				{
					MdiParent = MdiParent,
					CountryCode = Function.ToString(luk_H_Country.EditValue),
					Type = 2
				};
				frm_HRStates2.RefreshStates = (RefreshLookupStates)Delegate.Combine(frm_HRStates2.RefreshStates, new RefreshLookupStates(LoadComboState));
				frm_HRStates2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rlup_Role_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (e.NewValue == null)
			{
				return;
			}
			DataTable dataTable = grd_Roles.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				if (dataTable.Select("roleID=" + Function.ParseInt(e.NewValue)).FirstOrDefault() != null)
				{
					e.Cancel = true;
					return;
				}
				if (Function.ParseInt(e.NewValue) == -10000)
				{
					frm_HRRolesSetup frm_HRRolesSetup2 = new frm_HRRolesSetup
					{
						MdiParent = MdiParent
					};
					frm_HRRolesSetup2.RefreshRole = (RefreshLookupRole)Delegate.Combine(frm_HRRolesSetup2.RefreshRole, new RefreshLookupRole(LoadLookkUpRoles));
					frm_HRRolesSetup2.Show();
					e.Cancel = true;
					return;
				}
				if (Function.ParseInt(gv_Roles.GetFocusedRowCellValue("line")) == 0)
				{
					if (Function.ParseInt(e.NewValue) != 0)
					{
						dataTable.AcceptChanges();
						short? num = dataTable.AsEnumerable().Max((DataRow r) => Function.ParseShort(r["line"]));
						DataRow dataRow = dataTable.NewRow();
						dataRow["line"] = Function.ParseShort(num + 1);
						dataRow["roleID"] = Function.ParseInt(e.NewValue);
						dataTable.Rows.Add(dataRow);
					}
				}
				else if (Function.ParseInt(e.NewValue) == 0)
				{
					gv_Roles.BeginUpdate();
					gv_Roles.DeleteSelectedRows();
					gv_Roles.EndUpdate();
				}
				else
				{
					DataRow dataRow2 = dataTable.Select("line=" + Function.ParseInt(gv_Roles.GetFocusedRowCellValue("line"))).FirstOrDefault();
					if (dataRow2 != null)
					{
						dataRow2["roleID"] = Function.ParseInt(e.NewValue);
					}
				}
				dataTable.AcceptChanges();
				grd_Roles.DataSource = dataTable;
				grd_Roles.RefreshDataSource();
				e.Cancel = false;
				if (e.OldValue != null)
				{
					_listRole.Add(Function.ParseInt(e.OldValue));
				}
				DataRow dataRow3 = ((DataTable)rlup_Role.DataSource).Select("typeID=" + Function.ParseInt(e.NewValue)).FirstOrDefault();
				if (dataRow3 != null && Function.ParseInt(e.NewValue) != 0)
				{
					int item = Function.ParseInt(dataRow3["typeID"]);
					_listRole.Remove(item);
				}
				return;
			}
			switch (Function.ParseInt(e.NewValue))
			{
			case -10000:
			{
				frm_HRRolesSetup frm_HRRolesSetup3 = new frm_HRRolesSetup
				{
					MdiParent = MdiParent
				};
				frm_HRRolesSetup3.RefreshRole = (RefreshLookupRole)Delegate.Combine(frm_HRRolesSetup3.RefreshRole, new RefreshLookupRole(LoadLookkUpRoles));
				frm_HRRolesSetup3.Show();
				e.Cancel = true;
				return;
			}
			case 0:
				e.Cancel = true;
				return;
			}
			if (dataTable != null)
			{
				DataTable dataTable2 = dataTable.Clone();
				DataRow dataRow4 = dataTable2.NewRow();
				dataRow4["line"] = 1;
				dataRow4["roleID"] = Function.ParseInt(e.NewValue);
				dataTable2.Rows.Add(dataRow4);
				dataTable2.AcceptChanges();
				_listRole.Remove(Function.ParseInt(e.NewValue));
				grd_Roles.DataSource = dataTable2;
			}
			grd_Roles.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rlup_Role_PopupFilter(object sender, PopupFilterEventArgs e)
	{
		try
		{
			if (_listRole.Count > 0)
			{
				e.Criteria = new InOperator("typeID", _listRole.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = (DataTable)grd_Roles.DataSource;
			if (dataTable.IsNotNull())
			{
				int num = Function.ParseInt(gv_Roles.GetFocusedRowCellValue("roleID"));
				_listRole.Add(num);
				gv_Roles.BeginUpdate();
				gv_Roles.DeleteSelectedRows();
				gv_Roles.EndUpdate();
				dataTable.AcceptChanges();
				if (_defaultRole == num)
				{
					_defaultRole = 0;
				}
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
					btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Roles_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_SetRole_Click(object sender, EventArgs e)
	{
		try
		{
			int defaultRole = Function.ParseInt(gv_Roles.GetFocusedRowCellValue("roleID"));
			_defaultRole = defaultRole;
			gv_Roles.RefreshData();
			OnContentChanged(sender, e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Roles_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (Function.ParseInt(e.CellValue) == _defaultRole && e.Column.FieldName == "roleID")
		{
			e.Appearance.FontStyleDelta = FontStyle.Bold;
		}
	}

	private void rlup_Team_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (e.NewValue == null)
			{
				return;
			}
			DataTable dataTable = grd_Teams.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				if (dataTable.Select("teamID=" + Function.ParseInt(e.NewValue)).FirstOrDefault() != null)
				{
					e.Cancel = true;
					return;
				}
				if (Function.ParseInt(e.NewValue) == -10000)
				{
					e.Cancel = true;
					frm_HRTeamsSetup frm_HRTeamsSetup2 = new frm_HRTeamsSetup
					{
						MdiParent = MdiParent
					};
					frm_HRTeamsSetup2.refreshTeam = (RefreshLookupTeam)Delegate.Combine(frm_HRTeamsSetup2.refreshTeam, new RefreshLookupTeam(LoadLookUpTeams));
					frm_HRTeamsSetup2.Show();
					return;
				}
				if (Function.ParseInt(gv_Teams.GetFocusedRowCellValue("line")) == 0)
				{
					if (Function.ParseInt(e.NewValue) != 0)
					{
						dataTable.AcceptChanges();
						short? num = dataTable.AsEnumerable().Max((DataRow r) => Function.ParseShort(r["line"]));
						DataRow dataRow = dataTable.NewRow();
						dataRow["line"] = Function.ParseShort(num + 1);
						dataRow["teamID"] = Function.ParseInt(e.NewValue);
						dataRow["role"] = "M";
						dataTable.Rows.Add(dataRow);
					}
				}
				else if (Function.ParseInt(e.NewValue) == 0)
				{
					gv_Teams.BeginUpdate();
					gv_Teams.DeleteSelectedRows();
					gv_Teams.EndUpdate();
				}
				else
				{
					DataRow dataRow2 = dataTable.Select("line=" + Function.ParseInt(gv_Teams.GetFocusedRowCellValue("line"))).FirstOrDefault();
					if (dataRow2 != null)
					{
						dataRow2["teamID"] = Function.ParseInt(e.NewValue);
					}
				}
				dataTable.AcceptChanges();
				grd_Teams.DataSource = dataTable;
				grd_Teams.RefreshDataSource();
				e.Cancel = false;
				if (Function.ParseInt(e.OldValue) != 0)
				{
					_listTeam.Add(Function.ParseInt(e.OldValue));
				}
				DataRow dataRow3 = ((DataTable)rlup_Team.DataSource).Select("teamID=" + Function.ParseInt(e.NewValue)).FirstOrDefault();
				if (dataRow3 != null && Function.ParseInt(e.NewValue) != 0)
				{
					int item = Function.ParseInt(dataRow3["teamID"]);
					_listTeam.Remove(item);
				}
			}
			else if (Function.ParseInt(e.NewValue) == -10000)
			{
				e.Cancel = true;
				frm_HRTeamsSetup frm_HRTeamsSetup3 = new frm_HRTeamsSetup
				{
					MdiParent = MdiParent
				};
				frm_HRTeamsSetup3.refreshTeam = (RefreshLookupTeam)Delegate.Combine(frm_HRTeamsSetup3.refreshTeam, new RefreshLookupTeam(LoadLookUpTeams));
				frm_HRTeamsSetup3.Show();
			}
			else if (Function.ParseInt(e.NewValue) == 0)
			{
				e.Cancel = true;
			}
			else
			{
				if (dataTable != null)
				{
					DataTable dataTable2 = dataTable.Clone();
					DataRow dataRow4 = dataTable2.NewRow();
					dataRow4["line"] = 1;
					dataRow4["teamID"] = Function.ParseInt(e.NewValue);
					dataRow4["role"] = "M";
					dataTable2.Rows.Add(dataRow4);
					dataTable2.AcceptChanges();
					_listTeam.Remove(Function.ParseInt(e.NewValue));
					grd_Teams.DataSource = dataTable2;
				}
				grd_Teams.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rlup_TeamRole_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void rlup_Team_PopupFilter(object sender, PopupFilterEventArgs e)
	{
		try
		{
			if (_listTeam.Count > 0)
			{
				e.Criteria = new InOperator("teamID", _listTeam.ToArray());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = (DataTable)grd_Teams.DataSource;
			if (dataTable.IsNotNull())
			{
				int item = Function.ParseInt(gv_Teams.GetFocusedRowCellValue("teamID"));
				_listTeam.Add(item);
				gv_Teams.BeginUpdate();
				gv_Teams.DeleteSelectedRows();
				gv_Teams.EndUpdate();
				dataTable.AcceptChanges();
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
					btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Teams_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Relatives_Click(object sender, EventArgs e)
	{
		try
		{
			if (btn_Save.Text.Equals(Apzon.Commons.Properties.Resources.ButtonAdd))
			{
				WinCommons.Error(Enums.MessageCode.PleaseSaveThisEmployeeBeforeCreatingNewRelatives, "");
				return;
			}
			frm_EmpRelatives frm_EmpRelatives2 = new frm_EmpRelatives
			{
				MdiParent = MdiParent,
				EmpId = EmpId
			};
			frm_EmpRelatives2.Text = _empName + frm_EmpRelatives2.Text;
			frm_EmpRelatives2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Education_Click(object sender, EventArgs e)
	{
		try
		{
			frm_HREducation frm_HREducation2 = new frm_HREducation
			{
				MdiParent = MdiParent
			};
			SendFromParentDelegate = frm_HREducation2.GetFromParentMethod;
			SendFromParentDelegate(_dtEducation.Copy());
			frm_HREducation2.SendFromChildDelegate = GetFromChildMethod;
			frm_HREducation2.StartPosition = FormStartPosition.CenterScreen;
			frm_HREducation2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_PrevEmployment_Click(object sender, EventArgs e)
	{
		try
		{
			frm_PrevEmp frm_PrevEmp2 = new frm_PrevEmp
			{
				MdiParent = MdiParent
			};
			SendFromParentDelegate = frm_PrevEmp2.GetFromParentMethod;
			SendFromParentDelegate(_dtPrevEmp.Copy());
			frm_PrevEmp2.SendFromChildDelegate = GetFromChildMethod_PrevEmp;
			frm_PrevEmp2.StartPosition = FormStartPosition.CenterScreen;
			frm_PrevEmp2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Review_Click(object sender, EventArgs e)
	{
		try
		{
			frm_Reviews frm_Reviews2 = new frm_Reviews
			{
				MdiParent = MdiParent
			};
			SendFromParentDelegate = frm_Reviews2.GetFromParentMethod;
			SendFromParentDelegate(_dtReview.Copy());
			frm_Reviews2.SendFromChildDelegate = GetFromChildMethod_Review;
			frm_Reviews2.StartPosition = FormStartPosition.CenterScreen;
			frm_Reviews2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Absence_Click(object sender, EventArgs e)
	{
		try
		{
			frm_Absence frm_Absence2 = new frm_Absence
			{
				MdiParent = MdiParent
			};
			SendFromParentDelegate = frm_Absence2.GetFromParentMethod;
			SendFromParentDelegate(_dtAbsence.Copy());
			frm_Absence2.SendFromChildDelegate = GetFromChildMethod_Absence;
			frm_Absence2.StartPosition = FormStartPosition.CenterScreen;
			frm_Absence2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Health_Click(object sender, EventArgs e)
	{
		try
		{
			frm_HRHealth frm_HRHealth2 = new frm_HRHealth
			{
				DtHealth = _dtHealth.Copy(),
				EmpId = EmpId,
				MdiParent = MdiParent
			};
			frm_HRHealth2.SendHealthBackToMain = (ReloadHealth)Delegate.Combine(frm_HRHealth2.SendHealthBackToMain, new ReloadHealth(ReloadHealth));
			frm_HRHealth2.StartPosition = FormStartPosition.CenterScreen;
			frm_HRHealth2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnContract_Click(object sender, EventArgs e)
	{
	}

	private async void frm_HumanResources_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_isCancel)
			{
				return;
			}
			if (!_forceClose)
			{
				e.Cancel = true;
			}
			if (_forceClose)
			{
				return;
			}
			if (btn_Save.Text == Apzon.Commons.Properties.Resources.ButtonUpdate || (btn_Save.Text == Apzon.Commons.Properties.Resources.ButtonAdd && _add))
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.No:
					_forceClose = true;
					Close();
					break;
				case DialogResult.Yes:
					if (string.IsNullOrEmpty(await SaveData()))
					{
						_forceClose = true;
						Close();
					}
					break;
				}
			}
			else
			{
				_forceClose = true;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeEmployeeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (btn_Save.Text == Apzon.Commons.Properties.Resources.ButtonOK && btn_Save.Text == Apzon.Commons.Properties.Resources.ButtonUpdate)
			{
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteEmployee, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				int empID = Function.ParseInt(txt_EmpNo.EditValue);
				string text = await _httpClient.EmployeeMasterClient.Remove(empID);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
					await MODE_FIND();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void txt_Email_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (!WinFuntions._isemail(txt_Email))
			{
				txt_Email.ErrorText = "Invalid Email Address!";
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void contextMenuStrip4_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Find)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ReloadHealth(DataTable dtHealth)
	{
		try
		{
			_dtHealth = dtHealth;
			if (FormMode == Enums.FormViewModeType.View && !_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				_add = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ContractReload(DataTable dtContract)
	{
	}

	private async Task SetDictionaryControls()
	{
		try
		{
			_dicControls = new Dictionary<string, Control>();
			_dicControls.Add("OHEM.firstName", txt_FirstName);
			_dicControls.Add("OHEM.middleName", txt_MiddleName);
			_dicControls.Add("OHEM.lastName", txt_LastName);
			_dicControls.Add("OHEM.jobTitle", txt_JobTitle);
			_dicControls.Add("OHEM.position", luk_Position);
			_dicControls.Add("OHEM.dept", luk_Department);
			_dicControls.Add("OHEM.branch", luk_Branch);
			_dicControls.Add("OHEM.manager", luk_Manager1);
			_dicControls.Add("OHEM.BPLink", luk_LinkedVendor);
			_dicControls.Add("OHEM.userId", luk_UserCode);
			_dicControls.Add("OHEM.salesPrson", luk_SalesEmp);
			_dicControls.Add("OHEM.CostCenter", luk_CostCenter);
			_dicControls.Add("OHEM.empID", txt_EmpNo);
			_dicControls.Add("OHEM.Code", txt_empCode);
			_dicControls.Add("OHEM.ExtEmpNo", txt_ExtEmpNo);
			_dicControls.Add("OHEM.Active", chk_Active);
			_dicControls.Add("OHEM.officeTel", txt_OfficePhone);
			_dicControls.Add("OHEM.officeExt", txt_ExtPhone);
			_dicControls.Add("OHEM.mobile", txt_MobilePhone);
			_dicControls.Add("OHEM.pager", txt_Pager);
			_dicControls.Add("OHEM.homeTel", txt_HomePhone);
			_dicControls.Add("OHEM.fax", txt_Fax);
			_dicControls.Add("OHEM.email", txt_Email);
			_dicControls.Add("OHEM.workStreet", txt_W_Street);
			_dicControls.Add("OHEM.StreetNoW", txt_W_StreetNo);
			_dicControls.Add("OHEM.workBlock", txt_W_Block);
			_dicControls.Add("OHEM.WorkBuild", txt_W_Build);
			_dicControls.Add("OHEM.workZip", txt_W_Zipcode);
			_dicControls.Add("OHEM.workCity", txt_W_City);
			_dicControls.Add("OHEM.workCounty", txt_W_County);
			_dicControls.Add("OHEM.workState", luk_W_State);
			_dicControls.Add("OHEM.workCountr", luk_W_Country);
			_dicControls.Add("OHEM.homeStreet", txt_H_Street);
			_dicControls.Add("OHEM.StreetNoH", txt_H_StreetNo);
			_dicControls.Add("OHEM.homeBlock", txt_H_Block);
			_dicControls.Add("OHEM.HomeBuild", txt_H_Build);
			_dicControls.Add("OHEM.homeZip", txt_H_Zipcode);
			_dicControls.Add("OHEM.homeCity", txt_H_City);
			_dicControls.Add("OHEM.homeCounty", txt_H_County);
			_dicControls.Add("OHEM.homeState", luk_H_State);
			_dicControls.Add("OHEM.homeCountr", luk_H_Country);
			_dicControls.Add("OHEM.startDate", date_StartDate);
			_dicControls.Add("OHEM.status", luk_Status);
			_dicControls.Add("OHEM.termDate", date_TermDate);
			_dicControls.Add("OHEM.termReason", luk_TermReason);
			_dicControls.Add("OHEM.sex", luk_Gender);
			_dicControls.Add("OHEM.birthDate", date_Brith);
			_dicControls.Add("OHEM.brthCountr", luk_CountryOfBirth);
			_dicControls.Add("OHEM.martStatus", luk_MartialStt);
			_dicControls.Add("OHEM.nChildren", spin_NoChildren);
			_dicControls.Add("OHEM.govID", txt_IDNo);
			_dicControls.Add("OHEM.citizenshp", luk_Citizenship);
			_dicControls.Add("OHEM.passportNo", txt_PassNo);
			_dicControls.Add("OHEM.passportEx", date_PassExp);
			_dicControls.Add("OHEM.PassIssue", date_PassportDate);
			_dicControls.Add("OHEM.PassIssuer", txt_PasIss);
			spin_NoChildren.SetFormatControl(Enums.DisplayFormatType.Integer);
			_dicControls.Add("OHEM.salary", calc_salary);
			_dicControls.Add("OHEM.salaryUnit", luk_salaryUnit);
			_dicControls.Add("OHEM.EmplCost", calc_EmplCost);
			_dicControls.Add("OHEM.empCostUnt", luk_empCostUnt);
			_dicControls.Add("OHEM.BankCode", luk_BankCode);
			_dicControls.Add("OHEM.bankAcount", txt_bankAccount);
			_dicControls.Add("OHEM.bankBranch", txt_bankBranch);
			_dicControls.Add("OHEM.remark", txt_Remark);
			await this.SetControlMaxLength(_dicControls);
			foreach (KeyValuePair<string, Control> dicControl in _dicControls)
			{
				dicControl.Value.Tag = dicControl.Key.Replace(".", ",");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void BindControlsValue(string table, DataTable dt)
	{
		new List<PropertyInfo>();
		try
		{
			if (_dicControls == null)
			{
				WinCommons.Error(Enums.MessageCode.LoadDataError, "");
			}
			else
			{
				if (!_dicControls.Any((KeyValuePair<string, Control> t) => t.Key.Split('.')[0] == table))
				{
					return;
				}
				List<KeyValuePair<string, Control>> list = _dicControls.Where((KeyValuePair<string, Control> t) => t.Key.Split('.')[0] == table).ToList();
				if (!dt.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.DataIsEmpty, "");
					return;
				}
				DataRow dataRow = dt.Rows[0];
				for (int num = 0; num < list.Count(); num++)
				{
					string text = ((list[num].Key.Split('.') != null && list[num].Key.Split('.').Length == 2) ? list[num].Key.Split('.')[1] : "");
					if (string.IsNullOrWhiteSpace(text) || !dt.Columns.Contains(text))
					{
						continue;
					}
					if (dataRow[dt.Columns[text]] == DBNull.Value)
					{
						((BaseEdit)list[num].Value).EditValue = null;
						continue;
					}
					switch (dt.Columns[text].DataType.ToString().ToLower())
					{
					case "system.string":
						((BaseEdit)list[num].Value).EditValue = Function.ToString(dataRow[dt.Columns[text]]);
						break;
					case "system.double":
						((BaseEdit)list[num].Value).EditValue = Function.ParseDouble(dataRow[dt.Columns[text]]);
						break;
					case "system.decimal":
						((BaseEdit)list[num].Value).EditValue = Function.ParseDouble(dataRow[dt.Columns[text]]);
						break;
					case "system.float":
						((BaseEdit)list[num].Value).EditValue = Function.ParseDouble(dataRow[dt.Columns[text]]);
						break;
					case "system.single":
						((BaseEdit)list[num].Value).EditValue = Function.ParseDouble(dataRow[dt.Columns[text]]);
						break;
					case "system.int16":
						((BaseEdit)list[num].Value).EditValue = Function.ParseShort(dataRow[dt.Columns[text]]);
						break;
					case "system.int32":
						if (text.ToUpper() == "USERID")
						{
							((BaseEdit)list[num].Value).EditValue = Function.ParseShort(dataRow[dt.Columns[text]]);
						}
						((BaseEdit)list[num].Value).EditValue = Function.ParseInt(dataRow[dt.Columns[text]]);
						break;
					case "system.int64":
						((BaseEdit)list[num].Value).EditValue = Function.ParseLong(dataRow[dt.Columns[text]]);
						break;
					case "system.datetime":
						((BaseEdit)list[num].Value).EditValue = Function.ParseDateTime(dataRow[dt.Columns[text]]);
						break;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetControlsValueOCRDNoSAP()
	{
		DataTable tableStructure = WinFuntions.GetTableStructure("OHEM");
		DataRow dataRow = tableStructure.NewRow();
		try
		{
			if (_dicControls.Any((KeyValuePair<string, Control> t) => t.Key.Split('.')[0] == "OHEM"))
			{
				List<KeyValuePair<string, Control>> list = _dicControls.Where((KeyValuePair<string, Control> t) => t.Key.Split('.')[0] == "OHEM").ToList();
				for (int num = 0; num < list.Count(); num++)
				{
					string text = ((list[num].Key.Split('.') != null && list[num].Key.Split('.').Length == 2) ? list[num].Key.Split('.')[1] : "");
					if (string.IsNullOrWhiteSpace(text) || !tableStructure.Columns.Contains(text))
					{
						continue;
					}
					if (((BaseEdit)list[num].Value).EditValue == null || ((BaseEdit)list[num].Value).EditValue == DBNull.Value)
					{
						dataRow[text] = DBNull.Value;
						continue;
					}
					switch (tableStructure.Columns[text].DataType.ToString().ToLower())
					{
					case "system.string":
						dataRow[text] = Function.ToString(((BaseEdit)list[num].Value).EditValue);
						break;
					case "system.double":
						dataRow[text] = Function.ParseDouble(((BaseEdit)list[num].Value).EditValue);
						break;
					case "system.float":
						dataRow[text] = Function.ParseDouble(((BaseEdit)list[num].Value).EditValue);
						break;
					case "system.single":
						dataRow[text] = Function.ParseDouble(((BaseEdit)list[num].Value).EditValue);
						break;
					case "system.decimal":
						dataRow[text] = Function.ParseDouble(((BaseEdit)list[num].Value).EditValue);
						break;
					case "system.int16":
						dataRow[text] = Function.ParseShort(Function.ParseInt(((BaseEdit)list[num].Value).EditValue));
						break;
					case "system.int32":
						dataRow[text] = Convert.ToInt32(Function.ParseInt(((BaseEdit)list[num].Value).EditValue));
						break;
					case "system.int64":
						dataRow[text] = Convert.ToInt64(Function.ParseInt(((BaseEdit)list[num].Value).EditValue));
						break;
					case "system.datetime":
					{
						DateTime? dateTime = Function.ParseDateTime(((BaseEdit)list[num].Value).EditValue);
						DateTime dateTime2;
						if (dateTime.HasValue)
						{
							_ = dateTime.Value;
							dateTime2 = dateTime.Value;
						}
						else
						{
							dateTime2 = new DateTime(1900, 1, 1);
						}
						dataRow[text] = dateTime2;
						break;
					}
					}
				}
			}
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return tableStructure;
	}

	public async Task REFRESH()
	{
		_ = 1;
		try
		{
			if (!(btn_Save.Text == Apzon.Commons.Properties.Resources.ButtonUpdate) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				_listRole.Clear();
				_listTeam.Clear();
				this.ResetData();
				await LoadLookUp();
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
				FormMode = Enums.FormViewModeType.Add;
				EmpId = await _httpClient.EmployeeMasterClient.GetMaxId() + 1;
				DataTable tableStructure = WinFuntions.GetTableStructure("OHEM");
				tableStructure.SetDefaultAndMaxLen("OHEM");
				DataRow dataRow = tableStructure.NewRow();
				dataRow["Active"] = "Y";
				dataRow["EmpID"] = EmpId;
				if (tableStructure.Columns.Contains("Code"))
				{
					dataRow["Code"] = EmpId;
				}
				dataRow["sex"] = "M";
				dataRow["martStatus"] = "S";
				dataRow["salaryUnit"] = "M";
				dataRow["empCostUnt"] = "M";
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
				BindControlsValue("OHEM", tableStructure);
				chk_Active.Checked = Function.ToString(tableStructure.Rows[0]["Active"]).Equals("Y");
				pic_Images.Image = null;
				pic_Images.Properties.NullText = "No image data!";
				grd_Roles.DataSource = WinFuntions.GetTableStructure("HEM6");
				grd_Teams.DataSource = WinFuntions.GetTableStructure("HTM1");
				ucAttachment1.DataSource = WinFuntions.GetTableStructure("ATC1");
				_dtEducation = WinFuntions.GetTableStructure("HEM2");
				_dtAbsence = WinFuntions.GetTableStructure("HEM1");
				_dtReview = WinFuntions.GetTableStructure("HEM3");
				_dtPrevEmp = WinFuntions.GetTableStructure("HEM4");
				_dtHealth = WinFuntions.GetTableStructure("APZ_OHEH");
				_dtContract = WinFuntions.GetTableStructure("APZ_OHEC");
				DataTable tableStructure2 = WinFuntions.GetTableStructure("OHEM");
				if (tableStructure2.Columns.Cast<DataColumn>().Any((DataColumn c) => c.ColumnName.ToUpper().StartsWith("U_")))
				{
					tableStructure2.SetDefaultAndMaxLen("OHEM");
					DataRow row = tableStructure2.NewRow();
					tableStructure2.Rows.Add(row);
					tableStructure2.AcceptChanges();
					grd_Udf.DataSource = tableStructure2;
					grd_Udf.RefreshDataSource();
				}
				RefreshGridContract();
				_add = false;
				txt_FirstName.Focus();
				txt_EmpNo.ReadOnly = true;
				tab_Membership.PageEnabled = true;
				tab_Attachments.PageEnabled = true;
				tab_Contract.PageEnabled = true;
				tab_User.PageEnabled = true;
				btn_Education.Enabled = true;
				btn_Absence.Enabled = true;
				btn_Review.Enabled = true;
				btn_PrevEmployment.Enabled = true;
				btn_Relatives.Enabled = true;
				btn_Health.Enabled = true;
				pic_Images.Enabled = true;
				luk_Manager1.ReadOnly = false;
				luk_CostCenter.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA(bool reloadContract = true)
	{
		_ = 2;
		try
		{
			if (!_isRefresh && (!((btn_Save.Text == Apzon.Commons.Properties.Resources.ButtonUpdate) & reloadContract) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
			{
				_isRefresh = true;
				int num = await _httpClient.EmployeeMasterClient.GetMaxId() + 1;
				if (EmpId > 0 && EmpId < num)
				{
					await LoadLookUp();
					await GetDetails(EmpId, reloadContract);
				}
				_isRefresh = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "R"))
			{
				Controls.EnableDisableControl(isEnable: true);
				luk_Manager1.Properties.Buttons[0].Visible = true;
			}
			if ((!(btn_Save.Text == Apzon.Commons.Properties.Resources.ButtonUpdate) && (!(btn_Save.Text == Apzon.Commons.Properties.Resources.ButtonAdd) || !_add)) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				this.SetFindMode();
				DataTable tableStructure = WinFuntions.GetTableStructure("OHEM");
				if (tableStructure.Columns.Contains("Code"))
				{
					lbl_empCode.Enabled = true;
					txt_empCode.Enabled = true;
				}
				DataRow dataRow = tableStructure.NewRow();
				dataRow["Active"] = "Y";
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
				BindControlsValue("OHEM", tableStructure);
				chk_Active.Checked = Function.ToString(tableStructure.Rows[0]["Active"]).Equals("Y");
				pic_Images.Image = null;
				pic_Images.Properties.NullText = "No image data!";
				grd_Roles.DataSource = WinFuntions.GetTableStructure("HEM6");
				grd_Teams.DataSource = WinFuntions.GetTableStructure("HTM1");
				ucAttachment1.DataSource = WinFuntions.GetTableStructure("ATC1");
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonFind;
				FormMode = Enums.FormViewModeType.Find;
				txt_FirstName.Focus();
				txt_EmpNo.ReadOnly = false;
				txt_EmpNo.Text = string.Empty;
				tab_Membership.PageEnabled = false;
				tab_Attachments.PageEnabled = false;
				tab_Contract.PageEnabled = false;
				tab_User.PageEnabled = false;
				btn_Education.Enabled = false;
				btn_Absence.Enabled = false;
				btn_Review.Enabled = false;
				btn_PrevEmployment.Enabled = false;
				btn_Relatives.Enabled = false;
				btn_Health.Enabled = false;
				pic_Images.Enabled = false;
				luk_Manager1.ReadOnly = true;
				luk_CostCenter.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookkUpRoles()
	{
		try
		{
			DataTable gridRoleSource = grd_Roles.DataSourceTable;
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadLookkUpRole();
			if (dataTable == null || dataTable.Columns.Count <= 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.Add("typeID", typeof(int));
				dataTable.Columns.Add("name", typeof(string));
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["typeID"] = 0;
			dataRow["name"] = "";
			dataTable.Rows.InsertAt(dataRow, 0);
			dataTable.AcceptChanges();
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["typeID"] = -10000;
			dataRow2["name"] = "-Define New-";
			dataTable.Rows.Add(dataRow2);
			dataTable.AcceptChanges();
			rlup_Role.DataSource = null;
			rlup_Role.DataSource = dataTable.Copy();
			rlup_Team.DropDownRows = dataTable.Rows.Count;
			if (gridRoleSource.IsNotNull())
			{
				for (int i = 0; i < gridRoleSource.Rows.Count; i++)
				{
					DataRow dataRow3 = dataTable.Select("typeID=" + Function.ParseInt(gridRoleSource.Rows[i]["roleID"])).FirstOrDefault();
					if (dataRow3 != null)
					{
						dataTable.Rows.Remove(dataRow3);
						dataTable.AcceptChanges();
					}
				}
			}
			_listRole.Clear();
			for (int j = 0; j < dataTable.Rows.Count; j++)
			{
				_listRole.Add(Function.ParseInt(dataTable.Rows[j]["typeID"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookUpTeams()
	{
		try
		{
			DataTable gridTeamSource = grd_Teams.DataSourceTable;
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadLookUpTeam();
			if (dataTable == null || dataTable.Columns.Count <= 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.Add("teamID", typeof(int));
				dataTable.Columns.Add("name", typeof(string));
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["teamID"] = 0;
			dataRow["name"] = "";
			dataTable.Rows.InsertAt(dataRow, 0);
			dataTable.AcceptChanges();
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["teamID"] = -10000;
			dataRow2["name"] = "-Define New-";
			dataTable.Rows.Add(dataRow2);
			dataTable.AcceptChanges();
			rlup_Team.DataSource = null;
			rlup_Team.DataSource = dataTable.Copy();
			if (gridTeamSource.IsNotNull())
			{
				for (int i = 0; i < gridTeamSource.Rows.Count; i++)
				{
					DataRow dataRow3 = dataTable.Select("teamID=" + Function.ParseInt(gridTeamSource.Rows[i]["teamID"])).FirstOrDefault();
					if (dataRow3 != null)
					{
						dataTable.Rows.Remove(dataRow3);
						dataTable.AcceptChanges();
					}
				}
			}
			_listTeam.Clear();
			for (int j = 0; j < dataTable.Rows.Count; j++)
			{
				_listTeam.Add(Function.ParseInt(dataTable.Rows[j]["teamID"]));
			}
			rlup_Team.DropDownRows = dataTable.Rows.Count;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookupPosition()
	{
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboPosition();
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("Code", typeof(int));
			dataTable2.Columns.Add("Name", typeof(string));
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow dataRow = dataTable2.NewRow();
				dataRow["Code"] = Function.ParseInt(dataTable.Rows[i]["posID"]);
				dataRow["Name"] = Function.ToString(dataTable.Rows[i]["name"]);
				dataTable2.Rows.Add(dataRow);
				dataTable2.AcceptChanges();
			}
			SetDataSourceByDataTable(luk_Position, dataTable2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookupStatus()
	{
		try
		{
			DataTable dtSource = await _httpClient.EmployeeMasterClient.GetEmployeeStatus();
			SetDataSourceByDataTable(luk_Status, dtSource);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookupReasons()
	{
		try
		{
			DataTable dtSource = await _httpClient.EmployeeMasterClient.GetTermReason();
			SetDataSourceByDataTable(luk_TermReason, dtSource);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookupDepartment()
	{
		try
		{
			DataTable dtSource = await _httpClient.EmployeeMasterClient.LoadComboDept();
			SetDataSourceByDataTable(luk_Department, dtSource);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookupBranches()
	{
		try
		{
			DataTable dtSource = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			SetDataSourceByDataTable(luk_Branch, dtSource);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadLookUpContractType()
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "EmpContract");
			DataTable dataTable = httpClientHelper.Get("LoadContractTypes");
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OCTY");
			}
			dataTable.Columns.Remove("descriptio");
			dataTable.Columns.Remove("empStatus");
			dataTable.Columns.Remove("locked");
			DataRow dataRow = dataTable.NewRow();
			dataRow["typeID"] = -1;
			dataRow["code"] = "";
			dataRow["name"] = "";
			dataTable.Rows.InsertAt(dataRow, 0);
			dataTable.AcceptChanges();
			dataRow = dataTable.NewRow();
			dataRow["typeID"] = -2;
			dataRow["code"] = "-Define New-";
			dataRow["name"] = "";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			luk_contractType.Columns.Clear();
			luk_contractType.Columns.Add(new LookUpColumnInfo("typeID", 100, "type"));
			luk_contractType.Columns.Add(new LookUpColumnInfo("code", 100, "Code"));
			luk_contractType.Columns.Add(new LookUpColumnInfo("name", 150, "Name"));
			luk_contractType.DropDownRows = dataTable.Rows.Count;
			luk_contractType.DataSource = dataTable.Copy();
			luk_contractType.BestFitMode = BestFitMode.BestFitResizePopup;
			luk_contractType.ForceInitialize();
			luk_contractType.PopulateColumns();
			luk_contractType.NullText = "";
			luk_contractType.ValueMember = "typeID";
			luk_contractType.DisplayMember = "code";
			luk_contractType.Columns["typeID"].Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadLookUpPaymentMethod()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("typeID", typeof(string));
		dataTable.Columns.Add("name", typeof(string));
		DataRow dataRow = dataTable.NewRow();
		dataRow["typeID"] = "-1";
		dataRow["name"] = "";
		dataTable.Rows.Add(dataRow);
		dataTable.AcceptChanges();
		dataRow = dataTable.NewRow();
		dataRow["typeID"] = "5";
		dataRow["name"] = "Bank Transfer";
		dataTable.Rows.Add(dataRow);
		dataTable.AcceptChanges();
		luk_PymMeth.Columns.Add(new LookUpColumnInfo("typeID", 100, "Type"));
		luk_PymMeth.Columns.Add(new LookUpColumnInfo("name", 150, "Name"));
		luk_PymMeth.DropDownRows = dataTable.Rows.Count;
		luk_PymMeth.DataSource = dataTable.Copy();
		luk_PymMeth.BestFitMode = BestFitMode.BestFitResizePopup;
		luk_PymMeth.PopulateColumns();
		luk_PymMeth.NullText = "";
		luk_PymMeth.ValueMember = "typeID";
		luk_PymMeth.DisplayMember = "name";
		luk_PymMeth.Columns["typeID"].Visible = false;
		luk_PymMeth.ShowHeader = false;
	}

	private async Task LoadLookUpCurrency()
	{
		salaryCurrData = GlobalData.CurrencyCodes;
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("CurrCode", typeof(string));
		dataTable.Columns.Add("CurrName", typeof(string));
		foreach (CurrencyCodesImpl salaryCurrDatum in salaryCurrData)
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow["CurrCode"] = salaryCurrDatum.CurrCode;
			dataRow["CurrName"] = salaryCurrDatum.CurrName;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
		}
		luk_salaryCurr.DataSource = dataTable.Copy();
		luk_salaryCurr.NullText = "";
		luk_salaryCurr.ValueMember = "CurrCode";
		luk_salaryCurr.DisplayMember = "CurrCode";
		luk_salaryCurr.ShowHeader = false;
	}

	private async Task LoadLookUp()
	{
		_ = 6;
		try
		{
			LoadLookupPosition();
			LoadLookupStatus();
			LoadLookupReasons();
			LoadLookupDepartment();
			LoadLookupBranches();
			await GetEmployeeList();
			await LoadLookUpPaymentMethod();
			await LoadLookUpCurrency();
			LoadLookUpContractType();
			LoadLookUpTeams();
			LoadLookkUpRoles();
			Task<DataTable> task = _httpClient.EmployeeMasterClient.LoadEmployees();
			Task<DataTable> taskUserCode = _httpClient.EmployeeMasterClient.LoadComboUserCode();
			Task<DataTable> taskSaleEmp = _httpClient.EmployeeMasterClient.LoadComboSalesEmp();
			Task<DataTable> taskCountry = _httpClient.EmployeeMasterClient.LoadComboCountry();
			SetDataSourceByList(list: new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "M",
					Name = "Male"
				},
				new Common.Table_Code
				{
					Code = "F",
					Name = "Female"
				}
			}, luk: luk_Gender);
			SetDataSourceByList(list: new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "S",
					Name = "Single"
				},
				new Common.Table_Code
				{
					Code = "M",
					Name = "Married"
				},
				new Common.Table_Code
				{
					Code = "D",
					Name = "Divorced"
				},
				new Common.Table_Code
				{
					Code = "W",
					Name = "Widowed"
				}
			}, luk: luk_MartialStt);
			SetDataSourceByList(list: new Dictionary<string, string>
			{
				{ "H", "Hour" },
				{ "D", "Day" },
				{ "W", "Week" },
				{ "M", "Month" },
				{ "Y", "Year" },
				{ "S", "Semimonthly" },
				{ "B", "Biweekly" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList(), luk: luk_salaryUnit);
			SetDataSourceByList(list: new Dictionary<string, string>
			{
				{ "H", "Hour" },
				{ "D", "Day" },
				{ "W", "Week" },
				{ "M", "Month" },
				{ "Y", "Year" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList(), luk: luk_empCostUnt);
			List<Common.Table_Code> list = new Dictionary<string, string>
			{
				{ "L", "Leader" },
				{ "M", "Member" }
			}.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			rlup_TeamRole.DataSource = list;
			rlup_TeamRole.DisplayMember = "Name";
			rlup_TeamRole.ValueMember = "Code";
			rlup_TeamRole.BestFitMode = BestFitMode.BestFitResizePopup;
			rlup_TeamRole.ForceInitialize();
			rlup_TeamRole.PopulateColumns();
			rlup_TeamRole.Columns["Code"].Visible = false;
			rlup_TeamRole.DropDownRows = list.Count;
			DataTable dataTable = await task;
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("Code", typeof(int));
			dataTable2.Columns.Add("Name", typeof(string));
			for (int num = 0; num < dataTable.Rows.Count; num++)
			{
				DataRow dataRow = dataTable2.NewRow();
				dataRow["Code"] = Function.ParseInt(dataTable.Rows[num]["empID"]);
				dataRow["Name"] = Function.ToString(dataTable.Rows[num]["firstName"]) + "," + Function.ToString(dataTable.Rows[num]["lastName"]);
				dataTable2.Rows.Add(dataRow);
				dataTable2.AcceptChanges();
			}
			luk_Manager1.Properties.ContainerControl = this;
			luk_Manager1.Properties.ValueMember = "empID";
			luk_Manager1.Properties.DisplayMember = "lastName";
			luk_Manager1.Properties.DisplayMember2 = "firstName";
			luk_Manager1.Properties.AutoLoadDataSource = false;
			luk_Manager1.Properties.FindCFLWith(Enums.TypeCFL.Employees, false, "Apz_Cfl_EmployleeData", Enums.ItemObjectType.NoItemType, null, "");
			luk_Manager1.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			luk_Manager1.Properties.RefreshDataSource();
			bte_SignedBy.RefreshDataSource();
			DataTable dtSource = await taskUserCode;
			SetDataSourceByDataTable(luk_UserCode, dtSource, "USER_CODE", "USERID");
			DataTable dtSource2 = await taskSaleEmp;
			SetDataSourceByDataTable(luk_SalesEmp, dtSource2, "SlpName", "SlpCode");
			DataTable dataTable3 = await taskCountry;
			SetDataSourceByDataTable(luk_H_Country, dataTable3.Copy());
			SetDataSourceByDataTable(luk_W_Country, dataTable3.Copy());
			SetDataSourceByDataTable(luk_CountryOfBirth, dataTable3.Copy());
			SetDataSourceByDataTable(luk_Citizenship, dataTable3.Copy());
			calc_salary.Properties.DisplayFormat.FormatType = FormatType.Numeric;
			calc_salary.Properties.DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.QtyDec ?? 6)}";
			calc_EmplCost.Properties.Assign(calc_salary.Properties);
			calc_EmplCost.Properties.ReadOnly = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDataSourceByList(LookUpEdit luk, List<Common.Table_Code> list)
	{
		try
		{
			luk.Properties.DataSource = list;
			luk.Properties.DisplayMember = "Name";
			luk.Properties.ValueMember = "Code";
			luk.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			luk.Properties.ForceInitialize();
			luk.Properties.PopulateColumns();
			luk.Properties.Columns["Code"].Visible = false;
			luk.Properties.DropDownRows = list.Count;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDataSourceByDataTable(LookUpEdit luk, DataTable dtSource, string displayMember = "Name", string valueMember = "Code")
	{
		try
		{
			if (dtSource.IsNotNull())
			{
				DataRow dataRow = dtSource.NewRow();
				dataRow[valueMember] = DBNull.Value;
				if (!luk.Name.Equals("luk_SalesEmp"))
				{
					dtSource.Rows.InsertAt(dataRow, 0);
					dtSource.AcceptChanges();
				}
				if (luk.Name.Equals("luk_Position") || luk.Name.Equals("luk_Status") || luk.Name.Equals("luk_TermReason") || luk.Name.Equals("luk_Department") || luk.Name.Equals("luk_Branch"))
				{
					DataRow dataRow2 = dtSource.NewRow();
					dataRow2[valueMember] = -10000;
					dataRow2[displayMember] = "-Define New-";
					dtSource.Rows.Add(dataRow2);
					dtSource.AcceptChanges();
				}
				luk.Properties.DataSource = null;
				luk.Properties.DataSource = dtSource.Copy();
				luk.Properties.DisplayMember = displayMember;
				luk.Properties.ValueMember = valueMember;
				luk.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				luk.Properties.DropDownRows = dtSource.Rows.Count;
				luk.Properties.ForceInitialize();
				luk.Properties.PopulateColumns();
				luk.Properties.Columns[valueMember].Visible = false;
				if (luk.Name.Equals("luk_Department"))
				{
					luk.Properties.Columns["Remarks"].Visible = false;
				}
			}
			else if (luk.Name.Equals("luk_Position") || luk.Name.Equals("luk_Status") || luk.Name.Equals("luk_TermReason") || luk.Name.Equals("luk_Department") || luk.Name.Equals("luk_Branch"))
			{
				DataRow dataRow3 = dtSource.NewRow();
				dataRow3[valueMember] = -1;
				dtSource.Rows.InsertAt(dataRow3, 0);
				dtSource.AcceptChanges();
				DataRow dataRow4 = dtSource.NewRow();
				dataRow4[valueMember] = -10000;
				dataRow4[displayMember] = "-Define New-";
				dtSource.Rows.Add(dataRow4);
				dtSource.AcceptChanges();
				luk.Properties.DataSource = null;
				luk.Properties.DataSource = dtSource.Copy();
				luk.Properties.DisplayMember = displayMember;
				luk.Properties.ValueMember = valueMember;
				luk.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				luk.Properties.ForceInitialize();
				luk.Properties.PopulateColumns();
				luk.Properties.Columns[valueMember].Visible = false;
				if (luk.Name.Equals("luk_Department"))
				{
					luk.Properties.Columns["Remarks"].Visible = false;
				}
			}
			else
			{
				DataTable dataTable = (DataTable)luk.Properties.DataSource;
				if (dataTable.IsNotNull())
				{
					dataTable.Clear();
					dataTable.AcceptChanges();
					luk.Properties.DataSource = dataTable;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadComboState(string code, int type)
	{
		try
		{
			DataTable dataTable = _httpClient.EmployeeMasterClient.LoadComboState(code, type);
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("Code", typeof(string));
			dataTable2.Columns.Add("Name", typeof(string));
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow dataRow = dataTable2.NewRow();
				if (!string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["Code"])))
				{
					dataRow["Code"] = Function.ToString(dataTable.Rows[i]["Code"]);
					dataRow["Name"] = Function.ToString(dataTable.Rows[i]["Name"]);
					dataTable2.Rows.Add(dataRow);
					dataTable2.AcceptChanges();
				}
			}
			if ((type == 1 && !string.IsNullOrEmpty(Function.ToString(luk_W_Country.EditValue))) || (type == 2 && !string.IsNullOrEmpty(Function.ToString(luk_H_Country.EditValue))))
			{
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["Code"] = -10000;
				dataRow2["Name"] = "-Define New-";
				dataTable2.Rows.Add(dataRow2);
				dataTable2.AcceptChanges();
			}
			switch (type)
			{
			case 1:
				SetDataSourceByDataTable(luk_W_State, dataTable2);
				break;
			case 2:
				SetDataSourceByDataTable(luk_H_State, dataTable2);
				break;
			default:
				SetDataSourceByDataTable(luk_W_State, dataTable2);
				SetDataSourceByDataTable(luk_H_State, dataTable2);
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_contractType_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!_isLoading && Function.ParseInt(e.NewValue) == -2)
			{
				frm_EmpContractTypes frm_EmpContractTypes2 = new frm_EmpContractTypes();
				frm_EmpContractTypes2.MdiParent = MdiParent;
				frm_EmpContractTypes2.childSend = parentGet;
				frm_EmpContractTypes2.Show();
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void parentGet(int selectedIndex)
	{
		try
		{
			gv_Contract.BeginUpdate();
			LoadLookUpContractType();
			gv_Contract.SetFocusedRowCellValue("type", selectedIndex);
			gv_Contract.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task GetDetails(int empId, bool reloadContract = true)
	{
		_ = 2;
		try
		{
			_isLoading = true;
			WinCommons._OpenProcessing();
			DataSet dataSet = await _httpClient.EmployeeMasterClient.GetDetail(empId);
			if (dataSet.IsNotNull() && dataSet.Tables.Count > 2)
			{
				if (dataSet.Tables[0].Rows.Count <= 0)
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
					await MODE_FIND();
					WinCommons._CloseProcessing();
					return;
				}
				this.ResetData();
				BindControlsValue("OHEM", dataSet.Tables[0]);
				if (dataSet.Tables[0].Columns.Contains("Code"))
				{
					lbl_empCode.Enabled = true;
					txt_empCode.Enabled = true;
				}
				if (luk_SalesEmp.EditValue == null)
				{
					luk_SalesEmp.EditValue = -1;
				}
				_defaultRole = Function.ParseInt(dataSet.Tables[0].Rows[0]["type"]);
				grd_Roles.DataSource = dataSet.Tables[1];
				grd_Teams.DataSource = dataSet.Tables[7];
				LoadLookkUpRoles();
				LoadLookUpTeams();
				int absEntry = Function.ParseInt(dataSet.Tables[0].Rows[0]["AtcEntry"]);
				Task task = ucAttachment1.LoadAttachments(absEntry);
				_dtAbsence = dataSet.Tables[3];
				_dtEducation = dataSet.Tables[4];
				_dtReview = dataSet.Tables[5];
				_dtPrevEmp = dataSet.Tables[6];
				_dtHealth = dataSet.Tables[8];
				_dtContract = dataSet.Tables[9];
				if (dataSet.Tables[0].Columns.Cast<DataColumn>().Any((DataColumn c) => c.ColumnName.ToUpper().StartsWith("U_")))
				{
					DataTable dataTable = dataSet.Tables[0];
					dataTable.SetDefaultAndMaxLen("OHEM");
					grd_Udf.DataSource = dataTable;
					grd_Udf.RefreshDataSource();
				}
				if (reloadContract)
				{
					RefreshGridContract();
				}
				if (reloadContract)
				{
					FormMode = Enums.FormViewModeType.View;
					btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
				}
				if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza != "R"))
				{
					pic_Images.Enabled = true;
					luk_Manager1.ReadOnly = false;
					luk_CostCenter.ReadOnly = false;
				}
				txt_EmpNo.ReadOnly = true;
				tab_Membership.PageEnabled = true;
				tab_Attachments.PageEnabled = true;
				tab_Contract.PageEnabled = true;
				tab_User.PageEnabled = true;
				EnableAdminTabButtons();
				luk_CostCenter.Properties.RefreshDataSource();
				WinCommons._CloseProcessing();
				if (dataSet.Tables[0].Rows.Count > 0)
				{
					_empName = Function.ToString(dataSet.Tables[0].Rows[0]["firstName"]) + " " + Function.ToString(dataSet.Tables[0].Rows[0]["lastName"]);
					chk_Active.Checked = Function.ToString(dataSet.Tables[0].Rows[0]["Active"]).Equals("Y");
					pic_Images.Properties.NullText = "Loading...";
					_cloudImage = Function.ToString(dataSet.Tables[0].Rows[0]["CloudImage"]);
					_strImage = Function.ToString(dataSet.Tables[0].Rows[0]["picture"]);
					if (!string.IsNullOrEmpty(_cloudImage) || !string.IsNullOrEmpty(_strImage))
					{
						LoadPicture();
					}
					else
					{
						pic_Images.Image = null;
						pic_Images.Properties.NullText = "No image data!";
					}
				}
				await task;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			txt_FirstName.Focus();
			WinCommons._CloseProcessing();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			_isLoading = false;
			WinCommons._CloseProcessing();
		}
	}

	private void RefreshGridContract()
	{
		try
		{
			DataTable dataTable = _dtContract.Copy();
			DataRow dataRow = dataTable.NewRow();
			dataRow["contractID"] = -1;
			dataTable.Rows.InsertAt(dataRow, 0);
			dataTable.AcceptChanges();
			rep_luk_ParentContract.DataSource = dataTable;
			listContract.Clear();
			foreach (var item in from s in dataTable.AsEnumerable()
				select new
				{
					id = Function.ParseInt(s["contractID"])
				})
			{
				listContract.Add(item.id);
			}
			if (!_dtContract.ExistsColumn("IsNewRow"))
			{
				_dtContract.Columns.Add("IsNewRow", typeof(string));
			}
			foreach (DataRow item2 in _dtContract.AsEnumerable())
			{
				item2["IsNewRow"] = "N";
			}
			grd_Contract.DataSource = _dtContract;
			AddNewRow_Contract();
			int focusedRowHandle = gv_Contract.LocateByValue("IsNewRow", "Y");
			gv_Contract.FocusedRowHandle = focusedRowHandle;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetEmployeeList()
	{
		try
		{
			dtOHEM = await _httpClient.EmployeeMasterClient.LoadEmployees();
			if (!dtOHEM.IsNotNull())
			{
				dtOHEM = WinFuntions.GetTableStructure("OHEM");
			}
			if (dicOHEM != null)
			{
				dicOHEM.Clear();
			}
			dicOHEM = dtOHEM.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["empID"]), (DataRow s) => s["lastName"]?.ToString() + ", " + s["firstName"]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadUDF()
	{
		try
		{
			Dictionary<string, GridViewEx> dictionary = new Dictionary<string, GridViewEx> { ["OHEM"] = gv_Udf };
			dictionary["OHEM"] = gv_Udf;
			Task task = WinFuntions.LoadUdfColumns(gv_Udf, "OHEM", dictionary);
			Task task2 = WinFuntions.LoadUdfColumns(gv_Contract, "APZ_OHEC");
			await Task.WhenAll(task, task2);
			DataTable dataSourceTable = grd_Udf.DataSourceTable;
			if (dataSourceTable == null || dataSourceTable.Rows.Count == 0)
			{
				WinFuntions.GetNewRow_UDF(grd_Udf, "OHEM");
			}
			gv_Udf.CellValueChanged += OnContentChanged;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDataNavigations(int navType)
	{
		_ = 1;
		try
		{
			if ((btn_Save.Text == Apzon.Commons.Properties.Resources.ButtonUpdate || (btn_Save.Text == Apzon.Commons.Properties.Resources.ButtonAdd && _add)) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			WinCommons._OpenProcessing();
			EmpId = Function.ParseInt(await _httpClient.EmployeeMasterClient.LoadDataNavigation(EmpId, navType));
			await GetDetails(EmpId);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		await LoadDataNavigations(1);
	}

	public async Task MOVE_NEXT()
	{
		await LoadDataNavigations(3);
	}

	public async Task MOVE_LAST()
	{
		await LoadDataNavigations(4);
	}

	public async Task MOVE_PREVIOUS()
	{
		await LoadDataNavigations(2);
	}

	private void SetEventControl(Control ctr)
	{
		foreach (Control control in ctr.Controls)
		{
			if (control is CheckEdit checkEdit)
			{
				checkEdit.CheckStateChanged += OnContentChanged;
			}
			else if (control is ComboBoxEdit comboBoxEdit)
			{
				comboBoxEdit.SelectedIndexChanged += OnContentChanged;
			}
			else if (control is LookUpEdit lookUpEdit)
			{
				lookUpEdit.EditValueChanged += OnContentChanged;
				lookUpEdit.Properties.ShowHeader = false;
				lookUpEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			}
			else if (control is DateEdit dateEdit)
			{
				dateEdit.EditValueChanged += OnContentChanged;
			}
			else if (control is TextEdit || control is MemoEdit)
			{
				control.TextChanged += OnContentChanged;
				((BaseEdit)control).EditValueChanging += TextValueChanging;
			}
			else
			{
				SetEventControl(control);
			}
		}
	}

	private void TextValueChanging(object sender, ChangingEventArgs e)
	{
	}

	private void SetSuperTip(Control ctr)
	{
		foreach (Control control in ctr.Controls)
		{
			if ((control as BaseEdit) is ButtonEditCFL)
			{
				SuperToolTip superToolTip = new SuperToolTip();
				ToolTipItem item = new ToolTipItem
				{
					Text = Function.ToString(((ButtonEditCFL)control).Text)
				};
				superToolTip.Items.Add(item);
				((ButtonEditCFL)control).SuperTip = superToolTip;
			}
			else if (!(control is CheckEdit))
			{
				if (control is ComboBoxEdit comboBoxEdit)
				{
					SuperToolTip superToolTip2 = new SuperToolTip();
					ToolTipItem item2 = new ToolTipItem
					{
						Text = Function.ToString(comboBoxEdit.EditValue)
					};
					superToolTip2.Items.Add(item2);
					comboBoxEdit.SuperTip = superToolTip2;
				}
				else if (control is LookUpEdit lookUpEdit)
				{
					SuperToolTip superToolTip3 = new SuperToolTip();
					ToolTipItem item3 = new ToolTipItem
					{
						Text = Function.ToString(lookUpEdit.Properties.GetDisplayText(lookUpEdit.EditValue))
					};
					superToolTip3.Items.Add(item3);
					lookUpEdit.SuperTip = superToolTip3;
				}
				else if (control is DateEdit dateEdit)
				{
					SuperToolTip superToolTip4 = new SuperToolTip();
					ToolTipItem item4 = new ToolTipItem
					{
						Text = Function.ToString(dateEdit.EditValue)
					};
					superToolTip4.Items.Add(item4);
					dateEdit.SuperTip = superToolTip4;
				}
				else if (control is TextEdit textEdit)
				{
					SuperToolTip superToolTip5 = new SuperToolTip();
					ToolTipItem item5 = new ToolTipItem
					{
						Text = Function.ToString(textEdit.EditValue)
					};
					superToolTip5.Items.Add(item5);
					textEdit.SuperTip = superToolTip5;
				}
				else
				{
					SetSuperTip(control);
				}
			}
		}
	}

	private void AddHandlers()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
			{
				SetEventControl(this);
				gv_Roles.RowCountChanged += OnContentChanged;
				gv_Teams.RowCountChanged += OnContentChanged;
				ucAttachment1.gv_File.RowCountChanged += OnContentChanged;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected void OnContentChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && !_isLoading)
			{
				if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "R"))
				{
					return;
				}
				FormMode = Enums.FormViewModeType.Update;
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			}
			if (FormMode == Enums.FormViewModeType.Find)
			{
				_edit = true;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				_add = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private static bool VerifyFileExists(string path, int timeout)
	{
		try
		{
			StringBuilder sb = new StringBuilder(path);
			Task<bool> task = new Task<bool>(() => PathFileExists(sb));
			task.Start();
			return task.Wait(timeout) && task.Result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private void LoadPicture()
	{
		try
		{
			string text = CopyFileShareHelper.GetUpLoadPath("jpg") + "\\" + _strImage;
			_picFilePath = "";
			if (!string.IsNullOrEmpty(_cloudImage))
			{
				pic_Images.LoadAsync(_cloudImage);
			}
			else if (!string.IsNullOrWhiteSpace(_strImage) && VerifyFileExists(text, 5000))
			{
				pic_Images.Image = null;
				string tempPath = Path.GetTempPath();
				tempPath = (_strImage.Contains("APZ-Employee-") ? (tempPath + _strImage) : (tempPath + "APZ-Employee-" + _strImage));
				string text2 = "";
				if (VerifyFileExists(tempPath, 5000))
				{
					text2 = _ftpHelper.DeleteFileSharing(CopyFileSharingCfg.Domain, tempPath, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password);
				}
				if (text2 != "")
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Cannot delete file [" + tempPath + "] for copy Employees picture from server!");
					return;
				}
				string text3 = _ftpHelper.CopyFileSharing(CopyFileSharingCfg.Domain, text, CopyFileSharingCfg.UserName, CopyFileSharingCfg.Password, tempPath, copyForOpen: true);
				if (text3 != tempPath)
				{
					WinCommons.Warning(Enums.MessageCode.UploadPictureFailed, "", text3);
					pic_Images.Properties.NullText = "Could not load Pic!";
				}
				else if (VerifyFileExists(tempPath, 5000))
				{
					using (FileStream stream = new FileStream(tempPath, FileMode.Open, FileAccess.Read))
					{
						pic_Images.Image = Image.FromStream(stream);
					}
					_picFilePath = tempPath;
				}
				else
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Cannot copy Picture from server to temp folder for preview!");
					WinCommons.Warning(Enums.MessageCode.PicturePathDoesNotExistsOnServerOrBadAuth, "");
					pic_Images.Properties.NullText = "Could not load Pic!";
				}
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.PicturePathDoesNotExistsOnServerOrBadAuth, "");
				pic_Images.Image = null;
				pic_Images.Properties.NullText = "Could not load Pic!";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		if (FormMode == Enums.FormViewModeType.Find)
		{
			btn_Save.Enabled = true;
			Controls.EnableDisableControl(isEnable: true);
			luk_Manager1.Properties.Buttons[0].Visible = true;
			tab_Membership.PageEnabled = false;
			tab_Attachments.PageEnabled = false;
			tab_Contract.PageEnabled = false;
			tab_User.PageEnabled = false;
			btn_Education.Enabled = false;
			btn_Absence.Enabled = false;
			btn_Review.Enabled = false;
			btn_PrevEmployment.Enabled = false;
			btn_Relatives.Enabled = false;
			btn_Health.Enabled = false;
			pic_Images.Enabled = false;
			luk_Manager1.ReadOnly = true;
			luk_CostCenter.ReadOnly = true;
		}
		else
		{
			btn_Save.Enabled = false;
			Controls.EnableDisableControl(isEnable: false);
			luk_Manager1.Properties.Buttons[0].Visible = false;
			EnableAdminTabButtons();
		}
	}

	private void EnableAdminTabButtons()
	{
		btn_Education.Enabled = true;
		btn_Absence.Enabled = true;
		btn_Review.Enabled = true;
		btn_PrevEmployment.Enabled = true;
		btn_Relatives.Enabled = true;
		btn_Health.Enabled = true;
	}

	private void calc_salary_Properties_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		int num = -1;
		string text = new string('#', (num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num);
		e.DisplayText = string.Format("{0:#,0." + text + "} {1}", e.Value, GlobalData.MainCurrency);
	}

	public override void CustomChangeLogParam(out GetChangeLogParams changelog)
	{
		changelog = new GetChangeLogParams
		{
			Object = Enums.ChangeLog.clEmployeesInfo,
			PrimaryKey = Function.ToString(txt_EmpNo.EditValue)
		};
	}

	private async void organizationalChartToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_OrganizationalChart frm = new frm_OrganizationalChart();
			frm.Key = "empID";
			frm.Father = "manager";
			frm.FormCaption = "Employee's Organization Chart";
			int empID = Function.ParseInt(EmpId);
			DataTable source = await _httpClient.EmployeeMasterClient.GetOrgChartData(empID);
			frm.CurrentKeyVal = Function.ToString(empID);
			frm.Source = source;
			frm.MdiParent = MdiParent;
			frm.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private void pic_Images_EditValueChanged(object sender, EventArgs e)
	{
	}

	private void ucAttachment1_ButtonBrowse_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gv_Contract_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && !_isChanging)
			{
				_isChanging = true;
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
					btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				}
				string text = Function.ToString(gv_Contract.GetRowCellValue(e.RowHandle, "IsNewRow"));
				string text2 = Function.ToString(gv_Contract.GetRowCellValue(e.RowHandle, "code"));
				string text3 = Function.ToString(gv_Contract.GetRowCellValue(e.RowHandle, "signDate"));
				string text4 = Function.ToString(gv_Contract.GetRowCellValue(e.RowHandle, "startDate"));
				if (text == "Y" && text2 != "" && text3 != "" && text4 != "" && (e.Column.FieldName == "code" || e.Column.FieldName == "signDate" || e.Column.FieldName == "startDate"))
				{
					gv_Contract.SetRowCellValue(e.RowHandle, "IsNewRow", "N");
					AddNewRow_Contract();
				}
				_isChanging = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void luk_salaryCurr_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			foreach (CurrencyCodesImpl salaryCurrDatum in salaryCurrData)
			{
				if (salaryCurrDatum.CurrCode == Function.ToString(gv_Contract.GetFocusedRowCellValue("salaryCurr")))
				{
					string text = "0";
					if (salaryCurrDatum.Decimals != -1)
					{
						text = $"n{Function.ToString(salaryCurrDatum.Decimals)}";
						cal_Salary.DisplayFormat.FormatString = text;
					}
					else
					{
						text = $"n{GlobalData.QuantitiesFormat.NumberDecimalDigits}";
						cal_Salary.DisplayFormat.FormatString = text;
					}
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void contextMenuContract_DeleteRow_Click(object sender, EventArgs e)
	{
		try
		{
			if (!grd_Contract.DataSourceTable.IsNotNull())
			{
				return;
			}
			gv_Contract.BeginUpdate();
			gv_Contract.DeleteSelectedRows();
			gv_Contract.EndUpdate();
			grd_Contract.DataSourceTable.AcceptChanges();
			if (dicButtonEditValueType.ContainsKey(gv_Contract.FocusedRowHandle))
			{
				dicButtonEditValueType.Remove(gv_Contract.FocusedRowHandle);
			}
			Dictionary<int, string> dictionary = dicButtonEditValueType.ToDictionary((KeyValuePair<int, string> d) => (d.Key >= gv_Contract.FocusedRowHandle) ? (d.Key - 1) : d.Key, (KeyValuePair<int, string> d) => d.Value);
			dicButtonEditValueType.Clear();
			foreach (KeyValuePair<int, string> item in dictionary)
			{
				dicButtonEditValueType.Add(item.Key, item.Value);
			}
			AddNewRow_Contract();
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
				btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRow_Contract()
	{
		try
		{
			if (!grd_Contract.DataSourceTable.HasColumn())
			{
				grd_Contract.DataSource = _dtContract;
			}
			DataTable dataTable = grd_Contract.DataSource as DataTable;
			if (!dataTable.ExistsColumn("IsNewRow"))
			{
				dataTable.Columns.Add("IsNewRow", typeof(string));
			}
			dataTable.AcceptChanges();
			if (!dataTable.AsEnumerable().Any((DataRow s) => Function.ToString(s["IsNewRow"]) == "Y"))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["contractID"] = 0;
				dataRow["code"] = "";
				dataRow["empID"] = Function.ParseInt(txt_EmpNo.Text);
				dataRow["type"] = -1;
				dataRow["signer"] = DBNull.Value;
				dataRow["signDate"] = DBNull.Value;
				dataRow["startDate"] = DBNull.Value;
				dataRow["endDate"] = DBNull.Value;
				dataRow["ProbSalary"] = DBNull.Value;
				dataRow["basicSalary"] = DBNull.Value;
				dataRow["salary"] = DBNull.Value;
				dataRow["salaryCurr"] = GlobalData.MainCurrency;
				dataRow["PymMeth"] = "5";
				dataRow["note"] = "";
				dataRow["BPLId"] = -1;
				dataRow["IsNewRow"] = "Y";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				_dtContract = grd_Contract.DataSourceTable;
				int rowHandle = gv_Contract.LocateByValue("IsNewRow", "Y");
				gv_Contract.MakeRowVisible(rowHandle);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_luk_ParentContract_PopupFilter(object sender, PopupFilterEventArgs e)
	{
		try
		{
			List<int> list = listContract.ToList();
			int item = Function.ParseInt(gv_Contract.GetFocusedRowCellValue("contractID"));
			if (list.Contains(item))
			{
				list.Remove(item);
			}
			e.Criteria = new InOperator("contractID", list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_HumanResources));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		this.lbl_FirstName = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Department = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Position = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Job = new Apzon.Controls.Controls.LabelLine();
		this.lbl_LastName = new Apzon.Controls.Controls.LabelLine();
		this.lbl_MiddleName = new Apzon.Controls.Controls.LabelLine();
		this.lbl_UserCode = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Manager = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Branch = new Apzon.Controls.Controls.LabelLine();
		this.lbl_EmpNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Pager = new Apzon.Controls.Controls.LabelLine();
		this.lbl_MPhone = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ext = new Apzon.Controls.Controls.LabelLine();
		this.lbl_off = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ExtEmp = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Home = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Fax = new Apzon.Controls.Controls.LabelLine();
		this.txt_FirstName = new Apzon.Controls.Editor.TextEditEx();
		this.txt_MiddleName = new Apzon.Controls.Editor.TextEditEx();
		this.txt_LastName = new Apzon.Controls.Editor.TextEditEx();
		this.txt_JobTitle = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_SalesEmp = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Email = new Apzon.Controls.Controls.LabelLine();
		this.txt_EmpNo = new Apzon.Controls.Editor.TextEditEx();
		this.txt_ExtEmpNo = new Apzon.Controls.Editor.TextEditEx();
		this.chk_Active = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_OfficePhone = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Pager = new Apzon.Controls.Editor.TextEditEx();
		this.txt_MobilePhone = new Apzon.Controls.Editor.TextEditEx();
		this.txt_ExtPhone = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Email = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Fax = new Apzon.Controls.Editor.TextEditEx();
		this.txt_HomePhone = new Apzon.Controls.Editor.TextEditEx();
		this.tab_Detail = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Address = new Apzon.Controls.Controls.XtraTabPageEx();
		this.pnl_Address = new DevExpress.XtraEditors.PanelControl();
		this.luk_H_State = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_H_Country = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_W_Country = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_W_State = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_H_Block = new Apzon.Controls.Editor.TextEditEx();
		this.txt_H_StreetNo = new Apzon.Controls.Editor.TextEditEx();
		this.txt_H_Street = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_H_StrNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_H_Block = new Apzon.Controls.Controls.LabelLine();
		this.lbl_H_Street = new Apzon.Controls.Controls.LabelLine();
		this.txt_W_Street = new Apzon.Controls.Editor.TextEditEx();
		this.txt_W_Block = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_WStreet = new Apzon.Controls.Controls.LabelLine();
		this.txt_W_StreetNo = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_StrNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Block = new Apzon.Controls.Controls.LabelLine();
		this.txt_H_County = new Apzon.Controls.Editor.TextEditEx();
		this.txt_H_City = new Apzon.Controls.Editor.TextEditEx();
		this.txt_H_Zipcode = new Apzon.Controls.Editor.TextEditEx();
		this.txt_H_Build = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_H_zipCode = new Apzon.Controls.Controls.LabelLine();
		this.lbl_H_city = new Apzon.Controls.Controls.LabelLine();
		this.lbl_H_County = new Apzon.Controls.Controls.LabelLine();
		this.lbl_H_State = new Apzon.Controls.Controls.LabelLine();
		this.lbl_H_Country = new Apzon.Controls.Controls.LabelLine();
		this.lbl_H_Build = new Apzon.Controls.Controls.LabelLine();
		this.lbl_WorkAdress = new Apzon.Controls.Controls.LabelLine();
		this.lbl_HomeAddress = new Apzon.Controls.Controls.LabelLine();
		this.txt_W_County = new Apzon.Controls.Editor.TextEditEx();
		this.txt_W_Build = new Apzon.Controls.Editor.TextEditEx();
		this.txt_W_City = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_Build = new Apzon.Controls.Controls.LabelLine();
		this.txt_W_Zipcode = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_Country = new Apzon.Controls.Controls.LabelLine();
		this.lbl_State = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ZipCode = new Apzon.Controls.Controls.LabelLine();
		this.lbl_County = new Apzon.Controls.Controls.LabelLine();
		this.lbl_City = new Apzon.Controls.Controls.LabelLine();
		this.tab_Membership = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_SetRole = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Roles = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Roles = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Role = new Apzon.Controls.Grid.GridColumnEx();
		this.rlup_Role = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.lbl_Roles = new Apzon.Controls.Controls.LabelLine();
		this.grd_Teams = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Teams = new Apzon.Controls.Grid.GridViewEx();
		this.cl_Team = new Apzon.Controls.Grid.GridColumnEx();
		this.rlup_Team = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_TeamRole = new Apzon.Controls.Grid.GridColumnEx();
		this.rlup_TeamRole = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_lineNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.Tab_Administration = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.date_TermDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_StartDate = new Apzon.Controls.Editor.DateEditEx();
		this.btn_Health = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Relatives = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_Status = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_TermReason = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.btn_PrevEmployment = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Review = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Education = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Absence = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.tab_Personal = new Apzon.Controls.Controls.XtraTabPageEx();
		this.pnl_Personal = new DevExpress.XtraEditors.PanelControl();
		this.date_PassportDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_PassExp = new Apzon.Controls.Editor.DateEditEx();
		this.date_Brith = new Apzon.Controls.Editor.DateEditEx();
		this.luk_MartialStt = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_CountryOfBirth = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_Citizenship = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_Gender = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.spin_NoChildren = new Apzon.Controls.Editor.SpinEditEx();
		this.txt_PasIss = new Apzon.Controls.Editor.TextEditEx();
		this.txt_PassNo = new Apzon.Controls.Editor.TextEditEx();
		this.txt_IDNo = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_PassNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_PassExpDate = new Apzon.Controls.Controls.LabelLine();
		this.lbl_PassIss = new Apzon.Controls.Controls.LabelLine();
		this.lbl_PasIssuer = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Citizenship = new Apzon.Controls.Controls.LabelLine();
		this.lbl_IDNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Date = new Apzon.Controls.Controls.LabelLine();
		this.lbl_CountryOfBirth = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Marital = new Apzon.Controls.Controls.LabelLine();
		this.lbl_NoOfChildren = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Gender = new Apzon.Controls.Controls.LabelLine();
		this.tab_Contract = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl4 = new DevExpress.XtraEditors.PanelControl();
		this.grd_Contract = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuContract = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.contextMenuContract_DeleteRow = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Contract = new Apzon.Controls.Grid.GridViewEx();
		this.contractID = new Apzon.Controls.Grid.GridColumnEx();
		this.code = new Apzon.Controls.Grid.GridColumnEx();
		this.type = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_contractType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcSignedBy = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_SignedBy = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.signDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.startDate = new Apzon.Controls.Grid.GridColumnEx();
		this.endDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.salary = new Apzon.Controls.Grid.GridColumnEx();
		this.cal_Salary = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcProbSalary = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBasicSalary = new Apzon.Controls.Grid.GridColumnEx();
		this.gcSalaryCurr = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_salaryCurr = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcPayment = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_PymMeth = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.note = new Apzon.Controls.Grid.GridColumnEx();
		this.gcParentContract = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_luk_ParentContract = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.IsNewRow = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_cfl_SubContract = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.tab_Finance = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl3 = new DevExpress.XtraEditors.PanelControl();
		this.txt_BankCode = new Apzon.Controls.Editor.TextEditEx();
		this.luk_BankCode = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_bankAccount = new Apzon.Controls.Editor.TextEditEx();
		this.txt_bankBranch = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine11 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine10 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.calc_EmplCost = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.calc_salary = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.luk_salaryUnit = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_empCostUnt = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.tab_Remarks = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_Remark = new Apzon.Controls.Editor.MemoEditEx();
		this.tab_Attachments = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl5 = new DevExpress.XtraEditors.PanelControl();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.tab_User = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Udf = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Udf = new Apzon.Controls.Grid.GridViewEx();
		this.pic_Images = new DevExpress.XtraEditors.PictureEdit();
		this.contextMenuStrip3 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deletePictureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.luk_Position = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_Department = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_Branch = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_UserCode = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_SalesEmp = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.contextMenuStrip4 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeEmployeeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.organizationalChartToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.luk_Manager1 = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.luk_CostCenter = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.lbl_CostCenter = new Apzon.Controls.Controls.LabelLine();
		this.luk_LinkedVendor = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.lbl_LinkedVendor = new Apzon.Controls.Controls.LabelLine();
		this.txt_empCode = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_empCode = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.txt_FirstName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MiddleName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LastName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_JobTitle.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ExtEmpNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OfficePhone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Pager.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MobilePhone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ExtPhone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Email.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_HomePhone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tab_Detail).BeginInit();
		this.tab_Detail.SuspendLayout();
		this.tab_Address.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pnl_Address).BeginInit();
		this.pnl_Address.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_H_State.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_H_Country.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_W_Country.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_W_State.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Block.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_StreetNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Street.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Street.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Block.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_StreetNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_County.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_City.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Zipcode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Build.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_County.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Build.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_City.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Zipcode.Properties).BeginInit();
		this.tab_Membership.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Roles).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Roles).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rlup_Role).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Teams).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Teams).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rlup_Team).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rlup_TeamRole).BeginInit();
		this.Tab_Administration.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_TermDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_TermDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TermReason.Properties).BeginInit();
		this.tab_Personal.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pnl_Personal).BeginInit();
		this.pnl_Personal.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_PassportDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_PassportDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_PassExp.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_PassExp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Brith.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Brith.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_MartialStt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CountryOfBirth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Citizenship.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Gender.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.spin_NoChildren.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PasIss.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PassNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_IDNo.Properties).BeginInit();
		this.tab_Contract.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl4).BeginInit();
		this.panelControl4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Contract).BeginInit();
		this.contextMenuContract.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Contract).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_contractType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_SignedBy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Salary).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_salaryCurr).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PymMeth).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_ParentContract).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_SubContract).BeginInit();
		this.tab_Finance.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl3).BeginInit();
		this.panelControl3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_BankCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BankCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_bankAccount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_bankBranch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_EmplCost.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_salary.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_salaryUnit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_empCostUnt.Properties).BeginInit();
		this.tab_Remarks.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Remark.Properties).BeginInit();
		this.tab_Attachments.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl5).BeginInit();
		this.panelControl5.SuspendLayout();
		this.tab_User.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Udf).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Udf).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pic_Images.Properties).BeginInit();
		this.contextMenuStrip3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_Position.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_UserCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SalesEmp.Properties).BeginInit();
		this.contextMenuStrip4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_Manager1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CostCenter.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_LinkedVendor.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_empCode.Properties).BeginInit();
		base.SuspendLayout();
		this.lbl_FirstName.EditControl = null;
		this.lbl_FirstName.EditValue = "First Name";
		this.lbl_FirstName.FieldName = null;
		this.lbl_FirstName.HideWhenEmpty = false;
		this.lbl_FirstName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_FirstName.Location = new System.Drawing.Point(13, 8);
		this.lbl_FirstName.Name = "lbl_FirstName";
		this.lbl_FirstName.ReadOnly = false;
		this.lbl_FirstName.SettingInVisible = false;
		this.lbl_FirstName.SettingReadonly = false;
		this.lbl_FirstName.ShowUnderLine = true;
		this.lbl_FirstName.Size = new System.Drawing.Size(117, 20);
		this.lbl_FirstName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_FirstName.TabIndex = 67;
		this.lbl_FirstName.Utext = "First Name";
		this.lbl_Department.EditControl = null;
		this.lbl_Department.EditValue = "Department";
		this.lbl_Department.FieldName = null;
		this.lbl_Department.HideWhenEmpty = false;
		this.lbl_Department.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Department.Location = new System.Drawing.Point(13, 124);
		this.lbl_Department.Name = "lbl_Department";
		this.lbl_Department.ReadOnly = false;
		this.lbl_Department.SettingInVisible = false;
		this.lbl_Department.SettingReadonly = false;
		this.lbl_Department.ShowUnderLine = true;
		this.lbl_Department.Size = new System.Drawing.Size(117, 20);
		this.lbl_Department.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Department.TabIndex = 72;
		this.lbl_Department.Utext = "Department";
		this.lbl_Position.EditControl = null;
		this.lbl_Position.EditValue = "Position";
		this.lbl_Position.FieldName = null;
		this.lbl_Position.HideWhenEmpty = false;
		this.lbl_Position.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Position.Location = new System.Drawing.Point(13, 100);
		this.lbl_Position.Name = "lbl_Position";
		this.lbl_Position.ReadOnly = false;
		this.lbl_Position.SettingInVisible = false;
		this.lbl_Position.SettingReadonly = false;
		this.lbl_Position.ShowUnderLine = true;
		this.lbl_Position.Size = new System.Drawing.Size(117, 20);
		this.lbl_Position.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Position.TabIndex = 71;
		this.lbl_Position.Utext = "Position";
		this.lbl_Job.EditControl = null;
		this.lbl_Job.EditValue = "Job Title";
		this.lbl_Job.FieldName = null;
		this.lbl_Job.HideWhenEmpty = false;
		this.lbl_Job.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Job.Location = new System.Drawing.Point(13, 77);
		this.lbl_Job.Name = "lbl_Job";
		this.lbl_Job.ReadOnly = false;
		this.lbl_Job.SettingInVisible = false;
		this.lbl_Job.SettingReadonly = false;
		this.lbl_Job.ShowUnderLine = true;
		this.lbl_Job.Size = new System.Drawing.Size(117, 20);
		this.lbl_Job.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Job.TabIndex = 70;
		this.lbl_Job.Utext = "Job Title";
		this.lbl_LastName.EditControl = null;
		this.lbl_LastName.EditValue = "Last Name";
		this.lbl_LastName.FieldName = null;
		this.lbl_LastName.HideWhenEmpty = false;
		this.lbl_LastName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_LastName.Location = new System.Drawing.Point(13, 54);
		this.lbl_LastName.Name = "lbl_LastName";
		this.lbl_LastName.ReadOnly = false;
		this.lbl_LastName.SettingInVisible = false;
		this.lbl_LastName.SettingReadonly = false;
		this.lbl_LastName.ShowUnderLine = true;
		this.lbl_LastName.Size = new System.Drawing.Size(117, 20);
		this.lbl_LastName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_LastName.TabIndex = 69;
		this.lbl_LastName.Utext = "Last Name";
		this.lbl_MiddleName.EditControl = null;
		this.lbl_MiddleName.EditValue = "Middle Name";
		this.lbl_MiddleName.FieldName = null;
		this.lbl_MiddleName.HideWhenEmpty = false;
		this.lbl_MiddleName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_MiddleName.Location = new System.Drawing.Point(13, 31);
		this.lbl_MiddleName.Name = "lbl_MiddleName";
		this.lbl_MiddleName.ReadOnly = false;
		this.lbl_MiddleName.SettingInVisible = false;
		this.lbl_MiddleName.SettingReadonly = false;
		this.lbl_MiddleName.ShowUnderLine = true;
		this.lbl_MiddleName.Size = new System.Drawing.Size(117, 20);
		this.lbl_MiddleName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_MiddleName.TabIndex = 68;
		this.lbl_MiddleName.Utext = "Middle Name";
		this.lbl_UserCode.EditControl = null;
		this.lbl_UserCode.EditValue = "User Code";
		this.lbl_UserCode.FieldName = null;
		this.lbl_UserCode.HideWhenEmpty = false;
		this.lbl_UserCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_UserCode.Location = new System.Drawing.Point(13, 196);
		this.lbl_UserCode.Name = "lbl_UserCode";
		this.lbl_UserCode.ReadOnly = false;
		this.lbl_UserCode.SettingInVisible = false;
		this.lbl_UserCode.SettingReadonly = false;
		this.lbl_UserCode.ShowUnderLine = true;
		this.lbl_UserCode.Size = new System.Drawing.Size(117, 20);
		this.lbl_UserCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_UserCode.TabIndex = 75;
		this.lbl_UserCode.Utext = "User Code";
		this.lbl_Manager.EditControl = null;
		this.lbl_Manager.EditValue = "Manager";
		this.lbl_Manager.FieldName = null;
		this.lbl_Manager.HideWhenEmpty = false;
		this.lbl_Manager.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Manager.Location = new System.Drawing.Point(13, 172);
		this.lbl_Manager.Name = "lbl_Manager";
		this.lbl_Manager.ReadOnly = false;
		this.lbl_Manager.SettingInVisible = false;
		this.lbl_Manager.SettingReadonly = false;
		this.lbl_Manager.ShowUnderLine = true;
		this.lbl_Manager.Size = new System.Drawing.Size(117, 20);
		this.lbl_Manager.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Manager.TabIndex = 74;
		this.lbl_Manager.Utext = "Manager";
		this.lbl_Branch.EditControl = null;
		this.lbl_Branch.EditValue = "Branch";
		this.lbl_Branch.FieldName = null;
		this.lbl_Branch.HideWhenEmpty = false;
		this.lbl_Branch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Branch.Location = new System.Drawing.Point(13, 148);
		this.lbl_Branch.Name = "lbl_Branch";
		this.lbl_Branch.ReadOnly = false;
		this.lbl_Branch.SettingInVisible = false;
		this.lbl_Branch.SettingReadonly = false;
		this.lbl_Branch.ShowUnderLine = true;
		this.lbl_Branch.Size = new System.Drawing.Size(117, 20);
		this.lbl_Branch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Branch.TabIndex = 73;
		this.lbl_Branch.Utext = "Branch";
		this.lbl_EmpNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_EmpNo.EditControl = null;
		this.lbl_EmpNo.EditValue = "Employee No.";
		this.lbl_EmpNo.FieldName = null;
		this.lbl_EmpNo.HideWhenEmpty = false;
		this.lbl_EmpNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_EmpNo.Location = new System.Drawing.Point(362, 9);
		this.lbl_EmpNo.Name = "lbl_EmpNo";
		this.lbl_EmpNo.ReadOnly = false;
		this.lbl_EmpNo.SettingInVisible = false;
		this.lbl_EmpNo.SettingReadonly = false;
		this.lbl_EmpNo.ShowUnderLine = true;
		this.lbl_EmpNo.Size = new System.Drawing.Size(126, 20);
		this.lbl_EmpNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_EmpNo.TabIndex = 77;
		this.lbl_EmpNo.Utext = "Employee No.";
		this.lbl_Pager.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Pager.EditControl = null;
		this.lbl_Pager.EditValue = "Pager";
		this.lbl_Pager.FieldName = null;
		this.lbl_Pager.HideWhenEmpty = false;
		this.lbl_Pager.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Pager.Location = new System.Drawing.Point(362, 106);
		this.lbl_Pager.Name = "lbl_Pager";
		this.lbl_Pager.ReadOnly = false;
		this.lbl_Pager.SettingInVisible = false;
		this.lbl_Pager.SettingReadonly = false;
		this.lbl_Pager.ShowUnderLine = true;
		this.lbl_Pager.Size = new System.Drawing.Size(126, 20);
		this.lbl_Pager.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Pager.TabIndex = 82;
		this.lbl_Pager.Utext = "Pager";
		this.lbl_MPhone.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_MPhone.EditControl = null;
		this.lbl_MPhone.EditValue = "Mobile Phone";
		this.lbl_MPhone.FieldName = null;
		this.lbl_MPhone.HideWhenEmpty = false;
		this.lbl_MPhone.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_MPhone.Location = new System.Drawing.Point(362, 176);
		this.lbl_MPhone.Name = "lbl_MPhone";
		this.lbl_MPhone.ReadOnly = false;
		this.lbl_MPhone.SettingInVisible = false;
		this.lbl_MPhone.SettingReadonly = false;
		this.lbl_MPhone.ShowUnderLine = true;
		this.lbl_MPhone.Size = new System.Drawing.Size(126, 20);
		this.lbl_MPhone.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_MPhone.TabIndex = 81;
		this.lbl_MPhone.Utext = "Mobile Phone";
		this.lbl_ext.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_ext.EditControl = null;
		this.lbl_ext.EditValue = "Ext.";
		this.lbl_ext.FieldName = null;
		this.lbl_ext.HideWhenEmpty = false;
		this.lbl_ext.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ext.Location = new System.Drawing.Point(362, 152);
		this.lbl_ext.Name = "lbl_ext";
		this.lbl_ext.ReadOnly = false;
		this.lbl_ext.SettingInVisible = false;
		this.lbl_ext.SettingReadonly = false;
		this.lbl_ext.ShowUnderLine = true;
		this.lbl_ext.Size = new System.Drawing.Size(126, 20);
		this.lbl_ext.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ext.TabIndex = 80;
		this.lbl_ext.Utext = "Ext.";
		this.lbl_off.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_off.EditControl = null;
		this.lbl_off.EditValue = "Office Phone";
		this.lbl_off.FieldName = null;
		this.lbl_off.HideWhenEmpty = false;
		this.lbl_off.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_off.Location = new System.Drawing.Point(362, 129);
		this.lbl_off.Name = "lbl_off";
		this.lbl_off.ReadOnly = false;
		this.lbl_off.SettingInVisible = false;
		this.lbl_off.SettingReadonly = false;
		this.lbl_off.ShowUnderLine = true;
		this.lbl_off.Size = new System.Drawing.Size(126, 20);
		this.lbl_off.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_off.TabIndex = 79;
		this.lbl_off.Utext = "Office Phone";
		this.lbl_ExtEmp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_ExtEmp.EditControl = null;
		this.lbl_ExtEmp.EditValue = "Extened Emp No.";
		this.lbl_ExtEmp.FieldName = null;
		this.lbl_ExtEmp.HideWhenEmpty = false;
		this.lbl_ExtEmp.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ExtEmp.Location = new System.Drawing.Point(362, 60);
		this.lbl_ExtEmp.Name = "lbl_ExtEmp";
		this.lbl_ExtEmp.ReadOnly = false;
		this.lbl_ExtEmp.SettingInVisible = false;
		this.lbl_ExtEmp.SettingReadonly = false;
		this.lbl_ExtEmp.ShowUnderLine = true;
		this.lbl_ExtEmp.Size = new System.Drawing.Size(126, 20);
		this.lbl_ExtEmp.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ExtEmp.TabIndex = 78;
		this.lbl_ExtEmp.Utext = "Extened Emp No.";
		this.lbl_Home.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Home.EditControl = null;
		this.lbl_Home.EditValue = "Home Phone";
		this.lbl_Home.FieldName = null;
		this.lbl_Home.HideWhenEmpty = false;
		this.lbl_Home.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Home.Location = new System.Drawing.Point(362, 201);
		this.lbl_Home.Name = "lbl_Home";
		this.lbl_Home.ReadOnly = false;
		this.lbl_Home.SettingInVisible = false;
		this.lbl_Home.SettingReadonly = false;
		this.lbl_Home.ShowUnderLine = true;
		this.lbl_Home.Size = new System.Drawing.Size(126, 20);
		this.lbl_Home.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Home.TabIndex = 83;
		this.lbl_Home.Utext = "Home Phone";
		this.lbl_Fax.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Fax.EditControl = null;
		this.lbl_Fax.EditValue = "Fax";
		this.lbl_Fax.FieldName = null;
		this.lbl_Fax.HideWhenEmpty = false;
		this.lbl_Fax.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Fax.Location = new System.Drawing.Point(362, 225);
		this.lbl_Fax.Name = "lbl_Fax";
		this.lbl_Fax.ReadOnly = false;
		this.lbl_Fax.SettingInVisible = false;
		this.lbl_Fax.SettingReadonly = false;
		this.lbl_Fax.ShowUnderLine = true;
		this.lbl_Fax.Size = new System.Drawing.Size(126, 20);
		this.lbl_Fax.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Fax.TabIndex = 84;
		this.lbl_Fax.Utext = "Fax";
		this.txt_FirstName.EditControl = null;
		this.txt_FirstName.FieldName = "firstName";
		this.txt_FirstName.IsSearchControl = false;
		this.txt_FirstName.Location = new System.Drawing.Point(129, 5);
		this.txt_FirstName.Name = "txt_FirstName";
		this.txt_FirstName.SettingInVisible = false;
		this.txt_FirstName.SettingReadonly = false;
		this.txt_FirstName.Size = new System.Drawing.Size(167, 20);
		this.txt_FirstName.TabIndex = 1;
		this.txt_FirstName.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_MiddleName.EditControl = null;
		this.txt_MiddleName.FieldName = "middleName";
		this.txt_MiddleName.IsSearchControl = false;
		this.txt_MiddleName.Location = new System.Drawing.Point(129, 28);
		this.txt_MiddleName.Name = "txt_MiddleName";
		this.txt_MiddleName.SettingInVisible = false;
		this.txt_MiddleName.SettingReadonly = false;
		this.txt_MiddleName.Size = new System.Drawing.Size(167, 20);
		this.txt_MiddleName.TabIndex = 2;
		this.txt_MiddleName.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_LastName.EditControl = null;
		this.txt_LastName.FieldName = "lastName";
		this.txt_LastName.IsSearchControl = false;
		this.txt_LastName.Location = new System.Drawing.Point(129, 51);
		this.txt_LastName.Name = "txt_LastName";
		this.txt_LastName.SettingInVisible = false;
		this.txt_LastName.SettingReadonly = false;
		this.txt_LastName.Size = new System.Drawing.Size(167, 20);
		this.txt_LastName.TabIndex = 3;
		this.txt_LastName.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_JobTitle.EditControl = null;
		this.txt_JobTitle.FieldName = "jobTitle";
		this.txt_JobTitle.IsSearchControl = false;
		this.txt_JobTitle.Location = new System.Drawing.Point(129, 74);
		this.txt_JobTitle.Name = "txt_JobTitle";
		this.txt_JobTitle.SettingInVisible = false;
		this.txt_JobTitle.SettingReadonly = false;
		this.txt_JobTitle.Size = new System.Drawing.Size(167, 20);
		this.txt_JobTitle.TabIndex = 4;
		this.txt_JobTitle.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.lbl_SalesEmp.EditControl = null;
		this.lbl_SalesEmp.EditValue = "Sales Employee";
		this.lbl_SalesEmp.FieldName = null;
		this.lbl_SalesEmp.HideWhenEmpty = false;
		this.lbl_SalesEmp.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_SalesEmp.Location = new System.Drawing.Point(13, 220);
		this.lbl_SalesEmp.Name = "lbl_SalesEmp";
		this.lbl_SalesEmp.ReadOnly = false;
		this.lbl_SalesEmp.SettingInVisible = false;
		this.lbl_SalesEmp.SettingReadonly = false;
		this.lbl_SalesEmp.ShowUnderLine = true;
		this.lbl_SalesEmp.Size = new System.Drawing.Size(117, 20);
		this.lbl_SalesEmp.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_SalesEmp.TabIndex = 76;
		this.lbl_SalesEmp.Utext = "Sales Employee";
		this.lbl_Email.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Email.EditControl = null;
		this.lbl_Email.EditValue = "E-Mail";
		this.lbl_Email.FieldName = null;
		this.lbl_Email.HideWhenEmpty = false;
		this.lbl_Email.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Email.Location = new System.Drawing.Point(362, 249);
		this.lbl_Email.Name = "lbl_Email";
		this.lbl_Email.ReadOnly = false;
		this.lbl_Email.SettingInVisible = false;
		this.lbl_Email.SettingReadonly = false;
		this.lbl_Email.ShowUnderLine = true;
		this.lbl_Email.Size = new System.Drawing.Size(126, 20);
		this.lbl_Email.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Email.TabIndex = 85;
		this.lbl_Email.Utext = "E-Mail";
		this.txt_EmpNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_EmpNo.EditControl = null;
		this.txt_EmpNo.FieldName = "empID";
		this.txt_EmpNo.IsSearchControl = false;
		this.txt_EmpNo.Location = new System.Drawing.Point(487, 5);
		this.txt_EmpNo.Name = "txt_EmpNo";
		this.txt_EmpNo.Properties.ReadOnly = true;
		this.txt_EmpNo.SettingInVisible = false;
		this.txt_EmpNo.SettingReadonly = false;
		this.txt_EmpNo.Size = new System.Drawing.Size(167, 20);
		this.txt_EmpNo.TabIndex = 12;
		this.txt_EmpNo.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_ExtEmpNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_ExtEmpNo.EditControl = null;
		this.txt_ExtEmpNo.FieldName = "ExtEmpNo";
		this.txt_ExtEmpNo.IsSearchControl = false;
		this.txt_ExtEmpNo.Location = new System.Drawing.Point(487, 57);
		this.txt_ExtEmpNo.Name = "txt_ExtEmpNo";
		this.txt_ExtEmpNo.SettingInVisible = false;
		this.txt_ExtEmpNo.SettingReadonly = false;
		this.txt_ExtEmpNo.Size = new System.Drawing.Size(167, 20);
		this.txt_ExtEmpNo.TabIndex = 13;
		this.txt_ExtEmpNo.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.chk_Active.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_Active.EditControl = null;
		this.chk_Active.FieldName = "Active";
		this.chk_Active.IsSearchControl = false;
		this.chk_Active.Location = new System.Drawing.Point(362, 81);
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.Properties.Caption = "Active Employee";
		this.chk_Active.Properties.ValueChecked = "Y";
		this.chk_Active.Properties.ValueUnchecked = "N";
		this.chk_Active.SettingInVisible = false;
		this.chk_Active.SettingReadonly = false;
		this.chk_Active.Size = new System.Drawing.Size(126, 20);
		this.chk_Active.TabIndex = 14;
		this.chk_Active.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_OfficePhone.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_OfficePhone.EditControl = null;
		this.txt_OfficePhone.FieldName = "officeTel";
		this.txt_OfficePhone.IsSearchControl = false;
		this.txt_OfficePhone.Location = new System.Drawing.Point(487, 126);
		this.txt_OfficePhone.Name = "txt_OfficePhone";
		this.txt_OfficePhone.SettingInVisible = false;
		this.txt_OfficePhone.SettingReadonly = false;
		this.txt_OfficePhone.Size = new System.Drawing.Size(167, 20);
		this.txt_OfficePhone.TabIndex = 16;
		this.txt_OfficePhone.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_Pager.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Pager.EditControl = null;
		this.txt_Pager.FieldName = "pager";
		this.txt_Pager.IsSearchControl = false;
		this.txt_Pager.Location = new System.Drawing.Point(487, 103);
		this.txt_Pager.Name = "txt_Pager";
		this.txt_Pager.SettingInVisible = false;
		this.txt_Pager.SettingReadonly = false;
		this.txt_Pager.Size = new System.Drawing.Size(167, 20);
		this.txt_Pager.TabIndex = 15;
		this.txt_Pager.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_MobilePhone.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_MobilePhone.EditControl = null;
		this.txt_MobilePhone.FieldName = "mobile";
		this.txt_MobilePhone.IsSearchControl = false;
		this.txt_MobilePhone.Location = new System.Drawing.Point(487, 173);
		this.txt_MobilePhone.Name = "txt_MobilePhone";
		this.txt_MobilePhone.SettingInVisible = false;
		this.txt_MobilePhone.SettingReadonly = false;
		this.txt_MobilePhone.Size = new System.Drawing.Size(167, 20);
		this.txt_MobilePhone.TabIndex = 18;
		this.txt_MobilePhone.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_ExtPhone.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_ExtPhone.EditControl = null;
		this.txt_ExtPhone.FieldName = "officeExt";
		this.txt_ExtPhone.IsSearchControl = false;
		this.txt_ExtPhone.Location = new System.Drawing.Point(487, 149);
		this.txt_ExtPhone.Name = "txt_ExtPhone";
		this.txt_ExtPhone.SettingInVisible = false;
		this.txt_ExtPhone.SettingReadonly = false;
		this.txt_ExtPhone.Size = new System.Drawing.Size(167, 20);
		this.txt_ExtPhone.TabIndex = 17;
		this.txt_ExtPhone.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_Email.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Email.EditControl = null;
		this.txt_Email.FieldName = "email";
		this.txt_Email.IsSearchControl = false;
		this.txt_Email.Location = new System.Drawing.Point(487, 246);
		this.txt_Email.Name = "txt_Email";
		this.txt_Email.SettingInVisible = false;
		this.txt_Email.SettingReadonly = false;
		this.txt_Email.Size = new System.Drawing.Size(167, 20);
		this.txt_Email.TabIndex = 21;
		this.txt_Email.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_Email.Validating += new System.ComponentModel.CancelEventHandler(this.txt_Email_Validating);
		this.txt_Fax.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Fax.EditControl = null;
		this.txt_Fax.FieldName = "fax";
		this.txt_Fax.IsSearchControl = false;
		this.txt_Fax.Location = new System.Drawing.Point(487, 222);
		this.txt_Fax.Name = "txt_Fax";
		this.txt_Fax.SettingInVisible = false;
		this.txt_Fax.SettingReadonly = false;
		this.txt_Fax.Size = new System.Drawing.Size(167, 20);
		this.txt_Fax.TabIndex = 20;
		this.txt_Fax.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_HomePhone.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_HomePhone.EditControl = null;
		this.txt_HomePhone.FieldName = "homeTel";
		this.txt_HomePhone.IsSearchControl = false;
		this.txt_HomePhone.Location = new System.Drawing.Point(487, 198);
		this.txt_HomePhone.Name = "txt_HomePhone";
		this.txt_HomePhone.SettingInVisible = false;
		this.txt_HomePhone.SettingReadonly = false;
		this.txt_HomePhone.Size = new System.Drawing.Size(167, 20);
		this.txt_HomePhone.TabIndex = 19;
		this.txt_HomePhone.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.tab_Detail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tab_Detail.IsSearchControl = false;
		this.tab_Detail.Location = new System.Drawing.Point(0, 304);
		this.tab_Detail.Name = "tab_Detail";
		this.tab_Detail.SelectedTabPage = this.tab_Address;
		this.tab_Detail.Size = new System.Drawing.Size(768, 295);
		this.tab_Detail.TabIndex = 40;
		this.tab_Detail.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[9] { this.tab_Address, this.tab_Membership, this.Tab_Administration, this.tab_Personal, this.tab_Contract, this.tab_Finance, this.tab_Remarks, this.tab_Attachments, this.tab_User });
		this.tab_Address.Controls.Add(this.pnl_Address);
		this.tab_Address.EditControl = null;
		this.tab_Address.EditValue = "Address";
		this.tab_Address.FieldName = "tab_Address";
		this.tab_Address.IsSearchControl = false;
		this.tab_Address.Name = "tab_Address";
		this.tab_Address.ReadOnly = false;
		this.tab_Address.SettingInVisible = false;
		this.tab_Address.SettingReadonly = false;
		this.tab_Address.Size = new System.Drawing.Size(766, 270);
		this.tab_Address.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Address.Text = "Address";
		this.pnl_Address.Controls.Add(this.luk_H_State);
		this.pnl_Address.Controls.Add(this.luk_H_Country);
		this.pnl_Address.Controls.Add(this.luk_W_Country);
		this.pnl_Address.Controls.Add(this.luk_W_State);
		this.pnl_Address.Controls.Add(this.txt_H_Block);
		this.pnl_Address.Controls.Add(this.txt_H_StreetNo);
		this.pnl_Address.Controls.Add(this.txt_H_Street);
		this.pnl_Address.Controls.Add(this.lbl_H_StrNo);
		this.pnl_Address.Controls.Add(this.lbl_H_Block);
		this.pnl_Address.Controls.Add(this.lbl_H_Street);
		this.pnl_Address.Controls.Add(this.txt_W_Street);
		this.pnl_Address.Controls.Add(this.txt_W_Block);
		this.pnl_Address.Controls.Add(this.lbl_WStreet);
		this.pnl_Address.Controls.Add(this.txt_W_StreetNo);
		this.pnl_Address.Controls.Add(this.lbl_StrNo);
		this.pnl_Address.Controls.Add(this.lbl_Block);
		this.pnl_Address.Controls.Add(this.txt_H_County);
		this.pnl_Address.Controls.Add(this.txt_H_City);
		this.pnl_Address.Controls.Add(this.txt_H_Zipcode);
		this.pnl_Address.Controls.Add(this.txt_H_Build);
		this.pnl_Address.Controls.Add(this.lbl_H_zipCode);
		this.pnl_Address.Controls.Add(this.lbl_H_city);
		this.pnl_Address.Controls.Add(this.lbl_H_County);
		this.pnl_Address.Controls.Add(this.lbl_H_State);
		this.pnl_Address.Controls.Add(this.lbl_H_Country);
		this.pnl_Address.Controls.Add(this.lbl_H_Build);
		this.pnl_Address.Controls.Add(this.lbl_WorkAdress);
		this.pnl_Address.Controls.Add(this.lbl_HomeAddress);
		this.pnl_Address.Controls.Add(this.txt_W_County);
		this.pnl_Address.Controls.Add(this.txt_W_Build);
		this.pnl_Address.Controls.Add(this.txt_W_City);
		this.pnl_Address.Controls.Add(this.lbl_Build);
		this.pnl_Address.Controls.Add(this.txt_W_Zipcode);
		this.pnl_Address.Controls.Add(this.lbl_Country);
		this.pnl_Address.Controls.Add(this.lbl_State);
		this.pnl_Address.Controls.Add(this.lbl_ZipCode);
		this.pnl_Address.Controls.Add(this.lbl_County);
		this.pnl_Address.Controls.Add(this.lbl_City);
		this.pnl_Address.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnl_Address.Location = new System.Drawing.Point(0, 0);
		this.pnl_Address.Name = "pnl_Address";
		this.pnl_Address.Size = new System.Drawing.Size(766, 270);
		this.pnl_Address.TabIndex = 6;
		this.luk_H_State.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_H_State.EditControl = null;
		this.luk_H_State.FieldName = "homeState";
		this.luk_H_State.IsSearchControl = false;
		this.luk_H_State.Location = new System.Drawing.Point(486, 193);
		this.luk_H_State.Name = "luk_H_State";
		this.luk_H_State.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_H_State.Properties.IsSearchControl = false;
		this.luk_H_State.Properties.NullText = "";
		this.luk_H_State.Properties.PopupSizeable = false;
		this.luk_H_State.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_H_State.SettingInVisible = false;
		this.luk_H_State.SettingReadonly = false;
		this.luk_H_State.Size = new System.Drawing.Size(167, 20);
		this.luk_H_State.TabIndex = 40;
		this.luk_H_State.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_H_State.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_H_State_EditValueChanging);
		this.luk_H_Country.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_H_Country.EditControl = null;
		this.luk_H_Country.FieldName = "homeCountr";
		this.luk_H_Country.IsSearchControl = false;
		this.luk_H_Country.Location = new System.Drawing.Point(486, 217);
		this.luk_H_Country.Name = "luk_H_Country";
		this.luk_H_Country.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_H_Country.Properties.IsSearchControl = false;
		this.luk_H_Country.Properties.NullText = "";
		this.luk_H_Country.Properties.PopupSizeable = false;
		this.luk_H_Country.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_H_Country.SettingInVisible = false;
		this.luk_H_Country.SettingReadonly = false;
		this.luk_H_Country.Size = new System.Drawing.Size(167, 20);
		this.luk_H_Country.TabIndex = 41;
		this.luk_H_Country.EditValueChanged += new System.EventHandler(this.luk_H_Country_EditValueChanged);
		this.luk_W_Country.EditControl = null;
		this.luk_W_Country.FieldName = "workCountr";
		this.luk_W_Country.IsSearchControl = false;
		this.luk_W_Country.Location = new System.Drawing.Point(128, 217);
		this.luk_W_Country.Name = "luk_W_Country";
		this.luk_W_Country.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_W_Country.Properties.IsSearchControl = false;
		this.luk_W_Country.Properties.NullText = "";
		this.luk_W_Country.Properties.PopupSizeable = false;
		this.luk_W_Country.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_W_Country.SettingInVisible = false;
		this.luk_W_Country.SettingReadonly = false;
		this.luk_W_Country.Size = new System.Drawing.Size(167, 20);
		this.luk_W_Country.TabIndex = 32;
		this.luk_W_Country.EditValueChanged += new System.EventHandler(this.luk_W_Country_EditValueChanged);
		this.luk_W_State.EditControl = null;
		this.luk_W_State.FieldName = "workState";
		this.luk_W_State.IsSearchControl = false;
		this.luk_W_State.Location = new System.Drawing.Point(128, 193);
		this.luk_W_State.Name = "luk_W_State";
		this.luk_W_State.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_W_State.Properties.IsSearchControl = false;
		this.luk_W_State.Properties.NullText = "";
		this.luk_W_State.Properties.PopupSizeable = false;
		this.luk_W_State.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_W_State.SettingInVisible = false;
		this.luk_W_State.SettingReadonly = false;
		this.luk_W_State.Size = new System.Drawing.Size(167, 20);
		this.luk_W_State.TabIndex = 31;
		this.luk_W_State.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_W_State.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_W_State_EditValueChanging);
		this.txt_H_Block.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_H_Block.EditControl = null;
		this.txt_H_Block.FieldName = null;
		this.txt_H_Block.IsSearchControl = false;
		this.txt_H_Block.Location = new System.Drawing.Point(486, 77);
		this.txt_H_Block.Name = "txt_H_Block";
		this.txt_H_Block.SettingInVisible = false;
		this.txt_H_Block.SettingReadonly = false;
		this.txt_H_Block.Size = new System.Drawing.Size(167, 20);
		this.txt_H_Block.TabIndex = 35;
		this.txt_H_Block.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_H_StreetNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_H_StreetNo.EditControl = null;
		this.txt_H_StreetNo.FieldName = null;
		this.txt_H_StreetNo.IsSearchControl = false;
		this.txt_H_StreetNo.Location = new System.Drawing.Point(486, 54);
		this.txt_H_StreetNo.Name = "txt_H_StreetNo";
		this.txt_H_StreetNo.SettingInVisible = false;
		this.txt_H_StreetNo.SettingReadonly = false;
		this.txt_H_StreetNo.Size = new System.Drawing.Size(167, 20);
		this.txt_H_StreetNo.TabIndex = 34;
		this.txt_H_StreetNo.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_H_Street.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_H_Street.EditControl = null;
		this.txt_H_Street.FieldName = null;
		this.txt_H_Street.IsSearchControl = false;
		this.txt_H_Street.Location = new System.Drawing.Point(486, 31);
		this.txt_H_Street.Name = "txt_H_Street";
		this.txt_H_Street.SettingInVisible = false;
		this.txt_H_Street.SettingReadonly = false;
		this.txt_H_Street.Size = new System.Drawing.Size(167, 20);
		this.txt_H_Street.TabIndex = 33;
		this.txt_H_Street.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.lbl_H_StrNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_StrNo.EditControl = null;
		this.lbl_H_StrNo.EditValue = "Street No.";
		this.lbl_H_StrNo.FieldName = null;
		this.lbl_H_StrNo.HideWhenEmpty = false;
		this.lbl_H_StrNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_StrNo.Location = new System.Drawing.Point(361, 56);
		this.lbl_H_StrNo.Name = "lbl_H_StrNo";
		this.lbl_H_StrNo.ReadOnly = false;
		this.lbl_H_StrNo.SettingInVisible = false;
		this.lbl_H_StrNo.SettingReadonly = false;
		this.lbl_H_StrNo.ShowUnderLine = true;
		this.lbl_H_StrNo.Size = new System.Drawing.Size(126, 20);
		this.lbl_H_StrNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_StrNo.TabIndex = 74;
		this.lbl_H_StrNo.Utext = "Street No.";
		this.lbl_H_Block.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_Block.EditControl = null;
		this.lbl_H_Block.EditValue = "Block";
		this.lbl_H_Block.FieldName = null;
		this.lbl_H_Block.HideWhenEmpty = false;
		this.lbl_H_Block.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_Block.Location = new System.Drawing.Point(361, 80);
		this.lbl_H_Block.Name = "lbl_H_Block";
		this.lbl_H_Block.ReadOnly = false;
		this.lbl_H_Block.SettingInVisible = false;
		this.lbl_H_Block.SettingReadonly = false;
		this.lbl_H_Block.ShowUnderLine = true;
		this.lbl_H_Block.Size = new System.Drawing.Size(126, 20);
		this.lbl_H_Block.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_Block.TabIndex = 73;
		this.lbl_H_Block.Utext = "Block";
		this.lbl_H_Street.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_Street.EditControl = null;
		this.lbl_H_Street.EditValue = "Street";
		this.lbl_H_Street.FieldName = null;
		this.lbl_H_Street.HideWhenEmpty = false;
		this.lbl_H_Street.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_Street.Location = new System.Drawing.Point(361, 34);
		this.lbl_H_Street.Name = "lbl_H_Street";
		this.lbl_H_Street.ReadOnly = false;
		this.lbl_H_Street.SettingInVisible = false;
		this.lbl_H_Street.SettingReadonly = false;
		this.lbl_H_Street.ShowUnderLine = true;
		this.lbl_H_Street.Size = new System.Drawing.Size(126, 20);
		this.lbl_H_Street.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_Street.TabIndex = 72;
		this.lbl_H_Street.Utext = "Street";
		this.txt_W_Street.EditControl = null;
		this.txt_W_Street.FieldName = "workStreet";
		this.txt_W_Street.IsSearchControl = false;
		this.txt_W_Street.Location = new System.Drawing.Point(128, 31);
		this.txt_W_Street.Name = "txt_W_Street";
		this.txt_W_Street.SettingInVisible = false;
		this.txt_W_Street.SettingReadonly = false;
		this.txt_W_Street.Size = new System.Drawing.Size(167, 20);
		this.txt_W_Street.TabIndex = 24;
		this.txt_W_Street.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_W_Block.EditControl = null;
		this.txt_W_Block.FieldName = "workBlock";
		this.txt_W_Block.IsSearchControl = false;
		this.txt_W_Block.Location = new System.Drawing.Point(128, 77);
		this.txt_W_Block.Name = "txt_W_Block";
		this.txt_W_Block.SettingInVisible = false;
		this.txt_W_Block.SettingReadonly = false;
		this.txt_W_Block.Size = new System.Drawing.Size(167, 20);
		this.txt_W_Block.TabIndex = 26;
		this.txt_W_Block.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.lbl_WStreet.EditControl = null;
		this.lbl_WStreet.EditValue = "Street";
		this.lbl_WStreet.FieldName = null;
		this.lbl_WStreet.HideWhenEmpty = false;
		this.lbl_WStreet.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_WStreet.Location = new System.Drawing.Point(12, 34);
		this.lbl_WStreet.Name = "lbl_WStreet";
		this.lbl_WStreet.ReadOnly = false;
		this.lbl_WStreet.SettingInVisible = false;
		this.lbl_WStreet.SettingReadonly = false;
		this.lbl_WStreet.ShowUnderLine = true;
		this.lbl_WStreet.Size = new System.Drawing.Size(117, 20);
		this.lbl_WStreet.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_WStreet.TabIndex = 66;
		this.lbl_WStreet.Utext = "Street";
		this.txt_W_StreetNo.EditControl = null;
		this.txt_W_StreetNo.FieldName = "StreetNoW";
		this.txt_W_StreetNo.IsSearchControl = false;
		this.txt_W_StreetNo.Location = new System.Drawing.Point(128, 54);
		this.txt_W_StreetNo.Name = "txt_W_StreetNo";
		this.txt_W_StreetNo.SettingInVisible = false;
		this.txt_W_StreetNo.SettingReadonly = false;
		this.txt_W_StreetNo.Size = new System.Drawing.Size(167, 20);
		this.txt_W_StreetNo.TabIndex = 25;
		this.txt_W_StreetNo.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.lbl_StrNo.EditControl = null;
		this.lbl_StrNo.EditValue = "Street No.";
		this.lbl_StrNo.FieldName = null;
		this.lbl_StrNo.HideWhenEmpty = false;
		this.lbl_StrNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_StrNo.Location = new System.Drawing.Point(12, 57);
		this.lbl_StrNo.Name = "lbl_StrNo";
		this.lbl_StrNo.ReadOnly = false;
		this.lbl_StrNo.SettingInVisible = false;
		this.lbl_StrNo.SettingReadonly = false;
		this.lbl_StrNo.ShowUnderLine = true;
		this.lbl_StrNo.Size = new System.Drawing.Size(117, 20);
		this.lbl_StrNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_StrNo.TabIndex = 68;
		this.lbl_StrNo.Utext = "Street No.";
		this.lbl_Block.EditControl = null;
		this.lbl_Block.EditValue = "Block";
		this.lbl_Block.FieldName = null;
		this.lbl_Block.HideWhenEmpty = false;
		this.lbl_Block.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Block.Location = new System.Drawing.Point(12, 80);
		this.lbl_Block.Name = "lbl_Block";
		this.lbl_Block.ReadOnly = false;
		this.lbl_Block.SettingInVisible = false;
		this.lbl_Block.SettingReadonly = false;
		this.lbl_Block.ShowUnderLine = true;
		this.lbl_Block.Size = new System.Drawing.Size(117, 20);
		this.lbl_Block.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Block.TabIndex = 67;
		this.lbl_Block.Utext = "Block";
		this.txt_H_County.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_H_County.EditControl = null;
		this.txt_H_County.FieldName = null;
		this.txt_H_County.IsSearchControl = false;
		this.txt_H_County.Location = new System.Drawing.Point(486, 170);
		this.txt_H_County.Name = "txt_H_County";
		this.txt_H_County.SettingInVisible = false;
		this.txt_H_County.SettingReadonly = false;
		this.txt_H_County.Size = new System.Drawing.Size(167, 20);
		this.txt_H_County.TabIndex = 39;
		this.txt_H_County.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_H_City.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_H_City.EditControl = null;
		this.txt_H_City.FieldName = null;
		this.txt_H_City.IsSearchControl = false;
		this.txt_H_City.Location = new System.Drawing.Point(486, 147);
		this.txt_H_City.Name = "txt_H_City";
		this.txt_H_City.SettingInVisible = false;
		this.txt_H_City.SettingReadonly = false;
		this.txt_H_City.Size = new System.Drawing.Size(167, 20);
		this.txt_H_City.TabIndex = 38;
		this.txt_H_City.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_H_Zipcode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_H_Zipcode.EditControl = null;
		this.txt_H_Zipcode.FieldName = null;
		this.txt_H_Zipcode.IsSearchControl = false;
		this.txt_H_Zipcode.Location = new System.Drawing.Point(486, 124);
		this.txt_H_Zipcode.Name = "txt_H_Zipcode";
		this.txt_H_Zipcode.SettingInVisible = false;
		this.txt_H_Zipcode.SettingReadonly = false;
		this.txt_H_Zipcode.Size = new System.Drawing.Size(167, 20);
		this.txt_H_Zipcode.TabIndex = 37;
		this.txt_H_Zipcode.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_H_Build.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_H_Build.EditControl = null;
		this.txt_H_Build.FieldName = null;
		this.txt_H_Build.IsSearchControl = false;
		this.txt_H_Build.Location = new System.Drawing.Point(486, 101);
		this.txt_H_Build.Name = "txt_H_Build";
		this.txt_H_Build.SettingInVisible = false;
		this.txt_H_Build.SettingReadonly = false;
		this.txt_H_Build.Size = new System.Drawing.Size(167, 20);
		this.txt_H_Build.TabIndex = 36;
		this.txt_H_Build.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.lbl_H_zipCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_zipCode.EditControl = null;
		this.lbl_H_zipCode.EditValue = "Zip Code";
		this.lbl_H_zipCode.FieldName = null;
		this.lbl_H_zipCode.HideWhenEmpty = false;
		this.lbl_H_zipCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_zipCode.Location = new System.Drawing.Point(361, 127);
		this.lbl_H_zipCode.Name = "lbl_H_zipCode";
		this.lbl_H_zipCode.ReadOnly = false;
		this.lbl_H_zipCode.SettingInVisible = false;
		this.lbl_H_zipCode.SettingReadonly = false;
		this.lbl_H_zipCode.ShowUnderLine = true;
		this.lbl_H_zipCode.Size = new System.Drawing.Size(126, 20);
		this.lbl_H_zipCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_zipCode.TabIndex = 59;
		this.lbl_H_zipCode.Utext = "Zip Code";
		this.lbl_H_city.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_city.EditControl = null;
		this.lbl_H_city.EditValue = "City";
		this.lbl_H_city.FieldName = null;
		this.lbl_H_city.HideWhenEmpty = false;
		this.lbl_H_city.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_city.Location = new System.Drawing.Point(361, 150);
		this.lbl_H_city.Name = "lbl_H_city";
		this.lbl_H_city.ReadOnly = false;
		this.lbl_H_city.SettingInVisible = false;
		this.lbl_H_city.SettingReadonly = false;
		this.lbl_H_city.ShowUnderLine = true;
		this.lbl_H_city.Size = new System.Drawing.Size(126, 20);
		this.lbl_H_city.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_city.TabIndex = 58;
		this.lbl_H_city.Utext = "City";
		this.lbl_H_County.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_County.EditControl = null;
		this.lbl_H_County.EditValue = "County";
		this.lbl_H_County.FieldName = null;
		this.lbl_H_County.HideWhenEmpty = false;
		this.lbl_H_County.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_County.Location = new System.Drawing.Point(361, 173);
		this.lbl_H_County.Name = "lbl_H_County";
		this.lbl_H_County.ReadOnly = false;
		this.lbl_H_County.SettingInVisible = false;
		this.lbl_H_County.SettingReadonly = false;
		this.lbl_H_County.ShowUnderLine = true;
		this.lbl_H_County.Size = new System.Drawing.Size(126, 20);
		this.lbl_H_County.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_County.TabIndex = 57;
		this.lbl_H_County.Utext = "County";
		this.lbl_H_State.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_State.EditControl = null;
		this.lbl_H_State.EditValue = "State";
		this.lbl_H_State.FieldName = null;
		this.lbl_H_State.HideWhenEmpty = false;
		this.lbl_H_State.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_State.Location = new System.Drawing.Point(361, 196);
		this.lbl_H_State.Name = "lbl_H_State";
		this.lbl_H_State.ReadOnly = false;
		this.lbl_H_State.SettingInVisible = false;
		this.lbl_H_State.SettingReadonly = false;
		this.lbl_H_State.ShowUnderLine = true;
		this.lbl_H_State.Size = new System.Drawing.Size(126, 20);
		this.lbl_H_State.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_State.TabIndex = 56;
		this.lbl_H_State.Utext = "State";
		this.lbl_H_Country.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_Country.EditControl = null;
		this.lbl_H_Country.EditValue = "Country";
		this.lbl_H_Country.FieldName = null;
		this.lbl_H_Country.HideWhenEmpty = false;
		this.lbl_H_Country.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_Country.Location = new System.Drawing.Point(361, 220);
		this.lbl_H_Country.Name = "lbl_H_Country";
		this.lbl_H_Country.ReadOnly = false;
		this.lbl_H_Country.SettingInVisible = false;
		this.lbl_H_Country.SettingReadonly = false;
		this.lbl_H_Country.ShowUnderLine = true;
		this.lbl_H_Country.Size = new System.Drawing.Size(126, 20);
		this.lbl_H_Country.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_Country.TabIndex = 55;
		this.lbl_H_Country.Utext = "Country";
		this.lbl_H_Build.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_Build.EditControl = null;
		this.lbl_H_Build.EditValue = "Building/Floor/Room";
		this.lbl_H_Build.FieldName = null;
		this.lbl_H_Build.HideWhenEmpty = false;
		this.lbl_H_Build.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_Build.Location = new System.Drawing.Point(361, 104);
		this.lbl_H_Build.Name = "lbl_H_Build";
		this.lbl_H_Build.ReadOnly = false;
		this.lbl_H_Build.SettingInVisible = false;
		this.lbl_H_Build.SettingReadonly = false;
		this.lbl_H_Build.ShowUnderLine = true;
		this.lbl_H_Build.Size = new System.Drawing.Size(126, 20);
		this.lbl_H_Build.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_Build.TabIndex = 54;
		this.lbl_H_Build.Utext = "Building/Floor/Room";
		this.lbl_WorkAdress.EditControl = null;
		this.lbl_WorkAdress.EditValue = "Work Address";
		this.lbl_WorkAdress.FieldName = null;
		this.lbl_WorkAdress.HideWhenEmpty = false;
		this.lbl_WorkAdress.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_WorkAdress.Location = new System.Drawing.Point(12, 3);
		this.lbl_WorkAdress.Name = "lbl_WorkAdress";
		this.lbl_WorkAdress.ReadOnly = false;
		this.lbl_WorkAdress.SettingInVisible = false;
		this.lbl_WorkAdress.SettingReadonly = false;
		this.lbl_WorkAdress.ShowUnderLine = true;
		this.lbl_WorkAdress.Size = new System.Drawing.Size(72, 20);
		this.lbl_WorkAdress.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_WorkAdress.TabIndex = 2;
		this.lbl_WorkAdress.Utext = "Work Address";
		this.lbl_HomeAddress.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_HomeAddress.EditControl = null;
		this.lbl_HomeAddress.EditValue = "Home Address";
		this.lbl_HomeAddress.FieldName = null;
		this.lbl_HomeAddress.HideWhenEmpty = false;
		this.lbl_HomeAddress.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_HomeAddress.Location = new System.Drawing.Point(361, 5);
		this.lbl_HomeAddress.Name = "lbl_HomeAddress";
		this.lbl_HomeAddress.ReadOnly = false;
		this.lbl_HomeAddress.SettingInVisible = false;
		this.lbl_HomeAddress.SettingReadonly = false;
		this.lbl_HomeAddress.ShowUnderLine = true;
		this.lbl_HomeAddress.Size = new System.Drawing.Size(74, 20);
		this.lbl_HomeAddress.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_HomeAddress.TabIndex = 1;
		this.lbl_HomeAddress.Utext = "Home Address";
		this.txt_W_County.EditControl = null;
		this.txt_W_County.FieldName = "workCounty";
		this.txt_W_County.IsSearchControl = false;
		this.txt_W_County.Location = new System.Drawing.Point(128, 170);
		this.txt_W_County.Name = "txt_W_County";
		this.txt_W_County.SettingInVisible = false;
		this.txt_W_County.SettingReadonly = false;
		this.txt_W_County.Size = new System.Drawing.Size(167, 20);
		this.txt_W_County.TabIndex = 30;
		this.txt_W_County.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_W_Build.EditControl = null;
		this.txt_W_Build.FieldName = "WorkBuild";
		this.txt_W_Build.IsSearchControl = false;
		this.txt_W_Build.Location = new System.Drawing.Point(128, 101);
		this.txt_W_Build.Name = "txt_W_Build";
		this.txt_W_Build.SettingInVisible = false;
		this.txt_W_Build.SettingReadonly = false;
		this.txt_W_Build.Size = new System.Drawing.Size(167, 20);
		this.txt_W_Build.TabIndex = 27;
		this.txt_W_Build.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_W_City.EditControl = null;
		this.txt_W_City.FieldName = "workCity";
		this.txt_W_City.IsSearchControl = false;
		this.txt_W_City.Location = new System.Drawing.Point(128, 147);
		this.txt_W_City.Name = "txt_W_City";
		this.txt_W_City.SettingInVisible = false;
		this.txt_W_City.SettingReadonly = false;
		this.txt_W_City.Size = new System.Drawing.Size(167, 20);
		this.txt_W_City.TabIndex = 29;
		this.txt_W_City.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.lbl_Build.EditControl = null;
		this.lbl_Build.EditValue = "Building/Floor/Room";
		this.lbl_Build.FieldName = null;
		this.lbl_Build.HideWhenEmpty = false;
		this.lbl_Build.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Build.Location = new System.Drawing.Point(12, 104);
		this.lbl_Build.Name = "lbl_Build";
		this.lbl_Build.ReadOnly = false;
		this.lbl_Build.SettingInVisible = false;
		this.lbl_Build.SettingReadonly = false;
		this.lbl_Build.ShowUnderLine = true;
		this.lbl_Build.Size = new System.Drawing.Size(117, 20);
		this.lbl_Build.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Build.TabIndex = 42;
		this.lbl_Build.Utext = "Building/Floor/Room";
		this.txt_W_Zipcode.EditControl = null;
		this.txt_W_Zipcode.FieldName = "workZip";
		this.txt_W_Zipcode.IsSearchControl = false;
		this.txt_W_Zipcode.Location = new System.Drawing.Point(128, 124);
		this.txt_W_Zipcode.Name = "txt_W_Zipcode";
		this.txt_W_Zipcode.SettingInVisible = false;
		this.txt_W_Zipcode.SettingReadonly = false;
		this.txt_W_Zipcode.Size = new System.Drawing.Size(167, 20);
		this.txt_W_Zipcode.TabIndex = 28;
		this.txt_W_Zipcode.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.lbl_Country.EditControl = null;
		this.lbl_Country.EditValue = "Country";
		this.lbl_Country.FieldName = null;
		this.lbl_Country.HideWhenEmpty = false;
		this.lbl_Country.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Country.Location = new System.Drawing.Point(12, 220);
		this.lbl_Country.Name = "lbl_Country";
		this.lbl_Country.ReadOnly = false;
		this.lbl_Country.SettingInVisible = false;
		this.lbl_Country.SettingReadonly = false;
		this.lbl_Country.ShowUnderLine = true;
		this.lbl_Country.Size = new System.Drawing.Size(117, 20);
		this.lbl_Country.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Country.TabIndex = 43;
		this.lbl_Country.Utext = "Country";
		this.lbl_State.EditControl = null;
		this.lbl_State.EditValue = "State";
		this.lbl_State.FieldName = null;
		this.lbl_State.HideWhenEmpty = false;
		this.lbl_State.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_State.Location = new System.Drawing.Point(12, 195);
		this.lbl_State.Name = "lbl_State";
		this.lbl_State.ReadOnly = false;
		this.lbl_State.SettingInVisible = false;
		this.lbl_State.SettingReadonly = false;
		this.lbl_State.ShowUnderLine = true;
		this.lbl_State.Size = new System.Drawing.Size(117, 20);
		this.lbl_State.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_State.TabIndex = 44;
		this.lbl_State.Utext = "State";
		this.lbl_ZipCode.EditControl = null;
		this.lbl_ZipCode.EditValue = "Zip Code";
		this.lbl_ZipCode.FieldName = null;
		this.lbl_ZipCode.HideWhenEmpty = false;
		this.lbl_ZipCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ZipCode.Location = new System.Drawing.Point(12, 127);
		this.lbl_ZipCode.Name = "lbl_ZipCode";
		this.lbl_ZipCode.ReadOnly = false;
		this.lbl_ZipCode.SettingInVisible = false;
		this.lbl_ZipCode.SettingReadonly = false;
		this.lbl_ZipCode.ShowUnderLine = true;
		this.lbl_ZipCode.Size = new System.Drawing.Size(117, 20);
		this.lbl_ZipCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ZipCode.TabIndex = 47;
		this.lbl_ZipCode.Utext = "Zip Code";
		this.lbl_County.EditControl = null;
		this.lbl_County.EditValue = "County";
		this.lbl_County.FieldName = null;
		this.lbl_County.HideWhenEmpty = false;
		this.lbl_County.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_County.Location = new System.Drawing.Point(12, 173);
		this.lbl_County.Name = "lbl_County";
		this.lbl_County.ReadOnly = false;
		this.lbl_County.SettingInVisible = false;
		this.lbl_County.SettingReadonly = false;
		this.lbl_County.ShowUnderLine = true;
		this.lbl_County.Size = new System.Drawing.Size(117, 20);
		this.lbl_County.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_County.TabIndex = 45;
		this.lbl_County.Utext = "County";
		this.lbl_City.EditControl = null;
		this.lbl_City.EditValue = "City";
		this.lbl_City.FieldName = null;
		this.lbl_City.HideWhenEmpty = false;
		this.lbl_City.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_City.Location = new System.Drawing.Point(12, 150);
		this.lbl_City.Name = "lbl_City";
		this.lbl_City.ReadOnly = false;
		this.lbl_City.SettingInVisible = false;
		this.lbl_City.SettingReadonly = false;
		this.lbl_City.ShowUnderLine = true;
		this.lbl_City.Size = new System.Drawing.Size(117, 20);
		this.lbl_City.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_City.TabIndex = 46;
		this.lbl_City.Utext = "City";
		this.tab_Membership.Controls.Add(this.panelControl2);
		this.tab_Membership.EditControl = null;
		this.tab_Membership.EditValue = "Membership";
		this.tab_Membership.FieldName = "tab_Membership";
		this.tab_Membership.IsSearchControl = false;
		this.tab_Membership.Name = "tab_Membership";
		this.tab_Membership.ReadOnly = false;
		this.tab_Membership.SettingInVisible = false;
		this.tab_Membership.SettingReadonly = false;
		this.tab_Membership.Size = new System.Drawing.Size(766, 270);
		this.tab_Membership.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Membership.Text = "Membership";
		this.panelControl2.Controls.Add(this.splitContainerControl1);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(766, 270);
		this.panelControl2.TabIndex = 0;
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.btn_SetRole);
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Roles);
		this.splitContainerControl1.Panel1.Controls.Add(this.lbl_Roles);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Teams);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(762, 266);
		this.splitContainerControl1.SplitterPosition = 300;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.btn_SetRole.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_SetRole.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_SetRole.Appearance.Options.UseBackColor = true;
		this.btn_SetRole.EditControl = null;
		this.btn_SetRole.EditValue = "Set Role as Default";
		this.btn_SetRole.FieldName = null;
		this.btn_SetRole.IsSearchControl = false;
		this.btn_SetRole.Location = new System.Drawing.Point(4, 240);
		this.btn_SetRole.Name = "btn_SetRole";
		this.btn_SetRole.ReadOnly = false;
		this.btn_SetRole.SettingInVisible = false;
		this.btn_SetRole.SettingReadonly = false;
		this.btn_SetRole.Size = new System.Drawing.Size(123, 20);
		this.btn_SetRole.TabIndex = 60;
		this.btn_SetRole.Text = "Set Role as Default";
		this.btn_SetRole.Click += new System.EventHandler(this.btn_SetRole_Click);
		this.grd_Roles.AllowPasteExcel = false;
		this.grd_Roles.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Roles.ClipboardData = null;
		this.grd_Roles.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Roles.Location = new System.Drawing.Point(4, 26);
		this.grd_Roles.MainView = this.gv_Roles;
		this.grd_Roles.Name = "grd_Roles";
		this.grd_Roles.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rlup_Role });
		this.grd_Roles.Size = new System.Drawing.Size(295, 209);
		this.grd_Roles.TabIndex = 61;
		this.grd_Roles.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Roles });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(131, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Roles.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Roles.ColumnKey = null;
		this.gv_Roles.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.cl_Role, this.cl_LineNum });
		this.gv_Roles.DisableFormSetting = false;
		this.gv_Roles.DisplayException = false;
		this.gv_Roles.FieldCurrency = null;
		this.gv_Roles.FieldRate = null;
		this.gv_Roles.FormId = "oEmployeesInfo";
		this.gv_Roles.GridControl = this.grd_Roles;
		this.gv_Roles.IndicatorWidth = 30;
		this.gv_Roles.IsApzTable = false;
		this.gv_Roles.IsInsertNewRow = false;
		this.gv_Roles.IsLoadingData = false;
		this.gv_Roles.ItemId = "ORULE";
		this.gv_Roles.LstColumnAllowBatch = null;
		this.gv_Roles.LstColumnNotAllowBatch = null;
		this.gv_Roles.LstColumnNotEditable = null;
		this.gv_Roles.LstColumNotSetting = null;
		this.gv_Roles.Name = "gv_Roles";
		this.gv_Roles.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
		this.gv_Roles.OptionsView.ShowGroupPanel = false;
		this.gv_Roles.UseBatchMod = false;
		this.gv_Roles.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Roles_RowCellStyle);
		this.gv_Roles.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Roles_CellValueChanged);
		this.cl_Role.Caption = "Role";
		this.cl_Role.ColumnEdit = this.rlup_Role;
		this.cl_Role.CurrencyDateField = null;
		this.cl_Role.CurrencyField = null;
		this.cl_Role.CurrencyFixed = null;
		this.cl_Role.CurrencyFixedRate = 0.0;
		this.cl_Role.CurrencyRateField = null;
		this.cl_Role.FieldName = "roleID";
		this.cl_Role.Name = "cl_Role";
		this.cl_Role.PrimaryKey = false;
		this.cl_Role.UnitField = null;
		this.cl_Role.Visible = true;
		this.cl_Role.VisibleIndex = 0;
		this.rlup_Role.AcceptEditorTextAsNewValue = DevExpress.Utils.DefaultBoolean.True;
		this.rlup_Role.AutoHeight = false;
		this.rlup_Role.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rlup_Role.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("typeID", "TypeID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("name", "Name")
		});
		this.rlup_Role.DisplayMember = "name";
		this.rlup_Role.Name = "rlup_Role";
		this.rlup_Role.NullText = "";
		this.rlup_Role.ShowHeader = false;
		this.rlup_Role.ValueMember = "typeID";
		this.rlup_Role.PopupFilter += new DevExpress.XtraEditors.Controls.PopupFilterEventHandler(this.rlup_Role_PopupFilter);
		this.rlup_Role.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rlup_Role_EditValueChanging);
		this.cl_LineNum.Caption = "LineNum";
		this.cl_LineNum.CurrencyDateField = null;
		this.cl_LineNum.CurrencyField = null;
		this.cl_LineNum.CurrencyFixed = null;
		this.cl_LineNum.CurrencyFixedRate = 0.0;
		this.cl_LineNum.CurrencyRateField = null;
		this.cl_LineNum.FieldName = "line";
		this.cl_LineNum.Name = "cl_LineNum";
		this.cl_LineNum.PrimaryKey = false;
		this.cl_LineNum.UnitField = null;
		this.lbl_Roles.EditControl = null;
		this.lbl_Roles.EditValue = "Roles";
		this.lbl_Roles.FieldName = null;
		this.lbl_Roles.HideWhenEmpty = false;
		this.lbl_Roles.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Roles.Location = new System.Drawing.Point(4, 3);
		this.lbl_Roles.Name = "lbl_Roles";
		this.lbl_Roles.ReadOnly = false;
		this.lbl_Roles.SettingInVisible = false;
		this.lbl_Roles.SettingReadonly = false;
		this.lbl_Roles.ShowUnderLine = true;
		this.lbl_Roles.Size = new System.Drawing.Size(29, 20);
		this.lbl_Roles.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Roles.TabIndex = 60;
		this.lbl_Roles.Utext = "Roles";
		this.grd_Teams.AllowPasteExcel = false;
		this.grd_Teams.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Teams.ClipboardData = null;
		this.grd_Teams.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Teams.Location = new System.Drawing.Point(3, 26);
		this.grd_Teams.MainView = this.gv_Teams;
		this.grd_Teams.Name = "grd_Teams";
		this.grd_Teams.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rlup_Team, this.rlup_TeamRole });
		this.grd_Teams.Size = new System.Drawing.Size(454, 237);
		this.grd_Teams.TabIndex = 62;
		this.grd_Teams.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Teams });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem1 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
		this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem1.Text = "Delete Row";
		this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteRowToolStripMenuItem1_Click);
		this.gv_Teams.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Teams.ColumnKey = null;
		this.gv_Teams.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.cl_Team, this.cl_TeamRole, this.cl_lineNumber });
		this.gv_Teams.DisableFormSetting = false;
		this.gv_Teams.DisplayException = false;
		this.gv_Teams.FieldCurrency = null;
		this.gv_Teams.FieldRate = null;
		this.gv_Teams.FormId = "oEmployeesInfo";
		this.gv_Teams.GridControl = this.grd_Teams;
		this.gv_Teams.IndicatorWidth = 30;
		this.gv_Teams.IsApzTable = false;
		this.gv_Teams.IsInsertNewRow = false;
		this.gv_Teams.IsLoadingData = false;
		this.gv_Teams.ItemId = "OTEAM";
		this.gv_Teams.LstColumnAllowBatch = null;
		this.gv_Teams.LstColumnNotAllowBatch = null;
		this.gv_Teams.LstColumnNotEditable = null;
		this.gv_Teams.LstColumNotSetting = null;
		this.gv_Teams.Name = "gv_Teams";
		this.gv_Teams.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Teams.OptionsView.NewItemRowPosition = DevExpress.XtraGrid.Views.Grid.NewItemRowPosition.Bottom;
		this.gv_Teams.OptionsView.ShowGroupPanel = false;
		this.gv_Teams.UseBatchMod = false;
		this.gv_Teams.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Teams_CellValueChanged);
		this.cl_Team.Caption = "Team";
		this.cl_Team.ColumnEdit = this.rlup_Team;
		this.cl_Team.CurrencyDateField = null;
		this.cl_Team.CurrencyField = null;
		this.cl_Team.CurrencyFixed = null;
		this.cl_Team.CurrencyFixedRate = 0.0;
		this.cl_Team.CurrencyRateField = null;
		this.cl_Team.FieldName = "teamID";
		this.cl_Team.Name = "cl_Team";
		this.cl_Team.PrimaryKey = false;
		this.cl_Team.UnitField = null;
		this.cl_Team.Visible = true;
		this.cl_Team.VisibleIndex = 0;
		this.rlup_Team.AutoHeight = false;
		this.rlup_Team.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rlup_Team.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TeamID", "TeamID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("name", "name")
		});
		this.rlup_Team.DisplayMember = "name";
		this.rlup_Team.Name = "rlup_Team";
		this.rlup_Team.NullText = "";
		this.rlup_Team.ShowHeader = false;
		this.rlup_Team.ValueMember = "teamID";
		this.rlup_Team.PopupFilter += new DevExpress.XtraEditors.Controls.PopupFilterEventHandler(this.rlup_Team_PopupFilter);
		this.rlup_Team.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rlup_Team_EditValueChanging);
		this.cl_TeamRole.Caption = "Team Role";
		this.cl_TeamRole.ColumnEdit = this.rlup_TeamRole;
		this.cl_TeamRole.CurrencyDateField = null;
		this.cl_TeamRole.CurrencyField = null;
		this.cl_TeamRole.CurrencyFixed = null;
		this.cl_TeamRole.CurrencyFixedRate = 0.0;
		this.cl_TeamRole.CurrencyRateField = null;
		this.cl_TeamRole.FieldName = "role";
		this.cl_TeamRole.Name = "cl_TeamRole";
		this.cl_TeamRole.PrimaryKey = false;
		this.cl_TeamRole.UnitField = null;
		this.cl_TeamRole.Visible = true;
		this.cl_TeamRole.VisibleIndex = 1;
		this.rlup_TeamRole.AutoHeight = false;
		this.rlup_TeamRole.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rlup_TeamRole.Name = "rlup_TeamRole";
		this.rlup_TeamRole.NullText = "";
		this.rlup_TeamRole.ShowHeader = false;
		this.rlup_TeamRole.EditValueChanged += new System.EventHandler(this.rlup_TeamRole_EditValueChanged);
		this.cl_lineNumber.Caption = "Line num";
		this.cl_lineNumber.CurrencyDateField = null;
		this.cl_lineNumber.CurrencyField = null;
		this.cl_lineNumber.CurrencyFixed = null;
		this.cl_lineNumber.CurrencyFixedRate = 0.0;
		this.cl_lineNumber.CurrencyRateField = null;
		this.cl_lineNumber.FieldName = "line";
		this.cl_lineNumber.Name = "cl_lineNumber";
		this.cl_lineNumber.OptionsColumn.AllowEdit = false;
		this.cl_lineNumber.OptionsColumn.ReadOnly = true;
		this.cl_lineNumber.PrimaryKey = false;
		this.cl_lineNumber.UnitField = null;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Teams";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(3, 3);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(35, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 61;
		this.labelLine2.Utext = "Teams";
		this.Tab_Administration.Appearance.PageClient.BackColor = System.Drawing.Color.DarkGray;
		this.Tab_Administration.Appearance.PageClient.Options.UseBackColor = true;
		this.Tab_Administration.Controls.Add(this.panelControl1);
		this.Tab_Administration.EditControl = null;
		this.Tab_Administration.EditValue = "Administration";
		this.Tab_Administration.FieldName = "Tab_Administration";
		this.Tab_Administration.IsSearchControl = false;
		this.Tab_Administration.Name = "Tab_Administration";
		this.Tab_Administration.ReadOnly = false;
		this.Tab_Administration.SettingInVisible = false;
		this.Tab_Administration.SettingReadonly = false;
		this.Tab_Administration.Size = new System.Drawing.Size(766, 270);
		this.Tab_Administration.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.Tab_Administration.Text = "Administration";
		this.panelControl1.Controls.Add(this.date_TermDate);
		this.panelControl1.Controls.Add(this.date_StartDate);
		this.panelControl1.Controls.Add(this.btn_Health);
		this.panelControl1.Controls.Add(this.btn_Relatives);
		this.panelControl1.Controls.Add(this.luk_Status);
		this.panelControl1.Controls.Add(this.luk_TermReason);
		this.panelControl1.Controls.Add(this.btn_PrevEmployment);
		this.panelControl1.Controls.Add(this.btn_Review);
		this.panelControl1.Controls.Add(this.btn_Education);
		this.panelControl1.Controls.Add(this.btn_Absence);
		this.panelControl1.Controls.Add(this.labelLine4);
		this.panelControl1.Controls.Add(this.labelLine5);
		this.panelControl1.Controls.Add(this.labelLine1);
		this.panelControl1.Controls.Add(this.labelLine3);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(766, 270);
		this.panelControl1.TabIndex = 0;
		this.date_TermDate.EditControl = null;
		this.date_TermDate.EditValue = null;
		this.date_TermDate.FieldName = "termDate";
		this.date_TermDate.Location = new System.Drawing.Point(127, 74);
		this.date_TermDate.Name = "date_TermDate";
		this.date_TermDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_TermDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_TermDate.Properties.IsSearchControl = false;
		this.date_TermDate.SettingInVisible = false;
		this.date_TermDate.SettingReadonly = false;
		this.date_TermDate.Size = new System.Drawing.Size(167, 20);
		this.date_TermDate.TabIndex = 44;
		this.date_TermDate.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.date_StartDate.EditControl = null;
		this.date_StartDate.EditValue = null;
		this.date_StartDate.FieldName = "startDate";
		this.date_StartDate.Location = new System.Drawing.Point(127, 24);
		this.date_StartDate.Name = "date_StartDate";
		this.date_StartDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_StartDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_StartDate.Properties.IsSearchControl = false;
		this.date_StartDate.SettingInVisible = false;
		this.date_StartDate.SettingReadonly = false;
		this.date_StartDate.Size = new System.Drawing.Size(167, 20);
		this.date_StartDate.TabIndex = 42;
		this.date_StartDate.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.btn_Health.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Health.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Health.Appearance.Options.UseBackColor = true;
		this.btn_Health.EditControl = null;
		this.btn_Health.EditValue = "Health Status";
		this.btn_Health.FieldName = null;
		this.btn_Health.IsSearchControl = false;
		this.btn_Health.Location = new System.Drawing.Point(547, 126);
		this.btn_Health.Name = "btn_Health";
		this.btn_Health.ReadOnly = false;
		this.btn_Health.SettingInVisible = false;
		this.btn_Health.SettingReadonly = false;
		this.btn_Health.Size = new System.Drawing.Size(110, 20);
		this.btn_Health.TabIndex = 50;
		this.btn_Health.Text = "Health Status";
		this.btn_Health.Click += new System.EventHandler(this.btn_Health_Click);
		this.btn_Relatives.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Relatives.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Relatives.Appearance.Options.UseBackColor = true;
		this.btn_Relatives.EditControl = null;
		this.btn_Relatives.EditValue = "Relatives";
		this.btn_Relatives.FieldName = null;
		this.btn_Relatives.IsSearchControl = false;
		this.btn_Relatives.Location = new System.Drawing.Point(547, 24);
		this.btn_Relatives.Name = "btn_Relatives";
		this.btn_Relatives.ReadOnly = false;
		this.btn_Relatives.SettingInVisible = false;
		this.btn_Relatives.SettingReadonly = false;
		this.btn_Relatives.Size = new System.Drawing.Size(110, 20);
		this.btn_Relatives.TabIndex = 46;
		this.btn_Relatives.Text = "Relatives";
		this.btn_Relatives.Click += new System.EventHandler(this.btn_Relatives_Click);
		this.luk_Status.EditControl = null;
		this.luk_Status.FieldName = "status";
		this.luk_Status.IsSearchControl = false;
		this.luk_Status.Location = new System.Drawing.Point(127, 48);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Properties.IsSearchControl = false;
		this.luk_Status.Properties.NullText = "";
		this.luk_Status.Properties.PopupSizeable = false;
		this.luk_Status.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_Status.SettingInVisible = false;
		this.luk_Status.SettingReadonly = false;
		this.luk_Status.Size = new System.Drawing.Size(167, 20);
		this.luk_Status.TabIndex = 43;
		this.luk_Status.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_Status.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Status_EditValueChanging);
		this.luk_TermReason.EditControl = null;
		this.luk_TermReason.FieldName = "termReason";
		this.luk_TermReason.IsSearchControl = false;
		this.luk_TermReason.Location = new System.Drawing.Point(127, 98);
		this.luk_TermReason.Name = "luk_TermReason";
		this.luk_TermReason.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_TermReason.Properties.IsSearchControl = false;
		this.luk_TermReason.Properties.NullText = "";
		this.luk_TermReason.Properties.PopupSizeable = false;
		this.luk_TermReason.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_TermReason.SettingInVisible = false;
		this.luk_TermReason.SettingReadonly = false;
		this.luk_TermReason.Size = new System.Drawing.Size(167, 20);
		this.luk_TermReason.TabIndex = 45;
		this.luk_TermReason.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_TermReason.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_TermReason_EditValueChanging);
		this.btn_PrevEmployment.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_PrevEmployment.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_PrevEmployment.Appearance.Options.UseBackColor = true;
		this.btn_PrevEmployment.EditControl = null;
		this.btn_PrevEmployment.EditValue = "Previous Employment";
		this.btn_PrevEmployment.FieldName = null;
		this.btn_PrevEmployment.IsSearchControl = false;
		this.btn_PrevEmployment.Location = new System.Drawing.Point(547, 100);
		this.btn_PrevEmployment.Name = "btn_PrevEmployment";
		this.btn_PrevEmployment.ReadOnly = false;
		this.btn_PrevEmployment.SettingInVisible = false;
		this.btn_PrevEmployment.SettingReadonly = false;
		this.btn_PrevEmployment.Size = new System.Drawing.Size(110, 20);
		this.btn_PrevEmployment.TabIndex = 49;
		this.btn_PrevEmployment.Text = "Previous Employment";
		this.btn_PrevEmployment.Click += new System.EventHandler(this.btn_PrevEmployment_Click);
		this.btn_Review.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Review.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Review.Appearance.Options.UseBackColor = true;
		this.btn_Review.EditControl = null;
		this.btn_Review.EditValue = "Reviews";
		this.btn_Review.FieldName = null;
		this.btn_Review.IsSearchControl = false;
		this.btn_Review.Location = new System.Drawing.Point(547, 74);
		this.btn_Review.Name = "btn_Review";
		this.btn_Review.ReadOnly = false;
		this.btn_Review.SettingInVisible = false;
		this.btn_Review.SettingReadonly = false;
		this.btn_Review.Size = new System.Drawing.Size(110, 20);
		this.btn_Review.TabIndex = 48;
		this.btn_Review.Text = "Reviews";
		this.btn_Review.Click += new System.EventHandler(this.btn_Review_Click);
		this.btn_Education.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Education.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Education.Appearance.Options.UseBackColor = true;
		this.btn_Education.EditControl = null;
		this.btn_Education.EditValue = "Education";
		this.btn_Education.FieldName = null;
		this.btn_Education.IsSearchControl = false;
		this.btn_Education.Location = new System.Drawing.Point(547, 50);
		this.btn_Education.Name = "btn_Education";
		this.btn_Education.ReadOnly = false;
		this.btn_Education.SettingInVisible = false;
		this.btn_Education.SettingReadonly = false;
		this.btn_Education.Size = new System.Drawing.Size(110, 20);
		this.btn_Education.TabIndex = 47;
		this.btn_Education.Text = "Education";
		this.btn_Education.Click += new System.EventHandler(this.btn_Education_Click);
		this.btn_Absence.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Absence.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Absence.Appearance.Options.UseBackColor = true;
		this.btn_Absence.EditControl = null;
		this.btn_Absence.EditValue = "Absence";
		this.btn_Absence.FieldName = null;
		this.btn_Absence.IsSearchControl = false;
		this.btn_Absence.Location = new System.Drawing.Point(547, 152);
		this.btn_Absence.Name = "btn_Absence";
		this.btn_Absence.ReadOnly = false;
		this.btn_Absence.SettingInVisible = false;
		this.btn_Absence.SettingReadonly = false;
		this.btn_Absence.Size = new System.Drawing.Size(110, 20);
		this.btn_Absence.TabIndex = 51;
		this.btn_Absence.Text = "Absence";
		this.btn_Absence.Click += new System.EventHandler(this.btn_Absence_Click);
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Termination Date";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(12, 77);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(117, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 88;
		this.labelLine4.Utext = "Termination Date";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Termination Reason";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(11, 101);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(117, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 87;
		this.labelLine5.Utext = "Termination Reason";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Start Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 27);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(117, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 84;
		this.labelLine1.Utext = "Start Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Status";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(11, 51);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(117, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 83;
		this.labelLine3.Utext = "Status";
		this.tab_Personal.Controls.Add(this.pnl_Personal);
		this.tab_Personal.EditControl = null;
		this.tab_Personal.EditValue = "Personal";
		this.tab_Personal.FieldName = "tab_Personal";
		this.tab_Personal.IsSearchControl = false;
		this.tab_Personal.Name = "tab_Personal";
		this.tab_Personal.ReadOnly = false;
		this.tab_Personal.SettingInVisible = false;
		this.tab_Personal.SettingReadonly = false;
		this.tab_Personal.Size = new System.Drawing.Size(766, 270);
		this.tab_Personal.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Personal.Text = "Personal";
		this.pnl_Personal.Controls.Add(this.date_PassportDate);
		this.pnl_Personal.Controls.Add(this.date_PassExp);
		this.pnl_Personal.Controls.Add(this.date_Brith);
		this.pnl_Personal.Controls.Add(this.luk_MartialStt);
		this.pnl_Personal.Controls.Add(this.luk_CountryOfBirth);
		this.pnl_Personal.Controls.Add(this.luk_Citizenship);
		this.pnl_Personal.Controls.Add(this.luk_Gender);
		this.pnl_Personal.Controls.Add(this.spin_NoChildren);
		this.pnl_Personal.Controls.Add(this.txt_PasIss);
		this.pnl_Personal.Controls.Add(this.txt_PassNo);
		this.pnl_Personal.Controls.Add(this.txt_IDNo);
		this.pnl_Personal.Controls.Add(this.lbl_PassNo);
		this.pnl_Personal.Controls.Add(this.lbl_PassExpDate);
		this.pnl_Personal.Controls.Add(this.lbl_PassIss);
		this.pnl_Personal.Controls.Add(this.lbl_PasIssuer);
		this.pnl_Personal.Controls.Add(this.lbl_Citizenship);
		this.pnl_Personal.Controls.Add(this.lbl_IDNo);
		this.pnl_Personal.Controls.Add(this.lbl_Date);
		this.pnl_Personal.Controls.Add(this.lbl_CountryOfBirth);
		this.pnl_Personal.Controls.Add(this.lbl_Marital);
		this.pnl_Personal.Controls.Add(this.lbl_NoOfChildren);
		this.pnl_Personal.Controls.Add(this.lbl_Gender);
		this.pnl_Personal.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnl_Personal.Location = new System.Drawing.Point(0, 0);
		this.pnl_Personal.Name = "pnl_Personal";
		this.pnl_Personal.Size = new System.Drawing.Size(766, 270);
		this.pnl_Personal.TabIndex = 0;
		this.date_PassportDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_PassportDate.EditControl = null;
		this.date_PassportDate.EditValue = null;
		this.date_PassportDate.FieldName = "PassIssue";
		this.date_PassportDate.Location = new System.Drawing.Point(486, 78);
		this.date_PassportDate.Name = "date_PassportDate";
		this.date_PassportDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_PassportDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_PassportDate.Properties.IsSearchControl = false;
		this.date_PassportDate.SettingInVisible = false;
		this.date_PassportDate.SettingReadonly = false;
		this.date_PassportDate.Size = new System.Drawing.Size(167, 20);
		this.date_PassportDate.TabIndex = 58;
		this.date_PassportDate.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.date_PassExp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_PassExp.EditControl = null;
		this.date_PassExp.EditValue = null;
		this.date_PassExp.FieldName = "passportEx";
		this.date_PassExp.Location = new System.Drawing.Point(486, 54);
		this.date_PassExp.Name = "date_PassExp";
		this.date_PassExp.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_PassExp.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_PassExp.Properties.IsSearchControl = false;
		this.date_PassExp.SettingInVisible = false;
		this.date_PassExp.SettingReadonly = false;
		this.date_PassExp.Size = new System.Drawing.Size(167, 20);
		this.date_PassExp.TabIndex = 57;
		this.date_PassExp.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.date_Brith.EditControl = null;
		this.date_Brith.EditValue = null;
		this.date_Brith.FieldName = "birthDate";
		this.date_Brith.Location = new System.Drawing.Point(128, 30);
		this.date_Brith.Name = "date_Brith";
		this.date_Brith.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Brith.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_Brith.Properties.IsSearchControl = false;
		this.date_Brith.SettingInVisible = false;
		this.date_Brith.SettingReadonly = false;
		this.date_Brith.Size = new System.Drawing.Size(167, 20);
		this.date_Brith.TabIndex = 50;
		this.date_Brith.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_MartialStt.EditControl = null;
		this.luk_MartialStt.FieldName = "martStatus";
		this.luk_MartialStt.IsSearchControl = false;
		this.luk_MartialStt.Location = new System.Drawing.Point(128, 78);
		this.luk_MartialStt.Name = "luk_MartialStt";
		this.luk_MartialStt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_MartialStt.Properties.IsSearchControl = false;
		this.luk_MartialStt.Properties.NullText = "";
		this.luk_MartialStt.Properties.PopupSizeable = false;
		this.luk_MartialStt.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_MartialStt.SettingInVisible = false;
		this.luk_MartialStt.SettingReadonly = false;
		this.luk_MartialStt.Size = new System.Drawing.Size(167, 20);
		this.luk_MartialStt.TabIndex = 52;
		this.luk_MartialStt.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_CountryOfBirth.EditControl = null;
		this.luk_CountryOfBirth.FieldName = "brthCountr";
		this.luk_CountryOfBirth.IsSearchControl = false;
		this.luk_CountryOfBirth.Location = new System.Drawing.Point(128, 54);
		this.luk_CountryOfBirth.Name = "luk_CountryOfBirth";
		this.luk_CountryOfBirth.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_CountryOfBirth.Properties.IsSearchControl = false;
		this.luk_CountryOfBirth.Properties.NullText = "";
		this.luk_CountryOfBirth.Properties.PopupSizeable = false;
		this.luk_CountryOfBirth.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_CountryOfBirth.SettingInVisible = false;
		this.luk_CountryOfBirth.SettingReadonly = false;
		this.luk_CountryOfBirth.Size = new System.Drawing.Size(167, 20);
		this.luk_CountryOfBirth.TabIndex = 51;
		this.luk_CountryOfBirth.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_Citizenship.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_Citizenship.EditControl = null;
		this.luk_Citizenship.FieldName = "citizenshp";
		this.luk_Citizenship.IsSearchControl = false;
		this.luk_Citizenship.Location = new System.Drawing.Point(486, 6);
		this.luk_Citizenship.Name = "luk_Citizenship";
		this.luk_Citizenship.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Citizenship.Properties.IsSearchControl = false;
		this.luk_Citizenship.Properties.NullText = "";
		this.luk_Citizenship.Properties.PopupSizeable = false;
		this.luk_Citizenship.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_Citizenship.SettingInVisible = false;
		this.luk_Citizenship.SettingReadonly = false;
		this.luk_Citizenship.Size = new System.Drawing.Size(167, 20);
		this.luk_Citizenship.TabIndex = 55;
		this.luk_Citizenship.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_Gender.EditControl = null;
		this.luk_Gender.FieldName = "sex";
		this.luk_Gender.IsSearchControl = false;
		this.luk_Gender.Location = new System.Drawing.Point(128, 6);
		this.luk_Gender.Name = "luk_Gender";
		this.luk_Gender.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Gender.Properties.IsSearchControl = false;
		this.luk_Gender.Properties.NullText = "";
		this.luk_Gender.Properties.PopupSizeable = false;
		this.luk_Gender.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_Gender.SettingInVisible = false;
		this.luk_Gender.SettingReadonly = false;
		this.luk_Gender.Size = new System.Drawing.Size(167, 20);
		this.luk_Gender.TabIndex = 49;
		this.luk_Gender.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.spin_NoChildren.EditControl = null;
		this.spin_NoChildren.EditValue = new decimal(new int[4]);
		this.spin_NoChildren.FieldName = "nChildren";
		this.spin_NoChildren.IsSearchControl = false;
		this.spin_NoChildren.Location = new System.Drawing.Point(128, 102);
		this.spin_NoChildren.Name = "spin_NoChildren";
		this.spin_NoChildren.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.spin_NoChildren.Properties.IsFloatValue = false;
		this.spin_NoChildren.Properties.Mask.EditMask = "N00";
		this.spin_NoChildren.Properties.MaxValue = new decimal(new int[4] { 100, 0, 0, 0 });
		this.spin_NoChildren.SettingInVisible = false;
		this.spin_NoChildren.SettingReadonly = false;
		this.spin_NoChildren.Size = new System.Drawing.Size(167, 20);
		this.spin_NoChildren.TabIndex = 53;
		this.spin_NoChildren.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_PasIss.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_PasIss.EditControl = null;
		this.txt_PasIss.FieldName = "PassIssuer";
		this.txt_PasIss.IsSearchControl = false;
		this.txt_PasIss.Location = new System.Drawing.Point(486, 102);
		this.txt_PasIss.Name = "txt_PasIss";
		this.txt_PasIss.SettingInVisible = false;
		this.txt_PasIss.SettingReadonly = false;
		this.txt_PasIss.Size = new System.Drawing.Size(167, 20);
		this.txt_PasIss.TabIndex = 59;
		this.txt_PasIss.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_PassNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_PassNo.EditControl = null;
		this.txt_PassNo.FieldName = "passportNo";
		this.txt_PassNo.IsSearchControl = false;
		this.txt_PassNo.Location = new System.Drawing.Point(486, 30);
		this.txt_PassNo.Name = "txt_PassNo";
		this.txt_PassNo.SettingInVisible = false;
		this.txt_PassNo.SettingReadonly = false;
		this.txt_PassNo.Size = new System.Drawing.Size(167, 20);
		this.txt_PassNo.TabIndex = 56;
		this.txt_PassNo.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_IDNo.EditControl = null;
		this.txt_IDNo.FieldName = "govID";
		this.txt_IDNo.IsSearchControl = false;
		this.txt_IDNo.Location = new System.Drawing.Point(128, 126);
		this.txt_IDNo.Name = "txt_IDNo";
		this.txt_IDNo.SettingInVisible = false;
		this.txt_IDNo.SettingReadonly = false;
		this.txt_IDNo.Size = new System.Drawing.Size(167, 20);
		this.txt_IDNo.TabIndex = 54;
		this.txt_IDNo.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.lbl_PassNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_PassNo.EditControl = null;
		this.lbl_PassNo.EditValue = "Passport No.";
		this.lbl_PassNo.FieldName = null;
		this.lbl_PassNo.HideWhenEmpty = false;
		this.lbl_PassNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_PassNo.Location = new System.Drawing.Point(361, 33);
		this.lbl_PassNo.Name = "lbl_PassNo";
		this.lbl_PassNo.ReadOnly = false;
		this.lbl_PassNo.SettingInVisible = false;
		this.lbl_PassNo.SettingReadonly = false;
		this.lbl_PassNo.ShowUnderLine = true;
		this.lbl_PassNo.Size = new System.Drawing.Size(126, 20);
		this.lbl_PassNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_PassNo.TabIndex = 10;
		this.lbl_PassNo.Utext = "Passport No.";
		this.lbl_PassExpDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_PassExpDate.EditControl = null;
		this.lbl_PassExpDate.EditValue = "Passport Expiration Date";
		this.lbl_PassExpDate.FieldName = null;
		this.lbl_PassExpDate.HideWhenEmpty = false;
		this.lbl_PassExpDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_PassExpDate.Location = new System.Drawing.Point(361, 57);
		this.lbl_PassExpDate.Name = "lbl_PassExpDate";
		this.lbl_PassExpDate.ReadOnly = false;
		this.lbl_PassExpDate.SettingInVisible = false;
		this.lbl_PassExpDate.SettingReadonly = false;
		this.lbl_PassExpDate.ShowUnderLine = true;
		this.lbl_PassExpDate.Size = new System.Drawing.Size(126, 20);
		this.lbl_PassExpDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_PassExpDate.TabIndex = 9;
		this.lbl_PassExpDate.Utext = "Passport Expiration Date";
		this.lbl_PassIss.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_PassIss.EditControl = null;
		this.lbl_PassIss.EditValue = "Passport Issue Date";
		this.lbl_PassIss.FieldName = null;
		this.lbl_PassIss.HideWhenEmpty = false;
		this.lbl_PassIss.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_PassIss.Location = new System.Drawing.Point(361, 81);
		this.lbl_PassIss.Name = "lbl_PassIss";
		this.lbl_PassIss.ReadOnly = false;
		this.lbl_PassIss.SettingInVisible = false;
		this.lbl_PassIss.SettingReadonly = false;
		this.lbl_PassIss.ShowUnderLine = true;
		this.lbl_PassIss.Size = new System.Drawing.Size(126, 20);
		this.lbl_PassIss.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_PassIss.TabIndex = 8;
		this.lbl_PassIss.Utext = "Passport Issue Date";
		this.lbl_PasIssuer.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_PasIssuer.EditControl = null;
		this.lbl_PasIssuer.EditValue = "Passport Issuer";
		this.lbl_PasIssuer.FieldName = null;
		this.lbl_PasIssuer.HideWhenEmpty = false;
		this.lbl_PasIssuer.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_PasIssuer.Location = new System.Drawing.Point(361, 105);
		this.lbl_PasIssuer.Name = "lbl_PasIssuer";
		this.lbl_PasIssuer.ReadOnly = false;
		this.lbl_PasIssuer.SettingInVisible = false;
		this.lbl_PasIssuer.SettingReadonly = false;
		this.lbl_PasIssuer.ShowUnderLine = true;
		this.lbl_PasIssuer.Size = new System.Drawing.Size(126, 20);
		this.lbl_PasIssuer.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_PasIssuer.TabIndex = 7;
		this.lbl_PasIssuer.Utext = "Passport Issuer";
		this.lbl_Citizenship.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Citizenship.EditControl = null;
		this.lbl_Citizenship.EditValue = "Citizenship";
		this.lbl_Citizenship.FieldName = null;
		this.lbl_Citizenship.HideWhenEmpty = false;
		this.lbl_Citizenship.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Citizenship.Location = new System.Drawing.Point(361, 9);
		this.lbl_Citizenship.Name = "lbl_Citizenship";
		this.lbl_Citizenship.ReadOnly = false;
		this.lbl_Citizenship.SettingInVisible = false;
		this.lbl_Citizenship.SettingReadonly = false;
		this.lbl_Citizenship.ShowUnderLine = true;
		this.lbl_Citizenship.Size = new System.Drawing.Size(126, 20);
		this.lbl_Citizenship.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Citizenship.TabIndex = 6;
		this.lbl_Citizenship.Utext = "Citizenship";
		this.lbl_IDNo.EditControl = null;
		this.lbl_IDNo.EditValue = "ID No.";
		this.lbl_IDNo.FieldName = null;
		this.lbl_IDNo.HideWhenEmpty = false;
		this.lbl_IDNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_IDNo.Location = new System.Drawing.Point(12, 129);
		this.lbl_IDNo.Name = "lbl_IDNo";
		this.lbl_IDNo.ReadOnly = false;
		this.lbl_IDNo.SettingInVisible = false;
		this.lbl_IDNo.SettingReadonly = false;
		this.lbl_IDNo.ShowUnderLine = true;
		this.lbl_IDNo.Size = new System.Drawing.Size(117, 20);
		this.lbl_IDNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_IDNo.TabIndex = 5;
		this.lbl_IDNo.Utext = "ID No.";
		this.lbl_Date.EditControl = null;
		this.lbl_Date.EditValue = "Date of Birth";
		this.lbl_Date.FieldName = null;
		this.lbl_Date.HideWhenEmpty = false;
		this.lbl_Date.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Date.Location = new System.Drawing.Point(12, 33);
		this.lbl_Date.Name = "lbl_Date";
		this.lbl_Date.ReadOnly = false;
		this.lbl_Date.SettingInVisible = false;
		this.lbl_Date.SettingReadonly = false;
		this.lbl_Date.ShowUnderLine = true;
		this.lbl_Date.Size = new System.Drawing.Size(117, 20);
		this.lbl_Date.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Date.TabIndex = 4;
		this.lbl_Date.Utext = "Date of Birth";
		this.lbl_CountryOfBirth.EditControl = null;
		this.lbl_CountryOfBirth.EditValue = "Country of Birth";
		this.lbl_CountryOfBirth.FieldName = null;
		this.lbl_CountryOfBirth.HideWhenEmpty = false;
		this.lbl_CountryOfBirth.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_CountryOfBirth.Location = new System.Drawing.Point(11, 57);
		this.lbl_CountryOfBirth.Name = "lbl_CountryOfBirth";
		this.lbl_CountryOfBirth.ReadOnly = false;
		this.lbl_CountryOfBirth.SettingInVisible = false;
		this.lbl_CountryOfBirth.SettingReadonly = false;
		this.lbl_CountryOfBirth.ShowUnderLine = true;
		this.lbl_CountryOfBirth.Size = new System.Drawing.Size(118, 20);
		this.lbl_CountryOfBirth.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_CountryOfBirth.TabIndex = 3;
		this.lbl_CountryOfBirth.Utext = "Country of Birth";
		this.lbl_Marital.EditControl = null;
		this.lbl_Marital.EditValue = "Marital Status";
		this.lbl_Marital.FieldName = null;
		this.lbl_Marital.HideWhenEmpty = false;
		this.lbl_Marital.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Marital.Location = new System.Drawing.Point(11, 81);
		this.lbl_Marital.Name = "lbl_Marital";
		this.lbl_Marital.ReadOnly = false;
		this.lbl_Marital.SettingInVisible = false;
		this.lbl_Marital.SettingReadonly = false;
		this.lbl_Marital.ShowUnderLine = true;
		this.lbl_Marital.Size = new System.Drawing.Size(118, 20);
		this.lbl_Marital.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Marital.TabIndex = 2;
		this.lbl_Marital.Utext = "Marital Status";
		this.lbl_NoOfChildren.EditControl = null;
		this.lbl_NoOfChildren.EditValue = "No. of Children";
		this.lbl_NoOfChildren.FieldName = null;
		this.lbl_NoOfChildren.HideWhenEmpty = false;
		this.lbl_NoOfChildren.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_NoOfChildren.Location = new System.Drawing.Point(12, 105);
		this.lbl_NoOfChildren.Name = "lbl_NoOfChildren";
		this.lbl_NoOfChildren.ReadOnly = false;
		this.lbl_NoOfChildren.SettingInVisible = false;
		this.lbl_NoOfChildren.SettingReadonly = false;
		this.lbl_NoOfChildren.ShowUnderLine = true;
		this.lbl_NoOfChildren.Size = new System.Drawing.Size(117, 20);
		this.lbl_NoOfChildren.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_NoOfChildren.TabIndex = 1;
		this.lbl_NoOfChildren.Utext = "No. of Children";
		this.lbl_Gender.EditControl = null;
		this.lbl_Gender.EditValue = "Gender";
		this.lbl_Gender.FieldName = null;
		this.lbl_Gender.HideWhenEmpty = false;
		this.lbl_Gender.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Gender.Location = new System.Drawing.Point(12, 9);
		this.lbl_Gender.Name = "lbl_Gender";
		this.lbl_Gender.ReadOnly = false;
		this.lbl_Gender.SettingInVisible = false;
		this.lbl_Gender.SettingReadonly = false;
		this.lbl_Gender.ShowUnderLine = true;
		this.lbl_Gender.Size = new System.Drawing.Size(117, 20);
		this.lbl_Gender.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Gender.TabIndex = 0;
		this.lbl_Gender.Utext = "Gender";
		this.tab_Contract.Controls.Add(this.panelControl4);
		this.tab_Contract.EditControl = null;
		this.tab_Contract.EditValue = "Contract";
		this.tab_Contract.FieldName = "tab_Contract";
		this.tab_Contract.IsSearchControl = false;
		this.tab_Contract.Name = "tab_Contract";
		this.tab_Contract.ReadOnly = false;
		this.tab_Contract.SettingInVisible = false;
		this.tab_Contract.SettingReadonly = false;
		this.tab_Contract.Size = new System.Drawing.Size(766, 270);
		this.tab_Contract.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Contract.Text = "Contract";
		this.panelControl4.Controls.Add(this.grd_Contract);
		this.panelControl4.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl4.Location = new System.Drawing.Point(0, 0);
		this.panelControl4.Name = "panelControl4";
		this.panelControl4.Size = new System.Drawing.Size(766, 270);
		this.panelControl4.TabIndex = 2;
		this.grd_Contract.AllowPasteExcel = false;
		this.grd_Contract.ClipboardData = null;
		this.grd_Contract.ContextMenuStrip = this.contextMenuContract;
		this.grd_Contract.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Contract.Location = new System.Drawing.Point(2, 2);
		this.grd_Contract.MainView = this.gv_Contract;
		this.grd_Contract.Name = "grd_Contract";
		this.grd_Contract.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.repositoryItemDateEdit1, this.repositoryItemDateEdit2, this.luk_contractType, this.bte_SignedBy, this.luk_PymMeth, this.luk_salaryCurr, this.cal_Salary, this.rep_cfl_SubContract, this.rep_luk_ParentContract });
		this.grd_Contract.Size = new System.Drawing.Size(762, 266);
		this.grd_Contract.TabIndex = 0;
		this.grd_Contract.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Contract });
		this.contextMenuContract.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.contextMenuContract_DeleteRow });
		this.contextMenuContract.Name = "contextMenuStrip2";
		this.contextMenuContract.Size = new System.Drawing.Size(134, 26);
		this.contextMenuContract_DeleteRow.Name = "contextMenuContract_DeleteRow";
		this.contextMenuContract_DeleteRow.Size = new System.Drawing.Size(133, 22);
		this.contextMenuContract_DeleteRow.Text = "Delete Row";
		this.contextMenuContract_DeleteRow.Click += new System.EventHandler(this.contextMenuContract_DeleteRow_Click);
		this.gv_Contract.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Contract.ColumnKey = null;
		this.gv_Contract.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[15]
		{
			this.contractID, this.code, this.type, this.gcSignedBy, this.signDate, this.startDate, this.endDate, this.salary, this.gcProbSalary, this.gcBasicSalary,
			this.gcSalaryCurr, this.gcPayment, this.note, this.gcParentContract, this.IsNewRow
		});
		this.gv_Contract.DisableFormSetting = false;
		this.gv_Contract.DisplayException = false;
		this.gv_Contract.FieldCurrency = null;
		this.gv_Contract.FieldRate = null;
		this.gv_Contract.FormId = "oEmployeesInfo";
		this.gv_Contract.GridControl = this.grd_Contract;
		this.gv_Contract.IndicatorWidth = 30;
		this.gv_Contract.IsApzTable = false;
		this.gv_Contract.IsInsertNewRow = false;
		this.gv_Contract.IsLoadingData = false;
		this.gv_Contract.ItemId = "APZ_OHEC";
		this.gv_Contract.LstColumnAllowBatch = null;
		this.gv_Contract.LstColumnNotAllowBatch = null;
		this.gv_Contract.LstColumnNotEditable = null;
		this.gv_Contract.LstColumNotSetting = null;
		this.gv_Contract.Name = "gv_Contract";
		this.gv_Contract.OptionsBehavior.ImmediateUpdateRowPosition = false;
		this.gv_Contract.OptionsView.ColumnAutoWidth = false;
		this.gv_Contract.OptionsView.ShowGroupPanel = false;
		this.gv_Contract.UseBatchMod = false;
		this.gv_Contract.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Contract_CellValueChanged);
		this.contractID.Caption = "contractID";
		this.contractID.CurrencyDateField = null;
		this.contractID.CurrencyField = null;
		this.contractID.CurrencyFixed = null;
		this.contractID.CurrencyFixedRate = 0.0;
		this.contractID.CurrencyRateField = null;
		this.contractID.FieldName = "contractID";
		this.contractID.Name = "contractID";
		this.contractID.PrimaryKey = false;
		this.contractID.UnitField = null;
		this.code.Caption = "Contract Code";
		this.code.CurrencyDateField = null;
		this.code.CurrencyField = null;
		this.code.CurrencyFixed = null;
		this.code.CurrencyFixedRate = 0.0;
		this.code.CurrencyRateField = null;
		this.code.FieldName = "code";
		this.code.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.code.Name = "code";
		this.code.PrimaryKey = false;
		this.code.UnitField = null;
		this.code.Visible = true;
		this.code.VisibleIndex = 0;
		this.code.Width = 100;
		this.type.Caption = "Contract Type";
		this.type.ColumnEdit = this.luk_contractType;
		this.type.CurrencyDateField = null;
		this.type.CurrencyField = null;
		this.type.CurrencyFixed = null;
		this.type.CurrencyFixedRate = 0.0;
		this.type.CurrencyRateField = null;
		this.type.FieldName = "type";
		this.type.Name = "type";
		this.type.PrimaryKey = false;
		this.type.UnitField = null;
		this.type.Visible = true;
		this.type.VisibleIndex = 1;
		this.type.Width = 100;
		this.luk_contractType.AutoHeight = false;
		this.luk_contractType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_contractType.Name = "luk_contractType";
		this.luk_contractType.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_contractType_EditValueChanging);
		this.gcSignedBy.Caption = "Signed By";
		this.gcSignedBy.ColumnEdit = this.bte_SignedBy;
		this.gcSignedBy.CurrencyDateField = null;
		this.gcSignedBy.CurrencyField = null;
		this.gcSignedBy.CurrencyFixed = null;
		this.gcSignedBy.CurrencyFixedRate = 0.0;
		this.gcSignedBy.CurrencyRateField = null;
		this.gcSignedBy.FieldName = "signer";
		this.gcSignedBy.Name = "gcSignedBy";
		this.gcSignedBy.PrimaryKey = false;
		this.gcSignedBy.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcSignedBy.UnitField = null;
		this.gcSignedBy.Visible = true;
		this.gcSignedBy.VisibleIndex = 3;
		this.gcSignedBy.Width = 120;
		this.bte_SignedBy.AutoHeight = false;
		this.bte_SignedBy.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.bte_SignedBy.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_SignedBy.ContainerControl = this;
		this.bte_SignedBy.DisplayMember = "";
		this.bte_SignedBy.DisplayMember2 = "";
		this.bte_SignedBy.ListMandatoryField = null;
		this.bte_SignedBy.LoadDataSourceWhenEnter = false;
		this.bte_SignedBy.Name = "bte_SignedBy";
		this.bte_SignedBy.SeparateCharacterForMulti = ';';
		this.bte_SignedBy.SeparateString = ", ";
		this.bte_SignedBy.UsingAutoCompleteBox = false;
		this.bte_SignedBy.UsingDataSource = true;
		this.bte_SignedBy.ValidateWhenNoDataSource = false;
		this.bte_SignedBy.ValueMember = "";
		this.signDate.Caption = "Sign Date";
		this.signDate.ColumnEdit = this.repositoryItemDateEdit2;
		this.signDate.CurrencyDateField = null;
		this.signDate.CurrencyField = null;
		this.signDate.CurrencyFixed = null;
		this.signDate.CurrencyFixedRate = 0.0;
		this.signDate.CurrencyRateField = null;
		this.signDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.signDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.signDate.FieldName = "signDate";
		this.signDate.Name = "signDate";
		this.signDate.PrimaryKey = false;
		this.signDate.UnitField = null;
		this.signDate.Visible = true;
		this.signDate.VisibleIndex = 2;
		this.signDate.Width = 100;
		this.repositoryItemDateEdit2.AutoHeight = false;
		this.repositoryItemDateEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.Name = "repositoryItemDateEdit2";
		this.startDate.Caption = "Start Date";
		this.startDate.ColumnEdit = this.repositoryItemDateEdit2;
		this.startDate.CurrencyDateField = null;
		this.startDate.CurrencyField = null;
		this.startDate.CurrencyFixed = null;
		this.startDate.CurrencyFixedRate = 0.0;
		this.startDate.CurrencyRateField = null;
		this.startDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.startDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.startDate.FieldName = "startDate";
		this.startDate.Name = "startDate";
		this.startDate.PrimaryKey = false;
		this.startDate.UnitField = null;
		this.startDate.Visible = true;
		this.startDate.VisibleIndex = 4;
		this.startDate.Width = 100;
		this.endDate.Caption = "End Date";
		this.endDate.ColumnEdit = this.repositoryItemDateEdit1;
		this.endDate.CurrencyDateField = null;
		this.endDate.CurrencyField = null;
		this.endDate.CurrencyFixed = null;
		this.endDate.CurrencyFixedRate = 0.0;
		this.endDate.CurrencyRateField = null;
		this.endDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.endDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.endDate.FieldName = "endDate";
		this.endDate.Name = "endDate";
		this.endDate.PrimaryKey = false;
		this.endDate.UnitField = null;
		this.endDate.Visible = true;
		this.endDate.VisibleIndex = 5;
		this.endDate.Width = 100;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEdit1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEdit1.EditFormat.FormatString = "dd/MM/yyyy";
		this.repositoryItemDateEdit1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEdit1.Mask.EditMask = "dd/MM/yyyy";
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.salary.Caption = "Salary";
		this.salary.ColumnEdit = this.cal_Salary;
		this.salary.CurrencyDateField = null;
		this.salary.CurrencyField = null;
		this.salary.CurrencyFixed = null;
		this.salary.CurrencyFixedRate = 0.0;
		this.salary.CurrencyRateField = null;
		this.salary.FieldName = "salary";
		this.salary.Name = "salary";
		this.salary.PrimaryKey = false;
		this.salary.UnitField = null;
		this.salary.Visible = true;
		this.salary.VisibleIndex = 6;
		this.salary.Width = 80;
		this.cal_Salary.AutoHeight = false;
		this.cal_Salary.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_Salary.Name = "cal_Salary";
		this.gcProbSalary.Caption = "Probationary Salary";
		this.gcProbSalary.ColumnEdit = this.cal_Salary;
		this.gcProbSalary.CurrencyDateField = null;
		this.gcProbSalary.CurrencyField = null;
		this.gcProbSalary.CurrencyFixed = null;
		this.gcProbSalary.CurrencyFixedRate = 0.0;
		this.gcProbSalary.CurrencyRateField = null;
		this.gcProbSalary.FieldName = "ProbSalary";
		this.gcProbSalary.Name = "gcProbSalary";
		this.gcProbSalary.PrimaryKey = false;
		this.gcProbSalary.UnitField = null;
		this.gcProbSalary.Visible = true;
		this.gcProbSalary.VisibleIndex = 7;
		this.gcProbSalary.Width = 107;
		this.gcBasicSalary.Caption = "Basic Salary";
		this.gcBasicSalary.ColumnEdit = this.cal_Salary;
		this.gcBasicSalary.CurrencyDateField = null;
		this.gcBasicSalary.CurrencyField = null;
		this.gcBasicSalary.CurrencyFixed = null;
		this.gcBasicSalary.CurrencyFixedRate = 0.0;
		this.gcBasicSalary.CurrencyRateField = null;
		this.gcBasicSalary.FieldName = "basicSalary";
		this.gcBasicSalary.Name = "gcBasicSalary";
		this.gcBasicSalary.PrimaryKey = false;
		this.gcBasicSalary.UnitField = null;
		this.gcBasicSalary.Visible = true;
		this.gcBasicSalary.VisibleIndex = 8;
		this.gcBasicSalary.Width = 80;
		this.gcSalaryCurr.Caption = "Salary Currency";
		this.gcSalaryCurr.ColumnEdit = this.luk_salaryCurr;
		this.gcSalaryCurr.CurrencyDateField = null;
		this.gcSalaryCurr.CurrencyField = null;
		this.gcSalaryCurr.CurrencyFixed = null;
		this.gcSalaryCurr.CurrencyFixedRate = 0.0;
		this.gcSalaryCurr.CurrencyRateField = null;
		this.gcSalaryCurr.FieldName = "salaryCurr";
		this.gcSalaryCurr.Name = "gcSalaryCurr";
		this.gcSalaryCurr.PrimaryKey = false;
		this.gcSalaryCurr.UnitField = null;
		this.gcSalaryCurr.Visible = true;
		this.gcSalaryCurr.VisibleIndex = 10;
		this.gcSalaryCurr.Width = 80;
		this.luk_salaryCurr.AutoHeight = false;
		this.luk_salaryCurr.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_salaryCurr.Name = "luk_salaryCurr";
		this.luk_salaryCurr.EditValueChanged += new System.EventHandler(this.luk_salaryCurr_EditValueChanged);
		this.gcPayment.Caption = "Payment Method";
		this.gcPayment.ColumnEdit = this.luk_PymMeth;
		this.gcPayment.CurrencyDateField = null;
		this.gcPayment.CurrencyField = null;
		this.gcPayment.CurrencyFixed = null;
		this.gcPayment.CurrencyFixedRate = 0.0;
		this.gcPayment.CurrencyRateField = null;
		this.gcPayment.FieldName = "PymMeth";
		this.gcPayment.Name = "gcPayment";
		this.gcPayment.PrimaryKey = false;
		this.gcPayment.UnitField = null;
		this.gcPayment.Visible = true;
		this.gcPayment.VisibleIndex = 9;
		this.gcPayment.Width = 100;
		this.luk_PymMeth.AutoHeight = false;
		this.luk_PymMeth.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_PymMeth.Name = "luk_PymMeth";
		this.note.Caption = "Note";
		this.note.CurrencyDateField = null;
		this.note.CurrencyField = null;
		this.note.CurrencyFixed = null;
		this.note.CurrencyFixedRate = 0.0;
		this.note.CurrencyRateField = null;
		this.note.FieldName = "note";
		this.note.Name = "note";
		this.note.PrimaryKey = false;
		this.note.UnitField = null;
		this.note.Visible = true;
		this.note.VisibleIndex = 12;
		this.note.Width = 180;
		this.gcParentContract.Caption = "Subcontract Of ";
		this.gcParentContract.ColumnEdit = this.rep_luk_ParentContract;
		this.gcParentContract.CurrencyDateField = null;
		this.gcParentContract.CurrencyField = null;
		this.gcParentContract.CurrencyFixed = null;
		this.gcParentContract.CurrencyFixedRate = 0.0;
		this.gcParentContract.CurrencyRateField = null;
		this.gcParentContract.FieldName = "ParentContract";
		this.gcParentContract.Name = "gcParentContract";
		this.gcParentContract.PrimaryKey = false;
		this.gcParentContract.UnitField = null;
		this.gcParentContract.Visible = true;
		this.gcParentContract.VisibleIndex = 11;
		this.gcParentContract.Width = 85;
		this.rep_luk_ParentContract.AutoHeight = false;
		this.rep_luk_ParentContract.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_luk_ParentContract.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_ParentContract.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("contractID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_luk_ParentContract.IsSearchControl = false;
		this.rep_luk_ParentContract.Name = "rep_luk_ParentContract";
		this.rep_luk_ParentContract.NullText = "";
		this.rep_luk_ParentContract.ShowFooter = false;
		this.rep_luk_ParentContract.ShowHeader = false;
		this.rep_luk_ParentContract.PopupFilter += new DevExpress.XtraEditors.Controls.PopupFilterEventHandler(this.rep_luk_ParentContract_PopupFilter);
		this.IsNewRow.Caption = "gridColumnEx1";
		this.IsNewRow.CurrencyDateField = null;
		this.IsNewRow.CurrencyField = null;
		this.IsNewRow.CurrencyFixed = null;
		this.IsNewRow.CurrencyFixedRate = 0.0;
		this.IsNewRow.CurrencyRateField = null;
		this.IsNewRow.FieldName = "IsNewRow";
		this.IsNewRow.Name = "IsNewRow";
		this.IsNewRow.OptionsColumn.AllowEdit = false;
		this.IsNewRow.OptionsColumn.ReadOnly = true;
		this.IsNewRow.PrimaryKey = false;
		this.IsNewRow.UnitField = null;
		this.rep_cfl_SubContract.AutoHeight = false;
		this.rep_cfl_SubContract.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		this.rep_cfl_SubContract.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_cfl_SubContract.ContainerControl = this;
		this.rep_cfl_SubContract.DisplayMember = "";
		this.rep_cfl_SubContract.DisplayMember2 = "";
		this.rep_cfl_SubContract.ListMandatoryField = null;
		this.rep_cfl_SubContract.LoadDataSourceWhenEnter = false;
		this.rep_cfl_SubContract.Name = "rep_cfl_SubContract";
		this.rep_cfl_SubContract.SeparateCharacterForMulti = ';';
		this.rep_cfl_SubContract.SeparateString = ", ";
		this.rep_cfl_SubContract.UsingAutoCompleteBox = false;
		this.rep_cfl_SubContract.UsingDataSource = true;
		this.rep_cfl_SubContract.ValidateWhenNoDataSource = false;
		this.rep_cfl_SubContract.ValueMember = "";
		this.tab_Finance.Appearance.PageClient.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.tab_Finance.Appearance.PageClient.Options.UseBackColor = true;
		this.tab_Finance.Controls.Add(this.panelControl3);
		this.tab_Finance.EditControl = null;
		this.tab_Finance.EditValue = "Finance";
		this.tab_Finance.FieldName = "tab_Finance";
		this.tab_Finance.IsSearchControl = false;
		this.tab_Finance.Name = "tab_Finance";
		this.tab_Finance.ReadOnly = false;
		this.tab_Finance.SettingInVisible = false;
		this.tab_Finance.SettingReadonly = false;
		this.tab_Finance.Size = new System.Drawing.Size(766, 270);
		this.tab_Finance.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Finance.Text = "Finance";
		this.panelControl3.Controls.Add(this.txt_BankCode);
		this.panelControl3.Controls.Add(this.luk_BankCode);
		this.panelControl3.Controls.Add(this.txt_bankAccount);
		this.panelControl3.Controls.Add(this.txt_bankBranch);
		this.panelControl3.Controls.Add(this.labelLine11);
		this.panelControl3.Controls.Add(this.labelLine10);
		this.panelControl3.Controls.Add(this.labelLine9);
		this.panelControl3.Controls.Add(this.labelLine8);
		this.panelControl3.Controls.Add(this.calc_EmplCost);
		this.panelControl3.Controls.Add(this.labelLine7);
		this.panelControl3.Controls.Add(this.calc_salary);
		this.panelControl3.Controls.Add(this.labelLine6);
		this.panelControl3.Controls.Add(this.luk_salaryUnit);
		this.panelControl3.Controls.Add(this.luk_empCostUnt);
		this.panelControl3.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl3.Location = new System.Drawing.Point(0, 0);
		this.panelControl3.Name = "panelControl3";
		this.panelControl3.Size = new System.Drawing.Size(766, 270);
		this.panelControl3.TabIndex = 0;
		this.txt_BankCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_BankCode.EditControl = null;
		this.txt_BankCode.Enabled = false;
		this.txt_BankCode.FieldName = null;
		this.txt_BankCode.IsSearchControl = false;
		this.txt_BankCode.Location = new System.Drawing.Point(486, 8);
		this.txt_BankCode.Name = "txt_BankCode";
		this.txt_BankCode.SettingInVisible = false;
		this.txt_BankCode.SettingReadonly = false;
		this.txt_BankCode.Size = new System.Drawing.Size(167, 20);
		this.txt_BankCode.TabIndex = 105;
		this.txt_BankCode.Visible = false;
		this.txt_BankCode.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_BankCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_BankCode.EditControl = null;
		this.luk_BankCode.FieldName = "BankCode";
		this.luk_BankCode.IsSearchControl = false;
		this.luk_BankCode.Location = new System.Drawing.Point(486, 37);
		this.luk_BankCode.Name = "luk_BankCode";
		this.luk_BankCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_BankCode.Properties.IsSearchControl = false;
		this.luk_BankCode.Properties.NullText = "";
		this.luk_BankCode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_BankCode.SettingInVisible = false;
		this.luk_BankCode.SettingReadonly = false;
		this.luk_BankCode.Size = new System.Drawing.Size(167, 20);
		this.luk_BankCode.TabIndex = 64;
		this.luk_BankCode.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_bankAccount.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_bankAccount.EditControl = null;
		this.txt_bankAccount.FieldName = "bankAcount";
		this.txt_bankAccount.IsSearchControl = false;
		this.txt_bankAccount.Location = new System.Drawing.Point(486, 62);
		this.txt_bankAccount.Name = "txt_bankAccount";
		this.txt_bankAccount.SettingInVisible = false;
		this.txt_bankAccount.SettingReadonly = false;
		this.txt_bankAccount.Size = new System.Drawing.Size(167, 20);
		this.txt_bankAccount.TabIndex = 65;
		this.txt_bankAccount.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.txt_bankBranch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_bankBranch.EditControl = null;
		this.txt_bankBranch.FieldName = "bankBranch";
		this.txt_bankBranch.IsSearchControl = false;
		this.txt_bankBranch.Location = new System.Drawing.Point(486, 114);
		this.txt_bankBranch.Name = "txt_bankBranch";
		this.txt_bankBranch.SettingInVisible = false;
		this.txt_bankBranch.SettingReadonly = false;
		this.txt_bankBranch.Size = new System.Drawing.Size(167, 20);
		this.txt_bankBranch.TabIndex = 66;
		this.txt_bankBranch.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.labelLine11.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine11.EditControl = null;
		this.labelLine11.EditValue = "Branch";
		this.labelLine11.FieldName = null;
		this.labelLine11.HideWhenEmpty = false;
		this.labelLine11.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine11.Location = new System.Drawing.Point(361, 117);
		this.labelLine11.Name = "labelLine11";
		this.labelLine11.ReadOnly = false;
		this.labelLine11.SettingInVisible = false;
		this.labelLine11.SettingReadonly = false;
		this.labelLine11.ShowUnderLine = true;
		this.labelLine11.Size = new System.Drawing.Size(126, 20);
		this.labelLine11.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine11.TabIndex = 104;
		this.labelLine11.Utext = "Branch";
		this.labelLine10.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine10.EditControl = null;
		this.labelLine10.EditValue = "Account No.";
		this.labelLine10.FieldName = null;
		this.labelLine10.HideWhenEmpty = false;
		this.labelLine10.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine10.Location = new System.Drawing.Point(360, 65);
		this.labelLine10.Name = "labelLine10";
		this.labelLine10.ReadOnly = false;
		this.labelLine10.SettingInVisible = false;
		this.labelLine10.SettingReadonly = false;
		this.labelLine10.ShowUnderLine = true;
		this.labelLine10.Size = new System.Drawing.Size(127, 20);
		this.labelLine10.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine10.TabIndex = 102;
		this.labelLine10.Utext = "Account No.";
		this.labelLine9.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Bank Details";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(361, 8);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(66, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 101;
		this.labelLine9.Utext = "Bank Details";
		this.labelLine8.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Bank";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(361, 40);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(126, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 99;
		this.labelLine8.Utext = "Bank";
		this.calc_EmplCost.EditControl = null;
		this.calc_EmplCost.FieldName = "EmplCost";
		this.calc_EmplCost.IsSearchControl = false;
		this.calc_EmplCost.Location = new System.Drawing.Point(105, 89);
		this.calc_EmplCost.Name = "calc_EmplCost";
		this.calc_EmplCost.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_EmplCost.Properties.IsSearchControl = false;
		this.calc_EmplCost.SettingInVisible = false;
		this.calc_EmplCost.SettingReadonly = false;
		this.calc_EmplCost.Size = new System.Drawing.Size(133, 20);
		this.calc_EmplCost.TabIndex = 62;
		this.calc_EmplCost.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Employee Cost";
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(8, 92);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(117, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 96;
		this.labelLine7.Utext = "Employee Cost";
		this.calc_salary.EditControl = null;
		this.calc_salary.FieldName = "salary";
		this.calc_salary.IsSearchControl = false;
		this.calc_salary.Location = new System.Drawing.Point(105, 37);
		this.calc_salary.Name = "calc_salary";
		this.calc_salary.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_salary.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.calc_salary.Properties.IsSearchControl = false;
		this.calc_salary.Properties.ReadOnly = true;
		this.calc_salary.Properties.CustomDisplayText += new DevExpress.XtraEditors.Controls.CustomDisplayTextEventHandler(this.calc_salary_Properties_CustomDisplayText);
		this.calc_salary.SettingInVisible = false;
		this.calc_salary.SettingReadonly = false;
		this.calc_salary.Size = new System.Drawing.Size(133, 20);
		this.calc_salary.TabIndex = 60;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Salary";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(8, 40);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(117, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 93;
		this.labelLine6.Utext = "Salary";
		this.luk_salaryUnit.EditControl = null;
		this.luk_salaryUnit.FieldName = "salaryUnit";
		this.luk_salaryUnit.IsSearchControl = false;
		this.luk_salaryUnit.Location = new System.Drawing.Point(244, 37);
		this.luk_salaryUnit.Name = "luk_salaryUnit";
		this.luk_salaryUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_salaryUnit.Properties.IsSearchControl = false;
		this.luk_salaryUnit.Properties.NullText = "";
		this.luk_salaryUnit.Properties.PopupSizeable = false;
		this.luk_salaryUnit.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_salaryUnit.SettingInVisible = false;
		this.luk_salaryUnit.SettingReadonly = false;
		this.luk_salaryUnit.Size = new System.Drawing.Size(111, 20);
		this.luk_salaryUnit.TabIndex = 61;
		this.luk_salaryUnit.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_empCostUnt.EditControl = null;
		this.luk_empCostUnt.FieldName = "empCostUnt";
		this.luk_empCostUnt.IsSearchControl = false;
		this.luk_empCostUnt.Location = new System.Drawing.Point(244, 89);
		this.luk_empCostUnt.Name = "luk_empCostUnt";
		this.luk_empCostUnt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_empCostUnt.Properties.IsSearchControl = false;
		this.luk_empCostUnt.Properties.NullText = "";
		this.luk_empCostUnt.Properties.PopupSizeable = false;
		this.luk_empCostUnt.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_empCostUnt.SettingInVisible = false;
		this.luk_empCostUnt.SettingReadonly = false;
		this.luk_empCostUnt.Size = new System.Drawing.Size(111, 20);
		this.luk_empCostUnt.TabIndex = 63;
		this.luk_empCostUnt.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.tab_Remarks.Controls.Add(this.txt_Remark);
		this.tab_Remarks.EditControl = null;
		this.tab_Remarks.EditValue = "Remarks";
		this.tab_Remarks.FieldName = "tab_Remarks";
		this.tab_Remarks.IsSearchControl = false;
		this.tab_Remarks.Name = "tab_Remarks";
		this.tab_Remarks.ReadOnly = false;
		this.tab_Remarks.SettingInVisible = false;
		this.tab_Remarks.SettingReadonly = false;
		this.tab_Remarks.Size = new System.Drawing.Size(766, 270);
		this.tab_Remarks.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Remarks.Text = "Remarks";
		this.txt_Remark.Dock = System.Windows.Forms.DockStyle.Fill;
		this.txt_Remark.EditControl = null;
		this.txt_Remark.FieldName = "remark";
		this.txt_Remark.IsSearchControl = false;
		this.txt_Remark.Location = new System.Drawing.Point(0, 0);
		this.txt_Remark.Name = "txt_Remark";
		this.txt_Remark.SettingInVisible = false;
		this.txt_Remark.SettingReadonly = false;
		this.txt_Remark.Size = new System.Drawing.Size(766, 270);
		this.txt_Remark.TabIndex = 0;
		this.txt_Remark.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.tab_Attachments.Controls.Add(this.panelControl5);
		this.tab_Attachments.EditControl = null;
		this.tab_Attachments.EditValue = "Attachments";
		this.tab_Attachments.FieldName = "tab_Attachments";
		this.tab_Attachments.IsSearchControl = false;
		this.tab_Attachments.Name = "tab_Attachments";
		this.tab_Attachments.ReadOnly = false;
		this.tab_Attachments.SettingInVisible = false;
		this.tab_Attachments.SettingReadonly = false;
		this.tab_Attachments.Size = new System.Drawing.Size(766, 270);
		this.tab_Attachments.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Attachments.Text = "Attachments";
		this.panelControl5.Controls.Add(this.ucAttachment1);
		this.panelControl5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl5.Location = new System.Drawing.Point(0, 0);
		this.panelControl5.Name = "panelControl5";
		this.panelControl5.Size = new System.Drawing.Size(766, 270);
		this.panelControl5.TabIndex = 1;
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oEmployeesInfo";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(2, 2);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(762, 266);
		this.ucAttachment1.TabIndex = 0;
		this.tab_User.Controls.Add(this.grd_Udf);
		this.tab_User.EditControl = null;
		this.tab_User.EditValue = "User Defined Fields";
		this.tab_User.FieldName = "tab_User";
		this.tab_User.IsSearchControl = false;
		this.tab_User.Name = "tab_User";
		this.tab_User.ReadOnly = false;
		this.tab_User.SettingInVisible = false;
		this.tab_User.SettingReadonly = false;
		this.tab_User.Size = new System.Drawing.Size(766, 270);
		this.tab_User.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_User.Text = "User Defined Fields";
		this.grd_Udf.AllowPasteExcel = false;
		this.grd_Udf.ClipboardData = null;
		this.grd_Udf.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Udf.Location = new System.Drawing.Point(0, 0);
		this.grd_Udf.MainView = this.gv_Udf;
		this.grd_Udf.Name = "grd_Udf";
		this.grd_Udf.Size = new System.Drawing.Size(766, 270);
		this.grd_Udf.TabIndex = 0;
		this.grd_Udf.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Udf });
		this.gv_Udf.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Udf.ColumnKey = null;
		this.gv_Udf.DisableFormSetting = false;
		this.gv_Udf.DisplayException = false;
		this.gv_Udf.FieldCurrency = null;
		this.gv_Udf.FieldRate = null;
		this.gv_Udf.FormId = "oEmployeesInfo";
		this.gv_Udf.GridControl = this.grd_Udf;
		this.gv_Udf.IndicatorWidth = 35;
		this.gv_Udf.IsApzTable = false;
		this.gv_Udf.IsInsertNewRow = false;
		this.gv_Udf.IsLoadingData = false;
		this.gv_Udf.ItemId = "OUDF";
		this.gv_Udf.LstColumnAllowBatch = null;
		this.gv_Udf.LstColumnNotAllowBatch = null;
		this.gv_Udf.LstColumnNotEditable = null;
		this.gv_Udf.LstColumNotSetting = null;
		this.gv_Udf.Name = "gv_Udf";
		this.gv_Udf.OptionsView.ShowGroupPanel = false;
		this.gv_Udf.UseBatchMod = false;
		this.pic_Images.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.pic_Images.Cursor = System.Windows.Forms.Cursors.Default;
		this.pic_Images.Location = new System.Drawing.Point(663, 7);
		this.pic_Images.Name = "pic_Images";
		this.pic_Images.Properties.ContextMenuStrip = this.contextMenuStrip3;
		this.pic_Images.Properties.OptionsMask.MaskLayoutMode = DevExpress.XtraEditors.Controls.PictureEditMaskLayoutMode.MiddleCenter;
		this.pic_Images.Properties.ShowCameraMenuItem = DevExpress.XtraEditors.Controls.CameraMenuItemVisibility.Always;
		this.pic_Images.Properties.SizeMode = DevExpress.XtraEditors.Controls.PictureSizeMode.Stretch;
		this.pic_Images.Size = new System.Drawing.Size(105, 140);
		this.pic_Images.TabIndex = 23;
		this.pic_Images.EditValueChanged += new System.EventHandler(this.pic_Images_EditValueChanged);
		this.pic_Images.Click += new System.EventHandler(this.pic_Images_Click);
		this.contextMenuStrip3.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deletePictureToolStripMenuItem });
		this.contextMenuStrip3.Name = "contextMenuStrip3";
		this.contextMenuStrip3.Size = new System.Drawing.Size(148, 26);
		this.deletePictureToolStripMenuItem.Name = "deletePictureToolStripMenuItem";
		this.deletePictureToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
		this.deletePictureToolStripMenuItem.Text = "Delete Picture";
		this.deletePictureToolStripMenuItem.Click += new System.EventHandler(this.deletePictureToolStripMenuItem_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 604);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 59;
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
		this.btnCancel.Location = new System.Drawing.Point(70, 604);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 57;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.luk_Position.EditControl = null;
		this.luk_Position.FieldName = "position";
		this.luk_Position.IsSearchControl = false;
		this.luk_Position.Location = new System.Drawing.Point(129, 97);
		this.luk_Position.Name = "luk_Position";
		this.luk_Position.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Position.Properties.IsSearchControl = false;
		this.luk_Position.Properties.NullText = "";
		this.luk_Position.Properties.PopupSizeable = false;
		this.luk_Position.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_Position.SettingInVisible = false;
		this.luk_Position.SettingReadonly = false;
		this.luk_Position.Size = new System.Drawing.Size(167, 20);
		this.luk_Position.TabIndex = 5;
		this.luk_Position.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_Position.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Position_EditValueChanging);
		this.luk_Department.EditControl = null;
		this.luk_Department.FieldName = "dept";
		this.luk_Department.IsSearchControl = false;
		this.luk_Department.Location = new System.Drawing.Point(129, 121);
		this.luk_Department.Name = "luk_Department";
		this.luk_Department.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Department.Properties.IsSearchControl = false;
		this.luk_Department.Properties.NullText = "";
		this.luk_Department.Properties.PopupSizeable = false;
		this.luk_Department.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_Department.SettingInVisible = false;
		this.luk_Department.SettingReadonly = false;
		this.luk_Department.Size = new System.Drawing.Size(167, 20);
		this.luk_Department.TabIndex = 6;
		this.luk_Department.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_Department.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Department_EditValueChanging);
		this.luk_Branch.EditControl = null;
		this.luk_Branch.FieldName = "branch";
		this.luk_Branch.IsSearchControl = false;
		this.luk_Branch.Location = new System.Drawing.Point(129, 145);
		this.luk_Branch.Name = "luk_Branch";
		this.luk_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Branch.Properties.IsSearchControl = false;
		this.luk_Branch.Properties.NullText = "";
		this.luk_Branch.Properties.PopupSizeable = false;
		this.luk_Branch.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_Branch.SettingInVisible = false;
		this.luk_Branch.SettingReadonly = false;
		this.luk_Branch.Size = new System.Drawing.Size(167, 20);
		this.luk_Branch.TabIndex = 7;
		this.luk_Branch.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_Branch.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Branches_EditValueChanging);
		this.luk_UserCode.EditControl = null;
		this.luk_UserCode.FieldName = "userId";
		this.luk_UserCode.IsSearchControl = false;
		this.luk_UserCode.Location = new System.Drawing.Point(129, 193);
		this.luk_UserCode.Name = "luk_UserCode";
		this.luk_UserCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_UserCode.Properties.IsSearchControl = false;
		this.luk_UserCode.Properties.NullText = "";
		this.luk_UserCode.Properties.PopupSizeable = false;
		this.luk_UserCode.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_UserCode.SettingInVisible = false;
		this.luk_UserCode.SettingReadonly = false;
		this.luk_UserCode.Size = new System.Drawing.Size(167, 20);
		this.luk_UserCode.TabIndex = 9;
		this.luk_UserCode.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_SalesEmp.EditControl = null;
		this.luk_SalesEmp.FieldName = "salesPrson";
		this.luk_SalesEmp.IsSearchControl = false;
		this.luk_SalesEmp.Location = new System.Drawing.Point(129, 217);
		this.luk_SalesEmp.Name = "luk_SalesEmp";
		this.luk_SalesEmp.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_SalesEmp.Properties.IsSearchControl = false;
		this.luk_SalesEmp.Properties.NullText = "";
		this.luk_SalesEmp.Properties.PopupSizeable = false;
		this.luk_SalesEmp.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_SalesEmp.SettingInVisible = false;
		this.luk_SalesEmp.SettingReadonly = false;
		this.luk_SalesEmp.Size = new System.Drawing.Size(167, 20);
		this.luk_SalesEmp.TabIndex = 10;
		this.luk_SalesEmp.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.contextMenuStrip4.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.removeEmployeeToolStripMenuItem, this.organizationalChartToolStripMenuItem });
		this.contextMenuStrip4.Name = "contextMenuStrip4";
		this.contextMenuStrip4.Size = new System.Drawing.Size(184, 48);
		this.contextMenuStrip4.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip4_Opening);
		this.removeEmployeeToolStripMenuItem.Name = "removeEmployeeToolStripMenuItem";
		this.removeEmployeeToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
		this.removeEmployeeToolStripMenuItem.Text = "Remove Employee";
		this.removeEmployeeToolStripMenuItem.Click += new System.EventHandler(this.removeEmployeeToolStripMenuItem_Click);
		this.organizationalChartToolStripMenuItem.Name = "organizationalChartToolStripMenuItem";
		this.organizationalChartToolStripMenuItem.Size = new System.Drawing.Size(183, 22);
		this.organizationalChartToolStripMenuItem.Text = "Organizational Chart";
		this.organizationalChartToolStripMenuItem.Click += new System.EventHandler(this.organizationalChartToolStripMenuItem_Click);
		this.luk_Manager1.EditControl = null;
		this.luk_Manager1.FieldName = "manager";
		this.luk_Manager1.IsSearchControl = false;
		this.luk_Manager1.Location = new System.Drawing.Point(129, 169);
		this.luk_Manager1.Name = "luk_Manager1";
		this.luk_Manager1.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.luk_Manager1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_Manager1.Properties.ContainerControl = null;
		this.luk_Manager1.Properties.DisplayMember = "";
		this.luk_Manager1.Properties.DisplayMember2 = "";
		this.luk_Manager1.Properties.ListMandatoryField = null;
		this.luk_Manager1.Properties.LoadDataSourceWhenEnter = false;
		this.luk_Manager1.Properties.SeparateCharacterForMulti = ';';
		this.luk_Manager1.Properties.SeparateString = ", ";
		this.luk_Manager1.Properties.UsingAutoCompleteBox = false;
		this.luk_Manager1.Properties.UsingDataSource = true;
		this.luk_Manager1.Properties.ValidateWhenNoDataSource = false;
		this.luk_Manager1.Properties.ValueMember = "";
		this.luk_Manager1.SettingInVisible = false;
		this.luk_Manager1.SettingReadonly = false;
		this.luk_Manager1.Size = new System.Drawing.Size(167, 20);
		this.luk_Manager1.TabIndex = 8;
		this.luk_Manager1.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.luk_Manager1.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.luk_Manager1_EditValueChanging);
		this.luk_CostCenter.EditControl = null;
		this.luk_CostCenter.FieldName = "CostCenter";
		this.luk_CostCenter.IsSearchControl = false;
		this.luk_CostCenter.Location = new System.Drawing.Point(129, 244);
		this.luk_CostCenter.Name = "luk_CostCenter";
		this.luk_CostCenter.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.luk_CostCenter.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_CostCenter.Properties.ContainerControl = null;
		this.luk_CostCenter.Properties.DisplayMember = "PrcCode";
		this.luk_CostCenter.Properties.DisplayMember2 = "";
		this.luk_CostCenter.Properties.ListMandatoryField = null;
		this.luk_CostCenter.Properties.LoadDataSourceWhenEnter = false;
		this.luk_CostCenter.Properties.SeparateCharacterForMulti = ';';
		this.luk_CostCenter.Properties.SeparateString = ", ";
		this.luk_CostCenter.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListOfCostCenter;
		this.luk_CostCenter.Properties.UsingAutoCompleteBox = false;
		this.luk_CostCenter.Properties.UsingDataSource = true;
		this.luk_CostCenter.Properties.ValidateWhenNoDataSource = false;
		this.luk_CostCenter.Properties.ValueMember = "PrcCode";
		this.luk_CostCenter.SettingInVisible = false;
		this.luk_CostCenter.SettingReadonly = false;
		this.luk_CostCenter.Size = new System.Drawing.Size(167, 20);
		this.luk_CostCenter.TabIndex = 11;
		this.luk_CostCenter.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.lbl_CostCenter.EditControl = null;
		this.lbl_CostCenter.EditValue = "Cost Center";
		this.lbl_CostCenter.FieldName = null;
		this.lbl_CostCenter.HideWhenEmpty = false;
		this.lbl_CostCenter.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_CostCenter.Location = new System.Drawing.Point(13, 247);
		this.lbl_CostCenter.Name = "lbl_CostCenter";
		this.lbl_CostCenter.ReadOnly = false;
		this.lbl_CostCenter.SettingInVisible = false;
		this.lbl_CostCenter.SettingReadonly = false;
		this.lbl_CostCenter.ShowUnderLine = true;
		this.lbl_CostCenter.Size = new System.Drawing.Size(117, 20);
		this.lbl_CostCenter.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_CostCenter.TabIndex = 87;
		this.lbl_CostCenter.Utext = "Cost Center";
		this.luk_LinkedVendor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_LinkedVendor.EditControl = null;
		this.luk_LinkedVendor.FieldName = "BPLink";
		this.luk_LinkedVendor.IsSearchControl = false;
		this.luk_LinkedVendor.Location = new System.Drawing.Point(487, 273);
		this.luk_LinkedVendor.Name = "luk_LinkedVendor";
		this.luk_LinkedVendor.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.luk_LinkedVendor.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_LinkedVendor.Properties.ContainerControl = null;
		this.luk_LinkedVendor.Properties.DisplayMember = "CardCode";
		this.luk_LinkedVendor.Properties.DisplayMember2 = "";
		this.luk_LinkedVendor.Properties.ListMandatoryField = null;
		this.luk_LinkedVendor.Properties.LoadDataSourceWhenEnter = false;
		this.luk_LinkedVendor.Properties.SeparateCharacterForMulti = ';';
		this.luk_LinkedVendor.Properties.SeparateString = ", ";
		this.luk_LinkedVendor.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.VendorTypeEmployee;
		this.luk_LinkedVendor.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.luk_LinkedVendor.Properties.UsingAutoCompleteBox = false;
		this.luk_LinkedVendor.Properties.UsingDataSource = true;
		this.luk_LinkedVendor.Properties.ValidateWhenNoDataSource = false;
		this.luk_LinkedVendor.Properties.ValueMember = "CardCode";
		this.luk_LinkedVendor.SettingInVisible = false;
		this.luk_LinkedVendor.SettingReadonly = false;
		this.luk_LinkedVendor.Size = new System.Drawing.Size(167, 20);
		this.luk_LinkedVendor.TabIndex = 22;
		this.luk_LinkedVendor.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.lbl_LinkedVendor.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_LinkedVendor.EditControl = null;
		this.lbl_LinkedVendor.EditValue = "Linked Vendor";
		this.lbl_LinkedVendor.FieldName = null;
		this.lbl_LinkedVendor.HideWhenEmpty = false;
		this.lbl_LinkedVendor.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_LinkedVendor.Location = new System.Drawing.Point(362, 276);
		this.lbl_LinkedVendor.Name = "lbl_LinkedVendor";
		this.lbl_LinkedVendor.ReadOnly = false;
		this.lbl_LinkedVendor.SettingInVisible = false;
		this.lbl_LinkedVendor.SettingReadonly = false;
		this.lbl_LinkedVendor.ShowUnderLine = true;
		this.lbl_LinkedVendor.Size = new System.Drawing.Size(126, 20);
		this.lbl_LinkedVendor.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_LinkedVendor.TabIndex = 89;
		this.lbl_LinkedVendor.Utext = "Linked Vendor";
		this.txt_empCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_empCode.EditControl = null;
		this.txt_empCode.Enabled = false;
		this.txt_empCode.FieldName = "empID";
		this.txt_empCode.IsSearchControl = false;
		this.txt_empCode.Location = new System.Drawing.Point(487, 31);
		this.txt_empCode.Name = "txt_empCode";
		this.txt_empCode.SettingInVisible = false;
		this.txt_empCode.SettingReadonly = false;
		this.txt_empCode.Size = new System.Drawing.Size(167, 20);
		this.txt_empCode.TabIndex = 90;
		this.txt_empCode.EditValueChanged += new System.EventHandler(this.OnContentChanged);
		this.lbl_empCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_empCode.EditControl = null;
		this.lbl_empCode.EditValue = "Employee Code";
		this.lbl_empCode.Enabled = false;
		this.lbl_empCode.FieldName = null;
		this.lbl_empCode.HideWhenEmpty = false;
		this.lbl_empCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_empCode.Location = new System.Drawing.Point(362, 35);
		this.lbl_empCode.Name = "lbl_empCode";
		this.lbl_empCode.ReadOnly = false;
		this.lbl_empCode.SettingInVisible = false;
		this.lbl_empCode.SettingReadonly = false;
		this.lbl_empCode.ShowUnderLine = true;
		this.lbl_empCode.Size = new System.Drawing.Size(126, 20);
		this.lbl_empCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_empCode.TabIndex = 91;
		this.lbl_empCode.Utext = "Employee Code";
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(771, 632);
		this.ContextMenuStrip = this.contextMenuStrip4;
		base.Controls.Add(this.txt_empCode);
		base.Controls.Add(this.lbl_empCode);
		base.Controls.Add(this.luk_LinkedVendor);
		base.Controls.Add(this.lbl_LinkedVendor);
		base.Controls.Add(this.luk_CostCenter);
		base.Controls.Add(this.lbl_CostCenter);
		base.Controls.Add(this.luk_Manager1);
		base.Controls.Add(this.luk_Department);
		base.Controls.Add(this.luk_Branch);
		base.Controls.Add(this.luk_UserCode);
		base.Controls.Add(this.luk_SalesEmp);
		base.Controls.Add(this.luk_Position);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.pic_Images);
		base.Controls.Add(this.tab_Detail);
		base.Controls.Add(this.txt_HomePhone);
		base.Controls.Add(this.txt_Fax);
		base.Controls.Add(this.txt_Email);
		base.Controls.Add(this.txt_ExtPhone);
		base.Controls.Add(this.txt_MobilePhone);
		base.Controls.Add(this.txt_Pager);
		base.Controls.Add(this.txt_OfficePhone);
		base.Controls.Add(this.chk_Active);
		base.Controls.Add(this.txt_ExtEmpNo);
		base.Controls.Add(this.txt_EmpNo);
		base.Controls.Add(this.lbl_Email);
		base.Controls.Add(this.lbl_SalesEmp);
		base.Controls.Add(this.txt_JobTitle);
		base.Controls.Add(this.txt_LastName);
		base.Controls.Add(this.txt_MiddleName);
		base.Controls.Add(this.txt_FirstName);
		base.Controls.Add(this.lbl_Fax);
		base.Controls.Add(this.lbl_Home);
		base.Controls.Add(this.lbl_ExtEmp);
		base.Controls.Add(this.lbl_off);
		base.Controls.Add(this.lbl_ext);
		base.Controls.Add(this.lbl_MPhone);
		base.Controls.Add(this.lbl_Pager);
		base.Controls.Add(this.lbl_EmpNo);
		base.Controls.Add(this.lbl_Branch);
		base.Controls.Add(this.lbl_Manager);
		base.Controls.Add(this.lbl_UserCode);
		base.Controls.Add(this.lbl_MiddleName);
		base.Controls.Add(this.lbl_LastName);
		base.Controls.Add(this.lbl_Job);
		base.Controls.Add(this.lbl_Position);
		base.Controls.Add(this.lbl_Department);
		base.Controls.Add(this.lbl_FirstName);
		base.FormId = "oEmployeesInfo";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_HumanResources.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_HumanResources";
		base.ShowInTaskbar = false;
		this.Text = "Employee Master Data";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_HumanResources_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.txt_FirstName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MiddleName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LastName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_JobTitle.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_EmpNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ExtEmpNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OfficePhone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Pager.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MobilePhone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ExtPhone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Email.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_HomePhone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tab_Detail).EndInit();
		this.tab_Detail.ResumeLayout(false);
		this.tab_Address.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pnl_Address).EndInit();
		this.pnl_Address.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.luk_H_State.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_H_Country.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_W_Country.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_W_State.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Block.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_StreetNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Street.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Street.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Block.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_StreetNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_County.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_City.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Zipcode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Build.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_County.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Build.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_City.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Zipcode.Properties).EndInit();
		this.tab_Membership.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Roles).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Roles).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rlup_Role).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Teams).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Teams).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rlup_Team).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rlup_TeamRole).EndInit();
		this.Tab_Administration.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_TermDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_TermDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_StartDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_TermReason.Properties).EndInit();
		this.tab_Personal.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pnl_Personal).EndInit();
		this.pnl_Personal.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_PassportDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_PassportDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_PassExp.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_PassExp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Brith.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Brith.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_MartialStt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CountryOfBirth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Citizenship.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Gender.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.spin_NoChildren.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PasIss.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PassNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_IDNo.Properties).EndInit();
		this.tab_Contract.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl4).EndInit();
		this.panelControl4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Contract).EndInit();
		this.contextMenuContract.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Contract).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_contractType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_SignedBy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Salary).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_salaryCurr).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PymMeth).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_ParentContract).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_cfl_SubContract).EndInit();
		this.tab_Finance.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl3).EndInit();
		this.panelControl3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_BankCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BankCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_bankAccount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_bankBranch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_EmplCost.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_salary.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_salaryUnit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_empCostUnt.Properties).EndInit();
		this.tab_Remarks.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Remark.Properties).EndInit();
		this.tab_Attachments.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl5).EndInit();
		this.panelControl5.ResumeLayout(false);
		this.tab_User.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Udf).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Udf).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pic_Images.Properties).EndInit();
		this.contextMenuStrip3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.luk_Position.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_UserCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_SalesEmp.Properties).EndInit();
		this.contextMenuStrip4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.luk_Manager1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CostCenter.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_LinkedVendor.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_empCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
