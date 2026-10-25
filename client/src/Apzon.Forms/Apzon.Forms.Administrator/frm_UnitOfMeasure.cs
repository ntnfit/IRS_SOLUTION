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
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
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

public class frm_UnitOfMeasure : XtraFormEx
{
	private const string FormController = "UnitOfMeasure";

	private const string tableName = "OUOM";

	private bool _cellEdited;

	private bool _cellError;

	private bool _isCancel;

	private bool _forceClose;

	private int _maxEntry;

	private IContainer components;

	private GridViewEx gvDoc;

	private RepositoryItemButtonEdit rep_GLAcct;

	private GridControlEx grdDoc;

	public SimpleButtonEx btnCancel;

	public SimpleButtonEx btn_Save;

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

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private GridColumnEx gridColumn17;

	private GridColumnEx gridColumn18;

	private GridColumnEx gridColumn19;

	private GridColumnEx gridColumn20;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumn22;

	private GridColumnEx gc_vol;

	private GridColumnEx gc_volUom;

	private GridColumnEx gridColumn25;

	private GridColumnEx gridColumn26;

	private GridColumnEx gridColumn27;

	private GridColumnEx gridColumn28;

	private GridColumnEx gridColumn29;

	private RepositoryItemLookUpEdit rep_UOM;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private RepositoryItemLookUpEdit rep_VolUoM;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private GridColumnEx gc_Length1Disp;

	private GridColumnEx gc_Width1Disp;

	private GridColumnEx gc_Height1Disp;

	private GridColumnEx gc_Weight1Disp;

