using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Production;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.HttpClientHelper;

namespace Apzon.Forms.FormTemplate;

public class frm_TemplateControlGridGrid : XtraFormEx
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int _objectId;

	private Enums.FormTemplateFormType _formType;

	private DataSet _wizardData = new DataSet();

	private DataTable _wizardDoc = new DataTable();

	private DataTable _wizardContainer = new DataTable();

	private DataTable _wizardControls = new DataTable();

	private DataTable _wizardValidValue = new DataTable();

	private readonly DataTable _validValueStruct = new DataTable();

	private Dictionary<string, string> _query = new Dictionary<string, string>();

	private DataTable _openDataSource;

	private IContainer components;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_OK;

	private UcTemplate ucTemplate1;

	public int TemplateId { get; set; }

	public string FormLayoutCode { get; set; }

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		try
		{
			_formMode = ((value == 0) ? Enums.FormViewModeType.View : value);
			switch (_formMode)
			{
			case Enums.FormViewModeType.Add:
			{
				SimpleButtonEx simpleButtonEx4 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx4.Enabled = enabled;
				btn_OK.Text = Resources.ButtonSave;
				break;
			}
			case Enums.FormViewModeType.Update:
			{
				SimpleButtonEx simpleButtonEx3 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx3.Enabled = enabled;
				btn_OK.Text = Resources.ButtonUpdate;
				break;
			}
			case Enums.FormViewModeType.View:
			{
				SimpleButtonEx simpleButtonEx2 = btn_OK;
				bool enabled = (btn_Cancel.Enabled = true);
				simpleButtonEx2.Enabled = enabled;
				btn_OK.Text = Resources.ButtonOK;
				break;
			}
			case Enums.FormViewModeType.Busy:
			{
				SimpleButtonEx simpleButtonEx = btn_OK;
				bool enabled = (btn_Cancel.Enabled = false);
				simpleButtonEx.Enabled = enabled;
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

	public frm_TemplateControlGridGrid(int templateId)
	{
		InitializeComponent();
		TemplateId = templateId;
	}

	public frm_TemplateControlGridGrid(int templateId, int objectId)
	{
		InitializeComponent();
		TemplateId = templateId;
		_objectId = objectId;
	}

	public frm_TemplateControlGridGrid(int templateId, Dictionary<string, string> query)
	{
		InitializeComponent();
		TemplateId = templateId;
		_query = query;
	}

	public frm_TemplateControlGridGrid()
	{
		InitializeComponent();
	}

	public async Task REFRESH()
	{
	}

	public async Task MODE_FIND()
	{
	}

	public async Task MOVE_FIRST()
	{
	}

	public async Task MOVE_PREVIOUS()
	{
	}

	public async Task MOVE_NEXT()
	{
	}

	public async Task MOVE_LAST()
	{
	}

	public async Task REFRESH_DATA()
	{
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			int num = Function.ParseInt(ucTemplate1.ObjectId);
			await WinFuntions.PrintLayout("", num, FormLayoutCode, ((int)FormObjectType).ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PREVIEWLAYOUTS()
	{
		try
		{
			int num = Function.ParseInt(ucTemplate1.ObjectId);
			await WinFuntions.PreviewLayout("", num, FormLayoutCode, this, ((int)FormObjectType).ToString());
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task frm_TemplateControlGridGrid_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			_validValueStruct.Columns.Add("Code", typeof(string));
			_validValueStruct.Columns.Add("Name", typeof(string));
			if (TemplateId != 0)
			{
				FormId = "W" + TemplateId;
			}
			_wizardData = await _httpClient.FormWizard.GetById(TemplateId);
			Text = Function.ToString(_wizardData.Tables["OFWZ"].Rows[0]["Caption"]);
			FormLayoutCode = Function.ToString(_wizardData.Tables["OFWZ"].Rows[0]["LayoutCode"]);
			string text = Function.ToString(_wizardData.Tables["OFWZ"].Rows[0]["AcceptButton"]);
			_formType = (Enums.FormTemplateFormType)Function.ParseInt(_wizardData.Tables["OFWZ"].Rows[0]["FormType"]);
			if (text == "Y")
			{
				btn_OK.Visible = true;
			}
			else
			{
				btn_OK.Visible = false;
			}
			if (Function.ToString(_wizardData.Tables["OFWZ"].Rows[0]["CancelButton"]) == "Y")
			{
				btn_Cancel.Visible = true;
			}
			else
			{
				btn_Cancel.Visible = false;
			}
			ucTemplate1.FormId = FormId;
			ucTemplate1.TemplateId = TemplateId;
			ucTemplate1.ObjectId = _objectId;
			ucTemplate1.WizardData = _wizardData;
			await ucTemplate1.LoadLayoutControl();
			DataTable defaultHeaderData = _wizardData.Tables["FWZ1"];
			string text2 = Function.ToString(_wizardData.Tables["OFWZ"].Rows[0]["Stack0"]);
			string text3 = Function.ToString(_wizardData.Tables["OFWZ"].Rows[0]["Stack1"]);
			string text4 = Function.ToString(_wizardData.Tables["OFWZ"].Rows[0]["Stack2"]);
			if (text2 == "Y" && text3 != "Y" && text4 != "Y")
			{
				Height = ucTemplate1.SpliterPosition + 100;
				Width = 925;
			}
			if (!ucTemplate1.HasRight || !ucTemplate1.HasLeft)
			{
				if (text2 == "Y" && text3 == "N" && text4 == "N")
				{
					Width = 420;
				}
				else
				{
					Width = 925;
				}
			}
			await SetDefaultHeaderData(defaultHeaderData);
			if (_formType != Enums.FormTemplateFormType.InputBox)
			{
				if (_objectId != 0)
				{
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					FormMode = Enums.FormViewModeType.Add;
				}
			}
			ucTemplate1.HandleQuery(_query);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SetDefaultHeaderData(DataTable wizardContainer)
	{
		EnumerableRowCollection<DataRow> enumerableRowCollection = from t in wizardContainer.AsEnumerable()
			where Function.ToString(t["ContainerType"]) == "F"
			select t;
		if (!enumerableRowCollection.Any())
		{
			return;
		}
		DataSet ds = new DataSet();
		DataTable dtSource = new DataTable();
		foreach (DataRow item in enumerableRowCollection)
		{
			DataTable dataTable = await ucTemplate1.GetData(item);
			ds.Tables.Add(dataTable);
			foreach (DataColumn column in dataTable.Columns)
			{
				if (!dtSource.Columns.Contains(column.ColumnName))
				{
					dtSource.Columns.Add(column.ColumnName, column.DataType);
					dtSource.Columns[column.ColumnName].DefaultValue = column.DefaultValue;
				}
			}
		}
		dtSource.Rows.Add(dtSource.NewRow());
		foreach (DataTable table in ds.Tables)
		{
			if (table.Rows.Count <= 0)
			{
				continue;
			}
			foreach (DataColumn column2 in table.Columns)
			{
				dtSource.Rows[0][column2.ColumnName] = table.Rows[0][column2.ColumnName];
			}
		}
		dtSource.AcceptChanges();
		DataSource = dtSource;
	}

	public override bool SetVisibleControl(string fieldName, bool isVisible)
	{
		try
		{
			ucTemplate1.SetVisibleControl(fieldName, isVisible);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
		return isVisible;
	}

	public override void AfterAction(HttpResult ret, bool resetData = false)
	{
		try
		{
			if (!string.IsNullOrEmpty(ret.Message))
			{
				WinCommons.Error(Enums.MessageCode.None, ret.Message);
			}
			else
			{
				if (!resetData)
				{
					return;
				}
				this.ResetData(resetAppearance: false);
				if (ucTemplate1.Attachments != null && ucTemplate1.Attachments.Count > 0)
				{
					foreach (ucAttachment attachment in ucTemplate1.Attachments)
					{
						attachment.grd_File.DataSourceTable.Clear();
						attachment.grd_File.DataSourceTable.AcceptChanges();
					}
				}
				foreach (XtraTabPageEx tabPage in ucTemplate1.StackGroup1.TabPages)
				{
					foreach (Control control3 in tabPage.Controls)
					{
						if (control3 is GridControlEx)
						{
							DataTable dataSourceTable = (control3 as GridControlEx).DataSourceTable;
							dataSourceTable.Rows.Clear();
							dataSourceTable.AcceptChanges();
							dataSourceTable.Rows.Add(dataSourceTable.NewRow());
							break;
						}
					}
				}
				{
					foreach (XtraTabPageEx tabPage2 in ucTemplate1.StackGroup2.TabPages)
					{
						foreach (Control control4 in tabPage2.Controls)
						{
							if (control4 is GridControlEx)
							{
								DataTable dataSourceTable2 = (control4 as GridControlEx).DataSourceTable;
								dataSourceTable2.Rows.Clear();
								dataSourceTable2.AcceptChanges();
								dataSourceTable2.Rows.Add(dataSourceTable2.NewRow());
								break;
							}
						}
					}
					return;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_TemplateControlGridGrid_Shown(object sender, EventArgs e)
	{
		try
		{
			Size = new Size(Size.Width + 1, Size.Height);
			ucTemplate1.HandleSpliterPosition();
			if (DataSource is DataTable dataTable)
			{
				_openDataSource = dataTable.Copy();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		try
		{
			Close();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void frm_TemplateControlGridGrid_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (_formType != Enums.FormTemplateFormType.InputBox && XtraMessageBoxEx.Show(this, Enums.MessageCode.ConfirmCloseForm, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.Yes)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task btn_OK_Click(object sender, EventArgs e)
	{
		Enums.FormViewModeType currentView = FormMode;
		try
		{
			DataRow dataRow = DsContextAction?.Tables[0].AsEnumerable().FirstOrDefault((DataRow t) => Function.ParseInt(t["EditorType"]) == 3);
			if (dataRow == null)
			{
				FormMode = currentView;
				if (_formType == Enums.FormTemplateFormType.InputBox)
				{
					Close();
				}
				return;
			}
			FormMode = Enums.FormViewModeType.Busy;
			List<AttachmentFile_Impl> list = new List<AttachmentFile_Impl>();
			List<ucAttachment> attachments = ucTemplate1.Attachments;
			if (attachments != null)
			{
				foreach (ucAttachment item in attachments)
				{
					list = item.UploadFile();
					if (item.DataSource.IsNotNull() && list.Count == 0)
					{
						return;
					}
				}
			}
			int absEntry = Function.ParseInt(dataRow["AbsEntry"]);
			DataSet dataSet = DataSet();
			if (dataSet.Tables.Count == 1)
			{
				if (DefaultGrid != null)
				{
					new ContextActionHandle(absEntry, FormId, FirstItemId, DefaultGrid.GridControl.DataSource as DataTable, DefaultGrid);
					return;
				}
				if (DefaultTree != null)
				{
					new ContextActionHandle(absEntry, FormId, FirstItemId, DefaultTree.DataSourceTable, DefaultTree);
					return;
				}
				if (DefaultPivot != null)
				{
					new ContextActionHandle(absEntry, FormId, FirstItemId, DefaultPivot.DataSource as DataTable, DefaultPivot);
					return;
				}
				HttpResult httpResult = await new ContextActionHandle(absEntry, FormId, FirstItemId, dataSet, this, list).Handle();
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					_ = httpResult.Message == "EventCanceled";
					FormMode = currentView;
					return;
				}
				switch (currentView)
				{
				case Enums.FormViewModeType.Add:
					FormMode = Enums.FormViewModeType.Add;
					break;
				case Enums.FormViewModeType.Update:
					FormMode = Enums.FormViewModeType.View;
					break;
				}
				return;
			}
			HttpResult httpResult2 = await new ContextActionHandle(absEntry, FormId, FirstItemId, dataSet, this, list).Handle();
			if (!string.IsNullOrEmpty(httpResult2.Message))
			{
				_ = httpResult2.Message == "EventCanceled";
				FormMode = currentView;
				return;
			}
			switch (currentView)
			{
			case Enums.FormViewModeType.Add:
				FormMode = Enums.FormViewModeType.Add;
				break;
			case Enums.FormViewModeType.Update:
				FormMode = Enums.FormViewModeType.View;
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.ErrorLineNumber(ex);
			FormMode = currentView;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.FormTemplate.frm_TemplateControlGridGrid));
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.ucTemplate1 = new Apzon.Forms.FormTemplate.UcTemplate();
		base.SuspendLayout();
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Close";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(101, 599);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 4;
		this.btn_Cancel.Text = "Close";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.EditControl = null;
		this.btn_OK.EditValue = "OK";
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_OK.ImageOptions.Image");
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(12, 599);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.ReadOnly = false;
		this.btn_OK.SettingInVisible = false;
		this.btn_OK.SettingReadonly = false;
		this.btn_OK.Size = new System.Drawing.Size(83, 23);
		this.btn_OK.TabIndex = 3;
		this.btn_OK.Text = "OK";
		this.btn_OK.OnClickAsync += new Apzon.Controls.Controls.OnClickAsync(this.btn_OK_Click);
		this.ucTemplate1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.ucTemplate1.Attachments = null;
		this.ucTemplate1.FormId = "oTemplateControlGridGrid";
		this.ucTemplate1.HasLeft = false;
		this.ucTemplate1.HasRight = false;
		this.ucTemplate1.Location = new System.Drawing.Point(3, 1);
		this.ucTemplate1.Name = "ucTemplate1";
		this.ucTemplate1.Size = new System.Drawing.Size(1014, 592);
		this.ucTemplate1.SpliterPosition = 0;
		this.ucTemplate1.TabIndex = 5;
		this.ucTemplate1.TemplateId = 0;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1018, 634);
		base.Controls.Add(this.ucTemplate1);
		base.Controls.Add(this.btn_Cancel);
		base.Controls.Add(this.btn_OK);
		base.FormId = "oTemplateControlGridGrid";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_TemplateControlGridGrid";
		base.ShowInTaskbar = false;
		this.Text = "frm_TemplateControlGridGrid";
		base.OnLoadAsync += new Apzon.Controls.Form.LoadAsyncEventHandler(this.frm_TemplateControlGridGrid_Load);
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_TemplateControlGridGrid_FormClosing);
		base.Shown += new System.EventHandler(this.frm_TemplateControlGridGrid_Shown);
		base.ResumeLayout(false);
	}
}
