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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Forms.Administrator;
using Apzon.Forms.Commons;
using Apzon.Forms.Properties;
using Apzon.Forms.SalesOppotunity;
using Apzon.Forms.Service;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Mask;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.PM;

public class frm_RelatedActivites : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private readonly ApzObjectTypes _objType;

	private readonly string _objId;

	private readonly string _objLine;

	public string CardCode;

	public string WbsCode;

	public string ProjectCode;

	public string DocNum;

	public bool HideTask;

	public bool HideEvent;

	public bool HideNote;

	public DataTable DtActList;

	private Dictionary<int, string> _dictionEmployee = new Dictionary<int, string>();

	private Dictionary<int, string> _dictionUser = new Dictionary<int, string>();

	public List<Tuple<GridViewEx, string>> FormItem = new List<Tuple<GridViewEx, string>>();

	private IContainer components;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtra_Task;

	private XtraTabPageEx xtra_Event;

	private XtraTabPageEx xtra_Note;

	private ContextMenuStrip ctm_Task;

	private ToolStripMenuItem newTaskToolStripMenuItem;

	private ContextMenuStrip ctm_Event;

	private ToolStripMenuItem newToolStripMenuItem;

	private ContextMenuStrip ctm_Note;

	private ToolStripMenuItem newNoteToolStripMenuItem;

	private GridControlEx grd_Task;

	private GridViewEx gv_Task;

	private GridColumnEx gridColumnEx12;

	private GridColumnEx gc_Closed_Completed;

	private RepositoryItemButtonEdit rep_bte_Closed;

	private GridColumnEx gridColumnEx20;

	private GridColumnEx gc_DueDate_Completed;

	private GridColumnEx gc_ProjectEntry_Completed;

	private GridColumnEx gc_ProjectWBS_Completed;

	private GridColumnEx gridColumnEx21;

	private GridColumnEx gridColumnEx23;

	private GridColumnEx gridColumnEx22;

	private GridColumnEx gc_AssignTo_Completed;

	private GridColumnEx gc_CloseDate_Completed;

	private GridColumnEx gc_Priority_Completed;

	private RepositoryItemButtonEdit rep_bte_Open;

	private GridControlEx grd_Note;

	private GridViewEx gv_Note;

	private GridColumnEx cl_ClgCode;

	private GridColumnEx cl_Details;

	private GridColumnEx gc_AssignTo;

	private GridColumnEx gridColumnEx1;

	private GridControlEx grd_Event;

	private GridViewEx gv_Event;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemButtonEditCFL rep_Bp;

	private GridColumnEx gridColumnEx4;

	private RepositoryItemDateEdit repositoryItemDateEdit1;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx6;

	private RepositoryItemCheckEdit repositoryItemCheckEdit1;

	private GridColumnEx gridColumnEx8;

	private GridColumnEx gridColumnEx9;

	private RepositoryItemDateEditEx repositoryItemDateEditEx1;

	private RepositoryItemCheckedComboBoxEdit rep_Invite;

	public frm_RelatedActivites(ApzObjectTypes objType, string objId, string objLine = "-1")
	{
		InitializeComponent();
		FormId = "oRelatedActivities";
		_objType = objType;
		_objId = objId;
		_objLine = objLine;
		gv_Event.RowCellStyle += gv_Note_RowCellStyle;
		gv_Event.CustomUnboundColumnData += gv_Note_CustomUnboundColumnData;
		gv_Task.DoubleClick += Gv_Task_DoubleClick;
		gv_Event.DoubleClick += Gv_Task_DoubleClick;
		gv_Note.DoubleClick += Gv_Task_DoubleClick;
		OnLoadAsync += frm_RelatedActivites_Load;
	}

	private void Gv_Task_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			switch (((GridViewEx)sender).Name)
			{
			case "gv_Task":
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
				tableStructure.ImportRow(gv_Task.GetFocusedDataRow());
				DataRow focusedDataRow = tableStructure.Rows[0];
				frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement
				{
					ClgCode = Function.ParseInt(focusedDataRow["ClgCode"]),
					CallFromCal = true,
					MdiParent = MdiParent,
					DataRowFromCal = focusedDataRow
				};
				frm_TaskManagement2.ReloadRelated = (Func<DataRow, Task>)Delegate.Combine(frm_TaskManagement2.ReloadRelated, new Func<DataRow, Task>(ReloadTaskList));
				frm_TaskManagement2.ReloadAfterRemove = (Func<int, Task>)Delegate.Combine(frm_TaskManagement2.ReloadAfterRemove, new Func<int, Task>(RemoveTask));
				frm_TaskManagement2.Show();
				break;
			}
			case "gv_Event":
			{
				DataRow focusedDataRow = gv_Event.GetFocusedDataRow();
				frm_EventManagement frm_EventManagement2 = new frm_EventManagement
				{
					ClgCode = Function.ParseInt(focusedDataRow["ClgCode"]),
					CallFromRelated = true,
					MdiParent = MdiParent,
					DataRowFromCal = focusedDataRow
				};
				frm_EventManagement2.ReloadRelated = (Func<DataRow, Task>)Delegate.Combine(frm_EventManagement2.ReloadRelated, new Func<DataRow, Task>(ReloadEventList));
				frm_EventManagement2.ReloadAfterRemove = (Func<int, Task>)Delegate.Combine(frm_EventManagement2.ReloadAfterRemove, new Func<int, Task>(RemoveEvent));
				frm_EventManagement2.Show();
				break;
			}
			case "gv_Note":
			{
				DataRow focusedDataRow = gv_Note.GetFocusedDataRow();
				frm_NoteManagement frm_NoteManagement2 = new frm_NoteManagement
				{
					ClgCode = Function.ParseInt(focusedDataRow["ClgCode"]),
					CallFromRelated = true,
					MdiParent = MdiParent,
					DataRowFromCal = focusedDataRow
				};
				frm_NoteManagement2.ReloadRelated = (Func<DataRow, Task>)Delegate.Combine(frm_NoteManagement2.ReloadRelated, new Func<DataRow, Task>(ReloadNoteList));
				frm_NoteManagement2.ReloadAfterRemove = (Func<int, Task>)Delegate.Combine(frm_NoteManagement2.ReloadAfterRemove, new Func<int, Task>(RemoveNote));
				frm_NoteManagement2.Show();
				break;
			}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RemoveNote(int clgCode)
	{
		try
		{
			if (!(grd_Note.DataSource is DataTable dataTable))
			{
				return;
			}
			DataRow[] array = dataTable.Select("ClgCode=" + clgCode);
			DataRow[] array2 = DtActList.Select("ClgCode=" + clgCode);
			if (array.Any())
			{
				DataRow[] array3 = array;
				foreach (DataRow row in array3)
				{
					dataTable.Rows.Remove(row);
				}
			}
			if (array2.Any())
			{
				DataRow[] array3 = array2;
				foreach (DataRow row2 in array3)
				{
					DtActList.Rows.Remove(row2);
				}
			}
			dataTable.AcceptChanges();
			DtActList.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RemoveEvent(int clgCode)
	{
		try
		{
			if (!(grd_Event.DataSource is DataTable dataTable))
			{
				return;
			}
			DataRow[] array = dataTable.Select("ClgCode=" + clgCode);
			DataRow[] array2 = DtActList.Select("ClgCode=" + clgCode);
			if (array.Any())
			{
				DataRow[] array3 = array;
				foreach (DataRow row in array3)
				{
					dataTable.Rows.Remove(row);
				}
			}
			if (array2.Any())
			{
				DataRow[] array3 = array2;
				foreach (DataRow row2 in array3)
				{
					DtActList.Rows.Remove(row2);
				}
			}
			dataTable.AcceptChanges();
			DtActList.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task RemoveTask(int clgCode)
	{
		try
		{
			if (!(grd_Task.DataSource is DataTable dataTable))
			{
				return;
			}
			DataRow[] array = dataTable.Select("ClgCode=" + clgCode);
			DataRow[] array2 = DtActList.Select("ClgCode=" + clgCode);
			if (array.Any())
			{
				DataRow[] array3 = array;
				foreach (DataRow row in array3)
				{
					dataTable.Rows.Remove(row);
				}
			}
			if (array2.Any())
			{
				DataRow[] array3 = array2;
				foreach (DataRow row2 in array3)
				{
					DtActList.Rows.Remove(row2);
				}
			}
			dataTable.AcceptChanges();
			DtActList.AcceptChanges();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_RelatedActivites_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			xtra_Task.PageVisible = !HideTask;
			xtra_Event.PageVisible = !HideEvent;
			xtra_Note.PageVisible = !HideNote;
			Task task = LoadTaskByObjType();
			Task task2 = LoadEventByObjType();
			Task task3 = LoadNoteByObjType();
			await Task.WhenAll(task, task2, task3);
			DataTable dataTable = ((!(grd_Task.DataSource is DataTable dataTable2)) ? WinFuntions.GetTableStructure("APZ_OCLG") : dataTable2.Copy());
			DataTable dataTable3 = grd_Event.DataSource as DataTable;
			DataTable dataTable4 = grd_Note.DataSource as DataTable;
			DtActList = WinFuntions.GetTableStructure("APZ_OCLG");
			if (dataTable.IsNotNull())
			{
				if (dataTable.Columns.Contains("Id"))
				{
					dataTable.Columns.Remove("Id");
				}
				if (dataTable.Columns.Contains("Data"))
				{
					dataTable.Columns.Remove("Data");
				}
				if (dataTable.Columns.Contains("Id1"))
				{
					dataTable.Columns.Remove("Id1");
				}
				if (dataTable.Columns.Contains("Data1"))
				{
					dataTable.Columns.Remove("Data1");
				}
				foreach (DataRow row in dataTable.Rows)
				{
					DtActList.Rows.Add(row.ItemArray);
				}
			}
			if (dataTable3.IsNotNull())
			{
				foreach (DataRow row2 in dataTable3.Rows)
				{
					DtActList.Rows.Add(row2.ItemArray);
				}
			}
			if (dataTable4.IsNotNull())
			{
				foreach (DataRow row3 in dataTable4.Rows)
				{
					DtActList.Rows.Add(row3.ItemArray);
				}
			}
			DataSet dataSet = await _httpClient.Cfl.GetUserList();
			new DataTable();
			DataTable dataTable5 = ((!dataSet.IsHasStructure()) ? dataSet.Tables[0] : dataSet.Tables[1]);
			if (dataTable5.IsNotNull() && dataTable5.ExistsColumn("USERID") && dataTable5.ExistsColumn("U_NAME"))
			{
				_dictionUser = dataTable5.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["USERID"]), (DataRow s) => Function.ToString(s["U_NAME"]));
			}
			DataSet employeeListForSalesEmpSetup = _httpClient.Cfl.GetEmployeeListForSalesEmpSetup();
			new DataTable();
			DataTable dataTable6 = ((!employeeListForSalesEmpSetup.IsHasStructure()) ? employeeListForSalesEmpSetup.Tables[0] : employeeListForSalesEmpSetup.Tables[1]);
			if (dataTable6.IsNotNull() && dataTable6.ExistsColumn("empID") && dataTable6.ExistsColumn("lastName") && dataTable6.ExistsColumn("firstName"))
			{
				_dictionEmployee = dataTable6.AsEnumerable().ToDictionary((DataRow s) => Function.ParseInt(s["empID"]), (DataRow s) => Function.ToString(s["firstName"]) + "," + Function.ToString(s["lastName"]));
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadTaskByObjType()
	{
		try
		{
			DataTable dataTable = await _httpClient.SubActivities.LoadTaskByObjectType(GlobalData.UserCodeLogin, _objType, _objId);
			if (dataTable.IsNotNull() && !string.IsNullOrEmpty(_objLine) && _objLine != "-1")
			{
				DataRow[] source = dataTable.Select("OprLine=" + _objLine);
				if (source.Any())
				{
					grd_Task.DataSource = source.CopyToDataTable();
				}
				else
				{
					grd_Task.DataSource = dataTable.Clone();
				}
			}
			else
			{
				grd_Task.DataSource = dataTable;
			}
			grd_Task.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadNoteByObjType()
	{
		try
		{
			DataTable dataTable = await _httpClient.SubActivities.LoadNoteByObjectType(GlobalData.UserCodeLogin, ApzObjectTypes.oNoteManagement, _objType, _objId);
			if (dataTable.IsNotNull() && !string.IsNullOrEmpty(_objLine) && _objLine != "-1")
			{
				DataRow[] source = dataTable.Select("OprLine=" + _objLine);
				if (source.Any())
				{
					grd_Note.DataSource = source.CopyToDataTable();
				}
				else
				{
					grd_Note.DataSource = dataTable.Clone();
				}
			}
			else
			{
				grd_Note.DataSource = dataTable;
			}
			grd_Note.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task LoadEventByObjType()
	{
		try
		{
			DataTable dataTable = await _httpClient.SubActivities.LoadEventByObjectType(GlobalData.UserCodeLogin, ApzObjectTypes.oEventManagement, _objType, _objId);
			if (dataTable.IsNotNull() && !string.IsNullOrEmpty(_objLine) && _objLine != "-1")
			{
				DataRow[] source = dataTable.Select("OprLine=" + _objLine);
				if (source.Any())
				{
					grd_Event.DataSource = source.CopyToDataTable();
				}
				else
				{
					grd_Event.DataSource = dataTable.Clone();
				}
			}
			else
			{
				grd_Event.DataSource = dataTable;
			}
			grd_Event.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void newTaskToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(_objId) > 0)
			{
				frm_TaskManagement frm_TaskManagement2 = new frm_TaskManagement
				{
					ClgCode = 0,
					StartPosition = FormStartPosition.CenterScreen,
					MdiParent = MdiParent,
					CallFromCal = true
				};
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
				DataRow dataRow = tableStructure.NewRow();
				dataRow["GroupUser"] = GlobalData.UserID;
				if (Function.ToString(dataRow["Recontact"]) == "")
				{
					dataRow["Recontact"] = DateTime.Today;
					string text = DateTime.Now.Minute.ToString();
					dataRow["BeginTime"] = DateTime.Now.Hour + ((text.Length == 2) ? text : ("0" + text));
				}
				dataRow["Closed"] = "N";
				if (_objType == ApzObjectTypes.oProjectManager)
				{
					dataRow["ProjectEntry"] = _objId;
					dataRow["ProjectWBS"] = WbsCode;
				}
				else if (_objType == ApzObjectTypes.oTrackingIssue)
				{
					dataRow["ProjectEntry"] = ProjectCode;
					dataRow["ProjectWBS"] = WbsCode;
					dataRow["DocType"] = (int)_objType;
					dataRow["DocNum"] = _objId;
					dataRow["DocEntry"] = _objId;
				}
				else if (_objType == ApzObjectTypes.oSalesOpportunities)
				{
					dataRow["OprId"] = _objId;
					dataRow["OprLine"] = _objLine;
				}
				else
				{
					dataRow["DocType"] = (int)_objType;
					dataRow["DocNum"] = DocNum;
					dataRow["DocEntry"] = _objId;
				}
				dataRow["CardCode"] = CardCode;
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
				frm_TaskManagement2.DataRowFromCal = dataRow;
				frm_TaskManagement2.ReloadRelated = (Func<DataRow, Task>)Delegate.Combine(frm_TaskManagement2.ReloadRelated, new Func<DataRow, Task>(ReloadTaskList));
				frm_TaskManagement2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void newToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(_objId) > 0)
			{
				int userID = GlobalData.UserID;
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
				DataRow dataRow = tableStructure.NewRow();
				dataRow["ClgCode"] = 0;
				dataRow["Recontact"] = DateTime.Today;
				string text = DateTime.Now.ToString("HH:mm");
				dataRow["BeginTime"] = Function.ParseInt(text.Replace(":", ""));
				dataRow["EndDate"] = DateTime.Today;
				string text2 = DateTime.Now.AddMinutes(30.0).ToString("HH:mm");
				dataRow["ENDTime"] = Function.ParseInt(text2.Replace(":", ""));
				if (Function.ParseInt(dataRow["BeginTime"]) == 0 && Function.ParseInt(dataRow["ENDTime"]) == 0)
				{
					dataRow["AllDay"] = "Y";
				}
				else
				{
					dataRow["AllDay"] = "N";
				}
				dataRow["AllDayEvent"] = false;
				dataRow["GroupUser"] = Function.ToString(userID);
				dataRow["Closed"] = "N";
				if (_objType == ApzObjectTypes.oProjectManager)
				{
					dataRow["ProjectEntry"] = _objId;
					dataRow["ProjectWBS"] = WbsCode;
				}
				else if (_objType == ApzObjectTypes.oTrackingIssue)
				{
					dataRow["ProjectEntry"] = ProjectCode;
					dataRow["ProjectWBS"] = WbsCode;
					dataRow["DocType"] = (int)_objType;
					dataRow["DocNum"] = _objId;
					dataRow["DocEntry"] = _objId;
				}
				else if (_objType == ApzObjectTypes.oSalesOpportunities)
				{
					dataRow["OprId"] = _objId;
					dataRow["OprLine"] = _objLine;
				}
				else
				{
					dataRow["DocType"] = (int)_objType;
					dataRow["DocNum"] = DocNum;
					dataRow["DocEntry"] = _objId;
				}
				dataRow["CardCode"] = CardCode;
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
				frm_EventManagement frm_EventManagement2 = new frm_EventManagement
				{
					MdiParent = MdiParent,
					CallFromRelated = true,
					DataRowFromCal = dataRow,
					StartPosition = FormStartPosition.CenterScreen
				};
				frm_EventManagement2.ReloadRelated = (Func<DataRow, Task>)Delegate.Combine(frm_EventManagement2.ReloadRelated, new Func<DataRow, Task>(ReloadEventList));
				frm_EventManagement2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void newNoteToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (Function.ParseInt(_objId) > 0)
			{
				DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OCLG");
				DataRow dataRow = tableStructure.NewRow();
				dataRow["ClgCode"] = 0;
				dataRow["Recontact"] = DateTime.Today;
				string text = DateTime.Now.ToString("HH:mm");
				dataRow["BeginTime"] = Function.ParseInt(text.Replace(":", ""));
				dataRow["EndDate"] = DateTime.Today;
				string text2 = DateTime.Now.AddMinutes(30.0).ToString("HH:mm");
				dataRow["ENDTime"] = Function.ParseInt(text2.Replace(":", ""));
				if (Function.ParseInt(dataRow["BeginTime"]) == 0 && Function.ParseInt(dataRow["ENDTime"]) == 0)
				{
					dataRow["AllDay"] = "Y";
				}
				else
				{
					dataRow["AllDay"] = "N";
				}
				dataRow["GroupUser"] = Function.ToString(GlobalData.UserID);
				dataRow["Closed"] = "N";
				if (_objType == ApzObjectTypes.oProjectManager)
				{
					dataRow["ProjectEntry"] = _objId;
					dataRow["ProjectWBS"] = WbsCode;
				}
				else if (_objType == ApzObjectTypes.oTrackingIssue)
				{
					dataRow["ProjectEntry"] = ProjectCode;
					dataRow["ProjectWBS"] = WbsCode;
					dataRow["DocType"] = (int)_objType;
					dataRow["DocNum"] = _objId;
					dataRow["DocEntry"] = _objId;
				}
				else if (_objType == ApzObjectTypes.oSalesOpportunities)
				{
					dataRow["OprId"] = _objId;
					dataRow["OprLine"] = _objLine;
				}
				else
				{
					dataRow["DocType"] = (int)_objType;
					dataRow["DocNum"] = DocNum;
					dataRow["DocEntry"] = _objId;
				}
				dataRow["CardCode"] = CardCode;
				tableStructure.Rows.Add(dataRow);
				tableStructure.AcceptChanges();
				frm_NoteManagement frm_NoteManagement2 = new frm_NoteManagement
				{
					MdiParent = MdiParent,
					CallFromRelated = true,
					DataRowFromCal = dataRow,
					StartPosition = FormStartPosition.CenterScreen
				};
				frm_NoteManagement2.ReloadRelated = (Func<DataRow, Task>)Delegate.Combine(frm_NoteManagement2.ReloadRelated, new Func<DataRow, Task>(ReloadNoteList));
				frm_NoteManagement2.Show();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadTaskList(DataRow dr)
	{
		try
		{
			int num = Function.ParseInt(dr["ClgCode"]);
			if (grd_Task.DataSource is DataTable dataTable)
			{
				DataRow[] array = dataTable.Select("ClgCode=" + num);
				DataRow[] array2 = DtActList.Select("ClgCode=" + num);
				if (array.Any())
				{
					array[0].ItemArray = dr.ItemArray;
				}
				else
				{
					dataTable.ImportRow(dr);
				}
				if (array2.Any())
				{
					array2[0].ItemArray = dr.ItemArray;
				}
				else
				{
					DtActList.Rows.Add(dr.ItemArray);
				}
				dataTable.AcceptChanges();
				DtActList.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadEventList(DataRow dr)
	{
		try
		{
			int num = Function.ParseInt(dr["ClgCode"]);
			if (grd_Event.DataSource is DataTable dataTable)
			{
				DataRow[] array = dataTable.Select("ClgCode=" + num);
				DataRow[] array2 = DtActList.Select("ClgCode=" + num);
				if (array.Any())
				{
					array[0].ItemArray = dr.ItemArray;
				}
				else
				{
					dataTable.ImportRow(dr);
				}
				if (array2.Any())
				{
					array2[0].ItemArray = dr.ItemArray;
				}
				else
				{
					DtActList.Rows.Add(dr.ItemArray);
				}
				dataTable.AcceptChanges();
				DtActList.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task ReloadNoteList(DataRow dr)
	{
		try
		{
			int num = Function.ParseInt(dr["ClgCode"]);
			if (grd_Note.DataSource is DataTable dataTable)
			{
				DataRow[] array = dataTable.Select("ClgCode=" + num);
				DataRow[] array2 = DtActList.Select("ClgCode=" + num);
				if (array.Any())
				{
					array[0].ItemArray = dr.ItemArray;
				}
				else
				{
					dataTable.ImportRow(dr);
				}
				if (array2.Any())
				{
					array2[0].ItemArray = dr.ItemArray;
				}
				else
				{
					DtActList.Rows.Add(dr.ItemArray);
				}
				dataTable.AcceptChanges();
				DtActList.AcceptChanges();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Task_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		try
		{
			if (!(sender is GridView gridView) || e.Column.FieldName != "AssignTo" || !e.IsGetData)
			{
				return;
			}
			int listSourceRowIndex = e.ListSourceRowIndex;
			string text = Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "GroupUser"));
			string text2 = Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "GroupEmployee"));
			string text3 = "";
			if (!string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
			{
				foreach (string item in text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					int key = Function.ParseInt(item);
					if (_dictionUser.ContainsKey(key))
					{
						text3 = text3 + _dictionUser[key] + "; ";
					}
				}
			}
			if (!string.IsNullOrEmpty(text2))
			{
				foreach (string item2 in text2.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					int key2 = Function.ParseInt(item2);
					if (_dictionEmployee.ContainsKey(key2))
					{
						text3 = text3 + _dictionEmployee[key2] + "; ";
					}
				}
			}
			if (text3.Length > 2)
			{
				text3 = text3.Remove(text3.Length - 2);
			}
			e.Value = text3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Note_CustomUnboundColumnData(object sender, CustomColumnDataEventArgs e)
	{
		try
		{
			if (!(sender is GridView gridView) || e.Column.FieldName != "AssignTo" || !e.IsGetData)
			{
				return;
			}
			int listSourceRowIndex = e.ListSourceRowIndex;
			string text = Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "GroupUser"));
			string text2 = Function.ToString(gridView.GetListSourceRowCellValue(listSourceRowIndex, "GroupEmployee"));
			string text3 = "";
			if (!string.IsNullOrEmpty(text) && string.IsNullOrEmpty(text2))
			{
				foreach (string item in text.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					int key = Function.ParseInt(item);
					if (_dictionUser.ContainsKey(key))
					{
						text3 = text3 + _dictionUser[key] + "; ";
					}
				}
			}
			if (!string.IsNullOrEmpty(text2))
			{
				foreach (string item2 in text2.Split(new char[1] { ';' }, StringSplitOptions.RemoveEmptyEntries).ToList())
				{
					int key2 = Function.ParseInt(item2);
					if (_dictionEmployee.ContainsKey(key2))
					{
						text3 = text3 + _dictionEmployee[key2] + "; ";
					}
				}
			}
			if (text3.Length > 2)
			{
				text3 = text3.Remove(text3.Length - 2);
			}
			e.Value = text3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Task_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "Closed")
			{
				string text = Function.ToString(gv_Task.GetRowCellValue(e.RowHandle, e.Column));
				if (text == "Y")
				{
					e.RepositoryItem = rep_bte_Closed;
				}
				else if (text == "N")
				{
					e.RepositoryItem = rep_bte_Open;
				}
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Note_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "AssignTo")
			{
				string value = Function.ToString(gv_Note.GetRowCellValue(e.RowHandle, "GroupUser"));
				string value2 = Function.ToString(gv_Note.GetRowCellValue(e.RowHandle, "GroupEmployee"));
				if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2))
				{
					e.Appearance.ForeColor = Color.FromArgb(125, 88, 0);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Task_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.FieldName == "AssignTo")
			{
				string value = Function.ToString(gv_Task.GetRowCellValue(e.RowHandle, "GroupUser"));
				string value2 = Function.ToString(gv_Task.GetRowCellValue(e.RowHandle, "GroupEmployee"));
				if (string.IsNullOrEmpty(value) && !string.IsNullOrEmpty(value2))
				{
					e.Appearance.ForeColor = Color.FromArgb(125, 88, 0);
				}
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
		DevExpress.XtraGrid.GridFormatRule gridFormatRule = new DevExpress.XtraGrid.GridFormatRule();
		DevExpress.XtraEditors.FormatConditionRuleIconSet formatConditionRuleIconSet = new DevExpress.XtraEditors.FormatConditionRuleIconSet();
		DevExpress.XtraEditors.FormatConditionIconSet formatConditionIconSet = new DevExpress.XtraEditors.FormatConditionIconSet();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon2 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon3 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.FormatConditionIconSetIcon formatConditionIconSetIcon4 = new DevExpress.XtraEditors.FormatConditionIconSetIcon();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.PM.frm_RelatedActivites));
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		this.gc_Priority_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtra_Task = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Task = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Task = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.newTaskToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Task = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_Closed_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_Closed = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumnEx20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_DueDate_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ProjectEntry_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_ProjectWBS_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx21 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx23 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AssignTo_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_CloseDate_Completed = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_Open = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.xtra_Event = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Event = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Event = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.newToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Event = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Bp = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemCheckEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemDateEditEx1 = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.rep_Invite = new DevExpress.XtraEditors.Repository.RepositoryItemCheckedComboBoxEdit();
		this.xtra_Note = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Note = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Note = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.newNoteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Note = new Apzon.Controls.Grid.GridViewEx();
		this.cl_ClgCode = new Apzon.Controls.Grid.GridColumnEx();
		this.cl_Details = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_AssignTo = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtra_Task.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Task).BeginInit();
		this.ctm_Task.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Task).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Closed).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Open).BeginInit();
		this.xtra_Event.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Event).BeginInit();
		this.ctm_Event.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Event).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Bp).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Invite).BeginInit();
		this.xtra_Note.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Note).BeginInit();
		this.ctm_Note.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Note).BeginInit();
		base.SuspendLayout();
		this.gc_Priority_Completed.Caption = " Priority";
		this.gc_Priority_Completed.CurrencyDateField = null;
		this.gc_Priority_Completed.CurrencyField = null;
		this.gc_Priority_Completed.CurrencyFixed = null;
		this.gc_Priority_Completed.CurrencyFixedRate = 0.0;
		this.gc_Priority_Completed.CurrencyRateField = null;
		this.gc_Priority_Completed.FieldName = "Priority";
		this.gc_Priority_Completed.Name = "gc_Priority_Completed";
		this.gc_Priority_Completed.OptionsColumn.FixedWidth = true;
		this.gc_Priority_Completed.OptionsColumn.ReadOnly = true;
		this.gc_Priority_Completed.PrimaryKey = false;
		this.gc_Priority_Completed.UnitField = null;
		this.gc_Priority_Completed.Visible = true;
		this.gc_Priority_Completed.VisibleIndex = 6;
		this.gc_Priority_Completed.Width = 24;
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtra_Task;
		this.xtraTabControl1.Size = new System.Drawing.Size(971, 355);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[3] { this.xtra_Task, this.xtra_Event, this.xtra_Note });
		this.xtra_Task.Controls.Add(this.grd_Task);
		this.xtra_Task.EditControl = null;
		this.xtra_Task.EditValue = "Task List";
		this.xtra_Task.FieldName = null;
		this.xtra_Task.IsSearchControl = false;
		this.xtra_Task.Name = "xtra_Task";
		this.xtra_Task.ReadOnly = false;
		this.xtra_Task.SettingInVisible = false;
		this.xtra_Task.SettingReadonly = false;
		this.xtra_Task.Size = new System.Drawing.Size(969, 330);
		this.xtra_Task.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Task.Text = "Task List";
		this.grd_Task.AllowPasteExcel = true;
		this.grd_Task.ClipboardData = null;
		this.grd_Task.ContextMenuStrip = this.ctm_Task;
		this.grd_Task.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Task.Location = new System.Drawing.Point(0, 0);
		this.grd_Task.LookAndFeel.UseDefaultLookAndFeel = false;
		this.grd_Task.MainView = this.gv_Task;
		this.grd_Task.Name = "grd_Task";
		this.grd_Task.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_bte_Open, this.rep_bte_Closed });
		this.grd_Task.Size = new System.Drawing.Size(969, 330);
		this.grd_Task.TabIndex = 20;
		this.grd_Task.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Task });
		this.ctm_Task.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.newTaskToolStripMenuItem });
		this.ctm_Task.Name = "ctm_Task";
		this.ctm_Task.Size = new System.Drawing.Size(124, 26);
		this.newTaskToolStripMenuItem.Name = "newTaskToolStripMenuItem";
		this.newTaskToolStripMenuItem.Size = new System.Drawing.Size(123, 22);
		this.newTaskToolStripMenuItem.Text = "New Task";
		this.newTaskToolStripMenuItem.Click += new System.EventHandler(this.newTaskToolStripMenuItem_Click);
		this.gv_Task.Appearance.FocusedRow.BackColor = System.Drawing.Color.DarkSeaGreen;
		this.gv_Task.Appearance.FocusedRow.BorderColor = System.Drawing.Color.DarkSeaGreen;
		this.gv_Task.Appearance.Row.ForeColor = System.Drawing.Color.FromArgb(47, 46, 68);
		this.gv_Task.Appearance.Row.Options.UseForeColor = true;
		this.gv_Task.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.gv_Task.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Task.ColumnKey = null;
		this.gv_Task.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[12]
		{
			this.gridColumnEx12, this.gc_Closed_Completed, this.gridColumnEx20, this.gc_DueDate_Completed, this.gc_ProjectEntry_Completed, this.gc_ProjectWBS_Completed, this.gridColumnEx21, this.gridColumnEx23, this.gridColumnEx22, this.gc_AssignTo_Completed,
			this.gc_CloseDate_Completed, this.gc_Priority_Completed
		});
		this.gv_Task.DisableFormSetting = false;
		this.gv_Task.DisplayException = false;
		this.gv_Task.EditColumn = null;
		this.gv_Task.FieldCurrency = null;
		this.gv_Task.FieldRate = null;
		gridFormatRule.Column = this.gc_Priority_Completed;
		gridFormatRule.ColumnApplyTo = this.gc_Priority_Completed;
		gridFormatRule.Name = "Format0";
		formatConditionIconSet.CategoryName = "Shapes";
		formatConditionIconSetIcon.PredefinedName = "RedToBlack4_1.png";
		formatConditionIconSetIcon.Value = new decimal(new int[4] { 3, 0, 0, 0 });
		formatConditionIconSetIcon.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon2.PredefinedName = "TrafficLights23_2.png";
		formatConditionIconSetIcon2.Value = new decimal(new int[4] { 2, 0, 0, 0 });
		formatConditionIconSetIcon2.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon3.PredefinedName = "RedToBlack4_3.png";
		formatConditionIconSetIcon3.Value = new decimal(new int[4] { 1, 0, 0, 0 });
		formatConditionIconSetIcon3.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSetIcon4.PredefinedName = "Signs3_1.png";
		formatConditionIconSetIcon4.ValueComparison = DevExpress.XtraEditors.FormatConditionComparisonType.GreaterOrEqual;
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon2);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon3);
		formatConditionIconSet.Icons.Add(formatConditionIconSetIcon4);
		formatConditionIconSet.Name = "RedToBlack";
		formatConditionRuleIconSet.IconSet = formatConditionIconSet;
		gridFormatRule.Rule = formatConditionRuleIconSet;
		this.gv_Task.FormatRules.Add(gridFormatRule);
		this.gv_Task.FormId = "oRelatedActivities";
		this.gv_Task.GridControl = this.grd_Task;
		this.gv_Task.IgnoreFilter = false;
		this.gv_Task.IndexEditRow = -1;
		this.gv_Task.IndicatorWidth = 40;
		this.gv_Task.IsApzTable = false;
		this.gv_Task.IsInsertNewRow = false;
		this.gv_Task.IsLoadingData = false;
		this.gv_Task.IsMappingContextAction = false;
		this.gv_Task.ItemId = "OCLG";
		this.gv_Task.LstColumnAllowBatch = null;
		this.gv_Task.LstColumnNotAllowBatch = null;
		this.gv_Task.LstColumnNotEditable = null;
		this.gv_Task.LstColumNotSetting = null;
		this.gv_Task.Name = "gv_Task";
		this.gv_Task.NewItemRowText = "New Task";
		this.gv_Task.OptionsBehavior.Editable = false;
		this.gv_Task.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Task.OptionsView.ColumnAutoWidth = false;
		this.gv_Task.OptionsView.ShowGroupPanel = false;
		this.gv_Task.OptionsView.ShowHorizontalLines = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Task.OptionsView.ShowVerticalLines = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Task.UseBatchMod = false;
		this.gv_Task.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Task_RowCellStyle);
		this.gv_Task.CustomRowCellEdit += new DevExpress.XtraGrid.Views.Grid.CustomRowCellEditEventHandler(this.gv_Task_CustomRowCellEdit);
		this.gv_Task.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gv_Task_CustomUnboundColumnData);
		this.gridColumnEx12.Caption = "Code";
		this.gridColumnEx12.CurrencyDateField = null;
		this.gridColumnEx12.CurrencyField = null;
		this.gridColumnEx12.CurrencyFixed = null;
		this.gridColumnEx12.CurrencyFixedRate = 0.0;
		this.gridColumnEx12.CurrencyRateField = null;
		this.gridColumnEx12.FieldName = "ClgCode";
		this.gridColumnEx12.Name = "gridColumnEx12";
		this.gridColumnEx12.OptionsColumn.ReadOnly = true;
		this.gridColumnEx12.PrimaryKey = false;
		this.gridColumnEx12.UnitField = null;
		this.gc_Closed_Completed.Caption = " Closed";
		this.gc_Closed_Completed.ColumnEdit = this.rep_bte_Closed;
		this.gc_Closed_Completed.CurrencyDateField = null;
		this.gc_Closed_Completed.CurrencyField = null;
		this.gc_Closed_Completed.CurrencyFixed = null;
		this.gc_Closed_Completed.CurrencyFixedRate = 0.0;
		this.gc_Closed_Completed.CurrencyRateField = null;
		this.gc_Closed_Completed.FieldName = "Closed";
		this.gc_Closed_Completed.Name = "gc_Closed_Completed";
		this.gc_Closed_Completed.OptionsColumn.AllowSize = false;
		this.gc_Closed_Completed.OptionsColumn.FixedWidth = true;
		this.gc_Closed_Completed.OptionsColumn.ShowCaption = false;
		this.gc_Closed_Completed.OptionsColumn.TabStop = false;
		this.gc_Closed_Completed.PrimaryKey = false;
		this.gc_Closed_Completed.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gc_Closed_Completed.UnitField = null;
		this.gc_Closed_Completed.Visible = true;
		this.gc_Closed_Completed.VisibleIndex = 0;
		this.gc_Closed_Completed.Width = 24;
		this.rep_bte_Closed.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.completed;
		this.rep_bte_Closed.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_Closed.Name = "rep_bte_Closed";
		this.rep_bte_Closed.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.gridColumnEx20.Caption = "Details";
		this.gridColumnEx20.CurrencyDateField = null;
		this.gridColumnEx20.CurrencyField = null;
		this.gridColumnEx20.CurrencyFixed = null;
		this.gridColumnEx20.CurrencyFixedRate = 0.0;
		this.gridColumnEx20.CurrencyRateField = null;
		this.gridColumnEx20.FieldName = "Details";
		this.gridColumnEx20.Name = "gridColumnEx20";
		this.gridColumnEx20.OptionsColumn.ReadOnly = true;
		this.gridColumnEx20.PrimaryKey = false;
		this.gridColumnEx20.UnitField = null;
		this.gridColumnEx20.Visible = true;
		this.gridColumnEx20.VisibleIndex = 1;
		this.gridColumnEx20.Width = 677;
		this.gc_DueDate_Completed.AppearanceCell.Options.UseTextOptions = true;
		this.gc_DueDate_Completed.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Far;
		this.gc_DueDate_Completed.Caption = "Due Date";
		this.gc_DueDate_Completed.CurrencyDateField = null;
		this.gc_DueDate_Completed.CurrencyField = null;
		this.gc_DueDate_Completed.CurrencyFixed = null;
		this.gc_DueDate_Completed.CurrencyFixedRate = 0.0;
		this.gc_DueDate_Completed.CurrencyRateField = null;
		this.gc_DueDate_Completed.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gc_DueDate_Completed.FieldName = "endDate";
		this.gc_DueDate_Completed.Name = "gc_DueDate_Completed";
		this.gc_DueDate_Completed.OptionsColumn.AllowEdit = false;
		this.gc_DueDate_Completed.OptionsColumn.AllowFocus = false;
		this.gc_DueDate_Completed.OptionsColumn.FixedWidth = true;
		this.gc_DueDate_Completed.OptionsColumn.ReadOnly = true;
		this.gc_DueDate_Completed.OptionsColumn.TabStop = false;
		this.gc_DueDate_Completed.PrimaryKey = false;
		this.gc_DueDate_Completed.UnitField = null;
		this.gc_DueDate_Completed.Visible = true;
		this.gc_DueDate_Completed.VisibleIndex = 4;
		this.gc_DueDate_Completed.Width = 60;
		this.gc_ProjectEntry_Completed.AppearanceCell.Options.UseTextOptions = true;
		this.gc_ProjectEntry_Completed.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gc_ProjectEntry_Completed.Caption = "Project ";
		this.gc_ProjectEntry_Completed.CurrencyDateField = null;
		this.gc_ProjectEntry_Completed.CurrencyField = null;
		this.gc_ProjectEntry_Completed.CurrencyFixed = null;
		this.gc_ProjectEntry_Completed.CurrencyFixedRate = 0.0;
		this.gc_ProjectEntry_Completed.CurrencyRateField = null;
		this.gc_ProjectEntry_Completed.FieldName = "ProjectEntry";
		this.gc_ProjectEntry_Completed.Name = "gc_ProjectEntry_Completed";
		this.gc_ProjectEntry_Completed.OptionsColumn.FixedWidth = true;
		this.gc_ProjectEntry_Completed.OptionsColumn.ReadOnly = true;
		this.gc_ProjectEntry_Completed.PrimaryKey = false;
		this.gc_ProjectEntry_Completed.UnitField = null;
		this.gc_ProjectWBS_Completed.AppearanceCell.Options.UseTextOptions = true;
		this.gc_ProjectWBS_Completed.AppearanceCell.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.gc_ProjectWBS_Completed.Caption = "WBS";
		this.gc_ProjectWBS_Completed.CurrencyDateField = null;
		this.gc_ProjectWBS_Completed.CurrencyField = null;
		this.gc_ProjectWBS_Completed.CurrencyFixed = null;
		this.gc_ProjectWBS_Completed.CurrencyFixedRate = 0.0;
		this.gc_ProjectWBS_Completed.CurrencyRateField = null;
		this.gc_ProjectWBS_Completed.FieldName = "ProjectWBS";
		this.gc_ProjectWBS_Completed.Name = "gc_ProjectWBS_Completed";
		this.gc_ProjectWBS_Completed.OptionsColumn.FixedWidth = true;
		this.gc_ProjectWBS_Completed.OptionsColumn.ReadOnly = true;
		this.gc_ProjectWBS_Completed.PrimaryKey = false;
		this.gc_ProjectWBS_Completed.UnitField = null;
		this.gc_ProjectWBS_Completed.Visible = true;
		this.gc_ProjectWBS_Completed.VisibleIndex = 2;
		this.gc_ProjectWBS_Completed.Width = 100;
		this.gridColumnEx21.Caption = "OrderLine";
		this.gridColumnEx21.CurrencyDateField = null;
		this.gridColumnEx21.CurrencyField = null;
		this.gridColumnEx21.CurrencyFixed = null;
		this.gridColumnEx21.CurrencyFixedRate = 0.0;
		this.gridColumnEx21.CurrencyRateField = null;
		this.gridColumnEx21.FieldName = "OrderLine";
		this.gridColumnEx21.Name = "gridColumnEx21";
		this.gridColumnEx21.OptionsColumn.ReadOnly = true;
		this.gridColumnEx21.PrimaryKey = false;
		this.gridColumnEx21.UnitField = null;
		this.gridColumnEx23.Caption = "Create Date";
		this.gridColumnEx23.CurrencyDateField = null;
		this.gridColumnEx23.CurrencyField = null;
		this.gridColumnEx23.CurrencyFixed = null;
		this.gridColumnEx23.CurrencyFixedRate = 0.0;
		this.gridColumnEx23.CurrencyRateField = null;
		this.gridColumnEx23.FieldName = "CntctDate";
		this.gridColumnEx23.Name = "gridColumnEx23";
		this.gridColumnEx23.OptionsColumn.ReadOnly = true;
		this.gridColumnEx23.PrimaryKey = false;
		this.gridColumnEx23.UnitField = null;
		this.gridColumnEx22.Caption = "End Time";
		this.gridColumnEx22.CurrencyDateField = null;
		this.gridColumnEx22.CurrencyField = null;
		this.gridColumnEx22.CurrencyFixed = null;
		this.gridColumnEx22.CurrencyFixedRate = 0.0;
		this.gridColumnEx22.CurrencyRateField = null;
		this.gridColumnEx22.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.Time;
		this.gridColumnEx22.FieldName = "ENDTime";
		this.gridColumnEx22.Name = "gridColumnEx22";
		this.gridColumnEx22.OptionsColumn.ReadOnly = true;
		this.gridColumnEx22.PrimaryKey = false;
		this.gridColumnEx22.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridColumnEx22.UnitField = null;
		this.gridColumnEx22.Visible = true;
		this.gridColumnEx22.VisibleIndex = 5;
		this.gridColumnEx22.Width = 178;
		this.gc_AssignTo_Completed.Caption = "AssignTo";
		this.gc_AssignTo_Completed.CurrencyDateField = null;
		this.gc_AssignTo_Completed.CurrencyField = null;
		this.gc_AssignTo_Completed.CurrencyFixed = null;
		this.gc_AssignTo_Completed.CurrencyFixedRate = 0.0;
		this.gc_AssignTo_Completed.CurrencyRateField = null;
		this.gc_AssignTo_Completed.FieldName = "AssignTo";
		this.gc_AssignTo_Completed.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_AssignTo_Completed.Name = "gc_AssignTo_Completed";
		this.gc_AssignTo_Completed.OptionsColumn.FixedWidth = true;
		this.gc_AssignTo_Completed.OptionsColumn.ReadOnly = true;
		this.gc_AssignTo_Completed.PrimaryKey = false;
		this.gc_AssignTo_Completed.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gc_AssignTo_Completed.UnitField = null;
		this.gc_AssignTo_Completed.Visible = true;
		this.gc_AssignTo_Completed.VisibleIndex = 3;
		this.gc_AssignTo_Completed.Width = 120;
		this.gc_CloseDate_Completed.Caption = "gridColumnEx27";
		this.gc_CloseDate_Completed.CurrencyDateField = null;
		this.gc_CloseDate_Completed.CurrencyField = null;
		this.gc_CloseDate_Completed.CurrencyFixed = null;
		this.gc_CloseDate_Completed.CurrencyFixedRate = 0.0;
		this.gc_CloseDate_Completed.CurrencyRateField = null;
		this.gc_CloseDate_Completed.FieldName = "CloseDate";
		this.gc_CloseDate_Completed.Name = "gc_CloseDate_Completed";
		this.gc_CloseDate_Completed.OptionsColumn.ReadOnly = true;
		this.gc_CloseDate_Completed.PrimaryKey = false;
		this.gc_CloseDate_Completed.UnitField = null;
		this.rep_bte_Open.AutoHeight = false;
		editorButtonImageOptions2.Image = Apzon.Forms.Properties.Resources.incompleted;
		this.rep_bte_Open.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "Open", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_Open.Name = "rep_bte_Open";
		this.rep_bte_Open.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.HideTextEditor;
		this.xtra_Event.Controls.Add(this.grd_Event);
		this.xtra_Event.EditControl = null;
		this.xtra_Event.EditValue = "Events";
		this.xtra_Event.FieldName = null;
		this.xtra_Event.IsSearchControl = false;
		this.xtra_Event.Name = "xtra_Event";
		this.xtra_Event.ReadOnly = false;
		this.xtra_Event.SettingInVisible = false;
		this.xtra_Event.SettingReadonly = false;
		this.xtra_Event.Size = new System.Drawing.Size(969, 330);
		this.xtra_Event.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Event.Text = "Events";
		this.grd_Event.AllowPasteExcel = false;
		this.grd_Event.ClipboardData = null;
		this.grd_Event.ContextMenuStrip = this.ctm_Event;
		this.grd_Event.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Event.Location = new System.Drawing.Point(0, 0);
		this.grd_Event.MainView = this.gv_Event;
		this.grd_Event.Name = "grd_Event";
		this.grd_Event.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[5] { this.repositoryItemDateEditEx1, this.repositoryItemCheckEdit1, this.rep_Invite, this.repositoryItemDateEdit1, this.rep_Bp });
		this.grd_Event.Size = new System.Drawing.Size(969, 330);
		this.grd_Event.TabIndex = 5;
		this.grd_Event.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Event });
		this.ctm_Event.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.newToolStripMenuItem });
		this.ctm_Event.Name = "ctm_Event";
		this.ctm_Event.Size = new System.Drawing.Size(131, 26);
		this.newToolStripMenuItem.Name = "newToolStripMenuItem";
		this.newToolStripMenuItem.Size = new System.Drawing.Size(130, 22);
		this.newToolStripMenuItem.Text = "New Event";
		this.newToolStripMenuItem.Click += new System.EventHandler(this.newToolStripMenuItem_Click);
		this.gv_Event.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Event.ColumnKey = "ClgCode";
		this.gv_Event.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[8] { this.gridColumnEx7, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx5, this.gridColumnEx6, this.gridColumnEx8, this.gridColumnEx9 });
		this.gv_Event.DisableFormSetting = false;
		this.gv_Event.DisplayException = false;
		this.gv_Event.EditColumn = null;
		this.gv_Event.FieldCurrency = null;
		this.gv_Event.FieldRate = null;
		this.gv_Event.FormId = "oRelatedActivities";
		this.gv_Event.GridControl = this.grd_Event;
		this.gv_Event.IgnoreFilter = false;
		this.gv_Event.IndexEditRow = -1;
		this.gv_Event.IndicatorWidth = 30;
		this.gv_Event.IsApzTable = false;
		this.gv_Event.IsInsertNewRow = true;
		this.gv_Event.IsLoadingData = false;
		this.gv_Event.IsMappingContextAction = false;
		this.gv_Event.ItemId = "OEVENT";
		this.gv_Event.LstColumnAllowBatch = null;
		this.gv_Event.LstColumnNotAllowBatch = null;
		this.gv_Event.LstColumnNotEditable = null;
		this.gv_Event.LstColumNotSetting = null;
		this.gv_Event.Name = "gv_Event";
		this.gv_Event.OptionsView.ShowGroupPanel = false;
		this.gv_Event.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumnEx7, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Event.UseBatchMod = false;
		this.gridColumnEx7.Caption = "ClgCode";
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "ClgCode";
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.OptionsColumn.ReadOnly = true;
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx2.Caption = "Add Event";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "Details";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.OptionsColumn.ReadOnly = true;
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 0;
		this.gridColumnEx2.Width = 130;
		this.gridColumnEx3.Caption = "BP Code";
		this.gridColumnEx3.ColumnEdit = this.rep_Bp;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "CardCode";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowOnlyInEditor;
		this.gridColumnEx3.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx3.Width = 120;
		this.rep_Bp.AutoHeight = false;
		this.rep_Bp.AutoLoadDataSource = true;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.rep_Bp.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_Bp.ContainerControl = this;
		this.rep_Bp.DisplayMember = "CardCode";
		this.rep_Bp.DisplayMember2 = "";
		this.rep_Bp.LoadDataSourceWhenEnter = true;
		this.rep_Bp.Name = "rep_Bp";
		this.rep_Bp.ReadOnly = true;
		this.rep_Bp.SeparateCharacterForMulti = ';';
		this.rep_Bp.SeparateString = ", ";
		this.rep_Bp.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.Bp;
		this.rep_Bp.TypeDrillDown = Apzon.Commons.ApzObjectTypes.oBusinessPartners;
		this.rep_Bp.UsingAutoCompleteBox = false;
		this.rep_Bp.UsingDataSource = true;
		this.rep_Bp.ValidateWhenNoDataSource = false;
		this.rep_Bp.ValueMember = "CardCode";
		this.gridColumnEx4.Caption = "From Date";
		this.gridColumnEx4.ColumnEdit = this.repositoryItemDateEdit1;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx4.FieldName = "Recontact";
		this.gridColumnEx4.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.DateRange;
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.DateSmart;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
		this.gridColumnEx4.Width = 120;
		this.repositoryItemDateEdit1.AutoHeight = false;
		this.repositoryItemDateEdit1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEdit1.Name = "repositoryItemDateEdit1";
		this.gridColumnEx5.Caption = "To Date";
		this.gridColumnEx5.ColumnEdit = this.repositoryItemDateEdit1;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx5.FieldName = "endDate";
		this.gridColumnEx5.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.DateRange;
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.OptionsColumn.ReadOnly = true;
		this.gridColumnEx5.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.DateSmart;
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 3;
		this.gridColumnEx5.Width = 120;
		this.gridColumnEx6.Caption = "All Day";
		this.gridColumnEx6.ColumnEdit = this.repositoryItemCheckEdit1;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "AllDayEvent";
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.OptionsColumn.ReadOnly = true;
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 4;
		this.gridColumnEx6.Width = 120;
		this.repositoryItemCheckEdit1.AutoHeight = false;
		this.repositoryItemCheckEdit1.Name = "repositoryItemCheckEdit1";
		this.gridColumnEx8.Caption = "Invite";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "AssignTo";
		this.gridColumnEx8.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.OptionsColumn.ReadOnly = true;
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx8.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 5;
		this.gridColumnEx8.Width = 124;
		this.gridColumnEx9.Caption = "Create Date";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "CntctDate";
		this.gridColumnEx9.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.DateRange;
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.OptionsColumn.ReadOnly = true;
		this.gridColumnEx9.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.DateSmart;
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx9.UnitField = null;
		this.repositoryItemDateEditEx1.AutoHeight = false;
		this.repositoryItemDateEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemDateEditEx1.DisplayFormat.FormatString = "";
		this.repositoryItemDateEditEx1.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.EditFormat.FormatString = "";
		this.repositoryItemDateEditEx1.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.repositoryItemDateEditEx1.IsSearchControl = false;
		this.repositoryItemDateEditEx1.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.None;
		this.repositoryItemDateEditEx1.Name = "repositoryItemDateEditEx1";
		this.rep_Invite.AutoHeight = false;
		this.rep_Invite.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Invite.Name = "rep_Invite";
		this.xtra_Note.Controls.Add(this.grd_Note);
		this.xtra_Note.EditControl = null;
		this.xtra_Note.EditValue = "Notes";
		this.xtra_Note.FieldName = null;
		this.xtra_Note.IsSearchControl = false;
		this.xtra_Note.Name = "xtra_Note";
		this.xtra_Note.ReadOnly = false;
		this.xtra_Note.SettingInVisible = false;
		this.xtra_Note.SettingReadonly = false;
		this.xtra_Note.Size = new System.Drawing.Size(969, 330);
		this.xtra_Note.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtra_Note.Text = "Notes";
		this.grd_Note.AllowPasteExcel = false;
		this.grd_Note.ClipboardData = null;
		this.grd_Note.ContextMenuStrip = this.ctm_Note;
		this.grd_Note.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Note.Location = new System.Drawing.Point(0, 0);
		this.grd_Note.MainView = this.gv_Note;
		this.grd_Note.Name = "grd_Note";
		this.grd_Note.Size = new System.Drawing.Size(969, 330);
		this.grd_Note.TabIndex = 5;
		this.grd_Note.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Note });
		this.ctm_Note.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.newNoteToolStripMenuItem });
		this.ctm_Note.Name = "ctm_Note";
		this.ctm_Note.Size = new System.Drawing.Size(128, 26);
		this.newNoteToolStripMenuItem.Name = "newNoteToolStripMenuItem";
		this.newNoteToolStripMenuItem.Size = new System.Drawing.Size(127, 22);
		this.newNoteToolStripMenuItem.Text = "New Note";
		this.newNoteToolStripMenuItem.Click += new System.EventHandler(this.newNoteToolStripMenuItem_Click);
		this.gv_Note.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.gv_Note.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Note.ColumnKey = null;
		this.gv_Note.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.cl_ClgCode, this.cl_Details, this.gc_AssignTo, this.gridColumnEx1 });
		this.gv_Note.DisableFormSetting = false;
		this.gv_Note.DisplayException = false;
		this.gv_Note.EditColumn = null;
		this.gv_Note.FieldCurrency = null;
		this.gv_Note.FieldRate = null;
		this.gv_Note.FormId = "oRelatedActivities";
		this.gv_Note.GridControl = this.grd_Note;
		this.gv_Note.IgnoreFilter = false;
		this.gv_Note.IndexEditRow = -1;
		this.gv_Note.IndicatorWidth = 35;
		this.gv_Note.IsApzTable = false;
		this.gv_Note.IsInsertNewRow = false;
		this.gv_Note.IsLoadingData = false;
		this.gv_Note.IsMappingContextAction = false;
		this.gv_Note.ItemId = "ONOTE";
		this.gv_Note.LstColumnAllowBatch = null;
		this.gv_Note.LstColumnNotAllowBatch = null;
		this.gv_Note.LstColumnNotEditable = null;
		this.gv_Note.LstColumNotSetting = null;
		this.gv_Note.Name = "gv_Note";
		this.gv_Note.NewItemRowText = "New Note";
		this.gv_Note.OptionsView.BestFitMaxRowCount = 300;
		this.gv_Note.OptionsView.RowAutoHeight = true;
		this.gv_Note.OptionsView.ShowGroupPanel = false;
		this.gv_Note.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.cl_ClgCode, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Note.UseBatchMod = false;
		this.gv_Note.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Note_RowCellStyle);
		this.gv_Note.CustomUnboundColumnData += new DevExpress.XtraGrid.Views.Base.CustomColumnDataEventHandler(this.gv_Note_CustomUnboundColumnData);
		this.cl_ClgCode.Caption = "ClgCode";
		this.cl_ClgCode.CurrencyDateField = null;
		this.cl_ClgCode.CurrencyField = null;
		this.cl_ClgCode.CurrencyFixed = null;
		this.cl_ClgCode.CurrencyFixedRate = 0.0;
		this.cl_ClgCode.CurrencyRateField = null;
		this.cl_ClgCode.FieldName = "ClgCode";
		this.cl_ClgCode.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.cl_ClgCode.Fixed = DevExpress.XtraGrid.Columns.FixedStyle.Left;
		this.cl_ClgCode.Name = "cl_ClgCode";
		this.cl_ClgCode.OptionsColumn.ReadOnly = true;
		this.cl_ClgCode.PrimaryKey = false;
		this.cl_ClgCode.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.cl_ClgCode.UnitField = null;
		this.cl_ClgCode.Width = 100;
		this.cl_Details.Caption = "Add Note";
		this.cl_Details.CurrencyDateField = null;
		this.cl_Details.CurrencyField = null;
		this.cl_Details.CurrencyFixed = null;
		this.cl_Details.CurrencyFixedRate = 0.0;
		this.cl_Details.CurrencyRateField = null;
		this.cl_Details.FieldName = "Details";
		this.cl_Details.Name = "cl_Details";
		this.cl_Details.OptionsColumn.ReadOnly = true;
		this.cl_Details.PrimaryKey = false;
		this.cl_Details.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.cl_Details.UnitField = null;
		this.cl_Details.Visible = true;
		this.cl_Details.VisibleIndex = 0;
		this.cl_Details.Width = 569;
		this.gc_AssignTo.Caption = "AssignTo";
		this.gc_AssignTo.CurrencyDateField = null;
		this.gc_AssignTo.CurrencyField = null;
		this.gc_AssignTo.CurrencyFixed = null;
		this.gc_AssignTo.CurrencyFixedRate = 0.0;
		this.gc_AssignTo.CurrencyRateField = null;
		this.gc_AssignTo.FieldName = "AssignTo";
		this.gc_AssignTo.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gc_AssignTo.Name = "gc_AssignTo";
		this.gc_AssignTo.OptionsColumn.ReadOnly = true;
		this.gc_AssignTo.PrimaryKey = false;
		this.gc_AssignTo.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gc_AssignTo.UnboundType = DevExpress.Data.UnboundColumnType.String;
		this.gc_AssignTo.UnitField = null;
		this.gc_AssignTo.Visible = true;
		this.gc_AssignTo.VisibleIndex = 1;
		this.gc_AssignTo.Width = 201;
		this.gridColumnEx1.Caption = "Note Date";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx1.FieldName = "Recontact";
		this.gridColumnEx1.GroupInterval = DevExpress.XtraGrid.ColumnGroupInterval.DateRange;
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.OptionsColumn.ReadOnly = true;
		this.gridColumnEx1.OptionsFilter.FilterPopupMode = DevExpress.XtraGrid.Columns.FilterPopupMode.DateSmart;
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.SortMode = DevExpress.XtraGrid.ColumnSortMode.Custom;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 2;
		this.gridColumnEx1.Width = 100;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(971, 355);
		base.Controls.Add(this.xtraTabControl1);
		base.FormId = "oRelatedActivities";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_RelatedActivites";
		this.Text = "Related Activites";
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtra_Task.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Task).EndInit();
		this.ctm_Task.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Task).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Closed).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Open).EndInit();
		this.xtra_Event.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Event).EndInit();
		this.ctm_Event.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Event).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Bp).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCheckEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemDateEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Invite).EndInit();
		this.xtra_Note.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Note).EndInit();
		this.ctm_Note.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Note).EndInit();
		base.ResumeLayout(false);
	}
}
