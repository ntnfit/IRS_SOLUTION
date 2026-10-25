using System.Data;

namespace Apzon.DataServices.Interfaces;

public interface IUserDefinedTexts
{
	string Save(string sessionId, DataTable dtSave);

	string Remove(string sessionId, int entry);
}
