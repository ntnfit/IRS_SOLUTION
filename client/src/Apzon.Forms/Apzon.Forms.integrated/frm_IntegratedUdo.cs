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
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTab;

namespace Apzon.Forms.Integrated;

public class frm_IntegratedUdo : XtraFormEx
{
	private const string FormController = "IntegratedUdo";

	private List<GridColumn> _lstEditableLineColumns = new List<GridColumn>();

	private List<Tuple<string, DataSet>> _lstUdoGridStructDoc = new List<Tuple<string, DataSet>>();

	private List<Tuple<string, DataSet>> _lstUdoGridStructLine = new List<Tuple<string, DataSet>>();

	private DataTable _dtOudo = new DataTable();

	private string _udoType = "";

	private string _udoCode = "";

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private Label lbl_Progress;

	private System.Windows.Forms.ProgressBar progressBar1;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButtonEx btn_Search;

	private RadioGroupEx radioGroupStatus;

	private DateEditEx date_To;

	private LabelLine labelLine2;

	private DateEditEx date_From;

	private LabelLine labelLine1;

	private LookUpEditCombo luk_Udos;

	private LabelLine labelLine3;

	private XtraTabControlEx tabCtl_Line;

	private Label lbl_UDOName;

	private LabelLine labelLine4;

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
			btn_OK.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btn_OK.Text = Resources.ButtonFind;
			break;
		default:
			btn_OK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_IntegratedUdo()
	{
		InitializeComponent();
		FormMode = Enums.FormViewModeType.Add;
	}

