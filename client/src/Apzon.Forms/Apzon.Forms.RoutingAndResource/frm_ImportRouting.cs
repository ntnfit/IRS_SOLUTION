using System;
using System.Collections.Generic;
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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.UserControls;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace Apzon.Forms.RoutingAndResource;

public class frm_ImportRouting : XtraFormEx
{
	private DataTable _dataDoc;

	private DataTable _dataLine;

	private DataTable _dataEffic;

	private IContainer components;

	private SplitContainerControl splitContainerControlDoc;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnSave;

	private SimpleButtonEx btnChooseFile;

	private LabelLine lbFile;

	private TextEditEx txtFile;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridControlEx grdLine;

	private GridViewEx gvLine;

	private GridColumnEx gcRoutCode;

	private GridColumnEx gcRoutName;

	private GridColumnEx gcRoutFName;

	private GridColumnEx gcProdCode;

	private GridColumnEx gcProdName;

	private GridColumnEx Status;

	private RepositoryItemLookUpEdit rStatus;

	private GridColumnEx gcFromDate;

	private RepositoryItemDateEditEx rDate;

	private GridColumnEx gcToDate;

	private GridColumnEx gcRemarks;

	private GridColumnEx gcIsBase;

	private RepositoryItemLookUpEdit rIsBase;

	private GridColumnEx gcLineRoutCode;

	private GridColumnEx gcLineWorkOrder;

	private RepositoryItemLookUpEdit rWorkOrder;

	private GridColumnEx gcLineOpCode;

	private GridColumnEx gcLineOpName;

	private GridColumnEx gcLineResCode;

	private GridColumnEx gcLineResName;

	private GridColumnEx gcLineSetTime;

	private GridColumnEx gcLineUomSetTime;

	private RepositoryItemLookUpEdit rUom;

	private GridColumnEx gcLineRunTime;

	private GridColumnEx gcLineUomRunTime;

	private GridColumnEx gcLineTransTime;

	private GridColumnEx gcLineUomTransTime;

	private GridColumnEx gcLineBaseQty;

	private RepositoryItemCalcEdit rCal;

	private GridColumnEx gcLineUomCode;

	private GridColumnEx gcLineProdCode;

	private GridColumnEx gcLineProdName;

	private GridColumnEx gcLineVisOrder;

	private GridColumnEx gcLineSubRoutCode;

	private SplitContainerControl splitContainerLines;

	private GridControlEx grdEffic;

	private GridViewEx gvEffic;

	private GridColumnEx gcEfficResCode;

	private RepositoryItemButtonEditCFL rEfficResCode;

	private GridColumnEx gcEfficResName;

	private GridColumnEx gcEfficEfficiency;

	private RepositoryItemCalcEdit rEfficCal;

	public frm_ImportRouting()
	{
		InitializeComponent();
	}

	private void btnChooseFile_Click(object sender, EventArgs e)
	{
		OpenFileDialog openFileDialog = new OpenFileDialog
		{
			Multiselect = false
		};
		if (openFileDialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(openFileDialog.FileName))
		{
			txtFile.EditValue = openFileDialog.FileName;
		}
	}

