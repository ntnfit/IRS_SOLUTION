using System;
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
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.HumanResources.Biometric;
using Apzon.Forms.HumanResources.Biometric.FP_CLOCK;
using Apzon.Forms.HumanResources.Biometric.KJTech;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_BiometricEmpMapping : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DeviceManipulator manipulator = new DeviceManipulator();

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gc_EnrollNo;

	private GridColumnEx gc_EnrollName;

	private GridColumnEx gc_EmpNo;

	private GridColumnEx gc_Active;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private SimpleButtonEx btn_Search;

	private LabelLine labelLine1;

	private RepositoryItemButtonEditCFL repositoryItemButtonEditCFL1;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private LookUpEditEx txt_MachineNo;

	private GridColumnEx gc_EmpName;

	private RepositoryItemLookUpEdit luk_EmpName;

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
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_BiometricEmpMapping()
	{
		InitializeComponent();
		FormId = "oBiometricEmpMapping";
		gv_Doc.ItemId = "APZ_OTEM";
		OnLoadAsync += OnOnLoadAsync;
	}

	private async Task OnOnLoadAsync(object sender, EventArgs e)
	{
		try
		{
			await LoadInitForm();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 1;
		try
		{
			await txt_MachineNo.LoadBioMachines();
			await LoadEmployees();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadEmployees()
	{
		try
		{
			DataTable dtEmp = new DataTable
			{
				Columns = 
				{
					{
						"empID",
						typeof(string)
					},
					{
						"FullName",
						typeof(string)
					}
				}
			};
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadEmployees();
			if (dataTable.IsNotNull())
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					DataRow dataRow = dtEmp.NewRow();
					dataRow["empID"] = Function.ToString(item["empID"]);
					dataRow["FullName"] = ((Function.ToString(item["lastName"]) == "") ? "" : (Function.ToString(item["lastName"]) + " ")) + ((Function.ToString(item["middleName"]) == "") ? "" : (Function.ToString(item["middleName"]) + " ")) + ((Function.ToString(item["firstName"]) == "") ? "" : Function.ToString(item["firstName"]));
					dtEmp.Rows.Add(dataRow);
				}
			}
			luk_EmpName.ValueMember = "empID";
			luk_EmpName.DisplayMember = "FullName";
			luk_EmpName.DataSource = dtEmp;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		btn_Search.Enabled = false;
		try
		{
			int machineNo = Function.ParseInt(txt_MachineNo.EditValue);
			DataTable dataTable = (DataTable)txt_MachineNo.Properties.DataSource;
			DataTable userTable = WinFuntions.GetTableStructure("APZ_OTEM");
			userTable.Rows.Clear();
			if (machineNo < 0)
			{
				foreach (DataRow r in dataTable.Rows)
				{
					if (Function.ToString(r["DeviceCode"]).Equals("KJ"))
					{
						int machineNumber = Function.ParseInt(r["MachineID"]);
						Function.ToString(r["MachineName"]);
						string ip = Function.ToString(r["IPAddress"]);
						int port = Function.ParseInt(r["TCPPort"]);
						foreach (DataRow row4 in (await new KJtechManipulator().GetAllUserTable(machineNumber, ip, port)).Rows)
						{
							userTable.ImportRow(row4);
						}
						continue;
					}
					if (Function.ToString(r["DeviceCode"]).Equals("FP"))
					{
						int machineID = Function.ParseInt(r["MachineID"]);
						Function.ToString(r["MachineName"]);
						string ip2 = Function.ToString(r["IPAddress"]);
						int port2 = Function.ParseInt(r["TCPPort"]);
						FpClockManipulator fpClockManipulator = new FpClockManipulator();
						fpClockManipulator.OpenCommPort();
						if (!fpClockManipulator.SetIPAddress(ip2, port2))
						{
							WinCommons.Error(Enums.MessageCode.None, fpClockManipulator.GetErrorString());
							return;
						}
						if (!fpClockManipulator.OpenCommPort())
						{
							WinCommons.Error(Enums.MessageCode.None, fpClockManipulator.GetErrorString());
							return;
						}
						WinCommons._OpenProcessing();
						DataTable allUserID = fpClockManipulator.GetAllUserID(machineID);
						if (!userTable.IsNotNull())
						{
							WinCommons.Error(Enums.MessageCode.None, fpClockManipulator.GetErrorString());
							return;
						}
						fpClockManipulator.CloseCommPort();
						foreach (DataRow row5 in allUserID.Rows)
						{
							userTable.ImportRow(row5);
						}
						WinCommons._CloseProcessing();
						continue;
					}
					ZkemClient objZkeeper = new ZkemClient();
					if (await objZkeeper.ConnectDevice(Function.ParseInt(r["MachineID"])))
					{
						foreach (DataRow row6 in manipulator.GetAllUserTable(objZkeeper, Function.ParseInt(r["MachineID"])).Rows)
						{
							userTable.ImportRow(row6);
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.CannotConnectToDevicePleaseRecheckConfig, "");
					}
				}
			}
			else
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow myRow) => myRow.Field<int>("MachineID") == machineNo);
				if (Function.ToString(dataRow["DeviceCode"]).Equals("KJ"))
				{
					int machineNumber2 = Function.ParseInt(dataRow["MachineID"]);
					Function.ToString(dataRow["MachineName"]);
					string ip3 = Function.ToString(dataRow["IPAddress"]);
					int port3 = Function.ParseInt(dataRow["TCPPort"]);
					userTable = await new KJtechManipulator().GetAllUserTable(machineNumber2, ip3, port3);
				}
				else if (Function.ToString(dataRow["DeviceCode"]).Equals("FP"))
				{
					Function.ParseInt(dataRow["MachineID"]);
					Function.ToString(dataRow["MachineName"]);
					string ip4 = Function.ToString(dataRow["IPAddress"]);
					int port4 = Function.ParseInt(dataRow["TCPPort"]);
					FpClockManipulator fpClockManipulator2 = new FpClockManipulator();
					fpClockManipulator2.OpenCommPort();
					if (!fpClockManipulator2.SetIPAddress(ip4, port4))
					{
						WinCommons.Error(Enums.MessageCode.None, fpClockManipulator2.GetErrorString());
						return;
					}
					if (!fpClockManipulator2.OpenCommPort())
					{
						WinCommons.Error(Enums.MessageCode.None, fpClockManipulator2.GetErrorString());
						return;
					}
					WinCommons._OpenProcessing();
					userTable = fpClockManipulator2.GetAllUserID(machineNo);
					if (!userTable.IsNotNull())
					{
						WinCommons.Error(Enums.MessageCode.None, fpClockManipulator2.GetErrorString());
						return;
					}
					fpClockManipulator2.CloseCommPort();
					WinCommons._CloseProcessing();
				}
				else
				{
					ZkemClient objZkeeper = new ZkemClient();
					if (await objZkeeper.ConnectDevice(machineNo))
					{
						userTable = manipulator.GetAllUserTable(objZkeeper, machineNo);
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.CannotConnectToDevicePleaseRecheckConfig, "");
					}
				}
			}
			DataTable dataSource = await _httpClient.TimeSheetingClient.LoadBiometricMapping(userTable);
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			btn_Search.Enabled = true;
			WinCommons._CloseProcessing();
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveData();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> SaveData()
	{
		try
		{
			DataTable dt = grd_Doc.DataSource as DataTable;
			if (dt.IsNotNull())
			{
				WinCommons._OpenProcessing();
				HttpResult httpResult = await _httpClient.TimeSheetingClient.SaveBiometricMapping(dt);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
		return string.Empty;
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_BiometricEmpMapping));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_EnrollNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_EnrollName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_EmpNo = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemButtonEditCFL1 = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gc_EmpName = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_EmpName = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_Active = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_MachineNo = new Apzon.Controls.Editor.LookUpEditEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_EmpName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MachineNo.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(0, 35);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemCheckEdit1, this.repositoryItemButtonEditCFL1, this.luk_EmpName });
		this.grd_Doc.Size = new System.Drawing.Size(686, 369);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gc_EnrollNo, this.gc_EnrollName, this.gc_EmpNo, this.gc_EmpName, this.gc_Active });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oBiometricEmpMapping";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OTEM";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gc_EnrollNo.Caption = "Enroll No";
		this.gc_EnrollNo.CurrencyDateField = null;
		this.gc_EnrollNo.CurrencyField = null;
		this.gc_EnrollNo.CurrencyFixed = null;
		this.gc_EnrollNo.CurrencyFixedRate = 0.0;
		this.gc_EnrollNo.CurrencyRateField = null;
		this.gc_EnrollNo.FieldName = "EnrollNo";
		this.gc_EnrollNo.Name = "gc_EnrollNo";
		this.gc_EnrollNo.OptionsColumn.ReadOnly = true;
		this.gc_EnrollNo.PrimaryKey = true;
		this.gc_EnrollNo.UnitField = null;
		this.gc_EnrollNo.Visible = true;
		this.gc_EnrollNo.VisibleIndex = 0;
		this.gc_EnrollNo.Width = 87;
		this.gc_EnrollName.Caption = "Enroll Name";
		this.gc_EnrollName.CurrencyDateField = null;
		this.gc_EnrollName.CurrencyField = null;
		this.gc_EnrollName.CurrencyFixed = null;
		this.gc_EnrollName.CurrencyFixedRate = 0.0;
		this.gc_EnrollName.CurrencyRateField = null;
		this.gc_EnrollName.FieldName = "EnrollName";
		this.gc_EnrollName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_EnrollName.Name = "gc_EnrollName";
		this.gc_EnrollName.PrimaryKey = false;
		this.gc_EnrollName.UnitField = null;
		this.gc_EnrollName.Visible = true;
		this.gc_EnrollName.VisibleIndex = 1;
		this.gc_EnrollName.Width = 140;
		this.gc_EmpNo.Caption = "Employee No.";
		this.gc_EmpNo.ColumnEdit = this.repositoryItemButtonEditCFL1;
		this.gc_EmpNo.CurrencyDateField = null;
		this.gc_EmpNo.CurrencyField = null;
		this.gc_EmpNo.CurrencyFixed = null;
		this.gc_EmpNo.CurrencyFixedRate = 0.0;
		this.gc_EmpNo.CurrencyRateField = null;
		this.gc_EmpNo.FieldName = "empNo";
		this.gc_EmpNo.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_EmpNo.Name = "gc_EmpNo";
		this.gc_EmpNo.PrimaryKey = false;
		this.gc_EmpNo.UnitField = null;
		this.gc_EmpNo.Visible = true;
		this.gc_EmpNo.VisibleIndex = 2;
		this.gc_EmpNo.Width = 101;
		this.repositoryItemButtonEditCFL1.AutoHeight = false;
		this.repositoryItemButtonEditCFL1.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.repositoryItemButtonEditCFL1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEditCFL1.ContainerControl = this;
		this.repositoryItemButtonEditCFL1.DisplayMember = "EmpId";
		this.repositoryItemButtonEditCFL1.DisplayMember2 = "";
		this.repositoryItemButtonEditCFL1.LoadDataSourceWhenEnter = false;
		this.repositoryItemButtonEditCFL1.Name = "repositoryItemButtonEditCFL1";
		this.repositoryItemButtonEditCFL1.SeparateCharacterForMulti = ';';
		this.repositoryItemButtonEditCFL1.SeparateString = ", ";
		this.repositoryItemButtonEditCFL1.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Employees;
		this.repositoryItemButtonEditCFL1.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oEmployeesInfo;
		this.repositoryItemButtonEditCFL1.UsingAutoCompleteBox = false;
		this.repositoryItemButtonEditCFL1.UsingDataSource = true;
		this.repositoryItemButtonEditCFL1.ValidateWhenNoDataSource = false;
		this.repositoryItemButtonEditCFL1.ValueMember = "EmpId";
		this.gc_EmpName.Caption = "Employee Name";
		this.gc_EmpName.ColumnEdit = this.luk_EmpName;
		this.gc_EmpName.CurrencyDateField = null;
		this.gc_EmpName.CurrencyField = null;
		this.gc_EmpName.CurrencyFixed = null;
		this.gc_EmpName.CurrencyFixedRate = 0.0;
		this.gc_EmpName.CurrencyRateField = null;
		this.gc_EmpName.FieldName = "empNo";
		this.gc_EmpName.Name = "gc_EmpName";
		this.gc_EmpName.OptionsColumn.AllowEdit = false;
		this.gc_EmpName.OptionsColumn.ReadOnly = true;
		this.gc_EmpName.PrimaryKey = false;
		this.gc_EmpName.UnitField = null;
		this.gc_EmpName.Visible = true;
		this.gc_EmpName.VisibleIndex = 3;
		this.gc_EmpName.Width = 155;
		this.luk_EmpName.AutoHeight = false;
		this.luk_EmpName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.luk_EmpName.Name = "luk_EmpName";
		this.luk_EmpName.NullText = "";
		this.gc_Active.Caption = "Active";
		this.gc_Active.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gc_Active.CurrencyDateField = null;
		this.gc_Active.CurrencyField = null;
		this.gc_Active.CurrencyFixed = null;
		this.gc_Active.CurrencyFixedRate = 0.0;
		this.gc_Active.CurrencyRateField = null;
		this.gc_Active.FieldName = "Active";
		this.gc_Active.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_Active.Name = "gc_Active";
		this.gc_Active.PrimaryKey = false;
		this.gc_Active.UnitField = null;
		this.gc_Active.Width = 86;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.repositoryItemCheckEdit1.ValueChecked = "Y";
		this.repositoryItemCheckEdit1.ValueUnchecked = "N";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(206, 4);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(81, 23);
		this.btn_Search.TabIndex = 2;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Machine No.";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(0, 8);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 3;
		this.labelLine1.Utext = "Machine No.";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 410);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 95;
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
		this.btnCancel.Location = new System.Drawing.Point(70, 410);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 94;
		this.btnCancel.Text = "Cancel";
		this.txt_MachineNo.EditControl = null;
		this.txt_MachineNo.FieldName = null;
		this.txt_MachineNo.IsSearchControl = true;
		this.txt_MachineNo.Location = new System.Drawing.Point(70, 5);
		this.txt_MachineNo.Name = "txt_MachineNo";
		this.txt_MachineNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_MachineNo.Properties.NullText = "";
		this.txt_MachineNo.SettingInVisible = false;
		this.txt_MachineNo.SettingReadonly = false;
		this.txt_MachineNo.Size = new System.Drawing.Size(130, 20);
		this.txt_MachineNo.TabIndex = 96;
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(686, 438);
		base.Controls.Add(this.txt_MachineNo);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.grd_Doc);
		base.FormId = "oBiometricEmpMapping";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BiometricEmpMapping";
		this.Text = "Biometric Employee Mapping";
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEditCFL1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_EmpName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_MachineNo.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
