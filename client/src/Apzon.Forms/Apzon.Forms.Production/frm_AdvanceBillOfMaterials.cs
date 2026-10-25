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
using Apzon.Entities.Models.Production;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Interfaces.FormDocument;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Production;

public class frm_AdvanceBillOfMaterials : XtraFormEx
{
	private const string OkString = "OK";

	private const string UpdateString = "Update";

	private const string SaveString = "Save";

	private const string Controller = "BillOfMasterials";

	private bool _isLoad;

	private GridViewExPainter painter;

	private IContainer components;

	private SplitContainerControl splitContainerControl;

	private SimpleButtonEx btnSave;

	private SimpleButtonEx btnCancel;

	private LabelLine lbCode;

	private SimpleButtonEx btnSearch;

	private GridSplitContainer gridSplitContainer1;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridControlEx grdLine;

	private GridViewEx gvLine;

	private GridSplitContainer gridSplitContainer2;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcName;

	private GridColumnEx gcVersion;

	private GridColumnEx gcStatus;

	private GridColumnEx gcActForm;

	private GridColumnEx gcActiveTo;

	private RepositoryItemButtonEdit rItemCode;

	private RepositoryItemLookUpEdit rStatus;

	private RepositoryItemDateEdit rDate;

	private GridColumnEx gcType;

	private RepositoryItemLookUpEdit rType;

	private GridColumnEx gcLine;

	private RepositoryItemButtonEdit rLine;

	private GridColumnEx gcWhs;

	private RepositoryItemButtonEdit rWhs;

	private GridColumnEx gcDisRule;

	private RepositoryItemButtonEdit rDistriRule;

	private GridColumnEx gcProject;

	private RepositoryItemButtonEdit rProject;

	private GridColumnEx gcPriceList;

	private GridColumnEx gcLineType;

	private GridColumnEx gcItemCodeLine;

	private GridColumnEx gcNameLine;

	private GridColumnEx gcLineQuantity;

	private GridColumnEx gcLineAddQuantit;

	private GridColumnEx gcUoMNameLine;

	private GridColumnEx gcWarehouseLine;

	private GridColumnEx gcIssueMthLine;

	private GridColumnEx gcLineDisRule;

	private GridColumnEx gcLinePriceList;

	private GridColumnEx gcLineComment;

	private GridColumnEx gcQuantity;

	private RepositoryItemLookUpEdit rLineType;

	private RepositoryItemButtonEdit rLineWhs;

	private RepositoryItemLookUpEdit rLineIsueMth;

	private RepositoryItemButtonEdit rLineDisRule;

	private RepositoryItemLookUpEdit rLinePriceList;

	private TextEditEx txtVersionCode;

	private LabelLine lbVersion;

	private ContextMenuStrip contextMenuStripDoc;

	private ContextMenuStrip contextMenuStripLine;

	private ToolStripMenuItem addToolStripMenuItem;

	private RepositoryItemLookUpEdit rPriceList;

	private ButtonEditEx txtItemCode;

	private GridColumnEx gcWipCodeLine;

	private GridColumnEx gcLinePrice;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private GridColumnEx gcRemark;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private GridColumnEx gcBomType;

	private RepositoryItemLookUpEdit rTreeType;

	private RepositoryItemCalcEdit rDocCal;

	private RepositoryItemCalcEdit rLineCal;

	private RepositoryItemCalcEdit rLinePrice;

