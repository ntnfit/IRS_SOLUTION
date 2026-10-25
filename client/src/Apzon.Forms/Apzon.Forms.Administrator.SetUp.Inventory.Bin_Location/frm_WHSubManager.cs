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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location;

public class frm_WHSubManager : XtraFormEx
{
	public const string FromController = "WHSubManager";

	private int _fldAbs1 = 1;

	private int _fldAbs2 = 1;

	private string _descr1 = "";

	private string _descr2 = "";

	private IContainer components;

	private LabelControl lblMngTask;

	private LabelControl lblWHSubCodes;

	private GridControlEx grdSubCodeSegment;

	private GridViewEx gvSubCodeSegment;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnOk;

	private GridColumnEx colSubCodeSeg;

	private GridColumnEx colFrom;

	private GridColumnEx colTo;

	private LabelControl lblWHSubCodeProp;

	private TextEditEx txtDescr;

	private LookUpEdit cb_MngTask;

	private LookUpEdit cb_WHSubCodes;

	private RepositoryItemLookUpEdit cb_SubCodeSeg;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine lblSubCodeFrom;

	private ButtonEditCFL btnFrom;

	private LabelLine lblSubCodeTo;

	private ButtonEditCFL btnTo;

	private CheckEditEx checkDescr;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOk.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOk.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_WHSubManager()
	{
		InitializeComponent();
	}

