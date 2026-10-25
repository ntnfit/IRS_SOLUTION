using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Forms.UserControls;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraReports.UI;
using DevExpress.XtraTab;

namespace Apzon.Forms.Reports;

public class frm_ReportManager : XtraFormEx
{
	private const string FormController = "XtraReportManager";

	private const string TableName = "APZ_RDOC";

	private Function.ConfigData _ConfigData;

	private IContainer components;

	private SplitContainerControl splitContainerControl1;

	private XtraTabControlEx tab_Search;

	private XtraTabPageEx page_Report;

	private GridControlEx grd_Seach;

	private GridViewEx gv_Search;

	private PanelControl panelControl1;

	private SimpleButtonEx btn_Cancel;

	private SimpleButtonEx btn_Save;

	private PanelControl panelControl2;

	private SimpleButtonEx btn_Refresh;

	private SimpleButtonEx btn_Export;

	private SimpleButtonEx btn_Import;

	private SimpleButtonEx btn_RunReport;

	private SimpleButtonEx btn_Edit;

	private SimpleButtonEx btn_Design;

	private CheckBox chk_Active;

	private MemoEditEx txt_Desc;

	private TextEditEx txt_ReportName;

	private TextEditEx txt_PatchFile;

	private LabelLine labelLine4;

	private LabelLine labelLine3;

	private LabelLine lbl_Menu;

	private SimpleButtonEx btn_Browse;

	private LabelLine labelLine1;

	private XtraTabPageEx page_DocType;

	private GridControlEx grd_DocType;

	private GridViewEx gv_DocType;

	private TextEditEx txt_DocCode;

	private LabelLine lbl_DocCode;

	private SimpleButtonEx btn_PreviewExternal;

