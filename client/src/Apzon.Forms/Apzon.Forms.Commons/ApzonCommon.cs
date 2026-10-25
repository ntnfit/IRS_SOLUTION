using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Controls;
using Apzon.Controls.Form;
using Apzon.Controls.Grid;
using Apzon.Entities.Models.Documents;

namespace Apzon.Forms.Commons;

public class ApzonCommon
{
	public delegate void ShowForms(XtraFormEx frm);

	public static ShowForms frmShowChildrenForm;

	public static Color colorFocusRow = Color.PaleGoldenrod;

	public static Form GetActiveForm()
	{
		FormCollection openForms = Application.OpenForms;
		for (int i = 0; i < openForms.Count; i++)
		{
			Form form = openForms[i];
			if (!form.IsDisposed && !form.Disposing && form.IsMdiContainer)
			{
				Form activeMdiChild = form.ActiveMdiChild;
				if (activeMdiChild != null)
				{
					return activeMdiChild;
				}
			}
		}
		return null;
	}

	public static Form GetPopupForm()
	{
		FormCollection openForms = Application.OpenForms;
		if (openForms.Count == 0)
		{
			return null;
		}
		Form form = openForms["Main"];
		if (form == null || form.ActiveMdiChild == null)
		{
			return null;
		}
		Form activeMdiChild = form.ActiveMdiChild;
		for (int num = openForms.Count - 1; num >= 0; num--)
		{
			Form form2 = openForms[num];
			if (!form2.IsDisposed && !form2.Disposing)
			{
				if (form2.Modal)
				{
					if (form2.GetType() != typeof(Processing))
					{
						return form2;
					}
				}
				else if (!form2.IsMdiContainer && form2.MdiParent == null)
				{
					Form ownerForm = GetOwnerForm(form2);
					if (ownerForm != null && ownerForm.Name == activeMdiChild.Name)
					{
						return form2;
					}
				}
			}
		}
		return activeMdiChild;
	}

	private static Form GetOwnerForm(Form chilForm)
	{
		while (chilForm != null && chilForm.Owner != null)
		{
			chilForm = chilForm.Owner;
		}
		return chilForm;
	}

	public static List<Form> GetListChildForm()
	{
		List<Form> list = new List<Form>();
		FormCollection openForms = Application.OpenForms;
		if (openForms.Count == 0)
		{
			return list;
		}
		for (int i = 0; i < openForms.Count; i++)
		{
			Form form = openForms[i];
			if (form.IsDisposed || form.Disposing || !form.GetType().Namespace.StartsWith("Apzon."))
			{
				continue;
			}
			if (form.Modal)
			{
				if (form.GetType() != typeof(Processing))
				{
					list.Add(form);
				}
			}
			else if (!(form.GetType() == typeof(Main)))
			{
				list.Add(form);
			}
		}
		return list;
	}

