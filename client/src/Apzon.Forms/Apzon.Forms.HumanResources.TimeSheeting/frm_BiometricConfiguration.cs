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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.HumanResources.Biometric;
using Apzon.Forms.HumanResources.Biometric.FP_CLOCK;
using Apzon.Forms.HumanResources.Biometric.KJTech;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using Microsoft.Win32;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_BiometricConfiguration : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public int EmpId;

	private bool _isChanging;

	private bool _isCancel;

	private bool _forceClose;

	private bool _loadingData;

	private bool _connectOk;

	private DataTable _dtGeneSettings;

	private DataTable _dtDefault;

	public Func<Task> RefreshCal;

	public Func<Task> RefreshSettings;

	private DeviceManipulator manipulator = new DeviceManipulator();

	private static int idwErrorCode;

	private IContainer components;

	public SimpleButtonEx btnCancel;

	public SimpleButtonEx btn_Save;

	private RepositoryItemLookUpEdit rep_Deparment;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SplitContainerControl splitContainerControl1;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemLookUpEdit rep_CommType;

	private LabelLine labelLine3;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private LabelLine lbl_MachineID;

	private LookUpEditEx luk_CommType;

	private LabelLine labelLine4;

	private TextEditEx txt_MachineName;

	private TextEditEx txt_IP;

	private LabelLine labelLine5;

	private LabelLine labelLine6;

	private LookUpEditEx luk_BaudRate;

	private LookUpEditEx luk_COMPort;

	private PanelControl pnl_Com;

	private PanelControl pnl_Tcp;

	private LabelLine labelLine7;

	private TextEditEx calc_TCPPort;

	private TextEditEx calc_CommKey;

	private TextEditEx calc_MachineID;

	private CheckEditEx chk_Active;

	private TextEditEx txt_SerialNumber;

	private LabelLine labelLine8;

	public SimpleButtonEx btn_TestConnect;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemCheckEdit rep_Active;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private LookUpEditEx luk_McTyp;

	private LabelLine labelLine9;

	private new XtraScrollableControl ScrollEx;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (_formMode)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButtonEx simpleButtonEx4 = btn_Save;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx4.Enabled = enabled;
				btn_Save.Text = Resources.ButtonAdd;
				calc_MachineID.ReadOnly = false;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButtonEx simpleButtonEx3 = btn_Save;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx3.Enabled = enabled;
				btn_Save.Text = Resources.ButtonUpdate;
				calc_MachineID.ReadOnly = true;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButtonEx simpleButtonEx2 = btn_Save;
				bool enabled = (btnCancel.Enabled = true);
				simpleButtonEx2.Enabled = enabled;
				btn_Save.Text = Resources.ButtonOK;
				calc_MachineID.ReadOnly = true;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButtonEx simpleButtonEx = btn_Save;
				bool enabled = (btnCancel.Enabled = false);
				simpleButtonEx.Enabled = enabled;
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_BiometricConfiguration()
	{
		InitializeComponent();
		IsScrollable = true;
		IsSetSuperTip = true;
		FormId = "oBiometricConfiguration";
		OnLoadAsync += frm_BioConfig_Load;
	}

	private void btn_TestConnect_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			string text = TestConnect();
			if (string.IsNullOrEmpty(text))
			{
				_connectOk = true;
				WinCommons.Success(Enums.MessageCode.PingDeviceSuccess, "");
			}
			else
			{
				_connectOk = false;
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private bool SearchforCom1(ref string sCom)
	{
		RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("HARDWARE\\DEVICEMAP\\SERIALCOMM");
		string[] valueNames = registryKey.GetValueNames();
		for (int i = 0; i < valueNames.Length; i++)
		{
			string text = "";
			text = registryKey.GetValue(valueNames[i]).ToString();
			if (text == "")
			{
				continue;
			}
			sCom = "";
			if (!(valueNames[i] == "\\Device\\USBSER000"))
			{
				continue;
			}
			for (int j = 0; j <= 10; j++)
			{
				string text2 = "";
				RegistryKey registryKey2 = Registry.LocalMachine.OpenSubKey("SYSTEM\\CurrentControlSet\\Enum\\USB\\VID_1B55&PID_B400\\" + j + "\\Device Parameters");
				if (registryKey2 != null)
				{
					text2 = registryKey2.GetValue("PortName").ToString();
					if (text == text2)
					{
						sCom = text2;
						return true;
					}
				}
			}
		}
		return false;
	}

	private string TestConnect()
	{
		try
		{
			bool flag = false;
			DataTable dataTable = DataSource as DataTable;
			string text = "";
			string sCom = "";
			int num = 0;
			int baudRate = 115200;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.Rows[0];
				string text2 = Function.ToString(dataRow["CommType"]);
				string text3 = Function.ToString(dataRow["DeviceCode"]);
				text = Function.ToString(dataRow["MachineID"]);
				if (string.IsNullOrWhiteSpace(text))
				{
					return "Machine ID can not be null!";
				}
				if (Function.ParseInt(text) < 0 || Function.ParseInt(text) > 256)
				{
					return "*Device ID illegal!";
				}
				int num2 = Function.ParseInt(dataRow["CommKey"]);
				if (num2 < 0 || num2 > 999999)
				{
					return "*CommKey illegal!";
				}
				if (text3.Equals("KJ"))
				{
					return new KJtechManipulator().TestConnect(ip: Function.ToString(dataRow["IPAddress"]), port: Function.ParseInt(dataRow["TCPPort"]), machineId: Function.ParseInt(text));
				}
				if (text3.Equals("FP"))
				{
					FpClockManipulator fpClockManipulator = new FpClockManipulator();
					string ip = Function.ToString(dataRow["IPAddress"]);
					int port = Function.ParseInt(dataRow["TCPPort"]);
					string serialNumber = fpClockManipulator.GetSerialNumber(ip, port);
					if (!string.IsNullOrWhiteSpace(serialNumber))
					{
						txt_SerialNumber.Text = serialNumber;
						return string.Empty;
					}
					return "Can not ping device! " + fpClockManipulator.GetErrorString();
				}
				ZkemClient zkemClient = new ZkemClient();
				zkemClient.SetCommPassword(num2);
				switch (text2)
				{
				case "T":
				{
					string text4 = Function.ToString(dataRow["IPAddress"]);
					string text5 = Function.ToString(dataRow["TCPPort"]);
					if (text4 == "" || text5 == "")
					{
						return "* IP, Port can not be null!";
					}
					if (Function.ParseInt(text5) <= 0 || Function.ParseInt(text5) > 65535)
					{
						return "*Port illegal!";
					}
					flag = zkemClient.Connect_Net(text4, Function.ParseInt(text5));
					break;
				}
				case "R":
					num = Function.ParseInt(dataRow["COMPort"]);
					baudRate = Function.ParseInt(dataRow["BaudRate"]);
					if (Function.ToString(num) == "" || Function.ToString(baudRate) == "")
					{
						return "* Port, Baudrate cannot be null !";
					}
					flag = zkemClient.Connect_Com(num, Function.ParseInt(text), baudRate);
					break;
				case "U":
					if (!WinFuntions.SearchforCom(ref sCom))
					{
						return "*Can not find the virtual serial port that can be used !";
					}
					for (num = 1; num < 10; num++)
					{
						if (sCom.IndexOf(num.ToString()) > -1)
						{
							break;
						}
					}
					flag = zkemClient.Connect_Com(num, Function.ParseInt(text), baudRate);
					break;
				}
				if (flag)
				{
					zkemClient.GetSerialNumber(Function.ParseInt(text), out var dwSerialNumber);
					txt_SerialNumber.EditValue = dwSerialNumber;
					return string.Empty;
				}
				zkemClient.GetLastError(ref idwErrorCode);
				return "Can not ping device! ErrorCode:" + idwErrorCode;
			}
			return "Data is empty!";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return ex.Message;
		}
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
				comboBoxEdit.SelectedValueChanged += OnContentChanged;
				comboBoxEdit.EditValueChanged += OnContentChanged;
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
			}
			else if (control is SeparatorComboBox)
			{
				((SeparatorComboBox)control).SelectedIndexChanged += OnContentChanged;
			}
			else if (control is RadioButton)
			{
				((RadioButton)control).CheckedChanged += OnContentChanged;
			}
			else
			{
				SetEventControl(control);
			}
		}
	}

	protected void OnContentChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isChanging && !_loadingData && FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
				_connectOk = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_CommType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			switch (Function.ToString(luk_CommType.EditValue))
			{
			case "T":
				pnl_Tcp.Visible = true;
				pnl_Com.Visible = false;
				break;
			case "R":
				pnl_Com.Location = pnl_Tcp.Location;
				pnl_Tcp.Visible = false;
				pnl_Com.Visible = true;
				break;
			case "U":
				pnl_Tcp.Visible = false;
				pnl_Com.Visible = false;
				break;
			default:
				pnl_Tcp.Visible = false;
				pnl_Com.Visible = false;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			_loadingData = true;
			DataSource = gv_Doc.GetFocusedDataRow().ToArray().CopyToDataTable();
			_loadingData = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_BioConfig_Load(object sender, EventArgs e)
	{
		try
		{
			LoadInitForm();
			await LoadData();
			SetEventControl(this);
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_loadingData || FormMode != Enums.FormViewModeType.Update)
			{
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				e.Allow = false;
				return;
			}
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				dataTable.RejectChanges();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			_loadingData = true;
			DataTable dataTable = await _httpClient.TimeSheetingClient.GetAllBiometricConfig();
			if (dataTable.Rows.Count == 0)
			{
				await REFRESH();
				DataSource = gv_Doc.GetFocusedDataRow().ToArray().CopyToDataTable();
			}
			else
			{
				dataTable.AcceptChanges();
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				FormMode = Enums.FormViewModeType.View;
			}
			_loadingData = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		finally
		{
			_loadingData = false;
		}
	}

	private async Task<string> SaveData()
	{
		WinCommons._OpenProcessing();
		try
		{
			if (!_connectOk)
			{
				string text = TestConnect();
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					return "Error";
				}
				_connectOk = true;
			}
			int currentEntry = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			DataTable dt = DataSource as DataTable;
			HttpResult httpResult = await _httpClient.TimeSheetingClient.SaveBiometricConfig(dt);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				await REFRESH_DATA();
				int num = gv_Doc.LocateByValue("AbsEntry", currentEntry);
				if (num != int.MinValue)
				{
					gv_Doc.FocusedRowHandle = num;
					gv_Doc.MakeRowVisible(num);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
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
		}
		return string.Empty;
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OBMC");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["Status"] = "Y";
			tableStructure.Rows.Add(dataRow);
			grd_Doc.DataSource = tableStructure;
			tableStructure.AcceptChanges();
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private async void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
			{
				return;
			}
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("AbsEntry"));
			if (num != 0)
			{
				string text = await _httpClient.TimeSheetingClient.RemoveConfig(num);
				if (string.IsNullOrWhiteSpace(text))
				{
					WinCommons.Success(Enums.MessageCode.RemoveSuccess, "");
					await REFRESH_DATA();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, "Remove device error! " + text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadInitForm()
	{
		try
		{
			List<Common.Table_Code> list = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "T",
					Name = "TCP/IP"
				},
				new Common.Table_Code
				{
					Code = "R",
					Name = "RS232/RS485"
				},
				new Common.Table_Code
				{
					Code = "U",
					Name = "USB"
				}
			};
			SetDataSourceByList(luk_CommType, list);
			List<Common.Table_Code> list2 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "1",
					Name = "COM1"
				},
				new Common.Table_Code
				{
					Code = "2",
					Name = "COM2"
				},
				new Common.Table_Code
				{
					Code = "3",
					Name = "COM3"
				},
				new Common.Table_Code
				{
					Code = "4",
					Name = "COM4"
				},
				new Common.Table_Code
				{
					Code = "5",
					Name = "COM5"
				},
				new Common.Table_Code
				{
					Code = "6",
					Name = "COM6"
				},
				new Common.Table_Code
				{
					Code = "7",
					Name = "COM7"
				},
				new Common.Table_Code
				{
					Code = "8",
					Name = "COM8"
				},
				new Common.Table_Code
				{
					Code = "9",
					Name = "COM9"
				}
			};
			SetDataSourceByList(luk_COMPort, list2);
			List<Common.Table_Code> list3 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "1200",
					Name = "1200"
				},
				new Common.Table_Code
				{
					Code = "2400",
					Name = "2400"
				},
				new Common.Table_Code
				{
					Code = "4800",
					Name = "4800"
				},
				new Common.Table_Code
				{
					Code = "9600",
					Name = "9600"
				},
				new Common.Table_Code
				{
					Code = "19200",
					Name = "19200"
				},
				new Common.Table_Code
				{
					Code = "34800",
					Name = "34800"
				},
				new Common.Table_Code
				{
					Code = "57600",
					Name = "57600"
				},
				new Common.Table_Code
				{
					Code = "115200",
					Name = "115200"
				}
			};
			SetDataSourceByList(luk_BaudRate, list3);
			List<Common.Table_Code> list4 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "",
					Name = "Default"
				},
				new Common.Table_Code
				{
					Code = "KJ",
					Name = "KJTech"
				},
				new Common.Table_Code
				{
					Code = "FP",
					Name = "FpClock"
				}
			};
			SetDataSourceByList(luk_McTyp, list4);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetDataSourceByList(LookUpEditEx luk, List<Common.Table_Code> list)
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
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_BioConfig_FormClosing(object sender, FormClosingEventArgs e)
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
			if (FormMode == Enums.FormViewModeType.Update)
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

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				btn_TestConnect.Enabled = false;
				await SaveData();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
				btn_TestConnect.Enabled = true;
			}
			else
			{
				Close();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_BiometricConfiguration));
		this.ScrollEx = new DevExpress.XtraEditors.XtraScrollableControl();
		this.rep_Deparment = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CommType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Active = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.luk_McTyp = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.chk_Active = new Apzon.Controls.Editor.CheckEditEx();
		this.txt_SerialNumber = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.calc_CommKey = new Apzon.Controls.Editor.TextEditEx();
		this.calc_MachineID = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.pnl_Com = new DevExpress.XtraEditors.PanelControl();
		this.luk_COMPort = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.luk_BaudRate = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.pnl_Tcp = new DevExpress.XtraEditors.PanelControl();
		this.calc_TCPPort = new Apzon.Controls.Editor.TextEditEx();
		this.txt_IP = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txt_MachineName = new Apzon.Controls.Editor.TextEditEx();
		this.luk_CommType = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_MachineID = new Apzon.Controls.Controls.LabelLine();
		this.btn_TestConnect = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CommType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Active).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1.Panel1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1.Panel2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_McTyp.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SerialNumber.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_CommKey.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_MachineID.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pnl_Com).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_COMPort.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BaudRate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.pnl_Tcp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_TCPPort.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_IP.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MachineName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CommType.Properties).BeginInit();
		base.SuspendLayout();
		this.ScrollEx.Location = new System.Drawing.Point(0, 0);
		this.ScrollEx.Name = "ScrollEx";
		this.ScrollEx.TabIndex = 0;
		this.rep_Deparment.AutoHeight = false;
		this.rep_Deparment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Deparment.Name = "rep_Deparment";
		this.rep_Deparment.NullText = "";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.Enabled = true;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(99, 336);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 13;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Visible = true;
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.Enabled = true;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(2, 336);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(93, 20);
		this.btn_Save.TabIndex = 12;
		this.btn_Save.Text = "OK";
		this.btn_Save.Visible = true;
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_CommType, this.rep_Active });
		this.grd_Doc.Size = new System.Drawing.Size(340, 330);
		this.grd_Doc.TabIndex = 87;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.addNewToolStripMenuItem.Text = "Add new";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oBiometricConfiguration";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.ItemId = "ODOC";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.ReadOnly = true;
		this.gv_Doc.OptionsView.RowAutoHeight = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumnEx1.Caption = "ID";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "AbsEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Machine No.";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "MachineID";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 0;
		this.gridColumnEx3.Caption = "Machine Name";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "MachineName";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx4.Caption = "Commnucation Type";
		this.gridColumnEx4.ColumnEdit = this.rep_CommType;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "CommType";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.rep_CommType.AutoHeight = false;
		this.rep_CommType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CommType.Name = "rep_CommType";
		this.rep_CommType.NullText = "";
		this.gridColumnEx5.Caption = "Active";
		this.gridColumnEx5.ColumnEdit = this.rep_Active;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Status";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 2;
		this.rep_Active.AutoHeight = false;
		this.rep_Active.Name = "rep_Active";
		this.rep_Active.ValueChecked = "Y";
		this.rep_Active.ValueUnchecked = "N";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Panel1.Name = "";
		this.splitContainerControl1.Panel1.Size = new System.Drawing.Size(340, 330);
		this.splitContainerControl1.Panel1.TabIndex = 0;
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.luk_McTyp);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine9);
		this.splitContainerControl1.Panel2.Controls.Add(this.chk_Active);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_SerialNumber);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine8);
		this.splitContainerControl1.Panel2.Controls.Add(this.calc_CommKey);
		this.splitContainerControl1.Panel2.Controls.Add(this.calc_MachineID);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine7);
		this.splitContainerControl1.Panel2.Controls.Add(this.pnl_Com);
		this.splitContainerControl1.Panel2.Controls.Add(this.pnl_Tcp);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_MachineName);
		this.splitContainerControl1.Panel2.Controls.Add(this.luk_CommType);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel2.Controls.Add(this.lbl_MachineID);
		this.splitContainerControl1.Panel2.Location = new System.Drawing.Point(350, 0);
		this.splitContainerControl1.Panel2.Name = "";
		this.splitContainerControl1.Panel2.Size = new System.Drawing.Size(230, 330);
		this.splitContainerControl1.Panel2.TabIndex = 1;
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(580, 330);
		this.splitContainerControl1.SplitterPosition = 340;
		this.splitContainerControl1.TabIndex = 88;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.luk_McTyp.EditControl = null;
		this.luk_McTyp.FieldName = "DeviceCode";
		this.luk_McTyp.IsSearchControl = false;
		this.luk_McTyp.Location = new System.Drawing.Point(78, 301);
		this.luk_McTyp.Name = "luk_McTyp";
		this.luk_McTyp.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_McTyp.Properties.NullText = "";
		this.luk_McTyp.SettingInVisible = false;
		this.luk_McTyp.SettingReadonly = false;
		this.luk_McTyp.Size = new System.Drawing.Size(147, 20);
		this.luk_McTyp.TabIndex = 27;
		this.luk_McTyp.Visible = true;
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Machine Type";
		this.labelLine9.FieldName = "lblCOMPort";
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(4, 304);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(76, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 28;
		this.labelLine9.Utext = "Machine Type";
		this.chk_Active.EditControl = null;
		this.chk_Active.FieldName = "Status";
		this.chk_Active.IsSearchControl = false;
		this.chk_Active.Location = new System.Drawing.Point(79, 56);
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.Properties.Caption = "Active";
		this.chk_Active.Properties.ValueChecked = "Y";
		this.chk_Active.Properties.ValueUnchecked = "N";
		this.chk_Active.SettingInVisible = false;
		this.chk_Active.SettingReadonly = false;
		this.chk_Active.Size = new System.Drawing.Size(75, 20);
		this.chk_Active.TabIndex = 3;
		this.chk_Active.Visible = true;
		this.txt_SerialNumber.EditControl = null;
		this.txt_SerialNumber.FieldName = "SerialNumber";
		this.txt_SerialNumber.IsSearchControl = false;
		this.txt_SerialNumber.Location = new System.Drawing.Point(79, 137);
		this.txt_SerialNumber.Name = "txt_SerialNumber";
		this.txt_SerialNumber.SettingInVisible = false;
		this.txt_SerialNumber.SettingReadonly = false;
		this.txt_SerialNumber.Size = new System.Drawing.Size(147, 20);
		this.txt_SerialNumber.TabIndex = 6;
		this.txt_SerialNumber.Visible = true;
		this.labelLine8.EditControl = null;
		this.labelLine8.EditValue = "Serial Number";
		this.labelLine8.FieldName = "lblSerialNumber";
		this.labelLine8.HideWhenEmpty = false;
		this.labelLine8.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine8.Location = new System.Drawing.Point(4, 140);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.ReadOnly = false;
		this.labelLine8.SettingInVisible = false;
		this.labelLine8.SettingReadonly = false;
		this.labelLine8.ShowUnderLine = true;
		this.labelLine8.Size = new System.Drawing.Size(76, 20);
		this.labelLine8.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine8.TabIndex = 26;
		this.labelLine8.Utext = "Serial Number";
		this.calc_CommKey.EditControl = null;
		this.calc_CommKey.FieldName = "CommKey";
		this.calc_CommKey.IsSearchControl = false;
		this.calc_CommKey.Location = new System.Drawing.Point(79, 108);
		this.calc_CommKey.Name = "calc_CommKey";
		this.calc_CommKey.Properties.Mask.EditMask = "f0";
		this.calc_CommKey.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.calc_CommKey.SettingInVisible = false;
		this.calc_CommKey.SettingReadonly = false;
		this.calc_CommKey.Size = new System.Drawing.Size(147, 20);
		this.calc_CommKey.TabIndex = 5;
		this.calc_CommKey.Visible = true;
		this.calc_MachineID.EditControl = null;
		this.calc_MachineID.FieldName = "MachineID";
		this.calc_MachineID.IsSearchControl = false;
		this.calc_MachineID.Location = new System.Drawing.Point(79, 3);
		this.calc_MachineID.Name = "calc_MachineID";
		this.calc_MachineID.Properties.Mask.EditMask = "f0";
		this.calc_MachineID.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.calc_MachineID.SettingInVisible = false;
		this.calc_MachineID.SettingReadonly = false;
		this.calc_MachineID.Size = new System.Drawing.Size(147, 20);
		this.calc_MachineID.TabIndex = 1;
		this.calc_MachineID.Visible = true;
		this.labelLine7.EditControl = null;
		this.labelLine7.EditValue = "Comm Key";
		this.labelLine7.FieldName = "lblMachineName";
		this.labelLine7.HideWhenEmpty = false;
		this.labelLine7.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine7.Location = new System.Drawing.Point(5, 111);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.ReadOnly = false;
		this.labelLine7.SettingInVisible = false;
		this.labelLine7.SettingReadonly = false;
		this.labelLine7.ShowUnderLine = true;
		this.labelLine7.Size = new System.Drawing.Size(76, 20);
		this.labelLine7.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine7.TabIndex = 22;
		this.labelLine7.Utext = "Comm Key";
		this.pnl_Com.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.pnl_Com.Controls.Add(this.luk_COMPort);
		this.pnl_Com.Controls.Add(this.labelLine6);
		this.pnl_Com.Controls.Add(this.luk_BaudRate);
		this.pnl_Com.Controls.Add(this.labelLine5);
		this.pnl_Com.Location = new System.Drawing.Point(3, 234);
		this.pnl_Com.Name = "pnl_Com";
		this.pnl_Com.Size = new System.Drawing.Size(232, 64);
		this.pnl_Com.TabIndex = 21;
		this.luk_COMPort.EditControl = null;
		this.luk_COMPort.FieldName = "COMPort";
		this.luk_COMPort.IsSearchControl = false;
		this.luk_COMPort.Location = new System.Drawing.Point(77, 5);
		this.luk_COMPort.Name = "luk_COMPort";
		this.luk_COMPort.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_COMPort.Properties.NullText = "";
		this.luk_COMPort.SettingInVisible = false;
		this.luk_COMPort.SettingReadonly = false;
		this.luk_COMPort.Size = new System.Drawing.Size(147, 20);
		this.luk_COMPort.TabIndex = 7;
		this.luk_COMPort.Visible = true;
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Serial Port";
		this.labelLine6.FieldName = "lblCOMPort";
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(3, 8);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(76, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 14;
		this.labelLine6.Utext = "Serial Port";
		this.luk_BaudRate.EditControl = null;
		this.luk_BaudRate.FieldName = "BaudRate";
		this.luk_BaudRate.IsSearchControl = false;
		this.luk_BaudRate.Location = new System.Drawing.Point(77, 32);
		this.luk_BaudRate.Name = "luk_BaudRate";
		this.luk_BaudRate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_BaudRate.Properties.NullText = "";
		this.luk_BaudRate.SettingInVisible = false;
		this.luk_BaudRate.SettingReadonly = false;
		this.luk_BaudRate.Size = new System.Drawing.Size(147, 20);
		this.luk_BaudRate.TabIndex = 8;
		this.luk_BaudRate.Visible = true;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Baud Rate";
		this.labelLine5.FieldName = "lblBaudRate";
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(3, 35);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(76, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 15;
		this.labelLine5.Utext = "Baud Rate";
		this.pnl_Tcp.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.pnl_Tcp.Controls.Add(this.calc_TCPPort);
		this.pnl_Tcp.Controls.Add(this.txt_IP);
		this.pnl_Tcp.Controls.Add(this.labelLine3);
		this.pnl_Tcp.Controls.Add(this.labelLine4);
		this.pnl_Tcp.Location = new System.Drawing.Point(3, 163);
		this.pnl_Tcp.Name = "pnl_Tcp";
		this.pnl_Tcp.Size = new System.Drawing.Size(232, 64);
		this.pnl_Tcp.TabIndex = 20;
		this.calc_TCPPort.EditControl = null;
		this.calc_TCPPort.FieldName = "TCPPort";
		this.calc_TCPPort.IsSearchControl = false;
		this.calc_TCPPort.Location = new System.Drawing.Point(77, 32);
		this.calc_TCPPort.Name = "calc_TCPPort";
		this.calc_TCPPort.Properties.Mask.EditMask = "f0";
		this.calc_TCPPort.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.calc_TCPPort.SettingInVisible = false;
		this.calc_TCPPort.SettingReadonly = false;
		this.calc_TCPPort.Size = new System.Drawing.Size(147, 20);
		this.calc_TCPPort.TabIndex = 8;
		this.calc_TCPPort.Visible = true;
		this.txt_IP.EditControl = null;
		this.txt_IP.FieldName = "IPAddress";
		this.txt_IP.IsSearchControl = false;
		this.txt_IP.Location = new System.Drawing.Point(77, 5);
		this.txt_IP.Name = "txt_IP";
		this.txt_IP.SettingInVisible = false;
		this.txt_IP.SettingReadonly = false;
		this.txt_IP.Size = new System.Drawing.Size(147, 20);
		this.txt_IP.TabIndex = 7;
		this.txt_IP.Visible = true;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "IP";
		this.labelLine3.FieldName = "lblIPAddress";
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(3, 8);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(76, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 6;
		this.labelLine3.Utext = "IP";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Port";
		this.labelLine4.FieldName = "lblTCPPort";
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(3, 35);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(76, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 9;
		this.labelLine4.Utext = "Port";
		this.txt_MachineName.EditControl = null;
		this.txt_MachineName.FieldName = "MachineName";
		this.txt_MachineName.IsSearchControl = false;
		this.txt_MachineName.Location = new System.Drawing.Point(79, 30);
		this.txt_MachineName.Name = "txt_MachineName";
		this.txt_MachineName.SettingInVisible = false;
		this.txt_MachineName.SettingReadonly = false;
		this.txt_MachineName.Size = new System.Drawing.Size(147, 20);
		this.txt_MachineName.TabIndex = 2;
		this.txt_MachineName.Visible = true;
		this.luk_CommType.EditControl = null;
		this.luk_CommType.FieldName = "CommType";
		this.luk_CommType.IsSearchControl = false;
		this.luk_CommType.Location = new System.Drawing.Point(79, 82);
		this.luk_CommType.Name = "luk_CommType";
		this.luk_CommType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_CommType.Properties.NullText = "";
		this.luk_CommType.SettingInVisible = false;
		this.luk_CommType.SettingReadonly = false;
		this.luk_CommType.Size = new System.Drawing.Size(147, 20);
		this.luk_CommType.TabIndex = 4;
		this.luk_CommType.Visible = true;
		this.luk_CommType.EditValueChanged += new System.EventHandler(this.luk_CommType_EditValueChanged);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Comm. Type";
		this.labelLine2.FieldName = "lblCommType";
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(4, 85);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(76, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 4;
		this.labelLine2.Utext = "Comm. Type";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Machine Name";
		this.labelLine1.FieldName = "lblMachineName";
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(4, 33);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(76, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 2;
		this.labelLine1.Utext = "Machine Name";
		this.lbl_MachineID.EditControl = null;
		this.lbl_MachineID.EditValue = "Machine ID";
		this.lbl_MachineID.FieldName = "lblMachineID";
		this.lbl_MachineID.HideWhenEmpty = false;
		this.lbl_MachineID.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_MachineID.Location = new System.Drawing.Point(4, 6);
		this.lbl_MachineID.Name = "lbl_MachineID";
		this.lbl_MachineID.ReadOnly = false;
		this.lbl_MachineID.SettingInVisible = false;
		this.lbl_MachineID.SettingReadonly = false;
		this.lbl_MachineID.ShowUnderLine = true;
		this.lbl_MachineID.Size = new System.Drawing.Size(76, 20);
		this.lbl_MachineID.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_MachineID.TabIndex = 0;
		this.lbl_MachineID.Utext = "Machine ID";
		this.btn_TestConnect.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_TestConnect.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_TestConnect.Appearance.Options.UseBackColor = true;
		this.btn_TestConnect.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_TestConnect.EditControl = null;
		this.btn_TestConnect.EditValue = "Test Connection";
		this.btn_TestConnect.Enabled = true;
		this.btn_TestConnect.FieldName = null;
		this.btn_TestConnect.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_TestConnect.ImageOptions.Image");
		this.btn_TestConnect.IsSearchControl = false;
		this.btn_TestConnect.Location = new System.Drawing.Point(196, 336);
		this.btn_TestConnect.Name = "btn_TestConnect";
		this.btn_TestConnect.ReadOnly = false;
		this.btn_TestConnect.SettingInVisible = false;
		this.btn_TestConnect.SettingReadonly = false;
		this.btn_TestConnect.Size = new System.Drawing.Size(124, 20);
		this.btn_TestConnect.TabIndex = 89;
		this.btn_TestConnect.Text = "Test Connection";
		this.btn_TestConnect.Visible = true;
		this.btn_TestConnect.Click += new System.EventHandler(this.btn_TestConnect_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(587, 359);
		base.Controls.Add(this.btn_TestConnect);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oBiometricConfiguration";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_BiometricConfiguration.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BiometricConfiguration";
		base.ShowInTaskbar = false;
		this.Text = "Biometric Device Configuration";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_BioConfig_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CommType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Active).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1.Panel1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1.Panel2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_McTyp.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chk_Active.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SerialNumber.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_CommKey.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_MachineID.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pnl_Com).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_COMPort.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_BaudRate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.pnl_Tcp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_TCPPort.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_IP.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MachineName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CommType.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
