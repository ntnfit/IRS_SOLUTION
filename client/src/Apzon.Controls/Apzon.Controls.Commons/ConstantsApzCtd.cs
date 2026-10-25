namespace Apzon.Controls.Commons;

public class ConstantsApzCtd
{
	public static string[] HighlightKeywords = new string[47]
	{
		"count", "int", "double", "string", "list", "datatable", "abs", "columnedit", "focusrow", "rows",
		"columns", "or", "and", "else", "var", "true", "false", "add", "bool", "all",
		"any", "as", "break", "return", "datarow", "by", "case", "compute", "contains", "continue",
		"convert", "default", "table", "cancelevent", "now", "insert", "selectedrows", "focusedcolumn", "isselectedrow", "cancelnextcellvaluechangedevent",
		"cancelnextfocusobjectchangedevent", "cancelnextselectedchangedevent", "round", "first", "firstduplicate", "stringagg", "format"
	};

	public static string[] ColorKeyWork = new string[139]
	{
		"aliceblue", "antiquewhite", "aqua", "aquamarine", "azure", "beige", "bisque", "black", "blanchedalmond", "blue",
		"blueviolet", "brown", "burlywood", "cadetblue", "chartreuse", "chocolate", "coral", "cornflowerblue", "cornsilk", "crimson",
		"cyan", "darkblue", "darkcyan", "darkgoldenrod", "darkgreen", "darkkhaki", "darkmagenta", "darkolivegreen", "darkorange", "darkorchid",
		"darkred", "darksalmon", "darkseagreen", "darkslateblue", "darkslategray", "darkturquoise", "darkviolet", "deeppink", "deepSkyblue", "dimgray",
		"dodgerblue", "firebrick", "floralwhite", "forestgreen", "fuchsia", "gainsboro", "ghostwhite", "gold", "goldenrod", "gray",
		"green", "greenyellow", "honeydew", "hotpink", "indianred", "indigo", "ivory", "khaki", "lavender", "lavenderblush",
		"lawngreen", "lemonchiffon", "lightblue", "lightcoral", "lightcyan", "lightgoldenrodyellow", "lightgreen", "lightpink", "lightsalmon", "lightseagreen",
		"lightskyblue", "lightslategray", "lightsteelblue", "lightyellow", "lime", "limegreen", "linen", "magenta", "maroon", "mediumaquamarine",
		"mediumblue", "mediumorchid", "mediumpurple", "mediumseagreen", "mediumslateblue", "mediumspringgreen", "mediumturquoise", "mediumvioletred", "midnightblue", "mintcream",
		"mistyrose", "moccasin", "navajowhite", "navy", "oldlace", "olive", "olivedrab", "orange", "orangered", "orchid",
		"palegoldenrod", "palegreen", "paleturquoise", "palevioletred", "papayawhip", "peachpuff", "peru", "pink", "plum", "powderblue",
		"purple", "red", "rosybrown", "royalblue", "saddlebrown", "salmon", "sandybrown", "seagreen", "seashell", "sienna",
		"silver", "skyblue", "slateblue", "slategray", "snow", "springgreen", "steelblue", "tan", "teal", "thistle",
		"tomato", "transparent", "turquoise", "violet", "wheat", "white", "whitesmoke", "yellow", "yellowgreen"
	};

	public static string[] HighlightFunctions = new string[82]
	{
		"if", "for", "loop", "abs", "else", "while", "execute", "showmessagebox", "getdata", "substring",
		"split", "replace", "firstindexstring", "trim", "tolower", "toupper", "getrowcellvalue", "setrowcellvalue", "setfocusrowcellvalue", "getfocusrowcellvalue",
		"max", "min", "avg", "sum", "sqrt", "abs", "showsuccess", "showerror", "showinfo", "showwarning",
		"dateaddday", "dateaddmonth", "dateaddyear", "dategetday", "dategetmonth", "dategetyear", "getdate", "datetostring", "push", "show",
		"hide", "newrow", "focus", "clear", "selectall", "clearselection", "readonly", "enable", "color", "delete",
		"selected", "unselected", "length", "totaldays", "totalhours", "totalminutes", "totalseconds", "totalmilliseconds", "dayofweek", "dayofyear",
		"invoke", "httpresult", "message", "content", "form", "showform", "showdialog", "datasource", "dialog", "dialogresult",
		"contains", "setdatasourcecell", "setfocusdatasourcecell", "round", "openlink", "drilldown", "mergepdf", "getparentrowhandle", "getchildrowcount", "getchildrowhandle",
		"format", "processsapobject"
	};

