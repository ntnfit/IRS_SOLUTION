
CREATE TABLE APZ_D_OOVF 
(
	"Code" NVARCHAR(50) NOT NULL
	, "Name" NVARCHAR(254) NOT NULL
	, "Address" nvarchar(5000)
	, "Description" NVARCHAR(254)
	, "StartDate" timestamp
	, "Finished" CHAR(1) DEFAULT 'N'	-- Y/N Yes/No
	, "Recurring" CHAR(1) DEFAULT 'W'	-- 	W – Weekly/	M – Monthly
	, "RecurNum" INT DEFAULT 1			--	Number of Weeks/Months
	, "RecurNote" NVARCHAR(254)
	, "EveryDay" CHAR(1) DEFAULT 'N'	-- Y/N Yes/No
	, "DayOfWeek" CHAR(1) DEFAULT 'N'	-- Y/N Yes/No
	, "Mon" CHAR(1) DEFAULT 'N'
	, "Tue" CHAR(1) DEFAULT 'N'
	, "Wed" CHAR(1) DEFAULT 'N'
	, "Thu" CHAR(1) DEFAULT 'N'
	, "Fri" CHAR(1) DEFAULT 'N'
	, "Sat" CHAR(1) DEFAULT 'N'
	, "Sun" CHAR(1) DEFAULT 'N'
	, "DayOfMonth" CHAR(1) DEFAULT 'N'
	, "ListOfDate" NVARCHAR(1000),
	"UserSign" int NULL,
	"UpdateDate" timestamp NULL,
	"LogInstanc" int NULL,
	"NumberOfVisits" int
);
