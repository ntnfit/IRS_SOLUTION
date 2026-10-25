using Apzon.Commons;
using Apzon.Commons.Constants;
using Apzon.Commons.Enums;
using Apzon.HDBConnection.Interfaces;

namespace Apzon.Services;

public interface IDatabaseService : IDatabaseClient
{
	string GetObjectId(ApzObjectTypes objtype);

	string GetObjectId(string udocode);

	void SetNextAutoKey(ApzObjectTypes objtype);

	string GetObjectNum(ApzObjectTypes objtype, int bplId, int series, int usersign);

	string GetObjectNumBySeries(ApzObjectTypes objtype, int series, int usersign = -1);

	void LockNumbering(bool lockNumbering, ApzObjectTypes objtype);

	void ProcessChangeLog(Enums.TransactionType transType, ApzObjectTypes objtype, LogInstance header, LogInstance line, int headerMaxInsKey, string paramanter, int maxHistory);

	int MaxInstanceKey(string val, LogInstance logInstance);

	string ConvertSqlErrorMessage(string error);
}
