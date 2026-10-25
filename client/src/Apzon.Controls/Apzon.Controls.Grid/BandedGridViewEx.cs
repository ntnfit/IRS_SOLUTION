using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls.Commons;
using Apzon.Controls.Compiler;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Form.ExchangeRateAndIndex;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Export;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.BandedGrid;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;

namespace Apzon.Controls.Grid;

public class BandedGridViewEx : BandedGridView
{
	private List<Token> SyntaxOnCellValueChanged;

	private List<Token> SyntaxOnFocusedRowObjectChanged;

	private List<Token> SyntaxSelectionChanged;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly List<string> _listMappingField = new List<string>();

	public string FormId { get; set; }

	public string ItemId { get; set; }

	public string[] LstColumNotSetting { get; set; }

	public string[] LstColumnNotEditable { get; set; }

	public BandedGridColumnEx EditColumn { get; set; }

	public int IndexEditRow { get; set; }

	public bool IsLoadingData { get; set; }

	public bool CancelNextCellValueChangedScript { get; set; }

	public bool CancelNextSelectedChangedScript { get; set; }

	public bool CancelNextFocusObjectChangedScript { get; set; }

	public Enums.ChangeLog ChangeLogType { get; set; }

	public bool IsApzTable { get; set; }

	public BandedGridColumnCollectionEx ColumnsEx => Columns as BandedGridColumnCollectionEx;

	public void AddMappingField(string field)
	{
		if (!_listMappingField.Contains(field))
		{
			_listMappingField.Add(field);
		}
	}

	public void RemoveMappingField(string field)
	{
		_listMappingField.RemoveAll((string item) => item == field);
	}

	public void ClearListMappingField()
	{
		_listMappingField.Clear();
	}

	public BandedGridViewEx()
	{
		InitComponents();
		IndexEditRow = -1;
		EditColumn = null;
	}

	public BandedGridViewEx(GridControlEx grid)
		: base(grid)
	{
		InitComponents();
		IndexEditRow = -1;
		EditColumn = null;
	}

	protected override GridColumnCollection CreateColumnCollection()
	{
		return new BandedGridColumnCollectionEx(this);
	}

	public void InitComponents()
	{
		InvalidValueException += (object sender, InvalidValueExceptionEventArgs e) =>
		{
			e.ExceptionMode = ExceptionMode.Ignore;
		};
		CustomColumnDisplayText += BandedGridViewEx_CustomColumnDisplayText;
		ChangeLogType = Enums.ChangeLog.clNone;
	}

