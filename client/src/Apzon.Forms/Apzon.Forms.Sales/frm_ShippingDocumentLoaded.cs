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

namespace Apzon.Forms.Sales;

public class frm_ShippingDocumentLoaded : XtraFormEx
{
	private IContainer components;

	private SimpleButtonEx btn_Loaded;

	private SimpleButtonEx btn_Close;

	private TextEditEx txt_docNo;

	private SimpleButtonEx btn_search;

	private ButtonEditEx txt_BpCode;

	private DateEditEx date_ToDate;

	private DateEditEx date_FromDate;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LabelLine labelLine2;

	private LabelLine labelLine1;

	private SimpleButtonEx btn_Delivery;

	private GridControlEx grd_Loaded;

	private GridViewEx gv_Loaded;

	private RepositoryItemButtonEdit rep_btnDocNo;

	private RepositoryItemButtonEdit rep_btnBp;

	private RepositoryItemButtonEdit rep_btnItemCode;

	private RepositoryItemButtonEdit rep_btnWhsCode;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private RepositoryItemCalcEdit repositoryItemCalcEdit2;

	private RepositoryItemDateEdit rep_dateDoc;

	private RepositoryItemDateEdit rep_dateDe;

	private SimpleButtonEx btn_Unloaded;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private GridColumnEx gridColumn7;

	private RepositoryItemCheckEdit rep_chk_Confirm;

	public frm_ShippingDocumentLoaded()
	{
		InitializeComponent();
	}

