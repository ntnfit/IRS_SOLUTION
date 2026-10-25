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
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls.Commons;
using Apzon.Controls.Compiler;
using Apzon.Controls.Editor;
using Apzon.Controls.Editor.TextEditExPrice;
using Apzon.Controls.Form;
using Apzon.Controls.Form.ExchangeRateAndIndex;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.BusinessPartners;
using Apzon.Entities.Models.Common;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Export;
using DevExpress.Images;
using DevExpress.Utils;
using DevExpress.Utils.Design;
using DevExpress.Utils.Drawing;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraPrinting;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Localization;
using DevExpress.XtraTreeList.Menu;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Controls.Tree;

[ToolboxItem(true)]
public class TreeEx : TreeList
{
	public Task FirstLoadFormSetting;

	public bool NotLoadFormsettingAtLoad;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private DataSet _dsContextAction = new DataSet();

	private bool _cancelShowingEditor;

	public string IdColumn = "ID";

	public string FatherColumn = "FatherId";

	public List<Formular> Formulars = new List<Formular>();

	public Dictionary<string, TreeList> TblFormular = new Dictionary<string, TreeList>();

	private List<Token> SyntaxOnCellValueChanged;

	private List<Token> SyntaxOnFocusedRowObjectChanged;

	private readonly List<string> _listMappingField = new List<string>();

	private bool _setFontContextMenu;

	public string ItemId { get; set; }

	public string FormId { get; set; }

	public bool AllowPasteExcel { get; set; }

	public bool DisableFormSetting { get; set; }

	public string FieldCurrency { get; set; }

	public string FieldRate { get; set; }

	public string ColumnKey { get; set; }

	private string ChangedKeyValue { get; set; }

	public List<string> LstColumNotSetting { get; set; }

