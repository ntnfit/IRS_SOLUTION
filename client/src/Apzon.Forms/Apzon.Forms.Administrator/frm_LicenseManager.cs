using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using Apzon.Entities.Models.Common;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Administrator;

public class frm_LicenseManager : XtraFormEx
{
	private DataTable _listLicense;

	private bool _changeRow = true;

	private int _beforeChange;

	private static frm_LicenseManager _instance;

	private static readonly object PadLock = new object();

	private IContainer components;

	private SimpleButtonEx btnOK;

	private SimpleButtonEx btnCancel;

	private XtraTabPageEx tabLicense;

	private XtraTabPageEx tabImportLicense;

	private PanelControl panelControl1;

	private SimpleButtonEx btnCancel1;

	private SimpleButtonEx btnLoadLicenseFile;

	private ButtonEditEx txtLicenseFile;

	private TextEditEx txtServerIP;

	private TextEditEx txtHardwareKey;

	private LabelLine labHardwareKey;

	private LabelLine labLicenseFile;

	private LabelLine labServerIP;

	private XtraTabControlEx xtraTabControl1;

	private PanelControl panelControl2;

	private TextBoxEx txtLicense;

	private TextBoxEx txtDBName;

	private LabelLine lbDbName;

	private LabelLine labLicenseNumber;

	private LabelLine labValidUtil;

	private RepositoryItemCheckEdit rUse;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grdType;

	private GridViewEx gvType;

	private GridColumnEx gcType;

	private GridColumnEx gcCount;

	private GridSplitContainer gridSplitContainer2;

	private GridColumnEx gcLicenseUse;

	private GridColumnEx gcExpiredDate;

	private LookUpEdit txtLicenseType;

	private TextBoxEx txtDomain;

	private LabelLine lbDomain;

	private SplitContainer splitContainer1;

	private GridControlEx grdUser;

	private GridViewEx gvUser;

	private GridColumnEx gcUser;

	private GridColumnEx gcUse;

	private RepositoryItemCheckEdit rUser;

	private GridSplitContainer gridSplitContainer1;

	private GridControlEx grdMapUser;

	private GridViewEx gvMapUser;

	private GridColumnEx gcMapUserDatabase;

	private GridColumnEx gcMapUserUser;

	private LabelLine labelLine1;

	private RepositoryItemLookUpEdit rLicenseType;

