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
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Columns;

namespace Apzon.Forms.Dashboard;

public class frm_DashboardTab : XtraFormEx
{
	public int catId;

	public static int prevTabId;

	private bool _isCancel;

	private bool _forceClose;

	public new string FormId;

	public List<Tuple<GridViewEx, string>> FormItem = new List<Tuple<GridViewEx, string>>();

	private IContainer components;

	private GridControlEx grd_General;

	private GridViewEx gvGeneral;

	private GridColumnEx colName;

	private GridColumnEx colDescription;

	private GridColumnEx colObjectType;

	private GridColumnEx colCategory;

	private GridColumnEx colTarget;

	private GridColumnEx colValid;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_SubCtrl;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem chooseFromListToolStripMenuItem;

	private TreeList tlController;

	private TreeListColumnEx tlColCode;

	private TreeListColumnEx tlColDescription;

	private TreeListColumnEx tlColRemarks;

	private TreeListColumnEx tlColQryText;

	private TreeListColumnEx tlColQryType;

	private TreeListColumnEx tlColColumnKey;

	private TreeListColumnEx tlColObjKey;

	private RepositoryItemLookUpEdit rep_Categories;

	private RepositoryItemLookUpEdit rep_QryType;

	private RepositoryItemCheckedComboBoxEdit rep_CbTarget;

	private RepositoryItemCheckEdit rep_Valid;

	private RepositoryItemTreeListLookUpEdit rep_CodeTree;

	private TreeListColumnEx tlColTabId;

	private TreeListColumnEx tlColControllerId;

	private GridColumnEx colTabId;

	private RepositoryItemLookUpEdit repositoryItemLookUpEdit1;

	private TreeListColumnEx treeListColumn1;

	private ToolStripMenuItem addNewControllerCodeToolStripMenuItem;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private ContextMenuStrip contextMenuStrip2;

	private ToolStripMenuItem deleteRowToolStripMenuItem1;

	private RepositoryItemLookUpEdit rep_ObjType;

	private RepositoryItemLookUpEdit rep_ObjKey;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private TextEditEx txtSearch;

	private SimpleButtonEx btn_Search;

	private SplitContainerControl splitContainerControl1;

	private ToolStripMenuItem expandAllToolStripMenuItem;

	private ToolStripMenuItem collapseAllToolStripMenuItem;

	private ToolStripSeparator toolStripSeparator1;

	private ToolStripSeparator toolStripSeparator2;

	private LabelLine lblTabName;

	private RepositoryItemMemoExEdit rep_QryText;

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
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_DashboardTab()
	{
		InitializeComponent();
		rep_Categories.Columns.Add(new LookUpColumnInfo(rep_Categories.DisplayMember));
		rep_ObjType.Columns.Add(new LookUpColumnInfo(rep_ObjType.DisplayMember));
		rep_ObjKey.Columns.Add(new LookUpColumnInfo(rep_ObjKey.DisplayMember));
		rep_QryType.Columns.Add(new LookUpColumnInfo(rep_QryType.DisplayMember));
		OnLoadAsync += frm_DashboardTab_Load;
	}

	private void LoadAllContextStrip1()
	{
		contextMenuStrip1.Items.Add(chooseFromListToolStripMenuItem);
		contextMenuStrip1.Items.Add(toolStripSeparator1);
		contextMenuStrip1.Items.Add(addNewControllerCodeToolStripMenuItem);
		contextMenuStrip1.Items.Add(deleteRowToolStripMenuItem);
		contextMenuStrip1.Items.Add(toolStripSeparator2);
		contextMenuStrip1.Items.Add(expandAllToolStripMenuItem);
		contextMenuStrip1.Items.Add(collapseAllToolStripMenuItem);
		contextMenuStrip1.Refresh();
	}

	private void LoadAllContextStrip2()
	{
		contextMenuStrip2.Items.Clear();
		contextMenuStrip2.Items.Add(addNewToolStripMenuItem);
		contextMenuStrip2.Items.Add(deleteRowToolStripMenuItem1);
		contextMenuStrip2.Refresh();
	}

	private DataTable CreateTabDataTable()
	{
		return new DataTable
		{
			Columns = 
			{
				{
					"CatId",
					typeof(int)
				},
				{
					"TabId",
					typeof(int)
				},
				{
					"TabName",
					typeof(string)
				},
				{
					"TabDescription",
					typeof(string)
				},
				{
					"TabValid",
					typeof(bool)
				},
				{
					"TargetType",
					typeof(string)
				},
				{
					"ObjType",
					typeof(string)
				}
			}
		};
	}

	private DataTable CreateControllerDataTable()
	{
		return new DataTable
		{
			Columns = 
			{
				{
					"ControllerId",
					typeof(int)
				},
				{
					"TabId",
					typeof(int)
				},
				{
					"ControllerCode",
					typeof(string)
				},
				{
					"ControllerDescription",
					typeof(string)
				},
				{
					"Remarks",
					typeof(string)
				},
				{
					"QueryText",
					typeof(string)
				},
				{
					"QueryType",
					typeof(char)
				},
				{
					"ColumnKey",
					typeof(string)
				},
				{
					"ObjectKey",
					typeof(string)
				},
				{
					"FatherId",
					typeof(int)
				}
			}
		};
	}