	private RepositoryItemCalcEdit repositoryItemCalcEdit1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx2;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	protected override void SetFormMode(Enums.FormViewModeType value)
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
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_UnitOfMeasure()
	{
		InitializeComponent();
		gvDoc.KeyDown += KeyDownPaste;
		grdDoc.EditorKeyDown += KeyDownPaste;
		FormId = "oUnitOfMeasure";
		gvDoc.ItemId = "OUOM";
		gvDoc.FormId = FormId;
		OnLoadAsync += frm_UnitOfMeasure_Load;
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			FormMode = Enums.FormViewModeType.View;
			using (HttpClientHelper<DataTable> helper_1 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasure"))
			{
				DataTable dataTable = await helper_1.GetAsync("LoadDoc");
				dataTable.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
				AddNew();
			}
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "ItemsMaster");
			DataTable dataSource = await helper.GetAsync("GetVolumeUnit");
			rep_VolUoM.Columns.Clear();
			rep_VolUoM.Columns.Add(new LookUpColumnInfo
			{
				Caption = "",
				FieldName = "Name"
			});
			rep_VolUoM.DisplayMember = "Name";
			rep_VolUoM.ValueMember = "Code";
			rep_VolUoM.NullText = "";
			rep_VolUoM.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
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
		PasteFromExcel(e, grdDoc, gvDoc);
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
					int count = dataFromExcel.Rows.Count;
					int num2 = ((gv.Columns.Count > dataFromExcel.Columns.Count) ? dataFromExcel.Columns.Count : gv.Columns.Count);
					gv.BeginUpdate();
					for (int i = 0; i < count; i++)
					{
						int colIndex = focusedColumn.VisibleIndex;
						for (int j = 0; j < num2; j++)
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

	private async Task<string> SaveData()
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = (grdDoc.DataSource as DataTable).Copy();
			dataTable.Columns.Remove("Length1Disp");
			dataTable.Columns.Remove("Width1Disp");
			dataTable.Columns.Remove("Height1Disp");
			dataTable.Columns.Remove("Weight1Disp");
			dataTable.AcceptChanges();
			if (dataTable.IsNotNull())
			{
				if (string.IsNullOrEmpty(Function.ToString(dataTable.Rows[dataTable.Rows.Count - 1]["UomCode"])))
				{
					dataTable.Rows.RemoveAt(dataTable.Rows.Count - 1);
					dataTable.AcceptChanges();
				}
				if (dataTable.AsEnumerable().FirstOrDefault((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["UomCode"]))) != null)
				{
					WinCommons.Error(Enums.MessageCode.UomCodeCannotBeEmpty, "");
					return "Error";
				}
				if ((from dataRow in dataTable.AsEnumerable()
					group dataRow by Function.ToString(dataRow["UomCode"])).Any((IGrouping<string, DataRow> source2) => source2.Skip(1).Any()))
				{
					WinCommons.Error(Enums.MessageCode.UomCodeCannotBeDuplicated, "");
					return "Error";
				}
				EnumerableRowCollection<DataRow> source = from t in dataTable.AsEnumerable()
					where !string.IsNullOrEmpty(Function.ToString(t["UomCode"]))
					select t;
				if (source.Any())
				{
					dataTable = source.CopyToDataTable();
				}
				using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "UnitOfMeasure");
				HttpResult x = await helper.PostAsync("Save", dataTable);
				if (string.IsNullOrEmpty(x.Message))
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					await LoadData();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, x.Message);
				}
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

	private void AddNew()
	{
		try
		{
			gvDoc.BeginInsertRow();
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				_maxEntry = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["UomEntry"])) + 1;
				DataRow dataRow = dataTable.NewRow();
				dataRow["UomEntry"] = ((_maxEntry + 1 <= 0) ? 1 : (_maxEntry + 1));
				dataRow["UomCode"] = string.Empty;
				dataRow["UomName"] = string.Empty;
				dataRow["Locked"] = "N";
				dataRow["DataSource"] = "N";
				dataRow["UserSign"] = GlobalData.UserID;
				dataRow["LogInstanc"] = 0;
				dataRow["UserSign2"] = DBNull.Value;
				dataRow["UpdateDate"] = DBNull.Value;
				dataRow["CreateDate"] = DateTime.Now;
				dataRow["Length1"] = 0;
				dataRow["Len1Unit"] = 0;
				dataRow["Length1Disp"] = "";
				dataRow["length2"] = 0;
				dataRow["Len2Unit"] = 0;
				dataRow["Width1"] = 0;
				dataRow["Wdth1Unit"] = 0;
				dataRow["Width1Disp"] = "";
				dataRow["Width2"] = 0;
				dataRow["Wdth2Unit"] = 0;
				dataRow["Height1"] = 0;
				dataRow["Hght1Unit"] = 0;
				dataRow["Height1Disp"] = "";
				dataRow["Height2"] = 0;
				dataRow["Hght2Unit"] = 0;
				dataRow["Volume"] = DBNull.Value;
				dataRow["VolUnit"] = 4;
				dataRow["Weight1"] = 0;
				dataRow["WghtUnit"] = 0;
				dataRow["Weight1Disp"] = "";
				dataRow["Weight2"] = 0;
				dataRow["Wght2Unit"] = 0;
				dataRow["IntSymbol"] = string.Empty;
				dataTable.Rows.Add(dataRow);
				dataTable.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable;
				grdDoc.RefreshDataSource();
			}
			else
			{
				DataTable dataTable2 = new DataTable();
				dataTable2.Columns.Add("UomEntry", typeof(int));
				dataTable2.Columns.Add("UomCode", typeof(string));
				dataTable2.Columns.Add("UomName", typeof(string));
				dataTable2.Columns.Add("Locked", typeof(string));
				dataTable2.Columns.Add("DataSource", typeof(string));
				dataTable2.Columns.Add("UserSign", typeof(int));
				dataTable2.Columns.Add("LogInstanc", typeof(int));
				dataTable2.Columns.Add("UserSign2", typeof(int));
				dataTable2.Columns.Add("UpdateDate", typeof(DateTime));
				dataTable2.Columns.Add("CreateDate", typeof(DateTime));
				dataTable2.Columns.Add("Length1", typeof(double));
				dataTable2.Columns.Add("Len1Unit", typeof(int));
				dataTable2.Columns.Add("Length1Disp", typeof(string));
				dataTable2.Columns.Add("length2", typeof(double));
				dataTable2.Columns.Add("Len2Unit", typeof(int));
				dataTable2.Columns.Add("Width1", typeof(double));
				dataTable2.Columns.Add("Wdth1Unit", typeof(int));
				dataTable2.Columns.Add("Width1Disp", typeof(string));
				dataTable2.Columns.Add("Width2", typeof(double));
				dataTable2.Columns.Add("Wdth2Unit", typeof(int));
				dataTable2.Columns.Add("Height1", typeof(double));
				dataTable2.Columns.Add("Hght1Unit", typeof(int));
				dataTable2.Columns.Add("Height1Disp", typeof(string));
				dataTable2.Columns.Add("Height2", typeof(double));
				dataTable2.Columns.Add("Hght2Unit", typeof(int));
				dataTable2.Columns.Add("Volume", typeof(double));
				dataTable2.Columns.Add("VolUnit", typeof(int));
				dataTable2.Columns.Add("Weight1", typeof(double));
				dataTable2.Columns.Add("WghtUnit", typeof(int));
				dataTable2.Columns.Add("Weight1Disp", typeof(string));
				dataTable2.Columns.Add("Weight2", typeof(double));
				dataTable2.Columns.Add("Wght2Unit", typeof(int));
				dataTable2.Columns.Add("IntSymbol", typeof(string));
				dataTable2.Rows.Add(1, string.Empty, string.Empty, "N", "N", GlobalData.UserID, 0, DBNull.Value, DBNull.Value, DateTime.Now, 0, 0, "", 0, 0, 0, 0, "", 0, 0, 0, 0, "", 0, 0, DBNull.Value, 4, 0, 0, "", 0, 0, string.Empty);
				dataTable2.SetDefaultAndMaxLen(gvDoc.ItemId);
				grdDoc.DataSource = dataTable2;
				grdDoc.RefreshDataSource();
			}
			gvDoc.EndInsertRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		gvDoc.EndInsertRow();
	}

	private async Task<DataTable> GetDtAddNew()
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasure");
			DataTable dataTable2 = await helper.GetAsync("GetAddNewRow", new
			{
				tableName = "OUOM",
				codeMax = dataTable.Rows[dataTable.Rows.Count - 1]["UomEntry"]
			});
			if (dataTable2.IsNotNull())
			{
				return dataTable2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	private double SetLengthDataToSource(string checkValidQuery, string dispFName, string len1Fname, string unit1Fname, string len2Fname, string unit2Fname, string defaultValue, string basicValue, int rowHandle)
	{
		try
		{
			string text = "";
			string text2 = "";
			string text3 = Function.ToString(gvDoc.GetRowCellValue(rowHandle, dispFName)).Trim().Replace(" ", string.Empty);
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			for (int i = 0; i < text3.Length; i++)
			{
				if (Function.ParseInt(text3.Substring(i, 1)) != 0 || (Function.ParseInt(text3.Substring(i, 1)) == 0 && (Function.ToString(text3.Substring(i, 1)) == "0" || Function.ToString(text3.Substring(i, 1)) == ".")))
				{
					num = i;
					break;
				}
			}
			for (int j = num; j < text3.Length; j++)
			{
				if (Function.ParseInt(text3.Substring(j, 1)) == 0 && Function.ToString(text3.Substring(j, 1)) != "0" && Function.ToString(text3.Substring(j, 1)) != ".")
				{
					num3 = j - 1;
					num5 = j;
					break;
				}
			}
			if (num5 == 0)
			{
				num3 = text3.Length - 1;
				text = defaultValue;
			}
			else if (num5 < text3.Length)
			{
				for (int k = num5; k < text3.Length; k++)
				{
					if (Function.ParseInt(text3.Substring(k, 1)) != 0 || (Function.ParseInt(text3.Substring(k, 1)) == 0 && (Function.ToString(text3.Substring(k, 1)) == "0" || Function.ToString(text3.Substring(k, 1)) == ".")))
					{
						num7 = k - 1;
						num2 = k;
						break;
					}
				}
			}
			if (num7 == 0)
			{
				num7 = text3.Length - 1;
			}
			else
			{
				for (int l = num2; l < text3.Length; l++)
				{
					if (Function.ParseInt(text3.Substring(l, 1)) == 0 && Function.ToString(text3.Substring(l, 1)) != "0" && Function.ToString(text3.Substring(l, 1)) != ".")
					{
						num4 = l - 1;
						num6 = l;
						break;
					}
				}
				if (num6 == 0)
				{
					num4 = text3.Length - 1;
					text2 = defaultValue;
				}
				else if (num6 < text3.Length)
				{
					for (int m = num6; m < text3.Length; m++)
					{
						if (Function.ParseInt(text3.Substring(m, 1)) != 0)
						{
							num8 = m - 1;
							break;
						}
					}
					if (num8 == 0 && num7 != text3.Length)
					{
						num8 = text3.Length - 1;
					}
				}
			}
			double num9 = Math.Round(Function.ParseDouble(text3.Substring(num, num3 - num + 1)), 2);
			if (string.IsNullOrEmpty(text))
			{
				text = text3.Substring(num5, num7 - num5 + 1);
			}
			if (num3 == text3.Length - 1 || num7 == text3.Length - 1)
			{
				if (Math.Abs(num9) < double.Epsilon)
				{
					_cellEdited = true;
					gvDoc.SetRowCellValue(rowHandle, dispFName, "");
					gvDoc.SetRowCellValue(rowHandle, len1Fname, Function.ParseDouble(num9));
					_cellEdited = false;
					return 0.0;
				}
				DataTable dataTable;
				DataTable dataTable2;
				using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasure"))
				{
					dataTable = httpClientHelper.Get(checkValidQuery, new
					{
						unitDisply = text
					});
					dataTable2 = httpClientHelper.Get(checkValidQuery, new
					{
						unitDisply = basicValue
					});
					if (!dataTable.IsNotNull() || dataTable.Rows.Count == 0 || text.Length > 2)
					{
						WinCommons.Error(Enums.MessageCode.InvalidUomWithCode, "", text);
						return -1.0;
					}
					if (!dataTable2.IsNotNull() || dataTable2.Rows.Count == 0 || basicValue.Length > 2)
					{
						WinCommons.Error(Enums.MessageCode.InvalidBasicUnitParam, "", text);
						return -1.0;
					}
				}
				_cellEdited = true;
				gvDoc.SetRowCellValue(rowHandle, dispFName, num9 + text);
				gvDoc.SetRowCellValue(rowHandle, len1Fname, Function.ParseDouble(num9));
				gvDoc.SetRowCellValue(rowHandle, unit1Fname, Function.ParseInt(dataTable.Rows[0]["UnitCode"]));
				gvDoc.SetRowCellValue(rowHandle, len2Fname, 0);
				gvDoc.SetRowCellValue(rowHandle, unit2Fname, Function.ParseInt(dataTable2.Rows[0]["UnitCode"]));
				_cellEdited = false;
				if (dataTable.Columns.Contains("SizeInMM"))
				{
					return Function.ParseDouble(num9) * Function.ParseDouble(dataTable.Rows[0]["SizeInMM"]) / Function.ParseDouble(dataTable2.Rows[0]["SizeInMM"]);
				}
			}
			else
			{
				double num10 = Math.Round(Function.ParseDouble(text3.Substring(num2, num4 - num2 + 1)), 2);
				if (string.IsNullOrEmpty(text2))
				{
					text2 = text3.Substring(num6, num8 - num6 + 1);
				}
				if (Math.Abs(num9) < double.Epsilon || Math.Abs(num10) < double.Epsilon)
				{
					_cellEdited = true;
					gvDoc.SetRowCellValue(rowHandle, dispFName, "");
					gvDoc.SetRowCellValue(rowHandle, len1Fname, Function.ParseDouble(num9));
					gvDoc.SetRowCellValue(rowHandle, len2Fname, Function.ParseDouble(num10));
					_cellEdited = false;
					return 0.0;
				}
				DataTable dataTable3;
				DataTable dataTable4;
				DataTable dataTable5;
				using (HttpClientHelper<DataTable> httpClientHelper2 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasure"))
				{
					dataTable3 = httpClientHelper2.Get(checkValidQuery, new
					{
						unitDisply = text
					});
					dataTable4 = httpClientHelper2.Get(checkValidQuery, new
					{
						unitDisply = text2
					});
					dataTable5 = httpClientHelper2.Get(checkValidQuery, new
					{
						unitDisply = basicValue
					});
					if (!dataTable3.IsNotNull() || dataTable3.Rows.Count == 0 || text.Length > 2)
					{
						WinCommons.Error(Enums.MessageCode.InvalidUomWithCode, "", text);
						return -1.0;
					}
					if (!dataTable4.IsNotNull() || dataTable4.Rows.Count == 0 || text2.Length > 2)
					{
						WinCommons.Error(Enums.MessageCode.InvalidUomWithCode, "", text2);
						return -1.0;
					}
					if (!dataTable5.IsNotNull() || dataTable5.Rows.Count == 0 || basicValue.Length > 2)
					{
						WinCommons.Error(Enums.MessageCode.InvalidBasicUnitParam, "", text2);
						return -1.0;
					}
				}
				_cellEdited = true;
				gvDoc.SetRowCellValue(rowHandle, dispFName, num9 + text + " " + num10 + text2);
				gvDoc.SetRowCellValue(rowHandle, len1Fname, Function.ParseDouble(num9));
				gvDoc.SetRowCellValue(rowHandle, unit1Fname, Function.ParseInt(dataTable3.Rows[0]["UnitCode"]));
				gvDoc.SetRowCellValue(rowHandle, len2Fname, Function.ParseDouble(num10));
				gvDoc.SetRowCellValue(rowHandle, unit2Fname, Function.ParseInt(dataTable4.Rows[0]["UnitCode"]));
				_cellEdited = false;
				if (dataTable3.Columns.Contains("SizeInMM") && dataTable4.Columns.Contains("SizeInMM"))
				{
					return Function.ParseDouble(num9) * Function.ParseDouble(dataTable3.Rows[0]["SizeInMM"]) / Function.ParseDouble(dataTable5.Rows[0]["SizeInMM"]) + Function.ParseDouble(num10) * Function.ParseDouble(dataTable4.Rows[0]["SizeInMM"]) / Function.ParseDouble(dataTable5.Rows[0]["SizeInMM"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return 0.0;
	}

	private async Task frm_UnitOfMeasure_Load(object sender, EventArgs e)
	{
		await LoadData();
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (_cellError)
				{
					WinCommons.Error(Enums.MessageCode.InvalidUomInDataPleaseCheckBeforeSaving, "");
					return;
				}
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveData();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
		_isCancel = false;
	}

	private void gvDoc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
	}

	private async void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			double exchangeVolum = 0.0;
			if (!_cellEdited && (e.Column.FieldName == "Length1Disp" || e.Column.FieldName == "Width1Disp" || e.Column.FieldName == "Height1Disp"))
			{
				double num = SetLengthDataToSource("CheckLengthUnit", "Length1Disp", "Length1", "Len1Unit", "length2", "Len2Unit", "m", "mm", e.RowHandle);
				double num2 = SetLengthDataToSource("CheckLengthUnit", "Width1Disp", "Width1", "Wdth1Unit", "Width2", "Wdth2Unit", "m", "mm", e.RowHandle);
				double num3 = SetLengthDataToSource("CheckLengthUnit", "Height1Disp", "Height1", "Hght1Unit", "Height2", "Hght2Unit", "m", "mm", e.RowHandle);
				if (num < 0.0 || num2 < 0.0 || num3 < 0.0)
				{
					_cellEdited = true;
					gvDoc.SetRowCellValue(e.RowHandle, "Volume", DBNull.Value);
					_cellError = true;
					_cellEdited = false;
				}
				else if (Math.Abs(num) > 0.0 && Math.Abs(num2) > 0.0 && Math.Abs(num3) > 0.0)
				{
					double volum = num * num2 * num3;
					using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasure");
					DataTable dataTable = await _help.GetAsync("GetAllLengthUnits");
					DataRow[] array = dataTable.Select("UnitCode = 1");
					DataRow[] array2 = dataTable.Select("UnitCode = " + Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, "VolUnit")));
					if (array.Length != 0 && array2.Length != 0)
					{
						exchangeVolum = Function.ParseDouble(volum) * Function.ParseDouble(Math.Pow(Function.ParseDouble(array[0]["SizeInMM"]) / Function.ParseDouble(array2[0]["SizeInMM"]), 3.0));
						_cellEdited = true;
						gvDoc.SetRowCellValue(e.RowHandle, "Volume", exchangeVolum);
						_cellError = false;
						_cellEdited = false;
					}
				}
				else
				{
					_cellEdited = true;
					gvDoc.SetRowCellValue(e.RowHandle, "Volume", DBNull.Value);
					_cellError = false;
					_cellEdited = false;
				}
			}
			if (!_cellEdited && e.Column.FieldName == "Volume")
			{
				double num4 = Function.ParseDouble(gvDoc.GetRowCellValue(e.RowHandle, "Volume"));
				if (Math.Abs(exchangeVolum - num4) > 0.0)
				{
					_cellEdited = true;
					gvDoc.SetRowCellValue(e.RowHandle, "Length1Disp", "");
					gvDoc.SetRowCellValue(e.RowHandle, "Length1", 0);
					gvDoc.SetRowCellValue(e.RowHandle, "Len1Unit", "");
					gvDoc.SetRowCellValue(e.RowHandle, "length2", 0);
					gvDoc.SetRowCellValue(e.RowHandle, "Len2Unit", "");
					gvDoc.SetRowCellValue(e.RowHandle, "Width1Disp", "");
					gvDoc.SetRowCellValue(e.RowHandle, "Width1", 0);
					gvDoc.SetRowCellValue(e.RowHandle, "Wdth1Unit", "");
					gvDoc.SetRowCellValue(e.RowHandle, "Width2", 0);
					gvDoc.SetRowCellValue(e.RowHandle, "Wdth2Unit", "");
					gvDoc.SetRowCellValue(e.RowHandle, "Height1Disp", "");
					gvDoc.SetRowCellValue(e.RowHandle, "Height1", 0);
					gvDoc.SetRowCellValue(e.RowHandle, "Hght1Unit", "");
					gvDoc.SetRowCellValue(e.RowHandle, "Height2", 0);
					gvDoc.SetRowCellValue(e.RowHandle, "Hght2Unit", "");
					gvDoc.SetRowCellValue(e.RowHandle, "Volume", num4);
					_cellEdited = false;
				}
			}
			if (e.Column.FieldName == "Weight1Disp" && !_cellEdited)
			{
				SetLengthDataToSource("CheckWidthUnit", "Weight1Disp", "Weight1", "WghtUnit", "Weight2", "Wght2Unit", "kg", "mg", e.RowHandle);
			}
			if (grdDoc.DataSource is DataTable source)
			{
				int num5 = source.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["UomEntry"]));
				if (Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, "UomEntry")) == num5 && (!string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "UomCode"))) || !string.IsNullOrEmpty(Function.ToString(gvDoc.GetRowCellValue(e.RowHandle, "UomName")))))
				{
					AddNew();
				}
			}
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void deleteRowToolStripMenuItem_Click_1(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = grdDoc.DataSource as DataTable;
			if (!dataTable.IsNotNull())
			{
				return;
			}
			int num = Function.ParseInt(gvDoc.GetFocusedRowCellValue("UomEntry"));
			if (num == _maxEntry)
			{
				return;
			}
			if (Function.ToString(gvDoc.GetFocusedRowCellValue("UomCode")) == "" && Function.ToString(gvDoc.GetFocusedRowCellValue("UomName")) == "")
			{
				FormMode = Enums.FormViewModeType.Update;
				gvDoc.DeleteSelectedRows();
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) != DialogResult.Yes)
				{
					return;
				}
				if (Function.ParseInt(gvDoc.GetFocusedRowCellValue("UomEntry")) < 0)
				{
					WinCommons.Error(Enums.MessageCode.CannotDeleteDefaultRecord, "");
					return;
				}
				using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasureGroups"))
				{
					if (dataTable != null)
					{
						DataTable dataTable2 = await helper.GetAsync("CheckUoMInGroups", new
						{
							uomEntry = num
						});
						if (dataTable2.IsNotNull() || dataTable2.Rows.Count > 0)
						{
							WinCommons.Error(Enums.MessageCode.CannotDeleteUomAsignedToUomGroup, "", Function.ToString(gvDoc.GetFocusedRowCellValue("UomCode")));
							return;
						}
					}
				}
				FormMode = Enums.FormViewModeType.Update;
				gvDoc.DeleteSelectedRows();
				grdDoc.DataSourceTable.AcceptChanges();
				grdDoc.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_UnitOfMeasure_FormClosing(object sender, FormClosingEventArgs e)
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
					if (string.IsNullOrEmpty(await SaveData()))
					{
						_forceClose = true;
						Close();
					}
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

	private void gvDoc_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (gvDoc.FocusedColumn == gridColumn2 && Function.ParseInt(gvDoc.GetFocusedRowCellValue("UomEntry")) == -1)
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

	private void gvDoc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column == gridColumn2 && Function.ParseInt(gvDoc.GetRowCellValue(e.RowHandle, "UomEntry")) == -1)
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

	private void rep_VolUoM_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void rep_VolUoM_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UnitOfMeasure");
			DataTable dataTable = await _help.GetAsync("GetAllLengthUnits");
			DataRow[] array = dataTable.Select("UnitCode = " + e.OldValue);
			DataRow[] array2 = dataTable.Select("UnitCode = " + e.NewValue);
			double num = Function.ParseDouble(gvDoc.GetFocusedRowCellValue("Volume"));
			if (array.Length != 0 && array2.Length != 0)
			{
				double num2 = Function.ParseDouble(num) * Function.ParseDouble(Math.Pow(Function.ParseDouble(array[0]["SizeInMM"]) / Function.ParseDouble(array2[0]["SizeInMM"]), 3.0));
				_cellEdited = true;
				gvDoc.SetFocusedRowCellValue("Volume", num2);
				_cellEdited = false;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_UnitOfMeasure));
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx2 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Length1Disp = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Width1Disp = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Height1Disp = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_vol = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCalcEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gc_volUom = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_VolUoM = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Weight1Disp = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn27 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn28 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn29 = new Apzon.Controls.Grid.GridColumnEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.rep_GLAcct = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.rep_UOM = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_VolUoM).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.rep_GLAcct).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UOM).BeginInit();
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
		this.repositoryItemCalcEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx4.IsSearchControl = false;
		this.repositoryItemCalcEditEx4.Name = "repositoryItemCalcEditEx4";
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = "UomEntry";
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[33]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumn11, this.gridColumn12, this.gc_Length1Disp, this.gridColumn13, this.gridColumn14, this.gridColumn15, this.gridColumn16, this.gc_Width1Disp, this.gridColumn17, this.gridColumn18,
			this.gridColumn19, this.gridColumn20, this.gc_Height1Disp, this.gridColumn21, this.gridColumn22, this.gc_vol, this.gc_volUom, this.gridColumn25, this.gridColumn26, this.gc_Weight1Disp,
			this.gridColumn27, this.gridColumn28, this.gridColumn29
		});
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = "oUnitOfMeasure";
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 40;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = true;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = "OUOM";
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsSelection.MultiSelect = true;
		this.gvDoc.OptionsView.ColumnAutoWidth = false;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gvDoc_RowCellStyle);
		this.gvDoc.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gvDoc_ShowingEditor);
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.gvDoc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanging);
		this.gridColumn1.Caption = "UomEntry";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "UomEntry";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "UoM Code";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "UomCode";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 157;
		this.gridColumn3.Caption = "UoM Name";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "UomName";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn3.Width = 148;
		this.gridColumn4.Caption = "gridColumn4";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "Locked";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsColumn.AllowEdit = false;
		this.gridColumn4.OptionsColumn.ReadOnly = true;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn5.Caption = "gridColumn5";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "DataSource";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsColumn.AllowEdit = false;
		this.gridColumn5.OptionsColumn.ReadOnly = true;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn6.Caption = "gridColumn6";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "UserSign";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsColumn.AllowEdit = false;
		this.gridColumn6.OptionsColumn.ReadOnly = true;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn7.Caption = "gridColumn7";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "LogInstanc";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.OptionsColumn.AllowEdit = false;
		this.gridColumn7.OptionsColumn.ReadOnly = true;
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn8.Caption = "gridColumn8";
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "UserSign2";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.OptionsColumn.AllowEdit = false;
		this.gridColumn8.OptionsColumn.ReadOnly = true;
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn9.Caption = "gridColumn9";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "UpdateDate";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsColumn.AllowEdit = false;
		this.gridColumn9.OptionsColumn.ReadOnly = true;
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn10.Caption = "gridColumn10";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "CreateDate";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsColumn.AllowEdit = false;
		this.gridColumn10.OptionsColumn.ReadOnly = true;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn11.Caption = "Length";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "Length1";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.OptionsColumn.AllowEdit = false;
		this.gridColumn11.OptionsColumn.ReadOnly = true;
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Width = 127;
		this.gridColumn12.Caption = "gridColumn12";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "Len1Unit";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.OptionsColumn.AllowEdit = false;
		this.gridColumn12.OptionsColumn.ReadOnly = true;
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gc_Length1Disp.Caption = "Length";
		this.gc_Length1Disp.ColumnEdit = this.repositoryItemCalcEditEx1;
		this.gc_Length1Disp.CurrencyDateField = null;
		this.gc_Length1Disp.CurrencyField = null;
		this.gc_Length1Disp.CurrencyFixed = null;
		this.gc_Length1Disp.CurrencyFixedRate = 0.0;
		this.gc_Length1Disp.CurrencyRateField = null;
		this.gc_Length1Disp.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Unit;
		this.gc_Length1Disp.FieldName = "Length1Disp";
		this.gc_Length1Disp.Name = "gc_Length1Disp";
		this.gc_Length1Disp.PrimaryKey = false;
		this.gc_Length1Disp.UnitField = null;
		this.gc_Length1Disp.Visible = true;
		this.gc_Length1Disp.VisibleIndex = 2;
		this.gridColumn13.Caption = "gridColumn13";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "length2";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.OptionsColumn.AllowEdit = false;
		this.gridColumn13.OptionsColumn.ReadOnly = true;
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn14.Caption = "gridColumn14";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "Len2Unit";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.OptionsColumn.AllowEdit = false;
		this.gridColumn14.OptionsColumn.ReadOnly = true;
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn15.Caption = "Width";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "Width1";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.OptionsColumn.AllowEdit = false;
		this.gridColumn15.OptionsColumn.ReadOnly = true;
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn15.Width = 160;
		this.gridColumn16.Caption = "gridColumn16";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "Wdth1Unit";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.OptionsColumn.AllowEdit = false;
		this.gridColumn16.OptionsColumn.ReadOnly = true;
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gc_Width1Disp.Caption = "Width";
		this.gc_Width1Disp.ColumnEdit = this.repositoryItemCalcEditEx2;
		this.gc_Width1Disp.CurrencyDateField = null;
		this.gc_Width1Disp.CurrencyField = null;
		this.gc_Width1Disp.CurrencyFixed = null;
		this.gc_Width1Disp.CurrencyFixedRate = 0.0;
		this.gc_Width1Disp.CurrencyRateField = null;
		this.gc_Width1Disp.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Unit;
		this.gc_Width1Disp.FieldName = "Width1Disp";
		this.gc_Width1Disp.Name = "gc_Width1Disp";
		this.gc_Width1Disp.PrimaryKey = false;
		this.gc_Width1Disp.UnitField = null;
		this.gc_Width1Disp.Visible = true;
		this.gc_Width1Disp.VisibleIndex = 3;
		this.gridColumn17.Caption = "gridColumn17";
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "Width2";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.OptionsColumn.AllowEdit = false;
		this.gridColumn17.OptionsColumn.ReadOnly = true;
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn18.Caption = "gridColumn18";
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "Wdth2Unit";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.OptionsColumn.AllowEdit = false;
		this.gridColumn18.OptionsColumn.ReadOnly = true;
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn19.Caption = "Height";
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.FieldName = "Height1";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.OptionsColumn.AllowEdit = false;
		this.gridColumn19.OptionsColumn.ReadOnly = true;
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		this.gridColumn19.Width = 144;
		this.gridColumn20.Caption = "gridColumn20";
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.FieldName = "Hght1Unit";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.OptionsColumn.AllowEdit = false;
		this.gridColumn20.OptionsColumn.ReadOnly = true;
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gc_Height1Disp.Caption = "Height";
		this.gc_Height1Disp.ColumnEdit = this.repositoryItemCalcEditEx3;
		this.gc_Height1Disp.CurrencyDateField = null;
		this.gc_Height1Disp.CurrencyField = null;
		this.gc_Height1Disp.CurrencyFixed = null;
		this.gc_Height1Disp.CurrencyFixedRate = 0.0;
		this.gc_Height1Disp.CurrencyRateField = null;
		this.gc_Height1Disp.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Unit;
		this.gc_Height1Disp.FieldName = "Height1Disp";
		this.gc_Height1Disp.Name = "gc_Height1Disp";
		this.gc_Height1Disp.PrimaryKey = false;
		this.gc_Height1Disp.UnitField = null;
		this.gc_Height1Disp.Visible = true;
		this.gc_Height1Disp.VisibleIndex = 4;
		this.gridColumn21.Caption = "gridColumn21";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "Height2";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.OptionsColumn.AllowEdit = false;
		this.gridColumn21.OptionsColumn.ReadOnly = true;
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn22.Caption = "gridColumn22";
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.FieldName = "Hght2Unit";
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.OptionsColumn.AllowEdit = false;
		this.gridColumn22.OptionsColumn.ReadOnly = true;
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		this.gc_vol.Caption = "Volume";
		this.gc_vol.ColumnEdit = this.repositoryItemCalcEdit1;
		this.gc_vol.CurrencyDateField = null;
		this.gc_vol.CurrencyField = null;
		this.gc_vol.CurrencyFixed = null;
		this.gc_vol.CurrencyFixedRate = 0.0;
		this.gc_vol.CurrencyRateField = null;
		this.gc_vol.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Unit;
		this.gc_vol.FieldName = "Volume";
		this.gc_vol.Name = "gc_vol";
		this.gc_vol.PrimaryKey = false;
		this.gc_vol.UnitField = null;
		this.gc_vol.Visible = true;
		this.gc_vol.VisibleIndex = 5;
		this.gc_vol.Width = 151;
		this.repositoryItemCalcEdit1.AutoHeight = false;
		this.repositoryItemCalcEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEdit1.Name = "repositoryItemCalcEdit1";
		this.gc_volUom.Caption = "Volume UoM";
		this.gc_volUom.ColumnEdit = this.rep_VolUoM;
		this.gc_volUom.CurrencyDateField = null;
		this.gc_volUom.CurrencyField = null;
		this.gc_volUom.CurrencyFixed = null;
		this.gc_volUom.CurrencyFixedRate = 0.0;
		this.gc_volUom.CurrencyRateField = null;
		this.gc_volUom.FieldName = "VolUnit";
		this.gc_volUom.Name = "gc_volUom";
		this.gc_volUom.PrimaryKey = false;
		this.gc_volUom.UnitField = null;
		this.gc_volUom.Visible = true;
		this.gc_volUom.VisibleIndex = 6;
		this.gc_volUom.Width = 135;
		this.rep_VolUoM.AutoHeight = false;
		this.rep_VolUoM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_VolUoM.DisplayMember = "Name";
		this.rep_VolUoM.Name = "rep_VolUoM";
		this.rep_VolUoM.NullText = "";
		this.rep_VolUoM.ValueMember = "Code";
		this.rep_VolUoM.EditValueChanged += new System.EventHandler(this.rep_VolUoM_EditValueChanged);
		this.rep_VolUoM.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.rep_VolUoM_EditValueChanging);
		this.gridColumn25.Caption = "Weight";
		this.gridColumn25.CurrencyDateField = null;
		this.gridColumn25.CurrencyField = null;
		this.gridColumn25.CurrencyFixed = null;
		this.gridColumn25.CurrencyFixedRate = 0.0;
		this.gridColumn25.CurrencyRateField = null;
		this.gridColumn25.FieldName = "Weight1";
		this.gridColumn25.Name = "gridColumn25";
		this.gridColumn25.OptionsColumn.AllowEdit = false;
		this.gridColumn25.OptionsColumn.ReadOnly = true;
		this.gridColumn25.PrimaryKey = false;
		this.gridColumn25.UnitField = null;
		this.gridColumn25.Width = 184;
		this.gridColumn26.Caption = "gridColumn26";
		this.gridColumn26.CurrencyDateField = null;
		this.gridColumn26.CurrencyField = null;
		this.gridColumn26.CurrencyFixed = null;
		this.gridColumn26.CurrencyFixedRate = 0.0;
		this.gridColumn26.CurrencyRateField = null;
		this.gridColumn26.FieldName = "WghtUnit";
		this.gridColumn26.Name = "gridColumn26";
		this.gridColumn26.OptionsColumn.AllowEdit = false;
		this.gridColumn26.OptionsColumn.ReadOnly = true;
		this.gridColumn26.PrimaryKey = false;
		this.gridColumn26.UnitField = null;
		this.gc_Weight1Disp.Caption = "Weight";
		this.gc_Weight1Disp.ColumnEdit = this.repositoryItemCalcEditEx4;
		this.gc_Weight1Disp.CurrencyDateField = null;
		this.gc_Weight1Disp.CurrencyField = null;
		this.gc_Weight1Disp.CurrencyFixed = null;
		this.gc_Weight1Disp.CurrencyFixedRate = 0.0;
		this.gc_Weight1Disp.CurrencyRateField = null;
		this.gc_Weight1Disp.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Unit;
		this.gc_Weight1Disp.FieldName = "Weight1Disp";
		this.gc_Weight1Disp.Name = "gc_Weight1Disp";
		this.gc_Weight1Disp.PrimaryKey = false;
		this.gc_Weight1Disp.UnitField = null;
		this.gc_Weight1Disp.Visible = true;
		this.gc_Weight1Disp.VisibleIndex = 7;
		this.gridColumn27.Caption = "gridColumn27";
		this.gridColumn27.CurrencyDateField = null;
		this.gridColumn27.CurrencyField = null;
		this.gridColumn27.CurrencyFixed = null;
		this.gridColumn27.CurrencyFixedRate = 0.0;
		this.gridColumn27.CurrencyRateField = null;
		this.gridColumn27.FieldName = "Weight2";
		this.gridColumn27.Name = "gridColumn27";
		this.gridColumn27.OptionsColumn.AllowEdit = false;
		this.gridColumn27.OptionsColumn.ReadOnly = true;
		this.gridColumn27.PrimaryKey = false;
		this.gridColumn27.UnitField = null;
		this.gridColumn28.Caption = "gridColumn28";
		this.gridColumn28.CurrencyDateField = null;
		this.gridColumn28.CurrencyField = null;
		this.gridColumn28.CurrencyFixed = null;
		this.gridColumn28.CurrencyFixedRate = 0.0;
		this.gridColumn28.CurrencyRateField = null;
		this.gridColumn28.FieldName = "Wght2Unit";
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.OptionsColumn.AllowEdit = false;
		this.gridColumn28.OptionsColumn.ReadOnly = true;
		this.gridColumn28.PrimaryKey = false;
		this.gridColumn28.UnitField = null;
		this.gridColumn29.Caption = "gridColumn29";
		this.gridColumn29.CurrencyDateField = null;
		this.gridColumn29.CurrencyField = null;
		this.gridColumn29.CurrencyFixed = null;
		this.gridColumn29.CurrencyFixedRate = 0.0;
		this.gridColumn29.CurrencyRateField = null;
		this.gridColumn29.FieldName = "IntSymbol";
		this.gridColumn29.Name = "gridColumn29";
		this.gridColumn29.OptionsColumn.AllowEdit = false;
		this.gridColumn29.OptionsColumn.ReadOnly = true;
		this.gridColumn29.PrimaryKey = false;
		this.gridColumn29.UnitField = null;
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.ContextMenuStrip = this.contextMenuStrip1;
		this.grdDoc.Location = new System.Drawing.Point(1, -2);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[4] { this.rep_GLAcct, this.rep_UOM, this.rep_VolUoM, this.repositoryItemCalcEdit1 });
		this.grdDoc.Size = new System.Drawing.Size(747, 311);
		this.grdDoc.TabIndex = 77;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 48);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click_1);
		this.rep_GLAcct.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_GLAcct.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_GLAcct.Name = "rep_GLAcct";
		this.rep_UOM.AutoHeight = false;
		this.rep_UOM.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_UOM.Name = "rep_UOM";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(102, 317);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(99, 20);
		this.btnCancel.TabIndex = 78;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(1, 317);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(99, 20);
		this.btn_Save.TabIndex = 79;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(750, 347);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grdDoc);
		base.FormId = "oUnitOfMeasure";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_UnitOfMeasure.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_UnitOfMeasure";
		base.ShowInTaskbar = false;
		this.Text = "Unit of Measure";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_UnitOfMeasure_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_VolUoM).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.rep_GLAcct).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UOM).EndInit();
		base.ResumeLayout(false);
	}
}
