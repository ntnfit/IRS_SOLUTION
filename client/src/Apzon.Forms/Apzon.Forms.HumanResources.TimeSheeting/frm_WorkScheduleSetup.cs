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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_WorkScheduleSetup : XtraFormEx
{
	public readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public GridViewMenu _menu;

	private bool _isLoading;

	private RepositoryItemDateEditEx _repNoYear;

	private RepositoryItemCheckedComboBoxEdit _repWorkShift = new RepositoryItemCheckedComboBoxEdit();

	private DataTable _dtShift;

	private DataTable _dtWeek = new DataTable();

	private DataTable _dtMonth = new DataTable();

	private DataTable _dtYear = new DataTable();

	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SplitContainerControl splitContainerControl1;

	private GridColumnEx gridColumnEx1;

	private TextEditEx txt_Dscription;

	private LabelLine labelLine2;

	private TextEditEx txt_SchName;

	private LabelLine labelLine3;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx3;

	private LabelLine labelLine9;

	private RadioGroupEx rad_CycleMode;

	private GridControlEx grd_Line;

	private GridViewEx gv_Week;

	private GridColumnEx gridColumnEx4;

	private LookUpEditEx luk_HldCode;

	private LabelLine labelLine15;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Ok;

	private ContextMenuStrip ctm_Doc;

	private ToolStripMenuItem addToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private GridColumnEx gridColumnEx5;

	private GridColumnEx gridColumnEx8;

	private GridViewEx gv_Month;

	private GridColumnEx gridColumnEx6;

	private GridViewEx gv_Year;

	private GridColumnEx gridColumnEx7;

	private GridColumnEx gridColumnEx9;

	private GridColumnEx gridColumnEx10;

	private LookUpEditEx luk_Branch;

	private LabelLine labelLine4;

	private RepositoryItemLookUpEdit rep_Branch;

	private RepositoryItemLookUpEdit rep_WeekDay;

	private RepositoryItemLookUpEdit rep_DayInMonth;

	private RepositoryItemLookUpEdit rep_DayInYear;

	private LookUpEditEx luk_InOutID;

	private LabelLine labelLine1;

	private GridColumnEx gridColumnEx11;

	private RepositoryItemLookUpEdit rep_Cycle;

	private LookUpEditEx luk_CalcType;

	private LabelLine labelLine5;

	private CalcEditEx calcEditEx1;

	private LabelLine labelLine20;

	private SimpleButtonEx btn_AddWorkShift;

	public static string hldCode { get; set; }

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
			btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Ok.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public async Task REFRESH()
	{
		AddNewDoc();
	}

	public async Task REFRESH_DATA()
	{
		await LoadData();
	}

	public frm_WorkScheduleSetup()
	{
		InitializeComponent();
		FormId = "oWorkScheduleSetup";
		gv_Doc.FormId = FormId;
		gv_Doc.ItemId = "APZ_OWSC";
		gv_Week.FormId = FormId;
		gv_Week.ItemId = "APZ_WSC1";
		gv_Month.FormId = FormId;
		gv_Month.ItemId = "APZ_WSC2";
		gv_Year.FormId = FormId;
		gv_Year.ItemId = "APZ_WSC3";
		OnLoadAsync += frm_WorkScheduleSetup_Load;
		gv_Doc.BeforeLeaveRow += gv_Doc_BeforeLeaveRow;
		gv_Doc.ShowingEditor += GvDocOnShowingEditor;
		gv_Year.PopupMenuShowing += gv_Week_PopupMenuShowing;
		gv_Month.PopupMenuShowing += gv_Week_PopupMenuShowing;
		gv_Week.ShowingEditor += GvLineOnShowingEditor;
		gv_Month.ShowingEditor += GvLineOnShowingEditor;
		gv_Year.ShowingEditor += GvLineOnShowingEditor;
		gv_Week.CustomRowCellEdit += gv_Line_CustomRowCellEdit;
		gv_Month.CustomRowCellEdit += gv_Line_CustomRowCellEdit;
		gv_Year.CustomRowCellEdit += gv_Line_CustomRowCellEdit;
	}

	private async Task frm_WorkScheduleSetup_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			await LoadInitForm();
			await LoadData();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 3;
		try
		{
			DataTable dt = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			luk_Branch.SetDataSource(dt, "Name", "Code", emptyRow: true, emptyTop: true, hideOtherColumns: true);
			rep_Branch.Assign(luk_Branch.Properties);
			DataTable dt2 = await _httpClient.HolidaySetupClient.GetAll();
			luk_HldCode.SetDataSource(dt2, "HldName", "HldCode", emptyRow: true, emptyTop: true, hideOtherColumns: true);
			DataTable dt3 = await _httpClient.InOutMethodClient.GetAll();
			luk_InOutID.SetDataSource(dt3, "Code", "MethodId", emptyRow: true, emptyTop: true, hideOtherColumns: true);
			await LoadShift();
			List<Common.Table_Code> list = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "2",
					Name = "Monday"
				},
				new Common.Table_Code
				{
					Code = "3",
					Name = "Tuesday"
				},
				new Common.Table_Code
				{
					Code = "4",
					Name = "Wednesday"
				},
				new Common.Table_Code
				{
					Code = "5",
					Name = "Thursday"
				},
				new Common.Table_Code
				{
					Code = "6",
					Name = "Friday"
				},
				new Common.Table_Code
				{
					Code = "7",
					Name = "Saturday"
				},
				new Common.Table_Code
				{
					Code = "1",
					Name = "Sunday"
				},
				new Common.Table_Code
				{
					Code = "8",
					Name = "Holiday"
				}
			};
			LookUpEditEx lookUpEditEx = new LookUpEditEx();
			lookUpEditEx.SetDataSourceByList(list);
			rep_WeekDay.Assign(lookUpEditEx.Properties);
			List<Common.Table_Code> list2 = new List<Common.Table_Code>();
			for (int i = 0; i < 31; i++)
			{
				list2.Add(new Common.Table_Code
				{
					Code = (i + 1).ToString(),
					Name = "Day " + (i + 1)
				});
			}
			list2.Add(new Common.Table_Code
			{
				Code = "32",
				Name = "Holiday"
			});
			LookUpEditEx lookUpEditEx2 = new LookUpEditEx();
			lookUpEditEx2.SetDataSourceByList(list2);
			rep_DayInMonth.Assign(lookUpEditEx2.Properties);
			new LookUpEditEx();
			_repWorkShift.Buttons[0].Visible = false;
			_repWorkShift.Buttons[0].Enabled = false;
			_repWorkShift.ValueMember = "ShiftID";
			_repWorkShift.DisplayMember = "ShiftCode";
			_repWorkShift.TextEditStyle = TextEditStyles.Standard;
			_repWorkShift.DataSource = _dtShift;
			List<Common.Table_Code> list3 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "W",
					Name = "Week"
				},
				new Common.Table_Code
				{
					Code = "M",
					Name = "Month"
				},
				new Common.Table_Code
				{
					Code = "Y",
					Name = "Year"
				}
			};
			LookUpEditEx lookUpEditEx3 = new LookUpEditEx();
			lookUpEditEx3.SetDataSourceByList(list3);
			rep_Cycle.Assign(lookUpEditEx3.Properties);
			List<Common.Table_Code> list4 = new List<Common.Table_Code>
			{
				new Common.Table_Code
				{
					Code = "1",
					Name = "By shifts"
				},
				new Common.Table_Code
				{
					Code = "2",
					Name = "By total work hours"
				}
			};
			luk_CalcType.SetDataSourceByList(list4);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadShift()
	{
		_dtShift = await _httpClient.WorkShiftClient.GetAll();
	}

	private async Task LoadData()
	{
		_isLoading = true;
		try
		{
			DataTable dataSource = await _httpClient.WorkScheduleSetupClient.GetAll();
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
			grd_Doc.DataSourceTable.AcceptChanges();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	private async Task SearchLine()
	{
		btn_Ok.Enabled = false;
		try
		{
			DataTable dtSchedule = new DataTable
			{
				Columns = 
				{
					{
						"DayID",
						typeof(int)
					},
					{
						"WorkShift",
						typeof(int)
					}
				}
			};
			int schId = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("SchID"));
			string cycleMode = Function.ToString(gv_Doc.GetFocusedRowCellValue("CycleMode"));
			DataSet dataSet = await _httpClient.WorkScheduleSetupClient.GetLine(schId);
			if (dataSet.Tables.Count >= 3)
			{
				DataTable dataTable = dtSchedule.Clone();
				if (dataSet.Tables[0].IsNotNull())
				{
					List<object> list = (from t in dataSet.Tables[0].AsEnumerable()
						where Function.ParseInt(t["WorkShift"]) != 0
						select t["WorkShift"]).Distinct().ToList();
					dataTable.Rows.Clear();
					if (list != null && list.Count > 0)
					{
						foreach (object item in list)
						{
							int i;
							for (i = 0; i < 8; i++)
							{
								object obj = (from t in dataSet.Tables[0].AsEnumerable()
									where Function.ParseInt(t["DayID"]) == i + 1 && Function.ParseInt(t["WorkShift"]).Equals(item)
									select t["WorkShift"]).FirstOrDefault();
								if (Function.ParseInt(obj) == 0)
								{
									dataTable.Rows.Add(i + 1, DBNull.Value);
								}
								else
								{
									dataTable.Rows.Add(i + 1, Function.ParseInt(obj));
								}
							}
						}
					}
				}
				_dtWeek = dataTable;
				DataTable dataTable2 = dtSchedule.Clone();
				if (!dataSet.Tables[1].IsNotNull())
				{
					dataTable2.Rows.Clear();
					for (int num = 0; num < 32; num++)
					{
						dataTable2.Rows.Add(num + 1, DBNull.Value);
					}
				}
				DataTable table = dataSet.Tables[1].Copy();
				dataTable2.Merge(table);
				_dtMonth = dataTable2;
				if (dataSet.Tables[2].IsNotNull())
				{
					_dtYear = dataSet.Tables[2];
				}
				else
				{
					dtSchedule.Rows.Clear();
					_dtYear = dtSchedule.Copy();
				}
			}
			grd_Line.RefreshDataSource();
			switch (cycleMode)
			{
			case "W":
				grd_Line.DataSource = _dtWeek;
				grd_Line.MainView = gv_Week;
				break;
			case "M":
				grd_Line.DataSource = _dtMonth;
				grd_Line.MainView = gv_Month;
				break;
			case "Y":
				grd_Line.DataSource = _dtYear;
				grd_Line.MainView = gv_Year;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		btn_Ok.Enabled = true;
	}

	private void GvLineOnShowingEditor(object sender, CancelEventArgs e)
	{
		GridView gridView = (GridView)sender;
		if (gridView != null && gridView.FocusedColumn.FieldName == "DayID")
		{
			e.Cancel = true;
		}
	}

	private void GvDocOnShowingEditor(object sender, CancelEventArgs e)
	{
		e.Cancel = true;
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			btn_Ok.Enabled = false;
			await SaveData();
			btn_Ok.Enabled = true;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private void addToolStripMenuItem_Click(object sender, EventArgs e)
	{
		AddNewDoc();
	}

	private async void removeToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			btn_Ok.Enabled = false;
			try
			{
				int focusedRowHandle = gv_Doc.FocusedRowHandle;
				int num = Function.ParseInt(gv_Doc.GetRowCellValue(focusedRowHandle, "SchID"));
				if (num > 0)
				{
					string text = await _httpClient.WorkScheduleSetupClient.Remove(num);
					if (string.IsNullOrWhiteSpace(text))
					{
						gv_Doc.BeginUpdate();
						gv_Doc.DeleteSelectedRows();
						gv_Doc.EndUpdate();
						WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, Apzon.Commons.Properties.Resources.Msg_DeleteError + ": " + text);
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			}
			btn_Ok.Enabled = true;
		}
		catch (Exception ex2)
		{
			WinCommons.ErrorLineNumber(ex2);
			Logging.Write("ERROR", ex2);
		}
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode != Enums.FormViewModeType.Add && FormMode == Enums.FormViewModeType.View)
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

	private void gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode == Enums.FormViewModeType.View)
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

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		try
		{
			_isLoading = true;
			int focusId = Function.ParseInt(gv_Doc.GetRowCellValue(e.FocusedRowHandle, "SchID"));
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (dataSourceTable.IsNotNull())
			{
				DataRow dataRow = dataSourceTable.AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["SchID"]) == focusId);
				if (dataRow != null)
				{
					DataSource = dataRow.ToArray().CopyToDataTable();
				}
			}
			else
			{
				DataSource = WinFuntions.GetTableStructure("APZ_OEWS");
			}
			await SearchLine();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Line_CustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (!(e.Column.FieldName != "WorkShift"))
			{
				e.RepositoryItem = _repWorkShift;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (_isLoading || (FormMode != Enums.FormViewModeType.Add && FormMode != Enums.FormViewModeType.Update))
			{
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				e.Allow = false;
				return;
			}
			if (grd_Doc.DataSource is DataTable dataTable)
			{
				dataTable.RejectChanges();
			}
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Month_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
	}

	private void gv_Year_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
	}

	private void rad_CycleMode_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isLoading)
			{
				switch (Function.ToString(rad_CycleMode.EditValue))
				{
				case "W":
					grd_Line.DataSource = _dtWeek;
					grd_Line.MainView = gv_Week;
					break;
				case "M":
					grd_Line.DataSource = _dtMonth;
					grd_Line.MainView = gv_Month;
					break;
				case "Y":
					grd_Line.DataSource = _dtYear;
					grd_Line.MainView = gv_Year;
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Week_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode != Keys.Delete)
			{
				return;
			}
			gv_Week.BeginUpdate();
			GridCell[] selectedCells = gv_Week.GetSelectedCells();
			foreach (GridCell gridCell in selectedCells)
			{
				if (gridCell.Column.OptionsColumn.AllowEdit && !gridCell.Column.ReadOnly)
				{
					gv_Week.SetRowCellValue(gridCell.RowHandle, gridCell.Column, null);
				}
			}
			gv_Week.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void gv_Week_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			if (e.MenuType == GridMenuType.Row && e.HitInfo.InRow && !e.HitInfo.InGroupRow && (e.HitInfo.Column == null || !(e.HitInfo.Column.FieldName.ToLower() != "workshift")))
			{
				e.Allow = true;
				_menu.Show(e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	protected virtual void MenuGridClick(object sender, EventArgs e)
	{
		try
		{
			if (!(sender is DXMenuItem dXMenuItem))
			{
				return;
			}
			Tuple<string, string> obj = dXMenuItem.Tag as Tuple<string, string>;
			string text = Function.ToString(obj.Item2);
			if (obj == null)
			{
				return;
			}
			GridViewEx gridViewEx = (GridViewEx)grd_Line.MainView;
			GridCell[] selectedCells = ((GridViewEx)grd_Line.MainView).GetSelectedCells();
			gridViewEx.BeginUpdate();
			for (int num = selectedCells.Length - 1; num >= 0; num--)
			{
				GridCell gridCell = selectedCells[num];
				if (gridCell.Column.FieldName.ToLower().Equals("workshift"))
				{
					if (text.Equals("Define New"))
					{
						frm_WorkShift frm_WorkShift2 = new frm_WorkShift
						{
							MdiParent = MdiParent,
							StartPosition = FormStartPosition.CenterScreen
						};
						frm_WorkShift2.Reload = (Func<Task>)Delegate.Combine(frm_WorkShift2.Reload, new Func<Task>(LoadShift));
						frm_WorkShift2.Show();
						gridViewEx.EndUpdate();
						return;
					}
					if (text.Equals("-"))
					{
						gridViewEx.SetRowCellValue(gridCell.RowHandle, gridCell.Column, "");
					}
					else
					{
						gridViewEx.SetRowCellValue(gridCell.RowHandle, gridCell.Column, text);
					}
				}
			}
			gridViewEx.EndUpdate();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_WorkScheduleSetup_OnControlValueChanged(object sender, ControlChangedEventArgs e)
	{
		try
		{
			if (!_isLoading && FormMode != Enums.FormViewModeType.Add && FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private void luk_Branch_EditValueChanged(object sender, EventArgs e)
	{
		CreateContextMenu();
	}

	private async Task SaveData()
	{
		DataSet dataSet = new DataSet();
		try
		{
			if (string.IsNullOrEmpty(calcEditEx1.Text))
			{
				WinCommons.Error(Enums.MessageCode.RoundUnitCannotBeEmpty, "");
			}
			else if (DataSource is DataTable dataTable)
			{
				DataTable dataTable2 = dataTable.Copy();
				dataSet.Tables.Add(dataTable2.Copy());
				dataSet.Tables.Add((_dtWeek.IsNotNull() && (from r in _dtWeek.AsEnumerable()
					where !string.IsNullOrWhiteSpace(Function.ToString(r["WorkShift"]))
					select r).Any()) ? (from r in _dtWeek.AsEnumerable()
					where !string.IsNullOrWhiteSpace(Function.ToString(r["WorkShift"]))
					select r).CopyToDataTable() : _dtWeek.Copy());
				dataSet.Tables.Add((_dtMonth.IsNotNull() && (from r in _dtMonth.AsEnumerable()
					where !string.IsNullOrWhiteSpace(Function.ToString(r["WorkShift"]))
					select r).Any()) ? (from r in _dtMonth.AsEnumerable()
					where !string.IsNullOrWhiteSpace(Function.ToString(r["WorkShift"]))
					select r).CopyToDataTable() : _dtMonth.Copy());
				dataSet.Tables.Add(_dtYear.Copy());
				WinCommons._OpenProcessing();
				string text = await _httpClient.WorkScheduleSetupClient.Save(dataSet);
				WinCommons._CloseProcessing();
				if (string.IsNullOrWhiteSpace(text))
				{
					FormMode = Enums.FormViewModeType.View;
					await LoadData();
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.NoDataToSave, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void AddNewDoc()
	{
		_isLoading = true;
		try
		{
			FormMode = Enums.FormViewModeType.Add;
			btn_Ok.Enabled = true;
			DataTable tableStructure = WinFuntions.GetTableStructure("APZ_OWSC");
			DataRow dataRow = tableStructure.NewRow();
			dataRow["CycleMode"] = "W";
			tableStructure.Rows.Add(dataRow);
			grd_Doc.DataSource = tableStructure;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		_isLoading = false;
	}

	private void btn_AddWorkShift_Click(object sender, EventArgs e)
	{
		try
		{
			string searchtxt = "";
			frm_CFL frm = new frm_CFL(Enums.TypeCFL.UserDefinedData, true, searchtxt, "", Enums.ItemObjectType.NoItemType, null, "");
			DataSet dataSource = GenerateDataSource();
			List<object> list = (from t in grd_Line.DataSourceTable.AsEnumerable()
				where !string.IsNullOrWhiteSpace(Function.ToString(t["WorkShift"]))
				select t["WorkShift"]).Distinct().ToList();
			if (list != null && list.Count > 0)
			{
				for (int num = 0; num < list.Count; num++)
				{
					if (num == list.Count - 1)
					{
						searchtxt += list[num];
					}
					else
					{
						searchtxt = searchtxt + list[num]?.ToString() + ",";
					}
				}
			}
			frm.DataSource = dataSource;
			frm.ButtonEditMultiSelect = true;
			frm_CFL frm_CFL2 = frm;
			frm_CFL2.AfterLoadForm = (Action)Delegate.Combine(frm_CFL2.AfterLoadForm, (Action)(() =>
			{
				frm.gv_CFL.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
				string[] array = searchtxt.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
				GridColumn column = frm.gv_CFL.Columns["ShiftID"];
				frm.gv_CFL.ClearSelection();
				frm.gv_CFL.OptionsBehavior.ImmediateUpdateRowPosition = false;
				string[] array2 = array;
				foreach (string text in array2)
				{
					int rowHandle = frm.gv_CFL.LocateByDisplayText(0, column, text.Trim());
					frm.gv_CFL.SelectRow(rowHandle);
				}
			}));
			frm.ShowDialog(this);
			if (frm.ChooseItems == null || frm.ChooseItems.Count <= 0)
			{
				return;
			}
			DataTable dataTable = _dtWeek.Clone();
			switch (Function.ToString(gv_Doc.GetFocusedRowCellValue("CycleMode")))
			{
			case "W":
				foreach (DataRow chooseItem in frm.ChooseItems)
				{
					for (int num3 = 0; num3 < 8; num3++)
					{
						if (num3 == 0)
						{
							dataTable.Rows.Add(num3 + 1, DBNull.Value);
						}
						else
						{
							dataTable.Rows.Add(num3 + 1, chooseItem["ShiftID"]);
						}
					}
				}
				_dtWeek.Clear();
				_dtWeek = dataTable;
				break;
			case "M":
				foreach (DataRow chooseItem2 in frm.ChooseItems)
				{
					for (int num2 = 0; num2 < 32; num2++)
					{
						dataTable.Rows.Add(num2 + 1, chooseItem2["ShiftID"]);
					}
				}
				_dtMonth.Clear();
				_dtMonth = dataTable;
				break;
			case "Y":
				dataTable = _dtYear;
				break;
			}
			grd_Line.DataSource = dataTable;
			grd_Line.RefreshDataSource();
			grd_Line.DataSourceTable.AcceptChanges();
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private DataSet GenerateDataSource()
	{
		try
		{
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("ColumnName", typeof(string));
			dataTable.Columns.Add("Caption", typeof(string));
			dataTable.Columns.Add("DataType", typeof(string));
			dataTable.Columns.Add("SubType", typeof(string));
			dataTable.Columns.Add("Width", typeof(int));
			dataTable.Columns.Add("GroupIndex", typeof(int));
			dataTable.Columns.Add("Visible", typeof(int));
			dataTable.Columns.Add("FormatType", typeof(string));
			dataTable.Columns.Add("DisplayFormat", typeof(string));
			dataTable.Columns.Add("ReadOnly", typeof(int));
			dataTable.Columns.Add("AllowEdit", typeof(int));
			dataTable.Columns.Add("AllowMove", typeof(int));
			dataTable.Columns.Add("AllowSize", typeof(int));
			dataTable.Columns.Add("AllowSort", typeof(int));
			dataTable.Columns.Add("Fixed", typeof(string));
			dataTable.Columns.Add("SummaryType", typeof(string));
			dataTable.Columns.Add("GroupSummaryType", typeof(string));
			dataTable.Rows.Add("ShiftID", "ShiftID", "nvarchar", "", 80, -1, 1, DBNull.Value, DBNull.Value, 1, 0, 1, 1, 1, "L", DBNull.Value, DBNull.Value);
			dataTable.Rows.Add("ShiftCode", "ShiftCode", "nvarchar", "", 100, -1, 1, DBNull.Value, DBNull.Value, 1, 0, 1, 1, 1, "L", DBNull.Value, DBNull.Value);
			dataTable.Rows.Add("Dscription", "Description", "nvarchar", "", 275, -1, 1, DBNull.Value, DBNull.Value, 1, 0, 1, 1, 1, "L", DBNull.Value, DBNull.Value);
			DataTable dataTable2 = new DataTable();
			DataView defaultView = _dtShift.DefaultView;
			defaultView.RowFilter = "Branch= " + Function.ParseInt(luk_Branch.EditValue);
			dataTable2 = defaultView.ToTable(false, "ShiftID", "ShiftCode", "Dscription").Copy();
			dataSet.Tables.Add(dataTable);
			dataSet.Tables.Add(dataTable2);
			return dataSet;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataSet();
		}
	}

	protected virtual void CreateContextMenu()
	{
		GridViewEx view = (GridViewEx)grd_Line.MainView;
		_menu = new GridViewMenu(view);
		if (_dtShift.HasColumn())
		{
			DataView defaultView = _dtShift.DefaultView;
			defaultView.RowFilter = "Branch= " + Function.ParseInt(luk_Branch.EditValue);
			foreach (DataRow item in defaultView.ToTable().AsEnumerable())
			{
				DXMenuItem dXMenuItem = new DXMenuItem();
				dXMenuItem.Caption = Function.ToString(item["ShiftCode"]) + " - " + Function.ToString(item["Dscription"]);
				dXMenuItem.Click += MenuGridClick;
				dXMenuItem.Tag = new Tuple<string, string>(Function.ToString(item["Dscription"]), Function.ToString(item["ShiftId"]));
				_menu.Items.Add(dXMenuItem);
			}
		}
		DXMenuItem dXMenuItem2 = new DXMenuItem();
		dXMenuItem2.Caption = Apzon.Commons.Properties.Resources.ButtonDelete;
		dXMenuItem2.Click += MenuGridClick;
		dXMenuItem2.Tag = new Tuple<string, string>(Apzon.Commons.Properties.Resources.ButtonDelete, "-");
		_menu.Items.Add(dXMenuItem2);
		DXMenuItem dXMenuItem3 = new DXMenuItem();
		dXMenuItem3.Caption = "Define New";
		dXMenuItem3.Click += MenuGridClick;
		dXMenuItem3.Tag = new Tuple<string, string>("Define New", "Define New");
		_menu.Items.Add(dXMenuItem3);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_WorkScheduleSetup));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.ctm_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Cycle = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_AddWorkShift = new Apzon.Controls.Controls.SimpleButtonEx();
		this.calcEditEx1 = new Apzon.Controls.Editor.CalcEditEx();
		this.labelLine20 = new Apzon.Controls.Controls.LabelLine();
		this.luk_CalcType = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.luk_InOutID = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.luk_Branch = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.luk_HldCode = new Apzon.Controls.Editor.LookUpEditEx();
		this.labelLine15 = new Apzon.Controls.Controls.LabelLine();
		this.grd_Line = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Week = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_WeekDay = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DayInMonth = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.rep_DayInYear = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gv_Year = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx7 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gv_Month = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumnEx6 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rad_CycleMode = new Apzon.Controls.Editor.RadioGroupEx();
		this.labelLine9 = new Apzon.Controls.Controls.LabelLine();
		this.txt_SchName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Dscription = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.ctm_Doc.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Cycle).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.calcEditEx1.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CalcType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_InOutID.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HldCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Week).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WeekDay).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DayInMonth).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DayInYear).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Year).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Month).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rad_CycleMode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SchName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Dscription.Properties).BeginInit();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.ctm_Doc;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_Branch, this.rep_Cycle });
		this.grd_Doc.Size = new System.Drawing.Size(252, 379);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.ctm_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addToolStripMenuItem, this.removeToolStripMenuItem });
		this.ctm_Doc.Name = "ctm_Doc";
		this.ctm_Doc.Size = new System.Drawing.Size(118, 48);
		this.addToolStripMenuItem.Name = "addToolStripMenuItem";
		this.addToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.addToolStripMenuItem.Text = "Add";
		this.addToolStripMenuItem.Click += new System.EventHandler(this.addToolStripMenuItem_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(117, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.removeToolStripMenuItem_Click);
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[5] { this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx3, this.gridColumnEx4, this.gridColumnEx11 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oWorkScheduleSetup";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.HasFormSetting = false;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OESS";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.LstOldUDF = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gridColumnEx1.Caption = "ID";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.FieldName = "SchID";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx2.Caption = "Name";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.FieldName = "SchName";
		this.gridColumnEx2.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 0;
		this.gridColumnEx3.Caption = "Description";
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "Dscription";
		this.gridColumnEx3.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.gridColumnEx3.Visible = true;
		this.gridColumnEx3.VisibleIndex = 1;
		this.gridColumnEx4.Caption = "Branch";
		this.gridColumnEx4.ColumnEdit = this.rep_Branch;
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "branch";
		this.gridColumnEx4.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Visible = true;
		this.gridColumnEx4.VisibleIndex = 2;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.NullText = "";
		this.gridColumnEx11.Caption = "Cycle Mode";
		this.gridColumnEx11.ColumnEdit = this.rep_Cycle;
		this.gridColumnEx11.CurrencyDateField = null;
		this.gridColumnEx11.CurrencyField = null;
		this.gridColumnEx11.CurrencyFixed = null;
		this.gridColumnEx11.CurrencyFixedRate = 0.0;
		this.gridColumnEx11.CurrencyRateField = null;
		this.gridColumnEx11.FieldName = "CycleMode";
		this.gridColumnEx11.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx11.Name = "gridColumnEx11";
		this.gridColumnEx11.PrimaryKey = false;
		this.gridColumnEx11.UnitField = null;
		this.gridColumnEx11.Visible = true;
		this.gridColumnEx11.VisibleIndex = 3;
		this.rep_Cycle.AutoHeight = false;
		this.rep_Cycle.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Cycle.Name = "rep_Cycle";
		this.rep_Cycle.NullText = "";
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_AddWorkShift);
		this.splitContainerControl1.Panel2.Controls.Add(this.calcEditEx1);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine20);
		this.splitContainerControl1.Panel2.Controls.Add(this.luk_CalcType);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine5);
		this.splitContainerControl1.Panel2.Controls.Add(this.luk_InOutID);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine1);
		this.splitContainerControl1.Panel2.Controls.Add(this.luk_Branch);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine4);
		this.splitContainerControl1.Panel2.Controls.Add(this.luk_HldCode);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine15);
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Line);
		this.splitContainerControl1.Panel2.Controls.Add(this.rad_CycleMode);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine9);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_SchName);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine3);
		this.splitContainerControl1.Panel2.Controls.Add(this.txt_Dscription);
		this.splitContainerControl1.Panel2.Controls.Add(this.labelLine2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(850, 379);
		this.splitContainerControl1.SplitterPosition = 252;
		this.splitContainerControl1.TabIndex = 1;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.btn_AddWorkShift.EditControl = null;
		this.btn_AddWorkShift.EditValue = "Choose Work Shift";
		this.btn_AddWorkShift.FieldName = null;
		this.btn_AddWorkShift.ImageOptions.Image = Apzon.Forms.Properties.Resources.add_16x16;
		this.btn_AddWorkShift.IsSearchControl = false;
		this.btn_AddWorkShift.Location = new System.Drawing.Point(2, 108);
		this.btn_AddWorkShift.Name = "btn_AddWorkShift";
		this.btn_AddWorkShift.ReadOnly = false;
		this.btn_AddWorkShift.SettingInVisible = false;
		this.btn_AddWorkShift.SettingReadonly = false;
		this.btn_AddWorkShift.Size = new System.Drawing.Size(139, 20);
		this.btn_AddWorkShift.TabIndex = 48;
		this.btn_AddWorkShift.Text = "Choose Work Shift";
		this.btn_AddWorkShift.Click += new System.EventHandler(this.btn_AddWorkShift_Click);
		this.calcEditEx1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.calcEditEx1.EditControl = null;
		this.calcEditEx1.FieldName = "RoundUnit";
		this.calcEditEx1.IsSearchControl = false;
		this.calcEditEx1.Location = new System.Drawing.Point(272, 51);
		this.calcEditEx1.Name = "calcEditEx1";
		this.calcEditEx1.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcEditEx1.Properties.IsSearchControl = false;
		this.calcEditEx1.SettingInVisible = false;
		this.calcEditEx1.SettingReadonly = false;
		this.calcEditEx1.Size = new System.Drawing.Size(65, 20);
		this.calcEditEx1.TabIndex = 45;
		this.labelLine20.EditControl = null;
		this.labelLine20.EditValue = "Round Unit";
		this.labelLine20.FieldName = null;
		this.labelLine20.HideWhenEmpty = false;
		this.labelLine20.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine20.Location = new System.Drawing.Point(172, 54);
		this.labelLine20.Name = "labelLine20";
		this.labelLine20.ReadOnly = false;
		this.labelLine20.SettingInVisible = false;
		this.labelLine20.SettingReadonly = false;
		this.labelLine20.ShowUnderLine = true;
		this.labelLine20.Size = new System.Drawing.Size(102, 20);
		this.labelLine20.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine20.TabIndex = 46;
		this.labelLine20.Utext = "Round Unit";
		this.luk_CalcType.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.luk_CalcType.EditControl = null;
		this.luk_CalcType.FieldName = "CalcType";
		this.luk_CalcType.IsSearchControl = false;
		this.luk_CalcType.Location = new System.Drawing.Point(272, 26);
		this.luk_CalcType.Name = "luk_CalcType";
		this.luk_CalcType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_CalcType.Properties.NullText = "";
		this.luk_CalcType.SettingInVisible = false;
		this.luk_CalcType.SettingReadonly = false;
		this.luk_CalcType.Size = new System.Drawing.Size(164, 20);
		this.luk_CalcType.TabIndex = 31;
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Calc. Type";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(172, 29);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(102, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 30;
		this.labelLine5.Utext = "Calc. Type";
		this.luk_InOutID.EditControl = null;
		this.luk_InOutID.FieldName = "InOutID";
		this.luk_InOutID.IsSearchControl = false;
		this.luk_InOutID.Location = new System.Drawing.Point(68, 79);
		this.luk_InOutID.Name = "luk_InOutID";
		this.luk_InOutID.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_InOutID.Properties.NullText = "";
		this.luk_InOutID.SettingInVisible = false;
		this.luk_InOutID.SettingReadonly = false;
		this.luk_InOutID.Size = new System.Drawing.Size(101, 20);
		this.luk_InOutID.TabIndex = 29;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "In Out Mode";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(2, 82);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(66, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 28;
		this.labelLine1.Utext = "In Out Mode";
		this.luk_Branch.EditControl = null;
		this.luk_Branch.FieldName = "branch";
		this.luk_Branch.IsSearchControl = false;
		this.luk_Branch.Location = new System.Drawing.Point(67, 28);
		this.luk_Branch.Name = "luk_Branch";
		this.luk_Branch.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Branch.Properties.NullText = "";
		this.luk_Branch.SettingInVisible = false;
		this.luk_Branch.SettingReadonly = false;
		this.luk_Branch.Size = new System.Drawing.Size(101, 20);
		this.luk_Branch.TabIndex = 27;
		this.luk_Branch.EditValueChanged += new System.EventHandler(this.luk_Branch_EditValueChanged);
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Branch";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(1, 31);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(66, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 26;
		this.labelLine4.Utext = "Branch";
		this.luk_HldCode.EditControl = null;
		this.luk_HldCode.FieldName = "HldCode";
		this.luk_HldCode.IsSearchControl = false;
		this.luk_HldCode.Location = new System.Drawing.Point(67, 50);
		this.luk_HldCode.Name = "luk_HldCode";
		this.luk_HldCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_HldCode.Properties.NullText = "";
		this.luk_HldCode.SettingInVisible = false;
		this.luk_HldCode.SettingReadonly = false;
		this.luk_HldCode.Size = new System.Drawing.Size(101, 20);
		this.luk_HldCode.TabIndex = 25;
		this.labelLine15.EditControl = null;
		this.labelLine15.EditValue = "Holiday";
		this.labelLine15.FieldName = null;
		this.labelLine15.HideWhenEmpty = false;
		this.labelLine15.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine15.Location = new System.Drawing.Point(2, 54);
		this.labelLine15.Name = "labelLine15";
		this.labelLine15.ReadOnly = false;
		this.labelLine15.SettingInVisible = false;
		this.labelLine15.SettingReadonly = false;
		this.labelLine15.ShowUnderLine = true;
		this.labelLine15.Size = new System.Drawing.Size(66, 20);
		this.labelLine15.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine15.TabIndex = 24;
		this.labelLine15.Utext = "Holiday";
		this.grd_Line.AllowPasteExcel = true;
		this.grd_Line.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Line.ClipboardData = null;
		this.grd_Line.Location = new System.Drawing.Point(2, 138);
		this.grd_Line.MainView = this.gv_Week;
		this.grd_Line.Name = "grd_Line";
		this.grd_Line.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.rep_WeekDay, this.rep_DayInMonth, this.rep_DayInYear });
		this.grd_Line.Size = new System.Drawing.Size(588, 241);
		this.grd_Line.TabIndex = 12;
		this.grd_Line.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[3] { this.gv_Week, this.gv_Year, this.gv_Month });
		this.gv_Week.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Week.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Week.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Week.ColumnKey = null;
		this.gv_Week.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumnEx5, this.gridColumnEx8 });
		this.gv_Week.DisableFormSetting = false;
		this.gv_Week.DisplayException = false;
		this.gv_Week.EditColumn = null;
		this.gv_Week.FieldCurrency = null;
		this.gv_Week.FieldRate = null;
		this.gv_Week.FormId = "oWorkScheduleSetup";
		this.gv_Week.GridControl = this.grd_Line;
		this.gv_Week.HasFormSetting = false;
		this.gv_Week.IgnoreFilter = false;
		this.gv_Week.IndexEditRow = -1;
		this.gv_Week.IndicatorWidth = 35;
		this.gv_Week.IsApzTable = false;
		this.gv_Week.IsInsertNewRow = false;
		this.gv_Week.IsLoadingData = false;
		this.gv_Week.IsMappingContextAction = false;
		this.gv_Week.ItemId = "OWEEK";
		this.gv_Week.LstColumnAllowBatch = null;
		this.gv_Week.LstColumnNotAllowBatch = null;
		this.gv_Week.LstColumnNotEditable = null;
		this.gv_Week.LstColumNotSetting = null;
		this.gv_Week.LstOldUDF = null;
		this.gv_Week.Name = "gv_Week";
		this.gv_Week.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Week.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Week.OptionsPrint.AutoWidth = false;
		this.gv_Week.OptionsPrint.UsePrintStyles = false;
		this.gv_Week.OptionsSelection.MultiSelect = true;
		this.gv_Week.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.gv_Week.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Week.OptionsView.ColumnAutoWidth = false;
		this.gv_Week.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Week.OptionsView.ShowGroupedColumns = true;
		this.gv_Week.OptionsView.ShowGroupPanel = false;
		this.gv_Week.UseBatchMod = false;
		this.gv_Week.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Week_PopupMenuShowing);
		this.gv_Week.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Line_CellValueChanged);
		this.gv_Week.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_Week_KeyDown);
		this.gridColumnEx5.Caption = "Week Day";
		this.gridColumnEx5.ColumnEdit = this.rep_WeekDay;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "DayID";
		this.gridColumnEx5.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 0;
		this.rep_WeekDay.AutoHeight = false;
		this.rep_WeekDay.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_WeekDay.Name = "rep_WeekDay";
		this.rep_WeekDay.NullText = "";
		this.gridColumnEx8.Caption = "Work Shifts";
		this.gridColumnEx8.CurrencyDateField = null;
		this.gridColumnEx8.CurrencyField = null;
		this.gridColumnEx8.CurrencyFixed = null;
		this.gridColumnEx8.CurrencyFixedRate = 0.0;
		this.gridColumnEx8.CurrencyRateField = null;
		this.gridColumnEx8.FieldName = "WorkShift";
		this.gridColumnEx8.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx8.Name = "gridColumnEx8";
		this.gridColumnEx8.PrimaryKey = false;
		this.gridColumnEx8.UnitField = null;
		this.gridColumnEx8.Visible = true;
		this.gridColumnEx8.VisibleIndex = 1;
		this.rep_DayInMonth.AutoHeight = false;
		this.rep_DayInMonth.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_DayInMonth.Name = "rep_DayInMonth";
		this.rep_DayInMonth.NullText = "";
		this.rep_DayInYear.AutoHeight = false;
		this.rep_DayInYear.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo, "", -1, true, false, false, imageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_DayInYear.Name = "rep_DayInYear";
		this.rep_DayInYear.NullText = "";
		this.gv_Year.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Year.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Year.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Year.ColumnKey = null;
		this.gv_Year.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumnEx7, this.gridColumnEx10 });
		this.gv_Year.DisableFormSetting = false;
		this.gv_Year.DisplayException = false;
		this.gv_Year.EditColumn = null;
		this.gv_Year.FieldCurrency = null;
		this.gv_Year.FieldRate = null;
		this.gv_Year.FormId = "oWorkScheduleSetup";
		this.gv_Year.GridControl = this.grd_Line;
		this.gv_Year.HasFormSetting = false;
		this.gv_Year.IgnoreFilter = false;
		this.gv_Year.IndexEditRow = -1;
		this.gv_Year.IndicatorWidth = 35;
		this.gv_Year.IsApzTable = false;
		this.gv_Year.IsInsertNewRow = false;
		this.gv_Year.IsLoadingData = false;
		this.gv_Year.IsMappingContextAction = false;
		this.gv_Year.ItemId = "OYEAR";
		this.gv_Year.LstColumnAllowBatch = null;
		this.gv_Year.LstColumnNotAllowBatch = null;
		this.gv_Year.LstColumnNotEditable = null;
		this.gv_Year.LstColumNotSetting = null;
		this.gv_Year.LstOldUDF = null;
		this.gv_Year.Name = "gv_Year";
		this.gv_Year.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Year.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Year.OptionsPrint.AutoWidth = false;
		this.gv_Year.OptionsPrint.UsePrintStyles = false;
		this.gv_Year.OptionsSelection.MultiSelect = true;
		this.gv_Year.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.gv_Year.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Year.OptionsView.ColumnAutoWidth = false;
		this.gv_Year.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Year.OptionsView.ShowGroupedColumns = true;
		this.gv_Year.OptionsView.ShowGroupPanel = false;
		this.gv_Year.UseBatchMod = false;
		this.gv_Year.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Year_CellValueChanged);
		this.gridColumnEx7.Caption = "Day";
		this.gridColumnEx7.ColumnEdit = this.rep_DayInYear;
		this.gridColumnEx7.CurrencyDateField = null;
		this.gridColumnEx7.CurrencyField = null;
		this.gridColumnEx7.CurrencyFixed = null;
		this.gridColumnEx7.CurrencyFixedRate = 0.0;
		this.gridColumnEx7.CurrencyRateField = null;
		this.gridColumnEx7.FieldName = "DayID";
		this.gridColumnEx7.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx7.Name = "gridColumnEx7";
		this.gridColumnEx7.PrimaryKey = false;
		this.gridColumnEx7.UnitField = null;
		this.gridColumnEx7.Visible = true;
		this.gridColumnEx7.VisibleIndex = 0;
		this.gridColumnEx10.Caption = "Work Shifts";
		this.gridColumnEx10.CurrencyDateField = null;
		this.gridColumnEx10.CurrencyField = null;
		this.gridColumnEx10.CurrencyFixed = null;
		this.gridColumnEx10.CurrencyFixedRate = 0.0;
		this.gridColumnEx10.CurrencyRateField = null;
		this.gridColumnEx10.FieldName = "WorkShift";
		this.gridColumnEx10.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx10.Name = "gridColumnEx10";
		this.gridColumnEx10.PrimaryKey = false;
		this.gridColumnEx10.UnitField = null;
		this.gridColumnEx10.Visible = true;
		this.gridColumnEx10.VisibleIndex = 1;
		this.gv_Month.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Month.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Month.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Month.ColumnKey = null;
		this.gv_Month.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumnEx6, this.gridColumnEx9 });
		this.gv_Month.DisableFormSetting = false;
		this.gv_Month.DisplayException = false;
		this.gv_Month.EditColumn = null;
		this.gv_Month.FieldCurrency = null;
		this.gv_Month.FieldRate = null;
		this.gv_Month.FormId = "oWorkScheduleSetup";
		this.gv_Month.GridControl = this.grd_Line;
		this.gv_Month.HasFormSetting = false;
		this.gv_Month.IgnoreFilter = false;
		this.gv_Month.IndexEditRow = -1;
		this.gv_Month.IndicatorWidth = 35;
		this.gv_Month.IsApzTable = false;
		this.gv_Month.IsInsertNewRow = false;
		this.gv_Month.IsLoadingData = false;
		this.gv_Month.IsMappingContextAction = false;
		this.gv_Month.ItemId = "OMONTH";
		this.gv_Month.LstColumnAllowBatch = null;
		this.gv_Month.LstColumnNotAllowBatch = null;
		this.gv_Month.LstColumnNotEditable = null;
		this.gv_Month.LstColumNotSetting = null;
		this.gv_Month.LstOldUDF = null;
		this.gv_Month.Name = "gv_Month";
		this.gv_Month.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Month.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Month.OptionsPrint.AutoWidth = false;
		this.gv_Month.OptionsPrint.UsePrintStyles = false;
		this.gv_Month.OptionsSelection.MultiSelect = true;
		this.gv_Month.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CellSelect;
		this.gv_Month.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Month.OptionsView.ColumnAutoWidth = false;
		this.gv_Month.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Month.OptionsView.ShowGroupedColumns = true;
		this.gv_Month.OptionsView.ShowGroupPanel = false;
		this.gv_Month.UseBatchMod = false;
		this.gv_Month.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Month_CellValueChanged);
		this.gridColumnEx6.Caption = "Day In Month";
		this.gridColumnEx6.ColumnEdit = this.rep_DayInMonth;
		this.gridColumnEx6.CurrencyDateField = null;
		this.gridColumnEx6.CurrencyField = null;
		this.gridColumnEx6.CurrencyFixed = null;
		this.gridColumnEx6.CurrencyFixedRate = 0.0;
		this.gridColumnEx6.CurrencyRateField = null;
		this.gridColumnEx6.FieldName = "DayID";
		this.gridColumnEx6.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx6.Name = "gridColumnEx6";
		this.gridColumnEx6.PrimaryKey = false;
		this.gridColumnEx6.UnitField = null;
		this.gridColumnEx6.Visible = true;
		this.gridColumnEx6.VisibleIndex = 0;
		this.gridColumnEx9.Caption = "Work Shifts";
		this.gridColumnEx9.CurrencyDateField = null;
		this.gridColumnEx9.CurrencyField = null;
		this.gridColumnEx9.CurrencyFixed = null;
		this.gridColumnEx9.CurrencyFixedRate = 0.0;
		this.gridColumnEx9.CurrencyRateField = null;
		this.gridColumnEx9.FieldName = "WorkShift";
		this.gridColumnEx9.FilterMode = DevExpress.XtraGrid.ColumnFilterMode.DisplayText;
		this.gridColumnEx9.Name = "gridColumnEx9";
		this.gridColumnEx9.PrimaryKey = false;
		this.gridColumnEx9.UnitField = null;
		this.gridColumnEx9.Visible = true;
		this.gridColumnEx9.VisibleIndex = 1;
		this.rad_CycleMode.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.rad_CycleMode.EditControl = null;
		this.rad_CycleMode.FieldName = "CycleMode";
		this.rad_CycleMode.IsSearchControl = false;
		this.rad_CycleMode.Location = new System.Drawing.Point(272, 75);
		this.rad_CycleMode.Name = "rad_CycleMode";
		this.rad_CycleMode.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("W", "Week"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("M", "Month")
		});
		this.rad_CycleMode.SettingInVisible = false;
		this.rad_CycleMode.SettingReadonly = false;
		this.rad_CycleMode.Size = new System.Drawing.Size(313, 24);
		this.rad_CycleMode.TabIndex = 11;
		this.rad_CycleMode.EditValueChanged += new System.EventHandler(this.rad_CycleMode_EditValueChanged);
		this.labelLine9.EditControl = null;
		this.labelLine9.EditValue = "Cycle";
		this.labelLine9.FieldName = null;
		this.labelLine9.HideWhenEmpty = false;
		this.labelLine9.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine9.Location = new System.Drawing.Point(172, 82);
		this.labelLine9.Name = "labelLine9";
		this.labelLine9.ReadOnly = false;
		this.labelLine9.SettingInVisible = false;
		this.labelLine9.SettingReadonly = false;
		this.labelLine9.ShowUnderLine = true;
		this.labelLine9.Size = new System.Drawing.Size(99, 20);
		this.labelLine9.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine9.TabIndex = 10;
		this.labelLine9.Utext = "Cycle";
		this.txt_SchName.EditControl = null;
		this.txt_SchName.FieldName = "SchName";
		this.txt_SchName.IsSearchControl = false;
		this.txt_SchName.Location = new System.Drawing.Point(67, 5);
		this.txt_SchName.Name = "txt_SchName";
		this.txt_SchName.SettingInVisible = false;
		this.txt_SchName.SettingReadonly = false;
		this.txt_SchName.Size = new System.Drawing.Size(101, 20);
		this.txt_SchName.TabIndex = 9;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Name";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(2, 8);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(66, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 8;
		this.labelLine3.Utext = "Name";
		this.txt_Dscription.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txt_Dscription.EditControl = null;
		this.txt_Dscription.FieldName = "Dscription";
		this.txt_Dscription.IsSearchControl = false;
		this.txt_Dscription.Location = new System.Drawing.Point(272, 2);
		this.txt_Dscription.Name = "txt_Dscription";
		this.txt_Dscription.SettingInVisible = false;
		this.txt_Dscription.SettingReadonly = false;
		this.txt_Dscription.Size = new System.Drawing.Size(313, 20);
		this.txt_Dscription.TabIndex = 7;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Description";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(172, 5);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(102, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 6;
		this.labelLine2.Utext = "Description";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(81, 385);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 19;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "OK";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(0, 385);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(75, 20);
		this.btn_Ok.TabIndex = 18;
		this.btn_Ok.Text = "OK";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		base.AcceptButton = this.btn_Ok;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(850, 409);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Ok);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oWorkScheduleSetup";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_WorkScheduleSetup";
		this.Text = "Employee Schedule Setup";
		base.OnControlValueChanged += new Apzon.Controls.Form.ControlValueChanged(this.frm_WorkScheduleSetup_OnControlValueChanged);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.ctm_Doc.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Cycle).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.calcEditEx1.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_CalcType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_InOutID.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_HldCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Week).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_WeekDay).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DayInMonth).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DayInYear).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Year).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Month).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rad_CycleMode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_SchName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Dscription.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