	private void txtFile_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (_dataDoc == null || _dataDoc.Columns.Count == 0 || _dataLine == null || _dataLine.Columns.Count == 0 || _dataEffic == null || _dataEffic.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetStructureToImportData, "");
				return;
			}
			_dataDoc.Rows.Clear();
			_dataLine.Rows.Clear();
			_dataEffic.Rows.Clear();
			if (string.IsNullOrEmpty(txtFile.Text))
			{
				grdDoc.DataSourceTable.Rows.Clear();
				grdLine.DataSourceTable.Rows.Clear();
				grdEffic.DataSourceTable.Rows.Clear();
				return;
			}
			using (SpreadsheetDocument spreadsheetDocument = SpreadsheetDocument.Open(txtFile.Text, isEditable: false))
			{
				WorkbookPart workbookPart = spreadsheetDocument.WorkbookPart;
				Sheet[] array = workbookPart.Workbook.Descendants<Sheet>().ToArray();
				if (array.Length < 3)
				{
					WinCommons.Error(Enums.MessageCode.FileImportDoNotEnoughData, "");
					return;
				}
				WinCommons._OpenProcessing();
				Row[] array2 = ((WorksheetPart)workbookPart.GetPartById(array[0].Id)).Worksheet.Descendants<Row>().ToArray();
				for (int i = 2; i < array2.Length; i++)
				{
					Cell[] array3 = array2[i].Descendants<Cell>().ToArray();
					string[] array4 = new string[9];
					for (int j = 0; j < array3.Length; j++)
					{
						int num = CellReferenceToIndex(array3.ElementAt(j));
						array4[num] = GetCellValue(spreadsheetDocument, array3.ElementAt(j));
					}
					if (string.IsNullOrEmpty(array4[0]))
					{
						break;
					}
					DataRow dataRow = _dataDoc.NewRow();
					dataRow["RoutCode"] = array4[0];
					dataRow["RoutName"] = array4[1];
					dataRow["RoutFName"] = array4[2];
					dataRow["ProdCode"] = array4[3];
					dataRow["ProdName"] = array4[4];
					dataRow["Status"] = array4[5];
					DateTime? dateTime = Function.ParseDateTime(array4[6], "yyyyMMdd");
					if (dateTime.HasValue)
					{
						dataRow["FromDate"] = dateTime;
					}
					DateTime? dateTime2 = Function.ParseDateTime(array4[7], "yyyyMMdd");
					if (dateTime2.HasValue)
					{
						dataRow["ToDate"] = dateTime2;
					}
					dataRow["Remarks"] = array4[8];
					dataRow["IsBase"] = "Y";
					_dataDoc.Rows.Add(dataRow);
				}
				_dataDoc.AcceptChanges();
				Row[] array5 = ((WorksheetPart)workbookPart.GetPartById(array[1].Id)).Worksheet.Descendants<Row>().ToArray();
				for (int k = 2; k < array5.Length; k++)
				{
					Cell[] array6 = array5[k].Descendants<Cell>().ToArray();
					string[] array7 = new string[18];
					for (int l = 0; l < array6.Length; l++)
					{
						int num2 = CellReferenceToIndex(array6.ElementAt(l));
						array7[num2] = GetCellValue(spreadsheetDocument, array6.ElementAt(l));
					}
					if (string.IsNullOrEmpty(array7[0]))
					{
						break;
					}
					DataRow dataRow2 = _dataLine.NewRow();
					dataRow2["RoutCode"] = array7[0];
					dataRow2["LineNum"] = Function.ParseInt(array7[1]);
					dataRow2["WorkOrder"] = array7[2];
					dataRow2["OpCode"] = array7[3];
					dataRow2["OpName"] = array7[4];
					dataRow2["ResCode"] = array7[5];
					dataRow2["ResName"] = array7[6];
					dataRow2["SetTime"] = Function.ParseDouble(array7[7]);
					dataRow2["UomSetTime"] = array7[8];
					dataRow2["RunTime"] = Function.ParseDouble(array7[9]);
					dataRow2["UomRunTime"] = array7[10];
					dataRow2["TransTime"] = Function.ParseDouble(array7[11]);
					dataRow2["UomTransTime"] = array7[12];
					dataRow2["BaseQty"] = Function.ParseDouble(array7[13]);
					dataRow2["ProdCode"] = array7[14];
					dataRow2["ProdName"] = array7[15];
					dataRow2["VisOrder"] = Function.ParseInt(array7[16]);
					dataRow2["SubRoutCode"] = array7[17];
					_dataLine.Rows.Add(dataRow2);
				}
				_dataLine.AcceptChanges();
				Row[] array8 = ((WorksheetPart)workbookPart.GetPartById(array[2].Id)).Worksheet.Descendants<Row>().ToArray();
				for (int m = 2; m < array8.Length; m++)
				{
					Cell[] array9 = array8[m].Descendants<Cell>().ToArray();
					string[] array10 = new string[6];
					for (int n = 0; n < array9.Length; n++)
					{
						int num3 = CellReferenceToIndex(array9.ElementAt(n));
						array10[num3] = GetCellValue(spreadsheetDocument, array9.ElementAt(n));
					}
					if (string.IsNullOrEmpty(array10[0]))
					{
						break;
					}
					DataRow dataRow3 = _dataEffic.NewRow();
					dataRow3["RoutCode"] = array10[0];
					dataRow3["FatherLine"] = Function.ParseInt(array10[1]);
					dataRow3["ResCode"] = array10[2];
					dataRow3["ResName"] = array10[3];
					dataRow3["Effic"] = array10[4];
					dataRow3["Remarks"] = array10[5];
					_dataEffic.Rows.Add(dataRow3);
				}
				_dataEffic.AcceptChanges();
				WinCommons._CloseProcessing();
			}
			if (!_dataDoc.IsNotNull() || !_dataLine.IsNotNull())
			{
				_dataDoc.Rows.Clear();
				_dataLine.Rows.Clear();
				return;
			}
			grdDoc.DataSource = _dataDoc;
			gvDoc.RefreshData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private int CellReferenceToIndex(Cell cell)
	{
		int num = 0;
		string text = cell.CellReference.ToString().ToUpper();
		foreach (char c in text)
		{
			if (char.IsLetter(c))
			{
				int num2 = c - 65;
				num = ((num == 0) ? num2 : ((num + 1) * 26 + num2));
				continue;
			}
			return num;
		}
		return num;
	}

	public string GetCellValue(SpreadsheetDocument document, Cell cell)
	{
		SharedStringTablePart sharedStringTablePart = document.WorkbookPart.SharedStringTablePart;
		if (cell.CellValue == null)
		{
			return "";
		}
		string innerXml = cell.CellValue.InnerXml;
		if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
		{
			return sharedStringTablePart.SharedStringTable.ChildElements[int.Parse(innerXml)].InnerText;
		}
		return innerXml;
	}

	private async void frm_ImportRouting_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			List<Common.Table_Code> dataSource = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Active",
					Code = "A"
				},
				new Common.Table_Code
				{
					Name = "Inactive",
					Code = "I"
				},
				new Common.Table_Code
				{
					Name = "",
					Code = ""
				}
			};
			rStatus.DataSource = dataSource;
			List<Common.Table_Code> dataSource2 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Name = "Min",
					Code = "M"
				},
				new Common.Table_Code
				{
					Name = "Hour",
					Code = "H"
				},
				new Common.Table_Code
				{
					Name = "Day",
					Code = "Day"
				}
			};
			rUom.DataSource = dataSource2;
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
			rWorkOrder.DataSource = dataSource3;
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("RoutCode");
			dataTable.Columns.Add("Status");
			dataTable.Columns.Add("FromDate");
			dataTable.Columns.Add("ToDate");
			DataRow dataRow = dataTable.NewRow();
			dataRow["RoutCode"] = "-1001";
			dataTable.Rows.Add(dataRow);
			DataTable dataTable2 = await HttpClient.AdvanceRoutingClient.GetDataDoc(new Tuple<DataTable, Enums.NavigateType>(dataTable, Enums.NavigateType.Current));
			if (dataTable2 != null)
			{
				_dataDoc = dataTable2.Clone();
				_dataDoc.TableName = "TableDoc";
			}
			DataSet dataSet = await HttpClient.AdvanceRoutingClient.GetDataLine("");
			if (dataSet != null && dataSet.Tables.Count == 2)
			{
				_dataLine = dataSet.Tables[0].Clone();
				_dataEffic = dataSet.Tables[1].Clone();
				_dataLine.TableName = "TableLine";
				_dataEffic.TableName = "TableEffic";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gvDoc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			EnumerableRowCollection<DataRow> source = from t in _dataLine.AsEnumerable()
				where Function.ToString(t["RoutCode"]).Equals(Function.ToString(gvDoc.GetFocusedRowCellValue("RoutCode")))
				select t;
			if (source.Any())
			{
				grdLine.DataSource = source.CopyToDataTable();
				gvLine.RefreshData();
			}
			else
			{
				grdLine.DataSourceTable.Rows.Clear();
				grdLine.DataSourceTable.AcceptChanges();
			}
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

	private async void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (!_dataDoc.IsNotNull() || !_dataLine.IsNotNull() || _dataEffic == null || _dataEffic.Columns.Count == 0)
			{
				WinCommons.Warning(Enums.MessageCode.DataImportFromFileDoesNotEnough, "");
				return;
			}
			WinCommons._OpenProcessing();
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(_dataDoc.Copy());
			dataSet.Tables.Add(_dataLine.Copy());
			dataSet.Tables.Add(_dataEffic.Copy());
			HttpResult httpResult = await HttpClient.AdvanceRoutingClient.ImportFromFile(dataSet);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.ImportSuccess, "");
				txtFile.EditValue = "";
				_dataDoc.Rows.Clear();
				_dataLine.Rows.Clear();
				_dataEffic.Rows.Clear();
				grdDoc.DataSourceTable.Rows.Clear();
				grdLine.DataSourceTable.Rows.Clear();
				grdEffic.DataSourceTable.Rows.Clear();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void gvLine_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			if (_dataEffic != null && _dataEffic.Columns.Count != 0)
			{
				grdEffic.DataSourceTable.Rows.Clear();
				int lineNum = Function.ParseInt(gvDoc.GetRowCellValue(e.FocusedRowHandle, "LineNum"));
				List<DataRow> source = (from t in _dataEffic.AsEnumerable()
					where Function.ParseInt(t["FatherLine"]) == lineNum
					select t).ToList();
				if (source.Any())
				{
					grdEffic.DataSource = source.CopyToDataTable();
				}
				else
				{
					grdEffic.DataSource = _dataEffic.Clone();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_ImportRouting_Resize(object sender, EventArgs e)
	{
		splitContainerLines.Panel2.Width = 300;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.RoutingAndResource.frm_ImportRouting));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainerControlDoc = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gcRoutCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRoutName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRoutFName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcProdCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcProdName = new Apzon.Controls.Grid.GridColumnEx();
		this.Status = new Apzon.Controls.Grid.GridColumnEx();
		this.rStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcFromDate = new Apzon.Controls.Grid.GridColumnEx();
		this.rDate = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.gcToDate = new Apzon.Controls.Grid.GridColumnEx();
		this.gcRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gcIsBase = new Apzon.Controls.Grid.GridColumnEx();
		this.rIsBase = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.splitContainerLines = new DevExpress.XtraEditors.SplitContainerControl();
		this.grdLine = new Apzon.Controls.Grid.GridControlEx();
		this.gvLine = new Apzon.Controls.Grid.GridViewEx();
		this.gcLineRoutCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineWorkOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.rWorkOrder = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineOpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineOpName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineResCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineResName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineSetTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineUomSetTime = new Apzon.Controls.Grid.GridColumnEx();
		this.rUom = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gcLineRunTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineUomRunTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineTransTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineUomTransTime = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineBaseQty = new Apzon.Controls.Grid.GridColumnEx();
		this.rCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcLineUomCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineProdCode = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineProdName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineVisOrder = new Apzon.Controls.Grid.GridColumnEx();
		this.gcLineSubRoutCode = new Apzon.Controls.Grid.GridColumnEx();
		this.grdEffic = new Apzon.Controls.Grid.GridControlEx();
		this.gvEffic = new Apzon.Controls.Grid.GridViewEx();
		this.gcEfficResCode = new Apzon.Controls.Grid.GridColumnEx();
		this.rEfficResCode = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcEfficResName = new Apzon.Controls.Grid.GridColumnEx();
		this.gcEfficEfficiency = new Apzon.Controls.Grid.GridColumnEx();
		this.rEfficCal = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnChooseFile = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbFile = new Apzon.Controls.Controls.LabelLine();
		this.txtFile = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControlDoc).BeginInit();
		this.splitContainerControlDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rIsBase).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerLines).BeginInit();
		this.splitContainerLines.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grdLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvLine).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rWorkOrder).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rUom).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdEffic).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvEffic).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rEfficResCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rEfficCal).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtFile.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControlDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControlDoc.Horizontal = false;
		this.splitContainerControlDoc.Location = new System.Drawing.Point(3, 35);
		this.splitContainerControlDoc.Name = "splitContainerControlDoc";
		this.splitContainerControlDoc.Panel1.Controls.Add(this.grdDoc);
		this.splitContainerControlDoc.Panel1.Text = "Panel1";
		this.splitContainerControlDoc.Panel2.Controls.Add(this.splitContainerLines);
		this.splitContainerControlDoc.Panel2.Text = "Panel2";
		this.splitContainerControlDoc.Size = new System.Drawing.Size(1464, 490);
		this.splitContainerControlDoc.SplitterPosition = 154;
		this.splitContainerControlDoc.TabIndex = 0;
		this.splitContainerControlDoc.Text = "splitContainerControl1";
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdDoc.Location = new System.Drawing.Point(0, 0);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rStatus, this.rDate, this.rIsBase });
		this.grdDoc.Size = new System.Drawing.Size(1464, 154);
		this.grdDoc.TabIndex = 0;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.gvDoc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvDoc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.gcRoutCode, this.gcRoutName, this.gcRoutFName, this.gcProdCode, this.gcProdName, this.Status, this.gcFromDate, this.gcToDate, this.gcRemarks, this.gcIsBase });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oImportRouting";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "APZ_ORTA";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gvDoc.OptionsPrint.AutoWidth = false;
		this.gvDoc.OptionsView.BestFitMaxRowCount = 100;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvDoc.OptionsView.ShowGroupedColumns = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvDoc_FocusedRowObjectChanged);
		this.gcRoutCode.Caption = "Rout Code";
		this.gcRoutCode.CurrencyDateField = null;
		this.gcRoutCode.CurrencyField = null;
		this.gcRoutCode.CurrencyFixed = null;
		this.gcRoutCode.CurrencyFixedRate = 0.0;
		this.gcRoutCode.CurrencyRateField = null;
		this.gcRoutCode.FieldName = "RoutCode";
		this.gcRoutCode.Name = "gcRoutCode";
		this.gcRoutCode.PrimaryKey = true;
		this.gcRoutCode.UnitField = null;
		this.gcRoutCode.Visible = true;
		this.gcRoutCode.VisibleIndex = 0;
		this.gcRoutName.Caption = "Rout Name";
		this.gcRoutName.CurrencyDateField = null;
		this.gcRoutName.CurrencyField = null;
		this.gcRoutName.CurrencyFixed = null;
		this.gcRoutName.CurrencyFixedRate = 0.0;
		this.gcRoutName.CurrencyRateField = null;
		this.gcRoutName.FieldName = "RoutName";
		this.gcRoutName.Name = "gcRoutName";
		this.gcRoutName.PrimaryKey = false;
		this.gcRoutName.UnitField = null;
		this.gcRoutName.Visible = true;
		this.gcRoutName.VisibleIndex = 1;
		this.gcRoutFName.Caption = "Rout FName";
		this.gcRoutFName.CurrencyDateField = null;
		this.gcRoutFName.CurrencyField = null;
		this.gcRoutFName.CurrencyFixed = null;
		this.gcRoutFName.CurrencyFixedRate = 0.0;
		this.gcRoutFName.CurrencyRateField = null;
		this.gcRoutFName.FieldName = "RoutFName";
		this.gcRoutFName.Name = "gcRoutFName";
		this.gcRoutFName.PrimaryKey = false;
		this.gcRoutFName.UnitField = null;
		this.gcRoutFName.Visible = true;
		this.gcRoutFName.VisibleIndex = 2;
		this.gcProdCode.Caption = "Production Item Code";
		this.gcProdCode.CurrencyDateField = null;
		this.gcProdCode.CurrencyField = null;
		this.gcProdCode.CurrencyFixed = null;
		this.gcProdCode.CurrencyFixedRate = 0.0;
		this.gcProdCode.CurrencyRateField = null;
		this.gcProdCode.FieldName = "ProdCode";
		this.gcProdCode.Name = "gcProdCode";
		this.gcProdCode.PrimaryKey = false;
		this.gcProdCode.UnitField = null;
		this.gcProdCode.Visible = true;
		this.gcProdCode.VisibleIndex = 3;
		this.gcProdName.Caption = "Production Item Name";
		this.gcProdName.CurrencyDateField = null;
		this.gcProdName.CurrencyField = null;
		this.gcProdName.CurrencyFixed = null;
		this.gcProdName.CurrencyFixedRate = 0.0;
		this.gcProdName.CurrencyRateField = null;
		this.gcProdName.FieldName = "ProdName";
		this.gcProdName.Name = "gcProdName";
		this.gcProdName.PrimaryKey = false;
		this.gcProdName.UnitField = null;
		this.gcProdName.Visible = true;
		this.gcProdName.VisibleIndex = 4;
		this.Status.Caption = "Status";
		this.Status.ColumnEdit = this.rStatus;
		this.Status.CurrencyDateField = null;
		this.Status.CurrencyField = null;
		this.Status.CurrencyFixed = null;
		this.Status.CurrencyFixedRate = 0.0;
		this.Status.CurrencyRateField = null;
		this.Status.FieldName = "Status";
		this.Status.Name = "Status";
		this.Status.PrimaryKey = false;
		this.Status.UnitField = null;
		this.Status.Visible = true;
		this.Status.VisibleIndex = 5;
		this.rStatus.AutoHeight = false;
		this.rStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rStatus.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rStatus.DisplayMember = "Name";
		this.rStatus.Name = "rStatus";
		this.rStatus.NullText = "";
		this.rStatus.ShowHeader = false;
		this.rStatus.ValueMember = "Code";
		this.gcFromDate.Caption = "From Date";
		this.gcFromDate.ColumnEdit = this.rDate;
		this.gcFromDate.CurrencyDateField = null;
		this.gcFromDate.CurrencyField = null;
		this.gcFromDate.CurrencyFixed = null;
		this.gcFromDate.CurrencyFixedRate = 0.0;
		this.gcFromDate.CurrencyRateField = null;
		this.gcFromDate.FieldName = "FromDate";
		this.gcFromDate.Name = "gcFromDate";
		this.gcFromDate.PrimaryKey = false;
		this.gcFromDate.UnitField = null;
		this.gcFromDate.Visible = true;
		this.gcFromDate.VisibleIndex = 6;
		this.rDate.AutoHeight = false;
		this.rDate.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDate.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.rDate.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rDate.EditFormat.FormatString = "dd/MM/yyyy";
		this.rDate.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.rDate.IsSearchControl = false;
		this.rDate.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.rDate.Name = "rDate";
		this.gcToDate.Caption = "To Date";
		this.gcToDate.ColumnEdit = this.rDate;
		this.gcToDate.CurrencyDateField = null;
		this.gcToDate.CurrencyField = null;
		this.gcToDate.CurrencyFixed = null;
		this.gcToDate.CurrencyFixedRate = 0.0;
		this.gcToDate.CurrencyRateField = null;
		this.gcToDate.FieldName = "ToDate";
		this.gcToDate.Name = "gcToDate";
		this.gcToDate.PrimaryKey = false;
		this.gcToDate.UnitField = null;
		this.gcToDate.Visible = true;
		this.gcToDate.VisibleIndex = 7;
		this.gcRemarks.Caption = "Remarks";
		this.gcRemarks.CurrencyDateField = null;
		this.gcRemarks.CurrencyField = null;
		this.gcRemarks.CurrencyFixed = null;
		this.gcRemarks.CurrencyFixedRate = 0.0;
		this.gcRemarks.CurrencyRateField = null;
		this.gcRemarks.FieldName = "Remarks";
		this.gcRemarks.Name = "gcRemarks";
		this.gcRemarks.PrimaryKey = false;
		this.gcRemarks.UnitField = null;
		this.gcRemarks.Visible = true;
		this.gcRemarks.VisibleIndex = 8;
		this.gcIsBase.Caption = "Is Base";
		this.gcIsBase.ColumnEdit = this.rIsBase;
		this.gcIsBase.CurrencyDateField = null;
		this.gcIsBase.CurrencyField = null;
		this.gcIsBase.CurrencyFixed = null;
		this.gcIsBase.CurrencyFixedRate = 0.0;
		this.gcIsBase.CurrencyRateField = null;
		this.gcIsBase.FieldName = "IsBase";
		this.gcIsBase.Name = "gcIsBase";
		this.gcIsBase.PrimaryKey = false;
		this.gcIsBase.UnitField = null;
		this.rIsBase.AutoHeight = false;
		this.rIsBase.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rIsBase.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rIsBase.DisplayMember = "Name";
		this.rIsBase.Name = "rIsBase";
		this.rIsBase.NullText = "";
		this.rIsBase.ShowHeader = false;
		this.rIsBase.ValueMember = "Code";
		this.splitContainerLines.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerLines.Location = new System.Drawing.Point(0, 0);
		this.splitContainerLines.Name = "splitContainerLines";
		this.splitContainerLines.Panel1.Controls.Add(this.grdLine);
		this.splitContainerLines.Panel1.Text = "Panel1";
		this.splitContainerLines.Panel2.Controls.Add(this.grdEffic);
		this.splitContainerLines.Panel2.Text = "Panel2";
		this.splitContainerLines.Size = new System.Drawing.Size(1464, 326);
		this.splitContainerLines.SplitterPosition = 1188;
		this.splitContainerLines.TabIndex = 1;
		this.splitContainerLines.Text = "splitContainerControl1";
		this.grdLine.AllowPasteExcel = true;
		this.grdLine.ClipboardData = null;
		this.grdLine.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdLine.Location = new System.Drawing.Point(0, 0);
		this.grdLine.MainView = this.gvLine;
		this.grdLine.Name = "grdLine";
		this.grdLine.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rWorkOrder, this.rUom, this.rCal });
		this.grdLine.Size = new System.Drawing.Size(1188, 326);
		this.grdLine.TabIndex = 0;
		this.grdLine.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvLine });
		this.gvLine.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvLine.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvLine.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvLine.ColumnKey = null;
		this.gvLine.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[18]
		{
			this.gcLineRoutCode, this.gcLineWorkOrder, this.gcLineOpCode, this.gcLineOpName, this.gcLineResCode, this.gcLineResName, this.gcLineSetTime, this.gcLineUomSetTime, this.gcLineRunTime, this.gcLineUomRunTime,
			this.gcLineTransTime, this.gcLineUomTransTime, this.gcLineBaseQty, this.gcLineUomCode, this.gcLineProdCode, this.gcLineProdName, this.gcLineVisOrder, this.gcLineSubRoutCode
		});
		this.gvLine.DisableFormSetting = false;
		this.gvLine.DisplayException = false;
		this.gvLine.EditColumn = null;
		this.gvLine.FieldCurrency = null;
		this.gvLine.FieldRate = null;
		this.gvLine.FormId = "oImportRouting";
		this.gvLine.GridControl = this.grdLine;
		this.gvLine.IgnoreFilter = false;
		this.gvLine.IndexEditRow = -1;
		this.gvLine.IndicatorWidth = 35;
		this.gvLine.IsApzTable = false;
		this.gvLine.IsInsertNewRow = false;
		this.gvLine.IsLoadingData = false;
		this.gvLine.IsMappingContextAction = false;
		this.gvLine.ItemId = "APZ_RTA1";
		this.gvLine.LstColumnAllowBatch = null;
		this.gvLine.LstColumnNotAllowBatch = null;
		this.gvLine.LstColumnNotEditable = null;
		this.gvLine.LstColumNotSetting = null;
		this.gvLine.Name = "gvLine";
		this.gvLine.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvLine.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gvLine.OptionsPrint.AutoWidth = false;
		this.gvLine.OptionsView.BestFitMaxRowCount = 100;
		this.gvLine.OptionsView.ColumnAutoWidth = false;
		this.gvLine.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvLine.OptionsView.ShowGroupedColumns = true;
		this.gvLine.OptionsView.ShowGroupPanel = false;
		this.gvLine.UseBatchMod = false;
		this.gvLine.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gvLine_FocusedRowObjectChanged);
		this.gcLineRoutCode.Caption = "Rout Code";
		this.gcLineRoutCode.CurrencyDateField = null;
		this.gcLineRoutCode.CurrencyField = null;
		this.gcLineRoutCode.CurrencyFixed = null;
		this.gcLineRoutCode.CurrencyFixedRate = 0.0;
		this.gcLineRoutCode.CurrencyRateField = null;
		this.gcLineRoutCode.FieldName = "RoutCode";
		this.gcLineRoutCode.Name = "gcLineRoutCode";
		this.gcLineRoutCode.PrimaryKey = true;
		this.gcLineRoutCode.UnitField = null;
		this.gcLineRoutCode.Visible = true;
		this.gcLineRoutCode.VisibleIndex = 0;
		this.gcLineWorkOrder.Caption = "Work Order";
		this.gcLineWorkOrder.ColumnEdit = this.rWorkOrder;
		this.gcLineWorkOrder.CurrencyDateField = null;
		this.gcLineWorkOrder.CurrencyField = null;
		this.gcLineWorkOrder.CurrencyFixed = null;
		this.gcLineWorkOrder.CurrencyFixedRate = 0.0;
		this.gcLineWorkOrder.CurrencyRateField = null;
		this.gcLineWorkOrder.FieldName = "WorkOrder";
		this.gcLineWorkOrder.Name = "gcLineWorkOrder";
		this.gcLineWorkOrder.PrimaryKey = false;
		this.gcLineWorkOrder.UnitField = null;
		this.gcLineWorkOrder.Visible = true;
		this.gcLineWorkOrder.VisibleIndex = 1;
		this.rWorkOrder.AutoHeight = false;
		this.rWorkOrder.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rWorkOrder.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rWorkOrder.DisplayMember = "Name";
		this.rWorkOrder.Name = "rWorkOrder";
		this.rWorkOrder.NullText = "";
		this.rWorkOrder.ShowHeader = false;
		this.rWorkOrder.ValueMember = "Code";
		this.gcLineOpCode.Caption = "Ope. Code";
		this.gcLineOpCode.CurrencyDateField = null;
		this.gcLineOpCode.CurrencyField = null;
		this.gcLineOpCode.CurrencyFixed = null;
		this.gcLineOpCode.CurrencyFixedRate = 0.0;
		this.gcLineOpCode.CurrencyRateField = null;
		this.gcLineOpCode.FieldName = "OpCode";
		this.gcLineOpCode.Name = "gcLineOpCode";
		this.gcLineOpCode.PrimaryKey = false;
		this.gcLineOpCode.UnitField = null;
		this.gcLineOpCode.Visible = true;
		this.gcLineOpCode.VisibleIndex = 2;
		this.gcLineOpName.Caption = "Ope. Name";
		this.gcLineOpName.CurrencyDateField = null;
		this.gcLineOpName.CurrencyField = null;
		this.gcLineOpName.CurrencyFixed = null;
		this.gcLineOpName.CurrencyFixedRate = 0.0;
		this.gcLineOpName.CurrencyRateField = null;
		this.gcLineOpName.FieldName = "OpName";
		this.gcLineOpName.Name = "gcLineOpName";
		this.gcLineOpName.PrimaryKey = false;
		this.gcLineOpName.UnitField = null;
		this.gcLineOpName.Visible = true;
		this.gcLineOpName.VisibleIndex = 3;
		this.gcLineResCode.Caption = "Res. Code";
		this.gcLineResCode.CurrencyDateField = null;
		this.gcLineResCode.CurrencyField = null;
		this.gcLineResCode.CurrencyFixed = null;
		this.gcLineResCode.CurrencyFixedRate = 0.0;
		this.gcLineResCode.CurrencyRateField = null;
		this.gcLineResCode.FieldName = "ResCode";
		this.gcLineResCode.Name = "gcLineResCode";
		this.gcLineResCode.PrimaryKey = false;
		this.gcLineResCode.UnitField = null;
		this.gcLineResCode.Visible = true;
		this.gcLineResCode.VisibleIndex = 4;
		this.gcLineResName.Caption = "Res. Name";
		this.gcLineResName.CurrencyDateField = null;
		this.gcLineResName.CurrencyField = null;
		this.gcLineResName.CurrencyFixed = null;
		this.gcLineResName.CurrencyFixedRate = 0.0;
		this.gcLineResName.CurrencyRateField = null;
		this.gcLineResName.FieldName = "ResName";
		this.gcLineResName.Name = "gcLineResName";
		this.gcLineResName.PrimaryKey = false;
		this.gcLineResName.UnitField = null;
		this.gcLineResName.Visible = true;
		this.gcLineResName.VisibleIndex = 5;
		this.gcLineSetTime.Caption = "Setup Time";
		this.gcLineSetTime.CurrencyDateField = null;
		this.gcLineSetTime.CurrencyField = null;
		this.gcLineSetTime.CurrencyFixed = null;
		this.gcLineSetTime.CurrencyFixedRate = 0.0;
		this.gcLineSetTime.CurrencyRateField = null;
		this.gcLineSetTime.FieldName = "SetTime";
		this.gcLineSetTime.Name = "gcLineSetTime";
		this.gcLineSetTime.PrimaryKey = false;
		this.gcLineSetTime.UnitField = null;
		this.gcLineSetTime.Visible = true;
		this.gcLineSetTime.VisibleIndex = 6;
		this.gcLineUomSetTime.Caption = "Uom Setup Time";
		this.gcLineUomSetTime.ColumnEdit = this.rUom;
		this.gcLineUomSetTime.CurrencyDateField = null;
		this.gcLineUomSetTime.CurrencyField = null;
		this.gcLineUomSetTime.CurrencyFixed = null;
		this.gcLineUomSetTime.CurrencyFixedRate = 0.0;
		this.gcLineUomSetTime.CurrencyRateField = null;
		this.gcLineUomSetTime.FieldName = "UomSetTime";
		this.gcLineUomSetTime.Name = "gcLineUomSetTime";
		this.gcLineUomSetTime.PrimaryKey = false;
		this.gcLineUomSetTime.UnitField = null;
		this.gcLineUomSetTime.Visible = true;
		this.gcLineUomSetTime.VisibleIndex = 7;
		this.rUom.AutoHeight = false;
		this.rUom.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rUom.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rUom.DisplayMember = "Name";
		this.rUom.Name = "rUom";
		this.rUom.NullText = "";
		this.rUom.ValueMember = "Code";
		this.gcLineRunTime.Caption = "Run Time";
		this.gcLineRunTime.CurrencyDateField = null;
		this.gcLineRunTime.CurrencyField = null;
		this.gcLineRunTime.CurrencyFixed = null;
		this.gcLineRunTime.CurrencyFixedRate = 0.0;
		this.gcLineRunTime.CurrencyRateField = null;
		this.gcLineRunTime.FieldName = "RunTime";
		this.gcLineRunTime.Name = "gcLineRunTime";
		this.gcLineRunTime.PrimaryKey = false;
		this.gcLineRunTime.UnitField = null;
		this.gcLineRunTime.Visible = true;
		this.gcLineRunTime.VisibleIndex = 8;
		this.gcLineUomRunTime.Caption = "Uom Run Time";
		this.gcLineUomRunTime.ColumnEdit = this.rUom;
		this.gcLineUomRunTime.CurrencyDateField = null;
		this.gcLineUomRunTime.CurrencyField = null;
		this.gcLineUomRunTime.CurrencyFixed = null;
		this.gcLineUomRunTime.CurrencyFixedRate = 0.0;
		this.gcLineUomRunTime.CurrencyRateField = null;
		this.gcLineUomRunTime.FieldName = "UomRunTime";
		this.gcLineUomRunTime.Name = "gcLineUomRunTime";
		this.gcLineUomRunTime.PrimaryKey = false;
		this.gcLineUomRunTime.UnitField = null;
		this.gcLineUomRunTime.Visible = true;
		this.gcLineUomRunTime.VisibleIndex = 9;
		this.gcLineTransTime.Caption = "Transfer Time";
		this.gcLineTransTime.CurrencyDateField = null;
		this.gcLineTransTime.CurrencyField = null;
		this.gcLineTransTime.CurrencyFixed = null;
		this.gcLineTransTime.CurrencyFixedRate = 0.0;
		this.gcLineTransTime.CurrencyRateField = null;
		this.gcLineTransTime.FieldName = "TransTime";
		this.gcLineTransTime.Name = "gcLineTransTime";
		this.gcLineTransTime.PrimaryKey = false;
		this.gcLineTransTime.UnitField = null;
		this.gcLineTransTime.Visible = true;
		this.gcLineTransTime.VisibleIndex = 10;
		this.gcLineUomTransTime.Caption = "Uom Transfer Time";
		this.gcLineUomTransTime.ColumnEdit = this.rUom;
		this.gcLineUomTransTime.CurrencyDateField = null;
		this.gcLineUomTransTime.CurrencyField = null;
		this.gcLineUomTransTime.CurrencyFixed = null;
		this.gcLineUomTransTime.CurrencyFixedRate = 0.0;
		this.gcLineUomTransTime.CurrencyRateField = null;
		this.gcLineUomTransTime.FieldName = "UomTransTime";
		this.gcLineUomTransTime.Name = "gcLineUomTransTime";
		this.gcLineUomTransTime.PrimaryKey = false;
		this.gcLineUomTransTime.UnitField = null;
		this.gcLineUomTransTime.Visible = true;
		this.gcLineUomTransTime.VisibleIndex = 11;
		this.gcLineBaseQty.Caption = "Base Quantity";
		this.gcLineBaseQty.ColumnEdit = this.rCal;
		this.gcLineBaseQty.CurrencyDateField = null;
		this.gcLineBaseQty.CurrencyField = null;
		this.gcLineBaseQty.CurrencyFixed = null;
		this.gcLineBaseQty.CurrencyFixedRate = 0.0;
		this.gcLineBaseQty.CurrencyRateField = null;
		this.gcLineBaseQty.FieldName = "BaseQty";
		this.gcLineBaseQty.Name = "gcLineBaseQty";
		this.gcLineBaseQty.PrimaryKey = false;
		this.gcLineBaseQty.UnitField = null;
		this.gcLineBaseQty.Visible = true;
		this.gcLineBaseQty.VisibleIndex = 12;
		this.rCal.AutoHeight = false;
		this.rCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCal.Name = "rCal";
		this.gcLineUomCode.Caption = "Uom Code";
		this.gcLineUomCode.CurrencyDateField = null;
		this.gcLineUomCode.CurrencyField = null;
		this.gcLineUomCode.CurrencyFixed = null;
		this.gcLineUomCode.CurrencyFixedRate = 0.0;
		this.gcLineUomCode.CurrencyRateField = null;
		this.gcLineUomCode.FieldName = "UomCode";
		this.gcLineUomCode.Name = "gcLineUomCode";
		this.gcLineUomCode.PrimaryKey = false;
		this.gcLineUomCode.UnitField = null;
		this.gcLineUomCode.Visible = true;
		this.gcLineUomCode.VisibleIndex = 13;
		this.gcLineProdCode.Caption = "Prod. Item Code";
		this.gcLineProdCode.CurrencyDateField = null;
		this.gcLineProdCode.CurrencyField = null;
		this.gcLineProdCode.CurrencyFixed = null;
		this.gcLineProdCode.CurrencyFixedRate = 0.0;
		this.gcLineProdCode.CurrencyRateField = null;
		this.gcLineProdCode.FieldName = "ProdCode";
		this.gcLineProdCode.Name = "gcLineProdCode";
		this.gcLineProdCode.PrimaryKey = false;
		this.gcLineProdCode.UnitField = null;
		this.gcLineProdCode.Visible = true;
		this.gcLineProdCode.VisibleIndex = 14;
		this.gcLineProdName.Caption = "Prod. Item Name";
		this.gcLineProdName.CurrencyDateField = null;
		this.gcLineProdName.CurrencyField = null;
		this.gcLineProdName.CurrencyFixed = null;
		this.gcLineProdName.CurrencyFixedRate = 0.0;
		this.gcLineProdName.CurrencyRateField = null;
		this.gcLineProdName.FieldName = "ProdName";
		this.gcLineProdName.Name = "gcLineProdName";
		this.gcLineProdName.PrimaryKey = false;
		this.gcLineProdName.UnitField = null;
		this.gcLineProdName.Visible = true;
		this.gcLineProdName.VisibleIndex = 15;
		this.gcLineVisOrder.Caption = "Vis. Order";
		this.gcLineVisOrder.CurrencyDateField = null;
		this.gcLineVisOrder.CurrencyField = null;
		this.gcLineVisOrder.CurrencyFixed = null;
		this.gcLineVisOrder.CurrencyFixedRate = 0.0;
		this.gcLineVisOrder.CurrencyRateField = null;
		this.gcLineVisOrder.FieldName = "VisOrder";
		this.gcLineVisOrder.Name = "gcLineVisOrder";
		this.gcLineVisOrder.PrimaryKey = false;
		this.gcLineVisOrder.UnitField = null;
		this.gcLineVisOrder.Visible = true;
		this.gcLineVisOrder.VisibleIndex = 16;
		this.gcLineSubRoutCode.Caption = "Sub. Routing";
		this.gcLineSubRoutCode.CurrencyDateField = null;
		this.gcLineSubRoutCode.CurrencyField = null;
		this.gcLineSubRoutCode.CurrencyFixed = null;
		this.gcLineSubRoutCode.CurrencyFixedRate = 0.0;
		this.gcLineSubRoutCode.CurrencyRateField = null;
		this.gcLineSubRoutCode.FieldName = "SubRoutCode";
		this.gcLineSubRoutCode.Name = "gcLineSubRoutCode";
		this.gcLineSubRoutCode.PrimaryKey = false;
		this.gcLineSubRoutCode.UnitField = null;
		this.gcLineSubRoutCode.Visible = true;
		this.gcLineSubRoutCode.VisibleIndex = 17;
		this.grdEffic.AllowPasteExcel = false;
		this.grdEffic.ClipboardData = null;
		this.grdEffic.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grdEffic.Location = new System.Drawing.Point(0, 0);
		this.grdEffic.MainView = this.gvEffic;
		this.grdEffic.Name = "grdEffic";
		this.grdEffic.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rEfficResCode, this.rEfficCal });
		this.grdEffic.Size = new System.Drawing.Size(266, 326);
		this.grdEffic.TabIndex = 1;
		this.grdEffic.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvEffic });
		this.gvEffic.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gvEffic.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gvEffic.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvEffic.ColumnKey = null;
		this.gvEffic.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gcEfficResCode, this.gcEfficResName, this.gcEfficEfficiency });
		this.gvEffic.DisableFormSetting = false;
		this.gvEffic.DisplayException = false;
		this.gvEffic.EditColumn = null;
		this.gvEffic.FieldCurrency = null;
		this.gvEffic.FieldRate = null;
		this.gvEffic.FormId = "oImportRouting";
		this.gvEffic.GridControl = this.grdEffic;
		this.gvEffic.IgnoreFilter = false;
		this.gvEffic.IndexEditRow = -1;
		this.gvEffic.IndicatorWidth = 35;
		this.gvEffic.IsApzTable = false;
		this.gvEffic.IsInsertNewRow = false;
		this.gvEffic.IsLoadingData = false;
		this.gvEffic.IsMappingContextAction = false;
		this.gvEffic.ItemId = "APZ_RTA2";
		this.gvEffic.LstColumnAllowBatch = null;
		this.gvEffic.LstColumnNotAllowBatch = null;
		this.gvEffic.LstColumnNotEditable = null;
		this.gvEffic.LstColumNotSetting = null;
		this.gvEffic.Name = "gvEffic";
		this.gvEffic.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gvEffic.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gvEffic.OptionsPrint.AutoWidth = false;
		this.gvEffic.OptionsPrint.UsePrintStyles = false;
		this.gvEffic.OptionsView.BestFitMaxRowCount = 100;
		this.gvEffic.OptionsView.ColumnAutoWidth = false;
		this.gvEffic.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gvEffic.OptionsView.ShowGroupedColumns = true;
		this.gvEffic.OptionsView.ShowGroupPanel = false;
		this.gvEffic.UseBatchMod = false;
		this.gcEfficResCode.Caption = "Resource Code";
		this.gcEfficResCode.ColumnEdit = this.rEfficResCode;
		this.gcEfficResCode.CurrencyDateField = null;
		this.gcEfficResCode.CurrencyField = null;
		this.gcEfficResCode.CurrencyFixed = null;
		this.gcEfficResCode.CurrencyFixedRate = 0.0;
		this.gcEfficResCode.CurrencyRateField = null;
		this.gcEfficResCode.FieldName = "ResCode";
		this.gcEfficResCode.Name = "gcEfficResCode";
		this.gcEfficResCode.PrimaryKey = true;
		this.gcEfficResCode.UnitField = null;
		this.gcEfficResCode.Visible = true;
		this.gcEfficResCode.VisibleIndex = 0;
		this.gcEfficResCode.Width = 82;
		this.rEfficResCode.AutoHeight = false;
		this.rEfficResCode.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rEfficResCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rEfficResCode.ContainerControl = this;
		this.rEfficResCode.DisplayMember = "";
		this.rEfficResCode.DisplayMember2 = "";
		this.rEfficResCode.LoadDataSourceWhenEnter = false;
		this.rEfficResCode.Name = "rEfficResCode";
		this.rEfficResCode.SeparateCharacterForMulti = ';';
		this.rEfficResCode.SeparateString = ", ";
		this.rEfficResCode.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.ListWorkCenter;
		this.rEfficResCode.TypeDrillDown = Apzon.Commons.ApzObjectTypes.WorkCenter;
		this.rEfficResCode.UsingAutoCompleteBox = false;
		this.rEfficResCode.UsingDataSource = true;
		this.rEfficResCode.ValidateWhenNoDataSource = false;
		this.rEfficResCode.ValueMember = "ResCode";
		this.gcEfficResName.Caption = "Resource Name";
		this.gcEfficResName.CurrencyDateField = null;
		this.gcEfficResName.CurrencyField = null;
		this.gcEfficResName.CurrencyFixed = null;
		this.gcEfficResName.CurrencyFixedRate = 0.0;
		this.gcEfficResName.CurrencyRateField = null;
		this.gcEfficResName.FieldName = "ResName";
		this.gcEfficResName.Name = "gcEfficResName";
		this.gcEfficResName.PrimaryKey = false;
		this.gcEfficResName.UnitField = null;
		this.gcEfficResName.Visible = true;
		this.gcEfficResName.VisibleIndex = 1;
		this.gcEfficResName.Width = 100;
		this.gcEfficEfficiency.Caption = "Efficiency";
		this.gcEfficEfficiency.ColumnEdit = this.rEfficCal;
		this.gcEfficEfficiency.CurrencyDateField = null;
		this.gcEfficEfficiency.CurrencyField = null;
		this.gcEfficEfficiency.CurrencyFixed = null;
		this.gcEfficEfficiency.CurrencyFixedRate = 0.0;
		this.gcEfficEfficiency.CurrencyRateField = null;
		this.gcEfficEfficiency.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Percent;
		this.gcEfficEfficiency.FieldName = "Effic";
		this.gcEfficEfficiency.Name = "gcEfficEfficiency";
		this.gcEfficEfficiency.PrimaryKey = false;
		this.gcEfficEfficiency.UnitField = null;
		this.gcEfficEfficiency.Visible = true;
		this.gcEfficEfficiency.VisibleIndex = 2;
		this.rEfficCal.AutoHeight = false;
		this.rEfficCal.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rEfficCal.Name = "rEfficCal";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = "btnCancel";
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(1389, 531);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 4;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnSave.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btnSave.EditControl = null;
		this.btnSave.EditValue = "Save";
		this.btnSave.FieldName = "btnSave";
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.IsSearchControl = false;
		this.btnSave.Location = new System.Drawing.Point(1310, 531);
		this.btnSave.Name = "btnSave";
		this.btnSave.ReadOnly = false;
		this.btnSave.SettingInVisible = false;
		this.btnSave.SettingReadonly = false;
		this.btnSave.Size = new System.Drawing.Size(75, 23);
		this.btnSave.TabIndex = 3;
		this.btnSave.Text = "Save";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.btnChooseFile.EditControl = null;
		this.btnChooseFile.EditValue = "";
		this.btnChooseFile.FieldName = "btnChooseFile";
		this.btnChooseFile.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnChooseFile.ImageOptions.Image");
		this.btnChooseFile.IsSearchControl = false;
		this.btnChooseFile.Location = new System.Drawing.Point(365, 5);
		this.btnChooseFile.Name = "btnChooseFile";
		this.btnChooseFile.ReadOnly = false;
		this.btnChooseFile.SettingInVisible = false;
		this.btnChooseFile.SettingReadonly = false;
		this.btnChooseFile.Size = new System.Drawing.Size(25, 20);
		this.btnChooseFile.TabIndex = 2;
		this.btnChooseFile.Click += new System.EventHandler(this.btnChooseFile_Click);
		this.lbFile.EditControl = null;
		this.lbFile.EditValue = "Path File";
		this.lbFile.FieldName = "lbFile";
		this.lbFile.HideWhenEmpty = false;
		this.lbFile.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbFile.Location = new System.Drawing.Point(14, 8);
		this.lbFile.Name = "lbFile";
		this.lbFile.ReadOnly = false;
		this.lbFile.SettingInVisible = false;
		this.lbFile.SettingReadonly = false;
		this.lbFile.ShowUnderLine = true;
		this.lbFile.Size = new System.Drawing.Size(80, 20);
		this.lbFile.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbFile.TabIndex = 0;
		this.lbFile.Utext = "Path File";
		this.txtFile.EditControl = null;
		this.txtFile.FieldName = "PathFile";
		this.txtFile.IsSearchControl = false;
		this.txtFile.Location = new System.Drawing.Point(95, 5);
		this.txtFile.Name = "txtFile";
		this.txtFile.SettingInVisible = false;
		this.txtFile.SettingReadonly = false;
		this.txtFile.Size = new System.Drawing.Size(268, 20);
		this.txtFile.TabIndex = 1;
		this.txtFile.EditValueChanged += new System.EventHandler(this.txtFile_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1469, 558);
		base.Controls.Add(this.txtFile);
		base.Controls.Add(this.lbFile);
		base.Controls.Add(this.btnChooseFile);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.splitContainerControlDoc);
		base.FormId = "oImportRouting";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ImportRouting";
		this.Text = "Import Routing";
		base.Load += new System.EventHandler(this.frm_ImportRouting_Load);
		base.Resize += new System.EventHandler(this.frm_ImportRouting_Resize);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControlDoc).EndInit();
		this.splitContainerControlDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDate).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rIsBase).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerLines).EndInit();
		this.splitContainerLines.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grdLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvLine).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rWorkOrder).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rUom).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdEffic).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvEffic).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rEfficResCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rEfficCal).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtFile.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
