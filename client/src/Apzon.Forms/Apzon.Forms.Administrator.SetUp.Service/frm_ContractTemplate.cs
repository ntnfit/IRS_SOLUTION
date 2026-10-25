using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator.SetUp.Service;

public class frm_ContractTemplate : XtraFormEx
{
	private string _tmplName = "";

	private string _oldTmplName = "";

	private string _strNoRecordFound = "No matching record found";

	private Dictionary<string, Control> _dicControls = new Dictionary<string, Control>();

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	private bool _isChangedValueWhenAdd;

	private int _atcEntry;

	private IContainer components;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private TextEditEx txt_TmpltName;

	private LabelLine lb_CatalogNo;

	private LookUpEditCombo luk_CntrctType;

	private LabelLine lb_ItemType;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private TextEditEx txt_Descriptio;

	private LabelLine labelLine3;

	private LookUpEditCombo luk_ResponseU;

	private LookUpEditCombo luk_ResponsUnt;

	private CheckEditEx chk_Deleted;

	private LabelLine labelLine4;

	private CheckEditEx chk_Renewal;

	private LabelLine labelLine5;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage1;

	private XtraTabPageEx xtraTabPage2;

	private XtraTabPageEx xtraTabPage3;

	private LabelLine labelLine7;

	private LabelLine labelLine6;

	private HourEdit hrEdit_MonEnd;

	private HourEdit hrEdit_MonStart;

	private CheckEditEx chk_SunEnabled;

	private CheckEditEx chk_SatEnabled;

	private CheckEditEx chk_FriEnabled;

	private CheckEditEx chk_ThuEnabled;

	private CheckEditEx chk_WedEnabled;

	private CheckEditEx chk_TueEnabled;

	private CheckEditEx chk_MonEnabled;

	private HourEdit hrEdit_SunEnd;

	private HourEdit hrEdit_SunStrart;

	private HourEdit hrEdit_SatEnd;

	private HourEdit hrEdit_SatStart;

	private HourEdit hrEdit_FriEnd;

	private HourEdit hrEdit_FriStart;

	private HourEdit hrEdit_ThuEnd;

	private HourEdit hrEdit_ThuStart;

	private HourEdit hrEdit_WedEnd;

	private HourEdit hrEdit_WedStart;

	private HourEdit hrEdit_TueEnd;

	private HourEdit hrEdit_TueStart;

	private LabelLine labelLine8;

	private CheckEditEx chk_InclHldays;

	private Panel panel1;

	private CheckEditEx chk_InclTravel;

	private CheckEditEx chk_InclWork;

	private CheckEditEx chk_InclParts;

	private LookUpEditCombo luk_RemindUnit;

	private MemoEditEx memo_Remark;

	private ucAttachment ucAttachment1;

	private TextEditExPrice calc_Duration;

	private TextEditEx txt_DurationUnit;

	private TextEditExPrice calc_RemindVal;

	private TextEditEx calc_ResponseV;

