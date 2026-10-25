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
using Apzon.Controls.Commons;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Entities.Models.Common;
using Apzon.Forms.HumanResources.Salary;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraGrid.Views.Base;

namespace Apzon.Forms.Commons;

public class frm_SalaryFormula : frm_UserDefinedProcedureCreate
{
	public int Branch;

	public int Dept;

	private bool _isChanging;

	private List<ViewEditModel> _listGridView = new List<ViewEditModel>();

	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	private IContainer components;

	public frm_SalaryFormula(XtraFormEx baseform, string formCode, int events = -1)
		: base(baseform, formCode, events)
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryCalculation"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_SalaryCalculation" && x.authoriza == "F"))
			{
				Controls.EnableDisableControl(isEnable: false);
				look_Branch.ReadOnly = (look_dep.ReadOnly = (look_Events.ReadOnly = (chk_Branch.ReadOnly = (chk_Dept.ReadOnly = false))));
			}
			FormId = "oSalaryFormula";
			chk_Branch.Visible = true;
			chk_Dept.Visible = true;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	protected override async void LoadEvents()
	{
		_ = 1;
		try
		{
			base.LoadEvents();
			DataTable dataTable = await _httpClient.EmployeeMasterClient.LoadComboDept();
			DataRow row = dataTable.NewRow();
			dataTable.Rows.InsertAt(row, 0);
			dataTable.AcceptChanges();
			look_dep.Properties.DataSource = dataTable.Copy();
			look_dep.Properties.DisplayMember = "Name";
			look_dep.Properties.ValueMember = "Code";
			look_dep.Properties.ForceInitialize();
			look_dep.Properties.PopulateColumns();
			look_dep.Properties.Columns["Code"].Visible = false;
			look_dep.Properties.Columns["Remarks"].Visible = false;
			DataTable dataTable2 = await _httpClient.EmployeeMasterClient.LoadComboBranch();
			DataRow row2 = dataTable2.NewRow();
			dataTable2.Rows.InsertAt(row2, 0);
			dataTable2.AcceptChanges();
			look_Branch.Properties.DataSource = dataTable2.Copy();
			look_Branch.Properties.DisplayMember = "Name";
			look_Branch.Properties.ValueMember = "Code";
			look_Branch.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
			look_Branch.Properties.ForceInitialize();
			look_Branch.Properties.PopulateColumns();
			look_Branch.Properties.Columns["Code"].Visible = false;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	protected override async void LoadDataSet()
	{
		_ = 1;
		try
		{
			DataTable dtDataSet = new DataTable
			{
				Columns = 
				{
					{
						"Name",
						typeof(string)
					},
					{
						"DataSet",
						typeof(string)
					}
				}
			};
			DataTable dtField = new DataTable
			{
				Columns = 
				{
					{
						"ColumnName",
						typeof(string)
					},
					{
						"FieldName",
						typeof(string)
					}
				}
			};
			if (BaseForm != null && BaseForm.GetType() != typeof(frm_SalaryResult) && BaseForm.GetType() != typeof(frm_SalaryViewResult))
			{
				_listGridView = ControlFunction.GetListEditGriview(BaseForm);
				if (_listGridView.Count > 0)
				{
					for (int i = 0; i < _listGridView.Count; i++)
					{
						if (string.IsNullOrEmpty(_listGridView[i].ItemId))
						{
							continue;
						}
						DataRow dataRow = dtDataSet.NewRow();
						dataRow["Name"] = (string.IsNullOrEmpty(_listGridView[i].ViewCaption) ? _listGridView[i].Name : _listGridView[i].ViewCaption);
						dataRow["DataSet"] = _listGridView[i].ItemId;
						dtDataSet.Rows.Add(dataRow);
						if (i == 0)
						{
							for (int j = 0; j < _listGridView[i].DataSource.Rows.Count; j++)
							{
								DataRow dataRow2 = dtField.NewRow();
								dataRow2["ColumnName"] = _listGridView[i].GetCaptionColumInView(_listGridView[i].ColumnEditFieldName);
								dataRow2["FieldName"] = _listGridView[i].ColumnEditFieldName;
								dtField.Rows.Add(dataRow2);
							}
						}
					}
				}
				grd_DataSet.DataSource = dtDataSet;
				grd_DataSet.RefreshDataSource();
				grd_Field.DataSource = dtField;
				grd_Field.RefreshDataSource();
				_dtFormula = new DataTable();
				_dtFormula.Columns.Add("FormID", typeof(string));
				_dtFormula.Columns.Add("MethodID", typeof(string));
				_dtFormula.Columns.Add("Description", typeof(string));
				_dtFormula.Columns.Add("Formula", typeof(string));
				_dtFormula.Columns.Add("Active", typeof(string));
				_dtFormula.Columns.Add("branch", typeof(int));
				_dtFormula.Columns.Add("dept", typeof(int));
				DataTable dataTable = new DataTable();
				dataTable.Columns.Add("Name", typeof(string));
				DataRow dataRow3 = dataTable.NewRow();
				dataRow3["Name"] = "Functions";
				dataTable.Rows.Add(dataRow3);
				dataRow3 = dataTable.NewRow();
				dataRow3["Name"] = "DataType";
				dataTable.Rows.Add(dataRow3);
				dataRow3 = dataTable.NewRow();
				dataRow3["Name"] = "Constants";
				dataTable.Rows.Add(dataRow3);
				dataRow3 = dataTable.NewRow();
				dataRow3["Name"] = "Operators";
				dataTable.Rows.Add(dataRow3);
				grd_Type.DataSource = dataTable;
				grd_Type.RefreshDataSource();
				if (Event > 0)
				{
					look_Events.EditValue = Event;
				}
				return;
			}
			DataRow dataRow4 = dtDataSet.NewRow();
			dataRow4["Name"] = "gv_Doc";
			dataRow4["DataSet"] = "APZ_OSAL";
			dtDataSet.Rows.Add(dataRow4);
			DataTable dataTable2 = new DataTable();
			dataTable2.Columns.Add("empId", typeof(string));
			dataTable2.Columns.Add("Period", typeof(string));
			dataTable2.Columns.Add("branch", typeof(int));
			DataRow dataRow5 = dataTable2.NewRow();
			dataRow5["empId"] = DBNull.Value;
			dataRow5["Period"] = DBNull.Value;
			dataRow5["branch"] = Function.ParseInt(look_Branch.EditValue);
			dataTable2.Rows.Add(dataRow5);
			dataTable2.AcceptChanges();
			DataSet ds1 = await _httpClient.SalaryCalculationClient.Calculate(dataTable2);
			DataTable source = await _httpClient.FormSettingClient.GetFormSetting("oSalaryResult", "APZ_OSAL", GlobalData.UserCodeLogin, GlobalData.CURRENTLANGUAGE);
			if (ds1.Tables.Count > 0)
			{
				int k;
				for (k = 0; k < ds1.Tables[0].Columns.Count; k++)
				{
					DataRow dataRow6 = dtField.NewRow();
					DataRow dataRow7 = source.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["ColumnId"]) == ds1.Tables[0].Columns[k].ColumnName);
					if (dataRow7 != null)
					{
						if (!string.IsNullOrEmpty(Function.ToString(dataRow7["ColumnName"])))
						{
							dataRow6["ColumnName"] = Function.ToString(dataRow7["ColumnName"]);
						}
						else
						{
							dataRow6["ColumnName"] = ds1.Tables[0].Columns[k].Caption;
						}
					}
					else
					{
						dataRow6["ColumnName"] = ds1.Tables[0].Columns[k].Caption;
					}
					dataRow6["FieldName"] = ds1.Tables[0].Columns[k].ColumnName;
					dtField.Rows.Add(dataRow6);
				}
			}
			grd_DataSet.DataSource = dtDataSet;
			grd_DataSet.RefreshDataSource();
			grd_Field.DataSource = dtField;
			grd_Field.RefreshDataSource();
			_dtFormula = new DataTable();
			_dtFormula.Columns.Add("FormID", typeof(string));
			_dtFormula.Columns.Add("MethodID", typeof(string));
			_dtFormula.Columns.Add("Description", typeof(string));
			_dtFormula.Columns.Add("Formula", typeof(string));
			_dtFormula.Columns.Add("Active", typeof(string));
			_dtFormula.Columns.Add("branch", typeof(int));
			_dtFormula.Columns.Add("dept", typeof(int));
			DataTable dataTable3 = new DataTable();
			dataTable3.Columns.Add("Name", typeof(string));
			DataRow dataRow8 = dataTable3.NewRow();
			dataRow8["Name"] = "Functions";
			dataTable3.Rows.Add(dataRow8);
			dataRow8 = dataTable3.NewRow();
			dataRow8["Name"] = "DataType";
			dataTable3.Rows.Add(dataRow8);
			dataRow8 = dataTable3.NewRow();
			dataRow8["Name"] = "Constants";
			dataTable3.Rows.Add(dataRow8);
			dataRow8 = dataTable3.NewRow();
			dataRow8["Name"] = "Operators";
			dataTable3.Rows.Add(dataRow8);
			grd_Type.DataSource = dataTable3;
			grd_Type.RefreshDataSource();
			if (Event > 0)
			{
				look_Events.EditValue = Event;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void InitValues()
	{
		try
		{
			if (Branch != 0)
			{
				chk_Branch.Checked = true;
				look_Branch.EditValue = Function.ParseShort(Branch);
			}
			if (Dept != 0)
			{
				chk_Dept.Checked = true;
				look_dep.EditValue = Function.ParseShort(Dept);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override async Task GetFormulaText()
	{
		try
		{
			string text = Function.ToString(look_Events.EditValue);
			int branch = Function.ParseInt(look_Branch.EditValue);
			int dept = Function.ParseInt(look_dep.EditValue);
			if (!string.IsNullOrEmpty(text))
			{
				Enums.EventId enumEvent = (Enums.EventId)Function.ParseInt(text);
				DataTable dataTable = await _httpClient.SalaryFormula.GetFormula(FormCode, enumEvent, branch, dept);
				if (dataTable.IsNotNull())
				{
					txt_Formula.Text = Function.ToString(dataTable.Rows[0]["Formula"]);
					txt_Description.EditValue = Function.ToString(dataTable.Rows[0]["Description"]);
					chk_Active.EditValue = Function.ToString(dataTable.Rows[0]["Active"]);
				}
				else
				{
					txt_Formula.Text = "";
				}
			}
			else
			{
				txt_Formula.Text = "";
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		finally
		{
			FormMode = Enums.FormViewModeType.View;
		}
	}

	protected override async void SaveData()
	{
		try
		{
			if (string.IsNullOrEmpty(FormCode))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseCategory, "");
				return;
			}
			string value = Function.ToString(txt_Formula.Text);
			if (string.IsNullOrEmpty(value))
			{
				WinCommons.Error(Enums.MessageCode.PleaseWriteFomularBeforeSaving, "");
				return;
			}
			string value2 = Function.ToString(look_Events.EditValue);
			if (string.IsNullOrEmpty(value2))
			{
				WinCommons.Error(Enums.MessageCode.PleaseChooseEventType, "");
				return;
			}
			string text = Verify();
			if (!string.IsNullOrEmpty(text))
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				DataTable dataTable = _dtFormula.Clone();
				DataRow dataRow = dataTable.NewRow();
				dataRow["FormID"] = FormCode;
				dataRow["MethodID"] = value2;
				dataRow["Description"] = Function.ToString(txt_Description.EditValue);
				dataRow["Formula"] = value;
				dataRow["Active"] = Function.ToString(chk_Active.EditValue);
				dataRow["branch"] = Function.ParseInt(look_Branch.EditValue);
				dataRow["dept"] = Function.ParseInt(look_dep.EditValue);
				dataTable.Rows.Add(dataRow);
				HttpResult httpResult = await _httpClient.SalaryFormula.Save(dataTable);
				if (!string.IsNullOrEmpty(httpResult.Message))
				{
					WinCommons.Error(Enums.MessageCode.None, httpResult.Message);
					return;
				}
				WinCommons.Success(Enums.MessageCode.Success, "");
				FormMode = Enums.FormViewModeType.View;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override void gv_DataSet_FocusedRowObjectChanged(object sender, FocusedRowObjectChangedEventArgs e)
	{
	}

	private async void look_Branch_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			LoadDataSet();
			await GetFormulaText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_Branch_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void look_dep_EditValueChanged(object sender, EventArgs e)
	{
		try
		{
			await GetFormulaText();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void look_dep_EditValueChanging(object sender, ChangingEventArgs e)
	{
		try
		{
			if (FormMode != Enums.FormViewModeType.View && XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmContinueNotSave, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
			{
				e.Cancel = true;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_Branch_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isChanging)
			{
				if (chk_Branch.Checked)
				{
					look_Branch.Visible = true;
					return;
				}
				_isChanging = true;
				look_Branch.EditValue = null;
				_isChanging = false;
				look_Branch.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void chk_Dept_CheckedChanged(object sender, EventArgs e)
	{
		try
		{
			if (!_isChanging)
			{
				if (chk_Dept.Checked)
				{
					look_dep.Visible = true;
					return;
				}
				_isChanging = true;
				look_dep.EditValue = null;
				_isChanging = false;
				look_dep.Visible = false;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	protected override async void btnDelete_Click(object sender, EventArgs e)
	{
		try
		{
			if (string.IsNullOrEmpty(FormCode))
			{
				WinCommons.Error(Enums.MessageCode.FormIdCannotBeNull, "");
				return;
			}
			int branch = Function.ParseInt(look_Branch.EditValue);
			int dept = Function.ParseInt(look_dep.EditValue);
			int num = Function.ParseInt(look_Events.EditValue);
			if (num == 0)
			{
				WinCommons.Error(Enums.MessageCode.EventIdCannotBeNull, "");
			}
			else if (XtraMessageBoxEx.Show(Enums.MessageCode.ConfirmDelete, Enums.MessageCode.SystemMessage, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
			{
				string text = await _httpClient.SalaryFormula.DeleteFormula(FormCode, (Enums.EventId)num, branch, dept);
				if (string.IsNullOrEmpty(text))
				{
					WinCommons.Success(Enums.MessageCode.DeleteSuccess, "");
					txt_Formula.Text = "";
					FormMode = Enums.FormViewModeType.View;
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
		((System.ComponentModel.ISupportInitialize)base._dtFormula).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.look_Events.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.txt_Description.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.chk_Active.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.look_Branch.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.look_dep.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.chk_Dept.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)base.chk_Branch.Properties).BeginInit();
		base.SuspendLayout();
		base.look_Events.TabIndex = 1;
		base.txt_Description.TabIndex = 2;
		base.chk_Active.TabIndex = 3;
		base.look_Branch.TabIndex = 5;
		base.look_Branch.EditValueChanged += new System.EventHandler(this.look_Branch_EditValueChanged);
		base.look_Branch.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.look_Branch_EditValueChanging);
		base.look_dep.TabIndex = 7;
		base.look_dep.EditValueChanged += new System.EventHandler(this.look_dep_EditValueChanged);
		base.look_dep.EditValueChanging += new DevExpress.XtraEditors.Controls.ChangingEventHandler(this.look_dep_EditValueChanging);
		base.chk_Dept.TabIndex = 6;
		base.chk_Dept.CheckedChanged += new System.EventHandler(this.chk_Dept_CheckedChanged);
		base.chk_Branch.TabIndex = 4;
		base.chk_Branch.CheckedChanged += new System.EventHandler(this.chk_Branch_CheckedChanged);
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.ClientSize = new System.Drawing.Size(1237, 518);
		base.Name = "frm_SalaryFormula";
		((System.ComponentModel.ISupportInitialize)base._dtFormula).EndInit();
		((System.ComponentModel.ISupportInitialize)base.look_Events.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.txt_Description.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.chk_Active.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.look_Branch.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.look_dep.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.chk_Dept.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)base.chk_Branch.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