	public static bool CopyFromToDoc(ApzObjectTypes typeFrom, List<DocumentsImpl> lstCopy, DocumentsImpl toDoc)
	{
		try
		{
			string text = string.Empty;
			if (lstCopy.Count == 0)
			{
				return false;
			}
			for (int i = 0; i < lstCopy.Count; i++)
			{
				DocumentsImpl documentsImpl = lstCopy[i];
				if (i > 0 && !text.Equals(documentsImpl.CardCode))
				{
					WinCommons.writeStatus(Enums.MessageCode.CardCodeBeSame, Enums.MessageType.Error, Msg_Time._TimeMedium, "");
					return false;
				}
				text = documentsImpl.CardCode;
				switch (typeFrom)
				{
				case ApzObjectTypes.oOrders:
					toDoc.Comments += $"Base on SalesOrder {documentsImpl.DocEntry}. ";
					break;
				case ApzObjectTypes.oQuotations:
					toDoc.Comments += $"Base on SalesQuotation {documentsImpl.DocEntry}. ";
					break;
				case ApzObjectTypes.oDeliveryNotes:
					toDoc.Comments += $"Base on Delivery {documentsImpl.DocEntry}. ";
					break;
				case ApzObjectTypes.oDownPayments:
					toDoc.Comments += $"Base on A/R DownPayment {documentsImpl.DocEntry}. ";
					break;
				case ApzObjectTypes.oInvoices:
					toDoc.Comments += $"Base on A/R Invoice {documentsImpl.DocEntry}. ";
					break;
				case ApzObjectTypes.oPurchaseInvoices:
					toDoc.Comments += $"Base on A/P Invoice {documentsImpl.DocEntry}. ";
					break;
				case ApzObjectTypes.oPurchaseCreditNotes:
					toDoc.Comments += $"Base on A/P Credit Memo {documentsImpl.DocEntry}. ";
					break;
				case ApzObjectTypes.oPurchaseRequest:
					toDoc.Comments += $"Base on PurchaseRequest {documentsImpl.DocEntry}. ";
					break;
				case ApzObjectTypes.oPurchaseOrders:
					toDoc.Comments += $"Base on PurchaseOrders {documentsImpl.DocEntry}. ";
					break;
				case ApzObjectTypes.oDrafts:
					toDoc.Comments += $"Base on Document Draft {documentsImpl.DocEntry}. ";
					break;
				default:
					toDoc.ObjType = -1;
					break;
				}
				int num = 0;
				if (toDoc.Lines != null && toDoc.Lines.Count > 0)
				{
					num = toDoc.Lines.Max((Document_LinesImpl x) => x.LineNum) + 1;
				}
				if (documentsImpl.Lines != null)
				{
					foreach (Document_LinesImpl line in documentsImpl.Lines)
					{
						line.BaseEntry = line.DocEntry;
						line.BaseLine = line.LineNum;
						line.BaseType = Convert.ToInt32(typeFrom);
						line.LineNum = num++;
						line.DocEntry = toDoc.DocEntry;
						line.Quantity = line.RemainingOpenQuantity;
						line.LineTotal = line.Quantity * line.Price * (1.0 - line.DiscPrcnt);
						toDoc.TotalBeforeDis += line.LineTotal;
					}
					toDoc.Lines.InsertRange(0, documentsImpl.Lines);
				}
				toDoc.DiscPrcnt += documentsImpl.DiscPrcnt;
				toDoc.DocTotal += documentsImpl.DocTotal;
				toDoc.VatSum += documentsImpl.VatSum;
				toDoc.CardCode = documentsImpl.CardCode;
				toDoc.CardName = documentsImpl.CardName;
				toDoc.CntctCode = documentsImpl.CntctCode;
				toDoc.SlpCode = documentsImpl.SlpCode;
			}
			toDoc.ObjTypeFrom = Convert.ToInt32(typeFrom);
			toDoc.TypeFrom = typeFrom;
			toDoc.DocDueDate = DateTime.Now;
			toDoc.DocDate = DateTime.Now;
			toDoc.TaxDate = DateTime.Now;
			toDoc.DocStatus = "O";
			toDoc.DocStatusName = "Open";
			return true;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), "CopyFromToDoc_ApzonCommos " + ex.Message);
			return false;
		}
	}

	public static void WriteStatusHTTPOnTaskBar(HttpStatusCode statusCode)
	{
		string status = HttpHelper.GetStatus(statusCode);
		WinCommons.writeStatus(Enums.MessageCode.None, Enums.MessageType.Error, Msg_Time._TimeMedium, status);
	}

	public static double LineTotal(double qty, double price, double disPercent)
	{
		return qty * price * (1.0 - disPercent / 100.0);
	}

	public static double DocTotal(double totalBefDis, double totalTax, double disPercent, double freight, double dpmTotal = 0.0, int decimals = 0)
	{
		try
		{
			double num = Math.Round(totalBefDis * disPercent / 100.0, decimals, MidpointRounding.AwayFromZero);
			return Math.Round(totalBefDis - num + totalTax + freight - dpmTotal, decimals, MidpointRounding.AwayFromZero);
		}
		catch (Exception ex)
		{
			WinCommons.ErrorLineNumber(ex);
			return 0.0;
		}
	}

	public static XtraFormEx GetParentsForm(XtraFormEx frm)
	{
		return (XtraFormEx)frm.MdiParent;
	}

	public static void SetColorRowSelected(ref GridViewEx gv)
	{
		try
		{
			Color backColor = Color.FromArgb(254, 234, 163);
			gv.Appearance.HideSelectionRow.BackColor = backColor;
			gv.Appearance.FocusedRow.BackColor = backColor;
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			WinCommons.Error(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
