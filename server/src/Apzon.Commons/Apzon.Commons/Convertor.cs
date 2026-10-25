using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Newtonsoft.Json;

namespace Apzon.Commons;

public static class Convertor
{
	public class Utf8StringWriter : StringWriter
	{
		public override Encoding Encoding => Encoding.UTF8;
	}

	public static string JsonToObject<T>(object data, out T obj) where T : class
	{
		obj = null;
		try
		{
			obj = JsonConvert.DeserializeObject<T>(Convert.ToString(data));
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
		return "";
	}

	public static string ObjectToJson(object obj, out string value)
	{
		value = "";
		try
		{
			value = JsonConvert.SerializeObject(obj);
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
		return "";
	}

	public static string ObjectToXML(object obj, string prefixXML, string namespaceXML, out string xml)
	{
		string result = "";
		xml = "";
		try
		{
			using Utf8StringWriter utf8StringWriter = new Utf8StringWriter();
			using XmlWriter xmlWriter = XmlWriter.Create(utf8StringWriter, new XmlWriterSettings
			{
				Indent = true
			});
			xmlWriter.WriteStartDocument(standalone: true);
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			xmlSerializerNamespaces.Add(prefixXML, namespaceXML);
			new XmlSerializer(obj.GetType()).Serialize(xmlWriter, obj, xmlSerializerNamespaces);
			xml = utf8StringWriter.ToString();
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}

	public static string ObjectToXML(object obj, List<string> prefixXML, List<string> namespaceXML, out string xml)
	{
		string result = "";
		xml = "";
		try
		{
			using Utf8StringWriter utf8StringWriter = new Utf8StringWriter();
			using XmlWriter xmlWriter = XmlWriter.Create(utf8StringWriter, new XmlWriterSettings
			{
				Indent = true
			});
			xmlWriter.WriteStartDocument(standalone: true);
			XmlSerializerNamespaces xmlSerializerNamespaces = new XmlSerializerNamespaces();
			for (int i = 0; i < prefixXML.Count; i++)
			{
				xmlSerializerNamespaces.Add(prefixXML[i], namespaceXML[i]);
			}
			new XmlSerializer(obj.GetType()).Serialize(xmlWriter, obj, xmlSerializerNamespaces);
			xml = utf8StringWriter.ToString();
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}

	public static string XMLToObject<T>(string xml, out T obj)
	{
		string result = "";
		obj = default;
		try
		{
			using TextReader textReader = new StringReader(xml);
			obj = (T)new XmlSerializer(typeof(T)).Deserialize(textReader);
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}

	public static string StringToObject<T>(string stringData, out T obj) where T : class
	{
		return StringToObject<T>(isXML: false, stringData, out obj);
	}

	public static string StringToObject<T>(bool isXML, string stringData, out T obj) where T : class
	{
		obj = null;
		if (isXML)
		{
			return XMLToObject<T>(stringData, out obj);
		}
		return JsonToObject<T>(stringData, out obj);
	}

	public static string ObjectToString<T>(T objectData, out string value) where T : class
	{
		return ObjectToString(isXML: false, objectData, out value);
	}

	public static string ObjectToString<T>(bool isXML, T objectData, out string value) where T : class
	{
		value = null;
		if (isXML)
		{
			return ObjectToXML(objectData, "", "", out value);
		}
		return ObjectToJson(objectData, out value);
	}

	public static string ObjectToBase64String(object obj, out string value)
	{
		string result = "";
		value = null;
		if (obj == null)
		{
			return "Error: Object is null @ObjectToBase64String";
		}
		try
		{
			value = ObjectToBase64String(obj);
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}

	public static string ObjectToBase64String(object obj)
	{
		if (obj is byte[])
		{
			return Convert.ToBase64String((byte[])obj);
		}
		return Convert.ToBase64String(ObjectToBytes(obj));
	}

	public static byte[] ObjectToBytes(object obj)
	{
		if (obj == null)
		{
			return null;
		}
		using MemoryStream memoryStream = new MemoryStream();
		new BinaryFormatter().Serialize(memoryStream, obj);
		return memoryStream.ToArray();
	}

	public static string Base64StringToObject(string base64, out byte[] value)
	{
		string result = "";
		value = null;
		try
		{
			value = Convert.FromBase64String(base64);
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}

	public static string Base64StringToObject(string base64, out object value)
	{
		string result = "";
		value = null;
		try
		{
			value = Base64StringToObject(base64);
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}

	public static object Base64StringToObject(string base64)
	{
		return BytesToObject(Convert.FromBase64String(base64));
	}

	public static object BytesToObject(byte[] bytes)
	{
		BinaryFormatter binaryFormatter = new BinaryFormatter();
		using MemoryStream memoryStream = new MemoryStream();
		memoryStream.Write(bytes, 0, bytes.Length);
		memoryStream.Seek(0L, SeekOrigin.Begin);
		return binaryFormatter.Deserialize(memoryStream);
	}

	public static string StringToGuid(string guid, out Guid value)
	{
		return ObjectToGuid(guid, out value);
	}

	public static string ObjectToGuid(object guid, out Guid value)
	{
		value = Guid.Empty;
		try
		{
			value = new Guid(guid.ToString());
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
		return "";
	}

	public static string GetBytes(string data, out byte[] bytes)
	{
		string result = "";
		bytes = null;
		try
		{
			bytes = Encoding.UTF8.GetBytes(data);
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}

	public static string GetString(byte[] bytes, out string data)
	{
		string result = "";
		data = null;
		try
		{
			data = Encoding.UTF8.GetString(bytes);
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}

	public static void CopyTo(Stream src, Stream dest)
	{
		byte[] array = new byte[4096];
		int count;
		while ((count = src.Read(array, 0, array.Length)) != 0)
		{
			dest.Write(array, 0, count);
		}
	}

	public static string Zip(string str, out byte[] zippedData)
	{
		zippedData = null;
		try
		{
			if (str == null || str.Trim() == "")
			{
				return "input string is null or empty";
			}
			return Zip(Encoding.UTF8.GetBytes(str), out zippedData);
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
	}

	public static string Zip(object obj, out byte[] zippedData)
	{
		zippedData = null;
		try
		{
			if (obj == null)
			{
				return "object is null";
			}
			return Zip(ObjectToBytes(obj), out zippedData);
		}
		catch (Exception ex)
		{
			return ex.ToString();
		}
	}

	public static string Zip(byte[] data, out byte[] zippedData)
	{
		string result = "";
		zippedData = null;
		try
		{
			if (data == null)
			{
				return "data is null";
			}
			using MemoryStream src = new MemoryStream(data);
			using MemoryStream memoryStream = new MemoryStream();
			using (GZipStream dest = new GZipStream(memoryStream, CompressionMode.Compress))
			{
				CopyTo(src, dest);
			}
			zippedData = memoryStream.ToArray();
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}

	public static string Unzip(byte[] bytes, out string unzippedData)
	{
		unzippedData = null;
		string text;
		try
		{
			byte[] unzippedBytes = null;
			text = Unzip(bytes, out unzippedBytes);
			if (text.Length > 0)
			{
				return text;
			}
			unzippedData = Encoding.UTF8.GetString(unzippedBytes);
		}
		catch (Exception ex)
		{
			text = ex.ToString();
		}
		return text;
	}

	public static string Unzip(byte[] bytes, out object obj)
	{
		obj = null;
		string text;
		try
		{
			byte[] unzippedBytes = null;
			text = Unzip(bytes, out unzippedBytes);
			if (text.Length > 0)
			{
				return text;
			}
			obj = BytesToObject(unzippedBytes);
		}
		catch (Exception ex)
		{
			text = ex.ToString();
		}
		return text;
	}

	public static string Unzip(byte[] bytes, out byte[] unzippedBytes)
	{
		string result = "";
		unzippedBytes = null;
		try
		{
			using MemoryStream stream = new MemoryStream(bytes);
			using MemoryStream memoryStream = new MemoryStream();
			using (GZipStream src = new GZipStream(stream, CompressionMode.Decompress))
			{
				CopyTo(src, memoryStream);
			}
			unzippedBytes = memoryStream.ToArray();
		}
		catch (Exception ex)
		{
			result = ex.ToString();
		}
		return result;
	}
}
