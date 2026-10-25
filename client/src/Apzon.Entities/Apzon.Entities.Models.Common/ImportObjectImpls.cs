using Apzon.Commons.Enums;

namespace Apzon.Entities.Models.Common;

public class ImportObjectImpls
{
	private Enums.ImportObjectEnum _code;

	private string _name;

	private Enums.ImportObjectEnum _parent;

	private Enums.ImportCategoryEnum _category;

	private string _tableName;

	private bool _isTreeNode;

	private bool _isObject;

	public Enums.ImportObjectEnum Code => _code;

	public string Name => _name;

	public Enums.ImportObjectEnum Parent => _parent;

	public Enums.ImportCategoryEnum Category => _category;

	public string TableName => _tableName;

	public bool IsTreeNode => _isTreeNode;

	public bool IsObject => _isObject;

	public ImportObjectImpls(Enums.ImportObjectEnum code, string name, Enums.ImportObjectEnum parent, Enums.ImportCategoryEnum category, string tableName, bool isTreeNode, bool isObject)
	{
		_code = code;
		_name = name;
		_parent = parent;
		_category = category;
		_tableName = tableName;
		_isTreeNode = isTreeNode;
		_isObject = isObject;
	}

	public ImportObjectImpls()
	{
		_code = Enums.ImportObjectEnum.None;
		_name = string.Empty;
		_parent = Enums.ImportObjectEnum.None;
		_category = Enums.ImportCategoryEnum.None;
		_tableName = "";
		_isTreeNode = false;
		_isObject = false;
	}
}
