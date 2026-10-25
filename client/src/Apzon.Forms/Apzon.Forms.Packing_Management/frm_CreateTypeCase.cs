using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Apzon.Commons;
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
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Behaviors;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Packing_Management;

public class frm_CreateTypeCase : XtraFormEx
{
	private List<Common.ParameterValue> TrangThai = new List<Common.ParameterValue>();

	private IContainer components;

	private SimpleButton simpleBtn_OK;

	private SimpleButton simpleBtn_Cancel;

	private ContextMenuStrip contextMenuStrip_TypeCase;

	private ToolStripMenuItem addNewRowToolStripMenuItem;

	private ButtonEditCFL bte_TypeCase;

	private LabelControlEx labelControlEx1;

	private LabelControlEx labelControlEx4;

	private LabelControlEx labelControlEx2;

	private TextEditEx textEdit_TypeName;

	private BehaviorManager behaviorManager1;

	private ToolStripMenuItem deleteRowToolStripMenuItem;

	private SimpleButtonEx simpleBtn_Search;

	private DateEditEx textEdit_CreateDate;

	private GridControlEx gridControl_TypeCase;

	private GridViewEx gridView_TypeCase;

	private GridColumnEx TypeCode;

	private GridColumnEx TypeName;

	private GridColumnEx Amount;

	private GridColumnEx Status;

	private RepositoryItemGridLookUpEdit repositoryIGLE_Status;

	private GridView repositoryItemGridLookUpEdit1View;

	private GridColumn gridColumn1;

	private GridColumn gridColumn2;

	private GridColumnEx CreateDate;

	private GridColumnEx UserSign;

	private RepositoryItemGridLookUpEdit repositoryIGLE_UserSign;

	private GridView gridView1;

	private GridColumn USERID;

	private GridColumn U_NAME;

	private new GridColumnEx Height;

	private RepositoryItemCalcEdit repositoryICE_Height;

	private new GridColumnEx Width;

	private RepositoryItemCalcEdit repositoryICE_Width;

	private GridColumnEx Length;

	private RepositoryItemCalcEdit repositoryICE_Length;

	private GridColumnEx Weight;

	private RepositoryItemCalcEdit repositoryICE_Weight;

	private GridColumnEx Remark;

	private string _TypeCode { get; set; }

	private bool _isAddNew { get; set; }

	private bool _isDel { get; set; }

	public frm_CreateTypeCase()
	{
		InitializeComponent();
	}

	private async void frm_CreateTypeCase_Load(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.View;
		textEdit_CreateDate.SetFormatControl(Enums.DisplayFormatType.DateTime);
		try
		{
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CreateTypeCase"))
			{
				DataTable dataSource = await http.GetAsync("GetTypeCase");
				gridControl_TypeCase.DataSource = dataSource;
			}
			using (HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Traceability"))
			{
				DataTable dataSource2 = await http.GetAsync("GetUser");
				repositoryIGLE_UserSign.DataSource = dataSource2;
				repositoryIGLE_UserSign.DisplayMember = "U_NAME";
				repositoryIGLE_UserSign.ValueMember = "USERID";
			}
			RepositoryItemButtonEditCFL properties = bte_TypeCase.Properties;
			properties.HandleChooseItem = (Action<DataRow>)Delegate.Combine(properties.HandleChooseItem, (Action<DataRow>)((DataRow chooseItem) =>
			{
				if (chooseItem != null)
				{
					FormMode = Enums.FormViewModeType.View;
					bte_TypeCase.EditValue = chooseItem["TypeCode"];
					Refresh();
					SEARCH();
				}
			}));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		TrangThai.Add(new Common.ParameterValue("Active", "a"));
		TrangThai.Add(new Common.ParameterValue("Inactive", "i"));
		repositoryIGLE_Status.DataSource = TrangThai;
		repositoryIGLE_Status.DisplayMember = "Name";
		repositoryIGLE_Status.ValueMember = "Value";
		_isAddNew = false;
	}

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			simpleBtn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.View:
			simpleBtn_OK.Text = "OK";
			break;
		case Enums.FormViewModeType.Update:
			simpleBtn_OK.Text = "Update";
			break;
		case Enums.FormViewModeType.Add:
			simpleBtn_OK.Text = "Save";
			break;
		}
	}