	public static string[] HighlightDataType = new string[7] { "int", "double", "string", "datetime", "list", "table", "bool" };

	public static string[] HighlightOperations = new string[20]
	{
		"$", "+", "-", "*", "^", "/", "%", "(", ")", "{",
		"}", ",", "[", "]", "<", ">", "=", ".", "!", "'"
	};

	public static string[] HighlightGlobal = new string[7] { "@username", "@branchid", "@userid", "@currentdate", "@currenttime", "@rowindex", "@executepath" };

	public const string KeyCount = "count";

	public const string KeyInt = "int";

	public const string KeyDouble = "double";

	public const string KeyString = "string";

	public const string KeyDateTime = "datetime";

	public const string KeyDate = "date";

	public const string KeyList = "list";

	public const string KeyDatatable = "datatable";

	public const string KeyDataset = "dataset";

	public const string KeyAbs = "abs";

	public const string KeyColumnEdit = "columnedit";

	public const string KeyFocusRow = "focusrow";

	public const string KeyRows = "rows";

	public const string KeyColumns = "columns";

	public const string KeyIf = "if";

	public const string KeyFor = "for";

	public const string KeyOr = "or";

	public const string KeyAnd = "and";

	public const string KeyElse = "else";

	public const string KeyVar = "var";

	public const string KeyExecute = "execute";

	public const string KeyShowMessageBox = "showmessagebox";

	public const string KeyWhile = "while";

	public const string KeyTolower = "tolower";

	public const string KeyToUpper = "toupper";

	public const string KeyTrim = "trim";

	public const string KeySqrt = "sqrt";

	public const string KeyGetData = "getdata";

	public const string KeyDataRow = "datarow";

	public const string KeyTrue = "true";

	public const string KeyFalse = "false";

	public const string KeySubstring = "substring";

	public const string KeySplit = "split";

	public const string KeyReplace = "replace";

	public const string KeyFirstIndexString = "firstindexstring";

	public const string KeyAdd = "add";

	public const string KeyGetRowCellValue = "getrowcellvalue";

	public const string KeyGetFocusRowCellValue = "getfocusrowcellvalue";

	public const string KeySetRowCellValue = "setrowcellvalue";

	public const string KeySetFocusRowCellValue = "setfocusrowcellvalue";

	public const string KeySetDatasourceCell = "setdatasourcecell";

	public const string KeySetFocusDatasourceCell = "setfocusdatasourcecell";

	public const string KeyGetParentRowHandle = "getparentrowhandle";

	public const string KeyGetChildRowCount = "getchildrowcount";

	public const string KeyGetChildRowHandle = "getchildrowhandle";

	public const string KeyFirst = "first";

	public const string KeyFirstDuplicate = "firstduplicate";

	public const string KeyStringAgg = "stringagg";

	public const string KeyFormat = "format";

	public const string KeyNewRow = "newrow";

	public const string KeyFocus = "focus";

	public const string KeyShow = "show";

	public const string KeyHide = "hide";

	public const string KeyLoop = "loop";

	public const string KeyMax = "max";

	public const string KeyMin = "min";

	public const string KeyAvg = "avg";

	public const string KeySum = "sum";

	public const string KeyBoolean = "bool";

	public const string KeyAll = "all";

	public const string KeyAny = "any";

	public const string KeyAs = "as";

	public const string KeyBreak = "break";

	public const string KeyReturn = "return";

	public const string KeyBy = "by";

