using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls.Commons;
using Apzon.Controls.Compiler;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Form.ExchangeRateAndIndex;
using Apzon.Controls.Interfaces;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Export;
using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Localization;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraGrid.Views.Grid.ViewInfo;
using DevExpress.XtraPrinting;
using OfficeOpenXml;
using OfficeOpenXml.Style;

namespace Apzon.Controls.Grid;

public class GridViewEx : GridView
{
	private bool _cancelShowingEditor;

	public Task FirstLoadFormSetting;

	public bool NotLoadFormsettingAtLoad;

	public bool IsValidFromCellValueChanged = true;

	public bool IsCanChangeColumn = true;

	public GridColumn ColumnFirstFilter;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool CancelNextCellValueChangedScript;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool CancelNextSelectedChangedScript;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool CancelNextFocusObjectChangedScript;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public List<Token> SyntaxCellValueChanged;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public List<Token> SyntaxFocusedRowObjectChanged;

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public List<Token> SyntaxSelectionChanged;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataSet _dsContextAction = new DataSet();

	public bool InitPropetiesToNotDesignColumn;

	private readonly List<string> _listMappingField = new List<string>();

	private List<string> _lstChangeField = new List<string>();

	private bool _notSetFiltering;

	private int _rowCount;

	private bool _setFontContextMenu;

	public GridColumnCollectionEx ColumnsEx => Columns as GridColumnCollectionEx;

	public bool IsLoadingData { get; set; }

	public string FormId { get; set; }

