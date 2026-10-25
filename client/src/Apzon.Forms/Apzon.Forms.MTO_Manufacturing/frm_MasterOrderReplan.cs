using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
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
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Production_Addon;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTO_Manufacturing;

public class frm_MasterOrderReplan : XtraFormEx
{
	private DataTable _table = new DataTable();

	private List<ProductionPlanningImpl> lstProductionPlan;

	private DateTime _FromDate;

	private DateTime _ToDate;

	private int _ColCount;

	private IContainer components;

	private DateEditEx date_ToDate;

	private DateEditEx date_FromDate;

	private LabelLine lb_ToDate;

	private LabelLine lb_FromDate;

	private SimpleButtonEx btn_Search;

	private SplitContainerControl splitContainerControl1;

	private GroupControl gr_MOReplan;

	private GridControlEx grd_Replan;

	private GridViewEx gv_Replan;

	private SimpleButtonEx btn_Save;

	private GridControlEx grd_MasterOrder;

	private GridViewEx gv_MasterOrder;

	private GridColumnEx clMONo;

	private GridColumnEx clMoName;

	private GridColumnEx clPostDate;

	private GridColumnEx clDueDate;

	private GridColumnEx clRouting;

	private GridColumnEx clBatchToIssue;

	private GridColumnEx clUoM;

	private GridColumnEx clQuantity;

	private GridColumnEx clWidth;

	private GridColumnEx clLength;

	private GridColumnEx clDistRule;

	private GridColumnEx clProject;

	private GridColumnEx clFixedBatch;

	private GridColumnEx clStatus;

	private GridColumnEx clRemark;

	private RepositoryItemSpinEdit repositoryItemSpinEdit_N0;

