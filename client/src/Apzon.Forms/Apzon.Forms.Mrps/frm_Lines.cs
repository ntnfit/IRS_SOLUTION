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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Forms.Mrps;

public class frm_Lines : XtraFormEx
{
	private readonly string _msgUnsaved = "Unsaved data will be lost.Do you want to continue without saving?";

	private bool _isFNodeChanging;

	private bool _treeListFocused;

	private bool _isAdd;

	private string _formController = "Lines";

	private int _oldFocusedID = -1;

	private IContainer components;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Save;

	private LabelLine lbBranch;

	private LookUpEdit look_Branch;

	private ContextMenuStrip ctx_Lines;

	private ContextMenuStrip ctx_Items;

	private ContextMenuStrip ctx_Users;

	private ToolStripMenuItem addRowToolStripMenuItem;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private ToolStripMenuItem addRowToolStripMenuItem1;

	private ToolStripMenuItem removeRowToolStripMenuItem1;

	private ToolStripMenuItem addRowToolStripMenuItem2;

	private ToolStripMenuItem removeRowToolStripMenuItem2;

	private SplitContainer splitContainer1;

	private TreeEx treeList_Lines;

	private TreeListColumnEx treeListColumn1;

	private RepositoryItemLookUpEdit rep_Branch;

	private TreeListColumnEx treeListColumn2;

	private TreeListColumnEx treeListColumn3;

	private TreeListColumnEx treeListColumn4;

	private RepositoryItemLookUpEdit rep_LineType;

	private TreeListColumnEx treeListColumn6;

	private TreeListColumnEx treeListColumn7;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grd_Items;

	private GridViewEx gv_Items;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private RepositoryItemButtonEdit rep_ItemCode;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private XtraTabPageEx xtraTabPage3;

	private GridControlEx grd_Users;

	private GridViewEx gv_Users;

	private GridColumnEx gridColumn7;

	private GridColumnEx gridColumn8;

	private RepositoryItemButtonEdit rep_UserCode;

