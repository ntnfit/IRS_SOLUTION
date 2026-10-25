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
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources.TimeSheeting;

public class frm_EmployeeScheduleEdit : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public DataTable DtSearch;

	private bool _isChanging;

	private bool _forceClose;

	private bool _isCancel;

	private RepositoryItemButtonEditCFL rep_CFL_NoSearch = new RepositoryItemButtonEditCFL();

	private RepositoryItemButtonEditCFL rep_empName_NoSearch = new RepositoryItemButtonEditCFL();

	private bool _isPasteFromExcel;

	private IContainer components;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx3;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx4;

	private RepositoryItemCalcEditEx repositoryItemCalcEditEx1;

	private DateEditEx date_ValidTo;

	private DateEditEx date_ValidFrom;

	public SimpleButtonEx btnSearch;

	public SimpleButtonEx btn_Save;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private GridColumnEx gc_Branch;

	private RepositoryItemLookUpEdit rep_Branch;

	private GridColumnEx gridColumnEx3;

	private RepositoryItemLookUpEdit rep_Status;

	private GridColumnEx gridColumn1;

	private RepositoryItemLookUpEdit rep_Deparment;

	private GridColumnEx gc_EmpNo;

	private RepositoryItemButtonEditCFL rep_CFL;

	private LabelLine lbl_ValidTo;

	private LabelLine lbl_ValidFrom;

	public SimpleButtonEx btnCancel;

	private GridColumnEx gridColumnEx4;

	private GridColumnEx gc_empName;

	private RepositoryItemButtonEditCFL rep_empName;

	private GridColumnEx gridColumnEx1;

	private GridColumnEx gridColumnEx2;

	private GridColumnEx gridColumnEx5;

	private RepositoryItemLookUpEdit rep_Schedule;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem newRowToolStripMenuItem;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btnCancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_EmployeeScheduleEdit()
	{
		InitializeComponent();
		OnLoadAsync += frm_EmployeeScheduleEdit_Load;
		gv_Doc.CellValueChanged += GvDocOnCellValueChanged;
		FormClosing += OnFormClosing;
		gv_Doc.CustomRowCellEdit += GvDocOnCustomRowCellEdit;
	}

	private void GvDocOnCustomRowCellEdit(object sender, CustomRowCellEditEventArgs e)
	{
		try
		{
			if (e.Column == gc_EmpNo)
			{
				if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "LineNum")) != "")
				{
					e.RepositoryItem = rep_CFL_NoSearch;
				}
				else
				{
					e.RepositoryItem = rep_CFL;
				}
			}
			if (e.Column == gc_empName)
			{
				if (Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, "LineNum")) != "")
				{
					e.RepositoryItem = rep_empName_NoSearch;
				}
				else
				{
					e.RepositoryItem = rep_empName;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void OnFormClosing(object sender, FormClosingEventArgs e)
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
					if (string.IsNullOrEmpty(await SaveData()))
					{
						_forceClose = true;
						Close();
					}
					break;
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

	private async Task<string> SaveData()
	{
		WinCommons._OpenProcessing();
		string strResult = string.Empty;
		try
		{
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			if (dataSourceTable != null)
			{
				string text = await _httpClient.WorkScheduleRegisterClient.SaveDoc(dataSourceTable);
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
		finally
		{
			WinCommons._CloseProcessing();
		}
		return strResult;
	}

	private void GvDocOnCellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (!_isChanging)
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

	private async Task frm_EmployeeScheduleEdit_Load(object sender, EventArgs e)
	{
		try
		{
			await LoadInitForm();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async Task LoadInitForm()
	{
		_ = 3;
		try
		{
			if (DtSearch.IsNotNull())
			{
				DtSearch.Rows[0]["validFrom"] = DateTime.Today.AddDays(-Configuration.SearchDate);
				DtSearch.Rows[0]["validTo"] = DateTime.Today.AddDays(Configuration.SearchDate);
				DataSource = DtSearch;
			}
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboDept();
			rep_Deparment.DataSource = dataTable.Copy();
			rep_Deparment.DisplayMember = "Name";
			rep_Deparment.ValueMember = "Code";
			rep_Deparment.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Deparment.ForceInitialize();
			rep_Deparment.PopulateColumns();
			rep_Deparment.Columns["Code"].Visible = false;
			rep_Deparment.Columns["Remarks"].Visible = false;
			DataTable dataTable2 = await _httpClient.EmployeeMasterClient.GetEmployeeStatus();
			rep_Status.DataSource = dataTable2.Copy();
			rep_Status.DisplayMember = "Name";
			rep_Status.ValueMember = "Code";
			rep_Status.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Status.ForceInitialize();
			rep_Status.PopulateColumns();
			rep_Status.Columns["Code"].Visible = false;
			DataTable dataTable3 = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			DataRow row = dataTable3.NewRow();
			dataTable3.Rows.InsertAt(row, 0);
			dataTable3.AcceptChanges();
			rep_Branch.DataSource = dataTable3.Copy();
			rep_Branch.DisplayMember = "Name";
			rep_Branch.ValueMember = "Code";
			rep_Branch.BestFitMode = BestFitMode.BestFitResizePopup;
			rep_Branch.ForceInitialize();
			rep_Branch.PopulateColumns();
			rep_Branch.Columns["Code"].Visible = false;
			DataTable dataTable4 = await _httpClient.WorkScheduleSetupClient.GetAll();
			EnumerableRowCollection<DataRow> source = from t in dataTable4.AsEnumerable()
				where Function.ParseInt(t["branch"]) == Function.ParseInt(DtSearch.Rows[0]["branch"])
				select t;
			DataTable dt = dataTable4.Clone();
			if (source.Any())
			{
				dt = source.CopyToDataTable();
			}
			LookUpEditEx lookUpEditEx = new LookUpEditEx();
			lookUpEditEx.SetDataSource(dt, "SchName", "SchID", emptyRow: true, emptyTop: true, hideOtherColumns: true);
			rep_Schedule.Assign(lookUpEditEx.Properties);
			rep_CFL.ValueMember = "empID";
			rep_CFL.DisplayMember = "empID";
			rep_CFL.ContainerControl = this;
			rep_CFL.AutoLoadDataSource = false;
			rep_CFL.FindCFLWith(Enums.TypeCFL.Employees, true, "Apz_Cfl_EmployleeData", Enums.ItemObjectType.NoItemType, null, "");
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL = rep_CFL;
			repositoryItemButtonEditCFL.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL cfl) =>
			{
				cfl.Branch = Function.ParseInt(DtSearch.Rows[0]["branch"]);
			}));
			rep_CFL.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL2 = rep_CFL;
			repositoryItemButtonEditCFL2.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL2.HandleChooseItem, new Action<DataRow>(FillData));
			rep_CFL.HandleChooseItemMulti = HandleChooseItemMulti;
			rep_CFL.RefreshDataSource();
			rep_empName.ValueMember = "empID";
			rep_empName.DisplayMember = "lastName";
			rep_empName.DisplayMember2 = "firstName";
			rep_empName.ContainerControl = this;
			rep_empName.AutoLoadDataSource = false;
			rep_empName.FindCFLWith(Enums.TypeCFL.Employees, true, "Apz_Cfl_EmployleeData", Enums.ItemObjectType.NoItemType, null, "");
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL3 = rep_empName;
			repositoryItemButtonEditCFL3.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(repositoryItemButtonEditCFL3.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL cfl) =>
			{
				cfl.Branch = Function.ParseInt(DtSearch.Rows[0]["branch"]);
			}));
			rep_empName.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			RepositoryItemButtonEditCFL repositoryItemButtonEditCFL4 = rep_empName;
			repositoryItemButtonEditCFL4.HandleChooseItem = (Action<DataRow>)Delegate.Combine(repositoryItemButtonEditCFL4.HandleChooseItem, new Action<DataRow>(FillData));
			rep_empName.HandleChooseItemMulti = HandleChooseItemMulti;
			rep_empName.RefreshDataSource();
			rep_CFL_NoSearch.Assign(rep_CFL);
			rep_CFL_NoSearch.Buttons[0].Visible = false;
			rep_CFL_NoSearch.TextEditStyle = TextEditStyles.DisableTextEditor;
			rep_empName_NoSearch.Assign(rep_empName);
			rep_empName_NoSearch.Buttons[0].Visible = false;
			rep_empName_NoSearch.TextEditStyle = TextEditStyles.DisableTextEditor;
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			Logging.Write("ERROR", ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				btn_Save.Enabled = false;
				btnCancel.Enabled = false;
				await SaveData();
				btn_Save.Enabled = true;
				btnCancel.Enabled = true;
			}
			else
			{
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		_isCancel = true;
		Close();
		_isCancel = false;
	}

	private async void btnSearch_Click(object sender, EventArgs e)
	{
		try
		{
			if (date_ValidFrom.EditValue == null)
			{
				WinCommons.Error(Enums.MessageCode.ValidFromDateCannotBeNull, "");
			}
			else if (Function.ParseDateTimes(date_ValidTo.EditValue) < Function.ParseDateTimes(date_ValidFrom.EditValue) && date_ValidTo.EditValue != null)
			{
				WinCommons.Error(Enums.MessageCode.ValidFromDateCannotBeLargeThanValidToDate, "");
			}
			else if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				await LoadData();
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dt = DataSource as DataTable;
			DataTable dataSource = await _httpClient.WorkScheduleRegisterClient.Search(dt);
			grd_Doc.DataSource = dataSource;
			grd_Doc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void newRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			AddNewRow();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void AddNewRow()
	{
		try
		{
			gv_Doc.BeginInsertRow();
			DataTable dataTable = grd_Doc.DataSourceTable();
			if (!dataTable.IsNotNull())
			{
				dataTable = new DataTable
				{
					Columns = 
					{
						{
							"empID",
							typeof(int)
						},
						{
							"LineNum",
							typeof(int)
						},
						{
							"empName",
							typeof(string)
						},
						{
							"ExtEmpNo",
							typeof(string)
						},
						{
							"dept",
							typeof(int)
						},
						{
							"ValidFrom",
							typeof(DateTime)
						},
						{
							"ValidTo",
							typeof(DateTime)
						},
						{
							"SchID",
							typeof(int)
						}
					}
				}.Clone();
			}
			DataRow dataRow = dataTable.NewRow();
			dataRow["empID"] = DBNull.Value;
			dataRow["LineNum"] = DBNull.Value;
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
			gv_Doc.EndInsertRow();
			gv_Doc.ExpandAllGroups();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		gv_Doc.EndInsertRow();
	}

	private async void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			DataTable dt = grd_Doc.DataSourceTable;
			object focusedRowCellValue = gv_Doc.GetFocusedRowCellValue("LineNum");
			gv_Doc.GetFocusedRowCellValue("ValidFrom");
			int empID = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("empID"));
			if (string.IsNullOrWhiteSpace(Function.ToString(focusedRowCellValue)))
			{
				gv_Doc.BeginUpdate();
				gv_Doc.DeleteRow(gv_Doc.FocusedRowHandle);
				gv_Doc.EndUpdate();
				dt.AcceptChanges();
				grd_Doc.RefreshDataSource();
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteThisRow, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string text = await _httpClient.WorkScheduleRegisterClient.Remove(Function.ParseInt(focusedRowCellValue), empID);
				if (string.IsNullOrEmpty(text))
				{
					gv_Doc.BeginUpdate();
					gv_Doc.DeleteRow(gv_Doc.FocusedRowHandle);
					gv_Doc.EndUpdate();
					dt.AcceptChanges();
					grd_Doc.RefreshDataSource();
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
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
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Doc_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				PasteFromExcelToDataTable(grd_Doc, gv_Doc, new List<string> { "empID" });
				e.SuppressKeyPress = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void PasteFromExcelToDataTable(GridControlEx grd, GridViewEx gv, List<string> key)
	{
		try
		{
			_isPasteFromExcel = true;
			try
			{
				DataObject dataObject = (DataObject)Clipboard.GetDataObject();
				if (dataObject != null)
				{
					string clipboardData = dataObject.GetData(DataFormats.UnicodeText).ToString();
					if (dataObject.GetDataPresent(DataFormats.Text))
					{
						DataTable dataTable = Function.PasteFromExcel(clipboardData);
						if (dataTable != null && (dataTable.Rows.Count > 0 || dataTable.Columns.Count > 0))
						{
							int num = gv.FocusedRowHandle;
							GridColumn focusedColumn = gv.FocusedColumn;
							if (key.Contains(focusedColumn.FieldName))
							{
								bool flag = false;
								for (int i = 0; i < dataTable.Rows.Count; i++)
								{
									int num2 = Function.ParseInt(grd.DataSourceTable.Rows[num]["empID"]);
									if (Function.ParseInt(dataTable.Rows[i][0]) != num2 && num2 != 0)
									{
										WinCommons.Warning(Enums.MessageCode.InfoIncorrectEmployeeCode, "");
										break;
									}
									if (num >= grd.DataSourceTable.Rows.Count - 1)
									{
										AddNewRow();
										flag = true;
									}
									int colunmIndex = focusedColumn.VisibleIndex;
									for (int j = 0; j < dataTable.Columns.Count; j++)
									{
										GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colunmIndex);
										if (gridColumn == null)
										{
											break;
										}
										gv.SetRowCellValue(num, gridColumn, dataTable.Rows[i][j]);
										int num3 = colunmIndex + 1;
										colunmIndex = num3;
									}
									DataTableResult employeeInfo = _httpClient.EmployeeMasterClient.GetEmployeeInfo(Function.ParseInt(dataTable.Rows[i][0]));
									if (employeeInfo != null)
									{
										gv.SetRowCellValue(num, "dept", employeeInfo.Data.Rows[0]["dept"]);
										gv.SetRowCellValue(num, "empName", Function.ToString(employeeInfo.Data.Rows[0]["lastName"]) + ", " + Function.ToString(employeeInfo.Data.Rows[0]["firstName"]));
										gv.SetRowCellValue(num, "ExtEmpNo", employeeInfo.Data.Rows[0]["ExtEmpNo"]);
									}
									grd.DataSourceTable().AcceptChanges();
									num = (gv.FocusedRowHandle = num + 1);
								}
								if (flag)
								{
									grd.DataSourceTable.Rows.RemoveAt(gv.DataRowCount - 1);
								}
							}
							else
							{
								int focusedRowHandle = gv.FocusedRowHandle;
								for (int num5 = focusedRowHandle; num5 < gv.RowCount; num5++)
								{
									int colIndex = focusedColumn.VisibleIndex;
									bool flag2 = true;
									for (int num6 = 0; num6 < key.Count; num6++)
									{
										if (string.IsNullOrEmpty(Function.ToString(gv.GetRowCellValue(num5, key[num6]))))
										{
											flag2 = false;
											break;
										}
									}
									if (!flag2 || num5 - focusedRowHandle >= dataTable.Rows.Count)
									{
										break;
									}
									for (int num7 = 0; num7 < dataTable.Columns.Count; num7++)
									{
										GridColumn gridColumn2 = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
										if (gridColumn2 == null)
										{
											break;
										}
										gv.SetRowCellValue(num, gridColumn2, dataTable.Rows[num5 - focusedRowHandle][num7]);
										int num3 = colIndex + 1;
										colIndex = num3;
									}
									num++;
								}
								grd.RefreshDataSource();
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			}
			_isPasteFromExcel = false;
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
	}

	private void HandleChooseItemMulti(List<DataRow> rows)
	{
		try
		{
			if (!(grd_Doc.DataSource is DataTable dataTable) || dataTable.Rows.Count <= 0 || rows.Count <= 0)
			{
				return;
			}
			_isChanging = true;
			gv_Doc.SetFocusedRowCellValue("empID", Function.ParseInt(rows[0]["EmpId"]));
			gv_Doc.SetFocusedRowCellValue("empName", Function.ParseInt(rows[0]["EmpId"]));
			gv_Doc.SetFocusedRowCellValue("dept", Function.ParseInt(rows[0]["dept"]));
			gv_Doc.SetFocusedRowCellValue("ExtEmpNo", Function.ToString(rows[0]["ExtEmpNo"]));
			for (int i = 1; i < rows.Count; i++)
			{
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["EmpId"])));
				if (dataRow != null)
				{
					int dataSourceIndex = dataTable.Rows.IndexOf(dataRow);
					gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataSourceIndex), "empID", Function.ParseInt(rows[i]["EmpId"]));
					gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataSourceIndex), "empName", Function.ParseInt(rows[i]["EmpId"]));
					gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataSourceIndex), "dept", Function.ParseInt(rows[i]["dept"]));
					gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataSourceIndex), "ExtEmpNo", Function.ToString(rows[i]["ExtEmpNo"]));
					continue;
				}
				AddNewRow();
				DataRow row = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => string.IsNullOrWhiteSpace(Function.ToString(r["empID"])));
				int dataSourceIndex2 = dataTable.Rows.IndexOf(row);
				gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataSourceIndex2), "empID", Function.ParseInt(rows[i]["EmpId"]));
				gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataSourceIndex2), "empName", Function.ParseInt(rows[i]["EmpId"]));
				gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataSourceIndex2), "dept", Function.ParseInt(rows[i]["dept"]));
				gv_Doc.SetRowCellValue(gv_Doc.GetRowHandle(dataSourceIndex2), "ExtEmpNo", Function.ToString(rows[i]["ExtEmpNo"]));
			}
			_isChanging = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void FillData(DataRow dataRow)
	{
		try
		{
			if (Function.ToString(gv_Doc.GetFocusedRowCellValue("LineNum")) == "")
			{
				gv_Doc.SetFocusedRowCellValue("empID", Function.ParseInt(dataRow["empID"]));
				gv_Doc.SetFocusedRowCellValue("dept", Function.ParseInt(dataRow["dept"]));
				gv_Doc.SetFocusedRowCellValue("empName", Function.ParseInt(dataRow["empID"]));
				gv_Doc.SetFocusedRowCellValue("ExtEmpNo", Function.ToString(dataRow["ExtEmpNo"]));
				gv_Doc.PostEditor();
				gv_Doc.RefreshData();
			}
			else
			{
				WinCommons.Warning(Enums.MessageCode.CannotReselectedEmployee4ExistedRecord, "");
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.TimeSheeting.frm_EmployeeScheduleEdit));
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
		this.repositoryItemCalcEditEx3 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx4 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.repositoryItemCalcEditEx1 = new Apzon.Controls.Editor.RepositoryItemCalcEditEx();
		this.date_ValidTo = new Apzon.Controls.Editor.DateEditEx();
		this.date_ValidFrom = new Apzon.Controls.Editor.DateEditEx();
		this.btnSearch = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.newRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.gc_Branch = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Branch = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumnEx3 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Status = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Deparment = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gc_EmpNo = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_CFL = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx4 = new Apzon.Controls.Grid.GridColumnEx();
		this.gc_empName = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_empName = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.gridColumnEx1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumnEx5 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Schedule = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.lbl_ValidTo = new Apzon.Controls.Controls.LabelLine();
		this.lbl_ValidFrom = new Apzon.Controls.Controls.LabelLine();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_empName).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Schedule).BeginInit();
		base.SuspendLayout();
		this.repositoryItemCalcEditEx3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx3.IsSearchControl = false;
		this.repositoryItemCalcEditEx3.Name = "repositoryItemCalcEditEx3";
		this.repositoryItemCalcEditEx4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx4.IsSearchControl = false;
		this.repositoryItemCalcEditEx4.Name = "repositoryItemCalcEditEx4";
		this.repositoryItemCalcEditEx1.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryItemCalcEditEx1.IsSearchControl = false;
		this.repositoryItemCalcEditEx1.Name = "repositoryItemCalcEditEx1";
		this.date_ValidTo.EditControl = null;
		this.date_ValidTo.EditValue = null;
		this.date_ValidTo.FieldName = "validTo";
		this.date_ValidTo.Location = new System.Drawing.Point(450, 5);
		this.date_ValidTo.Name = "date_ValidTo";
		this.date_ValidTo.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidTo.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidTo.Properties.IsSearchControl = true;
		this.date_ValidTo.SettingInVisible = false;
		this.date_ValidTo.SettingReadonly = false;
		this.date_ValidTo.Size = new System.Drawing.Size(170, 20);
		this.date_ValidTo.TabIndex = 91;
		this.date_ValidFrom.EditControl = null;
		this.date_ValidFrom.EditValue = null;
		this.date_ValidFrom.FieldName = "validFrom";
		this.date_ValidFrom.Location = new System.Drawing.Point(114, 4);
		this.date_ValidFrom.Name = "date_ValidFrom";
		this.date_ValidFrom.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidFrom.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ValidFrom.Properties.IsSearchControl = true;
		this.date_ValidFrom.SettingInVisible = false;
		this.date_ValidFrom.SettingReadonly = false;
		this.date_ValidFrom.Size = new System.Drawing.Size(170, 20);
		this.date_ValidFrom.TabIndex = 90;
		this.btnSearch.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnSearch.Appearance.Options.UseBackColor = true;
		this.btnSearch.EditControl = null;
		this.btnSearch.EditValue = "Search";
		this.btnSearch.FieldName = null;
		this.btnSearch.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSearch.ImageOptions.Image");
		this.btnSearch.IsSearchControl = true;
		this.btnSearch.Location = new System.Drawing.Point(626, 5);
		this.btnSearch.Name = "btnSearch";
		this.btnSearch.ReadOnly = false;
		this.btnSearch.SettingInVisible = false;
		this.btnSearch.SettingReadonly = false;
		this.btnSearch.Size = new System.Drawing.Size(99, 20);
		this.btnSearch.TabIndex = 92;
		this.btnSearch.Text = "Search";
		this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Save.Appearance.Options.UseBackColor = true;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(1, 392);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(61, 20);
		this.btn_Save.TabIndex = 94;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.grd_Doc.AllowPasteExcel = false;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.ContextMenuStrip = this.contextMenuStrip1;
		this.grd_Doc.Location = new System.Drawing.Point(1, 34);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_Deparment, this.rep_CFL, this.rep_empName, this.rep_Branch, this.rep_Status, this.rep_Schedule });
		this.grd_Doc.Size = new System.Drawing.Size(727, 352);
		this.grd_Doc.TabIndex = 93;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.grd_Doc.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.grd_Doc_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.newRowToolStripMenuItem, this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 48);
		this.newRowToolStripMenuItem.Name = "newRowToolStripMenuItem";
		this.newRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.newRowToolStripMenuItem.Text = "New Row";
		this.newRowToolStripMenuItem.Click += new System.EventHandler(this.newRowToolStripMenuItem_Click);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = "empID";
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[9] { this.gc_Branch, this.gridColumnEx3, this.gridColumn1, this.gc_EmpNo, this.gridColumnEx4, this.gc_empName, this.gridColumnEx1, this.gridColumnEx2, this.gridColumnEx5 });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oEmployeeScheduleEdit";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.GroupCount = 1;
		this.gv_Doc.HasFormSetting = false;
		this.gv_Doc.HeaderExcelExport = null;
		this.gv_Doc.HeaderExcelType = Apzon.Commons.Enums.Enums.HeaderExcelType.Disabled;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 30;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "ODOC";
		this.gv_Doc.LstColumnAllowBatch = new string[3] { "ValidFrom", "ValidTo", "SchID" };
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.LstOldUDF = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AutoExpandAllGroups = true;
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.ReadOnly = false;
		this.gv_Doc.SettingReadonly = false;
		this.gv_Doc.SortInfo.AddRange(new DevExpress.XtraGrid.Columns.GridColumnSortInfo[1]
		{
			new DevExpress.XtraGrid.Columns.GridColumnSortInfo(this.gridColumn1, DevExpress.Data.ColumnSortOrder.Ascending)
		});
		this.gv_Doc.UseBatchMod = true;
		this.gc_Branch.Caption = "Branch";
		this.gc_Branch.ColumnEdit = this.rep_Branch;
		this.gc_Branch.CurrencyDateField = null;
		this.gc_Branch.CurrencyField = null;
		this.gc_Branch.CurrencyFixed = null;
		this.gc_Branch.CurrencyFixedRate = 0.0;
		this.gc_Branch.CurrencyRateField = null;
		this.gc_Branch.FieldName = "branch";
		this.gc_Branch.Name = "gc_Branch";
		this.gc_Branch.OptionsColumn.ReadOnly = true;
		this.gc_Branch.PrimaryKey = true;
		this.gc_Branch.UnitField = null;
		this.rep_Branch.AutoHeight = false;
		this.rep_Branch.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Branch.Name = "rep_Branch";
		this.rep_Branch.NullText = "";
		this.gridColumnEx3.Caption = "Status";
		this.gridColumnEx3.ColumnEdit = this.rep_Status;
		this.gridColumnEx3.CurrencyDateField = null;
		this.gridColumnEx3.CurrencyField = null;
		this.gridColumnEx3.CurrencyFixed = null;
		this.gridColumnEx3.CurrencyFixedRate = 0.0;
		this.gridColumnEx3.CurrencyRateField = null;
		this.gridColumnEx3.FieldName = "status";
		this.gridColumnEx3.Name = "gridColumnEx3";
		this.gridColumnEx3.OptionsColumn.ReadOnly = true;
		this.gridColumnEx3.PrimaryKey = false;
		this.gridColumnEx3.UnitField = null;
		this.rep_Status.AutoHeight = false;
		this.rep_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Status.Name = "rep_Status";
		this.rep_Status.NullText = "";
		this.gridColumn1.Caption = "Department";
		this.gridColumn1.ColumnEdit = this.rep_Deparment;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "dept";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.rep_Deparment.AutoHeight = false;
		this.rep_Deparment.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Deparment.Name = "rep_Deparment";
		this.rep_Deparment.NullText = "";
		this.gc_EmpNo.Caption = "Employee No";
		this.gc_EmpNo.ColumnEdit = this.rep_CFL;
		this.gc_EmpNo.CurrencyDateField = null;
		this.gc_EmpNo.CurrencyField = null;
		this.gc_EmpNo.CurrencyFixed = null;
		this.gc_EmpNo.CurrencyFixedRate = 0.0;
		this.gc_EmpNo.CurrencyRateField = null;
		this.gc_EmpNo.FieldName = "empID";
		this.gc_EmpNo.Name = "gc_EmpNo";
		this.gc_EmpNo.PrimaryKey = true;
		this.gc_EmpNo.UnitField = null;
		this.gc_EmpNo.Visible = true;
		this.gc_EmpNo.VisibleIndex = 0;
		this.gc_EmpNo.Width = 86;
		this.rep_CFL.AutoHeight = false;
		this.rep_CFL.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.rep_CFL.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_CFL.ContainerControl = this;
		this.rep_CFL.DisplayMember = "";
		this.rep_CFL.DisplayMember2 = "";
		this.rep_CFL.ListMandatoryField = null;
		this.rep_CFL.LoadDataSourceWhenEnter = false;
		this.rep_CFL.Name = "rep_CFL";
		this.rep_CFL.SeparateCharacterForMulti = ';';
		this.rep_CFL.SeparateString = ", ";
		this.rep_CFL.UsingAutoCompleteBox = false;
		this.rep_CFL.UsingDataSource = true;
		this.rep_CFL.ValidateWhenNoDataSource = false;
		this.rep_CFL.ValueMember = "";
		this.gridColumnEx4.Caption = "Extened Employee No";
		this.gridColumnEx4.CurrencyDateField = null;
		this.gridColumnEx4.CurrencyField = null;
		this.gridColumnEx4.CurrencyFixed = null;
		this.gridColumnEx4.CurrencyFixedRate = 0.0;
		this.gridColumnEx4.CurrencyRateField = null;
		this.gridColumnEx4.FieldName = "ExtEmpNo";
		this.gridColumnEx4.Name = "gridColumnEx4";
		this.gridColumnEx4.OptionsColumn.ReadOnly = true;
		this.gridColumnEx4.PrimaryKey = false;
		this.gridColumnEx4.UnitField = null;
		this.gridColumnEx4.Width = 115;
		this.gc_empName.Caption = "Employee Name";
		this.gc_empName.ColumnEdit = this.rep_empName;
		this.gc_empName.CurrencyDateField = null;
		this.gc_empName.CurrencyField = null;
		this.gc_empName.CurrencyFixed = null;
		this.gc_empName.CurrencyFixedRate = 0.0;
		this.gc_empName.CurrencyRateField = null;
		this.gc_empName.FieldName = "empName";
		this.gc_empName.Name = "gc_empName";
		this.gc_empName.PrimaryKey = false;
		this.gc_empName.UnitField = null;
		this.gc_empName.Visible = true;
		this.gc_empName.VisibleIndex = 1;
		this.gc_empName.Width = 142;
		this.rep_empName.AutoHeight = false;
		this.rep_empName.AutoLoadDataSource = true;
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.rep_empName.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_empName.ContainerControl = this;
		this.rep_empName.DisplayMember = "";
		this.rep_empName.DisplayMember2 = "";
		this.rep_empName.ListMandatoryField = null;
		this.rep_empName.LoadDataSourceWhenEnter = false;
		this.rep_empName.Name = "rep_empName";
		this.rep_empName.SeparateCharacterForMulti = ';';
		this.rep_empName.SeparateString = ", ";
		this.rep_empName.UsingAutoCompleteBox = false;
		this.rep_empName.UsingDataSource = true;
		this.rep_empName.ValidateWhenNoDataSource = false;
		this.rep_empName.ValueMember = "";
		this.gridColumnEx1.Caption = "Valid From";
		this.gridColumnEx1.CurrencyDateField = null;
		this.gridColumnEx1.CurrencyField = null;
		this.gridColumnEx1.CurrencyFixed = null;
		this.gridColumnEx1.CurrencyFixedRate = 0.0;
		this.gridColumnEx1.CurrencyRateField = null;
		this.gridColumnEx1.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx1.FieldName = "ValidFrom";
		this.gridColumnEx1.Name = "gridColumnEx1";
		this.gridColumnEx1.PrimaryKey = false;
		this.gridColumnEx1.UnitField = null;
		this.gridColumnEx1.Visible = true;
		this.gridColumnEx1.VisibleIndex = 2;
		this.gridColumnEx1.Width = 153;
		this.gridColumnEx2.Caption = "Valid To";
		this.gridColumnEx2.CurrencyDateField = null;
		this.gridColumnEx2.CurrencyField = null;
		this.gridColumnEx2.CurrencyFixed = null;
		this.gridColumnEx2.CurrencyFixedRate = 0.0;
		this.gridColumnEx2.CurrencyRateField = null;
		this.gridColumnEx2.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.gridColumnEx2.FieldName = "ValidTo";
		this.gridColumnEx2.Name = "gridColumnEx2";
		this.gridColumnEx2.PrimaryKey = false;
		this.gridColumnEx2.UnitField = null;
		this.gridColumnEx2.Visible = true;
		this.gridColumnEx2.VisibleIndex = 3;
		this.gridColumnEx2.Width = 142;
		this.gridColumnEx5.Caption = "Schedule";
		this.gridColumnEx5.ColumnEdit = this.rep_Schedule;
		this.gridColumnEx5.CurrencyDateField = null;
		this.gridColumnEx5.CurrencyField = null;
		this.gridColumnEx5.CurrencyFixed = null;
		this.gridColumnEx5.CurrencyFixedRate = 0.0;
		this.gridColumnEx5.CurrencyRateField = null;
		this.gridColumnEx5.FieldName = "SchID";
		this.gridColumnEx5.Name = "gridColumnEx5";
		this.gridColumnEx5.PrimaryKey = false;
		this.gridColumnEx5.UnitField = null;
		this.gridColumnEx5.Visible = true;
		this.gridColumnEx5.VisibleIndex = 4;
		this.gridColumnEx5.Width = 172;
		this.rep_Schedule.AutoHeight = false;
		this.rep_Schedule.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Schedule.Name = "rep_Schedule";
		this.rep_Schedule.NullText = "";
		this.lbl_ValidTo.EditControl = null;
		this.lbl_ValidTo.EditValue = "Valid To";
		this.lbl_ValidTo.FieldName = null;
		this.lbl_ValidTo.HideWhenEmpty = false;
		this.lbl_ValidTo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ValidTo.Location = new System.Drawing.Point(328, 8);
		this.lbl_ValidTo.Name = "lbl_ValidTo";
		this.lbl_ValidTo.ReadOnly = false;
		this.lbl_ValidTo.SettingInVisible = false;
		this.lbl_ValidTo.SettingReadonly = false;
		this.lbl_ValidTo.ShowUnderLine = true;
		this.lbl_ValidTo.Size = new System.Drawing.Size(126, 20);
		this.lbl_ValidTo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ValidTo.TabIndex = 97;
		this.lbl_ValidTo.Utext = "Valid To";
		this.lbl_ValidFrom.AllowDrop = true;
		this.lbl_ValidFrom.EditControl = null;
		this.lbl_ValidFrom.EditValue = "Valid From";
		this.lbl_ValidFrom.FieldName = null;
		this.lbl_ValidFrom.HideWhenEmpty = false;
		this.lbl_ValidFrom.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_ValidFrom.Location = new System.Drawing.Point(2, 7);
		this.lbl_ValidFrom.Name = "lbl_ValidFrom";
		this.lbl_ValidFrom.ReadOnly = false;
		this.lbl_ValidFrom.SettingInVisible = false;
		this.lbl_ValidFrom.SettingReadonly = false;
		this.lbl_ValidFrom.ShowUnderLine = true;
		this.lbl_ValidFrom.Size = new System.Drawing.Size(117, 20);
		this.lbl_ValidFrom.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_ValidFrom.TabIndex = 96;
		this.lbl_ValidFrom.Utext = "Valid From";
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btnCancel.Appearance.Options.UseBackColor = true;
		this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btnCancel.EditControl = null;
		this.btnCancel.EditValue = "Cancel";
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(68, 392);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.ReadOnly = false;
		this.btnCancel.SettingInVisible = false;
		this.btnCancel.SettingReadonly = false;
		this.btnCancel.Size = new System.Drawing.Size(93, 20);
		this.btnCancel.TabIndex = 95;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btnCancel;
		base.ClientSize = new System.Drawing.Size(731, 416);
		base.Controls.Add(this.date_ValidTo);
		base.Controls.Add(this.date_ValidFrom);
		base.Controls.Add(this.btnSearch);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_Doc);
		base.Controls.Add(this.lbl_ValidTo);
		base.Controls.Add(this.lbl_ValidFrom);
		base.Controls.Add(this.btnCancel);
		base.FormId = "oEmployeeScheduleEdit";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_EmployeeScheduleEdit.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_EmployeeScheduleEdit";
		base.ShowInTaskbar = false;
		this.Text = "Employee Schedule Edit";
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemCalcEditEx1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidTo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ValidFrom.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Branch).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Deparment).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_CFL).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_empName).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Schedule).EndInit();
		base.ResumeLayout(false);
	}
}
