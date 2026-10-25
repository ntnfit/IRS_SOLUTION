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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Administrator;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.ItemsMaster;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.Item_Management.Batches;

public class frm_BatchDetail : XtraFormEx
{
	private const string FormController = "BatchDetail";

	private string _privilege = "";

	private Dictionary<string, string> lstStatus = new Dictionary<string, string>
	{
		{ "0", "Released" },
		{ "1", "Not Accessible" },
		{ "2", "Locked" }
	};

	private bool _isLoading;

	private bool _isChangingFocusedRow;

	private DataTable _dtCurrent = new DataTable();

	public int _drillDownAbsEntry = -1;

	private IContainer components;

	private LabelLine labelLine1;

	private LabelLine labelLine3;

	private GridControlEx grd_Quantities;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btn_Cancel;

	private ButtonEditEx btn_ItemCode;

	private ButtonEditEx btn_ItemName;

	private SimpleButtonEx btn_Search;

	private GridColumnEx ItemCode;

	private GridColumnEx ItemName;

	private GridColumnEx OnHand;

	private GridColumnEx OnOrder;

	private GridColumnEx IsCommited;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Batch;

	private GroupControl groupControl2;

	private GroupControl groupControl1;

	private RepositoryItemDateEditEx rep_Date;

	private RepositoryItemButtonEdit rep_WhsCode;

	private LabelLine labelLine2;

	private TextEditEx txt_Batch;

	private GridViewEx gv_Quantities;

	private GridViewEx gv_Batch;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem batchToolStripMenuItem;

	private RepositoryItemLookUpEdit rep_Status;

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

	private RepositoryItemCalcEdit rep_Calc;

	private RepositoryItemButtonEdit rep_ItemCode;

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
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_BatchDetail()
	{
		InitializeComponent();
		OnLoadAsync += frm_BatchDetail_Load;
	}

