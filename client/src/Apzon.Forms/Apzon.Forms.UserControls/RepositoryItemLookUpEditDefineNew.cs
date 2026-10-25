using System;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using DevExpress.Utils.Win;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Popup;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace Apzon.Forms.UserControls;

[UserRepositoryItem("RegisterLookUpEditDefineNew")]
public class RepositoryItemLookUpEditDefineNew : RepositoryItemLookUpEdit
{
	private int selectIndex;

	private LookUpEditDefineNew editor;

	private bool initDataSource;

	private bool changeDataSource;

	private bool isEnter;

	private GridViewEx _view;

	private string gridFieldName = "";

	private frm_DefineNew formProperties = new frm_DefineNew();

	private bool addFirstBlankRow = true;

	private bool useDefineNewRow = true;

	private bool resizePopup = true;

	public Action RefreshMethod;

	public Func<DataTable> RefreshDataTable;

	public Func<Task> RefreshMethodAsync;

	public Func<Task<DataTable>> RefreshDataTableAsync;

	public Action<DataTable> DefineNewFormUpdated;

	public Action<DataTable> DataSourceRefreshed;

	public Action<Form> BeforeShowForm;

	public Action<Form> AfterShowForm;

	public const string CustomEditName = "LookUpEditDefineNew";

	[Category("FormProperties")]
	[Description("Properties for the Define New Form.")]
	[Browsable(true)]
	[EditorBrowsable(EditorBrowsableState.Always)]
	public frm_DefineNew FormProperties
	{
		get
		{
			return formProperties;
		}
		set
		{
			formProperties = value;
		}
	}

	[DefaultValue(true)]
	public bool AddFirstBlankRow
	{
		get
		{
			return addFirstBlankRow;
		}
		set
		{
			addFirstBlankRow = value;
		}
	}

	[DefaultValue(true)]
	public bool UseDefineNewRow
	{
		get
		{
			return useDefineNewRow;
		}
		set
		{
			useDefineNewRow = value;
		}
	}

	[DefaultValue(true)]
	public bool ResizePopup
	{
		get
		{
			return resizePopup;
		}
		set
		{
			resizePopup = value;
		}
	}

	public string CheckTables { get; set; }

	public string CheckFields { get; set; }

	public Type DefineNewForm { get; set; }

	public override string EditorTypeName => "LookUpEditDefineNew";

	private void OnBeforeShowForm(Form e)
	{
		if (BeforeShowForm != null)
		{
			BeforeShowForm(e);
		}
	}

	private void OnAfterShowForm(Form e)
	{
		if (AfterShowForm != null)
		{
			AfterShowForm(e);
		}
	}

	public void RefreshDataSource()
	{
		if (string.IsNullOrEmpty(FormProperties.TableName))
		{
			return;
		}
		PropertyInfo[] properties = typeof(frm_DefineNew).GetProperties(BindingFlags.Instance | BindingFlags.Public);
		frm_DefineNew frm_DefineNew2 = new frm_DefineNew();
		PropertyInfo[] array = properties;
		foreach (PropertyInfo propertyInfo in array)
		{
			if (propertyInfo.CanWrite && propertyInfo.Name != "WindowTarget")
			{
				propertyInfo.SetValue(frm_DefineNew2, propertyInfo.GetValue(FormProperties, null), null);
			}
		}
		OnBeforeShowForm(frm_DefineNew2);
		DataSource = frm_DefineNew2.LoadData();
	}

	public async Task RefreshDataSourceAsync()
	{
		if (string.IsNullOrEmpty(FormProperties.TableName))
		{
			return;
		}
		PropertyInfo[] properties = typeof(frm_DefineNew).GetProperties(BindingFlags.Instance | BindingFlags.Public);
		frm_DefineNew frm_DefineNew2 = new frm_DefineNew();
		PropertyInfo[] array = properties;
		foreach (PropertyInfo propertyInfo in array)
		{
			if (propertyInfo.CanWrite && propertyInfo.Name != "WindowTarget")
			{
				propertyInfo.SetValue(frm_DefineNew2, propertyInfo.GetValue(FormProperties, null), null);
			}
		}
		DataSource = await frm_DefineNew2.LoadDataAsync();
	}

