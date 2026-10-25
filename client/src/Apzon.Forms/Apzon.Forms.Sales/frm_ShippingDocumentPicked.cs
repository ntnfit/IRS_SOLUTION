using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Properties;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Sales;

public class frm_ShippingDocumentPicked : XtraFormEx
{
	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private RepositoryItemButtonEdit rep_btnDocNo;

	private RepositoryItemButtonEdit rep_btnBp;

	private RepositoryItemButtonEdit rep_btnItemCode;

	private RepositoryItemButtonEdit rep_btnWhsCode;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2;

	private RepositoryItemDateEdit rep_dateDoc;

	private RepositoryItemDateEdit rep_dateDe;

	private SimpleButtonEx btn_Release;

	private SimpleButtonEx btn_Close;

	private SplitContainer splitContainer1;

	private TextEditEx txt_docNo;

	private SimpleButtonEx btn_search;

	private ButtonEditEx txt_BpCode;

	private DateEditEx date_ToDate;

	private DateEditEx date_FromDate;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private SimpleButtonEx btn_PickList;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage1;

	private XtraTabControlEx xtraTabControl2;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grd_Line;

	private GridViewEx gv_Line;

	private RepositoryItemButtonEdit repositoryItemButtonEdit1;

	private RepositoryItemButtonEdit repositoryItemButtonEdit2;

	private RepositoryItemButtonEdit repositoryItemButtonEdit3;

	private RepositoryItemButtonEdit repositoryItemButtonEdit4;

	private RepositoryItemCalcEdit repositoryItemCalcEdit3;

	private RepositoryItemCalcEdit repositoryItemCalcEdit4;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private RepositoryItemDateEdit repositoryItemDateEdit2;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private GridColumnEx gridColumn17;

	private GridColumnEx gridColumn18;

	private GridColumnEx gridColumn19;

	private GridColumnEx gridColumn20;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumn22;

	private RepositoryItemCheckEdit rep_chk_Allocated;

	public frm_ShippingDocumentPicked()
	{
		InitializeComponent();
	}

