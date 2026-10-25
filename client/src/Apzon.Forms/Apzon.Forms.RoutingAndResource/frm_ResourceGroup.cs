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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.RoutingAndResource;

public class frm_ResourceGroup : XtraFormEx
{
	private const string OkString = "OK";

	private const string SaveString = "Save";

	private IContainer components;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gcResName;

	private GridColumnEx gcUnit;

	private GridColumnEx gcResType;

	private GridColumnEx gcResStdCostName1;

	private GridColumnEx gcResStdCost1;

	private RepositoryItemCalcEdit rCalEdit;

	private GridColumnEx gcResCostName2;

	private GridColumnEx gcResCost2;

	private GridColumnEx gcResCostName3;

	private GridColumnEx gcResCost3;

	private GridColumnEx gcResCostName4;

	private GridColumnEx gcResCost4;

	private GridColumnEx gcResCostName5;

	private GridColumnEx gcResCost5;

	private GridColumnEx gcResCostName6;

	private GridColumnEx gcResCost6;

	private GridColumnEx gcResCostName7;

	private GridColumnEx gcResCost7;

	private GridColumnEx gcResCostName8;

	private GridColumnEx gcResCost8;

	private GridColumnEx gcResCostName9;

	private GridColumnEx gcResCost9;

	private GridColumnEx gcResCostName10;

	private GridColumnEx gcResCost10;

	private GridColumnEx gcResGrCode;

	private RepositoryItemLookUpEdit rResType;

	private LabelLine lbSearch;

	private TextEdit txtSearch;

	private SimpleButton btnSearch;

	private SimpleButton btnCancel;

	private SimpleButton btnSave;