	public const string KeyCase = "case";

	public const string KeyCompute = "compute";

	public const string KeyContains = "contains";

	public const string KeyContinue = "continue";

	public const string KeyConvert = "convert";

	public const string KeyDefault = "default";

	public const string KeyTable = "table";

	public const string KeyOpenLink = "openlink";

	public const string KeyDrillDown = "drilldown";

	public const string KeyProcessSapObject = "processsapobject";

	public const string KeyCancelEvent = "cancelevent";

	public const string KeyCancelNextCellValueChangedEvent = "cancelnextcellvaluechangedevent";

	public const string KeyCancelNextSelectedChangedEvent = "cancelnextselectedchangedevent";

	public const string KeyCancelNextFocusObjectChangedEvent = "cancelnextfocusobjectchangedevent";

	public const string KeyCurrentUser = "@username";

	public const string KeyCurrentBranch = "@branchid";

	public const string KeyUser = "@userid";

	public const string KeyRowHandelCellValueChanged = "@rowindex";

	public const string KeyCurrentDate = "@currentdate";

	public const string KeyCurrentTime = "@currenttime";

	public const string KeyExecutePath = "@executepath";

	public const string KeyNow = "now";

	public const string KeyGetDate = "getdate";

	public const string KeyInsert = "insert";

	public const string KeyYesNo = "yesno";

	public const string KeyOkCancel = "okcancel";

	public const string KeyShowError = "showerror";

	public const string KeyShowWarring = "showwarning";

	public const string KeyShowSuccess = "showsuccess";

	public const string KeyShowInfo = "showinfo";

	public const string KeySumColumns = "sumcolumns";

	public const string KeySumRowsCoulumns = "sumrowscolumns";

	public const string KeyDateAddDay = "dateaddday";

	public const string KeyDateAddMonth = "dateaddmonth";

	public const string KeyDateAddYear = "dateaddyear";

	public const string KeyDateGetDay = "dategetday";

	public const string KeyDateGetMonth = "dategetmonth";

	public const string KeyDateGetYear = "dategetyear";

	public const string KeyDateToString = "datetostring";

	public const string KeySelectedRows = "selectedrows";

	public const string KeyFocusedColumn = "focusedcolumn";

	public const string KeyIsSelectedRow = "isselectedrow";

	public const string KeyPush = "push";

	public const string KeyClear = "clear";

	public const string KeySelectAll = "selectall";

	public const string KeyClearSelection = "clearselection";

	public const string KeyReadOnly = "readonly";

	public const string KeyEnable = "enable";

	public const string KeyColor = "color";

	public const string KeyDetele = "delete";

	public const string KeySelected = "selected";

	public const string KeyUnSelected = "unselected";

	public const string KeyLength = "length";

	public const string KeyTotalDays = "totaldays";

	public const string KeyTotalHours = "totalhours";

	public const string KeyTotalMinutes = "totalminutes";

	public const string KeyTotalSeconds = "totalseconds";

	public const string KeyTotalMilliseconds = "totalmilliseconds";

	public const string KeyDayOfWeek = "dayofweek";

	public const string KeyDayOfYear = "dayofyear";

	public const string KeyHttpResult = "httpresult";

	public const string KeyInvoke = "invoke";

	public const string KeyMessage = "message";

	public const string KeyContent = "content";

	public const string KeyForm = "form";

	public const string KeyDialog = "dialog";

	public const string KeyDataSource = "datasource";

	public const string KeyShowForm = "showform";

	public const string KeyShowDialog = "showdialog";

	public const string KeyDialogResult = "dialogresult";

	public const string KeyRound = "round";

	public const string KeyMergePDF = "mergepdf";

	public const string ColorKeyAliceBlue = "aliceblue";

	public const string ColorKeyAntiqueWhite = "antiquewhite";

	public const string ColorKeyAqua = "aqua";

	public const string ColorKeyAquamarine = "aquamarine";

	public const string ColorKeyAzure = "azure";

	public const string ColorKeyBeige = "beige";