	public string[] LstColumnNotEditable { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool HasFormSetting { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public List<string> LstOldUDF { get; set; }

	protected FieldValueMissing EditorValueMissing { get; set; }

	public TreeListColumnEx EditColumn { get; set; }

	public int IndexEditRow { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool CancelNextCellValueChangedScript { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool CancelNextSelectedChangedScript { get; set; }

	[Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	public bool CancelNextFocusObjectChangedScript { get; set; }

	public bool IsLoadingData { get; set; }

	public Enums.ChangeLog ChangeLogType { get; set; }

	public bool IsApzTable { get; set; }

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
	public DataTable DataSourceTable
	{
		get
		{
			if (DataSource is DataTable result)
			{
				return result;
			}
			return new DataTable();
		}
	}

	public TreeListColumnCollectionEx ColumnsEx => Columns as TreeListColumnCollectionEx;

	public new event CellValueChangedEventHandler CellValueChanged;

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

	public List<TreeListNode> GetCheckedNodeCollection()
	{
		CustomTreeListNodeOperation customTreeListNodeOperation = new CustomTreeListNodeOperation();
		NodesIterator.DoOperation(customTreeListNodeOperation);
		return customTreeListNodeOperation.CheckedNodeCollection;
	}

	public List<DataRow> GetListDataSourceSelectedRow()
	{
		DataTable dataTable = DataSourceTable.Clone();
		if (dataTable.Columns.Count == 0)
		{
			return null;
		}
		foreach (TreeListNode item in GetCheckedNodeCollection())
		{
			dataTable.ImportRow(GetDataRow(item.Id));
		}
		dataTable.AcceptChanges();
		return dataTable.Rows.Cast<DataRow>().ToList();
	}

	public TreeEx()
	{
		try
		{
			IndicatorWidth = 40;
			OptionsNavigation.AutoMoveRowFocus = true;
			PopupMenuShowing += TreeEx_PopupMenuShowing;
			CustomColumnDisplayText += TreeEx_CustomColumnDisplayText;
			CustomDrawColumnHeader += TreeEx_CustomDrawColumnHeader;
			CustomDrawNodeIndicator += TreeEx_CustomDrawNodeIndicator;
			CellValueChanged += TreeListEx_CellValueChanged;
			ValidatingEditor += TreeEx_ValidatingEditor;
			MouseWheel += TreeListEx_MouseWheel;
			ChangeLogType = Enums.ChangeLog.clNone;
			FormId = (FindForm() as XtraFormEx)?.FormId;
			IndexEditRow = -1;
			EditColumn = null;
			OptionsMenu.ShowExpandCollapseItems = false;
			if (LicenseManager.UsageMode != LicenseUsageMode.Runtime)
			{
				return;
			}
			Load += TreeEx_Load;
			Enter += (object sender, EventArgs e) =>
			{
				if (FindForm() is XtraFormEx { Modal: false } && XtraFormEx.EnableDirectionalControl != null)
				{
					Enums.DirectionalControl[] gridToolbarButtons = XtraFormEx.GridToolbarButtons;
					foreach (Enums.DirectionalControl diectioControl in gridToolbarButtons)
					{
						XtraFormEx.EnableDirectionalControl(diectioControl, isEnable: true);
					}
				}
			};
			Leave += (object sender, EventArgs e) =>
			{
				if (FindForm() is XtraFormEx { Modal: false })
				{
					Enums.DirectionalControl[] gridToolbarButtons = XtraFormEx.GridToolbarButtons;
					foreach (Enums.DirectionalControl diectioControl in gridToolbarButtons)
					{
						XtraFormEx.EnableDirectionalControl(diectioControl, isEnable: false);
					}
				}
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void TreeEx_CustomDrawNodeIndicator(object sender, CustomDrawNodeIndicatorEventArgs e)
	{
		if (e.IsNodeIndicator)
		{
			int visibleIndexByNode = GetVisibleIndexByNode(e.Node);
			if (visibleIndexByNode >= 0)
			{
				e.Info.DisplayText = (visibleIndexByNode + 1).ToString();
			}
		}
	}

	private void TreeEx_CustomDrawColumnHeader(object sender, CustomDrawColumnHeaderEventArgs e)
	{
		if (!e.Info.IsColumn && e.Info.HeaderPosition == HeaderPositionKind.Left)
		{
			using (StringFormat stringFormat = new StringFormat(e.Appearance.GetStringFormat()))
			{
				stringFormat.Alignment = StringAlignment.Center;
				e.DefaultDraw();
				e.Cache.DrawString("#", e.Appearance.Font, e.Appearance.ForeColor, e.CaptionRect, stringFormat);
				e.Handled = true;
			}
		}
	}

	private async void TreeEx_Load(object sender, EventArgs e)
	{
		if (!string.IsNullOrEmpty(FormId) && !string.IsNullOrEmpty(ItemId) && !DisableFormSetting)
		{
			Visible = false;
			if (!NotLoadFormsettingAtLoad && FirstLoadFormSetting == null)
			{
				FirstLoadFormSetting = ReloadFormSetting();
			}
			if (FirstLoadFormSetting != null)
			{
				await FirstLoadFormSetting;
			}
			Visible = true;
		}
		else
		{
			HideColumnInColumnChooser();
		}
	}

	private void HideColumnInColumnChooser()
	{
		foreach (TreeListColumn column in Columns)
		{
			if (!column.Visible)
			{
				column.OptionsColumn.ShowInCustomizationForm = false;
			}
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
		}
		catch (Exception)
		{
		}
	}

	protected override void RaiseShowingEditor(ref bool cancel)
	{
		base.RaiseShowingEditor(ref cancel);
		if (cancel && FocusedColumn.ColumnEdit is RepositoryItemButtonEdit repositoryItemButtonEdit && repositoryItemButtonEdit.Buttons[0].Kind == ButtonPredefines.Search)
		{
			_cancelShowingEditor = true;
			cancel = false;
		}
	}

	protected override void EndEdit(bool cancel)
	{
		try
		{
			XtraFormEx xtraFormEx = FindForm() as XtraFormEx;
			if (xtraFormEx != null && xtraFormEx.IsFormClosing)
			{
				base.EndEdit(cancel);
				return;
			}
			if (FocusedColumn is TreeListColumnEx treeListColumnEx)
			{
				if (xtraFormEx != null)
				{
					xtraFormEx.IsCanClose = false;
				}
				if (!string.IsNullOrEmpty(FieldCurrency) && treeListColumnEx.FieldName.Equals(FieldCurrency) && Math.Abs(Function.ParseDouble(GetFocusedRowCellValue(Columns[FieldRate]))) < 1E-08)
				{
					frm_ExchangeRatesAndIndexs frm_ExchangeRatesAndIndexs2 = new frm_ExchangeRatesAndIndexs();
					frm_ExchangeRatesAndIndexs2.Curr = Function.ToString(GetFocusedRowCellValue(Columns[FieldCurrency]));
					frm_ExchangeRatesAndIndexs2._date = DateTime.Now.Date;
					frm_ExchangeRatesAndIndexs2.ShowDialog();
					if (Math.Abs(frm_ExchangeRatesAndIndexs2.rate) < 1E-07)
					{
						SetFocusedRowCellValue(FieldRate, 0);
						Focus();
						FocusedColumn = treeListColumnEx;
						ShowEditor();
						base.EndEdit(cancel);
						return;
					}
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
		base.EndEdit(cancel);
	}

	protected override void RaiseCellValueChanged(CellValueChangedEventArgs e)
	{
		try
		{
			if (ActiveEditor == null && EditorValueMissing != null && EditorValueMissing.FieldName.Equals(e.Column.FieldName))
			{
				e = new CellValueChangedEventArgs(e.Column, e.Node, EditorValueMissing.Value);
				EditorValueMissing = null;
				base.OnSetValue(e.Node, e.Column, e.Value);
			}
			if (e.Column is TreeListColumnEx treeListColumnEx)
			{
				if (treeListColumnEx.DisplayFormatType == Enums.DisplayFormatType.Qty)
				{
					decimal num = Function.ParseDecimal(GetRowCellValue(e.Node, treeListColumnEx));
					int decimals = Function.ParseInt((GlobalData.QuantitiesFormat != null) ? GlobalData.QuantitiesFormat.NumberDecimalDigits : 0);
					decimal num2 = Math.Round(num, decimals, MidpointRounding.AwayFromZero);
					if (num2 != num)
					{
						SetRowCellValue(e.Node, treeListColumnEx, num2);
						return;
					}
				}
				if (treeListColumnEx.DisplayFormatType == Enums.DisplayFormatType.Weight || treeListColumnEx.DisplayFormatType == Enums.DisplayFormatType.LengthAndWidth)
				{
					decimal num3 = Function.ParseDecimal(GetRowCellValue(e.Node, treeListColumnEx));
					int decimals2 = Function.ParseInt(GlobalData.GeneralSetting.MeasureDec.GetValueOrDefault());
					decimal num4 = Math.Round(num3, decimals2);
					if (num4 != num3)
					{
						SetRowCellValue(e.Node, treeListColumnEx, num4);
						return;
					}
				}
				else if (treeListColumnEx.DisplayFormatType == Enums.DisplayFormatType.Price || treeListColumnEx.DisplayFormatType == Enums.DisplayFormatType.Amount)
				{
					string currency = string.Empty;
					switch (treeListColumnEx.Currency)
					{
					case Enums.CurrencyColumn.MainCurrency:
						currency = GlobalData.MainCurrency;
						break;
					case Enums.CurrencyColumn.SystemCurrency:
						currency = GlobalData.SystemCurrency;
						break;
					case Enums.CurrencyColumn.Custom:
						currency = Function.ToString(GetRowCellValue(e.Node, Columns[treeListColumnEx.CurrencyField]));
						break;
					}
					int num5 = -1;
					if (!treeListColumnEx.FieldName.StartsWith("U_") && (string.IsNullOrEmpty(currency) || currency == ""))
					{
						currency = GlobalData.MainCurrency;
					}
					else if (treeListColumnEx.FieldName.StartsWith("U_"))
					{
						currency = "";
					}
					else if (treeListColumnEx.Currency == Enums.CurrencyColumn.None)
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
					decimal num6 = Function.ParseDecimal(GetRowCellValue(e.Node, treeListColumnEx));
					decimal num7 = Math.Round(num6, (num5 == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num5, MidpointRounding.AwayFromZero);
					if (num7 != num6)
					{
						SetRowCellValue(e.Node, treeListColumnEx, num7);
						return;
					}
				}
			}
		}
		catch (Exception)
		{
			throw;
		}
		base.RaiseCellValueChanged(e);
		if (e.Column != null)
		{
			CellValueChanged(this, e);
		}
	}

	protected override void RaiseValidatingEditor(BaseContainerValidateEditorEventArgs e)
	{
		base.RaiseValidatingEditor(e);
		EditorValueMissing = ((ActiveEditor == null && FocusedColumn != null) ? new FieldValueMissing
		{
			FieldName = FocusedColumn.FieldName,
			Value = e.Value
		} : null);
	}

	private void TreeEx_ValidatingEditor(object sender, BaseContainerValidateEditorEventArgs e)
	{
		try
		{
			TreeListColumnEx col = FocusedColumn as TreeListColumnEx;
			if (col == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(col.CurrencyField))
			{
				string[] dataCurrency = Function.ToString(e.Value).GetDataCurrency();
				if (dataCurrency == null || dataCurrency.Length == 0)
				{
					return;
				}
				decimal num = Function.ParseDecimal(dataCurrency[0]);
				string alpha = Function.ToString(dataCurrency[1]);
				string text = Function.ToString(GetFocusedRowCellValue(Columns[col.CurrencyField]));
				if (text == "" || (!string.IsNullOrEmpty(alpha) && text.ToLower() != alpha.ToLower()))
				{
					if (alpha == "" && text != "")
					{
						alpha = text;
					}
					SetRowCellValue(FocusedNode, Columns[col.CurrencyField], (alpha != "") ? alpha.ToUpper() : GlobalData.MainCurrency);
				}
				if (alpha.ToUpper() == GlobalData.MainCurrency.ToUpper() || string.IsNullOrEmpty(alpha))
				{
					SetRowCellValue(FocusedNode, Columns[col.CurrencyRateField], 1);
				}
				else
				{
					DateTime date = DateTime.Today;
					string currencyDateField = col.CurrencyDateField;
					if (!string.IsNullOrEmpty(currencyDateField))
					{
						date = Function.ParseDateTime(GetFocusedRowCellValue(Columns[currencyDateField])) ?? DateTime.Today;
					}
					double exchangeRate = HttpClientHelperFactory.GetHttpClient().ExRateAndIndex.GetExchangeRate(alpha, date);
					if (exchangeRate > 0.0)
					{
						SetRowCellValue(FocusedNode, col.CurrencyRateField, exchangeRate);
					}
					else
					{
						frm_ExchangeRatesAndIndexs frm = new frm_ExchangeRatesAndIndexs();
						frm.Curr = alpha;
						frm._date = date;
						frm.FormClosing += (object s, FormClosingEventArgs e2) =>
						{
							SetRowCellValue(FocusedNode, col.CurrencyRateField, Function.ParseDouble(frm.rate));
						};
						frm.ShowDialog();
					}
				}
				if (!(col.FieldName != col.CurrencyField))
				{
					return;
				}
				CurrencyCodesImpl currencyCodesImpl = GlobalData.CurrencyCodes.FirstOrDefault((CurrencyCodesImpl t) => t.CurrCode.ToUpper().Equals(alpha.ToUpper()));
				if (currencyCodesImpl != null)
				{
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
					e.Value = num;
					SetRowCellValue(FocusedNode, col, num);
				}
			}
			else if (col.DisplayFormatType == Enums.DisplayFormatType.Time && col.ColumnType == typeof(int))
			{
				e.Value = string.Join("", from c in e.Value.ToString().Where(char.IsDigit)
					select (c));
				SetRowCellValue(FocusedNode, col, e.Value);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void TreeListEx_MouseWheel(object sender, MouseEventArgs e)
	{
		if (Control.ModifierKeys.HasFlag(Keys.Shift))
		{
			(sender as TreeList).LeftCoord -= e.Delta;
			((DXMouseEventArgs)e).Handled = true;
		}
	}

	private void TreeEx_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs events)
	{
		try
		{
			if (!(events.Column is TreeListColumnEx treeListColumnEx))
			{
				return;
			}
			Enums.DisplayFormatType displayFormatType = treeListColumnEx.DisplayFormatType;
			if ((uint)(displayFormatType - 2) > 1u)
			{
				return;
			}
			if (Math.Abs(Function.ParseDouble(events.Value)) < 1E-07)
			{
				events.DisplayText = "";
				return;
			}
			string currency = string.Empty;
			switch (treeListColumnEx.Currency)
			{
			case Enums.CurrencyColumn.MainCurrency:
				currency = GlobalData.MainCurrency;
				break;
			case Enums.CurrencyColumn.SystemCurrency:
				currency = GlobalData.SystemCurrency;
				break;
			case Enums.CurrencyColumn.Custom:
				currency = GlobalData.MainCurrency;
				currency = Function.ToString(GetRowCellValue(events.Node, Columns[treeListColumnEx.CurrencyField]));
				break;
			}
			int num = -1;
			if (!treeListColumnEx.FieldName.StartsWith("U_") && (string.IsNullOrEmpty(currency) || currency == ""))
			{
				currency = GlobalData.MainCurrency;
			}
			else if (treeListColumnEx.FieldName.StartsWith("U_"))
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
			if (treeListColumnEx.DisplayFormatType == Enums.DisplayFormatType.Price)
			{
				string text = new string('0', (num == -1) ? GlobalData.GeneralSetting.PriceDec.GetValueOrDefault() : num);
				events.DisplayText = string.Format("{0:#,0." + text + "} {1}", events.Value, currency);
			}
			else if (treeListColumnEx.DisplayFormatType == Enums.DisplayFormatType.Amount)
			{
				string text2 = new string('0', (num == -1) ? GlobalData.GeneralSetting.SumDec.GetValueOrDefault() : num);
				events.DisplayText = string.Format("{0:#,0." + text2 + "} {1}", events.Value, currency);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void TreeEx_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			if (e.MenuType == TreeListMenuType.Column)
			{
				DXMenuItem dXMenuItem = e.Menu.Items.FirstOrDefault((DXMenuItem i) => i.Caption == TreeListLocalizer.Active.GetLocalizedString(TreeListStringId.MenuColumnColumnCustomization));
				if (dXMenuItem != null)
				{
					dXMenuItem.Visible = !DisableFormSetting && GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_FormSetting" && x.authoriza == "F");
				}
			}
			CreateContextMenu(e.HitInfo);
			if (FindForm() is XtraFormEx { FormAuth: not Enums.FormAuthorities.Read, DisableContextAction: false })
			{
				ReloadContextAction();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void ReloadContextAction()
	{
		ContextMenuStrip contextMenuStrip = ContextMenuStrip;
		if (contextMenuStrip == null)
		{
			contextMenuStrip = (ContextMenuStrip = new ContextMenuStrip());
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
			string name = "CTA_" + absEntry;
			ToolStripMenuItem toolStripMenuItem2 = CheckExistsContext(contextMenuStrip, name);
			if (toolStripMenuItem2 != null)
			{
				contextMenuStrip.Items.Remove(toolStripMenuItem2);
			}
			toolStripMenuItem2 = new ToolStripMenuItem();
			toolStripMenuItem2.Name = name;
			toolStripMenuItem2.Text = Function.ToString(item["Caption"]);
			string text = Function.ToString(item["Icon"]);
			if (!string.IsNullOrEmpty(text))
			{
				toolStripMenuItem2.Image = ImageResourceCache.Default.GetImageById(text, ImageSize.Size16x16, ImageType.Office2013);
			}
			int num = Function.ParseInt(item["ActionTypeEx"]);
			if (num != 0)
			{
				toolStripMenuItem2.Click += (object s, EventArgs events) =>
				{
					try
					{
						Activator.CreateInstance(Type.GetType("Apzon.Forms.Commons.ContextActionHandle, Apzon.Forms"), absEntry, FormId, ItemId, DataSourceTable, this);
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
						WinCommons.ErrorLineNumber(ex);
					}
				};
			}
			contextMenuStrip.Items.Add(toolStripMenuItem2);
			if (num != 7000038)
			{
				AddDropDown(absEntry, dataTable, ref toolStripMenuItem2);
			}
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
			string name = "CTA_" + absEntry;
			ToolStripMenuItem toolStripMenuItem = CheckExistsContextDropDown(ctm, name);
			if (toolStripMenuItem != null)
			{
				ctm.DropDownItems.Remove(toolStripMenuItem);
			}
			toolStripMenuItem = new ToolStripMenuItem();
			toolStripMenuItem.Name = name;
			toolStripMenuItem.Text = Function.ToString(dataRow["Caption"]);
			string text = Function.ToString(dataRow["Icon"]);
			if (!string.IsNullOrEmpty(text))
			{
				toolStripMenuItem.Image = ImageResourceCache.Default.GetImageById(text, ImageSize.Size16x16, ImageType.Office2013);
			}
			if (Function.ParseInt(dataRow["ActionTypeEx"]) != 0)
			{
				toolStripMenuItem.Click += (object s, EventArgs events) =>
				{
					try
					{
						Activator.CreateInstance(Type.GetType("Apzon.Forms.Commons.ContextActionHandle, Apzon.Forms"), absEntry, FormId, ItemId, DataSourceTable, this);
					}
					catch (Exception ex)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
						WinCommons.ErrorLineNumber(ex);
					}
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

	private void CreateContextMenu(TreeListHitInfo hitInfo)
	{
		ContextMenuStrip contextMenuStrip = ContextMenuStrip;
		if (contextMenuStrip == null)
		{
			contextMenuStrip = (ContextMenuStrip = new ContextMenuStrip());
		}
		if (!_setFontContextMenu)
		{
			contextMenuStrip.Font = new Font(contextMenuStrip.Font.FontFamily, contextMenuStrip.Font.Size + 0.5f, contextMenuStrip.Font.Unit);
			_setFontContextMenu = true;
		}
		IEnumerable<ToolStripMenuItem> source = contextMenuStrip.Items.OfType<ToolStripMenuItem>();
		ToolStripMenuItem toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name == "ctm_Copy");
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
				Name = "ctm_Copy",
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
			toolStripMenuItem4.Text = "Copy Row(s)";
			toolStripMenuItem4.Click += CtxCopyrow_Click;
			contextMenuStrip.Items.Add(toolStripMenuItem4);
		}
		else
		{
			toolStripMenuItem.Visible = !hitInfo.InColumn;
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
				ToolStripSeparator value = new ToolStripSeparator();
				contextMenuStrip.Items.Add(value);
				ToolStripMenuItem toolStripMenuItem5 = new ToolStripMenuItem();
				toolStripMenuItem5.Text = "Save Form Setting";
				toolStripMenuItem5.Click += CtxSaveFormSetting_Click;
				contextMenuStrip.Items.Add(toolStripMenuItem5);
			}
			toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name.Equals("ctm_ReloadFormSetting") || i.Text.Equals("Reload Form Setting"));
			if (toolStripMenuItem == null)
			{
				ToolStripMenuItem toolStripMenuItem6 = new ToolStripMenuItem();
				toolStripMenuItem6.Name = "ctm_ReloadFormSetting";
				toolStripMenuItem6.Text = "Reload Form Setting";
				toolStripMenuItem6.Click += CtxReloadFormSetting_Click;
				contextMenuStrip.Items.Add(toolStripMenuItem6);
			}
		}
		if (FindForm() is XtraFormEx { FormAuth: not Enums.FormAuthorities.Read } xtraFormEx)
		{
			toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name.ToUpper().Equals("CTM_FORMATCONDITION") || i.Text.ToUpper().Equals("FORMAT CONDITION"));
			if (toolStripMenuItem == null)
			{
				ToolStripSeparator value2 = new ToolStripSeparator();
				contextMenuStrip.Items.Add(value2);
				ToolStripMenuItem toolStripMenuItem7 = new ToolStripMenuItem();
				toolStripMenuItem7.Name = "CTM_FORMATCONDITION";
				toolStripMenuItem7.Text = "Format Condition";
				toolStripMenuItem7.Click += CtxFormatRule_Click;
				contextMenuStrip.Items.Add(toolStripMenuItem7);
			}
			toolStripMenuItem = source.FirstOrDefault((ToolStripMenuItem i) => i.Name.ToUpper().Equals("CTM_ACTION") || i.Text.ToUpper().Equals("CONTEXT ACTION"));
			if (toolStripMenuItem == null && !xtraFormEx.DisableContextAction && GlobalData.ListAccessForm.FirstOrDefault((AuthsImpl r) => r.menucode == "barSub_ContextAction" && r.authoriza != "N") != null)
			{
				ToolStripSeparator value3 = new ToolStripSeparator();
				contextMenuStrip.Items.Add(value3);
				ToolStripMenuItem toolStripMenuItem8 = new ToolStripMenuItem();
				toolStripMenuItem8.Name = "CTM_ACTION";
				toolStripMenuItem8.Text = "Context Action";
				toolStripMenuItem8.Click += CtxAction_Click;
				contextMenuStrip.Items.Add(toolStripMenuItem8);
			}
		}
	}

	private void CtxFormatRule_Click(object sender, EventArgs e)
	{
		try
		{
			frm_FormatRule frm_FormatRule2 = new frm_FormatRule();
			frm_FormatRule2.InitTree(this);
			if (FindForm() is XtraFormEx frmParent)
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

	private void CtxCopytable_Click(object sender, EventArgs e)
	{
		bool multiSelect = OptionsSelection.MultiSelect;
		OptionsSelection.MultiSelect = true;
		SelectAll();
		CopyToClipboard();
		OptionsSelection.MultiSelect = multiSelect;
	}

	private void CtxCopyrow_Click(object sender, EventArgs e)
	{
		CopyToClipboard();
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

	private void CtxAction_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			XtraFormEx xtraFormEx = (XtraFormEx)Activator.CreateInstance(Type.GetType("Apzon.Forms.Commons.frm_ContextAction, Apzon.Forms"), FormId, ItemId, DataSourceTable, this);
			if (FindForm() is XtraFormEx frmParent)
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

	public TreeListColumnEx GetTreeListColumnByID(object columnID)
	{
		if (columnID == null)
		{
			return null;
		}
		if (columnID is TreeListColumnEx)
		{
			return (TreeListColumnEx)columnID;
		}
		if (columnID is int)
		{
			return Columns[(int)columnID] as TreeListColumnEx;
		}
		if (columnID is string)
		{
			return Columns[(string)columnID] as TreeListColumnEx;
		}
		if (columnID is DataColumn)
		{
			return Columns[((DataColumn)columnID).ColumnName] as TreeListColumnEx;
		}
		return null;
	}

	protected override void RaiseEditorKeyDown(KeyEventArgs e)
	{
		if (AllowPasteExcel && e.KeyCode == Keys.V && e.Modifiers == Keys.Control && this.ExcelToDataSource())
		{
			e.Handled = true;
		}
		else
		{
			base.RaiseEditorKeyDown(e);
		}
	}

	protected override TreeListColumnCollection CreateColumns()
	{
		return new TreeListColumnCollectionEx(this);
	}

	public virtual void TreeListEx_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (string.IsNullOrEmpty(FormId) || string.IsNullOrEmpty(GlobalData.SessionId))
		{
			return;
		}
		if (FindForm() is XtraFormEx xtraFormEx)
		{
			if (xtraFormEx.DisableLoadScript)
			{
				return;
			}
			if (SyntaxOnCellValueChanged == null)
			{
				string formulaText = HttpClientHelperFactory.GetHttpClient(xtraFormEx.FormObjectType).Formula.GetFormulaText(FormId, Enums.EventId.OnCellValueChanged, ItemId);
				SyntaxOnCellValueChanged = ((!string.IsNullOrEmpty(formulaText)) ? ControlFunction.GetToken(formulaText) : new List<Token>());
			}
			if (SyntaxOnCellValueChanged.Count != 0 && !CancelNextCellValueChangedScript)
			{
				TreeListColumnEx editColumn = EditColumn;
				int indexEditRow = IndexEditRow;
				EditColumn = e.Column as TreeListColumnEx;
				IndexEditRow = GetDataSourceRowIndex(e.Node.Id);
				CtdMessage ctdMessage = new Apzon.Controls.Compiler.Compiler(Configuration.ApiServer, GlobalData.SessionId, -1, e.Column.FieldName).UserdefinedFomular(xtraFormEx, SyntaxOnCellValueChanged, out var _);
				IndexEditRow = indexEditRow;
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

	public virtual async Task InitFormula(CellValueChangedEventArgs e)
	{
		try
		{
			if (Formulars == null || Formulars.Count <= 0 || TblFormular == null || TblFormular.Count <= 0)
			{
				return;
			}
			string dependTable = TblFormular.FirstOrDefault((KeyValuePair<string, TreeList> t) => t.Value == this).Key;
			if (string.IsNullOrEmpty(dependTable))
			{
				return;
			}
			string colName = e.Column.FieldName;
			List<Formular> list = Formulars.Where((Formular t) => !t.DependField.Equals(t.FieldName) && (from x in t.DependField.Split(',')
				select x.ToUpper().Trim().Split('.')[x.ToUpper().Trim().Split('.')
					.Length - 1]).Contains(colName.ToUpper()) && t.TableName.Equals(dependTable) && !t.FormularType.ToUpper().Equals("CBX")).ToList();
			if (list.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				string text = Function.ToString(list[i].FormularString);
				if (list[i].FormularType.ToUpper().Equals("DB"))
				{
					List<string> paramFromSqlString = Function.GetParamFromSqlString(text);
					if (paramFromSqlString != null && paramFromSqlString.Any())
					{
						foreach (string item in paramFromSqlString)
						{
							if (!string.IsNullOrEmpty(item))
							{
								string[] array = item.Replace("@", "").Replace(" ", "").Split('.');
								string key = list[i].DependTable;
								string columnID = "";
								if (array.Length > 1)
								{
									key = ((!string.IsNullOrEmpty(array[0])) ? array[0] : list[i].DependTable);
									columnID = array[1];
								}
								else if (array.Length != 0)
								{
									columnID = array[0];
								}
								TreeList treeList = TblFormular[key];
								TreeListNode treeListNode = ((!(treeList.Name == Name)) ? treeList.FocusedNode : e.Node);
								string arg = Function.ToString(treeListNode.GetValue(columnID));
								text = text.Replace(item, $"'{arg}'");
							}
						}
					}
					DataTable dataTable = new DataTable();
					using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons"))
					{
						dataTable = await helper.GetAsync("Query", new
						{
							query = text
						});
					}
					if (dataTable != null && dataTable.Rows.Count > 0)
					{
						dynamic val = list[i].FieldType switch
						{
							"STR" => (dynamic)Function.ToString(dataTable.Rows[0][0]), 
							"DEC" => Function.ParseDouble(dataTable.Rows[0][0]), 
							"DOU" => Function.ParseDouble(dataTable.Rows[0][0]), 
							"INT" => Function.ParseInt(dataTable.Rows[0][0]), 
							"DAT" => Function.ParseDateTime(dataTable.Rows[0][0]), 
							"TIM" => Function.ParseDateTime(dataTable.Rows[0][0]), 
							"DTM" => Function.ParseDateTime(dataTable.Rows[0][0]), 
							_ => Function.ToString(dataTable.Rows[0][0]), 
						};
						TreeList treeList2 = TblFormular[list[i].TableName];
						TreeListNode treeListNode2 = ((!(treeList2.Name == Name)) ? treeList2.FocusedNode : e.Node);
						treeListNode2.SetValue(list[i].FieldName, val);
						treeList2.ForceInitialize();
					}
					continue;
				}
				DataTable dataTable2 = (DataTable)DataSource;
				if (dataTable2.Columns.Contains("temp"))
				{
					dataTable2.Columns.Remove("temp");
				}
				if (dataTable2.Rows.Count > 0)
				{
					switch (list[i].FieldType)
					{
					case "STR":
						dataTable2.Columns.Add("temp", typeof(string));
						break;
					case "DEC":
						dataTable2.Columns.Add("temp", typeof(decimal));
						break;
					case "DOU":
						dataTable2.Columns.Add("temp", typeof(double));
						break;
					case "INT":
						dataTable2.Columns.Add("temp", typeof(int));
						break;
					case "DAT":
						dataTable2.Columns.Add("temp", typeof(DateTime));
						break;
					case "TIM":
						dataTable2.Columns.Add("temp", typeof(DateTime));
						break;
					case "DTM":
						dataTable2.Columns.Add("temp", typeof(DateTime));
						break;
					default:
						dataTable2.Columns.Add("temp", typeof(string));
						break;
					}
					dataTable2.Columns["temp"].Expression = text;
					TreeList treeList3 = TblFormular[list[i].TableName];
					Formulars.Where((Formular t) => t.FieldName.Equals(colName) && t.TableName.Equals(dependTable) && t.DependTable.Equals(list[i].TableName) && (from x in t.DependField.Split(',')
						select x.ToUpper().Trim()).Contains(list[i].FieldName.ToUpper())).ToList();
					int id = Function.ParseInt(treeList3.FocusedNode.GetValue(IdColumn));
					if (!dataTable2.Rows[0].IsExistsColumn(IdColumn))
					{
						IdColumn = "U_" + IdColumn;
					}
					if (dataTable2.Rows[0].IsExistsColumn(IdColumn))
					{
						DataRow dataRow = dataTable2.AsEnumerable().FirstOrDefault((DataRow t) => id == Function.ParseInt(t[IdColumn]));
						if (dataRow != null)
						{
							object val2 = dataRow["temp"];
							treeList3.FocusedNode.SetValue(list[i].FieldName, val2);
							treeList3.ForceInitialize();
							if (dataTable2.Columns.Contains("temp"))
							{
								dataTable2.Columns.Remove("temp");
							}
						}
					}
				}
				dataTable2.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	protected override void RaiseBeforeFocusNode(TreeListNode oldNode, TreeListNode node, ref bool canFocus)
	{
		if (IsLoadingData)
		{
			return;
		}
		IsLoadingData = true;
		bool cancelEvent = false;
		XtraFormEx xtraFormEx = null;
		if (!string.IsNullOrEmpty(FormId) && !string.IsNullOrEmpty(GlobalData.SessionId))
		{
			if (FindForm() is XtraFormEx xtraFormEx2)
			{
				if (!xtraFormEx2.DisableLoadScript)
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
			}
			else
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "Parent Form is null");
			}
		}
		canFocus = !cancelEvent;
		base.RaiseBeforeFocusNode(oldNode, node, ref canFocus);
		IsLoadingData = false;
	}

	protected override void OnColumnAdded(TreeListColumn column)
	{
		try
		{
			TreeListColumnEx colEx = column as TreeListColumnEx;
			if (colEx != null)
			{
				DataTable dataSourceTable = DataSourceTable;
				if (colEx.DisplayFormatType == Enums.DisplayFormatType.None && !string.IsNullOrEmpty(column.FieldName) && dataSourceTable != null && dataSourceTable.Columns.Count > 0 && dataSourceTable.Columns.Contains(column.FieldName))
				{
					DataColumn dataColumn = dataSourceTable.Columns[colEx.FieldName];
					if (dataColumn.DataType == typeof(DateTime) || dataColumn.DataType == typeof(DateTime?))
					{
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
							colEx.DisplayFormatType = Enums.DisplayFormatType.Rate;
							if (colEx.ColumnEdit == null)
							{
								colEx.ColumnEdit = new RepositoryItemCalcEditEx();
							}
						}
						else if (new string[4] { "percent", "discount", "disc", "prcnt" }.Any((string t) => colEx.FieldName.ToLower().Contains(t)))
						{
							colEx.DisplayFormatType = Enums.DisplayFormatType.Percent;
							if (colEx.ColumnEdit == null)
							{
								colEx.ColumnEdit = new RepositoryItemCalcEditEx();
							}
						}
						else
						{
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

	public void Export(Enums.ExportExt extention)
	{
		string text = "";
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
		case ".csv":
			ExportToCsv(fileName);
			break;
		default:
			ExportToXls(fileName);
			break;
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

	public async Task ReloadFormSetting()
	{
		try
		{
			if (string.IsNullOrEmpty(FormId) || string.IsNullOrEmpty(ItemId) || FormId.Equals("oUserDefinedReports"))
			{
				return;
			}
			DataTable dataTable;
			if (GlobalData.FormSettings.ContainsKey(FormId) && GlobalData.FormSettings[FormId].Tables.Contains(ItemId))
			{
				dataTable = GlobalData.FormSettings[FormId].Tables[ItemId];
			}
			else
			{
				dataTable = await _httpClient.FormSettingClient.GetFormSetting(FormId, ItemId, GlobalData.UserName, GlobalData.CURRENTLANGUAGE);
				if (dataTable == null || dataTable.Columns.Count == 0)
				{
					for (int num = VisibleColumns.Count - 1; num >= 0; num--)
					{
						VisibleColumns[num].Visible = false;
					}
					HasFormSetting = true;
					return;
				}
				if (!GlobalData.FormSettings.ContainsKey(FormId))
				{
					GlobalData.FormSettings.Add(FormId, new DataSet());
				}
				if (!GlobalData.FormSettings[FormId].Tables.Contains(ItemId))
				{
					dataTable.TableName = ItemId;
					GlobalData.FormSettings[FormId].Tables.Add(dataTable);
				}
			}
			if (dataTable.Rows.Count > 0)
			{
				HasFormSetting = true;
				LstOldUDF = (from r in dataTable.AsEnumerable()
					where Function.ToString(r["ColumnId"]).StartsWith("U_")
					select Function.ToString(r["ColumnId"])).ToList();
				TreeListColumn[] array = Columns.Where((TreeListColumn t) => t.Fixed != FixedStyle.None).ToArray();
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					array[num2].Fixed = FixedStyle.None;
				}
				for (int num3 = 0; num3 < dataTable.Rows.Count; num3++)
				{
					string text = Function.ToString(dataTable.Rows[num3]["ColumnId"]);
					FormSetting setting = new FormSetting
					{
						FormId = Function.ToString(dataTable.Rows[num3]["FormId"]),
						ItemId = Function.ToString(dataTable.Rows[num3]["ItemId"]),
						ColumnId = text,
						ColumnName = Function.ToString(dataTable.Rows[num3]["ColumnName"]),
						UserSign = Function.ToString(dataTable.Rows[num3]["UserSign"]),
						Width = Function.ParseInt(dataTable.Rows[num3]["Width"]),
						Visible = Function.ParseBool(dataTable.Rows[num3]["Visible"]),
						VisIndex = Function.ParseInt(dataTable.Rows[num3]["VisIndex"]),
						IsReadOnly = Function.ParseBool(dataTable.Rows[num3]["IsReadOnly"]),
						Fixed = Function.ToString(dataTable.Rows[num3]["Fixed"]),
						NeverEditable = (LstColumnNotEditable != null && LstColumnNotEditable.Contains(text)),
						CurrField = Function.ToString(dataTable.Rows[num3]["CurrField"])
					};
					setting.Editable = !setting.NeverEditable && Function.ParseBool(dataTable.Rows[num3]["Editable"]);
					if (!Columns.Any((TreeListColumn t) => t.FieldName.Equals(setting.ColumnId)))
					{
						if (!setting.Visible)
						{
							continue;
						}
						if (Columns.Count <= setting.VisIndex || setting.VisIndex < 0)
						{
							TreeListColumnEx treeListColumnEx = new TreeListColumnEx();
							treeListColumnEx.FieldName = setting.ColumnId;
							Columns.Add(treeListColumnEx);
						}
						else
						{
							ColumnsEx.Insert(setting.VisIndex).FieldName = setting.ColumnId;
						}
					}
					if (Columns.FirstOrDefault((TreeListColumn t) => t.FieldName.Equals(setting.ColumnId)) is TreeListColumnEx treeListColumnEx2)
					{
						if (!string.IsNullOrEmpty(setting.ColumnName))
						{
							treeListColumnEx2.Caption = setting.ColumnName;
						}
						else
						{
							treeListColumnEx2.Caption = setting.ColumnId;
						}
						treeListColumnEx2.Width = setting.Width;
						bool flag = setting.Visible && (LstColumNotSetting == null || !LstColumNotSetting.Contains(treeListColumnEx2.FieldName));
						if (flag)
						{
							treeListColumnEx2.VisibleIndex = setting.VisIndex;
							treeListColumnEx2.AbsoluteIndex = setting.VisIndex;
						}
						treeListColumnEx2.Visible = flag;
						treeListColumnEx2.OptionsColumn.AllowEdit = setting.Editable;
						treeListColumnEx2.OptionsColumn.ReadOnly = setting.IsReadOnly;
						treeListColumnEx2.Fixed = ((setting.Fixed == "L") ? FixedStyle.Left : ((setting.Fixed == "R") ? FixedStyle.Right : FixedStyle.None));
						if (!string.IsNullOrWhiteSpace(Function.ToString(setting.CurrField)))
						{
							treeListColumnEx2.Currency = Enums.CurrencyColumn.Custom;
							treeListColumnEx2.CurrencyField = setting.CurrField;
						}
					}
				}
			}
			else
			{
				HasFormSetting = false;
			}
			FixEditorDateTime();
			HideColumnsInColumnChooser();
			SetDistRuleColsCaption();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void FixEditorDateTime()
	{
		TreeListColumn[] array = Columns.Where((TreeListColumn c) => c is TreeListColumnEx { DisplayFormatType: Enums.DisplayFormatType.DateTime } treeListColumnEx && treeListColumnEx.ColumnEdit == null).ToArray();
		for (int num = 0; num < array.Length; num++)
		{
			array[num].ColumnEdit = new RepositoryItemDateEditEx();
		}
	}

	private void HideColumnsInColumnChooser()
	{
		if (LstColumNotSetting == null)
		{
			return;
		}
		foreach (string item in LstColumNotSetting)
		{
			TreeListColumn treeListColumn = Columns[item];
			if (treeListColumn != null)
			{
				treeListColumn.OptionsColumn.ShowInCustomizationForm = false;
			}
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
				TreeListColumn treeListColumn = Columns.FirstOrDefault((TreeListColumn x) => (x.FieldName.ToUpper() == "OcrCode".ToUpper() && Function.ToString(x.Tag).ToUpper() == dimCode.ToUpper()) || x.FieldName.ToUpper() == dimCode.ToUpper());
				if (treeListColumn != null)
				{
					treeListColumn.Caption = ((row["DimDesc"] == null || row["DimDesc"] == DBNull.Value) ? "" : Function.ToString(row["DimDesc"]));
				}
			}
			if (!GlobalData.Dimensions.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["DimCode"]) == 1) && Columns["OcrCode"] != null)
			{
				if (Columns["OcrCode"].Visible)
				{
					Columns["OcrCode"].Visible = false;
				}
				if (LstColumNotSetting == null)
				{
					LstColumNotSetting = new List<string>();
				}
				if (!LstColumNotSetting.Contains("OcrCode"))
				{
					LstColumNotSetting.Add("OcrCode");
				}
			}
			int i;
			for (i = 2; i <= 5; i++)
			{
				if (GlobalData.Dimensions.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["DimCode"]) == i))
				{
					continue;
				}
				string text = "OcrCode" + i;
				if (Columns[text] != null)
				{
					if (Columns[text].Visible)
					{
						Columns[text].Visible = false;
					}
					if (LstColumNotSetting == null)
					{
						LstColumNotSetting = new List<string>();
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
			foreach (TreeListColumnEx col in (from t in Columns
				orderby t.Visible descending, t.VisibleIndex
				select t).Cast<TreeListColumnEx>())
			{
				if (!list.Contains(col.FieldName) && ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Equals(col.FieldName)) == null)
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
					dataRow["AllowChSet"] = !col.PreventChangeSetting;
					dataRow["VisSetting"] = true;
					dataRow["IsSystem"] = "Y";
					dataRow["LngCode"] = "-";
					dataRow["GroupIndex"] = -1;
					dataRow["CurrField"] = ((col.Currency == Enums.CurrencyColumn.Custom) ? col.CurrencyField : string.Empty);
					ret.Rows.Add(dataRow);
				}
			}
			if (DataSource != null)
			{
				DataTable listKeyAlias = _httpClient.Udf.GetListKeyAlias(text);
				if ((text != "@OPR62" && DataSource.GetType() == typeof(DataTable)) || DataSource.GetType().BaseType == typeof(DataTable))
				{
					DataColumn[] array = (from DataColumn t in DataSourceTable.Columns
						where ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Equals(t.ColumnName)) == null
						select t).ToArray();
					foreach (DataColumn column in array)
					{
						if (list.Contains(column.ColumnName) || ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Equals(column.ColumnName)) != null)
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
				else if (text != "@OPR62" && DataSource.GetType().GetGenericArguments().Length != 0)
				{
					Type type = DataSource.GetType().GetGenericArguments()[0];
					if (type != null)
					{
						PropertyInfo[] properties = type.GetProperties();
						if (properties.Length != 0)
						{
							PropertyInfo[] array2 = properties;
							foreach (PropertyInfo propertyInfo in array2)
							{
								if (ret.AsEnumerable().FirstOrDefault((DataRow x) => Function.ToString(x["ColumnId"]).Equals(propertyInfo.Name)) != null || list.Contains(propertyInfo.Name))
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
			frm_FormSetting2.Tree = this;
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

	public int GetFocusedDataSourceRowIndex()
	{
		DataTable dataSourceTable = DataSourceTable;
		if (dataSourceTable.IsNotNull() && FocusedNode != null)
		{
			DataRow focusedDataRow = GetFocusedDataRow();
			return dataSourceTable.Rows.IndexOf(focusedDataRow);
		}
		return -1;
	}

	public int GetDataSourceRowIndex(int nodeId)
	{
		DataTable dataSourceTable = DataSourceTable;
		if (dataSourceTable.IsNotNull() && nodeId > -1)
		{
			DataRow dataRow = GetDataRow(nodeId);
			if (dataRow != null)
			{
				return dataSourceTable.Rows.IndexOf(dataRow);
			}
		}
		return -1;
	}
}
