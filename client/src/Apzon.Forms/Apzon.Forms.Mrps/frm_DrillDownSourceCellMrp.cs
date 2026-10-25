using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Pivot;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Production;
using Apzon.Forms.Sales;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPivotGrid;

namespace Apzon.Forms.Mrps;

public class frm_DrillDownSourceCellMrp : XtraFormEx
{
	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataTable _dataSource;

	private Enums.FormViewModeType _fomrViewMode = Enums.FormViewModeType.View;

	protected int ColumnIndexContext = -1;

	protected int RowIndexContext = -1;

	private DateTime _startDate;

	private DateTime _toDate;

	private DateTime _forecastDate;

	public DataTable SourceDetail = new DataTable();

	private IContainer components;

	private SimpleButtonEx btnOk;

	private PivotGridControlEx pgcDoc;

	private PivotGridField pcSourceType;

	private PivotGridField pcSourceEntry;

	private PivotGridField pcSourceLine;

	private PivotGridField pcRequirementDate;

	private PivotGridField pcRequirementShift;

	private PivotGridField pcRequirementQty;

	private RepositoryItemDateEdit rDateEdit;

	private RepositoryItemCalcEdit rCalEdit;

	private PivotGridField pcItemCode;

	private PivotGridField pcFatherCode;

	private PivotGridField pcCardCode;

	private PivotGridField pcPlanning;

	private PivotGridField pcVersion;

	protected SimpleButtonEx btnAutoFillData;

	private ContextMenuStrip contextMenuStripDoc;

	private ToolStripMenuItem autoFillColumnToolStripMenuItem;

	private ToolStripMenuItem openSourceFormToolStripMenuItem;

	private SimpleButton btnCancel;

	private PivotGridField pcBaseLine;

	private frm_DrillDownSourceCellMrp()
	{
		InitializeComponent();
	}

	public frm_DrillDownSourceCellMrp(DataTable dataSource, DateTime startDate, DateTime endDate, DateTime forecastDate, Enums.FormViewModeType formViewMode)
	{
		_dataSource = dataSource;
		_fomrViewMode = formViewMode;
		_startDate = startDate;
		_toDate = endDate;
		_forecastDate = forecastDate;
		InitializeComponent();
	}

