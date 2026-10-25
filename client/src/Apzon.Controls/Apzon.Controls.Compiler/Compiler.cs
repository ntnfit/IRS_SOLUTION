using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.ApzJson;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.Controls.Commons;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Common;
using DevExpress.XtraEditors;
using Newtonsoft.Json;

namespace Apzon.Controls.Compiler;

public class Compiler
{
	private readonly string _url = "";

	private readonly HttpClient _httpClientHelper = new HttpClient();

	public bool CancelEvent;

	private XtraFormEx _form;

	private List<ViewEditModel> _lstGrid;

	private List<Token> _syntax;

	private int _index;

	private List<Token> _variable;

	private GridViewEx _gridView;

	private Dictionary<string, object> _outVariable;

	private readonly int _rowHandle;

	private readonly string _columnEditFieldName;

	public Compiler(string api = "", string session = "", int rowHandle = -1, string columnEdit = "")
	{
		if (!string.IsNullOrEmpty(api) && !string.IsNullOrEmpty(session))
		{
			_httpClientHelper.Timeout = new TimeSpan(0, 30, 0);
			_httpClientHelper.DefaultRequestHeaders.Add("SessionId", session);
			_url = api;
		}
		_rowHandle = rowHandle;
		_columnEditFieldName = columnEdit;
	}

