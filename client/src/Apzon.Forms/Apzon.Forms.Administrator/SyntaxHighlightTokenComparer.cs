using System.Collections.Generic;
using DevExpress.XtraRichEdit.API.Native;

namespace Apzon.Forms.Administrator;

public class SyntaxHighlightTokenComparer : IComparer<SyntaxHighlightToken>
{
	public int Compare(SyntaxHighlightToken x, SyntaxHighlightToken y)
	{
		return x.Start - y.Start;
	}
}
