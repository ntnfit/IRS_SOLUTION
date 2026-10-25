using System;
using System.Collections.Generic;
using System.Data;
using Apzon.Forms.Commons.CFL;
using DevExpress.XtraGrid.Views.Grid;

namespace Apzon.Forms.UserControls;

public class ChooseFormListUDFEventArgs : EventArgs
{
	private readonly DataRow chooseItem;

	private readonly List<DataRow> chooseItems;

	private readonly GridView view;

	private readonly frm_CFL formCFL;

	public DataRow ChooseItem => chooseItem;

	public List<DataRow> ChooseItems => chooseItems;

	public GridView View => view;

	public frm_CFL FormCFL => formCFL;

	public ChooseFormListUDFEventArgs(DataRow chooseItem, List<DataRow> chooseItems, GridView view, frm_CFL formCFL)
	{
		this.chooseItem = chooseItem;
		this.chooseItems = chooseItems;
		this.view = view;
		this.formCFL = formCFL;
	}
}
