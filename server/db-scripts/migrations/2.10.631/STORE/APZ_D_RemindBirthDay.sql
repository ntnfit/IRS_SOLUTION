
CALL APZ_EXISTSPROC('APZ_D_RemindBirthDay');

GO

CREATE PROCEDURE APZ_D_RemindBirthDay
AS
BEGIN
	DECLARE listSendUser nvarchar(5000) = '';
	declare listSendUserDistinct nvarchar(5000) = '';
	declare dateSendNotification date;
	
	declare tblCardCode table
	(
		"CardCode" nvarchar(50),
		"userSent" nvarchar(15),
		"SuperVisor" int,
		"superVisorSent" nvarchar(15),
		"salesAdmin" nvarchar(5000),
		"CEO" nvarchar(50),
		"RouteCode" nvarchar(100)
	);
	declare tblBirthDay table
	(
		"CardCode" nvarchar(50),
		"Name" nvarchar(100),
		"BirthDate" datetime,
		"RouteCode" nvarchar(100),
		"CardName" nvarchar(100),
		"Address" nvarchar(254),
		"userSent" nvarchar(1000),
		"CntctCode" nvarchar(100)
	);
	
	dateSendNotification := add_days(to_date('2023-06-20'),3);
	
	insert into :tblCardCode select distinct(c."CardCode"), e.USER_CODE, b."SuperVisor", case when g.USER_CODE = e.USER_CODE then '' else g.USER_CODE end,
	(select string_agg(y.USER_CODE, ',') from OHEM x left join OUSR y on x."userId" = y.USERID where CompareSplit(x."empID", b."SalesAdmin", ';') = 1),
	(select string_agg(z.USER_CODE, ',') from APZ_D_STR1 x left join OHEM y on x."SuperVisor"=y."empID" left join OUSR z on z."USERID"=y."userId" where x."RouteCode"=a."RouteCode" and x."Id"=b."FatherId" and x."SuperVisor" is not null), a."RouteCode"
	from APZ_D_STR2 c 
	left join APZ_D_OSTR a on a."RouteCode" = c."RouteCode" and a."Status" = 'Y'
	left join APZ_D_STR1 b on b."RouteCode" = a."RouteCode" and c."Id" =  b."Id" and b."SuperVisor" is not null and b."Status"='Y'
	left join OHEM d on c."EmpId" = d."empID"
	left join OUSR e on d."userId" = e.USERID
	left join OHEM f on b."SuperVisor" = f."empID"
	left join OUSR g on f."userId" = g.USERID;
	

	
	insert into :tblBirthDay 
		select b."CardCode", b."Name", b."BirthDate", d."RouteCode",a."CardName",a."Address",
				ifnull(d."userSent",c.USER_CODE) || IFNULL(',' || d."superVisorSent",'') || IFNULL(',' || d."salesAdmin",'') || IFNULL(',' || d."CEO",''), --(case when ifnull(d."superVisorSent",'') = '' then '' else (',' || ifnull(d."superVisorSent",'')) end) ||  ifnull(d."salesAdmin",'')
				cast(b."CntctCode" as nvarchar(100))
			from OCPR b join OCRD a on a."CardCode" = b."CardCode" --and a."CntctPrsn" = b."Name" 
				join OUSR c on a."UserSign" = c.USERID
				left join :tblCardCode d on b."CardCode" = d."CardCode"
			where month(b."BirthDate") = month(:dateSendNotification)
			and dayofmonth(b."BirthDate") = dayofmonth(:dateSendNotification);

	tblAgg = SELECT "CntctCode" ,STRING_AGG("userSent",',') AS "Agg" FROM :tblBirthDay GROUP BY "CntctCode";
	tblSplit = MAP_MERGE(:tblAgg, SplitWithCode(:tblAgg."CntctCode",:tblAgg."Agg", ','));
	tblDistinct = select distinct "code", "part" from :tblSplit;
	tblDistinctAgg =  select "code",STRING_AGG("part",',') as "part" from :tblDistinct group by "code";
	tblRowNumber = select ROW_NUMBER() over (partition by t0."CntctCode" order by t0."CntctCode" desc) as "RowNum",t0.* from :tblBirthDay t0;
	tblFirstRowEachContact = select * from :tblRowNumber where "RowNum" = 1;
	
	--select STRING_AGG("userSent",',') into listSendUser from :tblBirthDay;
	--tblSendUser = (select distinct "part" from SplitString(:listSendUser,',') where "part" <> '');
	--select STRING_AGG("part",',') into listSendUserDistinct from :tblSendUser;
	
	INSERT INTO APZ_D_OELN ("EventCode", "Data", "SendUser", "Message", "DataExtra") 
		SELECT 'E005', t2."CardCode", t2."part" as "SendUser"
			,'Ngày ' || TO_VARCHAR(t2."BirthDate", 'DD/MM') || ' sinh nhật ông/bà ' || ifnull(t2."Name",'') || ' đại lý ' || ifnull(t2."CardName",'')
		--, N'Sắp tới sinh nhật khách hàng ' || ifnull("Name",'')
		--|| ' (' ||TO_VARCHAR("BirthDate", 'DD/MM') || ')' || CHAR(13) || CHAR(10)
		--|| N'Cửa Hàng: '|| ifnull("CardName",'') || CHAR(13) || CHAR(10)
		--|| N'Tuyến: '|| ifnull("RouteCode",'') || CHAR(13) || CHAR(10)
		--|| N'Địa chỉ: ' || ifnull("Address",''), 
			,'{"docEntry": "' || ifnull(t2."CardCode",'') || '", "contactCode": "'||ifnull(t2."CntctCode",'') ||'"}'
 		FROM (select t1."CardCode",t0."part",t1."BirthDate",t1."Name",t1."CardName",t1."CntctCode"
 			from :tblDistinctAgg t0 join :tblFirstRowEachContact t1 on t0."code" = t1."CntctCode") t2;
END