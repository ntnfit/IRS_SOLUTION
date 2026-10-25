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
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.HumanResources;

public class frm_JVApplicants : XtraFormEx
{
	private const string FormController = "JVApplicants";

	private bool isCancel;

	private bool _isLoading;

	private bool isSorting;

	private int focusedRow;

	private bool hasValueOnNewRow;

	private RepositoryItemTextEdit applicantNo_Rep = new RepositoryItemTextEdit();

	private DataTable dtOHAP;

	private DataTable dtOHAQ;

	private DataTable dtOHQU;

	public int AppID;

	public string AppNo = "";

	public string FirstName;

	public string MiddleName;

	public string LastName;

	public int conID;

	private IContainer components;

	private GridControlEx gridControl1;

	private GridViewEx gridView1;

	private GridColumnEx ApplicantNo;

	private GridColumnEx DoB;

	private GridColumnEx Gender;

	private GridColumnEx Qualifications;

	private GridColumnEx Address;

	private GridColumnEx Phone;

	private GridColumnEx ApplicantName;

	private GridColumnEx Email;

	private RepositoryItemDateEdit dateRep;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem removeRowToolStripMenuItem;

	private RepositoryItemLookUpEdit genderRep;

	private RepositoryItemButtonEdit qualif_Rep;

	private RepositoryItemTextEdit repositoryItemTextEdit1;

	private GridColumnEx ApplicantID;

	private SimpleButtonEx btCancel;

	private SimpleButtonEx btnOK;

