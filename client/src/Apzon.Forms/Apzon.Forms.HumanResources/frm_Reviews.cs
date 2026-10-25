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
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Data;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.HumanResources;

public class frm_Reviews : XtraFormEx
{
	private const string FormController = "Reviews";

	private bool isCancel;

	private bool _isLoading;

	private DataTable dtReviews;

	private int lineMax;

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public Action<DataTable> SendFromChildDelegate;

	private IContainer components;

	private GridControlEx gridControl1;

	private GridViewEx gridView1;

	private GridColumnEx line;

	private GridColumnEx date;

	private GridColumnEx reviewDesc;

	private GridColumnEx remarks;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private GridColumnEx grade;

	private RepositoryItemButtonEditCFL bteManager;

	private RepositoryItemDateEditEx rep_date;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btnOK;

	private GridColumnEx gridColumn1;

	private GridColumnEx gcManager;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btnOK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_Reviews()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_HumanResources" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			FormId = "oEmployeeReviews";
			gridView1.FormId = FormId;
			gridView1.ItemId = "HEM3";
			bteManager.ValueMember = "EmpId";
			bteManager.DisplayMember = "lastName";
			bteManager.DisplayMember2 = "firstName";
			bteManager.TypeCFL = Enums.TypeCFL.EmployeeListForSalesEmpSetup;
			bteManager.TypeDrillDown = ApzObjectTypes.oEmployeesInfo;
			FormMode = Enums.FormViewModeType.View;
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	public void GetFromParentMethod(DataTable dt)
	{
		try
		{
			_isLoading = true;
			if (dt.IsNotNull())
			{
				DataView defaultView = dt.DefaultView;
				defaultView.Sort = "date asc";
				dt = defaultView.ToTable();
				lineMax = Function.ParseInt((from s in dt.AsEnumerable()
					where Function.ToString(s.Field<short>("line")) != ""
					select s).Max((DataRow s) => s.Field<short>("line")));
			}
			if (!dt.ExistsColumn("IsNewRow"))
			{
				dt.Columns.Add("IsNewRow", typeof(string));
			}
			foreach (DataRow row in dt.Rows)
			{
				row["IsNewRow"] = "N";
			}
			dtReviews = dt.Copy();
			gridControl1.DataSource = dt;
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				_isLoading = true;
				gridControl1.DataSource = dtReviews.Copy();
				AddNewRow();
				_isLoading = false;
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void CheckSaveState()
	{
		if (!_isLoading)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void AddNewRow()
	{
		try
		{
			gridView1.BeginInsertRow();
			DataTable dataTable = gridControl1.DataSource as DataTable;
			DataRow dataRow = dataTable.NewRow();
			dataRow["line"] = 0;
			dataRow["date"] = DBNull.Value;
			dataRow["IsNewRow"] = "Y";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			gridControl1.DataSource = dataTable;
			gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			gridView1.EndInsertRow();
		}
	}

	private void UpdateData()
	{
		try
		{
			gridView1.ClearSorting();
			string requiredField1 = "date";
			string requiredField2 = "reviewDesc";
			DataTable dataTable = gridControl1.DataSource as DataTable;
			dataTable.AcceptChanges();
			if (dataTable.IsNotNull())
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (!(Function.ToString(item[requiredField1]) != "") || !(Function.ToString(item[requiredField2]) != ""))
					{
						if (!(Function.ToString(item["line"]) == "0"))
						{
							WinCommons.Error(Enums.MessageCode.DateOrDerscriptionFieldCannotBeNull, "");
							gridView1.FocusedRowHandle = dataTable.Rows.IndexOf(item);
							return;
						}
						if (Function.ToString(item["manager"]) != "" || Function.ToString(item["grade"]) != "" || Function.ToString(item["remarks"]) != "")
						{
							WinCommons.Error(Enums.MessageCode.DateOrDerscriptionFieldCannotBeNull, "");
							return;
						}
					}
				}
			}
			DataRow[] array = (from s in dataTable.AsEnumerable()
				where Function.ToString(s.Field<DateTime?>(requiredField1)) != "" && Function.ToString(s.Field<string>(requiredField2)) != ""
				select s).ToArray();
			if (array.Length == 0)
			{
				gridControl1.DataSource = dataTable.Clone();
				SendFromChildDelegate(dataTable.Clone());
			}
			else
			{
				dataTable = array.CopyToDataTable();
				dataTable.AcceptChanges();
				foreach (DataRow item2 in dataTable.AsEnumerable())
				{
					if (Function.ToString(item2["line"]) == "0" && Function.ToString(item2[requiredField1]) != "" && Function.ToString(item2[requiredField2]) != "")
					{
						item2["line"] = lineMax + 1;
						lineMax++;
					}
					if (Function.ParseInt(item2["manager"]) == 0)
					{
						item2["manager"] = DBNull.Value;
					}
				}
				dataTable.AcceptChanges();
				dtReviews = dataTable.Copy();
				gridControl1.DataSource = dataTable.Copy();
				if (dataTable.ExistsColumn("IsNewRow"))
				{
					dataTable.Columns.Remove("IsNewRow");
				}
				SendFromChildDelegate(dataTable.Copy());
			}
			FormMode = Enums.FormViewModeType.View;
			AddNewRow();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnOK_Click(object sender, EventArgs e)
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
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_Reviews_Load(object sender, EventArgs e)
	{
		try
		{
			_isLoading = true;
			RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
			repositoryItemTextEdit.MaxLength = 100;
			gridView1.Columns["reviewDesc"].ColumnEdit = repositoryItemTextEdit;
			RepositoryItemTextEdit repositoryItemTextEdit2 = new RepositoryItemTextEdit();
			repositoryItemTextEdit2.MaxLength = 50;
			gridView1.Columns["grade"].ColumnEdit = repositoryItemTextEdit2;
			_ = gridControl1.DataSource;
			Dictionary<string, GridViewEx> tblFormular = new Dictionary<string, GridViewEx> { ["HEM3"] = gridView1 };
			await WinFuntions.LoadUdfColumns(gridView1, "HEM3", tblFormular);
			AddNewRow();
			bteManager.RefreshDataSource();
			_isLoading = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
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

	private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
		if (!_isLoading)
		{
			string text = Function.ToString(gridView1.GetFocusedRowCellValue("IsNewRow"));
			string text2 = Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "date"));
			string text3 = Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "reviewDesc"));
			if (text == "Y" && text2 != "" && text3 != "" && (e.Column.FieldName == "date" || e.Column.FieldName == "reviewDesc"))
			{
				gridView1.SetFocusedRowCellValue("IsNewRow", "N");
				AddNewRow();
			}
		}
	}

