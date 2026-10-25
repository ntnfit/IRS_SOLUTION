using System;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Grid;
using Apzon.Forms.Commons;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Drawing;
using DevExpress.XtraEditors.Registrator;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraEditors.ViewInfo;

namespace Apzon.Forms.UserControls;

[UserRepositoryItem("RegisterCheckedComboBoxEditDefineNew")]
public class RepositoryItemCheckedComboBoxEditDefineNew : RepositoryItemCheckedComboBoxEdit
{
	private int selectIndex;

	private CheckedComboBoxEditDefineNew editor;

	private bool initDataSource;

	private GridViewEx _view;

	private string gridFieldName = "";

	private int dropDownLimit = 15;

	private frm_DefineNew formProperties = new frm_DefineNew();

	public bool AddFirstBlankRow = true;

	public bool ResizePopup = true;

	public Action RefreshMethod;

	public Func<DataTable> RefreshDataTable;

	public Func<Task> RefreshMethodAsync;

	public Func<Task<DataTable>> RefreshDataTableAsync;

	public Action<DataTable> DefineNewFormUpdated;

	public Action<DataTable> DataSourceRefreshed;

	public Action<Form> BeforeShowForm;

	public Action<Form> AfterShowForm;

	public const string CustomEditName = "CheckedComboBoxEditDefineNew";

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

	public bool DisableDefineNew { get; set; }

	public string CheckTables { get; set; }

	public string CheckFields { get; set; }

	public Type DefineNewForm { get; set; }

	public override string EditorTypeName => "CheckedComboBoxEditDefineNew";

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

	static RepositoryItemCheckedComboBoxEditDefineNew()
	{
		RegisterCheckedComboBoxEditDefineNew();
	}

	public RepositoryItemCheckedComboBoxEditDefineNew()
	{
		NullText = "";
		IncrementalSearch = true;
		Popup += RepositoryItemCheckedComboBoxEditDefineNew_Popup;
		CustomDisplayText += RepositoryItemCheckedComboBoxEditDefineNew_CustomDisplayText;
		EditValueChanging += RepositoryItemCheckedComboBoxEditDefineNew_EditValueChanging;
		ParseEditValue += RepositoryItemCheckedComboBoxEditDefineNew_ParseEditValue;
		KeyPress += RepositoryItemCheckedComboBoxEditDefineNew_KeyPress;
	}

