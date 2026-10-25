using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Properties;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using Newtonsoft.Json;

namespace Apzon.Forms.Commons.UDF;

public class frm_UDFFieldData : Form
{
	private List<Common.Table_Code> lstCode = new List<Common.Table_Code>();

	private List<UDFFieldTypeCode> lstType = new List<UDFFieldTypeCode>();

	private UDFsImpl UDFs = new UDFsImpl();

	public string tableName = "";

	public string fieldName = "";

	private IContainer components;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private LabelLine labelLine3;

	private LabelLine labelLine4;

	private TextEditEx txtFieldName;

	private ComboBoxEditEx cbStructure;

	private TextEditEx txtDescription;

	private CalcEditEx calcLength;

	private CheckEditEx chkSetValidValue;

	private GridControlEx grdDoc;

	private GridViewEx gvDoc;

	private GridColumnEx col_Value;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	private GridColumnEx col_Description;

	private RepositoryItemTextEdit repositoryItemTextEdit2;

	private SimpleButtonEx btNew;

	private SimpleButtonEx btDelete;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btOk;

	private CheckEditEx chkSetDefaultValue;

	private ComboBoxEditEx cbSetDefaultValue;

	private CheckEditEx chkSetLinkTable;

	private ComboBoxEditEx cbSetLinkTable;

	public frm_UDFFieldData()
	{
		InitializeComponent();
	}