	private void frm_ShippingDocumentPicked_Load(object sender, EventArgs e)
	{
		try
		{
			Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txt_BpCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_PickList_Click(object sender, EventArgs e)
	{
	}

	private void btn_search_Click(object sender, EventArgs e)
	{
		try
		{
			Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void Search()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("DocNum", typeof(string)));
			dataTable.Columns.Add(new DataColumn("LineNum", typeof(string)));
			dataTable.Columns.Add(new DataColumn("CardCode", typeof(string)));
			dataTable.Columns.Add(new DataColumn("CardName", typeof(string)));
			dataTable.Columns.Add(new DataColumn("ShipTo", typeof(string)));
			dataTable.Columns.Add(new DataColumn("ItemCode", typeof(string)));
			dataTable.Columns.Add(new DataColumn("PkgId", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Stage", typeof(string)));
			dataTable.Columns.Add(new DataColumn("PickedQty", typeof(string)));
			dataTable.Columns.Add(new DataColumn("VehicleId", typeof(string)));
			dataTable.Columns.Add(new DataColumn("PIC", typeof(string)));
			dataTable.Columns.Add(new DataColumn("RouteId", typeof(string)));
			dataTable.Columns.Add(new DataColumn("RouteOrder", typeof(string)));
			dataTable.Columns.Add(new DataColumn("RouteTime", typeof(string)));
			dataTable.Columns.Add(new DataColumn("RtDistance", typeof(string)));
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			dataRow["DocNum"] = "1";
			dataRow["LineNum"] = "0";
			dataRow["CardCode"] = "C0000001";
			dataRow["CardName"] = "Customer C0000001";
			dataRow["ShipTo"] = "101 road";
			dataRow["ItemCode"] = "I0000001";
			dataRow["PkgId"] = "123";
			dataRow["Stage"] = "1";
			dataRow["PickedQty"] = "20";
			dataRow["VehicleId"] = "ABC123";
			dataRow["PIC"] = "abc";
			dataRow["RouteId"] = "12";
			dataRow["RouteOrder"] = "1";
			dataRow["RouteTime"] = "09:12";
			dataRow["RtDistance"] = "22";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["DocNum"] = "1";
			dataRow2["LineNum"] = "1";
			dataRow2["CardCode"] = "C0000001";
			dataRow2["CardName"] = "Customer C0000001";
			dataRow2["ShipTo"] = "101 road";
			dataRow2["ItemCode"] = "I0000002";
			dataRow2["PkgId"] = "456";
			dataRow2["Stage"] = "2";
			dataRow2["PickedQty"] = "30";
			dataRow2["VehicleId"] = "ABC121";
			dataRow2["PIC"] = "def";
			dataRow2["RouteId"] = "11";
			dataRow2["RouteOrder"] = "2";
			dataRow2["RouteTime"] = "11:23";
			dataRow2["RtDistance"] = "123";
			dataTable.Rows.Add(dataRow2);
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add(new DataColumn("PackageId", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("Status", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("BatchNo", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("Allocated", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("ExpireDate", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("AdminDate", typeof(string)));
			dataTable2.Columns.Add(new DataColumn("Type", typeof(string)));
			dataTable2.AcceptChanges();
			DataRow dataRow3 = dataTable2.NewRow();
			dataRow3["PackageId"] = "123";
			dataRow3["Status"] = "Open";
			dataRow3["BatchNo"] = "12";
			dataRow3["Allocated"] = "Y";
			dataRow3["ExpireDate"] = "09-05-2017";
			dataRow3["AdminDate"] = "08-05-2017";
			dataRow3["Type"] = "B";
			dataTable2.Rows.Add(dataRow3);
			DataRow dataRow4 = dataTable2.NewRow();
			dataRow4["PackageId"] = "123";
			dataRow4["Status"] = "Open";
			dataRow4["BatchNo"] = "12";
			dataRow4["Allocated"] = "Y";
			dataRow4["ExpireDate"] = "09-05-2017";
			dataRow4["AdminDate"] = "08-05-2017";
			dataRow4["Type"] = "B";
			dataTable2.Rows.Add(dataRow4);
			dataTable2.AcceptChanges();
			grd_Line.DataSource = dataTable2;
			grd_Line.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_ShippingDocumentPicked));
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_btnDocNo = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btnBp = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btnItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btnWhsCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_dateDoc = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.rep_dateDe = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.btn_Release = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Close = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.xtraTabControl2 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Line = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_chk_Allocated = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemButtonEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemButtonEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemButtonEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemButtonEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.repositoryItemDateEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.repositoryItemCalcEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.repositoryItemCalcEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.txt_docNo = new Apzon.Controls.Editor.TextEditEx();
		this.btn_search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_BpCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_PickList = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnDocNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnBp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnWhsCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateDoc.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateDe).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateDe.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).BeginInit();
		this.xtraTabControl2.SuspendLayout();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Allocated).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_docNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rep_btnDocNo, this.rep_btnBp, this.rep_btnItemCode, this.rep_btnWhsCode, this.rep_dateDoc, this.rep_dateDe, this.repositoryItemCalcEdit1, this.repositoryItemCalcEdit2 });
		this.grd_Doc.Size = new System.Drawing.Size(1035, 250);
		this.grd_Doc.TabIndex = 12;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		DevExpress.XtraGrid.Columns.GridColumnCollection columns = this.gv_Doc.Columns;
		DevExpress.XtraGrid.Columns.GridColumn[] columns2 = new Apzon.Controls.Grid.GridColumnEx[15]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumn11, this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15
		};
		columns.AddRange(columns2);
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IndicatorWidth = 50;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsDetail.DetailMode = DevExpress.XtraGrid.Views.Grid.DetailMode.Default;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gridColumn1.Caption = "Doc No";
		this.gridColumn1.FieldName = "DocNum";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 14;
		this.gridColumn2.Caption = "Doc Row";
		this.gridColumn2.FieldName = "LineNum";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn3.Caption = "BP Code";
		this.gridColumn3.FieldName = "CardCode";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn4.Caption = "BP Name";
		this.gridColumn4.FieldName = "CardName";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 2;
		this.gridColumn5.Caption = "Ship To";
		this.gridColumn5.FieldName = "ShipTo";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 3;
		this.gridColumn6.Caption = "Item Code";
		this.gridColumn6.FieldName = "ItemCode";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 4;
		this.gridColumn7.Caption = "Packageg ID";
		this.gridColumn7.FieldName = "PkgId";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 5;
		this.gridColumn8.Caption = "Stage";
		this.gridColumn8.FieldName = "Stage";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 6;
		this.gridColumn9.Caption = "Picked";
		this.gridColumn9.FieldName = "PickedQty";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 7;
		this.gridColumn10.Caption = "Vehicle ID";
		this.gridColumn10.FieldName = "VehicleId";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 8;
		this.gridColumn11.Caption = "P.I.C";
		this.gridColumn11.FieldName = "PIC";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 9;
		this.gridColumn12.Caption = "Route ID";
		this.gridColumn12.FieldName = "RouteId";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 10;
		this.gridColumn13.Caption = "Route Order";
		this.gridColumn13.FieldName = "RouteOrder";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 11;
		this.gridColumn14.Caption = "Route Time";
		this.gridColumn14.FieldName = "RouteTime";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 12;
		this.gridColumn15.Caption = "Route Distance";
		this.gridColumn15.FieldName = "RtDistance";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.Visible = true;
		this.gridColumn15.VisibleIndex = 13;
		this.rep_btnDocNo.AutoHeight = false;
		this.rep_btnDocNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, Apzon.Forms.Properties.Resources.right, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, "", null, null, true)
		});
		this.rep_btnDocNo.Name = "rep_btnDocNo";
		this.rep_btnBp.AutoHeight = false;
		this.rep_btnBp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, Apzon.Forms.Properties.Resources.right, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, "", null, null, true)
		});
		this.rep_btnBp.Name = "rep_btnBp";
		this.rep_btnItemCode.AutoHeight = false;
		this.rep_btnItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, Apzon.Forms.Properties.Resources.right, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, "", null, null, true)
		});
		this.rep_btnItemCode.Name = "rep_btnItemCode";
		this.rep_btnWhsCode.AutoHeight = false;
		this.rep_btnWhsCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, Apzon.Forms.Properties.Resources.right, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, "", null, null, true)
		});
		this.rep_btnWhsCode.Name = "rep_btnWhsCode";
		this.rep_dateDoc.AutoHeight = false;
		this.rep_dateDoc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateDoc.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateDoc.Name = "rep_dateDoc";
		this.rep_dateDe.AutoHeight = false;
		this.rep_dateDe.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateDe.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_dateDe.Name = "rep_dateDe";
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.repositoryItemCalcEdit2.AutoHeight = false;
		this.repositoryItemCalcEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit2.Name = "repositoryItemCalcEdit2";
		this.btn_Release.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Release.Image = (System.Drawing.Image)resources.GetObject("btn_Release.Image");
		this.btn_Release.Location = new System.Drawing.Point(12, 635);
		this.btn_Release.Name = "btn_Release";
		this.btn_Release.Size = new System.Drawing.Size(75, 23);
		this.btn_Release.TabIndex = 14;
		this.btn_Release.Text = "Release ";
		this.btn_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Close.Image = (System.Drawing.Image)resources.GetObject("btn_Close.Image");
		this.btn_Close.Location = new System.Drawing.Point(93, 635);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.Size = new System.Drawing.Size(75, 23);
		this.btn_Close.TabIndex = 13;
		this.btn_Close.Text = "Close";
		this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
		this.splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainer1.Location = new System.Drawing.Point(12, 67);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Panel1.Controls.Add(this.xtraTabControl1);
		this.splitContainer1.Panel2.Controls.Add(this.xtraTabControl2);
		this.splitContainer1.Size = new System.Drawing.Size(1041, 557);
		this.splitContainer1.SplitterDistance = 278;
		this.splitContainer1.TabIndex = 15;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(1041, 278);
		this.xtraTabControl1.TabIndex = 13;
		DevExpress.XtraTab.XtraTabPageCollection tabPages = this.xtraTabControl1.TabPages;
		DevExpress.XtraTab.XtraTabPage[] pages = new Apzon.Controls.Controls.XtraTabPageEx[1] { this.xtraTabPage1 };
		tabPages.AddRange(pages);
		this.xtraTabPage1.Controls.Add(this.grd_Doc);
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.Size = new System.Drawing.Size(1035, 250);
		this.xtraTabPage1.Text = "Picked";
		this.xtraTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl2.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl2.Name = "xtraTabControl2";
		this.xtraTabControl2.SelectedTabPage = this.xtraTabPage2;
		this.xtraTabControl2.Size = new System.Drawing.Size(1041, 275);
		this.xtraTabControl2.TabIndex = 14;
		DevExpress.XtraTab.XtraTabPageCollection tabPages2 = this.xtraTabControl2.TabPages;
		pages = new Apzon.Controls.Controls.XtraTabPageEx[1] { this.xtraTabPage2 };
		tabPages2.AddRange(pages);
		this.xtraTabPage2.Controls.Add(this.grd_Line);
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.Size = new System.Drawing.Size(1035, 247);
		this.xtraTabPage2.Text = "Allocated Batch/Serial";
		this.grd_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Line.Location = new System.Drawing.Point(0, 0);
		this.grd_Line.MainView = this.gv_Line;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.repositoryItemButtonEdit1, this.repositoryItemButtonEdit2, this.repositoryItemButtonEdit3, this.repositoryItemButtonEdit4, this.repositoryItemDateEdit1, this.repositoryItemDateEdit2, this.repositoryItemCalcEdit3, this.repositoryItemCalcEdit4, this.rep_chk_Allocated });
		this.grd_Line.Size = new System.Drawing.Size(1035, 247);
		this.grd_Line.TabIndex = 12;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Line });
		DevExpress.XtraGrid.Columns.GridColumnCollection columns3 = this.gv_Line.Columns;
		columns2 = new Apzon.Controls.Grid.GridColumnEx[7] { this.gridColumn16, this.gridColumn17, this.gridColumn18, this.gridColumn19, this.gridColumn20, this.gridColumn21, this.gridColumn22 };
		columns3.AddRange(columns2);
		this.gv_Line.GridControl = this.grd_Line;
		this.gv_Line.IndicatorWidth = 50;
		this.gv_Line.Name = "gv_Line";
		this.gv_Line.OptionsDetail.DetailMode = DevExpress.XtraGrid.Views.Grid.DetailMode.Default;
		this.gv_Line.OptionsView.ColumnAutoWidth = false;
		this.gv_Line.OptionsView.ShowGroupPanel = false;
		this.gridColumn16.Caption = "Package ID";
		this.gridColumn16.FieldName = "PackageId";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn17.Caption = "Status";
		this.gridColumn17.FieldName = "Status";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.Visible = true;
		this.gridColumn17.VisibleIndex = 0;
		this.gridColumn18.Caption = "BatchNo";
		this.gridColumn18.FieldName = "BatchNo";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.Visible = true;
		this.gridColumn18.VisibleIndex = 1;
		this.gridColumn19.Caption = "Allocated";
		this.gridColumn19.ColumnEdit = this.rep_chk_Allocated;
		this.gridColumn19.FieldName = "Allocated";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.Visible = true;
		this.gridColumn19.VisibleIndex = 2;
		this.rep_chk_Allocated.AutoHeight = false;
		this.rep_chk_Allocated.DisplayValueChecked = "Y";
		this.rep_chk_Allocated.DisplayValueGrayed = "N";
		this.rep_chk_Allocated.DisplayValueUnchecked = "N";
		this.rep_chk_Allocated.Name = "rep_chk_Allocated";
		this.rep_chk_Allocated.ValueChecked = "Y";
		this.rep_chk_Allocated.ValueGrayed = "N";
		this.rep_chk_Allocated.ValueUnchecked = "N";
		this.gridColumn20.Caption = "ExpireDate";
		this.gridColumn20.FieldName = "ExpireDate";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.Visible = true;
		this.gridColumn20.VisibleIndex = 3;
		this.gridColumn21.Caption = "AdminDate";
		this.gridColumn21.FieldName = "AdminDate";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.Visible = true;
		this.gridColumn21.VisibleIndex = 4;
		this.gridColumn22.Caption = "Type";
		this.gridColumn22.FieldName = "Type";
		this.gridColumn22.Name = "gridColumn22";
		this.repositoryItemButtonEdit1.AutoHeight = false;
		this.repositoryItemButtonEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, Apzon.Forms.Properties.Resources.right, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, "", null, null, true)
		});
		this.repositoryItemButtonEdit1.Name = "repositoryItemButtonEdit1";
		this.repositoryItemButtonEdit2.AutoHeight = false;
		this.repositoryItemButtonEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, Apzon.Forms.Properties.Resources.right, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, "", null, null, true)
		});
		this.repositoryItemButtonEdit2.Name = "repositoryItemButtonEdit2";
		this.repositoryItemButtonEdit3.AutoHeight = false;
		this.repositoryItemButtonEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, Apzon.Forms.Properties.Resources.right, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, "", null, null, true)
		});
		this.repositoryItemButtonEdit3.Name = "repositoryItemButtonEdit3";
		this.repositoryItemButtonEdit4.AutoHeight = false;
		this.repositoryItemButtonEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, DevExpress.XtraEditors.ImageLocation.MiddleCenter, Apzon.Forms.Properties.Resources.right, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, "", null, null, true)
		});
		this.repositoryItemButtonEdit4.Name = "repositoryItemButtonEdit4";
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.repositoryItemDateEdit2.AutoHeight = false;
		this.repositoryItemDateEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit2.Name = "repositoryItemDateEdit2";
		this.repositoryItemCalcEdit3.AutoHeight = false;
		this.repositoryItemCalcEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit3.Name = "repositoryItemCalcEdit3";
		this.repositoryItemCalcEdit4.AutoHeight = false;
		this.repositoryItemCalcEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit4.Name = "repositoryItemCalcEdit4";
		this.txt_docNo.Location = new System.Drawing.Point(231, 34);
		this.txt_docNo.Name = "txt_docNo";
		this.txt_docNo.Size = new System.Drawing.Size(100, 20);
		this.txt_docNo.TabIndex = 24;
		this.btn_search.Image = (System.Drawing.Image)resources.GetObject("btn_search.Image");
		this.btn_search.Location = new System.Drawing.Point(346, 33);
		this.btn_search.Name = "btn_search";
		this.btn_search.Size = new System.Drawing.Size(75, 23);
		this.btn_search.TabIndex = 23;
		this.btn_search.Text = "Search";
		this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
		this.txt_BpCode.Location = new System.Drawing.Point(70, 35);
		this.txt_BpCode.Name = "txt_BpCode";
		this.txt_BpCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_BpCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_BpCode_Properties_ButtonClick);
		this.txt_BpCode.Size = new System.Drawing.Size(100, 20);
		this.txt_BpCode.TabIndex = 22;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.Location = new System.Drawing.Point(231, 9);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Size = new System.Drawing.Size(100, 20);
		this.date_ToDate.TabIndex = 21;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.Location = new System.Drawing.Point(70, 9);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Size = new System.Drawing.Size(100, 20);
		this.date_FromDate.TabIndex = 20;
		this.labelLine3.Location = new System.Drawing.Point(177, 37);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.TabIndex = 19;
		this.labelLine3.Utext = "Doc No";
		this.labelLine4.Location = new System.Drawing.Point(12, 38);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.TabIndex = 18;
		this.labelLine4.Utext = "Bp Code";
		this.labelLine2.Location = new System.Drawing.Point(177, 11);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.TabIndex = 17;
		this.labelLine2.Utext = "To Date";
		this.labelLine1.Location = new System.Drawing.Point(12, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.TabIndex = 16;
		this.labelLine1.Utext = "From Date";
		this.btn_PickList.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_PickList.Location = new System.Drawing.Point(916, 635);
		this.btn_PickList.Name = "btn_PickList";
		this.btn_PickList.Size = new System.Drawing.Size(137, 23);
		this.btn_PickList.TabIndex = 25;
		this.btn_PickList.Text = "Return to Picking List";
		this.btn_PickList.Click += new System.EventHandler(this.btn_PickList_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1065, 669);
		base.Controls.Add(this.btn_PickList);
		base.Controls.Add(this.txt_docNo);
		base.Controls.Add(this.btn_search);
		base.Controls.Add(this.txt_BpCode);
		base.Controls.Add(this.date_ToDate);
		base.Controls.Add(this.date_FromDate);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.splitContainer1);
		base.Controls.Add(this.btn_Release);
		base.Controls.Add(this.btn_Close);
		base.Name = "frm_ShippingDocumentPicked";
		base.ShowIcon = false;
		this.Text = "Picked & Route Planning";
		base.Load += new System.EventHandler(this.frm_ShippingDocumentPicked_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnDocNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnBp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_btnWhsCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateDoc.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateDe.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_dateDe).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit2).EndInit();
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).EndInit();
		this.xtraTabControl2.ResumeLayout(false);
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Allocated).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_docNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