	public const string ColorKeyBisque = "bisque";

	public const string ColorKeyBlack = "black";

	public const string ColorKeyBlanchedAlmond = "blanchedalmond";

	public const string ColorKeyBlue = "blue";

	public const string ColorKeyBlueViolet = "blueviolet";

	public const string ColorKeyBrown = "brown";

	public const string ColorKeyBurlyWood = "burlywood";

	public const string ColorKeyCadetBlue = "cadetblue";

	public const string ColorKeyChartreuse = "chartreuse";

	public const string ColorKeyChocolate = "chocolate";

	public const string ColorKeyCoral = "coral";

	public const string ColorKeyCornflowerBlue = "cornflowerblue";

	public const string ColorKeyCornsilk = "cornsilk";

	public const string ColorKeyCrimson = "crimson";

	public const string ColorKeyCyan = "cyan";

	public const string ColorKeyDarkBlue = "darkblue";

	public const string ColorKeyDarkCyan = "darkcyan";

	public const string ColorKeyDarkGoldenrod = "darkgoldenrod";

	public const string ColorKeyDarkGray = "darkgray";

	public const string ColorKeyDarkGreen = "darkgreen";

	public const string ColorKeyDarkKhaki = "darkkhaki";

	public const string ColorKeyDarkMagenta = "darkmagenta";

	public const string ColorKeyDarkOliveGreen = "darkolivegreen";

	public const string ColorKeyDarkOrange = "darkorange";

	public const string ColorKeyDarkOrchid = "darkorchid";

	public const string ColorKeyDarkRed = "darkred";

	public const string ColorKeyDarkSalmon = "darksalmon";

	public const string ColorKeyDarkSeaGreen = "darkseagreen";

	public const string ColorKeyDarkSlateBlue = "darkslateblue";

	public const string ColorKeyDarkSlateGray = "darkslategray";

	public const string ColorKeyDarkTurquoise = "darkturquoise";

	public const string ColorKeyDarkViolet = "darkviolet";

	public const string ColorKeyDeepPink = "deeppink";

	public const string ColorKeyDeepSkyBlue = "deepSkyblue";

	public const string ColorKeyDimGray = "dimgray";

	public const string ColorKeyDodgerBlue = "dodgerblue";

	public const string ColorKeyFirebrick = "firebrick";

	public const string ColorKeyFloralWhite = "floralwhite";

	public const string ColorKeyForestGreen = "forestgreen";

	public const string ColorKeyFuchsia = "fuchsia";

	public const string ColorKeyGainsboro = "gainsboro";

	public const string ColorKeyGhostWhite = "ghostwhite";

	public const string ColorKeyGold = "gold";

	public const string ColorKeyGoldenrod = "goldenrod";

	public const string ColorKeyGray = "gray";

	public const string ColorKeyGreen = "green";

	public const string ColorKeyGreenYellow = "greenyellow";

	public const string ColorKeyHoneydew = "honeydew";

	public const string ColorKeyHotPink = "hotpink";

	public const string ColorKeyIndianRed = "indianred";

	public const string ColorKeyIndigo = "indigo";

	public const string ColorKeyIvory = "ivory";

	public const string ColorKeyKhaki = "khaki";

	public const string ColorKeyLavender = "lavender";

	public const string ColorKeyLavenderBlush = "lavenderblush";

	public const string ColorKeyLawnGreen = "lawngreen";

	public const string ColorKeyLemonChiffon = "lemonchiffon";

	public const string ColorKeyLightBlue = "lightblue";

	public const string ColorKeyLightCoral = "lightcoral";

	public const string ColorKeyLightCyan = "lightcyan";

	public const string ColorKeyLightGoldenrodYellow = "lightgoldenrodyellow";

	public const string ColorKeyLightGray = "lightgray";

	public const string ColorKeyLightGreen = "lightgreen";

	public const string ColorKeyLightPink = "lightpink";

	public const string ColorKeyLightSalmon = "lightsalmon";