	public static frm_LicenseManager Instance
	{
		get
		{
			if (_instance == null || _instance.IsDisposed)
			{
				lock (PadLock)
				{
					if (_instance == null || _instance.IsDisposed)
					{
						_instance = new frm_LicenseManager();
					}
				}
			}
			return _instance;
		}
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOK.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public static bool IsOpening()
	{
		if (_instance != null)
		{
			return !_instance.IsDisposed;
		}
		return false;
	}

	public frm_LicenseManager()
	{
		InitializeComponent();
		OnLoadAsync += frm_LicenseManager_Load;
		FormMode = Enums.FormViewModeType.View;
	}

	private async void btnLoadLicenseFile_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			if (string.IsNullOrEmpty(txtServerIP.Text))
			{
				WinCommons.Error(Enums.MessageCode.NotEnteredIpServerYet, "");
				WinCommons._CloseProcessing();
				EnableDisableControl(isEnable: true);
				return;
			}
			if (string.IsNullOrEmpty(txtHardwareKey.Text))
			{
				WinCommons.Error(Enums.MessageCode.NotEnteredHardwareKeyYet, "");
				WinCommons._CloseProcessing();
				EnableDisableControl(isEnable: true);
				return;
			}
			if (string.IsNullOrEmpty(txtLicenseFile.Text))
			{
				WinCommons.Error(Enums.MessageCode.DoNotChooseLicenseFile, "");
				WinCommons._CloseProcessing();
				EnableDisableControl(isEnable: true);
				return;
			}
			ReadFile(txtLicenseFile.Text, out var file);
			string text = await Save(file);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.ImportLicenseSuccess, "");
				return;
			}
			WinCommons.Error(Enums.MessageCode.None, text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadLicenseFile, "");
		}
		EnableDisableControl(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private void txtLicenseFile_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				Multiselect = false
			};
			if (openFileDialog.ShowDialog() == DialogResult.OK)
			{
				string fileName = openFileDialog.FileName;
				if (!string.IsNullOrEmpty(fileName))
				{
					txtLicenseFile.Text = fileName;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadLicenseFile, "");
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async Task frm_LicenseManager_Load(object sender, EventArgs e)
	{
		try
		{
			txtDomain.Text = GlobalData.Domain;
			txtDBName.Text = GlobalData.DatabaseName;
			LoadDataLicenseTypeControl();
			txtServerIP.Text = Configuration.ApiServer;
			TextEditEx textEditEx = txtHardwareKey;
			textEditEx.Text = await GetHardwareKey();
			gcExpiredDate.DisplayFormat.FormatType = FormatType.DateTime;
			gcExpiredDate.DisplayFormat.FormatString = GlobalData.DateTimeFormat;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void xtraTabControl1_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		gvType.IsLoadingData = true;
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			if (e.Page == tabLicense)
			{
				LoadLicense();
				FormMode = Enums.FormViewModeType.View;
				btnCancel.Visible = true;
				btnOK.Visible = true;
				GetLicenseUser(Function.ToString(gvType.GetFocusedRowCellValue(gcType)));
				LoadUser();
			}
			else
			{
				txtLicenseFile.Text = "";
				_beforeChange = 0;
				_listLicense = null;
				FormMode = Enums.FormViewModeType.View;
				grdType.DataSource = null;
				grdUser.DataSource = null;
				grdMapUser.DataSource = null;
				grdType.RefreshDataSource();
				grdUser.RefreshDataSource();
				btnCancel.Visible = false;
				btnOK.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.CannotLoadLicenseFile, "");
		}
		EnableDisableControl(isEnable: true);
		gvType.IsLoadingData = false;
		WinCommons._CloseProcessing();
	}

	private void xtraTabControl1_SelectedPageChanging(object sender, TabPageChangingEventArgs e)
	{
		if (gvUser.IsLoadingData)
		{
			e.Cancel = true;
		}
	}

	private void rUser_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (Function.ToString(gvUser.GetFocusedRowCellValue(gcUser)).Equals(GlobalData.UserName))
			{
				e.Cancel = true;
				return;
			}
			if (Function.ParseInt(gvType.GetFocusedRowCellValue("Count")) == 0)
			{
				WinCommons.Warning(Enums.MessageCode.LicenseIsZero, "", Function.ToString(gvType.GetFocusedRowCellValue("LicenseType")));
				e.Cancel = true;
				return;
			}
			if (Function.ParseInt(gvType.GetFocusedRowCellValue("Count")) < Function.ParseInt(gvType.GetFocusedRowCellValue("Use")))
			{
				WinCommons.Warning(Enums.MessageCode.MaxLiceseNumber, "", Function.ToString(Function.ParseInt(gvType.GetFocusedRowCellValue("Count"))));
				e.Cancel = true;
				return;
			}
			if (Function.ToString(e.NewValue).Equals("Y"))
			{
				bool flag = false;
				if (_listLicense != null && _listLicense.Rows.Count > 0)
				{
					for (int i = 0; i < _listLicense.Rows.Count; i++)
					{
						if (Function.ToString(_listLicense.Rows[i]["User"]) == Function.ToString(gvUser.GetFocusedRowCellValue("User")))
						{
							flag = true;
							break;
						}
					}
					if (Function.ParseInt(gvType.GetFocusedRowCellValue("Count")) == Function.ParseInt(gvType.GetFocusedRowCellValue("Use")) && !flag)
					{
						WinCommons.Warning(Enums.MessageCode.MaxLiceseNumber, "", Function.ToString(Function.ParseInt(gvType.GetFocusedRowCellValue("Count"))));
						e.Cancel = true;
						return;
					}
				}
				if (_listLicense == null || _listLicense.Rows.Count == 0)
				{
					_listLicense = new DataTable();
					_listLicense.Columns.Add("User", typeof(string));
					_listLicense.Columns.Add("DataBase", typeof(string));
				}
				DataRow dataRow = _listLicense.NewRow();
				dataRow["User"] = Function.ToString(gvUser.GetFocusedRowCellValue("User"));
				dataRow["DataBase"] = GlobalData.DatabaseName;
				_listLicense.Rows.Add(dataRow);
				gvType.SetFocusedRowCellValue("Use", _listLicense.DefaultView.ToTable(true, "User").Rows.Count);
			}
			else
			{
				for (int j = 0; j < _listLicense.Rows.Count; j++)
				{
					if (Function.ToString(_listLicense.Rows[j]["User"]) == Function.ToString(gvUser.GetFocusedRowCellValue("User")) && Function.ToString(_listLicense.Rows[j]["DataBase"]) == GlobalData.DatabaseName)
					{
						_listLicense.Rows.Remove(_listLicense.Rows[j]);
						break;
					}
				}
				gvType.SetFocusedRowCellValue("Use", _listLicense.DefaultView.ToTable(true, "User").Rows.Count);
			}
			grdType.DataSourceTable.AcceptChanges();
			grdType.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnOK_Click(object sender, EventArgs e)
	{
		gvType.IsLoadingData = true;
		try
		{
			EnableDisableControl(isEnable: false);
			if (FormMode != Enums.FormViewModeType.Update)
			{
				Close();
				WinCommons._CloseProcessing();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				EnableDisableControl(isEnable: true);
				return;
			}
			WinCommons._OpenProcessing();
			DataTable dataSourceTable = grdUser.DataSourceTable;
			if (dataSourceTable != null)
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("InstallNumber", typeof(string));
				dataTable.Columns.Add("User", typeof(string));
				dataTable.Columns.Add("DataBase", typeof(string));
				dataTable.Columns.Add("LicenseType", typeof(string));
				for (int i = 0; i < dataSourceTable.Rows.Count; i++)
				{
					DataRow dataRow = dataTable.NewRow();
					if (Function.ToString(dataSourceTable.Rows[i]["Use"]) == "Y")
					{
						dataRow["InstallNumber"] = txtLicense.Text;
						dataRow["DataBase"] = GlobalData.DatabaseName;
						dataRow["User"] = dataSourceTable.Rows[i]["User"];
						dataRow["LicenseType"] = Function.ToString(gvType.GetFocusedRowCellValue(gcType));
						dataTable.Rows.Add(dataRow);
					}
				}
				string text = await SaveUser(dataTable);
				if (string.IsNullOrEmpty(text))
				{
					FormMode = Enums.FormViewModeType.View;
					WinCommons.Success(Enums.MessageCode.Success, "");
					GetLicenseUser(Function.ToString(gvType.GetFocusedRowCellValue(gcType)));
					LoadUser();
					xtraTabControl1.SelectedTabPage = tabLicense;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControl(isEnable: true);
		gvType.IsLoadingData = false;
		WinCommons._CloseProcessing();
	}

	private void gvUser_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvType_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvType_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		EnableDisableControl(isEnable: false);
		try
		{
			if (xtraTabControl1.SelectedTabPage == tabImportLicense)
			{
				return;
			}
			gvUser.IsLoadingData = true;
			if (!string.IsNullOrEmpty(txtLicense.Text) && _changeRow)
			{
				if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
				{
					_changeRow = false;
					gvType.FocusedRowHandle = e.PrevFocusedRowHandle;
					_changeRow = true;
					EnableDisableControl(isEnable: true);
					return;
				}
				WinCommons._OpenProcessing();
				_changeRow = true;
				if (gvType.FocusedRowHandle >= 0)
				{
					gvType.SetRowCellValue(e.PrevFocusedRowHandle, "Use", _beforeChange);
				}
				GetLicenseUser(Function.ToString(gvType.GetFocusedRowCellValue(gcType)));
				LoadUser();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControl(isEnable: true);
		gvUser.IsLoadingData = false;
		WinCommons._CloseProcessing();
	}

	public async Task MODE_FIND()
	{
		try
		{
			if (xtraTabControl1.SelectedTabPage == tabLicense)
			{
				btnCancel.Visible = true;
				btnOK.Visible = true;
				LoadLicense();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadUser()
	{
		WinCommons._OpenProcessing();
		using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LicenseManager"))
		{
			DataTable dataTable = httpClientHelper.Get("GetUser");
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				if (_listLicense == null || _listLicense.Rows.Count == 0)
				{
					grdUser.DataSource = dataTable;
					grdUser.DataSourceTable.AcceptChanges();
					grdUser.RefreshDataSource();
					WinCommons._CloseProcessing();
					return;
				}
				dataTable.BeginLoadData();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					for (int j = 0; j < _listLicense.Rows.Count; j++)
					{
						if (Function.ToString(dataTable.Rows[i]["User"]) == Function.ToString(_listLicense.Rows[j]["User"]) && GlobalData.DatabaseName == Function.ToString(_listLicense.Rows[j]["DataBase"]))
						{
							dataTable.Rows[i]["Use"] = 'Y';
							break;
						}
					}
				}
				dataTable.AcceptChanges();
				dataTable.EndLoadData();
				grdUser.DataSource = dataTable;
				grdUser.RefreshDataSource();
			}
		}
		WinCommons._CloseProcessing();
	}

	private async Task<string> SaveUser(DataTable dt)
	{
		using HttpClientHelper<string> helperLicense = new HttpClientHelper<string>(Configuration.ApiServer, "LicenseManager");
		return await helperLicense.PostAsync("SaveUser", new Tuple<DataTable, string, string>(dt, GlobalData.DatabaseName, txtLicense.Text));
	}

	private void LoadLicense()
	{
		try
		{
			using HttpClientHelper<Tuple<string, string, string, DataTable, List<Common.Table_Code>>> httpClientHelper = new HttpClientHelper<Tuple<string, string, string, DataTable, List<Common.Table_Code>>>(Configuration.ApiServer, "LicenseManager");
			Tuple<string, string, string, DataTable, List<Common.Table_Code>> tuple = httpClientHelper.Get("GetLicense", new
			{
				database = GlobalData.DatabaseName,
				versionAp = Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1)
			});
			if (tuple == null)
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadLicenseFile, "");
				return;
			}
			if (!string.IsNullOrEmpty(tuple.Item1))
			{
				WinCommons.Error(Enums.MessageCode.None, tuple.Item1);
				return;
			}
			rLicenseType.DataSource = tuple.Item5;
			txtLicense.Text = tuple.Item2;
			txtLicenseType.EditValue = tuple.Item3;
			grdType.DataSource = tuple.Item4;
			grdType.DataSourceTable.AcceptChanges();
			grdType.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetLicenseUser(string type)
	{
		using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LicenseManager");
		_listLicense = httpClientHelper.Post("GetLicenseUser", new Tuple<string, string, string>(txtLicense.Text, type, GlobalData.DatabaseName));
		if (_listLicense != null && _listLicense.Rows.Count > 0)
		{
			int count = _listLicense.DefaultView.ToTable(true, "User").Rows.Count;
			if (count > Function.ParseInt(gvType.GetFocusedRowCellValue("Count")))
			{
				WinCommons.Error(Enums.MessageCode.YouHaveChangedDatabasePleaseReloginAndMapLicense, "");
				DeleteAllLicenseUser();
			}
			_beforeChange = count;
			gvType.SetFocusedRowCellValue("Use", count);
			grdType.DataSourceTable.AcceptChanges();
			grdType.RefreshDataSource();
			grdMapUser.DataSource = _listLicense.Copy();
		}
		else
		{
			_beforeChange = 0;
			gvType.SetFocusedRowCellValue("Use", 0);
			grdType.DataSourceTable.AcceptChanges();
			grdType.RefreshDataSource();
			grdMapUser.DataSource = null;
		}
		grdMapUser.RefreshDataSource();
	}

	private void DeleteAllLicenseUser()
	{
		try
		{
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "DeleteAllLicenseUser");
			httpClientHelper.Get("DeleteAllLicenseUser", new
			{
				database = GlobalData.DatabaseName
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ReadFile(string fileName, out string file)
	{
		using StreamReader streamReader = new StreamReader(fileName);
		file = streamReader.ReadToEnd();
	}

	private async Task<string> Save(string file)
	{
		using HttpClientHelper<string> helperLicense = new HttpClientHelper<string>(Configuration.ApiServer, "LicenseManager");
		return await helperLicense.PostAsync("Save", new Tuple<string, string, string>(file, $"{Function.ConvertDoubleToStringEnUs(Versions.ApplicationVersion, 2, 1)}.{Function.ConvertDoubleToStringEnUs(Versions.BuildVersion, 0, 3)}", GlobalData.DatabaseName));
	}

	private async Task<string> GetHardwareKey()
	{
		using HttpClientHelper<string> helperLicense = new HttpClientHelper<string>(Configuration.ApiServer, "LicenseManager");
		return await helperLicense.GetAsync("GetHardwareKey", new
		{
			database = GlobalData.DatabaseName
		});
	}

	private void LoadDataLicenseTypeControl()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Productive",
					Code = "P"
				},
				new Common.Table_Code
				{
					Name = "Test",
					Code = "T"
				}
			};
			txtLicenseType.Properties.DataSource = dataSource;
			txtLicenseType.Properties.DisplayMember = "Name";
			txtLicenseType.Properties.ValueMember = "Code";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			txtLicenseFile.ReadOnly = !isEnable;
			if (!isEnable)
			{
				btnCancel.Enabled = false;
			}
			else if (xtraTabControl1.SelectedTabPage != tabImportLicense)
			{
				btnCancel.Enabled = true;
			}
			btnCancel1.Enabled = isEnable;
			btnLoadLicenseFile.Enabled = isEnable;
			if (!isEnable)
			{
				btnOK.Enabled = false;
			}
			else if (xtraTabControl1.SelectedTabPage != tabImportLicense)
			{
				btnOK.Enabled = true;
			}
			gvUser.OptionsBehavior.ReadOnly = !isEnable;
			if (FormAuth != Enums.FormAuthorities.Full)
			{
				btnOK.Enabled = false;
				btnLoadLicenseFile.Enabled = false;
				gvUser.OptionsBehavior.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvUser_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		if (Function.ToString(gvUser.GetRowCellValue(e.RowHandle, gcUser)).Equals(GlobalData.UserName))
		{
			e.Appearance.BackColor = Color.Silver;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_LicenseManager));
		this.rUse = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tabLicense = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.txtDomain = new Apzon.Controls.Editor.TextBoxEx();
		this.lbDomain = new Apzon.Controls.Controls.LabelLine();
		this.txtLicenseType = new DevExpress.XtraEditors.LookUpEdit();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.gridSplitContainer2 = new DevExpress.XtraGrid.GridSplitContainer();
		this.grdType = new Apzon.Controls.Grid.GridControlEx();
		this.gvType = new Apzon.Controls.Grid.GridViewEx();
		this.gcType = new Apzon.Controls.Grid.GridColumnEx();
		this.rLicenseType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcCount = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLicenseUse = new Apzon.Controls.Grid.GridColumnEx();
		this.gcExpiredDate = new Apzon.Controls.Grid.GridColumnEx();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.gridSplitContainer1 = new DevExpress.XtraGrid.GridSplitContainer();
		this.grdUser = new Apzon.Controls.Grid.GridControlEx();
		this.gvUser = new Apzon.Controls.Grid.GridViewEx();
		this.gcUser = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUse = new Apzon.Controls.Grid.GridColumnEx();
		this.rUser = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.grdMapUser = new Apzon.Controls.Grid.GridControlEx();
		this.gvMapUser = new Apzon.Controls.Grid.GridViewEx();
		this.gcMapUserDatabase = new Apzon.Controls.Grid.GridColumnEx();
		this.gcMapUserUser = new Apzon.Controls.Grid.GridColumnEx();
		this.labValidUtil = new Apzon.Controls.Controls.LabelLine();
		this.txtLicense = new Apzon.Controls.Editor.TextBoxEx();
		this.txtDBName = new Apzon.Controls.Editor.TextBoxEx();
		this.lbDbName = new Apzon.Controls.Controls.LabelLine();
		this.labLicenseNumber = new Apzon.Controls.Controls.LabelLine();
		this.tabImportLicense = new Apzon.Controls.Controls.XtraTabPageEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btnCancel1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnLoadLicenseFile = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtLicenseFile = new Apzon.Controls.Editor.ButtonEditEx();
		this.txtServerIP = new Apzon.Controls.Editor.TextEditEx();
		this.txtHardwareKey = new Apzon.Controls.Editor.TextEditEx();
		this.labHardwareKey = new Apzon.Controls.Controls.LabelLine();
		this.labLicenseFile = new Apzon.Controls.Controls.LabelLine();
		this.labServerIP = new Apzon.Controls.Controls.LabelLine();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		((System.ComponentModel.ISupportInitialize)this.rUse).BeginInit();
		this.tabLicense.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtLicenseType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer2).BeginInit();
		this.gridSplitContainer2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLicenseType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer1).BeginInit();
		this.gridSplitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdUser).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvUser).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUser).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdMapUser).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvMapUser).BeginInit();
		this.tabImportLicense.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txtLicenseFile.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtServerIP.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtHardwareKey.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		base.SuspendLayout();
		this.rUse.AutoHeight = false;
		this.rUse.Name = "rUse";
		this.rUse.ValueChecked = 'Y';
		this.rUse.ValueUnchecked = 'N';
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(835, 373);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 20);
		this.btnOK.TabIndex = 9;
		this.btnOK.Text = "OK";
		this.btnOK.Visible = false;
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(913, 373);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 10;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Visible = false;
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.tabLicense.Controls.Add(this.panelControl2);
		this.tabLicense.EditControl = null;
		this.tabLicense.EditValue = "License";
		this.tabLicense.FieldName = null;
		this.tabLicense.IsSearchControl = false;
		this.tabLicense.Name = "tabLicense";
		this.tabLicense.ReadOnly = false;
		this.tabLicense.SettingInVisible = false;
		this.tabLicense.SettingReadonly = false;
		this.tabLicense.Size = new System.Drawing.Size(981, 342);
		this.tabLicense.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabLicense.Text = "License";
		this.panelControl2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl2.Controls.Add(this.txtDomain);
		this.panelControl2.Controls.Add(this.lbDomain);
		this.panelControl2.Controls.Add(this.txtLicenseType);
		this.panelControl2.Controls.Add(this.splitContainerControl1);
		this.panelControl2.Controls.Add(this.labValidUtil);
		this.panelControl2.Controls.Add(this.txtLicense);
		this.panelControl2.Controls.Add(this.txtDBName);
		this.panelControl2.Controls.Add(this.lbDbName);
		this.panelControl2.Controls.Add(this.labLicenseNumber);
		this.panelControl2.Location = new System.Drawing.Point(-1, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(978, 339);
		this.panelControl2.TabIndex = 0;
		this.txtDomain.BackColor = System.Drawing.Color.White;
		this.txtDomain.EditControl = null;
		this.txtDomain.EditValue = "";
		this.txtDomain.FieldName = null;
		this.txtDomain.IsSearchControl = false;
		this.txtDomain.Location = new System.Drawing.Point(136, 37);
		this.txtDomain.Name = "txtDomain";
		this.txtDomain.ReadOnly = true;
		this.txtDomain.SettingInVisible = false;
		this.txtDomain.SettingReadonly = false;
		this.txtDomain.Size = new System.Drawing.Size(200, 21);
		this.txtDomain.TabIndex = 5;
		this.lbDomain.EditControl = null;
		this.lbDomain.EditValue = "Domain";
		this.lbDomain.FieldName = null;
		this.lbDomain.HideWhenEmpty = false;
		this.lbDomain.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbDomain.Location = new System.Drawing.Point(10, 40);
		this.lbDomain.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbDomain.Name = "lbDomain";
		this.lbDomain.ReadOnly = false;
		this.lbDomain.SettingInVisible = false;
		this.lbDomain.SettingReadonly = false;
		this.lbDomain.ShowUnderLine = true;
		this.lbDomain.Size = new System.Drawing.Size(120, 20);
		this.lbDomain.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbDomain.TabIndex = 16;
		this.lbDomain.Utext = "Domain";
		this.txtLicenseType.Location = new System.Drawing.Point(734, 10);
		this.txtLicenseType.Name = "txtLicenseType";
		this.txtLicenseType.Properties.NullText = "";
		this.txtLicenseType.Properties.ReadOnly = true;
		this.txtLicenseType.Size = new System.Drawing.Size(108, 20);
		this.txtLicenseType.TabIndex = 8;
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Flat;
		this.splitContainerControl1.Location = new System.Drawing.Point(4, 67);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.gridSplitContainer2);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitContainer1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(970, 269);
		this.splitContainerControl1.SplitterPosition = 472;
		this.splitContainerControl1.TabIndex = 14;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.gridSplitContainer2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridSplitContainer2.Grid = this.grdType;
		this.gridSplitContainer2.Location = new System.Drawing.Point(0, 3);
		this.gridSplitContainer2.Name = "gridSplitContainer2";
		this.gridSplitContainer2.Panel1.Controls.Add(this.grdType);
		this.gridSplitContainer2.Size = new System.Drawing.Size(471, 265);
		this.gridSplitContainer2.TabIndex = 0;
		this.grdType.AllowPasteExcel = false;
		this.grdType.ClipboardData = null;
		this.grdType.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdType.Location = new System.Drawing.Point(0, 0);
		this.grdType.MainView = this.gvType;
		this.grdType.Name = "grdType";
		this.grdType.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rLicenseType });
		this.grdType.Size = new System.Drawing.Size(471, 265);
		this.grdType.TabIndex = 0;
		this.grdType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvType });
		this.gvType.Appearance.FocusedCell.BackColor = System.Drawing.Color.Linen;
		this.gvType.Appearance.FocusedCell.Options.UseBackColor = true;
		this.gvType.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvType.ColumnKey = null;
		this.gvType.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gcType, this.gcCount, this.gcLicenseUse, this.gcExpiredDate });
		this.gvType.DisableFormSetting = true;
		this.gvType.DisplayException = false;
		this.gvType.EditColumn = null;
		this.gvType.FieldCurrency = null;
		this.gvType.FieldRate = null;
		this.gvType.FormId = "oLIcenseManager";
		this.gvType.GridControl = this.grdType;
		this.gvType.HasFormSetting = true;
		this.gvType.IgnoreFilter = false;
		this.gvType.IndexEditRow = -1;
		this.gvType.IndicatorWidth = 30;
		this.gvType.IsApzTable = false;
		this.gvType.IsInsertNewRow = false;
		this.gvType.IsLoadingData = false;
		this.gvType.IsMappingContextAction = false;
		this.gvType.ItemId = "License";
		this.gvType.LstColumnAllowBatch = null;
		this.gvType.LstColumnNotAllowBatch = null;
		this.gvType.LstColumnNotEditable = null;
		this.gvType.LstColumNotSetting = null;
		this.gvType.LstOldUDF = null;
		this.gvType.Name = "gvType";
		this.gvType.OptionsBehavior.ReadOnly = true;
		this.gvType.OptionsCustomization.AllowColumnMoving = false;
		this.gvType.OptionsCustomization.AllowFilter = false;
		this.gvType.OptionsCustomization.AllowGroup = false;
		this.gvType.OptionsCustomization.AllowSort = false;
		this.gvType.OptionsFind.AllowFindPanel = false;
		this.gvType.OptionsView.ColumnAutoWidth = false;
		this.gvType.OptionsView.ShowGroupPanel = false;
		this.gvType.ReadOnly = true;
		this.gvType.SettingReadonly = false;
		this.gvType.UseBatchMod = false;
		this.gvType.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvType_CustomDrawRowIndicator);
		this.gvType.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvType_FocusedRowChanged);
		this.gcType.Caption = "License Type";
		this.gcType.ColumnEdit = this.rLicenseType;
		this.gcType.CurrencyDateField = null;
		this.gcType.CurrencyField = null;
		this.gcType.CurrencyFixed = null;
		this.gcType.CurrencyFixedRate = 0.0;
		this.gcType.CurrencyRateField = null;
		this.gcType.FieldName = "LicenseType";
		this.gcType.Name = "gcType";
		this.gcType.OptionsColumn.AllowEdit = false;
		this.gcType.PrimaryKey = false;
		this.gcType.UnitField = null;
		this.gcType.Visible = true;
		this.gcType.VisibleIndex = 0;
		this.gcType.Width = 133;
		this.rLicenseType.AutoHeight = false;
		this.rLicenseType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLicenseType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rLicenseType.DisplayMember = "Name";
		this.rLicenseType.Name = "rLicenseType";
		this.rLicenseType.NullText = "";
		this.rLicenseType.ShowHeader = false;
		this.rLicenseType.ValueMember = "Code";
		this.gcCount.Caption = "Count User";
		this.gcCount.CurrencyDateField = null;
		this.gcCount.CurrencyField = null;
		this.gcCount.CurrencyFixed = null;
		this.gcCount.CurrencyFixedRate = 0.0;
		this.gcCount.CurrencyRateField = null;
		this.gcCount.FieldName = "Count";
		this.gcCount.Name = "gcCount";
		this.gcCount.OptionsColumn.AllowEdit = false;
		this.gcCount.PrimaryKey = false;
		this.gcCount.UnitField = null;
		this.gcCount.Visible = true;
		this.gcCount.VisibleIndex = 1;
		this.gcCount.Width = 83;
		this.gcLicenseUse.Caption = "Use";
		this.gcLicenseUse.CurrencyDateField = null;
		this.gcLicenseUse.CurrencyField = null;
		this.gcLicenseUse.CurrencyFixed = null;
		this.gcLicenseUse.CurrencyFixedRate = 0.0;
		this.gcLicenseUse.CurrencyRateField = null;
		this.gcLicenseUse.FieldName = "Use";
		this.gcLicenseUse.Name = "gcLicenseUse";
		this.gcLicenseUse.OptionsColumn.AllowEdit = false;
		this.gcLicenseUse.PrimaryKey = false;
		this.gcLicenseUse.UnitField = null;
		this.gcLicenseUse.Visible = true;
		this.gcLicenseUse.VisibleIndex = 2;
		this.gcExpiredDate.Caption = "Expired Date";
		this.gcExpiredDate.CurrencyDateField = null;
		this.gcExpiredDate.CurrencyField = null;
		this.gcExpiredDate.CurrencyFixed = null;
		this.gcExpiredDate.CurrencyFixedRate = 0.0;
		this.gcExpiredDate.CurrencyRateField = null;
		this.gcExpiredDate.FieldName = "ExpiredDate";
		this.gcExpiredDate.Name = "gcExpiredDate";
		this.gcExpiredDate.OptionsColumn.AllowEdit = false;
		this.gcExpiredDate.PrimaryKey = false;
		this.gcExpiredDate.UnitField = null;
		this.gcExpiredDate.Visible = true;
		this.gcExpiredDate.VisibleIndex = 3;
		this.gcExpiredDate.Width = 120;
		this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainer1.Location = new System.Drawing.Point(0, 0);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Panel1.Controls.Add(this.gridSplitContainer1);
		this.splitContainer1.Panel2.Controls.Add(this.labelLine1);
		this.splitContainer1.Panel2.Controls.Add(this.grdMapUser);
		this.splitContainer1.Size = new System.Drawing.Size(484, 265);
		this.splitContainer1.SplitterDistance = 190;
		this.splitContainer1.TabIndex = 0;
		this.gridSplitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gridSplitContainer1.Grid = this.grdUser;
		this.gridSplitContainer1.Location = new System.Drawing.Point(0, 0);
		this.gridSplitContainer1.Name = "gridSplitContainer1";
		this.gridSplitContainer1.Panel1.Controls.Add(this.grdUser);
		this.gridSplitContainer1.Size = new System.Drawing.Size(190, 265);
		this.gridSplitContainer1.TabIndex = 0;
		this.grdUser.AllowPasteExcel = false;
		this.grdUser.ClipboardData = null;
		this.grdUser.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdUser.Location = new System.Drawing.Point(0, 0);
		this.grdUser.MainView = this.gvUser;
		this.grdUser.Name = "grdUser";
		this.grdUser.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rUser });
		this.grdUser.Size = new System.Drawing.Size(190, 265);
		this.grdUser.TabIndex = 1;
		this.grdUser.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvUser });
		this.gvUser.Appearance.FocusedCell.BackColor = System.Drawing.Color.Linen;
		this.gvUser.Appearance.FocusedCell.Options.UseBackColor = true;
		this.gvUser.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvUser.ColumnKey = null;
		this.gvUser.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcUser, this.gcUse });
		this.gvUser.DisableFormSetting = true;
		this.gvUser.DisplayException = false;
		this.gvUser.EditColumn = null;
		this.gvUser.FieldCurrency = null;
		this.gvUser.FieldRate = null;
		this.gvUser.FormId = "oLIcenseManager";
		this.gvUser.GridControl = this.grdUser;
		this.gvUser.HasFormSetting = true;
		this.gvUser.IgnoreFilter = false;
		this.gvUser.IndexEditRow = -1;
		this.gvUser.IndicatorWidth = 30;
		this.gvUser.IsApzTable = false;
		this.gvUser.IsInsertNewRow = false;
		this.gvUser.IsLoadingData = false;
		this.gvUser.IsMappingContextAction = false;
		this.gvUser.ItemId = "OUSR";
		this.gvUser.LstColumnAllowBatch = null;
		this.gvUser.LstColumnNotAllowBatch = null;
		this.gvUser.LstColumnNotEditable = null;
		this.gvUser.LstColumNotSetting = null;
		this.gvUser.LstOldUDF = null;
		this.gvUser.Name = "gvUser";
		this.gvUser.OptionsCustomization.AllowQuickHideColumns = false;
		this.gvUser.OptionsView.ShowGroupPanel = false;
		this.gvUser.ReadOnly = false;
		this.gvUser.SettingReadonly = false;
		this.gvUser.UseBatchMod = false;
		this.gvUser.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvUser_CustomDrawRowIndicator);
		this.gvUser.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvUser_RowCellStyle);
		this.gcUser.Caption = "User Name";
		this.gcUser.CurrencyDateField = null;
		this.gcUser.CurrencyField = null;
		this.gcUser.CurrencyFixed = null;
		this.gcUser.CurrencyFixedRate = 0.0;
		this.gcUser.CurrencyRateField = null;
		this.gcUser.FieldName = "User";
		this.gcUser.Name = "gcUser";
		this.gcUser.OptionsColumn.AllowEdit = false;
		this.gcUser.OptionsColumn.ReadOnly = true;
		this.gcUser.PrimaryKey = false;
		this.gcUser.UnitField = null;
		this.gcUser.Visible = true;
		this.gcUser.VisibleIndex = 0;
		this.gcUse.Caption = "Used";
		this.gcUse.ColumnEdit = this.rUser;
		this.gcUse.CurrencyDateField = null;
		this.gcUse.CurrencyField = null;
		this.gcUse.CurrencyFixed = null;
		this.gcUse.CurrencyFixedRate = 0.0;
		this.gcUse.CurrencyRateField = null;
		this.gcUse.FieldName = "Use";
		this.gcUse.Name = "gcUse";
		this.gcUse.PrimaryKey = false;
		this.gcUse.UnitField = null;
		this.gcUse.Visible = true;
		this.gcUse.VisibleIndex = 1;
		this.rUser.AutoHeight = false;
		this.rUser.Name = "rUser";
		this.rUser.ValueChecked = "Y";
		this.rUser.ValueUnchecked = "N";
		this.rUser.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rUser_EditValueChanging);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "License User In Database";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 11);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(274, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "License User In Database";
		this.grdMapUser.AllowPasteExcel = false;
		this.grdMapUser.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdMapUser.ClipboardData = null;
		this.grdMapUser.Location = new System.Drawing.Point(3, 37);
		this.grdMapUser.MainView = this.gvMapUser;
		this.grdMapUser.Name = "grdMapUser";
		this.grdMapUser.Size = new System.Drawing.Size(286, 226);
		this.grdMapUser.TabIndex = 0;
		this.grdMapUser.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvMapUser });
		this.gvMapUser.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvMapUser.ColumnKey = null;
		this.gvMapUser.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcMapUserDatabase, this.gcMapUserUser });
		this.gvMapUser.DisableFormSetting = true;
		this.gvMapUser.DisplayException = false;
		this.gvMapUser.EditColumn = null;
		this.gvMapUser.FieldCurrency = null;
		this.gvMapUser.FieldRate = null;
		this.gvMapUser.FormId = "oLIcenseManager";
		this.gvMapUser.GridControl = this.grdMapUser;
		this.gvMapUser.HasFormSetting = true;
		this.gvMapUser.IgnoreFilter = false;
		this.gvMapUser.IndexEditRow = -1;
		this.gvMapUser.IndicatorWidth = 35;
		this.gvMapUser.IsApzTable = false;
		this.gvMapUser.IsInsertNewRow = false;
		this.gvMapUser.IsLoadingData = false;
		this.gvMapUser.IsMappingContextAction = false;
		this.gvMapUser.ItemId = "OUSU";
		this.gvMapUser.LstColumnAllowBatch = null;
		this.gvMapUser.LstColumnNotAllowBatch = null;
		this.gvMapUser.LstColumnNotEditable = null;
		this.gvMapUser.LstColumNotSetting = null;
		this.gvMapUser.LstOldUDF = null;
		this.gvMapUser.Name = "gvMapUser";
		this.gvMapUser.OptionsBehavior.ReadOnly = true;
		this.gvMapUser.OptionsView.ColumnAutoWidth = false;
		this.gvMapUser.OptionsView.ShowGroupPanel = false;
		this.gvMapUser.ReadOnly = true;
		this.gvMapUser.SettingReadonly = false;
		this.gvMapUser.UseBatchMod = false;
		this.gcMapUserDatabase.Caption = "Database";
		this.gcMapUserDatabase.CurrencyDateField = null;
		this.gcMapUserDatabase.CurrencyField = null;
		this.gcMapUserDatabase.CurrencyFixed = null;
		this.gcMapUserDatabase.CurrencyFixedRate = 0.0;
		this.gcMapUserDatabase.CurrencyRateField = null;
		this.gcMapUserDatabase.FieldName = "DataBase";
		this.gcMapUserDatabase.Name = "gcMapUserDatabase";
		this.gcMapUserDatabase.OptionsColumn.AllowEdit = false;
		this.gcMapUserDatabase.PrimaryKey = false;
		this.gcMapUserDatabase.UnitField = null;
		this.gcMapUserDatabase.Visible = true;
		this.gcMapUserDatabase.VisibleIndex = 0;
		this.gcMapUserDatabase.Width = 138;
		this.gcMapUserUser.Caption = "User";
		this.gcMapUserUser.CurrencyDateField = null;
		this.gcMapUserUser.CurrencyField = null;
		this.gcMapUserUser.CurrencyFixed = null;
		this.gcMapUserUser.CurrencyFixedRate = 0.0;
		this.gcMapUserUser.CurrencyRateField = null;
		this.gcMapUserUser.FieldName = "User";
		this.gcMapUserUser.Name = "gcMapUserUser";
		this.gcMapUserUser.OptionsColumn.AllowEdit = false;
		this.gcMapUserUser.PrimaryKey = false;
		this.gcMapUserUser.UnitField = null;
		this.gcMapUserUser.Visible = true;
		this.gcMapUserUser.VisibleIndex = 1;
		this.labValidUtil.EditControl = null;
		this.labValidUtil.EditValue = "License Type";
		this.labValidUtil.FieldName = null;
		this.labValidUtil.HideWhenEmpty = false;
		this.labValidUtil.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labValidUtil.Location = new System.Drawing.Point(648, 14);
		this.labValidUtil.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labValidUtil.Name = "labValidUtil";
		this.labValidUtil.ReadOnly = false;
		this.labValidUtil.SettingInVisible = false;
		this.labValidUtil.SettingReadonly = false;
		this.labValidUtil.ShowUnderLine = true;
		this.labValidUtil.Size = new System.Drawing.Size(80, 20);
		this.labValidUtil.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labValidUtil.TabIndex = 12;
		this.labValidUtil.Utext = "License Type";
		this.txtLicense.BackColor = System.Drawing.Color.White;
		this.txtLicense.EditControl = null;
		this.txtLicense.EditValue = "";
		this.txtLicense.FieldName = null;
		this.txtLicense.IsSearchControl = false;
		this.txtLicense.Location = new System.Drawing.Point(136, 10);
		this.txtLicense.Name = "txtLicense";
		this.txtLicense.ReadOnly = true;
		this.txtLicense.SettingInVisible = false;
		this.txtLicense.SettingReadonly = false;
		this.txtLicense.Size = new System.Drawing.Size(493, 21);
		this.txtLicense.TabIndex = 7;
		this.txtDBName.BackColor = System.Drawing.Color.White;
		this.txtDBName.EditControl = null;
		this.txtDBName.EditValue = "";
		this.txtDBName.FieldName = null;
		this.txtDBName.IsSearchControl = false;
		this.txtDBName.Location = new System.Drawing.Point(429, 37);
		this.txtDBName.Name = "txtDBName";
		this.txtDBName.ReadOnly = true;
		this.txtDBName.SettingInVisible = false;
		this.txtDBName.SettingReadonly = false;
		this.txtDBName.Size = new System.Drawing.Size(200, 21);
		this.txtDBName.TabIndex = 6;
		this.lbDbName.EditControl = null;
		this.lbDbName.EditValue = "Database";
		this.lbDbName.FieldName = null;
		this.lbDbName.HideWhenEmpty = false;
		this.lbDbName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbDbName.Location = new System.Drawing.Point(350, 40);
		this.lbDbName.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbDbName.Name = "lbDbName";
		this.lbDbName.ReadOnly = false;
		this.lbDbName.SettingInVisible = false;
		this.lbDbName.SettingReadonly = false;
		this.lbDbName.ShowUnderLine = true;
		this.lbDbName.Size = new System.Drawing.Size(80, 20);
		this.lbDbName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbDbName.TabIndex = 0;
		this.lbDbName.Utext = "Database";
		this.labLicenseNumber.EditControl = null;
		this.labLicenseNumber.EditValue = "License Number";
		this.labLicenseNumber.FieldName = null;
		this.labLicenseNumber.HideWhenEmpty = false;
		this.labLicenseNumber.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labLicenseNumber.Location = new System.Drawing.Point(10, 12);
		this.labLicenseNumber.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labLicenseNumber.Name = "labLicenseNumber";
		this.labLicenseNumber.ReadOnly = false;
		this.labLicenseNumber.SettingInVisible = false;
		this.labLicenseNumber.SettingReadonly = false;
		this.labLicenseNumber.ShowUnderLine = true;
		this.labLicenseNumber.Size = new System.Drawing.Size(120, 20);
		this.labLicenseNumber.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labLicenseNumber.TabIndex = 0;
		this.labLicenseNumber.Utext = "License Number";
		this.tabImportLicense.Controls.Add(this.panelControl1);
		this.tabImportLicense.EditControl = null;
		this.tabImportLicense.EditValue = "Import License";
		this.tabImportLicense.FieldName = null;
		this.tabImportLicense.IsSearchControl = false;
		this.tabImportLicense.Name = "tabImportLicense";
		this.tabImportLicense.ReadOnly = false;
		this.tabImportLicense.SettingInVisible = false;
		this.tabImportLicense.SettingReadonly = false;
		this.tabImportLicense.Size = new System.Drawing.Size(981, 342);
		this.tabImportLicense.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabImportLicense.Text = "Import License";
		this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl1.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.Style3D;
		this.panelControl1.Controls.Add(this.btnCancel1);
		this.panelControl1.Controls.Add(this.btnLoadLicenseFile);
		this.panelControl1.Controls.Add(this.txtLicenseFile);
		this.panelControl1.Controls.Add(this.txtServerIP);
		this.panelControl1.Controls.Add(this.txtHardwareKey);
		this.panelControl1.Controls.Add(this.labHardwareKey);
		this.panelControl1.Controls.Add(this.labLicenseFile);
		this.panelControl1.Controls.Add(this.labServerIP);
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(977, 339);
		this.panelControl1.TabIndex = 0;
		this.btnCancel1.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.btnCancel1.Appearance.Options.UseBackColor = true;
		this.btnCancel1.EditControl = null;
		this.btnCancel1.EditValue = "Cancel";
		this.btnCancel1.FieldName = null;
		this.btnCancel1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel1.ImageOptions.Image");
		this.btnCancel1.IsSearchControl = false;
		this.btnCancel1.Location = new System.Drawing.Point(522, 198);
		this.btnCancel1.Name = "btnCancel1";
		this.btnCancel1.ReadOnly = false;
		this.btnCancel1.SettingInVisible = false;
		this.btnCancel1.SettingReadonly = false;
		this.btnCancel1.Size = new System.Drawing.Size(75, 23);
		this.btnCancel1.TabIndex = 46;
		this.btnCancel1.Text = "Cancel";
		this.btnCancel1.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnLoadLicenseFile.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.btnLoadLicenseFile.Appearance.Options.UseBackColor = true;
		this.btnLoadLicenseFile.EditControl = null;
		this.btnLoadLicenseFile.EditValue = "Import License File";
		this.btnLoadLicenseFile.FieldName = null;
		this.btnLoadLicenseFile.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnLoadLicenseFile.ImageOptions.Image");
		this.btnLoadLicenseFile.IsSearchControl = false;
		this.btnLoadLicenseFile.Location = new System.Drawing.Point(398, 198);
		this.btnLoadLicenseFile.Name = "btnLoadLicenseFile";
		this.btnLoadLicenseFile.ReadOnly = false;
		this.btnLoadLicenseFile.SettingInVisible = false;
		this.btnLoadLicenseFile.SettingReadonly = false;
		this.btnLoadLicenseFile.Size = new System.Drawing.Size(118, 23);
		this.btnLoadLicenseFile.TabIndex = 45;
		this.btnLoadLicenseFile.Text = "Import License File";
		this.btnLoadLicenseFile.Click += new System.EventHandler(this.btnLoadLicenseFile_Click);
		this.txtLicenseFile.EditControl = null;
		this.txtLicenseFile.FieldName = null;
		this.txtLicenseFile.IsSearchControl = false;
		this.txtLicenseFile.Location = new System.Drawing.Point(417, 159);
		this.txtLicenseFile.Name = "txtLicenseFile";
		this.txtLicenseFile.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.txtLicenseFile.SettingInVisible = false;
		this.txtLicenseFile.SettingReadonly = false;
		this.txtLicenseFile.Size = new System.Drawing.Size(250, 20);
		this.txtLicenseFile.TabIndex = 4;
		this.txtLicenseFile.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtLicenseFile_ButtonClick);
		this.txtServerIP.EditControl = null;
		this.txtServerIP.FieldName = null;
		this.txtServerIP.IsSearchControl = false;
		this.txtServerIP.Location = new System.Drawing.Point(417, 91);
		this.txtServerIP.Name = "txtServerIP";
		this.txtServerIP.Properties.ReadOnly = true;
		this.txtServerIP.SettingInVisible = false;
		this.txtServerIP.SettingReadonly = false;
		this.txtServerIP.Size = new System.Drawing.Size(250, 20);
		this.txtServerIP.TabIndex = 1;
		this.txtHardwareKey.EditControl = null;
		this.txtHardwareKey.FieldName = null;
		this.txtHardwareKey.IsSearchControl = false;
		this.txtHardwareKey.Location = new System.Drawing.Point(417, 125);
		this.txtHardwareKey.Name = "txtHardwareKey";
		this.txtHardwareKey.Properties.ReadOnly = true;
		this.txtHardwareKey.SettingInVisible = false;
		this.txtHardwareKey.SettingReadonly = false;
		this.txtHardwareKey.Size = new System.Drawing.Size(250, 20);
		this.txtHardwareKey.TabIndex = 3;
		this.labHardwareKey.EditControl = null;
		this.labHardwareKey.EditValue = "Hardware Key";
		this.labHardwareKey.FieldName = null;
		this.labHardwareKey.HideWhenEmpty = false;
		this.labHardwareKey.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labHardwareKey.Location = new System.Drawing.Point(307, 128);
		this.labHardwareKey.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labHardwareKey.Name = "labHardwareKey";
		this.labHardwareKey.ReadOnly = false;
		this.labHardwareKey.SettingInVisible = false;
		this.labHardwareKey.SettingReadonly = false;
		this.labHardwareKey.ShowUnderLine = true;
		this.labHardwareKey.Size = new System.Drawing.Size(109, 20);
		this.labHardwareKey.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labHardwareKey.TabIndex = 36;
		this.labHardwareKey.Utext = "Hardware Key";
		this.labLicenseFile.EditControl = null;
		this.labLicenseFile.EditValue = "License File";
		this.labLicenseFile.FieldName = null;
		this.labLicenseFile.HideWhenEmpty = false;
		this.labLicenseFile.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labLicenseFile.Location = new System.Drawing.Point(307, 162);
		this.labLicenseFile.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labLicenseFile.Name = "labLicenseFile";
		this.labLicenseFile.ReadOnly = false;
		this.labLicenseFile.SettingInVisible = false;
		this.labLicenseFile.SettingReadonly = false;
		this.labLicenseFile.ShowUnderLine = true;
		this.labLicenseFile.Size = new System.Drawing.Size(109, 20);
		this.labLicenseFile.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labLicenseFile.TabIndex = 35;
		this.labLicenseFile.Utext = "License File";
		this.labServerIP.EditControl = null;
		this.labServerIP.EditValue = "API Infomation";
		this.labServerIP.FieldName = null;
		this.labServerIP.HideWhenEmpty = false;
		this.labServerIP.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labServerIP.Location = new System.Drawing.Point(307, 94);
		this.labServerIP.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.labServerIP.Name = "labServerIP";
		this.labServerIP.ReadOnly = false;
		this.labServerIP.SettingInVisible = false;
		this.labServerIP.SettingReadonly = false;
		this.labServerIP.ShowUnderLine = true;
		this.labServerIP.Size = new System.Drawing.Size(109, 20);
		this.labServerIP.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labServerIP.TabIndex = 34;
		this.labServerIP.Utext = "API Infomation";
		this.xtraTabControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(5, 1);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tabImportLicense;
		this.xtraTabControl1.Size = new System.Drawing.Size(983, 367);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.tabImportLicense, this.tabLicense });
		this.xtraTabControl1.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.xtraTabControl1_SelectedPageChanged);
		this.xtraTabControl1.SelectedPageChanging += new DevExpress.XtraTab.TabPageChangingEventHandler(this.xtraTabControl1_SelectedPageChanging);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
		base.ClientSize = new System.Drawing.Size(991, 399);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.xtraTabControl1);
		base.DisableContextAction = true;
		base.DisableLoadScript = true;
		base.FormId = "oLIcenseManager";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_LicenseManager";
		this.Text = "Lisense Manager";
		((System.ComponentModel.ISupportInitialize)this.rUse).EndInit();
		this.tabLicense.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		this.panelControl2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.txtLicenseType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer2).EndInit();
		this.gridSplitContainer2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLicenseType).EndInit();
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer1).EndInit();
		this.gridSplitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdUser).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvUser).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUser).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdMapUser).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvMapUser).EndInit();
		this.tabImportLicense.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txtLicenseFile.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtServerIP.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtHardwareKey.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
