using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Helper.HubHelpers;
using Apzon.Forms.ImportData.ImportHelper;
using Apzon.Forms.ImportData.ImportHelper.BillOfMaterials;
using Apzon.Forms.ImportData.ImportHelper.BusinessPartners;
using Apzon.Forms.ImportData.ImportHelper.Calendar;
using Apzon.Forms.ImportData.ImportHelper.HumanResources;
using Apzon.Forms.ImportData.ImportHelper.Inventory;
using Apzon.Forms.ImportData.ImportHelper.PM;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraPrinting;

namespace Apzon.Forms.ImportData;

public class frm_ImportDataProcess : XtraFormEx
{
	public class ProgressReportObject
	{
		public int ObjectIndex;

		public ProgressReportStatus ObjectStatus;

		public DataTable DtError;

		public bool SetButtonCancel;

		public bool FinishWithRollBack;

		public int SuccessObject;
	}

	public enum ProgressReportStatus
	{
		Proccessing,
		Completed,
		RowProcessing,
		Error,
		Finish
	}

	public Enums.ImportActionEnum ImpAction;

	private readonly Color _rowColor = Color.FromArgb(239, 240, 242);

	private int _totalRows;

	private int _processedRows;

	private int _successRows;

	private int _errorRows;

	private DataTable _dtProcessedObject = new DataTable();

	private DataTable _dtError = new DataTable();

	private BackgroundWorker _bw = new BackgroundWorker();

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private string _hubStatus;

	private IContainer components;

	private GridControlEx grd_Errors;

	private GridViewEx gv_Errors;

	private GridColumnEx gridColumn9;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private RepositoryItemButtonEdit repositoryItemButtonEdit4;

	private RepositoryItemButtonEdit repositoryItemButtonEdit3;

	private Label lbl_Process;

	private ProgressBar prg_Process;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Export;

	private GridControlEx grd_Object;

	private GridViewEx gv_Object;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn3;

	private RepositoryItemButtonEdit rep_bte_Object;

	private RepositoryItemButtonEdit repositoryItemButtonEdit2;

	private GridColumnEx gridColumn2;

	private Label lbl_Status;

	private SimpleButtonEx btn_Cancel;

	public List<ImportDataTableObject> LstDtImportObject { get; set; }

	public Enums.ImportErrorHandlingEnum ErrorHandling { get; set; }

	public int IgnoreUpTo { get; set; }

	public frm_ImportDataProcess()
	{
		InitializeComponent();
	}

