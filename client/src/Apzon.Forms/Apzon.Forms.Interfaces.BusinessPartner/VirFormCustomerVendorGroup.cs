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
using Apzon.Commons.Constants;
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
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Interfaces.BusinessPartner;

public class VirFormCustomerVendorGroup : XtraFormEx
{
	private string FormController = "GroupCustomerVender";

	private DataTable _OlData = new DataTable();

	private readonly HttpClientHelper<DataTable> _helper;

	protected string GroupType;

	public bool IsOpenFromBP;

	public short NewValue;

	private bool _isDelete;

	private DataTable _dtRemove = new DataTable();

	private int _hotTrackRow = int.MinValue;

	private IContainer components;

	private GridControlEx grdDoc;

	public SimpleButtonEx btn_Save;

	public SimpleButtonEx btnCancel;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private RepositoryItemLookUpEditCombo rep_effect;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	protected GridViewEx gvDoc;

	private RepositoryItemLookUpEdit rep_Price;

	private GridColumnEx gridColumn2;

	private RepositoryItemLookUpEdit rep_Prices;

	private int HotTrackRow
	{
		get
		{
			return _hotTrackRow;
		}
		set
		{
			try
			{
				if (_hotTrackRow != value)
				{
					int hotTrackRow = _hotTrackRow;
					_hotTrackRow = value;
					if (gvDoc.ActiveEditor != null)
					{
						gvDoc.PostEditor();
					}
					gvDoc.RefreshRow(hotTrackRow);
					gvDoc.RefreshRow(_hotTrackRow);
					if (_hotTrackRow >= 0)
					{
						grdDoc.Cursor = Cursors.Hand;
					}
					else
					{
						grdDoc.Cursor = Cursors.Default;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		}
	}

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
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public VirFormCustomerVendorGroup()
	{
		InitializeComponent();
		if (LicenseManager.UsageMode != LicenseUsageMode.Designtime && !DisableAuthorization)
		{
			_helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "GroupCustomerVender");
		}
	}

	private async Task LoadInItControl()
	{
		try
		{
			DataTable dataSource = await _helper.GetAsync("GetPriceList");
			rep_Prices.DataSource = dataSource;
			List<Common.Table_Code> dataSource2 = CommonList.EffectiveDiscount.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			rep_effect.DataSource = dataSource2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void AddNewRow()
	{
		_ = 2;
		try
		{
			DataTable dt = grdDoc.DataSource as DataTable;
			Function.ParseInt((await GetMaxCode()).Rows[0][0]);
			if (dt.IsNotNull())
			{
				if (!string.IsNullOrEmpty(Function.ToString(dt.Rows[dt.Rows.Count - 1]["GroupName"])))
				{
					DataRow row = dt.NewRow();
					row["GroupCode"] = 0;
					row["GroupType"] = GroupType;
					row["Locked"] = "N";
					row["DataSource"] = "I";
					DataRow dataRow = row;
					dataRow["UserSign"] = await GetUserSignByUserCode_tableOUSR();
					row["DiscRel"] = "L";
					dt.Rows.Add(row);
					dt.AcceptChanges();
				}
			}
			else
			{
				dt = new DataTable
				{
					Columns = 
					{
						{
							"GroupCode",
							typeof(int)
						},
						{
							"GroupName",
							typeof(string)
						},
						{
							"GroupType",
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
						},
						{
							"PriceList",
							typeof(int)
						},
						{
							"DiscRel",
							typeof(string)
						}
					}
				};
				DataRow row = dt.NewRow();
				row["GroupCode"] = 0;
				row["GroupType"] = GroupType;
				row["Locked"] = "N";
				row["DataSource"] = "I";
				DataRow dataRow = row;
				dataRow["UserSign"] = await GetUserSignByUserCode_tableOUSR();
				row["DiscRel"] = "L";
				dt.Rows.Add(row);
				dt.AcceptChanges();
			}
			grdDoc.DataSource = dt;
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> GetMaxCode()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("GetMaxCode");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task LoadDataGrid()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			DataTable dataTable = await _helper.GetAsync("LoadDoc", new
			{
				catelogy = GroupType
			});
			if (dataTable.IsNotNull())
			{
				if (IsOpenFromBP)
				{
					_OlData = dataTable.Copy();
				}
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData()
	{
		_ = 1;
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataTable dtNew = dataTable.Copy();
				DataRow[] array = (from t in dtNew.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["GroupName"]))
					select t).ToArray();
				if (array.Length != 0)
				{
					dtNew = array.CopyToDataTable();
				}
				DataTable dtAdd = dtNew.Clone();
				int code = Function.ParseInt((await GetMaxCode()).Rows[0][0]);
				foreach (DataRow drow in dtNew.Rows)
				{
					DataRow dataRow = dtAdd.NewRow();
					if (Function.ParseInt(drow["GroupCode"]) > 0)
					{
						dataRow["GroupCode"] = drow["GroupCode"];
						dataRow["GroupName"] = drow["GroupName"];
						dataRow["DataSource"] = drow["DataSource"];
						dataRow["UserSign"] = drow["UserSign"];
						dataRow["GroupType"] = drow["GroupType"];
						dataRow["Locked"] = drow["Locked"];
						dataRow["PriceList"] = drow["PriceList"];
						dataRow["DiscRel"] = drow["DiscRel"];
					}
					else
					{
						dataRow["GroupCode"] = 0;
						dataRow["GroupName"] = drow["GroupName"];
						dataRow["DataSource"] = "I";
						dataRow["Locked"] = "N";
						DataRow dataRow2 = dataRow;
						dataRow2["UserSign"] = await GetUserSignByUserCode_tableOUSR();
						dataRow["PriceList"] = drow["PriceList"];
						dataRow["DiscRel"] = drow["DiscRel"];
						dataRow["GroupType"] = GroupType;
						code++;
					}
					dtAdd.Rows.Add(dataRow);
				}
				new Tuple<DataTable, string>(new DataTable(), "");
				using HttpClientHelper<HttpResult> httpClientHelper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, FormController);
				if (_isDelete)
				{
					new Tuple<DataTable, string>(_dtRemove, "");
					httpClientHelper.PostAsync("Remove", _dtRemove);
					_isDelete = false;
				}
				Tuple<DataTable, string> pr = new Tuple<DataTable, string>(dtAdd, GroupType);
				HttpResult httpResult = httpClientHelper.Post("SaveDoc", pr);
				if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
				{
					return "";
				}
				return httpResult.Message;
			}
			return "No data to save";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
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

	private async void frm_CustomerGroup_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (GroupType == "S")
			{
				Text = "Vendor Group";
			}
			else
			{
				GroupType = "C";
				Text = "Customer Group";
			}
			await LoadInItControl();
			await LoadDataGrid();
			_dtRemove = (grdDoc.DataSource as DataTable).Copy().Clone();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
	}

	public virtual async void btn_Save_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				WinCommons._OpenProcessing();
				string text = await SaveData();
				if (string.IsNullOrEmpty(text))
				{
					DataTable oldData = _OlData.Copy();
					await LoadDataGrid();
					DataTable dataTable = grdDoc.DataSource as DataTable;
					if (IsOpenFromBP && dataTable.IsNotNull())
					{
						if (_OlData.IsNotNull())
						{
							if (oldData.Rows.Count < _OlData.Rows.Count)
							{
								NewValue = Function.ParseShort(_OlData.Rows[_OlData.Rows.Count - 1]["GroupCode"]).Value;
							}
							else
							{
								NewValue = Function.ParseShort(gvDoc.GetFocusedRowCellValue("GroupCode")).Value;
							}
						}
						else
						{
							DataRow dataRow = dataTable.Rows[0];
							NewValue = Function.ParseShort(dataRow["GroupCode"]).Value;
						}
						Close();
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, Function.ToString(text));
				}
				WinCommons._CloseProcessing();
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void grdDoc_EditorKeyDown(object sender, KeyEventArgs e)
	{
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			List<DataRow> listDataSourceSelectedRow = gvDoc.GetListDataSourceSelectedRow();
			foreach (DataRow row in listDataSourceSelectedRow)
			{
				int num = Function.ParseInt(row["GroupCode"]);
				if (num > 0)
				{
					if ((await CheckExistGroupCode(num)).IsNotNull())
					{
						WinCommons.Error(Enums.MessageCode.CannotDeleteThisGroupUsed, "");
						_isDelete = false;
						return;
					}
					_dtRemove.Rows.Add(row.ItemArray);
					_isDelete = true;
					FormMode = Enums.FormViewModeType.Update;
				}
			}
			gvDoc.BeginUpdate();
			gvDoc.DeleteDataSourceRowSelected();
			gvDoc.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> CheckExistGroupCode(int grCode)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await helper.GetAsync("CheckExistGroupCode", new { grCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async void btnCancel_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			WinCommons._OpenProcessing();
			await SaveData();
			WinCommons._CloseProcessing();
		}
		Close();
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvDoc_MouseLeave(object sender, EventArgs e)
	{
		HotTrackRow = int.MinValue;
	}

	private void gvDoc_MouseMove_1(object sender, MouseEventArgs e)
	{
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull() && (e.Column.FieldName == "GroupName" || e.Column.FieldName == "PriceList" || e.Column.FieldName == "DiscRel"))
			{
				if (e.RowHandle == dataTable.Rows.Count - 1)
				{
					AddNewRow();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Interfaces.BusinessPartner.VirFormCustomerVendorGroup));
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Prices = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_effect = new Apzon.Forms.UserControls.RepositoryItemLookUpEditCombo();
		this.rep_Price = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Prices).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_effect).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Price).BeginInit();
		base.SuspendLayout();
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.AllowRestoreSelectionAndFocusedRow = DevExpress.Utils.DefaultBoolean.True;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(1, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_effect, this.repositoryItemTextEdit1, this.rep_Price, this.rep_Prices });
		this.grdDoc.Size = new System.Drawing.Size(745, 370);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdDoc_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteToolStripMenuItem.Text = "Delete Row";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gvDoc.CancelNextCellValueChangedScript = true;
		this.gvDoc.CancelNextFocusObjectChangedScript = true;
		this.gvDoc.CancelNextSelectedChangedScript = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumn4, this.gridColumn1, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn2, this.gridColumn3 });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "VirFormCustomerVendorGroup";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OCRG";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsSelection.MultiSelect = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanging);
		this.gvDoc.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gvDoc_MouseMove_1);
		this.gvDoc.MouseLeave += new System.EventHandler(this.gvDoc_MouseLeave);
		this.gridColumn4.Caption = "GroupCode";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "GroupCode";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn4.PrimaryKey = true;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Width = 179;
		this.gridColumn1.Caption = "Group Name";
		this.gridColumn1.ColumnEdit = this.repositoryItemTextEdit1;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "GroupName";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 236;
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.MaxLength = 20;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.gridColumn5.Caption = "GroupType";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "GroupType";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn6.Caption = "Locked";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "Locked";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn7.Caption = "DataSource";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "DataSource";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn8.Caption = "UserSign";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "UserSign";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn2.Caption = "Price List";
		this.gridColumn2.ColumnEdit = this.rep_Prices;
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "PriceList";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 2;
		this.gridColumn2.Width = 345;
		this.rep_Prices.AutoHeight = false;
		this.rep_Prices.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Prices.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Prices.DisplayMember = "Name";
		this.rep_Prices.Name = "rep_Prices";
		this.rep_Prices.NullText = "";
		this.rep_Prices.ValueMember = "Code";
		this.gridColumn3.Caption = "Effective Discount";
		this.gridColumn3.ColumnEdit = this.rep_effect;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "DiscRel";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn3.Width = 326;
		this.rep_effect.AutoHeight = false;
		this.rep_effect.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFitResizePopup;
		this.rep_effect.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_effect.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_effect.DisplayMember = "Name";
		this.rep_effect.IsSearchControl = false;
		this.rep_effect.Name = "rep_effect";
		this.rep_effect.NullText = "";
		this.rep_effect.ShowFooter = false;
		this.rep_effect.ShowHeader = false;
		this.rep_effect.ValueMember = "Code";
		this.rep_Price.AutoHeight = false;
		this.rep_Price.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Price.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Price.DisplayMember = "Name";
		this.rep_Price.Name = "rep_Price";
		this.rep_Price.NullText = "";
		this.rep_Price.ShowFooter = false;
		this.rep_Price.ShowHeader = false;
		this.rep_Price.ValueMember = "Code";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(1, 375);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(99, 20);
		this.btn_Save.TabIndex = 55;
		this.btn_Save.Text = "Save";
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
		this.btnCancel.Location = new System.Drawing.Point(102, 375);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 54;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(747, 399);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grdDoc);
		base.FormId = "VirFormCustomerVendorGroup";
		base.IconOptions.ShowIcon = false;
		base.Name = "VirFormCustomerVendorGroup";
		this.Text = "Customer Groups";
		base.Load += new System.EventHandler(this.frm_CustomerGroup_Load);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Prices).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_effect).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Price).EndInit();
		base.ResumeLayout(false);
	}
}
