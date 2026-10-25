using System;
using FastColoredTextBoxNS;

namespace Apzon.Forms.Commons;

public class MethodAutocompleteItem2 : MethodAutocompleteItem
{
	private string firstPart;

	private string lowercaseText;

	public MethodAutocompleteItem2(string text)
		: base(text)
	{
		lowercaseText = Text.ToLower();
	}

	public override CompareResult Compare(string fragmentText)
	{
		int num = fragmentText.LastIndexOf('.');
		if (num < 0)
		{
			return CompareResult.Hidden;
		}
		string text = fragmentText.Substring(num + 1);
		if (text.Length < Parent.MinFragmentLength)
		{
			return CompareResult.Hidden;
		}
		firstPart = fragmentText.Substring(0, num);
		if (text == "")
		{
			return CompareResult.Visible;
		}
		if (Text.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
		{
			return CompareResult.VisibleAndSelected;
		}
		if (lowercaseText.Contains(text.ToLower()))
		{
			return CompareResult.Visible;
		}
		return CompareResult.Hidden;
	}

	public override string GetTextForReplace()
	{
		return firstPart + "." + Text;
	}
}
