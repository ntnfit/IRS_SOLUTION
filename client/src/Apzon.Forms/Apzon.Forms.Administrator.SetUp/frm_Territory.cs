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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Tree;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;
using DevExpress.XtraTreeList.Nodes;

namespace Apzon.Forms.Administrator.SetUp;

public class frm_Territory : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool _isCancel;

	private bool _isLoading;

	private bool _isSettingParentCombo;

	private bool _isSettingOrderCombo;

	private bool _isChangingNode;

	private bool _settingTxtName;

	private DataTable _dtStruct = WinFuntions.GetTableStructure("OTER");

	private DataTable _dtOriginal;

	private IContainer components;

	public SimpleButtonEx btCancel;

	public SimpleButtonEx btnOK;

	private TreeEx tl_OTER;

	private TreeListColumnEx tl_des;

	private TreeListColumnEx tl_id;

	private TreeListColumnEx tl_parrent;

	private TreeListColumnEx tl_index;

	private TreeListColumnEx tl_inactive;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeToolStripMenuItem;

	private LabelLine labelLine5;

	private RepositoryItemButtonEdit btn_AddNew;

	private RepositoryItemLookUpEdit luk_Parent;

	private RepositoryItemCheckEdit check_inactive;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem addSubInsertToolStripMenuItem;

	private ToolStripMenuItem addSameLevelEnterToolStripMenuItem;

	private TextEdit txt_Territory;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_Territory()
	{
		InitializeComponent();
		OnLoadAsync += frm_Territory_Load;
	}

	private void frm_Territory_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && !_isCancel)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					btnOK_Click(null, null);
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

	private void tl_OTER_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
	}

	private void tl_OTER_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		try
		{
			if (Function.ToString(tl_OTER.GetRowCellValue(e.Node, tl_OTER.Columns["inactive"])) == "Y")
			{
				e.Appearance.ForeColor = Color.LightGray;
			}
			if (Function.ParseInt(tl_OTER.GetRowCellValue(e.Node, tl_OTER.Columns["parent"])) == -1)
			{
				e.Appearance.Font = new Font(e.Appearance.Font, FontStyle.Bold);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void tl_OTER_BeforeFocusNode(object sender, BeforeFocusNodeEventArgs e)
	{
	}

	private void tl_OTER_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			ChangeButtonText(sender);
			UpdateAllRowDataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task frm_Territory_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			tl_OTER.OptionsBehavior.DragNodes = true;
			Task task = LoadData();
			LoadComboBox();
			await task;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
	}

	private void tl_OTER_BeforeCollapse(object sender, BeforeCollapseEventArgs e)
	{
	}

	private async void btnOK_Click(object sender, EventArgs e)
	{
		btnOK.Enabled = false;
		btCancel.Enabled = false;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				await SaveData();
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
		btnOK.Enabled = true;
		btCancel.Enabled = true;
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (!_dtOriginal.IsNotNull())
			{
				return;
			}
			int focusedId = Function.ParseInt(tl_OTER.GetFocusedRowCellValue(tl_OTER.Columns["territryID"]));
			if (await _httpClient.Territory.ExistsLinkedObject(focusedId))
			{
				WinCommons.Error(Enums.MessageCode.CannotDeleteTeritoryReferencedByOtherObject, "");
				return;
			}
			List<int> addedRows = new List<int>();
			GetChildRows(focusedId, ref addedRows, _dtOriginal);
			bool flag = true;
			if (addedRows.Count > 0)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteTerritory, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.No)
				{
					flag = false;
				}
				else
				{
					for (int num = _dtOriginal.Rows.Count - 1; num > 0; num--)
					{
						int item = Function.ParseInt(_dtOriginal.Rows[num]["territryID"]);
						if (addedRows.Contains(item))
						{
							_dtOriginal.Rows.Remove(_dtOriginal.Rows[num]);
						}
					}
					_dtOriginal.AcceptChanges();
				}
			}
			if (flag)
			{
				DataRow dataRow = _dtOriginal.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["territryID"]) == focusedId);
				if (dataRow != null)
				{
					_dtOriginal.Rows.Remove(dataRow);
				}
				_dtOriginal.AcceptChanges();
				RefreshTreeListDataSource(_dtOriginal.Copy());
				FormMode = Enums.FormViewModeType.Update;
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
		txt_Territory_EditValueChanged(null, null);
		FormMode = Enums.FormViewModeType.View;
	}

	private async Task LoadData()
	{
		try
		{
			RefreshTreeListDataSource(await _httpClient.Territory.GetData());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveData()
	{
		_ = 1;
		try
		{
			_dtOriginal = tl_OTER.DataSourceTable;
			if (!_dtOriginal.IsNotNull())
			{
				_dtOriginal = _dtStruct.Clone();
			}
			DataTable dataTable = _dtOriginal.Copy();
			if (!dataTable.IsNotNull())
			{
				dataTable = new DataTable();
			}
			DataTable dataTable2 = dataTable.Copy();
			(from DataRow t in dataTable2.Rows
				where string.IsNullOrEmpty(Function.ToString(t["descript"]))
				select t).ToList().ForEach((DataRow r) =>
			{
				r.Delete();
			});
			dataTable2.AcceptChanges();
			string text = await _httpClient.Territory.SaveData(dataTable2);
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.OperationCompletedSuccess, "");
				await LoadData();
				FormMode = Enums.FormViewModeType.View;
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void UpdateAllRowDataTable()
	{
		try
		{
			DataTable dataTable = tl_OTER.DataSource as DataTable;
			if (dataTable == null || dataTable.Rows.Count == 0)
			{
				dataTable = _dtStruct.Clone();
			}
			if (!_dtOriginal.IsNotNull())
			{
				_dtOriginal = _dtStruct.Clone();
			}
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				int id = Function.ParseInt(dataTable.Rows[i]["territryID"]);
				DataRow dataRow = _dtOriginal.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["territryID"]) == id);
				if (dataRow == null)
				{
					_dtOriginal.ImportRow(dataTable.Rows[i]);
					continue;
				}
				for (int num = 0; num < dataTable.Columns.Count; num++)
				{
					dataRow[dataTable.Columns[num].ColumnName] = dataTable.Rows[i][dataTable.Columns[num]];
				}
			}
			for (int num2 = _dtOriginal.Rows.Count - 1; num2 >= 0; num2--)
			{
				int id2 = Function.ParseInt(_dtOriginal.Rows[num2]["territryID"]);
				if (Function.ToString(_dtOriginal.Rows[num2]["inactive"]) == "N" && !dataTable.AsEnumerable().Any((DataRow r) => Function.ParseInt(r["territryID"]) == id2))
				{
					_dtOriginal.Rows.Remove(_dtOriginal.Rows[num2]);
				}
			}
			_dtOriginal.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void NewTerritory(int fatherId = -1, string id = "")
	{
		try
		{
			DataTable dataSourceTable = tl_OTER.DataSourceTable;
			if (dataSourceTable == null)
			{
				return;
			}
			tl_OTER.FindNodeByKeyID(fatherId);
			DataRow[] array = (from t in dataSourceTable.AsEnumerable()
				where Function.ParseInt(t["parent"]) == fatherId
				select t).ToArray();
			int num = Function.ParseInt(dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["territryID"])));
			if (array.Length != 0)
			{
				int num2 = Function.ParseInt(array.CopyToDataTable().AsEnumerable().Max((DataRow t) => Function.ParseInt(t["lindex"])));
				DataRow dataRow = (from t in dataSourceTable.AsEnumerable()
					where Function.ParseInt(t["territryID"]) == fatherId
					select t).ToArray().ToList().FirstOrDefault((DataRow t) => Function.ToString(t["descript"]) == "");
				if (dataRow != null)
				{
					Function.ToString(dataRow["territryID"]);
					return;
				}
				DataRow dataRow2 = dataSourceTable.NewRow();
				dataRow2["territryID"] = num + 1;
				dataRow2["parent"] = fatherId;
				dataRow2["lindex"] = num2 + 1;
				dataRow2["inactive"] = "N";
				dataSourceTable.Rows.Add(dataRow2);
				dataSourceTable.AcceptChanges();
			}
			else
			{
				DataRow dataRow3 = dataSourceTable.NewRow();
				dataRow3["territryID"] = num + 1;
				dataRow3["parent"] = fatherId;
				dataRow3["lindex"] = 0;
				dataRow3["inactive"] = "N";
				dataSourceTable.Rows.Add(dataRow3);
				dataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_AddNew_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			int fatherId = Function.ParseInt(tl_OTER.FocusedNode.GetValue("territryID"));
			NewTerritory(fatherId);
			tl_OTER.FocusedNode.Expand();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tl_OTER_KeyDown(object sender, KeyEventArgs e)
	{
		KeysEvent(sender, e);
	}

	private void check_inactive_Click(object sender, EventArgs e)
	{
		try
		{
			if (!_dtOriginal.IsNotNull())
			{
				_dtOriginal = _dtStruct.Clone();
			}
			if (!_dtOriginal.IsNotNull())
			{
				return;
			}
			int focusedId = Function.ParseInt(tl_OTER.GetFocusedRowCellValue(tl_OTER.Columns["territryID"]));
			int parentID = Function.ParseInt(tl_OTER.GetFocusedRowCellValue(tl_OTER.Columns["parent"]));
			DataRow dataRow = _dtOriginal.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["territryID"]) == focusedId);
			string text = Function.ToString(tl_OTER.GetFocusedRowCellValue(tl_OTER.Columns["inactive"]));
			if (string.Equals(text, "N"))
			{
				text = "Y";
			}
			else if (string.Equals(text, "Y"))
			{
				text = "N";
			}
			List<int> addedRows = new List<int>();
			GetChildRows(focusedId, ref addedRows, _dtOriginal);
			if (addedRows.Count > 0)
			{
				bool flag = false;
				for (int num = 0; num < _dtOriginal.Rows.Count; num++)
				{
					int id = Function.ParseInt(_dtOriginal.Rows[num]["territryID"]);
					if (addedRows.Any((int r) => r == id) && string.Compare(Function.ToString(_dtOriginal.Rows[num]["inactive"]), text, ignoreCase: true) != 0)
					{
						flag = true;
					}
				}
				if (flag)
				{
					if (text == "N")
					{
						DataRow dataRow2 = _dtOriginal.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["territryID"]) == parentID);
						if (dataRow2 != null && Function.ToString(dataRow2["inactive"]) == "Y")
						{
							WinCommons.Error(Enums.MessageCode.CannotSetTerritoryWhoseParentTerritoryIsInactiveToActive, "");
							return;
						}
						dataRow["inactive"] = text;
					}
					else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSettingTerritory, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						DataRow dataRow3 = _dtOriginal.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["territryID"]) == focusedId);
						if (dataRow3 != null)
						{
							dataRow3["inactive"] = text;
						}
						for (int num2 = 0; num2 < _dtOriginal.Rows.Count; num2++)
						{
							int id2 = Function.ParseInt(_dtOriginal.Rows[num2]["territryID"]);
							if (addedRows.Any((int r) => r == id2))
							{
								_dtOriginal.Rows[num2]["inactive"] = text;
							}
						}
					}
				}
				else if (text == "N")
				{
					DataRow dataRow4 = _dtOriginal.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["territryID"]) == parentID);
					if (dataRow4 != null && Function.ToString(dataRow4["inactive"]) == "Y")
					{
						WinCommons.Error(Enums.MessageCode.CannotSetTerritoryWhoseParentTerritoryIsInactiveToActive, "");
						return;
					}
					dataRow["inactive"] = text;
				}
				else
				{
					dataRow["inactive"] = text;
				}
			}
			else
			{
				dataRow["inactive"] = text;
			}
			_dtOriginal.AcceptChanges();
			RefreshTreeListDataSource(_dtOriginal);
			ChangeButtonText(sender);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataSourceTable = tl_OTER.DataSourceTable;
			if (dataSourceTable != null)
			{
				int num = Function.ParseInt(dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["territryID"])));
				Function.ParseInt(dataSourceTable.AsEnumerable().Max((DataRow t) => Function.ParseInt(t["lindex"])));
				DataRow dataRow = dataSourceTable.NewRow();
				dataRow["territryID"] = num + 1;
				dataRow["parent"] = -1;
				dataRow["lindex"] = 0;
				dataRow["inactive"] = "N";
				dataSourceTable.Rows.Add(dataRow);
				dataSourceTable.AcceptChanges();
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.InfoCannotLoadData, "");
			}
		}
		catch (Exception)
		{
			throw;
		}
	}

	private void addSubInsertToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int fatherId = Function.ParseInt(tl_OTER.FocusedNode.GetValue("territryID"));
			NewTerritory(fatherId);
			tl_OTER.FocusedNode.Expand();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addSameLevelEnterToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			int fatherId = Function.ParseInt(tl_OTER.FocusedNode.GetValue("parent"));
			NewTerritory(fatherId);
			tl_OTER.FocusedNode.Expand();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tl_OTER_DragOver(object sender, DragEventArgs e)
	{
		try
		{
			DXDragEventArgs dXDragEventArgs = tl_OTER.GetDXDragEventArgs(e);
			if (dXDragEventArgs.Node == null)
			{
				if (dXDragEventArgs.HitInfo.HitInfoType == HitInfoType.Empty || dXDragEventArgs.TargetNode != null)
				{
					dXDragEventArgs.Effect = DragDropEffects.Copy;
				}
				else
				{
					dXDragEventArgs.Effect = DragDropEffects.None;
				}
			}
			SetDragCursor(dXDragEventArgs.Effect);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tl_OTER_DragDrop(object sender, DragEventArgs e)
	{
		try
		{
			DXDragEventArgs dXDragEventArgs = tl_OTER.GetDXDragEventArgs(e);
			TreeListNode targetNode = dXDragEventArgs.TargetNode;
			if (dXDragEventArgs.HitInfo.HitInfoType == HitInfoType.Empty || targetNode != null)
			{
				tl_OTER.PointToClient(new Point(e.X, e.Y));
				TreeListNode treeListNode = e.Data.GetData(typeof(TreeListNode)) as TreeListNode;
				if (targetNode != null && treeListNode != null)
				{
					treeListNode.SetValue("parent", targetNode.GetValue("parent"));
					tl_OTER.DataSourceTable.AcceptChanges();
					int num = Function.ParseInt(targetNode.GetValue("lindex"));
					treeListNode.SetValue("lindex", num);
					RefreshIndex(treeListNode);
					tl_OTER.DataSourceTable.AcceptChanges();
				}
				tl_OTER.MakeNodeVisible(targetNode);
				TreeListNode parentNode = targetNode.ParentNode;
				if (parentNode != null && (e.KeyState & 4) != 0)
				{
					int index = -1;
					if (parentNode.ParentNode != null)
					{
						index = parentNode.ParentNode.Nodes.IndexOf(parentNode);
					}
					tl_OTER.MoveNode(targetNode, parentNode.ParentNode);
					tl_OTER.SetNodeIndex(targetNode, index);
				}
			}
			SetDefaultCursor();
			ChangeButtonText(sender);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tl_OTER_DragLeave(object sender, EventArgs e)
	{
		SetDefaultCursor();
	}

	private void tl_OTER_BeforeDragNode(object sender, BeforeDragNodeEventArgs e)
	{
		try
		{
			if (e.Nodes.Count <= 1)
			{
				return;
			}
			int num = 0;
			int num2 = num + 1;
			List<TreeListNode> list = new List<TreeListNode>();
			while (num != e.Nodes.Count - 1)
			{
				TreeListNode treeListNode = HasAsParent(e.Nodes[num], e.Nodes[num2]);
				if (treeListNode != null)
				{
					list.Add(treeListNode);
				}
				if (num2 == e.Nodes.Count - 1)
				{
					num++;
					num2 = num + 1;
				}
				else
				{
					num2++;
				}
			}
			if (list.Count == 0)
			{
				return;
			}
			foreach (TreeListNode item in list)
			{
				e.Nodes.Remove(item);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private TreeListNode HasAsParent(TreeListNode node1, TreeListNode node2)
	{
		try
		{
			if (node1.Level > node2.Level)
			{
				return node1.HasAsParent(node2) ? node1 : null;
			}
			if (node1.Level < node2.Level)
			{
				return node2.HasAsParent(node1) ? node2 : null;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
		return null;
	}

	private void tl_OTER_BeforeDropNode(object sender, BeforeDropNodeEventArgs e)
	{
		try
		{
			if (!e.IsCopy)
			{
				TreeListNodes nodes = ((e.DestinationNode == null) ? ((TreeList)sender).Nodes : e.DestinationNode.Nodes);
				e.Cancel = AllowDropNode(e.SourceNode, nodes);
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private bool AllowDropNode(TreeListNode sourceNode, TreeListNodes nodes)
	{
		try
		{
			foreach (TreeListNode node in nodes)
			{
				object value = sourceNode.GetValue("territryID");
				object value2 = node.GetValue("territryID");
				if (object.Equals(value, value2))
				{
					return true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return false;
	}

	private void SetDragCursor(DragDropEffects e)
	{
		try
		{
			if (e == DragDropEffects.Move)
			{
				Stream manifestResourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Apzon.Forms.Resources.move.ico");
				if (manifestResourceStream != null)
				{
					Cursor = new Cursor(manifestResourceStream);
				}
			}
			if (e == DragDropEffects.Copy)
			{
				Stream manifestResourceStream2 = Assembly.GetExecutingAssembly().GetManifestResourceStream("Apzon.Forms.Resources.copy.ico");
				if (manifestResourceStream2 != null)
				{
					Cursor = new Cursor(manifestResourceStream2);
				}
			}
			if (e == DragDropEffects.None)
			{
				Cursor = Cursors.No;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void SetDefaultCursor()
	{
		Cursor = Cursors.Default;
	}

	private DragDropEffects GetDragDropEffect(TreeList tl, TreeListNode dragNode)
	{
		Point pt = tl.PointToClient(Control.MousePosition);
		TreeListNode node = tl.CalcHitInfo(pt).Node;
		if (dragNode != null && node != null && dragNode != node && dragNode.ParentNode == node.ParentNode)
		{
			return DragDropEffects.Move;
		}
		return DragDropEffects.None;
	}

	private void tl_OTER_EditorKeyDown(object sender, KeyEventArgs e)
	{
		KeysEvent(sender, e);
	}

	private void tl_OTER_CustomColumnSort(object sender, CustomColumnSortEventArgs e)
	{
		try
		{
			if (e.Column.FieldName != "lindex")
			{
				return;
			}
			try
			{
				string text = Function.ToString(e.NodeValue1);
				string text2 = Function.ToString(e.NodeValue2);
				e.Result = Function.CompareString(text, text2);
			}
			catch
			{
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tl_OTER_CalcNodeDragImageIndex(object sender, CalcNodeDragImageIndexEventArgs e)
	{
		try
		{
			TreeList treeList = sender as TreeList;
			if (GetDragDropEffect(treeList, treeList.FocusedNode) == DragDropEffects.None)
			{
				e.ImageIndex = -1;
			}
			else
			{
				e.ImageIndex = 1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_Territory_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			string id = Function.ToString(txt_Territory.Text);
			DataTable dataTable = await _httpClient.Territory.Search(id);
			if (dataTable != null && dataTable.Rows.Count > 0)
			{
				tl_OTER.DataSource = dataTable;
				tl_OTER.RefreshDataSource();
				tl_OTER.Refresh();
				tl_OTER.ExpandAll();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void KeysEvent(object sender, KeyEventArgs e)
	{
		try
		{
			DataTable dataSourceTable = tl_OTER.DataSourceTable;
			if (!(sender is TreeList))
			{
				return;
			}
			if (e.KeyCode == Keys.Return)
			{
				int lineNum = Function.ParseInt(tl_OTER.FocusedNode.GetValue("parent"));
				string id = Function.ToString(tl_OTER.FocusedNode.GetValue("territryID"));
				NewTerritory(lineNum, id);
				tl_OTER.FocusedNode.Expand();
				DataRow dataRow = (from t in dataSourceTable.AsEnumerable()
					where Function.ParseInt(t["parent"]) == lineNum
					select t).ToArray().ToList().FirstOrDefault((DataRow t) => Function.ToString(t["descript"]) == "");
				if (dataRow != null)
				{
					string cellValue = Function.ToString(dataRow["territryID"]);
					tl_OTER.FocusedNode = tl_OTER.FindNodeByFieldValue("territryID", cellValue);
				}
			}
			if (e.KeyCode == Keys.Insert)
			{
				int lineNum2 = Function.ParseInt(tl_OTER.FocusedNode.GetValue("territryID"));
				NewTerritory(lineNum2, Function.ToString(lineNum2));
				tl_OTER.FocusedNode.Expand();
				DataRow dataRow2 = (from t in dataSourceTable.AsEnumerable()
					where Function.ParseInt(t["parent"]) == lineNum2
					select t).ToArray().ToList().FirstOrDefault((DataRow t) => Function.ToString(t["descript"]) == "");
				if (dataRow2 != null)
				{
					string cellValue2 = Function.ToString(dataRow2["territryID"]);
					tl_OTER.FocusedNode = tl_OTER.FindNodeByFieldValue("territryID", cellValue2);
				}
			}
			if (e.KeyCode == Keys.Delete && e.Modifiers == Keys.Shift)
			{
				if (dataSourceTable == null || dataSourceTable.Columns.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				}
				TreeListNode focusedNode = tl_OTER.FocusedNode;
				if (focusedNode.ParentNode != null)
				{
					focusedNode.ParentNode.Nodes.Remove(focusedNode);
				}
				else
				{
					tl_OTER.Nodes.Remove(focusedNode);
				}
				dataSourceTable.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void RefreshIndex(TreeListNode node)
	{
		try
		{
			if (node == null)
			{
				return;
			}
			DataTable dataSourceTable = tl_OTER.DataSourceTable;
			int fatherId = Function.ParseInt(node.GetValue("parent"));
			int lindex = Function.ParseInt(node.GetValue("lindex"));
			_ = node.Level;
			int linenum = Function.ParseInt(node.GetValue("territryID"));
			List<DataRow> list = (from t in dataSourceTable.AsEnumerable()
				where Function.ParseInt(t["parent"]) == fatherId && Function.ParseInt(t["territryID"]) != linenum && Function.ParseInt(t["lindex"]) >= lindex
				select t).ToList();
			if (list.Count > 0)
			{
				for (int num = 0; num < list.Count; num++)
				{
					list[num]["lindex"] = lindex + num + 1;
					Function.ParseInt(list[num]["territryID"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void ChangeButtonText(object sender)
	{
		try
		{
			if (!_isLoading && !_isChangingNode && !_isSettingOrderCombo && !_isSettingParentCombo && !_settingTxtName)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void RefreshTreeListDataSource(DataTable dt)
	{
		try
		{
			tl_OTER.DataSource = dt;
			tl_OTER.RefreshDataSource();
			tl_OTER.Refresh();
			tl_OTER.ExpandAll();
			UpdateAllRowDataTable();
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
			luk_Parent.Columns.Clear();
			luk_Parent.Columns.Add(new LookUpColumnInfo("descript", ""));
			luk_Parent.ValueMember = "territryID";
			luk_Parent.DisplayMember = "descript";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GetChildRows(int parentID, ref List<int> addedRows, DataTable sourceDT)
	{
		try
		{
			EnumerableRowCollection<DataRow> source = from r in sourceDT.AsEnumerable()
				where Function.ParseInt(r["parent"]) == parentID
				select r;
			if (!source.Any())
			{
				return;
			}
			DataRow[] array = source.ToArray();
			for (int num = 0; num < array.Length; num++)
			{
				int id = Function.ParseInt(array[num]["territryID"]);
				if (!addedRows.Any((int r) => r == id))
				{
					addedRows.Add(id);
				}
				GetChildRows(id, ref addedRows, sourceDT);
			}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SetUp.frm_Territory));
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tl_OTER = new Apzon.Controls.Tree.TreeEx();
		this.tl_des = new Apzon.Controls.Tree.TreeListColumnEx();
		this.btn_AddNew = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.tl_id = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_parrent = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_index = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tl_inactive = new Apzon.Controls.Tree.TreeListColumnEx();
		this.check_inactive = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addSubInsertToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.addSameLevelEnterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.luk_Parent = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Territory = new DevExpress.XtraEditors.TextEdit();
		((System.ComponentModel.ISupportInitialize)this.tl_OTER).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.btn_AddNew).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.check_inactive).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.luk_Parent).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Territory.Properties).BeginInit();
		base.SuspendLayout();
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(80, 357);
		this.btCancel.Name = "btCancel";
		this.btCancel.Size = new System.Drawing.Size(75, 20);
		this.btCancel.TabIndex = 14;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(2, 357);
		this.btnOK.Name = "btnOK";
		this.btnOK.Size = new System.Drawing.Size(75, 20);
		this.btnOK.TabIndex = 13;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.tl_OTER.AllowDrop = true;
		this.tl_OTER.AllowPasteExcel = true;
		this.tl_OTER.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tl_OTER.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tl_OTER.ColumnKey = null;
		this.tl_OTER.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[5] { this.tl_des, this.tl_id, this.tl_parrent, this.tl_index, this.tl_inactive });
		this.tl_OTER.ContextMenuStrip = this.contextMenuStrip1;
		this.tl_OTER.Cursor = System.Windows.Forms.Cursors.Default;
		this.tl_OTER.DisableFormSetting = false;
		this.tl_OTER.EditColumn = null;
		this.tl_OTER.FieldCurrency = null;
		this.tl_OTER.FieldRate = null;
		this.tl_OTER.FormId = "oTerritories";
		this.tl_OTER.IndexEditRow = 0;
		this.tl_OTER.IndicatorWidth = 40;
		this.tl_OTER.IsApzTable = false;
		this.tl_OTER.IsLoadingData = false;
		this.tl_OTER.ItemId = "OTER";
		this.tl_OTER.KeyFieldName = "territryID";
		this.tl_OTER.Location = new System.Drawing.Point(2, 35);
		this.tl_OTER.LstColumnNotEditable = null;
		this.tl_OTER.LstColumNotSetting = null;
		this.tl_OTER.Name = "tl_OTER";
		this.tl_OTER.OptionsBehavior.AutoChangeParent = false;
		this.tl_OTER.OptionsBehavior.CloseEditorOnLostFocus = false;
		this.tl_OTER.OptionsBehavior.PopulateServiceColumns = true;
		this.tl_OTER.OptionsBehavior.ShowToolTips = false;
		this.tl_OTER.OptionsBehavior.SmartMouseHover = false;
		this.tl_OTER.OptionsDragAndDrop.CanCloneNodesOnDrop = true;
		this.tl_OTER.OptionsDragAndDrop.DragNodesMode = DevExpress.XtraTreeList.DragNodesMode.Single;
		this.tl_OTER.ParentFieldName = "parent";
		this.tl_OTER.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.btn_AddNew, this.luk_Parent, this.check_inactive });
		this.tl_OTER.Size = new System.Drawing.Size(542, 319);
		this.tl_OTER.TabIndex = 20;
		this.tl_OTER.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.tl_OTER_NodeCellStyle);
		this.tl_OTER.BeforeCollapse += new DevExpress.XtraTreeList.BeforeCollapseEventHandler(this.tl_OTER_BeforeCollapse);
		this.tl_OTER.BeforeDragNode += new DevExpress.XtraTreeList.BeforeDragNodeEventHandler(this.tl_OTER_BeforeDragNode);
		this.tl_OTER.BeforeFocusNode += new DevExpress.XtraTreeList.BeforeFocusNodeEventHandler(this.tl_OTER_BeforeFocusNode);
		this.tl_OTER.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tl_OTER_FocusedNodeChanged);
		this.tl_OTER.CustomColumnSort += new DevExpress.XtraTreeList.CustomColumnSortEventHandler(this.tl_OTER_CustomColumnSort);
		this.tl_OTER.BeforeDropNode += new DevExpress.XtraTreeList.BeforeDropNodeEventHandler(this.tl_OTER_BeforeDropNode);
		this.tl_OTER.CalcNodeDragImageIndex += new DevExpress.XtraTreeList.CalcNodeDragImageIndexEventHandler(this.tl_OTER_CalcNodeDragImageIndex);
		this.tl_OTER.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tl_OTER_CellValueChanged);
		this.tl_OTER.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.tl_OTER_EditorKeyDown);
		this.tl_OTER.DragDrop += new System.Windows.Forms.DragEventHandler(this.tl_OTER_DragDrop);
		this.tl_OTER.DragOver += new System.Windows.Forms.DragEventHandler(this.tl_OTER_DragOver);
		this.tl_OTER.DragLeave += new System.EventHandler(this.tl_OTER_DragLeave);
		this.tl_OTER.KeyDown += new System.Windows.Forms.KeyEventHandler(this.tl_OTER_KeyDown);
		this.tl_des.Caption = "Territory";
		this.tl_des.ColumnEdit = this.btn_AddNew;
		this.tl_des.CurrencyDateField = null;
		this.tl_des.CurrencyField = null;
		this.tl_des.CurrencyFixed = null;
		this.tl_des.CurrencyFixedRate = 0.0;
		this.tl_des.CurrencyRateField = null;
		this.tl_des.FieldName = "descript";
		this.tl_des.Name = "tl_des";
		this.tl_des.OptionsColumn.AllowSort = false;
		this.tl_des.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowAlways;
		this.tl_des.Visible = true;
		this.tl_des.VisibleIndex = 0;
		this.tl_des.Width = 501;
		this.btn_AddNew.AutoHeight = false;
		this.btn_AddNew.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Plus)
		});
		this.btn_AddNew.Name = "btn_AddNew";
		this.btn_AddNew.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.btn_AddNew_ButtonClick);
		this.tl_id.Caption = "ID";
		this.tl_id.CurrencyDateField = null;
		this.tl_id.CurrencyField = null;
		this.tl_id.CurrencyFixed = null;
		this.tl_id.CurrencyFixedRate = 0.0;
		this.tl_id.CurrencyRateField = null;
		this.tl_id.FieldName = "territryID";
		this.tl_id.Name = "tl_id";
		this.tl_parrent.Caption = "Parent";
		this.tl_parrent.CurrencyDateField = null;
		this.tl_parrent.CurrencyField = null;
		this.tl_parrent.CurrencyFixed = null;
		this.tl_parrent.CurrencyFixedRate = 0.0;
		this.tl_parrent.CurrencyRateField = null;
		this.tl_parrent.FieldName = "parent";
		this.tl_parrent.Name = "tl_parrent";
		this.tl_parrent.OptionsColumn.ReadOnly = true;
		this.tl_parrent.Width = 501;
		this.tl_index.Caption = "lindex";
		this.tl_index.CurrencyDateField = null;
		this.tl_index.CurrencyField = null;
		this.tl_index.CurrencyFixed = null;
		this.tl_index.CurrencyFixedRate = 0.0;
		this.tl_index.CurrencyRateField = null;
		this.tl_index.FieldName = "lindex";
		this.tl_index.FieldNameSort = "lindex";
		this.tl_index.Name = "tl_index";
		this.tl_index.OptionsColumn.ReadOnly = true;
		this.tl_index.SortMode = DevExpress.XtraGrid.ColumnSortMode.Value;
		this.tl_index.SortOrder = System.Windows.Forms.SortOrder.Ascending;
		this.tl_inactive.Caption = "InActive";
		this.tl_inactive.ColumnEdit = this.check_inactive;
		this.tl_inactive.CurrencyDateField = null;
		this.tl_inactive.CurrencyField = null;
		this.tl_inactive.CurrencyFixed = null;
		this.tl_inactive.CurrencyFixedRate = 0.0;
		this.tl_inactive.CurrencyRateField = null;
		this.tl_inactive.FieldName = "inactive";
		this.tl_inactive.Name = "tl_inactive";
		this.tl_inactive.Visible = true;
		this.tl_inactive.VisibleIndex = 1;
		this.tl_inactive.Width = 500;
		this.check_inactive.AutoHeight = false;
		this.check_inactive.Name = "check_inactive";
		this.check_inactive.ValueChecked = "Y";
		this.check_inactive.ValueGrayed = "N";
		this.check_inactive.ValueUnchecked = "N";
		this.check_inactive.Click += new System.EventHandler(this.check_inactive_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[4] { this.addNewToolStripMenuItem, this.addSubInsertToolStripMenuItem, this.addSameLevelEnterToolStripMenuItem, this.removeToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(197, 92);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.addSubInsertToolStripMenuItem.Name = "addSubInsertToolStripMenuItem";
		this.addSubInsertToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
		this.addSubInsertToolStripMenuItem.Text = "Add Sub (Insert)";
		this.addSubInsertToolStripMenuItem.Click += new System.EventHandler(this.addSubInsertToolStripMenuItem_Click);
		this.addSameLevelEnterToolStripMenuItem.Name = "addSameLevelEnterToolStripMenuItem";
		this.addSameLevelEnterToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
		this.addSameLevelEnterToolStripMenuItem.Text = "Add Same Level (Enter)";
		this.addSameLevelEnterToolStripMenuItem.Click += new System.EventHandler(this.addSameLevelEnterToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
		this.removeToolStripMenuItem.Text = "Remove (Shift+Delete)";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.luk_Parent.AutoHeight = false;
		this.luk_Parent.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Parent.Name = "luk_Parent";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(12, 12);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(91, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 21;
		this.labelLine5.Utext = " Territory";
		this.txt_Territory.Location = new System.Drawing.Point(93, 9);
		this.txt_Territory.Name = "txt_Territory";
		this.txt_Territory.Size = new System.Drawing.Size(100, 20);
		this.txt_Territory.TabIndex = 23;
		this.txt_Territory.EditValueChanged += new System.EventHandler(this.txt_Territory_EditValueChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(546, 381);
		base.Controls.Add(this.txt_Territory);
		base.Controls.Add(this.labelLine5);
		base.Controls.Add(this.tl_OTER);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.FormId = "oTerritories";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Territory";
		base.ShowInTaskbar = false;
		this.Text = "Territories - Setup";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Territory_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.tl_OTER).EndInit();
		((System.ComponentModel.ISupportInitialize)this.btn_AddNew).EndInit();
		((System.ComponentModel.ISupportInitialize)this.check_inactive).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.luk_Parent).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Territory.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
