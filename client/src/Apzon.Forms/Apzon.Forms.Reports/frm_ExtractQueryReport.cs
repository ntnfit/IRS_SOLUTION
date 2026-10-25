using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons.CFL;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraRichEdit;
using DevExpress.XtraTab;
using DevExpress.XtraWizard;

namespace Apzon.Forms.Reports;

public class frm_ExtractQueryReport : XtraFormEx
{
	private const string FormText = "Extract Query Report";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	private GridViewEx gridView4;

	private GridControlEx grd_Parameters;

	private GridViewEx gv_Parameters;

	private WizardPage wzp_Columns;

	private GridControlEx grd_Columns;

	private GridViewEx gv_Columns;

	private WizardPage wzp_Parmeters;

	private SplitContainerControl splitContainerControl1;

	private XtraTabControlEx xtraTabControl1;

	private XtraTabPageEx xtraTabPage1;

	private GridControlEx grd_ValidValues;

	private GridViewEx gv_ValidValues;

	private GridViewEx gridView5;

	private WizardPage wzp_SelectQuery;

	private LabelLine labelLine6;

	private RichEditControl txt_Query;

	private LookUpEdit look_QueryType;

	private LabelLine labelLine5;

	private LabelLine labelLine4;

	private LookUpEdit look_Category;

	private LabelLine labelLine3;

	private TextEditEx txt_RptDesc;

	private LabelLine labelLine2;

	private TextEditEx txt_RptName;

	private LabelLine labelLine1;

	private WizardControlEx wizardControlEx_ExtractQueryReport;

	private WizardPage wzp_Subreport;

	private SplitContainerControl splitContainerControl2;

	private GridControlEx grd_Subreport;

	private GridViewEx gv_Subreport;

	private XtraTabControlEx xtraTabControl2;

	private XtraTabPageEx xtraTabPage2;

	private GridControlEx grd_Subreport_Parameters;

	private GridViewEx gv_Subreport_Paramaters;

	private ButtonEditEx bte_QueryName;

	private GridColumnEx gridColumn1;

	private GridColumnEx gridColumn2;

	private GridColumnEx gridColumn3;

	private GridColumnEx gridColumn4;

	private RepositoryItemLookUpEdit rep_DbType;

	private GridColumnEx gridColumn5;

	private GridColumnEx gridColumn6;

	private RepositoryItemCheckEdit rep_IsNullable;

	private GridColumnEx gridColumn7;

	private RepositoryItemCheckEdit rep_isApply;

	private GridColumnEx gridColumn8;

	private RepositoryItemLookUpEdit rep_SourceType;

	private GridColumnEx gridColumn9;

	private RepositoryItemButtonEdit rep_Query;

	private GridColumnEx gridColumn10;

	private GridColumnEx gridColumn11;

	private RepositoryItemLookUpEdit rep_ObjectType;

	private GridColumnEx gridColumn12;

	private GridColumnEx gridColumn13;

	private GridColumnEx gridColumn14;

	private GridColumnEx gridColumn15;

	private GridColumnEx gridColumn16;

	private GridColumnEx gridColumn17;

	private GridColumnEx gridColumn18;

	private GridColumnEx gridColumn19;

	private GridColumnEx gridColumn20;

	private GridColumnEx gridColumn21;

	private GridColumnEx gridColumn22;

	private GridColumnEx gridColumn23;

	private RepositoryItemButtonEdit rep_QueryReport;

	private GridColumnEx gridColumn24;

	private GridColumnEx gridColumn25;

	private GridColumnEx gridColumn26;

	private GridColumnEx gridColumn27;

	private RepositoryItemLookUpEdit rep_SubQueryParameters;

	private GridColumnEx gridColumn28;

	private RepositoryItemLookUpEdit rep_Columns;

	private RepositoryItemLookUpEdit rep_ColumnObjectType;

	public int Id { get; set; }

	public int QueryId { get; set; }

	public frm_ExtractQueryReport()
	{
		InitializeComponent();
	}