	public const string ColorKeyLightSeaGreen = "lightseagreen";

	public const string ColorKeyLightSkyBlue = "lightskyblue";

	public const string ColorKeyLightSlateGray = "lightslategray";

	public const string ColorKeyLightSteelBlue = "lightsteelblue";

	public const string ColorKeyLightYellow = "lightyellow";

	public const string ColorKeyLime = "lime";

	public const string ColorKeyLimeGreen = "limegreen";

	public const string ColorKeyLinen = "linen";

	public const string ColorKeyMagenta = "magenta";

	public const string ColorKeyMaroon = "maroon";

	public const string ColorKeyMediumAquamarine = "mediumaquamarine";

	public const string ColorKeyMediumBlue = "mediumblue";

	public const string ColorKeyMediumOrchid = "mediumorchid";

	public const string ColorKeyMediumPurple = "mediumpurple";

	public const string ColorKeyMediumSeaGreen = "mediumseagreen";

	public const string ColorKeyMediumSlateBlue = "mediumslateblue";

	public const string ColorKeyMediumSpringGreen = "mediumspringgreen";

	public const string ColorKeyMediumTurquoise = "mediumturquoise";

	public const string ColorKeyMediumVioletRed = "mediumvioletred";

	public const string ColorKeyMidnightBlue = "midnightblue";

	public const string ColorKeyMintCream = "mintcream";

	public const string ColorKeyMistyRose = "mistyrose";

	public const string ColorKeyMoccasin = "moccasin";

	public const string ColorKeyNavajoWhite = "navajowhite";

	public const string ColorKeyNavy = "navy";

	public const string ColorKeyOldLace = "oldlace";

	public const string ColorKeyOlive = "olive";

	public const string ColorKeyOliveDrab = "olivedrab";

	public const string ColorKeyOrange = "orange";

	public const string ColorKeyOrangeRed = "orangered";

	public const string ColorKeyOrchid = "orchid";

	public const string ColorKeyPaleGoldenrod = "palegoldenrod";

	public const string ColorKeyPaleGreen = "palegreen";

	public const string ColorKeyPaleTurquoise = "paleturquoise";

	public const string ColorKeyPaleVioletRed = "palevioletred";

	public const string ColorKeyPapayaWhip = "papayawhip";

	public const string ColorKeyPeachPuff = "peachpuff";

	public const string ColorKeyPeru = "peru";

	public const string ColorKeyPink = "pink";

	public const string ColorKeyPlum = "plum";

	public const string ColorKeyPowderBlue = "powderblue";

	public const string ColorKeyPurple = "purple";

	public const string ColorKeyRed = "red";

	public const string ColorKeyRosyBrown = "rosybrown";

	public const string ColorKeyRoyalBlue = "royalblue";

	public const string ColorKeySaddleBrown = "saddlebrown";

	public const string ColorKeySalmon = "salmon";

	public const string ColorKeySandyBrown = "sandybrown";

	public const string ColorKeySeaGreen = "seagreen";

	public const string ColorKeySeaShell = "seashell";

	public const string ColorKeySienna = "sienna";

	public const string ColorKeySilver = "silver";

	public const string ColorKeySkyBlue = "skyblue";

	public const string ColorKeySlateBlue = "slateblue";

	public const string ColorKeySlateGray = "slategray";

	public const string ColorKeySnow = "snow";

	public const string ColorKeySpringGreen = "springgreen";

	public const string ColorKeySteelBlue = "steelblue";

	public const string ColorKeyTan = "tan";

	public const string ColorKeyTeal = "teal";

	public const string ColorKeyThistle = "thistle";

	public const string ColorKeyTomato = "tomato";

	public const string ColorKeyTransparent = "transparent";

	public const string ColorKeyTurquoise = "turquoise";

	public const string ColorKeyViolet = "violet";

	public const string ColorKeyWheat = "wheat";

	public const string ColorKeyWhite = "white";

	public const string ColorKeyWhiteSmoke = "whitesmoke";

	public const string ColorKeyYellow = "yellow";

