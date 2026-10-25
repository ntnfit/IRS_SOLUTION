using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;

namespace Apzon.Commons.Helper;

public static class DynamicExtensions
{
	public static dynamic ToDynamic(this object value)
	{
		IDictionary<string, object> dictionary = new ExpandoObject();
		foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(value.GetType()))
		{
			dictionary.Add(property.Name, property.GetValue(value));
		}
		return dictionary as ExpandoObject;
	}
}
