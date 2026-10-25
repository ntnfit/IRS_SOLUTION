using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls;
using Apzon.Controls.Editor;
using Apzon.Entities.Models.Common;
using Apzon.Forms.Commons;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Properties;
using Apzon.Global;
using Apzon.HttpClientHelper;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Shared;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;

namespace Apzon.Forms.Reports;

public class frm_SelectionCriteria : XtraForm
{
	public ParameterFields lstPara = new ParameterFields();

	public List<Common.ParameterValue> lstParamValue = new List<Common.ParameterValue>();

	public IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public bool isUserEdit;

	public string pathFile;

	private ReportDocument _rp;

	private IContainer components;

	public frm_SelectionCriteria()
	{
		InitializeComponent();
	}

	private async void frm_SelecttionCriteria_Load(object sender, EventArgs e)
	{
		try
		{
			if (!string.IsNullOrEmpty(pathFile))
			{
				_rp = new ReportDocument();
				_rp.Load(pathFile);
				_rp.Refresh();
				lstPara = _rp.ParameterFields;
			}
			if (lstPara.Count <= 0)
			{
				return;
			}
			int tabOrder = 0;
			int labelX = 20;
			int labelY = 20;
			int controlX = 150;
			int controlY = 20;
			foreach (ParameterField para in lstPara)
			{
				if (!string.IsNullOrEmpty(para.ReportName))
				{
					continue;
				}
				string text = para.Name.Split(' ')[0];
				string text2 = "";
				if (para.Name.ToLower().Equals("schema@"))
				{
					TextEdit textEdit = new TextEdit();
					textEdit.Location = new Point(controlX, controlY);
					textEdit.Name = text;
					textEdit.Tag = para.Name;
					textEdit.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
					textEdit.Text = GlobalData.DatabaseName;
					textEdit.Visible = false;
					Controls.Add(textEdit);
					continue;
				}
				if (text.ToLower().Equals("printbyusername@"))
				{
					TextEdit textEdit2 = new TextEdit();
					textEdit2.Location = new Point(controlX, controlY);
					textEdit2.Name = text;
					textEdit2.Tag = para.Name;
					textEdit2.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
					textEdit2.Text = GlobalData.UserNameLogin;
					textEdit2.Visible = false;
					Controls.Add(textEdit2);
					continue;
				}
				if (text.ToLower().Equals("usercode@"))
				{
					TextEdit textEdit3 = new TextEdit();
					textEdit3.Location = new Point(controlX, controlY);
					textEdit3.Name = text;
					textEdit3.Tag = para.Name;
					textEdit3.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
					textEdit3.Text = GlobalData.UserName;
					textEdit3.Visible = false;
					Controls.Add(textEdit3);
					continue;
				}
				if (text.Contains("@"))
				{
					text = text.Split('@')[0];
					text2 = para.Name.Substring(text.Length + 1, para.Name.Length - (text.Length + 1));
				}
				string promptText = para.PromptText;
				ParameterValueKind parameterValueType = para.ParameterValueType;
				bool flag = true;
				Function.ToString(para.MinimumValue);
				Function.ToString(para.MaximumValue);
				LabelControl labelControl = new LabelControl();
				labelControl.Name = "lbl_" + text;
				labelControl.Text = promptText;
				labelControl.Location = new Point(labelX, labelY);
				Controls.Add(labelControl);
				if (!string.IsNullOrEmpty(text2))
				{
					if (flag)
					{
						ButtonEdit bte = new ButtonEdit();
						bte.Name = text;
						bte.ToolTip = text2;
						bte.Tag = para.Name;
						bte.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
						bte.TabIndex = tabOrder;
						if (para.DefaultValues.Count > 0)
						{
							new ParameterDiscreteValue();
							ParameterDiscreteValue parameterDiscreteValue = (ParameterDiscreteValue)para.DefaultValues[0];
							bte.EditValue = parameterDiscreteValue.Value;
						}
						tabOrder++;
						bte.Properties.Buttons.Clear();
						bte.Properties.Buttons.AddRange(new EditorButton[1]
						{
							new EditorButton(ButtonPredefines.Search)
						});
						bte.Properties.ButtonClick += (object s, ButtonPressedEventArgs v) =>
						{
							try
							{
								string text3 = bte.ToolTip;
								Regex.Replace(text3, "\\s+", " ");
								foreach (Control control in Controls)
								{
									if (control is ButtonEdit || control is LookUpEdit)
									{
										if (control is ComboBoxEdit comboBoxEdit2)
										{
											string text4 = Function.ToString((comboBoxEdit2.SelectedItem as Common.ListItems).Value);
											text3 = text3.Replace("@" + comboBoxEdit2.Name + " ", text4 + " ");
											text3 = text3.Replace("'@" + comboBoxEdit2.Name + "'", "'" + text4 + "'");
											text3 = text3.Replace("@" + comboBoxEdit2.Name + "=", text4 + "=");
										}
										else
										{
											dynamic val = control;
											text3 = text3.Replace($"@{(object)val.Name} ", val.EditValue + " ");
											text3 = text3.Replace($"'@{(object)val.Name}'", $"'{(object)val.EditValue}'");
											text3 = text3.Replace($"@{(object)val.Name}=", $"{(object)val.EditValue}=");
										}
									}
									else if (!(control is LabelControl))
									{
										dynamic val2 = control;
										text3 = text3.Replace($"@{(object)val2.Name} ", val2.Text + " ");
										text3 = text3.Replace($"'@{(object)val2.Name}'", $"'{(object)val2.Text}'");
										text3 = text3.Replace($"@{(object)val2.Name}=", $"{(object)val2.Text}=");
									}
								}
								frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedQuery, para.EnableAllowMultipleValue, bte.Text, StringToQuery(text3), Enums.ItemObjectType.NoItemType, null, "");
								frm_CFL2.ShowDialog();
								string text5 = "";
								if (!para.EnableAllowMultipleValue)
								{
									text5 = Function.ToString(frm_CFL2.ChooseItem?[0]);
								}
								else
								{
									List<DataRow> chooseItems = frm_CFL2.ChooseItems;
									if (chooseItems != null && chooseItems.Count > 0)
									{
										text5 = string.Join(",", chooseItems.Select((DataRow r) => Function.ToString(r[0])));
									}
								}
								bte.Text = text5;
							}
							catch (Exception)
							{
							}
						};
						bte.Properties.KeyDown += (object s, KeyEventArgs k) =>
						{
							try
							{
								if (k.KeyCode == Keys.Tab)
								{
									frm_CFL frm_CFL2 = new frm_CFL(Enums.TypeCFL.UserDefinedQuery, para.EnableAllowMultipleValue, bte.Text, StringToQuery(bte.ToolTip), Enums.ItemObjectType.NoItemType, null, "");
									frm_CFL2.ShowDialog();
									string text3 = "";
									if (!para.EnableAllowMultipleValue)
									{
										text3 = Function.ToString(frm_CFL2.ChooseItem?[0]);
									}
									else
									{
										List<DataRow> chooseItems = frm_CFL2.ChooseItems;
										if (chooseItems != null && chooseItems.Count > 0)
										{
											text3 = string.Join(",", chooseItems.Select((DataRow r) => Function.ToString(r[0])));
										}
									}
									bte.Text = text3;
								}
							}
							catch (Exception)
							{
							}
						};
						bte.Location = new Point(controlX, controlY);
						Controls.Add(bte);
					}
					else
					{
						LookUpEdit lke = new LookUpEdit();
						lke.Properties.NullText = "";
						lke.Name = text;
						lke.ToolTip = text2;
						lke.Tag = para.Name;
						lke.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
						lke.TabIndex = tabOrder;
						if (para.DefaultValues.Count > 0)
						{
							new ParameterDiscreteValue();
							ParameterDiscreteValue parameterDiscreteValue2 = (ParameterDiscreteValue)para.DefaultValues[0];
							lke.EditValue = parameterDiscreteValue2.Value;
						}
						tabOrder++;
						lke.Properties.Buttons.Clear();
						lke.Properties.Buttons.AddRange(new EditorButton[1]
						{
							new EditorButton(ButtonPredefines.Combo)
						});
						lke.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
						DataSet dataSet = await _httpClient.Cfl.GetDataByQuery(StringToQuery(text2));
						if (dataSet.IsNotNull())
						{
							lke.Properties.DataSource = dataSet;
							lke.Properties.ValueMember = dataSet.Tables[0].Columns[0].ColumnName;
							lke.Properties.DisplayMember = dataSet.Tables[0].Columns[0].ColumnName;
							if (dataSet.Tables[0].Columns.Count > 1)
							{
								lke.Properties.DisplayMember = dataSet.Tables[0].Columns[1].ColumnName;
							}
						}
						dataSet.DisposeDataSet();
						lke.Location = new Point(controlX, controlY);
						Controls.Add(lke);
					}
				}
				else if (para.DefaultValues.Count > 1 && _rp != null)
				{
					ComboBoxEdit comboBoxEdit = new ComboBoxEdit();
					comboBoxEdit.Name = text;
					comboBoxEdit.Tag = para.Name;
					comboBoxEdit.ToolTip = text;
					comboBoxEdit.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
					comboBoxEdit.TabIndex = tabOrder;
					comboBoxEdit.Location = new Point(controlX, controlY);
					comboBoxEdit.Properties.TextEditStyle = TextEditStyles.DisableTextEditor;
					List<Common.Table_ID> list = new List<Common.Table_ID>();
					foreach (ParameterValue defaultValue in _rp.DataDefinition.ParameterFields[text].DefaultValues)
					{
						if (!defaultValue.IsRange)
						{
							ParameterDiscreteValue parameterDiscreteValue3 = (ParameterDiscreteValue)defaultValue;
							Common.Table_ID table_ID = new Common.Table_ID();
							table_ID.ID = parameterDiscreteValue3.Value;
							table_ID.Name = (string.IsNullOrWhiteSpace(parameterDiscreteValue3.Description) ? Function.ToString(parameterDiscreteValue3.Value) : parameterDiscreteValue3.Description);
							list.Add(table_ID);
						}
					}
					WinFuntions._addComboBox(comboBoxEdit, list, _selectFirstItem: true);
					Controls.Add(comboBoxEdit);
				}
				else
				{
					switch (parameterValueType)
					{
					case ParameterValueKind.NumberParameter:
					case ParameterValueKind.CurrencyParameter:
					{
						CalcEdit calcEdit = new CalcEdit();
						calcEdit.Name = text;
						calcEdit.Tag = para.Name;
						calcEdit.ToolTip = text;
						calcEdit.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
						calcEdit.TabIndex = tabOrder;
						if (para.DefaultValues.Count > 0)
						{
							new ParameterDiscreteValue();
							ParameterDiscreteValue parameterDiscreteValue7 = (ParameterDiscreteValue)para.DefaultValues[0];
							calcEdit.EditValue = parameterDiscreteValue7.Value;
						}
						tabOrder++;
						calcEdit.Location = new Point(controlX, controlY);
						calcEdit.Properties.AllowMouseWheel = false;
						calcEdit.Properties.Buttons.Clear();
						Controls.Add(calcEdit);
						break;
					}
					case ParameterValueKind.BooleanParameter:
					{
						CheckEdit checkEdit = new CheckEdit();
						checkEdit.Location = new Point(controlX, controlY);
						checkEdit.Name = text;
						checkEdit.Tag = para.Name;
						checkEdit.Text = "";
						checkEdit.TabIndex = tabOrder;
						tabOrder++;
						Controls.Add(checkEdit);
						break;
					}
					case ParameterValueKind.DateParameter:
					{
						DateEditEx dateEditEx = new DateEditEx();
						dateEditEx.Location = new Point(controlX, controlY);
						dateEditEx.Name = text;
						dateEditEx.Tag = text;
						dateEditEx.ToolTip = text;
						dateEditEx.TabIndex = tabOrder;
						tabOrder++;
						if (para.DefaultValues.Count > 0)
						{
							new ParameterDiscreteValue();
							ParameterDiscreteValue parameterDiscreteValue5 = (ParameterDiscreteValue)para.DefaultValues[0];
							dateEditEx.EditValue = parameterDiscreteValue5.Value;
						}
						else
						{
							dateEditEx.EditValue = null;
						}
						Controls.Add(dateEditEx);
						break;
					}
					case ParameterValueKind.StringParameter:
					{
						TextEdit textEdit4 = new TextEdit();
						textEdit4.Location = new Point(controlX, controlY);
						textEdit4.Name = text;
						textEdit4.Tag = para.Name;
						textEdit4.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
						textEdit4.Text = "";
						textEdit4.TabIndex = tabOrder;
						if (para.DefaultValues.Count > 0)
						{
							new ParameterDiscreteValue();
							ParameterDiscreteValue parameterDiscreteValue6 = (ParameterDiscreteValue)para.DefaultValues[0];
							textEdit4.EditValue = parameterDiscreteValue6.Value;
						}
						tabOrder++;
						Controls.Add(textEdit4);
						break;
					}
					case ParameterValueKind.DateTimeParameter:
					{
						DateTimePicker dateTimePicker = new DateTimePicker();
						dateTimePicker.Location = new Point(controlX, controlY);
						dateTimePicker.Name = text;
						dateTimePicker.Tag = para.Name;
						dateTimePicker.TabIndex = tabOrder;
						tabOrder++;
						Controls.Add(dateTimePicker);
						break;
					}
					case ParameterValueKind.TimeParameter:
					{
						DateEdit dateEdit = new DateEdit();
						dateEdit.Location = new Point(controlX, controlY);
						dateEdit.Name = text;
						dateEdit.Tag = text;
						dateEdit.ToolTip = text;
						dateEdit.TabIndex = tabOrder;
						tabOrder++;
						dateEdit.Properties.DisplayFormat.FormatString = "HH:mm";
						dateEdit.Properties.DisplayFormat.FormatType = FormatType.DateTime;
						dateEdit.Properties.EditFormat.FormatString = "HH:mm";
						dateEdit.Properties.EditFormat.FormatType = FormatType.DateTime;
						dateEdit.Properties.EditMask = "HH:mm";
						if (para.DefaultValues.Count > 0)
						{
							new ParameterDiscreteValue();
							ParameterDiscreteValue parameterDiscreteValue4 = (ParameterDiscreteValue)para.DefaultValues[0];
							dateEdit.EditValue = parameterDiscreteValue4.Value;
						}
						Controls.Add(dateEdit);
						break;
					}
					}
				}
				labelY += 25;
				controlY += 25;
			}
			SimpleButton simpleButton = new SimpleButton();
			simpleButton.Image = Resources.apply_16x16;
			simpleButton.Name = "btn_Ok";
			simpleButton.Text = "OK";
			simpleButton.Location = new Point(labelX, labelY);
			simpleButton.Click += btn_Ok_Click;
			AcceptButton = simpleButton;
			SimpleButton simpleButton2 = new SimpleButton();
			simpleButton2.Image = Resources.cancel_16x16;
			simpleButton2.Name = "btn_Cancel";
			simpleButton2.Text = "Cancel";
			simpleButton2.Location = new Point(controlX, controlY);
			simpleButton2.Click += btnCancel_Click;
			CancelButton = simpleButton2;
			controlY += 25;
			Controls.Add(simpleButton);
			Controls.Add(simpleButton2);
			Size = new Size(300, 40 + controlY);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private async void btn_Ok_Click(object sender, EventArgs e)
	{
		try
		{
			WinCommons._OpenProcessing();
			lstParamValue.Clear();
			if (!string.IsNullOrEmpty(pathFile))
			{
				_rp = new ReportDocument();
				_rp.Load(pathFile);
				_rp.Refresh();
			}
			foreach (Control control in Controls)
			{
				Type type = control.GetType();
				if (type.Name.Equals("LabelControl"))
				{
					continue;
				}
				switch (type.Name)
				{
				case "CalcEdit":
				{
					CalcEdit calcEdit = (CalcEdit)control;
					Common.ParameterValue parameterValue2 = new Common.ParameterValue();
					parameterValue2.Value = Function.ToString(calcEdit.EditValue);
					parameterValue2.Name = calcEdit.Tag.ToString();
					lstParamValue.Add(parameterValue2);
					break;
				}
				case "CheckEdit":
				{
					CheckEdit checkEdit = (CheckEdit)control;
					Common.ParameterValue parameterValue8 = new Common.ParameterValue();
					parameterValue8.Value = Function.ParseBool(checkEdit.Checked);
					parameterValue8.Name = checkEdit.Tag.ToString();
					lstParamValue.Add(parameterValue8);
					break;
				}
				case "DateEditEx":
				{
					DateEditEx dateEditEx = (DateEditEx)control;
					Common.ParameterValue parameterValue7 = new Common.ParameterValue();
					parameterValue7.Value = Function.ParseDateTime(dateEditEx.EditValue);
					parameterValue7.Name = dateEditEx.Tag.ToString();
					lstParamValue.Add(parameterValue7);
					break;
				}
				case "DateTimePicker":
				{
					DateEdit dateEdit = (DateEdit)control;
					Common.ParameterValue parameterValue6 = new Common.ParameterValue();
					parameterValue6.Value = Function.ParseDateTime(dateEdit.EditValue);
					parameterValue6.Name = dateEdit.Tag.ToString();
					lstParamValue.Add(parameterValue6);
					break;
				}
				case "LookUpEdit":
				{
					LookUpEdit lookUpEdit = (LookUpEdit)control;
					Common.ParameterValue parameterValue5 = new Common.ParameterValue();
					parameterValue5.Value = Function.ToString(lookUpEdit.EditValue);
					parameterValue5.Name = lookUpEdit.Tag.ToString();
					lstParamValue.Add(parameterValue5);
					break;
				}
				case "ButtonEdit":
				{
					ButtonEdit buttonEdit = (ButtonEdit)control;
					Common.ParameterValue parameterValue4 = new Common.ParameterValue();
					parameterValue4.Value = Function.ToString(buttonEdit.EditValue);
					parameterValue4.Name = buttonEdit.Tag.ToString();
					lstParamValue.Add(parameterValue4);
					break;
				}
				case "TextEdit":
				{
					TextEdit textEdit = (TextEdit)control;
					Common.ParameterValue parameterValue3 = new Common.ParameterValue();
					parameterValue3.Value = Function.ToString(textEdit.EditValue);
					parameterValue3.Name = textEdit.Tag.ToString();
					lstParamValue.Add(parameterValue3);
					break;
				}
				case "ComboBoxEdit":
				{
					ComboBoxEdit comboBoxEdit = (ComboBoxEdit)control;
					Common.ParameterValue parameterValue = new Common.ParameterValue();
					if (comboBoxEdit.SelectedItem != null)
					{
						Common.ListItems listItems = (Common.ListItems)comboBoxEdit.SelectedItem;
						parameterValue.Value = listItems.Value;
						parameterValue.Name = comboBoxEdit.Tag.ToString();
						lstParamValue.Add(parameterValue);
					}
					break;
				}
				}
			}
			if (lstParamValue.Count > 0)
			{
				isUserEdit = true;
			}
			if (_rp == null)
			{
				Close();
				return;
			}
			await WinFuntions.ReportViewerListParam(pathFile, lstParamValue);
			WinCommons._CloseProcessing();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
	}

	private string StringToQuery(object obj)
	{
		try
		{
			string text = Function.ToString(obj);
			string text2 = text.ToLower();
			if (!string.IsNullOrEmpty(text))
			{
				if (text2.StartsWith("joinfrom"))
				{
					text = "select * from " + text.Substring(8);
				}
				else if (!text2.StartsWith("select"))
				{
					string text3 = "Select ";
					if (!text2.Contains("from"))
					{
						text3 = "Select * from ";
					}
					else if (text2.StartsWith("from"))
					{
						text3 = "Select * ";
					}
					text = text3 + text;
				}
			}
			for (int num = text.ToLower().IndexOf("printbyusername@"); num > -1; num = text.ToLower().IndexOf("printbyusername@"))
			{
				text = text.Substring(0, num) + GlobalData.UserName + text.Substring(num + 16);
			}
			for (int num = text.ToLower().IndexOf("usercode@"); num > -1; num = text.ToLower().IndexOf("usercode@"))
			{
				text = text.Substring(0, num) + GlobalData.UserCodeLogin + text.Substring(num + 9);
			}
			return text;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return "";
	}

	private void btnCancel_Click(object sender, EventArgs e)
	{
		Close();
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
		base.SuspendLayout();
		base.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Inherit;
		base.ClientSize = new System.Drawing.Size(284, 278);
		base.IconOptions.ShowIcon = false;
		base.MaximizeBox = false;
		base.MinimizeBox = false;
		base.Name = "frm_SelecttionCriteria";
		base.ShowInTaskbar = false;
		base.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
		this.Text = "Selection Criteria";
		base.Load += new System.EventHandler(this.frm_SelecttionCriteria_Load);
		base.ResumeLayout(false);
	}
}