	private void GetDataSourceTheFirstTime()
	{
		if (!initDataSource)
		{
			RefreshDataSource();
			if (DataSource != null)
			{
				initDataSource = true;
			}
		}
	}

	static RepositoryItemLookUpEditDefineNew()
	{
		RegisterLookUpEditDefineNew();
	}

	public RepositoryItemLookUpEditDefineNew()
	{
		NullText = "";
		ShowHeader = false;
		ShowFooter = false;
		BestFitMode = BestFitMode.BestFitResizePopup;
		EditValueChanging += luk_EditValueChanging;
		Popup += luk_Popup;
	}

	protected override void OnDataSourceChanged(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(ValueMember) || string.IsNullOrEmpty(DisplayMember) || changeDataSource || isEnter)
			{
				return;
			}
			DataTable dataTable = DataSource as DataTable;
			if (!dataTable.HasColumn() || (string.IsNullOrEmpty(formProperties.TableName) && DefineNewForm == null))
			{
				return;
			}
			changeDataSource = true;
			AddBlankRow(dataTable);
			string text = Function.ToString(dataTable.Columns[ValueMember].DataType);
			AddDefineNewRow(dataTable, text);
			if (dataTable.Rows.Count < 12)
			{
				DropDownRows = dataTable.Rows.Count;
			}
			else
			{
				DropDownRows = 12;
			}
			SetIndexValue(text);
			if (DataSourceRefreshed == null)
			{
				return;
			}
			DataTable dataTable2 = (DataSource as DataTable).Copy();
			if (dataTable2.HasColumn() && dataTable2.IsNotNull())
			{
				if (dataTable2.Rows.Count > 1 && addFirstBlankRow)
				{
					dataTable2.Rows.RemoveAt(0);
				}
				if (dataTable2.Rows.Count > 1 && useDefineNewRow)
				{
					dataTable2.Rows.RemoveAt(dataTable2.Rows.Count - 1);
				}
				dataTable2.AcceptChanges();
			}
			DataSourceRefreshed(dataTable2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			changeDataSource = false;
		}
	}

	private void AddDefineNewRow(DataTable dt, string columnType)
	{
		if (useDefineNewRow && !dt.AsEnumerable().Any((DataRow s) => Function.ToString(s[DisplayMember]).Equals("Define New")))
		{
			DataRow dataRow = dt.NewRow();
			switch (columnType)
			{
			case "System.String":
				dataRow[ValueMember] = "-12347";
				break;
			case "System.Int32":
			case "System.Int16":
				dataRow[ValueMember] = -12347;
				break;
			}
			dataRow[DisplayMember] = "Define New";
			dt.Rows.Add(dataRow);
			dt.AcceptChanges();
		}
	}

	private void AddBlankRow(DataTable dt)
	{
		if (addFirstBlankRow && !dt.AsEnumerable().Any((DataRow s) => string.IsNullOrEmpty(Function.ToString(s[ValueMember]))))
		{
			DataRow row = dt.NewRow();
			dt.Rows.InsertAt(row, 0);
			dt.AcceptChanges();
		}
	}

	private void SetIndexValue(string columnType)
	{
		if (selectIndex == 0)
		{
			return;
		}
		if (columnType == "System.Int32" || columnType == "System.Int16")
		{
			if (OwnerEdit != null)
			{
				OwnerEdit.EditValue = selectIndex;
			}
			if (_view != null)
			{
				if (!string.IsNullOrEmpty(gridFieldName))
				{
					_view.SetFocusedRowCellValue(gridFieldName, selectIndex);
				}
				else
				{
					_view.SetFocusedValue(selectIndex);
				}
			}
		}
		selectIndex = 0;
	}

	private void luk_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (!Function.ToString(e.NewValue).Equals("-12347"))
			{
				return;
			}
			editor = sender as LookUpEditDefineNew;
			GetGridViewFromEditor(editor);
			if (DefineNewForm != null)
			{
				if (typeof(Form).IsAssignableFrom(DefineNewForm))
				{
					Form form = (Form)Activator.CreateInstance(DefineNewForm);
					form.StartPosition = FormStartPosition.CenterScreen;
					form.FormClosing += Define_FormClosing;
					form.ShowDialog(editor.FindForm());
				}
			}
			else if (!string.IsNullOrEmpty(FormProperties.TableName))
			{
				PropertyInfo[] properties = typeof(frm_DefineNew).GetProperties(BindingFlags.Instance | BindingFlags.Public);
				frm_DefineNew frm_DefineNew2 = new frm_DefineNew();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (propertyInfo.CanWrite && propertyInfo.Name != "WindowTarget")
					{
						propertyInfo.SetValue(frm_DefineNew2, propertyInfo.GetValue(FormProperties, null), null);
					}
				}
				if (string.IsNullOrEmpty(FormProperties.CheckTables) && _view != null)
				{
					frm_DefineNew2.CheckTables = _view.ItemId;
				}
				if (string.IsNullOrEmpty(FormProperties.CheckFields) && _view != null)
				{
					frm_DefineNew2.CheckFields = _view.FocusedColumn.FieldName;
				}
				frm_DefineNew2.childSend = ParentGet;
				RefreshDataTable = frm_DefineNew2.LoadData;
				frm_DefineNew2.StartPosition = FormStartPosition.CenterScreen;
				OnBeforeShowForm(frm_DefineNew2);
				frm_DefineNew2.ShowDialog(editor.FindForm());
				OnAfterShowForm(frm_DefineNew2);
			}
			e.Cancel = true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void luk_Popup(object sender, EventArgs e)
	{
		try
		{
			if (!ResizePopup)
			{
				return;
			}
			LookUpEdit lookUpEdit = sender as LookUpEdit;
			PopupLookUpEditForm popupLookUpEditForm = ((IPopupControl)lookUpEdit).PopupWindow as PopupLookUpEditForm;
			lookUpEdit.Properties.BestFit();
			if (lookUpEdit.Properties.Columns.Count > 0)
			{
				float num = 0f;
				if (lookUpEdit.Properties.DataSource is DataTable dataTable && dataTable.Columns.Count > 0)
				{
					for (int i = 0; i < lookUpEdit.Properties.Columns.Count; i++)
					{
						num += (float)(lookUpEdit.Properties.Columns[i].Width + 18);
					}
					int num2 = Function.ParseInt(Math.Round(num, 0).ToString());
					if (num2 > lookUpEdit.Width)
					{
						popupLookUpEditForm.Width = num2;
					}
					else
					{
						popupLookUpEditForm.Width = lookUpEdit.Width;
					}
					if (dataTable.Rows.Count > 11)
					{
						popupLookUpEditForm.Width += 15;
					}
				}
			}
			else
			{
				popupLookUpEditForm.Width = lookUpEdit.Width;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void GetGridViewFromEditor(LookUpEditDefineNew editor)
	{
		try
		{
			if (editor.Parent is GridControlEx gridControlEx)
			{
				_view = (GridViewEx)gridControlEx.MainView;
				gridFieldName = _view.FocusedColumn.FieldName;
			}
			else
			{
				_view = null;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ParentGet(int selectedIndex)
	{
		try
		{
			selectIndex = selectedIndex;
			Callback();
			if (DefineNewFormUpdated != null)
			{
				DefineNewFormUpdated(DataSource as DataTable);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void Define_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			Callback();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void Callback()
	{
		_ = 1;
		try
		{
			if (RefreshMethod != null)
			{
				RefreshMethod();
			}
			else if (RefreshDataTable != null)
			{
				DataSource = RefreshDataTable();
			}
			else if (RefreshMethodAsync != null)
			{
				await RefreshMethodAsync();
			}
			else if (RefreshDataTableAsync != null)
			{
				DataSource = await RefreshDataTableAsync();
			}
			else
			{
				RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public static void RegisterLookUpEditDefineNew()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("LookUpEditDefineNew", typeof(LookUpEditDefineNew), typeof(RepositoryItemLookUpEditDefineNew), typeof(LookUpEditViewInfo), new ButtonEditPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemLookUpEditDefineNew;
		}
		finally
		{
			EndUpdate();
		}
	}
}
