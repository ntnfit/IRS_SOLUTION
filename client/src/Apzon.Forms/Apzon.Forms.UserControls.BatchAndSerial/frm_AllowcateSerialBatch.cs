using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.UserControls.BatchAndSerial;

public class frm_AllowcateSerialBatch : XtraFormEx
{
	private IContainer components;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx gcItemCode;

	private GridColumnEx gcDistNumber;

	private GridColumnEx gcMnfSerial;

	private GridColumnEx gcLotNumber;

	private GridColumnEx gcQty;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnOk;

	private RepositoryItemButtonEdit rItemCode;

	private RepositoryItemCalcEdit rCal;

	private RepositoryItemButtonEdit rBinCode;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grdLines;

	private GridViewEx gvLines;

	private GridColumnEx gcBinCode;

	private GridColumnEx gcLogQty;

	private RepositoryItemCalcEdit rLineCal;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	public DataTable DataBatchSerial { get; set; }

	public DataTable DataBin { get; set; }

	public string WhsCode { get; set; }

	public bool IsCanChangeValue { get; set; }

	public string TranType { get; set; }

	public frm_AllowcateSerialBatch(DataTable dataBatchSerial, DataTable dataBin, bool isCanChangeValue, string whsCode, string tranType, string formId)
	{
		if (!dataBatchSerial.IsNotNull())
		{
			throw new ArgumentNullException("dataBatchSerial");
		}
		if (dataBin == null || dataBin.Columns.Count == 0)
		{
			throw new ArgumentNullException("dataBin");
		}
		if (string.IsNullOrEmpty(whsCode))
		{
			throw new ArgumentNullException("whsCode");
		}
		if (string.IsNullOrEmpty(tranType))
		{
			throw new ArgumentNullException("tranType");
		}
		DataBatchSerial = dataBatchSerial;
		DataBin = dataBin;
		IsCanChangeValue = isCanChangeValue;
		WhsCode = whsCode;
		TranType = tranType;
		InitializeComponent();
		FormId = (gvDoc.FormId = (gvLines.FormId = "AllocateBin_" + formId));
	}

