using System;
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
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.Utils.Menu;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Menu;
using DevExpress.XtraGrid.Views.Base;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.Sales;

public class frm_HandoverDocument : XtraFormEx
{
	private const string FormController = "HandoverDocument";

	public DataSet DsControl = new DataSet();

	public new IHttpClient HttpClient = HttpClientHelperFactory.GetHttpClient();

	public string UdoCode = "OHDC";

	public DataTable UdoTable = new DataTable();

	private IContainer components;

	private LabelLine labelLine1;

	private LabelLine labelLine2;

	private TextEditEx txt_DocNum;

	private TextEditEx txt_UserName;

	private LookUpEdit look_Status;

	private LabelLine labelLine6;

	private DateEditEx date_CreateDate;

	private ContextMenuStrip ctx_Main;

	private ToolStripMenuItem menuMainClose;

	private ToolStripMenuItem menuMainCancel;

	private PanelControl panelControl1;

	private SplitContainerControl splitContainerControl1;

	private GridControlEx grd_Document;

	private GridViewEx gv_Document;

	private LabelLine labelLine3;

	private SimpleButtonEx btn_Clear;

	private TextEditEx txt_Document;

	private LabelLine labelLine4;

	private LabelLine labelLine5;

	private ButtonEditEx bte_EmpCode;

	private SimpleButtonEx btn_Save;

	private LabelLine labelLine7;

	private TextEditEx txt_Token;

	private TextEditEx txt_Remark;

