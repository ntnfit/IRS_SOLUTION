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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using Commons.Helper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.HumanResources;

public class frm_EmpRelatives : XtraFormEx
{
	private readonly int _atcEntry;

	private int _relaId;

	public int EmpId;

	private string _strImage;

	private string _picFilePath = "";

	private bool _edit;

	private bool _add;

	private bool _flag;

	private bool _isRefresh;

	private bool _isCancel;

	private bool _forceClose;

	private bool isSorting;

	private readonly string Controller = "EmpRelativesClient";

	private readonly FtpHelper _ftpHelper = new FtpHelper();

	private Dictionary<string, Control> _dicControls;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private IContainer components;

	private XtraTabControlEx tab_Detail;

	private XtraTabPageEx tab_Address;

	private XtraTabPageEx tab_Remarks;

	private XtraTabPageEx tab_Attachments;

	private XtraTabPageEx tab_User;

	private MemoEditEx txt_Remark;

	private PanelControl pnl_Address;

	private TextEditEx txt_H_Block;

	private TextEditEx txt_H_Street;

	private LabelLine lbl_H_Block;

	private LabelLine lbl_H_Street;

	private TextEditEx txt_W_Street;

	private TextEditEx txt_W_Block;

	private LabelLine lbl_WStreet;

	private LabelLine lbl_Block;

	private TextEditEx txt_H_County;

	private TextEditEx txt_H_City;

	private TextEditEx txt_H_Zipcode;

	private LabelLine lbl_H_zipCode;

	private LabelLine lbl_H_city;

	private LabelLine lbl_H_County;

	private LabelLine lbl_H_State;

	private LabelLine lbl_H_Country;

	private LabelLine lbl_WorkAdress;

	private LabelLine lbl_HomeAddress;

	private TextEditEx txt_W_County;

	private TextEditEx txt_W_City;

	private TextEditEx txt_W_Zipcode;

	private LabelLine lbl_Country;

	private LabelLine lbl_State;

	private LabelLine lbl_ZipCode;

	private LabelLine lbl_County;

	private LabelLine lbl_City;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private PanelControl panelControl5;

	private GridControlEx grd_Udf;

	private GridViewEx gv_Udf;

	private LookUpEditCombo luk_W_Country;

	private LookUpEditCombo luk_W_State;

	private LookUpEditCombo luk_H_State;

	private LookUpEditCombo luk_H_Country;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem deleteRowToolStripMenuItem1;

	private XtraTabPageEx tab_Main;

	private PanelControl panelControl1;

	private PictureEdit pic_Images;

	private TextEditEx txt_HomePhone;

	private TextEditEx txt_Fax;

	private TextEditEx txt_Email;

	private TextEditEx txt_ExtPhone;

	private TextEditEx txt_MobilePhone;

	private TextEditEx txt_OfficePhone;

	private TextEditEx txt_RelaNo;

	private LabelLine lbl_Email;

	private TextEditEx txt_JobTitle;

	private TextEditEx txt_LastName;

	private TextEditEx txt_MiddleName;

	private TextEditEx txt_FirstName;

	private LabelLine lbl_Fax;

	private LabelLine lbl_Home;

	private LabelLine lbl_off;

	private LabelLine lbl_ext;

	private LabelLine lbl_MPhone;

	private LabelLine lbl_EmpNo;

	private LabelLine lbl_MiddleName;

	private LabelLine lbl_LastName;

	private LabelLine lbl_Job;

	private LabelLine lbl_FirstName;

	private GridControlEx grd_Content;

	private GridViewEx gv_Content;

	private LookUpEditCombo luk_Citizenship;

	private TextEditEx txt_PassNo;

	private LabelLine lbl_PassNo;

	private LabelLine lbl_PassExpDate;

	private LabelLine lbl_Citizenship;

	private LookUpEditCombo luk_Gender;

	private TextEditEx txt_IDNo;

	private LabelLine lbl_IDNo;

	private LabelLine lbl_Date;

	private LabelLine lbl_CountryOfBirth;

	private LabelLine lbl_Gender;

	private LookUpEditCombo luk_CountryOfBirth;

	private LookUpEditCombo luk_PersGroup;

	private LabelLine lbl_PersGroup;

	private LookUpEditCombo luk_Relationship;

	private LabelLine lbl_Relationship;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn8;

	private ToolStripMenuItem addNewtoolStripMenuItem;

	private RepositoryItemLookUpEdit luk_grdGender;

	private RepositoryItemGridLookUpEdit luk_grdRelation;

	private GridViewEx repositoryItemGridLookUpEdit1View;

	private CheckEditEx chk_YearFormat;

	private CheckEditEx chk_FamDeduction;

	private ucAttachment ucAttachment1;

	private DateEditEx date_FromDate;

	private DateEditEx date_ToDate;

	private DateEditEx date_Brith;

	private DateEditEx date_PassExp;

