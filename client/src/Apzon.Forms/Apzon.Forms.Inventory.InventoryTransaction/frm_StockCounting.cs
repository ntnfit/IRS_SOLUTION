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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_StockCounting : XtraFormEx
{
	private const string OkString = "OK";

	private const string FinString = "Find";

	private const string UpdateString = "Update";

	private const string AddString = "Add";

	private const string Controller = "InventoryCounting";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public DataTable _dtClone = new DataTable();

	public string _whsCode = "";

	private IContainer components;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private DateEditEx date_CountDate;

	private TimeEdit time_CountTime;

	private LookUpEdit look_Status;

	private LookUpEdit look_No;

	private TextEditEx txt_Ref;

	private GridControlEx grid_InventoryCount;

	private GridViewEx gv_InventoryCount;

	private LabelLine lab_WareHouse;

	private LabelLine lab_Items;

	private TextEditEx txt_No;

	private TextEditEx txt_Item;

	private GridColumnEx ItemNo;

	private GridColumnEx ItemDescript;

	private GridColumnEx Whse;

	private GridColumnEx InWhsQty;

	private GridColumnEx Counted;

	private GridColumnEx UomCountedQty;

	private GridColumnEx CountedQty;

	private GridColumnEx Variance;

	private GridColumnEx VarianceP;

	private GridColumnEx UomCode;

	private ButtonEditEx txt_WareHouse;

	private RepositoryItemCheckEdit rep_Counted;

	private TextEditEx text_DocTry;

	private ContextMenuStrip contextMenu_InventoryCount;

	private ToolStripMenuItem closeToolStripMenuItem;

	private ToolStripMenuItem saveAsDraftToolStripMenuItem;

	private SimpleButtonEx btn_Ok;

	private SimpleButtonEx btn_Cancel;

	private RepositoryItemButtonEditCFL btn_ItemCode;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private GridColumnEx gridColumnEx7;

	private RepositoryItemButtonEditCFL btn_WhsLine;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemLookUpEdit luk_UomCode;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
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
				look_Status.EditValue = "O";
				EnableDisableControl(isEnable: true);
				btn_Ok.Text = "Add";
				break;
			case Enums.FormViewModeType.Update:
				EnableDisableControl(isEnable: true);
				btn_Ok.Text = "Update";
				break;
			case Enums.FormViewModeType.Find:
				EnableDisableControl(isEnable: true);
				btn_Ok.Text = "Find";
				SetDataDoc(null);
				txt_Item.ReadOnly = true;
				look_Status.ReadOnly = false;
				grid_InventoryCount.DataSource = null;
				break;
			case Enums.FormViewModeType.View:
				EnableDisableControl(isEnable: true);
				btn_Ok.Text = "OK";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public frm_StockCounting()
	{
		InitializeComponent();
	}

	public frm_StockCounting(DataTable dtClone, string whsCode)
		: this()
	{
		_dtClone = dtClone;
		grid_InventoryCount.DataSource = _dtClone;
		_whsCode = whsCode;
		txt_WareHouse.EditValue = _whsCode;
		txt_Item.Focus();
	}

	private async Task frm_InventoryCounting_Load(object sender, EventArgs e)
	{
		InitChangeDataEvent();
		string dateTimeFormat = GlobalData.DateTimeFormat;
		txt_Ref.Text = "";
		date_CountDate.Properties.DisplayFormat.FormatType = FormatType.DateTime;
		date_CountDate.Properties.DisplayFormat.FormatString = dateTimeFormat;
		date_CountDate.Properties.EditFormat.FormatType = FormatType.DateTime;
		date_CountDate.Properties.EditFormat.FormatString = dateTimeFormat;
		date_CountDate.Properties.Mask.MaskType = MaskType.DateTime;
		date_CountDate.Properties.Mask.EditMask = dateTimeFormat;
		SetDataStaus();
		SetDataUomCode();
		look_Status.EditValue = "O";
		date_CountDate.EditValue = DateTime.Now.Date;
		time_CountTime.EditValue = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
		btn_ItemCode.ValueMember = "ItemCode";
		btn_ItemCode.FindCFLBy(Enums.TypeCFL.ListItems, false, "", Enums.ItemObjectType.NoItemType, null);
		RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = btn_ItemCode;
		repositoryItemButtonEditCFL.DrillDownMethod = (Action<string>)Delegate.Combine(repositoryItemButtonEditCFL.DrillDownMethod, (Action<string>)((string input) =>
		{
			this.DrillDown(ApzObjectTypes.oItems, input);
		}));
		btn_WhsLine.ValueMember = "WhsCode";
		btn_WhsLine.TypeCFL = Enums.TypeCFL.Warehouse;
		btn_WhsLine.RefreshDataSource();
		btn_WhsLine.TypeDrillDown = ApzObjectTypes.oWarehouses;
		await SetDataNo();
		TextEditEx textEditEx = txt_No;
		textEditEx.EditValue = await GetNextNumber();
		if (!string.IsNullOrEmpty(Function.ToString(txt_WareHouse.EditValue)))
		{
			txt_Item.Focus();
		}
		else
		{
			txt_WareHouse.Focus();
		}
		_ = grid_InventoryCount.DataSourceTable;
		if (Function.ParseInt(text_DocTry.EditValue) > 0)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		else
		{
			FormMode = Enums.FormViewModeType.Add;
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Add:
				await Save(1);
				break;
			case Enums.FormViewModeType.Update:
				await Save(2);
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click_1(object sender, EventArgs e)
	{
		if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
		{
			Close();
		}
	}

	private async void txt_WareHouse_KeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			DataTable dataSource = await SearchDataByWarehouse(txt_WareHouse.Text);
			grid_InventoryCount.DataSource = dataSource;
			txt_Item.Focus();
		}
	}

	private async void txt_Item_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.Return)
			{
				return;
			}
			DataTable dataTable = await SearchDataByItemCode(txt_WareHouse.Text, txt_Item.Text);
			DataTable dataSourceTable = grid_InventoryCount.DataSourceTable;
			dataSourceTable.NewRow();
			string text = "";
			if (dataTable.Rows.Count > 0)
			{
				foreach (DataRow row in dataTable.Rows)
				{
					foreach (DataRow row2 in dataSourceTable.Rows)
					{
						if (Function.ToString(row2["ItemCode"]) == Function.ToString(row["ItemCode"]))
						{
							row2["CountQty"] = Function.ParseDouble(row2["CountQty"]) + 1.0;
							row2["Difference"] = Function.ParseDouble(row2["CountQty"]) - Function.ParseDouble(row2["InWhsQty"]);
							row2["DiffPercen"] = Function.ParseDouble(row2["Difference"]) / Function.ParseDouble(row2["CountQty"]) * 100.0;
							text = Function.ToString(row["ItemCode"]);
							if (!string.Equals(Function.ToString(row2["Counted"]), "Y"))
							{
								row2["Counted"] = "Y";
							}
						}
					}
				}
				int num = gv_InventoryCount.LocateByValue("ItemCode", Function.ToString(text));
				if (num != int.MinValue)
				{
					gv_InventoryCount.FocusedRowHandle = num;
					gv_InventoryCount.MakeRowVisible(num);
				}
			}
			else
			{
				XtraMessageBox.Show("No matching record found", "Warning", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			txt_Item.EditValue = "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle >= 0)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_WareHouse_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string txtText = Function.ToString(txt_WareHouse.EditValue);
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Warehouse, multiSelect: false, txtText);
			frm_ChooseFromList2.ShowDialog(this);
			DataRow dataRow = (DataRow)frm_ChooseFromList2.ChooseItem;
			if (dataRow != null)
			{
				txt_WareHouse.EditValue = Function.ToString(dataRow["WhsCode"]);
				txt_WareHouse.Focus();
				SendKeys.SendWait("{ENTER}");
			}
			_ = GlobalData.DateTimeFormat;
			_ = GlobalData.GeneralSetting.TimeFormat;
			_ = GlobalData.GeneralSetting.QtyDec;
			_ = GlobalData.GeneralSetting.RateDec;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_InventoryCount_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "CountQty")
			{
				double num = Function.ParseDouble(gv_InventoryCount.GetRowCellValue(e.RowHandle, "CountQty")) - Function.ParseDouble(gv_InventoryCount.GetRowCellValue(e.RowHandle, "InWhsQty"));
				gv_InventoryCount.SetRowCellValue(e.RowHandle, "Difference", num);
				double num2 = Function.ParseDouble(gv_InventoryCount.GetRowCellValue(e.RowHandle, "Difference")) / Function.ParseDouble(gv_InventoryCount.GetRowCellValue(e.RowHandle, "CountQty")) * 100.0;
				gv_InventoryCount.SetRowCellValue(e.RowHandle, "DiffPercen", num2);
				if (!string.Equals(Function.ToString(gv_InventoryCount.GetRowCellValue(e.RowHandle, "Counted")), "Y"))
				{
					gv_InventoryCount.SetRowCellValue(e.RowHandle, "Counted", "Y");
				}
			}
			if (FormMode != Enums.FormViewModeType.Add)
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

	private async void closeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			look_Status.EditValue = "C";
			await Save(2);
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void saveAsDraftToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		_ = grid_InventoryCount.DataSourceTable;
		try
		{
			await SetDataNoDraft();
			TextEditEx textEditEx = txt_No;
			textEditEx.EditValue = await GetNextNumberDraft();
			await SaveDraft(1);
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		_ = 2;
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			SetDataStaus();
			look_Status.EditValue = "O";
			date_CountDate.EditValue = DateTime.Now.Date;
			time_CountTime.EditValue = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, 0);
			await SetDataNo();
			TextEditEx textEditEx = txt_No;
			textEditEx.EditValue = await GetNextNumber();
			GridControlEx gridControlEx = grid_InventoryCount;
			gridControlEx.DataSource = await GetDataLine(Function.ParseInt(txt_No.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MODE_FIND()
	{
		if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				FormMode = Enums.FormViewModeType.Find;
			}
		}
		else
		{
			FormMode = Enums.FormViewModeType.Find;
		}
	}

	public async Task MOVE_FIRST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.First) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Next) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Last) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_PREVIOUS()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (await FindNextLastFrist(Enums.NavigateType.Previous) == 0)
			{
				FormMode = Enums.FormViewModeType.Find;
				WinCommons._CloseProcessing();
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	public async Task<DataTable> GetDataLine(int id)
	{
		try
		{
			return await _httpClient.StockCounting.GetTableLine(id);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public async Task<DataTable> SearchDataByWarehouse(string whs)
	{
		try
		{
			return await _httpClient.StockCounting.SearchDataByWarehouse(whs);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public async Task<DataTable> SearchDataByItemCode(string whs, string itemCode)
	{
		try
		{
			return await _httpClient.StockCounting.SearchDataByItemCode(whs, itemCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private async Task<int> Save(int type)
	{
		_ = 2;
		try
		{
			DataSet dataSet = new DataSet();
			if (string.IsNullOrEmpty(Function.ToString(date_CountDate.EditValue)))
			{
				date_CountDate.Focus();
				WinCommons.Error(Enums.MessageCode.DateFieldCannotBeNull, "");
				return 1;
			}
			if (string.IsNullOrEmpty(Function.ToString(time_CountTime.EditValue)))
			{
				time_CountTime.Focus();
				WinCommons.Error(Enums.MessageCode.TimeFieldCannotBeNull, "");
				return 1;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataTable dtdoc = new DataTable();
				DataTable dataSourceTable = grid_InventoryCount.DataSourceTable;
				GetDataDoc(ref dtdoc);
				dtdoc.TableName = "Doctb";
				dataSourceTable.TableName = "Linetb";
				dataSet.Tables.Add(dtdoc.Copy());
				dataSet.Tables.Add(dataSourceTable.Copy());
				HttpResult httpResult = null;
				switch (type)
				{
				case 1:
					httpResult = await _httpClient.StockCounting.Saves(dataSet);
					break;
				case 2:
					httpResult = await _httpClient.StockCounting.Update(dataSet);
					break;
				}
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					text_DocTry.EditValue = Function.ParseInt(dtdoc.Rows[0][0]);
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					FormMode = Enums.FormViewModeType.View;
					SetDataStaus();
					await SetDataNo();
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				WinCommons._CloseProcessing();
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		return 1;
	}

	private void GetDataDoc(ref DataTable dt)
	{
		try
		{
			dt = new DataTable();
			dt.Columns.Add("DocEntry", typeof(int));
			dt.Columns.Add("CountDate", typeof(DateTime));
			dt.Columns.Add("Time", typeof(int));
			dt.Columns.Add("Series", typeof(int));
			dt.Columns.Add("DocNum", typeof(int));
			dt.Columns.Add("Status", typeof(string));
			dt.Columns.Add("Ref2", typeof(string));
			DataRow dataRow = dt.NewRow();
			dataRow["DocEntry"] = Function.ParseInt(text_DocTry.EditValue);
			dataRow["CountDate"] = DateTime.Now.Date;
			dataRow["Time"] = Function.ParseTimeSpanToInt(TimeSpan.Parse(Function.ToString(time_CountTime.Text)));
			dataRow["Series"] = Function.ParseInt(look_No.EditValue);
			dataRow["DocNum"] = Function.ParseInt(txt_No.EditValue);
			dataRow["Status"] = Function.ToString(look_Status.EditValue);
			dataRow["Ref2"] = txt_Ref.EditValue;
			dt.Rows.Add(dataRow);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void SetDataStaus()
	{
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		list.Add(new Common.Table_Code
		{
			Name = "Open",
			Code = "O"
		});
		list.Add(new Common.Table_Code
		{
			Name = "Close",
			Code = "C"
		});
		look_Status.Properties.DataSource = list;
	}

	public async Task SetDataNo()
	{
		DataTable dataTable = await _httpClient.StockCounting.GetNo();
		look_No.Properties.DataSource = dataTable;
		look_No.Properties.DisplayMember = "SeriesName";
		look_No.Properties.ValueMember = "Series";
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			look_No.EditValue = dataTable.Rows[0]["Series"];
		}
	}

	public async Task SetDataNoDraft()
	{
		DataTable dataTable = await _httpClient.StockCounting.GetNoDraf();
		look_No.Properties.DataSource = dataTable;
		look_No.Properties.DisplayMember = "SeriesName";
		look_No.Properties.ValueMember = "Series";
		if (dataTable != null && dataTable.Rows.Count > 0)
		{
			look_No.EditValue = dataTable.Rows[0]["Series"];
		}
	}

	public void SetDataUomCode()
	{
		DataTable dataTable = new DataTable();
		if (GlobalData.LoginSap)
		{
			dataTable = _httpClient.Items.GetUoMGroup();
		}
		else
		{
			dataTable.Columns.Add(new DataColumn("Code", typeof(short)));
			dataTable.Columns.Add(new DataColumn("Name", typeof(string)));
		}
		if (!dataTable.AsEnumerable().Any((DataRow r) => Function.ParseShort(r["Code"]) == -1))
		{
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = Function.ParseShort(-1);
			dataRow[1] = "Manual";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
		}
		luk_UomCode.DisplayMember = "Name";
		luk_UomCode.ValueMember = "Code";
		luk_UomCode.NullText = "";
		luk_UomCode.DataSource = dataTable;
	}

	public async Task<int> GetNextNumber()
	{
		try
		{
			return await _httpClient.StockCounting.GetNextNumber(Function.ParseInt(look_No.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public async Task<int> GetNextNumberDraft()
	{
		try
		{
			return await _httpClient.StockCounting.GetNextNumberDraft(Function.ParseInt(look_No.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 0;
		}
	}

	public async Task<int> FindNextLastFrist(Enums.NavigateType type)
	{
		_ = 1;
		try
		{
			DataTable dataTable = await _httpClient.StockCounting.FindNextLastFrist(Function.ParseInt(txt_No.EditValue), (int)type);
			if (dataTable.IsNotNull())
			{
				SetDataDoc(dataTable.Rows[0]);
				SetDataStaus();
				await LoadDataLine();
				FormMode = Enums.FormViewModeType.View;
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private void SetDataDoc(DataRow row)
	{
		try
		{
			if (row != null)
			{
				text_DocTry.EditValue = Function.ToString(row["DocEntry"]);
				date_CountDate.EditValue = Function.ParseDateTime(row["CountDate"]);
				time_CountTime.EditValue = Function.ParseIntToTimeSpan(Function.ParseInt(row["Time"]));
				look_No.EditValue = Convert.ToInt16(Function.ParseInt(row["Series"]));
				txt_No.EditValue = Function.ParseInt(row["DocNum"]);
				look_Status.EditValue = Function.ToString(row["Status"]);
				txt_Ref.EditValue = Function.ToString(row["Ref2"]);
			}
			else
			{
				text_DocTry.EditValue = null;
				date_CountDate.EditValue = null;
				time_CountTime.EditValue = null;
				look_No.EditValue = null;
				txt_No.EditValue = null;
				look_Status.EditValue = null;
				txt_Ref.EditValue = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadDataLine()
	{
		try
		{
			DataTable dataSource = await GetDataLine(Function.ParseInt(text_DocTry.EditValue));
			grid_InventoryCount.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Find(bool isFind)
	{
		_ = 2;
		try
		{
			string countDate = ((!string.IsNullOrEmpty(Function.ToString(date_CountDate.EditValue))) ? Function.ToString(Function.ParseDateTimes(date_CountDate.EditValue).Date) : "");
			int countTime = ((!string.IsNullOrEmpty(time_CountTime.Text)) ? Function.ParseTimeSpanToInt(TimeSpan.Parse(Function.ToString(time_CountTime.Text))) : 0);
			string status = (isFind ? Function.ToString(look_Status.EditValue) : "");
			int series = (isFind ? Function.ParseInt(look_No.EditValue) : 0);
			string ref2 = (isFind ? Function.ToString(txt_Ref.EditValue) : "");
			int docNum = (isFind ? Function.ParseInt(txt_No.EditValue) : 0);
			string wareHouse = (isFind ? Function.ToString(txt_WareHouse.EditValue) : "");
			DataSet dataSet = await _httpClient.Cfl.GetListInventoryCounting(countDate, countTime, status, series, ref2, docNum, wareHouse);
			if (dataSet.Tables[1].Rows.Count == 1)
			{
				SetDataDoc(dataSet.Tables[1].Rows[0]);
				SetDataStaus();
				await LoadDataLine();
				FormMode = Enums.FormViewModeType.View;
				return;
			}
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.StockCounting, false, Function.ToString(text_DocTry.EditValue), "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.CountDate = countDate;
			frm_CFL2.CountTime = countTime;
			frm_CFL2.Status = status;
			frm_CFL2.Series = series;
			frm_CFL2.Ref2 = ref2;
			frm_CFL2.DocNum = docNum;
			frm_CFL2.WareHouse = wareHouse;
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				SetDataDoc(frm_CFL2.ChooseItem);
				SetDataStaus();
				await LoadDataLine();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			btn_Ok.Enabled = isEnable;
			btn_Cancel.Enabled = isEnable;
			date_CountDate.ReadOnly = !isEnable;
			time_CountTime.ReadOnly = !isEnable;
			look_No.ReadOnly = !isEnable;
			txt_No.ReadOnly = !isEnable;
			look_Status.ReadOnly = !isEnable;
			txt_Ref.ReadOnly = !isEnable;
			txt_WareHouse.ReadOnly = !isEnable;
			txt_Item.ReadOnly = !isEnable;
			closeToolStripMenuItem.Enabled = isEnable;
			saveAsDraftToolStripMenuItem.Enabled = isEnable;
			if (Function.ToString(look_Status.EditValue) == "C")
			{
				date_CountDate.ReadOnly = true;
				time_CountTime.ReadOnly = true;
				look_No.ReadOnly = true;
				txt_No.ReadOnly = true;
				look_Status.ReadOnly = true;
				txt_Ref.ReadOnly = true;
				txt_WareHouse.ReadOnly = true;
				txt_Item.ReadOnly = true;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> SaveDraft(int type)
	{
		_ = 1;
		try
		{
			DataSet dataSet = new DataSet();
			if (string.IsNullOrEmpty(Function.ToString(date_CountDate.EditValue)))
			{
				date_CountDate.Focus();
				WinCommons.Error(Enums.MessageCode.DateFieldCannotBeNull, "");
				return 1;
			}
			if (string.IsNullOrEmpty(Function.ToString(time_CountTime.EditValue)))
			{
				time_CountTime.Focus();
				WinCommons.Error(Enums.MessageCode.TimeFieldCannotBeNull, "");
				return 1;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataTable dtdoc = new DataTable();
				DataTable dataSourceTable = grid_InventoryCount.DataSourceTable;
				GetDataDoc(ref dtdoc);
				dtdoc.TableName = "Doctb";
				dataSourceTable.TableName = "Linetb";
				dataSet.Tables.Add(dtdoc.Copy());
				dataSet.Tables.Add(dataSourceTable.Copy());
				HttpResult httpResult = await _httpClient.StockCounting.SaveDraft(dataSet);
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					text_DocTry.EditValue = Function.ParseInt(dtdoc.Rows[0][0]);
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					FormMode = Enums.FormViewModeType.View;
					SetDataStaus();
					await SetDataNo();
					return 0;
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return 1;
		}
		return 1;
	}

	private void CheckChangedData()
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
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

	private void Control_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			CheckChangedData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetChangedEvent(Control control)
	{
		try
		{
			if (control is BaseEdit)
			{
				((BaseEdit)control).EditValueChanged += Control_EditValueChanged;
				return;
			}
			if (control is GridControl)
			{
				BaseView defaultView = ((GridControl)control).DefaultView;
				if (defaultView is GridView)
				{
					((GridView)defaultView).CellValueChanged += Control_EditValueChanged;
				}
				return;
			}
			foreach (Control control2 in control.Controls)
			{
				SetChangedEvent(control2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void InitChangeDataEvent()
	{
		try
		{
			foreach (Control control in Controls)
			{
				SetChangedEvent(control);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_InventoryCount_ShowingEditor(object sender, CancelEventArgs e)
	{
	}

	private void gv_InventoryCount_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_StockCounting));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.date_CountDate = new Apzon.Controls.Editor.DateEditEx();
		this.time_CountTime = new DevExpress.XtraEditors.TimeEdit();
		this.look_Status = new DevExpress.XtraEditors.LookUpEdit();
		this.look_No = new DevExpress.XtraEditors.LookUpEdit();
		this.txt_Ref = new Apzon.Controls.Editor.TextEditEx();
		this.grid_InventoryCount = new Apzon.Controls.Grid.GridControlEx();
		this.gv_InventoryCount = new Apzon.Controls.Grid.GridViewEx();
		this.ItemNo = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_ItemCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.ItemDescript = new Apzon.Controls.Grid.GridColumnEx();
		this.Whse = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_WhsLine = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.InWhsQty = new Apzon.Controls.Grid.GridColumnEx();
		this.Counted = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Counted = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.UomCountedQty = new Apzon.Controls.Grid.GridColumnEx();
		this.CountedQty = new Apzon.Controls.Grid.GridColumnEx();
		this.Variance = new Apzon.Controls.Grid.GridColumnEx();
		this.VarianceP = new Apzon.Controls.Grid.GridColumnEx();
		this.UomCode = new Apzon.Controls.Grid.GridColumnEx();
		this.luk_UomCode = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.lab_WareHouse = new Apzon.Controls.Controls.LabelLine();
		this.lab_Items = new Apzon.Controls.Controls.LabelLine();
		this.txt_No = new Apzon.Controls.Editor.TextEditEx();
		this.txt_Item = new Apzon.Controls.Editor.TextEditEx();
		this.txt_WareHouse = new Apzon.Controls.Editor.ButtonEditEx();
		this.text_DocTry = new Apzon.Controls.Editor.TextEditEx();
		this.contextMenu_InventoryCount = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.saveAsDraftToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_CountDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_CountDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.time_CountTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_No.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Ref.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grid_InventoryCount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_InventoryCount).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WhsLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Counted).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_UomCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Item.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_WareHouse.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.text_DocTry.Properties).BeginInit();
		this.contextMenu_InventoryCount.SuspendLayout();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.repositoryItemCalcEditEx2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx2.IsSearchControl = false;
		this.repositoryItemCalcEditEx2.Name = "repositoryItemCalcEditEx2";
		this.repositoryItemCalcEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx3.IsSearchControl = false;
		this.repositoryItemCalcEditEx3.Name = "repositoryItemCalcEditEx3";
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Count date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(13, 15);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Count date";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Ref";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(366, 41);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 1;
		this.labelLine2.Utext = "Ref";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Count time ";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(206, 15);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 2;
		this.labelLine3.Utext = "Count time ";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "No.";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(366, 15);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 3;
		this.labelLine4.Utext = "No.";
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Status";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(13, 39);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 4;
		this.labelLine5.Utext = "Status";
		this.date_CountDate.EditControl = null;
		this.date_CountDate.EditValue = null;
		this.date_CountDate.FieldName = null;
		this.date_CountDate.Location = new System.Drawing.Point(92, 13);
		this.date_CountDate.Name = "date_CountDate";
		this.date_CountDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_CountDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_CountDate.Properties.IsSearchControl = false;
		this.date_CountDate.SettingInVisible = false;
		this.date_CountDate.SettingReadonly = false;
		this.date_CountDate.Size = new System.Drawing.Size(108, 20);
		this.date_CountDate.TabIndex = 1;
		this.time_CountTime.EditValue = new System.DateTime(2017, 3, 14, 0, 0, 0, 0);
		this.time_CountTime.Location = new System.Drawing.Point(283, 13);
		this.time_CountTime.Name = "time_CountTime";
		this.time_CountTime.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.time_CountTime.Size = new System.Drawing.Size(69, 20);
		this.time_CountTime.TabIndex = 2;
		this.look_Status.Location = new System.Drawing.Point(92, 37);
		this.look_Status.Name = "look_Status";
		this.look_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Status.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Value")
		});
		this.look_Status.Properties.DisplayMember = "Name";
		this.look_Status.Properties.NullText = "";
		this.look_Status.Properties.ReadOnly = true;
		this.look_Status.Properties.ValueMember = "Code";
		this.look_Status.Size = new System.Drawing.Size(108, 20);
		this.look_Status.TabIndex = 5;
		this.look_No.Location = new System.Drawing.Point(442, 13);
		this.look_No.Name = "look_No";
		this.look_No.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_No.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("SeriesName", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Series", "Value")
		});
		this.look_No.Properties.DisplayMember = "SeriesName";
		this.look_No.Properties.NullText = "";
		this.look_No.Properties.ValueMember = "Series";
		this.look_No.Size = new System.Drawing.Size(86, 20);
		this.look_No.TabIndex = 3;
		this.txt_Ref.EditControl = null;
		this.txt_Ref.FieldName = null;
		this.txt_Ref.IsSearchControl = false;
		this.txt_Ref.Location = new System.Drawing.Point(443, 38);
		this.txt_Ref.Name = "txt_Ref";
		this.txt_Ref.SettingInVisible = false;
		this.txt_Ref.SettingReadonly = false;
		this.txt_Ref.Size = new System.Drawing.Size(191, 20);
		this.txt_Ref.TabIndex = 6;
		this.grid_InventoryCount.AllowPasteExcel = true;
		this.grid_InventoryCount.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grid_InventoryCount.ClipboardData = null;
		this.grid_InventoryCount.Location = new System.Drawing.Point(0, 112);
		this.grid_InventoryCount.MainView = this.gv_InventoryCount;
		this.grid_InventoryCount.Name = "grid_InventoryCount";
		this.grid_InventoryCount.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_Counted, this.btn_ItemCode, this.btn_WhsLine, this.luk_UomCode });
		this.grid_InventoryCount.Size = new System.Drawing.Size(650, 216);
		this.grid_InventoryCount.TabIndex = 10;
		this.grid_InventoryCount.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_InventoryCount });
		this.gv_InventoryCount.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_InventoryCount.ColumnKey = null;
		this.gv_InventoryCount.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[17]
		{
			this.ItemNo, this.ItemDescript, this.Whse, this.InWhsQty, this.Counted, this.UomCountedQty, this.CountedQty, this.Variance, this.VarianceP, this.UomCode,
			this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx7
		});
		this.gv_InventoryCount.DisableFormSetting = false;
		this.gv_InventoryCount.DisplayException = false;
		this.gv_InventoryCount.EditColumn = null;
		this.gv_InventoryCount.FieldCurrency = null;
		this.gv_InventoryCount.FieldRate = null;
		this.gv_InventoryCount.FormId = "oStockCounting";
		this.gv_InventoryCount.GridControl = this.grid_InventoryCount;
		this.gv_InventoryCount.IgnoreFilter = false;
		this.gv_InventoryCount.IndexEditRow = -1;
		this.gv_InventoryCount.IndicatorWidth = 30;
		this.gv_InventoryCount.IsApzTable = false;
		this.gv_InventoryCount.IsInsertNewRow = false;
		this.gv_InventoryCount.IsLoadingData = false;
		this.gv_InventoryCount.IsMappingContextAction = false;
		this.gv_InventoryCount.ItemId = "INC1";
		this.gv_InventoryCount.LstColumnAllowBatch = null;
		this.gv_InventoryCount.LstColumnNotAllowBatch = null;
		this.gv_InventoryCount.LstColumnNotEditable = null;
		this.gv_InventoryCount.LstColumNotSetting = null;
		this.gv_InventoryCount.Name = "gv_InventoryCount";
		this.gv_InventoryCount.OptionsView.ColumnAutoWidth = false;
		this.gv_InventoryCount.OptionsView.ShowGroupPanel = false;
		this.gv_InventoryCount.UseBatchMod = false;
		this.gv_InventoryCount.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gv_InventoryCount.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_InventoryCount_RowCellStyle);
		this.gv_InventoryCount.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_InventoryCount_ShowingEditor);
		this.gv_InventoryCount.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_InventoryCount_CellValueChanged);
		this.ItemNo.Caption = "Item No";
		this.ItemNo.ColumnEdit = this.btn_ItemCode;
		this.ItemNo.CurrencyDateField = null;
		this.ItemNo.CurrencyField = null;
		this.ItemNo.CurrencyFixed = null;
		this.ItemNo.CurrencyFixedRate = 0.0;
		this.ItemNo.CurrencyRateField = null;
		this.ItemNo.FieldName = "ItemCode";
		this.ItemNo.Name = "ItemNo";
		this.ItemNo.OptionsColumn.ReadOnly = true;
		this.ItemNo.PrimaryKey = true;
		this.ItemNo.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.ItemNo.UnitField = null;
		this.ItemNo.Visible = true;
		this.ItemNo.VisibleIndex = 0;
		this.btn_ItemCode.AutoHeight = false;
		this.btn_ItemCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.btn_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_ItemCode.ContainerControl = this;
		this.btn_ItemCode.DisplayMember = "ItemCode";
		this.btn_ItemCode.DisplayMember2 = "";
		this.btn_ItemCode.LoadDataSourceWhenEnter = false;
		this.btn_ItemCode.Name = "btn_ItemCode";
		this.btn_ItemCode.ReadOnly = true;
		this.btn_ItemCode.SeparateCharacterForMulti = ';';
		this.btn_ItemCode.SeparateString = ", ";
		this.btn_ItemCode.UsingAutoCompleteBox = false;
		this.btn_ItemCode.UsingDataSource = true;
		this.btn_ItemCode.ValidateWhenNoDataSource = false;
		this.btn_ItemCode.ValueMember = "ItemCode";
		this.ItemDescript.Caption = "Item Descript";
		this.ItemDescript.CurrencyDateField = null;
		this.ItemDescript.CurrencyField = null;
		this.ItemDescript.CurrencyFixed = null;
		this.ItemDescript.CurrencyFixedRate = 0.0;
		this.ItemDescript.CurrencyRateField = null;
		this.ItemDescript.FieldName = "ItemDesc";
		this.ItemDescript.Name = "ItemDescript";
		this.ItemDescript.OptionsColumn.ReadOnly = true;
		this.ItemDescript.PrimaryKey = false;
		this.ItemDescript.UnitField = null;
		this.ItemDescript.Visible = true;
		this.ItemDescript.VisibleIndex = 1;
		this.Whse.Caption = "Whse";
		this.Whse.ColumnEdit = this.btn_WhsLine;
		this.Whse.CurrencyDateField = null;
		this.Whse.CurrencyField = null;
		this.Whse.CurrencyFixed = null;
		this.Whse.CurrencyFixedRate = 0.0;
		this.Whse.CurrencyRateField = null;
		this.Whse.FieldName = "WhsCode";
		this.Whse.Name = "Whse";
		this.Whse.OptionsColumn.ReadOnly = true;
		this.Whse.PrimaryKey = false;
		this.Whse.UnitField = null;
		this.Whse.Visible = true;
		this.Whse.VisibleIndex = 2;
		this.btn_WhsLine.AutoHeight = false;
		this.btn_WhsLine.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.btn_WhsLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.btn_WhsLine.ContainerControl = this;
		this.btn_WhsLine.DisplayMember = "WhsCode";
		this.btn_WhsLine.DisplayMember2 = "";
		this.btn_WhsLine.LoadDataSourceWhenEnter = false;
		this.btn_WhsLine.Name = "btn_WhsLine";
		this.btn_WhsLine.ReadOnly = true;
		this.btn_WhsLine.SeparateCharacterForMulti = ';';
		this.btn_WhsLine.SeparateString = ", ";
		this.btn_WhsLine.UsingAutoCompleteBox = false;
		this.btn_WhsLine.UsingDataSource = true;
		this.btn_WhsLine.ValidateWhenNoDataSource = false;
		this.btn_WhsLine.ValueMember = "WhsCode";
		this.InWhsQty.Caption = "In Whs Qty";
		this.InWhsQty.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.InWhsQty.CurrencyDateField = null;
		this.InWhsQty.CurrencyField = null;
		this.InWhsQty.CurrencyFixed = null;
		this.InWhsQty.CurrencyFixedRate = 0.0;
		this.InWhsQty.CurrencyRateField = null;
		this.InWhsQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.InWhsQty.FieldName = "InWhsQty";
		this.InWhsQty.Name = "InWhsQty";
		this.InWhsQty.OptionsColumn.ReadOnly = true;
		this.InWhsQty.PrimaryKey = false;
		this.InWhsQty.UnitField = null;
		this.InWhsQty.Visible = true;
		this.InWhsQty.VisibleIndex = 3;
		this.Counted.Caption = "Counted";
		this.Counted.ColumnEdit = this.rep_Counted;
		this.Counted.CurrencyDateField = null;
		this.Counted.CurrencyField = null;
		this.Counted.CurrencyFixed = null;
		this.Counted.CurrencyFixedRate = 0.0;
		this.Counted.CurrencyRateField = null;
		this.Counted.FieldName = "Counted";
		this.Counted.Name = "Counted";
		this.Counted.PrimaryKey = false;
		this.Counted.UnitField = null;
		this.Counted.Visible = true;
		this.Counted.VisibleIndex = 4;
		this.rep_Counted.AutoHeight = false;
		this.rep_Counted.Name = "rep_Counted";
		this.rep_Counted.ValueChecked = "Y";
		this.rep_Counted.ValueGrayed = "N";
		this.rep_Counted.ValueUnchecked = "N";
		this.UomCountedQty.Caption = "Uom Counted Qty";
		this.UomCountedQty.CurrencyDateField = null;
		this.UomCountedQty.CurrencyField = null;
		this.UomCountedQty.CurrencyFixed = null;
		this.UomCountedQty.CurrencyFixedRate = 0.0;
		this.UomCountedQty.CurrencyRateField = null;
		this.UomCountedQty.FieldName = "UomQty";
		this.UomCountedQty.Name = "UomCountedQty";
		this.UomCountedQty.PrimaryKey = false;
		this.UomCountedQty.UnitField = null;
		this.UomCountedQty.Visible = true;
		this.UomCountedQty.VisibleIndex = 5;
		this.CountedQty.Caption = "Counted Qty";
		this.CountedQty.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.CountedQty.CurrencyDateField = null;
		this.CountedQty.CurrencyField = null;
		this.CountedQty.CurrencyFixed = null;
		this.CountedQty.CurrencyFixedRate = 0.0;
		this.CountedQty.CurrencyRateField = null;
		this.CountedQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.CountedQty.FieldName = "CountQty";
		this.CountedQty.Name = "CountedQty";
		this.CountedQty.PrimaryKey = false;
		this.CountedQty.UnitField = null;
		this.CountedQty.Visible = true;
		this.CountedQty.VisibleIndex = 6;
		this.Variance.Caption = "Variance";
		this.Variance.CurrencyDateField = null;
		this.Variance.CurrencyField = null;
		this.Variance.CurrencyFixed = null;
		this.Variance.CurrencyFixedRate = 0.0;
		this.Variance.CurrencyRateField = null;
		this.Variance.FieldName = "Difference";
		this.Variance.Name = "Variance";
		this.Variance.OptionsColumn.ReadOnly = true;
		this.Variance.PrimaryKey = false;
		this.Variance.UnitField = null;
		this.Variance.Visible = true;
		this.Variance.VisibleIndex = 7;
		this.VarianceP.Caption = "Variance %";
		this.VarianceP.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.VarianceP.CurrencyDateField = null;
		this.VarianceP.CurrencyField = null;
		this.VarianceP.CurrencyFixed = null;
		this.VarianceP.CurrencyFixedRate = 0.0;
		this.VarianceP.CurrencyRateField = null;
		this.VarianceP.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.VarianceP.FieldName = "DiffPercen";
		this.VarianceP.Name = "VarianceP";
		this.VarianceP.OptionsColumn.ReadOnly = true;
		this.VarianceP.PrimaryKey = false;
		this.VarianceP.UnitField = null;
		this.VarianceP.Visible = true;
		this.VarianceP.VisibleIndex = 8;
		this.UomCode.Caption = "Uom Code";
		this.UomCode.ColumnEdit = this.luk_UomCode;
		this.UomCode.CurrencyDateField = null;
		this.UomCode.CurrencyField = null;
		this.UomCode.CurrencyFixed = null;
		this.UomCode.CurrencyFixedRate = 0.0;
		this.UomCode.CurrencyRateField = null;
		this.UomCode.FieldName = "UomCode";
		this.UomCode.Name = "UomCode";
		this.UomCode.OptionsColumn.ReadOnly = true;
		this.UomCode.PrimaryKey = false;
		this.UomCode.UnitField = null;
		this.UomCode.Visible = true;
		this.UomCode.VisibleIndex = 9;
		this.luk_UomCode.AutoHeight = false;
		this.luk_UomCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_UomCode.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.luk_UomCode.DisplayMember = "Name";
		this.luk_UomCode.Name = "luk_UomCode";
		this.luk_UomCode.NullText = "";
		this.luk_UomCode.ValueMember = "Code";
		this.gridColumnEx1.Caption = "DocEntry";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "DocEntry";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "LineNum";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "LineNum";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx3.Caption = "Freeze";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Freeze";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx4.Caption = "CountQtyT1";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "CountQtyT1";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx5.Caption = "CountQtyT2";
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "CountQtyT2";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx6.Caption = "Remark";
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "Remark";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx7.Caption = "BarCode";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "BarCode";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.lab_WareHouse.EditControl = null;
		this.lab_WareHouse.EditValue = "Warehouse";
		this.lab_WareHouse.FieldName = null;
		this.lab_WareHouse.HideWhenEmpty = false;
		this.lab_WareHouse.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lab_WareHouse.Location = new System.Drawing.Point(12, 86);
		this.lab_WareHouse.Name = "lab_WareHouse";
		this.lab_WareHouse.ReadOnly = false;
		this.lab_WareHouse.SettingInVisible = false;
		this.lab_WareHouse.SettingReadonly = false;
		this.lab_WareHouse.ShowUnderLine = true;
		this.lab_WareHouse.Size = new System.Drawing.Size(80, 20);
		this.lab_WareHouse.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lab_WareHouse.TabIndex = 11;
		this.lab_WareHouse.Utext = "Warehouse";
		this.lab_Items.EditControl = null;
		this.lab_Items.EditValue = "Items";
		this.lab_Items.FieldName = null;
		this.lab_Items.HideWhenEmpty = false;
		this.lab_Items.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lab_Items.Location = new System.Drawing.Point(366, 86);
		this.lab_Items.Name = "lab_Items";
		this.lab_Items.ReadOnly = false;
		this.lab_Items.SettingInVisible = false;
		this.lab_Items.SettingReadonly = false;
		this.lab_Items.ShowUnderLine = true;
		this.lab_Items.Size = new System.Drawing.Size(80, 20);
		this.lab_Items.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lab_Items.TabIndex = 13;
		this.lab_Items.Utext = "Items";
		this.txt_No.EditControl = null;
		this.txt_No.FieldName = null;
		this.txt_No.IsSearchControl = false;
		this.txt_No.Location = new System.Drawing.Point(534, 13);
		this.txt_No.Name = "txt_No";
		this.txt_No.Properties.ReadOnly = true;
		this.txt_No.SettingInVisible = false;
		this.txt_No.SettingReadonly = false;
		this.txt_No.Size = new System.Drawing.Size(100, 20);
		this.txt_No.TabIndex = 4;
		this.txt_Item.EditControl = null;
		this.txt_Item.FieldName = null;
		this.txt_Item.IsSearchControl = false;
		this.txt_Item.Location = new System.Drawing.Point(443, 84);
		this.txt_Item.Name = "txt_Item";
		this.txt_Item.SettingInVisible = false;
		this.txt_Item.SettingReadonly = false;
		this.txt_Item.Size = new System.Drawing.Size(191, 20);
		this.txt_Item.TabIndex = 8;
		this.txt_Item.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Item_KeyDown);
		this.txt_WareHouse.EditControl = null;
		this.txt_WareHouse.FieldName = null;
		this.txt_WareHouse.IsSearchControl = false;
		this.txt_WareHouse.Location = new System.Drawing.Point(92, 83);
		this.txt_WareHouse.Name = "txt_WareHouse";
		this.txt_WareHouse.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.txt_WareHouse.SettingInVisible = false;
		this.txt_WareHouse.SettingReadonly = false;
		this.txt_WareHouse.Size = new System.Drawing.Size(108, 20);
		this.txt_WareHouse.TabIndex = 7;
		this.txt_WareHouse.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txt_WareHouse_ButtonClick);
		this.txt_WareHouse.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_WareHouse_KeyDown);
		this.text_DocTry.EditControl = null;
		this.text_DocTry.FieldName = null;
		this.text_DocTry.IsSearchControl = false;
		this.text_DocTry.Location = new System.Drawing.Point(220, 83);
		this.text_DocTry.Name = "text_DocTry";
		this.text_DocTry.Properties.ReadOnly = true;
		this.text_DocTry.SettingInVisible = false;
		this.text_DocTry.SettingReadonly = false;
		this.text_DocTry.Size = new System.Drawing.Size(100, 20);
		this.text_DocTry.TabIndex = 21;
		this.text_DocTry.Visible = false;
		this.contextMenu_InventoryCount.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.closeToolStripMenuItem, this.saveAsDraftToolStripMenuItem });
		this.contextMenu_InventoryCount.Name = "contextMenu_InventoryCount";
		this.contextMenu_InventoryCount.Size = new System.Drawing.Size(141, 48);
		this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
		this.closeToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
		this.closeToolStripMenuItem.Text = "Close";
		this.closeToolStripMenuItem.Click += new System.EventHandler(this.closeToolStripMenuItem_Click);
		this.saveAsDraftToolStripMenuItem.Name = "saveAsDraftToolStripMenuItem";
		this.saveAsDraftToolStripMenuItem.Size = new System.Drawing.Size(140, 22);
		this.saveAsDraftToolStripMenuItem.Text = "Save as draft";
		this.saveAsDraftToolStripMenuItem.Click += new System.EventHandler(this.saveAsDraftToolStripMenuItem_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(12, 335);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 23);
		this.btn_Ok.TabIndex = 9;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(93, 335);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 10;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click_1);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(650, 362);
		this.ContextMenuStrip = this.contextMenu_InventoryCount;
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.text_DocTry);
		base.Controls.Add(this.txt_Item);
		base.Controls.Add(this.txt_No);
		base.Controls.Add(this.lab_Items);
		base.Controls.Add(this.lab_WareHouse);
		base.Controls.Add(this.grid_InventoryCount);
		base.Controls.Add(this.txt_Ref);
		base.Controls.Add(this.look_No);
		base.Controls.Add(this.look_Status);
		base.Controls.Add(this.time_CountTime);
		base.Controls.Add(this.date_CountDate);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.txt_WareHouse);
		base.FormId = "oStockCounting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_StockCounting";
		base.ShowInTaskbar = false;
		this.Text = "Stock Counting";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_InventoryCounting_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_CountDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_CountDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.time_CountTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_No.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Ref.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grid_InventoryCount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_InventoryCount).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_WhsLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Counted).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_UomCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_No.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Item.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_WareHouse.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.text_DocTry.Properties).EndInit();
		this.contextMenu_InventoryCount.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
