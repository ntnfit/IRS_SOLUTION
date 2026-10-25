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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.RoutingAndResource;

public class frm_AdvanceRouting : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int uomGrpEntry;

	private bool _isLoadDataLine = true;

	private DataTable _dataResourceEffic = new DataTable();

	private IContainer components;

	private SimpleButton btnSearch;

	private DateEditEx txt_ToDate;

	private DateEditEx txt_FromDate;

	private LookUpEditEx txt_Status;

	private ButtonEditCFL txt_RoutNo;

	private LabelLine lbToDate;

	private LabelLine lbFromDate;

	private LabelLine lbStatus;

	private LabelLine lbRourCode;

	private SplitContainerControl splitContainerControl1;

	private SimpleButton btnSave;

	private SimpleButton btnCancel;

	private ContextMenuStrip contextMenuStrip_Doc;

	private ToolStripMenuItem addNewDocToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip_Line;

	private ToolStripMenuItem addNewLineToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gcRoutNo;

	private GridColumnEx gcRoutName;

	private GridColumnEx gcProdCode;

	private RepositoryItemButtonEditCFL rProdCode;

	private GridColumnEx gcProdName;

	private RepositoryItemButtonEditCFL rProdName;

	private GridColumnEx gcStatus;

	private RepositoryItemLookUpEdit rStatus;

	private GridColumnEx gcFromDate;

	private RepositoryItemDateEdit rDate;

	private GridColumnEx gcToDate;

	private GridColumnEx gcRemark;

	private RepositoryItemButtonEditCFL rlookupRoutNo;

	private ToolStripMenuItem deleteToolStripMenuItem1;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private GridColumnEx gcOpeCode;

	private RepositoryItemButtonEditCFL bte_OpeCode;

	private GridColumnEx gcOpeName;

	private RepositoryItemButtonEditCFL bte_OpeName;

	private GridColumnEx gcResCode;

	private RepositoryItemButtonEditCFL bte_ResCode;

	private GridColumnEx gcResName;

	private RepositoryItemButtonEditCFL bte_ResName;

	private GridColumnEx gcSetupTime;

	private RepositoryItemCalcEdit cal_Edit;

	private GridColumnEx gcSetupUnit;

	private RepositoryItemLookUpEdit lookup_SetUnit;

	private GridColumnEx gcRunTime;

	private GridColumnEx gcRunUnit;

	private RepositoryItemLookUpEdit lookup_RunUnit;

	private GridColumnEx gcBaseQty;

	private GridColumnEx gcUoM;

	private RepositoryItemButtonEditCFL bte_UoM;

	private GridColumnEx gcWorkOrder;

	private RepositoryItemLookUpEdit lookup_WorkOrder;

	private GridColumnEx gcLineProductCode;

	private RepositoryItemButtonEditCFL bte_ProdCode;

	private GridColumnEx gcLineProductName;

	private RepositoryItemButtonEditCFL bte_ProdName;

	private GridColumnEx gcSetupTask;

	private RepositoryItemButtonEditCFL bte_SetupTask;

	private GridColumnEx gcRunTask;

	private RepositoryItemButtonEditCFL bte_RunTask;

	private GridColumnEx gcVisOrder;

	private GridColumnEx gcTransTime;

	private GridColumnEx gcUomTransTIme;

	private RepositoryItemHourEdit rTime;

	private GridColumnEx gcSubRouting;

	private RepositoryItemButtonEdit rSubRouting;

	private GridColumnEx gcIsBase;

	private RepositoryItemLookUpEdit rLookIsBase;

	private SimpleButton btnImportExcel;

	private GridColumnEx gcLineUomName;

	private GridColumnEx gcLineNumPerMsr;

	private GridColumnEx gcLineInvQty;

	private RepositoryItemButtonEdit rUomCode;

	private SplitContainerControl splitLine;

	private GridControlEx grdEffic;

	private GridViewEx gvEffic;

	private GridColumnEx gcEfficResCode;

	private RepositoryItemButtonEditCFL rEfficResCode;

	private GridColumnEx gcEfficResName;

	private GridColumnEx gcEfficEfficiency;

	private RepositoryItemCalcEdit rEfficCal;

	private ContextMenuStrip contextMenuEffic;

	private ToolStripMenuItem addNewLineToolStripMenuItem1;

	private ToolStripMenuItem removeRowsToolStripMenuItem;

	private string routCode { get; set; }

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
		case Enums.FormViewModeType.View:
			gcRoutNo.OptionsColumn.ReadOnly = true;
			gv_Line.ReadOnly = false;
			gvDoc.ReadOnly = false;
			btnSave.Enabled = true;
			btnCancel.Enabled = true;
			btnSave.Text = "OK";
			break;
		case Enums.FormViewModeType.Add:
			gvDoc.ReadOnly = false;
			gv_Line.ReadOnly = false;
			gcRoutNo.OptionsColumn.ReadOnly = false;
			btnSave.Enabled = true;
			btnCancel.Enabled = true;
			btnSave.Text = "Save";
			break;
		case Enums.FormViewModeType.Update:
			gvDoc.ReadOnly = false;
			gv_Line.ReadOnly = false;
			gcRoutNo.OptionsColumn.ReadOnly = true;
			btnSave.Enabled = true;
			btnCancel.Enabled = true;
			btnSave.Text = "Update";
			break;
		case Enums.FormViewModeType.Busy:
			btnSave.Enabled = false;
			btnCancel.Enabled = false;
			gvDoc.ReadOnly = true;
			gv_Line.ReadOnly = true;
			break;
		}
	}

	public frm_AdvanceRouting()
	{
		InitializeComponent();
		OnLoadAsync += frm_AdvanceRoutings_Load;
	}

	private async Task frm_AdvanceRoutings_Load(object sender, EventArgs e)
	{
		try
		{
			LoadStatusData();
			LoadTimeUom();
			LoadDataWorkOrder();
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rProdName;
			repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue(gcProdCode, chooseItem["Code"]);
					grdDoc.DataSourceTable.AcceptChanges();
					gvDoc.RefreshData();
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = bte_ProdCode;
			repositoryItemButtonEditCFL2.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL2.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.ProductCode = Function.ToString(gvDoc.GetFocusedRowCellValue(gcProdCode));
				grd_Line.DataSourceTable.AcceptChanges();
				grd_Line.Refresh();
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = bte_ProdName;
			repositoryItemButtonEditCFL3.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL3.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.ProductCode = Function.ToString(gvDoc.GetFocusedRowCellValue(gcProdCode));
				grd_Line.DataSourceTable.AcceptChanges();
				grd_Line.Refresh();
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL4 = bte_OpeCode;
			repositoryItemButtonEditCFL4.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL4.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gv_Line.SetFocusedRowCellValue(gcOpeCode, chooseItem["OpeCode"]);
					grd_Line.DataSourceTable.AcceptChanges();
					grd_Line.Refresh();
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL5 = bte_ResCode;
			repositoryItemButtonEditCFL5.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL5.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gv_Line.SetFocusedRowCellValue(gcResCode, chooseItem["ResCode"]);
					grd_Line.DataSourceTable.AcceptChanges();
					grd_Line.Refresh();
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL6 = rEfficResCode;
			repositoryItemButtonEditCFL6.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL6.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gvEffic.SetFocusedRowCellValue(gcEfficResCode, chooseItem["ResCode"]);
					grd_Line.DataSourceTable.AcceptChanges();
					grd_Line.Refresh();
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL7 = bte_ProdCode;
			repositoryItemButtonEditCFL7.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL7.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gv_Line.SetFocusedRowCellValue(gcLineProductCode.FieldName, chooseItem["ItemCode"]);
					grd_Line.DataSourceTable.AcceptChanges();
					grd_Line.Refresh();
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL8 = bte_OpeName;
			repositoryItemButtonEditCFL8.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL8.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gv_Line.SetFocusedRowCellValue(gcOpeCode, chooseItem["OpeCode"]);
					grd_Line.DataSourceTable.AcceptChanges();
					grd_Line.Refresh();
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL9 = bte_ResName;
			repositoryItemButtonEditCFL9.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL9.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gv_Line.SetFocusedRowCellValue(gcResCode, chooseItem["ResCode"]);
					grd_Line.DataSourceTable.AcceptChanges();
					grd_Line.Refresh();
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL10 = bte_ProdName;
			repositoryItemButtonEditCFL10.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL10.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gv_Line.SetFocusedRowCellValue(gcLineProductCode.FieldName, chooseItem["ItemCode"]);
					grd_Line.DataSourceTable.AcceptChanges();
					grd_Line.Refresh();
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL11 = bte_RunTask;
			repositoryItemButtonEditCFL11.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL11.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gv_Line.SetFocusedRowCellValue(gcRunTask, chooseItem["ClgCode"]);
					grd_Line.DataSourceTable.AcceptChanges();
					grd_Line.Refresh();
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL12 = bte_SetupTask;
			repositoryItemButtonEditCFL12.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL12.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gv_Line.SetFocusedRowCellValue(gcSetupTask, chooseItem["ClgCode"]);
					grd_Line.DataSourceTable.AcceptChanges();
					grd_Line.Refresh();
				}
			}));
			await LoadDataDoc();
			bte_UoM.ValueMember = "UomCode";
			bte_UoM.DisplayMember = "UomName";
			bte_UoM.TypeCFL = Enums.TypeCFL.ListUoMByUoMGroupItemMaster;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL13 = bte_UoM;
			repositoryItemButtonEditCFL13.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL13.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm.DataInt1 = uomGrpEntry;
				frm.DataString2 = "I";
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL14 = bte_UoM;
			repositoryItemButtonEditCFL14.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL14.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					DataTable dataSourceTable = grd_Line.DataSourceTable;
					int focusedRowHandle = gv_Line.FocusedRowHandle;
					dataSourceTable.Rows[focusedRowHandle]["UomEntry"] = chooseItem["UomEntry"];
					dataSourceTable.AcceptChanges();
					grd_Line.DataSource = dataSourceTable;
					gv_Line.SetFocusedRowCellValue(gcUoM, chooseItem["UomCode"]);
					grd_Line.DataSourceTable.AcceptChanges();
					grd_Line.Refresh();
				}
			}));
			bte_UoM.RefreshDataSource();
			gvDoc.OptionsView.ColumnAutoWidth = true;
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "Y",
					Name = "Yes"
				},
				new Common.Table_Code
				{
					Code = "N",
					Name = "No"
				}
			};
			rLookIsBase.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataDoc()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			txt_RoutNo.ResetText();
			txt_Status.EditValue = "A";
			txt_FromDate.EditValue = null;
			txt_ToDate.EditValue = null;
			await LoadData(Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadStatusData()
	{
		List<Common.Table_Code> dataSource = new List<Common.Table_Code>
		{
			new Common.Table_Code
			{
				Name = "Active",
				Code = "A"
			},
			new Common.Table_Code
			{
				Name = "Inactive",
				Code = "I"
			},
			new Common.Table_Code
			{
				Name = "",
				Code = ""
			}
		};
		txt_Status.Properties.DataSource = dataSource;
		rStatus.DataSource = dataSource;
	}

	private void LoadTimeUom()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("M", "Min");
		dictionary.Add("H", "Hour");
		dictionary.Add("D", "Day");
		lookup_SetUnit.DataSource = dictionary;
		lookup_RunUnit.DataSource = dictionary;
	}

	private void LoadDataWorkOrder()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("Y", "Yes");
		dictionary.Add("N", "No");
		lookup_WorkOrder.DataSource = dictionary;
	}

	public async Task LoadDataLine(string routCode)
	{
		try
		{
			if (_isLoadDataLine)
			{
				DataSet dataSet = await _httpClient.AdvanceRoutingClient.GetDataLine(routCode);
				if (dataSet == null || dataSet.Tables.Count != 2)
				{
					WinCommons.Error(Enums.MessageCode.CannotGetRoutingLines, "");
					grdEffic.DataSourceTable.Rows.Clear();
					grd_Line.DataSourceTable.Rows.Clear();
					splitLine.PanelVisibility = SplitPanelVisibility.Panel1;
				}
				else
				{
					grdEffic.DataSource = dataSet.Tables[1].Clone();
					_dataResourceEffic = dataSet.Tables[1];
					grd_Line.DataSource = dataSet.Tables[0];
					grd_Line.Refresh();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task Search()
	{
		await LoadData(Enums.NavigateType.Current);
	}

	public async Task MODE_FIND()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			AddNewRowDoc();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
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
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
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
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
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
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadDataDoc();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task LoadData(Enums.NavigateType type)
	{
		try
		{
			WinCommons._OpenProcessing();
			string value = (string.IsNullOrEmpty(Function.ToString(txt_FromDate.EditValue)) ? "" : Function.ParseDateTimes(txt_FromDate.EditValue).ToString("yyyyMMdd"));
			string value2 = (string.IsNullOrEmpty(Function.ToString(txt_ToDate.EditValue)) ? "" : Function.ParseDateTimes(txt_ToDate.EditValue).ToString("yyyyMMdd"));
			object editValue = txt_RoutNo.EditValue;
			object editValue2 = txt_Status.EditValue;
			DataTable dataTable = new DataTable
			{
				Columns = { "RoutCode", "Status", "FromDate", "ToDate" }
			};
			DataRow dataRow = dataTable.NewRow();
			if (type == Enums.NavigateType.Current)
			{
				dataRow["RoutCode"] = editValue;
				dataRow["Status"] = editValue2;
				dataRow["FromDate"] = value;
				dataRow["ToDate"] = value2;
			}
			else
			{
				dataRow["RoutCode"] = routCode;
			}
			dataTable.Rows.Add(dataRow);
			Tuple<DataTable, Enums.NavigateType> tuple = new Tuple<DataTable, Enums.NavigateType>(dataTable, type);
			DataTable dataTable2 = await _httpClient.AdvanceRoutingClient.GetDataDoc(tuple);
			grdDoc.DataSource = dataTable2;
			if (!dataTable2.IsNotNull())
			{
				grd_Line.DataSource = null;
			}
			FormMode = Enums.FormViewModeType.View;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Save()
	{
		if (gvDoc.FocusedRowHandle < 0)
		{
			return;
		}
		if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcRoutNo))))
		{
			WinCommons.Error(Enums.MessageCode.YouMustEnterRoutNo, "");
			return;
		}
		if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcProdCode))))
		{
			WinCommons.Error(Enums.MessageCode.YouMustEnterProductCode, "");
			return;
		}
		DateTime? dateTime = Function.ParseDateTime(gvDoc.GetFocusedRowCellValue(gcFromDate));
		DateTime? dateTime2 = Function.ParseDateTime(gvDoc.GetFocusedRowCellValue(gcToDate));
		if (dateTime.HasValue && dateTime2.HasValue && dateTime2 < dateTime)
		{
			WinCommons.Error(Enums.MessageCode.ToDateMustGreaterFromDate, "");
			return;
		}
		if (!(grd_Line.DataSource as DataTable).IsNotNull())
		{
			WinCommons.Error(Enums.MessageCode.DataLinesIsEmpty, "");
			return;
		}
		Enums.FormViewModeType oldView = FormMode;
		WinCommons._OpenProcessing();
		int rowFocus = gvDoc.FocusedRowHandle;
		DataSet dataSet = new DataSet();
		DataRow focusedDataRow = gvDoc.GetFocusedDataRow();
		DataTable dataTable = focusedDataRow.Table.Clone();
		dataTable.Rows.Add(focusedDataRow.ItemArray);
		dataTable.AcceptChanges();
		DataTable dataSourceTable = grd_Line.DataSourceTable;
		DataTable dataTable2 = _dataResourceEffic.Copy();
		List<DataRow> list = (from t in grdEffic.DataSourceTable.AsEnumerable()
			where !string.IsNullOrEmpty(Function.ToString(t["ResCode"]))
			select t).ToList();
		for (int num = 0; num < list.Count; num++)
		{
			DataRow dataRow = dataTable2.NewRow();
			dataRow.ItemArray = list[num].ItemArray;
			dataTable2.Rows.Add(dataRow);
		}
		dataTable2.AcceptChanges();
		dataTable.TableName = "DataDoc";
		dataSourceTable.TableName = "DataLines";
		dataTable2.TableName = "DataEfficiency";
		dataSet.Tables.Add(dataTable);
		dataSet.Tables.Add(dataSourceTable.Copy());
		dataSet.Tables.Add(dataTable2);
		Tuple<DataSet, Enums.TransactionType> tuple = new Tuple<DataSet, Enums.TransactionType>(dataSet, (oldView != Enums.FormViewModeType.Add) ? Enums.TransactionType.U : Enums.TransactionType.A);
		HttpResult httpResult = await _httpClient.AdvanceRoutingClient.SaveData(tuple);
		if (!string.IsNullOrEmpty(httpResult.Message))
		{
			WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			FormMode = oldView;
		}
		else
		{
			WinCommons.Success(Enums.MessageCode.Success, "");
			await LoadDataDoc();
			gvDoc.FocusedRowHandle = rowFocus;
		}
		WinCommons._CloseProcessing();
	}

	public async void AddNewRowDoc()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSourceTable.Clone();
			if (dataTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetStructureToAddNew, "");
				return;
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow[gcFromDate.FieldName] = DateTime.Now.Date;
			dataRow[gcStatus.FieldName] = "A";
			dataRow[gcIsBase.FieldName] = "Y";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grdDoc.DataSource = dataTable;
			grdDoc.Refresh();
			FormMode = Enums.FormViewModeType.Add;
			await LoadDataLine(Function.ToString(gvDoc.GetFocusedRowCellValue(gcRoutNo)));
			AddNewRowLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void AddNewRowLine()
	{
		try
		{
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			if (dataSourceTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetStructureToAddNew, "");
				return;
			}
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow[gcRunUnit.FieldName] = "M";
			dataRow[gcSetupUnit.FieldName] = "M";
			dataRow[gcUomTransTIme.FieldName] = "M";
			dataRow[gcWorkOrder.FieldName] = "N";
			dataRow[gcBaseQty.FieldName] = 1;
			dataRow[gcLineInvQty.FieldName] = 1;
			dataRow[gcLineNumPerMsr.FieldName] = 1;
			dataRow[gcLineUomName.FieldName] = Function.ToString(gvDoc.GetFocusedRowCellValue("InvUnitMsr"));
			if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpCode")) == -1)
			{
				dataRow["UomCode"] = "Manual";
			}
			else
			{
				dataRow["UomCode"] = WinFuntions.GetUoMCodeByName(Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpCode")), Function.ToString(gvDoc.GetFocusedRowCellValue("InvUnitMsr")));
			}
			dataRow["LineNum"] = GetNewLineNum(dataSourceTable);
			dataRow[gcVisOrder.FieldName] = GetNewVisOrder(dataSourceTable);
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
			grd_Line.DataSource = dataSourceTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private int GetNewLineNum(DataTable dt)
	{
		if (!dt.IsNotNull())
		{
			return 0;
		}
		return dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"])) + 1;
	}

	private int GetNewVisOrder(DataTable dt)
	{
		if (!dt.IsNotNull())
		{
			return 0;
		}
		return dt.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["VisOrder"])) + 1;
	}

	private void LoadDataResourceEffic(int rowHandle)
	{
		try
		{
			if (_dataResourceEffic == null || _dataResourceEffic.Columns.Count == 0)
			{
				return;
			}
			List<DataRow> list = (from t in grdEffic.DataSourceTable.AsEnumerable()
				where !string.IsNullOrEmpty(Function.ToString(t["ResCode"]))
				select t).ToList();
			for (int num = 0; num < list.Count; num++)
			{
				DataRow dataRow = _dataResourceEffic.NewRow();
				dataRow.ItemArray = list[num].ItemArray;
				_dataResourceEffic.Rows.Add(dataRow);
			}
			_dataResourceEffic.AcceptChanges();
			grdEffic.DataSourceTable.Rows.Clear();
			if (Function.ToString(gv_Line.GetRowCellValue(rowHandle, gcWorkOrder)).Equals("Y"))
			{
				splitLine.PanelVisibility = SplitPanelVisibility.Panel1;
				return;
			}
			splitLine.PanelVisibility = SplitPanelVisibility.Both;
			splitLine.SplitterPosition = splitLine.Width - 300;
			int lineNum = Function.ParseInt(gv_Line.GetRowCellValue(rowHandle, "LineNum"));
			List<DataRow> list2 = (from t in _dataResourceEffic.AsEnumerable()
				where Function.ParseInt(t["FatherLine"]) == lineNum
				select t).ToList();
			if (list2.Any())
			{
				grdEffic.DataSource = list2.CopyToDataTable();
				for (int num2 = 0; num2 < list2.Count; num2++)
				{
					_dataResourceEffic.Rows.Remove(list2[num2]);
				}
				_dataResourceEffic.AcceptChanges();
			}
			else
			{
				grdEffic.DataSource = _dataResourceEffic.Clone();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addNewDocToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRowDoc();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addNewLineToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRowLine();
			if (FormMode != Enums.FormViewModeType.Add)
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

	private async void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			routCode = Function.ToString(gvDoc.GetFocusedRowCellValue(gcRoutNo));
			await LoadDataLine(routCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (!gvDoc.FocusedColumn.FieldName.Equals(gcRoutNo.FieldName) && string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcRoutNo))))
			{
				WinCommons.Warning(Enums.MessageCode.EnterRouteNo, "");
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
				{
					await Save();
				}
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e == null)
			{
				return;
			}
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			int dataSourceRowIndex = gvDoc.GetDataSourceRowIndex(e.RowHandle);
			if (e.Column.FieldName.Equals(gcProdCode.FieldName))
			{
				DataTable dataItemCode = _httpClient.Commons.GetDataItemCode(Function.ToString(e.Value), -1, GlobalData.UserName, GlobalData.DefaultBranch);
				if (dataItemCode.IsNotNull())
				{
					gvDoc.SetRowCellValue(e.RowHandle, gcProdName, dataItemCode.Rows[0]["ItemName"]);
					dataSourceTable.Rows[dataSourceRowIndex]["SalUnitMsr"] = dataItemCode.Rows[0]["SalUnitMsr"];
					dataSourceTable.Rows[dataSourceRowIndex]["NumInSale"] = dataItemCode.Rows[0]["NumInSale"];
					dataSourceTable.Rows[dataSourceRowIndex]["BuyUnitMsr"] = Function.ToString(dataItemCode.Rows[0]["BuyUnitMsr"]);
					dataSourceTable.Rows[dataSourceRowIndex]["NumInBuy"] = Function.ParseDouble(dataItemCode.Rows[0]["NumInBuy"]);
					dataSourceTable.Rows[dataSourceRowIndex]["UgpCode"] = dataItemCode.Rows[0]["UgpEntry"];
					dataSourceTable.Rows[dataSourceRowIndex]["InvUnitMsr"] = dataItemCode.Rows[0]["InvntryUom"];
					dataSourceTable.Rows[dataSourceRowIndex]["NumInInven"] = dataItemCode.Rows[0]["NumInInven"];
					grd_Line.DataSourceTable.Rows.Clear();
					AddNewRowLine();
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.ProductionDoesNotExitst, "");
					dataSourceTable.Rows[dataSourceRowIndex][gcProdCode.FieldName] = "";
				}
			}
			else if (e.Column.FieldName.Equals(gcFromDate.FieldName))
			{
				DateTime? dateTime = Function.ParseDateTime(gvDoc.GetRowCellValue(e.RowHandle, gcFromDate));
				DateTime? dateTime2 = Function.ParseDateTime(gvDoc.GetRowCellValue(e.RowHandle, gcToDate));
				if (dateTime.HasValue && dateTime2.HasValue && dateTime2 < dateTime)
				{
					WinCommons.Warning(Enums.MessageCode.FromDateMustBeBefore2Date, "");
					gvDoc.SetRowCellValue(e.RowHandle, gcFromDate, dateTime2);
				}
			}
			else if (e.Column.FieldName.Equals(gcToDate.FieldName))
			{
				DateTime? dateTime3 = Function.ParseDateTime(gvDoc.GetRowCellValue(e.RowHandle, gcFromDate));
				DateTime? dateTime4 = Function.ParseDateTime(gvDoc.GetRowCellValue(e.RowHandle, gcToDate));
				if (dateTime3.HasValue && dateTime4.HasValue && dateTime4 < dateTime3)
				{
					WinCommons.Warning(Enums.MessageCode.ToDateMustGreaterFromDate, "");
					gvDoc.SetRowCellValue(e.RowHandle, gcToDate, dateTime3);
				}
			}
			grdDoc.DataSourceTable.AcceptChanges();
			gvDoc.RefreshData();
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

	private void gvDoc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
				{
					e.Allow = false;
				}
				else
				{
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

	private async void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				HttpResult httpResult = await _httpClient.AdvanceRoutingClient.DeleteDataDoc(Function.ToString(gvDoc.GetFocusedRowCellValue(gcRoutNo)));
				if (httpResult.Message == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
				await LoadDataDoc();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				int[] selectedRows = gv_Line.GetSelectedRows();
				List<int> lstLineNum = new List<int>();
				for (int i = 0; i < selectedRows.Length; i++)
				{
					lstLineNum.Add(Function.ParseInt(gv_Line.GetRowCellValue(selectedRows[i], "LineNum")));
				}
				List<DataRow> list = (from t in _dataResourceEffic.AsEnumerable()
					where lstLineNum.Contains(Function.ParseInt(t["FatherLine"]))
					select t).ToList();
				for (int num = 0; num < list.Count; num++)
				{
					_dataResourceEffic.Rows.Remove(list[num]);
				}
				_dataResourceEffic.AcceptChanges();
				grdEffic.DataSourceTable.Rows.Clear();
				gv_Line.DeleteDataSourceRowSelected();
				AddNewRowLine();
				if (FormMode != Enums.FormViewModeType.Add)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void duplicateToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvDoc.FocusedRowHandle >= 0)
			{
				DataTable dataSource = grd_Line.DataSourceTable.Copy();
				DataTable dataTable = grdDoc.DataSourceTable.Clone();
				DataRow dataRow = dataTable.NewRow();
				DataRow focusedDataRow = gvDoc.GetFocusedDataRow();
				dataRow.ItemArray = focusedDataRow.ItemArray;
				dataRow[gcRoutNo.FieldName] = "";
				dataRow[gcRoutName.FieldName] = "";
				dataTable.Rows.Add(dataRow);
				_isLoadDataLine = false;
				grdDoc.DataSource = dataTable;
				grdDoc.Refresh();
				grd_Line.DataSource = dataSource;
				grd_Line.Refresh();
				_isLoadDataLine = true;
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_AdvanceRoutings_FormClosing(object sender, FormClosingEventArgs e)
	{
		if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
		{
			e.Cancel = true;
		}
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			await Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Line_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcRoutNo))) || string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcProdCode))))
			{
				WinCommons.Warning(Enums.MessageCode.EnterRouteNoAndProductCode, "");
				e.Cancel = true;
			}
			if (new string[14]
			{
				gcOpeCode.FieldName, gcResCode.FieldName, gcSetupTime.FieldName, gcSetupUnit.FieldName, gcRunTime.FieldName, gcRunUnit.FieldName, gcTransTime.FieldName, gcUomTransTIme.FieldName, gcSetupTask.FieldName, gcRunTask.FieldName,
				gcBaseQty.FieldName, gcUoM.FieldName, gcResName.FieldName, gcOpeName.FieldName
			}.Contains(gv_Line.FocusedColumn.FieldName) && Function.ToString(gv_Line.GetFocusedRowCellValue(gcWorkOrder)).Equals("Y"))
			{
				e.Cancel = true;
			}
			else if (new string[3] { gcLineProductCode.FieldName, gcLineProductName.FieldName, gcSubRouting.FieldName }.Contains(gv_Line.FocusedColumn.FieldName) && Function.ToString(gv_Line.GetFocusedRowCellValue(gcWorkOrder)).Equals("N"))
			{
				e.Cancel = true;
			}
			else if ((gv_Line.FocusedColumn.FieldName.Equals(gcLineProductCode.FieldName) || gv_Line.FocusedColumn.FieldName.Equals(gcLineProductName.FieldName)) && Function.ToString(gv_Line.GetFocusedRowCellValue(gcWorkOrder)).Equals("N"))
			{
				e.Cancel = true;
			}
			else if (gv_Line.FocusedColumn.FieldName.Equals(gcSubRouting.FieldName) && string.IsNullOrEmpty(Function.ToString(gv_Line.GetFocusedRowCellValue(gcLineProductCode))))
			{
				WinCommons.Warning(Enums.MessageCode.ProductCodeIsNull, "");
				e.Cancel = true;
			}
			else if (gv_Line.FocusedColumn.FieldName.Equals(gcUoM.FieldName))
			{
				if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpCode")) == -1)
				{
					e.Cancel = true;
				}
			}
			else if (gv_Line.FocusedColumn.FieldName.Equals(gcLineUomName.FieldName) && Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpCode")) != -1)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		_ = 1;
		try
		{
			if (e == null)
			{
				return;
			}
			DataTable dataSourceTable = grd_Line.DataSourceTable;
			int dataSourceRowIndex = gv_Line.GetDataSourceRowIndex(e.RowHandle);
			if (e.Column.FieldName.Equals(gcOpeCode.FieldName))
			{
				if (string.IsNullOrEmpty(Function.ToString(e.Value)))
				{
					dataSourceTable.Rows[dataSourceRowIndex]["OpName"] = DBNull.Value;
				}
				else
				{
					DataTable dataTable = await _httpClient.OpearationsClient.GetDataLine(Function.ToString(e.Value));
					if (dataTable.IsNotNull())
					{
						gv_Line.SetRowCellValue(e.RowHandle, gcOpeName, dataTable.Rows[0]["OpeName"]);
					}
					else
					{
						WinCommons.Warning(Enums.MessageCode.OperationDoesNotExists, "");
						gv_Line.SetRowCellValue(e.RowHandle, gcOpeName, "");
					}
				}
			}
			else if (e.Column.FieldName.Equals(gcResCode.FieldName))
			{
				if (string.IsNullOrEmpty(Function.ToString(e.Value)))
				{
					dataSourceTable.Rows[dataSourceRowIndex]["ResName"] = DBNull.Value;
				}
				else
				{
					DataTable dataTable2 = await _httpClient.ResourceWorkCenter.GetDataWorkCenterByCode(Function.ToString(e.Value));
					if (dataTable2.IsNotNull())
					{
						gv_Line.SetRowCellValue(e.RowHandle, gcResName, dataTable2.Rows[0]["ResName"]);
					}
					else
					{
						WinCommons.Warning(Enums.MessageCode.ResourceDoesNotExists, "");
						gv_Line.SetRowCellValue(e.RowHandle, gcResName, "");
					}
				}
				if (Function.ToString(gv_Line.GetRowCellValue(e.RowHandle, gcWorkOrder)).Equals("N"))
				{
					grdEffic.DataSourceTable.Rows.Clear();
					string value = Function.ToString(gv_Line.GetRowCellValue(e.RowHandle, gcResCode));
					if (!string.IsNullOrEmpty(value))
					{
						DataTable dataTable3 = _dataResourceEffic.Clone();
						DataRow dataRow = dataTable3.NewRow();
						dataRow["RoutCode"] = gvDoc.GetFocusedRowCellValue(gcRoutNo);
						dataRow["ResCode"] = value;
						dataRow["FatherLine"] = gv_Line.GetRowCellValue(e.RowHandle, "LineNum");
						dataTable3.Rows.Add(dataRow);
						dataTable3.AcceptChanges();
						grdEffic.DataSource = dataTable3;
						int rowHandle = gvEffic.GetRowHandle(0);
						gvEffic.SetRowCellValue(rowHandle, "ResCode", value);
					}
				}
			}
			else if (e.Column.FieldName.Equals(gcLineProductCode.FieldName))
			{
				if (string.IsNullOrEmpty(Function.ToString(e.Value)))
				{
					dataSourceTable.Rows[dataSourceRowIndex][gcLineProductCode.FieldName] = DBNull.Value;
				}
				else
				{
					DataTable dataItemCode = _httpClient.Commons.GetDataItemCode(Function.ToString(e.Value), -1, GlobalData.UserName, GlobalData.DefaultBranch);
					if (dataItemCode.IsNotNull())
					{
						gv_Line.SetRowCellValue(e.RowHandle, gcLineProductName, dataItemCode.Rows[0]["ItemName"]);
					}
					else
					{
						WinCommons.Warning(Enums.MessageCode.ProductionDoesNotExitst, "");
						dataSourceTable.Rows[dataSourceRowIndex][gcLineProductCode.FieldName] = "";
					}
				}
			}
			else if (e.Column.FieldName == gcUoM.FieldName)
			{
				DataTable dataUomByCode = WinFuntions.GetDataUomByCode(Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpCode")), Function.ToString(e.Value));
				if (dataUomByCode != null && dataUomByCode.Rows.Count > 0)
				{
					dataSourceTable.Rows[dataSourceRowIndex]["UomEntry"] = dataUomByCode.Rows[0]["UomEntry"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineUomName.FieldName] = dataUomByCode.Rows[0]["UomName"];
					dataSourceTable.Rows[dataSourceRowIndex][gcLineNumPerMsr.FieldName] = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("NumInInven")) * Function.ParseDouble(dataUomByCode.Rows[0]["BaseQty"]) / ((Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]) <= 0.0) ? 1.0 : Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]));
					dataSourceTable.Rows[dataSourceRowIndex][gcLineInvQty.FieldName] = Function.ParseDouble(gv_Line.GetRowCellValue(e.RowHandle, gcBaseQty)) * Function.ParseDouble(gv_Line.GetRowCellValue(e.RowHandle, gcLineNumPerMsr));
					grd_Line.DataSourceTable.AcceptChanges();
				}
			}
			else if (e.Column.FieldName.Equals(gcWorkOrder.FieldName))
			{
				if (Function.ToString(gv_Line.GetRowCellValue(e.RowHandle, gcWorkOrder)).Equals("Y"))
				{
					gv_Line.SetRowCellValue(e.RowHandle, gcOpeCode, "");
					gv_Line.SetRowCellValue(e.RowHandle, gcResCode, "");
					dataSourceTable.Rows[dataSourceRowIndex][gcSetupTask.FieldName] = DBNull.Value;
					dataSourceTable.Rows[dataSourceRowIndex][gcRunTask.FieldName] = DBNull.Value;
					dataSourceTable.Rows[dataSourceRowIndex][gcSetupTime.FieldName] = DBNull.Value;
					dataSourceTable.Rows[dataSourceRowIndex][gcRunTime.FieldName] = DBNull.Value;
					dataSourceTable.Rows[dataSourceRowIndex][gcTransTime.FieldName] = DBNull.Value;
					dataSourceTable.Rows[dataSourceRowIndex][gcBaseQty.FieldName] = DBNull.Value;
					dataSourceTable.Rows[dataSourceRowIndex][gcUoM.FieldName] = DBNull.Value;
					dataSourceTable.Rows[dataSourceRowIndex][gcBaseQty.FieldName] = DBNull.Value;
					dataSourceTable.Rows[dataSourceRowIndex][gcLineInvQty.FieldName] = DBNull.Value;
					dataSourceTable.Rows[dataSourceRowIndex][gcLineNumPerMsr.FieldName] = DBNull.Value;
					dataSourceTable.Rows[dataSourceRowIndex][gcLineUomName.FieldName] = DBNull.Value;
					grdEffic.DataSourceTable.Rows.Clear();
					splitLine.PanelVisibility = SplitPanelVisibility.Panel1;
				}
				else
				{
					gv_Line.SetRowCellValue(e.RowHandle, gcLineProductCode, DBNull.Value);
					gv_Line.SetRowCellValue(e.RowHandle, gcSubRouting, DBNull.Value);
					dataSourceTable.Rows[dataSourceRowIndex][gcRunTime.FieldName] = 1;
					dataSourceTable.Rows[dataSourceRowIndex][gcBaseQty.FieldName] = 1;
					dataSourceTable.Rows[dataSourceRowIndex][gcLineInvQty.FieldName] = 1;
					dataSourceTable.Rows[dataSourceRowIndex][gcRunUnit.FieldName] = "M";
					dataSourceTable.Rows[dataSourceRowIndex][gcSetupUnit.FieldName] = "M";
					dataSourceTable.Rows[dataSourceRowIndex][gcUomTransTIme.FieldName] = "M";
					dataSourceTable.Rows[dataSourceRowIndex][gcLineNumPerMsr.FieldName] = 1;
					dataSourceTable.Rows[dataSourceRowIndex][gcLineUomName.FieldName] = Function.ToString(gvDoc.GetFocusedRowCellValue("InvUnitMsr"));
					if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpCode")) == -1)
					{
						dataSourceTable.Rows[dataSourceRowIndex]["UomCode"] = "Manual";
					}
					else
					{
						dataSourceTable.Rows[dataSourceRowIndex]["UomCode"] = WinFuntions.GetUoMCodeByName(Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpCode")), Function.ToString(gvDoc.GetFocusedRowCellValue("InvUnitMsr")));
					}
				}
			}
			else if (e.Column.FieldName.Equals(gcBaseQty.FieldName))
			{
				dataSourceTable.Rows[dataSourceRowIndex][gcLineInvQty.FieldName] = Function.ParseDouble(e.Value) * Function.ParseDouble(gv_Line.GetRowCellValue(e.RowHandle, gcLineNumPerMsr));
			}
			grdDoc.DataSourceTable.AcceptChanges();
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

	private void gv_Line_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			LoadDataResourceEffic(e.FocusedRowHandle);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rSubRouting_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (!e.Button.IsLeft)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListAdvanceRouting, false, Function.ToString(gv_Line.GetFocusedRowCellValue(gcSubRouting)), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ProductCode = Function.ToString(gv_Line.GetFocusedRowCellValue(gcLineProductCode));
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gv_Line.SetFocusedRowCellValue(gcSubRouting, frm_CFL2.ChooseItem["RoutCode"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnImportExcel_Click(object sender, EventArgs e)
	{
		try
		{
			new frm_ImportRouting().ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rUomCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListUomCode, false, "", "", Enums.ItemObjectType.NoItemType, null, "")
			{
				UomGroup = Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpCode"))
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gv_Line.SetFocusedRowCellValue(gcUoM, frm_CFL2.ChooseItem["UomCode"]);
				grd_Line.DataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gvEffic_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals(gcEfficResCode.FieldName))
			{
				if (string.IsNullOrEmpty(Function.ToString(e.Value)))
				{
					gvEffic.SetRowCellValue(e.RowHandle, gcEfficResName, "");
					return;
				}
				if (grdEffic.DataSourceTable.AsEnumerable().Count((DataRow t) => Function.ToString(t["ResCode"]).Equals(Function.ToString(e.Value))) > 1)
				{
					WinCommons.Warning(Enums.MessageCode.CannotDuplicateResource, "");
					gvEffic.SetRowCellValue(e.RowHandle, gcEfficResName, "");
					return;
				}
				DataTable dataTable = await _httpClient.ResourceWorkCenter.GetDataWorkCenterByCode(Function.ToString(e.Value));
				if (dataTable.IsNotNull())
				{
					gvEffic.SetRowCellValue(e.RowHandle, gcEfficResName, dataTable.Rows[0]["ResName"]);
					gvEffic.SetRowCellValue(e.RowHandle, gcEfficEfficiency, 100);
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.ResourceDoesNotExists, "");
					gvEffic.SetRowCellValue(e.RowHandle, gcEfficResName, "");
				}
			}
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addNewLineToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grdEffic.DataSourceTable;
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["RoutCode"] = gvDoc.GetFocusedRowCellValue(gcRoutNo);
			dataRow["FatherLine"] = gv_Line.GetFocusedRowCellValue("LineNum");
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
			grdEffic.DataSource = dataSourceTable;
			if (FormMode != Enums.FormViewModeType.Add)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeRowsToolStripMenuItem_Click(object sender, EventArgs e)
	{
		gvEffic.DeleteDataSourceRowSelected();
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void frm_AdvanceRouting_Resize(object sender, EventArgs e)
	{
		try
		{
			if (splitLine.PanelVisibility == SplitPanelVisibility.Both)
			{
				splitLine.SplitterPosition = splitLine.Width - 300;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.RoutingAndResource.frm_AdvanceRouting));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions8 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions9 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions10 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions11 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions12 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions13 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions13 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions14 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions15 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions14 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
		this.txt_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.txt_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.txt_Status = new Apzon.Controls.Editor.LookUpEditEx();
		this.txt_RoutNo = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.lbToDate = new Apzon.Controls.Controls.LabelLine();
		this.lbFromDate = new Apzon.Controls.Controls.LabelLine();
		this.lbStatus = new Apzon.Controls.Controls.LabelLine();
		this.lbRourCode = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewDocToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcRoutNo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRoutName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcProdCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rProdCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcProdName = new Apzon.Controls.Grid.GridColumnEx();
		this.rProdName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcIsBase = new Apzon.Controls.Grid.GridColumnEx();
		this.rLookIsBase = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcFromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.rlookupRoutNo = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.splitLine = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Line = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewLineToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gcWorkOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_WorkOrder = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcOpeCode = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_OpeCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcOpeName = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_OpeName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcResCode = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_ResCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcResName = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_ResName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcSetupTime = new Apzon.Controls.Grid.GridColumnEx();
		this.cal_Edit = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcSetupUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_SetUnit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcRunTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRunUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.lookup_RunUnit = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcTransTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUomTransTIme = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBaseQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.rUomCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLineUomName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineNumPerMsr = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineInvQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineProductCode = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_ProdCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcLineProductName = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_ProdName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcSetupTask = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_SetupTask = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcRunTask = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_RunTask = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcSubRouting = new Apzon.Controls.Grid.GridColumnEx();
		this.rSubRouting = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcVisOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_UoM = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.rTime = new Apzon.Controls.Controls.RepositoryItemHourEdit();
		this.grdEffic = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuEffic = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewLineToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvEffic = new Apzon.Controls.Grid.GridViewEx();
		this.gcEfficResCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rEfficResCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcEfficResName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEfficEfficiency = new Apzon.Controls.Grid.GridColumnEx();
		this.rEfficCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.btnSave = new DevExpress.XtraEditors.SimpleButton();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.btnImportExcel = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_RoutNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rProdCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rProdName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLookIsBase).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rlookupRoutNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitLine).BeginInit();
		this.splitLine.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		this.contextMenuStrip_Line.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_WorkOrder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_OpeCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_OpeName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ResCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ResName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Edit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_SetUnit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_RunUnit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUomCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ProdCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ProdName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_SetupTask).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_RunTask).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rSubRouting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_UoM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rTime).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdEffic).BeginInit();
		this.contextMenuEffic.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvEffic).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rEfficResCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rEfficCal).BeginInit();
		base.SuspendLayout();
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.Location = new System.Drawing.Point(493, 28);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.Size = new System.Drawing.Size(75, 23);
		this.btnSearch.TabIndex = 17;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.txt_ToDate.EditValue = null;
		this.txt_ToDate.FieldName = null;
		this.txt_ToDate.Location = new System.Drawing.Point(335, 31);
		this.txt_ToDate.Name = "txt_ToDate";
		this.txt_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_ToDate.Properties.IsSearchControl = false;
		this.txt_ToDate.Size = new System.Drawing.Size(150, 20);
		this.txt_ToDate.TabIndex = 16;
		this.txt_FromDate.EditValue = null;
		this.txt_FromDate.FieldName = null;
		this.txt_FromDate.Location = new System.Drawing.Point(91, 31);
		this.txt_FromDate.Name = "txt_FromDate";
		this.txt_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_FromDate.Properties.IsSearchControl = false;
		this.txt_FromDate.Size = new System.Drawing.Size(150, 20);
		this.txt_FromDate.TabIndex = 15;
		this.txt_Status.FieldName = null;
		this.txt_Status.IsSearchControl = false;
		this.txt_Status.Location = new System.Drawing.Point(335, 5);
		this.txt_Status.Name = "txt_Status";
		this.txt_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txt_Status.Properties.DisplayMember = "Name";
		this.txt_Status.Properties.NullText = "";
		this.txt_Status.Properties.ValueMember = "Code";
		this.txt_Status.Size = new System.Drawing.Size(150, 20);
		this.txt_Status.TabIndex = 14;
		this.txt_RoutNo.FieldName = null;
		this.txt_RoutNo.IsSearchControl = false;
		this.txt_RoutNo.Location = new System.Drawing.Point(91, 5);
		this.txt_RoutNo.Name = "txt_RoutNo";
		this.txt_RoutNo.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.txt_RoutNo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions)
		});
		this.txt_RoutNo.Properties.ContainerControl = null;
		this.txt_RoutNo.Properties.DisplayMember = "";
		this.txt_RoutNo.Properties.DisplayMember2 = "";
		this.txt_RoutNo.Properties.LoadDataSourceWhenEnter = false;
		this.txt_RoutNo.Properties.SeparateCharacterForMulti = ';';
		this.txt_RoutNo.Properties.SeparateString = ", ";
		this.txt_RoutNo.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListAdvanceRouting;
		this.txt_RoutNo.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oRouting;
		this.txt_RoutNo.Properties.UsingAutoCompleteBox = false;
		this.txt_RoutNo.Properties.UsingDataSource = true;
		this.txt_RoutNo.Properties.ValidateWhenNoDataSource = false;
		this.txt_RoutNo.Properties.ValueMember = "RoutCode";
		this.txt_RoutNo.Size = new System.Drawing.Size(150, 20);
		this.txt_RoutNo.TabIndex = 13;
		this.lbToDate.FieldName = null;
		this.lbToDate.HideWhenEmpty = false;
		this.lbToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbToDate.Location = new System.Drawing.Point(253, 34);
		this.lbToDate.Name = "lbToDate";
		this.lbToDate.ShowUnderLine = true;
		this.lbToDate.Size = new System.Drawing.Size(80, 20);
		this.lbToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbToDate.TabIndex = 9;
		this.lbToDate.Utext = "To Date";
		this.lbFromDate.FieldName = null;
		this.lbFromDate.HideWhenEmpty = false;
		this.lbFromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFromDate.Location = new System.Drawing.Point(9, 34);
		this.lbFromDate.Name = "lbFromDate";
		this.lbFromDate.ShowUnderLine = true;
		this.lbFromDate.Size = new System.Drawing.Size(80, 20);
		this.lbFromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFromDate.TabIndex = 10;
		this.lbFromDate.Utext = "From Date";
		this.lbStatus.FieldName = null;
		this.lbStatus.HideWhenEmpty = false;
		this.lbStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbStatus.Location = new System.Drawing.Point(253, 8);
		this.lbStatus.Name = "lbStatus";
		this.lbStatus.ShowUnderLine = true;
		this.lbStatus.Size = new System.Drawing.Size(80, 20);
		this.lbStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbStatus.TabIndex = 11;
		this.lbStatus.Utext = "Status";
		this.lbRourCode.FieldName = null;
		this.lbRourCode.HideWhenEmpty = false;
		this.lbRourCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbRourCode.Location = new System.Drawing.Point(9, 8);
		this.lbRourCode.Name = "lbRourCode";
		this.lbRourCode.ShowUnderLine = true;
		this.lbRourCode.Size = new System.Drawing.Size(80, 20);
		this.lbRourCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbRourCode.TabIndex = 12;
		this.lbRourCode.Utext = "Route No.";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 60);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdDoc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.splitLine);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(1532, 534);
		this.splitContainerControl1.SplitterPosition = 146;
		this.splitContainerControl1.TabIndex = 18;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip_Doc;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rlookupRoutNo, this.rProdCode, this.rProdName, this.rStatus, this.rDate, this.rLookIsBase });
		this.grdDoc.Size = new System.Drawing.Size(1532, 146);
		this.grdDoc.TabIndex = 1;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addNewDocToolStripMenuItem, this.deleteToolStripMenuItem1, this.duplicateToolStripMenuItem });
		this.contextMenuStrip_Doc.Name = "contextMenuStrip_Doc";
		this.contextMenuStrip_Doc.Size = new System.Drawing.Size(148, 70);
		this.addNewDocToolStripMenuItem.Name = "addNewDocToolStripMenuItem";
		this.addNewDocToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
		this.addNewDocToolStripMenuItem.Text = "Add New Doc";
		this.addNewDocToolStripMenuItem.Click += new System.EventHandler(this.addNewDocToolStripMenuItem_Click);
		this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
		this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(147, 22);
		this.deleteToolStripMenuItem1.Text = "Delete";
		this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.duplicateToolStripMenuItem_Click);
		this.gvDoc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clRouting;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gcRoutNo, this.gcRoutName, this.gcProdCode, this.gcProdName, this.gcStatus, this.gcIsBase, this.gcFromDate, this.gcToDate, this.gcRemark });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oRouting";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = true;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.ItemId = "APZ_ORTA";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsView.BestFitMaxRowCount = 100;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvDoc_BeforeLeaveRow);
		this.gcRoutNo.Caption = "Route No";
		this.gcRoutNo.CurrencyDateField = null;
		this.gcRoutNo.CurrencyField = null;
		this.gcRoutNo.CurrencyFixed = null;
		this.gcRoutNo.CurrencyFixedRate = 0.0;
		this.gcRoutNo.CurrencyRateField = null;
		this.gcRoutNo.FieldName = "RoutCode";
		this.gcRoutNo.Name = "gcRoutNo";
		this.gcRoutNo.PrimaryKey = false;
		this.gcRoutNo.UnitField = null;
		this.gcRoutNo.Visible = true;
		this.gcRoutNo.VisibleIndex = 0;
		this.gcRoutName.Caption = "Route Name";
		this.gcRoutName.CurrencyDateField = null;
		this.gcRoutName.CurrencyField = null;
		this.gcRoutName.CurrencyFixed = null;
		this.gcRoutName.CurrencyFixedRate = 0.0;
		this.gcRoutName.CurrencyRateField = null;
		this.gcRoutName.FieldName = "RoutName";
		this.gcRoutName.Name = "gcRoutName";
		this.gcRoutName.PrimaryKey = false;
		this.gcRoutName.UnitField = null;
		this.gcRoutName.Visible = true;
		this.gcRoutName.VisibleIndex = 1;
		this.gcRoutName.Width = 123;
		this.gcProdCode.Caption = "Product Code";
		this.gcProdCode.ColumnEdit = this.rProdCode;
		this.gcProdCode.CurrencyDateField = null;
		this.gcProdCode.CurrencyField = null;
		this.gcProdCode.CurrencyFixed = null;
		this.gcProdCode.CurrencyFixedRate = 0.0;
		this.gcProdCode.CurrencyRateField = null;
		this.gcProdCode.FieldName = "ProdCode";
		this.gcProdCode.Name = "gcProdCode";
		this.gcProdCode.PrimaryKey = false;
		this.gcProdCode.UnitField = null;
		this.gcProdCode.Visible = true;
		this.gcProdCode.VisibleIndex = 2;
		this.gcProdCode.Width = 94;
		this.rProdCode.AutoHeight = false;
		this.rProdCode.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.rProdCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2)
		});
		this.rProdCode.ContainerControl = this;
		this.rProdCode.DisplayMember = "";
		this.rProdCode.DisplayMember2 = "";
		this.rProdCode.LoadDataSourceWhenEnter = false;
		this.rProdCode.Name = "rProdCode";
		this.rProdCode.SeparateCharacterForMulti = ';';
		this.rProdCode.SeparateString = ", ";
		this.rProdCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.BillOfMaterForProduction;
		this.rProdCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oProductTrees;
		this.rProdCode.UsingAutoCompleteBox = false;
		this.rProdCode.UsingDataSource = true;
		this.rProdCode.ValidateWhenNoDataSource = false;
		this.rProdCode.ValueMember = "Code";
		this.gcProdName.Caption = "Product Name";
		this.gcProdName.ColumnEdit = this.rProdName;
		this.gcProdName.CurrencyDateField = null;
		this.gcProdName.CurrencyField = null;
		this.gcProdName.CurrencyFixed = null;
		this.gcProdName.CurrencyFixedRate = 0.0;
		this.gcProdName.CurrencyRateField = null;
		this.gcProdName.FieldName = "ProdName";
		this.gcProdName.Name = "gcProdName";
		this.gcProdName.PrimaryKey = false;
		this.gcProdName.UnitField = null;
		this.gcProdName.Visible = true;
		this.gcProdName.VisibleIndex = 3;
		this.gcProdName.Width = 113;
		this.rProdName.AutoHeight = false;
		this.rProdName.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.rProdName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3)
		});
		this.rProdName.ContainerControl = this;
		this.rProdName.DisplayMember = "";
		this.rProdName.DisplayMember2 = "";
		this.rProdName.LoadDataSourceWhenEnter = false;
		this.rProdName.Name = "rProdName";
		this.rProdName.SeparateCharacterForMulti = ';';
		this.rProdName.SeparateString = ", ";
		this.rProdName.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.BillOfMaterForProduction;
		this.rProdName.UsingAutoCompleteBox = false;
		this.rProdName.UsingDataSource = true;
		this.rProdName.ValidateWhenNoDataSource = false;
		this.rProdName.ValueMember = "ItemName";
		this.gcStatus.Caption = "Status";
		this.gcStatus.ColumnEdit = this.rStatus;
		this.gcStatus.CurrencyDateField = null;
		this.gcStatus.CurrencyField = null;
		this.gcStatus.CurrencyFixed = null;
		this.gcStatus.CurrencyFixedRate = 0.0;
		this.gcStatus.CurrencyRateField = null;
		this.gcStatus.FieldName = "Status";
		this.gcStatus.Name = "gcStatus";
		this.gcStatus.PrimaryKey = false;
		this.gcStatus.UnitField = null;
		this.gcStatus.Visible = true;
		this.gcStatus.VisibleIndex = 4;
		this.rStatus.AutoHeight = false;
		this.rStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default)
		});
		this.rStatus.DisplayMember = "Name";
		this.rStatus.Name = "rStatus";
		this.rStatus.NullText = "";
		this.rStatus.ShowHeader = false;
		this.rStatus.ValueMember = "Code";
		this.gcIsBase.Caption = "Is Base";
		this.gcIsBase.ColumnEdit = this.rLookIsBase;
		this.gcIsBase.CurrencyDateField = null;
		this.gcIsBase.CurrencyField = null;
		this.gcIsBase.CurrencyFixed = null;
		this.gcIsBase.CurrencyFixedRate = 0.0;
		this.gcIsBase.CurrencyRateField = null;
		this.gcIsBase.FieldName = "IsBase";
		this.gcIsBase.Name = "gcIsBase";
		this.gcIsBase.PrimaryKey = false;
		this.gcIsBase.UnitField = null;
		this.rLookIsBase.AutoHeight = false;
		this.rLookIsBase.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLookIsBase.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.rLookIsBase.DisplayMember = "Name";
		this.rLookIsBase.Name = "rLookIsBase";
		this.rLookIsBase.NullText = "";
		this.rLookIsBase.ValueMember = "Code";
		this.gcFromDate.Caption = "From Date";
		this.gcFromDate.ColumnEdit = this.rDate;
		this.gcFromDate.CurrencyDateField = null;
		this.gcFromDate.CurrencyField = null;
		this.gcFromDate.CurrencyFixed = null;
		this.gcFromDate.CurrencyFixedRate = 0.0;
		this.gcFromDate.CurrencyRateField = null;
		this.gcFromDate.DisplayFormat.FormatString = "d";
		this.gcFromDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcFromDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcFromDate.FieldName = "FromDate";
		this.gcFromDate.Name = "gcFromDate";
		this.gcFromDate.PrimaryKey = false;
		this.gcFromDate.UnitField = null;
		this.gcFromDate.Visible = true;
		this.gcFromDate.VisibleIndex = 5;
		this.gcFromDate.Width = 111;
		this.rDate.AutoHeight = false;
		this.rDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.Name = "rDate";
		this.gcToDate.Caption = "To Date";
		this.gcToDate.ColumnEdit = this.rDate;
		this.gcToDate.CurrencyDateField = null;
		this.gcToDate.CurrencyField = null;
		this.gcToDate.CurrencyFixed = null;
		this.gcToDate.CurrencyFixedRate = 0.0;
		this.gcToDate.CurrencyRateField = null;
		this.gcToDate.DisplayFormat.FormatString = "d";
		this.gcToDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.gcToDate.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcToDate.FieldName = "ToDate";
		this.gcToDate.Name = "gcToDate";
		this.gcToDate.PrimaryKey = false;
		this.gcToDate.UnitField = null;
		this.gcToDate.Visible = true;
		this.gcToDate.VisibleIndex = 6;
		this.gcToDate.Width = 98;
		this.gcRemark.Caption = "Remark";
		this.gcRemark.CurrencyDateField = null;
		this.gcRemark.CurrencyField = null;
		this.gcRemark.CurrencyFixed = null;
		this.gcRemark.CurrencyFixedRate = 0.0;
		this.gcRemark.CurrencyRateField = null;
		this.gcRemark.FieldName = "Remarks";
		this.gcRemark.Name = "gcRemark";
		this.gcRemark.PrimaryKey = false;
		this.gcRemark.UnitField = null;
		this.gcRemark.Visible = true;
		this.gcRemark.VisibleIndex = 7;
		this.gcRemark.Width = 95;
		this.rlookupRoutNo.AutoHeight = false;
		this.rlookupRoutNo.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions8.Image");
		this.rlookupRoutNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions4)
		});
		this.rlookupRoutNo.ContainerControl = this;
		this.rlookupRoutNo.DisplayMember = "";
		this.rlookupRoutNo.DisplayMember2 = "";
		this.rlookupRoutNo.LoadDataSourceWhenEnter = false;
		this.rlookupRoutNo.Name = "rlookupRoutNo";
		this.rlookupRoutNo.SeparateCharacterForMulti = ';';
		this.rlookupRoutNo.SeparateString = ", ";
		this.rlookupRoutNo.UsingAutoCompleteBox = false;
		this.rlookupRoutNo.UsingDataSource = true;
		this.rlookupRoutNo.ValidateWhenNoDataSource = false;
		this.rlookupRoutNo.ValueMember = "";
		this.splitLine.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitLine.Location = new System.Drawing.Point(0, 0);
		this.splitLine.Name = "splitLine";
		this.splitLine.Panel1.Controls.Add(this.grd_Line);
		this.splitLine.Panel1.Text = "Panel1";
		this.splitLine.Panel2.Controls.Add(this.grdEffic);
		this.splitLine.Panel2.Text = "Panel2";
		this.splitLine.Size = new System.Drawing.Size(1532, 383);
		this.splitLine.SplitterPosition = 1236;
		this.splitLine.TabIndex = 2;
		this.splitLine.Text = "splitContainerControl2";
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.ContextMenuStrip = this.contextMenuStrip_Line;
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[16]
		{
			this.bte_OpeCode, this.bte_OpeName, this.bte_ResCode, this.bte_ResName, this.cal_Edit, this.lookup_SetUnit, this.lookup_RunUnit, this.bte_UoM, this.lookup_WorkOrder, this.bte_ProdCode,
			this.bte_ProdName, this.bte_SetupTask, this.bte_RunTask, this.rTime, this.rSubRouting, this.rUomCode
		});
		this.grd_Line.Size = new System.Drawing.Size(1236, 383);
		this.grd_Line.TabIndex = 1;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		this.contextMenuStrip_Line.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewLineToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuStrip_Line.Name = "contextMenuStrip_Line";
		this.contextMenuStrip_Line.Size = new System.Drawing.Size(149, 48);
		this.addNewLineToolStripMenuItem.Name = "addNewLineToolStripMenuItem";
		this.addNewLineToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.addNewLineToolStripMenuItem.Text = "Add New Line";
		this.addNewLineToolStripMenuItem.Click += new System.EventHandler(this.addNewLineToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gv_Line.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Line.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Line.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clRouting;
		this.gv_Line.ColumnKey = null;
		this.gv_Line.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[22]
		{
			this.gcWorkOrder, this.gcOpeCode, this.gcOpeName, this.gcResCode, this.gcResName, this.gcSetupTime, this.gcSetupUnit, this.gcRunTime, this.gcRunUnit, this.gcTransTime,
			this.gcUomTransTIme, this.gcBaseQty, this.gcUoM, this.gcLineUomName, this.gcLineNumPerMsr, this.gcLineInvQty, this.gcLineProductCode, this.gcLineProductName, this.gcSetupTask, this.gcRunTask,
			this.gcSubRouting, this.gcVisOrder
		});
		this.gv_Line.DisableFormSetting = false;
		this.gv_Line.DisplayException = false;
		this.gv_Line.EditColumn = null;
		this.gv_Line.FieldCurrency = null;
		this.gv_Line.FieldRate = null;
		this.gv_Line.FormId = "oRouting";
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IndexEditRow = -1;
		this.gv_Line.IndicatorWidth = 35;
		this.gv_Line.IsApzTable = true;
		this.gv_Line.IsInsertNewRow = false;
		this.gv_Line.IsLoadingData = false;
		this.gv_Line.ItemId = "APZ_RTA1";
		this.gv_Line.LstColumnAllowBatch = null;
		this.gv_Line.LstColumnNotAllowBatch = null;
		this.gv_Line.LstColumNotSetting = null;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Line.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Line.OptionsView.ColumnAutoWidth = false;
		this.gv_Line.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gv_Line.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gcVisOrder, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Line.UseBatchMod = false;
		this.gv_Line.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Line_ShowingEditor);
		this.gv_Line.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Line_FocusedRowObjectChanged);
		this.gv_Line.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanged);
		this.gcWorkOrder.Caption = "Work Order";
		this.gcWorkOrder.ColumnEdit = this.lookup_WorkOrder;
		this.gcWorkOrder.CurrencyDateField = null;
		this.gcWorkOrder.CurrencyField = null;
		this.gcWorkOrder.CurrencyFixed = null;
		this.gcWorkOrder.CurrencyFixedRate = 0.0;
		this.gcWorkOrder.CurrencyRateField = null;
		this.gcWorkOrder.FieldName = "WorkOrder";
		this.gcWorkOrder.Name = "gcWorkOrder";
		this.gcWorkOrder.PrimaryKey = false;
		this.gcWorkOrder.UnitField = null;
		this.gcWorkOrder.Visible = true;
		this.gcWorkOrder.VisibleIndex = 0;
		this.lookup_WorkOrder.AutoHeight = false;
		this.lookup_WorkOrder.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_WorkOrder.Name = "lookup_WorkOrder";
		this.lookup_WorkOrder.NullText = "";
		this.gcOpeCode.Caption = "Operation Code";
		this.gcOpeCode.ColumnEdit = this.bte_OpeCode;
		this.gcOpeCode.CurrencyDateField = null;
		this.gcOpeCode.CurrencyField = null;
		this.gcOpeCode.CurrencyFixed = null;
		this.gcOpeCode.CurrencyFixedRate = 0.0;
		this.gcOpeCode.CurrencyRateField = null;
		this.gcOpeCode.FieldName = "OpCode";
		this.gcOpeCode.Name = "gcOpeCode";
		this.gcOpeCode.PrimaryKey = false;
		this.gcOpeCode.UnitField = null;
		this.gcOpeCode.Visible = true;
		this.gcOpeCode.VisibleIndex = 1;
		this.bte_OpeCode.AutoHeight = false;
		this.bte_OpeCode.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions10.Image");
		this.bte_OpeCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions5)
		});
		this.bte_OpeCode.ContainerControl = this;
		this.bte_OpeCode.DisplayMember = "";
		this.bte_OpeCode.DisplayMember2 = "";
		this.bte_OpeCode.LoadDataSourceWhenEnter = false;
		this.bte_OpeCode.Name = "bte_OpeCode";
		this.bte_OpeCode.SeparateCharacterForMulti = ';';
		this.bte_OpeCode.SeparateString = ", ";
		this.bte_OpeCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListOperation;
		this.bte_OpeCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oOperation;
		this.bte_OpeCode.UsingAutoCompleteBox = false;
		this.bte_OpeCode.UsingDataSource = true;
		this.bte_OpeCode.ValidateWhenNoDataSource = false;
		this.bte_OpeCode.ValueMember = "OpeCode";
		this.gcOpeName.Caption = "Operation Name";
		this.gcOpeName.ColumnEdit = this.bte_OpeName;
		this.gcOpeName.CurrencyDateField = null;
		this.gcOpeName.CurrencyField = null;
		this.gcOpeName.CurrencyFixed = null;
		this.gcOpeName.CurrencyFixedRate = 0.0;
		this.gcOpeName.CurrencyRateField = null;
		this.gcOpeName.FieldName = "OpName";
		this.gcOpeName.Name = "gcOpeName";
		this.gcOpeName.PrimaryKey = false;
		this.gcOpeName.UnitField = null;
		this.gcOpeName.Visible = true;
		this.gcOpeName.VisibleIndex = 2;
		this.bte_OpeName.AutoHeight = false;
		this.bte_OpeName.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions12.Image");
		this.bte_OpeName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6)
		});
		this.bte_OpeName.ContainerControl = this;
		this.bte_OpeName.DisplayMember = "";
		this.bte_OpeName.DisplayMember2 = "";
		this.bte_OpeName.LoadDataSourceWhenEnter = false;
		this.bte_OpeName.Name = "bte_OpeName";
		this.bte_OpeName.SeparateCharacterForMulti = ';';
		this.bte_OpeName.SeparateString = ", ";
		this.bte_OpeName.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListOperation;
		this.bte_OpeName.UsingAutoCompleteBox = false;
		this.bte_OpeName.UsingDataSource = true;
		this.bte_OpeName.ValidateWhenNoDataSource = false;
		this.bte_OpeName.ValueMember = "OpeName";
		this.gcResCode.Caption = "Resource Code";
		this.gcResCode.ColumnEdit = this.bte_ResCode;
		this.gcResCode.CurrencyDateField = null;
		this.gcResCode.CurrencyField = null;
		this.gcResCode.CurrencyFixed = null;
		this.gcResCode.CurrencyFixedRate = 0.0;
		this.gcResCode.CurrencyRateField = null;
		this.gcResCode.FieldName = "ResCode";
		this.gcResCode.Name = "gcResCode";
		this.gcResCode.PrimaryKey = false;
		this.gcResCode.UnitField = null;
		this.gcResCode.Visible = true;
		this.gcResCode.VisibleIndex = 3;
		this.bte_ResCode.AutoHeight = false;
		this.bte_ResCode.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions14.Image");
		this.bte_ResCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions7)
		});
		this.bte_ResCode.ContainerControl = this;
		this.bte_ResCode.DisplayMember = "";
		this.bte_ResCode.DisplayMember2 = "";
		this.bte_ResCode.LoadDataSourceWhenEnter = false;
		this.bte_ResCode.Name = "bte_ResCode";
		this.bte_ResCode.SeparateCharacterForMulti = ';';
		this.bte_ResCode.SeparateString = ", ";
		this.bte_ResCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListWorkCenter;
		this.bte_ResCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.WorkCenter;
		this.bte_ResCode.UsingAutoCompleteBox = false;
		this.bte_ResCode.UsingDataSource = true;
		this.bte_ResCode.ValidateWhenNoDataSource = false;
		this.bte_ResCode.ValueMember = "ResCode";
		this.gcResName.Caption = "Resource Name";
		this.gcResName.ColumnEdit = this.bte_ResName;
		this.gcResName.CurrencyDateField = null;
		this.gcResName.CurrencyField = null;
		this.gcResName.CurrencyFixed = null;
		this.gcResName.CurrencyFixedRate = 0.0;
		this.gcResName.CurrencyRateField = null;
		this.gcResName.FieldName = "ResName";
		this.gcResName.Name = "gcResName";
		this.gcResName.PrimaryKey = false;
		this.gcResName.UnitField = null;
		this.gcResName.Visible = true;
		this.gcResName.VisibleIndex = 4;
		this.bte_ResName.AutoHeight = false;
		this.bte_ResName.AutoLoadDataSource = true;
		editorButtonImageOptions8.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions16.Image");
		this.bte_ResName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions8, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions8)
		});
		this.bte_ResName.ContainerControl = this;
		this.bte_ResName.DisplayMember = "";
		this.bte_ResName.DisplayMember2 = "";
		this.bte_ResName.LoadDataSourceWhenEnter = false;
		this.bte_ResName.Name = "bte_ResName";
		this.bte_ResName.SeparateCharacterForMulti = ';';
		this.bte_ResName.SeparateString = ", ";
		this.bte_ResName.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListWorkCenter;
		this.bte_ResName.UsingAutoCompleteBox = false;
		this.bte_ResName.UsingDataSource = true;
		this.bte_ResName.ValidateWhenNoDataSource = false;
		this.bte_ResName.ValueMember = "ResName";
		this.gcSetupTime.Caption = "Setup Time";
		this.gcSetupTime.ColumnEdit = this.cal_Edit;
		this.gcSetupTime.CurrencyDateField = null;
		this.gcSetupTime.CurrencyField = null;
		this.gcSetupTime.CurrencyFixed = null;
		this.gcSetupTime.CurrencyFixedRate = 0.0;
		this.gcSetupTime.CurrencyRateField = null;
		this.gcSetupTime.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcSetupTime.FieldName = "SetTime";
		this.gcSetupTime.Name = "gcSetupTime";
		this.gcSetupTime.PrimaryKey = false;
		this.gcSetupTime.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcSetupTime.UnitField = null;
		this.gcSetupTime.Visible = true;
		this.gcSetupTime.VisibleIndex = 5;
		this.cal_Edit.AutoHeight = false;
		this.cal_Edit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cal_Edit.Name = "cal_Edit";
		this.gcSetupUnit.Caption = "Setup Unit";
		this.gcSetupUnit.ColumnEdit = this.lookup_SetUnit;
		this.gcSetupUnit.CurrencyDateField = null;
		this.gcSetupUnit.CurrencyField = null;
		this.gcSetupUnit.CurrencyFixed = null;
		this.gcSetupUnit.CurrencyFixedRate = 0.0;
		this.gcSetupUnit.CurrencyRateField = null;
		this.gcSetupUnit.FieldName = "UomSetTime";
		this.gcSetupUnit.Name = "gcSetupUnit";
		this.gcSetupUnit.PrimaryKey = false;
		this.gcSetupUnit.UnitField = null;
		this.gcSetupUnit.Visible = true;
		this.gcSetupUnit.VisibleIndex = 6;
		this.lookup_SetUnit.AutoHeight = false;
		this.lookup_SetUnit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_SetUnit.Name = "lookup_SetUnit";
		this.lookup_SetUnit.NullText = "";
		this.gcRunTime.Caption = "Run Time";
		this.gcRunTime.ColumnEdit = this.cal_Edit;
		this.gcRunTime.CurrencyDateField = null;
		this.gcRunTime.CurrencyField = null;
		this.gcRunTime.CurrencyFixed = null;
		this.gcRunTime.CurrencyFixedRate = 0.0;
		this.gcRunTime.CurrencyRateField = null;
		this.gcRunTime.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcRunTime.FieldName = "RunTime";
		this.gcRunTime.Name = "gcRunTime";
		this.gcRunTime.PrimaryKey = false;
		this.gcRunTime.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcRunTime.UnitField = null;
		this.gcRunTime.Visible = true;
		this.gcRunTime.VisibleIndex = 7;
		this.gcRunUnit.Caption = "Run Unit";
		this.gcRunUnit.ColumnEdit = this.lookup_RunUnit;
		this.gcRunUnit.CurrencyDateField = null;
		this.gcRunUnit.CurrencyField = null;
		this.gcRunUnit.CurrencyFixed = null;
		this.gcRunUnit.CurrencyFixedRate = 0.0;
		this.gcRunUnit.CurrencyRateField = null;
		this.gcRunUnit.FieldName = "UomRunTime";
		this.gcRunUnit.Name = "gcRunUnit";
		this.gcRunUnit.PrimaryKey = false;
		this.gcRunUnit.UnitField = null;
		this.gcRunUnit.Visible = true;
		this.gcRunUnit.VisibleIndex = 8;
		this.lookup_RunUnit.AutoHeight = false;
		this.lookup_RunUnit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lookup_RunUnit.Name = "lookup_RunUnit";
		this.lookup_RunUnit.NullText = "";
		this.gcTransTime.Caption = "Trans. Time";
		this.gcTransTime.ColumnEdit = this.cal_Edit;
		this.gcTransTime.CurrencyDateField = null;
		this.gcTransTime.CurrencyField = null;
		this.gcTransTime.CurrencyFixed = null;
		this.gcTransTime.CurrencyFixedRate = 0.0;
		this.gcTransTime.CurrencyRateField = null;
		this.gcTransTime.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcTransTime.FieldName = "TransTime";
		this.gcTransTime.Name = "gcTransTime";
		this.gcTransTime.PrimaryKey = false;
		this.gcTransTime.UnitField = null;
		this.gcTransTime.Visible = true;
		this.gcTransTime.VisibleIndex = 9;
		this.gcUomTransTIme.Caption = "Trans. Unit";
		this.gcUomTransTIme.ColumnEdit = this.lookup_SetUnit;
		this.gcUomTransTIme.CurrencyDateField = null;
		this.gcUomTransTIme.CurrencyField = null;
		this.gcUomTransTIme.CurrencyFixed = null;
		this.gcUomTransTIme.CurrencyFixedRate = 0.0;
		this.gcUomTransTIme.CurrencyRateField = null;
		this.gcUomTransTIme.FieldName = "UomTransTime";
		this.gcUomTransTIme.Name = "gcUomTransTIme";
		this.gcUomTransTIme.PrimaryKey = false;
		this.gcUomTransTIme.UnitField = null;
		this.gcUomTransTIme.Visible = true;
		this.gcUomTransTIme.VisibleIndex = 10;
		this.gcBaseQty.Caption = "Base Qty";
		this.gcBaseQty.ColumnEdit = this.cal_Edit;
		this.gcBaseQty.CurrencyDateField = null;
		this.gcBaseQty.CurrencyField = null;
		this.gcBaseQty.CurrencyFixed = null;
		this.gcBaseQty.CurrencyFixedRate = 0.0;
		this.gcBaseQty.CurrencyRateField = null;
		this.gcBaseQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcBaseQty.FieldName = "BaseQty";
		this.gcBaseQty.Name = "gcBaseQty";
		this.gcBaseQty.PrimaryKey = false;
		this.gcBaseQty.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcBaseQty.UnitField = null;
		this.gcBaseQty.Visible = true;
		this.gcBaseQty.VisibleIndex = 11;
		this.gcUoM.Caption = "UoM";
		this.gcUoM.ColumnEdit = this.rUomCode;
		this.gcUoM.CurrencyDateField = null;
		this.gcUoM.CurrencyField = null;
		this.gcUoM.CurrencyFixed = null;
		this.gcUoM.CurrencyFixedRate = 0.0;
		this.gcUoM.CurrencyRateField = null;
		this.gcUoM.FieldName = "UomCode";
		this.gcUoM.Name = "gcUoM";
		this.gcUoM.OptionsColumn.ReadOnly = true;
		this.gcUoM.PrimaryKey = false;
		this.gcUoM.UnitField = null;
		this.gcUoM.Visible = true;
		this.gcUoM.VisibleIndex = 12;
		this.rUomCode.AutoHeight = false;
		this.rUomCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rUomCode.Name = "rUomCode";
		this.rUomCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rUomCode_ButtonClick);
		this.gcLineUomName.Caption = "Uom Name";
		this.gcLineUomName.CurrencyDateField = null;
		this.gcLineUomName.CurrencyField = null;
		this.gcLineUomName.CurrencyFixed = null;
		this.gcLineUomName.CurrencyFixedRate = 0.0;
		this.gcLineUomName.CurrencyRateField = null;
		this.gcLineUomName.FieldName = "UnitMsr";
		this.gcLineUomName.Name = "gcLineUomName";
		this.gcLineUomName.OptionsColumn.ReadOnly = true;
		this.gcLineUomName.PrimaryKey = false;
		this.gcLineUomName.UnitField = null;
		this.gcLineUomName.Visible = true;
		this.gcLineUomName.VisibleIndex = 13;
		this.gcLineNumPerMsr.Caption = "Item per unit";
		this.gcLineNumPerMsr.ColumnEdit = this.cal_Edit;
		this.gcLineNumPerMsr.CurrencyDateField = null;
		this.gcLineNumPerMsr.CurrencyField = null;
		this.gcLineNumPerMsr.CurrencyFixed = null;
		this.gcLineNumPerMsr.CurrencyFixedRate = 0.0;
		this.gcLineNumPerMsr.CurrencyRateField = null;
		this.gcLineNumPerMsr.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Unit;
		this.gcLineNumPerMsr.FieldName = "NumPerMsr";
		this.gcLineNumPerMsr.Name = "gcLineNumPerMsr";
		this.gcLineNumPerMsr.OptionsColumn.ReadOnly = true;
		this.gcLineNumPerMsr.PrimaryKey = false;
		this.gcLineNumPerMsr.UnitField = null;
		this.gcLineNumPerMsr.Visible = true;
		this.gcLineNumPerMsr.VisibleIndex = 14;
		this.gcLineInvQty.Caption = "Inv Qty";
		this.gcLineInvQty.ColumnEdit = this.cal_Edit;
		this.gcLineInvQty.CurrencyDateField = null;
		this.gcLineInvQty.CurrencyField = null;
		this.gcLineInvQty.CurrencyFixed = null;
		this.gcLineInvQty.CurrencyFixedRate = 0.0;
		this.gcLineInvQty.CurrencyRateField = null;
		this.gcLineInvQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineInvQty.FieldName = "InvQty";
		this.gcLineInvQty.Name = "gcLineInvQty";
		this.gcLineInvQty.OptionsColumn.ReadOnly = true;
		this.gcLineInvQty.PrimaryKey = false;
		this.gcLineInvQty.UnitField = null;
		this.gcLineInvQty.Visible = true;
		this.gcLineInvQty.VisibleIndex = 15;
		this.gcLineProductCode.Caption = "Product Code";
		this.gcLineProductCode.ColumnEdit = this.bte_ProdCode;
		this.gcLineProductCode.CurrencyDateField = null;
		this.gcLineProductCode.CurrencyField = null;
		this.gcLineProductCode.CurrencyFixed = null;
		this.gcLineProductCode.CurrencyFixedRate = 0.0;
		this.gcLineProductCode.CurrencyRateField = null;
		this.gcLineProductCode.FieldName = "ProdCode";
		this.gcLineProductCode.Name = "gcLineProductCode";
		this.gcLineProductCode.PrimaryKey = false;
		this.gcLineProductCode.UnitField = null;
		this.gcLineProductCode.Visible = true;
		this.gcLineProductCode.VisibleIndex = 16;
		this.bte_ProdCode.AutoHeight = false;
		this.bte_ProdCode.AutoLoadDataSource = true;
		editorButtonImageOptions9.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions18.Image");
		this.bte_ProdCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions9, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions9)
		});
		this.bte_ProdCode.ContainerControl = this;
		this.bte_ProdCode.DisplayMember = "";
		this.bte_ProdCode.DisplayMember2 = "";
		this.bte_ProdCode.LoadDataSourceWhenEnter = false;
		this.bte_ProdCode.Name = "bte_ProdCode";
		this.bte_ProdCode.SeparateCharacterForMulti = ';';
		this.bte_ProdCode.SeparateString = ", ";
		this.bte_ProdCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListSemiOfProduc;
		this.bte_ProdCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oProductTrees;
		this.bte_ProdCode.UsingAutoCompleteBox = false;
		this.bte_ProdCode.UsingDataSource = true;
		this.bte_ProdCode.ValidateWhenNoDataSource = false;
		this.bte_ProdCode.ValueMember = "ItemCode";
		this.gcLineProductName.Caption = "Product Name";
		this.gcLineProductName.ColumnEdit = this.bte_ProdName;
		this.gcLineProductName.CurrencyDateField = null;
		this.gcLineProductName.CurrencyField = null;
		this.gcLineProductName.CurrencyFixed = null;
		this.gcLineProductName.CurrencyFixedRate = 0.0;
		this.gcLineProductName.CurrencyRateField = null;
		this.gcLineProductName.FieldName = "ProdName";
		this.gcLineProductName.Name = "gcLineProductName";
		this.gcLineProductName.PrimaryKey = false;
		this.gcLineProductName.UnitField = null;
		this.gcLineProductName.Visible = true;
		this.gcLineProductName.VisibleIndex = 17;
		this.bte_ProdName.AutoHeight = false;
		this.bte_ProdName.AutoLoadDataSource = true;
		editorButtonImageOptions10.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions20.Image");
		this.bte_ProdName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions10, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions10)
		});
		this.bte_ProdName.ContainerControl = this;
		this.bte_ProdName.DisplayMember = "";
		this.bte_ProdName.DisplayMember2 = "";
		this.bte_ProdName.LoadDataSourceWhenEnter = false;
		this.bte_ProdName.Name = "bte_ProdName";
		this.bte_ProdName.SeparateCharacterForMulti = ';';
		this.bte_ProdName.SeparateString = ", ";
		this.bte_ProdName.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListSemiOfProduc;
		this.bte_ProdName.UsingAutoCompleteBox = false;
		this.bte_ProdName.UsingDataSource = true;
		this.bte_ProdName.ValidateWhenNoDataSource = false;
		this.bte_ProdName.ValueMember = "ItemName";
		this.gcSetupTask.Caption = "Setup Task";
		this.gcSetupTask.ColumnEdit = this.bte_SetupTask;
		this.gcSetupTask.CurrencyDateField = null;
		this.gcSetupTask.CurrencyField = null;
		this.gcSetupTask.CurrencyFixed = null;
		this.gcSetupTask.CurrencyFixedRate = 0.0;
		this.gcSetupTask.CurrencyRateField = null;
		this.gcSetupTask.FieldName = "SetTask";
		this.gcSetupTask.Name = "gcSetupTask";
		this.gcSetupTask.PrimaryKey = false;
		this.gcSetupTask.UnitField = null;
		this.gcSetupTask.Visible = true;
		this.gcSetupTask.VisibleIndex = 18;
		this.bte_SetupTask.AutoHeight = false;
		this.bte_SetupTask.AutoLoadDataSource = true;
		editorButtonImageOptions11.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions22.Image");
		this.bte_SetupTask.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions11, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions11)
		});
		this.bte_SetupTask.ContainerControl = this;
		this.bte_SetupTask.DisplayMember = "";
		this.bte_SetupTask.DisplayMember2 = "";
		this.bte_SetupTask.LoadDataSourceWhenEnter = false;
		this.bte_SetupTask.Name = "bte_SetupTask";
		this.bte_SetupTask.SeparateCharacterForMulti = ';';
		this.bte_SetupTask.SeparateString = ", ";
		this.bte_SetupTask.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.GetTaskList;
		this.bte_SetupTask.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oTaskManagement;
		this.bte_SetupTask.UsingAutoCompleteBox = false;
		this.bte_SetupTask.UsingDataSource = true;
		this.bte_SetupTask.ValidateWhenNoDataSource = false;
		this.bte_SetupTask.ValueMember = "ClgCode";
		this.gcRunTask.Caption = "Run Task";
		this.gcRunTask.ColumnEdit = this.bte_RunTask;
		this.gcRunTask.CurrencyDateField = null;
		this.gcRunTask.CurrencyField = null;
		this.gcRunTask.CurrencyFixed = null;
		this.gcRunTask.CurrencyFixedRate = 0.0;
		this.gcRunTask.CurrencyRateField = null;
		this.gcRunTask.FieldName = "RunTask";
		this.gcRunTask.Name = "gcRunTask";
		this.gcRunTask.PrimaryKey = false;
		this.gcRunTask.UnitField = null;
		this.gcRunTask.Visible = true;
		this.gcRunTask.VisibleIndex = 19;
		this.bte_RunTask.AutoHeight = false;
		this.bte_RunTask.AutoLoadDataSource = true;
		editorButtonImageOptions12.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions24.Image");
		this.bte_RunTask.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions12, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions12)
		});
		this.bte_RunTask.ContainerControl = this;
		this.bte_RunTask.DisplayMember = "";
		this.bte_RunTask.DisplayMember2 = "";
		this.bte_RunTask.LoadDataSourceWhenEnter = false;
		this.bte_RunTask.Name = "bte_RunTask";
		this.bte_RunTask.SeparateCharacterForMulti = ';';
		this.bte_RunTask.SeparateString = ", ";
		this.bte_RunTask.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.GetTaskList;
		this.bte_RunTask.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oTaskManagement;
		this.bte_RunTask.UsingAutoCompleteBox = false;
		this.bte_RunTask.UsingDataSource = true;
		this.bte_RunTask.ValidateWhenNoDataSource = false;
		this.bte_RunTask.ValueMember = "ClgCode";
		this.gcSubRouting.Caption = "Sub Routing";
		this.gcSubRouting.ColumnEdit = this.rSubRouting;
		this.gcSubRouting.CurrencyDateField = null;
		this.gcSubRouting.CurrencyField = null;
		this.gcSubRouting.CurrencyFixed = null;
		this.gcSubRouting.CurrencyFixedRate = 0.0;
		this.gcSubRouting.CurrencyRateField = null;
		this.gcSubRouting.FieldName = "SubRoutCode";
		this.gcSubRouting.Name = "gcSubRouting";
		this.gcSubRouting.PrimaryKey = false;
		this.gcSubRouting.UnitField = null;
		this.gcSubRouting.Visible = true;
		this.gcSubRouting.VisibleIndex = 20;
		this.rSubRouting.AutoHeight = false;
		editorButtonImageOptions13.Image = Apzon.Forms.Properties.Resources.right;
		this.rSubRouting.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions13)
		});
		this.rSubRouting.Name = "rSubRouting";
		this.rSubRouting.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rSubRouting_ButtonClick);
		this.gcVisOrder.Caption = "Vis. Order";
		this.gcVisOrder.CurrencyDateField = null;
		this.gcVisOrder.CurrencyField = null;
		this.gcVisOrder.CurrencyFixed = null;
		this.gcVisOrder.CurrencyFixedRate = 0.0;
		this.gcVisOrder.CurrencyRateField = null;
		this.gcVisOrder.FieldName = "VisOrder";
		this.gcVisOrder.Name = "gcVisOrder";
		this.gcVisOrder.PrimaryKey = false;
		this.gcVisOrder.UnitField = null;
		this.gcVisOrder.Visible = true;
		this.gcVisOrder.VisibleIndex = 21;
		this.bte_UoM.AutoHeight = false;
		this.bte_UoM.AutoLoadDataSource = true;
		this.bte_UoM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions13, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, imageOptions14)
		});
		this.bte_UoM.ContainerControl = this;
		this.bte_UoM.DisplayMember = "UomName";
		this.bte_UoM.DisplayMember2 = "";
		this.bte_UoM.LoadDataSourceWhenEnter = false;
		this.bte_UoM.Name = "bte_UoM";
		this.bte_UoM.SeparateCharacterForMulti = ';';
		this.bte_UoM.SeparateString = ", ";
		this.bte_UoM.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListUoMByUoMGroupItemMaster;
		this.bte_UoM.UsingAutoCompleteBox = false;
		this.bte_UoM.UsingDataSource = true;
		this.bte_UoM.ValidateWhenNoDataSource = false;
		this.bte_UoM.ValueMember = "";
		this.rTime.AutoHeight = false;
		this.rTime.IsSearchControl = false;
		this.rTime.MaxLength = 5;
		this.rTime.Name = "rTime";
		this.grdEffic.AllowPasteExcel = false;
		this.grdEffic.ClipboardData = null;
		this.grdEffic.ContextMenuStrip = this.contextMenuEffic;
		this.grdEffic.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdEffic.Location = new System.Drawing.Point(0, 0);
		this.grdEffic.MainView = this.gvEffic;
		this.grdEffic.Name = "grdEffic";
		this.grdEffic.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rEfficResCode, this.rEfficCal });
		this.grdEffic.Size = new System.Drawing.Size(291, 383);
		this.grdEffic.TabIndex = 0;
		this.grdEffic.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvEffic });
		this.contextMenuEffic.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewLineToolStripMenuItem1, this.removeRowsToolStripMenuItem });
		this.contextMenuEffic.Name = "contextMenuEffic";
		this.contextMenuEffic.Size = new System.Drawing.Size(149, 48);
		this.addNewLineToolStripMenuItem1.Name = "addNewLineToolStripMenuItem1";
		this.addNewLineToolStripMenuItem1.Size = new System.Drawing.Size(148, 22);
		this.addNewLineToolStripMenuItem1.Text = "Add New Line";
		this.addNewLineToolStripMenuItem1.Click += new System.EventHandler(this.addNewLineToolStripMenuItem1_Click);
		this.removeRowsToolStripMenuItem.Name = "removeRowsToolStripMenuItem";
		this.removeRowsToolStripMenuItem.Size = new System.Drawing.Size(148, 22);
		this.removeRowsToolStripMenuItem.Text = "Remove Rows";
		this.removeRowsToolStripMenuItem.Click += new System.EventHandler(this.removeRowsToolStripMenuItem_Click);
		this.gvEffic.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvEffic.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvEffic.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clRouting;
		this.gvEffic.ColumnKey = null;
		this.gvEffic.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcEfficResCode, this.gcEfficResName, this.gcEfficEfficiency });
		this.gvEffic.DisableFormSetting = false;
		this.gvEffic.DisplayException = false;
		this.gvEffic.EditColumn = null;
		this.gvEffic.FieldCurrency = null;
		this.gvEffic.FieldRate = null;
		this.gvEffic.FormId = "oRouting";
		this.gvEffic.GridControl = this.grdEffic;
		this.gvEffic.IndexEditRow = -1;
		this.gvEffic.IndicatorWidth = 35;
		this.gvEffic.IsApzTable = true;
		this.gvEffic.IsInsertNewRow = false;
		this.gvEffic.IsLoadingData = false;
		this.gvEffic.ItemId = "APZ_RTA2";
		this.gvEffic.LstColumnAllowBatch = null;
		this.gvEffic.LstColumnNotAllowBatch = null;
		this.gvEffic.LstColumNotSetting = null;
		this.gvEffic.Name = "gvEffic";
		this.gvEffic.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvEffic.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gvEffic.OptionsPrint.AutoWidth = false;
		this.gvEffic.OptionsPrint.UsePrintStyles = false;
		this.gvEffic.OptionsView.BestFitMaxRowCount = 100;
		this.gvEffic.OptionsView.ColumnAutoWidth = false;
		this.gvEffic.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvEffic.OptionsView.ShowGroupedColumns = true;
		this.gvEffic.OptionsView.ShowGroupPanel = false;
		this.gvEffic.UseBatchMod = false;
		this.gvEffic.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvEffic_CellValueChanged);
		this.gcEfficResCode.Caption = "Resource Code";
		this.gcEfficResCode.ColumnEdit = this.rEfficResCode;
		this.gcEfficResCode.CurrencyDateField = null;
		this.gcEfficResCode.CurrencyField = null;
		this.gcEfficResCode.CurrencyFixed = null;
		this.gcEfficResCode.CurrencyFixedRate = 0.0;
		this.gcEfficResCode.CurrencyRateField = null;
		this.gcEfficResCode.FieldName = "ResCode";
		this.gcEfficResCode.Name = "gcEfficResCode";
		this.gcEfficResCode.PrimaryKey = false;
		this.gcEfficResCode.UnitField = null;
		this.gcEfficResCode.Visible = true;
		this.gcEfficResCode.VisibleIndex = 0;
		this.gcEfficResCode.Width = 82;
		this.rEfficResCode.AutoHeight = false;
		this.rEfficResCode.AutoLoadDataSource = true;
		editorButtonImageOptions14.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions29.Image");
		this.rEfficResCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions15, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), "Search"),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions14)
		});
		this.rEfficResCode.ContainerControl = this;
		this.rEfficResCode.DisplayMember = "";
		this.rEfficResCode.DisplayMember2 = "";
		this.rEfficResCode.LoadDataSourceWhenEnter = false;
		this.rEfficResCode.Name = "rEfficResCode";
		this.rEfficResCode.SeparateCharacterForMulti = ';';
		this.rEfficResCode.SeparateString = ", ";
		this.rEfficResCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListWorkCenter;
		this.rEfficResCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.WorkCenter;
		this.rEfficResCode.UsingAutoCompleteBox = false;
		this.rEfficResCode.UsingDataSource = true;
		this.rEfficResCode.ValidateWhenNoDataSource = false;
		this.rEfficResCode.ValueMember = "ResCode";
		this.gcEfficResName.Caption = "Resource Name";
		this.gcEfficResName.CurrencyDateField = null;
		this.gcEfficResName.CurrencyField = null;
		this.gcEfficResName.CurrencyFixed = null;
		this.gcEfficResName.CurrencyFixedRate = 0.0;
		this.gcEfficResName.CurrencyRateField = null;
		this.gcEfficResName.FieldName = "ResName";
		this.gcEfficResName.Name = "gcEfficResName";
		this.gcEfficResName.PrimaryKey = false;
		this.gcEfficResName.UnitField = null;
		this.gcEfficResName.Visible = true;
		this.gcEfficResName.VisibleIndex = 1;
		this.gcEfficResName.Width = 100;
		this.gcEfficEfficiency.Caption = "Efficiency";
		this.gcEfficEfficiency.ColumnEdit = this.rEfficCal;
		this.gcEfficEfficiency.CurrencyDateField = null;
		this.gcEfficEfficiency.CurrencyField = null;
		this.gcEfficEfficiency.CurrencyFixed = null;
		this.gcEfficEfficiency.CurrencyFixedRate = 0.0;
		this.gcEfficEfficiency.CurrencyRateField = null;
		this.gcEfficEfficiency.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcEfficEfficiency.FieldName = "Effic";
		this.gcEfficEfficiency.Name = "gcEfficEfficiency";
		this.gcEfficEfficiency.PrimaryKey = false;
		this.gcEfficEfficiency.UnitField = null;
		this.gcEfficEfficiency.Visible = true;
		this.gcEfficEfficiency.VisibleIndex = 2;
		this.rEfficCal.AutoHeight = false;
		this.rEfficCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rEfficCal.Name = "rEfficCal";
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.Location = new System.Drawing.Point(12, 600);
		this.btnSave.Name = "btnSave";
		this.btnSave.Size = new System.Drawing.Size(75, 23);
		this.btnSave.TabIndex = 20;
		this.btnSave.Text = "Save";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(92, 600);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 19;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnImportExcel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnImportExcel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnImportExcel.ImageOptions.Image");
		this.btnImportExcel.Location = new System.Drawing.Point(1417, 600);
		this.btnImportExcel.Name = "btnImportExcel";
		this.btnImportExcel.Size = new System.Drawing.Size(113, 23);
		this.btnImportExcel.TabIndex = 21;
		this.btnImportExcel.Text = "Import From Excel";
		this.btnImportExcel.Click += new System.EventHandler(this.btnImportExcel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1534, 626);
		base.Controls.Add(this.btnImportExcel);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.txt_ToDate);
		base.Controls.Add(this.txt_FromDate);
		base.Controls.Add(this.txt_Status);
		base.Controls.Add(this.txt_RoutNo);
		base.Controls.Add(this.lbToDate);
		base.Controls.Add(this.lbFromDate);
		base.Controls.Add(this.lbStatus);
		base.Controls.Add(this.lbRourCode);
		base.FormId = "oRouting";
		base.Name = "frm_AdvanceRouting";
		base.ShowIcon = false;
		this.Text = "Routing";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_AdvanceRoutings_FormClosing);
		base.Resize += new System.EventHandler(this.frm_AdvanceRouting_Resize);
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_RoutNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rProdCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rProdName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLookIsBase).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rlookupRoutNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitLine).EndInit();
		this.splitLine.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		this.contextMenuStrip_Line.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_WorkOrder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_OpeCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_OpeName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ResCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ResName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cal_Edit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_SetUnit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lookup_RunUnit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUomCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ProdCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ProdName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_SetupTask).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_RunTask).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rSubRouting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_UoM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rTime).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdEffic).EndInit();
		this.contextMenuEffic.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvEffic).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rEfficResCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rEfficCal).EndInit();
		base.ResumeLayout(false);
	}
}