	private void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
	}

	private void frm_Reviews_FormClosing(object sender, FormClosingEventArgs e)
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

	private void btCancel_Click(object sender, EventArgs e)
	{
		isCancel = true;
		Close();
	}

	private void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
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

	private void DrillMethod(string editValue)
	{
		this.DrillDown(ApzObjectTypes.oEmployeesInfo, editValue);
	}

	private void bteManager_ButtonPressed(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			if (e.Button.Index == 1)
			{
				string text = Function.ToString((sender as ButtonEditCFL).EditValue);
				if (!(text == "0") && !(text == ""))
				{
					this.DrillDown(ApzObjectTypes.oEmployeesInfo, ApzObjectTypes.oEmployeesInfo, text);
				}
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_Reviews));
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.line = new Apzon.Controls.Grid.GridColumnEx();
		this.date = new Apzon.Controls.Grid.GridColumnEx();
		this.reviewDesc = new Apzon.Controls.Grid.GridColumnEx();
		this.gcManager = new Apzon.Controls.Grid.GridColumnEx();
		this.bteManager = new Apzon.Forms.UserControls.RepositoryItemButtonEditCFL();
		this.grade = new Apzon.Controls.Grid.GridColumnEx();
		this.remarks = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.rep_date = new Apzon.Controls.Editor.RepositoryItemDateEditEx();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bteManager).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date.CalendarTimeProperties).BeginInit();
		base.SuspendLayout();
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.gridControl1.Location = new System.Drawing.Point(0, 0);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[2] { this.bteManager, this.rep_date });
		this.gridControl1.Size = new System.Drawing.Size(603, 342);
		this.gridControl1.TabIndex = 2;
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
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[7] { this.line, this.date, this.reviewDesc, this.gcManager, this.grade, this.remarks, this.gridColumn1 });
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "oEmployeeReviews";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "HEM3";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
		this.line.Caption = "line";
		this.line.CurrencyDateField = null;
		this.line.CurrencyField = null;
		this.line.CurrencyFixed = null;
		this.line.CurrencyFixedRate = 0.0;
		this.line.CurrencyRateField = null;
		this.line.FieldName = "line";
		this.line.Name = "line";
		this.line.PrimaryKey = false;
		this.line.UnboundType = DevExpress.Data.UnboundColumnType.Integer;
		this.line.UnitField = null;
		this.line.Width = 21;
		this.date.Caption = "Date";
		this.date.CurrencyDateField = null;
		this.date.CurrencyField = null;
		this.date.CurrencyFixed = null;
		this.date.CurrencyFixedRate = 0.0;
		this.date.CurrencyRateField = null;
		this.date.FieldName = "date";
		this.date.Name = "date";
		this.date.PrimaryKey = false;
		this.date.UnitField = null;
		this.date.Visible = true;
		this.date.VisibleIndex = 0;
		this.date.Width = 101;
		this.reviewDesc.Caption = "Description";
		this.reviewDesc.CurrencyDateField = null;
		this.reviewDesc.CurrencyField = null;
		this.reviewDesc.CurrencyFixed = null;
		this.reviewDesc.CurrencyFixedRate = 0.0;
		this.reviewDesc.CurrencyRateField = null;
		this.reviewDesc.FieldName = "reviewDesc";
		this.reviewDesc.Name = "reviewDesc";
		this.reviewDesc.PrimaryKey = false;
		this.reviewDesc.UnitField = null;
		this.reviewDesc.Visible = true;
		this.reviewDesc.VisibleIndex = 1;
		this.reviewDesc.Width = 142;
		this.gcManager.Caption = "Manager";
		this.gcManager.ColumnEdit = this.bteManager;
		this.gcManager.CurrencyDateField = null;
		this.gcManager.CurrencyField = null;
		this.gcManager.CurrencyFixed = null;
		this.gcManager.CurrencyFixedRate = 0.0;
		this.gcManager.CurrencyRateField = null;
		this.gcManager.FieldName = "manager";
		this.gcManager.Name = "gcManager";
		this.gcManager.PrimaryKey = false;
		this.gcManager.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gcManager.UnitField = null;
		this.gcManager.Visible = true;
		this.gcManager.VisibleIndex = 2;
		this.gcManager.Width = 129;
		this.bteManager.AutoHeight = false;
		this.bteManager.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.bteManager.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bteManager.ContainerControl = this;
		this.bteManager.DisplayMember = "";
		this.bteManager.DisplayMember2 = "";
		this.bteManager.LoadDataSourceWhenEnter = false;
		this.bteManager.Name = "bteManager";
		this.bteManager.SeparateCharacterForMulti = ';';
		this.bteManager.SeparateString = ", ";
		this.bteManager.UsingAutoCompleteBox = false;
		this.bteManager.UsingDataSource = true;
		this.bteManager.ValidateWhenNoDataSource = false;
		this.bteManager.ValueMember = "";
		this.grade.Caption = "Grade";
		this.grade.CurrencyDateField = null;
		this.grade.CurrencyField = null;
		this.grade.CurrencyFixed = null;
		this.grade.CurrencyFixedRate = 0.0;
		this.grade.CurrencyRateField = null;
		this.grade.FieldName = "grade";
		this.grade.Name = "grade";
		this.grade.PrimaryKey = false;
		this.grade.UnitField = null;
		this.grade.Visible = true;
		this.grade.VisibleIndex = 3;
		this.grade.Width = 88;
		this.remarks.Caption = "Remarks";
		this.remarks.CurrencyDateField = null;
		this.remarks.CurrencyField = null;
		this.remarks.CurrencyFixed = null;
		this.remarks.CurrencyFixedRate = 0.0;
		this.remarks.CurrencyRateField = null;
		this.remarks.FieldName = "remarks";
		this.remarks.Name = "remarks";
		this.remarks.PrimaryKey = false;
		this.remarks.UnitField = null;
		this.remarks.Visible = true;
		this.remarks.VisibleIndex = 4;
		this.remarks.Width = 116;
		this.gridColumn1.Caption = "gridColumn1";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "IsNewRow";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ReadOnly = true;
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.rep_date.AutoHeight = false;
		this.rep_date.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_date.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.rep_date.IsSearchControl = false;
		this.rep_date.Name = "rep_date";
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(82, 348);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 11;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(4, 348);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 10;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		base.AcceptButton = this.btnOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(603, 375);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.gridControl1);
		base.FormId = "oEmployeeReviews";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_Reviews";
		base.ShowInTaskbar = false;
		this.Text = "Reviews";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_Reviews_FormClosing);
		base.Load += new System.EventHandler(this.frm_Reviews_Load);
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bteManager).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.rep_date).EndInit();
		base.ResumeLayout(false);
	}
}
