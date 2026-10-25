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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Mrps;

public class frm_ItemQuotaArrangement : XtraFormEx
{
	private string _formController = "ItemQuotaArrangement";

	private int _colVendorRHandle = -1;

	private int _colItemRHandle = -1;

	private bool _rowObjChanging;

	private bool _rowFocusedChanging;

	private bool _repBteItemLeaving;

	private string[] _ROnlyItemColumnUpdate = new string[2] { "ItemCode", "ItemName" };

	private IContainer components;

	private SplitContainer splitContainer1;

	private GridControlEx grd_Items;

	private GridColumnEx gridColumn8;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridControlEx grd_Period;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Save;

	private LabelLine lbBranch;

	private ContextMenuStrip ctm_Items;

	private ToolStripMenuItem addToolStripMenu_Item;

	private ToolStripMenuItem removeToolStripMenu_Item;

	private ContextMenuStrip ctm_Vendors;

	private ToolStripMenuItem addToolStripMenu_Vendor;

	private ToolStripMenuItem removeToolStripMenu_Vendor;

	private RepositoryItemButtonEdit repositoryItemButtonEdit2;

	private RepositoryItemButtonEdit rep_Item_ItemCode;

	private ButtonEditEx bte_ItemCode;

	private GridViewEx gv_Items;

	private GridViewEx gv_Period;