	[DllImport("Shlwapi.dll", CharSet = CharSet.Auto, SetLastError = true)]
	private static extern bool PathFileExists(StringBuilder path);

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
			btn_Save.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btn_Save.Text = Resources.ButtonFind;
			break;
		}
	}

	public frm_EmpRelatives()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
			}
			IsSetSuperTip = true;
			IsScrollable = true;
			FormId = "oEmpRalatives";
			gv_Content.FormId = FormId;
			gv_Content.ItemId = "APZ_OHER";
			OnLoadAsync += frm_EmpRelatives_Load;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async Task frm_EmpRelatives_Load(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			await SetDictionaryControls();
			await LoadLookUp();
			await LoadUDF();
			AddHandlers();
			await LoadData();
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
			WinCommons.ErrorLineNumber(ex);
		}
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
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_W_Country_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (luk_W_Country.EditValue != null)
			{
				string code = Function.ToString(luk_W_Country.EditValue);
				LoadComboState(code, 1);
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
				string code = Function.ToString(luk_H_Country.EditValue);
				LoadComboState(code, 2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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

	private async void frm_EmpRelatives_FormClosing(object sender, FormClosingEventArgs e)
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
			if (FormMode == Enums.FormViewModeType.Update || (FormMode == Enums.FormViewModeType.Add && _add))
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

	private void gv_Content_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Content_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (!_flag && !isSorting && e.RowHandle >= 0 && !_isLoading && grd_Content.DataSourceTable.IsNotNull() && e.FocusedRowHandle >= 0)
			{
				GetDetails(Function.ParseInt(gv_Content.GetFocusedRowCellValue("relaID")), EmpId);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Content_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_flag || isSorting)
			{
				e.Allow = false;
			}
			else if (e.RowHandle < 0 || _isLoading)
			{
				e.Allow = false;
			}
			else if ((FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				_flag = true;
				e.Allow = false;
				txt_FirstName.Focus();
				_flag = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			if (gv_Content.RowCount <= 0)
			{
				return;
			}
			_flag = true;
			if (string.IsNullOrEmpty(Function.ToString(_relaId)) || _relaId == _httpClient.EmpRelativesClient.GetNextId(EmpId))
			{
				gv_Content.BeginUpdate();
				gv_Content.DeleteSelectedRows();
				gv_Content.EndUpdate();
				txt_FirstName.Focus();
				gv_Content.GetFocusedRowCellValue("relaID");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				string text = await _httpClient.EmpRelativesClient.DeleteDoc(Function.ParseInt(_relaId), EmpId);
				if (string.IsNullOrEmpty(text))
				{
					gv_Content.BeginUpdate();
					gv_Content.DeleteSelectedRows();
					gv_Content.EndUpdate();
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					if (!grd_Content.DataSourceTable.IsNotNull())
					{
						await REFRESH();
					}
					else
					{
						await MOVE_NEXT();
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
				_flag = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			_flag = false;
		}
	}

	private async void addNewtoolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			await REFRESH();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_YearFormat_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (chk_YearFormat.Checked)
			{
				date_Brith.Properties.EditMask = "yyyy";
				date_Brith.Properties.DisplayFormat.FormatString = "yyyy";
			}
			else
			{
				date_Brith.Properties.Mask.MaskType = MaskType.None;
				date_Brith.Properties.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
				date_Brith.Properties.DisplayFormat.FormatType = FormatType.DateTime;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Content_StartSorting(object sender, EventArgs e)
	{
		isSorting = true;
	}

	private void gv_Content_EndSorting(object sender, EventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.View)
		{
			gv_Content.ClearSorting();
		}
		isSorting = false;
	}

	private async void gv_Content_ColumnFilterChanged(object sender, EventArgs e)
	{
		try
		{
			if (grd_Content.DataSourceTable.IsNotNull())
			{
				await GetDetails(Function.ParseInt(gv_Content.GetFocusedRowCellValue("relaID")), EmpId);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_FamDeduction_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!chk_FamDeduction.Checked)
			{
				date_FromDate.Visible = false;
				date_ToDate.Visible = false;
			}
			else
			{
				date_FromDate.Visible = true;
				date_ToDate.Visible = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Content_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.ReadOnly)
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Content_Enter(object sender, EventArgs e)
	{
	}

	private void grd_Content_Leave(object sender, EventArgs e)
	{
	}

	private void SetFocusedAppearance(bool isFocused)
	{
		gv_Content.OptionsSelection.EnableAppearanceFocusedRow = isFocused;
		gv_Content.OptionsSelection.EnableAppearanceFocusedCell = isFocused;
		gv_Content.OptionsSelection.EnableAppearanceHideSelection = isFocused;
	}

	private async Task LoadData()
	{
		_ = 2;
		try
		{
			_flag = true;
			await RefreshGrid();
			int num = gv_Content.FocusedRowHandle;
			if (grd_Content.DataSourceTable.IsNotNull())
			{
				if (num < 0)
				{
					num = 0;
				}
				gv_Content.FocusedRowHandle = num;
				if (grd_Content.DataSourceTable.IsNotNull() && num >= 0)
				{
					await GetDetails(Function.ParseInt(gv_Content.GetFocusedRowCellValue("relaID")), EmpId);
				}
			}
			else if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "F"))
			{
				await REFRESH();
			}
			_flag = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task<string> SaveData()
	{
		_ = 7;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Find:
			{
				WinCommons._OpenProcessing();
				DataTable dt2 = new DataTable();
				if (_edit)
				{
					DataTable controlsValueOCRDNoSAP = GetControlsValueOCRDNoSAP();
					if (controlsValueOCRDNoSAP.IsNotNull())
					{
						controlsValueOCRDNoSAP.Rows[0]["empID"] = EmpId;
						controlsValueOCRDNoSAP.AcceptChanges();
					}
					dt2 = await _httpClient.EmpRelativesClient.Search(controlsValueOCRDNoSAP);
				}
				if (dt2.IsNotNull())
				{
					if (dt2.Rows.Count > 1)
					{
						string empty = string.Empty;
						empty = empty + Function.ToString(EmpId) + ",";
						for (int i = 0; i < dt2.Rows.Count; i++)
						{
							empty = empty + Function.ToString(dt2.Rows[i]["relaID"]) + ",";
						}
						if (!string.IsNullOrEmpty(empty) && empty.Length > 0)
						{
							empty = empty.Substring(0, empty.Length - 1);
						}
						frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.EmployeeRelatives, false, string.Empty, "Apz_Cfl_EmployleeRelatives", Enums.ItemObjectType.NoItemType, null, "", empty);
						frm_CFL2.ShowDialog(this);
						DataRow row = frm_CFL2.ChooseItem;
						if (row != null)
						{
							await RefreshGrid();
							_relaId = Function.ParseInt(row["relaID"]);
							await GetDetails(_relaId, EmpId);
							if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "R"))
							{
								Controls.EnableDisableControl(isEnable: false);
							}
						}
					}
					else if (dt2.Rows.Count == 1)
					{
						await RefreshGrid();
						_relaId = Function.ParseInt(dt2.Rows[0]["relaID"]);
						await GetDetails(_relaId, EmpId);
						if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "R"))
						{
							Controls.EnableDisableControl(isEnable: false);
						}
					}
				}
				else if (_edit)
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
				}
				WinCommons._CloseProcessing();
				break;
			}
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
			{
				WinCommons._OpenProcessing();
				DataSet dataSet = new DataSet();
				DataTable dtDoc = GetControlsValueOCRDNoSAP();
				if (dtDoc.IsNotNull())
				{
					if (FormMode == Enums.FormViewModeType.Add)
					{
						dtDoc.Rows[0]["UserSign"] = GlobalData.UserID;
						dtDoc.Rows[0]["LogInstanc"] = 0;
					}
					if (luk_PersGroup.EditValue == null)
					{
						dtDoc.Rows[0]["PersGroup"] = "-1";
					}
					if (chk_YearFormat.EditValue == null)
					{
						dtDoc.Rows[0]["birthOnlyYear"] = "N";
					}
					if (chk_FamDeduction.EditValue == null)
					{
						dtDoc.Rows[0]["famDeduction"] = "N";
					}
					dtDoc.Rows[0]["empID"] = EmpId;
					dtDoc.Rows[0]["UserSign2"] = GlobalData.UserID;
					dtDoc.Rows[0]["UpdateDate"] = DateTime.Now.Date;
					dtDoc.AcceptChanges();
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
					text = ((fileName == null || fileName.Contains("APZ-EmpRelatives" + Function.ToString(EmpId) + Function.ToString(_relaId))) ? fileName : ("APZ-EmpRelatives-" + Function.ToString(EmpId) + Function.ToString(_relaId) + "-" + fileName));
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
				dtDoc.Rows[0]["picture"] = text;
				dtDoc.TableName = "dtDoc";
				dataSet.Tables.Add(dtDoc.Copy());
				if (!ucAttachment1.GetAttachmentNoSAP(out var dt))
				{
					return "error";
				}
				dt.TableName = "AttFile";
				dataSet.Tables.Add(dt.Copy());
				string rs = FormMode switch
				{
					Enums.FormViewModeType.Add => await _httpClient.EmpRelativesClient.SaveDoc(dataSet), 
					Enums.FormViewModeType.Update => await _httpClient.EmpRelativesClient.SaveDocUpdate(dataSet), 
					_ => string.Empty, 
				};
				if (string.IsNullOrEmpty(rs))
				{
					WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					if (FormMode == Enums.FormViewModeType.Add)
					{
						await REFRESH();
					}
					else
					{
						FormMode = Enums.FormViewModeType.View;
						PostDataBackToGridSource(dtDoc);
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

	private void PostDataBackToGridSource(DataTable dt)
	{
		try
		{
			foreach (DataRow item in grd_Content.DataSourceTable.AsEnumerable())
			{
				if (item["RelaId"].Equals(dt.Rows[0]["RelaId"]))
				{
					item.ItemArray = dt.Rows[0].ItemArray;
				}
			}
			grd_Content.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SetDictionaryControls()
	{
		try
		{
			_dicControls = new Dictionary<string, Control>();
			_dicControls.Add("APZ_OHER.firstName", txt_FirstName);
			_dicControls.Add("APZ_OHER.middleName", txt_MiddleName);
			_dicControls.Add("APZ_OHER.lastName", txt_LastName);
			_dicControls.Add("APZ_OHER.jobTitle", txt_JobTitle);
			_dicControls.Add("APZ_OHER.relaID", txt_RelaNo);
			_dicControls.Add("APZ_OHER.officeTel", txt_OfficePhone);
			_dicControls.Add("APZ_OHER.officeExt", txt_ExtPhone);
			_dicControls.Add("APZ_OHER.mobile", txt_MobilePhone);
			_dicControls.Add("APZ_OHER.homeTel", txt_HomePhone);
			_dicControls.Add("APZ_OHER.fax", txt_Fax);
			_dicControls.Add("APZ_OHER.email", txt_Email);
			_dicControls.Add("APZ_OHER.relationship", luk_Relationship);
			_dicControls.Add("APZ_OHER.PersGroup", luk_PersGroup);
			_dicControls.Add("APZ_OHER.sex", luk_Gender);
			_dicControls.Add("APZ_OHER.birthDate", date_Brith);
			_dicControls.Add("APZ_OHER.birthOnlyYear", chk_YearFormat);
			_dicControls.Add("APZ_OHER.brthCountr", luk_CountryOfBirth);
			_dicControls.Add("APZ_OHER.govID", txt_IDNo);
			_dicControls.Add("APZ_OHER.citizenshp", luk_Citizenship);
			_dicControls.Add("APZ_OHER.passportNo", txt_PassNo);
			_dicControls.Add("APZ_OHER.passportEx", date_PassExp);
			_dicControls.Add("APZ_OHER.famDeduction", chk_FamDeduction);
			_dicControls.Add("APZ_OHER.deducFromDate", date_FromDate);
			_dicControls.Add("APZ_OHER.deducToDate", date_ToDate);
			_dicControls.Add("APZ_OHER.workStreet", txt_W_Street);
			_dicControls.Add("APZ_OHER.workBlock", txt_W_Block);
			_dicControls.Add("APZ_OHER.workZip", txt_W_Zipcode);
			_dicControls.Add("APZ_OHER.workCity", txt_W_City);
			_dicControls.Add("APZ_OHER.workCounty", txt_W_County);
			_dicControls.Add("APZ_OHER.workState", luk_W_State);
			_dicControls.Add("APZ_OHER.workCountr", luk_W_Country);
			_dicControls.Add("APZ_OHER.homeStreet", txt_H_Street);
			_dicControls.Add("APZ_OHER.homeBlock", txt_H_Block);
			_dicControls.Add("APZ_OHER.homeZip", txt_H_Zipcode);
			_dicControls.Add("APZ_OHER.homeCity", txt_H_City);
			_dicControls.Add("APZ_OHER.homeCounty", txt_H_County);
			_dicControls.Add("APZ_OHER.homeState", luk_H_State);
			_dicControls.Add("APZ_OHER.homeCountr", luk_H_Country);
			_dicControls.Add("APZ_OHER.remark", txt_Remark);
			await this.SetControlMaxLength(_dicControls);
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
				return;
			}
			if (_dicControls.Any((KeyValuePair<string, Control> t) => t.Key.Split('.')[0] == table))
			{
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
			grd_Udf.DataSource = dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetControlsValueOCRDNoSAP()
	{
		DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHER");
		DataRow dataRow = tableStructure.NewRow();
		try
		{
			if (_dicControls.Any((KeyValuePair<string, Control> t) => t.Key.Split('.')[0] == "APZ_OHER"))
			{
				List<KeyValuePair<string, Control>> list = _dicControls.Where((KeyValuePair<string, Control> t) => t.Key.Split('.')[0] == "APZ_OHER").ToList();
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
						dataRow[text] = Convert.ToInt16(Function.ParseInt(((BaseEdit)list[num].Value).EditValue));
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
			DataTable dataTable = grd_Udf.DatasourceTable();
			if (dataTable.IsNotNull())
			{
				List<DataColumn> list2 = (from DataColumn t in dataTable.Columns
					where t.ColumnName.StartsWith("U_")
					select t).ToList();
				for (int num2 = 0; num2 < list2.Count; num2++)
				{
					if (tableStructure.Columns.Contains(list2[num2].ColumnName))
					{
						dataRow[list2[num2].ColumnName] = dataTable.Rows[0][list2[num2].ColumnName];
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
			if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "R"))
			{
				WinCommons.Error(Enums.MessageCode.YouDoNorHavePermisionToModifiedForm, "");
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			gv_Content.BeginInsertRow();
			gv_Content.ClearSorting();
			await RefreshGrid();
			this.ResetData();
			await LoadLookUp();
			FormMode = Enums.FormViewModeType.Add;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHER");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["relaID"] = _httpClient.EmpRelativesClient.GetNextId(EmpId);
			dataRow["empID"] = EmpId;
			dataRow["sex"] = "M";
			dataRow["birthOnlyYear"] = "N";
			tableStructure.Rows.Add(dataRow);
			tableStructure.AcceptChanges();
			BindControlsValue("APZ_OHER", tableStructure);
			chk_YearFormat.Checked = Function.ToString(tableStructure.Rows[0]["birthOnlyYear"]).Equals("Y");
			chk_FamDeduction.Checked = Function.ToString(tableStructure.Rows[0]["famDeduction"]).Equals("Y");
			if (!chk_FamDeduction.Checked)
			{
				date_FromDate.Visible = false;
				date_ToDate.Visible = false;
			}
			else
			{
				date_FromDate.Visible = true;
				date_ToDate.Visible = true;
			}
			pic_Images.Image = null;
			pic_Images.Properties.NullText = "No image data!";
			ucAttachment1.DataSource = WinFuntions.GetTableStructure("ATC1");
			_add = false;
			txt_FirstName.Focus();
			txt_RelaNo.Enabled = false;
			tab_Attachments.PageEnabled = true;
			pic_Images.Enabled = true;
			txt_RelaNo.Enabled = false;
			txt_RelaNo.ReadOnly = true;
			tab_Attachments.PageEnabled = true;
			pic_Images.Enabled = true;
			gv_Content.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Content.EndInsertRow();
	}

	public async Task REFRESH_DATA()
	{
		_ = 3;
		try
		{
			if (!_isRefresh && (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
			{
				await LoadData();
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
				contextMenuStrip1.Enabled = false;
				contextMenuStrip2.Enabled = false;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
			}
			else if (FormMode == Enums.FormViewModeType.Add && _add && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			this.SetFindMode();
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OHER");
			DataRow row = tableStructure.NewRow();
			tableStructure.Rows.Add(row);
			tableStructure.AcceptChanges();
			BindControlsValue("APZ_OHER", tableStructure);
			chk_YearFormat.EditValue = null;
			chk_FamDeduction.EditValue = null;
			pic_Images.Image = null;
			pic_Images.Properties.NullText = "No image data!";
			ucAttachment1.DataSource = WinFuntions.GetTableStructure("ATC1");
			FormMode = Enums.FormViewModeType.Find;
			txt_RelaNo.Enabled = true;
			txt_RelaNo.ReadOnly = false;
			txt_RelaNo.Text = string.Empty;
			tab_Attachments.PageEnabled = false;
			pic_Images.Enabled = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadLookUp()
	{
		try
		{
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboCountry();
			SetDataSourceByDataTable(luk_H_Country, dataTable.Copy());
			SetDataSourceByDataTable(luk_W_Country, dataTable.Copy());
			SetDataSourceByDataTable(luk_CountryOfBirth, dataTable.Copy());
			SetDataSourceByDataTable(luk_Citizenship, dataTable.Copy());
			List<Common.Table_Code> list = new List<Common.Table_Code>
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
			};
			SetDataSourceByList(luk_Gender, list);
			luk_grdGender.DataSource = list;
			luk_grdGender.DisplayMember = "Name";
			luk_grdGender.ValueMember = "Code";
			luk_grdGender.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_Code> list2 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "1",
					Name = "Father"
				},
				new Common.Table_Code
				{
					Code = "2",
					Name = "Mother"
				},
				new Common.Table_Code
				{
					Code = "3",
					Name = "Brother"
				},
				new Common.Table_Code
				{
					Code = "4",
					Name = "Sister"
				},
				new Common.Table_Code
				{
					Code = "5",
					Name = "Father-in-law"
				},
				new Common.Table_Code
				{
					Code = "6",
					Name = "Mother-in-law"
				},
				new Common.Table_Code
				{
					Code = "7",
					Name = "Wife"
				},
				new Common.Table_Code
				{
					Code = "8",
					Name = "Husband"
				},
				new Common.Table_Code
				{
					Code = "9",
					Name = "Children"
				},
				new Common.Table_Code
				{
					Code = "10",
					Name = "Grandfather"
				},
				new Common.Table_Code
				{
					Code = "11",
					Name = "Grandmother"
				},
				new Common.Table_Code
				{
					Code = "12",
					Name = "Other"
				}
			};
			SetDataSourceByList(luk_Relationship, list2);
			luk_grdRelation.DataSource = list2;
			luk_grdRelation.DisplayMember = "Name";
			luk_grdRelation.ValueMember = "Code";
			luk_grdRelation.BestFitMode = BestFitMode.BestFitResizePopup;
			List<Common.Table_Code> list3 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "101",
					Name = "Subject to Social Insurance"
				},
				new Common.Table_Code
				{
					Code = "102",
					Name = "Apprentice"
				},
				new Common.Table_Code
				{
					Code = "104",
					Name = "Home Worker"
				},
				new Common.Table_Code
				{
					Code = "105",
					Name = "Trainee"
				},
				new Common.Table_Code
				{
					Code = "106",
					Name = "Student"
				},
				new Common.Table_Code
				{
					Code = "108",
					Name = "Early Retirement"
				},
				new Common.Table_Code
				{
					Code = "109",
					Name = "Part-Time Employee"
				},
				new Common.Table_Code
				{
					Code = "110",
					Name = "Short-Term Employee"
				},
				new Common.Table_Code
				{
					Code = "112",
					Name = "Family Member: Agriculture"
				},
				new Common.Table_Code
				{
					Code = "113",
					Name = "Addnl Income: Agriculture"
				},
				new Common.Table_Code
				{
					Code = "114",
					Name = "Addnl Income: Seasonal Agriculture"
				},
				new Common.Table_Code
				{
					Code = "116",
					Name = "Receiving Compensation Pay"
				},
				new Common.Table_Code
				{
					Code = "118",
					Name = "Irregularly Employed"
				},
				new Common.Table_Code
				{
					Code = "119",
					Name = "Pensioner"
				},
				new Common.Table_Code
				{
					Code = "997",
					Name = "Not Specified"
				}
			};
			SetDataSourceByList(luk_PersGroup, list3);
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
				DataRow row = dtSource.NewRow();
				dtSource.Rows.InsertAt(row, 0);
				dtSource.AcceptChanges();
				luk.Properties.DataSource = null;
				luk.Properties.DataSource = dtSource.Copy();
				luk.Properties.DisplayMember = displayMember;
				luk.Properties.ValueMember = valueMember;
				luk.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
				luk.Properties.DropDownRows = dtSource.Rows.Count;
				luk.Properties.ForceInitialize();
				luk.Properties.PopulateColumns();
				luk.Properties.Columns[valueMember].Visible = false;
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

	private void SetFocusedRowBasedOnId(int relaId)
	{
		try
		{
			int focusedRowHandle = 0;
			for (int i = 0; i < gv_Content.RowCount; i++)
			{
				if (Function.ParseInt(gv_Content.GetRowCellValue(i, "relaID")) == relaId)
				{
					focusedRowHandle = i;
				}
			}
			gv_Content.FocusedRowHandle = focusedRowHandle;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task GetDetails(int relaId, int empId)
	{
		_ = 2;
		try
		{
			WinCommons._OpenProcessing();
			_isLoading = true;
			DataSet ds = await _httpClient.EmpRelativesClient.GetDetail(relaId, empId);
			if (ds.IsNotNull() && ds.Tables.Count > 0)
			{
				if (ds.Tables[0].Rows.Count <= 0)
				{
					WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
					await MODE_FIND();
					WinCommons._CloseProcessing();
					return;
				}
				this.ResetData();
				BindControlsValue("APZ_OHER", ds.Tables[0]);
				if (!chk_FamDeduction.Checked)
				{
					date_FromDate.Visible = false;
					date_ToDate.Visible = false;
				}
				else
				{
					date_FromDate.Visible = true;
					date_ToDate.Visible = true;
				}
				int absEntry = Function.ParseInt(ds.Tables[0].Rows[0]["AtcEntry"]);
				await ucAttachment1.LoadAttachments(absEntry);
				FormMode = Enums.FormViewModeType.View;
				txt_RelaNo.Enabled = false;
				txt_RelaNo.ReadOnly = true;
				tab_Attachments.PageEnabled = true;
				pic_Images.Enabled = true;
				WinCommons._CloseProcessing();
				if (ds.Tables[0].Rows.Count > 0)
				{
					_relaId = Function.ParseInt(ds.Tables[0].Rows[0]["relaID"]);
					pic_Images.Properties.NullText = "Loading...";
					if (!string.IsNullOrEmpty(Function.ToString(ds.Tables[0].Rows[0]["picture"])))
					{
						_strImage = Function.ToString(ds.Tables[0].Rows[0]["picture"]);
						LoadPicture();
					}
					else
					{
						pic_Images.Image = null;
						pic_Images.Properties.NullText = "No image data!";
					}
				}
			}
			_isLoading = false;
			SetFocusedRowBasedOnId(_relaId);
			WinCommons._CloseProcessing();
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

	private async Task LoadUDF()
	{
		try
		{
			Dictionary<string, GridViewEx> dictionary = new Dictionary<string, GridViewEx>();
			dictionary["APZ_OHER"] = gv_Udf;
			await WinFuntions.LoadUdfColumns(gv_Udf, "APZ_OHER", dictionary);
			if (!(grd_Udf.DataSource is DataTable dataTable) || dataTable.Rows.Count == 0)
			{
				WinFuntions.GetNewRow_UDF(grd_Udf, "APZ_OHER");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDataNavigations(int type)
	{
		_ = 3;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
			}
			else if (FormMode != Enums.FormViewModeType.Add || !_add)
			{
				await RefreshGrid();
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				await RefreshGrid();
			}
			WinCommons._OpenProcessing();
			await GetDetails(Function.ParseInt(await _httpClient.EmpRelativesClient.LoadDataNavigation(type, _relaId, EmpId)), EmpId);
			WinCommons._CloseProcessing();
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
			if (control is CheckEdit)
			{
				((CheckEdit)control).CheckStateChanged += OnContentChanged;
			}
			else if (control is ComboBoxEdit)
			{
				((ComboBoxEdit)control).SelectedIndexChanged += OnContentChanged;
			}
			else if (control is LookUpEdit)
			{
				((LookUpEdit)control).EditValueChanged += OnContentChanged;
				((LookUpEdit)control).Properties.ShowHeader = false;
				((LookUpEdit)control).Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
			}
			else if (control is DateEdit)
			{
				((DateEdit)control).EditValueChanged += OnContentChanged;
			}
			else if (control is TextEdit || control is MemoEdit)
			{
				control.TextChanged += OnContentChanged;
			}
			else
			{
				SetEventControl(control);
			}
		}
	}

	private void SetSuperTip(Control ctr)
	{
		foreach (Control control in ctr.Controls)
		{
			if (!(control is CheckEdit))
			{
				if (control is ComboBoxEdit)
				{
					SuperToolTip superToolTip = new SuperToolTip();
					ToolTipItem item = new ToolTipItem
					{
						Text = Function.ToString(((ComboBoxEdit)control).EditValue)
					};
					superToolTip.Items.Add(item);
					((ComboBoxEdit)control).SuperTip = superToolTip;
				}
				else if (control is LookUpEdit)
				{
					SuperToolTip superToolTip2 = new SuperToolTip();
					ToolTipItem item2 = new ToolTipItem
					{
						Text = Function.ToString(((LookUpEdit)control).Properties.GetDisplayText(((LookUpEdit)control).EditValue))
					};
					superToolTip2.Items.Add(item2);
					((LookUpEdit)control).SuperTip = superToolTip2;
				}
				else if (control is DateEdit)
				{
					SuperToolTip superToolTip3 = new SuperToolTip();
					ToolTipItem item3 = new ToolTipItem
					{
						Text = Function.ToString(((DateEdit)control).EditValue)
					};
					superToolTip3.Items.Add(item3);
					((DateEdit)control).SuperTip = superToolTip3;
				}
				else if (control is TextEdit)
				{
					SuperToolTip superToolTip4 = new SuperToolTip();
					ToolTipItem item4 = new ToolTipItem
					{
						Text = Function.ToString(((TextEdit)control).EditValue)
					};
					superToolTip4.Items.Add(item4);
					((TextEdit)control).SuperTip = superToolTip4;
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
			if (FormMode != Enums.FormViewModeType.Find)
			{
				SetEventControl(this);
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

	private bool VerifyFileExists(string path, int timeout)
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

	private void gv_Udf_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void LoadPicture()
	{
		try
		{
			string text = CopyFileShareHelper.GetUpLoadPath("jpg") + "\\" + _strImage;
			_picFilePath = "";
			if (!string.IsNullOrWhiteSpace(_strImage) && VerifyFileExists(text, 5000))
			{
				pic_Images.Image = null;
				string tempPath = Path.GetTempPath();
				tempPath = (_strImage.Contains("APZ-EmpRelatives-") ? (tempPath + _strImage) : (tempPath + "APZ-EmpRelatives-" + _strImage));
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
		}
		else
		{
			btn_Save.Enabled = false;
			Controls.EnableDisableControl(isEnable: false);
		}
	}

	private async Task RefreshGrid()
	{
		try
		{
			_flag = true;
			DataTable dataSource = await _httpClient.EmpRelativesClient.GetAllRecords(EmpId);
			gv_Content.BeginDataUpdate();
			grd_Content.DataSource = dataSource;
			grd_Content.RefreshDataSource();
			gv_Content.EndDataUpdate();
			_flag = false;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_EmpRelatives));
		this.tab_Detail = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tab_Main = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.date_Brith = new Apzon.Controls.Editor.DateEditEx();
		this.date_PassExp = new Apzon.Controls.Editor.DateEditEx();
		this.chk_FamDeduction = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_YearFormat = new Apzon.Controls.Editor.CheckEditEx();
		this.luk_PersGroup = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lbl_PersGroup = new Apzon.Controls.Controls.LabelLine();
		this.luk_Relationship = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lbl_Relationship = new Apzon.Controls.Controls.LabelLine();
		this.luk_Citizenship = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_PassNo = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_PassNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_PassExpDate = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Citizenship = new Apzon.Controls.Controls.LabelLine();
		this.luk_Gender = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_IDNo = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_IDNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Date = new Apzon.Controls.Controls.LabelLine();
		this.lbl_CountryOfBirth = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Gender = new Apzon.Controls.Controls.LabelLine();
		this.luk_CountryOfBirth = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.pic_Images = new DevExpress.XtraEditors.PictureEdit();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.txt_HomePhone = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Fax = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Email = new Apzon.Controls.Editor.TextEditEx();
		this.txt_ExtPhone = new Apzon.Controls.Editor.TextEditEx();
		this.txt_MobilePhone = new Apzon.Controls.Editor.TextEditEx();
		this.txt_OfficePhone = new Apzon.Controls.Editor.TextEditEx();
		this.txt_RelaNo = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_Email = new Apzon.Controls.Controls.LabelLine();
		this.txt_JobTitle = new Apzon.Controls.Editor.TextEditEx();
		this.txt_LastName = new Apzon.Controls.Editor.TextEditEx();
		this.txt_MiddleName = new Apzon.Controls.Editor.TextEditEx();
		this.txt_FirstName = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_Fax = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Home = new Apzon.Controls.Controls.LabelLine();
		this.lbl_off = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ext = new Apzon.Controls.Controls.LabelLine();
		this.lbl_MPhone = new Apzon.Controls.Controls.LabelLine();
		this.lbl_EmpNo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_MiddleName = new Apzon.Controls.Controls.LabelLine();
		this.lbl_LastName = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Job = new Apzon.Controls.Controls.LabelLine();
		this.lbl_FirstName = new Apzon.Controls.Controls.LabelLine();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.tab_Address = new Apzon.Controls.Controls.XtraTabPageEx();
		this.pnl_Address = new DevExpress.XtraEditors.PanelControl();
		this.luk_H_State = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_H_Country = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_W_Country = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_W_State = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.txt_H_Block = new Apzon.Controls.Editor.TextEditEx();
		this.txt_H_Street = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_H_Block = new Apzon.Controls.Controls.LabelLine();
		this.lbl_H_Street = new Apzon.Controls.Controls.LabelLine();
		this.txt_W_Street = new Apzon.Controls.Editor.TextEditEx();
		this.txt_W_Block = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_WStreet = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Block = new Apzon.Controls.Controls.LabelLine();
		this.txt_H_County = new Apzon.Controls.Editor.TextEditEx();
		this.txt_H_City = new Apzon.Controls.Editor.TextEditEx();
		this.txt_H_Zipcode = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_H_zipCode = new Apzon.Controls.Controls.LabelLine();
		this.lbl_H_city = new Apzon.Controls.Controls.LabelLine();
		this.lbl_H_County = new Apzon.Controls.Controls.LabelLine();
		this.lbl_H_State = new Apzon.Controls.Controls.LabelLine();
		this.lbl_H_Country = new Apzon.Controls.Controls.LabelLine();
		this.lbl_WorkAdress = new Apzon.Controls.Controls.LabelLine();
		this.lbl_HomeAddress = new Apzon.Controls.Controls.LabelLine();
		this.txt_W_County = new Apzon.Controls.Editor.TextEditEx();
		this.txt_W_City = new Apzon.Controls.Editor.TextEditEx();
		this.txt_W_Zipcode = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_Country = new Apzon.Controls.Controls.LabelLine();
		this.lbl_State = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ZipCode = new Apzon.Controls.Controls.LabelLine();
		this.lbl_County = new Apzon.Controls.Controls.LabelLine();
		this.lbl_City = new Apzon.Controls.Controls.LabelLine();
		this.tab_Remarks = new Apzon.Controls.Controls.XtraTabPageEx();
		this.txt_Remark = new Apzon.Controls.Editor.MemoEditEx();
		this.tab_Attachments = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl5 = new DevExpress.XtraEditors.PanelControl();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.tab_User = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Udf = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Udf = new Apzon.Controls.Grid.GridViewEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewtoolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Content = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Content = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_grdGender = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_grdRelation = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
		this.repositoryItemGridLookUpEdit1View = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.tab_Detail).BeginInit();
		this.tab_Detail.SuspendLayout();
		this.tab_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.date_Brith.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_Brith.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_PassExp.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_PassExp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_FamDeduction.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_YearFormat.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PersGroup.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Relationship.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Citizenship.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PassNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Gender.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_IDNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CountryOfBirth.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pic_Images.Properties).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_HomePhone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Email.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ExtPhone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MobilePhone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OfficePhone.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_RelaNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_JobTitle.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LastName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MiddleName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FirstName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		this.tab_Address.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.pnl_Address).BeginInit();
		this.pnl_Address.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_H_State.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_H_Country.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_W_Country.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_W_State.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Block.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Street.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Street.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Block.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_County.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_City.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Zipcode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_County.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_City.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Zipcode.Properties).BeginInit();
		this.tab_Remarks.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Remark.Properties).BeginInit();
		this.tab_Attachments.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl5).BeginInit();
		this.panelControl5.SuspendLayout();
		this.tab_User.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Udf).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Udf).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Content).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Content).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_grdGender).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_grdRelation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1View).BeginInit();
		base.SuspendLayout();
		this.tab_Detail.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tab_Detail.IsSearchControl = false;
		this.tab_Detail.Location = new System.Drawing.Point(3, 5);
		this.tab_Detail.Name = "tab_Detail";
		this.tab_Detail.SelectedTabPage = this.tab_Main;
		this.tab_Detail.Size = new System.Drawing.Size(874, 303);
		this.tab_Detail.TabIndex = 40;
		this.tab_Detail.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[5] { this.tab_Main, this.tab_Address, this.tab_Remarks, this.tab_Attachments, this.tab_User });
		this.tab_Main.Appearance.PageClient.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		this.tab_Main.Appearance.PageClient.Options.UseBackColor = true;
		this.tab_Main.Controls.Add(this.panelControl1);
		this.tab_Main.EditControl = null;
		this.tab_Main.EditValue = "Main";
		this.tab_Main.FieldName = null;
		this.tab_Main.IsSearchControl = false;
		this.tab_Main.Name = "tab_Main";
		this.tab_Main.ReadOnly = false;
		this.tab_Main.SettingInVisible = false;
		this.tab_Main.SettingReadonly = false;
		this.tab_Main.Size = new System.Drawing.Size(872, 278);
		this.tab_Main.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Main.Text = "Main";
		this.panelControl1.Controls.Add(this.date_Brith);
		this.panelControl1.Controls.Add(this.date_PassExp);
		this.panelControl1.Controls.Add(this.chk_FamDeduction);
		this.panelControl1.Controls.Add(this.chk_YearFormat);
		this.panelControl1.Controls.Add(this.luk_PersGroup);
		this.panelControl1.Controls.Add(this.lbl_PersGroup);
		this.panelControl1.Controls.Add(this.luk_Relationship);
		this.panelControl1.Controls.Add(this.lbl_Relationship);
		this.panelControl1.Controls.Add(this.luk_Citizenship);
		this.panelControl1.Controls.Add(this.txt_PassNo);
		this.panelControl1.Controls.Add(this.lbl_PassNo);
		this.panelControl1.Controls.Add(this.lbl_PassExpDate);
		this.panelControl1.Controls.Add(this.lbl_Citizenship);
		this.panelControl1.Controls.Add(this.luk_Gender);
		this.panelControl1.Controls.Add(this.txt_IDNo);
		this.panelControl1.Controls.Add(this.lbl_IDNo);
		this.panelControl1.Controls.Add(this.lbl_Date);
		this.panelControl1.Controls.Add(this.lbl_CountryOfBirth);
		this.panelControl1.Controls.Add(this.lbl_Gender);
		this.panelControl1.Controls.Add(this.luk_CountryOfBirth);
		this.panelControl1.Controls.Add(this.pic_Images);
		this.panelControl1.Controls.Add(this.txt_HomePhone);
		this.panelControl1.Controls.Add(this.txt_Fax);
		this.panelControl1.Controls.Add(this.txt_Email);
		this.panelControl1.Controls.Add(this.txt_ExtPhone);
		this.panelControl1.Controls.Add(this.txt_MobilePhone);
		this.panelControl1.Controls.Add(this.txt_OfficePhone);
		this.panelControl1.Controls.Add(this.txt_RelaNo);
		this.panelControl1.Controls.Add(this.lbl_Email);
		this.panelControl1.Controls.Add(this.txt_JobTitle);
		this.panelControl1.Controls.Add(this.txt_LastName);
		this.panelControl1.Controls.Add(this.txt_MiddleName);
		this.panelControl1.Controls.Add(this.txt_FirstName);
		this.panelControl1.Controls.Add(this.lbl_Fax);
		this.panelControl1.Controls.Add(this.lbl_Home);
		this.panelControl1.Controls.Add(this.lbl_off);
		this.panelControl1.Controls.Add(this.lbl_ext);
		this.panelControl1.Controls.Add(this.lbl_MPhone);
		this.panelControl1.Controls.Add(this.lbl_EmpNo);
		this.panelControl1.Controls.Add(this.lbl_MiddleName);
		this.panelControl1.Controls.Add(this.lbl_LastName);
		this.panelControl1.Controls.Add(this.lbl_Job);
		this.panelControl1.Controls.Add(this.lbl_FirstName);
		this.panelControl1.Controls.Add(this.date_FromDate);
		this.panelControl1.Controls.Add(this.date_ToDate);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(872, 278);
		this.panelControl1.TabIndex = 0;
		this.date_Brith.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_Brith.EditControl = null;
		this.date_Brith.EditValue = null;
		this.date_Brith.FieldName = "birthDate";
		this.date_Brith.Location = new System.Drawing.Point(501, 64);
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
		this.date_Brith.TabIndex = 13;
		this.date_PassExp.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_PassExp.EditControl = null;
		this.date_PassExp.EditValue = null;
		this.date_PassExp.FieldName = "passportEx";
		this.date_PassExp.Location = new System.Drawing.Point(501, 186);
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
		this.date_PassExp.TabIndex = 18;
		this.chk_FamDeduction.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_FamDeduction.EditControl = null;
		this.chk_FamDeduction.FieldName = "famDeduction";
		this.chk_FamDeduction.IsSearchControl = false;
		this.chk_FamDeduction.Location = new System.Drawing.Point(702, 186);
		this.chk_FamDeduction.Name = "chk_FamDeduction";
		this.chk_FamDeduction.Properties.Caption = "Family Deduction";
		this.chk_FamDeduction.Properties.ValueChecked = "Y";
		this.chk_FamDeduction.Properties.ValueUnchecked = "N";
		this.chk_FamDeduction.SettingInVisible = false;
		this.chk_FamDeduction.SettingReadonly = false;
		this.chk_FamDeduction.Size = new System.Drawing.Size(167, 20);
		this.chk_FamDeduction.TabIndex = 122;
		this.chk_FamDeduction.CheckedChanged += new System.EventHandler(this.chk_FamDeduction_CheckedChanged);
		this.chk_YearFormat.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.chk_YearFormat.EditControl = null;
		this.chk_YearFormat.FieldName = "birthOnlyYear";
		this.chk_YearFormat.IsSearchControl = false;
		this.chk_YearFormat.Location = new System.Drawing.Point(670, 66);
		this.chk_YearFormat.Name = "chk_YearFormat";
		this.chk_YearFormat.Properties.Caption = "Only Year";
		this.chk_YearFormat.Properties.ValueChecked = "Y";
		this.chk_YearFormat.Properties.ValueUnchecked = "N";
		this.chk_YearFormat.SettingInVisible = false;
		this.chk_YearFormat.SettingReadonly = false;
		this.chk_YearFormat.Size = new System.Drawing.Size(75, 20);
		this.chk_YearFormat.TabIndex = 121;
		this.chk_YearFormat.CheckedChanged += new System.EventHandler(this.chk_YearFormat_CheckedChanged);
		this.luk_PersGroup.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_PersGroup.EditControl = null;
		this.luk_PersGroup.FieldName = "PersGroup";
		this.luk_PersGroup.IsSearchControl = false;
		this.luk_PersGroup.Location = new System.Drawing.Point(501, 234);
		this.luk_PersGroup.Name = "luk_PersGroup";
		this.luk_PersGroup.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_PersGroup.Properties.IsSearchControl = false;
		this.luk_PersGroup.Properties.NullText = "";
		this.luk_PersGroup.Properties.PopupSizeable = false;
		this.luk_PersGroup.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_PersGroup.SettingInVisible = false;
		this.luk_PersGroup.SettingReadonly = false;
		this.luk_PersGroup.Size = new System.Drawing.Size(167, 20);
		this.luk_PersGroup.TabIndex = 20;
		this.lbl_PersGroup.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_PersGroup.EditControl = null;
		this.lbl_PersGroup.EditValue = "Person Group";
		this.lbl_PersGroup.FieldName = null;
		this.lbl_PersGroup.HideWhenEmpty = false;
		this.lbl_PersGroup.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_PersGroup.Location = new System.Drawing.Point(376, 237);
		this.lbl_PersGroup.Name = "lbl_PersGroup";
		this.lbl_PersGroup.ReadOnly = false;
		this.lbl_PersGroup.SettingInVisible = false;
		this.lbl_PersGroup.SettingReadonly = false;
		this.lbl_PersGroup.ShowUnderLine = true;
		this.lbl_PersGroup.Size = new System.Drawing.Size(126, 20);
		this.lbl_PersGroup.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_PersGroup.TabIndex = 119;
		this.lbl_PersGroup.Utext = "Person Group";
		this.luk_Relationship.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_Relationship.EditControl = null;
		this.luk_Relationship.FieldName = "relationship";
		this.luk_Relationship.IsSearchControl = false;
		this.luk_Relationship.Location = new System.Drawing.Point(501, 210);
		this.luk_Relationship.Name = "luk_Relationship";
		this.luk_Relationship.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Relationship.Properties.IsSearchControl = false;
		this.luk_Relationship.Properties.NullText = "";
		this.luk_Relationship.Properties.PopupSizeable = false;
		this.luk_Relationship.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.luk_Relationship.SettingInVisible = false;
		this.luk_Relationship.SettingReadonly = false;
		this.luk_Relationship.Size = new System.Drawing.Size(167, 20);
		this.luk_Relationship.TabIndex = 19;
		this.lbl_Relationship.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Relationship.EditControl = null;
		this.lbl_Relationship.EditValue = "Relationship";
		this.lbl_Relationship.FieldName = null;
		this.lbl_Relationship.HideWhenEmpty = false;
		this.lbl_Relationship.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Relationship.Location = new System.Drawing.Point(376, 213);
		this.lbl_Relationship.Name = "lbl_Relationship";
		this.lbl_Relationship.ReadOnly = false;
		this.lbl_Relationship.SettingInVisible = false;
		this.lbl_Relationship.SettingReadonly = false;
		this.lbl_Relationship.ShowUnderLine = true;
		this.lbl_Relationship.Size = new System.Drawing.Size(126, 20);
		this.lbl_Relationship.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Relationship.TabIndex = 117;
		this.lbl_Relationship.Utext = "Relationship";
		this.luk_Citizenship.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_Citizenship.EditControl = null;
		this.luk_Citizenship.FieldName = "citizenshp";
		this.luk_Citizenship.IsSearchControl = false;
		this.luk_Citizenship.Location = new System.Drawing.Point(501, 136);
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
		this.luk_Citizenship.TabIndex = 16;
		this.txt_PassNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_PassNo.EditControl = null;
		this.txt_PassNo.FieldName = "passportNo";
		this.txt_PassNo.IsSearchControl = false;
		this.txt_PassNo.Location = new System.Drawing.Point(501, 160);
		this.txt_PassNo.Name = "txt_PassNo";
		this.txt_PassNo.SettingInVisible = false;
		this.txt_PassNo.SettingReadonly = false;
		this.txt_PassNo.Size = new System.Drawing.Size(167, 20);
		this.txt_PassNo.TabIndex = 17;
		this.lbl_PassNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_PassNo.EditControl = null;
		this.lbl_PassNo.EditValue = "Passport No.";
		this.lbl_PassNo.FieldName = null;
		this.lbl_PassNo.HideWhenEmpty = false;
		this.lbl_PassNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_PassNo.Location = new System.Drawing.Point(376, 163);
		this.lbl_PassNo.Name = "lbl_PassNo";
		this.lbl_PassNo.ReadOnly = false;
		this.lbl_PassNo.SettingInVisible = false;
		this.lbl_PassNo.SettingReadonly = false;
		this.lbl_PassNo.ShowUnderLine = true;
		this.lbl_PassNo.Size = new System.Drawing.Size(126, 20);
		this.lbl_PassNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_PassNo.TabIndex = 113;
		this.lbl_PassNo.Utext = "Passport No.";
		this.lbl_PassExpDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_PassExpDate.EditControl = null;
		this.lbl_PassExpDate.EditValue = "Passport Expiration Date";
		this.lbl_PassExpDate.FieldName = null;
		this.lbl_PassExpDate.HideWhenEmpty = false;
		this.lbl_PassExpDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_PassExpDate.Location = new System.Drawing.Point(376, 189);
		this.lbl_PassExpDate.Name = "lbl_PassExpDate";
		this.lbl_PassExpDate.ReadOnly = false;
		this.lbl_PassExpDate.SettingInVisible = false;
		this.lbl_PassExpDate.SettingReadonly = false;
		this.lbl_PassExpDate.ShowUnderLine = true;
		this.lbl_PassExpDate.Size = new System.Drawing.Size(126, 20);
		this.lbl_PassExpDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_PassExpDate.TabIndex = 112;
		this.lbl_PassExpDate.Utext = "Passport Expiration Date";
		this.lbl_Citizenship.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Citizenship.EditControl = null;
		this.lbl_Citizenship.EditValue = "Citizenship";
		this.lbl_Citizenship.FieldName = null;
		this.lbl_Citizenship.HideWhenEmpty = false;
		this.lbl_Citizenship.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Citizenship.Location = new System.Drawing.Point(376, 139);
		this.lbl_Citizenship.Name = "lbl_Citizenship";
		this.lbl_Citizenship.ReadOnly = false;
		this.lbl_Citizenship.SettingInVisible = false;
		this.lbl_Citizenship.SettingReadonly = false;
		this.lbl_Citizenship.ShowUnderLine = true;
		this.lbl_Citizenship.Size = new System.Drawing.Size(126, 20);
		this.lbl_Citizenship.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Citizenship.TabIndex = 111;
		this.lbl_Citizenship.Utext = "Citizenship";
		this.luk_Gender.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_Gender.EditControl = null;
		this.luk_Gender.FieldName = "sex";
		this.luk_Gender.IsSearchControl = false;
		this.luk_Gender.Location = new System.Drawing.Point(501, 41);
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
		this.luk_Gender.TabIndex = 12;
		this.txt_IDNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_IDNo.EditControl = null;
		this.txt_IDNo.FieldName = "govID";
		this.txt_IDNo.IsSearchControl = false;
		this.txt_IDNo.Location = new System.Drawing.Point(501, 112);
		this.txt_IDNo.Name = "txt_IDNo";
		this.txt_IDNo.SettingInVisible = false;
		this.txt_IDNo.SettingReadonly = false;
		this.txt_IDNo.Size = new System.Drawing.Size(167, 20);
		this.txt_IDNo.TabIndex = 15;
		this.lbl_IDNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_IDNo.EditControl = null;
		this.lbl_IDNo.EditValue = "ID No.";
		this.lbl_IDNo.FieldName = null;
		this.lbl_IDNo.HideWhenEmpty = false;
		this.lbl_IDNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_IDNo.Location = new System.Drawing.Point(377, 115);
		this.lbl_IDNo.Name = "lbl_IDNo";
		this.lbl_IDNo.ReadOnly = false;
		this.lbl_IDNo.SettingInVisible = false;
		this.lbl_IDNo.SettingReadonly = false;
		this.lbl_IDNo.ShowUnderLine = true;
		this.lbl_IDNo.Size = new System.Drawing.Size(125, 20);
		this.lbl_IDNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_IDNo.TabIndex = 105;
		this.lbl_IDNo.Utext = "ID No.";
		this.lbl_Date.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Date.EditControl = null;
		this.lbl_Date.EditValue = "Date of Birth";
		this.lbl_Date.FieldName = null;
		this.lbl_Date.HideWhenEmpty = false;
		this.lbl_Date.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Date.Location = new System.Drawing.Point(376, 66);
		this.lbl_Date.Name = "lbl_Date";
		this.lbl_Date.ReadOnly = false;
		this.lbl_Date.SettingInVisible = false;
		this.lbl_Date.SettingReadonly = false;
		this.lbl_Date.ShowUnderLine = true;
		this.lbl_Date.Size = new System.Drawing.Size(126, 20);
		this.lbl_Date.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Date.TabIndex = 104;
		this.lbl_Date.Utext = "Date of Birth";
		this.lbl_CountryOfBirth.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_CountryOfBirth.EditControl = null;
		this.lbl_CountryOfBirth.EditValue = "Country of Birth";
		this.lbl_CountryOfBirth.FieldName = null;
		this.lbl_CountryOfBirth.HideWhenEmpty = false;
		this.lbl_CountryOfBirth.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_CountryOfBirth.Location = new System.Drawing.Point(376, 89);
		this.lbl_CountryOfBirth.Name = "lbl_CountryOfBirth";
		this.lbl_CountryOfBirth.ReadOnly = false;
		this.lbl_CountryOfBirth.SettingInVisible = false;
		this.lbl_CountryOfBirth.SettingReadonly = false;
		this.lbl_CountryOfBirth.ShowUnderLine = true;
		this.lbl_CountryOfBirth.Size = new System.Drawing.Size(125, 20);
		this.lbl_CountryOfBirth.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_CountryOfBirth.TabIndex = 103;
		this.lbl_CountryOfBirth.Utext = "Country of Birth";
		this.lbl_Gender.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_Gender.EditControl = null;
		this.lbl_Gender.EditValue = "Gender";
		this.lbl_Gender.FieldName = null;
		this.lbl_Gender.HideWhenEmpty = false;
		this.lbl_Gender.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Gender.Location = new System.Drawing.Point(376, 44);
		this.lbl_Gender.Name = "lbl_Gender";
		this.lbl_Gender.ReadOnly = false;
		this.lbl_Gender.SettingInVisible = false;
		this.lbl_Gender.SettingReadonly = false;
		this.lbl_Gender.ShowUnderLine = true;
		this.lbl_Gender.Size = new System.Drawing.Size(126, 20);
		this.lbl_Gender.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Gender.TabIndex = 100;
		this.lbl_Gender.Utext = "Gender";
		this.luk_CountryOfBirth.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_CountryOfBirth.EditControl = null;
		this.luk_CountryOfBirth.FieldName = "brthCountr";
		this.luk_CountryOfBirth.IsSearchControl = false;
		this.luk_CountryOfBirth.Location = new System.Drawing.Point(501, 87);
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
		this.luk_CountryOfBirth.TabIndex = 14;
		this.pic_Images.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.pic_Images.Cursor = System.Windows.Forms.Cursors.Default;
		this.pic_Images.Location = new System.Drawing.Point(764, 20);
		this.pic_Images.Name = "pic_Images";
		this.pic_Images.Properties.ContextMenuStrip = this.contextMenuStrip2;
		this.pic_Images.Size = new System.Drawing.Size(105, 140);
		this.pic_Images.TabIndex = 99;
		this.pic_Images.Click += new System.EventHandler(this.pic_Images_Click);
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem1 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(148, 26);
		this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
		this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(147, 22);
		this.deleteRowToolStripMenuItem1.Text = "Delete Picture";
		this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deletePictureToolStripMenuItem_Click);
		this.txt_HomePhone.EditControl = null;
		this.txt_HomePhone.FieldName = "homeTel";
		this.txt_HomePhone.IsSearchControl = false;
		this.txt_HomePhone.Location = new System.Drawing.Point(128, 186);
		this.txt_HomePhone.Name = "txt_HomePhone";
		this.txt_HomePhone.SettingInVisible = false;
		this.txt_HomePhone.SettingReadonly = false;
		this.txt_HomePhone.Size = new System.Drawing.Size(169, 20);
		this.txt_HomePhone.TabIndex = 8;
		this.txt_Fax.EditControl = null;
		this.txt_Fax.FieldName = "fax";
		this.txt_Fax.IsSearchControl = false;
		this.txt_Fax.Location = new System.Drawing.Point(128, 210);
		this.txt_Fax.Name = "txt_Fax";
		this.txt_Fax.SettingInVisible = false;
		this.txt_Fax.SettingReadonly = false;
		this.txt_Fax.Size = new System.Drawing.Size(169, 20);
		this.txt_Fax.TabIndex = 9;
		this.txt_Email.EditControl = null;
		this.txt_Email.FieldName = "email";
		this.txt_Email.IsSearchControl = false;
		this.txt_Email.Location = new System.Drawing.Point(128, 234);
		this.txt_Email.Name = "txt_Email";
		this.txt_Email.SettingInVisible = false;
		this.txt_Email.SettingReadonly = false;
		this.txt_Email.Size = new System.Drawing.Size(169, 20);
		this.txt_Email.TabIndex = 10;
		this.txt_ExtPhone.EditControl = null;
		this.txt_ExtPhone.FieldName = "officeExt";
		this.txt_ExtPhone.IsSearchControl = false;
		this.txt_ExtPhone.Location = new System.Drawing.Point(128, 136);
		this.txt_ExtPhone.Name = "txt_ExtPhone";
		this.txt_ExtPhone.SettingInVisible = false;
		this.txt_ExtPhone.SettingReadonly = false;
		this.txt_ExtPhone.Size = new System.Drawing.Size(169, 20);
		this.txt_ExtPhone.TabIndex = 6;
		this.txt_MobilePhone.EditControl = null;
		this.txt_MobilePhone.FieldName = "mobile";
		this.txt_MobilePhone.IsSearchControl = false;
		this.txt_MobilePhone.Location = new System.Drawing.Point(128, 160);
		this.txt_MobilePhone.Name = "txt_MobilePhone";
		this.txt_MobilePhone.SettingInVisible = false;
		this.txt_MobilePhone.SettingReadonly = false;
		this.txt_MobilePhone.Size = new System.Drawing.Size(169, 20);
		this.txt_MobilePhone.TabIndex = 7;
		this.txt_OfficePhone.EditControl = null;
		this.txt_OfficePhone.FieldName = "officeTel";
		this.txt_OfficePhone.IsSearchControl = false;
		this.txt_OfficePhone.Location = new System.Drawing.Point(128, 113);
		this.txt_OfficePhone.Name = "txt_OfficePhone";
		this.txt_OfficePhone.SettingInVisible = false;
		this.txt_OfficePhone.SettingReadonly = false;
		this.txt_OfficePhone.Size = new System.Drawing.Size(169, 20);
		this.txt_OfficePhone.TabIndex = 5;
		this.txt_RelaNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_RelaNo.EditControl = null;
		this.txt_RelaNo.FieldName = "relaID";
		this.txt_RelaNo.IsSearchControl = false;
		this.txt_RelaNo.Location = new System.Drawing.Point(501, 18);
		this.txt_RelaNo.Name = "txt_RelaNo";
		this.txt_RelaNo.Properties.ReadOnly = true;
		this.txt_RelaNo.SettingInVisible = false;
		this.txt_RelaNo.SettingReadonly = false;
		this.txt_RelaNo.Size = new System.Drawing.Size(167, 20);
		this.txt_RelaNo.TabIndex = 11;
		this.lbl_Email.EditControl = null;
		this.lbl_Email.EditValue = "E-Mail";
		this.lbl_Email.FieldName = null;
		this.lbl_Email.HideWhenEmpty = false;
		this.lbl_Email.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Email.Location = new System.Drawing.Point(5, 237);
		this.lbl_Email.Name = "lbl_Email";
		this.lbl_Email.ReadOnly = false;
		this.lbl_Email.SettingInVisible = false;
		this.lbl_Email.SettingReadonly = false;
		this.lbl_Email.ShowUnderLine = true;
		this.lbl_Email.Size = new System.Drawing.Size(126, 20);
		this.lbl_Email.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Email.TabIndex = 88;
		this.lbl_Email.Utext = "E-Mail";
		this.txt_JobTitle.EditControl = null;
		this.txt_JobTitle.FieldName = "jobTitle";
		this.txt_JobTitle.IsSearchControl = false;
		this.txt_JobTitle.Location = new System.Drawing.Point(128, 87);
		this.txt_JobTitle.Name = "txt_JobTitle";
		this.txt_JobTitle.SettingInVisible = false;
		this.txt_JobTitle.SettingReadonly = false;
		this.txt_JobTitle.Size = new System.Drawing.Size(169, 20);
		this.txt_JobTitle.TabIndex = 4;
		this.txt_LastName.EditControl = null;
		this.txt_LastName.FieldName = "lastName";
		this.txt_LastName.IsSearchControl = false;
		this.txt_LastName.Location = new System.Drawing.Point(128, 64);
		this.txt_LastName.Name = "txt_LastName";
		this.txt_LastName.SettingInVisible = false;
		this.txt_LastName.SettingReadonly = false;
		this.txt_LastName.Size = new System.Drawing.Size(169, 20);
		this.txt_LastName.TabIndex = 3;
		this.txt_MiddleName.EditControl = null;
		this.txt_MiddleName.FieldName = "middleName";
		this.txt_MiddleName.IsSearchControl = false;
		this.txt_MiddleName.Location = new System.Drawing.Point(128, 41);
		this.txt_MiddleName.Name = "txt_MiddleName";
		this.txt_MiddleName.SettingInVisible = false;
		this.txt_MiddleName.SettingReadonly = false;
		this.txt_MiddleName.Size = new System.Drawing.Size(169, 20);
		this.txt_MiddleName.TabIndex = 2;
		this.txt_FirstName.EditControl = null;
		this.txt_FirstName.FieldName = "firstName";
		this.txt_FirstName.IsSearchControl = false;
		this.txt_FirstName.Location = new System.Drawing.Point(128, 18);
		this.txt_FirstName.Name = "txt_FirstName";
		this.txt_FirstName.SettingInVisible = false;
		this.txt_FirstName.SettingReadonly = false;
		this.txt_FirstName.Size = new System.Drawing.Size(169, 20);
		this.txt_FirstName.TabIndex = 1;
		this.lbl_Fax.EditControl = null;
		this.lbl_Fax.EditValue = "Fax";
		this.lbl_Fax.FieldName = null;
		this.lbl_Fax.HideWhenEmpty = false;
		this.lbl_Fax.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Fax.Location = new System.Drawing.Point(5, 213);
		this.lbl_Fax.Name = "lbl_Fax";
		this.lbl_Fax.ReadOnly = false;
		this.lbl_Fax.SettingInVisible = false;
		this.lbl_Fax.SettingReadonly = false;
		this.lbl_Fax.ShowUnderLine = true;
		this.lbl_Fax.Size = new System.Drawing.Size(126, 20);
		this.lbl_Fax.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Fax.TabIndex = 77;
		this.lbl_Fax.Utext = "Fax";
		this.lbl_Home.EditControl = null;
		this.lbl_Home.EditValue = "Home Phone";
		this.lbl_Home.FieldName = null;
		this.lbl_Home.HideWhenEmpty = false;
		this.lbl_Home.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Home.Location = new System.Drawing.Point(5, 188);
		this.lbl_Home.Name = "lbl_Home";
		this.lbl_Home.ReadOnly = false;
		this.lbl_Home.SettingInVisible = false;
		this.lbl_Home.SettingReadonly = false;
		this.lbl_Home.ShowUnderLine = true;
		this.lbl_Home.Size = new System.Drawing.Size(126, 20);
		this.lbl_Home.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Home.TabIndex = 76;
		this.lbl_Home.Utext = "Home Phone";
		this.lbl_off.EditControl = null;
		this.lbl_off.EditValue = "Office Phone";
		this.lbl_off.FieldName = null;
		this.lbl_off.HideWhenEmpty = false;
		this.lbl_off.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_off.Location = new System.Drawing.Point(5, 116);
		this.lbl_off.Name = "lbl_off";
		this.lbl_off.ReadOnly = false;
		this.lbl_off.SettingInVisible = false;
		this.lbl_off.SettingReadonly = false;
		this.lbl_off.ShowUnderLine = true;
		this.lbl_off.Size = new System.Drawing.Size(126, 20);
		this.lbl_off.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_off.TabIndex = 74;
		this.lbl_off.Utext = "Office Phone";
		this.lbl_ext.EditControl = null;
		this.lbl_ext.EditValue = "Ext.";
		this.lbl_ext.FieldName = null;
		this.lbl_ext.HideWhenEmpty = false;
		this.lbl_ext.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ext.Location = new System.Drawing.Point(5, 139);
		this.lbl_ext.Name = "lbl_ext";
		this.lbl_ext.ReadOnly = false;
		this.lbl_ext.SettingInVisible = false;
		this.lbl_ext.SettingReadonly = false;
		this.lbl_ext.ShowUnderLine = true;
		this.lbl_ext.Size = new System.Drawing.Size(126, 20);
		this.lbl_ext.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ext.TabIndex = 73;
		this.lbl_ext.Utext = "Ext.";
		this.lbl_MPhone.EditControl = null;
		this.lbl_MPhone.EditValue = "Mobile Phone";
		this.lbl_MPhone.FieldName = null;
		this.lbl_MPhone.HideWhenEmpty = false;
		this.lbl_MPhone.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_MPhone.Location = new System.Drawing.Point(5, 163);
		this.lbl_MPhone.Name = "lbl_MPhone";
		this.lbl_MPhone.ReadOnly = false;
		this.lbl_MPhone.SettingInVisible = false;
		this.lbl_MPhone.SettingReadonly = false;
		this.lbl_MPhone.ShowUnderLine = true;
		this.lbl_MPhone.Size = new System.Drawing.Size(126, 20);
		this.lbl_MPhone.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_MPhone.TabIndex = 72;
		this.lbl_MPhone.Utext = "Mobile Phone";
		this.lbl_EmpNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_EmpNo.EditControl = null;
		this.lbl_EmpNo.EditValue = "Relationship No.";
		this.lbl_EmpNo.FieldName = null;
		this.lbl_EmpNo.HideWhenEmpty = false;
		this.lbl_EmpNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_EmpNo.Location = new System.Drawing.Point(376, 22);
		this.lbl_EmpNo.Name = "lbl_EmpNo";
		this.lbl_EmpNo.ReadOnly = false;
		this.lbl_EmpNo.SettingInVisible = false;
		this.lbl_EmpNo.SettingReadonly = false;
		this.lbl_EmpNo.ShowUnderLine = true;
		this.lbl_EmpNo.Size = new System.Drawing.Size(126, 20);
		this.lbl_EmpNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_EmpNo.TabIndex = 70;
		this.lbl_EmpNo.Utext = "Relationship No.";
		this.lbl_MiddleName.EditControl = null;
		this.lbl_MiddleName.EditValue = "Middle Name";
		this.lbl_MiddleName.FieldName = null;
		this.lbl_MiddleName.HideWhenEmpty = false;
		this.lbl_MiddleName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_MiddleName.Location = new System.Drawing.Point(4, 43);
		this.lbl_MiddleName.Name = "lbl_MiddleName";
		this.lbl_MiddleName.ReadOnly = false;
		this.lbl_MiddleName.SettingInVisible = false;
		this.lbl_MiddleName.SettingReadonly = false;
		this.lbl_MiddleName.ShowUnderLine = true;
		this.lbl_MiddleName.Size = new System.Drawing.Size(127, 20);
		this.lbl_MiddleName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_MiddleName.TabIndex = 66;
		this.lbl_MiddleName.Utext = "Middle Name";
		this.lbl_LastName.EditControl = null;
		this.lbl_LastName.EditValue = "Last Name";
		this.lbl_LastName.FieldName = null;
		this.lbl_LastName.HideWhenEmpty = false;
		this.lbl_LastName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_LastName.Location = new System.Drawing.Point(4, 67);
		this.lbl_LastName.Name = "lbl_LastName";
		this.lbl_LastName.ReadOnly = false;
		this.lbl_LastName.SettingInVisible = false;
		this.lbl_LastName.SettingReadonly = false;
		this.lbl_LastName.ShowUnderLine = true;
		this.lbl_LastName.Size = new System.Drawing.Size(127, 20);
		this.lbl_LastName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_LastName.TabIndex = 65;
		this.lbl_LastName.Utext = "Last Name";
		this.lbl_Job.EditControl = null;
		this.lbl_Job.EditValue = "Job Title";
		this.lbl_Job.FieldName = null;
		this.lbl_Job.HideWhenEmpty = false;
		this.lbl_Job.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Job.Location = new System.Drawing.Point(4, 90);
		this.lbl_Job.Name = "lbl_Job";
		this.lbl_Job.ReadOnly = false;
		this.lbl_Job.SettingInVisible = false;
		this.lbl_Job.SettingReadonly = false;
		this.lbl_Job.ShowUnderLine = true;
		this.lbl_Job.Size = new System.Drawing.Size(127, 20);
		this.lbl_Job.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Job.TabIndex = 64;
		this.lbl_Job.Utext = "Job Title";
		this.lbl_FirstName.EditControl = null;
		this.lbl_FirstName.EditValue = "First Name";
		this.lbl_FirstName.FieldName = null;
		this.lbl_FirstName.HideWhenEmpty = false;
		this.lbl_FirstName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_FirstName.Location = new System.Drawing.Point(4, 21);
		this.lbl_FirstName.Name = "lbl_FirstName";
		this.lbl_FirstName.ReadOnly = false;
		this.lbl_FirstName.SettingInVisible = false;
		this.lbl_FirstName.SettingReadonly = false;
		this.lbl_FirstName.ShowUnderLine = true;
		this.lbl_FirstName.Size = new System.Drawing.Size(127, 20);
		this.lbl_FirstName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_FirstName.TabIndex = 61;
		this.lbl_FirstName.Utext = "First Name";
		this.date_FromDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = "deducFromDate";
		this.date_FromDate.Location = new System.Drawing.Point(702, 210);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.IsSearchControl = false;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(167, 20);
		this.date_FromDate.TabIndex = 123;
		this.date_ToDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = "deducToDate";
		this.date_ToDate.Location = new System.Drawing.Point(702, 234);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = false;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(167, 20);
		this.date_ToDate.TabIndex = 124;
		this.tab_Address.Controls.Add(this.pnl_Address);
		this.tab_Address.EditControl = null;
		this.tab_Address.EditValue = "Address";
		this.tab_Address.FieldName = null;
		this.tab_Address.IsSearchControl = false;
		this.tab_Address.Name = "tab_Address";
		this.tab_Address.ReadOnly = false;
		this.tab_Address.SettingInVisible = false;
		this.tab_Address.SettingReadonly = false;
		this.tab_Address.Size = new System.Drawing.Size(872, 278);
		this.tab_Address.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Address.Text = "Address";
		this.pnl_Address.Controls.Add(this.luk_H_State);
		this.pnl_Address.Controls.Add(this.luk_H_Country);
		this.pnl_Address.Controls.Add(this.luk_W_Country);
		this.pnl_Address.Controls.Add(this.luk_W_State);
		this.pnl_Address.Controls.Add(this.txt_H_Block);
		this.pnl_Address.Controls.Add(this.txt_H_Street);
		this.pnl_Address.Controls.Add(this.lbl_H_Block);
		this.pnl_Address.Controls.Add(this.lbl_H_Street);
		this.pnl_Address.Controls.Add(this.txt_W_Street);
		this.pnl_Address.Controls.Add(this.txt_W_Block);
		this.pnl_Address.Controls.Add(this.lbl_WStreet);
		this.pnl_Address.Controls.Add(this.lbl_Block);
		this.pnl_Address.Controls.Add(this.txt_H_County);
		this.pnl_Address.Controls.Add(this.txt_H_City);
		this.pnl_Address.Controls.Add(this.txt_H_Zipcode);
		this.pnl_Address.Controls.Add(this.lbl_H_zipCode);
		this.pnl_Address.Controls.Add(this.lbl_H_city);
		this.pnl_Address.Controls.Add(this.lbl_H_County);
		this.pnl_Address.Controls.Add(this.lbl_H_State);
		this.pnl_Address.Controls.Add(this.lbl_H_Country);
		this.pnl_Address.Controls.Add(this.lbl_WorkAdress);
		this.pnl_Address.Controls.Add(this.lbl_HomeAddress);
		this.pnl_Address.Controls.Add(this.txt_W_County);
		this.pnl_Address.Controls.Add(this.txt_W_City);
		this.pnl_Address.Controls.Add(this.txt_W_Zipcode);
		this.pnl_Address.Controls.Add(this.lbl_Country);
		this.pnl_Address.Controls.Add(this.lbl_State);
		this.pnl_Address.Controls.Add(this.lbl_ZipCode);
		this.pnl_Address.Controls.Add(this.lbl_County);
		this.pnl_Address.Controls.Add(this.lbl_City);
		this.pnl_Address.Dock = System.Windows.Forms.DockStyle.Fill;
		this.pnl_Address.Location = new System.Drawing.Point(0, 0);
		this.pnl_Address.Name = "pnl_Address";
		this.pnl_Address.Size = new System.Drawing.Size(872, 278);
		this.pnl_Address.TabIndex = 6;
		this.luk_H_State.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_H_State.EditControl = null;
		this.luk_H_State.FieldName = "luk_H_State";
		this.luk_H_State.IsSearchControl = false;
		this.luk_H_State.Location = new System.Drawing.Point(591, 149);
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
		this.luk_H_State.TabIndex = 33;
		this.luk_H_Country.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.luk_H_Country.EditControl = null;
		this.luk_H_Country.FieldName = "homeCountr";
		this.luk_H_Country.IsSearchControl = false;
		this.luk_H_Country.Location = new System.Drawing.Point(591, 173);
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
		this.luk_H_Country.TabIndex = 34;
		this.luk_H_Country.EditValueChanged += new System.EventHandler(this.luk_H_Country_EditValueChanged);
		this.luk_W_Country.EditControl = null;
		this.luk_W_Country.FieldName = "luk_W_Country";
		this.luk_W_Country.IsSearchControl = false;
		this.luk_W_Country.Location = new System.Drawing.Point(114, 173);
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
		this.luk_W_Country.TabIndex = 27;
		this.luk_W_Country.EditValueChanged += new System.EventHandler(this.luk_W_Country_EditValueChanged);
		this.luk_W_State.EditControl = null;
		this.luk_W_State.FieldName = "luk_W_State";
		this.luk_W_State.IsSearchControl = false;
		this.luk_W_State.Location = new System.Drawing.Point(114, 149);
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
		this.luk_W_State.TabIndex = 26;
		this.txt_H_Block.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_H_Block.EditControl = null;
		this.txt_H_Block.FieldName = "txt_H_Block";
		this.txt_H_Block.IsSearchControl = false;
		this.txt_H_Block.Location = new System.Drawing.Point(591, 54);
		this.txt_H_Block.Name = "txt_H_Block";
		this.txt_H_Block.SettingInVisible = false;
		this.txt_H_Block.SettingReadonly = false;
		this.txt_H_Block.Size = new System.Drawing.Size(167, 20);
		this.txt_H_Block.TabIndex = 29;
		this.txt_H_Street.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_H_Street.EditControl = null;
		this.txt_H_Street.FieldName = "txt_H_Street";
		this.txt_H_Street.IsSearchControl = false;
		this.txt_H_Street.Location = new System.Drawing.Point(591, 28);
		this.txt_H_Street.Name = "txt_H_Street";
		this.txt_H_Street.SettingInVisible = false;
		this.txt_H_Street.SettingReadonly = false;
		this.txt_H_Street.Size = new System.Drawing.Size(167, 20);
		this.txt_H_Street.TabIndex = 28;
		this.lbl_H_Block.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_Block.EditControl = null;
		this.lbl_H_Block.EditValue = "Block";
		this.lbl_H_Block.FieldName = null;
		this.lbl_H_Block.HideWhenEmpty = false;
		this.lbl_H_Block.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_Block.Location = new System.Drawing.Point(489, 57);
		this.lbl_H_Block.Name = "lbl_H_Block";
		this.lbl_H_Block.ReadOnly = false;
		this.lbl_H_Block.SettingInVisible = false;
		this.lbl_H_Block.SettingReadonly = false;
		this.lbl_H_Block.ShowUnderLine = true;
		this.lbl_H_Block.Size = new System.Drawing.Size(103, 20);
		this.lbl_H_Block.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_Block.TabIndex = 73;
		this.lbl_H_Block.Utext = "Block";
		this.lbl_H_Street.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_Street.EditControl = null;
		this.lbl_H_Street.EditValue = "Street";
		this.lbl_H_Street.FieldName = null;
		this.lbl_H_Street.HideWhenEmpty = false;
		this.lbl_H_Street.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_Street.Location = new System.Drawing.Point(489, 31);
		this.lbl_H_Street.Name = "lbl_H_Street";
		this.lbl_H_Street.ReadOnly = false;
		this.lbl_H_Street.SettingInVisible = false;
		this.lbl_H_Street.SettingReadonly = false;
		this.lbl_H_Street.ShowUnderLine = true;
		this.lbl_H_Street.Size = new System.Drawing.Size(103, 20);
		this.lbl_H_Street.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_Street.TabIndex = 72;
		this.lbl_H_Street.Utext = "Street";
		this.txt_W_Street.EditControl = null;
		this.txt_W_Street.FieldName = "txt_W_Street";
		this.txt_W_Street.IsSearchControl = false;
		this.txt_W_Street.Location = new System.Drawing.Point(114, 28);
		this.txt_W_Street.Name = "txt_W_Street";
		this.txt_W_Street.SettingInVisible = false;
		this.txt_W_Street.SettingReadonly = false;
		this.txt_W_Street.Size = new System.Drawing.Size(167, 20);
		this.txt_W_Street.TabIndex = 21;
		this.txt_W_Block.EditControl = null;
		this.txt_W_Block.FieldName = "txt_W_Block";
		this.txt_W_Block.IsSearchControl = false;
		this.txt_W_Block.Location = new System.Drawing.Point(114, 54);
		this.txt_W_Block.Name = "txt_W_Block";
		this.txt_W_Block.SettingInVisible = false;
		this.txt_W_Block.SettingReadonly = false;
		this.txt_W_Block.Size = new System.Drawing.Size(167, 20);
		this.txt_W_Block.TabIndex = 22;
		this.lbl_WStreet.EditControl = null;
		this.lbl_WStreet.EditValue = "Street";
		this.lbl_WStreet.FieldName = null;
		this.lbl_WStreet.HideWhenEmpty = false;
		this.lbl_WStreet.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_WStreet.Location = new System.Drawing.Point(12, 31);
		this.lbl_WStreet.Name = "lbl_WStreet";
		this.lbl_WStreet.ReadOnly = false;
		this.lbl_WStreet.SettingInVisible = false;
		this.lbl_WStreet.SettingReadonly = false;
		this.lbl_WStreet.ShowUnderLine = true;
		this.lbl_WStreet.Size = new System.Drawing.Size(103, 20);
		this.lbl_WStreet.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_WStreet.TabIndex = 66;
		this.lbl_WStreet.Utext = "Street";
		this.lbl_Block.EditControl = null;
		this.lbl_Block.EditValue = "Block";
		this.lbl_Block.FieldName = null;
		this.lbl_Block.HideWhenEmpty = false;
		this.lbl_Block.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Block.Location = new System.Drawing.Point(12, 56);
		this.lbl_Block.Name = "lbl_Block";
		this.lbl_Block.ReadOnly = false;
		this.lbl_Block.SettingInVisible = false;
		this.lbl_Block.SettingReadonly = false;
		this.lbl_Block.ShowUnderLine = true;
		this.lbl_Block.Size = new System.Drawing.Size(103, 20);
		this.lbl_Block.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Block.TabIndex = 67;
		this.lbl_Block.Utext = "Block";
		this.txt_H_County.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_H_County.EditControl = null;
		this.txt_H_County.FieldName = "txt_H_County";
		this.txt_H_County.IsSearchControl = false;
		this.txt_H_County.Location = new System.Drawing.Point(591, 126);
		this.txt_H_County.Name = "txt_H_County";
		this.txt_H_County.SettingInVisible = false;
		this.txt_H_County.SettingReadonly = false;
		this.txt_H_County.Size = new System.Drawing.Size(167, 20);
		this.txt_H_County.TabIndex = 32;
		this.txt_H_City.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_H_City.EditControl = null;
		this.txt_H_City.FieldName = "txt_H_City";
		this.txt_H_City.IsSearchControl = false;
		this.txt_H_City.Location = new System.Drawing.Point(591, 103);
		this.txt_H_City.Name = "txt_H_City";
		this.txt_H_City.SettingInVisible = false;
		this.txt_H_City.SettingReadonly = false;
		this.txt_H_City.Size = new System.Drawing.Size(167, 20);
		this.txt_H_City.TabIndex = 31;
		this.txt_H_Zipcode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_H_Zipcode.EditControl = null;
		this.txt_H_Zipcode.FieldName = "txt_H_Zipcode";
		this.txt_H_Zipcode.IsSearchControl = false;
		this.txt_H_Zipcode.Location = new System.Drawing.Point(591, 80);
		this.txt_H_Zipcode.Name = "txt_H_Zipcode";
		this.txt_H_Zipcode.SettingInVisible = false;
		this.txt_H_Zipcode.SettingReadonly = false;
		this.txt_H_Zipcode.Size = new System.Drawing.Size(167, 20);
		this.txt_H_Zipcode.TabIndex = 30;
		this.lbl_H_zipCode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_zipCode.EditControl = null;
		this.lbl_H_zipCode.EditValue = "Zip Code";
		this.lbl_H_zipCode.FieldName = null;
		this.lbl_H_zipCode.HideWhenEmpty = false;
		this.lbl_H_zipCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_zipCode.Location = new System.Drawing.Point(489, 83);
		this.lbl_H_zipCode.Name = "lbl_H_zipCode";
		this.lbl_H_zipCode.ReadOnly = false;
		this.lbl_H_zipCode.SettingInVisible = false;
		this.lbl_H_zipCode.SettingReadonly = false;
		this.lbl_H_zipCode.ShowUnderLine = true;
		this.lbl_H_zipCode.Size = new System.Drawing.Size(103, 20);
		this.lbl_H_zipCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_zipCode.TabIndex = 59;
		this.lbl_H_zipCode.Utext = "Zip Code";
		this.lbl_H_city.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_city.EditControl = null;
		this.lbl_H_city.EditValue = "City";
		this.lbl_H_city.FieldName = null;
		this.lbl_H_city.HideWhenEmpty = false;
		this.lbl_H_city.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_city.Location = new System.Drawing.Point(489, 106);
		this.lbl_H_city.Name = "lbl_H_city";
		this.lbl_H_city.ReadOnly = false;
		this.lbl_H_city.SettingInVisible = false;
		this.lbl_H_city.SettingReadonly = false;
		this.lbl_H_city.ShowUnderLine = true;
		this.lbl_H_city.Size = new System.Drawing.Size(103, 20);
		this.lbl_H_city.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_city.TabIndex = 58;
		this.lbl_H_city.Utext = "City";
		this.lbl_H_County.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_County.EditControl = null;
		this.lbl_H_County.EditValue = "County";
		this.lbl_H_County.FieldName = null;
		this.lbl_H_County.HideWhenEmpty = false;
		this.lbl_H_County.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_County.Location = new System.Drawing.Point(489, 129);
		this.lbl_H_County.Name = "lbl_H_County";
		this.lbl_H_County.ReadOnly = false;
		this.lbl_H_County.SettingInVisible = false;
		this.lbl_H_County.SettingReadonly = false;
		this.lbl_H_County.ShowUnderLine = true;
		this.lbl_H_County.Size = new System.Drawing.Size(103, 20);
		this.lbl_H_County.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_County.TabIndex = 57;
		this.lbl_H_County.Utext = "County";
		this.lbl_H_State.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_State.EditControl = null;
		this.lbl_H_State.EditValue = "State";
		this.lbl_H_State.FieldName = null;
		this.lbl_H_State.HideWhenEmpty = false;
		this.lbl_H_State.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_State.Location = new System.Drawing.Point(489, 152);
		this.lbl_H_State.Name = "lbl_H_State";
		this.lbl_H_State.ReadOnly = false;
		this.lbl_H_State.SettingInVisible = false;
		this.lbl_H_State.SettingReadonly = false;
		this.lbl_H_State.ShowUnderLine = true;
		this.lbl_H_State.Size = new System.Drawing.Size(103, 20);
		this.lbl_H_State.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_State.TabIndex = 56;
		this.lbl_H_State.Utext = "State";
		this.lbl_H_Country.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbl_H_Country.EditControl = null;
		this.lbl_H_Country.EditValue = "Country";
		this.lbl_H_Country.FieldName = null;
		this.lbl_H_Country.HideWhenEmpty = false;
		this.lbl_H_Country.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_H_Country.Location = new System.Drawing.Point(489, 176);
		this.lbl_H_Country.Name = "lbl_H_Country";
		this.lbl_H_Country.ReadOnly = false;
		this.lbl_H_Country.SettingInVisible = false;
		this.lbl_H_Country.SettingReadonly = false;
		this.lbl_H_Country.ShowUnderLine = true;
		this.lbl_H_Country.Size = new System.Drawing.Size(103, 20);
		this.lbl_H_Country.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_H_Country.TabIndex = 55;
		this.lbl_H_Country.Utext = "Country";
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
		this.lbl_HomeAddress.Location = new System.Drawing.Point(489, 3);
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
		this.txt_W_County.FieldName = "txt_W_County";
		this.txt_W_County.IsSearchControl = false;
		this.txt_W_County.Location = new System.Drawing.Point(114, 126);
		this.txt_W_County.Name = "txt_W_County";
		this.txt_W_County.SettingInVisible = false;
		this.txt_W_County.SettingReadonly = false;
		this.txt_W_County.Size = new System.Drawing.Size(167, 20);
		this.txt_W_County.TabIndex = 25;
		this.txt_W_City.EditControl = null;
		this.txt_W_City.FieldName = "txt_W_City";
		this.txt_W_City.IsSearchControl = false;
		this.txt_W_City.Location = new System.Drawing.Point(114, 103);
		this.txt_W_City.Name = "txt_W_City";
		this.txt_W_City.SettingInVisible = false;
		this.txt_W_City.SettingReadonly = false;
		this.txt_W_City.Size = new System.Drawing.Size(167, 20);
		this.txt_W_City.TabIndex = 24;
		this.txt_W_Zipcode.EditControl = null;
		this.txt_W_Zipcode.FieldName = "txt_W_Zipcode";
		this.txt_W_Zipcode.IsSearchControl = false;
		this.txt_W_Zipcode.Location = new System.Drawing.Point(114, 80);
		this.txt_W_Zipcode.Name = "txt_W_Zipcode";
		this.txt_W_Zipcode.SettingInVisible = false;
		this.txt_W_Zipcode.SettingReadonly = false;
		this.txt_W_Zipcode.Size = new System.Drawing.Size(167, 20);
		this.txt_W_Zipcode.TabIndex = 23;
		this.lbl_Country.EditControl = null;
		this.lbl_Country.EditValue = "Country";
		this.lbl_Country.FieldName = null;
		this.lbl_Country.HideWhenEmpty = false;
		this.lbl_Country.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Country.Location = new System.Drawing.Point(12, 176);
		this.lbl_Country.Name = "lbl_Country";
		this.lbl_Country.ReadOnly = false;
		this.lbl_Country.SettingInVisible = false;
		this.lbl_Country.SettingReadonly = false;
		this.lbl_Country.ShowUnderLine = true;
		this.lbl_Country.Size = new System.Drawing.Size(103, 20);
		this.lbl_Country.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Country.TabIndex = 43;
		this.lbl_Country.Utext = "Country";
		this.lbl_State.EditControl = null;
		this.lbl_State.EditValue = "State";
		this.lbl_State.FieldName = null;
		this.lbl_State.HideWhenEmpty = false;
		this.lbl_State.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_State.Location = new System.Drawing.Point(12, 151);
		this.lbl_State.Name = "lbl_State";
		this.lbl_State.ReadOnly = false;
		this.lbl_State.SettingInVisible = false;
		this.lbl_State.SettingReadonly = false;
		this.lbl_State.ShowUnderLine = true;
		this.lbl_State.Size = new System.Drawing.Size(103, 20);
		this.lbl_State.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_State.TabIndex = 44;
		this.lbl_State.Utext = "State";
		this.lbl_ZipCode.EditControl = null;
		this.lbl_ZipCode.EditValue = "Zip Code";
		this.lbl_ZipCode.FieldName = null;
		this.lbl_ZipCode.HideWhenEmpty = false;
		this.lbl_ZipCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ZipCode.Location = new System.Drawing.Point(12, 83);
		this.lbl_ZipCode.Name = "lbl_ZipCode";
		this.lbl_ZipCode.ReadOnly = false;
		this.lbl_ZipCode.SettingInVisible = false;
		this.lbl_ZipCode.SettingReadonly = false;
		this.lbl_ZipCode.ShowUnderLine = true;
		this.lbl_ZipCode.Size = new System.Drawing.Size(103, 20);
		this.lbl_ZipCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ZipCode.TabIndex = 47;
		this.lbl_ZipCode.Utext = "Zip Code";
		this.lbl_County.EditControl = null;
		this.lbl_County.EditValue = "County";
		this.lbl_County.FieldName = null;
		this.lbl_County.HideWhenEmpty = false;
		this.lbl_County.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_County.Location = new System.Drawing.Point(12, 128);
		this.lbl_County.Name = "lbl_County";
		this.lbl_County.ReadOnly = false;
		this.lbl_County.SettingInVisible = false;
		this.lbl_County.SettingReadonly = false;
		this.lbl_County.ShowUnderLine = true;
		this.lbl_County.Size = new System.Drawing.Size(103, 20);
		this.lbl_County.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_County.TabIndex = 45;
		this.lbl_County.Utext = "County";
		this.lbl_City.EditControl = null;
		this.lbl_City.EditValue = "City";
		this.lbl_City.FieldName = null;
		this.lbl_City.HideWhenEmpty = false;
		this.lbl_City.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_City.Location = new System.Drawing.Point(12, 105);
		this.lbl_City.Name = "lbl_City";
		this.lbl_City.ReadOnly = false;
		this.lbl_City.SettingInVisible = false;
		this.lbl_City.SettingReadonly = false;
		this.lbl_City.ShowUnderLine = true;
		this.lbl_City.Size = new System.Drawing.Size(103, 20);
		this.lbl_City.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_City.TabIndex = 46;
		this.lbl_City.Utext = "City";
		this.tab_Remarks.Controls.Add(this.txt_Remark);
		this.tab_Remarks.EditControl = null;
		this.tab_Remarks.EditValue = "Remarks";
		this.tab_Remarks.FieldName = null;
		this.tab_Remarks.IsSearchControl = false;
		this.tab_Remarks.Name = "tab_Remarks";
		this.tab_Remarks.ReadOnly = false;
		this.tab_Remarks.SettingInVisible = false;
		this.tab_Remarks.SettingReadonly = false;
		this.tab_Remarks.Size = new System.Drawing.Size(872, 278);
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
		this.txt_Remark.Size = new System.Drawing.Size(872, 278);
		this.txt_Remark.TabIndex = 0;
		this.tab_Attachments.Controls.Add(this.panelControl5);
		this.tab_Attachments.EditControl = null;
		this.tab_Attachments.EditValue = "Attachments";
		this.tab_Attachments.FieldName = null;
		this.tab_Attachments.IsSearchControl = false;
		this.tab_Attachments.Name = "tab_Attachments";
		this.tab_Attachments.ReadOnly = false;
		this.tab_Attachments.SettingInVisible = false;
		this.tab_Attachments.SettingReadonly = false;
		this.tab_Attachments.Size = new System.Drawing.Size(872, 278);
		this.tab_Attachments.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_Attachments.Text = "Attachments";
		this.panelControl5.Controls.Add(this.ucAttachment1);
		this.panelControl5.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl5.Location = new System.Drawing.Point(0, 0);
		this.panelControl5.Name = "panelControl5";
		this.panelControl5.Size = new System.Drawing.Size(872, 278);
		this.panelControl5.TabIndex = 1;
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oEmpRalatives";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(2, 2);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(868, 274);
		this.ucAttachment1.TabIndex = 0;
		this.tab_User.Controls.Add(this.grd_Udf);
		this.tab_User.EditControl = null;
		this.tab_User.EditValue = "User Defined";
		this.tab_User.FieldName = null;
		this.tab_User.IsSearchControl = false;
		this.tab_User.Name = "tab_User";
		this.tab_User.ReadOnly = false;
		this.tab_User.SettingInVisible = false;
		this.tab_User.SettingReadonly = false;
		this.tab_User.Size = new System.Drawing.Size(872, 278);
		this.tab_User.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tab_User.Text = "User Defined";
		this.grd_Udf.AllowPasteExcel = false;
		this.grd_Udf.ClipboardData = null;
		this.grd_Udf.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Udf.Location = new System.Drawing.Point(0, 0);
		this.grd_Udf.MainView = this.gv_Udf;
		this.grd_Udf.Name = "grd_Udf";
		this.grd_Udf.Size = new System.Drawing.Size(872, 278);
		this.grd_Udf.TabIndex = 0;
		this.grd_Udf.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Udf });
		this.gv_Udf.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Udf.ColumnKey = null;
		this.gv_Udf.DisableFormSetting = false;
		this.gv_Udf.DisplayException = false;
		this.gv_Udf.EditColumn = null;
		this.gv_Udf.FieldCurrency = null;
		this.gv_Udf.FieldRate = null;
		this.gv_Udf.FormId = "oEmpRalatives";
		this.gv_Udf.GridControl = this.grd_Udf;
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
		this.gv_Udf.Name = "gv_Udf";
		this.gv_Udf.OptionsView.ShowGroupPanel = false;
		this.gv_Udf.UseBatchMod = false;
		this.gv_Udf.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Udf_CellValueChanged);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewtoolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(131, 48);
		this.addNewtoolStripMenuItem.Name = "addNewtoolStripMenuItem";
		this.addNewtoolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.addNewtoolStripMenuItem.Text = "Add new";
		this.addNewtoolStripMenuItem.Click += new System.EventHandler(this.addNewtoolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 541);
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
		this.btnCancel.Location = new System.Drawing.Point(70, 541);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 57;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.grd_Content.AllowPasteExcel = true;
		this.grd_Content.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Content.ClipboardData = null;
		this.grd_Content.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Content.Cursor = System.Windows.Forms.Cursors.Default;
		this.grd_Content.Location = new System.Drawing.Point(3, 309);
		this.grd_Content.MainView = this.gv_Content;
		this.grd_Content.Name = "grd_Content";
		this.grd_Content.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.luk_grdGender, this.luk_grdRelation });
		this.grd_Content.Size = new System.Drawing.Size(874, 226);
		this.grd_Content.TabIndex = 60;
		this.grd_Content.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Content });
		this.grd_Content.Enter += new System.EventHandler(this.grd_Content_Enter);
		this.grd_Content.Leave += new System.EventHandler(this.grd_Content_Leave);
		this.gv_Content.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Content.ColumnKey = null;
		this.gv_Content.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn8 });
		this.gv_Content.DisableFormSetting = false;
		this.gv_Content.DisplayException = false;
		this.gv_Content.EditColumn = null;
		this.gv_Content.FieldCurrency = null;
		this.gv_Content.FieldRate = null;
		this.gv_Content.FormId = "oEmpRalatives";
		this.gv_Content.GridControl = this.grd_Content;
		this.gv_Content.IgnoreFilter = false;
		this.gv_Content.IndexEditRow = -1;
		this.gv_Content.IndicatorWidth = 40;
		this.gv_Content.IsApzTable = false;
		this.gv_Content.IsInsertNewRow = false;
		this.gv_Content.IsLoadingData = false;
		this.gv_Content.IsMappingContextAction = false;
		this.gv_Content.ItemId = "APZ_OHER";
		this.gv_Content.LstColumnAllowBatch = null;
		this.gv_Content.LstColumnNotAllowBatch = null;
		this.gv_Content.LstColumnNotEditable = null;
		this.gv_Content.LstColumNotSetting = null;
		this.gv_Content.Name = "gv_Content";
		this.gv_Content.OptionsCustomization.AllowGroup = false;
		this.gv_Content.OptionsFind.AllowFindPanel = false;
		this.gv_Content.OptionsNavigation.AutoFocusNewRow = true;
		this.gv_Content.OptionsView.ShowGroupPanel = false;
		this.gv_Content.UseBatchMod = false;
		this.gv_Content.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Content_CustomDrawRowIndicator);
		this.gv_Content.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Content_RowCellStyle);
		this.gv_Content.StartSorting += new System.EventHandler(this.gv_Content_StartSorting);
		this.gv_Content.EndSorting += new System.EventHandler(this.gv_Content_EndSorting);
		this.gv_Content.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Content_FocusedRowObjectChanged);
		this.gv_Content.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Content_BeforeLeaveRow);
		this.gv_Content.ColumnFilterChanged += new System.EventHandler(this.gv_Content_ColumnFilterChanged);
		this.gridColumn1.Caption = "gridColumn1";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "relaID";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "gridColumn2";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "empID";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn3.Caption = "First Name";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "firstName";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 0;
		this.gridColumn4.Caption = "Last Name";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "lastName";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 1;
		this.gridColumn5.Caption = "Gender";
		this.gridColumn5.ColumnEdit = this.luk_grdGender;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "sex";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 2;
		this.luk_grdGender.AutoHeight = false;
		this.luk_grdGender.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_grdGender.Name = "luk_grdGender";
		this.luk_grdGender.NullText = "";
		this.gridColumn6.Caption = "Relationship";
		this.gridColumn6.ColumnEdit = this.luk_grdRelation;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "relationship";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsColumn.ReadOnly = true;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 3;
		this.luk_grdRelation.AutoHeight = false;
		this.luk_grdRelation.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_grdRelation.Name = "luk_grdRelation";
		this.luk_grdRelation.NullText = "";
		this.luk_grdRelation.PopupView = this.repositoryItemGridLookUpEdit1View;
		this.repositoryItemGridLookUpEdit1View.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.repositoryItemGridLookUpEdit1View.ColumnKey = null;
		this.repositoryItemGridLookUpEdit1View.DisableFormSetting = false;
		this.repositoryItemGridLookUpEdit1View.DisplayException = false;
		this.repositoryItemGridLookUpEdit1View.EditColumn = null;
		this.repositoryItemGridLookUpEdit1View.FieldCurrency = null;
		this.repositoryItemGridLookUpEdit1View.FieldRate = null;
		this.repositoryItemGridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.repositoryItemGridLookUpEdit1View.FormId = "oEmpRalatives";
		this.repositoryItemGridLookUpEdit1View.IgnoreFilter = false;
		this.repositoryItemGridLookUpEdit1View.IndexEditRow = -1;
		this.repositoryItemGridLookUpEdit1View.IndicatorWidth = 35;
		this.repositoryItemGridLookUpEdit1View.IsApzTable = false;
		this.repositoryItemGridLookUpEdit1View.IsInsertNewRow = false;
		this.repositoryItemGridLookUpEdit1View.IsLoadingData = false;
		this.repositoryItemGridLookUpEdit1View.IsMappingContextAction = false;
		this.repositoryItemGridLookUpEdit1View.ItemId = "OVIEW";
		this.repositoryItemGridLookUpEdit1View.LstColumnAllowBatch = null;
		this.repositoryItemGridLookUpEdit1View.LstColumnNotAllowBatch = null;
		this.repositoryItemGridLookUpEdit1View.LstColumnNotEditable = null;
		this.repositoryItemGridLookUpEdit1View.LstColumNotSetting = null;
		this.repositoryItemGridLookUpEdit1View.Name = "repositoryItemGridLookUpEdit1View";
		this.repositoryItemGridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.repositoryItemGridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.repositoryItemGridLookUpEdit1View.UseBatchMod = false;
		this.gridColumn8.Caption = "Mobile";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "mobile";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.OptionsColumn.ReadOnly = true;
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 4;
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(882, 569);
		base.Controls.Add(this.grd_Content);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.tab_Detail);
		base.FormId = "oEmpRalatives";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_EmpRelatives";
		base.ShowInTaskbar = false;
		this.Text = "'s Relatives";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_EmpRelatives_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.tab_Detail).EndInit();
		this.tab_Detail.ResumeLayout(false);
		this.tab_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.date_Brith.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_Brith.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_PassExp.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_PassExp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_FamDeduction.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_YearFormat.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PersGroup.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Relationship.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Citizenship.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PassNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Gender.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_IDNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CountryOfBirth.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pic_Images.Properties).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_HomePhone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Fax.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Email.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ExtPhone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MobilePhone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_OfficePhone.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_RelaNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_JobTitle.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_LastName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MiddleName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FirstName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		this.tab_Address.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.pnl_Address).EndInit();
		this.pnl_Address.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.luk_H_State.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_H_Country.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_W_Country.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_W_State.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Block.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Street.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Street.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Block.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_County.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_City.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_H_Zipcode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_County.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_City.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_W_Zipcode.Properties).EndInit();
		this.tab_Remarks.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Remark.Properties).EndInit();
		this.tab_Attachments.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl5).EndInit();
		this.panelControl5.ResumeLayout(false);
		this.tab_User.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Udf).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Udf).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Content).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Content).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_grdGender).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_grdRelation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1View).EndInit();
		base.ResumeLayout(false);
	}
}
