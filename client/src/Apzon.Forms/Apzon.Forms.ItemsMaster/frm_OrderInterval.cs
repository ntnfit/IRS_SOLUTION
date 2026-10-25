using System;
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
using Apzon.Entities.Models.MasterData;
using Apzon.Forms.Commons;
using Apzon.HttpClientHelper;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.ItemsMaster;

public class frm_OrderInterval : XtraFormEx
{
	private int _Code;

	private IContainer components;

	private LabelLine lb_CycleCode;

	private TextEditEx txt_CycleCode;

	private LabelLine lb_Recurrence;

	private ComboBoxEditEx cb_Frequency;

	private ComboBoxEditEx cb_Day;

	private SimpleButtonEx bt_Add;

	private SimpleButtonEx bt_Cancel;

	private TextEditEx txt_Day;

	protected override void SetFormMode(Enums.FormViewModeType value)
	{
		if (FormAuth != Enums.FormAuthorities.Full && FormAuth != Enums.FormAuthorities.Edit)
		{
			WinFuntions.DisableAllControl(Controls);
			bt_Cancel.Enabled = true;
			return;
		}
		_formMode = value;
		switch (value)
		{
		case Enums.FormViewModeType.Add:
			bt_Add.Text = Resources.ButtonAdd;
			break;
		case Enums.FormViewModeType.Update:
			bt_Add.Text = Resources.ButtonUpdate;
			break;
		case Enums.FormViewModeType.View:
			bt_Add.Text = Resources.ButtonOK;
			break;
		case Enums.FormViewModeType.Find:
			bt_Add.Text = Resources.ButtonOK;
			break;
		}
	}

	public frm_OrderInterval()
	{
		InitializeComponent();
	}

	private async void frm_OrderInterval_Load(object sender, EventArgs e)
	{
		RefreshData();
		await GetMaxCode();
		await LoadCb();
	}

	private void RefreshData()
	{
		cb_Frequency.SelectedIndex = 0;
		txt_Day.Visible = false;
	}

