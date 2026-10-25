using System.ComponentModel;

namespace Apzon.Entities.Models.Common;

public class Common
{
	public class ListItems
	{
		[DefaultValue("")]
		public string Value { get; set; }

		[DefaultValue("")]
		public string Text { get; set; }

		public object Tag { get; set; }

		public ListItems()
		{
		}

		public ListItems(string text)
		{
			Text = text;
		}

		public ListItems(string text, string value)
		{
			Text = text;
			Value = value;
		}

		public ListItems(string text, string value, object tag)
		{
			Text = text;
			Value = value;
			Tag = tag;
		}

		public override string ToString()
		{
			return Text;
		}
	}

	public class Table_Code
	{
		public string Code { get; set; }

		public string Name { get; set; }

		public Table_Code()
		{
		}

		public Table_Code(string code, string name)
		{
			Code = code;
			Name = name;
		}
	}

	public class Table_ID
	{
		public object ID { get; set; }

		public string Name { get; set; }
	}

	public class TableIdCode
	{
		public string Id { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }

		public TableIdCode()
		{
		}

		public TableIdCode(string id, string code, string name)
		{
			Id = id;
			Code = code;
			Name = name;
		}
	}

	public class Table_ID_Code
	{
		public object ID { get; set; }

		public string Code { get; set; }

		public string Name { get; set; }

		public Table_ID_Code()
		{
		}

		public Table_ID_Code(object id, string code, string name)
		{
			ID = id;
			Code = code;
			Name = name;
		}
	}

	public class ParameterValue
	{
		public object Value { get; set; }

		[DefaultValue("")]
		public string Name { get; set; }

		public ParameterValue()
		{
		}

		public ParameterValue(string name)
		{
			Name = name;
		}

		public ParameterValue(string name, object value)
		{
			Name = name;
			Value = value;
		}
	}
}