	private async Task frm_BatchDetail_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx> { ["OBTN"] = gv_Batch };
			await WinFuntions.LoadUdfColumns(gv_Batch, "OBTN");
			List<Formular> formulars = await WinFuntions.GetFormulas(10000044);
			gv_Batch.InitFormular(formulars, tblFormular);
			LoadComboBox();
			if (_drillDownAbsEntry != -1)
			{
				await LoadAnEntry(_drillDownAbsEntry);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Batch_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (_isLoading)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update && !_isChangingFocusedRow)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					if (grd_Batch.DataSource is DataTable && _dtCurrent != null && _dtCurrent.Rows.Count > 0)
					{
						_isLoading = true;
						DataRow dataRow = _dtCurrent.Rows[gv_Batch.GetDataSourceRowIndex(e.PrevFocusedRowHandle)];
						gv_Batch.SetRowCellValue(e.PrevFocusedRowHandle, "Status", Function.ToString(dataRow["Status"]));
						gv_Batch.SetRowCellValue(e.PrevFocusedRowHandle, "MnfSerial", Function.ToString(dataRow["MnfSerial"]));
						gv_Batch.SetRowCellValue(e.PrevFocusedRowHandle, "LotNumber", Function.ToString(dataRow["LotNumber"]));
						gv_Batch.SetRowCellValue(e.PrevFocusedRowHandle, "InDate", Function.ParseDateTime(dataRow["InDate"]));
						gv_Batch.SetRowCellValue(e.PrevFocusedRowHandle, "MnfDate", Function.ParseDateTime(dataRow["MnfDate"]));
						gv_Batch.SetRowCellValue(e.PrevFocusedRowHandle, "ExpDate", Function.ParseDateTime(dataRow["ExpDate"]));
						gv_Batch.SetRowCellValue(e.PrevFocusedRowHandle, "Notes", Function.ToString(dataRow["Notes"]));
						_isLoading = false;
					}
					LoadQuantities();
					FormMode = Enums.FormViewModeType.View;
					_isChangingFocusedRow = false;
				}
				else
				{
					_isChangingFocusedRow = true;
					gv_Batch.FocusedRowHandle = e.PrevFocusedRowHandle;
				}
			}
			else
			{
				LoadQuantities();
				_isChangingFocusedRow = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Batch_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && !_isLoading)
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

	private void gv_Quantities_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View && !_isLoading)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Batch_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Items_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Batch_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Quantities_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Batch_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Batch.FocusedColumn.OptionsColumn.ReadOnly && gv_Batch.FocusedColumn.FieldName != "ItemCode")
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

	private void gv_Quantities_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Quantities.FocusedColumn.OptionsColumn.ReadOnly && gv_Quantities.FocusedColumn.FieldName != "WhsCode")
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

	private void rep_WhsCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Quantities.GetFocusedRowCellValue("WhsCode"));
			if (!string.IsNullOrEmpty(text))
			{
				frm_WareHouses frm_WareHouses2 = new frm_WareHouses();
				frm_WareHouses2._whsSearchCode = text;
				frm_WareHouses2.MdiParent = MdiParent;
				frm_WareHouses2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		EnableDisableControls(p: false);
		WinCommons._OpenProcessing();
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (grd_Batch.DataSource is DataTable dataTable && dataTable.Rows.Count > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataTable dataTable2 = dataTable.Clone();
				dataTable2.ImportRow(gv_Batch.GetFocusedDataRow());
				dataTable2.TableName = "Btch";
				dataTable2.AcceptChanges();
				DataSet dataSet = dataTable2.DataSet;
				if (dataSet != null)
				{
					dataSet.Tables.Remove("Btch");
					dataSet.AcceptChanges();
				}
				DataTable dataTable3 = grd_Quantities.DataSource as DataTable;
				if (dataTable3 == null)
				{
					dataTable3 = new DataTable();
				}
				dataTable3.TableName = "Qty";
				dataTable3.AcceptChanges();
				DataSet dataSet2 = dataTable3.DataSet;
				if (dataSet2 != null)
				{
					dataSet2.Tables.Remove("Qty");
					dataSet2.AcceptChanges();
				}
				DataSet dataSet3 = new DataSet();
				dataSet3.Tables.Clear();
				dataSet3.Tables.AddRange(new DataTable[2] { dataTable2, dataTable3 });
				dataSet3.AcceptChanges();
				if (dataTable.IsNotNull())
				{
					using HttpClientHelper<string> _helper = new HttpClientHelper<string>(Configuration.ApiServer, "BatchDetail");
					string text = _helper.Post("Save", dataSet3);
					if (string.IsNullOrEmpty(text))
					{
						await Search();
						WinCommons.Success(Enums.MessageCode.Success, "");
						FormMode = Enums.FormViewModeType.View;
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
		EnableDisableControls(p: true);
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_drillDownAbsEntry = -1;
		EnableDisableControls(p: false);
		try
		{
			await Search();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControls(p: true);
	}

	private void batchToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			string id = Function.ToString(gv_Batch.GetFocusedRowCellValue("DistNumber"));
			this.DrillDown(ApzObjectTypes.oBatchDetailsTransactionReport, ApzObjectTypes.oBatchDetailsTransactionReport, id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_ItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Items, false, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				btn_ItemCode.EditValue = frm_CFL2.ChooseItem["ItemCode"];
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void rep_ItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Batch.GetFocusedRowCellValue("ItemCode"));
			if (!string.IsNullOrWhiteSpace(text))
			{
				frm_ItemMasterData frm_ItemMasterData2 = new frm_ItemMasterData(flag: false, flagSearchFrm: true, text);
				frm_ItemMasterData2.MdiParent = MdiParent;
				frm_ItemMasterData2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Search()
	{
		_isLoading = true;
		WinCommons._OpenProcessing();
		try
		{
			string itemCode = Function.ToString(btn_ItemCode.Text);
			string itemName = Function.ToString(btn_ItemName.Text);
			string batch = Function.ToString(txt_Batch.Text);
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchDetail");
			grd_Quantities.DataSource = new DataTable();
			grd_Quantities.RefreshDataSource();
			DataTable dataTable = await _helper.GetAsync("Search", new { itemCode, itemName, batch });
			grd_Batch.DataSource = dataTable;
			grd_Batch.RefreshDataSource();
			if (dataTable != null)
			{
				_dtCurrent = dataTable.Copy();
				_dtCurrent.AcceptChanges();
			}
			else
			{
				_dtCurrent = null;
			}
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				LoadQuantities();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
		_isLoading = false;
	}

	private void LoadComboBox()
	{
		List<Common.Table_Code> dataSource = lstStatus.Select((KeyValuePair<string, string> item) => new Common.Table_Code
		{
			Code = item.Key,
			Name = item.Value
		}).ToList();
		rep_Status.DataSource = dataSource;
	}

	private void EnableDisableControls(bool p)
	{
		try
		{
			btn_Cancel.Enabled = p;
			btn_Save.Enabled = p;
			btn_Search.Enabled = p;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadQuantities()
	{
		try
		{
			string text = Function.ToString(gv_Batch.GetFocusedRowCellValue("ItemCode"));
			int sysNumber = Function.ParseInt(gv_Batch.GetFocusedRowCellValue("SysNumber"));
			if (!string.IsNullOrEmpty(text))
			{
				using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchDetail"))
				{
					DataTable dataSource = httpClientHelper.Get("GetQuantities", new
					{
						itemCode = text,
						sysNumber = sysNumber
					});
					grd_Quantities.DataSource = dataSource;
					grd_Quantities.RefreshDataSource();
					return;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadAnEntry(int absEntry)
	{
		try
		{
			using HttpClientHelper<DataTable> _helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BatchDetail");
			DataTable dataSource = await _helper.GetAsync("LoadAnEntry", new { absEntry });
			grd_Batch.DataSource = dataSource;
			grd_Batch.RefreshDataSource();
			LoadQuantities();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			grd_Batch.DataSource = _dtCurrent.Copy();
			grd_Batch.RefreshDataSource();
			LoadQuantities();
			FormMode = Enums.FormViewModeType.View;
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
		this.components = new System.ComponentModel.Container();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Item_Management.Batches.frm_BatchDetail));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.grd_Quantities = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Quantities = new Apzon.Controls.Grid.GridViewEx();
		this.ItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_WhsCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.ItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Calc = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.OnHand = new Apzon.Controls.Grid.GridColumnEx();
		this.OnOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.IsCommited = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_ItemCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_ItemName = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.groupControl2 = new DevExpress.XtraEditors.GroupControl();
		this.grd_Batch = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.batchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Batch = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Date = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.groupControl1 = new DevExpress.XtraEditors.GroupControl();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Batch = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Quantities).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Quantities).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WhsCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Calc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.groupControl2).BeginInit();
		this.groupControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).BeginInit();
		this.groupControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Batch.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Item Number";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(16, 16);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(73, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Item Number";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Item Descripton";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(227, 16);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(82, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 4;
		this.labelLine3.Utext = "Item Descripton";
		this.grd_Quantities.AllowPasteExcel = true;
		this.grd_Quantities.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Quantities.ClipboardData = null;
		this.grd_Quantities.Location = new System.Drawing.Point(1, 23);
		this.grd_Quantities.MainView = this.gv_Quantities;
		this.grd_Quantities.Name = "grd_Quantities";
		this.grd_Quantities.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_WhsCode, this.rep_Calc });
		this.grd_Quantities.Size = new System.Drawing.Size(788, 175);
		this.grd_Quantities.TabIndex = 6;
		this.grd_Quantities.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Quantities });
		this.gv_Quantities.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Quantities.ColumnKey = null;
		this.gv_Quantities.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.ItemCode, this.ItemName, this.OnHand, this.OnOrder, this.IsCommited });
		this.gv_Quantities.DisableFormSetting = false;
		this.gv_Quantities.DisplayException = false;
		this.gv_Quantities.EditColumn = null;
		this.gv_Quantities.FieldCurrency = null;
		this.gv_Quantities.FieldRate = null;
		this.gv_Quantities.FormId = "oBatchDetails";
		this.gv_Quantities.GridControl = this.grd_Quantities;
		this.gv_Quantities.IgnoreFilter = false;
		this.gv_Quantities.IndexEditRow = -1;
		this.gv_Quantities.IndicatorWidth = 40;
		this.gv_Quantities.IsApzTable = false;
		this.gv_Quantities.IsInsertNewRow = false;
		this.gv_Quantities.IsLoadingData = false;
		this.gv_Quantities.IsMappingContextAction = false;
		this.gv_Quantities.ItemId = "OBTQ";
		this.gv_Quantities.LstColumnAllowBatch = null;
		this.gv_Quantities.LstColumnNotAllowBatch = null;
		this.gv_Quantities.LstColumnNotEditable = null;
		this.gv_Quantities.LstColumNotSetting = null;
		this.gv_Quantities.Name = "gv_Quantities";
		this.gv_Quantities.OptionsView.ShowGroupPanel = false;
		this.gv_Quantities.UseBatchMod = false;
		this.gv_Quantities.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Items_CustomDrawRowIndicator);
		this.gv_Quantities.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Quantities_RowCellStyle);
		this.gv_Quantities.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Quantities_ShowingEditor);
		this.gv_Quantities.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Quantities_CellValueChanged);
		this.ItemCode.Caption = "Whs Code";
		this.ItemCode.ColumnEdit = this.rep_WhsCode;
		this.ItemCode.CurrencyDateField = null;
		this.ItemCode.CurrencyField = null;
		this.ItemCode.CurrencyFixed = null;
		this.ItemCode.CurrencyFixedRate = 0.0;
		this.ItemCode.CurrencyRateField = null;
		this.ItemCode.FieldName = "WhsCode";
		this.ItemCode.Name = "ItemCode";
		this.ItemCode.OptionsColumn.ReadOnly = true;
		this.ItemCode.PrimaryKey = true;
		this.ItemCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.ItemCode.UnitField = null;
		this.ItemCode.Visible = true;
		this.ItemCode.VisibleIndex = 0;
		this.ItemCode.Width = 150;
		this.rep_WhsCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_WhsCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_WhsCode.Name = "rep_WhsCode";
		this.rep_WhsCode.ReadOnly = true;
		this.rep_WhsCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_WhsCode_ButtonClick);
		this.ItemName.Caption = "Existing Qty";
		this.ItemName.ColumnEdit = this.rep_Calc;
		this.ItemName.CurrencyDateField = null;
		this.ItemName.CurrencyField = null;
		this.ItemName.CurrencyFixed = null;
		this.ItemName.CurrencyFixedRate = 0.0;
		this.ItemName.CurrencyRateField = null;
		this.ItemName.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.ItemName.FieldName = "Quantity";
		this.ItemName.Name = "ItemName";
		this.ItemName.OptionsColumn.ReadOnly = true;
		this.ItemName.PrimaryKey = false;
		this.ItemName.UnitField = null;
		this.ItemName.Visible = true;
		this.ItemName.VisibleIndex = 1;
		this.ItemName.Width = 127;
		this.rep_Calc.AutoHeight = false;
		this.rep_Calc.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Calc.Name = "rep_Calc";
		this.OnHand.Caption = "Allocated Qty";
		this.OnHand.ColumnEdit = this.rep_Calc;
		this.OnHand.CurrencyDateField = null;
		this.OnHand.CurrencyField = null;
		this.OnHand.CurrencyFixed = null;
		this.OnHand.CurrencyFixedRate = 0.0;
		this.OnHand.CurrencyRateField = null;
		this.OnHand.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.OnHand.FieldName = "CommitQty";
		this.OnHand.Name = "OnHand";
		this.OnHand.OptionsColumn.ReadOnly = true;
		this.OnHand.PrimaryKey = false;
		this.OnHand.UnitField = null;
		this.OnHand.Visible = true;
		this.OnHand.VisibleIndex = 2;
		this.OnHand.Width = 146;
		this.OnOrder.Caption = "Availabe Qty";
		this.OnOrder.ColumnEdit = this.rep_Calc;
		this.OnOrder.CurrencyDateField = null;
		this.OnOrder.CurrencyField = null;
		this.OnOrder.CurrencyFixed = null;
		this.OnOrder.CurrencyFixedRate = 0.0;
		this.OnOrder.CurrencyRateField = null;
		this.OnOrder.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.OnOrder.FieldName = "AvailQty";
		this.OnOrder.Name = "OnOrder";
		this.OnOrder.OptionsColumn.ReadOnly = true;
		this.OnOrder.PrimaryKey = false;
		this.OnOrder.UnitField = null;
		this.OnOrder.Visible = true;
		this.OnOrder.VisibleIndex = 3;
		this.OnOrder.Width = 158;
		this.IsCommited.Caption = "Location";
		this.IsCommited.CurrencyDateField = null;
		this.IsCommited.CurrencyField = null;
		this.IsCommited.CurrencyFixed = null;
		this.IsCommited.CurrencyFixedRate = 0.0;
		this.IsCommited.CurrencyRateField = null;
		this.IsCommited.FieldName = "Location";
		this.IsCommited.Name = "IsCommited";
		this.IsCommited.PrimaryKey = false;
		this.IsCommited.UnitField = null;
		this.IsCommited.Visible = true;
		this.IsCommited.VisibleIndex = 4;
		this.IsCommited.Width = 165;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(6, 454);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(81, 20);
		this.btn_Save.TabIndex = 8;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btn_Cancel.IsSearchControl = true;
		this.btn_Cancel.Location = new System.Drawing.Point(90, 454);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(87, 20);
		this.btn_Cancel.TabIndex = 9;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_ItemCode.EditControl = null;
		this.btn_ItemCode.FieldName = null;
		this.btn_ItemCode.IsSearchControl = true;
		this.btn_ItemCode.Location = new System.Drawing.Point(89, 13);
		this.btn_ItemCode.Name = "btn_ItemCode";
		this.btn_ItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.btn_ItemCode.SettingInVisible = false;
		this.btn_ItemCode.SettingReadonly = false;
		this.btn_ItemCode.Size = new System.Drawing.Size(128, 20);
		this.btn_ItemCode.TabIndex = 1;
		this.btn_ItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_ItemCode_ButtonClick);
		this.btn_ItemName.EditControl = null;
		this.btn_ItemName.FieldName = null;
		this.btn_ItemName.IsSearchControl = true;
		this.btn_ItemName.Location = new System.Drawing.Point(309, 13);
		this.btn_ItemName.Name = "btn_ItemName";
		this.btn_ItemName.SettingInVisible = false;
		this.btn_ItemName.SettingReadonly = false;
		this.btn_ItemName.Size = new System.Drawing.Size(171, 20);
		this.btn_ItemName.TabIndex = 5;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = true;
		this.btn_Search.Location = new System.Drawing.Point(697, 13);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(81, 20);
		this.btn_Search.TabIndex = 15;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(6, 42);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.groupControl2);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.groupControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(792, 399);
		this.splitContainerControl1.SplitterPosition = 203;
		this.splitContainerControl1.TabIndex = 8;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.groupControl2.Appearance.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
		this.groupControl2.Appearance.Options.UseFont = true;
		this.groupControl2.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.groupControl2.AppearanceCaption.Options.UseFont = true;
		this.groupControl2.Controls.Add(this.grd_Batch);
		this.groupControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl2.Location = new System.Drawing.Point(0, 0);
		this.groupControl2.Name = "groupControl2";
		this.groupControl2.Size = new System.Drawing.Size(792, 203);
		this.groupControl2.TabIndex = 0;
		this.groupControl2.Text = "Batch Details";
		this.grd_Batch.AllowPasteExcel = true;
		this.grd_Batch.ClipboardData = null;
		this.grd_Batch.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Batch.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Batch.Location = new System.Drawing.Point(2, 23);
		this.grd_Batch.MainView = this.gv_Batch;
		this.grd_Batch.Name = "grd_Batch";
		this.grd_Batch.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_Date, this.rep_Status, this.rep_ItemCode });
		this.grd_Batch.Size = new System.Drawing.Size(788, 178);
		this.grd_Batch.TabIndex = 7;
		this.grd_Batch.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Batch });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.batchToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(258, 26);
		this.batchToolStripMenuItem.Name = "batchToolStripMenuItem";
		this.batchToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
		this.batchToolStripMenuItem.Text = "Batch Number Transactions Report";
		this.batchToolStripMenuItem.Click += new System.EventHandler(this.batchToolStripMenuItem_Click);
		this.gv_Batch.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Batch.ColumnKey = null;
		this.gv_Batch.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumn11
		});
		this.gv_Batch.DisableFormSetting = false;
		this.gv_Batch.DisplayException = false;
		this.gv_Batch.EditColumn = null;
		this.gv_Batch.FieldCurrency = null;
		this.gv_Batch.FieldRate = null;
		this.gv_Batch.FormId = "oBatchDetails";
		this.gv_Batch.GridControl = this.grd_Batch;
		this.gv_Batch.IgnoreFilter = false;
		this.gv_Batch.IndexEditRow = -1;
		this.gv_Batch.IndicatorWidth = 80;
		this.gv_Batch.IsApzTable = false;
		this.gv_Batch.IsInsertNewRow = false;
		this.gv_Batch.IsLoadingData = false;
		this.gv_Batch.IsMappingContextAction = false;
		this.gv_Batch.ItemId = "OBTN";
		this.gv_Batch.LstColumnAllowBatch = null;
		this.gv_Batch.LstColumnNotAllowBatch = null;
		this.gv_Batch.LstColumnNotEditable = null;
		this.gv_Batch.LstColumNotSetting = null;
		this.gv_Batch.Name = "gv_Batch";
		this.gv_Batch.OptionsView.ColumnAutoWidth = false;
		this.gv_Batch.OptionsView.ShowGroupPanel = false;
		this.gv_Batch.UseBatchMod = false;
		this.gv_Batch.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Batch_CustomDrawRowIndicator);
		this.gv_Batch.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Batch_RowCellStyle);
		this.gv_Batch.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Batch_ShowingEditor);
		this.gv_Batch.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Batch_FocusedRowChanged);
		this.gv_Batch.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Batch_CellValueChanged);
		this.gridColumn1.Caption = "Item Number";
		this.gridColumn1.ColumnEdit = this.rep_ItemCode;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "ItemCode";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn1.Width = 150;
		this.rep_ItemCode.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_ItemCode.Name = "rep_ItemCode";
		this.rep_ItemCode.ReadOnly = true;
		this.rep_ItemCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemCode_ButtonPressed);
		this.gridColumn2.Caption = "Item Description";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "itemName";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 1;
		this.gridColumn2.Width = 150;
		this.gridColumn3.Caption = "Status";
		this.gridColumn3.ColumnEdit = this.rep_Status;
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Status";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 2;
		this.gridColumn3.Width = 60;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[1]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Status.DisplayMember = "Name";
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.rep_Status.ShowFooter = false;
		this.rep_Status.ShowHeader = false;
		this.rep_Status.ShowLines = false;
		this.rep_Status.ValueMember = "Code";
		this.gridColumn4.Caption = "Batch";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "DistNumber";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 3;
		this.gridColumn4.Width = 150;
		this.gridColumn5.Caption = "Batch Attribute 1";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "MnfSerial";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 4;
		this.gridColumn5.Width = 150;
		this.gridColumn6.Caption = "Batch Attribute 2";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "LotNumber";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 5;
		this.gridColumn6.Width = 150;
		this.gridColumn7.Caption = "Admission Date";
		this.gridColumn7.ColumnEdit = this.rep_Date;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "InDate";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 6;
		this.gridColumn7.Width = 150;
		this.rep_Date.AutoHeight = false;
		this.rep_Date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.IsSearchControl = false;
		this.rep_Date.Name = "rep_Date";
		this.gridColumn8.Caption = "Manufacturing Date";
		this.gridColumn8.ColumnEdit = this.rep_Date;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "MnfDate";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 7;
		this.gridColumn8.Width = 150;
		this.gridColumn9.Caption = "Expiration Date";
		this.gridColumn9.ColumnEdit = this.rep_Date;
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "ExpDate";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 8;
		this.gridColumn9.Width = 150;
		this.gridColumn10.Caption = "Details";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "Notes";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 9;
		this.gridColumn10.Width = 150;
		this.gridColumn11.Caption = "System Number";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "SysNumber";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.OptionsColumn.ReadOnly = true;
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 10;
		this.gridColumn11.Width = 150;
		this.groupControl1.AppearanceCaption.Font = new System.Drawing.Font("Tahoma", 8.25f, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, 0);
		this.groupControl1.AppearanceCaption.Options.UseFont = true;
		this.groupControl1.Controls.Add(this.grd_Quantities);
		this.groupControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.groupControl1.Location = new System.Drawing.Point(0, 0);
		this.groupControl1.Name = "groupControl1";
		this.groupControl1.Size = new System.Drawing.Size(792, 186);
		this.groupControl1.TabIndex = 0;
		this.groupControl1.Text = "Batch Details Quantities";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Batch";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(489, 16);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(48, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 16;
		this.labelLine2.Utext = "Batch";
		this.txt_Batch.EditControl = null;
		this.txt_Batch.FieldName = null;
		this.txt_Batch.IsSearchControl = true;
		this.txt_Batch.Location = new System.Drawing.Point(533, 13);
		this.txt_Batch.Name = "txt_Batch";
		this.txt_Batch.SettingInVisible = false;
		this.txt_Batch.SettingReadonly = false;
		this.txt_Batch.Size = new System.Drawing.Size(148, 20);
		this.txt_Batch.TabIndex = 17;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(801, 479);
		base.Controls.Add(this.txt_Batch);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.btn_ItemCode);
		base.Controls.Add(this.btn_ItemName);
		base.FormId = "oBatchDetails";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_BatchDetail.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BatchDetail";
		base.ShowInTaskbar = false;
		this.Text = "Batch Detail";
		((System.ComponentModel.ISupportInitialize)this.grd_Quantities).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Quantities).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WhsCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Calc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.groupControl2).EndInit();
		this.groupControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Batch).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Batch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.groupControl1).EndInit();
		this.groupControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Batch.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