	private RepositoryItemDateEdit rep_Date;

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
			btn_Save.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btn_Save.Text = Resources.ButtonFind;
			break;
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				FormMode = Enums.FormViewModeType.Find;
				grd_Items.DataSource = null;
				grd_Items.RefreshDataSource();
				grd_Period.DataSource = null;
				grd_Period.RefreshDataSource();
				bte_ItemCode.Focus();
				EditorButton editorButton = rep_Item_ItemCode.Buttons.FirstOrDefault((EditorButton b) => b.Kind == ButtonPredefines.Search);
				if (editorButton != null)
				{
					editorButton.Visible = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
		try
		{
			addToolStripMenu_Item_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_ItemQuotaArrangement()
	{
		InitializeComponent();
	}

	private void frm_ItemQuotaArrangement_Load(object sender, EventArgs e)
	{
		try
		{
			SetFormatControlValue();
			GetDocs();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gv_Items_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "ItemCode")
			{
				string text = Function.ToString(e.Value);
				if (!string.IsNullOrWhiteSpace(text))
				{
					DataTable aDoc = GetADoc(text);
					if (aDoc != null && aDoc.Rows.Count == 1)
					{
						DataTable dataTable = grd_Items.DataSource as DataTable;
						if (dataTable != null && dataTable.Rows.Count == 1)
						{
							dataTable.Rows[0]["ItemName"] = Function.ToString(aDoc.Rows[0]["ItemName"]);
							dataTable.Rows[0]["MinQty"] = Function.ParseDouble(aDoc.Rows[0]["MinQty"]);
							dataTable.AcceptChanges();
						}
						grd_Items.DataSource = dataTable;
						grd_Items.RefreshDataSource();
						GetLines();
					}
				}
			}
			if (!_repBteItemLeaving && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Vendors_CellValueChanged(object sender, CellValueChangedEventArgs e)
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
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Vendors_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			int periodID = Function.ParseInt(gv_Period.GetFocusedRowCellValue("PeriodID"));
			if (CheckExistPeriod(periodID))
			{
				frm_ItemQuotaArrangementDate frm_ItemQuotaArrangementDate2 = new frm_ItemQuotaArrangementDate();
				frm_ItemQuotaArrangementDate2.MdiParent = MdiParent;
				frm_ItemQuotaArrangementDate2._PeriodID = periodID;
				frm_ItemQuotaArrangementDate2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void buttonEdit1_Properties_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					bte_ItemCode.EditValue = Function.ToString(frm_CFL2.ChooseItem["ItemCode"]);
					GetDocs();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Items_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_rowObjChanging = true;
		try
		{
			if (_rowFocusedChanging)
			{
				_rowObjChanging = false;
				return;
			}
			GetLines();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_rowObjChanging = false;
	}

	private void gv_Items_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		_rowFocusedChanging = true;
		try
		{
			if (_rowObjChanging)
			{
				_rowFocusedChanging = false;
				return;
			}
			GetLines();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_rowFocusedChanging = false;
	}

	private void bte_ItemCode_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && FormMode == Enums.FormViewModeType.Find)
			{
				btn_Save_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Items_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel, MessageBoxIcon.Question) == DialogResult.OK)
				{
					FormMode = Enums.FormViewModeType.Find;
					btn_Save_Click(null, null);
				}
				else
				{
					e.Allow = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Item_ItemCode_Enter(object sender, EventArgs e)
	{
		_colItemRHandle = gv_Items.FocusedRowHandle;
	}

	private void rep_Item_ItemCode_Leave(object sender, EventArgs e)
	{
		try
		{
			if (_colItemRHandle > -1)
			{
				string itemCode = Function.ToString(gv_Items.GetRowCellValue(_colItemRHandle, "ItemCode"));
				string itemName = GetItemName(itemCode);
				_repBteItemLeaving = true;
				gv_Items.SetRowCellValue(_colItemRHandle, "ItemName", itemName);
				_repBteItemLeaving = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_Vendor_Enter(object sender, EventArgs e)
	{
		_colVendorRHandle = gv_Period.FocusedRowHandle;
	}

	private void grd_Vendors_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				if (string.IsNullOrWhiteSpace(Function.ToString(gv_Items.GetFocusedRowCellValue("ItemCode"))))
				{
					WinCommons.Error(Enums.MessageCode.ItemCodeCannotBeNull, "");
					return;
				}
				ExtenseionMethods.PasteFromExcelToDataTable(grd_Period, gv_Period, new List<string> { "FromDate" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
			{
				if (ValidateData())
				{
					SaveData();
				}
			}
			else if (FormMode == Enums.FormViewModeType.Find)
			{
				SetReadonlyColumns();
				GetDocs();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void addToolStripMenu_Vendor_Click(object sender, EventArgs e)
	{
		try
		{
			string value = Function.ToString(gv_Items.GetFocusedRowCellValue("ItemCode"));
			if (string.IsNullOrWhiteSpace(value))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseItemFirst, "");
				return;
			}
			DataTable dataTable = grd_Period.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = GetTableStructure("P").Clone();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["ItemCode"] = value;
			dataRow["PeriodID"] = -1;
			DateTime dateTime = DateTime.Today;
			if (dataTable.Rows.Count > 0)
			{
				dateTime = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseDateTime(t["FromDate"]).Value.Date);
			}
			for (int num = 0; num < dataTable.Rows.Count; num++)
			{
				DateTime? dateTime2 = Function.ParseDateTime(dataTable.Rows[num]["ToDate"]);
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					if ((1u & ((dateTime2.Value.Date > dateTime.Date) ? 1u : 0u)) != 0)
					{
						dateTime = dateTime2.Value.Date;
					}
				}
			}
			dataRow["FromDate"] = dateTime.AddDays(1.0);
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Period.DataSource = dataTable;
			grd_Period.RefreshDataSource();
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void removeToolStripMenu_Vendor_Click(object sender, EventArgs e)
	{
		try
		{
			string text = Function.ToString(gv_Items.GetFocusedRowCellValue("ItemCode"));
			bool flag = !string.IsNullOrWhiteSpace(text);
			if (flag)
			{
				flag = !(await ExtenseionMethods.CheckExistItem(text));
			}
			if (!flag)
			{
				DataTable dataTable = grd_Period.DataSource as DataTable;
				gv_Period.DeleteSelectedRows();
				dataTable.AcceptChanges();
				grd_Period.DataSource = dataTable;
				grd_Period.RefreshDataSource();
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addToolStripMenu_Item_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes))
			{
				FormMode = Enums.FormViewModeType.Add;
				DataTable dataTable = GetTableStructure("I").Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow["ItemCode"] = "";
				dataRow["ItemName"] = "";
				dataRow["MinQty"] = 0.0;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_Items.DataSource = dataTable;
				grd_Items.RefreshDataSource();
				EditorButton editorButton = rep_Item_ItemCode.Buttons.FirstOrDefault((EditorButton b) => b.Kind == ButtonPredefines.Search);
				if (editorButton != null)
				{
					editorButton.Visible = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void removeToolStripMenu_Item_Click(object sender, EventArgs e)
	{
		try
		{
			if (!(grd_Items.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0)
			{
				return;
			}
			string ItemCode = Function.ToString(gv_Items.GetFocusedRowCellValue("ItemCode"));
			if (!(await ExtenseionMethods.CheckExistsItemCode(ItemCode)) || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes || string.IsNullOrWhiteSpace(ItemCode))
			{
				return;
			}
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
			string text = httpClientHelper.Post("DeleteDoc", ItemCode);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				GetDocs();
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

	private void rep_Item_ItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm = new frm_CFL(Enums.TypeCFL.ListItems, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm.ListMandatoryField = "ItemDescription";
				frm.ShowDialog(this);
				if (frm.ChooseItem == null)
				{
					return;
				}
				DataTable dataSourceTable = grd_Items.DataSourceTable;
				if (dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ItemCode"]).ToUpper() == Function.ToString(frm.ChooseItem).ToUpper()) != null)
				{
					WinCommons.Error(Enums.MessageCode.ItemExistedWithItemCode, "", Function.ToString(frm.ChooseItem));
					return;
				}
				string text = Function.ToString(frm.ChooseItem["ItemCode"]);
				if (!string.IsNullOrWhiteSpace(text))
				{
					DataTable aDoc = GetADoc(text);
					if (aDoc != null && aDoc.Rows.Count == 1)
					{
						DataTable dataSourceTable2 = grd_Items.DataSourceTable;
						if (dataSourceTable2 != null && dataSourceTable2.Rows.Count == 1)
						{
							dataSourceTable2.Rows[0]["ItemName"] = Function.ToString(aDoc.Rows[0]["ItemName"]);
							dataSourceTable2.Rows[0]["MinQty"] = Function.ParseDouble(aDoc.Rows[0]["MinQty"]);
							dataSourceTable2.Rows[0]["ItemCode"] = text;
							dataSourceTable2.AcceptChanges();
						}
						grd_Items.DataSource = dataSourceTable2;
						grd_Items.RefreshDataSource();
						GetLines();
					}
					else
					{
						DataRow dataRow = dataSourceTable.Rows[gv_Items.GetFocusedDataSourceRowIndex()];
						dataRow["ItemCode"] = text;
						dataRow["ItemName"] = Function.ToString(frm.ChooseItem["ItemDescription"]);
						dataSourceTable.AcceptChanges();
						grd_Items.DataSource = dataSourceTable;
						grd_Items.RefreshDataSource();
						addToolStripMenu_Vendor_Click(null, null);
					}
				}
				gv_Items.RefreshRow(gv_Items.FocusedRowHandle);
			}
			else if (e.Button.Index == 1)
			{
				string text2 = Function.ToString(gv_Items.GetFocusedRowCellValue("ItemCode"));
				if (WinFuntions.CheckExistValidValue("CheckExistValidItemCode", text2))
				{
					this.DrillDown(ApzObjectTypes.oItems, ApzObjectTypes.oItems, text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetLines()
	{
		try
		{
			string text = Function.ToString(gv_Items.GetFocusedRowCellValue("ItemCode"));
			if (!string.IsNullOrWhiteSpace(text))
			{
				using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController))
				{
					DataTable dataSource = httpClientHelper.Get("GetLines", new
					{
						ItemCode = text
					});
					grd_Period.DataSource = dataSource;
					grd_Period.RefreshDataSource();
					return;
				}
			}
			grd_Period.DataSource = null;
			grd_Period.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetDocs()
	{
		try
		{
			string str = Function.ToString(bte_ItemCode.EditValue);
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataSource = httpClientHelper.Get("GetDocs", new { str });
			grd_Items.DataSource = dataSource;
			grd_Items.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private DataTable GetADoc(string itemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			return httpClientHelper.Get("GetADoc", new { itemCode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataTable();
		}
	}

	private void SaveData()
	{
		DisableControls(p: true);
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataSet dataSet = new DataSet();
				dataSet.Tables.Clear();
				DataTable dataTable = grd_Items.DataSourceTable.Clone();
				DataRow focusedDataRow = gv_Items.GetFocusedDataRow();
				dataTable.ImportRow(focusedDataRow);
				string value = Function.ToString(focusedDataRow["ItemCode"]);
				dataTable.TableName = "item";
				dataTable.AcceptChanges();
				dataTable.DataSet?.Tables.Remove("item");
				DataTable dataTable2 = grd_Period.DataSourceTable.Copy();
				for (int i = 0; i < dataTable2.Rows.Count; i++)
				{
					dataTable2.Rows[i]["ItemCode"] = value;
				}
				dataTable2.TableName = "period";
				dataTable2.AcceptChanges();
				dataTable2.DataSet?.Tables.Remove("period");
				dataSet.Tables.Add(dataTable);
				dataSet.Tables.Add(dataTable2);
				using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
				string text = httpClientHelper.Post("Save", dataSet);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					GetDocs();
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
		DisableControls(p: false);
	}

	private bool ValidateData()
	{
		try
		{
			string text = Function.ToString(gv_Items.GetFocusedRowCellValue("ItemCode"));
			DataTable dataSourceTable = grd_Items.DataSourceTable;
			DataTable dataSourceTable2 = grd_Period.DataSourceTable;
			if (!WinFuntions.CheckExistValidValue("CheckExistValidItemCode", text))
			{
				WinCommons.Error(Enums.MessageCode.InvalidItemCodeWithCode, "", text);
				return false;
			}
			if (dataSourceTable2 == null || dataSourceTable2.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.PeriodCannotBeNull, "");
				return false;
			}
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Error(Enums.MessageCode.ItemCodeCannotBeNull, "");
				return false;
			}
			List<string> list = (from dr in dataSourceTable.AsEnumerable()
				select Function.ToString(dr["ItemCode"]) into x
				group x by x into g
				where g.Count() > 1
				select g.Key).ToList();
			if (list != null && list.Count > 0)
			{
				WinCommons.Error(Enums.MessageCode.DuplicateItemWithCode, "", list[0]);
				return false;
			}
			List<DateTime?> list2 = (from dr in dataSourceTable2.AsEnumerable()
				select Function.ParseDateTime(dr["FromDate"]) into x
				group x by x into g
				where g.Count() > 1
				select g.Key).ToList();
			if (list2 != null && list2.Count > 0)
			{
				WinCommons.Error(Enums.MessageCode.DuplicateFromDateWithDate, "", list2[0].Value.ToString(GlobalData.DateTimeFormat));
				return false;
			}
			for (int num = 0; num < dataSourceTable2.Rows.Count; num++)
			{
				DateTime? dateTime = Function.ParseDateTime(dataSourceTable2.Rows[num]["FromDate"]);
				DateTime? dateTime2 = Function.ParseDateTime(dataSourceTable2.Rows[num]["ToDate"]);
				if (dateTime2.HasValue)
				{
					_ = dateTime2.Value;
					if (dateTime.Value.Date > dateTime2.Value.Date)
					{
						WinCommons.Error(Enums.MessageCode.FromDateMustBeBefore2Date, "");
						return false;
					}
				}
			}
			if (OverLapedDate(dataSourceTable2.Select()))
			{
				return false;
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private bool OverLapedDate(DataRow[] rows)
	{
		try
		{
			string dateTimeFormat = GlobalData.DateTimeFormat;
			for (int i = 0; i < rows.Length; i++)
			{
				DateTime? dateTime = Function.ParseDateTime(rows[i]["FromDate"]);
				DateTime? dateTime2 = Function.ParseDateTime(rows[i]["ToDate"]);
				for (int j = 0; j < rows.Length; j++)
				{
					if (j == i)
					{
						continue;
					}
					DateTime? dateTime3 = Function.ParseDateTime(rows[j]["FromDate"]);
					DateTime? dateTime4 = Function.ParseDateTime(rows[j]["ToDate"]);
					if (dateTime.Value.Date > dateTime3.Value.Date)
					{
						if (dateTime4.HasValue)
						{
							_ = dateTime4.Value;
							if (!dateTime4.HasValue)
							{
								continue;
							}
							_ = dateTime4.Value;
							if (!(dateTime4.Value.Date >= dateTime.Value.Date))
							{
								continue;
							}
						}
						WinCommons.Error(Enums.MessageCode.DateTimeOverlappedWithValue, "", dateTime.Value.ToString(dateTimeFormat), dateTime3.Value.ToString(dateTimeFormat));
						return true;
					}
					if (!(dateTime.Value.Date < dateTime3.Value.Date))
					{
						continue;
					}
					if (dateTime2.HasValue)
					{
						_ = dateTime2.Value;
						if (!dateTime2.HasValue)
						{
							continue;
						}
						_ = dateTime2.Value;
						if (!(dateTime2.Value.Date >= dateTime3.Value.Date))
						{
							continue;
						}
					}
					WinCommons.Error(Enums.MessageCode.DateTimeOverlappedWithValue, "", dateTime.Value.ToString(dateTimeFormat), dateTime3.Value.ToString(dateTimeFormat));
					return true;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private DataTable GetTableStructure(string type)
	{
		DataTable dataTable = new DataTable();
		try
		{
			if (!(type == "I"))
			{
				if (type == "P")
				{
					dataTable.Columns.Add(new DataColumn
					{
						DataType = typeof(string),
						ColumnName = "ItemCode",
						DefaultValue = ""
					});
					dataTable.Columns.Add(new DataColumn
					{
						DataType = typeof(int),
						ColumnName = "PeriodID",
						DefaultValue = -1
					});
					dataTable.Columns.Add(new DataColumn
					{
						DataType = typeof(DateTime),
						ColumnName = "FromDate",
						DefaultValue = DateTime.Today
					});
					dataTable.Columns.Add(new DataColumn
					{
						DataType = typeof(DateTime),
						ColumnName = "ToDate",
						DefaultValue = DBNull.Value
					});
					dataTable.Columns.Add(new DataColumn
					{
						DataType = typeof(string),
						ColumnName = "Remarks",
						DefaultValue = ""
					});
				}
			}
			else
			{
				dataTable.Columns.Add("ItemCode", typeof(string));
				dataTable.Columns.Add("ItemName", typeof(string));
				dataTable.Columns.Add("MinQty", typeof(double));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dataTable;
	}

	private bool CheckExistItem(string ItemCode)
	{
		try
		{
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController))
			{
				DataTable dataTable = httpClientHelper.Get("CheckExistItem", new { ItemCode });
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return true;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private bool CheckExistPeriod(int periodID)
	{
		try
		{
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController))
			{
				DataTable dataTable = httpClientHelper.Get("CheckExistPeriod", new { periodID });
				if (dataTable != null && dataTable.Rows.Count > 0)
				{
					return true;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private void DisableControls(bool p)
	{
		try
		{
			btn_Cancel.Enabled = !p;
			btn_Save.Enabled = !p;
			grd_Items.Enabled = !p;
			grd_Period.Enabled = !p;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetFormatControlValue()
	{
		try
		{
			gv_Items.Columns["MinQty"].DisplayFormat.FormatType = FormatType.Numeric;
			gv_Items.Columns["MinQty"].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
			string dateTimeFormat = GlobalData.DateTimeFormat;
			rep_Date.Mask.MaskType = MaskType.DateTime;
			rep_Date.Mask.EditMask = dateTimeFormat;
			rep_Date.Mask.UseMaskAsDisplayFormat = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string GetItemName(string ItemCode)
	{
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataTable = httpClientHelper.Get("GetItemName", new { ItemCode });
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				return Function.ToString(dataTable.Rows[0][0]);
			}
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return "";
		}
	}

	private void SetReadonlyColumns()
	{
		try
		{
			for (int i = 0; i < gv_Items.Columns.Count; i++)
			{
				if (_ROnlyItemColumnUpdate.Contains(gv_Items.Columns[i].FieldName))
				{
					gv_Items.Columns[i].OptionsColumn.ReadOnly = true;
				}
				else
				{
					gv_Items.Columns[i].OptionsColumn.ReadOnly = false;
				}
			}
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_ItemQuotaArrangement));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.repositoryItemButtonEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.grd_Items = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Items = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenu_Item = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenu_Item = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Items = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Item_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.grd_Period = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Vendors = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenu_Vendor = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenu_Vendor = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Period = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Date = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbBranch = new Apzon.Controls.Controls.LabelLine();
		this.bte_ItemCode = new Apzon.Controls.Editor.ButtonEditEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Items).BeginInit();
		this.ctm_Items.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Items).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Item_ItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Period).BeginInit();
		this.ctm_Vendors.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Period).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemCode.Properties).BeginInit();
		base.SuspendLayout();
		this.repositoryItemButtonEdit2.AutoHeight = false;
		this.repositoryItemButtonEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.SpinRight, "", -1, true, true, true, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEdit2.Name = "repositoryItemButtonEdit2";
		this.splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainer1.Location = new System.Drawing.Point(3, 31);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Panel1.Controls.Add(this.grd_Items);
		this.splitContainer1.Panel2.Controls.Add(this.grd_Period);
		this.splitContainer1.Size = new System.Drawing.Size(760, 419);
		this.splitContainer1.SplitterDistance = 207;
		this.splitContainer1.TabIndex = 22;
		this.grd_Items.AllowPasteExcel = true;
		this.grd_Items.ClipboardData = null;
		this.grd_Items.ContextMenuStrip = this.ctm_Items;
		this.grd_Items.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Items.Location = new System.Drawing.Point(0, 0);
		this.grd_Items.MainView = this.gv_Items;
		this.grd_Items.Name = "grd_Items";
		this.grd_Items.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Item_ItemCode });
		this.grd_Items.Size = new System.Drawing.Size(760, 207);
		this.grd_Items.TabIndex = 1;
		this.grd_Items.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Items });
		this.ctm_Items.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctm_Items.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenu_Item, this.removeToolStripMenu_Item });
		this.ctm_Items.Name = "ctm_Items";
		this.ctm_Items.Size = new System.Drawing.Size(118, 48);
		this.addToolStripMenu_Item.Name = "addToolStripMenu_Item";
		this.addToolStripMenu_Item.Size = new System.Drawing.Size(117, 22);
		this.addToolStripMenu_Item.Text = "Add";
		this.addToolStripMenu_Item.Click += new System.EventHandler(this.addToolStripMenu_Item_Click);
		this.removeToolStripMenu_Item.Name = "removeToolStripMenu_Item";
		this.removeToolStripMenu_Item.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenu_Item.Text = "Remove";
		this.removeToolStripMenu_Item.Click += new System.EventHandler(this.removeToolStripMenu_Item_Click);
		this.gv_Items.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Items.ColumnKey = null;
		this.gv_Items.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn8, this.gridColumn9, this.gridColumn10 });
		this.gv_Items.DisableFormSetting = false;
		this.gv_Items.DisplayException = false;
		this.gv_Items.EditColumn = null;
		this.gv_Items.FieldCurrency = null;
		this.gv_Items.FieldRate = null;
		this.gv_Items.FormId = "oItemQuotaForPur";
		this.gv_Items.GridControl = this.grd_Items;
		this.gv_Items.IgnoreFilter = false;
		this.gv_Items.IndexEditRow = -1;
		this.gv_Items.IndicatorWidth = 70;
		this.gv_Items.IsApzTable = false;
		this.gv_Items.IsInsertNewRow = false;
		this.gv_Items.IsLoadingData = false;
		this.gv_Items.IsMappingContextAction = false;
		this.gv_Items.ItemId = "OITM";
		this.gv_Items.LstColumnAllowBatch = null;
		this.gv_Items.LstColumnNotAllowBatch = null;
		this.gv_Items.LstColumnNotEditable = null;
		this.gv_Items.LstColumNotSetting = null;
		this.gv_Items.Name = "gv_Items";
		this.gv_Items.OptionsFind.AllowFindPanel = false;
		this.gv_Items.OptionsSelection.MultiSelect = true;
		this.gv_Items.OptionsView.ColumnAutoWidth = false;
		this.gv_Items.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Items.OptionsView.ShowGroupPanel = false;
		this.gv_Items.UseBatchMod = false;
		this.gv_Items.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_CustomDrawRowIndicator);
		this.gv_Items.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gv_Items_FocusedRowChanged);
		this.gv_Items.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Items_FocusedRowObjectChanged);
		this.gv_Items.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Items_CellValueChanged);
		this.gv_Items.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gv_Items_BeforeLeaveRow);
		this.gridColumn8.Caption = "Material Code";
		this.gridColumn8.ColumnEdit = this.rep_Item_ItemCode;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "ItemCode";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn8.PrimaryKey = true;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 0;
		this.gridColumn8.Width = 138;
		this.rep_Item_ItemCode.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rep_Item_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Item_ItemCode.Name = "rep_Item_ItemCode";
		this.rep_Item_ItemCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_Item_ItemCode_ButtonPressed);
		this.rep_Item_ItemCode.Enter += new System.EventHandler(this.rep_Item_ItemCode_Enter);
		this.rep_Item_ItemCode.Leave += new System.EventHandler(this.rep_Item_ItemCode_Leave);
		this.gridColumn9.Caption = "Material Description";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "ItemName";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 1;
		this.gridColumn9.Width = 261;
		this.gridColumn10.Caption = "Mininum Qty";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "MinQty";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 2;
		this.grd_Period.AllowPasteExcel = true;
		this.grd_Period.ClipboardData = null;
		this.grd_Period.ContextMenuStrip = this.ctm_Vendors;
		this.grd_Period.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Period.Location = new System.Drawing.Point(0, 0);
		this.grd_Period.MainView = this.gv_Period;
		this.grd_Period.Name = "grd_Period";
		this.grd_Period.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_Date });
		this.grd_Period.Size = new System.Drawing.Size(760, 208);
		this.grd_Period.TabIndex = 1;
		this.grd_Period.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Period });
		this.grd_Period.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Vendors_EditorKeyDown);
		this.ctm_Vendors.ImageScalingSize = new System.Drawing.Size(20, 20);
		this.ctm_Vendors.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenu_Vendor, this.removeToolStripMenu_Vendor });
		this.ctm_Vendors.Name = "ctm_Vendors";
		this.ctm_Vendors.Size = new System.Drawing.Size(118, 48);
		this.addToolStripMenu_Vendor.Name = "addToolStripMenu_Vendor";
		this.addToolStripMenu_Vendor.Size = new System.Drawing.Size(117, 22);
		this.addToolStripMenu_Vendor.Text = "Add";
		this.addToolStripMenu_Vendor.Click += new System.EventHandler(this.addToolStripMenu_Vendor_Click);
		this.removeToolStripMenu_Vendor.Name = "removeToolStripMenu_Vendor";
		this.removeToolStripMenu_Vendor.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenu_Vendor.Text = "Remove";
		this.removeToolStripMenu_Vendor.Click += new System.EventHandler(this.removeToolStripMenu_Vendor_Click);
		this.gv_Period.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Period.ColumnKey = null;
		this.gv_Period.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6 });
		this.gv_Period.DisableFormSetting = false;
		this.gv_Period.DisplayException = false;
		this.gv_Period.EditColumn = null;
		this.gv_Period.FieldCurrency = null;
		this.gv_Period.FieldRate = null;
		this.gv_Period.FormId = "oItemQuotaForPur";
		this.gv_Period.GridControl = this.grd_Period;
		this.gv_Period.IgnoreFilter = false;
		this.gv_Period.IndexEditRow = -1;
		this.gv_Period.IndicatorWidth = 70;
		this.gv_Period.IsApzTable = false;
		this.gv_Period.IsInsertNewRow = false;
		this.gv_Period.IsLoadingData = false;
		this.gv_Period.IsMappingContextAction = false;
		this.gv_Period.ItemId = "OPERIOD";
		this.gv_Period.LstColumnAllowBatch = null;
		this.gv_Period.LstColumnNotAllowBatch = null;
		this.gv_Period.LstColumnNotEditable = null;
		this.gv_Period.LstColumNotSetting = null;
		this.gv_Period.Name = "gv_Period";
		this.gv_Period.OptionsFind.AllowFindPanel = false;
		this.gv_Period.OptionsSelection.MultiSelect = true;
		this.gv_Period.OptionsView.ColumnAutoWidth = false;
		this.gv_Period.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Period.OptionsView.ShowGroupPanel = false;
		this.gv_Period.UseBatchMod = false;
		this.gv_Period.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_CustomDrawRowIndicator);
		this.gv_Period.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Vendors_CellValueChanged);
		this.gv_Period.DoubleClick += new System.EventHandler(this.gv_Vendors_DoubleClick);
		this.gridColumn2.Caption = "ItemCode";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "ItemCode";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn2.PrimaryKey = true;
		this.gridColumn2.UnitField = null;
		this.gridColumn3.Caption = "PeriodID";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "PeriodID";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Width = 139;
		this.gridColumn4.Caption = "From Date";
		this.gridColumn4.ColumnEdit = this.rep_Date;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "FromDate";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 0;
		this.gridColumn4.Width = 136;
		this.rep_Date.AutoHeight = false;
		this.rep_Date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Date.Name = "rep_Date";
		this.gridColumn5.Caption = "To Date";
		this.gridColumn5.ColumnEdit = this.rep_Date;
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "ToDate";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 1;
		this.gridColumn5.Width = 129;
		this.gridColumn6.Caption = "Remarks";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "Remarks";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 2;
		this.gridColumn6.Width = 403;
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(76, 456);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(70, 20);
		this.btn_Cancel.TabIndex = 21;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 456);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(70, 20);
		this.btn_Save.TabIndex = 20;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.lbBranch.EditControl = null;
		this.lbBranch.EditValue = "Item Code";
		this.lbBranch.FieldName = null;
		this.lbBranch.HideWhenEmpty = false;
		this.lbBranch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBranch.Location = new System.Drawing.Point(9, 7);
		this.lbBranch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
		this.lbBranch.Name = "lbBranch";
		this.lbBranch.ReadOnly = false;
		this.lbBranch.SettingInVisible = false;
		this.lbBranch.SettingReadonly = false;
		this.lbBranch.ShowUnderLine = true;
		this.lbBranch.Size = new System.Drawing.Size(70, 20);
		this.lbBranch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBranch.TabIndex = 23;
		this.lbBranch.Utext = "Item Code";
		this.bte_ItemCode.EditControl = null;
		this.bte_ItemCode.FieldName = null;
		this.bte_ItemCode.IsSearchControl = false;
		this.bte_ItemCode.Location = new System.Drawing.Point(77, 4);
		this.bte_ItemCode.Name = "bte_ItemCode";
		this.bte_ItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_ItemCode.Properties.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.buttonEdit1_Properties_ButtonPressed);
		this.bte_ItemCode.SettingInVisible = false;
		this.bte_ItemCode.SettingReadonly = false;
		this.bte_ItemCode.Size = new System.Drawing.Size(100, 20);
		this.bte_ItemCode.TabIndex = 27;
		this.bte_ItemCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bte_ItemCode_KeyDown);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(767, 481);
		base.Controls.Add(this.bte_ItemCode);
		base.Controls.Add(this.splitContainer1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.lbBranch);
		base.FormId = "oItemQuotaForPur";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ItemQuotaArrangement";
		this.Text = "Item Quota for Purchase";
		base.Load += new System.EventHandler(this.frm_ItemQuotaArrangement_Load);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).EndInit();
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Items).EndInit();
		this.ctm_Items.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Items).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Item_ItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Period).EndInit();
		this.ctm_Vendors.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Period).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Date).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