	public async Task REFRESH()
	{
		_ = 3;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.No)
				{
					await NewTab();
					await AddNewfatherController();
					LoadAllContextStrip1();
					contextMenuStrip2.Items.Clear();
					contextMenuStrip2.Refresh();
				}
				else
				{
					btn_OK_Click(null, null);
				}
			}
			else
			{
				await NewTab();
				await AddNewfatherController();
				LoadAllContextStrip1();
				contextMenuStrip2.Items.Clear();
				contextMenuStrip2.Refresh();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	public async Task REFRESH_DATA()
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					await LoadData();
				}
			}
			else
			{
				await LoadData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void KeyDownPaste(object sender, KeyEventArgs e)
	{
		PasteFromExcel(e, grd_General, gvGeneral);
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
					int num2 = ((gv.RowCount > dataFromExcel.Rows.Count) ? dataFromExcel.Rows.Count : gv.RowCount);
					int num3 = ((gv.Columns.Count > dataFromExcel.Columns.Count) ? dataFromExcel.Columns.Count : gv.Columns.Count);
					gv.BeginUpdate();
					for (int i = 0; i < num2; i++)
					{
						int colIndex = focusedColumn.VisibleIndex;
						for (int j = 0; j < num3; j++)
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

	private async Task LoadData()
	{
		try
		{
			FormMode = Enums.FormViewModeType.View;
			await LoadGridTabs();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadTarget()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("TargetString", typeof(string));
			dataTable.Columns.Add("TargetType", typeof(string));
			foreach (object value in Enum.GetValues(typeof(ApzObjectTypes)))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["TargetString"] = Function.ToString(value);
				dataRow["TargetType"] = Function.ToString((int)value);
				dataTable.Rows.Add(dataRow);
			}
			rep_CbTarget.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadLookUp()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("QueryType", typeof(char));
			dataTable.Columns.Add("DisplayType", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["QueryType"] = 'S';
			dataRow["DisplayType"] = "Store procedure";
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2["QueryType"] = 'T';
			dataRow2["DisplayType"] = "Text";
			DataRow dataRow3 = dataTable.NewRow();
			dataRow3["QueryType"] = 'D';
			dataRow3["DisplayType"] = "Table Direct";
			dataTable.Rows.Add(dataRow);
			dataTable.Rows.Add(dataRow2);
			dataTable.Rows.Add(dataRow3);
			rep_QryType.DataSource = dataTable;
			using (HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashboardCategories"))
			{
				DataTable dataTable2 = await _help.GetDataTableAsync("GetData");
				DataTable dataTable3 = new DataTable();
				dataTable3.Columns.Add("CatId", typeof(int));
				dataTable3.Columns.Add("CatName", typeof(string));
				for (int i = 0; i < dataTable2.Rows.Count; i++)
				{
					DataRow dataRow4 = dataTable3.NewRow();
					dataRow4["CatId"] = Function.ParseInt(dataTable2.Rows[i]["CatId"]);
					dataRow4["CatName"] = Function.ToString(dataTable2.Rows[i]["Name"]);
					dataTable3.Rows.Add(dataRow4);
				}
				rep_Categories.DataSource = dataTable3;
			}
			DataTable dataTable4 = new DataTable();
			dataTable4.Columns.Add("ObjString", typeof(string));
			dataTable4.Columns.Add("ObjType", typeof(string));
			foreach (object value in Enum.GetValues(typeof(ApzObjectTypes)))
			{
				DataRow dataRow5 = dataTable4.NewRow();
				dataRow5["ObjString"] = Function.ToString(value);
				dataRow5["ObjType"] = Function.ToString((int)value);
				dataTable4.Rows.Add(dataRow5);
			}
			rep_ObjType.DataSource = dataTable4;
			DataTable dataTable5 = new DataTable();
			dataTable5.Columns.Add("ObjectKey", typeof(string));
			foreach (object value2 in Enum.GetValues(typeof(ApzObjectTypes)))
			{
				DataRow dataRow6 = dataTable5.NewRow();
				dataRow6["ObjectKey"] = value2;
				dataTable5.Rows.Add(dataRow6);
			}
			rep_ObjKey.DataSource = dataTable5;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadTreeList()
	{
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardTabs");
			DataTable dataTable = await _help.GetDataTableAsync("GetControllersByTab", new
			{
				tabId = GetFocusedTabId()
			});
			tlController.DataSource = (dataTable.IsNotNull() ? dataTable : null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadGridTabs()
	{
		_ = 3;
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardTabs");
			if (catId == 0)
			{
				DataTable dataTable = await _help.GetDataTableAsync("GetAllDashBoardTabs");
				if (dataTable.IsNotNull())
				{
					grd_General.DataSource = dataTable;
					await LoadTreeList();
					LoadAllContextStrip2();
					LoadAllContextStrip1();
				}
				else
				{
					grd_General.DataSource = null;
					contextMenuStrip2.Items.Clear();
					contextMenuStrip2.Items.Add(addNewToolStripMenuItem);
					contextMenuStrip1.Items.Clear();
				}
			}
			else
			{
				DataTable dataTable2 = await _help.GetDataTableAsync("GetAllTabsByCategory", new { catId });
				if (dataTable2.IsNotNull())
				{
					grd_General.DataSource = dataTable2;
					await LoadTreeList();
					LoadAllContextStrip2();
					LoadAllContextStrip1();
				}
				else
				{
					grd_General.DataSource = null;
					contextMenuStrip2.Items.Clear();
					contextMenuStrip2.Items.Add(addNewToolStripMenuItem);
					contextMenuStrip1.Items.Clear();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SearchTab(string tabName)
	{
		_ = 2;
		try
		{
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardTabs");
			DataTable dataTable = await _help.GetDataTableAsync("GetTabByTabName", new { tabName });
			if (dataTable.IsNotNull())
			{
				grd_General.DataSource = dataTable;
				await LoadTreeList();
				LoadAllContextStrip2();
				LoadAllContextStrip1();
			}
			else
			{
				grd_General.DataSource = null;
				await LoadTreeList();
				contextMenuStrip2.Items.Clear();
				contextMenuStrip2.Items.Add(addNewToolStripMenuItem);
				contextMenuStrip1.Items.Clear();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task NewTab()
	{
		try
		{
			int maxTabId = 0;
			using (HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardTabs"))
			{
				DataTable dataTable = await _help.GetDataTableAsync("GetMaxTabId");
				if (dataTable.IsNotNull())
				{
					maxTabId = Function.ParseInt(dataTable.Rows[0]["TabId"]);
				}
			}
			DataTable dataTable2 = CreateTabDataTable();
			DataRow dataRow = dataTable2.NewRow();
			dataRow["CatId"] = catId;
			dataRow["TabId"] = maxTabId + 1;
			dataRow["TabName"] = "";
			dataRow["TabDescription"] = "";
			dataRow["TabValid"] = true;
			dataRow["TargetType"] = "";
			dataRow["ObjType"] = "";
			dataTable2.Rows.Add(dataRow);
			dataTable2.AcceptChanges();
			grd_General.DataSource = dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task AddNewTabLine()
	{
		try
		{
			int maxTabId = 0;
			using (HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardTabs"))
			{
				DataTable dataTable = await _help.GetDataTableAsync("GetMaxTabId");
				if (dataTable.IsNotNull())
				{
					maxTabId = Function.ParseInt(dataTable.Rows[0]["TabId"]);
				}
			}
			DataTable dataTable2 = grd_General.DataSource as DataTable;
			if (dataTable2.IsNotNull())
			{
				DataRow dataRow = dataTable2.NewRow();
				dataRow["CatId"] = catId;
				dataRow["TabId"] = maxTabId + 1;
				dataRow["TabName"] = "";
				dataRow["TabDescription"] = "";
				dataRow["TabValid"] = true;
				dataRow["TargetType"] = "";
				dataRow["ObjType"] = "";
				dataTable2.Rows.Add(dataRow);
				dataTable2.AcceptChanges();
				grd_General.DataSource = dataTable2;
			}
			else
			{
				DataTable dataTable3 = CreateTabDataTable();
				DataRow dataRow2 = dataTable3.NewRow();
				dataRow2["CatId"] = catId;
				dataRow2["TabId"] = maxTabId + 1;
				dataRow2["TabName"] = "";
				dataRow2["TabDescription"] = "";
				dataRow2["TabValid"] = true;
				dataRow2["TargetType"] = "";
				dataRow2["ObjType"] = "";
				dataTable3.Rows.Add(dataRow2);
				dataTable3.AcceptChanges();
				grd_General.DataSource = dataTable3;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task NextTabLine()
	{
		try
		{
			int maxTabId = 0;
			using (HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardTabs"))
			{
				DataTable dataTable = await _help.GetDataTableAsync("GetMaxTabId");
				if (dataTable.IsNotNull())
				{
					maxTabId = Function.ParseInt(dataTable.Rows[0]["TabId"]);
				}
			}
			DataTable dataTable2 = grd_General.DataSource as DataTable;
			DataRow dataRow = dataTable2.NewRow();
			dataRow["CatId"] = catId;
			if (Function.ParseInt(dataTable2.Rows[dataTable2.Rows.Count - 1]["ControllerId"]) <= maxTabId)
			{
				dataRow["TabId"] = maxTabId + 1;
			}
			else
			{
				dataRow["TabId"] = Function.ParseInt(dataTable2.Rows[dataTable2.Rows.Count - 1]["TabId"]) + 1;
			}
			dataRow["TabName"] = "";
			dataRow["TabDescription"] = "";
			dataRow["TabValid"] = true;
			dataRow["TargetType"] = "";
			dataRow["ObjType"] = "";
			dataTable2.Rows.Add(dataRow);
			dataTable2.AcceptChanges();
			grd_General.DataSource = dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task AddNewfatherController()
	{
		_ = 1;
		try
		{
			DataTable dtController = tlController.DataSource as DataTable;
			int maxControllerId = 0;
			using (HttpClientHelper<DataTable> _help_1 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardTabs"))
			{
				DataTable dataTable = await _help_1.GetDataTableAsync("GetMaxControllerId");
				if (dataTable.IsNotNull())
				{
					maxControllerId = Function.ParseInt(dataTable.Rows[0]["ControllerId"]);
				}
			}
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardTabs");
			if ((await _help.GetDataTableAsync("GetControllersByTab", new
			{
				tabId = GetFocusedTabId()
			})).IsNotNull())
			{
				if (dtController != null)
				{
					DataRow dataRow = dtController.NewRow();
					dataRow["TabId"] = GetFocusedTabId();
					dataRow["ControllerId"] = maxControllerId + 1;
					dataRow["ControllerCode"] = "";
					dataRow["ControllerDescription"] = "";
					dataRow["Remarks"] = "";
					dataRow["QueryText"] = "";
					dataRow["QueryType"] = 'T';
					dataRow["ColumnKey"] = "";
					dataRow["ObjectKey"] = "";
					dataRow["FatherId"] = -1;
					dtController.Rows.Add(dataRow);
					dtController.AcceptChanges();
					tlController.DataSource = dtController;
				}
			}
			else
			{
				DataTable dataTable2 = CreateControllerDataTable();
				DataRow dataRow2 = dataTable2.NewRow();
				dataRow2["TabId"] = GetFocusedTabId();
				dataRow2["ControllerId"] = maxControllerId + 1;
				dataRow2["ControllerCode"] = "";
				dataRow2["ControllerDescription"] = "";
				dataRow2["Remarks"] = "";
				dataRow2["QueryText"] = "";
				dataRow2["QueryType"] = 'T';
				dataRow2["ColumnKey"] = "";
				dataRow2["ObjectKey"] = "";
				dataRow2["FatherId"] = -1;
				dataTable2.Rows.Add(dataRow2);
				dataTable2.AcceptChanges();
				tlController.DataSource = dataTable2;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task NextController()
	{
		try
		{
			int maxControllerId = 0;
			using (HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardTabs"))
			{
				DataTable dataTable = await _help.GetDataTableAsync("GetMaxControllerId");
				if (dataTable.IsNotNull())
				{
					maxControllerId = Function.ParseInt(dataTable.Rows[0]["ControllerId"]);
				}
			}
			DataTable dataTable2 = tlController.DataSource as DataTable;
			if (dataTable2.IsNotNull())
			{
				DataRow dataRow = dataTable2.NewRow();
				dataRow["TabId"] = GetFocusedTabId();
				if (Function.ParseInt(dataTable2.Rows[dataTable2.Rows.Count - 1]["ControllerId"]) <= maxControllerId)
				{
					dataRow["ControllerId"] = maxControllerId + 1;
				}
				else
				{
					dataRow["ControllerId"] = Function.ParseInt(dataTable2.Rows[dataTable2.Rows.Count - 1]["ControllerId"]) + 1;
				}
				dataRow["ControllerCode"] = "";
				dataRow["ControllerDescription"] = "";
				dataRow["Remarks"] = "";
				dataRow["QueryText"] = "";
				dataRow["QueryType"] = 'T';
				dataRow["ColumnKey"] = "";
				dataRow["ObjectKey"] = "";
				dataRow["FatherId"] = -1;
				dataTable2.Rows.Add(dataRow);
				dataTable2.AcceptChanges();
			}
			tlController.DataSource = dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task AddNewSubNode(int fatherId)
	{
		try
		{
			int maxControllerId = 0;
			using (HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardTabs"))
			{
				DataTable dataTable = await _help.GetDataTableAsync("GetMaxControllerId");
				if (dataTable.IsNotNull())
				{
					maxControllerId = Function.ParseInt(dataTable.Rows[0]["ControllerId"]);
				}
			}
			DataTable dataTable2 = tlController.DataSource as DataTable;
			if (dataTable2.IsNotNull())
			{
				DataRow dataRow = dataTable2.NewRow();
				dataRow["TabId"] = GetFocusedTabId();
				if (Function.ParseInt(dataTable2.Rows[dataTable2.Rows.Count - 1]["ControllerId"]) <= maxControllerId)
				{
					dataRow["ControllerId"] = maxControllerId + 1;
				}
				else
				{
					dataRow["ControllerId"] = Function.ParseInt(dataTable2.Rows[dataTable2.Rows.Count - 1]["ControllerId"]) + 1;
				}
				dataRow["ControllerCode"] = "";
				dataRow["ControllerDescription"] = "";
				dataRow["Remarks"] = "";
				dataRow["QueryText"] = "";
				dataRow["QueryType"] = 'T';
				dataRow["ColumnKey"] = "";
				dataRow["ObjectKey"] = "";
				dataRow["FatherId"] = fatherId;
				dataTable2.Rows.Add(dataRow);
				dataTable2.AcceptChanges();
			}
			tlController.DataSource = dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetFocusedNode()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ControllerCode", typeof(string));
			dataTable.Columns.Add("ControllerId", typeof(int));
			dataTable.Columns.Add("FatherId", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			int absoluteIndex = tlController.Columns["FatherId"].AbsoluteIndex;
			int absoluteIndex2 = tlController.Columns["ControllerId"].AbsoluteIndex;
			int absoluteIndex3 = tlController.Columns["ControllerCode"].AbsoluteIndex;
			if (tlController.FocusedNode != null)
			{
				object value = tlController.FocusedNode.GetValue(absoluteIndex);
				object value2 = tlController.FocusedNode.GetValue(absoluteIndex2);
				object value3 = tlController.FocusedNode.GetValue(absoluteIndex3);
				int num = Function.ParseInt(value);
				int num2 = Function.ParseInt(value2);
				string value4 = Function.ToString(value3);
				dataRow["FatherId"] = num;
				dataRow["ControllerId"] = num2;
				dataRow["ControllerCode"] = value4;
				dataTable.Rows.Add(dataRow);
				return dataTable;
			}
			return new DataTable();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	private int GetFocusedTabId()
	{
		return Function.ParseInt(Function.ParseInt(gvGeneral.GetFocusedRowCellValue("TabId")));
	}

	private async Task<bool> SaveData(DataTable dtTab, DataTable dtController)
	{
		_ = 2;
		try
		{
			if (!dtTab.IsNotNull())
			{
				dtTab = CreateTabDataTable();
				DataRow dataRow = dtTab.NewRow();
				dataRow["CatId"] = 0;
				dataRow["TabId"] = GetFocusedTabId();
				dataRow["TabName"] = "";
				dataRow["TabDescription"] = "";
				dataRow["TabValid"] = false;
				dataRow["TargetType"] = "";
				dataRow["ObjType"] = "";
				dtTab.Rows.Add(dataRow);
				dtTab.AcceptChanges();
			}
			if (!dtController.IsNotNull())
			{
				dtController = CreateControllerDataTable();
				DataRow dataRow2 = dtController.NewRow();
				dataRow2["TabId"] = GetFocusedTabId();
				dataRow2["ControllerId"] = 0;
				dataRow2["ControllerCode"] = "";
				dataRow2["ControllerDescription"] = "";
				dataRow2["Remarks"] = "";
				dataRow2["QueryText"] = "";
				dataRow2["QueryType"] = 'T';
				dataRow2["ColumnKey"] = "";
				dataRow2["ObjectKey"] = "";
				dataRow2["FatherId"] = -1;
				dtController.Rows.Add(dataRow2);
				dtController.AcceptChanges();
			}
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(dtTab.Copy());
			dataSet.Tables.Add(dtController.Copy());
			using HttpClientHelper<HttpResult> hp = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "DashBoardTabs");
			HttpResult httpResult = await hp.PostAsync("SaveDoc", dataSet);
			if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadData();
				await LoadTreeList();
				FormMode = Enums.FormViewModeType.View;
				return true;
			}
			if (httpResult != null)
			{
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
			}
			FormMode = Enums.FormViewModeType.Update;
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return false;
		}
	}

	private async Task DeleteTab(int tabId)
	{
		_ = 2;
		try
		{
			using HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "DashBoardTabs");
			string text = await hp.PostAsync("DeleteTab", tabId);
			if (text == "")
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				await LoadData();
				await LoadTreeList();
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

	private async Task<bool> CheckTabName()
	{
		_ = 1;
		try
		{
			DataTable dtTab = grd_General.DataSource as DataTable;
			string tabName = Function.ToString(gvGeneral.GetFocusedRowCellValue("TabName"));
			int cateId = Function.ParseInt(gvGeneral.GetFocusedRowCellValue("CatId"));
			int count = 0;
			int count2 = 0;
			int maxTabId = 0;
			using (HttpClientHelper<DataTable> _help_1 = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardTabs"))
			{
				DataTable dataTable = await _help_1.GetDataTableAsync("GetMaxTabId");
				if (dataTable.IsNotNull())
				{
					maxTabId = Function.ParseInt(dataTable.Rows[0]["TabId"]);
				}
			}
			using HttpClientHelper<DataTable> _help = new HttpClientHelper<DataTable>(Configuration.ApiServer, "DashBoardTabs");
			DataTable dataTable2 = await _help.GetDataTableAsync("GetAllDashBoardTabs");
			if (GetFocusedTabId() > maxTabId && dataTable2.IsNotNull())
			{
				if (dtTab.IsNotNull())
				{
					for (int i = 0; i < dtTab.Rows.Count; i++)
					{
						if (Function.ToString(dataTable2.Rows[i]["TabName"]).Trim().Equals(tabName) && Function.ParseInt(dataTable2.Rows[i]["CatId"]) == cateId)
						{
							count2++;
						}
					}
				}
				if (count2 > 0)
				{
					return true;
				}
			}
			else
			{
				if (dtTab.IsNotNull())
				{
					for (int j = 0; j < dtTab.Rows.Count; j++)
					{
						if (Function.ToString(dtTab.Rows[j]["TabName"]).Trim().Equals(tabName) && Function.ParseInt(dtTab.Rows[j]["CatId"]) == cateId)
						{
							count++;
						}
					}
				}
				if (count > 1)
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

	private async Task<bool> SaveProcess(int tabId)
	{
		try
		{
			int count = 0;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			DataTable dataTable = grd_General.DataSource as DataTable;
			DataTable dtController = tlController.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				DataRow row = dataTable.AsEnumerable().SingleOrDefault((DataRow r) => r.Field<int>("TabId") == tabId);
				DataTable dtTabSave = new DataTable();
				if (dataTable != null)
				{
					dtTabSave = dataTable.Clone();
					dtTabSave.ImportRow(row);
					dtTabSave.AcceptChanges();
				}
				if (dtController.IsNotNull())
				{
					if (dtController.Select("ControllerCode <> ''").Any() && (from dr in (from r in dtController.AsEnumerable()
							where r.Field<string>("ControllerCode").Trim() != ""
							select r).CopyToDataTable().AsEnumerable()
						select dr.Field<string>("ControllerCode").Trim() into x
						group x by x into g
						where g.Count() > 1
						select g.Key).ToList().Count > 0)
					{
						count++;
					}
					for (int num4 = 0; num4 < dtController.Rows.Count; num4++)
					{
						for (int num5 = 0; num5 < dtController.Rows.Count; num5++)
						{
							if (Function.ParseInt(dtController.Rows[num4]["ControllerId"]) == Function.ParseInt(dtController.Rows[num5]["FatherId"]) && string.IsNullOrEmpty(Function.ToString(dtController.Rows[num4]["ControllerCode"])))
							{
								num2++;
							}
						}
					}
				}
				for (int num6 = 0; num6 < dtTabSave.Rows.Count; num6++)
				{
					if (string.IsNullOrEmpty(Function.ToString(dtTabSave.Rows[num6]["TabName"])))
					{
						num++;
					}
					if (Function.ParseInt(dtTabSave.Rows[num6]["CatId"]) == 0)
					{
						num3++;
					}
				}
				if (num > 0)
				{
					WinCommons.Error(Enums.MessageCode.TabNameCannotBeEmpty, "");
				}
				else if (num3 > 0)
				{
					WinCommons.Error(Enums.MessageCode.CategoryCannotBeEmpty, "");
				}
				else if (num2 > 0)
				{
					WinCommons.Error(Enums.MessageCode.FatherControllerCannotBeEmpty, "");
				}
				else if (await CheckTabName())
				{
					WinCommons.Error(Enums.MessageCode.TabNameInACatagoryCannotBeBuplicated, "");
				}
				else if (count > 0)
				{
					WinCommons.Error(Enums.MessageCode.ControllerCodeCannotBeDuplicate, "");
				}
				else if (await SaveData(dtTabSave, dtController))
				{
					return true;
				}
			}
			return false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			WinCommons._CloseProcessing();
			return false;
		}
	}

	private async Task SearchProcess()
	{
		_ = 1;
		try
		{
			if (txtSearch.Text == "")
			{
				await LoadData();
			}
			else
			{
				await SearchTab(txtSearch.Text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.SaveError, "");
		}
	}

	protected override void NoAuthSetControlSearch()
	{
		btn_Search.Enabled = true;
		txtSearch.ReadOnly = false;
	}

	private async Task frm_DashboardTab_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			frm_DashboardTab frm_DashboardTab2 = this;
			string formId = (gvGeneral.FormId = "oDashboardTabs");
			frm_DashboardTab2.FormId = formId;
			gvGeneral.ItemId = "APZ_ODTB";
			prevTabId = Function.ParseInt(gvGeneral.GetFocusedRowCellValue("TabId"));
			FormMode = Enums.FormViewModeType.View;
			await LoadData();
			await LoadLookUp();
			LoadTarget();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvGeneral_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				int num = Function.ParseInt(gvGeneral.GetFocusedRowCellValue("TabId"));
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					FormMode = Enums.FormViewModeType.View;
					SaveProcess(prevTabId);
					prevTabId = num;
				}
				else
				{
					LoadData();
					prevTabId = num;
					FormMode = Enums.FormViewModeType.View;
				}
			}
			else
			{
				prevTabId = Function.ParseInt(gvGeneral.GetFocusedRowCellValue("TabId"));
			}
			LoadTreeList();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gvGeneral_CellValueChanged(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void gvGeneral_CellValueChanging(object sender, DevExpress.XtraGrid.Views.Base.CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void gvGeneral_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		try
		{
			if (e.Info.IsRowIndicator && e.RowHandle >= 0)
			{
				e.Info.DisplayText = (e.RowHandle + 1).ToString();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chooseFromListToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dt = tlController.DataSource as DataTable;
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.DashBoardCtrl, true, string.Empty, "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItems.Count > 0 && dt.IsNotNull())
			{
				DataRow dataRow = frm_CFL2.ChooseItems[0];
				tlController.FocusedNode.SetValue("ControllerCode", dataRow["ControllerCode"]);
				tlController.FocusedNode.SetValue("ControllerDescription", dataRow["ControllerDescription"]);
				tlController.FocusedNode.SetValue("Remarks", dataRow["Remarks"]);
				tlController.FocusedNode.SetValue("QueryText", dataRow["QueryText"]);
				tlController.FocusedNode.SetValue("QueryType", dataRow["QueryType"]);
				tlController.FocusedNode.SetValue("ColumnKey", dataRow["ColumnKey"]);
				tlController.FocusedNode.SetValue("ObjectKey", dataRow["ObjectKey"]);
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_SubCtrl_Click(object sender, EventArgs e)
	{
		try
		{
			int num = 0;
			DataTable focusedNode = GetFocusedNode();
			DataTable dataTable = tlController.DataSource as DataTable;
			if (dataTable.IsNotNull())
			{
				for (int i = 0; i < dataTable.Rows.Count; i++)
				{
					if (Function.ParseInt(dataTable.Rows[i]["FatherId"]) == Function.ParseInt(focusedNode.Rows[0]["ControllerId"]))
					{
						num++;
					}
				}
			}
			if (focusedNode.IsNotNull() && num == 0)
			{
				await AddNewSubNode(Function.ParseInt(focusedNode.Rows[0]["ControllerId"]));
				tlController.ExpandAll();
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				int tabId = Function.ParseInt(gvGeneral.GetFocusedRowCellValue("TabId"));
				WinCommons._OpenProcessing();
				btn_OK.Enabled = false;
				btn_Cancel.Enabled = false;
				SaveProcess(tabId);
				btn_OK.Enabled = true;
				btn_Cancel.Enabled = true;
				WinCommons._CloseProcessing();
			}
			else
			{
				LoadData();
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
		_isCancel = false;
	}

	private void addNewControllerCodeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
			if ((tlController.DataSource as DataTable).IsNotNull())
			{
				NextController();
			}
			else
			{
				AddNewfatherController();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void deleteRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ControllerCode", typeof(string));
			dataTable.Columns.Add("ControllerId", typeof(int));
			DataRow dataRow = dataTable.NewRow();
			int absoluteIndex = tlController.Columns["ControllerId"].AbsoluteIndex;
			int absoluteIndex2 = tlController.Columns["ControllerCode"].AbsoluteIndex;
			if (tlController.FocusedNode == null)
			{
				return;
			}
			object value = tlController.FocusedNode.GetValue(absoluteIndex);
			object value2 = tlController.FocusedNode.GetValue(absoluteIndex2);
			int controllerId = Function.ParseInt(value);
			string value3 = Function.ToString(value2);
			dataRow["ControllerId"] = controllerId;
			dataRow["ControllerCode"] = value3;
			dataTable.Rows.Add(dataRow);
			if (tlController.DataSource is DataTable dataTable2)
			{
				DataRow dataRow2 = dataTable2.AsEnumerable().SingleOrDefault((DataRow r) => r.Field<int>("ControllerId") == controllerId);
				if (dataRow2 != null)
				{
					dataTable2.Rows.Remove(dataRow2);
				}
				tlController.DataSource = dataTable2;
			}
			tlController.RefreshDataSource();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void deleteRowToolStripMenuItem1_Click(object sender, EventArgs e)
	{
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				DataTable dataTable = grd_General.DataSource as DataTable;
				if (dataTable.IsNotNull())
				{
					int tabId = Function.ParseInt(dataTable.Rows[gvGeneral.FocusedRowHandle]["TabId"]);
					await DeleteTab(tabId);
				}
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void addNewToolStripMenuItem_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			if (FormMode != Enums.FormViewModeType.Update)
			{
				await SearchProcess();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes)
			{
				int tabId = Function.ParseInt(gvGeneral.GetFocusedRowCellValue("TabId"));
				if (await SaveProcess(tabId))
				{
					await SearchProcess();
				}
			}
			else
			{
				FormMode = Enums.FormViewModeType.View;
				await SearchProcess();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void tlController_CellValueChanging(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void tlController_CellValueChanged(object sender, DevExpress.XtraTreeList.CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void expandAllToolStripMenuItem_Click(object sender, EventArgs e)
	{
		tlController.ExpandAll();
	}

	private void collapseAllToolStripMenuItem_Click(object sender, EventArgs e)
	{
		tlController.CollapseAll();
	}

	private async void frm_DashboardTab_FormClosing(object sender, FormClosingEventArgs e)
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
				{
					int tabId = Function.ParseInt(gvGeneral.GetFocusedRowCellValue("TabId"));
					WinCommons._OpenProcessing();
					bool flag = await SaveProcess(tabId);
					WinCommons._CloseProcessing();
					if (flag)
					{
						_forceClose = true;
						Close();
					}
					break;
				}
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Dashboard.frm_DashboardTab));
		this.rep_CbTarget = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.rep_Valid = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.rep_CodeTree = new DevExpress.XtraEditors.Repository.RepositoryItemTreeListLookUpEdit();
		this.grd_General = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip2 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
		this.gvGeneral = new Apzon.Controls.Grid.GridViewEx();
		this.colName = new Apzon.Controls.Grid.GridColumnEx();
		this.colDescription = new Apzon.Controls.Grid.GridColumnEx();
		this.colObjectType = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ObjType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.colCategory = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Categories = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.colTarget = new Apzon.Controls.Grid.GridColumnEx();
		this.colValid = new Apzon.Controls.Grid.GridColumnEx();
		this.colTabId = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemLookUpEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.chooseFromListToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
		this.addNewControllerCodeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
		this.expandAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.collapseAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_SubCtrl = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tlController = new DevExpress.XtraTreeList.TreeList();
		this.tlColCode = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlColDescription = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlColRemarks = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_QryText = new DevExpress.XtraEditors.Repository.RepositoryItemMemoExEdit();
		this.tlColQryText = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlColQryType = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_QryType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tlColColumnKey = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlColObjKey = new Apzon.Controls.Tree.TreeListColumnEx();
		this.rep_ObjKey = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.tlColTabId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.tlColControllerId = new Apzon.Controls.Tree.TreeListColumnEx();
		this.treeListColumn1 = new Apzon.Controls.Tree.TreeListColumnEx();
		this.txtSearch = new Apzon.Controls.Editor.TextEditEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.lblTabName = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.rep_CbTarget).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Valid).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CodeTree).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_General).BeginInit();
		this.contextMenuStrip2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gvGeneral).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ObjType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Categories).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tlController).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_QryText).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_QryType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ObjKey).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtSearch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		base.SuspendLayout();
		this.rep_CbTarget.AutoHeight = false;
		this.rep_CbTarget.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CbTarget.DisplayMember = "TargetString";
		this.rep_CbTarget.Name = "rep_CbTarget";
		this.rep_CbTarget.ValueMember = "TargetType";
		this.rep_Valid.AutoHeight = false;
		this.rep_Valid.Name = "rep_Valid";
		this.rep_CodeTree.AutoHeight = false;
		this.rep_CodeTree.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_CodeTree.DisplayMember = "ControllerCode";
		this.rep_CodeTree.Name = "rep_CodeTree";
		this.rep_CodeTree.NullText = "";
		this.rep_CodeTree.ValueMember = "ControllerId";
		this.grd_General.AllowPasteExcel = true;
		this.grd_General.ClipboardData = null;
		this.grd_General.ContextMenuStrip = this.contextMenuStrip2;
		this.grd_General.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_General.Location = new System.Drawing.Point(0, 0);
		this.grd_General.MainView = this.gvGeneral;
		this.grd_General.Name = "grd_General";
		this.grd_General.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_Categories, this.repositoryItemLookUpEdit1, this.rep_ObjType });
		this.grd_General.Size = new System.Drawing.Size(930, 156);
		this.grd_General.TabIndex = 0;
		this.grd_General.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvGeneral });
		this.contextMenuStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewToolStripMenuItem, this.deleteRowToolStripMenuItem1 });
		this.contextMenuStrip2.Name = "contextMenuStrip2";
		this.contextMenuStrip2.Size = new System.Drawing.Size(130, 48);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(129, 22);
		this.addNewToolStripMenuItem.Text = "Add new";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.addNewToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem1.Name = "deleteRowToolStripMenuItem1";
		this.deleteRowToolStripMenuItem1.Size = new System.Drawing.Size(129, 22);
		this.deleteRowToolStripMenuItem1.Text = "Delete Tab";
		this.deleteRowToolStripMenuItem1.Click += new System.EventHandler(this.deleteRowToolStripMenuItem1_Click);
		this.gvGeneral.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvGeneral.ColumnKey = null;
		this.gvGeneral.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.colName, this.colDescription, this.colObjectType, this.colCategory, this.colTarget, this.colValid, this.colTabId });
		this.gvGeneral.DisableFormSetting = false;
		this.gvGeneral.DisplayException = false;
		this.gvGeneral.EditColumn = null;
		this.gvGeneral.FieldCurrency = null;
		this.gvGeneral.FieldRate = null;
		this.gvGeneral.FormId = "oDasdBoardTab";
		this.gvGeneral.GridControl = this.grd_General;
		this.gvGeneral.IndexEditRow = -1;
		this.gvGeneral.IndicatorWidth = 40;
		this.gvGeneral.IsApzTable = false;
		this.gvGeneral.IsInsertNewRow = false;
		this.gvGeneral.IsLoadingData = false;
		this.gvGeneral.ItemId = "OGENERAL";
		this.gvGeneral.LstColumnAllowBatch = null;
		this.gvGeneral.LstColumnNotAllowBatch = null;
		this.gvGeneral.LstColumNotSetting = null;
		this.gvGeneral.Name = "gvGeneral";
		this.gvGeneral.OptionsFind.ShowClearButton = false;
		this.gvGeneral.OptionsView.ShowGroupPanel = false;
		this.gvGeneral.UseBatchMod = false;
		this.gvGeneral.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gvGeneral_CustomDrawRowIndicator);
		this.gvGeneral.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gvGeneral_FocusedRowChanged);
		this.gvGeneral.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvGeneral_CellValueChanged);
		this.gvGeneral.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvGeneral_CellValueChanging);
		this.colName.Caption = "Name";
		this.colName.CurrencyDateField = null;
		this.colName.CurrencyField = null;
		this.colName.CurrencyFixed = null;
		this.colName.CurrencyFixedRate = 0.0;
		this.colName.CurrencyRateField = null;
		this.colName.FieldName = "TabName";
		this.colName.Name = "colName";
		this.colName.PrimaryKey = false;
		this.colName.UnitField = null;
		this.colName.Visible = true;
		this.colName.VisibleIndex = 0;
		this.colDescription.Caption = "Description";
		this.colDescription.CurrencyDateField = null;
		this.colDescription.CurrencyField = null;
		this.colDescription.CurrencyFixed = null;
		this.colDescription.CurrencyFixedRate = 0.0;
		this.colDescription.CurrencyRateField = null;
		this.colDescription.FieldName = "TabDescription";
		this.colDescription.Name = "colDescription";
		this.colDescription.PrimaryKey = false;
		this.colDescription.UnitField = null;
		this.colDescription.Visible = true;
		this.colDescription.VisibleIndex = 1;
		this.colObjectType.Caption = "ObjectType";
		this.colObjectType.ColumnEdit = this.rep_ObjType;
		this.colObjectType.CurrencyDateField = null;
		this.colObjectType.CurrencyField = null;
		this.colObjectType.CurrencyFixed = null;
		this.colObjectType.CurrencyFixedRate = 0.0;
		this.colObjectType.CurrencyRateField = null;
		this.colObjectType.FieldName = "ObjType";
		this.colObjectType.Name = "colObjectType";
		this.colObjectType.PrimaryKey = false;
		this.colObjectType.UnitField = null;
		this.colObjectType.Visible = true;
		this.colObjectType.VisibleIndex = 2;
		this.rep_ObjType.AutoHeight = false;
		this.rep_ObjType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ObjType.DisplayMember = "ObjString";
		this.rep_ObjType.Name = "rep_ObjType";
		this.rep_ObjType.NullText = "";
		this.rep_ObjType.ValueMember = "ObjType";
		this.colCategory.Caption = "Category";
		this.colCategory.ColumnEdit = this.rep_Categories;
		this.colCategory.CurrencyDateField = null;
		this.colCategory.CurrencyField = null;
		this.colCategory.CurrencyFixed = null;
		this.colCategory.CurrencyFixedRate = 0.0;
		this.colCategory.CurrencyRateField = null;
		this.colCategory.FieldName = "CatId";
		this.colCategory.Name = "colCategory";
		this.colCategory.PrimaryKey = false;
		this.colCategory.UnitField = null;
		this.colCategory.Visible = true;
		this.colCategory.VisibleIndex = 3;
		this.rep_Categories.AccessibleName = "";
		this.rep_Categories.AutoHeight = false;
		this.rep_Categories.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Categories.DisplayMember = "CatName";
		this.rep_Categories.Name = "rep_Categories";
		this.rep_Categories.NullText = "";
		this.rep_Categories.ValueMember = "CatId";
		this.colTarget.Caption = "Target";
		this.colTarget.ColumnEdit = this.rep_CbTarget;
		this.colTarget.CurrencyDateField = null;
		this.colTarget.CurrencyField = null;
		this.colTarget.CurrencyFixed = null;
		this.colTarget.CurrencyFixedRate = 0.0;
		this.colTarget.CurrencyRateField = null;
		this.colTarget.FieldName = "TargetType";
		this.colTarget.Name = "colTarget";
		this.colTarget.PrimaryKey = false;
		this.colTarget.UnitField = null;
		this.colTarget.Visible = true;
		this.colTarget.VisibleIndex = 4;
		this.colValid.Caption = "Valid";
		this.colValid.ColumnEdit = this.rep_Valid;
		this.colValid.CurrencyDateField = null;
		this.colValid.CurrencyField = null;
		this.colValid.CurrencyFixed = null;
		this.colValid.CurrencyFixedRate = 0.0;
		this.colValid.CurrencyRateField = null;
		this.colValid.FieldName = "TabValid";
		this.colValid.Name = "colValid";
		this.colValid.PrimaryKey = false;
		this.colValid.UnitField = null;
		this.colValid.Visible = true;
		this.colValid.VisibleIndex = 5;
		this.colTabId.Caption = "Tab ID";
		this.colTabId.CurrencyDateField = null;
		this.colTabId.CurrencyField = null;
		this.colTabId.CurrencyFixed = null;
		this.colTabId.CurrencyFixedRate = 0.0;
		this.colTabId.CurrencyRateField = null;
		this.colTabId.FieldName = "TabId";
		this.colTabId.Name = "colTabId";
		this.colTabId.OptionsColumn.AllowEdit = false;
		this.colTabId.OptionsColumn.ReadOnly = true;
		this.colTabId.PrimaryKey = false;
		this.colTabId.UnitField = null;
		this.repositoryItemLookUpEdit1.AutoHeight = false;
		this.repositoryItemLookUpEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemLookUpEdit1.Name = "repositoryItemLookUpEdit1";
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[7] { this.chooseFromListToolStripMenuItem, this.toolStripSeparator1, this.addNewControllerCodeToolStripMenuItem, this.deleteRowToolStripMenuItem, this.toolStripSeparator2, this.expandAllToolStripMenuItem, this.collapseAllToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(167, 126);
		this.chooseFromListToolStripMenuItem.Name = "chooseFromListToolStripMenuItem";
		this.chooseFromListToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
		this.chooseFromListToolStripMenuItem.Text = "Choose From List";
		this.chooseFromListToolStripMenuItem.Click += new System.EventHandler(this.chooseFromListToolStripMenuItem_Click);
		this.toolStripSeparator1.Name = "toolStripSeparator1";
		this.toolStripSeparator1.Size = new System.Drawing.Size(163, 6);
		this.addNewControllerCodeToolStripMenuItem.Name = "addNewControllerCodeToolStripMenuItem";
		this.addNewControllerCodeToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
		this.addNewControllerCodeToolStripMenuItem.Text = "New Row";
		this.addNewControllerCodeToolStripMenuItem.Click += new System.EventHandler(this.addNewControllerCodeToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click);
		this.toolStripSeparator2.Name = "toolStripSeparator2";
		this.toolStripSeparator2.Size = new System.Drawing.Size(163, 6);
		this.expandAllToolStripMenuItem.Name = "expandAllToolStripMenuItem";
		this.expandAllToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
		this.expandAllToolStripMenuItem.Text = "Expand All";
		this.expandAllToolStripMenuItem.Click += new System.EventHandler(this.expandAllToolStripMenuItem_Click);
		this.collapseAllToolStripMenuItem.Name = "collapseAllToolStripMenuItem";
		this.collapseAllToolStripMenuItem.Size = new System.Drawing.Size(166, 22);
		this.collapseAllToolStripMenuItem.Text = "Collapse All";
		this.collapseAllToolStripMenuItem.Click += new System.EventHandler(this.collapseAllToolStripMenuItem_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(95, 464);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(88, 23);
		this.btn_Cancel.TabIndex = 8;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 464);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(87, 23);
		this.btn_OK.TabIndex = 9;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_SubCtrl.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_SubCtrl.FieldName = null;
		this.btn_SubCtrl.IsSearchControl = false;
		this.btn_SubCtrl.Location = new System.Drawing.Point(791, 464);
		this.btn_SubCtrl.Name = "btn_SubCtrl";
		this.btn_SubCtrl.Size = new System.Drawing.Size(141, 23);
		this.btn_SubCtrl.TabIndex = 10;
		this.btn_SubCtrl.Text = "Create Sub Controller";
		this.btn_SubCtrl.Click += new System.EventHandler(this.btn_SubCtrl_Click);
		this.tlController.Columns.AddRange(new DevExpress.XtraTreeList.Columns.TreeListColumn[10] { this.tlColCode, this.tlColDescription, this.tlColRemarks, this.tlColQryText, this.tlColQryType, this.tlColColumnKey, this.tlColObjKey, this.tlColTabId, this.tlColControllerId, this.treeListColumn1 });
		this.tlController.ContextMenuStrip = this.contextMenuStrip1;
		this.tlController.Cursor = System.Windows.Forms.Cursors.Default;
		this.tlController.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tlController.KeyFieldName = "ControllerId";
		this.tlController.Location = new System.Drawing.Point(0, 0);
		this.tlController.Name = "tlController";
		this.tlController.OptionsNavigation.UseTabKey = true;
		this.tlController.ParentFieldName = "FatherId";
		this.tlController.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_QryType, this.rep_ObjKey, this.rep_QryText });
		this.tlController.Size = new System.Drawing.Size(930, 258);
		this.tlController.TabIndex = 11;
		this.tlController.CellValueChanging += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tlController_CellValueChanging);
		this.tlController.CellValueChanged += new DevExpress.XtraTreeList.CellValueChangedEventHandler(this.tlController_CellValueChanged);
		this.tlColCode.Caption = "Controller Code";
		this.tlColCode.CurrencyDateField = null;
		this.tlColCode.CurrencyField = null;
		this.tlColCode.CurrencyFixed = null;
		this.tlColCode.CurrencyFixedRate = 0.0;
		this.tlColCode.CurrencyRateField = null;
		this.tlColCode.FieldName = "ControllerCode";
		this.tlColCode.Name = "tlColCode";
		this.tlColCode.Visible = true;
		this.tlColCode.VisibleIndex = 0;
		this.tlColDescription.Caption = "Description";
		this.tlColDescription.CurrencyDateField = null;
		this.tlColDescription.CurrencyField = null;
		this.tlColDescription.CurrencyFixed = null;
		this.tlColDescription.CurrencyFixedRate = 0.0;
		this.tlColDescription.CurrencyRateField = null;
		this.tlColDescription.FieldName = "ControllerDescription";
		this.tlColDescription.Name = "tlColDescription";
		this.tlColDescription.Visible = true;
		this.tlColDescription.VisibleIndex = 1;
		this.tlColRemarks.Caption = "Remarks";
		this.tlColRemarks.ColumnEdit = this.rep_QryText;
		this.tlColRemarks.CurrencyDateField = null;
		this.tlColRemarks.CurrencyField = null;
		this.tlColRemarks.CurrencyFixed = null;
		this.tlColRemarks.CurrencyFixedRate = 0.0;
		this.tlColRemarks.CurrencyRateField = null;
		this.tlColRemarks.FieldName = "Remarks";
		this.tlColRemarks.Name = "tlColRemarks";
		this.tlColRemarks.Visible = true;
		this.tlColRemarks.VisibleIndex = 2;
		this.rep_QryText.AutoHeight = false;
		this.rep_QryText.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_QryText.Name = "rep_QryText";
		this.rep_QryText.ShowIcon = false;
		this.tlColQryText.Caption = "Query Text";
		this.tlColQryText.ColumnEdit = this.rep_QryText;
		this.tlColQryText.CurrencyDateField = null;
		this.tlColQryText.CurrencyField = null;
		this.tlColQryText.CurrencyFixed = null;
		this.tlColQryText.CurrencyFixedRate = 0.0;
		this.tlColQryText.CurrencyRateField = null;
		this.tlColQryText.FieldName = "QueryText";
		this.tlColQryText.Name = "tlColQryText";
		this.tlColQryText.Visible = true;
		this.tlColQryText.VisibleIndex = 3;
		this.tlColQryType.Caption = "Query Type";
		this.tlColQryType.ColumnEdit = this.rep_QryType;
		this.tlColQryType.CurrencyDateField = null;
		this.tlColQryType.CurrencyField = null;
		this.tlColQryType.CurrencyFixed = null;
		this.tlColQryType.CurrencyFixedRate = 0.0;
		this.tlColQryType.CurrencyRateField = null;
		this.tlColQryType.FieldName = "QueryType";
		this.tlColQryType.Name = "tlColQryType";
		this.tlColQryType.Visible = true;
		this.tlColQryType.VisibleIndex = 4;
		this.rep_QryType.AutoHeight = false;
		this.rep_QryType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_QryType.DisplayMember = "DisplayType";
		this.rep_QryType.Name = "rep_QryType";
		this.rep_QryType.NullText = "";
		this.rep_QryType.ValueMember = "QueryType";
		this.tlColColumnKey.Caption = "Column Key";
		this.tlColColumnKey.CurrencyDateField = null;
		this.tlColColumnKey.CurrencyField = null;
		this.tlColColumnKey.CurrencyFixed = null;
		this.tlColColumnKey.CurrencyFixedRate = 0.0;
		this.tlColColumnKey.CurrencyRateField = null;
		this.tlColColumnKey.FieldName = "ColumnKey";
		this.tlColColumnKey.Name = "tlColColumnKey";
		this.tlColColumnKey.Visible = true;
		this.tlColColumnKey.VisibleIndex = 5;
		this.tlColObjKey.Caption = "Object Key";
		this.tlColObjKey.ColumnEdit = this.rep_ObjKey;
		this.tlColObjKey.CurrencyDateField = null;
		this.tlColObjKey.CurrencyField = null;
		this.tlColObjKey.CurrencyFixed = null;
		this.tlColObjKey.CurrencyFixedRate = 0.0;
		this.tlColObjKey.CurrencyRateField = null;
		this.tlColObjKey.FieldName = "ObjectKey";
		this.tlColObjKey.Name = "tlColObjKey";
		this.tlColObjKey.Visible = true;
		this.tlColObjKey.VisibleIndex = 6;
		this.rep_ObjKey.AutoHeight = false;
		this.rep_ObjKey.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ObjKey.DisplayMember = "ObjectKey";
		this.rep_ObjKey.Name = "rep_ObjKey";
		this.rep_ObjKey.NullText = "";
		this.rep_ObjKey.ValueMember = "ObjectKey";
		this.tlColTabId.Caption = "TabId";
		this.tlColTabId.CurrencyDateField = null;
		this.tlColTabId.CurrencyField = null;
		this.tlColTabId.CurrencyFixed = null;
		this.tlColTabId.CurrencyFixedRate = 0.0;
		this.tlColTabId.CurrencyRateField = null;
		this.tlColTabId.FieldName = "TabId";
		this.tlColTabId.Name = "tlColTabId";
		this.tlColControllerId.Caption = "Controller Id";
		this.tlColControllerId.CurrencyDateField = null;
		this.tlColControllerId.CurrencyField = null;
		this.tlColControllerId.CurrencyFixed = null;
		this.tlColControllerId.CurrencyFixedRate = 0.0;
		this.tlColControllerId.CurrencyRateField = null;
		this.tlColControllerId.FieldName = "ControllerId";
		this.tlColControllerId.Name = "tlColControllerId";
		this.treeListColumn1.Caption = "Father ID";
		this.treeListColumn1.CurrencyDateField = null;
		this.treeListColumn1.CurrencyField = null;
		this.treeListColumn1.CurrencyFixed = null;
		this.treeListColumn1.CurrencyFixedRate = 0.0;
		this.treeListColumn1.CurrencyRateField = null;
		this.treeListColumn1.FieldName = "FatherId";
		this.treeListColumn1.Name = "treeListColumn1";
		this.txtSearch.FieldName = null;
		this.txtSearch.IsSearchControl = false;
		this.txtSearch.Location = new System.Drawing.Point(57, 13);
		this.txtSearch.Name = "txtSearch";
		this.txtSearch.Size = new System.Drawing.Size(403, 20);
		this.txtSearch.TabIndex = 12;
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(466, 13);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.Size = new System.Drawing.Size(88, 20);
		this.btn_Search.TabIndex = 13;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 39);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_General);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.tlController);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(930, 419);
		this.splitContainerControl1.SplitterPosition = 156;
		this.splitContainerControl1.TabIndex = 14;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.lblTabName.FieldName = null;
		this.lblTabName.HideWhenEmpty = false;
		this.lblTabName.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lblTabName.Location = new System.Drawing.Point(2, 13);
		this.lblTabName.Name = "lblTabName";
		this.lblTabName.ShowUnderLine = true;
		this.lblTabName.Size = new System.Drawing.Size(56, 20);
		this.lblTabName.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lblTabName.TabIndex = 15;
		this.lblTabName.Utext = "Tab Name";
		base.AcceptButton = this.btn_OK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(932, 490);
		base.Controls.Add(this.lblTabName);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.txtSearch);
		base.Controls.Add(this.btn_SubCtrl);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		this.FormId = "oDasdBoardTab";
		base.Name = "frm_DashboardTab";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "DashBoard Tabs";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_DashboardTab_FormClosing);
		((System.ComponentModel.ISupportInitialize)this.rep_CbTarget).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Valid).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CodeTree).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_General).EndInit();
		this.contextMenuStrip2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gvGeneral).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ObjType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Categories).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemLookUpEdit1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tlController).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_QryText).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_QryType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ObjKey).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtSearch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
