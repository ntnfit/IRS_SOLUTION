
CREATE PROCEDURE APZ_SalaryFormula_GetFormulaText(in dept int, in branch int, in methodid int,in formid nvarchar(50))
as 
BEGIN

	declare checkCount int = 0;
	create local temporary table #tbl as (select top 0 * from APZ_SAL4);
	insert into #tbl
		select * from APZ_SAL4 where "FormID" = :formid and "MethodID" = :methodid and "Active"='Y' and "branch" = :branch and "dept" = :dept;
	--Ưu tiên lấy công thức theo department (chỉ áp dụng cho department trong branch đó)
	select ifnull((select top 1 1 from #tbl), 0) into checkCount from DUMMY;
	if :checkCount = 1
	THEN
			select * from #tbl;
	else
		create local temporary table #tbl2 as (select top 0 * from APZ_SAL4);
		
		insert into #tbl2
			select * from APZ_SAL4 where "FormID" = :formid and "MethodID" = :methodid and "Active"='Y' and "branch" = :branch and "dept" = 0;
		-- Nếu không select được công thức theo department, thì sẽ lấy công thức theo branch đó (công thức áp dụng cho toàn bộ branch đó)
		select ifnull((select top 1 1 from #tbl2), 0) into checkCount from DUMMY;
		if :checkCount = 1
		THEN
			select * from #tbl2;
		
		else
		
			select * from APZ_SAL4 where "FormID" = :formid and "MethodID" = :methodid and "Active"='Y' and "branch" = 0 and "dept" = 0;
			-- Cuối cùng là nếu không tồn tại 2 trường hợp trên, thì sẽ lấy công thức chỉ theo formId và methodId (công thức áp dụng cho toàn bộ hệ thống)
		end IF;
	end IF;

END