	public const string ColorKeyYellowGreen = "yellowgreen";

	public const string OperatorEndStatment = ";";

	public const string OperatorStartItem = "$";

	public const string OperatorAdd = "+";

	public const string OperatorMinus = "-";

	public const string OperatorMutiple = "*";

	public const string OperatorPower = "^";

	public const string OperatorDivision = "/";

	public const string OperatorDivisionToRemainder = "%";

	public const string OperatorOpenRoundBracket = "(";

	public const string OperatorCloseRoundBracket = ")";

	public const string OperatorOpenCurlyBraces = "{";

	public const string OperatorCloseCurlyBraces = "}";

	public const string OperatorComma = ",";

	public const string OperatorOpenSquareBracket = "[";

	public const string OperatorCloseSquareBracket = "]";

	public const string OperatorSmallerMark = "<";

	public const string OperatorLargerMark = ">";

	public const string OperatorEqualMark = "=";

	public const string OperatorSpaceMark = " ";

	public const string OperatorDotMark = ".";

	public const string OperatorGreaterOrEqual = ">=";

	public const string OperatorLessOrEqual = "<=";

	public const string OperatorDiffirentMark = "!=";

	public const string OperatorDiffirentMarkSql = "<>";

	public const string OperatorComparatorMark = "==";

	public const string OperatorHashtag = "!";

	public const string OperatorQuationMark = "\"";

	public const string OperatorParenthess = "'";

	public const string OperatorCrossOver = "/";

	public static string[] KeyWord = new string[259]
	{
		"int", "double", "string", "datetime", "date", "list", "datatable", "bool", "dataset", "abs",
		"columnedit", "if", "for", "loop", "or", "and", "else", "var", "execute", "showmessagebox",
		"while", "tolower", "toupper", "trim", "sqrt", "getdata", "datarow", "true", "substring", "split",
		"replace", "firstindexstring", "add", "getrowcellvalue", "getfocusrowcellvalue", "setrowcellvalue", "setfocusrowcellvalue", "max", "min", "count",
		"avg", "focusrow", "table", "sum", "rows", "columns", "cancelevent", "return", "yesno", "okcancel",
		"showerror", "showwarning", "showsuccess", "showinfo", "break", "continue", "sumcolumns", "sumrowscolumns", "dateaddday", "dateaddmonth",
		"dateaddyear", "showinfo", "dategetday", "dategetmonth", "dategetyear", "datetostring", "getdate", "push", "selectedrows", "focusedcolumn",
		"isselectedrow", "hide", "show", "newrow", "focus", "clear", "selectall", "clearselection", "enable", "readonly",
		"color", "delete", "selected", "unselected", "length", "totaldays", "totalhours", "totalminutes", "totalseconds", "totalmilliseconds",
		"dayofweek", "dayofyear", "aliceblue", "antiquewhite", "aqua", "aquamarine", "azure", "beige", "bisque", "black",
		"blanchedalmond", "blue", "blueviolet", "brown", "burlywood", "cadetblue", "chartreuse", "chocolate", "coral", "cornflowerblue",
		"cornsilk", "crimson", "cyan", "darkblue", "darkcyan", "darkgoldenrod", "darkgreen", "darkkhaki", "darkmagenta", "darkolivegreen",
		"darkorange", "darkorchid", "darkred", "darksalmon", "darkseagreen", "darkslateblue", "darkslategray", "darkturquoise", "darkviolet", "deeppink",
		"deepSkyblue", "dimgray", "dodgerblue", "firebrick", "floralwhite", "forestgreen", "fuchsia", "gainsboro", "ghostwhite", "gold",
		"goldenrod", "gray", "green", "greenyellow", "honeydew", "hotpink", "indianred", "indigo", "ivory", "khaki",
		"lavender", "lavenderblush", "lawngreen", "lemonchiffon", "lightblue", "lightcoral", "lightcyan", "lightgoldenrodyellow", "lightgreen", "lightpink",
		"lightsalmon", "lightseagreen", "lightskyblue", "lightslategray", "lightsteelblue", "lightyellow", "lime", "limegreen", "linen", "magenta",
		"maroon", "mediumaquamarine", "mediumblue", "mediumorchid", "mediumpurple", "mediumseagreen", "mediumslateblue", "mediumspringgreen", "mediumturquoise", "mediumvioletred",
		"midnightblue", "mintcream", "mistyrose", "moccasin", "navajowhite", "navy", "oldlace", "olive", "olivedrab", "orange",
		"orangered", "orchid", "palegoldenrod", "palegreen", "paleturquoise", "palevioletred", "papayawhip", "peachpuff", "peru", "pink",
		"plum", "powderblue", "purple", "red", "rosybrown", "royalblue", "saddlebrown", "salmon", "sandybrown", "seagreen",
		"seashell", "sienna", "silver", "skyblue", "slateblue", "slategray", "snow", "springgreen", "steelblue", "tan",
		"teal", "thistle", "tomato", "transparent", "turquoise", "violet", "wheat", "white", "whitesmoke", "yellow",
		"yellowgreen", "invoke", "message", "content", "httpresult", "form", "showform", "showdialog", "datasource", "dialog",
		"dialogresult", "contains", "cancelnextcellvaluechangedevent", "cancelnextfocusobjectchangedevent", "cancelnextselectedchangedevent", "setdatasourcecell", "setfocusdatasourcecell", "round", "first", "firstduplicate",
		"stringagg", "openlink", "drilldown", "processsapobject", "mergepdf", "getparentrowhandle", "getchildrowcount", "getchildrowhandle", "format"
	};