	private void frm_ImportDataProcess_Load(object sender, EventArgs e)
	{
		try
		{
			CloseBox = false;
			_bw.WorkerSupportsCancellation = true;
			_bw.WorkerReportsProgress = true;
			_bw.ProgressChanged += _bw_ProgressChanged;
			_bw.RunWorkerCompleted += _bw_RunWorkerCompleted;
			_bw.DoWork += _bw_DoWork;
			_dtError.Columns.Add(new DataColumn("Object", typeof(string)));
			_dtError.Columns.Add(new DataColumn("LineNo", typeof(int)));
			_dtError.Columns.Add(new DataColumn("Error", typeof(string)));
			_dtError.AcceptChanges();
			grd_Errors.DataSource = _dtError;
			grd_Errors.RefreshDataSource();
			_dtProcessedObject.Columns.Add(new DataColumn("Code", typeof(int)));
			_dtProcessedObject.Columns.Add(new DataColumn("Name", typeof(string)));
			_dtProcessedObject.Columns.Add(new DataColumn("Status", typeof(string)));
			for (int i = 0; i < LstDtImportObject.Count; i++)
			{
				DataRow dataRow = _dtProcessedObject.NewRow();
				dataRow["Code"] = (int)LstDtImportObject[i].ImportObject.Code;
				dataRow["Name"] = LstDtImportObject[i].ImportObject.Code.ToString();
				dataRow["Status"] = "Pending";
				_dtProcessedObject.Rows.Add(dataRow);
			}
			_dtProcessedObject.AcceptChanges();
			grd_Object.DataSource = _dtProcessedObject;
			grd_Object.RefreshDataSource();
			for (int j = 0; j < LstDtImportObject.Count; j++)
			{
				_totalRows += LstDtImportObject[j].ImportData.Rows.Count;
			}
			prg_Process.Maximum = _totalRows;
			prg_Process.Minimum = 0;
			prg_Process.Step = 1;
			prg_Process.Value = 0;
			lbl_Process.Text = "Row 0 of " + _totalRows + ". 0 Success";
			gv_Object.BestFitColumns();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void frm_ImportDataProcess_Shown(object sender, EventArgs e)
	{
		try
		{
			btn_OK.Enabled = false;
			btn_Export.Enabled = false;
			btn_Cancel.Enabled = true;
			await new HubClientHelper(this).CreateSocket<string>("ImportDataHub", new Dictionary<string, string> { { "UpdateStatus", "UpdateStatus" } });
			if (!_bw.IsBusy)
			{
				if (ErrorHandling == Enums.ImportErrorHandlingEnum.CancelOnError)
				{
					ProcessRollBackImport();
				}
				else
				{
					_bw.RunWorkerAsync();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void _bw_DoWork(object sender, DoWorkEventArgs e)
	{
		DoWork(sender, e);
	}

	private void _bw_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
		try
		{
			btn_OK.Enabled = true;
			btn_Export.Enabled = true;
			btn_Cancel.Enabled = false;
			if (e.Cancelled)
			{
				lbl_Status.Text = "User canceled!";
			}
			else if (e.Error != null)
			{
				lbl_Status.Text = "Error: " + e.Error.Message;
			}
			else
			{
				lbl_Status.Text = "Finished!";
			}
			gv_Object.BestFitColumns();
			DataTable dataTable = grd_Errors.DataSource as DataTable;
			btn_Export.Enabled = dataTable != null && dataTable.Rows.Count > 0;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void _bw_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
		try
		{
			ProgressReportObject progressReportObject = (ProgressReportObject)e.UserState;
			if (progressReportObject != null)
			{
				if (progressReportObject.SetButtonCancel)
				{
					btn_Cancel.Enabled = false;
				}
				else
				{
					lbl_Process.Text = "Row " + _processedRows + " of " + _totalRows + ". " + _successRows + " object(s) Success";
					lbl_Process.Update();
					prg_Process.Value = _processedRows;
					grd_Errors.DataSource = progressReportObject.DtError.Copy();
					grd_Errors.RefreshDataSource();
					grd_Errors.Update();
					if (progressReportObject.ObjectStatus == ProgressReportStatus.Completed || progressReportObject.ObjectStatus == ProgressReportStatus.Proccessing || progressReportObject.ObjectStatus == ProgressReportStatus.Error)
					{
						lbl_Status.Update();
						grd_Errors.Update();
						btn_OK.Update();
						btn_Export.Update();
						btn_Cancel.Update();
						_dtProcessedObject.Rows[progressReportObject.ObjectIndex]["Status"] = Function.ToString(progressReportObject.ObjectStatus);
						_dtProcessedObject.AcceptChanges();
						grd_Object.DataSource = _dtProcessedObject.Copy();
						grd_Object.RefreshDataSource();
						grd_Object.Update();
					}
				}
			}
			if (e.ProgressPercentage == 100)
			{
				lbl_Process.Text = "Row " + _totalRows + " of " + _totalRows + ". " + _successRows + " object(s) Success";
				prg_Process.Value = _totalRows;
				lbl_Status.Text = "Import Complete";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			_bw.CancelAsync();
		}
	}

	private void bt_Export_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.None;
		try
		{
			SaveFileDialog saveFileDialog = new SaveFileDialog
			{
				Filter = "Text Files (*.txt)|*.txt",
				DefaultExt = "txt"
			};
			saveFileDialog.ShowDialog(this);
			if (!string.IsNullOrEmpty(saveFileDialog.FileName))
			{
				gv_Errors.Export(ExportTarget.Text, saveFileDialog.FileName);
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
		DialogResult = DialogResult.None;
		try
		{
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCancelImportProcess, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo) == DialogResult.Yes && _bw.WorkerSupportsCancellation)
			{
				_bw.CancelAsync();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		DialogResult = DialogResult.OK;
	}

	private void gv_Errors_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.Column.Name != "Error")
			{
				e.Appearance.BackColor = _rowColor;
			}
			else
			{
				e.Appearance.BackColor = Color.White;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void DoWork(object sender, DoWorkEventArgs e)
	{
		WinCommons._OpenProcessing();
		try
		{
			DataTable dataTable = _dtError.Clone();
			bool flag = false;
			BackgroundWorker backgroundWorker = sender as BackgroundWorker;
			ProgressReportObject userState;
			for (int i = 0; i < LstDtImportObject.Count; i++)
			{
				userState = new ProgressReportObject
				{
					DtError = dataTable.Copy(),
					ObjectStatus = ProgressReportStatus.Proccessing,
					ObjectIndex = i
				};
				_bw.ReportProgress(1, userState);
				for (int j = 0; j < LstDtImportObject[i].ImportData.Rows.Count; j++)
				{
					if (backgroundWorker != null && backgroundWorker.CancellationPending)
					{
						e.Cancel = true;
						break;
					}
					_processedRows++;
					DataRow dataRow = LstDtImportObject[i].ImportData.Rows[j];
					DataTable mappingData = LstDtImportObject[i].MappingData;
					ImportObjectImpls importObject = LstDtImportObject[i].ImportObject;
					string value = ProcessData(dataRow, mappingData, importObject, LstDtImportObject);
					if (LstDtImportObject[i].ImportData.Rows.Count > j && LstDtImportObject[i].ImportData.Rows[j] != dataRow)
					{
						j--;
					}
					if (!string.IsNullOrEmpty(value))
					{
						DataRow dataRow2 = dataTable.NewRow();
						dataRow2["Object"] = LstDtImportObject[i].ImportObject.Code.ToString();
						dataRow2["LineNo"] = j + 1;
						dataRow2["Error"] = value;
						dataTable.Rows.Add(dataRow2);
						dataTable.AcceptChanges();
						_errorRows++;
						switch (ErrorHandling)
						{
						case Enums.ImportErrorHandlingEnum.CancelOnError:
							e.Cancel = true;
							flag = true;
							break;
						case Enums.ImportErrorHandlingEnum.IgnoreUpTo:
							if (IgnoreUpTo <= _errorRows)
							{
								e.Cancel = true;
								flag = true;
							}
							break;
						}
						if (flag)
						{
							break;
						}
					}
					else
					{
						_successRows++;
					}
					userState = new ProgressReportObject
					{
						DtError = dataTable.Copy(),
						ObjectStatus = ProgressReportStatus.RowProcessing,
						ObjectIndex = i
					};
					_bw.ReportProgress(1, userState);
					if (flag)
					{
						break;
					}
				}
				userState = new ProgressReportObject
				{
					DtError = dataTable.Copy(),
					ObjectStatus = ProgressReportStatus.Completed,
					ObjectIndex = i
				};
				_bw.ReportProgress(1, userState);
				if (e.Cancel)
				{
					break;
				}
			}
			userState = new ProgressReportObject
			{
				DtError = dataTable.Copy(),
				ObjectStatus = ProgressReportStatus.Finish,
				ObjectIndex = 0
			};
			if (!e.Cancel)
			{
				_bw.ReportProgress(100, userState);
				e.Result = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
	}

	private string ProcessData(DataRow dr, DataTable mappingDt, ImportObjectImpls importObject, List<ImportDataTableObject> lstDtImportObject)
	{
		try
		{
			DataTable dt = Apzon.Forms.ImportData.ImportHelper.ImportHelper.PrepareImportData(dr, mappingDt, importObject);
			bool isImportChild = false;
			BaseDataImport baseDataImport;
			switch (importObject.Code)
			{
			case Enums.ImportObjectEnum.BusinessPartners:
				baseDataImport = new BpMasterDataImport();
				break;
			case Enums.ImportObjectEnum.BPAddresses:
			case Enums.ImportObjectEnum.Contacts:
			case Enums.ImportObjectEnum.BPPaymentDates:
			case Enums.ImportObjectEnum.BPBankAccounts:
				baseDataImport = new BpMasterDataImport();
				isImportChild = true;
				break;
			case Enums.ImportObjectEnum.Items:
				baseDataImport = new ItemMasterDataImport();
				break;
			case Enums.ImportObjectEnum.Items_Prices:
			case Enums.ImportObjectEnum.ItemWarehouseInfo:
			case Enums.ImportObjectEnum.ItemUnitOfMeasurement:
				baseDataImport = new ItemMasterDataImport();
				isImportChild = true;
				break;
			case Enums.ImportObjectEnum.ProductTrees:
				baseDataImport = new BillOfMaterialsDataImport();
				break;
			case Enums.ImportObjectEnum.ProductTrees_Lines:
				baseDataImport = new BillOfMaterialsDataImport();
				isImportChild = true;
				break;
			case Enums.ImportObjectEnum.AdvanceBillOfMaterials:
				baseDataImport = new AdvanceBillOfMeterialsDataImport();
				break;
			case Enums.ImportObjectEnum.AdvanceBillOfMaterialsLines:
				baseDataImport = new AdvanceBillOfMeterialsDataImport();
				isImportChild = true;
				break;
			case Enums.ImportObjectEnum.EmployeesInfo:
				baseDataImport = new EmployeesDataImport();
				break;
			case Enums.ImportObjectEnum.EmployeesAbsenceInfo:
			case Enums.ImportObjectEnum.EmployeesEducationInfo:
			case Enums.ImportObjectEnum.EmployeesReviewsInfo:
			case Enums.ImportObjectEnum.EmployeesPrevEmpoymentInfo:
			case Enums.ImportObjectEnum.EmployeesRolesInfo:
			case Enums.ImportObjectEnum.EmployeesHealthInfo:
			case Enums.ImportObjectEnum.EmployeesContractsInfo:
				baseDataImport = new EmployeesDataImport();
				isImportChild = true;
				break;
			case Enums.ImportObjectEnum.EmployeesRelatives:
				baseDataImport = new EmployeeRelativesDataImport();
				break;
			case Enums.ImportObjectEnum.Task:
				baseDataImport = new TaskDataImport();
				break;
			case Enums.ImportObjectEnum.WBSTemplate:
				baseDataImport = new WBSTemplateDataImport();
				break;
			case Enums.ImportObjectEnum.WBSTemplateDetails:
			case Enums.ImportObjectEnum.WBSTemplateTask:
				baseDataImport = new WBSTemplateDataImport();
				isImportChild = true;
				break;
			default:
				return "Invalid import code '" + importObject.Code.ToString() + "'";
			}
			return baseDataImport.Import(dt, ImpAction, lstDtImportObject, isImportChild);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return ex.Message;
		}
	}

	protected override void OnFormClosing(FormClosingEventArgs e)
	{
		if (!_bw.CancellationPending)
		{
			_bw.CancelAsync();
		}
		base.OnFormClosing(e);
	}

	private void ProcessRollBackImport()
	{
		try
		{
			List<DataTable> list = new List<DataTable>();
			DataSet dataSet = new DataSet();
			DataTable dataTable = new DataTable();
			dataTable.TableName = "Action";
			dataTable.Columns.Add(new DataColumn("Action", typeof(int)));
			dataTable.Rows.Add(dataTable.NewRow());
			dataTable.AcceptChanges();
			dataTable.Rows[0][0] = (int)ImpAction;
			dataTable.AcceptChanges();
			DataTable dataTable2 = new DataTable();
			dataTable2.TableName = "Object";
			dataTable2.Columns.Add(new DataColumn("ImpObject", typeof(int)));
			for (int i = 0; i < LstDtImportObject.Count; i++)
			{
				DataRow dataRow = dataTable2.NewRow();
				dataRow[0] = (int)LstDtImportObject[i].ImportObject.Code;
				dataTable2.Rows.Add(dataRow);
				DataTable dataTableForRollBackImport = Apzon.Forms.ImportData.ImportHelper.ImportHelper.GetDataTableForRollBackImport(LstDtImportObject[i]);
				dataTableForRollBackImport.TableName = "Data" + i;
				list.Add(dataTableForRollBackImport);
			}
			dataTable2.AcceptChanges();
			list.Insert(0, dataTable);
			list.Insert(1, dataTable2);
			for (int j = 0; j < list.Count; j++)
			{
				DataSet dataSet2 = list[j].DataSet;
				if (dataSet2 != null)
				{
					dataSet2.Tables.Clear();
					dataSet2.AcceptChanges();
				}
			}
			dataSet.Tables.AddRange(list.ToArray());
			_httpClient.ImportDataClient.SaveWithRollBack(dataSet);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public void UpdateStatus(string status)
	{
		try
		{
			_hubStatus = status;
			if (_hubStatus == "OK")
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					_processedRows++;
					lbl_Process.Text = "Processed " + _processedRows + " objects of " + _totalRows + " Rows.";
					prg_Process.PerformStep();
					lbl_Process.Update();
				}));
			}
			else if (_hubStatus.StartsWith("ERR:"))
			{
				BeginInvoke((MethodInvoker)(() =>
				{
					string[] arrTmp = _hubStatus.Remove(0, 4).Split(';');
					DataTable dataTable = _dtError.Clone();
					DataRow dataRow = dataTable.NewRow();
					dataRow["Object"] = LstDtImportObject[0].ImportObject.Code.ToString();
					dataRow["LineNo"] = 1;
					dataRow["Error"] = arrTmp[1];
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
					grd_Errors.DataSource = dataTable.Copy();
					grd_Errors.RefreshDataSource();
					grd_Errors.Update();
					DataRow dataRow2 = _dtProcessedObject.AsEnumerable().FirstOrDefault((DataRow r) => Function.ParseInt(r["Code"]) == Function.ParseInt(arrTmp[0]));
					if (dataRow2 != null)
					{
						dataRow2["Status"] = "Error";
					}
					_dtProcessedObject.AcceptChanges();
					grd_Object.DataSource = _dtProcessedObject.Copy();
					grd_Object.RefreshDataSource();
					grd_Object.Update();
					lbl_Status.Text = "Finished!";
					btn_OK.Enabled = true;
					btn_Export.Enabled = true;
					btn_Cancel.Enabled = false;
					gv_Object.BestFitColumns();
					DataTable dataTable2 = grd_Errors.DataSource as DataTable;
					btn_Export.Enabled = dataTable2 != null && dataTable2.Rows.Count > 0;
				}));
			}
			else
			{
				if (!(_hubStatus == "END"))
				{
					return;
				}
				BeginInvoke((MethodInvoker)(() =>
				{
					for (int i = 0; i < _dtProcessedObject.Rows.Count && !(Function.ToString(_dtProcessedObject.Rows[i]["Status"]).ToLower() == "error"); i++)
					{
						_dtProcessedObject.Rows[i]["Status"] = "Completed";
					}
					_dtProcessedObject.AcceptChanges();
					grd_Object.DataSource = _dtProcessedObject.Copy();
					grd_Object.RefreshDataSource();
					grd_Object.Update();
					lbl_Status.Text = "Finished!";
					btn_OK.Enabled = true;
					btn_Export.Enabled = true;
					btn_Cancel.Enabled = false;
					gv_Object.BestFitColumns();
					DataTable dataTable = grd_Errors.DataSource as DataTable;
					btn_Export.Enabled = dataTable != null && dataTable.Rows.Count > 0;
				}));
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ImportData.frm_ImportDataProcess));
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions2 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions3 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled3 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions4 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled4 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions5 = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed5 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled5 = new DevExpress.Utils.SerializableAppearanceObject();
		this.grd_Errors = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Errors = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemButtonEdit4 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemButtonEdit3 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.lbl_Process = new System.Windows.Forms.Label();
		this.prg_Process = new System.Windows.Forms.ProgressBar();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Export = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Object = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Object = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_bte_Object = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemButtonEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.lbl_Status = new System.Windows.Forms.Label();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Errors).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Errors).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit3).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Object).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Object).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Object).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).BeginInit();
		base.SuspendLayout();
		this.grd_Errors.AllowPasteExcel = false;
		this.grd_Errors.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Errors.ClipboardData = null;
		this.grd_Errors.Location = new System.Drawing.Point(12, 209);
		this.grd_Errors.MainView = this.gv_Errors;
		this.grd_Errors.Name = "grd_Errors";
		this.grd_Errors.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemButtonEdit4, this.repositoryItemButtonEdit3 });
		this.grd_Errors.Size = new System.Drawing.Size(475, 204);
		this.grd_Errors.TabIndex = 21;
		this.grd_Errors.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Errors });
		this.gv_Errors.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Errors.ColumnKey = null;
		this.gv_Errors.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn9, this.gridColumn10, this.gridColumn11 });
		this.gv_Errors.DisableFormSetting = false;
		this.gv_Errors.DisplayException = false;
		this.gv_Errors.EditColumn = null;
		this.gv_Errors.FieldCurrency = null;
		this.gv_Errors.FieldRate = null;
		this.gv_Errors.FormId = "oImportProcess";
		this.gv_Errors.GridControl = this.grd_Errors;
		this.gv_Errors.IndexEditRow = -1;
		this.gv_Errors.IndicatorWidth = 35;
		this.gv_Errors.IsApzTable = false;
		this.gv_Errors.IsInsertNewRow = false;
		this.gv_Errors.IsLoadingData = false;
		this.gv_Errors.ItemId = "OERROR";
		this.gv_Errors.LstColumnAllowBatch = null;
		this.gv_Errors.LstColumnNotAllowBatch = null;
		this.gv_Errors.LstColumnNotEditable = null;
		this.gv_Errors.LstColumNotSetting = null;
		this.gv_Errors.Name = "gv_Errors";
		this.gv_Errors.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Errors.OptionsView.ShowGroupPanel = false;
		this.gv_Errors.UseBatchMod = false;
		this.gv_Errors.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Errors_RowCellStyle);
		this.gridColumn9.Caption = "Object";
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "Object";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.OptionsColumn.AllowEdit = false;
		this.gridColumn9.OptionsColumn.ReadOnly = true;
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 0;
		this.gridColumn9.Width = 141;
		this.gridColumn10.Caption = "Line No.";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "LineNo";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.OptionsColumn.AllowEdit = false;
		this.gridColumn10.OptionsColumn.ReadOnly = true;
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 1;
		this.gridColumn10.Width = 83;
		this.gridColumn11.Caption = "Error";
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "Error";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.OptionsColumn.ReadOnly = true;
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 2;
		this.gridColumn11.Width = 400;
		this.repositoryItemButtonEdit4.AutoHeight = false;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions1.Image");
		editorButtonImageOptions2.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.repositoryItemButtonEdit4.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, false, editorButtonImageOptions2, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEdit4.Name = "repositoryItemButtonEdit4";
		this.repositoryItemButtonEdit3.AutoHeight = false;
		editorButtonImageOptions3.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions3.Image");
		this.repositoryItemButtonEdit3.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions3, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance3, appearanceHovered3, appearancePressed3, appearanceDisabled3, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEdit3.Name = "repositoryItemButtonEdit3";
		this.lbl_Process.AutoSize = true;
		this.lbl_Process.Location = new System.Drawing.Point(280, 10);
		this.lbl_Process.Name = "lbl_Process";
		this.lbl_Process.Size = new System.Drawing.Size(83, 13);
		this.lbl_Process.TabIndex = 20;
		this.lbl_Process.Text = "Row 123 of 456";
		this.prg_Process.Anchor = System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.prg_Process.Location = new System.Drawing.Point(12, 30);
		this.prg_Process.Name = "prg_Process";
		this.prg_Process.Size = new System.Drawing.Size(475, 23);
		this.prg_Process.TabIndex = 19;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(12, 419);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(65, 20);
		this.btn_OK.TabIndex = 25;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Export.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Export.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Export.FieldName = null;
		this.btn_Export.IsSearchControl = false;
		this.btn_Export.Location = new System.Drawing.Point(361, 419);
		this.btn_Export.Name = "btn_Export";
		this.btn_Export.Size = new System.Drawing.Size(126, 20);
		this.btn_Export.TabIndex = 26;
		this.btn_Export.Text = "Export Errors to File";
		this.btn_Export.Click += new System.EventHandler(this.bt_Export_Click);
		this.grd_Object.AllowPasteExcel = false;
		this.grd_Object.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Object.ClipboardData = null;
		this.grd_Object.Location = new System.Drawing.Point(12, 69);
		this.grd_Object.MainView = this.gv_Object;
		this.grd_Object.Name = "grd_Object";
		this.grd_Object.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_bte_Object, this.repositoryItemButtonEdit2 });
		this.grd_Object.Size = new System.Drawing.Size(475, 125);
		this.grd_Object.TabIndex = 27;
		this.grd_Object.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Object });
		this.gv_Object.Appearance.FocusedCell.BackColor = System.Drawing.Color.FromArgb(239, 240, 242);
		this.gv_Object.Appearance.FocusedCell.Options.UseBackColor = true;
		this.gv_Object.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Object.ColumnKey = null;
		this.gv_Object.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn1, this.gridColumn3, this.gridColumn2 });
		this.gv_Object.DisableFormSetting = false;
		this.gv_Object.DisplayException = false;
		this.gv_Object.EditColumn = null;
		this.gv_Object.FieldCurrency = null;
		this.gv_Object.FieldRate = null;
		this.gv_Object.FormId = "oImportProcess";
		this.gv_Object.GridControl = this.grd_Object;
		this.gv_Object.IndexEditRow = -1;
		this.gv_Object.IndicatorWidth = 35;
		this.gv_Object.IsApzTable = false;
		this.gv_Object.IsInsertNewRow = false;
		this.gv_Object.IsLoadingData = false;
		this.gv_Object.ItemId = "OOBJECT";
		this.gv_Object.LstColumnAllowBatch = null;
		this.gv_Object.LstColumnNotAllowBatch = null;
		this.gv_Object.LstColumnNotEditable = null;
		this.gv_Object.LstColumNotSetting = null;
		this.gv_Object.Name = "gv_Object";
		this.gv_Object.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Object.OptionsView.ShowColumnHeaders = false;
		this.gv_Object.OptionsView.ShowGroupPanel = false;
		this.gv_Object.OptionsView.ShowIndicator = false;
		this.gv_Object.UseBatchMod = false;
		this.gridColumn1.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(239, 240, 242);
		this.gridColumn1.AppearanceCell.Options.UseBackColor = true;
		this.gridColumn1.Caption = "Object";
		this.gridColumn1.ColumnEdit = this.rep_bte_Object;
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Name";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.rep_bte_Object.AutoHeight = false;
		editorButtonImageOptions4.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions4.Image");
		this.rep_bte_Object.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions4, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance4, appearanceHovered4, appearancePressed4, appearanceDisabled4, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.rep_bte_Object.Name = "rep_bte_Object";
		this.gridColumn3.AppearanceCell.BackColor = System.Drawing.Color.FromArgb(239, 240, 242);
		this.gridColumn3.AppearanceCell.Options.UseBackColor = true;
		this.gridColumn3.Caption = "Status";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Status";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.OptionsColumn.AllowEdit = false;
		this.gridColumn3.OptionsColumn.ReadOnly = true;
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn2.Caption = "Code";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Code";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.AllowEdit = false;
		this.gridColumn2.OptionsColumn.ReadOnly = true;
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.repositoryItemButtonEdit2.AutoHeight = false;
		editorButtonImageOptions5.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions5.Image");
		this.repositoryItemButtonEdit2.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions5, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance5, appearanceHovered5, appearancePressed5, appearanceDisabled5, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.repositoryItemButtonEdit2.Name = "repositoryItemButtonEdit2";
		this.lbl_Status.AutoSize = true;
		this.lbl_Status.Location = new System.Drawing.Point(12, 10);
		this.lbl_Status.Name = "lbl_Status";
		this.lbl_Status.Size = new System.Drawing.Size(65, 13);
		this.lbl_Status.TabIndex = 28;
		this.lbl_Status.Text = "Importing...";
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(81, 419);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.Size = new System.Drawing.Size(65, 20);
		this.btn_Cancel.TabIndex = 29;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(501, 447);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.lbl_Status);
		base.Controls.Add(this.grd_Object);
		base.Controls.Add(this.btn_Export);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.grd_Errors);
		base.Controls.Add(this.lbl_Process);
		base.Controls.Add(this.prg_Process);
		base.FormId = "oImportProcess";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ImportDataProcess";
		this.Text = "Import Data Process";
		base.Load += new System.EventHandler(this.frm_ImportDataProcess_Load);
		base.Shown += new System.EventHandler(this.frm_ImportDataProcess_Shown);
		((System.ComponentModel.ISupportInitialize)this.grd_Errors).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Errors).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit3).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Object).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Object).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_bte_Object).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemButtonEdit2).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