	private void frm_ExtractQueryReport_Load(object sender, EventArgs e)
	{
		try
		{
			look_QueryType.SetDataSource(new Dictionary<string, string>
			{
				{ "T", "Text" },
				{ "S", "Store procedure" }
			});
			rep_SourceType.SetDataSource(new Dictionary<string, string>
			{
				{ "M", "Manual" },
				{ "V", "Valid Values" },
				{ "S", "TSQL Query" }
			});
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			foreach (ApzObjectTypes value in Enum.GetValues(typeof(ApzObjectTypes)))
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Code"] = (int)value;
				string text = Function.ToString(value);
				dataRow["Name"] = Function.SpaceBeforeCapital(text.Substring(1, text.Length - 2));
				dataTable.Rows.Add(dataRow);
			}
			rep_ObjectType.SetDataSource(dataTable, "Name", "Code", emptyRow: true);
			rep_ColumnObjectType.SetDataSource(dataTable, "Name", "Code", emptyRow: true);
			look_Category.SetDataSource(CommonList.ReportCategory);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_QueryName_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.Queries, false, "", "", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			if (frm_CFL2.ChooseItem != null)
			{
				bte_QueryName.EditValue = Function.ToString(frm_CFL2.ChooseItem["QName"]);
				look_QueryType.EditValue = "T";
				txt_Query.Text = Function.ToString(frm_CFL2.ChooseItem["QString"]);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void wizardControlEx_ExtractQueryReport_NextClick(object sender, WizardCommandButtonClickEventArgs e)
	{
	}

	private async void wizardControlEx_ExtractQueryReport_SelectedPageChanged(object sender, WizardPageChangedEventArgs e)
	{
		_ = 2;
		try
		{
			switch (e.Page.Name)
			{
			case "wzp_SelectQuery":
				Text = "Extract Query Report - Select Query";
				break;
			case "wzp_Parmeters":
				Text = "Extract Query Report - Parmeters";
				await GetParamerters();
				await GetParamertersValidValue();
				break;
			case "wzp_Columns":
				Text = "Extract Query Report - Columns";
				await GetColumns();
				break;
			case "wzp_Subreport":
				Text = "Extract Query Report - Sub Report";
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task GetParamertersValidValue()
	{
	}

	private async Task GetColumns()
	{
		try
		{
			WinCommons._OpenProcessing();
			if (Function.ToString(look_QueryType.EditValue) == "S")
			{
				string text = Function.ToString(bte_QueryName.EditValue);
				if (!string.IsNullOrEmpty(text))
				{
					DataTable dataSourceTable = grd_Parameters.DataSourceTable;
					DataTable dataSource = await _httpClient.Queries.GetColumns(text, CommandType.StoredProcedure, dataSourceTable);
					grd_Columns.DataSource = dataSource;
					grd_Columns.RefreshDataSource();
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

	private async Task GetParamerters()
	{
		try
		{
			if (grd_Parameters.DataSourceTable.IsNotNull())
			{
				return;
			}
			WinCommons._OpenProcessing();
			if (Function.ToString(look_QueryType.EditValue) == "S")
			{
				string text = Function.ToString(bte_QueryName.EditValue);
				if (!string.IsNullOrEmpty(text))
				{
					DataTable dataSource = await _httpClient.Queries.GetParameters(text);
					grd_Parameters.DataSource = dataSource;
					grd_Parameters.RefreshDataSource();
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private void look_QueryType_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (Function.ToString(look_QueryType.EditValue) == "S")
			{
				txt_Query.Text = Function.ToString(bte_QueryName.EditValue);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_QueryName_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(Function.ToString(bte_QueryName.EditValue)))
			{
				txt_Query.Text = string.Empty;
			}
			if (string.IsNullOrEmpty(txt_RptName.Text))
			{
				txt_RptName.Text = Function.ToTitleCase(Function.ToString(bte_QueryName.EditValue).Replace("_", " "));
			}
			if (Function.ToString(look_QueryType.EditValue) == "S")
			{
				txt_Query.Text = Function.ToString(bte_QueryName.EditValue);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void wizardControlEx_ExtractQueryReport_FinishClick(object sender, CancelEventArgs e)
	{
		try
		{
			DataSet dataSet = new DataSet();
			DataTable dataSelectQuery = GetDataSelectQuery();
			dataSet.Tables.Add(dataSelectQuery);
			DataTable dataParameters = GetDataParameters();
			dataSet.Tables.Add(dataParameters.Copy());
			DataTable validValues = GetValidValues();
			dataSet.Tables.Add(validValues.Copy());
			DataTable subReport = GetSubReport();
			dataSet.Tables.Add(subReport.Copy());
			DataTable subReportParamters = GetSubReportParamters();
			dataSet.Tables.Add(subReportParamters.Copy());
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) == DialogResult.Yes)
			{
				HttpResult httpResult = await _httpClient.Queries.SaveExtractQuery(dataSet);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.SuccessWithContent, "", Function.ToString(httpResult.Content));
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private DataTable GetSubReportParamters()
	{
		DataTable dataSourceTable = grd_Subreport_Parameters.DataSourceTable;
		dataSourceTable.TableName = "APZ_EQR4";
		return dataSourceTable;
	}

	private DataTable GetSubReport()
	{
		DataTable dataSourceTable = grd_Subreport.DataSourceTable;
		dataSourceTable.TableName = "APZ_EQR3";
		return dataSourceTable;
	}

	private DataTable GetValidValues()
	{
		DataTable dataSourceTable = grd_ValidValues.DataSourceTable;
		dataSourceTable.TableName = "APZ_EQR2";
		return dataSourceTable;
	}

	private DataTable GetDataParameters()
	{
		DataTable dataSourceTable = grd_Parameters.DataSourceTable;
		dataSourceTable.TableName = "APZ_EQR1";
		return dataSourceTable;
	}

	private DataTable GetDataSelectQuery()
	{
		DataTable dataTable = new DataTable("APZ_OEQR")
		{
			Columns = 
			{
				{
					"Id",
					typeof(int)
				},
				{
					"Category",
					typeof(string)
				},
				{
					"QueryId",
					typeof(int)
				},
				{
					"Qname",
					typeof(string)
				},
				{
					"Qtype",
					typeof(string)
				},
				{
					"Qstring",
					typeof(string)
				},
				{
					"Name",
					typeof(string)
				},
				{
					"Dscription",
					typeof(string)
				}
			}
		};
		DataRow dataRow = dataTable.NewRow();
		dataRow["Id"] = Id;
		dataRow["Category"] = Function.ToString(look_Category.EditValue);
		dataRow["QueryId"] = QueryId;
		dataRow["Qname"] = Function.ToString(bte_QueryName.EditValue);
		dataRow["Qtype"] = Function.ToString(look_QueryType.EditValue);
		dataRow["Qstring"] = Function.ToString(txt_Query.Text);
		dataRow["Name"] = Function.ToString(txt_RptName.EditValue);
		dataRow["Dscription"] = Function.ToString(txt_RptDesc.EditValue);
		dataTable.Rows.Add(dataRow);
		return dataTable;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.frm_ExtractQueryReport));
		this.gridView4 = new Apzon.Controls.Grid.GridViewEx();
		this.grd_Parameters = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Parameters = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn2 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn3 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn4 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_DbType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn5 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn6 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_IsNullable = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn7 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_isApply = new DevExpress.XtraEditors.Repository.RepositoryItemCheckEdit();
		this.gridColumn8 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SourceType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn9 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Query = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn10 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn11 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ObjectType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridView5 = new Apzon.Controls.Grid.GridViewEx();
		this.wzp_Columns = new DevExpress.XtraWizard.WizardPage();
		this.grd_Columns = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Columns = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn16 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn17 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn18 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_ColumnObjectType = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn19 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn20 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn21 = new Apzon.Controls.Grid.GridColumnEx();
		this.wzp_Parmeters = new DevExpress.XtraWizard.WizardPage();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.xtraTabControl1 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage1 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_ValidValues = new Apzon.Controls.Grid.GridControlEx();
		this.gv_ValidValues = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn12 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn13 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn14 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn15 = new Apzon.Controls.Grid.GridColumnEx();
		this.wzp_SelectQuery = new DevExpress.XtraWizard.WizardPage();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Query = new DevExpress.XtraRichEdit.RichEditControl();
		this.look_QueryType = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.look_Category = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.txt_RptDesc = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_RptName = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.bte_QueryName = new Apzon.Controls.Editor.ButtonEditEx();
		this.wizardControlEx_ExtractQueryReport = new Apzon.Controls.Controls.WizardControlEx();
		this.wzp_Subreport = new DevExpress.XtraWizard.WizardPage();
		this.splitContainerControl2 = new DevExpress.XtraEditors.SplitContainerControl();
		this.grd_Subreport = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Subreport = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn22 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn23 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_QueryReport = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.gridColumn24 = new Apzon.Controls.Grid.GridColumnEx();
		this.xtraTabControl2 = new Apzon.Controls.Controls.XtraTabControlEx();
		this.xtraTabPage2 = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Subreport_Parameters = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Subreport_Paramaters = new Apzon.Controls.Grid.GridViewEx();
		this.gridColumn25 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn26 = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn27 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_SubQueryParameters = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.gridColumn28 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_Columns = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		((System.ComponentModel.ISupportInitialize)this.gridView4).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Parameters).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Parameters).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DbType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_IsNullable).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_isApply).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Query).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ObjectType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView5).BeginInit();
		this.wzp_Columns.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Columns).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Columns).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ColumnObjectType).BeginInit();
		this.wzp_Parmeters.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).BeginInit();
		this.xtraTabControl1.SuspendLayout();
		this.xtraTabPage1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_ValidValues).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ValidValues).BeginInit();
		this.wzp_SelectQuery.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.look_QueryType.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Category.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_RptDesc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_RptName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_QueryName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.wizardControlEx_ExtractQueryReport).BeginInit();
		this.wizardControlEx_ExtractQueryReport.SuspendLayout();
		this.wzp_Subreport.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).BeginInit();
		this.splitContainerControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Subreport).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Subreport).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_QueryReport).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).BeginInit();
		this.xtraTabControl2.SuspendLayout();
		this.xtraTabPage2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Subreport_Parameters).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Subreport_Paramaters).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SubQueryParameters).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Columns).BeginInit();
		base.SuspendLayout();
		this.gridView4.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView4.ColumnKey = null;
		this.gridView4.DisableFormSetting = false;
		this.gridView4.DisplayException = false;
		this.gridView4.EditColumn = null;
		this.gridView4.FieldCurrency = null;
		this.gridView4.FieldRate = null;
		this.gridView4.FormId = "oExtractQueryReport";
		this.gridView4.GridControl = this.grd_Parameters;
		this.gridView4.IgnoreFilter = false;
		this.gridView4.IndexEditRow = -1;
		this.gridView4.IndicatorWidth = 35;
		this.gridView4.IsApzTable = false;
		this.gridView4.IsInsertNewRow = false;
		this.gridView4.IsLoadingData = false;
		this.gridView4.IsMappingContextAction = false;
		this.gridView4.ItemId = "OGRID4";
		this.gridView4.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gridView4.LstColumnAllowBatch = null;
		this.gridView4.LstColumnNotAllowBatch = null;
		this.gridView4.LstColumnNotEditable = null;
		this.gridView4.LstColumNotSetting = null;
		this.gridView4.Name = "gridView4";
		this.gridView4.UseBatchMod = false;
		this.grd_Parameters.AllowPasteExcel = false;
		this.grd_Parameters.ClipboardData = null;
		this.grd_Parameters.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Parameters.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Parameters.Location = new System.Drawing.Point(0, 0);
		this.grd_Parameters.MainView = this.gv_Parameters;
		this.grd_Parameters.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Parameters.Name = "grd_Parameters";
		this.grd_Parameters.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.rep_DbType, this.rep_IsNullable, this.rep_isApply, this.rep_SourceType, this.rep_Query, this.rep_ObjectType });
		this.grd_Parameters.Size = new System.Drawing.Size(544, 245);
		this.grd_Parameters.TabIndex = 0;
		this.grd_Parameters.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[3] { this.gv_Parameters, this.gridView5, this.gridView4 });
		this.gv_Parameters.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Parameters.ColumnKey = null;
		this.gv_Parameters.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.gridColumn1, this.gridColumn2, this.gridColumn3, this.gridColumn4, this.gridColumn5, this.gridColumn6, this.gridColumn7, this.gridColumn8, this.gridColumn9, this.gridColumn10,
			this.gridColumn11
		});
		this.gv_Parameters.DisableFormSetting = false;
		this.gv_Parameters.DisplayException = false;
		this.gv_Parameters.EditColumn = null;
		this.gv_Parameters.FieldCurrency = null;
		this.gv_Parameters.FieldRate = null;
		this.gv_Parameters.FormId = "oExtractQueryReport";
		this.gv_Parameters.GridControl = this.grd_Parameters;
		this.gv_Parameters.IgnoreFilter = false;
		this.gv_Parameters.IndexEditRow = -1;
		this.gv_Parameters.IndicatorWidth = 35;
		this.gv_Parameters.IsApzTable = false;
		this.gv_Parameters.IsInsertNewRow = false;
		this.gv_Parameters.IsLoadingData = false;
		this.gv_Parameters.IsMappingContextAction = false;
		this.gv_Parameters.ItemId = "OPARAMATER";
		this.gv_Parameters.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Parameters.LstColumnAllowBatch = null;
		this.gv_Parameters.LstColumnNotAllowBatch = null;
		this.gv_Parameters.LstColumnNotEditable = null;
		this.gv_Parameters.LstColumNotSetting = null;
		this.gv_Parameters.Name = "gv_Parameters";
		this.gv_Parameters.OptionsView.ColumnAutoWidth = false;
		this.gv_Parameters.OptionsView.ShowGroupPanel = false;
		this.gv_Parameters.UseBatchMod = false;
		this.gridColumn1.Caption = "ID";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "Id";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.gridColumn1.Width = 100;
		this.gridColumn2.Caption = "Name";
		this.gridColumn2.CurrencyDateField = null;
		this.gridColumn2.CurrencyField = null;
		this.gridColumn2.CurrencyFixed = null;
		this.gridColumn2.CurrencyFixedRate = 0.0;
		this.gridColumn2.CurrencyRateField = null;
		this.gridColumn2.FieldName = "Name";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.PrimaryKey = false;
		this.gridColumn2.UnitField = null;
		this.gridColumn2.Visible = true;
		this.gridColumn2.VisibleIndex = 0;
		this.gridColumn2.Width = 100;
		this.gridColumn3.Caption = "Caption";
		this.gridColumn3.CurrencyDateField = null;
		this.gridColumn3.CurrencyField = null;
		this.gridColumn3.CurrencyFixed = null;
		this.gridColumn3.CurrencyFixedRate = 0.0;
		this.gridColumn3.CurrencyRateField = null;
		this.gridColumn3.FieldName = "Caption";
		this.gridColumn3.Name = "gridColumn3";
		this.gridColumn3.PrimaryKey = false;
		this.gridColumn3.UnitField = null;
		this.gridColumn3.Visible = true;
		this.gridColumn3.VisibleIndex = 1;
		this.gridColumn3.Width = 100;
		this.gridColumn4.Caption = "DB Type";
		this.gridColumn4.ColumnEdit = this.rep_DbType;
		this.gridColumn4.CurrencyDateField = null;
		this.gridColumn4.CurrencyField = null;
		this.gridColumn4.CurrencyFixed = null;
		this.gridColumn4.CurrencyFixedRate = 0.0;
		this.gridColumn4.CurrencyRateField = null;
		this.gridColumn4.FieldName = "DbType";
		this.gridColumn4.Name = "gridColumn4";
		this.gridColumn4.PrimaryKey = false;
		this.gridColumn4.UnitField = null;
		this.gridColumn4.Visible = true;
		this.gridColumn4.VisibleIndex = 2;
		this.gridColumn4.Width = 100;
		this.rep_DbType.AutoHeight = false;
		this.rep_DbType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_DbType.Name = "rep_DbType";
		this.rep_DbType.NullText = "";
		this.gridColumn5.Caption = "Size";
		this.gridColumn5.CurrencyDateField = null;
		this.gridColumn5.CurrencyField = null;
		this.gridColumn5.CurrencyFixed = null;
		this.gridColumn5.CurrencyFixedRate = 0.0;
		this.gridColumn5.CurrencyRateField = null;
		this.gridColumn5.FieldName = "Size";
		this.gridColumn5.Name = "gridColumn5";
		this.gridColumn5.PrimaryKey = false;
		this.gridColumn5.UnitField = null;
		this.gridColumn5.Visible = true;
		this.gridColumn5.VisibleIndex = 3;
		this.gridColumn5.Width = 100;
		this.gridColumn6.Caption = "Nullable";
		this.gridColumn6.ColumnEdit = this.rep_IsNullable;
		this.gridColumn6.CurrencyDateField = null;
		this.gridColumn6.CurrencyField = null;
		this.gridColumn6.CurrencyFixed = null;
		this.gridColumn6.CurrencyFixedRate = 0.0;
		this.gridColumn6.CurrencyRateField = null;
		this.gridColumn6.FieldName = "IsNullable";
		this.gridColumn6.Name = "gridColumn6";
		this.gridColumn6.PrimaryKey = false;
		this.gridColumn6.UnitField = null;
		this.gridColumn6.Visible = true;
		this.gridColumn6.VisibleIndex = 4;
		this.gridColumn6.Width = 100;
		this.rep_IsNullable.AutoHeight = false;
		this.rep_IsNullable.Name = "rep_IsNullable";
		this.rep_IsNullable.ValueChecked = "Y";
		this.rep_IsNullable.ValueGrayed = "N";
		this.rep_IsNullable.ValueUnchecked = "N";
		this.gridColumn7.Caption = "Apply";
		this.gridColumn7.ColumnEdit = this.rep_isApply;
		this.gridColumn7.CurrencyDateField = null;
		this.gridColumn7.CurrencyField = null;
		this.gridColumn7.CurrencyFixed = null;
		this.gridColumn7.CurrencyFixedRate = 0.0;
		this.gridColumn7.CurrencyRateField = null;
		this.gridColumn7.FieldName = "IsApply";
		this.gridColumn7.Name = "gridColumn7";
		this.gridColumn7.PrimaryKey = false;
		this.gridColumn7.UnitField = null;
		this.gridColumn7.Visible = true;
		this.gridColumn7.VisibleIndex = 5;
		this.gridColumn7.Width = 100;
		this.rep_isApply.AutoHeight = false;
		this.rep_isApply.Name = "rep_isApply";
		this.rep_isApply.ValueChecked = "Y";
		this.rep_isApply.ValueGrayed = "N";
		this.rep_isApply.ValueUnchecked = "N";
		this.gridColumn8.Caption = "Source Type";
		this.gridColumn8.ColumnEdit = this.rep_SourceType;
		this.gridColumn8.CurrencyDateField = null;
		this.gridColumn8.CurrencyField = null;
		this.gridColumn8.CurrencyFixed = null;
		this.gridColumn8.CurrencyFixedRate = 0.0;
		this.gridColumn8.CurrencyRateField = null;
		this.gridColumn8.FieldName = "SourceType";
		this.gridColumn8.Name = "gridColumn8";
		this.gridColumn8.PrimaryKey = false;
		this.gridColumn8.UnitField = null;
		this.gridColumn8.Visible = true;
		this.gridColumn8.VisibleIndex = 6;
		this.gridColumn8.Width = 100;
		this.rep_SourceType.AutoHeight = false;
		this.rep_SourceType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SourceType.Name = "rep_SourceType";
		this.rep_SourceType.NullText = "";
		this.gridColumn9.Caption = "Query";
		this.gridColumn9.ColumnEdit = this.rep_Query;
		this.gridColumn9.CurrencyDateField = null;
		this.gridColumn9.CurrencyField = null;
		this.gridColumn9.CurrencyFixed = null;
		this.gridColumn9.CurrencyFixedRate = 0.0;
		this.gridColumn9.CurrencyRateField = null;
		this.gridColumn9.FieldName = "Query";
		this.gridColumn9.Name = "gridColumn9";
		this.gridColumn9.PrimaryKey = false;
		this.gridColumn9.UnitField = null;
		this.gridColumn9.Visible = true;
		this.gridColumn9.VisibleIndex = 7;
		this.gridColumn9.Width = 100;
		this.rep_Query.AutoHeight = false;
		this.rep_Query.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.rep_Query.Name = "rep_Query";
		this.gridColumn10.Caption = "Selected Field";
		this.gridColumn10.CurrencyDateField = null;
		this.gridColumn10.CurrencyField = null;
		this.gridColumn10.CurrencyFixed = null;
		this.gridColumn10.CurrencyFixedRate = 0.0;
		this.gridColumn10.CurrencyRateField = null;
		this.gridColumn10.FieldName = "SelectedField";
		this.gridColumn10.Name = "gridColumn10";
		this.gridColumn10.PrimaryKey = false;
		this.gridColumn10.UnitField = null;
		this.gridColumn10.Visible = true;
		this.gridColumn10.VisibleIndex = 8;
		this.gridColumn10.Width = 100;
		this.gridColumn11.Caption = "Object Type";
		this.gridColumn11.ColumnEdit = this.rep_ObjectType;
		this.gridColumn11.CurrencyDateField = null;
		this.gridColumn11.CurrencyField = null;
		this.gridColumn11.CurrencyFixed = null;
		this.gridColumn11.CurrencyFixedRate = 0.0;
		this.gridColumn11.CurrencyRateField = null;
		this.gridColumn11.FieldName = "ObjectType";
		this.gridColumn11.Name = "gridColumn11";
		this.gridColumn11.PrimaryKey = false;
		this.gridColumn11.UnitField = null;
		this.gridColumn11.Visible = true;
		this.gridColumn11.VisibleIndex = 9;
		this.gridColumn11.Width = 100;
		this.rep_ObjectType.AutoHeight = false;
		this.rep_ObjectType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ObjectType.Name = "rep_ObjectType";
		this.rep_ObjectType.NullText = "";
		this.gridView5.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView5.ColumnKey = null;
		this.gridView5.DisableFormSetting = false;
		this.gridView5.DisplayException = false;
		this.gridView5.EditColumn = null;
		this.gridView5.FieldCurrency = null;
		this.gridView5.FieldRate = null;
		this.gridView5.FormId = "oExtractQueryReport";
		this.gridView5.GridControl = this.grd_Parameters;
		this.gridView5.IgnoreFilter = false;
		this.gridView5.IndexEditRow = -1;
		this.gridView5.IndicatorWidth = 35;
		this.gridView5.IsApzTable = false;
		this.gridView5.IsInsertNewRow = false;
		this.gridView5.IsLoadingData = false;
		this.gridView5.IsMappingContextAction = false;
		this.gridView5.ItemId = "OGRID5";
		this.gridView5.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gridView5.LstColumnAllowBatch = null;
		this.gridView5.LstColumnNotAllowBatch = null;
		this.gridView5.LstColumnNotEditable = null;
		this.gridView5.LstColumNotSetting = null;
		this.gridView5.Name = "gridView5";
		this.gridView5.UseBatchMod = false;
		this.wzp_Columns.Controls.Add(this.grd_Columns);
		this.wzp_Columns.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.wzp_Columns.Name = "wzp_Columns";
		this.wzp_Columns.Size = new System.Drawing.Size(544, 420);
		this.grd_Columns.AllowPasteExcel = false;
		this.grd_Columns.ClipboardData = null;
		this.grd_Columns.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Columns.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Columns.Location = new System.Drawing.Point(0, 0);
		this.grd_Columns.MainView = this.gv_Columns;
		this.grd_Columns.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Columns.Name = "grd_Columns";
		this.grd_Columns.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_ColumnObjectType });
		this.grd_Columns.Size = new System.Drawing.Size(544, 420);
		this.grd_Columns.TabIndex = 0;
		this.grd_Columns.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Columns });
		this.gv_Columns.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Columns.ColumnKey = null;
		this.gv_Columns.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[6] { this.gridColumn16, this.gridColumn17, this.gridColumn18, this.gridColumn19, this.gridColumn20, this.gridColumn21 });
		this.gv_Columns.DisableFormSetting = false;
		this.gv_Columns.DisplayException = false;
		this.gv_Columns.EditColumn = null;
		this.gv_Columns.FieldCurrency = null;
		this.gv_Columns.FieldRate = null;
		this.gv_Columns.FormId = "oExtractQueryReport";
		this.gv_Columns.GridControl = this.grd_Columns;
		this.gv_Columns.IgnoreFilter = false;
		this.gv_Columns.IndexEditRow = -1;
		this.gv_Columns.IndicatorWidth = 35;
		this.gv_Columns.IsApzTable = false;
		this.gv_Columns.IsInsertNewRow = false;
		this.gv_Columns.IsLoadingData = false;
		this.gv_Columns.IsMappingContextAction = false;
		this.gv_Columns.ItemId = "OCOLUMN";
		this.gv_Columns.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Columns.LstColumnAllowBatch = null;
		this.gv_Columns.LstColumnNotAllowBatch = null;
		this.gv_Columns.LstColumnNotEditable = null;
		this.gv_Columns.LstColumNotSetting = null;
		this.gv_Columns.Name = "gv_Columns";
		this.gv_Columns.UseBatchMod = false;
		this.gridColumn16.Caption = "Column Id";
		this.gridColumn16.CurrencyDateField = null;
		this.gridColumn16.CurrencyField = null;
		this.gridColumn16.CurrencyFixed = null;
		this.gridColumn16.CurrencyFixedRate = 0.0;
		this.gridColumn16.CurrencyRateField = null;
		this.gridColumn16.FieldName = "ColumnId";
		this.gridColumn16.Name = "gridColumn16";
		this.gridColumn16.PrimaryKey = false;
		this.gridColumn16.UnitField = null;
		this.gridColumn16.Visible = true;
		this.gridColumn16.VisibleIndex = 0;
		this.gridColumn16.Width = 84;
		this.gridColumn17.Caption = "Caption";
		this.gridColumn17.CurrencyDateField = null;
		this.gridColumn17.CurrencyField = null;
		this.gridColumn17.CurrencyFixed = null;
		this.gridColumn17.CurrencyFixedRate = 0.0;
		this.gridColumn17.CurrencyRateField = null;
		this.gridColumn17.FieldName = "Caption";
		this.gridColumn17.Name = "gridColumn17";
		this.gridColumn17.PrimaryKey = false;
		this.gridColumn17.UnitField = null;
		this.gridColumn17.Visible = true;
		this.gridColumn17.VisibleIndex = 1;
		this.gridColumn17.Width = 84;
		this.gridColumn18.Caption = "Object Type";
		this.gridColumn18.ColumnEdit = this.rep_ColumnObjectType;
		this.gridColumn18.CurrencyDateField = null;
		this.gridColumn18.CurrencyField = null;
		this.gridColumn18.CurrencyFixed = null;
		this.gridColumn18.CurrencyFixedRate = 0.0;
		this.gridColumn18.CurrencyRateField = null;
		this.gridColumn18.FieldName = "ObjectType";
		this.gridColumn18.Name = "gridColumn18";
		this.gridColumn18.PrimaryKey = false;
		this.gridColumn18.UnitField = null;
		this.gridColumn18.Visible = true;
		this.gridColumn18.VisibleIndex = 2;
		this.gridColumn18.Width = 84;
		this.rep_ColumnObjectType.AutoHeight = false;
		this.rep_ColumnObjectType.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_ColumnObjectType.Name = "rep_ColumnObjectType";
		this.gridColumn19.Caption = "Display Format Type";
		this.gridColumn19.CurrencyDateField = null;
		this.gridColumn19.CurrencyField = null;
		this.gridColumn19.CurrencyFixed = null;
		this.gridColumn19.CurrencyFixedRate = 0.0;
		this.gridColumn19.CurrencyRateField = null;
		this.gridColumn19.FieldName = "DisplayFormat";
		this.gridColumn19.Name = "gridColumn19";
		this.gridColumn19.PrimaryKey = false;
		this.gridColumn19.UnitField = null;
		this.gridColumn19.Visible = true;
		this.gridColumn19.VisibleIndex = 3;
		this.gridColumn19.Width = 112;
		this.gridColumn20.Caption = "Visible";
		this.gridColumn20.CurrencyDateField = null;
		this.gridColumn20.CurrencyField = null;
		this.gridColumn20.CurrencyFixed = null;
		this.gridColumn20.CurrencyFixedRate = 0.0;
		this.gridColumn20.CurrencyRateField = null;
		this.gridColumn20.FieldName = "Visible";
		this.gridColumn20.Name = "gridColumn20";
		this.gridColumn20.PrimaryKey = false;
		this.gridColumn20.UnitField = null;
		this.gridColumn20.Visible = true;
		this.gridColumn20.VisibleIndex = 4;
		this.gridColumn20.Width = 70;
		this.gridColumn21.Caption = "Fixed";
		this.gridColumn21.CurrencyDateField = null;
		this.gridColumn21.CurrencyField = null;
		this.gridColumn21.CurrencyFixed = null;
		this.gridColumn21.CurrencyFixedRate = 0.0;
		this.gridColumn21.CurrencyRateField = null;
		this.gridColumn21.FieldName = "Fixed";
		this.gridColumn21.Name = "gridColumn21";
		this.gridColumn21.PrimaryKey = false;
		this.gridColumn21.UnitField = null;
		this.gridColumn21.Visible = true;
		this.gridColumn21.VisibleIndex = 5;
		this.gridColumn21.Width = 73;
		this.wzp_Parmeters.Controls.Add(this.splitContainerControl1);
		this.wzp_Parmeters.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.wzp_Parmeters.Name = "wzp_Parmeters";
		this.wzp_Parmeters.Size = new System.Drawing.Size(544, 420);
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.grd_Parameters);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.xtraTabControl1);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(544, 420);
		this.splitContainerControl1.SplitterPosition = 245;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.xtraTabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl1.IsSearchControl = false;
		this.xtraTabControl1.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xtraTabControl1.Name = "xtraTabControl1";
		this.xtraTabControl1.SelectedTabPage = this.xtraTabPage1;
		this.xtraTabControl1.Size = new System.Drawing.Size(544, 165);
		this.xtraTabControl1.TabIndex = 0;
		this.xtraTabControl1.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage1 });
		this.xtraTabPage1.Controls.Add(this.grd_ValidValues);
		this.xtraTabPage1.EditControl = null;
		this.xtraTabPage1.EditValue = "Valid Values";
		this.xtraTabPage1.FieldName = null;
		this.xtraTabPage1.IsSearchControl = false;
		this.xtraTabPage1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xtraTabPage1.Name = "xtraTabPage1";
		this.xtraTabPage1.ReadOnly = false;
		this.xtraTabPage1.SettingInVisible = false;
		this.xtraTabPage1.SettingReadonly = false;
		this.xtraTabPage1.Size = new System.Drawing.Size(542, 140);
		this.xtraTabPage1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage1.Text = "Valid Values";
		this.grd_ValidValues.AllowPasteExcel = false;
		this.grd_ValidValues.ClipboardData = null;
		this.grd_ValidValues.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_ValidValues.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_ValidValues.Location = new System.Drawing.Point(0, 0);
		this.grd_ValidValues.MainView = this.gv_ValidValues;
		this.grd_ValidValues.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_ValidValues.Name = "grd_ValidValues";
		this.grd_ValidValues.Size = new System.Drawing.Size(542, 140);
		this.grd_ValidValues.TabIndex = 1;
		this.grd_ValidValues.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_ValidValues });
		this.gv_ValidValues.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_ValidValues.ColumnKey = null;
		this.gv_ValidValues.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumn12, this.gridColumn13, this.gridColumn14, this.gridColumn15 });
		this.gv_ValidValues.DisableFormSetting = false;
		this.gv_ValidValues.DisplayException = false;
		this.gv_ValidValues.EditColumn = null;
		this.gv_ValidValues.FieldCurrency = null;
		this.gv_ValidValues.FieldRate = null;
		this.gv_ValidValues.FormId = "oExtractQueryReport";
		this.gv_ValidValues.GridControl = this.grd_ValidValues;
		this.gv_ValidValues.IgnoreFilter = false;
		this.gv_ValidValues.IndexEditRow = -1;
		this.gv_ValidValues.IndicatorWidth = 35;
		this.gv_ValidValues.IsApzTable = false;
		this.gv_ValidValues.IsInsertNewRow = false;
		this.gv_ValidValues.IsLoadingData = false;
		this.gv_ValidValues.IsMappingContextAction = false;
		this.gv_ValidValues.ItemId = "OVALIDVALUE";
		this.gv_ValidValues.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_ValidValues.LstColumnAllowBatch = null;
		this.gv_ValidValues.LstColumnNotAllowBatch = null;
		this.gv_ValidValues.LstColumnNotEditable = null;
		this.gv_ValidValues.LstColumNotSetting = null;
		this.gv_ValidValues.Name = "gv_ValidValues";
		this.gv_ValidValues.OptionsView.ShowGroupPanel = false;
		this.gv_ValidValues.UseBatchMod = false;
		this.gridColumn12.Caption = "Id";
		this.gridColumn12.CurrencyDateField = null;
		this.gridColumn12.CurrencyField = null;
		this.gridColumn12.CurrencyFixed = null;
		this.gridColumn12.CurrencyFixedRate = 0.0;
		this.gridColumn12.CurrencyRateField = null;
		this.gridColumn12.FieldName = "Id";
		this.gridColumn12.Name = "gridColumn12";
		this.gridColumn12.PrimaryKey = false;
		this.gridColumn12.UnitField = null;
		this.gridColumn13.Caption = "Name";
		this.gridColumn13.CurrencyDateField = null;
		this.gridColumn13.CurrencyField = null;
		this.gridColumn13.CurrencyFixed = null;
		this.gridColumn13.CurrencyFixedRate = 0.0;
		this.gridColumn13.CurrencyRateField = null;
		this.gridColumn13.FieldName = "Name";
		this.gridColumn13.Name = "gridColumn13";
		this.gridColumn13.PrimaryKey = false;
		this.gridColumn13.UnitField = null;
		this.gridColumn14.Caption = "Value";
		this.gridColumn14.CurrencyDateField = null;
		this.gridColumn14.CurrencyField = null;
		this.gridColumn14.CurrencyFixed = null;
		this.gridColumn14.CurrencyFixedRate = 0.0;
		this.gridColumn14.CurrencyRateField = null;
		this.gridColumn14.FieldName = "Value";
		this.gridColumn14.Name = "gridColumn14";
		this.gridColumn14.PrimaryKey = false;
		this.gridColumn14.UnitField = null;
		this.gridColumn14.Visible = true;
		this.gridColumn14.VisibleIndex = 0;
		this.gridColumn15.Caption = "Description";
		this.gridColumn15.CurrencyDateField = null;
		this.gridColumn15.CurrencyField = null;
		this.gridColumn15.CurrencyFixed = null;
		this.gridColumn15.CurrencyFixedRate = 0.0;
		this.gridColumn15.CurrencyRateField = null;
		this.gridColumn15.FieldName = "Dscription";
		this.gridColumn15.Name = "gridColumn15";
		this.gridColumn15.PrimaryKey = false;
		this.gridColumn15.UnitField = null;
		this.gridColumn15.Visible = true;
		this.gridColumn15.VisibleIndex = 1;
		this.wzp_SelectQuery.Controls.Add(this.labelLine6);
		this.wzp_SelectQuery.Controls.Add(this.txt_Query);
		this.wzp_SelectQuery.Controls.Add(this.look_QueryType);
		this.wzp_SelectQuery.Controls.Add(this.labelLine5);
		this.wzp_SelectQuery.Controls.Add(this.labelLine4);
		this.wzp_SelectQuery.Controls.Add(this.look_Category);
		this.wzp_SelectQuery.Controls.Add(this.labelLine3);
		this.wzp_SelectQuery.Controls.Add(this.txt_RptDesc);
		this.wzp_SelectQuery.Controls.Add(this.labelLine2);
		this.wzp_SelectQuery.Controls.Add(this.txt_RptName);
		this.wzp_SelectQuery.Controls.Add(this.labelLine1);
		this.wzp_SelectQuery.Controls.Add(this.bte_QueryName);
		this.wzp_SelectQuery.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.wzp_SelectQuery.Name = "wzp_SelectQuery";
		this.wzp_SelectQuery.Size = new System.Drawing.Size(544, 420);
		this.wzp_SelectQuery.Text = "Select Query";
		this.labelLine6.EditControl = null;
		this.labelLine6.EditValue = "Query Text";
		this.labelLine6.FieldName = null;
		this.labelLine6.HideWhenEmpty = false;
		this.labelLine6.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine6.Location = new System.Drawing.Point(3, 64);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.ReadOnly = false;
		this.labelLine6.SettingInVisible = false;
		this.labelLine6.SettingReadonly = false;
		this.labelLine6.ShowUnderLine = true;
		this.labelLine6.Size = new System.Drawing.Size(103, 20);
		this.labelLine6.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine6.TabIndex = 11;
		this.labelLine6.Utext = "Query Text";
		this.txt_Query.ActiveViewType = DevExpress.XtraRichEdit.RichEditViewType.Draft;
		this.txt_Query.LayoutUnit = DevExpress.XtraRichEdit.DocumentLayoutUnit.Pixel;
		this.txt_Query.Location = new System.Drawing.Point(105, 62);
		this.txt_Query.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.txt_Query.Name = "txt_Query";
		this.txt_Query.Options.HorizontalRuler.Visibility = DevExpress.XtraRichEdit.RichEditRulerVisibility.Hidden;
		this.txt_Query.Size = new System.Drawing.Size(420, 242);
		this.txt_Query.TabIndex = 2;
		this.look_QueryType.Location = new System.Drawing.Point(105, 36);
		this.look_QueryType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.look_QueryType.Name = "look_QueryType";
		this.look_QueryType.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_QueryType.Properties.NullText = "";
		this.look_QueryType.Size = new System.Drawing.Size(418, 20);
		this.look_QueryType.TabIndex = 1;
		this.look_QueryType.EditValueChanged += new System.EventHandler(this.look_QueryType_EditValueChanged);
		this.labelLine5.EditControl = null;
		this.labelLine5.EditValue = "Query Type";
		this.labelLine5.FieldName = null;
		this.labelLine5.HideWhenEmpty = false;
		this.labelLine5.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine5.Location = new System.Drawing.Point(3, 37);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.ReadOnly = false;
		this.labelLine5.SettingInVisible = false;
		this.labelLine5.SettingReadonly = false;
		this.labelLine5.ShowUnderLine = true;
		this.labelLine5.Size = new System.Drawing.Size(103, 20);
		this.labelLine5.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine5.TabIndex = 8;
		this.labelLine5.Utext = "Query Type";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Query Name";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(3, 10);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(103, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 6;
		this.labelLine4.Utext = "Query Name";
		this.look_Category.Location = new System.Drawing.Point(105, 389);
		this.look_Category.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.look_Category.Name = "look_Category";
		this.look_Category.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Category.Properties.NullText = "";
		this.look_Category.Size = new System.Drawing.Size(418, 20);
		this.look_Category.TabIndex = 5;
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Category";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(3, 391);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(103, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 4;
		this.labelLine3.Utext = "Category";
		this.txt_RptDesc.EditControl = null;
		this.txt_RptDesc.FieldName = null;
		this.txt_RptDesc.IsSearchControl = false;
		this.txt_RptDesc.Location = new System.Drawing.Point(105, 364);
		this.txt_RptDesc.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.txt_RptDesc.Name = "txt_RptDesc";
		this.txt_RptDesc.SettingInVisible = false;
		this.txt_RptDesc.SettingReadonly = false;
		this.txt_RptDesc.Size = new System.Drawing.Size(418, 20);
		this.txt_RptDesc.TabIndex = 4;
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Report Description";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(3, 365);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(103, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "Report Description";
		this.txt_RptName.EditControl = null;
		this.txt_RptName.FieldName = null;
		this.txt_RptName.IsSearchControl = false;
		this.txt_RptName.Location = new System.Drawing.Point(105, 340);
		this.txt_RptName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.txt_RptName.Name = "txt_RptName";
		this.txt_RptName.SettingInVisible = false;
		this.txt_RptName.SettingReadonly = false;
		this.txt_RptName.Size = new System.Drawing.Size(418, 20);
		this.txt_RptName.TabIndex = 3;
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Report Name";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(3, 342);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(103, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Report Name";
		this.bte_QueryName.EditControl = null;
		this.bte_QueryName.FieldName = null;
		this.bte_QueryName.IsSearchControl = false;
		this.bte_QueryName.Location = new System.Drawing.Point(105, 9);
		this.bte_QueryName.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.bte_QueryName.Name = "bte_QueryName";
		this.bte_QueryName.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_QueryName.SettingInVisible = false;
		this.bte_QueryName.SettingReadonly = false;
		this.bte_QueryName.Size = new System.Drawing.Size(418, 20);
		this.bte_QueryName.TabIndex = 0;
		this.bte_QueryName.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_QueryName_ButtonClick);
		this.bte_QueryName.EditValueChanged += new System.EventHandler(this.bte_QueryName_EditValueChanged);
		this.wizardControlEx_ExtractQueryReport.AllowPagePadding = false;
		this.wizardControlEx_ExtractQueryReport.Controls.Add(this.wzp_SelectQuery);
		this.wizardControlEx_ExtractQueryReport.Controls.Add(this.wzp_Parmeters);
		this.wizardControlEx_ExtractQueryReport.Controls.Add(this.wzp_Columns);
		this.wizardControlEx_ExtractQueryReport.Controls.Add(this.wzp_Subreport);
		this.wizardControlEx_ExtractQueryReport.Dock = System.Windows.Forms.DockStyle.Fill;
		this.wizardControlEx_ExtractQueryReport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.wizardControlEx_ExtractQueryReport.MinimumSize = new System.Drawing.Size(75, 81);
		this.wizardControlEx_ExtractQueryReport.Name = "wizardControlEx_ExtractQueryReport";
		this.wizardControlEx_ExtractQueryReport.Pages.AddRange(new DevExpress.XtraWizard.BaseWizardPage[4] { this.wzp_SelectQuery, this.wzp_Parmeters, this.wzp_Columns, this.wzp_Subreport });
		this.wizardControlEx_ExtractQueryReport.Size = new System.Drawing.Size(544, 466);
		this.wizardControlEx_ExtractQueryReport.TitleHeight = 0;
		this.wizardControlEx_ExtractQueryReport.SelectedPageChanged += new DevExpress.XtraWizard.WizardPageChangedEventHandler(this.wizardControlEx_ExtractQueryReport_SelectedPageChanged);
		this.wizardControlEx_ExtractQueryReport.FinishClick += new System.ComponentModel.CancelEventHandler(this.wizardControlEx_ExtractQueryReport_FinishClick);
		this.wizardControlEx_ExtractQueryReport.NextClick += new DevExpress.XtraWizard.WizardCommandButtonClickEventHandler(this.wizardControlEx_ExtractQueryReport_NextClick);
		this.wzp_Subreport.Controls.Add(this.splitContainerControl2);
		this.wzp_Subreport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.wzp_Subreport.Name = "wzp_Subreport";
		this.wzp_Subreport.Size = new System.Drawing.Size(544, 420);
		this.splitContainerControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl2.Horizontal = false;
		this.splitContainerControl2.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.splitContainerControl2.Name = "splitContainerControl2";
		this.splitContainerControl2.Panel1.Controls.Add(this.grd_Subreport);
		this.splitContainerControl2.Panel1.Text = "Panel1";
		this.splitContainerControl2.Panel2.Controls.Add(this.xtraTabControl2);
		this.splitContainerControl2.Panel2.Text = "Panel2";
		this.splitContainerControl2.Size = new System.Drawing.Size(544, 420);
		this.splitContainerControl2.SplitterPosition = 200;
		this.splitContainerControl2.TabIndex = 0;
		this.splitContainerControl2.Text = "splitContainerControl2";
		this.grd_Subreport.AllowPasteExcel = false;
		this.grd_Subreport.ClipboardData = null;
		this.grd_Subreport.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Subreport.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Subreport.Location = new System.Drawing.Point(0, 0);
		this.grd_Subreport.MainView = this.gv_Subreport;
		this.grd_Subreport.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Subreport.Name = "grd_Subreport";
		this.grd_Subreport.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.rep_QueryReport });
		this.grd_Subreport.Size = new System.Drawing.Size(544, 200);
		this.grd_Subreport.TabIndex = 0;
		this.grd_Subreport.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Subreport });
		this.gv_Subreport.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Subreport.ColumnKey = null;
		this.gv_Subreport.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.gridColumn22, this.gridColumn23, this.gridColumn24 });
		this.gv_Subreport.DisableFormSetting = false;
		this.gv_Subreport.DisplayException = false;
		this.gv_Subreport.EditColumn = null;
		this.gv_Subreport.FieldCurrency = null;
		this.gv_Subreport.FieldRate = null;
		this.gv_Subreport.FormId = "oExtractQueryReport";
		this.gv_Subreport.GridControl = this.grd_Subreport;
		this.gv_Subreport.IgnoreFilter = false;
		this.gv_Subreport.IndexEditRow = -1;
		this.gv_Subreport.IndicatorWidth = 35;
		this.gv_Subreport.IsApzTable = false;
		this.gv_Subreport.IsInsertNewRow = false;
		this.gv_Subreport.IsLoadingData = false;
		this.gv_Subreport.IsMappingContextAction = false;
		this.gv_Subreport.ItemId = "OSUBREPORT";
		this.gv_Subreport.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Subreport.LstColumnAllowBatch = null;
		this.gv_Subreport.LstColumnNotAllowBatch = null;
		this.gv_Subreport.LstColumnNotEditable = null;
		this.gv_Subreport.LstColumNotSetting = null;
		this.gv_Subreport.Name = "gv_Subreport";
		this.gv_Subreport.OptionsView.ColumnAutoWidth = false;
		this.gv_Subreport.UseBatchMod = false;
		this.gridColumn22.Caption = "Id";
		this.gridColumn22.CurrencyDateField = null;
		this.gridColumn22.CurrencyField = null;
		this.gridColumn22.CurrencyFixed = null;
		this.gridColumn22.CurrencyFixedRate = 0.0;
		this.gridColumn22.CurrencyRateField = null;
		this.gridColumn22.FieldName = "Id";
		this.gridColumn22.Name = "gridColumn22";
		this.gridColumn22.PrimaryKey = false;
		this.gridColumn22.UnitField = null;
		this.gridColumn23.Caption = "Query Report";
		this.gridColumn23.ColumnEdit = this.rep_QueryReport;
		this.gridColumn23.CurrencyDateField = null;
		this.gridColumn23.CurrencyField = null;
		this.gridColumn23.CurrencyFixed = null;
		this.gridColumn23.CurrencyFixedRate = 0.0;
		this.gridColumn23.CurrencyRateField = null;
		this.gridColumn23.FieldName = "SubId";
		this.gridColumn23.Name = "gridColumn23";
		this.gridColumn23.PrimaryKey = false;
		this.gridColumn23.UnitField = null;
		this.gridColumn23.Visible = true;
		this.gridColumn23.VisibleIndex = 0;
		this.gridColumn23.Width = 100;
		this.rep_QueryReport.AutoHeight = false;
		this.rep_QueryReport.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton()
		});
		this.rep_QueryReport.Name = "rep_QueryReport";
		this.gridColumn24.Caption = "Column Id";
		this.gridColumn24.CurrencyDateField = null;
		this.gridColumn24.CurrencyField = null;
		this.gridColumn24.CurrencyFixed = null;
		this.gridColumn24.CurrencyFixedRate = 0.0;
		this.gridColumn24.CurrencyRateField = null;
		this.gridColumn24.FieldName = "ColumnId";
		this.gridColumn24.Name = "gridColumn24";
		this.gridColumn24.PrimaryKey = false;
		this.gridColumn24.UnitField = null;
		this.gridColumn24.Visible = true;
		this.gridColumn24.VisibleIndex = 1;
		this.gridColumn24.Width = 100;
		this.xtraTabControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.xtraTabControl2.IsSearchControl = false;
		this.xtraTabControl2.Location = new System.Drawing.Point(0, 0);
		this.xtraTabControl2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xtraTabControl2.Name = "xtraTabControl2";
		this.xtraTabControl2.SelectedTabPage = this.xtraTabPage2;
		this.xtraTabControl2.Size = new System.Drawing.Size(544, 210);
		this.xtraTabControl2.TabIndex = 0;
		this.xtraTabControl2.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[1] { this.xtraTabPage2 });
		this.xtraTabPage2.Controls.Add(this.grd_Subreport_Parameters);
		this.xtraTabPage2.EditControl = null;
		this.xtraTabPage2.EditValue = "Parameters";
		this.xtraTabPage2.FieldName = null;
		this.xtraTabPage2.IsSearchControl = false;
		this.xtraTabPage2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.xtraTabPage2.Name = "xtraTabPage2";
		this.xtraTabPage2.ReadOnly = false;
		this.xtraTabPage2.SettingInVisible = false;
		this.xtraTabPage2.SettingReadonly = false;
		this.xtraTabPage2.Size = new System.Drawing.Size(542, 185);
		this.xtraTabPage2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.xtraTabPage2.Text = "Parameters";
		this.grd_Subreport_Parameters.AllowPasteExcel = false;
		this.grd_Subreport_Parameters.ClipboardData = null;
		this.grd_Subreport_Parameters.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Subreport_Parameters.EmbeddedNavigator.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Subreport_Parameters.Location = new System.Drawing.Point(0, 0);
		this.grd_Subreport_Parameters.MainView = this.gv_Subreport_Paramaters;
		this.grd_Subreport_Parameters.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.grd_Subreport_Parameters.Name = "grd_Subreport_Parameters";
		this.grd_Subreport_Parameters.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.rep_SubQueryParameters, this.rep_Columns });
		this.grd_Subreport_Parameters.Size = new System.Drawing.Size(542, 185);
		this.grd_Subreport_Parameters.TabIndex = 1;
		this.grd_Subreport_Parameters.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Subreport_Paramaters });
		this.gv_Subreport_Paramaters.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Subreport_Paramaters.ColumnKey = null;
		this.gv_Subreport_Paramaters.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.gridColumn25, this.gridColumn26, this.gridColumn27, this.gridColumn28 });
		this.gv_Subreport_Paramaters.DisableFormSetting = false;
		this.gv_Subreport_Paramaters.DisplayException = false;
		this.gv_Subreport_Paramaters.EditColumn = null;
		this.gv_Subreport_Paramaters.FieldCurrency = null;
		this.gv_Subreport_Paramaters.FieldRate = null;
		this.gv_Subreport_Paramaters.FormId = "oExtractQueryReport";
		this.gv_Subreport_Paramaters.GridControl = this.grd_Subreport_Parameters;
		this.gv_Subreport_Paramaters.IgnoreFilter = false;
		this.gv_Subreport_Paramaters.IndexEditRow = -1;
		this.gv_Subreport_Paramaters.IndicatorWidth = 35;
		this.gv_Subreport_Paramaters.IsApzTable = false;
		this.gv_Subreport_Paramaters.IsInsertNewRow = false;
		this.gv_Subreport_Paramaters.IsLoadingData = false;
		this.gv_Subreport_Paramaters.IsMappingContextAction = false;
		this.gv_Subreport_Paramaters.ItemId = "OSUBPARAMATER";
		this.gv_Subreport_Paramaters.LanguageSource = Apzon.Commons.Enums.Enums.LanguageSource.Documents;
		this.gv_Subreport_Paramaters.LstColumnAllowBatch = null;
		this.gv_Subreport_Paramaters.LstColumnNotAllowBatch = null;
		this.gv_Subreport_Paramaters.LstColumnNotEditable = null;
		this.gv_Subreport_Paramaters.LstColumNotSetting = null;
		this.gv_Subreport_Paramaters.Name = "gv_Subreport_Paramaters";
		this.gv_Subreport_Paramaters.UseBatchMod = false;
		this.gridColumn25.Caption = "Id";
		this.gridColumn25.CurrencyDateField = null;
		this.gridColumn25.CurrencyField = null;
		this.gridColumn25.CurrencyFixed = null;
		this.gridColumn25.CurrencyFixedRate = 0.0;
		this.gridColumn25.CurrencyRateField = null;
		this.gridColumn25.FieldName = "Id";
		this.gridColumn25.Name = "gridColumn25";
		this.gridColumn25.PrimaryKey = false;
		this.gridColumn25.UnitField = null;
		this.gridColumn25.Visible = true;
		this.gridColumn25.VisibleIndex = 0;
		this.gridColumn26.Caption = "Query Report";
		this.gridColumn26.CurrencyDateField = null;
		this.gridColumn26.CurrencyField = null;
		this.gridColumn26.CurrencyFixed = null;
		this.gridColumn26.CurrencyFixedRate = 0.0;
		this.gridColumn26.CurrencyRateField = null;
		this.gridColumn26.FieldName = "SubId";
		this.gridColumn26.Name = "gridColumn26";
		this.gridColumn26.PrimaryKey = false;
		this.gridColumn26.UnitField = null;
		this.gridColumn26.Visible = true;
		this.gridColumn26.VisibleIndex = 1;
		this.gridColumn27.Caption = "Parameter Name";
		this.gridColumn27.ColumnEdit = this.rep_SubQueryParameters;
		this.gridColumn27.CurrencyDateField = null;
		this.gridColumn27.CurrencyField = null;
		this.gridColumn27.CurrencyFixed = null;
		this.gridColumn27.CurrencyFixedRate = 0.0;
		this.gridColumn27.CurrencyRateField = null;
		this.gridColumn27.FieldName = "Prname";
		this.gridColumn27.Name = "gridColumn27";
		this.gridColumn27.PrimaryKey = false;
		this.gridColumn27.UnitField = null;
		this.gridColumn27.Visible = true;
		this.gridColumn27.VisibleIndex = 2;
		this.rep_SubQueryParameters.AutoHeight = false;
		this.rep_SubQueryParameters.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_SubQueryParameters.Name = "rep_SubQueryParameters";
		this.gridColumn28.Caption = "Column Id";
		this.gridColumn28.ColumnEdit = this.rep_Columns;
		this.gridColumn28.CurrencyDateField = null;
		this.gridColumn28.CurrencyField = null;
		this.gridColumn28.CurrencyFixed = null;
		this.gridColumn28.CurrencyFixedRate = 0.0;
		this.gridColumn28.CurrencyRateField = null;
		this.gridColumn28.FieldName = "ColumnId";
		this.gridColumn28.Name = "gridColumn28";
		this.gridColumn28.PrimaryKey = false;
		this.gridColumn28.UnitField = null;
		this.gridColumn28.Visible = true;
		this.gridColumn28.VisibleIndex = 3;
		this.rep_Columns.AutoHeight = false;
		this.rep_Columns.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_Columns.Name = "rep_Columns";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(544, 466);
		base.Controls.Add(this.wizardControlEx_ExtractQueryReport);
		base.FormId = "oExtractQueryReport";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_ExtractQueryReport.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_ExtractQueryReport";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Extract Query Report - Select Query";
		base.Load += new System.EventHandler(this.frm_ExtractQueryReport_Load);
		((System.ComponentModel.ISupportInitialize)this.gridView4).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Parameters).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Parameters).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_DbType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_IsNullable).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_isApply).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SourceType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Query).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ObjectType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView5).EndInit();
		this.wzp_Columns.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Columns).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Columns).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_ColumnObjectType).EndInit();
		this.wzp_Parmeters.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl1).EndInit();
		this.xtraTabControl1.ResumeLayout(false);
		this.xtraTabPage1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_ValidValues).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_ValidValues).EndInit();
		this.wzp_SelectQuery.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.look_QueryType.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Category.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_RptDesc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_RptName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_QueryName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.wizardControlEx_ExtractQueryReport).EndInit();
		this.wizardControlEx_ExtractQueryReport.ResumeLayout(false);
		this.wzp_Subreport.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl2).EndInit();
		this.splitContainerControl2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Subreport).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Subreport).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_QueryReport).EndInit();
		((System.ComponentModel.ISupportInitialize)this.xtraTabControl2).EndInit();
		this.xtraTabControl2.ResumeLayout(false);
		this.xtraTabPage2.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Subreport_Parameters).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Subreport_Paramaters).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_SubQueryParameters).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_Columns).EndInit();
		base.ResumeLayout(false);
	}
}
