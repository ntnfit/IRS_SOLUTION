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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Forms.Service;

public class frm_ServiceLevelAgreement : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private DataTable _dtDoc;

	private DataTable _dtConverage;

	private DataTable _dtResolution;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isLoading;

	public int SLAID;

	public int SLANo;

	private readonly Dictionary<string, string> _lstLukup = new Dictionary<string, string>
	{
		{ "Y", "Yes" },
		{ "N", "No" }
	};

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private XtraTabControlEx xtraTabControlEx1;

	private XtraTabPageEx xtraTabPage1;

	private GridControlEx grd_Coverage;

	private GridViewEx gv_Coverage;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grd_Resolution;

	private GridViewEx gv_Resolution;

	private SimpleButton btn_Ok;

	private SimpleButton btn_Cancel;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private SimpleButtonEx btn_Search;

	private TextEditEx txt_SLACode;

	private TextEditEx txt_No;

	private LookUpEditSimple luk_ProService;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemLookUpEditSimple luk_ProServiceDoc;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemLookUpEditSimple luk_LaborDoc;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemLookUpEditSimple luk_PartDoc;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemLookUpEditSimple luk_Holiday;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private RepositoryItemLookUpEditSimple luk_Travel;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private GridColumnEx gridColumnEx18;

	private RepositoryItemLookUpEditSimple luk_Day;

	private GridColumnEx gridColumnEx19;

	private RepositoryItemLookUpEditSimple luk_IssueLevel;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private RepositoryItemHourEdit repositoryItemHourEdit1;

	private RepositoryItemHourEdit repositoryItemHourEdit2;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_Ok.Text = "Add";
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = "Update";
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = "OK";
				break;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_ServiceLevelAgreement()
	{
		OnLoadAsync += frm_ServiceLevelAgreement_Load;
		InitializeComponent();
	}

	private async Task<DataTable> LoadData(int id, string SLANo, string proService)
	{
		try
		{
			_isLoading = true;
			DataTable result = await _httpClient.ServiceLevelAgreement.LoadData(id, SLANo, proService);
			_isLoading = false;
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task LoadLineById(int id)
	{
		try
		{
			WinCommons._OpenProcessing();
			_isLoading = true;
			DataSet dataSet = await _httpClient.ServiceLevelAgreement.GetDataById(id);
			if (dataSet.IsNotNull() && dataSet.Tables.Count > 0)
			{
				grd_Coverage.DataSource = dataSet.Tables[0];
				grd_Coverage.DatasourceTable().AcceptChanges();
				grd_Resolution.DataSource = dataSet.Tables[1];
				grd_Resolution.DatasourceTable().AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View || (FormMode != Enums.FormViewModeType.Update && FormMode != Enums.FormViewModeType.Add) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				FormMode = Enums.FormViewModeType.Add;
				grd_Doc.DataSourceTable.Rows.Clear();
				grd_Coverage.DataSourceTable().Rows.Clear();
				grd_Resolution.DataSourceTable().Rows.Clear();
				_dtDoc = grd_Doc.DataSourceTable;
				if (_dtDoc == null || _dtDoc.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				}
				DataRow row = _dtDoc.NewRow();
				DataRow dataRow = row;
				dataRow["SLAID"] = await SetDataNo();
				_dtDoc.Rows.Add(row);
				grd_Doc.DataSource = _dtDoc;
				NewCoverage();
				NewResolution();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void NewCoverage()
	{
		try
		{
			_dtConverage = grd_Coverage.DatasourceTable();
			for (int i = 0; i < 7; i++)
			{
				DataRow rowLine1 = _dtConverage.NewRow();
				rowLine1["Day"] = i;
				DataRow dataRow = rowLine1;
				dataRow["SLAID"] = await SetDataNo();
				rowLine1["LineNum"] = i + 1;
				_dtConverage.Rows.Add(rowLine1);
			}
			_dtConverage.AcceptChanges();
			grd_Coverage.DataSource = _dtConverage;
			grd_Coverage.DatasourceTable().AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void NewResolution()
	{
		try
		{
			_dtResolution = grd_Resolution.DatasourceTable();
			for (int i = 0; i < 4; i++)
			{
				DataRow rowLine1 = _dtResolution.NewRow();
				rowLine1["IssueLevel"] = i;
				DataRow dataRow = rowLine1;
				dataRow["SLAID"] = await SetDataNo();
				rowLine1["LineNum"] = i + 1;
				_dtResolution.Rows.Add(rowLine1);
			}
			_dtResolution.AcceptChanges();
			grd_Resolution.DataSource = _dtResolution;
			grd_Resolution.DatasourceTable().AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DataChanged()
	{
		if (!_isLoading && FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> SetDataNo()
	{
		try
		{
			return await _httpClient.ServiceLevelAgreement.GetMaxId() + 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.View;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task<int> FindNextLastFrist(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.ServiceLevelAgreement.FindNextLastFrist(Function.ParseInt(txt_No.EditValue), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
				grd_Doc.DatasourceTable().AcceptChanges();
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("SLAID"));
				txt_No.EditValue = num;
				await LoadLineById(num);
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async Task<int> Save(int type)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			DataTable dataTable = grd_Doc.DatasourceTable().Copy();
			(from DataRow t in dataTable.Rows
				where string.IsNullOrEmpty(Function.ToString(t["SLACode"]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
			dataTable.AcceptChanges();
			if (type == 2)
			{
				DataTable dataTable2 = dataTable.Copy();
				dataTable2.AcceptChanges();
				DataRow[] array = dataTable2.Select("SLAID=" + Function.ParseInt(gv_Doc.GetFocusedRowCellValue("SLAID")));
				if (array.Length != 0)
				{
					_dtDoc = array.CopyToDataTable();
				}
			}
			else
			{
				_dtDoc = dataTable.Copy();
			}
			_dtConverage = grd_Coverage.DatasourceTable().Copy();
			_dtResolution = grd_Resolution.DatasourceTable().Copy();
			_dtDoc.TableName = "dtDoc";
			_dtConverage.TableName = "Converage";
			_dtResolution.TableName = "Resolution";
			dataSet.Tables.Add(_dtDoc.Copy());
			dataSet.Tables.Add(_dtConverage.Copy());
			dataSet.Tables.Add(_dtResolution.Copy());
			HttpResult httpResult = ((type != 2) ? (await _httpClient.ServiceLevelAgreement.Add(dataSet)) : (await _httpClient.ServiceLevelAgreement.Update(dataSet)));
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				_isLoading = true;
				FormMode = Enums.FormViewModeType.View;
				if (type == 2)
				{
					grd_Doc.DatasourceTable().AcceptChanges();
					grd_Coverage.DatasourceTable().AcceptChanges();
					grd_Resolution.DatasourceTable().AcceptChanges();
				}
				else
				{
					btn_Search_Click(null, null);
				}
				_isLoading = false;
				return 0;
			}
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			WinCommons._CloseProcessing();
			return 1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
	}

	private async Task FocusRowBySLACode()
	{
		try
		{
			DataTable dataTable = await _httpClient.ServiceLevelAgreement.LoadData(SLANo, null, null);
			if (dataTable.IsNotNull())
			{
				grd_Doc.DataSource = dataTable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_ServiceLevelAgreement_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			luk_ProService.SetDataSource(new Dictionary<string, string>
			{
				{ "", "" },
				{ "Y", "Yes" },
				{ "N", "No" }
			});
			luk_ProServiceDoc.SetDataSource(_lstLukup);
			luk_LaborDoc.SetDataSource(_lstLukup);
			luk_PartDoc.SetDataSource(_lstLukup);
			luk_Holiday.SetDataSource(_lstLukup);
			luk_Travel.SetDataSource(_lstLukup);
			luk_Day.SetDataSource(new Dictionary<string, string>
			{
				{ "0", "Monday" },
				{ "1", "Tuesday" },
				{ "2", "Wednesday" },
				{ "3", "Thursday" },
				{ "4", "Friday" },
				{ "5", "Saturday" },
				{ "6", "Sunday" }
			});
			luk_IssueLevel.SetDataSource(new Dictionary<string, string>
			{
				{ "0", "Low" },
				{ "1", "Normal" },
				{ "2", "High" },
				{ "3", "Critical" }
			});
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Doc, "APZ_OSLA"), WinFuntions.LoadUdfColumns(gv_Coverage, "APZ_SLA1"), WinFuntions.LoadUdfColumns(gv_Resolution, "APZ_SLA2"));
			new DataTable();
			string sLANo = Function.ToString(txt_SLACode.EditValue);
			string proService = Function.ToString(luk_ProService.EditValue);
			if (SLAID > 0)
			{
				txt_No.EditValue = SLAID;
			}
			DataTable dataTable = await LoadData(Function.ParseInt(txt_No.EditValue), sLANo, proService);
			SLAID = 0;
			grd_Doc.DataSource = dataTable;
			grd_Doc.DatasourceTable().AcceptChanges();
			if (!dataTable.IsNotNull())
			{
				gv_Doc_FocusedRowChanged(null, null);
			}
			gv_Doc.FocusedRowHandle = 0;
			await FocusRowBySLACode();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string sLANo = Function.ToString(txt_SLACode.EditValue);
			string proService = Function.ToString(luk_ProService.EditValue);
			if (SLAID > 0)
			{
				txt_No.EditValue = SLAID;
			}
			DataTable dataSource = await LoadData(Function.ParseInt(txt_No.EditValue), sLANo, proService);
			SLAID = 0;
			if (!_isLoading && ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes))
			{
				FormMode = Enums.FormViewModeType.View;
				grd_Doc.DataSourceTable.Rows.Clear();
				grd_Coverage.DatasourceTable().Rows.Clear();
				grd_Resolution.DatasourceTable().Rows.Clear();
				grd_Doc.DataSource = dataSource;
				grd_Doc.DatasourceTable().AcceptChanges();
				int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("SLAID"));
				await LoadLineById(id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Save(1);
				break;
			case Enums.FormViewModeType.Update:
				await Save(2);
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

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void gv_Doc_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				int id = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("SLAID"));
				await LoadLineById(id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.Update)
			{
				DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				if (dialogResult == DialogResult.Yes && dialogResult == DialogResult.Yes)
				{
					grd_Doc.DataSourceTable.RejectChanges();
					FormMode = Enums.FormViewModeType.View;
				}
				if (dialogResult == DialogResult.No)
				{
					e.Allow = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private void gv_Coverage_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private void gv_Resolution_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private void gv_Coverage_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (sender is GridViewEx gridViewEx && gridViewEx.FocusedColumn.FieldName == "EndTime")
			{
				int num = Function.ParseInt(e.Value);
				int num2 = Function.ParseInt(gv_Coverage.GetFocusedRowCellValue("StartTime"));
				if (num < num2)
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.EndTimeMustBeGreaterThanStartTime, "");
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Coverage_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			if (sender is GridViewEx)
			{
				e.ExceptionMode = ExceptionMode.DisplayError;
				e.WindowCaption = "Input Error";
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Resolution_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			if (sender is GridViewEx)
			{
				e.ExceptionMode = ExceptionMode.DisplayError;
				e.WindowCaption = "Input Error";
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Resolution_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			if (sender is GridViewEx gridViewEx)
			{
				if (gridViewEx.FocusedColumn.FieldName == "ResponseTime" && !int.TryParse(Function.ToString(e.Value), out var result))
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.ResponseTimeMustBeInterger, "");
				}
				else if (gridViewEx.FocusedColumn.FieldName == "ResolutionTime" && !int.TryParse(Function.ToString(e.Value), out result))
				{
					e.Valid = false;
					e.ErrorText = "";
					WinCommons.Error(Enums.MessageCode.ResolutionTimeMustBeInterger, "");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Coverage_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "StartTime" && Function.ParseInt(e.Value) == 0)
			{
				e.DisplayText = "";
			}
			if (e.Column.FieldName == "EndTime" && Function.ParseInt(e.Value) == 0)
			{
				e.DisplayText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Doc.GetSelectedRows() == null && gv_Doc.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
				return;
			}
			Function.ParseInt(gv_Doc.GetFocusedRowCellValue("SLAID"));
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveSla, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				string id = Function.ToString(gv_Doc.GetFocusedRowCellValue("SLAID"));
				string text = await _httpClient.ServiceLevelAgreement.DeleteDocs(id);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					btn_Search_Click(null, null);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Service.frm_ServiceLevelAgreement));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_ProServiceDoc = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_LaborDoc = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_PartDoc = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Holiday = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Travel = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.xtraTabControlEx1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Coverage = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Coverage = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Day = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHourEdit1 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemHourEdit2 = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Resolution = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Resolution = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_IssueLevel = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Ok = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_SLACode = new Apzon.Controls.Editor.TextEditEx();
		this.txt_No = new Apzon.Controls.Editor.TextEditEx();
		this.luk_ProService = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ProServiceDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_LaborDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PartDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Holiday).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Travel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).BeginInit();
		this.xtraTabControlEx1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Coverage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Coverage).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Day).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit2).BeginInit();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Resolution).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Resolution).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_IssueLevel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SLACode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ProService.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 69);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControlEx1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(950, 332);
		this.splitContainerControl1.SplitterPosition = 127;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.luk_ProServiceDoc, this.luk_LaborDoc, this.luk_PartDoc, this.luk_Holiday, this.luk_Travel });
		this.grd_Doc.Size = new System.Drawing.Size(950, 127);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(124, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oServiceLevelAgreement";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "APZ_OSLA";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Doc_FocusedRowChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.gv_Doc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Doc_BeforeLeaveRow);
		this.gridColumnEx1.Caption = "SLA Id";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "SLAID";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx2.Caption = "SLA Code";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "SLACode";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 1;
		this.gridColumnEx2.Width = 94;
		this.gridColumnEx3.Caption = "SLA Name";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "SLAName";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 2;
		this.gridColumnEx3.Width = 116;
		this.gridColumnEx4.Caption = "Pro Service";
		this.gridColumnEx4.ColumnEdit = this.luk_ProServiceDoc;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "ProService";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.gridColumnEx4.Width = 117;
		this.luk_ProServiceDoc.AutoHeight = false;
		this.luk_ProServiceDoc.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_ProServiceDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ProServiceDoc.IsSearchControl = false;
		this.luk_ProServiceDoc.Name = "luk_ProServiceDoc";
		this.luk_ProServiceDoc.NullText = "";
		this.luk_ProServiceDoc.ShowFooter = false;
		this.luk_ProServiceDoc.ShowHeader = false;
		this.gridColumnEx5.Caption = "Labor";
		this.gridColumnEx5.ColumnEdit = this.luk_LaborDoc;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "Labor";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.luk_LaborDoc.AutoHeight = false;
		this.luk_LaborDoc.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_LaborDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_LaborDoc.IsSearchControl = false;
		this.luk_LaborDoc.Name = "luk_LaborDoc";
		this.luk_LaborDoc.NullText = "";
		this.luk_LaborDoc.ShowFooter = false;
		this.luk_LaborDoc.ShowHeader = false;
		this.gridColumnEx6.Caption = "Parts";
		this.gridColumnEx6.ColumnEdit = this.luk_PartDoc;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Parts";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 5;
		this.luk_PartDoc.AutoHeight = false;
		this.luk_PartDoc.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_PartDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_PartDoc.IsSearchControl = false;
		this.luk_PartDoc.Name = "luk_PartDoc";
		this.luk_PartDoc.NullText = "";
		this.luk_PartDoc.ShowFooter = false;
		this.luk_PartDoc.ShowHeader = false;
		this.gridColumnEx7.Caption = "Holidays";
		this.gridColumnEx7.ColumnEdit = this.luk_Holiday;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "Holidays";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 7;
		this.luk_Holiday.AutoHeight = false;
		this.luk_Holiday.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Holiday.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Holiday.IsSearchControl = false;
		this.luk_Holiday.Name = "luk_Holiday";
		this.luk_Holiday.NullText = "";
		this.luk_Holiday.ShowFooter = false;
		this.luk_Holiday.ShowHeader = false;
		this.gridColumnEx8.Caption = "Remark";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "Remark";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 8;
		this.gridColumnEx8.Width = 246;
		this.gridColumnEx9.Caption = "Travel";
		this.gridColumnEx9.ColumnEdit = this.luk_Travel;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "Travel";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 6;
		this.luk_Travel.AutoHeight = false;
		this.luk_Travel.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Travel.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Travel.IsSearchControl = false;
		this.luk_Travel.Name = "luk_Travel";
		this.luk_Travel.NullText = "";
		this.luk_Travel.ShowFooter = false;
		this.luk_Travel.ShowHeader = false;
		this.xtraTabControlEx1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControlEx1.IsSearchControl = false;
		this.xtraTabControlEx1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControlEx1.Name = "xtraTabControlEx1";
		this.xtraTabControlEx1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControlEx1.Size = new System.Drawing.Size(950, 195);
		this.xtraTabControlEx1.TabIndex = 0;
		this.xtraTabControlEx1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage1, this.xtraTabPage2 });
		this.xtraTabPage1.Controls.Add(this.grd_Coverage);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Coverage";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(948, 170);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Coverage";
		this.grd_Coverage.AllowPasteExcel = false;
		this.grd_Coverage.ClipboardData = null;
		this.grd_Coverage.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Coverage.Location = new System.Drawing.Point(0, 0);
		this.grd_Coverage.MainView = this.gv_Coverage;
		this.grd_Coverage.Name = "grd_Coverage";
		this.grd_Coverage.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.luk_Day, this.repositoryItemHourEdit1, this.repositoryItemHourEdit2 });
		this.grd_Coverage.Size = new System.Drawing.Size(948, 170);
		this.grd_Coverage.TabIndex = 0;
		this.grd_Coverage.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Coverage });
		this.gv_Coverage.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Coverage.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Coverage.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Coverage.ColumnKey = null;
		this.gv_Coverage.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx12, this.gridColumnEx13, this.gridColumnEx18 });
		this.gv_Coverage.DisableFormSetting = false;
		this.gv_Coverage.DisplayException = false;
		this.gv_Coverage.EditColumn = null;
		this.gv_Coverage.FieldCurrency = null;
		this.gv_Coverage.FieldRate = null;
		this.gv_Coverage.FormId = "oServiceLevelAgreement";
		this.gv_Coverage.GridControl = this.grd_Coverage;
		this.gv_Coverage.IgnoreFilter = false;
		this.gv_Coverage.IndexEditRow = -1;
		this.gv_Coverage.IndicatorWidth = 35;
		this.gv_Coverage.IsApzTable = false;
		this.gv_Coverage.IsInsertNewRow = false;
		this.gv_Coverage.IsLoadingData = false;
		this.gv_Coverage.IsMappingContextAction = false;
		this.gv_Coverage.ItemId = "APZ_SLA1";
		this.gv_Coverage.LstColumnAllowBatch = null;
		this.gv_Coverage.LstColumnNotAllowBatch = null;
		this.gv_Coverage.LstColumnNotEditable = null;
		this.gv_Coverage.LstColumNotSetting = null;
		this.gv_Coverage.Name = "gv_Coverage";
		this.gv_Coverage.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Coverage.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Coverage.OptionsView.ColumnAutoWidth = false;
		this.gv_Coverage.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Coverage.OptionsView.ShowGroupPanel = false;
		this.gv_Coverage.UseBatchMod = false;
		this.gv_Coverage.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Coverage_CellValueChanging);
		this.gv_Coverage.CustomColumnDisplayText += new DevExpress.XtraGrid.Views.Base.CustomColumnDisplayTextEventHandler(this.gv_Coverage_CustomColumnDisplayText);
		this.gv_Coverage.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Coverage_ValidatingEditor);
		this.gv_Coverage.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Coverage_InvalidValueException);
		this.gridColumnEx10.Caption = "Day";
		this.gridColumnEx10.ColumnEdit = this.luk_Day;
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "Day";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.OptionsColumn.AllowEdit = false;
		this.gridColumnEx10.OptionsColumn.ReadOnly = true;
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 0;
		this.luk_Day.AutoHeight = false;
		this.luk_Day.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Day.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Day.IsSearchControl = false;
		this.luk_Day.Name = "luk_Day";
		this.luk_Day.NullText = "";
		this.luk_Day.ShowFooter = false;
		this.luk_Day.ShowHeader = false;
		this.gridColumnEx11.Caption = "Start Time";
		this.gridColumnEx11.ColumnEdit = this.repositoryItemHourEdit1;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "StartTime";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 1;
		this.gridColumnEx11.Width = 90;
		this.repositoryItemHourEdit1.AutoHeight = false;
		this.repositoryItemHourEdit1.IsSearchControl = false;
		this.repositoryItemHourEdit1.MaxLength = 5;
		this.repositoryItemHourEdit1.Name = "repositoryItemHourEdit1";
		this.gridColumnEx12.Caption = "End Time";
		this.gridColumnEx12.ColumnEdit = this.repositoryItemHourEdit2;
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "EndTime";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 2;
		this.gridColumnEx12.Width = 98;
		this.repositoryItemHourEdit2.AutoHeight = false;
		this.repositoryItemHourEdit2.IsSearchControl = false;
		this.repositoryItemHourEdit2.MaxLength = 5;
		this.repositoryItemHourEdit2.Name = "repositoryItemHourEdit2";
		this.gridColumnEx13.Caption = "SLAID";
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.FieldName = "SLAID";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.OptionsColumn.AllowEdit = false;
		this.gridColumnEx13.OptionsColumn.ReadOnly = true;
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx18.Caption = "LineNum";
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "LineNum";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.OptionsColumn.AllowEdit = false;
		this.gridColumnEx18.OptionsColumn.ReadOnly = true;
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.xtraTabPage2.Controls.Add(this.grd_Resolution);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "Resolutions";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(948, 170);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Resolutions";
		this.grd_Resolution.AllowPasteExcel = true;
		this.grd_Resolution.ClipboardData = null;
		this.grd_Resolution.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Resolution.Location = new System.Drawing.Point(0, 0);
		this.grd_Resolution.MainView = this.gv_Resolution;
		this.grd_Resolution.Name = "grd_Resolution";
		this.grd_Resolution.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.luk_IssueLevel });
		this.grd_Resolution.Size = new System.Drawing.Size(948, 170);
		this.grd_Resolution.TabIndex = 0;
		this.grd_Resolution.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Resolution });
		this.gv_Resolution.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Resolution.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Resolution.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Resolution.ColumnKey = null;
		this.gv_Resolution.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx16, this.gridColumnEx17, this.gridColumnEx19 });
		this.gv_Resolution.DisableFormSetting = false;
		this.gv_Resolution.DisplayException = false;
		this.gv_Resolution.EditColumn = null;
		this.gv_Resolution.FieldCurrency = null;
		this.gv_Resolution.FieldRate = null;
		this.gv_Resolution.FormId = "oServiceLevelAgreement";
		this.gv_Resolution.GridControl = this.grd_Resolution;
		this.gv_Resolution.IgnoreFilter = false;
		this.gv_Resolution.IndexEditRow = -1;
		this.gv_Resolution.IndicatorWidth = 35;
		this.gv_Resolution.IsApzTable = false;
		this.gv_Resolution.IsInsertNewRow = false;
		this.gv_Resolution.IsLoadingData = false;
		this.gv_Resolution.IsMappingContextAction = false;
		this.gv_Resolution.ItemId = "APZ_SLA2";
		this.gv_Resolution.LstColumnAllowBatch = null;
		this.gv_Resolution.LstColumnNotAllowBatch = null;
		this.gv_Resolution.LstColumnNotEditable = null;
		this.gv_Resolution.LstColumNotSetting = null;
		this.gv_Resolution.Name = "gv_Resolution";
		this.gv_Resolution.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Resolution.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Resolution.OptionsView.ColumnAutoWidth = false;
		this.gv_Resolution.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Resolution.OptionsView.ShowGroupPanel = false;
		this.gv_Resolution.UseBatchMod = false;
		this.gv_Resolution.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Resolution_CellValueChanging);
		this.gv_Resolution.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Resolution_ValidatingEditor);
		this.gv_Resolution.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Resolution_InvalidValueException);
		this.gridColumnEx14.Caption = "SLAID";
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = null;
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.FieldName = "SLAID";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.OptionsColumn.AllowEdit = false;
		this.gridColumnEx14.OptionsColumn.ReadOnly = true;
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx15.Caption = "Issue Level";
		this.gridColumnEx15.ColumnEdit = this.luk_IssueLevel;
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = null;
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.FieldName = "IssueLevel";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.OptionsColumn.AllowEdit = false;
		this.gridColumnEx15.OptionsColumn.ReadOnly = true;
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 0;
		this.luk_IssueLevel.AutoHeight = false;
		this.luk_IssueLevel.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_IssueLevel.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_IssueLevel.IsSearchControl = false;
		this.luk_IssueLevel.Name = "luk_IssueLevel";
		this.luk_IssueLevel.NullText = "";
		this.luk_IssueLevel.ShowFooter = false;
		this.luk_IssueLevel.ShowHeader = false;
		this.gridColumnEx16.Caption = "Response Time";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Integer;
		this.gridColumnEx16.FieldName = "ResponseTime";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 1;
		this.gridColumnEx16.Width = 102;
		this.gridColumnEx17.Caption = "Resolution Time";
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Integer;
		this.gridColumnEx17.FieldName = "ResolutionTime";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 2;
		this.gridColumnEx17.Width = 108;
		this.gridColumnEx19.Caption = "LineNum";
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.FieldName = "LineNum";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.OptionsColumn.AllowEdit = false;
		this.gridColumnEx19.OptionsColumn.ReadOnly = true;
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.Location = new System.Drawing.Point(0, 407);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "Ok";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(81, 407);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "SLA Code";
		this.labelLine1.FieldName = "";
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(1, 13);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 3;
		this.labelLine1.Utext = "SLA Code";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Pro Service";
		this.labelLine2.FieldName = "";
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(1, 49);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 4;
		this.labelLine2.Utext = "Pro Service";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "SLA Id";
		this.labelLine3.FieldName = "";
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(193, 46);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(75, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 5;
		this.labelLine3.Utext = "SLA Id";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(391, 40);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 7;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.txt_SLACode.EditControl = null;
		this.txt_SLACode.FieldName = null;
		this.txt_SLACode.IsSearchControl = true;
		this.txt_SLACode.Location = new System.Drawing.Point(68, 10);
		this.txt_SLACode.Name = "txt_SLACode";
		this.txt_SLACode.SettingInVisible = false;
		this.txt_SLACode.SettingReadonly = false;
		this.txt_SLACode.Size = new System.Drawing.Size(100, 20);
		this.txt_SLACode.TabIndex = 8;
		this.txt_No.EditControl = null;
		this.txt_No.FieldName = null;
		this.txt_No.IsSearchControl = true;
		this.txt_No.Location = new System.Drawing.Point(264, 43);
		this.txt_No.Name = "txt_No";
		this.txt_No.SettingInVisible = false;
		this.txt_No.SettingReadonly = false;
		this.txt_No.Size = new System.Drawing.Size(100, 20);
		this.txt_No.TabIndex = 9;
		this.luk_ProService.EditControl = null;
		this.luk_ProService.FieldName = null;
		this.luk_ProService.IsSearchControl = true;
		this.luk_ProService.Location = new System.Drawing.Point(68, 46);
		this.luk_ProService.Name = "luk_ProService";
		this.luk_ProService.Properties.AutoHeight = false;
		this.luk_ProService.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_ProService.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_ProService.Properties.IsSearchControl = false;
		this.luk_ProService.Properties.NullText = "";
		this.luk_ProService.Properties.ShowFooter = false;
		this.luk_ProService.Properties.ShowHeader = false;
		this.luk_ProService.SettingInVisible = false;
		this.luk_ProService.SettingReadonly = false;
		this.luk_ProService.Size = new System.Drawing.Size(100, 20);
		this.luk_ProService.TabIndex = 10;
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(951, 430);
		base.Controls.Add(this.luk_ProService);
		base.Controls.Add(this.txt_No);
		base.Controls.Add(this.txt_SLACode);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oServiceLevelAgreement";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ServiceLevelAgreement";
		this.Text = "Service Level Agreement";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ProServiceDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_LaborDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_PartDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Holiday).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Travel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControlEx1).EndInit();
		this.xtraTabControlEx1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Coverage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Coverage).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Day).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemHourEdit2).EndInit();
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Resolution).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Resolution).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_IssueLevel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SLACode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_ProService.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