	private RepositoryItemButtonEdit rCodeLine;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
			{
				WinFuntions.DisableAllControl(Controls);
				btnCancel.Enabled = true;
				return;
			}
			_formMode = value;
			switch (value)
			{
			case Enums.FormViewModeType.View:
				EnableDisableControl(isEnable: true);
				btnSave.Text = "OK";
				break;
			case Enums.FormViewModeType.Busy:
				EnableDisableControl(isEnable: false);
				break;
			case Enums.FormViewModeType.Update:
				EnableDisableControl(isEnable: true);
				btnSave.Text = "Update";
				break;
			case Enums.FormViewModeType.Add:
				EnableDisableControl(isEnable: true);
				btnSave.Text = "Save";
				break;
			case Enums.FormViewModeType.Find:
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_AdvanceBillOfMaterials()
	{
		InitializeComponent();
		OnLoadAsync += frm_AdvanceBillOfMaterials_Load;
	}

	public frm_AdvanceBillOfMaterials(string bomCode)
	{
		InitializeComponent();
		OnLoadAsync += frm_AdvanceBillOfMaterials_Load;
		txtItemCode.EditValue = bomCode;
	}

	private void gvLine_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gvDoc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async Task frm_AdvanceBillOfMaterials_Load(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			LoadDocType();
			LoadDocStatus();
			LoadLineType();
			LoadLineIssueMth();
			await LoadPricelist();
			LoadBomType();
			await LoadData();
			if (painter == null)
			{
				painter = new GridViewExPainter(gvLine);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private async void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		if (!_isLoad)
		{
			WinCommons._OpenProcessing();
			try
			{
				grdLine.DataSourceTable.Clear();
				await LoadDataLine(Function.ParseInt(gvDoc.GetFocusedRowCellValue("BomVerID")));
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
			WinCommons._CloseProcessing();
		}
	}

	private async void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			await Addnew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	private async void txtItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			if (e.Button.Index == 0)
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.BillOfMater, multiSelect: false, Function.ToString(txtItemCode.EditValue));
				frm_ChooseFromList2._isBOMSearch = false;
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					txtItemCode.EditValue = ((BillOfMasterials_Impl_OITT)frm_ChooseFromList2.ChooseItem).Code;
				}
				await LoadData();
			}
			else
			{
				string text = Function.ToString(txtItemCode.EditValue);
				if (!string.IsNullOrWhiteSpace(text))
				{
					ExtenseionMethods.LoadBoMFromGridItem(text, MdiParent);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string fieldName = gvDoc.FocusedColumn.FieldName;
			if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode.FieldName))) && gvDoc.FocusedColumn.FieldName != gcItemCode.FieldName)
			{
				e.Cancel = true;
			}
			if ((fieldName == gcActForm.FieldName || fieldName == gcActiveTo.FieldName) && Function.ToString(gvDoc.GetFocusedRowCellValue(gcStatus.FieldName)).Equals("N"))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvLine_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode.FieldName))))
			{
				e.Cancel = true;
				return;
			}
			if (Function.ParseInt(gvLine.GetFocusedRowCellValue(gcType.FieldName)) <= 0 && gvLine.FocusedColumn.FieldName != gcLineType.FieldName && gvLine.FocusedColumn != gcItemCodeLine)
			{
				e.Cancel = true;
			}
			if (string.IsNullOrEmpty(Function.ToString(gvLine.GetFocusedRowCellValue(gcItemCodeLine.FieldName))) && gvLine.FocusedColumn.FieldName != gcItemCodeLine.FieldName && gvLine.FocusedColumn.FieldName != gcLineType.FieldName)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		Enums.FormViewModeType oldview = FormMode;
		try
		{
			if (e.Column.FieldName == gcItemCode.FieldName)
			{
				WinCommons._OpenProcessing();
				FormMode = Enums.FormViewModeType.Busy;
				grdLine.DataSource = null;
				DataSet dataSet = await GetDataBomSap(Function.ToString(e.Value));
				if (dataSet != null && dataSet.Tables.Count >= 2 && dataSet.Tables[0].Rows.Count > 0 && dataSet.Tables[1].Rows.Count > 0)
				{
					dataSet.Tables[1].Columns["Quantity"].DefaultValue = 1;
					dataSet.Tables[1].Columns["Price"].DefaultValue = 0;
					dataSet.Tables[1].Columns["AddQuantit"].DefaultValue = 0;
					dataSet.Tables[1].Columns["IssueMthd"].DefaultValue = "B";
					dataSet.Tables[1].Columns["WipActCode"].DefaultValue = "";
					dataSet.Tables[1].Columns["ChildNum"].DefaultValue = 0;
					dataSet.Tables[1].Columns["Father"].DefaultValue = 0;
					dataSet.Tables[1].Columns["Type"].DefaultValue = 4;
					dataSet.Tables[0].Rows[0]["BomVerID"] = 0;
					dataSet.Tables[0].AcceptChanges();
					_isLoad = true;
					grdDoc.DataSource = dataSet.Tables[0];
					grdLine.DataSource = dataSet.Tables[1];
				}
				grdDoc.DataSourceTable.AcceptChanges();
				grdLine.DataSourceTable.AcceptChanges();
				await AddnewRowLine();
				if (!string.IsNullOrEmpty(Function.ToString(e.Value)))
				{
					DataTable aDoc = HttpClientHelperFactory.GetHttpClient().Items.GetADoc(Function.ToString(e.Value));
					if (aDoc.IsNotNull())
					{
						gvDoc.SetFocusedRowCellValue(gcName.FieldName, aDoc.Rows[0]["ItemName"]);
						if (!string.IsNullOrEmpty(Function.ToString(aDoc.Rows[0]["DfltWH"])))
						{
							gvDoc.SetFocusedRowCellValue(gcWhs.FieldName, aDoc.Rows[0]["DfltWH"]);
						}
						else
						{
							gvDoc.SetFocusedRowCellValue(gcWhs.FieldName, GetWhsDefault(GlobalData.UserName, -2));
						}
					}
					GridViewEx gridViewEx = gvDoc;
					string fieldName = gcVersion.FieldName;
					gridViewEx.SetFocusedRowCellValue(fieldName, await GetNewVersionBom(Function.ToString(e.Value)));
				}
				grdDoc.DataSourceTable.AcceptChanges();
				grdLine.DataSourceTable.AcceptChanges();
				grdDoc.RefreshDataSource();
				grdLine.RefreshDataSource();
				FormMode = Enums.FormViewModeType.Add;
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			FormMode = oldview;
		}
		_isLoad = false;
		WinCommons._CloseProcessing();
	}

	private async void gvLine_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == gcItemCodeLine.FieldName)
			{
				LoadItemDetails(gvLine, gvDoc, e);
				await AddnewRowLine();
			}
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

	private void rItemCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItems, false, Function.ToString(txtItemCode.EditValue), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ListMandatoryField = "WarehouseCode";
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue(gcItemCode.FieldName, frm_CFL2.ChooseItem["ItemCode"]);
					if (!string.IsNullOrEmpty(Function.ToString(frm_CFL2.ChooseItem["WarehouseCode"])))
					{
						gvDoc.SetFocusedRowCellValue(gcWhs.FieldName, Function.ToString(frm_CFL2.ChooseItem["WarehouseCode"]));
					}
					else
					{
						gvDoc.SetFocusedRowCellValue(gcWhs.FieldName, GetWhsDefault(GlobalData.UserName, -2));
					}
					gvDoc.SetFocusedRowCellValue(gcPriceList, 1);
				}
			}
			else
			{
				string text = Function.ToString(gvDoc.GetFocusedRowCellValue("Code"));
				if (!string.IsNullOrWhiteSpace(text))
				{
					ExtenseionMethods.LoadBoMFromGridItem(text, MdiParent);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void rCodeLine_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		WinCommons._OpenProcessing();
		Enums.FormViewModeType oldview = FormMode;
		try
		{
			string text = Function.ToString(gvLine.GetFocusedRowCellValue(gcItemCodeLine.FieldName));
			if (e.Button.Index == 0)
			{
				if (Function.ParseInt(gvLine.GetFocusedRowCellValue("Type")) == -18)
				{
					WinCommons._CloseProcessing();
					return;
				}
				FormMode = Enums.FormViewModeType.Busy;
				grdLine.RefreshDataSource();
				grdLine.DataSourceTable.AcceptChanges();
				int num = Function.ParseInt(gvLine.GetFocusedRowCellValue(gcLineType.FieldName));
				Enums.TypeCFL type = Enums.TypeCFL.BoMItem;
				if (num == 290)
				{
					type = Enums.TypeCFL.ListMasterResource;
				}
				frm_CFL frm_CFL2 = new frm_CFL(type, true, text, "", Enums.ItemObjectType.NoItemType, null, "")
				{
					BomFather = Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode))
				};
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
				{
					int focusedRowHandle = gvLine.FocusedRowHandle;
					gvLine.DeleteRow(gvLine.FocusedRowHandle);
					grdLine.RefreshDataSource();
					grdLine.DataSourceTable.AcceptChanges();
					DataTable dataSourceTable = grdLine.DataSourceTable;
					dataSourceTable.BeginLoadData();
					if (num == 290)
					{
						for (int num2 = frm_CFL2.ChooseItems.Count - 1; num2 >= 0; num2--)
						{
							DataRow dataRow = dataSourceTable.NewRow();
							dataRow["Father"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("BomVerID"));
							dataRow["Code"] = Function.ToString(frm_CFL2.ChooseItems[num2]["ResCode"]);
							dataRow["Type"] = num;
							dataRow["PriceList"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue(gcPriceList.FieldName));
							dataRow["ItemName"] = Function.ToString(frm_CFL2.ChooseItems[num2]["ResName"]);
							dataSourceTable.Rows.InsertAt(dataRow, focusedRowHandle);
						}
					}
					else
					{
						for (int num3 = frm_CFL2.ChooseItems.Count - 1; num3 >= 0; num3--)
						{
							DataRow dataRow2 = dataSourceTable.NewRow();
							dataRow2["Father"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("BomVerID"));
							dataRow2["Code"] = Function.ToString(frm_CFL2.ChooseItems[num3]["ItemCode"]);
							dataRow2["PriceList"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue(gcPriceList.FieldName));
							dataRow2["ItemName"] = Function.ToString(frm_CFL2.ChooseItems[num3]["ItemDescription"]);
							dataRow2["IssueMthd"] = Function.ToString(frm_CFL2.ChooseItems[num3]["IssueMthd"]);
							if (!string.IsNullOrEmpty(Function.ToString(frm_CFL2.ChooseItems[num3]["WarehouseCode"])))
							{
								dataRow2["Warehouse"] = Function.ToString(frm_CFL2.ChooseItems[num3]["WarehouseCode"]);
							}
							else
							{
								dataRow2["Warehouse"] = GetWhsDefault(GlobalData.UserName, -2);
							}
							dataRow2["Price"] = Function.ToString(frm_CFL2.ChooseItems[num3]["Price"]);
							dataRow2["Uom"] = "Manual";
							dataRow2["PriceList"] = 1;
							dataSourceTable.Rows.InsertAt(dataRow2, focusedRowHandle);
						}
					}
					dataSourceTable.AcceptChanges();
					dataSourceTable.EndLoadData();
					grdLine.DataSource = dataSourceTable;
					await AddnewRowLine();
				}
				if (oldview == Enums.FormViewModeType.View)
				{
					oldview = Enums.FormViewModeType.Update;
				}
			}
			else if (Function.ParseInt(gvLine.GetFocusedRowCellValue("Type")) != -18)
			{
				if (!string.IsNullOrWhiteSpace(text))
				{
					ExtenseionMethods.LoadBoMFromGridItem(text, MdiParent);
				}
			}
			else
			{
				frm_RowTextDetails frm_RowTextDetails2 = new frm_RowTextDetails();
				frm_RowTextDetails2.RowText = Function.ToString(gvLine.GetFocusedRowCellValue("Code"));
				if (frm_RowTextDetails2.ShowDialog(this) == DialogResult.OK)
				{
					gvLine.SetRowCellValue(gvLine.FocusedRowHandle, "Code", frm_RowTextDetails2.RowText);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldview;
		WinCommons._CloseProcessing();
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
				await Save();
				break;
			case Enums.FormViewModeType.View:
				Close();
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void rLine_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0 && !string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode.FieldName))))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Lines, false, Function.ToString(gvDoc.GetFocusedRowCellValue(gcLine.FieldName)), "", Enums.ItemObjectType.NoItemType, null, "")
				{
					Params = new List<string> { "0" }
				};
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue(gcLine.FieldName, Function.ToString(frm_CFL2.ChooseItem["ID"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rWhs_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode.FieldName))))
				{
					frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetListWareHouse, false, Function.ToString(gvDoc.GetFocusedRowCellValue(gcWhs.FieldName)), "", Enums.ItemObjectType.NoItemType, null, "");
					frm_CFL2.ShowDialog(this);
					if (frm_CFL2.ChooseItem != null)
					{
						gvDoc.SetFocusedRowCellValue(gcWhs.FieldName, Function.ToString(frm_CFL2.ChooseItem["WhsCode"]));
					}
				}
			}
			else
			{
				string text = Function.ToString(gvDoc.GetFocusedRowCellValue(gcWhs.FieldName));
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rLineWhs_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode.FieldName))))
				{
					frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.GetListWareHouse, false, Function.ToString(gvLine.GetFocusedRowCellValue(gcWarehouseLine.FieldName)), "", Enums.ItemObjectType.NoItemType, null, "");
					frm_CFL2.ShowDialog(this);
					if (frm_CFL2.ChooseItem != null)
					{
						gvLine.SetFocusedRowCellValue(gcWarehouseLine.FieldName, Function.ToString(frm_CFL2.ChooseItem["WhsCode"]));
					}
				}
			}
			else
			{
				string text = Function.ToString(gvDoc.GetFocusedRowCellValue(gcWhs.FieldName));
				if (!string.IsNullOrEmpty(text))
				{
					this.DrillDown(ApzObjectTypes.oWarehouses, ApzObjectTypes.oWarehouses, text);
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
			if (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode.FieldName))))
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Project, false, Function.ToString(gvDoc.GetFocusedRowCellValue(gcProject.FieldName)), "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue(gcProject.FieldName, Function.ToString(frm_CFL2.ChooseItem["PrjCode"]));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rDistriRule_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvDoc.GetFocusedRowCellValue("DisRule"));
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.DistributeRule, multiSelect: false, "");
			frm_ChooseFromList2._strOcrDistRule = Function.ToString(text);
			frm_ChooseFromList2.ShowDialog(this);
			DataTable dtSource = frm_ChooseFromList2._dtSource;
			string text2 = "";
			if (dtSource != null && dtSource.Rows.Count > 0)
			{
				for (int i = 0; i < dtSource.Rows.Count; i++)
				{
					text2 = text2 + Function.ToString(dtSource.Rows[i]["OcrCode"]) + ";";
				}
				if (text2.Length > 1)
				{
					text2 = text2.Remove(text2.Length - 1);
				}
			}
			string value = text2.Replace(";", "");
			if (string.IsNullOrWhiteSpace(text) || !string.IsNullOrWhiteSpace(value))
			{
				gvDoc.SetFocusedRowCellValue("DisRule", text2);
			}
			grdDoc.RefreshDataSource();
			grdDoc.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void rLineDisRule_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			string text = Function.ToString(gvLine.GetFocusedRowCellValue("DisRule"));
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.DistributeRule, multiSelect: false, "");
			frm_ChooseFromList2._strOcrDistRule = Function.ToString(text);
			frm_ChooseFromList2.ShowDialog(this);
			DataTable dtSource = frm_ChooseFromList2._dtSource;
			string text2 = "";
			if (dtSource != null && dtSource.Rows.Count > 0)
			{
				for (int i = 0; i < dtSource.Rows.Count; i++)
				{
					text2 = text2 + Function.ToString(dtSource.Rows[i]["OcrCode"]) + ";";
				}
				if (text2.Length > 1)
				{
					text2 = text2.Remove(text2.Length - 1);
				}
			}
			string value = text2.Replace(";", "");
			if (string.IsNullOrWhiteSpace(text) || !string.IsNullOrWhiteSpace(value))
			{
				gvLine.SetFocusedRowCellValue("DisRule", text2);
			}
			grdLine.RefreshDataSource();
			grdLine.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvLine_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (Function.ParseInt(gvLine.GetRowCellValue(e.RowHandle, "Type")) != -18)
			{
				string fieldName = e.Column.FieldName;
				if (fieldName == gcItemCodeLine.FieldName || fieldName == gcLinePriceList.FieldName)
				{
					gvLine.SetFocusedRowCellValue(gcLinePrice.FieldName, GetPriceItem(Function.ToString(gvLine.GetFocusedRowCellValue(gcItemCodeLine.FieldName)), Function.ParseInt(gvDoc.GetFocusedRowCellValue(gcLinePriceList.FieldName))));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gvLine.FocusedRowHandle >= 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteSelectedRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				gvLine.DeleteSelectedRows();
				grdLine.RefreshDataSource();
				grdLine.DataSourceTable.AcceptChanges();
				await AddnewRowLine();
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

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		if (gvDoc.GetFocusedDataRow() == null)
		{
			return;
		}
		Enums.FormViewModeType view = FormMode;
		try
		{
			if ((FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "BillOfMasterials");
				HttpResult httpResult = await http.PostAsync("DeleteBomVersion", Function.ParseInt(gvDoc.GetFocusedRowCellValue("BomVerID")));
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					await LoadData();
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					view = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = view;
		WinCommons._CloseProcessing();
	}

	private async void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		await LoadData();
	}

	private void grdLine_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				WinFuntions.PasteFromExcelToDataTableBomDetail(gvLine, new List<string> { "Code" }, Function.ParseInt(gvLine.GetFocusedRowCellValue("Type")));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grdLine_Paint(object sender, PaintEventArgs e)
	{
		try
		{
			DrawMergedCells(e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void DrawMergedCells(PaintEventArgs e)
	{
		DataTable dataSourceTable = grdLine.DataSourceTable;
		for (int i = 0; i < dataSourceTable.Rows.Count; i++)
		{
			if (Function.ParseInt(dataSourceTable.Rows[i]["Type"]) != -18)
			{
				continue;
			}
			GridColumn col = gvLine.Columns.FirstOrDefault((GridColumn x) => x.VisibleIndex == gvLine.Columns.Max((GridColumn t) => t.VisibleIndex));
			GridViewExMergeCell cell = new GridViewExMergeCell(i, gcItemCodeLine, col);
			painter.DrawMergedCell(cell, e);
		}
	}

	private void rLineType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			gvLine.CloseEditor();
			if (Function.ParseInt(gvLine.GetFocusedRowCellValue("Type")) == -18)
			{
				foreach (GridColumn column in gvLine.Columns)
				{
					if (column != gcLineType)
					{
						gvLine.SetFocusedRowCellValue(column, null);
					}
				}
				frm_RowTextDetails frm_RowTextDetails2 = new frm_RowTextDetails();
				frm_RowTextDetails2.RowText = Function.ToString(gvLine.GetFocusedRowCellValue("Code"));
				if (frm_RowTextDetails2.ShowDialog(this) == DialogResult.OK)
				{
					gvLine.SetRowCellValue(gvLine.FocusedRowHandle, "Code", frm_RowTextDetails2.RowText);
				}
			}
			else
			{
				gvLine.SetFocusedRowCellValue(gcItemCodeLine, null);
				gvLine.SetFocusedRowCellValue(gcPriceList, null);
			}
			grdLine.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			await LoadDataNextLast(Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			await LoadDataNextLast(Enums.NavigateType.Next);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			await LoadDataNextLast(Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			await LoadDataNextLast(Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	public async Task MODE_FIND()
	{
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH()
	{
		WinCommons._OpenProcessing();
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			await Addnew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH_DATA()
	{
		WinCommons._OpenProcessing();
		try
		{
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void LoadDocType()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Normal",
					Code = "N"
				},
				new Common.Table_Code
				{
					Name = "Special",
					Code = "S"
				}
			};
			rType.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadDocStatus()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Active",
					Code = "Y"
				},
				new Common.Table_Code
				{
					Name = "InActive",
					Code = "N"
				}
			};
			rStatus.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadLineType()
	{
		try
		{
			List<Common.Table_ID> dataSource = new List<Common.Table_ID>
			{
				new Common.Table_ID
				{
					Name = "Item",
					ID = 4
				},
				new Common.Table_ID
				{
					Name = "Resource",
					ID = 290
				},
				new Common.Table_ID
				{
					Name = "Text",
					ID = -18
				}
			};
			rLineType.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadLineIssueMth()
	{
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Backflush",
					Code = "B"
				},
				new Common.Table_Code
				{
					Name = "Manual",
					Code = "M"
				}
			};
			rLineIsueMth.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadPricelist()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataSource = await http.GetAsync("GetDataPricelist");
			rPriceList.DataSource = dataSource;
			rLinePriceList.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			txtItemCode.Properties.Buttons[0].Enabled = isEnable;
			txtVersionCode.ReadOnly = !isEnable;
			btnSave.Enabled = isEnable;
			btnCancel.Enabled = isEnable;
			btnSearch.Enabled = isEnable;
			gvDoc.ReadOnly = !isEnable;
			gvLine.ReadOnly = !isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetDataDoc()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BillOfMasterials");
		return await http.GetAsync("GetDataDocBomVersion", new
		{
			code = txtItemCode.Text,
			version = txtVersionCode.Text
		});
	}

	private async Task LoadDataLine(int bomId)
	{
		DataTable dataTable;
		using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BillOfMasterials"))
		{
			dataTable = await http.GetAsync("GetDataLineBomVersion", new
			{
				id = bomId
			});
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				dataTable.Columns["Quantity"].DefaultValue = 1;
				dataTable.Columns["Price"].DefaultValue = 0;
				dataTable.Columns["AddQuantit"].DefaultValue = 0;
				dataTable.Columns["IssueMthd"].DefaultValue = "B";
				dataTable.Columns["WipActCode"].DefaultValue = "";
				dataTable.Columns["ChildNum"].DefaultValue = 0;
				dataTable.Columns["Father"].DefaultValue = 0;
				dataTable.Columns["Type"].DefaultValue = 4;
			}
		}
		grdLine.DataSource = dataTable;
		grdLine.RefreshDataSource();
		if (gvDoc.FocusedRowHandle >= 0)
		{
			await AddnewRowLine();
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataSource = await GetDataDoc();
			grdDoc.DataSource = dataSource;
			grdDoc.RefreshDataSource();
			gvDoc.FocusedRowHandle = 0;
			if (gvDoc.FocusedRowHandle < 0)
			{
				gvDoc_FocusedRowObjectChanged(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataTable> GetColumnsDocBomVersion()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BillOfMasterials");
		return await http.GetAsync("GetColumnsDocBomVersion");
	}

	private async Task<DataTable> GetColumnsLineBomVersion()
	{
		using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BillOfMasterials");
		return await http.GetAsync("GetColumnsLineBomVersion");
	}

	private async Task Addnew()
	{
		_ = 1;
		try
		{
			DataTable dataTable = grdDoc.DataSourceTable.Clone();
			if (dataTable.Columns.Count == 0)
			{
				dataTable = await GetColumnsDocBomVersion();
			}
			if (dataTable != null && dataTable.Columns.Count > 0)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["BomVerID"] = 0;
				dataRow[gcQuantity.FieldName] = 1;
				dataRow["CreateDate"] = DateTime.Now.Date;
				dataRow["Status"] = "Y";
				dataRow["ActFrom"] = DateTime.Now.Date;
				dataRow["ActTo"] = DateTime.Now.Date;
				dataRow["Type"] = "S";
				dataRow["TreeType"] = "P";
				dataRow["ToWH"] = DBNull.Value;
				dataRow["PriceList"] = 1;
				dataTable.Rows.Add(dataRow);
				_isLoad = true;
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
				_isLoad = false;
				if (gvDoc.Columns.FirstOrDefault((GridColumn col) => col.ColumnEditName.Contains("rep_line_")) == null)
				{
					await WinFuntions.CreateColumnsDistRule(gvDoc);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CanNotGetHeaderStruc2Save, "");
			}
			gvDoc.SetFocusedRowCellValue(gcItemCode.FieldName, txtItemCode.Text);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task AddnewRowLine()
	{
		_ = 1;
		try
		{
			DataTable dt = grdLine.DataSourceTable;
			if (dt.Columns.Count == 0)
			{
				dt = await GetColumnsLineBomVersion();
				if (dt != null && dt.Columns.Count > 0)
				{
					dt.Columns["Quantity"].DefaultValue = 1;
					dt.Columns["Price"].DefaultValue = 0;
					dt.Columns["AddQuantit"].DefaultValue = 0;
					dt.Columns["IssueMthd"].DefaultValue = "B";
					dt.Columns["WipActCode"].DefaultValue = "";
					dt.Columns["ChildNum"].DefaultValue = 0;
					dt.Columns["Father"].DefaultValue = 0;
					dt.Columns["Type"].DefaultValue = 4;
					dt.Columns["Uom"].DefaultValue = "Manual";
					dt.Columns["PriceList"].DefaultValue = 1;
					if (gvLine.Columns.FirstOrDefault((GridColumn col) => col.ColumnEditName.Contains("rep_line_")) == null)
					{
						await WinFuntions.CreateColumnsDistRule(gvLine);
					}
				}
			}
			if (!CheckExistLineBank(dt))
			{
				if (dt != null)
				{
					DataRow row = dt.NewRow();
					dt.Rows.Add(row);
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.CannotGetLineStructure, "");
				}
			}
			grdLine.DataSource = dt;
			grdLine.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool CheckExistLineBank(DataTable dt)
	{
		try
		{
			if (dt == null)
			{
				return true;
			}
			if (dt.Rows.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				if (string.IsNullOrEmpty(Function.ToString(dt.Rows[i][gcItemCodeLine.FieldName])))
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
			return true;
		}
	}

	private async Task<DataSet> GetDataBomSap(string productionNo)
	{
		using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "BillOfMasterials");
		return await http.GetAsync("GetDataBomSap", new { productionNo });
	}

	private static void LoadItemDetails(GridView line, GridView doc, CellValueChangedEventArgs e)
	{
		try
		{
			string text = Function.ToString(line.GetRowCellValue(e.RowHandle, "Code"));
			if (string.IsNullOrWhiteSpace(text))
			{
				return;
			}
			string text2 = ((Function.ParseInt(line.GetRowCellValue(e.RowHandle, "Type")) == 290) ? "R" : "I");
			DataTable itemDetails = GetItemDetails(text2, text);
			if (itemDetails != null && itemDetails.Rows.Count > 0)
			{
				DataTable dataTable = line.GridControl.DataSourceTable();
				DataRow dataRow = dataTable.Rows[line.GetDataSourceRowIndex(e.RowHandle)];
				DataRow dataRow2 = itemDetails.Rows[0];
				if (text2.Equals("R"))
				{
					dataRow["ItemName"] = Function.ToString(dataRow2["ResName"]);
					dataRow["Code"] = Function.ToString(dataRow2["ResCode"]);
					dataRow["Father"] = Function.ParseInt(doc.GetFocusedRowCellValue("BomVerID"));
					dataRow["Price"] = 0;
					dataRow["PriceList"] = Function.ParseInt(doc.GetFocusedRowCellValue("PriceList"));
					dataRow["Total"] = 0;
					dataRow["Quantity"] = 1;
					dataRow["IssueMthd"] = "B";
				}
				else
				{
					dataRow["ItemName"] = Function.ToString(dataRow2["ItemName"]);
					dataRow["Father"] = Function.ParseInt(doc.GetFocusedRowCellValue("BomVerID"));
					dataRow["PriceList"] = Function.ParseInt(doc.GetFocusedRowCellValue("PriceList"));
					dataRow["Warehouse"] = Function.ToString(dataRow2["DefaultWarehouse"]);
					dataRow["Price"] = 0;
					dataRow["Quantity"] = 1;
					dataRow["IssueMthd"] = "B";
				}
				dataTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private static DataTable GetItemDetails(string type, string code)
	{
		DataTable result = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BillOfMasterials");
			result = httpClientHelper.Get("GetItemDetails", new { type, code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private async Task<string> GetItemNameByCode(string code)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Commons");
			return await http.GetAsync("GetItemNameByCode", new { code });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return "";
		}
	}

	private async Task Save()
	{
		Enums.FormViewModeType oldview = FormMode;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				FormMode = Enums.FormViewModeType.Busy;
				DataTable doc = new DataTable();
				string dataDoc = GetDataDoc(ref doc);
				if (string.IsNullOrEmpty(dataDoc))
				{
					DataTable line = new DataTable();
					dataDoc = GetDataLine(ref line);
					if (string.IsNullOrEmpty(dataDoc))
					{
						DataSet dataSet = new DataSet();
						dataSet.Tables.Add(doc);
						dataSet.Tables.Add(line);
						using (HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "BillOfMasterials"))
						{
							dataDoc = (await http.PostAsync((oldview == Enums.FormViewModeType.Add) ? "Save" : "Update", dataSet)).Message;
						}
						if (string.IsNullOrEmpty(dataDoc))
						{
							await LoadData();
							WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
							oldview = Enums.FormViewModeType.View;
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, dataDoc);
						}
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, dataDoc);
					}
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, dataDoc);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldview;
		WinCommons._CloseProcessing();
	}

	private string GetDataDoc(ref DataTable doc)
	{
		if (doc == null)
		{
			throw new ArgumentNullException("doc");
		}
		doc = grdDoc.DataSourceTable.Clone();
		doc.ImportRow(gvDoc.GetFocusedDataRow());
		if (string.IsNullOrEmpty(Function.ToString(doc.Rows[0][gcItemCode.FieldName])))
		{
			return "Production code could not null!";
		}
		if (string.IsNullOrEmpty(Function.ToString(doc.Rows[0][gcWhs.FieldName])))
		{
			return "Whs could not null!";
		}
		string text = Function.ToString(doc.Rows[0]["DisRule"]);
		if (!string.IsNullOrWhiteSpace(text))
		{
			string[] array = text.Split(';');
			for (int i = 0; i < array.Length; i++)
			{
				if (!string.IsNullOrWhiteSpace(array[i]))
				{
					switch (i)
					{
					case 0:
						doc.Rows[0]["OcrCode"] = array[0];
						break;
					case 1:
						doc.Rows[0]["OcrCode2"] = array[1];
						break;
					case 2:
						doc.Rows[0]["OcrCode3"] = array[2];
						break;
					case 3:
						doc.Rows[0]["OcrCode4"] = array[3];
						break;
					case 4:
						doc.Rows[0]["OcrCode5"] = array[4];
						break;
					}
				}
			}
		}
		doc.Columns.RemoveAt(doc.Columns.Count - 1);
		doc.Columns.RemoveAt(doc.Columns.Count - 1);
		if (!(Function.ParseDouble(doc.Rows[0][gcQuantity.FieldName]) <= 0.0))
		{
			return "";
		}
		return "Quantity must greater than zero!";
	}

	private string GetDataLine(ref DataTable line)
	{
		if (line == null)
		{
			throw new ArgumentNullException("line");
		}
		line = grdLine.DataSourceTable.Copy();
		if (line.IsNotNull())
		{
			for (int i = 0; i < line.Rows.Count; i++)
			{
				if (Function.ParseInt(line.Rows[i]["Type"]) == -18)
				{
					continue;
				}
				if (string.IsNullOrEmpty(Function.ToString(line.Rows[i][gcItemCodeLine.FieldName])))
				{
					line.Rows.RemoveAt(i);
					continue;
				}
				if (string.IsNullOrEmpty(Function.ToString(line.Rows[i][gcWarehouseLine.FieldName])))
				{
					return "Item Code " + Function.ToString(line.Rows[i][gcItemCodeLine.FieldName]) + ", whs can't null!";
				}
				if (Function.ParseDouble(line.Rows[i][gcLineQuantity.FieldName]) <= 0.0)
				{
					return "Item Code " + Function.ToString(line.Rows[i][gcItemCodeLine.FieldName]) + ", quantity must greater than zero!";
				}
				line.Rows[i]["ChildNum"] = i;
				string text = Function.ToString(line.Rows[i]["DisRule"]);
				if (string.IsNullOrWhiteSpace(text))
				{
					continue;
				}
				string[] array = text.Split(';');
				for (int j = 0; j < array.Length; j++)
				{
					if (!string.IsNullOrWhiteSpace(array[j]))
					{
						switch (j)
						{
						case 0:
							line.Rows[i]["OcrCode"] = array[0];
							break;
						case 1:
							line.Rows[i]["OcrCode2"] = array[1];
							break;
						case 2:
							line.Rows[i]["OcrCode3"] = array[2];
							break;
						case 3:
							line.Rows[i]["OcrCode4"] = array[3];
							break;
						case 4:
							line.Rows[i]["OcrCode5"] = array[4];
							break;
						}
					}
				}
			}
			line.Columns.RemoveAt(line.Columns.Count - 1);
			line.Columns.RemoveAt(line.Columns.Count - 1);
			line.AcceptChanges();
			if (line.Rows.Count == 0)
			{
				return "Data could not null!";
			}
			return "";
		}
		return "Data could not null!";
	}

	private double GetPriceItem(string code, int pricelist)
	{
		double result = 0.0;
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = httpClientHelper.Get("GetPriceItem", new
			{
				priceList = pricelist,
				itemcode = code
			});
			result = ((dataTable != null && dataTable.Rows.Count != 0) ? Function.ParseDouble(dataTable.Rows[0]["Price"]) : 0.0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return result;
	}

	private async Task<string> GetNewVersionBom(string itemcode)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "BillOfMasterials");
			return await http.GetAsync("GetNewVersionBom", new { itemcode });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return "";
	}

	private async Task LoadDataNextLast(Enums.NavigateType type)
	{
		WinCommons._OpenProcessing();
		Enums.FormViewModeType oldview = FormMode;
		FormMode = Enums.FormViewModeType.Busy;
		try
		{
			DataTable dataTable;
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "BillOfMasterials"))
			{
				dataTable = await http.GetAsync("LoadDataNextLast", new
				{
					type = type,
					code = Function.ToString(txtItemCode.EditValue)
				});
			}
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				grdDoc.DataSource = null;
				txtItemCode.EditValue = "";
				grdDoc.RefreshDataSource();
			}
			else
			{
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
				txtItemCode.EditValue = Function.ToString(dataTable.Rows[0]["Code"]);
				gvDoc.FocusedRowHandle = 0;
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			FormMode = oldview;
		}
		WinCommons._CloseProcessing();
	}

	private void LoadBomType()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Name", typeof(string));
			dataTable.Columns.Add("Code", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = "Assembly";
			dataRow[1] = "A";
			dataTable.Rows.Add(dataRow);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2[0] = "Sales";
			dataRow2[1] = "S";
			dataTable.Rows.Add(dataRow2);
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3[0] = "Production";
			dataRow3[1] = "P";
			dataTable.Rows.Add(dataRow3);
			DataRow dataRow4 = dataTable.NewRow();
			dataRow4[0] = "Template";
			dataRow4[1] = "T";
			dataTable.Rows.Add(dataRow4);
			dataTable.AcceptChanges();
			rTreeType.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public string GetWhsDefault(string userName, int branch)
	{
		try
		{
			string result = string.Empty;
			using (HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, "Commons"))
			{
				result = httpClientHelper.Get("GetWhsDefault", new { userName, branch });
			}
			return result;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return string.Empty;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Production.frm_AdvanceBillOfMaterials));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainerControl = new DevExpress.XtraEditors.SplitContainerControl();
		this.gridSplitContainer1 = new DevExpress.XtraGrid.GridSplitContainer();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcVersion = new Apzon.Controls.Grid.GridColumnEx();
		this.gcStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcActForm = new Apzon.Controls.Grid.GridColumnEx();
		this.rDate = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gcActiveTo = new Apzon.Controls.Grid.GridColumnEx();
		this.gcType = new Apzon.Controls.Grid.GridColumnEx();
		this.rType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLine = new Apzon.Controls.Grid.GridColumnEx();
		this.rLine = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.rDocCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcWhs = new Apzon.Controls.Grid.GridColumnEx();
		this.rWhs = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDisRule = new Apzon.Controls.Grid.GridColumnEx();
		this.rDistriRule = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcProject = new Apzon.Controls.Grid.GridColumnEx();
		this.rProject = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcPriceList = new Apzon.Controls.Grid.GridColumnEx();
		this.rPriceList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBomType = new Apzon.Controls.Grid.GridColumnEx();
		this.rTreeType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridSplitContainer2 = new DevExpress.XtraGrid.GridSplitContainer();
		this.grdLine = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripLine = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvLine = new Apzon.Controls.Grid.GridViewEx();
		this.gcLineType = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcItemCodeLine = new Apzon.Controls.Grid.GridColumnEx();
		this.rCodeLine = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcNameLine = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineQuantity = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcLineAddQuantit = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUoMNameLine = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWarehouseLine = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineWhs = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcIssueMthLine = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineIsueMth = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineDisRule = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineDisRule = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcLinePriceList = new Apzon.Controls.Grid.GridColumnEx();
		this.rLinePriceList = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLinePrice = new Apzon.Controls.Grid.GridColumnEx();
		this.rLinePrice = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcLineComment = new Apzon.Controls.Grid.GridColumnEx();
		this.gcWipCodeLine = new Apzon.Controls.Grid.GridColumnEx();
		this.btnSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbCode = new Apzon.Controls.Controls.LabelLine();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtVersionCode = new Apzon.Controls.Editor.TextEditEx();
		this.lbVersion = new Apzon.Controls.Controls.LabelLine();
		this.txtItemCode = new Apzon.Controls.Editor.ButtonEditEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl).BeginInit();
		this.splitContainerControl.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer1).BeginInit();
		this.gridSplitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStripDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rWhs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDistriRule).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rProject).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rPriceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rTreeType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer2).BeginInit();
		this.gridSplitContainer2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdLine).BeginInit();
		this.contextMenuStripLine.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCodeLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineWhs).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineIsueMth).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineDisRule).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLinePriceList).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLinePrice).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtVersionCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtItemCode.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl.Horizontal = false;
		this.splitContainerControl.Location = new System.Drawing.Point(2, 34);
		this.splitContainerControl.Name = "splitContainerControl";
		this.splitContainerControl.Panel1.Controls.Add(this.gridSplitContainer1);
		this.splitContainerControl.Panel1.Text = "Panel1";
		this.splitContainerControl.Panel2.Controls.Add(this.gridSplitContainer2);
		this.splitContainerControl.Panel2.Text = "Panel2";
		this.splitContainerControl.Size = new System.Drawing.Size(796, 412);
		this.splitContainerControl.SplitterPosition = 183;
		this.splitContainerControl.TabIndex = 0;
		this.splitContainerControl.Text = "splitContainerControl1";
		this.gridSplitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridSplitContainer1.Grid = this.grdDoc;
		this.gridSplitContainer1.Location = new System.Drawing.Point(1, 0);
		this.gridSplitContainer1.Name = "gridSplitContainer1";
		this.gridSplitContainer1.Panel1.Controls.Add(this.grdDoc);
		this.gridSplitContainer1.Size = new System.Drawing.Size(794, 182);
		this.gridSplitContainer1.TabIndex = 0;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStripDoc;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[11]
		{
			this.rItemCode, this.rStatus, this.rDate, this.rType, this.rLine, this.rWhs, this.rDistriRule, this.rProject, this.rPriceList, this.rTreeType,
			this.rDocCal
		});
		this.grdDoc.Size = new System.Drawing.Size(794, 182);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStripDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addToolStripMenuItem, this.deleteToolStripMenuItem, this.refreshToolStripMenuItem });
		this.contextMenuStripDoc.Name = "contextMenuStripDoc";
		this.contextMenuStripDoc.Size = new System.Drawing.Size(114, 70);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(113, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[15]
		{
			this.gcItemCode, this.gcName, this.gcVersion, this.gcStatus, this.gcActForm, this.gcActiveTo, this.gcType, this.gcLine, this.gcQuantity, this.gcWhs,
			this.gcDisRule, this.gcProject, this.gcPriceList, this.gcRemark, this.gcBomType
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oBomVersion";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 30;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "APZ_OITT";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = new string[1] { "Version" };
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvDoc_CustomDrawRowIndicator);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gcItemCode.Caption = "Item Code";
		this.gcItemCode.ColumnEdit = this.rItemCode;
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "Code";
		this.gcItemCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.PrimaryKey = true;
		this.gcItemCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 0;
		this.gcItemCode.Width = 99;
		this.rItemCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemCode.Name = "rItemCode";
		this.rItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rItemCode_ButtonClick);
		this.gcName.Caption = "Item Name";
		this.gcName.CurrencyDateField = null;
		this.gcName.CurrencyField = null;
		this.gcName.CurrencyFixed = null;
		this.gcName.CurrencyFixedRate = 0.0;
		this.gcName.CurrencyRateField = null;
		this.gcName.FieldName = "ItemName";
		this.gcName.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.gcName.Name = "gcName";
		this.gcName.OptionsColumn.ReadOnly = true;
		this.gcName.PrimaryKey = false;
		this.gcName.UnitField = null;
		this.gcName.Visible = true;
		this.gcName.VisibleIndex = 1;
		this.gcName.Width = 116;
		this.gcVersion.Caption = "Version";
		this.gcVersion.CurrencyDateField = null;
		this.gcVersion.CurrencyField = null;
		this.gcVersion.CurrencyFixed = null;
		this.gcVersion.CurrencyFixedRate = 0.0;
		this.gcVersion.CurrencyRateField = null;
		this.gcVersion.FieldName = "Version";
		this.gcVersion.Name = "gcVersion";
		this.gcVersion.OptionsColumn.ReadOnly = true;
		this.gcVersion.PrimaryKey = false;
		this.gcVersion.UnitField = null;
		this.gcVersion.Visible = true;
		this.gcVersion.VisibleIndex = 2;
		this.gcStatus.Caption = "Status";
		this.gcStatus.ColumnEdit = this.rStatus;
		this.gcStatus.CurrencyDateField = null;
		this.gcStatus.CurrencyField = null;
		this.gcStatus.CurrencyFixed = null;
		this.gcStatus.CurrencyFixedRate = 0.0;
		this.gcStatus.CurrencyRateField = null;
		this.gcStatus.FieldName = "Status";
		this.gcStatus.Name = "gcStatus";
		this.gcStatus.PrimaryKey = false;
		this.gcStatus.UnitField = null;
		this.gcStatus.Visible = true;
		this.gcStatus.VisibleIndex = 3;
		this.rStatus.AutoHeight = false;
		this.rStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rStatus.DisplayMember = "Name";
		this.rStatus.Name = "rStatus";
		this.rStatus.NullText = "";
		this.rStatus.ShowFooter = false;
		this.rStatus.ShowHeader = false;
		this.rStatus.ValueMember = "Code";
		this.gcActForm.Caption = "Active from";
		this.gcActForm.ColumnEdit = this.rDate;
		this.gcActForm.CurrencyDateField = null;
		this.gcActForm.CurrencyField = null;
		this.gcActForm.CurrencyFixed = null;
		this.gcActForm.CurrencyFixedRate = 0.0;
		this.gcActForm.CurrencyRateField = null;
		this.gcActForm.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcActForm.FieldName = "ActFrom";
		this.gcActForm.Name = "gcActForm";
		this.gcActForm.PrimaryKey = false;
		this.gcActForm.UnitField = null;
		this.gcActForm.Visible = true;
		this.gcActForm.VisibleIndex = 4;
		this.rDate.AutoHeight = false;
		this.rDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.Name = "rDate";
		this.gcActiveTo.Caption = "Active To";
		this.gcActiveTo.ColumnEdit = this.rDate;
		this.gcActiveTo.CurrencyDateField = null;
		this.gcActiveTo.CurrencyField = null;
		this.gcActiveTo.CurrencyFixed = null;
		this.gcActiveTo.CurrencyFixedRate = 0.0;
		this.gcActiveTo.CurrencyRateField = null;
		this.gcActiveTo.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gcActiveTo.FieldName = "ActTo";
		this.gcActiveTo.Name = "gcActiveTo";
		this.gcActiveTo.PrimaryKey = false;
		this.gcActiveTo.UnitField = null;
		this.gcActiveTo.Visible = true;
		this.gcActiveTo.VisibleIndex = 5;
		this.gcType.Caption = "Type";
		this.gcType.ColumnEdit = this.rType;
		this.gcType.CurrencyDateField = null;
		this.gcType.CurrencyField = null;
		this.gcType.CurrencyFixed = null;
		this.gcType.CurrencyFixedRate = 0.0;
		this.gcType.CurrencyRateField = null;
		this.gcType.FieldName = "Type";
		this.gcType.Name = "gcType";
		this.gcType.PrimaryKey = false;
		this.gcType.UnitField = null;
		this.gcType.Visible = true;
		this.gcType.VisibleIndex = 6;
		this.rType.AutoHeight = false;
		this.rType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rType.DisplayMember = "Name";
		this.rType.Name = "rType";
		this.rType.NullText = "";
		this.rType.ShowFooter = false;
		this.rType.ShowHeader = false;
		this.rType.ValueMember = "Code";
		this.gcLine.Caption = "Line";
		this.gcLine.ColumnEdit = this.rLine;
		this.gcLine.CurrencyDateField = null;
		this.gcLine.CurrencyField = null;
		this.gcLine.CurrencyFixed = null;
		this.gcLine.CurrencyFixedRate = 0.0;
		this.gcLine.CurrencyRateField = null;
		this.gcLine.FieldName = "Line";
		this.gcLine.Name = "gcLine";
		this.gcLine.PrimaryKey = false;
		this.gcLine.UnitField = null;
		this.gcLine.Visible = true;
		this.gcLine.VisibleIndex = 7;
		this.rLine.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rLine.Name = "rLine";
		this.rLine.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rLine_ButtonClick);
		this.gcQuantity.Caption = "Quantity";
		this.gcQuantity.ColumnEdit = this.rDocCal;
		this.gcQuantity.CurrencyDateField = null;
		this.gcQuantity.CurrencyField = null;
		this.gcQuantity.CurrencyFixed = null;
		this.gcQuantity.CurrencyFixedRate = 0.0;
		this.gcQuantity.CurrencyRateField = null;
		this.gcQuantity.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcQuantity.FieldName = "Qauntity";
		this.gcQuantity.Name = "gcQuantity";
		this.gcQuantity.PrimaryKey = false;
		this.gcQuantity.UnitField = null;
		this.gcQuantity.Visible = true;
		this.gcQuantity.VisibleIndex = 8;
		this.rDocCal.AutoHeight = false;
		this.rDocCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDocCal.Name = "rDocCal";
		this.gcWhs.Caption = "Whs Code";
		this.gcWhs.ColumnEdit = this.rWhs;
		this.gcWhs.CurrencyDateField = null;
		this.gcWhs.CurrencyField = null;
		this.gcWhs.CurrencyFixed = null;
		this.gcWhs.CurrencyFixedRate = 0.0;
		this.gcWhs.CurrencyRateField = null;
		this.gcWhs.FieldName = "ToWH";
		this.gcWhs.Name = "gcWhs";
		this.gcWhs.PrimaryKey = false;
		this.gcWhs.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcWhs.UnitField = null;
		this.gcWhs.Visible = true;
		this.gcWhs.VisibleIndex = 9;
		this.rWhs.AutoHeight = false;
		editorButtonImageOptions3.Image = Apzon.Forms.Properties.Resources.right;
		this.rWhs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rWhs.Name = "rWhs";
		this.rWhs.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rWhs_ButtonClick);
		this.gcDisRule.Caption = "Dis.Rule";
		this.gcDisRule.ColumnEdit = this.rDistriRule;
		this.gcDisRule.CurrencyDateField = null;
		this.gcDisRule.CurrencyField = null;
		this.gcDisRule.CurrencyFixed = null;
		this.gcDisRule.CurrencyFixedRate = 0.0;
		this.gcDisRule.CurrencyRateField = null;
		this.gcDisRule.FieldName = "DisRule";
		this.gcDisRule.Name = "gcDisRule";
		this.gcDisRule.PrimaryKey = false;
		this.gcDisRule.UnitField = null;
		this.gcDisRule.Visible = true;
		this.gcDisRule.VisibleIndex = 13;
		this.rDistriRule.AutoHeight = false;
		this.rDistriRule.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rDistriRule.Name = "rDistriRule";
		this.rDistriRule.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rDistriRule_ButtonClick);
		this.gcProject.Caption = "Project";
		this.gcProject.ColumnEdit = this.rProject;
		this.gcProject.CurrencyDateField = null;
		this.gcProject.CurrencyField = null;
		this.gcProject.CurrencyFixed = null;
		this.gcProject.CurrencyFixedRate = 0.0;
		this.gcProject.CurrencyRateField = null;
		this.gcProject.FieldName = "Project";
		this.gcProject.Name = "gcProject";
		this.gcProject.PrimaryKey = false;
		this.gcProject.UnitField = null;
		this.gcProject.Visible = true;
		this.gcProject.VisibleIndex = 10;
		this.rProject.AutoHeight = false;
		this.rProject.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rProject.Name = "rProject";
		this.rProject.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rProject_ButtonClick);
		this.gcPriceList.Caption = "Price List";
		this.gcPriceList.ColumnEdit = this.rPriceList;
		this.gcPriceList.CurrencyDateField = null;
		this.gcPriceList.CurrencyField = null;
		this.gcPriceList.CurrencyFixed = null;
		this.gcPriceList.CurrencyFixedRate = 0.0;
		this.gcPriceList.CurrencyRateField = null;
		this.gcPriceList.FieldName = "PriceList";
		this.gcPriceList.Name = "gcPriceList";
		this.gcPriceList.PrimaryKey = false;
		this.gcPriceList.UnitField = null;
		this.gcPriceList.Visible = true;
		this.gcPriceList.VisibleIndex = 11;
		this.rPriceList.AutoHeight = false;
		this.rPriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rPriceList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name3"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name4", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rPriceList.DisplayMember = "Name";
		this.rPriceList.Name = "rPriceList";
		this.rPriceList.NullText = "";
		this.rPriceList.ShowFooter = false;
		this.rPriceList.ShowHeader = false;
		this.rPriceList.ValueMember = "Code";
		this.gcRemark.Caption = "Remark";
		this.gcRemark.CurrencyDateField = null;
		this.gcRemark.CurrencyField = null;
		this.gcRemark.CurrencyFixed = null;
		this.gcRemark.CurrencyFixedRate = 0.0;
		this.gcRemark.CurrencyRateField = null;
		this.gcRemark.FieldName = "Remark";
		this.gcRemark.Name = "gcRemark";
		this.gcRemark.PrimaryKey = false;
		this.gcRemark.UnitField = null;
		this.gcRemark.Visible = true;
		this.gcRemark.VisibleIndex = 12;
		this.gcBomType.Caption = "Tree Type";
		this.gcBomType.ColumnEdit = this.rTreeType;
		this.gcBomType.CurrencyDateField = null;
		this.gcBomType.CurrencyField = null;
		this.gcBomType.CurrencyFixed = null;
		this.gcBomType.CurrencyFixedRate = 0.0;
		this.gcBomType.CurrencyRateField = null;
		this.gcBomType.FieldName = "TreeType";
		this.gcBomType.Name = "gcBomType";
		this.gcBomType.PrimaryKey = false;
		this.gcBomType.UnitField = null;
		this.gcBomType.Visible = true;
		this.gcBomType.VisibleIndex = 14;
		this.rTreeType.AutoHeight = false;
		this.rTreeType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rTreeType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name1"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name2", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rTreeType.DisplayMember = "Name";
		this.rTreeType.Name = "rTreeType";
		this.rTreeType.NullText = "";
		this.rTreeType.ShowFooter = false;
		this.rTreeType.ShowHeader = false;
		this.rTreeType.ValueMember = "Code";
		this.gridSplitContainer2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridSplitContainer2.Grid = this.grdLine;
		this.gridSplitContainer2.Location = new System.Drawing.Point(2, 1);
		this.gridSplitContainer2.Name = "gridSplitContainer2";
		this.gridSplitContainer2.Panel1.Controls.Add(this.grdLine);
		this.gridSplitContainer2.Size = new System.Drawing.Size(791, 220);
		this.gridSplitContainer2.TabIndex = 0;
		this.grdLine.AllowPasteExcel = true;
		this.grdLine.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdLine.ClipboardData = null;
		this.grdLine.ContextMenuStrip = this.contextMenuStripLine;
		this.grdLine.Location = new System.Drawing.Point(2, 1);
		this.grdLine.MainView = this.gvLine;
		this.grdLine.Name = "grdLine";
		this.grdLine.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rLineType, this.rLineWhs, this.rLineIsueMth, this.rLineDisRule, this.rLinePriceList, this.rLineCal, this.rLinePrice, this.rCodeLine });
		this.grdLine.Size = new System.Drawing.Size(791, 220);
		this.grdLine.TabIndex = 0;
		this.grdLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvLine });
		this.grdLine.Paint += new System.Windows.Forms.PaintEventHandler(this.grdLine_Paint);
		this.grdLine.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grdLine_EditorKeyDown);
		this.contextMenuStripLine.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStripLine.Name = "contextMenuStripLine";
		this.contextMenuStripLine.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvLine.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvLine.ColumnKey = null;
		this.gvLine.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[13]
		{
			this.gcLineType, this.gcItemCodeLine, this.gcNameLine, this.gcLineQuantity, this.gcLineAddQuantit, this.gcUoMNameLine, this.gcWarehouseLine, this.gcIssueMthLine, this.gcLineDisRule, this.gcLinePriceList,
			this.gcLinePrice, this.gcLineComment, this.gcWipCodeLine
		});
		this.gvLine.DisableFormSetting = false;
		this.gvLine.DisplayException = false;
		this.gvLine.EditColumn = null;
		this.gvLine.FieldCurrency = null;
		this.gvLine.FieldRate = null;
		this.gvLine.FormId = "oBomVersion";
		this.gvLine.GridControl = this.grdLine;
		this.gvLine.IgnoreFilter = false;
		this.gvLine.IndexEditRow = -1;
		this.gvLine.IndicatorWidth = 30;
		this.gvLine.IsApzTable = false;
		this.gvLine.IsInsertNewRow = false;
		this.gvLine.IsLoadingData = false;
		this.gvLine.IsMappingContextAction = false;
		this.gvLine.ItemId = "APZ_ITT1";
		this.gvLine.LstColumnAllowBatch = null;
		this.gvLine.LstColumnNotAllowBatch = null;
		this.gvLine.LstColumnNotEditable = null;
		this.gvLine.LstColumNotSetting = null;
		this.gvLine.Name = "gvLine";
		this.gvLine.OptionsSelection.MultiSelect = true;
		this.gvLine.OptionsView.ColumnAutoWidth = false;
		this.gvLine.OptionsView.ShowGroupPanel = false;
		this.gvLine.UseBatchMod = false;
		this.gvLine.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvLine_CustomDrawRowIndicator);
		this.gvLine.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvLine_ShowingEditor);
		this.gvLine.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvLine_CellValueChanged);
		this.gvLine.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvLine_CellValueChanging);
		this.gcLineType.Caption = "Item Type";
		this.gcLineType.ColumnEdit = this.rLineType;
		this.gcLineType.CurrencyDateField = null;
		this.gcLineType.CurrencyField = null;
		this.gcLineType.CurrencyFixed = null;
		this.gcLineType.CurrencyFixedRate = 0.0;
		this.gcLineType.CurrencyRateField = null;
		this.gcLineType.FieldName = "Type";
		this.gcLineType.Name = "gcLineType";
		this.gcLineType.PrimaryKey = true;
		this.gcLineType.UnitField = null;
		this.gcLineType.Visible = true;
		this.gcLineType.VisibleIndex = 0;
		this.rLineType.AutoHeight = false;
		this.rLineType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineType.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name10"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("ID", "Name11", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rLineType.DisplayMember = "Name";
		this.rLineType.Name = "rLineType";
		this.rLineType.NullText = "";
		this.rLineType.ShowFooter = false;
		this.rLineType.ShowHeader = false;
		this.rLineType.ValueMember = "ID";
		this.rLineType.EditValueChanged += new System.EventHandler(this.rLineType_EditValueChanged);
		this.gcItemCodeLine.Caption = "No.";
		this.gcItemCodeLine.CflType = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.gcItemCodeLine.ColumnEdit = this.rCodeLine;
		this.gcItemCodeLine.CurrencyDateField = null;
		this.gcItemCodeLine.CurrencyField = null;
		this.gcItemCodeLine.CurrencyFixed = null;
		this.gcItemCodeLine.CurrencyFixedRate = 0.0;
		this.gcItemCodeLine.CurrencyRateField = null;
		this.gcItemCodeLine.FieldName = "Code";
		this.gcItemCodeLine.Name = "gcItemCodeLine";
		this.gcItemCodeLine.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gcItemCodeLine.PrimaryKey = false;
		this.gcItemCodeLine.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcItemCodeLine.UnitField = null;
		this.gcItemCodeLine.Visible = true;
		this.gcItemCodeLine.VisibleIndex = 1;
		this.rCodeLine.AutoHeight = false;
		editorButtonImageOptions4.Location = DevExpress.XtraEditors.ImageLocation.MiddleRight;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.rCodeLine.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rCodeLine.Name = "rCodeLine";
		this.rCodeLine.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rCodeLine_ButtonClick);
		this.gcNameLine.Caption = "Description";
		this.gcNameLine.CurrencyDateField = null;
		this.gcNameLine.CurrencyField = null;
		this.gcNameLine.CurrencyFixed = null;
		this.gcNameLine.CurrencyFixedRate = 0.0;
		this.gcNameLine.CurrencyRateField = null;
		this.gcNameLine.FieldName = "ItemName";
		this.gcNameLine.Name = "gcNameLine";
		this.gcNameLine.OptionsColumn.ReadOnly = true;
		this.gcNameLine.PrimaryKey = false;
		this.gcNameLine.UnitField = null;
		this.gcNameLine.Visible = true;
		this.gcNameLine.VisibleIndex = 2;
		this.gcNameLine.Width = 108;
		this.gcLineQuantity.Caption = "Quantity";
		this.gcLineQuantity.ColumnEdit = this.rLineCal;
		this.gcLineQuantity.CurrencyDateField = null;
		this.gcLineQuantity.CurrencyField = null;
		this.gcLineQuantity.CurrencyFixed = null;
		this.gcLineQuantity.CurrencyFixedRate = 0.0;
		this.gcLineQuantity.CurrencyRateField = null;
		this.gcLineQuantity.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineQuantity.FieldName = "Quantity";
		this.gcLineQuantity.Name = "gcLineQuantity";
		this.gcLineQuantity.PrimaryKey = false;
		this.gcLineQuantity.UnitField = null;
		this.gcLineQuantity.Visible = true;
		this.gcLineQuantity.VisibleIndex = 3;
		this.rLineCal.AutoHeight = false;
		this.rLineCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineCal.Name = "rLineCal";
		this.gcLineAddQuantit.Caption = "AddQuantit";
		this.gcLineAddQuantit.ColumnEdit = this.rLineCal;
		this.gcLineAddQuantit.CurrencyDateField = null;
		this.gcLineAddQuantit.CurrencyField = null;
		this.gcLineAddQuantit.CurrencyFixed = null;
		this.gcLineAddQuantit.CurrencyFixedRate = 0.0;
		this.gcLineAddQuantit.CurrencyRateField = null;
		this.gcLineAddQuantit.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcLineAddQuantit.FieldName = "AddQuantit";
		this.gcLineAddQuantit.Name = "gcLineAddQuantit";
		this.gcLineAddQuantit.PrimaryKey = false;
		this.gcLineAddQuantit.UnitField = null;
		this.gcLineAddQuantit.Visible = true;
		this.gcLineAddQuantit.VisibleIndex = 4;
		this.gcUoMNameLine.Caption = "UoM Name";
		this.gcUoMNameLine.CurrencyDateField = null;
		this.gcUoMNameLine.CurrencyField = null;
		this.gcUoMNameLine.CurrencyFixed = null;
		this.gcUoMNameLine.CurrencyFixedRate = 0.0;
		this.gcUoMNameLine.CurrencyRateField = null;
		this.gcUoMNameLine.FieldName = "Uom";
		this.gcUoMNameLine.Name = "gcUoMNameLine";
		this.gcUoMNameLine.OptionsColumn.ReadOnly = true;
		this.gcUoMNameLine.PrimaryKey = false;
		this.gcUoMNameLine.UnitField = null;
		this.gcUoMNameLine.Visible = true;
		this.gcUoMNameLine.VisibleIndex = 5;
		this.gcWarehouseLine.Caption = "Warehouse";
		this.gcWarehouseLine.ColumnEdit = this.rLineWhs;
		this.gcWarehouseLine.CurrencyDateField = null;
		this.gcWarehouseLine.CurrencyField = null;
		this.gcWarehouseLine.CurrencyFixed = null;
		this.gcWarehouseLine.CurrencyFixedRate = 0.0;
		this.gcWarehouseLine.CurrencyRateField = null;
		this.gcWarehouseLine.FieldName = "Warehouse";
		this.gcWarehouseLine.Name = "gcWarehouseLine";
		this.gcWarehouseLine.PrimaryKey = false;
		this.gcWarehouseLine.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcWarehouseLine.UnitField = null;
		this.gcWarehouseLine.Visible = true;
		this.gcWarehouseLine.VisibleIndex = 6;
		this.rLineWhs.AutoHeight = false;
		editorButtonImageOptions6.Image = Apzon.Forms.Properties.Resources.right;
		this.rLineWhs.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rLineWhs.Name = "rLineWhs";
		this.rLineWhs.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rLineWhs_ButtonClick);
		this.gcIssueMthLine.Caption = "Issue Method";
		this.gcIssueMthLine.ColumnEdit = this.rLineIsueMth;
		this.gcIssueMthLine.CurrencyDateField = null;
		this.gcIssueMthLine.CurrencyField = null;
		this.gcIssueMthLine.CurrencyFixed = null;
		this.gcIssueMthLine.CurrencyFixedRate = 0.0;
		this.gcIssueMthLine.CurrencyRateField = null;
		this.gcIssueMthLine.FieldName = "IssueMthd";
		this.gcIssueMthLine.Name = "gcIssueMthLine";
		this.gcIssueMthLine.PrimaryKey = false;
		this.gcIssueMthLine.UnitField = null;
		this.gcIssueMthLine.Visible = true;
		this.gcIssueMthLine.VisibleIndex = 7;
		this.rLineIsueMth.AutoHeight = false;
		this.rLineIsueMth.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineIsueMth.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name12"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name13", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rLineIsueMth.DisplayMember = "Name";
		this.rLineIsueMth.Name = "rLineIsueMth";
		this.rLineIsueMth.NullText = "";
		this.rLineIsueMth.ShowFooter = false;
		this.rLineIsueMth.ShowHeader = false;
		this.rLineIsueMth.ValueMember = "Code";
		this.gcLineDisRule.Caption = "Dist.Rule";
		this.gcLineDisRule.ColumnEdit = this.rLineDisRule;
		this.gcLineDisRule.CurrencyDateField = null;
		this.gcLineDisRule.CurrencyField = null;
		this.gcLineDisRule.CurrencyFixed = null;
		this.gcLineDisRule.CurrencyFixedRate = 0.0;
		this.gcLineDisRule.CurrencyRateField = null;
		this.gcLineDisRule.FieldName = "DisRule";
		this.gcLineDisRule.Name = "gcLineDisRule";
		this.gcLineDisRule.PrimaryKey = false;
		this.gcLineDisRule.UnitField = null;
		this.gcLineDisRule.Visible = true;
		this.gcLineDisRule.VisibleIndex = 12;
		this.rLineDisRule.AutoHeight = false;
		this.rLineDisRule.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rLineDisRule.Name = "rLineDisRule";
		this.rLineDisRule.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rLineDisRule_ButtonClick);
		this.gcLinePriceList.Caption = "Price List";
		this.gcLinePriceList.ColumnEdit = this.rLinePriceList;
		this.gcLinePriceList.CurrencyDateField = null;
		this.gcLinePriceList.CurrencyField = null;
		this.gcLinePriceList.CurrencyFixed = null;
		this.gcLinePriceList.CurrencyFixedRate = 0.0;
		this.gcLinePriceList.CurrencyRateField = null;
		this.gcLinePriceList.FieldName = "PriceList";
		this.gcLinePriceList.Name = "gcLinePriceList";
		this.gcLinePriceList.PrimaryKey = false;
		this.gcLinePriceList.UnitField = null;
		this.gcLinePriceList.Visible = true;
		this.gcLinePriceList.VisibleIndex = 8;
		this.rLinePriceList.AutoHeight = false;
		this.rLinePriceList.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLinePriceList.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name14"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name15", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default)
		});
		this.rLinePriceList.DisplayMember = "Name";
		this.rLinePriceList.Name = "rLinePriceList";
		this.rLinePriceList.NullText = "";
		this.rLinePriceList.ShowFooter = false;
		this.rLinePriceList.ShowHeader = false;
		this.rLinePriceList.ValueMember = "Code";
		this.gcLinePrice.Caption = "Price";
		this.gcLinePrice.ColumnEdit = this.rLinePrice;
		this.gcLinePrice.CurrencyDateField = null;
		this.gcLinePrice.CurrencyField = null;
		this.gcLinePrice.CurrencyFixed = null;
		this.gcLinePrice.CurrencyFixedRate = 0.0;
		this.gcLinePrice.CurrencyRateField = null;
		this.gcLinePrice.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Price;
		this.gcLinePrice.FieldName = "Price";
		this.gcLinePrice.Name = "gcLinePrice";
		this.gcLinePrice.PrimaryKey = false;
		this.gcLinePrice.UnitField = null;
		this.gcLinePrice.Visible = true;
		this.gcLinePrice.VisibleIndex = 9;
		this.rLinePrice.AutoHeight = false;
		this.rLinePrice.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLinePrice.Name = "rLinePrice";
		this.gcLineComment.Caption = "Comment";
		this.gcLineComment.CurrencyDateField = null;
		this.gcLineComment.CurrencyField = null;
		this.gcLineComment.CurrencyFixed = null;
		this.gcLineComment.CurrencyFixedRate = 0.0;
		this.gcLineComment.CurrencyRateField = null;
		this.gcLineComment.FieldName = "Comment";
		this.gcLineComment.Name = "gcLineComment";
		this.gcLineComment.PrimaryKey = false;
		this.gcLineComment.UnitField = null;
		this.gcLineComment.Visible = true;
		this.gcLineComment.VisibleIndex = 10;
		this.gcWipCodeLine.Caption = "WipActCode";
		this.gcWipCodeLine.CurrencyDateField = null;
		this.gcWipCodeLine.CurrencyField = null;
		this.gcWipCodeLine.CurrencyFixed = null;
		this.gcWipCodeLine.CurrencyFixedRate = 0.0;
		this.gcWipCodeLine.CurrencyRateField = null;
		this.gcWipCodeLine.FieldName = "WipActCode";
		this.gcWipCodeLine.Name = "gcWipCodeLine";
		this.gcWipCodeLine.PrimaryKey = false;
		this.gcWipCodeLine.UnitField = null;
		this.gcWipCodeLine.Visible = true;
		this.gcWipCodeLine.VisibleIndex = 11;
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnSave.EditControl = null;
		this.btnSave.EditValue = "OK";
		this.btnSave.FieldName = null;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.IsSearchControl = false;
		this.btnSave.Location = new System.Drawing.Point(5, 452);
		this.btnSave.Name = "btnSave";
		this.btnSave.ReadOnly = false;
		this.btnSave.SettingInVisible = false;
		this.btnSave.SettingReadonly = false;
		this.btnSave.Size = new System.Drawing.Size(75, 23);
		this.btnSave.TabIndex = 4;
		this.btnSave.Text = "OK";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(83, 452);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 2;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.lbCode.EditControl = null;
		this.lbCode.EditValue = "Item Code";
		this.lbCode.FieldName = null;
		this.lbCode.HideWhenEmpty = false;
		this.lbCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbCode.Location = new System.Drawing.Point(8, 8);
		this.lbCode.Name = "lbCode";
		this.lbCode.ReadOnly = false;
		this.lbCode.SettingInVisible = false;
		this.lbCode.SettingReadonly = false;
		this.lbCode.ShowUnderLine = true;
		this.lbCode.Size = new System.Drawing.Size(70, 20);
		this.lbCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbCode.TabIndex = 0;
		this.lbCode.Utext = "Item Code";
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = false;
		this.btnSearch.Location = new System.Drawing.Point(467, 6);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(70, 20);
		this.btnSearch.TabIndex = 3;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.txtVersionCode.EditControl = null;
		this.txtVersionCode.FieldName = null;
		this.txtVersionCode.IsSearchControl = false;
		this.txtVersionCode.Location = new System.Drawing.Point(309, 6);
		this.txtVersionCode.Name = "txtVersionCode";
		this.txtVersionCode.SettingInVisible = false;
		this.txtVersionCode.SettingReadonly = false;
		this.txtVersionCode.Size = new System.Drawing.Size(150, 20);
		this.txtVersionCode.TabIndex = 2;
		this.lbVersion.EditControl = null;
		this.lbVersion.EditValue = "Version Code";
		this.lbVersion.FieldName = null;
		this.lbVersion.HideWhenEmpty = false;
		this.lbVersion.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbVersion.Location = new System.Drawing.Point(240, 8);
		this.lbVersion.Name = "lbVersion";
		this.lbVersion.ReadOnly = false;
		this.lbVersion.SettingInVisible = false;
		this.lbVersion.SettingReadonly = false;
		this.lbVersion.ShowUnderLine = true;
		this.lbVersion.Size = new System.Drawing.Size(70, 20);
		this.lbVersion.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbVersion.TabIndex = 0;
		this.lbVersion.Utext = "Version Code";
		this.txtItemCode.EditControl = null;
		this.txtItemCode.FieldName = null;
		this.txtItemCode.IsSearchControl = false;
		this.txtItemCode.Location = new System.Drawing.Point(77, 6);
		this.txtItemCode.Name = "txtItemCode";
		this.txtItemCode.Properties.AppearanceReadOnly.BackColor = System.Drawing.Color.White;
		this.txtItemCode.Properties.AppearanceReadOnly.Options.UseBackColor = true;
		editorButtonImageOptions7.Image = Apzon.Forms.Properties.Resources.right;
		this.txtItemCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtItemCode.SettingInVisible = false;
		this.txtItemCode.SettingReadonly = false;
		this.txtItemCode.Size = new System.Drawing.Size(150, 20);
		this.txtItemCode.TabIndex = 1;
		this.txtItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.txtItemCode_ButtonClick);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(800, 480);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.txtVersionCode);
		base.Controls.Add(this.lbVersion);
		base.Controls.Add(this.lbCode);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.splitContainerControl);
		base.Controls.Add(this.txtItemCode);
		base.FormId = "oBomVersion";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AdvanceBillOfMaterials";
		this.Text = "Advance Bill Of Materials";
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl).EndInit();
		this.splitContainerControl.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer1).EndInit();
		this.gridSplitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStripDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDocCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rWhs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDistriRule).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rProject).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rPriceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rTreeType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridSplitContainer2).EndInit();
		this.gridSplitContainer2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdLine).EndInit();
		this.contextMenuStripLine.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCodeLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineWhs).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineIsueMth).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineDisRule).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLinePriceList).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLinePrice).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtVersionCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtItemCode.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
