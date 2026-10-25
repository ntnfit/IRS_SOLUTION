using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_UoMGroupDefinition : XtraFormEx
{
	private const string FormController = "UoMGroupDefinition";

	private const string TableName = "UGP1";

	public new string FormId;

	public int UgpEntry;

	public bool NewGr;

	private bool _isCancel;

	private bool _forceClose;

	private bool _isChanging;

	private DataTable _dtAlterUoM;

	private DataTable _dtBaseUoM;

	private DataRow _drowAlter;

	private DataRow _drowBase;

	public DataTable _dtSource = new DataTable();

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	public SimpleButtonEx btnCancel;

	public SimpleButtonEx btn_Save;

	private GridColumnEx gc_UDFFactor;

	private GridColumnEx gc_WeightFator;

	private GridColumnEx gc_AltQty;

	private GridColumnEx gc_AltUoM;

	private GridColumnEx gridColumn1;

	private GridColumnEx gc_BaseQty;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemButtonEdit rep_BaseUoM;

	private GridColumnEx gc_UomEntry;

	private RepositoryItemCalcEdit rep_AltQty;

	private GridColumnEx gc_BaseUoMCode;

	public bool _isUpdated { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_UoMGroupDefinition()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_UnitOfMeasureGroups"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_UnitOfMeasureGroups" && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
			}
			FormId = "oUoMGroupDefinition";
			gv_Doc.ItemId = "UGP1";
			gv_Doc.FormId = FormId;
			OnLoadAsync += frm_UoMGroupDefinition_Load;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await LoadData();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void KeyDownPaste(object sender, KeyEventArgs e)
	{
		PasteFromExcel(e, grd_Doc, gv_Doc);
	}

	private void PasteFromExcel(KeyEventArgs e, GridControl grd, GridView gv)
	{
		try
		{
			if (e.KeyCode != Keys.V || e.Modifiers != Keys.Control)
			{
				return;
			}
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			if (dataObject == null)
			{
				return;
			}
			string text = dataObject.GetData(DataFormats.UnicodeText).ToString();
			object data = text;
			if (dataObject.GetDataPresent(DataFormats.Text))
			{
				DataTable dataFromExcel = GetDataFromExcel(text);
				if (dataFromExcel != null && (dataFromExcel.Rows.Count > 1 || dataFromExcel.Columns.Count > 1))
				{
					data = dataFromExcel.Rows[0][0];
					int num = gv.FocusedRowHandle;
					GridColumn focusedColumn = gv.FocusedColumn;
					int num2 = ((gv.RowCount > dataFromExcel.Rows.Count) ? dataFromExcel.Rows.Count : gv.RowCount);
					int num3 = ((gv.Columns.Count > dataFromExcel.Columns.Count) ? dataFromExcel.Columns.Count : gv.Columns.Count);
					gv.BeginUpdate();
					for (int i = 0; i < num2; i++)
					{
						int colIndex = focusedColumn.VisibleIndex;
						for (int j = 0; j < num3; j++)
						{
							GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
							if (gridColumn != null && gridColumn.GroupIndex < 0)
							{
								gv.SetRowCellValue(num, gridColumn, dataFromExcel.Rows[i][j]);
							}
							colIndex++;
						}
						num++;
					}
					grd.RefreshDataSource();
					gv.EndUpdate();
					Clipboard.Clear();
				}
			}
			Clipboard.SetData(DataFormats.Text, data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetDataFromExcel(string clipboardData)
	{
		DataTable dataTable = new DataTable();
		try
		{
			List<string> list = new List<string>(clipboardData.Split('\n'));
			bool flag = true;
			if (list.Count > 0 && string.IsNullOrWhiteSpace(list[list.Count - 1]))
			{
				list.RemoveAt(list.Count - 1);
			}
			foreach (string item in list)
			{
				string text = item;
				if (text.EndsWith("\r"))
				{
					text = text.Substring(0, text.Length - "\r".Length);
				}
				string[] array = text.Split('\r', '\t');
				DataRow dataRow = dataTable.NewRow();
				if (flag)
				{
					int num = 0;
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						if (string.IsNullOrWhiteSpace(text2))
						{
							dataTable.Columns.Add($"[BLANK{num}]");
						}
						else if (!dataTable.Columns.Contains(text2))
						{
							dataTable.Columns.Add(text2);
						}
						else
						{
							dataTable.Columns.Add($"Column {num}");
						}
						num++;
					}
					for (int j = 0; j < array.Length && j < dataTable.Columns.Count; j++)
					{
						dataRow[j] = array[j];
					}
					dataTable.Rows.Add(dataRow);
					flag = false;
				}
				else
				{
					for (int k = 0; k < array.Length && k < dataTable.Columns.Count; k++)
					{
						dataRow[k] = array[k];
					}
					dataTable.Rows.Add(dataRow);
				}
			}
			return dataTable;
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	private async Task LoadData()
	{
		try
		{
			if (_dtSource.IsNotNull())
			{
				List<DataRow> list = (from row in _dtSource.AsEnumerable()
					where Function.ParseInt(row["UgpEntry"]) == UgpEntry
					select row).ToList();
				if (list.Count > 0)
				{
					grd_Doc.DataSource = list.CopyToDataTable();
					grd_Doc.RefreshDataSource();
					AddNew();
					return;
				}
			}
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UoMGroupDefinition"))
			{
				DataTable dataTable = await helper.GetAsync("LoadDoc", new
				{
					ugpEntry = UgpEntry
				});
				dataTable.SetDefaultAndMaxLen(gv_Doc.ItemId);
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
				AddNew();
			}
			_isUpdated = false;
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNew()
	{
		try
		{
			gv_Doc.BeginInsertRow();
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				int num = ((dataSourceTable.Rows.Count != 0) ? dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"])) : 0);
				string value = Function.ToString(gv_Doc.GetRowCellValue(0, "BaseUoMCode"));
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["UgpEntry"] = UgpEntry;
				dataRow["UomEntry"] = 0;
				dataRow["AltQty"] = DBNull.Value;
				dataRow["BaseQty"] = DBNull.Value;
				dataRow["LogInstanc"] = 0;
				dataRow["LineNum"] = num + 1;
				dataRow["WghtFactor"] = 0;
				dataRow["UdfFactor"] = -1;
				dataRow["BaseUoMCode"] = value;
				dataRow["UomCode"] = string.Empty;
				dataSourceTable.Rows.Add(dataRow.ItemArray);
				dataSourceTable.AcceptChanges();
				grd_Doc.DataSource = dataSourceTable;
				grd_Doc.RefreshDataSource();
				gv_Doc.EndInsertRow();
			}
			else
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("UgpEntry", typeof(int));
				dataTable.Columns.Add("UomEntry", typeof(int));
				dataTable.Columns.Add("AltQty", typeof(double));
				dataTable.Columns.Add("BaseQty", typeof(double));
				dataTable.Columns.Add("LogInstanc", typeof(int));
				dataTable.Columns.Add("LineNum", typeof(int));
				dataTable.Columns.Add("WghtFactor", typeof(int));
				dataTable.Columns.Add("UdfFactor", typeof(int));
				dataTable.Columns.Add("BaseUoMCode", typeof(string));
				dataTable.Columns.Add("UomCode", typeof(string));
				DataRow dataRow2 = dataTable.NewRow();
				dataRow2["UgpEntry"] = UgpEntry;
				dataRow2["UomEntry"] = 0;
				dataRow2["AltQty"] = DBNull.Value;
				dataRow2["BaseQty"] = DBNull.Value;
				dataRow2["LogInstanc"] = 0;
				dataRow2["LineNum"] = 1;
				dataRow2["WghtFactor"] = 0;
				dataRow2["UdfFactor"] = -1;
				dataRow2["BaseUoMCode"] = DBNull.Value;
				dataRow2["UomCode"] = string.Empty;
				dataTable.Rows.Add(dataRow2.ItemArray);
				grd_Doc.DataSource = dataTable;
				grd_Doc.RefreshDataSource();
			}
			gv_Doc.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Doc.EndInsertRow();
	}

	private async Task<string> SaveDoc()
	{
		_ = 1;
		try
		{
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataTable dataTable2 = dataTable.Copy();
				if (dataTable2.Rows.Count > 0 && string.IsNullOrEmpty(Function.ToString(dataTable2.Rows[dataTable2.Rows.Count - 1]["UomCode"])))
				{
					dataTable2.Rows.RemoveAt(dataTable2.Rows.Count - 1);
					dataTable2.AcceptChanges();
				}
				for (int i = 0; i < dataTable2.Rows.Count; i++)
				{
					decimal num = Function.ParseDecimal(dataTable2.Rows[i]["AltQty"]);
					decimal num2 = Function.ParseDecimal(dataTable2.Rows[i]["BaseQty"]);
					if (!(num > 0m) || !(num2 > 0m))
					{
						WinCommons.Error(Enums.MessageCode.EnterAPositiveValue, "");
						return "Error!";
					}
				}
				EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["UomCode"]))
					select t;
				if (source.Any())
				{
					dataTable = source.CopyToDataTable().Copy();
				}
				WinCommons._OpenProcessing();
				using HttpClientHelper<HttpResult> _help = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "UoMGroupDefinition");
				DataTable dataTable3 = dataTable.Copy();
				dataTable3.Columns.Remove("BaseUomCode");
				dataTable3.Columns.Remove("UomCode");
				dataTable3.AcceptChanges();
				HttpResult x = await _help.PostAsync("Save", dataTable3);
				if (string.IsNullOrEmpty(x.Message))
				{
					WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					FormMode = Enums.FormViewModeType.View;
					btn_Save.Enabled = true;
					btnCancel.Enabled = true;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, x.Message);
				}
				await LoadData();
				WinCommons._CloseProcessing();
				return x.Message;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
		return string.Empty;
	}

	public async Task ChooseFormListUoMs()
	{
		try
		{
			if (gv_Doc.FocusedColumn == gc_AltUoM)
			{
				DataTable dataTable = grd_Doc.DataSource as DataTable;
				if (dataTable.IsNotNull())
				{
					DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == 1);
					if (dataRow != null)
					{
						Function.ParseInt(dataRow["UomEntry"]);
					}
					string lstUoMEntry = string.Join(",", from r in (from r in dataTable.AsEnumerable()
							where !string.IsNullOrEmpty(Function.ToString(r["UomEntry"])) && Function.ParseInt(r["UomEntry"]) > 0
							select r).ToList()
						select Function.ToString(r["UomEntry"]));
					frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListAlterUoMs, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
					frm_CFL2.LstUoMEntry = lstUoMEntry;
					frm_CFL2.ShowDialog(this);
					DataRow chooseItem = frm_CFL2.ChooseItem;
					if (chooseItem != null)
					{
						gv_Doc.SetFocusedRowCellValue("UomCode", chooseItem["UomCode"]);
						gv_Doc.SetFocusedRowCellValue("UomEntry", chooseItem["UomEntry"]);
						grd_Doc.RefreshDataSource();
					}
				}
				return;
			}
			frm_CFL frm_CFL3 = new frm_CFL(Enums.TypeCFL.ListUoMs, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL3.ShowDialog(this);
			DataRow chooseItem2 = frm_CFL3.ChooseItem;
			if (chooseItem2 != null)
			{
				gv_Doc.SetFocusedRowCellValue("AltQty", 1);
				gv_Doc.SetFocusedRowCellValue("BaseQty", 1);
				gv_Doc.SetRowCellValue(0, "UomCode", chooseItem2["UomCode"]);
				gv_Doc.SetFocusedRowCellValue("UomEntry", chooseItem2["UomEntry"]);
				for (int num = 0; num < gv_Doc.RowCount; num++)
				{
					gv_Doc.SetRowCellValue(num, "BaseUoMCode", chooseItem2["UomCode"]);
				}
				grd_Doc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private async Task frm_UoMGroupDefinition_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			DataTable dtDoc = grd_Doc.DataSourceTable;
			if (string.IsNullOrEmpty(Function.ToString(dtDoc.Rows[dtDoc.Rows.Count - 1]["UomCode"])))
			{
				dtDoc.Rows.RemoveAt(dtDoc.Rows.Count - 1);
			}
			for (int i = 1; i < dtDoc.Rows.Count; i++)
			{
				if (Function.ToString(dtDoc.Rows[i]["UomCode"]) == Function.ToString(dtDoc.Rows[i]["BaseUomCode"]))
				{
					WinCommons.Error(Enums.MessageCode.AlternateUoMCannotEqualBaseUoM, "");
					return;
				}
			}
			if (dtDoc.AsEnumerable().FirstOrDefault((DataRow dataRow) => string.IsNullOrEmpty(Function.ToString(dataRow["BaseUomCode"]))) != null)
			{
				WinCommons.Error(Enums.MessageCode.EnterBaseUoM, "");
				return;
			}
			if (dtDoc.AsEnumerable().FirstOrDefault((DataRow dataRow) => !string.IsNullOrEmpty(Function.ToString(dataRow["UomCode"])) && Function.ParseDecimal(dataRow["AltQty"]) <= 0m && Function.ParseDecimal(dataRow["BaseQty"]) <= 0m) != null)
			{
				WinCommons.Error(Enums.MessageCode.EnterAPositiveValue, "");
				return;
			}
			if (_dtSource.IsNotNull())
			{
				List<DataRow> list = (from dataRow in _dtSource.AsEnumerable()
					where Function.ParseInt(dataRow["UgpEntry"]) == Function.ParseInt(dtDoc.Rows[0]["UgpEntry"])
					select dataRow).ToList();
				if (list.Count > 0)
				{
					foreach (DataRow item in list.ToList())
					{
						_dtSource.Rows.Remove(item);
					}
					_dtSource.AcceptChanges();
				}
			}
			foreach (DataRow row in dtDoc.Rows)
			{
				_dtSource.ImportRow(row);
				_dtSource.AcceptChanges();
			}
			Thread.Sleep(300);
			FormMode = Enums.FormViewModeType.View;
			if (FormMode == Enums.FormViewModeType.View && _forceClose)
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		_isUpdated = false;
		Close();
		_isCancel = false;
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (_isChanging)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Update;
			DataTable dataTable = grd_Doc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				if (e.RowHandle == dataTable.Rows.Count - 1 && e.Column != gc_BaseUoMCode)
				{
					AddNew();
				}
				if (e.Column == gc_AltUoM && _dtAlterUoM.IsNotNull())
				{
					_isChanging = true;
					gv_Doc.SetRowCellValue(e.RowHandle, "UomEntry", Function.ParseInt(_drowAlter["UomEntry"]));
					_isChanging = false;
				}
				if (e.Column == gc_BaseUoMCode)
				{
					grd_Doc.RefreshDataSource();
					if (_dtBaseUoM.IsNotNull())
					{
						_isChanging = true;
						gv_Doc.SetRowCellValue(e.RowHandle, "AltQty", 1);
						gv_Doc.SetRowCellValue(e.RowHandle, "BaseQty", 1);
						gv_Doc.SetRowCellValue(e.RowHandle, "UomEntry", Function.ParseInt(_drowBase["UomEntry"]));
						_isChanging = false;
					}
					GetAlterUoMs(dataTable);
				}
			}
			_isUpdated = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void GetAlterUoMs(DataTable dtCheck)
	{
		int uomEntry = 0;
		DataRow dataRow = dtCheck.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["LineNum"]) == 1);
		if (dataRow != null)
		{
			uomEntry = Function.ParseInt(dataRow["UomEntry"]);
		}
		frm_CFL frm = new frm_CFL(Enums.TypeCFL.ListAlterUoMs, false, "", "", Enums.ItemObjectType.NoItemType, null, "")
		{
			UomEntry = uomEntry
		};
		await Task.Run(() => frm.LoadForm());
		_dtAlterUoM = frm.gv_CFL.GridControl.DataSource as DataTable;
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private async void rep_BaseUoM_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				await ChooseFormListUoMs();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gv_Doc.FocusedColumn == gc_BaseUoMCode && Function.ParseInt(gv_Doc.GetFocusedRowCellValue("LineNum")) != 1)
			{
				e.Cancel = true;
			}
			if (Function.ParseInt(gv_Doc.GetFocusedRowCellValue("LineNum")) == 1 && gv_Doc.FocusedColumn != gc_BaseUoMCode)
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

	private async void frm_UoMGroupDefinition_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_isCancel)
			{
				return;
			}
			if (!_forceClose)
			{
				e.Cancel = true;
			}
			if (_forceClose)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSaveBeforeClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2))
				{
				case DialogResult.No:
					_forceClose = true;
					Close();
					break;
				case DialogResult.Yes:
					_forceClose = true;
					btn_Save_Click(null, null);
					break;
				}
			}
			else
			{
				_forceClose = true;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			AddNew();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (!(e.Column.Name == gc_BaseUoMCode.Name) || !grd_Doc.DataSourceTable.IsNotNull())
			{
				return;
			}
			for (int i = 0; i < gv_Doc.RowCount; i++)
			{
				if (e.RowHandle == 0)
				{
					e.Appearance.BackColor = StyleColor.Editable;
				}
				if (e.RowHandle == i)
				{
					e.Appearance.BackColor = StyleColor.GridDisable;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_RowStyle(object sender, RowStyleEventArgs e)
	{
		try
		{
			if (grd_Doc.DataSourceTable.IsNotNull() && e.RowHandle == 0)
			{
				e.Appearance.BackColor = StyleColor.GridDisable;
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
			int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("LineNum"));
			int num2 = grd_Doc.DataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineNum"]));
			if (num == num2)
			{
				return;
			}
			if (gv_Doc.RowCount > 1)
			{
				if (num != 1)
				{
					gv_Doc.DeleteSelectedRows();
					grd_Doc.DataSourceTable.AcceptChanges();
					FormMode = Enums.FormViewModeType.Update;
					_isUpdated = true;
				}
			}
			else if (gv_Doc.RowCount == 1 && (num != 1 || NewGr))
			{
				gv_Doc.DeleteSelectedRows();
				grd_Doc.DataSourceTable.AcceptChanges();
				grd_Doc.RefreshDataSource();
				AddNew();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			int entry = 0;
			if ((grd_Doc.DataSource as DataTable).IsNotNull() && sender is ColumnView columnView)
			{
				GridColumnEx column = (GridColumnEx)columnView.FocusedColumn;
				ValidateUomCode(e, column, entry);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ValidateUomCode(BaseContainerValidateEditorEventArgs e, GridColumnEx column, int entry)
	{
		if (column.FieldName == "UomCode")
		{
			if (_dtAlterUoM.IsNotNull())
			{
				DataRow dataRow = _dtAlterUoM.Copy().AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["UoMCode"]) == Function.ToString(e.Value));
				if (dataRow == null)
				{
					e.Valid = false;
					e.ErrorText = "Invalid UomCode!";
				}
				else
				{
					_drowAlter = dataRow;
				}
			}
			else
			{
				e.Valid = false;
				e.ErrorText = "You must define UomCode!";
			}
		}
		else
		{
			if (!(column.FieldName == "BaseUomCode"))
			{
				return;
			}
			if (_dtBaseUoM.IsNotNull())
			{
				DataRow dataRow2 = _dtBaseUoM.Copy().AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["UoMCode"]) == Function.ToString(e.Value));
				if (dataRow2 == null)
				{
					e.Valid = false;
					e.ErrorText = "Invalid UomCode!";
				}
				else
				{
					_drowBase = dataRow2;
				}
			}
			else
			{
				e.Valid = false;
				e.ErrorText = "You must define UomCode!";
			}
		}
	}

	private void gv_Doc_InvalidValueException(object sender, InvalidValueExceptionEventArgs e)
	{
		try
		{
			if (sender is ColumnView columnView)
			{
				e.ExceptionMode = ExceptionMode.DisplayError;
				e.WindowCaption = "Input Error";
				columnView.HideEditor();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_UoMGroupDefinition));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_UDFFactor = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_WeightFator = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AltQty = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_AltQty = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_AltUoM = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_BaseUoM = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.gc_BaseQty = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_UomEntry = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_BaseUoMCode = new Apzon.Controls.Grid.GridColumnEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AltQty).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BaseUoM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(3, 3);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repositoryItemTextEdit1, this.rep_BaseUoM, this.rep_AltQty });
		this.grd_Doc.Size = new System.Drawing.Size(556, 340);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(131, 48);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.addRowToolStripMenuItem.Text = "Add row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = "UomEntry";
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gc_UDFFactor, this.gc_WeightFator, this.gc_AltQty, this.gc_AltUoM, this.gridColumn1, this.gc_BaseQty, this.gc_UomEntry, this.gc_BaseUoMCode });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oUomGroupDefined";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 40;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = true;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.ItemId = "UGP1";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsCustomization.AllowFilter = false;
		this.gv_Doc.OptionsCustomization.AllowSort = false;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Doc_CustomDrawRowIndicator);
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.RowStyle += new DevExpress.XtraGrid.Views.Grid.RowStyleEventHandler(this.gv_Doc_RowStyle);
		this.gv_Doc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gv_Doc_ShowingEditor);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.gv_Doc.ValidatingEditor += new DevExpress.XtraEditors.Controls.BaseContainerValidateEditorEventHandler(this.gv_Doc_ValidatingEditor);
		this.gv_Doc.InvalidValueException += new DevExpress.XtraEditors.Controls.InvalidValueExceptionEventHandler(this.gv_Doc_InvalidValueException);
		this.gc_UDFFactor.AppearanceCell.BackColor = System.Drawing.Color.White;
		this.gc_UDFFactor.AppearanceCell.Options.UseBackColor = true;
		this.gc_UDFFactor.Caption = "UDF Factor";
		this.gc_UDFFactor.CurrencyDateField = null;
		this.gc_UDFFactor.CurrencyField = null;
		this.gc_UDFFactor.CurrencyFixed = null;
		this.gc_UDFFactor.CurrencyFixedRate = 0.0;
		this.gc_UDFFactor.CurrencyRateField = null;
		this.gc_UDFFactor.FieldName = "UdfFactor";
		this.gc_UDFFactor.Name = "gc_UDFFactor";
		this.gc_UDFFactor.OptionsColumn.ReadOnly = true;
		this.gc_UDFFactor.PrimaryKey = false;
		this.gc_UDFFactor.UnitField = null;
		this.gc_WeightFator.Caption = "Weight Fator";
		this.gc_WeightFator.CurrencyDateField = null;
		this.gc_WeightFator.CurrencyField = null;
		this.gc_WeightFator.CurrencyFixed = null;
		this.gc_WeightFator.CurrencyFixedRate = 0.0;
		this.gc_WeightFator.CurrencyRateField = null;
		this.gc_WeightFator.FieldName = "WghtFactor";
		this.gc_WeightFator.Name = "gc_WeightFator";
		this.gc_WeightFator.OptionsColumn.ReadOnly = true;
		this.gc_WeightFator.PrimaryKey = false;
		this.gc_WeightFator.UnitField = null;
		this.gc_AltQty.Caption = "Alt. Qty";
		this.gc_AltQty.ColumnEdit = this.rep_AltQty;
		this.gc_AltQty.CurrencyDateField = null;
		this.gc_AltQty.CurrencyField = null;
		this.gc_AltQty.CurrencyFixed = null;
		this.gc_AltQty.CurrencyFixedRate = 0.0;
		this.gc_AltQty.CurrencyRateField = null;
		this.gc_AltQty.DisplayFormat.FormatString = "n0";
		this.gc_AltQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc_AltQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gc_AltQty.FieldName = "AltQty";
		this.gc_AltQty.Name = "gc_AltQty";
		this.gc_AltQty.PrimaryKey = false;
		this.gc_AltQty.UnitField = null;
		this.gc_AltQty.Visible = true;
		this.gc_AltQty.VisibleIndex = 0;
		this.rep_AltQty.AutoHeight = false;
		this.rep_AltQty.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_AltQty.Name = "rep_AltQty";
		this.gc_AltUoM.Caption = "Alt. UoM";
		this.gc_AltUoM.ColumnEdit = this.rep_BaseUoM;
		this.gc_AltUoM.CurrencyDateField = null;
		this.gc_AltUoM.CurrencyField = null;
		this.gc_AltUoM.CurrencyFixed = null;
		this.gc_AltUoM.CurrencyFixedRate = 0.0;
		this.gc_AltUoM.CurrencyRateField = null;
		this.gc_AltUoM.FieldName = "UomCode";
		this.gc_AltUoM.Name = "gc_AltUoM";
		this.gc_AltUoM.PrimaryKey = false;
		this.gc_AltUoM.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gc_AltUoM.UnitField = null;
		this.gc_AltUoM.Visible = true;
		this.gc_AltUoM.VisibleIndex = 1;
		this.rep_BaseUoM.AutoHeight = false;
		this.rep_BaseUoM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_BaseUoM.Name = "rep_BaseUoM";
		this.rep_BaseUoM.UseReadOnlyAppearance = false;
		this.rep_BaseUoM.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_BaseUoM_ButtonClick);
		this.gridColumn1.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(219, 219, 219);
		this.gridColumn1.AppearanceCell.Options.UseBackColor = true;
		this.gridColumn1.Caption = "=";
		this.gridColumn1.ColumnEdit = this.repositoryItemTextEdit1;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.MaxWidth = 40;
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.AllowEdit = false;
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 2;
		this.gridColumn1.Width = 40;
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.repositoryItemTextEdit1.NullText = "=";
		this.repositoryItemTextEdit1.ReadOnly = true;
		this.gc_BaseQty.Caption = "Base Qty";
		this.gc_BaseQty.ColumnEdit = this.rep_AltQty;
		this.gc_BaseQty.CurrencyDateField = null;
		this.gc_BaseQty.CurrencyField = null;
		this.gc_BaseQty.CurrencyFixed = null;
		this.gc_BaseQty.CurrencyFixedRate = 0.0;
		this.gc_BaseQty.CurrencyRateField = null;
		this.gc_BaseQty.DisplayFormat.FormatString = "n0";
		this.gc_BaseQty.DisplayFormat.FormatType = DevExpress.Utils.FormatType.Numeric;
		this.gc_BaseQty.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Qty;
		this.gc_BaseQty.FieldName = "BaseQty";
		this.gc_BaseQty.Name = "gc_BaseQty";
		this.gc_BaseQty.PrimaryKey = false;
		this.gc_BaseQty.UnitField = null;
		this.gc_BaseQty.Visible = true;
		this.gc_BaseQty.VisibleIndex = 3;
		this.gc_UomEntry.Caption = "UomEntry";
		this.gc_UomEntry.CurrencyDateField = null;
		this.gc_UomEntry.CurrencyField = null;
		this.gc_UomEntry.CurrencyFixed = null;
		this.gc_UomEntry.CurrencyFixedRate = 0.0;
		this.gc_UomEntry.CurrencyRateField = null;
		this.gc_UomEntry.FieldName = "UomEntry";
		this.gc_UomEntry.Name = "gc_UomEntry";
		this.gc_UomEntry.OptionsColumn.ReadOnly = true;
		this.gc_UomEntry.PrimaryKey = false;
		this.gc_UomEntry.UnitField = null;
		this.gc_BaseUoMCode.Caption = "Base UoM";
		this.gc_BaseUoMCode.ColumnEdit = this.rep_BaseUoM;
		this.gc_BaseUoMCode.CurrencyDateField = null;
		this.gc_BaseUoMCode.CurrencyField = null;
		this.gc_BaseUoMCode.CurrencyFixed = null;
		this.gc_BaseUoMCode.CurrencyFixedRate = 0.0;
		this.gc_BaseUoMCode.CurrencyRateField = null;
		this.gc_BaseUoMCode.FieldName = "BaseUoMCode";
		this.gc_BaseUoMCode.Name = "gc_BaseUoMCode";
		this.gc_BaseUoMCode.PrimaryKey = false;
		this.gc_BaseUoMCode.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gc_BaseUoMCode.UnitField = null;
		this.gc_BaseUoMCode.Visible = true;
		this.gc_BaseUoMCode.VisibleIndex = 4;
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(106, 349);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 77;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 349);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(99, 20);
		this.btn_Save.TabIndex = 78;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(562, 372);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_Doc);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_UoMGroupDefinition";
		this.Text = "Group Definition";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_UoMGroupDefinition_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_AltQty).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_BaseUoM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