	private async void Savedt()
	{
		_ = 1;
		try
		{
			DataTable dataTable = gridControl_TypeCase.DataSourceTable.Clone();
			DataRow dataRow = gridView_TypeCase.GetDataRow(gridView_TypeCase.FocusedRowHandle);
			DataRow dataRow2 = dataTable.NewRow();
			dataRow2.ItemArray = dataRow.ItemArray;
			dataTable.Rows.Add(dataRow2);
			dataTable.TableName = "dtCase";
			DataSet ds = new DataSet
			{
				Tables = { dataTable }
			};
			WinCommons._OpenProcessing();
			using HttpClientHelper<HttpResult> http = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "CreateTypeCase");
			if (FormMode == Enums.FormViewModeType.Add)
			{
				HttpResult httpResult = await http.PostAsync("SaveTCDataSet", ds);
				if (httpResult.Message == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					_isAddNew = false;
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
			if (FormMode == Enums.FormViewModeType.Update && !_isDel)
			{
				HttpResult httpResult2 = await http.PostAsync("SaveTCDataSetUpdate", ds);
				if (httpResult2.Message == "")
				{
					WinCommons.Success(Enums.MessageCode.Success, "");
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult2.Message);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void simpleBtn_OK_Click(object sender, EventArgs e)
	{
		if (FormMode == Enums.FormViewModeType.View)
		{
			Close();
		}
		else if (XtraMessageBox.Show("Save Change", "Confirmation", MessageBoxButtons.YesNo) != DialogResult.No)
		{
			Savedt();
		}
	}

	public async Task SEARCH()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Current);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task REFRESH()
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CreateTypeCase");
			DataTable dataSource = await http.GetAsync("GetTypeCase");
			gridControl_TypeCase.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
	}

	private async Task LoadData(Enums.NavigateType type)
	{
		try
		{
			string typeCode = (string)bte_TypeCase.EditValue;
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CreateTypeCase");
			DataTable dataSource = await http.GetAsync("TypeCaseGetDataDoc", new
			{
				type = type,
				TypeCode = typeCode
			});
			gridControl_TypeCase.DataSource = dataSource;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void ADDNEW()
	{
		FormMode = Enums.FormViewModeType.View;
		try
		{
			int focusedRowHandle = gridView_TypeCase.RowCount - 1;
			gridView_TypeCase.FocusedRowHandle = focusedRowHandle;
			_isAddNew = true;
			gridView_TypeCase.AddNewRow();
			gridView_TypeCase.SetFocusedRowCellValue("USERID", GlobalData.UserID);
			gridView_TypeCase.SetFocusedRowCellValue("CreateDate", DateTime.Now);
			gridView_TypeCase.SetFocusedRowCellValue("Status", "a");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			await REFRESH();
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
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Previous);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_LAST()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Last);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.Next);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update) && XtraMessageBox.Show("Data were reset at all, do you want to continue?", "System message", MessageBoxButtons.YesNo) == DialogResult.No)
			{
				return;
			}
			WinCommons._OpenProcessing();
			FormMode = Enums.FormViewModeType.Busy;
			await LoadData(Enums.NavigateType.First);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		FormMode = Enums.FormViewModeType.View;
		WinCommons._CloseProcessing();
	}

