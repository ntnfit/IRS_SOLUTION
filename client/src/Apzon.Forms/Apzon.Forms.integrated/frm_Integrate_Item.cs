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
using Apzon.Entities.Models.MasterData;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Integrated;

public class frm_Integrate_Item : XtraFormEx
{
	private string _formController = "IntegratedItem";

	private IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int _manualSeries;

	private List<string> _asyncSelCols = new List<string>
	{
		"ItemCode", "ItemName", "ItmsGrpCod", "U_spec1Grp", "U_spec2Grp", "U_spec3Grp", "U_spec1Code", "U_spec1Name", "U_spec2Code", "U_spec2Name",
		"U_spec3Code", "U_spec3Name", "U_itemTypeCode", "U_itemTypeName", "U_originCode", "U_originName", "InvntryUom", "BuyUnitMsr", "NumInBuy", "SalUnitMsr",
		"NumInSale", "U_ID"
	};

	private IContainer components;

	private Label lbl_Progress;

	private ProgressBar progressBar1;

	private SimpleButtonEx btn_OK;

	private SimpleButtonEx btn_Cancel;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButtonEx btn_Search;

	private RadioGroupEx radioGroupStatus;

	private DateEditEx date_To;

	private LabelLine labelLine2;

	private DateEditEx date_From;

	private LabelLine labelLine1;

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

	public frm_Integrate_Item()
	{
		InitializeComponent();
		FormMode = Enums.FormViewModeType.Add;
	}

	private async void frm_Integrate_Item_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			await LoadManualSeries();
			lbl_Progress.Text = "";
			lbl_Progress.Visible = false;
			progressBar1.Visible = false;
			date_From.EditValue = DateTime.Today.AddDays(-30.0);
			date_To.EditValue = DateTime.Today;
			radioGroupStatus.SelectedIndex = 0;
			await LoadGridStructure();
			await Search();
			WindowState = FormWindowState.Maximized;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
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

	private async void btn_Search_Click(object sender, EventArgs e)
	{
		await Search();
	}

