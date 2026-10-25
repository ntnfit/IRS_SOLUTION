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
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Financials;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_Freight : XtraFormEx
{
	private readonly List<int> _delLst = new List<int>();

	private IContainer components;

	private SimpleButtonEx btn_Update;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gcExpnsName;

	private GridColumnEx gcRevenueAccount;

	private GridColumnEx gcExpenseAccount;

	private GridColumnEx gcOutputTaxGroup;

	private GridColumnEx gcFixedAmountRevenues;

	private GridColumnEx gcFixedAmountExpenses;

	private GridColumnEx gcWTaxLiable;

	private GridColumnEx gcDistributionMethod;

	private GridColumnEx gcDrawingMethod;

	private GridColumnEx gcStockFixedAsset;

	private GridColumnEx gcLastPurchasePrice;

	private GridColumnEx gcDepartment;

	private GridColumnEx gcProject;

	private GridColumnEx gcInputTaxGroup;

	private RepositoryItemButtonEdit rExpenseAccount;

	private RepositoryItemCalcEdit rFixedAmountExpenses;

	private RepositoryItemCheckEdit rWTaxLiable;

	private RepositoryItemTextEdit rExpnsName;

	private RepositoryItemButtonEdit rRevenueAccount;

	private RepositoryItemCalcEdit rFixedAmountRevenues;

	private RepositoryItemCheckEdit rStockFixedAsset;

	private RepositoryItemButtonEdit rOcrCode;

	private RepositoryItemButtonEdit rProject;

	private GridColumnEx gcExpnsCode;

	private ContextMenuStrip contextMenuStripDoc;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem insertToolStripMenuItem;

	private RepositoryItemCheckEdit rLstPchPrce;

	private RepositoryItemLookUpEdit rDrawingMethod;

	private RepositoryItemLookUpEdit rDistributionMethod;

	private GridColumnEx gcbusinessGroup;

	private RepositoryItemButtonEdit rbusinessGroup;

	private RepositoryItemLookUpEdit rVatGroupI;

	private RepositoryItemLookUpEdit rOutputTaxGroup;

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
		case Enums.FormViewModeType.Add:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			deleteToolStripMenuItem.Enabled = false;
			break;
		case Enums.FormViewModeType.Update:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			deleteToolStripMenuItem.Enabled = true;
			break;
		case Enums.FormViewModeType.View:
			btn_Update.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			deleteToolStripMenuItem.Enabled = true;
			break;
		}
	}

	public frm_Freight()
	{
		InitializeComponent();
		OnLoadAsync += frm_Freight_Load;
	}

	private async Task frm_Freight_Load(object sender, EventArgs e)
	{
		await LoadDataSourceEditor();
		await LoadData();
	}

	private async Task LoadDataSourceEditor()
	{
		List<Common.Table_ID> list = new List<Common.Table_ID>();
		list.Add(new Common.Table_ID
		{
			ID = "N",
			Name = "None"
		});
		list.Add(new Common.Table_ID
		{
			ID = "Q",
			Name = "Quantity"
		});
		list.Add(new Common.Table_ID
		{
			ID = "V",
			Name = "Volume"
		});
		list.Add(new Common.Table_ID
		{
			ID = "W",
			Name = "Weight"
		});
		list.Add(new Common.Table_ID
		{
			ID = "E",
			Name = "Equally"
		});
		list.Add(new Common.Table_ID
		{
			ID = "T",
			Name = "Row Total"
		});
		rDistributionMethod.DataSource = list;
		List<Common.Table_ID> list2 = new List<Common.Table_ID>();
		list2.Add(new Common.Table_ID
		{
			ID = "N",
			Name = "None"
		});
		list2.Add(new Common.Table_ID
		{
			ID = "Q",
			Name = "Quantity"
		});
		list2.Add(new Common.Table_ID
		{
			ID = "T",
			Name = "Total"
		});
		list2.Add(new Common.Table_ID
		{
			ID = "A",
			Name = "All"
		});
		rDrawingMethod.DataSource = list2;
		using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Freight");
		RepositoryItemLookUpEdit repositoryItemLookUpEdit = rOutputTaxGroup;
		repositoryItemLookUpEdit.DataSource = await helper.GetAsync("GetOutputTaxGroup");
		repositoryItemLookUpEdit = rVatGroupI;
		repositoryItemLookUpEdit.DataSource = await helper.GetAsync("GetVatGroupI");
	}

	private async Task LoadData()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Freight");
			DataTable dataSource = await helper.GetAsync("GetFreight");
			grdDoc.DataSource = dataSource;
			grdDoc.RefreshDataSource();
			_delLst.Clear();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Update_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) != DialogResult.No)
			{
				WinCommons._OpenProcessing();
				string text = await Save();
				if (string.IsNullOrEmpty(text))
				{
					await LoadData();
					WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.Error(Enums.MessageCode.None, ex.Message);
		}
	}

	private async Task<string> Save()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSourceTable.Copy();
			foreach (int item in _delLst)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["ExpnsCode"] = -item;
				dataTable.Rows.Add(dataRow);
			}
			dataTable.AcceptChanges();
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Freight");
			return await http.PostAsync("UpdateFreight", dataTable);
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSourceTable.Copy();
			if (dataTable.IsNotNull())
			{
				int focusedRowHandle = gvDoc.FocusedRowHandle;
				if (Function.ParseInt(dataTable.Rows[focusedRowHandle]["ExpnsCode"]) > 0)
				{
					_delLst.Add(Function.ParseInt(dataTable.Rows[focusedRowHandle]["ExpnsCode"]));
				}
				dataTable.Rows.RemoveAt(focusedRowHandle);
				dataTable.AcceptChanges();
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> Delete(int code)
	{
		using HttpClientHelper<string> _helper = new HttpClientHelper<string>(Configuration.ApiServer, "Freight");
		return await _helper.PostAsync("DeleteFreight", code);
	}

	private async void AddNewRow()
	{
		DataTable dataTable = new DataTable();
		dataTable.Columns.Add("ExpnsCode", typeof(int));
		dataTable.Columns.Add("ExpnsName", typeof(string));
		dataTable.Columns.Add("RevAcct", typeof(string));
		dataTable.Columns.Add("ExpnsAcct", typeof(string));
		dataTable.Columns.Add("VatGroupO", typeof(string));
		dataTable.Columns.Add("VatGroupI", typeof(string));
		dataTable.Columns.Add("RevFixSum", typeof(long));
		dataTable.Columns.Add("ExpFixSum", typeof(long));
		dataTable.Columns.Add("WTLiable", typeof(string));
		dataTable.Columns.Add("DistrbMthd", typeof(string));
		dataTable.Columns.Add("BaseMethod", typeof(string));
		dataTable.Columns.Add("Stock", typeof(string));
		dataTable.Columns.Add("LstPchPrce", typeof(string));
		dataTable.Columns.Add("OcrCode", typeof(string));
		dataTable.Columns.Add("OcrCode2", typeof(string));
		dataTable.Columns.Add("Project", typeof(string));
		DataRow dataRow = dataTable.NewRow();
		dataRow["ExpnsCode"] = 0;
		dataRow["ExpnsName"] = "";
		dataRow["RevAcct"] = "";
		dataRow["ExpnsAcct"] = "";
		dataRow["VatGroupO"] = "";
		dataRow["VatGroupI"] = "";
		dataRow["RevFixSum"] = 0;
		dataRow["ExpFixSum"] = 0;
		dataRow["WTLiable"] = 'N';
		dataRow["DistrbMthd"] = 'N';
		dataRow["BaseMethod"] = 'A';
		dataRow["Stock"] = 'N';
		dataRow["LstPchPrce"] = 'N';
		dataRow["OcrCode"] = "";
		dataRow["OcrCode2"] = "";
		dataRow["Project"] = "";
		dataTable.Rows.Add(dataRow);
		grdDoc.DataSource = dataTable;
		grdDoc.RefreshDataSource();
	}

	private async Task<DataTable> GetMaxExpnsCode()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Freight");
			return await helper.GetAsync("GetMaxExpnsCode");
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private void insertMenuItem_Click(object sender, EventArgs e)
	{
		AddNewRow();
		FormMode = Enums.FormViewModeType.Add;
	}

	private void rExpenseAccount_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.AcctCode, multiSelect: false, "");
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("ExpnsAcct", frm_ChooseFromList2.ChooseItem.AcctCode);
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rRevenueAccount_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string txtText = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcRevenueAccount));
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.RevennueAccount, multiSelect: false, txtText);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					_ = (AcctCodeImpl)frm_ChooseFromList2.ChooseItem;
					gvDoc.SetFocusedRowCellValue("RevAcct", frm_ChooseFromList2.ChooseItem.AcctCode);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rOcrCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string txtText = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcDepartment));
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OrcCode, multiSelect: false, txtText);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("OcrCode", frm_ChooseFromList2.ChooseItem["OcrCode"]);
				}
			}
			else if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "OcrCode"))))
			{
				WinCommons._OpenProcessing();
				frm_DistributeRule frm_DistributeRule2 = new frm_DistributeRule();
				frm_DistributeRule2.MdiParent = MdiParent;
				frm_DistributeRule2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void rbusinessGroup_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string txtText = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcbusinessGroup));
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.OrcCode2, multiSelect: false, txtText);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("OcrCode2", frm_ChooseFromList2.ChooseItem["OcrCode"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rProject_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				string txtText = Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, gcProject));
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Project, multiSelect: false, txtText);
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue("Project", frm_ChooseFromList2.ChooseItem["PrjCode"]);
				}
			}
			else if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(gvDoc.FocusedRowHandle, "Project"))))
			{
				WinCommons._OpenProcessing();
				frm_Project frm_Project2 = new frm_Project();
				frm_Project2.MdiParent = MdiParent;
				frm_Project2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void gv_Freight_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (FormMode != Enums.FormViewModeType.Add)
		{
			FormMode = Enums.FormViewModeType.Update;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		this.btn_Update = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.insertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcExpnsCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcExpnsName = new Apzon.Controls.Grid.GridColumnEx();
		this.rExpnsName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gcRevenueAccount = new Apzon.Controls.Grid.GridColumnEx();
		this.rRevenueAccount = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcExpenseAccount = new Apzon.Controls.Grid.GridColumnEx();
		this.rExpenseAccount = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcOutputTaxGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rOutputTaxGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcInputTaxGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rVatGroupI = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcFixedAmountRevenues = new Apzon.Controls.Grid.GridColumnEx();
		this.rFixedAmountRevenues = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcFixedAmountExpenses = new Apzon.Controls.Grid.GridColumnEx();
		this.rFixedAmountExpenses = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcWTaxLiable = new Apzon.Controls.Grid.GridColumnEx();
		this.rWTaxLiable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcDistributionMethod = new Apzon.Controls.Grid.GridColumnEx();
		this.rDistributionMethod = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcDrawingMethod = new Apzon.Controls.Grid.GridColumnEx();
		this.rDrawingMethod = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcStockFixedAsset = new Apzon.Controls.Grid.GridColumnEx();
		this.rStockFixedAsset = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcLastPurchasePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rLstPchPrce = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gcDepartment = new Apzon.Controls.Grid.GridColumnEx();
		this.rOcrCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcbusinessGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rbusinessGroup = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcProject = new Apzon.Controls.Grid.GridColumnEx();
		this.rProject = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStripDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rExpnsName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rRevenueAccount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rExpenseAccount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOutputTaxGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rVatGroupI).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rFixedAmountRevenues).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rFixedAmountExpenses).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rWTaxLiable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDistributionMethod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDrawingMethod).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rStockFixedAsset).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLstPchPrce).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rOcrCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rbusinessGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rProject).BeginInit();
		base.SuspendLayout();
		this.btn_Update.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Update.Appearance.ForeColor = System.Drawing.Color.Black;
		this.btn_Update.Appearance.Options.UseForeColor = true;
		this.btn_Update.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Update;
		this.btn_Update.EditControl = null;
		this.btn_Update.EditValue = "OK";
		this.btn_Update.FieldName = null;
		this.btn_Update.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Update.IsSearchControl = false;
		this.btn_Update.Location = new System.Drawing.Point(7, 398);
		this.btn_Update.Name = "btn_Update";
		this.btn_Update.ReadOnly = false;
		this.btn_Update.SettingInVisible = false;
		this.btn_Update.SettingReadonly = false;
		this.btn_Update.Size = new System.Drawing.Size(100, 20);
		this.btn_Update.TabIndex = 1;
		this.btn_Update.Text = "OK";
		this.btn_Update.Click += new System.EventHandler(this.btn_Update_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(113, 398);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(100, 20);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.grdDoc.AllowPasteExcel = false;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStripDoc;
		this.grdDoc.Location = new System.Drawing.Point(1, 4);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[15]
		{
			this.rExpenseAccount, this.rFixedAmountExpenses, this.rWTaxLiable, this.rExpnsName, this.rRevenueAccount, this.rFixedAmountRevenues, this.rStockFixedAsset, this.rOcrCode, this.rProject, this.rLstPchPrce,
			this.rDrawingMethod, this.rDistributionMethod, this.rbusinessGroup, this.rVatGroupI, this.rOutputTaxGroup
		});
		this.grdDoc.Size = new System.Drawing.Size(886, 388);
		this.grdDoc.TabIndex = 1;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStripDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.insertToolStripMenuItem, this.deleteToolStripMenuItem });
		this.contextMenuStripDoc.Name = "contextMenuStrip1";
		this.contextMenuStripDoc.Size = new System.Drawing.Size(124, 48);
		this.insertToolStripMenuItem.Name = "insertToolStripMenuItem";
		this.insertToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.insertToolStripMenuItem.Text = "Add New";
		this.insertToolStripMenuItem.Click += new System.EventHandler(this.insertMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.gvDoc.Appearance.HeaderPanel.Options.UseTextOptions = true;
		this.gvDoc.Appearance.HeaderPanel.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[16]
		{
			this.gcExpnsCode, this.gcExpnsName, this.gcRevenueAccount, this.gcExpenseAccount, this.gcOutputTaxGroup, this.gcInputTaxGroup, this.gcFixedAmountRevenues, this.gcFixedAmountExpenses, this.gcWTaxLiable, this.gcDistributionMethod,
			this.gcDrawingMethod, this.gcStockFixedAsset, this.gcLastPurchasePrice, this.gcDepartment, this.gcbusinessGroup, this.gcProject
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oFreight";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OEXD";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.False;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Freight_CustomDrawRowIndicator);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gcExpnsCode.AppearanceCell.Options.UseTextOptions = true;
		this.gcExpnsCode.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.gcExpnsCode.Caption = "Code";
		this.gcExpnsCode.CurrencyDateField = null;
		this.gcExpnsCode.CurrencyField = null;
		this.gcExpnsCode.CurrencyFixed = null;
		this.gcExpnsCode.CurrencyFixedRate = 0.0;
		this.gcExpnsCode.CurrencyRateField = null;
		this.gcExpnsCode.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcExpnsCode.FieldName = "ExpnsCode";
		this.gcExpnsCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcExpnsCode.Name = "gcExpnsCode";
		this.gcExpnsCode.OptionsColumn.AllowEdit = false;
		this.gcExpnsCode.PrimaryKey = true;
		this.gcExpnsCode.UnitField = null;
		this.gcExpnsCode.Width = 37;
		this.gcExpnsName.Caption = "Name";
		this.gcExpnsName.ColumnEdit = this.rExpnsName;
		this.gcExpnsName.CurrencyDateField = null;
		this.gcExpnsName.CurrencyField = null;
		this.gcExpnsName.CurrencyFixed = null;
		this.gcExpnsName.CurrencyFixedRate = 0.0;
		this.gcExpnsName.CurrencyRateField = null;
		this.gcExpnsName.FieldName = "ExpnsName";
		this.gcExpnsName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcExpnsName.Name = "gcExpnsName";
		this.gcExpnsName.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcExpnsName.PrimaryKey = true;
		this.gcExpnsName.UnitField = null;
		this.gcExpnsName.Visible = true;
		this.gcExpnsName.VisibleIndex = 0;
		this.gcExpnsName.Width = 91;
		this.rExpnsName.AutoHeight = false;
		this.rExpnsName.Name = "rExpnsName";
		this.gcRevenueAccount.Caption = "Revenue Account";
		this.gcRevenueAccount.ColumnEdit = this.rRevenueAccount;
		this.gcRevenueAccount.CurrencyDateField = null;
		this.gcRevenueAccount.CurrencyField = null;
		this.gcRevenueAccount.CurrencyFixed = null;
		this.gcRevenueAccount.CurrencyFixedRate = 0.0;
		this.gcRevenueAccount.CurrencyRateField = null;
		this.gcRevenueAccount.FieldName = "RevAcct";
		this.gcRevenueAccount.Name = "gcRevenueAccount";
		this.gcRevenueAccount.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcRevenueAccount.PrimaryKey = false;
		this.gcRevenueAccount.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcRevenueAccount.UnitField = null;
		this.gcRevenueAccount.Visible = true;
		this.gcRevenueAccount.VisibleIndex = 1;
		this.gcRevenueAccount.Width = 98;
		this.rRevenueAccount.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rRevenueAccount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rRevenueAccount.Name = "rRevenueAccount";
		this.rRevenueAccount.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rRevenueAccount_ButtonClick);
		this.gcExpenseAccount.Caption = "Expense Account";
		this.gcExpenseAccount.ColumnEdit = this.rExpenseAccount;
		this.gcExpenseAccount.CurrencyDateField = null;
		this.gcExpenseAccount.CurrencyField = null;
		this.gcExpenseAccount.CurrencyFixed = null;
		this.gcExpenseAccount.CurrencyFixedRate = 0.0;
		this.gcExpenseAccount.CurrencyRateField = null;
		this.gcExpenseAccount.FieldName = "ExpnsAcct";
		this.gcExpenseAccount.Name = "gcExpenseAccount";
		this.gcExpenseAccount.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcExpenseAccount.PrimaryKey = false;
		this.gcExpenseAccount.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcExpenseAccount.UnitField = null;
		this.gcExpenseAccount.Visible = true;
		this.gcExpenseAccount.VisibleIndex = 2;
		this.gcExpenseAccount.Width = 116;
		this.rExpenseAccount.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rExpenseAccount.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rExpenseAccount.Name = "rExpenseAccount";
		this.rExpenseAccount.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rExpenseAccount_ButtonClick);
		this.gcOutputTaxGroup.Caption = "Output Tax Group";
		this.gcOutputTaxGroup.ColumnEdit = this.rOutputTaxGroup;
		this.gcOutputTaxGroup.CurrencyDateField = null;
		this.gcOutputTaxGroup.CurrencyField = null;
		this.gcOutputTaxGroup.CurrencyFixed = null;
		this.gcOutputTaxGroup.CurrencyFixedRate = 0.0;
		this.gcOutputTaxGroup.CurrencyRateField = null;
		this.gcOutputTaxGroup.FieldName = "VatGroupI";
		this.gcOutputTaxGroup.Name = "gcOutputTaxGroup";
		this.gcOutputTaxGroup.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcOutputTaxGroup.PrimaryKey = false;
		this.gcOutputTaxGroup.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcOutputTaxGroup.UnitField = null;
		this.gcOutputTaxGroup.Visible = true;
		this.gcOutputTaxGroup.VisibleIndex = 3;
		this.gcOutputTaxGroup.Width = 115;
		this.rOutputTaxGroup.AutoHeight = false;
		this.rOutputTaxGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rOutputTaxGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name", 50, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rOutputTaxGroup.DisplayMember = "Code";
		this.rOutputTaxGroup.Name = "rOutputTaxGroup";
		this.rOutputTaxGroup.NullText = " ";
		this.rOutputTaxGroup.ShowFooter = false;
		this.rOutputTaxGroup.ShowHeader = false;
		this.rOutputTaxGroup.ShowLines = false;
		this.rOutputTaxGroup.ValueMember = "Code";
		this.gcInputTaxGroup.Caption = "Input Tax Group";
		this.gcInputTaxGroup.ColumnEdit = this.rVatGroupI;
		this.gcInputTaxGroup.CurrencyDateField = null;
		this.gcInputTaxGroup.CurrencyField = null;
		this.gcInputTaxGroup.CurrencyFixed = null;
		this.gcInputTaxGroup.CurrencyFixedRate = 0.0;
		this.gcInputTaxGroup.CurrencyRateField = null;
		this.gcInputTaxGroup.FieldName = "VatGroupO";
		this.gcInputTaxGroup.Name = "gcInputTaxGroup";
		this.gcInputTaxGroup.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcInputTaxGroup.PrimaryKey = false;
		this.gcInputTaxGroup.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcInputTaxGroup.UnitField = null;
		this.gcInputTaxGroup.Visible = true;
		this.gcInputTaxGroup.VisibleIndex = 4;
		this.gcInputTaxGroup.Width = 105;
		this.rVatGroupI.AutoHeight = false;
		this.rVatGroupI.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rVatGroupI.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name", 50, DevExpress.Utils.FormatType.None, "", true, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rVatGroupI.DisplayMember = "Code";
		this.rVatGroupI.Name = "rVatGroupI";
		this.rVatGroupI.NullText = " ";
		this.rVatGroupI.ShowFooter = false;
		this.rVatGroupI.ShowHeader = false;
		this.rVatGroupI.ShowLines = false;
		this.rVatGroupI.ValueMember = "Code";
		this.gcFixedAmountRevenues.AppearanceCell.Options.UseTextOptions = true;
		this.gcFixedAmountRevenues.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.gcFixedAmountRevenues.Caption = "Fixed Amount - Revenues";
		this.gcFixedAmountRevenues.ColumnEdit = this.rFixedAmountRevenues;
		this.gcFixedAmountRevenues.CurrencyDateField = null;
		this.gcFixedAmountRevenues.CurrencyField = null;
		this.gcFixedAmountRevenues.CurrencyFixed = null;
		this.gcFixedAmountRevenues.CurrencyFixedRate = 0.0;
		this.gcFixedAmountRevenues.CurrencyRateField = null;
		this.gcFixedAmountRevenues.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcFixedAmountRevenues.FieldName = "RevFixSum";
		this.gcFixedAmountRevenues.Name = "gcFixedAmountRevenues";
		this.gcFixedAmountRevenues.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcFixedAmountRevenues.PrimaryKey = false;
		this.gcFixedAmountRevenues.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcFixedAmountRevenues.UnitField = null;
		this.gcFixedAmountRevenues.Visible = true;
		this.gcFixedAmountRevenues.VisibleIndex = 5;
		this.gcFixedAmountRevenues.Width = 137;
		this.rFixedAmountRevenues.Appearance.Options.UseTextOptions = true;
		this.rFixedAmountRevenues.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.rFixedAmountRevenues.AutoHeight = false;
		this.rFixedAmountRevenues.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
		});
		this.rFixedAmountRevenues.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rFixedAmountRevenues.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rFixedAmountRevenues.Name = "rFixedAmountRevenues";
		this.gcFixedAmountExpenses.AppearanceCell.Options.UseTextOptions = true;
		this.gcFixedAmountExpenses.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.gcFixedAmountExpenses.Caption = "Fixed Amount Expenses";
		this.gcFixedAmountExpenses.ColumnEdit = this.rFixedAmountExpenses;
		this.gcFixedAmountExpenses.CurrencyDateField = null;
		this.gcFixedAmountExpenses.CurrencyField = null;
		this.gcFixedAmountExpenses.CurrencyFixed = null;
		this.gcFixedAmountExpenses.CurrencyFixedRate = 0.0;
		this.gcFixedAmountExpenses.CurrencyRateField = null;
		this.gcFixedAmountExpenses.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gcFixedAmountExpenses.FieldName = "ExpFixSum";
		this.gcFixedAmountExpenses.Name = "gcFixedAmountExpenses";
		this.gcFixedAmountExpenses.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcFixedAmountExpenses.PrimaryKey = false;
		this.gcFixedAmountExpenses.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcFixedAmountExpenses.UnitField = null;
		this.gcFixedAmountExpenses.Visible = true;
		this.gcFixedAmountExpenses.VisibleIndex = 6;
		this.gcFixedAmountExpenses.Width = 124;
		this.rFixedAmountExpenses.Appearance.Options.UseTextOptions = true;
		this.rFixedAmountExpenses.Appearance.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Near;
		this.rFixedAmountExpenses.AutoHeight = false;
		this.rFixedAmountExpenses.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph)
		});
		this.rFixedAmountExpenses.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rFixedAmountExpenses.EditFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.rFixedAmountExpenses.Name = "rFixedAmountExpenses";
		this.gcWTaxLiable.Caption = "WTax Liable";
		this.gcWTaxLiable.ColumnEdit = this.rWTaxLiable;
		this.gcWTaxLiable.CurrencyDateField = null;
		this.gcWTaxLiable.CurrencyField = null;
		this.gcWTaxLiable.CurrencyFixed = null;
		this.gcWTaxLiable.CurrencyFixedRate = 0.0;
		this.gcWTaxLiable.CurrencyRateField = null;
		this.gcWTaxLiable.FieldName = "WTLiable";
		this.gcWTaxLiable.Name = "gcWTaxLiable";
		this.gcWTaxLiable.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcWTaxLiable.PrimaryKey = false;
		this.gcWTaxLiable.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcWTaxLiable.UnitField = null;
		this.gcWTaxLiable.Visible = true;
		this.gcWTaxLiable.VisibleIndex = 7;
		this.gcWTaxLiable.Width = 69;
		this.rWTaxLiable.AutoHeight = false;
		this.rWTaxLiable.Name = "rWTaxLiable";
		this.rWTaxLiable.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive;
		this.rWTaxLiable.ValueChecked = "Y";
		this.rWTaxLiable.ValueUnchecked = "N";
		this.gcDistributionMethod.Caption = "Distribution Method";
		this.gcDistributionMethod.ColumnEdit = this.rDistributionMethod;
		this.gcDistributionMethod.CurrencyDateField = null;
		this.gcDistributionMethod.CurrencyField = null;
		this.gcDistributionMethod.CurrencyFixed = null;
		this.gcDistributionMethod.CurrencyFixedRate = 0.0;
		this.gcDistributionMethod.CurrencyRateField = null;
		this.gcDistributionMethod.FieldName = "DistrbMthd";
		this.gcDistributionMethod.Name = "gcDistributionMethod";
		this.gcDistributionMethod.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcDistributionMethod.PrimaryKey = false;
		this.gcDistributionMethod.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDistributionMethod.UnitField = null;
		this.gcDistributionMethod.Visible = true;
		this.gcDistributionMethod.VisibleIndex = 8;
		this.gcDistributionMethod.Width = 108;
		this.rDistributionMethod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDistributionMethod.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "", 5, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rDistributionMethod.DisplayMember = "Name";
		this.rDistributionMethod.DropDownRows = 6;
		this.rDistributionMethod.Name = "rDistributionMethod";
		this.rDistributionMethod.NullText = " ";
		this.rDistributionMethod.PopupFormMinSize = new System.Drawing.Size(90, 0);
		this.rDistributionMethod.PopupWidth = 50;
		this.rDistributionMethod.ShowFooter = false;
		this.rDistributionMethod.ShowHeader = false;
		this.rDistributionMethod.ShowLines = false;
		this.rDistributionMethod.ValueMember = "ID";
		this.gcDrawingMethod.Caption = "Drawing Method";
		this.gcDrawingMethod.ColumnEdit = this.rDrawingMethod;
		this.gcDrawingMethod.CurrencyDateField = null;
		this.gcDrawingMethod.CurrencyField = null;
		this.gcDrawingMethod.CurrencyFixed = null;
		this.gcDrawingMethod.CurrencyFixedRate = 0.0;
		this.gcDrawingMethod.CurrencyRateField = null;
		this.gcDrawingMethod.FieldName = "BaseMethod";
		this.gcDrawingMethod.Name = "gcDrawingMethod";
		this.gcDrawingMethod.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcDrawingMethod.PrimaryKey = false;
		this.gcDrawingMethod.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDrawingMethod.UnitField = null;
		this.gcDrawingMethod.Visible = true;
		this.gcDrawingMethod.VisibleIndex = 9;
		this.gcDrawingMethod.Width = 96;
		this.rDrawingMethod.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDrawingMethod.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rDrawingMethod.DisplayMember = "Name";
		this.rDrawingMethod.DropDownRows = 4;
		this.rDrawingMethod.Name = "rDrawingMethod";
		this.rDrawingMethod.NullText = " ";
		this.rDrawingMethod.PopupFormMinSize = new System.Drawing.Size(90, 0);
		this.rDrawingMethod.PopupWidth = 50;
		this.rDrawingMethod.ShowFooter = false;
		this.rDrawingMethod.ShowHeader = false;
		this.rDrawingMethod.ShowLines = false;
		this.rDrawingMethod.ShowPopupShadow = false;
		this.rDrawingMethod.ValueMember = "ID";
		this.gcStockFixedAsset.Caption = "Stock/Fixed Asset";
		this.gcStockFixedAsset.ColumnEdit = this.rStockFixedAsset;
		this.gcStockFixedAsset.CurrencyDateField = null;
		this.gcStockFixedAsset.CurrencyField = null;
		this.gcStockFixedAsset.CurrencyFixed = null;
		this.gcStockFixedAsset.CurrencyFixedRate = 0.0;
		this.gcStockFixedAsset.CurrencyRateField = null;
		this.gcStockFixedAsset.FieldName = "Stock";
		this.gcStockFixedAsset.Name = "gcStockFixedAsset";
		this.gcStockFixedAsset.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcStockFixedAsset.PrimaryKey = false;
		this.gcStockFixedAsset.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcStockFixedAsset.UnitField = null;
		this.gcStockFixedAsset.Visible = true;
		this.gcStockFixedAsset.VisibleIndex = 10;
		this.gcStockFixedAsset.Width = 98;
		this.rStockFixedAsset.AutoHeight = false;
		this.rStockFixedAsset.Name = "rStockFixedAsset";
		this.rStockFixedAsset.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive;
		this.rStockFixedAsset.ValueChecked = "Y";
		this.rStockFixedAsset.ValueUnchecked = "N";
		this.gcLastPurchasePrice.Caption = "Last Purchase Price";
		this.gcLastPurchasePrice.ColumnEdit = this.rLstPchPrce;
		this.gcLastPurchasePrice.CurrencyDateField = null;
		this.gcLastPurchasePrice.CurrencyField = null;
		this.gcLastPurchasePrice.CurrencyFixed = null;
		this.gcLastPurchasePrice.CurrencyFixedRate = 0.0;
		this.gcLastPurchasePrice.CurrencyRateField = null;
		this.gcLastPurchasePrice.FieldName = "LstPchPrce";
		this.gcLastPurchasePrice.Name = "gcLastPurchasePrice";
		this.gcLastPurchasePrice.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcLastPurchasePrice.PrimaryKey = false;
		this.gcLastPurchasePrice.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcLastPurchasePrice.UnitField = null;
		this.gcLastPurchasePrice.Visible = true;
		this.gcLastPurchasePrice.VisibleIndex = 11;
		this.gcLastPurchasePrice.Width = 110;
		this.rLstPchPrce.AutoHeight = false;
		this.rLstPchPrce.Name = "rLstPchPrce";
		this.rLstPchPrce.NullStyle = DevExpress.XtraEditors.Controls.StyleIndeterminate.Inactive;
		this.rLstPchPrce.ValueChecked = "Y";
		this.rLstPchPrce.ValueUnchecked = "N";
		this.gcDepartment.Caption = "Phòng ban";
		this.gcDepartment.ColumnEdit = this.rOcrCode;
		this.gcDepartment.CurrencyDateField = null;
		this.gcDepartment.CurrencyField = null;
		this.gcDepartment.CurrencyFixed = null;
		this.gcDepartment.CurrencyFixedRate = 0.0;
		this.gcDepartment.CurrencyRateField = null;
		this.gcDepartment.FieldName = "OcrCode";
		this.gcDepartment.Name = "gcDepartment";
		this.gcDepartment.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcDepartment.PrimaryKey = false;
		this.gcDepartment.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcDepartment.UnitField = null;
		this.gcDepartment.Visible = true;
		this.gcDepartment.VisibleIndex = 12;
		this.gcDepartment.Width = 72;
		this.rOcrCode.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rOcrCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rOcrCode.Name = "rOcrCode";
		this.rOcrCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rOcrCode_ButtonClick);
		this.gcbusinessGroup.Caption = "Nhóm kinh doanh";
		this.gcbusinessGroup.ColumnEdit = this.rbusinessGroup;
		this.gcbusinessGroup.CurrencyDateField = null;
		this.gcbusinessGroup.CurrencyField = null;
		this.gcbusinessGroup.CurrencyFixed = null;
		this.gcbusinessGroup.CurrencyFixedRate = 0.0;
		this.gcbusinessGroup.CurrencyRateField = null;
		this.gcbusinessGroup.FieldName = "OcrCode2";
		this.gcbusinessGroup.Name = "gcbusinessGroup";
		this.gcbusinessGroup.PrimaryKey = false;
		this.gcbusinessGroup.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcbusinessGroup.UnitField = null;
		this.gcbusinessGroup.Visible = true;
		this.gcbusinessGroup.VisibleIndex = 13;
		this.gcbusinessGroup.Width = 104;
		this.rbusinessGroup.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rbusinessGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, false, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rbusinessGroup.Name = "rbusinessGroup";
		this.rbusinessGroup.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rbusinessGroup_ButtonClick);
		this.gcProject.Caption = "Project";
		this.gcProject.ColumnEdit = this.rProject;
		this.gcProject.CurrencyDateField = null;
		this.gcProject.CurrencyField = null;
		this.gcProject.CurrencyFixed = null;
		this.gcProject.CurrencyFixedRate = 0.0;
		this.gcProject.CurrencyRateField = null;
		this.gcProject.FieldName = "Project";
		this.gcProject.Name = "gcProject";
		this.gcProject.OptionsColumn.AllowSort = DevExpress.Utils.DefaultBoolean.False;
		this.gcProject.PrimaryKey = false;
		this.gcProject.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcProject.UnitField = null;
		this.gcProject.Visible = true;
		this.gcProject.VisibleIndex = 14;
		this.gcProject.Width = 100;
		this.rProject.AutoHeight = false;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rProject.Name = "rProject";
		this.rProject.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rProject_ButtonClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(891, 428);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Update);
		base.FormId = "oFreight";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Freight";
		this.Text = "Freight - Setup";
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStripDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rExpnsName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rRevenueAccount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rExpenseAccount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOutputTaxGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rVatGroupI).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rFixedAmountRevenues).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rFixedAmountExpenses).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rWTaxLiable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDistributionMethod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDrawingMethod).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rStockFixedAsset).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLstPchPrce).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rOcrCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rbusinessGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rProject).EndInit();
		base.ResumeLayout(false);
	}

	private void rOutputTaxGroup_EditValueChanged(object sender, EventArgs e)
	{
	}
}