	private GridColumnEx gridColumn1;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btCancel.Enabled = true;
			return;
		}
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

	public frm_JVApplicants()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_JobVacancy"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_JobVacancy" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btCancel.Enabled = true;
			}
			FormMode = Enums.FormViewModeType.View;
			FormId = "oApplicants";
			gridView1.ItemId = "APZ_OHAP";
			gridView1.FormId = FormId;
			return;
		}
		throw new Exception(Apzon.Commons.Properties.Resources.Msg_FormAccessDenied);
	}

	private async void UpdateData()
	{
		try
		{
			gridView1.ClearSorting();
			DataTable obj = gridControl1.DataSource as DataTable;
			obj.AcceptChanges();
			foreach (DataRow item in obj.AsEnumerable())
			{
				if (!(Function.ToString(item["ApplicantNo"]) != "") || !(Function.ToString(item["ApplicantName"]) != "") || !(Function.ToString(item["Qualifications"]) != ""))
				{
					if (!(Function.ToString(item["ApplicantId"]) == "") && !(Function.ToString(item["ApplicantId"]) == "0"))
					{
						WinCommons.Error(Enums.MessageCode.ApplicantNoOrApplicantNameOrQualificationFieldCannotBeNull, "");
						return;
					}
					if (Function.ToString(item["ApplicantNo"]) != "" || Function.ToString(item["ApplicantName"]) != "" || Function.ToString(item["Qualifications"]) != "" || Function.ToString(item["DoB"]) != "" || Function.ParseInt(item["Gender"]) != 0 || Function.ToString(item["Address"]) != "" || Function.ToString(item["Phone"]) != "" || Function.ToString(item["Email"]) != "")
					{
						WinCommons.Error(Enums.MessageCode.ApplicantNoOrApplicantNameOrQualificationFieldCannotBeNull, "");
						return;
					}
				}
			}
			if (!(await SaveData() != "success"))
			{
				FormMode = Enums.FormViewModeType.View;
				gridView1.FocusedRowHandle = focusedRow;
				AddNewRow();
			}
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

	private void frm_JVApplicants_FormClosing(object sender, FormClosingEventArgs e)
	{
		try
		{
			if ((FormMode == Enums.FormViewModeType.Update || hasValueOnNewRow) && !isCancel)
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

	private async Task<DataTable> LoadQualification()
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "HRMQualifications");
			DataTable dataTable = await helper.GetAsync("GetQualifications");
			if (!dataTable.IsNotNull())
			{
				dataTable = WinFuntions.GetTableStructure("APZ_OHQU");
			}
			return dataTable;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public async Task LoadData()
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "JVApplicants");
			DataSet dataSet = await helper.GetAsync("LoadDoc", new
			{
				id = AppID,
				appNo = AppNo
			});
			if (dataSet.Tables.Count > 0)
			{
				if (dataSet.Tables[0].IsNotNull())
				{
					dtOHAP = dataSet.Tables[0];
				}
				else
				{
					dtOHAP = WinFuntions.GetTableStructure("APZ_OHAP");
				}
				if (dataSet.Tables[1].IsNotNull())
				{
					dtOHAQ = dataSet.Tables[1];
				}
				else
				{
					dtOHAQ = WinFuntions.GetTableStructure("APZ_OHAQ");
				}
				if (!dtOHAP.ExistsColumn("IsNewRow"))
				{
					dtOHAP.Columns.Add("IsNewRow", typeof(string));
				}
				foreach (DataRow item in dtOHAP.AsEnumerable())
				{
					int num = 0;
					foreach (DataRow item2 in dtOHAQ.AsEnumerable())
					{
						if (Function.ToString(item["ApplicantID"]) == Function.ToString(item2["ApplicantID"]))
						{
							if (num > 0)
							{
								item["Qualifications"] = Function.ToString(item["Qualifications"]) + ";" + Function.ToString(item2["QuaCode"]);
							}
							else
							{
								item["Qualifications"] = Function.ToString(item2["QuaCode"]);
							}
							num++;
						}
					}
					item["IsNewRow"] = "N";
				}
			}
			gridControl1.DataSource = dtOHAP.Copy();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void GenderLookUp()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(int));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = 1;
			dataRow["Name"] = "Male";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 2;
			dataRow["Name"] = "Female";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			dataRow = dataTable.NewRow();
			dataRow["Code"] = 3;
			dataRow["Name"] = "";
			dataTable.Rows.Add(dataRow);
			dataTable.AcceptChanges();
			genderRep.DataSource = dataTable;
			genderRep.BestFitMode = BestFitMode.BestFitResizePopup;
			genderRep.NullText = "";
			genderRep.ValueMember = "Code";
			genderRep.DisplayMember = "Name";
			genderRep.Columns["Code"].Visible = false;
			genderRep.ShowHeader = false;
			genderRep.DropDownRows = 3;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SetupQualification()
	{
		RepositoryItemCheckedComboBoxEdit qualif_CheckRep = new RepositoryItemCheckedComboBoxEdit();
		dtOHQU = await LoadQualification();
		foreach (DataRow item in dtOHQU.AsEnumerable())
		{
			qualif_CheckRep.Items.Add(Function.ToString(item["QuaCode"]), Function.ToString(item["Name"]), CheckState.Unchecked, enabled: true);
		}
		qualif_CheckRep.SeparatorChar = ';';
		gridView1.Columns["Qualifications"].ColumnEdit = qualif_CheckRep;
	}

	private async Task SetupControls()
	{
		RepositoryItemDateEdit columnEdit = new RepositoryItemDateEdit();
		gridView1.Columns["DoB"].ColumnEdit = columnEdit;
		applicantNo_Rep.MaxLength = 50;
		gridView1.Columns["ApplicantNo"].ColumnEdit = applicantNo_Rep;
		RepositoryItemTextEdit repositoryItemTextEdit = new RepositoryItemTextEdit();
		repositoryItemTextEdit.MaxLength = 100;
		gridView1.Columns["ApplicantName"].ColumnEdit = repositoryItemTextEdit;
		RepositoryItemTextEdit repositoryItemTextEdit2 = new RepositoryItemTextEdit();
		repositoryItemTextEdit2.MaxLength = 100;
		gridView1.Columns["Address"].ColumnEdit = repositoryItemTextEdit2;
		RepositoryItemTextEdit repositoryItemTextEdit3 = new RepositoryItemTextEdit();
		repositoryItemTextEdit3.MaxLength = 50;
		gridView1.Columns["Phone"].ColumnEdit = repositoryItemTextEdit3;
		RepositoryItemTextEdit repositoryItemTextEdit4 = new RepositoryItemTextEdit();
		repositoryItemTextEdit4.MaxLength = 50;
		gridView1.Columns["Email"].ColumnEdit = repositoryItemTextEdit4;
		await SetupQualification();
	}

	public async Task REFRESH_DATA()
	{
		_ = 1;
		try
		{
			if (FormMode != Enums.FormViewModeType.Update || XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1) != DialogResult.No)
			{
				_isLoading = true;
				await SetupQualification();
				await LoadData();
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

	private async void frm_JVApplicants_Load(object sender, EventArgs e)
	{
		_ = 2;
		try
		{
			gridView1.OptionsCustomization.AllowGroup = false;
			await SetupControls();
			_isLoading = true;
			await LoadData();
			GenderLookUp();
			AddNewRow();
			Dictionary<string, GridViewEx> dictionary = new Dictionary<string, GridViewEx> { ["APZ_OHAP"] = gridView1 };
			dictionary["APZ_OHAP"] = gridView1;
			await WinFuntions.LoadUdfColumns(gridView1, "APZ_OHAP", dictionary);
			_isLoading = false;
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
			DataTable dataTable2 = dtOHAQ.Clone();
			dataTable.AcceptChanges();
			string requiredField = "ApplicantName";
			if (dataTable.IsNotNull())
			{
				foreach (DataRow item in dataTable.AsEnumerable())
				{
					if (Function.ToString(item["Qualifications"]) != "")
					{
						string text = Function.ToString(item["Qualifications"]);
						item["Qualifications"] = text.Replace("; ", ";");
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
				if (dataTable.ExistsColumn("IsNewRow"))
				{
					dataTable.Columns.Remove("IsNewRow");
				}
				DataSet dataSet = new DataSet();
				dataTable.TableName = "APZ_OHAP";
				dataTable2.TableName = "APZ_OHAQ";
				using (HttpClientHelper<HttpResult> helper = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "JVApplicants"))
				{
					int num = helper.GetInt("GetMaxIdApplicant");
					foreach (DataRow row in dataTable.Rows)
					{
						if (string.IsNullOrEmpty(Function.ToString(row["ApplicantID"])))
						{
							row["ApplicantID"] = num + 1;
							num++;
						}
					}
					dataTable.AcceptChanges();
					dataSet.Tables.Add(dataTable);
					WinCommons._OpenProcessing();
					HttpResult httpResult = helper.Post("Save", dataSet);
					WinCommons._CloseProcessing();
					if (httpResult != null && string.IsNullOrEmpty(httpResult.Message))
					{
						focusedRow = gridView1.FocusedRowHandle;
						await LoadData();
						WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
						return "success";
					}
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
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

	private void AddNewRow()
	{
		try
		{
			gridView1.BeginInsertRow();
			if (AppID == 0 && string.IsNullOrEmpty(AppNo))
			{
				DataTable dataTable = gridControl1.DataSource as DataTable;
				if (!dataTable.AsEnumerable().Any((DataRow s) => Function.ToString(s["IsNewRow"]) == "Y"))
				{
					DataRow dataRow = dataTable.NewRow();
					dataRow["ApplicantID"] = DBNull.Value;
					dataRow["ApplicantNo"] = "";
					dataRow["ApplicantName"] = "";
					dataRow["DoB"] = DBNull.Value;
					dataRow["Gender"] = DBNull.Value;
					dataRow["Qualifications"] = "";
					dataRow["Address"] = "";
					dataRow["Phone"] = "";
					dataRow["Email"] = "";
					dataRow["IsNewRow"] = "Y";
					dataTable.Rows.Add(dataRow);
					dataTable.AcceptChanges();
					gridControl1.DataSource = dataTable;
					gridView1.MakeRowVisible(gridView1.DataRowCount - 1);
				}
			}
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

	private void CheckSaveState()
	{
		if (!_isLoading)
		{
			FormMode = Enums.FormViewModeType.Update;
		}
	}

	private void gridView1_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		CheckSaveState();
	}

	private void gridView1_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		if (!_isLoading)
		{
			string text = Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "IsNewRow"));
			string text2 = Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "ApplicantNo"));
			string text3 = Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "ApplicantName"));
			string text4 = Function.ToString(gridView1.GetRowCellValue(e.RowHandle, "Qualifications"));
			if (text == "Y" && text2 != "" && text3 != "" && text4 != "" && (e.Column.FieldName == "ApplicantNo" || e.Column.FieldName == "ApplicantName" || e.Column.FieldName == "Qualifications"))
			{
				gridView1.SetRowCellValue(e.RowHandle, "IsNewRow", "N");
				AddNewRow();
			}
			CheckSaveState();
		}
	}

	private void gridView1_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void gridView1_StartSorting(object sender, EventArgs e)
	{
		isSorting = true;
	}

	private void gridView1_EndSorting(object sender, EventArgs e)
	{
		isSorting = false;
	}

	private async void removeRowToolStripMenuItem_Click(object sender, EventArgs e)
	{
		try
		{
			if (gridView1.DataRowCount <= 0)
			{
				return;
			}
			if ((gridControl1.DataSource as DataTable).IsNotNull())
			{
				int num = Function.ParseInt(gridView1.GetFocusedRowCellValue("ApplicantID"));
				if (num > 0)
				{
					using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "JVApplicants");
					if ((await helper.GetAsync("CheckApplicant", new
					{
						ID = num
					})).IsNotNull())
					{
						WinCommons.Error(Enums.MessageCode.CannotDeleteThisRecordIsBeingUsed, "");
						return;
					}
				}
				else
				{
					AddNewRow();
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

	private void gridView1_FocusedRowChanged(object sender, FocusedRowChangedEventArgs e)
	{
	}

	private void gridControl1_EditorKeyDown(object sender, KeyEventArgs e)
	{
		if (e.KeyCode == Keys.Return)
		{
			gridView1.PostEditor();
			UpdateData();
		}
	}

	private void gridView1_ColumnFilterChanged(object sender, EventArgs e)
	{
	}

	private void gridView1_ShownEditor(object sender, EventArgs e)
	{
		try
		{
			GridView gridView = sender as GridView;
			if (gridView.FocusedColumn.FieldName == "ApplicantNo" && Function.ParseInt(gridView.GetFocusedRowCellValue("ApplicantID")) != 0)
			{
				gridView.ActiveEditor.Properties.ReadOnly = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.HumanResources.frm_JVApplicants));
		this.dateRep = new DevExpress.XtraEditors.Repository.RepositoryItemDateEdit();
		this.gridControl1 = new Apzon.Controls.Grid.GridControlEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.removeRowToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.gridView1 = new Apzon.Controls.Grid.GridViewEx();
		this.ApplicantID = new Apzon.Controls.Grid.GridColumnEx();
		this.ApplicantNo = new Apzon.Controls.Grid.GridColumnEx();
		this.ApplicantName = new Apzon.Controls.Grid.GridColumnEx();
		this.DoB = new Apzon.Controls.Grid.GridColumnEx();
		this.Gender = new Apzon.Controls.Grid.GridColumnEx();
		this.genderRep = new DevExpress.XtraEditors.Repository.RepositoryItemLookUpEdit();
		this.Qualifications = new Apzon.Controls.Grid.GridColumnEx();
		this.Address = new Apzon.Controls.Grid.GridColumnEx();
		this.Phone = new Apzon.Controls.Grid.GridColumnEx();
		this.Email = new Apzon.Controls.Grid.GridColumnEx();
		this.gridColumn1 = new Apzon.Controls.Grid.GridColumnEx();
		this.qualif_Rep = new DevExpress.XtraEditors.Repository.RepositoryItemButtonEdit();
		this.repositoryItemTextEdit1 = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.btCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnOK = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.dateRep).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.dateRep.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.gridView1).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.genderRep).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.qualif_Rep).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).BeginInit();
		base.SuspendLayout();
		this.dateRep.AutoHeight = false;
		this.dateRep.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateRep.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.dateRep.Name = "dateRep";
		this.gridControl1.AllowPasteExcel = true;
		this.gridControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.gridControl1.ClipboardData = null;
		this.gridControl1.ContextMenuStrip = this.contextMenuStrip1;
		this.gridControl1.Location = new System.Drawing.Point(0, 0);
		this.gridControl1.MainView = this.gridView1;
		this.gridControl1.Name = "gridControl1";
		this.gridControl1.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[3] { this.genderRep, this.qualif_Rep, this.repositoryItemTextEdit1 });
		this.gridControl1.Size = new System.Drawing.Size(1094, 323);
		this.gridControl1.TabIndex = 1;
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
		this.gridView1.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[10] { this.ApplicantID, this.ApplicantNo, this.ApplicantName, this.DoB, this.Gender, this.Qualifications, this.Address, this.Phone, this.Email, this.gridColumn1 });
		this.gridView1.DisableFormSetting = false;
		this.gridView1.DisplayException = false;
		this.gridView1.EditColumn = null;
		this.gridView1.FieldCurrency = null;
		this.gridView1.FieldRate = null;
		this.gridView1.FormId = "oApplicants";
		this.gridView1.GridControl = this.gridControl1;
		this.gridView1.IgnoreFilter = false;
		this.gridView1.IndexEditRow = -1;
		this.gridView1.IndicatorWidth = 30;
		this.gridView1.IsApzTable = false;
		this.gridView1.IsInsertNewRow = false;
		this.gridView1.IsLoadingData = false;
		this.gridView1.IsMappingContextAction = false;
		this.gridView1.ItemId = "APZ_OHQU";
		this.gridView1.LstColumnAllowBatch = null;
		this.gridView1.LstColumnNotAllowBatch = null;
		this.gridView1.LstColumnNotEditable = null;
		this.gridView1.LstColumNotSetting = null;
		this.gridView1.Name = "gridView1";
		this.gridView1.OptionsEditForm.ShowOnEnterKey = DevExpress.Utils.DefaultBoolean.False;
		this.gridView1.OptionsSelection.MultiSelect = true;
		this.gridView1.OptionsView.ShowGroupPanel = false;
		this.gridView1.UseBatchMod = false;
		this.gridView1.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gridView1_CustomDrawRowIndicator);
		this.gridView1.ShownEditor += new System.EventHandler(this.gridView1_ShownEditor);
		this.gridView1.StartSorting += new System.EventHandler(this.gridView1_StartSorting);
		this.gridView1.EndSorting += new System.EventHandler(this.gridView1_EndSorting);
		this.gridView1.FocusedRowChanged += new DevExpress.XtraGrid.Views.Base.FocusedRowChangedEventHandler(this.gridView1_FocusedRowChanged);
		this.gridView1.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanged);
		this.gridView1.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gridView1_CellValueChanging);
		this.gridView1.ColumnFilterChanged += new System.EventHandler(this.gridView1_ColumnFilterChanged);
		this.ApplicantID.Caption = "ApplicantID";
		this.ApplicantID.CurrencyDateField = null;
		this.ApplicantID.CurrencyField = null;
		this.ApplicantID.CurrencyFixed = null;
		this.ApplicantID.CurrencyFixedRate = 0.0;
		this.ApplicantID.CurrencyRateField = null;
		this.ApplicantID.FieldName = "ApplicantID";
		this.ApplicantID.Name = "ApplicantID";
		this.ApplicantID.PrimaryKey = false;
		this.ApplicantID.UnitField = null;
		this.ApplicantNo.Caption = "Applicant No.";
		this.ApplicantNo.CurrencyDateField = null;
		this.ApplicantNo.CurrencyField = null;
		this.ApplicantNo.CurrencyFixed = null;
		this.ApplicantNo.CurrencyFixedRate = 0.0;
		this.ApplicantNo.CurrencyRateField = null;
		this.ApplicantNo.FieldName = "ApplicantNo";
		this.ApplicantNo.Name = "ApplicantNo";
		this.ApplicantNo.PrimaryKey = true;
		this.ApplicantNo.UnitField = null;
		this.ApplicantNo.Visible = true;
		this.ApplicantNo.VisibleIndex = 0;
		this.ApplicantNo.Width = 98;
		this.ApplicantName.Caption = "Applicant Name";
		this.ApplicantName.CurrencyDateField = null;
		this.ApplicantName.CurrencyField = null;
		this.ApplicantName.CurrencyFixed = null;
		this.ApplicantName.CurrencyFixedRate = 0.0;
		this.ApplicantName.CurrencyRateField = null;
		this.ApplicantName.FieldName = "ApplicantName";
		this.ApplicantName.Name = "ApplicantName";
		this.ApplicantName.PrimaryKey = false;
		this.ApplicantName.UnitField = null;
		this.ApplicantName.Visible = true;
		this.ApplicantName.VisibleIndex = 1;
		this.ApplicantName.Width = 147;
		this.DoB.Caption = "DoB";
		this.DoB.CurrencyDateField = null;
		this.DoB.CurrencyField = null;
		this.DoB.CurrencyFixed = null;
		this.DoB.CurrencyFixedRate = 0.0;
		this.DoB.CurrencyRateField = null;
		this.DoB.DisplayFormatType = Apzon.Commons.Enums.Enums.DisplayFormatType.DateTime;
		this.DoB.FieldName = "DoB";
		this.DoB.Name = "DoB";
		this.DoB.PrimaryKey = false;
		this.DoB.UnitField = null;
		this.DoB.Visible = true;
		this.DoB.VisibleIndex = 2;
		this.DoB.Width = 82;
		this.Gender.AppearanceHeader.Options.UseTextOptions = true;
		this.Gender.AppearanceHeader.TextOptions.HAlignment = DevExpress.Utils.HorzAlignment.Center;
		this.Gender.Caption = "Gender";
		this.Gender.ColumnEdit = this.genderRep;
		this.Gender.CurrencyDateField = null;
		this.Gender.CurrencyField = null;
		this.Gender.CurrencyFixed = null;
		this.Gender.CurrencyFixedRate = 0.0;
		this.Gender.CurrencyRateField = null;
		this.Gender.FieldName = "Gender";
		this.Gender.Name = "Gender";
		this.Gender.PrimaryKey = false;
		this.Gender.UnitField = null;
		this.Gender.Visible = true;
		this.Gender.VisibleIndex = 3;
		this.Gender.Width = 79;
		this.genderRep.AutoHeight = false;
		this.genderRep.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.genderRep.Columns.AddRange(new DevExpress.XtraEditors.Controls.LookUpColumnInfo[2]
		{
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Code", "Name5"),
			new DevExpress.XtraEditors.Controls.LookUpColumnInfo("Name", "Name6")
		});
		this.genderRep.Name = "genderRep";
		this.genderRep.NullText = "\"\"";
		this.Qualifications.Caption = "Qualifications";
		this.Qualifications.CurrencyDateField = null;
		this.Qualifications.CurrencyField = null;
		this.Qualifications.CurrencyFixed = null;
		this.Qualifications.CurrencyFixedRate = 0.0;
		this.Qualifications.CurrencyRateField = null;
		this.Qualifications.FieldName = "Qualifications";
		this.Qualifications.Name = "Qualifications";
		this.Qualifications.PrimaryKey = false;
		this.Qualifications.UnitField = null;
		this.Qualifications.Visible = true;
		this.Qualifications.VisibleIndex = 4;
		this.Qualifications.Width = 279;
		this.Address.Caption = "Address";
		this.Address.CurrencyDateField = null;
		this.Address.CurrencyField = null;
		this.Address.CurrencyFixed = null;
		this.Address.CurrencyFixedRate = 0.0;
		this.Address.CurrencyRateField = null;
		this.Address.FieldName = "Address";
		this.Address.Name = "Address";
		this.Address.PrimaryKey = false;
		this.Address.UnitField = null;
		this.Address.Visible = true;
		this.Address.VisibleIndex = 5;
		this.Address.Width = 152;
		this.Phone.Caption = "Tel";
		this.Phone.CurrencyDateField = null;
		this.Phone.CurrencyField = null;
		this.Phone.CurrencyFixed = null;
		this.Phone.CurrencyFixedRate = 0.0;
		this.Phone.CurrencyRateField = null;
		this.Phone.FieldName = "Phone";
		this.Phone.Name = "Phone";
		this.Phone.PrimaryKey = false;
		this.Phone.UnitField = null;
		this.Phone.Visible = true;
		this.Phone.VisibleIndex = 6;
		this.Phone.Width = 79;
		this.Email.Caption = "Email";
		this.Email.CurrencyDateField = null;
		this.Email.CurrencyField = null;
		this.Email.CurrencyFixed = null;
		this.Email.CurrencyFixedRate = 0.0;
		this.Email.CurrencyRateField = null;
		this.Email.FieldName = "Email";
		this.Email.Name = "Email";
		this.Email.PrimaryKey = false;
		this.Email.UnitField = null;
		this.Email.Visible = true;
		this.Email.VisibleIndex = 7;
		this.Email.Width = 146;
		this.gridColumn1.Caption = "gridColumn1";
		this.gridColumn1.CurrencyDateField = null;
		this.gridColumn1.CurrencyField = null;
		this.gridColumn1.CurrencyFixed = null;
		this.gridColumn1.CurrencyFixedRate = 0.0;
		this.gridColumn1.CurrencyRateField = null;
		this.gridColumn1.FieldName = "IsNewRow";
		this.gridColumn1.Name = "gridColumn1";
		this.gridColumn1.PrimaryKey = false;
		this.gridColumn1.UnitField = null;
		this.qualif_Rep.AutoHeight = false;
		editorButtonImageOptions.Image = Apzon.Forms.Properties.Resources.right;
		this.qualif_Rep.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.qualif_Rep.Name = "qualif_Rep";
		this.repositoryItemTextEdit1.AutoHeight = false;
		this.repositoryItemTextEdit1.Name = "repositoryItemTextEdit1";
		this.btCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btCancel.EditControl = null;
		this.btCancel.EditValue = "Cancel";
		this.btCancel.FieldName = null;
		this.btCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btCancel.ImageOptions.Image");
		this.btCancel.IsSearchControl = false;
		this.btCancel.Location = new System.Drawing.Point(83, 329);
		this.btCancel.Name = "btCancel";
		this.btCancel.ReadOnly = false;
		this.btCancel.SettingInVisible = false;
		this.btCancel.SettingReadonly = false;
		this.btCancel.Size = new System.Drawing.Size(75, 23);
		this.btCancel.TabIndex = 9;
		this.btCancel.Text = "Cancel";
		this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
		this.btnOK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnOK.EditControl = null;
		this.btnOK.EditValue = "OK";
		this.btnOK.FieldName = null;
		this.btnOK.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnOK.ImageOptions.Image");
		this.btnOK.IsSearchControl = false;
		this.btnOK.Location = new System.Drawing.Point(5, 329);
		this.btnOK.Name = "btnOK";
		this.btnOK.ReadOnly = false;
		this.btnOK.SettingInVisible = false;
		this.btnOK.SettingReadonly = false;
		this.btnOK.Size = new System.Drawing.Size(75, 23);
		this.btnOK.TabIndex = 8;
		this.btnOK.Text = "OK";
		this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
		base.AcceptButton = this.btnOK;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(1094, 355);
		base.Controls.Add(this.btCancel);
		base.Controls.Add(this.btnOK);
		base.Controls.Add(this.gridControl1);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_JVApplicants";
		base.ShowInTaskbar = false;
		this.Text = "Applicants";
		base.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frm_JVApplicants_FormClosing);
		base.Load += new System.EventHandler(this.frm_JVApplicants_Load);
		((System.ComponentModel.ISupportInitialize)this.dateRep.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.dateRep).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gridControl1).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.gridView1).EndInit();
		((System.ComponentModel.ISupportInitialize)this.genderRep).EndInit();
		((System.ComponentModel.ISupportInitialize)this.qualif_Rep).EndInit();
		((System.ComponentModel.ISupportInitialize)this.repositoryItemTextEdit1).EndInit();
		base.ResumeLayout(false);
	}
}
