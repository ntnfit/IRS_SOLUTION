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
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Inventory.InventoryTransaction;

public class frm_BarcodeInventoryCounting : XtraFormEx
{
	private DataTable _structureDoc;

	private DataTable _structureLine;

	private bool _isLoadData;

	private IContainer components;

	private LabelLine lbCountDate;

	private LabelLine lbTime;

	private LabelLine lbInventoryCounter;

	private LabelLine lbWarehouseBin;

	private LabelLine lbBarcode;

	private LabelLine lbDocNo;

	private LabelLine lbStatus;

	private LabelLine lbRemark;

	private DateEditEx txtCountDate;

	private TimeEditEx txtTime;

	private LookUpEditEx txtTakerType;

	private TextEditEx txtBarcode;

	private CalcEditEx txtDocEntry;

	private LookUpEditEx txtStatus;

	private TextEditEx txtRemarks;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnAdd;

	private CheckEditEx txtSameBatch;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcItemName;

	private GridColumnEx gcDistNumber;

	private GridColumnEx gcMnfSerial;

	private GridColumnEx gcLotNumber;

	private GridColumnEx gcInWhsQty;

	private GridColumnEx gcCountQty;

	private GridColumnEx gcUomCode;

	private GridColumnEx gcItmPerUnit;

	private GridColumnEx gcInvQty;

	private GridColumnEx gcRemark;

	private GridColumnEx gcBarCode;

	private GridColumnEx gcInvUom;

	private GridColumnEx gcDifference;

	private RepositoryItemButtonEditCFL rItemCode;

	private RepositoryItemButtonEditCFL rItemName;

	private RepositoryItemButtonEdit rDistNumber;

	private RepositoryItemCalcEdit rCal;

	private RepositoryItemLookUpEdit rInventoryUom;

	private RepositoryItemButtonEdit rUomCode;

	private ContextMenuStrip contextMenuStripLine;

	private ToolStripMenuItem deletesToolStripMenuItem;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ButtonEditCFL txtWhsCode;

	private ButtonEditCFL txtTakerId;

	private GridColumnEx gcUomName;

	private GridColumnEx gcUomGroup;

	private RepositoryItemLookUpEdit rUomGroup;

	private ButtonEditCFL txtBinLocation;

	private GridColumnEx gcStatus;

	private RepositoryItemLookUpEdit rStatus;

	private ContextMenuStrip contextMenuStripDoc;

	private ToolStripMenuItem addNewToolStripMenuItem1;

