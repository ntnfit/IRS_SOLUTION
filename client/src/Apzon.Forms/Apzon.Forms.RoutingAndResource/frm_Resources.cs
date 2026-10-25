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
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.Production_Addon;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraTab;

namespace Apzon.Forms.RoutingAndResource;

public class frm_Resources : XtraFormEx
{
	private DataTable _dtDocs = new DataTable();

	private GridColumn resCode;

	private string entry;

	private string currentResCode;

	private string FormController = "Resources";

	private static bool statusAddNewRowDoc = true;

	private int rowHand = -1;

	private bool edittingLine;

	private bool _edit;

	private int hotTrackRow = int.MinValue;

	private IContainer components;

	private Panel panel1;

	private SimpleButtonEx btRefresh;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btSave;

	private SplitContainerControl splitContainerControl1;

	private GroupControl gpGeneral;

	private GridControlEx grdGeneral;

	private GridViewEx gvGeneral;

	private GridColumnEx clResCode;

	private GridColumnEx clResName;

	private GridColumnEx clResType;

	private GridColumnEx clResGrpCod;

	private GridColumnEx clStatus;

	private GridColumnEx clPerInCharge;

	private GridColumnEx clCodeBars;

	private GridColumnEx clPlant;

	private GridColumnEx clCalendar;

	private GridColumnEx clDist5;

	private GridColumnEx clSetupTime;

	private GridColumnEx clSetupUoM;

	private GridColumnEx clMachineTime;

	private GridColumnEx clMachineUoM;

	private GridColumnEx clLaborTime;

	private GridColumnEx clLaborUoM;

	private GridColumnEx clQuantity;

	private GridColumnEx clUnitOfMsr;

	private GridColumnEx clRemarks;

	private GroupControl groupControl2;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx tabCosting;

	private GridControlEx grdCosting;

	private GridViewEx gvCosting;

	private XtraTabPageEx tabCapacity;

	private XtraTabPageEx tabFixedAsset;

	private GridControlEx grdFixedAsset;

	private GridViewEx gvFixedAsset;

	private GridColumnEx clItemCode;

	private GridColumnEx clItemName;

	private GridColumnEx clStat;

	private GridColumnEx clLocation;

	private GridColumnEx clTechnician;

	private GridColumnEx clEmployee;

	private GridColumnEx clRemak;

	private GridColumnEx clStdCost;

	private GridColumnEx clDefaultCost;

	private GridColumnEx clCostExpAct;

	private GridColumnEx clWipAct;

	private GridColumnEx clSetupCost;

	private GridColumnEx clMachineCost;

	private GridColumnEx clLaborCost;

	private GridColumnEx clOtherCost;

	private RepositoryItemLookUpEdit repStatus2;

	private RepositoryItemLookUpEdit repStatus;

	private GridColumnEx colDist1;

	private GridColumnEx clDist2;

	private GridColumnEx clDist3;

	private GridColumnEx clDist4;

	private RepositoryItemLookUpEdit repType;

	private RepositoryItemLookUpEdit repGroup;

	private RepositoryItemCheckEdit repCheck;

	private RepositoryItemCalcEdit repTinh;

	private ContextMenuStrip contextMenuStrip_Docs;

	private ToolStripMenuItem addNewRowToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip_Doc;

	private GridColumnEx empID;

	private RepositoryItemLookUpEdit repStdCost;

	private GridColumnEx IssueMthd;

	private GridColumnEx clNumResUnit;

	private GridColumnEx clResFName;

	private GridColumnEx clCtrlkey;

	private GridColumnEx clCtrlArea;

	private RepositoryItemButtonEdit repAcctCode;

	private RepositoryItemButtonEdit repAcctCode2;

	private RepositoryItemButtonEdit repOcc;

	private ToolStripMenuItem insertNewDataToolStripMenuItem;

	private RepositoryItemButtonEdit repItemCode;

	private GridControlEx grd_CapacityMain;

	private GridViewEx gv_CapacityChildren;

	private GridViewEx gv_CapacityMain;

	private GridColumnEx clFromDate;

	private GridColumnEx clToDate;

	private GridColumnEx colResCode;

	private GridColumnEx colLineNum;

	private GridColumnEx colWeekDay;

	private GridColumnEx colUOM;

	private GridColumnEx clCapfactor1;

	private GridColumnEx clCapFactor2;

	private GridColumnEx clCapfactor3;

	private GridColumnEx clCapFactor4;

	private GridColumnEx clCapTotal;

	private GridColumnEx colRemarks;

	private GridColumnEx clStartTime;

	private GridColumnEx clFinishTime;

	private GridColumnEx clBreaksTime;

	private GridColumnEx clvalidFrom;

	private GridColumnEx clvalidTo;

	private RepositoryItemLookUpEdit repShowDay;

	private RepositoryItemLookUpEdit repUOM;

	private RepositoryItemButtonEdit rep_Ocr2;

	private RepositoryItemButtonEdit rep_Ocr3;

	private RepositoryItemButtonEdit rep_Ocr5;

	private GridColumnEx clDis5;

	private RepositoryItemButtonEdit rep_Ocr4;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem1;

	private ContextMenuStrip contextMenuStripFixasset;

	private ToolStripMenuItem deleteToolStripMenuItem2;

	private RepositoryItemButtonEdit repControlKey;

	private RepositoryItemCalcEdit repQuantity;

	private RepositoryItemMemoEdit repositoryItemMemoEdit1;

	private RepositoryItemCalcEdit repNumber;

	private RepositoryItemMemoEdit repositoryItemMemoEdit3;

	private RepositoryItemMemoEdit repositoryItemMemoEdit2;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private RepositoryItemMemoExEdit repositoryItemMemoExEdit1;

	private int HotTrackRow
	{
		get
		{
			return hotTrackRow;
		}
		set
		{
			if (hotTrackRow != value)
			{
				int rowHandle = hotTrackRow;
				hotTrackRow = value;
				if (gvGeneral.ActiveEditor != null)
				{
					gvGeneral.PostEditor();
				}
				gvGeneral.RefreshRow(rowHandle);
				gvGeneral.RefreshRow(hotTrackRow);
				if (hotTrackRow >= 0)
				{
					grdGeneral.Cursor = Cursors.Hand;
				}
				else
				{
					grdGeneral.Cursor = Cursors.Default;
				}
			}
		}
	}

