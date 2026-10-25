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
using Apzon.Commons.Helper;
using Apzon.Commons.Properties;
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Tree;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;
using DevExpress.XtraTreeList;

namespace Apzon.Forms.Administrator.SystemInitialization;

public class frm_MenuStructure : XtraFormEx
{
	private const string Controller = "MenuStructure";

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public RefreshMain RefreshMain;

	private TreeListViewState treeListViewState;

	private IContainer components;

	private SimpleButtonEx btn_OK;

	private TreeEx tre_Menu;

	private SimpleButtonEx btn_Rename;

	private SimpleButtonEx btn_New;

	private SimpleButtonEx btn_Delete;

	private SimpleButtonEx btn_Relocate;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_OK.Text = Apzon.Commons.Properties.Resources.ButtonOK;
			break;
		}
	}

	public frm_MenuStructure()
	{
		InitializeComponent();
		OnLoadAsync += frm_MenuStructure_Load;
	}

	private async Task frm_MenuStructure_Load(object sender, EventArgs e)
	{
		treeListViewState = new TreeListViewState(tre_Menu);
		await LoadData();
	}

	private async Task LoadData()
	{
		_ = 1;
		try
		{
			tre_Menu.ShowLoadingPanel();
			DataSet ds = await _httpClient.MenuStructure.GetListMenu(GlobalData.CURRENTLANGUAGE);
			if (ds.IsHasStructure())
			{
				await TreeListHelper.GenerateTreeList(tre_Menu, ds);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			tre_Menu.HideLoadingPanel();
		}
	}

	private void btn_OK_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void tre_Menu_FocusedNodeChanged(object sender, FocusedNodeChangedEventArgs e)
	{
		try
		{
			string text = Function.ToString(tre_Menu.GetFocusedDataRow()?["MenuType"]);
			if (text.Equals("O") || text.Equals("T") || text.Equals("A") || text.Equals("W") || text.Equals("C") || text.Equals("R"))
			{
				btn_Relocate.Enabled = true;
			}
			else
			{
				btn_Relocate.Enabled = false;
			}
			if (text.Equals("A"))
			{
				btn_Delete.Enabled = true;
				btn_Rename.Enabled = true;
			}
			else
			{
				btn_Delete.Enabled = false;
				btn_Rename.Enabled = false;
			}
			if (text.Equals("N") || text.Equals("A"))
			{
				btn_New.Enabled = true;
			}
			else
			{
				btn_New.Enabled = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_New_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (!tre_Menu.DataSourceTable().IsNotNull() || tre_Menu.FocusedNode == null)
			{
				return;
			}
			treeListViewState.SaveState();
			DataTable dataTable = tre_Menu.DataSourceTable().Clone();
			string text = XtraInputBox.Show(this, "Enter folder name and choose the \"OK\" button", "Create New Folder", "");
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataRow focusedDataRow = tre_Menu.GetFocusedDataRow();
			DataRow dataRow = dataTable.NewRow();
			dataRow["Id"] = -1;
			dataRow["FatherId"] = focusedDataRow["Id"];
			dataRow["Code"] = "barSub_" + text;
			dataRow["Name"] = text;
			dataRow["LanguageKey"] = focusedDataRow["Code"];
			dataRow["MenuType"] = "A";
			dataTable.Rows.Add(dataRow);
			string text2 = await _httpClient.MenuStructure.AddNewFolder(dataTable);
			if (!string.IsNullOrEmpty(text2))
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
			}
			else
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				if (RefreshMain != null)
				{
					RefreshMain();
				}
				await LoadData();
				treeListViewState.LoadState();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Relocate_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (!tre_Menu.DataSourceTable().IsNotNull() || tre_Menu.FocusedNode == null)
			{
				return;
			}
			treeListViewState.SaveState();
			DataRow focusedDataRow = tre_Menu.GetFocusedDataRow();
			DataTable dataTable = tre_Menu.DataSourceTable().Clone();
			List<Common.ParameterValue> list = new List<Common.ParameterValue>();
			list.Add(new Common.ParameterValue("@currentId", Function.ParseInt(focusedDataRow["Id"])));
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedProcedure, false, "", "Apz_Cfl_GetFatherMenu", Enums.ItemObjectType.NoItemType, list, "");
			frm_CFL2._tableType = "2";
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			int num = -1;
			string value = "";
			if (chooseItem != null && chooseItem.IsExistsColumn("Id"))
			{
				num = Function.ParseInt(chooseItem["Id"]);
				value = Function.ToString(chooseItem["Code"]);
			}
			if (num == -1)
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = focusedDataRow.ItemArray;
			dataRow["FatherId"] = num;
			dataRow["LanguageKey"] = value;
			dataTable.Rows.Add(dataRow);
			string text = await _httpClient.MenuStructure.RelocateFolder(dataTable);
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
				return;
			}
			WinCommons.Success(Enums.MessageCode.Success, "");
			if (RefreshMain != null)
			{
				RefreshMain();
			}
			await LoadData();
			treeListViewState.LoadState();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Rename_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (!tre_Menu.DataSourceTable().IsNotNull() || tre_Menu.FocusedNode == null)
			{
				return;
			}
			treeListViewState.SaveState();
			DataTable dataTable = tre_Menu.DataSourceTable().Clone();
			DataRow focusedDataRow = tre_Menu.GetFocusedDataRow();
			string text = XtraInputBox.Show(this, "Enter folder name and choose the \"OK\" button", "Rename Folder", Function.ToString(focusedDataRow["Name"]));
			if (string.IsNullOrEmpty(text))
			{
				return;
			}
			WinCommons._OpenProcessing();
			DataRow dataRow = dataTable.NewRow();
			dataRow.ItemArray = focusedDataRow.ItemArray;
			dataRow["Code"] = "barSub_" + text;
			dataRow["Name"] = text;
			dataTable.Rows.Add(dataRow);
			string text2 = await _httpClient.MenuStructure.Rename(dataTable);
			if (!string.IsNullOrEmpty(text2))
			{
				WinCommons.Error(Enums.MessageCode.None, text2);
				return;
			}
			WinCommons.Success(Enums.MessageCode.Success, "");
			if (RefreshMain != null)
			{
				RefreshMain();
			}
			await LoadData();
			treeListViewState.LoadState();
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Delete_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (!tre_Menu.DataSourceTable().IsNotNull() || tre_Menu.FocusedNode == null)
			{
				return;
			}
			if (tre_Menu.FocusedNode.HasChildren)
			{
				WinCommons.Error(Enums.MessageCode.CannotDeleteFolderItContainSubitem, "");
			}
			else
			{
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDeleteFolder, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
				WinCommons._OpenProcessing();
				treeListViewState.SaveState();
				DataRow focusedDataRow = tre_Menu.GetFocusedDataRow();
				string text = await _httpClient.MenuStructure.DeleteFolder(Function.ParseInt(focusedDataRow["Id"]));
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				if (RefreshMain != null)
				{
					RefreshMain();
				}
				await LoadData();
				treeListViewState.LoadState();
				WinCommons._CloseProcessing();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tre_Menu_NodeCellStyle(object sender, GetCustomNodeCellStyleEventArgs e)
	{
		try
		{
			if (!(e.Column.FieldName != "Name"))
			{
				object value = e.Node.GetValue("MenuType");
				if (value.Equals("O") || value.Equals("T"))
				{
					e.Appearance.BackColor = Color.LightGoldenrodYellow;
				}
				else if (value.Equals("A") || value.Equals("N"))
				{
					e.Appearance.BackColor = Color.FromArgb(215, 239, 250);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Administrator.SystemInitialization.frm_MenuStructure));
		this.btn_OK = new Apzon.Controls.Controls.SimpleButtonEx();
		this.tre_Menu = new Apzon.Controls.Tree.TreeEx();
		this.btn_Rename = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_New = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Delete = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Relocate = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.tre_Menu).BeginInit();
		base.SuspendLayout();
		this.btn_OK.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_OK.FieldName = null;
		this.btn_OK.ImageOptions.Image = Apzon.Forms.Properties.Resources.apply_16x16;
		this.btn_OK.IsSearchControl = false;
		this.btn_OK.Location = new System.Drawing.Point(2, 590);
		this.btn_OK.Name = "btn_OK";
		this.btn_OK.Size = new System.Drawing.Size(75, 20);
		this.btn_OK.TabIndex = 1;
		this.btn_OK.Text = "OK";
		this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
		this.tre_Menu.AllowPasteExcel = false;
		this.tre_Menu.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.tre_Menu.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.tre_Menu.ColumnKey = null;
		this.tre_Menu.DisableFormSetting = false;
		this.tre_Menu.EditColumn = null;
		this.tre_Menu.FieldCurrency = null;
		this.tre_Menu.FieldRate = null;
		this.tre_Menu.FormId = "oMenuStructure";
		this.tre_Menu.IndexEditRow = 0;
		this.tre_Menu.IndicatorWidth = 40;
		this.tre_Menu.IsApzTable = false;
		this.tre_Menu.IsLoadingData = false;
		this.tre_Menu.ItemId = "OTREE";
		this.tre_Menu.Location = new System.Drawing.Point(0, 0);
		this.tre_Menu.LstColumnNotEditable = null;
		this.tre_Menu.LstColumNotSetting = null;
		this.tre_Menu.Name = "tre_Menu";
		this.tre_Menu.ShowButtonMode = DevExpress.XtraTreeList.ShowButtonModeEnum.ShowForFocusedRow;
		this.tre_Menu.Size = new System.Drawing.Size(389, 586);
		this.tre_Menu.TabIndex = 2;
		this.tre_Menu.NodeCellStyle += new DevExpress.XtraTreeList.GetCustomNodeCellStyleEventHandler(this.tre_Menu_NodeCellStyle);
		this.tre_Menu.FocusedNodeChanged += new DevExpress.XtraTreeList.FocusedNodeChangedEventHandler(this.tre_Menu_FocusedNodeChanged);
		this.btn_Rename.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Rename.FieldName = null;
		this.btn_Rename.IsSearchControl = false;
		this.btn_Rename.Location = new System.Drawing.Point(404, 12);
		this.btn_Rename.Name = "btn_Rename";
		this.btn_Rename.Size = new System.Drawing.Size(75, 23);
		this.btn_Rename.TabIndex = 3;
		this.btn_Rename.Text = "&Rename";
		this.btn_Rename.Click += new System.EventHandler(this.btn_Rename_Click);
		this.btn_New.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_New.FieldName = null;
		this.btn_New.IsSearchControl = false;
		this.btn_New.Location = new System.Drawing.Point(404, 41);
		this.btn_New.Name = "btn_New";
		this.btn_New.Size = new System.Drawing.Size(75, 23);
		this.btn_New.TabIndex = 4;
		this.btn_New.Text = "&New";
		this.btn_New.Click += new System.EventHandler(this.btn_New_Click);
		this.btn_Delete.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Delete.FieldName = null;
		this.btn_Delete.IsSearchControl = false;
		this.btn_Delete.Location = new System.Drawing.Point(404, 70);
		this.btn_Delete.Name = "btn_Delete";
		this.btn_Delete.Size = new System.Drawing.Size(75, 23);
		this.btn_Delete.TabIndex = 5;
		this.btn_Delete.Text = "De&lete";
		this.btn_Delete.Click += new System.EventHandler(this.btn_Delete_Click);
		this.btn_Relocate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Relocate.FieldName = null;
		this.btn_Relocate.IsSearchControl = false;
		this.btn_Relocate.Location = new System.Drawing.Point(404, 99);
		this.btn_Relocate.Name = "btn_Relocate";
		this.btn_Relocate.Size = new System.Drawing.Size(75, 23);
		this.btn_Relocate.TabIndex = 6;
		this.btn_Relocate.Text = "Rel&ocate";
		this.btn_Relocate.Click += new System.EventHandler(this.btn_Relocate_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(491, 614);
		base.Controls.Add(this.btn_Relocate);
		base.Controls.Add(this.btn_Delete);
		base.Controls.Add(this.btn_New);
		base.Controls.Add(this.btn_Rename);
		base.Controls.Add(this.tre_Menu);
		base.Controls.Add(this.btn_OK);
		base.FormId = "oMenuStructure";
		base.IconOptions.Icon = (System.Drawing.Icon)resources.GetObject("frm_MenuStructure.IconOptions.Icon");
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_MenuStructure";
		base.ShowInTaskbar = false;
		this.Text = "Menu Structure";
		((System.ComponentModel.ISupportInitialize)this.tre_Menu).EndInit();
		base.ResumeLayout(false);
	}
}
