using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Apzon.Controls.Commons;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.XtraRichEdit.Services;

namespace Apzon.Forms.Commons;

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

	private readonly SyntaxHighlightProperties _functionSettings = new SyntaxHighlightProperties
	{
		ForeColor = Color.DarkViolet
	};

	private readonly SyntaxHighlightProperties _stringSettings = new SyntaxHighlightProperties
	{
		ForeColor = Color.Red
	};

	private readonly SyntaxHighlightProperties _commentSettings = new SyntaxHighlightProperties
	{
		ForeColor = Color.MediumSeaGreen
	};

	private readonly SyntaxHighlightProperties _globalSettings = new SyntaxHighlightProperties
	{
		ForeColor = Color.DeepSkyBlue
	};

	private readonly SyntaxHighlightProperties _keyColorSettings = new SyntaxHighlightProperties
	{
		ForeColor = Color.CadetBlue
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
			DocumentRange[] array = _document.FindAll("\"", SearchOptions.None);
			for (int i = 0; i < array.Length / 2; i++)
			{
				list.Add(new SyntaxHighlightToken(array[i * 2].Start.ToInt(), array[i * 2 + 1].Start.ToInt() - array[i * 2].Start.ToInt() + 1, _stringSettings));
			}
			DocumentRange[] array2 = _document.FindAll("/*", SearchOptions.None);
			for (int j = 0; j < array2.Length / 2; j++)
			{
				list.Add(new SyntaxHighlightToken(array2[j * 2].Start.ToInt(), array2[j * 2 + 1].Start.ToInt() - array2[j * 2].Start.ToInt() + 1, _commentSettings));
			}
			for (int k = 0; k < ConstantsApzCtd.HighlightKeywords.Length; k++)
			{
				array = _document.FindAll(ConstantsApzCtd.HighlightKeywords[k], SearchOptions.WholeWord);
				for (int l = 0; l < array.Length; l++)
				{
					if (!IsRangeInTokens(array[l], list))
					{
						list.Add(new SyntaxHighlightToken(array[l].Start.ToInt(), array[l].Length, _keywordSettings));
					}
				}
			}
			for (int m = 0; m < ConstantsApzCtd.HighlightFunctions.Length; m++)
			{
				array = _document.FindAll(ConstantsApzCtd.HighlightFunctions[m], SearchOptions.WholeWord);
				for (int n = 0; n < array.Length; n++)
				{
					if (!IsRangeInTokens(array[n], list))
					{
						list.Add(new SyntaxHighlightToken(array[n].Start.ToInt(), array[n].Length, _functionSettings));
					}
				}
			}
			for (int num = 0; num < ConstantsApzCtd.HighlightGlobal.Length; num++)
			{
				array = _document.FindAll(ConstantsApzCtd.HighlightGlobal[num], SearchOptions.WholeWord);
				for (int num2 = 0; num2 < array.Length; num2++)
				{
					if (!IsRangeInTokens(array[num2], list))
					{
						list.Add(new SyntaxHighlightToken(array[num2].Start.ToInt(), array[num2].Length, _globalSettings));
					}
				}
			}
			for (int num3 = 0; num3 < ConstantsApzCtd.ColorKeyWork.Length; num3++)
			{
				array = _document.FindAll(ConstantsApzCtd.ColorKeyWork[num3], SearchOptions.WholeWord);
				for (int num4 = 0; num4 < array.Length; num4++)
				{
					if (!IsRangeInTokens(array[num4], list))
					{
						list.Add(new SyntaxHighlightToken(array[num4].Start.ToInt(), array[num4].Length, _keyColorSettings));
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
