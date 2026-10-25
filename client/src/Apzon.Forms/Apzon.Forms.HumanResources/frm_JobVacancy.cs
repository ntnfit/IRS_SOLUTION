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
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.HumanResources;

public class frm_JobVacancy : XtraFormEx
{
	private const string Controller = "JobVacancy";

	private DataTable _dtJobVacancy;

	private DataTable _dtHiringTeam;

	private DataTable _dtAdvertisement;

	private DataTable _dtApplicants;

	private DataTable _dtInterview;

	private DataTable _dtOffer;

	private DataTable _dtHired;

	private DataTable _dtQualifications;

	private DataTable _dtAppInInter;

	private bool success;

	private bool _isLoading;

	private bool isSave;

	private bool _isCFl;

	private DataTable dtApplicantInterview;

	private List<CurrencyCodesImpl> salaryCurrData;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_JobVacancy;

	private GridViewEx gv_JobVacancy;

	private XtraTabControlEx xtra_APZ_HJV6;

	private XtraTabPageEx xtra_APZ_HJV1;

	private XtraTabPageEx xtra_APZ_HJV2;

	private SplitContainerControl splitContainerControl1;

	private XtraTabPageEx xtra_APZ_HJV3;

	private XtraTabPageEx xtra_APZ_HJV4;

	private XtraTabPageEx xtra_APZ_HJV5;

	private XtraTabPageEx xtra_Hired;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private DateEditEx date_FromDate;

	private DateEditEx date_ToDate;

	private LabelLine labelLine4;

	private TextEditEx txt_DocNo;

	private SimpleButtonEx btn_Search;

	private GridColumnEx DocEntry;

	private GridColumnEx DocNum;

	private GridColumnEx Series;

	private GridColumnEx Description;

	private GridColumnEx Status;

	private GridColumnEx Branch;

	private GridColumnEx Department;

	private GridColumnEx Positions;

	private GridColumnEx JobTitle;

	private GridColumnEx FromDate;

	private GridColumnEx ToDate;

	private GridColumnEx Qty;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private RepositoryItemLookUpEdit lok_Status;

	private RepositoryItemLookUpEdit luk_Department;

	private RepositoryItemLookUpEdit luk_Position;

	private RepositoryItemLookUpEdit luk_Branch;

	private RepositoryItemLookUpEdit luk_series;

	private XtraTabPageEx xtra_APZ_HJV7;

	private GridControlEx grd_Hiring;

	private GridViewEx gv_Hiring;

	private GridColumnEx Hiring_DocEntry;

	private GridColumnEx Hiring_LineNum;

	private GridColumnEx Hiring_EmpID;

	private GridColumnEx Hiring_EmpName;

	private GridColumnEx Hiring_TeamRole;

	private GridColumnEx Hiring_Primary;

	private GridControlEx grd_Advertisement;

	private GridViewEx gv_Advertisement;

	private GridColumnEx Ad_DocEntry;

	private GridColumnEx Ad_LineNum;

	private GridColumnEx Ad_AdvID;

	private GridColumnEx Ad_AdvName;

	private GridColumnEx Ad_AdvSources;

	private GridColumnEx Ad_AdvCost;

	private GridColumnEx Ad_Currency;

	private GridColumnEx Ad_FromDate;

	private GridColumnEx Ad_ToDate;

	private GridColumnEx Ad_Remark;

	private GridControlEx grd_App;

	private GridViewEx gv_App;

	private GridColumnEx Ap_DocEntry;

	private GridColumnEx Ap_LineNum;

	private GridColumnEx Ap_ApplicantNo;

	private GridColumnEx Ap_ApplicantName;

	private GridColumnEx Ap_DoB;

	private GridColumnEx Ap_Gender;

	private GridColumnEx Ap_Qualifications;

	private GridColumnEx Ap_Address;

	private GridColumnEx Ap_Phone;

	private GridColumnEx Email;

	private GridColumnEx Ap_InterviewStatus;

	private GridControlEx grd_Inte;

	private GridViewEx gv_Inte;

	private GridColumnEx Inte_DocEntry;

	private GridColumnEx Inte_LineNum;

	private GridColumnEx Inte_InterviewID;

	private GridColumnEx Inte_InterviewName;

	private GridColumnEx Inte_InterviewType;

	private GridColumnEx Inte_QuestionsLibrary;

	private GridColumnEx Inte_InterviewTeam;

	private GridColumnEx Inte_VisOrder;

	private GridColumnEx Inte_Status;

	private GridColumnEx Inte_Date;

	private GridControlEx grd_Offer;

	private GridViewEx gv_Offer;

	private GridColumnEx Of_DocEntry;

	private GridColumnEx Of_LineNum;

	private GridColumnEx Of_ApplicantName;

	private GridColumnEx Of_OfferLetter;

	private GridColumnEx Of_TrialPeriodSalary;

	private GridColumnEx Of_ContractSalary;

	private GridColumnEx Of_OtherBenefits;

	private RepositoryItemDateEdit r_FromDate;

	private RepositoryItemDateEdit r_ToDate;

	private RepositoryItemCalcEdit r_AdvCost;

	private RepositoryItemDateEdit r_AdvFromDate;

	private RepositoryItemDateEdit r_AdvToDate;

	private RepositoryItemDateEdit r_AppDoB;

	private SimpleButtonEx btn_Profile;

	private SimpleButtonEx btn_Hired;

	private GridControlEx grd_Hired;

	private GridViewEx gv_Hired;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private RepositoryItemButtonEdit rep_EmpID;

	private ContextMenuStrip contextMenuStrip1;

	private RepositoryItemLookUpEditDefineNew rep_lookTeamRole;

	private GridColumnEx jobVa_JobType;

	private RepositoryItemLookUpEditDefineNew rep_lookJobType;

	private RepositoryItemLookUpEdit rep_lookCurrency;

	private RepositoryItemLookUpEdit rep_lookGender;

	private RepositoryItemLookUpEditDefineNew rep_lookInterviewStatus;

	private RepositoryItemLookUpEditDefineNew rep_lookInteType;

	private RepositoryItemLookUpEdit rep_lookStatus;

	private RepositoryItemDateEdit rep_dateDateInter;

	private RepositoryItemCheckedComboBoxEdit rep_CheckedQualification;

	private RepositoryItemLookUpEdit rep_lookPrimary;

	private RepositoryItemButtonEdit rep_btnTeam;

	private RepositoryItemButtonEdit rep_btnQues;

	private RepositoryItemButtonEdit rep_btnAppInInter;

	private SimpleButtonEx btn_Accept;

	private RepositoryItemCheckedComboBoxEdit rep_checkBenefits;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ContextMenuStrip menu_Jobvacancy;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem duplicateRowToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem1;

	private GridColumnEx Offer_TrialCur;

	private GridColumnEx of_ContCur;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private RepositoryItemLookUpEdit rep_looTrialCur;

	private RepositoryItemLookUpEdit rep_lookContractCur;

	private RepositoryItemLookUpEdit rep_lookHTrialCur;

	private RepositoryItemLookUpEdit rep_lookHContractCur;

	private RepositoryItemButtonEdit rep_btnappHired;

	private RepositoryItemCheckedComboBoxEdit rep_checkBeneHired;

	private GridControlEx grd_Applications;

	private GridViewEx gv_Applications;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemButtonEdit r_appInIter;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemLookUpEdit r_Gender2;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemCheckedComboBoxEdit r_Qualification2;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private SplitContainerControl splitContainerControl2;

	private GridColumnEx gridColumnEx11;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem addRowToolStripMenuItem1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private GridColumnEx gridColumnEx12;

	private RepositoryItemCheckedComboBoxEdit r_checkTeam;

	private GridColumnEx gridColumnEx13;

	private RepositoryItemLookUpEdit luk_StatusApp;

	private GridColumnEx gridColumnEx14;

	private RepositoryItemLookUpEdit luk_StatusOff;

	private GridColumnEx gridColumnEx15;

	private RepositoryItemLookUpEdit luk_StatusHired;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx Of_ApplicantNo;

	private RepositoryItemButtonEdit btn_AppInOffer;

	private RepositoryItemLookUpEditDefineNew rep_lookAdvSource;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice2;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice3;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice4;

	private RepositoryItemLookUpEditSimple luk_StatusApplican;

	private RepositoryItemButtonEditCFL btn_AppNo;

	private LookUpEditEx look_Status;

	private GridControlEx grd_Qualification;

	private GridViewEx gv_Qualification;

	private GridColumnEx Qua_DocEntry;

	private GridColumnEx QuaCode;

	private RepositoryItemButtonEdit rep_btnQualificationCode;

	private GridColumnEx QuaGroup;

	private GridColumnEx Qua_Description;

	private GridColumnEx Qu_Name;

	private GridColumnEx Qua_Line;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
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
				btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
				break;
			case Enums.FormViewModeType.Find:
				btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonFind;
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonOK;
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_JobVacancy()
	{
		dtApplicantInterview = new DataTable();
		dtApplicantInterview.Columns.Add("DocEntry", typeof(int));
		dtApplicantInterview.Columns.Add("LineInterview", typeof(int));
		dtApplicantInterview.Columns.Add("LineNum", typeof(int));
		dtApplicantInterview.Columns.Add("ApplicantNo", typeof(string));
		dtApplicantInterview.Columns.Add("ApplicantName", typeof(string));
		dtApplicantInterview.Columns.Add("DoB", typeof(DateTime));
		dtApplicantInterview.Columns.Add("Gender", typeof(int));
		dtApplicantInterview.Columns.Add("Qualifications", typeof(string));
		dtApplicantInterview.Columns.Add("Address", typeof(string));
		dtApplicantInterview.Columns.Add("Phone", typeof(string));
		dtApplicantInterview.Columns.Add("Email", typeof(string));
		dtApplicantInterview.Columns.Add("Status", typeof(string));
		InitializeComponent();
		OnLoadAsync += frm_JobVacancy_Load;
		rep_lookInteType.DisplayMember = "Name";
		rep_lookInteType.ValueMember = "Id";
		rep_lookInteType.DefineNewForm = typeof(frm_JVInterviewTypes);
		rep_lookInteType.RefreshMethod = LoadLookupInteType;
		rep_lookAdvSource.DisplayMember = "Name";
		rep_lookAdvSource.ValueMember = "Id";
		rep_lookAdvSource.DefineNewForm = typeof(frm_AdvertisementSource);
		rep_lookAdvSource.RefreshMethod = LoadLookupAdvSource;
		rep_lookTeamRole.ValueMember = "typeId";
		rep_lookTeamRole.DisplayMember = "Name";
		rep_lookTeamRole.DefineNewForm = typeof(frm_HRRolesSetup);
		rep_lookTeamRole.RefreshMethod = LoadLookkUpTeamRoles;
		gv_JobVacancy.ChangeLogType = Enums.ChangeLog.clHumanresourcesJobVacacy;
		gv_JobVacancy.IsApzTable = true;
	}

