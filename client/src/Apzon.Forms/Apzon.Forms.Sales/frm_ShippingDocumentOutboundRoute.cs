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
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Sales;

public class frm_ShippingDocumentOutboundRoute : XtraFormEx
{
	private IContainer components;

	private SimpleButtonEx btn_Release;

	private SimpleButtonEx btn_Closed;

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

	private GridControlEx grd_Route;

	private GridViewEx gv_Route;

	private RepositoryItemButtonEdit rep_btnDocNo;

	private RepositoryItemButtonEdit rep_btnBp;

	private RepositoryItemButtonEdit rep_btnItemCode;

	private RepositoryItemButtonEdit rep_btnWhsCode;

	private RepositoryItemDateEdit rep_dateDoc;

	private RepositoryItemDateEdit rep_dateDe;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2;

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

	private RepositoryItemLookUpEdit rep_luk_Status;

	private TextEditEx textEdit1;

	private LabelLine labelLine5;

	public frm_ShippingDocumentOutboundRoute()
	{
		InitializeComponent();
	}

	private void txt_BpCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
	}

	private void btn_Close_Click(object sender, EventArgs e)
	{
	}

	private void btn_PickList_Click(object sender, EventArgs e)
	{
	}

	private void btn_search_Click(object sender, EventArgs e)
	{
		Search();
	}

	private void frm_ShippingDocumentPicked_Load(object sender, EventArgs e)
	{
		try
		{
			LoadComboBox();
			Search();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboBox()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("Code", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "S";
			dataRow["Name"] = "Shipped";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["Code"] = "O";
			dataRow2["Name"] = "Open";
			dataTable.Rows.Add(dataRow2);
			dataTable.AcceptChanges();
			rep_luk_Status.Columns.Clear();
			rep_luk_Status.NullText = "";
			rep_luk_Status.Columns.Add(new LookUpColumnInfo("Name", "Name"));
			rep_luk_Status.DisplayMember = "Name";
			rep_luk_Status.ValueMember = "Code";
			rep_luk_Status.DataSource = dataTable;
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
			dataTable.Columns.Add(new DataColumn("RouteId", typeof(string)));
			dataTable.Columns.Add(new DataColumn("RouteDate", typeof(string)));
			dataTable.Columns.Add(new DataColumn("RouteTime", typeof(string)));
			dataTable.Columns.Add(new DataColumn("RouteOrder", typeof(string)));
			dataTable.Columns.Add(new DataColumn("CardCode", typeof(string)));
			dataTable.Columns.Add(new DataColumn("CardName", typeof(string)));
			dataTable.Columns.Add(new DataColumn("VehicleId", typeof(string)));
			dataTable.Columns.Add(new DataColumn("PIC", typeof(string)));
			dataTable.Columns.Add(new DataColumn("PackageId", typeof(string)));
			dataTable.Columns.Add(new DataColumn("ItemCode", typeof(string)));
			dataTable.Columns.Add(new DataColumn("BatchNo", typeof(string)));
			dataTable.Columns.Add(new DataColumn("ShipDate", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Status", typeof(string)));
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			dataRow["RouteId"] = "1";
			dataRow["RouteDate"] = "08-05-2017";
			dataRow["RouteTime"] = "11:00";
			dataRow["RouteOrder"] = "1";
			dataRow["CardCode"] = "C0000001";
			dataRow["CardName"] = "Customer C0000001";
			dataRow["VehicleId"] = "A123";
			dataRow["PIC"] = "123";
			dataRow["PackageId"] = "32";
			dataRow["ItemCode"] = "I0000001";
			dataRow["BatchNo"] = "B000001";
			dataRow["ShipDate"] = "08-05-2017";
			dataRow["Status"] = "O";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["RouteId"] = "2";
			dataRow2["RouteDate"] = "08-05-2017";
			dataRow2["RouteTime"] = "22:15";
			dataRow2["RouteOrder"] = "3";
			dataRow2["CardCode"] = "C0000002";
			dataRow2["CardName"] = "Customer C0000002";
			dataRow2["VehicleId"] = "D456";
			dataRow2["PIC"] = "123";
			dataRow2["PackageId"] = "12";
			dataRow2["ItemCode"] = "I0000001";
			dataRow2["BatchNo"] = "B000001";
			dataRow2["ShipDate"] = "09-05-2017";
			dataRow2["Status"] = "S";
			dataTable.Rows.Add(dataRow2);
			dataTable.AcceptChanges();
			grd_Route.DataSource = dataTable;
			grd_Route.RefreshDataSource();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_ShippingDocumentOutboundRoute));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.btn_Release = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Closed = new Apzon.Controls.Controls.SimpleButtonEx();
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
		this.grd_Route = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Route = new Apzon.Controls.Grid.GridViewEx();
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
		this.rep_luk_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_btnDocNo = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btnBp = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btnItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btnWhsCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_dateDoc = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.rep_dateDe = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.textEdit1 = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.txt_docNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Route).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Route).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Status).BeginInit();
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
		((System.ComponentModel.ISupportInitialize)this.textEdit1.Properties).BeginInit();
		base.SuspendLayout();
		this.btn_Release.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Release.EditControl = null;
		this.btn_Release.EditValue = "Release ";
		this.btn_Release.FieldName = null;
		this.btn_Release.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Release.ImageOptions.Image");
		this.btn_Release.IsSearchControl = false;
		this.btn_Release.Location = new System.Drawing.Point(12, 635);
		this.btn_Release.Name = "btn_Release";
		this.btn_Release.ReadOnly = false;
		this.btn_Release.SettingInVisible = false;
		this.btn_Release.SettingReadonly = false;
		this.btn_Release.Size = new System.Drawing.Size(75, 23);
		this.btn_Release.TabIndex = 14;
		this.btn_Release.Text = "Release ";
		this.btn_Closed.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Closed.EditControl = null;
		this.btn_Closed.EditValue = "Closed";
		this.btn_Closed.FieldName = null;
		this.btn_Closed.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Closed.ImageOptions.Image");
		this.btn_Closed.IsSearchControl = false;
		this.btn_Closed.Location = new System.Drawing.Point(93, 635);
		this.btn_Closed.Name = "btn_Closed";
		this.btn_Closed.ReadOnly = false;
		this.btn_Closed.SettingInVisible = false;
		this.btn_Closed.SettingReadonly = false;
		this.btn_Closed.Size = new System.Drawing.Size(75, 23);
		this.btn_Closed.TabIndex = 13;
		this.btn_Closed.Text = "Closed";
		this.btn_Closed.Click += new System.EventHandler(this.btn_Close_Click);
		this.txt_docNo.EditControl = null;
		this.txt_docNo.FieldName = null;
		this.txt_docNo.IsSearchControl = false;
		this.txt_docNo.Location = new System.Drawing.Point(231, 34);
		this.txt_docNo.Name = "txt_docNo";
		this.txt_docNo.SettingInVisible = false;
		this.txt_docNo.SettingReadonly = false;
		this.txt_docNo.Size = new System.Drawing.Size(100, 20);
		this.txt_docNo.TabIndex = 24;
		this.btn_search.EditControl = null;
		this.btn_search.EditValue = "Search";
		this.btn_search.FieldName = null;
		this.btn_search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_search.ImageOptions.Image");
		this.btn_search.IsSearchControl = false;
		this.btn_search.Location = new System.Drawing.Point(346, 33);
		this.btn_search.Name = "btn_search";
		this.btn_search.ReadOnly = false;
		this.btn_search.SettingInVisible = false;
		this.btn_search.SettingReadonly = false;
		this.btn_search.Size = new System.Drawing.Size(75, 23);
		this.btn_search.TabIndex = 23;
		this.btn_search.Text = "Search";
		this.btn_search.Click += new System.EventHandler(this.btn_search_Click);
		this.txt_BpCode.EditControl = null;
		this.txt_BpCode.FieldName = null;
		this.txt_BpCode.IsSearchControl = false;
		this.txt_BpCode.Location = new System.Drawing.Point(70, 35);
		this.txt_BpCode.Name = "txt_BpCode";
		this.txt_BpCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_BpCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_BpCode_Properties_ButtonClick);
		this.txt_BpCode.SettingInVisible = false;
		this.txt_BpCode.SettingReadonly = false;
		this.txt_BpCode.Size = new System.Drawing.Size(100, 20);
		this.txt_BpCode.TabIndex = 22;
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
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
		this.date_ToDate.Properties.IsSearchControl = false;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(100, 20);
		this.date_ToDate.TabIndex = 21;
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = null;
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
		this.date_FromDate.Properties.IsSearchControl = false;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(100, 20);
		this.date_FromDate.TabIndex = 20;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Doc No";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(177, 37);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 19;
		this.labelLine3.Utext = "Doc No";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Bp Code";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(12, 38);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 18;
		this.labelLine4.Utext = "Bp Code";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(177, 11);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 17;
		this.labelLine2.Utext = "To Date";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 16;
		this.labelLine1.Utext = "From Date";
		this.btn_PickList.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_PickList.EditControl = null;
		this.btn_PickList.EditValue = "Return to Picking List";
		this.btn_PickList.FieldName = null;
		this.btn_PickList.IsSearchControl = false;
		this.btn_PickList.Location = new System.Drawing.Point(916, 635);
		this.btn_PickList.Name = "btn_PickList";
		this.btn_PickList.ReadOnly = false;
		this.btn_PickList.SettingInVisible = false;
		this.btn_PickList.SettingReadonly = false;
		this.btn_PickList.Size = new System.Drawing.Size(137, 23);
		this.btn_PickList.TabIndex = 25;
		this.btn_PickList.Text = "Return to Picking List";
		this.btn_PickList.Click += new System.EventHandler(this.btn_PickList_Click);
		this.grd_Route.AllowPasteExcel = true;
		this.grd_Route.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Route.ClipboardData = null;
		this.grd_Route.Location = new System.Drawing.Point(12, 64);
		this.grd_Route.MainView = this.gv_Route;
		this.grd_Route.Name = "grd_Route";
		this.grd_Route.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.rep_btnDocNo, this.rep_btnBp, this.rep_btnItemCode, this.rep_btnWhsCode, this.rep_dateDoc, this.rep_dateDe, this.repositoryItemCalcEdit1, this.repositoryItemCalcEdit2, this.rep_luk_Status });
		this.grd_Route.Size = new System.Drawing.Size(1041, 526);
		this.grd_Route.TabIndex = 26;
		this.grd_Route.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Route });
		this.gv_Route.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Route.ColumnKey = null;
		this.gv_Route.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumn11, this.gridColumn12, this.gridColumn13
		});
		this.gv_Route.DisableFormSetting = false;
		this.gv_Route.DisplayException = false;
		this.gv_Route.EditColumn = null;
		this.gv_Route.FieldCurrency = null;
		this.gv_Route.FieldRate = null;
		this.gv_Route.FormId = null;
		this.gv_Route.GridControl = this.grd_Route;
		this.gv_Route.IgnoreFilter = false;
		this.gv_Route.IndexEditRow = -1;
		this.gv_Route.IndicatorWidth = 50;
		this.gv_Route.IsApzTable = false;
		this.gv_Route.IsInsertNewRow = false;
		this.gv_Route.IsLoadingData = false;
		this.gv_Route.IsMappingContextAction = false;
		this.gv_Route.ItemId = null;
		this.gv_Route.LstColumnAllowBatch = null;
		this.gv_Route.LstColumnNotAllowBatch = null;
		this.gv_Route.LstColumnNotEditable = null;
		this.gv_Route.LstColumNotSetting = null;
		this.gv_Route.Name = "gv_Route";
		this.gv_Route.OptionsView.ColumnAutoWidth = false;
		this.gv_Route.OptionsView.ShowGroupPanel = false;
		this.gv_Route.UseBatchMod = false;
		this.gridColumn1.Caption = "Route ID";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "RouteId";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 12;
		this.gridColumn2.Caption = "Route Date";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "RouteDate";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn3.Caption = "Route Time";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "RouteTime";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn4.Caption = "Route Order";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "RouteOrder";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 2;
		this.gridColumn5.Caption = "BP Code";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "CardCode";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 3;
		this.gridColumn6.Caption = "BP Name";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "CardName";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 4;
		this.gridColumn7.Caption = "Vehicle ID";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "VehicleId";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 5;
		this.gridColumn8.Caption = "P.I.C";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "PIC";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 6;
		this.gridColumn9.Caption = "Package ID";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "PackageId";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 7;
		this.gridColumn10.Caption = "ItemCode";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "ItemCode";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 8;
		this.gridColumn11.Caption = "Batch No";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "BatchNo";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 9;
		this.gridColumn12.Caption = "Ship Date";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "ShipDate";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn12.Visible = true;
		this.gridColumn12.VisibleIndex = 10;
		this.gridColumn13.Caption = "Status";
		this.gridColumn13.ColumnEdit = this.rep_luk_Status;
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "Status";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn13.Visible = true;
		this.gridColumn13.VisibleIndex = 11;
		this.rep_luk_Status.AutoHeight = false;
		this.rep_luk_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_luk_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_luk_Status.Name = "rep_luk_Status";
		this.rep_luk_Status.NullText = "";
		this.rep_btnDocNo.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_btnDocNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_btnDocNo.Name = "rep_btnDocNo";
		this.rep_btnBp.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_btnBp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_btnBp.Name = "rep_btnBp";
		this.rep_btnItemCode.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_btnItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_btnItemCode.Name = "rep_btnItemCode";
		this.rep_btnWhsCode.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_btnWhsCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
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
		this.textEdit1.EditControl = null;
		this.textEdit1.FieldName = null;
		this.textEdit1.IsSearchControl = false;
		this.textEdit1.Location = new System.Drawing.Point(142, 602);
		this.textEdit1.Name = "textEdit1";
		this.textEdit1.SettingInVisible = false;
		this.textEdit1.SettingReadonly = false;
		this.textEdit1.Size = new System.Drawing.Size(176, 20);
		this.textEdit1.TabIndex = 28;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Scan barcode to confirm";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(12, 605);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(140, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 27;
		this.labelLine5.Utext = "Scan barcode to confirm";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1065, 669);
		base.Controls.Add(this.textEdit1);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.grd_Route);
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
		base.Controls.Add(this.btn_Release);
		base.Controls.Add(this.btn_Closed);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ShippingDocumentOutboundRoute";
		this.Text = "Outbound Route";
		base.Load += new System.EventHandler(this.frm_ShippingDocumentPicked_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_docNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Route).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Route).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_luk_Status).EndInit();
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
		((System.ComponentModel.ISupportInitialize)this.textEdit1.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