	public static string[] KeyWordOperator = new string[2] { "or", "and" };

	public static string[] KeyworkMath = new string[11]
	{
		"abs", "tolower", "toupper", "trim", "sqrt", "substring", "split", "replace", "firstindexstring", "round",
		"format"
	};

	public static string[] OperatorApz = new string[18]
	{
		"+", "-", "*", "/", "%", "^", "(", ")", ";", "{",
		"}", ",", "[", "]", "<", ">", ".", "$"
	};

	public static string[] DataTableKeyWordApz = new string[15]
	{
		"count", "focusrow", "columnedit", "columns", "rows", "selectedrows", "focusedcolumn", "isselectedrow", "clear", "selectall",
		"clearselection", "contains", "first", "firstduplicate", "stringagg"
	};

	public static string[] MathOperator = new string[15]
	{
		"+", "-", "*", "/", "%", "^", "<>", ">", "<", ">=",
		"<=", "!=", "==", "and", "or"
	};

	public static string[] NumericOperation = new string[6] { "+", "-", "*", "/", "%", "^" };

	public static string[] SpecialCharactersApz = new string[26]
	{
		"+", "-", "*", "/", "%", "^", "(", ")", "=", "!",
		";", " ", ">", "<", "{", "}", ",", ".", "[", "]",
		"$", "\t", "\n", "\r", "\"", "'"
	};

	public static string[] OperatorMathBoolean = new string[2] { "and", "or" };

	public static string[] PrefixComparisonApz = new string[4] { "<", "=", "!", ">" };

	public static string[] ListBoolVariable = new string[2] { "true", "false" };

	public static string[] TypeBase = new string[4] { "int", "double", "string", "bool" };

	public static string[] ListOperatorEndStatment = new string[7] { "[", "]", "{", "}", ";", ",", ")" };

	public static string[] ListOperatorCopare = new string[7] { "<", ">", "==", ">=", "<=", "!=", "<>" };

	public static string[] ListOperationBeginFormula = new string[2] { "-", "+" };

	public static string[] ListSystemValueInt = new string[4] { "@branchid", "@userid", "@rowindex", "@currenttime" };

	public static string[] ListSystemValueString = new string[2] { "@username", "@executepath" };

	public static string[] ListSystemValueDouble = new string[0];

	public static string[] ListSystemValueDatetime = new string[1] { "@currentdate" };
}