	private async void btn_OK_Click(object sender, EventArgs e)
	{
		WinCommons._OpenProcessing();
		EnableDisableButton();
		try
		{
			if (FormMode == Enums.FormViewModeType.Add)
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

	private async Task Search()
	{
		WinCommons._OpenProcessing();
		EnableDisableButton();
		try
		{
			FormMode = ((radioGroupStatus.SelectedIndex != 0) ? Enums.FormViewModeType.Find : Enums.FormViewModeType.Add);
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add(new DataColumn("FromDate", typeof(string)));
			dataTable.Columns.Add(new DataColumn("ToDate", typeof(string)));
			dataTable.Columns.Add(new DataColumn("IsImported", typeof(string)));
			DataRow dataRow = dataTable.NewRow();
			DateTime? dateTime = Function.ParseDateTime(date_From.EditValue);
			DateTime? dateTime2 = Function.ParseDateTime(date_To.EditValue);
			dataRow["FromDate"] = dateTime?.ToString("yyyyMMdd") ?? "";
			dataRow["ToDate"] = dateTime2?.ToString("yyyyMMdd") ?? "";
			dataRow["IsImported"] = ((radioGroupStatus.SelectedIndex == 0) ? "O" : "C");
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			DataTable dataTable2 = await hp.PostAsync("Search", dataTable);
			if (dataTable2 != null && dataTable2.Rows.Count > 0)
			{
				grd_Doc.DataSource = dataTable2;
				grd_Doc.RefreshDataSource();
				for (int i = 0; i < gv_Doc.Columns.Count; i++)
				{
					gv_Doc.Columns[i].OptionsColumn.ReadOnly = true;
				}
			}
			else
			{
				grd_Doc.DataSource = null;
				grd_Doc.RefreshDataSource();
				FormMode = Enums.FormViewModeType.View;
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

	private async Task LoadManualSeries()
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, _formController);
			_manualSeries = await hp.GetIntAsync("GetManualSeries");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Save()
	{
		_ = 2;
		try
		{
			new DataTable().Columns.Add("ItemCode", typeof(string));
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
				int[] arrSel = source.ToArray();
				int totalLine = arrSel.Length;
				progressBar1.Maximum = totalLine + 1;
				progressBar1.Minimum = 1;
				progressBar1.Step = 1;
				progressBar1.Value = 1;
				DataTable dtStruct = WinFuntions.GetTableStructure("OITM");
				List<string> revertCols = new List<string>();
				for (int num = 0; num < dtStruct.Columns.Count; num++)
				{
					if (!_asyncSelCols.Contains(dtStruct.Columns[num].ColumnName))
					{
						revertCols.Add(dtStruct.Columns[num].ColumnName);
					}
				}
				for (int i = 0; i < arrSel.Length; i++)
				{
					DataRow dataRow = gv_Doc.GetDataRow(arrSel[i]);
					dtStruct.TableName = "OITM";
					DataTable dtDoc = WinFuntions.GetDataTableNewRow(dtStruct);
					DataTable dataTable = dataRow.Table.Clone();
					dataTable.ImportRow(dataRow);
					dataTable.AcceptChanges();
					for (int num2 = 0; num2 < dataTable.Columns.Count; num2++)
					{
						string columnName = dataTable.Columns[num2].ColumnName;
						if (dtDoc.Columns.Contains(columnName) && dataTable.Rows[0][columnName] != DBNull.Value && dataTable.Rows[0][columnName] != null)
						{
							object valueByType = Function.GetValueByType(dtDoc.Columns[columnName].DataType.Name.ToLower(), dataTable.Rows[0][columnName]);
							dtDoc.Rows[0][columnName] = valueByType ?? DBNull.Value;
						}
					}
					dtDoc.AcceptChanges();
					DataTable aDoc = _httpClient.Items.GetADoc(Function.ToString(dtDoc.Rows[0]["ItemCode"]));
					if (aDoc != null && aDoc.Rows.Count > 0)
					{
						for (int num3 = 0; num3 < revertCols.Count; num3++)
						{
							object valueByType2 = Function.GetValueByType(aDoc.Columns[revertCols[num3]].DataType.Name.ToLower(), aDoc.Rows[0][revertCols[num3]]);
							dtDoc.Rows[0][revertCols[num3]] = valueByType2 ?? DBNull.Value;
						}
					}
					ItemsImpl doc = Function.DataRowToEntity<ItemsImpl>(dtDoc.Rows[0]);
					List<UserFieldsImpl> list = await WinFuntions.GetUdfFields("OITM");
					if (list != null && list.Count > 0)
					{
						DataTable dataTable2 = dtDoc.Copy();
						if (dataTable2 != null && dataTable2.Rows.Count > 0)
						{
							DataRow dataRow2 = dataTable2.Rows[0];
							doc.ListUserFields = new List<UserFieldsImpl>();
							for (int num4 = 0; num4 < list.Count; num4++)
							{
								UserFieldsImpl userFieldsImpl = new UserFieldsImpl();
								userFieldsImpl.Name = list[num4].Name;
								userFieldsImpl.Value = Function.ToString(dataRow2[list[num4].Name]);
								userFieldsImpl.Type = list[num4].Type;
								doc.ListUserFields.Add(userFieldsImpl);
							}
						}
					}
					doc.APZ_SAVE_ACTION = ((await ExtenseionMethods.CheckExistItem(doc.ItemCode)) ? "U" : "A");
					doc.Series = _manualSeries;
					if (doc.validFor == "N" && doc.frozenFor == "N")
					{
						doc.validFor = "Y";
					}
					if (doc.APZ_SAVE_ACTION == "A")
					{
						doc.SalFactor1 = 1.0;
						doc.SalFactor2 = 1.0;
						doc.SalFactor3 = 1.0;
						doc.SalFactor4 = 1.0;
						doc.PurFactor1 = 1.0;
						doc.PurFactor2 = 1.0;
						doc.PurFactor3 = 1.0;
						doc.PurFactor4 = 1.0;
						doc.SVolUnit = 1;
						doc.BVolUnit = 1;
						doc.ItemProperties = new List<string>();
						for (int num5 = 0; num5 < 64; num5++)
						{
							doc.ItemProperties.Add("N");
						}
						doc.GLMethod = "C";
						doc.ManBtchNum = "Y";
						doc.OneBOneRec = "Y";
						doc.EvalSystem = "B";
						doc.IssueMthd = "M";
						doc.MngMethod = "A";
					}
					doc.OldItemCode = doc.ItemCode;
					string text = await _httpClient.Items.SaveDoc(doc);
					if (string.IsNullOrWhiteSpace(text))
					{
						success++;
						gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "Success");
						gv_Doc.UnselectRow(arrSel[i]);
					}
					else
					{
						gv_Doc.SetRowCellValue(arrSel[i], "SaveStatus", "Error: " + text);
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

	private async Task LoadGridStructure()
	{
		_ = 2;
		try
		{
			using HttpClientHelper<DataSet> hp = new HttpClientHelper<DataSet>(Configuration.ApiServer, _formController);
			DataSet dataSet = await hp.GetAsync("GetDocGridStruct");
			if (dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0] != null && dataSet.Tables[0].Rows.Count > 0)
			{
				if (dataSet.Tables.Count != 2 || dataSet.Tables[1] == null)
				{
					await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, dataSet.Tables[0], null);
				}
				else
				{
					await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, dataSet.Tables[0], null, dataSet.Tables[1]);
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
				btn_OK.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Integrated.frm_Integrate_Item));
		this.lbl_Progress = new System.Windows.Forms.Label();
		this.progressBar1 = new System.Windows.Forms.ProgressBar();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.radioGroupStatus = new Apzon.Controls.Editor.RadioGroupEx();
		this.date_To = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.date_From = new Apzon.Controls.Editor.DateEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroupStatus.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).BeginInit();
		base.SuspendLayout();
		this.lbl_Progress.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.lbl_Progress.AutoSize = true;
		this.lbl_Progress.Location = new System.Drawing.Point(784, 544);
		this.lbl_Progress.Name = "lbl_Progress";
		this.lbl_Progress.Size = new System.Drawing.Size(35, 13);
		this.lbl_Progress.TabIndex = 15;
		this.lbl_Progress.Text = "label1";
		this.progressBar1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.progressBar1.Location = new System.Drawing.Point(564, 541);
		this.progressBar1.Name = "progressBar1";
		this.progressBar1.Size = new System.Drawing.Size(213, 20);
		this.progressBar1.TabIndex = 14;
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.Appearance.ForeColor = System.Drawing.Color.FromArgb(32, 31, 53);
		this.btn_OK.Appearance.Options.UseForeColor = true;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(12, 541);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(76, 20);
		this.btn_OK.TabIndex = 12;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(92, 541);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 13;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(12, 38);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(971, 497);
		this.grd_Doc.TabIndex = 1;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oItegratedItem";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 50;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "OITM";
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
		this.btn_Search.EditControl = null;
		this.btn_Search.EditValue = "Search";
		this.btn_Search.FieldName = null;
		this.btn_Search.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Search.ImageOptions.Image");
		this.btn_Search.IsSearchControl = false;
		this.btn_Search.Location = new System.Drawing.Point(556, 8);
		this.btn_Search.Name = "btn_Search";
		this.btn_Search.ReadOnly = false;
		this.btn_Search.SettingInVisible = false;
		this.btn_Search.SettingReadonly = false;
		this.btn_Search.Size = new System.Drawing.Size(107, 20);
		this.btn_Search.TabIndex = 22;
		this.btn_Search.Text = "Search";
		this.btn_Search.Click += new System.EventHandler(this.btn_Search_Click);
		this.radioGroupStatus.EditControl = null;
		this.radioGroupStatus.FieldName = null;
		this.radioGroupStatus.IsSearchControl = false;
		this.radioGroupStatus.Location = new System.Drawing.Point(417, 5);
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
		this.radioGroupStatus.TabIndex = 23;
		this.date_To.EditControl = null;
		this.date_To.EditValue = null;
		this.date_To.FieldName = null;
		this.date_To.Location = new System.Drawing.Point(285, 9);
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
		this.date_To.TabIndex = 21;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "To Date";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(210, 12);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(80, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 20;
		this.labelLine2.Utext = "To Date";
		this.date_From.EditControl = null;
		this.date_From.EditValue = null;
		this.date_From.FieldName = null;
		this.date_From.Location = new System.Drawing.Point(87, 9);
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
		this.date_From.TabIndex = 19;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "From Date";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(12, 12);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(80, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 18;
		this.labelLine1.Utext = "From Date";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(995, 570);
		base.Controls.Add(this.grd_Doc);
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
		base.FormId = "oItegratedItem";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Integrate_Item";
		this.Text = "Integrate Item";
		base.Load += new System.EventHandler(this.frm_Integrate_Item_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.radioGroupStatus.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_To.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_From.Properties).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