	private void frm_UDFFieldData_Load(object sender, EventArgs e)
	{
		lstType = WinFuntions.LoadUDFFieldType();
		grdDoc.Visible = false;
		cbSetDefaultValue.Visible = false;
		cbSetLinkTable.Visible = false;
		btNew.Visible = false;
		btDelete.Visible = false;
		cbStructure.SelectedIndex = 0;
		if (fieldName != "")
		{
			LoadField(tableName + "," + fieldName);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void chkSetValidValue_CheckedChanged(object sender, EventArgs e)
	{
		if (chkSetValidValue.Checked)
		{
			grdDoc.Visible = true;
			btNew.Visible = true;
			btDelete.Visible = true;
			chkSetLinkTable.Visible = false;
			cbSetLinkTable.Visible = false;
		}
		else
		{
			grdDoc.Visible = false;
			btNew.Visible = false;
			btDelete.Visible = false;
			chkSetLinkTable.Visible = false;
			cbSetLinkTable.Visible = false;
		}
	}

	private void chkSetLinkTable_CheckedChanged(object sender, EventArgs e)
	{
		if (chkSetLinkTable.Checked)
		{
			chkSetValidValue.Visible = false;
			grdDoc.Visible = false;
			btNew.Visible = false;
			btDelete.Visible = false;
			chkSetDefaultValue.Visible = false;
			cbSetLinkTable.Visible = true;
		}
		else
		{
			chkSetValidValue.Visible = true;
			grdDoc.Visible = false;
			btNew.Visible = false;
			btDelete.Visible = false;
			chkSetDefaultValue.Visible = true;
			cbSetLinkTable.Visible = false;
		}
	}

	private void chkSetDefaultValue_CheckedChanged(object sender, EventArgs e)
	{
		if (chkSetDefaultValue.Checked)
		{
			cbSetDefaultValue.Visible = true;
			chkSetLinkTable.Visible = false;
		}
		else
		{
			cbSetDefaultValue.Visible = false;
			chkSetLinkTable.Visible = true;
		}
	}

	private void btNew_Click(object sender, EventArgs e)
	{
		if (gvDoc.RowCount == 0)
		{
			Common.Table_Code table_Code = new Common.Table_Code();
			table_Code.Code = "";
			table_Code.Name = "";
			lstCode.Add(table_Code);
			AddRow();
		}
		else if (((List<Common.Table_Code>)gvDoc.DataSource).Where((Common.Table_Code x) => x.Name.Trim() == "" && x.Code.Trim() == "").ToList().Count == 0)
		{
			Common.Table_Code table_Code2 = new Common.Table_Code();
			table_Code2.Code = "";
			table_Code2.Name = "";
			lstCode.Add(table_Code2);
			AddRow();
		}
	}

	private void btDelete_Click(object sender, EventArgs e)
	{
		gvDoc.DeleteSelectedRows();
	}

	private void btOk_Click(object sender, EventArgs e)
	{
		if (txtFieldName.Text.Trim() != "" && fieldName == "")
		{
			AddData();
		}
		else
		{
			UpdateData();
		}
		Close();
	}

	private async void AddData()
	{
		UDFsImpl UDF = new UDFsImpl();
		UDF.TableName = tableName;
		UDF.FieldName = "U" + txtFieldName.Text.Trim();
		UDF.DefaultValue = (chkSetDefaultValue.Checked ? cbSetDefaultValue.SelectedItem.ToString() : null);
		UDF.Description = txtDescription.Text.Trim();
		UDF.TypeID = lstType.FirstOrDefault((UDFFieldTypeCode x) => x.comboString == cbStructure.SelectedItem.ToString()).TypeID;
		UDF.DataType = lstType.FirstOrDefault((UDFFieldTypeCode x) => x.TypeID == UDF.TypeID).dbType;
		UDF.TypeControl = lstType.FirstOrDefault((UDFFieldTypeCode x) => x.TypeID == UDF.TypeID).TypeControl;
		UDF.StatusDisplay = true;
		UDF.LinkTable = cbSetLinkTable.SelectedText;
		UDF.Lenght = calcLength.Text;
		await SaveDoc(UDF);
	}

	private async void UpdateData()
	{
		UDFsImpl uDFs = UDFs;
		uDFs.FieldName = txtFieldName.Text.Trim();
		try
		{
			uDFs.DefaultValue = (chkSetDefaultValue.Checked ? cbSetDefaultValue.SelectedItem.ToString() : null);
		}
		catch
		{
			uDFs.DefaultValue = null;
		}
		uDFs.Description = txtDescription.Text.Trim();
		uDFs.LinkTable = cbSetLinkTable.SelectedText;
		uDFs.Lenght = calcLength.Text;
		await SaveDoc(uDFs);
	}

	private void AddRow()
	{
		if (grdDoc.InvokeRequired)
		{
			BeginInvoke((MethodInvoker)(() =>
			{
				grdDoc.DataSource = lstCode;
				grdDoc.RefreshDataSource();
			}));
		}
		else
		{
			grdDoc.DataSource = lstCode;
			grdDoc.RefreshDataSource();
		}
	}

	private void cbStructure_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (fieldName == "")
		{
			string text = cbStructure.SelectedItem.ToString();
			cbSetDefaultValue.Properties.Items.Clear();
			cbSetDefaultValue.Text = "";
			lstCode.Clear();
			grdDoc.DataSource = lstCode;
			grdDoc.RefreshDataSource();
			lstCode = new List<Common.Table_Code>();
			switch (text)
			{
			case "String":
				calcLength.ReadOnly = false;
				calcLength.Text = "20";
				disableControls(isDisable: false);
				break;
			case "Numeric":
				calcLength.ReadOnly = true;
				calcLength.Text = "4";
				disableControls(isDisable: false);
				break;
			case "Currency":
				calcLength.ReadOnly = true;
				calcLength.Text = "18.3";
				disableControls(isDisable: true);
				break;
			case "Datetime":
				calcLength.ReadOnly = true;
				calcLength.Text = "8";
				disableControls(isDisable: true);
				break;
			case "Bool":
				calcLength.ReadOnly = true;
				calcLength.Text = "1";
				disableControls(isDisable: true);
				break;
			}
		}
	}

	private void disableControls(bool isDisable)
	{
		if (isDisable)
		{
			chkSetValidValue.Visible = false;
			chkSetValidValue.Checked = false;
			grdDoc.Visible = false;
			btNew.Visible = false;
			btDelete.Visible = false;
			chkSetDefaultValue.Visible = false;
			chkSetDefaultValue.Checked = false;
			chkSetLinkTable.Visible = false;
			chkSetLinkTable.Checked = false;
			cbSetDefaultValue.Visible = false;
			cbSetLinkTable.Visible = false;
		}
		else
		{
			chkSetValidValue.Visible = true;
			chkSetDefaultValue.Visible = true;
			chkSetLinkTable.Visible = true;
			chkSetValidValue.Checked = false;
			chkSetDefaultValue.Checked = false;
			chkSetLinkTable.Checked = false;
		}
	}

	private void gvDoc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (e.Column == col_Value && e.Value != null)
		{
			cbSetDefaultValue.Properties.Items.Clear();
			updateListCode();
		}
	}

