

CREATE FUNCTION getComponentByFGs(listFG nvarchar(4000))
RETURNS table ("Code" nvarchar(50))

AS
BEGIN
	declare tblRESULT table ("Code" nvarchar(50));
	declare tblFather table ("FCode" nvarchar(50));
	declare tblSub table ("SCode" nvarchar(50));
	declare tblRESULT_2	table ("Code" nvarchar(50));
	
	insert into :tblFather ("FCode") select Data from Split(:listFG,',');
	insert into :tblRESULT ("Code") select "FCode" from :tblFather;
	insert into :tblSub ("SCode") select "Code" from ITT1 where "Father" in (select x."FCode" from :tblFather x); -- BOM SAP
	insert into :tblSub ("SCode") select b."Code" from APZ_OITT a, APZ_ITT1 b where a."BomVerID"=b."Father" and a."Code" in (select x."FCode" from :tblFather x); -- BOM Version
	
	while (select count("SCode") from :tblSub) > 0 do
		
		delete from :tblFather;
		insert into :tblFather ("FCode") select "SCode" from :tblSub;

		insert into :tblRESULT ("Code") select "FCode" from :tblFather;
		delete from :tblSub;
		insert into :tblSub ("SCode") select "Code" from ITT1 where "Father" in (select x."FCode" from :tblFather x); -- BOM SAP
		insert into :tblSub ("SCode") select b."Code" from APZ_OITT a, APZ_ITT1 b where a."BomVerID"=b."Father" and a."Code" in (select x."FCode" from :tblFather x); -- BOM Version
	end while;

	-- Bổ sung các mã cha
	
	delete from :tblSub;
	insert into :tblSub ("SCode") select Data from Split(:listFG,',');
	delete from :tblFather;
	insert into :tblFather ("FCode") select "Father" from ITT1 where "Code" in (select "SCode" from :tblSub); -- BOM SAP
	insert into :tblFather ("FCode") select a."Code" from APZ_OITT a, APZ_ITT1 b where a."BomVerID"=b."Father" and b."Code" in (select "SCode" from :tblSub); -- BOM Version
	
	while (select count("FCode") from :tblFather)>0 do
	
		insert into :tblRESULT ("Code") select "FCode" from :tblFather;
		delete from :tblSub;
		insert into :tblSub("SCode") select "FCode" from :tblFather;
		delete from :tblFather;
		insert into :tblFather ("FCode") select "Father" from ITT1 where "Code" in (select "SCode" from :tblSub); -- BOM SAP
		insert into :tblFather ("FCode") select a."Code" from APZ_OITT a, APZ_ITT1 b where a."BomVerID"=b."Father" and b."Code" in (select "SCode" from :tblSub); -- BOM Version
	end while;

	return select "Code" from :tblRESULT group by "Code";

END;