	private void gridView_TypeCase_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				FormMode = Enums.FormViewModeType.Update;
			}
			if (FormMode == Enums.FormViewModeType.Add)
			{
				simpleBtn_OK.Text = "Save";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gridView_TypeCase_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
	}

	private void frm_CreateTypeCase_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
			{
				if (XtraMessageBox.Show("Do you want to save?", "System Message", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					Savedt();
					e.Cancel = true;
				}
				else
				{
					e.Cancel = true;
				}
			}
			e.Cancel = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void simpleBtn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void gridView_TypeCase_BeforeLeaveRow(object sender, RowAllowEventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update || FormMode == Enums.FormViewModeType.Add)
			{
				if (XtraMessageBox.Show("Do you want to Save?", "System message", MessageBoxButtons.YesNo) == DialogResult.Yes)
				{
					Savedt();
				}
				else
				{
					FormMode = Enums.FormViewModeType.View;
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void addNewRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		ADDNEW();
	}

	private async void deleteRowToolStripMenuItem_Click_1(object sender, EventArgs e)
	{
		try
		{
			string typeCode = Function.ToString(gridView_TypeCase.GetRowCellValue(gridView_TypeCase.FocusedRowHandle, "TypeCode"));
			if (XtraMessageBox.Show(Resources.xtraMsg_Delete, "", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				return;
			}
			WinCommons._OpenProcessing();
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "CreateTypeCase");
			string text = await http.GetAsync("DelTC", new
			{
				TypeCode = typeCode
			});
			if (string.IsNullOrWhiteSpace(text))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
				REFRESH();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void gridView_TypeCase_ShowingEditor(object sender, CancelEventArgs e)
	{
		try
		{
			_ = gridControl_TypeCase.DataSourceTable;
			if (gridView_TypeCase.FocusedColumn.FieldName == TypeCode.FieldName && !_isAddNew)
			{
				e.Cancel = true;
			}
			if (gridView_TypeCase.FocusedColumn.FieldName == TypeName.FieldName && !_isAddNew)
			{
				e.Cancel = true;
			}
			if (gridView_TypeCase.FocusedColumn.FieldName == Amount.FieldName && !_isAddNew)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void simpleBtn_Search_Click_1(object sender, EventArgs e)
	{
		try
		{
			string text = bte_TypeCase.Text.Trim();
			if (text == "")
			{
				text = "-1";
			}
			string text2 = textEdit_TypeName.Text.Trim();
			if (text2 == "")
			{
				text2 = "-1";
			}
			DateTime? createDate = Function.ParseDateTime(textEdit_CreateDate.EditValue);
			if (!createDate.HasValue)
			{
				createDate = Function.ParseDateTime("01/01/1990 12:00:00 AM");
			}
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CreateTypeCase");
			DataTable dataSource = await http.GetAsync("Search", new
			{
				TypeCode = text,
				TypeName = text2,
				CreateDate = createDate,
				Amount = Amount
			});
			gridControl_TypeCase.DataSource = dataSource;
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Packing_Management.frm_CreateTypeCase));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.simpleBtn_OK = new DevExpress.XtraEditors.SimpleButton();
		this.simpleBtn_Cancel = new DevExpress.XtraEditors.SimpleButton();
		this.contextMenuStrip_TypeCase = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.addNewRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.deleteRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.bte_TypeCase = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.labelControlEx1 = new Apzon.Controls.Controls.LabelControlEx();
		this.textEdit_TypeName = new Apzon.Controls.Editor.TextEditEx();
		this.labelControlEx2 = new Apzon.Controls.Controls.LabelControlEx();
		this.labelControlEx4 = new Apzon.Controls.Controls.LabelControlEx();
		this.behaviorManager1 = new DevExpress.Utils.Behaviors.BehaviorManager(this.components);
		this.simpleBtn_Search = new Apzon.Controls.Controls.SimpleButtonEx();
		this.textEdit_CreateDate = new Apzon.Controls.Editor.DateEditEx();
		this.repositoryIGLE_Status = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
		this.repositoryItemGridLookUpEdit1View = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.gridColumn1 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.gridColumn2 = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryIGLE_UserSign = new DevExpress.XtraEditors.Repository.RepositoryItemGridLookUpEdit();
		this.gridView1 = new DevExpress.XtraGrid.Views.Grid.GridView();
		this.USERID = new DevExpress.XtraGrid.Columns.GridColumn();
		this.U_NAME = new DevExpress.XtraGrid.Columns.GridColumn();
		this.repositoryICE_Height = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.repositoryICE_Width = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.repositoryICE_Length = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.repositoryICE_Weight = new DevExpress.XtraEditors.Repository.RepositoryItemCalcEdit();
		this.gridView_TypeCase = new Apzon.Controls.Grid.GridViewEx();
		this.TypeCode = new Apzon.Controls.Grid.GridColumnEx();
		this.TypeName = new Apzon.Controls.Grid.GridColumnEx();
		this.Amount = new Apzon.Controls.Grid.GridColumnEx();
		this.Status = new Apzon.Controls.Grid.GridColumnEx();
		this.CreateDate = new Apzon.Controls.Grid.GridColumnEx();
		this.UserSign = new Apzon.Controls.Grid.GridColumnEx();
		this.Height = new Apzon.Controls.Grid.GridColumnEx();
		this.Width = new Apzon.Controls.Grid.GridColumnEx();
		this.Length = new Apzon.Controls.Grid.GridColumnEx();
		this.Weight = new Apzon.Controls.Grid.GridColumnEx();
		this.Remark = new Apzon.Controls.Grid.GridColumnEx();
		this.gridControl_TypeCase = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip_TypeCase.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.bte_TypeCase.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit_TypeName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit_CreateDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit_CreateDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryIGLE_Status).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1View).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryIGLE_UserSign).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryICE_Height).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryICE_Width).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryICE_Length).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryICE_Weight).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridView_TypeCase).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl_TypeCase).BeginInit();
		base.SuspendLayout();
		this.simpleBtn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleBtn_OK.ImageOptions.ImageUri.Uri = "Apply;Size16x16";
		this.simpleBtn_OK.Location = new System.Drawing.Point(652, 256);
		this.simpleBtn_OK.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.simpleBtn_OK.Name = "simpleBtn_OK";
		this.simpleBtn_OK.Size = new System.Drawing.Size(80, 27);
		this.simpleBtn_OK.TabIndex = 1;
		this.simpleBtn_OK.Text = "OK";
		this.simpleBtn_OK.Click += new System.EventHandler(this.simpleBtn_OK_Click);
		this.simpleBtn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.simpleBtn_Cancel.ImageOptions.ImageUri.Uri = "Cancel;Size16x16";
		this.simpleBtn_Cancel.Location = new System.Drawing.Point(736, 256);
		this.simpleBtn_Cancel.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.simpleBtn_Cancel.Name = "simpleBtn_Cancel";
		this.simpleBtn_Cancel.Size = new System.Drawing.Size(78, 27);
		this.simpleBtn_Cancel.TabIndex = 2;
		this.simpleBtn_Cancel.Text = "Cancel";
		this.simpleBtn_Cancel.Click += new System.EventHandler(this.simpleBtn_Cancel_Click);
		this.contextMenuStrip_TypeCase.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.addNewRowToolStripMenuItem, this.deleteRowToolStripMenuItem });
		this.contextMenuStrip_TypeCase.Name = "contextMenuStrip_TypeCase";
		this.contextMenuStrip_TypeCase.Size = new System.Drawing.Size(150, 48);
		this.addNewRowToolStripMenuItem.Name = "addNewRowToolStripMenuItem";
		this.addNewRowToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
		this.addNewRowToolStripMenuItem.Text = "Add New Row";
		this.addNewRowToolStripMenuItem.Click += new System.EventHandler(this.addNewRowToolStripMenuItem_Click);
		this.deleteRowToolStripMenuItem.Name = "deleteRowToolStripMenuItem";
		this.deleteRowToolStripMenuItem.Size = new System.Drawing.Size(149, 22);
		this.deleteRowToolStripMenuItem.Text = "Delete Row";
		this.deleteRowToolStripMenuItem.Click += new System.EventHandler(this.deleteRowToolStripMenuItem_Click_1);
		this.bte_TypeCase.EditControl = null;
		this.bte_TypeCase.FieldName = "TypeCode";
		this.bte_TypeCase.IsSearchControl = false;
		this.bte_TypeCase.Location = new System.Drawing.Point(64, 16);
		this.bte_TypeCase.Name = "bte_TypeCase";
		this.bte_TypeCase.Properties.AutoLoadDataSource = true;
		this.bte_TypeCase.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.bte_TypeCase.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_TypeCase.Properties.ContainerControl = this;
		this.bte_TypeCase.Properties.DisplayMember = "";
		this.bte_TypeCase.Properties.DisplayMember2 = "";
		this.bte_TypeCase.Properties.LoadDataSourceWhenEnter = false;
		this.bte_TypeCase.Properties.SeparateCharacterForMulti = ';';
		this.bte_TypeCase.Properties.SeparateString = ", ";
		this.bte_TypeCase.Properties.TypeCFL = Apzon.Commons.Enums.Enums.TypeCFL.GetTypeCase;
		this.bte_TypeCase.Properties.UsingAutoCompleteBox = false;
		this.bte_TypeCase.Properties.UsingDataSource = true;
		this.bte_TypeCase.Properties.ValidateWhenNoDataSource = false;
		this.bte_TypeCase.Properties.ValueMember = "";
		this.bte_TypeCase.SettingInVisible = false;
		this.bte_TypeCase.SettingReadonly = false;
		this.bte_TypeCase.Size = new System.Drawing.Size(100, 22);
		this.bte_TypeCase.TabIndex = 5;
		this.labelControlEx1.EditControl = null;
		this.labelControlEx1.EditValue = "Type Code";
		this.labelControlEx1.FieldName = null;
		this.labelControlEx1.Location = new System.Drawing.Point(6, 19);
		this.labelControlEx1.Name = "labelControlEx1";
		this.labelControlEx1.ReadOnly = false;
		this.labelControlEx1.SettingInVisible = false;
		this.labelControlEx1.SettingReadonly = false;
		this.labelControlEx1.Size = new System.Drawing.Size(52, 13);
		this.labelControlEx1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx1.TabIndex = 6;
		this.labelControlEx1.Text = "Type Code";
		this.textEdit_TypeName.EditControl = null;
		this.textEdit_TypeName.FieldName = "TypeName";
		this.textEdit_TypeName.IsSearchControl = false;
		this.textEdit_TypeName.Location = new System.Drawing.Point(275, 16);
		this.textEdit_TypeName.Name = "textEdit_TypeName";
		this.textEdit_TypeName.Properties.BorderStyle = DevExpress.XtraEditors.Controls.BorderStyles.HotFlat;
		this.textEdit_TypeName.SettingInVisible = false;
		this.textEdit_TypeName.SettingReadonly = false;
		this.textEdit_TypeName.Size = new System.Drawing.Size(100, 22);
		this.textEdit_TypeName.TabIndex = 7;
		this.labelControlEx2.EditControl = null;
		this.labelControlEx2.EditValue = "Type Name";
		this.labelControlEx2.FieldName = "";
		this.labelControlEx2.Location = new System.Drawing.Point(217, 23);
		this.labelControlEx2.Name = "labelControlEx2";
		this.labelControlEx2.ReadOnly = false;
		this.labelControlEx2.SettingInVisible = false;
		this.labelControlEx2.SettingReadonly = false;
		this.labelControlEx2.Size = new System.Drawing.Size(54, 13);
		this.labelControlEx2.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx2.TabIndex = 8;
		this.labelControlEx2.Text = "Type Name";
		this.labelControlEx4.EditControl = null;
		this.labelControlEx4.EditValue = "Create Date";
		this.labelControlEx4.FieldName = null;
		this.labelControlEx4.Location = new System.Drawing.Point(428, 23);
		this.labelControlEx4.Name = "labelControlEx4";
		this.labelControlEx4.ReadOnly = false;
		this.labelControlEx4.SettingInVisible = false;
		this.labelControlEx4.SettingReadonly = false;
		this.labelControlEx4.Size = new System.Drawing.Size(59, 13);
		this.labelControlEx4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelControlEx4.TabIndex = 12;
		this.labelControlEx4.Text = "Create Date";
		this.simpleBtn_Search.EditControl = null;
		this.simpleBtn_Search.EditValue = "Search";
		this.simpleBtn_Search.FieldName = null;
		this.simpleBtn_Search.ImageOptions.ImageUri.Uri = "Zoom;Size16x16";
		this.simpleBtn_Search.IsSearchControl = false;
		this.simpleBtn_Search.Location = new System.Drawing.Point(738, 13);
		this.simpleBtn_Search.Name = "simpleBtn_Search";
		this.simpleBtn_Search.ReadOnly = false;
		this.simpleBtn_Search.SettingInVisible = false;
		this.simpleBtn_Search.SettingReadonly = false;
		this.simpleBtn_Search.Size = new System.Drawing.Size(78, 23);
		this.simpleBtn_Search.TabIndex = 14;
		this.simpleBtn_Search.Text = "Search";
		this.simpleBtn_Search.Click += new System.EventHandler(this.simpleBtn_Search_Click_1);
		this.textEdit_CreateDate.EditControl = null;
		this.textEdit_CreateDate.EditValue = null;
		this.textEdit_CreateDate.FieldName = null;
		this.textEdit_CreateDate.Location = new System.Drawing.Point(494, 19);
		this.textEdit_CreateDate.Name = "textEdit_CreateDate";
		this.textEdit_CreateDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit_CreateDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.textEdit_CreateDate.Properties.IsSearchControl = false;
		this.textEdit_CreateDate.SettingInVisible = false;
		this.textEdit_CreateDate.SettingReadonly = false;
		this.textEdit_CreateDate.Size = new System.Drawing.Size(100, 20);
		this.textEdit_CreateDate.TabIndex = 13;
		this.repositoryIGLE_Status.AutoHeight = false;
		this.repositoryIGLE_Status.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryIGLE_Status.Name = "repositoryIGLE_Status";
		this.repositoryIGLE_Status.NullText = "";
		this.repositoryIGLE_Status.PopupView = this.repositoryItemGridLookUpEdit1View;
		this.repositoryItemGridLookUpEdit1View.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.gridColumn1, this.gridColumn2 });
		this.repositoryItemGridLookUpEdit1View.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.repositoryItemGridLookUpEdit1View.Name = "repositoryItemGridLookUpEdit1View";
		this.repositoryItemGridLookUpEdit1View.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.repositoryItemGridLookUpEdit1View.OptionsView.ShowGroupPanel = false;
		this.gridColumn1.Caption = "Name";
		this.gridColumn1.FieldName = "Name";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.OptionsColumn.ShowCaption = false;
		this.gridColumn1.Visible = true;
		this.gridColumn1.VisibleIndex = 0;
		this.gridColumn2.Caption = "Code";
		this.gridColumn2.FieldName = "Code";
		this.gridColumn2.Name = "gridColumn2";
		this.gridColumn2.OptionsColumn.ShowCaption = false;
		this.repositoryIGLE_UserSign.AutoHeight = false;
		this.repositoryIGLE_UserSign.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryIGLE_UserSign.Name = "repositoryIGLE_UserSign";
		this.repositoryIGLE_UserSign.NullText = "";
		this.repositoryIGLE_UserSign.PopupView = this.gridView1;
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[2] { this.USERID, this.U_NAME });
		this.gridView1.FocusRectStyle = DevExpress.XtraGrid.Views.Grid.DrawFocusRectStyle.RowFocus;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsSelection.EnableAppearanceFocusedCell = false;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.USERID.Caption = "USERID";
		this.USERID.FieldName = "USERID";
		this.USERID.Name = "USERID";
		this.USERID.OptionsColumn.ShowCaption = false;
		this.U_NAME.Caption = "U_NAME";
		this.U_NAME.FieldName = "U_NAME";
		this.U_NAME.Name = "U_NAME";
		this.U_NAME.OptionsColumn.ShowCaption = false;
		this.U_NAME.Visible = true;
		this.U_NAME.VisibleIndex = 0;
		this.repositoryICE_Height.AutoHeight = false;
		this.repositoryICE_Height.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryICE_Height.Name = "repositoryICE_Height";
		this.repositoryICE_Width.AutoHeight = false;
		this.repositoryICE_Width.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryICE_Width.Name = "repositoryICE_Width";
		this.repositoryICE_Length.AutoHeight = false;
		this.repositoryICE_Length.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryICE_Length.Name = "repositoryICE_Length";
		this.repositoryICE_Weight.AutoHeight = false;
		this.repositoryICE_Weight.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.repositoryICE_Weight.Name = "repositoryICE_Weight";
		this.gridView_TypeCase.Appearance.FocusedRow.BackColor = System.Drawing.Color.FromArgb(254, 234, 163);
		this.gridView_TypeCase.Appearance.FocusedRow.Options.UseBackColor = true;
		this.gridView_TypeCase.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gridView_TypeCase.ColumnKey = null;
		this.gridView_TypeCase.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[11]
		{
			this.TypeCode, this.TypeName, this.Amount, this.Status, this.CreateDate, this.UserSign, this.Height, this.Width, this.Length, this.Weight,
			this.Remark
		});
		this.gridView_TypeCase.DisableFormSetting = false;
		this.gridView_TypeCase.DisplayException = false;
		this.gridView_TypeCase.EditColumn = null;
		this.gridView_TypeCase.FieldCurrency = null;
		this.gridView_TypeCase.FieldRate = null;
		this.gridView_TypeCase.FormId = "oCreateTypeCase";
		this.gridView_TypeCase.GridControl = this.gridControl_TypeCase;
		this.gridView_TypeCase.IgnoreFilter = false;
		this.gridView_TypeCase.IndexEditRow = -1;
		this.gridView_TypeCase.IndicatorWidth = 35;
		this.gridView_TypeCase.IsApzTable = false;
		this.gridView_TypeCase.IsInsertNewRow = false;
		this.gridView_TypeCase.IsLoadingData = false;
		this.gridView_TypeCase.IsMappingContextAction = false;
		this.gridView_TypeCase.ItemId = "oCreateTypeCaseGrid";
		this.gridView_TypeCase.LstColumnAllowBatch = null;
		this.gridView_TypeCase.LstColumnNotAllowBatch = null;
		this.gridView_TypeCase.LstColumnNotEditable = null;
		this.gridView_TypeCase.LstColumNotSetting = null;
		this.gridView_TypeCase.Name = "gridView_TypeCase";
		this.gridView_TypeCase.OptionsBehavior.AllowFixedGroups = DevExpress.Utils.DefaultBoolean.True;
		this.gridView_TypeCase.OptionsBehavior.AllowPartialGroups = DevExpress.Utils.DefaultBoolean.False;
		this.gridView_TypeCase.OptionsPrint.AutoWidth = false;
		this.gridView_TypeCase.OptionsPrint.UsePrintStyles = false;
		this.gridView_TypeCase.OptionsView.BestFitMaxRowCount = 100;
		this.gridView_TypeCase.OptionsView.ColumnAutoWidth = false;
		this.gridView_TypeCase.OptionsView.ShowButtonMode = DevExpress.XtraGrid.Views.Base.ShowButtonModeEnum.ShowAlways;
		this.gridView_TypeCase.OptionsView.ShowGroupedColumns = true;
		this.gridView_TypeCase.OptionsView.ShowGroupPanel = false;
		this.gridView_TypeCase.UseBatchMod = false;
		this.gridView_TypeCase.ShowingEditor += new System.ComponentModel.CancelEventHandler(this.gridView_TypeCase_ShowingEditor);
		this.gridView_TypeCase.FocusedRowObjectChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowObjectChangedEventHandler(this.gridView_TypeCase_FocusedRowObjectChanged);
		this.gridView_TypeCase.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView_TypeCase_CellValueChanging);
		this.gridView_TypeCase.BeforeLeaveRow += new DevExpress.XtraGrid.Views.Base.RowAllowEventHandler(this.gridView_TypeCase_BeforeLeaveRow);
		this.TypeCode.Caption = "Type Code";
		this.TypeCode.CurrencyDateField = null;
		this.TypeCode.CurrencyField = null;
		this.TypeCode.CurrencyFixed = null;
		this.TypeCode.CurrencyFixedRate = 0.0;
		this.TypeCode.CurrencyRateField = null;
		this.TypeCode.FieldName = "TypeCode";
		this.TypeCode.Name = "TypeCode";
		this.TypeCode.OptionsColumn.FixedWidth = true;
		this.TypeCode.PrimaryKey = true;
		this.TypeCode.UnitField = null;
		this.TypeCode.Visible = true;
		this.TypeCode.VisibleIndex = 0;
		this.TypeName.Caption = "Type Name";
		this.TypeName.CurrencyDateField = null;
		this.TypeName.CurrencyField = null;
		this.TypeName.CurrencyFixed = null;
		this.TypeName.CurrencyFixedRate = 0.0;
		this.TypeName.CurrencyRateField = null;
		this.TypeName.FieldName = "TypeName";
		this.TypeName.Name = "TypeName";
		this.TypeName.OptionsColumn.FixedWidth = true;
		this.TypeName.PrimaryKey = false;
		this.TypeName.UnitField = null;
		this.TypeName.Visible = true;
		this.TypeName.VisibleIndex = 1;
		this.Amount.Caption = "Amount";
		this.Amount.CurrencyDateField = null;
		this.Amount.CurrencyField = null;
		this.Amount.CurrencyFixed = null;
		this.Amount.CurrencyFixedRate = 0.0;
		this.Amount.CurrencyRateField = null;
		this.Amount.FieldName = "Amount";
		this.Amount.Name = "Amount";
		this.Amount.PrimaryKey = false;
		this.Amount.UnitField = null;
		this.Amount.Visible = true;
		this.Amount.VisibleIndex = 2;
		this.Status.Caption = "Status";
		this.Status.ColumnEdit = this.repositoryIGLE_Status;
		this.Status.CurrencyDateField = null;
		this.Status.CurrencyField = null;
		this.Status.CurrencyFixed = null;
		this.Status.CurrencyFixedRate = 0.0;
		this.Status.CurrencyRateField = null;
		this.Status.FieldName = "Status";
		this.Status.Name = "Status";
		this.Status.PrimaryKey = false;
		this.Status.UnitField = null;
		this.Status.Visible = true;
		this.Status.VisibleIndex = 3;
		this.CreateDate.Caption = "Create Date";
		this.CreateDate.CurrencyDateField = null;
		this.CreateDate.CurrencyField = null;
		this.CreateDate.CurrencyFixed = null;
		this.CreateDate.CurrencyFixedRate = 0.0;
		this.CreateDate.CurrencyRateField = null;
		this.CreateDate.FieldName = "CreateDate";
		this.CreateDate.Name = "CreateDate";
		this.CreateDate.OptionsColumn.AllowEdit = false;
		this.CreateDate.PrimaryKey = false;
		this.CreateDate.UnitField = null;
		this.CreateDate.Visible = true;
		this.CreateDate.VisibleIndex = 4;
		this.UserSign.Caption = "User Name";
		this.UserSign.ColumnEdit = this.repositoryIGLE_UserSign;
		this.UserSign.CurrencyDateField = null;
		this.UserSign.CurrencyField = null;
		this.UserSign.CurrencyFixed = null;
		this.UserSign.CurrencyFixedRate = 0.0;
		this.UserSign.CurrencyRateField = null;
		this.UserSign.FieldName = "USERID";
		this.UserSign.Name = "UserSign";
		this.UserSign.OptionsColumn.AllowEdit = false;
		this.UserSign.PrimaryKey = false;
		this.UserSign.UnitField = null;
		this.UserSign.Visible = true;
		this.UserSign.VisibleIndex = 5;
		this.Height.Caption = "Height";
		this.Height.ColumnEdit = this.repositoryICE_Height;
		this.Height.CurrencyDateField = null;
		this.Height.CurrencyField = null;
		this.Height.CurrencyFixed = null;
		this.Height.CurrencyFixedRate = 0.0;
		this.Height.CurrencyRateField = null;
		this.Height.FieldName = "Height";
		this.Height.Name = "Height";
		this.Height.PrimaryKey = false;
		this.Height.UnitField = null;
		this.Height.Visible = true;
		this.Height.VisibleIndex = 6;
		this.Width.Caption = "Width";
		this.Width.ColumnEdit = this.repositoryICE_Width;
		this.Width.CurrencyDateField = null;
		this.Width.CurrencyField = null;
		this.Width.CurrencyFixed = null;
		this.Width.CurrencyFixedRate = 0.0;
		this.Width.CurrencyRateField = null;
		this.Width.FieldName = "Width";
		this.Width.Name = "Width";
		this.Width.PrimaryKey = false;
		this.Width.UnitField = null;
		this.Width.Visible = true;
		this.Width.VisibleIndex = 7;
		this.Length.Caption = "Length";
		this.Length.ColumnEdit = this.repositoryICE_Length;
		this.Length.CurrencyDateField = null;
		this.Length.CurrencyField = null;
		this.Length.CurrencyFixed = null;
		this.Length.CurrencyFixedRate = 0.0;
		this.Length.CurrencyRateField = null;
		this.Length.FieldName = "Length";
		this.Length.Name = "Length";
		this.Length.PrimaryKey = false;
		this.Length.UnitField = null;
		this.Length.Visible = true;
		this.Length.VisibleIndex = 8;
		this.Weight.Caption = "Weight";
		this.Weight.ColumnEdit = this.repositoryICE_Weight;
		this.Weight.CurrencyDateField = null;
		this.Weight.CurrencyField = null;
		this.Weight.CurrencyFixed = null;
		this.Weight.CurrencyFixedRate = 0.0;
		this.Weight.CurrencyRateField = null;
		this.Weight.FieldName = "Weight";
		this.Weight.Name = "Weight";
		this.Weight.PrimaryKey = false;
		this.Weight.UnitField = null;
		this.Weight.Visible = true;
		this.Weight.VisibleIndex = 9;
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
		this.Remark.VisibleIndex = 10;
		this.gridControl_TypeCase.AllowPasteExcel = true;
		this.gridControl_TypeCase.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl_TypeCase.ClipboardData = null;
		this.gridControl_TypeCase.ContextMenuStrip = this.contextMenuStrip_TypeCase;
		this.gridControl_TypeCase.Location = new System.Drawing.Point(6, 49);
		this.gridControl_TypeCase.MainView = this.gridView_TypeCase;
		this.gridControl_TypeCase.Name = "gridControl_TypeCase";
		this.gridControl_TypeCase.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[6] { this.repositoryIGLE_Status, this.repositoryIGLE_UserSign, this.repositoryICE_Height, this.repositoryICE_Width, this.repositoryICE_Length, this.repositoryICE_Weight });
		this.gridControl_TypeCase.Size = new System.Drawing.Size(808, 198);
		this.gridControl_TypeCase.TabIndex = 4;
		this.gridControl_TypeCase.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gridView_TypeCase });
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(826, 294);
		base.Controls.Add(this.simpleBtn_Search);
		base.Controls.Add(this.labelControlEx4);
		base.Controls.Add(this.labelControlEx2);
		base.Controls.Add(this.textEdit_TypeName);
		base.Controls.Add(this.labelControlEx1);
		base.Controls.Add(this.bte_TypeCase);
		base.Controls.Add(this.gridControl_TypeCase);
		base.Controls.Add(this.simpleBtn_Cancel);
		base.Controls.Add(this.simpleBtn_OK);
		base.Controls.Add(this.textEdit_CreateDate);
		base.FirstItemId = "";
		base.FormId = "oCreateTypeCase";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_CreateTypeCase.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		base.Name = "frm_CreateTypeCase";
		this.Text = "Create Type Case";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_CreateTypeCase_FormClosing);
		base.Load += new System.EventHandler(this.frm_CreateTypeCase_Load);
		this.contextMenuStrip_TypeCase.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.bte_TypeCase.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit_TypeName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.behaviorManager1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit_CreateDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.textEdit_CreateDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryIGLE_Status).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemGridLookUpEdit1View).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryIGLE_UserSign).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryICE_Height).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryICE_Width).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryICE_Length).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryICE_Weight).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridView_TypeCase).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl_TypeCase).EndInit();
		base.ResumeLayout(false);
		base.PerformLayout();
	}
}
