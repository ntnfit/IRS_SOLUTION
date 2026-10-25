
CREATE TABLE APZ_OHWP
(
	"PYear" int NULL,
	"PeriodCode" nvarchar(50) NOT NULL,
	"PeriodName" nvarchar(254) NULL,
	"SubPeriod" char(1) NULL,
	"NumOfPeriod" int NULL,
	"PeriodStatus" char(1) NULL,
	"FromDate" timestamp NULL,
	"ToDate" timestamp NULL,
	"Branch" int NOT NULL,
	"WorkQuota" decimal(19, 6) NULL,
	"HourPerDay" decimal(19, 6) NULL,
	"TimeKeep" char(1) NULL,
	"NumOfWorkDf" decimal(19, 6) NULL,
	"FPLTotal" decimal(19, 6) NULL,
	"IsLastPeriod" char(1) NULL,
	"LogInstanc" int NULL,
	"IncreaseLeaveTotal" decimal(19, 6) NULL,
	"UserSign" int NULL,
	"UserSign2" int NULL,
	"UpdateDate" timestamp NULL,
	"ID" int NULL,
	PRIMARY KEY ("PeriodCode","Branch")
);
