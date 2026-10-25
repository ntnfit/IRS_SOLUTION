
CREATE TABLE APZ_D_OSTR
(
	"RouteCode" nvarchar(50) primary key not null,
	"RouteName" nvarchar(254) null,		
	"CountryCode" nvarchar(50) null,
	"Remarks" nvarchar(254) null,
	"Status" varchar(1) null,
	"ChannelCode" varchar(50) null,
	"UserSign" smallint,	
	"CreateDate" date null,
	"CreateTime" int null,
	"UpdateDate" date null,
	"UpdateTime" int,
	"PriceList" int,
	"PriceListDistributor" int,
	"LogInstance" int,
	"UserSign2" int,
	"Recurring" char(1),
	"RecurNum" int,
	"NumberOfVisits" int

)