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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Inventory.Item_Management;

public class frm_AlternativeItems : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private string originItem;

	private IContainer components;

	private GridControlEx grd_AlternativeItems;

	private GridViewEx gv_AlternativeItems;

	private GridColumnEx gcItemNo;

	private GridColumnEx gcRemarks;

	private GridColumnEx gcMatchFactor;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private ButtonEditCFL txt_ItemOrigin;

	private TextEditEx txt_ItemDescription;

	private SimpleButton btnSave;

	private SimpleButton btnCancel;

	private RepositoryItemButtonEditCFL bte_ItemNo;

	private RepositoryItemCalcEdit calc_MatchFactor;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem deleteDataToolStripMenuItem;

	private ToolStripMenuItem deleteToolStripMenuItem;

	private ToolStripMenuItem refreshToolStripMenuItem;

	private GridColumnEx gcOriginItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.View:
			txt_ItemOrigin.ReadOnly = true;
			btnSave.Text = Resources.ButtonOK;
			txt_ItemOrigin.Properties.Buttons[0].Visible = false;
			break;
		case Enums.FormViewModeType.Add:
			txt_ItemOrigin.ReadOnly = false;
			btnSave.Text = Resources.ButtonAdd;
			txt_ItemOrigin.Properties.Buttons[0].Visible = true;
			break;
		case Enums.FormViewModeType.Update:
			txt_ItemOrigin.ReadOnly = true;
			btnSave.Text = Resources.ButtonUpdate;
			txt_ItemOrigin.Properties.Buttons[0].Visible = false;
			break;
		}
	}

	public frm_AlternativeItems()
	{
		InitializeComponent();
		OnLoadAsync += frm_AlternativeItems_Load;
	}

	public async void LoadData(string itemNo)
	{
		try
		{
			DataTable dataTable = await _httpClient.AlternativeItemsClient.GetData(itemNo);
			grd_AlternativeItems.DataSource = dataTable;
			grd_AlternativeItems.RefreshDataSource();
			if (dataTable.Rows.Count > 0)
			{
				FormMode = Enums.FormViewModeType.View;
			}
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void AddNewRow()
	{
		try
		{
			DataTable dataSourceTable = grd_AlternativeItems.DataSourceTable;
			if (dataSourceTable.Columns.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.CannotGetStructureToAddNew, "");
				return;
			}
			DataRow dataRow = dataSourceTable.NewRow();
			dataRow["OrigItem"] = Function.ToString(txt_ItemOrigin.EditValue);
			dataRow["Match"] = 100;
			dataSourceTable.Rows.Add(dataRow);
			dataSourceTable.AcceptChanges();
			grd_AlternativeItems.DataSource = dataSourceTable;
			grd_AlternativeItems.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task LoadDataDoc(Enums.NavigateType type)
	{
		try
		{
			Tuple<string, Enums.NavigateType> tuple = new Tuple<string, Enums.NavigateType>(Function.ToString(txt_ItemOrigin.EditValue), type);
			DataTable dataTable = await _httpClient.AlternativeItemsClient.GetDataDoc(tuple);
			txt_ItemOrigin.EditValue = Function.ToString(dataTable.Rows[0][0]);
			LoadData(Function.ToString(txt_ItemOrigin.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool NeedConfirmSave()
	{
		try
		{
			foreach (DataRow row in grd_AlternativeItems.DataSourceTable.Rows)
			{
				if (Function.ToString(row["AltItem"]).Trim() != "")
				{
					return true;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return true;
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			if (((FormMode == Enums.FormViewModeType.Add && NeedConfirmSave()) || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadDataDoc(Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			if (((FormMode == Enums.FormViewModeType.Add && NeedConfirmSave()) || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadDataDoc(Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			if (((FormMode == Enums.FormViewModeType.Add && NeedConfirmSave()) || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadDataDoc(Enums.NavigateType.Next);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			if (((FormMode == Enums.FormViewModeType.Add && NeedConfirmSave()) || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadDataDoc(Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel) != DialogResult.Cancel)
			{
				ButtonEditCFL buttonEditCFL = txt_ItemOrigin;
				object editValue = (txt_ItemDescription.EditValue = null);
				buttonEditCFL.EditValue = editValue;
				DataTable dataSourceTable = grd_AlternativeItems.DataSourceTable;
				dataSourceTable.Rows.Clear();
				grd_AlternativeItems.DataSource = dataSourceTable;
				grd_AlternativeItems.RefreshDataSource();
				AddNewRow();
				FormMode = Enums.FormViewModeType.Add;
				XtraFormEx.EnableDirectionalControl(Enums.DirectionalControl.NewDoc, isEnable: false);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add && (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.OKCancel) != DialogResult.Cancel))
			{
				LoadData(Function.ToString(txt_ItemOrigin.EditValue));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_AlternativeItems_Load(object sender, EventArgs e)
	{
		try
		{
			RepositoryItemButtonEditCFL properties = txt_ItemOrigin.Properties;
			properties.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					txt_ItemDescription.EditValue = chooseItem[1];
					txt_ItemOrigin.EditValue = chooseItem[0];
					LoadData(Function.ToString(chooseItem[0]));
				}
			}));
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = bte_ItemNo;
			repositoryItemButtonEditCFL.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					if (Function.ToString(chooseItem["ItemCode"]) == Function.ToString(txt_ItemOrigin.EditValue))
					{
						WinCommons.Error(Enums.MessageCode.ItemIsIdenticalToItemOrogin, "");
					}
					else
					{
						DataTable dataSourceTable = grd_AlternativeItems.DataSourceTable;
						int focusedRowHandle = gv_AlternativeItems.FocusedRowHandle;
						for (int i = 0; i < dataSourceTable.Rows.Count - 1; i++)
						{
							if (i != focusedRowHandle && Function.ToString(chooseItem["ItemCode"]) == Function.ToString(dataSourceTable.Rows[i]["AltItem"]))
							{
								WinCommons.Error(Enums.MessageCode.DuplicateItemSubstitute, "");
								return;
							}
						}
						gv_AlternativeItems.SetFocusedRowCellValue(gcItemNo, chooseItem["ItemCode"]);
						grd_AlternativeItems.DataSourceTable.AcceptChanges();
						grd_AlternativeItems.Refresh();
					}
				}
			}));
			gv_AlternativeItems.OptionsView.ColumnAutoWidth = true;
			LoadData("");
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_AlternativeItems_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			DataTable dataSourceTable = grd_AlternativeItems.DataSourceTable;
			if (dataSourceTable.IsNotNull() && e.Column.FieldName == "AltItem" && e.RowHandle == dataSourceTable.Rows.Count - 1)
			{
				AddNewRow();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				WinCommons._OpenProcessing();
				if (string.IsNullOrEmpty(Function.ToString(txt_ItemOrigin.EditValue)))
				{
					WinCommons.Error(Enums.MessageCode.NoOriginItem, "");
					return;
				}
				if (!grd_AlternativeItems.DataSourceTable.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.NoAlternativeItem, "");
					return;
				}
				btnSave.Enabled = false;
				string item = Function.ToString(txt_ItemOrigin.EditValue);
				DataTable dataTable = grd_AlternativeItems.DataSourceTable.Copy();
				for (int num = dataTable.Rows.Count - 1; num >= 0; num--)
				{
					if (Function.ToString(dataTable.Rows[num]["AltItem"]).Trim() == "" && Function.ToString(dataTable.Rows[num]["Remarks"]).Trim() == "")
					{
						dataTable.Rows.RemoveAt(num);
					}
				}
				dataTable.AcceptChanges();
				Tuple<string, DataTable> tuple = new Tuple<string, DataTable>(item, dataTable);
				string text = await _httpClient.AlternativeItemsClient.Save(tuple);
				if (text == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					LoadData(Function.ToString(txt_ItemOrigin.EditValue));
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btnSave.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private async void deleteDataToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.No)
			{
				string itemNo = Function.ToString(txt_ItemOrigin.EditValue);
				string text = await _httpClient.AlternativeItemsClient.DeleteData(itemNo);
				if (text == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					REFRESH();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = grd_AlternativeItems.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				int focusedRowHandle = gv_AlternativeItems.FocusedRowHandle;
				dataSourceTable.Rows.RemoveAt(focusedRowHandle);
				grd_AlternativeItems.DataSource = dataSourceTable;
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void frm_AlternativeItems_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (((FormMode == Enums.FormViewModeType.Add && NeedConfirmSave()) || FormMode == Enums.FormViewModeType.Update) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void refreshToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			LoadData(Function.ToString(txt_ItemOrigin.EditValue));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void txt_ItemOrigin_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = await _httpClient.AlternativeItemsClient.GetItemNameByItemCode(Function.ToString(txt_ItemOrigin.EditValue));
			if (dataTable.Rows.Count > 0)
			{
				string editValue = Function.ToString(dataTable.Rows[0][0]);
				txt_ItemDescription.EditValue = editValue;
			}
			else
			{
				txt_ItemDescription.EditValue = string.Empty;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_AlternativeItems_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			string origItem = Function.ToString(gv_AlternativeItems.GetRowCellValue(gv_AlternativeItems.FocusedRowHandle, gcOriginItem.FieldName));
			if (string.IsNullOrEmpty(origItem))
			{
				txt_ItemOrigin.Text = string.Empty;
				txt_ItemDescription.Text = string.Empty;
				return;
			}
			DataTable dataTable = await _httpClient.AlternativeItemsClient.GetItemNameByItemCode(origItem);
			if (!dataTable.IsNotNull())
			{
				txt_ItemDescription.Text = string.Empty;
				return;
			}
			txt_ItemOrigin.Text = origItem;
			txt_ItemDescription.Text = Function.ToString(dataTable.Rows[0][0]);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Inventory.Item_Management.frm_AlternativeItems));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_AlternativeItems = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.refreshToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_AlternativeItems = new Apzon.Controls.Grid.GridViewEx();
		this.gcItemNo = new Apzon.Controls.Grid.GridColumnEx();
		this.bte_ItemNo = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gcRemarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gcMatchFactor = new Apzon.Controls.Grid.GridColumnEx();
		this.calc_MatchFactor = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gcOriginItem = new Apzon.Controls.Grid.GridColumnEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_ItemOrigin = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txt_ItemDescription = new Apzon.Controls.Editor.TextEditEx();
		this.btnSave = new DevExpress.XtraEditors.SimpleButton();
		this.btnCancel = new DevExpress.XtraEditors.SimpleButton();
		((System.ComponentModel.ISupportInitialize)this.grd_AlternativeItems).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_AlternativeItems).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemNo).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calc_MatchFactor).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemOrigin.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemDescription.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_AlternativeItems.AllowPasteExcel = true;
		this.grd_AlternativeItems.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_AlternativeItems.ClipboardData = null;
		this.grd_AlternativeItems.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_AlternativeItems.Location = new System.Drawing.Point(0, 38);
		this.grd_AlternativeItems.MainView = this.gv_AlternativeItems;
		this.grd_AlternativeItems.Name = "grd_AlternativeItems";
		this.grd_AlternativeItems.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.bte_ItemNo, this.calc_MatchFactor });
		this.grd_AlternativeItems.Size = new System.Drawing.Size(534, 292);
		this.grd_AlternativeItems.TabIndex = 0;
		this.grd_AlternativeItems.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_AlternativeItems });
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.addNewToolStripMenuItem, this.deleteToolStripMenuItem, this.deleteDataToolStripMenuItem, this.refreshToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(134, 92);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
		this.deleteToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteToolStripMenuItem.Text = "Delete Row";
		this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
		this.deleteDataToolStripMenuItem.Name = "deleteDataToolStripMenuItem";
		this.deleteDataToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.deleteDataToolStripMenuItem.Text = "Remove";
		this.deleteDataToolStripMenuItem.Click += new System.EventHandler(this.deleteDataToolStripMenuItem_Click);
		this.refreshToolStripMenuItem.Name = "refreshToolStripMenuItem";
		this.refreshToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
		this.refreshToolStripMenuItem.Text = "Refresh";
		this.refreshToolStripMenuItem.Click += new System.EventHandler(this.refreshToolStripMenuItem_Click);
		this.gv_AlternativeItems.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_AlternativeItems.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_AlternativeItems.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_AlternativeItems.ColumnKey = null;
		this.gv_AlternativeItems.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gcItemNo, this.gcRemarks, this.gcMatchFactor, this.gcOriginItem });
		this.gv_AlternativeItems.DisableFormSetting = false;
		this.gv_AlternativeItems.DisplayException = false;
		this.gv_AlternativeItems.EditColumn = null;
		this.gv_AlternativeItems.FieldCurrency = null;
		this.gv_AlternativeItems.FieldRate = null;
		this.gv_AlternativeItems.FormId = "oAlternativeItems";
		this.gv_AlternativeItems.GridControl = this.grd_AlternativeItems;
		this.gv_AlternativeItems.IgnoreFilter = false;
		this.gv_AlternativeItems.IndexEditRow = -1;
		this.gv_AlternativeItems.IndicatorWidth = 35;
		this.gv_AlternativeItems.IsApzTable = false;
		this.gv_AlternativeItems.IsInsertNewRow = false;
		this.gv_AlternativeItems.IsLoadingData = false;
		this.gv_AlternativeItems.IsMappingContextAction = false;
		this.gv_AlternativeItems.ItemId = "OALI";
		this.gv_AlternativeItems.LstColumnAllowBatch = null;
		this.gv_AlternativeItems.LstColumnNotAllowBatch = null;
		this.gv_AlternativeItems.LstColumnNotEditable = null;
		this.gv_AlternativeItems.LstColumNotSetting = null;
		this.gv_AlternativeItems.Name = "gv_AlternativeItems";
		this.gv_AlternativeItems.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_AlternativeItems.OptionsSelection.MultiSelect = true;
		this.gv_AlternativeItems.OptionsView.BestFitMaxRowCount = 100;
		this.gv_AlternativeItems.OptionsView.ColumnAutoWidth = false;
		this.gv_AlternativeItems.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_AlternativeItems.OptionsView.ShowGroupPanel = false;
		this.gv_AlternativeItems.UseBatchMod = false;
		this.gv_AlternativeItems.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_AlternativeItems_FocusedRowObjectChanged);
		this.gv_AlternativeItems.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_AlternativeItems_CellValueChanged);
		this.gcItemNo.Caption = "Alternative Item No";
		this.gcItemNo.ColumnEdit = this.bte_ItemNo;
		this.gcItemNo.CurrencyDateField = null;
		this.gcItemNo.CurrencyField = null;
		this.gcItemNo.CurrencyFixed = null;
		this.gcItemNo.CurrencyFixedRate = 0.0;
		this.gcItemNo.CurrencyRateField = null;
		this.gcItemNo.FieldName = "AltItem";
		this.gcItemNo.Name = "gcItemNo";
		this.gcItemNo.PrimaryKey = true;
		this.gcItemNo.UnitField = null;
		this.gcItemNo.Visible = true;
		this.gcItemNo.VisibleIndex = 0;
		this.gcItemNo.Width = 117;
		this.bte_ItemNo.AutoHeight = false;
		this.bte_ItemNo.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.bte_ItemNo.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_ItemNo.ContainerControl = this;
		this.bte_ItemNo.DisplayMember = "ItemCode";
		this.bte_ItemNo.DisplayMember2 = "";
		this.bte_ItemNo.LoadDataSourceWhenEnter = false;
		this.bte_ItemNo.Name = "bte_ItemNo";
		this.bte_ItemNo.SeparateCharacterForMulti = ';';
		this.bte_ItemNo.SeparateString = ", ";
		this.bte_ItemNo.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.bte_ItemNo.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oItems;
		this.bte_ItemNo.UsingAutoCompleteBox = false;
		this.bte_ItemNo.UsingDataSource = true;
		this.bte_ItemNo.ValidateWhenNoDataSource = false;
		this.bte_ItemNo.ValueMember = "ItemCode";
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
		this.gcRemarks.VisibleIndex = 1;
		this.gcRemarks.Width = 110;
		this.gcMatchFactor.Caption = "Match Factor";
		this.gcMatchFactor.ColumnEdit = this.calc_MatchFactor;
		this.gcMatchFactor.CurrencyDateField = null;
		this.gcMatchFactor.CurrencyField = null;
		this.gcMatchFactor.CurrencyFixed = null;
		this.gcMatchFactor.CurrencyFixedRate = 0.0;
		this.gcMatchFactor.CurrencyRateField = null;
		this.gcMatchFactor.FieldName = "Match";
		this.gcMatchFactor.Name = "gcMatchFactor";
		this.gcMatchFactor.PrimaryKey = false;
		this.gcMatchFactor.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gcMatchFactor.UnitField = null;
		this.gcMatchFactor.Visible = true;
		this.gcMatchFactor.VisibleIndex = 2;
		this.gcMatchFactor.Width = 136;
		this.calc_MatchFactor.AutoHeight = false;
		this.calc_MatchFactor.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calc_MatchFactor.Name = "calc_MatchFactor";
		this.gcOriginItem.Caption = "gcOriginItem";
		this.gcOriginItem.CurrencyDateField = null;
		this.gcOriginItem.CurrencyField = null;
		this.gcOriginItem.CurrencyFixed = null;
		this.gcOriginItem.CurrencyFixedRate = 0.0;
		this.gcOriginItem.CurrencyRateField = null;
		this.gcOriginItem.FieldName = "OrigItem";
		this.gcOriginItem.Name = "gcOriginItem";
		this.gcOriginItem.PrimaryKey = false;
		this.gcOriginItem.UnitField = null;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Item Description";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(250, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(92, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 1;
		this.labelLine1.Utext = "Item Description";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Item No";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(12, 12);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(92, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "Item No";
		this.txt_ItemOrigin.EditControl = null;
		this.txt_ItemOrigin.FieldName = null;
		this.txt_ItemOrigin.IsSearchControl = false;
		this.txt_ItemOrigin.Location = new System.Drawing.Point(107, 9);
		this.txt_ItemOrigin.Name = "txt_ItemOrigin";
		this.txt_ItemOrigin.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.txt_ItemOrigin.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.txt_ItemOrigin.Properties.ContainerControl = this;
		this.txt_ItemOrigin.Properties.DisplayMember = "ItemCode";
		this.txt_ItemOrigin.Properties.DisplayMember2 = "";
		this.txt_ItemOrigin.Properties.LoadDataSourceWhenEnter = false;
		this.txt_ItemOrigin.Properties.SeparateCharacterForMulti = ';';
		this.txt_ItemOrigin.Properties.SeparateString = ", ";
		this.txt_ItemOrigin.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Items;
		this.txt_ItemOrigin.Properties.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oItems;
		this.txt_ItemOrigin.Properties.UsingAutoCompleteBox = false;
		this.txt_ItemOrigin.Properties.UsingDataSource = true;
		this.txt_ItemOrigin.Properties.ValidateWhenNoDataSource = false;
		this.txt_ItemOrigin.Properties.ValueMember = "ItemCode";
		this.txt_ItemOrigin.SettingInVisible = false;
		this.txt_ItemOrigin.SettingReadonly = false;
		this.txt_ItemOrigin.Size = new System.Drawing.Size(100, 20);
		this.txt_ItemOrigin.TabIndex = 5;
		this.txt_ItemOrigin.EditValueChanged += new System.EventHandler(this.txt_ItemOrigin_EditValueChanged);
		this.txt_ItemDescription.EditControl = null;
		this.txt_ItemDescription.FieldName = null;
		this.txt_ItemDescription.IsSearchControl = false;
		this.txt_ItemDescription.Location = new System.Drawing.Point(345, 9);
		this.txt_ItemDescription.Name = "txt_ItemDescription";
		this.txt_ItemDescription.Properties.ReadOnly = true;
		this.txt_ItemDescription.SettingInVisible = false;
		this.txt_ItemDescription.SettingReadonly = false;
		this.txt_ItemDescription.Size = new System.Drawing.Size(100, 20);
		this.txt_ItemDescription.TabIndex = 6;
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.Location = new System.Drawing.Point(4, 336);
		this.btnSave.Name = "btnSave";
		this.btnSave.Size = new System.Drawing.Size(75, 23);
		this.btnSave.TabIndex = 22;
		this.btnSave.Text = "Save";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.Location = new System.Drawing.Point(82, 336);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(75, 23);
		this.btnCancel.TabIndex = 21;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(535, 363);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.txt_ItemDescription);
		base.Controls.Add(this.txt_ItemOrigin);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.grd_AlternativeItems);
		base.FormId = "oAlternativeItems";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_AlternativeItems";
		this.Text = "Alternative Items";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_AlternativeItems_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.grd_AlternativeItems).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_AlternativeItems).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_ItemNo).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calc_MatchFactor).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemOrigin.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ItemDescription.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