	private async Task frm_JobVacancy_Load(object sender, EventArgs e)
	{
		_ = 5;
		try
		{
			FormMode = Enums.FormViewModeType.View;
			SetDataStaus();
			LoadLookupPosition();
			LoadLookupDepartment();
			LoadLookupBranches();
			SetDataCurrency();
			SetDataGender();
			rep_lookStatus.SetDataSource(new Dictionary<string, string>
			{
				{ "P", "Pending" },
				{ "O", "Open" },
				{ "C", "Cancel" },
				{ "F", "Finished" }
			});
			luk_StatusApp.SetDataSource(new Dictionary<string, string>
			{
				{ "A", "Accepted" },
				{ "R", "Rejected" }
			});
			luk_StatusOff.SetDataSource(new Dictionary<string, string>
			{
				{ "A", "Accepted" },
				{ "R", "Rejected" }
			});
			luk_StatusHired.SetDataSource(new Dictionary<string, string>
			{
				{ "A", "Accepted" },
				{ "R", "Rejected" },
				{ "H", "Hired" }
			});
			LoadLookupAdvSource();
			LoadLookupInteType();
			SetDataPrimary();
			LoadLookkUpTeamRoles();
			btn_AppNo.ValueMember = "ApplicantNo";
			btn_AppNo.DisplayMember = "ApplicantNo";
			btn_AppNo.AutoLoadDataSource = false;
			btn_AppNo.FindCFLWith(Enums.TypeCFL.GetApplicantList, true, "", Enums.ItemObjectType.NoItemType, null);
			btn_AppNo.TypeDrillDown = ApzObjectTypes.oApplicants;
			btn_AppNo.HandleChooseItemMulti = HandleChooseItemMulti;
			rep_lookJobType.ValueMember = "Id";
			rep_lookJobType.DisplayMember = "Name";
			rep_lookJobType.FormProperties.TableName = "APZ_OHJT";
			await rep_lookJobType.RefreshDataSourceAsync();
			luk_StatusApplican.SetDataSource(new Dictionary<string, string>
			{
				{ "1", "Accpeted" },
				{ "2", "Not Accepted" }
			});
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx> { ["APZ_HJV6"] = gv_Hired };
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_JobVacancy, "APZ_OHJV"), WinFuntions.LoadUdfColumns(gv_Qualification, "APZ_HJV7"), WinFuntions.LoadUdfColumns(gv_Hiring, "APZ_HJV1"), WinFuntions.LoadUdfColumns(gv_Advertisement, "APZ_HJV2"), WinFuntions.LoadUdfColumns(gv_App, "APZ_HJV3"), WinFuntions.LoadUdfColumns(gv_Inte, "APZ_HJV4"), WinFuntions.LoadUdfColumns(gv_Applications, "APZ_HJV8"), WinFuntions.LoadUdfColumns(gv_Offer, "APZ_HJV5"), WinFuntions.LoadUdfColumns(gv_Hired, "APZ_HJV6", tblFormular));
			date_FromDate.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_ToDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			string startFrom = (Function.ParseDateTime(date_FromDate.EditValue).HasValue ? ((DateTime)date_FromDate.EditValue).ToString("yyyyMMdd") : "");
			string startTo = (Function.ParseDateTime(date_ToDate.EditValue).HasValue ? ((DateTime)date_ToDate.EditValue).ToString("yyyyMMdd") : "");
			duplicateRowToolStripMenuItem.Visible = false;
			await SetDataNo();
			await LoadQua();
			await LoadBenefits();
			btn_Accept.Visible = false;
			btn_Hired.Visible = false;
			DataTable dataTable = await GetData("", 0, startFrom, startTo);
			grd_JobVacancy.DataSource = dataTable;
			grd_JobVacancy.DataSourceTable().AcceptChanges();
			if (!dataTable.IsNotNull())
			{
				gv_JobVacancy_FocusedRowObjectChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			string status = Function.ToString(look_Status.EditValue);
			int docNo = Function.ParseInt(txt_DocNo.Text);
			string fromdate = (Function.ParseDateTime(date_FromDate.EditValue).HasValue ? ((DateTime)date_FromDate.EditValue).ToString("yyyyMMdd") : "");
			string todate = (Function.ParseDateTime(date_ToDate.EditValue).HasValue ? ((DateTime)date_ToDate.EditValue).ToString("yyyyMMdd") : "");
			DataTable dataSource = await GetData(status, docNo, fromdate, todate);
			if (FormMode == Enums.FormViewModeType.View)
			{
				ClearDataTable();
				grd_JobVacancy.DataSource = dataSource;
			}
			if (success)
			{
				ClearDataTable();
				grd_JobVacancy.DataSource = dataSource;
				return;
			}
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				FormMode = Enums.FormViewModeType.View;
				ClearDataTable();
				grd_JobVacancy.DataSource = dataSource;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void ClearDataTable()
	{
		try
		{
			grd_Qualification.DataSourceTable.Rows.Clear();
			grd_Advertisement.DataSourceTable.Rows.Clear();
			grd_App.DataSourceTable.Rows.Clear();
			grd_Inte.DataSourceTable.Rows.Clear();
			grd_Offer.DataSourceTable.Rows.Clear();
			grd_Hired.DataSourceTable.Rows.Clear();
			grd_Hiring.DataSourceTable.Rows.Clear();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_JobVacancy_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				await GetDetails(Function.ParseInt(gv_JobVacancy.GetFocusedRowCellValue("DocEntry")));
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_JobVacancy_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update)
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				grd_JobVacancy.DataSourceTable.RejectChanges();
				grd_Qualification.DataSourceTable.RejectChanges();
				grd_Hiring.DataSourceTable.RejectChanges();
				grd_Advertisement.DataSourceTable.RejectChanges();
				grd_App.DataSourceTable.RejectChanges();
				grd_Inte.DataSourceTable.RejectChanges();
				grd_Applications.DataSourceTable.RejectChanges();
				grd_Offer.DataSourceTable.RejectChanges();
				grd_Hired.DataSourceTable.RejectChanges();
				dtApplicantInterview.RejectChanges();
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				gv_JobVacancy.MoveBy(e.RowHandle);
			}
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Add();
				break;
			case Enums.FormViewModeType.Update:
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					await Update();
				}
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Inte_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			_ = gv_Inte.FocusedRowHandle;
			DataTable dataSourceTable = grd_Inte.DataSourceTable;
			bool flag = false;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				if (Function.ToString(dataSourceTable.Rows[i]["Status"]) == "O" || Function.ToString(dataSourceTable.Rows[i]["Status"]) == "F")
				{
					flag = true;
					break;
				}
			}
			if (flag && Function.ToString(gv_Inte.GetFocusedRowCellValue("Status")) != "O" && Function.ToString(gv_Inte.GetFocusedRowCellValue("Status")) != "F")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private async void btn_Hired_Click(object sender, EventArgs e)
	{
		try
		{
			int[] selectedRows = gv_Offer.GetSelectedRows();
			DataTable dt = ((!grd_Hired.DataSourceTable.IsNotNull()) ? WinFuntions.GetTableStructure("APZ_HJV6") : grd_Hired.DataSourceTable.Copy());
			int[] array = selectedRows;
			foreach (int rowHandle in array)
			{
				string app = Function.ToString(gv_Offer.GetRowCellValue(rowHandle, "ApplicantNo"));
				if (dt.AsEnumerable().Any((DataRow x) => Function.ToString(x["ApplicantNo"]) == app))
				{
					WinCommons.Error(Enums.MessageCode.None, "ApplicantNo: " + app + " exist in grid Hired.");
					return;
				}
				if (gv_Offer.GetRowCellValue(rowHandle, "Status").Equals("A"))
				{
					DataRow r = gv_Offer.GetDataRow(rowHandle);
					DataRow row = dt.NewRow();
					DataRow dataRow = row;
					dataRow["DocEntry"] = await GetDocEntry();
					row["LineNum"] = 0;
					row["ApplicantNo"] = Function.ToString(r["ApplicantNo"]);
					row["ApplicantName"] = Function.ToString(r["ApplicantName"]);
					row["OfferLetter"] = Function.ToString(r["OfferLetter"]);
					row["TrialPeriodSalary"] = Function.ParseDecimal(r["TrialPeriodSalary"]);
					row["TrialCur"] = Function.ToString(r["TrialCur"]);
					row["ContractSalary"] = Function.ParseDecimal(r["ContractSalary"]);
					row["ContCur"] = Function.ToString(r["ContCur"]);
					row["OtherBenefits"] = Function.ToString(r["OtherBenefits"]);
					row["empId"] = 0;
					row["Status"] = "";
					dt.Rows.Add(row);
					dt.AcceptChanges();
				}
			}
			grd_Hired.DataSource = dt;
			xtra_APZ_HJV6.SelectedTabPage = xtra_Hired;
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Accept_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			int[] selectedRows = gv_Hired.GetSelectedRows();
			Function.ParseInt(gv_JobVacancy.GetFocusedRowCellDisplayText("Qty"));
			int[] array = selectedRows;
			foreach (int item in array)
			{
				if (!gv_Hired.GetRowCellValue(item, "Status").Equals("A"))
				{
					continue;
				}
				DataRow r = gv_Hired.GetDataRow(item);
				DataTable tableStructure = WinFuntions.GetTableStructure("OHEM");
				DataRow dataRow = tableStructure.NewRow();
				string text = Function.ToString(r["ApplicantName"]);
				if (string.IsNullOrEmpty(text))
				{
					continue;
				}
				dataRow["empId"] = 0;
				if (!string.IsNullOrEmpty(text))
				{
					string[] array2 = text.Split(' ');
					if (text.Split(' ').Count() > 0)
					{
						dataRow["firstName"] = array2[text.Split(' ').Count() - 1];
						dataRow["lastName"] = text.Replace(array2[text.Split(' ').Count() - 1], "");
					}
				}
				else
				{
					dataRow["lastName"] = "";
					dataRow["middleName"] = "";
					dataRow["firstName"] = "";
				}
				dataRow["jobTitle"] = Function.ToString(gv_JobVacancy.GetFocusedRowCellDisplayText("JobTitle"));
				dataRow["dept"] = Function.ParseInt(gv_JobVacancy.GetFocusedRowCellValue("Department"));
				dataRow["branch"] = Function.ParseInt(gv_JobVacancy.GetFocusedRowCellValue("Branch"));
				DataRow dataRow2 = grd_App.DataSourceTable.Select(string.Format("ApplicantNo = '{0}'", Function.ToString(r["ApplicantNo"]))).FirstOrDefault();
				if (dataRow2 != null)
				{
					if (Function.ParseInt(dataRow2["Gender"]) == 1)
					{
						dataRow["sex"] = "M";
					}
					else if (Function.ParseInt(dataRow2["Gender"]) == 2)
					{
						dataRow["sex"] = "F";
					}
					dataRow["email"] = dataRow2["Email"];
					dataRow["birthDate"] = dataRow2["DoB"];
					dataRow["mobile"] = dataRow2["Phone"];
				}
				dataRow["salary"] = Function.ParseDecimal(r["ContractSalary"]);
				dataRow["salaryUnit"] = "M";
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["Active"] = "Y";
				dataRow["LogInstanc"] = "0";
				dataRow["UserSign2"] = GlobalData.UserID;
				dataRow["UpdateDate"] = DateTime.Now.Date;
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
				int num = await SaveEmplMasterData(tableStructure);
				gv_Hired.FocusedRowHandle = item;
				gv_Hired.SetRowCellValue(item, "empId", num);
				gv_Hired.SetRowCellValue(item, "Status", "H");
				grd_Hired.DatasourceTable().AcceptChanges();
				if (Function.ParseInt(r["DocEntry"]) > 0)
				{
					await Update();
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void xtra_JobVacancy_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV7)
		{
			duplicateRowToolStripMenuItem.Visible = false;
			btn_Hired.Visible = false;
			btn_Accept.Visible = false;
			btn_Profile.Visible = false;
		}
		else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV1)
		{
			duplicateRowToolStripMenuItem.Visible = false;
			btn_Hired.Visible = false;
			btn_Accept.Visible = false;
			btn_Profile.Visible = false;
		}
		else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV2)
		{
			duplicateRowToolStripMenuItem.Visible = true;
			btn_Hired.Visible = false;
			btn_Accept.Visible = false;
			btn_Profile.Visible = false;
		}
		else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV3)
		{
			duplicateRowToolStripMenuItem.Visible = false;
			btn_Hired.Visible = false;
			btn_Accept.Visible = false;
			btn_Profile.Visible = true;
		}
		else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV4)
		{
			duplicateRowToolStripMenuItem.Visible = true;
			btn_Hired.Visible = false;
			btn_Accept.Visible = false;
			btn_Profile.Visible = false;
			if (FormMode == Enums.FormViewModeType.View && (from dataRow4 in grd_App.DataSourceTable.AsEnumerable()
				where dataRow4["ApplicantNo"] != DBNull.Value && Function.ParseInt(dataRow4["InterviewStatus"]) == 1
				select dataRow4["ApplicantNo"]).ToList().Count > 0 && grd_Inte.DataSourceTable.Rows.Count > 0)
			{
				return;
			}
			List<DataRow> list = (from t in grd_App.DataSourceTable().AsEnumerable()
				where Function.ParseInt(t["InterviewStatus"]) == 1
				select t).ToList();
			Function.ParseInt(gv_Inte.GetFocusedRowCellValue("LineNum"));
			if (list == null || list.Count <= 0)
			{
				return;
			}
			DataTable dtAppITer = list.CopyToDataTable();
			_dtAppInInter = grd_Applications.DatasourceTable();
			int i;
			for (i = 0; i < dtAppITer.Rows.Count; i++)
			{
				if (!string.IsNullOrEmpty(Function.ToString(dtAppITer.Rows[i]["ApplicantNo"])) && (from t in dtApplicantInterview.AsEnumerable()
					where Function.ToString(t["ApplicantNo"]) == Function.ToString(dtAppITer.Rows[i]["ApplicantNo"]) && Function.ParseInt(t["DocEntry"]) == Function.ParseInt(gv_JobVacancy.GetFocusedRowCellValue("DocEntry")) && Function.ParseInt(t["LineInterview"]) == 1
					select t).ToList().Count() == 0)
				{
					DataRow dataRow = _dtAppInInter.NewRow();
					dataRow["DocEntry"] = Function.ParseInt(gv_JobVacancy.GetFocusedRowCellValue("DocEntry"));
					dataRow["LineInterview"] = 1;
					dataRow["ApplicantNo"] = Function.ToString(dtAppITer.Rows[i]["ApplicantNo"]);
					dataRow["ApplicantName"] = Function.ToString(dtAppITer.Rows[i]["ApplicantName"]);
					dataRow["DoB"] = Function.ParseDateTimes(dtAppITer.Rows[i]["DoB"]);
					dataRow["Gender"] = Function.ParseInt(dtAppITer.Rows[i]["Gender"]);
					dataRow["Qualifications"] = Function.ToString(dtAppITer.Rows[i]["Qualifications"]);
					dataRow["Address"] = Function.ToString(dtAppITer.Rows[i]["Address"]);
					dataRow["Phone"] = Function.ToString(dtAppITer.Rows[i]["Phone"]);
					dataRow["Email"] = Function.ToString(dtAppITer.Rows[i]["Email"]);
					dataRow["LineNum"] = i + 1;
					_dtAppInInter.Rows.Add(dataRow);
				}
			}
			for (int num = 0; num < _dtAppInInter.Rows.Count; num++)
			{
				if (string.IsNullOrEmpty(Function.ToString(_dtAppInInter.Rows[num]["DocEntry"])))
				{
					_dtAppInInter.Rows[num].Delete();
					_dtAppInInter.AcceptChanges();
				}
			}
			(from DataRow t in _dtAppInInter.Rows
				where string.IsNullOrEmpty(Function.ToString(t["ApplicantNo"]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
			_dtAppInInter.AcceptChanges();
			grd_Applications.DataSource = _dtAppInInter;
			AppliantsData();
			DataRow row = (from t in grd_Inte.DatasourceTable().AsEnumerable()
				where Function.ParseInt(t["LineNum"]) == 1
				select t).FirstOrDefault();
			gv_Inte.FocusedRowHandle = gv_Inte.FindRowHandleByDataRow(row);
		}
		else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV5)
		{
			duplicateRowToolStripMenuItem.Visible = false;
			btn_Hired.Visible = true;
			btn_Accept.Visible = false;
			btn_Profile.Visible = false;
			if (FormMode == Enums.FormViewModeType.View && !grd_Inte.DataSourceTable.IsNotNull())
			{
				return;
			}
			_dtInterview = grd_Inte.DatasourceTable();
			EnumerableRowCollection<DataRow> source = from t in grd_App.DataSourceTable().AsEnumerable()
				where Function.ToString(t["ApplicantNo"]) != ""
				select t;
			int maxVisOrder = Function.ParseInt(_dtInterview.Compute("MAX(VisOrder)", ""));
			if (source.Count() == 0)
			{
				WinCommons.Warning(Enums.MessageCode.YouHaveNoApplicant, "");
				return;
			}
			List<int> lstlineId = (from dr in _dtInterview.AsEnumerable()
				where Function.ToString(dr["Status"]) == "F" && Function.ParseInt(dr["VisOrder"]) == maxVisOrder
				select dr.Field<int>("LineNum")).ToList();
			if (lstlineId.Count <= 0)
			{
				return;
			}
			EnumerableRowCollection<DataRow> source2 = from t in dtApplicantInterview.AsEnumerable()
				where lstlineId.Contains(Function.ParseInt(t["LineInterview"])) && Function.ToString(t["Status"]) == "A"
				select t;
			if (source2.Count() <= 0)
			{
				return;
			}
			_dtApplicants = source2.CopyToDataTable();
			DataTable dtOffer = new DataTable();
			dtOffer.Columns.Add("ApplicantNo", typeof(string));
			dtOffer.Columns.Add("ApplicantName", typeof(string));
			for (int num2 = 0; num2 < _dtApplicants.Rows.Count; num2++)
			{
				DataRow dataRow2 = dtOffer.NewRow();
				dataRow2["ApplicantNo"] = _dtApplicants.Rows[num2]["ApplicantNo"];
				dataRow2["ApplicantName"] = _dtApplicants.Rows[num2]["ApplicantName"];
				dtOffer.Rows.Add(dataRow2);
			}
			dtOffer.AcceptChanges();
			for (int num3 = 0; num3 < dtOffer.Rows.Count; num3++)
			{
				for (int num4 = num3 + 1; num4 < dtOffer.Rows.Count; num4++)
				{
					if (dtOffer.Rows[num3]["ApplicantNo"] == dtOffer.Rows[num4]["ApplicantNo"])
					{
						dtOffer.Rows[num4].Delete();
						dtOffer.AcceptChanges();
					}
				}
			}
			DataTable dataSourceTable = grd_Offer.DataSourceTable;
			_ = grd_Applications.DataSourceTable;
			if (dtOffer != null && dtOffer.Rows.Count > 0)
			{
				int i2;
				for (i2 = 0; i2 < dtOffer.Rows.Count; i2++)
				{
					int num5 = Function.ParseInt(dataSourceTable.Compute("MAX(LineNum)", ""));
					if (!string.IsNullOrEmpty(Function.ToString(dtOffer.Rows[i2]["ApplicantNo"])) && (from t in dataSourceTable.AsEnumerable()
						where Function.ToString(t["ApplicantNo"]) == Function.ToString(dtOffer.Rows[i2]["ApplicantNo"])
						select t).Count() == 0)
					{
						DataRow dataRow3 = dataSourceTable.NewRow();
						dataRow3["LineNum"] = num5 + 1;
						dataRow3["ApplicantNo"] = Function.ToString(dtOffer.Rows[i2]["ApplicantNo"]);
						dataRow3["ApplicantName"] = Function.ToString(dtOffer.Rows[i2]["ApplicantName"]);
						dataSourceTable.Rows.Add(dataRow3);
						dataSourceTable.AcceptChanges();
					}
				}
				dataSourceTable.AcceptChanges();
				grd_Offer.DataSource = dataSourceTable;
			}
			DataChanged();
		}
		else if (xtra_APZ_HJV6.SelectedTabPage == xtra_Hired)
		{
			duplicateRowToolStripMenuItem.Visible = false;
			btn_Hired.Visible = false;
			btn_Profile.Visible = false;
		}
	}

	private void gv_Advertisement_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Remark")
			{
				_dtAdvertisement = grd_Advertisement.DataSourceTable;
				if ((from t in _dtAdvertisement.AsEnumerable()
					where string.IsNullOrWhiteSpace(Function.ToString(t["AdvID"]))
					select t).ToList().Count <= 0)
				{
					NewRowAdv();
				}
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_App_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private void gv_Inte_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtInterview = grd_Inte.DataSourceTable;
			int num = Function.ParseInt(gv_Inte.GetRowCellValue(e.RowHandle, "LineNum"));
			Function.ParseInt(gv_Inte.GetRowCellValue(e.RowHandle, "DocEntry"));
			if (e.Column.FieldName == "VisOrder" && (from t in _dtInterview.AsEnumerable()
				where string.IsNullOrWhiteSpace(Function.ToString(t["InterviewID"]))
				select t).ToList().Count <= 0)
			{
				NewRowInter();
			}
			if (e.Column.FieldName == "Status" && Function.ToString(e.Value) == "F")
			{
				DataRow dataRow = gv_Inte.GetDataRow(gv_Inte.FocusedRowHandle + 1);
				if (dataRow != null && !string.IsNullOrEmpty(Function.ToString(dataRow["InterviewID"])) && Function.ParseInt(dataRow["VisOrder"]) > Function.ParseInt(gv_Inte.GetRowCellValue(e.RowHandle, "VisOrder")))
				{
					_dtInterview.Rows[gv_Inte.GetDataSourceRowIndex(e.RowHandle + 1)]["Status"] = "O";
					List<DataRow> list = dtApplicantInterview.Select($"LineInterview = {num} and Status='A'").ToList();
					if (list.Count() > 0)
					{
						int num2 = Function.ParseInt(dtApplicantInterview.Compute("MAX(LineNum)", ""));
						List<DataRow> list2 = dtApplicantInterview.Select($"LineInterview = {num + 1} and Status='A'").ToList();
						List<DataRow> list3 = new List<DataRow>();
						foreach (DataRow item in list)
						{
							foreach (DataRow item2 in list2)
							{
								if (Function.ToString(item2["ApplicantNo"]).Equals(Function.ToString(item["ApplicantNo"])))
								{
									list3.Add(item);
								}
							}
						}
						foreach (DataRow item3 in list3)
						{
							list.Remove(item3);
						}
						for (int num3 = 0; num3 < list.Count; num3++)
						{
							DataRow dataRow2 = dtApplicantInterview.NewRow();
							dataRow2.ItemArray = list[num3].ItemArray.Clone() as object[];
							dataRow2["LineNum"] = num2 + num3 + 1;
							dataRow2["LineInterview"] = num + 1;
							dataRow2["Status"] = "";
							dtApplicantInterview.Rows.Add(dataRow2);
						}
					}
					dtApplicantInterview.AcceptChanges();
				}
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Offer_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "ApplicantNo")
			{
				_dtOffer = grd_Offer.DataSourceTable;
				string text = Function.ToString(e.Value);
				if (_dtOffer.Select($"ApplicantNo = '{Function.ToString(text)}'").Count() > 0)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
					return;
				}
				if (!string.IsNullOrEmpty(text))
				{
					DataTable dataTable = await _httpClient.JobVacancy.GetAppById(text);
					if (dataTable.Rows.Count > 0)
					{
						gv_Offer.SetRowCellValue(e.RowHandle, "ApplicantNo", Function.ToString(dataTable.Rows[0]["ApplicantNo"]));
						gv_Offer.SetRowCellValue(e.RowHandle, "ApplicantName", Function.ToString(dataTable.Rows[0]["ApplicantName"]));
					}
				}
				if ((from t in _dtOffer.AsEnumerable()
					where string.IsNullOrWhiteSpace(Function.ToString(t["ApplicantNo"]))
					select t).ToList().Count <= 0)
				{
					NewRowOffer();
				}
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_JobVacancy_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private void rep_btnTeam_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		FindEmpIdInGvInter();
	}

	private void rep_btnQues_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		FindQueByCFL();
	}

	private void rep_btnappHired_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Hired.GetFocusedRowCellValue("ApplicantNo"));
			if (!string.IsNullOrEmpty(text))
			{
				this.DrillDown(ApzObjectTypes.oApplicants, ApzObjectTypes.oApplicants, text);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_btnQualificationCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				FindQuaByCFL();
			}
			if (e.Button.Index == 1)
			{
				string text = Function.ToString(gv_Qualification.GetFocusedRowCellValue("QuaCode"));
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oQualifications, ApzObjectTypes.oQualifications, text);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_EmpID_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				FindEmpIdByCFL();
			}
			if (e.Button.Index == 1)
			{
				string text = Function.ToString(gv_Hiring.GetFocusedRowCellValue("EmpID"));
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oEmployeesInfo, ApzObjectTypes.oEmployeesInfo, text);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_AppInOffer_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				FindAppNoOffer();
			}
			if (e.Button.Index == 1)
			{
				string text = Function.ToString(gv_Offer.GetFocusedRowCellValue("ApplicantNo"));
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oApplicants, ApzObjectTypes.oApplicants, text);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Profile_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_Qualification.DataSourceTable;
			List<string> list = new List<string>();
			foreach (DataRow item in dataSourceTable.AsEnumerable())
			{
				list.Add(Function.ToString(item["QuaCode"]));
			}
			string qualification = string.Join(";", list.ToArray());
			await ProfileMatchUp(qualification);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookkUpTeamRoles()
	{
		try
		{
			_ = grd_Inte.DataSourceTable;
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadLookkUpRole();
			if (dataTable == null || dataTable.Columns.Count <= 0)
			{
				dataTable = new DataTable();
				dataTable.Columns.Add("typeID", typeof(int));
				dataTable.Columns.Add("name", typeof(string));
			}
			rep_lookTeamRole.DisplayMember = "name";
			rep_lookTeamRole.ValueMember = "typeID";
			rep_lookTeamRole.DataSource = dataTable.Copy();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV7)
			{
				DeleteSelectedRowQua();
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV1)
			{
				DeleteSelectedRowHiring();
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV2)
			{
				DeleteSelectedRowAdv();
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV3)
			{
				DeleteSelectedRowApp();
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV4)
			{
				DeleteSelectedRowInter();
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV5)
			{
				DeleteSelectedRowOffer();
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_Hired)
			{
				DeleteSelectedRowHired();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private async void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (gv_JobVacancy.GetSelectedRows() == null || gv_JobVacancy.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
			else
			{
				int num = Function.ParseInt(gv_JobVacancy.GetFocusedRowCellValue("DocNum"));
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveByDocNum, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, Function.ToString(num)) == DialogResult.Yes)
				{
					int[] selectedRows = gv_JobVacancy.GetSelectedRows();
					string text = "";
					int[] array = selectedRows;
					foreach (int rowHandle in array)
					{
						text = text + gv_JobVacancy.GetRowCellValue(rowHandle, "DocEntry").ToString() + ",";
					}
					if (text.Length > 0)
					{
						text = text.Remove(text.Length - 1);
						string text2 = await _httpClient.JobVacancy.DeleteDocs(text);
						if (string.IsNullOrEmpty(text2))
						{
							WinCommons.Success(Enums.MessageCode.Success, "");
							DataTable dataSource = await GetData("", 0, null, null);
							grd_JobVacancy.DataSource = dataSource;
							grd_Qualification.DataSource = null;
							gv_Qualification.RefreshData();
							grd_Hiring.DataSource = null;
							gv_Hiring.RefreshData();
							grd_Advertisement.DataSource = null;
							gv_Advertisement.RefreshData();
							grd_App.DataSource = null;
							gv_App.RefreshData();
							grd_Inte.DataSource = null;
							gv_Inte.RefreshData();
							grd_Offer.DataSource = null;
							gv_Offer.RefreshData();
							grd_Hired.DataSource = null;
							gv_Hired.RefreshData();
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, text2);
						}
					}
				}
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int ID = Function.ParseInt(gv_JobVacancy.GetRowCellValue(gv_JobVacancy.FocusedRowHandle, "DocEntry"));
			DataTable dataSourceTable = grd_JobVacancy.DataSourceTable;
			DataRow[] array = (from t in dataSourceTable.AsEnumerable()
				where Function.ParseInt(t["DocEntry"]) == ID
				select t).ToArray();
			int num = Function.ParseInt(dataSourceTable.Compute("MAX(DocNum)", ""));
			array[0]["DocNum"] = num;
			if (array != null && array.Length != 0)
			{
				array[0]["Status"] = "P";
				DataTable dataSource = array.CopyToDataTable();
				grd_JobVacancy.DataSource = null;
				grd_JobVacancy.DataSource = dataSource;
				_dtQualifications = grd_Qualification.DataSourceTable.Copy();
				foreach (DataRow item in _dtQualifications.AsEnumerable())
				{
					item["DocEntry"] = 0;
					item["LineNum"] = 0;
				}
				grd_Qualification.DataSource = null;
				grd_Qualification.DataSource = _dtQualifications;
				_dtHiringTeam = grd_Hiring.DataSourceTable.Copy();
				foreach (DataRow item2 in _dtHiringTeam.AsEnumerable())
				{
					item2["DocEntry"] = 0;
					item2["LineNum"] = 0;
				}
				grd_Hiring.DataSource = null;
				grd_Hiring.DataSource = _dtHiringTeam;
				_dtAdvertisement = grd_Hiring.DataSourceTable.Copy();
				foreach (DataRow item3 in _dtAdvertisement.AsEnumerable())
				{
					item3["DocEntry"] = 0;
					item3["LineNum"] = 0;
				}
				grd_Hiring.DataSource = null;
				grd_Hiring.DataSource = _dtAdvertisement;
				_dtApplicants = grd_Advertisement.DataSourceTable.Copy();
				foreach (DataRow item4 in _dtApplicants.AsEnumerable())
				{
					item4["DocEntry"] = 0;
					item4["LineNum"] = 0;
				}
				grd_Advertisement.DataSource = null;
				grd_Advertisement.DataSource = _dtApplicants;
				_dtInterview = grd_Inte.DataSourceTable.Copy();
				foreach (DataRow item5 in _dtInterview.AsEnumerable())
				{
					item5["DocEntry"] = 0;
					item5["LineNum"] = 0;
				}
				grd_Inte.DataSource = null;
				grd_Inte.DataSource = _dtInterview;
			}
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataChanged();
			if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV7)
			{
				NewRowQua();
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV1)
			{
				NewRowHiring();
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV2)
			{
				NewRowAdv();
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV3)
			{
				NewRowApp();
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV4)
			{
				NewRowInter();
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV5)
			{
				NewRowOffer();
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_Hired)
			{
				NewRowHired();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void duplicateRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV7)
			{
				duplicateRowToolStripMenuItem.Visible = false;
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV1)
			{
				duplicateRowToolStripMenuItem.Visible = false;
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV2)
			{
				duplicateRowToolStripMenuItem.Visible = true;
				int advID = Function.ParseInt(gv_Advertisement.GetRowCellValue(gv_Advertisement.FocusedRowHandle, "AdvID"));
				DataTable dataSourceTable = grd_Advertisement.DataSourceTable;
				DataRow[] array = (from t in dataSourceTable.AsEnumerable()
					where Function.ParseInt(t["AdvID"]) == advID
					select t).ToArray();
				int num = Function.ParseInt(dataSourceTable.Compute("MAX(AdvID)", ""));
				if (array != null && array.Length != 0)
				{
					for (int num2 = 0; num2 < array.Count(); num2++)
					{
						DataRow dataRow = dataSourceTable.NewRow();
						int num3 = num2 + 1;
						dataRow.ItemArray = array[num2].ItemArray;
						dataRow["AdvID"] = num + num3;
						dataSourceTable.Rows.Add(dataRow);
					}
					grd_Advertisement.DataSource = dataSourceTable;
				}
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV3)
			{
				duplicateRowToolStripMenuItem.Visible = false;
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV4)
			{
				duplicateRowToolStripMenuItem.Visible = true;
				int advID2 = Function.ParseInt(gv_Inte.GetRowCellValue(gv_Advertisement.FocusedRowHandle, "InterviewID"));
				DataTable dataSourceTable2 = grd_Inte.DataSourceTable;
				DataRow[] array2 = (from t in dataSourceTable2.AsEnumerable()
					where Function.ParseInt(t["InterviewID"]) == advID2
					select t).ToArray();
				int num4 = Function.ParseInt(dataSourceTable2.Compute("MAX(InterviewID)", ""));
				if (array2 != null && array2.Length != 0)
				{
					for (int num5 = 0; num5 < array2.Count(); num5++)
					{
						DataRow dataRow2 = dataSourceTable2.NewRow();
						int num6 = num5++;
						dataRow2.ItemArray = array2[num5].ItemArray;
						dataRow2["InterviewID"] = num4 + num6;
						dataSourceTable2.Rows.Add(dataRow2);
					}
					grd_Inte.DataSource = dataSourceTable2;
				}
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_APZ_HJV5)
			{
				duplicateRowToolStripMenuItem.Visible = false;
			}
			else if (xtra_APZ_HJV6.SelectedTabPage == xtra_Hired)
			{
				duplicateRowToolStripMenuItem.Visible = false;
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Inte.GetFocusedRowCellValue("LineNum"));
			int num2 = Function.ParseInt(gv_Inte.GetFocusedRowCellValue("DocEntry"));
			_dtAppInInter = grd_Applications.DataSourceTable;
			DataRow dataRow = _dtAppInInter.NewRow();
			dataRow["DocEntry"] = num2;
			dataRow["LineInterview"] = num;
			_dtAppInInter.Rows.Add(dataRow);
			_dtAppInInter.AcceptChanges();
			grd_Applications.DataSource = _dtAppInInter;
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			_dtOffer = grd_Offer.DataSourceTable;
			_dtAppInInter = grd_Applications.DataSourceTable;
			if (gv_Applications.DataRowCount <= 0)
			{
				return;
			}
			string value = Function.ToString(gv_Applications.GetFocusedRowCellValue("ApplicantNo"));
			for (int i = 0; i < _dtOffer.Rows.Count; i++)
			{
				if (Function.ToString(_dtOffer.Rows[i]["ApplicantNo"]).Equals(value))
				{
					WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
					return;
				}
			}
			int num = Function.ParseInt(gv_Applications.GetFocusedRowCellValue("LineInterview"));
			DataRow row = dtApplicantInterview.Select($"LineInterview = {num}").FirstOrDefault();
			dtApplicantInterview.Rows.Remove(row);
			_dtAppInInter.Rows.RemoveAt(gv_Applications.FocusedRowHandle);
			_dtAppInInter.AcceptChanges();
			grd_Applications.DataSource = _dtAppInInter;
			grd_Applications.RefreshDataSource();
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Inte_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_ = 1;
		try
		{
			if (gv_Inte.IsInsertNewRow)
			{
				return;
			}
			int lineNum = Function.ParseInt(gv_Inte.GetRowCellValue(e.RowHandle, "LineNum"));
			int docEntry = Function.ParseInt(gv_Inte.GetRowCellValue(e.RowHandle, "DocEntry"));
			if (lineNum > 0)
			{
				DataTable dataTable = await GetAppByLineNumInter(lineNum, docEntry);
				if (dataTable.Rows.Count > 0)
				{
					grd_Applications.DataSource = dataTable;
					return;
				}
				_dtAppInInter = grd_Applications.DataSourceTable;
				DataRow row = _dtAppInInter.NewRow();
				DataRow dataRow = row;
				dataRow["DocEntry"] = await GetMaxDocNum();
				row["LineInterview"] = lineNum;
				_dtAppInInter.Rows.Add(row);
				grd_Applications.DataSource = _dtAppInInter;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Inte_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtInterview = grd_Inte.DataSourceTable;
			_dtAppInInter = grd_Applications.DataSourceTable;
			Function.ParseInt(gv_Inte.GetRowCellValue(e.RowHandle, "LineNum"));
			Function.ParseInt(gv_Inte.GetRowCellValue(e.RowHandle, "DocEntry"));
			if (e.Column.FieldName == "VisOrder")
			{
				int rowHandle = e.RowHandle;
				Function.ParseInt(gv_Inte.GetRowCellValue(rowHandle - 1, "VisOrder"));
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Applications_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Applications.GetRowCellValue(e.RowHandle, "LineNum"));
			int num2 = Function.ParseInt(gv_Applications.GetRowCellValue(e.RowHandle, "DocEntry"));
			int num3 = Function.ParseInt(gv_Applications.GetFocusedRowCellValue("LineInterview"));
			if (e.Column.FieldName == "Status")
			{
				dtApplicantInterview.Select($"DocEntry={num2} and LineInterview={num3} and LineNum={num}").FirstOrDefault()["Status"] = e.Value;
				dtApplicantInterview.AcceptChanges();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Applications_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Applications.GetRowCellValue(e.RowHandle, "LineNum"));
			int num2 = Function.ParseInt(gv_Applications.GetRowCellValue(e.RowHandle, "DocEntry"));
			int num3 = Function.ParseInt(gv_Applications.GetFocusedRowCellValue("LineInterview"));
			if (!(e.Column.FieldName == "Status") || !Function.ToString(e.Value).Equals("A"))
			{
				return;
			}
			int num4 = Function.ParseInt(dtApplicantInterview.Select($"DocEntry={num2} and LineInterview={num3} and LineNum={num}").FirstOrDefault()["LineInterview"]);
			int num5 = Function.ParseInt(gv_Inte.GetRowCellValue(num4 - 1, "VisOrder"));
			gv_Inte.SetRowCellValue(num4 - 1, "Status", "O");
			for (int i = num4; i < gv_Inte.DataRowCount; i++)
			{
				if (Function.ParseInt(gv_Inte.GetRowCellValue(i, "VisOrder")) > num5)
				{
					gv_Inte.SetRowCellValue(i, "Status", "P");
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void r_appInIter_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				FindAppNoInter();
			}
			if (e.Button.Index == 1)
			{
				string text = Function.ToString(gv_Applications.GetFocusedRowCellValue("ApplicantNo"));
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oApplicants, ApzObjectTypes.oApplicants, text);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetAppByLineNumInter(int lineNum, int docEntry)
	{
		try
		{
			DataTable dt = grd_Applications.DataSourceTable().Clone();
			if (dtApplicantInterview.Rows.Count > 0 && lineNum > 0)
			{
				DataRow[] source = dtApplicantInterview.Select($"DocEntry={docEntry} and LineInterview = {lineNum}");
				if (source.Count() > 0)
				{
					dt = source.CopyToDataTable();
				}
				else
				{
					DataRow row = dt.NewRow();
					DataRow dataRow = row;
					dataRow["DocEntry"] = await GetDocEntry();
					row["LineInterview"] = lineNum;
					dt.Rows.Add(row);
					grd_Applications.DataSource = dt;
				}
			}
			return dt;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private DataTable SetDataGridAppByVisOrder(List<int> lineNums)
	{
		try
		{
			DataTable dataTable = grd_Applications.DataSourceTable;
			if (dtApplicantInterview.Rows.Count > 0 && lineNums.Count > 0)
			{
				for (int i = 0; i < lineNums.Count; i++)
				{
					DataRow[] array = dtApplicantInterview.Select($"LineInterview = {lineNums[i]} and Status='A'");
					if (array.Count() > 0)
					{
						dataTable = array.CopyToDataTable();
					}
					int num = Function.ParseInt(array.Max((DataRow r) => (int)r["LineInterview"]));
					for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
					{
						dataTable.Rows[num2]["LineInterview"] = num + 1;
						array[num2]["LineNum"] = i + 1;
						dtApplicantInterview.ImportRow(dataTable.Rows[num2]);
					}
				}
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void gv_Hired_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gv_Hired.GetFocusedRowCellValue("empId")) > 0)
			{
				gv_Hired.UnselectRow(gv_Hired.FocusedRowHandle);
			}
			if (FormMode != Enums.FormViewModeType.Add)
			{
				btn_Accept.Visible = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Hired_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (e.Column.FieldName == "empId")
		{
			int num = Function.ParseInt(gv_Hired.GetRowCellValue(e.RowHandle, "empId"));
			int rowHandle = e.RowHandle;
			if (num > 0)
			{
				gv_Hired.SetRowCellValue(rowHandle, "Status", "H");
			}
			DataChanged();
		}
	}

	private void gv_JobVacancy_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetFocusedRowCellValue("Status")), "C"))
			{
				gv_JobVacancy.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_JobVacancy_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetRowCellValue(e.RowHandle, "Status")), "C"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Advertisement_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetRowCellValue(e.RowHandle, "Status")), "C"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Advertisement_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetFocusedRowCellValue("Status")), "C"))
			{
				gv_Advertisement.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_App_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetRowCellValue(e.RowHandle, "Status")), "C"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_App_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetFocusedRowCellValue("Status")), "C"))
			{
				gv_App.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Inte_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetRowCellValue(e.RowHandle, "Status")), "C"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Offer_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetFocusedRowCellValue("Status")), "C"))
			{
				gv_Offer.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Offer_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetRowCellValue(e.RowHandle, "Status")), "C"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Hired_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetRowCellValue(e.RowHandle, "Status")), "C"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Hired_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetFocusedRowCellValue("Status")), "C"))
			{
				gv_Hired.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Qualification_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetFocusedRowCellValue("Status")), "C"))
			{
				gv_Qualification.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Qualification_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetRowCellValue(e.RowHandle, "Status")), "C"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Hiring_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetRowCellValue(e.RowHandle, "Status")), "C"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Hiring_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetFocusedRowCellValue("Status")), "C"))
			{
				gv_Hiring.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Applications_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetFocusedRowCellValue("Status")), "C"))
			{
				gv_Applications.FocusedColumn.OptionsColumn.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Applications_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (string.Equals(Function.ToString(gv_JobVacancy.GetRowCellValue(e.RowHandle, "Status")), "C"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_App_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "ApplicantNo")
			{
				_dtApplicants = grd_App.DataSourceTable;
				string id = Function.ToString(e.Value);
				DataRow dataRow = (from t in btn_AppNo.DataSource.AsEnumerable()
					where Function.ToString(t["ApplicantNo"]) == id
					select t).FirstOrDefault();
				if (_dtApplicants.Select($"ApplicantNo = '{Function.ToString(id)}'").Count() > 1)
				{
					WinCommons.Warning(Enums.MessageCode.ApplicantNoExistsInGrid, "", id);
					return;
				}
				if (Function.ParseInt(gv_App.GetRowCellValue(e.RowHandle, "InterviewStatus")) == 1)
				{
					string olValue = Function.ToString(gv_App.ActiveEditor.OldEditValue);
					if (!string.IsNullOrEmpty(olValue))
					{
						DataRow dataRow2 = (from t in dtApplicantInterview.AsEnumerable()
							where Function.ToString(t["ApplicantNo"]) == olValue && Function.ParseInt(t["DocEntry"]) == Function.ParseInt(gv_JobVacancy.GetFocusedRowCellValue("DocEntry")) && Function.ParseInt(t["LineInterview"]) == 1
							select t).FirstOrDefault();
						if (dataRow2 != null)
						{
							if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmChangeApplicant, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, olValue) != DialogResult.Yes)
							{
								return;
							}
							dataRow2["ApplicantNo"] = id;
							dataRow2["ApplicantName"] = Function.ToString(dataRow["ApplicantName"]);
							dataRow2["DoB"] = Function.ParseDateTimes(dataRow["DoB"]);
							dataRow2["Gender"] = Function.ParseInt(dataRow["Gender"]);
							dataRow2["Qualifications"] = Function.ToString(dataRow["Qualifications"]);
							dataRow2["Address"] = Function.ToString(dataRow["Address"]);
							dataRow2["Phone"] = Function.ToString(dataRow["Phone"]);
							dataRow2["Email"] = Function.ToString(dataRow["Email"]);
						}
					}
				}
				int num = Function.ParseInt(_dtApplicants.Compute("Max(LineNum)", ""));
				if (dataRow != null)
				{
					gv_App.SetRowCellValue(e.RowHandle, "ApplicantName", Function.ToString(dataRow["ApplicantName"]));
					gv_App.SetRowCellValue(e.RowHandle, "DoB", Function.ParseDateTimes(dataRow["DoB"]));
					gv_App.SetRowCellValue(e.RowHandle, "Gender", Function.ParseInt(dataRow["Gender"]));
					gv_App.SetRowCellValue(e.RowHandle, "Qualifications", Function.ToString(dataRow["Qualifications"]));
					gv_App.SetRowCellValue(e.RowHandle, "Address", Function.ToString(dataRow["Address"]));
					gv_App.SetRowCellValue(e.RowHandle, "Phone", Function.ToString(dataRow["Phone"]));
					gv_App.SetRowCellValue(e.RowHandle, "Email", Function.ToString(dataRow["Email"]));
					gv_App.SetRowCellValue(e.RowHandle, "LineNum", num + 1);
				}
			}
			if (!(e.Column.FieldName == "InterviewStatus"))
			{
				return;
			}
			int num2 = Function.ParseInt(e.Value);
			string appNo = Function.ToString(gv_App.GetRowCellValue(e.RowHandle, "ApplicantNo"));
			if (string.IsNullOrEmpty(appNo))
			{
				return;
			}
			DataRow dataRow3 = (from t in dtApplicantInterview.AsEnumerable()
				where Function.ToString(t["ApplicantNo"]) == appNo && Function.ParseInt(t["DocEntry"]) == Function.ParseInt(gv_JobVacancy.GetFocusedRowCellValue("DocEntry")) && Function.ParseInt(t["LineInterview"]) == 1
				select t).FirstOrDefault();
			switch (num2)
			{
			case 1:
				if (dataRow3 != null)
				{
					dataRow3["ApplicantName"] = Function.ToString(gv_App.GetRowCellValue(e.RowHandle, "ApplicantName"));
					dataRow3["DoB"] = Function.ParseDateTimes(gv_App.GetRowCellValue(e.RowHandle, "DoB"));
					dataRow3["Gender"] = Function.ParseInt(gv_App.GetRowCellValue(e.RowHandle, "Gender"));
					dataRow3["Qualifications"] = Function.ToString(gv_App.GetRowCellValue(e.RowHandle, "Qualifications"));
					dataRow3["Address"] = Function.ToString(gv_App.GetRowCellValue(e.RowHandle, "Address"));
					dataRow3["Phone"] = Function.ToString(gv_App.GetRowCellValue(e.RowHandle, "Phone"));
					dataRow3["Email"] = Function.ToString(gv_App.GetRowCellValue(e.RowHandle, "Email"));
				}
				break;
			case 2:
				if (dataRow3 != null)
				{
					WinCommons.Warning(Enums.MessageCode.ApplicantNoExistsInTabInterviewHouHaveRemoveApplicentNoBeforeChangeStatus, "", appNo);
					_dtApplicants.Rows[gv_App.GetDataSourceRowIndex(e.RowHandle)]["InterviewStatus"] = 1;
					FormMode = Enums.FormViewModeType.View;
				}
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Inte_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		AppliantsData();
	}

	private void gv_Hired_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private void DataChanged()
	{
		try
		{
			if (!_isLoading && FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindDataNextLast(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		_ = 1;
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			ClearDataTable();
			grd_Applications.DatasourceTable().Rows.Clear();
			dtApplicantInterview.Rows.Clear();
			_dtJobVacancy = grd_JobVacancy.DataSourceTable.Clone();
			if (_dtJobVacancy == null || _dtJobVacancy.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow row = _dtJobVacancy.NewRow();
			if (luk_series.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				row["Series"] = dataTable.Rows[0]["Series"];
			}
			row["DocNum"] = await GetNextNumber(Function.ParseInt(row["Series"]));
			DataRow dataRow = row;
			dataRow["DocEntry"] = await GetDocEntry() + 1;
			_dtJobVacancy.Rows.Add(row);
			grd_JobVacancy.DataSource = _dtJobVacancy;
			NewRowAdv();
			NewRowApp();
			NewRowHiring();
			NewRowInter();
			NewRowOffer();
			NewRowQua();
			NewRowHired();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		_ = 1;
		try
		{
			FormMode = Enums.FormViewModeType.View;
			DataTable dataTable = await GetData("", 0, null, null);
			if (dataTable.IsNotNull())
			{
				grd_JobVacancy.DataSource = dataTable;
				grd_JobVacancy.RefreshDataSource();
				int docId = Function.ParseInt(gv_JobVacancy.GetFocusedRowCellValue("DocEntry"));
				await GetDetails(docId);
			}
			else
			{
				grd_JobVacancy.DataSource = null;
				grd_JobVacancy.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void NewRowQua()
	{
		try
		{
			_dtQualifications = grd_Qualification.DataSourceTable;
			if (_dtQualifications == null || _dtQualifications.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			int num = Function.ParseInt(_dtQualifications.Compute("MAX(LineNum)", ""));
			DataRow dataRow = _dtQualifications.NewRow();
			dataRow["LineNum"] = num + 1;
			_dtQualifications.Rows.Add(dataRow);
			grd_Qualification.DataSource = _dtQualifications;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void NewRowHiring()
	{
		try
		{
			gv_Hiring.BeginInsertRow();
			_dtHiringTeam = grd_Hiring.DataSourceTable;
			if (_dtHiringTeam == null || _dtHiringTeam.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			int num = Function.ParseInt(_dtHiringTeam.Compute("MAX(LineNum)", ""));
			DataRow dataRow = _dtHiringTeam.NewRow();
			dataRow["LineNum"] = num + 1;
			_dtHiringTeam.Rows.Add(dataRow);
			gv_Hiring.EndInsertRow();
			grd_Hiring.DataSource = _dtHiringTeam;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Hiring.EndInsertRow();
	}

	public void NewRowAdv()
	{
		try
		{
			gv_Advertisement.BeginInsertRow();
			_dtAdvertisement = grd_Advertisement.DataSourceTable;
			if (_dtAdvertisement == null || _dtAdvertisement.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			int num = Function.ParseInt(_dtAdvertisement.Compute("MAX(LineNum)", ""));
			DataRow dataRow = _dtAdvertisement.NewRow();
			dataRow["LineNum"] = num + 1;
			_dtAdvertisement.Rows.Add(dataRow);
			gv_Advertisement.EndInsertRow();
			grd_Advertisement.DataSource = _dtAdvertisement;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Advertisement.EndInsertRow();
	}

	public void NewRowApp()
	{
		try
		{
			gv_App.BeginInsertRow();
			_dtApplicants = grd_App.DataSourceTable;
			if (_dtApplicants == null || _dtApplicants.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			int num = Function.ParseInt(_dtApplicants.Compute("MAX(LineNum)", ""));
			DataRow dataRow = _dtApplicants.NewRow();
			dataRow["LineNum"] = num + 1;
			_dtApplicants.Rows.Add(dataRow);
			gv_App.EndInsertRow();
			grd_App.DataSource = _dtApplicants;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		gv_App.EndInsertRow();
	}

	public async void NewRowInter()
	{
		try
		{
			gv_Inte.BeginInsertRow();
			_dtInterview = grd_Inte.DataSourceTable;
			if (_dtInterview == null || _dtInterview.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			int maxLineNum = Function.ParseInt(_dtInterview.Compute("MAX(LineNum)", ""));
			DataRow row5 = _dtInterview.NewRow();
			if (FormMode == Enums.FormViewModeType.Add)
			{
				DataRow dataRow = row5;
				dataRow["DocEntry"] = await GetDocEntry() + 1;
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				row5["DocEntry"] = gv_JobVacancy.GetFocusedRowCellValue("DocEntry");
			}
			row5["Status"] = "P";
			row5["LineNum"] = maxLineNum + 1;
			_dtInterview.Rows.Add(row5);
			gv_Inte.EndInsertRow();
			grd_Inte.DataSource = _dtInterview;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Inte.EndInsertRow();
	}

	public async void NewRowOffer()
	{
		try
		{
			gv_Offer.BeginInsertRow();
			_dtOffer = grd_Offer.DataSourceTable;
			if (_dtOffer == null || _dtOffer.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			int maxLineNum = Function.ParseInt(_dtOffer.Compute("MAX(LineNum)", ""));
			DataRow row6 = _dtOffer.NewRow();
			if (FormMode == Enums.FormViewModeType.Add)
			{
				DataRow dataRow = row6;
				dataRow["DocEntry"] = await GetDocEntry() + 1;
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				row6["DocEntry"] = gv_JobVacancy.GetFocusedRowCellValue("DocEntry");
			}
			row6["LineNum"] = maxLineNum + 1;
			_dtOffer.Rows.Add(row6);
			gv_Offer.EndInsertRow();
			grd_Offer.DataSource = _dtOffer;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Offer.EndInsertRow();
	}

	public async void NewRowHired()
	{
		try
		{
			gv_Hired.BeginInsertRow();
			_dtHired = grd_Hired.DataSourceTable;
			if (_dtHired == null || _dtHired.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			int maxLineNum = Function.ParseInt(_dtHired.Compute("MAX(LineNum)", ""));
			DataRow row6 = _dtHired.NewRow();
			if (FormMode == Enums.FormViewModeType.Add)
			{
				DataRow dataRow = row6;
				dataRow["DocEntry"] = await GetDocEntry() + 1;
			}
			else if (FormMode == Enums.FormViewModeType.Update)
			{
				row6["DocEntry"] = gv_JobVacancy.GetFocusedRowCellValue("DocEntry");
			}
			row6["LineNum"] = maxLineNum + 1;
			_dtHired.Rows.Add(row6);
			gv_Hired.EndInsertRow();
			grd_Hired.DataSource = _dtHired;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Hired.EndInsertRow();
	}

	public async Task<int> FindDataNextLast(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.JobVacancy.FindDataNextLast(Function.ParseInt(gv_JobVacancy.GetFocusedRowCellValue("DocEntry")), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_JobVacancy.DataSource = dataTable;
				int docId = Function.ParseInt(dataTable.Rows[0]["DocEntry"]);
				await GetDetails(docId);
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async Task<int> Add()
	{
		try
		{
			isSave = true;
			DataTable dataTable = grd_JobVacancy.DataSourceTable.Copy();
			dataTable.AcceptChanges();
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				if (!(Function.ToString(item["FromDate"]) != "") || !(Function.ToString(item["ToDate"]) != ""))
				{
					if (dataTable.Rows.IndexOf(item) != dataTable.Rows.Count - 1)
					{
						WinCommons.Error(Enums.MessageCode.FromDateOrToDateCannotBeNull, "");
						return 1;
					}
					if (Function.ToString(item["FromDate"]) != "" || Function.ToString(item["ToDate"]) != "" || Function.ToString(item["Description"]) != "" || Function.ToString(item["Status"]) != "" || Function.ToString(item["JobTitle"]) != "")
					{
						WinCommons.Error(Enums.MessageCode.FromDateOrToDateCannotBeNull, "");
						return 1;
					}
				}
			}
			AppliantsData();
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataSet dataSet = new DataSet();
				_dtJobVacancy = grd_JobVacancy.DataSourceTable.Copy();
				CheckRowNull(_dtJobVacancy, "DocNum");
				_dtJobVacancy.AcceptChanges();
				TableSave();
				dataSet.Tables.Add(_dtJobVacancy.Copy());
				dataSet.Tables.Add(_dtQualifications.Copy());
				dataSet.Tables.Add(_dtHiringTeam.Copy());
				dataSet.Tables.Add(_dtAdvertisement.Copy());
				dataSet.Tables.Add(_dtApplicants.Copy());
				dataSet.Tables.Add(_dtInterview.Copy());
				dataSet.Tables.Add(_dtOffer.Copy());
				dataSet.Tables.Add(_dtHired.Copy());
				dataSet.Tables.Add(_dtAppInInter.Copy());
				HttpResult httpResult = await _httpClient.JobVacancy.Add(dataSet);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					success = true;
					xtra_APZ_HJV6.SelectedTabPage = xtra_Hired;
					btn_Accept.Visible = true;
					btn_Search_Click(null, null);
					FormMode = Enums.FormViewModeType.View;
					success = false;
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return 1;
		}
		WinCommons._CloseProcessing();
		isSave = false;
		return 1;
	}

	private new async Task<int> Update()
	{
		try
		{
			isSave = true;
			DataTable dataTable = grd_JobVacancy.DataSourceTable.Copy();
			dataTable.AcceptChanges();
			foreach (DataRow item in dataTable.AsEnumerable())
			{
				if (!(Function.ToString(item["FromDate"]) != "") || !(Function.ToString(item["ToDate"]) != ""))
				{
					if (dataTable.Rows.IndexOf(item) != dataTable.Rows.Count - 1)
					{
						WinCommons.Error(Enums.MessageCode.FromDateOrToDateCannotBeNull, "");
						return 1;
					}
					if (Function.ToString(item["FromDate"]) != "" || Function.ToString(item["ToDate"]) != "" || Function.ToString(item["Description"]) != "" || Function.ToString(item["Status"]) != "" || Function.ToString(item["JobTitle"]) != "")
					{
						WinCommons.Error(Enums.MessageCode.FromDateOrToDateCannotBeNull, "");
						return 1;
					}
				}
			}
			AppliantsData();
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			DataRow[] array = dataTable.Select("DocEntry=" + Function.ParseInt(gv_JobVacancy.GetFocusedRowCellValue("DocEntry")));
			if (array.Length != 0)
			{
				_dtJobVacancy = array.CopyToDataTable();
				CheckRowNull(_dtJobVacancy, "DocNum");
			}
			TableSave();
			dataSet.Tables.Add(_dtJobVacancy.Copy());
			dataSet.Tables.Add(_dtQualifications.Copy());
			dataSet.Tables.Add(_dtHiringTeam.Copy());
			dataSet.Tables.Add(_dtAdvertisement.Copy());
			dataSet.Tables.Add(_dtApplicants.Copy());
			dataSet.Tables.Add(_dtInterview.Copy());
			dataSet.Tables.Add(_dtOffer.Copy());
			dataSet.Tables.Add(_dtHired.Copy());
			dataSet.Tables.Add(_dtAppInInter.Copy());
			HttpResult httpResult = await _httpClient.JobVacancy.Update(dataSet);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.UpdateSuccess, "");
				success = true;
				xtra_APZ_HJV6.SelectedTabPage = xtra_Hired;
				btn_Accept.Visible = true;
				grd_JobVacancy.DatasourceTable().AcceptChanges();
				grd_Qualification.DatasourceTable().AcceptChanges();
				grd_Hiring.DatasourceTable().AcceptChanges();
				grd_Advertisement.DatasourceTable().AcceptChanges();
				grd_App.DatasourceTable().AcceptChanges();
				grd_Inte.DatasourceTable().AcceptChanges();
				grd_Applications.DatasourceTable().AcceptChanges();
				grd_Offer.DatasourceTable().AcceptChanges();
				grd_Hired.DatasourceTable().AcceptChanges();
				FormMode = Enums.FormViewModeType.View;
				success = false;
				WinCommons._CloseProcessing();
				return 0;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		isSave = false;
		return 1;
	}

	public void AppliantsData()
	{
		try
		{
			DataTable dataSourceTable = grd_Applications.DataSourceTable;
			if (dataSourceTable != null && dataSourceTable.Rows.Count > 0)
			{
				DataRow[] array = dtApplicantInterview.Select(string.Format("LineInterview = {0} ", Function.ParseInt(dataSourceTable.Rows[0]["LineInterview"])));
				for (int i = 0; i < array.Length; i++)
				{
					dtApplicantInterview.Rows.Remove(array[i]);
				}
				for (int j = 0; j < dataSourceTable.Rows.Count; j++)
				{
					DataRow dataRow = dtApplicantInterview.NewRow();
					dataRow.ItemArray = dataSourceTable.Rows[j].ItemArray;
					dtApplicantInterview.Rows.Add(dataRow);
				}
				dtApplicantInterview.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void TableSave()
	{
		try
		{
			if (grd_Qualification.DataSource != null)
			{
				_dtQualifications = grd_Qualification.DataSourceTable.Copy();
				CheckRowNull(_dtQualifications, "QuaCode");
				_dtQualifications.AcceptChanges();
			}
			if (grd_Hiring.DataSource != null)
			{
				_dtHiringTeam = grd_Hiring.DataSourceTable.Copy();
				CheckRowNull(_dtHiringTeam, "EmpID");
				_dtHiringTeam.AcceptChanges();
			}
			if (grd_Advertisement.DataSource != null)
			{
				_dtAdvertisement = grd_Advertisement.DataSourceTable.Copy();
				CheckRowNull(_dtAdvertisement, "AdvId");
				_dtAdvertisement.AcceptChanges();
			}
			if (grd_App.DataSource != null)
			{
				_dtApplicants = grd_App.DataSourceTable.Copy();
				CheckRowNull(_dtApplicants, "ApplicantNo");
				_dtApplicants.AcceptChanges();
			}
			if (grd_Inte.DataSource != null)
			{
				_dtInterview = grd_Inte.DataSourceTable.Copy();
				CheckRowNull(_dtInterview, "InterviewID");
				_dtInterview.AcceptChanges();
			}
			if (grd_Offer.DataSource != null)
			{
				_dtOffer = grd_Offer.DataSourceTable.Copy();
				CheckRowNull(_dtOffer, "ApplicantNo");
				_dtOffer.AcceptChanges();
			}
			if (grd_Hired.DataSource != null)
			{
				_dtHired = grd_Hired.DataSourceTable.Copy();
				CheckRowNull(_dtHired, "ApplicantNo");
				_dtHired.AcceptChanges();
			}
			if (_dtInterview != null && _dtInterview.Rows.Count >= 1)
			{
				(from DataRow t in dtApplicantInterview.Rows
					where string.IsNullOrEmpty(Function.ToString(t["ApplicantNo"]))
					select t).ToList().ForEach((DataRow r) =>
				{
					r.Delete();
				});
				dtApplicantInterview.AcceptChanges();
			}
			else
			{
				dtApplicantInterview.Rows.Clear();
			}
			_dtAppInInter = dtApplicantInterview.Copy();
			_dtJobVacancy.TableName = "JobVacancy";
			_dtQualifications.TableName = "Qualifications";
			_dtHiringTeam.TableName = "HiringTeam";
			_dtAdvertisement.TableName = "Advertisement";
			_dtApplicants.TableName = "Applicants";
			_dtInterview.TableName = "Interview";
			_dtOffer.TableName = "Offer";
			_dtHired.TableName = "Hired";
			_dtAppInInter.TableName = "AppInInter";
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> SaveEmplMasterData(DataTable dt)
	{
		try
		{
			WinCommons._OpenProcessing();
			int num = 0;
			if (num < dt.Rows.Count)
			{
				DataSet dataSet = new DataSet();
				DataTable dataTable = dt.Clone();
				dataTable.ImportRow(dt.Rows[num]);
				dataTable.TableName = "dtDoc";
				dataSet.Tables.Add(dataTable);
				HttpResult httpResult = await _httpClient.EmployeeMasterClient.SaveDoc(dataSet);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					int result = Function.ParseInt(httpResult.Content);
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					return result;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				WinCommons._CloseProcessing();
				return 0;
			}
			btn_Accept.Visible = false;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		return 1;
	}

	public async Task<DataTable> GetData(string status, int docNo, string fromdate, string todate)
	{
		try
		{
			return await _httpClient.JobVacancy.GetData(status, docNo, fromdate, todate);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async void LoadLookupPosition()
	{
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboPosition();
			luk_Position.DataSource = dataTable;
			luk_Position.DisplayMember = "name";
			luk_Position.ValueMember = "posID";
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				gv_JobVacancy.SetFocusedRowCellValue("Positions", dataTable.Rows[0]["posID"]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookupDepartment()
	{
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboDept();
			luk_Department.DataSource = dataTable;
			luk_Department.DisplayMember = "Name";
			luk_Department.ValueMember = "Code";
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				gv_JobVacancy.SetFocusedRowCellValue("Department", dataTable.Rows[0]["Code"]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookupBranches()
	{
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			luk_Branch.DataSource = dataTable;
			luk_Branch.DisplayMember = "Name";
			luk_Branch.ValueMember = "Code";
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				gv_JobVacancy.SetFocusedRowCellValue("Branch", dataTable.Rows[0]["Code"]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SetDataStaus()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Pending",
			Code = "P"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Approved",
			Code = "A"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Close",
			Code = "C"
		});
		look_Status.Properties.DataSource = list;
		lok_Status.DataSource = list;
	}

	public void SetDataPrimary()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Yes",
			Code = "Y"
		});
		list.Add(new Common.Table_Code
		{
			Name = "No",
			Code = "N"
		});
		rep_lookPrimary.DataSource = list;
	}

	public void SetDataCurrency()
	{
		try
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
			rep_lookCurrency.DataSource = dataTable.Copy();
			rep_lookCurrency.NullText = "";
			rep_lookCurrency.ValueMember = "CurrCode";
			rep_lookCurrency.DisplayMember = "CurrCode";
			rep_looTrialCur.DataSource = dataTable.Copy();
			rep_looTrialCur.NullText = "";
			rep_looTrialCur.ValueMember = "CurrCode";
			rep_looTrialCur.DisplayMember = "CurrCode";
			rep_lookContractCur.DataSource = dataTable.Copy();
			rep_lookContractCur.NullText = "";
			rep_lookContractCur.ValueMember = "CurrCode";
			rep_lookContractCur.DisplayMember = "CurrCode";
			rep_lookHTrialCur.DataSource = dataTable.Copy();
			rep_lookHTrialCur.NullText = "";
			rep_lookHTrialCur.ValueMember = "CurrCode";
			rep_lookHTrialCur.DisplayMember = "CurrCode";
			rep_lookHContractCur.DataSource = dataTable.Copy();
			rep_lookHContractCur.NullText = "";
			rep_lookHContractCur.ValueMember = "CurrCode";
			rep_lookHContractCur.DisplayMember = "CurrCode";
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SetDataGender()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Male",
			Code = "1"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Female",
			Code = "2"
		});
		rep_lookGender.DataSource = list;
		r_Gender2.DataSource = list;
	}

	private async void LoadLookupAdvSource()
	{
		try
		{
			DataTable dataSource = await _httpClient.JobVacancy.SetDataLookUp("APZ_OHAS");
			rep_lookAdvSource.DataSource = dataSource;
			rep_lookAdvSource.DisplayMember = "Name";
			rep_lookAdvSource.ValueMember = "Id";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void LoadLookupInteType()
	{
		try
		{
			DataTable dataSource = await _httpClient.JobVacancy.SetDataLookUp("APZ_OHIT");
			rep_lookInteType.DataSource = dataSource;
			rep_lookInteType.DisplayMember = "Name";
			rep_lookInteType.ValueMember = "Id";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<int> GetNextNumber(int series)
	{
		try
		{
			return await _httpClient.JobVacancy.GetNextNumber(series);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public async Task SetDataNo()
	{
		DataTable dataTable = await _httpClient.JobVacancy.GetNo();
		luk_series.DataSource = dataTable;
		luk_series.DisplayMember = "SeriesName";
		luk_series.ValueMember = "Series";
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			gv_JobVacancy.SetFocusedRowCellValue("Series", dataTable.Rows[0]["Series"]);
		}
	}

	public async Task GetDetails(int docId)
	{
		try
		{
			_isLoading = true;
			WinCommons._OpenProcessing();
			DataSet dataSet = await _httpClient.JobVacancy.GetDetail(docId);
			if (dataSet.IsNotNull() && dataSet.Tables.Count > 2)
			{
				grd_Qualification.DataSource = dataSet.Tables[0];
				grd_Hiring.DataSource = dataSet.Tables[1];
				grd_Advertisement.DataSource = dataSet.Tables[2];
				grd_App.DataSource = dataSet.Tables[3];
				grd_Applications.DataSource = dataSet.Tables[7];
				dtApplicantInterview = dataSet.Tables[7].Copy();
				grd_Inte.DataSource = dataSet.Tables[4];
				grd_Offer.DataSource = dataSet.Tables[5];
				grd_Hired.DataSource = dataSet.Tables[6];
			}
			WinCommons._CloseProcessing();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task LoadBenefits()
	{
		try
		{
			foreach (DataRow item in (await _httpClient.JobVacancy.ListBenefits()).AsEnumerable())
			{
				rep_checkBenefits.Items.Add(Function.ToString(item["Id"]), Function.ToString(item["Name"]), CheckState.Unchecked, enabled: true);
				rep_checkBeneHired.Items.Add(Function.ToString(item["Id"]), Function.ToString(item["Name"]), CheckState.Unchecked, enabled: true);
			}
			rep_checkBenefits.SeparatorChar = ';';
			gv_Offer.Columns["OtherBenefits"].ColumnEdit = rep_checkBenefits;
			rep_checkBeneHired.SeparatorChar = ';';
			gv_Hired.Columns["OtherBenefits"].ColumnEdit = rep_checkBeneHired;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task LoadQua()
	{
		try
		{
			foreach (DataRow item in (await LoadQualification()).AsEnumerable())
			{
				rep_CheckedQualification.Items.Add(Function.ToString(item["QuaCode"]), Function.ToString(item["Name"]), CheckState.Unchecked, enabled: true);
			}
			rep_CheckedQualification.SeparatorChar = ';';
			gv_App.Columns["Qualifications"].ColumnEdit = rep_CheckedQualification;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataTable> LoadQualification()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "HRMQualifications");
			DataTable dataTable = await helper.GetAsync("GetQualifications");
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OHQU");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public async Task ProfileMatchUp(string qualification)
	{
		try
		{
			DataTable dataTable = await _httpClient.JobVacancy.ProfileMacthUp(qualification);
			if (dataTable != null)
			{
				grd_App.DataSource = dataTable;
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void FindEmpIdByCFL()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.EmployeeListForSalesEmpSetup, true, Function.ToString(gv_Hiring.GetFocusedRowCellValue("EmpID")), "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			int focusedRowHandle = gv_Hiring.FocusedRowHandle;
			gv_Hiring.DeleteRow(gv_Hiring.FocusedRowHandle);
			grd_Hiring.DataSourceTable.AcceptChanges();
			_dtHiringTeam = grd_Hiring.DataSourceTable;
			for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
			{
				if (_dtHiringTeam.Select(string.Format("EmpID = '{0}'", Function.ToString(frm_CFL2.ChooseItems[i]["empID"]))).Count() > 0)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
					return;
				}
				DataRow dataRow = _dtHiringTeam.NewRow();
				dataRow["EmpID"] = Function.ToString(frm_CFL2.ChooseItems[i]["empID"]);
				string value = Function.ToString(frm_CFL2.ChooseItems[i]["lastName"]) + "  " + Function.ToString(frm_CFL2.ChooseItems[i]["firstName"]);
				dataRow["EmpName"] = value;
				_dtHiringTeam.Rows.InsertAt(dataRow, focusedRowHandle);
			}
			grd_Hiring.DataSource = _dtHiringTeam;
			if ((from t in _dtHiringTeam.AsEnumerable()
				where string.IsNullOrWhiteSpace(Function.ToString(t["EmpID"]))
				select t).ToList().Count <= 0)
			{
				NewRowHiring();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void FindQuaByCFL()
	{
		try
		{
			Function.ToString(gv_Qualification.GetFocusedRowCellValue("QuaCode"));
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetQualificationList, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			int focusedRowHandle = gv_Qualification.FocusedRowHandle;
			gv_Qualification.DeleteRow(gv_Qualification.FocusedRowHandle);
			grd_Advertisement.DataSourceTable.AcceptChanges();
			_dtQualifications = grd_Qualification.DataSourceTable;
			for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
			{
				if (_dtQualifications.Select(string.Format("QuaCode = '{0}'", Function.ToString(frm_CFL2.ChooseItems[i]["QuaCode"]))).Count() > 0)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
					return;
				}
				DataRow dataRow = _dtQualifications.NewRow();
				dataRow["QuaCode"] = Function.ToString(frm_CFL2.ChooseItems[i]["QuaCode"]);
				dataRow["Name"] = Function.ToString(frm_CFL2.ChooseItems[i]["Name"]);
				dataRow["QuaGroup"] = Function.ToString(frm_CFL2.ChooseItems[i]["QuaGroup"]);
				dataRow["Description"] = Function.ToString(frm_CFL2.ChooseItems[i]["Description"]);
				_dtQualifications.Rows.InsertAt(dataRow, focusedRowHandle);
			}
			grd_Qualification.DataSource = _dtQualifications;
			if ((from t in _dtQualifications.AsEnumerable()
				where string.IsNullOrWhiteSpace(Function.ToString(t["QuaCode"]))
				select t).ToList().Count <= 0)
			{
				NewRowQua();
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void FindQueByCFL()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetListQuestion, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			string value = "";
			if (frm_CFL2.ChooseItems != null)
			{
				List<string> list = new List<string>();
				foreach (DataRow item in frm_CFL2.ChooseItems.AsEnumerable())
				{
					list.Add(Function.ToString(item["Id"]));
				}
				value = string.Join(";", list.ToArray());
			}
			gv_Inte.SetFocusedRowCellValue("QuestionsLibrary", value);
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HandleChooseItemMulti(List<DataRow> rows)
	{
		try
		{
			_isCFl = true;
			if (!(grd_App.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0)
			{
				return;
			}
			if (rows.Count > 0)
			{
				Function.ToString(rows[0]["ApplicantNo"]);
				Function.ParseInt(_dtApplicants.Compute("Max(LineNum)", ""));
				gv_App.SetFocusedRowCellValue("ApplicantNo", Function.ToString(rows[0]["ApplicantNo"]));
				for (int i = 1; i < rows.Count; i++)
				{
					DataRow dataRow = _dtApplicants.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ApplicantNo"])));
					if (dataRow != null)
					{
						int dataSourceIndex = _dtApplicants.Rows.IndexOf(dataRow);
						gv_App.SetRowCellValue(gv_App.GetRowHandle(dataSourceIndex), "ApplicantNo", Function.ToString(rows[i]["ApplicantNo"]));
						continue;
					}
					NewRowApp();
					DataRow dataRow2 = _dtApplicants.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["ApplicantNo"])));
					if (dataRow2 != null)
					{
						int dataSourceIndex2 = _dtApplicants.Rows.IndexOf(dataRow2);
						gv_App.SetRowCellValue(gv_App.GetRowHandle(dataSourceIndex2), "ApplicantNo", Function.ToString(rows[i]["ApplicantNo"]));
					}
				}
				grd_App.DataSource = _dtApplicants;
				if ((from t in _dtApplicants.AsEnumerable()
					where string.IsNullOrWhiteSpace(Function.ToString(t["ApplicantNo"]))
					select t).ToList().Count <= 0)
				{
					NewRowApp();
				}
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_isCFl = false;
	}

	private void FindEmpIdInGvInter()
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.GetListEmpId, multiSelect: true, "");
			DataTable dataSourceTable = grd_Hiring.DataSourceTable;
			if (dataSourceTable != null && dataSourceTable.Rows.Count > 0)
			{
				if (Function.ParseInt(dataSourceTable.Rows[0]["EmpID"]) > 0)
				{
					frm_ChooseFromList2.DataEmpJobVacancy = dataSourceTable.Select($"Primary = 'Y'").CopyToDataTable();
					frm_ChooseFromList2.ShowDialog(this);
					if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
					{
						string value = "";
						if (frm_ChooseFromList2.ChooseItems != null)
						{
							List<string> list = new List<string>();
							foreach (dynamic item in frm_ChooseFromList2.ChooseItems.AsEnumerable())
							{
								list.Add(Function.ToString(item["EmpID"]));
							}
							value = string.Join(";", list.ToArray());
						}
						gv_Inte.SetFocusedRowCellValue("InterviewTeam", value);
					}
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.ThereAreNoHiringYet, "");
				}
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void FindAppNoInter()
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.GetListAppNo, multiSelect: true, "");
			frm_ChooseFromList2.DataApplicant = (from t in grd_App.DataSourceTable().AsEnumerable()
				where Function.ParseInt(t["InterviewStatus"]) == 1
				select t).CopyToDataTable();
			frm_ChooseFromList2.ShowDialog(this);
			int focusedRowHandle = gv_Inte.FocusedRowHandle;
			int num = Function.ParseInt(gv_Inte.GetFocusedRowCellValue("VisOrder"));
			int num2 = Function.ParseInt(gv_Inte.GetRowCellValue(focusedRowHandle - 1, "VisOrder"));
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				int focusedRowHandle2 = gv_Applications.FocusedRowHandle;
				gv_Applications.DeleteRow(gv_Applications.FocusedRowHandle);
				_dtAppInInter = grd_Applications.DataSourceTable;
				for (int num3 = 0; num3 < frm_ChooseFromList2.ChooseItems.Count; num3++)
				{
					if (num == num2 && ((DataRow[])dtApplicantInterview.Select(string.Format("ApplicantNo = '{0}'", Function.ToString(frm_ChooseFromList2.ChooseItems[num3]["ApplicantNo"])))).Count() > 0)
					{
						WinCommons.Error(Enums.MessageCode.VisOrderMustNotBeIdentical, "");
						return;
					}
					if (((DataRow[])_dtAppInInter.Select(string.Format("ApplicantNo = '{0}'", Function.ToString(frm_ChooseFromList2.ChooseItems[num3]["ApplicantNo"])))).Count() > 0)
					{
						WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
						return;
					}
					DataRow dataRow = _dtAppInInter.NewRow();
					dataRow["DocEntry"] = Function.ParseInt(gv_Inte.GetFocusedRowCellValue("DocEntry"));
					dataRow["LineInterview"] = Function.ParseInt(gv_Inte.GetFocusedRowCellValue("LineNum"));
					dataRow["ApplicantNo"] = (object)Function.ToString(frm_ChooseFromList2.ChooseItems[num3]["ApplicantNo"]);
					dataRow["ApplicantName"] = (object)Function.ToString(frm_ChooseFromList2.ChooseItems[num3]["ApplicantName"]);
					dataRow["DoB"] = (object)Function.ParseDateTimes(frm_ChooseFromList2.ChooseItems[num3]["DoB"]);
					dataRow["Gender"] = (object)Function.ParseInt(frm_ChooseFromList2.ChooseItems[num3]["Gender"]);
					dataRow["Qualifications"] = (object)Function.ToString(frm_ChooseFromList2.ChooseItems[num3]["Qualifications"]);
					dataRow["Address"] = (object)Function.ToString(frm_ChooseFromList2.ChooseItems[num3]["Address"]);
					dataRow["Phone"] = (object)Function.ToString(frm_ChooseFromList2.ChooseItems[num3]["Phone"]);
					dataRow["Email"] = (object)Function.ToString(frm_ChooseFromList2.ChooseItems[num3]["Email"]);
					dataRow["LineNum"] = num3 + 1;
					_dtAppInInter.Rows.InsertAt(dataRow, focusedRowHandle2);
					_dtAppInInter.AcceptChanges();
				}
				for (int num4 = 0; num4 < _dtAppInInter.Rows.Count; num4++)
				{
					if (string.IsNullOrEmpty(Function.ToString(_dtAppInInter.Rows[num4]["DocEntry"])))
					{
						_dtAppInInter.Rows[num4].Delete();
						_dtAppInInter.AcceptChanges();
					}
				}
				grd_Applications.DataSource = _dtAppInInter;
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void FindAppNoOffer()
	{
		try
		{
			_dtInterview = grd_Inte.DatasourceTable();
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.GetListApplicantInOffer, multiSelect: true, "");
			EnumerableRowCollection<DataRow> source = from t in grd_App.DataSourceTable().AsEnumerable()
				where Function.ToString(t["ApplicantNo"]) != ""
				select t;
			int maxVisOrder = Function.ParseInt(_dtInterview.Compute("MAX(VisOrder)", ""));
			if (source.Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.YouHaveNoApplicant, "");
				return;
			}
			List<int> lstlineId = (from dr in _dtInterview.AsEnumerable()
				where Function.ToString(dr["Status"]) == "F" && Function.ParseInt(dr["VisOrder"]) == maxVisOrder
				select dr.Field<int>("LineNum")).ToList();
			if (lstlineId.Count <= 0)
			{
				return;
			}
			EnumerableRowCollection<DataRow> source2 = from t in dtApplicantInterview.AsEnumerable()
				where lstlineId.Contains(Function.ParseInt(t["LineInterview"])) && Function.ToString(t["Status"]) == "A"
				select t;
			if (source2.Count() <= 0)
			{
				return;
			}
			_dtApplicants = source2.CopyToDataTable();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ApplicantNo", typeof(string));
			dataTable.Columns.Add("ApplicantName", typeof(string));
			for (int num = 0; num < _dtApplicants.Rows.Count; num++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["ApplicantNo"] = _dtApplicants.Rows[num]["ApplicantNo"];
				dataRow["ApplicantName"] = _dtApplicants.Rows[num]["ApplicantName"];
				dataTable.Rows.Add(dataRow);
			}
			for (int num2 = 0; num2 < dataTable.Rows.Count; num2++)
			{
				for (int num3 = num2 + 1; num3 < dataTable.Rows.Count; num3++)
				{
					if (dataTable.Rows[num2]["ApplicantNo"] == dataTable.Rows[num3]["ApplicantNo"])
					{
						dataTable.Rows[num3].Delete();
						dataTable.AcceptChanges();
					}
				}
			}
			frm_ChooseFromList2.DataApplicantInOffer = dataTable;
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItems == null || frm_ChooseFromList2.ChooseItems.Count <= 0)
			{
				return;
			}
			DataTable dataSourceTable = grd_Offer.DataSourceTable;
			_ = grd_Applications.DataSourceTable;
			for (int num4 = 0; num4 < frm_ChooseFromList2.ChooseItems.Count; num4++)
			{
				if (((DataRow[])dataSourceTable.Select(string.Format("ApplicantNo = '{0}'", Function.ToString(frm_ChooseFromList2.ChooseItems[num4]["ApplicantNo"])))).Count() > 0)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
					return;
				}
				DataRow dataRow2 = dataSourceTable.NewRow();
				dataRow2["ApplicantNo"] = (object)Function.ToString(frm_ChooseFromList2.ChooseItems[num4]["ApplicantNo"]);
				dataRow2["ApplicantName"] = (object)Function.ToString(frm_ChooseFromList2.ChooseItems[num4]["ApplicantName"]);
				dataSourceTable.Rows.InsertAt(dataRow2, gv_Offer.FocusedRowHandle);
			}
			grd_Offer.DataSource = dataSourceTable;
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckRowNull(DataTable dt, string key)
	{
		try
		{
			(from DataRow t in dt.Rows
				where string.IsNullOrEmpty(Function.ToString(t[key]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteSelectedRowQua()
	{
		try
		{
			if (gv_Qualification.DataRowCount == 0)
			{
				return;
			}
			string value = Function.ToString(gv_Qualification.GetFocusedRowCellValue("QuaCode"));
			DataTable dataSourceTable = grd_App.DataSourceTable;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				string text = Function.ToString(dataSourceTable.Rows[i]["Qualifications"]);
				if (!(text != ""))
				{
					continue;
				}
				string[] array = text.Split(';');
				for (int num = 0; num < text.Split(';').Count(); num++)
				{
					if (Function.ToString(array[num]).Equals(value))
					{
						WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
						return;
					}
				}
			}
			grd_Qualification.BeginUpdate();
			gv_Qualification.DeleteSelectedRows();
			grd_Qualification.EndUpdate();
			grd_Qualification.DatasourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteSelectedRowApp()
	{
		try
		{
			if (gv_App.DataRowCount <= 0)
			{
				return;
			}
			string value = Function.ToString(gv_App.GetFocusedRowCellValue("ApplicantNo"));
			_ = grd_Inte.DataSourceTable;
			for (int i = 0; i < dtApplicantInterview.Rows.Count; i++)
			{
				if (Function.ToString(dtApplicantInterview.Rows[i]["ApplicantNo"]).Equals(value))
				{
					WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
					return;
				}
			}
			grd_App.BeginUpdate();
			gv_App.DeleteSelectedRows();
			grd_App.EndUpdate();
			grd_App.DatasourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteSelectedRowHiring()
	{
		try
		{
			if (gv_Hiring.DataRowCount <= 0)
			{
				return;
			}
			string value = Function.ToString(gv_Hiring.GetFocusedRowCellValue("EmpID"));
			DataTable dataSourceTable = grd_Inte.DataSourceTable;
			for (int i = 0; i < dataSourceTable.Rows.Count; i++)
			{
				string text = Function.ToString(dataSourceTable.Rows[i]["InterviewTeam"]);
				if (text.Split(';').Count() <= 0)
				{
					continue;
				}
				string[] array = text.Split(';');
				for (int num = 0; num < text.Split(';').Count(); num++)
				{
					if (Function.ToString(array[num]).Equals(value))
					{
						WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
						return;
					}
				}
			}
			grd_Hiring.BeginUpdate();
			gv_Hiring.DeleteSelectedRows();
			grd_Hiring.EndUpdate();
			grd_Hiring.DatasourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteSelectedRowAdv()
	{
		try
		{
			if (gv_Advertisement.DataRowCount > 0)
			{
				grd_Advertisement.BeginUpdate();
				gv_Advertisement.DeleteSelectedRows();
				grd_Advertisement.EndUpdate();
				grd_Advertisement.DatasourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteSelectedRowInter()
	{
		try
		{
			_dtInterview = grd_Inte.DataSourceTable;
			int focusedRowHandle = gv_Inte.FocusedRowHandle;
			_dtInterview.Rows.RemoveAt(focusedRowHandle);
			grd_Inte.DataSource = _dtInterview;
			grd_Inte.RefreshDataSource();
			grd_Inte.DatasourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteSelectedRowOffer()
	{
		try
		{
			if (gv_Offer.DataRowCount > 0)
			{
				grd_Offer.BeginUpdate();
				gv_Offer.DeleteSelectedRows();
				grd_Offer.EndUpdate();
				grd_Offer.DatasourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteSelectedRowHired()
	{
		try
		{
			if (gv_Hired.DataRowCount > 0)
			{
				grd_Hired.BeginUpdate();
				gv_Hired.DeleteSelectedRows();
				grd_Hired.EndUpdate();
				grd_Hired.DatasourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> GetMaxDocNum()
	{
		try
		{
			return await _httpClient.JobVacancy.GetMaxDocNum();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private async Task<int> GetDocEntry()
	{
		try
		{
			return await _httpClient.JobVacancy.GetDocMaxEntry();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_JobVacancy));
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice2 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice3 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemTextEditExPrice4 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.grd_JobVacancy = new Apzon.Controls.Grid.GridControlEx();
		this.menu_Jobvacancy = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_JobVacancy = new Apzon.Controls.Grid.GridViewEx();
		this.DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.DocNum = new Apzon.Controls.Grid.GridColumnEx();
		this.Series = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_series = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Description = new Apzon.Controls.Grid.GridColumnEx();
		this.Status = new Apzon.Controls.Grid.GridColumnEx();
		this.lok_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Branch = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Department = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Department = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Positions = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Position = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.JobTitle = new Apzon.Controls.Grid.GridColumnEx();
		this.FromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.r_FromDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.ToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.r_ToDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.Qty = new Apzon.Controls.Grid.GridColumnEx();
		this.jobVa_JobType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookJobType = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.xtra_APZ_HJV6 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.xtra_APZ_HJV7 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Qualification = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Qualification = new Apzon.Controls.Grid.GridViewEx();
		this.Qua_DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.QuaCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_btnQualificationCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.QuaGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.Qua_Description = new Apzon.Controls.Grid.GridColumnEx();
		this.Qu_Name = new Apzon.Controls.Grid.GridColumnEx();
		this.Qua_Line = new Apzon.Controls.Grid.GridColumnEx();
		this.xtra_APZ_HJV1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Hiring = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Hiring = new Apzon.Controls.Grid.GridViewEx();
		this.Hiring_DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.Hiring_LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.Hiring_EmpID = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_EmpID = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.Hiring_EmpName = new Apzon.Controls.Grid.GridColumnEx();
		this.Hiring_TeamRole = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookTeamRole = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.Hiring_Primary = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookPrimary = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.xtra_APZ_HJV2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Advertisement = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Advertisement = new Apzon.Controls.Grid.GridViewEx();
		this.Ad_DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.Ad_LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.Ad_AdvID = new Apzon.Controls.Grid.GridColumnEx();
		this.Ad_AdvName = new Apzon.Controls.Grid.GridColumnEx();
		this.Ad_AdvSources = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookAdvSource = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.Ad_AdvCost = new Apzon.Controls.Grid.GridColumnEx();
		this.r_AdvCost = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.Ad_Currency = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookCurrency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Ad_FromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.r_AdvFromDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.Ad_ToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.r_AdvToDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.Ad_Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.xtra_APZ_HJV3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_App = new Apzon.Controls.Grid.GridControlEx();
		this.gv_App = new Apzon.Controls.Grid.GridViewEx();
		this.Ap_DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.Ap_LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.Ap_ApplicantNo = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_AppNo = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.Ap_ApplicantName = new Apzon.Controls.Grid.GridColumnEx();
		this.Ap_DoB = new Apzon.Controls.Grid.GridColumnEx();
		this.r_AppDoB = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.Ap_Gender = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookGender = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Ap_Qualifications = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CheckedQualification = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.Ap_Address = new Apzon.Controls.Grid.GridColumnEx();
		this.Ap_Phone = new Apzon.Controls.Grid.GridColumnEx();
		this.Email = new Apzon.Controls.Grid.GridColumnEx();
		this.Ap_InterviewStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StatusApplican = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookInterviewStatus = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.xtra_APZ_HJV4 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Inte = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Inte = new Apzon.Controls.Grid.GridViewEx();
		this.Inte_DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.Inte_LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.Inte_InterviewID = new Apzon.Controls.Grid.GridColumnEx();
		this.Inte_InterviewName = new Apzon.Controls.Grid.GridColumnEx();
		this.Inte_InterviewType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookInteType = new Apzon.Forms.UserControls.RepositoryItemLookUpEditDefineNew();
		this.Inte_QuestionsLibrary = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_btnQues = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.Inte_InterviewTeam = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_btnTeam = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.Inte_VisOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.Inte_Status = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Inte_Date = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_dateDateInter = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.rep_btnAppInInter = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.r_checkTeam = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.grd_Applications = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Applications = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_appInIter = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_Gender2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.r_Qualification2 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StatusApp = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtra_APZ_HJV5 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Offer = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Offer = new Apzon.Controls.Grid.GridViewEx();
		this.Of_DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.Of_LineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.Of_ApplicantName = new Apzon.Controls.Grid.GridColumnEx();
		this.Of_OfferLetter = new Apzon.Controls.Grid.GridColumnEx();
		this.Of_TrialPeriodSalary = new Apzon.Controls.Grid.GridColumnEx();
		this.Of_ContractSalary = new Apzon.Controls.Grid.GridColumnEx();
		this.Of_OtherBenefits = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_checkBenefits = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.Offer_TrialCur = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_looTrialCur = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.of_ContCur = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookContractCur = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StatusOff = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Of_ApplicantNo = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_AppInOffer = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.xtra_Hired = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Hired = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Hired = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_btnappHired = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_checkBeneHired = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookHTrialCur = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_lookHContractCur = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StatusHired = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_DocNo = new Apzon.Controls.Editor.TextEditEx();
		this.look_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Profile = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Hired = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Accept = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_JobVacancy).BeginInit();
		this.menu_Jobvacancy.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_JobVacancy).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_series).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lok_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Position).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_FromDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_FromDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_ToDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_ToDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookJobType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtra_APZ_HJV6).BeginInit();
		this.xtra_APZ_HJV6.SuspendLayout();
		this.contextMenuStrip1.SuspendLayout();
		this.xtra_APZ_HJV7.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Qualification).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Qualification).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnQualificationCode).BeginInit();
		this.xtra_APZ_HJV1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Hiring).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Hiring).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_EmpID).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookTeamRole).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookPrimary).BeginInit();
		this.xtra_APZ_HJV2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Advertisement).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Advertisement).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookAdvSource).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_AdvCost).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookCurrency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_AdvFromDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_AdvFromDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_AdvToDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_AdvToDate.CalendarTimeProperties).BeginInit();
		this.xtra_APZ_HJV3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_App).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_App).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_AppNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_AppDoB).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_AppDoB.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookGender).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CheckedQualification).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusApplican).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookInterviewStatus).BeginInit();
		this.xtra_APZ_HJV4.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Inte).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Inte).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookInteType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnQues).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnTeam).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateDateInter).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateDateInter.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnAppInInter).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_checkTeam).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Applications).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Applications).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_appInIter).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_Gender2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.r_Qualification2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusApp).BeginInit();
		this.xtra_APZ_HJV5.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Offer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Offer).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_checkBenefits).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_looTrialCur).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookContractCur).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusOff).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_AppInOffer).BeginInit();
		this.xtra_Hired.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Hired).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Hired).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnappHired).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_checkBeneHired).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookHTrialCur).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookHContractCur).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusHired).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.repositoryItemTextEditExPrice2.Name = "repositoryItemTextEditExPrice2";
		this.repositoryItemTextEditExPrice3.Name = "repositoryItemTextEditExPrice3";
		this.repositoryItemTextEditExPrice4.Name = "repositoryItemTextEditExPrice4";
		this.grd_JobVacancy.AllowPasteExcel = true;
		this.grd_JobVacancy.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_JobVacancy.ClipboardData = null;
		this.grd_JobVacancy.ContextMenuStrip = this.menu_Jobvacancy;
		this.grd_JobVacancy.Location = new System.Drawing.Point(0, 0);
		this.grd_JobVacancy.MainView = this.gv_JobVacancy;
		this.grd_JobVacancy.Name = "grd_JobVacancy";
		this.grd_JobVacancy.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.lok_Status, this.luk_Department, this.luk_Position, this.luk_Branch, this.luk_series, this.r_FromDate, this.r_ToDate, this.rep_lookJobType });
		this.grd_JobVacancy.Size = new System.Drawing.Size(958, 147);
		this.grd_JobVacancy.TabIndex = 0;
		this.grd_JobVacancy.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_JobVacancy });
		this.menu_Jobvacancy.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addToolStripMenuItem, this.deleteToolStripMenuItem1, this.duplicateToolStripMenuItem });
		this.menu_Jobvacancy.Name = "contextMenuStrip1";
		this.menu_Jobvacancy.Size = new System.Drawing.Size(125, 70);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
		this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(124, 22);
		this.deleteToolStripMenuItem1.Text = "Delete";
		this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.gv_JobVacancy.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_JobVacancy.ColumnKey = null;
		this.gv_JobVacancy.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.DocEntry, this.DocNum, this.Series, this.Description, this.Status, this.Branch, this.Department, this.Positions, this.JobTitle, this.FromDate,
			this.ToDate, this.Qty, this.jobVa_JobType
		});
		this.gv_JobVacancy.DisableFormSetting = false;
		this.gv_JobVacancy.DisplayException = false;
		this.gv_JobVacancy.FieldCurrency = null;
		this.gv_JobVacancy.FieldRate = null;
		this.gv_JobVacancy.FormId = "700012";
		this.gv_JobVacancy.GridControl = this.grd_JobVacancy;
		this.gv_JobVacancy.IndicatorWidth = 30;
		this.gv_JobVacancy.IsApzTable = false;
		this.gv_JobVacancy.IsInsertNewRow = false;
		this.gv_JobVacancy.IsLoadingData = false;
		this.gv_JobVacancy.ItemId = "APZ_OHJV";
		this.gv_JobVacancy.LstColumnAllowBatch = null;
		this.gv_JobVacancy.LstColumnNotAllowBatch = null;
		this.gv_JobVacancy.LstColumnNotEditable = null;
		this.gv_JobVacancy.LstColumNotSetting = null;
		this.gv_JobVacancy.Name = "gv_JobVacancy";
		this.gv_JobVacancy.OptionsView.ShowGroupPanel = false;
		this.gv_JobVacancy.UseBatchMod = false;
		this.gv_JobVacancy.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_JobVacancy_RowCellStyle);
		this.gv_JobVacancy.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_JobVacancy_ShowingEditor);
		this.gv_JobVacancy.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_JobVacancy_FocusedRowObjectChanged);
		this.gv_JobVacancy.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_JobVacancy_CellValueChanging);
		this.gv_JobVacancy.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_JobVacancy_BeforeLeaveRow);
		this.DocEntry.Caption = "Doc Entry";
		this.DocEntry.CurrencyDateField = null;
		this.DocEntry.CurrencyField = null;
		this.DocEntry.CurrencyFixed = null;
		this.DocEntry.CurrencyFixedRate = 0.0;
		this.DocEntry.CurrencyRateField = null;
		this.DocEntry.FieldName = "DocEntry";
		this.DocEntry.Name = "DocEntry";
		this.DocEntry.OptionsColumn.ReadOnly = true;
		this.DocEntry.PrimaryKey = true;
		this.DocEntry.UnitField = null;
		this.DocNum.Caption = "Doc No";
		this.DocNum.CurrencyDateField = null;
		this.DocNum.CurrencyField = null;
		this.DocNum.CurrencyFixed = null;
		this.DocNum.CurrencyFixedRate = 0.0;
		this.DocNum.CurrencyRateField = null;
		this.DocNum.FieldName = "DocNum";
		this.DocNum.Name = "DocNum";
		this.DocNum.OptionsColumn.ReadOnly = true;
		this.DocNum.PrimaryKey = false;
		this.DocNum.UnitField = null;
		this.DocNum.Visible = true;
		this.DocNum.VisibleIndex = 0;
		this.Series.Caption = "Series";
		this.Series.ColumnEdit = this.luk_series;
		this.Series.CurrencyDateField = null;
		this.Series.CurrencyField = null;
		this.Series.CurrencyFixed = null;
		this.Series.CurrencyFixedRate = 0.0;
		this.Series.CurrencyRateField = null;
		this.Series.FieldName = "Series";
		this.Series.Name = "Series";
		this.Series.PrimaryKey = false;
		this.Series.UnitField = null;
		this.Series.Visible = true;
		this.Series.VisibleIndex = 1;
		this.luk_series.AutoHeight = false;
		this.luk_series.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_series.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SeriesName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Series", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_series.DisplayMember = "SeriesName";
		this.luk_series.Name = "luk_series";
		this.luk_series.NullText = "";
		this.luk_series.ValueMember = "Series";
		this.Description.Caption = "Desc";
		this.Description.CurrencyDateField = null;
		this.Description.CurrencyField = null;
		this.Description.CurrencyFixed = null;
		this.Description.CurrencyFixedRate = 0.0;
		this.Description.CurrencyRateField = null;
		this.Description.FieldName = "Description";
		this.Description.Name = "Description";
		this.Description.PrimaryKey = false;
		this.Description.UnitField = null;
		this.Description.Visible = true;
		this.Description.VisibleIndex = 2;
		this.Status.Caption = "Status";
		this.Status.ColumnEdit = this.lok_Status;
		this.Status.CurrencyDateField = null;
		this.Status.CurrencyField = null;
		this.Status.CurrencyFixed = null;
		this.Status.CurrencyFixedRate = 0.0;
		this.Status.CurrencyRateField = null;
		this.Status.FieldName = "Status";
		this.Status.Name = "Status";
		this.Status.PrimaryKey = false;
		this.Status.UnitField = null;
		this.Status.Visible = true;
		this.Status.VisibleIndex = 3;
		this.lok_Status.AutoHeight = false;
		this.lok_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lok_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.lok_Status.DisplayMember = "Name";
		this.lok_Status.Name = "lok_Status";
		this.lok_Status.NullText = "";
		this.lok_Status.ValueMember = "Code";
		this.Branch.Caption = "Branch";
		this.Branch.ColumnEdit = this.luk_Branch;
		this.Branch.CurrencyDateField = null;
		this.Branch.CurrencyField = null;
		this.Branch.CurrencyFixed = null;
		this.Branch.CurrencyFixedRate = 0.0;
		this.Branch.CurrencyRateField = null;
		this.Branch.FieldName = "Branch";
		this.Branch.Name = "Branch";
		this.Branch.PrimaryKey = false;
		this.Branch.UnitField = null;
		this.Branch.Visible = true;
		this.Branch.VisibleIndex = 4;
		this.luk_Branch.AutoHeight = false;
		this.luk_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Branch.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Branch.KeyMember = "Name";
		this.luk_Branch.Name = "luk_Branch";
		this.luk_Branch.NullText = "";
		this.luk_Branch.ValueMember = "Code";
		this.Department.Caption = "Department";
		this.Department.ColumnEdit = this.luk_Department;
		this.Department.CurrencyDateField = null;
		this.Department.CurrencyField = null;
		this.Department.CurrencyFixed = null;
		this.Department.CurrencyFixedRate = 0.0;
		this.Department.CurrencyRateField = null;
		this.Department.FieldName = "Department";
		this.Department.Name = "Department";
		this.Department.PrimaryKey = false;
		this.Department.UnitField = null;
		this.Department.Visible = true;
		this.Department.VisibleIndex = 5;
		this.luk_Department.AutoHeight = false;
		this.luk_Department.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Department.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Department.DisplayMember = "Name";
		this.luk_Department.Name = "luk_Department";
		this.luk_Department.NullText = "";
		this.luk_Department.ValueMember = "Code";
		this.Positions.Caption = "Positions";
		this.Positions.ColumnEdit = this.luk_Position;
		this.Positions.CurrencyDateField = null;
		this.Positions.CurrencyField = null;
		this.Positions.CurrencyFixed = null;
		this.Positions.CurrencyFixedRate = 0.0;
		this.Positions.CurrencyRateField = null;
		this.Positions.FieldName = "Positions";
		this.Positions.Name = "Positions";
		this.Positions.PrimaryKey = false;
		this.Positions.UnitField = null;
		this.Positions.Visible = true;
		this.Positions.VisibleIndex = 6;
		this.luk_Position.AutoHeight = false;
		this.luk_Position.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Position.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("posID", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_Position.DisplayMember = "name";
		this.luk_Position.Name = "luk_Position";
		this.luk_Position.NullText = "";
		this.luk_Position.ValueMember = "posID";
		this.JobTitle.Caption = "JobTitle";
		this.JobTitle.CurrencyDateField = null;
		this.JobTitle.CurrencyField = null;
		this.JobTitle.CurrencyFixed = null;
		this.JobTitle.CurrencyFixedRate = 0.0;
		this.JobTitle.CurrencyRateField = null;
		this.JobTitle.FieldName = "JobTitle";
		this.JobTitle.Name = "JobTitle";
		this.JobTitle.PrimaryKey = false;
		this.JobTitle.UnitField = null;
		this.JobTitle.Visible = true;
		this.JobTitle.VisibleIndex = 7;
		this.FromDate.Caption = "From Date";
		this.FromDate.ColumnEdit = this.r_FromDate;
		this.FromDate.CurrencyDateField = null;
		this.FromDate.CurrencyField = null;
		this.FromDate.CurrencyFixed = null;
		this.FromDate.CurrencyFixedRate = 0.0;
		this.FromDate.CurrencyRateField = null;
		this.FromDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.FromDate.FieldName = "FromDate";
		this.FromDate.Name = "FromDate";
		this.FromDate.PrimaryKey = false;
		this.FromDate.UnitField = null;
		this.FromDate.Visible = true;
		this.FromDate.VisibleIndex = 9;
		this.r_FromDate.AutoHeight = false;
		this.r_FromDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_FromDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_FromDate.Name = "r_FromDate";
		this.ToDate.Caption = "To Date";
		this.ToDate.ColumnEdit = this.r_ToDate;
		this.ToDate.CurrencyDateField = null;
		this.ToDate.CurrencyField = null;
		this.ToDate.CurrencyFixed = null;
		this.ToDate.CurrencyFixedRate = 0.0;
		this.ToDate.CurrencyRateField = null;
		this.ToDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.ToDate.FieldName = "ToDate";
		this.ToDate.Name = "ToDate";
		this.ToDate.PrimaryKey = false;
		this.ToDate.UnitField = null;
		this.ToDate.Visible = true;
		this.ToDate.VisibleIndex = 10;
		this.r_ToDate.AutoHeight = false;
		this.r_ToDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_ToDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_ToDate.Name = "r_ToDate";
		this.Qty.Caption = "Qty";
		this.Qty.CurrencyDateField = null;
		this.Qty.CurrencyField = null;
		this.Qty.CurrencyFixed = null;
		this.Qty.CurrencyFixedRate = 0.0;
		this.Qty.CurrencyRateField = null;
		this.Qty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Integer;
		this.Qty.FieldName = "Qty";
		this.Qty.Name = "Qty";
		this.Qty.PrimaryKey = false;
		this.Qty.UnitField = null;
		this.Qty.Visible = true;
		this.Qty.VisibleIndex = 11;
		this.jobVa_JobType.Caption = "Job Type";
		this.jobVa_JobType.ColumnEdit = this.rep_lookJobType;
		this.jobVa_JobType.CurrencyDateField = null;
		this.jobVa_JobType.CurrencyField = null;
		this.jobVa_JobType.CurrencyFixed = null;
		this.jobVa_JobType.CurrencyFixedRate = 0.0;
		this.jobVa_JobType.CurrencyRateField = null;
		this.jobVa_JobType.FieldName = "JobType";
		this.jobVa_JobType.Name = "jobVa_JobType";
		this.jobVa_JobType.PrimaryKey = false;
		this.jobVa_JobType.UnitField = null;
		this.jobVa_JobType.Visible = true;
		this.jobVa_JobType.VisibleIndex = 8;
		this.rep_lookJobType.AutoHeight = false;
		this.rep_lookJobType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_lookJobType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookJobType.CheckFields = null;
		this.rep_lookJobType.CheckTables = null;
		this.rep_lookJobType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Id", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookJobType.DisplayMember = "Name";
		this.rep_lookJobType.Name = "rep_lookJobType";
		this.rep_lookJobType.NullText = "";
		this.rep_lookJobType.ShowFooter = false;
		this.rep_lookJobType.ShowHeader = false;
		this.rep_lookJobType.ValueMember = "Id";
		this.xtra_APZ_HJV6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtra_APZ_HJV6.ContextMenuStrip = this.contextMenuStrip1;
		this.xtra_APZ_HJV6.IsSearchControl = false;
		this.xtra_APZ_HJV6.Location = new System.Drawing.Point(0, 0);
		this.xtra_APZ_HJV6.Name = "xtra_APZ_HJV6";
		this.xtra_APZ_HJV6.SelectedTabPage = this.xtra_APZ_HJV7;
		this.xtra_APZ_HJV6.Size = new System.Drawing.Size(958, 314);
		this.xtra_APZ_HJV6.TabIndex = 1;
		this.xtra_APZ_HJV6.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[7] { this.xtra_APZ_HJV7, this.xtra_APZ_HJV1, this.xtra_APZ_HJV2, this.xtra_APZ_HJV3, this.xtra_APZ_HJV4, this.xtra_APZ_HJV5, this.xtra_Hired });
		this.xtra_APZ_HJV6.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtra_JobVacancy_SelectedPageChanged);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addRowToolStripMenuItem, this.deleteToolStripMenuItem, this.duplicateRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(148, 70);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
		this.addRowToolStripMenuItem.Text = "AddRow";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
		this.deleteToolStripMenuItem.Text = "DeleteRow";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.duplicateRowToolStripMenuItem.Name = "duplicateRowToolStripMenuItem";
		this.duplicateRowToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
		this.duplicateRowToolStripMenuItem.Text = "DuplicateRow";
		this.duplicateRowToolStripMenuItem.Click += new System.EventHandler(this.duplicateRowToolStripMenuItem_Click);
		this.xtra_APZ_HJV7.Controls.Add(this.grd_Qualification);
		this.xtra_APZ_HJV7.EditControl = null;
		this.xtra_APZ_HJV7.EditValue = "Qualifications";
		this.xtra_APZ_HJV7.FieldName = "xtra_APZ_HJV7";
		this.xtra_APZ_HJV7.IsSearchControl = false;
		this.xtra_APZ_HJV7.Name = "xtra_APZ_HJV7";
		this.xtra_APZ_HJV7.ReadOnly = false;
		this.xtra_APZ_HJV7.SettingInVisible = false;
		this.xtra_APZ_HJV7.SettingReadonly = false;
		this.xtra_APZ_HJV7.Size = new System.Drawing.Size(956, 289);
		this.xtra_APZ_HJV7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_APZ_HJV7.Text = "Qualifications";
		this.grd_Qualification.AllowPasteExcel = true;
		this.grd_Qualification.ClipboardData = null;
		this.grd_Qualification.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Qualification.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Qualification.Location = new System.Drawing.Point(0, 0);
		this.grd_Qualification.MainView = this.gv_Qualification;
		this.grd_Qualification.Name = "grd_Qualification";
		this.grd_Qualification.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_btnQualificationCode });
		this.grd_Qualification.Size = new System.Drawing.Size(956, 289);
		this.grd_Qualification.TabIndex = 0;
		this.grd_Qualification.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Qualification });
		this.gv_Qualification.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Qualification.ColumnKey = null;
		this.gv_Qualification.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.Qua_DocEntry, this.QuaCode, this.QuaGroup, this.Qua_Description, this.Qu_Name, this.Qua_Line });
		this.gv_Qualification.DisableFormSetting = false;
		this.gv_Qualification.DisplayException = false;
		this.gv_Qualification.FieldCurrency = null;
		this.gv_Qualification.FieldRate = null;
		this.gv_Qualification.FormId = "700012";
		this.gv_Qualification.GridControl = this.grd_Qualification;
		this.gv_Qualification.IndicatorWidth = 30;
		this.gv_Qualification.IsApzTable = false;
		this.gv_Qualification.IsInsertNewRow = false;
		this.gv_Qualification.IsLoadingData = false;
		this.gv_Qualification.ItemId = "APZ_HJV7";
		this.gv_Qualification.LstColumnAllowBatch = null;
		this.gv_Qualification.LstColumnNotAllowBatch = null;
		this.gv_Qualification.LstColumnNotEditable = null;
		this.gv_Qualification.LstColumNotSetting = null;
		this.gv_Qualification.Name = "gv_Qualification";
		this.gv_Qualification.OptionsView.ShowGroupPanel = false;
		this.gv_Qualification.UseBatchMod = false;
		this.gv_Qualification.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Qualification_RowCellStyle);
		this.gv_Qualification.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Qualification_ShowingEditor);
		this.Qua_DocEntry.Caption = "Doc";
		this.Qua_DocEntry.CurrencyDateField = null;
		this.Qua_DocEntry.CurrencyField = null;
		this.Qua_DocEntry.CurrencyFixed = null;
		this.Qua_DocEntry.CurrencyFixedRate = 0.0;
		this.Qua_DocEntry.CurrencyRateField = null;
		this.Qua_DocEntry.FieldName = "DocEntry";
		this.Qua_DocEntry.Name = "Qua_DocEntry";
		this.Qua_DocEntry.OptionsColumn.AllowEdit = false;
		this.Qua_DocEntry.OptionsColumn.ReadOnly = true;
		this.Qua_DocEntry.PrimaryKey = true;
		this.Qua_DocEntry.UnitField = null;
		this.QuaCode.Caption = "Qualification Code";
		this.QuaCode.ColumnEdit = this.rep_btnQualificationCode;
		this.QuaCode.CurrencyDateField = null;
		this.QuaCode.CurrencyField = null;
		this.QuaCode.CurrencyFixed = null;
		this.QuaCode.CurrencyFixedRate = 0.0;
		this.QuaCode.CurrencyRateField = null;
		this.QuaCode.FieldName = "QuaCode";
		this.QuaCode.Name = "QuaCode";
		this.QuaCode.PrimaryKey = false;
		this.QuaCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.QuaCode.UnitField = null;
		this.QuaCode.Visible = true;
		this.QuaCode.VisibleIndex = 0;
		this.rep_btnQualificationCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_btnQualificationCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_btnQualificationCode.Name = "rep_btnQualificationCode";
		this.rep_btnQualificationCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_btnQualificationCode_ButtonClick);
		this.QuaGroup.Caption = "Qualification Group";
		this.QuaGroup.CurrencyDateField = null;
		this.QuaGroup.CurrencyField = null;
		this.QuaGroup.CurrencyFixed = null;
		this.QuaGroup.CurrencyFixedRate = 0.0;
		this.QuaGroup.CurrencyRateField = null;
		this.QuaGroup.FieldName = "QuaGroup";
		this.QuaGroup.Name = "QuaGroup";
		this.QuaGroup.OptionsColumn.ReadOnly = true;
		this.QuaGroup.PrimaryKey = false;
		this.QuaGroup.UnitField = null;
		this.QuaGroup.Visible = true;
		this.QuaGroup.VisibleIndex = 2;
		this.Qua_Description.Caption = "Desc";
		this.Qua_Description.CurrencyDateField = null;
		this.Qua_Description.CurrencyField = null;
		this.Qua_Description.CurrencyFixed = null;
		this.Qua_Description.CurrencyFixedRate = 0.0;
		this.Qua_Description.CurrencyRateField = null;
		this.Qua_Description.FieldName = "Description";
		this.Qua_Description.Name = "Qua_Description";
		this.Qua_Description.PrimaryKey = false;
		this.Qua_Description.UnitField = null;
		this.Qua_Description.Visible = true;
		this.Qua_Description.VisibleIndex = 3;
		this.Qu_Name.Caption = "Name";
		this.Qu_Name.CurrencyDateField = null;
		this.Qu_Name.CurrencyField = null;
		this.Qu_Name.CurrencyFixed = null;
		this.Qu_Name.CurrencyFixedRate = 0.0;
		this.Qu_Name.CurrencyRateField = null;
		this.Qu_Name.FieldName = "Name";
		this.Qu_Name.Name = "Qu_Name";
		this.Qu_Name.OptionsColumn.ReadOnly = true;
		this.Qu_Name.PrimaryKey = false;
		this.Qu_Name.UnitField = null;
		this.Qu_Name.Visible = true;
		this.Qu_Name.VisibleIndex = 1;
		this.Qua_Line.Caption = "Line Num";
		this.Qua_Line.CurrencyDateField = null;
		this.Qua_Line.CurrencyField = null;
		this.Qua_Line.CurrencyFixed = null;
		this.Qua_Line.CurrencyFixedRate = 0.0;
		this.Qua_Line.CurrencyRateField = null;
		this.Qua_Line.FieldName = "LineNum";
		this.Qua_Line.Name = "Qua_Line";
		this.Qua_Line.OptionsColumn.AllowEdit = false;
		this.Qua_Line.OptionsColumn.ReadOnly = true;
		this.Qua_Line.PrimaryKey = false;
		this.Qua_Line.UnitField = null;
		this.xtra_APZ_HJV1.Controls.Add(this.grd_Hiring);
		this.xtra_APZ_HJV1.EditControl = null;
		this.xtra_APZ_HJV1.EditValue = "Hiring Team";
		this.xtra_APZ_HJV1.FieldName = "xtra_APZ_HJV1";
		this.xtra_APZ_HJV1.IsSearchControl = false;
		this.xtra_APZ_HJV1.Name = "xtra_APZ_HJV1";
		this.xtra_APZ_HJV1.ReadOnly = false;
		this.xtra_APZ_HJV1.SettingInVisible = false;
		this.xtra_APZ_HJV1.SettingReadonly = false;
		this.xtra_APZ_HJV1.Size = new System.Drawing.Size(956, 289);
		this.xtra_APZ_HJV1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_APZ_HJV1.Text = "Hiring Team";
		this.grd_Hiring.AllowPasteExcel = true;
		this.grd_Hiring.ClipboardData = null;
		this.grd_Hiring.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Hiring.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Hiring.Location = new System.Drawing.Point(0, 0);
		this.grd_Hiring.MainView = this.gv_Hiring;
		this.grd_Hiring.Name = "grd_Hiring";
		this.grd_Hiring.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_EmpID, this.rep_lookTeamRole, this.rep_lookPrimary });
		this.grd_Hiring.Size = new System.Drawing.Size(956, 289);
		this.grd_Hiring.TabIndex = 0;
		this.grd_Hiring.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Hiring });
		this.gv_Hiring.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Hiring.ColumnKey = "EmpID";
		this.gv_Hiring.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.Hiring_DocEntry, this.Hiring_LineNum, this.Hiring_EmpID, this.Hiring_EmpName, this.Hiring_TeamRole, this.Hiring_Primary });
		this.gv_Hiring.DisableFormSetting = false;
		this.gv_Hiring.DisplayException = false;
		this.gv_Hiring.FieldCurrency = null;
		this.gv_Hiring.FieldRate = null;
		this.gv_Hiring.FormId = "700012";
		this.gv_Hiring.GridControl = this.grd_Hiring;
		this.gv_Hiring.IndicatorWidth = 30;
		this.gv_Hiring.IsApzTable = false;
		this.gv_Hiring.IsInsertNewRow = false;
		this.gv_Hiring.IsLoadingData = false;
		this.gv_Hiring.ItemId = "APZ_HJV1";
		this.gv_Hiring.LstColumnAllowBatch = null;
		this.gv_Hiring.LstColumnNotAllowBatch = null;
		this.gv_Hiring.LstColumnNotEditable = null;
		this.gv_Hiring.LstColumNotSetting = null;
		this.gv_Hiring.Name = "gv_Hiring";
		this.gv_Hiring.OptionsView.ShowGroupPanel = false;
		this.gv_Hiring.UseBatchMod = false;
		this.gv_Hiring.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Hiring_RowCellStyle);
		this.gv_Hiring.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Hiring_ShowingEditor);
		this.Hiring_DocEntry.Caption = "DocEntry";
		this.Hiring_DocEntry.CurrencyDateField = null;
		this.Hiring_DocEntry.CurrencyField = null;
		this.Hiring_DocEntry.CurrencyFixed = null;
		this.Hiring_DocEntry.CurrencyFixedRate = 0.0;
		this.Hiring_DocEntry.CurrencyRateField = null;
		this.Hiring_DocEntry.FieldName = "DocEntry";
		this.Hiring_DocEntry.Name = "Hiring_DocEntry";
		this.Hiring_DocEntry.OptionsColumn.AllowEdit = false;
		this.Hiring_DocEntry.OptionsColumn.ReadOnly = true;
		this.Hiring_DocEntry.PrimaryKey = false;
		this.Hiring_DocEntry.UnitField = null;
		this.Hiring_LineNum.Caption = "LineNum";
		this.Hiring_LineNum.CurrencyDateField = null;
		this.Hiring_LineNum.CurrencyField = null;
		this.Hiring_LineNum.CurrencyFixed = null;
		this.Hiring_LineNum.CurrencyFixedRate = 0.0;
		this.Hiring_LineNum.CurrencyRateField = null;
		this.Hiring_LineNum.FieldName = "LineNum";
		this.Hiring_LineNum.Name = "Hiring_LineNum";
		this.Hiring_LineNum.OptionsColumn.AllowEdit = false;
		this.Hiring_LineNum.OptionsColumn.ReadOnly = true;
		this.Hiring_LineNum.PrimaryKey = false;
		this.Hiring_LineNum.UnitField = null;
		this.Hiring_EmpID.Caption = "Emp ID";
		this.Hiring_EmpID.ColumnEdit = this.rep_EmpID;
		this.Hiring_EmpID.CurrencyDateField = null;
		this.Hiring_EmpID.CurrencyField = null;
		this.Hiring_EmpID.CurrencyFixed = null;
		this.Hiring_EmpID.CurrencyFixedRate = 0.0;
		this.Hiring_EmpID.CurrencyRateField = null;
		this.Hiring_EmpID.FieldName = "EmpID";
		this.Hiring_EmpID.Name = "Hiring_EmpID";
		this.Hiring_EmpID.PrimaryKey = false;
		this.Hiring_EmpID.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.Hiring_EmpID.UnitField = null;
		this.Hiring_EmpID.Visible = true;
		this.Hiring_EmpID.VisibleIndex = 0;
		this.rep_EmpID.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_EmpID.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_EmpID.Name = "rep_EmpID";
		this.rep_EmpID.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_EmpID_ButtonClick);
		this.Hiring_EmpName.Caption = "Emp Name";
		this.Hiring_EmpName.CurrencyDateField = null;
		this.Hiring_EmpName.CurrencyField = null;
		this.Hiring_EmpName.CurrencyFixed = null;
		this.Hiring_EmpName.CurrencyFixedRate = 0.0;
		this.Hiring_EmpName.CurrencyRateField = null;
		this.Hiring_EmpName.FieldName = "EmpName";
		this.Hiring_EmpName.Name = "Hiring_EmpName";
		this.Hiring_EmpName.OptionsColumn.ReadOnly = true;
		this.Hiring_EmpName.PrimaryKey = false;
		this.Hiring_EmpName.UnitField = null;
		this.Hiring_EmpName.Visible = true;
		this.Hiring_EmpName.VisibleIndex = 1;
		this.Hiring_TeamRole.Caption = "Team Role";
		this.Hiring_TeamRole.ColumnEdit = this.rep_lookTeamRole;
		this.Hiring_TeamRole.CurrencyDateField = null;
		this.Hiring_TeamRole.CurrencyField = null;
		this.Hiring_TeamRole.CurrencyFixed = null;
		this.Hiring_TeamRole.CurrencyFixedRate = 0.0;
		this.Hiring_TeamRole.CurrencyRateField = null;
		this.Hiring_TeamRole.FieldName = "TeamRole";
		this.Hiring_TeamRole.Name = "Hiring_TeamRole";
		this.Hiring_TeamRole.PrimaryKey = false;
		this.Hiring_TeamRole.UnitField = null;
		this.Hiring_TeamRole.Visible = true;
		this.Hiring_TeamRole.VisibleIndex = 2;
		this.rep_lookTeamRole.AutoHeight = false;
		this.rep_lookTeamRole.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_lookTeamRole.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookTeamRole.CheckFields = null;
		this.rep_lookTeamRole.CheckTables = null;
		this.rep_lookTeamRole.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("typeID", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookTeamRole.DisplayMember = "Name";
		this.rep_lookTeamRole.Name = "rep_lookTeamRole";
		this.rep_lookTeamRole.NullText = "";
		this.rep_lookTeamRole.ShowFooter = false;
		this.rep_lookTeamRole.ShowHeader = false;
		this.rep_lookTeamRole.ValueMember = "Id";
		this.Hiring_Primary.Caption = "Primary";
		this.Hiring_Primary.ColumnEdit = this.rep_lookPrimary;
		this.Hiring_Primary.CurrencyDateField = null;
		this.Hiring_Primary.CurrencyField = null;
		this.Hiring_Primary.CurrencyFixed = null;
		this.Hiring_Primary.CurrencyFixedRate = 0.0;
		this.Hiring_Primary.CurrencyRateField = null;
		this.Hiring_Primary.FieldName = "Primary";
		this.Hiring_Primary.Name = "Hiring_Primary";
		this.Hiring_Primary.PrimaryKey = false;
		this.Hiring_Primary.UnitField = null;
		this.Hiring_Primary.Visible = true;
		this.Hiring_Primary.VisibleIndex = 3;
		this.rep_lookPrimary.AutoHeight = false;
		this.rep_lookPrimary.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookPrimary.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookPrimary.DisplayMember = "Name";
		this.rep_lookPrimary.Name = "rep_lookPrimary";
		this.rep_lookPrimary.NullText = "";
		this.rep_lookPrimary.ValueMember = "Code";
		this.xtra_APZ_HJV2.Controls.Add(this.grd_Advertisement);
		this.xtra_APZ_HJV2.EditControl = null;
		this.xtra_APZ_HJV2.EditValue = "Advertisement";
		this.xtra_APZ_HJV2.FieldName = "xtra_APZ_HJV2";
		this.xtra_APZ_HJV2.IsSearchControl = false;
		this.xtra_APZ_HJV2.Name = "xtra_APZ_HJV2";
		this.xtra_APZ_HJV2.ReadOnly = false;
		this.xtra_APZ_HJV2.SettingInVisible = false;
		this.xtra_APZ_HJV2.SettingReadonly = false;
		this.xtra_APZ_HJV2.Size = new System.Drawing.Size(956, 289);
		this.xtra_APZ_HJV2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_APZ_HJV2.Text = "Advertisement";
		this.grd_Advertisement.AllowPasteExcel = true;
		this.grd_Advertisement.ClipboardData = null;
		this.grd_Advertisement.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Advertisement.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Advertisement.Location = new System.Drawing.Point(0, 0);
		this.grd_Advertisement.MainView = this.gv_Advertisement;
		this.grd_Advertisement.Name = "grd_Advertisement";
		this.grd_Advertisement.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.r_AdvCost, this.r_AdvFromDate, this.r_AdvToDate, this.rep_lookCurrency, this.rep_lookAdvSource });
		this.grd_Advertisement.Size = new System.Drawing.Size(956, 289);
		this.grd_Advertisement.TabIndex = 0;
		this.grd_Advertisement.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Advertisement });
		this.gv_Advertisement.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Advertisement.ColumnKey = "AdvID";
		this.gv_Advertisement.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.Ad_DocEntry, this.Ad_LineNum, this.Ad_AdvID, this.Ad_AdvName, this.Ad_AdvSources, this.Ad_AdvCost, this.Ad_Currency, this.Ad_FromDate, this.Ad_ToDate, this.Ad_Remark });
		this.gv_Advertisement.DisableFormSetting = false;
		this.gv_Advertisement.DisplayException = false;
		this.gv_Advertisement.FieldCurrency = null;
		this.gv_Advertisement.FieldRate = null;
		this.gv_Advertisement.FormId = "700012";
		this.gv_Advertisement.GridControl = this.grd_Advertisement;
		this.gv_Advertisement.IndicatorWidth = 30;
		this.gv_Advertisement.IsApzTable = false;
		this.gv_Advertisement.IsInsertNewRow = false;
		this.gv_Advertisement.IsLoadingData = false;
		this.gv_Advertisement.ItemId = "APZ_HJV2";
		this.gv_Advertisement.LstColumnAllowBatch = null;
		this.gv_Advertisement.LstColumnNotAllowBatch = null;
		this.gv_Advertisement.LstColumnNotEditable = null;
		this.gv_Advertisement.LstColumNotSetting = null;
		this.gv_Advertisement.Name = "gv_Advertisement";
		this.gv_Advertisement.OptionsView.ShowGroupPanel = false;
		this.gv_Advertisement.UseBatchMod = false;
		this.gv_Advertisement.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Advertisement_RowCellStyle);
		this.gv_Advertisement.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Advertisement_ShowingEditor);
		this.gv_Advertisement.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Advertisement_CellValueChanging);
		this.Ad_DocEntry.Caption = "DocEntry";
		this.Ad_DocEntry.CurrencyDateField = null;
		this.Ad_DocEntry.CurrencyField = null;
		this.Ad_DocEntry.CurrencyFixed = null;
		this.Ad_DocEntry.CurrencyFixedRate = 0.0;
		this.Ad_DocEntry.CurrencyRateField = null;
		this.Ad_DocEntry.FieldName = "DocEntry";
		this.Ad_DocEntry.Name = "Ad_DocEntry";
		this.Ad_DocEntry.OptionsColumn.AllowEdit = false;
		this.Ad_DocEntry.OptionsColumn.ReadOnly = true;
		this.Ad_DocEntry.PrimaryKey = false;
		this.Ad_DocEntry.UnitField = null;
		this.Ad_LineNum.Caption = "LineNum";
		this.Ad_LineNum.CurrencyDateField = null;
		this.Ad_LineNum.CurrencyField = null;
		this.Ad_LineNum.CurrencyFixed = null;
		this.Ad_LineNum.CurrencyFixedRate = 0.0;
		this.Ad_LineNum.CurrencyRateField = null;
		this.Ad_LineNum.FieldName = "LineNum";
		this.Ad_LineNum.Name = "Ad_LineNum";
		this.Ad_LineNum.OptionsColumn.AllowEdit = false;
		this.Ad_LineNum.OptionsColumn.ReadOnly = true;
		this.Ad_LineNum.PrimaryKey = false;
		this.Ad_LineNum.UnitField = null;
		this.Ad_AdvID.Caption = "Adv ID";
		this.Ad_AdvID.CurrencyDateField = null;
		this.Ad_AdvID.CurrencyField = null;
		this.Ad_AdvID.CurrencyFixed = null;
		this.Ad_AdvID.CurrencyFixedRate = 0.0;
		this.Ad_AdvID.CurrencyRateField = null;
		this.Ad_AdvID.FieldName = "AdvID";
		this.Ad_AdvID.Name = "Ad_AdvID";
		this.Ad_AdvID.PrimaryKey = false;
		this.Ad_AdvID.UnitField = null;
		this.Ad_AdvID.Visible = true;
		this.Ad_AdvID.VisibleIndex = 0;
		this.Ad_AdvName.Caption = "Adv Name";
		this.Ad_AdvName.CurrencyDateField = null;
		this.Ad_AdvName.CurrencyField = null;
		this.Ad_AdvName.CurrencyFixed = null;
		this.Ad_AdvName.CurrencyFixedRate = 0.0;
		this.Ad_AdvName.CurrencyRateField = null;
		this.Ad_AdvName.FieldName = "AdvName";
		this.Ad_AdvName.Name = "Ad_AdvName";
		this.Ad_AdvName.PrimaryKey = false;
		this.Ad_AdvName.UnitField = null;
		this.Ad_AdvName.Visible = true;
		this.Ad_AdvName.VisibleIndex = 1;
		this.Ad_AdvSources.Caption = "Adv Sources";
		this.Ad_AdvSources.ColumnEdit = this.rep_lookAdvSource;
		this.Ad_AdvSources.CurrencyDateField = null;
		this.Ad_AdvSources.CurrencyField = null;
		this.Ad_AdvSources.CurrencyFixed = null;
		this.Ad_AdvSources.CurrencyFixedRate = 0.0;
		this.Ad_AdvSources.CurrencyRateField = null;
		this.Ad_AdvSources.FieldName = "AdvSources";
		this.Ad_AdvSources.Name = "Ad_AdvSources";
		this.Ad_AdvSources.PrimaryKey = false;
		this.Ad_AdvSources.UnitField = null;
		this.Ad_AdvSources.Visible = true;
		this.Ad_AdvSources.VisibleIndex = 2;
		this.rep_lookAdvSource.AutoHeight = false;
		this.rep_lookAdvSource.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_lookAdvSource.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookAdvSource.CheckFields = null;
		this.rep_lookAdvSource.CheckTables = null;
		this.rep_lookAdvSource.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_lookAdvSource.Name = "rep_lookAdvSource";
		this.rep_lookAdvSource.NullText = "";
		this.rep_lookAdvSource.ShowFooter = false;
		this.rep_lookAdvSource.ShowHeader = false;
		this.Ad_AdvCost.Caption = "Adv Cost";
		this.Ad_AdvCost.ColumnEdit = this.r_AdvCost;
		this.Ad_AdvCost.CurrencyDateField = null;
		this.Ad_AdvCost.CurrencyField = null;
		this.Ad_AdvCost.CurrencyFixed = null;
		this.Ad_AdvCost.CurrencyFixedRate = 0.0;
		this.Ad_AdvCost.CurrencyRateField = null;
		this.Ad_AdvCost.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.Ad_AdvCost.FieldName = "AdvCost";
		this.Ad_AdvCost.Name = "Ad_AdvCost";
		this.Ad_AdvCost.PrimaryKey = false;
		this.Ad_AdvCost.UnitField = null;
		this.Ad_AdvCost.Visible = true;
		this.Ad_AdvCost.VisibleIndex = 3;
		this.r_AdvCost.AutoHeight = false;
		this.r_AdvCost.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_AdvCost.Name = "r_AdvCost";
		this.Ad_Currency.Caption = "Currency";
		this.Ad_Currency.ColumnEdit = this.rep_lookCurrency;
		this.Ad_Currency.CurrencyDateField = null;
		this.Ad_Currency.CurrencyField = null;
		this.Ad_Currency.CurrencyFixed = null;
		this.Ad_Currency.CurrencyFixedRate = 0.0;
		this.Ad_Currency.CurrencyRateField = null;
		this.Ad_Currency.FieldName = "Currency";
		this.Ad_Currency.Name = "Ad_Currency";
		this.Ad_Currency.PrimaryKey = false;
		this.Ad_Currency.UnitField = null;
		this.Ad_Currency.Visible = true;
		this.Ad_Currency.VisibleIndex = 4;
		this.rep_lookCurrency.AutoHeight = false;
		this.rep_lookCurrency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookCurrency.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookCurrency.DisplayMember = "CurrName";
		this.rep_lookCurrency.Name = "rep_lookCurrency";
		this.rep_lookCurrency.NullText = "";
		this.rep_lookCurrency.ValueMember = "CurrCode";
		this.Ad_FromDate.Caption = "From Date";
		this.Ad_FromDate.ColumnEdit = this.r_AdvFromDate;
		this.Ad_FromDate.CurrencyDateField = null;
		this.Ad_FromDate.CurrencyField = null;
		this.Ad_FromDate.CurrencyFixed = null;
		this.Ad_FromDate.CurrencyFixedRate = 0.0;
		this.Ad_FromDate.CurrencyRateField = null;
		this.Ad_FromDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.Ad_FromDate.FieldName = "FromDate";
		this.Ad_FromDate.Name = "Ad_FromDate";
		this.Ad_FromDate.PrimaryKey = false;
		this.Ad_FromDate.UnitField = null;
		this.Ad_FromDate.Visible = true;
		this.Ad_FromDate.VisibleIndex = 5;
		this.r_AdvFromDate.AutoHeight = false;
		this.r_AdvFromDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_AdvFromDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_AdvFromDate.Name = "r_AdvFromDate";
		this.Ad_ToDate.Caption = "To Date";
		this.Ad_ToDate.ColumnEdit = this.r_AdvToDate;
		this.Ad_ToDate.CurrencyDateField = null;
		this.Ad_ToDate.CurrencyField = null;
		this.Ad_ToDate.CurrencyFixed = null;
		this.Ad_ToDate.CurrencyFixedRate = 0.0;
		this.Ad_ToDate.CurrencyRateField = null;
		this.Ad_ToDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.Ad_ToDate.FieldName = "ToDate";
		this.Ad_ToDate.Name = "Ad_ToDate";
		this.Ad_ToDate.PrimaryKey = false;
		this.Ad_ToDate.UnitField = null;
		this.Ad_ToDate.Visible = true;
		this.Ad_ToDate.VisibleIndex = 6;
		this.r_AdvToDate.AutoHeight = false;
		this.r_AdvToDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_AdvToDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_AdvToDate.Name = "r_AdvToDate";
		this.Ad_Remark.Caption = "Remark";
		this.Ad_Remark.CurrencyDateField = null;
		this.Ad_Remark.CurrencyField = null;
		this.Ad_Remark.CurrencyFixed = null;
		this.Ad_Remark.CurrencyFixedRate = 0.0;
		this.Ad_Remark.CurrencyRateField = null;
		this.Ad_Remark.FieldName = "Remark";
		this.Ad_Remark.Name = "Ad_Remark";
		this.Ad_Remark.PrimaryKey = false;
		this.Ad_Remark.UnitField = null;
		this.Ad_Remark.Visible = true;
		this.Ad_Remark.VisibleIndex = 7;
		this.xtra_APZ_HJV3.Controls.Add(this.grd_App);
		this.xtra_APZ_HJV3.EditControl = null;
		this.xtra_APZ_HJV3.EditValue = "Applicants";
		this.xtra_APZ_HJV3.FieldName = "xtra_APZ_HJV3";
		this.xtra_APZ_HJV3.IsSearchControl = false;
		this.xtra_APZ_HJV3.Name = "xtra_APZ_HJV3";
		this.xtra_APZ_HJV3.ReadOnly = false;
		this.xtra_APZ_HJV3.SettingInVisible = false;
		this.xtra_APZ_HJV3.SettingReadonly = false;
		this.xtra_APZ_HJV3.Size = new System.Drawing.Size(956, 289);
		this.xtra_APZ_HJV3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_APZ_HJV3.Text = "Applicants";
		this.grd_App.AllowPasteExcel = true;
		this.grd_App.ClipboardData = null;
		this.grd_App.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_App.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_App.Location = new System.Drawing.Point(0, 0);
		this.grd_App.MainView = this.gv_App;
		this.grd_App.Name = "grd_App";
		this.grd_App.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.r_AppDoB, this.rep_lookInterviewStatus, this.rep_lookGender, this.rep_CheckedQualification, this.luk_StatusApplican, this.btn_AppNo });
		this.grd_App.Size = new System.Drawing.Size(956, 289);
		this.grd_App.TabIndex = 0;
		this.grd_App.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_App });
		this.gv_App.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_App.ColumnKey = "ApplicantNo";
		this.gv_App.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.Ap_DocEntry, this.Ap_LineNum, this.Ap_ApplicantNo, this.Ap_ApplicantName, this.Ap_DoB, this.Ap_Gender, this.Ap_Qualifications, this.Ap_Address, this.Ap_Phone, this.Email,
			this.Ap_InterviewStatus, this.gridColumnEx16
		});
		this.gv_App.DisableFormSetting = false;
		this.gv_App.DisplayException = false;
		this.gv_App.FieldCurrency = null;
		this.gv_App.FieldRate = null;
		this.gv_App.FormId = "700012";
		this.gv_App.GridControl = this.grd_App;
		this.gv_App.IndicatorWidth = 30;
		this.gv_App.IsApzTable = false;
		this.gv_App.IsInsertNewRow = false;
		this.gv_App.IsLoadingData = false;
		this.gv_App.ItemId = "APZ_HJV3";
		this.gv_App.LstColumnAllowBatch = null;
		this.gv_App.LstColumnNotAllowBatch = null;
		this.gv_App.LstColumnNotEditable = null;
		this.gv_App.LstColumNotSetting = null;
		this.gv_App.Name = "gv_App";
		this.gv_App.OptionsView.ShowGroupPanel = false;
		this.gv_App.UseBatchMod = false;
		this.gv_App.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_App_RowCellStyle);
		this.gv_App.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_App_ShowingEditor);
		this.gv_App.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_App_CellValueChanged);
		this.gv_App.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_App_CellValueChanging);
		this.Ap_DocEntry.Caption = "DocEntry";
		this.Ap_DocEntry.CurrencyDateField = null;
		this.Ap_DocEntry.CurrencyField = null;
		this.Ap_DocEntry.CurrencyFixed = null;
		this.Ap_DocEntry.CurrencyFixedRate = 0.0;
		this.Ap_DocEntry.CurrencyRateField = null;
		this.Ap_DocEntry.FieldName = "DocEntry";
		this.Ap_DocEntry.Name = "Ap_DocEntry";
		this.Ap_DocEntry.OptionsColumn.AllowEdit = false;
		this.Ap_DocEntry.OptionsColumn.ReadOnly = true;
		this.Ap_DocEntry.PrimaryKey = false;
		this.Ap_DocEntry.UnitField = null;
		this.Ap_LineNum.Caption = "LineNum";
		this.Ap_LineNum.CurrencyDateField = null;
		this.Ap_LineNum.CurrencyField = null;
		this.Ap_LineNum.CurrencyFixed = null;
		this.Ap_LineNum.CurrencyFixedRate = 0.0;
		this.Ap_LineNum.CurrencyRateField = null;
		this.Ap_LineNum.FieldName = "LineNum";
		this.Ap_LineNum.Name = "Ap_LineNum";
		this.Ap_LineNum.OptionsColumn.AllowEdit = false;
		this.Ap_LineNum.OptionsColumn.ReadOnly = true;
		this.Ap_LineNum.PrimaryKey = false;
		this.Ap_LineNum.UnitField = null;
		this.Ap_ApplicantNo.Caption = "Applicant No";
		this.Ap_ApplicantNo.ColumnEdit = this.btn_AppNo;
		this.Ap_ApplicantNo.CurrencyDateField = null;
		this.Ap_ApplicantNo.CurrencyField = null;
		this.Ap_ApplicantNo.CurrencyFixed = null;
		this.Ap_ApplicantNo.CurrencyFixedRate = 0.0;
		this.Ap_ApplicantNo.CurrencyRateField = null;
		this.Ap_ApplicantNo.FieldName = "ApplicantNo";
		this.Ap_ApplicantNo.Name = "Ap_ApplicantNo";
		this.Ap_ApplicantNo.OptionsColumn.ReadOnly = true;
		this.Ap_ApplicantNo.PrimaryKey = false;
		this.Ap_ApplicantNo.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.Ap_ApplicantNo.UnitField = null;
		this.Ap_ApplicantNo.Visible = true;
		this.Ap_ApplicantNo.VisibleIndex = 0;
		this.btn_AppNo.AutoHeight = false;
		this.btn_AppNo.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions13.Image");
		this.btn_AppNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_AppNo.ContainerControl = this;
		this.btn_AppNo.DisplayMember = "";
		this.btn_AppNo.DisplayMember2 = "";
		this.btn_AppNo.ListMandatoryField = null;
		this.btn_AppNo.LoadDataSourceWhenEnter = false;
		this.btn_AppNo.Name = "btn_AppNo";
		this.btn_AppNo.ReadOnly = true;
		this.btn_AppNo.SeparateCharacterForMulti = ';';
		this.btn_AppNo.SeparateString = ", ";
		this.btn_AppNo.UsingAutoCompleteBox = false;
		this.btn_AppNo.UsingDataSource = true;
		this.btn_AppNo.ValidateWhenNoDataSource = false;
		this.btn_AppNo.ValueMember = "";
		this.Ap_ApplicantName.Caption = "Applicant Name";
		this.Ap_ApplicantName.CurrencyDateField = null;
		this.Ap_ApplicantName.CurrencyField = null;
		this.Ap_ApplicantName.CurrencyFixed = null;
		this.Ap_ApplicantName.CurrencyFixedRate = 0.0;
		this.Ap_ApplicantName.CurrencyRateField = null;
		this.Ap_ApplicantName.FieldName = "ApplicantName";
		this.Ap_ApplicantName.Name = "Ap_ApplicantName";
		this.Ap_ApplicantName.OptionsColumn.ReadOnly = true;
		this.Ap_ApplicantName.PrimaryKey = false;
		this.Ap_ApplicantName.UnitField = null;
		this.Ap_ApplicantName.Visible = true;
		this.Ap_ApplicantName.VisibleIndex = 1;
		this.Ap_DoB.Caption = "DoB";
		this.Ap_DoB.ColumnEdit = this.r_AppDoB;
		this.Ap_DoB.CurrencyDateField = null;
		this.Ap_DoB.CurrencyField = null;
		this.Ap_DoB.CurrencyFixed = null;
		this.Ap_DoB.CurrencyFixedRate = 0.0;
		this.Ap_DoB.CurrencyRateField = null;
		this.Ap_DoB.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.Ap_DoB.FieldName = "DoB";
		this.Ap_DoB.Name = "Ap_DoB";
		this.Ap_DoB.OptionsColumn.ReadOnly = true;
		this.Ap_DoB.PrimaryKey = false;
		this.Ap_DoB.UnitField = null;
		this.Ap_DoB.Visible = true;
		this.Ap_DoB.VisibleIndex = 2;
		this.r_AppDoB.AutoHeight = false;
		this.r_AppDoB.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_AppDoB.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_AppDoB.Name = "r_AppDoB";
		this.Ap_Gender.Caption = "Gender";
		this.Ap_Gender.ColumnEdit = this.rep_lookGender;
		this.Ap_Gender.CurrencyDateField = null;
		this.Ap_Gender.CurrencyField = null;
		this.Ap_Gender.CurrencyFixed = null;
		this.Ap_Gender.CurrencyFixedRate = 0.0;
		this.Ap_Gender.CurrencyRateField = null;
		this.Ap_Gender.FieldName = "Gender";
		this.Ap_Gender.Name = "Ap_Gender";
		this.Ap_Gender.OptionsColumn.ReadOnly = true;
		this.Ap_Gender.PrimaryKey = false;
		this.Ap_Gender.UnitField = null;
		this.Ap_Gender.Visible = true;
		this.Ap_Gender.VisibleIndex = 3;
		this.rep_lookGender.AutoHeight = false;
		this.rep_lookGender.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookGender.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookGender.DisplayMember = "Name";
		this.rep_lookGender.Name = "rep_lookGender";
		this.rep_lookGender.NullText = "";
		this.rep_lookGender.ValueMember = "Code";
		this.Ap_Qualifications.Caption = "Qualifications";
		this.Ap_Qualifications.ColumnEdit = this.rep_CheckedQualification;
		this.Ap_Qualifications.CurrencyDateField = null;
		this.Ap_Qualifications.CurrencyField = null;
		this.Ap_Qualifications.CurrencyFixed = null;
		this.Ap_Qualifications.CurrencyFixedRate = 0.0;
		this.Ap_Qualifications.CurrencyRateField = null;
		this.Ap_Qualifications.FieldName = "Qualifications";
		this.Ap_Qualifications.Name = "Ap_Qualifications";
		this.Ap_Qualifications.OptionsColumn.ReadOnly = true;
		this.Ap_Qualifications.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.Ap_Qualifications.PrimaryKey = false;
		this.Ap_Qualifications.UnitField = null;
		this.Ap_Qualifications.Visible = true;
		this.Ap_Qualifications.VisibleIndex = 4;
		this.rep_CheckedQualification.AutoHeight = false;
		this.rep_CheckedQualification.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CheckedQualification.Name = "rep_CheckedQualification";
		this.Ap_Address.Caption = "Address";
		this.Ap_Address.CurrencyDateField = null;
		this.Ap_Address.CurrencyField = null;
		this.Ap_Address.CurrencyFixed = null;
		this.Ap_Address.CurrencyFixedRate = 0.0;
		this.Ap_Address.CurrencyRateField = null;
		this.Ap_Address.FieldName = "Address";
		this.Ap_Address.Name = "Ap_Address";
		this.Ap_Address.OptionsColumn.ReadOnly = true;
		this.Ap_Address.PrimaryKey = false;
		this.Ap_Address.UnitField = null;
		this.Ap_Address.Visible = true;
		this.Ap_Address.VisibleIndex = 5;
		this.Ap_Phone.Caption = "Phone";
		this.Ap_Phone.CurrencyDateField = null;
		this.Ap_Phone.CurrencyField = null;
		this.Ap_Phone.CurrencyFixed = null;
		this.Ap_Phone.CurrencyFixedRate = 0.0;
		this.Ap_Phone.CurrencyRateField = null;
		this.Ap_Phone.FieldName = "Phone";
		this.Ap_Phone.Name = "Ap_Phone";
		this.Ap_Phone.OptionsColumn.ReadOnly = true;
		this.Ap_Phone.PrimaryKey = false;
		this.Ap_Phone.UnitField = null;
		this.Ap_Phone.Visible = true;
		this.Ap_Phone.VisibleIndex = 6;
		this.Email.Caption = "Email";
		this.Email.CurrencyDateField = null;
		this.Email.CurrencyField = null;
		this.Email.CurrencyFixed = null;
		this.Email.CurrencyFixedRate = 0.0;
		this.Email.CurrencyRateField = null;
		this.Email.FieldName = "Email";
		this.Email.Name = "Email";
		this.Email.OptionsColumn.ReadOnly = true;
		this.Email.PrimaryKey = false;
		this.Email.UnitField = null;
		this.Email.Visible = true;
		this.Email.VisibleIndex = 7;
		this.Ap_InterviewStatus.Caption = "Interview Status";
		this.Ap_InterviewStatus.ColumnEdit = this.luk_StatusApplican;
		this.Ap_InterviewStatus.CurrencyDateField = null;
		this.Ap_InterviewStatus.CurrencyField = null;
		this.Ap_InterviewStatus.CurrencyFixed = null;
		this.Ap_InterviewStatus.CurrencyFixedRate = 0.0;
		this.Ap_InterviewStatus.CurrencyRateField = null;
		this.Ap_InterviewStatus.FieldName = "InterviewStatus";
		this.Ap_InterviewStatus.Name = "Ap_InterviewStatus";
		this.Ap_InterviewStatus.PrimaryKey = false;
		this.Ap_InterviewStatus.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.Ap_InterviewStatus.UnitField = null;
		this.Ap_InterviewStatus.Visible = true;
		this.Ap_InterviewStatus.VisibleIndex = 8;
		this.luk_StatusApplican.AutoHeight = false;
		this.luk_StatusApplican.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusApplican.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusApplican.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_StatusApplican.DisplayMember = "Name";
		this.luk_StatusApplican.IsSearchControl = false;
		this.luk_StatusApplican.Name = "luk_StatusApplican";
		this.luk_StatusApplican.NullText = "";
		this.luk_StatusApplican.ShowFooter = false;
		this.luk_StatusApplican.ShowHeader = false;
		this.luk_StatusApplican.ValueMember = "Code";
		this.gridColumnEx16.Caption = "ApplicantID";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "ApplicantID";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.OptionsColumn.ReadOnly = true;
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.rep_lookInterviewStatus.AutoHeight = false;
		this.rep_lookInterviewStatus.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_lookInterviewStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookInterviewStatus.CheckFields = null;
		this.rep_lookInterviewStatus.CheckTables = null;
		this.rep_lookInterviewStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Id", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookInterviewStatus.DisplayMember = "Name";
		this.rep_lookInterviewStatus.Name = "rep_lookInterviewStatus";
		this.rep_lookInterviewStatus.NullText = "";
		this.rep_lookInterviewStatus.ShowFooter = false;
		this.rep_lookInterviewStatus.ShowHeader = false;
		this.rep_lookInterviewStatus.ValueMember = "Id";
		this.xtra_APZ_HJV4.Controls.Add(this.splitContainerControl2);
		this.xtra_APZ_HJV4.EditControl = null;
		this.xtra_APZ_HJV4.EditValue = "Interview";
		this.xtra_APZ_HJV4.FieldName = "xtra_APZ_HJV4";
		this.xtra_APZ_HJV4.IsSearchControl = false;
		this.xtra_APZ_HJV4.Name = "xtra_APZ_HJV4";
		this.xtra_APZ_HJV4.ReadOnly = false;
		this.xtra_APZ_HJV4.SettingInVisible = false;
		this.xtra_APZ_HJV4.SettingReadonly = false;
		this.xtra_APZ_HJV4.Size = new System.Drawing.Size(956, 289);
		this.xtra_APZ_HJV4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_APZ_HJV4.Text = "Interview";
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_Inte);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.grd_Applications);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(956, 289);
		this.splitContainerControl2.SplitterPosition = 149;
		this.splitContainerControl2.TabIndex = 3;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.grd_Inte.AllowPasteExcel = true;
		this.grd_Inte.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Inte.ClipboardData = null;
		this.grd_Inte.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Inte.Location = new System.Drawing.Point(0, 0);
		this.grd_Inte.MainView = this.gv_Inte;
		this.grd_Inte.Name = "grd_Inte";
		this.grd_Inte.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.rep_lookInteType, this.rep_lookStatus, this.rep_dateDateInter, this.rep_btnTeam, this.rep_btnQues, this.rep_btnAppInInter, this.r_checkTeam });
		this.grd_Inte.Size = new System.Drawing.Size(956, 149);
		this.grd_Inte.TabIndex = 0;
		this.grd_Inte.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Inte });
		this.gv_Inte.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Inte.ColumnKey = null;
		this.gv_Inte.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.Inte_DocEntry, this.Inte_LineNum, this.Inte_InterviewID, this.Inte_InterviewName, this.Inte_InterviewType, this.Inte_QuestionsLibrary, this.Inte_InterviewTeam, this.Inte_VisOrder, this.Inte_Status, this.Inte_Date });
		this.gv_Inte.DisableFormSetting = false;
		this.gv_Inte.DisplayException = false;
		this.gv_Inte.FieldCurrency = null;
		this.gv_Inte.FieldRate = null;
		this.gv_Inte.FormId = "700012";
		this.gv_Inte.GridControl = this.grd_Inte;
		this.gv_Inte.IndicatorWidth = 30;
		this.gv_Inte.IsApzTable = false;
		this.gv_Inte.IsInsertNewRow = false;
		this.gv_Inte.IsLoadingData = false;
		this.gv_Inte.ItemId = "APZ_HJV4";
		this.gv_Inte.LstColumnAllowBatch = null;
		this.gv_Inte.LstColumnNotAllowBatch = null;
		this.gv_Inte.LstColumnNotEditable = null;
		this.gv_Inte.LstColumNotSetting = null;
		this.gv_Inte.Name = "gv_Inte";
		this.gv_Inte.OptionsView.ShowGroupPanel = false;
		this.gv_Inte.UseBatchMod = false;
		this.gv_Inte.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Inte_RowCellStyle);
		this.gv_Inte.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Inte_ShowingEditor);
		this.gv_Inte.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Inte_FocusedRowObjectChanged);
		this.gv_Inte.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Inte_CellValueChanged);
		this.gv_Inte.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Inte_CellValueChanging);
		this.gv_Inte.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Inte_BeforeLeaveRow);
		this.Inte_DocEntry.Caption = "DocEntry";
		this.Inte_DocEntry.CurrencyDateField = null;
		this.Inte_DocEntry.CurrencyField = null;
		this.Inte_DocEntry.CurrencyFixed = null;
		this.Inte_DocEntry.CurrencyFixedRate = 0.0;
		this.Inte_DocEntry.CurrencyRateField = null;
		this.Inte_DocEntry.FieldName = "DocEntry";
		this.Inte_DocEntry.Name = "Inte_DocEntry";
		this.Inte_DocEntry.OptionsColumn.AllowEdit = false;
		this.Inte_DocEntry.OptionsColumn.ReadOnly = true;
		this.Inte_DocEntry.PrimaryKey = false;
		this.Inte_DocEntry.UnitField = null;
		this.Inte_LineNum.Caption = "LineNum";
		this.Inte_LineNum.CurrencyDateField = null;
		this.Inte_LineNum.CurrencyField = null;
		this.Inte_LineNum.CurrencyFixed = null;
		this.Inte_LineNum.CurrencyFixedRate = 0.0;
		this.Inte_LineNum.CurrencyRateField = null;
		this.Inte_LineNum.FieldName = "LineNum";
		this.Inte_LineNum.Name = "Inte_LineNum";
		this.Inte_LineNum.OptionsColumn.AllowEdit = false;
		this.Inte_LineNum.OptionsColumn.ReadOnly = true;
		this.Inte_LineNum.PrimaryKey = false;
		this.Inte_LineNum.UnitField = null;
		this.Inte_InterviewID.Caption = "Interview ID";
		this.Inte_InterviewID.CurrencyDateField = null;
		this.Inte_InterviewID.CurrencyField = null;
		this.Inte_InterviewID.CurrencyFixed = null;
		this.Inte_InterviewID.CurrencyFixedRate = 0.0;
		this.Inte_InterviewID.CurrencyRateField = null;
		this.Inte_InterviewID.FieldName = "InterviewID";
		this.Inte_InterviewID.Name = "Inte_InterviewID";
		this.Inte_InterviewID.PrimaryKey = false;
		this.Inte_InterviewID.UnitField = null;
		this.Inte_InterviewID.Visible = true;
		this.Inte_InterviewID.VisibleIndex = 0;
		this.Inte_InterviewName.Caption = "Interview Name";
		this.Inte_InterviewName.CurrencyDateField = null;
		this.Inte_InterviewName.CurrencyField = null;
		this.Inte_InterviewName.CurrencyFixed = null;
		this.Inte_InterviewName.CurrencyFixedRate = 0.0;
		this.Inte_InterviewName.CurrencyRateField = null;
		this.Inte_InterviewName.FieldName = "InterviewName";
		this.Inte_InterviewName.Name = "Inte_InterviewName";
		this.Inte_InterviewName.PrimaryKey = false;
		this.Inte_InterviewName.UnitField = null;
		this.Inte_InterviewName.Visible = true;
		this.Inte_InterviewName.VisibleIndex = 1;
		this.Inte_InterviewType.Caption = "Interview Type";
		this.Inte_InterviewType.ColumnEdit = this.rep_lookInteType;
		this.Inte_InterviewType.CurrencyDateField = null;
		this.Inte_InterviewType.CurrencyField = null;
		this.Inte_InterviewType.CurrencyFixed = null;
		this.Inte_InterviewType.CurrencyFixedRate = 0.0;
		this.Inte_InterviewType.CurrencyRateField = null;
		this.Inte_InterviewType.FieldName = "InterviewType";
		this.Inte_InterviewType.Name = "Inte_InterviewType";
		this.Inte_InterviewType.PrimaryKey = false;
		this.Inte_InterviewType.UnitField = null;
		this.Inte_InterviewType.Visible = true;
		this.Inte_InterviewType.VisibleIndex = 2;
		this.rep_lookInteType.AutoHeight = false;
		this.rep_lookInteType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_lookInteType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookInteType.CheckFields = null;
		this.rep_lookInteType.CheckTables = null;
		this.rep_lookInteType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Id", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookInteType.DisplayMember = "Name";
		this.rep_lookInteType.Name = "rep_lookInteType";
		this.rep_lookInteType.NullText = "";
		this.rep_lookInteType.ShowFooter = false;
		this.rep_lookInteType.ShowHeader = false;
		this.rep_lookInteType.ValueMember = "Value";
		this.Inte_QuestionsLibrary.Caption = "Questions Library";
		this.Inte_QuestionsLibrary.ColumnEdit = this.rep_btnQues;
		this.Inte_QuestionsLibrary.CurrencyDateField = null;
		this.Inte_QuestionsLibrary.CurrencyField = null;
		this.Inte_QuestionsLibrary.CurrencyFixed = null;
		this.Inte_QuestionsLibrary.CurrencyFixedRate = 0.0;
		this.Inte_QuestionsLibrary.CurrencyRateField = null;
		this.Inte_QuestionsLibrary.FieldName = "QuestionsLibrary";
		this.Inte_QuestionsLibrary.Name = "Inte_QuestionsLibrary";
		this.Inte_QuestionsLibrary.PrimaryKey = false;
		this.Inte_QuestionsLibrary.UnitField = null;
		this.Inte_QuestionsLibrary.Visible = true;
		this.Inte_QuestionsLibrary.VisibleIndex = 3;
		this.rep_btnQues.AutoHeight = false;
		this.rep_btnQues.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_btnQues.Name = "rep_btnQues";
		this.rep_btnQues.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_btnQues_ButtonClick);
		this.Inte_InterviewTeam.Caption = "Interview Team";
		this.Inte_InterviewTeam.ColumnEdit = this.rep_btnTeam;
		this.Inte_InterviewTeam.CurrencyDateField = null;
		this.Inte_InterviewTeam.CurrencyField = null;
		this.Inte_InterviewTeam.CurrencyFixed = null;
		this.Inte_InterviewTeam.CurrencyFixedRate = 0.0;
		this.Inte_InterviewTeam.CurrencyRateField = null;
		this.Inte_InterviewTeam.FieldName = "InterviewTeam";
		this.Inte_InterviewTeam.Name = "Inte_InterviewTeam";
		this.Inte_InterviewTeam.PrimaryKey = false;
		this.Inte_InterviewTeam.UnitField = null;
		this.Inte_InterviewTeam.Visible = true;
		this.Inte_InterviewTeam.VisibleIndex = 4;
		this.rep_btnTeam.AutoHeight = false;
		this.rep_btnTeam.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_btnTeam.Name = "rep_btnTeam";
		this.rep_btnTeam.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_btnTeam_ButtonClick);
		this.Inte_VisOrder.Caption = "Vis Order";
		this.Inte_VisOrder.CurrencyDateField = null;
		this.Inte_VisOrder.CurrencyField = null;
		this.Inte_VisOrder.CurrencyFixed = null;
		this.Inte_VisOrder.CurrencyFixedRate = 0.0;
		this.Inte_VisOrder.CurrencyRateField = null;
		this.Inte_VisOrder.FieldName = "VisOrder";
		this.Inte_VisOrder.Name = "Inte_VisOrder";
		this.Inte_VisOrder.PrimaryKey = false;
		this.Inte_VisOrder.UnitField = null;
		this.Inte_VisOrder.Visible = true;
		this.Inte_VisOrder.VisibleIndex = 6;
		this.Inte_Status.Caption = "Status";
		this.Inte_Status.ColumnEdit = this.rep_lookStatus;
		this.Inte_Status.CurrencyDateField = null;
		this.Inte_Status.CurrencyField = null;
		this.Inte_Status.CurrencyFixed = null;
		this.Inte_Status.CurrencyFixedRate = 0.0;
		this.Inte_Status.CurrencyRateField = null;
		this.Inte_Status.FieldName = "Status";
		this.Inte_Status.Name = "Inte_Status";
		this.Inte_Status.PrimaryKey = false;
		this.Inte_Status.UnitField = null;
		this.Inte_Status.Visible = true;
		this.Inte_Status.VisibleIndex = 7;
		this.rep_lookStatus.AutoHeight = false;
		this.rep_lookStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookStatus.DisplayMember = "Name";
		this.rep_lookStatus.Name = "rep_lookStatus";
		this.rep_lookStatus.NullText = "";
		this.rep_lookStatus.ValueMember = "Code";
		this.Inte_Date.Caption = "Date";
		this.Inte_Date.ColumnEdit = this.rep_dateDateInter;
		this.Inte_Date.CurrencyDateField = null;
		this.Inte_Date.CurrencyField = null;
		this.Inte_Date.CurrencyFixed = null;
		this.Inte_Date.CurrencyFixedRate = 0.0;
		this.Inte_Date.CurrencyRateField = null;
		this.Inte_Date.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.Inte_Date.FieldName = "Date";
		this.Inte_Date.Name = "Inte_Date";
		this.Inte_Date.PrimaryKey = false;
		this.Inte_Date.UnitField = null;
		this.Inte_Date.Visible = true;
		this.Inte_Date.VisibleIndex = 5;
		this.rep_dateDateInter.AutoHeight = false;
		this.rep_dateDateInter.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateDateInter.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateDateInter.Name = "rep_dateDateInter";
		this.rep_btnAppInInter.Name = "rep_btnAppInInter";
		this.r_checkTeam.AutoHeight = false;
		this.r_checkTeam.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_checkTeam.Name = "r_checkTeam";
		this.grd_Applications.AllowPasteExcel = true;
		this.grd_Applications.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Applications.ClipboardData = null;
		this.grd_Applications.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_Applications.Location = new System.Drawing.Point(0, 0);
		this.grd_Applications.MainView = this.gv_Applications;
		this.grd_Applications.Name = "grd_Applications";
		this.grd_Applications.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.r_appInIter, this.r_Gender2, this.r_Qualification2, this.luk_StatusApp });
		this.grd_Applications.Size = new System.Drawing.Size(956, 135);
		this.grd_Applications.TabIndex = 2;
		this.grd_Applications.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Applications });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem1, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(134, 48);
		this.addRowToolStripMenuItem1.Name = "addRowToolStripMenuItem1";
		this.addRowToolStripMenuItem1.Size = new System.Drawing.Size(133, 22);
		this.addRowToolStripMenuItem1.Text = "Add Row";
		this.addRowToolStripMenuItem1.Click += new System.EventHandler(this.addRowToolStripMenuItem1_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Applications.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Applications.ColumnKey = null;
		this.gv_Applications.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10,
			this.gridColumnEx11, this.gridColumnEx13, this.gridColumnEx17
		});
		this.gv_Applications.DisableFormSetting = false;
		this.gv_Applications.DisplayException = false;
		this.gv_Applications.FieldCurrency = null;
		this.gv_Applications.FieldRate = null;
		this.gv_Applications.FormId = "700012";
		this.gv_Applications.GridControl = this.grd_Applications;
		this.gv_Applications.IndicatorWidth = 30;
		this.gv_Applications.IsApzTable = false;
		this.gv_Applications.IsInsertNewRow = false;
		this.gv_Applications.IsLoadingData = false;
		this.gv_Applications.ItemId = "APZ_HJV8";
		this.gv_Applications.LstColumnAllowBatch = null;
		this.gv_Applications.LstColumnNotAllowBatch = null;
		this.gv_Applications.LstColumnNotEditable = null;
		this.gv_Applications.LstColumNotSetting = null;
		this.gv_Applications.Name = "gv_Applications";
		this.gv_Applications.OptionsView.ShowGroupPanel = false;
		this.gv_Applications.UseBatchMod = false;
		this.gv_Applications.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Applications_RowCellStyle);
		this.gv_Applications.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Applications_ShowingEditor);
		this.gv_Applications.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Applications_CellValueChanged);
		this.gv_Applications.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Applications_CellValueChanging);
		this.gridColumnEx1.Caption = "DocEntry";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "DocEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.AllowEdit = false;
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "LineNum";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "LineNum";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.AllowEdit = false;
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx3.Caption = "Applicant No";
		this.gridColumnEx3.ColumnEdit = this.r_appInIter;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "ApplicantNo";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 0;
		this.gridColumnEx3.Width = 172;
		this.r_appInIter.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.r_appInIter.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.r_appInIter.Name = "r_appInIter";
		this.r_appInIter.ReadOnly = true;
		this.r_appInIter.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.r_appInIter_ButtonClick);
		this.gridColumnEx4.Caption = "Applicant Name";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "ApplicantName";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 1;
		this.gridColumnEx4.Width = 220;
		this.gridColumnEx5.Caption = "DoB";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx5.FieldName = "DoB";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 5;
		this.gridColumnEx5.Width = 165;
		this.gridColumnEx6.Caption = "Gender";
		this.gridColumnEx6.ColumnEdit = this.r_Gender2;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Gender";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 6;
		this.gridColumnEx6.Width = 165;
		this.r_Gender2.AutoHeight = false;
		this.r_Gender2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_Gender2.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.r_Gender2.DisplayMember = "Name";
		this.r_Gender2.Name = "r_Gender2";
		this.r_Gender2.NullText = "";
		this.r_Gender2.ValueMember = "Code";
		this.gridColumnEx7.Caption = "Qualifications";
		this.gridColumnEx7.ColumnEdit = this.r_Qualification2;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "Qualifications";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 4;
		this.gridColumnEx7.Width = 165;
		this.r_Qualification2.AutoHeight = false;
		this.r_Qualification2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.r_Qualification2.Name = "r_Qualification2";
		this.gridColumnEx8.Caption = "Address";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "Address";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.OptionsColumn.ReadOnly = true;
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 3;
		this.gridColumnEx8.Width = 165;
		this.gridColumnEx9.Caption = "Phone";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "Phone";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.OptionsColumn.ReadOnly = true;
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 2;
		this.gridColumnEx9.Width = 165;
		this.gridColumnEx10.Caption = "Email";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "Email";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 7;
		this.gridColumnEx10.Width = 165;
		this.gridColumnEx11.Caption = "LineInterview";
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "LineInterview";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.OptionsColumn.ReadOnly = true;
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx13.Caption = "Status";
		this.gridColumnEx13.ColumnEdit = this.luk_StatusApp;
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "Status";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 8;
		this.gridColumnEx13.Width = 167;
		this.luk_StatusApp.AutoHeight = false;
		this.luk_StatusApp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusApp.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_StatusApp.DisplayMember = "Name";
		this.luk_StatusApp.Name = "luk_StatusApp";
		this.luk_StatusApp.NullText = "";
		this.luk_StatusApp.ValueMember = "Code";
		this.gridColumnEx17.Caption = "ApplicantID";
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "ApplicantID";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.OptionsColumn.ReadOnly = true;
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.xtra_APZ_HJV5.Controls.Add(this.grd_Offer);
		this.xtra_APZ_HJV5.EditControl = null;
		this.xtra_APZ_HJV5.EditValue = "Offer";
		this.xtra_APZ_HJV5.FieldName = "xtra_APZ_HJV5";
		this.xtra_APZ_HJV5.IsSearchControl = false;
		this.xtra_APZ_HJV5.Name = "xtra_APZ_HJV5";
		this.xtra_APZ_HJV5.ReadOnly = false;
		this.xtra_APZ_HJV5.SettingInVisible = false;
		this.xtra_APZ_HJV5.SettingReadonly = false;
		this.xtra_APZ_HJV5.Size = new System.Drawing.Size(956, 289);
		this.xtra_APZ_HJV5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_APZ_HJV5.Text = "Offer";
		this.grd_Offer.AllowPasteExcel = true;
		this.grd_Offer.ClipboardData = null;
		this.grd_Offer.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Offer.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Offer.Location = new System.Drawing.Point(0, 0);
		this.grd_Offer.MainView = this.gv_Offer;
		this.grd_Offer.Name = "grd_Offer";
		this.grd_Offer.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_checkBenefits, this.rep_looTrialCur, this.rep_lookContractCur, this.luk_StatusOff, this.btn_AppInOffer });
		this.grd_Offer.Size = new System.Drawing.Size(956, 289);
		this.grd_Offer.TabIndex = 0;
		this.grd_Offer.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Offer });
		this.gv_Offer.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Offer.ColumnKey = "ApplicantNo";
		this.gv_Offer.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.Of_DocEntry, this.Of_LineNum, this.Of_ApplicantName, this.Of_OfferLetter, this.Of_TrialPeriodSalary, this.Of_ContractSalary, this.Of_OtherBenefits, this.Offer_TrialCur, this.of_ContCur, this.gridColumnEx14,
			this.Of_ApplicantNo
		});
		this.gv_Offer.DisableFormSetting = false;
		this.gv_Offer.DisplayException = false;
		this.gv_Offer.FieldCurrency = null;
		this.gv_Offer.FieldRate = null;
		this.gv_Offer.FormId = "700012";
		this.gv_Offer.GridControl = this.grd_Offer;
		this.gv_Offer.IndicatorWidth = 30;
		this.gv_Offer.IsApzTable = false;
		this.gv_Offer.IsInsertNewRow = false;
		this.gv_Offer.IsLoadingData = false;
		this.gv_Offer.ItemId = "APZ_HJV5";
		this.gv_Offer.LstColumnAllowBatch = null;
		this.gv_Offer.LstColumnNotAllowBatch = null;
		this.gv_Offer.LstColumnNotEditable = null;
		this.gv_Offer.LstColumNotSetting = null;
		this.gv_Offer.Name = "gv_Offer";
		this.gv_Offer.OptionsBehavior.EditorShowMode = DevExpress.Utils.EditorShowMode.MouseDown;
		this.gv_Offer.OptionsSelection.MultiSelect = true;
		this.gv_Offer.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Offer.OptionsView.ShowGroupPanel = false;
		this.gv_Offer.UseBatchMod = false;
		this.gv_Offer.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Offer_RowCellStyle);
		this.gv_Offer.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Offer_ShowingEditor);
		this.gv_Offer.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Offer_CellValueChanging);
		this.Of_DocEntry.Caption = "DocEntry";
		this.Of_DocEntry.CurrencyDateField = null;
		this.Of_DocEntry.CurrencyField = null;
		this.Of_DocEntry.CurrencyFixed = null;
		this.Of_DocEntry.CurrencyFixedRate = 0.0;
		this.Of_DocEntry.CurrencyRateField = null;
		this.Of_DocEntry.FieldName = "DocEntry";
		this.Of_DocEntry.Name = "Of_DocEntry";
		this.Of_DocEntry.OptionsColumn.AllowEdit = false;
		this.Of_DocEntry.OptionsColumn.ReadOnly = true;
		this.Of_DocEntry.PrimaryKey = false;
		this.Of_DocEntry.UnitField = null;
		this.Of_LineNum.Caption = "LineNum";
		this.Of_LineNum.CurrencyDateField = null;
		this.Of_LineNum.CurrencyField = null;
		this.Of_LineNum.CurrencyFixed = null;
		this.Of_LineNum.CurrencyFixedRate = 0.0;
		this.Of_LineNum.CurrencyRateField = null;
		this.Of_LineNum.FieldName = "LineNum";
		this.Of_LineNum.Name = "Of_LineNum";
		this.Of_LineNum.OptionsColumn.AllowEdit = false;
		this.Of_LineNum.OptionsColumn.ReadOnly = true;
		this.Of_LineNum.PrimaryKey = false;
		this.Of_LineNum.UnitField = null;
		this.Of_ApplicantName.Caption = "Applicant Name";
		this.Of_ApplicantName.CurrencyDateField = null;
		this.Of_ApplicantName.CurrencyField = null;
		this.Of_ApplicantName.CurrencyFixed = null;
		this.Of_ApplicantName.CurrencyFixedRate = 0.0;
		this.Of_ApplicantName.CurrencyRateField = null;
		this.Of_ApplicantName.FieldName = "ApplicantName";
		this.Of_ApplicantName.Name = "Of_ApplicantName";
		this.Of_ApplicantName.OptionsColumn.ReadOnly = true;
		this.Of_ApplicantName.PrimaryKey = false;
		this.Of_ApplicantName.UnitField = null;
		this.Of_ApplicantName.Visible = true;
		this.Of_ApplicantName.VisibleIndex = 2;
		this.Of_ApplicantName.Width = 110;
		this.Of_OfferLetter.Caption = "Offer Letter";
		this.Of_OfferLetter.CurrencyDateField = null;
		this.Of_OfferLetter.CurrencyField = null;
		this.Of_OfferLetter.CurrencyFixed = null;
		this.Of_OfferLetter.CurrencyFixedRate = 0.0;
		this.Of_OfferLetter.CurrencyRateField = null;
		this.Of_OfferLetter.FieldName = "OfferLetter";
		this.Of_OfferLetter.Name = "Of_OfferLetter";
		this.Of_OfferLetter.PrimaryKey = false;
		this.Of_OfferLetter.UnitField = null;
		this.Of_OfferLetter.Width = 93;
		this.Of_TrialPeriodSalary.Caption = "Trial Period Salary";
		this.Of_TrialPeriodSalary.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.Of_TrialPeriodSalary.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.Of_TrialPeriodSalary.CurrencyDateField = null;
		this.Of_TrialPeriodSalary.CurrencyField = "TrialCur";
		this.Of_TrialPeriodSalary.CurrencyFixed = null;
		this.Of_TrialPeriodSalary.CurrencyFixedRate = 0.0;
		this.Of_TrialPeriodSalary.CurrencyRateField = null;
		this.Of_TrialPeriodSalary.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.Of_TrialPeriodSalary.FieldName = "TrialPeriodSalary";
		this.Of_TrialPeriodSalary.Name = "Of_TrialPeriodSalary";
		this.Of_TrialPeriodSalary.PrimaryKey = false;
		this.Of_TrialPeriodSalary.UnitField = null;
		this.Of_TrialPeriodSalary.Visible = true;
		this.Of_TrialPeriodSalary.VisibleIndex = 3;
		this.Of_TrialPeriodSalary.Width = 111;
		this.Of_ContractSalary.Caption = "Contract Salary";
		this.Of_ContractSalary.ColumnEdit = this.repositoryItemTextEditExPrice2;
		this.Of_ContractSalary.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.Of_ContractSalary.CurrencyDateField = null;
		this.Of_ContractSalary.CurrencyField = "ContCur";
		this.Of_ContractSalary.CurrencyFixed = null;
		this.Of_ContractSalary.CurrencyFixedRate = 0.0;
		this.Of_ContractSalary.CurrencyRateField = null;
		this.Of_ContractSalary.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.Of_ContractSalary.FieldName = "ContractSalary";
		this.Of_ContractSalary.Name = "Of_ContractSalary";
		this.Of_ContractSalary.PrimaryKey = false;
		this.Of_ContractSalary.UnitField = null;
		this.Of_ContractSalary.Visible = true;
		this.Of_ContractSalary.VisibleIndex = 4;
		this.Of_ContractSalary.Width = 150;
		this.Of_OtherBenefits.Caption = "Other Benefits";
		this.Of_OtherBenefits.ColumnEdit = this.rep_checkBenefits;
		this.Of_OtherBenefits.CurrencyDateField = null;
		this.Of_OtherBenefits.CurrencyField = null;
		this.Of_OtherBenefits.CurrencyFixed = null;
		this.Of_OtherBenefits.CurrencyFixedRate = 0.0;
		this.Of_OtherBenefits.CurrencyRateField = null;
		this.Of_OtherBenefits.FieldName = "OtherBenefits";
		this.Of_OtherBenefits.Name = "Of_OtherBenefits";
		this.Of_OtherBenefits.PrimaryKey = false;
		this.Of_OtherBenefits.UnitField = null;
		this.Of_OtherBenefits.Visible = true;
		this.Of_OtherBenefits.VisibleIndex = 5;
		this.Of_OtherBenefits.Width = 116;
		this.rep_checkBenefits.AutoHeight = false;
		this.rep_checkBenefits.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_checkBenefits.Name = "rep_checkBenefits";
		this.Offer_TrialCur.Caption = "Trial Period Salary Currency";
		this.Offer_TrialCur.ColumnEdit = this.rep_looTrialCur;
		this.Offer_TrialCur.CurrencyDateField = null;
		this.Offer_TrialCur.CurrencyField = null;
		this.Offer_TrialCur.CurrencyFixed = null;
		this.Offer_TrialCur.CurrencyFixedRate = 0.0;
		this.Offer_TrialCur.CurrencyRateField = null;
		this.Offer_TrialCur.FieldName = "TrialCur";
		this.Offer_TrialCur.Name = "Offer_TrialCur";
		this.Offer_TrialCur.PrimaryKey = false;
		this.Offer_TrialCur.UnitField = null;
		this.Offer_TrialCur.Width = 181;
		this.rep_looTrialCur.AutoHeight = false;
		this.rep_looTrialCur.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_looTrialCur.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_looTrialCur.DisplayMember = "CurrName";
		this.rep_looTrialCur.Name = "rep_looTrialCur";
		this.rep_looTrialCur.NullText = "";
		this.rep_looTrialCur.ValueMember = "CurrCode";
		this.of_ContCur.Caption = "Contract Salary Currency";
		this.of_ContCur.ColumnEdit = this.rep_lookContractCur;
		this.of_ContCur.CurrencyDateField = null;
		this.of_ContCur.CurrencyField = null;
		this.of_ContCur.CurrencyFixed = null;
		this.of_ContCur.CurrencyFixedRate = 0.0;
		this.of_ContCur.CurrencyRateField = null;
		this.of_ContCur.FieldName = "ContCur";
		this.of_ContCur.Name = "of_ContCur";
		this.of_ContCur.PrimaryKey = false;
		this.of_ContCur.UnitField = null;
		this.of_ContCur.Width = 133;
		this.rep_lookContractCur.AutoHeight = false;
		this.rep_lookContractCur.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookContractCur.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookContractCur.Name = "rep_lookContractCur";
		this.rep_lookContractCur.NullText = "";
		this.gridColumnEx14.Caption = "Status";
		this.gridColumnEx14.ColumnEdit = this.luk_StatusOff;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "Status";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 6;
		this.luk_StatusOff.AutoHeight = false;
		this.luk_StatusOff.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusOff.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_StatusOff.DisplayMember = "Name";
		this.luk_StatusOff.Name = "luk_StatusOff";
		this.luk_StatusOff.NullText = "";
		this.luk_StatusOff.ValueMember = "Code";
		this.Of_ApplicantNo.Caption = "Applicant No";
		this.Of_ApplicantNo.ColumnEdit = this.btn_AppInOffer;
		this.Of_ApplicantNo.CurrencyDateField = null;
		this.Of_ApplicantNo.CurrencyField = null;
		this.Of_ApplicantNo.CurrencyFixed = null;
		this.Of_ApplicantNo.CurrencyFixedRate = 0.0;
		this.Of_ApplicantNo.CurrencyRateField = null;
		this.Of_ApplicantNo.FieldName = "ApplicantNo";
		this.Of_ApplicantNo.Name = "Of_ApplicantNo";
		this.Of_ApplicantNo.OptionsColumn.ReadOnly = true;
		this.Of_ApplicantNo.PrimaryKey = false;
		this.Of_ApplicantNo.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.Of_ApplicantNo.UnitField = null;
		this.Of_ApplicantNo.Visible = true;
		this.Of_ApplicantNo.VisibleIndex = 1;
		this.btn_AppInOffer.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.btn_AppInOffer.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_AppInOffer.Name = "btn_AppInOffer";
		this.btn_AppInOffer.ReadOnly = true;
		this.btn_AppInOffer.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_AppInOffer_ButtonClick);
		this.xtra_Hired.Controls.Add(this.grd_Hired);
		this.xtra_Hired.EditControl = null;
		this.xtra_Hired.EditValue = "Hired";
		this.xtra_Hired.FieldName = "xtra_Hired";
		this.xtra_Hired.IsSearchControl = false;
		this.xtra_Hired.Name = "xtra_Hired";
		this.xtra_Hired.ReadOnly = false;
		this.xtra_Hired.SettingInVisible = false;
		this.xtra_Hired.SettingReadonly = false;
		this.xtra_Hired.Size = new System.Drawing.Size(956, 289);
		this.xtra_Hired.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Hired.Text = "Hired";
		this.grd_Hired.AllowPasteExcel = true;
		this.grd_Hired.ClipboardData = null;
		this.grd_Hired.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Hired.Location = new System.Drawing.Point(0, 0);
		this.grd_Hired.MainView = this.gv_Hired;
		this.grd_Hired.Name = "grd_Hired";
		this.grd_Hired.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.rep_lookHTrialCur, this.rep_lookHContractCur, this.rep_btnappHired, this.rep_checkBeneHired, this.luk_StatusHired });
		this.grd_Hired.Size = new System.Drawing.Size(956, 289);
		this.grd_Hired.TabIndex = 1;
		this.grd_Hired.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Hired });
		this.gv_Hired.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Hired.ColumnKey = "ApplicantNo";
		this.gv_Hired.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumnEx12, this.gridColumnEx15
		});
		this.gv_Hired.DisableFormSetting = false;
		this.gv_Hired.DisplayException = false;
		this.gv_Hired.FieldCurrency = null;
		this.gv_Hired.FieldRate = null;
		this.gv_Hired.FormId = "700012";
		this.gv_Hired.GridControl = this.grd_Hired;
		this.gv_Hired.IndicatorWidth = 35;
		this.gv_Hired.IsApzTable = false;
		this.gv_Hired.IsInsertNewRow = false;
		this.gv_Hired.IsLoadingData = false;
		this.gv_Hired.ItemId = "APZ_HJV6";
		this.gv_Hired.LstColumnAllowBatch = null;
		this.gv_Hired.LstColumnNotAllowBatch = null;
		this.gv_Hired.LstColumnNotEditable = null;
		this.gv_Hired.LstColumNotSetting = null;
		this.gv_Hired.Name = "gv_Hired";
		this.gv_Hired.OptionsSelection.MultiSelect = true;
		this.gv_Hired.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Hired.OptionsView.ShowGroupPanel = false;
		this.gv_Hired.UseBatchMod = false;
		this.gv_Hired.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Hired_RowCellStyle);
		this.gv_Hired.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gv_Hired_SelectionChanged);
		this.gv_Hired.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Hired_ShowingEditor);
		this.gv_Hired.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Hired_CellValueChanged);
		this.gv_Hired.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Hired_CellValueChanging);
		this.gridColumn1.Caption = "DocEntry";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "DocEntry";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "LineNum";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "LineNum";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn3.Caption = "Applicant No";
		this.gridColumn3.ColumnEdit = this.rep_btnappHired;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "ApplicantNo";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn3.Width = 96;
		this.rep_btnappHired.AutoHeight = false;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_btnappHired.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_btnappHired.Name = "rep_btnappHired";
		this.rep_btnappHired.ReadOnly = true;
		this.rep_btnappHired.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_btnappHired_ButtonClick);
		this.gridColumn4.Caption = "Applicant Name";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "ApplicantName";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 2;
		this.gridColumn4.Width = 106;
		this.gridColumn5.Caption = "Offer Letter";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "OfferLetter";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Width = 98;
		this.gridColumn6.Caption = "Trial Period Salary";
		this.gridColumn6.ColumnEdit = this.repositoryItemTextEditExPrice3;
		this.gridColumn6.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = "TrialCur";
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn6.FieldName = "TrialPeriodSalary";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 3;
		this.gridColumn6.Width = 138;
		this.gridColumn7.Caption = "Contract Salary";
		this.gridColumn7.ColumnEdit = this.repositoryItemTextEditExPrice4;
		this.gridColumn7.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = "ContCur";
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumn7.FieldName = "ContractSalary";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 4;
		this.gridColumn7.Width = 159;
		this.gridColumn8.Caption = "Other Benefits";
		this.gridColumn8.ColumnEdit = this.rep_checkBeneHired;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "OtherBenefits";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 5;
		this.gridColumn8.Width = 102;
		this.rep_checkBeneHired.AutoHeight = false;
		this.rep_checkBeneHired.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_checkBeneHired.Name = "rep_checkBeneHired";
		this.gridColumn9.Caption = "Trial Period Salary Currency";
		this.gridColumn9.ColumnEdit = this.rep_lookHTrialCur;
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "TrialCur";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Width = 178;
		this.rep_lookHTrialCur.AutoHeight = false;
		this.rep_lookHTrialCur.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookHTrialCur.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookHTrialCur.DisplayMember = "CurrName";
		this.rep_lookHTrialCur.Name = "rep_lookHTrialCur";
		this.rep_lookHTrialCur.NullText = "";
		this.rep_lookHTrialCur.ValueMember = "CurrCode";
		this.gridColumn10.Caption = "Contract Salary Currency";
		this.gridColumn10.ColumnEdit = this.rep_lookHContractCur;
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "ContCur";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Width = 126;
		this.rep_lookHContractCur.AutoHeight = false;
		this.rep_lookHContractCur.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_lookHContractCur.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rep_lookHContractCur.DisplayMember = "CurrName";
		this.rep_lookHContractCur.Name = "rep_lookHContractCur";
		this.rep_lookHContractCur.NullText = "";
		this.rep_lookHContractCur.ValueMember = "CurrCode";
		this.gridColumnEx12.Caption = "EmployeeId";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "empId";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx15.Caption = "Status";
		this.gridColumnEx15.ColumnEdit = this.luk_StatusHired;
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "Status";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 6;
		this.luk_StatusHired.AutoHeight = false;
		this.luk_StatusHired.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusHired.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_StatusHired.DisplayMember = "Name";
		this.luk_StatusHired.Name = "luk_StatusHired";
		this.luk_StatusHired.NullText = "";
		this.luk_StatusHired.ValueMember = "Code";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Doc No";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(3, 38);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(70, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Doc No";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 74);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_JobVacancy);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtra_APZ_HJV6);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(958, 466);
		this.splitContainerControl1.SplitterPosition = 147;
		this.splitContainerControl1.TabIndex = 2;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "From Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(2, 12);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(61, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 1;
		this.labelLine2.Utext = "From Date";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "To Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(175, 12);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(53, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 4;
		this.labelLine3.Utext = "To Date";
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = null;
		this.date_FromDate.Location = new System.Drawing.Point(58, 12);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.IsSearchControl = true;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(100, 20);
		this.date_FromDate.TabIndex = 5;
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
		this.date_ToDate.Location = new System.Drawing.Point(224, 12);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = true;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(100, 20);
		this.date_ToDate.TabIndex = 6;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Status";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(175, 38);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(53, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 7;
		this.labelLine4.Utext = "Status";
		this.txt_DocNo.EditControl = null;
		this.txt_DocNo.FieldName = null;
		this.txt_DocNo.IsSearchControl = true;
		this.txt_DocNo.Location = new System.Drawing.Point(58, 38);
		this.txt_DocNo.Name = "txt_DocNo";
		this.txt_DocNo.SettingInVisible = false;
		this.txt_DocNo.SettingReadonly = false;
		this.txt_DocNo.Size = new System.Drawing.Size(100, 20);
		this.txt_DocNo.TabIndex = 8;
		this.look_Status.EditControl = null;
		this.look_Status.FieldName = null;
		this.look_Status.IsSearchControl = true;
		this.look_Status.Location = new System.Drawing.Point(224, 38);
		this.look_Status.Name = "look_Status";
		this.look_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Status.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.look_Status.Properties.DisplayMember = "Name";
		this.look_Status.Properties.NullText = "";
		this.look_Status.Properties.ValueMember = "Code";
		this.look_Status.SettingInVisible = false;
		this.look_Status.SettingReadonly = false;
		this.look_Status.Size = new System.Drawing.Size(100, 20);
		this.look_Status.TabIndex = 9;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(356, 35);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 10;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(3, 546);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 11;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(81, 546);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 12;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Profile.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Profile.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Profile.EditControl = null;
		this.btn_Profile.EditValue = "Profile Match Up";
		this.btn_Profile.FieldName = null;
		this.btn_Profile.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Profile.ImageOptions.Image");
		this.btn_Profile.IsSearchControl = false;
		this.btn_Profile.Location = new System.Drawing.Point(204, 546);
		this.btn_Profile.Name = "btn_Profile";
		this.btn_Profile.ReadOnly = false;
		this.btn_Profile.SettingInVisible = false;
		this.btn_Profile.SettingReadonly = false;
		this.btn_Profile.Size = new System.Drawing.Size(120, 23);
		this.btn_Profile.TabIndex = 13;
		this.btn_Profile.Text = "Profile Match Up";
		this.btn_Profile.Click += new System.EventHandler(this.btn_Profile_Click);
		this.btn_Hired.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Hired.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Hired.EditControl = null;
		this.btn_Hired.EditValue = "Hired";
		this.btn_Hired.FieldName = null;
		this.btn_Hired.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Hired.ImageOptions.Image");
		this.btn_Hired.IsSearchControl = false;
		this.btn_Hired.Location = new System.Drawing.Point(873, 546);
		this.btn_Hired.Name = "btn_Hired";
		this.btn_Hired.ReadOnly = false;
		this.btn_Hired.SettingInVisible = false;
		this.btn_Hired.SettingReadonly = false;
		this.btn_Hired.Size = new System.Drawing.Size(75, 23);
		this.btn_Hired.TabIndex = 14;
		this.btn_Hired.Text = "Hired";
		this.btn_Hired.Visible = false;
		this.btn_Hired.Click += new System.EventHandler(this.btn_Hired_Click);
		this.btn_Accept.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Accept.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Accept.EditControl = null;
		this.btn_Accept.EditValue = "Accept";
		this.btn_Accept.FieldName = null;
		this.btn_Accept.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Accept.ImageOptions.Image");
		this.btn_Accept.IsSearchControl = false;
		this.btn_Accept.Location = new System.Drawing.Point(792, 546);
		this.btn_Accept.Name = "btn_Accept";
		this.btn_Accept.ReadOnly = false;
		this.btn_Accept.SettingInVisible = false;
		this.btn_Accept.SettingReadonly = false;
		this.btn_Accept.Size = new System.Drawing.Size(75, 23);
		this.btn_Accept.TabIndex = 15;
		this.btn_Accept.Text = "Accept";
		this.btn_Accept.Visible = false;
		this.btn_Accept.Click += new System.EventHandler(this.btn_Accept_Click);
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(960, 572);
		base.Controls.Add(this.btn_Accept);
		base.Controls.Add(this.btn_Hired);
		base.Controls.Add(this.btn_Profile);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.look_Status);
		base.Controls.Add(this.txt_DocNo);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.date_ToDate);
		base.Controls.Add(this.date_FromDate);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "700012";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_JobVacancy.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_JobVacancy";
		this.Text = "Job Vacancy ";
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_JobVacancy).EndInit();
		this.menu_Jobvacancy.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_JobVacancy).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_series).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lok_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Department).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Position).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_FromDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_FromDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_ToDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_ToDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookJobType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtra_APZ_HJV6).EndInit();
		this.xtra_APZ_HJV6.ResumeLayout(false);
		this.contextMenuStrip1.ResumeLayout(false);
		this.xtra_APZ_HJV7.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Qualification).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Qualification).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnQualificationCode).EndInit();
		this.xtra_APZ_HJV1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Hiring).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Hiring).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_EmpID).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookTeamRole).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookPrimary).EndInit();
		this.xtra_APZ_HJV2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Advertisement).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Advertisement).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookAdvSource).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_AdvCost).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookCurrency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_AdvFromDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_AdvFromDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_AdvToDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_AdvToDate).EndInit();
		this.xtra_APZ_HJV3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_App).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_App).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_AppNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_AppDoB.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_AppDoB).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookGender).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CheckedQualification).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusApplican).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookInterviewStatus).EndInit();
		this.xtra_APZ_HJV4.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Inte).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Inte).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookInteType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnQues).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnTeam).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateDateInter.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateDateInter).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnAppInInter).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_checkTeam).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Applications).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Applications).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_appInIter).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_Gender2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.r_Qualification2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusApp).EndInit();
		this.xtra_APZ_HJV5.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Offer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Offer).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_checkBenefits).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_looTrialCur).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookContractCur).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusOff).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_AppInOffer).EndInit();
		this.xtra_Hired.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Hired).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Hired).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnappHired).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_checkBeneHired).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookHTrialCur).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_lookHContractCur).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusHired).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
