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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.LookAndFeel;
using DevExpress.Utils;
using DevExpress.Utils.Behaviors;
using DevExpress.XtraBars;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Packing_Management;

public class frm_PackingOrder : XtraFormEx
{
	private bool _isAddNew;

	private bool _isSavingdata;

	private bool _isLoadData;

	private bool _GB;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private LabelLine label_SoLenh;

	private LabelLine label_LoaiLenh;

	private LabelLine label_LoaiThung;

	private LabelLine label_TongSoThung;

	private LabelLine label_TrangThai;

	private LabelLine label_ChuyenDongGoi;

	private TextEdit textEdit_ChuyenDongGoi;

	private LookUpEditEx lookUpEdit_TrangThai;

	private LabelControlEx labelControlEx1;

	private ButtonEditCFL bte_PO;

	private BehaviorManager behaviorManager1;

	private SimpleButton simpleBtn_Cancel;

	private SimpleButton simpleBtn_OK;

	private LabelControl label_ChiTiet;

	private TextEditEx textEdit_TongSoThung;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem refresh_ToolStripMenuItem;

	private LookUpEditEx lookUpEdit_USERID;

	private LabelLine labelLine1;

	private LookUpEditEx lookUpEdit_Whs;

	private LabelLine labelLine2;

	private LookUpEditEx lookUpEdit_LoaiLenh;

	private DefaultLookAndFeel defaultLookAndFeel1;

	private TextEditEx textEdit_Amount;

	private TextEditEx textEdit_Remarks;

	private LabelControl labelControl1;

	private LookUpEditEx lookUpEdit_DV;

	private LookUpEditEx lookUpEdit_TypeCode;

	private LabelControl labelCtl_Loai;

	private LabelControl labelCtrl_Remark;

	private LabelControl labelCtl_SoLuong;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private DropDownButton drBtn_CopyFrom;

	private BarManager barManager1;

	private BarDockControl barDockControlTop;

	private BarDockControl barDockControlBottom;

	private BarDockControl barDockControlLeft;

	private BarDockControl barDockControlRight;

	private BarButtonItem PackingOrder;

	private BarButtonItem ProductionOrder;

	private PopupMenu popupMenu;

	private ButtonEditCFL lookUpEdit_ChildType;

	public int POCode { get; set; }