	private ToolStripMenuItem addChildRowToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_Save.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public async Task REFRESH()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Add)
			{
				_isAdd = false;
				bool flag = true;
				if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					flag = false;
				}
				if (flag)
				{
					addRowToolStripMenuItem_Click(null, null);
				}
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
			bool flag = true;
			if ((FormMode == Enums.FormViewModeType.Update || (FormMode == Enums.FormViewModeType.Add && _isAdd)) && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (flag)
			{
				_isAdd = false;
				LoadLines();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public frm_Lines()
	{
		InitializeComponent();
		OnLoadAsync += frm_Lines_Load;
	}

	private async Task frm_Lines_Load(object sender, EventArgs e)
	{
		try
		{
			LoadComboBox();
			if (!GlobalData.IsMultiBranch)
			{
				LoadLines();
			}
			SetFormatControlValue();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void look_Branch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(look_Branch.EditValue) != 0)
			{
				LoadLines();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void treeList_Lines_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if (Function.ParseInt(treeList_Lines.FocusedNode.GetValue("LineID")) < 0 || treeList_Lines.FocusedColumn.FieldName == "Branch")
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

	private void treeList_Lines_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			if (_isFNodeChanging || Function.ParseInt(treeList_Lines.FocusedNode.GetValue("LineID")) == _oldFocusedID)
			{
				return;
			}
			_oldFocusedID = Function.ParseInt(treeList_Lines.FocusedNode.GetValue("LineID"));
			_isFNodeChanging = true;
			LoadALine(_oldFocusedID);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isFNodeChanging = false;
	}

	private void treeList_Lines_CustomDrawNodeCell(object sender, CustomDrawNodeCellEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Branch" && Function.ParseInt(e.Node.GetValue("LineID")) >= 0)
			{
				e.CellText = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Items_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void treeList_Lines_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "LineType" && Function.ToString(e.Value) == "A")
			{
				AddNewRowItem(sender);
				addRowToolStripMenuItem2_Click(null, null);
			}
			SetButtonOKText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void treeList_Lines_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e)
	{
		try
		{
			if (_isFNodeChanging)
			{
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
				{
					_isFNodeChanging = true;
					e.CanFocus = false;
				}
				else
				{
					_isFNodeChanging = true;
					LoadLines();
					FormMode = Enums.FormViewModeType.View;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isFNodeChanging = false;
	}

	private void SetButtonOKText(object sender)
	{
		try
		{
			if (!_isFNodeChanging)
			{
				if (FormMode == Enums.FormViewModeType.View)
				{
					FormMode = Enums.FormViewModeType.Update;
				}
				if (FormMode == Enums.FormViewModeType.Add && !_isAdd)
				{
					_isAdd = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void grd_Items_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control && Function.ToString(treeList_Lines.FocusedNode.GetValue("LineType")) == "A")
			{
				ExtenseionMethods.PasteFromExcelToDataTable(grd_Items, gv_Items, new List<string> { "ItemCode" });
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_ItemCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 0)
			{
				frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItemsMRPLines, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
				frm_CFL2.MrpLineBplid = Function.ParseInt(treeList_Lines.FocusedNode.GetValue("Branch"));
				frm_CFL2.ShowDialog(this);
				if (frm_CFL2.ChooseItems != null && frm_CFL2.ChooseItems.Count > 0)
				{
					List<DataRow> chooseItems = frm_CFL2.ChooseItems;
					DataTable dataTable = grd_Items.DataSource as DataTable;
					DataRow dataRow = dataTable.Rows[gv_Items.GetFocusedDataSourceRowIndex()];
					DataRow dataRow2 = chooseItems[0];
					string value = Function.ToString(dataRow2["ItemCode"]);
					int num = Function.ParseInt(dataRow["LineID"]);
					dataRow["ItemCode"] = value;
					dataRow["ItemName"] = Function.ToString(dataRow2["ItemDescription"]);
					dataRow["CycleTime"] = 0.0;
					dataRow["TotalTime"] = 0.0;
					dataRow["NumberOfMch"] = 0.0;
					for (int i = 1; i < chooseItems.Count; i++)
					{
						DataRow dataRow3 = chooseItems[i];
						DataRow dataRow4 = dataTable.NewRow();
						string value2 = Function.ToString(dataRow3["ItemCode"]);
						dataRow4["LineID"] = num;
						dataRow4["ItemCode"] = value2;
						dataRow4["ItemName"] = Function.ToString(dataRow3["ItemDescription"]);
						dataRow4["CycleTime"] = 0.0;
						dataRow4["TotalTime"] = 0.0;
						dataRow4["NumberOfMch"] = 0.0;
						dataTable.Rows.Add(dataRow4);
					}
					dataTable.AcceptChanges();
					grd_Items.DataSource = dataTable;
					grd_Items.RefreshDataSource();
					gv_Items.RefreshRow(gv_Items.FocusedRowHandle);
					AddNewRowItem(sender);
				}
			}
			if (e.Button.Index == 1)
			{
				this.DrillDown(ApzObjectTypes.oProductionOrders, ApzObjectTypes.oItems, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void rep_UserCode_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.ListItemsMRPUsers, true, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems == null || frm_CFL2.ChooseItems.Count <= 0)
			{
				return;
			}
			List<DataRow> chooseItems = frm_CFL2.ChooseItems;
			DataTable dataTable = grd_Users.DataSource as DataTable;
			string text = "";
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				text = text + "," + Function.ToString(dataTable.Rows[i]["User_Code"]);
			}
			for (int num = chooseItems.Count - 1; num >= 0; num--)
			{
				if (text.Contains(Function.ToString(chooseItems[num]["Code"])))
				{
					chooseItems.RemoveAt(num);
				}
			}
			if (chooseItems != null && chooseItems.Count != 0)
			{
				DataRow dataRow = dataTable.Rows[gv_Users.GetFocusedDataSourceRowIndex()];
				DataRow dataRow2 = chooseItems[0];
				int num2 = Function.ParseInt(dataRow["LineID"]);
				dataRow["LineID"] = num2;
				dataRow["User_Code"] = Function.ToString(dataRow2["Code"]);
				for (int j = 1; j < chooseItems.Count; j++)
				{
					DataRow dataRow3 = chooseItems[j];
					DataRow dataRow4 = dataTable.NewRow();
					string value = Function.ToString(dataRow3["Code"]);
					dataRow4["LineID"] = num2;
					dataRow4["User_Code"] = value;
					dataTable.Rows.Add(dataRow4);
				}
				dataTable.AcceptChanges();
				grd_Users.DataSource = dataTable;
				grd_Users.RefreshDataSource();
				gv_Users.RefreshRow(gv_Users.FocusedRowHandle);
				addRowToolStripMenuItem2_Click(sender, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btn_Save_Click(object sender, EventArgs e)
	{
		DisableControls(p: true);
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (DataValidate())
			{
				SaveData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		DisableControls(p: false);
	}

	private void treeList_Lines_Enter(object sender, EventArgs e)
	{
		_treeListFocused = true;
	}

	private void treeList_Lines_Leave(object sender, EventArgs e)
	{
		_treeListFocused = false;
	}

	protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
	{
		try
		{
			if (keyData == Keys.Tab)
			{
				if (_treeListFocused)
				{
					int visibleIndex = treeList_Lines.FocusedColumn.VisibleIndex;
					visibleIndex = ((visibleIndex != treeList_Lines.VisibleColumns.Count) ? (visibleIndex + 1) : 0);
					treeList_Lines.FocusedColumn = treeList_Lines.VisibleColumns[visibleIndex];
					treeList_Lines.ShowEditor();
					return true;
				}
				return base.ProcessCmdKey(ref msg, keyData);
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return base.ProcessCmdKey(ref msg, keyData);
		}
	}

	private void addRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (!flag || FormMode == Enums.FormViewModeType.Add)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Add;
			DataTable dataTable = treeList_Lines.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = GetTableStructure("L");
			}
			int num = 0;
			try
			{
				num = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineID"]));
			}
			catch (Exception)
			{
			}
			DataTable dataTable2 = dataTable.Clone();
			_ = treeList_Lines.FocusedNode;
			DataRow dataRow = dataTable2.NewRow();
			num = ((num < 0) ? 1 : (num + 1));
			dataRow["LineID"] = num;
			dataRow["LineCode"] = "";
			dataRow["LineName"] = "";
			dataRow["LineType"] = "T";
			dataRow["TotalOfMch"] = 0.0;
			dataRow["Perfomance"] = 0.0;
			dataTable2.Rows.Add(dataRow);
			dataTable2.AcceptChanges();
			treeList_Lines.DataSource = dataTable2;
			treeList_Lines.RefreshDataSource();
			treeList_Lines.ForceInitialize();
			treeList_Lines.LayoutChanged();
			addRowToolStripMenuItem2_Click(null, null);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			DataTable dataTable = treeList_Lines.DataSource as DataTable;
			int num = Function.ParseInt(treeList_Lines.FocusedNode.GetValue("LineID"));
			if (num < 0)
			{
				return;
			}
			using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
			string text = httpClientHelper.Post("DeleteLine", num);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
				treeList_Lines.DeleteNode(treeList_Lines.FocusedNode);
				dataTable.AcceptChanges();
				treeList_Lines.DataSource = dataTable;
				treeList_Lines.RefreshDataSource();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(treeList_Lines.FocusedNode.GetValue("LineType")) == "A")
			{
				AddNewRowItem(sender);
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.CannotAddItemsWhenLineNotActive, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewRowItem(object sender)
	{
		try
		{
			SetButtonOKText(sender);
			if (Function.ToString(treeList_Lines.FocusedNode.GetValue("LineType")) == "A")
			{
				DataTable dataTable = grd_Items.DataSource as DataTable;
				if (dataTable == null || dataTable.Columns.Count == 0)
				{
					dataTable = GetTableStructure("I").Clone();
				}
				if (!dataTable.AsEnumerable().Any((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["ItemCode"]))))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["LineID"] = Function.ParseInt(treeList_Lines.FocusedNode.GetValue("LineID"));
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
					grd_Items.DataSource = dataTable;
					grd_Items.RefreshDataSource();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			SetButtonOKText(sender);
			DataTable dataTable = grd_Items.DataSource as DataTable;
			gv_Items.DeleteSelectedRows();
			dataTable.AcceptChanges();
			grd_Items.DataSource = dataTable;
			grd_Items.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addRowToolStripMenuItem2_Click(object sender, EventArgs e)
	{
		try
		{
			SetButtonOKText(sender);
			DataTable dataTable = grd_Users.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = GetTableStructure("U").Clone();
			}
			if (!dataTable.AsEnumerable().Any((DataRow t) => string.IsNullOrEmpty(Function.ToString(t["User_Code"]))))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["LineID"] = Function.ParseInt(treeList_Lines.FocusedNode.GetValue("LineID"));
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				grd_Users.DataSource = dataTable;
				grd_Users.RefreshDataSource();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void removeRowToolStripMenuItem2_Click(object sender, EventArgs e)
	{
		try
		{
			SetButtonOKText(sender);
			DataTable dataTable = grd_Users.DataSource as DataTable;
			gv_Users.DeleteSelectedRows();
			dataTable.AcceptChanges();
			grd_Users.DataSource = dataTable;
			grd_Users.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addChildRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (treeList_Lines.FocusedNode == null)
			{
				return;
			}
			bool flag = true;
			if (FormMode == Enums.FormViewModeType.Update && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				flag = false;
			}
			if (!flag || FormMode == Enums.FormViewModeType.Add)
			{
				return;
			}
			FormMode = Enums.FormViewModeType.Add;
			DataTable dataTable = treeList_Lines.DataSource as DataTable;
			if (dataTable == null || dataTable.Columns.Count == 0)
			{
				dataTable = GetTableStructure("L");
			}
			int maxId = 0;
			try
			{
				maxId = dataTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["LineID"]));
			}
			catch (Exception)
			{
			}
			DataTable dataTable2 = dataTable.Clone();
			DataRow dataRow = dataTable2.NewRow();
			maxId = ((maxId < 0) ? 1 : (maxId + 1));
			dataRow["LineID"] = maxId;
			dataRow["LineCode"] = "";
			dataRow["LineName"] = "";
			dataRow["LineType"] = "T";
			dataRow["TotalOfMch"] = 0.0;
			dataRow["Perfomance"] = 0.0;
			dataRow["Branch"] = treeList_Lines.FocusedNode.GetValue("Branch");
			object cellValue = (dataRow["FatherLine"] = treeList_Lines.FocusedNode.GetValue("LineID"));
			TreeListNode treeListNode = treeList_Lines.FindNodeByFieldValue("LineID", cellValue);
			treeListNode.Expanded = true;
			dataTable2.Rows.Add(dataRow);
			dataTable2.AcceptChanges();
			treeList_Lines.DataSource = dataTable2;
			treeList_Lines.RefreshDataSource();
			treeList_Lines.ForceInitialize();
			treeList_Lines.LayoutChanged();
			TreeListNode treeListNode2 = treeListNode.Nodes.FirstOrDefault((TreeListNode t) => Function.ParseInt(t.GetValue("LineID")) == maxId);
			if (treeListNode2 != null)
			{
				treeList_Lines.SetFocusedNode(treeListNode2);
			}
			addRowToolStripMenuItem2_Click(null, null);
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	private void LoadLines()
	{
		try
		{
			bool isFNodeChanging = _isFNodeChanging;
			using (HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController))
			{
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("BPLId", typeof(int));
				DataRow dataRow = dataTable.NewRow();
				if (GlobalData.IsMultiBranch)
				{
					dataRow["BPLId"] = Function.ParseInt(look_Branch.EditValue);
				}
				else
				{
					dataRow["BPLId"] = -1;
				}
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
				DataTable dataTable2 = httpClientHelper.Post("GetLines", dataTable);
				if (dataTable2 != null && dataTable2.Rows.Count == 1)
				{
					LoadALine(Function.ParseInt(dataTable2.Rows[0]["LineID"]));
				}
				if (dataTable2 == null || dataTable2.Columns.Count == 0)
				{
					dataTable2 = GetTableStructure("L");
				}
				if (GlobalData.IsMultiBranch)
				{
					if (Function.ParseInt(look_Branch.EditValue) == -1)
					{
						if (look_Branch.Properties.DataSource is DataTable dataTable3 && dataTable3.Rows.Count > 0)
						{
							for (int i = 0; i < dataTable3.Rows.Count; i++)
							{
								int num = Function.ParseInt(dataTable3.Rows[i]["Value"]);
								if (num > 0)
								{
									DataRow dataRow2 = dataTable2.NewRow();
									dataRow2["LineID"] = num * -1;
									dataRow2["Branch"] = num;
									dataRow2["LineCode"] = "";
									dataRow2["LineName"] = "";
									dataRow2["LineType"] = "";
									dataRow2["FatherLine"] = -100;
									dataRow2["TotalOfMch"] = 0.0;
									dataRow2["Perfomance"] = 0.0;
									dataTable2.Rows.Add(dataRow2);
									dataTable2.AcceptChanges();
								}
							}
						}
					}
					else
					{
						int num2 = Function.ParseInt(look_Branch.EditValue);
						if (num2 > 0)
						{
							DataRow dataRow3 = dataTable2.NewRow();
							dataRow3["LineID"] = num2 * -1;
							dataRow3["Branch"] = num2;
							dataRow3["LineCode"] = "";
							dataRow3["LineName"] = "";
							dataRow3["LineType"] = "";
							dataRow3["FatherLine"] = -100;
							dataRow3["TotalOfMch"] = 0.0;
							dataRow3["Perfomance"] = 0.0;
							dataTable2.Rows.Add(dataRow3);
							dataTable2.AcceptChanges();
						}
					}
				}
				else
				{
					DataRow dataRow4 = dataTable2.NewRow();
					dataRow4["LineID"] = -1;
					dataRow4["Branch"] = -1;
					dataRow4["LineCode"] = "";
					dataRow4["LineName"] = "";
					dataRow4["LineType"] = "";
					dataRow4["FatherLine"] = -100;
					dataRow4["TotalOfMch"] = 0.0;
					dataRow4["Perfomance"] = 0.0;
					dataTable2.Rows.Add(dataRow4);
					dataTable2.AcceptChanges();
				}
				_isFNodeChanging = true;
				treeList_Lines.DataSource = dataTable2;
				treeList_Lines.RefreshDataSource();
				_isFNodeChanging = isFNodeChanging;
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetFormatControlValue()
	{
		try
		{
			for (int i = 0; i < treeList_Lines.Columns.Count; i++)
			{
				string fieldName = treeList_Lines.Columns[i].FieldName;
				if (!(fieldName == "TotalOfMch"))
				{
					if (fieldName == "Perfomance")
					{
						treeList_Lines.Columns[i].Format.FormatType = FormatType.Numeric;
						treeList_Lines.Columns[i].Format.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
					}
				}
				else
				{
					treeList_Lines.Columns[i].Format.FormatType = FormatType.Numeric;
					treeList_Lines.Columns[i].Format.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
				}
			}
			for (int j = 0; j < gv_Items.Columns.Count; j++)
			{
				switch (gv_Items.Columns[j].FieldName)
				{
				case "CycleTime":
					gv_Items.Columns[j].DisplayFormat.FormatType = FormatType.Numeric;
					gv_Items.Columns[j].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
					break;
				case "TotalTime":
					gv_Items.Columns[j].DisplayFormat.FormatType = FormatType.Numeric;
					gv_Items.Columns[j].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
					break;
				case "NumberOfMch":
					gv_Items.Columns[j].DisplayFormat.FormatType = FormatType.Numeric;
					gv_Items.Columns[j].DisplayFormat.FormatString = "n" + GlobalData.QuantitiesFormat.NumberDecimalDigits;
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

	private DataTable GetTableStructure(string type)
	{
		DataTable dataTable = new DataTable();
		try
		{
			switch (type)
			{
			case "L":
				dataTable.Columns.Add("Branch", typeof(int));
				dataTable.Columns.Add("LineCode", typeof(string));
				dataTable.Columns.Add("LineName", typeof(string));
				dataTable.Columns.Add("LineType", typeof(string));
				dataTable.Columns.Add("FatherLine", typeof(int));
				dataTable.Columns.Add("TotalOfMch", typeof(double));
				dataTable.Columns.Add("Perfomance", typeof(double));
				dataTable.Columns.Add("LineID", typeof(int));
				break;
			case "I":
				dataTable.Columns.Add("LineID", typeof(int));
				dataTable.Columns.Add("ItemCode", typeof(string));
				dataTable.Columns.Add("ItemName", typeof(string));
				dataTable.Columns.Add("CycleTime", typeof(int));
				dataTable.Columns.Add("TotalTime", typeof(int));
				dataTable.Columns.Add("NumberOfMch", typeof(double));
				break;
			case "U":
				dataTable.Columns.Add("LineID", typeof(int));
				dataTable.Columns.Add("User_Code", typeof(string));
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return dataTable;
	}

	private void LoadALine(int LineID)
	{
		try
		{
			using (HttpClientHelper<DataSet> httpClientHelper = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController))
			{
				DataSet dataSet = httpClientHelper.Get("GetALine", new { LineID });
				if (dataSet != null)
				{
					if (dataSet.Tables.Count > 0 && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
					{
						grd_Items.DataSource = dataSet.Tables[0];
						grd_Items.RefreshDataSource();
					}
					else
					{
						grd_Items.DataSource = GetTableStructure("I");
						grd_Items.RefreshDataSource();
					}
					if (dataSet.Tables.Count > 1 && dataSet.Tables[1] != null && dataSet.Tables[1].Rows.Count > 0)
					{
						grd_Users.DataSource = dataSet.Tables[1];
						grd_Users.RefreshDataSource();
					}
					else
					{
						grd_Users.DataSource = GetTableStructure("U");
						grd_Users.RefreshDataSource();
					}
				}
			}
			AddNewRowItem("LoadALine()");
			addRowToolStripMenuItem2_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void SetDataBranch()
	{
		try
		{
			look_Branch.Properties.NullText = "";
			look_Branch.Properties.Columns.Add(new LookUpColumnInfo("Name", "Name"));
			look_Branch.Properties.Columns.Add(new LookUpColumnInfo("Value", "Value"));
			look_Branch.Properties.DisplayMember = "Name";
			look_Branch.Properties.ValueMember = "Value";
			if (!GlobalData.IsMultiBranch)
			{
				look_Branch.Enabled = false;
				return;
			}
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			DataTable dataTable = httpClientHelper.Get("GetDataBrach", new
			{
				userCode = GlobalData.UserCodeLogin
			});
			rep_Branch.DataSource = dataTable;
			if (dataTable == null)
			{
				dataTable = new DataTable();
			}
			if (dataTable.Columns.Count == 0)
			{
				dataTable.Columns.Add("Value", typeof(long));
				dataTable.Columns.Add("Name", typeof(string));
			}
			DataTable dataTable2 = dataTable.Copy();
			DataRow dataRow = dataTable2.NewRow();
			dataRow["Value"] = -1L;
			dataRow["Name"] = "All";
			dataTable2.Rows.Add(dataRow);
			dataTable2.AcceptChanges();
			look_Branch.Properties.DataSource = dataTable2;
			look_Branch.EditValue = (long)GlobalData.DefaultBranch;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void LoadComboBox()
	{
		try
		{
			SetDataBranch();
			rep_LineType.SetDataSource(new string[2, 2]
			{
				{ "T", "Title" },
				{ "A", "Active" }
			}, isStringCode: true);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private bool DataValidate()
	{
		try
		{
			if (!(treeList_Lines.DataSource is DataTable dataTable) || dataTable.Rows.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.DataLinesIsEmpty, "");
				return false;
			}
			DataRow row = ((DataRowView)treeList_Lines.GetDataRecordByNode(treeList_Lines.FocusedNode)).Row;
			if (Function.ParseInt(row["LineID"]) <= 0)
			{
				WinCommons.Error(Enums.MessageCode.MustFocusedOnChildNode, "");
				return false;
			}
			if (string.IsNullOrWhiteSpace(Function.ToString(row["LineCode"])) || string.IsNullOrWhiteSpace(Function.ToString(row["LineName"])) || Function.ParseDouble(row["TotalOfMch"]) <= 0.0)
			{
				WinCommons.Error(Enums.MessageCode.LineCodeLineNameCannotBeEmptyTotalMachineMustGreaterThan0, "");
				return false;
			}
			if (ExistsLineCode(Function.ToString(row["LineCode"]), Function.ParseInt(row["LineID"])))
			{
				WinCommons.Error(Enums.MessageCode.LineCodeExisted, "");
				return false;
			}
			int num = Function.ParseInt(row["FatherLine"]);
			if (num > 0 && !ExistsLine(num))
			{
				WinCommons.Error(Enums.MessageCode.FatherLineDoesNotExisted, "");
				return false;
			}
			DataTable dataTable2 = grd_Items.DataSource as DataTable;
			string text = Function.ToString(treeList_Lines.FocusedNode.GetValue("LineType"));
			if ((dataTable2 == null || dataTable2.Rows.Count == 0) && text == "A")
			{
				WinCommons.Error(Enums.MessageCode.ItemsDataIsEmpty, "");
				return false;
			}
			if (!(grd_Users.DataSource is DataTable dataTable3) || dataTable3.Rows.Count == 0 || !dataTable3.AsEnumerable().Any((DataRow r) => !string.IsNullOrEmpty(Function.ToString(r["User_Code"]))))
			{
				WinCommons.Error(Enums.MessageCode.UsersDataIsEmpty, "");
				return false;
			}
			for (int num2 = 0; num2 < dataTable3.Rows.Count; num2++)
			{
				if (!string.IsNullOrWhiteSpace(Function.ToString(dataTable3.Rows[num2]["User_Code"])) && !WinFuntions.CheckExistValidValue("CheckExistValidUserCode", Function.ToString(dataTable3.Rows[num2]["User_Code"])))
				{
					WinCommons.Error(Enums.MessageCode.InvalidUserCode, "");
					return false;
				}
			}
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return false;
		}
	}

	private bool ExistsLineCode(string LineCode, int LineID)
	{
		bool result = false;
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataTable = httpClientHelper.Get("ExistsLineCode", new { LineCode, LineID });
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = false;
		}
		return result;
	}

	private bool ExistsLine(int LineID)
	{
		bool result = false;
		try
		{
			using HttpClientHelper<DataTable> httpClientHelper = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataTable = httpClientHelper.Get("ExistsLine", new { LineID });
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				result = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			result = false;
		}
		return result;
	}

	private void SaveData()
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				DataSet dataSet = new DataSet();
				dataSet.Tables.Clear();
				DataTable dataTable = (treeList_Lines.DataSource as DataTable).Clone();
				DataRow row = ((DataRowView)treeList_Lines.GetDataRecordByNode(treeList_Lines.FocusedNode)).Row;
				dataTable.ImportRow(row);
				dataTable.TableName = "Line";
				dataTable.AcceptChanges();
				dataTable.DataSet?.Tables.Remove("Line");
				DataTable dataTable2 = grd_Items.DataSource as DataTable;
				for (int num = dataTable2.Rows.Count - 1; num >= 0; num--)
				{
					DataRow dataRow = dataTable2.Rows[num];
					if (string.IsNullOrWhiteSpace(Function.ToString(dataRow["ItemCode"])))
					{
						dataRow.Delete();
					}
				}
				dataTable2.TableName = "Item";
				dataTable2.DataSet?.Tables.Remove("Item");
				dataTable2.AcceptChanges();
				DataTable dataTable3 = grd_Users.DataSource as DataTable;
				for (int num2 = dataTable3.Rows.Count - 1; num2 >= 0; num2--)
				{
					DataRow dataRow2 = dataTable3.Rows[num2];
					if (string.IsNullOrWhiteSpace(Function.ToString(dataRow2["User_Code"])))
					{
						dataRow2.Delete();
					}
				}
				dataTable3.TableName = "User";
				dataTable3.DataSet?.Tables.Remove("User");
				dataTable3.AcceptChanges();
				dataSet.Tables.Add(dataTable);
				dataSet.Tables.Add(dataTable2);
				dataSet.Tables.Add(dataTable3);
				using HttpClientHelper<string> httpClientHelper = new HttpClientHelper<string>(Configuration.ApiServer, _formController);
				string text = httpClientHelper.Post("Save", dataSet);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					LoadLines();
					FormMode = Enums.FormViewModeType.View;
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
		WinCommons._CloseProcessing();
	}

	private void DisableControls(bool p)
	{
		try
		{
			btn_Cancel.Enabled = !p;
			btn_Save.Enabled = !p;
			look_Branch.Enabled = !p;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Mrps.frm_Lines));
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		this.ctx_Lines = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addChildRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.ctx_Items = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.ctx_Users = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addRowToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbBranch = new Apzon.Controls.Controls.LabelLine();
		this.look_Branch = new DevExpress.XtraEditors.LookUpEdit();
		this.splitContainer1 = new System.Windows.Forms.SplitContainer();
		this.treeList_Lines = new Apzon.Controls.Tree.TreeEx();
		this.treeListColumn1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumn2 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn3 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn4 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_LineType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.treeListColumn6 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn7 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Items = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Items = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ItemCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabPage3 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Users = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Users = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_UserCode = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.ctx_Lines.SuspendLayout();
		this.ctx_Items.SuspendLayout();
		this.ctx_Users.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).BeginInit();
		this.splitContainer1.Panel1.SuspendLayout();
		this.splitContainer1.Panel2.SuspendLayout();
		this.splitContainer1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.treeList_Lines).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Items).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Items).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).BeginInit();
		this.xtraTabPage3.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Users).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Users).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UserCode).BeginInit();
		base.SuspendLayout();
		this.ctx_Lines.Items.AddRange(new System.Windows.Forms.ToolStripItem[3] { this.addRowToolStripMenuItem, this.removeRowToolStripMenuItem, this.addChildRowToolStripMenuItem });
		this.ctx_Lines.Name = "ctx_Lines";
		this.ctx_Lines.Size = new System.Drawing.Size(154, 70);
		this.addRowToolStripMenuItem.Name = "addRowToolStripMenuItem";
		this.addRowToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
		this.addRowToolStripMenuItem.Text = "Add Row";
		this.addRowToolStripMenuItem.Click += new System.EventHandler(this.addRowToolStripMenuItem_Click);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.addChildRowToolStripMenuItem.Name = "addChildRowToolStripMenuItem";
		this.addChildRowToolStripMenuItem.Size = new System.Drawing.Size(153, 22);
		this.addChildRowToolStripMenuItem.Text = "Add Child Row";
		this.addChildRowToolStripMenuItem.Click += new System.EventHandler(this.addChildRowToolStripMenuItem_Click);
		this.ctx_Items.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem1, this.removeRowToolStripMenuItem1 });
		this.ctx_Items.Name = "ctx_Lines";
		this.ctx_Items.Size = new System.Drawing.Size(144, 48);
		this.addRowToolStripMenuItem1.Name = "addRowToolStripMenuItem1";
		this.addRowToolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
		this.addRowToolStripMenuItem1.Text = "Add Row";
		this.addRowToolStripMenuItem1.Click += new System.EventHandler(this.addRowToolStripMenuItem1_Click);
		this.removeRowToolStripMenuItem1.Name = "removeRowToolStripMenuItem1";
		this.removeRowToolStripMenuItem1.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem1.Text = "Remove Row";
		this.removeRowToolStripMenuItem1.Click += new System.EventHandler(this.removeRowToolStripMenuItem1_Click);
		this.ctx_Users.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addRowToolStripMenuItem2, this.removeRowToolStripMenuItem2 });
		this.ctx_Users.Name = "ctx_Lines";
		this.ctx_Users.Size = new System.Drawing.Size(144, 48);
		this.addRowToolStripMenuItem2.Name = "addRowToolStripMenuItem2";
		this.addRowToolStripMenuItem2.Size = new System.Drawing.Size(143, 22);
		this.addRowToolStripMenuItem2.Text = "Add Row";
		this.addRowToolStripMenuItem2.Click += new System.EventHandler(this.addRowToolStripMenuItem2_Click);
		this.removeRowToolStripMenuItem2.Name = "removeRowToolStripMenuItem2";
		this.removeRowToolStripMenuItem2.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem2.Text = "Remove Row";
		this.removeRowToolStripMenuItem2.Click += new System.EventHandler(this.removeRowToolStripMenuItem2_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(77, 468);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(70, 20);
		this.btn_Cancel.TabIndex = 16;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(4, 468);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(70, 20);
		this.btn_Save.TabIndex = 15;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.lbBranch.EditControl = null;
		this.lbBranch.EditValue = "Branch";
		this.lbBranch.FieldName = null;
		this.lbBranch.HideWhenEmpty = false;
		this.lbBranch.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbBranch.Location = new System.Drawing.Point(4, 5);
		this.lbBranch.Name = "lbBranch";
		this.lbBranch.ReadOnly = false;
		this.lbBranch.SettingInVisible = false;
		this.lbBranch.SettingReadonly = false;
		this.lbBranch.ShowUnderLine = true;
		this.lbBranch.Size = new System.Drawing.Size(70, 20);
		this.lbBranch.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbBranch.TabIndex = 17;
		this.lbBranch.Utext = "Branch";
		this.look_Branch.Location = new System.Drawing.Point(74, 3);
		this.look_Branch.Name = "look_Branch";
		this.look_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Branch.Properties.DisplayMember = "Name";
		this.look_Branch.Properties.NullText = "";
		this.look_Branch.Properties.ShowFooter = false;
		this.look_Branch.Properties.ShowHeader = false;
		this.look_Branch.Properties.ValueMember = "Value";
		this.look_Branch.Size = new System.Drawing.Size(120, 20);
		this.look_Branch.TabIndex = 18;
		this.look_Branch.EditValueChanged += new System.EventHandler(this.look_Branch_EditValueChanged);
		this.splitContainer1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainer1.Location = new System.Drawing.Point(4, 31);
		this.splitContainer1.Name = "splitContainer1";
		this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
		this.splitContainer1.Panel1.Controls.Add(this.treeList_Lines);
		this.splitContainer1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainer1.Size = new System.Drawing.Size(748, 431);
		this.splitContainer1.SplitterDistance = 215;
		this.splitContainer1.TabIndex = 19;
		this.treeList_Lines.AllowPasteExcel = false;
		this.treeList_Lines.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.treeList_Lines.ColumnKey = null;
		this.treeList_Lines.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[6] { this.treeListColumn1, this.treeListColumn2, this.treeListColumn3, this.treeListColumn4, this.treeListColumn6, this.treeListColumn7 });
		this.treeList_Lines.ContextMenuStrip = this.ctx_Lines;
		this.treeList_Lines.DisableFormSetting = false;
		this.treeList_Lines.Dock = System.Windows.Forms.DockStyle.Fill;
		this.treeList_Lines.EditColumn = null;
		this.treeList_Lines.FieldCurrency = null;
		this.treeList_Lines.FieldRate = null;
		this.treeList_Lines.FormId = "oLInes";
		this.treeList_Lines.IndexEditRow = -1;
		this.treeList_Lines.IndicatorWidth = 40;
		this.treeList_Lines.IsApzTable = false;
		this.treeList_Lines.IsLoadingData = false;
		this.treeList_Lines.ItemId = "APZ_OLIN";
		this.treeList_Lines.KeyFieldName = "LineID";
		this.treeList_Lines.Location = new System.Drawing.Point(0, 0);
		this.treeList_Lines.LstColumnNotEditable = null;
		this.treeList_Lines.LstColumNotSetting = null;
		this.treeList_Lines.Name = "treeList_Lines";
		this.treeList_Lines.ParentFieldName = "FatherLine";
		this.treeList_Lines.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_LineType, this.rep_Branch });
		this.treeList_Lines.Size = new System.Drawing.Size(748, 215);
		this.treeList_Lines.TabIndex = 1;
		this.treeList_Lines.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.treeList_Lines_CellValueChanged);
		this.treeList_Lines.BeforeFocusNode += new DevExpress.XtraTreeList.BeforeFocusNodeEventHandler(this.treeList_Lines_BeforeFocusNode);
		this.treeList_Lines.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.treeList_Lines_FocusedNodeChanged);
		this.treeList_Lines.CustomDrawNodeCell += new DevExpress.XtraTreeList.CustomDrawNodeCellEventHandler(this.treeList_Lines_CustomDrawNodeCell);
		this.treeList_Lines.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.treeList_Lines_ShowingEditor);
		this.treeList_Lines.Enter += new System.EventHandler(this.treeList_Lines_Enter);
		this.treeList_Lines.Leave += new System.EventHandler(this.treeList_Lines_Leave);
		this.treeListColumn1.Caption = "Branch";
		this.treeListColumn1.ColumnEdit = this.rep_Branch;
		this.treeListColumn1.CurrencyDateField = null;
		this.treeListColumn1.CurrencyField = null;
		this.treeListColumn1.CurrencyFixed = null;
		this.treeListColumn1.CurrencyFixedRate = 0.0;
		this.treeListColumn1.CurrencyRateField = null;
		this.treeListColumn1.FieldName = "Branch";
		this.treeListColumn1.Name = "treeListColumn1";
		this.treeListColumn1.Visible = true;
		this.treeListColumn1.VisibleIndex = 0;
		this.treeListColumn1.Width = 121;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Value", "ID"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name")
		});
		this.rep_Branch.DisplayMember = "Name";
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.NullText = "";
		this.rep_Branch.ValueMember = "Value";
		this.treeListColumn2.Caption = "LineCode";
		this.treeListColumn2.CurrencyDateField = null;
		this.treeListColumn2.CurrencyField = null;
		this.treeListColumn2.CurrencyFixed = null;
		this.treeListColumn2.CurrencyFixedRate = 0.0;
		this.treeListColumn2.CurrencyRateField = null;
		this.treeListColumn2.FieldName = "LineCode";
		this.treeListColumn2.Name = "treeListColumn2";
		this.treeListColumn2.Visible = true;
		this.treeListColumn2.VisibleIndex = 1;
		this.treeListColumn2.Width = 121;
		this.treeListColumn3.Caption = "LineName";
		this.treeListColumn3.CurrencyDateField = null;
		this.treeListColumn3.CurrencyField = null;
		this.treeListColumn3.CurrencyFixed = null;
		this.treeListColumn3.CurrencyFixedRate = 0.0;
		this.treeListColumn3.CurrencyRateField = null;
		this.treeListColumn3.FieldName = "LineName";
		this.treeListColumn3.Name = "treeListColumn3";
		this.treeListColumn3.Visible = true;
		this.treeListColumn3.VisibleIndex = 2;
		this.treeListColumn3.Width = 121;
		this.treeListColumn4.Caption = "LineType";
		this.treeListColumn4.ColumnEdit = this.rep_LineType;
		this.treeListColumn4.CurrencyDateField = null;
		this.treeListColumn4.CurrencyField = null;
		this.treeListColumn4.CurrencyFixed = null;
		this.treeListColumn4.CurrencyFixedRate = 0.0;
		this.treeListColumn4.CurrencyRateField = null;
		this.treeListColumn4.FieldName = "LineType";
		this.treeListColumn4.Name = "treeListColumn4";
		this.treeListColumn4.Visible = true;
		this.treeListColumn4.VisibleIndex = 3;
		this.treeListColumn4.Width = 121;
		this.rep_LineType.AutoHeight = false;
		this.rep_LineType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_LineType.Name = "rep_LineType";
		this.treeListColumn6.Caption = "TotalOfMch";
		this.treeListColumn6.CurrencyDateField = null;
		this.treeListColumn6.CurrencyField = null;
		this.treeListColumn6.CurrencyFixed = null;
		this.treeListColumn6.CurrencyFixedRate = 0.0;
		this.treeListColumn6.CurrencyRateField = null;
		this.treeListColumn6.FieldName = "TotalOfMch";
		this.treeListColumn6.Name = "treeListColumn6";
		this.treeListColumn6.Visible = true;
		this.treeListColumn6.VisibleIndex = 4;
		this.treeListColumn6.Width = 121;
		this.treeListColumn7.Caption = "Perfomance";
		this.treeListColumn7.CurrencyDateField = null;
		this.treeListColumn7.CurrencyField = null;
		this.treeListColumn7.CurrencyFixed = null;
		this.treeListColumn7.CurrencyFixedRate = 0.0;
		this.treeListColumn7.CurrencyRateField = null;
		this.treeListColumn7.FieldName = "Perfomance";
		this.treeListColumn7.Name = "treeListColumn7";
		this.treeListColumn7.Visible = true;
		this.treeListColumn7.VisibleIndex = 5;
		this.treeListColumn7.Width = 121;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage2;
		this.xtraTabControl1.Size = new System.Drawing.Size(748, 212);
		this.xtraTabControl1.TabIndex = 1;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.xtraTabPage2, this.xtraTabPage3 });
		this.xtraTabPage2.Controls.Add(this.grd_Items);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "Items";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(746, 187);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Items";
		this.grd_Items.AllowPasteExcel = true;
		this.grd_Items.ClipboardData = null;
		this.grd_Items.ContextMenuStrip = this.ctx_Items;
		this.grd_Items.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Items.Location = new System.Drawing.Point(0, 0);
		this.grd_Items.MainView = this.gv_Items;
		this.grd_Items.Name = "grd_Items";
		this.grd_Items.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_ItemCode });
		this.grd_Items.Size = new System.Drawing.Size(746, 187);
		this.grd_Items.TabIndex = 0;
		this.grd_Items.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Items });
		this.grd_Items.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Items_EditorKeyDown);
		this.gv_Items.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Items.ColumnKey = null;
		this.gv_Items.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6 });
		this.gv_Items.DisableFormSetting = false;
		this.gv_Items.DisplayException = false;
		this.gv_Items.EditColumn = null;
		this.gv_Items.FieldCurrency = null;
		this.gv_Items.FieldRate = null;
		this.gv_Items.FormId = "oLInes";
		this.gv_Items.GridControl = this.grd_Items;
		this.gv_Items.IgnoreFilter = false;
		this.gv_Items.IndexEditRow = -1;
		this.gv_Items.IndicatorWidth = 70;
		this.gv_Items.IsApzTable = false;
		this.gv_Items.IsInsertNewRow = false;
		this.gv_Items.IsLoadingData = false;
		this.gv_Items.IsMappingContextAction = false;
		this.gv_Items.ItemId = "OITM";
		this.gv_Items.LstColumnAllowBatch = null;
		this.gv_Items.LstColumnNotAllowBatch = null;
		this.gv_Items.LstColumnNotEditable = null;
		this.gv_Items.LstColumNotSetting = null;
		this.gv_Items.Name = "gv_Items";
		this.gv_Items.OptionsSelection.MultiSelect = true;
		this.gv_Items.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Items.OptionsView.ShowGroupPanel = false;
		this.gv_Items.UseBatchMod = false;
		this.gv_Items.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Items_CustomDrawRowIndicator);
		this.gridColumn1.Caption = "LineID";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "LineID";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn1.PrimaryKey = true;
		this.gridColumn1.UnitField = null;
		this.gridColumn2.Caption = "ItemCode";
		this.gridColumn2.ColumnEdit = this.rep_ItemCode;
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "ItemCode";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.rep_ItemCode.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		this.rep_ItemCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_ItemCode.Name = "rep_ItemCode";
		this.rep_ItemCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_ItemCode_ButtonPressed);
		this.gridColumn3.Caption = "ItemName";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "ItemName";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn4.Caption = "CycleTime";
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "CycleTime";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 2;
		this.gridColumn5.Caption = "TotalTime";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "TotalTime";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 3;
		this.gridColumn6.Caption = "NumberOfMch";
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "NumberOfMch";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.CheckedList;
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 4;
		this.xtraTabPage3.Controls.Add(this.grd_Users);
		this.xtraTabPage3.EditControl = null;
		this.xtraTabPage3.EditValue = "Users";
		this.xtraTabPage3.FieldName = null;
		this.xtraTabPage3.IsSearchControl = false;
		this.xtraTabPage3.Name = "xtraTabPage3";
		this.xtraTabPage3.ReadOnly = false;
		this.xtraTabPage3.SettingInVisible = false;
		this.xtraTabPage3.SettingReadonly = false;
		this.xtraTabPage3.Size = new System.Drawing.Size(746, 187);
		this.xtraTabPage3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage3.Text = "Users";
		this.grd_Users.AllowPasteExcel = false;
		this.grd_Users.ClipboardData = null;
		this.grd_Users.ContextMenuStrip = this.ctx_Users;
		this.grd_Users.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Users.Location = new System.Drawing.Point(0, 0);
		this.grd_Users.MainView = this.gv_Users;
		this.grd_Users.Name = "grd_Users";
		this.grd_Users.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_UserCode });
		this.grd_Users.Size = new System.Drawing.Size(746, 187);
		this.grd_Users.TabIndex = 1;
		this.grd_Users.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Users });
		this.gv_Users.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Users.ColumnKey = null;
		this.gv_Users.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn7, this.gridColumn8 });
		this.gv_Users.DisableFormSetting = false;
		this.gv_Users.DisplayException = false;
		this.gv_Users.EditColumn = null;
		this.gv_Users.FieldCurrency = null;
		this.gv_Users.FieldRate = null;
		this.gv_Users.FormId = "oLInes";
		this.gv_Users.GridControl = this.grd_Users;
		this.gv_Users.IgnoreFilter = false;
		this.gv_Users.IndexEditRow = -1;
		this.gv_Users.IndicatorWidth = 50;
		this.gv_Users.IsApzTable = false;
		this.gv_Users.IsInsertNewRow = false;
		this.gv_Users.IsLoadingData = false;
		this.gv_Users.IsMappingContextAction = false;
		this.gv_Users.ItemId = "OUSR";
		this.gv_Users.LstColumnAllowBatch = null;
		this.gv_Users.LstColumnNotAllowBatch = null;
		this.gv_Users.LstColumnNotEditable = null;
		this.gv_Users.LstColumNotSetting = null;
		this.gv_Users.Name = "gv_Users";
		this.gv_Users.OptionsSelection.MultiSelect = true;
		this.gv_Users.OptionsView.ColumnAutoWidth = false;
		this.gv_Users.OptionsView.ShowGroupPanel = false;
		this.gv_Users.UseBatchMod = false;
		this.gv_Users.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Items_CustomDrawRowIndicator);
		this.gridColumn7.Caption = "LineID";
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "LineID";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn8.Caption = "User_Code";
		this.gridColumn8.ColumnEdit = this.rep_UserCode;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "User_Code";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 0;
		this.gridColumn8.Width = 99;
		this.rep_UserCode.AutoHeight = false;
		this.rep_UserCode.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_UserCode.Name = "rep_UserCode";
		this.rep_UserCode.ButtonPressed += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.rep_UserCode_ButtonPressed);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(757, 494);
		base.Controls.Add(this.splitContainer1);
		base.Controls.Add(this.lbBranch);
		base.Controls.Add(this.look_Branch);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Save);
		base.FormId = "oLInes";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Lines";
		this.Text = "MRP Lines";
		this.ctx_Lines.ResumeLayout(false);
		this.ctx_Items.ResumeLayout(false);
		this.ctx_Users.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_Branch.Properties).EndInit();
		this.splitContainer1.Panel1.ResumeLayout(false);
		this.splitContainer1.Panel2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainer1).EndInit();
		this.splitContainer1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.treeList_Lines).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_LineType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Items).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Items).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ItemCode).EndInit();
		this.xtraTabPage3.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Users).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Users).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_UserCode).EndInit();
		base.ResumeLayout(false);
	}
}