	private async void frm_DrillDownSourceCellMrp_Load(object sender, EventArgs e)
	{
		try
		{
			btnAutoFillData.Visible = _fomrViewMode == Enums.FormViewModeType.Add;
			pcRequirementDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
			pcRequirementQty.SetFormatControl(Enums.DisplayFormatType.Qty);
			rCalEdit.ReadOnly = _fomrViewMode != Enums.FormViewModeType.Add;
			await LoadDataDocumentType();
			pgcDoc.DataSource = _dataSource;
			pgcDoc.Refresh();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void btnOk_Click(object sender, EventArgs e)
	{
		SourceDetail = pgcDoc.DataSourceTable();
		DialogResult = DialogResult.OK;
		Close();
	}

	private async Task LoadDataDocumentType()
	{
	}

	private void pgcDoc_CustomDrawFieldValue(object sender, PivotCustomDrawFieldValueEventArgs e)
	{
		try
		{
			if (e != null && e.ValueType == PivotGridValueType.Total)
			{
				e.Handled = true;
				Rectangle bounds = e.Bounds;
				bounds.Inflate(-1, -1);
				e.GraphicsCache.FillRectangle(Color.OldLace, bounds);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDoc_CustomCellDisplayText(object sender, PivotCellDisplayTextEventArgs e)
	{
		try
		{
			if (e != null)
			{
				if (e.RowValueType == PivotGridValueType.Total)
				{
					e.DisplayText = "";
				}
				else if ((e.RowValueType == PivotGridValueType.Value || e.RowValueType == PivotGridValueType.GrandTotal) && Function.ParseDouble(e.Value).Equals(0.0))
				{
					e.DisplayText = "";
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDoc_CustomCellValue(object sender, PivotCellValueEventArgs e)
	{
		try
		{
			if (e != null && e.ColumnField == null && e.ColumnValueType == PivotGridValueType.GrandTotal && Function.ToString(pgcDoc.GetFieldValue(pgcDoc.Fields[pcPlanning.FieldName], e.RowIndex)).StartsWith("4."))
			{
				PivotDrillDownDataSource pivotDrillDownDataSource = pgcDoc.CreateDrillDownDataSource(e.ColumnIndex - 1, e.RowIndex);
				if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0)
				{
					e.Value = pivotDrillDownDataSource[0][pcRequirementQty.FieldName];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void pgcDoc_CustomRowHeight(object sender, PivotCustomRowHeightEventArgs e)
	{
		try
		{
			if (e.ValueType == PivotGridValueType.Total)
			{
				e.RowHeight = e.RowHeight * 2 / 3;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void pgcDoc_ShowingEditor(object sender, CancelPivotCellEditEventArgs e)
	{
		try
		{
			if (e.ColumnValueType == PivotGridValueType.Total || e.ColumnValueType == PivotGridValueType.GrandTotal || e.RowValueType == PivotGridValueType.Total || e.RowValueType == PivotGridValueType.GrandTotal)
			{
				e.Cancel = true;
			}
			if (e.RowField == pcPlanning && !Function.ToString(pgcDoc.GetFieldValue(pcPlanning, e.RowIndex)).StartsWith("2"))
			{
				e.Cancel = true;
			}
			if (e.ColumnField == pcRequirementShift)
			{
				if (string.IsNullOrEmpty(Function.ToString(pgcDoc.GetFieldValue(pcRequirementShift, e.ColumnIndex))))
				{
					e.Cancel = true;
				}
				DateTime? dateTime = Function.ParseDateTime(pgcDoc.GetFieldValue(pcRequirementDate, e.ColumnIndex));
				if (dateTime.HasValue && (dateTime.Value.Date > _forecastDate.Date || dateTime.Value.Date < DateTime.Now.Date))
				{
					e.Cancel = true;
				}
				PivotDrillDownDataSource pivotDrillDownDataSource = pgcDoc.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
				if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0 && Function.ParseInt(pivotDrillDownDataSource[0]["DocEntry"]) > 0)
				{
					e.Cancel = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void pgcDoc_EditValueChanged(object sender, EditValueChangedEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (e == null || e.ColumnField == null || e.RowField == null)
			{
				return;
			}
			if (e.DataField == pcRequirementQty)
			{
				if (e.Editor.EditValue == null)
				{
					e.Editor.EditValue = 0;
				}
				ChangeCellValue(e, Function.ParseDouble(e.Editor.EditValue));
				pgcDoc.ShowEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private void pgcDoc_FieldValueDisplayText(object sender, PivotFieldDisplayTextEventArgs e)
	{
		try
		{
			if (e == null || e.Field == null || e.ValueType == PivotGridValueType.Total)
			{
				return;
			}
			if (e.ValueType == PivotGridValueType.Total)
			{
				e.DisplayText = "";
			}
			else if (e.Field.FieldName.Equals(pcRequirementDate.FieldName) && e.FieldIndex == 0 && Function.ParseDateTimes(pgcDoc.GetFieldValue(pcRequirementDate, 0)).Date == _startDate.Date.AddDays(-1.0))
			{
				e.DisplayText = "Over Due";
			}
			if (e.Field.FieldName.Equals(pcSourceType.FieldName) && e.ValueType == PivotGridValueType.Value)
			{
				switch ((ApzObjectTypes)Function.ParseInt(e.Value))
				{
				case ApzObjectTypes.oAdvanceForecast:
					e.DisplayText = "Forecast";
					break;
				case ApzObjectTypes.oOrders:
					e.DisplayText = "Sales Order";
					break;
				case ApzObjectTypes.oSchedule:
					e.DisplayText = "Schedule";
					break;
				case ApzObjectTypes.oProductionOrders:
					e.DisplayText = "Production";
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnAutoFillData_Click(object sender, EventArgs e)
	{
		try
		{
			LoadDataAutoFillClick();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void pgcDoc_CustomAppearance(object sender, PivotCustomAppearanceEventArgs e)
	{
		try
		{
			if (e == null || e.RowField == null || e.ColumnField == null)
			{
				return;
			}
			if (e.RowValueType == PivotGridValueType.Total)
			{
				e.Appearance.BackColor = Color.OldLace;
				e.Appearance.BackColor2 = Color.OldLace;
				return;
			}
			Point focusedCell = pgcDoc.Cells.FocusedCell;
			PivotCellEventArgs cellInfo = pgcDoc.Cells.GetCellInfo(focusedCell.X, focusedCell.Y);
			if (e.RowIndex == cellInfo.RowIndex)
			{
				if (e.ColumnIndex == cellInfo.ColumnIndex && Function.ToString(pgcDoc.GetFieldValue(pcPlanning, e.RowIndex)).StartsWith("2."))
				{
					if (e.ColumnField == pcRequirementShift)
					{
						if (string.IsNullOrEmpty(Function.ToString(pgcDoc.GetFieldValue(pcRequirementShift, e.ColumnIndex))))
						{
							e.Appearance.BackColor = pcRequirementQty.Appearance.Cell.BackColor;
							e.Appearance.BackColor2 = pcRequirementQty.Appearance.Cell.BackColor2;
							return;
						}
						DateTime? dateTime = Function.ParseDateTime(pgcDoc.GetFieldValue(pcRequirementDate, e.ColumnIndex));
						if (dateTime.HasValue && (dateTime.Value.Date > _forecastDate.Date || dateTime.Value.Date < DateTime.Now.Date))
						{
							e.Appearance.BackColor = pcRequirementQty.Appearance.Cell.BackColor;
							e.Appearance.BackColor2 = pcRequirementQty.Appearance.Cell.BackColor2;
							return;
						}
						PivotDrillDownDataSource pivotDrillDownDataSource = pgcDoc.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
						if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0 && Function.ParseInt(pivotDrillDownDataSource[0]["DocEntry"]) > 0)
						{
							e.Appearance.BackColor = pcRequirementQty.Appearance.Cell.BackColor;
							e.Appearance.BackColor2 = pcRequirementQty.Appearance.Cell.BackColor2;
							return;
						}
					}
					e.Appearance.BackColor = Color.White;
					e.Appearance.BackColor2 = Color.White;
				}
				else
				{
					e.Appearance.BackColor = Color.Moccasin;
				}
				return;
			}
			if (e.ColumnField.FieldName.Equals(pcRequirementDate.FieldName) && e.ColumnIndex == 0)
			{
				e.Appearance.BackColor = pgcDoc.BackColor;
			}
			if (e.RowField.FieldName.Equals("Planning") && Function.ToString(pgcDoc.GetFieldValue(pcPlanning, e.RowIndex)).StartsWith("2."))
			{
				if (e.ColumnField == pcRequirementShift)
				{
					if (string.IsNullOrEmpty(Function.ToString(pgcDoc.GetFieldValue(pcRequirementShift, e.ColumnIndex))))
					{
						e.Appearance.BackColor = pcRequirementQty.Appearance.Cell.BackColor;
						e.Appearance.BackColor2 = pcRequirementQty.Appearance.Cell.BackColor2;
						return;
					}
					DateTime? dateTime2 = Function.ParseDateTime(pgcDoc.GetFieldValue(pcRequirementDate, e.ColumnIndex));
					if (dateTime2.HasValue && (dateTime2.Value.Date > _forecastDate.Date || dateTime2.Value.Date < DateTime.Now.Date))
					{
						e.Appearance.BackColor = pcRequirementQty.Appearance.Cell.BackColor;
						e.Appearance.BackColor2 = pcRequirementQty.Appearance.Cell.BackColor2;
						return;
					}
					PivotDrillDownDataSource pivotDrillDownDataSource2 = pgcDoc.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
					if (pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2.RowCount > 0 && Function.ParseInt(pivotDrillDownDataSource2[0]["DocEntry"]) > 0)
					{
						e.Appearance.BackColor = pcRequirementQty.Appearance.Cell.BackColor;
						e.Appearance.BackColor2 = pcRequirementQty.Appearance.Cell.BackColor2;
						return;
					}
				}
				e.Appearance.BackColor = Color.White;
			}
			if (e.ColumnField.FieldName.Equals("Shift"))
			{
				switch (Function.ParseDateTimes(pgcDoc.GetFieldValue(pcRequirementDate, e.ColumnIndex)).DayOfWeek)
				{
				case DayOfWeek.Saturday:
					e.Appearance.BackColor = Color.FromArgb(233, 249, 242);
					break;
				case DayOfWeek.Sunday:
					e.Appearance.BackColor = Color.FromArgb(225, 234, 237);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void autoFillColumnToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			LoadDataAutoFillColumn();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void ChangeCellValue(EditValueChangedEventArgs e, double newValue)
	{
		try
		{
			PivotDrillDownDataSource pivotDrillDownDataSource = pgcDoc.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
			if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0)
			{
				pivotDrillDownDataSource[0][pcRequirementQty] = newValue;
			}
			pgcDoc.DataSourceTable().AcceptChanges();
			pgcDoc.RefreshData();
			LoadDataRemainQty();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void contextMenuStripDoc_Opening(object sender, CancelEventArgs e)
	{
		try
		{
			ColumnIndexContext = -1;
			Point hitPoint = PointToClient(Cursor.Position);
			if ((hitPoint.X < (pcCardCode.Visible ? pcCardCode.Width : 0) + (pcItemCode.Visible ? pcItemCode.Width : 0) + (pcVersion.Visible ? pcVersion.Width : 0) + (pcFatherCode.Visible ? pcFatherCode.Width : 0) + (pcPlanning.Visible ? pcPlanning.Width : 0) + (pcSourceEntry.Visible ? pcSourceEntry.Width : 0) + (pcSourceLine.Visible ? pcSourceLine.Width : 0) + (pcSourceType.Visible ? pcSourceType.Width : 0) + 20 * Main.WidScreen / 1920 && hitPoint.Y > pgcDoc.Location.Y) || (hitPoint.Y > pgcDoc.Location.Y && hitPoint.Y < pgcDoc.Location.Y + 75 * Main.HeightScreen / 1080))
			{
				e.Cancel = true;
				return;
			}
			PivotGridHitInfo pivotGridHitInfo = pgcDoc.CalcHitInfo(hitPoint);
			if (pivotGridHitInfo.HitTest == PivotGridHitTest.Cell)
			{
				ColumnIndexContext = pivotGridHitInfo.CellInfo.ColumnIndex;
				RowIndexContext = pivotGridHitInfo.CellInfo.RowIndex;
			}
			autoFillColumnToolStripMenuItem.Visible = ColumnIndexContext != -1;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void openSourceFormToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			PivotDrillDownDataSource pivotDrillDownDataSource = pgcDoc.CreateDrillDownDataSource(ColumnIndexContext, RowIndexContext);
			if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount != 0 && !string.IsNullOrEmpty(Function.ToString(pivotDrillDownDataSource[0]["DemandType"])))
			{
				ApzObjectTypes objType = (ApzObjectTypes)Function.ParseInt(pivotDrillDownDataSource[0]["DemandType"]);
				int docEntry = Function.ParseInt(pivotDrillDownDataSource[0]["DemandEntry"]);
				int lineNo = Function.ParseInt(pivotDrillDownDataSource[0]["DemandLine"]);
				DateTime? dueDate = Function.ParseDateTime(pivotDrillDownDataSource[0]["WorkDay"]);
				string itemCode = Function.ToString(pivotDrillDownDataSource[0]["ItemCode"]);
				string shift = Function.ToString(pivotDrillDownDataSource[0]["Shift"]);
				string whsCode = "";
				DataTable dataTable = await _httpClient.Mrp.LoadDataSourceCell(Function.ToString(pivotDrillDownDataSource[0]["DemandType"]), docEntry, lineNo);
				if (dataTable.IsNotNull())
				{
					whsCode = Function.ToString(dataTable.Rows[0]["WhsCode"]);
				}
				switch (objType)
				{
				case ApzObjectTypes.oOrders:
				{
					frm_SalesOrder frm_SalesOrder2 = new frm_SalesOrder(docEntry, lineNo);
					frm_SalesOrder2.MdiParent = MdiParent;
					frm_SalesOrder2.Show();
					break;
				}
				case ApzObjectTypes.oAdvanceForecast:
				{
					frm_AdvanceForecast frm_AdvanceForecast2 = new frm_AdvanceForecast(Function.ToString(docEntry), itemCode, whsCode, dueDate);
					frm_AdvanceForecast2.MdiParent = MdiParent;
					frm_AdvanceForecast2.Show();
					break;
				}
				case ApzObjectTypes.oSchedule:
				{
					frm_Schedule frm_Schedule2 = new frm_Schedule(docEntry, itemCode, dueDate, shift);
					frm_Schedule2.MdiParent = MdiParent;
					frm_Schedule2.Show();
					break;
				}
				case ApzObjectTypes.oProductionOrders:
				{
					frm_ProductionOrder frm_ProductionOrder2 = new frm_ProductionOrder(docEntry, lineNo);
					frm_ProductionOrder2.MdiParent = MdiParent;
					frm_ProductionOrder2.Show();
					break;
				}
				}
			}
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

	protected void LoadDataAutoFillClick()
	{
		int columnCount = pgcDoc.Cells.ColumnCount;
		int rowCount = pgcDoc.Cells.RowCount;
		for (int i = 0; i < rowCount; i++)
		{
			if (!Function.ToString(pgcDoc.GetFieldValue(pcPlanning, i)).StartsWith("1."))
			{
				continue;
			}
			for (int j = 1; j < columnCount - 1; j++)
			{
				PivotDrillDownDataSource pivotDrillDownDataSource = pgcDoc.CreateDrillDownDataSource(j, i + 2);
				PivotDrillDownDataSource pivotDrillDownDataSource2 = pgcDoc.CreateDrillDownDataSource(j, i);
				PivotDrillDownDataSource pivotDrillDownDataSource3 = pgcDoc.CreateDrillDownDataSource(j, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource4 = pgcDoc.CreateDrillDownDataSource(j - 1, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource5 = pgcDoc.CreateDrillDownDataSource(j, i + 1);
				double num = 0.0;
				double num2 = 0.0;
				bool flag = true;
				if (pivotDrillDownDataSource5 != null && pivotDrillDownDataSource5.RowCount > 0)
				{
					if (pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2.RowCount > 0)
					{
						if (string.IsNullOrEmpty(Function.ToString(pivotDrillDownDataSource2[0][pcRequirementShift])) || DateTime.Now.Date > Function.ParseDateTimes(pivotDrillDownDataSource2[0][pcRequirementDate]) || _forecastDate.Date < Function.ParseDateTimes(pivotDrillDownDataSource2[0][pcRequirementDate]) || Function.ParseInt(pivotDrillDownDataSource2[0]["DocEntry"]) > 0)
						{
							flag = false;
						}
						num = Function.ParseDouble(pivotDrillDownDataSource2[0][pcRequirementQty]);
					}
					if (j == 1)
					{
						PivotDrillDownDataSource pivotDrillDownDataSource6 = pgcDoc.CreateDrillDownDataSource(0, i);
						if (pivotDrillDownDataSource6 != null && pivotDrillDownDataSource6.RowCount > 0)
						{
							num2 = Function.ParseDouble(pivotDrillDownDataSource6[0][pcRequirementQty]);
						}
					}
					if (flag)
					{
						if (Function.ParseDouble(pivotDrillDownDataSource5[0][pcRequirementQty]) == 0.0)
						{
							num += num2;
							pivotDrillDownDataSource5[0][pcRequirementQty] = num;
							if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0)
							{
								pivotDrillDownDataSource[0][pcRequirementQty] = num;
							}
						}
						else
						{
							num = Function.ParseDouble(pivotDrillDownDataSource5[0][pcRequirementQty]);
						}
					}
					else
					{
						num = 0.0;
					}
				}
				double num3 = ((pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2[0] != null) ? Function.ParseDouble(pivotDrillDownDataSource2[0][pcRequirementQty]) : 0.0);
				double num4 = 0.0;
				if (pivotDrillDownDataSource4 != null && pivotDrillDownDataSource4.RowCount > 0)
				{
					num4 = Function.ParseDouble(pivotDrillDownDataSource4[0][pcRequirementQty]);
				}
				if (pivotDrillDownDataSource3 != null && pivotDrillDownDataSource3[0] != null)
				{
					pivotDrillDownDataSource3[0][pcRequirementQty] = num3 + num4 + num2 - num;
				}
			}
			i += 4;
		}
		pgcDoc.RefreshData();
	}

	protected void LoadDataAutoFillColumn()
	{
		int columnCount = pgcDoc.Cells.ColumnCount;
		int rowCount = pgcDoc.Cells.RowCount;
		for (int i = 0; i < rowCount; i++)
		{
			if (!Function.ToString(pgcDoc.GetFieldValue(pcPlanning, i)).StartsWith("1."))
			{
				continue;
			}
			for (int j = 1; j < columnCount - 1; j++)
			{
				PivotDrillDownDataSource pivotDrillDownDataSource = pgcDoc.CreateDrillDownDataSource(j, i + 2);
				PivotDrillDownDataSource pivotDrillDownDataSource2 = pgcDoc.CreateDrillDownDataSource(j, i);
				PivotDrillDownDataSource pivotDrillDownDataSource3 = pgcDoc.CreateDrillDownDataSource(j, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource4 = pgcDoc.CreateDrillDownDataSource(j - 1, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource5 = pgcDoc.CreateDrillDownDataSource(j, i + 1);
				double num = 0.0;
				double num2 = 0.0;
				bool flag = true;
				if (pivotDrillDownDataSource5 != null && pivotDrillDownDataSource5.RowCount > 0)
				{
					if (pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2.RowCount > 0)
					{
						if (string.IsNullOrEmpty(Function.ToString(pivotDrillDownDataSource2[0][pcRequirementShift])) || DateTime.Now.Date > Function.ParseDateTimes(pivotDrillDownDataSource2[0][pcRequirementDate]) || _forecastDate.Date < Function.ParseDateTimes(pivotDrillDownDataSource2[0][pcRequirementDate]) || Function.ParseInt(pivotDrillDownDataSource2[0]["DocEntry"]) > 0)
						{
							flag = false;
						}
						num = Function.ParseDouble(pivotDrillDownDataSource2[0][pcRequirementQty]);
					}
					if (j == 1)
					{
						PivotDrillDownDataSource pivotDrillDownDataSource6 = pgcDoc.CreateDrillDownDataSource(0, i);
						if (pivotDrillDownDataSource6 != null && pivotDrillDownDataSource6.RowCount > 0)
						{
							num2 = Function.ParseDouble(pivotDrillDownDataSource6[0][pcRequirementQty]);
						}
					}
					if (Math.Abs(Function.ParseDouble(pivotDrillDownDataSource5[0][pcRequirementQty])) < 1E-05)
					{
						num += num2;
						if (flag && j == ColumnIndexContext)
						{
							pivotDrillDownDataSource5[0][pcRequirementQty] = num;
							if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0)
							{
								pivotDrillDownDataSource[0][pcRequirementQty] = num;
							}
						}
						else
						{
							num = 0.0;
						}
					}
					else
					{
						num = Function.ParseDouble(pivotDrillDownDataSource5[0][pcRequirementQty]);
					}
				}
				double num3 = ((pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2[0] != null) ? Function.ParseDouble(pivotDrillDownDataSource2[0][pcRequirementQty]) : 0.0);
				double num4 = 0.0;
				if (pivotDrillDownDataSource4 != null && pivotDrillDownDataSource4.RowCount > 0)
				{
					num4 = Function.ParseDouble(pivotDrillDownDataSource4[0][pcRequirementQty]);
				}
				if (pivotDrillDownDataSource3 != null && pivotDrillDownDataSource3[0] != null)
				{
					pivotDrillDownDataSource3[0][pcRequirementQty] = num3 + num4 + num2 - num;
				}
			}
			i += 4;
		}
		pgcDoc.RefreshData();
	}

	protected void LoadDataRemainQty()
	{
		int columnCount = pgcDoc.Cells.ColumnCount;
		int rowCount = pgcDoc.Cells.RowCount;
		for (int i = 0; i < rowCount; i++)
		{
			if (!Function.ToString(pgcDoc.GetFieldValue(pcPlanning, i)).StartsWith("1."))
			{
				continue;
			}
			for (int j = 1; j < columnCount - 1; j++)
			{
				PivotDrillDownDataSource pivotDrillDownDataSource = pgcDoc.CreateDrillDownDataSource(j, i + 2);
				PivotDrillDownDataSource pivotDrillDownDataSource2 = pgcDoc.CreateDrillDownDataSource(j, i);
				PivotDrillDownDataSource pivotDrillDownDataSource3 = pgcDoc.CreateDrillDownDataSource(j, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource4 = pgcDoc.CreateDrillDownDataSource(j - 1, i + 3);
				PivotDrillDownDataSource pivotDrillDownDataSource5 = pgcDoc.CreateDrillDownDataSource(j, i + 1);
				double num = 0.0;
				double num2 = 0.0;
				if (pivotDrillDownDataSource5 != null && pivotDrillDownDataSource5.RowCount > 0)
				{
					if (j == 1)
					{
						PivotDrillDownDataSource pivotDrillDownDataSource6 = pgcDoc.CreateDrillDownDataSource(0, i);
						if (pivotDrillDownDataSource6 != null && pivotDrillDownDataSource6.RowCount > 0)
						{
							num2 = Function.ParseDouble(pivotDrillDownDataSource6[0][pcRequirementQty]);
						}
					}
					num = Function.ParseDouble(pivotDrillDownDataSource5[0][pcRequirementQty]);
					if (pivotDrillDownDataSource != null && pivotDrillDownDataSource.RowCount > 0)
					{
						pivotDrillDownDataSource[0][pcRequirementQty] = num;
					}
				}
				double num3 = ((pivotDrillDownDataSource2 != null && pivotDrillDownDataSource2[0] != null) ? Function.ParseDouble(pivotDrillDownDataSource2[0][pcRequirementQty]) : 0.0);
				double num4 = 0.0;
				if (pivotDrillDownDataSource4 != null && pivotDrillDownDataSource4.RowCount > 0)
				{
					num4 = Function.ParseDouble(pivotDrillDownDataSource4[0][pcRequirementQty]);
				}
				if (pivotDrillDownDataSource3 != null && pivotDrillDownDataSource3[0] != null)
				{
					pivotDrillDownDataSource3[0][pcRequirementQty] = num3 + num4 + num2 - num;
				}
			}
			i += 4;
		}
		pgcDoc.RefreshData();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_DrillDownSourceCellMrp));
		this.btnOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.pgcDoc = new Apzon.Controls.Pivot.PivotGridControlEx();
		this.contextMenuStripDoc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.autoFillColumnToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.openSourceFormToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.pcSourceType = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pcSourceEntry = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pcSourceLine = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pcItemCode = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pcFatherCode = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pcCardCode = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pcVersion = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pcBaseLine = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pcPlanning = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pcRequirementDate = new DevExpress.XtraPivotGrid.PivotGridField();
		this.rDateEdit = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.pcRequirementShift = new DevExpress.XtraPivotGrid.PivotGridField();
		this.pcRequirementQty = new DevExpress.XtraPivotGrid.PivotGridField();
		this.rCalEdit = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.btnAutoFillData = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.pgcDoc).BeginInit();
		this.contextMenuStripDoc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.rDateEdit).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rDateEdit.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rCalEdit).BeginInit();
		base.SuspendLayout();
		this.btnOk.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOk.FieldName = null;
		this.btnOk.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOk.ImageOptions.Image");
		this.btnOk.IsSearchControl = false;
		this.btnOk.Location = new System.Drawing.Point(8, 498);
		this.btnOk.Name = "btnOk";
		this.btnOk.Size = new System.Drawing.Size(75, 23);
		this.btnOk.TabIndex = 0;
		this.btnOk.Text = "OK";
		this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
		this.pgcDoc.AllowPasteExcel = false;
		this.pgcDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pgcDoc.ClipboardData = null;
		this.pgcDoc.ContextMenuStrip = this.contextMenuStripDoc;
		this.pgcDoc.Fields.AddRange(new DevExpress.XtraPivotGrid.PivotGridField[12]
		{
			this.pcSourceType, this.pcSourceEntry, this.pcSourceLine, this.pcItemCode, this.pcFatherCode, this.pcCardCode, this.pcVersion, this.pcBaseLine, this.pcPlanning, this.pcRequirementDate,
			this.pcRequirementShift, this.pcRequirementQty
		});
		this.pgcDoc.FilterFieldControl = null;
		this.pgcDoc.FilterValueControl = null;
		this.pgcDoc.FormId = "oSourceCellMrp";
		this.pgcDoc.ItemId = "ODATA";
		this.pgcDoc.Location = new System.Drawing.Point(2, 3);
		this.pgcDoc.Name = "pgcDoc";
		this.pgcDoc.ObjectType = Apzon.Commons.ApzObjectTypes.oNone;
		this.pgcDoc.OptionsData.DataProcessingEngine = DevExpress.XtraPivotGrid.PivotDataProcessingEngine.LegacyOptimized;
		this.pgcDoc.OptionsView.ShowFilterHeaders = false;
		this.pgcDoc.OptionsView.ShowRowGrandTotals = false;
		this.pgcDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rCalEdit, this.rDateEdit });
		this.pgcDoc.Size = new System.Drawing.Size(1095, 491);
		this.pgcDoc.TabIndex = 1;
		this.pgcDoc.FieldValueDisplayText += new DevExpress.XtraPivotGrid.PivotFieldDisplayTextEventHandler(this.pgcDoc_FieldValueDisplayText);
		this.pgcDoc.CustomCellDisplayText += new DevExpress.XtraPivotGrid.PivotCellDisplayTextEventHandler(this.pgcDoc_CustomCellDisplayText);
		this.pgcDoc.CustomCellValue += new System.EventHandler<DevExpress.XtraPivotGrid.PivotCellValueEventArgs>(this.pgcDoc_CustomCellValue);
		this.pgcDoc.CustomAppearance += new DevExpress.XtraPivotGrid.PivotCustomAppearanceEventHandler(this.pgcDoc_CustomAppearance);
		this.pgcDoc.CustomDrawFieldValue += new DevExpress.XtraPivotGrid.PivotCustomDrawFieldValueEventHandler(this.pgcDoc_CustomDrawFieldValue);
		this.pgcDoc.CustomRowHeight += new System.EventHandler<DevExpress.XtraPivotGrid.PivotCustomRowHeightEventArgs>(this.pgcDoc_CustomRowHeight);
		this.pgcDoc.EditValueChanged += new DevExpress.XtraPivotGrid.EditValueChangedEventHandler(this.pgcDoc_EditValueChanged);
		this.pgcDoc.ShowingEditor += new System.EventHandler<DevExpress.XtraPivotGrid.CancelPivotCellEditEventArgs>(this.pgcDoc_ShowingEditor);
		this.contextMenuStripDoc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.autoFillColumnToolStripMenuItem, this.openSourceFormToolStripMenuItem });
		this.contextMenuStripDoc.Name = "contextMenuStripDoc";
		this.contextMenuStripDoc.Size = new System.Drawing.Size(174, 48);
		this.contextMenuStripDoc.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStripDoc_Opening);
		this.autoFillColumnToolStripMenuItem.Name = "autoFillColumnToolStripMenuItem";
		this.autoFillColumnToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
		this.autoFillColumnToolStripMenuItem.Text = "Auto Fill Column";
		this.autoFillColumnToolStripMenuItem.Click += new System.EventHandler(this.autoFillColumnToolStripMenuItem_Click);
		this.openSourceFormToolStripMenuItem.Name = "openSourceFormToolStripMenuItem";
		this.openSourceFormToolStripMenuItem.Size = new System.Drawing.Size(173, 22);
		this.openSourceFormToolStripMenuItem.Text = "Open Source Form";
		this.openSourceFormToolStripMenuItem.Click += new System.EventHandler(this.openSourceFormToolStripMenuItem_Click);
		this.pcSourceType.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pcSourceType.AreaIndex = 0;
		this.pcSourceType.Caption = "Source Type";
		this.pcSourceType.FieldName = "DemandType";
		this.pcSourceType.Name = "pcSourceType";
		this.pcSourceType.Width = 105;
		this.pcSourceEntry.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pcSourceEntry.AreaIndex = 1;
		this.pcSourceEntry.Caption = "Source Entry";
		this.pcSourceEntry.FieldName = "DemandEntry";
		this.pcSourceEntry.Name = "pcSourceEntry";
		this.pcSourceLine.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pcSourceLine.AreaIndex = 2;
		this.pcSourceLine.Caption = "Source Line";
		this.pcSourceLine.FieldName = "DemandLine";
		this.pcSourceLine.Name = "pcSourceLine";
		this.pcItemCode.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pcItemCode.AreaIndex = 3;
		this.pcItemCode.Caption = "Item Code";
		this.pcItemCode.FieldName = "ItemCode";
		this.pcItemCode.Name = "pcItemCode";
		this.pcFatherCode.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pcFatherCode.AreaIndex = 4;
		this.pcFatherCode.Caption = "Father Code";
		this.pcFatherCode.FieldName = "FatherCode";
		this.pcFatherCode.Name = "pcFatherCode";
		this.pcCardCode.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pcCardCode.AreaIndex = 5;
		this.pcCardCode.Caption = "CardCode";
		this.pcCardCode.FieldName = "CardCode";
		this.pcCardCode.Name = "pcCardCode";
		this.pcVersion.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pcVersion.AreaIndex = 6;
		this.pcVersion.Caption = "Version";
		this.pcVersion.FieldName = "BomVersion";
		this.pcVersion.Name = "pcVersion";
		this.pcBaseLine.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pcBaseLine.AreaIndex = 7;
		this.pcBaseLine.Caption = "Base Line";
		this.pcBaseLine.FieldName = "BaseLine";
		this.pcBaseLine.Name = "pcBaseLine";
		this.pcPlanning.Appearance.Cell.BackColor = System.Drawing.Color.FromArgb(224, 224, 224);
		this.pcPlanning.Appearance.Cell.Options.UseBackColor = true;
		this.pcPlanning.Area = DevExpress.XtraPivotGrid.PivotArea.RowArea;
		this.pcPlanning.AreaIndex = 8;
		this.pcPlanning.Caption = "Planning";
		this.pcPlanning.FieldName = "Planning";
		this.pcPlanning.Name = "pcPlanning";
		this.pcRequirementDate.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
		this.pcRequirementDate.AreaIndex = 0;
		this.pcRequirementDate.Caption = "Date";
		this.pcRequirementDate.FieldEdit = this.rDateEdit;
		this.pcRequirementDate.FieldName = "WorkDate";
		this.pcRequirementDate.Name = "pcRequirementDate";
		this.rDateEdit.AutoHeight = false;
		this.rDateEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDateEdit.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rDateEdit.DisplayFormat.FormatString = "";
		this.rDateEdit.Name = "rDateEdit";
		this.pcRequirementShift.Area = DevExpress.XtraPivotGrid.PivotArea.ColumnArea;
		this.pcRequirementShift.AreaIndex = 1;
		this.pcRequirementShift.Caption = "Shift";
		this.pcRequirementShift.FieldName = "Shift";
		this.pcRequirementShift.Name = "pcRequirementShift";
		this.pcRequirementQty.Appearance.Cell.BackColor = System.Drawing.Color.FromArgb(244, 245, 246);
		this.pcRequirementQty.Appearance.Cell.Options.UseBackColor = true;
		this.pcRequirementQty.Area = DevExpress.XtraPivotGrid.PivotArea.DataArea;
		this.pcRequirementQty.AreaIndex = 0;
		this.pcRequirementQty.Caption = "Quantity";
		this.pcRequirementQty.FieldEdit = this.rCalEdit;
		this.pcRequirementQty.FieldName = "Qty";
		this.pcRequirementQty.Name = "pcRequirementQty";
		this.rCalEdit.AutoHeight = false;
		this.rCalEdit.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rCalEdit.Name = "rCalEdit";
		this.btnAutoFillData.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnAutoFillData.FieldName = null;
		this.btnAutoFillData.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnAutoFillData.ImageOptions.Image");
		this.btnAutoFillData.IsSearchControl = false;
		this.btnAutoFillData.Location = new System.Drawing.Point(1010, 498);
		this.btnAutoFillData.Name = "btnAutoFillData";
		this.btnAutoFillData.Size = new System.Drawing.Size(87, 20);
		this.btnAutoFillData.TabIndex = 19;
		this.btnAutoFillData.Text = "Auto Fill";
		this.btnAutoFillData.Click += new System.EventHandler(this.btnAutoFillData_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(87, 498);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 20;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1100, 527);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnAutoFillData);
		base.Controls.Add(this.pgcDoc);
		base.Controls.Add(this.btnOk);
		base.FormId = "oSourceCellMrp";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_DrillDownSourceCellMrp";
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Drill Down Source Cell Mrp";
		base.Load += new System.EventHandler(this.frm_DrillDownSourceCellMrp_Load);
		((System.ComponentModel.ISupportInitialize)this.pgcDoc).EndInit();
		this.contextMenuStripDoc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.rDateEdit.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rDateEdit).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rCalEdit).EndInit();
		base.ResumeLayout(false);
	}
}
