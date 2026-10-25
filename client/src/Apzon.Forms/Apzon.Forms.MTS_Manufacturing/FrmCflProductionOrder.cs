using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.PlanningProduction;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.MTS_Manufacturing;

public class FrmCflProductionOrder : XtraFormEx
{
	private IHttpClientHelper<Mrp> _mrpClientHelper = new HttpClientHelper<Mrp>(Configuration.ApiServer, "Mrp");

	public List<Common.ListItems> ChooseItems = new List<Common.ListItems>();

	private string strSearch = string.Empty;

	private IContainer components;

	private GridControlEx grd_ProductionOrder;

	private GridViewEx gv_ProductionOrder;

	private GridColumnEx cl_DocEntry;

	private GridColumnEx cl_ItemCode;

	private GridColumnEx cl_ItemName;

	private GridColumnEx cl_Type;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit_Type;

	private GridColumnEx cl_PlannedQty;

	private GridColumnEx cl_CmpltQty;

	private GridColumnEx cl_RjctQty;

	private GridColumnEx cl_DueDate;

	private GridColumnEx cl_CreateDate;

	private ButtonEditEx txt_Search;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnChoose;

	private LabelLine labelLine1;

	public FrmCflProductionOrder(string itemCode)
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmCflProductionOrder"))
		{
			strSearch = itemCode;
			InitializeComponent();
			ChooseItems.Clear();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "FrmCflProductionOrder" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void FrmCflProductionOrder_Load(object sender, EventArgs e)
	{
		try
		{
			List<Common.ListItems> pOType = GetPOType();
			repositoryItemLookUpEdit_Type.DataSource = pOType;
			repositoryItemLookUpEdit_Type.DisplayMember = "Text";
			repositoryItemLookUpEdit_Type.ValueMember = "Value";
			DataTable dataSource = await GetProductionOrder();
			grd_ProductionOrder.DataSource = dataSource;
			grd_ProductionOrder.RefreshDataSource();
			if (!string.IsNullOrEmpty(strSearch))
			{
				txt_Search.Text = strSearch;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetProductionOrder()
	{
		try
		{
			return await _mrpClientHelper.GetDataTableAsync("GetProductionOrder");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return new DataTable();
	}

	public static List<Common.ListItems> GetPOType()
	{
		return new List<Common.ListItems>
		{
			new Common.ListItems
			{
				Value = "S",
				Text = "Standard"
			},
			new Common.ListItems
			{
				Value = "P",
				Text = "Special"
			},
			new Common.ListItems
			{
				Value = "D",
				Text = "Disassembly"
			}
		};
	}

	private void txt_Search_EditValueChanged(object sender, EventArgs e)
	{
		string[] array = txt_Search.Text.Split(',');
		if (array.Length > 1)
		{
			string empty = string.Empty;
			empty = empty + "[ItemCode] LIKE '%" + array[0] + "%'";
			for (int i = 1; i < array.Length; i++)
			{
				empty = empty + " OR [ItemCode] LIKE '%" + array[i] + "%'";
			}
			gv_ProductionOrder.ActiveFilterString = empty;
		}
		else
		{
			gv_ProductionOrder.ActiveFilterString = "[ItemCode] LIKE '%" + txt_Search.Text + "%'";
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnChoose_Click(object sender, EventArgs e)
	{
		AddChooseItemsGrid();
	}

	private void AddChooseItemsGrid()
	{
		try
		{
			if (gv_ProductionOrder.GetSelectedRows() != null && gv_ProductionOrder.GetSelectedRows().Length != 0)
			{
				int[] selectedRows = gv_ProductionOrder.GetSelectedRows();
				foreach (int rowHandle in selectedRows)
				{
					Common.ListItems listItems = new Common.ListItems();
					listItems.Value = ((gv_ProductionOrder.GetRowCellValue(rowHandle, cl_DocEntry) == DBNull.Value) ? string.Empty : gv_ProductionOrder.GetRowCellValue(rowHandle, cl_DocEntry).ToString());
					listItems.Text = ((gv_ProductionOrder.GetRowCellValue(rowHandle, cl_ItemCode) == DBNull.Value) ? string.Empty : gv_ProductionOrder.GetRowCellValue(rowHandle, cl_ItemCode).ToString());
					ChooseItems.Add(listItems);
					Close();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_ProductionOrder_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_ProductionOrder_DoubleClick(object sender, EventArgs e)
	{
		AddChooseItemsGrid();
	}

	private void gv_ProductionOrder_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			AddChooseItemsGrid();
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.MTS_Manufacturing.FrmCflProductionOrder));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_ProductionOrder = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ProductionOrder = new Apzon.Controls.Grid.GridViewEx();
		this.cl_DocEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Type = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemLookUpEdit_Type = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.cl_PlannedQty = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_CmpltQty = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_RjctQty = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_DueDate = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_CreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.txt_Search = new Apzon.Controls.Editor.ButtonEditEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnChoose = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.grd_ProductionOrder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ProductionOrder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit_Type).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_ProductionOrder.AllowPasteExcel = true;
		this.grd_ProductionOrder.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_ProductionOrder.ClipboardData = null;
		this.grd_ProductionOrder.Location = new System.Drawing.Point(0, 32);
		this.grd_ProductionOrder.MainView = this.gv_ProductionOrder;
		this.grd_ProductionOrder.Name = "grd_ProductionOrder";
		this.grd_ProductionOrder.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.repositoryItemLookUpEdit_Type });
		this.grd_ProductionOrder.Size = new System.Drawing.Size(590, 305);
		this.grd_ProductionOrder.TabIndex = 5;
		this.grd_ProductionOrder.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ProductionOrder });
		this.gv_ProductionOrder.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ProductionOrder.ColumnKey = null;
		this.gv_ProductionOrder.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.cl_DocEntry, this.cl_ItemCode, this.cl_ItemName, this.cl_Type, this.cl_PlannedQty, this.cl_CmpltQty, this.cl_RjctQty, this.cl_DueDate, this.cl_CreateDate });
		this.gv_ProductionOrder.DisableFormSetting = false;
		this.gv_ProductionOrder.DisplayException = false;
		this.gv_ProductionOrder.EditColumn = null;
		this.gv_ProductionOrder.FieldCurrency = null;
		this.gv_ProductionOrder.FieldRate = null;
		this.gv_ProductionOrder.FormId = null;
		this.gv_ProductionOrder.GridControl = this.grd_ProductionOrder;
		this.gv_ProductionOrder.IgnoreFilter = false;
		this.gv_ProductionOrder.IndexEditRow = -1;
		this.gv_ProductionOrder.IndicatorWidth = 30;
		this.gv_ProductionOrder.IsApzTable = false;
		this.gv_ProductionOrder.IsInsertNewRow = false;
		this.gv_ProductionOrder.IsLoadingData = false;
		this.gv_ProductionOrder.IsMappingContextAction = false;
		this.gv_ProductionOrder.ItemId = null;
		this.gv_ProductionOrder.LstColumnAllowBatch = null;
		this.gv_ProductionOrder.LstColumnNotAllowBatch = null;
		this.gv_ProductionOrder.LstColumnNotEditable = null;
		this.gv_ProductionOrder.LstColumNotSetting = null;
		this.gv_ProductionOrder.Name = "gv_ProductionOrder";
		this.gv_ProductionOrder.OptionsBehavior.Editable = false;
		this.gv_ProductionOrder.OptionsSelection.MultiSelect = true;
		this.gv_ProductionOrder.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowForFocusedRow;
		this.gv_ProductionOrder.OptionsView.ShowFilterPanelMode = DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.Never;
		this.gv_ProductionOrder.OptionsView.ShowGroupPanel = false;
		this.gv_ProductionOrder.UseBatchMod = false;
		this.gv_ProductionOrder.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_ProductionOrder_CustomDrawRowIndicator);
		this.gv_ProductionOrder.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_ProductionOrder_KeyDown);
		this.gv_ProductionOrder.DoubleClick += new System.EventHandler(this.gv_ProductionOrder_DoubleClick);
		this.cl_DocEntry.Caption = "Docentry";
		this.cl_DocEntry.CurrencyDateField = null;
		this.cl_DocEntry.CurrencyField = null;
		this.cl_DocEntry.CurrencyFixed = null;
		this.cl_DocEntry.CurrencyFixedRate = 0.0;
		this.cl_DocEntry.CurrencyRateField = null;
		this.cl_DocEntry.FieldName = "DocEntry";
		this.cl_DocEntry.Name = "cl_DocEntry";
		this.cl_DocEntry.PrimaryKey = true;
		this.cl_DocEntry.UnitField = null;
		this.cl_ItemCode.Caption = "Item code";
		this.cl_ItemCode.CurrencyDateField = null;
		this.cl_ItemCode.CurrencyField = null;
		this.cl_ItemCode.CurrencyFixed = null;
		this.cl_ItemCode.CurrencyFixedRate = 0.0;
		this.cl_ItemCode.CurrencyRateField = null;
		this.cl_ItemCode.FieldName = "ItemCode";
		this.cl_ItemCode.Name = "cl_ItemCode";
		this.cl_ItemCode.PrimaryKey = false;
		this.cl_ItemCode.UnitField = null;
		this.cl_ItemCode.Visible = true;
		this.cl_ItemCode.VisibleIndex = 0;
		this.cl_ItemCode.Width = 63;
		this.cl_ItemName.Caption = "Item Name";
		this.cl_ItemName.CurrencyDateField = null;
		this.cl_ItemName.CurrencyField = null;
		this.cl_ItemName.CurrencyFixed = null;
		this.cl_ItemName.CurrencyFixedRate = 0.0;
		this.cl_ItemName.CurrencyRateField = null;
		this.cl_ItemName.FieldName = "ItemName";
		this.cl_ItemName.Name = "cl_ItemName";
		this.cl_ItemName.PrimaryKey = false;
		this.cl_ItemName.UnitField = null;
		this.cl_ItemName.Visible = true;
		this.cl_ItemName.VisibleIndex = 1;
		this.cl_ItemName.Width = 116;
		this.cl_Type.AppearanceCell.Options.UseTextOptions = true;
		this.cl_Type.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.cl_Type.AppearanceCell.TextOptions.VAlignment = DevExpress.Utils.VertAlignment.Center;
		this.cl_Type.Caption = "Type";
		this.cl_Type.ColumnEdit = this.repositoryItemLookUpEdit_Type;
		this.cl_Type.CurrencyDateField = null;
		this.cl_Type.CurrencyField = null;
		this.cl_Type.CurrencyFixed = null;
		this.cl_Type.CurrencyFixedRate = 0.0;
		this.cl_Type.CurrencyRateField = null;
		this.cl_Type.FieldName = "Type";
		this.cl_Type.Name = "cl_Type";
		this.cl_Type.PrimaryKey = false;
		this.cl_Type.UnitField = null;
		this.cl_Type.Visible = true;
		this.cl_Type.VisibleIndex = 2;
		this.cl_Type.Width = 71;
		this.repositoryItemLookUpEdit_Type.AutoHeight = false;
		this.repositoryItemLookUpEdit_Type.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit_Type.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "Value", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Text", " ")
		});
		this.repositoryItemLookUpEdit_Type.Name = "repositoryItemLookUpEdit_Type";
		this.repositoryItemLookUpEdit_Type.NullText = "";
		this.cl_PlannedQty.Caption = "Planned quantity";
		this.cl_PlannedQty.CurrencyDateField = null;
		this.cl_PlannedQty.CurrencyField = null;
		this.cl_PlannedQty.CurrencyFixed = null;
		this.cl_PlannedQty.CurrencyFixedRate = 0.0;
		this.cl_PlannedQty.CurrencyRateField = null;
		this.cl_PlannedQty.DisplayFormat.FormatString = "n0";
		this.cl_PlannedQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.cl_PlannedQty.FieldName = "PlannedQty";
		this.cl_PlannedQty.Name = "cl_PlannedQty";
		this.cl_PlannedQty.PrimaryKey = false;
		this.cl_PlannedQty.UnitField = null;
		this.cl_PlannedQty.Visible = true;
		this.cl_PlannedQty.VisibleIndex = 3;
		this.cl_PlannedQty.Width = 87;
		this.cl_CmpltQty.Caption = "Complete quantity";
		this.cl_CmpltQty.CurrencyDateField = null;
		this.cl_CmpltQty.CurrencyField = null;
		this.cl_CmpltQty.CurrencyFixed = null;
		this.cl_CmpltQty.CurrencyFixedRate = 0.0;
		this.cl_CmpltQty.CurrencyRateField = null;
		this.cl_CmpltQty.DisplayFormat.FormatString = "n0";
		this.cl_CmpltQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.cl_CmpltQty.FieldName = "CmpltQty";
		this.cl_CmpltQty.Name = "cl_CmpltQty";
		this.cl_CmpltQty.PrimaryKey = false;
		this.cl_CmpltQty.UnitField = null;
		this.cl_CmpltQty.Visible = true;
		this.cl_CmpltQty.VisibleIndex = 4;
		this.cl_CmpltQty.Width = 87;
		this.cl_RjctQty.Caption = "Reject quantity";
		this.cl_RjctQty.CurrencyDateField = null;
		this.cl_RjctQty.CurrencyField = null;
		this.cl_RjctQty.CurrencyFixed = null;
		this.cl_RjctQty.CurrencyFixedRate = 0.0;
		this.cl_RjctQty.CurrencyRateField = null;
		this.cl_RjctQty.DisplayFormat.FormatString = "n0";
		this.cl_RjctQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.cl_RjctQty.FieldName = "RjctQty";
		this.cl_RjctQty.Name = "cl_RjctQty";
		this.cl_RjctQty.PrimaryKey = false;
		this.cl_RjctQty.UnitField = null;
		this.cl_RjctQty.Visible = true;
		this.cl_RjctQty.VisibleIndex = 5;
		this.cl_DueDate.Caption = "Due date";
		this.cl_DueDate.CurrencyDateField = null;
		this.cl_DueDate.CurrencyField = null;
		this.cl_DueDate.CurrencyFixed = null;
		this.cl_DueDate.CurrencyFixedRate = 0.0;
		this.cl_DueDate.CurrencyRateField = null;
		this.cl_DueDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.cl_DueDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_DueDate.FieldName = "DueDate";
		this.cl_DueDate.Name = "cl_DueDate";
		this.cl_DueDate.PrimaryKey = false;
		this.cl_DueDate.UnitField = null;
		this.cl_DueDate.Visible = true;
		this.cl_DueDate.VisibleIndex = 6;
		this.cl_DueDate.Width = 59;
		this.cl_CreateDate.Caption = "Create date";
		this.cl_CreateDate.CurrencyDateField = null;
		this.cl_CreateDate.CurrencyField = null;
		this.cl_CreateDate.CurrencyFixed = null;
		this.cl_CreateDate.CurrencyFixedRate = 0.0;
		this.cl_CreateDate.CurrencyRateField = null;
		this.cl_CreateDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.cl_CreateDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.cl_CreateDate.FieldName = "CreateDate";
		this.cl_CreateDate.Name = "cl_CreateDate";
		this.cl_CreateDate.PrimaryKey = false;
		this.cl_CreateDate.UnitField = null;
		this.txt_Search.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Search.EditControl = null;
		this.txt_Search.FieldName = null;
		this.txt_Search.IsSearchControl = false;
		this.txt_Search.Location = new System.Drawing.Point(94, 4);
		this.txt_Search.Name = "txt_Search";
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.txt_Search.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_Search.SettingInVisible = false;
		this.txt_Search.SettingReadonly = false;
		this.txt_Search.Size = new System.Drawing.Size(412, 24);
		this.txt_Search.TabIndex = 4;
		this.txt_Search.ToolTip = "Search by Item code";
		this.txt_Search.EditValueChanged += new System.EventHandler(this.txt_Search_EditValueChanged);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(94, 341);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 7;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnChoose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnChoose.EditControl = null;
		this.btnChoose.EditValue = "Choose";
		this.btnChoose.FieldName = null;
		this.btnChoose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnChoose.ImageOptions.Image");
		this.btnChoose.IsSearchControl = false;
		this.btnChoose.Location = new System.Drawing.Point(13, 341);
		this.btnChoose.Name = "btnChoose";
		this.btnChoose.ReadOnly = false;
		this.btnChoose.SettingInVisible = false;
		this.btnChoose.SettingReadonly = false;
		this.btnChoose.Size = new System.Drawing.Size(75, 23);
		this.btnChoose.TabIndex = 6;
		this.btnChoose.Text = "Choose";
		this.btnChoose.Click += new System.EventHandler(this.btnChoose_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Search";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 8);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 8;
		this.labelLine1.Utext = "Search";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(590, 368);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.grd_ProductionOrder);
		base.Controls.Add(this.txt_Search);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnChoose);
		base.IconOptions.ShowIcon = false;
		base.Name = "FrmCflProductionOrder";
		base.ShowInTaskbar = false;
		this.Text = "List Of Production Order";
		base.Load += new System.EventHandler(this.FrmCflProductionOrder_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_ProductionOrder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ProductionOrder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit_Type).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Search.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