	private async void frm_IntegratedUdo_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			lbl_UDOName.Text = "";
			await LoadUdoCombo();
			lbl_Progress.Text = "";
			lbl_Progress.Visible = false;
			progressBar1.Visible = false;
			date_From.EditValue = DateTime.Today.AddDays(-30.0);
			date_To.EditValue = DateTime.Today;
			radioGroupStatus.SelectedIndex = 0;
			await Search();
			WindowState = FormWindowState.Maximized;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Doc_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
		_ = 1;
		try
		{
			string id = ((_udoType == "1") ? Function.ToString(gv_Doc.GetFocusedRowCellValue("Code")) : Function.ParseInt(gv_Doc.GetFocusedRowCellValue("U_DocEntry")).ToString());
			DataSet dsRet = await SearchLine(id);
			tabCtl_Line.TabPages.Clear();
			if (dsRet == null || dsRet.Tables.Count <= 0)
			{
				return;
			}
			DataTable dtHeader = dsRet.Tables[0];
			for (int i = 1; i < dsRet.Tables.Count; i++)
			{
				string tableName = dsRet.Tables[i].TableName;
				XtraTabPage xtraTabPage = new XtraTabPage
				{
					Name = "xtp_" + tableName,
					Text = Function.ToString(dtHeader.Rows[i - 1][0])
				};
				GridControlEx grd = new GridControlEx
				{
					Name = "grdLine_" + tableName
				};
				GridViewEx gv = new GridViewEx
				{
					Name = "gvLine_" + tableName
				};
				tabCtl_Line.TabPages.Add(xtraTabPage);
				xtraTabPage.Controls.Add(grd);
				grd.Dock = DockStyle.Fill;
				grd.ViewCollection.Add(gv);
				grd.MainView = gv;
				gv.GridControl = grd;
				gv.RowCellStyle += gv_Line_RowCellStyle;
				await LoadGridLineStructure(gv);
				grd.DataSource = dsRet.Tables[i];
				grd.RefreshDataSource();
				for (int j = 0; j < gv.Columns.Count; j++)
				{
					gv.Columns[j].OptionsColumn.ReadOnly = true;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void splitContainerControl1_Resize(object sender, EventArgs e)
	{
		splitContainerControl1.SplitterPosition = splitContainerControl1.Height / 2;
	}

	private void gv_Doc_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (e.RowHandle == gv_Doc.FocusedRowHandle)
			{
				e.Appearance.BackColor = StyleColor.FocusedRow;
			}
			else if (FormMode == Enums.FormViewModeType.Add)
			{
				if (e.Column.FieldName == "SaveStatus")
				{
					string text = Function.ToString(gv_Doc.GetRowCellValue(e.RowHandle, e.Column));
					if (!string.IsNullOrEmpty(text))
					{
						if (text.ToLower() == "success")
						{
							e.Appearance.ForeColor = Color.Green;
						}
						else
						{
							e.Appearance.ForeColor = Color.Red;
						}
					}
				}
				else if (!e.Column.OptionsColumn.AllowEdit || e.Column.OptionsColumn.ReadOnly)
				{
					e.Appearance.BackColor = StyleColor.GridDisable1;
				}
			}
			else if (!e.Column.OptionsColumn.AllowEdit || e.Column.OptionsColumn.ReadOnly)
			{
				e.Appearance.BackColor = StyleColor.GridDisable1;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Line_RowCellStyle(object sender, RowCellStyleEventArgs e)
	{
		try
		{
			if (sender is GridView gridView)
			{
				if (e.RowHandle == gridView.FocusedRowHandle)
				{
					e.Appearance.BackColor = StyleColor.FocusedRow;
				}
				else if (!e.Column.OptionsColumn.AllowEdit || e.Column.OptionsColumn.ReadOnly || FormMode != Enums.FormViewModeType.Add)
				{
					e.Appearance.BackColor = StyleColor.GridDisable1;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gv_Doc_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			if (e.Action == CollectionChangeAction.Add && (Function.ToString(gv_Doc.GetRowCellValue(e.ControllerRow, "SaveStatus")).ToLower() == "success" || FormMode == Enums.FormViewModeType.Find))
			{
				gv_Doc.UnselectRow(e.ControllerRow);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		EnableDisableButton();
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || _udoType == "1")
			{
				await Save();
			}
			else if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.ItemAlreadyImported, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableButton(enable: true);
		WinCommons._CloseProcessing();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		await Search();
	}

	private void EnableDisableButton(bool enable = false)
	{
		try
		{
			radioGroupStatus.Enabled = enable;
			btn_Search.Enabled = enable;
			btn_OK.Enabled = enable;
			btn_Cancel.Enabled = enable;
			date_From.Enabled = enable;
			date_To.Enabled = enable;
			FormMode = ((gv_Doc.DataRowCount > 0) ? Enums.FormViewModeType.Add : Enums.FormViewModeType.View);
			if (enable && FormMode == Enums.FormViewModeType.Find)
			{
				btn_OK.Text = ((gv_Doc.DataRowCount > 0) ? "Save" : "OK");
			}
			if (enable && FormMode == Enums.FormViewModeType.Find)
			{
				btn_OK.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Save()
	{
		_ = 1;
		try
		{
			EnableDisableButton();
			int[] selectedRows = gv_Doc.GetSelectedRows();
			int success = 0;
			if (selectedRows.Length != 0)
			{
				IEnumerable<int> source = selectedRows.Where((int t) => Function.ToString(gv_Doc.GetRowCellValue(t, "SaveStatus")).ToLower() != "success");
				if (!source.Any())
				{
					return;
				}
				lbl_Progress.Text = "";
				lbl_Progress.Visible = true;
				progressBar1.Visible = true;
				DataTable dtsDoc = grd_Doc.DataSource as DataTable;
				int[] arrSel = source.ToArray();
				int totalLine = arrSel.Length;
				progressBar1.Maximum = totalLine + 1;
				progressBar1.Minimum = 1;
				progressBar1.Step = 1;
				progressBar1.Value = 1;
				for (int i = 0; i < arrSel.Length; i++)
				{
					DataSet ds = new DataSet();
					if (_dtOudo == null || _dtOudo.Rows.Count != 1)
					{
						WinCommons.Error(Enums.MessageCode.NoDataToImport, "");
						break;
					}
					ds.Tables.Add(_dtOudo.Copy());
					DataRow docRow = dtsDoc.Rows[gv_Doc.GetDataSourceRowIndex(arrSel[i])];
					DataTable dataTable = dtsDoc.Clone();
					dataTable.ImportRow(docRow);
					dataTable.TableName = "@" + _udoCode;
					if (dataTable.Columns.Contains("SaveStatus"))
					{
						dataTable.Columns.Remove(dataTable.Columns["SaveStatus"]);
					}
					dataTable.AcceptChanges();
					ds.Tables.Add(dataTable);
					string id = ((_udoType == "1") ? Function.ToString(docRow["Code"]) : Function.ToString(docRow["U_DocEntry"]));
					DataSet dataSet = await SearchLine(id);
					if (dataSet != null && dataSet.Tables.Count > 0)
					{
						DataTable dataTable2 = dataSet.Tables[0];
						for (int num = 1; num < dataSet.Tables.Count; num++)
						{
							if (dataSet.Tables[num] != null)
							{
								DataTable dataTable3 = dataSet.Tables[num].Copy();
								dataTable3.TableName = Function.ToString(dataTable2.Rows[num - 1][0]);
								if (dataTable3.Columns.Contains("U_UpdateDate"))
								{
									dataTable3.Columns.Remove("U_UpdateDate");
								}
								if (dataTable3.Columns.Contains("U_UpdateTime"))
								{
									dataTable3.Columns.Remove("U_UpdateTime");
								}
								if (dataTable3.Columns.Contains("TempId"))
								{
									dataTable3.Columns.Remove("TempId");
								}
								dataTable3.AcceptChanges();
								ds.Tables.Add(dataTable3);
							}
						}
					}
					int num2 = Function.ParseInt(docRow["DocEntry"]);
					if (_udoType == "3")
					{
						for (int num3 = 2; num3 < ds.Tables.Count; num3++)
						{
							if (ds.Tables[num3] != null && ds.Tables[num3].Rows.Count > 0)
							{
								for (int num4 = 0; num4 < ds.Tables[num3].Rows.Count; num4++)
								{
									ds.Tables[num3].Rows[num4]["DocEntry"] = num2;
								}
								ds.Tables[num3].AcceptChanges();
							}
						}
					}
					HttpResult httpResult = await _httpClient.Udo.Save(ds);
					if (httpResult == null || string.IsNullOrWhiteSpace(httpResult.Message))
					{
						success++;
						gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "Success");
						gv_Doc.UnselectRow(arrSel[i]);
					}
					else
					{
						gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "Error: " + httpResult.Message);
					}
					progressBar1.PerformStep();
					lbl_Progress.Text = i + 1 + " of " + totalLine + ", " + success + " success.";
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InfoNoRowSelected, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<bool> CheckExists(string id)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "IntegratedUdo");
			return await hp.GetBoolAsync("CheckExists", new
			{
				id = id,
				code = _udoCode,
				type = _udoType
			});
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return true;
		}
	}

	private async Task Search()
	{
		WinCommons._OpenProcessing();
		EnableDisableButton();
		try
		{
			FormMode = ((radioGroupStatus.SelectedIndex != 0) ? Enums.FormViewModeType.Find : Enums.FormViewModeType.Add);
			if (luk_Udos.Properties.DataSource is DataTable dataTable && dataTable.Rows.Count > 0)
			{
				string code = Function.ToString(luk_Udos.EditValue).ToUpper();
				DataRow dataRow = dataTable.AsEnumerable().FirstOrDefault((DataRow r) => Function.ToString(r["Code"]).ToUpper() == code);
				if (dataRow != null)
				{
					_dtOudo = dataTable.Clone();
					_dtOudo.ImportRow(dataRow);
					_dtOudo.AcceptChanges();
					_udoType = Function.ToString(dataRow["TYPE"]);
					_udoCode = Function.ToString(dataRow["Code"]);
					lbl_UDOName.Text = Function.ToString(dataRow["Name"]);
					DataTable dataTable2 = new DataTable();
					dataTable2.Columns.Add(new DataColumn("UdoCode", typeof(string)));
					dataTable2.Columns.Add(new DataColumn("FromDate", typeof(string)));
					dataTable2.Columns.Add(new DataColumn("ToDate", typeof(string)));
					dataTable2.Columns.Add(new DataColumn("IsImported", typeof(string)));
					DataRow dataRow2 = dataTable2.NewRow();
					dataRow2["UdoCode"] = Function.ToString(luk_Udos.EditValue);
					DateTime? dateTime = Function.ParseDateTime(date_From.EditValue);
					DateTime? dateTime2 = Function.ParseDateTime(date_To.EditValue);
					dataRow2["FromDate"] = dateTime?.ToString("yyyyMMdd") ?? "";
					dataRow2["ToDate"] = dateTime2?.ToString("yyyyMMdd") ?? "";
					dataRow2["IsImported"] = ((radioGroupStatus.SelectedIndex == 0) ? "O" : "C");
					dataTable2.Rows.Add(dataRow2);
					dataTable2.AcceptChanges();
					using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "IntegratedUdo");
					DataTable dtRet = await hp.PostAsync("Search", dataTable2);
					gv_Doc.Columns.Clear();
					if (dtRet != null && dtRet.Rows.Count > 0)
					{
						await LoadGridStructure();
						dtRet.Columns.Add(new DataColumn("SaveStatus", typeof(string)));
						grd_Doc.DataSource = dtRet;
						grd_Doc.RefreshDataSource();
						for (int num = 0; num < gv_Doc.Columns.Count; num++)
						{
							gv_Doc.Columns[num].OptionsColumn.ReadOnly = true;
						}
						if (dtRet.Rows.Count == 0)
						{
							tabCtl_Line.TabPages.Clear();
						}
						else if (dtRet.Rows.Count == 1)
						{
							gv_Doc_FocusedRowObjectChanged(null, null);
						}
						if (FormMode == Enums.FormViewModeType.Add)
						{
							for (int num2 = 0; num2 < _lstEditableLineColumns.Count; num2++)
							{
								_lstEditableLineColumns[num2].OptionsColumn.AllowEdit = true;
								_lstEditableLineColumns[num2].OptionsColumn.ReadOnly = false;
							}
						}
						else
						{
							for (int num3 = 0; num3 < _lstEditableLineColumns.Count; num3++)
							{
								_lstEditableLineColumns[num3].OptionsColumn.AllowEdit = false;
								_lstEditableLineColumns[num3].OptionsColumn.ReadOnly = true;
							}
						}
					}
					else
					{
						tabCtl_Line.TabPages.Clear();
						grd_Doc.DataSource = null;
						grd_Doc.RefreshDataSource();
						btn_OK.Text = "OK";
					}
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableButton(enable: true);
		WinCommons._CloseProcessing();
	}

	private async Task LoadUdoCombo()
	{
		_ = 1;
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "IntegratedUdo");
			DataTable dt = await hp.GetAsync("LoadUdoCombo");
			luk_Udos.Properties.DataSource = dt;
			using HttpClientHelper<DataSet> hp2 = new HttpClientHelper<DataSet>(Configuration.ApiServer, "IntegratedUdo");
			if (dt == null || dt.Rows.Count <= 0)
			{
				return;
			}
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				string udoCode = Function.ToString(dt.Rows[i]["Code"]);
				DataSet dataSet = await hp2.GetAsync("GetDocGridStruct", new { udoCode });
				if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
				{
					_lstUdoGridStructDoc.Add(new Tuple<string, DataSet>(udoCode, dataSet));
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadGridStructure()
	{
		_ = 1;
		try
		{
			gv_Doc.Columns.Clear();
			Tuple<string, DataSet> tuple = _lstUdoGridStructDoc.FirstOrDefault((Tuple<string, DataSet> t) => t.Item1.ToUpper() == Function.ToString(luk_Udos.EditValue).ToUpper());
			if (tuple != null)
			{
				DataSet item = tuple.Item2;
				if (item.Tables.Count <= 0 || item.Tables[0] == null || item.Tables[0].Rows.Count <= 0 || item.Tables.Count != 2 || item.Tables[1] == null)
				{
					await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, item.Tables[0], null);
				}
				else
				{
					await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, item.Tables[0], null, item.Tables[1]);
				}
			}
			gv_Doc.OptionsView.EnableAppearanceEvenRow = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task LoadGridLineStructure(GridViewEx gvLine)
	{
		_ = 2;
		try
		{
			if (gvLine == null)
			{
				return;
			}
			GridControlEx grdLine = gvLine.GridControl as GridControlEx;
			gvLine.Columns.Clear();
			using (HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "IntegratedUdo"))
			{
				DataSet dataSet = await hp.GetAsync("GetDocLineGridStruct", new
				{
					udoCode = gvLine.Name.Replace("gvLine_", "")
				});
				if (dataSet.Tables.Count <= 0 || dataSet.Tables[0] == null || dataSet.Tables[0].Rows.Count <= 0 || dataSet.Tables.Count != 2 || dataSet.Tables[1] == null)
				{
					await GridViewHelper.GenerateGrid(grdLine, gvLine, dataSet.Tables[0], null);
				}
				else
				{
					await GridViewHelper.GenerateGrid(grdLine, gvLine, dataSet.Tables[0], null, dataSet.Tables[1]);
				}
			}
			gvLine.OptionsView.EnableAppearanceEvenRow = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<DataSet> SearchLine(string id)
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.AddRange(new DataColumn[2]
			{
				new DataColumn("UdoCode", typeof(string)),
				new DataColumn("CustomID", typeof(string))
			});
			dataTable.AcceptChanges();
			dataTable.Rows.Add(dataTable.NewRow());
			dataTable.Rows[0]["UdoCode"] = _udoCode;
			dataTable.Rows[0]["CustomID"] = id;
			dataTable.AcceptChanges();
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, "IntegratedUdo");
			return await hp.PostAsync("SearchLine", dataTable);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			return new DataSet();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Integrated.frm_IntegratedUdo));
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.tabCtl_Line = new Apzon.Controls.Controls.XtraTabControlEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lbl_Progress = new System.Windows.Forms.Label();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.radioGroupStatus = new Apzon.Controls.Editor.RadioGroupEx();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.luk_Udos = new Apzon.Forms.UserControls.LookUpEditCombo();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_UDOName = new System.Windows.Forms.Label();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.tabCtl_Line).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroupStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Udos.Properties).BeginInit();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(13, 42);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.tabCtl_Line);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(985, 447);
		this.splitContainerControl1.SplitterPosition = 221;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.splitContainerControl1.Resize += new System.EventHandler(this.splitContainerControl1_Resize);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(985, 221);
		this.grd_Doc.TabIndex = 1;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oItegratedUdo";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 50;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "ODOC";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsSelection.CheckBoxSelectorColumnWidth = 40;
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsSelection.MultiSelectMode = DevExpress.XtraGrid.Views.Grid.GridMultiSelectMode.CheckBoxRowSelect;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.RowCellStyle += new DevExpress.XtraGrid.Views.Grid.RowCellStyleEventHandler(this.gv_Doc_RowCellStyle);
		this.gv_Doc.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gv_Doc_SelectionChanged);
		this.gv_Doc.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gv_Doc_FocusedRowObjectChanged);
		this.tabCtl_Line.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tabCtl_Line.IsSearchControl = false;
		this.tabCtl_Line.Location = new System.Drawing.Point(0, 0);
		this.tabCtl_Line.Name = "tabCtl_Line";
		this.tabCtl_Line.Size = new System.Drawing.Size(985, 216);
		this.tabCtl_Line.TabIndex = 0;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Appearance.ForeColor = System.Drawing.Color.FromArgb(32, 31, 53);
		this.btn_OK.Appearance.Options.UseForeColor = true;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(13, 500);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(76, 20);
		this.btn_OK.TabIndex = 8;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(92, 500);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 9;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.lbl_Progress.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_Progress.AutoSize = true;
		this.lbl_Progress.Location = new System.Drawing.Point(785, 503);
		this.lbl_Progress.Name = "lbl_Progress";
		this.lbl_Progress.Size = new System.Drawing.Size(35, 13);
		this.lbl_Progress.TabIndex = 11;
		this.lbl_Progress.Text = "label1";
		this.progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.progressBar1.Location = new System.Drawing.Point(565, 500);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(213, 20);
		this.progressBar1.TabIndex = 10;
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(816, 8);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(107, 20);
		this.btn_Search.TabIndex = 16;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.radioGroupStatus.EditControl = null;
		this.radioGroupStatus.FieldName = null;
		this.radioGroupStatus.IsSearchControl = false;
		this.radioGroupStatus.Location = new System.Drawing.Point(677, 5);
		this.radioGroupStatus.Name = "radioGroupStatus";
		this.radioGroupStatus.Properties.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.radioGroupStatus.Properties.Appearance.Options.UseBackColor = true;
		this.radioGroupStatus.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.NoBorder;
		this.radioGroupStatus.Properties.Items.AddRange(new DevExpress.XtraEditors.Controls.RadioGroupItem[2]
		{
			new DevExpress.XtraEditors.Controls.RadioGroupItem("O", "Open"),
			new DevExpress.XtraEditors.Controls.RadioGroupItem("C", "Close")
		});
		this.radioGroupStatus.SettingInVisible = false;
		this.radioGroupStatus.SettingReadonly = false;
		this.radioGroupStatus.Size = new System.Drawing.Size(121, 26);
		this.radioGroupStatus.TabIndex = 17;
		this.date_To.EditControl = null;
		this.date_To.EditValue = null;
		this.date_To.FieldName = null;
		this.date_To.Location = new System.Drawing.Point(545, 9);
		this.date_To.Name = "date_To";
		this.date_To.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_To.Properties.IsSearchControl = false;
		this.date_To.SettingInVisible = false;
		this.date_To.SettingReadonly = false;
		this.date_To.Size = new System.Drawing.Size(100, 20);
		this.date_To.TabIndex = 15;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(470, 12);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 14;
		this.labelLine2.Utext = "To Date";
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(347, 9);
		this.date_From.Name = "date_From";
		this.date_From.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_From.Properties.IsSearchControl = false;
		this.date_From.SettingInVisible = false;
		this.date_From.SettingReadonly = false;
		this.date_From.Size = new System.Drawing.Size(100, 20);
		this.date_From.TabIndex = 13;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(272, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 12;
		this.labelLine1.Utext = "From Date";
		this.luk_Udos.EditControl = null;
		this.luk_Udos.FieldName = null;
		this.luk_Udos.IsSearchControl = false;
		this.luk_Udos.Location = new System.Drawing.Point(94, 9);
		this.luk_Udos.Name = "luk_Udos";
		this.luk_Udos.Properties.AutoHeight = false;
		this.luk_Udos.Properties.BestFitMode = DevExpress.XtraEditors.Controls.BestFitMode.BestFit;
		this.luk_Udos.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.luk_Udos.Properties.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Code")
		});
		this.luk_Udos.Properties.DisplayMember = "Name";
		this.luk_Udos.Properties.IsSearchControl = false;
		this.luk_Udos.Properties.NullText = "";
		this.luk_Udos.Properties.ShowFooter = false;
		this.luk_Udos.Properties.ShowHeader = false;
		this.luk_Udos.Properties.ValueMember = "Code";
		this.luk_Udos.SettingInVisible = false;
		this.luk_Udos.SettingReadonly = false;
		this.luk_Udos.Size = new System.Drawing.Size(157, 20);
		this.luk_Udos.TabIndex = 18;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "UDO code";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(13, 12);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(80, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 19;
		this.labelLine3.Utext = "UDO code";
		this.lbl_UDOName.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_UDOName.AutoSize = true;
		this.lbl_UDOName.Location = new System.Drawing.Point(286, 503);
		this.lbl_UDOName.Name = "lbl_UDOName";
		this.lbl_UDOName.Size = new System.Drawing.Size(72, 13);
		this.lbl_UDOName.TabIndex = 20;
		this.lbl_UDOName.Text = "lbl_UDOName";
		this.labelLine4.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Current UDO";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(200, 502);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 21;
		this.labelLine4.Utext = "Current UDO";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1010, 532);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.lbl_UDOName);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.luk_Udos);
		base.Controls.Add(this.btn_Search);
		base.Controls.Add(this.radioGroupStatus);
		base.Controls.Add(this.date_To);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.date_From);
		base.Controls.Add(this.labelLine1);
		base.Controls.Add(this.lbl_Progress);
		base.Controls.Add(this.progressBar1);
		base.Controls.Add(this.btn_OK);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oItegratedUdo";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_IntegratedUdo";
		this.Text = "Integrated Udo";
		base.Load += new System.EventHandler(this.frm_IntegratedUdo_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.tabCtl_Line).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroupStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.luk_Udos.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
