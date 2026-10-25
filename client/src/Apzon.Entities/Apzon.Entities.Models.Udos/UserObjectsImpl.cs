using System.Collections.Generic;
using SAPbobsCOM;

namespace Apzon.Entities.Models.Udos;

public class UserObjectsImpl
{
	public bool CanArchive { get; set; }

	public bool CanCancel { get; set; }

	public bool CanClose { get; set; }

	public bool CanCreateDefaultForm { get; set; }

	public bool CanDelete { get; set; }

	public bool CanFind { get; set; }

	public bool CanLog { get; set; }

	public bool CanYearTransfer { get; set; }

	public List<UserObjectChildTableImpl> ChildTables { get; set; }

	public string Code { get; set; }

	public bool EnableEnhancedForm { get; set; }

	public List<UserObjectEnhancedFormColumnImpl> EnhancedFormColumns { get; set; }

	public string ExtensionName { get; set; }

	public int FatherMenuId { get; set; }

	public List<UserObjectFindColumnImpl> FindColumns { get; set; }

	public List<UserObjectFormColumnImpl> FormColumns { get; set; }

	public string FormSrf { get; set; }

	public string LogTableName { get; set; }

	public bool ManageSeries { get; set; }

	public string MenuCaption { get; set; }

	public bool MenuItem { get; set; }

	public string MenuUid { get; set; }

	public string Name { get; set; }

	public BoUDOObjType ObjectType { get; set; }

	public bool OverwriteDllfile { get; set; }

	public int Position { get; set; }

	public bool RebuildEnhancedForm { get; set; }

	public string TableName { get; set; }

	public bool UseUniqueFormType { get; set; }
}
