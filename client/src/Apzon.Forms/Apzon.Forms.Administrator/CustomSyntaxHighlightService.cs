using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Services;

namespace Apzon.Forms.Administrator;

public class CustomSyntaxHighlightService : ISyntaxHighlightService
{
	public Timer timer;

	private readonly Document _document;

	private readonly SyntaxHighlightProperties _defaultSettings = new SyntaxHighlightProperties
	{
		ForeColor = Color.Black
	};

	private readonly SyntaxHighlightProperties _keywordSettings = new SyntaxHighlightProperties
	{
		ForeColor = Color.Blue
	};

	private readonly SyntaxHighlightProperties _stringSettings = new SyntaxHighlightProperties
	{
		ForeColor = Color.Red
	};

	private readonly string[] _keywords = new string[173]
	{
		"ADD", "ALL", "ALTER", "AND", "ANY", "AS", "ASC", "AUTHORIZATION", "BACKUP", "BEGIN",
		"BETWEEN", "BREAK", "BROWSE", "BULK", "BY", "CASCADE", "CASE", "CHECK", "CHECKPOINT", "CLOSE",
		"CLUSTERED", "COALESCE", "COLLATE", "COLUMN", "COMMIT", "COMPUTE", "CONSTRAINT", "CONTAINS", "CONTAINSTABLE", "CONTINUE",
		"CONVERT", "CREATE", "CROSS", "CURRENT", "CURRENT_DATE", "CURRENT_TIME", "CURRENT_TIMESTAMP", "CURRENT_USER", "CURSOR", "DATABASE",
		"DBCC", "DEALLOCATE", "DECLARE", "DEFAULT", "DELETE", "DENY", "DESC", "DISK", "DISTINCT", "DISTRIBUTED",
		"DOUBLE", "DROP", "DUMMY", "DUMP", "ELSE", "END", "ERRLVL", "ESCAPE", "EXCEPT", "EXEC",
		"EXECUTE", "EXISTS", "EXIT", "FETCH", "FILE", "FILLFACTOR", "FOR", "FOREIGN", "FREETEXT", "FREETEXTTABLE",
		"FROM", "FULL", "FUNCTION", "GOTO", "GRANT", "GROUP", "HAVING", "HOLDLOCK", "IDENTITY", "IDENTITY_INSERT",
		"IDENTITYCOL", "IF", "IN", "INDEX", "INNER", "INSERT", "INTERSECT", "INTO", "IS", "JOIN",
		"KEY", "KILL", "LEFT", "LIKE", "LINENO", "LOAD", "NATIONAL", "NOCHECK", "NONCLUSTERED", "NOT",
		"NULLIF", "OF", "OFF", "OFFSETS", "ON", "OPEN", "OPENDATASOURCE", "OPENQUERY", "OPENROWSET", "OPENXML",
		"OPTION", "OR", "ORDER", "OUTER", "OVER", "PERCENT", "PLAN", "PRECISION", "PRIMARY", "PRINT",
		"PROC", "PROCEDURE", "PUBLIC", "RAISERROR", "READ", "READTEXT", "RECONFIGURE", "REFERENCES", "REPLICATION", "RESTORE",
		"RESTRICT", "RETURN", "REVOKE", "RIGHT", "ROLLBACK", "ROWCOUNT", "ROWGUIDCOL", "RULE", "SAVE", "SCHEMA",
		"SELECT", "SESSION_USER", "SET", "SETUSER", "SHUTDOWN", "SOME", "STATISTICS", "SYSTEM_USER", "TABLE", "TEXTSIZE",
		"THEN", "TO", "TOP", "TRAN", "TRANSACTION", "TRIGGER", "TRUNCATE", "TSEQUAL", "UNION", "UNIQUE",
		"UPDATE", "UPDATETEXT", "USE", "USER", "VALUES", "VARYING", "VIEW", "WAITFOR", "WHEN", "WHERE",
		"WHILE", "WITH", "WRITETEXT"
	};

	public bool CheckAllowed = true;

	public CustomSyntaxHighlightService(Document document)
	{
		try
		{
			timer = new Timer
			{
				Interval = 1000
			};
			timer.Tick += timer_Tick;
			timer.Start();
			_document = document;
		}
		catch (Exception)
		{
		}
	}

	private void timer_Tick(object sender, EventArgs e)
	{
		if (!CheckAllowed)
		{
			CheckAllowed = true;
			ForceExecute();
		}
	}

	private List<SyntaxHighlightToken> ParseTokens()
	{
		List<SyntaxHighlightToken> list = new List<SyntaxHighlightToken>();
		try
		{
			DocumentRange[] array = _document.FindAll("'", SearchOptions.None);
			for (int i = 0; i < array.Length / 2; i++)
			{
				list.Add(new SyntaxHighlightToken(array[i * 2].Start.ToInt(), array[i * 2 + 1].Start.ToInt() - array[i * 2].Start.ToInt() + 1, _stringSettings));
			}
			for (int j = 0; j < _keywords.Length; j++)
			{
				array = _document.FindAll(_keywords[j], SearchOptions.WholeWord);
				for (int k = 0; k < array.Length; k++)
				{
					if (!IsRangeInTokens(array[k], list))
					{
						list.Add(new SyntaxHighlightToken(array[k].Start.ToInt(), array[k].Length, _keywordSettings));
					}
				}
			}
			list.Sort(new SyntaxHighlightTokenComparer());
			AddPlainTextTokens(list);
		}
		catch (Exception)
		{
		}
		return list;
	}

	private void AddPlainTextTokens(List<SyntaxHighlightToken> tokens)
	{
		try
		{
			int count = tokens.Count;
			if (count == 0)
			{
				tokens.Add(new SyntaxHighlightToken(0, _document.Range.End.ToInt(), _defaultSettings));
				return;
			}
			tokens.Insert(0, new SyntaxHighlightToken(0, tokens[0].Start, _defaultSettings));
			for (int i = 1; i < count; i++)
			{
				tokens.Insert(i * 2, new SyntaxHighlightToken(tokens[i * 2 - 1].End, tokens[i * 2].Start - tokens[i * 2 - 1].End, _defaultSettings));
			}
			tokens.Add(new SyntaxHighlightToken(tokens[count * 2 - 1].End, _document.Range.End.ToInt() - tokens[count * 2 - 1].End, _defaultSettings));
		}
		catch (Exception)
		{
		}
	}

	private bool IsRangeInTokens(DocumentRange range, List<SyntaxHighlightToken> tokens)
	{
		try
		{
			for (int i = 0; i < tokens.Count; i++)
			{
				if (range.Start.ToInt() >= tokens[i].Start && range.End.ToInt() <= tokens[i].End)
				{
					return true;
				}
			}
		}
		catch (Exception)
		{
		}
		return false;
	}

	public void ForceExecute()
	{
		Execute();
	}

	public void Execute()
	{
		try
		{
			if (CheckAllowed)
			{
				_document.ApplySyntaxHighlight(ParseTokens());
				CheckAllowed = false;
			}
		}
		catch (Exception)
		{
		}
	}
}
