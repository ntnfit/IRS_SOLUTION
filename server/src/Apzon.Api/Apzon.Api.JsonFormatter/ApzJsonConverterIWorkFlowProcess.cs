using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Financials;
using Newtonsoft.Json;

namespace Apzon.Api.JsonFormatter;

public class ApzJsonConverterIWorkFlowProcess : JsonConverter
{
	private bool loadData;

	public override bool CanConvert(Type objectType)
	{
		return true;
	}

	public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
	{
		try
		{
			Type type = null;
			object obj = null;
			ApzObjectTypes objectType2 = ApzObjectTypes.oNone;
			Enums.ContextActionActionTypeEx actionType = Enums.ContextActionActionTypeEx.None;
			Enums.ContextActionMethod method = Enums.ContextActionMethod.AddNew;
			List<IWorkflowProcess> subProcess = null;
			List<IWorkflowProcess> subProcessGroup = null;
			if (reader.TokenType == JsonToken.StartObject)
			{
				reader.Read();
			}
			ApzJsonConverterListWorkFlowProcess apzJsonConverterListWorkFlowProcess = new ApzJsonConverterListWorkFlowProcess();
			while (reader.TokenType == JsonToken.PropertyName)
			{
				if (Function.ToString(reader.Value) == "TypeOfClass")
				{
					reader.Read();
					if (!string.IsNullOrEmpty(Function.ToString(reader.Value)))
					{
						type = Type.GetType(Function.ToString(reader.Value));
					}
					reader.Read();
				}
				else if (Function.ToString(reader.Value) == "Content")
				{
					reader.Read();
					if (type != null)
					{
						obj = new JsonSerializer().Deserialize(reader, type);
					}
					reader.Read();
				}
				else if (Function.ToString(reader.Value) == "ObjectType")
				{
					reader.Read();
					if (!string.IsNullOrEmpty(Function.ToString(reader.Value)))
					{
						objectType2 = (ApzObjectTypes)Enum.ToObject(typeof(ApzObjectTypes), reader.Value);
					}
					reader.Read();
				}
				else if (Function.ToString(reader.Value) == "ActionType")
				{
					reader.Read();
					if (!string.IsNullOrEmpty(Function.ToString(reader.Value)))
					{
						actionType = (Enums.ContextActionActionTypeEx)Enum.ToObject(typeof(Enums.ContextActionActionTypeEx), reader.Value);
					}
					reader.Read();
				}
				else if (Function.ToString(reader.Value) == "Method")
				{
					reader.Read();
					if (!string.IsNullOrEmpty(Function.ToString(reader.Value)))
					{
						method = (Enums.ContextActionMethod)Enum.ToObject(typeof(Enums.ContextActionMethod), reader.Value);
					}
					reader.Read();
				}
				else if (Function.ToString(reader.Value) == "SubProcess")
				{
					reader.Read();
					subProcess = ((reader.TokenType != JsonToken.Null) ? ((List<IWorkflowProcess>)apzJsonConverterListWorkFlowProcess.ReadJson(reader, typeof(IWorkflowProcess), existingValue, serializer)) : null);
					reader.Read();
				}
				else if (Function.ToString(reader.Value) == "SubProcessGroup")
				{
					reader.Read();
					subProcessGroup = ((reader.TokenType != JsonToken.Null) ? ((List<IWorkflowProcess>)apzJsonConverterListWorkFlowProcess.ReadJson(reader, typeof(IWorkflowProcess), existingValue, serializer)) : null);
					reader.Read();
				}
			}
			if (type == typeof(DocumentsImpl))
			{
				return new WorkflowProcess<DocumentsImpl>
				{
					Content = (DocumentsImpl)obj,
					ObjectType = objectType2,
					SubProcess = subProcess,
					SubProcessGroup = subProcessGroup,
					ActionType = actionType,
					Method = method
				};
			}
			if (type == typeof(JournalEntriesImpl))
			{
				return new WorkflowProcess<JournalEntriesImpl>
				{
					Content = (JournalEntriesImpl)obj,
					ObjectType = objectType2,
					SubProcess = subProcess,
					SubProcessGroup = subProcessGroup,
					ActionType = actionType,
					Method = method
				};
			}
			if (type == typeof(DataSet))
			{
				return new WorkflowProcess<DataSet>
				{
					Content = (DataSet)obj,
					ObjectType = objectType2,
					SubProcess = subProcess,
					SubProcessGroup = subProcessGroup,
					ActionType = actionType,
					Method = method
				};
			}
			return new WorkflowProcess<object>
			{
				Content = obj,
				ObjectType = objectType2,
				SubProcess = subProcess,
				SubProcessGroup = subProcessGroup,
				ActionType = actionType,
				Method = method
			};
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
			return null;
		}
	}

	public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
	{
		try
		{
			string json = JsonConvert.SerializeObject(value);
			writer.WriteRaw(json);
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", new StackTrace(new StackFrame(0)).ToString().Substring(5, new StackTrace(new StackFrame(0)).ToString().Length - 5), ex.Message);
		}
	}
}
