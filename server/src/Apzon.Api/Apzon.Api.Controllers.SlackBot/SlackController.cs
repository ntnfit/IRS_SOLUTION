using System.Data;
using System.Web.Http;
using Apzon.Api.Controllers.Sales;
using Apzon.Commons;

namespace Apzon.Api.Controllers.SlackBot;

public class SlackController : BaseApiController
{
	[HttpGet]
	public string SendMessage(string user, string message)
	{
		DataTable service = UnitOfWork.ServiceCallSetting.GetService(user);
		int num = 0;
		string user2 = "bot";
		string subject = "";
		if (service != null && service.Rows.Count > 0)
		{
			num = Function.ParseInt(service.Rows[0]["ServiceID"]);
			user2 = Function.ToString(service.Rows[0]["Name"]);
			subject = Function.ToString(service.Rows[0]["Email"]);
		}
		switch (num)
		{
		case 0:
			new SlackClient().send(message, user2);
			break;
		case 2:
			new MailClient().SendEmail(subject, message);
			break;
		case 1:
			new WhatsappClient().send("thanh", "hello");
			break;
		}
		return "";
	}
}
