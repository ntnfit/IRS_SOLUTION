using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
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

namespace Apzon.Forms.KPIs;

public class frm_KPIsMasterData : XtraFormEx
{
	private const string OkString = "OK";

	private const string AddString = "Save";

	private DataTable _dtDoc;

	private bool _isLoading;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private RepositoryItemLookUpEditSimple luk_Status;

	private GridColumnEx gridColumnEx9;

	private RepositoryItemMemoExEdit memo_Remarks;

	private RepositoryItemLookUpEditSimple luk_FType;

	private RepositoryItemLookUpEditSimple luk_KpiType;

	private RepositoryItemLookUpEdit luk_Currency;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btn_Closed;

	private SimpleButtonEx btn_Search;

	private RepositoryItemLookUpEditDefineNew luk_FCode;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private TextEditEx txt_Code;

	private TextEditEx txt_Name;

	private LookUpEditSimple luk_StatusFind;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private RepositoryItemLookUpEdit luc_FoCode;

	public string Code { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Closed.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_Save.Text = "Save";
				break;
			case Enums.FormViewModeType.View:
				btn_Save.Text = "OK";
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_KPIsMasterData()
	{
		InitializeComponent();
		OnLoadAsync += Frm_KPIsMasterData_Load;
	}

	private async Task Frm_KPIsMasterData_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			Dictionary<string, string> dic = GlobalData.CurrencyCodes.ToDictionary((CurrencyCodesImpl item) => item.CurrCode, (CurrencyCodesImpl item) => item.CurrName);
			luk_Currency.SetDataSource(dic);
			luk_Status.SetDataSource(new Dictionary<string, string>
			{
				{ "Y", "Hoạt động" },
				{ "N", "Không hoạt động" }
			});
			luk_StatusFind.SetDataSource(new Dictionary<string, string>
			{
				{ "Y", "Hoạt động" },
				{ "N", "Không hoạt động" }
			});
			luk_KpiType.SetDataSource(new Dictionary<string, string>
			{
				{ "Q", "Số lượng" },
				{ "C", "Tiền tệ" },
				{ "R", "%" }
			});
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, "APZ_D_OKPI"));
			SetDataFormulaCode();
			luk_FType.SetDataSource(new Dictionary<string, string>
			{
				{ "F", "Công thức" },
				{ "M", "Nhập tay" }
			});
			if (!string.IsNullOrEmpty(Code))
			{
				txt_Code.EditValue = Code;
			}
			new DataTable();
			DataTable dataTable = await LoadData(Function.ToString(txt_Code.EditValue), Function.ToString(txt_Name.EditValue), Function.ToString(luk_StatusFind.EditValue));
			Code = "";
			dataTable.Columns.Add("IsDb", typeof(string));
			for (int num = 0; num < dataTable.Rows.Count; num++)
			{
				dataTable.Rows[num]["IsDb"] = "Y";
			}
			grd_Doc.DataSource = dataTable;
			grd_Doc.DatasourceTable().AcceptChanges();
			gv_Doc.FocusedRowHandle = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	public async void SetDataFormulaCode()
	{
		try
		{
			DataTable dataTable = await _httpClient.KPIsMaterDataClient.LoadDataFormulaCode();
			luc_FoCode.DisplayMember = "Name";
			luc_FoCode.ValueMember = "Code";
			luc_FoCode.DataSource = dataTable.Copy();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Code))
			{
				txt_Code.EditValue = Code;
			}
			string code = Function.ToString(txt_Code.EditValue);
			string name = Function.ToString(txt_Name.EditValue);
			string status = Function.ToString(luk_StatusFind.EditValue);
			DataTable dataTable = await LoadData(code, name, status);
			Code = "";
			dataTable.Columns.Add("IsDb", typeof(string));
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				dataTable.Rows[i]["IsDb"] = "Y";
			}
			if (!_isLoading && (FormMode != Enums.FormViewModeType.Add || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes))
			{
				FormMode = Enums.FormViewModeType.View;
				grd_Doc.DataSourceTable.Rows.Clear();
				grd_Doc.DataSource = dataTable;
				grd_Doc.DatasourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					await Save();
				}
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Btn_Closed_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private void Gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DataChanged()
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			Btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Frm_KPIsMasterData_FormClosing(object sender, FormClosingEventArgs e)
	{
	}

	public async Task REFRESH()
	{
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			_dtDoc = grd_Doc.DataSourceTable;
			if (_dtDoc == null || _dtDoc.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
			}
			DataRow dataRow = _dtDoc.NewRow();
			dataRow["StatusCode"] = "Y";
			dataRow["CreateDate"] = DateTime.Now.Date;
			dataRow["CreateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			dataRow["UpdateDate"] = DateTime.Now.Date;
			dataRow["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
			dataRow["Usersign"] = GlobalData.UserID;
			dataRow["Usersign2"] = GlobalData.UserID;
			_dtDoc.Rows.Add(dataRow);
			grd_Doc.DataSource = _dtDoc;
			DataChanged();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			REFRESH();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RemoveToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.GetSelectedRows() == null || gv_Doc.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.Yes)
				{
					return;
				}
				string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("Code"));
				if (!string.IsNullOrEmpty(text))
				{
					string text2 = await _httpClient.KPIsMaterDataClient.DeleteDocs(text);
					if (string.IsNullOrEmpty(text2))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						Btn_Search_Click(null, null);
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text2);
					}
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> LoadData(string code, string name, string status)
	{
		try
		{
			_isLoading = true;
			DataTable result = await _httpClient.KPIsMaterDataClient.LoadData(code, name, status);
			_isLoading = false;
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<int> Save()
	{
		try
		{
			WinCommons._OpenProcessing();
			_dtDoc = grd_Doc.DataSourceTable;
			List<string> list = (from dr in _dtDoc.Copy().AsEnumerable()
				select dr.Field<string>("Code") into x
				group x by x into g
				where g.Count() > 1
				select g.Key).ToList();
			if (list.Count() > 0)
			{
				WinCommons.Error(Enums.MessageCode.ValueExistsInTableWithId, "", Function.ToString(list[0]));
				return 1;
			}
			if (_dtDoc != null && _dtDoc.Rows.Count > 0)
			{
				for (int num = 0; num < _dtDoc.Rows.Count; num++)
				{
					if (Function.ToString(_dtDoc.Rows[num]["Code"]) == "" && Function.ToString(_dtDoc.Rows[num]["Name"]) == "" && Function.ToString(_dtDoc.Rows[num]["DisplayCode"]) == "")
					{
						_dtDoc.Rows.RemoveAt(num--);
					}
					if (Function.ToString(_dtDoc.Rows[num]["Code"]) == "")
					{
						WinCommons.Warning(Enums.MessageCode.CodeNotNull, "");
						return 1;
					}
					_dtDoc.Rows[num]["UpdateDate"] = DateTime.Now.Date;
					_dtDoc.Rows[num]["UpdateTime"] = Function.ParseTimeSpanToInt(DateTime.Now.TimeOfDay);
				}
			}
			_dtDoc.TableName = "dtDoc";
			HttpResult httpResult = await _httpClient.KPIsMaterDataClient.Saves(_dtDoc);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				_isLoading = true;
				FormMode = Enums.FormViewModeType.View;
				Btn_Search_Click(null, null);
				_isLoading = false;
				return 0;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			WinCommons._CloseProcessing();
			return 1;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
	}

	private void Gv_Doc_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		if (!(e.Column.FieldName != "FormulaCode"))
		{
			if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "FormulaType")) == "F")
			{
				e.RepositoryItem = luc_FoCode;
			}
			else
			{
				e.RepositoryItem = null;
			}
		}
	}

	private void Gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Doc.GetFocusedRowCellValue("IsDb"));
			if (gv_Doc.FocusedColumn.FieldName == "Code" && text.Equals("Y"))
			{
				gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = true;
			}
			else
			{
				gv_Doc.Columns["Code"].OptionsColumn.ReadOnly = false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "IsDb"));
		if (e.Column.FieldName == "Code" && text.Equals("Y"))
		{
			e.Appearance.BackColor = GridColor.GridReadOnly;
		}
	}

	private void Gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.KPIs.frm_KPIsMasterData));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_FType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_KpiType = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Currency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Status = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.memo_Remarks = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.luc_FoCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Closed = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Name = new Apzon.Controls.Editor.TextEditEx();
		this.luk_StatusFind = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_KpiType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remarks).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luc_FoCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusFind.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(3, 54);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.memo_Remarks, this.luk_Status, this.luk_Currency, this.luk_KpiType, this.luk_FType, this.luc_FoCode });
		this.grd_Doc.Size = new System.Drawing.Size(836, 237);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.AddNewToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.removeToolStripMenuItem.Text = "Delete";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.RemoveToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clKPIsMasterData;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oKPIsMasterData";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = true;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_D_OKPI";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.Gv_Doc_RowCellStyle);
		this.gv_Doc.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.Gv_Doc_CustomRowCellEdit);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.Gv_Doc_ShowingEditor);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.Gv_Doc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.Gv_Doc_CellValueChanging);
		this.gridColumnEx1.Caption = "Mã KPI";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "Code";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx2.Caption = "Tên KPI";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Name";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 135;
		this.gridColumnEx3.Caption = "Mã hiển thị";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "DisplayCode";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 108;
		this.gridColumnEx4.Caption = "Loại công thức";
		this.gridColumnEx4.ColumnEdit = this.luk_FType;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "FormulaType";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx4.Width = 100;
		this.luk_FType.AutoHeight = false;
		this.luk_FType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_FType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_FType.IsSearchControl = false;
		this.luk_FType.Name = "luk_FType";
		this.luk_FType.NullText = "";
		this.luk_FType.ShowFooter = false;
		this.luk_FType.ShowHeader = false;
		this.gridColumnEx5.Caption = "Công thức";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "FormulaCode";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gridColumnEx5.Width = 160;
		this.gridColumnEx6.Caption = "Kiểu";
		this.gridColumnEx6.ColumnEdit = this.luk_KpiType;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "KpiType";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 5;
		this.luk_KpiType.AutoHeight = false;
		this.luk_KpiType.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_KpiType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_KpiType.IsSearchControl = false;
		this.luk_KpiType.Name = "luk_KpiType";
		this.luk_KpiType.NullText = "";
		this.luk_KpiType.ShowFooter = false;
		this.luk_KpiType.ShowHeader = false;
		this.gridColumnEx7.Caption = "Đồng tiền";
		this.gridColumnEx7.ColumnEdit = this.luk_Currency;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "Currency";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 6;
		this.gridColumnEx7.Width = 106;
		this.luk_Currency.AutoHeight = false;
		this.luk_Currency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Currency.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_Currency.DisplayMember = "Name";
		this.luk_Currency.Name = "luk_Currency";
		this.luk_Currency.NullText = "";
		this.luk_Currency.ValueMember = "Code";
		this.gridColumnEx8.Caption = "Trạng thái";
		this.gridColumnEx8.ColumnEdit = this.luk_Status;
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "StatusCode";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 7;
		this.luk_Status.AutoHeight = false;
		this.luk_Status.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.IsSearchControl = false;
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.NullText = "";
		this.luk_Status.ShowFooter = false;
		this.luk_Status.ShowHeader = false;
		this.gridColumnEx9.Caption = "Ghi chú";
		this.gridColumnEx9.ColumnEdit = this.memo_Remarks;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "Remarks";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 8;
		this.gridColumnEx9.Width = 212;
		this.memo_Remarks.AutoHeight = false;
		this.memo_Remarks.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.memo_Remarks.Name = "memo_Remarks";
		this.memo_Remarks.ShowIcon = false;
		this.luc_FoCode.AutoHeight = false;
		this.luc_FoCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luc_FoCode.DisplayMember = "Code";
		this.luc_FoCode.Name = "luc_FoCode";
		this.luc_FoCode.NullText = "";
		this.luc_FoCode.ValueMember = "Name";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(13, 297);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(59, 23);
		this.btn_Save.TabIndex = 1;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.Btn_Save_Click);
		this.btn_Closed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Closed.EditControl = null;
		this.btn_Closed.EditValue = "Closed";
		this.btn_Closed.FieldName = null;
		this.btn_Closed.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Closed.IsSearchControl = false;
		this.btn_Closed.Location = new System.Drawing.Point(76, 297);
		this.btn_Closed.Name = "btn_Closed";
		this.btn_Closed.ReadOnly = false;
		this.btn_Closed.SettingInVisible = false;
		this.btn_Closed.SettingReadonly = false;
		this.btn_Closed.Size = new System.Drawing.Size(65, 23);
		this.btn_Closed.TabIndex = 2;
		this.btn_Closed.Text = "Closed";
		this.btn_Closed.Click += new System.EventHandler(this.Btn_Closed_Click);
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(655, 9);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 3;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.Btn_Search_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Mã KPI";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(11, 15);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 4;
		this.labelLine1.Utext = "Mã KPI";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Tên KPI";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(219, 15);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 5;
		this.labelLine2.Utext = "Tên KPI";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Trạng thái";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(413, 15);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 6;
		this.labelLine3.Utext = "Trạng thái";
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = false;
		this.txt_Code.Location = new System.Drawing.Point(86, 12);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(100, 20);
		this.txt_Code.TabIndex = 7;
		this.txt_Name.EditControl = null;
		this.txt_Name.FieldName = null;
		this.txt_Name.IsSearchControl = false;
		this.txt_Name.Location = new System.Drawing.Point(297, 12);
		this.txt_Name.Name = "txt_Name";
		this.txt_Name.SettingInVisible = false;
		this.txt_Name.SettingReadonly = false;
		this.txt_Name.Size = new System.Drawing.Size(100, 20);
		this.txt_Name.TabIndex = 8;
		this.luk_StatusFind.EditControl = null;
		this.luk_StatusFind.FieldName = null;
		this.luk_StatusFind.IsSearchControl = false;
		this.luk_StatusFind.Location = new System.Drawing.Point(493, 12);
		this.luk_StatusFind.Name = "luk_StatusFind";
		this.luk_StatusFind.Properties.AutoHeight = false;
		this.luk_StatusFind.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusFind.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusFind.Properties.IsSearchControl = false;
		this.luk_StatusFind.Properties.NullText = "";
		this.luk_StatusFind.Properties.ShowFooter = false;
		this.luk_StatusFind.Properties.ShowHeader = false;
		this.luk_StatusFind.SettingInVisible = false;
		this.luk_StatusFind.SettingReadonly = false;
		this.luk_StatusFind.Size = new System.Drawing.Size(100, 20);
		this.luk_StatusFind.TabIndex = 9;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(841, 328);
		base.Controls.Add(this.luk_StatusFind);
		base.Controls.Add(this.txt_Name);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.btn_Closed);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_Doc);
		base.FormId = "oKPIsMasterData";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_KPIsMasterData";
		this.Text = "Khai báo KPIs";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Frm_KPIsMasterData_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_FType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_KpiType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.memo_Remarks).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luc_FoCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Name.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusFind.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
