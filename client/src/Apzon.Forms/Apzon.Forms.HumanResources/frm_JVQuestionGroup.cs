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
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_JVQuestionGroup : XtraFormEx
{
	private List<int> changedCells = new List<int>();

	private const string FormController = "JVQuestion";

	private bool isLoading;

	private bool isCancel;

	private bool isInserted;

	private int _IdMax;

	private int focusedRow;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private int idMax;

	public Action<int> childSend;

	private IContainer components;

	private GridControlEx gridControl1;

	private GridViewEx gridView1;

	private GridColumnEx Id;

	private GridColumnEx name;

	private GridColumnEx Remark;

	private RepositoryItemLookUpEdit repEmpStatus;

	private RepositoryItemTextEdit repCode;

	private RepositoryItemTextEdit repName;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btnOK;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOK.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_JVQuestionGroup()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_JobVacancy"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_JobVacancy" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			FormId = "oJVQuestionGroup";
			gridView1.FormId = FormId;
			gridView1.ItemId = "APZ_OQLG";
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async Task LoadData()
	{
		try
		{
			DataTable dataTable = _httpClient.JVQuestion.LoadQuestionGroup();
			if (!dataTable.HasColumn())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OQLG");
			}
			else
			{
				_IdMax = Function.ParseInt((from s in dataTable.AsEnumerable()
					where Function.ToString(s.Field<int?>("Id")) != ""
					select s).Max((DataRow s) => s.Field<int?>("Id")));
			}
			gridControl1.DataSource = dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<string> SaveData()
	{
		try
		{
			DataTable dataTable = gridControl1.DataSource as DataTable;
			dataTable.AcceptChanges();
			string requiredField = "Name";
			if (dataTable.HasColumn())
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (Function.ToString(item["Id"]) == "" && Function.ToString(item[requiredField]) != "")
					{
						item["Id"] = 0;
						isInserted = true;
					}
				}
				dataTable.AcceptChanges();
				DataRow[] array = (from s in dataTable.AsEnumerable()
					where Function.ToString(s.Field<string>(requiredField)) != ""
					select s).ToArray();
				if (array.Length == 0)
				{
					gridControl1.DataSource = dataTable.Clone();
					dataTable = dataTable.Clone();
					dataTable.AcceptChanges();
				}
				else
				{
					dataTable = array.CopyToDataTable();
					dataTable.AcceptChanges();
				}
				dataTable.TableName = "APZ_OQLG";
				Tuple<DataTable, string> dt = new Tuple<DataTable, string>(dataTable, dataTable.TableName);
				HttpResult httpResult = _httpClient.JVQuestion.SaveQuestion(dt);
				if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
				{
					focusedRow = gridView1.FocusedRowHandle;
					await LoadData();
					WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
					return "success";
				}
				WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				return "";
			}
			WinCommons.Error(Enums.MessageCode.ThisEntryIsNull, "");
			return "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return "";
		}
	}

	private async void UpdateData()
	{
		try
		{
			gridView1.ClearSorting();
			DataTable dataTable = gridControl1.DataSource as DataTable;
			dataTable.AcceptChanges();
			if (dataTable.HasColumn())
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (Function.ToString(item["Name"]) == "")
					{
						if (!(Function.ToString(item["Id"]) == "") && !(Function.ToString(item["Id"]) == "0"))
						{
							WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
							return;
						}
						if (Function.ToString(item["Remark"]) != "")
						{
							WinCommons.Error(Enums.MessageCode.NameCannotBeEmpty, "");
							return;
						}
					}
				}
			}
			if (await SaveData() != "success")
			{
				return;
			}
			FormMode = Enums.FormViewModeType.View;
			gridView1.FocusedRowHandle = focusedRow;
			int obj;
			if (changedCells.Count > 0)
			{
				if (isInserted)
				{
					obj = _IdMax;
					isInserted = false;
				}
				else
				{
					for (int i = 0; i < changedCells.Count; i++)
					{
						int value = Function.ParseInt(gridView1.GetRowCellValue(changedCells[i], "Id"));
						changedCells[i] = value;
					}
					obj = changedCells.Max();
				}
			}
			else
			{
				obj = Function.ParseInt(gridView1.GetFocusedRowCellValue("Id"));
			}
			if (childSend != null)
			{
				childSend(obj);
			}
			gridView1.AddNewRow();
			gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnOK_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
			}
			else
			{
				UpdateData();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	private void frm_JVInterviewStatus_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update && !isCancel)
			{
				switch (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1))
				{
				case DialogResult.Cancel:
					e.Cancel = true;
					break;
				case DialogResult.Yes:
					btnOK_Click(null, null);
					break;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckSaveState()
	{
		if (!isLoading)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!isLoading)
		{
			if (Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "Name")) != "")
			{
				changedCells.Add(e.RowHandle);
			}
			CheckSaveState();
		}
	}

	private void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				isLoading = true;
				await LoadData();
				gridView1.AddNewRow();
				gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
				isLoading = false;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_JVInterviewStatus_Load(object sender, EventArgs e)
	{
		try
		{
			isLoading = true;
			RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
			repositoryItemTextEdit.MaxLength = 254;
			gridView1.Columns["Name"].ColumnEdit = repositoryItemTextEdit;
			await LoadData();
			gridView1.OptionsBehavior.KeepFocusedRowOnUpdate = true;
			gridView1.AddNewRow();
			gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
			isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gridView1.DataRowCount <= 0)
			{
				return;
			}
			DataTable dt = gridControl1.DataSource as DataTable;
			if (dt.HasColumn() && dt.IsNotNull())
			{
				int num = Function.ParseInt(gridView1.GetFocusedRowCellValue("Id"));
				if (num > 0 && _httpClient.JVQuestion.CheckQuestionGroup(num).IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
					return;
				}
			}
			gridView1.BeginUpdate();
			gridView1.DeleteSelectedRows();
			gridView1.EndUpdate();
			FormMode = Enums.FormViewModeType.Update;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void gridControl1_EditorKeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			gridView1.PostEditor();
			UpdateData();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_JVQuestionGroup));
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.Id = new Apzon.Controls.Grid.GridColumnEx();
		this.name = new Apzon.Controls.Grid.GridColumnEx();
		this.Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.repEmpStatus = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.repCode = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.repName = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repEmpStatus).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repCode).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repName).BeginInit();
		base.SuspendLayout();
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(84, 246);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 18;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(6, 246);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 17;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.gridControl1.Location = new System.Drawing.Point(0, 0);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.repEmpStatus, this.repCode, this.repName });
		this.gridControl1.Size = new System.Drawing.Size(512, 240);
		this.gridControl1.TabIndex = 16;
		this.gridControl1.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView1 });
		this.gridControl1.EditorKeyDown += new System.Windows.Forms.KeyEventHandler(this.gridControl1_EditorKeyDown);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.removeRowToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(144, 26);
		this.removeRowToolStripMenuItem.Name = "removeRowToolStripMenuItem";
		this.removeRowToolStripMenuItem.Size = new System.Drawing.Size(143, 22);
		this.removeRowToolStripMenuItem.Text = "Remove Row";
		this.removeRowToolStripMenuItem.Click += new System.EventHandler(this.removeRowToolStripMenuItem_Click);
		this.gridView1.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView1.ColumnKey = null;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[3] { this.Id, this.name, this.Remark });
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "oJobVacancyQuestionGroup";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "APZ_OQLG";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
		this.Id.Caption = "Id";
		this.Id.CurrencyDateField = null;
		this.Id.CurrencyField = null;
		this.Id.CurrencyFixed = null;
		this.Id.CurrencyFixedRate = 0.0;
		this.Id.CurrencyRateField = null;
		this.Id.FieldName = "Id";
		this.Id.Name = "Id";
		this.Id.PrimaryKey = false;
		this.Id.UnitField = null;
		this.name.Caption = "Name";
		this.name.CurrencyDateField = null;
		this.name.CurrencyField = null;
		this.name.CurrencyFixed = null;
		this.name.CurrencyFixedRate = 0.0;
		this.name.CurrencyRateField = null;
		this.name.FieldName = "Name";
		this.name.Name = "name";
		this.name.PrimaryKey = false;
		this.name.UnitField = null;
		this.name.Visible = true;
		this.name.VisibleIndex = 0;
		this.name.Width = 179;
		this.Remark.Caption = "Remark";
		this.Remark.CurrencyDateField = null;
		this.Remark.CurrencyField = null;
		this.Remark.CurrencyFixed = null;
		this.Remark.CurrencyFixedRate = 0.0;
		this.Remark.CurrencyRateField = null;
		this.Remark.FieldName = "Remark";
		this.Remark.Name = "Remark";
		this.Remark.PrimaryKey = false;
		this.Remark.UnitField = null;
		this.Remark.Visible = true;
		this.Remark.VisibleIndex = 1;
		this.Remark.Width = 288;
		this.repEmpStatus.AutoHeight = false;
		this.repEmpStatus.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repEmpStatus.Name = "repEmpStatus";
		this.repCode.AutoHeight = false;
		this.repCode.MaxLength = 20;
		this.repCode.Name = "repCode";
		this.repName.AutoHeight = false;
		this.repName.MaxLength = 100;
		this.repName.Name = "repName";
		base.AcceptButton = this.btnOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(512, 274);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.gridControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_JVQuestionGroup";
		base.ShowInTaskbar = false;
		this.Text = "Question Group";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_JVInterviewStatus_FormClosing);
		base.Load += new System.EventHandler(this.frm_JVInterviewStatus_Load);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repEmpStatus).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repCode).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repName).EndInit();
		base.ResumeLayout(false);
	}
}