	public frm_Resources()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Resources"))
		{
			InitializeComponent();
			SettingLanguage();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_Resources" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void SettingLanguage()
	{
		Text = Resources.Title_frmResources;
		addNewRowToolStripMenuItem.Text = Resources.ctx_Add;
		insertNewDataToolStripMenuItem.Text = Resources.ctx_Add;
		btSave.Text = Resources.ButtonSave;
		btCancel.Text = Resources.ButtonCancel;
		btRefresh.Text = Resources.ButtonRefresh;
		gpGeneral.Text = Resources.tab_Genral;
		clResCode.Caption = Resources.lb_ResourceNo;
		clResName.Caption = Resources.lb_ResourceName;
		clResType.Caption = Resources.lb_Type;
		clResGrpCod.Caption = Resources.lb_Group;
		clStatus.Caption = Resources.lb_Status;
		colDist1.Caption = Resources.lb_DistRule1;
		clDist2.Caption = Resources.lb_DistRule2;
		clDist3.Caption = Resources.lb_DistRule3;
		clDist4.Caption = Resources.lb_DistRule4;
		clDis5.Caption = Resources.lb_DistRule5;
		clPlant.Caption = Resources.lb_Plant;
		clCtrlArea.Caption = Resources.lb_CtrlArea;
		clCtrlkey.Caption = Resources.lb_CtrlKey;
		clRemarks.Caption = Resources.lb_Remark;
		clCalendar.Caption = Resources.lb_Calendar;
		clSetupTime.Caption = Resources.lb_SetupTime;
		clSetupUoM.Caption = Resources.lb_SetupUoM;
		clMachineTime.Caption = Resources.lb_MachineTime;
		clMachineUoM.Caption = Resources.lb_MachineUoM;
		clLaborTime.Caption = Resources.lb_LaborTime;
		clLaborUoM.Caption = Resources.lb_LaborUoM;
		clQuantity.Caption = Resources.lb_BaseQty;
		tabCapacity.Text = Resources.tab_Capacity;
		clFromDate.Caption = Resources.Doc_lb_FromDate;
		clToDate.Caption = Resources.Doc_lb_ToDate;
		tabFixedAsset.Text = Resources.tab_FixedAsset;
		clItemCode.Caption = Resources.lb_FixedAssetCode;
		clItemName.Caption = Resources.lb_FAName;
		clStat.Caption = Resources.lb_Status;
		clLocation.Caption = Resources.lb_Location;
		clTechnician.Caption = Resources.lb_Technical;
		clEmployee.Caption = Resources.lb_Employee;
		clRemak.Caption = Resources.lb_Remark;
		tabCosting.Text = Resources.tab_Costing;
		clStdCost.Caption = Resources.lb_ResourceStandardCostName;
		clDefaultCost.Caption = Resources.lb_DefaultCost;
		clCostExpAct.Caption = Resources.lb_CostExpenseAct;
		clWipAct.Caption = Resources.lb_WipAccount;
		clSetupCost.Caption = Resources.lb_SetupCost;
		clMachineCost.Caption = Resources.lb_MachineCost;
		clLaborCost.Caption = Resources.lb_LaborCost;
		clOtherCost.Caption = Resources.lb_OtherCost;
	}

	private void frm_Resources_Load(object sender, EventArgs e)
	{
		try
		{
			LoadInIt();
			GetDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadInIt()
	{
		try
		{
			List<Common.Table_ID> list = new List<Common.Table_ID>();
			list.Add(new Common.Table_ID
			{
				Name = "Active",
				ID = "Y"
			});
			list.Add(new Common.Table_ID
			{
				Name = "Inactive",
				ID = "N"
			});
			repStatus.DataSource = list;
			List<Common.Table_ID> list2 = new List<Common.Table_ID>();
			list2.Add(new Common.Table_ID
			{
				Name = "New",
				ID = "N"
			});
			list2.Add(new Common.Table_ID
			{
				Name = "Active",
				ID = "A"
			});
			list2.Add(new Common.Table_ID
			{
				Name = "Inactive",
				ID = "I"
			});
			repStatus2.DataSource = list2;
			List<Common.Table_ID> list3 = new List<Common.Table_ID>();
			list3.Add(new Common.Table_ID
			{
				Name = "Labor",
				ID = "L"
			});
			list3.Add(new Common.Table_ID
			{
				Name = "Machine",
				ID = "M"
			});
			list3.Add(new Common.Table_ID
			{
				Name = "Other",
				ID = "O"
			});
			repType.DataSource = list3;
			List<Common.Table_ID> list4 = new List<Common.Table_ID>();
			list4.Add(new Common.Table_ID
			{
				Name = "Resources",
				ID = 1
			});
			repGroup.DataSource = list4;
			List<Common.Table_ID> list5 = new List<Common.Table_ID>();
			list5.Add(new Common.Table_ID
			{
				Name = "Cost 1",
				ID = 1
			});
			list5.Add(new Common.Table_ID
			{
				Name = "Cost 2",
				ID = 2
			});
			list5.Add(new Common.Table_ID
			{
				Name = "Cost 3",
				ID = 3
			});
			list5.Add(new Common.Table_ID
			{
				Name = "Cost 4",
				ID = 4
			});
			list5.Add(new Common.Table_ID
			{
				Name = "Cost 5",
				ID = 5
			});
			list5.Add(new Common.Table_ID
			{
				Name = "Cost 6",
				ID = 6
			});
			list5.Add(new Common.Table_ID
			{
				Name = "Cost 7",
				ID = 7
			});
			list5.Add(new Common.Table_ID
			{
				Name = "Cost 8",
				ID = 8
			});
			list5.Add(new Common.Table_ID
			{
				Name = "Cost 9",
				ID = 9
			});
			list5.Add(new Common.Table_ID
			{
				Name = "Cost 10",
				ID = 10
			});
			repStdCost.DataSource = list5;
			LoadDates();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadDates()
	{
		try
		{
			List<Common.Table_ID> list = new List<Common.Table_ID>();
			list.Add(new Common.Table_ID
			{
				Name = "Sunday",
				ID = 1
			});
			list.Add(new Common.Table_ID
			{
				Name = "Monday",
				ID = 2
			});
			list.Add(new Common.Table_ID
			{
				Name = "Tueday",
				ID = 3
			});
			list.Add(new Common.Table_ID
			{
				Name = "Wednesday",
				ID = 4
			});
			list.Add(new Common.Table_ID
			{
				Name = "Thursday",
				ID = 5
			});
			list.Add(new Common.Table_ID
			{
				Name = "Friday",
				ID = 6
			});
			list.Add(new Common.Table_ID
			{
				Name = "Saturday",
				ID = 7
			});
			repShowDay.DataSource = list;
			List<Common.Table_ID> list2 = new List<Common.Table_ID>();
			list2.Add(new Common.Table_ID
			{
				Name = "Hours",
				ID = "H"
			});
			list2.Add(new Common.Table_ID
			{
				Name = "Minutes",
				ID = "M"
			});
			repUOM.DataSource = list2;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void GetDoc()
	{
		_ = 3;
		try
		{
			string resCode = Function.ToString(gvGeneral.GetRowCellValue(gvGeneral.FocusedRowHandle, "ResCode"));
			if (!string.IsNullOrEmpty(resCode))
			{
				List<Obj_Date> lstDate = await GetDate();
				if (lstDate != null && lstDate.Count > 0)
				{
					foreach (Obj_Date item in lstDate)
					{
						item.ResCode = resCode;
						item.lstCapacity = new List<Obj_Capacity>();
						item.lstCapacity = await GetCapacityByRes(item);
					}
				}
				grd_CapacityMain.DataSource = lstDate;
				grd_CapacityMain.RefreshDataSource();
				DataTable dataSource = await GetDocTabFix(resCode);
				grdFixedAsset.DataSource = dataSource;
				grdFixedAsset.RefreshDataSource();
				DataTable dataSource2 = await GetDocTabCosting(resCode);
				grdCosting.DataSource = dataSource2;
				grdCosting.RefreshDataSource();
			}
			else
			{
				grdFixedAsset.DataSource = null;
				grdFixedAsset.RefreshDataSource();
				grdCosting.DataSource = null;
				grdCosting.RefreshDataSource();
				grd_CapacityMain.DataSource = null;
				grd_CapacityMain.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowCosting()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ResCode", typeof(string));
			dataTable.Columns.Add("StdCost", typeof(int));
			dataTable.Columns.Add("DefaultCost", typeof(int));
			dataTable.Columns.Add("CostExpAct", typeof(string));
			dataTable.Columns.Add("WipAct", typeof(string));
			dataTable.Columns.Add("SetupCost", typeof(string));
			dataTable.Columns.Add("MachineCost", typeof(string));
			dataTable.Columns.Add("LaborCost", typeof(string));
			dataTable.Columns.Add("OtherCost", typeof(string));
			dataTable.AcceptChanges();
			for (int i = 1; i < 11; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["ResCode"] = Function.ToString(gvGeneral.GetRowCellValue(gvGeneral.FocusedRowHandle, "ResCode"));
				dataRow["StdCost"] = i;
				dataRow["DefaultCost"] = 0;
				dataRow["CostExpAct"] = "";
				dataRow["WipAct"] = "";
				dataRow["SetupCost"] = "N";
				dataRow["MachineCost"] = "N";
				dataRow["LaborCost"] = "N";
				dataRow["OtherCost"] = "N";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			grdCosting.DataSource = dataTable;
			grdCosting.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteRowEndEmptyFixAsset()
	{
		try
		{
			DataTable dataTable = grdFixedAsset.DataSource as DataTable;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
				dataTable.AcceptChanges();
			}
			grdFixedAsset.DataSource = dataTable;
			grdFixedAsset.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRowFixasset()
	{
		try
		{
			if (!(grdFixedAsset.DataSource is DataTable dataTable) || dataTable.Rows.Count == 0)
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("ResCode", typeof(string));
				dataTable2.Columns.Add("ItemCode", typeof(string));
				dataTable2.Columns.Add("ItemName", typeof(string));
				dataTable2.Columns.Add("AsstStatus", typeof(string));
				dataTable2.Columns.Add("Location", typeof(int));
				dataTable2.Columns.Add("Technician", typeof(int));
				dataTable2.Columns.Add("Employee", typeof(int));
				dataTable2.Columns.Add("Remark", typeof(string));
				DataRow dataRow = dataTable2.NewRow();
				dataRow["ResCode"] = gvGeneral.GetRowCellValue(gvGeneral.FocusedRowHandle, "ResCode");
				object obj = (dataRow["AsstStatus"] = "");
				object obj3 = (dataRow["ItemName"] = obj);
				object value = (dataRow["ItemCode"] = obj3);
				dataRow["ItemCode"] = value;
				obj3 = (dataRow["Employee"] = 0);
				value = (dataRow["Technician"] = obj3);
				dataRow["Location"] = value;
				dataRow["Remark"] = "";
				dataTable2.Rows.Add(dataRow);
				dataTable2.AcceptChanges();
				grdFixedAsset.DataSource = dataTable2;
				grdFixedAsset.RefreshDataSource();
			}
			else if (!string.IsNullOrEmpty(Function.ToString(gvFixedAsset.GetRowCellValue(gvFixedAsset.RowCount - 1, "ItemCode"))))
			{
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["ResCode"] = gvGeneral.GetRowCellValue(gvGeneral.FocusedRowHandle, "ResCode");
				object obj = (dataRow2["AsstStatus"] = "");
				object obj3 = (dataRow2["ItemName"] = obj);
				object value = (dataRow2["ItemCode"] = obj3);
				dataRow2["ItemCode"] = value;
				obj3 = (dataRow2["Employee"] = 0);
				value = (dataRow2["Technician"] = obj3);
				dataRow2["Location"] = value;
				dataRow2["Remark"] = "";
				dataTable.Rows.Add(dataRow2);
				dataTable.AcceptChanges();
				grdFixedAsset.DataSource = dataTable;
				grdFixedAsset.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Save()
	{
		try
		{
			edittingLine = false;
			DataTable dataTable = grdGeneral.DataSource as DataTable;
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[i]["ResCode"])))
				{
					dataTable.Rows.RemoveAt(i);
					i--;
					dataTable.AcceptChanges();
				}
			}
			List<Obj_Date> obj = grd_CapacityMain.DataSource as List<Obj_Date>;
			DataTable dataTable2 = ConvertListObjToDatatable(obj);
			DataTable dataTable3 = grdCosting.DataSource as DataTable;
			DataTable dataTable4 = grdFixedAsset.DataSource as DataTable;
			for (int j = 0; j < dataTable4.Rows.Count; j++)
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[j]["ItemCode"])))
				{
					dataTable4.Rows.RemoveAt(j);
					j--;
					dataTable4.AcceptChanges();
				}
			}
			DataTable dataTable5 = new DataTable();
			if (dataTable4.IsNotNull())
			{
				dataTable5.Columns.Add("ResCode", typeof(string));
				dataTable5.Columns.Add("ItemCode", typeof(string));
				dataTable5.Columns.Add("Remark", typeof(string));
				for (int k = 0; k < dataTable4.Rows.Count; k++)
				{
					DataRow dataRow = dataTable5.NewRow();
					dataRow["ResCode"] = gvGeneral.GetRowCellValue(gvGeneral.FocusedRowHandle, "ResCode");
					dataRow["ItemCode"] = dataTable4.Rows[k]["ItemCode"];
					dataRow["Remark"] = dataTable4.Rows[k]["Remark"];
					dataTable5.Rows.Add(dataRow);
					dataTable5.AcceptChanges();
				}
			}
			DataSet ds = new DataSet();
			ds.Tables.AddRange(new DataTable[4] { dataTable, dataTable2, dataTable3, dataTable5 });
			ds.AcceptChanges();
			string text = await WaitSaveData(ds);
			ds.Reset();
			ds.Clear();
			if (text == "")
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			GetDocs();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable ConvertListObjToDatatable(List<Obj_Date> obj)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ResCode", typeof(string));
			dataTable.Columns.Add("LineNum", typeof(int));
			dataTable.Columns.Add("WeekDay", typeof(int));
			dataTable.Columns.Add("UnitOfMsr", typeof(string));
			dataTable.Columns.Add("CapFactor1", typeof(double));
			dataTable.Columns.Add("CapFactor2", typeof(double));
			dataTable.Columns.Add("CapFactor3", typeof(double));
			dataTable.Columns.Add("CapFactor4", typeof(double));
			dataTable.Columns.Add("CapTotal", typeof(double));
			dataTable.Columns.Add("Remarks", typeof(string));
			dataTable.Columns.Add("StartTime", typeof(TimeSpan));
			dataTable.Columns.Add("FinishTime", typeof(TimeSpan));
			dataTable.Columns.Add("BreaksTime", typeof(double));
			dataTable.Columns.Add("validFrom", typeof(DateTime));
			dataTable.Columns.Add("validTo", typeof(DateTime));
			dataTable.AcceptChanges();
			foreach (Obj_Date item in obj)
			{
				for (int i = 0; i < item.lstCapacity.Count; i++)
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["ResCode"] = item.ResCode;
					dataRow["validFrom"] = item.validFrom;
					dataRow["validTo"] = item.validTo;
					Obj_Capacity obj_Capacity = item.lstCapacity[i];
					dataRow["LineNum"] = obj_Capacity.LineNum;
					dataRow["WeekDay"] = obj_Capacity.WeekDay;
					dataRow["UnitOfMsr"] = obj_Capacity.UnitOfMsr;
					dataRow["CapFactor1"] = ((!obj_Capacity.CapFactor1.HasValue) ? new decimal?(0m) : obj_Capacity.CapFactor1);
					dataRow["CapFactor2"] = ((!obj_Capacity.CapFactor2.HasValue) ? new decimal?(0m) : obj_Capacity.CapFactor2);
					dataRow["CapFactor3"] = ((!obj_Capacity.CapFactor3.HasValue) ? new decimal?(0m) : obj_Capacity.CapFactor3);
					dataRow["CapFactor4"] = ((!obj_Capacity.CapFactor4.HasValue) ? new decimal?(0m) : obj_Capacity.CapFactor4);
					dataRow["CapTotal"] = ((!obj_Capacity.CapTotal.HasValue) ? new decimal?(0m) : obj_Capacity.CapTotal);
					dataRow["Remarks"] = ((obj_Capacity.Remarks == null) ? "" : obj_Capacity.Remarks);
					dataRow["StartTime"] = obj_Capacity.StartTime;
					dataRow["FinishTime"] = obj_Capacity.FinishTime;
					dataRow["BreaksTime"] = ((!obj_Capacity.BreaksTime.HasValue) ? new double?(0.0) : obj_Capacity.BreaksTime);
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
				}
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<string> WaitSaveData(DataSet dtFather)
	{
		try
		{
			using HttpClientHelper<ResourcesImpl> _helper = new HttpClientHelper<ResourcesImpl>(Configuration.ApiServer, FormController);
			return await _helper.PostParamsAsJsonAsync("Save", dtFather);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return null;
		}
	}

	private async Task<DataTable> GetDocTabCapacity(string Entry)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Resources");
			DataTable result = await _helper.PostAsync("GetDocTabCapacity", Entry);
			if (_dtDocs == null)
			{
				return new DataTable();
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> GetCapacityChildren(string Entry)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Resources");
			return await _helper.PostAsync("GetCapacityChildren", Entry);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<List<Obj_Capacity>> GetCapacityByRes(Obj_Date obj)
	{
		try
		{
			DataTable dataTable = await AwaitGetCap(obj);
			List<Obj_Capacity> list = new List<Obj_Capacity>();
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				dataTable.BeginLoadData();
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					Obj_Capacity item = new Obj_Capacity
					{
						ResCode = dataTable.Rows[i]["ResCode"].ToString(),
						Remarks = dataTable.Rows[i]["Remarks"].ToString(),
						UnitOfMsr = dataTable.Rows[i]["UnitOfMsr"].ToString(),
						CapFactor1 = Function.ParseDecimal(dataTable.Rows[i]["CapFactor1"]),
						CapFactor2 = Function.ParseDecimal(dataTable.Rows[i]["CapFactor2"]),
						CapFactor3 = Function.ParseDecimal(dataTable.Rows[i]["CapFactor3"]),
						CapFactor4 = Function.ParseDecimal(dataTable.Rows[i]["CapFactor4"]),
						CapTotal = Function.ParseDecimal(dataTable.Rows[i]["CapTotal"]),
						LineNum = Function.ParseInt(dataTable.Rows[i]["LineNum"]),
						WeekDay = Function.ParseInt(dataTable.Rows[i]["WeekDay"]),
						StartTime = TimeSpan.Parse(dataTable.Rows[i]["StartTime"].ToString()),
						FinishTime = TimeSpan.Parse(dataTable.Rows[i]["FinishTime"].ToString()),
						BreaksTime = Function.ParseDouble(dataTable.Rows[i]["BreaksTime"])
					};
					list.Add(item);
				}
				dataTable.EndLoadData();
			}
			return list;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new List<Obj_Capacity>();
		}
	}

	private async Task<DataTable> AwaitGetCap(Obj_Date obj)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await _helper.PostAsync("GetCapacityByRes", obj);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> GetDocTabFix(string Entry)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Resources");
			DataTable result = await _helper.PostAsync("GetDocTabFix", Entry);
			if (_dtDocs == null)
			{
				return new DataTable();
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> GetDocTabCosting(string Entry)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Resources");
			DataTable result = await _helper.PostAsync("GetDocTabCosting", Entry);
			if (_dtDocs == null)
			{
				return new DataTable();
			}
			return result;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void GetDocs()
	{
		DataTable dataTable = await AsyncGetDocs();
		if (dataTable == null || dataTable.Rows.Count == 0)
		{
			AddNewRowDocs();
			return;
		}
		grdGeneral.DataSource = dataTable;
		grdGeneral.RefreshDataSource();
		AddNewRowDocs();
	}

	private async Task<DataTable> AsyncGetDocs()
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Resources");
			DataTable dataTable = await _helper.GetDataTableAsync("GetDocs");
			if (dataTable == null)
			{
				return new DataTable();
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private void addNewRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			Duplacate();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void DeleteRowEndEmpty()
	{
		try
		{
			DataTable dataTable = grdGeneral.DataSource as DataTable;
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
				dataTable.AcceptChanges();
			}
			grdGeneral.DataSource = dataTable;
			grdGeneral.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Duplacate()
	{
		try
		{
			if (grdGeneral.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				DataRow dataRow = dataTable.NewRow();
				object value = (dataRow["ResName"] = "");
				dataRow["ResCode"] = value;
				dataRow["ResType"] = gvGeneral.GetFocusedRowCellValue("ResType");
				dataRow["UnitOfMsr"] = gvGeneral.GetFocusedRowCellValue("UnitOfMsr");
				dataRow["Status"] = gvGeneral.GetFocusedRowCellValue("Status");
				dataRow["CodeBars"] = gvGeneral.GetFocusedRowCellValue("CodeBars");
				dataRow["Plant"] = gvGeneral.GetFocusedRowCellValue("Plant");
				dataRow["Calendar"] = gvGeneral.GetFocusedRowCellValue("Calendar");
				dataRow["OcrCode"] = gvGeneral.GetFocusedRowCellValue("OcrCode");
				dataRow["OcrCode2"] = gvGeneral.GetFocusedRowCellValue("OcrCode2");
				dataRow["OcrCode3"] = gvGeneral.GetFocusedRowCellValue("OcrCode3");
				dataRow["OcrCode4"] = gvGeneral.GetFocusedRowCellValue("OcrCode4");
				dataRow["SetUpUoM"] = gvGeneral.GetFocusedRowCellValue("SetUpUoM");
				dataRow["MachineUoM"] = gvGeneral.GetFocusedRowCellValue("MachineUoM");
				dataRow["Remark"] = "";
				dataRow["Quantity"] = gvGeneral.GetFocusedRowCellValue("Quantity");
				dataRow["ResGrpCod"] = gvGeneral.GetFocusedRowCellValue("ResGrpCod");
				dataRow["LaborTime"] = gvGeneral.GetFocusedRowCellValue("LaborTime");
				dataRow["MachineTime"] = gvGeneral.GetFocusedRowCellValue("MachineTime");
				dataRow["SetupTime"] = gvGeneral.GetFocusedRowCellValue("SetupTime");
				dataRow["UnitOfMsr"] = gvGeneral.GetFocusedRowCellValue("UnitOfMsr");
				dataTable.Rows.InsertAt(dataRow, gvGeneral.RowCount - 1);
				dataTable.AcceptChanges();
				grdGeneral.DataSource = dataTable;
				grdGeneral.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void AddNewRowDocs()
	{
		try
		{
			DataTable dt = grdGeneral.DataSource as DataTable;
			object obj2;
			object obj4;
			object obj6;
			object obj8;
			object obj10;
			object obj12;
			object obj14;
			object obj16;
			object value;
			if (dt.IsNotNull())
			{
				if (!string.IsNullOrEmpty(Function.ToString(dt.Rows[dt.Rows.Count - 1]["ResCode"])))
				{
					DataRow dataRow = dt.NewRow();
					value = (dataRow["ResName"] = "");
					dataRow["ResCode"] = value;
					dataRow["ResType"] = "L";
					dataRow["UnitOfMsr"] = "";
					dataRow["Status"] = "N";
					obj2 = (dataRow["Remark"] = "");
					obj4 = (dataRow["MachineUoM"] = obj2);
					obj6 = (dataRow["SetUpUoM"] = obj4);
					obj8 = (dataRow["OcrCode4"] = obj6);
					obj10 = (dataRow["OcrCode3"] = obj8);
					obj12 = (dataRow["OcrCode2"] = obj10);
					obj14 = (dataRow["OcrCode"] = obj12);
					obj16 = (dataRow["Calendar"] = obj14);
					value = (dataRow["Plant"] = obj16);
					dataRow["CodeBars"] = value;
					dataRow["Quantity"] = 0;
					dataRow["ResGrpCod"] = 1;
					obj14 = (dataRow["UnitOfMsr"] = 0);
					obj16 = (dataRow["SetupTime"] = obj14);
					value = (dataRow["MachineTime"] = obj16);
					dataRow["LaborTime"] = value;
					dt.Rows.Add(dataRow);
					dt.AcceptChanges();
					grdGeneral.DataSource = dt;
					grdGeneral.RefreshDataSource();
				}
				return;
			}
			if (dt == null || dt.Rows.Count == 0)
			{
				dt = new DataTable();
				DataTable dataTable = await AwaitGetTableSQL("APZ_ORSC");
				if (dataTable == null && dataTable.Rows.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.TableDoesNotExitsWithTableName, "", "APZ_ORSC");
					return;
				}
				for (int i = 0; i < dataTable.Count(); i++)
				{
					Type typeFromSql = WinFuntions.GetTypeFromSql(Function.ToString(dataTable.Rows[i]["DATA_TYPE"]));
					dt.Columns.Add(Function.ToString(dataTable.Rows[i]["COLUMN_NAME"]), typeFromSql);
				}
			}
			DataRow dataRow2 = dt.NewRow();
			value = (dataRow2["ResName"] = "");
			dataRow2["ResCode"] = value;
			dataRow2["ResType"] = "L";
			dataRow2["UnitOfMsr"] = "";
			dataRow2["Status"] = "N";
			obj2 = (dataRow2["Remark"] = "");
			obj4 = (dataRow2["MachineUoM"] = obj2);
			obj6 = (dataRow2["SetUpUoM"] = obj4);
			obj8 = (dataRow2["OcrCode4"] = obj6);
			obj10 = (dataRow2["OcrCode3"] = obj8);
			obj12 = (dataRow2["OcrCode2"] = obj10);
			obj14 = (dataRow2["OcrCode"] = obj12);
			obj16 = (dataRow2["Calendar"] = obj14);
			value = (dataRow2["Plant"] = obj16);
			dataRow2["CodeBars"] = value;
			dataRow2["Quantity"] = 0;
			dataRow2["ResGrpCod"] = 1;
			obj14 = (dataRow2["UnitOfMsr"] = 0);
			obj16 = (dataRow2["SetupTime"] = obj14);
			value = (dataRow2["MachineTime"] = obj16);
			dataRow2["LaborTime"] = value;
			dt.Rows.Add(dataRow2);
			dt.AcceptChanges();
			grdGeneral.DataSource = dt;
			grdGeneral.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void AddNewRowCapacity()
	{
		try
		{
			string resCode = Function.ToString(gvGeneral.GetRowCellValue(gvGeneral.FocusedRowHandle, "ResCode"));
			if (string.IsNullOrEmpty(resCode))
			{
				return;
			}
			List<Obj_Date> lstSourceDate = grd_CapacityMain.DataSource as List<Obj_Date>;
			if (lstSourceDate == null || lstSourceDate.Count == 0)
			{
				lstSourceDate = new List<Obj_Date>();
				Obj_Date obj_Date = new Obj_Date();
				obj_Date.validFrom = DateTime.Now;
				obj_Date.validTo = DateTime.Now;
				obj_Date.ResCode = resCode;
				obj_Date.lstCapacity = new List<Obj_Capacity>();
				for (int i = 1; i < 8; i++)
				{
					Obj_Capacity obj_Capacity = new Obj_Capacity();
					obj_Capacity.ResCode = resCode;
					obj_Capacity.LineNum = i;
					obj_Capacity.WeekDay = i;
					obj_Capacity.UnitOfMsr = "H";
					obj_Capacity.Remarks = "";
					obj_Date.lstCapacity.Add(obj_Capacity);
				}
				lstSourceDate.Add(obj_Date);
			}
			else
			{
				Obj_Date ob = new Obj_Date
				{
					validFrom = DateTime.Today,
					validTo = DateTime.Today,
					ResCode = resCode,
					lstCapacity = new List<Obj_Capacity>()
				};
				int num = Function.ParseInt((await GetMaxLineNumByResCode(resCode)).Rows[0][0]);
				for (int j = 1; j < 8; j++)
				{
					Obj_Capacity obj_Capacity2 = new Obj_Capacity();
					obj_Capacity2.ResCode = resCode;
					obj_Capacity2.LineNum = j + num;
					obj_Capacity2.WeekDay = j;
					obj_Capacity2.UnitOfMsr = "H";
					obj_Capacity2.Remarks = "";
					ob.lstCapacity.Add(obj_Capacity2);
				}
				lstSourceDate.Add(ob);
			}
			grd_CapacityMain.DataSource = lstSourceDate;
			grd_CapacityMain.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataTable> AwaitGetTableSQL(string nameTable)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await _helper.PostAsync("GetAllFieldTable", nameTable);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async Task<DataTable> GetMaxLineNumByResCode(string resCode)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await _helper.PostAsync("GetMaxLineNumByResCode", resCode);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return null;
		}
	}

	private async Task<List<Obj_Date>> GetDate()
	{
		try
		{
			string text = Function.ToString(gvGeneral.GetRowCellValue(gvGeneral.FocusedRowHandle, "ResCode"));
			return ConvertHelper.DataTableToList<Obj_Date>(await AwaitDate(text));
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new List<Obj_Date>();
		}
	}

	private async Task<DataTable> AwaitDate(string ResCode)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, FormController);
			return await _helper.PostAsync("GetDate", ResCode);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private async void insertNewDataToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRowCapacity();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CapacityMain_MasterRowGetChildList(object sender, MasterRowGetChildListEventArgs e)
	{
		try
		{
			if (gv_CapacityMain.GetRow(e.RowHandle) is Obj_Date obj_Date && e.RelationIndex == 0)
			{
				e.ChildList = obj_Date.lstCapacity;
			}
		}
		catch (Exception)
		{
		}
	}

	private void gv_CapacityMain_MasterRowGetRelationCount(object sender, MasterRowGetRelationCountEventArgs e)
	{
		e.RelationCount = 1;
	}

	private void gv_CapacityMain_MasterRowGetRelationName(object sender, MasterRowGetRelationNameEventArgs e)
	{
		e.RelationName = "Level1";
	}

	private void gv_CapacityChildren_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
	}

	private void gv_CapacityChildren_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "CapFactor1" || e.Column.FieldName == "CapFactor2" || e.Column.FieldName == "CapFactor3" || e.Column.FieldName == "CapFactor4")
			{
				int rowHandle = gv_CapacityMain.GetSelectedRows()[0];
				GridView gridView = (GridView)gv_CapacityMain.GetDetailView(rowHandle, 0);
				if (Function.ParseDouble(e.Value) >= 0.0 && Function.ParseDouble(e.Value) <= 100.0)
				{
					edittingLine = true;
					double num = Function.ParseDouble(gridView.GetRowCellValue(gridView.FocusedRowHandle, "CapFactor1")) * Function.ParseDouble(gridView.GetRowCellValue(gridView.FocusedRowHandle, "CapFactor2")) * Function.ParseDouble(gridView.GetRowCellValue(gridView.FocusedRowHandle, "CapFactor3")) * Function.ParseDouble(gridView.GetRowCellValue(gridView.FocusedRowHandle, "CapFactor4"));
					gridView.SetRowCellValue(gridView.FocusedRowHandle, "CapTotal", num);
				}
				else
				{
					WinCommons.Warning(Enums.MessageCode.WarningValueOnlyBetween0and1, "");
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repOcc_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OrcCode, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
				gvGeneral.SetRowCellValue(gvGeneral.FocusedRowHandle, "OcrCode", row[0]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Ocr2_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OrcCode2, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
				gvGeneral.SetRowCellValue(gvGeneral.FocusedRowHandle, "OcrCode2", row[0]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Ocr3_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OrcCode3, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
				gvGeneral.SetRowCellValue(gvGeneral.FocusedRowHandle, "OcrCode3", row[0]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_Ocr5_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OrcCode5, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
				gvGeneral.SetRowCellValue(gvGeneral.FocusedRowHandle, "OcrCode5", row[0]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvGeneral_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void rep_Ocr4_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OrcCode4, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				DataRow row = ((DataRowView)frm_ChooseFromList2.ChooseItem).Row;
				gvGeneral.SetRowCellValue(gvGeneral.FocusedRowHandle, "OcrCode4", row[0]);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvGeneral_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		if (edittingLine)
		{
			DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmRefreshData, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (DialogResult.Yes == dialogResult)
			{
				GetDoc();
				rowHand = gvGeneral.FocusedRowHandle;
				edittingLine = false;
			}
			else
			{
				gvGeneral.FocusedRowHandle = rowHand;
				edittingLine = false;
			}
		}
		else
		{
			GetDoc();
			rowHand = gvGeneral.FocusedRowHandle;
		}
	}

	private void panel1_Paint(object sender, PaintEventArgs e)
	{
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btRefresh_Click(object sender, EventArgs e)
	{
		GetDocs();
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string value = Function.ToString(gvGeneral.GetFocusedRowCellValue("ResCode"));
			if (string.IsNullOrEmpty(value))
			{
				WinCommons.Error(Enums.MessageCode.RowIsEmpty, "");
				return;
			}
			DialogResult dialogResult = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
			if (DialogResult.Yes == dialogResult)
			{
				string text = await Delete(value);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					GetDocs();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> Delete(string resCode)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, FormController);
			return await helper.PostAsync("DeleteDocs", resCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return ex.Message;
		}
	}

	private void deleteToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			List<Obj_Date> list = grd_CapacityMain.DataSource as List<Obj_Date>;
			list.RemoveAt(gv_CapacityMain.FocusedRowHandle);
			grd_CapacityMain.DataSource = list;
			grd_CapacityMain.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvGeneral_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gvGeneral.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CapacityMain_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_CapacityMain.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvFixedAsset_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gvFixedAsset.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvCosting_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gvCosting.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_CapacityChildren_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_CapacityChildren.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.GridEditting;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addNewRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRowFixasset();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteToolStripMenuItem2_Click(object sender, EventArgs e)
	{
		try
		{
			if (!(grdFixedAsset.DataSource is DataTable dataTable) || dataTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.NoRecord, "");
				return;
			}
			dataTable.Rows.RemoveAt(gvFixedAsset.FocusedRowHandle);
			dataTable.AcceptChanges();
			grdFixedAsset.DataSource = dataTable;
			grdFixedAsset.RefreshDataSource();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvGeneral_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			edittingLine = true;
			_ = grdGeneral.DataSource;
			if (e.Column.FieldName == "ResCode")
			{
				if (!string.IsNullOrEmpty(Function.ToString(gvGeneral.GetFocusedRowCellValue("ResCode"))))
				{
					btSave.Enabled = true;
					AddNewRowDocs();
					AddNewRowCapacity();
					AddNewRowFixasset();
					addRowCosting();
				}
				else
				{
					btSave.Enabled = false;
					DeleteRowEndEmpty();
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repControlKey_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdGeneral.DataSource as DataTable;
			ButtonEdit buttonEdit = sender as ButtonEdit;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.ControlKey, multiSelect: false, "");
			frm_ChooseFromList2.ShowDialog(this);
			if (frm_ChooseFromList2.ChooseItem != null)
			{
				ControlKeyImpl controlKeyImpl = (ControlKeyImpl)frm_ChooseFromList2.ChooseItem;
				dataTable.Rows[gvGeneral.GetDataSourceRowIndex(gvGeneral.FocusedRowHandle)]["Ctrlkey"] = controlKeyImpl.KeyCode;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = controlKeyImpl.KeyCode;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.writeStatus(Enums.MessageCode.ExceptionMessage, Enums.MessageType.Warning, Msg_Time._TimeMedium, ex.Message);
		}
	}

	private void gv_CapacityMain_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			edittingLine = true;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvFixedAsset_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			edittingLine = true;
			if (e.Column.FieldName == "ItemCode")
			{
				if (!string.IsNullOrEmpty(Function.ToString(gvFixedAsset.GetFocusedRowCellValue("ItemCode"))))
				{
					AddNewRowFixasset();
				}
				else
				{
					DeleteRowEndEmptyFixAsset();
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvCosting_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			edittingLine = true;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(Function.ToString(gvGeneral.GetFocusedRowCellValue("ResCode"))))
			{
				if (DialogResult.Yes == XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question))
				{
					Save();
					statusAddNewRowDoc = true;
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.RowSelectedResCodeEmpty, "");
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvCosting_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
	}

	private void repAcctCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdCosting.DataSource as DataTable;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			ButtonEdit buttonEdit = sender as ButtonEdit;
			if (frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				AcctCodeImpl acctCodeImpl = (AcctCodeImpl)frm_ChooseFromList2.ChooseItems[0];
				dataTable.Rows[gvCosting.GetDataSourceRowIndex(gvCosting.FocusedRowHandle)]["WipAct"] = acctCodeImpl.AcctCode;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = acctCodeImpl.AcctCode;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repAcctCode2_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdCosting.DataSource as DataTable;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			ButtonEdit buttonEdit = sender as ButtonEdit;
			if (frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				AcctCodeImpl acctCodeImpl = (AcctCodeImpl)frm_ChooseFromList2.ChooseItems[0];
				dataTable.Rows[gvCosting.GetDataSourceRowIndex(gvCosting.FocusedRowHandle)]["CostExpAct"] = acctCodeImpl.AcctCode;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = acctCodeImpl.AcctCode;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void repItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdFixedAsset.DataSource as DataTable;
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Items, multiSelect: true, string.Empty);
			frm_ChooseFromList2.ShowDialog(this);
			ButtonEdit buttonEdit = sender as ButtonEdit;
			if (frm_ChooseFromList2.ChooseItems.Count > 0)
			{
				ItemsImpl itemsImpl = (ItemsImpl)frm_ChooseFromList2.ChooseItems[0];
				DataRow dataRow = dataTable.Rows[gvFixedAsset.GetDataSourceRowIndex(gvFixedAsset.FocusedRowHandle)];
				dataRow["ItemCode"] = itemsImpl.ItemCode;
				dataRow["ItemName"] = itemsImpl.ItemName;
				dataTable.AcceptChanges();
				buttonEdit.EditValue = itemsImpl.ItemCode;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_Resources_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (edittingLine)
			{
				if (XtraMessageBox.Show(this, "Are you sure?", "Do you still want ... ?", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					e.Cancel = true;
				}
				edittingLine = false;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvGeneral_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gvGeneral.GetFocusedRowCellValue("ResCode"))))
			{
				btSave.Enabled = false;
			}
			else
			{
				btSave.Enabled = true;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewRowFixasset();
	}

	private void gvGeneral_MouseLeave(object sender, EventArgs e)
	{
		HotTrackRow = int.MinValue;
	}

	private void gvGeneral_MouseMove(object sender, MouseEventArgs e)
	{
		GridHitInfo gridHitInfo = (sender as GridView).CalcHitInfo(new Point(e.X, e.Y));
		if (gridHitInfo.InRowCell)
		{
			HotTrackRow = gridHitInfo.RowHandle;
		}
		else
		{
			HotTrackRow = int.MinValue;
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
		DevExpress.XtraGrid.GridLevelNode gridLevelNode = new DevExpress.XtraGrid.GridLevelNode();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.RoutingAndResource.frm_Resources));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.gv_CapacityChildren = new Apzon.Controls.Grid.GridViewEx();
		this.colResCode = new Apzon.Controls.Grid.GridColumnEx();
		this.colLineNum = new Apzon.Controls.Grid.GridColumnEx();
		this.colWeekDay = new Apzon.Controls.Grid.GridColumnEx();
		this.repShowDay = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.colUOM = new Apzon.Controls.Grid.GridColumnEx();
		this.repUOM = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clCapfactor1 = new Apzon.Controls.Grid.GridColumnEx();
		this.repQuantity = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.clCapFactor2 = new Apzon.Controls.Grid.GridColumnEx();
		this.clCapfactor3 = new Apzon.Controls.Grid.GridColumnEx();
		this.clCapFactor4 = new Apzon.Controls.Grid.GridColumnEx();
		this.clCapTotal = new Apzon.Controls.Grid.GridColumnEx();
		this.colRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.clStartTime = new Apzon.Controls.Grid.GridColumnEx();
		this.clFinishTime = new Apzon.Controls.Grid.GridColumnEx();
		this.clBreaksTime = new Apzon.Controls.Grid.GridColumnEx();
		this.clvalidFrom = new Apzon.Controls.Grid.GridColumnEx();
		this.clvalidTo = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_CapacityMain = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.insertNewDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_CapacityMain = new Apzon.Controls.Grid.GridViewEx();
		this.clFromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.clToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.repNumber = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.panel1 = new System.Windows.Forms.Panel();
		this.btRefresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.gpGeneral = new DevExpress.XtraEditors.GroupControl();
		this.grdGeneral = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_Docs = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvGeneral = new Apzon.Controls.Grid.GridViewEx();
		this.clResCode = new Apzon.Controls.Grid.GridColumnEx();
		this.clResName = new Apzon.Controls.Grid.GridColumnEx();
		this.clResFName = new Apzon.Controls.Grid.GridColumnEx();
		this.clResType = new Apzon.Controls.Grid.GridColumnEx();
		this.repType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clResGrpCod = new Apzon.Controls.Grid.GridColumnEx();
		this.repGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clUnitOfMsr = new Apzon.Controls.Grid.GridColumnEx();
		this.clStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.repStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.IssueMthd = new Apzon.Controls.Grid.GridColumnEx();
		this.clPerInCharge = new Apzon.Controls.Grid.GridColumnEx();
		this.clCodeBars = new Apzon.Controls.Grid.GridColumnEx();
		this.clNumResUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.empID = new Apzon.Controls.Grid.GridColumnEx();
		this.colDist1 = new Apzon.Controls.Grid.GridColumnEx();
		this.repOcc = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clDist2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Ocr2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clDist3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Ocr3 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clDist4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Ocr4 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clDis5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Ocr5 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clPlant = new Apzon.Controls.Grid.GridColumnEx();
		this.clCtrlArea = new Apzon.Controls.Grid.GridColumnEx();
		this.clCtrlkey = new Apzon.Controls.Grid.GridColumnEx();
		this.repControlKey = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoExEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.clCalendar = new Apzon.Controls.Grid.GridColumnEx();
		this.clSetupTime = new Apzon.Controls.Grid.GridColumnEx();
		this.clSetupUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.clMachineTime = new Apzon.Controls.Grid.GridColumnEx();
		this.clMachineUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.clLaborTime = new Apzon.Controls.Grid.GridColumnEx();
		this.clLaborUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.clQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.tabCapacity = new Apzon.Controls.Controls.XtraTabPageEx();
		this.tabFixedAsset = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdFixedAsset = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripFixasset = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvFixedAsset = new Apzon.Controls.Grid.GridViewEx();
		this.clItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.repItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.clStat = new Apzon.Controls.Grid.GridColumnEx();
		this.repStatus2 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clLocation = new Apzon.Controls.Grid.GridColumnEx();
		this.clTechnician = new Apzon.Controls.Grid.GridColumnEx();
		this.clEmployee = new Apzon.Controls.Grid.GridColumnEx();
		this.clRemak = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemMemoEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemMemoEdit();
		this.tabCosting = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grdCosting = new Apzon.Controls.Grid.GridControlEx();
		this.gvCosting = new Apzon.Controls.Grid.GridViewEx();
		this.clStdCost = new Apzon.Controls.Grid.GridColumnEx();
		this.repStdCost = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.clDefaultCost = new Apzon.Controls.Grid.GridColumnEx();
		this.clCostExpAct = new Apzon.Controls.Grid.GridColumnEx();
		this.repAcctCode2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clWipAct = new Apzon.Controls.Grid.GridColumnEx();
		this.repAcctCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.clSetupCost = new Apzon.Controls.Grid.GridColumnEx();
		this.repCheck = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.clMachineCost = new Apzon.Controls.Grid.GridColumnEx();
		this.clLaborCost = new Apzon.Controls.Grid.GridColumnEx();
		this.clOtherCost = new Apzon.Controls.Grid.GridColumnEx();
		this.repTinh = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		((System.ComponentModel.ISupportInitialize)this.gv_CapacityChildren).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repShowDay).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repUOM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repQuantity).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_CapacityMain).BeginInit();
		this.contextMenuStrip_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_CapacityMain).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repNumber).BeginInit();
		this.panel1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gpGeneral).BeginInit();
		this.gpGeneral.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdGeneral).BeginInit();
		this.contextMenuStrip_Docs.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvGeneral).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repOcc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Ocr2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Ocr3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Ocr4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Ocr5).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repControlKey).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.tabCapacity.SuspendLayout();
		this.tabFixedAsset.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdFixedAsset).BeginInit();
		this.contextMenuStripFixasset.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvFixedAsset).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repStatus2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit2).BeginInit();
		this.tabCosting.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdCosting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvCosting).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repStdCost).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repAcctCode2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repAcctCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repCheck).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repTinh).BeginInit();
		base.SuspendLayout();
		this.gv_CapacityChildren.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_CapacityChildren.ColumnKey = null;
		this.gv_CapacityChildren.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[15]
		{
			this.colResCode, this.colLineNum, this.colWeekDay, this.colUOM, this.clCapfactor1, this.clCapFactor2, this.clCapfactor3, this.clCapFactor4, this.clCapTotal, this.colRemarks,
			this.clStartTime, this.clFinishTime, this.clBreaksTime, this.clvalidFrom, this.clvalidTo
		});
		this.gv_CapacityChildren.DisableFormSetting = false;
		this.gv_CapacityChildren.DisplayException = false;
		this.gv_CapacityChildren.EditColumn = null;
		this.gv_CapacityChildren.FieldCurrency = null;
		this.gv_CapacityChildren.FieldRate = null;
		this.gv_CapacityChildren.FormId = null;
		this.gv_CapacityChildren.GridControl = this.grd_CapacityMain;
		this.gv_CapacityChildren.IgnoreFilter = false;
		this.gv_CapacityChildren.IndexEditRow = -1;
		this.gv_CapacityChildren.IndicatorWidth = 40;
		this.gv_CapacityChildren.IsApzTable = false;
		this.gv_CapacityChildren.IsInsertNewRow = false;
		this.gv_CapacityChildren.IsLoadingData = false;
		this.gv_CapacityChildren.IsMappingContextAction = false;
		this.gv_CapacityChildren.ItemId = null;
		this.gv_CapacityChildren.LstColumnAllowBatch = null;
		this.gv_CapacityChildren.LstColumnNotAllowBatch = null;
		this.gv_CapacityChildren.LstColumnNotEditable = null;
		this.gv_CapacityChildren.LstColumNotSetting = null;
		this.gv_CapacityChildren.Name = "gv_CapacityChildren";
		this.gv_CapacityChildren.OptionsView.ShowGroupPanel = false;
		this.gv_CapacityChildren.UseBatchMod = false;
		this.gv_CapacityChildren.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_CapacityChildren_RowStyle);
		this.gv_CapacityChildren.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CapacityChildren_CellValueChanged);
		this.gv_CapacityChildren.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CapacityChildren_CellValueChanging);
		this.colResCode.Caption = "ResCode";
		this.colResCode.CurrencyDateField = null;
		this.colResCode.CurrencyField = null;
		this.colResCode.CurrencyFixed = null;
		this.colResCode.CurrencyFixedRate = 0.0;
		this.colResCode.CurrencyRateField = null;
		this.colResCode.FieldName = "ResCode";
		this.colResCode.Name = "colResCode";
		this.colResCode.PrimaryKey = false;
		this.colResCode.UnitField = null;
		this.colResCode.Width = 72;
		this.colLineNum.Caption = "LineNum";
		this.colLineNum.CurrencyDateField = null;
		this.colLineNum.CurrencyField = null;
		this.colLineNum.CurrencyFixed = null;
		this.colLineNum.CurrencyFixedRate = 0.0;
		this.colLineNum.CurrencyRateField = null;
		this.colLineNum.FieldName = "LineNum";
		this.colLineNum.Name = "colLineNum";
		this.colLineNum.PrimaryKey = false;
		this.colLineNum.UnitField = null;
		this.colLineNum.Width = 64;
		this.colWeekDay.Caption = "Days";
		this.colWeekDay.ColumnEdit = this.repShowDay;
		this.colWeekDay.CurrencyDateField = null;
		this.colWeekDay.CurrencyField = null;
		this.colWeekDay.CurrencyFixed = null;
		this.colWeekDay.CurrencyFixedRate = 0.0;
		this.colWeekDay.CurrencyRateField = null;
		this.colWeekDay.FieldName = "WeekDay";
		this.colWeekDay.Name = "colWeekDay";
		this.colWeekDay.OptionsColumn.AllowEdit = false;
		this.colWeekDay.PrimaryKey = false;
		this.colWeekDay.UnitField = null;
		this.colWeekDay.Visible = true;
		this.colWeekDay.VisibleIndex = 0;
		this.colWeekDay.Width = 73;
		this.repShowDay.AutoHeight = false;
		this.repShowDay.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repShowDay.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Day Of Week"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.repShowDay.DisplayMember = "Name";
		this.repShowDay.Name = "repShowDay";
		this.repShowDay.NullText = "";
		this.repShowDay.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.repShowDay.ValueMember = "ID";
		this.colUOM.Caption = "UoM";
		this.colUOM.ColumnEdit = this.repUOM;
		this.colUOM.CurrencyDateField = null;
		this.colUOM.CurrencyField = null;
		this.colUOM.CurrencyFixed = null;
		this.colUOM.CurrencyFixedRate = 0.0;
		this.colUOM.CurrencyRateField = null;
		this.colUOM.FieldName = "UnitOfMsr";
		this.colUOM.Name = "colUOM";
		this.colUOM.PrimaryKey = false;
		this.colUOM.UnitField = null;
		this.colUOM.Visible = true;
		this.colUOM.VisibleIndex = 1;
		this.colUOM.Width = 66;
		this.repUOM.AutoHeight = false;
		this.repUOM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repUOM.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.repUOM.DisplayMember = "Name";
		this.repUOM.Name = "repUOM";
		this.repUOM.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.Standard;
		this.repUOM.ValueMember = "ID";
		this.clCapfactor1.Caption = "Capfactor 1";
		this.clCapfactor1.ColumnEdit = this.repQuantity;
		this.clCapfactor1.CurrencyDateField = null;
		this.clCapfactor1.CurrencyField = null;
		this.clCapfactor1.CurrencyFixed = null;
		this.clCapfactor1.CurrencyFixedRate = 0.0;
		this.clCapfactor1.CurrencyRateField = null;
		this.clCapfactor1.FieldName = "CapFactor1";
		this.clCapfactor1.Name = "clCapfactor1";
		this.clCapfactor1.PrimaryKey = false;
		this.clCapfactor1.UnitField = null;
		this.clCapfactor1.Visible = true;
		this.clCapfactor1.VisibleIndex = 2;
		this.clCapfactor1.Width = 89;
		this.repQuantity.AutoHeight = false;
		this.repQuantity.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repQuantity.Name = "repQuantity";
		this.clCapFactor2.Caption = "CapFactor 2";
		this.clCapFactor2.ColumnEdit = this.repQuantity;
		this.clCapFactor2.CurrencyDateField = null;
		this.clCapFactor2.CurrencyField = null;
		this.clCapFactor2.CurrencyFixed = null;
		this.clCapFactor2.CurrencyFixedRate = 0.0;
		this.clCapFactor2.CurrencyRateField = null;
		this.clCapFactor2.FieldName = "CapFactor2";
		this.clCapFactor2.Name = "clCapFactor2";
		this.clCapFactor2.PrimaryKey = false;
		this.clCapFactor2.UnitField = null;
		this.clCapFactor2.Visible = true;
		this.clCapFactor2.VisibleIndex = 3;
		this.clCapfactor3.Caption = "CapFactor 3";
		this.clCapfactor3.ColumnEdit = this.repQuantity;
		this.clCapfactor3.CurrencyDateField = null;
		this.clCapfactor3.CurrencyField = null;
		this.clCapfactor3.CurrencyFixed = null;
		this.clCapfactor3.CurrencyFixedRate = 0.0;
		this.clCapfactor3.CurrencyRateField = null;
		this.clCapfactor3.FieldName = "CapFactor3";
		this.clCapfactor3.Name = "clCapfactor3";
		this.clCapfactor3.PrimaryKey = false;
		this.clCapfactor3.UnitField = null;
		this.clCapfactor3.Visible = true;
		this.clCapfactor3.VisibleIndex = 4;
		this.clCapfactor3.Width = 80;
		this.clCapFactor4.Caption = "CapFactor 4";
		this.clCapFactor4.ColumnEdit = this.repQuantity;
		this.clCapFactor4.CurrencyDateField = null;
		this.clCapFactor4.CurrencyField = null;
		this.clCapFactor4.CurrencyFixed = null;
		this.clCapFactor4.CurrencyFixedRate = 0.0;
		this.clCapFactor4.CurrencyRateField = null;
		this.clCapFactor4.FieldName = "CapFactor4";
		this.clCapFactor4.Name = "clCapFactor4";
		this.clCapFactor4.PrimaryKey = false;
		this.clCapFactor4.UnitField = null;
		this.clCapFactor4.Visible = true;
		this.clCapFactor4.VisibleIndex = 5;
		this.clCapFactor4.Width = 81;
		this.clCapTotal.Caption = "CapTotal";
		this.clCapTotal.CurrencyDateField = null;
		this.clCapTotal.CurrencyField = null;
		this.clCapTotal.CurrencyFixed = null;
		this.clCapTotal.CurrencyFixedRate = 0.0;
		this.clCapTotal.CurrencyRateField = null;
		this.clCapTotal.FieldName = "CapTotal";
		this.clCapTotal.Name = "clCapTotal";
		this.clCapTotal.OptionsColumn.AllowEdit = false;
		this.clCapTotal.PrimaryKey = false;
		this.clCapTotal.UnitField = null;
		this.clCapTotal.Visible = true;
		this.clCapTotal.VisibleIndex = 6;
		this.clCapTotal.Width = 65;
		this.colRemarks.Caption = "Remarks";
		this.colRemarks.ColumnEdit = this.repositoryItemMemoEdit1;
		this.colRemarks.CurrencyDateField = null;
		this.colRemarks.CurrencyField = null;
		this.colRemarks.CurrencyFixed = null;
		this.colRemarks.CurrencyFixedRate = 0.0;
		this.colRemarks.CurrencyRateField = null;
		this.colRemarks.FieldName = "Remarks";
		this.colRemarks.Name = "colRemarks";
		this.colRemarks.PrimaryKey = false;
		this.colRemarks.UnitField = null;
		this.colRemarks.Visible = true;
		this.colRemarks.VisibleIndex = 7;
		this.colRemarks.Width = 106;
		this.repositoryItemMemoEdit1.Name = "repositoryItemMemoEdit1";
		this.clStartTime.Caption = "Start Time";
		this.clStartTime.CurrencyDateField = null;
		this.clStartTime.CurrencyField = null;
		this.clStartTime.CurrencyFixed = null;
		this.clStartTime.CurrencyFixedRate = 0.0;
		this.clStartTime.CurrencyRateField = null;
		this.clStartTime.FieldName = "StartTime";
		this.clStartTime.Name = "clStartTime";
		this.clStartTime.PrimaryKey = false;
		this.clStartTime.UnitField = null;
		this.clStartTime.Width = 20;
		this.clFinishTime.Caption = "Finish Time";
		this.clFinishTime.CurrencyDateField = null;
		this.clFinishTime.CurrencyField = null;
		this.clFinishTime.CurrencyFixed = null;
		this.clFinishTime.CurrencyFixedRate = 0.0;
		this.clFinishTime.CurrencyRateField = null;
		this.clFinishTime.FieldName = "FinishTime";
		this.clFinishTime.Name = "clFinishTime";
		this.clFinishTime.PrimaryKey = false;
		this.clFinishTime.UnitField = null;
		this.clFinishTime.Width = 20;
		this.clBreaksTime.Caption = "Breaks Time";
		this.clBreaksTime.CurrencyDateField = null;
		this.clBreaksTime.CurrencyField = null;
		this.clBreaksTime.CurrencyFixed = null;
		this.clBreaksTime.CurrencyFixedRate = 0.0;
		this.clBreaksTime.CurrencyRateField = null;
		this.clBreaksTime.FieldName = "BreaksTime";
		this.clBreaksTime.Name = "clBreaksTime";
		this.clBreaksTime.PrimaryKey = false;
		this.clBreaksTime.UnitField = null;
		this.clBreaksTime.Width = 20;
		this.clvalidFrom.Caption = "From Date";
		this.clvalidFrom.CurrencyDateField = null;
		this.clvalidFrom.CurrencyField = null;
		this.clvalidFrom.CurrencyFixed = null;
		this.clvalidFrom.CurrencyFixedRate = 0.0;
		this.clvalidFrom.CurrencyRateField = null;
		this.clvalidFrom.FieldName = "validFrom";
		this.clvalidFrom.Name = "clvalidFrom";
		this.clvalidFrom.PrimaryKey = false;
		this.clvalidFrom.UnitField = null;
		this.clvalidFrom.Width = 20;
		this.clvalidTo.Caption = "To Date";
		this.clvalidTo.CurrencyDateField = null;
		this.clvalidTo.CurrencyField = null;
		this.clvalidTo.CurrencyFixed = null;
		this.clvalidTo.CurrencyFixedRate = 0.0;
		this.clvalidTo.CurrencyRateField = null;
		this.clvalidTo.FieldName = "validTo";
		this.clvalidTo.Name = "clvalidTo";
		this.clvalidTo.PrimaryKey = false;
		this.clvalidTo.UnitField = null;
		this.clvalidTo.Width = 20;
		this.grd_CapacityMain.AllowPasteExcel = true;
		this.grd_CapacityMain.ClipboardData = null;
		this.grd_CapacityMain.ContextMenuStrip = this.contextMenuStrip_Doc;
		this.grd_CapacityMain.Dock = System.Windows.Forms.DockStyle.Fill;
		gridLevelNode.LevelTemplate = this.gv_CapacityChildren;
		gridLevelNode.RelationName = "Level1";
		this.grd_CapacityMain.LevelTree.Nodes.AddRange(new DevExpress.XtraGrid.GridLevelNode[1] { gridLevelNode });
		this.grd_CapacityMain.Location = new System.Drawing.Point(0, 0);
		this.grd_CapacityMain.MainView = this.gv_CapacityMain;
		this.grd_CapacityMain.Name = "grd_CapacityMain";
		this.grd_CapacityMain.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.repShowDay, this.repUOM, this.repNumber, this.repQuantity, this.repositoryItemMemoEdit1 });
		this.grd_CapacityMain.Size = new System.Drawing.Size(681, 158);
		this.grd_CapacityMain.TabIndex = 0;
		this.grd_CapacityMain.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[2] { this.gv_CapacityMain, this.gv_CapacityChildren });
		this.contextMenuStrip_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.insertNewDataToolStripMenuItem, this.deleteToolStripMenuItem1 });
		this.contextMenuStrip_Doc.Name = "contextMenuStrip_Doc";
		this.contextMenuStrip_Doc.Size = new System.Drawing.Size(150, 48);
		this.insertNewDataToolStripMenuItem.Name = "insertNewDataToolStripMenuItem";
		this.insertNewDataToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
		this.insertNewDataToolStripMenuItem.Text = "Add New Row";
		this.insertNewDataToolStripMenuItem.Click += new System.EventHandler(this.insertNewDataToolStripMenuItem_Click);
		this.deleteToolStripMenuItem1.Name = "deleteToolStripMenuItem1";
		this.deleteToolStripMenuItem1.Size = new System.Drawing.Size(149, 22);
		this.deleteToolStripMenuItem1.Text = "Delete";
		this.deleteToolStripMenuItem1.Click += new System.EventHandler(this.deleteToolStripMenuItem1_Click);
		this.gv_CapacityMain.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_CapacityMain.ColumnKey = null;
		this.gv_CapacityMain.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.clFromDate, this.clToDate });
		this.gv_CapacityMain.DisableFormSetting = false;
		this.gv_CapacityMain.DisplayException = false;
		this.gv_CapacityMain.EditColumn = null;
		this.gv_CapacityMain.FieldCurrency = null;
		this.gv_CapacityMain.FieldRate = null;
		this.gv_CapacityMain.FormId = null;
		this.gv_CapacityMain.GridControl = this.grd_CapacityMain;
		this.gv_CapacityMain.IgnoreFilter = false;
		this.gv_CapacityMain.IndexEditRow = -1;
		this.gv_CapacityMain.IndicatorWidth = 40;
		this.gv_CapacityMain.IsApzTable = false;
		this.gv_CapacityMain.IsInsertNewRow = false;
		this.gv_CapacityMain.IsLoadingData = false;
		this.gv_CapacityMain.IsMappingContextAction = false;
		this.gv_CapacityMain.ItemId = null;
		this.gv_CapacityMain.LstColumnAllowBatch = null;
		this.gv_CapacityMain.LstColumnNotAllowBatch = null;
		this.gv_CapacityMain.LstColumnNotEditable = null;
		this.gv_CapacityMain.LstColumNotSetting = null;
		this.gv_CapacityMain.Name = "gv_CapacityMain";
		this.gv_CapacityMain.OptionsDetail.ShowDetailTabs = false;
		this.gv_CapacityMain.OptionsView.ShowGroupPanel = false;
		this.gv_CapacityMain.UseBatchMod = false;
		this.gv_CapacityMain.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_CapacityMain_RowStyle);
		this.gv_CapacityMain.MasterRowGetChildList += new DevExpress.XtraGrid.Views.Grid.MasterRowGetChildListEventHandler(this.gv_CapacityMain_MasterRowGetChildList);
		this.gv_CapacityMain.MasterRowGetRelationName += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationNameEventHandler(this.gv_CapacityMain_MasterRowGetRelationName);
		this.gv_CapacityMain.MasterRowGetRelationCount += new DevExpress.XtraGrid.Views.Grid.MasterRowGetRelationCountEventHandler(this.gv_CapacityMain_MasterRowGetRelationCount);
		this.gv_CapacityMain.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_CapacityMain_CellValueChanged);
		this.clFromDate.Caption = "From Date";
		this.clFromDate.CurrencyDateField = null;
		this.clFromDate.CurrencyField = null;
		this.clFromDate.CurrencyFixed = null;
		this.clFromDate.CurrencyFixedRate = 0.0;
		this.clFromDate.CurrencyRateField = null;
		this.clFromDate.DisplayFormat.FormatString = "d";
		this.clFromDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.clFromDate.FieldName = "validFrom";
		this.clFromDate.Name = "clFromDate";
		this.clFromDate.PrimaryKey = false;
		this.clFromDate.UnitField = null;
		this.clFromDate.Visible = true;
		this.clFromDate.VisibleIndex = 0;
		this.clToDate.Caption = "To Date";
		this.clToDate.CurrencyDateField = null;
		this.clToDate.CurrencyField = null;
		this.clToDate.CurrencyFixed = null;
		this.clToDate.CurrencyFixedRate = 0.0;
		this.clToDate.CurrencyRateField = null;
		this.clToDate.DisplayFormat.FormatString = "d";
		this.clToDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.clToDate.FieldName = "validTo";
		this.clToDate.Name = "clToDate";
		this.clToDate.PrimaryKey = false;
		this.clToDate.UnitField = null;
		this.clToDate.Visible = true;
		this.clToDate.VisibleIndex = 1;
		this.repNumber.AutoHeight = false;
		this.repNumber.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repNumber.Name = "repNumber";
		this.panel1.Controls.Add(this.btRefresh);
		this.panel1.Controls.Add(this.btCancel);
		this.panel1.Controls.Add(this.btSave);
		this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panel1.Location = new System.Drawing.Point(0, 335);
		this.panel1.Name = "panel1";
		this.panel1.Size = new System.Drawing.Size(687, 33);
		this.panel1.TabIndex = 62;
		this.panel1.Tag = "ss";
		this.panel1.Paint += new System.Windows.Forms.PaintEventHandler(this.panel1_Paint);
		this.btRefresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btRefresh.EditControl = null;
		this.btRefresh.EditValue = "Refresh";
		this.btRefresh.FieldName = null;
		this.btRefresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btRefresh.ImageOptions.Image");
		this.btRefresh.IsSearchControl = false;
		this.btRefresh.Location = new System.Drawing.Point(217, 7);
		this.btRefresh.Name = "btRefresh";
		this.btRefresh.ReadOnly = false;
		this.btRefresh.SettingInVisible = false;
		this.btRefresh.SettingReadonly = false;
		this.btRefresh.Size = new System.Drawing.Size(99, 19);
		this.btRefresh.TabIndex = 44;
		this.btRefresh.Text = "Refresh";
		this.btRefresh.Click += new System.EventHandler(this.btRefresh_Click);
		this.btCancel.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btCancel.Appearance.Options.UseFont = true;
		this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(113, 7);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(99, 19);
		this.btCancel.TabIndex = 14;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btSave.Appearance.Font = new System.Drawing.Font("Tahoma", 7.5f);
		this.btSave.Appearance.Options.UseFont = true;
		this.btSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btSave.EditControl = null;
		this.btSave.EditValue = "Save";
		this.btSave.FieldName = null;
		this.btSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btSave.ImageOptions.Image");
		this.btSave.IsSearchControl = false;
		this.btSave.Location = new System.Drawing.Point(9, 7);
		this.btSave.Name = "btSave";
		this.btSave.ReadOnly = false;
		this.btSave.SettingInVisible = false;
		this.btSave.SettingReadonly = false;
		this.btSave.Size = new System.Drawing.Size(99, 19);
		this.btSave.TabIndex = 13;
		this.btSave.Text = "Save";
		this.btSave.Click += new System.EventHandler(this.btSave_Click);
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.gpGeneral);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.groupControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(687, 335);
		this.splitContainerControl1.SplitterPosition = 117;
		this.splitContainerControl1.TabIndex = 63;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.gpGeneral.Controls.Add(this.grdGeneral);
		this.gpGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
		this.gpGeneral.Location = new System.Drawing.Point(0, 0);
		this.gpGeneral.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.gpGeneral.Name = "gpGeneral";
		this.gpGeneral.Size = new System.Drawing.Size(687, 117);
		this.gpGeneral.TabIndex = 0;
		this.gpGeneral.Text = "Genral";
		this.grdGeneral.AllowPasteExcel = true;
		this.grdGeneral.ClipboardData = null;
		this.grdGeneral.ContextMenuStrip = this.contextMenuStrip_Docs;
		this.grdGeneral.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdGeneral.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdGeneral.Location = new System.Drawing.Point(2, 23);
		this.grdGeneral.MainView = this.gvGeneral;
		this.grdGeneral.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdGeneral.Name = "grdGeneral";
		this.grdGeneral.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[11]
		{
			this.repStatus, this.repType, this.repGroup, this.repOcc, this.rep_Ocr2, this.rep_Ocr3, this.rep_Ocr5, this.rep_Ocr4, this.repControlKey, this.repositoryItemMemoEdit3,
			this.repositoryItemMemoExEdit1
		});
		this.grdGeneral.Size = new System.Drawing.Size(683, 92);
		this.grdGeneral.TabIndex = 0;
		this.grdGeneral.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvGeneral });
		this.contextMenuStrip_Docs.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewRowToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuStrip_Docs.Name = "contextMenuStrip_Docs";
		this.contextMenuStrip_Docs.Size = new System.Drawing.Size(128, 48);
		this.addNewRowToolStripMenuItem.Name = "addNewRowToolStripMenuItem";
		this.addNewRowToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
		this.addNewRowToolStripMenuItem.Text = "Duplacate";
		this.addNewRowToolStripMenuItem.Click += new System.EventHandler(this.addNewRowToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gvGeneral.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvGeneral.ColumnKey = null;
		this.gvGeneral.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[29]
		{
			this.clResCode, this.clResName, this.clResFName, this.clResType, this.clResGrpCod, this.clUnitOfMsr, this.clStatus, this.IssueMthd, this.clPerInCharge, this.clCodeBars,
			this.clNumResUnit, this.empID, this.colDist1, this.clDist2, this.clDist3, this.clDist4, this.clDis5, this.clPlant, this.clCtrlArea, this.clCtrlkey,
			this.clRemarks, this.clCalendar, this.clSetupTime, this.clSetupUoM, this.clMachineTime, this.clMachineUoM, this.clLaborTime, this.clLaborUoM, this.clQuantity
		});
		this.gvGeneral.DisableFormSetting = false;
		this.gvGeneral.DisplayException = false;
		this.gvGeneral.EditColumn = null;
		this.gvGeneral.FieldCurrency = null;
		this.gvGeneral.FieldRate = null;
		this.gvGeneral.FormId = null;
		this.gvGeneral.GridControl = this.grdGeneral;
		this.gvGeneral.IgnoreFilter = false;
		this.gvGeneral.IndexEditRow = -1;
		this.gvGeneral.IndicatorWidth = 40;
		this.gvGeneral.IsApzTable = false;
		this.gvGeneral.IsInsertNewRow = false;
		this.gvGeneral.IsLoadingData = false;
		this.gvGeneral.IsMappingContextAction = false;
		this.gvGeneral.ItemId = null;
		this.gvGeneral.LstColumnAllowBatch = null;
		this.gvGeneral.LstColumnNotAllowBatch = null;
		this.gvGeneral.LstColumnNotEditable = null;
		this.gvGeneral.LstColumNotSetting = null;
		this.gvGeneral.Name = "gvGeneral";
		this.gvGeneral.OptionsView.ColumnAutoWidth = false;
		this.gvGeneral.OptionsView.ShowGroupPanel = false;
		this.gvGeneral.UseBatchMod = false;
		this.gvGeneral.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvGeneral_CustomDrawRowIndicator);
		this.gvGeneral.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvGeneral_RowStyle);
		this.gvGeneral.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvGeneral_FocusedRowChanged);
		this.gvGeneral.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvGeneral_FocusedRowObjectChanged);
		this.gvGeneral.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvGeneral_CellValueChanged);
		this.gvGeneral.MouseMove += new System.Windows.Forms.MouseEventHandler(this.gvGeneral_MouseMove);
		this.gvGeneral.MouseLeave += new System.EventHandler(this.gvGeneral_MouseLeave);
		this.clResCode.Caption = "Resource No";
		this.clResCode.CurrencyDateField = null;
		this.clResCode.CurrencyField = null;
		this.clResCode.CurrencyFixed = null;
		this.clResCode.CurrencyFixedRate = 0.0;
		this.clResCode.CurrencyRateField = null;
		this.clResCode.FieldName = "ResCode";
		this.clResCode.Name = "clResCode";
		this.clResCode.PrimaryKey = true;
		this.clResCode.UnitField = null;
		this.clResCode.Visible = true;
		this.clResCode.VisibleIndex = 0;
		this.clResCode.Width = 78;
		this.clResName.Caption = "Resource Name";
		this.clResName.CurrencyDateField = null;
		this.clResName.CurrencyField = null;
		this.clResName.CurrencyFixed = null;
		this.clResName.CurrencyFixedRate = 0.0;
		this.clResName.CurrencyRateField = null;
		this.clResName.FieldName = "ResName";
		this.clResName.Name = "clResName";
		this.clResName.PrimaryKey = false;
		this.clResName.UnitField = null;
		this.clResName.Visible = true;
		this.clResName.VisibleIndex = 1;
		this.clResName.Width = 117;
		this.clResFName.Caption = "ResFName";
		this.clResFName.CurrencyDateField = null;
		this.clResFName.CurrencyField = null;
		this.clResFName.CurrencyFixed = null;
		this.clResFName.CurrencyFixedRate = 0.0;
		this.clResFName.CurrencyRateField = null;
		this.clResFName.FieldName = "ResFName";
		this.clResFName.Name = "clResFName";
		this.clResFName.PrimaryKey = false;
		this.clResFName.UnitField = null;
		this.clResType.Caption = "Type";
		this.clResType.ColumnEdit = this.repType;
		this.clResType.CurrencyDateField = null;
		this.clResType.CurrencyField = null;
		this.clResType.CurrencyFixed = null;
		this.clResType.CurrencyFixedRate = 0.0;
		this.clResType.CurrencyRateField = null;
		this.clResType.FieldName = "ResType";
		this.clResType.Name = "clResType";
		this.clResType.PrimaryKey = false;
		this.clResType.UnitField = null;
		this.clResType.Visible = true;
		this.clResType.VisibleIndex = 2;
		this.clResType.Width = 95;
		this.repType.AutoHeight = false;
		this.repType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Type")
		});
		this.repType.DisplayMember = "Name";
		this.repType.Name = "repType";
		this.repType.ValueMember = "ID";
		this.clResGrpCod.Caption = "Group";
		this.clResGrpCod.ColumnEdit = this.repGroup;
		this.clResGrpCod.CurrencyDateField = null;
		this.clResGrpCod.CurrencyField = null;
		this.clResGrpCod.CurrencyFixed = null;
		this.clResGrpCod.CurrencyFixedRate = 0.0;
		this.clResGrpCod.CurrencyRateField = null;
		this.clResGrpCod.FieldName = "ResGrpCod";
		this.clResGrpCod.Name = "clResGrpCod";
		this.clResGrpCod.PrimaryKey = false;
		this.clResGrpCod.UnitField = null;
		this.clResGrpCod.Visible = true;
		this.clResGrpCod.VisibleIndex = 3;
		this.clResGrpCod.Width = 103;
		this.repGroup.AutoHeight = false;
		this.repGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Group")
		});
		this.repGroup.DisplayMember = "Name";
		this.repGroup.Name = "repGroup";
		this.repGroup.ValueMember = "ID";
		this.clUnitOfMsr.Caption = "Base UoM";
		this.clUnitOfMsr.CurrencyDateField = null;
		this.clUnitOfMsr.CurrencyField = null;
		this.clUnitOfMsr.CurrencyFixed = null;
		this.clUnitOfMsr.CurrencyFixedRate = 0.0;
		this.clUnitOfMsr.CurrencyRateField = null;
		this.clUnitOfMsr.FieldName = "UnitOfMsr";
		this.clUnitOfMsr.Name = "clUnitOfMsr";
		this.clUnitOfMsr.PrimaryKey = false;
		this.clUnitOfMsr.UnitField = null;
		this.clUnitOfMsr.Visible = true;
		this.clUnitOfMsr.VisibleIndex = 13;
		this.clStatus.Caption = "Status";
		this.clStatus.ColumnEdit = this.repStatus;
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
		this.clStatus.VisibleIndex = 4;
		this.clStatus.Width = 56;
		this.repStatus.AutoHeight = false;
		this.repStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Status")
		});
		this.repStatus.DisplayMember = "Name";
		this.repStatus.Name = "repStatus";
		this.repStatus.ValueMember = "ID";
		this.IssueMthd.Caption = "IssueMthd";
		this.IssueMthd.CurrencyDateField = null;
		this.IssueMthd.CurrencyField = null;
		this.IssueMthd.CurrencyFixed = null;
		this.IssueMthd.CurrencyFixedRate = 0.0;
		this.IssueMthd.CurrencyRateField = null;
		this.IssueMthd.FieldName = "IssueMthd";
		this.IssueMthd.Name = "IssueMthd";
		this.IssueMthd.PrimaryKey = false;
		this.IssueMthd.UnitField = null;
		this.clPerInCharge.Caption = "Per In Charge";
		this.clPerInCharge.CurrencyDateField = null;
		this.clPerInCharge.CurrencyField = null;
		this.clPerInCharge.CurrencyFixed = null;
		this.clPerInCharge.CurrencyFixedRate = 0.0;
		this.clPerInCharge.CurrencyRateField = null;
		this.clPerInCharge.FieldName = "PerInCharge";
		this.clPerInCharge.Name = "clPerInCharge";
		this.clPerInCharge.PrimaryKey = false;
		this.clPerInCharge.UnitField = null;
		this.clPerInCharge.Width = 159;
		this.clCodeBars.Caption = "Bar Code";
		this.clCodeBars.CurrencyDateField = null;
		this.clCodeBars.CurrencyField = null;
		this.clCodeBars.CurrencyFixed = null;
		this.clCodeBars.CurrencyFixedRate = 0.0;
		this.clCodeBars.CurrencyRateField = null;
		this.clCodeBars.FieldName = "CodeBars";
		this.clCodeBars.Name = "clCodeBars";
		this.clCodeBars.PrimaryKey = false;
		this.clCodeBars.UnitField = null;
		this.clCodeBars.Visible = true;
		this.clCodeBars.VisibleIndex = 11;
		this.clCodeBars.Width = 98;
		this.clNumResUnit.Caption = "NumResUnit";
		this.clNumResUnit.CurrencyDateField = null;
		this.clNumResUnit.CurrencyField = null;
		this.clNumResUnit.CurrencyFixed = null;
		this.clNumResUnit.CurrencyFixedRate = 0.0;
		this.clNumResUnit.CurrencyRateField = null;
		this.clNumResUnit.FieldName = "NumResUnit";
		this.clNumResUnit.Name = "clNumResUnit";
		this.clNumResUnit.PrimaryKey = false;
		this.clNumResUnit.UnitField = null;
		this.clNumResUnit.Visible = true;
		this.clNumResUnit.VisibleIndex = 23;
		this.empID.Caption = "empID";
		this.empID.CurrencyDateField = null;
		this.empID.CurrencyField = null;
		this.empID.CurrencyFixed = null;
		this.empID.CurrencyFixedRate = 0.0;
		this.empID.CurrencyRateField = null;
		this.empID.FieldName = "empID";
		this.empID.Name = "empID";
		this.empID.PrimaryKey = false;
		this.empID.UnitField = null;
		this.colDist1.Caption = "Dist. Rule 1";
		this.colDist1.ColumnEdit = this.repOcc;
		this.colDist1.CurrencyDateField = null;
		this.colDist1.CurrencyField = null;
		this.colDist1.CurrencyFixed = null;
		this.colDist1.CurrencyFixedRate = 0.0;
		this.colDist1.CurrencyRateField = null;
		this.colDist1.FieldName = "OcrCode";
		this.colDist1.Name = "colDist1";
		this.colDist1.PrimaryKey = false;
		this.colDist1.UnitField = null;
		this.colDist1.Visible = true;
		this.colDist1.VisibleIndex = 6;
		this.repOcc.AutoHeight = false;
		this.repOcc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repOcc.Name = "repOcc";
		this.repOcc.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repOcc_ButtonClick);
		this.clDist2.Caption = "Dist. Rule 2";
		this.clDist2.ColumnEdit = this.rep_Ocr2;
		this.clDist2.CurrencyDateField = null;
		this.clDist2.CurrencyField = null;
		this.clDist2.CurrencyFixed = null;
		this.clDist2.CurrencyFixedRate = 0.0;
		this.clDist2.CurrencyRateField = null;
		this.clDist2.FieldName = "OcrCode2";
		this.clDist2.Name = "clDist2";
		this.clDist2.PrimaryKey = false;
		this.clDist2.UnitField = null;
		this.clDist2.Visible = true;
		this.clDist2.VisibleIndex = 7;
		this.rep_Ocr2.AutoHeight = false;
		this.rep_Ocr2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Ocr2.Name = "rep_Ocr2";
		this.rep_Ocr2.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Ocr2_ButtonClick);
		this.clDist3.Caption = "Dist. Rule 3";
		this.clDist3.ColumnEdit = this.rep_Ocr3;
		this.clDist3.CurrencyDateField = null;
		this.clDist3.CurrencyField = null;
		this.clDist3.CurrencyFixed = null;
		this.clDist3.CurrencyFixedRate = 0.0;
		this.clDist3.CurrencyRateField = null;
		this.clDist3.FieldName = "OcrCode3";
		this.clDist3.Name = "clDist3";
		this.clDist3.PrimaryKey = false;
		this.clDist3.UnitField = null;
		this.clDist3.Visible = true;
		this.clDist3.VisibleIndex = 8;
		this.rep_Ocr3.AutoHeight = false;
		this.rep_Ocr3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Ocr3.Name = "rep_Ocr3";
		this.rep_Ocr3.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Ocr3_ButtonClick);
		this.clDist4.Caption = "Dist. Rule 4 ";
		this.clDist4.ColumnEdit = this.rep_Ocr4;
		this.clDist4.CurrencyDateField = null;
		this.clDist4.CurrencyField = null;
		this.clDist4.CurrencyFixed = null;
		this.clDist4.CurrencyFixedRate = 0.0;
		this.clDist4.CurrencyRateField = null;
		this.clDist4.FieldName = "OcrCode4";
		this.clDist4.Name = "clDist4";
		this.clDist4.PrimaryKey = false;
		this.clDist4.UnitField = null;
		this.clDist4.Visible = true;
		this.clDist4.VisibleIndex = 9;
		this.rep_Ocr4.AutoHeight = false;
		this.rep_Ocr4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Ocr4.Name = "rep_Ocr4";
		this.rep_Ocr4.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Ocr4_ButtonClick);
		this.clDis5.Caption = "Dist. Rule 5";
		this.clDis5.ColumnEdit = this.rep_Ocr5;
		this.clDis5.CurrencyDateField = null;
		this.clDis5.CurrencyField = null;
		this.clDis5.CurrencyFixed = null;
		this.clDis5.CurrencyFixedRate = 0.0;
		this.clDis5.CurrencyRateField = null;
		this.clDis5.FieldName = "OcrCode5";
		this.clDis5.Name = "clDis5";
		this.clDis5.PrimaryKey = false;
		this.clDis5.UnitField = null;
		this.clDis5.Visible = true;
		this.clDis5.VisibleIndex = 10;
		this.rep_Ocr5.AutoHeight = false;
		this.rep_Ocr5.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Ocr5.Name = "rep_Ocr5";
		this.rep_Ocr5.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Ocr5_ButtonClick);
		this.clPlant.Caption = "Plant";
		this.clPlant.CurrencyDateField = null;
		this.clPlant.CurrencyField = null;
		this.clPlant.CurrencyFixed = null;
		this.clPlant.CurrencyFixedRate = 0.0;
		this.clPlant.CurrencyRateField = null;
		this.clPlant.FieldName = "Plant";
		this.clPlant.Name = "clPlant";
		this.clPlant.PrimaryKey = false;
		this.clPlant.UnitField = null;
		this.clPlant.Visible = true;
		this.clPlant.VisibleIndex = 12;
		this.clCtrlArea.Caption = "CtrlArea";
		this.clCtrlArea.CurrencyDateField = null;
		this.clCtrlArea.CurrencyField = null;
		this.clCtrlArea.CurrencyFixed = null;
		this.clCtrlArea.CurrencyFixedRate = 0.0;
		this.clCtrlArea.CurrencyRateField = null;
		this.clCtrlArea.FieldName = "CtrlArea";
		this.clCtrlArea.Name = "clCtrlArea";
		this.clCtrlArea.PrimaryKey = false;
		this.clCtrlArea.UnitField = null;
		this.clCtrlkey.Caption = "Ctrlkey";
		this.clCtrlkey.ColumnEdit = this.repControlKey;
		this.clCtrlkey.CurrencyDateField = null;
		this.clCtrlkey.CurrencyField = null;
		this.clCtrlkey.CurrencyFixed = null;
		this.clCtrlkey.CurrencyFixedRate = 0.0;
		this.clCtrlkey.CurrencyRateField = null;
		this.clCtrlkey.FieldName = "Ctrlkey";
		this.clCtrlkey.Name = "clCtrlkey";
		this.clCtrlkey.PrimaryKey = false;
		this.clCtrlkey.UnitField = null;
		this.clCtrlkey.Visible = true;
		this.clCtrlkey.VisibleIndex = 5;
		this.repControlKey.AutoHeight = false;
		this.repControlKey.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repControlKey.Name = "repControlKey";
		this.repControlKey.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repControlKey_ButtonClick);
		this.clRemarks.Caption = "Remark";
		this.clRemarks.ColumnEdit = this.repositoryItemMemoExEdit1;
		this.clRemarks.CurrencyDateField = null;
		this.clRemarks.CurrencyField = null;
		this.clRemarks.CurrencyFixed = null;
		this.clRemarks.CurrencyFixedRate = 0.0;
		this.clRemarks.CurrencyRateField = null;
		this.clRemarks.FieldName = "Remark";
		this.clRemarks.Name = "clRemarks";
		this.clRemarks.PrimaryKey = false;
		this.clRemarks.UnitField = null;
		this.clRemarks.Visible = true;
		this.clRemarks.VisibleIndex = 22;
		this.repositoryItemMemoExEdit1.AutoHeight = false;
		this.repositoryItemMemoExEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemMemoExEdit1.Name = "repositoryItemMemoExEdit1";
		this.repositoryItemMemoExEdit1.ShowIcon = false;
		this.clCalendar.Caption = "Calendar";
		this.clCalendar.CurrencyDateField = null;
		this.clCalendar.CurrencyField = null;
		this.clCalendar.CurrencyFixed = null;
		this.clCalendar.CurrencyFixedRate = 0.0;
		this.clCalendar.CurrencyRateField = null;
		this.clCalendar.FieldName = "Calendar";
		this.clCalendar.Name = "clCalendar";
		this.clCalendar.PrimaryKey = false;
		this.clCalendar.UnitField = null;
		this.clCalendar.Visible = true;
		this.clCalendar.VisibleIndex = 14;
		this.clSetupTime.Caption = "Setup Time";
		this.clSetupTime.CurrencyDateField = null;
		this.clSetupTime.CurrencyField = null;
		this.clSetupTime.CurrencyFixed = null;
		this.clSetupTime.CurrencyFixedRate = 0.0;
		this.clSetupTime.CurrencyRateField = null;
		this.clSetupTime.FieldName = "SetupTime";
		this.clSetupTime.Name = "clSetupTime";
		this.clSetupTime.PrimaryKey = false;
		this.clSetupTime.UnitField = null;
		this.clSetupTime.Visible = true;
		this.clSetupTime.VisibleIndex = 15;
		this.clSetupUoM.Caption = "Setup UoM";
		this.clSetupUoM.CurrencyDateField = null;
		this.clSetupUoM.CurrencyField = null;
		this.clSetupUoM.CurrencyFixed = null;
		this.clSetupUoM.CurrencyFixedRate = 0.0;
		this.clSetupUoM.CurrencyRateField = null;
		this.clSetupUoM.FieldName = "SetupUoM";
		this.clSetupUoM.Name = "clSetupUoM";
		this.clSetupUoM.PrimaryKey = false;
		this.clSetupUoM.UnitField = null;
		this.clSetupUoM.Visible = true;
		this.clSetupUoM.VisibleIndex = 16;
		this.clMachineTime.Caption = "Machine Time";
		this.clMachineTime.CurrencyDateField = null;
		this.clMachineTime.CurrencyField = null;
		this.clMachineTime.CurrencyFixed = null;
		this.clMachineTime.CurrencyFixedRate = 0.0;
		this.clMachineTime.CurrencyRateField = null;
		this.clMachineTime.FieldName = "MachineTime";
		this.clMachineTime.Name = "clMachineTime";
		this.clMachineTime.PrimaryKey = false;
		this.clMachineTime.UnitField = null;
		this.clMachineTime.Visible = true;
		this.clMachineTime.VisibleIndex = 17;
		this.clMachineUoM.Caption = "Machine UoM";
		this.clMachineUoM.CurrencyDateField = null;
		this.clMachineUoM.CurrencyField = null;
		this.clMachineUoM.CurrencyFixed = null;
		this.clMachineUoM.CurrencyFixedRate = 0.0;
		this.clMachineUoM.CurrencyRateField = null;
		this.clMachineUoM.FieldName = "MachineUoM";
		this.clMachineUoM.Name = "clMachineUoM";
		this.clMachineUoM.PrimaryKey = false;
		this.clMachineUoM.UnitField = null;
		this.clMachineUoM.Visible = true;
		this.clMachineUoM.VisibleIndex = 18;
		this.clLaborTime.Caption = "Labor Time";
		this.clLaborTime.CurrencyDateField = null;
		this.clLaborTime.CurrencyField = null;
		this.clLaborTime.CurrencyFixed = null;
		this.clLaborTime.CurrencyFixedRate = 0.0;
		this.clLaborTime.CurrencyRateField = null;
		this.clLaborTime.FieldName = "LaborTime";
		this.clLaborTime.Name = "clLaborTime";
		this.clLaborTime.PrimaryKey = false;
		this.clLaborTime.UnitField = null;
		this.clLaborTime.Visible = true;
		this.clLaborTime.VisibleIndex = 19;
		this.clLaborUoM.Caption = "Labor UoM";
		this.clLaborUoM.CurrencyDateField = null;
		this.clLaborUoM.CurrencyField = null;
		this.clLaborUoM.CurrencyFixed = null;
		this.clLaborUoM.CurrencyFixedRate = 0.0;
		this.clLaborUoM.CurrencyRateField = null;
		this.clLaborUoM.FieldName = "LaborUoM";
		this.clLaborUoM.Name = "clLaborUoM";
		this.clLaborUoM.PrimaryKey = false;
		this.clLaborUoM.UnitField = null;
		this.clLaborUoM.Visible = true;
		this.clLaborUoM.VisibleIndex = 20;
		this.clQuantity.Caption = "Base Qty.";
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
		this.clQuantity.VisibleIndex = 21;
		this.repositoryItemMemoEdit3.Name = "repositoryItemMemoEdit3";
		this.groupControl2.Controls.Add(this.xtraTabControl1);
		this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl2.Location = new System.Drawing.Point(0, 0);
		this.groupControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(687, 208);
		this.groupControl2.TabIndex = 1;
		this.groupControl2.Text = "Detail";
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(2, 23);
		this.xtraTabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.tabCapacity;
		this.xtraTabControl1.Size = new System.Drawing.Size(683, 183);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.tabCapacity, this.tabFixedAsset, this.tabCosting });
		this.tabCapacity.Controls.Add(this.grd_CapacityMain);
		this.tabCapacity.EditControl = null;
		this.tabCapacity.EditValue = "Capacity";
		this.tabCapacity.FieldName = null;
		this.tabCapacity.IsSearchControl = false;
		this.tabCapacity.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tabCapacity.Name = "tabCapacity";
		this.tabCapacity.ReadOnly = false;
		this.tabCapacity.SettingInVisible = false;
		this.tabCapacity.SettingReadonly = false;
		this.tabCapacity.Size = new System.Drawing.Size(681, 158);
		this.tabCapacity.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabCapacity.Text = "Capacity";
		this.tabFixedAsset.Controls.Add(this.grdFixedAsset);
		this.tabFixedAsset.EditControl = null;
		this.tabFixedAsset.EditValue = "Fixed Asset";
		this.tabFixedAsset.FieldName = null;
		this.tabFixedAsset.IsSearchControl = false;
		this.tabFixedAsset.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tabFixedAsset.Name = "tabFixedAsset";
		this.tabFixedAsset.ReadOnly = false;
		this.tabFixedAsset.SettingInVisible = false;
		this.tabFixedAsset.SettingReadonly = false;
		this.tabFixedAsset.Size = new System.Drawing.Size(681, 165);
		this.tabFixedAsset.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabFixedAsset.Text = "Fixed Asset";
		this.grdFixedAsset.AllowPasteExcel = false;
		this.grdFixedAsset.ClipboardData = null;
		this.grdFixedAsset.ContextMenuStrip = this.contextMenuStripFixasset;
		this.grdFixedAsset.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdFixedAsset.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdFixedAsset.Location = new System.Drawing.Point(0, 0);
		this.grdFixedAsset.MainView = this.gvFixedAsset;
		this.grdFixedAsset.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdFixedAsset.Name = "grdFixedAsset";
		this.grdFixedAsset.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repStatus2, this.repItemCode, this.repositoryItemMemoEdit2 });
		this.grdFixedAsset.Size = new System.Drawing.Size(681, 165);
		this.grdFixedAsset.TabIndex = 1;
		this.grdFixedAsset.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvFixedAsset });
		this.contextMenuStripFixasset.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.deleteToolStripMenuItem2, this.addNewToolStripMenuItem });
		this.contextMenuStripFixasset.Name = "contextMenuStripFixasset";
		this.contextMenuStripFixasset.Size = new System.Drawing.Size(124, 48);
		this.deleteToolStripMenuItem2.Name = "deleteToolStripMenuItem2";
		this.deleteToolStripMenuItem2.Size = new System.Drawing.Size(123, 22);
		this.deleteToolStripMenuItem2.Text = "Delete";
		this.deleteToolStripMenuItem2.Click += new System.EventHandler(this.deleteToolStripMenuItem2_Click);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.gvFixedAsset.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvFixedAsset.ColumnKey = null;
		this.gvFixedAsset.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.clItemCode, this.clItemName, this.clStat, this.clLocation, this.clTechnician, this.clEmployee, this.clRemak });
		this.gvFixedAsset.DisableFormSetting = false;
		this.gvFixedAsset.DisplayException = false;
		this.gvFixedAsset.EditColumn = null;
		this.gvFixedAsset.FieldCurrency = null;
		this.gvFixedAsset.FieldRate = null;
		this.gvFixedAsset.FormId = null;
		this.gvFixedAsset.GridControl = this.grdFixedAsset;
		this.gvFixedAsset.IgnoreFilter = false;
		this.gvFixedAsset.IndexEditRow = -1;
		this.gvFixedAsset.IndicatorWidth = 40;
		this.gvFixedAsset.IsApzTable = false;
		this.gvFixedAsset.IsInsertNewRow = false;
		this.gvFixedAsset.IsLoadingData = false;
		this.gvFixedAsset.IsMappingContextAction = false;
		this.gvFixedAsset.ItemId = null;
		this.gvFixedAsset.LstColumnAllowBatch = null;
		this.gvFixedAsset.LstColumnNotAllowBatch = null;
		this.gvFixedAsset.LstColumnNotEditable = null;
		this.gvFixedAsset.LstColumNotSetting = null;
		this.gvFixedAsset.Name = "gvFixedAsset";
		this.gvFixedAsset.OptionsView.ColumnAutoWidth = false;
		this.gvFixedAsset.OptionsView.ShowGroupPanel = false;
		this.gvFixedAsset.UseBatchMod = false;
		this.gvFixedAsset.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvFixedAsset_RowStyle);
		this.gvFixedAsset.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvFixedAsset_CellValueChanged);
		this.clItemCode.Caption = "Fixed Asset Code";
		this.clItemCode.ColumnEdit = this.repItemCode;
		this.clItemCode.CurrencyDateField = null;
		this.clItemCode.CurrencyField = null;
		this.clItemCode.CurrencyFixed = null;
		this.clItemCode.CurrencyFixedRate = 0.0;
		this.clItemCode.CurrencyRateField = null;
		this.clItemCode.FieldName = "ItemCode";
		this.clItemCode.Name = "clItemCode";
		this.clItemCode.OptionsColumn.ReadOnly = true;
		this.clItemCode.PrimaryKey = false;
		this.clItemCode.UnitField = null;
		this.clItemCode.Visible = true;
		this.clItemCode.VisibleIndex = 0;
		this.clItemCode.Width = 111;
		this.repItemCode.AutoHeight = false;
		this.repItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repItemCode.Name = "repItemCode";
		this.repItemCode.ReadOnly = true;
		this.repItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repItemCode_ButtonClick);
		this.clItemName.Caption = "FA Name";
		this.clItemName.CurrencyDateField = null;
		this.clItemName.CurrencyField = null;
		this.clItemName.CurrencyFixed = null;
		this.clItemName.CurrencyFixedRate = 0.0;
		this.clItemName.CurrencyRateField = null;
		this.clItemName.FieldName = "ItemName";
		this.clItemName.Name = "clItemName";
		this.clItemName.OptionsColumn.ReadOnly = true;
		this.clItemName.PrimaryKey = false;
		this.clItemName.UnitField = null;
		this.clItemName.Visible = true;
		this.clItemName.VisibleIndex = 1;
		this.clItemName.Width = 169;
		this.clStat.Caption = "Status";
		this.clStat.ColumnEdit = this.repStatus2;
		this.clStat.CurrencyDateField = null;
		this.clStat.CurrencyField = null;
		this.clStat.CurrencyFixed = null;
		this.clStat.CurrencyFixedRate = 0.0;
		this.clStat.CurrencyRateField = null;
		this.clStat.FieldName = "AsstStatus";
		this.clStat.Name = "clStat";
		this.clStat.OptionsColumn.AllowEdit = false;
		this.clStat.PrimaryKey = false;
		this.clStat.UnitField = null;
		this.clStat.Visible = true;
		this.clStat.VisibleIndex = 2;
		this.clStat.Width = 86;
		this.repStatus2.AutoHeight = false;
		this.repStatus2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repStatus2.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Status"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.repStatus2.DisplayMember = "Name";
		this.repStatus2.Name = "repStatus2";
		this.repStatus2.ValidateOnEnterKey = true;
		this.repStatus2.ValueMember = "ID";
		this.clLocation.Caption = "Location";
		this.clLocation.CurrencyDateField = null;
		this.clLocation.CurrencyField = null;
		this.clLocation.CurrencyFixed = null;
		this.clLocation.CurrencyFixedRate = 0.0;
		this.clLocation.CurrencyRateField = null;
		this.clLocation.FieldName = "Location";
		this.clLocation.Name = "clLocation";
		this.clLocation.OptionsColumn.AllowEdit = false;
		this.clLocation.OptionsColumn.ReadOnly = true;
		this.clLocation.PrimaryKey = false;
		this.clLocation.UnitField = null;
		this.clLocation.Visible = true;
		this.clLocation.VisibleIndex = 3;
		this.clLocation.Width = 117;
		this.clTechnician.Caption = "Technical";
		this.clTechnician.CurrencyDateField = null;
		this.clTechnician.CurrencyField = null;
		this.clTechnician.CurrencyFixed = null;
		this.clTechnician.CurrencyFixedRate = 0.0;
		this.clTechnician.CurrencyRateField = null;
		this.clTechnician.FieldName = "Technician";
		this.clTechnician.Name = "clTechnician";
		this.clTechnician.OptionsColumn.AllowEdit = false;
		this.clTechnician.PrimaryKey = false;
		this.clTechnician.UnitField = null;
		this.clTechnician.Visible = true;
		this.clTechnician.VisibleIndex = 4;
		this.clTechnician.Width = 111;
		this.clEmployee.Caption = "Employee";
		this.clEmployee.CurrencyDateField = null;
		this.clEmployee.CurrencyField = null;
		this.clEmployee.CurrencyFixed = null;
		this.clEmployee.CurrencyFixedRate = 0.0;
		this.clEmployee.CurrencyRateField = null;
		this.clEmployee.FieldName = "Employee";
		this.clEmployee.Name = "clEmployee";
		this.clEmployee.OptionsColumn.AllowEdit = false;
		this.clEmployee.PrimaryKey = false;
		this.clEmployee.UnitField = null;
		this.clEmployee.Visible = true;
		this.clEmployee.VisibleIndex = 5;
		this.clEmployee.Width = 123;
		this.clRemak.Caption = "Remark";
		this.clRemak.ColumnEdit = this.repositoryItemMemoEdit2;
		this.clRemak.CurrencyDateField = null;
		this.clRemak.CurrencyField = null;
		this.clRemak.CurrencyFixed = null;
		this.clRemak.CurrencyFixedRate = 0.0;
		this.clRemak.CurrencyRateField = null;
		this.clRemak.FieldName = "Remark";
		this.clRemak.Name = "clRemak";
		this.clRemak.PrimaryKey = false;
		this.clRemak.UnitField = null;
		this.clRemak.Visible = true;
		this.clRemak.VisibleIndex = 6;
		this.clRemak.Width = 159;
		this.repositoryItemMemoEdit2.Name = "repositoryItemMemoEdit2";
		this.tabCosting.Controls.Add(this.grdCosting);
		this.tabCosting.EditControl = null;
		this.tabCosting.EditValue = "Costing";
		this.tabCosting.FieldName = null;
		this.tabCosting.IsSearchControl = false;
		this.tabCosting.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.tabCosting.Name = "tabCosting";
		this.tabCosting.ReadOnly = false;
		this.tabCosting.SettingInVisible = false;
		this.tabCosting.SettingReadonly = false;
		this.tabCosting.Size = new System.Drawing.Size(681, 165);
		this.tabCosting.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.tabCosting.Text = "Costing";
		this.grdCosting.AllowPasteExcel = false;
		this.grdCosting.ClipboardData = null;
		this.grdCosting.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdCosting.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdCosting.Location = new System.Drawing.Point(0, 0);
		this.grdCosting.MainView = this.gvCosting;
		this.grdCosting.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grdCosting.Name = "grdCosting";
		this.grdCosting.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.repCheck, this.repTinh, this.repStdCost, this.repAcctCode, this.repAcctCode2 });
		this.grdCosting.Size = new System.Drawing.Size(681, 165);
		this.grdCosting.TabIndex = 1;
		this.grdCosting.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvCosting });
		this.gvCosting.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvCosting.ColumnKey = null;
		this.gvCosting.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.clStdCost, this.clDefaultCost, this.clCostExpAct, this.clWipAct, this.clSetupCost, this.clMachineCost, this.clLaborCost, this.clOtherCost });
		this.gvCosting.DisableFormSetting = false;
		this.gvCosting.DisplayException = false;
		this.gvCosting.EditColumn = null;
		this.gvCosting.FieldCurrency = null;
		this.gvCosting.FieldRate = null;
		this.gvCosting.FormId = null;
		this.gvCosting.GridControl = this.grdCosting;
		this.gvCosting.IgnoreFilter = false;
		this.gvCosting.IndexEditRow = -1;
		this.gvCosting.IndicatorWidth = 40;
		this.gvCosting.IsApzTable = false;
		this.gvCosting.IsInsertNewRow = false;
		this.gvCosting.IsLoadingData = false;
		this.gvCosting.IsMappingContextAction = false;
		this.gvCosting.ItemId = null;
		this.gvCosting.LstColumnAllowBatch = null;
		this.gvCosting.LstColumnNotAllowBatch = null;
		this.gvCosting.LstColumnNotEditable = null;
		this.gvCosting.LstColumNotSetting = null;
		this.gvCosting.Name = "gvCosting";
		this.gvCosting.OptionsView.ColumnAutoWidth = false;
		this.gvCosting.OptionsView.ShowGroupPanel = false;
		this.gvCosting.UseBatchMod = false;
		this.gvCosting.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gvCosting_RowStyle);
		this.gvCosting.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvCosting_CellValueChanged);
		this.gvCosting.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvCosting_CellValueChanging);
		this.clStdCost.Caption = "Resource Standard Cost Name";
		this.clStdCost.ColumnEdit = this.repStdCost;
		this.clStdCost.CurrencyDateField = null;
		this.clStdCost.CurrencyField = null;
		this.clStdCost.CurrencyFixed = null;
		this.clStdCost.CurrencyFixedRate = 0.0;
		this.clStdCost.CurrencyRateField = null;
		this.clStdCost.FieldName = "StdCost";
		this.clStdCost.Name = "clStdCost";
		this.clStdCost.OptionsColumn.ReadOnly = true;
		this.clStdCost.PrimaryKey = false;
		this.clStdCost.UnitField = null;
		this.clStdCost.Visible = true;
		this.clStdCost.VisibleIndex = 0;
		this.clStdCost.Width = 164;
		this.repStdCost.AutoHeight = false;
		this.repStdCost.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repStdCost.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "ID", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.repStdCost.DisplayMember = "Name";
		this.repStdCost.Name = "repStdCost";
		this.repStdCost.ValueMember = "ID";
		this.clDefaultCost.Caption = "Default Cost";
		this.clDefaultCost.CurrencyDateField = null;
		this.clDefaultCost.CurrencyField = null;
		this.clDefaultCost.CurrencyFixed = null;
		this.clDefaultCost.CurrencyFixedRate = 0.0;
		this.clDefaultCost.CurrencyRateField = null;
		this.clDefaultCost.FieldName = "DefaultCost";
		this.clDefaultCost.Name = "clDefaultCost";
		this.clDefaultCost.PrimaryKey = false;
		this.clDefaultCost.UnitField = null;
		this.clDefaultCost.Visible = true;
		this.clDefaultCost.VisibleIndex = 1;
		this.clDefaultCost.Width = 98;
		this.clCostExpAct.Caption = "Cost Expense Act.";
		this.clCostExpAct.ColumnEdit = this.repAcctCode2;
		this.clCostExpAct.CurrencyDateField = null;
		this.clCostExpAct.CurrencyField = null;
		this.clCostExpAct.CurrencyFixed = null;
		this.clCostExpAct.CurrencyFixedRate = 0.0;
		this.clCostExpAct.CurrencyRateField = null;
		this.clCostExpAct.FieldName = "CostExpAct";
		this.clCostExpAct.Name = "clCostExpAct";
		this.clCostExpAct.PrimaryKey = false;
		this.clCostExpAct.UnitField = null;
		this.clCostExpAct.Visible = true;
		this.clCostExpAct.VisibleIndex = 2;
		this.clCostExpAct.Width = 117;
		this.repAcctCode2.AutoHeight = false;
		this.repAcctCode2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repAcctCode2.Name = "repAcctCode2";
		this.repAcctCode2.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repAcctCode2_ButtonClick);
		this.clWipAct.Caption = "Resource WIP Act.";
		this.clWipAct.ColumnEdit = this.repAcctCode;
		this.clWipAct.CurrencyDateField = null;
		this.clWipAct.CurrencyField = null;
		this.clWipAct.CurrencyFixed = null;
		this.clWipAct.CurrencyFixedRate = 0.0;
		this.clWipAct.CurrencyRateField = null;
		this.clWipAct.FieldName = "WipAct";
		this.clWipAct.Name = "clWipAct";
		this.clWipAct.PrimaryKey = false;
		this.clWipAct.UnitField = null;
		this.clWipAct.Visible = true;
		this.clWipAct.VisibleIndex = 3;
		this.clWipAct.Width = 116;
		this.repAcctCode.AutoHeight = false;
		this.repAcctCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.repAcctCode.Name = "repAcctCode";
		this.repAcctCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.repAcctCode_ButtonClick);
		this.clSetupCost.Caption = "Setup Cost";
		this.clSetupCost.ColumnEdit = this.repCheck;
		this.clSetupCost.CurrencyDateField = null;
		this.clSetupCost.CurrencyField = null;
		this.clSetupCost.CurrencyFixed = null;
		this.clSetupCost.CurrencyFixedRate = 0.0;
		this.clSetupCost.CurrencyRateField = null;
		this.clSetupCost.FieldName = "SetupCost";
		this.clSetupCost.Name = "clSetupCost";
		this.clSetupCost.PrimaryKey = false;
		this.clSetupCost.UnitField = null;
		this.clSetupCost.Visible = true;
		this.clSetupCost.VisibleIndex = 4;
		this.repCheck.AutoHeight = false;
		this.repCheck.Name = "repCheck";
		this.repCheck.ValueChecked = "Y";
		this.repCheck.ValueUnchecked = "N";
		this.clMachineCost.Caption = "Machine Cost";
		this.clMachineCost.ColumnEdit = this.repCheck;
		this.clMachineCost.CurrencyDateField = null;
		this.clMachineCost.CurrencyField = null;
		this.clMachineCost.CurrencyFixed = null;
		this.clMachineCost.CurrencyFixedRate = 0.0;
		this.clMachineCost.CurrencyRateField = null;
		this.clMachineCost.FieldName = "MachineCost";
		this.clMachineCost.Name = "clMachineCost";
		this.clMachineCost.PrimaryKey = false;
		this.clMachineCost.UnitField = null;
		this.clMachineCost.Visible = true;
		this.clMachineCost.VisibleIndex = 5;
		this.clMachineCost.Width = 88;
		this.clLaborCost.Caption = "Labor Cost";
		this.clLaborCost.ColumnEdit = this.repCheck;
		this.clLaborCost.CurrencyDateField = null;
		this.clLaborCost.CurrencyField = null;
		this.clLaborCost.CurrencyFixed = null;
		this.clLaborCost.CurrencyFixedRate = 0.0;
		this.clLaborCost.CurrencyRateField = null;
		this.clLaborCost.FieldName = "LaborCost";
		this.clLaborCost.Name = "clLaborCost";
		this.clLaborCost.PrimaryKey = false;
		this.clLaborCost.UnitField = null;
		this.clLaborCost.Visible = true;
		this.clLaborCost.VisibleIndex = 6;
		this.clOtherCost.Caption = "Other Cost";
		this.clOtherCost.ColumnEdit = this.repCheck;
		this.clOtherCost.CurrencyDateField = null;
		this.clOtherCost.CurrencyField = null;
		this.clOtherCost.CurrencyFixed = null;
		this.clOtherCost.CurrencyFixedRate = 0.0;
		this.clOtherCost.CurrencyRateField = null;
		this.clOtherCost.FieldName = "OtherCost";
		this.clOtherCost.Name = "clOtherCost";
		this.clOtherCost.PrimaryKey = false;
		this.clOtherCost.UnitField = null;
		this.clOtherCost.Visible = true;
		this.clOtherCost.VisibleIndex = 7;
		this.repTinh.AutoHeight = false;
		this.repTinh.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repTinh.Name = "repTinh";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(687, 368);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.panel1);
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_Resources";
		base.ShowInTaskbar = false;
		this.Text = "Resources";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Resources_FormClosing);
		base.Load += new System.EventHandler(this.frm_Resources_Load);
		((System.ComponentModel.ISupportInitialize)this.gv_CapacityChildren).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repShowDay).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repUOM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repQuantity).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_CapacityMain).EndInit();
		this.contextMenuStrip_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_CapacityMain).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repNumber).EndInit();
		this.panel1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gpGeneral).EndInit();
		this.gpGeneral.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdGeneral).EndInit();
		this.contextMenuStrip_Docs.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvGeneral).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repOcc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Ocr2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Ocr3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Ocr4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Ocr5).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repControlKey).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoExEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.tabCapacity.ResumeLayout(false);
		this.tabFixedAsset.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdFixedAsset).EndInit();
		this.contextMenuStripFixasset.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvFixedAsset).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repStatus2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemMemoEdit2).EndInit();
		this.tabCosting.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdCosting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvCosting).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repStdCost).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repAcctCode2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repAcctCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repCheck).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repTinh).EndInit();
		base.ResumeLayout(false);
	}
}
