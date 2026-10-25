using System.Collections.Generic;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraRichEdit;

namespace Apzon.Forms.Service;

public class ObjectHelper
{
	private static RichEditDocumentServer rich = new RichEditDocumentServer();

	public static string GetPlainText(string htmlText)
	{
		rich.HtmlText = htmlText;
		return rich.Text.TrimStart();
	}

	public static void GetChildDataRowHandles(GridView view, int rowHandle, List<Message> list)
	{
		for (int i = 0; i < view.GetChildRowCount(rowHandle); i++)
		{
			int childRowHandle = view.GetChildRowHandle(rowHandle, i);
			if (childRowHandle >= 0)
			{
				list.Add(view.GetRow(childRowHandle) as Message);
			}
			else
			{
				GetChildDataRowHandles(view, childRowHandle, list);
			}
		}
	}
}