	private void RepositoryItemCheckedComboBoxEditDefineNew_KeyPress(object sender, KeyPressEventArgs e)
	{
		try
		{
			if (sender is CheckedComboBoxEdit { IsPopupOpen: false } checkedComboBoxEdit)
			{
				checkedComboBoxEdit.ShowPopup();
				SendKeys.Send(e.KeyChar.ToString());
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RepositoryItemCheckedComboBoxEditDefineNew_ParseEditValue(object sender, ConvertEditValueEventArgs e)
	{
		try
		{
			if (e.Value is DBNull || Function.ToString(e.Value) == "")
			{
				e.Value = null;
				e.Handled = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RepositoryItemCheckedComboBoxEditDefineNew_CustomDisplayText(object sender, CustomDisplayTextEventArgs e)
	{
		try
		{
			if (e.DisplayText.Contains(", Define New"))
			{
				e.DisplayText = e.DisplayText.Replace(", Define New", "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RepositoryItemCheckedComboBoxEditDefineNew_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			string text = Function.ToString(e.NewValue);
			if (text.Contains("-12347"))
			{
				text = text.Replace("-12347", "");
				e.NewValue = text;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public void RefreshData()
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
		DataSource = frm_DefineNew2.LoadData();
	}

	public async Task RefreshDataAsync()
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

	protected override void OnDataSourceChanged(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(ValueMember) || string.IsNullOrEmpty(DisplayMember))
			{
				return;
			}
			DataTable dataTable = DataSource as DataTable;
			if (!dataTable.HasColumn())
			{
				return;
			}
			string columnType = Function.ToString(dataTable.Columns[ValueMember].DataType);
			if (!DisableDefineNew)
			{
				AddDefineNewRow(dataTable, columnType);
			}
			if (dataTable.Rows.Count < dropDownLimit)
			{
				DropDownRows = dataTable.Rows.Count + 1;
			}
			else
			{
				DropDownRows = dropDownLimit + 1;
			}
			if (DataSourceRefreshed == null)
			{
				return;
			}
			DataTable dataTable2 = (DataSource as DataTable).Copy();
			if (dataTable2.HasColumn() && dataTable2.IsNotNull())
			{
				if (dataTable2.Rows.Count > 1 && !DisableDefineNew)
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
	}

	private void AddDefineNewRow(DataTable dt, string columnType)
	{
		if (!dt.AsEnumerable().Any((DataRow s) => Function.ToString(s[DisplayMember]).Equals("Define New")))
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

	private void RepositoryItemCheckedComboBoxEditDefineNew_Popup(object sender, EventArgs e)
	{
		try
		{
			editor = sender as CheckedComboBoxEditDefineNew;
			GetGridViewFromEditor(editor);
			if (editor != null)
			{
				editor.InternalListBox.ItemChecking -= ccb_DefineNew_ItemChecking;
				editor.InternalListBox.ItemChecking += ccb_DefineNew_ItemChecking;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void ccb_DefineNew_ItemChecking(object sender, ItemCheckingEventArgs e)
	{
		try
		{
			CheckedListBoxControl checkedListBoxControl = sender as CheckedListBoxControl;
			if (editor == null || !(checkedListBoxControl.Items[e.Index].Description == "Define New") || e.OldValue != CheckState.Unchecked || e.NewValue != CheckState.Checked)
			{
				return;
			}
			if (DefineNewForm != null)
			{
				if (typeof(Form).IsAssignableFrom(DefineNewForm))
				{
					Form frm = (Form)Activator.CreateInstance(DefineNewForm);
					frm.StartPosition = FormStartPosition.CenterScreen;
					if (editor != null)
					{
						frm.MdiParent = editor.FindForm().MdiParent;
					}
					frm.FormClosing += Define_FormClosing;
					OnBeforeShowForm(frm);
					editor.BeginInvoke((MethodInvoker)(() =>
					{
						frm.Show();
					}));
					OnAfterShowForm(frm);
				}
			}
			else if (!string.IsNullOrEmpty(FormProperties.TableName))
			{
				PropertyInfo[] properties = typeof(frm_DefineNew).GetProperties(BindingFlags.Instance | BindingFlags.Public);
				frm_DefineNew frm2 = new frm_DefineNew();
				PropertyInfo[] array = properties;
				foreach (PropertyInfo propertyInfo in array)
				{
					if (propertyInfo.CanWrite && propertyInfo.Name != "WindowTarget")
					{
						propertyInfo.SetValue(frm2, propertyInfo.GetValue(FormProperties, null), null);
					}
				}
				if (string.IsNullOrEmpty(FormProperties.CheckTables) && _view != null)
				{
					frm2.CheckTables = _view.ItemId;
				}
				if (string.IsNullOrEmpty(FormProperties.CheckFields) && _view != null)
				{
					frm2.CheckFields = _view.FocusedColumn.FieldName;
				}
				frm2.childSend = ParentGet;
				RefreshDataTable = frm2.LoadData;
				frm2.StartPosition = FormStartPosition.CenterScreen;
				if (editor != null)
				{
					frm2.MdiParent = editor.FindForm().MdiParent;
				}
				OnBeforeShowForm(frm2);
				editor.BeginInvoke((MethodInvoker)(() =>
				{
					frm2.Show();
				}));
				OnAfterShowForm(frm2);
			}
			e.Cancel = true;
			if (editor != null)
			{
				editor.ClosePopup();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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
				RefreshData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetGridViewFromEditor(BaseEdit editor)
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

	public static void RegisterCheckedComboBoxEditDefineNew()
	{
		Image image = null;
		EditorRegistrationInfo.Default.Editors.Add(new EditorClassInfo("CheckedComboBoxEditDefineNew", typeof(CheckedComboBoxEditDefineNew), typeof(RepositoryItemCheckedComboBoxEditDefineNew), typeof(PopupContainerEditViewInfo), new ButtonEditPainter(), designTimeVisible: true, image));
	}

	public override void Assign(RepositoryItem item)
	{
		BeginUpdate();
		try
		{
			base.Assign(item);
			_ = item is RepositoryItemCheckedComboBoxEditDefineNew;
		}
		finally
		{
			EndUpdate();
		}
	}
}
