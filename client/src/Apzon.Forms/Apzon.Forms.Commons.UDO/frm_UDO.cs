using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Export;
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

namespace Apzon.Forms.Commons.UDO;

public class frm_UDO : XtraFormEx
{
	private string uCode;

	private string udoType;

	private DataTable udoTable;

	private DataTable dtChild;

	private DataTable dtSearch;

	private List<UserFieldsImpl> lstUDF;

	private List<UserFieldsImpl> lstUDFHeader;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private bool isLoadDetail;

	private bool isNotLoadDetail;

	public List<Formular> Formulars = new List<Formular>();

	public Dictionary<string, GridViewEx> TblFormular = new Dictionary<string, GridViewEx>();

	private BackgroundWorker bgwExcel = new BackgroundWorker();

	private XtraTabControl XtraTabLine = new XtraTabControl();

	private AuthsImpl _auth;

	public object ChoosedEntry;

	private IContainer components;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private PanelControl panelControl1;

	private SplitContainerControl split_Main;

	private LabelLine lb_FromDate;

	private LabelLine lb_ToDate;

	private SimpleButtonEx btn_Search;

	private TextEditEx txt_DocNo;

	private LabelLine lb_DocNo;

	private DateEditEx date_FromDate;

	private DateEditEx date_ToDate;

	private ContextMenuStrip ctx_Doc;

	private ToolStripMenuItem addNewToolStripMenuItem;

	private ToolStripMenuItem duplicateToolStripMenuItem;

	private ToolStripMenuItem removeToolStripMenuItem;

	private ToolStripMenuItem closeToolStripMenuItem;

	private ToolStripMenuItem cancelToolStripMenuItem;

	private SimpleButton btn_Choose;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private int _fromDocEntry { get; set; }

	private string _fromCode { get; set; }

	private DataTable OpenFormHeader { get; set; }

	private ContextActionMappingImpl[] Mapping { get; set; }

	private DataTable OpenFormLines { get; set; }