	private void frm_AllowcateSerialBatch_Load(object sender, EventArgs e)
	{
		try
		{
			if (TranType.Equals("B"))
			{
				gcDistNumber.Caption = "Batch Number";
				gcMnfSerial.Caption = "Batch Attribute 1";
				gcLotNumber.Caption = "Batch Attribute 2";
			}
			else
			{
				gcDistNumber.Caption = "Serial Number";
				gcMnfSerial.Caption = "Manufacturer Serial No.";
				gcLotNumber.Caption = "Lot Number";
				gcLogQty.OptionsColumn.ReadOnly = true;
			}
			gvLines.ReadOnly = !IsCanChangeValue;
			rBinCode.Buttons[0].Visible = IsCanChangeValue;
			deleteRowToolStripMenuItem.Visible = IsCanChangeValue;
			DataTable dt = new DataTable();
			dt.Columns.Add("ItemCode", typeof(string));
			dt.Columns.Add("DistNumber", typeof(string));
			dt.Columns.Add("MnfSerial", typeof(string));
			dt.Columns.Add("LotNumber", typeof(string));
			dt.Columns.Add("LogQty", typeof(double));
			dt.Columns.Add("AbsEntry", typeof(int));
			dt.Columns.Add("SysNumber", typeof(int));
			dt.Columns.Add("BaseLineNumber", typeof(int));
			dt.Columns.Add("ManBatchSerial", typeof(string));
			dt.Columns.Add("ObjType", typeof(string));
			DataTable dataSource = (from row in DataBatchSerial.AsEnumerable()
				group row by new
				{
					ItemCode = Function.ToString(row["ItemCode"]),
					DistNumber = Function.ToString(row["DistNumber"]),
					MnfSerial = Function.ToString(row["MnfSerial"]),
					LotNumber = Function.ToString(row["LotNumber"]),
					AbsEntry = Function.ParseInt(row["AbsEntry"]),
					SysNumber = Function.ParseInt(row["SysNumber"]),
					BaseLineNumber = Function.ParseInt(row["BaseLineNumber"]),
					ManBatchSerial = Function.ToString(row["ManBatchSerial"]),
					ObjType = Function.ToString(row["ObjType"])
				} into g
				select dt.LoadDataRow(new object[10]
				{
					g.Key.ItemCode,
					g.Key.DistNumber,
					g.Key.MnfSerial,
					g.Key.LotNumber,
					Function.ParseDouble(g.Sum((DataRow r) => Function.ParseDouble(r["LogQty"]))),
					g.Key.AbsEntry,
					g.Key.SysNumber,
					g.Key.BaseLineNumber,
					g.Key.ManBatchSerial,
					g.Key.ObjType
				}, fAcceptChanges: false)).CopyToDataTable();
			grdDoc.DataSource = dataSource;
			grdDoc.DataSourceTable.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.Cancel;
		Close();
	}

	private void btnOk_Click(object sender, EventArgs e)
	{
		try
		{
			grdLines.DataSourceTable.AcceptChanges();
			DataRow[] array = grdLines.DataSourceTable.Select("Isnull(BinCode, '') <> ''");
			if (array.Length == 0)
			{
				DataBin.AcceptChanges();
				DialogResult = DialogResult.OK;
				Close();
				return;
			}
			DataTable dataTable = array.CopyToDataTable();
			if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => !string.IsNullOrEmpty(Function.ToString(t["BinCode"])) && Function.ParseDouble(t["LogQty"]) == 0.0) != null)
			{
				WinCommons.Error(Enums.MessageCode.AllocateBinLocationGreater0, "");
				return;
			}
			double num = Function.ParseDouble(dataTable.Compute("SUM(LogQty)", ""));
			if (Function.ParseDouble(gvDoc.GetFocusedRowCellValue(gcQty)) < num)
			{
				WinCommons.Error(Enums.MessageCode.AllocateQtyGreaterThanBatchQty, "");
				return;
			}
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				DataBin.ImportRow(array[num2]);
			}
			DataBin.AcceptChanges();
			DialogResult = DialogResult.OK;
			Close();
			return;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		DialogResult = DialogResult.Cancel;
		Close();
	}

	private void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			DataTable dataTable = grdLines.DataSourceTable.Copy();
			DataRow[] array = DataBin.Select(string.Format("AbsEntry = {0}", Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsEntry"))));
			grdLines.DataSource = ((array.Length == 0) ? DataBin.Clone() : array.CopyToDataTable());
			grdLines.DataSourceTable.AcceptChanges();
			for (int i = 0; i < array.Length; i++)
			{
				DataBin.Rows.Remove(array[i]);
			}
			DataBin.AcceptChanges();
			if (dataTable.IsNotNull())
			{
				DataRow[] array2 = dataTable.Select("isnull(BinCode,'') <> ''");
				for (int j = 0; j < array2.Length; j++)
				{
					DataBin.ImportRow(array2[j]);
				}
				DataBin.AcceptChanges();
			}
			AddNewRowLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRowLine()
	{
		try
		{
			if (!IsCanChangeValue)
			{
				return;
			}
			grdLines.DataSourceTable.AcceptChanges();
			DataTable dataSourceTable = grdLines.DataSourceTable;
			if (dataSourceTable == null || dataSourceTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CouldNotGetDataBinAllocate, "");
				return;
			}
			DataRow[] array = dataSourceTable.Select("isnull(BinCode,'') = ''");
			if (array.Length != 0)
			{
				for (int i = 0; i < array.Length - 1; i++)
				{
					dataSourceTable.Rows.Remove(array[i]);
				}
				dataSourceTable.AcceptChanges();
				grdLines.DataSource = dataSourceTable;
				return;
			}
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["ItemCode"] = Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode));
			dataRow["DistNumber"] = Function.ToString(gvDoc.GetFocusedRowCellValue("DistNumber"));
			dataRow["MnfSerial"] = Function.ToString(gvDoc.GetFocusedRowCellValue("MnfSerial"));
			dataRow["LotNumber"] = Function.ToString(gvDoc.GetFocusedRowCellValue("LotNumber"));
			dataRow["LogQty"] = 0;
			dataRow["AbsEntry"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsEntry"));
			dataRow["SysNumber"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("SysNumber"));
			dataRow["BaseLineNumber"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("BaseLineNumber"));
			dataRow["ManBatchSerial"] = Function.ToString(gvDoc.GetFocusedRowCellValue("ManBatchSerial"));
			dataRow["ObjType"] = Function.ToString(gvDoc.GetFocusedRowCellValue("ObjType"));
			dataRow["IsExits"] = "Y";
			dataRow["TranType"] = 2;
			dataRow["BinAbs"] = 0;
			dataRow["BinCode"] = "";
			dataSourceTable.Rows.Add(dataRow);
			grdLines.DataSource = dataSourceTable;
			grdLines.DataSourceTable.AcceptChanges();
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
			this.DrillDown(ApzObjectTypes.oItems, ApzObjectTypes.oItems, Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode)));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rBinCode_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index != 0)
			{
				return;
			}
			int batchSerialAbs = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsEntry"));
			string itemCode = Function.ToString(gvDoc.GetFocusedRowCellValue("ItemCode"));
			string txtText = Function.ToString(gvLines.GetFocusedRowCellValue("BinCode"));
			frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.Bin, TranType.Equals("B"), txtText)
			{
				WhsCode = WhsCode,
				ItemCode = itemCode,
				BatchSerialAbs = batchSerialAbs,
				BatchSerialTrantype = 0
			};
			frm_ChooseFromList2.ShowDialog(FindForm());
			if (TranType.Equals("S"))
			{
				if (frm_ChooseFromList2.ChooseItem != null)
				{
					DataRow focusedDataRow = gvLines.GetFocusedDataRow();
					focusedDataRow["BinAbs"] = (object)frm_ChooseFromList2.ChooseItem["AbsEntry"];
					focusedDataRow["BinCode"] = (object)frm_ChooseFromList2.ChooseItem["BinCode"];
					focusedDataRow["ItemCode"] = Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode));
					focusedDataRow["DistNumber"] = Function.ToString(gvDoc.GetFocusedRowCellValue("DistNumber"));
					focusedDataRow["MnfSerial"] = Function.ToString(gvDoc.GetFocusedRowCellValue("MnfSerial"));
					focusedDataRow["LotNumber"] = Function.ToString(gvDoc.GetFocusedRowCellValue("LotNumber"));
					focusedDataRow["LogQty"] = 1;
					focusedDataRow["AbsEntry"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsEntry"));
					focusedDataRow["SysNumber"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("SysNumber"));
					focusedDataRow["BaseLineNumber"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("BaseLineNumber"));
					focusedDataRow["ManBatchSerial"] = Function.ToString(gvDoc.GetFocusedRowCellValue("ManBatchSerial"));
					focusedDataRow["ObjType"] = Function.ToString(gvDoc.GetFocusedRowCellValue("ObjType"));
					focusedDataRow["IsExits"] = "Y";
					focusedDataRow["TranType"] = 2;
					grdLines.DataSourceTable.AcceptChanges();
					gvLines.FocusedColumn = gcLogQty;
				}
				AddNewRowLine();
			}
			else
			{
				if (frm_ChooseFromList2.ChooseItems == null || frm_ChooseFromList2.ChooseItems.Count <= 0)
				{
					return;
				}
				DataTable dataTable = grdLines.DataSourceTable();
				DataRow focusedDataRow2 = gvLines.GetFocusedDataRow();
				int focusedDataSourceRowIndex = gvLines.GetFocusedDataSourceRowIndex();
				for (int i = 0; i < frm_ChooseFromList2.ChooseItems.Count; i++)
				{
					DataRowView dataRowView = (DataRowView)frm_ChooseFromList2.ChooseItems[i];
					int num = Function.ParseInt(dataRowView["AbsEntry"]);
					string value = Function.ToString(dataRowView["BinCode"]);
					double num2 = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("LogQty")) - Function.ParseDouble(dataTable.Compute("Sum(LogQty)", ""));
					if (i == 0)
					{
						num2 += Function.ParseDouble(focusedDataRow2["LogQty"]);
						focusedDataRow2["BinAbs"] = num;
						focusedDataRow2["BinCode"] = value;
						focusedDataRow2["ItemCode"] = Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode));
						focusedDataRow2["DistNumber"] = Function.ToString(gvDoc.GetFocusedRowCellValue("DistNumber"));
						focusedDataRow2["MnfSerial"] = Function.ToString(gvDoc.GetFocusedRowCellValue("MnfSerial"));
						focusedDataRow2["LotNumber"] = Function.ToString(gvDoc.GetFocusedRowCellValue("LotNumber"));
						focusedDataRow2["LogQty"] = ((num2 >= 0.0) ? num2 : 0.0);
						focusedDataRow2["AbsEntry"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsEntry"));
						focusedDataRow2["SysNumber"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("SysNumber"));
						focusedDataRow2["BaseLineNumber"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("BaseLineNumber"));
						focusedDataRow2["ManBatchSerial"] = Function.ToString(gvDoc.GetFocusedRowCellValue("ManBatchSerial"));
						focusedDataRow2["ObjType"] = Function.ToString(gvDoc.GetFocusedRowCellValue("ObjType"));
						focusedDataRow2["IsExits"] = "Y";
						focusedDataRow2["TranType"] = 2;
					}
					else
					{
						DataRow dataRow = dataTable.NewRow();
						dataRow.ItemArray = focusedDataRow2.ItemArray;
						dataRow["BinAbs"] = num;
						dataRow["BinCode"] = value;
						dataRow["ItemCode"] = Function.ToString(gvDoc.GetFocusedRowCellValue(gcItemCode));
						dataRow["DistNumber"] = Function.ToString(gvDoc.GetFocusedRowCellValue("DistNumber"));
						dataRow["MnfSerial"] = Function.ToString(gvDoc.GetFocusedRowCellValue("MnfSerial"));
						dataRow["LotNumber"] = Function.ToString(gvDoc.GetFocusedRowCellValue("LotNumber"));
						dataRow["LogQty"] = ((num2 >= 0.0) ? num2 : 0.0);
						dataRow["AbsEntry"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsEntry"));
						dataRow["SysNumber"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("SysNumber"));
						dataRow["BaseLineNumber"] = Function.ParseInt(gvDoc.GetFocusedRowCellValue("BaseLineNumber"));
						dataRow["ManBatchSerial"] = Function.ToString(gvDoc.GetFocusedRowCellValue("ManBatchSerial"));
						dataRow["ObjType"] = Function.ToString(gvDoc.GetFocusedRowCellValue("ObjType"));
						dataRow["IsExits"] = "Y";
						dataRow["TranType"] = 2;
						dataTable.Rows.InsertAt(dataRow, focusedDataSourceRowIndex + 1);
					}
				}
				grdLines.DataSource = dataTable;
				grdLines.DataSourceTable.AcceptChanges();
				gvLines.FocusedColumn = gcLogQty;
				AddNewRowLine();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gvLines_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName.Equals(gcBinCode.FieldName))
			{
				DataTable dt = grdLines.DataSourceTable;
				int rowIndex = gvLines.GetDataSourceRowIndex(e.RowHandle);
				if (string.IsNullOrEmpty(Function.ToString(e.Value)))
				{
					dt.Rows[rowIndex]["BinCode"] = "";
					dt.Rows[rowIndex]["BinAbs"] = 0;
					AddNewRowLine();
					return;
				}
				if (Function.ParseInt(dt.Compute("count(1)", $"BinCode = {Function.ToString(e.Value)}")) > 1)
				{
					WinCommons.Error(Enums.MessageCode.BinLocationExistsOnGrid, "");
					dt.Rows.RemoveAt(rowIndex);
					AddNewRowLine();
					return;
				}
				int batSerialAbs = Function.ParseInt(gvDoc.GetFocusedRowCellValue("AbsEntry"));
				string itemCode = Function.ToString(gvDoc.GetFocusedRowCellValue("ItemCode"));
				string binCode = Function.ToString(gvLines.GetFocusedRowCellValue("BinCode"));
				if (!(await HttpClientHelperFactory.GetHttpClient().InventoryCountingBin.LoadAllBin(itemCode, WhsCode, batSerialAbs, 0, binCode)).IsNotNull())
				{
					dt.Rows[rowIndex]["BinCode"] = "";
					dt.Rows[rowIndex]["BinAbs"] = 0;
					WinCommons.Error(Enums.MessageCode.BinDoesNotExists, "");
				}
				else
				{
					double num = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("LogQty")) - Function.ParseDouble(dt.Compute("Sum(LogQty)", "")) + Function.ParseDouble(gvLines.GetFocusedRowCellValue(gcLogQty));
					dt.Rows[rowIndex]["BinCode"] = dt.Rows[0]["BinCode"];
					dt.Rows[rowIndex]["BinAbs"] = dt.Rows[0]["AbsEntry"];
					dt.Rows[rowIndex]["LogQty"] = ((!TranType.Equals("B")) ? 1.0 : ((num >= 0.0) ? num : 0.0));
				}
				dt.AcceptChanges();
			}
			AddNewRowLine();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvLines_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (TranType.Equals("S") && gvLines.FocusedColumn.FieldName.Equals(gcBinCode.FieldName) && string.IsNullOrEmpty(Function.ToString(gvLines.GetFocusedRowCellValue(gcBinCode))) && grdLines.DataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => !string.IsNullOrEmpty(Function.ToString(t[gcBinCode.FieldName]))) != null)
			{
				WinCommons.Error(Enums.MessageCode.ASerialNumberOnlyAllocate2ABin, "");
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			grdLines.DataSourceTable.AcceptChanges();
			DataRow[] array = grdLines.DataSourceTable.Select("Isnull(BinCode, '') <> ''");
			if (array.Length == 0)
			{
				return;
			}
			DataTable dataTable = array.CopyToDataTable();
			if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => !string.IsNullOrEmpty(Function.ToString(t["BinCode"])) && Function.ParseDouble(t["LogQty"]) == 0.0) != null)
			{
				WinCommons.Error(Enums.MessageCode.AllocateBinLocationGreater0, "");
				e.Allow = false;
				return;
			}
			double num = Function.ParseDouble(dataTable.Compute("SUM(LogQty)", "isnull(BinCode, '') <> ''"));
			if (Function.ParseDouble(gvDoc.GetFocusedRowCellValue(gcQty)) < num)
			{
				WinCommons.Error(Enums.MessageCode.AllocateQtyGreaterThanBatchQty, "");
				e.Allow = false;
				return;
			}
			var array2 = (from row in dataTable.AsEnumerable()
				group row by new
				{
					BinCode = Function.ToString(row["BinCode"])
				} into g
				where g.Count() > 1
				select new { g.Key.BinCode }).ToArray();
			if (array2.Any())
			{
				WinCommons.Error(Enums.MessageCode.DuplicateRowWithBinCode, "", array2[0].BinCode);
				e.Allow = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			gvLines.DeleteSelectedRows();
			grdLines.DataSourceTable.AcceptChanges();
			AddNewRowLine();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.UserControls.BatchAndSerial.frm_AllowcateSerialBatch));
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gcDistNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.gcMnfSerial = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLotNumber = new Apzon.Controls.Grid.GridColumnEx();
		this.gcQty = new Apzon.Controls.Grid.GridColumnEx();
		this.rCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.rBinCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdLines = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gvLines = new Apzon.Controls.Grid.GridViewEx();
		this.gcBinCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLogQty = new Apzon.Controls.Grid.GridColumnEx();
		this.rLineCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rBinCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdLines).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvLines).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rLineCal).BeginInit();
		base.SuspendLayout();
		this.grdDoc.AllowPasteExcel = false;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rItemCode, this.rCal });
		this.grdDoc.Size = new System.Drawing.Size(515, 248);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gcItemCode, this.gcDistNumber, this.gcMnfSerial, this.gcLotNumber, this.gcQty });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oAllocateSerialBatchToWhs";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "FromData";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.ReadOnly = true;
		this.gvDoc.OptionsView.BestFitMaxRowCount = 100;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gvDoc.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gvDoc_BeforeLeaveRow);
		this.gcItemCode.Caption = "ItemCode";
		this.gcItemCode.ColumnEdit = this.rItemCode;
		this.gcItemCode.CurrencyDateField = null;
		this.gcItemCode.CurrencyField = null;
		this.gcItemCode.CurrencyFixed = null;
		this.gcItemCode.CurrencyFixedRate = 0.0;
		this.gcItemCode.CurrencyRateField = null;
		this.gcItemCode.FieldName = "ItemCode";
		this.gcItemCode.Name = "gcItemCode";
		this.gcItemCode.OptionsColumn.ReadOnly = true;
		this.gcItemCode.PrimaryKey = false;
		this.gcItemCode.UnitField = null;
		this.gcItemCode.Visible = true;
		this.gcItemCode.VisibleIndex = 0;
		this.gcItemCode.Width = 104;
		this.rItemCode.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rItemCode.Name = "rItemCode";
		this.rItemCode.ReadOnly = true;
		this.rItemCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rItemCode_ButtonClick);
		this.gcDistNumber.Caption = "Batch Number";
		this.gcDistNumber.CurrencyDateField = null;
		this.gcDistNumber.CurrencyField = null;
		this.gcDistNumber.CurrencyFixed = null;
		this.gcDistNumber.CurrencyFixedRate = 0.0;
		this.gcDistNumber.CurrencyRateField = null;
		this.gcDistNumber.FieldName = "DistNumber";
		this.gcDistNumber.Name = "gcDistNumber";
		this.gcDistNumber.OptionsColumn.ReadOnly = true;
		this.gcDistNumber.PrimaryKey = false;
		this.gcDistNumber.UnitField = null;
		this.gcDistNumber.Visible = true;
		this.gcDistNumber.VisibleIndex = 1;
		this.gcDistNumber.Width = 91;
		this.gcMnfSerial.Caption = "Batch Attribute 1";
		this.gcMnfSerial.CurrencyDateField = null;
		this.gcMnfSerial.CurrencyField = null;
		this.gcMnfSerial.CurrencyFixed = null;
		this.gcMnfSerial.CurrencyFixedRate = 0.0;
		this.gcMnfSerial.CurrencyRateField = null;
		this.gcMnfSerial.FieldName = "MnfSerial";
		this.gcMnfSerial.Name = "gcMnfSerial";
		this.gcMnfSerial.OptionsColumn.ReadOnly = true;
		this.gcMnfSerial.PrimaryKey = false;
		this.gcMnfSerial.UnitField = null;
		this.gcMnfSerial.Visible = true;
		this.gcMnfSerial.VisibleIndex = 2;
		this.gcMnfSerial.Width = 102;
		this.gcLotNumber.Caption = "Batch Attribute 2";
		this.gcLotNumber.CurrencyDateField = null;
		this.gcLotNumber.CurrencyField = null;
		this.gcLotNumber.CurrencyFixed = null;
		this.gcLotNumber.CurrencyFixedRate = 0.0;
		this.gcLotNumber.CurrencyRateField = null;
		this.gcLotNumber.FieldName = "LotNumber";
		this.gcLotNumber.Name = "gcLotNumber";
		this.gcLotNumber.OptionsColumn.ReadOnly = true;
		this.gcLotNumber.PrimaryKey = false;
		this.gcLotNumber.UnitField = null;
		this.gcLotNumber.Visible = true;
		this.gcLotNumber.VisibleIndex = 3;
		this.gcLotNumber.Width = 105;
		this.gcQty.Caption = "Qty";
		this.gcQty.ColumnEdit = this.rCal;
		this.gcQty.CurrencyDateField = null;
		this.gcQty.CurrencyField = null;
		this.gcQty.CurrencyFixed = null;
		this.gcQty.CurrencyFixedRate = 0.0;
		this.gcQty.CurrencyRateField = null;
		this.gcQty.FieldName = "LogQty";
		this.gcQty.Name = "gcQty";
		this.gcQty.PrimaryKey = false;
		this.gcQty.UnitField = null;
		this.gcQty.Visible = true;
		this.gcQty.VisibleIndex = 4;
		this.rCal.AutoHeight = false;
		this.rCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCal.Name = "rCal";
		this.rBinCode.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.right;
		this.rBinCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rBinCode.Name = "rBinCode";
		this.rBinCode.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rBinCode_ButtonClick);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(762, 261);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 20);
		this.btnCancel.TabIndex = 1;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnOk.EditControl = null;
		this.btnOk.EditValue = "Ok";
		this.btnOk.FieldName = null;
		this.btnOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOk.ImageOptions.Image");
		this.btnOk.IsSearchControl = false;
		this.btnOk.Location = new System.Drawing.Point(683, 261);
		this.btnOk.Name = "btnOk";
		this.btnOk.ReadOnly = false;
		this.btnOk.SettingInVisible = false;
		this.btnOk.SettingReadonly = false;
		this.btnOk.Size = new System.Drawing.Size(75, 20);
		this.btnOk.TabIndex = 2;
		this.btnOk.Text = "Ok";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(3, 3);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grdDoc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grdLines);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(834, 248);
		this.splitContainerControl1.SplitterPosition = 515;
		this.splitContainerControl1.TabIndex = 3;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.grdLines.AllowPasteExcel = false;
		this.grdLines.ClipboardData = null;
		this.grdLines.ContextMenuStrip = this.contextMenuStrip1;
		this.grdLines.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdLines.Location = new System.Drawing.Point(0, 0);
		this.grdLines.MainView = this.gvLines;
		this.grdLines.Name = "grdLines";
		this.grdLines.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rLineCal, this.rBinCode });
		this.grdLines.Size = new System.Drawing.Size(309, 248);
		this.grdLines.TabIndex = 0;
		this.grdLines.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvLines });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 26);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gvLines.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvLines.ColumnKey = null;
		this.gvLines.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gcBinCode, this.gcLogQty });
		this.gvLines.DisableFormSetting = false;
		this.gvLines.DisplayException = false;
		this.gvLines.EditColumn = null;
		this.gvLines.FieldCurrency = null;
		this.gvLines.FieldRate = null;
		this.gvLines.FormId = "oAllocateSerialBatchToWhs";
		this.gvLines.GridControl = this.grdLines;
		this.gvLines.IgnoreFilter = false;
		this.gvLines.IndexEditRow = -1;
		this.gvLines.IndicatorWidth = 35;
		this.gvLines.IsApzTable = false;
		this.gvLines.IsInsertNewRow = false;
		this.gvLines.IsLoadingData = false;
		this.gvLines.IsMappingContextAction = false;
		this.gvLines.ItemId = "AllowcateBinData";
		this.gvLines.LstColumnAllowBatch = null;
		this.gvLines.LstColumnNotAllowBatch = null;
		this.gvLines.LstColumnNotEditable = null;
		this.gvLines.LstColumNotSetting = null;
		this.gvLines.Name = "gvLines";
		this.gvLines.OptionsView.BestFitMaxRowCount = 100;
		this.gvLines.OptionsView.ColumnAutoWidth = false;
		this.gvLines.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvLines.OptionsView.ShowGroupPanel = false;
		this.gvLines.UseBatchMod = false;
		this.gvLines.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvLines_ShowingEditor);
		this.gvLines.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvLines_CellValueChanged);
		this.gcBinCode.Caption = "Bin Location";
		this.gcBinCode.ColumnEdit = this.rBinCode;
		this.gcBinCode.CurrencyDateField = null;
		this.gcBinCode.CurrencyField = null;
		this.gcBinCode.CurrencyFixed = null;
		this.gcBinCode.CurrencyFixedRate = 0.0;
		this.gcBinCode.CurrencyRateField = null;
		this.gcBinCode.FieldName = "BinCode";
		this.gcBinCode.Name = "gcBinCode";
		this.gcBinCode.PrimaryKey = false;
		this.gcBinCode.UnitField = null;
		this.gcBinCode.Visible = true;
		this.gcBinCode.VisibleIndex = 0;
		this.gcBinCode.Width = 153;
		this.gcLogQty.Caption = "Quantity";
		this.gcLogQty.ColumnEdit = this.rLineCal;
		this.gcLogQty.CurrencyDateField = null;
		this.gcLogQty.CurrencyField = null;
		this.gcLogQty.CurrencyFixed = null;
		this.gcLogQty.CurrencyFixedRate = 0.0;
		this.gcLogQty.CurrencyRateField = null;
		this.gcLogQty.FieldName = "LogQty";
		this.gcLogQty.Name = "gcLogQty";
		this.gcLogQty.PrimaryKey = false;
		this.gcLogQty.UnitField = null;
		this.gcLogQty.Visible = true;
		this.gcLogQty.VisibleIndex = 1;
		this.rLineCal.AutoHeight = false;
		this.rLineCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rLineCal.Name = "rLineCal";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(841, 285);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btnOk);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oAllocateSerialBatchToWhs";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AllowcateSerialBatch";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
		this.Text = "Allocate Serial/Batch to Whse";
		base.Load += new System.EventHandler(this.frm_AllowcateSerialBatch_Load);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rItemCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rBinCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdLines).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvLines).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rLineCal).EndInit();
		base.ResumeLayout(false);
	}
}
