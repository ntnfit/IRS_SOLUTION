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
using Apzon.Controls;
using Apzon.Controls.Controls;
using Apzon.Controls.Editor;
using Apzon.Controls.Form;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.UserFields;
using Apzon.Forms.Commons.CFL;
using Apzon.Forms.Commons.UDO;
using Apzon.Forms.Helper;
using Apzon.Global;
using Apzon.HttpClientHelper;
using DevExpress.Utils;
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraLayout;
using SAPbobsCOM;

namespace Apzon.Forms.UserControls;

[ToolboxItem(true)]
public class LayoutUDF : LayoutControl
{
	private readonly IHttpClient _httpClient = HttpClientHelperFactory.GetHttpClient();

	public LayoutUDF()
	{
		LayoutControlGroup root = new LayoutControlGroup
		{
			EnableIndentsWithoutBorders = DefaultBoolean.True,
			GroupBordersVisible = false,
			Location = new Point(0, 0),
			TextVisible = false
		};
		Name = "layout_UDF";
		Root = root;
		TabIndex = 2;
		base.Text = "User Defined Fields";
	}

	public async Task<int> LoadControls(string tableName, ApzObjectTypes objctType)
	{
		int count = 0;
		try
		{
			List<UserFieldsImpl> listFieldUDF = await _httpClient.Udf.GetListField(tableName);
			if (listFieldUDF == null || listFieldUDF.Count <= 0)
			{
				return count;
			}
			DataTable dtFormularField = _httpClient.Commons.GetCflFormulars(Function.ToString((int)objctType));
			for (int i = 0; i < listFieldUDF.Count; i++)
			{
				UserFieldsImpl item = listFieldUDF[i];
				string fieldName = "U_" + item.Name;
				if (Controls.Cast<Control>().Any((Control ctrl) => ctrl.Name.Equals(fieldName)))
				{
					continue;
				}
				LayoutControlItem layoutItem = AddItem();
				layoutItem.Text = item.Description;
				layoutItem.AppearanceItemCaption.Font = new Font(layoutItem.AppearanceItemCaption.Font.FontFamily, layoutItem.AppearanceItemCaption.Font.Size, FontStyle.Underline);
				layoutItem.AppearanceItemCaption.ForeColor = Color.FromArgb(174, 174, 186);
				DataRow formularRow = dtFormularField.AsEnumerable().FirstOrDefault((DataRow t) => Function.ToString(t["U_FieldName"]).Equals(fieldName));
				await AddControlToLayoutItem(FindForm() as XtraFormEx, item, layoutItem, formularRow);
				if (layoutItem.Control is BaseEdit baseEdit)
				{
					baseEdit.AccessibleDescription = item.DefaultValue;
					if (baseEdit is TextEdit)
					{
						((TextEdit)baseEdit).Properties.MaxLength = Function.ParseInt(item.EditSize);
					}
					if (baseEdit is MemoExEdit)
					{
						((MemoExEdit)baseEdit).Properties.MaxLength = Function.ParseInt(item.EditSize);
					}
					if (baseEdit is MemoEdit)
					{
						((MemoEdit)baseEdit).Properties.MaxLength = Function.ParseInt(item.EditSize);
					}
				}
			}
			count = listFieldUDF.Count;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.ErrorLineNumber(ex);
		}
		return count;
	}

