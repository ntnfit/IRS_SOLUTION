using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.Queries;
using Apzon.Forms.Helper;
using Apzon.Forms.Reports;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.Utils.Extensions;
using DevExpress.XtraEditors;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using FastColoredTextBoxNS;

namespace Apzon.Forms.Administrator;

public class frm_QueryPreview : XtraFormEx
{
	protected FastColoredTextBox txtQuery;

	private int _structureId;

	public string Sql = string.Empty;

	public bool IsLoad;

	public bool ManagerQ;

	public int QueryId;

	public string Captonbtnsave = Resources.ButtonOK;

	public string ActionId = "";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public List<QueryParamater> QueryParamaters = new List<QueryParamater>();

	private Type[] types = new Type[5]
	{
		typeof(double),
		typeof(int),
		typeof(short),
		typeof(long),
		typeof(decimal)
	};

	private IContainer components;

	private GridControlEx grd_Doc;

	private SimpleButtonEx btn_Execute;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_LoadSavedQueries;

	private SplitContainerControl splitContainerControl1;

	private GridViewEx gv_Doc;

	private SimpleButtonEx btn_Save;

	private SimpleButtonEx btn_EditQuery;

	public frm_QueryPreview()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_QueryPreview"))
		{
			InitializeComponent();
			InitEditor();
			splitContainerControl1.PanelVisibility = SplitPanelVisibility.Panel2;
			IsLoad = true;
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_QueryPreview" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btn_Execute.Enabled = false;
				btn_Save.Enabled = false;
			}
			else
			{
				btn_Execute.Enabled = true;
				btn_Save.Enabled = true;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private void InitEditor()
	{
		txtQuery = new FastColoredTextBox
		{
			Language = Language.SQL,
			Font = new Font("Courier New", 11.25f),
			Paddings = new Padding(10),
			LeftPadding = 17,
			CaretBlinking = false,
			Dock = DockStyle.Fill
		};
		splitContainerControl1.Panel1.AddControl(txtQuery);
	}

	private async void frm_QueryPreview_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			frm_QueryPreview frm_QueryPreview2 = this;
			string formId = (gv_Doc.FormId = ApzObjectTypes.oUserQueries.ToString());
			frm_QueryPreview2.FormId = formId;
			gv_Doc.ItemId = ((QueryId > 0) ? QueryId.ToString() : ActionId);
			gv_Doc.DisableFormSetting = true;
			EnableDisableControl(isEnable: false);
			if (QueryId != 0 && string.IsNullOrEmpty(Sql))
			{
				DataTable dataTable = await _httpClient.Queries.GetQueryById(QueryId);
				if (dataTable.IsNotNull())
				{
					Sql = Function.ToString(dataTable.Rows[0]["QString"]);
					Text = Function.ToString(dataTable.Rows[0]["QName"]);
				}
			}
			if (!ManagerQ)
			{
				if (IsLoad)
				{
					txtQuery.Text = Sql;
					await Execute();
				}
				else
				{
					Sql = txtQuery.Text;
				}
			}
			else
			{
				txtQuery.Text = Sql;
				await Execute();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		WinCommons._CloseProcessing();
		EnableDisableControl(isEnable: true);
	}

	private async void bt_Execute_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			if (!string.IsNullOrEmpty(txtQuery.Text.ToUpper()))
			{
				await Execute();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControl(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void bt_LoadSavedQueries_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			frm_ManagerQuery frm_ManagerQuery2 = new frm_ManagerQuery
			{
				Captonbtnsave = Resources.ButtonOK,
				Type = "dialog",
				Location = new Point(300, 200)
			};
			frm_ManagerQuery2.ShowDialog(this);
			if (!string.IsNullOrEmpty(frm_ManagerQuery2.SqlOpen))
			{
				IsLoad = false;
				txtQuery.Text = frm_ManagerQuery2.SqlOpen;
				Sql = txtQuery.Text;
				QueryId = frm_ManagerQuery2.QueryId;
				gv_Doc.ItemId = QueryId.ToString();
				grd_Doc.DataSource = null;
				await Execute();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		EnableDisableControl(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			EnableDisableControl(isEnable: false);
			if (string.IsNullOrEmpty(txtQuery.Text))
			{
				WinCommons.Error(Enums.MessageCode.QueryIsNull, "");
				EnableDisableControl(isEnable: true);
				return;
			}
			frm_ManagerQuery frm_ManagerQuery2 = new frm_ManagerQuery(Enums.FormViewModeType.Update);
			frm_ManagerQuery2.SqlSave = txtQuery.Text;
			frm_ManagerQuery2.Location = new Point(300, 200);
			frm_ManagerQuery2.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		EnableDisableControl(isEnable: true);
		WinCommons._CloseProcessing();
	}

	private async Task Execute()
	{
		if (!IsLoad)
		{
			gv_Doc.ShowLoadingPanel();
		}
		try
		{
			if (string.IsNullOrEmpty(txtQuery.Text))
			{
				return;
			}
			string filter = await WinFuntions.ShowFilterSetting("Q-" + QueryId);
			string input = txtQuery.Text;
			input = Regex.Replace(input, "@CURRENTUSER", $"N'{GlobalData.UserName}'", RegexOptions.IgnoreCase);
			input = Regex.Replace(input, "\\$\\[USER\\]", $"N'{GlobalData.UserID}'", RegexOptions.IgnoreCase);
			input = Regex.Replace(input, "@CURRENTBRANCH", $"N'{GlobalData.DefaultBranch}'", RegexOptions.IgnoreCase);
			List<DatabaseParamater> list = Function.ParseSql(input);
			List<QueryParamater> prs = new List<QueryParamater>();
			if (QueryParamaters == null || (QueryParamaters != null && QueryParamaters.Count == 0))
			{
				if (list != null && list.Count > 0)
				{
					frm_SetParamater frm_SetParamater2 = new frm_SetParamater
					{
						List = list,
						StartPosition = FormStartPosition.CenterScreen
					};
					if (frm_SetParamater2.ShowDialog(this) != DialogResult.OK)
					{
						return;
					}
					prs = frm_SetParamater2.Paramater;
				}
			}
			else
			{
				prs = QueryParamaters;
				QueryParamaters = new List<QueryParamater>();
			}
			DataSet ds = _httpClient.Commons.ExecuteSql(input, prs, filter);
			grd_Doc.DataSource = null;
			gv_Doc.Columns.Clear();
			if (ds != null && ds.Tables.Count > 1)
			{
				DataTable dataTable = ds.Tables["Message"];
				if (dataTable.IsNotNull())
				{
					string text = Function.ToString(dataTable.Rows[0]["MsgType"]);
					string msg = Function.ToString(dataTable.Rows[0]["Message"]);
					if (!(text == "200"))
					{
						if (text == "5xx")
						{
							WinCommons.Error(Enums.MessageCode.None, msg);
						}
					}
					else
					{
						WinCommons.Success(Enums.MessageCode.None, msg);
						ds.Tables.RemoveAt(0);
					}
				}
				DataTable dt = ds.Tables[0];
				if (dt != null && dt.Columns.Count != 0)
				{
					DataSet dataSet = await _httpClient.FormSettingClient.GetStructure(0, gv_Doc.FormId, gv_Doc.ItemId, GlobalData.UserName, GlobalData.DefaultBranch, "0");
					if (dataSet.IsNotNull() && dataSet.Tables[0].IsNotNull())
					{
						_structureId = Function.ParseInt(dataSet.Tables[0].Rows[0]["ID"]);
						dataSet.Tables[0].TableName = "Structure";
						DataTable dataTable2 = new DataTable();
						if (dataSet.IsHasStructure())
						{
							dataSet.Tables[1].TableName = "Valid";
							dataTable2 = dataSet.Tables[1].Copy();
							dataSet.Tables.RemoveAt(1);
						}
						for (int i = 0; i < ds.Tables.Count; i++)
						{
							dataSet.Tables.Add(ds.Tables[i].Copy());
						}
						dataSet.Tables.Add(dataTable2.Copy());
						await GridViewHelper.GenerateGrid(grd_Doc, gv_Doc, dataSet);
					}
					else
					{
						gv_Doc.BeginInit();
						foreach (DataColumn a in dt.Columns)
						{
							GridColumn gridColumn = gv_Doc.Columns.Add();
							gridColumn.Name = a.ColumnName;
							gridColumn.FieldName = a.ColumnName;
							gridColumn.Visible = true;
							gridColumn.OptionsFilter.AllowFilter = true;
							gridColumn.OptionsFilter.FilterPopupMode = FilterPopupMode.Default;
							Type dataType = a.DataType;
							if (dataType == typeof(double) || dataType == typeof(float) || dataType == typeof(double?) || dataType == typeof(float?) || dataType == typeof(decimal) || dataType == typeof(decimal?))
							{
								if (new string[6] { "qty", "quantity", "stock", "onhand", "order", "commit" }.Any((string t) => a.ColumnName.ToLower().Contains(t)))
								{
									gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
									gridColumn.DisplayFormat.FormatString = "###" + GlobalData.GeneralSetting.ThousSep + "###" + GlobalData.GeneralSetting.DecSep + new string('#', GlobalData.GeneralSetting.QtyDec ?? 6);
								}
								else if (new string[1] { "price" }.Any((string t) => a.ColumnName.ToLower().Contains(t)))
								{
									gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
									gridColumn.DisplayFormat.FormatString = "n" + (GlobalData.GeneralSetting.PriceDec ?? 6);
								}
								else if (new string[3] { "amount", "total", "amt" }.Any((string t) => a.ColumnName.ToLower().Contains(t)))
								{
									gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
									gridColumn.DisplayFormat.FormatString = "n" + (GlobalData.GeneralSetting.PriceDec ?? 6);
								}
								else if (new string[1] { "rate" }.Any((string t) => a.ColumnName.ToLower().Contains(t)))
								{
									gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
									gridColumn.DisplayFormat.FormatString = "n" + (GlobalData.GeneralSetting.RateDec ?? 6);
								}
								else if (new string[4] { "percent", "discount", "disc", "prcnt" }.Any((string t) => a.ColumnName.ToLower().Contains(t)))
								{
									gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
									gridColumn.DisplayFormat.FormatString = "n" + (GlobalData.GeneralSetting.PercentDec ?? 6);
								}
								else
								{
									gridColumn.DisplayFormat.FormatType = FormatType.Numeric;
									gridColumn.DisplayFormat.FormatString = "n" + (GlobalData.GeneralSetting.QueryDec ?? 6);
								}
							}
							else if (dataType == typeof(DateTime) || dataType == typeof(DateTime?))
							{
								List<string> list2 = new List<string> { "/", "-", ".", "\\" };
								string dateTimeFormat = GlobalData.DateTimeFormat;
								try
								{
									list2.Add(GlobalData.DateSeparatorFormat);
								}
								catch (Exception ex)
								{
									Logging.Write("TRACE", ex);
								}
								gridColumn.DisplayFormat.FormatType = FormatType.DateTime;
								gridColumn.DisplayFormat.FormatString = dateTimeFormat;
							}
						}
						gv_Doc.EndInit();
						grd_Doc.DataSource = dt;
						grd_Doc.RefreshDataSource();
						await gv_Doc.ReloadFormSetting();
					}
					ds.DisposeDataSet();
				}
			}
		}
		catch (Exception ex2)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex2.Message);
		}
		if (!IsLoad)
		{
			gv_Doc.HideLoadingPanel();
		}
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			btn_Save.Enabled = isEnable;
			btn_Execute.Enabled = isEnable;
			btn_Cancel.Enabled = isEnable;
			btn_LoadSavedQueries.Enabled = isEnable;
			txtQuery.ReadOnly = !isEnable;
			gv_Doc.ReadOnly = isEnable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private void btn_EditQuery_Click(object sender, EventArgs e)
	{
		try
		{
			splitContainerControl1.PanelVisibility = SplitPanelVisibility.Both;
			txtQuery.Focus();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gv_Doc_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		try
		{
			GridCell[] selectedCells = gv_Doc.GetSelectedCells();
			if (selectedCells.Length == 0)
			{
				return;
			}
			double num = 0.0;
			GridCell[] array = selectedCells;
			foreach (GridCell gridCell in array)
			{
				if (types.Contains(gridCell.Column.ColumnType))
				{
					num += Function.ParseDouble(gv_Doc.GetRowCellValue(gridCell.RowHandle, gridCell.Column));
				}
			}
			double num2 = num / (double)selectedCells.Length;
			if (num != 0.0)
			{
				WinCommons.InfoForm(Enums.MessageCode.None, string.Format("AVERAGE:{0:n" + GlobalData.GeneralSetting.SumDec + "}\tSUM: {1:n" + GlobalData.GeneralSetting.SumDec + "}\t COUNT: {2:n0}", num2, num, selectedCells.Length));
			}
			else
			{
				WinCommons.InfoForm(Enums.MessageCode.None, $" COUNT: {selectedCells.Length:n0}");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public override void FORMSETTING()
	{
		try
		{
			if (string.IsNullOrEmpty(gv_Doc.ItemId))
			{
				return;
			}
			WinCommons._OpenProcessing();
			frm_ReportFormSettings frm = new frm_ReportFormSettings("1", grd_Doc, new TreeEx(), gv_Doc.ItemId, FormId, _structureId, grd_Doc.DataSourceTable.Clone())
			{
				Text = string.Format("{0} - {1}", "Form Settings", Text),
				MdiParent = MdiParent,
				WindowState = FormWindowState.Normal,
				StartPosition = FormStartPosition.CenterScreen
			};
			frm.FormClosed += (object s, FormClosedEventArgs e) =>
			{
				if (frm.isSave)
				{
					_structureId = frm.id;
				}
			};
			frm.Show();
			WinCommons._CloseProcessing();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_QueryPreview));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.btn_Execute = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_LoadSavedQueries = new Apzon.Controls.Controls.SimpleButtonEx();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_EditQuery = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = false;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(0, 0);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.Size = new System.Drawing.Size(672, 291);
		this.grd_Doc.TabIndex = 1;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oQueryPreview";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.HasFormSetting = false;
		this.gv_Doc.HeaderExcelAlign = Apzon.Commons.Enums.Enums.HeaderExcelAlign.Left;
		this.gv_Doc.HeaderExcelExport = null;
		this.gv_Doc.HeaderExcelType = Apzon.Commons.Enums.Enums.HeaderExcelType.Disabled;
		this.gv_Doc.IgnoreFilter = false;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 50;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.IsMappingContextAction = false;
		this.gv_Doc.ItemId = "ORESULT";
		this.gv_Doc.ItemId2 = null;
		this.gv_Doc.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.LstOldUDF = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsBehavior.ReadOnly = true;
		this.gv_Doc.OptionsSelection.MultiSelect = true;
		this.gv_Doc.OptionsView.ColumnAutoWidth = false;
		this.gv_Doc.OptionsView.ShowFooter = true;
		this.gv_Doc.ReadOnly = true;
		this.gv_Doc.SettingReadonly = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.SelectionChanged += new DevExpress.Data.SelectionChangedEventHandler(this.gv_Doc_SelectionChanged);
		this.btn_Execute.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Execute.EditControl = null;
		this.btn_Execute.EditValue = "Execute";
		this.btn_Execute.FieldName = null;
		this.btn_Execute.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Execute.ImageOptions.Image");
		this.btn_Execute.IsSearchControl = false;
		this.btn_Execute.Location = new System.Drawing.Point(2, 429);
		this.btn_Execute.Name = "btn_Execute";
		this.btn_Execute.ReadOnly = false;
		this.btn_Execute.SettingInVisible = false;
		this.btn_Execute.SettingReadonly = false;
		this.btn_Execute.Size = new System.Drawing.Size(75, 20);
		this.btn_Execute.TabIndex = 2;
		this.btn_Execute.Text = "Execute";
		this.btn_Execute.Click += new System.EventHandler(this.bt_Execute_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(80, 429);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 20);
		this.btn_Cancel.TabIndex = 2;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		this.btn_LoadSavedQueries.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_LoadSavedQueries.EditControl = null;
		this.btn_LoadSavedQueries.EditValue = "Open Queries";
		this.btn_LoadSavedQueries.FieldName = null;
		this.btn_LoadSavedQueries.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_LoadSavedQueries.ImageOptions.Image");
		this.btn_LoadSavedQueries.IsSearchControl = false;
		this.btn_LoadSavedQueries.Location = new System.Drawing.Point(576, 430);
		this.btn_LoadSavedQueries.Name = "btn_LoadSavedQueries";
		this.btn_LoadSavedQueries.ReadOnly = false;
		this.btn_LoadSavedQueries.SettingInVisible = false;
		this.btn_LoadSavedQueries.SettingReadonly = false;
		this.btn_LoadSavedQueries.Size = new System.Drawing.Size(92, 20);
		this.btn_LoadSavedQueries.TabIndex = 3;
		this.btn_LoadSavedQueries.Text = "Open Queries";
		this.btn_LoadSavedQueries.Click += new System.EventHandler(this.bt_LoadSavedQueries_Click);
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(2, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Doc);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(672, 422);
		this.splitContainerControl1.SplitterPosition = 127;
		this.splitContainerControl1.TabIndex = 4;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "Save Query";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(482, 430);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(91, 20);
		this.btn_Save.TabIndex = 5;
		this.btn_Save.Text = "Save Query";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_EditQuery.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_EditQuery.EditControl = null;
		this.btn_EditQuery.EditValue = "Edit Query";
		this.btn_EditQuery.FieldName = null;
		this.btn_EditQuery.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_EditQuery.ImageOptions.Image");
		this.btn_EditQuery.IsSearchControl = false;
		this.btn_EditQuery.Location = new System.Drawing.Point(395, 430);
		this.btn_EditQuery.Name = "btn_EditQuery";
		this.btn_EditQuery.ReadOnly = false;
		this.btn_EditQuery.SettingInVisible = false;
		this.btn_EditQuery.SettingReadonly = false;
		this.btn_EditQuery.Size = new System.Drawing.Size(83, 20);
		this.btn_EditQuery.TabIndex = 8;
		this.btn_EditQuery.Text = "Edit Query";
		this.btn_EditQuery.Click += new System.EventHandler(this.btn_EditQuery_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(674, 454);
		base.Controls.Add(this.btn_EditQuery);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.splitContainerControl1);
		base.Controls.Add(this.btn_LoadSavedQueries);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_Execute);
		base.FormId = "oQueryPreview";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_QueryPreview.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_QueryPreview";
		this.Text = "Query Preview";
		base.Load += new System.EventHandler(this.frm_QueryPreview_Load);
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
