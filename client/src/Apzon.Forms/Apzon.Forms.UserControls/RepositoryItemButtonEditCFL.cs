using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.UserControls;

[UserRepositoryItem("RegisterButtonEditCFL")]
public class RepositoryItemButtonEditCFL : RepositoryItemButtonEdit
{
	private enum AutoCompleteFlag
	{
		None,
		Auto,
		BackToOldText,
		SetEmpty,
		MoreThanOne,
		Invalid
	}

	private bool _isLoading;

	private bool isPressed;

	private bool isLeave;

	private bool showCFL;

	public bool fromCFL;

	private AutoCompleteFlag _autoCompleteFlag;

	private string firstInvalidValue = "";

	private string autoValue = "";

	private DataTable dtSource = new DataTable();

	public Dictionary<string, string> dictionSource = new Dictionary<string, string>();

	private string displayMember = "";

	private string displayMember2 = "";

	private string valueMember = "";

	private List<string> listValue = new List<string>();

	private List<string> fieldNameInDataRow = new List<string>();

	private List<string> fieldNameInGrid = new List<string>();

	private GridView _view;

	private GridControl _gridToRefresh;

	private ApzObjectTypes _fromType;

	private ApzObjectTypes _toType;

	private bool _reserve;

	private bool _baseProduction;

	private string _id;

	private Enums.TypeCFL _type;

	private bool _multiSelect;

	private string _storeProcedure = "";

	protected List<string> _lstNullValue = new List<string>();

	private Enums.ItemObjectType _itemObjectType;

	private List<Common.ParameterValue> _lstpr;

	private string[] _prms = new string[100];

	private string _separateString = ", ";

	private char separateCharMulti = ';';

	private bool spaceAfterSeparate = true;

	public DataRow selectedRowFromCFL;

	public Dictionary<string, string> ColumnToFill = new Dictionary<string, string>();

	public List<DataRow> selectedRowFromCFL_Multi;

	public Action<DataRow> HandleChooseItem;

	public Action<DataRow, ButtonEditCFL> HandleChooseItemWithEditor;

	public Action<List<DataRow>> HandleChooseItemMulti;

	public Action<List<DataRow>, ButtonEditCFL> HandleChooseItemMultiWithEditor;

	public Action<DataRow, GridView> ItemSelected;

	public Func<DataTable> CustomDataSourceForValidating;

	[Obsolete("Use TypeDrillDown instead.")]
	public Action<string> DrillDownMethod;

	public Action<string, GridView> DrillDownMethodWithGridView;

	public Action<frm_CFL> BeforeShowCFL;

	public Action<frm_CFL> AfterShowCFL;

	public Action<DataTable> DataSourceLoaded;

	private ContainerControl _containerControl;

	private bool _autoLoadDataSource = true;

	private bool _usingDataSource = true;

	private bool _usingAutoCompleteBox;

	private bool loadDataSourceWhenEnter;

	private bool _acceptFalseInput;

	private bool customButtonSearchClick;

	private bool tabWhenEmpty = true;

	private bool multiSelectMode;

	public const string CustomEditName = "ButtonEditCFL";

	[DefaultValue(null)]
	public GridControl GridToRefreshDisplayMember
	{
		get
		{
			return _gridToRefresh;
		}
		set
		{
			_gridToRefresh = value;
		}
	}