	private async Task AddControlToLayoutItem(XtraFormEx baseForm, UserFieldsImpl item, LayoutControlItem layoutItem, DataRow formularRow)
	{
		try
		{
			int layoutWidth = Width - 50;
			if (formularRow != null)
			{
				IHttpClient httpClient = HttpClientHelperFactory.GetHttpClient();
				string text = Function.ToString(formularRow["U_FormularType"]).ToUpper();
				if (text == "CBX")
				{
					LookUpEditEx look = new LookUpEditEx();
					look.Name = "U_" + item.Name;
					look.EditValue = item.DefaultValue;
					look.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
					look.Size = new Size(layoutWidth, 20);
					string sql = Function.ToString(formularRow["U_Formular"]);
					DataTable data = (await httpClient.Commons.ExecuteQuery(sql)).Data;
					look.Properties.DataSource = data;
					if (data != null && data.Columns.Count >= 2)
					{
						look.Properties.ValueMember = data.Columns[0].ColumnName;
						look.Properties.DisplayMember = data.Columns[1].ColumnName;
					}
					look.Properties.NullText = string.Empty;
					look.StyleController = this;
					look.FieldName = "U_" + item.Name;
					layoutItem.Control = look;
					return;
				}
				if (text == "CFL")
				{
					ButtonEditEx btn = new ButtonEditEx();
					btn.Properties.Buttons[0].Kind = ButtonPredefines.Search;
					btn.Properties.TextEditStyle = TextEditStyles.Standard;
					btn.Visible = true;
					btn.Name = "U_" + item.Name;
					btn.EditValue = item.DefaultValue;
					btn.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
					btn.Size = new Size(layoutWidth, 20);
					btn.StyleController = this;
					btn.ToolTip = item.Description;
					btn.FieldName = "U_" + item.Name;
					layoutItem.Control = btn;
					btn.ButtonClick += (object sender, ButtonPressedEventArgs e) =>
					{
						try
						{
							frm_ChooseFromList frm_ChooseFromList2 = new frm_ChooseFromList(Enums.TypeCFL.FormattedSearch, multiSelect: false, "");
							string text2 = Function.ToString(formularRow["U_Formular"]);
							List<string> paramFromSqlString = Function.GetParamFromSqlString(text2);
							if (paramFromSqlString != null && paramFromSqlString.Any())
							{
								foreach (string item2 in paramFromSqlString)
								{
									if (!string.IsNullOrEmpty(item2))
									{
										string[] array = item2.Replace("@", "").Replace(" ", "").Split('.');
										Function.ToString(formularRow["U_DependTable"]);
										string fieldName = "";
										if (array.Length > 1)
										{
											if (string.IsNullOrEmpty(array[0]))
											{
												Function.ToString(formularRow["U_DependTable"]);
											}
											else
											{
												_ = array[0];
											}
											fieldName = array[1];
										}
										else if (array.Length != 0)
										{
											fieldName = array[0];
										}
										string text3 = Function.ToString(baseForm.GetDataFormSourceControl(fieldName));
										text2 = text2.Replace(item2, string.IsNullOrEmpty(text3) ? "NULL" : $"'{text3}'");
									}
								}
							}
							frm_ChooseFromList2.FormattedSearch = text2;
							frm_ChooseFromList2.ShowDialog(baseForm);
							dynamic chooseItem = frm_ChooseFromList2.ChooseItem;
							btn.EditValue = Function.ToString(((DataRow)chooseItem.Row)[0]);
						}
						catch (Exception ex2)
						{
							Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						}
					};
					return;
				}
			}
			if (!string.IsNullOrEmpty(item.LinkedUDO))
			{
				ButtonEditEx btn2 = new ButtonEditEx();
				btn2.Properties.Buttons[0].Kind = ButtonPredefines.Search;
				btn2.Visible = true;
				btn2.Name = "U_" + item.Name;
				btn2.EditValue = item.DefaultValue;
				btn2.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
				btn2.Size = new Size(layoutWidth, 20);
				btn2.StyleController = this;
				btn2.ToolTip = item.Description;
				btn2.Tag = item.LinkedUDO;
				btn2.FieldName = "U_" + item.Name;
				layoutItem.Control = btn2;
				string udoCode = item.LinkedUDO;
				btn2.ButtonClick += (object sender, ButtonPressedEventArgs e) =>
				{
					try
					{
						int docEntry = Function.ParseInt(btn2.EditValue);
						frm_UDO frm_UDO2 = new frm_UDO(udoCode, docEntry)
						{
							Text = item.Description
						};
						frm_UDO2.ShowDialog(this);
						if (frm_UDO2.ChoosedEntry != null)
						{
							btn2.EditValue = frm_UDO2.ChoosedEntry;
						}
					}
					catch (Exception ex2)
					{
						Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex2.Message);
						WinCommons.ErrorLineNumber(ex2);
					}
				};
				return;
			}
			if (item.ValidValues != null && item.ValidValues.Count > 0)
			{
				LookUpEditEx lookUpEditEx = new LookUpEditEx();
				lookUpEditEx.Name = "U_" + item.Name;
				lookUpEditEx.EditValue = item.DefaultValue;
				lookUpEditEx.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
				lookUpEditEx.Size = new Size(layoutWidth, 20);
				switch (item.Type)
				{
				case BoFieldTypes.db_Alpha:
					lookUpEditEx.Properties.DataSource = item.ValidValues;
					break;
				case BoFieldTypes.db_Numeric:
					lookUpEditEx.Properties.DataSource = item.ValidValues.Select((ValidValuesMDImpl r) => new
					{
						Value = Function.ParseInt(r.Value),
						Description = r.Description
					});
					break;
				case BoFieldTypes.db_Float:
					lookUpEditEx.Properties.DataSource = item.ValidValues.Select((ValidValuesMDImpl r) => new
					{
						Value = Function.ParseDecimal(r.Value),
						Description = r.Description
					});
					break;
				default:
					lookUpEditEx.Properties.DataSource = item.ValidValues;
					break;
				}
				lookUpEditEx.Properties.ValueMember = "Value";
				lookUpEditEx.Properties.DisplayMember = "Description";
				lookUpEditEx.StyleController = this;
				lookUpEditEx.FieldName = "U_" + item.Name;
				layoutItem.Control = lookUpEditEx;
				return;
			}
			switch (item.Type)
			{
			case BoFieldTypes.db_Alpha:
				if (!string.IsNullOrEmpty(item.LinkedTable))
				{
					List<Common.Table_Code> list = await _httpClient.Udf.GetLinkedTable(item.LinkedTable, GlobalData.UserName, GlobalData.DefaultBranch);
					list.Insert(0, new Common.Table_Code("", ""));
					LookUpEditEx lookUpEditEx2 = new LookUpEditEx();
					lookUpEditEx2.Name = "U_" + item.Name;
					lookUpEditEx2.EditValue = item.DefaultValue;
					lookUpEditEx2.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
					lookUpEditEx2.Size = new Size(layoutWidth, 20);
					lookUpEditEx2.Properties.DataSource = list;
					lookUpEditEx2.Properties.ValueMember = "Code";
					lookUpEditEx2.Properties.DisplayMember = "Name";
					lookUpEditEx2.Properties.BestFitMode = BestFitMode.BestFitResizePopup;
					lookUpEditEx2.StyleController = this;
					lookUpEditEx2.FieldName = "U_" + item.Name;
					layoutItem.Control = lookUpEditEx2;
				}
				else if (string.IsNullOrEmpty(item.LinkedTable))
				{
					TextEditEx textEditEx = new TextEditEx();
					textEditEx.Name = "U_" + item.Name;
					textEditEx.EditValue = item.DefaultValue;
					textEditEx.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
					textEditEx.Size = new Size(layoutWidth, 20);
					textEditEx.StyleController = this;
					textEditEx.FieldName = "U_" + item.Name;
					layoutItem.Control = textEditEx;
				}
				break;
			case BoFieldTypes.db_Date:
				if (item.SubType == BoFldSubTypes.st_Time)
				{
					HourEdit hourEdit2 = new HourEdit();
					hourEdit2.Name = "U_" + item.Name;
					hourEdit2.EditValue = item.DefaultValue;
					hourEdit2.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
					hourEdit2.Size = new Size(layoutWidth, 20);
					hourEdit2.StyleController = this;
					hourEdit2.FieldName = "U_" + item.Name;
					layoutItem.Control = hourEdit2;
				}
				else if (item.SubType != BoFldSubTypes.st_Time)
				{
					DateEditEx dateEditEx = new DateEditEx();
					dateEditEx.Name = "U_" + item.Name;
					dateEditEx.EditValue = item.DefaultValue;
					dateEditEx.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
					dateEditEx.Size = new Size(layoutWidth, 20);
					dateEditEx.FieldName = "U_" + item.Name;
					dateEditEx.StyleController = this;
					layoutItem.Control = dateEditEx;
					layoutItem.Control = dateEditEx;
				}
				break;
			case BoFieldTypes.db_Float:
			{
				CalcEditEx calcEditEx2 = new CalcEditEx();
				calcEditEx2.Name = "U_" + item.Name;
				calcEditEx2.EditValue = item.DefaultValue;
				calcEditEx2.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
				calcEditEx2.Size = new Size(layoutWidth, 20);
				calcEditEx2.Properties.DisplayFormat.FormatType = FormatType.Numeric;
				calcEditEx2.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
				calcEditEx2.StyleController = this;
				calcEditEx2.FieldName = "U_" + item.Name;
				layoutItem.Control = calcEditEx2;
				break;
			}
			case BoFieldTypes.db_Memo:
				if (item.SubType == BoFldSubTypes.st_Link)
				{
					ButtonEditAttach buttonEditAttach = new ButtonEditAttach();
					buttonEditAttach.Properties.ObjType = ((XtraFormEx)FindForm()).GetObjectType();
					buttonEditAttach.Name = "U_" + item.Name;
					buttonEditAttach.EditValue = item.DefaultValue;
					buttonEditAttach.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
					buttonEditAttach.Size = new Size(layoutWidth, 20);
					buttonEditAttach.StyleController = this;
					buttonEditAttach.FieldName = "U_" + item.Name;
					layoutItem.Control = buttonEditAttach;
				}
				else
				{
					MemoExEditEx memoExEditEx = new MemoExEditEx();
					memoExEditEx.Name = "U_" + item.Name;
					memoExEditEx.Properties.ShowIcon = false;
					memoExEditEx.EditValue = item.DefaultValue;
					memoExEditEx.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
					memoExEditEx.Size = new Size(layoutWidth, 20);
					memoExEditEx.StyleController = this;
					memoExEditEx.FieldName = "U_" + item.Name;
					layoutItem.Control = memoExEditEx;
				}
				break;
			case BoFieldTypes.db_Numeric:
				if (item.SubType == BoFldSubTypes.st_Time)
				{
					HourEdit hourEdit = new HourEdit();
					hourEdit.Name = "U_" + item.Name;
					hourEdit.EditValue = item.DefaultValue;
					hourEdit.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
					hourEdit.Size = new Size(layoutWidth, 20);
					hourEdit.StyleController = this;
					hourEdit.FieldName = "U_" + item.Name;
					layoutItem.Control = hourEdit;
				}
				else if (item.SubType != BoFldSubTypes.st_Time)
				{
					CalcEditEx calcEditEx = new CalcEditEx();
					calcEditEx.Name = "U_" + item.Name;
					calcEditEx.EditValue = item.DefaultValue;
					calcEditEx.Properties.AllowNullInput = ((item.Mandatory != BoYesNoEnum.tYES) ? DefaultBoolean.False : DefaultBoolean.True);
					calcEditEx.Size = new Size(layoutWidth, 20);
					calcEditEx.StyleController = this;
					calcEditEx.Properties.DisplayFormat.FormatType = FormatType.Numeric;
					calcEditEx.Properties.DisplayFormat.Format = GlobalData.UnitPriceFormat;
					calcEditEx.FieldName = "U_" + item.Name;
					layoutItem.Control = calcEditEx;
				}
				break;
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.ToString());
			WinCommons.ErrorLineNumber(ex);
		}
	}
}