	public frm_MasterOrderReplan(List<ProductionPlanningImpl> lst)
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_MasterOrderReplan"))
		{
			InitializeComponent();
			SettingLanguage();
			if (lst != null && lst.Count > 0)
			{
				lstProductionPlan = lst;
			}
			else
			{
				lstProductionPlan = new List<ProductionPlanningImpl>();
			}
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_MasterOrderReplan" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Search.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void frm_MasterOrderReplan_Load(object sender, EventArgs e)
	{
		if (lstProductionPlan == null)
		{
			lstProductionPlan = new List<ProductionPlanningImpl>();
		}
		grd_MasterOrder.DataSource = lstProductionPlan;
		grd_MasterOrder.RefreshDataSource();
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		try
		{
			if (ValidateSearch())
			{
				WinCommons._OpenProcessing();
				_FromDate = (DateTime)date_FromDate.EditValue;
				_ToDate = (DateTime)date_ToDate.EditValue;
				GridView_CreateTable();
				GridView_LoadColumns();
				await GridView_LoadData();
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Replan_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void date_FromDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (date_FromDate.EditValue != null && !string.IsNullOrEmpty(date_FromDate.Text))
			{
				DateTime minValue = (DateTime)date_FromDate.EditValue;
				date_ToDate.Properties.MinValue = minValue;
			}
			else
			{
				date_ToDate.Properties.MinValue = default;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void date_ToDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (date_ToDate.EditValue != null && !string.IsNullOrEmpty(date_ToDate.Text))
			{
				DateTime maxValue = (DateTime)date_ToDate.EditValue;
				date_FromDate.Properties.MaxValue = maxValue;
			}
			else
			{
				date_FromDate.Properties.MaxValue = default;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataTable dataTable = (DataTable)grd_Replan.DataSource;
				if (dataTable == null || dataTable.Rows.Count == 0)
				{
					return;
				}
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Clear();
				dataTable2.Rows.Clear();
				dataTable2.Columns.Add("MONo", typeof(string));
				dataTable2.Columns.Add("MOName", typeof(string));
				dataTable2.Columns.Add("Uom", typeof(string));
				dataTable2.Columns.Add("TotalQty", typeof(double));
				dataTable2.Columns.Add("ReplanQty", typeof(double));
				dataTable2.Columns.Add("OpenQty", typeof(double));
				dataTable2.Columns.Add("PlanDate", typeof(DateTime));
				dataTable2.Columns.Add("QtyOnDate", typeof(double));
				dataTable2.Columns.Add("Notes", typeof(string));
				dataTable2.Columns.Add("ItemCode", typeof(string));
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					DateTime dateTime = _FromDate;
					for (int j = 1; j < _ColCount; j++)
					{
						DataRow dataRow = dataTable2.Rows.Add();
						dataRow["MONo"] = dataTable.Rows[i]["MONo"];
						dataRow["MOName"] = dataTable.Rows[i]["MOName"];
						dataRow["Uom"] = dataTable.Rows[i]["Uom"];
						dataRow["TotalQty"] = dataTable.Rows[i]["TotalQty"];
						dataRow["ReplanQty"] = dataTable.Rows[i]["ReplanQty"];
						dataRow["OpenQty"] = dataTable.Rows[i]["OpenQty"];
						dataRow["PlanDate"] = dateTime;
						dataRow["QtyOnDate"] = dataTable.Rows[i][j.ToString()];
						dataRow["Notes"] = dataTable.Rows[i]["Notes"];
						dataRow["ItemCode"] = dataTable.Rows[i]["ItemCode"];
						dateTime = dateTime.AddDays(1.0);
					}
				}
				string text = await Save(dataTable2);
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, text);
				}
				else
				{
					WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task<string> Save(DataTable dt)
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MasterOrderReplan");
			string text = await _help.PostAsJsonAsync("Save", dt);
			if (text != null && !(text == ""))
			{
				return text;
			}
			WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return ex.Message;
		}
		return null;
	}

	private void SettingLanguage()
	{
		Text = Resources.Titel_frmMasterOrderReplan;
		btn_Save.Text = Resources.ButtonSave;
		btn_Search.Text = Resources.ButtonSearch;
		lb_FromDate.Utext = Resources.Doc_lb_FromDate;
		lb_ToDate.Utext = Resources.Doc_lb_ToDate;
		clMONo.Caption = Resources.lb_MasterOrderNo;
		clMoName.Caption = Resources.lb_MasterOrderName;
		clPostDate.Caption = Resources.Doc_gvDoc_gcPostingDate;
		clDueDate.Caption = Resources.Doc_gvDoc_gcDocDueDate;
		clRouting.Caption = Resources.Titel_frmRouting;
		clBatchToIssue.Caption = Resources.lb_BatchToIssue;
		clUoM.Caption = Resources.lb_UoM;
		clQuantity.Caption = Resources.lb_Quantity;
		clWidth.Caption = Resources.lb_Width;
		clLength.Caption = Resources.lb_Length;
		clDistRule.Caption = Resources.lb_DistRule;
		clProject.Caption = Resources.lb_Project;
		clFixedBatch.Caption = Resources.lb_FixedBatch;
		clStatus.Caption = Resources.lb_Status;
		clRemark.Caption = Resources.Doc_gvDoc_Remark;
	}

	private void GridView_CreateTable()
	{
		if (_table == null)
		{
			_table = new DataTable();
		}
		_table.Columns.Clear();
		_table.Rows.Clear();
		_table.Columns.Add("MONo", typeof(string));
		_table.Columns.Add("MOName", typeof(string));
		_table.Columns.Add("ItemCode", typeof(string));
		_table.Columns.Add("Uom", typeof(string));
		_table.Columns.Add("TotalQty", typeof(double));
		_table.Columns.Add("ReplanQty", typeof(double));
		_table.Columns.Add("OpenQty", typeof(double));
		_table.Columns.Add("PlanDate", typeof(DateTime));
		_table.Columns.Add("QtyOnDate", typeof(double));
		_table.Columns.Add("Notes", typeof(string));
		int num = 1;
		DateTime dateTime = _FromDate;
		while (dateTime <= _ToDate)
		{
			_table.Columns.Add(num.ToString());
			dateTime = dateTime.AddDays(1.0);
			num++;
		}
		_ColCount = num - 1;
	}

	private void GridView_LoadColumns()
	{
		gv_Replan.Columns.Clear();
		gv_Replan.OptionsView.GroupFooterShowMode = GroupFooterShowMode.VisibleIfExpanded;
		GridColumnEx gridColumnEx = new GridColumnEx();
		gridColumnEx.Caption = "Master order No";
		gridColumnEx.Name = "cl_MONo";
		gridColumnEx.FieldName = "MONo";
		gridColumnEx.Visible = true;
		gridColumnEx.VisibleIndex = 0;
		gridColumnEx.MaxWidth = 0;
		gridColumnEx.MinWidth = 50;
		gridColumnEx.Width = 60;
		gridColumnEx.OptionsColumn.AllowEdit = false;
		gridColumnEx.Fixed = FixedStyle.Left;
		gridColumnEx.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx2.Caption = "Master order name";
		gridColumnEx2.Name = "cl_MOName";
		gridColumnEx2.FieldName = "MOName";
		gridColumnEx2.Visible = true;
		gridColumnEx2.VisibleIndex = 1;
		gridColumnEx2.MaxWidth = 0;
		gridColumnEx2.MinWidth = 150;
		gridColumnEx2.Width = 150;
		gridColumnEx2.OptionsColumn.AllowEdit = true;
		gridColumnEx2.Fixed = FixedStyle.Left;
		gridColumnEx2.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		gridColumnEx3.Caption = "ItemCode";
		gridColumnEx3.Name = "cl_ItemCode";
		gridColumnEx3.FieldName = "ItemCode";
		gridColumnEx3.Visible = true;
		gridColumnEx3.VisibleIndex = 2;
		gridColumnEx3.MaxWidth = 0;
		gridColumnEx3.MinWidth = 150;
		gridColumnEx3.Width = 150;
		gridColumnEx3.OptionsColumn.AllowEdit = true;
		gridColumnEx3.Fixed = FixedStyle.Left;
		gridColumnEx3.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		gridColumnEx4.Name = "cl_Uom";
		gridColumnEx4.FieldName = "Uom";
		gridColumnEx4.Visible = true;
		gridColumnEx4.VisibleIndex = 3;
		gridColumnEx4.MaxWidth = 0;
		gridColumnEx4.MinWidth = 50;
		gridColumnEx4.Width = 50;
		gridColumnEx4.OptionsColumn.AllowEdit = false;
		gridColumnEx4.Fixed = FixedStyle.Left;
		gridColumnEx4.OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		gridColumnEx5.Caption = "Total quantity";
		gridColumnEx5.Name = "cl_TotalQty";
		gridColumnEx5.FieldName = "TotalQty";
		gridColumnEx5.Visible = true;
		gridColumnEx5.VisibleIndex = 4;
		gridColumnEx5.MaxWidth = 0;
		gridColumnEx5.MinWidth = 80;
		gridColumnEx5.Width = 80;
		gridColumnEx5.OptionsColumn.AllowEdit = false;
		gridColumnEx5.Fixed = FixedStyle.Left;
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		gridColumnEx6.Caption = "Replan quantity";
		gridColumnEx6.Name = "cl_ReplanQty";
		gridColumnEx6.FieldName = "ReplanQty";
		gridColumnEx6.Visible = true;
		gridColumnEx6.VisibleIndex = 5;
		gridColumnEx6.MaxWidth = 0;
		gridColumnEx6.MinWidth = 80;
		gridColumnEx6.Width = 80;
		gridColumnEx6.OptionsColumn.AllowEdit = false;
		gridColumnEx6.Fixed = FixedStyle.Left;
		GridColumnEx gridColumnEx7 = new GridColumnEx();
		gridColumnEx7.Caption = "Open quantity";
		gridColumnEx7.Name = "cl_OpenQty";
		gridColumnEx7.FieldName = "OpenQty";
		gridColumnEx7.Visible = true;
		gridColumnEx7.VisibleIndex = 6;
		gridColumnEx7.MaxWidth = 0;
		gridColumnEx7.MinWidth = 80;
		gridColumnEx7.Width = 80;
		gridColumnEx7.OptionsColumn.AllowEdit = false;
		gridColumnEx7.Fixed = FixedStyle.Left;
		int num = 1;
		DateTime dateTime = _FromDate;
		while (dateTime <= _ToDate)
		{
			GridColumnEx gridColumnEx8 = new GridColumnEx();
			gridColumnEx8.Caption = dateTime.ToString("dd/MM/yyyy");
			if (dateTime.DayOfWeek == DayOfWeek.Sunday)
			{
				gridColumnEx8.AppearanceCell.BackColor = Color.Aqua;
			}
			gridColumnEx8.FieldName = num.ToString();
			gridColumnEx8.MinWidth = 40;
			gridColumnEx8.Width = 50;
			gridColumnEx8.Name = "cl_" + num;
			gridColumnEx8.OptionsColumn.AllowEdit = true;
			gridColumnEx8.OptionsColumn.AllowMove = false;
			gridColumnEx8.ColumnEdit = repositoryItemSpinEdit_N0;
			gridColumnEx8.Visible = true;
			gridColumnEx8.VisibleIndex = 6 + num;
			dateTime = dateTime.AddDays(1.0);
			num++;
		}
		GridColumnEx gridColumnEx9 = new GridColumnEx();
		gridColumnEx9.Caption = "Notes";
		gridColumnEx9.Name = "cl_Notes";
		gridColumnEx9.FieldName = "Notes";
		gridColumnEx9.Visible = true;
		gridColumnEx9.VisibleIndex = 6 + num;
		gridColumnEx9.MaxWidth = 0;
		gridColumnEx9.MinWidth = 100;
		gridColumnEx9.Width = 150;
		gridColumnEx9.OptionsColumn.AllowEdit = true;
	}

	private async Task GridView_LoadData()
	{
		try
		{
			DataTable dataTable = await Search();
			string text = string.Empty;
			_table.NewRow();
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataRow row in dataTable.Rows)
			{
				if (text.Equals(row["MONo"].ToString()))
				{
					continue;
				}
				DataRow dataRow2 = _table.NewRow();
				text = row["MONo"].ToString();
				dataRow2["MONo"] = row["MONo"];
				dataRow2["MOName"] = row["MOName"];
				dataRow2["Uom"] = row["Uom"];
				dataRow2["TotalQty"] = row["TotalQty"];
				dataRow2["ReplanQty"] = row["ReplanQty"];
				dataRow2["OpenQty"] = row["OpenQty"];
				dataRow2["ItemCode"] = row["ItemCode"];
				DateTime dateTime = _FromDate;
				int num = 1;
				while (dateTime <= _ToDate)
				{
					DataTable dataTable2 = dataTable.Select($"PlanDate = '{dateTime}'").CopyToDataTable();
					if (dataTable2 != null && dataTable2.Rows.Count > 0)
					{
						dataRow2[num.ToString()] = dataTable2.Rows[0]["QtyOnDate"];
					}
					else
					{
						dataRow2[num.ToString()] = 0;
					}
					dateTime = dateTime.AddDays(1.0);
					num++;
				}
				_table.Rows.Add(dataRow2);
			}
			grd_Replan.DataSource = _table;
			gv_Replan.BeginDataUpdate();
			GridView gridView = grd_Replan.MainView as GridView;
			_ = new GridColumnSortInfo[2]
			{
				new GridColumnSortInfo(gridView.Columns["MONo"], ColumnSortOrder.Ascending),
				new GridColumnSortInfo(gridView.Columns["MOName"], ColumnSortOrder.Ascending)
			};
			gv_Replan.EndDataUpdate();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> Search()
	{
		string text = "";
		List<ProductionPlanningImpl> list = (List<ProductionPlanningImpl>)grd_MasterOrder.DataSource;
		if (list != null && list.Count > 0)
		{
			foreach (ProductionPlanningImpl item in list)
			{
				text += $"{item.MONo},";
			}
			text.Remove(text.Length - 1, 1);
		}
		DataTable dt = new DataTable();
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("MONo", typeof(string));
		dataTable.Columns.Add("FromDate", typeof(DateTime));
		dataTable.Columns.Add("ToDate", typeof(DateTime));
		DataRow dataRow = dataTable.NewRow();
		dataRow["MONo"] = "20,21,22";
		dataRow["FromDate"] = _FromDate;
		dataRow["ToDate"] = _ToDate;
		dataTable.Rows.Add(dataRow);
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "MasterOrderReplan");
			dt = await _helper.PostDataTableAsync("Search", dataTable);
			if (dt != null && dt.Rows.Count > 0)
			{
				return dt;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	private bool isColumnDay(string _column)
	{
		try
		{
			if (int.Parse(_column.Substring(3, _column.Length - 3)) > 0)
			{
				return true;
			}
		}
		catch (Exception)
		{
			return false;
		}
		return true;
	}

	private bool ValidateSearch()
	{
		if (!Check._date(date_FromDate))
		{
			return false;
		}
		if (!Check._date(date_ToDate))
		{
			return false;
		}
		return true;
	}

	private void gv_MasterOrder_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_MasterOrder.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Replan_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_Replan.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTO_Manufacturing.frm_MasterOrderReplan));
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_MasterOrder = new Apzon.Controls.Grid.GridControlEx();
		this.gv_MasterOrder = new Apzon.Controls.Grid.GridViewEx();
		this.clMONo = new Apzon.Controls.Grid.GridColumnEx();
		this.clMoName = new Apzon.Controls.Grid.GridColumnEx();
		this.clPostDate = new Apzon.Controls.Grid.GridColumnEx();
		this.clDueDate = new Apzon.Controls.Grid.GridColumnEx();
		this.clRouting = new Apzon.Controls.Grid.GridColumnEx();
		this.clBatchToIssue = new Apzon.Controls.Grid.GridColumnEx();
		this.clUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.clQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.clWidth = new Apzon.Controls.Grid.GridColumnEx();
		this.clLength = new Apzon.Controls.Grid.GridColumnEx();
		this.clDistRule = new Apzon.Controls.Grid.GridColumnEx();
		this.clProject = new Apzon.Controls.Grid.GridColumnEx();
		this.clFixedBatch = new Apzon.Controls.Grid.GridColumnEx();
		this.clStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.clRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.gr_MOReplan = new DevExpress.XtraEditors.GroupControl();
		this.grd_Replan = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Replan = new Apzon.Controls.Grid.GridViewEx();
		this.repositoryItemSpinEdit_N0 = new DevExpress.XtraEditors.Repository.RepositoryItemSpinEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_MasterOrder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MasterOrder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gr_MOReplan).BeginInit();
		this.gr_MOReplan.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Replan).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Replan).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSpinEdit_N0).BeginInit();
		base.SuspendLayout();
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
		this.date_ToDate.Location = new System.Drawing.Point(314, 11);
		this.date_ToDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = false;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(114, 20);
		this.date_ToDate.TabIndex = 74;
		this.date_ToDate.EditValueChanged += new System.EventHandler(this.date_ToDate_EditValueChanged);
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = null;
		this.date_FromDate.Location = new System.Drawing.Point(100, 12);
		this.date_FromDate.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.IsSearchControl = false;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(114, 20);
		this.date_FromDate.TabIndex = 73;
		this.date_FromDate.EditValueChanged += new System.EventHandler(this.date_FromDate_EditValueChanged);
		this.lb_ToDate.EditControl = null;
		this.lb_ToDate.EditValue = "To Date";
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(240, 14);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ReadOnly = false;
		this.lb_ToDate.SettingInVisible = false;
		this.lb_ToDate.SettingReadonly = false;
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(77, 20);
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.TabIndex = 71;
		this.lb_ToDate.Utext = "To Date";
		this.lb_FromDate.EditControl = null;
		this.lb_FromDate.EditValue = "From Date";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(26, 15);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ReadOnly = false;
		this.lb_FromDate.SettingInVisible = false;
		this.lb_FromDate.SettingReadonly = false;
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(77, 20);
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.TabIndex = 70;
		this.lb_FromDate.Utext = "From Date";
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(457, 9);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 75;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 41);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_MasterOrder);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.gr_MOReplan);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.RightToLeft = System.Windows.Forms.RightToLeft.No;
		this.splitContainerControl1.Size = new System.Drawing.Size(761, 372);
		this.splitContainerControl1.SplitterPosition = 189;
		this.splitContainerControl1.TabIndex = 76;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grd_MasterOrder.AllowPasteExcel = true;
		this.grd_MasterOrder.ClipboardData = null;
		this.grd_MasterOrder.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_MasterOrder.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_MasterOrder.Location = new System.Drawing.Point(0, 0);
		this.grd_MasterOrder.MainView = this.gv_MasterOrder;
		this.grd_MasterOrder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_MasterOrder.Name = "grd_MasterOrder";
		this.grd_MasterOrder.Size = new System.Drawing.Size(761, 189);
		this.grd_MasterOrder.TabIndex = 1;
		this.grd_MasterOrder.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_MasterOrder });
		this.gv_MasterOrder.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_MasterOrder.ColumnKey = null;
		this.gv_MasterOrder.ColumnPanelRowHeight = 40;
		this.gv_MasterOrder.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[15]
		{
			this.clMONo, this.clMoName, this.clPostDate, this.clDueDate, this.clRouting, this.clBatchToIssue, this.clUoM, this.clQuantity, this.clWidth, this.clLength,
			this.clDistRule, this.clProject, this.clFixedBatch, this.clStatus, this.clRemark
		});
		this.gv_MasterOrder.DisableFormSetting = false;
		this.gv_MasterOrder.DisplayException = false;
		this.gv_MasterOrder.EditColumn = null;
		this.gv_MasterOrder.FieldCurrency = null;
		this.gv_MasterOrder.FieldRate = null;
		this.gv_MasterOrder.FormId = null;
		this.gv_MasterOrder.GridControl = this.grd_MasterOrder;
		this.gv_MasterOrder.IgnoreFilter = false;
		this.gv_MasterOrder.IndexEditRow = -1;
		this.gv_MasterOrder.IndicatorWidth = 30;
		this.gv_MasterOrder.IsApzTable = false;
		this.gv_MasterOrder.IsInsertNewRow = false;
		this.gv_MasterOrder.IsLoadingData = false;
		this.gv_MasterOrder.IsMappingContextAction = false;
		this.gv_MasterOrder.ItemId = null;
		this.gv_MasterOrder.LstColumnAllowBatch = null;
		this.gv_MasterOrder.LstColumnNotAllowBatch = null;
		this.gv_MasterOrder.LstColumnNotEditable = null;
		this.gv_MasterOrder.LstColumNotSetting = null;
		this.gv_MasterOrder.Name = "gv_MasterOrder";
		this.gv_MasterOrder.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_MasterOrder.OptionsView.ColumnAutoWidth = false;
		this.gv_MasterOrder.OptionsView.ShowGroupPanel = false;
		this.gv_MasterOrder.UseBatchMod = false;
		this.gv_MasterOrder.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_MasterOrder_RowStyle);
		this.clMONo.AppearanceHeader.Options.UseTextOptions = true;
		this.clMONo.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.clMONo.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.clMONo.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.clMONo.Caption = "Master Order No.";
		this.clMONo.CurrencyDateField = null;
		this.clMONo.CurrencyField = null;
		this.clMONo.CurrencyFixed = null;
		this.clMONo.CurrencyFixedRate = 0.0;
		this.clMONo.CurrencyRateField = null;
		this.clMONo.FieldName = "MONo";
		this.clMONo.Name = "clMONo";
		this.clMONo.PrimaryKey = true;
		this.clMONo.UnitField = null;
		this.clMONo.Visible = true;
		this.clMONo.VisibleIndex = 0;
		this.clMoName.AppearanceHeader.Options.UseTextOptions = true;
		this.clMoName.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.clMoName.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.clMoName.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.clMoName.Caption = "Master Order Name";
		this.clMoName.CurrencyDateField = null;
		this.clMoName.CurrencyField = null;
		this.clMoName.CurrencyFixed = null;
		this.clMoName.CurrencyFixedRate = 0.0;
		this.clMoName.CurrencyRateField = null;
		this.clMoName.FieldName = "MOName";
		this.clMoName.Name = "clMoName";
		this.clMoName.PrimaryKey = false;
		this.clMoName.UnitField = null;
		this.clMoName.Visible = true;
		this.clMoName.VisibleIndex = 1;
		this.clMoName.Width = 120;
		this.clPostDate.Caption = "Posting Date";
		this.clPostDate.CurrencyDateField = null;
		this.clPostDate.CurrencyField = null;
		this.clPostDate.CurrencyFixed = null;
		this.clPostDate.CurrencyFixedRate = 0.0;
		this.clPostDate.CurrencyRateField = null;
		this.clPostDate.FieldName = "PostDate";
		this.clPostDate.Name = "clPostDate";
		this.clPostDate.PrimaryKey = false;
		this.clPostDate.UnitField = null;
		this.clPostDate.Visible = true;
		this.clPostDate.VisibleIndex = 2;
		this.clPostDate.Width = 95;
		this.clDueDate.Caption = "Due Date";
		this.clDueDate.CurrencyDateField = null;
		this.clDueDate.CurrencyField = null;
		this.clDueDate.CurrencyFixed = null;
		this.clDueDate.CurrencyFixedRate = 0.0;
		this.clDueDate.CurrencyRateField = null;
		this.clDueDate.FieldName = "DueDate";
		this.clDueDate.Name = "clDueDate";
		this.clDueDate.PrimaryKey = false;
		this.clDueDate.UnitField = null;
		this.clDueDate.Visible = true;
		this.clDueDate.VisibleIndex = 3;
		this.clDueDate.Width = 86;
		this.clRouting.Caption = "Routing";
		this.clRouting.CurrencyDateField = null;
		this.clRouting.CurrencyField = null;
		this.clRouting.CurrencyFixed = null;
		this.clRouting.CurrencyFixedRate = 0.0;
		this.clRouting.CurrencyRateField = null;
		this.clRouting.FieldName = "RouCode";
		this.clRouting.Name = "clRouting";
		this.clRouting.PrimaryKey = false;
		this.clRouting.UnitField = null;
		this.clRouting.Visible = true;
		this.clRouting.VisibleIndex = 4;
		this.clBatchToIssue.AppearanceCell.Options.UseTextOptions = true;
		this.clBatchToIssue.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.clBatchToIssue.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.clBatchToIssue.AppearanceCell.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.clBatchToIssue.AppearanceHeader.Options.UseTextOptions = true;
		this.clBatchToIssue.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.clBatchToIssue.AppearanceHeader.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.clBatchToIssue.AppearanceHeader.TextOptions.WordWrap = DevExpress.Utils.WordWrap.Wrap;
		this.clBatchToIssue.Caption = "Batch To Issue";
		this.clBatchToIssue.CurrencyDateField = null;
		this.clBatchToIssue.CurrencyField = null;
		this.clBatchToIssue.CurrencyFixed = null;
		this.clBatchToIssue.CurrencyFixedRate = 0.0;
		this.clBatchToIssue.CurrencyRateField = null;
		this.clBatchToIssue.FieldName = "BatchToIssue";
		this.clBatchToIssue.Name = "clBatchToIssue";
		this.clBatchToIssue.PrimaryKey = false;
		this.clBatchToIssue.UnitField = null;
		this.clBatchToIssue.Visible = true;
		this.clBatchToIssue.VisibleIndex = 5;
		this.clBatchToIssue.Width = 73;
		this.clUoM.Caption = "UoM";
		this.clUoM.CurrencyDateField = null;
		this.clUoM.CurrencyField = null;
		this.clUoM.CurrencyFixed = null;
		this.clUoM.CurrencyFixedRate = 0.0;
		this.clUoM.CurrencyRateField = null;
		this.clUoM.FieldName = "UoM";
		this.clUoM.Name = "clUoM";
		this.clUoM.PrimaryKey = false;
		this.clUoM.UnitField = null;
		this.clUoM.Visible = true;
		this.clUoM.VisibleIndex = 6;
		this.clQuantity.Caption = "Quantity";
		this.clQuantity.CurrencyDateField = null;
		this.clQuantity.CurrencyField = null;
		this.clQuantity.CurrencyFixed = null;
		this.clQuantity.CurrencyFixedRate = 0.0;
		this.clQuantity.CurrencyRateField = null;
		this.clQuantity.FieldName = "Quantity";
		this.clQuantity.Name = "clQuantity";
		this.clQuantity.PrimaryKey = false;
		this.clQuantity.UnitField = null;
		this.clQuantity.Visible = true;
		this.clQuantity.VisibleIndex = 7;
		this.clWidth.Caption = "Width";
		this.clWidth.CurrencyDateField = null;
		this.clWidth.CurrencyField = null;
		this.clWidth.CurrencyFixed = null;
		this.clWidth.CurrencyFixedRate = 0.0;
		this.clWidth.CurrencyRateField = null;
		this.clWidth.FieldName = "Width";
		this.clWidth.Name = "clWidth";
		this.clWidth.PrimaryKey = false;
		this.clWidth.UnitField = null;
		this.clWidth.Visible = true;
		this.clWidth.VisibleIndex = 8;
		this.clLength.Caption = "Length";
		this.clLength.CurrencyDateField = null;
		this.clLength.CurrencyField = null;
		this.clLength.CurrencyFixed = null;
		this.clLength.CurrencyFixedRate = 0.0;
		this.clLength.CurrencyRateField = null;
		this.clLength.FieldName = "Length";
		this.clLength.Name = "clLength";
		this.clLength.PrimaryKey = false;
		this.clLength.UnitField = null;
		this.clLength.Visible = true;
		this.clLength.VisibleIndex = 9;
		this.clDistRule.Caption = "Dist. Rule";
		this.clDistRule.CurrencyDateField = null;
		this.clDistRule.CurrencyField = null;
		this.clDistRule.CurrencyFixed = null;
		this.clDistRule.CurrencyFixedRate = 0.0;
		this.clDistRule.CurrencyRateField = null;
		this.clDistRule.FieldName = "DistRule";
		this.clDistRule.Name = "clDistRule";
		this.clDistRule.PrimaryKey = false;
		this.clDistRule.UnitField = null;
		this.clDistRule.Visible = true;
		this.clDistRule.VisibleIndex = 10;
		this.clProject.Caption = "Project";
		this.clProject.CurrencyDateField = null;
		this.clProject.CurrencyField = null;
		this.clProject.CurrencyFixed = null;
		this.clProject.CurrencyFixedRate = 0.0;
		this.clProject.CurrencyRateField = null;
		this.clProject.FieldName = "Project";
		this.clProject.Name = "clProject";
		this.clProject.PrimaryKey = false;
		this.clProject.UnitField = null;
		this.clProject.Visible = true;
		this.clProject.VisibleIndex = 11;
		this.clFixedBatch.Caption = "Fixed Batch";
		this.clFixedBatch.CurrencyDateField = null;
		this.clFixedBatch.CurrencyField = null;
		this.clFixedBatch.CurrencyFixed = null;
		this.clFixedBatch.CurrencyFixedRate = 0.0;
		this.clFixedBatch.CurrencyRateField = null;
		this.clFixedBatch.FieldName = "BatchSOFixed";
		this.clFixedBatch.Name = "clFixedBatch";
		this.clFixedBatch.PrimaryKey = false;
		this.clFixedBatch.UnitField = null;
		this.clFixedBatch.Visible = true;
		this.clFixedBatch.VisibleIndex = 12;
		this.clStatus.Caption = "Status";
		this.clStatus.CurrencyDateField = null;
		this.clStatus.CurrencyField = null;
		this.clStatus.CurrencyFixed = null;
		this.clStatus.CurrencyFixedRate = 0.0;
		this.clStatus.CurrencyRateField = null;
		this.clStatus.FieldName = "Status";
		this.clStatus.Name = "clStatus";
		this.clStatus.PrimaryKey = false;
		this.clStatus.UnitField = null;
		this.clStatus.Visible = true;
		this.clStatus.VisibleIndex = 13;
		this.clRemark.Caption = "Remark";
		this.clRemark.CurrencyDateField = null;
		this.clRemark.CurrencyField = null;
		this.clRemark.CurrencyFixed = null;
		this.clRemark.CurrencyFixedRate = 0.0;
		this.clRemark.CurrencyRateField = null;
		this.clRemark.FieldName = "Remark";
		this.clRemark.Name = "clRemark";
		this.clRemark.PrimaryKey = false;
		this.clRemark.UnitField = null;
		this.clRemark.Visible = true;
		this.clRemark.VisibleIndex = 14;
		this.gr_MOReplan.Controls.Add(this.grd_Replan);
		this.gr_MOReplan.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gr_MOReplan.Location = new System.Drawing.Point(0, 0);
		this.gr_MOReplan.Name = "gr_MOReplan";
		this.gr_MOReplan.Size = new System.Drawing.Size(761, 173);
		this.gr_MOReplan.TabIndex = 0;
		this.gr_MOReplan.Text = "Master Order to Replan";
		this.grd_Replan.AllowPasteExcel = true;
		this.grd_Replan.ClipboardData = null;
		this.grd_Replan.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Replan.Location = new System.Drawing.Point(2, 23);
		this.grd_Replan.MainView = this.gv_Replan;
		this.grd_Replan.Name = "grd_Replan";
		this.grd_Replan.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemSpinEdit_N0 });
		this.grd_Replan.Size = new System.Drawing.Size(757, 148);
		this.grd_Replan.TabIndex = 1;
		this.grd_Replan.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Replan });
		this.gv_Replan.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Replan.ColumnKey = null;
		this.gv_Replan.DisableFormSetting = false;
		this.gv_Replan.DisplayException = false;
		this.gv_Replan.EditColumn = null;
		this.gv_Replan.FieldCurrency = null;
		this.gv_Replan.FieldRate = null;
		this.gv_Replan.FormId = null;
		this.gv_Replan.GridControl = this.grd_Replan;
		this.gv_Replan.IgnoreFilter = false;
		this.gv_Replan.IndexEditRow = -1;
		this.gv_Replan.IndicatorWidth = 30;
		this.gv_Replan.IsApzTable = false;
		this.gv_Replan.IsInsertNewRow = false;
		this.gv_Replan.IsLoadingData = false;
		this.gv_Replan.IsMappingContextAction = false;
		this.gv_Replan.ItemId = null;
		this.gv_Replan.LstColumnAllowBatch = null;
		this.gv_Replan.LstColumnNotAllowBatch = null;
		this.gv_Replan.LstColumnNotEditable = null;
		this.gv_Replan.LstColumNotSetting = null;
		this.gv_Replan.Name = "gv_Replan";
		this.gv_Replan.OptionsView.ShowGroupPanel = false;
		this.gv_Replan.UseBatchMod = false;
		this.gv_Replan.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Replan_CustomDrawRowIndicator);
		this.gv_Replan.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Replan_RowStyle);
		this.repositoryItemSpinEdit_N0.AutoHeight = false;
		this.repositoryItemSpinEdit_N0.DisplayFormat.FormatString = "n0";
		this.repositoryItemSpinEdit_N0.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemSpinEdit_N0.EditFormat.FormatString = "n0";
		this.repositoryItemSpinEdit_N0.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.repositoryItemSpinEdit_N0.Mask.EditMask = "n0";
		this.repositoryItemSpinEdit_N0.Name = "repositoryItemSpinEdit_N0";
		this.repositoryItemSpinEdit_N0.NullText = "0";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(12, 419);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(85, 23);
		this.btn_Save.TabIndex = 77;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(762, 446);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.date_ToDate);
		base.Controls.Add(this.date_FromDate);
		base.Controls.Add(this.lb_ToDate);
		base.Controls.Add(this.lb_FromDate);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_MasterOrderReplan";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Master Order Planning";
		base.Load += new System.EventHandler(this.frm_MasterOrderReplan_Load);
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_MasterOrder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_MasterOrder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gr_MOReplan).EndInit();
		this.gr_MOReplan.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Replan).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Replan).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemSpinEdit_N0).EndInit();
		base.ResumeLayout(false);
	}
}