	[DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
	[Editor("System.Windows.Forms.Design.StringCollectionEditor, System.Design, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
	public List<string> AcceptNullValue
	{
		get
		{
			return _lstNullValue;
		}
		set
		{
			_lstNullValue = value;
		}
	}

	public string SeparateString
	{
		get
		{
			return _separateString;
		}
		set
		{
			_separateString = value;
		}
	}

	public char SeparateCharacterForMulti
	{
		get
		{
			return separateCharMulti;
		}
		set
		{
			separateCharMulti = value;
		}
	}

	public string DisplayMember
	{
		get
		{
			return displayMember;
		}
		set
		{
			displayMember = value;
		}
	}

	public string DisplayMember2
	{
		get
		{
			return displayMember2;
		}
		set
		{
			displayMember2 = value;
		}
	}

	public string ValueMember
	{
		get
		{
			return valueMember;
		}
		set
		{
			valueMember = value;
		}
	}

	public ContainerControl ContainerControl
	{
		get
		{
			return _containerControl;
		}
		set
		{
			_containerControl = value;
		}
	}

	public override ISite Site
	{
		get
		{
			return base.Site;
		}
		set
		{
			base.Site = value;
			if (value != null && (_containerControl == null || !(_containerControl is Form)) && value.GetService(typeof(IDesignerHost)) is IDesignerHost { RootComponent: var rootComponent } && rootComponent is ContainerControl)
			{
				ContainerControl = rootComponent as ContainerControl;
			}
		}
	}

	public bool AutoLoadDataSource
	{
		get
		{
			return _autoLoadDataSource;
		}
		set
		{
			_autoLoadDataSource = value;
		}
	}

	public bool UsingDataSource
	{
		get
		{
			return _usingDataSource;
		}
		set
		{
			_usingDataSource = value;
		}
	}

	public bool UsingAutoCompleteBox
	{
		get
		{
			return _usingAutoCompleteBox;
		}
		set
		{
			_usingAutoCompleteBox = value;
		}
	}

	public bool LoadDataSourceWhenEnter
	{
		get
		{
			return loadDataSourceWhenEnter;
		}
		set
		{
			loadDataSourceWhenEnter = value;
		}
	}

	[DefaultValue(false)]
	public bool AcceptFalseInput
	{
		get
		{
			return _acceptFalseInput;
		}
		set
		{
			_acceptFalseInput = value;
		}
	}

	[DefaultValue(false)]
	public bool CustomButtonSearchClick
	{
		get
		{
			return customButtonSearchClick;
		}
		set
		{
			customButtonSearchClick = value;
		}
	}

	[DefaultValue(true)]
	public bool TabWhenEmpty
	{
		get
		{
			return tabWhenEmpty;
		}
		set
		{
			tabWhenEmpty = value;
		}
	}

	[DefaultValue(false)]
	public bool MultiSelectMode
	{
		get
		{
			return multiSelectMode;
		}
		set
		{
			SetMultiSelectMode(value);
		}
	}

	public bool ValidateWhenNoDataSource { get; set; }

	[DefaultValue(Enums.TypeCFL.None)]
	public Enums.TypeCFL TypeCFL
	{
		get
		{
			return _type;
		}
		set
		{
			_type = value;
		}
	}

	[DefaultValue(ApzObjectTypes.oNone)]
	public ApzObjectTypes TypeDrillDown
	{
		get
		{
			return _fromType;
		}
		set
		{
			_fromType = value;
			_toType = value;
		}
	}

	[DefaultValue("")]
	public string StoredProcedure
	{
		get
		{
			return _storeProcedure;
		}
		set
		{
			_storeProcedure = value;
		}
	}

	public DataTable DataSource
	{
		get
		{
			return dtSource;
		}
		set
		{
			SetDataSource(value);
		}
	}

	public string ListMandatoryField { get; set; }

	public override string EditorTypeName => "ButtonEditCFL";

	public event EventHandler<ButtonEditCFLEventArgs> SelectedFromCFL;

	public event EventHandler<ButtonEditCFLEventArgs> CustomHandleChooseItem;

	public virtual void OnSelectedFromCFL(ButtonEditCFL editor, DataRow chooseItem = null, List<DataRow> chooseItems = null, GridView view = null, frm_CFL formCFL = null)
	{
		ButtonEditCFLEventArgs e = new ButtonEditCFLEventArgs(chooseItem, chooseItems, view, formCFL);
		if (SelectedFromCFL != null)
		{
			SelectedFromCFL(editor, e);
		}
	}

	private void OnCustomHandleChooseItem(ButtonEditCFL editor, DataRow chooseItem = null, List<DataRow> chooseItems = null, GridView view = null, frm_CFL formCFL = null)
	{
		ButtonEditCFLEventArgs e = new ButtonEditCFLEventArgs(chooseItem, chooseItems, view, formCFL);
		if (CustomHandleChooseItem != null)
		{
			CustomHandleChooseItem(editor, e);
		}
	}

	private void OnBeforeShowCFL(frm_CFL e)
	{
		if (BeforeShowCFL != null)
		{
			BeforeShowCFL(e);
		}
	}

	private void OnAfterShowCFL(frm_CFL e)
	{
		if (AfterShowCFL != null)
		{
			AfterShowCFL(e);
		}
	}

	private void OnDataSourceLoaded(DataTable e)
	{
		if (DataSourceLoaded != null)
		{
			DataSourceLoaded(e);
		}
	}

	public RepositoryItemButtonEditCFL()
	{
		SerializableAppearanceObject serializableAppearanceObject = new SerializableAppearanceObject();
		Buttons.Clear();
		Buttons.Add(new EditorButton(ButtonPredefines.Search, "Search"));
		Buttons.Add(new EditorButton(ButtonPredefines.Glyph, "DrillDown", -1, enabled: true, visible: true, isLeft: true, ImageLocation.MiddleCenter, Resources.right, new KeyShortcut(Keys.None), serializableAppearanceObject, "", null, null, enableImageTransparency: true));
		ButtonPressed += ButtonEditCFL_ButtonPressed;
		FormatEditValue += ButtonEditCFL_FormatEditValue;
		ParseEditValue += ButtonEditCFL_ParseEditValue;
		CustomDisplayText += ButtonEditCFL_CustomDisplayText;
		Validating += ButtonEditCFL_Validating;
		KeyDown += RepositoryItemButtonEditCFL_KeyDown;
		Enter += ButtonEditCFL_Enter;
	}

	internal void RepositoryItemButtonEditCFL_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (isPressed || isLeave || fromCFL || !tabWhenEmpty)
			{
				return;
			}
			ButtonEditCFL editor = sender as ButtonEditCFL;
			if (e.Shift && e.KeyCode == Keys.Tab)
			{
				GetGridViewFromEditor(editor);
				e.SuppressKeyPress = true;
				editor.Invoke((Action)(() =>
				{
					ShowCFL(editor, invalidInput: false, tabWhenEmpty: true);
				}));
			}
			else
			{
				if (e.Shift || e.KeyCode != Keys.Tab)
				{
					return;
				}
				string value = Function.ToString(editor.EditValue);
				if (string.IsNullOrEmpty(editor.Text) && string.IsNullOrEmpty(value))
				{
					GetGridViewFromEditor(editor);
					editor.Invoke((Action)(() =>
					{
						ShowCFL(editor, invalidInput: false, tabWhenEmpty: true);
					}));
					if (OwnerEdit != null)
					{
						editor.Parent.SelectNextControl(editor.FindForm().ActiveControl, forward: true, tabStopOnly: true, nested: true, wrap: true);
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

	public RepositoryItemButtonEditCFL(string ValueMember, string DisplayMember)
		: this()
	{
		this.ValueMember = ValueMember;
		this.DisplayMember = DisplayMember;
	}

	private void SetMultiSelectMode(bool value)
	{
		try
		{
			multiSelectMode = value;
			if (multiSelectMode)
			{
				EditorButton editorButton = Buttons.FirstOrDefault((EditorButton s) => s.Kind == ButtonPredefines.Glyph && s.IsLeft);
				if (editorButton != null)
				{
					editorButton.Visible = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void DrillDownBy(ApzObjectTypes fromType, ApzObjectTypes toType, bool reserve = false, bool baseProduction = false, string id = "")
	{
		_fromType = fromType;
		_toType = toType;
		_id = id;
		_reserve = reserve;
		_baseProduction = baseProduction;
	}

	public void DrillDownBy(ApzObjectTypes type, bool reserve = false, bool baseProduction = false, string id = "")
	{
		DrillDownBy(type, type, reserve, baseProduction, id);
	}

	private void SetDataSource(DataTable value)
	{
		try
		{
			dtSource = value;
			dictionSource = new Dictionary<string, string>();
			if (dtSource.IsNotNull() && _usingDataSource)
			{
				dictionSource = SetDictionSource(dtSource, dictionSource);
				OnDataSourceLoaded(dtSource);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private Dictionary<string, string> SetDictionSource(DataTable _dtSource, Dictionary<string, string> _dictionSource)
	{
		try
		{
			if (!string.IsNullOrEmpty(valueMember))
			{
				if (!string.IsNullOrEmpty(displayMember))
				{
					if (!string.IsNullOrEmpty(displayMember2))
					{
						foreach (var item in (from s in _dtSource.AsEnumerable()
							select new
							{
								value = Function.ToString(s[valueMember]),
								display = Function.ToString(s[displayMember]) + _separateString + Function.ToString(s[displayMember2])
							}).Distinct())
						{
							_dictionSource.Add(item.value, item.display);
						}
					}
					else
					{
						foreach (var item2 in (from s in _dtSource.AsEnumerable()
							select new
							{
								value = Function.ToString(s[valueMember]),
								display = Function.ToString(s[displayMember])
							}).Distinct())
						{
							_dictionSource.Add(item2.value, item2.display);
						}
					}
				}
				else
				{
					foreach (DataRow item3 in _dtSource.AsEnumerable())
					{
						string text = Function.ToString(item3[valueMember]);
						if (!_dictionSource.ContainsKey(text))
						{
							_dictionSource.Add(text, text);
						}
					}
				}
			}
			return _dictionSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new Dictionary<string, string>();
		}
	}

	[Obsolete("Use FindCFLWith instead.")]
	public void FindCFLBy(Enums.TypeCFL type, bool multiSelect = false, string storeProcedure = "", Enums.ItemObjectType itemObjectType = Enums.ItemObjectType.NoItemType, List<Common.ParameterValue> lstpr = null, params string[] prms)
	{
		_type = type;
		_multiSelect = multiSelect;
		_storeProcedure = storeProcedure;
		_itemObjectType = itemObjectType;
		_lstpr = lstpr;
		_prms = prms;
		if (_autoLoadDataSource)
		{
			GetDataSourceFromCFL();
		}
	}

	public void FindCFLWith(Enums.TypeCFL type, bool multiSelect = false, string storeProcedure = "", Enums.ItemObjectType itemObjectType = Enums.ItemObjectType.NoItemType, List<Common.ParameterValue> lstpr = null, params string[] prms)
	{
		_type = type;
		_multiSelect = multiSelect;
		_storeProcedure = storeProcedure;
		_itemObjectType = itemObjectType;
		_lstpr = lstpr;
		_prms = prms;
	}

	private async Task GetDataSourceFromCFL()
	{
		try
		{
			if (_type == Enums.TypeCFL.None)
			{
				return;
			}
			using (frm_CFL frm = new frm_CFL(true, _type, _multiSelect, "", _storeProcedure, _itemObjectType, _lstpr, ""))
			{
				if (_type == Enums.TypeCFL.ListItems || _type == Enums.TypeCFL.Items)
				{
					frm.ListMandatoryField = ListMandatoryField + "," + ValueMember + "," + displayMember;
				}
				OnBeforeShowCFL(frm);
				await frm.LoadForm(buttonEditSource: true);
				OnAfterShowCFL(frm);
				DataSource = frm.gv_CFL.GridControl.DataSource as DataTable;
			}
			if (!displayMember.Equals(valueMember) || !string.IsNullOrEmpty(displayMember2))
			{
				if (OwnerEdit != null)
				{
					OwnerEdit.Refresh();
				}
				else if (ContainerControl != null)
				{
					Form control = ContainerControl as Form;
					FindGridInForm(control);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void FindGridInForm(Control control)
	{
		try
		{
			if (control == null || control.Controls == null || control.Controls.Count <= 0)
			{
				return;
			}
			foreach (Control control2 in control.Controls)
			{
				if (control2 is GridControl)
				{
					GridControl grid = control2 as GridControl;
					RefreshGridsHasRepository(grid);
				}
				FindGridInForm(control2);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RefreshGridsHasRepository(GridControl grid)
	{
		try
		{
			if (!(grid.MainView is GridView gridView))
			{
				return;
			}
			foreach (GridColumn column in gridView.Columns)
			{
				if (column.ColumnEdit != null && column.ColumnEdit.Name == Name)
				{
					for (int i = 0; i < gridView.DataRowCount; i++)
					{
						gridView.RefreshRowCell(i, column);
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

	private async Task GetDataSourceFromCFL(GridControl _gridToRefresh)
	{
		try
		{
			await GetDataSourceFromCFL();
			if (_gridToRefresh != null)
			{
				_gridToRefresh.RefreshDataSource();
				_view = _gridToRefresh.MainView as GridViewEx;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetAutoDataSearchMenu(ButtonEditCFL editor)
	{
		try
		{
			if (dictionSource != null && dictionSource.Count > 0 && editor != null)
			{
				TextBoxMaskBox maskBox = editor.MaskBox;
				if (maskBox.AutoCompleteCustomSource.Count <= 0)
				{
					maskBox.AutoCompleteMode = AutoCompleteMode.Suggest;
					maskBox.AcceptsTab = true;
					maskBox.AutoCompleteSource = AutoCompleteSource.CustomSource;
					maskBox.Width = editor.Width + 40;
					AutoCompleteStringCollection autoCompleteStringCollection = new AutoCompleteStringCollection();
					string[] value = dictionSource.Values.ToArray();
					autoCompleteStringCollection.AddRange(value);
					maskBox.AutoCompleteCustomSource = autoCompleteStringCollection;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void RefreshDataSource()
	{
		try
		{
			if (_gridToRefresh != null)
			{
				GetDataSourceFromCFL(_gridToRefresh);
			}
			else
			{
				GetDataSourceFromCFL();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task RefreshDataSourceAsync()
	{
		_ = 1;
		try
		{
			if (_gridToRefresh != null)
			{
				await GetDataSourceFromCFL(_gridToRefresh);
			}
			else
			{
				await GetDataSourceFromCFL();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void FieldNameInDataRow(params string[] fieldList)
	{
		try
		{
			for (int i = 0; i < fieldList.Length; i++)
			{
				fieldNameInDataRow.Add(fieldList[i]);
			}
			SetColumnToFill();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void FieldNameInGrid(params string[] fieldList)
	{
		try
		{
			for (int i = 0; i < fieldList.Length; i++)
			{
				fieldNameInGrid.Add(fieldList[i]);
			}
			SetColumnToFill();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetColumnToFill()
	{
		try
		{
			if (fieldNameInDataRow.Count <= 0)
			{
				return;
			}
			ColumnToFill.Clear();
			for (int i = 0; i < fieldNameInDataRow.Count; i++)
			{
				if (fieldNameInGrid.Count > 0 && fieldNameInGrid.Count == fieldNameInDataRow.Count)
				{
					ColumnToFill.Add(fieldNameInDataRow[i], fieldNameInGrid[i]);
				}
				else
				{
					ColumnToFill.Add(fieldNameInDataRow[i], fieldNameInDataRow[i]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void FillColumns(frm_CFL frm)
	{
		if (_view == null)
		{
			return;
		}
		foreach (KeyValuePair<string, string> item in ColumnToFill)
		{
			string key = item.Key;
			string value = item.Value;
			object value2 = frm.ChooseItem[key];
			_view.SetFocusedRowCellValue(value, value2);
		}
	}

	protected virtual void DefaultHandleItemFromCFL(ButtonEditCFL editor, frm_CFL frm)
	{
		if (ColumnToFill.Count > 0)
		{
			FillColumns(frm);
			return;
		}
		object obj = frm.ChooseItem[valueMember];
		editor.EditValue = Function.ToString(obj);
	}

	static RepositoryItemButtonEditCFL()
	{
		RegisterButtonEditCFL();
	}

	private void ButtonEditCFL_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			isPressed = true;
			ButtonEditCFL editor = sender as ButtonEditCFL;
			GetGridViewFromEditor(editor);
			if (editor.IsModified && !editor.DoValidate())
			{
				return;
			}
			if (e.Button.Kind == ButtonPredefines.Search)
			{
				if (!customButtonSearchClick)
				{
					editor.BeginInvoke((Action)(() =>
					{
						ShowCFL(editor);
					}));
				}
				return;
			}
			string valueInText = Function.ToString(editor.EditValue);
			if (valueInText == "0" || valueInText == "" || e.Button.Kind != ButtonPredefines.Glyph || multiSelectMode)
			{
				return;
			}
			if (DrillDownMethodWithGridView != null && _view != null)
			{
				DrillDownMethodWithGridView(valueInText, _view);
			}
			else if (DrillDownMethod != null)
			{
				DrillDownMethod(valueInText);
			}
			else
			{
				if (_fromType == ApzObjectTypes.oNone)
				{
					return;
				}
				Control control = sender as Control;
				Form form = control.FindForm();
				XtraFormEx form2 = form as XtraFormEx;
				if (form2 != null)
				{
					control.BeginInvoke((MethodInvoker)(() =>
					{
						form2.DrillDown(_fromType, _toType, valueInText, _reserve, _baseProduction, _id);
					}));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			isPressed = false;
		}
	}

	private void ShowCFL(ButtonEditCFL editor, bool invalidInput = false, bool tabWhenEmpty = false)
	{
		if (showCFL)
		{
			return;
		}
		try
		{
			if (editor == null || _type == Enums.TypeCFL.None)
			{
				return;
			}
			showCFL = true;
			if (!multiSelectMode)
			{
				string text = Function.ToString(editor.Text);
				frm_CFL frm;
				if (dictionSource != null && dictionSource.Count > 0)
				{
					if (invalidInput)
					{
						editor.EditValue = null;
					}
					else
					{
						text = "";
					}
					frm = new frm_CFL(_type, _multiSelect, text, _storeProcedure, _itemObjectType, _lstpr, "", "");
				}
				else
				{
					if (!editor.IsModified)
					{
						text = "";
					}
					frm = new frm_CFL(_type, _multiSelect, text, _storeProcedure, _itemObjectType, _lstpr, "", "");
				}
				OnBeforeShowCFL(frm);
				frm_CFL frm_CFL2 = frm;
				frm_CFL2.AfterLoadForm = (Action)Delegate.Combine(frm_CFL2.AfterLoadForm, (Action)(() =>
				{
					OnAfterShowCFL(frm);
				}));
				if (_type == Enums.TypeCFL.ListItems || _type == Enums.TypeCFL.Items)
				{
					frm.ListMandatoryField = ListMandatoryField + "," + ValueMember + "," + displayMember;
				}
				frm.ShowDialog(editor.FindForm());
				DataSource = frm.grd_CFL.DatasourceTable();
				fromCFL = true;
				if (frm.ChooseItem != null)
				{
					selectedRowFromCFL = frm.ChooseItem;
					OnCustomHandleChooseItem(editor, frm.ChooseItem, null, _view, frm);
					if (HandleChooseItem != null)
					{
						HandleChooseItem(selectedRowFromCFL);
					}
					if (HandleChooseItemWithEditor != null)
					{
						HandleChooseItemWithEditor(selectedRowFromCFL, editor);
					}
					if (CustomHandleChooseItem == null && HandleChooseItem == null)
					{
						DefaultHandleItemFromCFL(editor, frm);
					}
					if (ItemSelected != null)
					{
						ItemSelected(selectedRowFromCFL, _view);
					}
					OnSelectedFromCFL(editor, frm.ChooseItem, null, formCFL: frm, view: _view);
					editor.IsModified = true;
					editor.DoValidate();
					if (OwnerEdit != null)
					{
						editor.IsModified = false;
					}
					return;
				}
				selectedRowFromCFL = null;
				selectedRowFromCFL_Multi = frm.ChooseItems;
				if (frm.ChooseItems != null && frm.ChooseItems.Count > 0)
				{
					OnCustomHandleChooseItem(editor, null, frm.ChooseItems, _view, frm);
					if (HandleChooseItemMulti != null)
					{
						HandleChooseItemMulti(selectedRowFromCFL_Multi);
					}
					if (HandleChooseItemMulti == null && HandleChooseItemMultiWithEditor != null)
					{
						HandleChooseItemMultiWithEditor(selectedRowFromCFL_Multi, editor);
					}
					if (ItemSelected != null)
					{
						ItemSelected(selectedRowFromCFL, _view);
					}
					OnSelectedFromCFL(editor, null, frm.ChooseItems, formCFL: frm, view: _view);
					editor.IsModified = true;
					editor.DoValidate();
					if (OwnerEdit != null)
					{
						editor.IsModified = false;
					}
					return;
				}
				string.IsNullOrEmpty(text);
			}
			if (!multiSelectMode)
			{
				return;
			}
			string txtSeach = "";
			if (invalidInput)
			{
				txtSeach = firstInvalidValue;
				firstInvalidValue = "";
			}
			string editorValue = Function.ToString(editor.EditValue);
			frm_CFL frm2 = new frm_CFL(_type, true, txtSeach, _storeProcedure, _itemObjectType, _lstpr, "", "");
			frm2.ButtonEditMultiSelect = true;
			OnBeforeShowCFL(frm2);
			frm_CFL frm_CFL3 = frm2;
			frm_CFL3.AfterLoadForm = (Action)Delegate.Combine(frm_CFL3.AfterLoadForm, (Action)(() =>
			{
				MultiSelectFormShown(frm2, editorValue);
				OnAfterShowCFL(frm2);
			}));
			frm2.ListMandatoryField = ListMandatoryField;
			frm2.ShowDialog(editor.FindForm());
			DataSource = frm2.grd_CFL.DatasourceTable();
			fromCFL = true;
			selectedRowFromCFL_Multi = frm2.ChooseItems;
			if (frm2.ChooseItems.Count > 0)
			{
				listValue.Clear();
				listValue = frm2.ChooseItems.Select((DataRow s) => Function.ToString(s[valueMember])).ToList();
				string editValue = string.Join(Function.ToString(separateCharMulti), listValue.ToArray());
				editor.EditValue = editValue;
				if (ItemSelected != null)
				{
					ItemSelected(selectedRowFromCFL, _view);
				}
				OnSelectedFromCFL(editor, null, frm2.ChooseItems, formCFL: frm2, view: _view);
				editor.IsModified = true;
				editor.DoValidate();
				if (OwnerEdit != null)
				{
					editor.IsModified = false;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			fromCFL = false;
			showCFL = false;
		}
	}

	private void MultiSelectFormShown(frm_CFL frm, string editorValue)
	{
		frm.gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
		string[] array = editorValue.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
		GridColumn column = new GridColumn();
		foreach (GridColumn column2 in frm.gv_CFL.Columns)
		{
			if (column2.FieldName.ToLower() == valueMember.ToLower())
			{
				column = column2;
				break;
			}
		}
		frm.gv_CFL.ClearSelection();
		frm.gv_CFL.OptionsBehavior.ImmediateUpdateRowPosition = false;
		string[] array2 = array;
		foreach (string text in array2)
		{
			int rowHandle = frm.gv_CFL.LocateByDisplayText(0, column, text.Trim());
			frm.gv_CFL.SelectRow(rowHandle);
		}
	}

	private void GetGridViewFromEditor(ButtonEditCFL editor)
	{
		try
		{
			if (_view == null && editor != null)
			{
				if (editor.Parent is GridControl gridControl)
				{
					_view = (GridView)gridControl.MainView;
				}
				else
				{
					_view = null;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ButtonEditCFL_FormatEditValue(object sender, ConvertEditValueEventArgs e)
	{
		try
		{
			string text = Function.ToString(e.Value);
			if (_lstNullValue.Any() && _lstNullValue.Contains(text))
			{
				e.Value = "";
				e.Handled = true;
			}
			else
			{
				if (dictionSource == null || dictionSource.Count <= 0 || !(sender is ButtonEditCFL buttonEditCFL))
				{
					return;
				}
				string key = Function.ToString(buttonEditCFL.EditValue);
				if (!multiSelectMode)
				{
					if (string.IsNullOrEmpty(displayMember) || (displayMember.Equals(valueMember) && string.IsNullOrEmpty(displayMember2)) || !dictionSource.ContainsKey(key) || text == dictionSource[key])
					{
						return;
					}
					if (dictionSource.ContainsKey(text))
					{
						string value = dictionSource[text];
						e.Value = value;
					}
					e.Handled = true;
				}
				if (!multiSelectMode)
				{
					return;
				}
				string[] array = text.Split(new char[1] { separateCharMulti }, StringSplitOptions.RemoveEmptyEntries);
				List<string> list = new List<string>();
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i].Trim();
					if (dictionSource.ContainsValue(text2))
					{
						list.Add(text2);
					}
					else if (dictionSource.ContainsKey(text2))
					{
						string item = dictionSource[text2];
						list.Add(item);
					}
				}
				string value2 = string.Join(Function.ToString(separateCharMulti) + (spaceAfterSeparate ? " " : ""), list);
				e.Value = value2;
				e.Handled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ButtonEditCFL_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		try
		{
			string text = Function.ToString(e.Value);
			if (_lstNullValue.Any() && _lstNullValue.Contains(text))
			{
				e.DisplayText = "";
			}
			else
			{
				if (dictionSource == null || dictionSource.Count <= 0)
				{
					return;
				}
				if (!multiSelectMode)
				{
					if (string.IsNullOrEmpty(displayMember) || (displayMember.Equals(valueMember) && string.IsNullOrEmpty(displayMember2)))
					{
						return;
					}
					if (dictionSource.ContainsKey(text))
					{
						string displayText = dictionSource[text];
						e.DisplayText = displayText;
					}
				}
				if (!multiSelectMode)
				{
					return;
				}
				string[] array = text.Split(new char[1] { separateCharMulti }, StringSplitOptions.RemoveEmptyEntries);
				List<string> list = new List<string>();
				string[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					string text2 = array2[i].Trim();
					if (dictionSource.ContainsValue(text2))
					{
						list.Add(text2);
					}
					else if (dictionSource.ContainsKey(text2))
					{
						string item = dictionSource[text2];
						list.Add(item);
					}
				}
				string displayText2 = string.Join(Function.ToString(separateCharMulti) + (spaceAfterSeparate ? " " : ""), list);
				e.DisplayText = displayText2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ButtonEditCFL_ParseEditValue(object sender, ConvertEditValueEventArgs e)
	{
		try
		{
			if (dictionSource != null && dictionSource.Count > 0 && sender is ButtonEditCFL buttonEditCFL)
			{
				Function.ToString(buttonEditCFL.EditValue);
				_ = multiSelectMode;
				if (multiSelectMode)
				{
					e.Value = RemoveLastSeparator(Function.ToString(e.Value));
					e.Handled = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string RemoveLastSeparator(string value)
	{
		while (value.Length > 0 && value[value.Length - 1].Equals(separateCharMulti))
		{
			value = value.Remove(value.Length - 1);
		}
		return value;
	}

	private object SetAutoCompleteValue(object checkingValue, ButtonEditCFL editor)
	{
		try
		{
			DataTable source = dtSource.Copy();
			Dictionary<string, string> dictionary = new Dictionary<string, string>(dictionSource);
			if (CustomDataSourceForValidating != null)
			{
				source = CustomDataSourceForValidating();
				dictionary = new Dictionary<string, string>();
				dictionary = SetDictionSource(source, dictionary);
			}
			if ((dictionary == null || dictionary.Count <= 0) && !ValidateWhenNoDataSource)
			{
				return checkingValue;
			}
			if (dictionary.ContainsKey(Function.ToString(checkingValue)))
			{
				if (editor.IsModified)
				{
					_autoCompleteFlag = AutoCompleteFlag.Auto;
				}
				return checkingValue;
			}
			string text = Function.ToString(editor.EditValue);
			string _displayMember = ((!string.IsNullOrEmpty(displayMember)) ? displayMember : valueMember);
			DataRow[] array = (from s in source.AsEnumerable()
				where Function.ToString(s[_displayMember]).ToLower().Contains(Function.ToString(checkingValue).ToLower()) && Function.ToString(checkingValue) != ""
				select s).ToArray();
			switch (array.Length)
			{
			case 0:
				if (text != "0" && text != "")
				{
					if (Function.ToString(checkingValue) == "")
					{
						_autoCompleteFlag = AutoCompleteFlag.SetEmpty;
						return null;
					}
					if (!_acceptFalseInput)
					{
						KeyValuePair<string, string> keyValuePair = dictionary.FirstOrDefault((KeyValuePair<string, string> s) => s.Value.ToLower().Contains(Function.ToString(checkingValue).ToLower()));
						if (!string.IsNullOrEmpty(keyValuePair.Key))
						{
							_autoCompleteFlag = AutoCompleteFlag.BackToOldText;
							return keyValuePair.Key;
						}
					}
				}
				else if (Function.ToString(checkingValue) == "")
				{
					_autoCompleteFlag = AutoCompleteFlag.SetEmpty;
					return null;
				}
				if (!dictionary.ContainsKey(Function.ToString(checkingValue)))
				{
					_autoCompleteFlag = AutoCompleteFlag.Invalid;
				}
				break;
			case 1:
				_autoCompleteFlag = AutoCompleteFlag.Auto;
				autoValue = Function.ToString(array[0][valueMember]);
				return autoValue;
			default:
				_autoCompleteFlag = AutoCompleteFlag.MoreThanOne;
				break;
			}
			return checkingValue;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		return checkingValue;
	}

	private object SetAutoCompleteValueMulti(object checkingValue, ButtonEditCFL editor, ref bool invalidInput)
	{
		try
		{
			if (dictionSource == null || dictionSource.Count <= 0)
			{
				return checkingValue;
			}
			string[] array = Function.ToString(checkingValue).Split(new char[1] { separateCharMulti }, StringSplitOptions.RemoveEmptyEntries);
			List<string> list = new List<string>();
			string[] array2 = array;
			foreach (string text in array2)
			{
				string splitValue = text.Trim();
				if (dictionSource.ContainsKey(splitValue))
				{
					list.Add(splitValue);
					continue;
				}
				if (!string.IsNullOrEmpty(displayMember) && dictionSource.ContainsValue(splitValue))
				{
					string key = dictionSource.FirstOrDefault((KeyValuePair<string, string> s) => s.Value == splitValue).Key;
					if (key != null)
					{
						list.Add(key);
					}
					continue;
				}
				if (string.IsNullOrEmpty(displayMember))
				{
					List<KeyValuePair<string, string>> list2 = dictionSource.Where((KeyValuePair<string, string> s) => s.Key.ToLower().Contains(splitValue.ToLower())).ToList();
					if (list2.Count == 1)
					{
						list.Add(list2[0].Key);
						continue;
					}
				}
				if (!string.IsNullOrEmpty(displayMember))
				{
					List<KeyValuePair<string, string>> list3 = dictionSource.Where((KeyValuePair<string, string> s) => s.Value.ToLower().Contains(splitValue.ToLower())).ToList();
					if (list3.Count == 1)
					{
						list.Add(list3[0].Key);
						continue;
					}
				}
				if (firstInvalidValue == "")
				{
					firstInvalidValue = splitValue;
					invalidInput = true;
				}
			}
			return string.Join(Function.ToString(separateCharMulti), list.Distinct());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return checkingValue;
		}
	}

	internal void ButtonEditCFL_Validating(object sender, CancelEventArgs e)
	{
		try
		{
			if (((dictionSource == null || dictionSource.Count <= 0) && !ValidateWhenNoDataSource) || _isLoading || fromCFL || isLeave)
			{
				return;
			}
			isLeave = true;
			ButtonEditCFL editor = sender as ButtonEditCFL;
			GetGridViewFromEditor(editor);
			bool invalidInput = false;
			CheckBeforeLeave(editor, ref invalidInput);
			if (!multiSelectMode & invalidInput)
			{
				editor.Invoke((Action)(() =>
				{
					ShowCFL(editor, invalidInput: true);
				}));
			}
			if (multiSelectMode & invalidInput)
			{
				editor.BeginInvoke((Action)(() =>
				{
					ShowCFL(editor, invalidInput: true);
				}));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			_autoCompleteFlag = AutoCompleteFlag.None;
			isLeave = false;
		}
	}

	private void CheckBeforeLeave(ButtonEditCFL editor, ref bool invalidInput)
	{
		try
		{
			if (editor == null || !editor.IsModified)
			{
				return;
			}
			string checkingValue = Function.ToString(editor.EditValue);
			if (!multiSelectMode)
			{
				object obj = (editor.EditValue = SetAutoCompleteValue(checkingValue, editor));
				if (_autoCompleteFlag == AutoCompleteFlag.Auto)
				{
					string filterExpression = $"{valueMember} = '{Function.ToString(obj)}'";
					DataRow[] array = dtSource.Select(filterExpression);
					if (array.Length == 0)
					{
						return;
					}
					if (HandleChooseItem != null)
					{
						HandleChooseItem(array[0]);
					}
					OnCustomHandleChooseItem(editor, array[0], null, _view);
					if (HandleChooseItem == null && CustomHandleChooseItem == null && ColumnToFill.Count > 0)
					{
						foreach (KeyValuePair<string, string> item in ColumnToFill)
						{
							string key = item.Key;
							string value = item.Value;
							object value2 = array[0][key];
							if (_view != null)
							{
								_view.SetFocusedRowCellValue(value, value2);
							}
						}
					}
					if (ItemSelected != null)
					{
						ItemSelected(array[0], _view);
					}
					return;
				}
				if ((_autoCompleteFlag == AutoCompleteFlag.Invalid || _autoCompleteFlag == AutoCompleteFlag.MoreThanOne) && Function.ToString(obj) != "")
				{
					if (_acceptFalseInput)
					{
						return;
					}
					if (!_acceptFalseInput)
					{
						invalidInput = true;
						return;
					}
				}
			}
			if (multiSelectMode)
			{
				object editValue = SetAutoCompleteValueMulti(editor.EditValue, editor, ref invalidInput);
				editor.EditValue = editValue;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	internal void ButtonEditCFL_Enter(object sender, EventArgs e)
	{
		try
		{
			ButtonEditCFL buttonEditCFL = sender as ButtonEditCFL;
			if (loadDataSourceWhenEnter && (dictionSource == null || dictionSource.Count <= 0))
			{
				RefreshDataSource();
			}
			if (dictionSource != null && dictionSource.Count > 0 && _usingAutoCompleteBox && buttonEditCFL != null)
			{
				SetAutoDataSearchMenu(buttonEditCFL);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static void RegisterButtonEditCFL()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("ButtonEditCFL", typeof(ButtonEditCFL), typeof(RepositoryItemButtonEditCFL), typeof(ButtonEditViewInfo), new ButtonEditPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemButtonEditCFL;
		}
		finally
		{
			EndUpdate();
		}
	}

	private void InitializeComponent()
	{
		((ISupportInitialize)this).BeginInit();
		((ISupportInitialize)this).EndInit();
	}
}
