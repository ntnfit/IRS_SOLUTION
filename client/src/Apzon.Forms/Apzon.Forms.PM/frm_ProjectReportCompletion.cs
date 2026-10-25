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
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.PM;

public class frm_ProjectReportCompletion : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private DataTable _dtDoc;

	private DataTable _dtLine;

	private DataTable _dtClone;

	private DataTable _dtCopy;

	private bool _isLoading;

	private bool _isCpyWbs;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly RepositoryItemLookUpEdit _typeP = new RepositoryItemLookUpEdit();

	private readonly RepositoryItemLookUpEdit _typeC = new RepositoryItemLookUpEdit();

	private List<CurrencyCodesImpl> CurrData;

	private readonly Dictionary<string, string> _lstValidP = new Dictionary<string, string>
	{
		{ "P", "Planned" },
		{ "O", "Open" }
	};

	private readonly Dictionary<string, string> _lstValidC = new Dictionary<string, string>
	{
		{ "P", "Planned" },
		{ "O", "Open" },
		{ "C", "Canceled" },
		{ "L", "Closed" }
	};

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Report;

	private GridViewEx gv_Report;

	private GridControlEx grd_LineReport;

	private GridViewEx gv_LineReport;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private GridColumnEx gridColumnEx11;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gridColumnEx13;

	private GridColumnEx gridColumnEx14;

	private GridColumnEx gridColumnEx15;

	private GridColumnEx gridColumnEx16;

	private GridColumnEx gridColumnEx17;

	private SimpleButton btn_Ok;

	private SimpleButton btn_Cancel;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private SimpleButtonEx btn_Search;

	private ButtonEditCFL btn_ProjectCode;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ContextMenuStrip ctm_Line;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private RepositoryItemDateEditEx dat_Posting;

	private DateEditEx dat_FromDate;

	private DateEditEx dat_ToDate;

	private GridColumnEx gridColumnEx2;

	private LookUpEditSimple luk_Status;

	private LabelLine labelLine1;

	private RepositoryItemLookUpEditSimple luk_StatusGrd;

	private ToolStripMenuItem canceledToolStripMenuItem;

	private ToolStripMenuItem closeToolStripMenuItem;

	private GridColumnEx gridColumnEx18;

	private GridColumnEx gridColumnEx19;

	private DropDownButton drBtn_CopyTo;

	private RepositoryItemLookUpEditSimple luk_StatusLine;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gridColumnEx21;

	private RepositoryItemButtonEditCFL btn_PjCode;

	private RepositoryItemButtonEditCFL btn_Wbs;

	private RepositoryItemButtonEditCFL btn_Item;

	private GridColumnEx gridColumnEx22;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx24;

	private GridColumnEx gridColumnEx25;

	private GridColumnEx gridColumnEx26;

	private GridColumnEx gridColumnEx27;

	private GridColumnEx gridColumnEx28;

	private GridColumnEx gridColumnEx29;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private RepositoryItemDateEditEx repositoryItemDateEditEx2;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit2;

	private ToolStripMenuItem viewDiagramToolStripMenuItem;

	private TextEditEx txt_Code;

	private LabelLine labelLine5;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemTextEditExPrice repositoryItemTextEditExPrice1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx5;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx6;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx7;

	private GridColumnEx gridColumnEx30;

	private RepositoryItemLookUpEdit luk_DocCurr;

	private GridColumnEx gridColumnEx31;

	private RepositoryItemLookUpEdit luk_Currency;

	public int PjReportId { get; set; }

	public List<ApzObjectTypes> PopCopyFrom { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				btn_Ok.Text = "Add";
				drBtn_CopyTo.Enabled = false;
				closeToolStripMenuItem.Enabled = false;
				canceledToolStripMenuItem.Enabled = false;
				break;
			case Enums.FormViewModeType.Update:
				btn_Ok.Text = "Update";
				drBtn_CopyTo.Enabled = true;
				closeToolStripMenuItem.Enabled = true;
				canceledToolStripMenuItem.Enabled = true;
				break;
			case Enums.FormViewModeType.View:
				btn_Ok.Text = "OK";
				drBtn_CopyTo.Enabled = true;
				closeToolStripMenuItem.Enabled = true;
				canceledToolStripMenuItem.Enabled = true;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_ProjectReportCompletion()
	{
		InitializeComponent();
		OnLoadAsync += frm_ReportCompletion_Load;
		RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_PjCode;
		repositoryItemButtonEditCFL.AfterShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.AfterShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
		{
			try
			{
				DataTable dataTable = frm.gv_CFL.GridControl.DataSource as DataTable;
				if (dataTable.Rows.Count > 0 && dataTable != null)
				{
					DataRow[] source = (from s in dataTable.AsEnumerable()
						where Function.ToString(s["PrgStatus"]) == "S" || Function.ToString(s["PrgStatus"]) == "P"
						select s).ToArray();
					if (source.Count() > 0)
					{
						frm.tree.DataSource = source.CopyToDataTable();
					}
					else
					{
						dataTable.Clear();
						dataTable.AcceptChanges();
					}
				}
			}
			catch (Exception ex)
			{
				WinCommons.ErrorLineNumber(ex);
			}
		}));
		gv_Report.FormId = FormId;
		gv_Report.ItemId = "APZ_ORTC";
		gv_LineReport.FormId = FormId;
		gv_LineReport.ItemId = "APZ_RTC1";
		gv_Report.ChangeLogType = Enums.ChangeLog.clReportCompletion;
		gv_Report.IsApzTable = true;
	}

	public frm_ProjectReportCompletion(DataTable dtClone, DataTable dtDoc)
		: this()
	{
		_isCpyWbs = true;
		_dtClone = dtClone;
		grd_LineReport.DataSource = _dtClone;
		_dtCopy = dtDoc;
		grd_Report.DataSource = _dtCopy;
		FormMode = Enums.FormViewModeType.Add;
	}

	private async Task<int> Save(int type)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			if (type == 2)
			{
				DataTable dataSourceTable = grd_Report.DataSourceTable;
				dataSourceTable.AcceptChanges();
				DataRow[] array = dataSourceTable.Select("DocEntry=" + Function.ParseInt(gv_Report.GetFocusedRowCellValue("DocEntry")));
				array[0]["UserSign2"] = Function.ToString(GlobalData.UserID);
				array[0]["UpdateDate"] = Function.ToString(DateTime.Now);
				if (array.Length != 0)
				{
					_dtDoc = array.CopyToDataTable();
				}
			}
			else
			{
				_dtDoc = grd_Report.DataSourceTable;
				if (_dtDoc.IsNotNull())
				{
					_dtDoc.Rows[gv_Report.FocusedRowHandle]["UserSign"] = GlobalData.UserID;
					_dtDoc.Rows[gv_Report.FocusedRowHandle]["CreateDate"] = DateTime.Now;
					_dtDoc.AcceptChanges();
				}
			}
			_dtLine = grd_LineReport.DataSourceTable().Copy();
			if (_dtLine != null && _dtLine.Rows.Count > 0)
			{
				for (int i = 0; i < _dtLine.Rows.Count; i++)
				{
					if (Function.ToString(_dtLine.Rows[i]["ItemCode"]) == "" && Function.ToString(_dtLine.Rows[i]["TaskCode"]) == "")
					{
						_dtLine.Rows.RemoveAt(i--);
					}
				}
			}
			_dtDoc.TableName = "dtDoc";
			_dtLine.TableName = "dtLine";
			dataSet.Tables.Add(_dtDoc.Copy());
			dataSet.Tables.Add(_dtLine.Copy());
			HttpResult httpResult = ((type != 2) ? (await _httpClient.ReportCompletion.Add(dataSet)) : (await _httpClient.ReportCompletion.Update(dataSet)));
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				_isLoading = true;
				FormMode = Enums.FormViewModeType.View;
				if (type == 2)
				{
					grd_Report.DatasourceTable().AcceptChanges();
					grd_LineReport.DatasourceTable().AcceptChanges();
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
	}

	private async Task<DataTable> LoadData(int id, string projectCode, string startFrom, string startTo, string status)
	{
		try
		{
			_isLoading = true;
			DataTable result = await _httpClient.ReportCompletion.LoadData(id, projectCode.Trim(), startFrom, startTo, status);
			_isLoading = false;
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
			DataTable dataSource = await _httpClient.ReportCompletion.GetDataById(id);
			grd_LineReport.DataSource = dataSource;
			grd_LineReport.DatasourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
				grd_Report.DataSourceTable.Rows.Clear();
				grd_LineReport.DataSourceTable().Rows.Clear();
				_dtDoc = grd_Report.DataSourceTable;
				if (_dtDoc == null || _dtDoc.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				}
				DataRow row = _dtDoc.NewRow();
				DataRow dataRow = row;
				dataRow["DocEntry"] = await SetDataNo();
				row["Status"] = "O";
				row["PostingDate"] = DateTime.Now.Date;
				_dtDoc.Rows.Add(row);
				grd_Report.DataSource = _dtDoc;
				NewLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void NewLine()
	{
		try
		{
			_dtLine = grd_LineReport.DataSourceTable;
			if (_dtLine == null || _dtLine.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				return;
			}
			DataRow dataRow = _dtLine.NewRow();
			dataRow["LineStatus"] = "O";
			_dtLine.Rows.Add(dataRow);
			_dtLine.AcceptChanges();
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DataChanged()
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
			{
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
		try
		{
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> SetDataNo()
	{
		try
		{
			return await _httpClient.ReportCompletion.GetNo();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public void LoadCopyMenu()
	{
		try
		{
			PopCopyFrom = new List<ApzObjectTypes> { ApzObjectTypes.oProjectConfirm };
			DXPopupMenu dXPopupMenu = new DXPopupMenu();
			foreach (ApzObjectTypes item in PopCopyFrom)
			{
				DXMenuItem dXMenuItem = new DXMenuItem();
				if (item == ApzObjectTypes.oProjectConfirm)
				{
					dXMenuItem.Caption = "Project Confirm";
				}
				dXMenuItem.Click += (object s, EventArgs e) =>
				{
					WinCommons._OpenProcessing();
					try
					{
						SetDataConfirm();
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
						WinCommons.ErrorLineNumber(ex2);
					}
					WinCommons._CloseProcessing();
				};
				dXPopupMenu.Items.Add(dXMenuItem);
			}
			drBtn_CopyTo.DropDownControl = dXPopupMenu;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void SetDataConfirm()
	{
		try
		{
			WinCommons._OpenProcessing();
			int projectid = Function.ParseInt(gv_Report.GetFocusedRowCellValue("ProjectId"));
			string docCur = Function.ToString(gv_Report.GetFocusedRowCellValue("DocCur"));
			_ = DateTime.Now.Date;
			DataTable dataTable = grd_LineReport.DatasourceTable().Copy();
			DataTable dtLine = WinFuntions.GetTableStructure("APZ_PJC1");
			if (dataTable.Rows.Count > 0 && dataTable != null)
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DataRow dataRow = dtLine.NewRow();
					dataRow.ItemArray = dataTable.Rows[i].ItemArray;
					dataRow["DocEntry"] = 0;
					dataRow["LineNum"] = 0;
					dataRow["LineStatus"] = "O";
					dataRow["PostingDate"] = DateTime.Now.Date;
					dataRow["Currency"] = Function.ToString(dataTable.Rows[i]["Currency"]);
					dataRow["BaseLine"] = Function.ParseInt(dataTable.Rows[i]["LineNum"]);
					dataRow["BaseEntry"] = Function.ParseInt(dataTable.Rows[i]["DocEntry"]);
					dataRow["Remarks"] = "";
					dtLine.Rows.Add(dataRow);
				}
				dtLine.AcceptChanges();
				double total = dataTable.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["ActualAmount"]));
				DataTable dtDoc = WinFuntions.GetTableStructure("APZ_OPJC");
				DataRow dtRow = dtDoc.NewRow();
				DataRow dataRow2 = dtRow;
				dataRow2["DocEntry"] = await SetDataDocEntryConfirm();
				dtRow["ProjectId"] = projectid;
				dtRow["Status"] = "O";
				dtRow["DocCur"] = docCur;
				dtRow["PostingDate"] = DateTime.Now.Date;
				dtRow["Total"] = total;
				dtDoc.Rows.Add(dtRow);
				dtDoc.AcceptChanges();
				frm_ProjectConfirm frm_ProjectConfirm2 = new frm_ProjectConfirm();
				frm_ProjectConfirm2.MdiParent = MdiParent;
				frm_ProjectConfirm2.CallFromCal = true;
				frm_ProjectConfirm2.DataDocCopy = dtDoc;
				frm_ProjectConfirm2.DataLineCopy = dtLine;
				frm_ProjectConfirm2._projectId = Function.ParseInt(gv_Report.GetFocusedRowCellValue("ProjectId"));
				frm_ProjectConfirm2.StartPosition = FormStartPosition.CenterScreen;
				frm_ProjectConfirm2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.YouMustInputItemCode, "");
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> SetDataDocEntryConfirm()
	{
		try
		{
			return await _httpClient.ProjectConfirm.GetNo();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public async Task ReloadLineReport(DataTable dtCopyTo, int type)
	{
		try
		{
			_dtLine = grd_LineReport.DatasourceTable();
			_dtDoc = grd_Report.DatasourceTable();
			if (type == 2)
			{
				return;
			}
			int i;
			for (i = 0; i < dtCopyTo.Rows.Count; i++)
			{
				DataRow dataRow = (from t in _dtLine.AsEnumerable()
					where Function.ParseInt(t["DocEntry"]) == Function.ParseInt(dtCopyTo.Rows[i]["BaseEntry"]) && Function.ParseInt(t["LineNum"]) == Function.ParseInt(dtCopyTo.Rows[i]["BaseLine"])
					select t).FirstOrDefault();
				if (dataRow != null)
				{
					double num = Function.ParseDouble(dataRow["Quantity"]);
					double num2 = 0.0;
					if (type == 1)
					{
						num2 = num - Function.ParseDouble(dtCopyTo.Rows[i]["Quantity"]);
						dataRow["OpenQty"] = num2;
					}
					if (type == 3)
					{
						dataRow["LineStatus"] = "O";
						num2 = num - (Function.ParseDouble(dtCopyTo.Rows[i]["Quantity"]) + Function.ParseDouble(dataRow["OpenQty"]));
						dataRow["OpenQty"] = num2;
					}
					if (num2 == 0.0)
					{
						dataRow["LineStatus"] = "L";
					}
				}
				_dtLine.AcceptChanges();
			}
			grd_LineReport.DataSource = _dtLine;
			grd_LineReport.RefreshDataSource();
			List<DataRow> list = (from t in _dtLine.AsEnumerable()
				where Function.ToString(t["LineStatus"]) != "L"
				select t).ToList();
			if (list != null && list.Count > 0)
			{
				gv_Report.SetFocusedRowCellValue("Status", "O");
			}
			else
			{
				gv_Report.SetFocusedRowCellValue("Status", "L");
			}
			grd_Report.DatasourceTable().AcceptChanges();
			await Save(2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task<int> FindNextLastFrist(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.ReportCompletion.FindNextLastFrist(Function.ParseInt(txt_Code.EditValue), (int)type);
			if (dataTable.IsNotNull())
			{
				grd_Report.DataSource = dataTable;
				grd_Report.DatasourceTable().AcceptChanges();
				int num = Function.ParseInt(gv_Report.GetFocusedRowCellValue("DocEntry"));
				txt_Code.EditValue = num;
				await LoadLineById(num);
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	public void SetDataCurrency()
	{
		try
		{
			CurrData = GlobalData.CurrencyCodes;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("CurrCode", typeof(string));
			dataTable.Columns.Add("CurrName", typeof(string));
			foreach (CurrencyCodesImpl currDatum in CurrData)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["CurrCode"] = currDatum.CurrCode;
				dataRow["CurrName"] = currDatum.CurrName;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			luk_DocCurr.DataSource = dataTable.Copy();
			luk_DocCurr.NullText = "";
			luk_DocCurr.ValueMember = "CurrCode";
			luk_DocCurr.DisplayMember = "CurrCode";
			luk_Currency.DataSource = dataTable.Copy();
			luk_Currency.NullText = "";
			luk_Currency.ValueMember = "CurrCode";
			luk_Currency.DisplayMember = "CurrCode";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_ReportCompletion_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			_isLoading = true;
			FormMode = Enums.FormViewModeType.View;
			SetDataCurrency();
			LoadCopyMenu();
			LoadInItForm();
			btn_ProjectCode.Properties.ValueMember = "AbsEntry";
			btn_ProjectCode.Properties.DisplayMember = "Code";
			btn_ProjectCode.Properties.TypeCFL = Enums.TypeCFL.ProjectManager;
			btn_ProjectCode.Properties.RefreshDataSource();
			btn_ProjectCode.Properties.TypeDrillDown = ApzObjectTypes.oProjectManager;
			btn_PjCode.ValueMember = "AbsEntry";
			btn_PjCode.DisplayMember = "Code";
			btn_PjCode.TypeCFL = Enums.TypeCFL.ProjectManager;
			btn_PjCode.RefreshDataSource();
			btn_PjCode.TypeDrillDown = ApzObjectTypes.oProjectManager;
			btn_Wbs.ValueMember = "TaskCode";
			btn_Wbs.FieldNameInDataRow("TaskCode", "TaskName");
			btn_Wbs.FieldNameInGrid("TaskCode", "TaskName");
			btn_Wbs.TypeCFL = Enums.TypeCFL.WbsInTabItem;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_Wbs;
			repositoryItemButtonEditCFL.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.projectId = Function.ParseInt(gv_Report.GetFocusedRowCellValue("ProjectId"));
			}));
			btn_Wbs.RefreshDataSource();
			btn_Wbs.TypeDrillDown = ApzObjectTypes.oProjectManager;
			btn_Item.ValueMember = "ItemCode";
			btn_Item.FieldNameInDataRow("ItemCode", "ItemName");
			btn_Item.FieldNameInGrid("ItemCode", "Dscription");
			btn_Item.TypeCFL = Enums.TypeCFL.ItemInTabItem;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = btn_Item;
			repositoryItemButtonEditCFL2.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL2.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.projectId = Function.ParseInt(gv_Report.GetFocusedRowCellValue("ProjectId"));
				frm.wbsCode = Function.ToString(gv_LineReport.GetFocusedRowCellValue("TaskCode"));
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = btn_Item;
			repositoryItemButtonEditCFL3.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL3.HandleChooseItem, new Action<DataRow>(ItemCode_HandleChooseItem));
			btn_Item.TypeDrillDown = ApzObjectTypes.oItems;
			btn_Item.RefreshDataSource();
			dat_FromDate.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			dat_ToDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			luk_StatusGrd.SetDataSource(new Dictionary<string, string>
			{
				{ "P", "Planned" },
				{ "O", "Open" },
				{ "C", "Canceled" },
				{ "L", "Closed" }
			});
			luk_Status.SetDataSource(new Dictionary<string, string>
			{
				{ "", "" },
				{ "P", "Planned" },
				{ "O", "Open" },
				{ "C", "Canceled" },
				{ "L", "Closed" }
			});
			luk_StatusLine.SetDataSource(new Dictionary<string, string>
			{
				{ "O", "Open" },
				{ "C", "Canceled" },
				{ "L", "Closed" }
			});
			await Task.WhenAll(WinFuntions.LoadUdfColumns(gv_Report, "APZ_ORTC"), WinFuntions.LoadUdfColumns(gv_LineReport, "APZ_RTC1"));
			if (_isCpyWbs)
			{
				FormMode = Enums.FormViewModeType.Add;
				return;
			}
			new DataTable();
			string startFrom = DateTime.Today.AddDays(-Configuration.SearchDate).ToString("ddMMyyyy");
			string startTo = DateTime.Today.AddDays(Configuration.SearchDate).ToString("ddMMyyyy");
			string projectCode = Function.ToString(btn_ProjectCode.EditValue);
			string status = Function.ToString(luk_Status.EditValue);
			if (PjReportId > 0)
			{
				txt_Code.EditValue = PjReportId;
			}
			DataTable dataTable = await LoadData(Function.ParseInt(txt_Code.EditValue), projectCode, startFrom, startTo, status);
			PjReportId = 0;
			grd_Report.DataSource = dataTable;
			grd_Report.DatasourceTable().AcceptChanges();
			if (!dataTable.IsNotNull())
			{
				gv_Report_FocusedRowChanged(null, null);
			}
			gv_Report.FocusedRowHandle = 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		_isLoading = false;
	}

	private void LoadInItForm()
	{
		try
		{
			List<Common.Table_Code> dataSource = _lstValidP.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			_typeP.DataSource = dataSource;
			_typeP.DisplayMember = "Name";
			_typeP.ValueMember = "Code";
			_typeP.NullText = string.Empty;
			List<Common.Table_Code> dataSource2 = _lstValidC.Select((KeyValuePair<string, string> item) => new Common.Table_Code
			{
				Code = item.Key,
				Name = item.Value
			}).ToList();
			_typeC.DataSource = dataSource2;
			_typeC.DisplayMember = "Name";
			_typeC.ValueMember = "Code";
			_typeC.NullText = string.Empty;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ItemCode_HandleChooseItem(DataRow dr)
	{
		try
		{
			if (dr != null)
			{
				gv_LineReport.SetFocusedRowCellValue("ItemCode", Function.ToString(dr["ItemCode"]));
				gv_LineReport.SetFocusedRowCellValue("ItemName", Function.ToString(dr["ItemName"]));
				gv_LineReport.SetFocusedRowCellValue("UoM", Function.ToString(dr["InvntryUom"]));
				gv_LineReport.SetFocusedRowCellValue("ItmsGrpCod", Function.ParseInt(dr["ItmsGrpCod"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string projectCode = Function.ToString(btn_ProjectCode.EditValue);
			string status = Function.ToString(luk_Status.EditValue);
			string startFrom = (Function.ParseDateTime(dat_FromDate.EditValue).HasValue ? ((DateTime)dat_FromDate.EditValue).ToString("yyyyMMdd") : "");
			string startTo = (Function.ParseDateTime(dat_ToDate.EditValue).HasValue ? ((DateTime)dat_ToDate.EditValue).ToString("yyyyMMdd") : "");
			if (PjReportId > 0)
			{
				txt_Code.EditValue = PjReportId;
			}
			DataTable dataSource = await LoadData(Function.ParseInt(txt_Code.EditValue), projectCode, startFrom, startTo, status);
			PjReportId = 0;
			if (!_isLoading && ((FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes))
			{
				FormMode = Enums.FormViewModeType.View;
				grd_LineReport.DataSourceTable.Rows.Clear();
				grd_Report.DataSource = dataSource;
				grd_Report.DatasourceTable().AcceptChanges();
				int id = Function.ParseInt(gv_Report.GetFocusedRowCellValue("DocEntry"));
				await LoadLineById(id);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Report_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && !_isCpyWbs)
			{
				int id = Function.ParseInt(gv_Report.GetFocusedRowCellValue("DocEntry"));
				await LoadLineById(id);
				switch (Function.ToString(gv_Report.GetFocusedRowCellValue("Status")))
				{
				case "P":
					drBtn_CopyTo.Enabled = false;
					break;
				case "O":
					drBtn_CopyTo.Enabled = true;
					break;
				case "C":
					drBtn_CopyTo.Enabled = false;
					break;
				case "L":
					drBtn_CopyTo.Enabled = false;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private void gv_Report_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.Update)
			{
				DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
				if (dialogResult == DialogResult.Yes && dialogResult == DialogResult.Yes)
				{
					grd_Report.DataSourceTable.RejectChanges();
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
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_Report.GetSelectedRows() == null && gv_Report.GetSelectedRows().Count() == 0)
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
				return;
			}
			string text = Function.ToString(gv_Report.GetFocusedRowCellValue("DocEntry"));
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRemoveByDocEntry, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, text) == DialogResult.Yes)
			{
				string id = Function.ToString(gv_Report.GetFocusedRowCellValue("DocEntry"));
				string text2 = await _httpClient.ReportCompletion.DeleteDocs(id);
				if (string.IsNullOrEmpty(text2))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					btn_Search_Click(null, null);
				}
				WinCommons.Error(Enums.MessageCode.None, text2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		NewLine();
		DataChanged();
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gv_LineReport.DataRowCount > 0)
			{
				grd_LineReport.BeginUpdate();
				gv_LineReport.DeleteSelectedRows();
				grd_LineReport.EndUpdate();
				grd_LineReport.DatasourceTable().AcceptChanges();
				double num = _dtLine.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["Total"]));
				_dtDoc.Rows[gv_Report.GetFocusedDataSourceRowIndex()]["Total"] = num;
				DataChanged();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LineReport_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtDoc = grd_Report.DataSourceTable;
			_dtLine = grd_LineReport.DataSourceTable();
			string a = Function.ToString(gv_Report.GetRowCellValue(e.RowHandle, "Status"));
			if (e.Column.FieldName == "Quantity")
			{
				double num = Function.ParseDouble(e.Value);
				double num2 = Function.ParseDouble(gv_LineReport.GetRowCellValue(e.RowHandle, "RefPrice"));
				double num3 = Math.Round(num * num2, 6);
				_dtLine.Rows[gv_LineReport.GetDataSourceRowIndex(e.RowHandle)]["Total"] = num3;
				double num4 = _dtLine.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["Total"]));
				_dtDoc.Rows[gv_Report.GetFocusedDataSourceRowIndex()]["Total"] = num4;
			}
			if (e.Column.FieldName == "RefPrice")
			{
				double num5 = Function.ParseDouble(e.Value);
				if (string.Equals(a, "P"))
				{
					double num6 = Math.Round(Function.ParseDouble(gv_LineReport.GetRowCellValue(e.RowHandle, "Quantity")) * num5, 6);
					gv_LineReport.SetRowCellValue(e.RowHandle, "Total", num6);
					double num7 = _dtLine.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["Total"]));
					_dtDoc.Rows[gv_Report.GetFocusedDataSourceRowIndex()]["Total"] = num7;
				}
				if (string.Equals(a, "O"))
				{
					double num8 = Math.Round(Function.ParseDouble(gv_LineReport.GetRowCellValue(e.RowHandle, "ActualQty")) * num5, 6);
					gv_LineReport.SetRowCellValue(e.RowHandle, "ActualAmount", num8);
					double num9 = _dtLine.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["ActualAmount"]));
					_dtDoc.Rows[gv_Report.GetFocusedDataSourceRowIndex()]["TotalActual"] = num9;
				}
			}
			if (e.Column.FieldName == "ActualQty")
			{
				double num10 = Function.ParseDouble(e.Value);
				double num11 = Function.ParseDouble(gv_LineReport.GetRowCellValue(e.RowHandle, "RefPrice"));
				double num12 = Math.Round(num10 * num11, 6);
				_dtLine.Rows[gv_LineReport.GetDataSourceRowIndex(e.RowHandle)]["ActualAmount"] = num12;
				double num13 = _dtLine.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["ActualAmount"]));
				_dtDoc.Rows[gv_Report.GetFocusedDataSourceRowIndex()]["TotalActual"] = num13;
			}
			if (e.Column.FieldName == "Total")
			{
				double num14 = Function.ParseDouble(gv_LineReport.GetRowCellValue(e.RowHandle, "Quantity"));
				double num15 = Function.ParseDouble(e.Value);
				if (num14 > 0.0)
				{
					if (_dtLine != null && _dtLine.Rows.Count > 0)
					{
						_dtLine.Rows[gv_LineReport.GetDataSourceRowIndex(e.RowHandle)]["RefPrice"] = Math.Round(num15 / num14);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.InQuantityColumnEnterWholeNumberGreaterThan0QuantityMandatory, "");
				}
				double num16 = _dtLine.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["Total"]));
				_dtDoc.Rows[gv_Report.GetFocusedDataSourceRowIndex()]["Total"] = num16;
			}
			if (e.Column.FieldName == "ActualAmount")
			{
				double num17 = Function.ParseDouble(gv_LineReport.GetRowCellValue(e.RowHandle, "ActualQty"));
				double num18 = Function.ParseDouble(e.Value);
				if (num17 > 0.0)
				{
					if (_dtLine != null && _dtLine.Rows.Count > 0)
					{
						_dtLine.Rows[gv_LineReport.GetDataSourceRowIndex(e.RowHandle)]["RefPrice"] = Math.Round(num18 / num17);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.InQuantityColumnEnterWholeNumberGreaterThan0QuantityMandatory, "");
				}
				double num19 = _dtLine.AsEnumerable().Sum((DataRow t) => Function.ParseDouble(t["ActualAmount"]));
				_dtDoc.Rows[gv_Report.GetFocusedDataSourceRowIndex()]["TotalActual"] = num19;
			}
			DataChanged();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Report_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		DataChanged();
	}

	private async void canceledToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Report.GetFocusedRowCellValue("DocEntry"));
			string text = "C";
			if (num > 0 && !string.IsNullOrEmpty(text) && string.IsNullOrEmpty((await _httpClient.ReportCompletion.UpdateStatus(num, text)).Message))
			{
				btn_Search_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void closeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int num = Function.ParseInt(gv_Report.GetFocusedRowCellValue("DocEntry"));
			string text = "L";
			if (num > 0 && !string.IsNullOrEmpty(text) && string.IsNullOrEmpty((await _httpClient.ReportCompletion.UpdateStatus(num, text)).Message))
			{
				btn_Search_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Report_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			_ = gv_Report.FocusedRowHandle;
			string a = Function.ToString(gv_Report.GetFocusedRowCellValue("Status"));
			if ((gv_Report.FocusedColumn.FieldName == "ProjectId" || gv_Report.FocusedColumn.FieldName == "PostingDate" || gv_Report.FocusedColumn.FieldName == "Total" || gv_Report.FocusedColumn.FieldName == "DueDate" || gv_Report.FocusedColumn.FieldName == "DocumentDate" || gv_Report.FocusedColumn.FieldName == "Status" || gv_Report.FocusedColumn.FieldName == "TotalActual") && (string.Equals(a, "C") || string.Equals(a, "L")))
			{
				gv_Report.FocusedColumn.OptionsColumn.ReadOnly = true;
				gv_Report.Columns["Remark"].OptionsColumn.ReadOnly = false;
			}
			else
			{
				gv_Report.FocusedColumn.OptionsColumn.ReadOnly = false;
				gv_Report.Columns["DocEntry"].OptionsColumn.ReadOnly = true;
				gv_Report.Columns["Total"].OptionsColumn.ReadOnly = true;
			}
			if (gv_Report.FocusedColumn.FieldName == "TotalActual" || gv_Report.FocusedColumn.FieldName == "Total")
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Report_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			_dtLine = grd_LineReport.DatasourceTable();
			if (e.Column.FieldName == "PostingDate")
			{
				DateTime dateTime = Function.ParseDateTimes(e.Value);
				if (_dtLine != null && _dtLine.Rows.Count > 0)
				{
					foreach (DataRow row in _dtLine.Rows)
					{
						row["PostingDate"] = dateTime;
					}
				}
			}
			if (e.Column.FieldName == "DueDate")
			{
				DateTime dateTime2 = Function.ParseDateTimes(e.Value);
				if (_dtLine != null && _dtLine.Rows.Count > 0)
				{
					foreach (DataRow row2 in _dtLine.Rows)
					{
						if (string.IsNullOrEmpty(Function.ToString(row2["DueDate"])))
						{
							row2["DueDate"] = dateTime2;
						}
					}
				}
			}
			if (!(e.Column.FieldName == "DocumentDate"))
			{
				return;
			}
			DateTime dateTime3 = Function.ParseDateTimes(e.Value);
			if (_dtLine == null || _dtLine.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataRow row3 in _dtLine.Rows)
			{
				if (string.IsNullOrEmpty(Function.ToString(row3["DocumentDate"])))
				{
					row3["DocumentDate"] = dateTime3;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Report_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			_ = gv_Report.FocusedRowHandle;
			string a = Function.ToString(gv_Report.GetRowCellValue(e.RowHandle, "Status"));
			if ((e.Column.FieldName == "ProjectId" || e.Column.FieldName == "PostingDate" || e.Column.FieldName == "DocumentDate" || e.Column.FieldName == "Total" || e.Column.FieldName == "DueDate" || e.Column.FieldName == "Status") && (string.Equals(a, "C") || string.Equals(a, "L")))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (e.Column.FieldName == "Total" || e.Column.FieldName == "TotalActual" || e.Column.FieldName == "DocEntry")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LineReport_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			_ = gv_Report.FocusedRowHandle;
			string a = Function.ToString(gv_Report.GetFocusedRowCellValue("Status"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Cancel = true;
			}
			if (string.Equals(a, "P") && (gv_LineReport.FocusedColumn.FieldName == "ActualQty" || gv_LineReport.FocusedColumn.FieldName == "ActualAmount"))
			{
				e.Cancel = true;
			}
			if (string.Equals(a, "O") && (gv_LineReport.FocusedColumn.FieldName == "Total" || gv_LineReport.FocusedColumn.FieldName == "Quantity"))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Report_CustomRowCellEditForEditing(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "Status")
			{
				return;
			}
			string text = Function.ToString(gv_Report.GetRowCellValue(e.RowHandle, "Status"));
			if (!string.IsNullOrWhiteSpace(text))
			{
				switch (text)
				{
				case "P":
					e.RepositoryItem = _typeP;
					break;
				case "O":
					e.RepositoryItem = _typeP;
					break;
				case "C":
					e.RepositoryItem = _typeC;
					break;
				case "L":
					e.RepositoryItem = _typeC;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void viewDiagramToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			frm_Diagram frm_Diagram2 = new frm_Diagram();
			frm_Diagram2.ObjectType = ApzObjectTypes.oReportCompletion;
			frm_Diagram2.ObjectId = Function.ToString(gv_Report.GetFocusedRowCellValue("DocEntry"));
			frm_Diagram2.MdiParent = MdiParent;
			frm_Diagram2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LineReport_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			string a = Function.ToString(gv_Report.GetFocusedRowCellValue("Status"));
			string a2 = Function.ToString(gv_LineReport.GetRowCellValue(e.RowHandle, "LineStatus"));
			if (string.Equals(a, "C") || string.Equals(a, "L"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (string.Equals(a, "O") && !string.Equals(a2, "O") && (e.Column.FieldName == "TaskCode" || e.Column.FieldName == "TaskName" || e.Column.FieldName == "ItemCode" || e.Column.FieldName == "ItemName" || e.Column.FieldName == "Quantity" || e.Column.FieldName == "RefPrice" || e.Column.FieldName == "Total" || e.Column.FieldName == "UoM" || e.Column.FieldName == "LineStatus"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (string.Equals(a, "P") && (e.Column.FieldName == "ActualQty" || e.Column.FieldName == "ActualAmount"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (string.Equals(a, "O") && (e.Column.FieldName == "Quantity" || e.Column.FieldName == "Total"))
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
			}
			if (e.Column.FieldName == "OpenQty")
			{
				e.Appearance.BackColor = GridColor.GridReadOnly;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.PM.frm_ProjectReportCompletion));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemTextEditExPrice1 = new Apzon.Controls.Editor.TextEditExPrice.RepositoryItemTextEditExPrice();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx5 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx6 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx7 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Report = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.canceledToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.viewDiagramToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Report = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_PjCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.dat_Posting = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StatusGrd = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx26 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx2 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumnEx30 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_DocCurr = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.grd_LineReport = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_LineReport = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Wbs = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Item = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx17 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.gridColumnEx18 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_StatusLine = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gridColumnEx19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx24 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx29 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx31 = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_Currency = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btn_Ok = new DevExpress.XtraEditors.SimpleButton();
		this.btn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ProjectCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.dat_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.dat_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.luk_Status = new Apzon.Controls.Editor.MyLookUpEdit.LookUpEditSimple();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.drBtn_CopyTo = new DevExpress.XtraEditors.DropDownButton();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Code = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Report).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Report).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_PjCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dat_Posting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dat_Posting.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusGrd).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DocCurr).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_LineReport).BeginInit();
		this.ctm_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_LineReport).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Wbs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Item).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ProjectCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dat_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dat_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dat_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dat_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.repositoryItemCalcEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx2.IsSearchControl = false;
		this.repositoryItemCalcEditEx2.Name = "repositoryItemCalcEditEx2";
		this.repositoryItemCalcEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx3.IsSearchControl = false;
		this.repositoryItemCalcEditEx3.Name = "repositoryItemCalcEditEx3";
		this.repositoryItemTextEditExPrice1.Name = "repositoryItemTextEditExPrice1";
		this.repositoryItemCalcEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx4.IsSearchControl = false;
		this.repositoryItemCalcEditEx4.Name = "repositoryItemCalcEditEx4";
		this.repositoryItemCalcEditEx5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx5.IsSearchControl = false;
		this.repositoryItemCalcEditEx5.Name = "repositoryItemCalcEditEx5";
		this.repositoryItemCalcEditEx6.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx6.IsSearchControl = false;
		this.repositoryItemCalcEditEx6.Name = "repositoryItemCalcEditEx6";
		this.repositoryItemCalcEditEx7.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx7.IsSearchControl = false;
		this.repositoryItemCalcEditEx7.Name = "repositoryItemCalcEditEx7";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 71);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Report);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_LineReport);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(832, 390);
		this.splitContainerControl1.SplitterPosition = 187;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_Report.AllowPasteExcel = true;
		this.grd_Report.ClipboardData = null;
		this.grd_Report.ContextMenuStrip = this.ctm_Doc;
		this.grd_Report.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Report.Location = new System.Drawing.Point(0, 0);
		this.grd_Report.MainView = this.gv_Report;
		this.grd_Report.Name = "grd_Report";
		this.grd_Report.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[7] { this.dat_Posting, this.luk_StatusGrd, this.btn_PjCode, this.repositoryItemDateEditEx1, this.repositoryItemDateEditEx2, this.repositoryItemMemoExEdit1, this.luk_DocCurr });
		this.grd_Report.Size = new System.Drawing.Size(832, 187);
		this.grd_Report.TabIndex = 0;
		this.grd_Report.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Report });
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.addNewToolStripMenuItem, this.canceledToolStripMenuItem, this.closeToolStripMenuItem, this.viewDiagramToolStripMenuItem });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(148, 92);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.canceledToolStripMenuItem.Name = "canceledToolStripMenuItem";
		this.canceledToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
		this.canceledToolStripMenuItem.Text = "Cancel";
		this.canceledToolStripMenuItem.Click += new System.EventHandler(this.canceledToolStripMenuItem_Click);
		this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
		this.closeToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
		this.closeToolStripMenuItem.Text = "Close";
		this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
		this.viewDiagramToolStripMenuItem.Name = "viewDiagramToolStripMenuItem";
		this.viewDiagramToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
		this.viewDiagramToolStripMenuItem.Text = "View Diagram";
		this.viewDiagramToolStripMenuItem.Click += new System.EventHandler(this.viewDiagramToolStripMenuItem_Click);
		this.gv_Report.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Report.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Report.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Report.ColumnKey = null;
		this.gv_Report.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gridColumnEx1, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx2, this.gridColumnEx22, this.gridColumnEx25, this.gridColumnEx26, this.gridColumnEx30 });
		this.gv_Report.DisableFormSetting = false;
		this.gv_Report.DisplayException = false;
		this.gv_Report.EditColumn = null;
		this.gv_Report.FieldCurrency = null;
		this.gv_Report.FieldRate = null;
		this.gv_Report.FormId = "oReportCompletion";
		this.gv_Report.GridControl = this.grd_Report;
		this.gv_Report.IgnoreFilter = false;
		this.gv_Report.IndexEditRow = -1;
		this.gv_Report.IndicatorWidth = 35;
		this.gv_Report.IsApzTable = false;
		this.gv_Report.IsInsertNewRow = false;
		this.gv_Report.IsLoadingData = false;
		this.gv_Report.IsMappingContextAction = false;
		this.gv_Report.ItemId = "APZ_ORTC";
		this.gv_Report.LstColumnAllowBatch = null;
		this.gv_Report.LstColumnNotAllowBatch = null;
		this.gv_Report.LstColumnNotEditable = null;
		this.gv_Report.LstColumNotSetting = null;
		this.gv_Report.Name = "gv_Report";
		this.gv_Report.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Report.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Report.OptionsView.ColumnAutoWidth = false;
		this.gv_Report.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Report.OptionsView.ShowGroupPanel = false;
		this.gv_Report.UseBatchMod = false;
		this.gv_Report.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Report_RowCellStyle);
		this.gv_Report.CustomRowCellEditForEditing += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Report_CustomRowCellEditForEditing);
		this.gv_Report.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Report_ShowingEditor);
		this.gv_Report.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Report_FocusedRowChanged);
		this.gv_Report.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Report_CellValueChanged);
		this.gv_Report.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Report_CellValueChanging);
		this.gv_Report.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Report_BeforeLeaveRow);
		this.gridColumnEx1.Caption = "Doc. No.";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "DocEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = true;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 0;
		this.gridColumnEx3.Caption = "Project Code";
		this.gridColumnEx3.ColumnEdit = this.btn_PjCode;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "ProjectId";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.btn_PjCode.AutoHeight = false;
		this.btn_PjCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.btn_PjCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_PjCode.ContainerControl = this;
		this.btn_PjCode.DisplayMember = "Code";
		this.btn_PjCode.DisplayMember2 = "";
		this.btn_PjCode.LoadDataSourceWhenEnter = false;
		this.btn_PjCode.Name = "btn_PjCode";
		this.btn_PjCode.SeparateCharacterForMulti = ';';
		this.btn_PjCode.SeparateString = ", ";
		this.btn_PjCode.UsingAutoCompleteBox = false;
		this.btn_PjCode.UsingDataSource = true;
		this.btn_PjCode.ValidateWhenNoDataSource = false;
		this.btn_PjCode.ValueMember = "AbsEntry";
		this.gridColumnEx4.Caption = "Posting Date";
		this.gridColumnEx4.ColumnEdit = this.dat_Posting;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx4.FieldName = "PostingDate";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 3;
		this.dat_Posting.AutoHeight = false;
		this.dat_Posting.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dat_Posting.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dat_Posting.DisplayFormat.FormatString = "";
		this.dat_Posting.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.dat_Posting.EditFormat.FormatString = "";
		this.dat_Posting.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.dat_Posting.IsSearchControl = false;
		this.dat_Posting.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.dat_Posting.Name = "dat_Posting";
		this.gridColumnEx5.Caption = "Plan Total";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gridColumnEx5.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = "DocCur";
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx5.FieldName = "Total";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gridColumnEx6.Caption = "Comment";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Comment";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 8;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.gridColumnEx2.Caption = "Status";
		this.gridColumnEx2.ColumnEdit = this.luk_StatusGrd;
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Status";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 2;
		this.luk_StatusGrd.AutoHeight = false;
		this.luk_StatusGrd.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusGrd.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusGrd.IsSearchControl = false;
		this.luk_StatusGrd.Name = "luk_StatusGrd";
		this.luk_StatusGrd.NullText = "";
		this.luk_StatusGrd.ShowFooter = false;
		this.luk_StatusGrd.ShowHeader = false;
		this.gridColumnEx22.Caption = "Due Date";
		this.gridColumnEx22.ColumnEdit = this.repositoryItemDateEditEx1;
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx22.FieldName = "DueDate";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Visible = true;
		this.gridColumnEx22.VisibleIndex = 5;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.DisplayFormat.FormatString = "";
		this.repositoryItemDateEditEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.EditFormat.FormatString = "";
		this.repositoryItemDateEditEx1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.gridColumnEx25.Caption = "Actual Total";
		this.gridColumnEx25.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gridColumnEx25.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx25.CurrencyDateField = null;
		this.gridColumnEx25.CurrencyField = "DocCur";
		this.gridColumnEx25.CurrencyFixed = null;
		this.gridColumnEx25.CurrencyFixedRate = 0.0;
		this.gridColumnEx25.CurrencyRateField = null;
		this.gridColumnEx25.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx25.FieldName = "TotalActual";
		this.gridColumnEx25.Name = "gridColumnEx25";
		this.gridColumnEx25.OptionsColumn.ReadOnly = true;
		this.gridColumnEx25.PrimaryKey = false;
		this.gridColumnEx25.UnitField = null;
		this.gridColumnEx25.Visible = true;
		this.gridColumnEx25.VisibleIndex = 7;
		this.gridColumnEx26.Caption = "Document Date";
		this.gridColumnEx26.ColumnEdit = this.repositoryItemDateEditEx2;
		this.gridColumnEx26.CurrencyDateField = null;
		this.gridColumnEx26.CurrencyField = null;
		this.gridColumnEx26.CurrencyFixed = null;
		this.gridColumnEx26.CurrencyFixedRate = 0.0;
		this.gridColumnEx26.CurrencyRateField = null;
		this.gridColumnEx26.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx26.FieldName = "DocumentDate";
		this.gridColumnEx26.Name = "gridColumnEx26";
		this.gridColumnEx26.PrimaryKey = false;
		this.gridColumnEx26.UnitField = null;
		this.gridColumnEx26.Visible = true;
		this.gridColumnEx26.VisibleIndex = 6;
		this.gridColumnEx26.Width = 93;
		this.repositoryItemDateEditEx2.AutoHeight = false;
		this.repositoryItemDateEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx2.DisplayFormat.FormatString = "";
		this.repositoryItemDateEditEx2.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx2.EditFormat.FormatString = "";
		this.repositoryItemDateEditEx2.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx2.IsSearchControl = false;
		this.repositoryItemDateEditEx2.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx2.Name = "repositoryItemDateEditEx2";
		this.gridColumnEx30.Caption = "DocCur";
		this.gridColumnEx30.ColumnEdit = this.luk_DocCurr;
		this.gridColumnEx30.CurrencyDateField = null;
		this.gridColumnEx30.CurrencyField = null;
		this.gridColumnEx30.CurrencyFixed = null;
		this.gridColumnEx30.CurrencyFixedRate = 0.0;
		this.gridColumnEx30.CurrencyRateField = null;
		this.gridColumnEx30.FieldName = "DocCur";
		this.gridColumnEx30.Name = "gridColumnEx30";
		this.gridColumnEx30.PrimaryKey = false;
		this.gridColumnEx30.UnitField = null;
		this.gridColumnEx30.Visible = true;
		this.gridColumnEx30.VisibleIndex = 9;
		this.luk_DocCurr.AutoHeight = false;
		this.luk_DocCurr.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_DocCurr.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Code")
		});
		this.luk_DocCurr.DisplayMember = "CurrCode";
		this.luk_DocCurr.Name = "luk_DocCurr";
		this.luk_DocCurr.NullText = "";
		this.luk_DocCurr.ValueMember = "CurrCode";
		this.grd_LineReport.AllowPasteExcel = true;
		this.grd_LineReport.ClipboardData = null;
		this.grd_LineReport.ContextMenuStrip = this.ctm_Line;
		this.grd_LineReport.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_LineReport.Location = new System.Drawing.Point(0, 0);
		this.grd_LineReport.MainView = this.gv_LineReport;
		this.grd_LineReport.Name = "grd_LineReport";
		this.grd_LineReport.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.luk_StatusLine, this.btn_Wbs, this.btn_Item, this.repositoryItemMemoExEdit2, this.luk_Currency });
		this.grd_LineReport.Size = new System.Drawing.Size(832, 193);
		this.grd_LineReport.TabIndex = 0;
		this.grd_LineReport.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_LineReport });
		this.ctm_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.addRowToolStripMenuItem });
		this.ctm_Line.Name = "ctm_Line";
		this.ctm_Line.Size = new System.Drawing.Size(123, 26);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(122, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.gv_LineReport.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_LineReport.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_LineReport.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_LineReport.ColumnKey = null;
		this.gv_LineReport.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[21]
		{
			this.gridColumnEx7, this.gridColumnEx8, this.gridColumnEx9, this.gridColumnEx10, this.gridColumnEx11, this.gridColumnEx12, this.gridColumnEx13, this.gridColumnEx14, this.gridColumnEx15, this.gridColumnEx16,
			this.gridColumnEx17, this.gridColumnEx18, this.gridColumnEx19, this.gridColumnEx20, this.gridColumnEx21, this.gridColumnEx23, this.gridColumnEx24, this.gridColumnEx27, this.gridColumnEx28, this.gridColumnEx29,
			this.gridColumnEx31
		});
		this.gv_LineReport.DisableFormSetting = false;
		this.gv_LineReport.DisplayException = false;
		this.gv_LineReport.EditColumn = null;
		this.gv_LineReport.FieldCurrency = null;
		this.gv_LineReport.FieldRate = null;
		this.gv_LineReport.FormId = "oReportCompletion";
		this.gv_LineReport.GridControl = this.grd_LineReport;
		this.gv_LineReport.IgnoreFilter = false;
		this.gv_LineReport.IndexEditRow = -1;
		this.gv_LineReport.IndicatorWidth = 35;
		this.gv_LineReport.IsApzTable = false;
		this.gv_LineReport.IsInsertNewRow = false;
		this.gv_LineReport.IsLoadingData = false;
		this.gv_LineReport.IsMappingContextAction = false;
		this.gv_LineReport.ItemId = "APZ_RTC1";
		this.gv_LineReport.LstColumnAllowBatch = null;
		this.gv_LineReport.LstColumnNotAllowBatch = null;
		this.gv_LineReport.LstColumnNotEditable = null;
		this.gv_LineReport.LstColumNotSetting = null;
		this.gv_LineReport.Name = "gv_LineReport";
		this.gv_LineReport.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_LineReport.OptionsView.BestFitMaxRowCount = 100;
		this.gv_LineReport.OptionsView.ColumnAutoWidth = false;
		this.gv_LineReport.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_LineReport.OptionsView.ShowGroupPanel = false;
		this.gv_LineReport.UseBatchMod = false;
		this.gv_LineReport.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_LineReport_RowCellStyle);
		this.gv_LineReport.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_LineReport_ShowingEditor);
		this.gv_LineReport.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_LineReport_CellValueChanged);
		this.gridColumnEx7.Caption = "DocEntry";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "DocEntry";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.PrimaryKey = true;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx8.Caption = "LineNum";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "LineNum";
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.OptionsColumn.ReadOnly = true;
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx9.Caption = "Wbs Code";
		this.gridColumnEx9.ColumnEdit = this.btn_Wbs;
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "TaskCode";
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 0;
		this.btn_Wbs.AutoHeight = false;
		this.btn_Wbs.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.btn_Wbs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Wbs.ContainerControl = this;
		this.btn_Wbs.DisplayMember = "TaskCode";
		this.btn_Wbs.DisplayMember2 = "";
		this.btn_Wbs.LoadDataSourceWhenEnter = false;
		this.btn_Wbs.Name = "btn_Wbs";
		this.btn_Wbs.SeparateCharacterForMulti = ';';
		this.btn_Wbs.SeparateString = ", ";
		this.btn_Wbs.UsingAutoCompleteBox = false;
		this.btn_Wbs.UsingDataSource = true;
		this.btn_Wbs.ValidateWhenNoDataSource = false;
		this.btn_Wbs.ValueMember = "TaskCode";
		this.gridColumnEx10.Caption = "Wbs Name";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "TaskName";
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 1;
		this.gridColumnEx11.Caption = "Item Code";
		this.gridColumnEx11.ColumnEdit = this.btn_Item;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "ItemCode";
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 2;
		this.btn_Item.AutoHeight = false;
		this.btn_Item.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.btn_Item.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_Item.ContainerControl = this;
		this.btn_Item.DisplayMember = "ItemCode";
		this.btn_Item.DisplayMember2 = "";
		this.btn_Item.LoadDataSourceWhenEnter = false;
		this.btn_Item.Name = "btn_Item";
		this.btn_Item.SeparateCharacterForMulti = ';';
		this.btn_Item.SeparateString = ", ";
		this.btn_Item.UsingAutoCompleteBox = false;
		this.btn_Item.UsingDataSource = true;
		this.btn_Item.ValidateWhenNoDataSource = false;
		this.btn_Item.ValueMember = "ItemCode";
		this.gridColumnEx12.Caption = "Item Name";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "ItemName";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gridColumnEx12.Visible = true;
		this.gridColumnEx12.VisibleIndex = 3;
		this.gridColumnEx13.Caption = "Plan Quantity";
		this.gridColumnEx13.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gridColumnEx13.CurrencyDateField = null;
		this.gridColumnEx13.CurrencyField = null;
		this.gridColumnEx13.CurrencyFixed = null;
		this.gridColumnEx13.CurrencyFixedRate = 0.0;
		this.gridColumnEx13.CurrencyRateField = null;
		this.gridColumnEx13.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx13.FieldName = "Quantity";
		this.gridColumnEx13.Name = "gridColumnEx13";
		this.gridColumnEx13.PrimaryKey = false;
		this.gridColumnEx13.UnitField = null;
		this.gridColumnEx13.Visible = true;
		this.gridColumnEx13.VisibleIndex = 4;
		this.gridColumnEx14.Caption = "Ref.Price";
		this.gridColumnEx14.ColumnEdit = this.repositoryItemTextEditExPrice1;
		this.gridColumnEx14.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx14.CurrencyDateField = null;
		this.gridColumnEx14.CurrencyField = "Currency";
		this.gridColumnEx14.CurrencyFixed = null;
		this.gridColumnEx14.CurrencyFixedRate = 0.0;
		this.gridColumnEx14.CurrencyRateField = null;
		this.gridColumnEx14.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gridColumnEx14.FieldName = "RefPrice";
		this.gridColumnEx14.Name = "gridColumnEx14";
		this.gridColumnEx14.PrimaryKey = false;
		this.gridColumnEx14.UnitField = null;
		this.gridColumnEx14.Visible = true;
		this.gridColumnEx14.VisibleIndex = 5;
		this.gridColumnEx15.Caption = "Total";
		this.gridColumnEx15.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gridColumnEx15.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx15.CurrencyDateField = null;
		this.gridColumnEx15.CurrencyField = "Currency";
		this.gridColumnEx15.CurrencyFixed = null;
		this.gridColumnEx15.CurrencyFixedRate = 0.0;
		this.gridColumnEx15.CurrencyRateField = null;
		this.gridColumnEx15.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx15.FieldName = "Total";
		this.gridColumnEx15.Name = "gridColumnEx15";
		this.gridColumnEx15.PrimaryKey = false;
		this.gridColumnEx15.UnitField = null;
		this.gridColumnEx15.Visible = true;
		this.gridColumnEx15.VisibleIndex = 6;
		this.gridColumnEx16.Caption = "UoM";
		this.gridColumnEx16.CurrencyDateField = null;
		this.gridColumnEx16.CurrencyField = null;
		this.gridColumnEx16.CurrencyFixed = null;
		this.gridColumnEx16.CurrencyFixedRate = 0.0;
		this.gridColumnEx16.CurrencyRateField = null;
		this.gridColumnEx16.FieldName = "UoM";
		this.gridColumnEx16.Name = "gridColumnEx16";
		this.gridColumnEx16.PrimaryKey = false;
		this.gridColumnEx16.UnitField = null;
		this.gridColumnEx16.Visible = true;
		this.gridColumnEx16.VisibleIndex = 7;
		this.gridColumnEx17.Caption = "Remarks";
		this.gridColumnEx17.ColumnEdit = this.repositoryItemMemoExEdit2;
		this.gridColumnEx17.CurrencyDateField = null;
		this.gridColumnEx17.CurrencyField = null;
		this.gridColumnEx17.CurrencyFixed = null;
		this.gridColumnEx17.CurrencyFixedRate = 0.0;
		this.gridColumnEx17.CurrencyRateField = null;
		this.gridColumnEx17.FieldName = "Remarks";
		this.gridColumnEx17.Name = "gridColumnEx17";
		this.gridColumnEx17.PrimaryKey = false;
		this.gridColumnEx17.UnitField = null;
		this.gridColumnEx17.Visible = true;
		this.gridColumnEx17.VisibleIndex = 8;
		this.repositoryItemMemoExEdit2.AutoHeight = false;
		this.repositoryItemMemoExEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit2.Name = "repositoryItemMemoExEdit2";
		this.repositoryItemMemoExEdit2.ShowIcon = false;
		this.gridColumnEx18.Caption = "LineStatus";
		this.gridColumnEx18.ColumnEdit = this.luk_StatusLine;
		this.gridColumnEx18.CurrencyDateField = null;
		this.gridColumnEx18.CurrencyField = null;
		this.gridColumnEx18.CurrencyFixed = null;
		this.gridColumnEx18.CurrencyFixedRate = 0.0;
		this.gridColumnEx18.CurrencyRateField = null;
		this.gridColumnEx18.FieldName = "LineStatus";
		this.gridColumnEx18.Name = "gridColumnEx18";
		this.gridColumnEx18.OptionsColumn.ReadOnly = true;
		this.gridColumnEx18.PrimaryKey = false;
		this.gridColumnEx18.UnitField = null;
		this.gridColumnEx18.Visible = true;
		this.gridColumnEx18.VisibleIndex = 9;
		this.luk_StatusLine.AutoHeight = false;
		this.luk_StatusLine.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_StatusLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_StatusLine.IsSearchControl = false;
		this.luk_StatusLine.Name = "luk_StatusLine";
		this.luk_StatusLine.NullText = "";
		this.luk_StatusLine.ShowFooter = false;
		this.luk_StatusLine.ShowHeader = false;
		this.gridColumnEx19.Caption = "OpenQty";
		this.gridColumnEx19.ColumnEdit = this.repositoryItemCalcEditEx5;
		this.gridColumnEx19.CurrencyDateField = null;
		this.gridColumnEx19.CurrencyField = null;
		this.gridColumnEx19.CurrencyFixed = null;
		this.gridColumnEx19.CurrencyFixedRate = 0.0;
		this.gridColumnEx19.CurrencyRateField = null;
		this.gridColumnEx19.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx19.FieldName = "OpenQty";
		this.gridColumnEx19.Name = "gridColumnEx19";
		this.gridColumnEx19.OptionsColumn.ReadOnly = true;
		this.gridColumnEx19.PrimaryKey = false;
		this.gridColumnEx19.UnitField = null;
		this.gridColumnEx19.Visible = true;
		this.gridColumnEx19.VisibleIndex = 10;
		this.gridColumnEx20.Caption = "BaseLine";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "BaseLine";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.OptionsColumn.ReadOnly = true;
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx21.Caption = "BaseEntry";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "BaseEntry";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.OptionsColumn.ReadOnly = true;
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx23.Caption = "Actual Quantity";
		this.gridColumnEx23.ColumnEdit = this.repositoryItemCalcEditEx6;
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gridColumnEx23.FieldName = "ActualQty";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx23.Visible = true;
		this.gridColumnEx23.VisibleIndex = 11;
		this.gridColumnEx24.Caption = "ActualAmount";
		this.gridColumnEx24.ColumnEdit = this.repositoryItemCalcEditEx7;
		this.gridColumnEx24.Currency = Apzon.Commons.Enums.Enums.CurrencyColumn.Custom;
		this.gridColumnEx24.CurrencyDateField = null;
		this.gridColumnEx24.CurrencyField = "Currency";
		this.gridColumnEx24.CurrencyFixed = null;
		this.gridColumnEx24.CurrencyFixedRate = 0.0;
		this.gridColumnEx24.CurrencyRateField = null;
		this.gridColumnEx24.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Amount;
		this.gridColumnEx24.FieldName = "ActualAmount";
		this.gridColumnEx24.Name = "gridColumnEx24";
		this.gridColumnEx24.PrimaryKey = false;
		this.gridColumnEx24.UnitField = null;
		this.gridColumnEx24.Visible = true;
		this.gridColumnEx24.VisibleIndex = 12;
		this.gridColumnEx27.Caption = "PostingDate";
		this.gridColumnEx27.CurrencyDateField = null;
		this.gridColumnEx27.CurrencyField = null;
		this.gridColumnEx27.CurrencyFixed = null;
		this.gridColumnEx27.CurrencyFixedRate = 0.0;
		this.gridColumnEx27.CurrencyRateField = null;
		this.gridColumnEx27.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx27.FieldName = "PostingDate";
		this.gridColumnEx27.Name = "gridColumnEx27";
		this.gridColumnEx27.PrimaryKey = false;
		this.gridColumnEx27.UnitField = null;
		this.gridColumnEx27.Visible = true;
		this.gridColumnEx27.VisibleIndex = 13;
		this.gridColumnEx28.Caption = "Due Date";
		this.gridColumnEx28.CurrencyDateField = null;
		this.gridColumnEx28.CurrencyField = null;
		this.gridColumnEx28.CurrencyFixed = null;
		this.gridColumnEx28.CurrencyFixedRate = 0.0;
		this.gridColumnEx28.CurrencyRateField = null;
		this.gridColumnEx28.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx28.FieldName = "DueDate";
		this.gridColumnEx28.Name = "gridColumnEx28";
		this.gridColumnEx28.PrimaryKey = false;
		this.gridColumnEx28.UnitField = null;
		this.gridColumnEx28.Visible = true;
		this.gridColumnEx28.VisibleIndex = 14;
		this.gridColumnEx29.Caption = "Document Date";
		this.gridColumnEx29.CurrencyDateField = null;
		this.gridColumnEx29.CurrencyField = null;
		this.gridColumnEx29.CurrencyFixed = null;
		this.gridColumnEx29.CurrencyFixedRate = 0.0;
		this.gridColumnEx29.CurrencyRateField = null;
		this.gridColumnEx29.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx29.FieldName = "DocumentDate";
		this.gridColumnEx29.Name = "gridColumnEx29";
		this.gridColumnEx29.PrimaryKey = false;
		this.gridColumnEx29.UnitField = null;
		this.gridColumnEx29.Visible = true;
		this.gridColumnEx29.VisibleIndex = 15;
		this.gridColumnEx31.Caption = "Currency";
		this.gridColumnEx31.ColumnEdit = this.luk_Currency;
		this.gridColumnEx31.CurrencyDateField = null;
		this.gridColumnEx31.CurrencyField = null;
		this.gridColumnEx31.CurrencyFixed = null;
		this.gridColumnEx31.CurrencyFixedRate = 0.0;
		this.gridColumnEx31.CurrencyRateField = null;
		this.gridColumnEx31.FieldName = "Currency";
		this.gridColumnEx31.Name = "gridColumnEx31";
		this.gridColumnEx31.PrimaryKey = false;
		this.gridColumnEx31.UnitField = null;
		this.gridColumnEx31.Visible = true;
		this.gridColumnEx31.VisibleIndex = 16;
		this.luk_Currency.AutoHeight = false;
		this.luk_Currency.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Currency.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("CurrCode", "Code")
		});
		this.luk_Currency.DisplayMember = "CurrCode";
		this.luk_Currency.Name = "luk_Currency";
		this.luk_Currency.NullText = "";
		this.luk_Currency.ValueMember = "CurrCode";
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.Location = new System.Drawing.Point(1, 467);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 1;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.Location = new System.Drawing.Point(82, 467);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Project Code";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(184, 41);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 4;
		this.labelLine2.Utext = "Project Code";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "From Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(1, 7);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 5;
		this.labelLine3.Utext = "From Date";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "To Date";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(1, 41);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 6;
		this.labelLine4.Utext = "To Date";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(580, 35);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 8;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.btn_ProjectCode.EditControl = null;
		this.btn_ProjectCode.FieldName = null;
		this.btn_ProjectCode.IsSearchControl = true;
		this.btn_ProjectCode.Location = new System.Drawing.Point(260, 38);
		this.btn_ProjectCode.Name = "btn_ProjectCode";
		this.btn_ProjectCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_ProjectCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ProjectCode.Properties.ContainerControl = null;
		this.btn_ProjectCode.Properties.DisplayMember = "Code";
		this.btn_ProjectCode.Properties.DisplayMember2 = "";
		this.btn_ProjectCode.Properties.LoadDataSourceWhenEnter = false;
		this.btn_ProjectCode.Properties.SeparateCharacterForMulti = ';';
		this.btn_ProjectCode.Properties.SeparateString = ", ";
		this.btn_ProjectCode.Properties.UsingAutoCompleteBox = false;
		this.btn_ProjectCode.Properties.UsingDataSource = true;
		this.btn_ProjectCode.Properties.ValidateWhenNoDataSource = false;
		this.btn_ProjectCode.Properties.ValueMember = "AbsEntry";
		this.btn_ProjectCode.SettingInVisible = false;
		this.btn_ProjectCode.SettingReadonly = false;
		this.btn_ProjectCode.Size = new System.Drawing.Size(100, 20);
		this.btn_ProjectCode.TabIndex = 12;
		this.dat_FromDate.EditControl = null;
		this.dat_FromDate.EditValue = null;
		this.dat_FromDate.FieldName = null;
		this.dat_FromDate.Location = new System.Drawing.Point(78, 4);
		this.dat_FromDate.Name = "dat_FromDate";
		this.dat_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dat_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dat_FromDate.Properties.IsSearchControl = true;
		this.dat_FromDate.SettingInVisible = false;
		this.dat_FromDate.SettingReadonly = false;
		this.dat_FromDate.Size = new System.Drawing.Size(100, 20);
		this.dat_FromDate.TabIndex = 13;
		this.dat_ToDate.EditControl = null;
		this.dat_ToDate.EditValue = null;
		this.dat_ToDate.FieldName = null;
		this.dat_ToDate.Location = new System.Drawing.Point(78, 38);
		this.dat_ToDate.Name = "dat_ToDate";
		this.dat_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dat_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dat_ToDate.Properties.IsSearchControl = true;
		this.dat_ToDate.SettingInVisible = false;
		this.dat_ToDate.SettingReadonly = false;
		this.dat_ToDate.Size = new System.Drawing.Size(100, 20);
		this.dat_ToDate.TabIndex = 14;
		this.luk_Status.EditControl = null;
		this.luk_Status.FieldName = null;
		this.luk_Status.IsSearchControl = true;
		this.luk_Status.Location = new System.Drawing.Point(260, 4);
		this.luk_Status.Name = "luk_Status";
		this.luk_Status.Properties.AutoHeight = false;
		this.luk_Status.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Status.Properties.IsSearchControl = false;
		this.luk_Status.Properties.NullText = "";
		this.luk_Status.Properties.ShowFooter = false;
		this.luk_Status.Properties.ShowHeader = false;
		this.luk_Status.SettingInVisible = false;
		this.luk_Status.SettingReadonly = false;
		this.luk_Status.Size = new System.Drawing.Size(100, 20);
		this.luk_Status.TabIndex = 15;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Status";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(184, 7);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(76, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 16;
		this.labelLine1.Utext = "Status";
		this.drBtn_CopyTo.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.drBtn_CopyTo.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("drBtn_CopyTo.ImageOptions.Image");
		this.drBtn_CopyTo.Location = new System.Drawing.Point(728, 467);
		this.drBtn_CopyTo.Name = "drBtn_CopyTo";
		this.drBtn_CopyTo.Size = new System.Drawing.Size(98, 23);
		this.drBtn_CopyTo.TabIndex = 19;
		this.drBtn_CopyTo.Text = "Copy To";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Doc. No";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(377, 41);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 20;
		this.labelLine5.Utext = "Doc. No";
		this.txt_Code.EditControl = null;
		this.txt_Code.FieldName = null;
		this.txt_Code.IsSearchControl = true;
		this.txt_Code.Location = new System.Drawing.Point(455, 38);
		this.txt_Code.Name = "txt_Code";
		this.txt_Code.SettingInVisible = false;
		this.txt_Code.SettingReadonly = false;
		this.txt_Code.Size = new System.Drawing.Size(100, 20);
		this.txt_Code.TabIndex = 21;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(832, 491);
		base.Controls.Add(this.txt_Code);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.drBtn_CopyTo);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.luk_Status);
		base.Controls.Add(this.dat_ToDate);
		base.Controls.Add(this.dat_FromDate);
		base.Controls.Add(this.btn_ProjectCode);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oReportCompletion";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ProjectReportCompletion";
		this.Text = "Report";
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEditExPrice1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx6).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx7).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Report).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Report).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_PjCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dat_Posting.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dat_Posting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusGrd).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_DocCurr).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_LineReport).EndInit();
		this.ctm_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_LineReport).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Wbs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_Item).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_StatusLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Currency).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ProjectCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dat_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dat_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dat_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dat_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Code.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
