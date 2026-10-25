using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Controls.Interfaces;
using Apzon.Controls.Tree;
using Apzon.Global;
using DevExpress.XtraEditors.Container;

namespace Apzon.Controls.Commons;

public class ControlFunction
{
	public static string ToString(object obj)
	{
		try
		{
			if (obj == null)
			{
				return string.Empty;
			}
			return obj.ToString();
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static bool IsNumeric(object obj)
	{
		try
		{
			if (obj == null)
			{
				return false;
			}
			if (double.TryParse(obj.ToString(), out var _))
			{
				return true;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public static bool IsInt(string obj)
	{
		try
		{
			if (string.IsNullOrEmpty(obj))
			{
				return false;
			}
			char[] array = obj.ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] < '0' || array[i] > '9')
				{
					return false;
				}
			}
			return true;
		}
		catch
		{
			return false;
		}
	}

	public static bool ParseBoolean(object obj)
	{
		try
		{
			if (obj == null)
			{
				return false;
			}
			if (bool.TryParse(obj.ToString(), out var result))
			{
				return result;
			}
			return false;
		}
		catch
		{
			return false;
		}
	}

	public static string Format(object obj, string formatString)
	{
		try
		{
			if (obj == null)
			{
				return string.Empty;
			}
			if (obj is DateTime dateTime)
			{
				return dateTime.ToString(formatString, CultureInfo.InvariantCulture);
			}
			return string.Format("{0:" + formatString + "}", obj);
		}
		catch (Exception)
		{
			return string.Empty;
		}
	}

	public static bool CompareOperatorLevel(string a, string b)
	{
		if (a.Equals("("))
		{
			return b.Equals(")");
		}
		if (b.Equals("("))
		{
			return false;
		}
		if (ConstantsApzCtd.ListOperatorCopare.Contains(b))
		{
			if (!a.Equals("(") && !a.Equals("and"))
			{
				return !a.Equals("or");
			}
			return false;
		}
		if (ConstantsApzCtd.OperatorMathBoolean.Contains(b))
		{
			return true;
		}
		bool result = false;
		switch (a)
		{
		case "+":
		case "-":
			if (b.Equals("+") || b.Equals("-"))
			{
				result = true;
			}
			break;
		case "*":
		case "%":
		case "/":
			if (!b.Equals("^") && !b.Equals("abs") && !b.Equals("sqrt") && !b.Equals("trim") && !b.EndsWith("toupper") && !b.Equals("tolower"))
			{
				result = true;
			}
			break;
		case "^":
			if (!b.Equals("abs") && !b.Equals("sqrt") && !b.Equals("trim") && !b.Equals("toupper") && !b.Equals("tolower"))
			{
				result = true;
			}
			break;
		case "or":
			if (b.Equals("and") || b.Equals("or"))
			{
				result = true;
			}
			break;
		case "and":
			if (b.Equals("and"))
			{
				result = true;
			}
			break;
		}
		return result;
	}

	public static IControlEx CheckExistControl(XtraFormEx form, string itemId)
	{
		return form?.LstControlSource.FirstOrDefault((IControlEx t) => t.FieldName.Equals(itemId));
	}

	public static Component CheckExistGrid(List<Component> lst, string itemId)
	{
		try
		{
			for (int i = 0; i < lst.Count; i++)
			{
				dynamic val = Function.ToString(((dynamic)lst[i]).ItemId);
				if (!string.IsNullOrEmpty(val) && val.ToLower().Equals(itemId.ToLower()))
				{
					return lst[i];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public static ViewEditModel CheckExistGrid(List<ViewEditModel> lst, string itemId)
	{
		try
		{
			for (int i = 0; i < lst.Count; i++)
			{
				dynamic val = Function.ToString(((dynamic)lst[i].Grid).ItemId);
				if (!string.IsNullOrEmpty(val) && val.ToLower().Equals(itemId.ToLower()))
				{
					return lst[i];
				}
			}
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
		return null;
	}

	public static bool CheckExistGridColumn(ViewEditModel lst, string fieldId)
	{
		if (lst == null || string.IsNullOrEmpty(fieldId))
		{
			return false;
		}
		DataTable dataSource = lst.DataSource;
		if (dataSource == null || dataSource.Columns.Count == 0)
		{
			return false;
		}
		return dataSource.Columns.Cast<DataColumn>().Any((DataColumn t) => t.ColumnName.Equals(fieldId));
	}

	public static List<ViewEditModel> GetListEditGriview(XtraFormEx form)
	{
		List<ViewEditModel> list = new List<ViewEditModel>();
		List<EditorContainer> lst = new List<EditorContainer>();
		foreach (Control control in form.Controls)
		{
			GetControl(control, ref lst);
		}
		for (int i = 0; i < lst.Count; i++)
		{
			for (int j = i + 1; j < lst.Count; j++)
			{
				if (lst[i].Location.Y > lst[j].Location.Y)
				{
					EditorContainer value = lst[i];
					lst[i] = lst[j];
					lst[j] = value;
				}
			}
			EditorContainer editorContainer = lst[i];
			if (editorContainer is GridControlEx)
			{
				GridControlEx gridControlEx = editorContainer as GridControlEx;
				for (int k = 0; k < gridControlEx.ViewCollection.Count; k++)
				{
					if (gridControlEx.ViewCollection[k] is GridViewEx)
					{
						GridViewEx gridViewEx = gridControlEx.ViewCollection[k] as GridViewEx;
						if (!string.IsNullOrEmpty(gridViewEx.ItemId) && !string.IsNullOrEmpty(gridViewEx.FormId))
						{
							list.Add(new ViewEditModel
							{
								Grid = gridViewEx,
								ColumnEdit = gridViewEx.EditColumn
							});
						}
					}
					else if (gridControlEx.ViewCollection[k] is BandedGridViewEx)
					{
						BandedGridViewEx bandedGridViewEx = gridControlEx.ViewCollection[k] as BandedGridViewEx;
						if (!string.IsNullOrEmpty(bandedGridViewEx.ItemId) && !string.IsNullOrEmpty(bandedGridViewEx.FormId))
						{
							list.Add(new ViewEditModel
							{
								Grid = bandedGridViewEx,
								ColumnEdit = bandedGridViewEx.EditColumn
							});
						}
					}
				}
			}
			else if (editorContainer is TreeEx)
			{
				TreeEx treeEx = editorContainer as TreeEx;
				list.Add(new ViewEditModel
				{
					Grid = treeEx,
					ColumnEdit = treeEx.EditColumn,
					IndexEditRow = ((treeEx.IndexEditRow < 0) ? treeEx.GetFocusedDataSourceRowIndex() : treeEx.IndexEditRow)
				});
			}
		}
		return list;
	}

	public static FormEditControlModel GetFormEditControl(XtraFormEx form)
	{
		return new FormEditControlModel
		{
			FormEx = form
		};
	}

	private static void GetControl(Control control, ref List<EditorContainer> lst)
	{
		if (control == null)
		{
			return;
		}
		if (control is GridControlEx)
		{
			lst.Add(control as GridControlEx);
			return;
		}
		if (control is TreeEx)
		{
			lst.Add(control as TreeEx);
			return;
		}
		foreach (Control control2 in control.Controls)
		{
			GetControl(control2, ref lst);
		}
	}

	public static CtdMessage ParseObj(List<Token> lst)
	{
		try
		{
			if (lst == null || lst.Count == 0)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not get formular to parse object"
				};
			}
			List<Token> list = new List<Token>();
			List<Token> list2 = new List<Token>();
			for (int i = 0; i < lst.Count; i++)
			{
				if (lst[i].Type != TypeSyntax.Operator && (lst[i].Type != TypeSyntax.KeyWord || !ConstantsApzCtd.OperatorMathBoolean.Contains(lst[i].Name.ToLower())))
				{
					list2.Add(lst[i]);
					continue;
				}
				if (ConstantsApzCtd.ListOperationBeginFormula.Contains(lst[i].Name) && (i == 0 || (lst[i - 1].Type == TypeSyntax.Operator && !lst[i - 1].Name.Equals(")"))))
				{
					list2.Add(new Token
					{
						Name = "-1",
						DataType = DataTypeCtd.Int,
						Type = TypeSyntax.Constant,
						Value = ((!lst[i].Name.Equals("-")) ? 1 : (-1))
					});
					lst[i].Name = "*";
				}
				if (list.Count == 0)
				{
					list.Add(lst[i]);
				}
				else if (lst[i].Name.Equals(")"))
				{
					int num = list.Count - 1;
					while (!list[num].Name.Equals("("))
					{
						list2.Add(list[num]);
						list.RemoveAt(num);
						num--;
					}
					list.RemoveAt(num);
				}
				else if (lst[i].Name.Equals("("))
				{
					list.Add(lst[i]);
				}
				else
				{
					while (list.Count > 0 && CompareOperatorLevel(list[list.Count - 1].Name, lst[i].Name))
					{
						list2.Add(list[list.Count - 1]);
						list.RemoveAt(list.Count - 1);
					}
					list.Add(lst[i]);
				}
			}
			while (list.Count > 0)
			{
				list2.Add(list[list.Count - 1]);
				list.RemoveAt(list.Count - 1);
			}
			if (list2.Count >= 1)
			{
				List<Token> list3 = new List<Token>();
				for (int j = 0; j < list2.Count; j++)
				{
					if (list2[j].Type != TypeSyntax.Operator && (list2[j].Type != TypeSyntax.KeyWord || !ConstantsApzCtd.OperatorMathBoolean.Contains(list2[j].Name.ToLower())))
					{
						list3.Add(list2[j]);
						continue;
					}
					switch (list2[j].Name.ToLower())
					{
					case "+":
						if (list3.Count > 1)
						{
							CtdMessage ctdMessage = CheckInputMathFunction(list3[list3.Count - 2], list3[list3.Count - 1]);
							if (!string.IsNullOrEmpty(ctdMessage.Message))
							{
								return ctdMessage;
							}
							if (list3[list3.Count - 2].DataType == DataTypeCtd.String || list3[list3.Count - 1].DataType == DataTypeCtd.String)
							{
								list3[list3.Count - 2].DataType = DataTypeCtd.String;
							}
							else if (list3[list3.Count - 2].DataType == DataTypeCtd.Double || list3[list3.Count - 1].DataType == DataTypeCtd.Double || list3[list3.Count - 2].DataType == DataTypeCtd.Numeric || list3[list3.Count - 1].DataType == DataTypeCtd.Numeric || (list3[list3.Count - 2].DataType == DataTypeCtd.Object && list3[list3.Count - 1].DataType == DataTypeCtd.Object))
							{
								list3[list3.Count - 2].DataType = DataTypeCtd.Double;
							}
							else
							{
								if (list3[list3.Count - 2].DataType != DataTypeCtd.Int || list3[list3.Count - 1].DataType != DataTypeCtd.Int)
								{
									return new CtdMessage
									{
										Type = MessageType.ERROR,
										Message = "Could not parse statment"
									};
								}
								list3[list3.Count - 2].DataType = DataTypeCtd.Int;
							}
							list3.RemoveAt(list3.Count - 1);
						}
						else if (list3.Count == 1)
						{
							if (list3[list3.Count - 1].DataType != DataTypeCtd.Int && list3[list3.Count - 1].DataType != DataTypeCtd.Double && list3[list3.Count - 1].DataType != DataTypeCtd.Numeric && list3[list3.Count - 1].DataType != DataTypeCtd.Object)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Could not parse statment. Could not use function for data type difirent numberic"
								};
							}
							if (list3[list3.Count - 1].DataType == DataTypeCtd.Double || list3[list3.Count - 1].DataType == DataTypeCtd.Numeric || list3[list3.Count - 1].DataType == DataTypeCtd.Object)
							{
								list3[0].DataType = DataTypeCtd.Double;
							}
							else
							{
								list3[0].DataType = DataTypeCtd.Int;
							}
						}
						else if (list3.Count == 0)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Could not parse statment"
							};
						}
						break;
					case "-":
						if (list3.Count > 1)
						{
							CtdMessage ctdMessage = CheckInputMathFunction(list3[list3.Count - 2], list3[list3.Count - 1]);
							if (!string.IsNullOrEmpty(ctdMessage.Message))
							{
								return ctdMessage;
							}
							if (list3[list3.Count - 2].DataType == DataTypeCtd.String || list3[list3.Count - 1].DataType == DataTypeCtd.String)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Statment '-' could not use for string"
								};
							}
							if (list3[list3.Count - 2].DataType == DataTypeCtd.Double || list3[list3.Count - 1].DataType == DataTypeCtd.Double || list3[list3.Count - 2].DataType == DataTypeCtd.Numeric || list3[list3.Count - 1].DataType == DataTypeCtd.Numeric || (list3[list3.Count - 2].DataType == DataTypeCtd.Object && list3[list3.Count - 1].DataType == DataTypeCtd.Object))
							{
								list3[list3.Count - 2].DataType = DataTypeCtd.Double;
							}
							else
							{
								list3[list3.Count - 2].DataType = DataTypeCtd.Int;
							}
							list3.RemoveAt(list3.Count - 1);
						}
						else if (list3.Count == 1)
						{
							if (list3[list3.Count - 1].DataType != DataTypeCtd.Int && list3[list3.Count - 1].DataType != DataTypeCtd.Double && list3[list3.Count - 1].DataType != DataTypeCtd.Numeric && list3[list3.Count - 1].DataType != DataTypeCtd.Object)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Could not parse statment. Could not usr function '-' for data type difirent numberic"
								};
							}
							if (list3[list3.Count - 1].DataType == DataTypeCtd.Double || list3[list3.Count - 1].DataType == DataTypeCtd.Numeric || list3[list3.Count - 1].DataType == DataTypeCtd.Object)
							{
								list3[0].DataType = DataTypeCtd.Double;
							}
							else
							{
								list3[0].DataType = DataTypeCtd.Int;
							}
						}
						else if (list3.Count == 0)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Could not parse statment '-'. Missing paramater"
							};
						}
						break;
					case "^":
						if (list3.Count > 1)
						{
							CtdMessage ctdMessage = CheckInputMathFunction(list3[list3.Count - 2], list3[list3.Count - 1]);
							if (!string.IsNullOrEmpty(ctdMessage.Message))
							{
								return ctdMessage;
							}
							if (list3[list3.Count - 2].DataType == DataTypeCtd.String || list3[list3.Count - 1].DataType == DataTypeCtd.String)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Statment '^' could not use for string"
								};
							}
							list3[list3.Count - 2].DataType = DataTypeCtd.Double;
							list3.RemoveAt(list3.Count - 1);
							break;
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Could not parse statment '^'. Missing paramater"
						};
					case "*":
					case "/":
						if (list3.Count > 1)
						{
							CtdMessage ctdMessage = CheckInputMathFunction(list3[list3.Count - 2], list3[list3.Count - 1]);
							if (!string.IsNullOrEmpty(ctdMessage.Message))
							{
								return ctdMessage;
							}
							if (list3[list3.Count - 2].DataType == DataTypeCtd.String || list3[list3.Count - 1].DataType == DataTypeCtd.String)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = $"Function '{list2[j].Name.ToLower()}' could not use for string"
								};
							}
							if (list3[list3.Count - 2].DataType == DataTypeCtd.Double || list3[list3.Count - 1].DataType == DataTypeCtd.Double || list3[list3.Count - 2].DataType == DataTypeCtd.Numeric || list3[list3.Count - 1].DataType == DataTypeCtd.Numeric || (list3[list3.Count - 2].DataType == DataTypeCtd.Object && list3[list3.Count - 1].DataType == DataTypeCtd.Object))
							{
								list3[list3.Count - 2].DataType = DataTypeCtd.Double;
							}
							else
							{
								list3[list3.Count - 2].DataType = DataTypeCtd.Int;
							}
							list3.RemoveAt(list3.Count - 1);
							break;
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Could not parse statment '/'. Missing paramater"
						};
					case "%":
						if (list3.Count > 1)
						{
							CtdMessage ctdMessage = CheckInputMathFunction(list3[list3.Count - 2], list3[list3.Count - 1]);
							if (!string.IsNullOrEmpty(ctdMessage.Message))
							{
								return ctdMessage;
							}
							if (list3[list3.Count - 2].DataType == DataTypeCtd.String || list3[list3.Count - 1].DataType == DataTypeCtd.String)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Statment '%' could not use for string"
								};
							}
							if (list3[list3.Count - 2].DataType == DataTypeCtd.Double || list3[list3.Count - 1].DataType == DataTypeCtd.Double || list3[list3.Count - 2].DataType == DataTypeCtd.Numeric || list3[list3.Count - 1].DataType == DataTypeCtd.Numeric)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Statment '%' could not use for double"
								};
							}
							list3[list3.Count - 2].DataType = DataTypeCtd.Int;
							list3.RemoveAt(list3.Count - 1);
							break;
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Could not parse statment '%'. Missing paramater"
						};
					case ">=":
					case "<=":
					case "!=":
					case "<>":
					case "==":
					case "<":
					case ">":
						if (list3.Count > 1)
						{
							if (((list2[j].Name.ToLower().Equals("==") || list2[j].Name.ToLower().Equals("!=") || list2[j].Name.ToLower().Equals("<>")) && list3[list3.Count - 2].DataType == DataTypeCtd.Boolean) || list3[list3.Count - 1].DataType == DataTypeCtd.Boolean)
							{
								list3[list3.Count - 2].DataType = DataTypeCtd.Boolean;
								list3.RemoveAt(list3.Count - 1);
								break;
							}
							if (list3[list3.Count - 2].DataType == DataTypeCtd.DateTime && list3[list3.Count - 1].DataType == DataTypeCtd.DateTime)
							{
								list3[list3.Count - 2].DataType = DataTypeCtd.Boolean;
								list3.RemoveAt(list3.Count - 1);
								break;
							}
							CtdMessage ctdMessage = CheckInputMathFunction(list3[list3.Count - 2], list3[list3.Count - 1]);
							if (!string.IsNullOrEmpty(ctdMessage.Message))
							{
								return ctdMessage;
							}
							if ((list3[list3.Count - 2].DataType == DataTypeCtd.String || list3[list3.Count - 1].DataType == DataTypeCtd.String) && !list2[j].Name.ToLower().Equals("==") && !list2[j].Name.ToLower().Equals("!=") && !list2[j].Name.ToLower().Equals("<>"))
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = $"Function '{list2[j].Name.ToLower()}' could not use for string"
								};
							}
							list3[list3.Count - 2].DataType = DataTypeCtd.Boolean;
							list3.RemoveAt(list3.Count - 1);
							break;
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = $"Function '{list2[j].Name.ToLower()}'. Could not parse statment"
						};
					case "or":
					case "and":
						if (list3.Count > 1 && list3[list3.Count - 2].DataType == DataTypeCtd.Boolean && list3[list3.Count - 1].DataType == DataTypeCtd.Boolean)
						{
							list3.RemoveAt(list3.Count - 1);
							break;
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = $"Function '{list2[j].Name.ToLower()}' could not use for datatype diffirent boolean"
						};
					}
				}
				return new CtdMessage
				{
					Obj = list3[0].DataType
				};
			}
			if (list2[0].Type != TypeSyntax.Operator && (list2[0].Type != TypeSyntax.KeyWord || !ConstantsApzCtd.OperatorMathBoolean.Contains(list2[0].Name.ToLower())))
			{
				return new CtdMessage
				{
					Obj = list2[0].DataType
				};
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statment '-'. Missing paramater"
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message, "CTD");
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = ex.Message
			};
		}
	}

	public static CtdMessage CheckInputMathFunction(Token var1, Token var2)
	{
		if (var2.DataType == DataTypeCtd.Dynamic || var1.DataType == DataTypeCtd.Dynamic)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "could not use function with datatype 'char'"
			};
		}
		if (var2.DataType == DataTypeCtd.ColumnData || var1.DataType == DataTypeCtd.ColumnData)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "could not math in a list"
			};
		}
		if (var2.DataType == DataTypeCtd.DateTime || var1.DataType == DataTypeCtd.DateTime)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "could not use function with datatype datetime"
			};
		}
		if (var2.DataType == DataTypeCtd.DataRow || var1.DataType == DataTypeCtd.DataRow)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not use function with datatype datarow"
			};
		}
		if (var2.DataType == DataTypeCtd.DataTable || var1.DataType == DataTypeCtd.DataTable)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Can not use function with datatype datatable"
			};
		}
		if (var2.DataType == DataTypeCtd.DataSet || var1.DataType == DataTypeCtd.DataSet)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Can not use function with datatype dataset"
			};
		}
		if (var2.DataType == DataTypeCtd.Boolean || var1.DataType == DataTypeCtd.Boolean)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Can not use function with variable boolean"
			};
		}
		return new CtdMessage();
	}

	public static dynamic GetValueSystemByName(string name, int rowHandle = -1)
	{
		return name.ToLower() switch
		{
			"@branchid" => (dynamic)GlobalData.DefaultBranch, 
			"@username" => GlobalData.UserName, 
			"@userid" => GlobalData.UserID, 
			"@currentdate" => DateTime.Now.Date, 
			"@currenttime" => DateTime.Now.ToString("HHmm"), 
			"@rowindex" => rowHandle, 
			"@executepath" => Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), 
			_ => throw new Exception("System value does not support"), 
		};
	}

	private static void ReadNewLine(StringReader reader, ref char[] charArray, ref int length, ref int lineIndex, ref int charIndex, ref string tokenName, out string textLine)
	{
		while (true)
		{
			textLine = reader.ReadLine();
			if (textLine != null)
			{
				tokenName += Environment.NewLine;
				lineIndex++;
				charArray = textLine.ToCharArray();
				length = charArray.Length;
				if (length != 0)
				{
					charIndex = 0;
					break;
				}
				continue;
			}
			break;
		}
	}

	private static void ReadCommentToken(StringReader reader, ref char[] charArray, ref int lengh, ref int lineIndex, ref int charIndex, ref string tokenName, ref string textLine)
	{
		charIndex++;
		if (charIndex == lengh)
		{
			ReadNewLine(reader, ref charArray, ref lengh, ref lineIndex, ref charIndex, ref tokenName, out textLine);
		}
		while (charIndex < lengh)
		{
			if (ToString(charArray[charIndex]).Equals("\""))
			{
				if (charIndex >= lengh - 1 || !ToString(charArray[charIndex + 1]).Equals("\""))
				{
					break;
				}
				tokenName += charArray[charIndex++];
				charIndex++;
				if (charIndex == lengh)
				{
					ReadNewLine(reader, ref charArray, ref lengh, ref lineIndex, ref charIndex, ref tokenName, out textLine);
				}
			}
			else
			{
				tokenName += charArray[charIndex++];
				if (charIndex == lengh)
				{
					ReadNewLine(reader, ref charArray, ref lengh, ref lineIndex, ref charIndex, ref tokenName, out textLine);
				}
			}
		}
		if (charIndex == lengh && !ToString(charArray[charIndex - 1]).Equals("\""))
		{
			throw new ArgumentException($"Line {lineIndex} Missing \"");
		}
	}

	private static bool AddTokenToList(int lineIndex, char[] charArray, int charIndex, string tokenNameLower, ref string tokenName, ref List<Token> lstToken, int rowHandle = -1)
	{
		if (ConstantsApzCtd.KeyWordOperator.Contains(tokenNameLower))
		{
			lstToken.Add(new Token
			{
				Name = tokenName,
				Type = TypeSyntax.Operator,
				LineNum = lineIndex
			});
			tokenName = "";
		}
		else if (ConstantsApzCtd.ListBoolVariable.Contains(tokenNameLower))
		{
			lstToken.Add(new Token
			{
				Name = tokenNameLower,
				Type = TypeSyntax.Constant,
				Value = tokenNameLower.Equals("true"),
				DataType = DataTypeCtd.Boolean,
				LineNum = lineIndex
			});
			tokenName = "";
		}
		else if (ConstantsApzCtd.KeyWord.Contains(tokenNameLower))
		{
			lstToken.Add(new Token
			{
				Name = tokenNameLower,
				Type = TypeSyntax.KeyWord,
				LineNum = lineIndex
			});
			tokenName = "";
		}
		else if (IsNumeric(tokenName))
		{
			if (charIndex < charArray.Length && ToString(charArray[charIndex]).Equals("."))
			{
				tokenName += charArray[charIndex];
				return true;
			}
			Token token = new Token
			{
				Name = tokenName,
				Type = TypeSyntax.Constant,
				DataType = (IsInt(tokenName) ? DataTypeCtd.Int : DataTypeCtd.Double),
				LineNum = lineIndex
			};
			if (IsInt(tokenName))
			{
				token.Value = tokenName;
			}
			else
			{
				token.Value = Function.ParseDouble(tokenName, GlobalData.QuantitiesFormat);
			}
			lstToken.Add(token);
			tokenName = "";
		}
		else if (ConstantsApzCtd.ListSystemValueString.Contains(tokenNameLower))
		{
			lstToken.Add(new Token
			{
				Name = tokenNameLower,
				Type = TypeSyntax.Constant,
				DataType = DataTypeCtd.String,
				LineNum = lineIndex
			});
			tokenName = "";
		}
		else if (ConstantsApzCtd.ListSystemValueInt.Contains(tokenNameLower))
		{
			lstToken.Add(new Token
			{
				Name = tokenNameLower,
				Type = TypeSyntax.Constant,
				DataType = DataTypeCtd.Int,
				LineNum = lineIndex
			});
			tokenName = "";
		}
		else if (ConstantsApzCtd.ListSystemValueDouble.Contains(tokenNameLower))
		{
			lstToken.Add(new Token
			{
				Name = tokenNameLower,
				Type = TypeSyntax.Constant,
				DataType = DataTypeCtd.Double,
				LineNum = lineIndex
			});
			tokenName = "";
		}
		else if (ConstantsApzCtd.ListSystemValueDatetime.Contains(tokenNameLower))
		{
			lstToken.Add(new Token
			{
				Name = tokenNameLower,
				Type = TypeSyntax.Constant,
				DataType = DataTypeCtd.DateTime,
				LineNum = lineIndex
			});
			tokenName = "";
		}
		else if (!string.IsNullOrEmpty(tokenName))
		{
			lstToken.Add(new Token
			{
				Name = tokenName,
				Type = TypeSyntax.Variable,
				Value = tokenName,
				DataType = DataTypeCtd.None,
				LineNum = lineIndex
			});
			tokenName = "";
		}
		return false;
	}

	public static List<Token> GetToken(string expression)
	{
		if (string.IsNullOrEmpty(expression))
		{
			throw new Exception("Expression could not null");
		}
		List<Token> lstToken = new List<Token>();
		using StringReader stringReader = new StringReader(expression);
		int lineIndex = 0;
		for (string textLine = stringReader.ReadLine(); textLine != null; textLine = stringReader.ReadLine())
		{
			lineIndex++;
			char[] charArray = textLine.ToCharArray();
			int lengh = charArray.Length;
			string tokenName = "";
			for (int i = 0; i < lengh; i++)
			{
				if (ToString(charArray[i]).Equals("\"") && string.IsNullOrEmpty(tokenName))
				{
					ReadCommentToken(stringReader, ref charArray, ref lengh, ref lineIndex, ref i, ref tokenName, ref textLine);
					lstToken.Add(new Token
					{
						Name = tokenName,
						Type = TypeSyntax.Constant,
						Value = tokenName,
						DataType = DataTypeCtd.String,
						LineNum = lineIndex
					});
					tokenName = "";
				}
				else if (ConstantsApzCtd.SpecialCharactersApz.Contains(ToString(charArray[i])))
				{
					if (!string.IsNullOrEmpty(tokenName))
					{
						string tokenNameLower = tokenName.ToLower();
						if (AddTokenToList(lineIndex, charArray, i, tokenNameLower, ref tokenName, ref lstToken))
						{
							continue;
						}
					}
					if (ConstantsApzCtd.PrefixComparisonApz.Contains(ToString(charArray[i])))
					{
						if (i < lengh - 1 && ToString(charArray[i + 1]).Equals("="))
						{
							lstToken.Add(new Token
							{
								Name = charArray[i++] + "=",
								Type = TypeSyntax.Operator,
								LineNum = lineIndex
							});
						}
						else if (ToString(charArray[i]).Equals("<") && i < lengh - 1 && ToString(charArray[i + 1]).Equals(">"))
						{
							lstToken.Add(new Token
							{
								Name = charArray[i++] + ">",
								Type = TypeSyntax.Operator,
								LineNum = lineIndex
							});
						}
						else
						{
							lstToken.Add(new Token
							{
								Name = ToString(charArray[i]),
								Type = TypeSyntax.Operator,
								LineNum = lineIndex
							});
						}
					}
					else if (ToString(charArray[i]).Equals("/") && i < lengh - 1 && ToString(charArray[i + 1]).Equals("*"))
					{
						i += 2;
						int num;
						for (num = textLine.IndexOf("*/"); num == -1; num = textLine.IndexOf("*/"))
						{
							lineIndex++;
							textLine = stringReader.ReadLine();
							if (textLine == null)
							{
								throw new ArgumentException($"Line {lineIndex} comment mising /*");
							}
						}
						i = num + 1;
						charArray = textLine.ToCharArray();
						tokenName = "";
					}
					else if (ToString(charArray[i]).Equals("/") && i < lengh - 1 && ToString(charArray[i + 1]).Equals("/"))
					{
						i = lengh;
						tokenName = "";
					}
					else if (ConstantsApzCtd.OperatorApz.Contains(ToString(charArray[i])))
					{
						lstToken.Add(new Token
						{
							Name = ToString(charArray[i]),
							Type = TypeSyntax.Operator,
							LineNum = lineIndex
						});
					}
					else if (!ToString(charArray[i]).Equals(" ") && charArray[i] != '\r' && charArray[i] != '\t' && charArray[i] != '\n')
					{
						throw new ArgumentException(string.Format("The command line {1} ends with a special character {0}", charArray[i], lineIndex));
					}
				}
				else if (!string.IsNullOrEmpty(tokenName) || (!ToString(charArray[i]).Equals(" ") && charArray[i] != '\t' && charArray[i] != '\r' && charArray[i] != '\n'))
				{
					tokenName += charArray[i];
				}
			}
			if (!string.IsNullOrEmpty(tokenName))
			{
				AddTokenToList(lineIndex, charArray, lengh, tokenName.ToLower(), ref tokenName, ref lstToken);
			}
		}
		return lstToken;
	}

	public static Color GetColorByName(string nameColor)
	{
		return nameColor switch
		{
			"aliceblue" => Color.AliceBlue, 
			"antiquewhite" => Color.AntiqueWhite, 
			"aqua" => Color.Aqua, 
			"aquamarine" => Color.Aquamarine, 
			"azure" => Color.Azure, 
			"beige" => Color.Beige, 
			"bisque" => Color.Bisque, 
			"black" => Color.Black, 
			"blanchedalmond" => Color.BlanchedAlmond, 
			"blue" => Color.Blue, 
			"blueviolet" => Color.BlueViolet, 
			"brown" => Color.Brown, 
			"burlywood" => Color.BurlyWood, 
			"cadetblue" => Color.CadetBlue, 
			"chartreuse" => Color.Chartreuse, 
			"chocolate" => Color.Chocolate, 
			"coral" => Color.Coral, 
			"cornflowerblue" => Color.CornflowerBlue, 
			"cornsilk" => Color.Cornsilk, 
			"crimson" => Color.Crimson, 
			"cyan" => Color.Cyan, 
			"darkblue" => Color.DarkBlue, 
			"darkcyan" => Color.DarkCyan, 
			"darkgoldenrod" => Color.DarkGoldenrod, 
			"darkgreen" => Color.DarkGreen, 
			"darkkhaki" => Color.DarkKhaki, 
			"darkmagenta" => Color.DarkMagenta, 
			"darkolivegreen" => Color.DarkOliveGreen, 
			"darkorange" => Color.DarkOrange, 
			"darkorchid" => Color.DarkOrchid, 
			"darkred" => Color.DarkRed, 
			"darksalmon" => Color.DarkSalmon, 
			"darkseagreen" => Color.DarkSeaGreen, 
			"darkslateblue" => Color.DarkSlateBlue, 
			"darkslategray" => Color.DarkSlateGray, 
			"darkturquoise" => Color.DarkTurquoise, 
			"darkviolet" => Color.DarkViolet, 
			"deeppink" => Color.DeepPink, 
			"deepSkyblue" => Color.DeepSkyBlue, 
			"dimgray" => Color.DimGray, 
			"dodgerblue" => Color.DodgerBlue, 
			"firebrick" => Color.Firebrick, 
			"floralwhite" => Color.FloralWhite, 
			"forestgreen" => Color.ForestGreen, 
			"fuchsia" => Color.Fuchsia, 
			"gainsboro" => Color.Gainsboro, 
			"ghostwhite" => Color.GhostWhite, 
			"gold" => Color.Gold, 
			"goldenrod" => Color.Goldenrod, 
			"gray" => Color.Gray, 
			"green" => Color.Green, 
			"greenyellow" => Color.GreenYellow, 
			"honeydew" => Color.Honeydew, 
			"hotpink" => Color.HotPink, 
			"indianred" => Color.IndianRed, 
			"indigo" => Color.Indigo, 
			"ivory" => Color.Ivory, 
			"khaki" => Color.Khaki, 
			"lavender" => Color.Lavender, 
			"lavenderblush" => Color.LavenderBlush, 
			"lawngreen" => Color.LawnGreen, 
			"lemonchiffon" => Color.LemonChiffon, 
			"lightblue" => Color.LightBlue, 
			"lightcoral" => Color.LightCoral, 
			"lightcyan" => Color.LightCyan, 
			"lightgoldenrodyellow" => Color.LightGoldenrodYellow, 
			"lightgreen" => Color.LightGreen, 
			"lightpink" => Color.LightPink, 
			"lightsalmon" => Color.LightSalmon, 
			"lightseagreen" => Color.LightSeaGreen, 
			"lightskyblue" => Color.LightSkyBlue, 
			"lightslategray" => Color.LightSlateGray, 
			"lightsteelblue" => Color.LightSteelBlue, 
			"lightyellow" => Color.LightYellow, 
			"lime" => Color.Lime, 
			"limegreen" => Color.LimeGreen, 
			"linen" => Color.Linen, 
			"magenta" => Color.Magenta, 
			"maroon" => Color.Maroon, 
			"mediumaquamarine" => Color.MediumAquamarine, 
			"mediumblue" => Color.MediumBlue, 
			"mediumorchid" => Color.MediumOrchid, 
			"mediumpurple" => Color.MediumPurple, 
			"mediumseagreen" => Color.MediumSeaGreen, 
			"mediumslateblue" => Color.MediumSlateBlue, 
			"mediumspringgreen" => Color.MediumSpringGreen, 
			"mediumturquoise" => Color.MediumTurquoise, 
			"mediumvioletred" => Color.MediumVioletRed, 
			"midnightblue" => Color.MidnightBlue, 
			"mintcream" => Color.MintCream, 
			"mistyrose" => Color.MistyRose, 
			"moccasin" => Color.Moccasin, 
			"navajowhite" => Color.NavajoWhite, 
			"navy" => Color.Navy, 
			"oldlace" => Color.OldLace, 
			"olive" => Color.Olive, 
			"olivedrab" => Color.OliveDrab, 
			"orange" => Color.Orange, 
			"orangered" => Color.OrangeRed, 
			"orchid" => Color.Orchid, 
			"palegoldenrod" => Color.PaleGoldenrod, 
			"palegreen" => Color.PaleGreen, 
			"paleturquoise" => Color.PaleTurquoise, 
			"palevioletred" => Color.PaleVioletRed, 
			"papayawhip" => Color.PapayaWhip, 
			"peachpuff" => Color.PeachPuff, 
			"peru" => Color.Peru, 
			"pink" => Color.Pink, 
			"plum" => Color.Plum, 
			"powderblue" => Color.PowderBlue, 
			"purple" => Color.Purple, 
			"red" => Color.Red, 
			"rosybrown" => Color.RosyBrown, 
			"royalblue" => Color.RoyalBlue, 
			"saddlebrown" => Color.SaddleBrown, 
			"salmon" => Color.Salmon, 
			"sandybrown" => Color.SandyBrown, 
			"seagreen" => Color.SeaGreen, 
			"seashell" => Color.SeaShell, 
			"sienna" => Color.Sienna, 
			"silver" => Color.Silver, 
			"skyblue" => Color.SkyBlue, 
			"slateblue" => Color.SlateBlue, 
			"slategray" => Color.SlateGray, 
			"snow" => Color.Snow, 
			"springgreen" => Color.SpringGreen, 
			"steelblue" => Color.SteelBlue, 
			"tan" => Color.Tan, 
			"teal" => Color.Teal, 
			"thistle" => Color.Thistle, 
			"tomato" => Color.Tomato, 
			"transparent" => Color.Transparent, 
			"turquoise" => Color.Turquoise, 
			"violet" => Color.Violet, 
			"wheat" => Color.Wheat, 
			"white" => Color.White, 
			"whitesmoke" => Color.WhiteSmoke, 
			"yellow" => Color.Yellow, 
			"yellowgreen" => Color.YellowGreen, 
			_ => Color.White, 
		};
	}
}
