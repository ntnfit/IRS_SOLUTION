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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Financials;
using Apzon.Entities.Models.Inventory;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTab;

namespace Apzon.Forms.Financials;

public class frm_Costing : XtraFormEx
{
	private string _period = string.Empty;

	private DateTime _tRefDatePeriod;

	private int highestPercentageReached;

	private string _privilege = "";

	private DataTable dt = new DataTable();

	private IContainer components;

	private LabelLine labelLine1;

	private SimpleButtonEx btn_Run;

	private XtraTabControlEx tab_Main;

	private XtraTabPageEx tabpage_Costing;

	private XtraTabPageEx tabpage_Revaluation;

	private XtraTabPageEx tabpage_COGS;

	private XtraTabPageEx tabpage_KCSX;

	private GridControlEx grd_Costing;

	private SimpleButtonEx btn_Next;

	private SimpleButtonEx btn_Cancel;

	private AdvBandedGridViewEx bgv_Costing;

	private BandedGridColumn bandedGridColumn1;

	private BandedGridColumn bandedGridColumn2;

	private BandedGridColumn bandedGridColumn3;

	private GridControlEx grd_Revaluation;

	private GridViewEx gv_Revaluation;

	private SimpleButtonEx btn_Revaluation;

	private SimpleButtonEx btn_CancelRevaluation;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridControlEx grd_KCGV;

	private GridViewEx gv_KCGV;

	private SimpleButtonEx btn_CancelKCGV;

	private SimpleButtonEx btn_Add;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private SimpleButtonEx btn_CancelKCSX;

	private SimpleButtonEx btn_AddKCSX;

	private GridControlEx grd_KCSX;

	private GridViewEx gv_KCSX;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private BackgroundWorker bgw_Costing;

	private GridColumnEx gridColumn15;

	private XtraTabPageEx tabpage_CostingAllocate;

	private SimpleButtonEx simpleButton1;

	private SimpleButtonEx simpleButton2;

	private GridControlEx grd_Allocate;

	private GridViewEx gv_Allocate;

	private GridColumnEx gridColumn16;

	private GridColumnEx gridColumn17;

	private GridColumnEx gridColumn18;

	private GridColumnEx gridColumn19;

	private GridColumnEx gridColumn20;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumn22;

	private GridColumnEx gridColumn23;

	private GridColumnEx gridColumn24;

	private BackgroundWorker bgw_Allocate;

	private BackgroundWorker bgw_COGS;

	private BackgroundWorker bgw_Revaluation;

	private BackgroundWorker bgw_Allocation_ToRevaluation;

	private BackgroundWorker bgw_KCGV;

	private BackgroundWorker bgw_KCSX;

	private SimpleButtonEx btn_Export;

	private SimpleButtonEx btn_ExportPhanBoChiPhi;

	private SimpleButtonEx btn_ExportPhanBoHachToanKho;

	private SimpleButtonEx btn_ExportKCGV;

	private SimpleButtonEx btn_ExportCPSX;

	private ProgressBarControl progressBarControl1;

	private ComboBoxEditEx cb_Period;