	public string ItemId { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string ItemId2 { get; set; }

	private string ChangedKeyValue { get; set; }

	public bool IsInsertNewRow { get; set; }

	public string ColumnKey { get; set; }

	public bool DisplayException { get; set; }

	public bool DisableFormSetting { get; set; }

	public string FieldCurrency { get; set; }

	public string FieldRate { get; set; }

	public Enums.ChangeLog ChangeLogType { get; set; }

	public bool IsApzTable { get; set; }

	public bool UseBatchMod { get; set; }

	public string[] LstColumnNotAllowBatch { get; set; }

	public string[] LstColumnAllowBatch { get; set; }

	[DefaultValue(Enums.LanguageSource.None)]
	public Enums.LanguageSource LanguageSource { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public GridColumnEx EditColumn { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public int IndexEditRow { get; set; }

	public List<string> LstColumNotSetting { get; set; }

	public string[] LstColumnNotEditable { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool HasFormSetting { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool IsMappingContextAction { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public List<string> LstOldUDF { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool SettingReadonly { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool ReadOnly
	{
		get
		{
			return OptionsBehavior.ReadOnly;
		}
		set
		{
			if (SettingReadonly)
			{
				OptionsBehavior.ReadOnly = true;
			}
			else
			{
				OptionsBehavior.ReadOnly = value;
			}
		}
	}

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool IgnoreFilter { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool IsSorting { get; private set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool IsFiltering { get; private set; }

	private bool IsUpdategData { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Enums.HeaderExcelType HeaderExcelType { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public Enums.HeaderExcelAlign HeaderExcelAlign { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public string HeaderExcelExport { get; set; }

	public event RaiseFocusedRowObjectChangedAsync FocusedRowObjectChangedAsync;

	public event CellValueChangedAsync CellValueChangedAsync;

	public event CheckValidChanged CheckValidChanged;

	public GridViewEx()
	{
		InitComponents();
		EditColumn = null;
	}

	public GridViewEx(GridControlEx grid)
		: base(grid)
	{
		InitComponents();
		EditColumn = null;
	}

	protected override GridColumnCollection CreateColumnCollection()
	{
		return new GridColumnCollectionEx(this);
	}

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

	public bool AnyMappingField()
	{
		return _listMappingField.Any();
	}

	public void AddChangedField(string field)
	{
		if (IsUpdategData && !_lstChangeField.Contains(field))
		{
			_lstChangeField.Add(field);
		}
	}

	public void RemoveChangedField(string field)
	{
		_lstChangeField.RemoveAll((string item) => item == field);
	}

	public void ClearListChangedField()
	{
		_lstChangeField.Clear();
	}

	public void InitComponents()
	{
		ChangeLogType = Enums.ChangeLog.clNone;
		OptionsView.ColumnAutoWidth = false;
		IndicatorWidth = 35;
		OptionsView.ShowButtonMode = ShowButtonModeEnum.ShowAlways;
		OptionsView.ShowGroupPanel = false;
		OptionsView.BestFitMaxRowCount = 100;
		OptionsView.ShowGroupedColumns = true;
		CustomDrawRowIndicator += GridViewEx_CustomDrawRowIndicator;
		PopupMenuShowing += GridViewEx_PopupMenuShowing;
		InvalidValueException += (object sender, InvalidValueExceptionEventArgs e) =>
		{
			e.ExceptionMode = ((!DisplayException) ? ExceptionMode.Ignore : ExceptionMode.DisplayError);
		};
		CustomColumnDisplayText += GridViewEx_CustomColumnDisplayText;
		CustomRowFilter += GridViewEx_CustomRowFilter;
		ColumnFilterChanged += (object sender, EventArgs args) =>
		{
			IsFiltering = false;
		};
		StartSorting += (object sender, EventArgs args) =>
		{
			IsSorting = true;
		};
		EndSorting += (object sender, EventArgs args) =>
		{
			IsSorting = false;
		};
		MouseWheel += GridViewEx_MouseWheel;
		OptionsBehavior.AlignGroupSummaryInGroupRow = DefaultBoolean.Default;
		OptionsBehavior.AllowPartialGroups = DefaultBoolean.False;
		OptionsBehavior.AllowFixedGroups = DefaultBoolean.True;
		Appearance.FocusedRow.BackColor = StyleColor.GridEditting;
		OptionsPrint.UsePrintStyles = false;
		OptionsPrint.AutoWidth = false;
		OptionsClipboard.ClipboardMode = ClipboardMode.Formatted;
	}

	public bool GetCheckValidValueStatus(int rowHandle, string columnEdit)
	{
		if (CheckValidChanged != null)
		{
			return CheckValidChanged(rowHandle, columnEdit);
		}
		return true;
	}

	protected override bool RaiseBeforeLeaveRow(RowAllowEventArgs e)
	{
		try
		{
			XtraFormEx xtraFormEx = GridControl.FindForm() as XtraFormEx;
			if (xtraFormEx != null && xtraFormEx.IsFormClosing)
			{
				return base.RaiseBeforeLeaveRow(e);
			}
			if (FocusedColumn is GridColumnEx { IsChangeValue: not false } gridColumnEx)
			{
				if (xtraFormEx != null)
				{
					xtraFormEx.IsCanClose = false;
				}
				gridColumnEx.IsChangeValue = false;
				bool checkValidValueStatus = GetCheckValidValueStatus(FocusedRowHandle, gridColumnEx.FieldName);
				string text = Function.ToString(GetFocusedRowCellValue(gridColumnEx));
				if (checkValidValueStatus && !string.IsNullOrEmpty(text) && gridColumnEx.CflType != Enums.TypeCFL.None && (!IsValidFromCellValueChanged || !_httpClient.Commons.CheckExistsByCodeObject(gridColumnEx.CflType, text)))
				{
					IsValidFromCellValueChanged = true;
					if (gridColumnEx.ColumnEdit != null)
					{
						if (gridColumnEx.ColumnEdit is RepositoryItemButtonEdit repositoryItemButtonEdit)
						{
							MethodInfo[] methods = repositoryItemButtonEdit.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
							ButtonPressedEventArgs e2 = new ButtonPressedEventArgs(repositoryItemButtonEdit.Buttons[0]);
							List<int> listIndexButtonPress = methods.GetListIndexButtonPress();
							for (int i = 0; i < listIndexButtonPress.Count; i++)
							{
								methods[listIndexButtonPress[i]].Invoke(repositoryItemButtonEdit, new object[1] { e2 });
							}
						}
						if (text.Equals(Function.ToString(GetFocusedRowCellValue(gridColumnEx))))
						{
							Focus();
							FocusedColumn = gridColumnEx;
							ShowEditor();
							gridColumnEx.IsChangeValue = true;
							FocusedRowHandle = e.RowHandle;
							e.Allow = false;
						}
						return base.RaiseBeforeLeaveRow(e);
					}
				}
				if (gridColumnEx.FieldName.Equals(FieldCurrency) && Math.Abs(Function.ParseDouble(GetFocusedRowCellValue(FieldRate))) < 1E-08)
				{
					frm_ExchangeRatesAndIndexs frm_ExchangeRatesAndIndexs2 = new frm_ExchangeRatesAndIndexs();
					frm_ExchangeRatesAndIndexs2.Curr = Function.ToString(GetFocusedRowCellValue(FieldCurrency));
					frm_ExchangeRatesAndIndexs2._date = DateTime.Now.Date;
					frm_ExchangeRatesAndIndexs2.ShowDialog();
					if (Math.Abs(frm_ExchangeRatesAndIndexs2.rate) < 1E-07)
					{
						SetFocusedRowCellValue(FieldRate, 0);
						e.Allow = false;
						return base.RaiseBeforeLeaveRow(e);
					}
					gridColumnEx.IsChangeValue = false;
					SetFocusedRowCellValue(FieldRate, frm_ExchangeRatesAndIndexs2.rate);
				}
				if (xtraFormEx != null)
				{
					xtraFormEx.IsCanClose = true;
				}
			}
			if (IsLoadingData)
			{
				e.Allow = false;
			}
			return base.RaiseBeforeLeaveRow(e);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return true;
	}

	protected override async void RaiseFocusedRowObjectChanged(FocusedRowObjectChangedEventArgs e)
	{
		if (IsLoadingData)
		{
			return;
		}
		IsLoadingData = true;
		bool cancelEvent = false;
		GridControl gridControl = GridControl;
		if (gridControl != null && !string.IsNullOrEmpty(FormId) && !string.IsNullOrEmpty(GlobalData.SessionId))
		{
			if (gridControl.FindForm() is XtraFormEx xtraFormEx)
			{
				if (!xtraFormEx.DisableLoadScript)
				{
					if (SyntaxFocusedRowObjectChanged == null)
					{
						string formulaText = _httpClient.Formula.GetFormulaText(FormId, Enums.EventId.OnFocusedRowObjectChanged, ItemId);
						SyntaxFocusedRowObjectChanged = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
					}
					if (SyntaxFocusedRowObjectChanged.Count != 0 && !CancelNextFocusObjectChangedScript)
					{
						CtdMessage ctdMessage = new Apzon.Controls.Compiler.Compiler(Configuration.ApiServer, GlobalData.SessionId).UserdefinedFomular(xtraFormEx, this, SyntaxFocusedRowObjectChanged, out cancelEvent);
						if (!string.IsNullOrEmpty(ctdMessage.Message))
						{
							WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
						}
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
			if (FocusedRowObjectChangedAsync != null)
			{
				await FocusedRowObjectChangedAsync(e);
			}
			base.RaiseFocusedRowObjectChanged(e);
		}
		IsLoadingData = false;
	}

	public void BeginUpdatingData()
	{
		IsUpdategData = true;
	}

	public void EndUpdatingData()
	{
		IsUpdategData = false;
		ClearListChangedField();
	}

	protected override void SetRowCellValueCore(int rowHandle, GridColumn column, object _value, bool fromEditor)
	{
		try
		{
			GridColumnEx col = column as GridColumnEx;
			if (col != null)
			{
				if (_listMappingField.Contains(col.FieldName) || _lstChangeField.Contains(col.FieldName))
				{
					return;
				}
				if (!string.IsNullOrEmpty(col.CurrencyField))
				{
					string[] dataCurrency = Function.ToString(_value).GetDataCurrency();
					if (dataCurrency != null && dataCurrency.Length != 0)
					{
						decimal num = Function.ParseDecimal(dataCurrency[0]);
						string text = Function.ToString(dataCurrency[1]);
						string currency = GlobalData.MainCurrency;
						if ((!fromEditor || string.IsNullOrEmpty(text)) && !string.IsNullOrEmpty(Function.ToString(GetRowCellValue(rowHandle, col.CurrencyField))))
						{
							currency = Function.ToString(GetRowCellValue(rowHandle, col.CurrencyField));
						}
						currency = ((text != "") ? text.ToUpper() : currency);
						if (Function.ToString(GetRowCellValue(rowHandle, col.CurrencyField)) != currency)
						{
							SetRowCellValue(rowHandle, col.CurrencyField, currency);
						}
						if (currency.ToUpper() == GlobalData.MainCurrency.ToUpper() || string.IsNullOrEmpty(currency))
						{
							if (Function.ParseDouble(GetRowCellValue(rowHandle, col.CurrencyRateField)) != 1.0)
							{
								SetRowCellValue(rowHandle, col.CurrencyRateField, 1);
							}
						}
						else
						{
							DateTime date = DateTime.Today;
							string currencyDateField = col.CurrencyDateField;
							if (!string.IsNullOrEmpty(currencyDateField))
							{
								date = Function.ParseDateTime(GetRowCellValue(rowHandle, currencyDateField)) ?? DateTime.Today;
							}
							double exchangeRate = _httpClient.ExRateAndIndex.GetExchangeRate(currency, date);
							if (exchangeRate > 0.0)
							{
								if (Function.ParseDouble(GetRowCellValue(rowHandle, col.CurrencyRateField)) <= 0.0)
								{
									SetRowCellValue(rowHandle, col.CurrencyRateField, exchangeRate);
								}
							}
							else
							{
								frm_ExchangeRatesAndIndexs frm = new frm_ExchangeRatesAndIndexs();
								frm.Curr = text;
								frm._date = date;
								frm.FormClosing += (object s, FormClosingEventArgs e1) =>
								{
									SetRowCellValue(rowHandle, col.CurrencyRateField, Function.ParseDouble(frm.rate));
								};
								frm.ShowDialog();
							}
						}
						if (col.FieldName != col.CurrencyField)
						{
							CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
							if (currencyCodesImpl == null)
							{
								return;
							}
							int num2 = -1;
							if (col.DisplayFormatType == Enums.DisplayFormatType.Price)
							{
								num2 = currencyCodesImpl.Decimals;
								num2 = ((num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
								num = Math.Round(num, num2, MidpointRounding.AwayFromZero);
							}
							else if (col.DisplayFormatType == Enums.DisplayFormatType.Amount)
							{
								num2 = currencyCodesImpl.Decimals;
								num2 = ((num2 == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num2);
								num = Math.Round(num, num2, MidpointRounding.AwayFromZero);
							}
							_value = num;
						}
					}
				}
				if ((col.DisplayFormatType == Enums.DisplayFormatType.LengthAndWidth || col.DisplayFormatType == Enums.DisplayFormatType.Weight) && !string.IsNullOrEmpty(col.UnitField))
				{
					string[] dataUnit = MeasureUnitEditEx.GetDataUnit(Function.ToString(_value).ToLower());
					if (dataUnit != null && dataUnit.Length == 2)
					{
						decimal num3 = Function.ParseDecimal(dataUnit[0]);
						short? unitCodeByDisplayName = Function.ToString(dataUnit[1]).GetUnitCodeByDisplayName((col.DisplayFormatType == Enums.DisplayFormatType.LengthAndWidth) ? Enums.MeasureType.LengthAndWidth : Enums.MeasureType.Weight);
						if (unitCodeByDisplayName.HasValue)
						{
							if (num3 != 0m)
							{
								SetRowCellValue(rowHandle, col.UnitField, unitCodeByDisplayName);
							}
							else
							{
								SetRowCellValue(rowHandle, col.UnitField, DBNull.Value);
							}
							num3 = Math.Round(num3, GlobalData.GeneralSetting.MeasureDec.GetValueOrDefault(), MidpointRounding.AwayFromZero);
							_value = num3;
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.InvalidUnit, "");
							SetRowCellValue(rowHandle, col.UnitField, DBNull.Value);
							_value = null;
						}
					}
				}
				else if (col.DisplayFormatType == Enums.DisplayFormatType.Time && (col.ColumnType == typeof(int) || col.ColumnType == typeof(short)))
				{
					_value = string.Join("", from c in Function.ToString(_value).Where(char.IsDigit)
						select (c));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		base.SetRowCellValueCore(rowHandle, column, _value, fromEditor);
	}

	protected override async void RaiseCellValueChanged(CellValueChangedEventArgs e)
	{
		try
		{
			if (e != null && e.Column != null)
			{
				if (e.Column.FieldName.Equals(ColumnKey))
				{
					ChangedKeyValue = Function.ToString(e.Value);
				}
				try
				{
					XtraFormEx xtraFormEx = GridControl.FindForm() as XtraFormEx;
					if (e.Column is GridColumnEx gridColumnEx)
					{
						if (gridColumnEx.IsChangeValue)
						{
							bool checkValidValueStatus = GetCheckValidValueStatus(e.RowHandle, gridColumnEx.FieldName);
							string text = Function.ToString(GetRowCellValue(e.RowHandle, gridColumnEx));
							if (checkValidValueStatus && !string.IsNullOrEmpty(text) && gridColumnEx.CflType != Enums.TypeCFL.None && !_httpClient.Commons.CheckExistsByCodeObject(gridColumnEx.CflType, text))
							{
								IsValidFromCellValueChanged = false;
								if (xtraFormEx != null && xtraFormEx.IsFormClosing)
								{
									gridColumnEx.IsChangeValue = true;
									FocusedColumn = e.Column;
									ShowEditor();
									return;
								}
								if (gridColumnEx.ColumnEdit != null)
								{
									return;
								}
							}
							if (!string.IsNullOrEmpty(text) && e.Column.FieldName.Equals(FieldCurrency) && Math.Abs(_httpClient.ExRateAndIndex.GetExchangeRate(Function.ToString(e.Value), DateTime.Now.Date)) < 1E-07)
							{
								frm_ExchangeRatesAndIndexs frm_ExchangeRatesAndIndexs2 = new frm_ExchangeRatesAndIndexs();
								frm_ExchangeRatesAndIndexs2.Curr = Function.ToString(GetRowCellValue(e.RowHandle, FieldCurrency));
								frm_ExchangeRatesAndIndexs2._date = DateTime.Now.Date;
								frm_ExchangeRatesAndIndexs2.ShowDialog();
								if (Math.Abs(frm_ExchangeRatesAndIndexs2.rate) < 1E-07)
								{
									SetRowCellValue(e.RowHandle, FieldRate, 0);
									return;
								}
								gridColumnEx.IsChangeValue = false;
								SetRowCellValue(e.RowHandle, FieldRate, frm_ExchangeRatesAndIndexs2.rate);
							}
						}
						if (gridColumnEx.DisplayFormatType == Enums.DisplayFormatType.Qty)
						{
							decimal num = Function.ParseDecimal(GetRowCellValue(e.RowHandle, gridColumnEx));
							int decimals = ((GlobalData.QuantitiesFormat != null) ? GlobalData.QuantitiesFormat.NumberDecimalDigits : 0);
							decimal num2 = Math.Round(num, decimals, MidpointRounding.AwayFromZero);
							if (num2 != num)
							{
								SetRowCellValue(e.RowHandle, gridColumnEx, num2);
								return;
							}
						}
						if (gridColumnEx.DisplayFormatType == Enums.DisplayFormatType.Weight || gridColumnEx.DisplayFormatType == Enums.DisplayFormatType.LengthAndWidth)
						{
							decimal num3 = Function.ParseDecimal(GetRowCellValue(e.RowHandle, gridColumnEx));
							int decimals2 = Function.ParseInt(GlobalData.GeneralSetting.MeasureDec.GetValueOrDefault());
							decimal num4 = Math.Round(num3, decimals2, MidpointRounding.AwayFromZero);
							if (num4 != num3)
							{
								SetRowCellValue(e.RowHandle, gridColumnEx, num4);
								return;
							}
						}
						else if (gridColumnEx.DisplayFormatType == Enums.DisplayFormatType.Price || gridColumnEx.DisplayFormatType == Enums.DisplayFormatType.Amount)
						{
							string currency = string.Empty;
							switch (gridColumnEx.Currency)
							{
							case Enums.CurrencyColumn.MainCurrency:
								currency = GlobalData.MainCurrency;
								break;
							case Enums.CurrencyColumn.SystemCurrency:
								currency = GlobalData.SystemCurrency;
								break;
							case Enums.CurrencyColumn.Custom:
								currency = Function.ToString(GetListSourceRowCellValue(e.RowHandle, gridColumnEx.CurrencyField));
								break;
							}
							int num5 = -1;
							if (!gridColumnEx.FieldName.StartsWith("U_") && (string.IsNullOrEmpty(currency) || currency == ""))
							{
								currency = GlobalData.MainCurrency;
							}
							else if (gridColumnEx.FieldName.StartsWith("U_"))
							{
								currency = "";
							}
							else if (gridColumnEx.Currency == Enums.CurrencyColumn.None)
							{
								currency = "";
							}
							if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
							{
								CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
								if (currencyCodesImpl != null)
								{
									num5 = ((currencyCodesImpl.Decimals == -1) ? num5 : currencyCodesImpl.Decimals);
								}
							}
							decimal num6 = Function.ParseDecimal(GetRowCellValue(e.RowHandle, gridColumnEx));
							decimal num7 = Math.Round(num6, (num5 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num5, MidpointRounding.AwayFromZero);
							if (num7 != num6)
							{
								SetRowCellValue(e.RowHandle, gridColumnEx, num7);
								return;
							}
						}
					}
				}
				catch (Exception ex)
				{
					Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				}
			}
			if (CellValueChangedAsync != null)
			{
				await CellValueChangedAsync(e);
			}
			base.RaiseCellValueChanged(e);
			GridViewEx_CellValueChanged(this, e);
		}
		finally
		{
			if (ActiveEditor != null && !e.Column.FieldName.Equals(FocusedColumn.FieldName))
			{
				ActiveEditor.Dispose();
			}
		}
	}

	private void GridViewEx_CustomRowFilter(object sender, RowFilterEventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(DataController.FilterExpression) || !string.IsNullOrEmpty(ActiveFilterString))
			{
				if (!IsFiltering)
				{
					IsFiltering = true;
				}
				if (DataController.FilterExpression == ActiveFilterString && !_notSetFiltering && ++_rowCount == ((DataTable)GridControl.DataSource).Rows.Count)
				{
					IsFiltering = false;
					_rowCount = 0;
				}
			}
			if (_notSetFiltering)
			{
				bool notSetFiltering = (IsFiltering = false);
				_notSetFiltering = notSetFiltering;
			}
			int focusedDataSourceRowIndex = GetFocusedDataSourceRowIndex();
			if (IgnoreFilter && e.ListSourceRow == focusedDataSourceRowIndex)
			{
				e.Visible = true;
				e.Handled = true;
			}
			else if (IsInsertNewRow && !string.IsNullOrEmpty(ColumnKey))
			{
				string text = Function.ToString(GetListSourceRowCellValue(e.ListSourceRow, ColumnKey));
				if (string.IsNullOrEmpty(text) || e.ListSourceRow == focusedDataSourceRowIndex || text.Equals(ChangedKeyValue))
				{
					e.Visible = true;
					e.Handled = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected virtual void GridViewEx_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		_notSetFiltering = true;
		DataRow dataRow = GetDataRow(e.RowHandle);
		if (dataRow == null || dataRow.RowState == DataRowState.Deleted)
		{
			return;
		}
		GridControl gridControl = GridControl;
		if (gridControl == null || string.IsNullOrEmpty(FormId) || string.IsNullOrEmpty(GlobalData.SessionId))
		{
			return;
		}
		if (gridControl.FindForm() is XtraFormEx xtraFormEx)
		{
			if (xtraFormEx.DisableLoadScript)
			{
				return;
			}
			if (SyntaxCellValueChanged == null)
			{
				string formulaText = _httpClient.Formula.GetFormulaText(FormId, Enums.EventId.OnCellValueChanged, ItemId);
				SyntaxCellValueChanged = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
			}
			if (SyntaxCellValueChanged.Count != 0 && !CancelNextCellValueChangedScript)
			{
				GridColumnEx editColumn = EditColumn;
				EditColumn = e.Column as GridColumnEx;
				CtdMessage ctdMessage = new Apzon.Controls.Compiler.Compiler(Configuration.ApiServer, GlobalData.SessionId, e.RowHandle, e.Column.FieldName).UserdefinedFomular(xtraFormEx, SyntaxCellValueChanged, out var _);
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

	private void GridViewEx_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs events)
	{
		try
		{
			if (!(events.Column is GridColumnEx gridColumnEx))
			{
				return;
			}
			switch (gridColumnEx.DisplayFormatType)
			{
			case Enums.DisplayFormatType.Weight:
			case Enums.DisplayFormatType.LengthAndWidth:
			{
				if (Math.Abs(Function.ParseDouble(events.Value)) < 1E-07)
				{
					events.DisplayText = "";
					break;
				}
				string displayNameUnitByCode = Function.ParseShort(GetListSourceRowCellValue(events.ListSourceRowIndex, gridColumnEx.UnitField)).GetDisplayNameUnitByCode((gridColumnEx.DisplayFormatType == Enums.DisplayFormatType.LengthAndWidth) ? Enums.MeasureType.LengthAndWidth : Enums.MeasureType.Weight);
				events.DisplayText = string.Format("{0:#,0." + new string('#', GlobalData.GeneralSetting.MeasureDec.GetValueOrDefault()) + "} {1}", events.Value, displayNameUnitByCode);
				break;
			}
			case Enums.DisplayFormatType.Price:
			case Enums.DisplayFormatType.Amount:
			{
				if (Math.Abs(Function.ParseDouble(events.Value)) < 1E-07)
				{
					events.DisplayText = "";
					break;
				}
				string currency = string.Empty;
				switch (gridColumnEx.Currency)
				{
				case Enums.CurrencyColumn.MainCurrency:
					currency = GlobalData.MainCurrency;
					break;
				case Enums.CurrencyColumn.SystemCurrency:
					currency = GlobalData.SystemCurrency;
					break;
				case Enums.CurrencyColumn.Custom:
					currency = Function.ToString(GetListSourceRowCellValue(events.ListSourceRowIndex, gridColumnEx.CurrencyField));
					break;
				}
				int num2 = -1;
				if (!gridColumnEx.FieldName.StartsWith("U_") && (string.IsNullOrEmpty(currency) || currency == ""))
				{
					currency = GlobalData.MainCurrency;
				}
				if (gridColumnEx.Currency == Enums.CurrencyColumn.None)
				{
					currency = "";
				}
				if (GlobalData.CurrencyCodes != null && GlobalData.CurrencyCodes.Count > 0)
				{
					CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(currency.ToUpper()));
					if (currencyCodesImpl != null)
					{
						num2 = currencyCodesImpl.Decimals;
					}
				}
				if (gridColumnEx.DisplayFormatType == Enums.DisplayFormatType.Price)
				{
					if (num2 == -1)
					{
						num2 = GlobalData.GeneralSetting.PriceDec.Value;
					}
					string text = new string('0', (num2 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num2);
					events.DisplayText = string.Format("{0:#,0." + text + "} {1}", events.Value, currency);
				}
				else if (gridColumnEx.DisplayFormatType == Enums.DisplayFormatType.Amount)
				{
					if (num2 == -1)
					{
						num2 = GlobalData.GeneralSetting.SumDec.Value;
					}
					string text2 = new string('0', (num2 == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num2);
					events.DisplayText = string.Format("{0:#,0." + text2 + "} {1}", events.Value, currency);
				}
				break;
			}
			case Enums.DisplayFormatType.Time:
			{
				string source = Function.ToString(events.Value);
				source = new string(source.Where((char c) => char.IsDigit(c)).ToArray());
				if (source != "")
				{
					int num = Function.ParseInt(source);
					events.DisplayText = (num / 100).ToString("00") + ":" + (num % 100).ToString("00");
				}
				break;
			}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GridViewEx_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			if (e.MenuType == GridMenuType.Column)
			{
				DXMenuItem dXMenuItem = e.Menu.Items.FirstOrDefault((DXMenuItem i) => i.Caption == GridLocalizer.Active.GetLocalizedString(GridStringId.MenuColumnColumnCustomization));
				if (dXMenuItem != null)
				{
					dXMenuItem.Visible = GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_FormSetting" && x.authoriza == "F");
				}
			}
			CreateContextMenu(e.HitInfo);
			if (GridControl.FindForm() is XtraFormEx { FormAuth: not Enums.FormAuthorities.Read, DisableContextAction: false })
			{
				ReloadContextAction();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void GridViewEx_MouseWheel(object sender, MouseEventArgs e)
	{
		if (Control.ModifierKeys.HasFlag(Keys.Shift))
		{
			(sender as GridView).LeftCoord -= e.Delta;
			((DXMouseEventArgs)e).Handled = true;
		}
	}

	protected override void RaiseShownEditor()
	{
		try
		{
			base.RaiseShownEditor();
			if (_cancelShowingEditor)
			{
				ButtonEdit buttonEdit = ActiveEditor as ButtonEdit;
				if (buttonEdit.Properties.Buttons[0].Visible)
				{
					buttonEdit.Properties.Buttons[0].Enabled = false;
				}
				buttonEdit.Properties.ReadOnly = true;
				_cancelShowingEditor = false;
			}
			else if (ActiveEditor is CalcEdit calcEdit)
			{
				calcEdit.Properties.AllowMouseWheel = false;
			}
		}
		catch (Exception)
		{
		}
	}

	protected override bool RaiseShowingEditor()
	{
		try
		{
			bool flag = !base.RaiseShowingEditor();
			if (flag && FocusedColumn.ColumnEdit is RepositoryItemButtonEdit repositoryItemButtonEdit && repositoryItemButtonEdit.Buttons.Count > 1 && repositoryItemButtonEdit.Buttons[0].Kind == ButtonPredefines.Search && repositoryItemButtonEdit.Buttons[1].Visible)
			{
				_cancelShowingEditor = true;
				return true;
			}
			return !flag;
		}
		catch (Exception)
		{
			return true;
		}
	}

	private void CreateContextMenu(GridHitInfo hitInfo)
	{
		ContextMenuStrip contextMenuStrip = GridControl.ContextMenuStrip;
		if (contextMenuStrip == null)
		{
			contextMenuStrip = new ContextMenuStrip();
			GridControl.ContextMenuStrip = contextMenuStrip;
		}
		if (!_setFontContextMenu)
		{
			contextMenuStrip.Font = new Font(contextMenuStrip.Font.FontFamily, contextMenuStrip.Font.Size + 0.5f, contextMenuStrip.Font.Unit);
			_setFontContextMenu = true;
		}
		IEnumerable<ToolStripMenuItem> source = contextMenuStrip.Items.OfType<ToolStripMenuItem>();
		ToolStripMenuItem toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name == "ctm_CopyCell");
		if (toolStripMenuItem != null)
		{
			toolStripMenuItem.Visible = hitInfo.InRowCell;
		}
		else
		{
			if (contextMenuStrip.Items.Count != 0)
			{
				contextMenuStrip.Items.Add(new ToolStripSeparator
				{
					Visible = !hitInfo.InColumn
				});
			}
			ToolStripMenuItem toolStripMenuItem2 = new ToolStripMenuItem
			{
				Name = "ctm_CopyCell",
				Text = "Copy",
				Visible = hitInfo.InRowCell
			};
			toolStripMenuItem2.Click += CtxCopy_Click;
			contextMenuStrip.Items.Add(toolStripMenuItem2);
		}
		toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name.ToUpper().Equals("CTM_COPYTABLE") || i.Text.ToUpper().Equals("COPY TABLE"));
		if (toolStripMenuItem == null)
		{
			ToolStripMenuItem toolStripMenuItem3 = new ToolStripMenuItem();
			toolStripMenuItem3.Name = "ctm_CopyTable";
			toolStripMenuItem3.Text = "Copy Table";
			toolStripMenuItem3.Click += CtxCopytable_Click;
			contextMenuStrip.Items.Add(toolStripMenuItem3);
		}
		else
		{
			toolStripMenuItem.Visible = !hitInfo.InColumn;
		}
		toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name.ToUpper().Equals("CTM_COPYROWS") || i.Text.ToUpper().Equals("COPY ROW(S)"));
		if (toolStripMenuItem == null)
		{
			ToolStripMenuItem toolStripMenuItem4 = new ToolStripMenuItem();
			toolStripMenuItem4.Name = "ctm_CopyRows";
			toolStripMenuItem4.Text = "Copy Row(s)";
			toolStripMenuItem4.Click += CtxCopyrow_Click;
			contextMenuStrip.Items.Add(toolStripMenuItem4);
		}
		else
		{
			toolStripMenuItem.Visible = !hitInfo.InColumn;
		}
		if (ChangeLogType != Enums.ChangeLog.clNone)
		{
			toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name.ToUpper().Equals("CTM_CHANGELOG") || i.Text.ToUpper().Equals("CHANGE LOG"));
			if (toolStripMenuItem == null)
			{
				ToolStripSeparator value = new ToolStripSeparator();
				contextMenuStrip.Items.Add(value);
				ToolStripMenuItem toolStripMenuItem5 = new ToolStripMenuItem();
				toolStripMenuItem5.Name = "CTM_CHANGELOG";
				toolStripMenuItem5.Text = "Change Log";
				toolStripMenuItem5.Click += ChangeLogItemOnClick;
				contextMenuStrip.Items.Add(toolStripMenuItem5);
			}
		}
		if (UseBatchMod)
		{
			toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name.ToUpper().Equals("CTM_BATCHMOD") || i.Text.ToUpper().Equals("BATCH MODIFICATION"));
			if (toolStripMenuItem == null)
			{
				ToolStripSeparator value2 = new ToolStripSeparator();
				contextMenuStrip.Items.Add(value2);
				ToolStripMenuItem toolStripMenuItem6 = new ToolStripMenuItem();
				toolStripMenuItem6.Name = "CTM_BATCHMOD";
				toolStripMenuItem6.Text = "Batch Modification";
				toolStripMenuItem6.Click += BatchModItemOnClick;
				contextMenuStrip.Items.Add(toolStripMenuItem6);
			}
		}
		if (string.IsNullOrEmpty(FormId) || string.IsNullOrEmpty(ItemId))
		{
			return;
		}
		if (!DisableFormSetting)
		{
			toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name.ToUpper().Equals("CTM_SAVEFORMSETTING") || i.Text.ToUpper().Equals("SAVE FORM SETTING"));
			if (toolStripMenuItem == null && GlobalData.ListAccessForm.Any((AuthsImpl f) => f.menucode == "frm_FormSetting" && f.authoriza == "F"))
			{
				ToolStripSeparator value3 = new ToolStripSeparator();
				contextMenuStrip.Items.Add(value3);
				ToolStripMenuItem toolStripMenuItem7 = new ToolStripMenuItem();
				toolStripMenuItem7.Name = "ctm_SaveFormsetting";
				toolStripMenuItem7.Text = "Save Form Setting";
				toolStripMenuItem7.Click += CtxSaveFormSetting_Click;
				contextMenuStrip.Items.Add(toolStripMenuItem7);
			}
			toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name.Equals("ctm_ReloadFormSetting") || i.Text.Equals("Reload Form Setting"));
			if (toolStripMenuItem == null)
			{
				ToolStripMenuItem toolStripMenuItem8 = new ToolStripMenuItem();
				toolStripMenuItem8.Name = "ctm_ReloadFormSetting";
				toolStripMenuItem8.Text = "Reload Form Setting";
				toolStripMenuItem8.Click += CtxReloadFormSetting_Click;
				contextMenuStrip.Items.Add(toolStripMenuItem8);
			}
		}
		if (GridControl.FindForm() is XtraFormEx { FormAuth: not Enums.FormAuthorities.Read } xtraFormEx)
		{
			toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name.ToUpper().Equals("CTM_FORMATCONDITION") || i.Text.ToUpper().Equals("FORMAT CONDITION"));
			if (toolStripMenuItem == null)
			{
				ToolStripSeparator value4 = new ToolStripSeparator();
				contextMenuStrip.Items.Add(value4);
				ToolStripMenuItem toolStripMenuItem9 = new ToolStripMenuItem();
				toolStripMenuItem9.Name = "CTM_FORMATCONDITION";
				toolStripMenuItem9.Text = "Format Condition";
				toolStripMenuItem9.Click += CtxFormatRule_Click;
				contextMenuStrip.Items.Add(toolStripMenuItem9);
			}
			toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name.ToUpper().Equals("CTM_ACTION") || i.Text.ToUpper().Equals("CONTEXT ACTION"));
			if (toolStripMenuItem == null && !xtraFormEx.DisableContextAction && GlobalData.ListAccessForm.FirstOrDefault((AuthsImpl r) => r.menucode == "barSub_ContextAction" && r.authoriza != "N") != null)
			{
				ToolStripSeparator value5 = new ToolStripSeparator();
				contextMenuStrip.Items.Add(value5);
				ToolStripMenuItem toolStripMenuItem10 = new ToolStripMenuItem();
				toolStripMenuItem10.Name = "CTM_ACTION";
				toolStripMenuItem10.Text = "Context Action";
				toolStripMenuItem10.Click += CtxAction_Click;
				contextMenuStrip.Items.Add(toolStripMenuItem10);
			}
		}
	}

	private void ChangeLogItemOnClick(object sender, EventArgs e)
	{
		if (GridControl.FindForm() is XtraFormEx xtraFormEx)
		{
			xtraFormEx.CHANGELOG();
		}
	}

	private void BatchModItemOnClick(object sender, EventArgs e)
	{
		frm_BatchModification frm_BatchModification2 = new frm_BatchModification();
		frm_BatchModification2.InitGrid(this);
		if (GridControl.FindForm() is XtraFormEx frmParent)
		{
			frm_BatchModification2.ShowDialog(frmParent);
		}
		else
		{
			frm_BatchModification2.ShowDialog();
		}
	}

	private void CtxAction_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			XtraFormEx xtraFormEx = (XtraFormEx)Activator.CreateInstance(Type.GetType("Apzon.Forms.Commons.frm_ContextAction, Apzon.Forms"), FormId, ItemId, GridControl.DataSourceTable(), this);
			if (GridControl.FindForm() is XtraFormEx frmParent)
			{
				xtraFormEx.ShowDialog(frmParent);
			}
			else
			{
				xtraFormEx.ShowDialog();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CtxFormatRule_Click(object sender, EventArgs e)
	{
		try
		{
			frm_FormatRule frm_FormatRule2 = new frm_FormatRule();
			frm_FormatRule2.InitGrid(this);
			if (GridControl.FindForm() is XtraFormEx frmParent)
			{
				frm_FormatRule2.ShowDialog(frmParent);
			}
			else
			{
				frm_FormatRule2.ShowDialog();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RemoveDropDownAction(ToolStripMenuItem ctm)
	{
		for (int i = 0; i < ctm.DropDownItems.Count; i++)
		{
			if (ctm.DropDownItems[i] is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.Name.ToUpper().Equals("CTA_"))
			{
				RemoveDropDownAction(toolStripMenuItem);
				ctm.DropDownItems.Remove(toolStripMenuItem);
				i--;
			}
		}
	}

	private void ReloadContextAction()
	{
		ContextMenuStrip contextMenuStrip = GridControl.ContextMenuStrip;
		if (contextMenuStrip == null)
		{
			contextMenuStrip = new ContextMenuStrip();
			GridControl.ContextMenuStrip = contextMenuStrip;
		}
		for (int i = 0; i < contextMenuStrip.Items.Count; i++)
		{
			if (contextMenuStrip.Items[i] is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.Name.ToUpper().StartsWith("CTA_"))
			{
				RemoveDropDownAction(toolStripMenuItem);
				contextMenuStrip.Items.Remove(toolStripMenuItem);
				i--;
			}
		}
		_dsContextAction = _httpClient.ContextAction.GetData(FormId, ItemId);
		if (!_dsContextAction.IsNotNull())
		{
			return;
		}
		DataTable dataTable = _dsContextAction.Tables[0];
		foreach (DataRow item in from t in dataTable.AsEnumerable()
			where Function.ParseInt(t["FatherId"]) <= 0 && Function.ToString(t["Invisible"]) != "Y"
			select t)
		{
			int absEntry = Function.ParseInt(item["AbsEntry"]);
			string text = "CTA_" + absEntry;
			ToolStripMenuItem toolStripMenuItem2 = CheckExistsContext(contextMenuStrip, text);
			if (toolStripMenuItem2 != null)
			{
				contextMenuStrip.Items.Remove(toolStripMenuItem2);
			}
			toolStripMenuItem2 = new ToolStripMenuItem();
			toolStripMenuItem2.Name = text;
			toolStripMenuItem2.Text = Function.ToString(item["Caption"]);
			string text2 = Function.ToString(item["Icon"]);
			if (!string.IsNullOrEmpty(text2))
			{
				toolStripMenuItem2.Image = ImageResourceCache.Default.GetImageById(text2, ImageSize.Size16x16, ImageType.Office2013);
			}
			int num = Function.ParseInt(item["ActionTypeEx"]);
			if (num != 0)
			{
				toolStripMenuItem2.Click += (object s, EventArgs events) =>
				{
					HandleContextAction(absEntry);
				};
			}
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			if (num != 7000038)
			{
				AddDropDown(absEntry, dataTable, ref toolStripMenuItem2);
			}
		}
	}

	protected virtual void HandleContextAction(int absEntry)
	{
		try
		{
			Activator.CreateInstance(Type.GetType("Apzon.Forms.Commons.ContextActionHandle, Apzon.Forms"), absEntry, FormId, ItemId, GridControl.DataSourceTable(), this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddDropDown(int fatherId, DataTable dt, ref ToolStripMenuItem ctm)
	{
		DataRow[] array = (from t in dt.AsEnumerable()
			where Function.ParseInt(t["FatherId"]) == fatherId && Function.ToString(t["Invisible"]) != "Y"
			select t).ToArray();
		if (array.Length == 0)
		{
			return;
		}
		DataRow[] array2 = array;
		foreach (DataRow dataRow in array2)
		{
			int absEntry = Function.ParseInt(dataRow["AbsEntry"]);
			string text = "CTA_" + absEntry;
			ToolStripMenuItem toolStripMenuItem = CheckExistsContextDropDown(ctm, text);
			if (toolStripMenuItem != null)
			{
				ctm.DropDownItems.Remove(toolStripMenuItem);
			}
			toolStripMenuItem = new ToolStripMenuItem();
			toolStripMenuItem.Name = text;
			toolStripMenuItem.Text = Function.ToString(dataRow["Caption"]);
			string text2 = Function.ToString(dataRow["Icon"]);
			if (!string.IsNullOrEmpty(text2))
			{
				toolStripMenuItem.Image = ImageResourceCache.Default.GetImageById(text2, ImageSize.Size16x16, ImageType.Office2013);
			}
			if (Function.ParseInt(dataRow["ActionTypeEx"]) != 0)
			{
				toolStripMenuItem.Click += (object s, EventArgs events) =>
				{
					HandleContextAction(absEntry);
				};
			}
			ctm.DropDownItems.Add(toolStripMenuItem);
			AddDropDown(absEntry, dt, ref toolStripMenuItem);
		}
	}

	private ToolStripMenuItem CheckExistsContextDropDown(ToolStripMenuItem ctx, string name)
	{
		foreach (object dropDownItem in ctx.DropDownItems)
		{
			if (dropDownItem is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.Name.ToUpper().Equals(name))
			{
				return toolStripMenuItem;
			}
		}
		return null;
	}

	private ToolStripMenuItem CheckExistsContext(ContextMenuStrip ctx, string name)
	{
		foreach (object item in ctx.Items)
		{
			if (item is ToolStripMenuItem toolStripMenuItem && toolStripMenuItem.Name.ToUpper().Equals(name))
			{
				return toolStripMenuItem;
			}
		}
		return null;
	}

	private async void CtxSaveFormSetting_Click(object sender, EventArgs e)
	{
		await this.SaveFormSetting();
	}

	private async void CtxReloadFormSetting_Click(object sender, EventArgs e)
	{
		if (GlobalData.FormSettings.ContainsKey(FormId) && GlobalData.FormSettings[FormId].Tables.Contains(ItemId))
		{
			GlobalData.FormSettings[FormId].Tables.Remove(ItemId);
		}
		await ReloadFormSetting();
	}

	protected void CtxCopyrow_Click(object sender, EventArgs e)
	{
		Export(Enums.ExportExt.Clb);
	}

	protected void CtxCopytable_Click(object sender, EventArgs e)
	{
		bool multiSelect = OptionsSelection.MultiSelect;
		OptionsSelection.MultiSelect = true;
		OptionsClipboard.ClipboardMode = ClipboardMode.Default;
		SelectAll();
		Export(Enums.ExportExt.Clb);
		OptionsSelection.MultiSelect = multiSelect;
	}

	private void CtxCopy_Click(object sender, EventArgs e)
	{
		try
		{
			string focusedDisplayText = GetFocusedDisplayText();
			if (!string.IsNullOrEmpty(focusedDisplayText))
			{
				Clipboard.SetText(focusedDisplayText);
			}
		}
		catch (Exception)
		{
		}
	}

	private void GridViewEx_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle >= 0)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
			else if (e.Info.Kind == IndicatorKind.Header)
			{
				e.Info.Appearance.TextOptions.HAlignment = HorzAlignment.Center;
				e.Info.DisplayText = "#";
			}
		}
		catch (Exception)
		{
		}
	}

	protected override void RaiseRowCountChanged()
	{
		if (RowCount < 100)
		{
			if (IndicatorWidth != 35)
			{
				IndicatorWidth = 35;
			}
		}
		else if (RowCount < 1000)
		{
			if (IndicatorWidth != 40)
			{
				IndicatorWidth = 40;
			}
		}
		else if (RowCount < 10000)
		{
			if (IndicatorWidth != 45)
			{
				IndicatorWidth = 45;
			}
		}
		else if (RowCount < 100000)
		{
			if (IndicatorWidth != 50)
			{
				IndicatorWidth = 50;
			}
		}
		else if (RowCount < 1000000)
		{
			if (IndicatorWidth != 55)
			{
				IndicatorWidth = 55;
			}
		}
		else if (RowCount < 10000000)
		{
			if (IndicatorWidth != 60)
			{
				IndicatorWidth = 60;
			}
		}
		else if (IndicatorWidth != 45)
		{
			IndicatorWidth = 45;
		}
		base.RaiseRowCountChanged();
	}

	protected override void RaiseCellValueChanging(CellValueChangedEventArgs e)
	{
		base.RaiseCellValueChanging(e);
		if (e != null && e.Column != null && e.Column is GridColumnEx { IsChangeValue: false } gridColumnEx)
		{
			gridColumnEx.IsChangeValue = true;
		}
	}

	protected override void RaiseFocusedColumnChanged(GridColumn prevFocused, GridColumn focusedColumn)
	{
		try
		{
			if (!IsCanChangeColumn)
			{
				base.RaiseFocusedColumnChanged(prevFocused, focusedColumn);
				IsCanChangeColumn = true;
				return;
			}
			XtraFormEx xtraFormEx = GridControl.FindForm() as XtraFormEx;
			if (xtraFormEx != null && xtraFormEx.IsFormClosing)
			{
				base.RaiseFocusedColumnChanged(prevFocused, focusedColumn);
				return;
			}
			if (prevFocused is GridColumnEx { IsChangeValue: not false } gridColumnEx)
			{
				if (xtraFormEx != null)
				{
					xtraFormEx.IsCanClose = false;
				}
				gridColumnEx.IsChangeValue = false;
				bool checkValidValueStatus = GetCheckValidValueStatus(FocusedRowHandle, gridColumnEx.FieldName);
				string text = Function.ToString(GetFocusedRowCellValue(gridColumnEx));
				if (checkValidValueStatus && !string.IsNullOrEmpty(text) && gridColumnEx.CflType != Enums.TypeCFL.None && (!IsValidFromCellValueChanged || !_httpClient.Commons.CheckExistsByCodeObject(gridColumnEx.CflType, text)))
				{
					IsValidFromCellValueChanged = true;
					if (gridColumnEx.ColumnEdit != null)
					{
						if (gridColumnEx.ColumnEdit is RepositoryItemButtonEdit repositoryItemButtonEdit)
						{
							MethodInfo[] methods = repositoryItemButtonEdit.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
							ButtonPressedEventArgs e = new ButtonPressedEventArgs(repositoryItemButtonEdit.Buttons[0]);
							List<int> listIndexButtonPress = methods.GetListIndexButtonPress();
							IsCanChangeColumn = false;
							FocusedColumn = prevFocused;
							for (int i = 0; i < listIndexButtonPress.Count; i++)
							{
								methods[listIndexButtonPress[i]].Invoke(repositoryItemButtonEdit, new object[1] { e });
							}
						}
						if (text.Equals(Function.ToString(GetFocusedRowCellValue(prevFocused))))
						{
							Focus();
							FocusedColumn = gridColumnEx;
							ShowEditor();
							gridColumnEx.IsChangeValue = true;
						}
						return;
					}
				}
				if (string.IsNullOrEmpty(FieldCurrency) && gridColumnEx.FieldName.Equals(FieldCurrency) && Math.Abs(Function.ParseDouble(GetFocusedRowCellValue(FieldRate))) < 1E-08)
				{
					frm_ExchangeRatesAndIndexs frm_ExchangeRatesAndIndexs2 = new frm_ExchangeRatesAndIndexs();
					frm_ExchangeRatesAndIndexs2.Curr = Function.ToString(GetFocusedRowCellValue(FieldCurrency));
					frm_ExchangeRatesAndIndexs2._date = DateTime.Now.Date;
					frm_ExchangeRatesAndIndexs2.ShowDialog();
					if (Math.Abs(frm_ExchangeRatesAndIndexs2.rate) < 1E-07)
					{
						SetFocusedRowCellValue(FieldRate, 0);
						return;
					}
					gridColumnEx.IsChangeValue = false;
					SetFocusedRowCellValue(FieldRate, frm_ExchangeRatesAndIndexs2.rate);
				}
				if (xtraFormEx != null)
				{
					xtraFormEx.IsCanClose = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		base.RaiseFocusedColumnChanged(prevFocused, focusedColumn);
	}

	protected override bool EndEditOnLeave()
	{
		try
		{
			XtraFormEx xtraFormEx = GridControl.FindForm() as XtraFormEx;
			if (xtraFormEx != null && xtraFormEx.IsFormClosing)
			{
				return base.EndEditOnLeave();
			}
			if (FocusedColumn is GridColumnEx { IsChangeValue: not false } gridColumnEx)
			{
				if (xtraFormEx != null)
				{
					xtraFormEx.IsCanClose = false;
				}
				gridColumnEx.IsChangeValue = false;
				bool checkValidValueStatus = GetCheckValidValueStatus(FocusedRowHandle, gridColumnEx.FieldName);
				string text = Function.ToString(GetFocusedRowCellValue(gridColumnEx));
				if (checkValidValueStatus && !string.IsNullOrEmpty(text) && gridColumnEx.CflType != Enums.TypeCFL.None && (!IsValidFromCellValueChanged || !_httpClient.Commons.CheckExistsByCodeObject(gridColumnEx.CflType, text)))
				{
					IsValidFromCellValueChanged = true;
					if (gridColumnEx.ColumnEdit != null)
					{
						if (gridColumnEx.ColumnEdit is RepositoryItemButtonEdit repositoryItemButtonEdit)
						{
							MethodInfo[] methods = repositoryItemButtonEdit.GetType().GetMethods(BindingFlags.DeclaredOnly | BindingFlags.Instance | BindingFlags.NonPublic);
							ButtonPressedEventArgs e = new ButtonPressedEventArgs(repositoryItemButtonEdit.Buttons[0]);
							List<int> listIndexButtonPress = methods.GetListIndexButtonPress();
							for (int i = 0; i < listIndexButtonPress.Count; i++)
							{
								methods[listIndexButtonPress[i]].Invoke(repositoryItemButtonEdit, new object[1] { e });
							}
						}
						if (text.Equals(Function.ToString(GetFocusedRowCellValue(gridColumnEx))))
						{
							Focus();
							FocusedColumn = gridColumnEx;
							ShowEditor();
							gridColumnEx.IsChangeValue = true;
							return false;
						}
						return base.EndEditOnLeave();
					}
				}
				if (!string.IsNullOrEmpty(FieldCurrency) && gridColumnEx.FieldName.Equals(FieldCurrency) && Math.Abs(Function.ParseDouble(GetFocusedRowCellValue(FieldRate))) < 1E-08)
				{
					frm_ExchangeRatesAndIndexs frm_ExchangeRatesAndIndexs2 = new frm_ExchangeRatesAndIndexs();
					frm_ExchangeRatesAndIndexs2.Curr = Function.ToString(GetFocusedRowCellValue(FieldCurrency));
					frm_ExchangeRatesAndIndexs2._date = DateTime.Now.Date;
					frm_ExchangeRatesAndIndexs2.ShowDialog();
					if (Math.Abs(frm_ExchangeRatesAndIndexs2.rate) < 1E-07)
					{
						SetFocusedRowCellValue(FieldRate, 0);
						Focus();
						FocusedColumn = gridColumnEx;
						ShowEditor();
						return false;
					}
					gridColumnEx.IsChangeValue = false;
					SetFocusedRowCellValue(FieldRate, frm_ExchangeRatesAndIndexs2.rate);
				}
				if (xtraFormEx != null)
				{
					xtraFormEx.IsCanClose = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return base.EndEditOnLeave();
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
					if (!xtraFormEx.DisableLoadScript)
					{
						if (SyntaxSelectionChanged == null)
						{
							string formulaText = _httpClient.Formula.GetFormulaText(FormId, Enums.EventId.OnSelectionChanged, ItemId);
							SyntaxSelectionChanged = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
						}
						if (SyntaxSelectionChanged.Count != 0 && !CancelNextSelectedChangedScript)
						{
							CtdMessage ctdMessage = new Apzon.Controls.Compiler.Compiler(Configuration.ApiServer, GlobalData.SessionId, e.ControllerRow).UserdefinedFomular(xtraFormEx, SyntaxSelectionChanged, out cancelEvent);
							if (!string.IsNullOrEmpty(ctdMessage.Message))
							{
								WinCommons.Warning(Enums.MessageCode.None, ctdMessage.Message);
							}
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

	protected override void RaiseCustomDrawCell(RowCellCustomDrawEventArgs e)
	{
		base.RaiseCustomDrawCell(e);
		try
		{
			RepositoryItem columnEdit = e.Column.ColumnEdit;
			if (columnEdit != null && columnEdit is RepositoryItemLookUpEdit)
			{
				RepositoryItemLookUpEdit repositoryItemLookUpEdit = columnEdit as RepositoryItemLookUpEdit;
				object rowCellValue = GetRowCellValue(e.RowHandle, e.Column);
				TextEditViewInfo textEditViewInfo = (e.Cell as GridCellInfo).ViewInfo as TextEditViewInfo;
				string text = Function.ToString(rowCellValue);
				if (!string.IsNullOrEmpty(text) && (!Function.IsInt(text) || (text != "0" && text != "-1")) && repositoryItemLookUpEdit.GetDataSourceRowByKeyValue(rowCellValue) == null)
				{
					textEditViewInfo.ShowErrorIcon = true;
					textEditViewInfo.SetDisplayText(text);
					textEditViewInfo.ErrorIconText = "Value does not exist";
					textEditViewInfo.CalcViewInfo(e.Graphics);
				}
				else
				{
					textEditViewInfo.ShowErrorIcon = false;
					textEditViewInfo.ErrorIconText = "";
					textEditViewInfo.CalcViewInfo(e.Graphics);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	public void BeginInsertRow()
	{
		IsInsertNewRow = true;
	}

	public void EndInsertRow()
	{
		ChangedKeyValue = "";
		IsInsertNewRow = false;
	}

	public List<int> GetListDataSourceSelectedRowIndex()
	{
		List<int> list = new List<int>();
		try
		{
			int[] selectedRows = GetSelectedRows();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				int dataSourceRowIndex = GetDataSourceRowIndex(selectedRows[i]);
				if (dataSourceRowIndex >= 0)
				{
					list.Add(dataSourceRowIndex);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return list;
	}

	public List<DataRow> GetListDataSourceSelectedRow()
	{
		List<DataRow> list = new List<DataRow>();
		try
		{
			int[] selectedRows = GetSelectedRows();
			for (int i = 0; i < selectedRows.Length; i++)
			{
				if (selectedRows[i] >= 0)
				{
					DataRow dataRow = GetDataRow(selectedRows[i]);
					if (dataRow != null)
					{
						list.Add(dataRow);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return list;
	}

	public void DeleteDataSourceRowSelected()
	{
		try
		{
			List<DataRow> listDataSourceSelectedRow = GetListDataSourceSelectedRow();
			DataTable dataTable = GridControl.DatasourceTable();
			for (int i = 0; i < listDataSourceSelectedRow.Count; i++)
			{
				dataTable.Rows.Remove(listDataSourceSelectedRow[i]);
			}
			GridControl.DataSource = dataTable;
			GridControl.DatasourceTable().AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void Export(Enums.ExportExt extension)
	{
		string text = "";
		Dictionary<string, Enums.DisplayFormatType> dictionary = new Dictionary<string, Enums.DisplayFormatType>();
		Dictionary<string, RepositoryItem> dictionary2 = new Dictionary<string, RepositoryItem>();
		foreach (GridColumnEx item in Columns.Cast<GridColumnEx>())
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
				item.DisplayFormat.FormatType = FormatType.Numeric;
				item.DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PercentDec ?? 6)}\\%";
				if (item.ColumnEdit != null)
				{
					item.ColumnEdit.DisplayFormat.FormatType = FormatType.Numeric;
					item.ColumnEdit.DisplayFormat.FormatString = $"#,0.{new string('#', GlobalData.GeneralSetting.PercentDec ?? 6)}\\%";
					dictionary2.Add(item.FieldName, item.ColumnEdit);
				}
				break;
			}
		}
		XlsExportOptionsEx options = new XlsExportOptionsEx
		{
			ExportType = ExportType.WYSIWYG,
			TextExportMode = TextExportMode.Text
		};
		XlsxExportOptionsEx options2 = new XlsxExportOptionsEx
		{
			ExportType = ExportType.WYSIWYG,
			TextExportMode = TextExportMode.Text
		};
		switch (extension)
		{
		case Enums.ExportExt.Xls:
			text = ExtensionMethods.PathSaveFile("Excel (2003)", extension);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToXls(text, options);
			}
			break;
		case Enums.ExportExt.Xlsx:
			text = ExtensionMethods.PathSaveFile("Excel (2007)", extension);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToXlsx(text, options2);
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
			text = ExtensionMethods.PathSaveFile("Text", extension);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToText(text);
			}
			break;
		case Enums.ExportExt.Rtf:
			text = ExtensionMethods.PathSaveFile("RTF", extension);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToRtf(text);
			}
			break;
		case Enums.ExportExt.Pdf:
			text = ExtensionMethods.PathSaveFile("PDF", extension);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToPdf(text);
			}
			break;
		case Enums.ExportExt.Html:
			text = ExtensionMethods.PathSaveFile("HTML", extension);
			if (!string.IsNullOrEmpty(text))
			{
				ExportToHtml(text);
			}
			break;
		case Enums.ExportExt.Csv:
			text = ExtensionMethods.PathSaveFile("CSV", extension);
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
				if (Columns[item2.Key] is GridColumnEx gridColumnEx)
				{
					gridColumnEx.DisplayFormatType = item2.Value;
					if (dictionary2.ContainsKey(item2.Key))
					{
						Columns[item2.Key].ColumnEdit = dictionary2[item2.Key];
					}
				}
			}
		}
		if (string.IsNullOrEmpty(text) || !FileIO.FileExist(text))
		{
			return;
		}
		if (extension.ToString().StartsWith("Xls") && HeaderExcelType != Enums.HeaderExcelType.Disabled)
		{
			try
			{
				SetHeaderExcelExport(text);
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", "Add header to excel file", ex.ToString());
				WinCommons.Warning(Enums.MessageCode.None, "Cannot add description to export file.");
			}
		}
		try
		{
			Process.Start(text);
		}
		catch
		{
			XtraMessageBoxEx.Show(Enums.MessageCode.CanNotOpenFile, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand, text);
		}
	}

	private void SetHeaderExcelExport(string pathFile)
	{
		if (!(GridControl.FindForm() is XtraFormEx xtraFormEx))
		{
			return;
		}
		string text = HeaderExcelExport;
		List<IControlEx> lstControlSource = xtraFormEx.LstControlSource;
		for (int i = 0; i < lstControlSource.Count; i++)
		{
			text = text.Replace("[@" + lstControlSource[i].FieldName + "]", xtraFormEx.GetDisplayTextControl(lstControlSource[i].FieldName));
			text = text.Replace("(@" + lstControlSource[i].FieldName + ")", Function.ToString(xtraFormEx.GetDataFormSourceControl(lstControlSource[i].FieldName)));
		}
		using ExcelPackage excelPackage = new ExcelPackage(pathFile);
		ExcelWorksheet excelWorksheet = excelPackage.Workbook.Worksheets.First();
		if (HeaderExcelType == Enums.HeaderExcelType.Merge)
		{
			excelWorksheet.InsertRow(1, 1);
			excelWorksheet.Cells["A1"].Value = text;
			excelWorksheet.Cells["A1"].Style.HorizontalAlignment = ((HeaderExcelAlign == Enums.HeaderExcelAlign.Center) ? ExcelHorizontalAlignment.Center : ((HeaderExcelAlign != Enums.HeaderExcelAlign.Right) ? ExcelHorizontalAlignment.Left : ExcelHorizontalAlignment.Right));
			excelWorksheet.Cells[1, 1, 1, VisibleColumns.Count].Merge = true;
			excelWorksheet.Cells["A1"].Style.WrapText = true;
			excelWorksheet.Cells["A1"].Style.Font.Bold = true;
			int num = text.Count((char c) => c.Equals('\n')) + 1;
			excelWorksheet.Cells["A1"].EntireRow.Height *= num;
			excelWorksheet.View.FreezePanes(3, 1);
		}
		else if (HeaderExcelType == Enums.HeaderExcelType.Split)
		{
			string[] array = text.Split('\n');
			excelWorksheet.InsertRow(1, array.Length);
			for (int num2 = 0; num2 < array.Length; num2++)
			{
				excelWorksheet.Cells[num2 + 1, 1, num2 + 1, VisibleColumns.Count].Merge = true;
				excelWorksheet.Cells["A" + (num2 + 1)].Value = array[num2];
			}
			excelWorksheet.Cells[1, 1, array.Length, VisibleColumns.Count].Style.HorizontalAlignment = ((HeaderExcelAlign == Enums.HeaderExcelAlign.Center) ? ExcelHorizontalAlignment.Center : ((HeaderExcelAlign != Enums.HeaderExcelAlign.Right) ? ExcelHorizontalAlignment.Left : ExcelHorizontalAlignment.Right));
			excelWorksheet.Cells[1, 1, array.Length, VisibleColumns.Count].Style.Font.Bold = true;
			excelWorksheet.View.FreezePanes(array.Length + 2, 1);
		}
		excelPackage.Save();
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
		foreach (GridColumnEx item in Columns.Cast<GridColumnEx>())
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
				item.DisplayFormat.FormatType = FormatType.None;
				if (item.ColumnEdit != null && item.ColumnEdit.DisplayFormat.FormatType != FormatType.None)
				{
					dictionary2.Add(item.FieldName, item.ColumnEdit);
					item.ColumnEdit = null;
				}
				break;
			}
		}
		XlsExportOptionsEx options = new XlsExportOptionsEx
		{
			ExportType = ExportType.WYSIWYG
		};
		XlsxExportOptionsEx options2 = new XlsxExportOptionsEx
		{
			ExportType = ExportType.WYSIWYG
		};
		string fileName = saveFileDialog.FileName;
		switch (new FileInfo(fileName).Extension)
		{
		case ".xls":
			ExportToXls(fileName, options);
			break;
		case ".xlsx":
			ExportToXlsx(fileName, options2);
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
			ExportToXls(fileName, options);
			break;
		}
		if (dictionary.Count > 0)
		{
			foreach (KeyValuePair<string, Enums.DisplayFormatType> item2 in dictionary)
			{
				if (Columns[item2.Key] is GridColumnEx gridColumnEx)
				{
					gridColumnEx.DisplayFormatType = item2.Value;
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

	private List<string> GetListColumReadOnly()
	{
		try
		{
			List<string> list = new List<string>();
			for (int i = 0; i < Columns.Count; i++)
			{
				if (Columns[i].OptionsColumn.ReadOnly || !Columns[i].OptionsColumn.AllowEdit)
				{
					list.Add(Columns[i].FieldName);
				}
			}
			return list;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public async Task ReloadFormSetting(Enums.PivotFormSetting pivotType = Enums.PivotFormSetting.None, bool reloadGrid = true)
	{
		if (string.IsNullOrEmpty(FormId) || string.IsNullOrEmpty(ItemId) || new string[5] { "oUserDefinedReports", "oUserQueries", "oReportFormSetting", "oSalaryResult", "oSalaryResultView" }.Contains(FormId))
		{
			return;
		}
		try
		{
			BeginUpdate();
			GridMultiSelectMode selectMode = OptionsSelection.MultiSelectMode;
			if (selectMode == GridMultiSelectMode.CheckBoxRowSelect)
			{
				OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
			}
			string itemId = ((!string.IsNullOrEmpty(ItemId2)) ? ItemId2 : ItemId);
			DataTable dataTable;
			if (GlobalData.FormSettings.ContainsKey(FormId) && GlobalData.FormSettings[FormId].Tables.Contains(itemId))
			{
				dataTable = GlobalData.FormSettings[FormId].Tables[itemId];
			}
			else
			{
				dataTable = await _httpClient.FormSettingClient.GetFormSetting(FormId, itemId, GlobalData.UserName, GlobalData.CURRENTLANGUAGE);
				if (dataTable != null && dataTable.Columns.Count != 0)
				{
					if (!GlobalData.FormSettings.ContainsKey(FormId))
					{
						GlobalData.FormSettings.Add(FormId, new DataSet());
					}
					if (!GlobalData.FormSettings[FormId].Tables.Contains(itemId))
					{
						dataTable.TableName = itemId;
						GlobalData.FormSettings[FormId].Tables.Add(dataTable);
					}
				}
				else if (reloadGrid)
				{
					for (int num = VisibleColumns.Count - 1; num >= 0; num--)
					{
						VisibleColumns[num].Visible = false;
					}
					HasFormSetting = true;
					return;
				}
			}
			if (!reloadGrid)
			{
				return;
			}
			if (dataTable.Rows.Count > 0)
			{
				HasFormSetting = true;
				LstOldUDF = (from r in dataTable.AsEnumerable()
					where Function.ToString(r["ColumnId"]).StartsWith("U_")
					select Function.ToString(r["ColumnId"])).ToList();
				if ((uint)(pivotType - 1) <= 1u)
				{
					dataTable = (from t in dataTable.AsEnumerable()
						where !char.IsDigit(Function.ToString(t["ColumnId"])[0])
						select t).CopyToDataTable();
				}
				GridColumn[] array = Columns.Where((GridColumn t) => t.Fixed != FixedStyle.None).ToArray();
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					array[num2].Fixed = FixedStyle.None;
				}
				List<DataRow> list = (from t in dataTable.AsEnumerable()
					orderby Function.ParseInt(t["VisIndex"])
					select t).ToList();
				var list2 = (from r in dataTable.AsEnumerable()
					where Function.ParseInt(r["GroupIndex"]) >= 0
					select new
					{
						FieldName = Function.ToString(r["ColumnId"]),
						GroupIndex = Function.ParseInt(r["GroupIndex"])
					}).ToList();
				if (Columns.Any((GridColumn c) => c.GroupIndex >= 0) || list2.Count > 0)
				{
					BeginLockFocusedRowChange();
					ClearGrouping();
				}
				for (int num3 = 0; num3 < list.Count; num3++)
				{
					DataRow dataRow = list[num3];
					string text = Function.ToString(dataRow["ColumnId"]);
					if (!Function.ParseBool(dataRow["VisSetting"]))
					{
						continue;
					}
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
					try
					{
						if (!Columns.Any((GridColumn n) => n.FieldName == setting.ColumnId))
						{
							if (!setting.Visible)
							{
								continue;
							}
							GridColumnEx column = new GridColumnEx
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
					}
					catch (Exception)
					{
					}
					if (Columns.FirstOrDefault((GridColumn t) => t.FieldName.Equals(setting.ColumnId)) is GridColumnEx gridColumnEx)
					{
						if (!string.IsNullOrEmpty(setting.ColumnName))
						{
							gridColumnEx.Caption = setting.ColumnName;
						}
						else
						{
							gridColumnEx.Caption = setting.ColumnId;
						}
						gridColumnEx.Width = setting.Width;
						bool flag = setting.Visible && (LstColumNotSetting == null || !LstColumNotSetting.Contains(gridColumnEx.FieldName));
						if (flag)
						{
							gridColumnEx.VisibleIndex = setting.VisIndex;
							gridColumnEx.AbsoluteIndex = setting.VisIndex;
						}
						gridColumnEx.Visible = flag;
						gridColumnEx.OptionsFilter.AllowFilter = gridColumnEx.Visible;
						gridColumnEx.OptionsColumn.ReadOnly = !setting.Editable;
						gridColumnEx.Fixed = ((setting.Fixed == "L") ? FixedStyle.Left : ((setting.Fixed == "R") ? FixedStyle.Right : FixedStyle.None));
						gridColumnEx.AllowChSet = setting.AllowChSet;
						if (!string.IsNullOrWhiteSpace(Function.ToString(setting.CurrField)))
						{
							gridColumnEx.Currency = Enums.CurrencyColumn.Custom;
							gridColumnEx.CurrencyField = setting.CurrField;
						}
					}
				}
				foreach (var item in list2)
				{
					Columns[item.FieldName].GroupIndex = item.GroupIndex;
				}
			}
			else
			{
				HasFormSetting = false;
			}
			FixEditorDateTime();
			SetDistRuleColsCaption();
			HideColumnsInColumnChooser();
			OptionsSelection.MultiSelectMode = selectMode;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", "ReloadFormsetting", ex2.ToString());
			WinCommons.ErrorLineNumber(ex2);
		}
		finally
		{
			if (IsFocusedRowChangeLocked)
			{
				EndLockFocusedRowChange();
			}
			EndUpdate();
		}
	}

	private void FixEditorDateTime()
	{
		GridColumn[] array = Columns.Where((GridColumn c) => c is GridColumnEx { DisplayFormatType: Enums.DisplayFormatType.DateTime } gridColumnEx && gridColumnEx.ColumnEdit == null).ToArray();
		for (int num = 0; num < array.Length; num++)
		{
			array[num].ColumnEdit = new RepositoryItemDateEditEx();
		}
	}

	public void HideColumnsInColumnChooser()
	{
		if (LstColumNotSetting == null)
		{
			return;
		}
		foreach (string item in LstColumNotSetting)
		{
			GridColumn gridColumn = Columns[item];
			if (gridColumn != null)
			{
				gridColumn.OptionsColumn.ShowInCustomizationForm = false;
			}
		}
	}

	public void BeginLockFocusRowChange()
	{
		BeginLockFocusedRowChange();
	}

	public void EndLockFocusRowChange()
	{
		if (IsFocusedRowChangeLocked)
		{
			EndLockFocusedRowChange();
		}
	}

	private void SetDistRuleColsCaption()
	{
		try
		{
			if (GlobalData.Dimensions == null || GlobalData.Dimensions.Rows.Count <= 0)
			{
				return;
			}
			foreach (DataRow row in GlobalData.Dimensions.Rows)
			{
				string dimCode = ((Function.ToString(row["DimCode"]) == "1") ? "OcrCode" : ("OcrCode" + Function.ToString(row["DimCode"])));
				GridColumn gridColumn = Columns.FirstOrDefault((GridColumn x) => (x.FieldName.ToUpper() == "OcrCode".ToUpper() && Function.ToString(x.Tag).ToUpper() == dimCode.ToUpper()) || x.FieldName.ToUpper() == dimCode.ToUpper());
				if (gridColumn != null)
				{
					gridColumn.Caption = ((row["DimDesc"] == null || row["DimDesc"] == DBNull.Value) ? "" : Function.ToString(row["DimDesc"]));
				}
			}
			if (LstColumNotSetting == null)
			{
				LstColumNotSetting = new List<string>();
			}
			if (!GlobalData.Dimensions.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["DimCode"]) == 1))
			{
				if (Columns["OcrCode"] != null && Columns["OcrCode"].Visible)
				{
					Columns["OcrCode"].Visible = false;
				}
				if (!LstColumNotSetting.Contains("OcrCode"))
				{
					LstColumNotSetting.Add("OcrCode");
				}
			}
			int i;
			for (i = 2; i <= 5; i++)
			{
				if (!GlobalData.Dimensions.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["DimCode"]) == i))
				{
					string text = "OcrCode" + i;
					if (Columns[text] != null && Columns[text].Visible)
					{
						Columns[text].Visible = false;
					}
					if (!LstColumNotSetting.Contains(text))
					{
						LstColumNotSetting.Add(text);
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

	public void ShowFormSetting(XtraFormEx form, bool showdialog = false)
	{
		try
		{
			if (DisableFormSetting || string.IsNullOrEmpty(FormId) || string.IsNullOrEmpty(ItemId))
			{
				WinCommons.Warning(Enums.MessageCode.FormsettingNotAvailable, "");
				return;
			}
			List<string> list = LstColumNotSetting ?? new List<string>();
			string text = ((!string.IsNullOrEmpty(ItemId2)) ? ItemId2 : ItemId);
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
			IEnumerable<GridColumnEx> enumerable = ((!(ItemId != "APZ_OTSL")) ? (from t in Columns
				where !char.IsDigit(t.FieldName[0])
				orderby t.Visible descending, t.VisibleIndex
				select t).Cast<GridColumnEx>() : (from t in Columns
				orderby t.Visible descending, t.VisibleIndex
				select t).Cast<GridColumnEx>());
			Dictionary<string, Tuple<bool, bool>> dictionary = new Dictionary<string, Tuple<bool, bool>>();
			if (GlobalData.FormSettings.ContainsKey(FormId) && GlobalData.FormSettings[FormId].Tables.Contains(text))
			{
				dictionary = (from r in GlobalData.FormSettings[FormId].Tables[text].AsEnumerable()
					where Function.ParseBool(r["Visible"])
					select r).ToDictionary((DataRow r) => Function.ToString(r["ColumnId"]), (DataRow r) => new Tuple<bool, bool>(Function.ParseBool(r["Editable"]), Function.ParseBool(r["IsReadOnly"])));
			}
			foreach (GridColumnEx col in enumerable)
			{
				if (!list.Contains(col.FieldName) && ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Trim().Equals(col.FieldName.Trim())) == null)
				{
					DataRow dataRow = ret.NewRow();
					dataRow["FormId"] = form.FormId;
					dataRow["ItemId"] = text;
					dataRow["ColumnId"] = col.FieldName;
					dataRow["ColumnName"] = (string.IsNullOrEmpty(col.Caption) ? col.FieldName : col.Caption);
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
						dataRow["Editable"] = col.Visible && (dictionary.ContainsKey(col.FieldName) ? dictionary[col.FieldName].Item1 : (!col.OptionsColumn.ReadOnly));
						dataRow["NeverEditable"] = false;
					}
					dataRow["IsReadOnly"] = (dictionary.ContainsKey(col.FieldName) ? dictionary[col.FieldName].Item2 : col.OptionsColumn.ReadOnly);
					dataRow["DataType"] = Function.ToString(col.ColumnType);
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
					DataColumn[] array = ((!(ItemId != "APZ_OTSL")) ? (from DataColumn t in gridControl.DataSourceTable().Columns
						where !char.IsDigit(t.ColumnName[0]) && ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Trim().Equals(t.ColumnName.Trim())) == null
						select t).ToArray() : (from DataColumn t in gridControl.DataSourceTable().Columns
						where ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Trim().Equals(t.ColumnName.Trim())) == null
						select t).ToArray());
					foreach (DataColumn column in array)
					{
						if (list.Contains(column.ColumnName) || ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Trim().Equals(column.ColumnName.Trim())) != null)
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
						dataRow3["CurrField"] = "";
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
								if (ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Trim().Equals(propertyInfo.Name.Trim())) != null || list.Contains(propertyInfo.Name))
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
								dataRow5["CurrField"] = "";
								ret.Rows.Add(dataRow5);
							}
						}
					}
				}
			}
			frm_FormSetting frm_FormSetting2 = new frm_FormSetting();
			frm_FormSetting2.DtSource = ret;
			frm_FormSetting2.Grid = gridControl as GridControlEx;
			frm_FormSetting2.Item = new Tuple<GridViewEx, string>(this, text);
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

	public void InitFormatColumnWhenChangeSource(DataTable dtSource)
	{
		try
		{
			if (InitPropetiesToNotDesignColumn || dtSource == null || dtSource.Columns.Count == 0)
			{
				return;
			}
			foreach (GridColumnEx item in Columns.Where((GridColumn t) =>
			{
				if (!string.IsNullOrEmpty(t.FieldName) && t.ColumnEdit == null && t is GridColumnEx { DisplayFormatType: Enums.DisplayFormatType.None })
				{
					DataColumn dataColumn = dtSource.Columns[t.FieldName];
					if (dataColumn != null)
					{
						return dataColumn.DataType == typeof(DateTime);
					}
				}
				return false;
			}).ToList())
			{
				item.ColumnEdit = new RepositoryItemDateEditEx();
				item.DisplayFormat.FormatType = FormatType.DateTime;
				item.DisplayFormatType = Enums.DisplayFormatType.DateTime;
			}
			foreach (GridColumnEx item2 in Columns.Where((GridColumn t) =>
			{
				if (!string.IsNullOrEmpty(t.FieldName) && t.ColumnEdit == null && t is GridColumnEx { DisplayFormatType: Enums.DisplayFormatType.None } gridColumnEx3 && gridColumnEx3.DisplayFormat.FormatType == FormatType.None)
				{
					DataColumn dataColumn = dtSource.Columns[t.FieldName];
					if (dataColumn != null)
					{
						return dataColumn.DataType == typeof(decimal);
					}
				}
				return false;
			}).ToList())
			{
				item2.ColumnEdit = new RepositoryItemCalcEditEx();
				item2.DisplayFormat.FormatType = FormatType.Numeric;
				item2.DisplayFormat.FormatString = "#" + GlobalData.GeneralSetting.ThousSep + "##0" + GlobalData.GeneralSetting.DecSep + "######";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void SetViewModeButtonEdit(bool isEditable, bool isHeader = true)
	{
		try
		{
			for (int i = 0; i < Columns.Count; i++)
			{
				if ((!isEditable & isHeader) && Columns[i].FieldName.StartsWith("U_"))
				{
					continue;
				}
				RepositoryItem columnEdit = Columns[i].ColumnEdit;
				if (columnEdit is RepositoryItemButtonEdit)
				{
					RepositoryItemButtonEdit repositoryItemButtonEdit = columnEdit as RepositoryItemButtonEdit;
					if (repositoryItemButtonEdit.Buttons.Count > 0)
					{
						repositoryItemButtonEdit.Buttons[0].Visible = isEditable;
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

	protected override void OnColumnAdded(GridColumn column)
	{
		try
		{
			GridColumnEx colEx = column as GridColumnEx;
			if (colEx != null)
			{
				DataTable dataTable = GridControl.DataSourceTable();
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