	private void BandedGridViewEx_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs events)
	{
		if (DesignMode || LicenseManager.UsageMode == LicenseUsageMode.Designtime)
		{
			return;
		}
		try
		{
			if (!(events.Column is BandedGridColumnEx bandedGridColumnEx))
			{
				return;
			}
			Enums.DisplayFormatType displayFormatType = bandedGridColumnEx.DisplayFormatType;
			if ((uint)(displayFormatType - 2) > 1u)
			{
				return;
			}
			string currency = string.Empty;
			switch (bandedGridColumnEx.Currency)
			{
			case Enums.CurrencyColumn.MainCurrency:
				currency = GlobalData.MainCurrency;
				break;
			case Enums.CurrencyColumn.SystemCurrency:
				currency = GlobalData.SystemCurrency;
				break;
			case Enums.CurrencyColumn.Custom:
				currency = Function.ToString(GetListSourceRowCellValue(events.ListSourceRowIndex, bandedGridColumnEx.CurrencyField));
				break;
			}
			int num = -1;
			if (string.IsNullOrEmpty(currency) || currency == "")
			{
				currency = GlobalData.MainCurrency;
			}
			if (bandedGridColumnEx.Currency == Enums.CurrencyColumn.None)
			{
				currency = "";
			}
			if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
			{
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
				if (currencyCodesImpl != null)
				{
					num = currencyCodesImpl.Decimals;
				}
			}
			if (bandedGridColumnEx.DisplayFormatType == Enums.DisplayFormatType.Price)
			{
				string text = new string('#', (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
				events.DisplayText = string.Format("{0:#,0." + text + "} {1}", events.Value, currency);
			}
			else if (bandedGridColumnEx.DisplayFormatType == Enums.DisplayFormatType.Amount)
			{
				string text2 = new string('#', (num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num);
				events.DisplayText = string.Format("{0:#,0." + text2 + "} {1}", events.Value, currency);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void SetRowCellValueCore(int rowHandle, GridColumn column, object _value, bool fromEditor)
	{
		if (!(column is BandedGridColumnEx bandedGridColumnEx) || !_listMappingField.Contains(bandedGridColumnEx.FieldName))
		{
			base.SetRowCellValueCore(rowHandle, column, _value, fromEditor);
		}
	}

	protected override void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs e)
	{
		base.RaiseValidatingEditor(e);
		try
		{
			BandedGridColumnEx col = FocusedColumn as BandedGridColumnEx;
			if (col != null && !string.IsNullOrEmpty(col.CurrencyField))
			{
				string[] dataCurrency = Function.ToString(e.Value).GetDataCurrency();
				if (dataCurrency != null && dataCurrency.Length != 0)
				{
					decimal d = Function.ParseDecimal(dataCurrency[0]);
					string alpha = Function.ToString(dataCurrency[1]);
					SetRowCellValue(FocusedRowHandle, col.CurrencyField, (alpha != "") ? alpha.ToUpper() : GlobalData.MainCurrency);
					if (alpha.ToUpper() == GlobalData.MainCurrency.ToUpper() || string.IsNullOrEmpty(alpha))
					{
						SetRowCellValue(FocusedRowHandle, col.CurrencyRateField, 1);
					}
					else
					{
						DateTime date = DateTime.Today;
						string currencyField = col.CurrencyField;
						if (!string.IsNullOrEmpty(currencyField))
						{
							date = Function.ParseDateTime(GetFocusedRowCellValue(currencyField)) ?? DateTime.Today;
						}
						double exchangeRate = HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(alpha, date);
						if (exchangeRate > 0.0)
						{
							SetRowCellValue(FocusedRowHandle, col.CurrencyRateField, exchangeRate);
						}
						else
						{
							frm_ExchangeRatesAndIndexs frm = new frm_ExchangeRatesAndIndexs();
							frm.Curr = alpha;
							frm._date = date;
							frm.FormClosing += (object s, FormClosingEventArgs e2) =>
							{
								SetRowCellValue(FocusedRowHandle, col.CurrencyRateField, Function.ParseDouble(frm.rate));
							};
							frm.ShowDialog();
						}
					}
					if (col.FieldName != col.CurrencyField)
					{
						int num = -1;
						if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
						{
							CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(alpha.ToUpper()));
							if (currencyCodesImpl != null)
							{
								num = currencyCodesImpl.Decimals;
							}
						}
						d = Math.Round(d, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
						e.Value = d;
						SetRowCellValue(FocusedRowHandle, col, d);
					}
				}
			}
			if (col != null && col.DisplayFormatType == Enums.DisplayFormatType.Time && col.ColumnType == typeof(int))
			{
				e.Value = string.Join("", from c in e.Value.ToString().Where(char.IsDigit)
					select (c));
				SetRowCellValue(FocusedRowHandle, col, e.Value);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void RaiseCellValueChanged(CellValueChangedEventArgs e)
	{
		if (e != null && e.Column != null)
		{
			try
			{
				BandedGridColumnEx bandedGridColumnEx = e.Column as BandedGridColumnEx;
				if (bandedGridColumnEx.DisplayFormatType == Enums.DisplayFormatType.Price)
				{
					string currency = string.Empty;
					switch (bandedGridColumnEx.Currency)
					{
					case Enums.CurrencyColumn.MainCurrency:
						currency = GlobalData.MainCurrency;
						break;
					case Enums.CurrencyColumn.SystemCurrency:
						currency = GlobalData.SystemCurrency;
						break;
					case Enums.CurrencyColumn.Custom:
						if (string.IsNullOrEmpty(Function.ToString(GetListSourceRowCellValue(e.RowHandle, bandedGridColumnEx.CurrencyField))))
						{
							SetRowCellValue(e.RowHandle, bandedGridColumnEx.CurrencyField, GlobalData.MainCurrency);
						}
						currency = Function.ToString(GetListSourceRowCellValue(e.RowHandle, bandedGridColumnEx.CurrencyField));
						break;
					}
					int num = -1;
					if (string.IsNullOrEmpty(currency) || currency == "")
					{
						currency = GlobalData.MainCurrency;
					}
					if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
					{
						CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
						if (currencyCodesImpl != null)
						{
							num = ((currencyCodesImpl.Decimals == -1) ? num : currencyCodesImpl.Decimals);
						}
					}
					decimal num2 = Function.ParseDecimal(GetRowCellValue(e.RowHandle, bandedGridColumnEx));
					decimal num3 = Math.Round(num2, (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
					if (num3 != num2)
					{
						SetRowCellValue(e.RowHandle, bandedGridColumnEx, num3);
						return;
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			}
		}
		base.RaiseCellValueChanged(e);
		GridControl gridControl = GridControl;
		if (gridControl == null || string.IsNullOrEmpty(FormId) || string.IsNullOrEmpty(GlobalData.SessionId))
		{
			return;
		}
		if (gridControl.FindForm() is XtraFormEx xtraFormEx)
		{
			if (SyntaxOnCellValueChanged == null)
			{
				string formulaText = HttpClientHelperFactory.GetHttpClient(xtraFormEx.FormObjectType).Formula.GetFormulaText(FormId, Enums.EventId.OnCellValueChanged, ItemId);
				SyntaxOnCellValueChanged = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
			}
			if (SyntaxOnCellValueChanged.Count != 0 && !CancelNextCellValueChangedScript)
			{
				BandedGridColumnEx editColumn = EditColumn;
				_ = IndexEditRow;
				EditColumn = e.Column as BandedGridColumnEx;
				CtdMessage ctdMessage = new Apzon.Controls.Compiler.Compiler(Configuration.ApiServer, GlobalData.SessionId, e.RowHandle, e.Column.FieldName).UserdefinedFomular(xtraFormEx, SyntaxOnCellValueChanged, out var _);
				EditColumn = editColumn;
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
				}
			}
		}
		else
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Parent Form is null");
		}
	}

	protected override void RaiseFocusedRowObjectChanged(FocusedRowObjectChangedEventArgs e)
	{
		if (IsLoadingData)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "GridViewEx_FocusedRowObjectChanged Return cause loading data");
			return;
		}
		IsLoadingData = true;
		bool cancelEvent = false;
		GridControl gridControl = GridControl;
		XtraFormEx xtraFormEx = null;
		if (gridControl != null && !string.IsNullOrEmpty(FormId) && !string.IsNullOrEmpty(GlobalData.SessionId))
		{
			if (gridControl.FindForm() is XtraFormEx xtraFormEx2)
			{
				if (SyntaxOnFocusedRowObjectChanged == null)
				{
					string formulaText = HttpClientHelperFactory.GetHttpClient(xtraFormEx2.FormObjectType).Formula.GetFormulaText(FormId, Enums.EventId.OnFocusedRowObjectChanged, ItemId);
					SyntaxOnFocusedRowObjectChanged = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
				}
				if (SyntaxOnFocusedRowObjectChanged.Count != 0 && !CancelNextFocusObjectChangedScript)
				{
					CtdMessage ctdMessage = new Apzon.Controls.Compiler.Compiler(Configuration.ApiServer, GlobalData.SessionId).UserdefinedFomular(xtraFormEx2, SyntaxOnFocusedRowObjectChanged, out cancelEvent);
					if (!string.IsNullOrEmpty(ctdMessage.Message))
					{
						WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
					}
				}
			}
			else
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Parent Form is null");
			}
		}
		if (!cancelEvent)
		{
			base.RaiseFocusedRowObjectChanged(e);
		}
		IsLoadingData = false;
	}

	protected override void OnSelectionChangedCore(SelectionChangedEventArgs e)
	{
		try
		{
			bool cancelEvent = false;
			GridControl gridControl = GridControl;
			if (gridControl != null && !string.IsNullOrEmpty(FormId) && !string.IsNullOrEmpty(GlobalData.SessionId))
			{
				if (gridControl.FindForm() is XtraFormEx xtraFormEx)
				{
					if (SyntaxSelectionChanged == null)
					{
						string formulaText = HttpClientHelperFactory.GetHttpClient(xtraFormEx.FormObjectType).Formula.GetFormulaText(FormId, Enums.EventId.OnSelectionChanged, ItemId);
						SyntaxSelectionChanged = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
					}
					if (SyntaxSelectionChanged.Count != 0 && !CancelNextSelectedChangedScript)
					{
						CtdMessage ctdMessage = new Apzon.Controls.Compiler.Compiler(Configuration.ApiServer, GlobalData.SessionId).UserdefinedFomular(xtraFormEx, SyntaxSelectionChanged, out cancelEvent);
						if (!string.IsNullOrEmpty(ctdMessage.Message))
						{
							WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
						}
					}
				}
				else
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Parent Form is null");
				}
			}
			if (cancelEvent)
			{
				return;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		base.OnSelectionChangedCore(e);
	}

	public async Task ReloadFormSetting()
	{
		try
		{
			if (string.IsNullOrEmpty(FormId) || string.IsNullOrEmpty(ItemId))
			{
				return;
			}
			DataTable dataTable = await HttpClientHelperFactory.GetHttpClient().FormSettingClient.GetFormSetting(FormId, ItemId, GlobalData.UserName, GlobalData.CURRENTLANGUAGE);
			if (dataTable == null || dataTable.Rows.Count <= 0)
			{
				return;
			}
			GridColumn[] array = Columns.Where((GridColumn t) => t.Fixed != FixedStyle.None).ToArray();
			for (int num = 0; num < array.Length; num++)
			{
				array[num].Fixed = FixedStyle.None;
			}
			List<DataRow> list = (from t in dataTable.AsEnumerable()
				orderby Function.ParseInt(t["GroupIndex"]), Function.ParseInt(t["VisIndex"])
				select t).ToList();
			if (list.Any((DataRow t) => Function.ParseInt(t["GroupIndex"]) > -1))
			{
				ClearGrouping();
				OptionsView.ShowGroupPanel = true;
			}
			for (int num2 = 0; num2 < list.Count; num2++)
			{
				DataRow dataRow = list[num2];
				if (!Function.ParseBool(dataRow["VisSetting"]))
				{
					continue;
				}
				string text = Function.ToString(dataRow["ColumnId"]);
				FormSetting setting = new FormSetting
				{
					FormId = Function.ToString(dataRow["FormId"]),
					ItemId = Function.ToString(dataRow["ItemId"]),
					ColumnId = text,
					ColumnName = Function.ToString(dataRow["ColumnName"]),
					UserSign = Function.ToString(dataRow["UserSign"]),
					Width = Function.ParseInt(dataRow["Width"]),
					Visible = Function.ParseBool(dataRow["Visible"]),
					VisIndex = Function.ParseInt(dataRow["VisIndex"]),
					IsReadOnly = Function.ParseBool(dataRow["IsReadOnly"]),
					Fixed = Function.ToString(dataRow["Fixed"]),
					AllowChSet = Function.ParseBool(dataRow["AllowChSet"]),
					VisSetting = Function.ParseBool(dataRow["VisSetting"]),
					NeverEditable = (LstColumnNotEditable != null && LstColumnNotEditable.Contains(text)),
					GroupIndex = Function.ParseInt(dataRow["GroupIndex"]),
					CurrField = Function.ToString(dataRow["CurrField"])
				};
				setting.Editable = !setting.NeverEditable && Function.ParseBool(dataRow["Editable"]);
				if (Columns.Where((GridColumn t) => t.FieldName.Equals(setting.ColumnId)).ToList().Count <= 0)
				{
					BandedGridColumnEx column = new BandedGridColumnEx
					{
						FieldName = setting.ColumnId
					};
					if (Columns.Count <= setting.VisIndex || setting.VisIndex < 0)
					{
						Columns.Add(column);
					}
					else
					{
						Columns.Insert(setting.VisIndex, column);
					}
				}
				if (Columns.FirstOrDefault((GridColumn t) => t.FieldName.Equals(setting.ColumnId)) is BandedGridColumnEx bandedGridColumnEx)
				{
					if (!string.IsNullOrEmpty(setting.ColumnName))
					{
						bandedGridColumnEx.Caption = setting.ColumnName;
					}
					else
					{
						bandedGridColumnEx.Caption = setting.ColumnId;
					}
					bandedGridColumnEx.Width = setting.Width;
					bandedGridColumnEx.Visible = setting.Visible;
					if (setting.Visible)
					{
						bandedGridColumnEx.VisibleIndex = setting.VisIndex;
						bandedGridColumnEx.AbsoluteIndex = setting.VisIndex;
					}
					bandedGridColumnEx.OptionsColumn.AllowEdit = setting.Editable;
					bandedGridColumnEx.OptionsColumn.ReadOnly = setting.IsReadOnly;
					bandedGridColumnEx.Fixed = ((setting.Fixed == "L") ? FixedStyle.Left : ((setting.Fixed == "R") ? FixedStyle.Right : FixedStyle.None));
					bandedGridColumnEx.AllowChSet = setting.AllowChSet;
					bandedGridColumnEx.GroupIndex = setting.GroupIndex;
					if (!string.IsNullOrWhiteSpace(Function.ToString(setting.CurrField)))
					{
						bandedGridColumnEx.Currency = Enums.CurrencyColumn.Custom;
						bandedGridColumnEx.CurrencyField = setting.CurrField;
						bandedGridColumnEx.CurrencyFixedRate = 0.0;
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void Export(Enums.ExportExt extention)
	{
		string text = "";
		Dictionary<string, Enums.DisplayFormatType> dictionary = new Dictionary<string, Enums.DisplayFormatType>();
		Dictionary<string, RepositoryItem> dictionary2 = new Dictionary<string, RepositoryItem>();
		foreach (BandedGridColumnEx item in Columns.Cast<BandedGridColumnEx>())
		{
			switch (item.DisplayFormatType)
			{
			case Enums.DisplayFormatType.Qty:
			case Enums.DisplayFormatType.Rate:
			case Enums.DisplayFormatType.Unit:
				dictionary.Add(item.FieldName, item.DisplayFormatType);
				item.DisplayFormat.FormatType = FormatType.None;
				if (item.ColumnEdit != null && item.ColumnEdit.DisplayFormat.FormatType != FormatType.None)
				{
					dictionary2.Add(item.FieldName, item.ColumnEdit);
					item.ColumnEdit = null;
				}
				break;
			case Enums.DisplayFormatType.Percent:
				dictionary.Add(item.FieldName, item.DisplayFormatType);
				item.DisplayFormat.FormatString = "p";
				break;
			}
		}
		switch (extention)
		{
		case Enums.ExportExt.Xls:
			text = ExtensionMethods.PathSaveFile("Excel (2003)", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToXls(text);
			}
			break;
		case Enums.ExportExt.Xlsx:
			text = ExtensionMethods.PathSaveFile("Excel (2007)", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToXlsx(text);
			}
			break;
		case Enums.ExportExt.Xlsx2:
			text = ExtensionMethods.PathSaveFile("Excel (2007)", Enums.ExportExt.Xlsx);
			if (!string.IsNullOrEmpty(text))
			{
				using MemoryStream memoryStream3 = new MemoryStream();
				ExportToXlsx(memoryStream3);
				FileStream fileStream3 = new FileStream(text, FileMode.Create, FileAccess.Write);
				memoryStream3.WriteTo(fileStream3);
				fileStream3.Close();
			}
			break;
		case Enums.ExportExt.Xlsx3:
			text = ExtensionMethods.PathSaveFile("Excel (2007)", Enums.ExportExt.Xlsx);
			if (!string.IsNullOrWhiteSpace(text))
			{
				using MemoryStream memoryStream2 = new MemoryStream();
				ExportToXlsx(memoryStream2, new XlsxExportOptionsEx
				{
					ExportType = ExportType.DataAware,
					TextExportMode = TextExportMode.Text
				});
				FileStream fileStream2 = new FileStream(text, FileMode.Create, FileAccess.Write);
				memoryStream2.WriteTo(fileStream2);
				fileStream2.Close();
			}
			break;
		case Enums.ExportExt.Xlsx4:
			text = ExtensionMethods.PathSaveFile("Excel (2007)", Enums.ExportExt.Xlsx);
			if (!string.IsNullOrWhiteSpace(text))
			{
				using MemoryStream memoryStream = new MemoryStream();
				ExportToXlsx(memoryStream, new XlsxExportOptionsEx
				{
					ExportType = ExportType.WYSIWYG,
					TextExportMode = TextExportMode.Text
				});
				FileStream fileStream = new FileStream(text, FileMode.Create, FileAccess.Write);
				memoryStream.WriteTo(fileStream);
				fileStream.Close();
			}
			break;
		case Enums.ExportExt.Txt:
			text = ExtensionMethods.PathSaveFile("Text", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToText(text);
			}
			break;
		case Enums.ExportExt.Rtf:
			text = ExtensionMethods.PathSaveFile("RTF", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToRtf(text);
			}
			break;
		case Enums.ExportExt.Pdf:
			text = ExtensionMethods.PathSaveFile("PDF", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToPdf(text);
			}
			break;
		case Enums.ExportExt.Html:
			text = ExtensionMethods.PathSaveFile("HTML", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToHtml(text);
			}
			break;
		case Enums.ExportExt.Csv:
			text = ExtensionMethods.PathSaveFile("CSV", extention);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToCsv(text);
			}
			break;
		case Enums.ExportExt.Clb:
			CopyToClipboard();
			break;
		}
		if (dictionary.Count > 0)
		{
			foreach (KeyValuePair<string, Enums.DisplayFormatType> item2 in dictionary)
			{
				if (Columns[item2.Key] is BandedGridColumnEx bandedGridColumnEx)
				{
					bandedGridColumnEx.DisplayFormatType = item2.Value;
					if (dictionary2.ContainsKey(item2.Key))
					{
						Columns[item2.Key].ColumnEdit = dictionary2[item2.Key];
					}
				}
			}
		}
		if (!string.IsNullOrEmpty(text) && FileIO.FileExist(text))
		{
			try
			{
				Process.Start(text);
			}
			catch
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.CanNotOpenFile, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand, text);
			}
		}
	}

	public void Export()
	{
		using SaveFileDialog saveFileDialog = new SaveFileDialog();
		saveFileDialog.Filter = "Excel (2003)(.xls)|*.xls|Excel (2010) (.xlsx)|*.xlsx |RichText File (.rtf)|*.rtf |Pdf File (.pdf)|*.pdf |Html File (.html)|*.html |Text File (.txt)|*.txt |Csv File (.csv)|*.csv";
		if (saveFileDialog.ShowDialog() == DialogResult.Cancel)
		{
			return;
		}
		Dictionary<string, Enums.DisplayFormatType> dictionary = new Dictionary<string, Enums.DisplayFormatType>();
		Dictionary<string, RepositoryItem> dictionary2 = new Dictionary<string, RepositoryItem>();
		foreach (BandedGridColumnEx item in Columns.Cast<BandedGridColumnEx>())
		{
			switch (item.DisplayFormatType)
			{
			case Enums.DisplayFormatType.Qty:
			case Enums.DisplayFormatType.Rate:
			case Enums.DisplayFormatType.Unit:
				dictionary.Add(item.FieldName, item.DisplayFormatType);
				item.DisplayFormat.FormatType = FormatType.None;
				if (item.ColumnEdit != null && item.ColumnEdit.DisplayFormat.FormatType != FormatType.None)
				{
					dictionary2.Add(item.FieldName, item.ColumnEdit);
					item.ColumnEdit = null;
				}
				break;
			case Enums.DisplayFormatType.Percent:
				dictionary.Add(item.FieldName, item.DisplayFormatType);
				item.DisplayFormat.FormatString = "p";
				break;
			}
		}
		string fileName = saveFileDialog.FileName;
		switch (new FileInfo(fileName).Extension)
		{
		case ".xls":
			ExportToXls(fileName);
			break;
		case ".xlsx":
			ExportToXlsx(fileName);
			break;
		case ".rtf":
			ExportToRtf(fileName);
			break;
		case ".pdf":
			ExportToPdf(fileName);
			break;
		case ".txt":
			ExportToText(fileName);
			break;
		case ".html":
			ExportToHtml(fileName);
			break;
		case ".mht":
			ExportToMht(fileName);
			break;
		case ".csv":
			ExportToCsv(fileName);
			break;
		default:
			ExportToXls(fileName);
			break;
		}
		if (dictionary.Count > 0)
		{
			foreach (KeyValuePair<string, Enums.DisplayFormatType> item2 in dictionary)
			{
				if (Columns[item2.Key] is BandedGridColumnEx bandedGridColumnEx)
				{
					bandedGridColumnEx.DisplayFormatType = item2.Value;
					if (dictionary2.ContainsKey(item2.Key))
					{
						Columns[item2.Key].ColumnEdit = dictionary2[item2.Key];
					}
				}
			}
		}
		if (FileIO.FileExist(fileName))
		{
			try
			{
				Process.Start(fileName);
				return;
			}
			catch
			{
				XtraMessageBoxEx.Show(Enums.MessageCode.CanNotOpenFile, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand, fileName);
				return;
			}
		}
		XtraMessageBoxEx.Show(Enums.MessageCode.CanNotSaveFile, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand, fileName);
	}

	public void ShowFormSetting(XtraFormEx form, bool showdialog = false)
	{
		try
		{
			string[] source = LstColumNotSetting ?? new string[0];
			string text = Function.ToString(ItemId);
			DataTable ret = new DataTable();
			ret.Columns.Add("FormId", typeof(string));
			ret.Columns.Add("ItemId", typeof(string));
			ret.Columns.Add("ColumnId", typeof(string));
			ret.Columns.Add("ColumnName", typeof(string));
			ret.Columns.Add("UserSign", typeof(string));
			ret.Columns.Add("Width", typeof(int));
			ret.Columns.Add("Visible", typeof(bool));
			ret.Columns.Add("VisIndex", typeof(int));
			ret.Columns.Add("Editable", typeof(bool));
			ret.Columns.Add("IsReadOnly", typeof(bool));
			ret.Columns.Add("DataType", typeof(string));
			ret.Columns.Add("Fixed", typeof(string));
			ret.Columns.Add("AllowChSet", typeof(bool));
			ret.Columns.Add("VisSetting", typeof(bool));
			ret.Columns.Add("IsSystem", typeof(string));
			ret.Columns.Add("LngCode", typeof(string));
			ret.Columns.Add("NeverEditable", typeof(bool));
			ret.Columns.Add("GroupIndex", typeof(int));
			ret.Columns.Add("CurrField", typeof(string));
			foreach (BandedGridColumnEx col in (from t in Columns
				orderby t.Visible descending, t.VisibleIndex
				select t).Cast<BandedGridColumnEx>())
			{
				if (!source.Contains(col.FieldName) && ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Equals(col.FieldName)) == null)
				{
					DataRow dataRow = ret.NewRow();
					dataRow["FormId"] = form.FormId;
					dataRow["ItemId"] = text;
					dataRow["ColumnId"] = col.FieldName;
					dataRow["ColumnName"] = col.Caption;
					dataRow["UserSign"] = GlobalData.UserName;
					dataRow["Width"] = col.Width;
					dataRow["Visible"] = col.Visible;
					dataRow["VisIndex"] = col.AbsoluteIndex;
					if (LstColumnNotEditable != null && LstColumnNotEditable.Contains(col.FieldName))
					{
						dataRow["Editable"] = false;
						dataRow["NeverEditable"] = true;
					}
					else
					{
						dataRow["Editable"] = col.Visible && !col.OptionsColumn.ReadOnly;
						dataRow["NeverEditable"] = false;
					}
					dataRow["IsReadOnly"] = col.OptionsColumn.ReadOnly;
					dataRow["DataType"] = col.ColumnType.ToString();
					DataRow dataRow2 = dataRow;
					string value;
					if (col.Fixed == FixedStyle.Left)
					{
						value = "L";
					}
					else
					{
						value = ((col.Fixed == FixedStyle.Right) ? "R" : "N");
					}
					dataRow2["Fixed"] = value;
					dataRow["AllowChSet"] = col.AllowChSet;
					dataRow["VisSetting"] = true;
					dataRow["IsSystem"] = "Y";
					dataRow["LngCode"] = "-";
					dataRow["GroupIndex"] = col.GroupIndex;
					dataRow["CurrField"] = ((col.Currency == Enums.CurrencyColumn.Custom) ? col.CurrencyField : string.Empty);
					ret.Rows.Add(dataRow);
				}
			}
			GridControl gridControl = GridControl;
			if (gridControl.DataSource != null)
			{
				DataTable listKeyAlias = _httpClient.Udf.GetListKeyAlias(text);
				if ((text != "@OPR62" && gridControl.DataSource.GetType() == typeof(DataTable)) || gridControl.DataSource.GetType().BaseType == typeof(DataTable))
				{
					DataColumn[] array = (from DataColumn t in gridControl.DataSourceTable().Columns
						where ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Equals(t.ColumnName)) == null
						select t).ToArray();
					foreach (DataColumn column in array)
					{
						if (source.Contains(column.ColumnName) || ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Equals(column.ColumnName)) != null)
						{
							continue;
						}
						DataRow dataRow3 = ret.NewRow();
						dataRow3["FormId"] = form.FormId;
						dataRow3["ItemId"] = text;
						dataRow3["ColumnId"] = column.ColumnName;
						if (column.ColumnName.StartsWith("U_"))
						{
							DataRow dataRow4 = listKeyAlias.AsEnumerable().FirstOrDefault((DataRow r) => "U_" + Function.ToString(r["AliasID"]) == column.ColumnName);
							if (dataRow4 != null)
							{
								dataRow3["ColumnName"] = dataRow4["Descr"];
							}
							else
							{
								dataRow3["ColumnName"] = column.ColumnName;
							}
						}
						else
						{
							dataRow3["ColumnName"] = column.ColumnName;
						}
						dataRow3["UserSign"] = GlobalData.UserName;
						dataRow3["Width"] = 100;
						dataRow3["Visible"] = false;
						dataRow3["VisIndex"] = ret.Rows.Count;
						dataRow3["Editable"] = false;
						dataRow3["IsReadOnly"] = true;
						dataRow3["DataType"] = column.DataType.Name;
						dataRow3["Fixed"] = "N";
						dataRow3["AllowChSet"] = true;
						dataRow3["VisSetting"] = true;
						dataRow3["IsSystem"] = "N";
						dataRow3["LngCode"] = "-";
						dataRow3["NeverEditable"] = LstColumnNotEditable != null && LstColumnNotEditable.Contains(column.ColumnName);
						dataRow3["GroupIndex"] = -1;
						dataRow3["CurrField"] = string.Empty;
						ret.Rows.Add(dataRow3);
					}
				}
				else if (text != "@OPR62" && gridControl.DataSource.GetType().GetGenericArguments().Length != 0)
				{
					Type type = gridControl.DataSource.GetType().GetGenericArguments()[0];
					if (type != null)
					{
						PropertyInfo[] properties = type.GetProperties();
						if (properties.Length != 0)
						{
							PropertyInfo[] array2 = properties;
							foreach (PropertyInfo propertyInfo in array2)
							{
								if (ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Equals(propertyInfo.Name)) != null || source.Contains(propertyInfo.Name))
								{
									continue;
								}
								DataRow dataRow5 = ret.NewRow();
								dataRow5["FormId"] = form.FormId;
								dataRow5["ItemId"] = text;
								dataRow5["ColumnId"] = propertyInfo.Name;
								if (propertyInfo.Name.StartsWith("U_"))
								{
									DataRow dataRow6 = listKeyAlias.AsEnumerable().FirstOrDefault((DataRow r) => "U_" + Function.ToString(r["AliasID"]) == propertyInfo.Name);
									if (dataRow6 != null)
									{
										dataRow5["ColumnName"] = dataRow6["Descr"];
									}
									else
									{
										dataRow5["ColumnName"] = propertyInfo.Name;
									}
								}
								else
								{
									dataRow5["ColumnName"] = propertyInfo.Name;
								}
								dataRow5["UserSign"] = GlobalData.UserName;
								dataRow5["Width"] = 100;
								dataRow5["Visible"] = false;
								dataRow5["VisIndex"] = ret.Rows.Count;
								dataRow5["Editable"] = false;
								dataRow5["IsReadOnly"] = true;
								dataRow5["DataType"] = propertyInfo.PropertyType.Name;
								dataRow5["Fixed"] = "N";
								dataRow5["AllowChSet"] = true;
								dataRow5["VisSetting"] = true;
								dataRow5["IsSystem"] = "N";
								dataRow5["LngCode"] = "-";
								dataRow5["NeverEditable"] = LstColumnNotEditable != null && LstColumnNotEditable.Contains(propertyInfo.Name);
								dataRow5["GroupIndex"] = -1;
								dataRow5["CurrField"] = string.Empty;
								ret.Rows.Add(dataRow5);
							}
						}
					}
				}
			}
			frm_FormSetting frm_FormSetting2 = new frm_FormSetting();
			frm_FormSetting2.DtSource = ret;
			frm_FormSetting2.Grid = gridControl as GridControlEx;
			frm_FormSetting2.BandView = this;
			frm_FormSetting2.FormSettingId = form.FormId;
			if (showdialog)
			{
				frm_FormSetting2.ShowInTaskbar = true;
				frm_FormSetting2.ShowDialog();
			}
			else if (form.MdiParent != null)
			{
				frm_FormSetting2.MdiParent = form.MdiParent;
				frm_FormSetting2.Show();
			}
			else
			{
				frm_FormSetting2.ShowDialog();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void RaisePopupMenuShowing(PopupMenuShowingEventArgs e)
	{
		try
		{
			CreateContextMenu();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void CreateContextMenu()
	{
		ContextMenuStrip contextMenuStrip = GridControl.ContextMenuStrip;
		if (contextMenuStrip == null)
		{
			contextMenuStrip = new ContextMenuStrip();
			GridControl.ContextMenuStrip = contextMenuStrip;
		}
		bool flag = true;
		foreach (object item in contextMenuStrip.Items)
		{
			if (item is ToolStripMenuItem toolStripMenuItem && (toolStripMenuItem.Name.ToUpper().Equals("CTM_COPYTABLE") || toolStripMenuItem.Text.ToUpper().Equals("COPY TABLE")))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			ToolStripSeparator value = new ToolStripSeparator();
			contextMenuStrip.Items.Add(value);
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem();
			toolStripMenuItem2.Text = "Copy Table";
			toolStripMenuItem2.Click += CtxCopytable_Click;
			contextMenuStrip.Items.Add(toolStripMenuItem2);
		}
		flag = true;
		foreach (object item2 in contextMenuStrip.Items)
		{
			if (item2 is ToolStripMenuItem toolStripMenuItem3 && (toolStripMenuItem3.Name.ToUpper().Equals("CTM_COPYROWS") || toolStripMenuItem3.Text.ToUpper().Equals("COPY ROW(S)")))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem();
			toolStripMenuItem4.Text = "Copy Row(s)";
			toolStripMenuItem4.Click += CtxCopyrow_Click;
			contextMenuStrip.Items.Add(toolStripMenuItem4);
		}
		if (ChangeLogType == Enums.ChangeLog.clNone)
		{
			return;
		}
		foreach (object item3 in contextMenuStrip.Items)
		{
			if (item3 is ToolStripMenuItem toolStripMenuItem5 && (toolStripMenuItem5.Name.ToUpper().Equals("CTM_CHANGELOG") || toolStripMenuItem5.Text.ToUpper().Equals("CHANGE LOG")))
			{
				flag = false;
				break;
			}
		}
		if (flag)
		{
			ToolStripSeparator value2 = new ToolStripSeparator();
			contextMenuStrip.Items.Add(value2);
			ToolStripMenuItem toolStripMenuItem6 = new ToolStripMenuItem();
			toolStripMenuItem6.Name = "CTM_CHANGELOG";
			toolStripMenuItem6.Text = "Change Log";
			toolStripMenuItem6.Click += ChangeLogItemOnClick;
			contextMenuStrip.Items.Add(toolStripMenuItem6);
		}
	}

	private void ChangeLogItemOnClick(object sender, EventArgs e)
	{
		if (GridControl.FindForm() is XtraFormEx xtraFormEx)
		{
			xtraFormEx.CHANGELOG();
		}
	}

	private void CtxCopyrow_Click(object sender, EventArgs e)
	{
		Export(Enums.ExportExt.Clb);
	}

	private void CtxCopytable_Click(object sender, EventArgs e)
	{
		bool multiSelect = OptionsSelection.MultiSelect;
		OptionsSelection.MultiSelect = true;
		SelectAll();
		Export(Enums.ExportExt.Clb);
		OptionsSelection.MultiSelect = multiSelect;
	}

	protected override void OnColumnAdded(GridColumn column)
	{
		try
		{
			BandedGridColumnEx colEx = column as BandedGridColumnEx;
			DataTable dataTable = GridControl.DataSourceTable();
			if (colEx != null)
			{
				if (colEx.DisplayFormatType == Enums.DisplayFormatType.None && !string.IsNullOrEmpty(column.FieldName) && dataTable != null && dataTable.Columns.Count > 0 && dataTable.Columns.Contains(column.FieldName))
				{
					DataColumn dataColumn = dataTable.Columns[colEx.FieldName];
					if (dataColumn.DataType == typeof(DateTime) || dataColumn.DataType == typeof(DateTime?))
					{
						colEx.DisplayFormat.FormatType = FormatType.DateTime;
						colEx.DisplayFormatType = Enums.DisplayFormatType.DateTime;
						if (colEx.ColumnEdit == null)
						{
							colEx.ColumnEdit = new RepositoryItemDateEditEx();
						}
					}
					else if (dataColumn.DataType == typeof(double) || dataColumn.DataType == typeof(float) || dataColumn.DataType == typeof(double?) || dataColumn.DataType == typeof(float?) || dataColumn.DataType == typeof(decimal) || dataColumn.DataType == typeof(decimal?))
					{
						if (new string[1] { "rate" }.Any((string t) => colEx.FieldName.ToLower().Contains(t)))
						{
							colEx.DisplayFormat.FormatType = FormatType.Numeric;
							colEx.DisplayFormatType = Enums.DisplayFormatType.Rate;
							if (colEx.ColumnEdit == null)
							{
								colEx.ColumnEdit = new RepositoryItemCalcEditEx();
							}
						}
						else if (new string[4] { "percent", "discount", "disc", "prcnt" }.Any((string t) => colEx.FieldName.ToLower().Contains(t)))
						{
							colEx.DisplayFormat.FormatType = FormatType.Numeric;
							colEx.DisplayFormatType = Enums.DisplayFormatType.Percent;
							if (colEx.ColumnEdit == null)
							{
								colEx.ColumnEdit = new RepositoryItemCalcEditEx();
							}
						}
						else
						{
							colEx.DisplayFormat.FormatType = FormatType.Numeric;
							colEx.DisplayFormatType = Enums.DisplayFormatType.Qty;
							if (colEx.ColumnEdit == null)
							{
								colEx.ColumnEdit = new RepositoryItemCalcEditEx();
							}
						}
					}
				}
				if (colEx.ColumnEdit == null)
				{
					switch (colEx.DisplayFormatType)
					{
					case Enums.DisplayFormatType.Qty:
					case Enums.DisplayFormatType.Amount:
					case Enums.DisplayFormatType.Rate:
					case Enums.DisplayFormatType.Percent:
					case Enums.DisplayFormatType.Unit:
						colEx.ColumnEdit = new RepositoryItemCalcEditEx();
						break;
					case Enums.DisplayFormatType.Price:
						colEx.ColumnEdit = new RepositoryItemTextEditExPrice();
						break;
					}
				}
			}
			base.OnColumnAdded(colEx);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			base.OnColumnAdded(column);
		}
	}
}