	private void frm_ShippingDocumentLoaded_Load(object sender, EventArgs e)
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
			dataTable.Columns.Add(new DataColumn("GateNo", typeof(string)));
			dataTable.Columns.Add(new DataColumn("VehicleId", typeof(string)));
			dataTable.Columns.Add(new DataColumn("ItemCode", typeof(string)));
			dataTable.Columns.Add(new DataColumn("CardCode", typeof(string)));
			dataTable.Columns.Add(new DataColumn("CardName", typeof(string)));
			dataTable.Columns.Add(new DataColumn("Confirm", typeof(string)));
			dataTable.Columns.Add(new DataColumn("PackageId", typeof(string)));
			dataTable.AcceptChanges();
			DataRow dataRow = dataTable.NewRow();
			dataRow["GateNo"] = "1";
			dataRow["Confirm"] = "Y";
			dataRow["VehicleId"] = "abc123";
			dataRow["ItemCode"] = "I0000001";
			dataRow["CardCode"] = "C0000001";
			dataRow["CardName"] = "Customer C0000001";
			dataRow["PackageId"] = "222";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["GateNo"] = "2";
			dataRow["Confirm"] = "N";
			dataRow2["VehicleId"] = "abc125";
			dataRow2["ItemCode"] = "I0000001";
			dataRow2["CardCode"] = "C0000001";
			dataRow2["CardName"] = "Customer C0000001";
			dataRow2["PackageId"] = "111";
			dataTable.Rows.Add(dataRow2);
			dataTable.AcceptChanges();
			grd_Loaded.DataSource = dataTable;
			grd_Loaded.RefreshDataSource();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_ShippingDocumentLoaded));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.btn_Loaded = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Close = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_docNo = new Apzon.Controls.Editor.TextEditEx();
		this.btn_search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_BpCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Delivery = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Loaded = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Loaded = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_chk_Confirm = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.rep_btnDocNo = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btnBp = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btnItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_btnWhsCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_dateDoc = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.rep_dateDe = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.repositoryItemCalcEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.btn_Unloaded = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txt_docNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Loaded).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Loaded).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Confirm).BeginInit();
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
		base.SuspendLayout();
		this.btn_Loaded.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Loaded.Image = (System.Drawing.Image)resources.GetObject("btn_Loaded.Image");
		this.btn_Loaded.Location = new System.Drawing.Point(12, 635);
		this.btn_Loaded.Name = "btn_Loaded";
		this.btn_Loaded.Size = new System.Drawing.Size(75, 23);
		this.btn_Loaded.TabIndex = 14;
		this.btn_Loaded.Text = "Loaded";
		this.btn_Close.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Close.Image = (System.Drawing.Image)resources.GetObject("btn_Close.Image");
		this.btn_Close.Location = new System.Drawing.Point(194, 635);
		this.btn_Close.Name = "btn_Close";
		this.btn_Close.Size = new System.Drawing.Size(75, 23);
		this.btn_Close.TabIndex = 13;
		this.btn_Close.Text = "Close";
		this.btn_Close.Click += new System.EventHandler(this.btn_Close_Click);
		this.txt_docNo.Location = new System.Drawing.Point(142, 601);
		this.txt_docNo.Name = "txt_docNo";
		this.txt_docNo.Size = new System.Drawing.Size(176, 20);
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
		this.labelLine3.Location = new System.Drawing.Point(12, 604);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.Size = new System.Drawing.Size(140, 20);
		this.labelLine3.TabIndex = 19;
		this.labelLine3.Utext = "Scan barcode to confirm";
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
		this.btn_Delivery.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Delivery.Location = new System.Drawing.Point(916, 635);
		this.btn_Delivery.Name = "btn_Delivery";
		this.btn_Delivery.Size = new System.Drawing.Size(137, 23);
		this.btn_Delivery.TabIndex = 25;
		this.btn_Delivery.Text = "Delivery";
		this.btn_Delivery.Click += new System.EventHandler(this.btn_PickList_Click);
		this.grd_Loaded.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Loaded.Location = new System.Drawing.Point(12, 64);
		this.grd_Loaded.MainView = this.gv_Loaded;
		this.grd_Loaded.Name = "grd_Loaded";
		this.grd_Loaded.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[9] { this.rep_btnDocNo, this.rep_btnBp, this.rep_btnItemCode, this.rep_btnWhsCode, this.rep_dateDoc, this.rep_dateDe, this.repositoryItemCalcEdit1, this.repositoryItemCalcEdit2, this.rep_chk_Confirm });
		this.grd_Loaded.Size = new System.Drawing.Size(1041, 529);
		this.grd_Loaded.TabIndex = 26;
		this.grd_Loaded.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Loaded });
		DevExpress.XtraGrid.Columns.GridColumnCollection columns = this.gv_Loaded.Columns;
		DevExpress.XtraGrid.Columns.GridColumn[] columns2 = new Apzon.Controls.Grid.GridColumnEx[7] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7 };
		columns.AddRange(columns2);
		this.gv_Loaded.GridControl = this.grd_Loaded;
		this.gv_Loaded.IndicatorWidth = 50;
		this.gv_Loaded.Name = "gv_Loaded";
		this.gv_Loaded.OptionsDetail.DetailMode = DevExpress.XtraGrid.Views.Grid.DetailMode.Default;
		this.gv_Loaded.OptionsView.ColumnAutoWidth = false;
		this.gv_Loaded.OptionsView.ShowGroupPanel = false;
		this.gridColumn1.Caption = "Gate No";
		this.gridColumn1.FieldName = "GateNo";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn2.Caption = "Vehicle ID";
		this.gridColumn2.FieldName = "VehicleId";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn3.Caption = "Package ID";
		this.gridColumn3.FieldName = "PackageId";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn4.Caption = "Item Code";
		this.gridColumn4.FieldName = "ItemCode";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn5.Caption = "BP code";
		this.gridColumn5.FieldName = "CardCode";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 4;
		this.gridColumn6.Caption = "BP Name";
		this.gridColumn6.FieldName = "CardName";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 5;
		this.gridColumn7.Caption = "Confirm";
		this.gridColumn7.ColumnEdit = this.rep_chk_Confirm;
		this.gridColumn7.FieldName = "Confirm";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 6;
		this.rep_chk_Confirm.AutoHeight = false;
		this.rep_chk_Confirm.DisplayValueChecked = "Y";
		this.rep_chk_Confirm.DisplayValueGrayed = "N";
		this.rep_chk_Confirm.DisplayValueUnchecked = "N";
		this.rep_chk_Confirm.Name = "rep_chk_Confirm";
		this.rep_chk_Confirm.ValueChecked = "Y";
		this.rep_chk_Confirm.ValueGrayed = "N";
		this.rep_chk_Confirm.ValueUnchecked = "Y";
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
		this.btn_Unloaded.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Unloaded.Image = (System.Drawing.Image)resources.GetObject("btn_Unloaded.Image");
		this.btn_Unloaded.Location = new System.Drawing.Point(105, 635);
		this.btn_Unloaded.Name = "btn_Unloaded";
		this.btn_Unloaded.Size = new System.Drawing.Size(75, 23);
		this.btn_Unloaded.TabIndex = 27;
		this.btn_Unloaded.Text = "Unloaded";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1065, 669);
		base.Controls.Add(this.btn_Unloaded);
		base.Controls.Add(this.grd_Loaded);
		base.Controls.Add(this.btn_Delivery);
		base.Controls.Add(this.txt_docNo);
		base.Controls.Add(this.btn_search);
		base.Controls.Add(this.txt_BpCode);
		base.Controls.Add(this.date_ToDate);
		base.Controls.Add(this.date_FromDate);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_Loaded);
		base.Controls.Add(this.btn_Close);
		base.Name = "frm_ShippingDocumentLoaded";
		base.ShowIcon = false;
		this.Text = "Loaded";
		base.Load += new System.EventHandler(this.frm_ShippingDocumentLoaded_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_docNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_BpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Loaded).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Loaded).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_chk_Confirm).EndInit();
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
		base.ResumeLayout(false);
	}
}