	private ButtonEditCFL bte_ParentMenuId;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			btn_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_ReportManager()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ReportManager"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ReportManager" && x.authoriza == "F"))
			{
				btn_Browse.Enabled = false;
				btn_Save.Enabled = false;
				btn_Edit.Enabled = false;
				btn_Import.Enabled = false;
				btn_Export.Enabled = false;
				btn_RunReport.Enabled = false;
			}
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_XtraReportManager_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			btn_Edit.Visible = false;
			btn_Export.Visible = false;
			bte_ParentMenuId.Properties.TypeCFL = Enums.TypeCFL.UserDefinedProcedure;
			bte_ParentMenuId.Properties.StoredProcedure = "Apz_Cfl_GetFatherMenu";
			bte_ParentMenuId.Properties.RefreshDataSource();
			bte_ParentMenuId.Properties.Buttons[1].Visible = false;
			RepositoryItemButtonEditCFL properties = bte_ParentMenuId.Properties;
			properties.BeforeShowCFL = (Action<frm_CFL>)Delegate.Combine(properties.BeforeShowCFL, (Action<frm_CFL>)((frm_CFL frm) =>
			{
				frm._tableType = "2";
			}));
			await REFRESH();
			using HttpClientHelper<Function.ConfigData> helper = new HttpClientHelper<Function.ConfigData>(Configuration.ApiServer, "ConfigConnect");
			_ConfigData = await helper.GetAsync("GetConfig");
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (!grd_Seach.DataSourceTable.IsNotNull() && !grd_DocType.DataSourceTable.IsNotNull())
				{
					WinCommons.Error(Enums.MessageCode.SaveDataCanNotBeNull, "");
					return;
				}
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					return;
				}
				WinCommons._OpenProcessing();
				btn_Save.Enabled = false;
				string value = "";
				XtraTabPage selectedTabPage = tab_Search.SelectedTabPage;
				if (string.IsNullOrEmpty(txt_PatchFile.Text))
				{
					value = ((!selectedTabPage.Name.Equals("page_Report")) ? Function.ToString(gv_DocType.GetRowCellValue(gv_DocType.FocusedRowHandle, "Text")) : Function.ToString(gv_Search.GetRowCellValue(gv_Search.FocusedRowHandle, "Text")));
				}
				else
				{
					XtraReport xtraReport = new XtraReport();
					xtraReport.LoadLayout(txt_PatchFile.Text);
					MemoryStream memoryStream = new MemoryStream();
					xtraReport.SaveLayout(memoryStream);
					memoryStream.Position = 0L;
					using StreamReader streamReader = new StreamReader(memoryStream);
					value = WinFuntions.EncryptStringAES(streamReader.ReadToEnd(), "APZ_RDOC");
				}
				DataTable dtSave = CreateTable();
				DataRow dataRow = dtSave.NewRow();
				string name = selectedTabPage.Name;
				if (!(name == "page_Report"))
				{
					if (name == "page_DocType")
					{
						dataRow["ID"] = gv_DocType.GetRowCellValue(gv_DocType.FocusedRowHandle, "ID");
						dataRow["TypeCode"] = gv_DocType.GetRowCellValue(gv_DocType.FocusedRowHandle, "TypeCode");
						dataRow["TypeName"] = gv_DocType.GetRowCellValue(gv_DocType.FocusedRowHandle, "TypeName");
						dataRow["CreateDate"] = gv_DocType.GetRowCellValue(gv_DocType.FocusedRowHandle, "CreateDate");
						dataRow["UserSign"] = gv_DocType.GetRowCellValue(gv_DocType.FocusedRowHandle, "UserSign");
					}
				}
				else
				{
					dataRow["ID"] = gv_Search.GetRowCellValue(gv_Search.FocusedRowHandle, "ID");
					dataRow["TypeCode"] = gv_Search.GetRowCellValue(gv_Search.FocusedRowHandle, "TypeCode");
					dataRow["TypeName"] = gv_Search.GetRowCellValue(gv_Search.FocusedRowHandle, "TypeName");
					dataRow["CreateDate"] = gv_Search.GetRowCellValue(gv_Search.FocusedRowHandle, "CreateDate");
					dataRow["UserSign"] = gv_Search.GetRowCellValue(gv_Search.FocusedRowHandle, "UserSign");
				}
				dataRow["DocCode"] = txt_DocCode.Text;
				dataRow["DocName"] = txt_ReportName.Text;
				dataRow["Menu"] = Function.ToString(bte_ParentMenuId.EditValue);
				dataRow["UpdateDate"] = DateTime.Now;
				dataRow["UserSign2"] = GlobalData.UserName;
				dataRow["Status"] = (chk_Active.Checked ? "A" : "I");
				dataRow["Description"] = txt_Desc.Text;
				dataRow["Text"] = value;
				dtSave.Rows.Add(dataRow);
				HttpResult httpResult = await Save(dtSave);
				dtSave.Columns.Clear();
				dtSave.Dispose();
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					await REFRESH();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		btn_Save.Enabled = true;
		WinCommons._CloseProcessing();
	}

	private void btn_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void btn_Import_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			dlg_ImportReport dlg_ImportReport2 = new dlg_ImportReport();
			dlg_ImportReport2.ShowDialog(this);
			if (!string.IsNullOrEmpty(dlg_ImportReport2.patchFile))
			{
				WinCommons._OpenProcessing();
				btn_Save.Enabled = false;
				string plainText = "";
				XtraReport xtraReport = new XtraReport();
				xtraReport.LoadLayout(dlg_ImportReport2.patchFile);
				MemoryStream memoryStream = new MemoryStream();
				xtraReport.SaveLayout(memoryStream);
				memoryStream.Position = 0L;
				using (StreamReader streamReader = new StreamReader(memoryStream))
				{
					plainText = streamReader.ReadToEnd();
				}
				DataTable dtSave = CreateTable();
				DataRow dataRow = dtSave.NewRow();
				dataRow["ID"] = 0;
				dataRow["TypeCode"] = dlg_ImportReport2.typeCode;
				dataRow["TypeName"] = dlg_ImportReport2.typeName;
				dataRow["DocCode"] = dlg_ImportReport2.docCode;
				dataRow["DocName"] = dlg_ImportReport2.reportName;
				dataRow["Menu"] = dlg_ImportReport2.menu;
				dataRow["CreateDate"] = DateTime.Now;
				dataRow["UserSign"] = GlobalData.UserName;
				dataRow["Status"] = "A";
				dataRow["Description"] = dlg_ImportReport2.desc;
				dataRow["Text"] = WinFuntions.EncryptStringAES(plainText, "APZ_RDOC");
				dtSave.Rows.Add(dataRow);
				HttpResult httpResult = await Save(dtSave);
				WinCommons._CloseProcessing();
				dtSave.Columns.Clear();
				dtSave.Dispose();
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					await REFRESH();
					FormMode = Enums.FormViewModeType.View;
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Export_Click(object sender, EventArgs e)
	{
	}

	private void gv_Search_RowClick(object sender, RowClickEventArgs e)
	{
		try
		{
			btn_RunReport.Enabled = true;
			txt_ReportName.Text = Function.ToString(gv_Search.GetRowCellValue(gv_Search.FocusedRowHandle, "DocName"));
			bte_ParentMenuId.EditValue = Function.ToString(gv_Search.GetRowCellValue(gv_Search.FocusedRowHandle, "Menu"));
			chk_Active.Checked = Function.ToString(gv_Search.GetRowCellValue(gv_Search.FocusedRowHandle, "Status")).Equals("A");
			txt_Desc.Text = Function.ToString(gv_Search.GetRowCellValue(gv_Search.FocusedRowHandle, "Description"));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_RunReport_Click(object sender, EventArgs e)
	{
		try
		{
			if ((tab_Search.SelectedTabPage.Name.Equals("page_Report") && gv_Search.FocusedRowHandle < 0) || (tab_Search.SelectedTabPage.Name.Equals("page_DocType") && gv_DocType.FocusedRowHandle < 0))
			{
				return;
			}
			string text = "";
			text = ((!tab_Search.SelectedTabPage.Name.Equals("page_Report")) ? WinFuntions.DecryptStringAES(Function.ToString(gv_DocType.GetRowCellValue(gv_DocType.FocusedRowHandle, "Text")), "APZ_RDOC") : WinFuntions.DecryptStringAES(Function.ToString(gv_Search.GetRowCellValue(gv_Search.FocusedRowHandle, "Text")), "APZ_RDOC"));
			if (!string.IsNullOrEmpty(text))
			{
				XtraReport xtraReport;
				using (StreamWriter streamWriter = new StreamWriter(new MemoryStream()))
				{
					streamWriter.Write(text);
					streamWriter.Flush();
					xtraReport = XtraReport.FromStream(streamWriter.BaseStream);
				}
				if (xtraReport.Parameters.Count > 0)
				{
					frm_XtraParameterSubmit frm_XtraParameterSubmit2 = new frm_XtraParameterSubmit();
					frm_XtraParameterSubmit2.lstPara = xtraReport.Parameters;
					frm_XtraParameterSubmit2._txtReport = text;
					frm_XtraParameterSubmit2.MdiParent = MdiParent;
					frm_XtraParameterSubmit2.Show();
				}
				else
				{
					WinFuntions.XtraReportViewer(xtraReport, new List<Common.ParameterValue>());
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_Design_Click(object sender, EventArgs e)
	{
		try
		{
			new frm_ReportDesigner().ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Refresh_Click(object sender, EventArgs e)
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

	private void btn_Browse_Click(object sender, EventArgs e)
	{
		try
		{
			OpenFileDialog openFileDialog = new OpenFileDialog
			{
				DefaultExt = ".repx",
				Multiselect = false,
				Filter = "XreaReport Layout (.repx)|*.repx"
			};
			openFileDialog.ShowDialog(this);
			if (!string.IsNullOrEmpty(openFileDialog.FileName))
			{
				txt_PatchFile.Text = openFileDialog.FileName;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_TextBox_EditValueChanged(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	private void bte_ParentMenuId_EditValueChanged(object sender, EventArgs e)
	{
		FormMode = Enums.FormViewModeType.Update;
	}

	public async Task REFRESH()
	{
		btn_RunReport.Enabled = false;
		txt_PatchFile.Text = "";
		txt_ReportName.Text = "";
		txt_Desc.Text = "";
		txt_DocCode.Text = "";
		chk_Active.Checked = false;
		bte_ParentMenuId.Text = "";
		FormMode = Enums.FormViewModeType.View;
		btn_Save.Enabled = true;
		new DataSet();
		DataSet ds = await LoadReport(GlobalData.UserName, GlobalData.DefaultBranch);
		if (ds.IsNotNull() && ds.Tables[1].IsNotNull())
		{
			await GridViewHelper.GenerateGrid(grd_Seach, gv_Search, ds, null, expand: true);
		}
		ds.Tables.Clear();
		ds.Dispose();
		DataSet dsLayout = await LoadLayout(GlobalData.UserName, GlobalData.DefaultBranch);
		if (dsLayout.IsNotNull() && dsLayout.Tables[1].IsNotNull())
		{
			await GridViewHelper.GenerateGrid(grd_DocType, gv_DocType, dsLayout, null, expand: true);
		}
		dsLayout.Tables.Clear();
		dsLayout.Dispose();
	}

	private DataTable CreateTable()
	{
		return new DataTable("APZ_RDOC")
		{
			Columns = 
			{
				{
					"ID",
					typeof(int)
				},
				{
					"TypeCode",
					typeof(string)
				},
				{
					"TypeName",
					typeof(string)
				},
				{
					"DocCode",
					typeof(string)
				},
				{
					"DocName",
					typeof(string)
				},
				{
					"Menu",
					typeof(string)
				},
				{
					"CreateDate",
					typeof(DateTime)
				},
				{
					"UpdateDate",
					typeof(DateTime)
				},
				{
					"UserSign",
					typeof(string)
				},
				{
					"UserSign2",
					typeof(string)
				},
				{
					"Status",
					typeof(string)
				},
				{
					"Description",
					typeof(string)
				},
				{
					"Text",
					typeof(string)
				}
			}
		};
	}

	private async Task<DataSet> LoadReport(string userName, int branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "XtraReportManager");
			return await helper.GetAsync("LoadReport", new { userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<DataSet> LoadLayout(string userName, int branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "XtraReportManager");
			return await helper.GetAsync("LoadLayout", new { userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task<HttpResult> Save(DataTable dt)
	{
		try
		{
			using HttpClientHelper<HttpResult> _help = new HttpClientHelper<HttpResult>(Configuration.ApiServer, "XtraReportManager");
			return await _help.PostAsync("Save", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> RemoveReport(int id)
	{
		try
		{
			using HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "XtraReportManager");
			return await helper.GetAsync("RemoveReport", new { id });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private void gv_DocType_RowClick(object sender, RowClickEventArgs e)
	{
		try
		{
			btn_RunReport.Enabled = true;
			txt_ReportName.Text = Function.ToString(gv_DocType.GetRowCellValue(gv_DocType.FocusedRowHandle, "DocName"));
			txt_DocCode.EditValue = Function.ToString(gv_DocType.GetRowCellValue(gv_DocType.FocusedRowHandle, "DocCode"));
			chk_Active.Checked = Function.ToString(gv_DocType.GetRowCellValue(gv_DocType.FocusedRowHandle, "Status")).Equals("A");
			txt_Desc.Text = Function.ToString(gv_DocType.GetRowCellValue(gv_DocType.FocusedRowHandle, "Description"));
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void tab_Search_SelectedPageChanged(object sender, TabPageChangedEventArgs e)
	{
		try
		{
			string name = tab_Search.SelectedTabPage.Name;
			if (!(name == "page_Report"))
			{
				if (name == "page_DocType")
				{
					bte_ParentMenuId.Visible = false;
					lbl_Menu.Visible = false;
					lbl_DocCode.Visible = true;
					txt_DocCode.Visible = true;
				}
			}
			else
			{
				bte_ParentMenuId.Visible = true;
				lbl_Menu.Visible = true;
				lbl_DocCode.Visible = false;
				txt_DocCode.Visible = false;
				txt_DocCode.Text = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btn_PreviewExternal_Click(object sender, EventArgs e)
	{
		try
		{
			string text = WinFuntions.PathOpenFile("Crystall Report File (.repx)", "repx");
			if (!string.IsNullOrEmpty(text))
			{
				string text2 = ReportViewerManualParam(text);
				if (!string.IsNullOrEmpty(text2))
				{
					WinCommons.Error(Enums.MessageCode.None, text2);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private string ReportViewerManualParam(string pathFile)
	{
		try
		{
			if (!File.Exists(pathFile))
			{
				return "Không tìm thấy file báo cáo: " + Path.GetFileName(pathFile);
			}
			XtraReport xtraReport = new XtraReport();
			xtraReport.LoadLayout(pathFile);
			MemoryStream stream = new MemoryStream();
			xtraReport.SaveLayout(stream);
			return WinFuntions.XtraReportViewerManualParam(xtraReport, "", pathFile);
		}
		catch (Exception ex)
		{
			return ex.Message;
		}
	}

	private void gv_Search_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu(gv_Search);
			if (gv_Search.FocusedRowHandle >= 0)
			{
				DXMenuItem dXMenuItem = new DXMenuItem();
				dXMenuItem.Caption = "Remove";
				dXMenuItem.Click += gv_Search_Click;
				gridViewMenu.Items.Add(dXMenuItem);
				if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
				{
					e.Allow = true;
					gridViewMenu.Show(e.Point);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void gv_Search_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (gv_Search.FocusedRowHandle >= 0 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				WinCommons._OpenProcessing();
				int id = Function.ParseInt(gv_Search.GetRowCellValue(gv_Search.FocusedRowHandle, "ID"));
				string text = await RemoveReport(id);
				if (string.IsNullOrEmpty(text))
				{
					await REFRESH();
					WinCommons.writeStatus(Enums.MessageCode.RemoveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, text);
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
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions imageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		DevExpress.Utils.SerializableAppearanceObject appearance = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.XtraEditors.Controls.EditorButtonImageOptions editorButtonImageOptions = new DevExpress.XtraEditors.Controls.EditorButtonImageOptions();
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Reports.frm_ReportManager));
		DevExpress.Utils.SerializableAppearanceObject appearance2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceHovered2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearancePressed2 = new DevExpress.Utils.SerializableAppearanceObject();
		DevExpress.Utils.SerializableAppearanceObject appearanceDisabled2 = new DevExpress.Utils.SerializableAppearanceObject();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.tab_Search = new Apzon.Controls.Controls.XtraTabControlEx();
		this.page_Report = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_Seach = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Search = new Apzon.Controls.Grid.GridViewEx();
		this.page_DocType = new Apzon.Controls.Controls.XtraTabPageEx();
		this.grd_DocType = new Apzon.Controls.Grid.GridControlEx();
		this.gv_DocType = new Apzon.Controls.Grid.GridViewEx();
		this.panelControl2 = new DevExpress.XtraEditors.PanelControl();
		this.bte_ParentMenuId = new Apzon.Forms.UserControls.ButtonEditCFL();
		this.txt_DocCode = new Apzon.Controls.Editor.TextEditEx();
		this.lbl_DocCode = new Apzon.Controls.Controls.LabelLine();
		this.chk_Active = new System.Windows.Forms.CheckBox();
		this.txt_Desc = new Apzon.Controls.Editor.MemoEditEx();
		this.txt_ReportName = new Apzon.Controls.Editor.TextEditEx();
		this.txt_PatchFile = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.lbl_Menu = new Apzon.Controls.Controls.LabelLine();
		this.btn_Browse = new Apzon.Controls.Controls.SimpleButtonEx();
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.btn_Edit = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_RunReport = new Apzon.Controls.Controls.SimpleButtonEx();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.btn_PreviewExternal = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Design = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Export = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Import = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Refresh = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.tab_Search).BeginInit();
		this.tab_Search.SuspendLayout();
		this.page_Report.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_Seach).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Search).BeginInit();
		this.page_DocType.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.grd_DocType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DocType).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).BeginInit();
		this.panelControl2.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.bte_ParentMenuId.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Desc.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ReportName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PatchFile.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		base.SuspendLayout();
		this.splitContainerControl1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.splitContainerControl1.Location = new System.Drawing.Point(1, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.tab_Search);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.panelControl2);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(914, 455);
		this.splitContainerControl1.SplitterPosition = 291;
		this.splitContainerControl1.TabIndex = 0;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.tab_Search.Dock = System.Windows.Forms.DockStyle.Fill;
		this.tab_Search.IsSearchControl = false;
		this.tab_Search.Location = new System.Drawing.Point(0, 0);
		this.tab_Search.Name = "tab_Search";
		this.tab_Search.SelectedTabPage = this.page_Report;
		this.tab_Search.Size = new System.Drawing.Size(291, 455);
		this.tab_Search.TabIndex = 0;
		this.tab_Search.TabPages.AddRange(new DevExpress.XtraTab.XtraTabPage[2] { this.page_Report, this.page_DocType });
		this.tab_Search.SelectedPageChanged += new DevExpress.XtraTab.TabPageChangedEventHandler(this.tab_Search_SelectedPageChanged);
		this.page_Report.Controls.Add(this.grd_Seach);
		this.page_Report.EditControl = null;
		this.page_Report.EditValue = "Report";
		this.page_Report.FieldName = null;
		this.page_Report.IsSearchControl = false;
		this.page_Report.Name = "page_Report";
		this.page_Report.ReadOnly = false;
		this.page_Report.SettingInVisible = false;
		this.page_Report.SettingReadonly = false;
		this.page_Report.Size = new System.Drawing.Size(289, 430);
		this.page_Report.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.page_Report.Text = "Report";
		this.grd_Seach.AllowPasteExcel = true;
		this.grd_Seach.ClipboardData = null;
		this.grd_Seach.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_Seach.Location = new System.Drawing.Point(0, 0);
		this.grd_Seach.MainView = this.gv_Search;
		this.grd_Seach.Name = "grd_Seach";
		this.grd_Seach.Size = new System.Drawing.Size(289, 430);
		this.grd_Seach.TabIndex = 0;
		this.grd_Seach.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Search });
		this.gv_Search.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_Search.ColumnKey = null;
		this.gv_Search.DisableFormSetting = false;
		this.gv_Search.DisplayException = false;
		this.gv_Search.EditColumn = null;
		this.gv_Search.FieldCurrency = null;
		this.gv_Search.FieldRate = null;
		this.gv_Search.FormId = "oReportManager";
		this.gv_Search.GridControl = this.grd_Seach;
		this.gv_Search.IgnoreFilter = false;
		this.gv_Search.IndexEditRow = -1;
		this.gv_Search.IndicatorWidth = 35;
		this.gv_Search.IsApzTable = false;
		this.gv_Search.IsInsertNewRow = false;
		this.gv_Search.IsLoadingData = false;
		this.gv_Search.IsMappingContextAction = false;
		this.gv_Search.ItemId = "OSEARCH";
		this.gv_Search.LstColumnAllowBatch = null;
		this.gv_Search.LstColumnNotAllowBatch = null;
		this.gv_Search.LstColumnNotEditable = null;
		this.gv_Search.LstColumNotSetting = null;
		this.gv_Search.Name = "gv_Search";
		this.gv_Search.UseBatchMod = false;
		this.gv_Search.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gv_Search_RowClick);
		this.gv_Search.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Search_PopupMenuShowing);
		this.page_DocType.Controls.Add(this.grd_DocType);
		this.page_DocType.EditControl = null;
		this.page_DocType.EditValue = "Document";
		this.page_DocType.FieldName = null;
		this.page_DocType.IsSearchControl = false;
		this.page_DocType.Name = "page_DocType";
		this.page_DocType.ReadOnly = false;
		this.page_DocType.SettingInVisible = false;
		this.page_DocType.SettingReadonly = false;
		this.page_DocType.Size = new System.Drawing.Size(289, 430);
		this.page_DocType.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.page_DocType.Text = "Document";
		this.grd_DocType.AllowPasteExcel = false;
		this.grd_DocType.ClipboardData = null;
		this.grd_DocType.Dock = System.Windows.Forms.DockStyle.Fill;
		this.grd_DocType.Location = new System.Drawing.Point(0, 0);
		this.grd_DocType.MainView = this.gv_DocType;
		this.grd_DocType.Name = "grd_DocType";
		this.grd_DocType.Size = new System.Drawing.Size(289, 430);
		this.grd_DocType.TabIndex = 1;
		this.grd_DocType.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_DocType });
		this.gv_DocType.ChangeLogType = Apzon.Commons.Enums.Enums.ChangeLog.clNone;
		this.gv_DocType.ColumnKey = null;
		this.gv_DocType.DisableFormSetting = false;
		this.gv_DocType.DisplayException = false;
		this.gv_DocType.EditColumn = null;
		this.gv_DocType.FieldCurrency = null;
		this.gv_DocType.FieldRate = null;
		this.gv_DocType.FormId = "oReportManager";
		this.gv_DocType.GridControl = this.grd_DocType;
		this.gv_DocType.IgnoreFilter = false;
		this.gv_DocType.IndexEditRow = -1;
		this.gv_DocType.IndicatorWidth = 35;
		this.gv_DocType.IsApzTable = false;
		this.gv_DocType.IsInsertNewRow = false;
		this.gv_DocType.IsLoadingData = false;
		this.gv_DocType.IsMappingContextAction = false;
		this.gv_DocType.ItemId = "ODOCTYPE";
		this.gv_DocType.LstColumnAllowBatch = null;
		this.gv_DocType.LstColumnNotAllowBatch = null;
		this.gv_DocType.LstColumnNotEditable = null;
		this.gv_DocType.LstColumNotSetting = null;
		this.gv_DocType.Name = "gv_DocType";
		this.gv_DocType.UseBatchMod = false;
		this.gv_DocType.RowClick += new DevExpress.XtraGrid.Views.Grid.RowClickEventHandler(this.gv_DocType_RowClick);
		this.panelControl2.Controls.Add(this.bte_ParentMenuId);
		this.panelControl2.Controls.Add(this.txt_DocCode);
		this.panelControl2.Controls.Add(this.lbl_DocCode);
		this.panelControl2.Controls.Add(this.chk_Active);
		this.panelControl2.Controls.Add(this.txt_Desc);
		this.panelControl2.Controls.Add(this.txt_ReportName);
		this.panelControl2.Controls.Add(this.txt_PatchFile);
		this.panelControl2.Controls.Add(this.labelLine4);
		this.panelControl2.Controls.Add(this.labelLine3);
		this.panelControl2.Controls.Add(this.lbl_Menu);
		this.panelControl2.Controls.Add(this.btn_Browse);
		this.panelControl2.Controls.Add(this.labelLine1);
		this.panelControl2.Controls.Add(this.btn_Edit);
		this.panelControl2.Controls.Add(this.btn_RunReport);
		this.panelControl2.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl2.Location = new System.Drawing.Point(0, 0);
		this.panelControl2.Name = "panelControl2";
		this.panelControl2.Size = new System.Drawing.Size(613, 455);
		this.panelControl2.TabIndex = 0;
		this.bte_ParentMenuId.EditControl = null;
		this.bte_ParentMenuId.FieldName = null;
		this.bte_ParentMenuId.IsSearchControl = false;
		this.bte_ParentMenuId.Location = new System.Drawing.Point(114, 174);
		this.bte_ParentMenuId.Name = "bte_ParentMenuId";
		this.bte_ParentMenuId.Properties.AutoLoadDataSource = true;
		editorButtonImageOptions.Image = (System.Drawing.Image)resources.GetObject("editorButtonImageOptions2.Image");
		this.bte_ParentMenuId.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[2]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search, "", -1, true, true, false, imageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance, appearanceHovered, appearancePressed, appearanceDisabled, "Search", null, null, DevExpress.Utils.ToolTipAnchor.Default),
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Glyph, "DrillDown", -1, true, true, true, editorButtonImageOptions, new DevExpress.Utils.KeyShortcut(System.Windows.Forms.Keys.None), appearance2, appearanceHovered2, appearancePressed2, appearanceDisabled2, "", null, null, DevExpress.Utils.ToolTipAnchor.Default)
		});
		this.bte_ParentMenuId.Properties.ContainerControl = null;
		this.bte_ParentMenuId.Properties.DisplayMember = "Name";
		this.bte_ParentMenuId.Properties.DisplayMember2 = "";
		this.bte_ParentMenuId.Properties.LoadDataSourceWhenEnter = false;
		this.bte_ParentMenuId.Properties.SeparateCharacterForMulti = ';';
		this.bte_ParentMenuId.Properties.SeparateString = ", ";
		this.bte_ParentMenuId.Properties.UsingAutoCompleteBox = false;
		this.bte_ParentMenuId.Properties.UsingDataSource = true;
		this.bte_ParentMenuId.Properties.ValidateWhenNoDataSource = false;
		this.bte_ParentMenuId.Properties.ValueMember = "Code";
		this.bte_ParentMenuId.SettingInVisible = false;
		this.bte_ParentMenuId.SettingReadonly = false;
		this.bte_ParentMenuId.Size = new System.Drawing.Size(337, 20);
		this.bte_ParentMenuId.TabIndex = 47;
		this.bte_ParentMenuId.EditValueChanged += new System.EventHandler(this.bte_ParentMenuId_EditValueChanged);
		this.txt_DocCode.EditControl = null;
		this.txt_DocCode.FieldName = null;
		this.txt_DocCode.IsSearchControl = false;
		this.txt_DocCode.Location = new System.Drawing.Point(114, 200);
		this.txt_DocCode.Name = "txt_DocCode";
		this.txt_DocCode.SettingInVisible = false;
		this.txt_DocCode.SettingReadonly = false;
		this.txt_DocCode.Size = new System.Drawing.Size(337, 20);
		this.txt_DocCode.TabIndex = 46;
		this.txt_DocCode.EditValueChanged += new System.EventHandler(this.txt_TextBox_EditValueChanged);
		this.lbl_DocCode.EditControl = null;
		this.lbl_DocCode.EditValue = "Doc Code";
		this.lbl_DocCode.FieldName = null;
		this.lbl_DocCode.HideWhenEmpty = false;
		this.lbl_DocCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_DocCode.Location = new System.Drawing.Point(27, 203);
		this.lbl_DocCode.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.lbl_DocCode.Name = "lbl_DocCode";
		this.lbl_DocCode.ReadOnly = false;
		this.lbl_DocCode.SettingInVisible = false;
		this.lbl_DocCode.SettingReadonly = false;
		this.lbl_DocCode.ShowUnderLine = true;
		this.lbl_DocCode.Size = new System.Drawing.Size(87, 21);
		this.lbl_DocCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_DocCode.TabIndex = 45;
		this.lbl_DocCode.Utext = "Doc Code";
		this.chk_Active.AutoSize = true;
		this.chk_Active.Location = new System.Drawing.Point(114, 226);
		this.chk_Active.Name = "chk_Active";
		this.chk_Active.Size = new System.Drawing.Size(56, 17);
		this.chk_Active.TabIndex = 42;
		this.chk_Active.Text = "Active";
		this.chk_Active.UseVisualStyleBackColor = true;
		this.chk_Active.CheckedChanged += new System.EventHandler(this.txt_TextBox_EditValueChanged);
		this.txt_Desc.EditControl = null;
		this.txt_Desc.FieldName = null;
		this.txt_Desc.IsSearchControl = false;
		this.txt_Desc.Location = new System.Drawing.Point(114, 72);
		this.txt_Desc.Name = "txt_Desc";
		this.txt_Desc.SettingInVisible = false;
		this.txt_Desc.SettingReadonly = false;
		this.txt_Desc.Size = new System.Drawing.Size(337, 96);
		this.txt_Desc.TabIndex = 41;
		this.txt_Desc.EditValueChanged += new System.EventHandler(this.txt_TextBox_EditValueChanged);
		this.txt_ReportName.EditControl = null;
		this.txt_ReportName.FieldName = null;
		this.txt_ReportName.IsSearchControl = false;
		this.txt_ReportName.Location = new System.Drawing.Point(114, 46);
		this.txt_ReportName.Name = "txt_ReportName";
		this.txt_ReportName.SettingInVisible = false;
		this.txt_ReportName.SettingReadonly = false;
		this.txt_ReportName.Size = new System.Drawing.Size(337, 20);
		this.txt_ReportName.TabIndex = 40;
		this.txt_ReportName.EditValueChanged += new System.EventHandler(this.txt_TextBox_EditValueChanged);
		this.txt_PatchFile.EditControl = null;
		this.txt_PatchFile.FieldName = null;
		this.txt_PatchFile.IsSearchControl = false;
		this.txt_PatchFile.Location = new System.Drawing.Point(114, 20);
		this.txt_PatchFile.Name = "txt_PatchFile";
		this.txt_PatchFile.Properties.ReadOnly = true;
		this.txt_PatchFile.SettingInVisible = false;
		this.txt_PatchFile.SettingReadonly = false;
		this.txt_PatchFile.Size = new System.Drawing.Size(337, 20);
		this.txt_PatchFile.TabIndex = 39;
		this.txt_PatchFile.EditValueChanged += new System.EventHandler(this.txt_TextBox_EditValueChanged);
		this.labelLine4.EditControl = null;
		this.labelLine4.EditValue = "Description";
		this.labelLine4.FieldName = null;
		this.labelLine4.HideWhenEmpty = false;
		this.labelLine4.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine4.Location = new System.Drawing.Point(25, 73);
		this.labelLine4.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.ReadOnly = false;
		this.labelLine4.SettingInVisible = false;
		this.labelLine4.SettingReadonly = false;
		this.labelLine4.ShowUnderLine = true;
		this.labelLine4.Size = new System.Drawing.Size(89, 21);
		this.labelLine4.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine4.TabIndex = 38;
		this.labelLine4.Utext = "Description";
		this.labelLine3.EditControl = null;
		this.labelLine3.EditValue = "Report Name";
		this.labelLine3.FieldName = null;
		this.labelLine3.HideWhenEmpty = false;
		this.labelLine3.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine3.Location = new System.Drawing.Point(25, 49);
		this.labelLine3.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.ReadOnly = false;
		this.labelLine3.SettingInVisible = false;
		this.labelLine3.SettingReadonly = false;
		this.labelLine3.ShowUnderLine = true;
		this.labelLine3.Size = new System.Drawing.Size(89, 21);
		this.labelLine3.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine3.TabIndex = 37;
		this.labelLine3.Utext = "Report Name";
		this.lbl_Menu.EditControl = null;
		this.lbl_Menu.EditValue = "Menu";
		this.lbl_Menu.FieldName = null;
		this.lbl_Menu.HideWhenEmpty = false;
		this.lbl_Menu.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbl_Menu.Location = new System.Drawing.Point(25, 174);
		this.lbl_Menu.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.lbl_Menu.Name = "lbl_Menu";
		this.lbl_Menu.ReadOnly = false;
		this.lbl_Menu.SettingInVisible = false;
		this.lbl_Menu.SettingReadonly = false;
		this.lbl_Menu.ShowUnderLine = true;
		this.lbl_Menu.Size = new System.Drawing.Size(89, 21);
		this.lbl_Menu.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbl_Menu.TabIndex = 35;
		this.lbl_Menu.Utext = "Menu";
		this.btn_Browse.EditControl = null;
		this.btn_Browse.EditValue = "Browse";
		this.btn_Browse.FieldName = null;
		this.btn_Browse.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Browse.ImageOptions.Image");
		this.btn_Browse.IsSearchControl = false;
		this.btn_Browse.Location = new System.Drawing.Point(457, 18);
		this.btn_Browse.Name = "btn_Browse";
		this.btn_Browse.ReadOnly = false;
		this.btn_Browse.SettingInVisible = false;
		this.btn_Browse.SettingReadonly = false;
		this.btn_Browse.Size = new System.Drawing.Size(71, 23);
		this.btn_Browse.TabIndex = 34;
		this.btn_Browse.Text = "Browse";
		this.btn_Browse.Click += new System.EventHandler(this.btn_Browse_Click);
		this.labelLine1.EditControl = null;
		this.labelLine1.EditValue = "File Location";
		this.labelLine1.FieldName = null;
		this.labelLine1.HideWhenEmpty = false;
		this.labelLine1.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.labelLine1.Location = new System.Drawing.Point(25, 22);
		this.labelLine1.Margin = new System.Windows.Forms.Padding(2, 4, 2, 4);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.ReadOnly = false;
		this.labelLine1.SettingInVisible = false;
		this.labelLine1.SettingReadonly = false;
		this.labelLine1.ShowUnderLine = true;
		this.labelLine1.Size = new System.Drawing.Size(89, 21);
		this.labelLine1.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.labelLine1.TabIndex = 33;
		this.labelLine1.Utext = "File Location";
		this.btn_Edit.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Edit.EditControl = null;
		this.btn_Edit.EditValue = "Edit";
		this.btn_Edit.FieldName = null;
		this.btn_Edit.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Edit.ImageOptions.Image");
		this.btn_Edit.IsSearchControl = false;
		this.btn_Edit.Location = new System.Drawing.Point(133, 427);
		this.btn_Edit.Name = "btn_Edit";
		this.btn_Edit.ReadOnly = false;
		this.btn_Edit.SettingInVisible = false;
		this.btn_Edit.SettingReadonly = false;
		this.btn_Edit.Size = new System.Drawing.Size(75, 23);
		this.btn_Edit.TabIndex = 1;
		this.btn_Edit.Text = "Edit";
		this.btn_RunReport.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_RunReport.EditControl = null;
		this.btn_RunReport.EditValue = "Run Report";
		this.btn_RunReport.FieldName = null;
		this.btn_RunReport.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_RunReport.ImageOptions.Image");
		this.btn_RunReport.IsSearchControl = false;
		this.btn_RunReport.Location = new System.Drawing.Point(25, 427);
		this.btn_RunReport.Name = "btn_RunReport";
		this.btn_RunReport.ReadOnly = false;
		this.btn_RunReport.SettingInVisible = false;
		this.btn_RunReport.SettingReadonly = false;
		this.btn_RunReport.Size = new System.Drawing.Size(102, 23);
		this.btn_RunReport.TabIndex = 0;
		this.btn_RunReport.Text = "Run Report";
		this.btn_RunReport.Click += new System.EventHandler(this.btn_RunReport_Click);
		this.panelControl1.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.panelControl1.Controls.Add(this.btn_PreviewExternal);
		this.panelControl1.Controls.Add(this.btn_Design);
		this.panelControl1.Controls.Add(this.btn_Export);
		this.panelControl1.Controls.Add(this.btn_Import);
		this.panelControl1.Controls.Add(this.btn_Refresh);
		this.panelControl1.Controls.Add(this.btn_Cancel);
		this.panelControl1.Controls.Add(this.btn_Save);
		this.panelControl1.Location = new System.Drawing.Point(2, 456);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(913, 38);
		this.panelControl1.TabIndex = 1;
		this.btn_PreviewExternal.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_PreviewExternal.EditControl = null;
		this.btn_PreviewExternal.EditValue = "Preview External Report";
		this.btn_PreviewExternal.FieldName = null;
		this.btn_PreviewExternal.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_PreviewExternal.ImageOptions.Image");
		this.btn_PreviewExternal.IsSearchControl = false;
		this.btn_PreviewExternal.Location = new System.Drawing.Point(251, 8);
		this.btn_PreviewExternal.Name = "btn_PreviewExternal";
		this.btn_PreviewExternal.ReadOnly = false;
		this.btn_PreviewExternal.SettingInVisible = false;
		this.btn_PreviewExternal.SettingReadonly = false;
		this.btn_PreviewExternal.Size = new System.Drawing.Size(148, 23);
		this.btn_PreviewExternal.TabIndex = 26;
		this.btn_PreviewExternal.Text = "Preview External Report";
		this.btn_PreviewExternal.Click += new System.EventHandler(this.btn_PreviewExternal_Click);
		this.btn_Design.EditControl = null;
		this.btn_Design.EditValue = "Design Report";
		this.btn_Design.FieldName = null;
		this.btn_Design.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Design.ImageOptions.Image");
		this.btn_Design.IsSearchControl = false;
		this.btn_Design.Location = new System.Drawing.Point(638, 10);
		this.btn_Design.Name = "btn_Design";
		this.btn_Design.ReadOnly = false;
		this.btn_Design.SettingInVisible = false;
		this.btn_Design.SettingReadonly = false;
		this.btn_Design.Size = new System.Drawing.Size(108, 23);
		this.btn_Design.TabIndex = 2;
		this.btn_Design.Text = "Design Report";
		this.btn_Design.Click += new System.EventHandler(this.btn_Design_Click);
		this.btn_Export.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Export.EditControl = null;
		this.btn_Export.EditValue = "Export";
		this.btn_Export.FieldName = null;
		this.btn_Export.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Export.ImageOptions.Image");
		this.btn_Export.IsSearchControl = false;
		this.btn_Export.Location = new System.Drawing.Point(752, 8);
		this.btn_Export.Name = "btn_Export";
		this.btn_Export.ReadOnly = false;
		this.btn_Export.SettingInVisible = false;
		this.btn_Export.SettingReadonly = false;
		this.btn_Export.Size = new System.Drawing.Size(75, 23);
		this.btn_Export.TabIndex = 25;
		this.btn_Export.Text = "Export";
		this.btn_Export.Click += new System.EventHandler(this.btn_Export_Click);
		this.btn_Import.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Import.EditControl = null;
		this.btn_Import.EditValue = "Import";
		this.btn_Import.FieldName = null;
		this.btn_Import.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Import.ImageOptions.Image");
		this.btn_Import.IsSearchControl = false;
		this.btn_Import.Location = new System.Drawing.Point(833, 8);
		this.btn_Import.Name = "btn_Import";
		this.btn_Import.ReadOnly = false;
		this.btn_Import.SettingInVisible = false;
		this.btn_Import.SettingReadonly = false;
		this.btn_Import.Size = new System.Drawing.Size(75, 23);
		this.btn_Import.TabIndex = 24;
		this.btn_Import.Text = "Import";
		this.btn_Import.Click += new System.EventHandler(this.btn_Import_Click);
		this.btn_Refresh.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Refresh.EditControl = null;
		this.btn_Refresh.EditValue = "Refresh";
		this.btn_Refresh.FieldName = null;
		this.btn_Refresh.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Refresh.ImageOptions.Image");
		this.btn_Refresh.IsSearchControl = false;
		this.btn_Refresh.Location = new System.Drawing.Point(170, 8);
		this.btn_Refresh.Name = "btn_Refresh";
		this.btn_Refresh.ReadOnly = false;
		this.btn_Refresh.SettingInVisible = false;
		this.btn_Refresh.SettingReadonly = false;
		this.btn_Refresh.Size = new System.Drawing.Size(75, 23);
		this.btn_Refresh.TabIndex = 23;
		this.btn_Refresh.Text = "Refresh";
		this.btn_Refresh.Click += new System.EventHandler(this.btn_Refresh_Click);
		this.btn_Cancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Cancel.EditControl = null;
		this.btn_Cancel.EditValue = "Cancel";
		this.btn_Cancel.FieldName = null;
		this.btn_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Cancel.ImageOptions.Image");
		this.btn_Cancel.IsSearchControl = false;
		this.btn_Cancel.Location = new System.Drawing.Point(89, 8);
		this.btn_Cancel.Name = "btn_Cancel";
		this.btn_Cancel.ReadOnly = false;
		this.btn_Cancel.SettingInVisible = false;
		this.btn_Cancel.SettingReadonly = false;
		this.btn_Cancel.Size = new System.Drawing.Size(75, 23);
		this.btn_Cancel.TabIndex = 22;
		this.btn_Cancel.Text = "Cancel";
		this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.EditControl = null;
		this.btn_Save.EditValue = "OK";
		this.btn_Save.FieldName = null;
		this.btn_Save.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Save.ImageOptions.Image");
		this.btn_Save.IsSearchControl = false;
		this.btn_Save.Location = new System.Drawing.Point(3, 8);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.ReadOnly = false;
		this.btn_Save.SettingInVisible = false;
		this.btn_Save.SettingReadonly = false;
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 21;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(916, 492);
		base.Controls.Add(this.panelControl1);
		base.Controls.Add(this.splitContainerControl1);
		base.FormId = "oReportManager";
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_ReportManager";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Report Manager";
		base.Load += new System.EventHandler(this.frm_XtraReportManager_Load);
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.tab_Search).EndInit();
		this.tab_Search.ResumeLayout(false);
		this.page_Report.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_Seach).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Search).EndInit();
		this.page_DocType.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.grd_DocType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_DocType).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl2).EndInit();
		this.panelControl2.ResumeLayout(false);
		this.panelControl2.PerformLayout();
		((System.ComponentModel.ISupportInitialize)this.bte_ParentMenuId.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_DocCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Desc.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_ReportName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_PatchFile.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
