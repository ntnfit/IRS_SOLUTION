
CREATE  PROCEDURE APZ_DocumentNumberSeries_GetCodeByObject(
in ObjectCode nvarchar(30) default '',
in SeriesName nvarchar(30) default '',
in BPLId nvarchar(30) default '')
as
BEGIN
	
	DECLARE MultiBranch NVARCHAR(1);
	select IFNULL((SELECT TOP 1 "MltpBrnchs" FROM OADM),'N') into MultiBranch from DUMMY;
	
	select case when "IsDigSerie" = 'Y' then cast("NextNumber" as nvarchar) else  ifnull("BeginStr",'') || RIGHT(right(cast(power(10,"NumSize") as nvarchar),"NumSize") || 
			cast(ifnull("NextNumber",0) as nvarchar),ifnull("NumSize",0)) end
	as "ReturnStr"
	 from nnm1 where "ObjectCode"=:ObjectCode and "IsManual"='N' 
	 and (:MultiBranch='N' OR (:MultiBranch='Y' and ifnull("BPLId", :BPLId)=:BPLId))
	 and "SeriesName"=:SeriesName;
END




