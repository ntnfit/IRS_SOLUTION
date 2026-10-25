

CREATE PROCEDURE apz_saveSql(
	in sqlstring nclob,
	in queryName nvarchar(5000),
	in category nvarchar(5000))
AS
BEGIN
 declare checkCount int = 0;
 
 select ifnull((select top 1 1 from OQCN where "CatName" = :category),0) into checkCount from DUMMY;

if :checkCount = 1
	then
	 
	 	select ifnull((select top 1 1 from OUQR where "QName" = :queryName and "QCategory" = (select "CategoryId" from OQCN where "CatName" = :category)),0) into checkCount from DUMMY;
	 	
		if :checkCount = 1
			then
				Update OUQR set "QString" = :sqlstring where "QName" = :queryName and "QCategory" = (select "CategoryId" from OQCN where "CatName" = :category);
		else
				insert into OUQR ("IntrnalKey", "QCategory", "QName", "QString") values (ifnull((select max("IntrnalKey") from OUQR),0) + 1, 
										(select "CategoryId" from OQCN where "CatName" = :category), :queryName, :sqlstring);
			end if;
else

		INSERT into OQCN ("CategoryId", "CatName", "PermMask") values (ifnull((select max("CategoryId") from OQCN),0) + 1, :category, 'Y');
		
		select ifnull((select top 1 1 from OUQR where "QName" = :queryName and "QCategory" = (select "CategoryId" from OQCN where "CatName" = :category)),0) into checkCount from DUMMY;
		
		if :checkCount = 1
			then
				Update OUQR set "QString" = :sqlstring where "QName" = :queryName and "QCategory" = (select "CategoryId" from OQCN where "CatName" = :category);
		else
				insert into OUQR ("IntrnalKey", "QCategory", "QName", "QString") values (ifnull((select max("IntrnalKey") from OUQR),0) + 1, 
										(select "CategoryId" from OQCN where "CatName" = :category), :queryName, :sqlstring);
			end if;

	end if;

END




