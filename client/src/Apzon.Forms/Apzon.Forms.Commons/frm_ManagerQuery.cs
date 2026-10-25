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
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Administrator;
using Apzon.Forms.Administrator;
using Apzon.Forms.Commons.Queries;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors;

namespace Apzon.Forms.Commons;

public class frm_ManagerQuery : XtraFormEx
{
	private DataSet _dtset;

	private readonly List<LinkLabel> _category = new List<LinkLabel>();

	private readonly List<LinkLabel> _queryName = new List<LinkLabel>();

	public string Type = "form";

	public string Captonbtnsave = Resources.ButtonOK;

	private int _showquery;

	public string SqlSave = "";

	public string SqlOpen = "";

	public string QueryName = "";

	private int _currentCatLink;

	public int QueryId;

	private IContainer components;

	private LabelLine lbNameQuery;

	private TextEditEx txtQeryName;

	private LabelLine lbQueryCalegory;

	private TextEditEx txtQeryCalegory;

	private SimpleButtonEx btnManagerCalegories;

	private SimpleButtonEx btnSave;

	private SimpleButtonEx btnCancel;

	private SimpleButtonEx btnSchedule;

	private SimpleButtonEx btnRemove;

	private XtraScrollableControl pancategory;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Update:
			btnSave.Text = Resources.ButtonSave;
			break;
		case Enums.FormViewModeType.View:
			btnSave.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_ManagerQuery()
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ManagerQuery"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ManagerQuery" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnSave.Enabled = false;
				btnRemove.Enabled = false;
			}
			else
			{
				btnSave.Enabled = true;
				btnRemove.Enabled = true;
			}
			FormMode = Enums.FormViewModeType.View;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	public frm_ManagerQuery(Enums.FormViewModeType viewMode)
	{
		if (GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ManagerQuery"))
		{
			InitializeComponent();
			if (!GlobalData.ListAccessForm.Exists((AuthsImpl x) => x.menucode == "frm_ManagerQuery" && x.authoriza == "F"))
			{
				WinFuntions.DisableAllControl(Controls);
				btnSave.Enabled = false;
				btnRemove.Enabled = false;
			}
			else
			{
				btnSave.Enabled = true;
				btnRemove.Enabled = true;
			}
			FormMode = viewMode;
			return;
		}
		throw new Exception(Resources.Msg_FormAccessDenied);
	}

	private async void frm_SaveQuery_Load(object sender, EventArgs e)
	{
		try
		{
			await Reload();
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
			txtQeryCalegory.EditValue = "";
			txtQeryName.EditValue = "";
			_showquery = 0;
			_queryName.Clear();
			_currentCatLink = 0;
			_category.Clear();
			await Reload();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LbOnClick(object sender, EventArgs e)
	{
		try
		{
			int num = 1;
			int num2 = _category[_showquery].Location.Y;
			if (_queryName.Count > 0)
			{
				for (int i = _showquery + 1; i < _category.Count; i++)
				{
					_category[i].Location = new Point(20, num2 + num * 25);
					num++;
				}
			}
			for (int num3 = _queryName.Count - 1; num3 >= 0; num3--)
			{
				pancategory.Controls.Remove(_queryName[num3]);
				_queryName.Remove(_queryName[num3]);
			}
			txtQeryName.Text = "";
			txtQeryCalegory.Text = ((LinkLabel)sender).Text;
			if (_currentCatLink == num2)
			{
				_currentCatLink = 0;
				return;
			}
			_currentCatLink = num2;
			num = 1;
			num2 = ((LinkLabel)sender).Location.Y;
			int num4 = -1;
			for (int j = 0; j < _dtset.Tables[0].Rows.Count; j++)
			{
				if (Function.ToString(_dtset.Tables[0].Rows[j]["catname"]) == ((LinkLabel)sender).Text)
				{
					num4 = Function.ParseInt(_dtset.Tables[0].Rows[j]["Categoryid"]);
				}
			}
			if (_dtset.Tables[1] != null && _dtset.Tables[1].Rows.Count > 0)
			{
				for (int k = 0; k < _dtset.Tables[1].Rows.Count; k++)
				{
					if (Function.ParseInt(_dtset.Tables[1].Rows[k]["Qcategory"]) == num4)
					{
						LinkLabel linkLabel = new LinkLabel
						{
							Name = "lb" + k,
							Location = new Point(30, num2 + num * 25),
							Size = new Size(300, 20),
							Anchor = (AnchorStyles.Top | AnchorStyles.Left),
							TabIndex = 0,
							Text = Function.ToString(_dtset.Tables[1].Rows[k]["Qname"])
						};
						linkLabel.Click += LinkOnClick;
						linkLabel.DoubleClick += Link_DoubleClick;
						pancategory.Controls.Add(linkLabel);
						num++;
						_queryName.Add(linkLabel);
					}
				}
			}
			if (num == 1)
			{
				LinkLabel linkLabel2 = new LinkLabel
				{
					Name = "lb",
					Location = new Point(30, num2 + num * 25),
					Size = new Size(300, 20),
					Anchor = (AnchorStyles.Top | AnchorStyles.Left),
					TabIndex = 0,
					Text = "No queries belong to this group",
					Enabled = false
				};
				pancategory.Controls.Add(linkLabel2);
				num++;
				_queryName.Add(linkLabel2);
			}
			for (int l = 0; l < _category.Count; l++)
			{
				if (_category[l].Text == ((LinkLabel)sender).Text)
				{
					for (int m = l + 1; m < _category.Count; m++)
					{
						_category[m].Location = new Point(20, num2 + num * 25);
						num++;
					}
					_showquery = l;
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

	private void Link_DoubleClick(object sender, EventArgs e)
	{
		try
		{
			LinkOnClick(sender, e);
			btnSave_Click(null, null);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void LinkOnClick(object sender, EventArgs eventArgs)
	{
		try
		{
			txtQeryName.Text = ((LinkLabel)sender).Text;
			for (int i = 0; i < _dtset.Tables[1].Rows.Count; i++)
			{
				if (Function.ToString(_dtset.Tables[1].Rows[i]["Qname"]) == txtQeryName.Text)
				{
					SqlOpen = Function.ToString(_dtset.Tables[1].Rows[i]["Qstring"]);
					QueryId = Function.ParseInt(_dtset.Tables[1].Rows[i]["ID"]);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnSave_Click(object sender, EventArgs e)
	{
		try
		{
			if (FormMode == Enums.FormViewModeType.Update)
			{
				if (string.IsNullOrEmpty(txtQeryName.Text))
				{
					WinCommons.Error(Enums.MessageCode.NoEntryCatalogName, "");
				}
				else if (string.IsNullOrEmpty(txtQeryCalegory.Text))
				{
					WinCommons.Error(Enums.MessageCode.NoEntryCategory, "");
				}
				else if (string.IsNullOrEmpty(SqlSave))
				{
					WinCommons.Error(Enums.MessageCode.QueryIsNull, "");
				}
				else
				{
					await Save();
				}
			}
			else if (Type == "form")
			{
				frm_QueryPreview frm_QueryPreview2 = new frm_QueryPreview();
				frm_QueryPreview2.MdiParent = MdiParent;
				frm_QueryPreview2.Sql = SqlOpen;
				frm_QueryPreview2.QueryId = QueryId;
				frm_QueryPreview2.Show();
			}
			else if (Type == "dialog")
			{
				QueryName = txtQeryName.Text;
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private void btnManagerCalegories_Click(object sender, EventArgs e)
	{
		try
		{
			frm_ManagerCategories frm_ManagerCategories2 = new frm_ManagerCategories();
			frm_ManagerCategories2.RefreshManager = (RefreshQueryManager)Delegate.Combine(frm_ManagerCategories2.RefreshManager, new RefreshQueryManager(REFRESH_DATA));
			frm_ManagerCategories2.ShowDialog(this);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btnRemove_Click(object sender, EventArgs e)
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Queries");
			string text = await http.PostAsync("DeleteQuery", new Tuple<string, string>(txtQeryName.Text, txtQeryCalegory.Text));
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				txtQeryCalegory.EditValue = "";
				txtQeryName.EditValue = "";
				REFRESH_DATA();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task Reload()
	{
		_ = 1;
		try
		{
			pancategory.Controls.Clear();
			pancategory.Refresh();
			string filter = await WinFuntions.ShowFilterSetting("OUQR");
			int stt = 0;
			_dtset = await GetDataCategories(filter);
			if (_dtset.Tables.Count != 2 || _dtset.Tables[0] == null || _dtset.Tables[0].Rows.Count <= 0)
			{
				return;
			}
			_ = _dtset.Tables[1];
			for (int i = 0; i < _dtset.Tables[0].Rows.Count; i++)
			{
				Function.ParseInt(_dtset.Tables[0].Rows[i]["Categoryid"]);
				if (!string.IsNullOrEmpty(Function.ToString(_dtset.Tables[0].Rows[i]["permMask"])) && Function.ToString(_dtset.Tables[0].Rows[i]["permMask"]).Contains('Y'))
				{
					LinkLabel linkLabel = new LinkLabel
					{
						Name = "lb" + i,
						Location = new Point(20, 10 + stt * 25),
						Size = new Size(200, 20),
						Anchor = (AnchorStyles.Top | AnchorStyles.Left),
						TabIndex = 0,
						LinkColor = Color.Black,
						Text = Function.ToString(_dtset.Tables[0].Rows[i]["catname"])
					};
					linkLabel.Click += LbOnClick;
					pancategory.Controls.Add(linkLabel);
					stt++;
					_category.Add(linkLabel);
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async Task<DataSet> GetDataCategories(string filter = "")
	{
		try
		{
			using HttpClientHelper<DataSet> http = new HttpClientHelper<DataSet>(Configuration.ApiServer, "Commons");
			return await http.GetAsync("GetDataCategories", new { filter });
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return new DataSet();
	}

	private async Task Save()
	{
		try
		{
			using HttpClientHelper<string> http = new HttpClientHelper<string>(Configuration.ApiServer, "Commons");
			string text = await http.PostAsync("SaveQuery", new Tuple<string, string, string>(SqlSave, txtQeryName.Text, txtQeryCalegory.Text));
			if (string.IsNullOrEmpty(text))
			{
				WinCommons.Success(Enums.MessageCode.Success, "");
				Close();
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.None, text);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.frm_ManagerQuery));
		this.txtQeryName = new Apzon.Controls.Editor.TextEditEx();
		this.txtQeryCalegory = new Apzon.Controls.Editor.TextEditEx();
		this.btnManagerCalegories = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSave = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnCancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnSchedule = new Apzon.Controls.Controls.SimpleButtonEx();
		this.btnRemove = new Apzon.Controls.Controls.SimpleButtonEx();
		this.pancategory = new DevExpress.XtraEditors.XtraScrollableControl();
		this.lbQueryCalegory = new Apzon.Controls.Controls.LabelLine();
		this.lbNameQuery = new Apzon.Controls.Controls.LabelLine();
		((System.ComponentModel.ISupportInitialize)this.txtQeryName.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txtQeryCalegory.Properties).BeginInit();
		base.SuspendLayout();
		this.txtQeryName.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtQeryName.FieldName = null;
		this.txtQeryName.IsSearchControl = false;
		this.txtQeryName.Location = new System.Drawing.Point(98, 10);
		this.txtQeryName.Name = "txtQeryName";
		this.txtQeryName.Size = new System.Drawing.Size(318, 20);
		this.txtQeryName.TabIndex = 1;
		this.txtQeryCalegory.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.txtQeryCalegory.FieldName = null;
		this.txtQeryCalegory.IsSearchControl = false;
		this.txtQeryCalegory.Location = new System.Drawing.Point(98, 39);
		this.txtQeryCalegory.Name = "txtQeryCalegory";
		this.txtQeryCalegory.Size = new System.Drawing.Size(318, 20);
		this.txtQeryCalegory.TabIndex = 3;
		this.btnManagerCalegories.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right;
		this.btnManagerCalegories.FieldName = null;
		this.btnManagerCalegories.IsSearchControl = false;
		this.btnManagerCalegories.Location = new System.Drawing.Point(430, 36);
		this.btnManagerCalegories.Name = "btnManagerCalegories";
		this.btnManagerCalegories.Size = new System.Drawing.Size(106, 23);
		this.btnManagerCalegories.TabIndex = 4;
		this.btnManagerCalegories.Text = "Manager Categories";
		this.btnManagerCalegories.Click += new System.EventHandler(this.btnManagerCalegories_Click);
		this.btnSave.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnSave.FieldName = null;
		this.btnSave.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnSave.ImageOptions.Image");
		this.btnSave.IsSearchControl = false;
		this.btnSave.Location = new System.Drawing.Point(9, 328);
		this.btnSave.Name = "btnSave";
		this.btnSave.Size = new System.Drawing.Size(65, 23);
		this.btnSave.TabIndex = 6;
		this.btnSave.Text = "Open";
		this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
		this.btnCancel.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnCancel.FieldName = null;
		this.btnCancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnCancel.ImageOptions.Image");
		this.btnCancel.IsSearchControl = false;
		this.btnCancel.Location = new System.Drawing.Point(80, 329);
		this.btnCancel.Name = "btnCancel";
		this.btnCancel.Size = new System.Drawing.Size(69, 23);
		this.btnCancel.TabIndex = 7;
		this.btnCancel.Text = "Cancel";
		this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
		this.btnSchedule.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left;
		this.btnSchedule.Enabled = false;
		this.btnSchedule.FieldName = null;
		this.btnSchedule.IsSearchControl = false;
		this.btnSchedule.Location = new System.Drawing.Point(155, 328);
		this.btnSchedule.Name = "btnSchedule";
		this.btnSchedule.Size = new System.Drawing.Size(69, 23);
		this.btnSchedule.TabIndex = 8;
		this.btnSchedule.Text = "Schedule";
		this.btnSchedule.Visible = false;
		this.btnRemove.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btnRemove.Enabled = false;
		this.btnRemove.FieldName = null;
		this.btnRemove.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btnRemove.ImageOptions.Image");
		this.btnRemove.IsSearchControl = false;
		this.btnRemove.Location = new System.Drawing.Point(460, 329);
		this.btnRemove.Name = "btnRemove";
		this.btnRemove.Size = new System.Drawing.Size(75, 23);
		this.btnRemove.TabIndex = 9;
		this.btnRemove.Text = "Remove";
		this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
		this.pancategory.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.pancategory.Appearance.BackColor = System.Drawing.Color.White;
		this.pancategory.Appearance.Options.UseBackColor = true;
		this.pancategory.Location = new System.Drawing.Point(9, 65);
		this.pancategory.Name = "pancategory";
		this.pancategory.Size = new System.Drawing.Size(527, 258);
		this.pancategory.TabIndex = 10;
		this.lbQueryCalegory.FieldName = null;
		this.lbQueryCalegory.HideWhenEmpty = false;
		this.lbQueryCalegory.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbQueryCalegory.Location = new System.Drawing.Point(13, 42);
		this.lbQueryCalegory.Name = "lbQueryCalegory";
		this.lbQueryCalegory.ShowUnderLine = true;
		this.lbQueryCalegory.Size = new System.Drawing.Size(80, 20);
		this.lbQueryCalegory.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbQueryCalegory.TabIndex = 2;
		this.lbQueryCalegory.Utext = "Query Calegory";
		this.lbNameQuery.FieldName = null;
		this.lbNameQuery.HideWhenEmpty = false;
		this.lbNameQuery.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lbNameQuery.Location = new System.Drawing.Point(13, 13);
		this.lbNameQuery.Name = "lbNameQuery";
		this.lbNameQuery.ShowUnderLine = true;
		this.lbNameQuery.Size = new System.Drawing.Size(80, 20);
		this.lbNameQuery.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lbNameQuery.TabIndex = 0;
		this.lbNameQuery.Utext = "Query Name";
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(545, 358);
		base.Controls.Add(this.pancategory);
		base.Controls.Add(this.btnRemove);
		base.Controls.Add(this.btnSchedule);
		base.Controls.Add(this.btnCancel);
		base.Controls.Add(this.btnSave);
		base.Controls.Add(this.btnManagerCalegories);
		base.Controls.Add(this.txtQeryCalegory);
		base.Controls.Add(this.lbQueryCalegory);
		base.Controls.Add(this.txtQeryName);
		base.Controls.Add(this.lbNameQuery);
		base.FormId = "oManagerQuery";
		base.Name = "frm_ManagerQuery";
		base.ShowIcon = false;
		this.Text = "Query manager";
		base.Load += new System.EventHandler(this.frm_SaveQuery_Load);
		((System.ComponentModel.ISupportInitialize)this.txtQeryName.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txtQeryCalegory.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
