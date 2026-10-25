
Drop Function APZ_D_CalcDistance;

CREATE FUNCTION APZ_D_CalcDistance
(
	IN cardCode nvarchar(100),
	IN lat1 float,
	IN lat2 float, 
	IN lon1 float,
	IN lon2 float
)
returns table (
	"CardCode" nvarchar(100),
	"Result" float
)
AS
BEGIN
	declare piNumber float = 3.14159265358979;
	declare result table 
	(
		"CardCode" nvarchar(100),
		"Result" float
	);
	insert into :result("CardCode","Result")
	select 
	:cardCode as "CardCode",
	ACOS(SIN(:piNumber*:lat1/180.0)
	*SIN(:piNumber*:lat2/180.0)
	+COS(:piNumber*:lat1/180.0)
	*COS(:piNumber*:lat2/180.0)
	*COS(:piNumber*:lon2/180.0-:piNumber*:lon1/180.0))*6371
	as "Result"
	from dummy;
	return :result;
END;