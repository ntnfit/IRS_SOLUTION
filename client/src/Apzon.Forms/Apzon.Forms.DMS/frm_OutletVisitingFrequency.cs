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
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.MyLookUpEdit;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Apzon.Forms.DMS;

public class frm_OutletVisitingFrequency : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private GridViewEx gv_Doc = new GridViewEx();

	private string[] lstDayOfWeek = new string[7] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };

	private DataTable OutletsData = new DataTable();

	private DataTable DtFrequency = new DataTable();

	private IContainer components;

	private GridControlEx grd_Doc;

	private AdvBandedGridViewEx adv_Doc;

	private PanelControl panelControl1;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Refresh;

	private SplitContainerControl dock;

	private GridControlEx grd_Routes;

	private GridViewEx gv_Routes;

	private GridColumnEx gc_UserSign;

	private GridColumnEx gc_RouteCode;

	private GridColumnEx gc_RouteName;

	private GridColumnEx gc_CountryCode;

	private RepositoryItemButtonEdit rep_CountryCode;

	private GridColumnEx gc_DocStatus;

	private RepositoryItemLookUpEdit rep_DocStatus;

	private GridColumnEx gc_RouteRmarks;

	private GridColumnEx gc_ChannelCode;

	private RepositoryItemButtonEdit rep_Channel;

	private GridColumnEx gc_CreateDate;

	private GridColumnEx gc_CreateTime;

	private GridColumnEx gc_UpdateDate;

	private GridColumnEx gc_UpdateTime;

	private SplitContainerControl split_Employee;

	private GroupControl groupControl1;

	private TreeEx tv_Details;

	private TreeListColumnEx tc_Id;

	private TreeListColumnEx tc_RouteCode;

	private TreeListColumnEx tc_Type;

	private RepositoryItemLookUpEdit rep_Type;

	private TreeListColumnEx tc_Code;

	private RepositoryItemButtonEdit rep_ChooseType;

	private TreeListColumnEx tc_Name;

	private TreeListColumnEx tc_CountryCode;

	private TreeListColumnEx tc_Status1;

	private RepositoryItemLookUpEdit rep_Status1;

	private TreeListColumnEx tc_Remarks1;

	private TreeListColumnEx tc_SuperVisor;

	private RepositoryItemButtonEditCFL rep_Employee;

	private TreeListColumnEx treeListColumnEx8;

	private RepositoryItemButtonEditCFL rep_SalesAdmin;

	private TreeListColumnEx treeListColumnEx12;

	private TreeListColumnEx tc_LevelGgp;

	private TreeListColumnEx tc_BaseId;

	private RepositoryItemButtonEdit rep_Geography;

	private RepositoryItemButtonEdit btn_AddTree;

	private RepositoryItemButtonEdit rep_BP;

	private RepositoryItemButtonEdit rep_BPCode;

	private GridColumnEx gc_Recurring;

	private GridColumnEx gc_RecurNum;

	private RepositoryItemLookUpEditSimple luk_reccurringDOc;

	private GridColumnEx gv_NumberOfVisits;

	protected override void SetFormMode(Enums.FormViewModeType value)
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
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_OutletVisitingFrequency()
	{
		InitializeComponent();
		OnLoadAsync += frm_OutletVisitingFrequency_Load;
	}

	private async Task frm_OutletVisitingFrequency_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			WinCommons._OpenProcessing();
			InitGridViewEx();
			DataTable dt = await _httpClient.OutletVisitingFrequencyClient.GetData(GlobalData.UserName, GlobalData.DefaultBranch);
			if (!dt.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.None, "Data is Empty!");
				return;
			}
			DtFrequency = CreateDataSource(dt);
			await InitData();
			await LoadData("", "", "");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task InitData()
	{
		_ = 1;
		try
		{
			Dictionary<string, string> dic = new Dictionary<string, string>
			{
				{ "Y", "Hoạt động" },
				{ "N", "Không hoạt động" }
			};
			Dictionary<string, string> dic2 = new Dictionary<string, string>
			{
				{ "T", "Vùng" },
				{ "G", "Vị trí địa lý" }
			};
			rep_DocStatus.SetDataSource(dic);
			rep_Status1.SetDataSource(dic);
			luk_reccurringDOc.SetDataSource(new Dictionary<string, string>
			{
				{ "", "_" },
				{ "W", "Hàng tuần" },
				{ "M", "Hàng tháng" }
			});
			rep_Type.SetDataSource(dic2);
			rep_Employee.TypeCFL = Enums.TypeCFL.Employees;
			rep_Employee.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			rep_Employee.ValueMember = "empID";
			rep_Employee.DisplayMember = "fullName";
			await rep_Employee.RefreshDataSourceAsync();
			rep_SalesAdmin.ValueMember = "empID";
			rep_SalesAdmin.DisplayMember = "fullName";
			rep_SalesAdmin.DisplayMember2 = "";
			rep_SalesAdmin.TypeCFL = Enums.TypeCFL.Employees;
			rep_SalesAdmin.TypeDrillDown = ApzObjectTypes.oNone;
			rep_SalesAdmin.MultiSelectMode = true;
			await rep_SalesAdmin.RefreshDataSourceAsync();
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_D_OSTR");
			DataTable tableStructure2 = WinFuntions.GetTableStructure("APZ_D_STR1");
			DataTable tableStructure3 = WinFuntions.GetTableStructure("APZ_D_STR2");
			DataTable tableStructure4 = WinFuntions.GetTableStructure("APZ_D_STR3");
			if (tableStructure.Columns.Count == 0 || tableStructure2.Columns.Count == 0 || tableStructure3.Columns.Count == 0 || tableStructure4.Columns.Count == 0)
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Cancel.Enabled = true;
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private async Task LoadData(string routeCode, string routeName, string status)
	{
		try
		{
			DataTable dataSource = await _httpClient.SalesRouteClient.GetDataDoc(routeCode, routeName, status);
			grd_Routes.DataSource = dataSource;
			grd_Routes.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void LoadDetails(string routeCode)
	{
		try
		{
			DataTable outlets = _httpClient.SalesRouteClient.GetOutlets(routeCode);
			OutletsData = outlets;
			tv_Details.DataSource = _httpClient.SalesRouteClient.GetDataDetails(routeCode);
			tv_Details.RefreshDataSource();
			tv_Details.ExpandAll();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
	}

	private void tv_Details_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			if (tv_Details.FocusedNode != null)
			{
				int num = Function.ParseInt(tv_Details.FocusedNode.GetValue("Id"));
				DataRow[] lstDataRowOutlet = OutletsData.Select($"Id = {num}");
				if (DtFrequency.IsNotNull() && DtFrequency.ExistsColumn("LevelDL"))
				{
					for (int i = 0; i < gv_Doc.Columns.Count; i++)
					{
						if (gv_Doc.Columns[i].Name == "cl_LevelDL")
						{
							gv_Doc.Columns[i].Visible = true;
							break;
						}
					}
				}
				if (lstDataRowOutlet.Length != 0)
				{
					EnumerableRowCollection<DataRow> enumerableRowCollection = from dataRow in DtFrequency.AsEnumerable()
						where lstDataRowOutlet.CopyToDataTable().AsEnumerable().Any((DataRow a) => Function.ToString(dataRow["Code"]) == Function.ToString(a["CardCode"]))
						select dataRow;
					if (enumerableRowCollection != null && enumerableRowCollection.Count() > 0)
					{
						grd_Doc.DataSource = enumerableRowCollection.CopyToDataTable();
						grd_Doc.RefreshDataSource();
					}
					else
					{
						grd_Doc.DataSource = DtFrequency.Clone();
						grd_Doc.RefreshDataSource();
					}
				}
				else
				{
					grd_Doc.DataSource = DtFrequency.Clone();
					grd_Doc.RefreshDataSource();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, "");
		}
		WinCommons._CloseProcessing();
	}

	private void gv_Routes_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					return;
				}
				FormMode = Enums.FormViewModeType.View;
			}
			LoadDetails(Function.ToString(gv_Routes.GetRowCellValue(gv_Routes.FocusedRowHandle, gc_RouteCode.FieldName)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void InitBandedGrid()
	{
		adv_Doc.Bands.Clear();
		GridBand gridBand = new GridBand();
		gridBand.Name = "ban_Info";
		gridBand.Caption = "Thông tin";
		gridBand.VisibleIndex = 0;
		gridBand.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		adv_Doc.Bands.Add(gridBand);
		GridBand gridBand2 = new GridBand();
		gridBand2.Name = "ban_Days";
		gridBand2.Caption = "Ngày";
		gridBand2.VisibleIndex = 1;
		adv_Doc.Bands.Add(gridBand2);
		GridBand gridBand3 = new GridBand();
		gridBand3.Name = "ban_Recur";
		gridBand3.Caption = "Tần suất";
		gridBand3.VisibleIndex = 2;
		adv_Doc.Bands.Add(gridBand3);
		adv_Doc.Columns.Clear();
		BandedGridColumn bandedGridColumn = new BandedGridColumn();
		bandedGridColumn.Caption = "Mã cửa hàng";
		bandedGridColumn.FieldName = "Code";
		bandedGridColumn.Name = "cl_Code";
		bandedGridColumn.OptionsColumn.ReadOnly = true;
		bandedGridColumn.Visible = true;
		bandedGridColumn.Width = 75;
		bandedGridColumn.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		adv_Doc.Columns.Add(bandedGridColumn);
		gridBand.Columns.Add(bandedGridColumn);
		BandedGridColumn bandedGridColumn2 = new BandedGridColumn();
		bandedGridColumn2.Caption = "Tên cửa hàng";
		bandedGridColumn2.FieldName = "Name";
		bandedGridColumn2.Name = "cl_Name";
		bandedGridColumn2.OptionsColumn.ReadOnly = true;
		bandedGridColumn2.Visible = true;
		bandedGridColumn2.Width = 115;
		bandedGridColumn2.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		adv_Doc.Columns.Add(bandedGridColumn2);
		gridBand.Columns.Add(bandedGridColumn2);
		BandedGridColumn bandedGridColumn3 = new BandedGridColumn();
		bandedGridColumn3.Caption = "Địa chỉ";
		bandedGridColumn3.FieldName = "Address";
		bandedGridColumn3.Name = "cl_Address";
		bandedGridColumn3.OptionsColumn.ReadOnly = true;
		bandedGridColumn3.Visible = true;
		bandedGridColumn3.Width = 115;
		bandedGridColumn3.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		adv_Doc.Columns.Add(bandedGridColumn3);
		gridBand.Columns.Add(bandedGridColumn3);
		BandedGridColumn bandedGridColumn4 = new BandedGridColumn();
		bandedGridColumn4.Caption = "Description";
		bandedGridColumn4.FieldName = "Description";
		bandedGridColumn4.Name = "cl_Description";
		bandedGridColumn4.OptionsColumn.ReadOnly = true;
		bandedGridColumn4.Width = 117;
		bandedGridColumn4.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		adv_Doc.Columns.Add(bandedGridColumn4);
		gridBand.Columns.Add(bandedGridColumn4);
		RepositoryItemCheckEdit repositoryItemCheckEdit = new RepositoryItemCheckEdit();
		repositoryItemCheckEdit.AutoHeight = false;
		repositoryItemCheckEdit.Name = "repositoryItemCheckEdit1";
		repositoryItemCheckEdit.ValueChecked = "Y";
		repositoryItemCheckEdit.ValueUnchecked = "N";
		BandedGridColumn bandedGridColumn5 = new BandedGridColumn();
		bandedGridColumn5.Caption = "Mỗi ngày";
		bandedGridColumn5.ColumnEdit = repositoryItemCheckEdit;
		bandedGridColumn5.FieldName = "EveryDay";
		bandedGridColumn5.Name = "cl_EveryDay";
		bandedGridColumn5.Visible = true;
		bandedGridColumn5.Width = 106;
		adv_Doc.Columns.Add(bandedGridColumn5);
		gridBand2.Columns.Add(bandedGridColumn5);
		BandedGridColumn bandedGridColumn6 = new BandedGridColumn();
		bandedGridColumn6.Caption = "Các thứ trong tuần";
		bandedGridColumn6.ColumnEdit = repositoryItemCheckEdit;
		bandedGridColumn6.FieldName = "DayOfWeek";
		bandedGridColumn6.Name = "cl_DayOfWeek";
		bandedGridColumn6.Visible = true;
		bandedGridColumn6.Width = 109;
		adv_Doc.Columns.Add(bandedGridColumn6);
		gridBand2.Columns.Add(bandedGridColumn6);
		BandedGridColumn bandedGridColumn7 = new BandedGridColumn();
		bandedGridColumn7.Caption = "Hai";
		bandedGridColumn7.ColumnEdit = repositoryItemCheckEdit;
		bandedGridColumn7.FieldName = "Mon";
		bandedGridColumn7.Name = "cl_Mon";
		bandedGridColumn7.Visible = true;
		bandedGridColumn7.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn7);
		gridBand2.Columns.Add(bandedGridColumn7);
		BandedGridColumn bandedGridColumn8 = new BandedGridColumn();
		bandedGridColumn8.Caption = "Ba";
		bandedGridColumn8.ColumnEdit = repositoryItemCheckEdit;
		bandedGridColumn8.FieldName = "Tue";
		bandedGridColumn8.Name = "cl_Tue";
		bandedGridColumn8.Visible = true;
		bandedGridColumn8.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn8);
		gridBand2.Columns.Add(bandedGridColumn8);
		BandedGridColumn bandedGridColumn9 = new BandedGridColumn();
		bandedGridColumn9.Caption = "Tư";
		bandedGridColumn9.ColumnEdit = repositoryItemCheckEdit;
		bandedGridColumn9.FieldName = "Wed";
		bandedGridColumn9.Name = "cl_Wed";
		bandedGridColumn9.Visible = true;
		bandedGridColumn9.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn9);
		gridBand2.Columns.Add(bandedGridColumn9);
		BandedGridColumn bandedGridColumn10 = new BandedGridColumn();
		bandedGridColumn10.Caption = "Năm";
		bandedGridColumn10.ColumnEdit = repositoryItemCheckEdit;
		bandedGridColumn10.FieldName = "Thu";
		bandedGridColumn10.Name = "cl_Thu";
		bandedGridColumn10.Visible = true;
		bandedGridColumn10.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn10);
		gridBand2.Columns.Add(bandedGridColumn10);
		BandedGridColumn bandedGridColumn11 = new BandedGridColumn();
		bandedGridColumn11.Caption = "Sáu";
		bandedGridColumn11.ColumnEdit = repositoryItemCheckEdit;
		bandedGridColumn11.FieldName = "Fri";
		bandedGridColumn11.Name = "cl_Fri";
		bandedGridColumn11.Visible = true;
		bandedGridColumn11.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn11);
		gridBand2.Columns.Add(bandedGridColumn11);
		BandedGridColumn bandedGridColumn12 = new BandedGridColumn();
		bandedGridColumn12.Caption = "Bảy";
		bandedGridColumn12.ColumnEdit = repositoryItemCheckEdit;
		bandedGridColumn12.FieldName = "Sat";
		bandedGridColumn12.Name = "cl_Sat";
		bandedGridColumn12.Visible = true;
		bandedGridColumn12.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn12);
		gridBand2.Columns.Add(bandedGridColumn12);
		BandedGridColumn bandedGridColumn13 = new BandedGridColumn();
		bandedGridColumn13.Caption = "CN";
		bandedGridColumn13.ColumnEdit = repositoryItemCheckEdit;
		bandedGridColumn13.FieldName = "Sun";
		bandedGridColumn13.Name = "cl_Sun";
		bandedGridColumn13.Visible = true;
		bandedGridColumn13.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn13);
		gridBand2.Columns.Add(bandedGridColumn13);
		BandedGridColumn bandedGridColumn14 = new BandedGridColumn();
		bandedGridColumn14.Caption = "Các ngày trong tháng";
		bandedGridColumn14.ColumnEdit = repositoryItemCheckEdit;
		bandedGridColumn14.FieldName = "DayOfMonth";
		bandedGridColumn14.Name = "cl_DayOfMonth";
		bandedGridColumn14.Visible = true;
		bandedGridColumn14.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn14);
		gridBand2.Columns.Add(bandedGridColumn14);
		for (int i = 1; i <= 31; i++)
		{
			BandedGridColumn bandedGridColumn15 = new BandedGridColumn();
			bandedGridColumn15.Caption = i.ToString();
			bandedGridColumn15.ColumnEdit = repositoryItemCheckEdit;
			bandedGridColumn15.FieldName = i.ToString();
			bandedGridColumn15.Name = "cl_" + i;
			bandedGridColumn15.Visible = true;
			bandedGridColumn15.Width = 45;
			adv_Doc.Columns.Add(bandedGridColumn15);
			gridBand2.Columns.Add(bandedGridColumn15);
		}
		BandedGridColumn bandedGridColumn16 = new BandedGridColumn();
		bandedGridColumn16.Caption = "Thời gian bắt đầu";
		bandedGridColumn16.FieldName = "StartDate";
		bandedGridColumn16.Name = "cl_StartDate";
		bandedGridColumn16.DisplayFormat.FormatType = FormatType.DateTime;
		bandedGridColumn16.Visible = true;
		bandedGridColumn16.Width = 45;
		RepositoryItemDateEditEx repositoryItemDateEditEx = new RepositoryItemDateEditEx();
		repositoryItemDateEditEx.Name = bandedGridColumn16.Name;
		repositoryItemDateEditEx.AllowMouseWheel = false;
		bandedGridColumn16.ColumnEdit = repositoryItemDateEditEx;
		adv_Doc.Columns.Add(bandedGridColumn16);
		gridBand3.Columns.Add(bandedGridColumn16);
		BandedGridColumn bandedGridColumn17 = new BandedGridColumn();
		bandedGridColumn17.Caption = "Đã kết thúc";
		bandedGridColumn17.ColumnEdit = repositoryItemCheckEdit;
		bandedGridColumn17.FieldName = "Finished";
		bandedGridColumn17.Name = "cl_Finished";
		bandedGridColumn17.Visible = true;
		bandedGridColumn17.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn17);
		gridBand3.Columns.Add(bandedGridColumn17);
		BandedGridColumn bandedGridColumn18 = new BandedGridColumn();
		bandedGridColumn18.Caption = "Loại";
		bandedGridColumn18.FieldName = "Recurring";
		bandedGridColumn18.Name = "cl_Recurring";
		bandedGridColumn18.Visible = true;
		bandedGridColumn18.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn18);
		gridBand3.Columns.Add(bandedGridColumn18);
		BandedGridColumn bandedGridColumn19 = new BandedGridColumn();
		bandedGridColumn19.Caption = "Số ngày/tuần/tháng";
		bandedGridColumn19.FieldName = "RecurNum";
		bandedGridColumn19.Name = "cl_RecurNum";
		bandedGridColumn19.Visible = true;
		bandedGridColumn19.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn19);
		gridBand3.Columns.Add(bandedGridColumn19);
		BandedGridColumn bandedGridColumn20 = new BandedGridColumn();
		bandedGridColumn20.Caption = "Mô tả";
		bandedGridColumn20.FieldName = "RecurNote";
		bandedGridColumn20.Name = "cl_RecurNote";
		bandedGridColumn20.Visible = true;
		bandedGridColumn20.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn20);
		gridBand3.Columns.Add(bandedGridColumn20);
		new BandedGridColumn();
		bandedGridColumn19.Caption = "Số lần ghé thăm hàng tuần/tháng";
		bandedGridColumn19.FieldName = "NumberOfVisits";
		bandedGridColumn19.Name = "cl_NumberOfVisits";
		bandedGridColumn19.Visible = true;
		bandedGridColumn19.Width = 45;
		adv_Doc.Columns.Add(bandedGridColumn19);
		gridBand3.Columns.Add(bandedGridColumn19);
	}

	private void InitGridViewEx()
	{
		adv_Doc.Bands.Clear();
		gv_Doc = new GridViewEx();
		gv_Doc.Columns.Clear();
		gv_Doc.OptionsView.ColumnHeaderAutoHeight = DefaultBoolean.True;
		gv_Doc.CellValueChanged += gv_Doc_CellValueChanged;
		gv_Doc.ShowingEditor += gv_Doc_ShowingEditor;
		grd_Doc.MainView = gv_Doc;
		GridColumnEx gridColumnEx = new GridColumnEx();
		gridColumnEx.Caption = "Mã cửa hàng";
		gridColumnEx.FieldName = "Code";
		gridColumnEx.Name = "cl_Code";
		gridColumnEx.OptionsColumn.ReadOnly = true;
		gridColumnEx.Visible = true;
		gridColumnEx.VisibleIndex = 0;
		gridColumnEx.Width = 75;
		gridColumnEx.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		gv_Doc.Columns.Add(gridColumnEx);
		GridColumnEx gridColumnEx2 = new GridColumnEx();
		gridColumnEx2.Caption = "Tên cửa hàng";
		gridColumnEx2.FieldName = "Name";
		gridColumnEx2.Name = "cl_Name";
		gridColumnEx2.OptionsColumn.ReadOnly = true;
		gridColumnEx2.Visible = true;
		gridColumnEx2.VisibleIndex = 1;
		gridColumnEx2.Width = 115;
		gridColumnEx2.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		gv_Doc.Columns.Add(gridColumnEx2);
		GridColumnEx gridColumnEx3 = new GridColumnEx();
		gridColumnEx3.Caption = "Cấp đại lý";
		gridColumnEx3.FieldName = "LevelDL";
		gridColumnEx3.Name = "cl_LevelDL";
		gridColumnEx3.OptionsColumn.ReadOnly = true;
		gridColumnEx3.Visible = false;
		gridColumnEx3.VisibleIndex = 2;
		gridColumnEx3.Width = 75;
		gridColumnEx3.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		gv_Doc.Columns.Add(gridColumnEx3);
		GridColumnEx gridColumnEx4 = new GridColumnEx();
		gridColumnEx4.Caption = "Địa chỉ";
		gridColumnEx4.FieldName = "Address";
		gridColumnEx4.Name = "cl_Address";
		gridColumnEx4.OptionsColumn.ReadOnly = true;
		gridColumnEx4.Visible = true;
		gridColumnEx4.VisibleIndex = 3;
		gridColumnEx4.Width = 200;
		gridColumnEx4.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		gv_Doc.Columns.Add(gridColumnEx4);
		GridColumnEx gridColumnEx5 = new GridColumnEx();
		gridColumnEx5.Caption = "Description";
		gridColumnEx5.FieldName = "Description";
		gridColumnEx5.Name = "cl_Description";
		gridColumnEx5.OptionsColumn.ReadOnly = true;
		gridColumnEx5.Visible = false;
		gridColumnEx5.Width = 117;
		gridColumnEx5.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		gv_Doc.Columns.Add(gridColumnEx5);
		RepositoryItemCheckEdit repositoryItemCheckEdit = new RepositoryItemCheckEdit();
		repositoryItemCheckEdit.AutoHeight = false;
		repositoryItemCheckEdit.Name = "rep_Chk";
		repositoryItemCheckEdit.ValueChecked = "Y";
		repositoryItemCheckEdit.ValueUnchecked = "N";
		GridColumnEx gridColumnEx6 = new GridColumnEx();
		gridColumnEx6.Caption = "Mỗi ngày";
		gridColumnEx6.ColumnEdit = repositoryItemCheckEdit;
		gridColumnEx6.FieldName = "EveryDay";
		gridColumnEx6.Name = "cl_EveryDay";
		gridColumnEx6.Visible = true;
		gridColumnEx6.Width = 75;
		gv_Doc.Columns.Add(gridColumnEx6);
		GridColumnEx gridColumnEx7 = new GridColumnEx();
		gridColumnEx7.Caption = "Các thứ trong tuần";
		gridColumnEx7.ColumnEdit = repositoryItemCheckEdit;
		gridColumnEx7.FieldName = "DayOfWeek";
		gridColumnEx7.Name = "cl_DayOfWeek";
		gridColumnEx7.Visible = true;
		gridColumnEx7.Width = 75;
		gv_Doc.Columns.Add(gridColumnEx7);
		GridColumnEx gridColumnEx8 = new GridColumnEx();
		gridColumnEx8.Caption = "Hai";
		gridColumnEx8.ColumnEdit = repositoryItemCheckEdit;
		gridColumnEx8.FieldName = "Mon";
		gridColumnEx8.Name = "cl_Mon";
		gridColumnEx8.Visible = true;
		gridColumnEx8.Width = 45;
		gv_Doc.Columns.Add(gridColumnEx8);
		GridColumnEx gridColumnEx9 = new GridColumnEx();
		gridColumnEx9.Caption = "Ba";
		gridColumnEx9.ColumnEdit = repositoryItemCheckEdit;
		gridColumnEx9.FieldName = "Tue";
		gridColumnEx9.Name = "cl_Tue";
		gridColumnEx9.Visible = true;
		gridColumnEx9.Width = 45;
		gv_Doc.Columns.Add(gridColumnEx9);
		GridColumnEx gridColumnEx10 = new GridColumnEx();
		gridColumnEx10.Caption = "Tư";
		gridColumnEx10.ColumnEdit = repositoryItemCheckEdit;
		gridColumnEx10.FieldName = "Wed";
		gridColumnEx10.Name = "cl_Wed";
		gridColumnEx10.Visible = true;
		gridColumnEx10.Width = 45;
		gv_Doc.Columns.Add(gridColumnEx10);
		GridColumnEx gridColumnEx11 = new GridColumnEx();
		gridColumnEx11.Caption = "Năm";
		gridColumnEx11.ColumnEdit = repositoryItemCheckEdit;
		gridColumnEx11.FieldName = "Thu";
		gridColumnEx11.Name = "cl_Thu";
		gridColumnEx11.Visible = true;
		gridColumnEx11.Width = 45;
		gv_Doc.Columns.Add(gridColumnEx11);
		GridColumnEx gridColumnEx12 = new GridColumnEx();
		gridColumnEx12.Caption = "Sáu";
		gridColumnEx12.ColumnEdit = repositoryItemCheckEdit;
		gridColumnEx12.FieldName = "Fri";
		gridColumnEx12.Name = "cl_Fri";
		gridColumnEx12.Visible = true;
		gridColumnEx12.Width = 45;
		gv_Doc.Columns.Add(gridColumnEx12);
		GridColumnEx gridColumnEx13 = new GridColumnEx();
		gridColumnEx13.Caption = "Bảy";
		gridColumnEx13.ColumnEdit = repositoryItemCheckEdit;
		gridColumnEx13.FieldName = "Sat";
		gridColumnEx13.Name = "cl_Sat";
		gridColumnEx13.Visible = true;
		gridColumnEx13.Width = 45;
		gv_Doc.Columns.Add(gridColumnEx13);
		GridColumnEx gridColumnEx14 = new GridColumnEx();
		gridColumnEx14.Caption = "CN";
		gridColumnEx14.ColumnEdit = repositoryItemCheckEdit;
		gridColumnEx14.FieldName = "Sun";
		gridColumnEx14.Name = "cl_Sun";
		gridColumnEx14.Visible = true;
		gridColumnEx14.Width = 45;
		gv_Doc.Columns.Add(gridColumnEx14);
		GridColumnEx gridColumnEx15 = new GridColumnEx();
		gridColumnEx15.Caption = "Các ngày trong tháng";
		gridColumnEx15.ColumnEdit = repositoryItemCheckEdit;
		gridColumnEx15.FieldName = "DayOfMonth";
		gridColumnEx15.Name = "cl_DayOfMonth";
		gridColumnEx15.Visible = true;
		gridColumnEx15.Width = 75;
		gv_Doc.Columns.Add(gridColumnEx15);
		for (int i = 1; i <= 31; i++)
		{
			GridColumnEx gridColumnEx16 = new GridColumnEx();
			gridColumnEx16.Caption = i.ToString();
			gridColumnEx16.ColumnEdit = repositoryItemCheckEdit;
			gridColumnEx16.FieldName = i.ToString();
			gridColumnEx16.Name = "cl_" + i;
			gridColumnEx16.Visible = true;
			gridColumnEx16.Width = 45;
			gv_Doc.Columns.Add(gridColumnEx16);
		}
		GridColumnEx gridColumnEx17 = new GridColumnEx();
		gridColumnEx17.Caption = "Thời gian bắt đầu";
		gridColumnEx17.FieldName = "StartDate";
		gridColumnEx17.Name = "cl_StartDate";
		gridColumnEx17.DisplayFormat.FormatType = FormatType.DateTime;
		gridColumnEx17.Visible = true;
		gridColumnEx17.Width = 75;
		RepositoryItemDateEditEx repositoryItemDateEditEx = new RepositoryItemDateEditEx();
		repositoryItemDateEditEx.Name = gridColumnEx17.Name;
		repositoryItemDateEditEx.AllowMouseWheel = false;
		gridColumnEx17.ColumnEdit = repositoryItemDateEditEx;
		gv_Doc.Columns.Add(gridColumnEx17);
		GridColumnEx gridColumnEx18 = new GridColumnEx();
		gridColumnEx18.Caption = "Đã kết thúc";
		gridColumnEx18.ColumnEdit = repositoryItemCheckEdit;
		gridColumnEx18.FieldName = "Finished";
		gridColumnEx18.Name = "cl_Finished";
		gridColumnEx18.Visible = true;
		gridColumnEx18.Width = 75;
		gv_Doc.Columns.Add(gridColumnEx18);
		List<Common.ListItems> list = new List<Common.ListItems>();
		list.Add(new Common.ListItems("Hàng tuần", "W"));
		list.Add(new Common.ListItems("Hàng tháng", "M"));
		RepositoryItemLookUpEdit columnEdit = new RepositoryItemLookUpEdit
		{
			DataSource = list,
			ValueMember = "Value",
			DisplayMember = "Text",
			NullText = "",
			BestFitMode = BestFitMode.BestFitResizePopup
		};
		GridColumnEx gridColumnEx19 = new GridColumnEx();
		gridColumnEx19.Caption = "Loại";
		gridColumnEx19.ColumnEdit = columnEdit;
		gridColumnEx19.FieldName = "Recurring";
		gridColumnEx19.Name = "cl_Recurring";
		gridColumnEx19.Visible = true;
		gridColumnEx19.Width = 75;
		gv_Doc.Columns.Add(gridColumnEx19);
		GridColumnEx gridColumnEx20 = new GridColumnEx();
		gridColumnEx20.Caption = "Số ngày/tuần/tháng";
		gridColumnEx20.FieldName = "RecurNum";
		gridColumnEx20.Name = "cl_RecurNum";
		gridColumnEx20.Visible = true;
		gridColumnEx20.Width = 75;
		gv_Doc.Columns.Add(gridColumnEx20);
		GridColumnEx gridColumnEx21 = new GridColumnEx();
		gridColumnEx21.Caption = "Mô tả";
		gridColumnEx21.FieldName = "RecurNote";
		gridColumnEx21.Name = "cl_RecurNote";
		gridColumnEx21.Visible = true;
		gridColumnEx21.Width = 100;
		gv_Doc.Columns.Add(gridColumnEx21);
		new GridColumnEx();
		gridColumnEx20.Caption = "Số lần ghé thăm hàng tuần/tháng";
		gridColumnEx20.FieldName = "NumberOfVisits";
		gridColumnEx20.Name = "cl_NumberOfVisits";
		gridColumnEx20.Visible = true;
		gridColumnEx20.Width = 75;
		gv_Doc.Columns.Add(gridColumnEx20);
	}

	private DataTable CreateDataSource(DataTable dt)
	{
		for (int i = 1; i <= 31; i++)
		{
			DataColumn dataColumn = new DataColumn(i.ToString(), typeof(string));
			dataColumn.DefaultValue = "N";
			dt.Columns.Add(dataColumn);
		}
		foreach (DataRow row in dt.Rows)
		{
			List<string> list = Function.ToString(row["ListOfDate"]).Split(',').ToList();
			if (list.Count <= 0 || string.IsNullOrEmpty(list[0]))
			{
				continue;
			}
			foreach (string item in list)
			{
				row[item] = "Y";
			}
		}
		return dt;
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			DataTable dataTable = grd_Doc.DataSourceTable.Copy();
			if (!dataTable.IsNotNull())
			{
				return;
			}
			dataTable.Columns.Add("UserSign", typeof(int));
			dataTable.Columns.Add("UpdateDate", typeof(DateTime));
			dataTable.Columns.Add("LogInstanc", typeof(int));
			WinCommons._OpenProcessing();
			foreach (DataRow row in dataTable.Rows)
			{
				List<string> list = new List<string>();
				for (int i = 1; i <= 31; i++)
				{
					if ("Y".Equals(row[i.ToString()].ToString()))
					{
						list.Add(i.ToString());
					}
				}
				if (list.Count > 0)
				{
					row["ListOfDate"] = string.Join(",", list);
				}
				row["UserSign"] = GlobalData.UserID;
				row["UpdateDate"] = DateTime.Today;
			}
			for (int j = 1; j <= 31; j++)
			{
				dataTable.Columns.Remove(dataTable.Columns[j.ToString()]);
			}
			HttpResult httpResult = await _httpClient.OutletVisitingFrequencyClient.Save(dataTable);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.None, Apzon.Commons.Properties.Resources.Msg_Success + httpResult.Content);
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
		WinCommons._CloseProcessing();
	}

	private void adv_Doc_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
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

	private void gv_Doc_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			if (gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, e.Column.FieldName).Equals("Y"))
			{
				if ("EveryDay".Equals(e.Column.FieldName))
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "DayOfWeek", "N");
					gv_Doc.SetRowCellValue(e.RowHandle, "DayOfMonth", "N");
				}
				else if ("DayOfMonth".Equals(e.Column.FieldName))
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "EveryDay", "N");
					gv_Doc.SetRowCellValue(e.RowHandle, "DayOfWeek", "N");
				}
				else if ("DayOfWeek".Equals(e.Column.FieldName))
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "EveryDay", "N");
					gv_Doc.SetRowCellValue(e.RowHandle, "DayOfMonth", "N");
				}
				else if (lstDayOfWeek.Contains(e.Column.FieldName))
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "DayOfWeek", "Y");
				}
				else if (Function.ParseInt(e.Column.FieldName) >= 1)
				{
					gv_Doc.SetRowCellValue(e.RowHandle, "DayOfMonth", "Y");
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			string text = Function.ToString(gridViewEx.GetRowCellValue(gridViewEx.FocusedRowHandle, "EveryDay"));
			if ((lstDayOfWeek.Contains(gridViewEx.FocusedColumn.FieldName) || Function.ParseInt(gridViewEx.FocusedColumn.FieldName) >= 1) && text.Equals("Y"))
			{
				e.Cancel = true;
				return;
			}
			string text2 = Function.ToString(gridViewEx.GetRowCellValue(gridViewEx.FocusedRowHandle, "DayOfWeek"));
			if (Function.ParseInt(gridViewEx.FocusedColumn.FieldName) >= 1 && text2.Equals("Y"))
			{
				e.Cancel = true;
				return;
			}
			string text3 = Function.ToString(gridViewEx.GetRowCellValue(gridViewEx.FocusedRowHandle, "DayOfMonth"));
			if (lstDayOfWeek.Contains(gridViewEx.FocusedColumn.FieldName) && text3.Equals("Y"))
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

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Refresh_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			grd_Doc = new GridControlEx();
			grd_Doc.DataSource = new DataTable();
			InitGridViewEx();
			DataTable dt = await _httpClient.OutletVisitingFrequencyClient.GetData(GlobalData.UserName, GlobalData.DefaultBranch);
			if (!dt.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.None, "Data is Empty!");
				return;
			}
			DataTable dataSource = CreateDataSource(dt);
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void tv_Details_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
			{
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				e.CanFocus = false;
				return;
			}
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				dataTable.RejectChanges();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Routes_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		try
		{
			DataTable dataTable = grd_Doc.DatasourceTable();
			DataTable dataSourceTable = grd_Routes.DataSourceTable;
			int focusedDataSourceRowIndex = gv_Routes.GetFocusedDataSourceRowIndex();
			if (e.Column.FieldName == "Recurring")
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.UpdateValueLineByValueHeader, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					if (dataTable != null && dataTable.Rows.Count > 0)
					{
						for (int i = 0; i < dataTable.Rows.Count; i++)
						{
							dataTable.Rows[i]["Recurring"] = Function.ToString(e.Value);
						}
					}
					dataSourceTable.Rows[focusedDataSourceRowIndex]["Recurring"] = "";
				}
				else if (DtFrequency != null && DtFrequency.Rows.Count > 0)
				{
					for (int j = 0; j < DtFrequency.Rows.Count; j++)
					{
						DtFrequency.Rows[j]["Recurring"] = Function.ToString(e.Value);
					}
				}
			}
			else if (e.Column.FieldName == "RecurNum")
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.UpdateValueLineByValueHeader, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
				{
					if (dataTable != null && dataTable.Rows.Count > 0)
					{
						for (int k = 0; k < dataTable.Rows.Count; k++)
						{
							dataTable.Rows[k]["RecurNum"] = Function.ToString(e.Value);
						}
					}
					dataSourceTable.Rows[focusedDataSourceRowIndex]["RecurNum"] = DBNull.Value;
				}
				else if (DtFrequency != null && DtFrequency.Rows.Count > 0)
				{
					for (int l = 0; l < DtFrequency.Rows.Count; l++)
					{
						DtFrequency.Rows[l]["RecurNum"] = Function.ToString(e.Value);
					}
				}
			}
			if (e.Column.FieldName == "NumberOfVisits" && DtFrequency != null && DtFrequency.Rows.Count > 0)
			{
				for (int m = 0; m < DtFrequency.Rows.Count; m++)
				{
					DtFrequency.Rows[m]["NumberOfVisits"] = Function.ToString(e.Value);
					DtFrequency.Rows[m]["DayOfWeek"] = "N";
					DtFrequency.Rows[m]["DayOfMonth"] = "N";
					DtFrequency.Rows[m]["Mon"] = "N";
					DtFrequency.Rows[m]["Tue"] = "N";
					DtFrequency.Rows[m]["Wed"] = "N";
					DtFrequency.Rows[m]["Thu"] = "N";
					DtFrequency.Rows[m]["Fri"] = "N";
					DtFrequency.Rows[m]["Sat"] = "N";
					DtFrequency.Rows[m]["Sun"] = "N";
					for (int n = 1; n <= 31; n++)
					{
						DtFrequency.Rows[m][n.ToString()] = "N";
					}
				}
			}
			if (FormMode != Enums.FormViewModeType.Update)
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.DMS.frm_OutletVisitingFrequency));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.adv_Doc = new Apzon.Controls.Grid.AdvBandedGridViewEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.dock = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Routes = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Routes = new Apzon.Controls.Grid.GridViewEx();
		this.gc_UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_RouteCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_RouteName = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CountryCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CountryCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_DocStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DocStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_RouteRmarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ChannelCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Channel = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gc_CreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CreateTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UpdateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UpdateTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Recurring = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_reccurringDOc = new Apzon.Controls.Editor.MyLookUpEdit.RepositoryItemLookUpEditSimple();
		this.gc_RecurNum = new Apzon.Controls.Grid.GridColumnEx();
		this.split_Employee = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.tv_Details = new Apzon.Controls.Tree.TreeEx();
		this.tc_Id = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_RouteCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_Type = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tc_Code = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_ChooseType = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tc_Name = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_CountryCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_Status1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_Status1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tc_Remarks1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_SuperVisor = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_Employee = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.treeListColumnEx8 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_SalesAdmin = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.treeListColumnEx12 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_LevelGgp = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tc_BaseId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_Geography = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btn_AddTree = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_BP = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_BPCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gv_NumberOfVisits = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.adv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.dock).BeginInit();
		this.dock.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Routes).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Routes).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CountryCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Channel).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_reccurringDOc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.split_Employee).BeginInit();
		this.split_Employee.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tv_Details).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ChooseType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Employee).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalesAdmin).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Geography).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_AddTree).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BP).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BPCode).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.adv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(804, 319);
		this.grd_Doc.TabIndex = 1;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.adv_Doc });
		this.adv_Doc.Appearance.BandPanel.Options.UseTextOptions = true;
		this.adv_Doc.Appearance.BandPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.adv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.adv_Doc.FormId = null;
		this.adv_Doc.GridControl = this.grd_Doc;
		this.adv_Doc.IndicatorWidth = 30;
		this.adv_Doc.IsApzTable = false;
		this.adv_Doc.ItemId = null;
		this.adv_Doc.Name = "adv_Doc";
		this.adv_Doc.OptionsView.ShowGroupPanel = false;
		this.adv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.adv_Doc_CellValueChanged);
		this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl1.Controls.Add(this.btn_Refresh);
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Controls.Add(this.btn_OK);
		this.panelControl1.Location = new System.Drawing.Point(0, 499);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(1185, 32);
		this.panelControl1.TabIndex = 2;
		this.btn_Refresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Refresh.EditControl = null;
		this.btn_Refresh.EditValue = "Refresh";
		this.btn_Refresh.FieldName = null;
		this.btn_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Refresh.ImageOptions.Image");
		this.btn_Refresh.IsSearchControl = false;
		this.btn_Refresh.Location = new System.Drawing.Point(167, 4);
		this.btn_Refresh.Name = "btn_Refresh";
		this.btn_Refresh.ReadOnly = false;
		this.btn_Refresh.SettingInVisible = false;
		this.btn_Refresh.SettingReadonly = false;
		this.btn_Refresh.Size = new System.Drawing.Size(75, 23);
		this.btn_Refresh.TabIndex = 7;
		this.btn_Refresh.Text = "Refresh";
		this.btn_Refresh.Visible = false;
		this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(86, 4);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 6;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(5, 4);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 23);
		this.btn_OK.TabIndex = 5;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.dock.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.dock.Horizontal = false;
		this.dock.Location = new System.Drawing.Point(5, 12);
		this.dock.Name = "dock";
		this.dock.Panel1.Controls.Add(this.grd_Routes);
		this.dock.Panel1.Text = "Panel1";
		this.dock.Panel2.Controls.Add(this.split_Employee);
		this.dock.Panel2.Text = "Panel2";
		this.dock.Size = new System.Drawing.Size(1180, 481);
		this.dock.SplitterPosition = 152;
		this.dock.TabIndex = 6;
		this.dock.Text = "splitContainerControl1";
		this.grd_Routes.AllowPasteExcel = true;
		this.grd_Routes.ClipboardData = null;
		this.grd_Routes.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Routes.Location = new System.Drawing.Point(0, 0);
		this.grd_Routes.MainView = this.gv_Routes;
		this.grd_Routes.Name = "grd_Routes";
		this.grd_Routes.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_DocStatus, this.rep_CountryCode, this.rep_Channel, this.luk_reccurringDOc });
		this.grd_Routes.Size = new System.Drawing.Size(1180, 152);
		this.grd_Routes.TabIndex = 6;
		this.grd_Routes.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Routes });
		this.gv_Routes.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Routes.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Routes.AutoFillColumn = this.gc_UserSign;
		this.gv_Routes.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clOutletVisitingFrequency;
		this.gv_Routes.ColumnKey = null;
		this.gv_Routes.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[14]
		{
			this.gc_RouteCode, this.gc_RouteName, this.gc_CountryCode, this.gc_DocStatus, this.gc_RouteRmarks, this.gc_ChannelCode, this.gc_UserSign, this.gc_CreateDate, this.gc_CreateTime, this.gc_UpdateDate,
			this.gc_UpdateTime, this.gc_Recurring, this.gc_RecurNum, this.gv_NumberOfVisits
		});
		this.gv_Routes.DisableFormSetting = false;
		this.gv_Routes.DisplayException = false;
		this.gv_Routes.EditColumn = null;
		this.gv_Routes.FieldCurrency = null;
		this.gv_Routes.FieldRate = null;
		this.gv_Routes.FormId = "oSalesRoute";
		this.gv_Routes.GridControl = this.grd_Routes;
		this.gv_Routes.HasFormSetting = false;
		this.gv_Routes.IgnoreFilter = false;
		this.gv_Routes.IndexEditRow = -1;
		this.gv_Routes.IndicatorWidth = 35;
		this.gv_Routes.IsApzTable = false;
		this.gv_Routes.IsInsertNewRow = false;
		this.gv_Routes.IsLoadingData = false;
		this.gv_Routes.IsMappingContextAction = false;
		this.gv_Routes.ItemId = "APZ_D_OSTR";
		this.gv_Routes.LstColumnAllowBatch = null;
		this.gv_Routes.LstColumnNotAllowBatch = null;
		this.gv_Routes.LstColumnNotEditable = null;
		this.gv_Routes.LstColumNotSetting = null;
		this.gv_Routes.LstOldUDF = null;
		this.gv_Routes.Name = "gv_Routes";
		this.gv_Routes.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Routes.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Routes.OptionsPrint.AutoWidth = false;
		this.gv_Routes.OptionsPrint.UsePrintStyles = false;
		this.gv_Routes.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Routes.OptionsView.ColumnAutoWidth = false;
		this.gv_Routes.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Routes.OptionsView.ShowGroupedColumns = true;
		this.gv_Routes.OptionsView.ShowGroupPanel = false;
		this.gv_Routes.UseBatchMod = false;
		this.gv_Routes.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Routes_FocusedRowObjectChanged);
		this.gv_Routes.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Routes_CellValueChanged);
		this.gc_UserSign.Caption = "User Sign";
		this.gc_UserSign.CurrencyDateField = null;
		this.gc_UserSign.CurrencyField = null;
		this.gc_UserSign.CurrencyFixed = null;
		this.gc_UserSign.CurrencyFixedRate = 0.0;
		this.gc_UserSign.CurrencyRateField = null;
		this.gc_UserSign.FieldName = "UserSign";
		this.gc_UserSign.Name = "gc_UserSign";
		this.gc_UserSign.OptionsColumn.ReadOnly = true;
		this.gc_UserSign.PrimaryKey = false;
		this.gc_UserSign.UnitField = null;
		this.gc_RouteCode.Caption = "Mã";
		this.gc_RouteCode.CurrencyDateField = null;
		this.gc_RouteCode.CurrencyField = null;
		this.gc_RouteCode.CurrencyFixed = null;
		this.gc_RouteCode.CurrencyFixedRate = 0.0;
		this.gc_RouteCode.CurrencyRateField = null;
		this.gc_RouteCode.FieldName = "RouteCode";
		this.gc_RouteCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_RouteCode.Name = "gc_RouteCode";
		this.gc_RouteCode.OptionsColumn.ReadOnly = true;
		this.gc_RouteCode.PrimaryKey = true;
		this.gc_RouteCode.UnitField = null;
		this.gc_RouteCode.Visible = true;
		this.gc_RouteCode.VisibleIndex = 0;
		this.gc_RouteCode.Width = 83;
		this.gc_RouteName.Caption = "Tên";
		this.gc_RouteName.CurrencyDateField = null;
		this.gc_RouteName.CurrencyField = null;
		this.gc_RouteName.CurrencyFixed = null;
		this.gc_RouteName.CurrencyFixedRate = 0.0;
		this.gc_RouteName.CurrencyRateField = null;
		this.gc_RouteName.FieldName = "RouteName";
		this.gc_RouteName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gc_RouteName.Name = "gc_RouteName";
		this.gc_RouteName.OptionsColumn.ReadOnly = true;
		this.gc_RouteName.PrimaryKey = false;
		this.gc_RouteName.UnitField = null;
		this.gc_RouteName.Visible = true;
		this.gc_RouteName.VisibleIndex = 1;
		this.gc_RouteName.Width = 183;
		this.gc_CountryCode.Caption = "Mã quốc gia";
		this.gc_CountryCode.ColumnEdit = this.rep_CountryCode;
		this.gc_CountryCode.CurrencyDateField = null;
		this.gc_CountryCode.CurrencyField = null;
		this.gc_CountryCode.CurrencyFixed = null;
		this.gc_CountryCode.CurrencyFixedRate = 0.0;
		this.gc_CountryCode.CurrencyRateField = null;
		this.gc_CountryCode.FieldName = "CountryCode";
		this.gc_CountryCode.Name = "gc_CountryCode";
		this.gc_CountryCode.OptionsColumn.ReadOnly = true;
		this.gc_CountryCode.PrimaryKey = false;
		this.gc_CountryCode.UnitField = null;
		this.gc_CountryCode.Visible = true;
		this.gc_CountryCode.VisibleIndex = 2;
		this.gc_CountryCode.Width = 147;
		this.rep_CountryCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CountryCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CountryCode.Name = "rep_CountryCode";
		this.rep_CountryCode.ReadOnly = true;
		this.gc_DocStatus.Caption = "Trạng thái";
		this.gc_DocStatus.ColumnEdit = this.rep_DocStatus;
		this.gc_DocStatus.CurrencyDateField = null;
		this.gc_DocStatus.CurrencyField = null;
		this.gc_DocStatus.CurrencyFixed = null;
		this.gc_DocStatus.CurrencyFixedRate = 0.0;
		this.gc_DocStatus.CurrencyRateField = null;
		this.gc_DocStatus.FieldName = "Status";
		this.gc_DocStatus.Name = "gc_DocStatus";
		this.gc_DocStatus.OptionsColumn.ReadOnly = true;
		this.gc_DocStatus.PrimaryKey = false;
		this.gc_DocStatus.UnitField = null;
		this.gc_DocStatus.Visible = true;
		this.gc_DocStatus.VisibleIndex = 3;
		this.rep_DocStatus.AutoHeight = false;
		this.rep_DocStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DocStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_DocStatus.DisplayMember = "Name";
		this.rep_DocStatus.Name = "rep_DocStatus";
		this.rep_DocStatus.NullText = "";
		this.rep_DocStatus.ShowHeader = false;
		this.rep_DocStatus.ValueMember = "Code";
		this.gc_RouteRmarks.Caption = "Diễn giải";
		this.gc_RouteRmarks.CurrencyDateField = null;
		this.gc_RouteRmarks.CurrencyField = null;
		this.gc_RouteRmarks.CurrencyFixed = null;
		this.gc_RouteRmarks.CurrencyFixedRate = 0.0;
		this.gc_RouteRmarks.CurrencyRateField = null;
		this.gc_RouteRmarks.FieldName = "Remarks";
		this.gc_RouteRmarks.Name = "gc_RouteRmarks";
		this.gc_RouteRmarks.OptionsColumn.ReadOnly = true;
		this.gc_RouteRmarks.PrimaryKey = false;
		this.gc_RouteRmarks.UnitField = null;
		this.gc_RouteRmarks.Visible = true;
		this.gc_RouteRmarks.VisibleIndex = 4;
		this.gc_RouteRmarks.Width = 252;
		this.gc_ChannelCode.Caption = "Kênh";
		this.gc_ChannelCode.ColumnEdit = this.rep_Channel;
		this.gc_ChannelCode.CurrencyDateField = null;
		this.gc_ChannelCode.CurrencyField = null;
		this.gc_ChannelCode.CurrencyFixed = null;
		this.gc_ChannelCode.CurrencyFixedRate = 0.0;
		this.gc_ChannelCode.CurrencyRateField = null;
		this.gc_ChannelCode.FieldName = "ChannelCode";
		this.gc_ChannelCode.Name = "gc_ChannelCode";
		this.gc_ChannelCode.OptionsColumn.ReadOnly = true;
		this.gc_ChannelCode.PrimaryKey = false;
		this.gc_ChannelCode.UnitField = null;
		this.gc_ChannelCode.Visible = true;
		this.gc_ChannelCode.VisibleIndex = 5;
		this.gc_ChannelCode.Width = 125;
		this.rep_Channel.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Channel.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Channel.Name = "rep_Channel";
		this.rep_Channel.ReadOnly = true;
		this.gc_CreateDate.Caption = "Create Date";
		this.gc_CreateDate.CurrencyDateField = null;
		this.gc_CreateDate.CurrencyField = null;
		this.gc_CreateDate.CurrencyFixed = null;
		this.gc_CreateDate.CurrencyFixedRate = 0.0;
		this.gc_CreateDate.CurrencyRateField = null;
		this.gc_CreateDate.FieldName = "CreateDate";
		this.gc_CreateDate.Name = "gc_CreateDate";
		this.gc_CreateDate.OptionsColumn.ReadOnly = true;
		this.gc_CreateDate.PrimaryKey = false;
		this.gc_CreateDate.UnitField = null;
		this.gc_CreateTime.Caption = "Create Time";
		this.gc_CreateTime.CurrencyDateField = null;
		this.gc_CreateTime.CurrencyField = null;
		this.gc_CreateTime.CurrencyFixed = null;
		this.gc_CreateTime.CurrencyFixedRate = 0.0;
		this.gc_CreateTime.CurrencyRateField = null;
		this.gc_CreateTime.FieldName = "CreateTime";
		this.gc_CreateTime.Name = "gc_CreateTime";
		this.gc_CreateTime.OptionsColumn.ReadOnly = true;
		this.gc_CreateTime.PrimaryKey = false;
		this.gc_CreateTime.UnitField = null;
		this.gc_UpdateDate.Caption = "Update Date";
		this.gc_UpdateDate.CurrencyDateField = null;
		this.gc_UpdateDate.CurrencyField = null;
		this.gc_UpdateDate.CurrencyFixed = null;
		this.gc_UpdateDate.CurrencyFixedRate = 0.0;
		this.gc_UpdateDate.CurrencyRateField = null;
		this.gc_UpdateDate.FieldName = "UpdateDate";
		this.gc_UpdateDate.Name = "gc_UpdateDate";
		this.gc_UpdateDate.OptionsColumn.ReadOnly = true;
		this.gc_UpdateDate.PrimaryKey = false;
		this.gc_UpdateDate.UnitField = null;
		this.gc_UpdateTime.Caption = "Update Time";
		this.gc_UpdateTime.CurrencyDateField = null;
		this.gc_UpdateTime.CurrencyField = null;
		this.gc_UpdateTime.CurrencyFixed = null;
		this.gc_UpdateTime.CurrencyFixedRate = 0.0;
		this.gc_UpdateTime.CurrencyRateField = null;
		this.gc_UpdateTime.FieldName = "UpdateTime";
		this.gc_UpdateTime.Name = "gc_UpdateTime";
		this.gc_UpdateTime.OptionsColumn.ReadOnly = true;
		this.gc_UpdateTime.PrimaryKey = false;
		this.gc_UpdateTime.UnitField = null;
		this.gc_Recurring.Caption = "Loại";
		this.gc_Recurring.ColumnEdit = this.luk_reccurringDOc;
		this.gc_Recurring.CurrencyDateField = null;
		this.gc_Recurring.CurrencyField = null;
		this.gc_Recurring.CurrencyFixed = null;
		this.gc_Recurring.CurrencyFixedRate = 0.0;
		this.gc_Recurring.CurrencyRateField = null;
		this.gc_Recurring.FieldName = "Recurring";
		this.gc_Recurring.Name = "gc_Recurring";
		this.gc_Recurring.PrimaryKey = false;
		this.gc_Recurring.UnitField = null;
		this.gc_Recurring.Visible = true;
		this.gc_Recurring.VisibleIndex = 6;
		this.luk_reccurringDOc.AutoHeight = false;
		this.luk_reccurringDOc.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_reccurringDOc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_reccurringDOc.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.luk_reccurringDOc.DisplayMember = "Name";
		this.luk_reccurringDOc.IsSearchControl = false;
		this.luk_reccurringDOc.Name = "luk_reccurringDOc";
		this.luk_reccurringDOc.NullText = "";
		this.luk_reccurringDOc.ShowFooter = false;
		this.luk_reccurringDOc.ShowHeader = false;
		this.gc_RecurNum.Caption = "Số ngày/tuần/tháng";
		this.gc_RecurNum.CurrencyDateField = null;
		this.gc_RecurNum.CurrencyField = null;
		this.gc_RecurNum.CurrencyFixed = null;
		this.gc_RecurNum.CurrencyFixedRate = 0.0;
		this.gc_RecurNum.CurrencyRateField = null;
		this.gc_RecurNum.FieldName = "RecurNum";
		this.gc_RecurNum.Name = "gc_RecurNum";
		this.gc_RecurNum.PrimaryKey = false;
		this.gc_RecurNum.UnitField = null;
		this.gc_RecurNum.Visible = true;
		this.gc_RecurNum.VisibleIndex = 7;
		this.split_Employee.Dock = System.Windows.Forms.DockStyle.Fill;
		this.split_Employee.Location = new System.Drawing.Point(0, 0);
		this.split_Employee.Name = "split_Employee";
		this.split_Employee.Panel1.Controls.Add(this.groupControl1);
		this.split_Employee.Panel1.Text = "Panel1";
		this.split_Employee.Panel2.Controls.Add(this.grd_Doc);
		this.split_Employee.Panel2.Text = "Panel2";
		this.split_Employee.Size = new System.Drawing.Size(1180, 319);
		this.split_Employee.SplitterPosition = 366;
		this.split_Employee.TabIndex = 10;
		this.groupControl1.Controls.Add(this.tv_Details);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(366, 319);
		this.groupControl1.TabIndex = 8;
		this.groupControl1.Text = "Chi tiết";
		this.tv_Details.AllowPasteExcel = false;
		this.tv_Details.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clOutletVisitingFrequency;
		this.tv_Details.ColumnKey = null;
		this.tv_Details.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[13]
		{
			this.tc_Id, this.tc_RouteCode, this.tc_Type, this.tc_Code, this.tc_Name, this.tc_CountryCode, this.tc_Status1, this.tc_Remarks1, this.tc_SuperVisor, this.treeListColumnEx8,
			this.treeListColumnEx12, this.tc_LevelGgp, this.tc_BaseId
		});
		this.tv_Details.Cursor = System.Windows.Forms.Cursors.Default;
		this.tv_Details.DisableFormSetting = false;
		this.tv_Details.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tv_Details.EditColumn = null;
		this.tv_Details.FieldCurrency = null;
		this.tv_Details.FieldRate = null;
		this.tv_Details.FormId = "oSalesRoute";
		this.tv_Details.HasFormSetting = false;
		this.tv_Details.HorzScrollVisibility = DevExpress.XtraTreeList.ScrollVisibility.Always;
		this.tv_Details.IndexEditRow = -1;
		this.tv_Details.IndicatorWidth = 40;
		this.tv_Details.IsApzTable = false;
		this.tv_Details.IsLoadingData = false;
		this.tv_Details.ItemId = "APZ_D_STR1";
		this.tv_Details.KeyFieldName = "Id";
		this.tv_Details.Location = new System.Drawing.Point(2, 23);
		this.tv_Details.LstColumnNotEditable = null;
		this.tv_Details.LstColumNotSetting = null;
		this.tv_Details.LstOldUDF = null;
		this.tv_Details.Name = "tv_Details";
		this.tv_Details.OptionsBehavior.AllowPixelScrolling = DevExpress.Utils.DefaultBoolean.True;
		this.tv_Details.OptionsBehavior.ReadOnly = true;
		this.tv_Details.OptionsMenu.ShowExpandCollapseItems = false;
		this.tv_Details.OptionsNavigation.AutoMoveRowFocus = true;
		this.tv_Details.OptionsView.AutoWidth = false;
		this.tv_Details.ParentFieldName = "FatherId";
		this.tv_Details.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.rep_Employee, this.rep_Geography, this.rep_Status1, this.btn_AddTree, this.rep_Type, this.rep_BP, this.rep_BPCode, this.rep_ChooseType, this.rep_SalesAdmin });
		this.tv_Details.Size = new System.Drawing.Size(362, 294);
		this.tv_Details.TabIndex = 9;
		this.tv_Details.VertScrollVisibility = DevExpress.XtraTreeList.ScrollVisibility.Always;
		this.tv_Details.BeforeFocusNode += new DevExpress.XtraTreeList.BeforeFocusNodeEventHandler(this.tv_Details_BeforeFocusNode);
		this.tv_Details.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tv_Details_FocusedNodeChanged);
		this.tc_Id.Caption = "Id";
		this.tc_Id.CurrencyDateField = null;
		this.tc_Id.CurrencyField = null;
		this.tc_Id.CurrencyFixed = null;
		this.tc_Id.CurrencyFixedRate = 0.0;
		this.tc_Id.CurrencyRateField = null;
		this.tc_Id.FieldName = "Id";
		this.tc_Id.Name = "tc_Id";
		this.tc_Id.OptionsColumn.AllowEdit = false;
		this.tc_Id.OptionsColumn.ReadOnly = true;
		this.tc_RouteCode.Caption = "Route Code";
		this.tc_RouteCode.CurrencyDateField = null;
		this.tc_RouteCode.CurrencyField = null;
		this.tc_RouteCode.CurrencyFixed = null;
		this.tc_RouteCode.CurrencyFixedRate = 0.0;
		this.tc_RouteCode.CurrencyRateField = null;
		this.tc_RouteCode.FieldName = "RouteCode";
		this.tc_RouteCode.Name = "tc_RouteCode";
		this.tc_RouteCode.OptionsColumn.ReadOnly = true;
		this.tc_Type.Caption = "Loại";
		this.tc_Type.ColumnEdit = this.rep_Type;
		this.tc_Type.CurrencyDateField = null;
		this.tc_Type.CurrencyField = null;
		this.tc_Type.CurrencyFixed = null;
		this.tc_Type.CurrencyFixedRate = 0.0;
		this.tc_Type.CurrencyRateField = null;
		this.tc_Type.FieldName = "Type";
		this.tc_Type.Name = "tc_Type";
		this.tc_Type.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tc_Type.Visible = true;
		this.tc_Type.VisibleIndex = 0;
		this.tc_Type.Width = 199;
		this.rep_Type.AppearanceDropDown.BackColor = System.Drawing.Color.FromArgb(250, 249, 215);
		this.rep_Type.AppearanceDropDown.Options.UseBackColor = true;
		this.rep_Type.AutoHeight = false;
		this.rep_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Type.DisplayMember = "Name";
		this.rep_Type.Name = "rep_Type";
		this.rep_Type.NullText = "";
		this.rep_Type.ShowHeader = false;
		this.rep_Type.ValueMember = "Code";
		this.tc_Code.Caption = "Mã";
		this.tc_Code.ColumnEdit = this.rep_ChooseType;
		this.tc_Code.CurrencyDateField = null;
		this.tc_Code.CurrencyField = null;
		this.tc_Code.CurrencyFixed = null;
		this.tc_Code.CurrencyFixedRate = 0.0;
		this.tc_Code.CurrencyRateField = null;
		this.tc_Code.FieldName = "Code";
		this.tc_Code.Name = "tc_Code";
		this.tc_Code.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowForFocusedRow;
		this.tc_Code.Visible = true;
		this.tc_Code.VisibleIndex = 1;
		this.tc_Code.Width = 138;
		this.rep_ChooseType.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_ChooseType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_ChooseType.Name = "rep_ChooseType";
		this.tc_Name.Caption = "Tên";
		this.tc_Name.CurrencyDateField = null;
		this.tc_Name.CurrencyField = null;
		this.tc_Name.CurrencyFixed = null;
		this.tc_Name.CurrencyFixedRate = 0.0;
		this.tc_Name.CurrencyRateField = null;
		this.tc_Name.FieldName = "Name";
		this.tc_Name.Name = "tc_Name";
		this.tc_Name.Visible = true;
		this.tc_Name.VisibleIndex = 2;
		this.tc_Name.Width = 197;
		this.tc_CountryCode.Caption = "Country Code";
		this.tc_CountryCode.CurrencyDateField = null;
		this.tc_CountryCode.CurrencyField = null;
		this.tc_CountryCode.CurrencyFixed = null;
		this.tc_CountryCode.CurrencyFixedRate = 0.0;
		this.tc_CountryCode.CurrencyRateField = null;
		this.tc_CountryCode.FieldName = "CountryCode";
		this.tc_CountryCode.Name = "tc_CountryCode";
		this.tc_CountryCode.OptionsColumn.ReadOnly = true;
		this.tc_CountryCode.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tc_Status1.Caption = "Trạng thái";
		this.tc_Status1.ColumnEdit = this.rep_Status1;
		this.tc_Status1.CurrencyDateField = null;
		this.tc_Status1.CurrencyField = null;
		this.tc_Status1.CurrencyFixed = null;
		this.tc_Status1.CurrencyFixedRate = 0.0;
		this.tc_Status1.CurrencyRateField = null;
		this.tc_Status1.FieldName = "Status";
		this.tc_Status1.Name = "tc_Status1";
		this.tc_Status1.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tc_Status1.Visible = true;
		this.tc_Status1.VisibleIndex = 4;
		this.tc_Status1.Width = 106;
		this.rep_Status1.AutoHeight = false;
		this.rep_Status1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status1.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Status1.DisplayMember = "Name";
		this.rep_Status1.Name = "rep_Status1";
		this.rep_Status1.NullText = "";
		this.rep_Status1.ShowHeader = false;
		this.rep_Status1.ValueMember = "Code";
		this.tc_Remarks1.Caption = "Diễn giải";
		this.tc_Remarks1.CurrencyDateField = null;
		this.tc_Remarks1.CurrencyField = null;
		this.tc_Remarks1.CurrencyFixed = null;
		this.tc_Remarks1.CurrencyFixedRate = 0.0;
		this.tc_Remarks1.CurrencyRateField = null;
		this.tc_Remarks1.FieldName = "Remarks";
		this.tc_Remarks1.Name = "tc_Remarks1";
		this.tc_Remarks1.Visible = true;
		this.tc_Remarks1.VisibleIndex = 5;
		this.tc_Remarks1.Width = 196;
		this.tc_SuperVisor.Caption = "Giám sát kinh doanh";
		this.tc_SuperVisor.ColumnEdit = this.rep_Employee;
		this.tc_SuperVisor.CurrencyDateField = null;
		this.tc_SuperVisor.CurrencyField = null;
		this.tc_SuperVisor.CurrencyFixed = null;
		this.tc_SuperVisor.CurrencyFixedRate = 0.0;
		this.tc_SuperVisor.CurrencyRateField = null;
		this.tc_SuperVisor.FieldName = "SuperVisor";
		this.tc_SuperVisor.Name = "tc_SuperVisor";
		this.tc_SuperVisor.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tc_SuperVisor.Visible = true;
		this.tc_SuperVisor.VisibleIndex = 3;
		this.tc_SuperVisor.Width = 130;
		this.rep_Employee.AutoHeight = false;
		this.rep_Employee.AutoLoadDataSource = true;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Employee.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Employee.ContainerControl = this;
		this.rep_Employee.DisplayMember = "";
		this.rep_Employee.DisplayMember2 = "";
		this.rep_Employee.LoadDataSourceWhenEnter = false;
		this.rep_Employee.Name = "rep_Employee";
		this.rep_Employee.SeparateCharacterForMulti = ';';
		this.rep_Employee.SeparateString = ", ";
		this.rep_Employee.UsingAutoCompleteBox = false;
		this.rep_Employee.UsingDataSource = true;
		this.rep_Employee.ValidateWhenNoDataSource = false;
		this.rep_Employee.ValueMember = "";
		this.treeListColumnEx8.Caption = "Trợ lý kinh doanh";
		this.treeListColumnEx8.ColumnEdit = this.rep_SalesAdmin;
		this.treeListColumnEx8.CurrencyDateField = null;
		this.treeListColumnEx8.CurrencyField = null;
		this.treeListColumnEx8.CurrencyFixed = null;
		this.treeListColumnEx8.CurrencyFixedRate = 0.0;
		this.treeListColumnEx8.CurrencyRateField = null;
		this.treeListColumnEx8.FieldName = "SalesAdmin";
		this.treeListColumnEx8.Name = "treeListColumnEx8";
		this.treeListColumnEx8.Visible = true;
		this.treeListColumnEx8.VisibleIndex = 6;
		this.rep_SalesAdmin.AutoHeight = false;
		this.rep_SalesAdmin.AutoLoadDataSource = true;
		this.rep_SalesAdmin.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_SalesAdmin.ContainerControl = this;
		this.rep_SalesAdmin.DisplayMember = "fullName";
		this.rep_SalesAdmin.DisplayMember2 = "";
		this.rep_SalesAdmin.LoadDataSourceWhenEnter = false;
		this.rep_SalesAdmin.Name = "rep_SalesAdmin";
		this.rep_SalesAdmin.SeparateCharacterForMulti = ';';
		this.rep_SalesAdmin.SeparateString = ", ";
		this.rep_SalesAdmin.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Employees;
		this.rep_SalesAdmin.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oEmployeesInfo;
		this.rep_SalesAdmin.UsingAutoCompleteBox = false;
		this.rep_SalesAdmin.UsingDataSource = true;
		this.rep_SalesAdmin.ValidateWhenNoDataSource = false;
		this.rep_SalesAdmin.ValueMember = "empID";
		this.treeListColumnEx12.Caption = "FatherId";
		this.treeListColumnEx12.CurrencyDateField = null;
		this.treeListColumnEx12.CurrencyField = null;
		this.treeListColumnEx12.CurrencyFixed = null;
		this.treeListColumnEx12.CurrencyFixedRate = 0.0;
		this.treeListColumnEx12.CurrencyRateField = null;
		this.treeListColumnEx12.FieldName = "FatherId";
		this.treeListColumnEx12.Name = "treeListColumnEx12";
		this.treeListColumnEx12.OptionsColumn.AllowEdit = false;
		this.treeListColumnEx12.OptionsColumn.ReadOnly = true;
		this.tc_LevelGgp.Caption = "LevelGgp";
		this.tc_LevelGgp.CurrencyDateField = null;
		this.tc_LevelGgp.CurrencyField = null;
		this.tc_LevelGgp.CurrencyFixed = null;
		this.tc_LevelGgp.CurrencyFixedRate = 0.0;
		this.tc_LevelGgp.CurrencyRateField = null;
		this.tc_LevelGgp.FieldName = "LevelGgp";
		this.tc_LevelGgp.Name = "tc_LevelGgp";
		this.tc_LevelGgp.OptionsColumn.AllowEdit = false;
		this.tc_LevelGgp.OptionsColumn.ReadOnly = true;
		this.tc_BaseId.Caption = "treeListColumnEx8";
		this.tc_BaseId.CurrencyDateField = null;
		this.tc_BaseId.CurrencyField = null;
		this.tc_BaseId.CurrencyFixed = null;
		this.tc_BaseId.CurrencyFixedRate = 0.0;
		this.tc_BaseId.CurrencyRateField = null;
		this.tc_BaseId.FieldName = "BaseId";
		this.tc_BaseId.Name = "tc_BaseId";
		this.rep_Geography.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_Geography.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Geography.Name = "rep_Geography";
		this.btn_AddTree.AutoHeight = false;
		this.btn_AddTree.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
		});
		this.btn_AddTree.Name = "btn_AddTree";
		this.rep_BP.AutoHeight = false;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_BP.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_BP.Name = "rep_BP";
		this.rep_BPCode.AutoHeight = false;
		editorButtonImageOptions7.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_BPCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Ellipsis, "", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_BPCode.Name = "rep_BPCode";
		this.gv_NumberOfVisits.Caption = "Số lần ghé thăm hàng tuần/tháng";
		this.gv_NumberOfVisits.CurrencyDateField = null;
		this.gv_NumberOfVisits.CurrencyField = null;
		this.gv_NumberOfVisits.CurrencyFixed = null;
		this.gv_NumberOfVisits.CurrencyFixedRate = 0.0;
		this.gv_NumberOfVisits.CurrencyRateField = null;
		this.gv_NumberOfVisits.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gv_NumberOfVisits.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Integer;
		this.gv_NumberOfVisits.FieldName = "NumberOfVisits";
		this.gv_NumberOfVisits.Name = "gv_NumberOfVisits";
		this.gv_NumberOfVisits.PrimaryKey = false;
		this.gv_NumberOfVisits.UnitField = null;
		this.gv_NumberOfVisits.Visible = true;
		this.gv_NumberOfVisits.VisibleIndex = 8;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1186, 531);
		base.Controls.Add(this.dock);
		base.Controls.Add(this.panelControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_OutletVisitingFrequency";
		base.ShowInTaskbar = false;
		this.Text = "Lịch ghé thăm cửa hàng/đại lý";
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.adv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.dock).EndInit();
		this.dock.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Routes).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Routes).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CountryCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DocStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Channel).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_reccurringDOc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.split_Employee).EndInit();
		this.split_Employee.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tv_Details).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ChooseType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Employee).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SalesAdmin).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Geography).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_AddTree).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BP).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BPCode).EndInit();
		base.ResumeLayout(false);
	}
}
