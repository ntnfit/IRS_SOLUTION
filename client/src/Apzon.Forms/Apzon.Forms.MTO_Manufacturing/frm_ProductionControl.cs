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
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Production;
using Apzon.Entities.Models.Production_Addon;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Sales;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_ProductionControl : XtraFormEx
{
	private bool _edit;

	private static int[] nums;

	private static bool status;

	private static string ids;

	private IContainer components;

	private PanelControl panelControl1;

	private DateEditEx dtToDate;

	private DateEditEx dtFromDate;

	private SimpleButtonEx btSearch;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private ButtonEditEx txtCardCode;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private RepositoryItemCheckEdit repchecked;

	private ButtonEditEx txtCardName;

	private GridViewEx gridView1;

	private GridViewEx gridView2;

	private GridViewEx gridView3;

	private SimpleButtonEx btSave;

	private SimpleButtonEx btCancel;

	private GridControlEx grd_MO;

	private GridViewEx gv_MO;

	private SimpleButtonEx btn_TinhGiay;

	public frm_ProductionControl()
	{
		InitializeComponent();
		if (Configuration.MoPlanning == "0")
		{
			btn_TinhGiay.Show();
		}
		else
		{
			btn_TinhGiay.Hide();
		}
	}

	private void frm_ProductionControl_Load(object sender, EventArgs e)
	{
		try
		{
			FormLoad();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void GetDocsDelegate()
	{
		FormLoad();
	}

	public void DelegateFillBatch(string batchs)
	{
		try
		{
			gv_MO.SetRowCellValue(gv_MO.FocusedRowHandle, "POBatch", batchs);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void FormLoad()
	{
		_ = 2;
		try
		{
			string fromdate = null;
			string todate = null;
			if (!string.IsNullOrEmpty(dtFromDate.Text))
			{
				fromdate = Function.ParseDateTimes(dtFromDate.Text).ToString("yyyy-MM-dd");
			}
			if (!string.IsNullOrEmpty(dtToDate.Text))
			{
				todate = Function.ParseDateTimes(dtToDate.Text).ToString("yyyy-MM-dd");
			}
			DataSet dataSet = await GetDocs(txtCardCode.Text, txtCardName.Text, fromdate, todate);
			if (dataSet.IsHasStructure())
			{
				if (gv_MO.Columns.Count > 0)
				{
					grd_MO.DataSource = dataSet.Tables[1];
					grd_MO.RefreshDataSource();
				}
				else if (dataSet.IsHasValidValue())
				{
					await GridViewHelper.GenerateGrid(grd_MO, gv_MO, dataSet.Tables[0], dataSet.Tables[1], dataSet.Tables[2]);
				}
				else
				{
					await GridViewHelper.GenerateGrid(grd_MO, gv_MO, dataSet.Tables[0], dataSet.Tables[1]);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataSet> GetDocs(string cardcode, string cardname, string fromdate, string todate)
	{
		try
		{
			using HttpClientHelper<DataSet> _helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "ProductionControl");
			return await _helper.GetAsync("GetDocs", new { cardcode, cardname, fromdate, todate });
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataSet();
		}
	}

	private async void Pro()
	{
		try
		{
			DataTable dataTable = gv_MO.DataSource as DataTable;
			nums = gv_MO.GetSelectedRows();
			if (nums.Count() > 0)
			{
				List<DataRow> list = new List<DataRow>();
				for (int i = 0; i < nums.Count(); i++)
				{
					DataRow item = dataTable.Rows[nums[i]];
					list.Add(item);
				}
				frm_ProductionPlanning frm_ProductionPlanning2 = new frm_ProductionPlanning(list.CopyToDataTable());
				frm_ProductionPlanning2.MdiParent = MdiParent;
				frm_ProductionPlanning2.PcClose = ReloadFromPC;
				frm_ProductionPlanning2.Show();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task<DataTable> GetDocsTabPlaned(ProductionOder_Impl_OWOR po)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionControl");
			DataTable dataTable = await _helper.PostAsync("GetDocsTabPlaned", po);
			if (dataTable == null)
			{
				return new DataTable();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void btSave_Click(object sender, EventArgs e)
	{
		try
		{
			Pro();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ReloadFromPC()
	{
		FormLoad();
	}

	private async Task<int> GetMaxMOCode()
	{
		try
		{
			using HttpClientHelper<ProductionPlanningImpl> _helper = new HttpClientHelper<ProductionPlanningImpl>(Configuration.ApiServer, "ProductionPlanning");
			return await _helper.GetIntAsync("GetMaxMOCode");
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	private async Task<DataTable> GetDocsWhenProPlan(string ids)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ProductionControl");
			return await _helper.PostAsync("GetDocsWhenProPlan", ids);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return null;
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		btCancel.Enabled = true;
	}

	private void frm_ProductionControl_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_edit)
			{
				if (MessageBox.Show(this, "Are you sure?", "Do you still want ... ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					e.Cancel = true;
				}
				_edit = false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_TinhGiay_Click(object sender, EventArgs e)
	{
		try
		{
			TinhGiay();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "  FAILED ", ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_MO_CustomDrawRowIndicator_1(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void txtCardCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Customer, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			ButtonEdit buttonEdit = sender as ButtonEdit;
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItems[0];
				txtCardCode.Text = businessPartnersImpl.CardCode;
				txtCardName.Text = businessPartnersImpl.CardName;
				buttonEdit.EditValue = businessPartnersImpl.CardCode;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txtCardName_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Customer, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			ButtonEdit buttonEdit = sender as ButtonEdit;
			if (frm_ChooseFromList2.ChooseItems != null && frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				BusinessPartnersImpl businessPartnersImpl = (BusinessPartnersImpl)frm_ChooseFromList2.ChooseItems[0];
				txtCardCode.Text = businessPartnersImpl.CardCode;
				txtCardName.Text = businessPartnersImpl.CardName;
				buttonEdit.EditValue = businessPartnersImpl.CardName;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btSearch_Click(object sender, EventArgs e)
	{
		try
		{
			FormLoad();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void TinhGiay()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("DocEntry", typeof(int));
			dataTable.Columns.Add("LineId", typeof(int));
			dataTable.Columns.Add("VisOrder", typeof(int));
			dataTable.Columns.Add("Object", typeof(string));
			dataTable.Columns.Add("LogInst", typeof(int));
			dataTable.Columns.Add("U_SoNo", typeof(int));
			dataTable.Columns.Add("U_SoLine", typeof(int));
			dataTable.Columns.Add("U_ItemCode", typeof(string));
			dataTable.Columns.Add("U_ItemName", typeof(string));
			dataTable.Columns.Add("U_Width", typeof(decimal));
			dataTable.Columns.Add("U_Length", typeof(decimal));
			dataTable.Columns.Add("U_QtyUoM", typeof(int));
			dataTable.Columns.Add("U_UnitUoM", typeof(string));
			dataTable.Columns.Add("U_Quantity", typeof(decimal));
			dataTable.Columns.Add("U_ReqDel", typeof(string));
			dataTable.Columns.Add("U_YCGC", typeof(int));
			dataTable.AcceptChanges();
			for (int i = 0; i < gv_MO.GetSelectedRows().Length; i++)
			{
				int rowHandle = gv_MO.GetSelectedRows()[i];
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = 0;
				dataRow["LineId"] = 0;
				dataRow["VisOrder"] = 0;
				dataRow["Object"] = "";
				dataRow["LogInst"] = 0;
				dataRow["U_SoNo"] = gv_MO.GetRowCellValue(rowHandle, "DocEntry");
				dataRow["U_SoLine"] = gv_MO.GetRowCellValue(rowHandle, "LineNum");
				dataRow["U_ItemCode"] = gv_MO.GetRowCellValue(rowHandle, "ItemCode");
				dataRow["U_ItemName"] = gv_MO.GetRowCellValue(rowHandle, "ItemName");
				dataRow["U_Width"] = gv_MO.GetRowCellValue(rowHandle, "Width");
				dataRow["U_Length"] = gv_MO.GetRowCellValue(rowHandle, "Length");
				dataRow["U_QtyUoM"] = 0;
				dataRow["U_UnitUoM"] = gv_MO.GetRowCellValue(rowHandle, "UoM");
				dataRow["U_Quantity"] = gv_MO.GetRowCellValue(rowHandle, "Quantity");
				dataRow["U_ReqDel"] = gv_MO.GetRowCellValue(rowHandle, "Priorities");
				dataRow["U_YCGC"] = gv_MO.GetRowCellValue(rowHandle, "YCGC");
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			frm_ChooseBatchForQuotation frm_ChooseBatchForQuotation2 = new frm_ChooseBatchForQuotation(dataTable);
			frm_ChooseBatchForQuotation2.MdiParent = MdiParent;
			frm_ChooseBatchForQuotation2.DelegateFillBatch = DelegateFillBatch;
			frm_ChooseBatchForQuotation2.Show();
		}
		catch (Exception ex)
		{
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_ProductionControl));
		this.repchecked = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.dtToDate = new Apzon.Controls.Editor.DateEditEx();
		this.dtFromDate = new Apzon.Controls.Editor.DateEditEx();
		this.btSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txtCardCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.txtCardName = new Apzon.Controls.Editor.ButtonEditEx();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.gridView2 = new Apzon.Controls.Grid.GridViewEx();
		this.gridView3 = new Apzon.Controls.Grid.GridViewEx();
		this.btSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_MO = new Apzon.Controls.Grid.GridControlEx();
		this.gv_MO = new Apzon.Controls.Grid.GridViewEx();
		this.btn_TinhGiay = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repchecked).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dtToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtFromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dtFromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_MO).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MO).BeginInit();
		base.SuspendLayout();
		this.repchecked.AutoHeight = false;
		this.repchecked.Name = "repchecked";
		this.panelControl1.Controls.Add(this.dtToDate);
		this.panelControl1.Controls.Add(this.dtFromDate);
		this.panelControl1.Controls.Add(this.btSearch);
		this.panelControl1.Controls.Add(this.labelLine3);
		this.panelControl1.Controls.Add(this.labelLine4);
		this.panelControl1.Controls.Add(this.txtCardCode);
		this.panelControl1.Controls.Add(this.labelLine2);
		this.panelControl1.Controls.Add(this.labelLine1);
		this.panelControl1.Controls.Add(this.txtCardName);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Top;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(787, 53);
		this.panelControl1.TabIndex = 66;
		this.dtToDate.EditControl = null;
		this.dtToDate.EditValue = null;
		this.dtToDate.FieldName = null;
		this.dtToDate.Location = new System.Drawing.Point(447, 28);
		this.dtToDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.dtToDate.Name = "dtToDate";
		this.dtToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtToDate.Properties.IsSearchControl = false;
		this.dtToDate.SettingInVisible = false;
		this.dtToDate.SettingReadonly = false;
		this.dtToDate.Size = new System.Drawing.Size(147, 20);
		this.dtToDate.TabIndex = 69;
		this.dtFromDate.EditControl = null;
		this.dtFromDate.EditValue = null;
		this.dtFromDate.FieldName = null;
		this.dtFromDate.Location = new System.Drawing.Point(447, 3);
		this.dtFromDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.dtFromDate.Name = "dtFromDate";
		this.dtFromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtFromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dtFromDate.Properties.IsSearchControl = false;
		this.dtFromDate.SettingInVisible = false;
		this.dtFromDate.SettingReadonly = false;
		this.dtFromDate.Size = new System.Drawing.Size(147, 20);
		this.dtFromDate.TabIndex = 68;
		this.btSearch.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSearch.Appearance.Options.UseFont = true;
		this.btSearch.EditControl = null;
		this.btSearch.EditValue = "Search";
		this.btSearch.FieldName = null;
		this.btSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSearch.ImageOptions.Image");
		this.btSearch.IsSearchControl = false;
		this.btSearch.Location = new System.Drawing.Point(615, 27);
		this.btSearch.Name = "btSearch";
		this.btSearch.ReadOnly = false;
		this.btSearch.SettingInVisible = false;
		this.btSearch.SettingReadonly = false;
		this.btSearch.Size = new System.Drawing.Size(99, 19);
		this.btSearch.TabIndex = 66;
		this.btSearch.Text = "Search";
		this.btSearch.Click += new System.EventHandler(this.btSearch_Click);
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "To Date";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(336, 32);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(114, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 64;
		this.labelLine3.Utext = "To Date";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "From Date";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(336, 7);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(114, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 63;
		this.labelLine4.Utext = "From Date";
		this.txtCardCode.EditControl = null;
		this.txtCardCode.FieldName = null;
		this.txtCardCode.IsSearchControl = false;
		this.txtCardCode.Location = new System.Drawing.Point(127, 4);
		this.txtCardCode.Name = "txtCardCode";
		this.txtCardCode.Properties.Appearance.Font = new System.Drawing.Font("Tahoma", 6.5f);
		this.txtCardCode.Properties.Appearance.Options.UseFont = true;
		this.txtCardCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txtCardCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCardCode_Properties_ButtonClick);
		this.txtCardCode.SettingInVisible = false;
		this.txtCardCode.SettingReadonly = false;
		this.txtCardCode.Size = new System.Drawing.Size(147, 18);
		this.txtCardCode.TabIndex = 52;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Customer Name";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 31);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(117, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 50;
		this.labelLine2.Utext = "Customer Name";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Customer Code";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 6);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(117, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 49;
		this.labelLine1.Utext = "Customer Code";
		this.txtCardName.EditControl = null;
		this.txtCardName.FieldName = null;
		this.txtCardName.IsSearchControl = false;
		this.txtCardName.Location = new System.Drawing.Point(127, 27);
		this.txtCardName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.txtCardName.Name = "txtCardName";
		this.txtCardName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txtCardName.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtCardName_Properties_ButtonClick);
		this.txtCardName.SettingInVisible = false;
		this.txtCardName.SettingReadonly = false;
		this.txtCardName.Size = new System.Drawing.Size(147, 20);
		this.txtCardName.TabIndex = 51;
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = null;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 35;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = null;
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.UseBatchMod = false;
		this.gridView2.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView2.ColumnKey = null;
		this.gridView2.DisableFormSetting = false;
		this.gridView2.DisplayException = false;
		this.gridView2.EditColumn = null;
		this.gridView2.FieldCurrency = null;
		this.gridView2.FieldRate = null;
		this.gridView2.FormId = null;
		this.gridView2.IgnoreFilter = false;
		this.gridView2.IndexEditRow = -1;
		this.gridView2.IndicatorWidth = 35;
		this.gridView2.IsApzTable = false;
		this.gridView2.IsInsertNewRow = false;
		this.gridView2.IsLoadingData = false;
		this.gridView2.IsMappingContextAction = false;
		this.gridView2.ItemId = null;
		this.gridView2.LstColumnAllowBatch = null;
		this.gridView2.LstColumnNotAllowBatch = null;
		this.gridView2.LstColumnNotEditable = null;
		this.gridView2.LstColumNotSetting = null;
		this.gridView2.Name = "gridView2";
		this.gridView2.UseBatchMod = false;
		this.gridView3.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView3.ColumnKey = null;
		this.gridView3.DisableFormSetting = false;
		this.gridView3.DisplayException = false;
		this.gridView3.EditColumn = null;
		this.gridView3.FieldCurrency = null;
		this.gridView3.FieldRate = null;
		this.gridView3.FormId = null;
		this.gridView3.IgnoreFilter = false;
		this.gridView3.IndexEditRow = -1;
		this.gridView3.IndicatorWidth = 35;
		this.gridView3.IsApzTable = false;
		this.gridView3.IsInsertNewRow = false;
		this.gridView3.IsLoadingData = false;
		this.gridView3.IsMappingContextAction = false;
		this.gridView3.ItemId = null;
		this.gridView3.LstColumnAllowBatch = null;
		this.gridView3.LstColumnNotAllowBatch = null;
		this.gridView3.LstColumnNotEditable = null;
		this.gridView3.LstColumNotSetting = null;
		this.gridView3.Name = "gridView3";
		this.gridView3.UseBatchMod = false;
		this.btSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btSave.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSave.Appearance.Options.UseFont = true;
		this.btSave.EditControl = null;
		this.btSave.EditValue = "Pro. Planning";
		this.btSave.FieldName = null;
		this.btSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSave.ImageOptions.Image");
		this.btSave.IsSearchControl = false;
		this.btSave.Location = new System.Drawing.Point(0, 440);
		this.btSave.Name = "btSave";
		this.btSave.ReadOnly = false;
		this.btSave.SettingInVisible = false;
		this.btSave.SettingReadonly = false;
		this.btSave.Size = new System.Drawing.Size(99, 19);
		this.btSave.TabIndex = 13;
		this.btSave.Text = "Pro. Planning";
		this.btSave.Click += new System.EventHandler(this.btSave_Click);
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btCancel.Appearance.Options.UseFont = true;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(105, 440);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 19);
		this.btCancel.TabIndex = 14;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.grd_MO.AllowPasteExcel = true;
		this.grd_MO.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_MO.ClipboardData = null;
		this.grd_MO.Location = new System.Drawing.Point(0, 58);
		this.grd_MO.MainView = this.gv_MO;
		this.grd_MO.Name = "grd_MO";
		this.grd_MO.Size = new System.Drawing.Size(787, 376);
		this.grd_MO.TabIndex = 67;
		this.grd_MO.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_MO });
		this.gv_MO.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_MO.ColumnKey = null;
		this.gv_MO.DisableFormSetting = false;
		this.gv_MO.DisplayException = false;
		this.gv_MO.EditColumn = null;
		this.gv_MO.FieldCurrency = null;
		this.gv_MO.FieldRate = null;
		this.gv_MO.FormId = null;
		this.gv_MO.GridControl = this.grd_MO;
		this.gv_MO.IgnoreFilter = false;
		this.gv_MO.IndexEditRow = -1;
		this.gv_MO.IndicatorWidth = 35;
		this.gv_MO.IsApzTable = false;
		this.gv_MO.IsInsertNewRow = false;
		this.gv_MO.IsLoadingData = false;
		this.gv_MO.IsMappingContextAction = false;
		this.gv_MO.ItemId = null;
		this.gv_MO.LstColumnAllowBatch = null;
		this.gv_MO.LstColumnNotAllowBatch = null;
		this.gv_MO.LstColumnNotEditable = null;
		this.gv_MO.LstColumNotSetting = null;
		this.gv_MO.Name = "gv_MO";
		this.gv_MO.OptionsCustomization.AllowGroup = false;
		this.gv_MO.OptionsSelection.MultiSelect = true;
		this.gv_MO.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_MO.OptionsView.ColumnAutoWidth = false;
		this.gv_MO.OptionsView.ShowGroupPanel = false;
		this.gv_MO.UseBatchMod = false;
		this.gv_MO.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_MO_CustomDrawRowIndicator_1);
		this.btn_TinhGiay.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_TinhGiay.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btn_TinhGiay.Appearance.Options.UseFont = true;
		this.btn_TinhGiay.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_TinhGiay.EditControl = null;
		this.btn_TinhGiay.EditValue = "Tính Giấy";
		this.btn_TinhGiay.FieldName = null;
		this.btn_TinhGiay.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_TinhGiay.ImageOptions.Image");
		this.btn_TinhGiay.IsSearchControl = false;
		this.btn_TinhGiay.Location = new System.Drawing.Point(641, 440);
		this.btn_TinhGiay.Name = "btn_TinhGiay";
		this.btn_TinhGiay.ReadOnly = false;
		this.btn_TinhGiay.SettingInVisible = false;
		this.btn_TinhGiay.SettingReadonly = false;
		this.btn_TinhGiay.Size = new System.Drawing.Size(134, 19);
		this.btn_TinhGiay.TabIndex = 68;
		this.btn_TinhGiay.Text = "Tính Giấy";
		this.btn_TinhGiay.Click += new System.EventHandler(this.btn_TinhGiay_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(787, 461);
		base.Controls.Add(this.btn_TinhGiay);
		base.Controls.Add(this.grd_MO);
		base.Controls.Add(this.panelControl1);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btSave);
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_ProductionControl";
		base.ShowInTaskbar = false;
		this.Text = "Production Control";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_ProductionControl_FormClosing);
		base.Load += new System.EventHandler(this.frm_ProductionControl_Load);
		((System.ComponentModel.ISupportInitialize)this.repchecked).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dtToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtFromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dtFromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCardName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_MO).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MO).EndInit();
		base.ResumeLayout(false);
	}
}