	public CtdMessage ParseSyntax(XtraFormEx form, string expression)
	{
		try
		{
			if (form == null || string.IsNullOrEmpty(form.FormId))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "form could not null"
				};
			}
			List<Token> token = ControlFunction.GetToken(expression);
			if (token.Count == 0)
			{
				return new CtdMessage();
			}
			List<ViewEditModel> listEditGriview = ControlFunction.GetListEditGriview(form);
			return new StateMachines(form, listEditGriview, token).ParseStatment(StatmentType.Normal);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString(), "CTD");
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statment. Missing operator"
			};
		}
	}

	public CtdMessage UserdefinedFomular(XtraFormEx form, List<Token> syntax, out bool cancelEvent)
	{
		try
		{
			CancelEvent = false;
			if (syntax == null || syntax.Count == 0)
			{
				cancelEvent = false;
				return new CtdMessage();
			}
			_syntax = syntax;
			_index = 0;
			_variable = new List<Token>();
			_lstGrid = ControlFunction.GetListEditGriview(form);
			_form = form;
			_outVariable = new Dictionary<string, object>();
			MathStatment();
			cancelEvent = CancelEvent;
			return new CtdMessage();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString(), "CTD");
			cancelEvent = CancelEvent;
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = ex.Message
			};
		}
	}

	public CtdMessage UserdefinedFomular(XtraFormEx form, GridViewEx gridView, List<Token> syntax, out bool cancelEvent)
	{
		try
		{
			_gridView = gridView;
			return UserdefinedFomular(form, syntax, out cancelEvent);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex.ToString(), "CTD");
			cancelEvent = CancelEvent;
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = ex.Message
			};
		}
	}

	private CtdMessage MathStatment()
	{
		_variable.Add(new Token
		{
			Name = "0"
		});
		while (_index < _syntax.Count)
		{
			switch (_syntax[_index].Type)
			{
			case TypeSyntax.KeyWord:
				switch (_syntax[_index].Name.ToLower())
				{
				case "if":
					MathIf();
					_index--;
					break;
				case "for":
					_variable.Add(new Token
					{
						Name = "0"
					});
					MathFor();
					RemoveVariable();
					_index--;
					break;
				case "while":
					MathWhile();
					_index--;
					break;
				case "cancelevent":
					MathVariable();
					_index--;
					break;
				case "showmessagebox":
					MathShowMessageBox();
					break;
				case "drilldown":
					MathDrillDown();
					break;
				case "showerror":
					MathShowStatus(ShowMessageType.Error);
					break;
				case "showsuccess":
					MathShowStatus(ShowMessageType.Success);
					break;
				case "showwarning":
					MathShowStatus(ShowMessageType.Warning);
					break;
				case "showinfo":
					MathShowStatus(ShowMessageType.Info);
					break;
				case "openlink":
					OpenLink();
					break;
				case "add":
					MathFunctionList();
					break;
				case "mergepdf":
					MathMergePDF();
					_index--;
					break;
				case "processsapobject":
					MathProcessSAPObject();
					_index--;
					break;
				case "return":
					return new CtdMessage
					{
						Action = ActionStatment.Return
					};
				case "continue":
				case "break":
					return new CtdMessage
					{
						Action = ((!_syntax[_index].Name.ToLower().Equals("continue")) ? ActionStatment.Break : ActionStatment.Continue)
					};
				case "push":
					MathPushStatment();
					break;
				case "invoke":
					MathInvokMethod();
					_index--;
					break;
				case "showform":
					MathShowFormAndShowDialogMethod();
					_index--;
					break;
				case "showdialog":
					MathShowFormAndShowDialogMethod(isDialog: true);
					_index--;
					break;
				default:
					MathAssign();
					_index--;
					break;
				}
				break;
			case TypeSyntax.Operator:
				switch (_syntax[_index].Name)
				{
				case "$":
					MathVariable();
					_index--;
					break;
				case ";":
					_index++;
					continue;
				case "}":
					_index++;
					return new CtdMessage();
				}
				break;
			case TypeSyntax.Variable:
				if (_variable.Find((Token t) => t.Name == _syntax[_index].Name.ToLower()).DataType != DataTypeCtd.List || _syntax[_index + 1].Type != TypeSyntax.Operator || !_syntax[_index + 1].Name.Equals("."))
				{
					MathVariable();
					_index--;
				}
				else
				{
					MathFunctionList();
				}
				break;
			}
			_index++;
		}
		return new CtdMessage();
	}

	private void MathPushStatment()
	{
		_index += 2;
		Token token = MathObj();
		_index++;
		Token token2 = MathObj();
		_outVariable.Add(Function.ToString(token.Value), token2.Value);
		_index++;
	}

	private void MathWhile()
	{
		_index++;
		int index = _index;
		Token token = MathObj();
		int num = _index + 1;
		while ((bool)token.Value)
		{
			CtdMessage ctdMessage = MathStatment();
			if (ctdMessage.Action == ActionStatment.Return)
			{
				return;
			}
			RemoveVariable();
			_index = index;
			token = MathObj();
			if (ctdMessage.Action == ActionStatment.Break)
			{
				break;
			}
		}
		_index = num + 1;
		int num2 = 1;
		while (num2 > 0 && _index < _syntax.Count)
		{
			if (_syntax[_index].Type == TypeSyntax.Operator)
			{
				if (_syntax[_index].Name.Equals("{"))
				{
					num2++;
				}
				else if (_syntax[_index].Name.Equals("}"))
				{
					num2--;
				}
			}
			_index++;
		}
	}

	private void MathFor()
	{
		_index += 2;
		if (_syntax[_index].Type == TypeSyntax.KeyWord)
		{
			_index++;
			_variable.Add(new Token
			{
				Name = _syntax[_index].Name.ToLower(),
				Type = TypeSyntax.Variable,
				DataType = DataTypeCtd.Int,
				Value = 0
			});
		}
		string name = _syntax[_index].Name.ToLower();
		Token token = _variable.Find((Token t) => t.Name == name);
		_index += 2;
		Token token2 = MathObj();
		_index++;
		int index = _index;
		Token token3 = MathObj();
		_index++;
		Token token4 = MathObj();
		_index += 2;
		if (Function.ParseInt(token4.Value) > 0)
		{
			for (dynamic val = Function.ParseInt(token2.Value); val < Function.ParseInt(token3.Value); val += Function.ParseInt(token4.Value))
			{
				token.Value = (object)val;
				CtdMessage ctdMessage = MathStatment();
				if (ctdMessage.Action == ActionStatment.Return)
				{
					return;
				}
				RemoveVariable();
				_index = index;
				token3 = MathObj();
				_index++;
				token4 = MathObj();
				_index += 2;
				if (ctdMessage.Action == ActionStatment.Break)
				{
					break;
				}
			}
		}
		else
		{
			for (dynamic val2 = Function.ParseInt(token2.Value); val2 > Function.ParseInt(token3.Value); val2 += Function.ParseInt(token4.Value))
			{
				token.Value = (object)val2;
				CtdMessage ctdMessage2 = MathStatment();
				if (ctdMessage2.Action == ActionStatment.Return)
				{
					return;
				}
				RemoveVariable();
				_index = index;
				token3 = MathObj();
				_index++;
				token4 = MathObj();
				_index += 2;
				if (ctdMessage2.Action == ActionStatment.Break)
				{
					break;
				}
			}
		}
		int num = 1;
		while (num > 0 && _index < _syntax.Count)
		{
			if (_syntax[_index].Type == TypeSyntax.Operator)
			{
				if (_syntax[_index].Name.Equals("{"))
				{
					num++;
				}
				else if (_syntax[_index].Name.Equals("}"))
				{
					num--;
				}
			}
			_index++;
		}
	}

	private void MathLoop()
	{
		string itemId = _syntax[_index - 3].Name.ToLower();
		_index += 2;
		string name = _syntax[_index].Name.ToLower();
		Token token = new Token
		{
			Name = name,
			DataType = DataTypeCtd.Int,
			Value = 0
		};
		_variable.Add(token);
		_index += 3;
		int index = _index;
		ViewEditModel viewEditModel = ControlFunction.CheckExistGrid(_lstGrid, itemId);
		for (int i = 0; i < viewEditModel.DataSource.Rows.Count; i++)
		{
			token.Value = i;
			CtdMessage ctdMessage = MathStatment();
			if (ctdMessage.Action == ActionStatment.Return)
			{
				return;
			}
			RemoveVariable();
			_index = index;
			if (ctdMessage.Action == ActionStatment.Break)
			{
				break;
			}
		}
		int num = 1;
		while (num > 0 && _index < _syntax.Count)
		{
			if (_syntax[_index].Type == TypeSyntax.Operator)
			{
				if (_syntax[_index].Name.Equals("{"))
				{
					num++;
				}
				else if (_syntax[_index].Name.Equals("}"))
				{
					num--;
				}
			}
			_index++;
		}
	}

	private void MathAssign()
	{
		DataTypeCtd dataTypeCtd = DataTypeCtd.Object;
		DataTypeCtd tagetType = DataTypeCtd.Object;
		switch (_syntax[_index].Name.ToLower())
		{
		case "int":
			dataTypeCtd = DataTypeCtd.Int;
			break;
		case "double":
			dataTypeCtd = DataTypeCtd.Double;
			break;
		case "bool":
			dataTypeCtd = DataTypeCtd.Boolean;
			break;
		case "list":
			dataTypeCtd = DataTypeCtd.List;
			_index += 2;
			switch (_syntax[_index].Name.ToLower())
			{
			case "int":
				tagetType = DataTypeCtd.Int;
				break;
			case "double":
				tagetType = DataTypeCtd.Double;
				break;
			case "bool":
				tagetType = DataTypeCtd.Boolean;
				break;
			case "string":
				tagetType = DataTypeCtd.String;
				break;
			}
			_index++;
			break;
		case "datarow":
			dataTypeCtd = DataTypeCtd.DataRow;
			break;
		case "datatable":
			dataTypeCtd = DataTypeCtd.DataTable;
			break;
		case "dataset":
			dataTypeCtd = DataTypeCtd.DataSet;
			break;
		case "string":
			dataTypeCtd = DataTypeCtd.String;
			break;
		case "var":
			dataTypeCtd = DataTypeCtd.Object;
			break;
		case "date":
			dataTypeCtd = DataTypeCtd.DateTime;
			break;
		case "datetime":
			dataTypeCtd = DataTypeCtd.DateTime;
			break;
		case "httpresult":
			dataTypeCtd = DataTypeCtd.HttpResult;
			break;
		case "showdialog":
			dataTypeCtd = DataTypeCtd.Dialog;
			break;
		case "form":
			dataTypeCtd = DataTypeCtd.Form;
			break;
		}
		_index++;
		string name = _syntax[_index].Name.ToLower();
		if (_syntax[_index + 1].Name.Equals(ControlFunction.ToString(";")))
		{
			if (dataTypeCtd == DataTypeCtd.List)
			{
				_variable.Add(new Token
				{
					Name = name,
					DataType = dataTypeCtd,
					TagetType = tagetType
				});
			}
			else
			{
				_variable.Add(new Token
				{
					Name = name,
					DataType = dataTypeCtd
				});
			}
			_index += 2;
			return;
		}
		_index += 2;
		Token token = MathObj(-1, dataTypeCtd);
		switch (dataTypeCtd)
		{
		case DataTypeCtd.DateTime:
			_variable.Add(new Token
			{
				Name = name,
				DataType = dataTypeCtd,
				Value = Function.ParseDateTime(token.Value)
			});
			_index++;
			break;
		case DataTypeCtd.Double:
			_variable.Add(new Token
			{
				Name = name,
				DataType = dataTypeCtd,
				Value = Function.ParseDecimal(token.Value)
			});
			_index++;
			break;
		case DataTypeCtd.Int:
			_variable.Add(new Token
			{
				Name = name,
				DataType = dataTypeCtd,
				Value = Function.ParseInt(token.Value)
			});
			_index++;
			break;
		case DataTypeCtd.Boolean:
		case DataTypeCtd.HttpResult:
			_variable.Add(new Token
			{
				Name = name,
				DataType = dataTypeCtd,
				Value = Function.ParseBool(token.Value)
			});
			_index++;
			break;
		case DataTypeCtd.String:
			_variable.Add(new Token
			{
				Name = name,
				DataType = dataTypeCtd,
				Value = Function.ToString(token.Value)
			});
			_index++;
			break;
		case DataTypeCtd.DataTable:
		case DataTypeCtd.DataSet:
		case DataTypeCtd.DataRow:
		case DataTypeCtd.Form:
		case DataTypeCtd.Dialog:
			_variable.Add(new Token
			{
				Name = name,
				DataType = dataTypeCtd,
				Value = token.Value
			});
			_index++;
			break;
		case DataTypeCtd.List:
			_variable.Add(new Token
			{
				Name = name,
				DataType = dataTypeCtd,
				TagetType = tagetType,
				Value = token.Value
			});
			_index++;
			break;
		case DataTypeCtd.Object:
			_variable.Add(new Token
			{
				Name = name,
				DataType = token.DataType,
				Value = token.Value
			});
			break;
		case DataTypeCtd.Dynamic:
		case DataTypeCtd.Numeric:
		case (DataTypeCtd)13:
		case DataTypeCtd.ColumnData:
			break;
		}
	}

	private Token MathShowMessageBox()
	{
		_index += 2;
		Token token = MathObj();
		_index++;
		Token token2 = MathObj();
		if (_syntax[_index].Name.Equals(","))
		{
			string text = _syntax[_index + 1].Name.ToLower();
			_index += 2;
			if (text.Equals("yesno"))
			{
				if (XtraMessageBox.Show(ControlFunction.ToString(token.Value), ControlFunction.ToString(token2.Value), MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
				{
					return new Token
					{
						DataType = DataTypeCtd.Boolean,
						Type = TypeSyntax.Constant,
						Value = true
					};
				}
				return new Token
				{
					DataType = DataTypeCtd.Boolean,
					Type = TypeSyntax.Constant,
					Value = false
				};
			}
			if (XtraMessageBox.Show(ControlFunction.ToString(token.Value), ControlFunction.ToString(token2.Value), MessageBoxButtons.OKCancel) == DialogResult.OK)
			{
				return new Token
				{
					DataType = DataTypeCtd.Boolean,
					Type = TypeSyntax.Constant,
					Value = true
				};
			}
			return new Token
			{
				DataType = DataTypeCtd.Boolean,
				Type = TypeSyntax.Constant,
				Value = false
			};
		}
		XtraMessageBox.Show(ControlFunction.ToString(token.Value), ControlFunction.ToString(token2.Value), MessageBoxButtons.OK);
		return new Token
		{
			Type = TypeSyntax.Constant,
			Value = DBNull.Value,
			DataType = DataTypeCtd.Object
		};
	}

	private void MathDrillDown()
	{
		_index += 2;
		Token token = MathObj();
		_index++;
		Token token2 = MathObj();
		Type type = Type.GetType("Apzon.Forms.Helper.ExtenseionMethods, Apzon.Forms");
		Type[] types = new Type[7]
		{
			typeof(XtraFormEx),
			typeof(ApzObjectTypes),
			typeof(ApzObjectTypes),
			typeof(string),
			typeof(bool),
			typeof(bool),
			typeof(string)
		};
		MethodInfo method = type.GetMethod("DrillDown", types);
		int num = default;
		dynamic val = int.TryParse(Function.ToString(token.Value), out num);
		ApzObjectTypes apzObjectTypes = ((val) ? ((ApzObjectTypes)num) : ApzObjectTypes.oUserDefinedObject);
		method.Invoke(_form, new object[7]
		{
			_form,
			apzObjectTypes,
			apzObjectTypes,
			Function.ToString(token2.Value),
			false,
			false,
			Function.ToString(token.Value)
		});
	}

	private void MathShowStatus(ShowMessageType typeMessage)
	{
		_index += 2;
		Token token = MathObj();
		switch (typeMessage)
		{
		case ShowMessageType.Success:
			WinCommons.Success(Enums.MessageCode.None, (string)Function.ToString(token.Value));
			break;
		case ShowMessageType.Error:
			WinCommons.Error(Enums.MessageCode.None, (string)Function.ToString(token.Value));
			break;
		case ShowMessageType.Warning:
			WinCommons.Warning(Enums.MessageCode.None, (string)Function.ToString(token.Value));
			break;
		case ShowMessageType.Info:
			WinCommons.Info(Enums.MessageCode.None, (string)Function.ToString(token.Value));
			break;
		}
	}

	private void OpenLink()
	{
		_index += 2;
		Token token = MathObj();
		Process.Start((string)(object)Function.ToString(token.Value));
	}

	private void MathFunctionList()
	{
		string name = _syntax[_index].Name.ToLower();
		Token token = _variable.Find((Token v) => v.Name == name);
		string text = _syntax[_index + 2].Name.ToLower();
		_index += 2;
		Token token2 = MathObj();
		if (text == "add")
		{
			if (token.Value == null)
			{
				switch (token.TagetType)
				{
				case DataTypeCtd.Int:
					token.Value = new List<int>();
					break;
				case DataTypeCtd.Double:
					token.Value = new List<double>();
					break;
				case DataTypeCtd.Boolean:
					token.Value = new List<bool>();
					break;
				case DataTypeCtd.String:
					token.Value = new List<string>();
					break;
				}
			}
			token.Value.Add(token2.Value);
		}
		else if (text == "remove")
		{
			token.Value.Remove(token2.Value);
		}
	}

	private void MathIf()
	{
		_index++;
		Token token = MathObj();
		int num;
		if (Function.ParseBool(token.Value))
		{
			_index++;
			MathStatment();
			if (_index < _syntax.Count && _syntax[_index].Type == TypeSyntax.KeyWord && _syntax[_index].Name.Equals("return"))
			{
				return;
			}
			RemoveVariable();
			if (_index >= _syntax.Count - 1 || _syntax[_index].Type != TypeSyntax.KeyWord || !_syntax[_index].Name.ToLower().Equals("else"))
			{
				return;
			}
			_index += 2;
			if (_syntax[_index - 1].Type == TypeSyntax.KeyWord && _syntax[_index - 1].Name.ToLower().Equals("if"))
			{
				while (_syntax[_index].Type != TypeSyntax.Operator || !_syntax[_index].Name.Equals("{"))
				{
					_index++;
				}
				_index++;
			}
			num = 1;
			while (num > 0 && _index < _syntax.Count)
			{
				if (_syntax[_index].Type == TypeSyntax.Operator)
				{
					if (_syntax[_index].Name.Equals("{"))
					{
						num++;
					}
					else if (_syntax[_index].Name.Equals("}"))
					{
						num--;
					}
				}
				_index++;
				if (num != 0 || _index >= _syntax.Count || _syntax[_index].Type != TypeSyntax.KeyWord || !_syntax[_index].Name.Equals("else"))
				{
					continue;
				}
				_index += 2;
				if (_syntax[_index - 1].Type == TypeSyntax.KeyWord && _syntax[_index - 1].Name.ToLower().Equals("if"))
				{
					while (_syntax[_index].Type != TypeSyntax.Operator || !_syntax[_index].Name.Equals("{"))
					{
						_index++;
					}
					_index++;
				}
				num = 1;
			}
			return;
		}
		num = 1;
		_index += 2;
		while (num > 0 && _index < _syntax.Count)
		{
			if (_syntax[_index].Type == TypeSyntax.Operator)
			{
				if (_syntax[_index].Name.Equals("{"))
				{
					num++;
				}
				else if (_syntax[_index].Name.Equals("}"))
				{
					num--;
				}
			}
			_index++;
		}
		if (_index >= _syntax.Count || _syntax[_index].Type != TypeSyntax.KeyWord || !_syntax[_index].Name.ToLower().Equals("else"))
		{
			return;
		}
		if (_syntax[_index + 1].Type == TypeSyntax.KeyWord && _syntax[_index + 1].Name.ToLower().Equals("if"))
		{
			_index++;
			MathIf();
			return;
		}
		_index += 2;
		MathStatment();
		if (_index >= _syntax.Count || _syntax[_index].Type != TypeSyntax.KeyWord || !_syntax[_index].Name.Equals("return"))
		{
			RemoveVariable();
		}
	}

	private void RemoveVariable()
	{
		int num = _variable.Count - 1;
		while (num >= 0 && _variable[num].Name != "0")
		{
			_variable.RemoveAt(num);
			num--;
		}
		_variable.RemoveAt(_variable.Count - 1);
	}

	private Token MathObj(int sourceIndex = -1, DataTypeCtd outputType = DataTypeCtd.None)
	{
		List<Token> listFomular = GetListFomular(sourceIndex, outputType);
		List<Token> list = new List<Token>();
		List<Token> list2 = new List<Token>();
		for (int i = 0; i < listFomular.Count; i++)
		{
			if (listFomular[i].Type != TypeSyntax.Operator && listFomular[i].Type != TypeSyntax.KeyWord)
			{
				list2.Add(listFomular[i]);
				continue;
			}
			if (ConstantsApzCtd.ListOperationBeginFormula.Contains(listFomular[i].Name) && (i == 0 || (listFomular[i - 1].Type == TypeSyntax.Operator && !listFomular[i - 1].Name.Equals(")"))))
			{
				list2.Add(new Token
				{
					Name = "-1",
					DataType = DataTypeCtd.Int,
					Type = TypeSyntax.Constant,
					Value = ((!listFomular[i].Name.Equals("-")) ? 1 : (-1))
				});
				listFomular[i].Name = "*";
			}
			if (list.Count == 0)
			{
				list.Add(listFomular[i]);
			}
			else if (listFomular[i].Type == TypeSyntax.Operator && listFomular[i].Name.Equals(")"))
			{
				int num = list.Count - 1;
				while (list[num].Type == TypeSyntax.Operator && !list[num].Name.Equals("("))
				{
					list2.Add(list[num]);
					list.RemoveAt(num);
					num--;
				}
				list.RemoveAt(num);
			}
			else if (listFomular[i].Type == TypeSyntax.Operator && listFomular[i].Name.Equals(")"))
			{
				list.Add(listFomular[i]);
			}
			else
			{
				while (list.Count > 0 && ControlFunction.CompareOperatorLevel(list[list.Count - 1].Name.ToLower(), listFomular[i].Name.ToLower()))
				{
					list2.Add(list[list.Count - 1]);
					list.RemoveAt(list.Count - 1);
				}
				list.Add(listFomular[i]);
			}
		}
		while (list.Count > 0)
		{
			list2.Add(list[list.Count - 1]);
			list.RemoveAt(list.Count - 1);
		}
		if (list2.Count > 1)
		{
			List<Token> list3 = new List<Token>();
			for (int j = 0; j < list2.Count; j++)
			{
				if (list2[j].Type != TypeSyntax.Operator && list2[j].Type != TypeSyntax.KeyWord)
				{
					list3.Add(list2[j]);
					continue;
				}
				switch (list2[j].Name.ToLower())
				{
				case "+":
					if (list3[list3.Count - 2].DataType == DataTypeCtd.String || list3[list3.Count - 1].DataType == DataTypeCtd.String)
					{
						list3[list3.Count - 2].DataType = DataTypeCtd.String;
						list3[list3.Count - 2].Value = (object)(ControlFunction.ToString(list3[list3.Count - 2].Value) + ControlFunction.ToString(list3[list3.Count - 1].Value));
					}
					else if (list3[list3.Count - 2].DataType == DataTypeCtd.Double || list3[list3.Count - 1].DataType == DataTypeCtd.Double || list3[list3.Count - 2].DataType == DataTypeCtd.Numeric || list3[list3.Count - 1].DataType == DataTypeCtd.Numeric || (list3[list3.Count - 2].DataType == DataTypeCtd.Object && list3[list3.Count - 1].DataType == DataTypeCtd.Object))
					{
						list3[list3.Count - 2].DataType = DataTypeCtd.Double;
						list3[list3.Count - 2].Value = (object)(Function.ParseDouble(list3[list3.Count - 2].Value) + Function.ParseDouble(list3[list3.Count - 1].Value));
					}
					else if (list3[list3.Count - 2].DataType == DataTypeCtd.Int || list3[list3.Count - 1].DataType == DataTypeCtd.Int)
					{
						list3[list3.Count - 2].DataType = DataTypeCtd.Int;
						list3[list3.Count - 2].Value = (object)(Function.ParseInt(list3[list3.Count - 2].Value) + Function.ParseInt(list3[list3.Count - 1].Value));
					}
					else
					{
						list3[list3.Count - 2].DataType = DataTypeCtd.String;
						list3[list3.Count - 2].Value = (object)(ControlFunction.ToString(list3[list3.Count - 2].Value) + ControlFunction.ToString(list3[list3.Count - 1].Value));
					}
					list3.RemoveAt(list3.Count - 1);
					break;
				case "-":
					if (list3.Count > 1)
					{
						if (list3[list3.Count - 2].DataType == DataTypeCtd.Double || list3[list3.Count - 1].DataType == DataTypeCtd.Double || list3[list3.Count - 2].DataType == DataTypeCtd.Numeric || list3[list3.Count - 1].DataType == DataTypeCtd.Numeric || (list3[list3.Count - 2].DataType == DataTypeCtd.Object && list3[list3.Count - 1].DataType == DataTypeCtd.Object))
						{
							list3[list3.Count - 2].DataType = DataTypeCtd.Double;
							list3[list3.Count - 2].Value = (object)(Function.ParseDouble(list3[list3.Count - 2].Value) - Function.ParseDouble(list3[list3.Count - 1].Value));
						}
						else
						{
							list3[list3.Count - 2].DataType = DataTypeCtd.Int;
							list3[list3.Count - 2].Value = (object)(Function.ParseInt(list3[list3.Count - 2].Value) - Function.ParseInt(list3[list3.Count - 1].Value));
						}
						list3.RemoveAt(list3.Count - 1);
					}
					else if (list3.Count == 1)
					{
						if (list3[0].DataType == DataTypeCtd.Double || list3[0].DataType == DataTypeCtd.Numeric || list3[0].DataType == DataTypeCtd.Object)
						{
							list3[0].DataType = DataTypeCtd.Double;
							list3[0].Value = (object)(0 - Function.ParseDouble(list3[0].Value));
						}
						else
						{
							list3[0].DataType = DataTypeCtd.Int;
							list3[0].Value = (object)(0 - Function.ParseInt(list3[0].Value));
						}
					}
					break;
				case "^":
					list3[list3.Count - 2].DataType = DataTypeCtd.Double;
					list3[list3.Count - 2].Value = (object)Math.Pow(Function.ParseDouble(list3[list3.Count - 2].Value), Function.ParseDouble(list3[list3.Count - 1].Value));
					list3.RemoveAt(list3.Count - 1);
					break;
				case "*":
					if (list3[list3.Count - 2].DataType == DataTypeCtd.Double || list3[list3.Count - 1].DataType == DataTypeCtd.Double || list3[list3.Count - 2].DataType == DataTypeCtd.Numeric || list3[list3.Count - 1].DataType == DataTypeCtd.Numeric || (list3[list3.Count - 2].DataType == DataTypeCtd.Object && list3[list3.Count - 1].DataType == DataTypeCtd.Object))
					{
						list3[list3.Count - 2].DataType = DataTypeCtd.Double;
						list3[list3.Count - 2].Value = (object)(Function.ParseDouble(list3[list3.Count - 2].Value) * Function.ParseDouble(list3[list3.Count - 1].Value));
					}
					else
					{
						list3[list3.Count - 2].DataType = DataTypeCtd.Int;
						list3[list3.Count - 2].Value = (object)(Function.ParseInt(list3[list3.Count - 2].Value) * Function.ParseInt(list3[list3.Count - 1].Value));
					}
					list3.RemoveAt(list3.Count - 1);
					break;
				case "/":
					if (list3[list3.Count - 2].DataType == DataTypeCtd.Double || list3[list3.Count - 1].DataType == DataTypeCtd.Double || list3[list3.Count - 2].DataType == DataTypeCtd.Numeric || list3[list3.Count - 1].DataType == DataTypeCtd.Numeric || (list3[list3.Count - 2].DataType == DataTypeCtd.Object && list3[list3.Count - 1].DataType == DataTypeCtd.Object))
					{
						list3[list3.Count - 2].DataType = DataTypeCtd.Double;
						list3[list3.Count - 2].Value = (object)(Function.ParseDouble(list3[list3.Count - 2].Value) / Function.ParseDouble(list3[list3.Count - 1].Value));
					}
					else
					{
						list3[list3.Count - 2].DataType = DataTypeCtd.Int;
						list3[list3.Count - 2].Value = (object)(Function.ParseInt(list3[list3.Count - 2].Value) / Function.ParseInt(list3[list3.Count - 1].Value));
					}
					list3.RemoveAt(list3.Count - 1);
					break;
				case "%":
					list3[list3.Count - 2].DataType = DataTypeCtd.Int;
					list3[list3.Count - 2].Value = (object)(Function.ParseInt(list3[list3.Count - 2].Value) % Function.ParseInt(list3[list3.Count - 1].Value));
					list3.RemoveAt(list3.Count - 1);
					break;
				case ">":
				{
					DataTypeCtd dataType3 = list3[list3.Count - 2].DataType;
					list3[list3.Count - 2].DataType = DataTypeCtd.Boolean;
					if ((dataType3 == DataTypeCtd.DateTime && (list3[list3.Count - 1].DataType == DataTypeCtd.DateTime || list3[list3.Count - 1].DataType == DataTypeCtd.Object)) || (list3[list3.Count - 1].DataType == DataTypeCtd.DateTime && (dataType3 == DataTypeCtd.DateTime || dataType3 == DataTypeCtd.Object)))
					{
						list3[list3.Count - 2].Value = (object)(Function.ParseDateTime(list3[list3.Count - 2].Value) > Function.ParseDateTime(list3[list3.Count - 1].Value));
					}
					else
					{
						list3[list3.Count - 2].Value = (object)(Function.ParseDouble(list3[list3.Count - 2].Value) > Function.ParseDouble(list3[list3.Count - 1].Value));
					}
					list3.RemoveAt(list3.Count - 1);
					break;
				}
				case "<":
				{
					DataTypeCtd dataType2 = list3[list3.Count - 2].DataType;
					list3[list3.Count - 2].DataType = DataTypeCtd.Boolean;
					if ((dataType2 == DataTypeCtd.DateTime && (list3[list3.Count - 1].DataType == DataTypeCtd.DateTime || list3[list3.Count - 1].DataType == DataTypeCtd.Object)) || (list3[list3.Count - 1].DataType == DataTypeCtd.DateTime && (dataType2 == DataTypeCtd.DateTime || dataType2 == DataTypeCtd.Object)))
					{
						list3[list3.Count - 2].Value = (object)(Function.ParseDateTime(list3[list3.Count - 2].Value) < Function.ParseDateTime(list3[list3.Count - 1].Value));
					}
					else
					{
						list3[list3.Count - 2].Value = (object)(Function.ParseDouble(list3[list3.Count - 2].Value) < Function.ParseDouble(list3[list3.Count - 1].Value));
					}
					list3.RemoveAt(list3.Count - 1);
					break;
				}
				case ">=":
				{
					DataTypeCtd dataType4 = list3[list3.Count - 2].DataType;
					list3[list3.Count - 2].DataType = DataTypeCtd.Boolean;
					if ((dataType4 == DataTypeCtd.DateTime && (list3[list3.Count - 1].DataType == DataTypeCtd.DateTime || list3[list3.Count - 1].DataType == DataTypeCtd.Object)) || (list3[list3.Count - 1].DataType == DataTypeCtd.DateTime && (dataType4 == DataTypeCtd.DateTime || dataType4 == DataTypeCtd.Object)))
					{
						list3[list3.Count - 2].Value = (object)(Function.ParseDateTime(list3[list3.Count - 2].Value) >= Function.ParseDateTime(list3[list3.Count - 1].Value));
					}
					else
					{
						list3[list3.Count - 2].Value = (object)(Function.ParseDouble(list3[list3.Count - 2].Value) >= Function.ParseDouble(list3[list3.Count - 1].Value));
					}
					list3.RemoveAt(list3.Count - 1);
					break;
				}
				case "<=":
				{
					DataTypeCtd dataType5 = list3[list3.Count - 2].DataType;
					list3[list3.Count - 2].DataType = DataTypeCtd.Boolean;
					if ((dataType5 == DataTypeCtd.DateTime && (list3[list3.Count - 1].DataType == DataTypeCtd.DateTime || list3[list3.Count - 1].DataType == DataTypeCtd.Object)) || (list3[list3.Count - 1].DataType == DataTypeCtd.DateTime && (dataType5 == DataTypeCtd.DateTime || dataType5 == DataTypeCtd.Object)))
					{
						list3[list3.Count - 2].Value = (object)(Function.ParseDateTime(list3[list3.Count - 2].Value) <= Function.ParseDateTime(list3[list3.Count - 1].Value));
					}
					else
					{
						list3[list3.Count - 2].Value = (object)(Function.ParseDouble(list3[list3.Count - 2].Value) <= Function.ParseDouble(list3[list3.Count - 1].Value));
					}
					list3.RemoveAt(list3.Count - 1);
					break;
				}
				case "<>":
				case "!=":
				{
					DataTypeCtd dataType = list3[list3.Count - 2].DataType;
					list3[list3.Count - 2].DataType = DataTypeCtd.Boolean;
					if ((dataType == DataTypeCtd.DateTime && (list3[list3.Count - 1].DataType == DataTypeCtd.DateTime || list3[list3.Count - 1].DataType == DataTypeCtd.Object)) || (list3[list3.Count - 1].DataType == DataTypeCtd.DateTime && (dataType == DataTypeCtd.DateTime || dataType == DataTypeCtd.Object)))
					{
						list3[list3.Count - 2].Value = (object)(Function.ParseDateTime(list3[list3.Count - 2].Value) != Function.ParseDateTime(list3[list3.Count - 1].Value));
					}
					else if (dataType == DataTypeCtd.String || list3[list3.Count - 1].DataType == DataTypeCtd.String)
					{
						list3[list3.Count - 2].Value = (object)(ControlFunction.ToString(list3[list3.Count - 2].Value) != ControlFunction.ToString(list3[list3.Count - 1].Value));
					}
					else if (dataType == DataTypeCtd.Boolean && list3[list3.Count - 1].DataType == DataTypeCtd.Boolean)
					{
						list3[list3.Count - 2].Value = (object)(ControlFunction.ParseBoolean(list3[list3.Count - 2].Value) != ControlFunction.ParseBoolean(list3[list3.Count - 1].Value));
					}
					else
					{
						list3[list3.Count - 2].Value = (object)(Function.ParseDouble(list3[list3.Count - 2].Value) != Function.ParseDouble(list3[list3.Count - 1].Value));
					}
					list3.RemoveAt(list3.Count - 1);
					break;
				}
				case "==":
				{
					DataTypeCtd dataType6 = list3[list3.Count - 2].DataType;
					list3[list3.Count - 2].DataType = DataTypeCtd.Boolean;
					if ((dataType6 == DataTypeCtd.DateTime && (list3[list3.Count - 1].DataType == DataTypeCtd.DateTime || list3[list3.Count - 1].DataType == DataTypeCtd.Object)) || (list3[list3.Count - 1].DataType == DataTypeCtd.DateTime && (dataType6 == DataTypeCtd.DateTime || dataType6 == DataTypeCtd.Object)))
					{
						list3[list3.Count - 2].Value = (object)(Function.ParseDateTime(list3[list3.Count - 2].Value) == Function.ParseDateTime(list3[list3.Count - 1].Value));
					}
					else if (dataType6 == DataTypeCtd.String || list3[list3.Count - 1].DataType == DataTypeCtd.String)
					{
						list3[list3.Count - 2].Value = (object)(ControlFunction.ToString(list3[list3.Count - 2].Value) == ControlFunction.ToString(list3[list3.Count - 1].Value));
					}
					else if (dataType6 == DataTypeCtd.Boolean && list3[list3.Count - 1].DataType == DataTypeCtd.Boolean)
					{
						list3[list3.Count - 2].Value = (object)(ControlFunction.ParseBoolean(list3[list3.Count - 2].Value) == ControlFunction.ParseBoolean(list3[list3.Count - 1].Value));
					}
					else
					{
						list3[list3.Count - 2].Value = (object)(Function.ParseDouble(list3[list3.Count - 2].Value) == Function.ParseDouble(list3[list3.Count - 1].Value));
					}
					list3.RemoveAt(list3.Count - 1);
					break;
				}
				case "and":
					list3[list3.Count - 2].DataType = DataTypeCtd.Boolean;
					list3[list3.Count - 2].Value = (bool)list3[list3.Count - 2].Value && (bool)list3[list3.Count - 1].Value;
					list3.RemoveAt(list3.Count - 1);
					break;
				case "or":
					list3[list3.Count - 2].DataType = DataTypeCtd.Boolean;
					list3[list3.Count - 2].Value = (bool)list3[list3.Count - 2].Value || (bool)list3[list3.Count - 1].Value;
					list3.RemoveAt(list3.Count - 1);
					break;
				}
			}
			return list3[0];
		}
		return list2[0];
	}

	private List<Token> GetListFomular(int sourceIndex, DataTypeCtd outputType = DataTypeCtd.None)
	{
		List<Token> list = new List<Token>();
		int num = 0;
		while (_index < _syntax.Count)
		{
			if (_syntax[_index].Type == TypeSyntax.Operator && ConstantsApzCtd.ListOperatorEndStatment.Contains(_syntax[_index].Name))
			{
				if (!")".Contains(_syntax[_index].Name) || num == 0)
				{
					break;
				}
				list.Add(new Token
				{
					DataType = _syntax[_index].DataType,
					Name = _syntax[_index].Name,
					TagetType = _syntax[_index].TagetType,
					Type = _syntax[_index].Type,
					Value = _syntax[_index].Value
				});
				num--;
				_index++;
				continue;
			}
			if (_syntax[_index].Type == TypeSyntax.Operator && "(".Equals(_syntax[_index].Name))
			{
				num++;
			}
			else if (ConstantsApzCtd.HighlightGlobal.Contains(_syntax[_index].Name))
			{
				list.Add(new Token
				{
					Type = _syntax[_index].Type,
					DataType = _syntax[_index].DataType,
					Value = ControlFunction.GetValueSystemByName(_syntax[_index].Name, _rowHandle)
				});
				_index++;
				continue;
			}
			if (_syntax[_index].Type == TypeSyntax.Variable)
			{
				string name = _syntax[_index].Name.ToLower();
				Token token = _variable.Find((Token t) => t.Name == name);
				if (token.DataType == DataTypeCtd.DataRow)
				{
					_index++;
					Token item = ParseGetDataRow(token.Value);
					list.Add(item);
				}
				else if (token.DataType == DataTypeCtd.DataTable)
				{
					_index++;
					Token item = ParseGetDataDatatable(token.Value);
					list.Add(item);
				}
				else if (token.DataType == DataTypeCtd.DataSet)
				{
					_index++;
					Token item = ParseGetDataDataset(token.Value);
					list.Add(item);
				}
				else if (token.DataType == DataTypeCtd.HttpResult)
				{
					HttpResult httpResult = token.Value as HttpResult;
					_index++;
					Token item;
					if (_syntax[_index].Type == TypeSyntax.Operator && _syntax[_index].Name.Equals("."))
					{
						_index++;
						item = ((!_syntax[_index].Name.Equals("message")) ? new Token
						{
							Name = Function.ToString(_index),
							Type = TypeSyntax.Constant,
							DataType = DataTypeCtd.Object,
							Value = httpResult.Content
						} : new Token
						{
							Name = Function.ToString(_index),
							Type = TypeSyntax.Constant,
							DataType = DataTypeCtd.String,
							Value = httpResult.Message
						});
					}
					else
					{
						item = new Token
						{
							Name = token.Name,
							Type = token.Type,
							DataType = token.DataType,
							Value = token.Value
						};
					}
					_index++;
					list.Add(item);
				}
				else if (token.DataType == DataTypeCtd.Form || token.DataType == DataTypeCtd.Dialog)
				{
					XtraFormEx xtraFormEx = token.Value as XtraFormEx;
					_index++;
					Token item;
					if (_syntax[_index].Type == TypeSyntax.Operator && _syntax[_index].Name.Equals("."))
					{
						_index++;
						if (!_syntax[_index].Name.Equals("dialogresult"))
						{
							_index++;
							item = ParseGetDataDataset(xtraFormEx.DataSet());
							list.Add(item);
							continue;
						}
						item = new Token
						{
							Name = Function.ToString(_index),
							Type = TypeSyntax.Constant,
							DataType = DataTypeCtd.String,
							Value = Function.ToString(xtraFormEx.DialogResult)
						};
					}
					else
					{
						item = new Token
						{
							Name = token.Name,
							Type = token.Type,
							DataType = token.DataType,
							Value = token.Value
						};
					}
					_index++;
					list.Add(item);
				}
				else if (token.DataType == DataTypeCtd.List)
				{
					if (_index >= 2 && _syntax[_index + 1].Type == TypeSyntax.Operator && _syntax[_index + 1].Name.ToLower().Equals(".") && _syntax[_index + 2].Type == TypeSyntax.KeyWord && _syntax[_index + 2].Name.ToLower().Equals("count"))
					{
						list.Add(new Token
						{
							Type = TypeSyntax.Constant,
							DataType = DataTypeCtd.Int,
							Value = ((token.Value != null) ? token.Value.Count : ((object)0))
						});
						_index += 2;
						continue;
					}
					if (_index >= 2 && _syntax[_index + 1].Type == TypeSyntax.Operator && (_syntax[_index + 1].Name.ToLower().Equals(",") || _syntax[_index + 1].Name.ToLower().Equals(";")))
					{
						list.Add(new Token
						{
							Type = TypeSyntax.Constant,
							DataType = DataTypeCtd.List,
							Value = token.Value
						});
						_index++;
						continue;
					}
					_index += 2;
					Token item = MathObj();
					if (_syntax[_index + 1].Type != TypeSyntax.Operator || !_syntax[_index + 1].Name.Equals("."))
					{
						list.Add(new Token
						{
							DataType = token.TagetType,
							Type = TypeSyntax.Constant,
							Value = token.Value[Function.ParseInt(item.Value)]
						});
					}
					else
					{
						_index += 2;
						if (_syntax[_index].Type == TypeSyntax.KeyWord)
						{
							switch (_syntax[_index].Name.ToLower())
							{
							case "string":
								list.Add(new Token
								{
									DataType = DataTypeCtd.String,
									Type = TypeSyntax.Constant,
									Value = Function.ToString(token.Value[Function.ParseInt(item.Value)])
								});
								break;
							case "int":
								list.Add(new Token
								{
									DataType = DataTypeCtd.Int,
									Type = TypeSyntax.Constant,
									Value = Function.ParseInt(token.Value[Function.ParseInt(item.Value)])
								});
								break;
							case "double":
								list.Add(new Token
								{
									DataType = DataTypeCtd.Double,
									Type = TypeSyntax.Constant,
									Value = Function.ParseDouble(token.Value[Function.ParseInt(item.Value)])
								});
								break;
							case "bool":
								list.Add(new Token
								{
									DataType = DataTypeCtd.Boolean,
									Type = TypeSyntax.Constant,
									Value = Function.ParseBool(token.Value[Function.ParseInt(item.Value)])
								});
								break;
							case "datetime":
								list.Add(new Token
								{
									DataType = DataTypeCtd.DateTime,
									Type = TypeSyntax.Constant,
									Value = Function.ParseDateTime(token.Value[Function.ParseInt(item.Value)])
								});
								break;
							case "date":
								list.Add(new Token
								{
									DataType = DataTypeCtd.DateTime,
									Type = TypeSyntax.Constant,
									Value = Function.ParseDateTime(token.Value[Function.ParseInt(item.Value)])
								});
								break;
							}
						}
					}
					_index++;
				}
				else
				{
					list.Add(new Token
					{
						Name = token.Name,
						Type = token.Type,
						DataType = token.DataType,
						Value = token.Value
					});
					_index++;
				}
				continue;
			}
			if (_syntax[_index].Type == TypeSyntax.Operator && _syntax[_index].Name.Equals(ControlFunction.ToString("$")))
			{
				Token item2 = ParseGetValueDataItem(sourceIndex);
				list.Add(item2);
				continue;
			}
			if (_syntax[_index].Type == TypeSyntax.KeyWord)
			{
				string text = _syntax[_index].Name.ToLower();
				if (text.Equals("abs") || text.Equals("sqrt"))
				{
					_index += 2;
					Token item = MathObj();
					if (text.Equals("abs"))
					{
						if (item.DataType == DataTypeCtd.Double || item.DataType == DataTypeCtd.Numeric)
						{
							list.Add(new Token
							{
								Type = TypeSyntax.Constant,
								DataType = DataTypeCtd.Double,
								Value = Math.Abs(Function.ParseDouble(item.Value))
							});
						}
						else
						{
							list.Add(new Token
							{
								Type = TypeSyntax.Constant,
								DataType = DataTypeCtd.Int,
								Value = Math.Abs(Function.ParseInt(item.Value))
							});
						}
					}
					else
					{
						list.Add(new Token
						{
							Type = TypeSyntax.Constant,
							DataType = DataTypeCtd.Double,
							Value = Math.Sqrt(Function.ParseDouble(item.Value))
						});
					}
					_index++;
					continue;
				}
				if (text.Equals("tolower") || text.Equals("toupper") || text.Equals("trim"))
				{
					_index += 2;
					Token item = MathObj();
					_index++;
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.String,
						Value = (text.Equals("tolower") ? ControlFunction.ToString(item.Value).ToLower() : (text.Equals("toupper") ? ControlFunction.ToString(item.Value).ToUpper() : ControlFunction.ToString(item.Value).Trim()))
					});
					continue;
				}
				if (text.Equals("format"))
				{
					_index += 2;
					Token item = MathObj();
					_index++;
					Token token2 = MathObj();
					_index++;
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.String,
						Value = ControlFunction.Format(item.Value, Function.ToString(token2.Value))
					});
					continue;
				}
				if (text.Equals("substring"))
				{
					_index += 2;
					Token item = MathObj();
					_index++;
					Token token3 = MathObj();
					_index++;
					Token token4 = MathObj();
					_index++;
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.String,
						Value = ControlFunction.ToString(item.Value).Substring(Function.ParseInt(token3.Value), Function.ParseInt(token4.Value))
					});
					continue;
				}
				if (text.Equals("replace"))
				{
					_index += 2;
					Token item = MathObj();
					_index++;
					Token token5 = MathObj();
					_index++;
					Token token6 = MathObj();
					_index++;
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.String,
						Value = ControlFunction.ToString(item.Value).Replace(ControlFunction.ToString(token5.Value), ControlFunction.ToString(token6.Value))
					});
					continue;
				}
				if (text.Equals("split"))
				{
					_index += 2;
					Token item = MathObj();
					_index++;
					Token token7 = MathObj();
					_index++;
					string[] collection = item.Value.ToString().Split(new string[1] { token7.Value.ToString() }, StringSplitOptions.None);
					List<string> list2 = new List<string>();
					list2.AddRange(collection);
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.List,
						TagetType = DataTypeCtd.Object,
						Value = list2
					});
					continue;
				}
				if (text.Equals("firstindexstring"))
				{
					_index += 2;
					Token item = MathObj();
					_index++;
					Token token8 = MathObj();
					_index++;
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.Int,
						Value = ControlFunction.ToString(item.Value).IndexOf(ControlFunction.ToString(token8.Value))
					});
					continue;
				}
				if (text.Equals("round"))
				{
					_index += 2;
					Token item = MathObj();
					if (_syntax[_index].Type == TypeSyntax.Operator && _syntax[_index].Name.Equals(")"))
					{
						list.Add(new Token
						{
							Type = TypeSyntax.Constant,
							DataType = DataTypeCtd.Double,
							Value = Math.Round(Function.ParseDouble(item.Value), MidpointRounding.AwayFromZero)
						});
					}
					else
					{
						_index++;
						Token token9 = MathObj();
						_index++;
						list.Add(new Token
						{
							Type = TypeSyntax.Constant,
							DataType = DataTypeCtd.Double,
							Value = Math.Round(Function.ParseDouble(item.Value), Function.ParseInt(token9.Value), MidpointRounding.AwayFromZero)
						});
					}
					continue;
				}
				if (text.Equals("dateaddday") || text.Equals("dateaddmonth") || text.Equals("dateaddyear"))
				{
					_index += 2;
					Token item = MathObj();
					_index++;
					Token token10 = MathObj();
					_index++;
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.DateTime,
						Value = (text.Equals("dateaddday") ? ((DateTime)Function.ParseDateTimes(item.Value)).AddDays(Function.ParseInt(token10.Value)) : (text.Equals("dateaddmonth") ? ((DateTime)Function.ParseDateTimes(item.Value)).AddMonths(Function.ParseInt(token10.Value)) : ((DateTime)Function.ParseDateTimes(item.Value)).AddYears(Function.ParseInt(token10.Value))))
					});
					continue;
				}
				if (text.Equals("dategetday") || text.Equals("dategetmonth") || text.Equals("dategetyear"))
				{
					_index += 2;
					Token item = MathObj();
					_index++;
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.Int,
						Value = (text.Equals("dategetday") ? ((DateTime)Function.ParseDateTimes(item.Value)).Day : (text.Equals("dategetmonth") ? ((DateTime)Function.ParseDateTimes(item.Value)).Month : ((DateTime)Function.ParseDateTimes(item.Value)).Year))
					});
					continue;
				}
				if (text.Equals("datetostring"))
				{
					_index += 2;
					Token item = MathObj();
					if (_syntax[_index].Type == TypeSyntax.Operator && _syntax[_index].Name.Equals(")"))
					{
						_index++;
						list.Add(new Token
						{
							Type = TypeSyntax.Constant,
							DataType = DataTypeCtd.String,
							Value = ((DateTime)Function.ParseDateTimes(item.Value)).ToString(CultureInfo.InvariantCulture)
						});
					}
					else
					{
						_index++;
						Token token11 = MathObj();
						_index++;
						list.Add(new Token
						{
							Type = TypeSyntax.Constant,
							DataType = DataTypeCtd.String,
							Value = ((DateTime)Function.ParseDateTimes(item.Value)).ToString(ControlFunction.ToString(token11.Value))
						});
					}
					continue;
				}
				if (text.Equals("getdata"))
				{
					Token item = MathGetData();
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.Boolean,
						Value = item.Value
					});
					_index++;
					continue;
				}
				if (text.Equals("execute"))
				{
					Token item = MathExecute(outputType);
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = item.DataType,
						Value = item.Value
					});
					_index++;
					continue;
				}
				if (text.Equals("invoke"))
				{
					Token item = MathInvokMethod();
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.HttpResult,
						Value = item.Value
					});
					_index++;
					continue;
				}
				if (text.Equals("mergepdf"))
				{
					Token item = MathMergePDF();
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.String,
						Value = item.Value
					});
					_index++;
					continue;
				}
				if (text.Equals("processsapobject"))
				{
					Token item = MathProcessSAPObject();
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.HttpResult,
						Value = item.Value
					});
					_index++;
					continue;
				}
				if (text.Equals("showform") || text.Equals("showdialog"))
				{
					Token item = MathShowFormAndShowDialogMethod(text.Equals("showdialog"));
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = (text.Equals("showdialog") ? DataTypeCtd.Dialog : DataTypeCtd.Form),
						Value = item.Value
					});
					_index++;
					continue;
				}
				if (text.Equals("showmessagebox"))
				{
					Token item = MathShowMessageBox();
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.Boolean,
						Value = item.Value
					});
					_index++;
					continue;
				}
				if (text.Equals("getdate"))
				{
					_index += 3;
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.DateTime,
						Value = DateTime.Now
					});
					continue;
				}
				if (text.Equals("length"))
				{
					_index += 2;
					Token item = MathObj();
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.Int,
						Value = ((item.DataType != DataTypeCtd.List) ? ControlFunction.ToString(item.Value).Length : item.Value.Count)
					});
					_index++;
					continue;
				}
				if (new string[5] { "totaldays", "totalhours", "totalminutes", "totalseconds", "totalmilliseconds" }.Contains(text))
				{
					_index += 2;
					Token token12 = MathObj();
					_index++;
					Token token13 = MathObj();
					_index++;
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = DataTypeCtd.Double,
						Value = ("totaldays".Equals(text) ? (Function.ParseDateTimes(token12.Value).Date - Function.ParseDateTimes(token13.Value).Date).TotalDays : ("totalhours".Equals(text) ? (Function.ParseDateTimes(token12.Value) - Function.ParseDateTimes(token13.Value)).TotalHours : ("totalminutes".Equals(text) ? (Function.ParseDateTimes(token12.Value) - Function.ParseDateTimes(token13.Value)).TotalMinutes : ("totalseconds".Equals(text) ? (Function.ParseDateTimes(token12.Value) - Function.ParseDateTimes(token13.Value)).TotalSeconds : (Function.ParseDateTimes(token12.Value) - Function.ParseDateTimes(token13.Value)).TotalMilliseconds))))
					});
					continue;
				}
				if (text.Equals("dayofweek") || text.Equals("dayofyear"))
				{
					_index += 2;
					Token item = MathObj();
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = (text.Equals("dayofyear") ? DataTypeCtd.Int : DataTypeCtd.String),
						Value = (text.Equals("dayofyear") ? Function.ParseDateTimes(item.Value).DayOfYear : Function.ToString(Function.ParseDateTimes(item.Value).DayOfWeek))
					});
					_index++;
					continue;
				}
			}
			list.Add(new Token
			{
				DataType = _syntax[_index].DataType,
				Name = _syntax[_index].Name,
				TagetType = _syntax[_index].TagetType,
				Type = _syntax[_index].Type,
				Value = _syntax[_index].Value
			});
			_index++;
		}
		return list;
	}

	private Token ParseGetDataDataset(DataSet sourceValue)
	{
		if (sourceValue == null || sourceValue.Tables.Count == 0)
		{
			return new Token
			{
				Type = TypeSyntax.Constant,
				Value = DBNull.Value,
				DataType = DataTypeCtd.Object
			};
		}
		if (!_syntax[_index].Name.Equals("."))
		{
			return new Token
			{
				Value = sourceValue.Copy(),
				DataType = DataTypeCtd.DataSet,
				Type = TypeSyntax.Constant
			};
		}
		if (_syntax[_index + 1].Name.ToLower().Equals("table"))
		{
			if (_syntax[_index + 3].Name.ToLower().Equals("count"))
			{
				_index += 4;
				return new Token
				{
					DataType = DataTypeCtd.Int,
					Type = TypeSyntax.Constant,
					Value = sourceValue.Tables.Count
				};
			}
			return new Token
			{
				Type = TypeSyntax.Constant,
				Value = DBNull.Value,
				DataType = DataTypeCtd.Object
			};
		}
		if (_syntax[_index + 1].Name.Equals("["))
		{
			_index += 2;
			Token token = MathObj();
			_index++;
			if (token.DataType == DataTypeCtd.String)
			{
				dynamic val = Function.ToString(token.Value);
				if (sourceValue.Tables.Contains(val))
				{
					return ParseGetDataDatatable(sourceValue.Tables[val]);
				}
				return new Token
				{
					Type = TypeSyntax.Constant,
					Value = DBNull.Value,
					DataType = DataTypeCtd.Object
				};
			}
			if (sourceValue.Tables.Count <= Function.ParseInt(token.Value))
			{
				return new Token
				{
					Type = TypeSyntax.Constant,
					Value = DBNull.Value,
					DataType = DataTypeCtd.Object
				};
			}
			return ParseGetDataDatatable(sourceValue.Tables[Function.ParseInt(token.Value)]);
		}
		return new Token
		{
			Type = TypeSyntax.Constant,
			Value = DBNull.Value,
			DataType = DataTypeCtd.Object
		};
	}

	private Token ParseGetDataDatatable(DataTable sourceData)
	{
		if (sourceData == null || sourceData.Columns.Count == 0)
		{
			return new Token
			{
				Type = TypeSyntax.Constant,
				Value = DBNull.Value,
				DataType = DataTypeCtd.Object
			};
		}
		if (!_syntax[_index].Name.Equals("."))
		{
			return new Token
			{
				Value = sourceData.Copy(),
				DataType = DataTypeCtd.DataTable,
				Type = TypeSyntax.Constant
			};
		}
		if (_syntax[_index + 1].Name.ToLower().Equals("rows") && _syntax[_index + 3].Name.ToLower().Equals("count"))
		{
			_index += 4;
			return new Token
			{
				DataType = DataTypeCtd.Int,
				Type = TypeSyntax.Constant,
				Value = ControlFunction.ToString(sourceData.Rows.Count)
			};
		}
		if (_syntax[_index + 1].Name.ToLower().Equals("columns"))
		{
			if (_syntax[_index + 3].Name.ToLower().Equals("count"))
			{
				_index += 4;
				return new Token
				{
					DataType = DataTypeCtd.Int,
					Type = TypeSyntax.Constant,
					Value = ControlFunction.ToString(sourceData.Columns.Count)
				};
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("contains"))
			{
				_index += 4;
				Token token = MathObj();
				Token token2 = new Token();
				token2.Type = TypeSyntax.Constant;
				token2.Value = (object)sourceData.Columns.Contains(Function.ToString(token.Value));
				token2.DataType = DataTypeCtd.Boolean;
				return token2;
			}
		}
		if (_syntax[_index + 1].Name.ToLower().Equals("first"))
		{
			_index += 3;
			List<DataRow> source = sourceData.AsEnumerable().ToList();
			Token fieldName = MathObj();
			_index++;
			Token condition = MathObj();
			if (condition.DataType == DataTypeCtd.Int)
			{
				source = source.Where((DataRow t) => Function.ParseInt(t[fieldName.Value]) == Function.ParseInt(condition.Value)).ToList();
			}
			else
			{
				source = ((condition.DataType != DataTypeCtd.Double) ? source.Where((DataRow t) => Function.ToString(t[fieldName.Value]) == Function.ToString(condition.Value)).ToList() : source.Where((DataRow t) => Function.ParseDouble(t[fieldName.Value]) == Function.ParseDouble(condition.Value)).ToList());
			}
			_index++;
			return new Token
			{
				Type = TypeSyntax.Constant,
				Value = source.FirstOrDefault(),
				DataType = DataTypeCtd.DataRow
			};
		}
		if (_syntax[_index + 1].Name.ToLower().Equals("firstduplicate"))
		{
			_index += 3;
			string text = Function.ToString(MathObj().Value);
			string[] group = (from x in text.Split(',')
				select x.Trim()).ToArray();
			int num = -1;
			DataRow row = sourceData.NewRow();
			if (group.Length == 1)
			{
				row = (from r in sourceData.AsEnumerable()
					group r by new
					{
						Col1 = r[@group[0]]
					} into g
					where g.Count() > 1
					select g.FirstOrDefault()).FirstOrDefault();
			}
			else if (group.Length == 2)
			{
				row = (from r in sourceData.AsEnumerable()
					group r by new
					{
						Col1 = r[@group[0]],
						Col2 = r[@group[1]]
					} into g
					where g.Count() > 1
					select g.FirstOrDefault()).FirstOrDefault();
			}
			else if (group.Length == 3)
			{
				row = (from r in sourceData.AsEnumerable()
					group r by new
					{
						Col1 = r[@group[0]],
						Col2 = r[@group[1]],
						Col3 = r[@group[2]]
					} into g
					where g.Count() > 1
					select g.FirstOrDefault()).FirstOrDefault();
			}
			else if (group.Length == 4)
			{
				row = (from r in sourceData.AsEnumerable()
					group r by new
					{
						Col1 = r[@group[0]],
						Col2 = r[@group[1]],
						Col3 = r[@group[2]],
						Col4 = r[@group[3]]
					} into g
					where g.Count() > 1
					select g.FirstOrDefault()).FirstOrDefault();
			}
			else if (group.Length == 5)
			{
				row = (from r in sourceData.AsEnumerable()
					group r by new
					{
						Col1 = r[@group[0]],
						Col2 = r[@group[1]],
						Col3 = r[@group[2]],
						Col4 = r[@group[3]],
						Col5 = r[@group[4]]
					} into g
					where g.Count() > 1
					select g.FirstOrDefault()).FirstOrDefault();
			}
			num = sourceData.Rows.IndexOf(row);
			return new Token
			{
				Type = TypeSyntax.Constant,
				Value = num,
				DataType = DataTypeCtd.Int
			};
		}
		if (_syntax[_index + 1].Name.ToLower().Equals("stringagg"))
		{
			_index += 3;
			string fieldName2 = Function.ToString(MathObj().Value);
			_index++;
			string separator = Function.ToString(MathObj().Value);
			EnumerableRowCollection<string> values = from r in sourceData.AsEnumerable()
				select Function.ToString(r[fieldName2]);
			string value = string.Join(separator, values);
			_index++;
			return new Token
			{
				Type = TypeSyntax.Variable,
				Value = value,
				DataType = DataTypeCtd.String
			};
		}
		if (_syntax[_index + 1].Name.Equals("["))
		{
			_index += 2;
			Token token3 = MathObj();
			_index++;
			if (sourceData.Rows.Count <= Function.ParseInt(token3.Value))
			{
				return new Token
				{
					Type = TypeSyntax.Constant,
					Value = DBNull.Value,
					DataType = DataTypeCtd.Object
				};
			}
			return ParseGetDataRow(sourceData.Rows[Function.ParseInt(token3.Value)]);
		}
		return new Token
		{
			Type = TypeSyntax.Constant,
			Value = DBNull.Value,
			DataType = DataTypeCtd.Object
		};
	}

	private Token ParseGetDataRow(DataRow sourceData)
	{
		if (sourceData == null || sourceData.Table.Columns.Count == 0)
		{
			return new Token
			{
				Type = TypeSyntax.Constant,
				Value = DBNull.Value,
				DataType = DataTypeCtd.Object
			};
		}
		_index += 2;
		Token token = MathObj();
		_index++;
		int num = -1;
		if (token.DataType == DataTypeCtd.Int)
		{
			num = Function.ParseInt(token.Value);
		}
		else if (sourceData.Table.Columns.Contains(Function.ToString(token.Value)))
		{
			num = sourceData.Table.Columns[Function.ToString(token.Value)].Ordinal;
		}
		if (num < 0 || num > sourceData.Table.Columns.Count)
		{
			WinCommons.Error(Enums.MessageCode.ColumnFieldNotBelongTable, "");
			return new Token
			{
				Type = TypeSyntax.Constant,
				Value = DBNull.Value,
				DataType = DataTypeCtd.Object
			};
		}
		if (!_syntax[_index].Name.Equals("."))
		{
			return new Token
			{
				DataType = DataTypeCtd.Object,
				Type = TypeSyntax.Constant,
				Value = sourceData[num]
			};
		}
		_index += 2;
		return _syntax[_index - 1].Name.ToLower() switch
		{
			"int" => new Token
			{
				DataType = DataTypeCtd.Int,
				Type = TypeSyntax.Constant,
				Value = sourceData[num]
			}, 
			"double" => new Token
			{
				DataType = DataTypeCtd.Double,
				Type = TypeSyntax.Constant,
				Value = sourceData[num]
			}, 
			"string" => new Token
			{
				DataType = DataTypeCtd.String,
				Type = TypeSyntax.Constant,
				Value = sourceData[num]
			}, 
			"bool" => new Token
			{
				DataType = DataTypeCtd.Boolean,
				Type = TypeSyntax.Constant,
				Value = sourceData[num]
			}, 
			"date" => new Token
			{
				DataType = DataTypeCtd.DateTime,
				Type = TypeSyntax.Constant,
				Value = Function.ParseDateTime(sourceData[num])
			}, 
			"datetime" => new Token
			{
				DataType = DataTypeCtd.DateTime,
				Type = TypeSyntax.Constant,
				Value = Function.ParseDateTime(sourceData[num])
			}, 
			_ => new Token
			{
				Type = TypeSyntax.Constant,
				Value = DBNull.Value,
				DataType = DataTypeCtd.Object
			}, 
		};
	}

	private Token ParseGetValueDataItem(int sourceIndex)
	{
		Token token = new Token();
		_index += 2;
		if (_syntax[_index].Type == TypeSyntax.Constant)
		{
			token = _syntax[_index++];
		}
		else
		{
			token.Type = TypeSyntax.Variable;
			while (_index < _syntax.Count && !_syntax[_index].Name.Equals("]"))
			{
				token.Name += _syntax[_index++].Name;
			}
		}
		if (_form.FormId.ToLower().Equals(token.Name.ToLower()))
		{
			if (_syntax[_index + 2].Type == TypeSyntax.KeyWord)
			{
				_index += 2;
				return new Token
				{
					DataType = DataTypeCtd.String,
					Type = TypeSyntax.Constant,
					Value = _columnEditFieldName
				};
			}
			_index += 3;
			if (_syntax[_index].Type == TypeSyntax.Constant)
			{
				string name = _syntax[_index].Name;
				if (_syntax[_index + 2].Type != TypeSyntax.Operator || !_syntax[_index + 2].Name.Equals("."))
				{
					_index += 2;
					return new Token
					{
						DataType = DataTypeCtd.Object,
						Type = TypeSyntax.Constant,
						Value = _form.GetDataFormSourceControl(name)
					};
				}
				_index++;
				if (_syntax[_index + 2].Type == TypeSyntax.KeyWord)
				{
					if (_syntax[_index + 2].Name.Equals("string"))
					{
						_index += 3;
						return new Token
						{
							DataType = DataTypeCtd.String,
							Type = TypeSyntax.Constant,
							Value = _form.GetDataFormSourceControl(name)
						};
					}
					if (_syntax[_index + 2].Name.Equals("int"))
					{
						_index += 3;
						return new Token
						{
							DataType = DataTypeCtd.Int,
							Type = TypeSyntax.Constant,
							Value = _form.GetDataFormSourceControl(name)
						};
					}
					if (_syntax[_index + 2].Name.Equals("double"))
					{
						_index += 3;
						return new Token
						{
							DataType = DataTypeCtd.Double,
							Type = TypeSyntax.Constant,
							Value = _form.GetDataFormSourceControl(name)
						};
					}
					if (_syntax[_index + 2].Name.Equals("bool"))
					{
						_index += 3;
						return new Token
						{
							DataType = DataTypeCtd.Boolean,
							Type = TypeSyntax.Constant,
							Value = _form.GetDataFormSourceControl(name)
						};
					}
					if (_syntax[_index + 2].Name.Equals("datetime"))
					{
						_index += 3;
						return new Token
						{
							DataType = DataTypeCtd.DateTime,
							Type = TypeSyntax.Constant,
							Value = Function.ParseDateTime(_form.GetDataFormSourceControl(name))
						};
					}
					if (_syntax[_index + 2].Name.Equals("date"))
					{
						_index += 3;
						return new Token
						{
							DataType = DataTypeCtd.DateTime,
							Type = TypeSyntax.Constant,
							Value = Function.ParseDateTime(_form.GetDataFormSourceControl(name))
						};
					}
					return new Token
					{
						Type = TypeSyntax.Constant,
						Value = DBNull.Value,
						DataType = DataTypeCtd.Object
					};
				}
			}
			else
			{
				token.Type = TypeSyntax.Variable;
				while (_index < _syntax.Count && !_syntax[_index].Name.Equals("]"))
				{
					token.Name += _syntax[_index++].Name;
				}
			}
		}
		_index--;
		ViewEditModel viewEditModel = ControlFunction.CheckExistGrid(_lstGrid, token.Name);
		DataTable dataSource = viewEditModel.DataSource;
		if (_syntax[_index + 2].Type != TypeSyntax.Operator || !_syntax[_index + 2].Name.Equals("."))
		{
			_index += 2;
			return new Token
			{
				DataType = DataTypeCtd.DataTable,
				Type = TypeSyntax.Constant,
				Value = dataSource
			};
		}
		if (_syntax[_index + 3].Type == TypeSyntax.KeyWord)
		{
			if (_syntax[_index + 3].Name.ToLower().Equals("newrow"))
			{
				DataTable dataSource2 = viewEditModel.DataSource;
				DataRow row = dataSource2.NewRow();
				dataSource2.Rows.Add(row);
				dataSource2.AcceptChanges();
				_index += 6;
				return new Token
				{
					DataType = DataTypeCtd.Int,
					Type = TypeSyntax.Constant,
					Value = viewEditModel.GetRowIndex(dataSource2.Rows.Count - 1)
				};
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("focusrow"))
			{
				_index += 4;
				return new Token
				{
					DataType = DataTypeCtd.Int,
					Type = TypeSyntax.Constant,
					Value = viewEditModel.GetFocusedRowHandle()
				};
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("columnedit"))
			{
				_index += 4;
				return new Token
				{
					DataType = DataTypeCtd.String,
					Type = TypeSyntax.Constant,
					Value = _columnEditFieldName
				};
			}
			string text = _syntax[_index + 3].Name.ToLower();
			if ((text.Equals("rows") || text.Equals("columns")) && _syntax[_index + 5].Type == TypeSyntax.KeyWord && _syntax[_index + 5].Name.ToLower().Equals("count"))
			{
				_index += 6;
				if (!text.Equals("rows"))
				{
					return new Token
					{
						DataType = DataTypeCtd.Int,
						Type = TypeSyntax.Constant,
						Value = dataSource.Columns.Count
					};
				}
				return new Token
				{
					DataType = DataTypeCtd.Int,
					Type = TypeSyntax.Constant,
					Value = viewEditModel.RowCount()
				};
			}
			if (text.Equals("columns") && _syntax[_index + 5].Type == TypeSyntax.KeyWord && _syntax[_index + 5].Name.ToLower().Equals("contains"))
			{
				_index += 7;
				Token token2 = MathObj();
				Token token3 = new Token();
				token3.Type = TypeSyntax.Constant;
				token3.Value = (object)dataSource.Columns.Contains(Function.ToString(token2.Value));
				token3.DataType = DataTypeCtd.Boolean;
				return token3;
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("selectedrows"))
			{
				_index += 4;
				return new Token
				{
					DataType = DataTypeCtd.DataTable,
					Type = TypeSyntax.Constant,
					Value = viewEditModel.GetSelectedRowIndex()
				};
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("firstduplicate"))
			{
				_index += 5;
				string text2 = Function.ToString(MathObj().Value);
				string[] group = (from x in text2.Split(',')
					select x.Trim()).ToArray();
				int num = -1;
				DataRow row2 = dataSource.NewRow();
				if (group.Length == 1)
				{
					row2 = (from r in dataSource.AsEnumerable()
						group r by new
						{
							Col1 = r[@group[0]]
						} into g
						where g.Count() > 1
						select g.FirstOrDefault()).FirstOrDefault();
				}
				else if (group.Length == 2)
				{
					row2 = (from r in dataSource.AsEnumerable()
						group r by new
						{
							Col1 = r[@group[0]],
							Col2 = r[@group[1]]
						} into g
						where g.Count() > 1
						select g.FirstOrDefault()).FirstOrDefault();
				}
				else if (group.Length == 3)
				{
					row2 = (from r in dataSource.AsEnumerable()
						group r by new
						{
							Col1 = r[@group[0]],
							Col2 = r[@group[1]],
							Col3 = r[@group[2]]
						} into g
						where g.Count() > 1
						select g.FirstOrDefault()).FirstOrDefault();
				}
				else if (group.Length == 4)
				{
					row2 = (from r in dataSource.AsEnumerable()
						group r by new
						{
							Col1 = r[@group[0]],
							Col2 = r[@group[1]],
							Col3 = r[@group[2]],
							Col4 = r[@group[3]]
						} into g
						where g.Count() > 1
						select g.FirstOrDefault()).FirstOrDefault();
				}
				else if (group.Length == 5)
				{
					row2 = (from r in dataSource.AsEnumerable()
						group r by new
						{
							Col1 = r[@group[0]],
							Col2 = r[@group[1]],
							Col3 = r[@group[2]],
							Col4 = r[@group[3]],
							Col5 = r[@group[4]]
						} into g
						where g.Count() > 1
						select g.FirstOrDefault()).FirstOrDefault();
				}
				num = dataSource.Rows.IndexOf(row2);
				num = viewEditModel.GetRowIndex(num);
				return new Token
				{
					DataType = DataTypeCtd.Int,
					Type = TypeSyntax.Constant,
					Value = num
				};
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("stringagg"))
			{
				_index += 5;
				string fieldName = Function.ToString(MathObj().Value);
				_index++;
				string text3 = Function.ToString(MathObj().Value);
				string text4 = "";
				for (int num2 = 0; num2 < viewEditModel.RowCount(); num2++)
				{
					text4 = text4 + viewEditModel.GetRowCellValue(num2, fieldName)?.ToString() + text3;
				}
				_index++;
				if (text4 != "")
				{
					text4 = text4.Substring(0, text4.Length - 1);
				}
				return new Token
				{
					DataType = DataTypeCtd.String,
					Type = TypeSyntax.Variable,
					Value = text4
				};
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("focusedcolumn"))
			{
				_index += 4;
				return new Token
				{
					DataType = DataTypeCtd.String,
					Type = TypeSyntax.Constant,
					Value = viewEditModel.GetFocusedColumnName()
				};
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("getrowcellvalue"))
			{
				_index += 5;
				Token token4 = MathObj();
				_index++;
				Token token5 = MathObj();
				_index++;
				if (Function.ParseInt(token4.Value) >= viewEditModel.RowCount())
				{
					return new Token
					{
						Type = TypeSyntax.Constant,
						Value = DBNull.Value,
						DataType = DataTypeCtd.Object
					};
				}
				Token token3 = new Token();
				token3.DataType = DataTypeCtd.Object;
				token3.Type = TypeSyntax.Constant;
				token3.Value = (object)viewEditModel.GetRowCellValue(Function.ParseInt(token4.Value), ControlFunction.ToString(token5.Value));
				return token3;
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("getparentrowhandle"))
			{
				_index += 5;
				Token token6 = MathObj();
				_index++;
				Token token3 = new Token();
				token3.DataType = DataTypeCtd.Int;
				token3.Type = TypeSyntax.Constant;
				token3.Value = (object)viewEditModel.GetParentRowHandle(Function.ParseInt(token6.Value));
				return token3;
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("getfocusrowcellvalue"))
			{
				_index += 5;
				Token token7 = MathObj();
				_index++;
				Token token3 = new Token();
				token3.DataType = DataTypeCtd.Object;
				token3.Type = TypeSyntax.Constant;
				token3.Value = (object)viewEditModel.GetFocusedRowCellValue(ControlFunction.ToString(token7.Value));
				return token3;
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("getchildrowhandle"))
			{
				_index += 5;
				Token token8 = MathObj();
				_index++;
				Token token9 = MathObj();
				_index++;
				Token token3 = new Token();
				token3.DataType = DataTypeCtd.Int;
				token3.Type = TypeSyntax.Constant;
				token3.Value = (object)viewEditModel.GetChildRowHandle(Function.ParseInt(token8.Value), Function.ParseInt(token9.Value));
				return token3;
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("getchildrowcount"))
			{
				_index += 5;
				Token token10 = MathObj();
				_index++;
				Token token3 = new Token();
				token3.DataType = DataTypeCtd.Int;
				token3.Type = TypeSyntax.Constant;
				token3.Value = (object)viewEditModel.GetChildRowCount(Function.ParseInt(token10.Value));
				return token3;
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("min"))
			{
				_index += 5;
				Token token11 = MathObj();
				_index++;
				Token token3 = new Token();
				token3.DataType = DataTypeCtd.Double;
				token3.Type = TypeSyntax.Constant;
				token3.Value = (object)dataSource.Compute(string.Format("Min({0})", ControlFunction.ToString(token11.Value)), "");
				return token3;
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("max"))
			{
				_index += 5;
				Token token12 = MathObj();
				_index++;
				Token token3 = new Token();
				token3.DataType = DataTypeCtd.Double;
				token3.Type = TypeSyntax.Constant;
				token3.Value = (object)dataSource.Compute(string.Format("MAX({0})", ControlFunction.ToString(token12.Value)), "");
				return token3;
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("sum"))
			{
				_index += 5;
				Token token13 = MathObj();
				_index++;
				Token token3 = new Token();
				token3.DataType = DataTypeCtd.Double;
				token3.Type = TypeSyntax.Constant;
				token3.Value = (object)dataSource.Compute(string.Format("SUM({0})", ControlFunction.ToString(token13.Value)), "");
				return token3;
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("sumcolumns"))
			{
				_index += 5;
				Token token14 = MathObj();
				_index++;
				Token token15 = MathObj();
				_index++;
				string text5 = ControlFunction.ToString(token15.Value);
				if (viewEditModel.RowCount() <= Function.ParseInt(token14.Value))
				{
					return new Token
					{
						DataType = DataTypeCtd.Double,
						Type = TypeSyntax.Constant,
						Value = 0
					};
				}
				double num3 = 0.0;
				if (text5.StartsWith("%"))
				{
					string value = text5.Substring(1, text5.Length - 1);
					for (int num4 = 0; num4 < dataSource.Columns.Count; num4++)
					{
						if (dataSource.Columns[num4].ColumnName.EndsWith(value))
						{
							num3 += Function.ParseDouble(viewEditModel.GetRowCellValue(Function.ParseInt(token14.Value), dataSource.Columns[num4].ColumnName));
						}
					}
					return new Token
					{
						DataType = DataTypeCtd.Double,
						Type = TypeSyntax.Constant,
						Value = num3
					};
				}
				if (text5.EndsWith("%"))
				{
					string value2 = text5.Substring(0, text5.Length - 2);
					for (int num5 = 0; num5 < dataSource.Columns.Count; num5++)
					{
						if (dataSource.Columns[num5].ColumnName.StartsWith(value2))
						{
							num3 += Function.ParseDouble(viewEditModel.GetRowCellValue(Function.ParseInt(token14.Value), dataSource.Columns[num5].ColumnName));
						}
					}
					return new Token
					{
						DataType = DataTypeCtd.Double,
						Type = TypeSyntax.Constant,
						Value = num3
					};
				}
				Token token3 = new Token();
				token3.DataType = DataTypeCtd.Double;
				token3.Type = TypeSyntax.Constant;
				token3.Value = (object)Function.ParseDouble(viewEditModel.GetRowCellValue(Function.ParseInt(token14.Value), text5));
				return token3;
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("sumrowscolumns"))
			{
				_index += 5;
				Token token16 = MathObj();
				_index++;
				string text6 = ControlFunction.ToString(token16.Value);
				double num6 = 0.0;
				if (text6.StartsWith("%"))
				{
					string value3 = text6.Substring(1, text6.Length - 1);
					for (int num7 = 0; num7 < dataSource.Rows.Count; num7++)
					{
						for (int num8 = 0; num8 < dataSource.Columns.Count; num8++)
						{
							if (dataSource.Columns[num8].ColumnName.EndsWith(value3))
							{
								num6 += Function.ParseDouble(dataSource.Rows[num7][num8]);
							}
						}
					}
					return new Token
					{
						DataType = DataTypeCtd.Double,
						Type = TypeSyntax.Constant,
						Value = num6
					};
				}
				if (text6.EndsWith("%"))
				{
					string value4 = text6.Substring(0, text6.Length - 2);
					for (int num9 = 0; num9 < dataSource.Rows.Count; num9++)
					{
						for (int num10 = 0; num10 < dataSource.Columns.Count; num10++)
						{
							if (dataSource.Columns[num10].ColumnName.StartsWith(value4))
							{
								num6 += Function.ParseDouble(dataSource.Rows[num9][num10]);
							}
						}
					}
					return new Token
					{
						DataType = DataTypeCtd.Double,
						Type = TypeSyntax.Constant,
						Value = num6
					};
				}
				return new Token
				{
					DataType = DataTypeCtd.Double,
					Type = TypeSyntax.Constant,
					Value = dataSource.Compute($"SUM({text6})", "")
				};
			}
			if (_syntax[_index + 3].Name.ToLower().Equals("avg"))
			{
				_index += 5;
				Token token17 = MathObj();
				_index++;
				Token token3 = new Token();
				token3.DataType = DataTypeCtd.Double;
				token3.Type = TypeSyntax.Constant;
				token3.Value = (object)dataSource.Compute(string.Format("AVG({0})", ControlFunction.ToString(token17.Value)), "");
				return token3;
			}
			return new Token
			{
				Type = TypeSyntax.Constant,
				Value = DBNull.Value,
				DataType = DataTypeCtd.Object
			};
		}
		if (_syntax[_index + 3].Type == TypeSyntax.Operator && _syntax[_index + 3].Name.Equals("["))
		{
			_index += 4;
			int num11 = viewEditModel.IndexEditRow;
			Token token18 = MathObj();
			if (token18.DataType == DataTypeCtd.Int)
			{
				num11 = Function.ParseInt(token18.Value);
				_index += 2;
				if (_syntax[_index].Type == TypeSyntax.KeyWord && _syntax[_index].Name.Equals("isselectedrow"))
				{
					return new Token
					{
						DataType = DataTypeCtd.Boolean,
						Type = TypeSyntax.Constant,
						Value = viewEditModel.IsSelectedRow(num11)
					};
				}
				_index++;
				token18 = MathObj();
				_index--;
			}
			else if (token18.DataType == DataTypeCtd.String)
			{
				num11 = sourceIndex;
				_index--;
			}
			if (token18.DataType == DataTypeCtd.String)
			{
				_index++;
				if (_syntax[_index + 1].Type != TypeSyntax.Operator || !_syntax[_index + 1].Name.Equals("."))
				{
					_index++;
					Token token3 = new Token();
					token3.DataType = DataTypeCtd.Object;
					token3.Type = TypeSyntax.Constant;
					token3.Value = (object)viewEditModel.GetRowCellValue(num11, ControlFunction.ToString(token18.Value));
					return token3;
				}
				if (_syntax[_index + 2].Type == TypeSyntax.KeyWord)
				{
					if (_syntax[_index + 2].Name.Equals("string"))
					{
						_index += 3;
						Token token3 = new Token();
						token3.DataType = DataTypeCtd.String;
						token3.Type = TypeSyntax.Constant;
						token3.Value = (object)viewEditModel.GetRowCellValue(num11, ControlFunction.ToString(token18.Value));
						return token3;
					}
					if (_syntax[_index + 2].Name.Equals("int"))
					{
						_index += 3;
						Token token3 = new Token();
						token3.DataType = DataTypeCtd.Int;
						token3.Type = TypeSyntax.Constant;
						token3.Value = (object)viewEditModel.GetRowCellValue(num11, ControlFunction.ToString(token18.Value));
						return token3;
					}
					if (_syntax[_index + 2].Name.Equals("double"))
					{
						_index += 3;
						Token token3 = new Token();
						token3.DataType = DataTypeCtd.Double;
						token3.Type = TypeSyntax.Constant;
						token3.Value = (object)viewEditModel.GetRowCellValue(num11, ControlFunction.ToString(token18.Value));
						return token3;
					}
					if (_syntax[_index + 2].Name.Equals("bool"))
					{
						_index += 3;
						Token token3 = new Token();
						token3.DataType = DataTypeCtd.Boolean;
						token3.Type = TypeSyntax.Constant;
						token3.Value = (object)viewEditModel.GetRowCellValue(num11, ControlFunction.ToString(token18.Value));
						return token3;
					}
					if (_syntax[_index + 2].Name.Equals("datetime"))
					{
						_index += 3;
						Token token3 = new Token();
						token3.DataType = DataTypeCtd.DateTime;
						token3.Type = TypeSyntax.Constant;
						token3.Value = (object)Function.ParseDateTime(viewEditModel.GetRowCellValue(num11, ControlFunction.ToString(token18.Value)));
						return token3;
					}
					if (_syntax[_index + 2].Name.Equals("date"))
					{
						_index += 3;
						Token token3 = new Token();
						token3.DataType = DataTypeCtd.DateTime;
						token3.Type = TypeSyntax.Constant;
						token3.Value = (object)Function.ParseDateTime(viewEditModel.GetRowCellValue(num11, ControlFunction.ToString(token18.Value)));
						return token3;
					}
					return new Token
					{
						Type = TypeSyntax.Constant,
						Value = DBNull.Value,
						DataType = DataTypeCtd.Object
					};
				}
			}
			return new Token
			{
				Type = TypeSyntax.Constant,
				Value = DBNull.Value,
				DataType = DataTypeCtd.Object
			};
		}
		return new Token
		{
			Type = TypeSyntax.Constant,
			Value = DBNull.Value,
			DataType = DataTypeCtd.Object
		};
	}

	private void MathVariable()
	{
		if (_syntax[_index].Type == TypeSyntax.Operator)
		{
			SetDataView();
			return;
		}
		if (_syntax[_index].Type == TypeSyntax.KeyWord)
		{
			_index += 2;
			Token token = MathObj();
			CancelEvent = ControlFunction.ParseBoolean(token.Value);
			return;
		}
		string name = _syntax[_index].Name.ToLower();
		Token token2 = _variable.Find((Token t) => t.Name == name);
		if (token2.DataType == DataTypeCtd.DataSet)
		{
			SetDataset();
			return;
		}
		if (token2.DataType == DataTypeCtd.DataTable)
		{
			SetDatatable();
			return;
		}
		if (token2.DataType == DataTypeCtd.DataRow)
		{
			SetDatarow();
			return;
		}
		if (token2.DataType == DataTypeCtd.List)
		{
			SetDatalist();
			return;
		}
		_index += 2;
		Token token3 = MathObj(-1, token2.DataType);
		token2.Value = (object)token3.Value;
	}

	private void SetDatalist()
	{
		string name = _syntax[_index].Name.ToLower();
		Token token = _variable.Find((Token t) => t.Name == name);
		_index++;
		if (_syntax[_index].Name.Equals("="))
		{
			_index++;
			string name2 = _syntax[_index].Name.ToLower();
			Token token2 = _variable.Find((Token t) => t.Name == name2);
			token.Value = (object)token2.Value;
			_index++;
		}
		else
		{
			_index++;
			Token token3 = MathObj();
			_index += 2;
			Token token4 = MathObj();
			token.Value[ControlFunction.ToString(token3.Value)] = token4.Value;
			_index++;
		}
	}

	private void SetDatarow()
	{
		string name = _syntax[_index].Name.ToLower();
		Token token = _variable.Find((Token t) => t.Name == name);
		_index++;
		if (_syntax[_index].Name.Equals("="))
		{
			_index++;
			Token token2 = MathObj();
			token.Value = (object)token2.Value;
			_index++;
		}
		else
		{
			_index += 2;
			Token token3 = MathObj();
			_index += 2;
			Token token4 = MathObj();
			token.Value[ControlFunction.ToString(token3.Value)] = token4.Value;
			_index++;
		}
	}

	private void SetDatatable()
	{
		string name = _syntax[_index].Name.ToLower();
		Token token = _variable.Find((Token t) => t.Name == name);
		_index++;
		if (_syntax[_index].Name.Equals("="))
		{
			_index++;
			Token token2 = MathObj();
			token.Value = (object)token2.Value;
			_index++;
			return;
		}
		_index += 2;
		Token obj = MathObj();
		if (_syntax[_index + 1].Name.Equals("="))
		{
			_index += 2;
			Token token3 = MathObj();
			DataRow dataRow = token.Value.NewRow();
			dataRow.ItemArray = token3.Value.ItemArray;
			token.Value.Rows.RemoveAt(Function.ParseInt(obj));
			token.Value.Rows.InsertAt(dataRow, Function.ParseInt(obj));
			_index++;
			return;
		}
		_index += 2;
		Token token4 = MathObj();
		_index += 2;
		Token token5 = MathObj();
		dynamic val = Function.ParseInt(obj) >= token.Value.Rows.Count;
		if (!(val ? true : false) && !((val | !token.Value.Columns.Contains(ControlFunction.ToString(token4.Value))) ? true : false))
		{
			token.Value.Rows[Function.ParseInt(obj)][ControlFunction.ToString(token4.Value)] = token5.Value;
			_index++;
		}
	}

	private void SetDataset()
	{
		string name = _syntax[_index].Name.ToLower();
		Token token = _variable.Find((Token t) => t.Name == name);
		_index++;
		if (_syntax[_index].Name.Equals("="))
		{
			_index++;
			Token token2 = MathObj();
			token.Value = (object)token2.Value;
			_index++;
			return;
		}
		_index += 2;
		Token obj = MathObj();
		if (Function.ParseInt(obj) >= token.Value.Tables.Count)
		{
			return;
		}
		_index++;
		if (_syntax[_index].Name.Equals("="))
		{
			_index++;
			Token token3 = MathObj();
			token.Value.Tables[Function.ParseInt(obj)] = token3.Value;
			_index++;
			return;
		}
		_index += 2;
		Token obj2 = MathObj();
		if (Function.ParseInt(obj2) >= token.Value.Tables[Function.ParseInt(obj)].Rows.Count)
		{
			return;
		}
		if (_syntax[_index + 1].Name.Equals("="))
		{
			_index += 2;
			Token token4 = MathObj();
			DataRow dataRow = token.Value.Tables[Function.ParseInt(obj)].NewRow();
			dataRow.ItemArray = token4.Value.ItemArray;
			token.Value.Tables[Function.ParseInt(obj)].Rows.RemoveAt(Function.ParseInt(obj2));
			token.Value.Tables[Function.ParseInt(obj)].Rows.InsertAt(dataRow, Function.ParseInt(obj2));
			_index++;
			return;
		}
		_index += 3;
		Token token5 = MathObj();
		if (!((!token.Value.Tables[Function.ParseInt(obj)].Columns.Contains(ControlFunction.ToString(token5.Value))) ? true : false))
		{
			_index += 2;
			Token token6 = MathObj();
			token.Value.Tables[Function.ParseInt(obj)].Rows[Function.ParseInt(obj2)][ControlFunction.ToString(token5.Value)] = token6.Value;
			_index++;
		}
	}

	private void SetDataView()
	{
		bool flag = true;
		if (_form.FormId.ToLower().Equals(_syntax[_index + 2].Name.ToLower()))
		{
			_index += 6;
			if (_syntax[_index].Type == TypeSyntax.Constant)
			{
				string name = _syntax[_index].Name;
				_index += 3;
				if (_syntax[_index].Type == TypeSyntax.KeyWord)
				{
					switch (_syntax[_index].Name.ToLower())
					{
					case "hide":
						_form.SetVisibleControl(name, isVisible: false);
						if (_syntax[_index + 1].Name.Equals("("))
						{
							_index += 2;
						}
						_index++;
						return;
					case "show":
						_form.SetVisibleControl(name, isVisible: true);
						if (_syntax[_index + 1].Name.Equals("("))
						{
							_index += 2;
						}
						_index++;
						return;
					case "focus":
						_form.SetFocusControl(name);
						if (_syntax[_index + 1].Name.Equals("("))
						{
							_index += 2;
						}
						_index++;
						return;
					case "enable":
					{
						_index += 2;
						Token token2 = MathObj();
						_form.SetEnableControl(name, token2.Value);
						_index++;
						return;
					}
					case "readonly":
					{
						_index += 2;
						Token token = MathObj();
						_form.SetReadOnlyControl(name, token.Value);
						_index++;
						return;
					}
					case "color":
					{
						_index += 2;
						Color colorByName = ControlFunction.GetColorByName(_syntax[_index].Name.ToLower());
						_form.SetColorControl(name, colorByName);
						_index++;
						return;
					}
					}
				}
				Token token3 = MathObj();
				_form.SetDataSourceControl(name, token3.Value);
				_index++;
				return;
			}
		}
		else
		{
			_index += 2;
		}
		string name2 = _syntax[_index].Name;
		ViewEditModel viewEditModel = ControlFunction.CheckExistGrid(_lstGrid, name2);
		if (_syntax[_index + 2].Name.Equals("="))
		{
			_index += 3;
			Token token4 = MathObj();
			viewEditModel.DataSource = token4.Value;
			viewEditModel.DataSource.AcceptChanges();
			_index++;
			return;
		}
		_index += 3;
		if (_syntax[_index].Type == TypeSyntax.KeyWord)
		{
			if (_syntax[_index].Name.ToLower().Equals("setrowcellvalue"))
			{
				_index += 2;
				Token token5 = MathObj();
				_index++;
				Token token6 = MathObj();
				if ((!ControlFunction.CheckExistGridColumn(viewEditModel, ControlFunction.ToString(token6.Value))))
				{
					WinCommons.Error(Enums.MessageCode.ColumnDoesNotBelongItem, "", (string)ControlFunction.ToString(token6.Value), viewEditModel.ItemId);
					flag = false;
				}
				_index++;
				Token token7 = MathObj();
				if (flag)
				{
					if (viewEditModel.CheckExistColumnVisible(ControlFunction.ToString(token6.Value)))
					{
						viewEditModel.RemoveChangedField(ControlFunction.ToString(token6.Value));
						viewEditModel.SetRowCellValue(Function.ParseInt(token5.Value), ControlFunction.ToString(token6.Value), token7.Value);
						viewEditModel.AddChangedField(ControlFunction.ToString(token6.Value));
					}
					else
					{
						viewEditModel.DataSource.Rows[Function.ParseInt(token5.Value)][ControlFunction.ToString(token6.Value)] = token7.Value;
						viewEditModel.DataSource.AcceptChanges();
					}
				}
				_index++;
				return;
			}
			if (_syntax[_index].Name.ToLower().Equals("setdatasourcecell"))
			{
				_index += 2;
				Token token8 = MathObj();
				_index++;
				Token token9 = MathObj();
				if ((!ControlFunction.CheckExistGridColumn(viewEditModel, ControlFunction.ToString(token9.Value))))
				{
					WinCommons.Error(Enums.MessageCode.ColumnDoesNotBelongItem, "", (string)ControlFunction.ToString(token9.Value), viewEditModel.ItemId);
					flag = false;
				}
				_index++;
				Token token10 = MathObj();
				if (flag)
				{
					viewEditModel.DataSource.Rows[viewEditModel.GetSourceIndex(Function.ParseInt(token8.Value))][ControlFunction.ToString(token9.Value)] = token10.Value;
					viewEditModel.DataSource.AcceptChanges();
				}
				_index++;
				return;
			}
			if (_syntax[_index].Name.ToLower().Equals("setfocusrowcellvalue"))
			{
				_index += 2;
				Token token11 = MathObj();
				if ((!ControlFunction.CheckExistGridColumn(viewEditModel, ControlFunction.ToString(token11.Value))))
				{
					WinCommons.Error(Enums.MessageCode.ColumnDoesNotBelongItem, "", (string)ControlFunction.ToString(token11.Value), viewEditModel.ItemId);
					flag = false;
				}
				_index++;
				Token token12 = MathObj();
				if (flag)
				{
					if (viewEditModel.CheckExistColumnVisible(ControlFunction.ToString(token11.Value)))
					{
						viewEditModel.RemoveChangedField(ControlFunction.ToString(token11.Value));
						viewEditModel.SetFocusedRowCellValue(ControlFunction.ToString(token11.Value), token12.Value);
						viewEditModel.AddChangedField(ControlFunction.ToString(token11.Value));
					}
					else
					{
						viewEditModel.DataSource.Rows[viewEditModel.IndexEditRow][ControlFunction.ToString(token11.Value)] = token12.Value;
						viewEditModel.DataSource.AcceptChanges();
					}
				}
				_index++;
				return;
			}
			if (_syntax[_index].Name.ToLower().Equals("setfocusdatasourcecell"))
			{
				_index += 2;
				Token token13 = MathObj();
				if ((!ControlFunction.CheckExistGridColumn(viewEditModel, ControlFunction.ToString(token13.Value))))
				{
					WinCommons.Error(Enums.MessageCode.ColumnDoesNotBelongItem, "", (string)ControlFunction.ToString(token13.Value), viewEditModel.ItemId);
					flag = false;
				}
				_index++;
				Token token14 = MathObj();
				if (flag)
				{
					viewEditModel.DataSource.Rows[viewEditModel.IndexEditRow][ControlFunction.ToString(token13.Value)] = token14.Value;
					viewEditModel.DataSource.AcceptChanges();
				}
				_index++;
				return;
			}
			if (_syntax[_index].Name.ToLower().Equals("columns") && _syntax[_index + 1].Name.Equals("["))
			{
				_index += 2;
				Token token15 = MathObj();
				bool flag2 = _syntax[_index + 2].Name.ToLower().Equals("show");
				_index += 3;
				if (token15.DataType == DataTypeCtd.Int)
				{
					viewEditModel.SetColumnVisible(Function.ParseInt(token15.Value), flag2);
				}
				else
				{
					viewEditModel.SetColumnVisible(Function.ToString(token15.Value), flag2);
				}
				return;
			}
			if (_syntax[_index].Name.ToLower().Equals("newrow"))
			{
				DataTable dataSource = viewEditModel.DataSource;
				DataRow row = dataSource.NewRow();
				dataSource.Rows.Add(row);
				dataSource.AcceptChanges();
				if (_syntax[_index + 1].Name.Equals("("))
				{
					_index += 2;
				}
				_index++;
				return;
			}
			if (_syntax[_index].Name.ToLower().Equals("loop"))
			{
				_variable.Add(new Token
				{
					Name = "0"
				});
				MathLoop();
				RemoveVariable();
				return;
			}
			if (_syntax[_index].Name.ToLower().Equals("clear") || _syntax[_index].Name.ToLower().Equals("selectall") || _syntax[_index].Name.ToLower().Equals("clearselection"))
			{
				switch (_syntax[_index].Name.ToLower())
				{
				case "clear":
					viewEditModel.DataSource.Rows.Clear();
					viewEditModel.DataSource.AcceptChanges();
					break;
				case "selectall":
					viewEditModel.SelectAll();
					break;
				case "clearselection":
					viewEditModel.ClearSelection();
					break;
				}
				if (_syntax[_index + 1].Name.Equals("("))
				{
					_index += 2;
				}
				_index++;
				return;
			}
			string text = _syntax[_index].Name.ToLower();
			if (text.Equals("cancelnextcellvaluechangedevent") || text.Equals("cancelnextfocusobjectchangedevent") || text.Equals("cancelnextselectedchangedevent"))
			{
				_index += 2;
				Token token16 = MathObj();
				dynamic val = Function.ParseBool(token16.Value);
				if (text.Equals("cancelnextcellvaluechangedevent"))
				{
					viewEditModel.SetCancelNextCellValueChangedScript(val);
				}
				else if (text.Equals("cancelnextfocusobjectchangedevent"))
				{
					viewEditModel.SetCancelNextFocusObjectChangedScript(val);
				}
				else if (text.Equals("cancelnextselectedchangedevent"))
				{
					viewEditModel.SetCancelNextSelectedChangedScript(val);
				}
				return;
			}
		}
		_index++;
		Token token17 = MathObj();
		if (token17.DataType == DataTypeCtd.String)
		{
			int index = _index + 2;
			if (viewEditModel.DataSource.Rows.Count == 0)
			{
				_index = index;
				MathObj(0);
			}
			else
			{
				int num = viewEditModel.RowCount();
				if (viewEditModel.CheckExistColumnVisible(ControlFunction.ToString(token17.Value)))
				{
					for (int i = 0; i < num; i++)
					{
						_index = index;
						Token token18 = MathObj(i);
						viewEditModel.RemoveChangedField(ControlFunction.ToString(token17.Value));
						viewEditModel.SetRowCellValue(i, ControlFunction.ToString(token17.Value), token18.Value);
						viewEditModel.AddChangedField(ControlFunction.ToString(token17.Value));
					}
				}
				else
				{
					for (int j = 0; j < num; j++)
					{
						_index = index;
						Token token19 = MathObj(j);
						viewEditModel.DataSource.Rows[j][ControlFunction.ToString(token17.Value)] = token19.Value;
					}
					viewEditModel.DataSource.AcceptChanges();
				}
			}
			_index++;
			return;
		}
		if (_syntax[_index + 2].Name.Equals("="))
		{
			_index += 3;
			Token token20 = MathObj();
			DataTable dataSource2 = viewEditModel.DataSource;
			dataSource2.Rows[Function.ParseInt(token17.Value)].ItemArray = ((DataRow)token20.Value).ItemArray;
			dataSource2.AcceptChanges();
			_index++;
			return;
		}
		_index += 2;
		if (_syntax[_index].Type == TypeSyntax.KeyWord)
		{
			switch (_syntax[_index].Name)
			{
			case "focus":
				viewEditModel.SetFocusRow(Function.ParseInt(token17.Value));
				break;
			case "delete":
				viewEditModel.DeleteRow(Function.ParseInt(token17.Value));
				break;
			case "selected":
				viewEditModel.SetSelectedRow(Function.ParseInt(token17.Value), true);
				break;
			case "unselected":
				viewEditModel.SetSelectedRow(Function.ParseInt(token17.Value), false);
				break;
			}
			viewEditModel.DataSource.AcceptChanges();
			_index++;
			if (_syntax[_index].Name.Equals("("))
			{
				_index += 2;
			}
			return;
		}
		_index++;
		Token token21 = MathObj();
		if ((!ControlFunction.CheckExistGridColumn(viewEditModel, ControlFunction.ToString(token21.Value))))
		{
			if (viewEditModel.DataSource.Columns.Count > 0)
			{
				WinCommons.Error(Enums.MessageCode.ColumnDoesNotBelongItem, "", (string)ControlFunction.ToString(token21.Value), viewEditModel.ItemId);
			}
			flag = false;
		}
		if (Function.ParseInt(token17.Value) < 0)
		{
			flag = false;
		}
		if (_syntax[_index + 1].Name.Equals("."))
		{
			_index += 3;
			if (_syntax[_index].Name.Equals("("))
			{
				_index += 2;
			}
			viewEditModel.SetFocusColumn(Function.ParseInt(token17.Value), ControlFunction.ToString(token21.Value));
			return;
		}
		_index += 2;
		Token token22 = MathObj();
		if (flag)
		{
			if (viewEditModel.CheckExistColumnVisible(ControlFunction.ToString(token21.Value)))
			{
				viewEditModel.RemoveChangedField(ControlFunction.ToString(token21.Value));
				viewEditModel.SetRowCellValue(Function.ParseInt(token17.Value), ControlFunction.ToString(token21.Value), token22.Value);
				viewEditModel.AddChangedField(ControlFunction.ToString(token21.Value));
			}
			else
			{
				viewEditModel.DataSource.Rows[Function.ParseInt(token17.Value)][ControlFunction.ToString(token21.Value)] = token22.Value;
				viewEditModel.DataSource.AcceptChanges();
			}
		}
		_index++;
	}

	private Token MathGetData()
	{
		_index += 2;
		string text = _syntax[_index].Name.ToLower();
		_index += 2;
		Token token = MathObj();
		List<Token> list = new List<Token>();
		_index++;
		while ((_syntax[_index].Type != TypeSyntax.Operator || !_syntax[_index].Name.Equals(")")) && _index < _syntax.Count)
		{
			string name;
			if (_syntax[_index].Type == TypeSyntax.Operator && _syntax[_index].Name.Equals(","))
			{
				_index++;
			}
			else if (_syntax[_index + 1].Type != TypeSyntax.Operator || !_syntax[_index + 1].Name.Equals("="))
			{
				name = _syntax[_index].Name.ToLower();
				Token token2 = _variable.Find((Token t) => t.Name == name);
				list.Add(new Token
				{
					Name = name,
					DataType = token2.DataType,
					Value = token2.Value
				});
				_index++;
			}
			else
			{
				name = _syntax[_index].Name;
				_index += 2;
				Token token3 = MathObj();
				list.Add(new Token
				{
					Name = name,
					DataType = token3.DataType,
					Value = token3.Value
				});
			}
		}
		string text2 = string.Join("/", _url, "Formula", ControlFunction.ToString(token.Value));
		text2 += "?";
		if (list.Count > 0)
		{
			List<string> list2 = new List<string>();
			for (int num = 0; num < list.Count; num++)
			{
				list2.Add(list[num].Name + "=" + ControlFunction.ToString(list[num].Value));
			}
			text2 += string.Join("&", list2.ToArray());
		}
		Token token4;
		using (HttpResponseMessage httpResponseMessage = _httpClientHelper.GetAsync(text2).Result)
		{
			if (httpResponseMessage.IsSuccessStatusCode)
			{
				string value = httpResponseMessage.Content.ReadAsStringAsync().Result;
				if (httpResponseMessage.Content.Headers.ContentEncoding.ToList().Exists((string t) => t.Equals("deflate")))
				{
					byte[] bytes = CompressionHelper.UndeflateByte(httpResponseMessage.Content.ReadAsByteArrayAsync().Result);
					value = Encoding.UTF8.GetString(bytes);
				}
				else if (httpResponseMessage.Content.Headers.ContentEncoding.ToList().Exists((string t) => t.Equals("deflate-Gzip")))
				{
					byte[] bytes2 = JsoncCompressionHelper.Decompress(httpResponseMessage.Content.ReadAsByteArrayAsync().Result);
					value = Encoding.UTF8.GetString(bytes2);
				}
				switch (text)
				{
				case "int":
					token4 = new Token();
					token4.DataType = DataTypeCtd.Int;
					token4.Type = TypeSyntax.Constant;
					token4.Value = Function.ParseInt(JsonConvert.DeserializeObject<string>(value, new JsonConverter[2]
					{
						new ApzJsonConverterDataTable(),
						new ApzJsonConverterDataSet()
					}));
					token4 = token4;
					break;
				case "bool":
				{
					Token token5 = new Token();
					token5.DataType = DataTypeCtd.Boolean;
					token5.Type = TypeSyntax.Constant;
					token5.Value = JsonConvert.DeserializeObject<bool>(value, new JsonConverter[2]
					{
						new ApzJsonConverterDataTable(),
						new ApzJsonConverterDataSet()
					});
					token4 = token5;
					break;
				}
				case "date":
				{
					Token token5 = new Token();
					token5.DataType = DataTypeCtd.DateTime;
					token5.Type = TypeSyntax.Constant;
					token5.Value = JsonConvert.DeserializeObject<DateTime>(value, new JsonConverter[2]
					{
						new ApzJsonConverterDataTable(),
						new ApzJsonConverterDataSet()
					});
					token4 = token5;
					break;
				}
				case "datetime":
				{
					Token token5 = new Token();
					token5.DataType = DataTypeCtd.DateTime;
					token5.Type = TypeSyntax.Constant;
					token5.Value = JsonConvert.DeserializeObject<DateTime>(value, new JsonConverter[2]
					{
						new ApzJsonConverterDataTable(),
						new ApzJsonConverterDataSet()
					});
					token4 = token5;
					break;
				}
				case "double":
				{
					Token token5 = new Token();
					token5.DataType = DataTypeCtd.Double;
					token5.Type = TypeSyntax.Constant;
					token5.Value = Function.ParseDouble(JsonConvert.DeserializeObject<string>(value, new JsonConverter[2]
					{
						new ApzJsonConverterDataTable(),
						new ApzJsonConverterDataSet()
					}));
					token4 = token5;
					break;
				}
				case "string":
				{
					Token token5 = new Token();
					token5.DataType = DataTypeCtd.String;
					token5.Type = TypeSyntax.Constant;
					token5.Value = ControlFunction.ToString(JsonConvert.DeserializeObject<string>(value, new JsonConverter[2]
					{
						new ApzJsonConverterDataTable(),
						new ApzJsonConverterDataSet()
					}));
					token4 = token5;
					break;
				}
				case "datatable":
				{
					Token token5 = new Token();
					token5.DataType = DataTypeCtd.DataTable;
					token5.Type = TypeSyntax.Constant;
					token5.Value = JsonConvert.DeserializeObject<DataTable>(value, new JsonConverter[2]
					{
						new ApzJsonConverterDataTable(),
						new ApzJsonConverterDataSet()
					});
					token4 = token5;
					break;
				}
				case "dataset":
				{
					Token token5 = new Token();
					token5.DataType = DataTypeCtd.DataSet;
					token5.Type = TypeSyntax.Constant;
					token5.Value = JsonConvert.DeserializeObject<DataSet>(value, new JsonConverter[2]
					{
						new ApzJsonConverterDataTable(),
						new ApzJsonConverterDataSet()
					});
					token4 = token5;
					break;
				}
				default:
					token4 = new Token
					{
						Type = TypeSyntax.Constant,
						Value = DBNull.Value,
						DataType = DataTypeCtd.Object
					};
					break;
				}
			}
			else
			{
				token4 = new Token
				{
					Type = TypeSyntax.Constant,
					Value = DBNull.Value,
					DataType = DataTypeCtd.Object
				};
			}
		}
		return token4;
	}

	private Token MathExecute(DataTypeCtd outputType = DataTypeCtd.None)
	{
		_index += 2;
		Token token = MathObj();
		List<Token> list = new List<Token>();
		while ((_syntax[_index].Type != TypeSyntax.Operator || !_syntax[_index].Name.Equals(")")) && _index < _syntax.Count)
		{
			if (_syntax[_index].Type == TypeSyntax.Operator && _syntax[_index].Name.Equals(","))
			{
				_index++;
				continue;
			}
			Token token2 = MathObj();
			list.Add(new Token
			{
				Name = token2.Name,
				Value = token2.Value,
				DataType = token2.DataType,
				TagetType = token2.TagetType
			});
		}
		List<DatabaseParamater> list2 = Function.ParseSql(ControlFunction.ToString(token.Value));
		List<QueryParamater> list3 = new List<QueryParamater>();
		for (int i = 0; i < list2.Count; i++)
		{
			if (i >= list.Count)
			{
				list3.Add(new QueryParamater
				{
					ParaName = list2[i].ParaName,
					DataType = "string",
					Value = ""
				});
			}
			else
			{
				list3.Add(new QueryParamater
				{
					ParaName = list2[i].ParaName,
					DataType = Function.ToString(list[i].DataType),
					Value = list[i].Value
				});
			}
		}
		DataSet dataSet = GetDataFromService(Function.ToString(token.Value), list3);
		switch (outputType)
		{
		case DataTypeCtd.None:
		case DataTypeCtd.Dynamic:
		case DataTypeCtd.DataSet:
		case DataTypeCtd.Object:
			return new Token
			{
				Type = TypeSyntax.Constant,
				DataType = DataTypeCtd.DataSet,
				Value = dataSet
			};
		case DataTypeCtd.DataTable:
			return new Token
			{
				Type = TypeSyntax.Constant,
				DataType = DataTypeCtd.DataTable,
				Value = dataSet.Tables[1]
			};
		default:
			if (dataSet == null || dataSet.Tables.Count < 2 || dataSet.Tables[1].Rows.Count == 0)
			{
				return new Token
				{
					Type = TypeSyntax.Constant,
					DataType = DataTypeCtd.DataSet,
					Value = DBNull.Value
				};
			}
			return new Token
			{
				Type = TypeSyntax.Constant,
				DataType = outputType,
				Value = dataSet.Tables[1].Rows[0][0]
			};
		}
	}

	protected virtual DataSet GetDataFromService(string query, List<QueryParamater> lstParamaters)
	{
		Tuple<string, List<QueryParamater>, string> value = new Tuple<string, List<QueryParamater>, string>(query, lstParamaters, "");
		string apiUri = Function.GetApiUri(_url, "Commons", "ExecuteSql");
		using HttpResponseMessage httpResponseMessage = _httpClientHelper.PostAsJsonAsync(apiUri, value).Result;
		if (httpResponseMessage.IsSuccessStatusCode)
		{
			string value2 = httpResponseMessage.Content.ReadAsStringAsync().Result;
			if (httpResponseMessage.Content.Headers.ContentEncoding.ToList().Exists((string t) => t.Equals("deflate")))
			{
				byte[] bytes = CompressionHelper.UndeflateByte(httpResponseMessage.Content.ReadAsByteArrayAsync().Result);
				value2 = Encoding.UTF8.GetString(bytes);
			}
			else if (httpResponseMessage.Content.Headers.ContentEncoding.ToList().Exists((string t) => t.Equals("deflate-Gzip")))
			{
				byte[] bytes2 = JsoncCompressionHelper.Decompress(httpResponseMessage.Content.ReadAsByteArrayAsync().Result);
				value2 = Encoding.UTF8.GetString(bytes2);
			}
			return JsonConvert.DeserializeObject<DataSet>(value2, new JsonConverter[2]
			{
				new ApzJsonConverterDataTable(),
				new ApzJsonConverterDataSet()
			});
		}
		return new DataSet();
	}

	private Token MathInvokMethod()
	{
		_index += 2;
		Token token = MathObj();
		dynamic val = Function.ToString(token.Value);
		object value = ProcessContextActionMethod(val);
		return new Token
		{
			DataType = DataTypeCtd.HttpResult,
			Value = value,
			Name = Function.ToString(_index),
			Type = TypeSyntax.Constant
		};
	}

	private Token MathMergePDF()
	{
		_index += 2;
		Token token = MathObj();
		_index++;
		Token token2 = MathObj();
		object value = Function.MergePDFFromStringBase64((List<string>)token.Value, Function.ToString(token2.Value));
		return new Token
		{
			DataType = DataTypeCtd.String,
			Value = value,
			Name = Function.ToString(_index),
			Type = TypeSyntax.Constant
		};
	}

	private Token MathProcessSAPObject()
	{
		_index += 2;
		Token token = MathObj();
		_index++;
		Token token2 = MathObj();
		_index++;
		Token token3 = MathObj();
		_index++;
		Token token4 = MathObj();
		string apiUri = Function.GetApiUri(_url, "Commons", "ProcessSAPJson");
		ProcessJsonImpl processJsonImpl = new ProcessJsonImpl();
		processJsonImpl.ObjType = Function.ToString(token.Value);
		processJsonImpl.Action = Function.ToString(token2.Value);
		processJsonImpl.Json = Function.ToString(token3.Value);
		processJsonImpl.ObjKey = (object)Function.ToString(token4.Value);
		ProcessJsonImpl value = processJsonImpl;
		using HttpResponseMessage httpResponseMessage = _httpClientHelper.PostAsJsonAsync(apiUri, value).Result;
		string result = httpResponseMessage.Content.ReadAsStringAsync().Result;
		HttpResult value2 = ((!httpResponseMessage.IsSuccessStatusCode) ? new HttpResult
		{
			MessageCode = Enums.MessageCode.Error,
			Message = result,
			Content = httpResponseMessage.ReasonPhrase
		} : JsonConvert.DeserializeObject<HttpResult>(result));
		return new Token
		{
			DataType = DataTypeCtd.HttpResult,
			Value = value2,
			Name = Function.ToString(_index),
			Type = TypeSyntax.Constant
		};
	}

	protected virtual HttpResult ProcessContextActionMethod(string aliasName)
	{
		Type type = Type.GetType("Apzon.Forms.Commons.ContextActionHandle, Apzon.Forms");
		if (type != null)
		{
			dynamic val = Activator.CreateInstance(type, aliasName, _form, _gridView);
			return val.Handle().Result;
		}
		return new HttpResult();
	}

	private Token MathShowFormAndShowDialogMethod(bool isDialog = false)
	{
		_index += 2;
		Token token = MathObj();
		List<Token> list = new List<Token>();
		while ((_syntax[_index].Type != TypeSyntax.Operator || !_syntax[_index].Name.Equals(")")) && _index < _syntax.Count)
		{
			if (_syntax[_index].Type == TypeSyntax.Operator && _syntax[_index].Name.Equals(","))
			{
				_index++;
				continue;
			}
			Token token2 = MathObj();
			list.Add(new Token
			{
				Name = token2.Name,
				Value = token2.Value,
				DataType = token2.DataType,
				TagetType = token2.TagetType
			});
		}
		List<object> list2 = new List<object>();
		for (int i = 0; i < list.Count; i++)
		{
			list2.Add(list[i].Value);
		}
		Token token3 = new Token();
		token3.DataType = DataTypeCtd.HttpResult;
		token3.Value = (object)(isDialog ? _form.ShowFormDialog(Function.ToString(token.Value), list2) : _form.ShowForm(Function.ToString(token.Value), list2));
		token3.Name = Function.ToString(_index);
		token3.Type = TypeSyntax.Constant;
		return token3;
	}
}
