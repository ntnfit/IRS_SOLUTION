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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator.SetUp.Purchasing;

public class frm_LandedCostsSetup : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public ReloadLandedCost SendDataBackToMain;

	public string Code;

	private IContainer components;

	private GridControlEx grd_LandedCostSetup;

	private GridViewEx gv_LandedCostSetup;

	private GridColumnEx col_AlcCode;

	private GridColumnEx col_AlcName;

	private GridColumnEx col_OhType;

	private GridColumnEx col_AcctCode;

	private GridColumnEx col_DataSource;

	private GridColumnEx col_UserSign;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Ok;

	private GridColumnEx col_LaCAllcAcc;

	private RepositoryItemLookUpEdit lk_OhType;

	private ContextMenuStrip contextMenuStrip_LC;

	private ToolStripMenuItem toolStripMenuItem_Remove;

	private RepositoryItemButtonEdit rep_LaCAll;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			btn_Ok.Enabled = false;
			btn_Cancel.Enabled = true;
			return;
		}
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_LandedCostsSetup()
	{
		InitializeComponent();
		OnLoadAsync += frm_LandedCostsSetup_Load;
	}

	private async Task frm_LandedCostsSetup_Load(object sender, EventArgs e)
	{
		try
		{
			SetDataType();
			await RefreshLandedCost();
			if (!string.IsNullOrEmpty(Code))
			{
				gv_LandedCostSetup.FocusedRowHandle = gv_LandedCostSetup.LocateByValue("AlcCode", Code);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SetDataType()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Cash Value Before Customs",
			Code = "F"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Cash Value After Customs",
			Code = "C"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Quanlity",
			Code = "Q"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Weight",
			Code = "W"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Volume",
			Code = "V"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Equal",
			Code = "A"
		});
		lk_OhType.DataSource = list;
	}

	private void toolStripMenuItem_Remove_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_LandedCostSetup.GetFocusedRowCellValue("AlcCode"));
			if (text != null && !(text == "") && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				RemoveLanedCost(text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				btn_Ok.Enabled = false;
				await SaveLandedCost();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Ok.Enabled = true;
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gv_LandedCost_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			int num = 0;
			for (int i = 0; i < gv_LandedCostSetup.RowCount; i++)
			{
				num = gv_LandedCostSetup.GetVisibleRowHandle(i);
			}
			string text = Function.ToString(gv_LandedCostSetup.GetRowCellValue(e.RowHandle, "LaCAllcAcc"));
			if (e.Column.FieldName == "LaCAllcAcc" && text != "")
			{
				_ = e.RowHandle;
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_LandedCost_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle != -2147483647)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await RefreshLandedCost();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveLandedCost()
	{
		_ = 1;
		try
		{
			DataTable dataTable = (grd_LandedCostSetup.DataSource as DataTable).Copy();
			if (dataTable.IsNotNull())
			{
				if ((from t in dataTable.AsEnumerable()
					select Function.ToString(t["AlcCode"])).Distinct().Count() != dataTable.Rows.Count)
				{
					WinCommons.Error(Enums.MessageCode.ThisEntryAlreadyExistsInTable, "");
				}
				else
				{
					if (!dataTable.IsNotNull())
					{
						return;
					}
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["AlcCode"]).Length > 2) != null)
					{
						WinCommons.Warning(Enums.MessageCode.TheLengthOfStringEnteredIsLargerThanSpecified, "");
						return;
					}
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrEmpty(Function.ToString(r["AlcCode"]))) != null)
					{
						dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
						dataTable.AcceptChanges();
					}
					if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["AlcCode"]))) != null)
					{
						WinCommons.Error(Enums.MessageCode.AlcCodeCannotBeEmpty, "");
						return;
					}
					for (int num = 0; num < dataTable.Rows.Count; num++)
					{
						if (dataTable.Rows[num]["UserSign"] == DBNull.Value)
						{
							dataTable.Rows[num]["UserSign"] = GlobalData.UserID;
						}
					}
					dataTable.AcceptChanges();
					WinCommons._OpenProcessing();
					HttpResult httpResult = await _httpClient.LandedCostsSetupCLient.SaveLandedCostsSetup(dataTable);
					WinCommons._CloseProcessing();
					if (string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.Success(Enums.MessageCode.Success, "");
						if (SendDataBackToMain != null)
						{
							SendDataBackToMain();
						}
						await RefreshLandedCost();
						FormMode = Enums.FormViewModeType.View;
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
				}
			}
			else
			{
				grd_LandedCostSetup.DataSource = dataTable.Clone();
				grd_LandedCostSetup.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void RemoveLanedCost(string code)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			string text = await _httpClient.LandedCostsSetupCLient.RemoveLandedCostsSetup(code);
			WinCommons._CloseProcessing();
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				gv_LandedCostSetup.BeginUpdate();
				gv_LandedCostSetup.DeleteSelectedRows();
				gv_LandedCostSetup.EndUpdate();
				await RefreshLandedCost();
				if (SendDataBackToMain != null)
				{
					SendDataBackToMain();
				}
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task RefreshLandedCost()
	{
		try
		{
			Task<DataTable> landedCostsSetup = _httpClient.LandedCostsSetupCLient.GetLandedCostsSetup();
			GridControlEx gridControlEx = grd_LandedCostSetup;
			gridControlEx.DataSource = await landedCostsSetup;
			grd_LandedCostSetup.DataSourceTable.AcceptChanges();
			grd_LandedCostSetup.RefreshDataSource();
			AddNewRow();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRow()
	{
		try
		{
			DataTable dataTable = grd_LandedCostSetup.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["AlcCode"] = "";
				dataRow["AlcName"] = "";
				dataRow["OhType"] = 'F';
				dataRow["AcctCode"] = "";
				dataRow["DataSource"] = "";
				dataRow["UserSign"] = 0;
				dataRow["LaCAllcAcc"] = "";
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_LandedCostSetup.DataSource = dataTable;
				grd_LandedCostSetup.RefreshDataSource();
			}
			else
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("OALC");
				DataRow dataRow2 = tableStructure.NewRow();
				dataRow2["AlcCode"] = "";
				dataRow2["AlcName"] = "";
				dataRow2["OhType"] = 'F';
				dataRow2["AcctCode"] = "";
				dataRow2["DataSource"] = "";
				dataRow2["UserSign"] = 0;
				dataRow2["LaCAllcAcc"] = "";
				tableStructure.Rows.Add(dataRow2);
				tableStructure.AcceptChanges();
				grd_LandedCostSetup.DataSource = tableStructure;
				grd_LandedCostSetup.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_LaCAll_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				FindLandCostByCFL();
			}
			if (e.Button.Index == 1)
			{
				string text = Function.ToString(gv_LandedCostSetup.GetFocusedRowCellValue("LaCAllcAcc"));
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oChartOfAccounts, ApzObjectTypes.oChartOfAccounts, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void FindLandCostByCFL()
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ChartOfAccount, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems != null)
			{
				_ = grd_LandedCostSetup.DataSourceTable;
				for (int i = 0; i < frm_CFL2.ChooseItems.Count; i++)
				{
					gv_LandedCostSetup.SetFocusedRowCellValue("LaCAllcAcc", Function.ToString(frm_CFL2.ChooseItems[i]["AcctCode"]));
				}
				grd_LandedCostSetup.DataSourceTable.AcceptChanges();
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_LandedCostsSetup_FormClosing(object sender, FormClosingEventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
		{
			await SaveLandedCost();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.Purchasing.frm_LandedCostsSetup));
		this.grd_LandedCostSetup = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_LC = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.toolStripMenuItem_Remove = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_LandedCostSetup = new Apzon.Controls.Grid.GridViewEx();
		this.col_AlcCode = new Apzon.Controls.Grid.GridColumnEx();
		this.col_AlcName = new Apzon.Controls.Grid.GridColumnEx();
		this.col_OhType = new Apzon.Controls.Grid.GridColumnEx();
		this.lk_OhType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.col_AcctCode = new Apzon.Controls.Grid.GridColumnEx();
		this.col_DataSource = new Apzon.Controls.Grid.GridColumnEx();
		this.col_UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.col_LaCAllcAcc = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_LaCAll = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_LandedCostSetup).BeginInit();
		this.contextMenuStrip_LC.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_LandedCostSetup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.lk_OhType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LaCAll).BeginInit();
		base.SuspendLayout();
		this.grd_LandedCostSetup.AllowPasteExcel = true;
		this.grd_LandedCostSetup.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_LandedCostSetup.ClipboardData = null;
		this.grd_LandedCostSetup.ContextMenuStrip = this.contextMenuStrip_LC;
		this.grd_LandedCostSetup.Location = new System.Drawing.Point(2, 2);
		this.grd_LandedCostSetup.MainView = this.gv_LandedCostSetup;
		this.grd_LandedCostSetup.Name = "grd_LandedCostSetup";
		this.grd_LandedCostSetup.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.lk_OhType, this.rep_LaCAll });
		this.grd_LandedCostSetup.Size = new System.Drawing.Size(710, 380);
		this.grd_LandedCostSetup.TabIndex = 1;
		this.grd_LandedCostSetup.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_LandedCostSetup });
		this.contextMenuStrip_LC.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.contextMenuStrip_LC.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.toolStripMenuItem_Remove });
		this.contextMenuStrip_LC.Name = "contextMenuStrip_LC";
		this.contextMenuStrip_LC.Size = new System.Drawing.Size(118, 26);
		this.toolStripMenuItem_Remove.Name = "toolStripMenuItem_Remove";
		this.toolStripMenuItem_Remove.Size = new System.Drawing.Size(117, 22);
		this.toolStripMenuItem_Remove.Text = "Remove";
		this.toolStripMenuItem_Remove.Click += new System.EventHandler(this.toolStripMenuItem_Remove_Click);
		this.gv_LandedCostSetup.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_LandedCostSetup.ColumnKey = null;
		this.gv_LandedCostSetup.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.col_AlcCode, this.col_AlcName, this.col_OhType, this.col_AcctCode, this.col_DataSource, this.col_UserSign, this.col_LaCAllcAcc });
		this.gv_LandedCostSetup.DisableFormSetting = false;
		this.gv_LandedCostSetup.DisplayException = false;
		this.gv_LandedCostSetup.EditColumn = null;
		this.gv_LandedCostSetup.FieldCurrency = null;
		this.gv_LandedCostSetup.FieldRate = null;
		this.gv_LandedCostSetup.FormId = "oLandedCostSetup";
		this.gv_LandedCostSetup.GridControl = this.grd_LandedCostSetup;
		this.gv_LandedCostSetup.IgnoreFilter = false;
		this.gv_LandedCostSetup.IndexEditRow = -1;
		this.gv_LandedCostSetup.IndicatorWidth = 30;
		this.gv_LandedCostSetup.IsApzTable = false;
		this.gv_LandedCostSetup.IsInsertNewRow = false;
		this.gv_LandedCostSetup.IsLoadingData = false;
		this.gv_LandedCostSetup.IsMappingContextAction = false;
		this.gv_LandedCostSetup.ItemId = "OALC";
		this.gv_LandedCostSetup.LstColumnAllowBatch = null;
		this.gv_LandedCostSetup.LstColumnNotAllowBatch = null;
		this.gv_LandedCostSetup.LstColumnNotEditable = null;
		this.gv_LandedCostSetup.LstColumNotSetting = null;
		this.gv_LandedCostSetup.Name = "gv_LandedCostSetup";
		this.gv_LandedCostSetup.OptionsView.ShowGroupPanel = false;
		this.gv_LandedCostSetup.UseBatchMod = false;
		this.gv_LandedCostSetup.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_LandedCost_CustomDrawRowIndicator);
		this.gv_LandedCostSetup.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_LandedCost_CellValueChanged);
		this.col_AlcCode.Caption = "Code";
		this.col_AlcCode.CurrencyDateField = null;
		this.col_AlcCode.CurrencyField = null;
		this.col_AlcCode.CurrencyFixed = null;
		this.col_AlcCode.CurrencyFixedRate = 0.0;
		this.col_AlcCode.CurrencyRateField = null;
		this.col_AlcCode.FieldName = "AlcCode";
		this.col_AlcCode.Name = "col_AlcCode";
		this.col_AlcCode.PrimaryKey = true;
		this.col_AlcCode.UnitField = null;
		this.col_AlcCode.Visible = true;
		this.col_AlcCode.VisibleIndex = 0;
		this.col_AlcCode.Width = 58;
		this.col_AlcName.Caption = "Name";
		this.col_AlcName.CurrencyDateField = null;
		this.col_AlcName.CurrencyField = null;
		this.col_AlcName.CurrencyFixed = null;
		this.col_AlcName.CurrencyFixedRate = 0.0;
		this.col_AlcName.CurrencyRateField = null;
		this.col_AlcName.FieldName = "AlcName";
		this.col_AlcName.Name = "col_AlcName";
		this.col_AlcName.PrimaryKey = false;
		this.col_AlcName.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.col_AlcName.UnitField = null;
		this.col_AlcName.Visible = true;
		this.col_AlcName.VisibleIndex = 1;
		this.col_AlcName.Width = 107;
		this.col_OhType.Caption = "Allocation By";
		this.col_OhType.ColumnEdit = this.lk_OhType;
		this.col_OhType.CurrencyDateField = null;
		this.col_OhType.CurrencyField = null;
		this.col_OhType.CurrencyFixed = null;
		this.col_OhType.CurrencyFixedRate = 0.0;
		this.col_OhType.CurrencyRateField = null;
		this.col_OhType.FieldName = "OhType";
		this.col_OhType.Name = "col_OhType";
		this.col_OhType.PrimaryKey = false;
		this.col_OhType.UnitField = null;
		this.col_OhType.Visible = true;
		this.col_OhType.VisibleIndex = 2;
		this.col_OhType.Width = 93;
		this.lk_OhType.AutoHeight = false;
		this.lk_OhType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.lk_OhType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.lk_OhType.DisplayMember = "Name";
		this.lk_OhType.Name = "lk_OhType";
		this.lk_OhType.NullText = "";
		this.lk_OhType.ValueMember = "Code";
		this.col_AcctCode.Caption = "AcctCode";
		this.col_AcctCode.CurrencyDateField = null;
		this.col_AcctCode.CurrencyField = null;
		this.col_AcctCode.CurrencyFixed = null;
		this.col_AcctCode.CurrencyFixedRate = 0.0;
		this.col_AcctCode.CurrencyRateField = null;
		this.col_AcctCode.FieldName = "AcctCode";
		this.col_AcctCode.Name = "col_AcctCode";
		this.col_AcctCode.PrimaryKey = false;
		this.col_AcctCode.UnitField = null;
		this.col_DataSource.Caption = "DataSource";
		this.col_DataSource.CurrencyDateField = null;
		this.col_DataSource.CurrencyField = null;
		this.col_DataSource.CurrencyFixed = null;
		this.col_DataSource.CurrencyFixedRate = 0.0;
		this.col_DataSource.CurrencyRateField = null;
		this.col_DataSource.FieldName = "DataSource";
		this.col_DataSource.Name = "col_DataSource";
		this.col_DataSource.PrimaryKey = false;
		this.col_DataSource.UnitField = null;
		this.col_UserSign.Caption = "UserSign";
		this.col_UserSign.CurrencyDateField = null;
		this.col_UserSign.CurrencyField = null;
		this.col_UserSign.CurrencyFixed = null;
		this.col_UserSign.CurrencyFixedRate = 0.0;
		this.col_UserSign.CurrencyRateField = null;
		this.col_UserSign.FieldName = "UserSign";
		this.col_UserSign.Name = "col_UserSign";
		this.col_UserSign.PrimaryKey = false;
		this.col_UserSign.UnitField = null;
		this.col_LaCAllcAcc.Caption = "Landed Costs Alloc. Account";
		this.col_LaCAllcAcc.ColumnEdit = this.rep_LaCAll;
		this.col_LaCAllcAcc.CurrencyDateField = null;
		this.col_LaCAllcAcc.CurrencyField = null;
		this.col_LaCAllcAcc.CurrencyFixed = null;
		this.col_LaCAllcAcc.CurrencyFixedRate = 0.0;
		this.col_LaCAllcAcc.CurrencyRateField = null;
		this.col_LaCAllcAcc.FieldName = "LaCAllcAcc";
		this.col_LaCAllcAcc.Name = "col_LaCAllcAcc";
		this.col_LaCAllcAcc.PrimaryKey = false;
		this.col_LaCAllcAcc.UnitField = null;
		this.col_LaCAllcAcc.Visible = true;
		this.col_LaCAllcAcc.VisibleIndex = 3;
		this.col_LaCAllcAcc.Width = 124;
		this.rep_LaCAll.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_LaCAll.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_LaCAll.Name = "rep_LaCAll";
		this.rep_LaCAll.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_LaCAll_ButtonClick);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(80, 387);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 14;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(2, 387);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 13;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(714, 414);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.grd_LandedCostSetup);
		base.FormId = "oLandedCostSetup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_LandedCostsSetup";
		this.Text = "Landed Costs - Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_LandedCostsSetup_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_LandedCostSetup).EndInit();
		this.contextMenuStrip_LC.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_LandedCostSetup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.lk_OhType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LaCAll).EndInit();
		base.ResumeLayout(false);
	}
}
