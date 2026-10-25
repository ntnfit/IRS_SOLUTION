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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_Weight : XtraFormEx
{
	private int _UnitCode;

	private string _privilege = "";

	private string _UnitDisply;

	private string _UnitName;

	private string _DataSource;

	private string _Locked = string.Empty;

	private decimal _WightInMG;

	private DataTable dt = new DataTable();

	private DataTable dtObj = new DataTable();

	private DataTable dtOriginal = new DataTable();

	private List<string> lstChangedWeight = new List<string>();

	private DataView clone;

	private DataTable _dtSLRow = new DataTable();

	private bool _isDelete;

	private DataTable dtDelete = new DataTable();

	private IContainer components;

	private GridControlEx grd_Weight;

	private GridViewEx gv_Weight;

	private GridColumnEx gcUnitCode;

	private GridColumnEx gcUnitDisply;

	private GridColumnEx gcUnitName;

	private GridColumnEx gcWightInMG;

	private GridColumnEx gcLocked;

	private GridColumnEx gcDataSource;

	private GridColumnEx gcUserSign;

	private SimpleButtonEx bt_Cancel;

	private SimpleButtonEx bt_OK;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			bt_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			bt_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_Weight()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Weight"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Weight" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				_privilege = "R";
			}
			else
			{
				_privilege = "F";
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_Weight_Load(object sender, EventArgs e)
	{
		await RefreshData();
	}

	private async void bt_OK_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				WinCommons._OpenProcessing();
				DataTable dataTable = grd_Weight.DataSource as DataTable;
				DataTable dt = dataTable.Copy();
				DataRow[] array = (from t in dt.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["UnitDisply"]))
					select t).ToArray();
				if (array.Length != 0)
				{
					dt = array.CopyToDataTable();
				}
				List<IGrouping<string, DataRow>> list = (from i in dt.AsEnumerable()
					group i by Function.ToString(i.Field<string>("UnitDisply")).ToUpper() into gr
					where gr.Count() > 1
					select gr).ToList();
				if (list != null && list.Count > 0)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableWeightUnit, "");
					return;
				}
				if (Function.ToString(gv_Weight.GetFocusedRowCellValue("UnitDisply")).Trim().Length > 2)
				{
					WinCommons.Error(Enums.MessageCode.CharacterNumberIsGreaterThanAllowed, "");
					gv_Weight.FocusedColumn = gcUnitDisply;
					return;
				}
				if (Function.ParseDouble(dt.Rows[dt.Rows.Count - 1]["WightInMG"]) == 0.0)
				{
					WinCommons.Error(Enums.MessageCode.NotEnoughDataWeight, "");
					return;
				}
				if ((from r in dt.AsEnumerable()
					group r by r["UnitDisply"] into gr
					where gr.Count() > 1
					select gr).ToList().Any())
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableWeightUnit, "");
					return;
				}
				if ((from r in dt.AsEnumerable()
					group r by r["UnitName"] into gr
					where gr.Count() > 1
					select gr).ToList().Any())
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInFollowingTableWeightUnit, "");
					return;
				}
				if (_isDelete && _dtSLRow != null && _dtSLRow.Rows.Count > 0)
				{
					using HttpClientHelper<HttpResult> hr = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Weight");
					WinCommons._OpenProcessing();
					HttpResult httpResult = await hr.PostAsync("Remove", _dtSLRow);
					WinCommons._CloseProcessing();
					if (!string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
						return;
					}
					_isDelete = false;
				}
				await Add(dt);
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Add(DataTable dt)
	{
		_ = 1;
		try
		{
			DataTable dtAdd = dt.Clone();
			int code = GetNewCode(dt);
			foreach (DataRow row in dt.Rows)
			{
				DataRow dataRow2 = dtAdd.NewRow();
				if (Function.ParseInt(row["UnitCode"]) > 0)
				{
					dataRow2["UnitCode"] = row["UnitCode"];
					dataRow2["UnitDisply"] = row["UnitDisply"];
					dataRow2["UnitName"] = row["UnitName"];
					dataRow2["WightInMG"] = row["WightInMG"];
					dataRow2["Locked"] = row["Locked"];
					dataRow2["DataSource"] = row["DataSource"];
					dataRow2["UserSign"] = row["UserSign"];
				}
				else
				{
					dataRow2["UnitCode"] = code;
					dataRow2["UnitDisply"] = row["UnitDisply"];
					dataRow2["UnitName"] = row["UnitName"];
					dataRow2["WightInMG"] = row["WightInMG"];
					dataRow2["Locked"] = "N";
					dataRow2["DataSource"] = "I";
					DataRow dataRow3 = dataRow2;
					dataRow3["UserSign"] = await GetUserSignByUserCode_tableOUSR();
					code++;
				}
				dtAdd.Rows.Add(dataRow2);
			}
			string text = Save(dtAdd);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				return;
			}
			WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			FormMode = Enums.FormViewModeType.View;
			await RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string Save(DataTable dtObj)
	{
		try
		{
			using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "Weight");
			return httpClientHelper.Post("Save", dtObj).Message;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async Task GetMax()
	{
		try
		{
			using HttpClientHelper<DataTable> _hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Weight");
			_UnitCode = await _hp.GetIntAsync("GetMax") + 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RefreshData()
	{
		lstChangedWeight.Clear();
		dtDelete.Rows.Clear();
		await GetMax();
		await GetUserSignByUserCode_tableOUSR();
		await LoadData();
	}

	private async Task LoadData()
	{
		_ = 2;
		try
		{
			using HttpClientHelper<DataTable> _hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Weight");
			DataTable dataTable = await _hp.GetDataTableAsync("GetData");
			dtOriginal = dataTable.Copy();
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				await GetDefaultTable();
			}
			else
			{
				grd_Weight.DataSource = dataTable;
				await AddNewRow();
			}
			repositoryItemCalcEdit1.Mask.MaskType = MaskType.Numeric;
			repositoryItemCalcEdit1.Mask.EditMask = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
			repositoryItemCalcEdit1.Mask.UseMaskAsDisplayFormat = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetDefaultTable()
	{
		try
		{
			DataTable dt = new DataTable
			{
				Columns = 
				{
					{
						"UnitCode",
						typeof(string)
					},
					{
						"UnitDisply",
						typeof(string)
					},
					{
						"UnitName",
						typeof(string)
					},
					{
						"WightInMG",
						typeof(decimal)
					},
					{
						"Locked",
						typeof(string)
					},
					{
						"DataSource",
						typeof(string)
					},
					{
						"UserSign",
						typeof(string)
					}
				}
			};
			DataRow dr = dt.NewRow();
			dr["UnitCode"] = 1;
			dr["UnitDisply"] = string.Empty;
			dr["UnitName"] = string.Empty;
			dr["WightInMG"] = 0;
			dr["Locked"] = "N";
			dr["DataSource"] = "I";
			dr["UserSign"] = await GetUserSignByUserCode_tableOUSR();
			dt.Rows.Add(dr);
			grd_Weight.DataSource = dt;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Weight_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async Task<int> GetUserSignByUserCode_tableOUSR()
	{
		string userCodeLogin = GlobalData.UserCodeLogin;
		try
		{
			using HttpClientHelper<UsersImpl> _helper = new HttpClientHelper<UsersImpl>(Configuration.ApiServer, "Users");
			DataTable dataTable = await _helper.GetDataTableAsync("GetUserSign", new
			{
				nameLogin = userCodeLogin
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ParseInt(dataTable.Rows[0]["UserSign"].ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return -1;
	}

	private async Task AddNewRow()
	{
		_ = 1;
		try
		{
			dt = grd_Weight.DataSource as DataTable;
			if (dt == null)
			{
				await GetDefaultTable();
				return;
			}
			if (!string.IsNullOrEmpty(Function.ToString(dt.Rows[dt.Rows.Count - 1]["UnitCode"])))
			{
				DataRow dr = dt.NewRow();
				string value = Function.ToString(GetNewCode(dt));
				dr["UnitCode"] = value;
				string value2 = "N";
				dr["Locked"] = value2;
				string value3 = "I";
				dr["DataSource"] = value3;
				dr["UserSign"] = await GetUserSignByUserCode_tableOUSR();
				dt.Rows.Add(dr);
			}
			grd_Weight.DataSource = dt;
			grd_Weight.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private int GetNewCode(DataTable dt)
	{
		try
		{
			List<int> list = new List<int>();
			foreach (DataRow row in dt.Rows)
			{
				if (row["UnitCode"] != null)
				{
					list.Add(Function.ParseInt(row["UnitCode"].ToString()));
				}
			}
			if (list.Count > 0)
			{
				int? num = ((IEnumerable<int>)list).Max((Func<int, int?>)((int x) => x));
				if (num.HasValue)
				{
					return Function.ParseInt(num.ToString()) + 1;
				}
				return 0;
			}
			return 0;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				_DataSource = Function.ToString(gv_Weight.GetFocusedRowCellValue("DataSource"));
				_Locked = Function.ToString(gv_Weight.GetFocusedRowCellValue("Locked"));
				if (_DataSource == "N" && _Locked == "Y")
				{
					WinCommons.Error(Enums.MessageCode.CannotRemoveARecordCreatedBySystem, "");
					return;
				}
				_isDelete = true;
				_dtSLRow = gv_Weight.GetListDataSourceSelectedRow().CopyToDataTable();
				gv_Weight.BeginUpdate();
				gv_Weight.DeleteSelectedRows();
				gv_Weight.EndUpdate();
				dtDelete = grd_Weight.DataSource as DataTable;
				grd_Weight.RefreshDataSource();
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataTable> GetWeightByUnitDisply(string key)
	{
		try
		{
			key = Function.ToString(gv_Weight.GetFocusedRowCellValue("DataSource"));
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Weight");
			return await hp.GetDataTableAsync("GetWeightByUnitDisply", new { key });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void gv_Weight_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			dt = grd_Weight.DataSource as DataTable;
			if (dt.IsNotNull() && (e.Column.FieldName == "WightInMG" || e.Column.FieldName == "UnitName" || e.Column.FieldName == "UnitDisply"))
			{
				if (e.RowHandle == dt.Rows.Count - 1)
				{
					await AddNewRow();
				}
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		await RefreshData();
	}

	private async void grd_Weight_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grd_Weight, gv_Weight, new List<string> { "UnitDisply" });
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_Weight));
		this.grd_Weight = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Weight = new Apzon.Controls.Grid.GridViewEx();
		this.gcUnitCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUnitDisply = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUnitName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWightInMG = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcLocked = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDataSource = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Weight).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Weight).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		base.SuspendLayout();
		this.grd_Weight.AllowPasteExcel = true;
		this.grd_Weight.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Weight.ClipboardData = null;
		this.grd_Weight.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Weight.Location = new System.Drawing.Point(2, 2);
		this.grd_Weight.MainView = this.gv_Weight;
		this.grd_Weight.Name = "grd_Weight";
		this.grd_Weight.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemCalcEdit1 });
		this.grd_Weight.Size = new System.Drawing.Size(360, 272);
		this.grd_Weight.TabIndex = 0;
		this.grd_Weight.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Weight });
		this.grd_Weight.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Weight_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Weight.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Weight.ColumnKey = null;
		this.gv_Weight.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gcUnitCode, this.gcUnitDisply, this.gcUnitName, this.gcWightInMG, this.gcLocked, this.gcDataSource, this.gcUserSign });
		this.gv_Weight.DisableFormSetting = false;
		this.gv_Weight.DisplayException = false;
		this.gv_Weight.EditColumn = null;
		this.gv_Weight.FieldCurrency = null;
		this.gv_Weight.FieldRate = null;
		this.gv_Weight.FormId = "oWeight";
		this.gv_Weight.GridControl = this.grd_Weight;
		this.gv_Weight.IgnoreFilter = false;
		this.gv_Weight.IndexEditRow = -1;
		this.gv_Weight.IndicatorWidth = 30;
		this.gv_Weight.IsApzTable = false;
		this.gv_Weight.IsInsertNewRow = false;
		this.gv_Weight.IsLoadingData = false;
		this.gv_Weight.IsMappingContextAction = false;
		this.gv_Weight.ItemId = "OWGT";
		this.gv_Weight.LstColumnAllowBatch = null;
		this.gv_Weight.LstColumnNotAllowBatch = null;
		this.gv_Weight.LstColumnNotEditable = null;
		this.gv_Weight.LstColumNotSetting = null;
		this.gv_Weight.Name = "gv_Weight";
		this.gv_Weight.OptionsSelection.MultiSelect = true;
		this.gv_Weight.OptionsView.ColumnAutoWidth = false;
		this.gv_Weight.OptionsView.ShowGroupPanel = false;
		this.gv_Weight.UseBatchMod = false;
		this.gv_Weight.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Weight_CustomDrawRowIndicator);
		this.gv_Weight.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Weight_CellValueChanged);
		this.gcUnitCode.Caption = "UnitCode";
		this.gcUnitCode.CurrencyDateField = null;
		this.gcUnitCode.CurrencyField = null;
		this.gcUnitCode.CurrencyFixed = null;
		this.gcUnitCode.CurrencyFixedRate = 0.0;
		this.gcUnitCode.CurrencyRateField = null;
		this.gcUnitCode.FieldName = "UnitCode";
		this.gcUnitCode.Name = "gcUnitCode";
		this.gcUnitCode.PrimaryKey = false;
		this.gcUnitCode.UnitField = null;
		this.gcUnitDisply.Caption = "Code";
		this.gcUnitDisply.CurrencyDateField = null;
		this.gcUnitDisply.CurrencyField = null;
		this.gcUnitDisply.CurrencyFixed = null;
		this.gcUnitDisply.CurrencyFixedRate = 0.0;
		this.gcUnitDisply.CurrencyRateField = null;
		this.gcUnitDisply.FieldName = "UnitDisply";
		this.gcUnitDisply.Name = "gcUnitDisply";
		this.gcUnitDisply.PrimaryKey = true;
		this.gcUnitDisply.UnitField = null;
		this.gcUnitDisply.Visible = true;
		this.gcUnitDisply.VisibleIndex = 0;
		this.gcUnitName.Caption = "Unit Name";
		this.gcUnitName.CurrencyDateField = null;
		this.gcUnitName.CurrencyField = null;
		this.gcUnitName.CurrencyFixed = null;
		this.gcUnitName.CurrencyFixedRate = 0.0;
		this.gcUnitName.CurrencyRateField = null;
		this.gcUnitName.FieldName = "UnitName";
		this.gcUnitName.Name = "gcUnitName";
		this.gcUnitName.PrimaryKey = false;
		this.gcUnitName.UnitField = null;
		this.gcUnitName.Visible = true;
		this.gcUnitName.VisibleIndex = 1;
		this.gcUnitName.Width = 100;
		this.gcWightInMG.Caption = "Weight (mg)";
		this.gcWightInMG.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gcWightInMG.CurrencyDateField = null;
		this.gcWightInMG.CurrencyField = null;
		this.gcWightInMG.CurrencyFixed = null;
		this.gcWightInMG.CurrencyFixedRate = 0.0;
		this.gcWightInMG.CurrencyRateField = null;
		this.gcWightInMG.FieldName = "WightInMG";
		this.gcWightInMG.Name = "gcWightInMG";
		this.gcWightInMG.PrimaryKey = false;
		this.gcWightInMG.UnitField = null;
		this.gcWightInMG.Visible = true;
		this.gcWightInMG.VisibleIndex = 2;
		this.gcWightInMG.Width = 150;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gcLocked.Caption = "Locked";
		this.gcLocked.CurrencyDateField = null;
		this.gcLocked.CurrencyField = null;
		this.gcLocked.CurrencyFixed = null;
		this.gcLocked.CurrencyFixedRate = 0.0;
		this.gcLocked.CurrencyRateField = null;
		this.gcLocked.FieldName = "Locked";
		this.gcLocked.Name = "gcLocked";
		this.gcLocked.PrimaryKey = false;
		this.gcLocked.UnitField = null;
		this.gcDataSource.Caption = "DataSource";
		this.gcDataSource.CurrencyDateField = null;
		this.gcDataSource.CurrencyField = null;
		this.gcDataSource.CurrencyFixed = null;
		this.gcDataSource.CurrencyFixedRate = 0.0;
		this.gcDataSource.CurrencyRateField = null;
		this.gcDataSource.FieldName = "DataSource";
		this.gcDataSource.Name = "gcDataSource";
		this.gcDataSource.PrimaryKey = false;
		this.gcDataSource.UnitField = null;
		this.gcUserSign.Caption = "UserSign";
		this.gcUserSign.CurrencyDateField = null;
		this.gcUserSign.CurrencyField = null;
		this.gcUserSign.CurrencyFixed = null;
		this.gcUserSign.CurrencyFixedRate = 0.0;
		this.gcUserSign.CurrencyRateField = null;
		this.gcUserSign.FieldName = "UserSign";
		this.gcUserSign.Name = "gcUserSign";
		this.gcUserSign.PrimaryKey = false;
		this.gcUserSign.UnitField = null;
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.EditControl = null;
		this.bt_Cancel.EditValue = "Cancel";
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(80, 278);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.ReadOnly = false;
		this.bt_Cancel.SettingInVisible = false;
		this.bt_Cancel.SettingReadonly = false;
		this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
		this.bt_Cancel.TabIndex = 9;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.EditControl = null;
		this.bt_OK.EditValue = "OK";
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(2, 278);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.ReadOnly = false;
		this.bt_OK.SettingInVisible = false;
		this.bt_OK.SettingReadonly = false;
		this.bt_OK.Size = new System.Drawing.Size(75, 23);
		this.bt_OK.TabIndex = 10;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(363, 303);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.grd_Weight);
		base.FormId = "oWeight";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Weight";
		this.Text = "Weight - Setup";
		base.Load += new System.EventHandler(this.frm_Weight_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Weight).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Weight).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
