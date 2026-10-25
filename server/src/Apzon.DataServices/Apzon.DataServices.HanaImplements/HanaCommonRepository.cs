using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Apzon.Commons;
using Apzon.Commons.Enums;
using Apzon.Commons.Helper;
using Apzon.DataServices.Implements;
using Apzon.Entities.Models.Common;
using Apzon.Entities.Models.Documents;
using Apzon.Entities.Models.Financials;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaCommonRepository : CommonRepository
{
	public HanaCommonRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IUnitOfWork unitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, unitOfWork, dataService)
	{
	}

	protected override string GetQueryExistsTable(string table)
	{
		return "DECLARE @Exists smallint = (SELECT top 1 1 from tables where table_name=@Table and schema_name = current_schema);\r\n                     select @Exists from dummy;";
	}

	public override HttpResult HandleWorkflow(List<Workflow> processGroup, string sessionId)
	{
		try
		{
			Logging.Write("WATCH", "Start Handle Workflow");
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			foreach (Workflow item in processGroup)
			{
				switch (item.ActionType)
				{
				case Enums.ContextActionActionTypeEx.MaketingDocument:
				{
					DocumentsImpl document = item.Document;
					string text2 = "";
					if (item.Method == Enums.ContextActionMethod.Update)
					{
						string text3 = string.Join(",", document.Lines.Select((Document_LinesImpl t) => t.LineNum));
						List<string> tableName = Function.GetTableName(item.ObjectType);
						DataTable dataTable3 = ExecuteDataTable("SELECT \"LineNum\",\"ItemCode\" FROM " + tableName[1] + " WHERE \"DocEntry\"=@DocEntry AND \"LineNum\" NOT IN (" + text3 + ");", CommandType.Text, new SqlParameter("@DocEntry", document.DocEntry));
						if (dataTable3.IsNotNull())
						{
							for (int num2 = 0; num2 < dataTable3.Rows.Count; num2++)
							{
								Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
								document_LinesImpl.LineNum = Function.ParseInt(dataTable3.Rows[num2]["LineNum"]);
								document_LinesImpl.ItemCode = Function.ToString(dataTable3.Rows[num2]["ItemCode"]);
								document.Lines.Add(document_LinesImpl);
							}
							document.Lines = document.Lines.OrderBy((Document_LinesImpl t) => t.LineNum).ToList();
						}
						text2 = _unitOfWork.Documents.Mapping(document, EnumCommandType.Update, item.ObjectType, item.ObjectType, ignoreUpdateEmptyValue: true);
					}
					else
					{
						text2 = _unitOfWork.Documents.Mapping(document, EnumCommandType.Add, item.ObjectType, item.ObjectType);
					}
					if (!string.IsNullOrEmpty(text2))
					{
						Logging.Write("TRACE", "cta-workflow-xmldocument: " + text2);
						list.Add(new BodyRequestObject
						{
							ObjectType = item.ObjectType,
							Key = document.DocEntry,
							OutObjectKey = true,
							Action = ((item.Method != Enums.ContextActionMethod.Update) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
							BodyObject = text2
						});
					}
					break;
				}
				case Enums.ContextActionActionTypeEx.UserDefinedObject:
					if (item.ObjectType == ApzObjectTypes.oUserDefinedObject)
					{
						DataSet dataSet = item.DataSet;
						DataTable dataTable = dataSet.Tables[0];
						DataTable dataTable2 = dataSet.Tables[1];
						dataSet.Tables.RemoveAt(0);
						dataSet.Tables.RemoveAt(0);
						string text = _unitOfWork.UDOs.MappingXml(dataTable, dataTable2, dataSet, sessionId, "Add");
						if (!string.IsNullOrEmpty(text))
						{
							Logging.Write("TRACE", "cta-workflow-xmludo: " + text);
							int num = Function.ParseInt(dataTable2.Rows[0]["DocEntry"]);
							list.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oNone,
								OutObjectKey = true,
								Action = ((num <= 0) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
								BodyObject = text,
								UdoCode = Function.ToString(dataTable.Rows[0]["Code"]),
								Key = ((num <= 0) ? null : (Function.ToString(dataTable.Rows[0]["TYPE"]).Equals("1") ? Function.ToString(dataTable2.Rows[0]["Code"]) : ((object)Function.ParseInt(num))))
							});
						}
					}
					break;
				}
			}
			Logging.Write("WATCH", "End - Handle Workflow");
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("WATCH", "[ERROR]End - Handle Workflow");
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult HandleWorkflowProcess(List<IWorkflowProcess> processGroup, string sessionId)
	{
		try
		{
			Logging.Write("WATCH", "Start Handle Workflow");
			List<BodyRequestObject> list = new List<BodyRequestObject>();
			foreach (IWorkflowProcess item in processGroup)
			{
				Type typeOfClass = item.TypeOfClass;
				if (typeOfClass == typeof(DocumentsImpl))
				{
					WorkflowProcess<DocumentsImpl> workflowProcess = (WorkflowProcess<DocumentsImpl>)item;
					DocumentsImpl content = workflowProcess.Content;
					string text = "";
					switch (workflowProcess.Method)
					{
					case Enums.ContextActionMethod.SaveAsDraft:
						if (workflowProcess.ObjectType == ApzObjectTypes.oStockTransfer || workflowProcess.ObjectType == ApzObjectTypes.oInventoryTransferRequest)
						{
							text = _unitOfWork.StockTransfer.MappingXml(workflowProcess.ObjectType, content, isDraft: true, (content.DocEntry <= 0) ? EnumCommandType.Add : EnumCommandType.Update);
							workflowProcess.ObjectType = ApzObjectTypes.oStockTransferDraft;
						}
						else
						{
							text = _unitOfWork.Documents.Mapping(content, (content.DocEntry <= 0) ? EnumCommandType.Add : EnumCommandType.Update, ApzObjectTypes.oDrafts, workflowProcess.ObjectType);
							workflowProcess.ObjectType = ApzObjectTypes.oDrafts;
						}
						break;
					case Enums.ContextActionMethod.Update:
					{
						string text2 = string.Join(",", content.Lines.Select((Document_LinesImpl t) => t.LineNum));
						List<string> tableName = Function.GetTableName(workflowProcess.ObjectType);
						DataTable dataTable = ExecuteDataTable("SELECT \"LineNum\",\"ItemCode\" FROM " + tableName[1] + " WHERE \"DocEntry\"=@DocEntry AND \"LineNum\" NOT IN (" + text2 + ");", CommandType.Text, new SqlParameter("@DocEntry", content.DocEntry));
						if (dataTable.IsNotNull())
						{
							for (int num = 0; num < dataTable.Rows.Count; num++)
							{
								Document_LinesImpl document_LinesImpl = new Document_LinesImpl();
								document_LinesImpl.LineNum = Function.ParseInt(dataTable.Rows[num]["LineNum"]);
								document_LinesImpl.ItemCode = Function.ToString(dataTable.Rows[num]["ItemCode"]);
								content.Lines.Add(document_LinesImpl);
							}
							content.Lines = content.Lines.OrderBy((Document_LinesImpl t) => t.LineNum).ToList();
						}
						text = ((workflowProcess.ObjectType != ApzObjectTypes.oStockTransfer && workflowProcess.ObjectType != ApzObjectTypes.oInventoryTransferRequest) ? _unitOfWork.Documents.Mapping(content, EnumCommandType.Update, workflowProcess.ObjectType, workflowProcess.ObjectType, ignoreUpdateEmptyValue: true) : _unitOfWork.StockTransfer.MappingXml(workflowProcess.ObjectType, content, isDraft: false, EnumCommandType.Update));
						break;
					}
					default:
						text = ((workflowProcess.ObjectType != ApzObjectTypes.oStockTransfer && workflowProcess.ObjectType != ApzObjectTypes.oInventoryTransferRequest) ? _unitOfWork.Documents.Mapping(content, EnumCommandType.Add, workflowProcess.ObjectType, workflowProcess.ObjectType) : _unitOfWork.StockTransfer.MappingXml(workflowProcess.ObjectType, content, isDraft: false));
						break;
					}
					if (!string.IsNullOrEmpty(text))
					{
						Logging.Write("TRACE", "cta-workflow-xmldocument: " + text);
						list.Add(new BodyRequestObject
						{
							ObjectType = workflowProcess.ObjectType,
							Key = content.DocEntry,
							OutObjectKey = true,
							Action = ((workflowProcess.Method != Enums.ContextActionMethod.Update) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
							BodyObject = text
						});
					}
				}
				if (typeOfClass == typeof(JournalEntriesImpl))
				{
					WorkflowProcess<JournalEntriesImpl> workflowProcess2 = (WorkflowProcess<JournalEntriesImpl>)item;
					JournalEntriesImpl content2 = workflowProcess2.Content;
					string text3 = "";
					text3 = ((workflowProcess2.Method != Enums.ContextActionMethod.Update) ? _unitOfWork.JournalEntries.Mapping(content2, EnumCommandType.Add) : _unitOfWork.JournalEntries.Mapping(content2, EnumCommandType.Update));
					if (!string.IsNullOrEmpty(text3))
					{
						Logging.Write("TRACE", "cta-workflow-xml journalEntry: " + text3);
						list.Add(new BodyRequestObject
						{
							Action = ((workflowProcess2.Method != Enums.ContextActionMethod.Update) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
							BodyObject = text3,
							ObjectType = ApzObjectTypes.oJournalEntries,
							Key = ((workflowProcess2.Method == Enums.ContextActionMethod.Update) ? ((object)content2.TransId) : null),
							OutObjectKey = true
						});
					}
				}
				else
				{
					if (!(typeOfClass == typeof(DataSet)))
					{
						continue;
					}
					WorkflowProcess<DataSet> workflowProcess3 = (WorkflowProcess<DataSet>)item;
					if (workflowProcess3.ObjectType == ApzObjectTypes.oUserDefinedObject)
					{
						DataSet content3 = workflowProcess3.Content;
						DataTable dataTable2 = content3.Tables[0];
						DataTable dataTable3 = content3.Tables[1];
						content3.Tables.RemoveAt(0);
						content3.Tables.RemoveAt(0);
						string text4 = _unitOfWork.UDOs.MappingXml(dataTable2, dataTable3, content3, sessionId, "Add");
						if (!string.IsNullOrEmpty(text4))
						{
							Logging.Write("TRACE", "cta-workflow-xmludo: " + text4);
							int num2 = Function.ParseInt(dataTable3.Rows[0]["DocEntry"]);
							list.Add(new BodyRequestObject
							{
								ObjectType = ApzObjectTypes.oNone,
								OutObjectKey = true,
								Action = ((num2 <= 0) ? Enums.HttpAction.AddNew : Enums.HttpAction.Update),
								BodyObject = text4,
								UdoCode = Function.ToString(dataTable2.Rows[0]["Code"]),
								Key = ((num2 <= 0) ? null : (Function.ToString(dataTable2.Rows[0]["TYPE"]).Equals("1") ? Function.ToString(dataTable3.Rows[0]["Code"]) : ((object)Function.ParseInt(num2))))
							});
						}
					}
				}
			}
			Logging.Write("WATCH", "End - Handle Workflow");
			return DataService.ProcessMutiTransaction(sessionId, list);
		}
		catch (Exception ex)
		{
			Logging.Write("WATCH", "[ERROR]End - Handle Workflow");
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult DoQuery(List<ContextActionDoQuery> doquery, string sessionId, string userName)
	{
		try
		{
			List<string> list = new List<string>();
			foreach (ContextActionDoQuery item in doquery)
			{
				string text = item.QueryText;
				foreach (QueryParamater paramater in item.Paramaters)
				{
					text = text.Replace(paramater.ParaName, $"'{paramater.Value}'");
				}
				if (!string.IsNullOrEmpty(text))
				{
					list.Add(text);
				}
			}
			string text2 = string.Join("\r\n", list.ToArray());
			if (!string.IsNullOrEmpty(text2))
			{
				return ServiceUnitOfWork.Commons.DoQuery(text2, userName);
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}

	public override HttpResult DoQueryWithTempTable(ContextActionDoQuery doquery, string sessionId, string userName)
	{
		try
		{
			if (!string.IsNullOrEmpty(doquery.QueryText))
			{
				return ServiceUnitOfWork.Commons.DoQueryWithTempTable(doquery.QueryText, doquery.TempTableName, doquery.TempTableData, userName);
			}
			return new HttpResult();
		}
		catch (Exception ex)
		{
			Logging.Write("ERROR", ex);
			return new HttpResult(Enums.MessageCode.ExceptionMessage, ex.Message);
		}
	}
}