	private TextEditEx calc_ResponsVal;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			btn_OK.Enabled = false;
			btn_Cancel.Enabled = true;
			return;
		}
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btn_OK.Text = Resources.ButtonFind;
			break;
		}
	}

	public async Task MODE_FIND()
	{
		bool flag = true;
		if ((FormMode == Enums.FormViewModeType.Update || (FormMode == Enums.FormViewModeType.Update && _isChangedValueWhenAdd)) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			flag = false;
		}
		if (flag)
		{
			ResetControls("F");
			FormMode = Enums.FormViewModeType.Find;
			_isChangedValueWhenAdd = false;
			txt_TmpltName.Focus();
		}
	}

	public async Task REFRESH()
	{
		bool flag = true;
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
		{
			flag = false;
		}
		if (!flag)
		{
			return;
		}
		FormMode = Enums.FormViewModeType.Add;
		DataTable tableStructure = WinFuntions.GetTableStructure("OCTT");
		if (tableStructure != null)
		{
			tableStructure.Rows.Clear();
			tableStructure.AcceptChanges();
			DataTable dataTableNewRow = WinFuntions.GetDataTableNewRow(tableStructure.Copy());
			if (dataTableNewRow != null && dataTableNewRow.Columns.Count > 0)
			{
				dataTableNewRow.AcceptChanges();
				DataSource = dataTableNewRow.Copy();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CannotGetDatasourceTableStructWithTableName, "", "OITM");
			}
		}
		else
		{
			WinCommons.Error(Enums.MessageCode.CannotGetDatasourceTableStructWithTableName, "", "OITM");
		}
		ResetControls("A");
		_isChangedValueWhenAdd = false;
		txt_TmpltName.Focus();
	}

	public async Task MOVE_FIRST()
	{
		if (!_isLoading)
		{
			_isLoading = true;
			try
			{
				GetRecord("F", "");
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
			_isLoading = false;
		}
	}

	public async Task MOVE_NEXT()
	{
		if (_isLoading)
		{
			return;
		}
		_isLoading = true;
		try
		{
			if (!string.IsNullOrWhiteSpace(txt_TmpltName.Text) && CheckDoc())
			{
				GetRecord("N", txt_TmpltName.Text);
			}
			else
			{
				GetRecord("F", "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	public async Task MOVE_LAST()
	{
		if (!_isLoading)
		{
			_isLoading = true;
			try
			{
				GetRecord("L", "");
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
			_isLoading = false;
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		if (_isLoading)
		{
			return;
		}
		_isLoading = true;
		try
		{
			if (!string.IsNullOrWhiteSpace(txt_TmpltName.Text) && CheckDoc())
			{
				GetRecord("P", txt_TmpltName.Text);
			}
			else
			{
				GetRecord("L", "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			LoadData(txt_TmpltName.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_ContractTemplate(string tmplName)
	{
		InitializeComponent();
		OnLoadAsync += frm_ContractTemplate_Load;
		_tmplName = tmplName;
		ucAttachment1.ButtonBrowse_Click += Controls_EditValueChanged;
		ucAttachment1.ButtonDelete_Click += Controls_EditValueChanged;
	}

	public frm_ContractTemplate()
	{
		InitializeComponent();
		OnLoadAsync += frm_ContractTemplate_Load;
	}

	private async Task frm_ContractTemplate_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			ucAttachment1.gv_File.OptionsFind.AllowFindPanel = false;
			SetDictionary();
			LoadComboBox();
			if (_tmplName != "")
			{
				LoadData(_tmplName);
			}
			else
			{
				REFRESH();
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			btn_OK.Enabled = false;
			if (FormMode == Enums.FormViewModeType.Find)
			{
				Search();
			}
			else if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				SaveData();
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_OK.Enabled = true;
	}

	private void Controls_EditValueChanged(object sender, EventArgs e)
	{
		ChangeButtonOKText(sender);
	}

	private void chk_MonEnabled_CheckedChanged(object sender, EventArgs e)
	{
		hrEdit_MonStart.Enabled = chk_MonEnabled.Checked;
		hrEdit_MonEnd.Enabled = chk_MonEnabled.Checked;
	}

	private void chk_TueEnabled_CheckedChanged(object sender, EventArgs e)
	{
		hrEdit_TueStart.Enabled = chk_TueEnabled.Checked;
		hrEdit_TueEnd.Enabled = chk_TueEnabled.Checked;
	}

	private void chk_WedEnabled_CheckedChanged(object sender, EventArgs e)
	{
		hrEdit_WedStart.Enabled = chk_WedEnabled.Checked;
		hrEdit_WedEnd.Enabled = chk_WedEnabled.Checked;
	}

	private void chk_ThuEnabled_CheckedChanged(object sender, EventArgs e)
	{
		hrEdit_ThuStart.Enabled = chk_ThuEnabled.Checked;
		hrEdit_ThuEnd.Enabled = chk_ThuEnabled.Checked;
	}

	private void chk_FriEnabled_CheckedChanged(object sender, EventArgs e)
	{
		hrEdit_FriStart.Enabled = chk_FriEnabled.Checked;
		hrEdit_FriEnd.Enabled = chk_FriEnabled.Checked;
	}

	private void chk_SatEnabled_CheckedChanged(object sender, EventArgs e)
	{
		hrEdit_SatStart.Enabled = chk_SatEnabled.Checked;
		hrEdit_SatEnd.Enabled = chk_SatEnabled.Checked;
	}

	private void chk_SunEnabled_CheckedChanged(object sender, EventArgs e)
	{
		hrEdit_SunStrart.Enabled = chk_SunEnabled.Checked;
		hrEdit_SunEnd.Enabled = chk_SunEnabled.Checked;
	}

	private void chk_Deleted_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Find)
			{
				return;
			}
			txt_TmpltName.Enabled = !chk_Deleted.Checked;
			luk_CntrctType.Enabled = !chk_Deleted.Checked;
			calc_ResponseV.Enabled = !chk_Deleted.Checked;
			luk_ResponseU.Enabled = !chk_Deleted.Checked;
			calc_ResponsVal.Enabled = !chk_Deleted.Checked;
			luk_ResponsUnt.Enabled = !chk_Deleted.Checked;
			txt_Descriptio.Enabled = !chk_Deleted.Checked;
			chk_Renewal.Enabled = !chk_Deleted.Checked;
			if (chk_Deleted.Checked)
			{
				calc_RemindVal.Enabled = false;
				luk_RemindUnit.Enabled = false;
				hrEdit_MonStart.Enabled = false;
				hrEdit_MonEnd.Enabled = false;
				hrEdit_TueStart.Enabled = false;
				hrEdit_TueEnd.Enabled = false;
				hrEdit_WedStart.Enabled = false;
				hrEdit_WedEnd.Enabled = false;
				hrEdit_ThuStart.Enabled = false;
				hrEdit_ThuEnd.Enabled = false;
				hrEdit_FriStart.Enabled = false;
				hrEdit_FriEnd.Enabled = false;
				hrEdit_SatStart.Enabled = false;
				hrEdit_SatEnd.Enabled = false;
				hrEdit_SunStrart.Enabled = false;
				hrEdit_SunEnd.Enabled = false;
			}
			else
			{
				if (chk_Renewal.Checked)
				{
					calc_RemindVal.Enabled = true;
					luk_RemindUnit.Enabled = true;
				}
				else
				{
					calc_RemindVal.Enabled = false;
					luk_RemindUnit.Enabled = false;
				}
				if (chk_MonEnabled.Checked)
				{
					hrEdit_MonStart.Enabled = true;
					hrEdit_MonEnd.Enabled = true;
				}
				else
				{
					hrEdit_MonStart.Enabled = false;
					hrEdit_MonEnd.Enabled = false;
				}
				if (chk_TueEnabled.Checked)
				{
					hrEdit_TueStart.Enabled = true;
					hrEdit_TueEnd.Enabled = true;
				}
				else
				{
					hrEdit_TueStart.Enabled = false;
					hrEdit_TueEnd.Enabled = false;
				}
				if (chk_WedEnabled.Checked)
				{
					hrEdit_WedStart.Enabled = true;
					hrEdit_WedEnd.Enabled = true;
				}
				else
				{
					hrEdit_WedStart.Enabled = false;
					hrEdit_WedEnd.Enabled = false;
				}
				if (chk_ThuEnabled.Checked)
				{
					hrEdit_ThuStart.Enabled = true;
					hrEdit_ThuEnd.Enabled = true;
				}
				else
				{
					hrEdit_ThuStart.Enabled = false;
					hrEdit_ThuEnd.Enabled = false;
				}
				if (chk_FriEnabled.Checked)
				{
					hrEdit_FriStart.Enabled = true;
					hrEdit_FriEnd.Enabled = true;
				}
				else
				{
					hrEdit_FriStart.Enabled = false;
					hrEdit_FriEnd.Enabled = false;
				}
				if (chk_SatEnabled.Checked)
				{
					hrEdit_SatStart.Enabled = true;
					hrEdit_SatEnd.Enabled = true;
				}
				else
				{
					hrEdit_SatStart.Enabled = false;
					hrEdit_SatEnd.Enabled = false;
				}
				if (chk_SunEnabled.Checked)
				{
					hrEdit_SunStrart.Enabled = true;
					hrEdit_SunEnd.Enabled = true;
				}
				else
				{
					hrEdit_SunStrart.Enabled = false;
					hrEdit_SunEnd.Enabled = false;
				}
			}
			chk_MonEnabled.Enabled = !chk_Deleted.Checked;
			chk_TueEnabled.Enabled = !chk_Deleted.Checked;
			chk_WedEnabled.Enabled = !chk_Deleted.Checked;
			chk_ThuEnabled.Enabled = !chk_Deleted.Checked;
			chk_FriEnabled.Enabled = !chk_Deleted.Checked;
			chk_SatEnabled.Enabled = !chk_Deleted.Checked;
			chk_SunEnabled.Enabled = !chk_Deleted.Checked;
			chk_InclParts.Enabled = !chk_Deleted.Checked;
			chk_InclWork.Enabled = !chk_Deleted.Checked;
			chk_InclTravel.Enabled = !chk_Deleted.Checked;
			chk_InclHldays.Enabled = !chk_Deleted.Checked;
			memo_Remark.Enabled = !chk_Deleted.Checked;
			ucAttachment1.Enabled = !chk_Deleted.Checked;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void chk_Renewal_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Find)
			{
				calc_RemindVal.Enabled = chk_Renewal.Checked;
				luk_RemindUnit.Enabled = chk_Renewal.Checked;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadData(string tmpltName)
	{
		try
		{
			DataTable data = _httpClient.ContractTemplateClient.GetData(tmpltName);
			if (data != null && data.Rows.Count > 0)
			{
				ShowData(data);
				return;
			}
			ResetControls("A");
			WinCommons.Error(Enums.MessageCode.None, _strNoRecordFound);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ShowData(DataTable dt)
	{
		try
		{
			ResetControls("U");
			_oldTmplName = Function.ToString(dt.Rows[0]["TmpltName"]);
			FormMode = Enums.FormViewModeType.View;
			_isChangedValueWhenAdd = false;
			DataSource = dt;
			_atcEntry = Function.ParseInt(dt.Rows[0]["AtcEntry"]);
			ucAttachment1.LoadAttachments(_atcEntry);
			chk_Deleted_CheckedChanged(null, null);
			chk_Renewal_CheckedChanged(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboBox()
	{
		try
		{
			luk_CntrctType.SetDataSource(new string[3, 2]
			{
				{ "C", "Customer" },
				{ "G", "Item Group" },
				{ "S", "Serial Number" }
			}, isStringCode: true, "C");
			luk_ResponseU.SetDataSource(new string[2, 2]
			{
				{ "H", "Hour(s)" },
				{ "D", "Day(s)" }
			}, isStringCode: true, "H");
			luk_ResponsUnt.SetDataSource(new string[2, 2]
			{
				{ "D", "Day(s)" },
				{ "H", "Hour(s)" }
			}, isStringCode: true, "D");
			luk_RemindUnit.SetDataSource(new string[3, 2]
			{
				{ "D", "Day(s)" },
				{ "W", "Week(s)" },
				{ "M", "Month(s)" }
			}, isStringCode: true, "D");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetRecord(string position, string tmpltName)
	{
		try
		{
			_isLoading = true;
			bool flag = true;
			if ((FormMode == Enums.FormViewModeType.Update || (FormMode == Enums.FormViewModeType.Update && _isChangedValueWhenAdd)) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				DataTable record = _httpClient.ContractTemplateClient.GetRecord(position, tmpltName);
				if (record != null && record.Rows.Count > 0)
				{
					txt_TmpltName.Text = Function.ToString(record.Rows[0]["TmpltName"]);
					ShowData(record);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, _strNoRecordFound);
				}
			}
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ResetControls(string mode)
	{
		try
		{
			if (mode == "A")
			{
				txt_TmpltName.Text = "";
				luk_RemindUnit.EditValue = "D";
				luk_ResponseU.EditValue = "H";
				luk_ResponsUnt.EditValue = "D";
				calc_ResponsVal.EditValue = 0;
				calc_ResponseV.EditValue = 0;
				calc_Duration.EditValue = 0;
				calc_RemindVal.EditValue = 0;
				txt_Descriptio.EditValue = "";
				hrEdit_MonStart.EditValue = 800;
				hrEdit_MonEnd.EditValue = 1700;
				hrEdit_TueStart.EditValue = 800;
				hrEdit_TueEnd.EditValue = 1700;
				hrEdit_WedStart.EditValue = 800;
				hrEdit_WedEnd.EditValue = 1700;
				hrEdit_ThuStart.EditValue = 800;
				hrEdit_ThuEnd.EditValue = 1700;
				hrEdit_FriStart.EditValue = 800;
				hrEdit_FriEnd.EditValue = 1700;
				hrEdit_SatStart.EditValue = 0;
				hrEdit_SatEnd.EditValue = 2359;
				hrEdit_SunStrart.EditValue = 0;
				hrEdit_SunEnd.EditValue = 2359;
				memo_Remark.EditValue = "";
				chk_InclParts.EditValue = "N";
				chk_InclWork.EditValue = "N";
				chk_InclTravel.EditValue = "N";
				chk_InclHldays.EditValue = "N";
				chk_MonEnabled.EditValue = "Y";
				chk_TueEnabled.EditValue = "Y";
				chk_WedEnabled.EditValue = "Y";
				chk_ThuEnabled.EditValue = "Y";
				chk_FriEnabled.EditValue = "Y";
				chk_SatEnabled.EditValue = "Y";
				chk_SunEnabled.EditValue = "Y";
				chk_Renewal.EditValue = "N";
				chk_Deleted.EditValue = "N";
				calc_RemindVal.Enabled = false;
				luk_RemindUnit.Enabled = false;
				hrEdit_MonStart.Enabled = true;
				hrEdit_MonEnd.Enabled = true;
				hrEdit_TueStart.Enabled = true;
				hrEdit_TueEnd.Enabled = true;
				hrEdit_WedStart.Enabled = true;
				hrEdit_WedEnd.Enabled = true;
				hrEdit_ThuStart.Enabled = true;
				hrEdit_ThuEnd.Enabled = true;
				hrEdit_FriStart.Enabled = true;
				hrEdit_FriEnd.Enabled = true;
				hrEdit_SatStart.Enabled = true;
				hrEdit_SatEnd.Enabled = true;
				hrEdit_SunStrart.Enabled = true;
				hrEdit_SunEnd.Enabled = true;
				chk_InclParts.Enabled = true;
				chk_InclWork.Enabled = true;
				chk_InclTravel.Enabled = true;
				chk_InclHldays.Enabled = true;
				chk_MonEnabled.Enabled = true;
				chk_TueEnabled.Enabled = true;
				chk_WedEnabled.Enabled = true;
				chk_ThuEnabled.Enabled = true;
				chk_FriEnabled.Enabled = true;
				chk_SatEnabled.Enabled = true;
				chk_SunEnabled.Enabled = true;
				ucAttachment1.Enabled = true;
				txt_TmpltName.BackColor = Color.White;
				luk_CntrctType.BackColor = Color.White;
				luk_RemindUnit.BackColor = Color.White;
				luk_ResponseU.BackColor = Color.White;
				luk_ResponsUnt.BackColor = Color.White;
				calc_ResponsVal.BackColor = Color.White;
				calc_ResponseV.BackColor = Color.White;
				calc_Duration.BackColor = Color.White;
				calc_RemindVal.BackColor = Color.White;
				chk_Renewal.BackColor = Color.White;
				chk_Deleted.BackColor = Color.White;
				txt_Descriptio.BackColor = Color.White;
				memo_Remark.BackColor = Color.White;
			}
			else if (mode == "F")
			{
				txt_TmpltName.Text = "";
				luk_RemindUnit.EditValue = "D";
				luk_ResponseU.EditValue = "H";
				luk_ResponsUnt.EditValue = "D";
				calc_ResponsVal.EditValue = 0;
				calc_ResponseV.EditValue = 0;
				calc_Duration.EditValue = 0;
				calc_RemindVal.EditValue = 0;
				txt_Descriptio.EditValue = "";
				hrEdit_MonStart.EditValue = 800;
				hrEdit_MonEnd.EditValue = 1700;
				hrEdit_TueStart.EditValue = 800;
				hrEdit_TueEnd.EditValue = 1700;
				hrEdit_WedStart.EditValue = 800;
				hrEdit_WedEnd.EditValue = 1700;
				hrEdit_ThuStart.EditValue = 800;
				hrEdit_ThuEnd.EditValue = 1700;
				hrEdit_FriStart.EditValue = 800;
				hrEdit_FriEnd.EditValue = 1700;
				hrEdit_SatStart.EditValue = 0;
				hrEdit_SatEnd.EditValue = 2359;
				hrEdit_SunStrart.EditValue = 0;
				hrEdit_SunEnd.EditValue = 2359;
				memo_Remark.EditValue = "";
				chk_InclParts.EditValue = "N";
				chk_InclWork.EditValue = "N";
				chk_InclTravel.EditValue = "N";
				chk_InclHldays.EditValue = "N";
				chk_MonEnabled.EditValue = "N";
				chk_TueEnabled.EditValue = "N";
				chk_WedEnabled.EditValue = "N";
				chk_ThuEnabled.EditValue = "N";
				chk_FriEnabled.EditValue = "N";
				chk_SatEnabled.EditValue = "N";
				chk_SunEnabled.EditValue = "N";
				chk_Renewal.EditValue = "N";
				chk_Deleted.EditValue = "N";
				calc_RemindVal.Enabled = true;
				luk_RemindUnit.Enabled = true;
				hrEdit_MonStart.Enabled = false;
				hrEdit_MonEnd.Enabled = false;
				hrEdit_TueStart.Enabled = false;
				hrEdit_TueEnd.Enabled = false;
				hrEdit_WedStart.Enabled = false;
				hrEdit_WedEnd.Enabled = false;
				hrEdit_ThuStart.Enabled = false;
				hrEdit_ThuEnd.Enabled = false;
				hrEdit_FriStart.Enabled = false;
				hrEdit_FriEnd.Enabled = false;
				hrEdit_SatStart.Enabled = false;
				hrEdit_SatEnd.Enabled = false;
				hrEdit_SunStrart.Enabled = false;
				hrEdit_SunEnd.Enabled = false;
				chk_InclParts.Enabled = false;
				chk_InclWork.Enabled = false;
				chk_InclTravel.Enabled = false;
				chk_InclHldays.Enabled = false;
				chk_MonEnabled.Enabled = false;
				chk_TueEnabled.Enabled = false;
				chk_WedEnabled.Enabled = false;
				chk_ThuEnabled.Enabled = false;
				chk_FriEnabled.Enabled = false;
				chk_SatEnabled.Enabled = false;
				chk_SunEnabled.Enabled = false;
				ucAttachment1.Enabled = false;
				txt_TmpltName.BackColor = Color.FromArgb(250, 249, 215);
				luk_CntrctType.BackColor = Color.FromArgb(250, 249, 215);
				luk_RemindUnit.BackColor = Color.FromArgb(250, 249, 215);
				luk_ResponseU.BackColor = Color.FromArgb(250, 249, 215);
				luk_ResponsUnt.BackColor = Color.FromArgb(250, 249, 215);
				calc_ResponsVal.BackColor = Color.FromArgb(250, 249, 215);
				calc_ResponseV.BackColor = Color.FromArgb(250, 249, 215);
				calc_Duration.BackColor = Color.FromArgb(250, 249, 215);
				calc_RemindVal.BackColor = Color.FromArgb(250, 249, 215);
				chk_Renewal.BackColor = Color.FromArgb(250, 249, 215);
				chk_Deleted.BackColor = Color.FromArgb(250, 249, 215);
				txt_Descriptio.BackColor = Color.FromArgb(250, 249, 215);
				memo_Remark.BackColor = Color.FromArgb(250, 249, 215);
			}
			else
			{
				txt_TmpltName.BackColor = Color.White;
				luk_CntrctType.BackColor = Color.White;
				luk_RemindUnit.BackColor = Color.White;
				luk_ResponseU.BackColor = Color.White;
				luk_ResponsUnt.BackColor = Color.White;
				calc_ResponsVal.BackColor = Color.White;
				calc_ResponseV.BackColor = Color.White;
				calc_Duration.BackColor = Color.White;
				calc_RemindVal.BackColor = Color.White;
				chk_Renewal.BackColor = Color.White;
				chk_Deleted.BackColor = Color.White;
				txt_Descriptio.BackColor = Color.White;
				memo_Remark.BackColor = Color.White;
			}
			_atcEntry = 0;
			luk_CntrctType.EditValue = "S";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SaveData()
	{
		try
		{
			DataTable controlsValueToDataTable = WinFuntions.GetControlsValueToDataTable("OCTT", _dicControls);
			if (controlsValueToDataTable != null && controlsValueToDataTable.Rows.Count > 0)
			{
				if (string.IsNullOrWhiteSpace(txt_TmpltName.Text))
				{
					WinCommons.Error(Enums.MessageCode.TemplateNameIsMissingContractTemplate, "");
					return;
				}
				controlsValueToDataTable.Rows[0]["AtcEntry"] = _atcEntry;
				if (!controlsValueToDataTable.Columns.Contains("SaveAction"))
				{
					controlsValueToDataTable.Columns.Add(new DataColumn("SaveAction"));
					controlsValueToDataTable.AcceptChanges();
				}
				if (FormMode == Enums.FormViewModeType.Add)
				{
					controlsValueToDataTable.Rows[0]["SaveAction"] = "A";
				}
				else
				{
					controlsValueToDataTable.Rows[0]["SaveAction"] = "U";
				}
				if (FormMode == Enums.FormViewModeType.Update && string.IsNullOrWhiteSpace(_oldTmplName))
				{
					controlsValueToDataTable.Rows[0]["SaveAction"] = "A";
				}
				if (!controlsValueToDataTable.Columns.Contains("OldName"))
				{
					controlsValueToDataTable.Columns.Add(new DataColumn("OldName"));
					controlsValueToDataTable.AcceptChanges();
				}
				controlsValueToDataTable.Rows[0]["OldName"] = _oldTmplName;
				controlsValueToDataTable.TableName = "OCTT";
				controlsValueToDataTable.AcceptChanges();
				DataSet dataSet = controlsValueToDataTable.DataSet;
				if (dataSet != null)
				{
					dataSet.Tables.Remove(controlsValueToDataTable.TableName);
					dataSet.AcceptChanges();
				}
				DataTable dataTable = ucAttachment1.UploadFileNoSAP();
				if (dataTable.IsNotNull() || !ucAttachment1.DataSource.IsNotNull())
				{
					dataTable.TableName = "ATC1";
					dataTable.AcceptChanges();
					DataSet dataSet2 = controlsValueToDataTable.DataSet;
					if (dataSet2 != null)
					{
						dataSet2.Tables.Remove(controlsValueToDataTable.TableName);
						dataSet2.AcceptChanges();
					}
					DataSet dataSet3 = new DataSet();
					dataSet3.Tables.Clear();
					dataSet3.Tables.AddRange(new DataTable[2] { controlsValueToDataTable, dataTable });
					dataSet3.AcceptChanges();
					string text = _httpClient.ContractTemplateClient.SaveDoc(dataSet3);
					if (string.IsNullOrEmpty(text))
					{
						WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
						FormMode = Enums.FormViewModeType.View;
						_oldTmplName = Function.ToString(txt_TmpltName.Text);
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Search()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("TmpltName", typeof(string));
			dataTable.Columns.Add("CntrctType", typeof(string));
			dataTable.Columns.Add("ResponseV", typeof(int));
			dataTable.Columns.Add("ResponseU", typeof(string));
			dataTable.Columns.Add("Descriptio", typeof(string));
			dataTable.Columns.Add("Deleted", typeof(string));
			dataTable.Columns.Add("Renewal", typeof(string));
			dataTable.Columns.Add("Duration", typeof(int));
			dataTable.Columns.Add("RemindVal", typeof(int));
			dataTable.Columns.Add("RemindUnit", typeof(string));
			dataTable.Columns.Add("Remark", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["TmpltName"] = Function.ToString(txt_TmpltName.Text);
			dataRow["CntrctType"] = Function.ToString(luk_CntrctType.EditValue);
			dataRow["ResponseV"] = Function.ParseInt(calc_ResponseV.EditValue);
			dataRow["ResponseU"] = Function.ToString(luk_ResponseU.EditValue);
			dataRow["Descriptio"] = Function.ToString(txt_Descriptio.Text);
			dataRow["Deleted"] = Function.ToString(chk_Deleted.EditValue);
			dataRow["Renewal"] = Function.ToString(chk_Renewal.EditValue);
			dataRow["Duration"] = Function.ParseInt(calc_Duration.EditValue);
			dataRow["RemindVal"] = Function.ToString(calc_RemindVal.EditValue);
			dataRow["Remark"] = Function.ToString(memo_Remark.Text);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			DataSet dataSet = _httpClient.ContractTemplateClient.Search(dataTable);
			if (dataSet != null && dataSet.Tables.Count == 2 && dataSet.Tables[1] != null && dataSet.Tables[1].Rows.Count > 0)
			{
				if (dataSet.Tables[1].Rows.Count == 1)
				{
					ShowData(dataSet.Tables[1]);
					return;
				}
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListContractTemplate, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.DataSource = dataSet.Copy();
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					DataTable dataTable2 = dataSet.Tables[1].Clone();
					dataTable2.ImportRow(frm_CFL2.ChooseItem);
					if (dataTable2.Columns.Contains("ContractType"))
					{
						dataTable2.Columns.Remove("ContractType");
						dataTable2.AcceptChanges();
					}
					ShowData(dataTable2);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoMatchingRecordFound, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeButtonOKText(object sender)
	{
		try
		{
			if (!_isLoading)
			{
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
				else if (FormMode == Enums.FormViewModeType.Add)
				{
					_isChangedValueWhenAdd = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool CheckDoc()
	{
		try
		{
			DataTable dataTable = _httpClient.ContractTemplateClient.CheckDoc(txt_TmpltName.Text);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return true;
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private void SetDictionary()
	{
		try
		{
			_dicControls.Add("OCTT.TmpltName", txt_TmpltName);
			_dicControls.Add("OCTT.CntrctType", luk_CntrctType);
			_dicControls.Add("OCTT.ResponseV", calc_ResponseV);
			_dicControls.Add("OCTT.ResponseU", luk_ResponseU);
			_dicControls.Add("OCTT.ResponsVal", calc_ResponsVal);
			_dicControls.Add("OCTT.ResponsUnt", luk_ResponsUnt);
			_dicControls.Add("OCTT.Descriptio", txt_Descriptio);
			_dicControls.Add("OCTT.Duration", calc_Duration);
			_dicControls.Add("OCTT.DurationUnit", txt_DurationUnit);
			_dicControls.Add("OCTT.RemindVal", calc_RemindVal);
			_dicControls.Add("OCTT.RemindUnit", luk_RemindUnit);
			_dicControls.Add("OCTT.MonStart", hrEdit_MonStart);
			_dicControls.Add("OCTT.MonEnd", hrEdit_MonEnd);
			_dicControls.Add("OCTT.TueStart", hrEdit_TueStart);
			_dicControls.Add("OCTT.TueEnd", hrEdit_TueEnd);
			_dicControls.Add("OCTT.WedStart", hrEdit_WedStart);
			_dicControls.Add("OCTT.WedEnd", hrEdit_WedEnd);
			_dicControls.Add("OCTT.ThuStart", hrEdit_ThuStart);
			_dicControls.Add("OCTT.ThuEnd", hrEdit_ThuEnd);
			_dicControls.Add("OCTT.FriStart", hrEdit_FriStart);
			_dicControls.Add("OCTT.FriEnd", hrEdit_FriEnd);
			_dicControls.Add("OCTT.SatStart", hrEdit_SatStart);
			_dicControls.Add("OCTT.SatEnd", hrEdit_SatEnd);
			_dicControls.Add("OCTT.SunStrart", hrEdit_SunStrart);
			_dicControls.Add("OCTT.SunEnd", hrEdit_SunEnd);
			_dicControls.Add("OCTT.Remark", memo_Remark);
			_dicControls.Add("OCTT.InclParts", chk_InclParts);
			_dicControls.Add("OCTT.InclWork", chk_InclWork);
			_dicControls.Add("OCTT.InclTravel", chk_InclTravel);
			_dicControls.Add("OCTT.InclHldays", chk_InclHldays);
			_dicControls.Add("OCTT.MonEnabled", chk_MonEnabled);
			_dicControls.Add("OCTT.TueEnabled", chk_TueEnabled);
			_dicControls.Add("OCTT.WedEnabled", chk_WedEnabled);
			_dicControls.Add("OCTT.ThuEnabled", chk_ThuEnabled);
			_dicControls.Add("OCTT.FriEnabled", chk_FriEnabled);
			_dicControls.Add("OCTT.SatEnabled", chk_SatEnabled);
			_dicControls.Add("OCTT.SunEnabled", chk_SunEnabled);
			_dicControls.Add("OCTT.Renewal", chk_Renewal);
			_dicControls.Add("OCTT.Deleted", chk_Deleted);
			foreach (KeyValuePair<string, Control> dicControl in _dicControls)
			{
				dicControl.Value.Tag = dicControl.Key.Replace(".", ",");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		if ((msg.Msg == 256 || msg.Msg == 260) && !_isLoading && keyData == Keys.Return)
		{
			btn_OK_Click(null, null);
		}
		return base.ProcessCmdKey(ref msg, keyData);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.Service.frm_ContractTemplate));
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_TmpltName = new Apzon.Controls.Editor.TextEditEx();
		this.lb_CatalogNo = new Apzon.Controls.Controls.LabelLine();
		this.luk_CntrctType = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.lb_ItemType = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Descriptio = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.luk_ResponseU = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.luk_ResponsUnt = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.chk_Deleted = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.chk_Renewal = new Apzon.Controls.Editor.CheckEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.chk_InclHldays = new Apzon.Controls.Editor.CheckEditEx();
		this.panel1 = new System.Windows.Forms.Panel();
		this.chk_InclTravel = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_InclWork = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_InclParts = new Apzon.Controls.Editor.CheckEditEx();
		this.hrEdit_SunEnd = new Apzon.Controls.Controls.HourEdit();
		this.hrEdit_SunStrart = new Apzon.Controls.Controls.HourEdit();
		this.hrEdit_SatEnd = new Apzon.Controls.Controls.HourEdit();
		this.hrEdit_SatStart = new Apzon.Controls.Controls.HourEdit();
		this.hrEdit_FriEnd = new Apzon.Controls.Controls.HourEdit();
		this.hrEdit_FriStart = new Apzon.Controls.Controls.HourEdit();
		this.hrEdit_ThuEnd = new Apzon.Controls.Controls.HourEdit();
		this.hrEdit_ThuStart = new Apzon.Controls.Controls.HourEdit();
		this.hrEdit_WedEnd = new Apzon.Controls.Controls.HourEdit();
		this.hrEdit_WedStart = new Apzon.Controls.Controls.HourEdit();
		this.hrEdit_TueEnd = new Apzon.Controls.Controls.HourEdit();
		this.hrEdit_TueStart = new Apzon.Controls.Controls.HourEdit();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.hrEdit_MonEnd = new Apzon.Controls.Controls.HourEdit();
		this.hrEdit_MonStart = new Apzon.Controls.Controls.HourEdit();
		this.chk_SunEnabled = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_SatEnabled = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_FriEnabled = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_ThuEnabled = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_WedEnabled = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_TueEnabled = new Apzon.Controls.Editor.CheckEditEx();
		this.chk_MonEnabled = new Apzon.Controls.Editor.CheckEditEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.memo_Remark = new Apzon.Controls.Editor.MemoEditEx();
		this.xtraTabPage3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.ucAttachment1 = new Apzon.Forms.UserControls.ucAttachment();
		this.luk_RemindUnit = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.calc_ResponseV = new Apzon.Controls.Editor.TextEditEx();
		this.calc_ResponsVal = new Apzon.Controls.Editor.TextEditEx();
		this.calc_Duration = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		this.txt_DurationUnit = new Apzon.Controls.Editor.TextEditEx();
		this.calc_RemindVal = new Apzon.Controls.Editor.TextEditExPrice.TextEditExPrice();
		((System.ComponentModel.ISupportInitialize)this.txt_TmpltName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CntrctType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Descriptio.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ResponseU.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ResponsUnt.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Deleted.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Renewal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_InclHldays.Properties).BeginInit();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.chk_InclTravel.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InclWork.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InclParts.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_SunEnd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_SunStrart.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_SatEnd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_SatStart.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_FriEnd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_FriStart.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_ThuEnd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_ThuStart.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_WedEnd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_WedStart.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_TueEnd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_TueStart.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_MonEnd.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_MonStart.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_SunEnabled.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_SatEnabled.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_FriEnabled.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ThuEnabled.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_WedEnabled.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_TueEnabled.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_MonEnabled.Properties).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.memo_Remark.Properties).BeginInit();
		this.xtraTabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_RemindUnit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_ResponseV.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_ResponsVal.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Duration.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DurationUnit.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_RemindVal.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(85, 334);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 19;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(8, 334);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 18;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.txt_TmpltName.FieldName = "TmpltName";
		this.txt_TmpltName.IsSearchControl = false;
		this.txt_TmpltName.Location = new System.Drawing.Point(126, 6);
		this.txt_TmpltName.Name = "txt_TmpltName";
		this.txt_TmpltName.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_TmpltName.Properties.Appearance.Options.UseFont = true;
		this.txt_TmpltName.Properties.AutoHeight = false;
		this.txt_TmpltName.Size = new System.Drawing.Size(132, 18);
		this.txt_TmpltName.TabIndex = 20;
		this.txt_TmpltName.Tag = "OITM,SuppCatNum";
		this.txt_TmpltName.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.lb_CatalogNo.FieldName = null;
		this.lb_CatalogNo.HideWhenEmpty = false;
		this.lb_CatalogNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_CatalogNo.Location = new System.Drawing.Point(3, 7);
		this.lb_CatalogNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lb_CatalogNo.Name = "lb_CatalogNo";
		this.lb_CatalogNo.ShowUnderLine = true;
		this.lb_CatalogNo.Size = new System.Drawing.Size(124, 20);
		this.lb_CatalogNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_CatalogNo.TabIndex = 21;
		this.lb_CatalogNo.TabStop = false;
		this.lb_CatalogNo.Utext = "Name";
		this.luk_CntrctType.FieldName = "CntrctType";
		this.luk_CntrctType.IsSearchControl = false;
		this.luk_CntrctType.Location = new System.Drawing.Point(126, 25);
		this.luk_CntrctType.Name = "luk_CntrctType";
		this.luk_CntrctType.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_CntrctType.Properties.Appearance.Options.UseFont = true;
		this.luk_CntrctType.Properties.AutoHeight = false;
		this.luk_CntrctType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_CntrctType.Properties.IsSearchControl = false;
		this.luk_CntrctType.Properties.NullText = "";
		this.luk_CntrctType.Size = new System.Drawing.Size(132, 18);
		this.luk_CntrctType.TabIndex = 23;
		this.luk_CntrctType.Tag = "OITM,ItemType";
		this.luk_CntrctType.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.lb_ItemType.FieldName = null;
		this.lb_ItemType.HideWhenEmpty = false;
		this.lb_ItemType.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ItemType.Location = new System.Drawing.Point(3, 26);
		this.lb_ItemType.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.lb_ItemType.Name = "lb_ItemType";
		this.lb_ItemType.ShowUnderLine = true;
		this.lb_ItemType.Size = new System.Drawing.Size(124, 21);
		this.lb_ItemType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ItemType.TabIndex = 22;
		this.lb_ItemType.TabStop = false;
		this.lb_ItemType.Utext = "Contract Type";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(3, 45);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(124, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 25;
		this.labelLine1.TabStop = false;
		this.labelLine1.Utext = "Response Time";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(3, 64);
		this.labelLine2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(124, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 27;
		this.labelLine2.TabStop = false;
		this.labelLine2.Utext = "Resolution Time";
		this.txt_Descriptio.FieldName = "Descriptio";
		this.txt_Descriptio.IsSearchControl = false;
		this.txt_Descriptio.Location = new System.Drawing.Point(126, 82);
		this.txt_Descriptio.Name = "txt_Descriptio";
		this.txt_Descriptio.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_Descriptio.Properties.Appearance.Options.UseFont = true;
		this.txt_Descriptio.Properties.AutoHeight = false;
		this.txt_Descriptio.Size = new System.Drawing.Size(386, 18);
		this.txt_Descriptio.TabIndex = 28;
		this.txt_Descriptio.Tag = "OITM,SuppCatNum";
		this.txt_Descriptio.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(3, 83);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(124, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 29;
		this.labelLine3.TabStop = false;
		this.labelLine3.Utext = "Description";
		this.luk_ResponseU.FieldName = "ResponseU";
		this.luk_ResponseU.IsSearchControl = false;
		this.luk_ResponseU.Location = new System.Drawing.Point(172, 44);
		this.luk_ResponseU.Name = "luk_ResponseU";
		this.luk_ResponseU.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_ResponseU.Properties.Appearance.Options.UseFont = true;
		this.luk_ResponseU.Properties.AutoHeight = false;
		this.luk_ResponseU.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ResponseU.Properties.IsSearchControl = false;
		this.luk_ResponseU.Properties.NullText = "";
		this.luk_ResponseU.Size = new System.Drawing.Size(86, 18);
		this.luk_ResponseU.TabIndex = 30;
		this.luk_ResponseU.Tag = "OITM,ItemType";
		this.luk_ResponseU.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.luk_ResponsUnt.FieldName = "ResponsUnt";
		this.luk_ResponsUnt.IsSearchControl = false;
		this.luk_ResponsUnt.Location = new System.Drawing.Point(172, 63);
		this.luk_ResponsUnt.Name = "luk_ResponsUnt";
		this.luk_ResponsUnt.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_ResponsUnt.Properties.Appearance.Options.UseFont = true;
		this.luk_ResponsUnt.Properties.AutoHeight = false;
		this.luk_ResponsUnt.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ResponsUnt.Properties.IsSearchControl = false;
		this.luk_ResponsUnt.Properties.NullText = "";
		this.luk_ResponsUnt.Size = new System.Drawing.Size(86, 18);
		this.luk_ResponsUnt.TabIndex = 31;
		this.luk_ResponsUnt.Tag = "OITM,ItemType";
		this.luk_ResponsUnt.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_Deleted.FieldName = "Deleted";
		this.chk_Deleted.IsSearchControl = false;
		this.chk_Deleted.Location = new System.Drawing.Point(286, 5);
		this.chk_Deleted.Name = "chk_Deleted";
		this.chk_Deleted.Properties.Caption = "E&xpired";
		this.chk_Deleted.Properties.DisplayValueChecked = "Y";
		this.chk_Deleted.Properties.DisplayValueGrayed = "N";
		this.chk_Deleted.Properties.DisplayValueUnchecked = "N";
		this.chk_Deleted.Properties.ValueChecked = "Y";
		this.chk_Deleted.Properties.ValueGrayed = "N";
		this.chk_Deleted.Properties.ValueUnchecked = "N";
		this.chk_Deleted.Size = new System.Drawing.Size(75, 19);
		this.chk_Deleted.TabIndex = 32;
		this.chk_Deleted.CheckedChanged += new System.EventHandler(this.chk_Deleted_CheckedChanged);
		this.chk_Deleted.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(286, 23);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(96, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 34;
		this.labelLine4.TabStop = false;
		this.labelLine4.Utext = "Duration";
		this.chk_Renewal.FieldName = "Renewal";
		this.chk_Renewal.IsSearchControl = false;
		this.chk_Renewal.Location = new System.Drawing.Point(286, 42);
		this.chk_Renewal.Name = "chk_Renewal";
		this.chk_Renewal.Properties.Caption = "&Renewal";
		this.chk_Renewal.Properties.DisplayValueChecked = "Y";
		this.chk_Renewal.Properties.DisplayValueGrayed = "N";
		this.chk_Renewal.Properties.DisplayValueUnchecked = "N";
		this.chk_Renewal.Properties.ValueChecked = "Y";
		this.chk_Renewal.Properties.ValueGrayed = "N";
		this.chk_Renewal.Properties.ValueUnchecked = "N";
		this.chk_Renewal.Size = new System.Drawing.Size(75, 19);
		this.chk_Renewal.TabIndex = 35;
		this.chk_Renewal.CheckedChanged += new System.EventHandler(this.chk_Renewal_CheckedChanged);
		this.chk_Renewal.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(286, 61);
		this.labelLine5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(96, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 38;
		this.labelLine5.TabStop = false;
		this.labelLine5.Utext = "Reminder";
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.xtraTabControl1.Appearance.Options.UseBackColor = true;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(8, 106);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.PaintStyleName = "Standard";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(504, 222);
		this.xtraTabControl1.TabIndex = 40;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.xtraTabPage1, this.xtraTabPage2, this.xtraTabPage3 });
		this.xtraTabPage1.Controls.Add(this.labelLine8);
		this.xtraTabPage1.Controls.Add(this.chk_InclHldays);
		this.xtraTabPage1.Controls.Add(this.panel1);
		this.xtraTabPage1.Controls.Add(this.hrEdit_SunEnd);
		this.xtraTabPage1.Controls.Add(this.hrEdit_SunStrart);
		this.xtraTabPage1.Controls.Add(this.hrEdit_SatEnd);
		this.xtraTabPage1.Controls.Add(this.hrEdit_SatStart);
		this.xtraTabPage1.Controls.Add(this.hrEdit_FriEnd);
		this.xtraTabPage1.Controls.Add(this.hrEdit_FriStart);
		this.xtraTabPage1.Controls.Add(this.hrEdit_ThuEnd);
		this.xtraTabPage1.Controls.Add(this.hrEdit_ThuStart);
		this.xtraTabPage1.Controls.Add(this.hrEdit_WedEnd);
		this.xtraTabPage1.Controls.Add(this.hrEdit_WedStart);
		this.xtraTabPage1.Controls.Add(this.hrEdit_TueEnd);
		this.xtraTabPage1.Controls.Add(this.hrEdit_TueStart);
		this.xtraTabPage1.Controls.Add(this.labelLine7);
		this.xtraTabPage1.Controls.Add(this.labelLine6);
		this.xtraTabPage1.Controls.Add(this.hrEdit_MonEnd);
		this.xtraTabPage1.Controls.Add(this.hrEdit_MonStart);
		this.xtraTabPage1.Controls.Add(this.chk_SunEnabled);
		this.xtraTabPage1.Controls.Add(this.chk_SatEnabled);
		this.xtraTabPage1.Controls.Add(this.chk_FriEnabled);
		this.xtraTabPage1.Controls.Add(this.chk_ThuEnabled);
		this.xtraTabPage1.Controls.Add(this.chk_WedEnabled);
		this.xtraTabPage1.Controls.Add(this.chk_TueEnabled);
		this.xtraTabPage1.Controls.Add(this.chk_MonEnabled);
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.Size = new System.Drawing.Size(500, 198);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "C&overage";
		this.labelLine8.FieldName = null;
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(331, 40);
		this.labelLine8.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ShowUnderLine = false;
		this.labelLine8.Size = new System.Drawing.Size(56, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 22;
		this.labelLine8.TabStop = false;
		this.labelLine8.Utext = "Include";
		this.chk_InclHldays.FieldName = "InclHldays";
		this.chk_InclHldays.IsSearchControl = false;
		this.chk_InclHldays.Location = new System.Drawing.Point(331, 155);
		this.chk_InclHldays.Name = "chk_InclHldays";
		this.chk_InclHldays.Properties.Caption = "Including Holidays";
		this.chk_InclHldays.Properties.DisplayValueChecked = "Y";
		this.chk_InclHldays.Properties.DisplayValueGrayed = "N";
		this.chk_InclHldays.Properties.DisplayValueUnchecked = "N";
		this.chk_InclHldays.Properties.ValueChecked = "Y";
		this.chk_InclHldays.Properties.ValueGrayed = "N";
		this.chk_InclHldays.Properties.ValueUnchecked = "N";
		this.chk_InclHldays.Size = new System.Drawing.Size(105, 19);
		this.chk_InclHldays.TabIndex = 57;
		this.chk_InclHldays.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
		this.panel1.Controls.Add(this.chk_InclTravel);
		this.panel1.Controls.Add(this.chk_InclWork);
		this.panel1.Controls.Add(this.chk_InclParts);
		this.panel1.Location = new System.Drawing.Point(331, 62);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(144, 88);
		this.panel1.TabIndex = 56;
		this.chk_InclTravel.FieldName = "InclTravel";
		this.chk_InclTravel.IsSearchControl = false;
		this.chk_InclTravel.Location = new System.Drawing.Point(8, 58);
		this.chk_InclTravel.Name = "chk_InclTravel";
		this.chk_InclTravel.Properties.Caption = "Travel";
		this.chk_InclTravel.Properties.DisplayValueChecked = "Y";
		this.chk_InclTravel.Properties.DisplayValueGrayed = "N";
		this.chk_InclTravel.Properties.DisplayValueUnchecked = "N";
		this.chk_InclTravel.Properties.ValueChecked = "Y";
		this.chk_InclTravel.Properties.ValueGrayed = "N";
		this.chk_InclTravel.Properties.ValueUnchecked = "N";
		this.chk_InclTravel.Size = new System.Drawing.Size(85, 19);
		this.chk_InclTravel.TabIndex = 38;
		this.chk_InclTravel.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_InclWork.FieldName = "InclWork";
		this.chk_InclWork.IsSearchControl = false;
		this.chk_InclWork.Location = new System.Drawing.Point(8, 33);
		this.chk_InclWork.Name = "chk_InclWork";
		this.chk_InclWork.Properties.Caption = "Labor";
		this.chk_InclWork.Properties.DisplayValueChecked = "Y";
		this.chk_InclWork.Properties.DisplayValueGrayed = "N";
		this.chk_InclWork.Properties.DisplayValueUnchecked = "N";
		this.chk_InclWork.Properties.ValueChecked = "Y";
		this.chk_InclWork.Properties.ValueGrayed = "N";
		this.chk_InclWork.Properties.ValueUnchecked = "N";
		this.chk_InclWork.Size = new System.Drawing.Size(75, 19);
		this.chk_InclWork.TabIndex = 37;
		this.chk_InclWork.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_InclParts.FieldName = "InclParts";
		this.chk_InclParts.IsSearchControl = false;
		this.chk_InclParts.Location = new System.Drawing.Point(8, 8);
		this.chk_InclParts.Name = "chk_InclParts";
		this.chk_InclParts.Properties.Caption = "Parts";
		this.chk_InclParts.Properties.DisplayValueChecked = "Y";
		this.chk_InclParts.Properties.DisplayValueGrayed = "N";
		this.chk_InclParts.Properties.DisplayValueUnchecked = "N";
		this.chk_InclParts.Properties.ValueChecked = "Y";
		this.chk_InclParts.Properties.ValueGrayed = "N";
		this.chk_InclParts.Properties.ValueUnchecked = "N";
		this.chk_InclParts.Size = new System.Drawing.Size(75, 19);
		this.chk_InclParts.TabIndex = 36;
		this.chk_InclParts.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_SunEnd.FieldName = "SunEnd";
		this.hrEdit_SunEnd.IsSearchControl = false;
		this.hrEdit_SunEnd.Location = new System.Drawing.Point(209, 155);
		this.hrEdit_SunEnd.Name = "hrEdit_SunEnd";
		this.hrEdit_SunEnd.Properties.AutoHeight = false;
		this.hrEdit_SunEnd.Properties.IsSearchControl = false;
		this.hrEdit_SunEnd.Properties.MaxLength = 5;
		this.hrEdit_SunEnd.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_SunEnd.TabIndex = 55;
		this.hrEdit_SunEnd.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_SunStrart.FieldName = "SunStrart";
		this.hrEdit_SunStrart.IsSearchControl = false;
		this.hrEdit_SunStrart.Location = new System.Drawing.Point(101, 155);
		this.hrEdit_SunStrart.Name = "hrEdit_SunStrart";
		this.hrEdit_SunStrart.Properties.AutoHeight = false;
		this.hrEdit_SunStrart.Properties.IsSearchControl = false;
		this.hrEdit_SunStrart.Properties.MaxLength = 5;
		this.hrEdit_SunStrart.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_SunStrart.TabIndex = 54;
		this.hrEdit_SunStrart.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_SatEnd.FieldName = "SatEnd";
		this.hrEdit_SatEnd.IsSearchControl = false;
		this.hrEdit_SatEnd.Location = new System.Drawing.Point(209, 134);
		this.hrEdit_SatEnd.Name = "hrEdit_SatEnd";
		this.hrEdit_SatEnd.Properties.AutoHeight = false;
		this.hrEdit_SatEnd.Properties.IsSearchControl = false;
		this.hrEdit_SatEnd.Properties.MaxLength = 5;
		this.hrEdit_SatEnd.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_SatEnd.TabIndex = 53;
		this.hrEdit_SatEnd.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_SatStart.FieldName = "SatStart";
		this.hrEdit_SatStart.IsSearchControl = false;
		this.hrEdit_SatStart.Location = new System.Drawing.Point(101, 134);
		this.hrEdit_SatStart.Name = "hrEdit_SatStart";
		this.hrEdit_SatStart.Properties.AutoHeight = false;
		this.hrEdit_SatStart.Properties.IsSearchControl = false;
		this.hrEdit_SatStart.Properties.MaxLength = 5;
		this.hrEdit_SatStart.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_SatStart.TabIndex = 52;
		this.hrEdit_SatStart.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_FriEnd.FieldName = "FriEnd";
		this.hrEdit_FriEnd.IsSearchControl = false;
		this.hrEdit_FriEnd.Location = new System.Drawing.Point(209, 114);
		this.hrEdit_FriEnd.Name = "hrEdit_FriEnd";
		this.hrEdit_FriEnd.Properties.AutoHeight = false;
		this.hrEdit_FriEnd.Properties.IsSearchControl = false;
		this.hrEdit_FriEnd.Properties.MaxLength = 5;
		this.hrEdit_FriEnd.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_FriEnd.TabIndex = 51;
		this.hrEdit_FriEnd.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_FriStart.FieldName = "FriStart";
		this.hrEdit_FriStart.IsSearchControl = false;
		this.hrEdit_FriStart.Location = new System.Drawing.Point(101, 114);
		this.hrEdit_FriStart.Name = "hrEdit_FriStart";
		this.hrEdit_FriStart.Properties.AutoHeight = false;
		this.hrEdit_FriStart.Properties.IsSearchControl = false;
		this.hrEdit_FriStart.Properties.MaxLength = 5;
		this.hrEdit_FriStart.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_FriStart.TabIndex = 50;
		this.hrEdit_FriStart.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_ThuEnd.FieldName = "ThuEnd";
		this.hrEdit_ThuEnd.IsSearchControl = false;
		this.hrEdit_ThuEnd.Location = new System.Drawing.Point(209, 93);
		this.hrEdit_ThuEnd.Name = "hrEdit_ThuEnd";
		this.hrEdit_ThuEnd.Properties.AutoHeight = false;
		this.hrEdit_ThuEnd.Properties.IsSearchControl = false;
		this.hrEdit_ThuEnd.Properties.MaxLength = 5;
		this.hrEdit_ThuEnd.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_ThuEnd.TabIndex = 49;
		this.hrEdit_ThuEnd.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_ThuStart.FieldName = "ThuStart";
		this.hrEdit_ThuStart.IsSearchControl = false;
		this.hrEdit_ThuStart.Location = new System.Drawing.Point(101, 93);
		this.hrEdit_ThuStart.Name = "hrEdit_ThuStart";
		this.hrEdit_ThuStart.Properties.AutoHeight = false;
		this.hrEdit_ThuStart.Properties.IsSearchControl = false;
		this.hrEdit_ThuStart.Properties.MaxLength = 5;
		this.hrEdit_ThuStart.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_ThuStart.TabIndex = 48;
		this.hrEdit_ThuStart.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_WedEnd.FieldName = "WedEnd";
		this.hrEdit_WedEnd.IsSearchControl = false;
		this.hrEdit_WedEnd.Location = new System.Drawing.Point(209, 72);
		this.hrEdit_WedEnd.Name = "hrEdit_WedEnd";
		this.hrEdit_WedEnd.Properties.AutoHeight = false;
		this.hrEdit_WedEnd.Properties.IsSearchControl = false;
		this.hrEdit_WedEnd.Properties.MaxLength = 5;
		this.hrEdit_WedEnd.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_WedEnd.TabIndex = 47;
		this.hrEdit_WedEnd.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_WedStart.FieldName = "WedStart";
		this.hrEdit_WedStart.IsSearchControl = false;
		this.hrEdit_WedStart.Location = new System.Drawing.Point(101, 72);
		this.hrEdit_WedStart.Name = "hrEdit_WedStart";
		this.hrEdit_WedStart.Properties.AutoHeight = false;
		this.hrEdit_WedStart.Properties.IsSearchControl = false;
		this.hrEdit_WedStart.Properties.MaxLength = 5;
		this.hrEdit_WedStart.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_WedStart.TabIndex = 46;
		this.hrEdit_WedStart.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_TueEnd.FieldName = "TueEnd";
		this.hrEdit_TueEnd.IsSearchControl = false;
		this.hrEdit_TueEnd.Location = new System.Drawing.Point(209, 51);
		this.hrEdit_TueEnd.Name = "hrEdit_TueEnd";
		this.hrEdit_TueEnd.Properties.AutoHeight = false;
		this.hrEdit_TueEnd.Properties.IsSearchControl = false;
		this.hrEdit_TueEnd.Properties.MaxLength = 5;
		this.hrEdit_TueEnd.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_TueEnd.TabIndex = 45;
		this.hrEdit_TueEnd.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_TueStart.FieldName = "TueStart";
		this.hrEdit_TueStart.IsSearchControl = false;
		this.hrEdit_TueStart.Location = new System.Drawing.Point(101, 51);
		this.hrEdit_TueStart.Name = "hrEdit_TueStart";
		this.hrEdit_TueStart.Properties.AutoHeight = false;
		this.hrEdit_TueStart.Properties.IsSearchControl = false;
		this.hrEdit_TueStart.Properties.MaxLength = 5;
		this.hrEdit_TueStart.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_TueStart.TabIndex = 44;
		this.hrEdit_TueStart.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.labelLine7.FieldName = null;
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(209, 10);
		this.labelLine7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ShowUnderLine = false;
		this.labelLine7.Size = new System.Drawing.Size(81, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 43;
		this.labelLine7.TabStop = false;
		this.labelLine7.Utext = "End Time";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(101, 10);
		this.labelLine6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ShowUnderLine = false;
		this.labelLine6.Size = new System.Drawing.Size(81, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 42;
		this.labelLine6.TabStop = false;
		this.labelLine6.Utext = "Start Time";
		this.hrEdit_MonEnd.FieldName = "MonEnd";
		this.hrEdit_MonEnd.IsSearchControl = false;
		this.hrEdit_MonEnd.Location = new System.Drawing.Point(209, 30);
		this.hrEdit_MonEnd.Name = "hrEdit_MonEnd";
		this.hrEdit_MonEnd.Properties.AutoHeight = false;
		this.hrEdit_MonEnd.Properties.IsSearchControl = false;
		this.hrEdit_MonEnd.Properties.MaxLength = 5;
		this.hrEdit_MonEnd.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_MonEnd.TabIndex = 41;
		this.hrEdit_MonEnd.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.hrEdit_MonStart.FieldName = "MonStart";
		this.hrEdit_MonStart.IsSearchControl = false;
		this.hrEdit_MonStart.Location = new System.Drawing.Point(101, 30);
		this.hrEdit_MonStart.Name = "hrEdit_MonStart";
		this.hrEdit_MonStart.Properties.AutoHeight = false;
		this.hrEdit_MonStart.Properties.IsSearchControl = false;
		this.hrEdit_MonStart.Properties.MaxLength = 5;
		this.hrEdit_MonStart.Size = new System.Drawing.Size(86, 18);
		this.hrEdit_MonStart.TabIndex = 40;
		this.hrEdit_MonStart.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_SunEnabled.FieldName = null;
		this.chk_SunEnabled.IsSearchControl = false;
		this.chk_SunEnabled.Location = new System.Drawing.Point(12, 157);
		this.chk_SunEnabled.Name = "chk_SunEnabled";
		this.chk_SunEnabled.Properties.Caption = "S&unday";
		this.chk_SunEnabled.Properties.DisplayValueChecked = "Y";
		this.chk_SunEnabled.Properties.DisplayValueGrayed = "N";
		this.chk_SunEnabled.Properties.DisplayValueUnchecked = "N";
		this.chk_SunEnabled.Properties.ValueChecked = "Y";
		this.chk_SunEnabled.Properties.ValueGrayed = "N";
		this.chk_SunEnabled.Properties.ValueUnchecked = "N";
		this.chk_SunEnabled.Size = new System.Drawing.Size(75, 19);
		this.chk_SunEnabled.TabIndex = 39;
		this.chk_SunEnabled.CheckedChanged += new System.EventHandler(this.chk_SunEnabled_CheckedChanged);
		this.chk_SunEnabled.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_SatEnabled.FieldName = null;
		this.chk_SatEnabled.IsSearchControl = false;
		this.chk_SatEnabled.Location = new System.Drawing.Point(12, 136);
		this.chk_SatEnabled.Name = "chk_SatEnabled";
		this.chk_SatEnabled.Properties.Caption = "Saturda&y";
		this.chk_SatEnabled.Properties.DisplayValueChecked = "Y";
		this.chk_SatEnabled.Properties.DisplayValueGrayed = "N";
		this.chk_SatEnabled.Properties.DisplayValueUnchecked = "N";
		this.chk_SatEnabled.Properties.ValueChecked = "Y";
		this.chk_SatEnabled.Properties.ValueGrayed = "N";
		this.chk_SatEnabled.Properties.ValueUnchecked = "N";
		this.chk_SatEnabled.Size = new System.Drawing.Size(75, 19);
		this.chk_SatEnabled.TabIndex = 38;
		this.chk_SatEnabled.CheckedChanged += new System.EventHandler(this.chk_SatEnabled_CheckedChanged);
		this.chk_SatEnabled.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_FriEnabled.FieldName = null;
		this.chk_FriEnabled.IsSearchControl = false;
		this.chk_FriEnabled.Location = new System.Drawing.Point(12, 115);
		this.chk_FriEnabled.Name = "chk_FriEnabled";
		this.chk_FriEnabled.Properties.Caption = "Friday";
		this.chk_FriEnabled.Properties.DisplayValueChecked = "Y";
		this.chk_FriEnabled.Properties.DisplayValueGrayed = "N";
		this.chk_FriEnabled.Properties.DisplayValueUnchecked = "N";
		this.chk_FriEnabled.Properties.ValueChecked = "Y";
		this.chk_FriEnabled.Properties.ValueGrayed = "N";
		this.chk_FriEnabled.Properties.ValueUnchecked = "N";
		this.chk_FriEnabled.Size = new System.Drawing.Size(75, 19);
		this.chk_FriEnabled.TabIndex = 37;
		this.chk_FriEnabled.CheckedChanged += new System.EventHandler(this.chk_FriEnabled_CheckedChanged);
		this.chk_FriEnabled.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_ThuEnabled.FieldName = null;
		this.chk_ThuEnabled.IsSearchControl = false;
		this.chk_ThuEnabled.Location = new System.Drawing.Point(12, 93);
		this.chk_ThuEnabled.Name = "chk_ThuEnabled";
		this.chk_ThuEnabled.Properties.Caption = "Thursday";
		this.chk_ThuEnabled.Properties.DisplayValueChecked = "Y";
		this.chk_ThuEnabled.Properties.DisplayValueGrayed = "N";
		this.chk_ThuEnabled.Properties.DisplayValueUnchecked = "N";
		this.chk_ThuEnabled.Properties.ValueChecked = "Y";
		this.chk_ThuEnabled.Properties.ValueGrayed = "N";
		this.chk_ThuEnabled.Properties.ValueUnchecked = "N";
		this.chk_ThuEnabled.Size = new System.Drawing.Size(75, 19);
		this.chk_ThuEnabled.TabIndex = 36;
		this.chk_ThuEnabled.CheckedChanged += new System.EventHandler(this.chk_ThuEnabled_CheckedChanged);
		this.chk_ThuEnabled.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_WedEnabled.FieldName = null;
		this.chk_WedEnabled.IsSearchControl = false;
		this.chk_WedEnabled.Location = new System.Drawing.Point(12, 72);
		this.chk_WedEnabled.Name = "chk_WedEnabled";
		this.chk_WedEnabled.Properties.Caption = "Wednesday";
		this.chk_WedEnabled.Properties.DisplayValueChecked = "Y";
		this.chk_WedEnabled.Properties.DisplayValueGrayed = "N";
		this.chk_WedEnabled.Properties.DisplayValueUnchecked = "N";
		this.chk_WedEnabled.Properties.ValueChecked = "Y";
		this.chk_WedEnabled.Properties.ValueGrayed = "N";
		this.chk_WedEnabled.Properties.ValueUnchecked = "N";
		this.chk_WedEnabled.Size = new System.Drawing.Size(85, 19);
		this.chk_WedEnabled.TabIndex = 35;
		this.chk_WedEnabled.CheckedChanged += new System.EventHandler(this.chk_WedEnabled_CheckedChanged);
		this.chk_WedEnabled.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_TueEnabled.FieldName = null;
		this.chk_TueEnabled.IsSearchControl = false;
		this.chk_TueEnabled.Location = new System.Drawing.Point(12, 52);
		this.chk_TueEnabled.Name = "chk_TueEnabled";
		this.chk_TueEnabled.Properties.Caption = "Tuesday";
		this.chk_TueEnabled.Properties.DisplayValueChecked = "Y";
		this.chk_TueEnabled.Properties.DisplayValueGrayed = "N";
		this.chk_TueEnabled.Properties.DisplayValueUnchecked = "N";
		this.chk_TueEnabled.Properties.ValueChecked = "Y";
		this.chk_TueEnabled.Properties.ValueGrayed = "N";
		this.chk_TueEnabled.Properties.ValueUnchecked = "N";
		this.chk_TueEnabled.Size = new System.Drawing.Size(75, 19);
		this.chk_TueEnabled.TabIndex = 34;
		this.chk_TueEnabled.CheckedChanged += new System.EventHandler(this.chk_TueEnabled_CheckedChanged);
		this.chk_TueEnabled.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.chk_MonEnabled.FieldName = "MonEnabled";
		this.chk_MonEnabled.IsSearchControl = false;
		this.chk_MonEnabled.Location = new System.Drawing.Point(12, 31);
		this.chk_MonEnabled.Name = "chk_MonEnabled";
		this.chk_MonEnabled.Properties.Caption = "Mo&nday";
		this.chk_MonEnabled.Properties.DisplayValueChecked = "Y";
		this.chk_MonEnabled.Properties.DisplayValueGrayed = "N";
		this.chk_MonEnabled.Properties.DisplayValueUnchecked = "N";
		this.chk_MonEnabled.Properties.ValueChecked = "Y";
		this.chk_MonEnabled.Properties.ValueGrayed = "N";
		this.chk_MonEnabled.Properties.ValueUnchecked = "N";
		this.chk_MonEnabled.Size = new System.Drawing.Size(75, 19);
		this.chk_MonEnabled.TabIndex = 33;
		this.chk_MonEnabled.CheckedChanged += new System.EventHandler(this.chk_MonEnabled_CheckedChanged);
		this.chk_MonEnabled.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.xtraTabPage2.Controls.Add(this.memo_Remark);
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.Size = new System.Drawing.Size(500, 198);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Remar&ks";
		this.memo_Remark.FieldName = "Remark";
		this.memo_Remark.IsSearchControl = false;
		this.memo_Remark.Location = new System.Drawing.Point(10, 8);
		this.memo_Remark.Name = "memo_Remark";
		this.memo_Remark.Size = new System.Drawing.Size(479, 204);
		this.memo_Remark.TabIndex = 0;
		this.memo_Remark.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.xtraTabPage3.Controls.Add(this.ucAttachment1);
		this.xtraTabPage3.FieldName = null;
		this.xtraTabPage3.IsSearchControl = false;
		this.xtraTabPage3.Name = "xtraTabPage3";
		this.xtraTabPage3.Size = new System.Drawing.Size(500, 198);
		this.xtraTabPage3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage3.Text = "&Attachments";
		this.ucAttachment1.DataSource = null;
		this.ucAttachment1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.ucAttachment1.FormId = "oContractTemplate";
		this.ucAttachment1.ItemId = "ATC1";
		this.ucAttachment1.Location = new System.Drawing.Point(0, 0);
		this.ucAttachment1.Name = "ucAttachment1";
		this.ucAttachment1.Size = new System.Drawing.Size(500, 198);
		this.ucAttachment1.TabIndex = 0;
		this.luk_RemindUnit.FieldName = "RemindUnit";
		this.luk_RemindUnit.IsSearchControl = false;
		this.luk_RemindUnit.Location = new System.Drawing.Point(426, 60);
		this.luk_RemindUnit.Name = "luk_RemindUnit";
		this.luk_RemindUnit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.luk_RemindUnit.Properties.Appearance.Options.UseFont = true;
		this.luk_RemindUnit.Properties.AutoHeight = false;
		this.luk_RemindUnit.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_RemindUnit.Properties.IsSearchControl = false;
		this.luk_RemindUnit.Properties.NullText = "";
		this.luk_RemindUnit.Size = new System.Drawing.Size(86, 18);
		this.luk_RemindUnit.TabIndex = 42;
		this.luk_RemindUnit.Tag = "OITM,ItemType";
		this.luk_RemindUnit.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.calc_ResponseV.FieldName = "ResponseV";
		this.calc_ResponseV.IsSearchControl = false;
		this.calc_ResponseV.Location = new System.Drawing.Point(126, 44);
		this.calc_ResponseV.Name = "calc_ResponseV";
		this.calc_ResponseV.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_ResponseV.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_ResponseV.Properties.AutoHeight = false;
		this.calc_ResponseV.Properties.Mask.EditMask = "\\d{1,6}";
		this.calc_ResponseV.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.calc_ResponseV.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.calc_ResponseV.Size = new System.Drawing.Size(45, 18);
		this.calc_ResponseV.TabIndex = 58;
		this.calc_ResponseV.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.calc_ResponsVal.FieldName = "ResponsVal";
		this.calc_ResponsVal.IsSearchControl = false;
		this.calc_ResponsVal.Location = new System.Drawing.Point(126, 63);
		this.calc_ResponsVal.Name = "calc_ResponsVal";
		this.calc_ResponsVal.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_ResponsVal.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_ResponsVal.Properties.AutoHeight = false;
		this.calc_ResponsVal.Properties.Mask.EditMask = "\\d{1,6}";
		this.calc_ResponsVal.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.calc_ResponsVal.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.calc_ResponsVal.Size = new System.Drawing.Size(45, 18);
		this.calc_ResponsVal.TabIndex = 59;
		this.calc_ResponsVal.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.calc_Duration.Currency = null;
		this.calc_Duration.FieldName = "Duration";
		this.calc_Duration.IsSearchControl = false;
		this.calc_Duration.Location = new System.Drawing.Point(379, 22);
		this.calc_Duration.Name = "calc_Duration";
		this.calc_Duration.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_Duration.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_Duration.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.calc_Duration.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.calc_Duration.Size = new System.Drawing.Size(45, 20);
		this.calc_Duration.TabIndex = 60;
		this.calc_Duration.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.txt_DurationUnit.EditValue = "Month(s)";
		this.txt_DurationUnit.Enabled = false;
		this.txt_DurationUnit.FieldName = null;
		this.txt_DurationUnit.IsSearchControl = false;
		this.txt_DurationUnit.Location = new System.Drawing.Point(426, 22);
		this.txt_DurationUnit.Name = "txt_DurationUnit";
		this.txt_DurationUnit.Properties.Appearance.Font = new System.Drawing.Font("Segoe UI", 7f);
		this.txt_DurationUnit.Properties.Appearance.Options.UseFont = true;
		this.txt_DurationUnit.Properties.AutoHeight = false;
		this.txt_DurationUnit.Size = new System.Drawing.Size(86, 18);
		this.txt_DurationUnit.TabIndex = 43;
		this.txt_DurationUnit.Tag = "OITM,SuppCatNum";
		this.txt_DurationUnit.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		this.calc_RemindVal.Currency = null;
		this.calc_RemindVal.FieldName = "RemindVal";
		this.calc_RemindVal.IsSearchControl = false;
		this.calc_RemindVal.Location = new System.Drawing.Point(379, 60);
		this.calc_RemindVal.Name = "calc_RemindVal";
		this.calc_RemindVal.Properties.Appearance.Options.UseTextOptions = true;
		this.calc_RemindVal.Properties.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.calc_RemindVal.Properties.AutoHeight = false;
		this.calc_RemindVal.Properties.Mask.UseMaskAsDisplayFormat = true;
		this.calc_RemindVal.Size = new System.Drawing.Size(45, 18);
		this.calc_RemindVal.TabIndex = 61;
		this.calc_RemindVal.EditValueChanged += new System.EventHandler(this.Controls_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(518, 363);
		base.Controls.Add(this.calc_RemindVal);
		base.Controls.Add(this.txt_DurationUnit);
		base.Controls.Add(this.calc_Duration);
		base.Controls.Add(this.luk_RemindUnit);
		base.Controls.Add(this.calc_ResponsVal);
		base.Controls.Add(this.xtraTabControl1);
		base.Controls.Add(this.calc_ResponseV);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.chk_Renewal);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.chk_Deleted);
		base.Controls.Add(this.luk_ResponsUnt);
		base.Controls.Add(this.luk_ResponseU);
		base.Controls.Add(this.txt_Descriptio);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.luk_CntrctType);
		base.Controls.Add(this.lb_ItemType);
		base.Controls.Add(this.txt_TmpltName);
		base.Controls.Add(this.lb_CatalogNo);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.FormId = "oContractTemplate";
		base.Name = "frm_ContractTemplate";
		base.ShowIcon = false;
		this.Text = "Contract Templates - Setup";
		((System.ComponentModel.ISupportInitialize)this.txt_TmpltName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CntrctType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Descriptio.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ResponseU.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ResponsUnt.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Deleted.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Renewal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_InclHldays.Properties).EndInit();
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.chk_InclTravel.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InclWork.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_InclParts.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_SunEnd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_SunStrart.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_SatEnd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_SatStart.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_FriEnd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_FriStart.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_ThuEnd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_ThuStart.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_WedEnd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_WedStart.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_TueEnd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_TueStart.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_MonEnd.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.hrEdit_MonStart.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_SunEnabled.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_SatEnabled.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_FriEnabled.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_ThuEnabled.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_WedEnabled.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_TueEnabled.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_MonEnabled.Properties).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.memo_Remark.Properties).EndInit();
		this.xtraTabPage3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.luk_RemindUnit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_ResponseV.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_ResponsVal.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_Duration.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DurationUnit.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_RemindVal.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
