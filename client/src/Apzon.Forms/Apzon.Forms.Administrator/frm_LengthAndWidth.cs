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
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_LengthAndWidth : XtraFormEx
{
	private DataTable dt = new DataTable();

	private DataTable dtDelete = new DataTable();

	private int _maxCode;

	private DataTable _dtSelectRows = new DataTable();

	private bool _isDelete = true;

	private IContainer components;

	private GridControlEx grd_LengthWidth;

	private GridViewEx gv_LengthWidth;

	private SimpleButtonEx bt_OK;

	private SimpleButtonEx bt_Cancel;

	private GridColumnEx gcUnitDisply;

	private GridColumnEx gcUnitName;

	private GridColumnEx gcVolumeCode;

	private GridColumnEx gcLength;

	private GridColumnEx gcCode;

	private GridColumnEx gcDataSource;

	private GridColumnEx gcLocked;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private GridColumnEx gcUserSign;

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

	public frm_LengthAndWidth()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_LengthAndWidth"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_LengthAndWidth" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void bt_OK_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
		}
		if (FormMode != Enums.FormViewModeType.Update)
		{
			return;
		}
		if (_isDelete && _dtSelectRows != null && _dtSelectRows.Rows.Count > 0)
		{
			using HttpClientHelper<HttpResult> hr = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "LengthAndWidth");
			HttpResult httpResult = await hr.PostAsync("Remove", _dtSelectRows);
			if (!string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return;
			}
		}
		Save();
	}

	private async void frm_LengthAndWidth_Load(object sender, EventArgs e)
	{
		await GetMaxCode();
		await GetUserSignByUserCode_tableOUSR();
		await LoadData();
	}

	private async Task GetMaxCode()
	{
		try
		{
			using HttpClientHelper<LengthAndWidthImpl> _hp = new HttpClientHelper<LengthAndWidthImpl>(Configuration.ApiServer, "LengthAndWidth");
			_maxCode = await _hp.GetIntAsync("GetMax") + 1;
			gv_LengthWidth.SetFocusedRowCellValue("UnitCode", _maxCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadData()
	{
		_ = 2;
		try
		{
			using HttpClientHelper<DataTable> _hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LengthAndWidth");
			dt = await _hp.GetDataTableAsync("GetData");
			if (dt == null || dt.Rows.Count <= 0)
			{
				await GetDataDefault();
			}
			else
			{
				grd_LengthWidth.DataSource = dt;
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

	private async Task GetDataDefault()
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
					"VolDisply",
					typeof(string)
				},
				{
					"SizeInMM",
					typeof(string)
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
					typeof(int)
				}
			}
		};
		DataRow dr = dt.NewRow();
		dr["UnitCode"] = gv_LengthWidth.EditingValue;
		dr["UnitDisply"] = string.Empty;
		dr["UnitName"] = string.Empty;
		dr["VolDisply"] = string.Empty;
		dr["SizeInMM"] = string.Empty;
		dr["Locked"] = "Y";
		dr["DataSource"] = "N";
		dr["UserSign"] = await GetUserSignByUserCode_tableOUSR();
		dt.Rows.Add(dr);
		grd_LengthWidth.DataSource = dt;
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

	private void gv_LengthWidth_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				int num = Function.ParseInt(gv_LengthWidth.GetFocusedRowCellValue("UnitCode"));
				if (num == 1 || num == 2 || num == 3 || num == 4 || num == 5 || num == 6)
				{
					WinCommons.Warning(Enums.MessageCode.CannotRemoveARecordCreatedBySystem, "");
				}
				else if (!(await CheckExistUnitCode(num)).IsNotNull())
				{
					_isDelete = true;
					_dtSelectRows = gv_LengthWidth.GetListDataSourceSelectedRow().CopyToDataTable();
					gv_LengthWidth.BeginUpdate();
					gv_LengthWidth.DeleteSelectedRows();
					gv_LengthWidth.EndUpdate();
					dtDelete = grd_LengthWidth.DataSource as DataTable;
					grd_LengthWidth.RefreshDataSource();
					FormMode = Enums.FormViewModeType.Update;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.CannotDeleteUnitCodeUsed, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> CheckExistUnitCode(int unitCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "LengthAndWidth");
			return await helper.GetAsync("CheckExistUnitCode", new { unitCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task AddNewRow()
	{
		_ = 1;
		try
		{
			if (!(grd_LengthWidth.DataSource is DataTable dt))
			{
				await GetDataDefault();
				return;
			}
			if (!string.IsNullOrEmpty(Function.ToString(dt.Rows[dt.Rows.Count - 1]["UnitCode"])))
			{
				DataRow dr = dt.NewRow();
				string value = Function.ToString(GetNewCode(dt));
				dr["UnitCode"] = value;
				string value2 = "N";
				dr["Locked"] = value2;
				string value3 = "N";
				dr["DataSource"] = value3;
				dr["UserSign"] = await GetUserSignByUserCode_tableOUSR();
				dt.Rows.Add(dr);
			}
			grd_LengthWidth.DataSource = dt;
			grd_LengthWidth.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private async void Save()
	{
		try
		{
			dt = grd_LengthWidth.DataSource as DataTable;
			DataTable dataTable = dt.Copy();
			DataRow[] array = (from t in dataTable.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["UnitDisply"]))
				select t).ToArray();
			if (array.Length != 0)
			{
				dataTable = array.CopyToDataTable();
			}
			if (Function.ParseDouble(dataTable.Rows[dataTable.Rows.Count - 1]["SizeInMM"]) == 0.0)
			{
				WinCommons.Error(Enums.MessageCode.NotEnoughDataLength, "");
				return;
			}
			List<IGrouping<string, DataRow>> list = (from i in dataTable.AsEnumerable()
				group i by Function.ToString(i.Field<string>("UnitDisply")).ToUpper() into gr
				where gr.Count() > 1
				select gr).ToList();
			if (list != null && list.Count > 0)
			{
				WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyInFollowingTableLengthUnits, "");
				return;
			}
			DataTable dtAdd = dataTable.Clone();
			int code = GetNewCode(dataTable);
			foreach (DataRow row in dataTable.Rows)
			{
				DataRow dataRow2 = dtAdd.NewRow();
				if (Function.ParseInt(row["UnitCode"]) > 0)
				{
					dataRow2["UnitCode"] = row["UnitCode"];
					dataRow2["UnitDisply"] = row["UnitDisply"];
					dataRow2["UnitName"] = row["UnitName"];
					dataRow2["VolDisply"] = row["VolDisply"];
					dataRow2["SizeInMM"] = row["SizeInMM"];
					dataRow2["Locked"] = row["Locked"];
					dataRow2["DataSource"] = row["DataSource"];
					dataRow2["UserSign"] = row["UserSign"];
				}
				else
				{
					dataRow2["UnitCode"] = code;
					dataRow2["UnitDisply"] = row["UnitDisply"];
					dataRow2["UnitName"] = row["UnitName"];
					dataRow2["VolDisply"] = row["VolDisply"];
					dataRow2["SizeInMM"] = row["SizeInMM"];
					dataRow2["Locked"] = "N";
					dataRow2["DataSource"] = "N";
					DataRow dataRow3 = dataRow2;
					dataRow3["UserSign"] = await GetUserSignByUserCode_tableOUSR();
					code++;
				}
				dtAdd.Rows.Add(dataRow2);
			}
			using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "LengthAndWidth");
			WinCommons._OpenProcessing();
			HttpResult httpResult = httpClientHelper.Post("Save", dtAdd);
			WinCommons._CloseProcessing();
			if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
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
		frm_LengthAndWidth_Load(null, null);
	}

	private async void grd_LengthWidth_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grd_LengthWidth, gv_LengthWidth, new List<string> { "UnitDisply" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_LengthWidth_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		dt = grd_LengthWidth.DataSource as DataTable;
		if (dt.IsNotNull() && (e.Column.FieldName == "VolDisply" || e.Column.FieldName == "SizeInMM" || e.Column.FieldName == "UnitName" || e.Column.FieldName == "UnitDisply"))
		{
			if (e.RowHandle == dt.Rows.Count - 1)
			{
				await AddNewRow();
			}
			FormMode = Enums.FormViewModeType.Update;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_LengthAndWidth));
		this.grd_LengthWidth = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_LengthWidth = new Apzon.Controls.Grid.GridViewEx();
		this.gcUnitDisply = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUnitName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcVolumeCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLength = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDataSource = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLocked = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.bt_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_LengthWidth).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_LengthWidth).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		base.SuspendLayout();
		this.grd_LengthWidth.AllowPasteExcel = true;
		this.grd_LengthWidth.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_LengthWidth.ClipboardData = null;
		this.grd_LengthWidth.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_LengthWidth.Location = new System.Drawing.Point(1, 2);
		this.grd_LengthWidth.MainView = this.gv_LengthWidth;
		this.grd_LengthWidth.Name = "grd_LengthWidth";
		this.grd_LengthWidth.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemCalcEdit1 });
		this.grd_LengthWidth.Size = new System.Drawing.Size(604, 289);
		this.grd_LengthWidth.TabIndex = 0;
		this.grd_LengthWidth.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_LengthWidth });
		this.grd_LengthWidth.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_LengthWidth_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(118, 26);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_LengthWidth.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_LengthWidth.ColumnKey = null;
		this.gv_LengthWidth.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gcUnitDisply, this.gcUnitName, this.gcVolumeCode, this.gcLength, this.gcCode, this.gcDataSource, this.gcLocked, this.gcUserSign });
		this.gv_LengthWidth.DisableFormSetting = false;
		this.gv_LengthWidth.DisplayException = false;
		this.gv_LengthWidth.EditColumn = null;
		this.gv_LengthWidth.FieldCurrency = null;
		this.gv_LengthWidth.FieldRate = null;
		this.gv_LengthWidth.FormId = "oLengthAndWidth";
		this.gv_LengthWidth.GridControl = this.grd_LengthWidth;
		this.gv_LengthWidth.IgnoreFilter = false;
		this.gv_LengthWidth.IndexEditRow = -1;
		this.gv_LengthWidth.IndicatorWidth = 35;
		this.gv_LengthWidth.IsApzTable = false;
		this.gv_LengthWidth.IsInsertNewRow = false;
		this.gv_LengthWidth.IsLoadingData = false;
		this.gv_LengthWidth.IsMappingContextAction = false;
		this.gv_LengthWidth.ItemId = "OLGT";
		this.gv_LengthWidth.LstColumnAllowBatch = null;
		this.gv_LengthWidth.LstColumnNotAllowBatch = null;
		this.gv_LengthWidth.LstColumnNotEditable = null;
		this.gv_LengthWidth.LstColumNotSetting = null;
		this.gv_LengthWidth.Name = "gv_LengthWidth";
		this.gv_LengthWidth.OptionsSelection.MultiSelect = true;
		this.gv_LengthWidth.OptionsView.ShowGroupPanel = false;
		this.gv_LengthWidth.UseBatchMod = false;
		this.gv_LengthWidth.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_LengthWidth_CustomDrawRowIndicator);
		this.gv_LengthWidth.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_LengthWidth_CellValueChanged);
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
		this.gcVolumeCode.Caption = "Volume Code";
		this.gcVolumeCode.CurrencyDateField = null;
		this.gcVolumeCode.CurrencyField = null;
		this.gcVolumeCode.CurrencyFixed = null;
		this.gcVolumeCode.CurrencyFixedRate = 0.0;
		this.gcVolumeCode.CurrencyRateField = null;
		this.gcVolumeCode.FieldName = "VolDisply";
		this.gcVolumeCode.Name = "gcVolumeCode";
		this.gcVolumeCode.PrimaryKey = false;
		this.gcVolumeCode.UnitField = null;
		this.gcVolumeCode.Visible = true;
		this.gcVolumeCode.VisibleIndex = 2;
		this.gcLength.Caption = "Length(mm)";
		this.gcLength.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gcLength.CurrencyDateField = null;
		this.gcLength.CurrencyField = null;
		this.gcLength.CurrencyFixed = null;
		this.gcLength.CurrencyFixedRate = 0.0;
		this.gcLength.CurrencyRateField = null;
		this.gcLength.DisplayFormat.FormatString = "n2";
		this.gcLength.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcLength.FieldName = "SizeInMM";
		this.gcLength.Name = "gcLength";
		this.gcLength.PrimaryKey = false;
		this.gcLength.UnitField = null;
		this.gcLength.Visible = true;
		this.gcLength.VisibleIndex = 3;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gcCode.Caption = "CodeId";
		this.gcCode.CurrencyDateField = null;
		this.gcCode.CurrencyField = null;
		this.gcCode.CurrencyFixed = null;
		this.gcCode.CurrencyFixedRate = 0.0;
		this.gcCode.CurrencyRateField = null;
		this.gcCode.FieldName = "UnitCode";
		this.gcCode.Name = "gcCode";
		this.gcCode.PrimaryKey = false;
		this.gcCode.UnitField = null;
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
		this.bt_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_OK.EditControl = null;
		this.bt_OK.EditValue = "OK";
		this.bt_OK.FieldName = null;
		this.bt_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_OK.ImageOptions.Image");
		this.bt_OK.IsSearchControl = false;
		this.bt_OK.Location = new System.Drawing.Point(1, 296);
		this.bt_OK.Name = "bt_OK";
		this.bt_OK.ReadOnly = false;
		this.bt_OK.SettingInVisible = false;
		this.bt_OK.SettingReadonly = false;
		this.bt_OK.Size = new System.Drawing.Size(75, 23);
		this.bt_OK.TabIndex = 1;
		this.bt_OK.Text = "OK";
		this.bt_OK.Click += new System.EventHandler(this.bt_OK_Click);
		this.bt_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.bt_Cancel.EditControl = null;
		this.bt_Cancel.EditValue = "Cancel";
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(79, 296);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.ReadOnly = false;
		this.bt_Cancel.SettingInVisible = false;
		this.bt_Cancel.SettingReadonly = false;
		this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
		this.bt_Cancel.TabIndex = 1;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(606, 324);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_OK);
		base.Controls.Add(this.grd_LengthWidth);
		base.FormId = "oLengthAndWidth";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_LengthAndWidth";
		this.Text = "Length and Width - SetUp";
		base.Load += new System.EventHandler(this.frm_LengthAndWidth_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_LengthWidth).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_LengthWidth).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
