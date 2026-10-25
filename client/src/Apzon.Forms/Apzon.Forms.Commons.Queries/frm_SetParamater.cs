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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;

namespace Apzon.Forms.Commons.Queries;

public class frm_SetParamater : XtraForm
{
	public List<DatabaseParamater> List = new List<DatabaseParamater>();

	public List<QueryParamater> Paramater = new List<QueryParamater>();

	private readonly List<dynamic> _lstEdit = new List<object>();

	private DataTable _dtColuTable;

	private IContainer components;

	private SimpleButtonEx btn_Ok;

	private XtraScrollableControl paneParamater;

	private SimpleButtonEx simpleButton1;

	public frm_SetParamater()
	{
		InitializeComponent();
	}

	private async void frm_SetParamater_Load(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (List == null || List.Count <= 0)
			{
				return;
			}
			_dtColuTable = await GetInfoColum();
			if (_dtColuTable != null && _dtColuTable.Rows.Count > 0)
			{
				Size = new Size(385, (_dtColuTable.Rows.Count < 10) ? ((_dtColuTable.Rows.Count + 2) * 23 + 100) : 400);
				paneParamater.Size = new Size(380, (_dtColuTable.Rows.Count < 10) ? ((_dtColuTable.Rows.Count + 1) * 23 + 50) : 300);
				Refresh();
				int tab = 1;
				for (int i = 0; i < _dtColuTable.Rows.Count; i++)
				{
					paneParamater.Controls.Add(new LabelLine
					{
						Name = "lb" + Function.ToString(_dtColuTable.Rows[i]["ColumnField"]) + i,
						Location = new Point(10, (i + 2) * 23 - 15),
						Size = new Size(150, 20),
						Anchor = (AnchorStyles.Top | AnchorStyles.Left),
						TabIndex = 0,
						Utext = string.Format("{0}{1}", (List[i].ParaType == ParaType.Normal) ? "" : ((List[i].ParaType == ParaType.FromBetween) ? "From " : "To "), Function.ToString(_dtColuTable.Rows[i]["ColumnName"]).Replace("\"", ""))
					});
					switch (Function.ToString(_dtColuTable.Rows[i]["DataType"]).ToLower())
					{
					case "datetime":
					case "date":
					case "timestamp":
					{
						DateEditEx dateEditEx = new DateEditEx
						{
							Location = new Point(160, (i + 2) * 23 - 18),
							Name = "Dparamater" + i,
							Anchor = (AnchorStyles.Top | AnchorStyles.Left)
						};
						dateEditEx.Properties.NullText = "";
						dateEditEx.Size = new Size(180, 20);
						dateEditEx.Properties.Buttons[0].Kind = ButtonPredefines.DropDown;
						dateEditEx.TabIndex = tab++;
						dateEditEx.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
						paneParamater.Controls.Add(dateEditEx);
						dateEditEx.EditValue = null;
						_lstEdit.Add(dateEditEx);
						break;
					}
					case "smallint":
					case "int":
					case "bigint":
					case "tinyint":
						CreateCal(i, tab++, Enums.DisplayFormatType.Integer);
						break;
					case "double":
					case "float":
					case "numeric":
					case "decimal":
					case "money":
					case "smallmoney":
					case "real":
						CreateCal(i, tab++, Enums.DisplayFormatType.Qty);
						break;
					default:
						if (!List[i].Combobox)
						{
							TextEditEx textEditEx = new TextEditEx
							{
								Location = new Point(160, (i + 2) * 23 - 18),
								Name = "Dparamater" + i,
								Anchor = (AnchorStyles.Top | AnchorStyles.Left)
							};
							textEditEx.Properties.NullText = "";
							textEditEx.Size = new Size(180, 20);
							textEditEx.TabIndex = tab++;
							textEditEx.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
							paneParamater.Controls.Add(textEditEx);
							_lstEdit.Add(textEditEx);
						}
						break;
					}
					if (List[i].Combobox)
					{
						LookUpEditEx lk = new LookUpEditEx();
						lk.Location = new Point(160, (i + 2) * 23 - 18);
						lk.Name = "paramater" + _lstEdit.Count;
						lk.Anchor = AnchorStyles.Top | AnchorStyles.Left;
						lk.Size = new Size(180, 20);
						lk.TabIndex = tab++;
						lk.Properties.NullText = "";
						int count = _lstEdit.Count;
						dynamic val = ((count > 0) ? Function.ParseInt(_lstEdit[count - 1].Name.Substring(10)) : ((object)0));
						string text = Function.ToString(_dtColuTable.Rows[val]["ColumnField"]);
						dynamic val2 = "select distinct " + text.Replace("[", "").Replace("]", "") + " as \"Value\" from " + _dtColuTable.Rows[val]["TableName"] + ";";
						object dataSource = await GetData(val2);
						lk.Properties.DataSource = dataSource;
						RepositoryItemLookUpEdit properties = lk.Properties;
						string valueMember = (lk.Properties.DisplayMember = "Value");
						properties.ValueMember = valueMember;
						paneParamater.Controls.Add(lk);
						_lstEdit.Add(lk);
					}
					else
					{
						SimpleButton simpleButton = new SimpleButton
						{
							Location = new Point(340, (i + 2) * 23 - 18),
							Name = "paramater" + _lstEdit.Count,
							Anchor = (AnchorStyles.Top | AnchorStyles.Left),
							Size = new Size(25, 21),
							TabIndex = tab++
						};
						simpleButton.Click += BtnOnButtonClick;
						simpleButton.Image = (Image)new ComponentResourceManager(typeof(frm_SetParamater)).GetObject("simpleButton1.ImageOptions.Image");
						paneParamater.Controls.Add(simpleButton);
						_lstEdit.Add(simpleButton);
					}
				}
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.InfoCannotLoadData, "");
				Close();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private void CreateCal(int i, int index, Enums.DisplayFormatType type)
	{
		CalcEditEx calcEditEx = new CalcEditEx
		{
			Location = new Point(160, (i + 2) * 23 - 18),
			Name = "Dparamater" + i,
			Anchor = (AnchorStyles.Top | AnchorStyles.Left)
		};
		calcEditEx.SetFormatControl(type);
		calcEditEx.Properties.NullText = "";
		calcEditEx.Size = new Size(180, 20);
		calcEditEx.Properties.Buttons[0].Kind = ButtonPredefines.DropDown;
		calcEditEx.TabIndex = index;
		calcEditEx.Properties.Appearance.TextOptions.HAlignment = HorzAlignment.Near;
		paneParamater.Controls.Add(calcEditEx);
		_lstEdit.Add(calcEditEx);
	}

	private async void BtnOnButtonClick(object sender, EventArgs e)
	{
		try
		{
			int id = Function.ParseInt(((SimpleButton)sender).Name.Substring(9));
			dynamic val = Function.ParseInt(_lstEdit[id - 1].Name.Substring(10));
			string text = Function.ToString(_dtColuTable.Rows[val]["ColumnField"]);
			dynamic val2 = "select distinct " + text.Replace("[", "").Replace("]", "") + " as \"Value\" from " + _dtColuTable.Rows[val]["TableName"] + ";";
			dynamic val3 = await GetData(val2);
			if (val3 != null && val3.Rows.Count >= 0)
			{
				CFLParamaterSql cFLParamaterSql = new CFLParamaterSql
				{
					Dt = val3
				};
				cFLParamaterSql.ShowDialog(this);
				if (cFLParamaterSql.Value != null)
				{
					_lstEdit[id - 1].EditValue = cFLParamaterSql.Value["Value"];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
	}

	private async Task<DataTable> GetData(string sql)
	{
		try
		{
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "Commons");
			return await http.PostAsync("GetDataParamaterSql", sql);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			return new DataTable();
		}
	}

	private void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			for (int i = 0; i < _dtColuTable.Rows.Count; i++)
			{
				QueryParamater queryParamater = new QueryParamater();
				queryParamater.DataType = Function.ToString(_dtColuTable.Rows[i]["DataType"]);
				queryParamater.ParaName = Function.ToString(_dtColuTable.Rows[i]["Para"]);
				queryParamater.Value = (object)_lstEdit[i * 2].EditValue;
				QueryParamater item = queryParamater;
				Paramater.Add(item);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
		}
		DialogResult = DialogResult.OK;
		Close();
	}

	private void EnableDisableControl(bool isEnable)
	{
		try
		{
			if (_lstEdit != null && _lstEdit.Count != 0)
			{
				for (int i = 0; i < _lstEdit.Count; i++)
				{
					_lstEdit[i].Enabled = isEnable;
				}
				btn_Ok.Enabled = isEnable;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}

	private async Task<DataTable> GetInfoColum()
	{
		try
		{
			if (List == null || List.Count == 0)
			{
				return new DataTable();
			}
			DataTable dataTable = new DataTable();
			dataTable.Columns.Add("Para", typeof(string));
			dataTable.Columns.Add("Column", typeof(string));
			dataTable.Columns.Add("Table", typeof(string));
			dataTable.BeginLoadData();
			for (int i = 0; i < List.Count; i++)
			{
				DataRow dataRow = dataTable.NewRow();
				dataRow["Para"] = List[i].ParaName;
				string text = (string)(dataRow["Column"] = List[i].ColumnName);
				if (text.LastIndexOf(".", StringComparison.Ordinal) >= 0)
				{
					dataRow["Column"] = text.Substring(text.LastIndexOf(".", StringComparison.Ordinal) + 1);
				}
				dataRow["Table"] = List[i].TableName;
				dataTable.Rows.Add(dataRow);
				dataTable.AcceptChanges();
			}
			dataTable.EndLoadData();
			using HttpClientHelper<DataTable> http = new HttpClientHelper<DataTable>(Configuration.ApiServer, "CFL");
			return await http.PostAsync("GetInfoColum", new Tuple<DataTable, string>(dataTable, GlobalData.UserCodeLogin));
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return new DataTable();
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.Commons.Queries.frm_SetParamater));
		this.btn_Ok = new Apzon.Controls.Controls.SimpleButtonEx();
		this.paneParamater = new DevExpress.XtraEditors.XtraScrollableControl();
		this.simpleButton1 = new Apzon.Controls.Controls.SimpleButtonEx();
		base.SuspendLayout();
		this.btn_Ok.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right;
		this.btn_Ok.EditControl = null;
		this.btn_Ok.EditValue = "Ok";
		this.btn_Ok.FieldName = null;
		this.btn_Ok.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("btn_Ok.ImageOptions.Image");
		this.btn_Ok.IsSearchControl = false;
		this.btn_Ok.Location = new System.Drawing.Point(283, 136);
		this.btn_Ok.Name = "btn_Ok";
		this.btn_Ok.ReadOnly = false;
		this.btn_Ok.SettingInVisible = false;
		this.btn_Ok.SettingReadonly = false;
		this.btn_Ok.Size = new System.Drawing.Size(71, 23);
		this.btn_Ok.TabIndex = 2;
		this.btn_Ok.Text = "Ok";
		this.btn_Ok.Click += new System.EventHandler(this.btn_Ok_Click);
		this.paneParamater.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
		this.paneParamater.Location = new System.Drawing.Point(0, 2);
		this.paneParamater.Name = "paneParamater";
		this.paneParamater.Size = new System.Drawing.Size(364, 125);
		this.paneParamater.TabIndex = 3;
		this.simpleButton1.EditControl = null;
		this.simpleButton1.EditValue = "simpleButton1";
		this.simpleButton1.FieldName = null;
		this.simpleButton1.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("simpleButton1.ImageOptions.Image");
		this.simpleButton1.IsSearchControl = false;
		this.simpleButton1.Location = new System.Drawing.Point(219, 140);
		this.simpleButton1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
		this.simpleButton1.Name = "simpleButton1";
		this.simpleButton1.ReadOnly = false;
		this.simpleButton1.SettingInVisible = false;
		this.simpleButton1.SettingReadonly = false;
		this.simpleButton1.Size = new System.Drawing.Size(33, 19);
		this.simpleButton1.TabIndex = 4;
		this.simpleButton1.Text = "simpleButton1";
		this.simpleButton1.Visible = false;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		this.AutoScroll = true;
		base.ClientSize = new System.Drawing.Size(366, 165);
		base.Controls.Add(this.simpleButton1);
		base.Controls.Add(this.paneParamater);
		base.Controls.Add(this.btn_Ok);
		base.IconOptions.ShowIcon = false;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frm_SetParamater";
		base.ShowInTaskbar = false;
		this.Text = "Set paramater";
		base.Load += new System.EventHandler(this.frm_SetParamater_Load);
		base.ResumeLayout(false);
	}
}
