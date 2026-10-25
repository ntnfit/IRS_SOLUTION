using System.ComponentModel;
using Apzon.Commons.Enums;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraEditors.Repository;
using DevExpress.XtraGrid.Views.Grid;
using DevExpress.XtraTreeList;

namespace Apzon.Controls.Editor.ChooseFromList;

[ToolboxItem(true)]
public class RepChooseFromList : RepositoryItemButtonEdit
{
	public Enums.TypeCFL TypeCfl;

	public bool MultiSelect;

	public bool IsSearchControl { get; set; }

	public GridView GridView { get; set; }

	public TreeList TreeList { get; set; }

	public string FieldName { get; set; }

	public RepChooseFromList()
	{
		Buttons[0].Kind = ButtonPredefines.Search;
	}
}