	private LabelLine labelLine8;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			btn_Save.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			btn_Save.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			btn_Save.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			btn_Save.Text = Resources.ButtonFind;
			break;
		}
	}

	public frm_HandoverDocument()
	{
		InitializeComponent();
	}

	public async Task MOVE_NEXT()
	{
		try
		{
			int type = 3;
			int currentId = Function.ParseInt(txt_DocNum.EditValue);
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_FIRST()
	{
		try
		{
			int type = 1;
			int currentId = Function.ParseInt(txt_DocNum.EditValue);
			await Navigate(currentId, type);
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
			int type = 2;
			int currentId = Function.ParseInt(txt_DocNum.EditValue);
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MOVE_LAST()
	{
		try
		{
			int type = 4;
			int currentId = Function.ParseInt(txt_DocNum.EditValue);
			await Navigate(currentId, type);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task MODE_FIND()
	{
		try
		{
			txt_DocNum.Properties.ReadOnly = false;
			txt_Document.Enabled = true;
			txt_DocNum.Text = "";
			bte_EmpCode.Text = "";
			txt_Document.Text = "";
			txt_Remark.Text = "";
			date_CreateDate.EditValue = null;
			grd_Document.DataSource = new DataTable();
			txt_DocNum.BackColor = Color.FromArgb(250, 221, 122);
			bte_EmpCode.BackColor = Color.FromArgb(250, 221, 122);
			txt_Document.BackColor = Color.FromArgb(250, 221, 122);
			date_CreateDate.BackColor = Color.FromArgb(250, 221, 122);
			FormMode = Enums.FormViewModeType.Find;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task REFRESH_DATA()
	{
		try
		{
			int type = 5;
			int currentId = Function.ParseInt(txt_DocNum.EditValue);
			await Navigate(currentId, type);
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task Navigate(int currentId, int type)
	{
		WinCommons._OpenProcessing();
		try
		{
			int num = ((type != 5) ? (await GetPage(currentId, type, GlobalData.UserName, GlobalData.DefaultBranch)) : currentId);
			int id = num;
			REFRESH();
			await GetById(id, "");
			FormMode = Enums.FormViewModeType.View;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<int> GetPage(int currentId, int type, string username, int branch)
	{
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "HandoverDocument");
			return await helper.GetIntAsync("GetPage", new { currentId, type, username, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return 0;
	}

	private async void txt_DocNum_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(txt_DocNum.Text) && FormMode == Enums.FormViewModeType.Find)
			{
				int type = 5;
				int num = Function.ParseInt(txt_DocNum.EditValue);
				if (num > 0)
				{
					await Navigate(num, type);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void frm_HandoverDocument_Load(object sender, EventArgs e)
	{
		try
		{
			date_CreateDate.EditValue = DateTime.Today;
			txt_UserName.Text = GlobalData.UserName;
			LoadStatus();
			UdoTable = await GetUdoByCode(new Common.Table_Code
			{
				Name = "OUDO",
				Code = "OHDC"
			});
			FormMode = Enums.FormViewModeType.View;
			look_Status.EditValue = "O";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LoadStatus()
	{
		try
		{
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Code", typeof(string));
			dataTable.Columns.Add("Name", typeof(string));
			DataRow dataRow = dataTable.NewRow();
			dataRow["Code"] = "O";
			dataRow["Name"] = "Open";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "L";
			dataRow["Name"] = "Closed";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "C";
			dataRow["Name"] = "Canceled";
			dataTable.Rows.Add(dataRow);
			dataRow = dataTable.NewRow();
			dataRow["Code"] = "P";
			dataRow["Name"] = "Printed";
			dataTable.Rows.Add(dataRow);
			look_Status.Properties.DataSource = dataTable;
			look_Status.Properties.DisplayMember = "Name";
			look_Status.Properties.ValueMember = "Code";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Save_Click(object sender, EventArgs e)
	{
		_ = 4;
		try
		{
			btn_Save.Enabled = false;
			if (FormMode == Enums.FormViewModeType.View)
			{
				Close();
				return;
			}
			if (FormMode == Enums.FormViewModeType.Find)
			{
				if (string.IsNullOrEmpty(txt_DocNum.Text))
				{
					await SearchDocuments();
				}
				else
				{
					int type = 5;
					int currentId = Function.ParseInt(txt_DocNum.EditValue);
					await Navigate(currentId, type);
				}
			}
			else if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				DataTable dtLine = grd_Document.DataSourceTable;
				if (!dtLine.IsNotNull())
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.InfoDocumentNotSelect, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					btn_Save.Enabled = true;
					return;
				}
				string value = Function.ToString(dtLine.Rows[0]["U_EmpCode"]);
				string token = txt_Token.Text;
				if (string.IsNullOrEmpty(token))
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.InfoConfirmPassToHandOver, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					btn_Save.Enabled = true;
					return;
				}
				string empID = "";
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("U_OldCode", typeof(string));
				DataRow dataRow = dataTable.NewRow();
				dataRow["U_OldCode"] = value;
				dataTable.Rows.Add(dataRow);
				DataTable dataTable2 = await GetEmpId(dataTable);
				if (dataTable2.IsNotNull())
				{
					empID = Function.ToString(dataTable2.Rows[0][0]);
				}
				if (Function.ParseInt(empID) <= 0)
				{
					XtraMessageBoxEx.Show(Enums.MessageCode.InfoIncorrectEmployeeCode, Enums.MessageCode.SystemMessage, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					btn_Save.Enabled = true;
					return;
				}
				string text = "";
				using (HttpClientHelper<string> helper = new HttpClientHelper<string>(Configuration.ApiServer, "EmpToken"))
				{
					text = await helper.GetAsync("CheckToken", new
					{
						empId = empID,
						token = token
					});
				}
				if (!string.IsNullOrEmpty(text))
				{
					WinCommons.Error(Enums.MessageCode.None, text);
					btn_Save.Enabled = true;
					return;
				}
				if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmUpdate, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
				{
					btn_Save.Enabled = true;
					return;
				}
				WinCommons._OpenProcessing();
				if (UdoTable == null || UdoTable.Rows.Count == 0)
				{
					WinCommons.Error(Enums.MessageCode.UdoTableIsInvalid, "");
					btn_Save.Enabled = true;
					return;
				}
				btn_Save.Enabled = false;
				string tableName = Function.ToString(UdoTable.Rows[0]["TableName"]);
				DataSet ds = new DataSet
				{
					Tables = { UdoTable.Copy() }
				};
				DataTable dataTable3 = new DataTable(tableName);
				dataTable3.Columns.Add("DocEntry", typeof(int));
				dataTable3.Columns.Add("Remark", typeof(string));
				dataTable3.Columns.Add("U_UserSign2", typeof(string));
				DataRow dataRow2 = dataTable3.NewRow();
				dataRow2["DocEntry"] = ((FormMode != Enums.FormViewModeType.Add) ? Function.ParseInt(txt_DocNum.Text) : 0);
				dataRow2["Remark"] = Function.ToString(txt_Remark.EditValue);
				dataTable3.Rows.Add(dataRow2);
				ds.Tables.Add(dataTable3);
				DataTable dataTable4 = dtLine.Copy();
				dataTable4.TableName = "HDC1";
				foreach (string item in (from DataColumn t in dataTable4.Columns
					where !t.ColumnName.Equals("DocEntry") && !t.ColumnName.Equals("LineNum") && !t.ColumnName.StartsWith("U_")
					select t.ColumnName).ToList())
				{
					dataTable4.Columns.Remove(item);
				}
				ds.Tables.Add(dataTable4.Copy());
				HttpResult httpResult = await Save(ds);
				WinCommons._CloseProcessing();
				ds.Tables.Clear();
				ds.Dispose();
				if (string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.writeStatus(Enums.MessageCode.SaveSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
					FormMode = Enums.FormViewModeType.View;
					if (httpResult.Content != null)
					{
						txt_DocNum.EditValue = httpResult.Content;
					}
					txt_DocNum.Properties.ReadOnly = true;
					FormMode = Enums.FormViewModeType.View;
					PRINTLAYOUT();
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
	}

	private async Task<HttpResult> Save(DataSet ds)
	{
		return await HttpClientHelperFactory.GetHttpClient().Udo.Save(ds);
	}

	private async Task<DataTable> GetUdoByCode(Common.Table_Code item)
	{
		DataTable dt = new DataTable();
		try
		{
			using HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "UDO");
			dt = await helper.PostAsync("GetUdoByCode", item);
			if (dt == null)
			{
				return new DataTable();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return dt;
	}

	private async void menuMainClose_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txt_DocNum.Text) || Function.ParseInt(txt_DocNum.Text) <= 0)
			{
				return;
			}
			int num = Function.ParseInt(txt_DocNum.Text);
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmClose, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataSet ds = new DataSet();
				ds.Tables.Clear();
				ds.Tables.Add(UdoTable);
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("DocEntry", typeof(int));
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = num;
				dataTable.Rows.Add(dataRow);
				ds.Tables.Add(dataTable);
				string text = await OperatorDoc(ds, "CloseDoc");
				ds.Tables.Clear();
				ds.Dispose();
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.writeStatus(Enums.MessageCode.CloseSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
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

	private async void menuMainCancel_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(txt_DocNum.Text) || Function.ParseInt(txt_DocNum.Text) <= 0)
			{
				return;
			}
			int num = Function.ParseInt(txt_DocNum.Text);
			if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmCancel, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataSet ds = new DataSet();
				ds.Tables.Clear();
				ds.Tables.Add(UdoTable);
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("DocEntry", typeof(int));
				DataRow dataRow = dataTable.NewRow();
				dataRow["DocEntry"] = num;
				dataTable.Rows.Add(dataRow);
				ds.Tables.Add(dataTable);
				string text = await OperatorDoc(ds, "CancelDoc");
				ds.Tables.Clear();
				ds.Dispose();
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.writeStatus(Enums.MessageCode.CancelSuccess, Enums.MessageType.Success, Msg_Time._TimeMedium, "");
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

	private async Task<string> OperatorDoc(DataSet ds, string command)
	{
		try
		{
			using HttpClientHelper<DataSet> _help = new HttpClientHelper<DataSet>(Configuration.ApiServer, "UDO");
			return await _help.PostAsJsonAsync(command, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private void btn_Clear_Click(object sender, EventArgs e)
	{
		REFRESH();
	}

	private void gv_Batch_PopupMenuShowing(object sender, PopupMenuShowingEventArgs e)
	{
		try
		{
			GridViewMenu gridViewMenu = new GridViewMenu((GridView)sender);
			DXMenuItem dXMenuItem = new DXMenuItem();
			dXMenuItem.Caption = "Remove Row";
			dXMenuItem.Click += docMenu_Click;
			gridViewMenu.Items.Add(dXMenuItem);
			if (e.MenuType == GridMenuType.User || e.MenuType == GridMenuType.Row)
			{
				e.Allow = true;
				gridViewMenu.Show(e.Point);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void docMenu_Click(object sender, EventArgs e)
	{
		try
		{
			if ((sender as DXMenuItem).Caption.Equals("Remove Row") && gv_Document.FocusedRowHandle > -1 && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				gv_Document.DeleteRow(gv_Document.FocusedRowHandle);
				FormMode = Enums.FormViewModeType.Update;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_EmpCode_Properties_ButtonClick(object sender, ButtonPressedEventArgs e)
	{
		try
		{
			frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedCfl, false, bte_EmpCode.Text, "Apz_Cfl_GetEmployee", Enums.ItemObjectType.NoItemType, null, "");
			frm_CFL2.ShowDialog(this);
			DataRow chooseItem = frm_CFL2.ChooseItem;
			if (chooseItem != null && chooseItem.IsExistsColumn("ExtEmpNo"))
			{
				bte_EmpCode.Text = Function.ToString(chooseItem["ExtEmpNo"]);
				date_CreateDate.EditValue = DateTime.Today;
				grd_Document.DataSource = new DataTable();
				grd_Document.RefreshDataSource();
				look_Status.EditValue = "O";
				txt_DocNum.Text = "";
				txt_Document.Enabled = true;
				txt_Document.Text = "";
				txt_Document.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void bte_EmpCode_KeyDown(object sender, KeyEventArgs e)
	{
		try
		{
			if (e.KeyCode == Keys.Return && !string.IsNullOrEmpty(bte_EmpCode.Text))
			{
				date_CreateDate.EditValue = DateTime.Today;
				grd_Document.DataSource = new DataTable();
				grd_Document.RefreshDataSource();
				look_Status.EditValue = "O";
				txt_DocNum.Text = "";
				txt_Document.Enabled = true;
				txt_Document.Text = "";
				txt_Document.Focus();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void txt_Document_KeyDown(object sender, KeyEventArgs e)
	{
		_ = 3;
		try
		{
			if (e.KeyCode != Keys.Return || string.IsNullOrEmpty(Function.ToString(txt_Document.Text)))
			{
				return;
			}
			WinCommons._OpenProcessing();
			if (FormMode == Enums.FormViewModeType.Find)
			{
				if (string.IsNullOrEmpty(txt_DocNum.Text))
				{
					await SearchDocuments();
				}
				else
				{
					int type = 5;
					int currentId = Function.ParseInt(txt_DocNum.EditValue);
					await Navigate(currentId, type);
				}
			}
			else
			{
				DataSet ds = await GetDocumentDetail(bte_EmpCode.Text, txt_Document.Text, GlobalData.UserName, GlobalData.DefaultBranch);
				if (ds.IsHasStructure() && ds.Tables[1].IsNotNull())
				{
					DataTable dataSourceTable = grd_Document.DataSourceTable;
					if (!dataSourceTable.IsNotNull())
					{
						await GridViewHelper.GenerateGrid(grd_Document, gv_Document, ds);
					}
					else
					{
						string arg = Function.ToString(ds.Tables[1].Rows[0]["U_Barcode"]);
						DataRow[] array = dataSourceTable.Select($"U_Barcode = '{arg}' ");
						if (array.Length != 0)
						{
							array[0]["U_Slip"] = Function.ParseInt(array[0]["U_Slip"]) + 1;
							gv_Document.FocusedRowHandle = dataSourceTable.Rows.IndexOf(array[0]);
							txt_Document.Text = "";
							txt_Document.Focus();
							WinCommons._CloseProcessing();
							return;
						}
						foreach (DataRow row in ds.Tables[1].Rows)
						{
							DataRow dataRow2 = dataSourceTable.NewRow();
							dataRow2.ItemArray = row.ItemArray;
							dataSourceTable.Rows.Add(dataRow2);
						}
						gv_Document.FocusedRowHandle = dataSourceTable.Rows.Count - 1;
					}
					FormMode = Enums.FormViewModeType.Add;
					ds.Tables.Clear();
					ds.Dispose();
				}
				else
				{
					WinCommons.Error(Enums.MessageCode.None, "Không tìm thấy dữ liệu phù hợp!");
				}
				txt_Document.Text = "";
				txt_Document.Focus();
			}
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
			WinCommons._CloseProcessing();
		}
	}

	public async void PREVIEWLAYOUTS()
	{
		try
		{
			int num = Function.ParseInt(txt_DocNum.Text);
			await WinFuntions.PreviewLayout("", num, UdoCode, this, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async void PRINTLAYOUT()
	{
		try
		{
			int num = Function.ParseInt(txt_DocNum.Text);
			await WinFuntions.PrintLayout("", num, UdoCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	public async Task PRINT()
	{
		try
		{
			int docEntry = Function.ParseInt(txt_DocNum.Text);
			await WinFuntions.Print("", docEntry, UdoCode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataSet> GetDocumentDetail(string empCode, string barcode, string userName, int branch)
	{
		try
		{
			using HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "HandoverDocument");
			return await helper.GetAsync("GetDocumentDetail", new { empCode, barcode, userName, branch });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task GetById(int id, string barcode)
	{
		WinCommons._OpenProcessing();
		try
		{
			DataTable dataTable;
			using (HttpClientHelper<DataTable> helper = new HttpClientHelper<DataTable>(Configuration.ApiServer, "HandoverDocument"))
			{
				dataTable = await helper.GetAsync("GetDoc", new { id });
			}
			if (dataTable.IsNotNull())
			{
				txt_DocNum.EditValue = id;
				look_Status.EditValue = Function.ToString(dataTable.Rows[0]["Status"]);
				txt_UserName.EditValue = Function.ToString(dataTable.Rows[0]["Creator"]);
				txt_Remark.EditValue = Function.ToString(dataTable.Rows[0]["Remark"]);
				date_CreateDate.EditValue = Function.ParseDateTime(dataTable.Rows[0]["CreateDate"]);
			}
			await LoadDocuments(id, barcode);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		WinCommons._CloseProcessing();
	}

	private async Task LoadDocuments(int id, string barcode)
	{
		_ = 1;
		try
		{
			DataSet ds;
			using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "HandoverDocument"))
			{
				ds = await helper.GetAsync("LoadDocuments", new { id, barcode });
			}
			await GridViewHelper.GenerateGrid(grd_Document, gv_Document, ds);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task SearchDocuments()
	{
		_ = 1;
		try
		{
			DocumentsImpl documentsImpl = new DocumentsImpl();
			documentsImpl.DocEntry = Function.ParseInt(txt_DocNum.Text);
			documentsImpl.CardCode = txt_Document.Text;
			documentsImpl.CardName = bte_EmpCode.Text;
			documentsImpl.UserName = GlobalData.UserName;
			documentsImpl.Branch = GlobalData.DefaultBranch;
			if (date_CreateDate.EditValue != null)
			{
				documentsImpl.CreateDate = Function.ParseDateTimes(date_CreateDate.EditValue);
			}
			DataSet ds;
			using (HttpClientHelper<DataSet> helper = new HttpClientHelper<DataSet>(Configuration.ApiServer, "HandoverDocument"))
			{
				ds = await helper.PostAsync("SearchDocuments", documentsImpl);
			}
			await GridViewHelper.GenerateGrid(grd_Document, gv_Document, ds);
			bte_EmpCode.Text = "";
			txt_Document.Text = "";
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void NoAuthSetControlSearch()
	{
	}

	private async Task<DataTable> GetEmpId(DataTable dt)
	{
		try
		{
			using HttpClientHelper<DataTable> hp = new HttpClientHelper<DataTable>(Configuration.ApiServer, "EmpMasterData");
			return await hp.PostAsJsonAsyncTable("Search", dt);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataTable();
	}

	public async Task REFRESH()
	{
		try
		{
			date_CreateDate.EditValue = DateTime.Today;
			grd_Document.DataSource = new DataTable();
			grd_Document.RefreshDataSource();
			txt_DocNum.Properties.ReadOnly = true;
			look_Status.EditValue = "O";
			txt_DocNum.Text = "";
			txt_UserName.Text = GlobalData.UserName;
			bte_EmpCode.Text = "";
			bte_EmpCode.Focus();
			txt_Token.Text = "";
			txt_Remark.Text = "";
			txt_Document.Text = "";
			txt_Document.Enabled = false;
			txt_DocNum.ResetBackColor();
			bte_EmpCode.ResetBackColor();
			txt_Document.ResetBackColor();
			date_CreateDate.ResetBackColor();
			FormMode = Enums.FormViewModeType.Add;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void txt_Remark_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			FormMode = Enums.FormViewModeType.Update;
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Sales.frm_HandoverDocument));
		this.labelLine1 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine2 = new Apzon.Controls.Controls.LabelLine();
		this.txt_DocNum = new Apzon.Controls.Editor.TextEditEx();
		this.txt_UserName = new Apzon.Controls.Editor.TextEditEx();
		this.look_Status = new DevExpress.XtraEditors.LookUpEdit();
		this.labelLine6 = new Apzon.Controls.Controls.LabelLine();
		this.date_CreateDate = new Apzon.Controls.Editor.DateEditEx();
		this.grd_Document = new Apzon.Controls.Grid.GridControlEx();
		this.gv_Document = new Apzon.Controls.Grid.GridViewEx();
		this.ctx_Main = new System.Windows.Forms.ContextMenuStrip(this.components);
		this.menuMainClose = new System.Windows.Forms.ToolStripMenuItem();
		this.menuMainCancel = new System.Windows.Forms.ToolStripMenuItem();
		this.panelControl1 = new DevExpress.XtraEditors.PanelControl();
		this.txt_Remark = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine8 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine7 = new Apzon.Controls.Controls.LabelLine();
		this.txt_Token = new Apzon.Controls.Editor.TextEditEx();
		this.bte_EmpCode = new Apzon.Controls.Editor.ButtonEditEx();
		this.btn_Clear = new Apzon.Controls.Controls.SimpleButtonEx();
		this.txt_Document = new Apzon.Controls.Editor.TextEditEx();
		this.labelLine4 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine5 = new Apzon.Controls.Controls.LabelLine();
		this.labelLine3 = new Apzon.Controls.Controls.LabelLine();
		this.splitContainerControl1 = new DevExpress.XtraEditors.SplitContainerControl();
		this.btn_Save = new Apzon.Controls.Controls.SimpleButtonEx();
		((System.ComponentModel.ISupportInitialize)this.txt_DocNum.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_UserName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_CreateDate.Properties.CalendarTimeProperties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.date_CreateDate.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Document).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Document).BeginInit();
		this.ctx_Main.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.panelControl1).BeginInit();
		this.panelControl1.SuspendLayout();
		((System.ComponentModel.ISupportInitialize)this.txt_Remark.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Token.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.bte_EmpCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Document.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).BeginInit();
		this.splitContainerControl1.SuspendLayout();
		base.SuspendLayout();
		this.labelLine1.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine1.Location = new System.Drawing.Point(745, 6);
		this.labelLine1.Name = "labelLine1";
		this.labelLine1.Size = new System.Drawing.Size(63, 20);
		this.labelLine1.TabIndex = 0;
		this.labelLine1.Utext = "Doc. Num";
		this.labelLine2.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine2.Location = new System.Drawing.Point(605, 29);
		this.labelLine2.Name = "labelLine2";
		this.labelLine2.Size = new System.Drawing.Size(140, 20);
		this.labelLine2.TabIndex = 2;
		this.labelLine2.Utext = "Trạng thái";
		this.txt_DocNum.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_DocNum.Location = new System.Drawing.Point(805, 3);
		this.txt_DocNum.Name = "txt_DocNum";
		this.txt_DocNum.Properties.ReadOnly = true;
		this.txt_DocNum.Size = new System.Drawing.Size(85, 20);
		this.txt_DocNum.TabIndex = 4;
		this.txt_DocNum.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_DocNum_KeyDown);
		this.txt_UserName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.txt_UserName.Enabled = false;
		this.txt_UserName.Location = new System.Drawing.Point(637, 3);
		this.txt_UserName.Name = "txt_UserName";
		this.txt_UserName.Properties.ReadOnly = true;
		this.txt_UserName.Size = new System.Drawing.Size(106, 20);
		this.txt_UserName.TabIndex = 8;
		this.look_Status.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.look_Status.Location = new System.Drawing.Point(745, 29);
		this.look_Status.Name = "look_Status";
		this.look_Status.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.look_Status.Properties.NullText = "";
		this.look_Status.Size = new System.Drawing.Size(145, 20);
		this.look_Status.TabIndex = 3;
		this.labelLine6.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine6.Location = new System.Drawing.Point(606, 56);
		this.labelLine6.Name = "labelLine6";
		this.labelLine6.Size = new System.Drawing.Size(140, 20);
		this.labelLine6.TabIndex = 11;
		this.labelLine6.Utext = "Ngày bàn giao";
		this.date_CreateDate.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.date_CreateDate.EditValue = null;
		this.date_CreateDate.Location = new System.Drawing.Point(745, 55);
		this.date_CreateDate.Name = "date_CreateDate";
		this.date_CreateDate.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_CreateDate.Properties.CalendarTimeProperties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.date_CreateDate.Properties.DisplayFormat.FormatString = "dd/MM/yyyy";
		this.date_CreateDate.Properties.DisplayFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_CreateDate.Properties.EditFormat.FormatString = "dd/MM/yyyy";
		this.date_CreateDate.Properties.EditFormat.FormatType = DevExpress.Utils.FormatType.DateTime;
		this.date_CreateDate.Properties.Mask.EditMask = "dd/MM/yyyy";
		this.date_CreateDate.Size = new System.Drawing.Size(145, 20);
		this.date_CreateDate.TabIndex = 12;
		this.grd_Document.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.grd_Document.Location = new System.Drawing.Point(6, 1);
		this.grd_Document.MainView = this.gv_Document;
		this.grd_Document.Name = "grd_Document";
		this.grd_Document.Size = new System.Drawing.Size(893, 369);
		this.grd_Document.TabIndex = 0;
		this.grd_Document.ViewCollection.AddRange(new DevExpress.XtraGrid.Views.Base.BaseView[1] { this.gv_Document });
		this.gv_Document.GridControl = this.grd_Document;
		this.gv_Document.Name = "gv_Document";
		this.gv_Document.OptionsDetail.DetailMode = DevExpress.XtraGrid.Views.Grid.DetailMode.Default;
		this.gv_Document.PopupMenuShowing += new DevExpress.XtraGrid.Views.Grid.PopupMenuShowingEventHandler(this.gv_Batch_PopupMenuShowing);
		this.ctx_Main.Items.AddRange(new System.Windows.Forms.ToolStripItem[2] { this.menuMainClose, this.menuMainCancel });
		this.ctx_Main.Name = "ctx_Main";
		this.ctx_Main.Size = new System.Drawing.Size(111, 48);
		this.menuMainClose.Name = "menuMainClose";
		this.menuMainClose.Size = new System.Drawing.Size(110, 22);
		this.menuMainClose.Text = "Close";
		this.menuMainClose.Click += new System.EventHandler(this.menuMainClose_Click);
		this.menuMainCancel.Name = "menuMainCancel";
		this.menuMainCancel.Size = new System.Drawing.Size(110, 22);
		this.menuMainCancel.Text = "Cancel";
		this.menuMainCancel.Click += new System.EventHandler(this.menuMainCancel_Click);
		this.panelControl1.ContextMenuStrip = this.ctx_Main;
		this.panelControl1.Controls.Add(this.txt_Remark);
		this.panelControl1.Controls.Add(this.labelLine8);
		this.panelControl1.Controls.Add(this.labelLine7);
		this.panelControl1.Controls.Add(this.txt_Token);
		this.panelControl1.Controls.Add(this.bte_EmpCode);
		this.panelControl1.Controls.Add(this.btn_Clear);
		this.panelControl1.Controls.Add(this.txt_Document);
		this.panelControl1.Controls.Add(this.labelLine4);
		this.panelControl1.Controls.Add(this.labelLine5);
		this.panelControl1.Controls.Add(this.labelLine3);
		this.panelControl1.Controls.Add(this.date_CreateDate);
		this.panelControl1.Controls.Add(this.labelLine2);
		this.panelControl1.Controls.Add(this.look_Status);
		this.panelControl1.Controls.Add(this.labelLine6);
		this.panelControl1.Controls.Add(this.txt_UserName);
		this.panelControl1.Controls.Add(this.txt_DocNum);
		this.panelControl1.Controls.Add(this.labelLine1);
		this.panelControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.panelControl1.Location = new System.Drawing.Point(0, 0);
		this.panelControl1.Name = "panelControl1";
		this.panelControl1.Size = new System.Drawing.Size(899, 84);
		this.panelControl1.TabIndex = 20;
		this.txt_Remark.Location = new System.Drawing.Point(85, 55);
		this.txt_Remark.Name = "txt_Remark";
		this.txt_Remark.Size = new System.Drawing.Size(347, 20);
		this.txt_Remark.TabIndex = 63;
		this.txt_Remark.EditValueChanged += new System.EventHandler(this.txt_Remark_EditValueChanged);
		this.labelLine8.Location = new System.Drawing.Point(6, 57);
		this.labelLine8.Name = "labelLine8";
		this.labelLine8.Size = new System.Drawing.Size(80, 20);
		this.labelLine8.TabIndex = 62;
		this.labelLine8.Utext = "Ghi chú";
		this.labelLine7.Location = new System.Drawing.Point(309, 3);
		this.labelLine7.Name = "labelLine7";
		this.labelLine7.Size = new System.Drawing.Size(34, 20);
		this.labelLine7.TabIndex = 61;
		this.labelLine7.Utext = "Token";
		this.txt_Token.Location = new System.Drawing.Point(343, 2);
		this.txt_Token.Name = "txt_Token";
		this.txt_Token.Properties.PasswordChar = '*';
		this.txt_Token.Size = new System.Drawing.Size(89, 20);
		this.txt_Token.TabIndex = 60;
		this.bte_EmpCode.Location = new System.Drawing.Point(85, 3);
		this.bte_EmpCode.Name = "bte_EmpCode";
		this.bte_EmpCode.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Search)
		});
		this.bte_EmpCode.Properties.ButtonClick += new DevExpress.XtraEditors.Controls.ButtonPressedEventHandler(this.bte_EmpCode_Properties_ButtonClick);
		this.bte_EmpCode.Size = new System.Drawing.Size(212, 20);
		this.bte_EmpCode.TabIndex = 59;
		this.bte_EmpCode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.bte_EmpCode_KeyDown);
		this.btn_Clear.Image = (System.Drawing.Image)resources.GetObject("btn_Clear.Image");
		this.btn_Clear.Location = new System.Drawing.Point(356, 29);
		this.btn_Clear.Name = "btn_Clear";
		this.btn_Clear.Size = new System.Drawing.Size(76, 20);
		this.btn_Clear.TabIndex = 28;
		this.btn_Clear.Text = "Clear";
		this.btn_Clear.Click += new System.EventHandler(this.btn_Clear_Click);
		this.txt_Document.Enabled = false;
		this.txt_Document.Location = new System.Drawing.Point(85, 29);
		this.txt_Document.Name = "txt_Document";
		this.txt_Document.Size = new System.Drawing.Size(265, 20);
		this.txt_Document.TabIndex = 27;
		this.txt_Document.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_Document_KeyDown);
		this.labelLine4.Location = new System.Drawing.Point(5, 32);
		this.labelLine4.Name = "labelLine4";
		this.labelLine4.Size = new System.Drawing.Size(80, 20);
		this.labelLine4.TabIndex = 26;
		this.labelLine4.Utext = "Barcode tài liệu:";
		this.labelLine5.Location = new System.Drawing.Point(5, 6);
		this.labelLine5.Name = "labelLine5";
		this.labelLine5.Size = new System.Drawing.Size(80, 20);
		this.labelLine5.TabIndex = 24;
		this.labelLine5.Utext = "Mã Nhân Viên:";
		this.labelLine3.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.labelLine3.Location = new System.Drawing.Point(603, 5);
		this.labelLine3.Name = "labelLine3";
		this.labelLine3.Size = new System.Drawing.Size(34, 20);
		this.labelLine3.TabIndex = 23;
		this.labelLine3.Utext = "User";
		this.splitContainerControl1.Dock = System.Windows.Forms.DockStyle.Fill;
		this.splitContainerControl1.Horizontal = false;
		this.splitContainerControl1.Location = new System.Drawing.Point(0, 0);
		this.splitContainerControl1.Name = "splitContainerControl1";
		this.splitContainerControl1.Panel1.Controls.Add(this.panelControl1);
		this.splitContainerControl1.Panel1.Text = "Panel1";
		this.splitContainerControl1.Panel2.Controls.Add(this.btn_Save);
		this.splitContainerControl1.Panel2.Controls.Add(this.grd_Document);
		this.splitContainerControl1.Panel2.Text = "Panel2";
		this.splitContainerControl1.Size = new System.Drawing.Size(899, 491);
		this.splitContainerControl1.SplitterPosition = 84;
		this.splitContainerControl1.TabIndex = 21;
		this.splitContainerControl1.Text = "splitContainerControl1";
		this.btn_Save.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btn_Save.Image = (System.Drawing.Image)resources.GetObject("btn_Save.Image");
		this.btn_Save.Location = new System.Drawing.Point(6, 376);
		this.btn_Save.Name = "btn_Save";
		this.btn_Save.Size = new System.Drawing.Size(75, 23);
		this.btn_Save.TabIndex = 8;
		this.btn_Save.Text = "OK";
		this.btn_Save.Click += new System.EventHandler(this.btn_Save_Click);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(899, 491);
		base.Controls.Add(this.splitContainerControl1);
		base.Icon = (System.Drawing.Icon)resources.GetObject("$this.Icon");
		base.Name = "frm_HandoverDocument";
		base.ShowIcon = false;
		base.ShowInTaskbar = false;
		this.Text = "Document Transfer";
		base.Load += new System.EventHandler(this.frm_HandoverDocument_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_DocNum.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_UserName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.look_Status.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_CreateDate.Properties.CalendarTimeProperties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.date_CreateDate.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.grd_Document).EndInit();
		((System.ComponentModel.ISupportInitialize)this.gv_Document).EndInit();
		this.ctx_Main.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.panelControl1).EndInit();
		this.panelControl1.ResumeLayout(false);
		((System.ComponentModel.ISupportInitialize)this.txt_Remark.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Token.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.bte_EmpCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Document.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.splitContainerControl1).EndInit();
		this.splitContainerControl1.ResumeLayout(false);
		base.ResumeLayout(false);
	}
}