	private void updateListCode()
	{
		int rowCount = gvDoc.RowCount;
		List<Common.Table_Code> list = new List<Common.Table_Code>();
		if (rowCount <= 0)
		{
			return;
		}
		for (int i = 0; i < rowCount; i++)
		{
			Common.Table_Code table_Code = new Common.Table_Code();
			try
			{
				table_Code.Code = (string.IsNullOrEmpty(gvDoc.GetRowCellValue(i, "Code").ToString()) ? "" : gvDoc.GetRowCellValue(i, "Code").ToString());
			}
			catch
			{
				table_Code.Code = "";
			}
			try
			{
				table_Code.Name = (string.IsNullOrEmpty(gvDoc.GetRowCellValue(i, "Name").ToString()) ? "" : gvDoc.GetRowCellValue(i, "Name").ToString());
			}
			catch
			{
				table_Code.Name = "";
			}
			if (table_Code.Code != "")
			{
				list.Add(table_Code);
				cbSetDefaultValue.Properties.Items.Add(table_Code.Code);
			}
		}
		lstCode = list;
		grdDoc.RefreshDataSource();
	}

	private async void LoadField(string tableAndField)
	{
		using HttpClient client = new HttpClient();
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "UDF", "GetUDF", new { tableAndField });
		new List<UDFsImpl>();
		using HttpResponseMessage response = await client.GetAsync(apiUri);
		if (!response.IsSuccessStatusCode)
		{
			return;
		}
		List<UDFsImpl> list = JsonConvert.DeserializeObject<List<UDFsImpl>>(await response.Content.ReadAsStringAsync()).ToList();
		if (list.Count > 0)
		{
			UDFs = list.First();
			txtFieldName.Text = fieldName;
			txtFieldName.ReadOnly = true;
			cbStructure.ReadOnly = true;
			btOk.Text = "Update";
			txtDescription.Text = UDFs.Description;
			int selectedIndex = lstType.FindIndex((UDFFieldTypeCode x) => x.TypeID == UDFs.TypeID);
			cbStructure.SelectedIndex = selectedIndex;
			calcLength.Text = UDFs.Lenght;
			if (UDFs.TypeID != "T")
			{
				calcLength.ReadOnly = true;
			}
			if (!string.IsNullOrEmpty(UDFs.DefaultValue))
			{
				chkSetDefaultValue.Checked = true;
				chkSetDefaultValue.Visible = true;
				cbSetDefaultValue.Properties.Items.Add(UDFs.DefaultValue);
				cbSetDefaultValue.SelectedIndex = 0;
				cbSetDefaultValue.Visible = true;
			}
		}
	}

	private async Task SaveDoc(UDFsImpl doc)
	{
		HttpRequestMessage httpRequestMessage = new HttpRequestMessage();
		new MultipartFormDataContent();
		httpRequestMessage.Content = new ObjectContent<UDFsImpl>(doc, new JsonMediaTypeFormatter());
		httpRequestMessage.Method = HttpMethod.Post;
		string apiUri = Function.GetApiUri(Configuration.ApiServer, "UDF", "SaveDoc");
		httpRequestMessage.RequestUri = new Uri(apiUri);
		await new HttpClient().SendAsync(httpRequestMessage).ContinueWith((Task<HttpResponseMessage> task) =>
		{
			if (task.Result.IsSuccessStatusCode)
			{
				Task<string> task2 = task.Result.Content.ReadAsStringAsync();
				if (task2 != null && string.IsNullOrEmpty(task2.Result))
				{
					return task2.Result;
				}
				WinCommons.writeStatus(Enums.MessageCode.Success, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
			}
			return (string)null;
		});
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
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.txtFieldName = new Apzon.Controls.Editor.TextEditEx();
		this.cbStructure = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.txtDescription = new Apzon.Controls.Editor.TextEditEx();
		this.calcLength = new Apzon.Controls.Editor.CalcEditEx();
		this.chkSetValidValue = new Apzon.Controls.Editor.CheckEditEx();
		this.grdDoc = new Apzon.Controls.Grid.GridControlEx();
		this.gvDoc = new Apzon.Controls.Grid.GridViewEx();
		this.col_Value = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.col_Description = new Apzon.Controls.Grid.GridColumnEx();
		this.repositoryItemTextEdit2 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.btNew = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btDelete = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btOk = new Apzon.Controls.Controls.SimpleButtonEx();
		this.chkSetDefaultValue = new Apzon.Controls.Editor.CheckEditEx();
		this.cbSetDefaultValue = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.chkSetLinkTable = new Apzon.Controls.Editor.CheckEditEx();
		this.cbSetLinkTable = new Apzon.Controls.Controls.ComboBoxEditEx();
		((System.ComponentModel.ISupportInitialize)this.txtFieldName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbStructure.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtDescription.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.calcLength.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkSetValidValue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit2).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkSetDefaultValue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbSetDefaultValue.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.chkSetLinkTable.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cbSetLinkTable.Properties).BeginInit();
		base.SuspendLayout();
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "Field Name";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(8, 10);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(57, 20);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Field Name";
		this.labelLine2.EditControl = null;
		this.labelLine2.EditValue = "Structure";
		this.labelLine2.FieldName = null;
		this.labelLine2.HideWhenEmpty = false;
		this.labelLine2.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine2.Location = new System.Drawing.Point(8, 34);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.ReadOnly = false;
		this.labelLine2.SettingInVisible = false;
		this.labelLine2.SettingReadonly = false;
		this.labelLine2.ShowUnderLine = true;
		this.labelLine2.Size = new System.Drawing.Size(57, 20);
		this.labelLine2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine2.TabIndex = 1;
		this.labelLine2.Utext = "Structure";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Description";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(162, 8);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(57, 20);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 2;
		this.labelLine3.Utext = "Description";
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Length";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(164, 31);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(57, 20);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 3;
		this.labelLine4.Utext = "Length";
		this.txtFieldName.EditControl = null;
		this.txtFieldName.FieldName = null;
		this.txtFieldName.IsSearchControl = false;
		this.txtFieldName.Location = new System.Drawing.Point(65, 7);
		this.txtFieldName.Name = "txtFieldName";
		this.txtFieldName.SettingInVisible = false;
		this.txtFieldName.SettingReadonly = false;
		this.txtFieldName.Size = new System.Drawing.Size(87, 20);
		this.txtFieldName.TabIndex = 4;
		this.cbStructure.EditControl = null;
		this.cbStructure.FieldName = null;
		this.cbStructure.IsSearchControl = false;
		this.cbStructure.Location = new System.Drawing.Point(65, 30);
		this.cbStructure.Name = "cbStructure";
		this.cbStructure.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbStructure.Properties.Items.AddRange(new object[5] { "String", "Numeric", "Currency", "Datetime", "Bool" });
		this.cbStructure.SettingInVisible = false;
		this.cbStructure.SettingReadonly = false;
		this.cbStructure.Size = new System.Drawing.Size(87, 20);
		this.cbStructure.TabIndex = 5;
		this.cbStructure.SelectedIndexChanged += new System.EventHandler(this.cbStructure_SelectedIndexChanged);
		this.txtDescription.EditControl = null;
		this.txtDescription.FieldName = null;
		this.txtDescription.IsSearchControl = false;
		this.txtDescription.Location = new System.Drawing.Point(220, 5);
		this.txtDescription.Name = "txtDescription";
		this.txtDescription.SettingInVisible = false;
		this.txtDescription.SettingReadonly = false;
		this.txtDescription.Size = new System.Drawing.Size(166, 20);
		this.txtDescription.TabIndex = 6;
		this.calcLength.EditControl = null;
		this.calcLength.FieldName = null;
		this.calcLength.IsSearchControl = false;
		this.calcLength.Location = new System.Drawing.Point(220, 28);
		this.calcLength.Name = "calcLength";
		this.calcLength.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.calcLength.Properties.IsSearchControl = false;
		this.calcLength.SettingInVisible = false;
		this.calcLength.SettingReadonly = false;
		this.calcLength.Size = new System.Drawing.Size(63, 20);
		this.calcLength.TabIndex = 7;
		this.chkSetValidValue.EditControl = null;
		this.chkSetValidValue.FieldName = null;
		this.chkSetValidValue.IsSearchControl = false;
		this.chkSetValidValue.Location = new System.Drawing.Point(8, 60);
		this.chkSetValidValue.Name = "chkSetValidValue";
		this.chkSetValidValue.Properties.Caption = "Set valid value for field";
		this.chkSetValidValue.SettingInVisible = false;
		this.chkSetValidValue.SettingReadonly = false;
		this.chkSetValidValue.Size = new System.Drawing.Size(144, 20);
		this.chkSetValidValue.TabIndex = 8;
		this.chkSetValidValue.CheckedChanged += new System.EventHandler(this.chkSetValidValue_CheckedChanged);
		this.grdDoc.AllowPasteExcel = true;
		this.grdDoc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grdDoc.ClipboardData = null;
		this.grdDoc.Location = new System.Drawing.Point(8, 85);
		this.grdDoc.MainView = this.gvDoc;
		this.grdDoc.Name = "grdDoc";
		this.grdDoc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.repositoryItemTextEdit1, this.repositoryItemTextEdit2 });
		this.grdDoc.Size = new System.Drawing.Size(298, 166);
		this.grdDoc.TabIndex = 9;
		this.grdDoc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gvDoc });
		this.grdDoc.Visible = false;
		this.gvDoc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gvDoc.ColumnKey = null;
		this.gvDoc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.col_Value, this.col_Description });
		this.gvDoc.DisableFormSetting = false;
		this.gvDoc.DisplayException = false;
		this.gvDoc.EditColumn = null;
		this.gvDoc.FieldCurrency = null;
		this.gvDoc.FieldRate = null;
		this.gvDoc.FormId = null;
		this.gvDoc.GridControl = this.grdDoc;
		this.gvDoc.IgnoreFilter = false;
		this.gvDoc.IndexEditRow = -1;
		this.gvDoc.IndicatorWidth = 35;
		this.gvDoc.IsApzTable = false;
		this.gvDoc.IsInsertNewRow = false;
		this.gvDoc.IsLoadingData = false;
		this.gvDoc.IsMappingContextAction = false;
		this.gvDoc.ItemId = null;
		this.gvDoc.LstColumnAllowBatch = null;
		this.gvDoc.LstColumnNotAllowBatch = null;
		this.gvDoc.LstColumnNotEditable = null;
		this.gvDoc.LstColumNotSetting = null;
		this.gvDoc.Name = "gvDoc";
		this.gvDoc.OptionsBehavior.AllowAddRows = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsBehavior.AllowDeleteRows = DevExpress.Utils.DefaultBoolean.True;
		this.gvDoc.OptionsSelection.MultiSelect = true;
		this.gvDoc.OptionsView.ShowGroupPanel = false;
		this.gvDoc.UseBatchMod = false;
		this.gvDoc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gvDoc_CellValueChanged);
		this.col_Value.Caption = "Value";
		this.col_Value.ColumnEdit = this.repositoryItemTextEdit1;
		this.col_Value.CurrencyDateField = null;
		this.col_Value.CurrencyField = null;
		this.col_Value.CurrencyFixed = null;
		this.col_Value.CurrencyFixedRate = 0.0;
		this.col_Value.CurrencyRateField = null;
		this.col_Value.FieldName = "Code";
		this.col_Value.Name = "col_Value";
		this.col_Value.OptionsColumn.AllowMove = false;
		this.col_Value.PrimaryKey = true;
		this.col_Value.UnitField = null;
		this.col_Value.Visible = true;
		this.col_Value.VisibleIndex = 0;
		this.repositoryItemTextEdit1.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.repositoryItemTextEdit1.Appearance.BackColor2 = System.Drawing.Color.FromArgb(255, 255, 192);
		this.repositoryItemTextEdit1.Appearance.Options.UseBackColor = true;
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.col_Description.Caption = "Description";
		this.col_Description.ColumnEdit = this.repositoryItemTextEdit2;
		this.col_Description.CurrencyDateField = null;
		this.col_Description.CurrencyField = null;
		this.col_Description.CurrencyFixed = null;
		this.col_Description.CurrencyFixedRate = 0.0;
		this.col_Description.CurrencyRateField = null;
		this.col_Description.FieldName = "Name";
		this.col_Description.Name = "col_Description";
		this.col_Description.OptionsColumn.AllowMove = false;
		this.col_Description.PrimaryKey = false;
		this.col_Description.UnitField = null;
		this.col_Description.Visible = true;
		this.col_Description.VisibleIndex = 1;
		this.repositoryItemTextEdit2.Appearance.BackColor = System.Drawing.Color.FromArgb(255, 255, 192);
		this.repositoryItemTextEdit2.Appearance.BackColor2 = System.Drawing.Color.FromArgb(255, 255, 192);
		this.repositoryItemTextEdit2.Appearance.Options.UseBackColor = true;
		this.repositoryItemTextEdit2.AutoHeight = false;
		this.repositoryItemTextEdit2.Name = "repositoryItemTextEdit2";
		this.btNew.EditControl = null;
		this.btNew.EditValue = "New";
		this.btNew.FieldName = null;
		this.btNew.ImageOptions.Image = Apzon.Forms.Properties.Resources.add_16x16;
		this.btNew.IsSearchControl = false;
		this.btNew.Location = new System.Drawing.Point(312, 85);
		this.btNew.Name = "btNew";
		this.btNew.ReadOnly = false;
		this.btNew.SettingInVisible = false;
		this.btNew.SettingReadonly = false;
		this.btNew.Size = new System.Drawing.Size(74, 23);
		this.btNew.TabIndex = 10;
		this.btNew.Text = "New";
		this.btNew.Click += new System.EventHandler(this.btNew_Click);
		this.btDelete.EditControl = null;
		this.btDelete.EditValue = "Delete";
		this.btDelete.FieldName = null;
		this.btDelete.ImageOptions.Image = Apzon.Forms.Properties.Resources.remove_16x16;
		this.btDelete.IsSearchControl = false;
		this.btDelete.Location = new System.Drawing.Point(312, 114);
		this.btDelete.Name = "btDelete";
		this.btDelete.ReadOnly = false;
		this.btDelete.SettingInVisible = false;
		this.btDelete.SettingReadonly = false;
		this.btDelete.Size = new System.Drawing.Size(74, 23);
		this.btDelete.TabIndex = 11;
		this.btDelete.Text = "Delete";
		this.btDelete.Click += new System.EventHandler(this.btDelete_Click);
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = Apzon.Forms.Properties.Resources.cancel_16x16;
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(88, 346);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(74, 23);
		this.btCancel.TabIndex = 13;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btOk.EditControl = null;
		this.btOk.EditValue = "OK";
		this.btOk.FieldName = null;
		this.btOk.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btOk.IsSearchControl = false;
		this.btOk.Location = new System.Drawing.Point(8, 346);
		this.btOk.Name = "btOk";
		this.btOk.ReadOnly = false;
		this.btOk.SettingInVisible = false;
		this.btOk.SettingReadonly = false;
		this.btOk.Size = new System.Drawing.Size(74, 23);
		this.btOk.TabIndex = 12;
		this.btOk.Text = "OK";
		this.btOk.Click += new System.EventHandler(this.btOk_Click);
		this.chkSetDefaultValue.EditControl = null;
		this.chkSetDefaultValue.FieldName = null;
		this.chkSetDefaultValue.IsSearchControl = false;
		this.chkSetDefaultValue.Location = new System.Drawing.Point(8, 259);
		this.chkSetDefaultValue.Name = "chkSetDefaultValue";
		this.chkSetDefaultValue.Properties.Caption = "Set default value for field";
		this.chkSetDefaultValue.SettingInVisible = false;
		this.chkSetDefaultValue.SettingReadonly = false;
		this.chkSetDefaultValue.Size = new System.Drawing.Size(144, 20);
		this.chkSetDefaultValue.TabIndex = 14;
		this.chkSetDefaultValue.CheckedChanged += new System.EventHandler(this.chkSetDefaultValue_CheckedChanged);
		this.cbSetDefaultValue.EditControl = null;
		this.cbSetDefaultValue.FieldName = null;
		this.cbSetDefaultValue.IsSearchControl = false;
		this.cbSetDefaultValue.Location = new System.Drawing.Point(8, 278);
		this.cbSetDefaultValue.Name = "cbSetDefaultValue";
		this.cbSetDefaultValue.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbSetDefaultValue.SettingInVisible = false;
		this.cbSetDefaultValue.SettingReadonly = false;
		this.cbSetDefaultValue.Size = new System.Drawing.Size(207, 20);
		this.cbSetDefaultValue.TabIndex = 15;
		this.chkSetLinkTable.EditControl = null;
		this.chkSetLinkTable.FieldName = null;
		this.chkSetLinkTable.IsSearchControl = false;
		this.chkSetLinkTable.Location = new System.Drawing.Point(8, 310);
		this.chkSetLinkTable.Name = "chkSetLinkTable";
		this.chkSetLinkTable.Properties.Caption = "Set link table";
		this.chkSetLinkTable.SettingInVisible = false;
		this.chkSetLinkTable.SettingReadonly = false;
		this.chkSetLinkTable.Size = new System.Drawing.Size(144, 20);
		this.chkSetLinkTable.TabIndex = 16;
		this.chkSetLinkTable.CheckedChanged += new System.EventHandler(this.chkSetLinkTable_CheckedChanged);
		this.cbSetLinkTable.EditControl = null;
		this.cbSetLinkTable.FieldName = null;
		this.cbSetLinkTable.IsSearchControl = false;
		this.cbSetLinkTable.Location = new System.Drawing.Point(164, 310);
		this.cbSetLinkTable.Name = "cbSetLinkTable";
		this.cbSetLinkTable.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cbSetLinkTable.SettingInVisible = false;
		this.cbSetLinkTable.SettingReadonly = false;
		this.cbSetLinkTable.Size = new System.Drawing.Size(222, 20);
		this.cbSetLinkTable.TabIndex = 17;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.BackColor = System.Drawing.Color.FromArgb(235, 236, 239);
		base.ClientSize = new System.Drawing.Size(391, 381);
		base.Controls.Add(this.cbSetLinkTable);
		base.Controls.Add(this.chkSetLinkTable);
		base.Controls.Add(this.cbSetDefaultValue);
		base.Controls.Add(this.chkSetDefaultValue);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btOk);
		base.Controls.Add(this.btDelete);
		base.Controls.Add(this.btNew);
		base.Controls.Add(this.grdDoc);
		base.Controls.Add(this.chkSetValidValue);
		base.Controls.Add(this.calcLength);
		base.Controls.Add(this.txtDescription);
		base.Controls.Add(this.cbStructure);
		base.Controls.Add(this.txtFieldName);
		base.Controls.Add(this.labelLine4);
		base.Controls.Add(this.labelLine3);
		base.Controls.Add(this.labelLine2);
		base.Controls.Add(this.labelLine1);
		base.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
		base.MaximizeBox = false;
		base.Name = "frm_UDFFieldData";
		base.ShowIcon = false;
		this.Text = "Field Data";
		base.Load += new System.EventHandler(this.frm_UDFFieldData_Load);
		((System.ComponentModel.ISupportInitialize)this.txtFieldName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbStructure.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtDescription.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.calcLength.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkSetValidValue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grdDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gvDoc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit2).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkSetDefaultValue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbSetDefaultValue.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.chkSetLinkTable.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cbSetLinkTable.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