	public frm_Costing()
	{
		try
		{
			if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Costing"))
			{
				InitializeComponent();
				if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Costing" && x.authoriza == "F"))
				{
					WinFuntions.DisableAllControl(Controls);
					btn_Cancel.Enabled = true;
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
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_Costing_Load(object sender, EventArgs e)
	{
		try
		{
			await NewData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadCb_Period()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialRevaluation");
			DataTable dataTable = await hp.GetDataTableAsync("LoadCBPeriod");
			List<Common.Table_Code> list = new List<Common.Table_Code>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				Common.Table_Code table_Code = new Common.Table_Code();
				table_Code.Code = dataTable.Rows[i]["Code"].ToString();
				table_Code.Name = dataTable.Rows[i]["Name"].ToString();
				list.Add(table_Code);
			}
			WinFuntions._addComboBox(cb_Period, list);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task NewData()
	{
		try
		{
			await LoadCb_Period();
			btn_Next.Enabled = false;
			tabpage_Revaluation.PageEnabled = false;
			tabpage_COGS.PageEnabled = false;
			tabpage_KCSX.PageEnabled = false;
			tab_Main.SelectedTabPage = tabpage_Costing;
			grd_Costing.DataSource = null;
			grd_Costing.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Run_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			tabpage_Revaluation.PageEnabled = false;
			tabpage_COGS.PageEnabled = false;
			tabpage_KCSX.PageEnabled = false;
			tabpage_CostingAllocate.PageEnabled = false;
			tab_Main.SelectedTabPage = tabpage_Costing;
			grd_Costing.DataSource = null;
			grd_Costing.RefreshDataSource();
			btn_Next.Enabled = false;
			btn_Cancel.Enabled = false;
			btn_Run.Enabled = false;
			btn_Run.Text = "Running...";
			bgw_Costing.RunWorkerAsync();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<long> Run(object argument, BackgroundWorker worker, DoWorkEventArgs e)
	{
		if (worker.CancellationPending)
		{
			e.Cancel = true;
		}
		else
		{
			try
			{
				if (cb_Period.SelectedItem != null)
				{
					string period = (_period = Function.ToString(((Common.ListItems)cb_Period.SelectedItem).Value));
					using (HttpClientHelper<DataTable> hp_1 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialRevaluation"))
					{
						WinCommons._OpenProcessing();
						DataTable dataTable = await hp_1.GetDataTableAsync("LastDate", new { period });
						if (dataTable != null && dataTable.Rows.Count > 0)
						{
							DateTime tRefDatePeriod = Convert.ToDateTime(dataTable.Rows[0]["T_RefDate"]);
							_tRefDatePeriod = tRefDatePeriod;
						}
						else
						{
							_tRefDatePeriod = DateTime.Today;
						}
						WinCommons.Warning(Enums.MessageCode.None, _tRefDatePeriod.ToString() ?? "");
						WinCommons._CloseProcessing();
					}
					using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialRevaluation");
					WinCommons._OpenProcessing();
					DataTable dtProductionCosting = await hp.GetDataTableAsync("Costing", new { period });
					BeginInvoke((MethodInvoker)(() =>
					{
						bgv_Costing.Bands.Clear();
						bgv_Costing.Columns.Clear();
						DataTable dataTable2 = new DataTable();
						if (dtProductionCosting != null && dtProductionCosting.Rows.Count > 0)
						{
							dataTable2.Columns.Add("ClassID", typeof(string));
							dataTable2.Columns.Add("PCostType", typeof(string));
							GridBand gridBand = bgv_Costing.Bands.Add();
							gridBand.Fixed = FixedStyle.Left;
							BandedGridColumn column = new BandedGridColumn
							{
								Caption = "Production Qty",
								Name = "cl_PrdQty",
								Visible = true,
								FieldName = "PrdQty"
							};
							dataTable2.Columns.Add("PrdQty", typeof(double));
							gridBand.Columns.Add(column);
							column = new BandedGridColumn
							{
								Caption = "Item Name",
								Name = "cl_ItemName",
								Visible = true,
								FieldName = "ItmName"
							};
							dataTable2.Columns.Add("ItmName", typeof(string));
							gridBand.Columns.Add(column);
							column = new BandedGridColumn
							{
								Caption = "Item Code",
								Name = "cl_ItemCode",
								Visible = true,
								FieldName = "ItmCode"
							};
							dataTable2.Columns.Add("ItmCode", typeof(string));
							gridBand.Columns.Add(column);
							foreach (var item in (from t in (from s in dtProductionCosting.AsEnumerable()
									select new
									{
										Id = s.Field<string>("ClassID"),
										Name = s.Field<string>("ClassName")
									}).Distinct()
								orderby t.Name
								select t).ToList())
							{
								GridBand gridBand2 = bgv_Costing.Bands.Add();
								gridBand2.Caption = item.Name;
								gridBand2.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Center;
								gridBand2.Name = "band_" + item.Id;
								gridBand2.Visible = true;
								gridBand2.MinWidth = 300;
								DataRow[] array = dtProductionCosting.Select("ClassID='" + item.Id + "'");
								if (array != null && array.Length != 0)
								{
									foreach (var item2 in (from t in (from s in array.CopyToDataTable().AsEnumerable()
											select new
											{
												Id = s.Field<string>("PCostType"),
												Name = s.Field<string>("PCostName")
											}).Distinct()
										orderby t.Name descending
										select t).ToList())
									{
										column = new BandedGridColumn
										{
											Caption = item2.Name,
											Name = "cl_" + item.Id + item2.Id,
											Visible = true,
											FieldName = item.Id + item2.Id
										};
										column.DisplayFormat.FormatType = FormatType.Numeric;
										column.DisplayFormat.FormatString = "n0";
										column.GroupFormat.FormatType = FormatType.Numeric;
										column.SummaryItem.FieldName = column.FieldName;
										column.SummaryItem.DisplayFormat = "{0:n0}";
										column.SummaryItem.SummaryType = SummaryItemType.Sum;
										column.Width = 100;
										column.AppearanceHeader.TextOptions.HAlignment = HorzAlignment.Far;
										dataTable2.Columns.Add(item.Id + item2.Id, typeof(double));
										gridBand2.MinWidth = 200;
										gridBand2.Columns.Add(column);
									}
								}
							}
							dataTable2.PrimaryKey = new DataColumn[1] { dataTable2.Columns["ItmCode"] };
							foreach (DataRow row in dtProductionCosting.Rows)
							{
								DataRow dataRow2 = dataTable2.NewRow();
								DataRow dataRow3 = dataTable2.Rows.Find(new object[1] { row["ItmCode"] });
								if (dataRow3 != null)
								{
									dataRow3[Function.ToString(row["ClassID"]) + Function.ToString(row["PCostType"])] = row["Amt"];
								}
								else
								{
									dataRow2["ItmCode"] = row["ItmCode"];
									dataRow2["ClassID"] = row["ClassID"];
									dataRow2["ItmName"] = row["ItmName"];
									dataRow2["PrdQty"] = row["PrdQty"];
									dataRow2["PCostType"] = row["PCostType"];
									dataRow2[Function.ToString(row["ClassID"]) + Function.ToString(row["PCostType"])] = row["Amt"];
								}
								if (dataRow3 == null)
								{
									dataTable2.Rows.Add(dataRow2);
								}
							}
							grd_Costing.DataSource = dataTable2;
							grd_Costing.RefreshDataSource();
							WinCommons._CloseProcessing();
						}
					}));
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		}
		return 1L;
	}

	private void simpleButton2_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_Next_Click(object sender, EventArgs e)
	{
		try
		{
			tabpage_CostingAllocate.PageEnabled = true;
			tab_Main.SelectedTabPage = tabpage_CostingAllocate;
			btn_Run.Enabled = false;
			bgw_Allocate.RunWorkerAsync();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_CancelRevaluation_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Revaluation_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (btn_Revaluation.Text == "Next")
			{
				btn_Revaluation.Enabled = true;
				btn_Run.Enabled = true;
				tab_Main.SelectedTabPage = tabpage_COGS;
				tabpage_COGS.PageEnabled = true;
				if (cb_Period.SelectedItem != null)
				{
					string period = Function.ToString(((Common.ListItems)cb_Period.SelectedItem).Value);
					using (HttpClientHelper<DataTable> hp_1 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialRevaluation"))
					{
						if (!(await hp_1.GetBoolAsync("CheckCogs", new { period })))
						{
							btn_Add.Text = "Next";
						}
					}
					using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialRevaluation");
					DataTable dataTable = await hp.GetDataTableAsync("Cogs", new { period });
					if (dataTable == null || dataTable.Rows.Count <= 0)
					{
						btn_Add.Text = "Next";
					}
					grd_KCGV.DataSource = dataTable;
					grd_KCGV.RefreshDataSource();
					return;
				}
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmAddInventoryRevaluation, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				btn_Revaluation.Enabled = false;
				btn_Revaluation.Text = "Processing...";
				btn_Run.Enabled = false;
				progressBarControl1.Visible = true;
				bgw_Revaluation.RunWorkerAsync();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_CancelKCGV_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Add_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (btn_Add.Text == "Next")
			{
				tab_Main.SelectedTabPage = tabpage_KCSX;
				tabpage_KCSX.PageEnabled = true;
				btn_Run.Enabled = true;
				btn_Add.Enabled = true;
				DataTable dtKCSX = new DataTable();
				if (cb_Period.SelectedItem != null)
				{
					string period = Function.ToString(((Common.ListItems)cb_Period.SelectedItem).Value);
					using (HttpClientHelper<DataTable> hp_1 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialRevaluation"))
					{
						dtKCSX = await hp_1.GetDataTableAsync("KCSX", new { period });
					}
					using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialRevaluation");
					if (!(await hp.GetBoolAsync("CheckProd", new { period })))
					{
						btn_AddKCSX.Text = "Finish";
					}
					grd_KCSX.DataSource = dtKCSX;
					grd_KCSX.RefreshDataSource();
					for (int i = 0; i < gv_KCSX.Columns.Count; i++)
					{
						gv_KCSX.Columns[i].OptionsColumn.ReadOnly = true;
					}
					return;
				}
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				btn_Add.Enabled = false;
				btn_Add.Text = "Adding...";
				btn_Run.Enabled = false;
				bgw_KCGV.RunWorkerAsync();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_CancelKCSX_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_AddKCSX_Click(object sender, EventArgs e)
	{
		try
		{
			if (btn_AddKCSX.Text == "Finish")
			{
				btn_Run.Enabled = true;
				btn_AddKCSX.Enabled = true;
				btn_AddKCSX.Text = "Add";
				await NewData();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				btn_AddKCSX.Enabled = false;
				btn_Run.Enabled = false;
				btn_AddKCSX.Text = "Adding...";
				bgw_KCSX.RunWorkerAsync();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bgw_Costing_DoWork(object sender, DoWorkEventArgs e)
	{
		try
		{
			BackgroundWorker backgroundWorker = sender as BackgroundWorker;
			backgroundWorker.WorkerReportsProgress = true;
			e.Result = await Run(e.Argument, backgroundWorker, e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bgw_Costing_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
	}

	private void bgw_Costing_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		try
		{
			if (e.Error != null)
			{
				WinCommons.Error(Enums.MessageCode.Error, "");
			}
			else if (e.Cancelled)
			{
				WinCommons.Error(Enums.MessageCode.ThatOperationHasCanceled, "");
			}
			else if (Function.ParseInt(e.Result) == 1)
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			btn_Next.Enabled = true;
			btn_Cancel.Enabled = true;
			btn_Run.Enabled = true;
			btn_Run.Text = "Run";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void simpleButton1_Click(object sender, EventArgs e)
	{
		try
		{
			tabpage_Revaluation.PageEnabled = true;
			tab_Main.SelectedTabPage = tabpage_Revaluation;
			btn_Run.Enabled = false;
			bgw_Allocation_ToRevaluation.RunWorkerAsync();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bgw_Allocate_DoWork(object sender, DoWorkEventArgs e)
	{
		try
		{
			BackgroundWorker backgroundWorker = sender as BackgroundWorker;
			backgroundWorker.WorkerReportsProgress = true;
			e.Result = await NextAllocate(e.Argument, backgroundWorker, e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<object> NextAllocate(object obj, BackgroundWorker worker, DoWorkEventArgs e)
	{
		try
		{
			DataTable dt = new DataTable();
			if (worker.CancellationPending)
			{
				e.Cancel = true;
			}
			else
			{
				try
				{
					if (cb_Period.SelectedItem != null)
					{
						string period = Function.ToString(((Common.ListItems)cb_Period.SelectedItem).Value);
						using (HttpClientHelper<DataTable> hp1 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialRevaluation"))
						{
							dt = await hp1.GetDataTableAsync("CostingAllocate", new { period });
						}
						BeginInvoke((MethodInvoker)(() =>
						{
							grd_Allocate.DataSource = dt;
							gv_Allocate.PopulateColumns();
							grd_Allocate.RefreshDataSource();
							gv_Allocate.PopulateColumns();
							for (int i = 0; i < gv_Allocate.Columns.Count; i++)
							{
								gv_Allocate.Columns[i].OptionsColumn.ReadOnly = true;
								GridColumn gridColumn = gv_Allocate.Columns[i];
								gv_Allocate.Columns[i].Width = 150;
								if (gridColumn.ColumnType == typeof(int) || gridColumn.ColumnType == typeof(double) || gridColumn.ColumnType == typeof(float) || gridColumn.ColumnType == typeof(long) || gridColumn.ColumnType == typeof(short) || gridColumn.ColumnType == typeof(decimal))
								{
									gv_Allocate.Columns[i].DisplayFormat.FormatType = FormatType.Numeric;
									gv_Allocate.Columns[i].DisplayFormat.FormatString = "n0";
								}
							}
						}));
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
					return 0;
				}
			}
			return 1;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
		return 1;
	}

	private void bgw_Allocate_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
	}

	private void bgw_Allocate_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		try
		{
			if (e.Error != null)
			{
				WinCommons.Error(Enums.MessageCode.Error, "");
			}
			else if (e.Cancelled)
			{
				WinCommons.Error(Enums.MessageCode.ThatOperationHasCanceled, "");
			}
			else if (Function.ParseInt(e.Result) == 1)
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				btn_Run.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void bgw_Revaluation_DoWork(object sender, DoWorkEventArgs e)
	{
		try
		{
			BackgroundWorker backgroundWorker = sender as BackgroundWorker;
			backgroundWorker.WorkerReportsProgress = true;
			e.Result = await Revaluation(e.Argument, backgroundWorker, e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<long> Revaluation(object obj, BackgroundWorker worker, DoWorkEventArgs e)
	{
		if (worker.CancellationPending)
		{
			e.Cancel = true;
		}
		else
		{
			try
			{
				new DataTable();
				DataTable dataTable = (DataTable)grd_Revaluation.DataSource;
				if (dataTable == null || dataTable.Rows == null || dataTable.Rows.Count <= 0)
				{
					WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
					return 0L;
				}
				MaterialRevaluation materialRevaluation = new MaterialRevaluation();
				materialRevaluation.DocDate = _tRefDatePeriod;
				materialRevaluation.RevalType = "M";
				materialRevaluation.TaxDate = _tRefDatePeriod;
				materialRevaluation.Comments = "Add-on: Add new Inventory Revaluation";
				materialRevaluation.UserFields = new List<UserFieldsImpl>
				{
					new UserFieldsImpl
					{
						Name = "U_Period",
						Value = _period
					}
				};
				materialRevaluation.Lines = new List<MaterialRevaluation_linesImpl>();
				materialRevaluation.Lines = (from t in dataTable.AsEnumerable()
					select new MaterialRevaluation_linesImpl
					{
						ItemCode = Function.ToString(t["ItmCode"]),
						Quantity = Function.ParseDouble(t["Qty"]),
						DebitCredit = Function.ParseDouble(t["Amount"]),
						WarehouseCode = Function.ToString(t["WhsCode"]),
						RevaluationDecrementAccount = Function.ToString(t["DebAcct"]),
						RevaluationIncrementAccount = Function.ToString(t["CredAcct"])
					}).ToList();
				using HttpClientHelper<MaterialRevaluation> hp = new HttpClientHelper<MaterialRevaluation>(Configuration.ApiServer, "MaterialRevaluation");
				if (!string.IsNullOrEmpty(Function.ToString(await hp.PostDataTableAsync("AddPart", new
				{
					doc = materialRevaluation,
					Lines = materialRevaluation.Lines
				}))))
				{
					return 0L;
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
				return 0L;
			}
		}
		return 1L;
	}

	private void bgw_Revaluation_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		progressBarControl1.EditValue = e.ProgressPercentage;
	}

	private void bgw_Revaluation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error != null)
		{
			WinCommons.Error(Enums.MessageCode.Error, "");
		}
		else if (e.Cancelled)
		{
			WinCommons.Error(Enums.MessageCode.ThatOperationHasCanceled, "");
		}
		else if (Function.ParseInt(e.Result) == 1)
		{
			WinCommons.Success(Enums.MessageCode.Success, "");
			btn_Revaluation.Text = "Next";
			btn_Revaluation.Enabled = true;
			return;
		}
		btn_Revaluation.Text = "Revaluation";
		btn_Revaluation.Enabled = true;
	}

	private async void bgw_Allocation_ToRevaluation_DoWork(object sender, DoWorkEventArgs e)
	{
		BackgroundWorker backgroundWorker = sender as BackgroundWorker;
		backgroundWorker.WorkerReportsProgress = true;
		e.Result = await NextToRevaluation(e.Argument, backgroundWorker, e);
	}

	private async Task<long> NextToRevaluation(object p, BackgroundWorker worker, DoWorkEventArgs e)
	{
		_ = 1;
		try
		{
			if (worker.CancellationPending)
			{
				e.Cancel = true;
			}
			else
			{
				try
				{
					if (!string.IsNullOrEmpty(cb_Period.SelectedItem.ToString()))
					{
						string period = Function.ToString(((Common.ListItems)cb_Period.SelectedItem).Value);
						using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialRevaluation");
						dt = await hp.GetDataTableAsync("CostingTable", new { period });
						using (HttpClientHelper<DataTable> hp2 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MaterialRevaluation"))
						{
							if (!(await hp2.GetBoolAsync("CheckRevaluation", new { period })))
							{
								BeginInvoke((MethodInvoker)(() =>
								{
									btn_Revaluation.Text = "Next";
									btn_Revaluation.Enabled = true;
								}));
							}
						}
						BeginInvoke((MethodInvoker)(() =>
						{
							grd_Revaluation.DataSource = dt;
							grd_Revaluation.RefreshDataSource();
							for (int i = 0; i < gv_Revaluation.Columns.Count; i++)
							{
								gv_Revaluation.Columns[i].OptionsColumn.ReadOnly = true;
							}
						}));
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
					WinCommons.ErrorLineNumber(ex);
					return 0L;
				}
			}
			return 1L;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.ErrorLineNumber(ex2);
		}
		return 1L;
	}

	private void bgw_Allocation_ToRevaluation_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error != null)
		{
			WinCommons.Error(Enums.MessageCode.Error, "");
		}
		else if (e.Cancelled)
		{
			WinCommons.Error(Enums.MessageCode.ThatOperationHasCanceled, "");
		}
		else if (Function.ParseInt(e.Result) == 1)
		{
			WinCommons.Success(Enums.MessageCode.Success, "");
			btn_Run.Enabled = true;
		}
	}

	private async void bgw_KCGV_DoWork(object sender, DoWorkEventArgs e)
	{
		BackgroundWorker backgroundWorker = sender as BackgroundWorker;
		backgroundWorker.WorkerReportsProgress = true;
		e.Result = await KCGV(e.Argument, backgroundWorker, e);
	}

	private async Task<long> KCGV(object obj, BackgroundWorker worker, DoWorkEventArgs e)
	{
		try
		{
			dt = (DataTable)grd_KCGV.DataSource;
			if (dt == null || dt.Rows.Count <= 0)
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return 0L;
			}
			JournalEntriesImpl journalEntriesImpl = new JournalEntriesImpl();
			journalEntriesImpl.RefDate = _tRefDatePeriod;
			journalEntriesImpl.DueDate = _tRefDatePeriod;
			journalEntriesImpl.UserFields = new List<UserFieldsImpl>
			{
				new UserFieldsImpl
				{
					Name = "U_PeriodCogs",
					Value = _period
				}
			};
			journalEntriesImpl.Lines = new List<JournalEntries_LinesImpl>();
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				JournalEntries_LinesImpl journalEntries_LinesImpl = new JournalEntries_LinesImpl();
				journalEntries_LinesImpl.Account = Function.ToString(dt.Rows[i]["AcctCode"]);
				journalEntries_LinesImpl.Debit = Function.ParseDouble(dt.Rows[i]["Debit"]);
				journalEntries_LinesImpl.Credit = Function.ParseDouble(dt.Rows[i]["Credit"]);
				journalEntriesImpl.Lines.Add(journalEntries_LinesImpl);
			}
			using HttpClientHelper<JournalEntriesImpl> hp1 = new HttpClientHelper<JournalEntriesImpl>(Configuration.ApiServer, "JournalEntries");
			if (!string.IsNullOrEmpty(await hp1.GetStringAsync("CheckRevaluation", new
			{
				doc = journalEntriesImpl
			})))
			{
				return 0L;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0L;
		}
		return 1L;
	}

	private void bgw_KCGV_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error != null)
		{
			WinCommons.Error(Enums.MessageCode.Error, "");
		}
		else if (e.Cancelled)
		{
			WinCommons.Error(Enums.MessageCode.ThatOperationHasCanceled, "");
		}
		else if (Function.ParseInt(e.Result) == 1)
		{
			WinCommons.Success(Enums.MessageCode.Success, "");
			btn_Add.Text = "Next";
			btn_Add.Enabled = true;
		}
	}

	private async void bgw_KCSX_DoWork(object sender, DoWorkEventArgs e)
	{
		BackgroundWorker backgroundWorker = sender as BackgroundWorker;
		backgroundWorker.WorkerReportsProgress = true;
		e.Result = await KCSX(e.Argument, backgroundWorker, e);
	}

	private async Task<long> KCSX(object obj, BackgroundWorker worker, DoWorkEventArgs e)
	{
		try
		{
			new DataTable();
			DataTable dataTable = (DataTable)grd_KCSX.DataSource;
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				WinCommons.Error(Enums.MessageCode.DatasourceIsNull, "");
				return 0L;
			}
			JournalEntriesImpl journalEntriesImpl = new JournalEntriesImpl();
			journalEntriesImpl.RefDate = _tRefDatePeriod;
			journalEntriesImpl.DueDate = _tRefDatePeriod;
			journalEntriesImpl.UserFields = new List<UserFieldsImpl>
			{
				new UserFieldsImpl
				{
					Name = "U_PeriodProd",
					Value = _period
				}
			};
			journalEntriesImpl.Lines = new List<JournalEntries_LinesImpl>();
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				JournalEntries_LinesImpl journalEntries_LinesImpl = new JournalEntries_LinesImpl();
				journalEntries_LinesImpl.Account = Function.ToString(dataTable.Rows[i]["AcctCode"]);
				journalEntries_LinesImpl.Debit = Function.ParseDouble(dataTable.Rows[i]["Debit"]);
				journalEntries_LinesImpl.Credit = Function.ParseDouble(dataTable.Rows[i]["Credit"]);
				journalEntriesImpl.Lines.Add(journalEntries_LinesImpl);
			}
			using HttpClientHelper<JournalEntriesImpl> hp1 = new HttpClientHelper<JournalEntriesImpl>(Configuration.ApiServer, "JournalEntries");
			string text = await hp1.GetStringAsync("CheckRevaluation", new
			{
				doc = journalEntriesImpl
			});
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				return 0L;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0L;
		}
		return 1L;
	}

	private async void bgw_KCSX_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		if (e.Error != null)
		{
			WinCommons.Error(Enums.MessageCode.Error, "");
		}
		else if (e.Cancelled)
		{
			WinCommons.Error(Enums.MessageCode.ThatOperationHasCanceled, "");
		}
		else if (Function.ParseInt(e.Result) == 1)
		{
			WinCommons.Success(Enums.MessageCode.Success, "");
			btn_Run.Enabled = true;
			btn_AddKCSX.Enabled = true;
			btn_AddKCSX.Text = "Add";
			await NewData();
		}
	}

	private void tabpage_COGS_Paint(object sender, PaintEventArgs e)
	{
	}

	private void btn_Export_Click(object sender, EventArgs e)
	{
		try
		{
			string text = WinFuntions.ExportGridcontrolToXls("Ba\u0309ng ti\u0301nh gia\u0301 tha\u0300nh", grd_Costing);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Warning(Enums.MessageCode.None, text ?? "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ExportPhanBoChiPhi_Click(object sender, EventArgs e)
	{
		try
		{
			string text = WinFuntions.ExportGridcontrolToXls("Phân bô\u0309 chi phi\u0301", grd_Allocate);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Warning(Enums.MessageCode.None, text ?? "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ExportPhanBoHachToanKho_Click(object sender, EventArgs e)
	{
		try
		{
			string text = WinFuntions.ExportGridcontrolToXls("Phân bô\u0309 ha\u0323ch toa\u0301n kho", grd_Revaluation);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Warning(Enums.MessageCode.None, text ?? "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ExportKCGV_Click(object sender, EventArgs e)
	{
		try
		{
			string text = WinFuntions.ExportGridcontrolToXls("Kê\u0301t chuyê\u0309n gia\u0301 vô\u0301n", grd_KCGV);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Warning(Enums.MessageCode.None, text ?? "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ExportCPSX_Click(object sender, EventArgs e)
	{
		try
		{
			string text = WinFuntions.ExportGridcontrolToXls("Kê\u0301t chi phi\u0301 sa\u0309n xuâ\u0301t", grd_KCSX);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Warning(Enums.MessageCode.None, text ?? "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_Costing_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((bgw_Costing.IsBusy || bgw_KCGV.IsBusy || bgw_Revaluation.IsBusy || bgw_Allocate.IsBusy || bgw_COGS.IsBusy || bgw_Allocation_ToRevaluation.IsBusy || bgw_KCSX.IsBusy) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.No)
			{
				e.Cancel = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Financials.frm_Costing));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Run = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tab_Main = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabpage_Costing = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_Export = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Next = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Costing = new Apzon.Controls.Grid.GridControlEx();
		this.bgv_Costing = new Apzon.Controls.Grid.AdvBandedGridViewEx();
		this.bandedGridColumn1 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.bandedGridColumn2 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.bandedGridColumn3 = new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn();
		this.tabpage_CostingAllocate = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_ExportPhanBoChiPhi = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton1 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.simpleButton2 = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Allocate = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Allocate = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn24 = new Apzon.Controls.Grid.GridColumnEx();
		this.tabpage_Revaluation = new Apzon.Controls.Controls.XtraTabPageEx();
		this.progressBarControl1 = new DevExpress.XtraEditors.ProgressBarControl();
		this.btn_ExportPhanBoHachToanKho = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_CancelRevaluation = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Revaluation = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Revaluation = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Revaluation = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.tabpage_COGS = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_ExportKCGV = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_CancelKCGV = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Add = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_KCGV = new Apzon.Controls.Grid.GridControlEx();
		this.gv_KCGV = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.tabpage_KCSX = new Apzon.Controls.Controls.XtraTabPageEx();
		this.btn_ExportCPSX = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_CancelKCSX = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_AddKCSX = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_KCSX = new Apzon.Controls.Grid.GridControlEx();
		this.gv_KCSX = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.bgw_Costing = new System.ComponentModel.BackgroundWorker();
		this.bgw_Allocate = new System.ComponentModel.BackgroundWorker();
		this.bgw_COGS = new System.ComponentModel.BackgroundWorker();
		this.bgw_Revaluation = new System.ComponentModel.BackgroundWorker();
		this.bgw_Allocation_ToRevaluation = new System.ComponentModel.BackgroundWorker();
		this.bgw_KCGV = new System.ComponentModel.BackgroundWorker();
		this.bgw_KCSX = new System.ComponentModel.BackgroundWorker();
		this.cb_Period = new Apzon.Controls.Controls.ComboBoxEditEx();
		((System.ComponentModel.ISupportInitialize)this.tab_Main).BeginInit();
		this.tab_Main.SuspendLayout();
		this.tabpage_Costing.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Costing).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bgv_Costing).BeginInit();
		this.tabpage_CostingAllocate.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Allocate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Allocate).BeginInit();
		this.tabpage_Revaluation.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.progressBarControl1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Revaluation).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Revaluation).BeginInit();
		this.tabpage_COGS.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_KCGV).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_KCGV).BeginInit();
		this.tabpage_KCSX.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_KCSX).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_KCSX).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Period.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Period";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(2, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Period";
		this.btn_Run.EditControl = null;
		this.btn_Run.EditValue = "Run";
		this.btn_Run.FieldName = null;
		this.btn_Run.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Run.ImageOptions.Image");
		this.btn_Run.IsSearchControl = false;
		this.btn_Run.Location = new System.Drawing.Point(234, 1);
		this.btn_Run.Name = "btn_Run";
		this.btn_Run.ReadOnly = false;
		this.btn_Run.SettingInVisible = false;
		this.btn_Run.SettingReadonly = false;
		this.btn_Run.Size = new System.Drawing.Size(75, 23);
		this.btn_Run.TabIndex = 2;
		this.btn_Run.Text = "Run";
		this.btn_Run.Click += new System.EventHandler(this.btn_Run_Click);
		this.tab_Main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tab_Main.IsSearchControl = false;
		this.tab_Main.Location = new System.Drawing.Point(2, 28);
		this.tab_Main.Name = "tab_Main";
		this.tab_Main.SelectedTabPage = this.tabpage_Costing;
		this.tab_Main.Size = new System.Drawing.Size(785, 362);
		this.tab_Main.TabIndex = 3;
		this.tab_Main.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[5] { this.tabpage_Costing, this.tabpage_CostingAllocate, this.tabpage_Revaluation, this.tabpage_COGS, this.tabpage_KCSX });
		this.tabpage_Costing.Controls.Add(this.btn_Export);
		this.tabpage_Costing.Controls.Add(this.btn_Next);
		this.tabpage_Costing.Controls.Add(this.btn_Cancel);
		this.tabpage_Costing.Controls.Add(this.grd_Costing);
		this.tabpage_Costing.EditControl = null;
		this.tabpage_Costing.EditValue = "Ba\u0309ng ti\u0301nh gia\u0301 tha\u0300nh";
		this.tabpage_Costing.FieldName = null;
		this.tabpage_Costing.IsSearchControl = false;
		this.tabpage_Costing.Name = "tabpage_Costing";
		this.tabpage_Costing.ReadOnly = false;
		this.tabpage_Costing.SettingInVisible = false;
		this.tabpage_Costing.SettingReadonly = false;
		this.tabpage_Costing.Size = new System.Drawing.Size(783, 337);
		this.tabpage_Costing.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_Costing.Text = "Ba\u0309ng ti\u0301nh gia\u0301 tha\u0300nh";
		this.btn_Export.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Export.EditControl = null;
		this.btn_Export.EditValue = "Export";
		this.btn_Export.FieldName = null;
		this.btn_Export.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Export.ImageOptions.Image");
		this.btn_Export.IsSearchControl = false;
		this.btn_Export.Location = new System.Drawing.Point(1, 306);
		this.btn_Export.Name = "btn_Export";
		this.btn_Export.ReadOnly = false;
		this.btn_Export.SettingInVisible = false;
		this.btn_Export.SettingReadonly = false;
		this.btn_Export.Size = new System.Drawing.Size(75, 23);
		this.btn_Export.TabIndex = 6;
		this.btn_Export.Text = "Export";
		this.btn_Export.Click += new System.EventHandler(this.btn_Export_Click);
		this.btn_Next.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Next.EditControl = null;
		this.btn_Next.EditValue = "Next";
		this.btn_Next.FieldName = null;
		this.btn_Next.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Next.ImageOptions.Image");
		this.btn_Next.IsSearchControl = false;
		this.btn_Next.Location = new System.Drawing.Point(702, 306);
		this.btn_Next.Name = "btn_Next";
		this.btn_Next.ReadOnly = false;
		this.btn_Next.SettingInVisible = false;
		this.btn_Next.SettingReadonly = false;
		this.btn_Next.Size = new System.Drawing.Size(75, 23);
		this.btn_Next.TabIndex = 4;
		this.btn_Next.Text = "Next";
		this.btn_Next.Click += new System.EventHandler(this.btn_Next_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(621, 306);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 5;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.simpleButton2_Click);
		this.grd_Costing.AllowPasteExcel = false;
		this.grd_Costing.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Costing.ClipboardData = null;
		this.grd_Costing.Location = new System.Drawing.Point(0, 0);
		this.grd_Costing.MainView = this.bgv_Costing;
		this.grd_Costing.Name = "grd_Costing";
		this.grd_Costing.Size = new System.Drawing.Size(781, 300);
		this.grd_Costing.TabIndex = 0;
		this.grd_Costing.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.bgv_Costing });
		this.bgv_Costing.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.bgv_Costing.Columns.AddRange(new DevExpress.XtraGrid.Views.BandedGrid.BandedGridColumn[3] { this.bandedGridColumn1, this.bandedGridColumn2, this.bandedGridColumn3 });
		this.bgv_Costing.CustomizationFormBounds = new System.Drawing.Rectangle(441, 346, 222, 219);
		this.bgv_Costing.FormId = null;
		this.bgv_Costing.GridControl = this.grd_Costing;
		this.bgv_Costing.IsApzTable = false;
		this.bgv_Costing.ItemId = null;
		this.bgv_Costing.Name = "bgv_Costing";
		this.bgv_Costing.OptionsView.ShowFooter = true;
		this.bgv_Costing.OptionsView.ShowGroupPanel = false;
		this.bandedGridColumn1.Caption = "Item Code";
		this.bandedGridColumn1.FieldName = "ItemCode";
		this.bandedGridColumn1.Name = "bandedGridColumn1";
		this.bandedGridColumn1.Visible = true;
		this.bandedGridColumn1.Width = 136;
		this.bandedGridColumn2.Caption = "Item Name";
		this.bandedGridColumn2.FieldName = "ItemName";
		this.bandedGridColumn2.Name = "bandedGridColumn2";
		this.bandedGridColumn2.Visible = true;
		this.bandedGridColumn2.Width = 155;
		this.bandedGridColumn3.Caption = "Production Qty";
		this.bandedGridColumn3.FieldName = "ProdQty";
		this.bandedGridColumn3.Name = "bandedGridColumn3";
		this.bandedGridColumn3.Visible = true;
		this.bandedGridColumn3.Width = 141;
		this.tabpage_CostingAllocate.Controls.Add(this.btn_ExportPhanBoChiPhi);
		this.tabpage_CostingAllocate.Controls.Add(this.simpleButton1);
		this.tabpage_CostingAllocate.Controls.Add(this.simpleButton2);
		this.tabpage_CostingAllocate.Controls.Add(this.grd_Allocate);
		this.tabpage_CostingAllocate.EditControl = null;
		this.tabpage_CostingAllocate.EditValue = "Phân bô\u0309 chi phi\u0301";
		this.tabpage_CostingAllocate.FieldName = null;
		this.tabpage_CostingAllocate.IsSearchControl = false;
		this.tabpage_CostingAllocate.Name = "tabpage_CostingAllocate";
		this.tabpage_CostingAllocate.PageEnabled = false;
		this.tabpage_CostingAllocate.ReadOnly = false;
		this.tabpage_CostingAllocate.SettingInVisible = false;
		this.tabpage_CostingAllocate.SettingReadonly = false;
		this.tabpage_CostingAllocate.Size = new System.Drawing.Size(783, 337);
		this.tabpage_CostingAllocate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_CostingAllocate.Text = "Phân bô\u0309 chi phi\u0301";
		this.btn_ExportPhanBoChiPhi.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_ExportPhanBoChiPhi.EditControl = null;
		this.btn_ExportPhanBoChiPhi.EditValue = "Export";
		this.btn_ExportPhanBoChiPhi.FieldName = null;
		this.btn_ExportPhanBoChiPhi.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ExportPhanBoChiPhi.ImageOptions.Image");
		this.btn_ExportPhanBoChiPhi.IsSearchControl = false;
		this.btn_ExportPhanBoChiPhi.Location = new System.Drawing.Point(0, 306);
		this.btn_ExportPhanBoChiPhi.Name = "btn_ExportPhanBoChiPhi";
		this.btn_ExportPhanBoChiPhi.ReadOnly = false;
		this.btn_ExportPhanBoChiPhi.SettingInVisible = false;
		this.btn_ExportPhanBoChiPhi.SettingReadonly = false;
		this.btn_ExportPhanBoChiPhi.Size = new System.Drawing.Size(75, 23);
		this.btn_ExportPhanBoChiPhi.TabIndex = 8;
		this.btn_ExportPhanBoChiPhi.Text = "Export";
		this.btn_ExportPhanBoChiPhi.Click += new System.EventHandler(this.btn_ExportPhanBoChiPhi_Click);
		this.simpleButton1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton1.EditControl = null;
		this.simpleButton1.EditValue = "Next";
		this.simpleButton1.FieldName = null;
		this.simpleButton1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton1.ImageOptions.Image");
		this.simpleButton1.IsSearchControl = false;
		this.simpleButton1.Location = new System.Drawing.Point(701, 306);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.ReadOnly = false;
		this.simpleButton1.SettingInVisible = false;
		this.simpleButton1.SettingReadonly = false;
		this.simpleButton1.Size = new System.Drawing.Size(75, 23);
		this.simpleButton1.TabIndex = 6;
		this.simpleButton1.Text = "Next";
		this.simpleButton1.Click += new System.EventHandler(this.simpleButton1_Click);
		this.simpleButton2.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleButton2.EditControl = null;
		this.simpleButton2.EditValue = "Cancel";
		this.simpleButton2.FieldName = null;
		this.simpleButton2.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton2.ImageOptions.Image");
		this.simpleButton2.IsSearchControl = false;
		this.simpleButton2.Location = new System.Drawing.Point(620, 306);
		this.simpleButton2.Name = "simpleButton2";
		this.simpleButton2.ReadOnly = false;
		this.simpleButton2.SettingInVisible = false;
		this.simpleButton2.SettingReadonly = false;
		this.simpleButton2.Size = new System.Drawing.Size(75, 23);
		this.simpleButton2.TabIndex = 7;
		this.simpleButton2.Text = "Cancel";
		this.grd_Allocate.AllowPasteExcel = false;
		this.grd_Allocate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Allocate.ClipboardData = null;
		this.grd_Allocate.Location = new System.Drawing.Point(-1, 0);
		this.grd_Allocate.MainView = this.gv_Allocate;
		this.grd_Allocate.Name = "grd_Allocate";
		this.grd_Allocate.Size = new System.Drawing.Size(780, 300);
		this.grd_Allocate.TabIndex = 0;
		this.grd_Allocate.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Allocate });
		this.gv_Allocate.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Allocate.ColumnKey = null;
		this.gv_Allocate.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gridColumn16, this.gridColumn17, this.gridColumn18, this.gridColumn19, this.gridColumn20, this.gridColumn21, this.gridColumn22, this.gridColumn23, this.gridColumn24 });
		this.gv_Allocate.DisableFormSetting = false;
		this.gv_Allocate.DisplayException = false;
		this.gv_Allocate.EditColumn = null;
		this.gv_Allocate.FieldCurrency = null;
		this.gv_Allocate.FieldRate = null;
		this.gv_Allocate.FormId = null;
		this.gv_Allocate.GridControl = this.grd_Allocate;
		this.gv_Allocate.IgnoreFilter = false;
		this.gv_Allocate.IndexEditRow = -1;
		this.gv_Allocate.IndicatorWidth = 35;
		this.gv_Allocate.IsApzTable = false;
		this.gv_Allocate.IsInsertNewRow = false;
		this.gv_Allocate.IsLoadingData = false;
		this.gv_Allocate.IsMappingContextAction = false;
		this.gv_Allocate.ItemId = null;
		this.gv_Allocate.LstColumnAllowBatch = null;
		this.gv_Allocate.LstColumnNotAllowBatch = null;
		this.gv_Allocate.LstColumnNotEditable = null;
		this.gv_Allocate.LstColumNotSetting = null;
		this.gv_Allocate.Name = "gv_Allocate";
		this.gv_Allocate.OptionsView.ColumnAutoWidth = false;
		this.gv_Allocate.OptionsView.ShowFooter = true;
		this.gv_Allocate.OptionsView.ShowGroupPanel = false;
		this.gv_Allocate.UseBatchMod = false;
		this.gridColumn16.Caption = "Item Code";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn16.Width = 150;
		this.gridColumn17.Caption = "Item Name";
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn17.Width = 150;
		this.gridColumn18.Caption = "Production Q'ty";
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.DisplayFormat.FormatString = "n0";
		this.gridColumn18.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "PrdQty", "{0:n3}")
		});
		this.gridColumn18.UnitField = null;
		this.gridColumn18.Width = 150;
		this.gridColumn19.Caption = "Ending Balance";
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.DisplayFormat.FormatString = "n0";
		this.gridColumn19.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "EBQty", "{0:n3}")
		});
		this.gridColumn19.UnitField = null;
		this.gridColumn19.Width = 150;
		this.gridColumn20.Caption = "Onhand Q'ty";
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.DisplayFormat.FormatString = "n0";
		this.gridColumn20.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "OnhdQty", "{0:n3}")
		});
		this.gridColumn20.UnitField = null;
		this.gridColumn20.Width = 150;
		this.gridColumn21.Caption = "Allocate to Stock";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.DisplayFormat.FormatString = "n0";
		this.gridColumn21.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "ItmVal", "{0:n3}")
		});
		this.gridColumn21.UnitField = null;
		this.gridColumn21.Width = 150;
		this.gridColumn22.Caption = "COGS (Period)";
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.DisplayFormat.FormatString = "n0";
		this.gridColumn22.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Cogs1", "{0:n3}")
		});
		this.gridColumn22.UnitField = null;
		this.gridColumn22.Width = 150;
		this.gridColumn23.Caption = "COGS (Next Period)";
		this.gridColumn23.CurrencyDateField = null;
		this.gridColumn23.CurrencyField = null;
		this.gridColumn23.CurrencyFixed = null;
		this.gridColumn23.CurrencyFixedRate = 0.0;
		this.gridColumn23.CurrencyRateField = null;
		this.gridColumn23.DisplayFormat.FormatString = "n0";
		this.gridColumn23.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn23.PrimaryKey = false;
		this.gridColumn23.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Cogs2", "{0:n3}")
		});
		this.gridColumn23.UnitField = null;
		this.gridColumn23.Width = 150;
		this.gridColumn24.Caption = "Total";
		this.gridColumn24.CurrencyDateField = null;
		this.gridColumn24.CurrencyField = null;
		this.gridColumn24.CurrencyFixed = null;
		this.gridColumn24.CurrencyFixedRate = 0.0;
		this.gridColumn24.CurrencyRateField = null;
		this.gridColumn24.DisplayFormat.FormatString = "n0";
		this.gridColumn24.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn24.Name = "gridColumn24";
		this.gridColumn24.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn24.PrimaryKey = false;
		this.gridColumn24.Summary.AddRange(new DevExpress.XtraGrid.GridSummaryItem[1]
		{
			new DevExpress.XtraGrid.GridColumnSummaryItem(DevExpress.Data.SummaryItemType.Sum, "Total", "{0:n3}")
		});
		this.gridColumn24.UnitField = null;
		this.gridColumn24.Width = 150;
		this.tabpage_Revaluation.Controls.Add(this.progressBarControl1);
		this.tabpage_Revaluation.Controls.Add(this.btn_ExportPhanBoHachToanKho);
		this.tabpage_Revaluation.Controls.Add(this.btn_CancelRevaluation);
		this.tabpage_Revaluation.Controls.Add(this.btn_Revaluation);
		this.tabpage_Revaluation.Controls.Add(this.grd_Revaluation);
		this.tabpage_Revaluation.EditControl = null;
		this.tabpage_Revaluation.EditValue = "Phân bô\u0309 ha\u0323ch toa\u0301n kho";
		this.tabpage_Revaluation.FieldName = null;
		this.tabpage_Revaluation.IsSearchControl = false;
		this.tabpage_Revaluation.Name = "tabpage_Revaluation";
		this.tabpage_Revaluation.PageEnabled = false;
		this.tabpage_Revaluation.ReadOnly = false;
		this.tabpage_Revaluation.SettingInVisible = false;
		this.tabpage_Revaluation.SettingReadonly = false;
		this.tabpage_Revaluation.Size = new System.Drawing.Size(783, 337);
		this.tabpage_Revaluation.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_Revaluation.Text = "Phân bô\u0309 ha\u0323ch toa\u0301n kho";
		this.progressBarControl1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.progressBarControl1.Location = new System.Drawing.Point(488, 307);
		this.progressBarControl1.Name = "progressBarControl1";
		this.progressBarControl1.Size = new System.Drawing.Size(100, 18);
		this.progressBarControl1.TabIndex = 10;
		this.btn_ExportPhanBoHachToanKho.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_ExportPhanBoHachToanKho.EditControl = null;
		this.btn_ExportPhanBoHachToanKho.EditValue = "Export";
		this.btn_ExportPhanBoHachToanKho.FieldName = null;
		this.btn_ExportPhanBoHachToanKho.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ExportPhanBoHachToanKho.ImageOptions.Image");
		this.btn_ExportPhanBoHachToanKho.IsSearchControl = false;
		this.btn_ExportPhanBoHachToanKho.Location = new System.Drawing.Point(3, 305);
		this.btn_ExportPhanBoHachToanKho.Name = "btn_ExportPhanBoHachToanKho";
		this.btn_ExportPhanBoHachToanKho.ReadOnly = false;
		this.btn_ExportPhanBoHachToanKho.SettingInVisible = false;
		this.btn_ExportPhanBoHachToanKho.SettingReadonly = false;
		this.btn_ExportPhanBoHachToanKho.Size = new System.Drawing.Size(75, 23);
		this.btn_ExportPhanBoHachToanKho.TabIndex = 9;
		this.btn_ExportPhanBoHachToanKho.Text = "Export";
		this.btn_ExportPhanBoHachToanKho.Click += new System.EventHandler(this.btn_ExportPhanBoHachToanKho_Click);
		this.btn_CancelRevaluation.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CancelRevaluation.EditControl = null;
		this.btn_CancelRevaluation.EditValue = "Cancel";
		this.btn_CancelRevaluation.FieldName = null;
		this.btn_CancelRevaluation.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CancelRevaluation.ImageOptions.Image");
		this.btn_CancelRevaluation.IsSearchControl = false;
		this.btn_CancelRevaluation.Location = new System.Drawing.Point(592, 305);
		this.btn_CancelRevaluation.Name = "btn_CancelRevaluation";
		this.btn_CancelRevaluation.ReadOnly = false;
		this.btn_CancelRevaluation.SettingInVisible = false;
		this.btn_CancelRevaluation.SettingReadonly = false;
		this.btn_CancelRevaluation.Size = new System.Drawing.Size(75, 23);
		this.btn_CancelRevaluation.TabIndex = 2;
		this.btn_CancelRevaluation.Text = "Cancel";
		this.btn_CancelRevaluation.Click += new System.EventHandler(this.btn_CancelRevaluation_Click);
		this.btn_Revaluation.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Revaluation.EditControl = null;
		this.btn_Revaluation.EditValue = "Revaluation";
		this.btn_Revaluation.FieldName = null;
		this.btn_Revaluation.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Revaluation.ImageOptions.Image");
		this.btn_Revaluation.IsSearchControl = false;
		this.btn_Revaluation.Location = new System.Drawing.Point(671, 305);
		this.btn_Revaluation.Name = "btn_Revaluation";
		this.btn_Revaluation.ReadOnly = false;
		this.btn_Revaluation.SettingInVisible = false;
		this.btn_Revaluation.SettingReadonly = false;
		this.btn_Revaluation.Size = new System.Drawing.Size(100, 23);
		this.btn_Revaluation.TabIndex = 1;
		this.btn_Revaluation.Text = "Revaluation";
		this.btn_Revaluation.Click += new System.EventHandler(this.btn_Revaluation_Click);
		this.grd_Revaluation.AllowPasteExcel = false;
		this.grd_Revaluation.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Revaluation.ClipboardData = null;
		this.grd_Revaluation.Location = new System.Drawing.Point(0, 0);
		this.grd_Revaluation.MainView = this.gv_Revaluation;
		this.grd_Revaluation.Name = "grd_Revaluation";
		this.grd_Revaluation.Size = new System.Drawing.Size(779, 299);
		this.grd_Revaluation.TabIndex = 0;
		this.grd_Revaluation.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Revaluation });
		this.gv_Revaluation.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Revaluation.ColumnKey = null;
		this.gv_Revaluation.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn15 });
		this.gv_Revaluation.DisableFormSetting = false;
		this.gv_Revaluation.DisplayException = false;
		this.gv_Revaluation.EditColumn = null;
		this.gv_Revaluation.FieldCurrency = null;
		this.gv_Revaluation.FieldRate = null;
		this.gv_Revaluation.FormId = null;
		this.gv_Revaluation.GridControl = this.grd_Revaluation;
		this.gv_Revaluation.IgnoreFilter = false;
		this.gv_Revaluation.IndexEditRow = -1;
		this.gv_Revaluation.IndicatorWidth = 35;
		this.gv_Revaluation.IsApzTable = false;
		this.gv_Revaluation.IsInsertNewRow = false;
		this.gv_Revaluation.IsLoadingData = false;
		this.gv_Revaluation.IsMappingContextAction = false;
		this.gv_Revaluation.ItemId = null;
		this.gv_Revaluation.LstColumnAllowBatch = null;
		this.gv_Revaluation.LstColumnNotAllowBatch = null;
		this.gv_Revaluation.LstColumnNotEditable = null;
		this.gv_Revaluation.LstColumNotSetting = null;
		this.gv_Revaluation.Name = "gv_Revaluation";
		this.gv_Revaluation.OptionsView.ColumnAutoWidth = false;
		this.gv_Revaluation.OptionsView.ShowGroupPanel = false;
		this.gv_Revaluation.UseBatchMod = false;
		this.gridColumn1.Caption = "Item Code";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ItmCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 150;
		this.gridColumn2.Caption = "Item Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "ItmName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 150;
		this.gridColumn3.Caption = "Whs";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "WhsCode";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn3.Width = 150;
		this.gridColumn4.Caption = "Qty";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.DisplayFormat.FormatString = "n0";
		this.gridColumn4.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn4.FieldName = "Qty";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn4.Width = 150;
		this.gridColumn5.Caption = "Amount";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.DisplayFormat.FormatString = "n0";
		this.gridColumn5.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn5.FieldName = "Amount";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 4;
		this.gridColumn5.Width = 150;
		this.gridColumn6.Caption = "Deb. Account";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "DebAcct";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 5;
		this.gridColumn6.Width = 150;
		this.gridColumn15.Caption = "Cred. Acct";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "CredAcct";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn15.Visible = true;
		this.gridColumn15.VisibleIndex = 6;
		this.tabpage_COGS.Controls.Add(this.btn_ExportKCGV);
		this.tabpage_COGS.Controls.Add(this.btn_CancelKCGV);
		this.tabpage_COGS.Controls.Add(this.btn_Add);
		this.tabpage_COGS.Controls.Add(this.grd_KCGV);
		this.tabpage_COGS.EditControl = null;
		this.tabpage_COGS.EditValue = "Kê\u0301t chuyê\u0309n gia\u0301 vô\u0301n";
		this.tabpage_COGS.FieldName = null;
		this.tabpage_COGS.IsSearchControl = false;
		this.tabpage_COGS.Name = "tabpage_COGS";
		this.tabpage_COGS.PageEnabled = false;
		this.tabpage_COGS.ReadOnly = false;
		this.tabpage_COGS.SettingInVisible = false;
		this.tabpage_COGS.SettingReadonly = false;
		this.tabpage_COGS.Size = new System.Drawing.Size(783, 337);
		this.tabpage_COGS.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_COGS.Text = "Kê\u0301t chuyê\u0309n gia\u0301 vô\u0301n";
		this.tabpage_COGS.Paint += new System.Windows.Forms.PaintEventHandler(this.tabpage_COGS_Paint);
		this.btn_ExportKCGV.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_ExportKCGV.EditControl = null;
		this.btn_ExportKCGV.EditValue = "Export";
		this.btn_ExportKCGV.FieldName = null;
		this.btn_ExportKCGV.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ExportKCGV.ImageOptions.Image");
		this.btn_ExportKCGV.IsSearchControl = false;
		this.btn_ExportKCGV.Location = new System.Drawing.Point(1, 306);
		this.btn_ExportKCGV.Name = "btn_ExportKCGV";
		this.btn_ExportKCGV.ReadOnly = false;
		this.btn_ExportKCGV.SettingInVisible = false;
		this.btn_ExportKCGV.SettingReadonly = false;
		this.btn_ExportKCGV.Size = new System.Drawing.Size(75, 23);
		this.btn_ExportKCGV.TabIndex = 10;
		this.btn_ExportKCGV.Text = "Export";
		this.btn_ExportKCGV.Click += new System.EventHandler(this.btn_ExportKCGV_Click);
		this.btn_CancelKCGV.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CancelKCGV.EditControl = null;
		this.btn_CancelKCGV.EditValue = "Cancel";
		this.btn_CancelKCGV.FieldName = null;
		this.btn_CancelKCGV.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CancelKCGV.ImageOptions.Image");
		this.btn_CancelKCGV.IsSearchControl = false;
		this.btn_CancelKCGV.Location = new System.Drawing.Point(598, 306);
		this.btn_CancelKCGV.Name = "btn_CancelKCGV";
		this.btn_CancelKCGV.ReadOnly = false;
		this.btn_CancelKCGV.SettingInVisible = false;
		this.btn_CancelKCGV.SettingReadonly = false;
		this.btn_CancelKCGV.Size = new System.Drawing.Size(75, 23);
		this.btn_CancelKCGV.TabIndex = 4;
		this.btn_CancelKCGV.Text = "Cancel";
		this.btn_CancelKCGV.Click += new System.EventHandler(this.btn_CancelKCGV_Click);
		this.btn_Add.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Add.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btn_Add.EditControl = null;
		this.btn_Add.EditValue = "Add";
		this.btn_Add.FieldName = null;
		this.btn_Add.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Add.ImageOptions.Image");
		this.btn_Add.IsSearchControl = false;
		this.btn_Add.Location = new System.Drawing.Point(677, 306);
		this.btn_Add.Name = "btn_Add";
		this.btn_Add.ReadOnly = false;
		this.btn_Add.SettingInVisible = false;
		this.btn_Add.SettingReadonly = false;
		this.btn_Add.Size = new System.Drawing.Size(100, 23);
		this.btn_Add.TabIndex = 3;
		this.btn_Add.Text = "Add";
		this.btn_Add.Click += new System.EventHandler(this.btn_Add_Click);
		this.grd_KCGV.AllowPasteExcel = false;
		this.grd_KCGV.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_KCGV.ClipboardData = null;
		this.grd_KCGV.Location = new System.Drawing.Point(1, 0);
		this.grd_KCGV.MainView = this.gv_KCGV;
		this.grd_KCGV.Name = "grd_KCGV";
		this.grd_KCGV.Size = new System.Drawing.Size(781, 300);
		this.grd_KCGV.TabIndex = 0;
		this.grd_KCGV.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_KCGV });
		this.gv_KCGV.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_KCGV.ColumnKey = null;
		this.gv_KCGV.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumn7, this.gridColumn13, this.gridColumn8, this.gridColumn9 });
		this.gv_KCGV.DisableFormSetting = false;
		this.gv_KCGV.DisplayException = false;
		this.gv_KCGV.EditColumn = null;
		this.gv_KCGV.FieldCurrency = null;
		this.gv_KCGV.FieldRate = null;
		this.gv_KCGV.FormId = null;
		this.gv_KCGV.GridControl = this.grd_KCGV;
		this.gv_KCGV.IgnoreFilter = false;
		this.gv_KCGV.IndexEditRow = -1;
		this.gv_KCGV.IndicatorWidth = 35;
		this.gv_KCGV.IsApzTable = false;
		this.gv_KCGV.IsInsertNewRow = false;
		this.gv_KCGV.IsLoadingData = false;
		this.gv_KCGV.IsMappingContextAction = false;
		this.gv_KCGV.ItemId = null;
		this.gv_KCGV.LstColumnAllowBatch = null;
		this.gv_KCGV.LstColumnNotAllowBatch = null;
		this.gv_KCGV.LstColumnNotEditable = null;
		this.gv_KCGV.LstColumNotSetting = null;
		this.gv_KCGV.Name = "gv_KCGV";
		this.gv_KCGV.OptionsView.ColumnAutoWidth = false;
		this.gv_KCGV.OptionsView.ShowGroupPanel = false;
		this.gv_KCGV.UseBatchMod = false;
		this.gridColumn7.Caption = "Account";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "AcctCode";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 0;
		this.gridColumn7.Width = 150;
		this.gridColumn13.Caption = "Account Name";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "AcctName";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 1;
		this.gridColumn13.Width = 305;
		this.gridColumn8.Caption = "Debit";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.DisplayFormat.FormatString = "n0";
		this.gridColumn8.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn8.FieldName = "Debit";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 2;
		this.gridColumn8.Width = 150;
		this.gridColumn9.Caption = "Credit";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.DisplayFormat.FormatString = "n0";
		this.gridColumn9.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn9.FieldName = "Credit";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 3;
		this.gridColumn9.Width = 150;
		this.tabpage_KCSX.Controls.Add(this.btn_ExportCPSX);
		this.tabpage_KCSX.Controls.Add(this.btn_CancelKCSX);
		this.tabpage_KCSX.Controls.Add(this.btn_AddKCSX);
		this.tabpage_KCSX.Controls.Add(this.grd_KCSX);
		this.tabpage_KCSX.EditControl = null;
		this.tabpage_KCSX.EditValue = "Kê\u0301t chuyê\u0309n chi phi\u0301 sa\u0309n xuâ\u0301t";
		this.tabpage_KCSX.FieldName = null;
		this.tabpage_KCSX.IsSearchControl = false;
		this.tabpage_KCSX.Name = "tabpage_KCSX";
		this.tabpage_KCSX.PageEnabled = false;
		this.tabpage_KCSX.ReadOnly = false;
		this.tabpage_KCSX.SettingInVisible = false;
		this.tabpage_KCSX.SettingReadonly = false;
		this.tabpage_KCSX.Size = new System.Drawing.Size(783, 337);
		this.tabpage_KCSX.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabpage_KCSX.Text = "Kê\u0301t chuyê\u0309n chi phi\u0301 sa\u0309n xuâ\u0301t";
		this.btn_ExportCPSX.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_ExportCPSX.EditControl = null;
		this.btn_ExportCPSX.EditValue = "Export";
		this.btn_ExportCPSX.FieldName = null;
		this.btn_ExportCPSX.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ExportCPSX.ImageOptions.Image");
		this.btn_ExportCPSX.IsSearchControl = false;
		this.btn_ExportCPSX.Location = new System.Drawing.Point(0, 308);
		this.btn_ExportCPSX.Name = "btn_ExportCPSX";
		this.btn_ExportCPSX.ReadOnly = false;
		this.btn_ExportCPSX.SettingInVisible = false;
		this.btn_ExportCPSX.SettingReadonly = false;
		this.btn_ExportCPSX.Size = new System.Drawing.Size(75, 23);
		this.btn_ExportCPSX.TabIndex = 8;
		this.btn_ExportCPSX.Text = "Export";
		this.btn_ExportCPSX.Click += new System.EventHandler(this.btn_ExportCPSX_Click);
		this.btn_CancelKCSX.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_CancelKCSX.EditControl = null;
		this.btn_CancelKCSX.EditValue = "Cancel";
		this.btn_CancelKCSX.FieldName = null;
		this.btn_CancelKCSX.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_CancelKCSX.ImageOptions.Image");
		this.btn_CancelKCSX.IsSearchControl = false;
		this.btn_CancelKCSX.Location = new System.Drawing.Point(599, 308);
		this.btn_CancelKCSX.Name = "btn_CancelKCSX";
		this.btn_CancelKCSX.ReadOnly = false;
		this.btn_CancelKCSX.SettingInVisible = false;
		this.btn_CancelKCSX.SettingReadonly = false;
		this.btn_CancelKCSX.Size = new System.Drawing.Size(75, 23);
		this.btn_CancelKCSX.TabIndex = 7;
		this.btn_CancelKCSX.Text = "Cancel";
		this.btn_CancelKCSX.Click += new System.EventHandler(this.btn_CancelKCSX_Click);
		this.btn_AddKCSX.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_AddKCSX.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btn_AddKCSX.EditControl = null;
		this.btn_AddKCSX.EditValue = "Add";
		this.btn_AddKCSX.FieldName = null;
		this.btn_AddKCSX.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_AddKCSX.ImageOptions.Image");
		this.btn_AddKCSX.IsSearchControl = false;
		this.btn_AddKCSX.Location = new System.Drawing.Point(678, 308);
		this.btn_AddKCSX.Name = "btn_AddKCSX";
		this.btn_AddKCSX.ReadOnly = false;
		this.btn_AddKCSX.SettingInVisible = false;
		this.btn_AddKCSX.SettingReadonly = false;
		this.btn_AddKCSX.Size = new System.Drawing.Size(100, 23);
		this.btn_AddKCSX.TabIndex = 6;
		this.btn_AddKCSX.Text = "Add";
		this.btn_AddKCSX.Click += new System.EventHandler(this.btn_AddKCSX_Click);
		this.grd_KCSX.AllowPasteExcel = false;
		this.grd_KCSX.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_KCSX.ClipboardData = null;
		this.grd_KCSX.Location = new System.Drawing.Point(0, 0);
		this.grd_KCSX.MainView = this.gv_KCSX;
		this.grd_KCSX.Name = "grd_KCSX";
		this.grd_KCSX.Size = new System.Drawing.Size(779, 302);
		this.grd_KCSX.TabIndex = 5;
		this.grd_KCSX.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_KCSX });
		this.gv_KCSX.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_KCSX.ColumnKey = null;
		this.gv_KCSX.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumn10, this.gridColumn14, this.gridColumn11, this.gridColumn12 });
		this.gv_KCSX.DisableFormSetting = false;
		this.gv_KCSX.DisplayException = false;
		this.gv_KCSX.EditColumn = null;
		this.gv_KCSX.FieldCurrency = null;
		this.gv_KCSX.FieldRate = null;
		this.gv_KCSX.FormId = null;
		this.gv_KCSX.GridControl = this.grd_KCSX;
		this.gv_KCSX.IgnoreFilter = false;
		this.gv_KCSX.IndexEditRow = -1;
		this.gv_KCSX.IndicatorWidth = 35;
		this.gv_KCSX.IsApzTable = false;
		this.gv_KCSX.IsInsertNewRow = false;
		this.gv_KCSX.IsLoadingData = false;
		this.gv_KCSX.IsMappingContextAction = false;
		this.gv_KCSX.ItemId = null;
		this.gv_KCSX.LstColumnAllowBatch = null;
		this.gv_KCSX.LstColumnNotAllowBatch = null;
		this.gv_KCSX.LstColumnNotEditable = null;
		this.gv_KCSX.LstColumNotSetting = null;
		this.gv_KCSX.Name = "gv_KCSX";
		this.gv_KCSX.OptionsView.ColumnAutoWidth = false;
		this.gv_KCSX.OptionsView.ShowGroupPanel = false;
		this.gv_KCSX.UseBatchMod = false;
		this.gridColumn10.Caption = "Account";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "AcctCode";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 0;
		this.gridColumn10.Width = 150;
		this.gridColumn14.Caption = "Account Name";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "AcctName";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 1;
		this.gridColumn14.Width = 307;
		this.gridColumn11.Caption = "Debit";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.DisplayFormat.FormatString = "n0";
		this.gridColumn11.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn11.FieldName = "Debit";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 2;
		this.gridColumn11.Width = 150;
		this.gridColumn12.Caption = "Credit";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.DisplayFormat.FormatString = "n0";
		this.gridColumn12.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gridColumn12.FieldName = "Credit";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 3;
		this.gridColumn12.Width = 150;
		this.bgw_Costing.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_Costing_DoWork);
		this.bgw_Costing.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_Costing_ProgressChanged);
		this.bgw_Costing.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_Costing_RunWorkerCompleted);
		this.bgw_Allocate.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_Allocate_DoWork);
		this.bgw_Allocate.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_Allocate_ProgressChanged);
		this.bgw_Allocate.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_Allocate_RunWorkerCompleted);
		this.bgw_Revaluation.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_Revaluation_DoWork);
		this.bgw_Revaluation.ProgressChanged += new System.ComponentModel.ProgressChangedEventHandler(this.bgw_Revaluation_ProgressChanged);
		this.bgw_Revaluation.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_Revaluation_RunWorkerCompleted);
		this.bgw_Allocation_ToRevaluation.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_Allocation_ToRevaluation_DoWork);
		this.bgw_Allocation_ToRevaluation.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_Allocation_ToRevaluation_RunWorkerCompleted);
		this.bgw_KCGV.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_KCGV_DoWork);
		this.bgw_KCGV.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_KCGV_RunWorkerCompleted);
		this.bgw_KCSX.DoWork += new System.ComponentModel.DoWorkEventHandler(this.bgw_KCSX_DoWork);
		this.bgw_KCSX.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.bgw_KCSX_RunWorkerCompleted);
		this.cb_Period.EditControl = null;
		this.cb_Period.FieldName = null;
		this.cb_Period.IsSearchControl = false;
		this.cb_Period.Location = new System.Drawing.Point(77, 2);
		this.cb_Period.Name = "cb_Period";
		this.cb_Period.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_Period.SettingInVisible = false;
		this.cb_Period.SettingReadonly = false;
		this.cb_Period.Size = new System.Drawing.Size(151, 20);
		this.cb_Period.TabIndex = 4;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(786, 392);
		base.Controls.Add(this.cb_Period);
		base.Controls.Add(this.tab_Main);
		base.Controls.Add(this.btn_Run);
		base.Controls.Add(this.labelLine1);
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_Costing.IconOptions.Icon");
		base.Name = "frm_Costing";
		this.Text = "Ti\u0301nh gia\u0301 tha\u0300nh";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Costing_FormClosing);
		base.Load += new System.EventHandler(this.frm_Costing_Load);
		((System.ComponentModel.ISupportInitialize)this.tab_Main).EndInit();
		this.tab_Main.ResumeLayout(false);
		this.tabpage_Costing.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Costing).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bgv_Costing).EndInit();
		this.tabpage_CostingAllocate.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Allocate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Allocate).EndInit();
		this.tabpage_Revaluation.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.progressBarControl1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Revaluation).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Revaluation).EndInit();
		this.tabpage_COGS.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_KCGV).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_KCGV).EndInit();
		this.tabpage_KCSX.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_KCSX).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_KCSX).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Period.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