	private ContextMenuStrip contextMenuStripDoc;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
		}
		else
		{
			_formMode = value;
			btnSave.Text = ((value == Enums.FormViewModeType.View) ? "OK" : "Save");
		}
	}

	public frm_ResourceGroup()
	{
		InitializeComponent();
	}

	private async void frm_ResourceGroup_Load(object sender, EventArgs e)
	{
		try
		{
			LoadGroupType();
			await SearchData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Add || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
		{
			Close();
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				List<DataRow> listDataSourceSelectedRow = gvDoc.GetListDataSourceSelectedRow();
				DataTable dataSourceTable = grdDoc.DataSourceTable;
				for (int i = 0; i < listDataSourceSelectedRow.Count; i++)
				{
					dataSourceTable.Rows.Remove(listDataSourceSelectedRow[i]);
				}
				grdDoc.DataSource = dataSourceTable;
				grdDoc.DataSourceTable.AcceptChanges();
				FormMode = Enums.FormViewModeType.Add;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			await SearchData();
			if (!string.IsNullOrEmpty(Function.ToString(txtSearch.EditValue)))
			{
				string text = "";
				text += string.Format("OR {0} LIKE '%{1}%' ", "ResGrpNam", Function.ToString(txtSearch.EditValue));
				text = ((text.Length > 2) ? text.Substring(2) : "");
				gvDoc.ActiveFilterString = text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			DataTable dataTable = grdDoc.DataSourceTable.Copy();
			List<DataRow> list = (from t in dataTable.AsEnumerable()
				where Function.ParseInt(t["ResGrpCod"]) == 0 && string.IsNullOrEmpty(Function.ToString(t["ResGrpNam"]))
				select t).ToList();
			for (int num = 0; num < list.Count; num++)
			{
				dataTable.Rows.Remove(list[num]);
			}
			dataTable.AcceptChanges();
			await HttpClient.ResourceGroupClient.Save(dataTable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadGroupType()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Machine",
					Code = "M"
				},
				new Common.Table_Code
				{
					Name = "Labor",
					Code = "L"
				},
				new Common.Table_Code
				{
					Name = "Other",
					Code = "O"
				}
			};
			rResType.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SearchData()
	{
		try
		{
			gvDoc.ActiveFilterString = "";
			DataTable dataSource = await HttpClient.ResourceGroupClient.SearchData(Function.ToString(txtSearch.EditValue));
			grdDoc.DataSource = dataSource;
			gvDoc.RefreshData();
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			if (dataSourceTable.Columns.Count != 0 && !dataSourceTable.AsEnumerable().Any((DataRow t) => Function.ParseInt(t["ResGrpCod"]) == 0 && string.IsNullOrEmpty(Function.ToString(t["ResGrpNam"]))))
			{
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["ResType"] = "M";
				dataRow["CostName1"] = "Resource Std Cost 1";
				dataRow["CostName2"] = "Resource Std Cost 2";
				dataRow["CostName3"] = "Resource Std Cost 3";
				dataRow["CostName4"] = "Resource Std Cost 4";
				dataRow["CostName5"] = "Resource Std Cost 5";
				dataRow["CostName6"] = "Resource Std Cost 6";
				dataRow["CostName7"] = "Resource Std Cost 7";
				dataRow["CostName8"] = "Resource Std Cost 8";
				dataRow["CostName9"] = "Resource Std Cost 9";
				dataRow["CostName10"] = "Resource Std Cost 10";
				dataSourceTable.Rows.Add(dataRow);
				grdDoc.DataSource = dataSourceTable;
				grdDoc.DataSourceTable.AcceptChanges();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.RoutingAndResource.frm_ResourceGroup));
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcResName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResType = new Apzon.Controls.Grid.GridColumnEx();
		this.rResType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcResStdCostName1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResStdCost1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rCalEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcResCostName2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCost2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCostName3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCost3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCostName4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCost4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCostName5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCost5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCostName6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCost6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCostName7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCost7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCostName8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCost8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCostName9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCost9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCostName10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResCost10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gcResGrCode = new Apzon.Controls.Grid.GridColumnEx();
		this.lbSearch = new Apzon.Controls.Controls.LabelLine();
		this.txtSearch = new DevExpress.XtraEditors.TextEdit();
		this.btnSearch = new DevExpress.XtraEditors.SimpleButton();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		this.btnSave = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStripDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rResType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCalEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSearch.Properties).BeginInit();
		base.SuspendLayout();
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStripDoc;
		this.grdDoc.Location = new System.Drawing.Point(1, 35);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rCalEdit, this.rResType });
		this.grdDoc.Size = new System.Drawing.Size(741, 174);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStripDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStripDoc.Name = "contextMenuStripDoc";
		this.contextMenuStripDoc.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvDoc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[24]
		{
			this.gcResName, this.gcUnit, this.gcResType, this.gcResStdCostName1, this.gcResStdCost1, this.gcResCostName2, this.gcResCost2, this.gcResCostName3, this.gcResCost3, this.gcResCostName4,
			this.gcResCost4, this.gcResCostName5, this.gcResCost5, this.gcResCostName6, this.gcResCost6, this.gcResCostName7, this.gcResCost7, this.gcResCostName8, this.gcResCost8, this.gcResCostName9,
			this.gcResCost9, this.gcResCostName10, this.gcResCost10, this.gcResGrCode
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oResourceGroup";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "ORSG";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsPrint.AutoWidth = false;
		this.gvDoc.OptionsView.BestFitMaxRowCount = 100;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gcResName.Caption = "Resource Group Name";
		this.gcResName.CurrencyDateField = null;
		this.gcResName.CurrencyField = null;
		this.gcResName.CurrencyFixed = null;
		this.gcResName.CurrencyFixedRate = 0.0;
		this.gcResName.CurrencyRateField = null;
		this.gcResName.FieldName = "ResGrpNam";
		this.gcResName.Name = "gcResName";
		this.gcResName.PrimaryKey = true;
		this.gcResName.UnitField = null;
		this.gcResName.Visible = true;
		this.gcResName.VisibleIndex = 0;
		this.gcResName.Width = 121;
		this.gcUnit.Caption = "Unit of Measure Text";
		this.gcUnit.CurrencyDateField = null;
		this.gcUnit.CurrencyField = null;
		this.gcUnit.CurrencyFixed = null;
		this.gcUnit.CurrencyFixedRate = 0.0;
		this.gcUnit.CurrencyRateField = null;
		this.gcUnit.FieldName = "ResUoM";
		this.gcUnit.Name = "gcUnit";
		this.gcUnit.PrimaryKey = false;
		this.gcUnit.UnitField = null;
		this.gcUnit.Visible = true;
		this.gcUnit.VisibleIndex = 1;
		this.gcUnit.Width = 114;
		this.gcResType.Caption = "Resource Type";
		this.gcResType.ColumnEdit = this.rResType;
		this.gcResType.CurrencyDateField = null;
		this.gcResType.CurrencyField = null;
		this.gcResType.CurrencyFixed = null;
		this.gcResType.CurrencyFixedRate = 0.0;
		this.gcResType.CurrencyRateField = null;
		this.gcResType.FieldName = "ResType";
		this.gcResType.Name = "gcResType";
		this.gcResType.PrimaryKey = false;
		this.gcResType.UnitField = null;
		this.gcResType.Visible = true;
		this.gcResType.VisibleIndex = 2;
		this.gcResType.Width = 93;
		this.rResType.AutoHeight = false;
		this.rResType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rResType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rResType.DisplayMember = "Name";
		this.rResType.Name = "rResType";
		this.rResType.NullText = "";
		this.rResType.ValueMember = "Code";
		this.gcResStdCostName1.Caption = "Resource Std Cost Name 1";
		this.gcResStdCostName1.CurrencyDateField = null;
		this.gcResStdCostName1.CurrencyField = null;
		this.gcResStdCostName1.CurrencyFixed = null;
		this.gcResStdCostName1.CurrencyFixedRate = 0.0;
		this.gcResStdCostName1.CurrencyRateField = null;
		this.gcResStdCostName1.FieldName = "CostName1";
		this.gcResStdCostName1.Name = "gcResStdCostName1";
		this.gcResStdCostName1.PrimaryKey = false;
		this.gcResStdCostName1.UnitField = null;
		this.gcResStdCostName1.Visible = true;
		this.gcResStdCostName1.VisibleIndex = 3;
		this.gcResStdCostName1.Width = 140;
		this.gcResStdCost1.Caption = "Resource Std Cost 1";
		this.gcResStdCost1.ColumnEdit = this.rCalEdit;
		this.gcResStdCost1.CurrencyDateField = null;
		this.gcResStdCost1.CurrencyField = null;
		this.gcResStdCost1.CurrencyFixed = null;
		this.gcResStdCost1.CurrencyFixedRate = 0.0;
		this.gcResStdCost1.CurrencyRateField = null;
		this.gcResStdCost1.FieldName = "CostVal1";
		this.gcResStdCost1.Name = "gcResStdCost1";
		this.gcResStdCost1.PrimaryKey = false;
		this.gcResStdCost1.UnitField = null;
		this.gcResStdCost1.Visible = true;
		this.gcResStdCost1.VisibleIndex = 4;
		this.gcResStdCost1.Width = 119;
		this.rCalEdit.AutoHeight = false;
		this.rCalEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCalEdit.Name = "rCalEdit";
		this.gcResCostName2.Caption = "Resource Std Cost Name 2";
		this.gcResCostName2.CurrencyDateField = null;
		this.gcResCostName2.CurrencyField = null;
		this.gcResCostName2.CurrencyFixed = null;
		this.gcResCostName2.CurrencyFixedRate = 0.0;
		this.gcResCostName2.CurrencyRateField = null;
		this.gcResCostName2.FieldName = "CostName2";
		this.gcResCostName2.Name = "gcResCostName2";
		this.gcResCostName2.PrimaryKey = false;
		this.gcResCostName2.UnitField = null;
		this.gcResCostName2.Visible = true;
		this.gcResCostName2.VisibleIndex = 5;
		this.gcResCostName2.Width = 138;
		this.gcResCost2.Caption = "Resource Std Cost 2";
		this.gcResCost2.ColumnEdit = this.rCalEdit;
		this.gcResCost2.CurrencyDateField = null;
		this.gcResCost2.CurrencyField = null;
		this.gcResCost2.CurrencyFixed = null;
		this.gcResCost2.CurrencyFixedRate = 0.0;
		this.gcResCost2.CurrencyRateField = null;
		this.gcResCost2.FieldName = "CostVal2";
		this.gcResCost2.Name = "gcResCost2";
		this.gcResCost2.PrimaryKey = false;
		this.gcResCost2.UnitField = null;
		this.gcResCost2.Visible = true;
		this.gcResCost2.VisibleIndex = 6;
		this.gcResCost2.Width = 108;
		this.gcResCostName3.Caption = "Resource Std Cost Name 3";
		this.gcResCostName3.CurrencyDateField = null;
		this.gcResCostName3.CurrencyField = null;
		this.gcResCostName3.CurrencyFixed = null;
		this.gcResCostName3.CurrencyFixedRate = 0.0;
		this.gcResCostName3.CurrencyRateField = null;
		this.gcResCostName3.FieldName = "CostName3";
		this.gcResCostName3.Name = "gcResCostName3";
		this.gcResCostName3.PrimaryKey = false;
		this.gcResCostName3.UnitField = null;
		this.gcResCostName3.Visible = true;
		this.gcResCostName3.VisibleIndex = 7;
		this.gcResCostName3.Width = 138;
		this.gcResCost3.Caption = "Resource Std Cost 3";
		this.gcResCost3.ColumnEdit = this.rCalEdit;
		this.gcResCost3.CurrencyDateField = null;
		this.gcResCost3.CurrencyField = null;
		this.gcResCost3.CurrencyFixed = null;
		this.gcResCost3.CurrencyFixedRate = 0.0;
		this.gcResCost3.CurrencyRateField = null;
		this.gcResCost3.FieldName = "CostVal3";
		this.gcResCost3.Name = "gcResCost3";
		this.gcResCost3.PrimaryKey = false;
		this.gcResCost3.UnitField = null;
		this.gcResCost3.Visible = true;
		this.gcResCost3.VisibleIndex = 8;
		this.gcResCost3.Width = 108;
		this.gcResCostName4.Caption = "Resource Std Cost Name 4";
		this.gcResCostName4.CurrencyDateField = null;
		this.gcResCostName4.CurrencyField = null;
		this.gcResCostName4.CurrencyFixed = null;
		this.gcResCostName4.CurrencyFixedRate = 0.0;
		this.gcResCostName4.CurrencyRateField = null;
		this.gcResCostName4.FieldName = "CostName4";
		this.gcResCostName4.Name = "gcResCostName4";
		this.gcResCostName4.PrimaryKey = false;
		this.gcResCostName4.UnitField = null;
		this.gcResCostName4.Visible = true;
		this.gcResCostName4.VisibleIndex = 9;
		this.gcResCostName4.Width = 138;
		this.gcResCost4.Caption = "Resource Std Cost 4";
		this.gcResCost4.ColumnEdit = this.rCalEdit;
		this.gcResCost4.CurrencyDateField = null;
		this.gcResCost4.CurrencyField = null;
		this.gcResCost4.CurrencyFixed = null;
		this.gcResCost4.CurrencyFixedRate = 0.0;
		this.gcResCost4.CurrencyRateField = null;
		this.gcResCost4.FieldName = "CostVal4";
		this.gcResCost4.Name = "gcResCost4";
		this.gcResCost4.PrimaryKey = false;
		this.gcResCost4.UnitField = null;
		this.gcResCost4.Visible = true;
		this.gcResCost4.VisibleIndex = 10;
		this.gcResCost4.Width = 108;
		this.gcResCostName5.Caption = "Resource Std Cost Name 5";
		this.gcResCostName5.CurrencyDateField = null;
		this.gcResCostName5.CurrencyField = null;
		this.gcResCostName5.CurrencyFixed = null;
		this.gcResCostName5.CurrencyFixedRate = 0.0;
		this.gcResCostName5.CurrencyRateField = null;
		this.gcResCostName5.FieldName = "CostName5";
		this.gcResCostName5.Name = "gcResCostName5";
		this.gcResCostName5.PrimaryKey = false;
		this.gcResCostName5.UnitField = null;
		this.gcResCostName5.Visible = true;
		this.gcResCostName5.VisibleIndex = 11;
		this.gcResCostName5.Width = 138;
		this.gcResCost5.Caption = "Resource Std Cost 5";
		this.gcResCost5.ColumnEdit = this.rCalEdit;
		this.gcResCost5.CurrencyDateField = null;
		this.gcResCost5.CurrencyField = null;
		this.gcResCost5.CurrencyFixed = null;
		this.gcResCost5.CurrencyFixedRate = 0.0;
		this.gcResCost5.CurrencyRateField = null;
		this.gcResCost5.FieldName = "CostVal5";
		this.gcResCost5.Name = "gcResCost5";
		this.gcResCost5.PrimaryKey = false;
		this.gcResCost5.UnitField = null;
		this.gcResCost5.Visible = true;
		this.gcResCost5.VisibleIndex = 12;
		this.gcResCost5.Width = 108;
		this.gcResCostName6.Caption = "Resource Std Cost Name 6";
		this.gcResCostName6.CurrencyDateField = null;
		this.gcResCostName6.CurrencyField = null;
		this.gcResCostName6.CurrencyFixed = null;
		this.gcResCostName6.CurrencyFixedRate = 0.0;
		this.gcResCostName6.CurrencyRateField = null;
		this.gcResCostName6.FieldName = "CostName6";
		this.gcResCostName6.Name = "gcResCostName6";
		this.gcResCostName6.PrimaryKey = false;
		this.gcResCostName6.UnitField = null;
		this.gcResCostName6.Visible = true;
		this.gcResCostName6.VisibleIndex = 13;
		this.gcResCostName6.Width = 138;
		this.gcResCost6.Caption = "Resource Std Cost 6";
		this.gcResCost6.ColumnEdit = this.rCalEdit;
		this.gcResCost6.CurrencyDateField = null;
		this.gcResCost6.CurrencyField = null;
		this.gcResCost6.CurrencyFixed = null;
		this.gcResCost6.CurrencyFixedRate = 0.0;
		this.gcResCost6.CurrencyRateField = null;
		this.gcResCost6.FieldName = "CostVal6";
		this.gcResCost6.Name = "gcResCost6";
		this.gcResCost6.PrimaryKey = false;
		this.gcResCost6.UnitField = null;
		this.gcResCost6.Visible = true;
		this.gcResCost6.VisibleIndex = 14;
		this.gcResCost6.Width = 108;
		this.gcResCostName7.Caption = "Resource Std Cost Name 7";
		this.gcResCostName7.CurrencyDateField = null;
		this.gcResCostName7.CurrencyField = null;
		this.gcResCostName7.CurrencyFixed = null;
		this.gcResCostName7.CurrencyFixedRate = 0.0;
		this.gcResCostName7.CurrencyRateField = null;
		this.gcResCostName7.FieldName = "CostName7";
		this.gcResCostName7.Name = "gcResCostName7";
		this.gcResCostName7.PrimaryKey = false;
		this.gcResCostName7.UnitField = null;
		this.gcResCostName7.Visible = true;
		this.gcResCostName7.VisibleIndex = 15;
		this.gcResCostName7.Width = 138;
		this.gcResCost7.Caption = "Resource Std Cost 7";
		this.gcResCost7.ColumnEdit = this.rCalEdit;
		this.gcResCost7.CurrencyDateField = null;
		this.gcResCost7.CurrencyField = null;
		this.gcResCost7.CurrencyFixed = null;
		this.gcResCost7.CurrencyFixedRate = 0.0;
		this.gcResCost7.CurrencyRateField = null;
		this.gcResCost7.FieldName = "CostVal7";
		this.gcResCost7.Name = "gcResCost7";
		this.gcResCost7.PrimaryKey = false;
		this.gcResCost7.UnitField = null;
		this.gcResCost7.Visible = true;
		this.gcResCost7.VisibleIndex = 16;
		this.gcResCost7.Width = 108;
		this.gcResCostName8.Caption = "Resource Std Cost Name 8";
		this.gcResCostName8.CurrencyDateField = null;
		this.gcResCostName8.CurrencyField = null;
		this.gcResCostName8.CurrencyFixed = null;
		this.gcResCostName8.CurrencyFixedRate = 0.0;
		this.gcResCostName8.CurrencyRateField = null;
		this.gcResCostName8.FieldName = "CostName8";
		this.gcResCostName8.Name = "gcResCostName8";
		this.gcResCostName8.PrimaryKey = false;
		this.gcResCostName8.UnitField = null;
		this.gcResCostName8.Visible = true;
		this.gcResCostName8.VisibleIndex = 17;
		this.gcResCostName8.Width = 138;
		this.gcResCost8.Caption = "Resource Std Cost 8";
		this.gcResCost8.ColumnEdit = this.rCalEdit;
		this.gcResCost8.CurrencyDateField = null;
		this.gcResCost8.CurrencyField = null;
		this.gcResCost8.CurrencyFixed = null;
		this.gcResCost8.CurrencyFixedRate = 0.0;
		this.gcResCost8.CurrencyRateField = null;
		this.gcResCost8.FieldName = "CostVal8";
		this.gcResCost8.Name = "gcResCost8";
		this.gcResCost8.PrimaryKey = false;
		this.gcResCost8.UnitField = null;
		this.gcResCost8.Visible = true;
		this.gcResCost8.VisibleIndex = 18;
		this.gcResCost8.Width = 108;
		this.gcResCostName9.Caption = "Resource Std Cost Name 9";
		this.gcResCostName9.CurrencyDateField = null;
		this.gcResCostName9.CurrencyField = null;
		this.gcResCostName9.CurrencyFixed = null;
		this.gcResCostName9.CurrencyFixedRate = 0.0;
		this.gcResCostName9.CurrencyRateField = null;
		this.gcResCostName9.FieldName = "CostName9";
		this.gcResCostName9.Name = "gcResCostName9";
		this.gcResCostName9.PrimaryKey = false;
		this.gcResCostName9.UnitField = null;
		this.gcResCostName9.Visible = true;
		this.gcResCostName9.VisibleIndex = 19;
		this.gcResCostName9.Width = 138;
		this.gcResCost9.Caption = "Resource Std Cost 9";
		this.gcResCost9.ColumnEdit = this.rCalEdit;
		this.gcResCost9.CurrencyDateField = null;
		this.gcResCost9.CurrencyField = null;
		this.gcResCost9.CurrencyFixed = null;
		this.gcResCost9.CurrencyFixedRate = 0.0;
		this.gcResCost9.CurrencyRateField = null;
		this.gcResCost9.FieldName = "CostVal9";
		this.gcResCost9.Name = "gcResCost9";
		this.gcResCost9.PrimaryKey = false;
		this.gcResCost9.UnitField = null;
		this.gcResCost9.Visible = true;
		this.gcResCost9.VisibleIndex = 20;
		this.gcResCost9.Width = 108;
		this.gcResCostName10.Caption = "Resource Std Cost Name 10";
		this.gcResCostName10.CurrencyDateField = null;
		this.gcResCostName10.CurrencyField = null;
		this.gcResCostName10.CurrencyFixed = null;
		this.gcResCostName10.CurrencyFixedRate = 0.0;
		this.gcResCostName10.CurrencyRateField = null;
		this.gcResCostName10.FieldName = "CostName10";
		this.gcResCostName10.Name = "gcResCostName10";
		this.gcResCostName10.PrimaryKey = false;
		this.gcResCostName10.UnitField = null;
		this.gcResCostName10.Visible = true;
		this.gcResCostName10.VisibleIndex = 21;
		this.gcResCostName10.Width = 144;
		this.gcResCost10.Caption = "Resource Std Cost 10";
		this.gcResCost10.ColumnEdit = this.rCalEdit;
		this.gcResCost10.CurrencyDateField = null;
		this.gcResCost10.CurrencyField = null;
		this.gcResCost10.CurrencyFixed = null;
		this.gcResCost10.CurrencyFixedRate = 0.0;
		this.gcResCost10.CurrencyRateField = null;
		this.gcResCost10.FieldName = "CostVal10";
		this.gcResCost10.Name = "gcResCost10";
		this.gcResCost10.PrimaryKey = false;
		this.gcResCost10.UnitField = null;
		this.gcResCost10.Visible = true;
		this.gcResCost10.VisibleIndex = 22;
		this.gcResCost10.Width = 114;
		this.gcResGrCode.AllowChSet = false;
		this.gcResGrCode.Caption = "Group Code";
		this.gcResGrCode.CurrencyDateField = null;
		this.gcResGrCode.CurrencyField = null;
		this.gcResGrCode.CurrencyFixed = null;
		this.gcResGrCode.CurrencyFixedRate = 0.0;
		this.gcResGrCode.CurrencyRateField = null;
		this.gcResGrCode.FieldName = "ResGrpCod";
		this.gcResGrCode.Name = "gcResGrCode";
		this.gcResGrCode.OptionsColumn.ReadOnly = true;
		this.gcResGrCode.PrimaryKey = false;
		this.gcResGrCode.UnitField = null;
		this.lbSearch.EditControl = null;
		this.lbSearch.EditValue = "Resource Group Name";
		this.lbSearch.FieldName = null;
		this.lbSearch.HideWhenEmpty = false;
		this.lbSearch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbSearch.Location = new System.Drawing.Point(13, 8);
		this.lbSearch.Name = "lbSearch";
		this.lbSearch.ReadOnly = false;
		this.lbSearch.SettingInVisible = false;
		this.lbSearch.SettingReadonly = false;
		this.lbSearch.ShowUnderLine = true;
		this.lbSearch.Size = new System.Drawing.Size(121, 20);
		this.lbSearch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbSearch.TabIndex = 1;
		this.lbSearch.Utext = "Resource Group Name";
		this.txtSearch.Location = new System.Drawing.Point(135, 5);
		this.txtSearch.Name = "txtSearch";
		this.txtSearch.Size = new System.Drawing.Size(196, 20);
		this.txtSearch.TabIndex = 2;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.Location = new System.Drawing.Point(339, 5);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.Size = new System.Drawing.Size(75, 20);
		this.btnSearch.TabIndex = 3;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(663, 213);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 4;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.Location = new System.Drawing.Point(583, 213);
		this.btnSave.Name = "btnSave";
		this.btnSave.Size = new System.Drawing.Size(75, 23);
		this.btnSave.TabIndex = 5;
		this.btnSave.Text = "Save";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(744, 240);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.txtSearch);
		base.Controls.Add(this.lbSearch);
		base.Controls.Add(this.grdDoc);
		base.FormId = "oResourceGroup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ResourceGroup";
		this.Text = "Resource Group";
		base.Load += new System.EventHandler(this.frm_ResourceGroup_Load);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStripDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rResType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCalEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSearch.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