	public DataRow dataRow { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
		{
			btn_OK.Text = "Add";
			GridColumn gridColumn2 = gv_Doc.Columns.FirstOrDefault((GridColumn c) => c.FieldName == "Series");
			if (gridColumn2 == null || !(Function.ToString(udoTable.Rows[0]["MngSeries"]) == "Y"))
			{
				break;
			}
			RepositoryItemLookUpEdit repositoryItemLookUpEdit = new RepositoryItemLookUpEdit();
			string valueMember = (repositoryItemLookUpEdit.DisplayMember = "Series");
			repositoryItemLookUpEdit.ValueMember = valueMember;
			DataTable result = Task.Run(async () => await _httpClient.Udo.GetSeriesByUser(uCode)).Result;
			repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("Series", "Series"));
			repositoryItemLookUpEdit.Columns.Add(new LookUpColumnInfo("SeriesName", "Series Name"));
			repositoryItemLookUpEdit.DataSource = result;
			repositoryItemLookUpEdit.NullText = "";
			repositoryItemLookUpEdit.EditValueChanged += async (object sender, EventArgs e) =>
			{
				int series2 = Function.ParseInt((sender as LookUpEdit).EditValue);
				string value2 = await _httpClient.Udo.GetNextNumber(uCode, series2);
				(grd_Doc.DataSource as DataTable).Rows[0][(udoType == "3") ? "DocNum" : "Code"] = value2;
				grd_Doc.RefreshDataSource();
			};
			grd_Doc.RepositoryItems.Add(repositoryItemLookUpEdit);
			gridColumn2.OptionsColumn.ReadOnly = false;
			gridColumn2.OptionsColumn.AllowEdit = true;
			gridColumn2.ColumnEdit = repositoryItemLookUpEdit;
			if (result.Rows.Count > 0)
			{
				gv_Doc.SetFocusedRowCellValue("Series", result.Rows[0]["Default"]);
				int series = Function.ParseInt(Function.ParseInt(result.Rows[0]["Default"]));
				string result2 = Task.Run(async () => await _httpClient.Udo.GetNextNumber(uCode, series)).Result;
				(grd_Doc.DataSource as DataTable).Rows[0][(udoType == "3") ? "DocNum" : "Code"] = result2;
				grd_Doc.RefreshDataSource();
			}
			break;
		}
		case Enums.FormViewModeType.Update:
			btn_OK.Text = "Save";
			break;
		case Enums.FormViewModeType.View:
		{
			btn_OK.Text = "OK";
			GridColumn gridColumn = gv_Doc.Columns.FirstOrDefault((GridColumn c) => c.FieldName == "Series");
			if (gridColumn != null)
			{
				gridColumn.ColumnEdit = null;
				gridColumn.OptionsColumn.ReadOnly = true;
				gridColumn.OptionsColumn.AllowEdit = false;
			}
			break;
		}
		}
	}

	public frm_UDO()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_UDO"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_UDO" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public frm_UDO(string udoCode, int docEntry = -1, string code = "")
	{
		udoCode = (udoCode.StartsWith("UDO_") ? udoCode : ("UDO_" + udoCode));
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == udoCode))
		{
			_auth = GlobalData.ListAccessForm.FirstOrDefault((AuthsImpl x) => x.menucode == udoCode);
			if (_auth.authoriza == "N")
			{
				throw new Exception(Resources.Msg_FormAccessDenied);
			}
			InitializeComponent();
			gv_Doc.NotLoadFormsettingAtLoad = true;
			uCode = (udoCode.StartsWith("UDO_") ? udoCode.Substring(4, udoCode.Length - 4) : udoCode);
			FormId = uCode;
			_fromDocEntry = docEntry;
			_fromCode = code;
			OnLoadAsync += frm_UDO_Load;
			if (_auth.authoriza != "F")
			{
				btn_OK.Visible = false;
				panelControl1.Visible = false;
				split_Main.Height = 407;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public void SetMapping(DataTable header, DataTable lines, ContextActionMappingImpl[] mapping)
	{
		OpenFormHeader = header;
		OpenFormLines = lines;
		Mapping = mapping;
	}

	public async Task REFRESH()
	{
		try
		{
			addNewToolStripMenuItem.PerformClick();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString());
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		try
		{
			if ("Choose".Equals(btn_OK.Text))
			{
				if (gv_Doc.FocusedRowHandle != -2147483647)
				{
					if (udoType.Equals("3"))
					{
						ChoosedEntry = Function.ParseInt(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "DocEntry"));
					}
					else
					{
						ChoosedEntry = Function.ToString(gv_Doc.GetRowCellValue(gv_Doc.FocusedRowHandle, "Code"));
					}
				}
				Close();
				return;
			}
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
			{
				return;
			}
			btn_OK.Enabled = false;
			btn_Search.Enabled = false;
			WinCommons._OpenProcessing();
			DataSet ds = new DataSet();
			ds.Tables.Clear();
			ds.Tables.Add(udoTable.Copy());
			if (split_Main.Panel1.Controls.Count > 0)
			{
				int rowIndex = gv_Doc.FocusedRowHandle;
				int num = Function.ParseInt(gv_Doc.GetRowCellValue(rowIndex, "DocEntry"));
				if (FormMode == Enums.FormViewModeType.Add)
				{
					num = 0;
				}
				if (grd_Doc.DataSource != null)
				{
					DataTable dataTable = (DataTable)grd_Doc.DataSource;
					dataTable = dataTable.Select("DocEntry = " + num).CopyToDataTable();
					string tableName = grd_Doc.Name.Substring(5, grd_Doc.Name.Length - 5);
					if (dataTable.IsExistsColumn("Status") && Function.ToString(dataTable.Rows[0]["Status"]) == "L")
					{
						dataTable.Rows[0]["Status"] = "C";
					}
					dataTable.TableName = tableName;
					ds.Tables.Add(dataTable);
					if (split_Main.Panel2.Controls.Count > 0)
					{
						XtraTabControl xtraTabControl = (XtraTabControl)split_Main.Panel2.Controls[0];
						if (xtraTabControl.TabPages.Count > 0)
						{
							for (int i = 0; i < xtraTabControl.TabPages.Count; i++)
							{
								if (xtraTabControl.TabPages[i].Controls.Count > 0)
								{
									GridControlEx gridControlEx = (GridControlEx)xtraTabControl.TabPages[i].Controls[0];
									DataTable dataTable2 = (DataTable)gridControlEx.DataSource;
									if (dataTable2 != null)
									{
										tableName = gridControlEx.Name.Substring(5, gridControlEx.Name.Length - 5);
										dataTable2.TableName = tableName;
										ds.Tables.Add(dataTable2);
									}
								}
							}
						}
					}
					HttpResult httpResult = await _httpClient.Udo.Save(ds);
					ds.Tables.Clear();
					ds.Dispose();
					if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
					{
						WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
						FormMode = Enums.FormViewModeType.View;
						if (udoType.Equals("3") && _fromDocEntry > -1)
						{
							if (Function.ParseInt(httpResult.Content) > 0)
							{
								ChoosedEntry = httpResult.Content;
							}
							else
							{
								ChoosedEntry = _fromDocEntry;
							}
							Close();
						}
						else if (udoType.Equals("3") && _fromDocEntry > -1 && httpResult.Content == null)
						{
							ChoosedEntry = _fromDocEntry;
							Close();
						}
						else if (udoType.Equals("1") && _fromDocEntry != -1)
						{
							if (httpResult.Content != null)
							{
								ChoosedEntry = httpResult.Content;
							}
							else
							{
								ChoosedEntry = _fromCode;
							}
							Close();
						}
						else
						{
							btn_Search_Click(null, null);
							gv_Doc.FocusedRowHandle = rowIndex;
						}
					}
					else if (httpResult != null)
					{
						WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.UnknownMessage, "");
					}
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_OK.Enabled = true;
		btn_Search.Enabled = true;
	}

	private async Task frm_UDO_Load(object sender, EventArgs e)
	{
		_ = 5;
		try
		{
			date_FromDate.EditValue = DateTime.Today.AddDays(-Configuration.SearchDate);
			date_ToDate.EditValue = DateTime.Today.AddDays(Configuration.SearchDate);
			DateTime fromDate = (string.IsNullOrEmpty(date_FromDate.Text) ? DateTime.Today : Convert.ToDateTime(date_FromDate.EditValue));
			DateTime toDate = (string.IsNullOrEmpty(date_ToDate.Text) ? DateTime.Today : Convert.ToDateTime(date_ToDate.EditValue));
			FormMode = Enums.FormViewModeType.View;
			btn_OK.Enabled = false;
			btn_Search.Enabled = false;
			udoTable = await _httpClient.Udo.GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = uCode
			});
			if (udoTable == null || udoTable.Rows.Count == 0)
			{
				return;
			}
			if (Text.Equals("User Defined Object Form"))
			{
				Text = Function.ToString(udoTable.Rows[0]["MenuCapt"]);
			}
			string docTblName = "@" + Function.ToString(udoTable.Rows[0]["TableName"]);
			udoType = Function.ToString(udoTable.Rows[0]["TYPE"]);
			if (udoType.Equals("1"))
			{
				lb_DocNo.Utext = "Code";
				txt_DocNo.ToolTip = "Master data Code";
				txt_DocNo.Properties.Mask.EditMask = "";
				txt_DocNo.Properties.Mask.MaskType = MaskType.None;
			}
			lstUDFHeader = await _httpClient.Udf.GetListField(docTblName);
			new DataSet();
			grd_Doc.Name = "grd_" + docTblName;
			grd_Doc.Text = docTblName;
			grd_Doc.Dock = DockStyle.Fill;
			grd_Doc.ContextMenuStrip = ctx_Doc;
			gv_Doc.Name = "gv_" + docTblName;
			gv_Doc.Tag = docTblName;
			gv_Doc.FormId = uCode;
			gv_Doc.ItemId = docTblName;
			gv_Doc.FocusedRowObjectChanged += gv_Doc_FocusedRowObjectChanged;
			gv_Doc.BeforeLeaveRow += gv_Doc_BeforeLeaveRow;
			gv_Doc.InitNewRow += gv_Doc_InitNewRow;
			gv_Doc.PopupMenuShowing += gv_Doc_PopupMenuShowing;
			gv_Doc.RowCellStyle += gv_Doc_RowCellStyle;
			gv_Doc.CellValueChanged += gv_Doc_CellValueChanged;
			gv_Doc.ShowingEditor += gv_Doc_CellShowingEditor;
			if (_fromDocEntry == 0)
			{
				btn_OK.Text = "Choose";
				fromDate = new DateTime(1900, 1, 1);
				gv_Doc.DoubleClick += gv_Doc_DoubleClick;
			}
			if (_auth.authoriza != "F")
			{
				gv_Doc.DisableFormSetting = true;
				gv_Doc.ReadOnly = true;
			}
			DataSet dsHeader = await GetData(uCode, docTblName, _fromDocEntry, fromDate, toDate, GlobalData.UserName, (GlobalData.DefaultBranch == 0) ? (-1) : GlobalData.DefaultBranch, _fromCode);
			TblFormular[docTblName] = gv_Doc;
			if (dsHeader.IsHasStructure())
			{
				dsHeader.Tables[1].TableName = docTblName;
				if (OpenFormHeader.IsNotNull() && Mapping.Length != 0)
				{
					if (udoType.Equals("3") && _fromDocEntry == 0)
					{
						dsHeader.Tables[1].Rows.Clear();
						DataRow dataRow = dsHeader.Tables[1].Rows.Add();
						if (this.dataRow != null)
						{
							WinFuntions.DataRowToDataRow(this.dataRow, dataRow);
						}
						if (dataRow.IsExistsColumn("DocEntry"))
						{
							dataRow["DocEntry"] = Convert.ToInt64(0);
						}
						if (dataRow.IsExistsColumn("Status"))
						{
							dataRow["Status"] = "O";
						}
						FormMode = Enums.FormViewModeType.Add;
					}
					else if (udoType.Equals("1") && _fromCode == "")
					{
						dsHeader.Tables[1].Rows.Clear();
						DataRow dataRow2 = dsHeader.Tables[1].Rows.Add();
						if (this.dataRow != null)
						{
							WinFuntions.DataRowToDataRow(this.dataRow, dataRow2);
						}
						if (dataRow2.IsExistsColumn("DocEntry"))
						{
							dataRow2["DocEntry"] = Convert.ToInt64(0);
						}
						if (dataRow2.IsExistsColumn("Status"))
						{
							dataRow2["Status"] = "O";
						}
						FormMode = Enums.FormViewModeType.Add;
					}
				}
				int docSearch = 0;
				string codeSearch = "";
				if (udoType.Equals("3") && dsHeader.Tables[1].IsNotNull() && dsHeader.Tables[1].Columns.Contains("DocEntry"))
				{
					docSearch = Function.ParseInt(dsHeader.Tables[1].Rows[0]["DocEntry"]);
				}
				else if (udoType.Equals("1") && dsHeader.Tables[1].IsNotNull() && dsHeader.Tables[1].Columns.Contains("Code"))
				{
					codeSearch = Function.ToString(dsHeader.Tables[1].Rows[0]["Code"]);
				}
				await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, dsHeader, lstUDFHeader, expand: false, filterRow: false, docTblName, TblFormular);
				dsHeader.Tables.Clear();
				dsHeader.Dispose();
				grd_Doc.Enabled = false;
				Formulars = await WinFuntions.GetFormulas(uCode);
				gv_Doc.InitFormular(Formulars, TblFormular);
				split_Main.Panel2.Controls.Add(XtraTabLine);
				await LoadGridViewLines(gv_Doc, uCode, docSearch, codeSearch, fromDate, toDate);
				if (OpenFormHeader.IsNotNull() && Mapping.Length != 0)
				{
					ContextActionMappingImpl[] array = Mapping.Where((ContextActionMappingImpl t) => t.TargetItem == uCode).ToArray();
					ContextActionMappingSource.MappingValue(OpenFormHeader.DefaultView.ToTable(distinct: true, (from t in array
						where !string.IsNullOrEmpty(t.SourceField)
						select t.SourceField).Distinct().ToArray()), array, 0, gv_Doc);
				}
			}
			grd_Doc.Enabled = true;
			btn_OK.Enabled = true;
			btn_Search.Enabled = true;
			bgwExcel.DoWork += bgw_Excel_DoWork;
			bgwExcel.ProgressChanged += bgw_Excel_ProgressChanged;
			bgwExcel.RunWorkerCompleted += bgw_Excel_RunWorkerCompleted;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bgw_Excel_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
	{
	}

	private void bgw_Excel_ProgressChanged(object sender, ProgressChangedEventArgs e)
	{
	}

	private void bgw_Excel_DoWork(object sender, DoWorkEventArgs e)
	{
		try
		{
			if (sender is BackgroundWorker backgroundWorker)
			{
				backgroundWorker.WorkerReportsProgress = true;
				e.Result = Run(e.Argument, backgroundWorker, e);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private long Run(object argument, BackgroundWorker worker, DoWorkEventArgs e)
	{
		if (worker.CancellationPending)
		{
			e.Cancel = true;
		}
		else
		{
			try
			{
				if (argument is Tuple<GridViewEx, DataObject> tuple)
				{
					PasteFromExcel(tuple.Item1, tuple.Item2);
				}
			}
			catch (Exception ex)
			{
				Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
				WinCommons.ErrorLineNumber(ex);
			}
		}
		return 1L;
	}

	private void gv_Doc_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			if (!"Choose".Equals(btn_OK.Text))
			{
				return;
			}
			GridViewEx gridViewEx = (GridViewEx)((GridControlEx)split_Main.Panel1.Controls[0]).MainView;
			if (gridViewEx.FocusedRowHandle != -2147483647)
			{
				if (udoType.Equals("3"))
				{
					ChoosedEntry = Function.ParseInt(gridViewEx.GetRowCellValue(gridViewEx.FocusedRowHandle, "DocEntry"));
				}
				else
				{
					ChoosedEntry = Function.ToString(gridViewEx.GetRowCellValue(gridViewEx.FocusedRowHandle, "Code"));
				}
			}
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_CellShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if ((sender as GridViewEx).FocusedColumn.FieldName.Equals("DocEntry"))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (isLoadDetail)
			{
				e.Allow = false;
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
			{
				e.Allow = XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
			}
			if (e.Allow && FormMode == Enums.FormViewModeType.Add)
			{
				GridViewEx gridViewEx = (GridViewEx)sender;
				gridViewEx.BeginDataUpdate();
				gridViewEx.DeleteRow(e.RowHandle);
				gridViewEx.EndDataUpdate();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		if (isNotLoadDetail)
		{
			return;
		}
		WinCommons._OpenProcessing();
		try
		{
			isLoadDetail = true;
			GridViewEx gridViewEx = (GridViewEx)sender;
			int docentry = 0;
			string code = "";
			if (gridViewEx.FocusedRowHandle != -2147483647)
			{
				if (udoType.Equals("3"))
				{
					docentry = Function.ParseInt(gridViewEx.GetRowCellValue(gridViewEx.FocusedRowHandle, "DocEntry"));
				}
				else
				{
					code = Function.ToString(gridViewEx.GetRowCellValue(gridViewEx.FocusedRowHandle, "Code"));
				}
			}
			if (split_Main.Panel2.Controls.Count > 0)
			{
				XtraTabControl tab = (XtraTabControl)split_Main.Panel2.Controls[0];
				if (tab.TabPages.Count > 0)
				{
					for (int i = 0; i < tab.TabPages.Count; i++)
					{
						if (tab.TabPages[i].Controls.Count > 0)
						{
							GridControlEx grd_Line = (GridControlEx)tab.TabPages[i].Controls[0];
							GridViewEx gv_Line = (GridViewEx)grd_Line.MainView;
							string tableName = grd_Line.Name.Substring(4, grd_Line.Name.Length - 4);
							DataSet dsDetail = await GetData(uCode, tableName, docentry, DateTime.Today, DateTime.Today, GlobalData.UserName, GlobalData.DefaultBranch, code);
							if (dsDetail.IsHasStructure())
							{
								dsDetail.Tables[1].TableName = tableName;
								await GridViewHelper.GenerateGrid(grd_Line, gv_Line, dsDetail, lstUDF, expand: false, filterRow: false, tableName, TblFormular);
							}
							dsDetail.Tables.Clear();
							dsDetail.Dispose();
						}
					}
				}
			}
			if (!btn_OK.Text.Equals("Choose") && ((udoType.Equals("3") && docentry > 0) || (udoType.Equals("1") && code != "")))
			{
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		isLoadDetail = false;
		WinCommons._CloseProcessing();
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void grd_Line_EditorKeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.V && e.Modifiers == Keys.Control)
			{
				WinCommons._OpenProcessing();
				GridViewEx item = (GridViewEx)((GridControlEx)sender).MainView;
				DataObject dataObject = (DataObject)Clipboard.GetDataObject();
				if (dataObject.GetDataPresent(DataFormats.Text))
				{
					bgwExcel.RunWorkerAsync(new Tuple<GridViewEx, DataObject>(item, dataObject));
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = (GridViewEx)sender;
			DataTable dataTable = (DataTable)((GridControlEx)gridViewEx.GridControl).DataSource;
			if (dataTable != null && dataTable.Rows.Count > 0 && dataTable.Rows[0].IsExistsColumn("Status") && !Function.ToString(gridViewEx.GetRowCellValue(e.RowHandle, "Status")).Equals("O"))
			{
				e.Appearance.BackColor = Color.LightGray;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = (GridViewEx)sender;
			if (_auth.authoriza != "F")
			{
				foreach (object item in gridViewEx.GridControl.ContextMenuStrip.Items)
				{
					if (item is ToolStripMenuItem toolStripMenuItem && !toolStripMenuItem.Name.ToUpper().Equals("CTM_COPYTABLE") && !toolStripMenuItem.Text.ToUpper().Equals("COPY TABLE") && !toolStripMenuItem.Name.ToUpper().Equals("CTM_COPYROWS") && !toolStripMenuItem.Text.ToUpper().Equals("COPY ROW(S)"))
					{
						toolStripMenuItem.Visible = false;
					}
				}
				return;
			}
			if (udoTable == null || udoTable.Rows.Count == 0)
			{
				addNewToolStripMenuItem.Visible = false;
				duplicateToolStripMenuItem.Visible = false;
				removeToolStripMenuItem.Visible = false;
				closeToolStripMenuItem.Visible = false;
				cancelToolStripMenuItem.Visible = false;
				return;
			}
			duplicateToolStripMenuItem.Visible = _fromDocEntry < 0 && gridViewEx.FocusedRowHandle > -1;
			removeToolStripMenuItem.Visible = udoTable.Rows[0]["CanDelete"] != null && Function.ToString(udoTable.Rows[0]["CanDelete"]).Equals("Y");
			closeToolStripMenuItem.Visible = udoTable.Rows[0]["CanClose"] != null && Function.ToString(udoTable.Rows[0]["CanClose"]).Equals("Y");
			cancelToolStripMenuItem.Visible = udoTable.Rows[0]["CanCancel"] != null && Function.ToString(udoTable.Rows[0]["CanCancel"]).Equals("Y");
			if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void docMenu_Click(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			ToolStripMenuItem toolStripMenuItem = sender as ToolStripMenuItem;
			if (split_Main.Panel1.Controls.Count == 0)
			{
				return;
			}
			GridControlEx gridControlEx = (GridControlEx)split_Main.Panel1.Controls[0];
			GridViewEx gv_Doc = (GridViewEx)gridControlEx.MainView;
			if (toolStripMenuItem == null)
			{
				return;
			}
			if (toolStripMenuItem.Text.Equals("Add New"))
			{
				isNotLoadDetail = true;
				if (gridControlEx.DataSource is DataTable dataTable)
				{
					DataTable dataTable2 = dataTable.Clone();
					DataRow dataRow = dataTable2.NewRow();
					if (dataRow.IsExistsColumn("DocEntry"))
					{
						dataRow["DocEntry"] = Convert.ToInt64(0);
					}
					if (dataRow.IsExistsColumn("Status"))
					{
						dataRow["Status"] = "O";
					}
					dataTable2.Rows.Add(dataRow);
					gridControlEx.DataSource = dataTable2;
					gridControlEx.RefreshDataSource();
					gv_Doc.FocusedRowHandle = dataTable2.Rows.IndexOf(dataRow);
					ClearDetails();
					txt_DocNo.Text = "";
					FormMode = Enums.FormViewModeType.Add;
					isNotLoadDetail = false;
				}
			}
			else if (toolStripMenuItem.Text.Equals("Duplicate"))
			{
				isNotLoadDetail = true;
				gv_Doc.ActiveFilterString = "";
				gv_Doc.FindFilterText = "";
				gv_Doc.ActiveFilter.Clear();
				if (gv_Doc.FocusedRowHandle > -1)
				{
					DataRow focusedDataRow = gv_Doc.GetFocusedDataRow();
					DataTable dataTable3 = gridControlEx.DatasourceTable();
					DataRow dataRow2 = dataTable3.NewRow();
					dataRow2.ItemArray = focusedDataRow.ItemArray;
					if (dataRow2.IsExistsColumn("DocEntry"))
					{
						dataRow2["DocEntry"] = Convert.ToInt64(0);
					}
					if (dataRow2.IsExistsColumn("Code"))
					{
						dataRow2["Code"] = "";
					}
					dataTable3.Rows.Add(dataRow2);
					gridControlEx.RefreshDataSource();
					gv_Doc.FocusedRowHandle = dataTable3.Rows.IndexOf(dataRow2);
					XtraTabControl xtraTabControl = (XtraTabControl)split_Main.Panel2.Controls[0];
					if (xtraTabControl.TabPages.Count > 0)
					{
						for (int i = 0; i < xtraTabControl.TabPages.Count; i++)
						{
							if (xtraTabControl.TabPages[i].Controls.Count <= 0)
							{
								continue;
							}
							DataTable dataTable4 = ((GridControlEx)xtraTabControl.TabPages[i].Controls[0]).DataSourceTable();
							if (!dataTable4.IsNotNull())
							{
								continue;
							}
							if (udoType.Equals("3"))
							{
								if (!dataTable4.IsExistsColumn("DocEntry"))
								{
									continue;
								}
								foreach (DataRow row in dataTable4.Rows)
								{
									row["DocEntry"] = Convert.ToInt64(0);
								}
							}
							else
							{
								if (!dataTable4.IsExistsColumn("Code"))
								{
									continue;
								}
								foreach (DataRow row2 in dataTable4.Rows)
								{
									row2["Code"] = "";
								}
							}
						}
					}
					txt_DocNo.Text = "";
					FormMode = Enums.FormViewModeType.Add;
				}
				isNotLoadDetail = false;
			}
			else if (toolStripMenuItem.Text.Equals("Remove"))
			{
				if (gv_Doc.FocusedRowHandle < 0 || gv_Doc.RowCount < 1)
				{
					WinCommons.Error(Enums.MessageCode.DocumentHeaderNotFocus, "");
					return;
				}
				_ = gv_Doc.FocusedRowHandle;
				int num = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				if (num > 0)
				{
					if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
					{
						DataSet ds = new DataSet();
						ds.Tables.Clear();
						ds.Tables.Add(udoTable);
						DataTable dataTable5 = (DataTable)gridControlEx.DataSource;
						dataTable5 = dataTable5.Select("DocEntry = " + num).CopyToDataTable();
						ds.Tables.Add(dataTable5);
						string text = await OperatorDoc(ds, "DeleteDoc");
						ds.Tables.Clear();
						ds.Dispose();
						if (string.IsNullOrEmpty(text))
						{
							btn_Search_Click(null, null);
							WinCommons.writeStatus(Enums.MessageCode.DeleteSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
						}
						else
						{
							WinCommons.Error(Enums.MessageCode.None, text);
						}
					}
				}
				else
				{
					gv_Doc.DeleteRow(gv_Doc.FocusedRowHandle);
				}
			}
			else if (toolStripMenuItem.Text.Equals("Close"))
			{
				if (gv_Doc.FocusedRowHandle < 0 || gv_Doc.RowCount < 1)
				{
					WinCommons.Error(Enums.MessageCode.DocumentHeaderNotFocus, "");
					return;
				}
				if (GlobalData.ListAccessForm.Where((AuthsImpl f) => f.menucode == "barSub_CloseDocument" && f.authoriza == "F").FirstOrDefault() == null)
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.NoPermissionPerformAction, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				int rowIndex = gv_Doc.FocusedRowHandle;
				int num2 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				if (num2 > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					DataSet ds = new DataSet();
					ds.Tables.Clear();
					ds.Tables.Add(udoTable);
					DataTable dataTable6 = (DataTable)gridControlEx.DataSource;
					dataTable6 = dataTable6.Select("DocEntry = " + num2).CopyToDataTable();
					ds.Tables.Add(dataTable6);
					string text2 = await OperatorDoc(ds, "CloseDoc");
					ds.Tables.Clear();
					ds.Dispose();
					if (string.IsNullOrEmpty(text2))
					{
						btn_Search_Click(null, null);
						gv_Doc.FocusedRowHandle = rowIndex;
						WinCommons.writeStatus(Enums.MessageCode.CloseSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text2);
					}
				}
			}
			else
			{
				if (!toolStripMenuItem.Text.Equals("Cancel"))
				{
					return;
				}
				if (gv_Doc.FocusedRowHandle < 0 || gv_Doc.RowCount < 1)
				{
					WinCommons.Error(Enums.MessageCode.DocumentHeaderNotFocus, "");
					return;
				}
				if (GlobalData.ListAccessForm.Where((AuthsImpl f) => f.menucode == "barSub_CancelOtherDoc" && f.authoriza == "F").FirstOrDefault() == null)
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.NoPermissionPerformAction, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				int rowIndex = gv_Doc.FocusedRowHandle;
				int num3 = Function.ParseInt(gv_Doc.GetFocusedRowCellValue("DocEntry"));
				if (num3 > 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCancel, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					DataSet ds = new DataSet();
					ds.Tables.Clear();
					ds.Tables.Add(udoTable);
					DataTable dataTable7 = (DataTable)gridControlEx.DataSource;
					dataTable7 = dataTable7.Select("DocEntry = " + num3).CopyToDataTable();
					ds.Tables.Add(dataTable7);
					string text3 = await OperatorDoc(ds, "CancelDoc");
					ds.Tables.Clear();
					ds.Dispose();
					if (string.IsNullOrEmpty(text3))
					{
						btn_Search_Click(null, null);
						gv_Doc.FocusedRowHandle = rowIndex;
						WinCommons.writeStatus(Enums.MessageCode.CancelSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					}
					else
					{
						WinCommons.Error(Enums.MessageCode.None, text3);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			isNotLoadDetail = false;
		}
	}

	private void gv_Doc_InitNewRow(object sender, InitNewRowEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			if (udoType.Equals("3"))
			{
				gridViewEx.SetRowCellValue(e.RowHandle, gridViewEx.Columns["DocEntry"], 0);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Line_InitNewRow(object sender, InitNewRowEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = sender as GridViewEx;
			gridViewEx.SetRowCellValue(e.RowHandle, gridViewEx.Columns["DocEntry"], 0);
			gridViewEx.SetRowCellValue(e.RowHandle, gridViewEx.Columns["LineId"], 0);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			WinCommons._OpenProcessing();
			int docentry = -1;
			string code = "";
			if (udoType.Equals("3") && !string.IsNullOrEmpty(txt_DocNo.Text) && Function.ParseInt(txt_DocNo.EditValue) > 0)
			{
				docentry = Function.ParseInt(txt_DocNo.EditValue);
			}
			else if (udoType.Equals("1"))
			{
				code = txt_DocNo.Text;
			}
			DateTime fromDate = (string.IsNullOrWhiteSpace(date_FromDate.Text) ? new DateTime(1900, 1, 1) : Convert.ToDateTime(date_FromDate.EditValue));
			DateTime toDate = (string.IsNullOrWhiteSpace(date_ToDate.Text) ? DateTime.Today : Convert.ToDateTime(date_ToDate.EditValue));
			if (split_Main.Panel1.Controls.Count > 0)
			{
				GridControlEx grd_Doc = (GridControlEx)split_Main.Panel1.Controls[0];
				GridViewEx gv_Doc = (GridViewEx)grd_Doc.MainView;
				string tableName = grd_Doc.Name.Substring(4, grd_Doc.Name.Length - 4);
				new DataSet();
				DataSet dsHeader = await GetData(uCode, tableName, docentry, fromDate, toDate, GlobalData.UserName, GlobalData.DefaultBranch, code);
				if (dsHeader.IsHasStructure())
				{
					dsHeader.Tables[1].TableName = tableName;
					await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, dsHeader, lstUDFHeader, expand: false, filterRow: false, tableName, TblFormular);
				}
				dsHeader.Tables.Clear();
				dsHeader.Dispose();
				if (grd_Doc.DatasourceTable().IsNotNull())
				{
					gv_Doc.FocusedRowHandle = 0;
				}
				else
				{
					ClearDetails();
				}
				if (!"Choose".Equals(btn_OK.Text))
				{
					FormMode = Enums.FormViewModeType.View;
				}
				if (_fromDocEntry == 0)
				{
					btn_OK.Text = "Choose";
				}
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void HideMenuItem(ContextMenuStrip ctx, string text)
	{
		ToolStripItem toolStripItem = ctx.Items.Cast<ToolStripItem>().FirstOrDefault((ToolStripItem r) => r.Text == text);
		if (toolStripItem != null)
		{
			toolStripItem.Visible = false;
		}
	}

	private void gv_Line_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewEx gridViewEx = (GridViewEx)sender;
			ContextMenuStrip contextMenuStrip = gridViewEx.GridControl.ContextMenuStrip;
			if (_auth.authoriza != "F")
			{
				foreach (object item in contextMenuStrip.Items)
				{
					if (item is ToolStripMenuItem toolStripMenuItem && !toolStripMenuItem.Name.ToUpper().Equals("CTM_COPYTABLE") && !toolStripMenuItem.Text.ToUpper().Equals("COPY TABLE") && !toolStripMenuItem.Name.ToUpper().Equals("CTM_COPYROWS") && !toolStripMenuItem.Text.ToUpper().Equals("COPY ROW(S)") && !(toolStripMenuItem.Name == "ctm_CopyCell"))
					{
						toolStripMenuItem.Visible = false;
					}
				}
				return;
			}
			if (_auth.authoriza != "F" || dtChild == null || dtChild.Rows.Count == 0)
			{
				HideMenuItem(contextMenuStrip, "Add New Row");
				HideMenuItem(contextMenuStrip, "Insert New Row");
				HideMenuItem(contextMenuStrip, "Duplicate Row");
				HideMenuItem(contextMenuStrip, "Delete Row");
				HideMenuItem(contextMenuStrip, "Paste from Clipboard");
				HideMenuItem(contextMenuStrip, "Import from Excel file");
				return;
			}
			ToolStripItem toolStripItem = contextMenuStrip.Items.Cast<ToolStripItem>().FirstOrDefault((ToolStripItem r) => r.Text == "Duplicate Row");
			if (toolStripItem != null)
			{
				toolStripItem.Visible = gridViewEx.FocusedRowHandle > -1;
			}
			if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Line_CellShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			if ((sender as GridViewEx).FocusedColumn.FieldName.Equals("DocEntry"))
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void lineMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if (split_Main.Panel1.Controls.Count == 0)
			{
				WinCommons.Error(Enums.MessageCode.DocumentHeaderNotFound, "");
				return;
			}
			GridControl gridControl = (GridControl)split_Main.Panel1.Controls[0];
			GridView gridView = (GridView)gridControl.MainView;
			if (gridView.FocusedRowHandle < 0 || gridView.RowCount < 1)
			{
				WinCommons.Error(Enums.MessageCode.DocumentHeaderNotFocus, "");
			}
			else
			{
				if (split_Main.Panel2.Controls.Count == 0)
				{
					return;
				}
				if (sender is ToolStripMenuItem toolStripMenuItem)
				{
					if (toolStripMenuItem.Text.Equals("Add New Row"))
					{
						WinCommons._OpenProcessing();
						XtraTabControl xtraTabControl = (XtraTabControl)split_Main.Panel2.Controls[0];
						XtraTabPage xtraTabPage = xtraTabControl.TabPages[xtraTabControl.SelectedTabPageIndex];
						if (xtraTabPage.Controls.Count == 0)
						{
							WinCommons._CloseProcessing();
							return;
						}
						GridControlEx gridControlEx = (GridControlEx)xtraTabPage.Controls[0];
						GridViewEx gridViewEx = (GridViewEx)gridControlEx.MainView;
						DataTable dataTable = (DataTable)gridControlEx.DataSource;
						DataRow dataRow = dataTable.NewRow();
						if (dataRow.IsExistsColumn("DocEntry"))
						{
							dataRow["DocEntry"] = Convert.ToInt64(0);
						}
						if (dataRow.IsExistsColumn("LineId"))
						{
							dataRow["LineId"] = Convert.ToInt64(0);
						}
						dataTable.Rows.Add(dataRow);
						gridViewEx.FocusedRowHandle = dataTable.Rows.IndexOf(dataRow);
						gridControlEx.RefreshDataSource();
						if (FormMode == Enums.FormViewModeType.View)
						{
							FormMode = Enums.FormViewModeType.Update;
						}
					}
					if (toolStripMenuItem.Text.Equals("Insert New Row"))
					{
						WinCommons._OpenProcessing();
						XtraTabControl xtraTabControl2 = (XtraTabControl)split_Main.Panel2.Controls[0];
						XtraTabPage xtraTabPage2 = xtraTabControl2.TabPages[xtraTabControl2.SelectedTabPageIndex];
						if (xtraTabPage2.Controls.Count == 0)
						{
							WinCommons._CloseProcessing();
							return;
						}
						GridControlEx gridControlEx2 = (GridControlEx)xtraTabPage2.Controls[0];
						GridViewEx gridViewEx2 = (GridViewEx)gridControlEx2.MainView;
						DataTable dataTable2 = (DataTable)gridControlEx2.DataSource;
						DataRow dataRow2 = dataTable2.NewRow();
						if (dataRow2.IsExistsColumn("DocEntry"))
						{
							dataRow2["DocEntry"] = Convert.ToInt64(0);
						}
						if (dataRow2.IsExistsColumn("LineId"))
						{
							dataRow2["LineId"] = Convert.ToInt64(0);
						}
						if (gridViewEx2.FocusedRowHandle > -1)
						{
							dataTable2.Rows.InsertAt(dataRow2, gridViewEx2.FocusedRowHandle + 1);
						}
						else
						{
							dataTable2.Rows.Add(dataRow2);
						}
						gridViewEx2.FocusedRowHandle = dataTable2.Rows.IndexOf(dataRow2);
						gridControlEx2.RefreshDataSource();
						if (FormMode == Enums.FormViewModeType.View)
						{
							FormMode = Enums.FormViewModeType.Update;
						}
					}
					else if (toolStripMenuItem.Text.Equals("Duplicate Row"))
					{
						WinCommons._OpenProcessing();
						XtraTabControl xtraTabControl3 = (XtraTabControl)split_Main.Panel2.Controls[0];
						XtraTabPage xtraTabPage3 = xtraTabControl3.TabPages[xtraTabControl3.SelectedTabPageIndex];
						if (xtraTabPage3.Controls.Count == 0)
						{
							WinCommons._CloseProcessing();
							return;
						}
						GridControlEx gridControlEx3 = (GridControlEx)xtraTabPage3.Controls[0];
						GridViewEx gridViewEx3 = (GridViewEx)gridControlEx3.MainView;
						if (gridViewEx3.FocusedRowHandle > -1)
						{
							DataRow focusedDataRow = gridViewEx3.GetFocusedDataRow();
							DataTable dataTable3 = gridControlEx3.DatasourceTable();
							DataRow dataRow3 = dataTable3.NewRow();
							dataRow3.ItemArray = focusedDataRow.ItemArray;
							if (dataRow3.IsExistsColumn("DocEntry"))
							{
								dataRow3["DocEntry"] = Convert.ToInt64(0);
							}
							if (dataRow3.IsExistsColumn("LineId"))
							{
								dataRow3["LineId"] = Convert.ToInt64(0);
							}
							dataTable3.Rows.Add(dataRow3);
							gridViewEx3.FocusedRowHandle = dataTable3.Rows.IndexOf(dataRow3);
						}
						if (FormMode == Enums.FormViewModeType.View)
						{
							FormMode = Enums.FormViewModeType.Update;
						}
					}
					else if (toolStripMenuItem.Text.Equals("Delete Row"))
					{
						XtraTabControl xtraTabControl4 = (XtraTabControl)split_Main.Panel2.Controls[0];
						XtraTabPage xtraTabPage4 = xtraTabControl4.TabPages[xtraTabControl4.SelectedTabPageIndex];
						if (xtraTabPage4.Controls.Count == 0)
						{
							WinCommons._CloseProcessing();
							return;
						}
						GridControlEx gridControlEx4 = (GridControlEx)xtraTabPage4.Controls[0];
						GridViewEx gridViewEx4 = (GridViewEx)gridControlEx4.MainView;
						if (gridViewEx4.FocusedRowHandle > -1)
						{
							if (Function.ParseInt(gridViewEx4.GetFocusedRowCellValue("DocEntry")) > 0)
							{
								if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
								{
									gridControlEx4.DataSourceTable.Rows.Remove(gridViewEx4.GetFocusedDataRow());
								}
							}
							else
							{
								gridControlEx4.DataSourceTable.Rows.Remove(gridViewEx4.GetFocusedDataRow());
							}
							gridControlEx4.RefreshDataSource();
						}
						if (FormMode == Enums.FormViewModeType.View)
						{
							FormMode = Enums.FormViewModeType.Update;
						}
					}
					else if (toolStripMenuItem.Text.Equals("Paste from Clipboard"))
					{
						DataObject dataObject = (DataObject)Clipboard.GetDataObject();
						if (dataObject.GetDataPresent(DataFormats.Text))
						{
							XtraTabControl xtraTabControl5 = (XtraTabControl)split_Main.Panel2.Controls[0];
							XtraTabPage xtraTabPage5 = xtraTabControl5.TabPages[xtraTabControl5.SelectedTabPageIndex];
							if (xtraTabPage5.Controls.Count == 0)
							{
								WinCommons._CloseProcessing();
								return;
							}
							GridControlEx gridControlEx5 = (GridControlEx)xtraTabPage5.Controls[0];
							GridViewEx view = (GridViewEx)gridControlEx5.MainView;
							gridControl.Enabled = false;
							gridControlEx5.Enabled = false;
							btn_OK.Enabled = false;
							btn_Search.Enabled = false;
							PasteFromExcel(view, dataObject);
							gridControl.Enabled = true;
							gridControlEx5.Enabled = true;
							btn_OK.Enabled = true;
							btn_Search.Enabled = true;
							if (FormMode == Enums.FormViewModeType.View)
							{
								FormMode = Enums.FormViewModeType.Update;
							}
						}
					}
					else if (toolStripMenuItem.Text.Equals("Import from Excel file"))
					{
						DataTable dataTable4 = new DataTable();
						OpenFileDialog openFileDialog = new OpenFileDialog();
						openFileDialog.Filter = $"Excel Files|*.xls;*.xlsx;*.xlsm";
						openFileDialog.FilterIndex = 1;
						openFileDialog.Multiselect = false;
						if (openFileDialog.ShowDialog() == DialogResult.OK)
						{
							string fileName = openFileDialog.FileName;
							DataSet dataSet = WinFuntions.ReadExcelToDataTable(fileName, Path.GetExtension(fileName).ToLower());
							if (dataSet.IsNotNull())
							{
								dataTable4 = dataSet.Tables[0];
							}
						}
						if (dataTable4.IsNotNull())
						{
							WinCommons._OpenProcessing();
							XtraTabControl xtraTabControl6 = (XtraTabControl)split_Main.Panel2.Controls[0];
							XtraTabPage xtraTabPage6 = xtraTabControl6.TabPages[xtraTabControl6.SelectedTabPageIndex];
							if (xtraTabPage6.Controls.Count == 0)
							{
								WinCommons._CloseProcessing();
								return;
							}
							GridControlEx gridControlEx6 = (GridControlEx)xtraTabPage6.Controls[0];
							_ = (GridViewEx)gridControlEx6.MainView;
							DataTable dataTable5 = (DataTable)gridControlEx6.DataSource;
							gridControl.Enabled = false;
							btn_OK.Enabled = false;
							btn_Search.Enabled = false;
							dataTable5.Rows.Clear();
							for (int i = 0; i < dataTable4.Rows.Count; i++)
							{
								DataRow dataRow4 = dataTable4.Rows[i];
								DataRow dataRow5 = dataTable5.NewRow();
								for (int j = 0; j < dataTable4.Columns.Count && j < dataTable5.Columns.Count; j++)
								{
									dataRow5[j] = dataRow4[j];
								}
								dataTable5.Rows.Add(dataRow5);
							}
							gridControlEx6.DataSource = dataTable5;
							gridControlEx6.RefreshDataSource();
							gridControl.Enabled = true;
							gridControlEx6.Enabled = true;
							btn_OK.Enabled = true;
							btn_Search.Enabled = true;
							if (FormMode == Enums.FormViewModeType.View)
							{
								FormMode = Enums.FormViewModeType.Update;
							}
						}
					}
				}
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			grd_Doc.Enabled = true;
			btn_OK.Enabled = true;
			btn_Search.Enabled = true;
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void date_FromDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			txt_DocNo.Text = "";
			if (date_FromDate.EditValue != null && !string.IsNullOrEmpty(date_FromDate.Text))
			{
				date_ToDate.Properties.MinValue = (DateTime)date_FromDate.EditValue;
			}
			else if (date_FromDate.EditValue == null || string.IsNullOrEmpty(date_FromDate.Text))
			{
				date_ToDate.Properties.MinValue = default;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void date_ToDate_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			txt_DocNo.Text = "";
			if (date_ToDate.EditValue != null && !string.IsNullOrEmpty(date_ToDate.Text))
			{
				date_FromDate.Properties.MaxValue = (DateTime)date_ToDate.EditValue;
			}
			else if (date_ToDate.EditValue == null || string.IsNullOrEmpty(date_ToDate.Text))
			{
				date_FromDate.Properties.MaxValue = DateTime.Today.AddMonths(1);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task<string> OperatorDoc(DataSet ds, string command)
	{
		try
		{
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "UDO");
			return await _help.PostAsJsonAsync(command, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private void HideShowSearchControl(bool isVisible)
	{
		date_FromDate.Visible = isVisible;
		date_ToDate.Visible = isVisible;
		lb_FromDate.Visible = isVisible;
		lb_ToDate.Visible = isVisible;
		lb_DocNo.Visible = isVisible;
		txt_DocNo.Visible = isVisible;
		btn_Search.Visible = isVisible;
		split_Main.Dock = ((!isVisible) ? DockStyle.Fill : DockStyle.None);
	}

	private ContextMenuStrip CreateContextMenuLine()
	{
		return new ContextMenuStrip
		{
			Items = 
			{
				{
					"Add New Row",
					(Image)null,
					(EventHandler)lineMenu_Click
				},
				{
					"Insert New Row",
					(Image)null,
					(EventHandler)lineMenu_Click
				},
				{
					"Duplicate Row",
					(Image)null,
					(EventHandler)lineMenu_Click
				},
				{
					"Delete Row",
					(Image)null,
					(EventHandler)lineMenu_Click
				},
				{
					"Paste from Clipboard",
					(Image)null,
					(EventHandler)lineMenu_Click
				},
				{
					"Import from Excel file",
					(Image)null,
					(EventHandler)lineMenu_Click
				}
			}
		};
	}

	private async Task LoadGridViewLines(GridViewEx gvDoc, string udoCode, int docEntry, string codeSearch, DateTime fromDate, DateTime toDate)
	{
		_ = 4;
		try
		{
			XtraTabLine.Dock = DockStyle.Fill;
			dtChild = await _httpClient.Udo.GetUdo1ByCode(udoCode);
			if (dtChild == null || dtChild.Rows.Count == 0)
			{
				split_Main.PanelVisibility = SplitPanelVisibility.Panel1;
			}
			List<Task> taskList = new List<Task>();
			foreach (DataRow row in dtChild.Rows)
			{
				XtraTabPage xtraTabPage = new XtraTabPage();
				string tableName = "@" + row["TableName"];
				GridControlEx grd_Line = new GridControlEx();
				GridViewEx gv_Line = new GridViewEx();
				grd_Line.Name = "grd_" + tableName;
				gv_Line.NotLoadFormsettingAtLoad = true;
				xtraTabPage.Controls.Add(grd_Line);
				grd_Line.EditorKeyDown += grd_Line_EditorKeyDown;
				grd_Line.Dock = DockStyle.Fill;
				grd_Line.ContextMenuStrip = CreateContextMenuLine();
				gv_Line.Name = "gv_" + tableName;
				gv_Line.OptionsSelection.MultiSelect = true;
				gv_Line.Tag = tableName;
				gv_Line.FormId = udoCode;
				gv_Line.ItemId = tableName;
				gv_Line.OptionsView.NewItemRowPosition = NewItemRowPosition.None;
				gv_Line.InitNewRow += gv_Line_InitNewRow;
				gv_Line.PopupMenuShowing += gv_Line_PopupMenuShowing;
				gv_Line.CellValueChanged += gv_Line_CellValueChanged;
				gv_Line.ShowingEditor += gv_Line_CellShowingEditor;
				xtraTabPage.Name = tableName;
				xtraTabPage.Text = Function.ToString(row["Descr"]);
				XtraTabLine.TabPages.Add(xtraTabPage);
				if (_auth.authoriza != "F")
				{
					gv_Line.DisableFormSetting = true;
					gv_Line.ReadOnly = true;
				}
				lstUDF = await _httpClient.Udf.GetListField(tableName);
				TblFormular[tableName] = gv_Line;
				DataSet dsDetail = await GetData(udoCode, tableName, docEntry, fromDate, toDate, GlobalData.UserName, GlobalData.DefaultBranch, codeSearch);
				if (dsDetail.IsHasStructure())
				{
					dsDetail.Tables[1].TableName = tableName;
					Task task = GridViewHelper.GenerateGrid(grd_Line, gv_Line, dsDetail, lstUDF, expand: false, filterRow: false, tableName, TblFormular);
					taskList.Add(task);
					if (OpenFormLines.IsNotNull() && Mapping.Length != 0)
					{
						ContextActionMappingImpl[] mapField = Mapping.Where((ContextActionMappingImpl t) => t.TargetItem == Function.ToString(row["SonName"])).ToArray();
						DataTable dataTable = OpenFormLines.DefaultView.ToTable(distinct: false, (from t in mapField
							where !string.IsNullOrEmpty(t.SourceField)
							select t.SourceField).Distinct().ToArray());
						ContextActionMappingImpl[] array = mapField.Where((ContextActionMappingImpl t) => t.SumType == "S").ToArray();
						DataTable dtLineSource = dataTable.Clone();
						if (array.Length != 0)
						{
							ContextActionMappingImpl[] groupcolLine = mapField.Where((ContextActionMappingImpl t) => Function.ToString(t.SumType) == "").ToArray();
							foreach (IGrouping<NTuple<object>, DataRow> item2 in from r in OpenFormLines.AsEnumerable()
								group r by new NTuple<object>(from mapItem in groupcolLine
									where !string.IsNullOrEmpty(mapItem.SourceField)
									select new
									{
										Name = mapItem.SourceField,
										Value = r[mapItem.SourceField]
									}))
							{
								DataRow dataRow = dtLineSource.NewRow();
								List<Common.ParameterValue> list = new List<Common.ParameterValue>();
								object[] values = item2.Key.Values;
								for (int num = 0; num < values.Length; num++)
								{
									if (values[num].ToType(typeof(Common.ParameterValue)) is Common.ParameterValue item)
									{
										list.Add(item);
									}
								}
								ContextActionMappingImpl[] array2 = groupcolLine;
								foreach (ContextActionMappingImpl col in array2)
								{
									if (!string.IsNullOrEmpty(col.SourceField))
									{
										Common.ParameterValue parameterValue = list.FirstOrDefault((Common.ParameterValue t) => t.Name == col.SourceField);
										if (parameterValue != null)
										{
											dataRow[col.SourceField] = parameterValue.Value;
										}
									}
								}
								array2 = array;
								foreach (ContextActionMappingImpl col2 in array2)
								{
									dataRow[col2.SourceField] = item2.Sum((DataRow t) => Function.ParseDouble(t[col2.SourceField]));
								}
								dtLineSource.Rows.Add(dataRow);
							}
						}
						else
						{
							dtLineSource = dataTable;
						}
						await task;
						DataTable dataSourceTable = grd_Line.DataSourceTable;
						for (int num2 = 0; num2 < dtLineSource.Rows.Count; num2++)
						{
							DataRow dataRow2 = dataSourceTable.NewRow();
							if (dataRow2.IsExistsColumn("DocEntry"))
							{
								dataRow2["DocEntry"] = Convert.ToInt64(0);
							}
							if (dataRow2.IsExistsColumn("LineId"))
							{
								dataRow2["LineId"] = Convert.ToInt64(0);
							}
							dataSourceTable.Rows.Add(dataRow2);
							dataSourceTable.AcceptChanges();
							gv_Line.FocusedRowHandle = num2;
							ContextActionMappingSource.MappingValue(dtLineSource, mapField, num2, gv_Line);
						}
					}
				}
				dsDetail.Tables.Clear();
				dsDetail.Dispose();
				gv_Line.InitUdoLineCellValueChanged(gvDoc, Formulars, TblFormular);
			}
			await Task.WhenAll(taskList);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task<DataSet> GetData(string udoCode, string tableName, int docentry, DateTime fromDate, DateTime toDate, string userName, int branch, string code)
	{
		try
		{
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dataRow["udoCode"] = udoCode;
			dataRow["tableName"] = tableName;
			dataRow["docentry"] = docentry;
			dataRow["fromDate"] = fromDate;
			dataRow["toDate"] = toDate;
			dataRow["userName"] = userName;
			dataRow["branch"] = branch;
			dataRow["code"] = code;
			dtSearch.Rows.Add(dataRow);
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "UDO");
			return await helper.PostAsync("GetData", dtSearch.Copy());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private void ClearDetails()
	{
		if (split_Main.Panel2.Controls.Count <= 0)
		{
			return;
		}
		XtraTabControl xtraTabControl = (XtraTabControl)split_Main.Panel2.Controls[0];
		if (xtraTabControl.TabPages.Count <= 0)
		{
			return;
		}
		for (int i = 0; i < xtraTabControl.TabPages.Count; i++)
		{
			if (xtraTabControl.TabPages[i].Controls.Count > 0)
			{
				GridControlEx gridControlEx = (GridControlEx)xtraTabControl.TabPages[i].Controls[0];
				gridControlEx.DataSourceTable().Rows.Clear();
				gridControlEx.RefreshDataSource();
			}
		}
	}

	private void CreateDataSearch()
	{
		if (!dtSearch.IsNotNull() || dtSearch.Columns.Count == 0)
		{
			dtSearch = new DataTable();
			dtSearch.Columns.Add(new DataColumn("udoCode", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("tableName", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("docentry", typeof(int)));
			dtSearch.Columns.Add(new DataColumn("fromDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("toDate", typeof(DateTime)));
			dtSearch.Columns.Add(new DataColumn("userName", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("branch", typeof(int)));
			dtSearch.Columns.Add(new DataColumn("code", typeof(string)));
			dtSearch.Columns.Add(new DataColumn("type", typeof(int)));
		}
		else
		{
			dtSearch.Rows.Clear();
		}
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			if (split_Main.Panel1.Controls.Count > 0)
			{
				int num = Function.ParseInt(((GridViewEx)((GridControlEx)split_Main.Panel1.Controls[0]).MainView).GetFocusedRowCellValue("DocEntry"));
				await WinFuntions.PrintLayout("", num, uCode, uCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PREVIEWLAYOUTS()
	{
		try
		{
			if (split_Main.Panel1.Controls.Count > 0)
			{
				int num = Function.ParseInt(((GridViewEx)((GridControlEx)split_Main.Panel1.Controls[0]).MainView).GetFocusedRowCellValue("DocEntry"));
				await WinFuntions.PreviewLayout("", num, uCode, this, uCode);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PRINT()
	{
		try
		{
			int docEntry = Function.ParseInt(((GridViewEx)((GridControlEx)split_Main.Panel1.Controls[0]).MainView).GetFocusedRowCellValue("DocEntry"));
			await WinFuntions.Print("", docEntry, uCode, uCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void PasteFromExcel(GridViewEx view, DataObject obj)
	{
		WinCommons._OpenProcessing();
		List<string> list = new List<string>(obj.GetText().Split('\n'));
		if (list.Count > 0 && string.IsNullOrWhiteSpace(list[list.Count - 1]))
		{
			list.RemoveAt(list.Count - 1);
		}
		if (list.Count < 2)
		{
			return;
		}
		int rowHandle = view.FocusedRowHandle;
		GridColumn[] visibleCol = (from r in view.Columns
			where r.VisibleIndex >= 0
			orderby r.VisibleIndex
			select r).ToArray();
		foreach (string item in list)
		{
			string text = item;
			if (text.EndsWith("\r"))
			{
				text = text.Substring(0, text.Length - "\r".Length);
			}
			string[] rowData = text.Split('\r', '\t');
			view.GridControl.Invoke((MethodInvoker)(() =>
			{
				DataTable dataTable = (DataTable)view.GridControl.DataSource;
				if (!string.IsNullOrEmpty(rowData[0]))
				{
					if (view.IsNewItemRow(rowHandle) || rowHandle > view.RowCount - 1)
					{
						DataRow dataRow = dataTable.NewRow();
						if (dataRow.IsExistsColumn("DocEntry"))
						{
							dataRow["DocEntry"] = 0;
						}
						if (dataRow.IsExistsColumn("LineId"))
						{
							dataRow["LineId"] = 0;
						}
						dataTable.Rows.Add(dataRow);
						dataTable.AcceptChanges();
						rowHandle = view.GetRowHandle(dataTable.Rows.IndexOf(dataRow));
						view.FocusedRowHandle = rowHandle;
					}
					int visibleIndex = view.FocusedColumn.VisibleIndex;
					int num2 = ((rowData.Length <= view.VisibleColumns.Count) ? rowData.Length : view.VisibleColumns.Count);
					int num3 = 0;
					int num4 = visibleIndex;
					while (num3 < num2 && num4 < visibleCol.Length)
					{
						string fieldName = visibleCol[num4].FieldName;
						if (!view.Columns[fieldName].OptionsColumn.AllowEdit || view.Columns[fieldName].OptionsColumn.ReadOnly)
						{
							num4++;
							num3++;
						}
						else if (num4 < view.Columns.Count)
						{
							object value = (string.IsNullOrEmpty(rowData[num3]) ? null : rowData[num3]);
							view.SetRowCellValue(rowHandle, fieldName, value);
						}
						num3++;
						num4++;
					}
				}
			}));
			int num = rowHandle;
			rowHandle = num + 1;
		}
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			await Navigate(3);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			await Navigate(1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_PREVIOUS()
	{
		try
		{
			await Navigate(2);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			await Navigate(4);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			btn_Search_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_DocNo_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(txt_DocNo.Text) && FormMode == Enums.FormViewModeType.View)
			{
				btn_Search_Click(null, null);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Navigate(int type)
	{
		WinCommons._OpenProcessing();
		try
		{
			string tableName = "@" + Function.ToString(udoTable.Rows[0]["TableName"]);
			int docentry = -1;
			string code = "";
			if (udoType.Equals("3") && !string.IsNullOrEmpty(txt_DocNo.Text) && Function.ParseInt(txt_DocNo.EditValue) > 0)
			{
				docentry = Function.ParseInt(txt_DocNo.EditValue);
			}
			else if (udoType.Equals("1"))
			{
				code = txt_DocNo.Text;
			}
			HttpResult httpResult = await GetPage(uCode, tableName, docentry, code, type, GlobalData.UserName, GlobalData.DefaultBranch);
			if (string.IsNullOrEmpty(httpResult.Message))
			{
				txt_DocNo.Text = Function.ToString(httpResult.Content);
			}
			btn_Search_Click(null, null);
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<HttpResult> GetPage(string udoCode, string tableName, int docentry, string code, int type, string userName, int branch)
	{
		try
		{
			CreateDataSearch();
			DataRow dataRow = dtSearch.NewRow();
			dataRow["udoCode"] = udoCode;
			dataRow["tableName"] = tableName;
			dataRow["docentry"] = docentry;
			dataRow["type"] = type;
			dataRow["userName"] = userName;
			dataRow["branch"] = branch;
			dataRow["code"] = code;
			dtSearch.Rows.Add(dataRow);
			using HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "UDO");
			return await helper.PostAsync("GetPage", dtSearch.Copy());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new HttpResult();
	}

	public override void CHANGELOG()
	{
		try
		{
			string primaryKey = Function.ParseInt(((GridViewEx)((GridControlEx)split_Main.Panel1.Controls[0]).MainView).GetFocusedRowCellValue("DocEntry")).ToString();
			Enums.ChangeLog changeLog = Enums.ChangeLog.clUDO;
			GetChangeLogParams getChangeLogParams = new GetChangeLogParams();
			getChangeLogParams.PrimaryKey = primaryKey;
			getChangeLogParams.Object = changeLog;
			getChangeLogParams.UDOObjectCode = uCode;
			frm_ChangeLogList frm_ChangeLogList2 = new frm_ChangeLogList();
			frm_ChangeLogList2.GetChangeLogParams = getChangeLogParams;
			frm_ChangeLogList2.MdiParent = MdiParent;
			frm_ChangeLogList2.Show();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.UDO.frm_UDO));
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btn_Choose = new DevExpress.XtraEditors.SimpleButton();
		this.split_Main = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_ToDate = new Apzon.Controls.Controls.LabelLine();
		this.lb_FromDate = new Apzon.Controls.Controls.LabelLine();
		this.txt_DocNo = new Apzon.Controls.Editor.TextEditEx();
		this.lb_DocNo = new Apzon.Controls.Controls.LabelLine();
		this.date_FromDate = new Apzon.Controls.Editor.DateEditEx();
		this.date_ToDate = new Apzon.Controls.Editor.DateEditEx();
		this.ctx_Doc = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.duplicateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.removeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.cancelToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.split_Main).BeginInit();
		this.split_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).BeginInit();
		this.ctx_Doc.SuspendLayout();
		base.SuspendLayout();
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_OK.Appearance.Options.UseBackColor = true;
		this.btn_OK.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "Save";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(12, 6);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(75, 25);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "Save";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Cancel.Appearance.Options.UseBackColor = true;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(90, 6);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 25);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.panelControl1.Controls.Add(this.btn_Choose);
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Controls.Add(this.btn_OK);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Bottom;
		this.panelControl1.Location = new System.Drawing.Point(0, 410);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(741, 36);
		this.panelControl1.TabIndex = 3;
		this.btn_Choose.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Choose.Appearance.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.btn_Choose.Appearance.Options.UseBackColor = true;
		this.btn_Choose.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Choose.ImageOptions.Image");
		this.btn_Choose.Location = new System.Drawing.Point(171, 6);
		this.btn_Choose.Name = "btn_Choose";
		this.btn_Choose.Size = new System.Drawing.Size(75, 25);
		this.btn_Choose.TabIndex = 3;
		this.btn_Choose.Text = "Choose";
		this.btn_Choose.Visible = false;
		this.split_Main.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.split_Main.Horizontal = false;
		this.split_Main.Location = new System.Drawing.Point(0, 39);
		this.split_Main.Name = "split_Main";
		this.split_Main.Panel1.Controls.Add(this.grd_Doc);
		this.split_Main.Panel1.Text = "pn_Doc";
		this.split_Main.Panel2.Text = "pn_Line";
		this.split_Main.Size = new System.Drawing.Size(741, 371);
		this.split_Main.SplitterPosition = 196;
		this.split_Main.TabIndex = 4;
		this.split_Main.Text = "splitContainerControl1";
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(741, 196);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gv_Doc.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = null;
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 35;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = null;
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gv_Doc.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gv_Doc.OptionsClipboard.ClipboardMode = DevExpress.Export.ClipboardMode.Formatted;
		this.gv_Doc.OptionsPrint.AutoWidth = false;
		this.gv_Doc.OptionsPrint.UsePrintStyles = false;
		this.gv_Doc.OptionsView.BestFitMaxRowCount = 100;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gv_Doc.OptionsView.ShowGroupedColumns = true;
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(609, 7);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(75, 23);
		this.btn_Search.TabIndex = 9;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.lb_ToDate.EditControl = null;
		this.lb_ToDate.EditValue = "To Date";
		this.lb_ToDate.FieldName = null;
		this.lb_ToDate.HideWhenEmpty = false;
		this.lb_ToDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_ToDate.Location = new System.Drawing.Point(437, 7);
		this.lb_ToDate.Name = "lb_ToDate";
		this.lb_ToDate.ReadOnly = false;
		this.lb_ToDate.SettingInVisible = false;
		this.lb_ToDate.SettingReadonly = false;
		this.lb_ToDate.ShowUnderLine = true;
		this.lb_ToDate.Size = new System.Drawing.Size(62, 20);
		this.lb_ToDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_ToDate.TabIndex = 8;
		this.lb_ToDate.Utext = "To Date";
		this.lb_FromDate.EditControl = null;
		this.lb_FromDate.EditValue = "From Date";
		this.lb_FromDate.FieldName = null;
		this.lb_FromDate.HideWhenEmpty = false;
		this.lb_FromDate.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_FromDate.Location = new System.Drawing.Point(261, 7);
		this.lb_FromDate.Name = "lb_FromDate";
		this.lb_FromDate.ReadOnly = false;
		this.lb_FromDate.SettingInVisible = false;
		this.lb_FromDate.SettingReadonly = false;
		this.lb_FromDate.ShowUnderLine = true;
		this.lb_FromDate.Size = new System.Drawing.Size(67, 20);
		this.lb_FromDate.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_FromDate.TabIndex = 6;
		this.lb_FromDate.Utext = "From Date";
		this.txt_DocNo.EditControl = null;
		this.txt_DocNo.FieldName = "txtDocNo";
		this.txt_DocNo.IsSearchControl = false;
		this.txt_DocNo.Location = new System.Drawing.Point(57, 7);
		this.txt_DocNo.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
		this.txt_DocNo.Name = "txt_DocNo";
		this.txt_DocNo.Properties.Mask.EditMask = "n0";
		this.txt_DocNo.Properties.Mask.MaskType = DevExpress.XtraEditors.Mask.MaskType.Numeric;
		this.txt_DocNo.SettingInVisible = false;
		this.txt_DocNo.SettingReadonly = false;
		this.txt_DocNo.Size = new System.Drawing.Size(177, 20);
		this.txt_DocNo.TabIndex = 11;
		this.txt_DocNo.ToolTip = "Doc Num";
		this.txt_DocNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DocNo_KeyDown);
		this.lb_DocNo.EditControl = null;
		this.lb_DocNo.EditValue = "Doc No";
		this.lb_DocNo.FieldName = null;
		this.lb_DocNo.HideWhenEmpty = false;
		this.lb_DocNo.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_DocNo.Location = new System.Drawing.Point(13, 7);
		this.lb_DocNo.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.lb_DocNo.Name = "lb_DocNo";
		this.lb_DocNo.ReadOnly = false;
		this.lb_DocNo.SettingInVisible = false;
		this.lb_DocNo.SettingReadonly = false;
		this.lb_DocNo.ShowUnderLine = true;
		this.lb_DocNo.Size = new System.Drawing.Size(47, 21);
		this.lb_DocNo.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_DocNo.TabIndex = 10;
		this.lb_DocNo.Utext = "Doc No";
		this.date_FromDate.EditControl = null;
		this.date_FromDate.EditValue = null;
		this.date_FromDate.FieldName = null;
		this.date_FromDate.Location = new System.Drawing.Point(331, 7);
		this.date_FromDate.Name = "date_FromDate";
		this.date_FromDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_FromDate.Properties.IsSearchControl = false;
		this.date_FromDate.SettingInVisible = false;
		this.date_FromDate.SettingReadonly = false;
		this.date_FromDate.Size = new System.Drawing.Size(100, 20);
		this.date_FromDate.TabIndex = 12;
		this.date_FromDate.EditValueChanged += new System.EventHandler(this.date_FromDate_EditValueChanged);
		this.date_ToDate.EditControl = null;
		this.date_ToDate.EditValue = null;
		this.date_ToDate.FieldName = null;
		this.date_ToDate.Location = new System.Drawing.Point(502, 7);
		this.date_ToDate.Name = "date_ToDate";
		this.date_ToDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_ToDate.Properties.IsSearchControl = false;
		this.date_ToDate.SettingInVisible = false;
		this.date_ToDate.SettingReadonly = false;
		this.date_ToDate.Size = new System.Drawing.Size(101, 20);
		this.date_ToDate.TabIndex = 13;
		this.date_ToDate.EditValueChanged += new System.EventHandler(this.date_ToDate_EditValueChanged);
		this.ctx_Doc.Items.AddRange(new System.Windows.Forms.ToolStripItem[5] { this.addNewToolStripMenuItem, this.duplicateToolStripMenuItem, this.removeToolStripMenuItem, this.closeToolStripMenuItem, this.cancelToolStripMenuItem });
		this.ctx_Doc.Name = "ctx_Doc";
		this.ctx_Doc.Size = new System.Drawing.Size(125, 114);
		this.addNewToolStripMenuItem.Name = "addNewToolStripMenuItem";
		this.addNewToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.addNewToolStripMenuItem.Text = "Add New";
		this.addNewToolStripMenuItem.Click += new System.EventHandler(this.docMenu_Click);
		this.duplicateToolStripMenuItem.Name = "duplicateToolStripMenuItem";
		this.duplicateToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.duplicateToolStripMenuItem.Text = "Duplicate";
		this.duplicateToolStripMenuItem.Click += new System.EventHandler(this.docMenu_Click);
		this.removeToolStripMenuItem.Name = "removeToolStripMenuItem";
		this.removeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.removeToolStripMenuItem.Text = "Remove";
		this.removeToolStripMenuItem.Click += new System.EventHandler(this.docMenu_Click);
		this.closeToolStripMenuItem.Name = "closeToolStripMenuItem";
		this.closeToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.closeToolStripMenuItem.Text = "Close";
		this.closeToolStripMenuItem.Click += new System.EventHandler(this.docMenu_Click);
		this.cancelToolStripMenuItem.Name = "cancelToolStripMenuItem";
		this.cancelToolStripMenuItem.Size = new System.Drawing.Size(124, 22);
		this.cancelToolStripMenuItem.Text = "Cancel";
		this.cancelToolStripMenuItem.Click += new System.EventHandler(this.docMenu_Click);
		base.AcceptButton = this.btn_Search;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.CancelButton = this.btn_Cancel;
		base.ClientSize = new System.Drawing.Size(741, 446);
		base.Controls.Add(this.date_ToDate);
		base.Controls.Add(this.date_FromDate);
		base.Controls.Add(this.txt_DocNo);
		base.Controls.Add(this.lb_DocNo);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.lb_ToDate);
		base.Controls.Add(this.lb_FromDate);
		base.Controls.Add(this.split_Main);
		base.Controls.Add(this.panelControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_UDO";
		base.ShowInTaskbar = false;
		this.Text = "User Defined Object Form";
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.split_Main).EndInit();
		this.split_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNo.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_FromDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_ToDate.Properties).EndInit();
		this.ctx_Doc.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