	private ToolStripMenuItem deleteToolStripMenuItem;

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
			switch (value)
			{
			case Enums.FormViewModeType.Add:
				txtCountDate.ReadOnly = false;
				txtTime.ReadOnly = false;
				txtTakerType.ReadOnly = false;
				txtTakerId.ReadOnly = false;
				txtTakerId.Properties.Buttons[0].Visible = true;
				txtWhsCode.ReadOnly = false;
				txtWhsCode.Properties.Buttons[0].Visible = true;
				txtBinLocation.ReadOnly = false;
				txtDocEntry.ReadOnly = true;
				txtStatus.ReadOnly = false;
				txtRemarks.ReadOnly = false;
				gvDoc.ReadOnly = false;
				btnAdd.Text = "Add";
				btnAdd.Enabled = true;
				btnCancel.Enabled = true;
				break;
			case Enums.FormViewModeType.Find:
				txtCountDate.ReadOnly = false;
				txtTime.ReadOnly = false;
				txtTakerType.ReadOnly = false;
				txtTakerId.ReadOnly = false;
				txtTakerId.Properties.Buttons[0].Visible = true;
				txtWhsCode.ReadOnly = false;
				txtWhsCode.Properties.Buttons[0].Visible = true;
				txtBinLocation.ReadOnly = false;
				txtDocEntry.ReadOnly = false;
				txtStatus.ReadOnly = false;
				txtRemarks.ReadOnly = false;
				gvDoc.ReadOnly = true;
				btnAdd.Text = "Find";
				btnAdd.Enabled = true;
				btnCancel.Enabled = true;
				break;
			case Enums.FormViewModeType.View:
				txtCountDate.ReadOnly = false;
				txtTime.ReadOnly = false;
				txtTakerType.ReadOnly = false;
				txtTakerId.ReadOnly = false;
				txtTakerId.Properties.Buttons[0].Visible = true;
				txtWhsCode.ReadOnly = false;
				txtWhsCode.Properties.Buttons[0].Visible = true;
				txtBinLocation.ReadOnly = false;
				txtDocEntry.ReadOnly = true;
				txtStatus.ReadOnly = false;
				txtRemarks.ReadOnly = false;
				gvDoc.ReadOnly = false;
				btnAdd.Text = "Ok";
				btnAdd.Enabled = true;
				btnCancel.Enabled = true;
				break;
			case Enums.FormViewModeType.Update:
				txtCountDate.ReadOnly = false;
				txtTime.ReadOnly = false;
				txtTakerType.ReadOnly = false;
				txtTakerId.ReadOnly = false;
				txtTakerId.Properties.Buttons[0].Visible = true;
				txtWhsCode.ReadOnly = false;
				txtWhsCode.Properties.Buttons[0].Visible = true;
				txtBinLocation.ReadOnly = false;
				txtDocEntry.ReadOnly = true;
				txtStatus.ReadOnly = false;
				txtRemarks.ReadOnly = false;
				gvDoc.ReadOnly = false;
				btnAdd.Text = "Update";
				btnAdd.Enabled = true;
				btnCancel.Enabled = true;
				break;
			case Enums.FormViewModeType.Busy:
				txtCountDate.ReadOnly = true;
				txtTime.ReadOnly = true;
				txtTakerType.ReadOnly = true;
				txtTakerId.ReadOnly = true;
				txtTakerId.Properties.Buttons[0].Visible = false;
				txtWhsCode.ReadOnly = true;
				txtWhsCode.Properties.Buttons[0].Visible = false;
				txtBinLocation.ReadOnly = true;
				txtDocEntry.ReadOnly = true;
				txtStatus.ReadOnly = true;
				txtRemarks.ReadOnly = true;
				gvDoc.ReadOnly = true;
				btnAdd.Enabled = false;
				btnCancel.Enabled = false;
				break;
			}
			SetColorControll(value);
			_formMode = value;
			if (value != Enums.FormViewModeType.Busy)
			{
				txtStatus_EditValueChanged(null, null);
			}
		}
		catch
		{
		}
	}

	public frm_BarcodeInventoryCounting()
	{
		InitializeComponent();
	}

	private async Task frm_BarcodeInventoryCounting_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(int));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow[0] = 12;
			dataRow[1] = "User";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow[0] = 171;
			dataRow[1] = "Employee";
			dataTable.Rows.Add(dataRow);
			txtTakerType.Properties.DataSource = dataTable;
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Draft",
					Code = "D"
				},
				new Common.Table_Code
				{
					Name = "Approval",
					Code = "A"
				},
				new Common.Table_Code
				{
					Name = "Close",
					Code = "C"
				}
			};
			txtStatus.Properties.DataSource = dataSource;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rItemName;
			repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					gvDoc.SetFocusedRowCellValue(gcItemCode, chooseItem["ItemCode"]);
					grdDoc.DataSourceTable.AcceptChanges();
					gvDoc.RefreshData();
				}
			}));
			RepositoryItemButtonEditCFL properties = txtBinLocation.Properties;
			properties.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(properties.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL cfl) =>
			{
				cfl.WhsCode = Function.ToString(txtWhsCode.EditValue);
			}));
			List<Common.Table_Code> dataSource2 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Ok",
					Code = "O"
				},
				new Common.Table_Code
				{
					Name = "Not Exists",
					Code = "N"
				},
				new Common.Table_Code
				{
					Name = "Surplus Goods",
					Code = "S"
				}
			};
			rStatus.DataSource = dataSource2;
			DataSet dataSet = await HttpClient.BarcodeInventoryCountingClient.GetDataStructure();
			if (dataSet == null || dataSet.Tables.Count != 2 || dataSet.Tables[0] == null || dataSet.Tables[0].Columns.Count == 0 || dataSet.Tables[1] == null || dataSet.Tables[1].Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotLoadStructure, "");
			}
			else
			{
				_structureDoc = dataSet.Tables[0].Clone();
				_structureLine = dataSet.Tables[1].Clone();
			}
			await LoadUomGroup();
			List<Common.Table_Code> dataSource3 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Yes",
					Code = "Y"
				},
				new Common.Table_Code
				{
					Name = "No",
					Code = "N"
				}
			};
			rInventoryUom.DataSource = dataSource3;
			await REFRESH();
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task gvDoc_CellValueChangedAsync(CellValueChangedEventArgs e)
	{
		try
		{
			DataRow dataRow = gvDoc.GetDataRow(e.RowHandle);
			if (e.Column.FieldName.Equals(gcItemCode.FieldName))
			{
				string text = Function.ToString(e.Value);
				DataTable dataItemCode = HttpClient.Commons.GetDataItemCode(text, -1, GlobalData.UserName, GlobalData.DefaultBranch);
				if (dataItemCode.IsNotNull())
				{
					if (string.IsNullOrEmpty(Function.ToString(dataRow["LineNum"])))
					{
						dataRow["LineNum"] = grdDoc.DataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"])) + 1;
					}
					dataRow["ItemName"] = dataItemCode.Rows[0]["ItemName"];
					dataRow["NumPerMsr"] = 1;
					dataRow["UgpCode"] = dataItemCode.Rows[0]["UgpEntry"];
					dataRow["unitMsr"] = dataItemCode.Rows[0]["InvntryUom"];
					dataRow["NumInInven"] = dataItemCode.Rows[0]["NumInInven"];
					dataRow["InvUnitMsr"] = dataItemCode.Rows[0]["InvntryUom"];
					dataRow["InvUoM"] = "Y";
					dataRow[gcCountQty.FieldName] = 0;
					dataRow[gcInvQty.FieldName] = 0;
					if (Function.ParseInt(dataItemCode.Rows[0]["UgpEntry"]) == -1)
					{
						gvDoc.SetRowCellValue(e.RowHandle, gcUomCode, gvDoc.GetRowCellDisplayText(e.RowHandle, "UgpCode"));
					}
					else
					{
						gvDoc.SetRowCellValue(e.RowHandle, gcUomCode, WinFuntions.GetUoMCodeByName(Function.ParseInt(dataItemCode.Rows[0]["UgpEntry"]), Function.ToString(dataItemCode.Rows[0]["InvntryUom"])));
					}
				}
				string text2 = Function.ToString(txtWhsCode.EditValue);
				if (string.IsNullOrEmpty(text2) || string.IsNullOrEmpty(text))
				{
					dataRow[gcInWhsQty.FieldName] = 0;
				}
				else
				{
					DataTable onstockItem = HttpClient.Commons.GetOnstockItem(text, text2);
					if (onstockItem.IsNotNull())
					{
						dataRow[gcInWhsQty.FieldName] = onstockItem.Rows[0]["InWhs"];
					}
				}
				double num = Function.ParseDouble(dataRow[gcInWhsQty.FieldName]);
				dataRow[gcDifference.FieldName] = -1.0 * num;
				dataRow["CountDiffP"] = ((Math.Abs(num) > 1E-07) ? (-100.0 * num / num) : 0.0);
			}
			else if (e.Column.FieldName == gcUomCode.FieldName)
			{
				DataTable dataUomByCode = WinFuntions.GetDataUomByCode(Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, "UgpCode")), Function.ToString(e.Value));
				if (dataUomByCode != null && dataUomByCode.Rows.Count > 0)
				{
					dataRow["UomEntry"] = dataUomByCode.Rows[0]["UomEntry"];
					dataRow[gcUomName.FieldName] = dataUomByCode.Rows[0]["UomName"];
					dataRow[gcItmPerUnit.FieldName] = Function.ParseDouble(gvDoc.GetRowCellValue(e.RowHandle, "NumInInven")) * Function.ParseDouble(dataUomByCode.Rows[0]["BaseQty"]) / ((Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]) <= 0.0) ? 1.0 : Function.ParseDouble(dataUomByCode.Rows[0]["AltQty"]));
					if (Function.ToString(dataUomByCode.Rows[0]["UomName"]).Equals(Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "InvUnitMsr"))))
					{
						dataRow[gcInvUom.FieldName] = "Y";
					}
					dataRow[gcInvQty.FieldName] = Function.ParseDouble(gvDoc.GetRowCellValue(e.RowHandle, gcCountQty)) * Function.ParseDouble(gvDoc.GetRowCellValue(e.RowHandle, gcItmPerUnit));
				}
			}
			else if (e.Column.FieldName == gcCountQty.FieldName)
			{
				gvDoc.SetRowCellValue(e.RowHandle, gcInvQty.FieldName, Function.ParseDouble(e.Value) * Function.ParseDouble(gvDoc.GetRowCellValue(e.RowHandle, gcItmPerUnit)));
			}
			else if (e.Column.FieldName == gcInvUom.FieldName)
			{
				if (Function.ToString(e.Value).Equals("Y"))
				{
					DataTable inventoryUoMByItemCode = WinFuntions.GetInventoryUoMByItemCode(Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode)), Enums.ItemUomType.InventoryUom);
					if (inventoryUoMByItemCode != null && inventoryUoMByItemCode.Rows.Count > 0)
					{
						gvDoc.SetFocusedRowCellValue(gcUomCode, inventoryUoMByItemCode.Rows[0]["UomCode"]);
						gvDoc.SetFocusedRowCellValue(gcItmPerUnit, 1);
						gvDoc.SetFocusedRowCellValue("InvUnitMsr", inventoryUoMByItemCode.Rows[0]["InvntryUom"]);
					}
				}
			}
			else if (e.Column.FieldName.Equals(gcInvQty.FieldName))
			{
				double num2 = Function.ParseDouble(dataRow[gcInWhsQty.FieldName]);
				double num3 = Function.ParseDouble(e.Value);
				dataRow[gcDifference.FieldName] = num3 - num2;
				dataRow["CountDiffP"] = ((Math.Abs(num2) > 1E-07) ? (num3 - num2 / num2) : 0.0);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		grdDoc.DataSourceTable.AcceptChanges();
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void rUomCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListUomCode, false, "", "", Enums.ItemObjectType.NoItemType, null, "")
			{
				UomGroup = Function.ParseInt(gvDoc.GetFocusedRowCellValue("UgpCode"))
			};
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				gvDoc.SetFocusedRowCellValue(gcUomCode, frm_CFL2.ChooseItem["UomCode"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnAdd_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			switch (FormMode)
			{
			case Enums.FormViewModeType.View:
				Close();
				break;
			case Enums.FormViewModeType.Find:
				await SearchDoc();
				break;
			case Enums.FormViewModeType.Update:
			case Enums.FormViewModeType.Add:
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					WinCommons._OpenProcessing();
					await SaveData(FormMode == Enums.FormViewModeType.Add);
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void deletesToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (grdDoc.DataSourceTable.IsNotNull() && gvDoc.GetFocusedDataRow() != null && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
			{
				gvDoc.DeleteDataSourceRowSelected();
				if (FormMode != Enums.FormViewModeType.Add)
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

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			DataRow row = dataSourceTable.NewRow();
			dataSourceTable.Rows.Add(row);
			dataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			string fieldName = gvDoc.FocusedColumn.FieldName;
			if (fieldName == gcInvUom.FieldName)
			{
				if (Function.ParseInt(gvDoc.GetFocusedRowCellValue(gcUomGroup)) == -1)
				{
					e.Cancel = true;
				}
			}
			else if (fieldName == gcUomCode.FieldName)
			{
				if (Function.ToString(gvDoc.GetFocusedRowCellValue(gcInvUom)).Equals("Y") || Function.ParseInt(gvDoc.GetFocusedRowCellValue(gcUomGroup)) == -1)
				{
					e.Cancel = true;
				}
			}
			else if (fieldName == gcUomName.FieldName && (!Function.ToString(gvDoc.GetFocusedRowCellValue(gcInvUom)).Equals("N") || Function.ParseInt(gvDoc.GetFocusedRowCellValue(gcUomGroup)) != -1))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txtTakerType_EditValueChanged(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (Function.ParseInt(txtTakerType.EditValue) == 12)
			{
				txtTakerId.Properties.ValueMember = "USERID";
				txtTakerId.Properties.DisplayMember = "U_NAME";
				txtTakerId.Properties.DisplayMember2 = "";
				txtTakerId.Properties.TypeCFL = Enums.TypeCFL.GetUserList;
				txtTakerId.Properties.TypeDrillDown = ApzObjectTypes.oNone;
				txtTakerId.Properties.Buttons[1].Visible = false;
				txtTakerId.Properties.MultiSelectMode = true;
				await txtTakerId.Properties.RefreshDataSourceAsync();
			}
			else if (Function.ParseInt(txtTakerType.EditValue) == 171)
			{
				txtTakerId.Properties.ValueMember = "EmpId";
				txtTakerId.Properties.DisplayMember = "lastName";
				txtTakerId.Properties.DisplayMember2 = "firstName";
				txtTakerId.Properties.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
				txtTakerId.Properties.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
				txtTakerId.Properties.MultiSelectMode = true;
				await txtTakerId.Properties.RefreshDataSourceAsync();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_BarcodeInventoryCounting_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (_isLoadData)
			{
				return;
			}
			if (e.FieldName.Equals(txtTakerType.FieldName))
			{
				txtTakerId.EditValue = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		if (FormMode == Enums.FormViewModeType.View)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void txtBarcode_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.Return)
			{
				return;
			}
			string text = Function.ToString(txtBarcode.EditValue);
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			txtBarcode.EditValue = "";
			string text2 = Function.ToString(txtWhsCode.EditValue);
			if (string.IsNullOrEmpty(text2))
			{
				WinCommons.Warning(Enums.MessageCode.ChooseWarehouse2Counting, "");
				txtWhsCode.Focus();
				return;
			}
			DataTable dtCounting = HttpClient.BarcodeInventoryCountingClient.GetDataByBarcode(text, text2, Function.ToString(txtBinLocation.EditValue));
			if (!dtCounting.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.BarcodeDoesNotExists, "");
				txtBarcode.Focus();
				return;
			}
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			if (txtSameBatch.Checked && dataSourceTable.AsEnumerable().Any((DataRow t) => Function.ToString(t[gcDistNumber.FieldName]).Equals(Function.ToString(dtCounting.Rows[0]["DistNumber"]))))
			{
				WinCommons.Warning(Enums.MessageCode.BatchNumberHaveBeenCounting, "");
				txtBarcode.Focus();
				return;
			}
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["CountDate"] = dtCounting.Rows[0]["CountDate"];
			dataRow["CountTime"] = dtCounting.Rows[0]["CountTime"];
			dataRow["ObjType"] = dtCounting.Rows[0]["ObjType"];
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
			int rowHandle = gvDoc.GetRowHandle(dataSourceTable.Rows.Count - 1);
			gvDoc.SetRowCellValue(rowHandle, gcItemCode, dtCounting.Rows[0]["ItemCode"]);
			gvDoc.SetRowCellValue(rowHandle, gcDistNumber, dtCounting.Rows[0]["DistNumber"]);
			gvDoc.SetRowCellValue(rowHandle, gcMnfSerial, dtCounting.Rows[0]["MnfSerial"]);
			gvDoc.SetRowCellValue(rowHandle, gcLotNumber, dtCounting.Rows[0]["LotNumber"]);
			gvDoc.SetRowCellValue(rowHandle, "BarCode", dtCounting.Rows[0]["BarCode"]);
			gvDoc.SetRowCellValue(rowHandle, gcStatus, dtCounting.Rows[0]["LineStatus"]);
			gvDoc.SetRowCellValue(rowHandle, gcInWhsQty, dtCounting.Rows[0]["InWhsQty"]);
			double num = Function.ParseDouble(dtCounting.Rows[0]["InWhsQty"]);
			gvDoc.SetRowCellValue(rowHandle, gcDifference, -1.0 * num);
			gvDoc.SetRowCellValue(rowHandle, "CountDiffP", (Math.Abs(num) > 1E-07) ? (-100.0 * num / num) : 0.0);
			txtBarcode.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void addNewToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		await REFRESH();
	}

	private async void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			string text = await HttpClient.BarcodeInventoryCountingClient.Delete(Function.ParseInt(txtDocEntry.EditValue));
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				await REFRESH();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void contextMenuStripDoc_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			addNewToolStripMenuItem1.Visible = FormMode != Enums.FormViewModeType.Add;
			deleteToolStripMenuItem.Visible = (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update) && Function.ToString(txtStatus.EditValue).Equals("D");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void contextMenuStripLine_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			addNewToolStripMenuItem.Visible = FormMode == Enums.FormViewModeType.Add || Function.ToString(txtStatus.EditValue).Equals("D");
			deletesToolStripMenuItem.Visible = FormMode == Enums.FormViewModeType.Add || Function.ToString(txtStatus.EditValue).Equals("D");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void txtStatus_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			bool flag = (FormMode == Enums.FormViewModeType.View || FormMode == Enums.FormViewModeType.Update) && Function.ToString(txtStatus.EditValue).Equals("A");
			gvDoc.ReadOnly = flag;
			DateEditEx dateEditEx = txtCountDate;
			ButtonEditCFL buttonEditCFL = txtWhsCode;
			TimeEditEx timeEditEx = txtTime;
			ButtonEditCFL buttonEditCFL2 = txtBinLocation;
			ButtonEditCFL buttonEditCFL3 = txtTakerId;
			LookUpEditEx lookUpEditEx = txtTakerType;
			LookUpEditEx lookUpEditEx2 = txtStatus;
			TextEditEx textEditEx = txtBarcode;
			bool flag2 = (txtSameBatch.ReadOnly = flag);
			bool flag4 = (textEditEx.ReadOnly = flag2);
			bool flag6 = (lookUpEditEx2.ReadOnly = flag4);
			bool flag8 = (lookUpEditEx.ReadOnly = flag6);
			bool flag10 = (buttonEditCFL3.ReadOnly = flag8);
			bool flag12 = (buttonEditCFL2.ReadOnly = flag10);
			bool flag14 = (timeEditEx.ReadOnly = flag12);
			bool readOnly = (buttonEditCFL.ReadOnly = flag14);
			dateEditEx.ReadOnly = readOnly;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_BarcodeInventoryCounting_FormClosing(object sender, FormClosingEventArgs e)
	{
		if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
		{
			e.Cancel = true;
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		await NextLastData(Enums.NavigateType.Previous);
	}

	public async Task MOVE_NEXT()
	{
		await NextLastData(Enums.NavigateType.Next);
	}

	public async Task MOVE_FIRST()
	{
		await NextLastData(Enums.NavigateType.First);
	}

	public async Task MOVE_LAST()
	{
		await NextLastData(Enums.NavigateType.Last);
	}

	public async Task MODE_FIND()
	{
		FormMode = Enums.FormViewModeType.Find;
		try
		{
			DataTable dataTable = _structureDoc.Clone();
			DataRow row = dataTable.NewRow();
			dataTable.Rows.Add(row);
			DataSource = dataTable;
			grdDoc.DataSourceTable.Rows.Clear();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		await NextLastData(Enums.NavigateType.Current);
	}

	public async Task REFRESH()
	{
		FormMode = Enums.FormViewModeType.Busy;
		_isLoadData = true;
		WinCommons._OpenProcessing();
		try
		{
			DataTable dtAdd = _structureDoc.Clone();
			DataRow rowAdd = dtAdd.NewRow();
			DataRow dataRow = rowAdd;
			dataRow["DocEntry"] = await HttpClient.BarcodeInventoryCountingClient.GetNewDocEntry();
			rowAdd["CountDate"] = DateTime.Now.Date;
			rowAdd["CountTime"] = DateTime.Now.Hour * 100 + DateTime.Now.Minute;
			rowAdd["TakerType"] = 12;
			rowAdd["TakerId"] = Function.ToString(GlobalData.UserID);
			rowAdd["Status"] = "D";
			dtAdd.Rows.Add(rowAdd);
			DataSource = dtAdd;
			DataTable dataSource = _structureLine.Clone();
			grdDoc.DataSource = dataSource;
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.Add;
		_isLoadData = false;
		WinCommons._CloseProcessing();
	}

	private async Task SearchDoc()
	{
		_ = 2;
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("DocEntry", typeof(int));
			dataTable.Columns.Add("CountDate", typeof(DateTime));
			dataTable.Columns.Add("TakerType", typeof(int));
			dataTable.Columns.Add("TakerId", typeof(string));
			dataTable.Columns.Add("WhsCode", typeof(string));
			dataTable.Columns.Add("BinCode", typeof(string));
			dataTable.Columns.Add("Status", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			if (!string.IsNullOrEmpty(txtDocEntry.Text))
			{
				dataRow[0] = txtDocEntry.EditValue;
			}
			if (!string.IsNullOrEmpty(txtCountDate.Text))
			{
				dataRow[1] = txtCountDate.EditValue;
			}
			if (!string.IsNullOrEmpty(txtTakerType.Text))
			{
				dataRow[2] = txtTakerType.EditValue;
			}
			if (!string.IsNullOrEmpty(txtTakerId.Text))
			{
				dataRow[3] = txtTakerId.EditValue;
			}
			if (!string.IsNullOrEmpty(txtWhsCode.Text))
			{
				dataRow[4] = txtWhsCode.EditValue;
			}
			if (!string.IsNullOrEmpty(txtBinLocation.Text))
			{
				dataRow[5] = txtBinLocation.EditValue;
			}
			if (!string.IsNullOrEmpty(txtStatus.Text))
			{
				dataRow[6] = txtStatus.EditValue;
			}
			dataTable.Rows.Add(dataRow);
			DataTable dataTable2 = await HttpClient.BarcodeInventoryCountingClient.SearchDoc(dataTable);
			if (dataTable2 == null || dataTable2.Rows.Count == 0)
			{
				WinCommons.Warning(Enums.MessageCode.NoMatchingRecordFound, "");
				return;
			}
			if (dataTable2.Rows.Count == 1)
			{
				DataSource = dataTable2;
				await LoadDataLine(Function.ParseInt(txtDocEntry.EditValue));
			}
			else
			{
				frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.BarcodeInventoryCounting, multiSelect: false, "");
				frm_ChooseFromList2.DataBarcodeCounting = dataTable2;
				frm_ChooseFromList2.ShowDialog(this);
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					DataTable dataTable3 = dataTable2.Clone();
					DataRow dataRow2 = dataTable3.NewRow();
					if (frm_ChooseFromList2.ChooseItem is DataRowView dataRowView)
					{
						dataRow2.ItemArray = dataRowView.Row.ItemArray;
					}
					dataTable3.Rows.Add(dataRow2);
					dataTable3.AcceptChanges();
					DataSource = dataTable3;
					await LoadDataLine(Function.ParseInt(txtDocEntry.EditValue));
				}
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadDataLine(int docEntry)
	{
		try
		{
			DataTable dataSource = await HttpClient.BarcodeInventoryCountingClient.GetDataLineByDocEntry(docEntry);
			grdDoc.DataSource = dataSource;
			grdDoc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task NextLastData(Enums.NavigateType type)
	{
		if (((FormMode == Enums.FormViewModeType.Add && grdDoc.DataSourceTable.IsNotNull()) || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
		{
			return;
		}
		WinCommons._OpenProcessing();
		_isLoadData = true;
		try
		{
			DataTable dataTable = await HttpClient.BarcodeInventoryCountingClient.GetNextLastData(type, Function.ParseInt(txtDocEntry.EditValue));
			if (dataTable.IsNotNull())
			{
				DataSource = dataTable;
				await LoadDataLine(Function.ParseInt(txtDocEntry.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoadData = false;
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private async Task LoadUomGroup()
	{
		try
		{
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = rUomGroup;
			repositoryItemLookUpEdit.DataSource = await HttpClient.Commons.GetDataUomGroup();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task SaveData(bool isAdNew)
	{
		Enums.FormViewModeType oldView = FormMode;
		try
		{
			DataTable dataTable = DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["TakerId"])))
			{
				WinCommons.Error(Enums.MessageCode.TakerIdCannotBeNull, "");
				return;
			}
			if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[0]["WhsCode"])))
			{
				WinCommons.Error(Enums.MessageCode.WarehouseCannotBeNull, "");
				return;
			}
			dataTable.TableName = "DataDoc";
			DataTable dataSourceTable = grdDoc.DataSourceTable;
			if (!dataSourceTable.IsNotNull())
			{
				WinCommons.Error(Enums.MessageCode.DataCountingCannotBenull, "");
				return;
			}
			dataSourceTable.TableName = "DataLine";
			FormMode = Enums.FormViewModeType.Busy;
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dataTable.Copy());
			dataSet.Tables.Add(dataSourceTable.Copy());
			HttpResult httpResult = ((!isAdNew) ? (await HttpClient.BarcodeInventoryCountingClient.UpdateData(dataSet)) : (await HttpClient.BarcodeInventoryCountingClient.AddNew(dataSet)));
			HttpResult httpResult2 = httpResult;
			if (string.IsNullOrEmpty(httpResult2.Message))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccessWithDocEntry, "", Function.ToString(httpResult2.Content));
				oldView = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = oldView;
		WinCommons._CloseProcessing();
	}

	private void SetColorControll(Enums.FormViewModeType type)
	{
		try
		{
			AppearanceObject appearance = txtCountDate.Properties.Appearance;
			AppearanceObject appearance2 = txtTime.Properties.Appearance;
			AppearanceObject appearance3 = txtTakerType.Properties.Appearance;
			AppearanceObject appearance4 = txtTakerId.Properties.Appearance;
			AppearanceObject appearance5 = txtWhsCode.Properties.Appearance;
			AppearanceObject appearance6 = txtBinLocation.Properties.Appearance;
			AppearanceObject appearance7 = txtBarcode.Properties.Appearance;
			AppearanceObject appearance8 = txtDocEntry.Properties.Appearance;
			AppearanceObject appearance9 = txtStatus.Properties.Appearance;
			Color color = (txtRemarks.Properties.Appearance.BackColor = ((type == Enums.FormViewModeType.Find) ? Color.PapayaWhip : Color.White));
			Color color3 = (appearance9.BackColor = color);
			Color color5 = (appearance8.BackColor = color3);
			Color color7 = (appearance7.BackColor = color5);
			Color color9 = (appearance6.BackColor = color7);
			Color color11 = (appearance5.BackColor = color9);
			Color color13 = (appearance4.BackColor = color11);
			Color color15 = (appearance3.BackColor = color13);
			Color backColor = (appearance2.BackColor = color15);
			appearance.BackColor = backColor;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled6 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled7 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled8 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions6 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled9 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled10 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions7 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.InventoryTransaction.frm_BarcodeInventoryCounting));
		DevExpress.Utils.SerializableAppearanceObject appearance11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed11 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled11 = new DevExpress.Utils.SerializableAppearanceObject();
		this.lbCountDate = new Apzon.Controls.Controls.LabelLine();
		this.lbTime = new Apzon.Controls.Controls.LabelLine();
		this.lbInventoryCounter = new Apzon.Controls.Controls.LabelLine();
		this.lbWarehouseBin = new Apzon.Controls.Controls.LabelLine();
		this.lbBarcode = new Apzon.Controls.Controls.LabelLine();
		this.lbDocNo = new Apzon.Controls.Controls.LabelLine();
		this.lbStatus = new Apzon.Controls.Controls.LabelLine();
		this.lbRemark = new Apzon.Controls.Controls.LabelLine();
		this.txtCountDate = new Apzon.Controls.Editor.DateEditEx();
		this.txtTime = new Apzon.Controls.Editor.TimeEditEx();
		this.txtTakerType = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtBarcode = new Apzon.Controls.Editor.TextEditEx();
		this.txtDocEntry = new Apzon.Controls.Editor.CalcEditEx();
		this.txtStatus = new Apzon.Controls.Editor.LookUpEditEx();
		this.txtRemarks = new Apzon.Controls.Editor.TextEditEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStripLine = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deletesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcItemName = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcDistNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.rDistNumber = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcMnfSerial = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLotNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.gcInWhsQty = new Apzon.Controls.Grid.GridColumnEx();
		this.rCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcStatus = new Apzon.Controls.Grid.GridColumnEx();
		this.rStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcCountQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcInvUom = new Apzon.Controls.Grid.GridColumnEx();
		this.rInventoryUom = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcUomGroup = new Apzon.Controls.Grid.GridColumnEx();
		this.rUomGroup = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcUomName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcUomCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rUomCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcItmPerUnit = new Apzon.Controls.Grid.GridColumnEx();
		this.gcInvQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemark = new Apzon.Controls.Grid.GridColumnEx();
		this.gcBarCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcDifference = new Apzon.Controls.Grid.GridColumnEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnAdd = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txtSameBatch = new Apzon.Controls.Editor.CheckEditEx();
		this.txtWhsCode = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txtTakerId = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txtBinLocation = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.contextMenuStripDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.txtCountDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtCountDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTime.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTakerType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBarcode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDocEntry.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtRemarks.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStripLine.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDistNumber).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rInventoryUom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUomGroup).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUomCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSameBatch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtWhsCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtTakerId.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtBinLocation.Properties).BeginInit();
		this.contextMenuStripDoc.SuspendLayout();
		base.SuspendLayout();
		this.lbCountDate.EditControl = null;
		this.lbCountDate.EditValue = "Count Date";
		this.lbCountDate.FieldName = null;
		this.lbCountDate.HideWhenEmpty = false;
		this.lbCountDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbCountDate.Location = new System.Drawing.Point(12, 11);
		this.lbCountDate.Name = "lbCountDate";
		this.lbCountDate.ReadOnly = false;
		this.lbCountDate.SettingInVisible = false;
		this.lbCountDate.SettingReadonly = false;
		this.lbCountDate.ShowUnderLine = true;
		this.lbCountDate.Size = new System.Drawing.Size(110, 20);
		this.lbCountDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbCountDate.TabIndex = 0;
		this.lbCountDate.Utext = "Count Date";
		this.lbTime.EditControl = null;
		this.lbTime.EditValue = "Time";
		this.lbTime.FieldName = null;
		this.lbTime.HideWhenEmpty = false;
		this.lbTime.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbTime.Location = new System.Drawing.Point(257, 8);
		this.lbTime.Name = "lbTime";
		this.lbTime.ReadOnly = false;
		this.lbTime.SettingInVisible = false;
		this.lbTime.SettingReadonly = false;
		this.lbTime.ShowUnderLine = true;
		this.lbTime.Size = new System.Drawing.Size(30, 20);
		this.lbTime.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbTime.TabIndex = 0;
		this.lbTime.Utext = "Time";
		this.lbInventoryCounter.EditControl = null;
		this.lbInventoryCounter.EditValue = "Inventory Counter";
		this.lbInventoryCounter.FieldName = null;
		this.lbInventoryCounter.HideWhenEmpty = false;
		this.lbInventoryCounter.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbInventoryCounter.Location = new System.Drawing.Point(12, 37);
		this.lbInventoryCounter.Name = "lbInventoryCounter";
		this.lbInventoryCounter.ReadOnly = false;
		this.lbInventoryCounter.SettingInVisible = false;
		this.lbInventoryCounter.SettingReadonly = false;
		this.lbInventoryCounter.ShowUnderLine = true;
		this.lbInventoryCounter.Size = new System.Drawing.Size(110, 20);
		this.lbInventoryCounter.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbInventoryCounter.TabIndex = 0;
		this.lbInventoryCounter.Utext = "Inventory Counter";
		this.lbWarehouseBin.EditControl = null;
		this.lbWarehouseBin.EditValue = "Warehouse/BIN";
		this.lbWarehouseBin.FieldName = null;
		this.lbWarehouseBin.HideWhenEmpty = false;
		this.lbWarehouseBin.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbWarehouseBin.Location = new System.Drawing.Point(12, 63);
		this.lbWarehouseBin.Name = "lbWarehouseBin";
		this.lbWarehouseBin.ReadOnly = false;
		this.lbWarehouseBin.SettingInVisible = false;
		this.lbWarehouseBin.SettingReadonly = false;
		this.lbWarehouseBin.ShowUnderLine = true;
		this.lbWarehouseBin.Size = new System.Drawing.Size(110, 20);
		this.lbWarehouseBin.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbWarehouseBin.TabIndex = 0;
		this.lbWarehouseBin.Utext = "Warehouse/BIN";
		this.lbBarcode.EditControl = null;
		this.lbBarcode.EditValue = "Barcode(2D/1D)";
		this.lbBarcode.FieldName = null;
		this.lbBarcode.HideWhenEmpty = false;
		this.lbBarcode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBarcode.Location = new System.Drawing.Point(12, 89);
		this.lbBarcode.Name = "lbBarcode";
		this.lbBarcode.ReadOnly = false;
		this.lbBarcode.SettingInVisible = false;
		this.lbBarcode.SettingReadonly = false;
		this.lbBarcode.ShowUnderLine = true;
		this.lbBarcode.Size = new System.Drawing.Size(110, 20);
		this.lbBarcode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBarcode.TabIndex = 0;
		this.lbBarcode.Utext = "Barcode(2D/1D)";
		this.lbDocNo.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbDocNo.EditControl = null;
		this.lbDocNo.EditValue = "Document No.";
		this.lbDocNo.FieldName = null;
		this.lbDocNo.HideWhenEmpty = false;
		this.lbDocNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbDocNo.Location = new System.Drawing.Point(1076, 12);
		this.lbDocNo.Name = "lbDocNo";
		this.lbDocNo.ReadOnly = false;
		this.lbDocNo.SettingInVisible = false;
		this.lbDocNo.SettingReadonly = false;
		this.lbDocNo.ShowUnderLine = true;
		this.lbDocNo.Size = new System.Drawing.Size(80, 20);
		this.lbDocNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbDocNo.TabIndex = 0;
		this.lbDocNo.Utext = "Document No.";
		this.lbStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbStatus.EditControl = null;
		this.lbStatus.EditValue = "Status";
		this.lbStatus.FieldName = null;
		this.lbStatus.HideWhenEmpty = false;
		this.lbStatus.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbStatus.Location = new System.Drawing.Point(1076, 37);
		this.lbStatus.Name = "lbStatus";
		this.lbStatus.ReadOnly = false;
		this.lbStatus.SettingInVisible = false;
		this.lbStatus.SettingReadonly = false;
		this.lbStatus.ShowUnderLine = true;
		this.lbStatus.Size = new System.Drawing.Size(80, 20);
		this.lbStatus.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbStatus.TabIndex = 0;
		this.lbStatus.Utext = "Status";
		this.lbRemark.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.lbRemark.EditControl = null;
		this.lbRemark.EditValue = "Remark";
		this.lbRemark.FieldName = null;
		this.lbRemark.HideWhenEmpty = false;
		this.lbRemark.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbRemark.Location = new System.Drawing.Point(1076, 63);
		this.lbRemark.Name = "lbRemark";
		this.lbRemark.ReadOnly = false;
		this.lbRemark.SettingInVisible = false;
		this.lbRemark.SettingReadonly = false;
		this.lbRemark.ShowUnderLine = true;
		this.lbRemark.Size = new System.Drawing.Size(80, 20);
		this.lbRemark.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbRemark.TabIndex = 0;
		this.lbRemark.Utext = "Remark";
		this.txtCountDate.EditControl = null;
		this.txtCountDate.EditValue = null;
		this.txtCountDate.FieldName = "CountDate";
		this.txtCountDate.Location = new System.Drawing.Point(122, 8);
		this.txtCountDate.Name = "txtCountDate";
		this.txtCountDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtCountDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtCountDate.Properties.IsSearchControl = false;
		this.txtCountDate.SettingInVisible = false;
		this.txtCountDate.SettingReadonly = false;
		this.txtCountDate.Size = new System.Drawing.Size(120, 20);
		this.txtCountDate.TabIndex = 1;
		this.txtTime.EditControl = null;
		this.txtTime.FieldName = "CountTime";
		this.txtTime.IsSearchControl = false;
		this.txtTime.Location = new System.Drawing.Point(293, 7);
		this.txtTime.Name = "txtTime";
		this.txtTime.Properties.DisplayFormat.FormatString = "##:##";
		this.txtTime.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txtTime.Properties.EditFormat.FormatString = "##:##";
		this.txtTime.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.Custom;
		this.txtTime.Properties.Mask.EditMask = "(2[0-3]|[01]?[0-9]):([0-5]{1}[0-9])";
		this.txtTime.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.RegEx;
		this.txtTime.SettingInVisible = false;
		this.txtTime.SettingReadonly = false;
		this.txtTime.Size = new System.Drawing.Size(74, 20);
		this.txtTime.TabIndex = 2;
		this.txtTakerType.EditControl = null;
		this.txtTakerType.FieldName = "TakerType";
		this.txtTakerType.IsSearchControl = false;
		this.txtTakerType.Location = new System.Drawing.Point(122, 34);
		this.txtTakerType.Name = "txtTakerType";
		this.txtTakerType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtTakerType.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.txtTakerType.Properties.DisplayMember = "Name";
		this.txtTakerType.Properties.NullText = "";
		this.txtTakerType.Properties.ShowHeader = false;
		this.txtTakerType.Properties.ValueMember = "Code";
		this.txtTakerType.SettingInVisible = false;
		this.txtTakerType.SettingReadonly = false;
		this.txtTakerType.Size = new System.Drawing.Size(120, 20);
		this.txtTakerType.TabIndex = 3;
		this.txtTakerType.EditValueChanged += new System.EventHandler(this.txtTakerType_EditValueChanged);
		this.txtBarcode.EditControl = null;
		this.txtBarcode.FieldName = "BarCode";
		this.txtBarcode.IsSearchControl = false;
		this.txtBarcode.Location = new System.Drawing.Point(122, 86);
		this.txtBarcode.Name = "txtBarcode";
		this.txtBarcode.SettingInVisible = false;
		this.txtBarcode.SettingReadonly = false;
		this.txtBarcode.Size = new System.Drawing.Size(245, 20);
		this.txtBarcode.TabIndex = 10;
		this.txtBarcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtBarcode_KeyDown);
		this.txtDocEntry.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtDocEntry.EditControl = null;
		this.txtDocEntry.FieldName = "DocEntry";
		this.txtDocEntry.IsSearchControl = false;
		this.txtDocEntry.Location = new System.Drawing.Point(1157, 9);
		this.txtDocEntry.Name = "txtDocEntry";
		this.txtDocEntry.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtDocEntry.Properties.IsSearchControl = false;
		this.txtDocEntry.SettingInVisible = false;
		this.txtDocEntry.SettingReadonly = false;
		this.txtDocEntry.Size = new System.Drawing.Size(100, 20);
		this.txtDocEntry.TabIndex = 7;
		this.txtStatus.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtStatus.EditControl = null;
		this.txtStatus.FieldName = "Status";
		this.txtStatus.IsSearchControl = false;
		this.txtStatus.Location = new System.Drawing.Point(1157, 34);
		this.txtStatus.Name = "txtStatus";
		this.txtStatus.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.txtStatus.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.txtStatus.Properties.DisplayMember = "Name";
		this.txtStatus.Properties.NullText = "";
		this.txtStatus.Properties.ShowHeader = false;
		this.txtStatus.Properties.ValueMember = "Code";
		this.txtStatus.SettingInVisible = false;
		this.txtStatus.SettingReadonly = false;
		this.txtStatus.Size = new System.Drawing.Size(100, 20);
		this.txtStatus.TabIndex = 8;
		this.txtStatus.EditValueChanged += new System.EventHandler(this.txtStatus_EditValueChanged);
		this.txtRemarks.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txtRemarks.EditControl = null;
		this.txtRemarks.FieldName = "Remarks";
		this.txtRemarks.IsSearchControl = false;
		this.txtRemarks.Location = new System.Drawing.Point(1157, 60);
		this.txtRemarks.Name = "txtRemarks";
		this.txtRemarks.SettingInVisible = false;
		this.txtRemarks.SettingReadonly = false;
		this.txtRemarks.Size = new System.Drawing.Size(100, 20);
		this.txtRemarks.TabIndex = 9;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStripLine;
		this.grdDoc.Location = new System.Drawing.Point(1, 115);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[8] { this.rItemCode, this.rItemName, this.rDistNumber, this.rCal, this.rInventoryUom, this.rUomCode, this.rUomGroup, this.rStatus });
		this.grdDoc.Size = new System.Drawing.Size(1265, 386);
		this.grdDoc.TabIndex = 18;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStripLine.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deletesToolStripMenuItem });
		this.contextMenuStripLine.Name = "contextMenuStripLine";
		this.contextMenuStripLine.Size = new System.Drawing.Size(150, 48);
		this.contextMenuStripLine.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripLine_Opening);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
		this.addNewToolStripMenuItem.Text = "Add New Row";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deletesToolStripMenuItem.Name = "deletesToolStripMenuItem";
		this.deletesToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
		this.deletesToolStripMenuItem.Text = "Delete(s)";
		this.deletesToolStripMenuItem.Click += new System.EventHandler(this.deletesToolStripMenuItem_Click);
		this.gvDoc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[17]
		{
			this.gcItemCode, this.gcItemName, this.gcDistNumber, this.gcMnfSerial, this.gcLotNumber, this.gcInWhsQty, this.gcStatus, this.gcCountQty, this.gcInvUom, this.gcUomGroup,
			this.gcUomName, this.gcUomCode, this.gcItmPerUnit, this.gcInvQty, this.gcRemark, this.gcBarCode, this.gcDifference
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oBarcodeCounting";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "APZ_BIC1";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gvDoc.OptionsPrint.AutoWidth = false;
		this.gvDoc.OptionsPrint.UsePrintStyles = false;
		this.gvDoc.OptionsView.BestFitMaxRowCount = 100;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvDoc.OptionsView.ShowGroupedColumns = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CellValueChangedAsync += new Apzon.Controls.Grid.CellValueChangedAsync(this.gvDoc_CellValueChangedAsync);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gcItemCode.Caption = "Item Code";
		this.gcItemCode.ColumnEdit = this.rItemCode;
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.PrimaryKey = true;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 0;
		this.gcItemCode.Width = 107;
		this.rItemCode.AutoHeight = false;
		this.rItemCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemCode.ContainerControl = this;
		this.rItemCode.DisplayMember = "ItemCode";
		this.rItemCode.DisplayMember2 = "";
		this.rItemCode.LoadDataSourceWhenEnter = false;
		this.rItemCode.Name = "rItemCode";
		this.rItemCode.SeparateCharacterForMulti = ';';
		this.rItemCode.SeparateString = ", ";
		this.rItemCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.rItemCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oItems;
		this.rItemCode.UsingAutoCompleteBox = false;
		this.rItemCode.UsingDataSource = true;
		this.rItemCode.ValidateWhenNoDataSource = false;
		this.rItemCode.ValueMember = "ItemCode";
		this.gcItemName.Caption = "Item Name";
		this.gcItemName.ColumnEdit = this.rItemName;
		this.gcItemName.CurrencyDateField = null;
		this.gcItemName.CurrencyField = null;
		this.gcItemName.CurrencyFixed = null;
		this.gcItemName.CurrencyFixedRate = 0.0;
		this.gcItemName.CurrencyRateField = null;
		this.gcItemName.FieldName = "ItemName";
		this.gcItemName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcItemName.Name = "gcItemName";
		this.gcItemName.PrimaryKey = false;
		this.gcItemName.UnitField = null;
		this.gcItemName.Visible = true;
		this.gcItemName.VisibleIndex = 1;
		this.gcItemName.Width = 131;
		this.rItemName.AutoHeight = false;
		this.rItemName.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions10.Image");
		this.rItemName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemName.ContainerControl = this;
		this.rItemName.DisplayMember = "ItemName";
		this.rItemName.DisplayMember2 = "";
		this.rItemName.LoadDataSourceWhenEnter = false;
		this.rItemName.Name = "rItemName";
		this.rItemName.SeparateCharacterForMulti = ';';
		this.rItemName.SeparateString = ", ";
		this.rItemName.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.rItemName.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oItems;
		this.rItemName.UsingAutoCompleteBox = false;
		this.rItemName.UsingDataSource = true;
		this.rItemName.ValidateWhenNoDataSource = false;
		this.rItemName.ValueMember = "ItemName";
		this.gcDistNumber.Caption = "Batch No.";
		this.gcDistNumber.ColumnEdit = this.rDistNumber;
		this.gcDistNumber.CurrencyDateField = null;
		this.gcDistNumber.CurrencyField = null;
		this.gcDistNumber.CurrencyFixed = null;
		this.gcDistNumber.CurrencyFixedRate = 0.0;
		this.gcDistNumber.CurrencyRateField = null;
		this.gcDistNumber.FieldName = "DistNumber";
		this.gcDistNumber.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcDistNumber.Name = "gcDistNumber";
		this.gcDistNumber.PrimaryKey = false;
		this.gcDistNumber.UnitField = null;
		this.gcDistNumber.Visible = true;
		this.gcDistNumber.VisibleIndex = 2;
		this.gcDistNumber.Width = 99;
		this.rDistNumber.AutoHeight = false;
		editorButtonImageOptions4.Image = Apzon.Forms.Properties.Resources.right;
		this.rDistNumber.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rDistNumber.Name = "rDistNumber";
		this.gcMnfSerial.Caption = "Mnf Serial";
		this.gcMnfSerial.CurrencyDateField = null;
		this.gcMnfSerial.CurrencyField = null;
		this.gcMnfSerial.CurrencyFixed = null;
		this.gcMnfSerial.CurrencyFixedRate = 0.0;
		this.gcMnfSerial.CurrencyRateField = null;
		this.gcMnfSerial.FieldName = "MnfSerial";
		this.gcMnfSerial.Name = "gcMnfSerial";
		this.gcMnfSerial.PrimaryKey = false;
		this.gcMnfSerial.UnitField = null;
		this.gcMnfSerial.Visible = true;
		this.gcMnfSerial.VisibleIndex = 3;
		this.gcLotNumber.Caption = "Lot Number";
		this.gcLotNumber.CurrencyDateField = null;
		this.gcLotNumber.CurrencyField = null;
		this.gcLotNumber.CurrencyFixed = null;
		this.gcLotNumber.CurrencyFixedRate = 0.0;
		this.gcLotNumber.CurrencyRateField = null;
		this.gcLotNumber.FieldName = "LotNumber";
		this.gcLotNumber.Name = "gcLotNumber";
		this.gcLotNumber.PrimaryKey = false;
		this.gcLotNumber.UnitField = null;
		this.gcLotNumber.Visible = true;
		this.gcLotNumber.VisibleIndex = 4;
		this.gcInWhsQty.Caption = "In-Whs Quantity";
		this.gcInWhsQty.ColumnEdit = this.rCal;
		this.gcInWhsQty.CurrencyDateField = null;
		this.gcInWhsQty.CurrencyField = null;
		this.gcInWhsQty.CurrencyFixed = null;
		this.gcInWhsQty.CurrencyFixedRate = 0.0;
		this.gcInWhsQty.CurrencyRateField = null;
		this.gcInWhsQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcInWhsQty.FieldName = "InWhsQty";
		this.gcInWhsQty.Name = "gcInWhsQty";
		this.gcInWhsQty.OptionsColumn.AllowEdit = false;
		this.gcInWhsQty.OptionsColumn.ReadOnly = true;
		this.gcInWhsQty.PrimaryKey = false;
		this.gcInWhsQty.UnitField = null;
		this.gcInWhsQty.Visible = true;
		this.gcInWhsQty.VisibleIndex = 5;
		this.rCal.AutoHeight = false;
		this.rCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCal.Name = "rCal";
		this.gcStatus.Caption = "Status";
		this.gcStatus.ColumnEdit = this.rStatus;
		this.gcStatus.CurrencyDateField = null;
		this.gcStatus.CurrencyField = null;
		this.gcStatus.CurrencyFixed = null;
		this.gcStatus.CurrencyFixedRate = 0.0;
		this.gcStatus.CurrencyRateField = null;
		this.gcStatus.FieldName = "LineStatus";
		this.gcStatus.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcStatus.Name = "gcStatus";
		this.gcStatus.PrimaryKey = false;
		this.gcStatus.UnitField = null;
		this.gcStatus.Visible = true;
		this.gcStatus.VisibleIndex = 6;
		this.rStatus.AutoHeight = false;
		this.rStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rStatus.DisplayMember = "Name";
		this.rStatus.Name = "rStatus";
		this.rStatus.NullText = "";
		this.rStatus.ShowHeader = false;
		this.rStatus.ValueMember = "Code";
		this.gcCountQty.Caption = "Counted Qty";
		this.gcCountQty.ColumnEdit = this.rCal;
		this.gcCountQty.CurrencyDateField = null;
		this.gcCountQty.CurrencyField = null;
		this.gcCountQty.CurrencyFixed = null;
		this.gcCountQty.CurrencyFixedRate = 0.0;
		this.gcCountQty.CurrencyRateField = null;
		this.gcCountQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcCountQty.FieldName = "CountQty";
		this.gcCountQty.Name = "gcCountQty";
		this.gcCountQty.PrimaryKey = false;
		this.gcCountQty.UnitField = null;
		this.gcCountQty.Visible = true;
		this.gcCountQty.VisibleIndex = 7;
		this.gcInvUom.Caption = "Inventory Uom";
		this.gcInvUom.ColumnEdit = this.rInventoryUom;
		this.gcInvUom.CurrencyDateField = null;
		this.gcInvUom.CurrencyField = null;
		this.gcInvUom.CurrencyFixed = null;
		this.gcInvUom.CurrencyFixedRate = 0.0;
		this.gcInvUom.CurrencyRateField = null;
		this.gcInvUom.FieldName = "InvUoM";
		this.gcInvUom.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcInvUom.Name = "gcInvUom";
		this.gcInvUom.PrimaryKey = false;
		this.gcInvUom.UnitField = null;
		this.gcInvUom.Visible = true;
		this.gcInvUom.VisibleIndex = 14;
		this.gcInvUom.Width = 106;
		this.rInventoryUom.AutoHeight = false;
		this.rInventoryUom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rInventoryUom.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rInventoryUom.DisplayMember = "Name";
		this.rInventoryUom.Name = "rInventoryUom";
		this.rInventoryUom.NullText = "";
		this.rInventoryUom.ShowHeader = false;
		this.rInventoryUom.ValueMember = "Code";
		this.gcUomGroup.Caption = "Uom Group";
		this.gcUomGroup.ColumnEdit = this.rUomGroup;
		this.gcUomGroup.CurrencyDateField = null;
		this.gcUomGroup.CurrencyField = null;
		this.gcUomGroup.CurrencyFixed = null;
		this.gcUomGroup.CurrencyFixedRate = 0.0;
		this.gcUomGroup.CurrencyRateField = null;
		this.gcUomGroup.FieldName = "UgpCode";
		this.gcUomGroup.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcUomGroup.Name = "gcUomGroup";
		this.gcUomGroup.OptionsColumn.AllowEdit = false;
		this.gcUomGroup.PrimaryKey = false;
		this.gcUomGroup.UnitField = null;
		this.gcUomGroup.Visible = true;
		this.gcUomGroup.VisibleIndex = 16;
		this.rUomGroup.AutoHeight = false;
		this.rUomGroup.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rUomGroup.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code", 20, DevExpress.Utils.FormatType.None, "", false, DevExpress.Utils.HorzAlignment.Default, DevExpress.Data.ColumnSortOrder.None, DevExpress.Utils.DefaultBoolean.Default),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rUomGroup.DisplayMember = "Name";
		this.rUomGroup.Name = "rUomGroup";
		this.rUomGroup.NullText = "";
		this.rUomGroup.ShowHeader = false;
		this.rUomGroup.ValueMember = "Code";
		this.gcUomName.Caption = "Uom Name";
		this.gcUomName.CurrencyDateField = null;
		this.gcUomName.CurrencyField = null;
		this.gcUomName.CurrencyFixed = null;
		this.gcUomName.CurrencyFixedRate = 0.0;
		this.gcUomName.CurrencyRateField = null;
		this.gcUomName.FieldName = "unitMsr";
		this.gcUomName.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcUomName.Name = "gcUomName";
		this.gcUomName.PrimaryKey = false;
		this.gcUomName.UnitField = null;
		this.gcUomName.Visible = true;
		this.gcUomName.VisibleIndex = 8;
		this.gcUomCode.Caption = "Uom Counted";
		this.gcUomCode.ColumnEdit = this.rUomCode;
		this.gcUomCode.CurrencyDateField = null;
		this.gcUomCode.CurrencyField = null;
		this.gcUomCode.CurrencyFixed = null;
		this.gcUomCode.CurrencyFixedRate = 0.0;
		this.gcUomCode.CurrencyRateField = null;
		this.gcUomCode.FieldName = "UomCode";
		this.gcUomCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcUomCode.Name = "gcUomCode";
		this.gcUomCode.PrimaryKey = false;
		this.gcUomCode.UnitField = null;
		this.gcUomCode.Visible = true;
		this.gcUomCode.VisibleIndex = 9;
		this.rUomCode.AutoHeight = false;
		this.rUomCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rUomCode.Name = "rUomCode";
		this.rUomCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rUomCode_ButtonClick);
		this.gcItmPerUnit.Caption = "Item Per Unit";
		this.gcItmPerUnit.ColumnEdit = this.rCal;
		this.gcItmPerUnit.CurrencyDateField = null;
		this.gcItmPerUnit.CurrencyField = null;
		this.gcItmPerUnit.CurrencyFixed = null;
		this.gcItmPerUnit.CurrencyFixedRate = 0.0;
		this.gcItmPerUnit.CurrencyRateField = null;
		this.gcItmPerUnit.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Unit;
		this.gcItmPerUnit.FieldName = "NumPerMsr";
		this.gcItmPerUnit.Name = "gcItmPerUnit";
		this.gcItmPerUnit.PrimaryKey = false;
		this.gcItmPerUnit.UnitField = null;
		this.gcItmPerUnit.Visible = true;
		this.gcItmPerUnit.VisibleIndex = 10;
		this.gcInvQty.Caption = "Inventory Qty";
		this.gcInvQty.ColumnEdit = this.rCal;
		this.gcInvQty.CurrencyDateField = null;
		this.gcInvQty.CurrencyField = null;
		this.gcInvQty.CurrencyFixed = null;
		this.gcInvQty.CurrencyFixedRate = 0.0;
		this.gcInvQty.CurrencyRateField = null;
		this.gcInvQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcInvQty.FieldName = "InvQty";
		this.gcInvQty.Name = "gcInvQty";
		this.gcInvQty.PrimaryKey = false;
		this.gcInvQty.UnitField = null;
		this.gcInvQty.Visible = true;
		this.gcInvQty.VisibleIndex = 11;
		this.gcInvQty.Width = 103;
		this.gcRemark.Caption = "Remark";
		this.gcRemark.CurrencyDateField = null;
		this.gcRemark.CurrencyField = null;
		this.gcRemark.CurrencyFixed = null;
		this.gcRemark.CurrencyFixedRate = 0.0;
		this.gcRemark.CurrencyRateField = null;
		this.gcRemark.FieldName = "Remark";
		this.gcRemark.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcRemark.Name = "gcRemark";
		this.gcRemark.PrimaryKey = false;
		this.gcRemark.UnitField = null;
		this.gcRemark.Visible = true;
		this.gcRemark.VisibleIndex = 12;
		this.gcBarCode.Caption = "BarCode";
		this.gcBarCode.CurrencyDateField = null;
		this.gcBarCode.CurrencyField = null;
		this.gcBarCode.CurrencyFixed = null;
		this.gcBarCode.CurrencyFixedRate = 0.0;
		this.gcBarCode.CurrencyRateField = null;
		this.gcBarCode.FieldName = "BarCode";
		this.gcBarCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gcBarCode.Name = "gcBarCode";
		this.gcBarCode.PrimaryKey = false;
		this.gcBarCode.UnitField = null;
		this.gcBarCode.Visible = true;
		this.gcBarCode.VisibleIndex = 13;
		this.gcDifference.Caption = "Difference";
		this.gcDifference.ColumnEdit = this.rCal;
		this.gcDifference.CurrencyDateField = null;
		this.gcDifference.CurrencyField = null;
		this.gcDifference.CurrencyFixed = null;
		this.gcDifference.CurrencyFixedRate = 0.0;
		this.gcDifference.CurrencyRateField = null;
		this.gcDifference.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gcDifference.FieldName = "Difference";
		this.gcDifference.Name = "gcDifference";
		this.gcDifference.OptionsColumn.AllowEdit = false;
		this.gcDifference.PrimaryKey = false;
		this.gcDifference.UnitField = null;
		this.gcDifference.Visible = true;
		this.gcDifference.VisibleIndex = 15;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(1185, 507);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 19;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnAdd.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnAdd.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.btnAdd.EditControl = null;
		this.btnAdd.EditValue = "Add";
		this.btnAdd.FieldName = null;
		this.btnAdd.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnAdd.ImageOptions.Image");
		this.btnAdd.IsSearchControl = false;
		this.btnAdd.Location = new System.Drawing.Point(1104, 507);
		this.btnAdd.Name = "btnAdd";
		this.btnAdd.ReadOnly = false;
		this.btnAdd.SettingInVisible = false;
		this.btnAdd.SettingReadonly = false;
		this.btnAdd.Size = new System.Drawing.Size(75, 23);
		this.btnAdd.TabIndex = 20;
		this.btnAdd.Text = "Add";
		this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
		this.txtSameBatch.EditControl = null;
		this.txtSameBatch.FieldName = "txtSameBatch";
		this.txtSameBatch.IsSearchControl = false;
		this.txtSameBatch.Location = new System.Drawing.Point(374, 90);
		this.txtSameBatch.Name = "txtSameBatch";
		this.txtSameBatch.Properties.Caption = "Block Multiple Receipts for Same Batch";
		this.txtSameBatch.Properties.CheckStyle = DevExpress.XtraEditors.Controls.CheckStyles.Radio;
		this.txtSameBatch.SettingInVisible = false;
		this.txtSameBatch.SettingReadonly = false;
		this.txtSameBatch.Size = new System.Drawing.Size(221, 20);
		this.txtSameBatch.TabIndex = 11;
		this.txtWhsCode.EditControl = null;
		this.txtWhsCode.FieldName = "WhsCode";
		this.txtWhsCode.IsSearchControl = false;
		this.txtWhsCode.Location = new System.Drawing.Point(122, 60);
		this.txtWhsCode.Name = "txtWhsCode";
		this.txtWhsCode.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions5.Image = Apzon.Forms.Properties.Resources.right;
		this.txtWhsCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance6, appearanceHovered6, appearancePressed6, appearanceDisabled6, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance7, appearanceHovered7, appearancePressed7, appearanceDisabled7, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtWhsCode.Properties.ContainerControl = null;
		this.txtWhsCode.Properties.DisplayMember = "";
		this.txtWhsCode.Properties.DisplayMember2 = "";
		this.txtWhsCode.Properties.LoadDataSourceWhenEnter = false;
		this.txtWhsCode.Properties.SeparateCharacterForMulti = ';';
		this.txtWhsCode.Properties.SeparateString = ", ";
		this.txtWhsCode.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Warehouse;
		this.txtWhsCode.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oWarehouses;
		this.txtWhsCode.Properties.UsingAutoCompleteBox = false;
		this.txtWhsCode.Properties.UsingDataSource = true;
		this.txtWhsCode.Properties.ValidateWhenNoDataSource = false;
		this.txtWhsCode.Properties.ValueMember = "WhsCode";
		this.txtWhsCode.SettingInVisible = false;
		this.txtWhsCode.SettingReadonly = false;
		this.txtWhsCode.Size = new System.Drawing.Size(120, 20);
		this.txtWhsCode.TabIndex = 5;
		this.txtTakerId.EditControl = null;
		this.txtTakerId.FieldName = "TakerId";
		this.txtTakerId.IsSearchControl = false;
		this.txtTakerId.Location = new System.Drawing.Point(249, 34);
		this.txtTakerId.Name = "txtTakerId";
		this.txtTakerId.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions6.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions6.Image");
		this.txtTakerId.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance8, appearanceHovered8, appearancePressed8, appearanceDisabled8, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions6, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance9, appearanceHovered9, appearancePressed9, appearanceDisabled9, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtTakerId.Properties.ContainerControl = null;
		this.txtTakerId.Properties.DisplayMember = "";
		this.txtTakerId.Properties.DisplayMember2 = "";
		this.txtTakerId.Properties.LoadDataSourceWhenEnter = false;
		this.txtTakerId.Properties.SeparateCharacterForMulti = ';';
		this.txtTakerId.Properties.SeparateString = ", ";
		this.txtTakerId.Properties.UsingAutoCompleteBox = false;
		this.txtTakerId.Properties.UsingDataSource = true;
		this.txtTakerId.Properties.ValidateWhenNoDataSource = false;
		this.txtTakerId.Properties.ValueMember = "";
		this.txtTakerId.SettingInVisible = false;
		this.txtTakerId.SettingReadonly = false;
		this.txtTakerId.Size = new System.Drawing.Size(118, 20);
		this.txtTakerId.TabIndex = 4;
		this.txtBinLocation.EditControl = null;
		this.txtBinLocation.FieldName = "BinCode";
		this.txtBinLocation.IsSearchControl = false;
		this.txtBinLocation.Location = new System.Drawing.Point(249, 60);
		this.txtBinLocation.Name = "txtBinLocation";
		this.txtBinLocation.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions7.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.txtBinLocation.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance10, appearanceHovered10, appearancePressed10, appearanceDisabled10, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, false, true, editorButtonImageOptions7, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance11, appearanceHovered11, appearancePressed11, appearanceDisabled11, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txtBinLocation.Properties.ContainerControl = null;
		this.txtBinLocation.Properties.DisplayMember = "";
		this.txtBinLocation.Properties.DisplayMember2 = "";
		this.txtBinLocation.Properties.LoadDataSourceWhenEnter = false;
		this.txtBinLocation.Properties.SeparateCharacterForMulti = ';';
		this.txtBinLocation.Properties.SeparateString = ", ";
		this.txtBinLocation.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.GetDataBin;
		this.txtBinLocation.Properties.UsingAutoCompleteBox = false;
		this.txtBinLocation.Properties.UsingDataSource = true;
		this.txtBinLocation.Properties.ValidateWhenNoDataSource = false;
		this.txtBinLocation.Properties.ValueMember = "";
		this.txtBinLocation.SettingInVisible = false;
		this.txtBinLocation.SettingReadonly = false;
		this.txtBinLocation.Size = new System.Drawing.Size(118, 20);
		this.txtBinLocation.TabIndex = 21;
		this.contextMenuStripDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem1, this.deleteToolStripMenuItem });
		this.contextMenuStripDoc.Name = "contextMenuStripDoc";
		this.contextMenuStripDoc.Size = new System.Drawing.Size(124, 48);
		this.contextMenuStripDoc.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripDoc_Opening);
		this.addNewToolStripMenuItem1.Name = "addNewToolStripMenuItem1";
		this.addNewToolStripMenuItem1.Size = new System.Drawing.Size(123, 22);
		this.addNewToolStripMenuItem1.Text = "Add New";
		this.addNewToolStripMenuItem1.Click += new System.EventHandler(this.addNewToolStripMenuItem1_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.deleteToolStripMenuItem.Text = "Delete";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1266, 536);
		this.ContextMenuStrip = this.contextMenuStripDoc;
		base.Controls.Add(this.txtBinLocation);
		base.Controls.Add(this.txtTakerId);
		base.Controls.Add(this.txtWhsCode);
		base.Controls.Add(this.txtSameBatch);
		base.Controls.Add(this.btnAdd);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.txtRemarks);
		base.Controls.Add(this.txtStatus);
		base.Controls.Add(this.txtDocEntry);
		base.Controls.Add(this.txtBarcode);
		base.Controls.Add(this.txtTakerType);
		base.Controls.Add(this.txtTime);
		base.Controls.Add(this.txtCountDate);
		base.Controls.Add(this.lbRemark);
		base.Controls.Add(this.lbStatus);
		base.Controls.Add(this.lbDocNo);
		base.Controls.Add(this.lbBarcode);
		base.Controls.Add(this.lbWarehouseBin);
		base.Controls.Add(this.lbInventoryCounter);
		base.Controls.Add(this.lbTime);
		base.Controls.Add(this.lbCountDate);
		base.FormId = "oBarcodeCounting";
		base.FormObjectType = Apzon.Commons.ApzObjectTypes.oBarcodeInventoryCounting;
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_BarcodeInventoryCounting";
		this.Text = "Barcode Inventory Counting";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_BarcodeInventoryCounting_Load);
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_BarcodeInventoryCounting_OnControlValueChanged);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_BarcodeInventoryCounting_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.txtCountDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtCountDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTime.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTakerType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBarcode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDocEntry.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtRemarks.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStripLine.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDistNumber).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rInventoryUom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUomGroup).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUomCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSameBatch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtWhsCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtTakerId.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtBinLocation.Properties).EndInit();
		this.contextMenuStripDoc.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
