using Apzon.Entities.Models.Common;

namespace Apzon.DataServices.Interfaces;

public interface IMessage
{
	MessageHeaderImpl SendMessage(string sessionId, string msg);

	string GetMessage(string subject, string content, string firstColumn, string firstLine, string objType, string objKey, string repName);
}
