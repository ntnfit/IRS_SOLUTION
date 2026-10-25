namespace Apzon.Controls.Commons;

public static class ConstantSqlApz
{
	public static string[] KeyWords = new string[173]
	{
		"add", "all", "alter", "and", "any", "as", "asc", "authorization", "backup", "begin",
		"between", "break", "browse", "bulk", "by", "cascade", "case", "check", "checkpoint", "close",
		"clustered", "coalesce", "collate", "column", "commit", "compute", "constraint", "contains", "containstable", "continue",
		"convert", "create", "cross", "current", "current_date", "current_time", "current_timestamp", "current_user", "cursor", "database",
		"dbcc", "deallocate", "declare", "default", "delete", "deny", "desc", "disk", "distinct", "distributed",
		"double", "drop", "dummy", "dump", "else", "end", "errlvl", "escape", "except", "exec",
		"execute", "exists", "exit", "fetch", "file", "fillfactor", "for", "foreign", "freetext", "freetexttable",
		"from", "full", "function", "goto", "grant", "group", "having", "holdlock", "identity", "identity_insert",
		"identitycol", "if", "in", "index", "inner", "insert", "intersect", "into", "is", "join",
		"key", "kill", "left", "like", "lineno", "load", "national", "nocheck", "nonclustered", "not",
		"nullif", "of", "off", "offsets", "on", "open", "opendatasource", "openquery", "openrowset", "openxml",
		"option", "or", "order", "outer", "over", "percent", "plan", "precision", "primary", "print",
		"proc", "procedure", "public", "raiserror", "read", "readtext", "reconfigure", "references", "replication", "restore",
		"restrict", "return", "revoke", "right", "rollback", "rowcount", "rowguidcol", "rule", "save", "schema",
		"select", "session_user", "set", "setuser", "shutdown", "some", "statistics", "system_user", "table", "textsize",
		"then", "to", "top", "tran", "transaction", "trigger", "truncate", "tsequal", "union", "unique",
		"update", "updatetext", "use", "user", "values", "varying", "view", "waitfor", "when", "where",
		"while", "with", "writetext"
	};

	public static string[] ComparisonOperators = new string[11]
	{
		"=", "<=", ">=", "!=", "!>", "!<", "<>", ">", "<", "in",
		"not in"
	};

	public static string[] JoinOperator = new string[5] { "cross", "outer", "full", "right", "left" };
}
