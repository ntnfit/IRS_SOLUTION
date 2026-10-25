using Apzon.DataServices.Implements;
using Apzon.Services;

namespace Apzon.DataServices.HanaImplements;

public class HanaMessagesRepository : MessagesRepository
{
	public HanaMessagesRepository(Apzon.Services.IUnitOfWork serviceUnitOfWork, IDataService dataService)
		: base(serviceUnitOfWork, dataService)
	{
	}

	public override string GetMessage(string subject, string content, string firstColumn, string firstLine, string objType, string objKey, string repName)
	{
		return $"{{\r\n                    \"MessageDataColumns\": [\r\n                        {{\r\n                            \"ColumnName\": \"Request for Document Approval\",\r\n                            \"Link\": \"tYES\",\r\n                            \"MessageDataLines\": [\r\n                                {{\r\n                                    \"Object\": \"{objType}\",\r\n                                    \"ObjectKey\": \"{objKey}\",\r\n                                    \"Value\": \"{firstLine}\"\r\n                                }}\r\n                            ]\r\n                        }}\r\n                    ],\r\n                    \"RecipientCollection\": [\r\n                        {{\r\n                            \"SendInternal\": \"tYES\",\r\n                            \"UserCode\": \"{repName}\"\r\n                        }}\r\n                    ],\r\n                    \"Subject\": \"{subject}\",\r\n                    \"Text\": \"{content}\"\r\n                }}";
	}
}