	private async Task LoadCb()
	{
		var dataSource = new Dictionary<int, string>
		{
			{ 0, "" },
			{ 1, "Fixed" },
			{ 2, "Alphabetic" },
			{ 3, "Numeric" }
		}.Select((KeyValuePair<int, string> r) => new
		{
			id = r.Key,
			text = r.Value
		});
		cb_SubCodeSeg.DataSource = dataSource;
		var dataSource2 = new Dictionary<int, string>
		{
			{ 0, "Generate Warehouse Sublevel Codes" },
			{ 1, "Update Warehouse Sublevel Codes Properties" },
			{ 2, "Delete Warehouse Sublevel Codes" }
		}.Select((KeyValuePair<int, string> r) => new
		{
			id = r.Key,
			text = r.Value
		});
		cb_MngTask.Properties.DataSource = dataSource2;
		cb_MngTask.EditValue = 0;
		using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "WHSubSetup");
		var dataSource3 = (from s in (await hp.GetAsync("GetAllWHSub")).AsEnumerable()
			select new
			{
				id = Function.ParseInt(s["FldNum"]),
				text = Function.ToString(s["DispName"])
			}).ToList();
		cb_WHSubCodes.Properties.DataSource = dataSource3;
		cb_WHSubCodes.EditValue = 1;
	}

	private string Check()
	{
		foreach (DataRow row in grdSubCodeSegment.DataSourceTable.Rows)
		{
			if (Function.ParseInt(row["Segment"]) != 0 && string.IsNullOrEmpty(Function.ToString(row["From"])) && string.IsNullOrEmpty(Function.ToString(row["To"])))
			{
				return "Specify sublevel code segment";
			}
			if (Function.ParseInt(row["Segment"]) == 2)
			{
				if (!IsAlphabeticOnly(Function.ToString(row["From"])) || !IsAlphabeticOnly(Function.ToString(row["To"])))
				{
					return "Entry not valid; enter alphabetic characters in \"From\" and \"To\" fields";
				}
				if (Function.ToString(row["From"]).Length != Function.ToString(row["To"]).Length)
				{
					return "Invalid entries; character string in \"From\" and \"To\" fields must have the same length";
				}
				if (string.CompareOrdinal(Function.ToString(row["From"]).ToUpper(), Function.ToString(row["To"]).ToUpper()) > 0)
				{
					return "Invalid entries; characters in \"From\" and \"To\" fields must follow alphabetic or alphanumeric order";
				}
			}
			else if (Function.ParseInt(row["Segment"]) == 3)
			{
				if (!IsDigitOnly(Function.ToString(row["From"])) || !IsDigitOnly(Function.ToString(row["To"])))
				{
					return "Entry not valid; enter numeric characters in \"From\" and \"To\" fields";
				}
				if (Function.ParseInt(row["From"]) > Function.ParseInt(row["To"]))
				{
					return "Entry not valid; \"To\" value must be greater than or equal to \"From\" value";
				}
			}
		}
		return "";
	}

	private bool CheckRangeCharacter(string str, char from, char to)
	{
		return str.All((char c) => c >= from && c <= to);
	}

	private bool IsDigitOnly(string str)
	{
		return CheckRangeCharacter(str, '0', '9');
	}

	private bool IsAlphabeticOnly(string str)
	{
		if (!CheckRangeCharacter(str, 'A', 'Z'))
		{
			return CheckRangeCharacter(str, 'a', 'z');
		}
		return true;
	}

	private DataTable NextSegment(DataTable dt, List<string> lst, string type, int fldAbs, string descr)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("SLCode", typeof(string));
		dataTable.Columns.Add("Type", typeof(string));
		dataTable.Columns.Add("FldAbs", typeof(int));
		dataTable.Columns.Add("Descr", typeof(string));
		for (int i = 0; i < dt.Rows.Count; i++)
		{
			foreach (string item in lst)
			{
				dataTable.Rows.Add(dt.Rows[i]["SLCode"]?.ToString() + item, type, fldAbs, descr);
			}
		}
		dataTable.AcceptChanges();
		return dataTable;
	}

	private DataTable GetAllSublevelCode(DataTable dt, string type, int fldAbs, string descr)
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("SLCode");
		dataTable.Rows.Add("");
		for (int i = 0; i < dt.Rows.Count; i++)
		{
			List<string> lst;
			if (Function.ParseInt(dt.Rows[i]["Segment"]) == 1)
			{
				lst = new List<string> { Function.ToString(dt.Rows[i]["From"]).ToUpper() };
			}
			else
			{
				string text = Function.ToString(dt.Rows[i]["From"]).Trim().ToUpper();
				string text2 = Function.ToString(dt.Rows[i]["To"]).Trim().ToUpper();
				if (Function.ParseInt(dt.Rows[i]["Segment"]) == 3)
				{
					if (text.Length > text2.Length)
					{
						text2 = text2.PadLeft(text.Length, '0');
					}
					else if (text2.Length > text.Length)
					{
						text = text.PadLeft(text2.Length, '0');
					}
				}
				lst = CreateListString(text, text2);
			}
			dataTable = NextSegment(dataTable, lst, type, fldAbs, descr);
		}
		return dataTable;
	}

	private List<string> CreateListString(string from, string to)
	{
		List<string> list = new List<string> { from };
		while (string.CompareOrdinal(from, to) < 0)
		{
			IncrementByOne(ref from);
			list.Add(from);
		}
		return list;
	}

	private void IncrementByOne(ref string str)
	{
		char[] array = str.ToCharArray();
		for (int num = str.Length - 1; num >= 0; num--)
		{
			array[num] = (char)(array[num] + 1);
			if ((array[num] <= 'Z' && array[num] >= 'A') || array[num] <= '9')
			{
				str = new string(array);
				break;
			}
			if (array[num] > 'Z')
			{
				array[num] = 'A';
			}
			else
			{
				array[num] = '0';
			}
			if (num == 0)
			{
				str = array[num] + new string(array);
				break;
			}
		}
	}

	private async Task<DataTable> GetRangeSlCode(int fldAbs, string slFrom, string slTo, string descr = null)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "WHSubSetup");
			return await http.GetAsync("GetRangeSlCode", new { fldAbs, slFrom, slTo, descr });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		SimpleButtonEx simpleButtonEx = btnOk;
		LookUpEdit lookUpEdit = cb_MngTask;
		ButtonEditCFL buttonEditCFL = btnFrom;
		ButtonEditCFL buttonEditCFL2 = btnTo;
		bool flag = (cb_WHSubCodes.Enabled = isEnable);
		bool flag3 = (buttonEditCFL2.Enabled = flag);
		bool flag5 = (buttonEditCFL.Enabled = flag3);
		bool enabled = (lookUpEdit.Enabled = flag5);
		simpleButtonEx.Enabled = enabled;
		txtDescr.Enabled = isEnable && (checkDescr.Checked || !checkDescr.Visible);
		gvSubCodeSegment.ReadOnly = !isEnable;
		ControlBox = isEnable;
	}

	private async void frm_WHSubManager_Load(object sender, EventArgs e)
	{
		await LoadCb();
		LoadGridView();
	}

	private void gvSubCodeSegment_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void LoadGridView()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("Segment", typeof(int));
		dataTable.Columns.Add("From", typeof(string));
		dataTable.Columns.Add("To", typeof(string));
		dataTable.Columns["Segment"].DefaultValue = 0;
		for (int i = 0; i < 6; i++)
		{
			DataRow row = dataTable.NewRow();
			dataTable.Rows.Add(row);
		}
		dataTable.AcceptChanges();
		grdSubCodeSegment.DataSource = dataTable;
		grdSubCodeSegment.RefreshDataSource();
	}

	private void cb_MngTask_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(cb_MngTask.EditValue);
			if (num == 0)
			{
				lblWHSubCodes.Text = "Warehouse Sublevel Codes";
				cb_WHSubCodes.EditValue = _fldAbs1;
				lblSubCodeFrom.Visible = false;
				btnFrom.Visible = false;
				lblSubCodeTo.Visible = false;
				btnTo.Visible = false;
				grdSubCodeSegment.Visible = true;
				lblWHSubCodeProp.Text = "Warehouse Sublevel Code Properties";
				txtDescr.Text = _descr1;
				checkDescr.Visible = false;
				txtDescr.Enabled = true;
				return;
			}
			lblWHSubCodes.Text = "Select Warehouse Sublevel Codes";
			cb_WHSubCodes.EditValue = _fldAbs2;
			lblSubCodeFrom.Visible = true;
			btnFrom.Visible = true;
			lblSubCodeTo.Visible = true;
			btnTo.Visible = true;
			grdSubCodeSegment.Visible = false;
			checkDescr.Visible = true;
			if (!checkDescr.Checked)
			{
				txtDescr.Enabled = false;
			}
			lblWHSubCodeProp.Text = ((num == 1) ? "Specify Properties to Update for Selected Warehouse Sublevel Codes" : "Expanded Selection Criteria");
			txtDescr.Text = _descr2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void gvSubCodeSegment_ShowingEditor(object sender, CancelEventArgs e)
	{
		if (gvSubCodeSegment.FocusedColumn.FieldName == "Segment")
		{
			return;
		}
		switch (Function.ParseInt(gvSubCodeSegment.GetFocusedRowCellValue("Segment")))
		{
		case 0:
			e.Cancel = true;
			break;
		case 1:
			if (gvSubCodeSegment.FocusedColumn.FieldName == "To")
			{
				e.Cancel = true;
			}
			break;
		}
	}

	private async void btnOk_Click(object sender, EventArgs e)
	{
		_ = 3;
		try
		{
			WinCommons._OpenProcessing();
			DataTable dtPre = new DataTable();
			dtPre.Columns.Add("AbsEntry");
			dtPre.Columns.Add("FldAbs");
			dtPre.Columns.Add("SLCode");
			dtPre.Columns.Add("Descr");
			dtPre.Columns.Add("Type");
			dtPre.Columns.Add("Error");
			dtPre.Columns["FldAbs"].DefaultValue = Function.ParseInt(cb_WHSubCodes.EditValue);
			switch (Function.ParseInt(cb_MngTask.EditValue))
			{
			case 0:
			{
				string text = Check();
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					return;
				}
				EnableDisableControl(isEnable: false);
				DataTable dataSourceTable = grdSubCodeSegment.DataSourceTable;
				dtPre.Columns["Descr"].DefaultValue = txtDescr.Text.Trim();
				dtPre.Columns["Type"].DefaultValue = "A";
				DataTable allSublevelCode = GetAllSublevelCode(dataSourceTable.Select("Segment <> 0").CopyToDataTable(), "A", Function.ParseInt(cb_WHSubCodes.EditValue), txtDescr.Text.Trim());
				allSublevelCode.Columns.Add("AbsEntry");
				allSublevelCode.Columns.Add("Error");
				using (HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "WHSubSetup"))
				{
					dtPre = await hp.PostAsync("CheckCodeExist", allSublevelCode);
				}
				break;
			}
			case 1:
			{
				dtPre.Columns["Type"].DefaultValue = "U";
				DataTable dataTable2 = await GetRangeSlCode(Function.ParseInt(cb_WHSubCodes.EditValue), btnFrom.Text, btnTo.Text);
				if (!dataTable2.IsNotNull())
				{
					WinCommons.Info(Enums.MessageCode.DataIsNotAvailable, "");
					return;
				}
				EnableDisableControl(isEnable: false);
				foreach (DataRow row in dataTable2.Rows)
				{
					DataRow dataRow2 = dtPre.NewRow();
					dataRow2["SLCode"] = Function.ToString(row["SLCode"]);
					dataRow2["AbsEntry"] = Function.ParseInt(row["AbsEntry"]);
					if (checkDescr.Checked)
					{
						dataRow2["Descr"] = txtDescr.Text;
					}
					else
					{
						dataRow2["Descr"] = Function.ToString(row["Descr"]);
					}
					dtPre.Rows.Add(dataRow2);
				}
				dtPre.AcceptChanges();
				break;
			}
			case 2:
			{
				DataTable dataTable = await GetRangeSlCode(descr: (!checkDescr.Checked) ? null : txtDescr.Text.Trim(), fldAbs: Function.ParseInt(cb_WHSubCodes.EditValue), slFrom: btnFrom.Text, slTo: btnTo.Text);
				if (!dataTable.IsNotNull())
				{
					WinCommons.Info(Enums.MessageCode.DataIsNotAvailable, "");
					return;
				}
				EnableDisableControl(isEnable: false);
				dataTable.Columns.Add("Error");
				dataTable.Columns.Add("Type");
				dataTable.Columns["Type"].DefaultValue = "D";
				using (HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "WHSubSetup"))
				{
					dtPre = await hp.PostAsync("IsUsedInBinLocCodes", dataTable);
				}
				break;
			}
			}
			EnableDisableControl(isEnable: true);
			if (dtPre.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.None, "Cannot get data to process");
				return;
			}
			frm_WHSubSetupPreview frm_WHSubSetupPreview2 = new frm_WHSubSetupPreview();
			frm_WHSubSetupPreview2.DtSource = dtPre;
			frm_WHSubSetupPreview2.WhSubName = cb_WHSubCodes.Text;
			frm_WHSubSetupPreview2.ShowDialog(this);
		}
		catch (Exception ex)
		{
			WinCommons.Error(Enums.MessageCode.None, "Cannot get data to process");
			EnableDisableControl(isEnable: true);
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void txtDescr_EditValueChanged(object sender, EventArgs e)
	{
		if (Function.ParseInt(cb_MngTask.EditValue) == 0)
		{
			_descr1 = txtDescr.Text;
		}
		else
		{
			_descr2 = txtDescr.Text;
		}
	}

	private void cb_WHSubCodes_Properties_EditValueChanged(object sender, EventArgs e)
	{
		if (Function.ParseInt(cb_MngTask.EditValue) == 0)
		{
			_fldAbs1 = Function.ParseInt(cb_WHSubCodes.EditValue);
			return;
		}
		_fldAbs2 = Function.ParseInt(cb_WHSubCodes.EditValue);
		ButtonEditCFL buttonEditCFL = btnFrom;
		string text = (btnTo.Text = string.Empty);
		buttonEditCFL.Text = text;
	}

	private void checkDescr_CheckedChanged(object sender, EventArgs e)
	{
		if (checkDescr.Checked)
		{
			txtDescr.Enabled = true;
			return;
		}
		_descr2 = string.Empty;
		txtDescr.Text = string.Empty;
		txtDescr.Enabled = false;
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		if (btnOk.Enabled || XtraMessageBox.Show("System is generating data to process. Do you want to exit?", "System Message", MessageBoxButtons.YesNo) != DialogResult.No)
		{
			Close();
		}
	}

	private void btnFrom_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			List<Common.ParameterValue> lstpr = new List<Common.ParameterValue>
			{
				new Common.ParameterValue("fldAbs", Function.ParseInt(cb_WHSubCodes.EditValue))
			};
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.WarehouseSublevel, false, "", "", Enums.ItemObjectType.NoItemType, lstpr, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				btnFrom.Text = Function.ToString(frm_CFL2.ChooseItem["SLCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Info(Enums.MessageCode.None, ex.Message);
		}
	}

	private void btnTo_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			List<Common.ParameterValue> lstpr = new List<Common.ParameterValue>
			{
				new Common.ParameterValue("fldAbs", Function.ParseInt(cb_WHSubCodes.EditValue))
			};
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.WarehouseSublevel, false, "", "", Enums.ItemObjectType.NoItemType, lstpr, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				btnTo.Text = Function.ToString(frm_CFL2.ChooseItem["SLCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Info(Enums.MessageCode.None, ex.Message);
		}
	}

	private void cb_SubCodeSeg_EditValueChanged(object sender, EventArgs e)
	{
		gvSubCodeSegment.SetFocusedRowCellValue("From", null);
		gvSubCodeSegment.SetFocusedRowCellValue("To", null);
		if (sender is LookUpEdit lookUpEdit && Function.ParseInt(lookUpEdit.EditValue) != 0)
		{
			gvSubCodeSegment.FocusedColumn = colFrom;
			gvSubCodeSegment.ShowEditor();
		}
		else
		{
			gvSubCodeSegment.CloseEditor();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.Inventory.Bin_Location.frm_WHSubManager));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.lblMngTask = new DevExpress.XtraEditors.LabelControl();
		this.lblWHSubCodes = new DevExpress.XtraEditors.LabelControl();
		this.grdSubCodeSegment = new Apzon.Controls.Grid.GridControlEx();
		this.gvSubCodeSegment = new Apzon.Controls.Grid.GridViewEx();
		this.colSubCodeSeg = new Apzon.Controls.Grid.GridColumnEx();
		this.cb_SubCodeSeg = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.colFrom = new Apzon.Controls.Grid.GridColumnEx();
		this.colTo = new Apzon.Controls.Grid.GridColumnEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lblWHSubCodeProp = new DevExpress.XtraEditors.LabelControl();
		this.txtDescr = new Apzon.Controls.Editor.TextEditEx();
		this.cb_MngTask = new DevExpress.XtraEditors.LookUpEdit();
		this.cb_WHSubCodes = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.lblSubCodeFrom = new Apzon.Controls.Controls.LabelLine();
		this.btnFrom = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.lblSubCodeTo = new Apzon.Controls.Controls.LabelLine();
		this.btnTo = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.checkDescr = new Apzon.Controls.Editor.CheckEditEx();
		((System.ComponentModel.ISupportInitialize)this.grdSubCodeSegment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvSubCodeSegment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_SubCodeSeg).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDescr.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_MngTask.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_WHSubCodes.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btnFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btnTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.checkDescr.Properties).BeginInit();
		base.SuspendLayout();
		this.lblMngTask.Location = new System.Drawing.Point(13, 13);
		this.lblMngTask.Name = "lblMngTask";
		this.lblMngTask.Size = new System.Drawing.Size(87, 13);
		this.lblMngTask.TabIndex = 0;
		this.lblMngTask.Text = "Management Task";
		this.lblWHSubCodes.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline);
		this.lblWHSubCodes.Appearance.Options.UseFont = true;
		this.lblWHSubCodes.Location = new System.Drawing.Point(12, 41);
		this.lblWHSubCodes.Name = "lblWHSubCodes";
		this.lblWHSubCodes.Size = new System.Drawing.Size(131, 13);
		this.lblWHSubCodes.TabIndex = 2;
		this.lblWHSubCodes.Text = "Warehouse Sublevel Codes";
		this.grdSubCodeSegment.AllowPasteExcel = true;
		this.grdSubCodeSegment.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdSubCodeSegment.ClipboardData = null;
		this.grdSubCodeSegment.Location = new System.Drawing.Point(0, 86);
		this.grdSubCodeSegment.MainView = this.gvSubCodeSegment;
		this.grdSubCodeSegment.Name = "grdSubCodeSegment";
		this.grdSubCodeSegment.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.cb_SubCodeSeg });
		this.grdSubCodeSegment.Size = new System.Drawing.Size(689, 245);
		this.grdSubCodeSegment.TabIndex = 5;
		this.grdSubCodeSegment.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvSubCodeSegment });
		this.gvSubCodeSegment.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvSubCodeSegment.ColumnKey = null;
		this.gvSubCodeSegment.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.colSubCodeSeg, this.colFrom, this.colTo });
		this.gvSubCodeSegment.DisableFormSetting = false;
		this.gvSubCodeSegment.DisplayException = false;
		this.gvSubCodeSegment.EditColumn = null;
		this.gvSubCodeSegment.FieldCurrency = null;
		this.gvSubCodeSegment.FieldRate = null;
		this.gvSubCodeSegment.FormId = null;
		this.gvSubCodeSegment.GridControl = this.grdSubCodeSegment;
		this.gvSubCodeSegment.IgnoreFilter = false;
		this.gvSubCodeSegment.IndexEditRow = -1;
		this.gvSubCodeSegment.IndicatorWidth = 40;
		this.gvSubCodeSegment.IsApzTable = false;
		this.gvSubCodeSegment.IsInsertNewRow = false;
		this.gvSubCodeSegment.IsLoadingData = false;
		this.gvSubCodeSegment.IsMappingContextAction = false;
		this.gvSubCodeSegment.ItemId = null;
		this.gvSubCodeSegment.LstColumnAllowBatch = null;
		this.gvSubCodeSegment.LstColumnNotAllowBatch = null;
		this.gvSubCodeSegment.LstColumnNotEditable = null;
		this.gvSubCodeSegment.LstColumNotSetting = null;
		this.gvSubCodeSegment.Name = "gvSubCodeSegment";
		this.gvSubCodeSegment.OptionsCustomization.AllowSort = false;
		this.gvSubCodeSegment.OptionsView.ShowGroupPanel = false;
		this.gvSubCodeSegment.UseBatchMod = false;
		this.gvSubCodeSegment.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvSubCodeSegment_CustomDrawRowIndicator);
		this.gvSubCodeSegment.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvSubCodeSegment_ShowingEditor);
		this.colSubCodeSeg.Caption = "Sublevel Code Segment";
		this.colSubCodeSeg.ColumnEdit = this.cb_SubCodeSeg;
		this.colSubCodeSeg.CurrencyDateField = null;
		this.colSubCodeSeg.CurrencyField = null;
		this.colSubCodeSeg.CurrencyFixed = null;
		this.colSubCodeSeg.CurrencyFixedRate = 0.0;
		this.colSubCodeSeg.CurrencyRateField = null;
		this.colSubCodeSeg.FieldName = "Segment";
		this.colSubCodeSeg.Name = "colSubCodeSeg";
		this.colSubCodeSeg.OptionsColumn.AllowMove = false;
		this.colSubCodeSeg.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colSubCodeSeg.PrimaryKey = true;
		this.colSubCodeSeg.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.colSubCodeSeg.UnitField = null;
		this.colSubCodeSeg.Visible = true;
		this.colSubCodeSeg.VisibleIndex = 0;
		this.colSubCodeSeg.Width = 264;
		this.cb_SubCodeSeg.AutoHeight = false;
		this.cb_SubCodeSeg.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_SubCodeSeg.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("id", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("text", "Text")
		});
		this.cb_SubCodeSeg.DisplayMember = "text";
		this.cb_SubCodeSeg.Name = "cb_SubCodeSeg";
		this.cb_SubCodeSeg.NullText = "";
		this.cb_SubCodeSeg.ShowFooter = false;
		this.cb_SubCodeSeg.ShowHeader = false;
		this.cb_SubCodeSeg.ValueMember = "id";
		this.cb_SubCodeSeg.EditValueChanged += new System.EventHandler(this.cb_SubCodeSeg_EditValueChanged);
		this.colFrom.Caption = "From";
		this.colFrom.CurrencyDateField = null;
		this.colFrom.CurrencyField = null;
		this.colFrom.CurrencyFixed = null;
		this.colFrom.CurrencyFixedRate = 0.0;
		this.colFrom.CurrencyRateField = null;
		this.colFrom.FieldName = "From";
		this.colFrom.Name = "colFrom";
		this.colFrom.OptionsColumn.AllowMove = false;
		this.colFrom.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colFrom.PrimaryKey = false;
		this.colFrom.UnitField = null;
		this.colFrom.Visible = true;
		this.colFrom.VisibleIndex = 1;
		this.colTo.Caption = "To";
		this.colTo.CurrencyDateField = null;
		this.colTo.CurrencyField = null;
		this.colTo.CurrencyFixed = null;
		this.colTo.CurrencyFixedRate = 0.0;
		this.colTo.CurrencyRateField = null;
		this.colTo.FieldName = "To";
		this.colTo.Name = "colTo";
		this.colTo.OptionsColumn.AllowMove = false;
		this.colTo.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.colTo.PrimaryKey = false;
		this.colTo.UnitField = null;
		this.colTo.Visible = true;
		this.colTo.VisibleIndex = 2;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(96, 404);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 7;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.EditControl = null;
		this.btnOk.EditValue = "OK";
		this.btnOk.FieldName = null;
		this.btnOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOk.ImageOptions.Image");
		this.btnOk.IsSearchControl = false;
		this.btnOk.Location = new System.Drawing.Point(15, 404);
		this.btnOk.Name = "btnOk";
		this.btnOk.ReadOnly = false;
		this.btnOk.SettingInVisible = false;
		this.btnOk.SettingReadonly = false;
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 6;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.lblWHSubCodeProp.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lblWHSubCodeProp.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Underline);
		this.lblWHSubCodeProp.Appearance.Options.UseFont = true;
		this.lblWHSubCodeProp.Location = new System.Drawing.Point(12, 337);
		this.lblWHSubCodeProp.Name = "lblWHSubCodeProp";
		this.lblWHSubCodeProp.Size = new System.Drawing.Size(178, 13);
		this.lblWHSubCodeProp.TabIndex = 8;
		this.lblWHSubCodeProp.Text = "Warehouse Sublevel Code Properties";
		this.txtDescr.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.txtDescr.EditControl = null;
		this.txtDescr.FieldName = null;
		this.txtDescr.IsSearchControl = false;
		this.txtDescr.Location = new System.Drawing.Point(128, 356);
		this.txtDescr.Name = "txtDescr";
		this.txtDescr.SettingInVisible = false;
		this.txtDescr.SettingReadonly = false;
		this.txtDescr.Size = new System.Drawing.Size(314, 20);
		this.txtDescr.TabIndex = 10;
		this.txtDescr.EditValueChanged += new System.EventHandler(this.txtDescr_EditValueChanged);
		this.cb_MngTask.Location = new System.Drawing.Point(118, 10);
		this.cb_MngTask.Name = "cb_MngTask";
		this.cb_MngTask.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_MngTask.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("id", "Task ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("text", "Task Content")
		});
		this.cb_MngTask.Properties.DisplayMember = "text";
		this.cb_MngTask.Properties.NullText = "";
		this.cb_MngTask.Properties.PopupSizeable = false;
		this.cb_MngTask.Properties.ShowFooter = false;
		this.cb_MngTask.Properties.ShowHeader = false;
		this.cb_MngTask.Properties.ValueMember = "id";
		this.cb_MngTask.Size = new System.Drawing.Size(314, 20);
		this.cb_MngTask.TabIndex = 1;
		this.cb_MngTask.EditValueChanged += new System.EventHandler(this.cb_MngTask_EditValueChanged);
		this.cb_WHSubCodes.Location = new System.Drawing.Point(211, 60);
		this.cb_WHSubCodes.Name = "cb_WHSubCodes";
		this.cb_WHSubCodes.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_WHSubCodes.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("id", "WH Sublevel ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("text", "WHSublevel Name")
		});
		this.cb_WHSubCodes.Properties.DisplayMember = "text";
		this.cb_WHSubCodes.Properties.NullText = "";
		this.cb_WHSubCodes.Properties.PopupSizeable = false;
		this.cb_WHSubCodes.Properties.ShowFooter = false;
		this.cb_WHSubCodes.Properties.ShowHeader = false;
		this.cb_WHSubCodes.Properties.ValueMember = "id";
		this.cb_WHSubCodes.Properties.EditValueChanged += new System.EventHandler(this.cb_WHSubCodes_Properties_EditValueChanged);
		this.cb_WHSubCodes.Size = new System.Drawing.Size(276, 20);
		this.cb_WHSubCodes.TabIndex = 4;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Warehouse Sublevel";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 60);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(193, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 11;
		this.labelLine1.Utext = "Warehouse Sublevel";
		this.labelLine2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Description";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(35, 356);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(87, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 12;
		this.labelLine2.Utext = "Description";
		this.lblSubCodeFrom.EditControl = null;
		this.lblSubCodeFrom.EditValue = "Sublevel Code From";
		this.lblSubCodeFrom.FieldName = null;
		this.lblSubCodeFrom.HideWhenEmpty = false;
		this.lblSubCodeFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblSubCodeFrom.Location = new System.Drawing.Point(12, 86);
		this.lblSubCodeFrom.Name = "lblSubCodeFrom";
		this.lblSubCodeFrom.ReadOnly = false;
		this.lblSubCodeFrom.SettingInVisible = false;
		this.lblSubCodeFrom.SettingReadonly = false;
		this.lblSubCodeFrom.ShowUnderLine = true;
		this.lblSubCodeFrom.Size = new System.Drawing.Size(193, 20);
		this.lblSubCodeFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblSubCodeFrom.TabIndex = 13;
		this.lblSubCodeFrom.Utext = "Sublevel Code From";
		this.btnFrom.EditControl = null;
		this.btnFrom.FieldName = null;
		this.btnFrom.IsSearchControl = false;
		this.btnFrom.Location = new System.Drawing.Point(211, 86);
		this.btnFrom.Name = "btnFrom";
		this.btnFrom.Properties.AutoLoadDataSource = true;
		this.btnFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btnFrom.Properties.ContainerControl = null;
		this.btnFrom.Properties.DisplayMember = "";
		this.btnFrom.Properties.DisplayMember2 = "";
		this.btnFrom.Properties.LoadDataSourceWhenEnter = false;
		this.btnFrom.Properties.SeparateCharacterForMulti = ';';
		this.btnFrom.Properties.SeparateString = ", ";
		this.btnFrom.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.btnFrom.Properties.UsingAutoCompleteBox = false;
		this.btnFrom.Properties.UsingDataSource = true;
		this.btnFrom.Properties.ValidateWhenNoDataSource = false;
		this.btnFrom.Properties.ValueMember = "";
		this.btnFrom.SettingInVisible = false;
		this.btnFrom.SettingReadonly = false;
		this.btnFrom.Size = new System.Drawing.Size(100, 20);
		this.btnFrom.TabIndex = 14;
		this.btnFrom.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnFrom_ButtonClick);
		this.lblSubCodeTo.EditControl = null;
		this.lblSubCodeTo.EditValue = "To";
		this.lblSubCodeTo.FieldName = null;
		this.lblSubCodeTo.HideWhenEmpty = false;
		this.lblSubCodeTo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblSubCodeTo.Location = new System.Drawing.Point(359, 86);
		this.lblSubCodeTo.Name = "lblSubCodeTo";
		this.lblSubCodeTo.ReadOnly = false;
		this.lblSubCodeTo.SettingInVisible = false;
		this.lblSubCodeTo.SettingReadonly = false;
		this.lblSubCodeTo.ShowUnderLine = true;
		this.lblSubCodeTo.Size = new System.Drawing.Size(39, 20);
		this.lblSubCodeTo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblSubCodeTo.TabIndex = 15;
		this.lblSubCodeTo.Utext = "To";
		this.btnTo.EditControl = null;
		this.btnTo.FieldName = null;
		this.btnTo.IsSearchControl = false;
		this.btnTo.Location = new System.Drawing.Point(379, 86);
		this.btnTo.Name = "btnTo";
		this.btnTo.Properties.AutoLoadDataSource = true;
		this.btnTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btnTo.Properties.ContainerControl = null;
		this.btnTo.Properties.DisplayMember = "";
		this.btnTo.Properties.DisplayMember2 = "";
		this.btnTo.Properties.LoadDataSourceWhenEnter = false;
		this.btnTo.Properties.SeparateCharacterForMulti = ';';
		this.btnTo.Properties.SeparateString = ", ";
		this.btnTo.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.btnTo.Properties.UsingAutoCompleteBox = false;
		this.btnTo.Properties.UsingDataSource = true;
		this.btnTo.Properties.ValidateWhenNoDataSource = false;
		this.btnTo.Properties.ValueMember = "";
		this.btnTo.SettingInVisible = false;
		this.btnTo.SettingReadonly = false;
		this.btnTo.Size = new System.Drawing.Size(100, 20);
		this.btnTo.TabIndex = 16;
		this.btnTo.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btnTo_ButtonClick);
		this.checkDescr.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.checkDescr.EditControl = null;
		this.checkDescr.FieldName = null;
		this.checkDescr.IsSearchControl = false;
		this.checkDescr.Location = new System.Drawing.Point(13, 357);
		this.checkDescr.Name = "checkDescr";
		this.checkDescr.Properties.Caption = "";
		this.checkDescr.SettingInVisible = false;
		this.checkDescr.SettingReadonly = false;
		this.checkDescr.Size = new System.Drawing.Size(16, 20);
		this.checkDescr.TabIndex = 17;
		this.checkDescr.CheckedChanged += new System.EventHandler(this.checkDescr_CheckedChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(689, 441);
		base.Controls.Add(this.checkDescr);
		base.Controls.Add(this.btnTo);
		base.Controls.Add(this.lblSubCodeTo);
		base.Controls.Add(this.btnFrom);
		base.Controls.Add(this.lblSubCodeFrom);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.lblWHSubCodeProp);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.grdSubCodeSegment);
		base.Controls.Add(this.lblWHSubCodes);
		base.Controls.Add(this.lblMngTask);
		base.Controls.Add(this.txtDescr);
		base.Controls.Add(this.cb_MngTask);
		base.Controls.Add(this.cb_WHSubCodes);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WHSubManager";
		this.Text = "Warehouse Sublevel Code Management";
		base.Load += new System.EventHandler(this.frm_WHSubManager_Load);
		((System.ComponentModel.ISupportInitialize)this.grdSubCodeSegment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvSubCodeSegment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_SubCodeSeg).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDescr.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_MngTask.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_WHSubCodes.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btnFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btnTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.checkDescr.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
