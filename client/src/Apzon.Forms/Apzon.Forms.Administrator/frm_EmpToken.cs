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
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Commons;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid;
using DevExpress.XtraGrid.Columns;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Administrator;

public class frm_EmpToken : XtraFormEx
{
	private IContainer components;

	private GridControlEx grd_Doc;

	private GridViewEx gv_Doc;

	private SimpleButtonEx btn_Save;

	private GridColumnEx colEmpName;

	private GridColumnEx colToken;

	public SimpleButtonEx btn_Author;

	private RepositoryItemTextEdit te_Token;

	public SimpleButtonEx btn_Refresh;

	private ContextMenuStrip contextMenuStrip1;

	private ToolStripMenuItem showPasswordToolStripMenuItem;

	public SimpleButtonEx btn_ShowPass;

	private GridColumnEx colEmpID;

	private GridColumnEx colEmpCode;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_EmpToken()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_EmpToken"))
		{
			InitializeComponent();
			gv_Doc.KeyDown += KeyDownPaste;
			grd_Doc.EditorKeyDown += KeyDownPaste;
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_EmpToken" && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
			}
			FormId = "oEmpToken";
			gv_Doc.ItemId = "APZ_OHET";
			gv_Doc.ItemId = FormId;
			OnLoadAsync += frm_EmpToken_Load;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			DataTable dataTable = await GetData(GlobalData.UserID);
			dataTable.SetDefaultAndMaxLen(gv_Doc.ItemId);
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SaveData(DataTable dt)
	{
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("EmpId", typeof(int));
			dataTable.Columns.Add("TokenStr", typeof(string));
			for (int i = 0; i < dt.Rows.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				if (Function.ToString(dt.Rows[i]["TokenStr"]) == "")
				{
					dataRow["TokenStr"] = "";
					continue;
				}
				dataRow["EmpId"] = Function.ParseInt(dt.Rows[i]["EmpId"]);
				dataRow["TokenStr"] = Function.Encrypt(RSA.PublicKey, Function.ToString(dt.Rows[i]["TokenStr"]));
				dataTable.Rows.Add(dataRow);
			}
			string text;
			using (HttpClientHelper<string> hp = new HttpClientHelper<string>(Configuration.ApiServer, "EmpToken"))
			{
				text = await hp.PostAsync("Save", dataTable);
			}
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
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
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			WinCommons._CloseProcessing();
		}
	}

	private async Task<DataTable> GetData(int userId)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "EmpToken");
			DataTable dataTable = await hp.GetDataTableAsync("GetEmpTokens", new { userId });
			if (dataTable.Columns.Contains("U_OldCode"))
			{
				colEmpCode.Visible = true;
			}
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("EmpId", typeof(int));
			dataTable2.Columns.Add("FullName", typeof(string));
			dataTable2.Columns.Add("TokenStr", typeof(string));
			dataTable2.Columns.Add("U_OldCode", typeof(string));
			for (int i = 0; i < dataTable.Rows.Count; i++)
			{
				DataRow dataRow = dataTable2.NewRow();
				dataRow["EmpId"] = Function.ParseInt(dataTable.Rows[i]["EmpId"]);
				if (dataTable.Columns.Contains("U_OldCode"))
				{
					dataRow["U_OldCode"] = Function.ToString(dataTable.Rows[i]["U_OldCode"]);
				}
				dataRow["FullName"] = Function.ToString(dataTable.Rows[i]["FullName"]);
				if (Function.ToString(dataTable.Rows[i]["TokenStr"]) != "")
				{
					dataRow["TokenStr"] = Function.Decrypt(RSA.PrivateKey, Function.ToString(dataTable.Rows[i]["TokenStr"]));
				}
				else
				{
					dataRow["TokenStr"] = "";
				}
				dataTable2.Rows.Add(dataRow);
			}
			return dataTable2;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			return new DataTable();
		}
	}

	public void KeyDownPaste(object sender, KeyEventArgs e)
	{
		PasteFromExcel(e, grd_Doc, gv_Doc);
	}

	public void PasteFromExcel(KeyEventArgs e, GridControl grd, GridView gv)
	{
		try
		{
			if (e.KeyCode != Keys.V || e.Modifiers != Keys.Control)
			{
				return;
			}
			DataObject dataObject = (DataObject)Clipboard.GetDataObject();
			string text = dataObject.GetData(DataFormats.UnicodeText).ToString();
			object data = text;
			if (dataObject.GetDataPresent(DataFormats.Text))
			{
				DataTable dataFromExcel = GetDataFromExcel(text);
				if (dataFromExcel != null && (dataFromExcel.Rows.Count > 1 || dataFromExcel.Columns.Count > 1))
				{
					data = dataFromExcel.Rows[0][0];
					int num = gv.FocusedRowHandle;
					GridColumn focusedColumn = gv.FocusedColumn;
					int num2 = ((gv.RowCount > dataFromExcel.Rows.Count) ? dataFromExcel.Rows.Count : gv.RowCount);
					int num3 = ((gv.Columns.Count > dataFromExcel.Columns.Count) ? dataFromExcel.Columns.Count : gv.Columns.Count);
					gv.BeginUpdate();
					for (int i = 0; i < num2; i++)
					{
						int colIndex = focusedColumn.VisibleIndex;
						for (int j = 0; j < num3; j++)
						{
							GridColumn gridColumn = gv.Columns.FirstOrDefault((GridColumn t) => t.VisibleIndex == colIndex);
							if (gridColumn != null && gridColumn.GroupIndex < 0)
							{
								gv.SetRowCellValue(num, gridColumn, dataFromExcel.Rows[i][j]);
							}
							colIndex++;
						}
						num++;
					}
					grd.RefreshDataSource();
					gv.EndUpdate();
					Clipboard.Clear();
				}
			}
			Clipboard.SetData(DataFormats.Text, data);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public DataTable GetDataFromExcel(string clipboardData)
	{
		DataTable dataTable = new DataTable();
		try
		{
			List<string> list = new List<string>(clipboardData.Split('\n'));
			bool flag = true;
			if (list.Count > 0 && string.IsNullOrWhiteSpace(list[list.Count - 1]))
			{
				list.RemoveAt(list.Count - 1);
			}
			foreach (string item in list)
			{
				string text = item;
				if (text.EndsWith("\r"))
				{
					text = text.Substring(0, text.Length - "\r".Length);
				}
				string[] array = text.Split('\r', '\t');
				DataRow dataRow = dataTable.NewRow();
				if (flag)
				{
					int num = 0;
					string[] array2 = array;
					foreach (string text2 in array2)
					{
						if (string.IsNullOrWhiteSpace(text2))
						{
							dataTable.Columns.Add($"[BLANK{num}]");
						}
						else if (!dataTable.Columns.Contains(text2))
						{
							dataTable.Columns.Add(text2);
						}
						else
						{
							dataTable.Columns.Add($"Column {num}");
						}
						num++;
					}
					for (int j = 0; j < array.Length && j < dataTable.Columns.Count; j++)
					{
						dataRow[j] = array[j];
					}
					dataTable.Rows.Add(dataRow);
					flag = false;
				}
				else
				{
					for (int k = 0; k < array.Length && k < dataTable.Columns.Count; k++)
					{
						dataRow[k] = array[k];
					}
					dataTable.Rows.Add(dataRow);
				}
			}
			return dataTable;
		}
		catch (Exception)
		{
			return new DataTable();
		}
	}

	private async Task frm_EmpToken_Load(object sender, EventArgs e)
	{
		btn_ShowPass.Text = "Show Token";
		FormMode = Enums.FormViewModeType.View;
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = await GetData(GlobalData.UserID);
			dataTable.SetDefaultAndMaxLen(gv_Doc.ItemId);
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			WinCommons._CloseProcessing();
		}
		WinCommons._CloseProcessing();
	}

	public async void btn_Refresh_Click(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.View;
		try
		{
			WinCommons._OpenProcessing();
			DataTable dataTable = await GetData(GlobalData.UserID);
			dataTable.SetDefaultAndMaxLen(gv_Doc.ItemId);
			grd_Doc.DataSource = dataTable;
			grd_Doc.RefreshDataSource();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
			WinCommons._CloseProcessing();
		}
		WinCommons._CloseProcessing();
	}

	private void btn_Author_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			frm_AuthorToken frm_AuthorToken2 = new frm_AuthorToken();
			frm_AuthorToken2.frmEmp = this;
			frm_AuthorToken2.MdiParent = MdiParent;
			frm_AuthorToken2.Show();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			DataTable dataSourceTable = grd_Doc.DataSourceTable;
			WinCommons._OpenProcessing();
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				btn_Save.Enabled = false;
				btn_Author.Enabled = false;
				btn_Refresh.Enabled = false;
				await SaveData(dataSourceTable);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Save.Enabled = true;
		btn_Author.Enabled = true;
		btn_Refresh.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void gv_Doc_KeyDown(object sender, KeyEventArgs e)
	{
	}

	private void gv_Doc_CellValueChanged(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void gv_Doc_CellValueChanging(object sender, CellValueChangedEventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void gv_Doc_CustomDrawRowIndicator(object sender, RowIndicatorCustomDrawEventArgs e)
	{
		if (e.Info.IsRowIndicator && e.RowHandle >= 0)
		{
			e.Info.DisplayText = (e.RowHandle + 1).ToString();
		}
	}

	private void showPasswordToolStripMenuItem_Click(object sender, EventArgs e)
	{
	}

	private void btnShowPass_Click(object sender, EventArgs e)
	{
		if (btn_ShowPass.Text == "Show Token")
		{
			btn_ShowPass.Text = "Hide Token";
			te_Token.UseSystemPasswordChar = false;
		}
		else
		{
			btn_ShowPass.Text = "Show Token";
			te_Token.UseSystemPasswordChar = true;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.frm_EmpToken));
		this.grd_Doc = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Doc = new Apzon.Controls.Grid.GridViewEx();
		this.colEmpID = new Apzon.Controls.Grid.GridColumnEx();
		this.colEmpCode = new Apzon.Controls.Grid.GridColumnEx();
		this.colEmpName = new Apzon.Controls.Grid.GridColumnEx();
		this.colToken = new Apzon.Controls.Grid.GridColumnEx();
		this.te_Token = new DevExpress.XtraEditors.Repository.RepositoryItemTextEdit();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Author = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.showPasswordToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
		this.btn_ShowPass = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.te_Token).BeginInit();
		this.contextMenuStrip1.SuspendLayout();
		base.SuspendLayout();
		this.grd_Doc.AllowPasteExcel = true;
		this.grd_Doc.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Doc.ClipboardData = null;
		this.grd_Doc.Location = new System.Drawing.Point(2, 2);
		this.grd_Doc.MainView = this.gv_Doc;
		this.grd_Doc.Name = "grd_Doc";
		this.grd_Doc.RepositoryItems.AddRange(new DevExpress.XtraEditors.Repository.RepositoryItem[1] { this.te_Token });
		this.grd_Doc.Size = new System.Drawing.Size(586, 310);
		this.grd_Doc.TabIndex = 0;
		this.grd_Doc.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Doc });
		this.gv_Doc.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Doc.ColumnKey = null;
		this.gv_Doc.Columns.AddRange(new DevExpress.XtraGrid.Columns.GridColumn[4] { this.colEmpID, this.colEmpCode, this.colEmpName, this.colToken });
		this.gv_Doc.DisableFormSetting = false;
		this.gv_Doc.DisplayException = false;
		this.gv_Doc.EditColumn = null;
		this.gv_Doc.FieldCurrency = null;
		this.gv_Doc.FieldRate = null;
		this.gv_Doc.FormId = "oEmployeeToken";
		this.gv_Doc.GridControl = this.grd_Doc;
		this.gv_Doc.IndexEditRow = -1;
		this.gv_Doc.IndicatorWidth = 40;
		this.gv_Doc.IsApzTable = false;
		this.gv_Doc.IsInsertNewRow = false;
		this.gv_Doc.IsLoadingData = false;
		this.gv_Doc.ItemId = "OEPT";
		this.gv_Doc.LstColumnAllowBatch = null;
		this.gv_Doc.LstColumnNotAllowBatch = null;
		this.gv_Doc.LstColumnNotEditable = null;
		this.gv_Doc.LstColumNotSetting = null;
		this.gv_Doc.Name = "gv_Doc";
		this.gv_Doc.OptionsView.ShowGroupPanel = false;
		this.gv_Doc.UseBatchMod = false;
		this.gv_Doc.CustomDrawRowIndicator += new DevExpress.XtraGrid.Views.Grid.RowIndicatorCustomDrawEventHandler(this.gv_Doc_CustomDrawRowIndicator);
		this.gv_Doc.CellValueChanged += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanged);
		this.gv_Doc.CellValueChanging += new DevExpress.XtraGrid.Views.Base.CellValueChangedEventHandler(this.gv_Doc_CellValueChanging);
		this.gv_Doc.KeyDown += new System.Windows.Forms.KeyEventHandler(this.gv_Doc_KeyDown);
		this.colEmpID.Caption = "ID";
		this.colEmpID.CurrencyDateField = null;
		this.colEmpID.CurrencyField = null;
		this.colEmpID.CurrencyFixed = null;
		this.colEmpID.CurrencyFixedRate = 0.0;
		this.colEmpID.CurrencyRateField = null;
		this.colEmpID.FieldName = "EmpId";
		this.colEmpID.Name = "colEmpID";
		this.colEmpID.OptionsColumn.ReadOnly = true;
		this.colEmpID.PrimaryKey = false;
		this.colEmpID.UnitField = null;
		this.colEmpID.Visible = true;
		this.colEmpID.VisibleIndex = 0;
		this.colEmpCode.Caption = "Code";
		this.colEmpCode.CurrencyDateField = null;
		this.colEmpCode.CurrencyField = null;
		this.colEmpCode.CurrencyFixed = null;
		this.colEmpCode.CurrencyFixedRate = 0.0;
		this.colEmpCode.CurrencyRateField = null;
		this.colEmpCode.FieldName = "U_OldCode";
		this.colEmpCode.Name = "colEmpCode";
		this.colEmpCode.OptionsColumn.ReadOnly = true;
		this.colEmpCode.PrimaryKey = false;
		this.colEmpCode.UnitField = null;
		this.colEmpName.Caption = "Full Name";
		this.colEmpName.CurrencyDateField = null;
		this.colEmpName.CurrencyField = null;
		this.colEmpName.CurrencyFixed = null;
		this.colEmpName.CurrencyFixedRate = 0.0;
		this.colEmpName.CurrencyRateField = null;
		this.colEmpName.FieldName = "FullName";
		this.colEmpName.Name = "colEmpName";
		this.colEmpName.OptionsColumn.ReadOnly = true;
		this.colEmpName.PrimaryKey = false;
		this.colEmpName.UnitField = null;
		this.colEmpName.Visible = true;
		this.colEmpName.VisibleIndex = 1;
		this.colToken.Caption = "Token";
		this.colToken.ColumnEdit = this.te_Token;
		this.colToken.CurrencyDateField = null;
		this.colToken.CurrencyField = null;
		this.colToken.CurrencyFixed = null;
		this.colToken.CurrencyFixedRate = 0.0;
		this.colToken.CurrencyRateField = null;
		this.colToken.FieldName = "TokenStr";
		this.colToken.Name = "colToken";
		this.colToken.PrimaryKey = false;
		this.colToken.UnitField = null;
		this.colToken.Visible = true;
		this.colToken.VisibleIndex = 2;
		this.te_Token.AutoHeight = false;
		this.te_Token.Name = "te_Token";
		this.te_Token.UseSystemPasswordChar = true;
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Save;
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(2, 316);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(95, 20);
		this.btn_Save.TabIndex = 18;
		this.btn_Save.Text = "Save";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		this.btn_Author.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Author.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btn_Author.Appearance.Options.UseBackColor = true;
		this.btn_Author.FieldName = null;
		this.btn_Author.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Author.ImageOptions.Image");
		this.btn_Author.IsSearchControl = false;
		this.btn_Author.Location = new System.Drawing.Point(100, 316);
		this.btn_Author.Name = "btn_Author";
		this.btn_Author.Size = new System.Drawing.Size(124, 20);
		this.btn_Author.TabIndex = 46;
		this.btn_Author.Text = "Authorization";
		this.btn_Author.Click += new System.EventHandler(this.btn_Author_Click);
		this.btn_Refresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Refresh.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btn_Refresh.Appearance.Options.UseBackColor = true;
		this.btn_Refresh.FieldName = null;
		this.btn_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Refresh.ImageOptions.Image");
		this.btn_Refresh.IsSearchControl = false;
		this.btn_Refresh.Location = new System.Drawing.Point(464, 316);
		this.btn_Refresh.Name = "btn_Refresh";
		this.btn_Refresh.Size = new System.Drawing.Size(124, 20);
		this.btn_Refresh.TabIndex = 47;
		this.btn_Refresh.Text = "REFRESH";
		this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
		this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[1] { this.showPasswordToolStripMenuItem });
		this.contextMenuStrip1.Name = "contextMenuStrip1";
		this.contextMenuStrip1.Size = new System.Drawing.Size(157, 26);
		this.showPasswordToolStripMenuItem.Name = "showPasswordToolStripMenuItem";
		this.showPasswordToolStripMenuItem.Size = new System.Drawing.Size(156, 22);
		this.showPasswordToolStripMenuItem.Text = "Show Password";
		this.showPasswordToolStripMenuItem.Click += new System.EventHandler(this.showPasswordToolStripMenuItem_Click);
		this.btn_ShowPass.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_ShowPass.Appearance.BackColor = System.Drawing.Color.Transparent;
		this.btn_ShowPass.Appearance.Options.UseBackColor = true;
		this.btn_ShowPass.FieldName = null;
		this.btn_ShowPass.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_ShowPass.ImageOptions.Image");
		this.btn_ShowPass.IsSearchControl = false;
		this.btn_ShowPass.Location = new System.Drawing.Point(337, 316);
		this.btn_ShowPass.Name = "btn_ShowPass";
		this.btn_ShowPass.Size = new System.Drawing.Size(124, 20);
		this.btn_ShowPass.TabIndex = 48;
		this.btn_ShowPass.Text = "Show Password";
		this.btn_ShowPass.Visible = false;
		this.btn_ShowPass.Click += new System.EventHandler(this.btnShowPass_Click);
		base.AcceptButton = this.btn_Save;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(590, 339);
		base.Controls.Add(this.btn_ShowPass);
		base.Controls.Add(this.btn_Refresh);
		base.Controls.Add(this.btn_Author);
		base.Controls.Add(this.btn_Save);
		base.Controls.Add(this.grd_Doc);
		base.FormId = "oEmployeeToken";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_EmpToken";
		this.Text = "Employee Token Management";
		((System.ComponentModel.ISupportInitialize)this.grd_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Doc).EndInit();
		((System.ComponentModel.ISupportInitialize)this.te_Token).EndInit();
		this.contextMenuStrip1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