	private async Task LoadCb()
	{
		try
		{
			await GetCb_Day();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task GetCb_Day()
	{
		try
		{
			if (cb_Frequency.SelectedItem == "Weekly" && cb_Frequency.SelectedIndex == 0)
			{
				cb_Day.Visible = true;
				cb_Day.Location = new Point(250, 45);
				cb_Day.Properties.Items.Clear();
				cb_Day.Properties.Items.Add("On Sunday");
				cb_Day.Properties.Items.Add("On Monday");
				cb_Day.Properties.Items.Add("On Tuesday");
				cb_Day.Properties.Items.Add("On Wednesday");
				cb_Day.Properties.Items.Add("On Thursday");
				cb_Day.Properties.Items.Add("On Friday");
				cb_Day.Properties.Items.Add("On Saturday");
				txt_Day.Visible = false;
			}
			if (cb_Frequency.SelectedItem == "Monthly" && cb_Frequency.SelectedIndex == 1)
			{
				cb_Day.Visible = true;
				cb_Day.Location = new Point(250, 45);
				cb_Day.Properties.Items.Clear();
				cb_Day.Properties.Items.Add("On the1");
				cb_Day.Properties.Items.Add("On the2");
				cb_Day.Properties.Items.Add("On the3");
				cb_Day.Properties.Items.Add("On the4");
				cb_Day.Properties.Items.Add("On the5");
				cb_Day.Properties.Items.Add("On the6");
				cb_Day.Properties.Items.Add("On the7");
				cb_Day.Properties.Items.Add("On the8");
				cb_Day.Properties.Items.Add("On the9");
				cb_Day.Properties.Items.Add("On the10");
				cb_Day.Properties.Items.Add("On the11");
				cb_Day.Properties.Items.Add("On the12");
				cb_Day.Properties.Items.Add("On the13");
				cb_Day.Properties.Items.Add("On the14");
				cb_Day.Properties.Items.Add("On the15");
				cb_Day.Properties.Items.Add("On the16");
				cb_Day.Properties.Items.Add("On the17");
				cb_Day.Properties.Items.Add("On the18");
				cb_Day.Properties.Items.Add("On the19");
				cb_Day.Properties.Items.Add("On the20");
				cb_Day.Properties.Items.Add("On the21");
				cb_Day.Properties.Items.Add("On the22");
				cb_Day.Properties.Items.Add("On the23");
				cb_Day.Properties.Items.Add("On the24");
				cb_Day.Properties.Items.Add("On the25");
				cb_Day.Properties.Items.Add("On the26");
				cb_Day.Properties.Items.Add("On the27");
				cb_Day.Properties.Items.Add("On the28");
				cb_Day.Properties.Items.Add("On the29");
				cb_Day.Properties.Items.Add("On the30");
				cb_Day.Properties.Items.Add("On the31");
				txt_Day.Visible = false;
			}
			if (cb_Frequency.SelectedIndex == 2)
			{
				cb_Day.Properties.Items.Clear();
				cb_Day.Visible = false;
				txt_Day.Visible = true;
				txt_Day.Location = new Point(250, 45);
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async void cb_Frequency_SelectedIndexChanged(object sender, EventArgs e)
	{
		try
		{
			await GetCb_Day();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private void bt_Cancel_Click(object sender, EventArgs e)
	{
		Close();
	}

	private async void bt_Add_Click(object sender, EventArgs e)
	{
		_ = 1;
		try
		{
			if (FormMode == Enums.FormViewModeType.Add || FormMode == Enums.FormViewModeType.Update)
			{
				await Save();
			}
			if (FormMode == Enums.FormViewModeType.Find)
			{
				await Search();
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	private async Task<string> Save()
	{
		string ret = string.Empty;
		try
		{
			OCYC_Impl oCYC_Impl = new OCYC_Impl();
			oCYC_Impl.Code = _Code;
			oCYC_Impl.Name = txt_CycleCode.Text;
			if (cb_Frequency.SelectedItem != null || cb_Frequency.SelectedItem != "" || cb_Frequency.SelectedItem != "-1")
			{
				if (cb_Frequency.SelectedIndex == 2)
				{
					oCYC_Impl.Frequency = "8";
				}
				else
				{
					string text = cb_Frequency.SelectedItem.ToString();
					if (!(text == "Weekly"))
					{
						if (text == "Monthly")
						{
							text = "2";
						}
					}
					else
					{
						text = "1";
					}
					oCYC_Impl.Frequency = text;
				}
			}
			if (cb_Frequency.SelectedIndex != 2)
			{
				if (cb_Day.SelectedItem != null || cb_Day.SelectedItem != "" || cb_Day.SelectedItem != "-1")
				{
					string text2 = cb_Day.SelectedItem.ToString();
					switch (text2)
					{
					case "On Sunday":
						text2 = "1";
						break;
					case "On Monday":
						text2 = "2";
						break;
					case "On Tuesday":
						text2 = "3";
						break;
					case "On Wednesday":
						text2 = "4";
						break;
					case "On Thursday":
						text2 = "5";
						break;
					case "On Friday":
						text2 = "6";
						break;
					case "On Saturday":
						text2 = "7";
						break;
					case "On the1":
						text2 = "1";
						break;
					case "On the2":
						text2 = "2";
						break;
					case "On the3":
						text2 = "3";
						break;
					case "On the4":
						text2 = "4";
						break;
					case "On the5":
						text2 = "5";
						break;
					case "On the6":
						text2 = "6";
						break;
					case "On the7":
						text2 = "7";
						break;
					case "On the8":
						text2 = "8";
						break;
					case "On the9":
						text2 = "9";
						break;
					case "On the10":
						text2 = "10";
						break;
					case "On the11":
						text2 = "11";
						break;
					case "On the12":
						text2 = "12";
						break;
					case "On the13":
						text2 = "13";
						break;
					case "On the14":
						text2 = "14";
						break;
					case "On the15":
						text2 = "15";
						break;
					case "On the16":
						text2 = "16";
						break;
					case "On the17":
						text2 = "17";
						break;
					case "On the18":
						text2 = "18";
						break;
					case "On the19":
						text2 = "19";
						break;
					case "On the20":
						text2 = "20";
						break;
					case "On the21":
						text2 = "21";
						break;
					case "On the22":
						text2 = "22";
						break;
					case "On the23":
						text2 = "23";
						break;
					case "On the24":
						text2 = "24";
						break;
					case "On the25":
						text2 = "25";
						break;
					case "On the26":
						text2 = "26";
						break;
					case "On the27":
						text2 = "27";
						break;
					case "On the28":
						text2 = "28";
						break;
					case "On the29":
						text2 = "29";
						break;
					case "On the30":
						text2 = "30";
						break;
					case "On the31":
						text2 = "31";
						break;
					}
					oCYC_Impl.Day = Function.ParseInt(text2);
				}
				else
				{
					oCYC_Impl.Day = Function.ParseInt(txt_Day.Text);
				}
				oCYC_Impl.Type = "M";
			}
			using HttpClientHelper<OCYC_Impl> _help = new HttpClientHelper<OCYC_Impl>(Configuration.ApiServer, "OCYC");
			ret = await _help.PostAsJsonAsync("Save", oCYC_Impl);
			if (string.IsNullOrWhiteSpace(ret))
			{
				WinCommons.Success(Enums.MessageCode.SaveSuccess, "");
			}
			else
			{
				WinCommons.Error(Enums.MessageCode.SaveError, "");
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
		return ret;
	}

	private async Task Search()
	{
	}

	private async Task GetMaxCode()
	{
		try
		{
			using HttpClientHelper<DataTable> _hlep = new HttpClientHelper<DataTable>(Configuration.ApiServer, "OCYC");
			WinCommons._OpenProcessing();
			_Code = await _hlep.GetIntAsync("GetMaxCode") + 1;
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
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
		System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Apzon.Forms.ItemsMaster.frm_OrderInterval));
		this.txt_CycleCode = new Apzon.Controls.Editor.TextEditEx();
		this.cb_Frequency = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.cb_Day = new Apzon.Controls.Controls.ComboBoxEditEx();
		this.bt_Add = new Apzon.Controls.Controls.SimpleButtonEx();
		this.bt_Cancel = new Apzon.Controls.Controls.SimpleButtonEx();
		this.lb_Recurrence = new Apzon.Controls.Controls.LabelLine();
		this.lb_CycleCode = new Apzon.Controls.Controls.LabelLine();
		this.txt_Day = new Apzon.Controls.Editor.TextEditEx();
		((System.ComponentModel.ISupportInitialize)this.txt_CycleCode.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Frequency.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Day.Properties).BeginInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Day.Properties).BeginInit();
		base.SuspendLayout();
		this.txt_CycleCode.FieldName = null;
		this.txt_CycleCode.IsSearchControl = false;
		this.txt_CycleCode.Location = new System.Drawing.Point(144, 9);
		this.txt_CycleCode.Name = "txt_CycleCode";
		this.txt_CycleCode.Size = new System.Drawing.Size(199, 20);
		this.txt_CycleCode.TabIndex = 1;
		this.cb_Frequency.EditValue = "Weekly";
		this.cb_Frequency.FieldName = null;
		this.cb_Frequency.IsSearchControl = false;
		this.cb_Frequency.Location = new System.Drawing.Point(144, 45);
		this.cb_Frequency.Name = "cb_Frequency";
		this.cb_Frequency.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_Frequency.Properties.Items.AddRange(new object[3] { "Weekly", "Monthly", " Every \"X\" Days: " });
		this.cb_Frequency.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cb_Frequency.Size = new System.Drawing.Size(100, 20);
		this.cb_Frequency.TabIndex = 2;
		this.cb_Frequency.SelectedIndexChanged += new System.EventHandler(this.cb_Frequency_SelectedIndexChanged);
		this.cb_Day.FieldName = null;
		this.cb_Day.IsSearchControl = false;
		this.cb_Day.Location = new System.Drawing.Point(250, 45);
		this.cb_Day.Name = "cb_Day";
		this.cb_Day.Properties.Buttons.AddRange(new DevExpress.XtraEditors.Controls.EditorButton[1]
		{
			new DevExpress.XtraEditors.Controls.EditorButton(DevExpress.XtraEditors.Controls.ButtonPredefines.Combo)
		});
		this.cb_Day.Properties.TextEditStyle = DevExpress.XtraEditors.Controls.TextEditStyles.DisableTextEditor;
		this.cb_Day.Size = new System.Drawing.Size(93, 20);
		this.cb_Day.TabIndex = 3;
		this.bt_Add.ButtonKind = Apzon.Commons.Enums.Enums.SimpleButtonType.Add;
		this.bt_Add.FieldName = null;
		this.bt_Add.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Add.ImageOptions.Image");
		this.bt_Add.IsSearchControl = false;
		this.bt_Add.Location = new System.Drawing.Point(3, 240);
		this.bt_Add.Name = "bt_Add";
		this.bt_Add.Size = new System.Drawing.Size(75, 23);
		this.bt_Add.TabIndex = 4;
		this.bt_Add.Text = "Add";
		this.bt_Add.Click += new System.EventHandler(this.bt_Add_Click);
		this.bt_Cancel.FieldName = null;
		this.bt_Cancel.ImageOptions.Image = (System.Drawing.Image)resources.GetObject("bt_Cancel.ImageOptions.Image");
		this.bt_Cancel.IsSearchControl = false;
		this.bt_Cancel.Location = new System.Drawing.Point(81, 240);
		this.bt_Cancel.Name = "bt_Cancel";
		this.bt_Cancel.Size = new System.Drawing.Size(75, 23);
		this.bt_Cancel.TabIndex = 5;
		this.bt_Cancel.Text = "Cancel";
		this.bt_Cancel.Click += new System.EventHandler(this.bt_Cancel_Click);
		this.lb_Recurrence.FieldName = null;
		this.lb_Recurrence.HideWhenEmpty = false;
		this.lb_Recurrence.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_Recurrence.Location = new System.Drawing.Point(26, 48);
		this.lb_Recurrence.Name = "lb_Recurrence";
		this.lb_Recurrence.ShowUnderLine = true;
		this.lb_Recurrence.Size = new System.Drawing.Size(121, 20);
		this.lb_Recurrence.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_Recurrence.TabIndex = 0;
		this.lb_Recurrence.Utext = "Recurrence";
		this.lb_CycleCode.FieldName = null;
		this.lb_CycleCode.HideWhenEmpty = false;
		this.lb_CycleCode.LineColor = System.Drawing.Color.FromArgb(213, 218, 223);
		this.lb_CycleCode.Location = new System.Drawing.Point(26, 12);
		this.lb_CycleCode.Name = "lb_CycleCode";
		this.lb_CycleCode.ShowUnderLine = true;
		this.lb_CycleCode.Size = new System.Drawing.Size(121, 20);
		this.lb_CycleCode.SourceType = Apzon.Commons.Enums.Enums.DataSourceType.TittleValue;
		this.lb_CycleCode.TabIndex = 0;
		this.lb_CycleCode.Utext = "Cycle Code";
		this.txt_Day.FieldName = null;
		this.txt_Day.IsSearchControl = false;
		this.txt_Day.Location = new System.Drawing.Point(250, 71);
		this.txt_Day.Name = "txt_Day";
		this.txt_Day.Size = new System.Drawing.Size(93, 20);
		this.txt_Day.TabIndex = 6;
		base.AutoScaleDimensions = new System.Drawing.SizeF(6f, 13f);
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
		base.ClientSize = new System.Drawing.Size(415, 265);
		base.Controls.Add(this.txt_Day);
		base.Controls.Add(this.bt_Cancel);
		base.Controls.Add(this.bt_Add);
		base.Controls.Add(this.cb_Day);
		base.Controls.Add(this.cb_Frequency);
		base.Controls.Add(this.lb_Recurrence);
		base.Controls.Add(this.txt_CycleCode);
		base.Controls.Add(this.lb_CycleCode);
		base.IconOptions.ShowIcon = false;
		base.Name = "frm_OrderInterval";
		this.Text = "Order Interval - Setup";
		base.Load += new System.EventHandler(this.frm_OrderInterval_Load);
		((System.ComponentModel.ISupportInitialize)this.txt_CycleCode.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Frequency.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.cb_Day.Properties).EndInit();
		((System.ComponentModel.ISupportInitialize)this.txt_Day.Properties).EndInit();
		base.ResumeLayout(false);
	}
}
