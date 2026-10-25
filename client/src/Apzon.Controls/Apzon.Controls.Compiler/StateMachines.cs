using System.Collections.Generic;
using System.Linq;
using Apzon.Controls.Commons;
using Apzon.Controls.Form;

namespace Apzon.Controls.Compiler;

public class StateMachines
{
	private readonly XtraFormEx _form;

	private readonly List<ViewEditModel> _lstGridView;

	private readonly List<Token> _sytax;

	private List<Token> _variable;

	private int _index;

	public StateMachines(XtraFormEx form, List<ViewEditModel> lstGridView, List<Token> sytax)
	{
		_form = form;
		_lstGridView = lstGridView;
		_sytax = sytax;
		_variable = new List<Token>
		{
			new Token
			{
				Type = (TypeSyntax)0
			}
		};
		_index = 0;
	}

	public CtdMessage ParseStatment(StatmentType type)
	{
		CtdMessage ctdMessage = new CtdMessage();
		int count = _sytax.Count;
		while (string.IsNullOrEmpty(ctdMessage.Message) && _index < count)
		{
			if (_sytax[_index].Type == TypeSyntax.Constant)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Statement can not start with constant",
					LineNumber = _sytax[_index].LineNum
				};
			}
			switch (_sytax[_index].Type)
			{
			case TypeSyntax.Operator:
				switch (_sytax[_index].Name)
				{
				case ";":
					_index++;
					break;
				case "}":
					_index++;
					return new CtdMessage
					{
						Message = "end"
					};
				case "$":
					ctdMessage = ParseAssign();
					_index++;
					break;
				default:
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = $"Statement can not start with operator '{_sytax[_index].Name}'",
						LineNumber = _sytax[_index].LineNum
					};
				}
				break;
			case TypeSyntax.KeyWord:
			{
				string text = _sytax[_index].Name.ToLower();
				switch (text)
				{
				case "if":
					ctdMessage = ParseStatmentIf(type);
					_index++;
					break;
				case "for":
					_variable.Add(new Token
					{
						Name = "0"
					});
					ctdMessage = ParseStatmentFor();
					RemoveVariable(ref _variable);
					break;
				case "else":
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Statement can not start with keywork 'else'",
						LineNumber = _sytax[_index].LineNum
					};
				case "showmessagebox":
					ctdMessage = ParseShowMessageBox(ismessage: true);
					_index++;
					break;
				case "drilldown":
					ctdMessage = ParseDrillDown();
					_index++;
					break;
				case "showerror":
				case "showsuccess":
				case "showwarning":
				case "showinfo":
				case "openlink":
					ctdMessage = ParseShowStatus();
					_index++;
					break;
				case "while":
					ctdMessage = ParseWhile();
					break;
				case "return":
					if (_index >= _sytax.Count - 1 || _sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals(";"))
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Statement missing operator ';' after 'return' statment",
							LineNumber = _sytax[_index].LineNum
						};
					}
					_index += 2;
					break;
				case "cancelevent":
					ctdMessage = ParseAssign();
					_index++;
					break;
				case "getdata":
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Statement can not start with keywork 'getdata'",
						LineNumber = _sytax[_index].LineNum
					};
				case "execute":
					ctdMessage = ParseExecute();
					_index++;
					break;
				case "invoke":
					ctdMessage = ParseInvokeMethod();
					_index++;
					break;
				case "showform":
				case "showdialog":
					ctdMessage = ParseShowFormAndDialogMethod();
					_index++;
					break;
				case "push":
					_index++;
					ctdMessage = ParsePushFunction();
					_index++;
					break;
				case "continue":
				case "break":
					if (type == StatmentType.Loop)
					{
						_index++;
						break;
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = $"Statement can not start with keywork '{text}'",
						LineNumber = _sytax[_index].LineNum
					};
				case "add":
					ctdMessage = ParseFunctionList();
					_index++;
					break;
				default:
					if (ConstantsApzCtd.DataTableKeyWordApz.Contains(text))
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = $"Statement can not start with datatable keyword '{_sytax[_index].Name}'",
							LineNumber = _sytax[_index].LineNum
						};
					}
					ctdMessage = ParseDeclare();
					break;
				}
				break;
			}
			case TypeSyntax.Variable:
				if (_variable.Find((Token t) => t.Name == _sytax[_index].Name.ToLower()).DataType != DataTypeCtd.List || _sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("."))
				{
					ctdMessage = ParseAssign();
					_index++;
				}
				else
				{
					_index += 2;
					ctdMessage = ParseFunctionList();
					_index++;
				}
				break;
			}
		}
		return ctdMessage;
	}

	private CtdMessage ParseFunctionList()
	{
		if (_sytax.Count < _index + 4)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement for list data type",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		Token token = _variable.Find((Token t) => t.Name == _sytax[_index - 2].Name.ToLower());
		if (_sytax[_index].Type == TypeSyntax.KeyWord && _sytax[_index].Name.ToLower().Equals("add"))
		{
			if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement add. Mssing '('",
					LineNumber = _sytax[_index].LineNum
				};
			}
			_index += 2;
			CtdMessage ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj != token.TagetType && ctdMessage.Obj != DataTypeCtd.Object)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement add. Mising item",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement add.",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Execute statement add. Missing operator ')'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			_index++;
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operator",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(";"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement invoke. Missing ';'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			return new CtdMessage();
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Method not support for list data type",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	private CtdMessage ParsePushFunction()
	{
		if (_index >= _sytax.Count - 5)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement 'push'",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		PushState pushState = PushState.Begin;
		for (_index = _index; _index < _sytax.Count; _index++)
		{
			switch (pushState)
			{
			case PushState.Begin:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("("))
				{
					pushState = PushState.NameVariable;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Push statement must be begin with '('",
					LineNumber = _sytax[_index].LineNum
				};
			case PushState.NameVariable:
			{
				CtdMessage ctdMessage = ParseObj();
				_index--;
				if (string.IsNullOrEmpty(ctdMessage.Message))
				{
					if (ctdMessage.Obj == DataTypeCtd.String || ctdMessage.Obj == DataTypeCtd.None)
					{
						pushState = PushState.NameAccept;
						break;
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Variable naem of Push statement must be string",
						LineNumber = _sytax[_index].LineNum
					};
				}
				return ctdMessage;
			}
			case PushState.NameAccept:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(","))
				{
					pushState = PushState.Value;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Push statement missing operator ','",
					LineNumber = _sytax[_index].LineNum
				};
			case PushState.Value:
			{
				CtdMessage ctdMessage = ParseObj();
				_index--;
				if (string.IsNullOrEmpty(ctdMessage.Message))
				{
					pushState = PushState.Accept;
					break;
				}
				return ctdMessage;
			}
			case PushState.Accept:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(")"))
				{
					pushState = PushState.End;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Push statement missing operator ')'",
					LineNumber = _sytax[_index].LineNum
				};
			case PushState.End:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(";"))
				{
					return new CtdMessage();
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Push statement missing operator ';'",
					LineNumber = _sytax[_index].LineNum
				};
			}
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could not parse statement 'push'",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseWhile()
	{
		if (_index >= _sytax.Count - 2)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement 'while'",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		WhileState whileState = WhileState.BeginWhile;
		_index++;
		while (_index < _sytax.Count)
		{
			switch (whileState)
			{
			case WhileState.BeginWhile:
			{
				CtdMessage ctdMessage = ParseObj();
				_index--;
				if (string.IsNullOrEmpty(ctdMessage.Message))
				{
					if (ctdMessage.Obj == DataTypeCtd.Boolean)
					{
						whileState = WhileState.AcceptCondition;
						break;
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Condition statement while must be boolean",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				return ctdMessage;
			}
			case WhileState.AcceptCondition:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("{"))
				{
					whileState = WhileState.OpenQuotation;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = string.Format("Could not parse statement while. Missing '{0}'", "{"),
					LineNumber = _sytax[_index - 1].LineNum
				};
			case WhileState.OpenQuotation:
			{
				_variable.Add(new Token
				{
					Name = "0"
				});
				CtdMessage ctdMessage = ParseStatment(StatmentType.Loop);
				RemoveVariable(ref _variable);
				if (ctdMessage.Message.Equals("end"))
				{
					return new CtdMessage();
				}
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = string.Format("Could not parse statement while. Missing '{0}'", "{"),
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			}
			_index++;
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could not parse statement 'while'",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseShowMessageBox(bool ismessage = false)
	{
		if (_index > _sytax.Count - 7)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement ShowMessageBox",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		_index++;
		if (!_sytax[_index].Name.Equals("("))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement ShowMessageBox. Missing '('",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		_index++;
		CtdMessage ctdMessage = ParseObj();
		if (!string.IsNullOrEmpty(ctdMessage.Message))
		{
			return ctdMessage;
		}
		if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement ShowMessageBox. Missing ','",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		_index++;
		ctdMessage = ParseObj();
		if (!string.IsNullOrEmpty(ctdMessage.Message))
		{
			return ctdMessage;
		}
		if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(")"))
		{
			if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals(";"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse statement ShowMessageBox. Missing ';'",
					LineNumber = _sytax[_index].LineNum
				};
			}
			return new CtdMessage
			{
				Obj = DataTypeCtd.None
			};
		}
		if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement ShowMessageBox. Missing ','",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		_index++;
		if (_sytax[_index].Type != TypeSyntax.KeyWord || (!_sytax[_index].Name.ToLower().Equals("yesno") && !_sytax[_index].Name.ToLower().Equals("okcancel")))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement ShowMessageBox. Missing type mesabox",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		_index++;
		if (_sytax[_index].Type != TypeSyntax.Operator || !(_sytax[_index].Name == ")"))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement ShowMessageBox. Missing ','",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (ismessage && (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals(";")))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement ShowMessageBox. Missing ';'",
				LineNumber = _sytax[_index].LineNum
			};
		}
		return new CtdMessage
		{
			Obj = DataTypeCtd.Boolean
		};
	}

	public CtdMessage ParseDrillDown()
	{
		if (_index > _sytax.Count - 7)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement DrillDown",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		_index++;
		if (!_sytax[_index].Name.Equals("("))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement DrillDown. Missing '('",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		_index++;
		CtdMessage ctdMessage = ParseObj();
		if (!string.IsNullOrEmpty(ctdMessage.Message))
		{
			return ctdMessage;
		}
		if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement DrillDown. Missing ','",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		_index++;
		ctdMessage = ParseObj();
		if (!string.IsNullOrEmpty(ctdMessage.Message))
		{
			return ctdMessage;
		}
		if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement DrillDown. Missing ')'",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals(";"))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement DrillDown. Missing ';'",
				LineNumber = _sytax[_index].LineNum
			};
		}
		return new CtdMessage();
	}

	public CtdMessage ParseShowStatus()
	{
		if (_index > _sytax.Count - 4)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement ShowStatus",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		_index++;
		if (!_sytax[_index].Name.Equals("("))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement ShowStatus. Missing '('",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		_index++;
		CtdMessage ctdMessage = ParseObj();
		if (!string.IsNullOrEmpty(ctdMessage.Message))
		{
			return ctdMessage;
		}
		if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement ShowStatus. Missing ')'",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals(";"))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement ShowStatus. Missing ';'",
				LineNumber = _sytax[_index].LineNum
			};
		}
		return new CtdMessage();
	}

	public CtdMessage ParseStatmentFor()
	{
		if (_index >= _sytax.Count)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statement for. The statement is terminated",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		ForState forState = ForState.Begin;
		_index++;
		string name;
		while (_index < _sytax.Count)
		{
			switch (forState)
			{
			case ForState.Begin:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("("))
				{
					forState = ForState.ConditionStart;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "For statement missing '('",
					LineNumber = _sytax[_index - 1].LineNum
				};
			case ForState.ConditionStart:
				if (_sytax[_index].Type != TypeSyntax.Variable)
				{
					if (_sytax[_index].Type != TypeSyntax.KeyWord || (!_sytax[_index].Name.Equals("int") && !_sytax[_index].Name.Equals("var")))
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "For statement for must be contain count variable",
							LineNumber = _sytax[_index].LineNum
						};
					}
					_index++;
					if (_sytax[_index].Type != TypeSyntax.Variable)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "For statement for must be contain count variable",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					name = _sytax[_index].Name.ToLower();
					if (_variable.Find((Token t) => t.Name == name) != null)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "For statment. Variable _index had been exists",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					_variable.Add(new Token
					{
						Name = name,
						Type = TypeSyntax.Variable,
						DataType = DataTypeCtd.Int,
						Value = 0
					});
				}
				else
				{
					name = _sytax[_index].Name.ToLower();
					Token token = _variable.Find((Token t) => t.Name == name);
					if (token == null)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Count variable of statement for could not decalre.",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					if (token.DataType != DataTypeCtd.Int)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Count variable of statement must be integer",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
				}
				forState = ForState.Variable;
				break;
			case ForState.Variable:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(","))
				{
					forState = ForState.VariableBegin;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Count parse statement 'for'. Missing ','",
					LineNumber = _sytax[_index - 1].LineNum
				};
			case ForState.VariableBegin:
			{
				CtdMessage ctdMessage = ParseObj();
				_index--;
				if (string.IsNullOrEmpty(ctdMessage.Message))
				{
					if (ctdMessage.Obj == DataTypeCtd.Int)
					{
						forState = ForState.VariableAccept;
						break;
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse statement for. Begin variable must be interger",
						LineNumber = _sytax[_index].LineNum
					};
				}
				return ctdMessage;
			}
			case ForState.VariableAccept:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(","))
				{
					forState = ForState.VariableEnd;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Count parse statement 'for'. Missing ','",
					LineNumber = _sytax[_index - 1].LineNum
				};
			case ForState.VariableEnd:
			{
				CtdMessage ctdMessage = ParseObj();
				_index--;
				if (string.IsNullOrEmpty(ctdMessage.Message))
				{
					if (ctdMessage.Obj == DataTypeCtd.Int)
					{
						forState = ForState.VariableEndAccept;
						break;
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse statement for. End variable must be interger",
						LineNumber = _sytax[_index].LineNum
					};
				}
				return ctdMessage;
			}
			case ForState.VariableEndAccept:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(","))
				{
					forState = ForState.VariableAdd;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Count parse statement 'for'. Missing ','",
					LineNumber = _sytax[_index - 1].LineNum
				};
			case ForState.VariableAdd:
			{
				CtdMessage ctdMessage = ParseObj();
				_index--;
				if (string.IsNullOrEmpty(ctdMessage.Message))
				{
					if (ctdMessage.Obj == DataTypeCtd.Int)
					{
						forState = ForState.VariableAddAccept;
						break;
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse statement for. Add variable must be interger",
						LineNumber = _sytax[_index].LineNum
					};
				}
				return ctdMessage;
			}
			case ForState.VariableAddAccept:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(")"))
				{
					forState = ForState.AcceptCondition;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Count parse statement 'for'. Missing ')'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			case ForState.AcceptCondition:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("{"))
				{
					forState = ForState.BeginStatment;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Count parse statement 'for'. Missing '{'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			case ForState.BeginStatment:
			{
				_variable.Add(new Token
				{
					Name = "0"
				});
				CtdMessage ctdMessage = ParseStatment(StatmentType.Loop);
				RemoveVariable(ref _variable);
				if (ctdMessage.Message.Equals("end"))
				{
					return new CtdMessage();
				}
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Count parse statement 'for'. Missing '}'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			}
			_index++;
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Count parse statement 'for'",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseStatmentIf(StatmentType type)
	{
		IfState ifState = IfState.BeginIf;
		if (_index >= _sytax.Count - 2)
		{
			return new CtdMessage
			{
				Message = "Could not parse statement if",
				Type = MessageType.ERROR,
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		_index++;
		while (_index < _sytax.Count)
		{
			switch (ifState)
			{
			case IfState.BeginIf:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("("))
				{
					ifState = IfState.StartCondition;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "IF statement missing '('",
					LineNumber = _sytax[_index - 1].LineNum
				};
			case IfState.StartCondition:
			{
				CtdMessage ctdMessage2 = ParseObj();
				_index--;
				if (!string.IsNullOrEmpty(ctdMessage2.Message))
				{
					return ctdMessage2;
				}
				if (ctdMessage2.Obj == DataTypeCtd.Boolean)
				{
					ifState = IfState.AcceptCondition;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "IF condition must be Comparative statement",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			case IfState.AcceptCondition:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(")"))
				{
					ifState = IfState.StartStatment;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "IF statement missing ')'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			case IfState.StartStatment:
				if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("{"))
				{
					ifState = IfState.OpenQuotation;
					break;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "IF statement missing '{'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			case IfState.OpenQuotation:
			{
				_variable.Add(new Token
				{
					Name = "0"
				});
				CtdMessage ctdMessage = ParseStatment(type);
				RemoveVariable(ref _variable);
				if (ctdMessage.Message.Equals("end"))
				{
					if (_index < _sytax.Count && _sytax[_index].Type == TypeSyntax.KeyWord && _sytax[_index].Name.ToLower().Equals("else"))
					{
						if (_index < _sytax.Count - 1 && _sytax[_index + 1].Type == TypeSyntax.KeyWord && _sytax[_index + 1].Name.Equals("if"))
						{
							_index++;
							return ParseStatmentIf(type);
						}
						if (_index < _sytax.Count - 1 && _sytax[_index + 1].Type == TypeSyntax.Operator && _sytax[_index + 1].Name.Equals("{"))
						{
							_variable.Add(new Token
							{
								Name = "0"
							});
							_index += 2;
							ctdMessage = ParseStatment(type);
							RemoveVariable(ref _variable);
							if (ctdMessage.Message.Equals("end"))
							{
								_index--;
								return new CtdMessage();
							}
							if (!string.IsNullOrEmpty(ctdMessage.Message))
							{
								return ctdMessage;
							}
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Statement If must be start with '{'",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					_index--;
					return new CtdMessage();
				}
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse statement If. Missing operator '}'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			}
			_index++;
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could not parse statement If",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseStatmentAssignItem()
	{
		CtdMessage dataFromItem = GetDataFromItem();
		if (!string.IsNullOrEmpty(dataFromItem.Message))
		{
			return dataFromItem;
		}
		if (dataFromItem.Action == ActionStatment.GetData)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Can not assign data to get mothod",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (dataFromItem.Action == ActionStatment.ShowHide)
		{
			if (_index < _sytax.Count - 2 && _sytax[_index].Type != TypeSyntax.Operator && _sytax[_index].Name.Equals("(") && _sytax[_index + 1].Type != TypeSyntax.Operator && _sytax[_index + 1].Name.Equals(")"))
			{
				_index += 2;
			}
			if (_index > _sytax.Count - 1 || (_sytax[_index].Type != TypeSyntax.Operator && _sytax[_index].Name.Equals(";")))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse show/hide method. Missing operation ';'",
					LineNumber = _sytax[_index].LineNum
				};
			}
			return new CtdMessage();
		}
		if (dataFromItem.Action == ActionStatment.NewRow)
		{
			if (_index < _sytax.Count - 2 && _sytax[_index].Type != TypeSyntax.Operator && _sytax[_index].Name.Equals("(") && _sytax[_index + 1].Type != TypeSyntax.Operator && _sytax[_index + 1].Name.Equals(")"))
			{
				_index += 2;
			}
			if (_index > _sytax.Count - 1 || (_sytax[_index].Type != TypeSyntax.Operator && _sytax[_index].Name.Equals(";")))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse show/hide method. Missing operation ';'",
					LineNumber = _sytax[_index].LineNum
				};
			}
			return new CtdMessage();
		}
		if (dataFromItem.Action == ActionStatment.Clear || dataFromItem.Action == ActionStatment.AllSelection)
		{
			if (_index < _sytax.Count - 2 && _sytax[_index].Type != TypeSyntax.Operator && _sytax[_index].Name.Equals("(") && _sytax[_index + 1].Type != TypeSyntax.Operator && _sytax[_index + 1].Name.Equals(")"))
			{
				_index += 2;
			}
			if (_index > _sytax.Count - 1 || (_sytax[_index].Type != TypeSyntax.Operator && _sytax[_index].Name.Equals(";")))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse method. Missing operation ';'",
					LineNumber = _sytax[_index].LineNum
				};
			}
			return new CtdMessage();
		}
		if (dataFromItem.Action == ActionStatment.Focus)
		{
			if (_index < _sytax.Count - 2 && _sytax[_index].Type != TypeSyntax.Operator && _sytax[_index].Name.Equals("(") && _sytax[_index + 1].Type != TypeSyntax.Operator && _sytax[_index + 1].Name.Equals(")"))
			{
				_index += 2;
			}
			if (_index > _sytax.Count - 1 || (_sytax[_index].Type != TypeSyntax.Operator && _sytax[_index].Name.Equals(";")))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse show/hide method. Missing operation ';'",
					LineNumber = _sytax[_index].LineNum
				};
			}
			return new CtdMessage();
		}
		if (dataFromItem.Obj == DataTypeCtd.Dynamic)
		{
			_index--;
			return new CtdMessage();
		}
		if (dataFromItem.Obj == DataTypeCtd.ColumnData || dataFromItem.Action == ActionStatment.SetReadOnlyOrEditable || dataFromItem.Action == ActionStatment.SetColor)
		{
			if (_index > _sytax.Count - 2)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse statment. Missing paramater",
					LineNumber = _sytax[_index].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("="))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse statment. Missing operator",
					LineNumber = _sytax[_index].LineNum
				};
			}
			_index++;
			if (dataFromItem.Action == ActionStatment.SetColor && _index < _sytax.Count - 1 && _sytax[_index].Type == TypeSyntax.KeyWord && ConstantsApzCtd.ColorKeyWork.Contains(_sytax[_index].Name) && _sytax[_index + 1].Type == TypeSyntax.Operator && _sytax[_index + 1].Name.Equals(";"))
			{
				_index++;
				return new CtdMessage();
			}
			dataFromItem = ParseObj(isColumnFormular: true);
			if (!string.IsNullOrEmpty(dataFromItem.Message))
			{
				return dataFromItem;
			}
			if (dataFromItem.Action == ActionStatment.SetReadOnlyOrEditable && dataFromItem.Obj != DataTypeCtd.Boolean)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Patamater must be boolean",
					LineNumber = _sytax[_index].LineNum
				};
			}
			return new CtdMessage();
		}
		if (_index > _sytax.Count - 1)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse assign statment. Missing operation ';'",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (dataFromItem.Obj == DataTypeCtd.None)
		{
			if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(";"))
			{
				return new CtdMessage();
			}
			if (_index < _sytax.Count - 3 && _sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("(") && _sytax[_index + 1].Type == TypeSyntax.Operator && _sytax[_index + 1].Name.Equals(")") && _sytax[_index + 2].Type == TypeSyntax.Operator && _sytax[_index + 2].Name.Equals(";"))
			{
				_index += 2;
				return new CtdMessage();
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = string.Format("Could not parse assign statment. Missing operation '{0}'", ";"),
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("="))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse assign statment. Missing operation '='",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		_index++;
		CtdMessage ctdMessage = ParseObj();
		if (!string.IsNullOrEmpty(ctdMessage.Message))
		{
			return ctdMessage;
		}
		_index--;
		switch (dataFromItem.Obj)
		{
		case DataTypeCtd.DataTable:
			if (ctdMessage.Obj != DataTypeCtd.DataTable)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = $"Could not parse assign statment. Could not assign {ControlFunction.ToString(dataFromItem.Obj)} to datatable",
					LineNumber = _sytax[_index].LineNum
				};
			}
			return new CtdMessage();
		case DataTypeCtd.DataRow:
			if (ctdMessage.Obj != DataTypeCtd.DataRow)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = $"Could not parse assign statment. Could not assign {ControlFunction.ToString(dataFromItem.Obj)} to datarow",
					LineNumber = _sytax[_index].LineNum
				};
			}
			return new CtdMessage();
		case DataTypeCtd.Object:
			if (ctdMessage.Obj == DataTypeCtd.DataTable || ctdMessage.Obj == DataTypeCtd.DataSet || ctdMessage.Obj == DataTypeCtd.DataRow || ctdMessage.Obj == DataTypeCtd.List)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = $"Could not parse assign statment. Could not assign {ControlFunction.ToString(dataFromItem.Obj)} to column",
					LineNumber = _sytax[_index].LineNum
				};
			}
			return new CtdMessage();
		default:
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse assign statment. Could not get datatype to assign",
				LineNumber = _sytax[_index].LineNum
			};
		}
	}

	public CtdMessage ParseAssign()
	{
		if (_index >= _sytax.Count - 1)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not assign variable",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(ControlFunction.ToString("$")))
		{
			_index++;
			return ParseStatmentAssignItem();
		}
		if (_sytax[_index].Type == TypeSyntax.KeyWord && _sytax[_index].Name.ToLower().Equals("cancelevent"))
		{
			if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("="))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not assign. Missing operator '='",
					LineNumber = _sytax[_index].LineNum
				};
			}
			_index += 2;
			CtdMessage ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj != DataTypeCtd.Boolean)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "CancelEvent must be boolean",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			return new CtdMessage();
		}
		if (_sytax[_index].Type == TypeSyntax.Variable)
		{
			string name = _sytax[_index].Name.ToLower();
			Token token = _variable.Find((Token t) => t.Name == name);
			if (token != null)
			{
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("="))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not assign. Missing operator '='",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj(isColumnFormular: false, token.DataType);
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj == DataTypeCtd.None)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not assign. Could not get type value",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				switch (token.DataType)
				{
				case DataTypeCtd.Dynamic:
					if (ctdMessage.Obj == DataTypeCtd.None || ctdMessage.Obj == DataTypeCtd.Dynamic)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Statement declare variable. ",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					token.DataType = ctdMessage.Obj;
					return new CtdMessage();
				case DataTypeCtd.List:
					if (token.DataType != ctdMessage.Obj)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Statement declare. Data type can not target type for list!",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					return new CtdMessage();
				case DataTypeCtd.DateTime:
					if (ctdMessage.Obj != DataTypeCtd.DateTime && ctdMessage.Obj != DataTypeCtd.Object && ctdMessage.Obj != DataTypeCtd.String)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Cannot parse object to datetime data type",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					return new CtdMessage();
				default:
					if (token.DataType == ctdMessage.Obj || ctdMessage.Obj == DataTypeCtd.Object)
					{
						return new CtdMessage();
					}
					if (token.DataType == DataTypeCtd.Double && ctdMessage.Obj == DataTypeCtd.Int)
					{
						return new CtdMessage();
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Statement declare. Data type can not target type for " + token.DataType,
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
			}
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could not parse statment. Begin statement declare must be variable or system variable",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseDeclare()
	{
		if (_index > _sytax.Count - 2)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not declare. The assignment statement has ended",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		DeclareState declareState = DeclareState.Begin;
		DataTypeCtd dataTypeCtd = DataTypeCtd.Dynamic;
		DataTypeCtd tagetType = DataTypeCtd.DataTable;
		string name;
		for (_index = _index; _index < _sytax.Count; _index++)
		{
			switch (declareState)
			{
			case DeclareState.Begin:
				if (_sytax[_index].Name.ToLower().Equals("var"))
				{
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("bool"))
				{
					dataTypeCtd = DataTypeCtd.Boolean;
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("double"))
				{
					dataTypeCtd = DataTypeCtd.Double;
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("int"))
				{
					dataTypeCtd = DataTypeCtd.Int;
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("string"))
				{
					dataTypeCtd = DataTypeCtd.String;
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("datetime"))
				{
					dataTypeCtd = DataTypeCtd.DateTime;
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("date"))
				{
					dataTypeCtd = DataTypeCtd.DateTime;
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("datarow"))
				{
					dataTypeCtd = DataTypeCtd.DataRow;
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("datatable"))
				{
					dataTypeCtd = DataTypeCtd.DataTable;
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("dataset"))
				{
					dataTypeCtd = DataTypeCtd.DataSet;
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("httpresult"))
				{
					dataTypeCtd = DataTypeCtd.HttpResult;
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("dialog"))
				{
					dataTypeCtd = DataTypeCtd.Dialog;
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("form"))
				{
					dataTypeCtd = DataTypeCtd.Form;
					declareState = DeclareState.DeclareVariable;
					break;
				}
				if (_sytax[_index].Name.ToLower().Equals("list"))
				{
					if (_index > _sytax.Count - 3)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Could not declare. The assignment statement has ended",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					if (_sytax[_index + 1].Type == TypeSyntax.Operator && _sytax[_index + 1].Name.Equals("<") && _sytax[_index + 2].Type == TypeSyntax.KeyWord && ConstantsApzCtd.TypeBase.Contains(_sytax[_index + 2].Name) && _sytax[_index + 3].Type == TypeSyntax.Operator && _sytax[_index + 3].Name.Equals(">"))
					{
						switch (_sytax[_index + 2].Name.ToLower())
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
						dataTypeCtd = DataTypeCtd.List;
						_index += 3;
						declareState = DeclareState.DeclareVariable;
						break;
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not declare. The assignment statement has ended",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not declare. The assignment statement has ended",
					LineNumber = _sytax[_index - 1].LineNum
				};
			case DeclareState.DeclareVariable:
				if (_sytax[_index].Type != TypeSyntax.Variable)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not declare. Must be declare for variable",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				name = _sytax[_index].Name.ToLower();
				if (_variable.Find((Token t) => t.Name == name) != null)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not declare. Could not declare duplicate variable",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
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
				declareState = DeclareState.DecalreVariableEnd;
				break;
			case DeclareState.DecalreVariableEnd:
				if (_sytax[_index].Type == TypeSyntax.Operator)
				{
					if (_sytax[_index].Name.Equals(ControlFunction.ToString(";")))
					{
						if (dataTypeCtd == DataTypeCtd.Dynamic)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Could not declare. Missing value for variable",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						return new CtdMessage();
					}
					if (_sytax[_index].Name.Equals("="))
					{
						_index--;
						return ParseAssign();
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not declare. Missing '='",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not declare. Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could not declare. The assignment statement has ended",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseAsignList(ref DataTypeCtd type)
	{
		if (_index >= _sytax.Count - 1)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not assign to list. Missing operation",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Type == TypeSyntax.Operator)
		{
			if (_sytax[_index].Name.Equals("="))
			{
				_index++;
				if (_sytax[_index].Type == TypeSyntax.Variable)
				{
					string name = _sytax[_index].Name.ToLower();
					Token token = _variable.Find((Token t) => t.Name == name);
					if (token != null)
					{
						if (token.DataType == DataTypeCtd.List && (token.TagetType == type || type == DataTypeCtd.Object) && _index < _sytax.Count - 1 && _sytax[_index + 1].Type == TypeSyntax.Operator && _sytax[_index + 1].Name.Equals(";"))
						{
							type = token.TagetType;
							_index++;
							return new CtdMessage();
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Could not assign to list. Missing operation",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not assign to list. Variable doest not exists",
						LineNumber = _sytax[_index].LineNum
					};
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not assign to list. Missing variable to assign",
					LineNumber = _sytax[_index].LineNum
				};
			}
			if (_sytax[_index].Name.Equals("["))
			{
				_index++;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not assign to list. _index must be integer",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_index < _sytax.Count - 2 && _sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("]") && _sytax[_index + 1].Type == TypeSyntax.Operator && _sytax[_index + 1].Name.Equals("]"))
				{
					_index += 2;
					ctdMessage = ParseObj();
					if (!string.IsNullOrEmpty(ctdMessage.Message))
					{
						return ctdMessage;
					}
					if (_index < _sytax.Count - 2 && _sytax[_index + 1].Type == TypeSyntax.KeyWord && _sytax[_index + 1].Name.ToLower().Equals("."))
					{
						_index += 2;
						return new CtdMessage();
					}
					if (ctdMessage.Obj == DataTypeCtd.None)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Could not get type value to assign",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					if (type == DataTypeCtd.Double)
					{
						if (ctdMessage.Obj == DataTypeCtd.Double || ctdMessage.Obj == DataTypeCtd.Int || ctdMessage.Obj == DataTypeCtd.Object)
						{
							return new CtdMessage();
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Data type must be double, integer or object to assing for double variable",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					if (type == DataTypeCtd.Int)
					{
						if (ctdMessage.Obj == DataTypeCtd.Object || ctdMessage.Obj == DataTypeCtd.Int)
						{
							return new CtdMessage();
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Data type must be integer or object to assing for integer variable",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					if (type == DataTypeCtd.Boolean)
					{
						if (ctdMessage.Obj != DataTypeCtd.Boolean)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Data type must be boolean to assing for boolean variable",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						return new CtdMessage();
					}
					if (type == DataTypeCtd.String)
					{
						if (ctdMessage.Obj == DataTypeCtd.Boolean)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Could not assign boolean to string variable",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						return new CtdMessage();
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not get type variable to assign",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse statment. Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Name.Equals(".") && _index < _sytax.Count - 1 && _sytax[_index + 1].Type == TypeSyntax.KeyWord && _sytax[_index + 1].Name.ToLower().Equals("add"))
			{
				_index += 2;
				CtdMessage ctdMessage2 = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage2.Message))
				{
					return ctdMessage2;
				}
				if (ctdMessage2.Obj == DataTypeCtd.None)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not get type value to assign",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (type == DataTypeCtd.Double)
				{
					if (ctdMessage2.Obj == DataTypeCtd.Double || ctdMessage2.Obj == DataTypeCtd.Int || ctdMessage2.Obj == DataTypeCtd.Object)
					{
						return new CtdMessage();
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Data type must be double, integer or object to assing for double variable",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (type == DataTypeCtd.Int)
				{
					if (ctdMessage2.Obj == DataTypeCtd.Object || ctdMessage2.Obj == DataTypeCtd.Int)
					{
						return new CtdMessage();
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Data type must be integer or object to assing for integer variable",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (type == DataTypeCtd.Boolean)
				{
					if (ctdMessage2.Obj != DataTypeCtd.Boolean)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Data type must be boolean to assing for boolean variable",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					return new CtdMessage();
				}
				if (type == DataTypeCtd.String)
				{
					if (ctdMessage2.Obj == DataTypeCtd.Boolean)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Could not assign boolean to string variable",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					return new CtdMessage();
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not get type variable to assign",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statment. Missing operation",
				LineNumber = _sytax[_index].LineNum
			};
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could not parse statment. Missing operation",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseAsignDataRow()
	{
		if (_index >= _sytax.Count - 1)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse assign datarow",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Type != TypeSyntax.Operator)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse object",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Name.Equals("="))
		{
			_index++;
			CtdMessage ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj == DataTypeCtd.Object || ctdMessage.Obj == DataTypeCtd.DataRow)
			{
				return new CtdMessage();
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = $"Could assign tatatype {ControlFunction.ToString(ctdMessage.Obj)} to datarow",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Name.Equals("["))
		{
			_index++;
			CtdMessage ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj != DataTypeCtd.String)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse assign statment.Field name nust be string",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_index < _sytax.Count - 1 && _sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("]") && _sytax[_index + 1].Type == TypeSyntax.Operator && _sytax[_index + 1].Name.Equals("="))
			{
				_index += 2;
				ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.None && _index < _sytax.Count - 1 && _sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(ControlFunction.ToString(";")))
				{
					_index++;
					return new CtdMessage();
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse assign statment. Mising ';'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse assign statment. Mising '['",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could not parse assign statment. Mising operation",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage GetDataFromItem()
	{
		if (_index > _sytax.Count - 2)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse assign statment. Missing operation",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("["))
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Missing operation '['",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		Token token = new Token();
		_index++;
		if (_sytax[_index].Type == TypeSyntax.Constant)
		{
			token = _sytax[_index++];
		}
		else
		{
			token.Type = TypeSyntax.Variable;
			while (_index < _sytax.Count && !_sytax[_index].Name.Equals("]"))
			{
				token.Name += _sytax[_index++].Name;
			}
		}
		if (_index == _sytax.Count)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse assign statment. Missing operation ']'",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_form.FormId.ToLower().Equals(token.Name.ToLower()))
		{
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("]"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operation ']'",
					LineNumber = _sytax[_index].LineNum
				};
			}
			_index++;
			if (_index == _sytax.Count)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operation '.'",
					LineNumber = _sytax[_index].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("."))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operation '.'",
					LineNumber = _sytax[_index].LineNum
				};
			}
			_index++;
			if (_index == _sytax.Count)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Can not parse statment. Missing operation '[' or function to get data",
					LineNumber = _sytax[_index].LineNum
				};
			}
			if (_sytax[_index].Type == TypeSyntax.KeyWord && _sytax[_index].Name.Equals("columnedit"))
			{
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.String
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("["))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operation '['",
					LineNumber = _sytax[_index].LineNum
				};
			}
			_index++;
			if (_index == _sytax.Count)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Can not parse statment. Missing paramater ItemId",
					LineNumber = _sytax[_index].LineNum
				};
			}
			if (_sytax[_index].Type == TypeSyntax.Constant)
			{
				token = _sytax[_index++];
			}
			else
			{
				token.Type = TypeSyntax.Variable;
				while (_index < _sytax.Count && !_sytax[_index].Name.Equals("]"))
				{
					token.Name += _sytax[_index++].Name;
				}
			}
			if (_index == _sytax.Count)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse assign statment. Missing operation ']'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
		}
		if (token.Type == TypeSyntax.Constant)
		{
			if (ControlFunction.CheckExistControl(_form, token.Name) != null)
			{
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("]"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Missing operation ']'",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index++;
				if (_index == _sytax.Count || _sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("."))
				{
					return new CtdMessage
					{
						Obj = DataTypeCtd.Object
					};
				}
				_index++;
				if (_index == _sytax.Count)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type == TypeSyntax.KeyWord)
				{
					_index++;
					switch (_sytax[_index - 1].Name.ToLower())
					{
					case "show":
					case "hide":
						return new CtdMessage
						{
							Obj = DataTypeCtd.None,
							Action = ActionStatment.ShowHide
						};
					case "focus":
						return new CtdMessage
						{
							Obj = DataTypeCtd.None,
							Action = ActionStatment.Focus
						};
					case "readonly":
					case "enable":
						return new CtdMessage
						{
							Obj = DataTypeCtd.Boolean,
							Action = ActionStatment.SetReadOnlyOrEditable
						};
					case "color":
						return new CtdMessage
						{
							Obj = DataTypeCtd.None,
							Action = ActionStatment.SetColor
						};
					case "string":
						return new CtdMessage
						{
							Obj = DataTypeCtd.String,
							Action = ActionStatment.GetData
						};
					case "int":
						return new CtdMessage
						{
							Obj = DataTypeCtd.Int,
							Action = ActionStatment.GetData
						};
					case "double":
						return new CtdMessage
						{
							Obj = DataTypeCtd.Double,
							Action = ActionStatment.GetData
						};
					case "bool":
						return new CtdMessage
						{
							Obj = DataTypeCtd.Boolean,
							Action = ActionStatment.GetData
						};
					case "datetime":
						return new CtdMessage
						{
							Obj = DataTypeCtd.DateTime,
							Action = ActionStatment.GetData
						};
					case "date":
						return new CtdMessage
						{
							Obj = DataTypeCtd.DateTime,
							Action = ActionStatment.GetData
						};
					}
				}
			}
			return new CtdMessage
			{
				Type = MessageType.WARNING,
				Message = "Can not getdata form item",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (token.Type == TypeSyntax.Variable)
		{
			if (ControlFunction.CheckExistGrid(_lstGridView, token.Name) == null)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Item does not exist in form",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("]"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operation ']'",
					LineNumber = _sytax[_index].LineNum
				};
			}
			_index++;
			if (_sytax[_index].Type != TypeSyntax.Operator)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (!_sytax[_index].Name.Equals("."))
			{
				return new CtdMessage
				{
					Obj = DataTypeCtd.DataTable
				};
			}
			if (_index > _sytax.Count - 2)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse assign statment. Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			_index++;
			CtdMessage ctdMessage = CheckFunctionItem();
			if (string.IsNullOrEmpty(ctdMessage.Message) || ctdMessage.Type == MessageType.ERROR)
			{
				return ctdMessage;
			}
			if (_index > _sytax.Count - 2)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse assign statment. Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("["))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operation '['",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			_index++;
			ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj == DataTypeCtd.String)
			{
				if (_index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("]"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ']'",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.ColumnData
				};
			}
			if (ctdMessage.Obj != DataTypeCtd.Int)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse assign statment. Row _index must be integer",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_index > _sytax.Count - 3)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse assign statment. Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("]"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operation ']'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index + 1].Type != TypeSyntax.Operator)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operation",
					LineNumber = _sytax[_index].LineNum
				};
			}
			if (!_sytax[_index + 1].Name.Equals("."))
			{
				_index += 2;
				return new CtdMessage
				{
					Obj = DataTypeCtd.DataRow
				};
			}
			if (_sytax[_index + 2].Type == TypeSyntax.KeyWord && _sytax[_index + 2].Name.Equals("isselectedrow"))
			{
				_index += 3;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Boolean
				};
			}
			if (_sytax[_index + 2].Type == TypeSyntax.KeyWord && _sytax[_index + 2].Name.Equals("focus"))
			{
				_index += 3;
				return new CtdMessage
				{
					Action = ActionStatment.Focus,
					Obj = DataTypeCtd.None
				};
			}
			if (_sytax[_index + 2].Type == TypeSyntax.KeyWord && _sytax[_index + 2].Name.Equals("delete"))
			{
				_index += 3;
				return new CtdMessage
				{
					Action = ActionStatment.Delete,
					Obj = DataTypeCtd.None
				};
			}
			if (_sytax[_index + 2].Type == TypeSyntax.KeyWord && _sytax[_index + 2].Name.Equals("selected"))
			{
				_index += 3;
				return new CtdMessage
				{
					Action = ActionStatment.Selected,
					Obj = DataTypeCtd.None
				};
			}
			if (_sytax[_index + 2].Type == TypeSyntax.KeyWord && _sytax[_index + 2].Name.Equals("unselected"))
			{
				_index += 3;
				return new CtdMessage
				{
					Action = ActionStatment.UnSelected,
					Obj = DataTypeCtd.None
				};
			}
			if (_sytax[_index + 2].Type != TypeSyntax.Operator || !_sytax[_index + 2].Name.Equals("["))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operation '['",
					LineNumber = _sytax[_index + 1].LineNum
				};
			}
			_index += 3;
			ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj == DataTypeCtd.String)
			{
				if (_index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("]"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ']'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("."))
				{
					_index++;
					return new CtdMessage
					{
						Obj = DataTypeCtd.Object
					};
				}
				_index += 2;
				if (_index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type == TypeSyntax.KeyWord)
				{
					switch (_sytax[_index].Name.ToLower())
					{
					case "string":
						_index++;
						return new CtdMessage
						{
							Obj = DataTypeCtd.String,
							Action = ActionStatment.GetData
						};
					case "int":
						_index++;
						return new CtdMessage
						{
							Obj = DataTypeCtd.Int,
							Action = ActionStatment.GetData
						};
					case "double":
						_index++;
						return new CtdMessage
						{
							Obj = DataTypeCtd.Double,
							Action = ActionStatment.GetData
						};
					case "bool":
						_index++;
						return new CtdMessage
						{
							Obj = DataTypeCtd.Boolean,
							Action = ActionStatment.GetData
						};
					case "datetime":
						_index++;
						return new CtdMessage
						{
							Obj = DataTypeCtd.DateTime,
							Action = ActionStatment.GetData
						};
					case "date":
						_index++;
						return new CtdMessage
						{
							Obj = DataTypeCtd.DateTime,
							Action = ActionStatment.GetData
						};
					case "focus":
						_index++;
						if (_index >= _sytax.Count)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Could not parse assign statment. missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("("))
						{
							_index++;
							if (_index >= _sytax.Count - 1)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Could not parse assign statment. missing operator",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
							if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Could not parse assign statment. missing operator ')'",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
							_index++;
						}
						return new CtdMessage
						{
							Obj = DataTypeCtd.None
						};
					}
				}
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse assign statment. Datatype did not support",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could not parse assign statment. Missing operation",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage CheckFunctionItem()
	{
		if (_sytax == null)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse assign statment. Missing operation",
				LineNumber = 1
			};
		}
		if (_index > _sytax.Count - 1)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse assign statment. Missing operation",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Type == TypeSyntax.KeyWord)
		{
			if (_sytax[_index].Name.ToLower().Equals("focusrow"))
			{
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Int
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("cancelnextcellvaluechangedevent") || _sytax[_index].Name.ToLower().Equals("cancelnextfocusobjectchangedevent") || _sytax[_index].Name.ToLower().Equals("cancelnextselectedchangedevent"))
			{
				_index++;
				if (_index > _sytax.Count - 3)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = $"Could not parse assign statement {_sytax[_index - 1].Name.ToLower()}. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("="))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index++;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj == DataTypeCtd.Boolean)
				{
					return new CtdMessage
					{
						Obj = DataTypeCtd.None
					};
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "DataType must be boolean",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("columnedit"))
			{
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.String
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("rows"))
			{
				if (_index > _sytax.Count - 3)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("."))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '.'",
						LineNumber = _sytax[_index].LineNum
					};
				}
				if (_sytax[_index + 2].Type != TypeSyntax.KeyWord || !_sytax[_index + 2].Name.Equals("count"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '.'",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 3;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Int
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("columns"))
			{
				if (_index > _sytax.Count - 3)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type == TypeSyntax.Operator)
				{
					if (_sytax[_index + 1].Name.Equals("."))
					{
						if (_sytax[_index + 2].Type == TypeSyntax.KeyWord && _sytax[_index + 2].Name.Equals("contains"))
						{
							_index += 4;
							CtdMessage ctdMessage2 = ParseObj();
							if (!string.IsNullOrEmpty(ctdMessage2.Message))
							{
								return ctdMessage2;
							}
							if (ctdMessage2.Obj != DataTypeCtd.String)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Datatype of paramater must be string",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
							if (_index < _sytax.Count && _sytax[_index].Name.ToLower().Equals(")"))
							{
								_index++;
								return new CtdMessage
								{
									Obj = DataTypeCtd.Boolean
								};
							}
						}
						if (_sytax[_index + 2].Type != TypeSyntax.KeyWord || !_sytax[_index + 2].Name.Equals("count"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Could not parse assign statment. Function just be column.count",
								LineNumber = _sytax[_index].LineNum
							};
						}
						_index += 3;
						return new CtdMessage
						{
							Obj = DataTypeCtd.Int
						};
					}
					if (_sytax[_index + 1].Name.Equals("["))
					{
						_index += 2;
						CtdMessage ctdMessage = ParseObj();
						if (!string.IsNullOrEmpty(ctdMessage.Message))
						{
							return ctdMessage;
						}
						if (ctdMessage.Obj != DataTypeCtd.String && ctdMessage.Obj != DataTypeCtd.Int)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Datatype to access column be not match",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index >= _sytax.Count || !_sytax[_index].Name.Equals("]"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Can not parse statment. Missing operation ']'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("."))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Could not parse assign statment. Missing operation '.'",
								LineNumber = _sytax[_index].LineNum
							};
						}
						if (_sytax[_index + 2].Type != TypeSyntax.KeyWord || (!_sytax[_index + 2].Name.Equals("hide") && !_sytax[_index + 2].Name.Equals("show")))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Could not parse assign statment. Function just be show/hide",
								LineNumber = _sytax[_index].LineNum
							};
						}
						_index += 3;
						return new CtdMessage
						{
							Obj = DataTypeCtd.None
						};
					}
				}
			}
			if (_sytax[_index].Name.ToLower().Equals("selectedrows"))
			{
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.DataTable
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("firstduplicate"))
			{
				if (_index > _sytax.Count - 4)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.String || _index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing fieldId for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Int
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("stringagg"))
			{
				if (_index > _sytax.Count - 4)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.String || _index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing fieldId for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ','",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.String || _index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing seperator for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.String
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("focusedcolumn"))
			{
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.String
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("newrow"))
			{
				if (_index > _sytax.Count - 3)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Int,
					Action = ActionStatment.NewRow
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("min") || _sytax[_index].Name.ToLower().Equals("max") || _sytax[_index].Name.ToLower().Equals("avg") || _sytax[_index].Name.ToLower().Equals("sum"))
			{
				if (_index > _sytax.Count - 4)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.String || _index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing fieldId for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Double
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("sumcolumns"))
			{
				if (_index > _sytax.Count - 4)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int || _index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing row _index for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ','",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.String || _index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing fieldId for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Double
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("sumrowscolumns"))
			{
				if (_index > _sytax.Count - 4)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.String || _index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing fieldId for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Double
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("setrowcellvalue"))
			{
				if (_index > _sytax.Count - 8)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int || _index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing row for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ','",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.String || _index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing fieldIf for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ','",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index++;
				ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int && ctdMessage.Obj != DataTypeCtd.Numeric && ctdMessage.Obj != DataTypeCtd.Object && ctdMessage.Obj != DataTypeCtd.String && ctdMessage.Obj != DataTypeCtd.Double && ctdMessage.Obj != DataTypeCtd.Boolean)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = $"Could not parse assign statment. Could not use datatype {ControlFunction.ToString(ctdMessage.Obj)} to column",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.None
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("setdatasourcecell"))
			{
				if (_index > _sytax.Count - 8)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int || _index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing row for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ','",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.String || _index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing fieldIf for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ','",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index++;
				ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int && ctdMessage.Obj != DataTypeCtd.Numeric && ctdMessage.Obj != DataTypeCtd.Object && ctdMessage.Obj != DataTypeCtd.String && ctdMessage.Obj != DataTypeCtd.Double && ctdMessage.Obj != DataTypeCtd.Boolean)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = $"Could not parse assign statment. Could not use datatype {ControlFunction.ToString(ctdMessage.Obj)} to column",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.None
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("setfocusrowcellvalue"))
			{
				if (_index > _sytax.Count - 5)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.String || _index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing fieldIf for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ','",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int && ctdMessage.Obj != DataTypeCtd.Numeric && ctdMessage.Obj != DataTypeCtd.Object && ctdMessage.Obj != DataTypeCtd.String && ctdMessage.Obj != DataTypeCtd.Double && ctdMessage.Obj != DataTypeCtd.Boolean)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = $"Could not parse assign statment. Could not use datatype {ControlFunction.ToString(ctdMessage.Obj)} to column",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.None
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("setfocusdatasourcecell"))
			{
				if (_index > _sytax.Count - 5)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.String || _index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing fieldIf for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ','",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int && ctdMessage.Obj != DataTypeCtd.Numeric && ctdMessage.Obj != DataTypeCtd.Object && ctdMessage.Obj != DataTypeCtd.String && ctdMessage.Obj != DataTypeCtd.Double && ctdMessage.Obj != DataTypeCtd.Boolean)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = $"Could not parse assign statment. Could not use datatype {ControlFunction.ToString(ctdMessage.Obj)} to column",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.None
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("getfocusrowcellvalue"))
			{
				if (_index > _sytax.Count - 4)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.String || _index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing fieldIf for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Object
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("getparentrowhandle"))
			{
				if (_index > _sytax.Count - 4)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int || _index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing row handle for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Int
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("getchildrowcount"))
			{
				if (_index > _sytax.Count - 4)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int || _index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing row handle for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Int
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("getrowcellvalue"))
			{
				if (_index > _sytax.Count - 8)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int || _index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing row for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ','",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.String || _index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing fieldIf for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Object
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("getchildrowhandle"))
			{
				if (_index > _sytax.Count - 8)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int || _index > _sytax.Count - 2)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing row for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ','",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if (ctdMessage.Obj != DataTypeCtd.Int || _index > _sytax.Count - 1)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing fieldIf for function",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Int
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("loop"))
			{
				_variable.Add(new Token
				{
					Name = "0"
				});
				if (_index > _sytax.Count - 6)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse 'Loop' statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse 'Loop' statment. Missing operation '('",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				if (_sytax[_index].Type != TypeSyntax.Variable)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse 'Loop' statment. Missing operation jump variable",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				string jumName = _sytax[_index].Name.ToLower();
				if (_variable.Find((Token t) => t.Name == jumName) != null)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse 'Loop' statment. Missing operation jump variable",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_variable.Add(new Token
				{
					Name = jumName,
					Type = TypeSyntax.Variable,
					DataType = DataTypeCtd.Int
				});
				_index++;
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse 'Loop' statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("{"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse 'Loop' statment. Missing operation '{'",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				CtdMessage ctdMessage = ParseStatment(StatmentType.Loop);
				RemoveVariable(ref _variable);
				if (ctdMessage.Message.Equals("end"))
				{
					return new CtdMessage
					{
						Obj = DataTypeCtd.Dynamic
					};
				}
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse 'Loop' statment",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("clear") || _sytax[_index].Name.ToLower().Equals("selectall") || _sytax[_index].Name.ToLower().Equals("clearselection"))
			{
				if (_index > _sytax.Count - 3)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation '('",
						LineNumber = _sytax[_index].LineNum
					};
				}
				_index += 2;
				if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Missing operation ')'",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Int,
					Action = (_sytax[_index].Name.ToLower().Equals("clear") ? ActionStatment.Clear : ActionStatment.AllSelection)
				};
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Missing operation '['",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		return new CtdMessage
		{
			Type = MessageType.WARNING,
			Message = "Statement not be function",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseAsignDataTable()
	{
		if (_index >= _sytax.Count - 1)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could parse statement assign datatable",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Type != TypeSyntax.Operator)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could parse statement assign datatable. Missing operation",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Name.Equals("="))
		{
			_index++;
			CtdMessage ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj == DataTypeCtd.Object || ctdMessage.Obj == DataTypeCtd.DataTable)
			{
				return new CtdMessage();
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = $"Could assign tatatype {ControlFunction.ToString(ctdMessage.Obj)} to datatable",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Name.Equals("["))
		{
			_index++;
			CtdMessage ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj != DataTypeCtd.Int)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement assign. Mising operation '['",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_index < _sytax.Count - 1 && _sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("]"))
			{
				_index++;
				return ParseAsignDataRow();
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could parse statement assign. Mising operation ']'",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could parse statement assign. Mising operation '['",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseAssignDataset()
	{
		if (_index >= _sytax.Count - 1)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statment. Missing operation",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Type != TypeSyntax.Operator)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statment. Missing operation",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Name.Equals("="))
		{
			_index++;
			CtdMessage ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj == DataTypeCtd.Object || ctdMessage.Obj == DataTypeCtd.DataSet)
			{
				return new CtdMessage();
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = $"Could not assign datatype {ControlFunction.ToString(ctdMessage.Obj)} to dataset",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Name.Equals("["))
		{
			_index++;
			CtdMessage ctdMessage2 = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage2.Message))
			{
				return ctdMessage2;
			}
			if (ctdMessage2.Obj != DataTypeCtd.Int)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse statment. _index must be integer",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_index < _sytax.Count - 1 && _sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("]"))
			{
				_index++;
				return ParseAsignDataTable();
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statment. Missing ']'",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could not parse statment. Missing '['",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	private void RemoveVariable(ref List<Token> variable)
	{
		int num = variable.Count - 1;
		while (num >= 0 && variable[num].Name != "0")
		{
			variable.RemoveAt(num);
			num--;
		}
		variable.RemoveAt(variable.Count - 1);
	}

	public CtdMessage ParseGetData()
	{
		if (_index >= _sytax.Count)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could parse statement GetData.",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Type == TypeSyntax.KeyWord && _sytax[_index].Name.ToLower().Equals("getdata"))
		{
			if (_index > _sytax.Count - 6)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement GetData. Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement GetData. Mssing '('",
					LineNumber = _sytax[_index].LineNum
				};
			}
			_index += 2;
			if (_sytax[_index].Type != TypeSyntax.KeyWord)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Getdata statment. Missing output type",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			DataTypeCtd obj;
			switch (_sytax[_index].Name.ToLower())
			{
			case "int":
				obj = DataTypeCtd.Int;
				break;
			case "bool":
				obj = DataTypeCtd.Boolean;
				break;
			case "date":
				obj = DataTypeCtd.DateTime;
				break;
			case "datetime":
				obj = DataTypeCtd.DateTime;
				break;
			case "double":
				obj = DataTypeCtd.Double;
				break;
			case "datatable":
				obj = DataTypeCtd.DataTable;
				break;
			case "dataset":
				obj = DataTypeCtd.DataSet;
				break;
			case "string":
				obj = DataTypeCtd.String;
				break;
			default:
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Getdata statment. Data type ouput could not support",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			_index++;
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement GetData.",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement GetData. Missing ','",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			_index++;
			CtdMessage ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj != DataTypeCtd.String)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement GetData. Mising Action",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement GetData.",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(","))
			{
				_index++;
				while (_index < _sytax.Count - 1)
				{
					if (_sytax[_index].Type != TypeSyntax.Variable)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Missing variable for paramater",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					if (_sytax[_index + 1].Type == TypeSyntax.Operator && _sytax[_index + 1].Name.Equals("="))
					{
						_index += 2;
						ctdMessage = ParseObj();
						if (!string.IsNullOrEmpty(ctdMessage.Message))
						{
							return ctdMessage;
						}
					}
					else
					{
						string varName = _sytax[_index].Name.ToLower();
						if (_variable.Find((Token t) => t.Name == varName) == null)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing variable",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
					}
					if (_index > _sytax.Count - 1)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Missing operator",
							LineNumber = _sytax[_sytax.Count - 1].LineNum
						};
					}
					if (_sytax[_index].Type == TypeSyntax.Operator)
					{
						if (_sytax[_index].Name.Equals(","))
						{
							_index++;
							continue;
						}
						if (_sytax[_index].Name.Equals(")"))
						{
							break;
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Getdata statment. Missing operator",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Getdata statment. Missing operator",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
			}
			else if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Getdata statment. Missing operator ')'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			_index++;
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operator",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(";"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement GetData. Missing ';'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			return new CtdMessage
			{
				Obj = obj
			};
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could parse statement GetData.",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseExecute(DataTypeCtd outputType = DataTypeCtd.None)
	{
		if (_index >= _sytax.Count)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could parse statement Execute.",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Type == TypeSyntax.KeyWord && _sytax[_index].Name.ToLower().Equals("execute"))
		{
			if (_index > _sytax.Count - 4)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement Execute. Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement Execute. Mssing '('",
					LineNumber = _sytax[_index].LineNum
				};
			}
			_index += 2;
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement Execute.",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			CtdMessage ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj != DataTypeCtd.String)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement Execute. Mising Sql command",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement GetData.",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(","))
			{
				_index++;
				while (_index < _sytax.Count - 1)
				{
					ctdMessage = ParseObj();
					if (!string.IsNullOrEmpty(ctdMessage.Message))
					{
						return ctdMessage;
					}
					if (ctdMessage.Obj == DataTypeCtd.DataSet || ctdMessage.Obj == DataTypeCtd.DataTable || ctdMessage.Obj == DataTypeCtd.DataRow)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = $"Could not use datatype {ControlFunction.ToString(ctdMessage.Obj)} for exedata function",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					if (_index > _sytax.Count - 1)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Missing operator",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					if (_sytax[_index].Type == TypeSyntax.Operator)
					{
						if (_sytax[_index].Name.Equals(","))
						{
							_index++;
							continue;
						}
						if (_sytax[_index].Name.Equals(")"))
						{
							break;
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Execute statment. Missing operator",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Execute statment. Missing operator",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
			}
			else if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Execute statment. Missing operator ')'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			_index++;
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operator",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(";"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement Execute. Missing ';'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			switch (outputType)
			{
			case DataTypeCtd.None:
			case DataTypeCtd.Dynamic:
			case DataTypeCtd.DataSet:
			case DataTypeCtd.Object:
				return new CtdMessage
				{
					Obj = DataTypeCtd.DataSet
				};
			case DataTypeCtd.DataTable:
				return new CtdMessage
				{
					Obj = DataTypeCtd.DataTable
				};
			default:
				return new CtdMessage
				{
					Obj = outputType
				};
			}
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could parse statement GetData.",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseInvokeMethod()
	{
		if (_index >= _sytax.Count)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could parse statement invoke",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_sytax[_index].Type == TypeSyntax.KeyWord && _sytax[_index].Name.ToLower().Equals("invoke"))
		{
			if (_index > _sytax.Count - 4)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement invoke. Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement invoke. Mssing '('",
					LineNumber = _sytax[_index].LineNum
				};
			}
			_index += 2;
			CtdMessage ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj != DataTypeCtd.String && ctdMessage.Obj != DataTypeCtd.Object)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement invoke. Mising alias name",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement invoke.",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Execute statement invoke. Missing operator ')'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			_index++;
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operator",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(";"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement invoke. Missing ';'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			return new CtdMessage
			{
				Obj = DataTypeCtd.HttpResult
			};
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could parse statement invoke",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseShowFormAndDialogMethod()
	{
		if (_index >= _sytax.Count)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could parse statement Execute.",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		DataTypeCtd obj = DataTypeCtd.Form;
		if (_sytax[_index].Type == TypeSyntax.KeyWord && (_sytax[_index].Name.ToLower().Equals("showform") || _sytax[_index].Name.ToLower().Equals("showdialog")))
		{
			if (_index > _sytax.Count - 4)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement ShowForm or ShowDialog. Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Name.ToLower().Equals("showdialog"))
			{
				obj = DataTypeCtd.Dialog;
			}
			if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("("))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement ShowForm or ShowDialog. Mssing '('",
					LineNumber = _sytax[_index].LineNum
				};
			}
			_index += 2;
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement ShowForm or ShowDialog.",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			CtdMessage ctdMessage = ParseObj();
			if (!string.IsNullOrEmpty(ctdMessage.Message))
			{
				return ctdMessage;
			}
			if (ctdMessage.Obj != DataTypeCtd.String)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement ShowForm or ShowDialog. Missing FormType",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement ShowForm or ShowDialog.",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(","))
			{
				_index++;
				while (_index < _sytax.Count - 1)
				{
					ctdMessage = ParseObj();
					if (!string.IsNullOrEmpty(ctdMessage.Message))
					{
						return ctdMessage;
					}
					if (_index > _sytax.Count - 1)
					{
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Missing operator",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					if (_sytax[_index].Type == TypeSyntax.Operator)
					{
						if (_sytax[_index].Name.Equals(","))
						{
							_index++;
							continue;
						}
						if (_sytax[_index].Name.Equals(")"))
						{
							break;
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Execute statment. Missing operator",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Execute statment. Missing operator",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
			}
			else if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Execute statment. Missing operator ')'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			_index++;
			if (_index > _sytax.Count - 1)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operator",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(";"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could parse statement ShowForm or ShowDialog. Missing ';'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			return new CtdMessage
			{
				Obj = obj
			};
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could parse statement GetData.",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage ParseObj(bool isColumnFormular = false, DataTypeCtd outputType = DataTypeCtd.None)
	{
		if (_index >= _sytax.Count)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse object.Missing statment",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		List<Token> list = new List<Token>();
		int num = 0;
		MathState mathState = MathState.Start;
		bool flag = true;
		while (_index < _sytax.Count)
		{
			if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("("))
			{
				num++;
			}
			if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(")"))
			{
				if (num == 0)
				{
					flag = false;
					break;
				}
				num--;
			}
			switch (mathState)
			{
			case MathState.Start:
				if (_sytax[_index].Type == TypeSyntax.Operator)
				{
					if (_sytax[_index].Name.Equals("(") || ConstantsApzCtd.KeyWordOperator.Contains(_sytax[_index].Name.ToLower()) || _sytax[_index].Name.Equals(ControlFunction.ToString("+")) || _sytax[_index].Name.Equals(ControlFunction.ToString("-")))
					{
						list.Add(new Token
						{
							Name = _sytax[_index].Name,
							Type = _sytax[_index].Type
						});
						_index++;
						continue;
					}
					if (_sytax[_index].Name.Equals(ControlFunction.ToString("$")))
					{
						_index++;
						CtdMessage dataFromItem = GetDataFromItem();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Action == ActionStatment.ShowHide)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Can not use show/hide method in the math expresion",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (dataFromItem.Action == ActionStatment.Focus)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Can not use focus method in math expression",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							DataType = ((isColumnFormular && dataFromItem.Obj == DataTypeCtd.ColumnData) ? DataTypeCtd.Object : dataFromItem.Obj),
							Type = TypeSyntax.Variable
						});
						mathState = MathState.Accept;
						continue;
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_sytax[_index].Type == TypeSyntax.Constant)
				{
					list.Add(new Token
					{
						Type = TypeSyntax.Constant,
						DataType = _sytax[_index].DataType
					});
					_index++;
					mathState = MathState.Accept;
					continue;
				}
				if (_sytax[_index].Type == TypeSyntax.Variable)
				{
					CtdMessage dataFromItem = ParseCheckVariable();
					if (!string.IsNullOrEmpty(dataFromItem.Message))
					{
						return dataFromItem;
					}
					list.Add(new Token
					{
						DataType = dataFromItem.Obj,
						Type = TypeSyntax.Variable
					});
					mathState = MathState.Accept;
					_index++;
					continue;
				}
				if (_sytax[_index].Type == TypeSyntax.KeyWord)
				{
					if (_sytax[_index].Name.ToLower().Equals("count") && _index >= 2 && _sytax[_index - 1].Type == TypeSyntax.Operator && _sytax[_index - 1].Name.ToLower().Equals(".") && _sytax[_index - 2].Type == TypeSyntax.Variable && _sytax[_index - 2].DataType == DataTypeCtd.List)
					{
						list.Add(new Token
						{
							DataType = DataTypeCtd.Int,
							Type = TypeSyntax.Constant
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("abs") || _sytax[_index].Name.ToLower().Equals("sqrt"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (_sytax[_index].Name.ToLower().Equals("abs"))
						{
							switch (dataFromItem.Obj)
							{
							case DataTypeCtd.Int:
							case DataTypeCtd.Object:
								list.Add(new Token
								{
									Type = TypeSyntax.Variable,
									DataType = DataTypeCtd.Int
								});
								break;
							case DataTypeCtd.Double:
							case DataTypeCtd.Numeric:
								list.Add(new Token
								{
									Type = TypeSyntax.Variable,
									DataType = DataTypeCtd.Double
								});
								break;
							default:
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Mathematics only contains numbers",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
						}
						else
						{
							if (dataFromItem.Obj != DataTypeCtd.Numeric && dataFromItem.Obj != DataTypeCtd.Int && dataFromItem.Obj != DataTypeCtd.Double && dataFromItem.Obj != DataTypeCtd.Object)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Mathematics only contains numbers",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
							list.Add(new Token
							{
								Type = TypeSyntax.Variable,
								DataType = DataTypeCtd.Double
							});
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("tolower") || _sytax[_index].Name.ToLower().Equals("toupper") || _sytax[_index].Name.ToLower().Equals("trim"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj == DataTypeCtd.String)
						{
							if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Missing operator ')'",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
							list.Add(new Token
							{
								Type = TypeSyntax.Variable,
								DataType = DataTypeCtd.String
							});
							mathState = MathState.Accept;
							_index++;
							continue;
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "It is not possible to use string processing functions to handle arithmetic",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					if (_sytax[_index].Name.ToLower().Equals("format"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function format missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function format missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing formatString to format function",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function format missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function format missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.String
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("substring"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "It is not possible to use string processing functions to handle arithmetic",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function substring missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function substring missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.Int)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing start _index to substring",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function substring missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function substring missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.Int)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing end _index to substring",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function substring missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function substring missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.String
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("split"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "It is not possible to use string processing functions to handle arithmetic",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function substring missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function substring missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "It is not possible to use string processing functions to handle arithmetic",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function substring missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.List,
							TagetType = DataTypeCtd.String
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("replace"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "It is not possible to use string processing functions to handle arithmetic",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function replace missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function replace missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing old_string to replace string",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function replace missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function replace missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing new_string to replace",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function replace missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function replace missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.String
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("firstindexstring"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "It is not possible to use string processing functions to handle arithmetic",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function firstindexstring missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function firstindexstring missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "It is not possible to use string processing functions to handle arithmetic",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function firstindexstring missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.Int
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("round"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.Double && dataFromItem.Obj != DataTypeCtd.Int && dataFromItem.Obj != DataTypeCtd.Numeric)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Parameter must be numeric value",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function round missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							if (_index > _sytax.Count - 1)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Function round missing operator",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
							if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Function round missing operator ')'",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
						}
						else
						{
							_index++;
							dataFromItem = ParseObj();
							if (!string.IsNullOrEmpty(dataFromItem.Message))
							{
								return dataFromItem;
							}
							if (dataFromItem.Obj != DataTypeCtd.Int)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Missing decimals to round",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
							if (_index > _sytax.Count - 1)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Function round missing operator",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
							if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Function round missing operator ')'",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.Double
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("dateaddday") || _sytax[_index].Name.ToLower().Equals("dateaddmonth") || _sytax[_index].Name.ToLower().Equals("dateaddyear"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.DateTime)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "It is not possible to use string processing functions to handle arithmetic",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = $"Function {_sytax[_index].Name.ToLower()} missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = $"Function {_sytax[_index].Name.ToLower()} missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.Int)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing paramater",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = $"Function {_sytax[_index].Name.ToLower()} missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = $"Function {_sytax[_index].Name.ToLower()} missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.DateTime
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("dategetday") || _sytax[_index].Name.ToLower().Equals("dategetmonth") || _sytax[_index].Name.ToLower().Equals("dategetyear"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.DateTime)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "It is not possible to use string processing functions to handle arithmetic",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = $"Function {_sytax[_index].Name.ToLower()} missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = $"Function {_sytax[_index].Name.ToLower()} missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.Int
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("datetostring"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.DateTime)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "It is not possible to use string processing functions to handle arithmetic",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = $"Function {_sytax[_index].Name.ToLower()} missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals(","))
						{
							_index++;
							dataFromItem = ParseObj();
							if (!string.IsNullOrEmpty(dataFromItem.Message))
							{
								return dataFromItem;
							}
							if (dataFromItem.Obj != DataTypeCtd.String)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Missing paramater",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
							if (_index > _sytax.Count - 1)
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = $"Function {_sytax[_index].Name.ToLower()} missing operator",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = $"Function {_sytax[_index].Name.ToLower()} missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.String
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("showmessagebox"))
					{
						CtdMessage dataFromItem = ParseShowMessageBox();
						_index++;
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.Boolean)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "You must chose type messagebox yesno or okcancel",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = dataFromItem.Obj
						});
						mathState = MathState.Accept;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("getdata"))
					{
						CtdMessage dataFromItem = ParseGetData();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = dataFromItem.Obj
						});
						mathState = MathState.Accept;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("execute"))
					{
						CtdMessage dataFromItem = ParseExecute(outputType);
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = dataFromItem.Obj
						});
						mathState = MathState.Accept;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("getdate"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = $"Function {_sytax[_index].Name.ToLower()} missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = $"Function {_sytax[_index].Name.ToLower()} missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.DateTime
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("length"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						DataTypeCtd obj = dataFromItem.Obj;
						if ((uint)(obj - 1) <= 2u || (uint)(obj - 9) <= 2u)
						{
							list.Add(new Token
							{
								Type = TypeSyntax.Variable,
								DataType = DataTypeCtd.Int
							});
							if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
							{
								return new CtdMessage
								{
									Type = MessageType.ERROR,
									Message = "Missing operator ')'",
									LineNumber = _sytax[_index - 1].LineNum
								};
							}
							mathState = MathState.Accept;
							_index++;
							continue;
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Methods Length must be apply to text and number type",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					if (new string[5] { "totaldays", "totalhours", "totalminutes", "totalseconds", "totalmilliseconds" }.Contains(_sytax[_index].Name.ToLower()))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.DateTime)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function just be apply to datetime type",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.DateTime)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function just be apply to datetime type",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.Double
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("dayofweek") || _sytax[_index].Name.ToLower().Equals("dayofyear"))
					{
						DataTypeCtd dataType = ((!_sytax[_index].Name.ToLower().Equals("dayofweek")) ? DataTypeCtd.Int : DataTypeCtd.String);
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.DateTime)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function just be apply to datetime type",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = dataType
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("invoke"))
					{
						CtdMessage dataFromItem = ParseInvokeMethod();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = dataFromItem.Obj
						});
						mathState = MathState.Accept;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("mergepdf"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.List)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Merge PDF: First parameter must be List type",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function mergepdf missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function mergepdf missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Merge PDF: Second param must be String type",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function mergepdf missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function mergepdf missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.String
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("processsapobject"))
					{
						_index++;
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals("("))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Missing operator '('",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						CtdMessage dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Process SAP Object: First parameter must be string",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function processsapobject missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function processsapobject missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Process SAP Object: Second param must be string",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function processsapobject missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function processsapobject missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Process SAP Object: Third param must be string",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function processsapobject missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(","))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function processsapobject missing operator ','",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						_index++;
						dataFromItem = ParseObj();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						if (dataFromItem.Obj != DataTypeCtd.String)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Process SAP Object: Fourth param must be string",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_index > _sytax.Count - 1)
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function processsapobject missing operator",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						if (_sytax[_index].Type != TypeSyntax.Operator || !_sytax[_index].Name.Equals(")"))
						{
							return new CtdMessage
							{
								Type = MessageType.ERROR,
								Message = "Function processsapobject missing operator ')'",
								LineNumber = _sytax[_index - 1].LineNum
							};
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = DataTypeCtd.HttpResult
						});
						mathState = MathState.Accept;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.ToLower().Equals("showform") || _sytax[_index].Name.ToLower().Equals("showdialog"))
					{
						CtdMessage dataFromItem = ParseShowFormAndDialogMethod();
						if (!string.IsNullOrEmpty(dataFromItem.Message))
						{
							return dataFromItem;
						}
						list.Add(new Token
						{
							Type = TypeSyntax.Variable,
							DataType = dataFromItem.Obj
						});
						mathState = MathState.Accept;
						continue;
					}
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse statment",
					LineNumber = _sytax[_index].LineNum
				};
			case MathState.Accept:
				if (_sytax[_index].Type == TypeSyntax.Operator)
				{
					if (ConstantsApzCtd.MathOperator.Contains(_sytax[_index].Name))
					{
						list.Add(new Token
						{
							Type = TypeSyntax.Operator,
							Name = _sytax[_index].Name
						});
						mathState = MathState.Start;
						_index++;
						continue;
					}
					if (_sytax[_index].Name.Equals(")"))
					{
						list.Add(new Token
						{
							Type = TypeSyntax.Operator,
							Name = ")"
						});
						_index++;
						continue;
					}
					if (_sytax[_index].Name.Equals(";") || _sytax[_index].Name.Equals("{") || _sytax[_index].Name.Equals("]") || _sytax[_index].Name.Equals(","))
					{
						if (num == 0)
						{
							flag = false;
							break;
						}
						return new CtdMessage
						{
							Type = MessageType.ERROR,
							Message = "Could not parse statment. statement missing ')'",
							LineNumber = _sytax[_index - 1].LineNum
						};
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse statment",
						LineNumber = _sytax[_index].LineNum
					};
				}
				if (_sytax[_index].Type == TypeSyntax.KeyWord && ConstantsApzCtd.OperatorMathBoolean.Contains(_sytax[_index].Name.ToLower()))
				{
					list.Add(new Token
					{
						Name = _sytax[_index].Name,
						Type = TypeSyntax.KeyWord
					});
					mathState = MathState.Start;
					_index++;
					continue;
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not parse statment",
					LineNumber = _sytax[_index].LineNum
				};
			}
			if (!flag)
			{
				break;
			}
			_index++;
		}
		if (flag)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not parse statment",
				LineNumber = _sytax[_sytax.Count - 1].LineNum
			};
		}
		return ControlFunction.ParseObj(list);
	}

	public CtdMessage ParseCheckVariable()
	{
		if (_index >= _sytax.Count)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not check variable",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		string varName = _sytax[_index].Name.ToLower();
		Token token = _variable.Find((Token t) => t.Name == varName);
		if (token == null)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Variable did not exists",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		return token.DataType switch
		{
			DataTypeCtd.Boolean => new CtdMessage
			{
				Obj = DataTypeCtd.Boolean
			}, 
			DataTypeCtd.Int => new CtdMessage
			{
				Obj = DataTypeCtd.Int
			}, 
			DataTypeCtd.Double => new CtdMessage
			{
				Obj = DataTypeCtd.Double
			}, 
			DataTypeCtd.String => new CtdMessage
			{
				Obj = DataTypeCtd.String
			}, 
			DataTypeCtd.DateTime => new CtdMessage
			{
				Obj = DataTypeCtd.DateTime
			}, 
			DataTypeCtd.Object => new CtdMessage
			{
				Obj = DataTypeCtd.Object
			}, 
			DataTypeCtd.HttpResult => IsObjFromData(DataTypeCtd.HttpResult), 
			DataTypeCtd.DataRow => IsObjFromData(DataTypeCtd.DataRow), 
			DataTypeCtd.DataTable => IsObjFromData(DataTypeCtd.DataTable), 
			DataTypeCtd.DataSet => IsObjFromData(DataTypeCtd.DataSet), 
			DataTypeCtd.List => IsList(token.TagetType), 
			DataTypeCtd.Form => IsObjFromData(DataTypeCtd.Form), 
			DataTypeCtd.Dialog => IsObjFromData(DataTypeCtd.Dialog), 
			_ => new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not check variable. Data type did not supported",
				LineNumber = _sytax[_index - 1].LineNum
			}, 
		};
	}

	public CtdMessage IsList(DataTypeCtd childType)
	{
		if (_index < _sytax.Count - 3 && _sytax[_index + 1].Type == TypeSyntax.Operator && _sytax[_index + 1].Name.Equals("[") && _sytax[_index + 3].Type == TypeSyntax.Operator && _sytax[_index + 3].Name.Equals("]"))
		{
			if (_sytax[_index + 2].Type == TypeSyntax.Constant && _sytax[_index + 2].DataType == DataTypeCtd.Int)
			{
				_index += 3;
				if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("."))
				{
					switch (childType)
					{
					case DataTypeCtd.Boolean:
						return new CtdMessage
						{
							Obj = DataTypeCtd.Boolean
						};
					case DataTypeCtd.Int:
					case DataTypeCtd.Double:
						return new CtdMessage
						{
							Obj = DataTypeCtd.Numeric
						};
					case DataTypeCtd.String:
						return new CtdMessage
						{
							Obj = DataTypeCtd.String
						};
					}
				}
				_index += 2;
				if (_sytax[_index].Type == TypeSyntax.KeyWord)
				{
					switch (_sytax[_index].Name.ToLower())
					{
					case "string":
						return new CtdMessage
						{
							Obj = DataTypeCtd.String
						};
					case "int":
						return new CtdMessage
						{
							Obj = DataTypeCtd.Int
						};
					case "double":
						return new CtdMessage
						{
							Obj = DataTypeCtd.Double
						};
					case "bool":
						return new CtdMessage
						{
							Obj = DataTypeCtd.Boolean
						};
					case "datetime":
						return new CtdMessage
						{
							Obj = DataTypeCtd.DateTime
						};
					case "date":
						return new CtdMessage
						{
							Obj = DataTypeCtd.DateTime
						};
					}
				}
			}
			if (_sytax[_index + 2].Type == TypeSyntax.Variable)
			{
				string sttName = _sytax[_index + 2].Name.ToLower();
				Token token = _variable.Find((Token t) => t.Name == sttName);
				if (token != null && token.DataType == DataTypeCtd.Int)
				{
					_index += 3;
					switch (childType)
					{
					case DataTypeCtd.Boolean:
						return new CtdMessage
						{
							Obj = DataTypeCtd.Boolean
						};
					case DataTypeCtd.Int:
					case DataTypeCtd.Double:
						return new CtdMessage
						{
							Obj = DataTypeCtd.Numeric
						};
					case DataTypeCtd.String:
						return new CtdMessage
						{
							Obj = DataTypeCtd.String
						};
					}
				}
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Missing variable",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		if (_index < _sytax.Count - 2 && _sytax[_index + 1].Type == TypeSyntax.Operator && _sytax[_index + 1].Name.Equals(".") && _sytax[_index + 2].Type == TypeSyntax.KeyWord && _sytax[_index + 2].Name.ToLower().Equals("count"))
		{
			_index += 2;
			return new CtdMessage
			{
				Obj = DataTypeCtd.Int
			};
		}
		if (_index < _sytax.Count - 3 && _index > 2 && _sytax[_index - 1].Type == TypeSyntax.Operator && _sytax[_index - 1].Name.ToLower().Equals("(") && _sytax[_index - 2].Type == TypeSyntax.KeyWord && _sytax[_index - 2].Name.ToLower().Equals("mergepdf"))
		{
			return new CtdMessage
			{
				Obj = DataTypeCtd.List
			};
		}
		return new CtdMessage
		{
			Type = MessageType.ERROR,
			Message = "Could not parse list .Missing operation",
			LineNumber = _sytax[_index - 1].LineNum
		};
	}

	public CtdMessage IsObjFromData(DataTypeCtd inputType, bool isGrid = false)
	{
		if (_index >= _sytax.Count)
		{
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not check type object.",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
		switch (inputType)
		{
		case DataTypeCtd.DataRow:
			if (_index < _sytax.Count - 1 && (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals(".")))
			{
				return new CtdMessage
				{
					Obj = DataTypeCtd.DataRow
				};
			}
			if (_index < _sytax.Count - 3)
			{
				if (_sytax[_index + 2].Type != TypeSyntax.Operator || !_sytax[_index + 2].Name.Equals("["))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not check type object. Missing operation '['",
						LineNumber = _sytax[_index + 1].LineNum
					};
				}
				_index += 3;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if ((ctdMessage.Obj == DataTypeCtd.String || ctdMessage.Obj == DataTypeCtd.Object || ctdMessage.Obj == DataTypeCtd.Int) && _index < _sytax.Count - 1 && _sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("]"))
				{
					if (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals("."))
					{
						return new CtdMessage
						{
							Obj = DataTypeCtd.Object
						};
					}
					_index += 2;
					if (_sytax[_index].Type == TypeSyntax.KeyWord)
					{
						switch (_sytax[_index].Name.ToLower())
						{
						case "string":
							return new CtdMessage
							{
								Obj = DataTypeCtd.String
							};
						case "int":
							return new CtdMessage
							{
								Obj = DataTypeCtd.Int
							};
						case "double":
							return new CtdMessage
							{
								Obj = DataTypeCtd.Double
							};
						case "bool":
							return new CtdMessage
							{
								Obj = DataTypeCtd.Boolean
							};
						case "datetime":
							return new CtdMessage
							{
								Obj = DataTypeCtd.DateTime
							};
						case "date":
							return new CtdMessage
							{
								Obj = DataTypeCtd.DateTime
							};
						}
					}
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Could not parse assign statment. Datatype did not support",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not check type object. Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not check type object. Missing operation",
				LineNumber = _sytax[_index - 1].LineNum
			};
		case DataTypeCtd.DataTable:
			if (_index < _sytax.Count - 1 && (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals(".")))
			{
				return new CtdMessage
				{
					Obj = DataTypeCtd.DataTable
				};
			}
			if (_index > _sytax.Count - 3)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not check type object. Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index + 2].Type == TypeSyntax.Operator)
			{
				if (_sytax[_index + 2].Name.Equals("["))
				{
					_index += 3;
					CtdMessage ctdMessage = ParseObj();
					if (!string.IsNullOrEmpty(ctdMessage.Message))
					{
						return ctdMessage;
					}
					if (ctdMessage.Obj == DataTypeCtd.Int && _index < _sytax.Count - 1 && _sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("]"))
					{
						return IsObjFromData(DataTypeCtd.DataRow);
					}
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Missing operation '['",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index + 2].Type == TypeSyntax.KeyWord && (_sytax[_index + 2].Name.ToLower().Equals("focusrow") || _sytax[_index + 2].Name.ToLower().Equals("columnedit")))
			{
				_index += 2;
				return new CtdMessage
				{
					Obj = (_sytax[_index].Name.ToLower().Equals("focusrow") ? DataTypeCtd.Int : DataTypeCtd.String)
				};
			}
			if (_sytax[_index + 2].Name.ToLower().Equals("selectedrows") || _sytax[_index + 2].Name.ToLower().Equals("focusedcolumn"))
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = _sytax[_index + 2].Name.ToLower() + " can only be used for grid.",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index + 2].Name.ToLower().Equals("firstduplicate"))
			{
				if (!_sytax[_index + 3].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index += 4;
				CtdMessage ctdMessage2 = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage2.Message))
				{
					return ctdMessage2;
				}
				if (ctdMessage2.Obj != DataTypeCtd.String)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "FileName must be string",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				return new CtdMessage
				{
					Obj = DataTypeCtd.Int
				};
			}
			if (_sytax[_index + 2].Name.ToLower().Equals("stringagg"))
			{
				if (!_sytax[_index + 3].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index += 4;
				CtdMessage ctdMessage3 = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage3.Message))
				{
					return ctdMessage3;
				}
				if (ctdMessage3.Obj != DataTypeCtd.String)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "FileName must be string",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				CtdMessage ctdMessage4 = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage4.Message))
				{
					return ctdMessage4;
				}
				if (ctdMessage4.Obj != DataTypeCtd.String)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Seperator must be string",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				return new CtdMessage
				{
					Obj = DataTypeCtd.String
				};
			}
			if ((_sytax[_index + 2].Name.ToLower().Equals("rows") || _sytax[_index + 2].Name.ToLower().Equals("columns")) && _sytax[_index + 3].Name == "." && _sytax[_index + 4].Name.ToLower().Equals("count"))
			{
				_index += 4;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Int
				};
			}
			if (_sytax[_index + 2].Name.ToLower().Equals("first"))
			{
				if (!_sytax[_index + 3].Name.Equals("("))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index += 4;
				CtdMessage ctdMessage5 = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage5.Message))
				{
					return ctdMessage5;
				}
				if (ctdMessage5.Obj != DataTypeCtd.String && ctdMessage5.Obj != DataTypeCtd.Object)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "FileName must be string",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_index > _sytax.Count - 1 || !_sytax[_index].Name.ToLower().Equals(";"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				_index++;
				CtdMessage ctdMessage6 = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage6.Message))
				{
					return ctdMessage6;
				}
				if (_index > _sytax.Count - 1 || !_sytax[_index].Name.Equals(")"))
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Missing operation",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				return new CtdMessage
				{
					Obj = DataTypeCtd.DataRow
				};
			}
			if (_sytax[_index + 2].Name.ToLower().Equals("columns") && _sytax[_index + 3].Name == "." && _sytax[_index + 4].Name.ToLower().Equals("contains") && _sytax[_index + 5].Name.ToLower().Equals("("))
			{
				_index += 6;
				CtdMessage ctdMessage7 = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage7.Message))
				{
					return ctdMessage7;
				}
				if (ctdMessage7.Obj != DataTypeCtd.String)
				{
					return new CtdMessage
					{
						Type = MessageType.ERROR,
						Message = "Datatype of paramater must be string",
						LineNumber = _sytax[_index - 1].LineNum
					};
				}
				if (_index < _sytax.Count && _sytax[_index].Name.ToLower().Equals(")"))
				{
					return new CtdMessage
					{
						Obj = DataTypeCtd.Boolean
					};
				}
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not check type object. Missing operation",
				LineNumber = _sytax[_index - 1].LineNum
			};
		case DataTypeCtd.DataSet:
			if (_index < _sytax.Count - 1 && (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals(".")))
			{
				return new CtdMessage
				{
					Obj = DataTypeCtd.DataSet
				};
			}
			if (_index > _sytax.Count - 3)
			{
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not check type object. Missing operation",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index + 2].Type == TypeSyntax.Operator && _sytax[_index + 2].Name.Equals("["))
			{
				_index += 3;
				CtdMessage ctdMessage = ParseObj();
				if (!string.IsNullOrEmpty(ctdMessage.Message))
				{
					return ctdMessage;
				}
				if ((ctdMessage.Obj == DataTypeCtd.Int || ctdMessage.Obj == DataTypeCtd.String) && _index < _sytax.Count - 1 && _sytax[_index].Type == TypeSyntax.Operator && _sytax[_index].Name.Equals("]"))
				{
					return IsObjFromData(DataTypeCtd.DataTable);
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not check type object. Missing operation ']'",
					LineNumber = _sytax[_index - 1].LineNum
				};
			}
			if (_sytax[_index + 2].Name.ToLower().Equals("table") && _sytax[_index + 3].Name.ToLower().Equals(".") && _sytax[_index + 4].Name.ToLower().Equals("count"))
			{
				_index += 4;
				return new CtdMessage
				{
					Obj = DataTypeCtd.Int
				};
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not check type object. Missing operation",
				LineNumber = _sytax[_index - 1].LineNum
			};
		case DataTypeCtd.HttpResult:
			if (_index < _sytax.Count - 1 && (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals(".")))
			{
				return new CtdMessage
				{
					Obj = DataTypeCtd.HttpResult
				};
			}
			if (_index < _sytax.Count - 2)
			{
				_index += 2;
				if (_sytax[_index].Type == TypeSyntax.KeyWord)
				{
					if (_sytax[_index].Name.Equals("message"))
					{
						return new CtdMessage
						{
							Obj = DataTypeCtd.String
						};
					}
					if (_sytax[_index].Name.Equals("content"))
					{
						return new CtdMessage
						{
							Obj = DataTypeCtd.Object
						};
					}
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not get value form httpresult. Missing operation",
					LineNumber = _sytax[_index].LineNum
				};
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not get value form httpresult. Missing operation",
				LineNumber = _sytax[_index].LineNum
			};
		case DataTypeCtd.Form:
			if (_index < _sytax.Count - 1 && (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals(".")))
			{
				return new CtdMessage
				{
					Obj = DataTypeCtd.Form
				};
			}
			if (_index < _sytax.Count - 2)
			{
				_index += 2;
				if (_sytax[_index].Type == TypeSyntax.KeyWord && _sytax[_index].Name.Equals("datasource"))
				{
					return IsObjFromData(DataTypeCtd.DataSet);
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not get value form httpresult. Missing operation",
					LineNumber = _sytax[_index].LineNum
				};
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not get value form httpresult. Missing operation",
				LineNumber = _sytax[_index].LineNum
			};
		case DataTypeCtd.Dialog:
			if (_index < _sytax.Count - 1 && (_sytax[_index + 1].Type != TypeSyntax.Operator || !_sytax[_index + 1].Name.Equals(".")))
			{
				return new CtdMessage
				{
					Obj = DataTypeCtd.Dialog
				};
			}
			if (_index < _sytax.Count - 2)
			{
				_index += 2;
				if (_sytax[_index].Type == TypeSyntax.KeyWord)
				{
					if (_sytax[_index].Name.Equals("datasource"))
					{
						return IsObjFromData(DataTypeCtd.DataSet);
					}
					if (_sytax[_index].Name.Equals("dialogresult"))
					{
						return new CtdMessage
						{
							Obj = DataTypeCtd.String
						};
					}
				}
				return new CtdMessage
				{
					Type = MessageType.ERROR,
					Message = "Could not get value form httpresult. Missing operation",
					LineNumber = _sytax[_index].LineNum
				};
			}
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not get value form httpresult. Missing operation",
				LineNumber = _sytax[_index].LineNum
			};
		default:
			return new CtdMessage
			{
				Type = MessageType.ERROR,
				Message = "Could not check type object.",
				LineNumber = _sytax[_index - 1].LineNum
			};
		}
	}
}