	public frm_PackingOrder()
	{
		InitializeComponent();
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			simpleBtn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.View:
			simpleBtn_OK.Text = "OK";
			break;
		case Enums.FormViewModeType.Update:
			simpleBtn_OK.Text = "Update";
			break;
		case Enums.FormViewModeType.Add:
			simpleBtn_OK.Text = "Add";
			break;
		}
	}

	private async void frm_PackingOrder_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			textEdit_Amount.Text = "0";
			REFRESH();
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			List<Common.ParameterValue> list2 = new List<Common.ParameterValue>();
			List<Common.ParameterValue> list3 = new List<Common.ParameterValue>();
			list.Add(new Common.ParameterValue("Wrap", "w"));
			list.Add(new Common.ParameterValue("Unwrap", "u"));
			lookUpEdit_LoaiLenh.Properties.DataSource = list;
			lookUpEdit_LoaiLenh.Properties.DisplayMember = "Name";
			lookUpEdit_LoaiLenh.Properties.ValueMember = "Value";
			lookUpEdit_LoaiLenh.Properties.ShowHeader = false;
			list3.Add(new Common.ParameterValue("Item", "i"));
			list3.Add(new Common.ParameterValue("Case", "c"));
			lookUpEdit_DV.Properties.DisplayMember = "Name";
			lookUpEdit_DV.Properties.ValueMember = "Value";
			lookUpEdit_DV.ItemIndex = 0;
			lookUpEdit_DV.Properties.ShowHeader = false;
			lookUpEdit_DV.Properties.DataSource = list3;
			list2.Add(new Common.ParameterValue("Plan", "p"));
			list2.Add(new Common.ParameterValue("Close", "c"));
			list2.Add(new Common.ParameterValue("Cancel", "x"));
			lookUpEdit_TrangThai.Properties.DisplayMember = "Name";
			lookUpEdit_TrangThai.Properties.ValueMember = "Value";
			lookUpEdit_TrangThai.ItemIndex = 0;
			lookUpEdit_TrangThai.Properties.ShowHeader = false;
			lookUpEdit_TrangThai.Properties.DataSource = list2;
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PackingOrder"))
			{
				DataTable dataSource = await http.GetAsync("GetType");
				lookUpEdit_TypeCode.Properties.DataSource = dataSource;
				lookUpEdit_TypeCode.Properties.DisplayMember = "TypeName";
				lookUpEdit_TypeCode.Properties.ValueMember = "TypeCode";
				lookUpEdit_TypeCode.ItemIndex = 0;
				DataTable dataSource2 = await http.GetAsync("GetWhs");
				lookUpEdit_Whs.Properties.DataSource = dataSource2;
				lookUpEdit_Whs.Properties.DisplayMember = "WhsName";
				lookUpEdit_Whs.Properties.ShowHeader = false;
			}
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Traceability"))
			{
				DataTable dataSource3 = await http.GetAsync("GetUser");
				lookUpEdit_USERID.Properties.DataSource = dataSource3;
				lookUpEdit_USERID.Properties.DisplayMember = "U_NAME";
				lookUpEdit_USERID.Properties.ValueMember = "USERID";
				lookUpEdit_USERID.Properties.ShowHeader = false;
			}
			RepositoryItemButtonEditCFL properties = bte_PO.Properties;
			properties.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					_GB = true;
					drBtn_CopyFrom.Visible = false;
					bte_PO.EditValue = chooseItem["POCode"];
					lookUpEdit_LoaiLenh.EditValue = chooseItem["TypePO"];
					lookUpEdit_ChildType.EditValue = chooseItem["TypeCode"];
					lookUpEdit_TrangThai.EditValue = chooseItem["Status"];
					lookUpEdit_TypeCode.EditValue = chooseItem["ChildType"];
					lookUpEdit_Whs.EditValue = chooseItem["WhsCode"];
					textEdit_TongSoThung.EditValue = chooseItem["AmountItem"];
					lookUpEdit_USERID.EditValue = chooseItem["USERID"];
					textEdit_Amount.EditValue = chooseItem["Amountbundle"];
					lookUpEdit_DV.EditValue = chooseItem["Unit"];
					textEdit_Remarks.EditValue = chooseItem["Remark"];
					Refresh();
					FormMode = Enums.FormViewModeType.Busy;
				}
				_isLoadData = true;
				_GB = false;
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
			}));
			RepositoryItemButtonEditCFL properties2 = lookUpEdit_ChildType.Properties;
			properties2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties2.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					if (lookUpEdit_DV.Text == "Case")
					{
						lookUpEdit_ChildType.EditValue = chooseItem["TypeCode"];
						Refresh();
						FormMode = Enums.FormViewModeType.Busy;
					}
					if (lookUpEdit_DV.Text == "Item")
					{
						lookUpEdit_ChildType.EditValue = chooseItem["ItemCode"];
						Refresh();
						FormMode = Enums.FormViewModeType.Busy;
					}
					FormMode = Enums.FormViewModeType.Add;
				}
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void LoadDataDoc()
	{
		_ = 1;
		try
		{
			_isLoadData = false;
			drBtn_CopyFrom.Visible = true;
			FormMode = Enums.FormViewModeType.Add;
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PackingOrder");
			try
			{
				DataSource = await http.GetAsync("GetPOCode");
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
			await LoadData(Enums.NavigateType.Current);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	private async void AddNew()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			if (!_isAddNew)
			{
				REFRESH();
			}
			_isLoadData = false;
			_isSavingdata = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			lookUpEdit_ChildType.Properties.TypeCFL = Enums.TypeCFL.GetTC;
			drBtn_CopyFrom.Visible = true;
			Refresh();
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PackingOrder"))
			{
				DataSource = await http.GetAsync("GetPOCode");
			}
			int userID = GlobalData.UserID;
			new DataTable();
			DataTable dataTable = (DataTable)DataSource;
			dataTable.Rows[0]["TypeCode"] = "T1";
			dataTable.Rows[0]["Amountbundle"] = 0;
			dataTable.Rows[0]["USERID"] = userID;
			dataTable.Rows[0]["Status"] = "p";
			dataTable.Rows[0]["TypePO"] = "w";
			dataTable.Rows[0]["AmountItem"] = 0;
			dataTable.Rows[0]["ChildType"] = "T1";
			dataTable.Rows[0]["WhsCode"] = "General Warehouse";
			dataTable.Rows[0]["Unit"] = "c";
			dataTable.Rows[0]["Remark"] = "";
			dataTable.Rows[0]["BaseEntry"] = 0;
			dataTable.Rows[0]["LineNum"] = 0;
			dataTable.AcceptChanges();
			DataSource = dataTable;
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData(Enums.NavigateType type)
	{
		object pOCode = (DataSource as DataTable).Rows[0]["POCode"];
		using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "PackingOrder"))
		{
			try
			{
				DataSource = await http.GetAsync("POGetDataDoc", new
				{
					POCode = pOCode,
					type = type
				});
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		}
		_isLoadData = false;
	}

	public async Task REFRESH_DATA()
	{
		drBtn_CopyFrom.Visible = false;
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		drBtn_CopyFrom.Visible = false;
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		drBtn_CopyFrom.Visible = false;
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		drBtn_CopyFrom.Visible = false;
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Next);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		drBtn_CopyFrom.Visible = false;
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.First);
			POCode = 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task<bool> Savedt()
	{
		_ = 1;
		try
		{
			int num = Function.ParseInt(textEdit_Amount.EditValue);
			if (lookUpEdit_ChildType.EditValue == lookUpEdit_TypeCode.EditValue)
			{
				XtraMessageBox.Show("Case and Item must be different");
				return false;
			}
			if (num == 0)
			{
				XtraMessageBox.Show("Amount must fill");
				return false;
			}
			_isSavingdata = true;
			DataTable dataTable = DataSource as DataTable;
			dataTable.TableName = "dtDoc";
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable);
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "PackingOrder");
			if (FormMode == Enums.FormViewModeType.Add)
			{
				HttpResult httpResult = await http.PostAsync("SaveDocDataSet", dataSet);
				if (httpResult.Message == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					REFRESH();
					return true;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return false;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				HttpResult httpResult2 = await http.PostAsync("SaveDocDataSetUpdate", dataSet);
				if (httpResult2.Message == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					return true;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
				return false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
		return true;
	}

	private async void simpleBtn_OK_Click_1(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBox.Show(Resources.xtraMsg_Save, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				Savedt();
				if (_isSavingdata)
				{
					AddNew();
				}
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				Savedt();
				if (_isSavingdata)
				{
					LoadDataDoc();
					FormMode = Enums.FormViewModeType.View;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void simpleBtn_Cancel_Click_1(object sender, EventArgs e)
	{
		Close();
	}

	private void addNew_ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNew();
	}

	private void refresh_ToolStripMenuItem_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private async void frm_PackingOrder_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update || (FormMode == Enums.FormViewModeType.Add && _isAddNew))
		{
			if (XtraMessageBox.Show("Data were reset at all, do you want to save before close form?", "System message", MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				if (!(await Savedt()))
				{
					e.Cancel = true;
				}
				else
				{
					e.Cancel = false;
				}
			}
		}
		else
		{
			e.Cancel = false;
		}
	}

	private void frm_PackingOrder_OnControlValueChanging(object sender, ControlChangingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && !_isLoadData)
			{
				FormMode = Enums.FormViewModeType.Update;
				return;
			}
			_isLoadData = false;
			_isAddNew = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_PackingOrder_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		if (lookUpEdit_TrangThai.Text == "Close" || lookUpEdit_TrangThai.Text == "Cancel")
		{
			lookUpEdit_LoaiLenh.Properties.ReadOnly = true;
			lookUpEdit_ChildType.Properties.ReadOnly = true;
			lookUpEdit_Whs.Properties.ReadOnly = true;
			lookUpEdit_USERID.Properties.ReadOnly = true;
			lookUpEdit_TypeCode.Properties.ReadOnly = true;
			lookUpEdit_TrangThai.Properties.ReadOnly = true;
			textEdit_Amount.Properties.ReadOnly = true;
			lookUpEdit_DV.Properties.ReadOnly = true;
			textEdit_Remarks.Properties.ReadOnly = true;
		}
		else
		{
			lookUpEdit_LoaiLenh.Properties.ReadOnly = false;
			lookUpEdit_ChildType.Properties.ReadOnly = false;
			lookUpEdit_Whs.Properties.ReadOnly = false;
			lookUpEdit_USERID.Properties.ReadOnly = false;
			lookUpEdit_TypeCode.Properties.ReadOnly = false;
			lookUpEdit_TrangThai.Properties.ReadOnly = false;
			textEdit_Amount.Properties.ReadOnly = false;
			lookUpEdit_DV.Properties.ReadOnly = false;
			textEdit_Remarks.Properties.ReadOnly = false;
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private void lookUpEdit_ChildType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_GB && Function.ParseInt(textEdit_Amount.EditValue) != 0 && lookUpEdit_TypeCode.EditValue != null && lookUpEdit_TypeCode.Properties.DataSource is DataTable source)
			{
				string typeCode = Function.ToString(lookUpEdit_TypeCode.EditValue);
				int num = Function.ParseInt(source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["TypeCode"]).Equals(typeCode))["Amount"]);
				int num2 = Function.ParseInt(textEdit_Amount.EditValue) / num;
				textEdit_TongSoThung.EditValue = num2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void textEdit_Amount_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_GB && Function.ParseInt(textEdit_Amount.EditValue) != 0 && lookUpEdit_TypeCode.EditValue != null && lookUpEdit_TypeCode.Properties.DataSource is DataTable source)
			{
				string typeCode = Function.ToString(lookUpEdit_TypeCode.EditValue);
				int num = Function.ParseInt(source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["TypeCode"]).Equals(typeCode))["Amount"]);
				int num2 = Function.ParseInt(textEdit_Amount.EditValue);
				if (num2 % num == 0)
				{
					int num3 = num2 / num;
					textEdit_TongSoThung.EditValue = num3;
				}
				else
				{
					int num4 = num2 / num + 1;
					textEdit_TongSoThung.EditValue = num4;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CopyfromPackingOrder()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetPOforCopy, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				FormMode = Enums.FormViewModeType.Add;
				textEdit_Amount.EditValue = frm_CFL2.ChooseItem["AmountItem"];
				lookUpEdit_DV.EditValue = "c";
				lookUpEdit_ChildType.EditValue = frm_CFL2.ChooseItem["TypeCode"];
				textEdit_Remarks.EditValue = null;
				Refresh();
				DataTable dataTable = new DataTable();
				dataTable = (DataTable)DataSource;
				dataTable.Rows[0]["BaseEntry"] = frm_CFL2.ChooseItem["POCode"];
				dataTable.Rows[0]["LineNum"] = 0;
				dataTable.AcceptChanges();
				DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CopyfromProductionOrder()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetProductionOrder, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				FormMode = Enums.FormViewModeType.Add;
				textEdit_Amount.EditValue = frm_CFL2.ChooseItem["Quantity"];
				lookUpEdit_DV.EditValue = "i";
				lookUpEdit_ChildType.EditValue = frm_CFL2.ChooseItem["ItemCode"];
				textEdit_Remarks.EditValue = null;
				Refresh();
				DataTable dataTable = new DataTable();
				dataTable = (DataTable)DataSource;
				dataTable.Rows[0]["BaseEntry"] = frm_CFL2.ChooseItem["BaseEntry"];
				dataTable.Rows[0]["LineNum"] = frm_CFL2.ChooseItem["LineNum"];
				dataTable.AcceptChanges();
				DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void PackingOrder_ItemClick(object sender, ItemClickEventArgs e)
	{
		CopyfromPackingOrder();
	}

	private void ProductionOrder_ItemClick(object sender, ItemClickEventArgs e)
	{
		CopyfromProductionOrder();
	}

	private void lookUpEdit_DV_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (lookUpEdit_DV.Text == "Case")
			{
				lookUpEdit_ChildType.Properties.TypeCFL = Enums.TypeCFL.GetTC;
			}
			else
			{
				lookUpEdit_ChildType.Properties.TypeCFL = Enums.TypeCFL.GetItem;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void drBtn_CopyFrom_Click(object sender, EventArgs e)
	{
		try
		{
			if (lookUpEdit_DV.Text == "Case")
			{
				CopyfromPackingOrder();
			}
			else
			{
				CopyfromProductionOrder();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Packing_Management.frm_PackingOrder));
		this.label_SoLenh = new Apzon.Controls.Controls.LabelLine();
		this.label_LoaiLenh = new Apzon.Controls.Controls.LabelLine();
		this.label_LoaiThung = new Apzon.Controls.Controls.LabelLine();
		this.label_TongSoThung = new Apzon.Controls.Controls.LabelLine();
		this.label_TrangThai = new Apzon.Controls.Controls.LabelLine();
		this.label_ChuyenDongGoi = new Apzon.Controls.Controls.LabelLine();
		this.textEdit_ChuyenDongGoi = new DevExpress.XtraEditors.TextEdit();
		this.lookUpEdit_TrangThai = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
		this.simpleBtn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.simpleBtn_OK = new DevExpress.XtraEditors.SimpleButton();
		this.label_ChiTiet = new DevExpress.XtraEditors.LabelControl();
		this.textEdit_TongSoThung = new Apzon.Controls.Editor.TextEditEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.refresh_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.lookUpEdit_USERID = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.lookUpEdit_Whs = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.lookUpEdit_LoaiLenh = new Apzon.Controls.Editor.LookUpEditEx();
		this.defaultLookAndFeel1 = new DevExpress.LookAndFeel.DefaultLookAndFeel(this.components);
		this.textEdit_Amount = new Apzon.Controls.Editor.TextEditEx();
		this.textEdit_Remarks = new Apzon.Controls.Editor.TextEditEx();
		this.labelControl1 = new DevExpress.XtraEditors.LabelControl();
		this.lookUpEdit_DV = new Apzon.Controls.Editor.LookUpEditEx();
		this.lookUpEdit_TypeCode = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelCtl_Loai = new DevExpress.XtraEditors.LabelControl();
		this.labelCtrl_Remark = new DevExpress.XtraEditors.LabelControl();
		this.labelCtl_SoLuong = new DevExpress.XtraEditors.LabelControl();
		this.bte_PO = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.drBtn_CopyFrom = new DevExpress.XtraEditors.DropDownButton();
		this.barManager1 = new DevExpress.XtraBars.BarManager(this.components);
		this.popupMenu = new DevExpress.XtraBars.PopupMenu(this.components);
		this.PackingOrder = new DevExpress.XtraBars.BarButtonItem();
		this.ProductionOrder = new DevExpress.XtraBars.BarButtonItem();
		this.barDockControlTop = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlBottom = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlLeft = new DevExpress.XtraBars.BarDockControl();
		this.barDockControlRight = new DevExpress.XtraBars.BarDockControl();
		this.lookUpEdit_ChildType = new Apzon.Forms.UserControls.ButtonEditCFL();
		((System.ComponentModel.ISupportInitialize)this.textEdit_ChuyenDongGoi.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_TrangThai.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit_TongSoThung.Properties).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_USERID.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Whs.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_LoaiLenh.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit_Amount.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit_Remarks.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_DV.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_TypeCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_PO.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_ChildType.Properties).BeginInit();
		base.SuspendLayout();
		this.label_SoLenh.FieldName = null;
		this.label_SoLenh.HideWhenEmpty = false;
		this.label_SoLenh.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label_SoLenh.Location = new System.Drawing.Point(11, 13);
		this.label_SoLenh.Name = "label_SoLenh";
		this.label_SoLenh.ShowUnderLine = true;
		this.label_SoLenh.Size = new System.Drawing.Size(111, 20);
		this.label_SoLenh.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label_SoLenh.TabIndex = 2;
		this.label_SoLenh.Utext = "Packing Order Code";
		this.label_LoaiLenh.FieldName = null;
		this.label_LoaiLenh.HideWhenEmpty = false;
		this.label_LoaiLenh.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label_LoaiLenh.Location = new System.Drawing.Point(11, 49);
		this.label_LoaiLenh.Name = "label_LoaiLenh";
		this.label_LoaiLenh.ShowUnderLine = true;
		this.label_LoaiLenh.Size = new System.Drawing.Size(111, 20);
		this.label_LoaiLenh.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label_LoaiLenh.TabIndex = 4;
		this.label_LoaiLenh.Utext = "Type PO";
		this.label_LoaiThung.FieldName = null;
		this.label_LoaiThung.HideWhenEmpty = false;
		this.label_LoaiThung.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label_LoaiThung.Location = new System.Drawing.Point(11, 85);
		this.label_LoaiThung.Name = "label_LoaiThung";
		this.label_LoaiThung.ShowUnderLine = true;
		this.label_LoaiThung.Size = new System.Drawing.Size(112, 20);
		this.label_LoaiThung.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label_LoaiThung.TabIndex = 6;
		this.label_LoaiThung.Utext = "Type Code";
		this.label_TongSoThung.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label_TongSoThung.FieldName = null;
		this.label_TongSoThung.HideWhenEmpty = false;
		this.label_TongSoThung.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label_TongSoThung.Location = new System.Drawing.Point(425, 10);
		this.label_TongSoThung.Name = "label_TongSoThung";
		this.label_TongSoThung.ShowUnderLine = true;
		this.label_TongSoThung.Size = new System.Drawing.Size(128, 20);
		this.label_TongSoThung.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label_TongSoThung.TabIndex = 8;
		this.label_TongSoThung.Utext = "Amount of Item";
		this.label_TrangThai.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label_TrangThai.FieldName = null;
		this.label_TrangThai.HideWhenEmpty = false;
		this.label_TrangThai.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label_TrangThai.Location = new System.Drawing.Point(425, 83);
		this.label_TrangThai.Name = "label_TrangThai";
		this.label_TrangThai.ShowUnderLine = true;
		this.label_TrangThai.Size = new System.Drawing.Size(128, 20);
		this.label_TrangThai.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label_TrangThai.TabIndex = 14;
		this.label_TrangThai.Utext = "Status";
		this.label_ChuyenDongGoi.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.label_ChuyenDongGoi.FieldName = null;
		this.label_ChuyenDongGoi.HideWhenEmpty = false;
		this.label_ChuyenDongGoi.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.label_ChuyenDongGoi.Location = new System.Drawing.Point(425, 46);
		this.label_ChuyenDongGoi.Name = "label_ChuyenDongGoi";
		this.label_ChuyenDongGoi.ShowUnderLine = true;
		this.label_ChuyenDongGoi.Size = new System.Drawing.Size(128, 20);
		this.label_ChuyenDongGoi.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.label_ChuyenDongGoi.TabIndex = 12;
		this.label_ChuyenDongGoi.Utext = "Packing Line";
		this.textEdit_ChuyenDongGoi.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.textEdit_ChuyenDongGoi.Location = new System.Drawing.Point(562, 45);
		this.textEdit_ChuyenDongGoi.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.textEdit_ChuyenDongGoi.Name = "textEdit_ChuyenDongGoi";
		this.textEdit_ChuyenDongGoi.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.textEdit_ChuyenDongGoi.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.textEdit_ChuyenDongGoi.Properties.Appearance.Options.UseBackColor = true;
		this.textEdit_ChuyenDongGoi.Properties.Appearance.Options.UseFont = true;
		this.textEdit_ChuyenDongGoi.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.textEdit_ChuyenDongGoi.Size = new System.Drawing.Size(168, 22);
		this.textEdit_ChuyenDongGoi.TabIndex = 11;
		this.lookUpEdit_TrangThai.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lookUpEdit_TrangThai.FieldName = "Status";
		this.lookUpEdit_TrangThai.IsSearchControl = false;
		this.lookUpEdit_TrangThai.Location = new System.Drawing.Point(562, 81);
		this.lookUpEdit_TrangThai.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.lookUpEdit_TrangThai.Name = "lookUpEdit_TrangThai";
		this.lookUpEdit_TrangThai.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.lookUpEdit_TrangThai.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lookUpEdit_TrangThai.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
		this.lookUpEdit_TrangThai.Properties.Appearance.Options.UseBackColor = true;
		this.lookUpEdit_TrangThai.Properties.Appearance.Options.UseFont = true;
		this.lookUpEdit_TrangThai.Properties.Appearance.Options.UseForeColor = true;
		this.lookUpEdit_TrangThai.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.lookUpEdit_TrangThai.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEdit_TrangThai.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.lookUpEdit_TrangThai.Properties.DisplayMember = "Name";
		this.lookUpEdit_TrangThai.Properties.NullText = "";
		this.lookUpEdit_TrangThai.Properties.ValueMember = "Value";
		this.lookUpEdit_TrangThai.Size = new System.Drawing.Size(168, 22);
		this.lookUpEdit_TrangThai.TabIndex = 28;
		this.labelControlEx1.FieldName = null;
		this.labelControlEx1.Location = new System.Drawing.Point(146, -19);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.Size = new System.Drawing.Size(0, 13);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 29;
		this.simpleBtn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.simpleBtn_Cancel.ImageOptions.ImageUri.Uri = "Cancel;Size16x16";
		this.simpleBtn_Cancel.Location = new System.Drawing.Point(94, 287);
		this.simpleBtn_Cancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.simpleBtn_Cancel.Name = "simpleBtn_Cancel";
		this.simpleBtn_Cancel.Size = new System.Drawing.Size(82, 23);
		this.simpleBtn_Cancel.TabIndex = 34;
		this.simpleBtn_Cancel.Text = "Cancel";
		this.simpleBtn_Cancel.Click += new System.EventHandler(this.simpleBtn_Cancel_Click_1);
		this.simpleBtn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.simpleBtn_OK.ImageOptions.ImageUri.Uri = "Apply;Size16x16";
		this.simpleBtn_OK.Location = new System.Drawing.Point(9, 287);
		this.simpleBtn_OK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.simpleBtn_OK.Name = "simpleBtn_OK";
		this.simpleBtn_OK.Size = new System.Drawing.Size(82, 23);
		this.simpleBtn_OK.TabIndex = 33;
		this.simpleBtn_OK.Text = "OK";
		this.simpleBtn_OK.Click += new System.EventHandler(this.simpleBtn_OK_Click_1);
		this.label_ChiTiet.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.label_ChiTiet.Appearance.Font = new System.Drawing.Font("Tahoma", 13.8f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.label_ChiTiet.Appearance.Options.UseFont = true;
		this.label_ChiTiet.Location = new System.Drawing.Point(12, 166);
		this.label_ChiTiet.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.label_ChiTiet.Name = "label_ChiTiet";
		this.label_ChiTiet.Size = new System.Drawing.Size(232, 22);
		this.label_ChiTiet.TabIndex = 31;
		this.label_ChiTiet.Text = "DETAIL OF PACKING ORDER";
		this.textEdit_TongSoThung.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.textEdit_TongSoThung.FieldName = "AmountItem";
		this.textEdit_TongSoThung.IsSearchControl = false;
		this.textEdit_TongSoThung.Location = new System.Drawing.Point(562, 10);
		this.textEdit_TongSoThung.Name = "textEdit_TongSoThung";
		this.textEdit_TongSoThung.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.textEdit_TongSoThung.Properties.Appearance.Options.UseFont = true;
		this.textEdit_TongSoThung.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.textEdit_TongSoThung.Properties.ReadOnly = true;
		this.textEdit_TongSoThung.Size = new System.Drawing.Size(168, 22);
		this.textEdit_TongSoThung.TabIndex = 36;
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.refresh_ToolStripMenuItem, this.addNewToolStripMenuItem });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(143, 48);
		this.refresh_ToolStripMenuItem.Name = "refresh_ToolStripMenuItem";
		this.refresh_ToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
		this.refresh_ToolStripMenuItem.Text = "Refresh";
		this.refresh_ToolStripMenuItem.Click += new System.EventHandler(this.refresh_ToolStripMenuItem_Click);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(142, 22);
		this.addNewToolStripMenuItem.Text = "Add New PO";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.lookUpEdit_USERID.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lookUpEdit_USERID.FieldName = "USERID";
		this.lookUpEdit_USERID.IsSearchControl = false;
		this.lookUpEdit_USERID.Location = new System.Drawing.Point(562, 118);
		this.lookUpEdit_USERID.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.lookUpEdit_USERID.Name = "lookUpEdit_USERID";
		this.lookUpEdit_USERID.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.lookUpEdit_USERID.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lookUpEdit_USERID.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
		this.lookUpEdit_USERID.Properties.Appearance.Options.UseBackColor = true;
		this.lookUpEdit_USERID.Properties.Appearance.Options.UseFont = true;
		this.lookUpEdit_USERID.Properties.Appearance.Options.UseForeColor = true;
		this.lookUpEdit_USERID.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.lookUpEdit_USERID.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEdit_USERID.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("U_NAME", "U_NAME"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("USERID", "USERID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.lookUpEdit_USERID.Properties.DisplayMember = "U_NAME";
		this.lookUpEdit_USERID.Properties.NullText = "";
		this.lookUpEdit_USERID.Properties.ValueMember = "USERID";
		this.lookUpEdit_USERID.Size = new System.Drawing.Size(168, 22);
		this.lookUpEdit_USERID.TabIndex = 39;
		this.labelLine1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(425, 120);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(128, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 38;
		this.labelLine1.Utext = "User Name";
		this.lookUpEdit_Whs.FieldName = "WhsCode";
		this.lookUpEdit_Whs.IsSearchControl = false;
		this.lookUpEdit_Whs.Location = new System.Drawing.Point(128, 118);
		this.lookUpEdit_Whs.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.lookUpEdit_Whs.Name = "lookUpEdit_Whs";
		this.lookUpEdit_Whs.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.lookUpEdit_Whs.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lookUpEdit_Whs.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
		this.lookUpEdit_Whs.Properties.Appearance.Options.UseBackColor = true;
		this.lookUpEdit_Whs.Properties.Appearance.Options.UseFont = true;
		this.lookUpEdit_Whs.Properties.Appearance.Options.UseForeColor = true;
		this.lookUpEdit_Whs.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.lookUpEdit_Whs.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEdit_Whs.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("WhsCode", "WhsCode", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("WhsName", "WhsName")
		});
		this.lookUpEdit_Whs.Properties.DisplayMember = "WhsCode";
		this.lookUpEdit_Whs.Properties.NullText = "";
		this.lookUpEdit_Whs.Properties.ValueMember = "WhsName";
		this.lookUpEdit_Whs.Size = new System.Drawing.Size(167, 22);
		this.lookUpEdit_Whs.TabIndex = 41;
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(11, 120);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(112, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 40;
		this.labelLine2.Utext = "Warehouse";
		this.lookUpEdit_LoaiLenh.FieldName = "TypePO";
		this.lookUpEdit_LoaiLenh.IsSearchControl = false;
		this.lookUpEdit_LoaiLenh.Location = new System.Drawing.Point(127, 47);
		this.lookUpEdit_LoaiLenh.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.lookUpEdit_LoaiLenh.Name = "lookUpEdit_LoaiLenh";
		this.lookUpEdit_LoaiLenh.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.lookUpEdit_LoaiLenh.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lookUpEdit_LoaiLenh.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
		this.lookUpEdit_LoaiLenh.Properties.Appearance.Options.UseBackColor = true;
		this.lookUpEdit_LoaiLenh.Properties.Appearance.Options.UseFont = true;
		this.lookUpEdit_LoaiLenh.Properties.Appearance.Options.UseForeColor = true;
		this.lookUpEdit_LoaiLenh.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.lookUpEdit_LoaiLenh.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEdit_LoaiLenh.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.lookUpEdit_LoaiLenh.Properties.DisplayMember = "Name";
		this.lookUpEdit_LoaiLenh.Properties.NullText = "";
		this.lookUpEdit_LoaiLenh.Properties.ValueMember = "Value";
		this.lookUpEdit_LoaiLenh.Size = new System.Drawing.Size(168, 22);
		this.lookUpEdit_LoaiLenh.TabIndex = 43;
		this.textEdit_Amount.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.textEdit_Amount.EditValue = "0";
		this.textEdit_Amount.FieldName = "Amountbundle";
		this.textEdit_Amount.IsSearchControl = false;
		this.textEdit_Amount.Location = new System.Drawing.Point(136, 222);
		this.textEdit_Amount.Name = "textEdit_Amount";
		this.textEdit_Amount.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.textEdit_Amount.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
		this.textEdit_Amount.Properties.Appearance.Options.UseFont = true;
		this.textEdit_Amount.Properties.Appearance.Options.UseForeColor = true;
		this.textEdit_Amount.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.textEdit_Amount.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.textEdit_Amount.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.textEdit_Amount.Size = new System.Drawing.Size(93, 22);
		this.textEdit_Amount.TabIndex = 51;
		this.textEdit_Amount.EditValueChanged += new System.EventHandler(this.textEdit_Amount_EditValueChanged);
		this.textEdit_Remarks.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.textEdit_Remarks.FieldName = "Remark";
		this.textEdit_Remarks.IsSearchControl = false;
		this.textEdit_Remarks.Location = new System.Drawing.Point(445, 222);
		this.textEdit_Remarks.Name = "textEdit_Remarks";
		this.textEdit_Remarks.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.textEdit_Remarks.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
		this.textEdit_Remarks.Properties.Appearance.Options.UseFont = true;
		this.textEdit_Remarks.Properties.Appearance.Options.UseForeColor = true;
		this.textEdit_Remarks.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.textEdit_Remarks.Size = new System.Drawing.Size(285, 22);
		this.textEdit_Remarks.TabIndex = 50;
		this.labelControl1.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.labelControl1.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelControl1.Appearance.Options.UseFont = true;
		this.labelControl1.Location = new System.Drawing.Point(55, 205);
		this.labelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.labelControl1.Name = "labelControl1";
		this.labelControl1.Size = new System.Drawing.Size(19, 13);
		this.labelControl1.TabIndex = 49;
		this.labelControl1.Text = "Unit";
		this.lookUpEdit_DV.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.lookUpEdit_DV.FieldName = "Unit";
		this.lookUpEdit_DV.IsSearchControl = false;
		this.lookUpEdit_DV.Location = new System.Drawing.Point(9, 222);
		this.lookUpEdit_DV.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.lookUpEdit_DV.Name = "lookUpEdit_DV";
		this.lookUpEdit_DV.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.lookUpEdit_DV.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lookUpEdit_DV.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
		this.lookUpEdit_DV.Properties.Appearance.Options.UseBackColor = true;
		this.lookUpEdit_DV.Properties.Appearance.Options.UseFont = true;
		this.lookUpEdit_DV.Properties.Appearance.Options.UseForeColor = true;
		this.lookUpEdit_DV.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.lookUpEdit_DV.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEdit_DV.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.lookUpEdit_DV.Properties.DisplayMember = "Name";
		this.lookUpEdit_DV.Properties.NullText = "";
		this.lookUpEdit_DV.Properties.ShowHeader = false;
		this.lookUpEdit_DV.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.lookUpEdit_DV.Properties.ValueMember = "Value";
		this.lookUpEdit_DV.Size = new System.Drawing.Size(121, 22);
		this.lookUpEdit_DV.TabIndex = 48;
		this.lookUpEdit_DV.EditValueChanged += new System.EventHandler(this.lookUpEdit_DV_EditValueChanged);
		this.lookUpEdit_TypeCode.FieldName = "TypeCode";
		this.lookUpEdit_TypeCode.IsSearchControl = false;
		this.lookUpEdit_TypeCode.Location = new System.Drawing.Point(127, 83);
		this.lookUpEdit_TypeCode.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.lookUpEdit_TypeCode.Name = "lookUpEdit_TypeCode";
		this.lookUpEdit_TypeCode.Properties.Appearance.BackColor = System.Drawing.Color.White;
		this.lookUpEdit_TypeCode.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lookUpEdit_TypeCode.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
		this.lookUpEdit_TypeCode.Properties.Appearance.Options.UseBackColor = true;
		this.lookUpEdit_TypeCode.Properties.Appearance.Options.UseFont = true;
		this.lookUpEdit_TypeCode.Properties.Appearance.Options.UseForeColor = true;
		this.lookUpEdit_TypeCode.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.lookUpEdit_TypeCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookUpEdit_TypeCode.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[3]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TypeCode", "TypeCode", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("TypeName", "TypeName"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Amount", "Amount")
		});
		this.lookUpEdit_TypeCode.Properties.DisplayMember = "TypeName";
		this.lookUpEdit_TypeCode.Properties.NullText = "";
		this.lookUpEdit_TypeCode.Properties.ValueMember = "TypeCode";
		this.lookUpEdit_TypeCode.Size = new System.Drawing.Size(168, 22);
		this.lookUpEdit_TypeCode.TabIndex = 47;
		this.lookUpEdit_TypeCode.EditValueChanged += new System.EventHandler(this.lookUpEdit_ChildType_EditValueChanged);
		this.labelCtl_Loai.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.labelCtl_Loai.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelCtl_Loai.Appearance.Options.UseFont = true;
		this.labelCtl_Loai.Location = new System.Drawing.Point(323, 206);
		this.labelCtl_Loai.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.labelCtl_Loai.Name = "labelCtl_Loai";
		this.labelCtl_Loai.Size = new System.Drawing.Size(58, 13);
		this.labelCtl_Loai.TabIndex = 44;
		this.labelCtl_Loai.Text = "Item in Case";
		this.labelCtrl_Remark.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.labelCtrl_Remark.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelCtrl_Remark.Appearance.Options.UseFont = true;
		this.labelCtrl_Remark.Location = new System.Drawing.Point(549, 206);
		this.labelCtrl_Remark.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.labelCtrl_Remark.Name = "labelCtrl_Remark";
		this.labelCtrl_Remark.Size = new System.Drawing.Size(42, 13);
		this.labelCtrl_Remark.TabIndex = 46;
		this.labelCtrl_Remark.Text = "Remarks";
		this.labelCtl_SoLuong.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.labelCtl_SoLuong.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.labelCtl_SoLuong.Appearance.Options.UseFont = true;
		this.labelCtl_SoLuong.Location = new System.Drawing.Point(163, 205);
		this.labelCtl_SoLuong.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.labelCtl_SoLuong.Name = "labelCtl_SoLuong";
		this.labelCtl_SoLuong.Size = new System.Drawing.Size(36, 13);
		this.labelCtl_SoLuong.TabIndex = 45;
		this.labelCtl_SoLuong.Text = "Amount";
		this.bte_PO.EnterMoveNextControl = true;
		this.bte_PO.FieldName = "POCode";
		this.bte_PO.IsSearchControl = false;
		this.bte_PO.Location = new System.Drawing.Point(128, 12);
		this.bte_PO.Name = "bte_PO";
		this.bte_PO.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.bte_PO.Properties.Appearance.Options.UseFont = true;
		this.bte_PO.Properties.AutoHeight = false;
		this.bte_PO.Properties.AutoLoadDataSource = true;
		this.bte_PO.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.bte_PO.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_PO.Properties.ContainerControl = this;
		this.bte_PO.Properties.DisplayMember = "";
		this.bte_PO.Properties.DisplayMember2 = "";
		this.bte_PO.Properties.LoadDataSourceWhenEnter = false;
		this.bte_PO.Properties.SeparateCharacterForMulti = ';';
		this.bte_PO.Properties.SeparateString = ", ";
		this.bte_PO.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.GetPO;
		this.bte_PO.Properties.UsingAutoCompleteBox = false;
		this.bte_PO.Properties.UsingDataSource = true;
		this.bte_PO.Properties.ValidateWhenNoDataSource = false;
		this.bte_PO.Properties.ValueMember = "";
		this.bte_PO.Size = new System.Drawing.Size(168, 21);
		this.bte_PO.TabIndex = 30;
		this.drBtn_CopyFrom.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.drBtn_CopyFrom.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drBtn_CopyFrom.ImageOptions.Image");
		this.drBtn_CopyFrom.Location = new System.Drawing.Point(622, 287);
		this.drBtn_CopyFrom.Name = "drBtn_CopyFrom";
		this.barManager1.SetPopupContextMenu(this.drBtn_CopyFrom, this.popupMenu);
		this.drBtn_CopyFrom.Size = new System.Drawing.Size(108, 23);
		this.drBtn_CopyFrom.TabIndex = 52;
		this.drBtn_CopyFrom.Text = "Copy from";
		this.drBtn_CopyFrom.Click += new System.EventHandler(this.drBtn_CopyFrom_Click);
		this.barManager1.DockControls.Add(this.barDockControlTop);
		this.barManager1.DockControls.Add(this.barDockControlBottom);
		this.barManager1.DockControls.Add(this.barDockControlLeft);
		this.barManager1.DockControls.Add(this.barDockControlRight);
		this.barManager1.Form = this;
		this.barManager1.Items.AddRange(new DevExpress.XtraBars.BarItem[2] { this.PackingOrder, this.ProductionOrder });
		this.barManager1.MaxItemId = 9;
		this.popupMenu.LinksPersistInfo.AddRange(new DevExpress.XtraBars.LinkPersistInfo[2]
		{
			new DevExpress.XtraBars.LinkPersistInfo(this.PackingOrder),
			new DevExpress.XtraBars.LinkPersistInfo(this.ProductionOrder)
		});
		this.popupMenu.Manager = this.barManager1;
		this.popupMenu.Name = "popupMenu";
		this.PackingOrder.Caption = "Packing Order";
		this.PackingOrder.Id = 6;
		this.PackingOrder.Name = "PackingOrder";
		this.PackingOrder.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.PackingOrder_ItemClick);
		this.ProductionOrder.Caption = "Production Order";
		this.ProductionOrder.Id = 7;
		this.ProductionOrder.Name = "ProductionOrder";
		this.ProductionOrder.ItemClick += new DevExpress.XtraBars.ItemClickEventHandler(this.ProductionOrder_ItemClick);
		this.barDockControlTop.CausesValidation = false;
		this.barDockControlTop.Dock = System.Windows.Forms.DockStyle.Top;
		this.barDockControlTop.Location = new System.Drawing.Point(0, 0);
		this.barDockControlTop.Manager = this.barManager1;
		this.barDockControlTop.Size = new System.Drawing.Size(742, 0);
		this.barDockControlBottom.CausesValidation = false;
		this.barDockControlBottom.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.barDockControlBottom.Location = new System.Drawing.Point(0, 315);
		this.barDockControlBottom.Manager = this.barManager1;
		this.barDockControlBottom.Size = new System.Drawing.Size(742, 0);
		this.barDockControlLeft.CausesValidation = false;
		this.barDockControlLeft.Dock = System.Windows.Forms.DockStyle.Left;
		this.barDockControlLeft.Location = new System.Drawing.Point(0, 0);
		this.barDockControlLeft.Manager = this.barManager1;
		this.barDockControlLeft.Size = new System.Drawing.Size(0, 315);
		this.barDockControlRight.CausesValidation = false;
		this.barDockControlRight.Dock = System.Windows.Forms.DockStyle.Right;
		this.barDockControlRight.Location = new System.Drawing.Point(742, 0);
		this.barDockControlRight.Manager = this.barManager1;
		this.barDockControlRight.Size = new System.Drawing.Size(0, 315);
		this.lookUpEdit_ChildType.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
		this.lookUpEdit_ChildType.EnterMoveNextControl = true;
		this.lookUpEdit_ChildType.FieldName = "ChildType";
		this.lookUpEdit_ChildType.IsSearchControl = false;
		this.lookUpEdit_ChildType.Location = new System.Drawing.Point(235, 222);
		this.lookUpEdit_ChildType.Name = "lookUpEdit_ChildType";
		this.lookUpEdit_ChildType.Properties.Appearance.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.lookUpEdit_ChildType.Properties.Appearance.ForeColor = System.Drawing.Color.Black;
		this.lookUpEdit_ChildType.Properties.Appearance.Options.UseFont = true;
		this.lookUpEdit_ChildType.Properties.Appearance.Options.UseForeColor = true;
		this.lookUpEdit_ChildType.Properties.AutoHeight = false;
		this.lookUpEdit_ChildType.Properties.AutoLoadDataSource = true;
		this.lookUpEdit_ChildType.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.lookUpEdit_ChildType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.lookUpEdit_ChildType.Properties.ContainerControl = this;
		this.lookUpEdit_ChildType.Properties.DisplayMember = "";
		this.lookUpEdit_ChildType.Properties.DisplayMember2 = "";
		this.lookUpEdit_ChildType.Properties.LoadDataSourceWhenEnter = false;
		this.lookUpEdit_ChildType.Properties.SeparateCharacterForMulti = ';';
		this.lookUpEdit_ChildType.Properties.SeparateString = ", ";
		this.lookUpEdit_ChildType.Properties.UsingAutoCompleteBox = false;
		this.lookUpEdit_ChildType.Properties.UsingDataSource = true;
		this.lookUpEdit_ChildType.Properties.ValidateWhenNoDataSource = false;
		this.lookUpEdit_ChildType.Properties.ValueMember = "";
		this.lookUpEdit_ChildType.Size = new System.Drawing.Size(204, 21);
		this.lookUpEdit_ChildType.TabIndex = 57;
		base.Appearance.BackColor = System.Drawing.Color.White;
		base.Appearance.Options.UseBackColor = true;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(742, 315);
		this.ContextMenuStrip = this.contextMenuStrip2;
		base.Controls.Add(this.lookUpEdit_ChildType);
		base.Controls.Add(this.drBtn_CopyFrom);
		base.Controls.Add(this.textEdit_Amount);
		base.Controls.Add(this.textEdit_Remarks);
		base.Controls.Add(this.labelControl1);
		base.Controls.Add(this.lookUpEdit_DV);
		base.Controls.Add(this.lookUpEdit_TypeCode);
		base.Controls.Add(this.labelCtl_Loai);
		base.Controls.Add(this.labelCtrl_Remark);
		base.Controls.Add(this.labelCtl_SoLuong);
		base.Controls.Add(this.lookUpEdit_LoaiLenh);
		base.Controls.Add(this.lookUpEdit_Whs);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.lookUpEdit_USERID);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.textEdit_TongSoThung);
		base.Controls.Add(this.simpleBtn_Cancel);
		base.Controls.Add(this.simpleBtn_OK);
		base.Controls.Add(this.label_ChiTiet);
		base.Controls.Add(this.labelControlEx1);
		base.Controls.Add(this.lookUpEdit_TrangThai);
		base.Controls.Add(this.label_TrangThai);
		base.Controls.Add(this.label_ChuyenDongGoi);
		base.Controls.Add(this.textEdit_ChuyenDongGoi);
		base.Controls.Add(this.label_TongSoThung);
		base.Controls.Add(this.label_LoaiThung);
		base.Controls.Add(this.label_LoaiLenh);
		base.Controls.Add(this.label_SoLenh);
		base.Controls.Add(this.bte_PO);
		base.Controls.Add(this.barDockControlLeft);
		base.Controls.Add(this.barDockControlRight);
		base.Controls.Add(this.barDockControlBottom);
		base.Controls.Add(this.barDockControlTop);
		base.FormId = "oPackingOrder";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_PackingOrder.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_PackingOrder";
		this.Text = " Packing Order";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_PackingOrder_OnControlValueChanged);
		base.OnControlValueChanging += new Apzon.Controls.Form.ControlValueChanging(this.frm_PackingOrder_OnControlValueChanging);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_PackingOrder_FormClosing);
		base.Load += new System.EventHandler(this.frm_PackingOrder_Load);
		((System.ComponentModel.ISupportInitialize)this.textEdit_ChuyenDongGoi.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_TrangThai.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit_TongSoThung.Properties).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_USERID.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_Whs.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_LoaiLenh.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit_Amount.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit_Remarks.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_DV.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_TypeCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_PO.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.barManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.popupMenu).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookUpEdit_ChildType.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
